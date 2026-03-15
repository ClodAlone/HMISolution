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
    using System.Windows.Data;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
	using System.Windows.Resources;
    using System.IO;
    using System.Windows.Markup;
    using System.Windows.Threading;
    using System.Text;
#if !SILVERLIGHT
    using Microsoft.Win32;
#endif

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  create and maintains all the appointments and events
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(true)]
#endif
   
#if SILVERLIGHT
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(Schedule), XamlResource = "/Syncfusion.Theming.Blend;component/ScheduleControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
            Type = typeof(Schedule), XamlResource = "/Syncfusion.Theming.Office2007Black;component/ScheduleControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
            Type = typeof(Schedule), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/ScheduleControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
            Type = typeof(Schedule), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/ScheduleControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
            Type = typeof(Schedule), XamlResource = "/Syncfusion.Theming.Office2003;component/ScheduleControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
            Type = typeof(Schedule), XamlResource = "/Syncfusion.Theming.Default;component/ScheduleControl.xaml")] 
#endif
    [StyleTypedProperty(Property = "GoToDateWindowStyle", StyleTargetType = typeof(GoToDateWindow))]
    public class Schedule : Control, IDisposable
    {
        internal Popup p;
        DispatcherTimer ReminderTimer;
        /// <summary>
        /// Initializes a new instance of the <see cref="Schedule"/> class.
        /// </summary>
        public Schedule()
        {
            this.DefaultStyleKey = typeof(Schedule);
            p = new Popup();
            ReminderTimer = new DispatcherTimer();
            this.AppointmentStatusCollection = new ScheduleAppointmentStatusCollection() { new ScheduleAppointmentStatus() { Brush = new SolidColorBrush(Colors.White), Status = "Free" }, new ScheduleAppointmentStatus() { Brush = new SolidColorBrush(Colors.Purple), Status = "Tentative" }, new ScheduleAppointmentStatus() { Brush = new SolidColorBrush(Color.FromArgb(0XFF, 0X90, 0X90, 0xF0)), Status = "Busy" }, new ScheduleAppointmentStatus() { Brush = new SolidColorBrush(Color.FromArgb(0XFF, 0X60, 0X00, 0x60)), Status = "Out Of Office" } };
            this.Model.Appointments.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Appointments_CollectionChanged);
            ReminderTimer.Tick += new EventHandler(ReminderTimer_Tick);
            this.SetUpDefaultColors();
            this.AppointmentStatusCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(AppointmentStatusCollection_CollectionChanged);
            this.KeyDown += new KeyEventHandler(Schedule_KeyDown);
            this.ContextMenuTimeSlotItems = new ObservableCollection<object>();
            this.ContextMenuTimeLineItems = new ObservableCollection<object>();
            this.ContextMenuMonthViewItems = new ObservableCollection<object>();
            this.ContextMenuMonthViewAppointmentItems = new ObservableCollection<object>();
            this.ContextMenuHorizontalViewTimeLineItems = new ObservableCollection<object>();
            this.ContextMenuHorizontalViewItems = new ObservableCollection<object>();
            this.ContextMenuHorizontalViewAppointmentItems = new ObservableCollection<object>();
            this.ContextMenuDaysHeaderItems = new ObservableCollection<object>();
            this.ContextMenuAppointmentItems = new ObservableCollection<object>();
            this.ContextMenuAllDayAppointmentItems = new ObservableCollection<object>();
            
        }

        void AppointmentStatusCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Model.AppointmentStatusCollection = this.AppointmentStatusCollection;
        }

        private void SetUpDefaultColors()
        {
            this.Foreground = new SolidColorBrush(Colors.Black);
            this.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            this.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xE6, 0xED, 0xF7));
            this.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x4C, 0x53, 0x5C));
            this.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xCE, 0xDB, 0xEF));
            this.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xA5, 0xBF, 0xE1));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint=new Point(0.5,0);
            appBackground.EndPoint=new Point(0.5,1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 228, 229, 229);
            gs1.Offset = 0.095;
            gs2.Color = Color.FromArgb(255, 199, 204, 210);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            this.AppointmentBackground = appBackground;

#if SILVERLIGHT
            this.StrokeThickness = new Thickness(0.5);
#else
            this.StrokeThickness = new Thickness(0.3);
#endif
            //this.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xA5, 0xBF, 0xE1));
            //this.BorderThickness = new Thickness(0.5);
        }

        private void Schedule_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.Model != null && e.Key == Key.Delete)
            {
                if (this.model.AllowDelete == true)
                {
                    this.Model.DeleteCurrentSelectedAppointment();
                    this.DeleteCurrentSelectedAppointment();
                }
            }
        }
#if SILVERLIGHT

        static Schedule()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
            }
        }
#endif

        #region Theme Set Office14Silver / Blend Theme
        /// <summary>
        /// Gets / Sets the Theme for the Schedule Style.
        /// </summary>
        public VisualStyle VisualStyle
        {
            get { return (VisualStyle)GetValue(VisualThemeProperty); }
            set { SetValue(VisualThemeProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for VisualThemeProperty.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisualThemeProperty =
            DependencyProperty.Register("VisualStyle", typeof(VisualStyle), typeof(Schedule), new PropertyMetadata(VisualStyle.Office14Silver, OnVisualThemePropertyChanged));

        private static void OnVisualThemePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Schedule scheduleThemer = sender as Schedule;
            scheduleThemer.SetUpDefaultColors();                 
            scheduleThemer.ApplyVisualTheme();
            scheduleThemer.UpdateLayout();
            if(scheduleThemer.mainViewItems != null)
                scheduleThemer.UpdateMainViewItems();
        }

        private void ApplyVisualTheme()
        {
            switch (this.VisualStyle)
            {
                case VisualStyle.Office14Silver:                    
                    this.Style = null;                   
                    break;
                case VisualStyle.Blend:
                    SetBlendTheme();                    
                    break;
                case VisualStyle.Office14Blue:
                    SetOfficeBlueTheme();                    
                    break;
                case VisualStyle.Office14Black:
                    SetOfficeBlackTheme();                    
                    break;
                case VisualStyle.VS2010:
                    SetVS2010Theme();
                    break;
                case VisualStyle.Metro:
                    SetMetroTheme();
                    break;
            }
        }
      

        private void SetBlendTheme()
        {            
            this.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x52, 0x55, 0x5A));
            this.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x00));
            this.SelectionBackground = new SolidColorBrush(Colors.Gray);
            this.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xDE, 0xDE, 0xDE));
            this.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0x90, 0x90, 0x90));
            this.StrokeThickness = new Thickness(0.5);
            this.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0XFF, 0XFF, 0xFF));
            LinearGradientBrush appBackground = new LinearGradientBrush();
            appBackground.StartPoint = new Point(0.5, 0);
            appBackground.EndPoint = new Point(0.5, 1);
            GradientStop gs1 = new GradientStop();
            GradientStop gs2 = new GradientStop();
            gs1.Color = Color.FromArgb(255, 228, 229, 229);
            gs1.Offset = 0.095;
            gs2.Color = Color.FromArgb(255, 199, 204, 210);
            gs2.Offset = 1;
            appBackground.GradientStops.Add(gs1);
            appBackground.GradientStops.Add(gs2);
            this.AppointmentBackground = appBackground;
#if SILVERLIGHT
            StreamResourceInfo StreamResourceInfoObj = Application.GetResourceStream(new Uri("/Syncfusion.Schedule.Silverlight;component/Themes/BlendTheme.xaml", UriKind.RelativeOrAbsolute));

            if (StreamResourceInfoObj != null && StreamResourceInfoObj.Stream != null)
            {
                using (StreamReader StreamReaderObj = new StreamReader(StreamResourceInfoObj.Stream))
                {
                    string resourcemerged = StreamReaderObj.ReadToEnd();

                    if (!string.IsNullOrEmpty(resourcemerged))
                    {
                        this.Resources = XamlReader.Load(resourcemerged) as ResourceDictionary;
                    }
                }
            }
#else

            if (!this.Resources.Contains("BlendScheduleStyle"))
            {
                this.Resources.Source = new Uri("/Syncfusion.Schedule.WPF;component/Themes/BlendTheme.xaml", UriKind.RelativeOrAbsolute);
            }
#endif
            if (this.Resources.Contains("BlendScheduleStyle"))
                this.Style = this.Resources["BlendScheduleStyle"] as Style;
        }
        private void SetOfficeBlackTheme()
        {
#if SILVERLIGHT
            StreamResourceInfo StreamResourceInfoObj = Application.GetResourceStream(new Uri("/Syncfusion.Schedule.Silverlight;component/Themes/OfficeBlack.xaml", UriKind.RelativeOrAbsolute));

            if (StreamResourceInfoObj != null && StreamResourceInfoObj.Stream != null)
            {
                using (StreamReader StreamReaderObj = new StreamReader(StreamResourceInfoObj.Stream))
                {
                    string resourcemerged = StreamReaderObj.ReadToEnd();

                    if (!string.IsNullOrEmpty(resourcemerged))
                    {
                        this.Resources = XamlReader.Load(resourcemerged) as ResourceDictionary;
                    }
                }
            }
#else
            this.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xDE, 0xDE, 0xDE));
            if (!this.Resources.Contains("OfficeBlackScheduleStyle"))
            {
                this.Resources.Source = new Uri("/Syncfusion.Schedule.WPF;component/Themes/OfficeBlack.xaml", UriKind.RelativeOrAbsolute);
            }
#endif
            if (this.Resources.Contains("OfficeBlackScheduleStyle"))
                this.Style = this.Resources["OfficeBlackScheduleStyle"] as Style;
        }
        private void SetOfficeBlueTheme()
        {
#if SILVERLIGHT
            StreamResourceInfo StreamResourceInfoObj = Application.GetResourceStream(new Uri("/Syncfusion.Schedule.Silverlight;component/Themes/OfficeBlue.xaml", UriKind.RelativeOrAbsolute));

            if (StreamResourceInfoObj != null && StreamResourceInfoObj.Stream != null)
            {
                using (StreamReader StreamReaderObj = new StreamReader(StreamResourceInfoObj.Stream))
                {
                    string resourcemerged = StreamReaderObj.ReadToEnd();

                    if (!string.IsNullOrEmpty(resourcemerged))
                    {
                        this.Resources = XamlReader.Load(resourcemerged) as ResourceDictionary;
                    }
                }
            }
#else
            
            if (!this.Resources.Contains("OfficeBlueScheduleStyle"))
            {
                this.Resources.Source = new Uri("/Syncfusion.Schedule.WPF;component/Themes/OfficeBlue.xaml", UriKind.RelativeOrAbsolute);
            }
#endif
            if (this.Resources.Contains("OfficeBlueScheduleStyle"))
                this.Style = this.Resources["OfficeBlueScheduleStyle"] as Style;

        }
        private void SetVS2010Theme()
        {
            this.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x46, 0x5A, 0x7D));
            this.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            this.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xEC, 0xB5));
            this.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0));
            this.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0x98, 0x9D, 0xAB));
            this.StrokeThickness = new Thickness(0.5);
            this.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0X00, 0X00, 0x00));
#if SILVERLIGHT
            StreamResourceInfo StreamResourceInfoObj = Application.GetResourceStream(new Uri("/Syncfusion.Schedule.Silverlight;component/Themes/VS2010Style.xaml", UriKind.RelativeOrAbsolute));

            if (StreamResourceInfoObj != null && StreamResourceInfoObj.Stream != null)
            {
                using (StreamReader StreamReaderObj = new StreamReader(StreamResourceInfoObj.Stream))
                {
                    string resourcemerged = StreamReaderObj.ReadToEnd();

                    if (!string.IsNullOrEmpty(resourcemerged))
                    {
                        this.Resources = XamlReader.Load(resourcemerged) as ResourceDictionary;
                    }
                }
            }
#else

            if (!this.Resources.Contains("VS2010ScheduleStyle"))
            {
                this.Resources.Source = new Uri("/Syncfusion.Schedule.WPF;component/Themes/VS2010Style.xaml", UriKind.RelativeOrAbsolute);
            }
#endif
            if (this.Resources.Contains("VS2010ScheduleStyle"))
                this.Style = this.Resources["VS2010ScheduleStyle"] as Style;
        }

        private void SetMetroTheme()
        {
            this.HeaderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x2A, 0xBF, 0xF1));
            this.ScheduleBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            this.SelectionBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0x2A, 0xAB, 0xCF));
            this.ShadedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xEB, 0xEB, 0xEB));
            this.StrokeLine = new SolidColorBrush(Color.FromArgb(0xFF, 0xC5, 0xC5, 0xC5));
            this.StrokeThickness = new Thickness(0.5);
            this.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0X8A, 0X8A, 0x8A));
#if SILVERLIGHT
            StreamResourceInfo StreamResourceInfoObj = Application.GetResourceStream(new Uri("/Syncfusion.Schedule.Silverlight;component/Themes/Metro.xaml", UriKind.RelativeOrAbsolute));

            if (StreamResourceInfoObj != null && StreamResourceInfoObj.Stream != null)
            {
                using (StreamReader StreamReaderObj = new StreamReader(StreamResourceInfoObj.Stream))
                {
                    string resourcemerged = StreamReaderObj.ReadToEnd();

                    if (!string.IsNullOrEmpty(resourcemerged))
                    {
                        this.Resources = XamlReader.Load(resourcemerged) as ResourceDictionary;
                    }
                }
            }
#else
            if (!this.Resources.Contains("MetroScheduleStyle"))
            {
                this.Resources.Source = new Uri("/Syncfusion.Schedule.WPF;component/Themes/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
            }
#endif

            if (this.Resources.Contains("MetroScheduleStyle"))
                this.Style = this.Resources["MetroScheduleStyle"] as Style;

        }
        #endregion

        /// <summary>
        /// Gets or sets Custom palette value for the schedule.
        /// </summary>
        public ColorPaletteModel CustomPalette
        {
            get { return (ColorPaletteModel)GetValue(CustomPaletteProperty); }
            set { SetValue(CustomPaletteProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for CustomPalette.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CustomPaletteProperty =
            DependencyProperty.Register("CustomPalette", typeof(ColorPaletteModel), typeof(Schedule), new PropertyMetadata(OnCustomPaletteChanged));

        private static void OnCustomPaletteChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            Schedule schedule= dpo as Schedule;
            if (schedule != null && schedule.CustomPalette != null)
                if (schedule.Palette == ColorPalette.Custom)
                {
                    schedule.Model.CustomPalette = schedule.CustomPalette;
                    schedule.Model.SetPalette(ColorPalette.Custom);
                }
                else
                {
                    schedule.Model.CustomPalette = schedule.CustomPalette;
                }
        }

        

        #region palette


        /// <summary>
        /// Gets or sets the color palette value for the palette
        /// </summary>
        public ColorPalette Palette
        {
            get { return (ColorPalette)GetValue(PaletteProperty); }
            set
            {
                SetValue(PaletteProperty, value);

            }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for Palette with color palette type
        /// </summary>
        public static readonly DependencyProperty PaletteProperty =
           DependencyProperty.Register("Palette", typeof(ColorPalette), typeof(Schedule), new PropertyMetadata(ColorPalette.ColorButton1, OnPaletteChanged));



        private static void OnPaletteChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            Schedule schedule = dpo as Schedule;

            //schedule.SetUpDefaultColors();
            schedule.Model.SetPalette(schedule.Palette);
            //ScheduleDaysView sdv = schedule.daysView;
            //if (sdv != null)
            //{
            //    sdv.ColorButton1Method(schedule.Palette.ToString());
            //}
        }
        
        
   
        #endregion
        #region Reminder Methods And events

        void ReminderTimer_Tick(object sender, EventArgs e)
        {
            if (!p.IsOpen && this.DisplayReminder )
            {
                ShowReminder();
            }
        }

        /// <summary>
        /// EventHandler Delegate for schedule reminder alert events
        /// </summary>
        public delegate void AlarmEventHandler(Object sender, ScheduleReminderEventArgs e);

        /// <summary>
        /// Occurs when reminder is invoking.
        /// </summary>
        public event AlarmEventHandler ReminderInvoking;

        /// <summary>
        /// Occurs when reminder is invoked.
        /// </summary>
        public event AlarmEventHandler ReminderInvoked;

        void Appointments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.SetAppointmentDatesBold();
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    calculate();
                    this.Model.GenerateAppointmentID(e);
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    calculate();
                    break;
            }
        }

        void calculate()
        {
            if (this.DisplayReminder)
            {               
                bool allDismissed = true;
                foreach (ScheduleAppointment app in this.Model.Appointments)
                {
                    if (app.IsDismissed == false)
                    {
                        allDismissed = false;
                    }
                    ReminderTimer.Stop();
                }
                if (!allDismissed)
                {
                    DateTime reminderTime = GetNearestReminder();
                    if (reminderTime <= DateTime.Now)
                    {
                        ReminderTimer.Interval = new TimeSpan(0, 0, 0,0,2);
                        ReminderTimer.Start();
                    }

                    else
                    {
                        reminderTime = reminderTime.Add(new TimeSpan(0, 0, 5));
                        ReminderTimer.Interval = reminderTime - DateTime.Now;
                        ReminderTimer.Start();
                    }
                }
            }
        }

        private void dismissAppointments()
        {
            ScheduleAppointmentCollection newcollection = new ScheduleAppointmentCollection();
            ScheduleAppointmentCollection oldcollection = this.Model.Appointments;
            foreach (ScheduleAppointment r in this.Model.Appointments)
            {
                if (this.Model.ReminderAppointments.Contains(r))
                {
                    r.IsDismissed = true;
                }
                newcollection.Add(r);
            }
            this.Model.Appointments = newcollection;
            this.Model.ReminderAppointments.Clear();
        }
        
        private void ShowReminder()
        {

            this.Dispatcher.BeginInvoke(
                (ThreadStart)delegate()
                {
                    ScheduleReminderControl remain = new ScheduleReminderControl();                   
                    this.ReminderModel.Pop = p;
                    p.Closed += new EventHandler(p_Closed);
                    p.HorizontalOffset = this.ActualWidth / 2;
                    p.VerticalOffset = this.ActualHeight / 3;
                    remain.DataContext = this.ReminderModel;
                    if (!p.IsOpen && this.Model.Appointments.Count != 0)
                    {
                        this.Model.UpdateReminders();
                        ObservableCollection<ScheduleAppointment> apps = this.Model.ReminderAppointments as ObservableCollection<ScheduleAppointment>;
                        ScheduleReminderEventArgs args = new ScheduleReminderEventArgs(apps);
                        if (ReminderInvoking != null)
                        {
                            ReminderInvoking(this, args);
                            dismissAppointments();
                        }

                        if (this.ReminderService != null)
                        {
                            this.ReminderService.ReminderInvoked(this, args);
                        }
                        else
                        {
                            p.Child = remain;
                            p.IsOpen = true;
                            if (ReminderInvoked != null)
                            {
                                ReminderInvoked(this, args);
                            }
                        }

                    }
                }
            );
        }

        void p_Closed(object sender, EventArgs e)
        {
            this.calculate();
        }

      
        private DateTime GetNearestReminder()
        {
            DateTime nearest = new DateTime();
            ScheduleAppointmentCollection coll = this.Model.Appointments;
            foreach (ScheduleAppointment app in coll)
            {
                if (!(app.IsDismissed))
                {
                    if (nearest == new DateTime())
                        nearest = app.ReminderTime;
                    else if (app.ReminderTime <= nearest)
                        nearest = app.ReminderTime;
                }
            }
            return nearest;
        }

        #endregion

        #region Model

        private ScheduleCalendarViewModel model;
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public ScheduleCalendarViewModel Model
        {
            get
            {
                if (this.model == null)
                {
                    this.model = this.OnCreateCalendarViewModel();
                    
                    this.model.PropertyChanged += new PropertyChangedEventHandler(OnModelPropertyChanged);
                    this.model.AppointmentClick += new ScheduleAppointmentClickEventHandler(model_OnAppointmentClick);
                    this.model.AppointmentDoubleClick += new ScheduleAppointmentClickEventHandler(model_OnAppointmentDoubleClick);
                    this.model.AppointmentWindowOpening += new ScheduleAppointmentCancelEventHandler(model_AppointmentWindowOpening);
                    this.model.AppointmentWindowOpened += new ScheduleAppointmentClickEventHandler(model_OnAppointmentWindowOpened);
                    this.model.AppointmentWindowClosing += new ScheduleAppointmentCancelEventHandler(model_AppointmentWindowClosing);
                    this.model.AppointmentWindowClosed += new ScheduleAppointmentClickEventHandler(model_OnAppointmentWindowClosed);
                    this.model.AppointmentResized += new ScheduleAppointmentResizedEventHandler(model_AppointmentResized);
                    this.model.AppointmentResizing += new ScheduleAppointmentResizingEventHandler(model_AppointmentResizing);
                    this.model.RaiseAppointmentDatesBoldEvent += new EventHandler(model_SetAppointmentDatesBoldEvent);
                    this.model.CalendarAdded +=new ScheduleCalendarAddedEventHandler(model_CalendarAdded);
                    this.model.RaiseDeleteSelectedAppointment += new EventHandler<EventArgs>(model_RaiseDeleteSelectedAppointment);
                    this.model.SelectedAppointmentChanged += new ScheduleSelectedAppointmentEventHandler(model_SelectedAppointmentChanged);
                }

                return this.model;
            }
        }

        void Holidays_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        ScheduleAppointment tempapp;
                        foreach (ScheduleHolidays item in e.NewItems)
                        {
                            //if (!CheckHolidayAppointment(item))
                            //{
                                tempapp = new ScheduleAppointment();
                                tempapp.InitializeFromApp(item);
                                tempapp.AllDay = true;
                                tempapp.CurrentAppointmentType = AppointmentType.Holiday;
                                this.Appointments.Add(tempapp);
                            //}
                        }
                        tempapp = null;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        foreach (ScheduleHolidays item in e.OldItems)
                        {
                            RemoveHolidayAppointment(item);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Remove the Selected holiday item from the Holiday collection
        /// </summary>
        /// <param name="item"></param>
        private void RemoveHolidayAppointment(ScheduleHolidays item)
        {
            var removeItems = this.Appointments.Where(app => app.StartTime == item.StartTime &&
               app.EndTime == item.EndTime && app.Subject == item.Subject
               && app.Location == item.Location && app.IsRecurrenceAppointment == item.IsRecurrenceAppointment).ToList();
            foreach (var rec in removeItems)
            {
                this.Appointments.Remove(rec);
            }
        }

        /// <summary>
        /// Check for the Holiday item element availability in the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool CheckHolidayAppointment(ScheduleHolidays item)
        {
            var existingItems = this.Appointments.Where(app => app.StartTime == item.StartTime &&
               app.EndTime == item.EndTime && app.Subject == item.Subject
               && app.Location == item.Location && app.IsRecurrenceAppointment == item.IsRecurrenceAppointment).ToList();
            return ((existingItems.Count > 0) ? true : false);
        }

        
        private void model_RaiseDeleteSelectedAppointment(object sender, EventArgs e)
        {
            this.DeleteCurrentSelectedAppointment();
        }

        private void DeleteCurrentSelectedAppointment()
        {
            switch (this.ScheduleType)
            {
                case ScheduleType.Day:
                case ScheduleType.Week:
                case ScheduleType.WorkWeek:
                    this.daysView.SetupAppointments();
                    break;
                case ScheduleType.Month:
                    this.monthView.SetupAppointments();
                    break;
                case ScheduleType.ScheduleView:
                    this.horizontalView.SetupAppointments();
                    break;
            }
        }

        private void model_CalendarAdded(ScheduleCalendarAddedEventArgs e)
        {
            if (CalendarAdded != null)
                CalendarAdded(e);
        }      

        private void model_SelectedAppointmentChanged(ScheduleAppointmentEventArgs e)
        {
            if (SelectedAppointmentChanged != null)
                SelectedAppointmentChanged(e);      
        }           


        private void model_SetAppointmentDatesBoldEvent(object sender, EventArgs e)
        {
            this.SetAppointmentDatesBold();
        }

        private void model_AppointmentResizing(ScheduleAppointmentResizingEventArgs e)
        {
            if (AppointmentResizing != null)
                AppointmentResizing(e);
        }

        private void model_AppointmentResized(ScheduleAppointmentResizedEventArgs e)
        {
            if (AppointmentResized != null)
                AppointmentResized(e);
        }

        private void model_AppointmentWindowClosing(ScheduleAppointmentCancelEventArgs e)
        {
            if (AppointmentWindowClosing != null)
                AppointmentWindowClosing(e);
        }

        private void model_AppointmentWindowOpening(ScheduleAppointmentCancelEventArgs e)
        {
            if (AppointmentWindowOpening != null)
                AppointmentWindowOpening(e);
        }

        private void model_OnAppointmentWindowClosed(object sender,ScheduleAppointmentEventArgs e)
        {
            if (AppointmentWindowClosed != null)
                AppointmentWindowClosed(this,e);
        }

        private void model_OnAppointmentWindowOpened(object sender,ScheduleAppointmentEventArgs e)
        {
            if (AppointmentWindowOpened != null)
                AppointmentWindowOpened(this,e);
        }

        private void model_OnAppointmentDoubleClick(object sender,ScheduleAppointmentEventArgs e)
        {
            if (AppointmentDoubleClick != null)
                AppointmentDoubleClick(this,e);
        }

        private void model_OnAppointmentClick(object sender,ScheduleAppointmentEventArgs e)
        {
            if (AppointmentClick != null)
                AppointmentClick(this,e);
        }

        private void SetAppointmentDatesBold()
        {
            if (this.calendarView != null)
                this.calendarView.SetAppointmentDateBold();
        }

        /// <summary>
        /// Occurs when [calendar added].
        /// </summary>
        public event ScheduleCalendarAddedEventHandler CalendarAdded;

        /// <summary>
        /// Occurs when [Selected appointment changes]
        /// </summary>
        public event ScheduleSelectedAppointmentEventHandler SelectedAppointmentChanged;

        /// <summary>
        /// Occurs when [appointment resizing].
        /// </summary>
        public event ScheduleAppointmentResizingEventHandler AppointmentResizing;
        /// <summary>
        /// Occurs when [appointment resized].
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
        /// Occurs when [appointment window opening].
        /// </summary>
        public event ScheduleAppointmentCancelEventHandler AppointmentWindowOpening;
        /// <summary>
        /// Occurs when [appointment window opened].
        /// </summary>
        public event ScheduleAppointmentClickEventHandler AppointmentWindowOpened;
        /// <summary>
        /// Occurs when [appointment window closing].
        /// </summary>
        public event ScheduleAppointmentCancelEventHandler AppointmentWindowClosing;

        /// <summary>
        /// Occurs when [appointment window closed].
        /// </summary>
        public event ScheduleAppointmentClickEventHandler AppointmentWindowClosed;

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentSelectedAppointment")
            {
                this.SelectedAppointment = this.Model.CurrentSelectedAppointment;
            }
            else if (e.PropertyName == "CurrentScheduleType")
            {
                this.ScheduleType = this.Model.CurrentScheduleType;
            }
            else if (e.PropertyName == "Holidays")
            {
                NotifyHollidaysCollection();               
            }
            //else if (e.PropertyName == "Appointments")
            //{
            //    this.Appointments = this.Model.Appointments;
            //    calculate();
            //}
        }
        
        private void NotifyHollidaysCollection()
        {
            this.Holidays.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Holidays_CollectionChanged);
            this.Holidays.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Holidays_CollectionChanged);
        }


        /// <summary>
        /// This method will return a new schedule calendar view model
        /// </summary>
        protected virtual ScheduleCalendarViewModel OnCreateCalendarViewModel()
        {
            var model = new ScheduleCalendarViewModel();
            model.SelectedDates.Add(DateTime.Now.Date);
            return model;
        }

        #endregion

        #region ScheduleReminderWrapper

        private ScheduleReminderWrapper reminderModel;
        /// <summary>
        /// Gets the reminder model.
        /// </summary>
        /// <value>The reminder model.</value>
        public ScheduleReminderWrapper ReminderModel
        {
            get
            {
                if (this.reminderModel == null)
                {
                    this.reminderModel = new ScheduleReminderWrapper();
                    this.reminderModel.Model = this.Model;

                }

                return this.reminderModel;
            }
        }

        #endregion

        #region ItemsSource (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ItemsSource for the Schedule Appointments.
        /// </summary>
        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ItemsSource.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(Schedule), new PropertyMetadata(null, OnItemsSourceChanged));

        private bool isItemsSourceChangedBeforeTemplateApplied = false;
        private static void OnItemsSourceChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            //if (schedule.ItemsSource is IEnumerable)
            //{
            //    if (schedule.ItemsSource is INotifyCollectionChanged)
            //    {
            //        (schedule.ItemsSource as INotifyCollectionChanged).CollectionChanged += Schedule_CollectionChanged;
            //    }
            //}
            if (schedule.isTemplateApplied)
            {
                schedule.SetItemsSourceOnModel(args.NewValue);
            }
            else
            {
                schedule.isItemsSourceChangedBeforeTemplateApplied = true;
            }
        }

        private void SetItemsSourceOnModel(object itemsSource)
        {
            this.Model.SetSourceList(itemsSource);
            var handler = this.ItemsSourceChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs when itemsSource changed
        /// </summary>
        public event EventHandler ItemsSourceChanged;

        #endregion

        #region AppointmentMapping (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AppointmentMapping for the ItemsSource set.
        /// </summary>
        public ScheduleAppointmentMapping AppointmentMapping
        {
            get { return (ScheduleAppointmentMapping)GetValue(AppointmentMappingProperty); }
            set { SetValue(AppointmentMappingProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AppointmentMapping.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentMappingProperty = DependencyProperty.Register("AppointmentMapping", typeof(ScheduleAppointmentMapping), typeof(Schedule), new PropertyMetadata(null, OnAppointmentMappingChanged));

        private static void OnAppointmentMappingChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.AppointmentMapping = (ScheduleAppointmentMapping)args.NewValue;
        }

        #endregion
				
        #region Appointments

        /// <summary>
        /// Gets or sets the appointments.
        /// </summary>
        /// <value>The appointments.</value>
        public ScheduleAppointmentCollection Appointments
        {
            get
            {
                return this.Model.Appointments;
            }

            set
            {
                if (this.ItemsSource == null && !this.Model.IsDataBound && this.Model.Appointments != value)
                {
                   // this.Model.Appointments = value;
                    var appCollection = value as ScheduleAppointmentCollection;
                    foreach (var app in appCollection)
                        this.model.Appointments.Add(app);
                    this.Model.Appointments = value;
                    this.setReminderThresholdTimeChanged();
                }
            }
        }

        #endregion

        #region Holidays

        /// <summary>
        /// Gets or sets the appointments.
        /// </summary>
        /// <value>The appointments.</value>
        public ScheduleHolidaysCollection Holidays
        {
            get
            {
                return this.Model.Holidays;
            }

            set
            {
                if (this.ItemsSource == null)
                {
                    this.Model.Holidays = value;                                    
                }
            }
        }

        #endregion

        #region AllowResize

        /// <summary>
        /// Gets / Sets the AllowResize property.
        /// </summary>
        public bool AllowResize
        {
            get { return (bool)GetValue(AllowResizeProperty); }
            set { SetValue(AllowResizeProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AllowResize.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowResizeProperty = DependencyProperty.Register("AllowResize",
            typeof(bool), typeof(Schedule), new PropertyMetadata(true, OnAllowResizeChanged));

        private static void OnAllowResizeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.AllowResize = schedule.AllowResize;
        }

        #endregion 

        #region ShowContextMenu

        /// <summary>
        /// Gets / Sets the ShowContextMenu property.
        /// </summary>
        public bool ShowContextMenu
        {
            get { return (bool)GetValue(ShowContextMenuProperty); }
            set { SetValue(ShowContextMenuProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ShowContextMenu.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowContextMenuProperty = DependencyProperty.Register("ShowContextMenu",
            typeof(bool), typeof(Schedule), new PropertyMetadata(true, OnShowContextMenuPropertyChanged));

        private static void OnShowContextMenuPropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.ShowContextMenu = schedule.ShowContextMenu;
        }

        #endregion 

        #region AllowRecurrence

        /// <summary>
        /// Gets / Sets the AllowRecurrence property.
        /// </summary>
        public bool AllowRecurrence
        {
            get { return (bool)GetValue(AllowRecurrenceProperty); }
            set { SetValue(AllowRecurrenceProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AllowRecurrence.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowRecurrenceProperty = DependencyProperty.Register("AllowRecurrence",
            typeof(bool), typeof(Schedule), new PropertyMetadata(true, OnAllowRecurrenceChanged));

        private static void OnAllowRecurrenceChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.AllowRecurrence = schedule.AllowRecurrence;
        }

        #endregion 

        #region AllowDragAndDrop

        /// <summary>
        /// Gets / Sets the AllowDragAndDrop property.
        /// </summary>
        public bool AllowDragAndDrop
        {
            get { return (bool)GetValue(AllowDragAndDropProperty); }
            set { SetValue(AllowDragAndDropProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AllowDragAndDrop.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowDragAndDropProperty = DependencyProperty.Register("AllowDragAndDrop", typeof(bool),
            typeof(Schedule), new PropertyMetadata(true, OnAllowDragAndDropChanged));

        private static void OnAllowDragAndDropChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.AllowDragAndDrop = schedule.AllowDragAndDrop;
        }

        /// <summary>
        /// Occurs when appointments dragging
        /// </summary>
        public event ScheduleAppointmentDragEventHandler AppointmentDragging;
        /// <summary>
        /// Occurs when appointments dragged
        /// </summary>
        public event ScheduleAppointmentDragEventHandler AppointmentDragged;
        /// <summary>
        /// Occurs when appointments dropping
        /// </summary>
        public event ScheduleAppointmentDropEventHandler AppointmentDropping;
        /// <summary>
        /// Occurs when appointments dropped
        /// </summary>
        public event ScheduleAppointmentDropEventHandler AppointmentDropped;

        internal void OnAppointmentDragged(ScheduleAppointmentDragEventArgs args)
        {
            if (AppointmentDragged != null)
            {
                AppointmentDragged(this, args);
            }
        }
        internal void OnAppointmentDropped(ScheduleAppointmentDropEventArgs args)
        {
            if (AppointmentDropped != null)
            {
                AppointmentDropped(this, args);
            }
        }
        internal void OnAppoinmentDragging(ScheduleAppointmentDragEventArgs args)
        {
            if (AppointmentDragging != null)
            {
                AppointmentDragging(this, args);
            }
        }
        internal void OnAppointmentDropping(ScheduleAppointmentDropEventArgs args)
        {
            if (AppointmentDropping != null)
            {
                AppointmentDropping(this, args);
            }
        }



        #endregion 

        #region ScheduleBackground (DependencyProperty)

        /// <summary>
        /// Gets / Sets the SelectionBackground property.
        /// </summary>
        public Brush ScheduleBackground
        {
            get { return (Brush)GetValue(ScheduleBackgroundProperty); }
            set { SetValue(ScheduleBackgroundProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ScheduleBackground.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScheduleBackgroundProperty = DependencyProperty.Register("ScheduleBackground",
            typeof(Brush), typeof(Schedule), new PropertyMetadata(OnScheduleBackgroundChanged));

        private static void OnScheduleBackgroundChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.model.ScheduleBackground = (Brush)args.NewValue;
        }

        #endregion

      
        #region AppointmentBackground (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AppointmentBackground property.
        /// </summary>
        public Brush AppointmentBackground
        {
            get { return (Brush)GetValue(AppointmentBackgroundProperty); }
            set { SetValue(AppointmentBackgroundProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AppointmentBackground.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentBackgroundProperty = DependencyProperty.Register("AppointmentBackground",
            typeof(Brush), typeof(Schedule), new PropertyMetadata(OnAppointmentBackgroundChanged));

        private static void OnAppointmentBackgroundChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.model.AppointmentBackground = (Brush)args.NewValue;
        }

        #endregion
        
        #region SelectionBackground (DependencyProperty)

        /// <summary>
        /// Gets / Sets the SelectionBackground property.
        /// </summary>
        public Brush SelectionBackground
        {
            get { return (Brush)GetValue(SelectionBackgroundProperty); }
            set { SetValue(SelectionBackgroundProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SelectionBackground.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectionBackgroundProperty = DependencyProperty.Register("SelectionBackground", 
            typeof(Brush), typeof(Schedule), new PropertyMetadata(OnSelectionBackgroundChanged));

        private static void OnSelectionBackgroundChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.model.SelectionBackground = (Brush)args.NewValue;
        }

        #endregion

        #region ShadedBackground (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ShadedBackground property.
        /// </summary>
        public Brush ShadedBackground
        {
            get { return (Brush)GetValue(ShadedBackgroundProperty); }
            set { SetValue(ShadedBackgroundProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ShadedBackground.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShadedBackgroundProperty = DependencyProperty.Register("ShadedBackground",
            typeof(Brush), typeof(Schedule), new PropertyMetadata(OnShadedBackgroundChanged));

        private static void OnShadedBackgroundChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.model.ShadedBackground = (Brush)args.NewValue;
        }

        #endregion       

        #region HeaderBrush (DependencyProperty)

        /// <summary>
        /// Gets / Sets the HeaderBrush property.
        /// </summary>
        public Brush HeaderBrush
        {
            get { return (Brush)GetValue(HeaderBrushProperty); }
            set { SetValue(HeaderBrushProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for HeaderBrush.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderBrushProperty = DependencyProperty.Register("HeaderBrush", typeof(Brush), 
            typeof(Schedule), new PropertyMetadata(OnHeaderBrushChanged));

        private static void OnHeaderBrushChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.model.HeaderBrush = (Brush)args.NewValue;
        }

        #endregion

        #region StrokeLine (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ShadedBackground property.
        /// </summary>
        public Brush StrokeLine
        {
            get { return (Brush)GetValue(StrokeLineProperty); }
            set { SetValue(StrokeLineProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for HeaderBrush.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeLineProperty = DependencyProperty.Register("StrokeLine",
            typeof(Brush), typeof(Schedule), new PropertyMetadata(OnStrokeLineChanged));

        private static void OnStrokeLineChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.model.StrokeLine = (Brush)args.NewValue;
        }

        #endregion

        #region StrokeThickness (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ShadedBackground property.
        /// </summary>
        public Thickness StrokeThickness
        {
            get { return (Thickness)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for StrokeThickness.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness",
            typeof(Thickness), typeof(Schedule), new PropertyMetadata(OnStrokeThicknessChanged));

        private static void OnStrokeThicknessChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.model.StrokeThickness = (Thickness)args.NewValue;
        }

        #endregion

        #region DisplayReminder (DependencyProperty)

        /// <summary>
        /// Gets / Sets the DisplayReminder property.
        /// </summary>
        public bool DisplayReminder
        {
            get { return (bool)GetValue(DisplayReminderProperty); }
            set { SetValue(DisplayReminderProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DisplayReminder.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DisplayReminderProperty = DependencyProperty.Register("DisplayReminder", typeof(bool), typeof(Schedule), new PropertyMetadata(false, OnDisplayReminderChanged));

        private static void OnDisplayReminderChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.calculate();

        }

        #endregion

        #region ReminderThresholdTime (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ReminderThresholdTime.
        /// </summary>
        public TimeSpan ReminderThresholdTime
        {
            get { return (TimeSpan)GetValue(ReminderThresholdTimeProperty); }
            set { SetValue(ReminderThresholdTimeProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ReminderThresholdTime.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ReminderThresholdTimeProperty = DependencyProperty.Register("ReminderThresholdTime", typeof(TimeSpan), typeof(Schedule), new PropertyMetadata(new TimeSpan(0, 15, 0), OnReminderThresholdTimeChanged));

        private static void OnReminderThresholdTimeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.setReminderThresholdTimeChanged();
            schedule.calculate();
            //schedule.Model.CurrentReminderThresholdTime = (TimeInterval)args.NewValue;
        }

        private void setReminderThresholdTimeChanged()
        {
            foreach (ScheduleAppointment app in this.Appointments)
            {
                app.ThresholdTime = this.ReminderThresholdTime;
            }
        }

        #endregion

        #region ReminderService

        /// <summary>
        /// Gets or sets the reminder service.
        /// </summary>
        /// <value>The reminder service.</value>
        IScheduleReminderService ReminderService
        {
            get;
            set;
        }

        #endregion

        #region WorkingDays (DependencyProperty)

        ///<summary>
        /// Gets / Sets the WorkingDays property.
        ///</summary>
        public string WorkingDays
        {
            get { return (string)GetValue(WorkingDaysProperty); }
            set { SetValue(WorkingDaysProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for WorkingDays.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WorkingDaysProperty = DependencyProperty.Register("WorkingDays", typeof(string), typeof(Schedule), new PropertyMetadata(null, OnWorkingDaysChanged));

        private static void OnWorkingDaysChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;            
            schedule.UpdateWorkingDays();           
        }

        private void UpdateWorkingDays()
        {
            this.Model.WorkingDays = this.StringToDaysOfWeekConverter(this.WorkingDays);
            
           
        }

        private ObservableCollection<DayOfWeek> StringToDaysOfWeekConverter(string workingDaysString)
        {
            List<DayOfWeek> dayslist = new List<DayOfWeek>();
            DayOfWeek [] weekDays = new DayOfWeek[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday};
            
            string[] days = workingDaysString.Split(',');
            string[] titleCaseDays = new string[days.Count<string>()];
            int i=0;            
            foreach (string daysName in days)
            {
                titleCaseDays[i] = daysName.ToLower().Substring(0, 1).ToUpper() + daysName.ToLower().Substring(1);
                DayOfWeek DayofWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), titleCaseDays[i],false);
                {
                    titleCaseDays[i] = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DayofWeek);
                }
                i = i+1;
            }

            foreach (string d in titleCaseDays)
            {
                //string day = d.Trim().ToLower();
                if (CultureInfo.CurrentCulture.DateTimeFormat.DayNames.Contains(d))
                    dayslist.Add(weekDays[CultureInfo.CurrentCulture.DateTimeFormat.DayNames.ToList().IndexOf(d)]);                
                else
                {
                    ArgumentException e = new ArgumentException("Input string doesnot match any DaysOfWeek instance");
                    throw (e);
                }
            }
            dayslist.Sort();
            ObservableCollection<DayOfWeek> collection = new ObservableCollection<DayOfWeek>();
            foreach (DayOfWeek d in dayslist)
            {
                collection.Add(d);
            }
            return collection;
        }

        #endregion

        #region ScheduleType (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ScheduleType property.
        /// </summary>
        public ScheduleType ScheduleType
        {
            get { return (ScheduleType)GetValue(ScheduleTypeProperty); }
            set { SetValue(ScheduleTypeProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ScheduleType.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScheduleTypeProperty = DependencyProperty.Register("ScheduleType", typeof(ScheduleType), typeof(Schedule), new PropertyMetadata(ScheduleType.Day, OnScheduleTypeChanged));

        private static void OnScheduleTypeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.UpdateScheduleType();
        }
        private bool isMainViewItemsNeedsUpdate = false;
        private void UpdateScheduleType()
        {
            this.Model.CurrentViewMode = ViewMode.Vertical;
            switch (this.ScheduleType)
            {
                case ScheduleType.Day:
                    this.Model.MoveToDayType();
                    break;
                case ScheduleType.WorkWeek:
                    this.Model.MoveToWorkWeekType();
                    break;
                case ScheduleType.Week:
                    this.Model.MoveToWeekType();
                    break;
                case ScheduleType.Month:
                    this.Model.MoveToMonthType();
                    break;
                case ScheduleType.ScheduleView:
                    this.Model.CurrentViewMode = ViewMode.Horizontal;
                    this.Model.MoveToHorizontalType();
                    break;
            }

            if (this.isTemplateApplied)
            {
                UpdateMainViewItems();
            }
            else
            {
                this.isMainViewItemsNeedsUpdate = true;
            }
        }

        private void UpdateMainViewItems()
        {
            switch (this.ScheduleType)
            {
                case ScheduleType.Day:
                case ScheduleType.Week:
                case ScheduleType.WorkWeek:
                    this.mainViewItems.SelectedIndex = 0;
                    break;
                case ScheduleType.Month:
                    this.mainViewItems.SelectedIndex = 1;
                    break;
                case ScheduleType.ScheduleView:
                    this.mainViewItems.SelectedIndex = 2;
                    break;
            }

        }


        #endregion

        #region IsAmPmTimeMode (DependencyProperty)

        /// <summary>
        /// Gets / Sets the IsAmPmTimeMode.
        /// </summary>
        public bool IsAmPmTimeMode
        {
            get { return (bool)GetValue(IsAmPmTimeModeProperty); }
            set { SetValue(IsAmPmTimeModeProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IsAmPmTimeMode.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsAmPmTimeModeProperty =
            DependencyProperty.Register("IsAmPmTimeMode", typeof(bool), typeof(Schedule), new PropertyMetadata(true, OnIsAmPmModeChanged));

        private static void OnIsAmPmModeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.IsAmPmTimeMode = (bool)args.NewValue;
        }

        #endregion

        #region AppointmentStatus (DependencyProperty)
        
        /// <summary>
        /// Gets / Sets the ReminderThresholdTime.
        /// </summary>
        public ScheduleAppointmentStatusCollection AppointmentStatusCollection
        {
            get { return (ScheduleAppointmentStatusCollection)GetValue(AppointmentStatusCollectionProperty); }
            set { SetValue(AppointmentStatusCollectionProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AppointmentStatusCollection.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentStatusCollectionProperty = DependencyProperty.Register("AppointmentStatusCollection", typeof(ScheduleAppointmentStatusCollection), typeof(Schedule), new PropertyMetadata(null, onAppointmentStatusCollectionChanged));

        private static void onAppointmentStatusCollectionChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.AppointmentStatusCollection = (ScheduleAppointmentStatusCollection)args.NewValue;
        }

        #endregion

        #region ContextMenuTimeSlotItems (DependencyProperty)

        /// <summary>
        /// Gets / Sets the contextMenuTimeSlotItems. 
        /// </summary>
        public ObservableCollection<object> ContextMenuTimeSlotItems
        {
            get { return (ObservableCollection<object>)GetValue(contextMenuTimeSlotItemsProperty); }
            set { SetValue(contextMenuTimeSlotItemsProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ContextMenuTimeSlotItems.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty contextMenuTimeSlotItemsProperty = DependencyProperty.Register("ContextMenuTimeSlotItems", typeof(ObservableCollection<object>), typeof(Schedule), new PropertyMetadata(null, onContextMenuTimeSlotItemsChanged));

        private static void onContextMenuTimeSlotItemsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.ContextMenuTimeSlotItems = (ObservableCollection<object>)args.NewValue;
        }
        #endregion        

        #region ContextMenuTimeLineItems (DependencyProperty)

        /// <summary>
        /// Gets / Sets the contextMenuTimeLineItems.
        /// </summary>
        public ObservableCollection<object> ContextMenuTimeLineItems
        {
            get { return (ObservableCollection<object>)GetValue(contextMenuTimeLineItemsProperty); }
            set { SetValue(contextMenuTimeLineItemsProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ContextMenuTimeLineItems.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty contextMenuTimeLineItemsProperty = DependencyProperty.Register("ContextMenuTimeLineItems", typeof(ObservableCollection<object>), typeof(Schedule), new PropertyMetadata(null, contextMenuTimeLineItemsChanged));

        private static void contextMenuTimeLineItemsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.ContextMenuTimeLineItems = (ObservableCollection<object>)args.NewValue;
        }
        #endregion

        #region contextMenuDaysHeaderItems (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ContextMenuDaysHeaderItems.
        /// </summary>
        public ObservableCollection<object> ContextMenuDaysHeaderItems
        {
            get { return (ObservableCollection<object>)GetValue(contextMenuDaysHeaderItemsProperty); }
            set { SetValue(contextMenuDaysHeaderItemsProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ContextMenuDaysHeaderItems.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty contextMenuDaysHeaderItemsProperty = DependencyProperty.Register("ContextMenuDaysHeaderItems", typeof(ObservableCollection<object>), typeof(Schedule), new PropertyMetadata(null, contextMenuDaysHeaderItemsChanged));

        private static void contextMenuDaysHeaderItemsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.ContextMenuDaysHeaderItems = (ObservableCollection<object>)args.NewValue;
        }
        #endregion

        #region ContextMenuAppointmentItems (DependencyProperty)

        /// <summary>
        /// Gets / Sets the contextMenuAppointmentItems.
        /// </summary>
        public ObservableCollection<object> ContextMenuAppointmentItems
        {
            get { return (ObservableCollection<object>)GetValue(contextMenuAppointmentItemsProperty); }
            set { SetValue(contextMenuAppointmentItemsProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ContextMenuAppointmentItems.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty contextMenuAppointmentItemsProperty = DependencyProperty.Register("ContextMenuAppointmentItems", typeof(ObservableCollection<object>), typeof(Schedule), new PropertyMetadata(null, contextMenuAppointmentItemsChanged));

        private static void contextMenuAppointmentItemsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.ContextMenuAppointmentItems = (ObservableCollection<object>)args.NewValue;
        }
        #endregion

        #region ContextMenuAllDayAppointmentItems (DependencyProperty)

        /// <summary>
        /// Gets / Sets the contextMenuAllDayAppointmentItems.
        /// </summary>
        public ObservableCollection<object> ContextMenuAllDayAppointmentItems
        {
            get { return (ObservableCollection<object>)GetValue(contextMenuAllDayAppointmentItemsProperty); }
            set { SetValue(contextMenuAllDayAppointmentItemsProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ContextMenuAllDayAppointmentItems.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty contextMenuAllDayAppointmentItemsProperty = DependencyProperty.Register("ContextMenuAllDayAppointmentItems", typeof(ObservableCollection<object>), typeof(Schedule), new PropertyMetadata(null, contextMenuAllDayAppointmentItemsChanged));

        private static void contextMenuAllDayAppointmentItemsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.ContextMenuAllDayAppointmentItems = (ObservableCollection<object>)args.NewValue;
        }
        #endregion

        #region ContextMenuMonthViewItems (DependencyProperty)

        /// <summary>
        /// Gets / Sets the contextMenuMonthViewTimeLineItems.
        /// </summary>
        public ObservableCollection<object> ContextMenuMonthViewItems
        {
            get { return (ObservableCollection<object>)GetValue(contextMenuMonthViewItemsProperty); }
            set { SetValue(contextMenuMonthViewItemsProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ContextMenuMonthViewItems.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty contextMenuMonthViewItemsProperty = DependencyProperty.Register("ContextMenuMonthViewItems", typeof(ObservableCollection<object>), typeof(Schedule), new PropertyMetadata(null, contextMenuMonthViewItemsChanged));

        private static void contextMenuMonthViewItemsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.ContextMenuMonthViewItems = (ObservableCollection<object>)args.NewValue;
        }
        #endregion

        #region ContextMenuMonthViewAppointmentItems (DependencyProperty)

        /// <summary>
        /// Gets / Sets the contextMenuMonthViewTimeLineItems.
        /// </summary>
        public ObservableCollection<object> ContextMenuMonthViewAppointmentItems
        {
            get { return (ObservableCollection<object>)GetValue(contextMenuMonthViewAppointmentItemsProperty); }
            set { SetValue(contextMenuMonthViewAppointmentItemsProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ContextMenuMonthViewAppointmentItems.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty contextMenuMonthViewAppointmentItemsProperty = DependencyProperty.Register("ContextMenuMonthViewAppointmentItems", typeof(ObservableCollection<object>), typeof(Schedule), new PropertyMetadata(null, contextMenuMonthViewAppointmentItemsChanged));

        private static void contextMenuMonthViewAppointmentItemsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.ContextMenuMonthViewAppointmentItems = (ObservableCollection<object>)args.NewValue;
        }
        #endregion

        #region ContextMenuHorizontalViewItems (DependencyProperty)

        /// <summary>
        /// Gets / Sets the contextMenuHorizontalViewTimeLineItems.
        /// </summary>
        public ObservableCollection<object> ContextMenuHorizontalViewItems
        {
            get { return (ObservableCollection<object>)GetValue(contextMenuHorizontalViewItemsProperty); }
            set { SetValue(contextMenuHorizontalViewItemsProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ContextMenuHorizontalViewItems.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty contextMenuHorizontalViewItemsProperty = DependencyProperty.Register("ContextMenuHorizontalViewItems", typeof(ObservableCollection<object>), typeof(Schedule), new PropertyMetadata(null, contextMenuHorizontalViewItemsChanged));

        private static void contextMenuHorizontalViewItemsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.ContextMenuHorizontalViewItems = (ObservableCollection<object>)args.NewValue;
        }
        #endregion

        #region ContextMenuHorizontalViewAppointmentItems (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ContextMenuHorizontalViewTimeLineItems.
        /// </summary>
        public ObservableCollection<object> ContextMenuHorizontalViewAppointmentItems
        {
            get { return (ObservableCollection<object>)GetValue(contextMenuHorizontalViewAppointmentItemsProperty); }
            set { SetValue(contextMenuHorizontalViewAppointmentItemsProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ContextMenuHorizontalViewAppointmentItems.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty contextMenuHorizontalViewAppointmentItemsProperty = DependencyProperty.Register("ContextMenuHorizontalViewAppointmentItems", typeof(ObservableCollection<object>), typeof(Schedule), new PropertyMetadata(null, contextMenuHorizontalViewAppointmentItemsChanged));

        private static void contextMenuHorizontalViewAppointmentItemsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.ContextMenuHorizontalViewAppointmentItems = (ObservableCollection<object>)args.NewValue;
        }
        #endregion

        #region ContextMenuHorizontalViewTimeLineItems (DependencyProperty)

        /// <summary>
        /// Gets / Sets the contextMenuHorizontalViewTimeLineItems.
        /// </summary>
        public ObservableCollection<object> ContextMenuHorizontalViewTimeLineItems
        {
            get { return (ObservableCollection<object>)GetValue(contextMenuHorizontalViewTimeLineItemsProperty); }
            set { SetValue(contextMenuHorizontalViewTimeLineItemsProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ContextMenuHorizontalViewTimeLineItems.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty contextMenuHorizontalViewTimeLineItemsProperty = DependencyProperty.Register("ContextMenuHorizontalViewTimeLineItems", typeof(ObservableCollection<object>), typeof(Schedule), new PropertyMetadata(null, contextMenuHorizontalViewTimeLineItemsChanged));

        private static void contextMenuHorizontalViewTimeLineItemsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.ContextMenuHorizontalViewTimeLineItems = (ObservableCollection<object>)args.NewValue;
        }
        #endregion

        #region ContextMenuType (DependencyProperty)
        /// <summary>
        /// Gets / Sets the ContextMenuType.
        /// </summary>
        public ContextMenuType ContextMenuType
        {
            get { return (ContextMenuType)GetValue(ContextMenuTypeProperty); }
            set { SetValue(ContextMenuTypeProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ContextMenuType.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ContextMenuTypeProperty = DependencyProperty.Register("ContextMenuType", typeof(ContextMenuType), typeof(Schedule), new PropertyMetadata(ContextMenuType.Default, OnContextMenuTypeChanged));

        private static void OnContextMenuTypeChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.ContextMenuType = (ContextMenuType)args.NewValue;
        }

        #endregion

        #region TimeInterval (DependencyProperty)

        /// <summary>
        /// Gets / Sets the TimeInterval.
        /// </summary>
        public TimeInterval TimeInterval
        {
            get { return (TimeInterval)GetValue(TimeIntervalProperty); }
            set { SetValue(TimeIntervalProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for TimeInterval.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeIntervalProperty = DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(Schedule), new PropertyMetadata(TimeInterval.ThirtyMin, OnTimeIntervalChanged));

        private static void OnTimeIntervalChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.CurrentTimeInterval = (TimeInterval)args.NewValue;
        }

        #endregion

        #region StartWorkHour (DependencyProperty)

        /// <summary>
        /// Gets / Sets the StartWorkHour property.
        /// </summary>
        public int StartWorkHour
        {
            get { return (int)GetValue(StartWorkHourProperty); }
            set { SetValue(StartWorkHourProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for StartWorkHour.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartWorkHourProperty = DependencyProperty.Register("StartWorkHour", typeof(int), typeof(Schedule), new PropertyMetadata(8, OnStartWorkHourChanged));

        private static void OnStartWorkHourChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.StartWorkHour = (int)args.NewValue;
        }

        #endregion

        #region EndWorkHour (DependencyProperty)

        /// <summary>
        /// Gets / Sets the EndWorkHour property.
        /// </summary>
        public int EndWorkHour
        {
            get { return (int)GetValue(EndWorkHourProperty); }
            set { SetValue(EndWorkHourProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for EndWorkHour.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EndWorkHourProperty = DependencyProperty.Register("EndWorkHour", typeof(int), typeof(Schedule), new PropertyMetadata(17, OnEndWorkHourChanged));

        private static void OnEndWorkHourChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.EndWorkHour = (int)args.NewValue;
        }

        #endregion

        #region IntervalHeight (DependencyProperty)

        /// <summary>
        /// Gets / Sets the IntervalHeight property.
        /// </summary>
        public double IntervalHeight
        {
            get { return (double)GetValue(IntervalHeightProperty); }
            set { SetValue(IntervalHeightProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for IntervalHeight.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalHeightProperty = DependencyProperty.Register("IntervalHeight", typeof(double), typeof(Schedule), new PropertyMetadata(ScheduleTimeLineItemsControl.DefaultIntervalHeight, OnIntervalHeightChanged));

        private static void OnIntervalHeightChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.IntervalHeight = (double)args.NewValue;
        }

        #endregion

        #region CalendarVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the CalendarVisibility property.
        /// </summary>
        public Visibility CalendarVisibility
        {
            get { return (Visibility)GetValue(CalendarVisibilityProperty); }
            set { SetValue(CalendarVisibilityProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for CalendarVisibility.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CalendarVisibilityProperty = DependencyProperty.Register("CalendarVisibility", typeof(Visibility), typeof(Schedule), new PropertyMetadata(Visibility.Collapsed, OnCalendarVisibilityChanged));
        private bool isCalendarVisibilityChangedBeforeLoaded = false;
        private static void OnCalendarVisibilityChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            if (schedule.isTemplateApplied)
            {
                schedule.GotoCalendarState();
            }
            else
            {
                schedule.isCalendarVisibilityChangedBeforeLoaded = true;
            }
        }

        private void GotoCalendarState()
        {
            if (this.CalendarVisibility == Visibility.Visible)
            {
                //VisualStateManager.GoToState(this, "CalendarVisible", false);
            }
            else
            {
               // VisualStateManager.GoToState(this, "CalendarCollapsed", false);
            }
        }

        #endregion

        #region MonthViewWeekHeaderVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the MonthViewWeekHeaderVisibility property.
        /// </summary>
        public Visibility MonthViewWeekHeaderVisibility
        {
            get { return (Visibility)GetValue(MonthViewWeekHeaderVisibilityProperty); }
            set { SetValue(MonthViewWeekHeaderVisibilityProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for MonthViewWeekHeaderVisibility.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthViewWeekHeaderVisibilityProperty = DependencyProperty.Register("MonthViewWeekHeaderVisibility", typeof(Visibility), typeof(Schedule), new PropertyMetadata(Visibility.Collapsed, OnMonthViewWeekHeaderVisibilityChanged));

        private static void OnMonthViewWeekHeaderVisibilityChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.model.MonthViewWeekHeaderVisibility =(Visibility)args.NewValue;
        }

        #endregion


        #region ApplyTemplate

        private bool isTemplateApplied = false;
        private ScheduleDaysView daysView;
        private ScheduleMonthView monthView;
        private ScheduleHorizontalView horizontalView;
        private ScheduleHeaderTitleBar titleBar;
        private ScheduleCalendarViewItemsControl calendarView;
        private ContentViewItemsControl mainViewItems;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.daysView = this.GetTemplateChild("PART_DaysView") as ScheduleDaysView;
            this.monthView = this.GetTemplateChild("PART_MonthView") as ScheduleMonthView;
            this.horizontalView = this.GetTemplateChild("PART_HorizontalView") as ScheduleHorizontalView;
            this.titleBar = this.GetTemplateChild("PART_HeaderTitleBar") as ScheduleHeaderTitleBar;
            this.calendarView = this.GetTemplateChild("PART_CalendarView") as ScheduleCalendarViewItemsControl;
            this.mainViewItems = this.GetTemplateChild("PART_MainViewItems") as ContentViewItemsControl;
            this.UpdateModelToInnerControls();
            this.isTemplateApplied = true;
            this.EnsureProperties();
        }

        private void UpdateModelToInnerControls()
        {
            this.daysView.SetCalendarViewModel(this.Model);
            this.monthView.SetCalendarViewModel(this.Model);
            this.horizontalView.SetCalendarViewModel(this.Model);
            this.titleBar.SetCalendarViewModel(this.Model);
            this.calendarView.SetCalendarViewModel(this.Model);
        }

        private void EnsureProperties()
        {
            if (this.isCalendarVisibilityChangedBeforeLoaded)
            {
                this.GotoCalendarState();
            }

            if (this.isMainViewItemsNeedsUpdate)
            {
                this.UpdateMainViewItems();
            }

            if (this.isItemsSourceChangedBeforeTemplateApplied)
            {
                this.Model.SetSourceList(this.ItemsSource);
            }
        }

        #endregion

        #region CalendarItemsCount (DependencyProperty)

        /// <summary>
        /// Gets / Sets the CalendarItems in the CalendarView Panel.
        /// </summary>
        public int CalendarItemsCount
        {
            get { return (int)GetValue(CalendarItemsCountProperty); }
            set
            {
                if (value > 0)
                {
                    SetValue(CalendarItemsCountProperty, value);
                }
                else
                {
                    throw new InvalidProgramException("CalendarItemsCount should be greater than 0");
                }
            }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for CalendarItemsCount.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CalendarItemsCountProperty = DependencyProperty.Register("CalendarItemsCount", typeof(int), typeof(Schedule), new PropertyMetadata(1));

        #endregion

        #region AllowEdit (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllowEdit property.
        /// </summary>
        public bool AllowEdit
        {
            get { return (bool)GetValue(AllowEditProperty); }
            set { SetValue(AllowEditProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AllowEdit.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowEditProperty = DependencyProperty.Register("AllowEdit", typeof(bool), typeof(Schedule), new PropertyMetadata(true, OnAllowEditChanged));

        private static void OnAllowEditChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.AllowEdit = (bool)args.NewValue;
        }

        #endregion

        #region AllowDelete (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllowDelete property.
        /// </summary>
        public bool AllowDelete
        {
            get { return (bool)GetValue(AllowDeleteProperty); }
            set { SetValue(AllowDeleteProperty, value); }
        }

        /// <summary>
        /// Static Dependency Property for AllowDelete
        /// </summary> 
        public static readonly DependencyProperty AllowDeleteProperty = DependencyProperty.Register("AllowDelete", typeof(bool), typeof(Schedule), new PropertyMetadata(true, OnAllowDeleteChanged));

        private static void OnAllowDeleteChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.AllowDelete = (bool)args.NewValue;
        }

        #endregion

        #region AllowAddNew (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllowAddNew property.
        /// </summary>
        public bool AllowAddNew
        {
            get { return (bool)GetValue(AllowAddNewProperty); }
            set { SetValue(AllowAddNewProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AllowAddNew.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowAddNewProperty = DependencyProperty.Register("AllowAddNew", typeof(bool), typeof(Schedule), new PropertyMetadata(true, OnAllowAddNewChanged));

        private static void OnAllowAddNewChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.AllowAddNew = (bool)args.NewValue;
        }

        #endregion

        #region SelectedAppointment (DependencyProperty)

        /// <summary>
        /// Gets / Sets the selected appointment
        /// </summary>
        public ScheduleAppointment SelectedAppointment
        {
            get { return (ScheduleAppointment)GetValue(SelectedAppointmentProperty); }
            set { SetValue(SelectedAppointmentProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for SelectedAppointment.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedAppointmentProperty = DependencyProperty.Register("SelectedAppointment", typeof(ScheduleAppointment), typeof(Schedule), new PropertyMetadata(null));


        #endregion

        #region ScheduleTitleBarVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ScheduleTitleBarVisibility.
        /// </summary>
        public Visibility TitleBarVisibility
        {
            get { return (Visibility)GetValue(TitleBarVisibilityProperty); }
            set { SetValue(TitleBarVisibilityProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for TitleBarVisibility.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleBarVisibilityProperty = DependencyProperty.Register("TitleBarVisibility", typeof(Visibility), typeof(Schedule), new PropertyMetadata(Visibility.Visible));

        #endregion

        #region TitleBarTextConverter (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ScheduleTitleBarVisibility.
        /// </summary>
        public IValueConverter TitleBarTextConverter
        {
            get { return (IValueConverter)GetValue(TitleBarTextConverterProperty); }
            set { SetValue(TitleBarTextConverterProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for TitleBarTextConverter.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleBarTextConverterProperty = DependencyProperty.Register("TitleBarTextConverter", typeof(IValueConverter), typeof(Schedule), new PropertyMetadata(new ScheduleTextToSelectedDatesConverter(), TitleBarTextConverterChanged));

        private static void TitleBarTextConverterChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.TitleBarTextConverter = (IValueConverter)args.NewValue;
        }


        #endregion

        #region DaysHeaderTextConverter (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ScheduleTitleBarVisibility.
        /// </summary>
        public IValueConverter DaysHeaderTextConverter
        {
            get { return (IValueConverter)GetValue(DaysHeaderTextConverterProperty); }
            set { SetValue(DaysHeaderTextConverterProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for DaysHeaderTextConverter.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DaysHeaderTextConverterProperty = DependencyProperty.Register("DaysHeaderTextConverter", typeof(IValueConverter), typeof(Schedule), new PropertyMetadata(DaysHeaderTextConverterChanged));

        private static void DaysHeaderTextConverterChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.DaysHeaderTextConverter = (IValueConverter)args.NewValue;
        }
        #endregion

        #region MonthViewDateTextConverter (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ScheduleTitleBarVisibility.
        /// </summary>
        public IValueConverter MonthViewDateTextConverter
        {
            get { return (IValueConverter)GetValue(MonthViewDateTextConverterProperty); }
            set { SetValue(MonthViewDateTextConverterProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for MonthViewDateTextConverter.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MonthViewDateTextConverterProperty = DependencyProperty.Register("MonthViewDateTextConverter", typeof(IValueConverter), typeof(Schedule), new PropertyMetadata(MonthViewDateTextConverterChanged));

        private static void MonthViewDateTextConverterChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var schedule = dpo as Schedule;
            schedule.Model.MonthViewDateTextConverter = (IValueConverter)args.NewValue;
        }
        #endregion

        #region TimelineHourDivisionVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ScheduleTimelineHourDivisionVisibility.
        /// </summary>
        public Visibility TimelineHourDivisionVisibility
        {
            get { return (Visibility)GetValue(TimelineHourDivisionVisibilityProperty); }
            set { SetValue(TimelineHourDivisionVisibilityProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for TimelineHourDivisionVisibility.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimelineHourDivisionVisibilityProperty = DependencyProperty.Register("TimelineHourDivisionVisibility", typeof(Visibility), typeof(Schedule), new PropertyMetadata(Visibility.Visible));

        #endregion

        #region TimelineVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ScheduleTimelineVisibility.
        /// </summary>
        public Visibility TimelineVisibility
        {
            get { return (Visibility)GetValue(TimelineVisibilityProperty); }
            set { SetValue(TimelineVisibilityProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for TimelineVisibility.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimelineVisibilityProperty = DependencyProperty.Register("TimelineVisibility", typeof(Visibility), typeof(Schedule), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region AppointmentToolTip (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ShadedBackground property.
        /// </summary>
        public DataTemplate AppointmentToolTipDataTemplate
        {
            get { return (DataTemplate)GetValue(AppointmentToolTipDataTemplateProperty); }
            set { SetValue(AppointmentToolTipDataTemplateProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AppointmentToolTipDataTemplate.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentToolTipDataTemplateProperty = DependencyProperty.Register("AppointmentToolTipDataTemplate",
            typeof(DataTemplate), typeof(Schedule), new PropertyMetadata(null));

        //private static void OnToolTipChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        //{
        //    var schedule = dpo as Schedule;
        //    schedule.model.AppointmentToolTip = (DataTemplate)args.NewValue;
        //}

        #endregion

        #region Method
        /// <summary>
        /// Moves to date given as arguement.
        /// </summary>
        /// <param name="date">The date.</param>
        public void MoveToDate(DateTime date)
        {
            ObservableCollection<DateTime> dateColl = new ObservableCollection<DateTime>();

            if (this.ScheduleType == ScheduleType.Day || this.ScheduleType == ScheduleType.ScheduleView)
            {
                dateColl.Add(date);
                this.Model.SelectedDates = dateColl;
            }
            else if (this.ScheduleType == ScheduleType.Week)
            {
                var firstDate = date;
                var startDate = firstDate.StartOfWeek(DayOfWeek.Sunday);
                var newDates = new ObservableCollection<DateTime>();
                for (int i = 0; i < 7; i++)
                {
                    newDates.Add(startDate);
                    startDate = startDate.AddDays(1);

                }
                this.Model.SelectedDates = newDates;
            }
            else if (this.ScheduleType == ScheduleType.WorkWeek)
            {
                var firstDate = date.StartOfWeek(DayOfWeek.Sunday).AddDays(7);
                var newDates = new ObservableCollection<DateTime>();
                for (int i = 0; i < this.Model.WorkingDays.Count; i++)
                {
                    newDates.Add(firstDate.StartOfWeek(this.Model.WorkingDays[i]));
                }
                this.Model.SelectedDates = newDates;
            }
            else if (this.ScheduleType == ScheduleType.Month)
            {
                var firstDate = date;
                var monthStart = new DateTime(firstDate.Year, firstDate.Month, 1);
                var startDate = monthStart.AddDays(-(int)monthStart.DayOfWeek);
                var newDates = new ObservableCollection<DateTime>();
                newDates.Add(startDate);
                for (int i = 1; i < 42; i++)
                {
                    startDate = startDate.AddDays(1);
                    newDates.Add(startDate);
                }

                this.Model.SelectedDates = newDates;
            }
        }


        /// <summary>
        ///  this method routed to get preview appointments
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.RoutedEventArgs"/> that contains
        /// the event data.</param>
        public ScheduleAppointment GetPreviewAppointment(RoutedEventArgs e)
        {
  #if !SILVERLIGHT          
            switch (this.Model.CurrentScheduleType) 
            {

                case Controls.Schedule.ScheduleType.Day:
                case Controls.Schedule.ScheduleType.Week:
                case Controls.Schedule.ScheduleType.WorkWeek:
                    {
                        return this.daysView.GetTimeIntevalFromMousePoint(e);
                        
                    }
                case Controls.Schedule.ScheduleType.Month:
                    {
                        return this.monthView.GetTimeIntevalFromMousePoint(e);
                        
                    }
                case Controls.Schedule.ScheduleType.ScheduleView:
                    {
                        return this.horizontalView.GetTimeIntevalFromMousePoint(e);
                      
                    }
                default:
                    return null;

            }
#elif  SILVERLIGHT 
            return null;
#endif
           
        }
        #endregion

        #region Import Implementation

        /// <summary>
        /// Imports the ICS.
        /// </summary>
        public void ImportICS()
        {
            String contents = null;
            List<String> events = new List<string>();
            String content;
            String localcontent;
            String[] line;
            String[] eventline;
            //String[] recurrence_rule;
            DateTime starttime, endtime;
            ScheduleAppointmentCollection apps = new ScheduleAppointmentCollection();

            OpenFileDialog opendialog = new OpenFileDialog();
            opendialog.Multiselect = false;
            opendialog.Filter = "ICalendar|*.ics";
            bool? dialogResult = opendialog.ShowDialog();

            if (dialogResult == true)
            {
#if SILVERLIGHT
                
                StreamReader reader = new StreamReader(opendialog.File.OpenRead());
#else
                StreamReader reader = new StreamReader(opendialog.OpenFile());
#endif

                while (!reader.EndOfStream)
                {
                    contents = reader.ReadLine();
                    if (contents.Length != 0)
                    {
                        line = contents.Split(':');
                        if (line[0] == "BEGIN" && line[1] == "VEVENT")
                        {
                            ScheduleAppointment schedule_appointment = new ScheduleAppointment();
                            content = "";
                            do
                            {
                            nextline:
                                eventline = null;
                                localcontent = reader.ReadLine();
                                content += localcontent + "?";
                                eventline = localcontent.Split(':');
                                if (eventline[0] == "LOCATION")
                                {
                                    schedule_appointment.Location = eventline[1];
                                }
                                if (eventline[0].StartsWith("SUMMARY"))
                                {
                                    schedule_appointment.Subject = eventline[1];

                                }
                                if (eventline[0].StartsWith("DTSTART"))
                                {
                                    if (!eventline[1].Contains('T'))
                                    {
                                        schedule_appointment.AllDay = true;

                                        schedule_appointment.StartTime = parseDateTime(eventline[1], true, false);
                                    }
                                    else
                                    {

                                        starttime = parseDateTime(eventline[1], false, false);
                                        if (eventline[1].Contains('Z') || eventline[1].Contains('z') || eventline[0].Contains("Coordinated Universal Time"))
                                            schedule_appointment.StartTime = starttime.ToLocalTime();
                                        else
                                            schedule_appointment.StartTime = starttime;
                                    }
                                }
                                if (eventline[0].StartsWith("DTEND"))
                                {
                                    if (!eventline[1].Contains('T'))
                                    {
                                        schedule_appointment.AllDay = true;
                                        schedule_appointment.EndTime = parseDateTime(eventline[1], true, true);
                                        endtime = schedule_appointment.EndTime.AddDays(-1);
                                        schedule_appointment.EndTime = endtime;

                                    }
                                    else
                                    {
                                        endtime = parseDateTime(eventline[1], false, true);
                                        if (eventline[1].Contains('Z') || eventline[1].Contains('z') || eventline[0].Contains("Coordinated Universal Time"))
                                            schedule_appointment.EndTime = endtime.ToLocalTime();
                                        else
                                            schedule_appointment.EndTime = endtime;
                                    }
                                }
                                if (eventline[0] == "RRULE")
                                {
                                    //schedule_appointment.IsRecurrenceAppointment = true;
                                    //recurrence_rule = eventline[1].Split(';');
                                    //foreach (String param in recurrence_rule)
                                    //{
                                    //    String[] attribute = param.Split('=');
                                    //    if (attribute[0] == "FREQ")
                                    //    {
                                    //        schedule_appointment.CurrentRecurrencePatternMode = getRecurrencePatternType(attribute[1]);
                                    //    }
                                    //    if (attribute[0] == "COUNT")
                                    //    {
                                    //        schedule_appointment.EndOccurenceCount = Convert.ToInt32(attribute[1]);
                                    //        schedule_appointment.StartRecurrenceTime = schedule_appointment.StartTime;
                                    //        schedule_appointment.EndRecurrenceTime = schedule_appointment.EndTime.AddDays(schedule_appointment.EndOccurenceCount);
                                    //    }

                                    //}

                                    RecurrenceStringConverter recStringConverter = new RecurrenceStringConverter();
                                    var recApp = recStringConverter.Convert(localcontent, typeof(ScheduleAppointment), null, System.Globalization.CultureInfo.CurrentCulture) as ScheduleAppointment;
                                    if (recApp != null)
                                    {
                                        //Daily
                                        schedule_appointment.IsDailySelected = recApp.IsDailySelected;
                                        schedule_appointment.DailyDays = recApp.DailyDays;

                                        //weekly
                                        schedule_appointment.IsWeeklySelected = recApp.IsWeeklySelected;
                                        schedule_appointment.IsWeeklySelected = recApp.IsWeeklySelected;
                                        schedule_appointment.IsWeeklySundaySelected = recApp.IsWeeklySundaySelected;
                                        schedule_appointment.IsWeeklyMondaySelected = recApp.IsWeeklyMondaySelected;
                                        schedule_appointment.IsWeeklyTuesdaySelected = recApp.IsWeeklyTuesdaySelected;
                                        schedule_appointment.IsWeeklyWednesdaySelected = recApp.IsWeeklyWednesdaySelected;
                                        schedule_appointment.IsWeeklyThursdaySelected = recApp.IsWeeklyThursdaySelected;
                                        schedule_appointment.IsWeeklyFridaySelected = recApp.IsWeeklyFridaySelected;
                                        schedule_appointment.IsWeeklySaturdaySelected = recApp.IsWeeklySaturdaySelected;
                                        schedule_appointment.WeeklyWeeks = recApp.WeeklyWeeks;

                                        //monthly
                                        schedule_appointment.IsMonthlySelected = recApp.IsMonthlySelected;
                                        schedule_appointment.IsMonthlyMultiDays = recApp.IsMonthlyMultiDays;
                                        schedule_appointment.IsMonthlyCustomDays = recApp.IsMonthlyCustomDays;
                                        schedule_appointment.MonthlyDays = recApp.MonthlyDays;
                                        schedule_appointment.MonthlyMonth = recApp.MonthlyMonth;
                                        schedule_appointment.MonthlyMonthMulti = recApp.MonthlyMonthMulti;
                                        schedule_appointment.MonthlyWeekOrderSelected = recApp.MonthlyWeekOrderSelected;
                                        schedule_appointment.MonthlyDaySelected = recApp.MonthlyDaySelected;

                                        //yearly
                                        schedule_appointment.IsYearlySelected = recApp.IsYearlySelected;
                                        schedule_appointment.IsYearlyMultiDays = recApp.IsYearlyMultiDays;
                                        schedule_appointment.IsYearlyCustomDays = recApp.IsYearlyCustomDays;
                                        schedule_appointment.YearlyDays = recApp.YearlyDays;
                                        schedule_appointment.YearlyYear = recApp.YearlyYear;
                                        schedule_appointment.YearlyMonthSelected = recApp.YearlyMonthSelected;
                                        schedule_appointment.YearlyMultiMonthSelected = recApp.YearlyMultiMonthSelected;
                                        schedule_appointment.YearlyMultiWeekOrderSelected = recApp.YearlyMultiWeekOrderSelected;
                                        schedule_appointment.YearlyMultiDaySelected = recApp.YearlyMultiDaySelected;
                                        schedule_appointment.YearlyMonthSelected = recApp.YearlyMonthSelected;

                                        //common
                                        schedule_appointment.IsRecurrenceAppointment = true;
                                        schedule_appointment.StartRecurrenceTime = schedule_appointment.StartTime;
                                        schedule_appointment.EndOccurenceCount = recApp.EndOccurenceCount;
                                        schedule_appointment.IsEndAfter = recApp.IsEndAfter;
                                        schedule_appointment.CurrentRecurrencePatternMode = recApp.CurrentRecurrencePatternMode;
                                    }                


                                }
                                if (eventline.Count() != 2)
                                {
                                    goto nextline;
                                }

                            } while (eventline[1] != "VEVENT");
                            events.Add(content);
                            apps.Add(schedule_appointment);
                        }
                    }


                }

                reader.Close();
            }
            if (apps != null)
            {
                foreach (ScheduleAppointment app in apps)
                {
                    this.Model.Appointments.Add(app);
                }
            }
        }

        private RecurrencePatternMode getRecurrencePatternType(String type)
        {
            if (type == "DAILY")
                return RecurrencePatternMode.Daily;
            if (type == "MONTHLY")
                return RecurrencePatternMode.Monthly;
            if (type == "WEEKLY")
                return RecurrencePatternMode.Weekly;
            if (type == "YEARLY")
                return RecurrencePatternMode.Yearly;

            return RecurrencePatternMode.Daily;
        }

        private DateTime parseDateTime(String date, bool allday, bool endtime)
        {
            int day, month, year, hour, min, sec;
            DateTime ret_date;
            if (!allday)
            {
                year = Convert.ToInt32(date.Substring(0, 4));
                month = Convert.ToInt32(date.Substring(4, 2));
                day = Convert.ToInt32(date.Substring(6, 2));
                hour = Convert.ToInt32(date.Substring(9, 2));
                min = Convert.ToInt32(date.Substring(11, 2));
                sec = Convert.ToInt32(date.Substring(13, 2));
                ret_date = new DateTime(year, month, day, hour, min, sec);
            }
            else
            {
                if (!endtime)
                {
                    year = Convert.ToInt32(date.Substring(0, 4));
                    month = Convert.ToInt32(date.Substring(4, 2));
                    day = Convert.ToInt32(date.Substring(6, 2));
                    ret_date = new DateTime(year, month, day, 0, 0, 0);
                }
                else
                {
                    year = Convert.ToInt32(date.Substring(0, 4));
                    month = Convert.ToInt32(date.Substring(4, 2));
                    day = Convert.ToInt32(date.Substring(6, 2));
                    ret_date = new DateTime(year, month, day, 0, 30, 0);
                }
            }

            return ret_date;
        }
        #endregion

        #region Export Implementation

        /// <summary>
        /// Exports the ICS.
        /// </summary>
        public void ExportICS()
        {

            String contents = null;
            byte[] contents_byte;
            TimeZoneInfo tz = TimeZoneInfo.Local;


            SaveFileDialog sd = new SaveFileDialog();
            sd.DefaultExt = ".ics";
            sd.Filter = "ICalendar|*.ics";
            bool? dialogResult = sd.ShowDialog();

            if (dialogResult == true)
            {
                contents = "BEGIN:VCALENDAR\nPRODID:-//SYNCFUSION//SCHEDULE CONTROL//EN\nVERSION:2.0\nMETHOD:PUBLISH\nX-WR-CALNAME:" + "Schedule" + "_MEETINGS\n\n";


                foreach (ScheduleAppointment sa in this.Model.Appointments)
                {

                    contents += "BEGIN:VEVENT\n";
                    if (sa.Location != null)
                    {
                        contents += "LOCATION:";
                        contents += sa.Location + "\n";
                    }
                    if (sa.Subject != null)
                    {
                        contents += "SUMMARY:";
                        contents += sa.Subject + "\n";
                        contents += "DESCRIPTION:";
                        contents += sa.Subject + "\n";
                    }
                    if (!sa.AllDay && !sa.IsRecurrenceAppointment)
                    {

                        contents += "DTSTART";
                        String starttime = String.Format(":{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}Z", sa.StartTime.ToUniversalTime().Year, sa.StartTime.ToUniversalTime().Month, sa.StartTime.ToUniversalTime().Day, sa.StartTime.ToUniversalTime().Hour, sa.StartTime.ToUniversalTime().Minute, sa.StartTime.ToUniversalTime().Second);
                        contents += starttime + "\n";
                        contents += "DTEND";
                        String endtime = String.Format(":{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}Z", sa.EndTime.ToUniversalTime().Year, sa.EndTime.ToUniversalTime().Month, sa.EndTime.ToUniversalTime().Day, sa.EndTime.ToUniversalTime().Hour, sa.EndTime.ToUniversalTime().Minute, sa.EndTime.ToUniversalTime().Second);
                        contents += endtime + "\n";

                    }
                    else
                    {
                        if (sa.AllDay == true)
                        {
                            contents += "DTSTART;VALUE=DATE:";
                            contents += sa.StartTime.ToUniversalTime().ToString("yyyyMMdd") + "\n";
                            contents += "DTEND;VALUE=DATE:";
                            DateTime endtime = sa.EndTime.AddDays(1);
                            contents += endtime.ToUniversalTime().ToString("yyyyMMdd") + "\n";
                        }
                        if (sa.IsRecurrenceAppointment == true)
                        {


                            contents += "DTSTART";
                            String starttime = String.Format(":{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}Z", sa.StartTime.ToUniversalTime().Year, sa.StartTime.ToUniversalTime().Month, sa.StartTime.ToUniversalTime().Day, sa.StartTime.ToUniversalTime().Hour, sa.StartTime.ToUniversalTime().Minute, sa.StartTime.ToUniversalTime().Second);
                            contents += starttime + "\n";
                            contents += "DTEND";
                            String endtime = String.Format(":{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}Z", sa.EndTime.ToUniversalTime().Year, sa.EndTime.ToUniversalTime().Month, sa.EndTime.ToUniversalTime().Day, sa.EndTime.ToUniversalTime().Hour, sa.EndTime.ToUniversalTime().Minute, sa.EndTime.ToUniversalTime().Second);
                            contents += endtime + "\n";

                            //contents += "RRULE:FREQ=" + sa.CurrentRecurrencePatternMode + ";COUNT=" + sa.EndOccurenceCount + "\n";
                            RecurrenceStringConverter recStrCon= new RecurrenceStringConverter();
                            contents += recStrCon.ConvertBack(sa, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture) + "\n";
                        }
                    }
                    contents += "END:VEVENT\n\n";
                }

                contents += "END:VCALENDAR";

                System.Text.Encoding ascii = Encoding.Unicode;
                contents_byte = ascii.GetBytes(contents);

                using (Stream fs = (Stream)sd.OpenFile())
                {
                    foreach (byte content in contents_byte)
                    {
                        if (content != 0 && content != 34)
                            fs.WriteByte(content);
                    }
                    fs.Close();
                }

            }

        }


        /// <summary>
        /// Exports the CSV.
        /// </summary>
        public void ExportCSV()
        {
            String contents = null;
            byte[] contents_byte;
            String header = "Subject,Start Date,Start Time,End Date,End Time,All day event,Reminder on/off,Reminder Date,Reminder Time,Meeting Organizer,Required Attendees,Optional Attendees,Meeting Resources,Billing Information,Categories,Description,Location,Mileage,Priority,Private,Sensitivity,Show time as";

            SaveFileDialog sd = new SaveFileDialog();
            sd.DefaultExt = ".CSV";
            sd.Filter = "Comma Separated Values|*.CSV";
            bool? dialogResult = sd.ShowDialog();

            if (dialogResult == true)
            {

                contents += header;
                contents += Environment.NewLine;
                foreach (ScheduleAppointment sa in this.Model.Appointments)
                {
                    if (!sa.IsRecurrenceAppointment)
                    {
                        if (sa.Subject != null)
                            contents += sa.Subject;
                        contents += ",";
                        contents += sa.StartTime.ToString("dd-MM-yyyy,hh:mm:ss tt,");
                        contents += sa.EndTime.ToString("dd-MM-yyyy,hh:mm:ss tt,");
                        if (sa.AllDay)
                            contents += "True";
                        else
                            contents += "False";
                        contents += ",,,,,,,,,,";
                        if (sa.Subject != null)
                            contents += sa.Subject;
                        contents += ",";
                        if (sa.Location != null)
                            contents += sa.Location;
                        contents += ",,Normal,False,Normal,";
                        contents += Environment.NewLine;
                    }
                    else
                    {
                        int days = (sa.EndRecurrenceTime - sa.StartRecurrenceTime).Days;
                        int count = sa.EndOccurenceCount < days ? sa.EndOccurenceCount : days;
                        CultureInfo culture = CultureInfo.CurrentCulture;
                        System.Globalization.Calendar calendar = culture.Calendar;
                        for (int i = 0; i < count; i++)
                        {
                            if (sa.Subject != null)
                                contents += sa.Subject;
                            contents += ",";
                            DateTime starttime = calendar.AddDays(sa.StartRecurrenceTime, i);
                            DateTime endtime = calendar.AddDays(sa.EndRecurrenceTime, i);
                            contents += starttime.ToString("dd-MM-yyyy,");
                            contents += sa.StartTime.ToString("hh:mm:ss tt,");
                            contents += starttime.ToString("dd-MM-yyyy,");
                            contents += sa.EndTime.ToString("hh:mm:ss tt,");
                            if (sa.AllDay)
                                contents += "True";
                            else
                                contents += "False";
                            contents += ",,,,,,,,,,";
                            if (sa.Subject != null)
                                contents += sa.Subject;
                            contents += ",";
                            if (sa.Location != null)
                                contents += sa.Location;
                            contents += ",,Normal,False,Normal,";
                            contents += Environment.NewLine;
                        }

                    }
                }

                System.Text.Encoding ascii = Encoding.Unicode;
                contents_byte = ascii.GetBytes(contents);

                using (Stream fs = (Stream)sd.OpenFile())
                {
                    foreach (byte content in contents_byte)
                    {
                        if (content != 0 && content != 34)
                            fs.WriteByte(content);
                    }
                    fs.Close();
                }

            }

        }

#if !SILVERLIGHT
        public void Import(string fileName)
        {
            string Icsextension = new FileInfo(fileName).Extension.ToLower(CultureInfo.InvariantCulture);
            if (File.Exists(fileName))
            using (Stream stream = File.OpenRead(fileName))
            {
                if (Icsextension == ".ics")
                {
                    String contents = null;
                    List<String> events = new List<string>();
                    String content;
                    String localcontent;
                    String[] line;
                    String[] eventline;
                    //String[] recurrence_rule;
                    DateTime starttime, endtime;
                    ScheduleAppointmentCollection apps = new ScheduleAppointmentCollection();

                    OpenFileDialog opendialog = new OpenFileDialog();
                    opendialog.Multiselect = false;
                    opendialog.Filter = "ICalendar|*.ics";
                    opendialog.FileName = fileName;
                  //  bool? dialogResult = opendialog.ShowDialog();
                    stream.Close();
                    if (true)
                    {
#if SILVERLIGHT
                
                StreamReader reader = new StreamReader(opendialog.File.OpenRead());
#else
                        StreamReader reader = new StreamReader(opendialog.OpenFile());
#endif

                        while (!reader.EndOfStream)
                        {
                            contents = reader.ReadLine();
                            if (contents.Length != 0)
                            {
                                line = contents.Split(':');
                                if (line[0] == "BEGIN" && line[1] == "VEVENT")
                                {
                                    ScheduleAppointment schedule_appointment = new ScheduleAppointment();
                                    content = "";
                                    do
                                    {
                                    nextline:
                                        eventline = null;
                                        localcontent = reader.ReadLine();
                                        content += localcontent + "?";
                                        eventline = localcontent.Split(':');
                                        if (eventline[0] == "LOCATION")
                                        {
                                            schedule_appointment.Location = eventline[1];
                                        }
                                        if (eventline[0].StartsWith("SUMMARY"))
                                        {
                                            schedule_appointment.Subject = eventline[1];

                                        }
                                        if (eventline[0].StartsWith("DTSTART"))
                                        {
                                            if (!eventline[1].Contains('T'))
                                            {
                                                schedule_appointment.AllDay = true;

                                                schedule_appointment.StartTime = parseDateTime(eventline[1], true, false);
                                            }
                                            else
                                            {

                                                starttime = parseDateTime(eventline[1], false, false);
                                                if (eventline[1].Contains('Z') || eventline[1].Contains('z') || eventline[0].Contains("Coordinated Universal Time"))
                                                    schedule_appointment.StartTime = starttime.ToLocalTime();
                                                else
                                                    schedule_appointment.StartTime = starttime;
                                            }
                                        }
                                        if (eventline[0].StartsWith("DTEND"))
                                        {
                                            if (!eventline[1].Contains('T'))
                                            {
                                                schedule_appointment.AllDay = true;
                                                schedule_appointment.EndTime = parseDateTime(eventline[1], true, true);
                                                endtime = schedule_appointment.EndTime.AddDays(-1);
                                                schedule_appointment.EndTime = endtime;

                                            }
                                            else
                                            {
                                                endtime = parseDateTime(eventline[1], false, true);
                                                if (eventline[1].Contains('Z') || eventline[1].Contains('z') || eventline[0].Contains("Coordinated Universal Time"))
                                                    schedule_appointment.EndTime = endtime.ToLocalTime();
                                                else
                                                    schedule_appointment.EndTime = endtime;
                                            }
                                        }
                                        if (eventline[0] == "RRULE")
                                        {
                                            RecurrenceStringConverter recStringConverter = new RecurrenceStringConverter();
                                            var recApp = recStringConverter.Convert(localcontent, typeof(ScheduleAppointment), null, System.Globalization.CultureInfo.CurrentCulture) as ScheduleAppointment;
                                            if (recApp != null)
                                            {
                                                //Daily
                                                schedule_appointment.IsDailySelected = recApp.IsDailySelected;
                                                schedule_appointment.DailyDays = recApp.DailyDays;

                                                //weekly
                                                schedule_appointment.IsWeeklySelected = recApp.IsWeeklySelected;
                                                schedule_appointment.IsWeeklySelected = recApp.IsWeeklySelected;
                                                schedule_appointment.IsWeeklySundaySelected = recApp.IsWeeklySundaySelected;
                                                schedule_appointment.IsWeeklyMondaySelected = recApp.IsWeeklyMondaySelected;
                                                schedule_appointment.IsWeeklyTuesdaySelected = recApp.IsWeeklyTuesdaySelected;
                                                schedule_appointment.IsWeeklyWednesdaySelected = recApp.IsWeeklyWednesdaySelected;
                                                schedule_appointment.IsWeeklyThursdaySelected = recApp.IsWeeklyThursdaySelected;
                                                schedule_appointment.IsWeeklyFridaySelected = recApp.IsWeeklyFridaySelected;
                                                schedule_appointment.IsWeeklySaturdaySelected = recApp.IsWeeklySaturdaySelected;
                                                schedule_appointment.WeeklyWeeks = recApp.WeeklyWeeks;

                                                //monthly
                                                schedule_appointment.IsMonthlySelected = recApp.IsMonthlySelected;
                                                schedule_appointment.IsMonthlyMultiDays = recApp.IsMonthlyMultiDays;
                                                schedule_appointment.IsMonthlyCustomDays = recApp.IsMonthlyCustomDays;
                                                schedule_appointment.MonthlyDays = recApp.MonthlyDays;
                                                schedule_appointment.MonthlyMonth = recApp.MonthlyMonth;
                                                schedule_appointment.MonthlyMonthMulti = recApp.MonthlyMonthMulti;
                                                schedule_appointment.MonthlyWeekOrderSelected = recApp.MonthlyWeekOrderSelected;
                                                schedule_appointment.MonthlyDaySelected = recApp.MonthlyDaySelected;

                                                //yearly
                                                schedule_appointment.IsYearlySelected = recApp.IsYearlySelected;
                                                schedule_appointment.IsYearlyMultiDays = recApp.IsYearlyMultiDays;
                                                schedule_appointment.IsYearlyCustomDays = recApp.IsYearlyCustomDays;
                                                schedule_appointment.YearlyDays = recApp.YearlyDays;
                                                schedule_appointment.YearlyYear = recApp.YearlyYear;
                                                schedule_appointment.YearlyMonthSelected = recApp.YearlyMonthSelected;
                                                schedule_appointment.YearlyMultiMonthSelected = recApp.YearlyMultiMonthSelected;
                                                schedule_appointment.YearlyMultiWeekOrderSelected = recApp.YearlyMultiWeekOrderSelected;
                                                schedule_appointment.YearlyMultiDaySelected = recApp.YearlyMultiDaySelected;
                                                schedule_appointment.YearlyMonthSelected = recApp.YearlyMonthSelected;

                                                //common
                                                schedule_appointment.IsRecurrenceAppointment = true;
                                                schedule_appointment.StartRecurrenceTime = schedule_appointment.StartTime;
                                                schedule_appointment.EndOccurenceCount = recApp.EndOccurenceCount;
                                                schedule_appointment.IsEndAfter = recApp.IsEndAfter;
                                                schedule_appointment.CurrentRecurrencePatternMode = recApp.CurrentRecurrencePatternMode;
                                            }


                                        }
                                        if (eventline.Count() != 2)
                                        {
                                            goto nextline;
                                        }

                                    } while (eventline[1] != "VEVENT");
                                    events.Add(content);
                                    apps.Add(schedule_appointment);
                                }
                            }


                        }

                        reader.Close();
                    }
                    if (apps != null)
                    {
                        foreach (ScheduleAppointment app in apps)
                        {
                            this.Model.Appointments.Add(app);
                        }
                    }
                }
            }
        }

  
        public void Export(string fileName)
        {
            string Icsextension = new FileInfo(fileName).Extension.ToLower(CultureInfo.InvariantCulture);
            string Csvextension = new FileInfo(fileName).Extension.ToUpper(CultureInfo.InvariantCulture);
            string directory = fileName.Remove(fileName.LastIndexOf("\\"));
            if (Directory.Exists(directory))
            {
                using (Stream stream = File.Create(fileName))
                {
                    if (Csvextension == ".CSV")
                    {
                        String contents = null;
                        byte[] contents_byte;
                        String header = "Subject,Start Date,Start Time,End Date,End Time,All day event,Reminder on/off,Reminder Date,Reminder Time,Meeting Organizer,Required Attendees,Optional Attendees,Meeting Resources,Billing Information,Categories,Description,Location,Mileage,Priority,Private,Sensitivity,Show time as";

                        SaveFileDialog sd = new SaveFileDialog();
                        sd.DefaultExt = ".CSV";
                        sd.Filter = "Comma Separated Values|*.CSV";
                        sd.FileName = fileName;
                        contents += header;
                        contents += Environment.NewLine;
                        foreach (ScheduleAppointment sa in this.Model.Appointments)
                        {
                            if (!sa.IsRecurrenceAppointment)
                            {
                                if (sa.Subject != null)
                                    contents += sa.Subject;
                                contents += ",";
                                contents += sa.StartTime.ToString("dd-MM-yyyy,hh:mm:ss tt,");
                                contents += sa.EndTime.ToString("dd-MM-yyyy,hh:mm:ss tt,");
                                if (sa.AllDay)
                                    contents += "True";
                                else
                                    contents += "False";
                                contents += ",,,,,,,,,,";
                                if (sa.Subject != null)
                                    contents += sa.Subject;
                                contents += ",";
                                if (sa.Location != null)
                                    contents += sa.Location;
                                contents += ",,Normal,False,Normal,";
                                contents += Environment.NewLine;
                            }
                            else
                            {
                                int days = (sa.EndRecurrenceTime - sa.StartRecurrenceTime).Days;
                                int count = sa.EndOccurenceCount < days ? sa.EndOccurenceCount : days;
                                CultureInfo culture = CultureInfo.CurrentCulture;
                                System.Globalization.Calendar calendar = culture.Calendar;
                                for (int i = 0; i < count; i++)
                                {
                                    if (sa.Subject != null)
                                        contents += sa.Subject;
                                    contents += ",";
                                    DateTime starttime = calendar.AddDays(sa.StartRecurrenceTime, i);
                                    DateTime endtime = calendar.AddDays(sa.EndRecurrenceTime, i);
                                    contents += starttime.ToString("dd-MM-yyyy,");
                                    contents += sa.StartTime.ToString("hh:mm:ss tt,");
                                    contents += starttime.ToString("dd-MM-yyyy,");
                                    contents += sa.EndTime.ToString("hh:mm:ss tt,");
                                    if (sa.AllDay)
                                        contents += "True";
                                    else
                                        contents += "False";
                                    contents += ",,,,,,,,,,";
                                    if (sa.Subject != null)
                                        contents += sa.Subject;
                                    contents += ",";
                                    if (sa.Location != null)
                                        contents += sa.Location;
                                    contents += ",,Normal,False,Normal,";
                                    contents += Environment.NewLine;
                                }

                            }
                        }

                        System.Text.Encoding ascii = Encoding.Unicode;
                        contents_byte = ascii.GetBytes(contents);
                        stream.Close();
                        using (Stream fs = (Stream)sd.OpenFile())
                        {
                            foreach (byte content in contents_byte)
                            {
                                if (content != 0 && content != 34)
                                    fs.WriteByte(content);
                            }
                            fs.Close();
                        }


                    }
                    else if (Icsextension == ".ics")
                    {

                        String contents = null;
                        byte[] contents_byte;
                        TimeZoneInfo tz = TimeZoneInfo.Local;


                        SaveFileDialog sd = new SaveFileDialog();
                        sd.DefaultExt = ".ics";
                        sd.Filter = "ICalendar|*.ics";
                        sd.FileName = fileName;
                        contents = "BEGIN:VCALENDAR\nPRODID:-//SYNCFUSION//SCHEDULE CONTROL//EN\nVERSION:2.0\nMETHOD:PUBLISH\nX-WR-CALNAME:" + "Schedule" + "_MEETINGS\n\n";


                        foreach (ScheduleAppointment sa in this.Model.Appointments)
                        {

                            contents += "BEGIN:VEVENT\n";
                            if (sa.Location != null)
                            {
                                contents += "LOCATION:";
                                contents += sa.Location + "\n";
                            }
                            if (sa.Subject != null)
                            {
                                contents += "SUMMARY:";
                                contents += sa.Subject + "\n";
                                contents += "DESCRIPTION:";
                                contents += sa.Subject + "\n";
                            }
                            if (!sa.AllDay && !sa.IsRecurrenceAppointment)
                            {

                                contents += "DTSTART";
                                String starttime = String.Format(":{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}Z", sa.StartTime.ToUniversalTime().Year, sa.StartTime.ToUniversalTime().Month, sa.StartTime.ToUniversalTime().Day, sa.StartTime.ToUniversalTime().Hour, sa.StartTime.ToUniversalTime().Minute, sa.StartTime.ToUniversalTime().Second);
                                contents += starttime + "\n";
                                contents += "DTEND";
                                String endtime = String.Format(":{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}Z", sa.EndTime.ToUniversalTime().Year, sa.EndTime.ToUniversalTime().Month, sa.EndTime.ToUniversalTime().Day, sa.EndTime.ToUniversalTime().Hour, sa.EndTime.ToUniversalTime().Minute, sa.EndTime.ToUniversalTime().Second);
                                contents += endtime + "\n";

                            }
                            else
                            {
                                if (sa.AllDay == true)
                                {
                                    contents += "DTSTART;VALUE=DATE:";
                                    contents += sa.StartTime.ToUniversalTime().ToString("yyyyMMdd") + "\n";
                                    contents += "DTEND;VALUE=DATE:";
                                    DateTime endtime = sa.EndTime.AddDays(1);
                                    contents += endtime.ToUniversalTime().ToString("yyyyMMdd") + "\n";
                                }
                                if (sa.IsRecurrenceAppointment == true)
                                {


                                    contents += "DTSTART";
                                    String starttime = String.Format(":{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}Z", sa.StartTime.ToUniversalTime().Year, sa.StartTime.ToUniversalTime().Month, sa.StartTime.ToUniversalTime().Day, sa.StartTime.ToUniversalTime().Hour, sa.StartTime.ToUniversalTime().Minute, sa.StartTime.ToUniversalTime().Second);
                                    contents += starttime + "\n";
                                    contents += "DTEND";
                                    String endtime = String.Format(":{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}Z", sa.EndTime.ToUniversalTime().Year, sa.EndTime.ToUniversalTime().Month, sa.EndTime.ToUniversalTime().Day, sa.EndTime.ToUniversalTime().Hour, sa.EndTime.ToUniversalTime().Minute, sa.EndTime.ToUniversalTime().Second);
                                    contents += endtime + "\n";

                                    //contents += "RRULE:FREQ=" + sa.CurrentRecurrencePatternMode + ";COUNT=" + sa.EndOccurenceCount + "\n";
                                    RecurrenceStringConverter recStrCon = new RecurrenceStringConverter();
                                    contents += recStrCon.ConvertBack(sa, typeof(string), null, System.Globalization.CultureInfo.CurrentCulture) + "\n";
                                }
                            }
                            contents += "END:VEVENT\n\n";
                        }

                        contents += "END:VCALENDAR";

                        System.Text.Encoding ascii = Encoding.Unicode;
                        contents_byte = ascii.GetBytes(contents);
                        stream.Close();
                        using (Stream fs = (Stream)sd.OpenFile())
                        {
                            foreach (byte content in contents_byte)
                            {
                                if (content != 0 && content != 34)
                                    fs.WriteByte(content);
                            }
                            fs.Close();
                        }
                    }
                }
            }
        }
#endif
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.daysView.Dispose();
            this.horizontalView.Dispose();
            this.monthView.Dispose();
            this.Model.Appointments.CollectionChanged -= Appointments_CollectionChanged;
            ReminderTimer.Tick -= ReminderTimer_Tick;
            this.calendarView.Items.Clear();
            this.mainViewItems.Items.Clear();
            this.model.PropertyChanged -= new PropertyChangedEventHandler(OnModelPropertyChanged);
            this.model.AppointmentClick -= new ScheduleAppointmentClickEventHandler(model_OnAppointmentClick);
            this.model.AppointmentDoubleClick -= new ScheduleAppointmentClickEventHandler(model_OnAppointmentDoubleClick);           
            this.model.AppointmentWindowOpened -= new ScheduleAppointmentClickEventHandler(model_OnAppointmentWindowOpened);
            this.model.AppointmentWindowClosed -= new ScheduleAppointmentClickEventHandler(model_OnAppointmentWindowClosed);
            this.model.AppointmentResized -= new ScheduleAppointmentResizedEventHandler(model_AppointmentResized);
            this.model.AppointmentResizing -= new ScheduleAppointmentResizingEventHandler(model_AppointmentResizing);
            this.model.RaiseAppointmentDatesBoldEvent -= new EventHandler(model_SetAppointmentDatesBoldEvent);
            this.model.CalendarAdded -= new ScheduleCalendarAddedEventHandler(model_CalendarAdded);
            this.model.SelectedAppointmentChanged -= new ScheduleSelectedAppointmentEventHandler(model_SelectedAppointmentChanged);
            this.Appointments.Clear();
            this.Holidays.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Holidays_CollectionChanged);            
        }

        #endregion


        //public event PropertyChangedEventHandler PropertyChanged;
    }


    /// <summary>
    ///  Delegate for  schedule calendar added event
    /// </summary>
    /// <param name="e">An <see
    /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleCalendarAddedEventArgs"/>
    /// that contains the event data.</param>
    public delegate void ScheduleCalendarAddedEventHandler(ScheduleCalendarAddedEventArgs e);
    /// <summary>
    ///  Delegate for  schedule appointment resized event
    /// </summary>
    /// <param name="e">An <see
    /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentResizedEventArgs"/>
    /// that contains the event data.</param>
    public delegate void ScheduleAppointmentResizedEventHandler(ScheduleAppointmentResizedEventArgs e);
    /// <summary>
    ///   Delegate for  schedule appointment resizing event
    /// </summary>
    /// <param name="e">An <see
    /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentResizingEventArgs"/>
    /// that contains the event data.</param>
    public delegate void ScheduleAppointmentResizingEventHandler(ScheduleAppointmentResizingEventArgs e);
    /// <summary>
    ///  Delegate for schedule appointment click event
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see
    /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentEventArgs"/>
    /// that contains the event data.</param>
    public delegate void ScheduleAppointmentClickEventHandler(object sender,ScheduleAppointmentEventArgs e);
    /// <summary>
    /// Delegate for schedule appointment cancel event
    /// </summary>
    /// <param name="e">An <see
    /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentCancelEventArgs"/>
    /// that contains the event data.</param>
    public delegate void ScheduleAppointmentCancelEventHandler(ScheduleAppointmentCancelEventArgs e);
    /// <summary>
    ///  Delegate for schedule selected appointment  event
    /// </summary>
    /// <param name="e">An <see
    /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleAppointmentEventArgs"/>
    /// that contains the event data.</param>
    public delegate void ScheduleSelectedAppointmentEventHandler(ScheduleAppointmentEventArgs e);

   

#if SILVERLIGHT
    public class LoadDependentAssemblies
    {

        /// <summary>
        /// Initializes a new instance of the LoadDependentAssemblies class.
        /// </summary>
        public LoadDependentAssemblies()
        {
            System.Windows.Controls.Button button = new System.Windows.Controls.Button();
            System.Windows.Controls.Calendar calender = new System.Windows.Controls.Calendar();
            System.Windows.Controls.DatePicker datePicker = new System.Windows.Controls.DatePicker();
            //System.Windows.Controls.DataGrid dataGrid = new DataGrid();

            datePicker = null;
            button = null;
            calender = null;
            //dataGrid = null;
        }
    }
#endif
/// <summary>
    /// Icommand Class for handling Commands in Context Menu Items.
    /// </summary>
    public class ContextMenuCommand : ICommand
    {
        private Predicate<object> canExecute;
        private Action<object> method;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ContextMenuCommand"/> class.
        /// </summary>
        /// <param name="method"></param>
        public ContextMenuCommand(Action<object> method)
            : this(method, null)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ContextMenuCommand"/> class.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="canExecute"></param>
        public ContextMenuCommand(Action<object> method, Predicate<object> canExecute)
        {
            this.method = method;
            this.canExecute = canExecute;
        }

        public void UpdateCanExecute()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        /// <summary>
        ///  This method return a bool value to determine can execute
        /// </summary>
        /// <param name="parameter"></param>
        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null)
            {
                return true;
            }

            return this.canExecute(parameter);
        }

        /// <summary>
        ///  This method execute the object passes as a parameter
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            this.method.Invoke(parameter);
        }

        /// <summary>
        /// Occurs when canExecute changed
        /// </summary>
        public event EventHandler CanExecuteChanged;

    }

    /// <summary>
    /// Class for Go to Date Window
    /// </summary>
    public class GoToDateWindow : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.GoToDateWindow"/> class.
        /// </summary>
        public GoToDateWindow()
        {
            this.DefaultStyleKey = typeof(GoToDateWindow);
        }

#if SILVERLIGHT
        internal DatePicker PART_GoToDatePicker;
#else  
        internal Syncfusion.Windows.Controls.DatePicker PART_GoToDatePicker;
#endif

        private Button PART_OkButton;
        private Button PART_CancelButton;       
        internal ComboBox PART_ViewCombo;


        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_OkButton = this.GetTemplateChild("PART_OkButton") as Button;
            this.PART_CancelButton = this.GetTemplateChild("PART_CancelButton") as Button;
#if !SILVERLIGHT
          this.PART_GoToDatePicker = this.GetTemplateChild("PART_GoToDatePicker") as Syncfusion.Windows.Controls.DatePicker;
#else
            this.PART_GoToDatePicker = this.GetTemplateChild("PART_GoToDatePicker") as  DatePicker;
#endif
            this.PART_ViewCombo = this.GetTemplateChild("PART_ViewCombo") as ComboBox;

            this.SetupEvents();
            
        }

        /// <summary>
        /// Setups the events.
        /// </summary>
        private void SetupEvents()
        {
            this.PART_OkButton.Click += new RoutedEventHandler(PART_OkButton_Click);
            this.PART_CancelButton.Click += new RoutedEventHandler(PART_CancelButton_Click);
            this.PART_ViewCombo.SelectionChanged += new SelectionChangedEventHandler(PART_ViewCombo_SelectionChanged);
            this.PART_ViewCombo.DropDownClosed += new EventHandler(PART_ViewCombo_DropDownClosed);
        }

        void PART_ViewCombo_DropDownClosed(object sender, EventArgs e)
        {
            this.Focus();            
        }

        void PART_ViewCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Focus();
        }

        /// <summary>
        /// Occurs when [cancel button click].
        /// </summary>
        public event RoutedEventHandler CancelButtonClick;
        private void PART_CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = this.CancelButtonClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when [ok button click].
        /// </summary>
        public event RoutedEventHandler OkButtonClick;
        private void PART_OkButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = this.OkButtonClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    /// <summary>
    /// Class that hold color palette model
    /// </summary>
    public class ColorPaletteModel
        {
            /// <summary>
            /// Gets or sets  ScheduleBackground
            /// </summary>
            public  SolidColorBrush ScheduleBackground { get; set; }
            /// <summary>
            /// Gets or sets  SelectionBackground
            /// </summary>
            public  SolidColorBrush SelectionBackground { get; set; }
            /// <summary>
            /// Gets or sets  HeaderBrush
            /// </summary>
            public  SolidColorBrush HeaderBrush { get; set; }
            /// <summary>
            /// Gets or sets  ShadedBackground
            /// </summary>
            public  SolidColorBrush ShadedBackground { get; set; }
            /// <summary>
            /// Gets or sets  StrokeLine
            /// </summary>
            public  SolidColorBrush StrokeLine { get; set; }
            /// <summary>
            /// Gets or sets  AppointmentBackground
            /// </summary>
            public  LinearGradientBrush AppointmentBackground { get; set; }
        }
   

}
