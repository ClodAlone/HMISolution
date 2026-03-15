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
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows.Threading;
    using System.Windows.Data;
#if SILVERLIGHT
    using Syncfusion.Windows.Shared;
#endif

    /// <summary>
    /// Represents Schedule's Month View.
    /// </summary>
    [StyleTypedProperty(Property = "RecurrenceAlertWindowStyle", StyleTargetType = typeof(ScheduleRecurrenceConfirmationWindow))]
#if !SILVERLIGHT
    [StyleTypedProperty(Property = "AppointmentScheduleWindowStyle", StyleTargetType = typeof(System.Windows.Window))]
#else
    [StyleTypedProperty(Property = "AppointmentScheduleWindowStyle", StyleTargetType = typeof(ChildWindow))]
#endif
 
    [StyleTypedProperty(Property = "AppointmentEditorStyle", StyleTargetType = typeof(ScheduleAppointmentEditorControl))]
    [StyleTypedProperty(Property = "AppointmentStyle", StyleTargetType = typeof(ScheduleMonthAppointmentViewControl))]
  
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleMonthView : Control, IScheduleCalendarViewModelHost, IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleMonthView"/> class.
        /// </summary>
        public ScheduleMonthView()
        {
            this.DefaultStyleKey = typeof(ScheduleMonthView);
            resourceDictionary = new ResourceDictionary();
#if SILVERLIGHT
            resourceDictionary.Source = new Uri("/Syncfusion.Schedule.Silverlight;component/Control/Themes/Generic.xaml",
                               UriKind.RelativeOrAbsolute);
#else
             resourceDictionary.Source = new Uri("/Syncfusion.Schedule.Wpf;component/Control/Themes/Generic.xaml",
                               UriKind.RelativeOrAbsolute);
#endif                      
        }

        private ScheduleCalendarViewModel model;
        private ResourceDictionary resourceDictionary;
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
            internal set
            {
                this.model = value;
                OnPropertyChanged("Model");
            }
        }

        /// <summary>
        /// Sets the calendar view model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void SetCalendarViewModel(ScheduleCalendarViewModel model)
        {
            if (this.model != null)
            {
                this.model.PropertyChanged -= new PropertyChangedEventHandler(model_PropertyChanged);
            }
            this.Model = model;
            this.UpdateModelsToInnerControls();
            this.Model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);
        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedDates" && this.Model.CurrentScheduleType == ScheduleType.Month)
            {
                if (this.monthViewItemsControl != null)
                {
                    this.monthViewItemsControl.GenerateDates();
                    SetCurrentDateBorder();
                }
                this.SetupAppointments();
           
            }
            if (this.model.ShowContextMenu == true)
            {
                if (e.PropertyName == "ContextMenuMonthViewItems")
                {
                    LoadContextMenuMonthViewItems();
                }
                if (e.PropertyName == "ContextMenuMonthViewAppointmentItems")
                {
                    LoadContextMenuMonthViewAppointmentItems();
                }
                if (e.PropertyName == "ContextMenuType")
                {
                    LoadContextMenuMonthViewAppointmentItems();
                    LoadContextMenuMonthViewItems();
                }
            }
            if (e.PropertyName == "ShowContextMenu")
            {
                if (this.model.ShowContextMenu == true && this.model.CurrentScheduleType == ScheduleType.Month)
                {
                    InitializeContextMenu();
                }
                else if (this.model.ShowContextMenu == false && this.model.CurrentScheduleType == ScheduleType.Month)
                {
                    DeInitializeContextMenu();

                }
            }

        }

        //private void UpdatePriority(ScheduleAppointment app)
        //{
        //    if (!isTemplateApplied) return;
        //    var resty = from res in this.monthViewItemsControl.Items.OfType<ScheduleMonthDateContentControl>()
        //                where res.Date >= app.StartTime && res.Date <= app.EndTime
        //                select res;
        //    foreach (ScheduleMonthDateContentControl date in resty)
        //    {
        //        var apps = this.Model.GetCurrentAppointmentsByDate(date.Date) as IEnumerable<ScheduleAppointmentInfo>;
        //        AppointmentPriority priority = AppointmentPriority.Free;
        //        foreach (ScheduleAppointmentInfo appointment in apps)
        //        {
        //            if (appointment == null) continue;
        //            priority = IsFirstIsHigherPrior(priority, appointment.Appointment.Priority) ? priority : appointment.Appointment.Priority;
        //        }
        //        date.Priority = priority;
        //    }
        //}

        //private bool IsFirstIsHigherPrior(AppointmentPriority first, AppointmentPriority second)
        //{
        //    if (first == AppointmentPriority.OutOfOffice) return true;
        //    else if (first == AppointmentPriority.Busy && second != AppointmentPriority.OutOfOffice) return true;
        //    else if (first == AppointmentPriority.Tentative && second != AppointmentPriority.OutOfOffice && second != AppointmentPriority.Busy) return true;
        //    else return false;
        //}

#if Test
        internal ScheduleMonthViewItemsControl monthViewItemsControl;
        internal ScheduleMonthAppointmentLayoutItemsControl monthAppointmentLayoutItemsControl;
      
        internal Popup DragPopUp;
        internal Grid AppointmentPopup;
        internal bool isDragged = false;
        internal bool isDragging = false;
        internal ScheduleAppointment draggedAppointment;
#else
       
        private ScheduleMonthViewItemsControl monthViewItemsControl;
        private ScheduleMonthAppointmentLayoutItemsControl monthAppointmentLayoutItemsControl;
        private ScheduleMonthViewSideItemsControl monthViewSideItemsControl;
        private ScheduleAppointmentNavigatorControl monthappointmentNavigatorControl;
        private ScheduleMonthViewHeaderItemsControl monthViewHeaderItemsControl;
        private Popup DragPopUp;
        private Grid todayBrushGrid;
        private Grid AppointmentPopup;
        private bool isDragged = false;
        private bool isDragging = false;
        private ScheduleAppointment draggedAppointment;
#endif
        private bool isTemplateApplied = false;
private ScheduleMonthDateContentControl elem;

#if SILVERLIGHT
        private ContextMenuAdv ContextMenuMonthViewItemsControl;
        private ContextMenuAdv ContextMenuMonthViewAppointmentLayout;
#else
        private ContextMenu ContextMenuMonthViewItemsControl;
        private ContextMenu ContextMenuMonthViewAppointmentLayout;
#endif

        private ContextMenuCommand allDayAppointment;
        private ContextMenuCommand newAppointment;
        private ContextMenuCommand todayDate;
        private ContextMenuCommand goToDate;
        private ContextMenuCommand newRecurringAppointment;
        private ContextMenuCommand newRecurringEvent;
        private ContextMenuCommand openAppointment;
        private ContextMenuCommand deleteAppointment;
        private ContextMenuCommand showAsCommand;
        private ContextMenuCommand colorButton1;
        private ObservableCollection<object> DefaultContextMenuItemsMonthViewItems = new ObservableCollection<object>();
        private ObservableCollection<object> DefaultContextMenuItemsMonthViewAppointments = new ObservableCollection<object>();

        private void LoadContextMenuMonthViewItems()
        {
            if (ContextMenuMonthViewItemsControl != null)
            {
                if (this.model.ContextMenuType == ContextMenuType.Default)
                {
                    DefaultContextMenuMonthViewItems(); 
                }
                else if (this.model.ContextMenuType == ContextMenuType.Custom)
                {
                    ContextMenuMonthViewItemsControl.Items.Clear();
                    if (this.model.ContextMenuMonthViewItems.Count!=0)
                    {
                        if (this.model.ShowContextMenu == true)
                        {
                            ContextMenuMonthViewItemsControl.Visibility = Visibility.Visible;
                        }
                        CustomContextMenuMonthViewItems();                    }
                    else
                    {
                        ContextMenuMonthViewItemsControl.Visibility = Visibility.Collapsed;
                    }
                }
                else if (this.model.ContextMenuType == ContextMenuType.CustomWithDefault)
                {
                    DefaultContextMenuMonthViewItems();
                    CustomContextMenuMonthViewItems();
                }
            }

        }

        private void LoadContextMenuMonthViewAppointmentItems()
        {
            if (ContextMenuMonthViewAppointmentLayout != null)
            {
                if (this.model.ContextMenuType == ContextMenuType.Default)
                {
                    DefaultContextMenuMonthViewAppointmentItems();
                }
                else if (this.model.ContextMenuType == ContextMenuType.Custom)
                {
                    ContextMenuMonthViewAppointmentLayout.Items.Clear();

                    if (this.model.ContextMenuMonthViewAppointmentItems.Count!=0)
                    {
                        ContextMenuMonthViewAppointmentLayout.Visibility = Visibility.Visible;
                        CustomContextMenuMonthViewAppointmentItems();
                    }
                    else
                    {
                        ContextMenuMonthViewAppointmentLayout.Visibility = Visibility.Collapsed;
                    }
                }
                else if (this.model.ContextMenuType == ContextMenuType.CustomWithDefault)
                {
                    DefaultContextMenuMonthViewAppointmentItems();
                    CustomContextMenuMonthViewAppointmentItems();
                }
            }
        }

        private void DefaultContextMenuMonthViewItems()
        {
            if (ContextMenuMonthViewItemsControl != null)
            {
                ContextMenuMonthViewItemsControl.Visibility = Visibility.Visible;
                ContextMenuMonthViewItemsControl.Items.Clear();
                foreach (var item in DefaultContextMenuItemsMonthViewItems)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        ContextMenuMonthViewItemsControl.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuMonthViewItemsControl.Items.Add(s);
                    }
#else
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        ContextMenuMonthViewItemsControl.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        ContextMenuMonthViewItemsControl.Items.Add(s);
                    }
#endif
                }
            } 
        }

        private void CustomContextMenuMonthViewItems()
        {
            if (this.model.ContextMenuMonthViewItems != null)
            {
                foreach (var item in this.model.ContextMenuMonthViewItems)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        m.Style = (Style)resourceDictionary["ContextMenuItemAdvStyle"];
                        ContextMenuMonthViewItemsControl.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuMonthViewItemsControl.Items.Add(s);
                    }
#else
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        m.Style = (Style)resourceDictionary["ContextMenuItemStyle"];
                        ContextMenuMonthViewItemsControl.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        s.Style = (Style)resourceDictionary["SeparatorStyle"];
                        ContextMenuMonthViewItemsControl.Items.Add(s);
                    }
#endif
                }
            }
        }

        private void DefaultContextMenuMonthViewAppointmentItems()
        {
            if (ContextMenuMonthViewAppointmentLayout != null)
            {
                ContextMenuMonthViewAppointmentLayout.Items.Clear();
                ContextMenuMonthViewAppointmentLayout.Visibility = Visibility.Visible;
                foreach (var item in DefaultContextMenuItemsMonthViewAppointments)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        ContextMenuMonthViewAppointmentLayout.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuMonthViewAppointmentLayout.Items.Add(s);
                    }
#else
                     if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        ContextMenuMonthViewAppointmentLayout.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        ContextMenuMonthViewAppointmentLayout.Items.Add(s);
                    }
#endif
                }
            }
        }

        private void CustomContextMenuMonthViewAppointmentItems()
        {
            if (this.model.ContextMenuMonthViewAppointmentItems != null)
            {

                foreach (var item in this.model.ContextMenuMonthViewAppointmentItems)
                {
#if SILVERLIGHT
                     if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        m.Style = (Style)resourceDictionary["ContextMenuItemAdvStyle"];
                        ContextMenuMonthViewAppointmentLayout.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuMonthViewAppointmentLayout.Items.Add(s);
                    }
#else
                     if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        m.Style = (Style)resourceDictionary["ContextMenuItemStyle"];
                        ContextMenuMonthViewAppointmentLayout.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        s.Style = (Style)resourceDictionary["SeparatorStyle"];
                        ContextMenuMonthViewAppointmentLayout.Items.Add(s);
                    }
#endif
                }
            }
        }
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.DragPopUp = new Popup();
            this.monthViewItemsControl = this.GetTemplateChild("PART_MonthViewItemsControl") as ScheduleMonthViewItemsControl;
            this.monthAppointmentLayoutItemsControl = this.GetTemplateChild("PART_MonthViewAppointmentLayout") as ScheduleMonthAppointmentLayoutItemsControl;
            this.monthViewSideItemsControl = this.GetTemplateChild("PART_MonthViewSideItmesControl") as ScheduleMonthViewSideItemsControl;
            this.monthappointmentNavigatorControl = this.GetTemplateChild("PART_AppointmentNavigatorControl") as ScheduleAppointmentNavigatorControl;
            this.monthViewHeaderItemsControl = this.GetTemplateChild("PART_MonthViewHeaderItemsControl") as ScheduleMonthViewHeaderItemsControl;
            this.MouseMove += new MouseEventHandler(ScheduleMonthView_MouseMove);
            this.AppointmentPopup = this.GetTemplateChild("AppointmentPopUp") as Grid;
            this.todayBrushGrid = this.GetTemplateChild("PART_TodayBrushGrid") as Grid;
            this.isTemplateApplied = true;
            this.UpdateModelsToInnerControls();
            this.SetUpMonthDateContentEvents();
            this.SetCurrentDateBorder();
            this.SetupAppointments();            
            this.SetUpAppointmentEditor();
            DefaultContextMenuItemsMonthViewAppointments = resourceDictionary["DefaultContextMenuItemsMonthViewAppointments"] as ObservableCollection<object>;
            DefaultContextMenuItemsMonthViewItems = resourceDictionary["DefaultContextMenuItemsMonthViewItems"] as ObservableCollection<object>;
            this.monthViewSideItemsControl.DataContext = this;
            if (this.model.ShowContextMenu == true )
            {
                InitializeContextMenu();
            }
            else
            {
                DeInitializeContextMenu();
            }
        }
 private void InitializeContextMenu()
        {
#if SILVERLIGHT
            ContextMenuMonthViewItemsControl = this.GetTemplateChild("ContextMenuMonthViewItemsControl") as ContextMenuAdv;
            ContextMenuMonthViewAppointmentLayout = this.GetTemplateChild("ContextMenuMonthViewAppointmentLayout") as ContextMenuAdv;
#else
            ContextMenuMonthViewItemsControl = this.GetTemplateChild("ContextMenuMonthViewItemsControl") as ContextMenu;            
            ContextMenuMonthViewAppointmentLayout = this.GetTemplateChild("ContextMenuMonthViewAppointmentLayout") as ContextMenu;
#endif
            ContextMenuMonthViewItemsControl.DataContext = this;
            ContextMenuMonthViewAppointmentLayout.DataContext = this;
            LoadContextMenuMonthViewAppointmentItems();
            LoadContextMenuMonthViewItems(); 
        }

        private void DeInitializeContextMenu()
        {
#if SILVERLIGHT
            ContextMenuMonthViewItemsControl = this.GetTemplateChild("ContextMenuMonthViewItemsControl") as ContextMenuAdv;
            ContextMenuMonthViewAppointmentLayout = this.GetTemplateChild("ContextMenuMonthViewAppointmentLayout") as ContextMenuAdv;
#else
            ContextMenuMonthViewItemsControl = this.GetTemplateChild("ContextMenuMonthViewItemsControl") as ContextMenu;            
            ContextMenuMonthViewAppointmentLayout = this.GetTemplateChild("ContextMenuMonthViewAppointmentLayout") as ContextMenu;
#endif
            ContextMenuMonthViewItemsControl.Visibility = Visibility.Collapsed;
            ContextMenuMonthViewAppointmentLayout.Visibility = Visibility.Collapsed;
            ContextMenuMonthViewItemsControl = null;
            ContextMenuMonthViewAppointmentLayout = null;
        }
        private static double oldPositionX;
        private static double oldPositionY;



        /// <summary>
        /// Gets ShowAsCommand from ContextMenuCommand
        /// </summary>
        public ContextMenuCommand ShowAsCommand
        {
            get
            {
                if (showAsCommand == null)
                    showAsCommand = new ContextMenuCommand(ShowAsMethod);
                return showAsCommand;
            }
        }

        private void ShowAsMethod(object parameter)
        {
            string choice = (string)parameter;
            //switch (choice)
            //{
                //case "Free":
                //    this.model.CurrentSelectedAppointment.Priority = AppointmentPriority.Free;
                //    break;
                //case "Tentative":
                //    this.model.CurrentSelectedAppointment.Priority = AppointmentPriority.Tentative;
                //    break;
                //case "Busy":
                //    this.model.CurrentSelectedAppointment.Priority = AppointmentPriority.Busy;
                //    break;
                //case "OutofOffice":
                //    this.model.CurrentSelectedAppointment.Priority = AppointmentPriority.OutOfOffice;
                //    break;
                //default:
                //    break;
            //}
        }
        /// <summary>
        /// Gets ColorButton1 from ContextMenuCommand
        /// </summary>
        public ContextMenuCommand ColorButton1
        {
            get
            {
                if (colorButton1 == null)
                {
                    colorButton1 = new ContextMenuCommand(ColorButton1Method);
                }
                return colorButton1;
            }
        }
        
        internal void ColorButton1Method(object parameter)
        {
            string choice = (string)parameter;
            ContextMenuMonthViewItemsControl.IsOpen = false;
            switch (choice)
            {
                case "ColorButton1":
                   this.model.SetPalette(ColorPalette.ColorButton1);
                   break;
                case "ColorButton2":
                    this.model.SetPalette(ColorPalette.ColorButton2);
                    break;
                case "ColorButton3":
                    this.model.SetPalette(ColorPalette.ColorButton3);
                    break;
                case "ColorButton4":
                    this.model.SetPalette(ColorPalette.ColorButton4);
                    break;
                case "ColorButton5":
                    this.model.SetPalette(ColorPalette.ColorButton5);
                    break;
                case "ColorButton6":
                    this.model.SetPalette(ColorPalette.ColorButton6);
                    break;
                case "ColorButton7":
                    this.model.SetPalette(ColorPalette.ColorButton7);
                    break;
                case "ColorButton8":
                    this.model.SetPalette(ColorPalette.ColorButton8);
                    break;
                case "ColorButton9":
                    this.model.SetPalette(ColorPalette.ColorButton9);
                    break;
                case "ColorButton10":
                    this.model.SetPalette(ColorPalette.ColorButton10);
                    break;
                case "ColorButton11":
                    this.model.SetPalette(ColorPalette.ColorButton11);
                    break;
                case "ColorButton12":
                    this.model.SetPalette(ColorPalette.ColorButton12);
                    break;
                case "ColorButton13":
                    this.model.SetPalette(ColorPalette.ColorButton13);
                    break;
                case "ColorButton14":
                    this.model.SetPalette(ColorPalette.ColorButton14);
                    break;
                case "ColorButton15":
                    this.model.SetPalette(ColorPalette.ColorButton15);
                    break;
                case "Automatic":
                    this.model.SetPalette(ColorPalette.ColorButton1);
                    break;
                default:
                    break; 
            }

        }

        /// <summary>
        /// Gets NewAppointment from ContextMenuCommand
        /// </summary>
        public ContextMenuCommand NewAppointment
        {
            get
            {
                if (newAppointment == null)
                    newAppointment = new ContextMenuCommand(NewAppointmentMethod);

                return newAppointment;
            }
        }
        /// <summary>
        /// Gets AllDayAppointment from ContextMenuCommand
        /// </summary>
        public ContextMenuCommand AllDayAppointment
        {
            get
            {
                if (allDayAppointment == null)
                    allDayAppointment = new ContextMenuCommand(AllDayAppointmentMethod);

                return allDayAppointment;
            }
        }
        /// <summary>
        /// Gets NewRecurringAppointment from ContextMenuCommand
        /// </summary>
        public ContextMenuCommand NewRecurringAppointment
        {
            get
            {
                if (newRecurringAppointment == null)
                    newRecurringAppointment = new ContextMenuCommand(NewRecurringAppointmentMethod);

                return newRecurringAppointment;
            }
        }
        /// <summary>
        /// Gets NewRecurringEvent from ContextMenuCommand
        /// </summary>
        public ContextMenuCommand NewRecurringEvent
        {
            get
            {
                if (newRecurringEvent == null)
                    newRecurringEvent = new ContextMenuCommand(NewRecurringEventMethod);

                return newRecurringEvent;
            }
        }
        /// <summary>
        /// Gets OpenAppointment from ContextMenuCommand
        /// </summary>
        public ContextMenuCommand OpenAppointment
        {
            get
            {
                if (openAppointment == null)
                    openAppointment = new ContextMenuCommand(OpenAppointmentMethod);

                return openAppointment;
            }
        }

        private void OpenAppointmentMethod(object parameter)
        {
            if (!this.Model.AllowEdit)
            {
                return;
            }
            this.ShowWindow(this.Model.CurrentSelectedAppointment, true);
        }
        /// <summary>
        /// Gets DeleteAppointment from ContextMenuCommand
        /// </summary>
        public ContextMenuCommand DeleteAppointment
        {
            get
            {
                if (deleteAppointment == null)
                    deleteAppointment = new ContextMenuCommand(DeleteAppointmentMethod);

                return deleteAppointment;
            }
        }

        private void DeleteAppointmentMethod(object parameter)
        {
            if (!this.model.AllowDelete)
            {
                return;
            }
            this.model.DeleteCurrentSelectedAppointment();
        }

        private void NewAppointmentMethod(object parameter)
        {
            if (elem == null)
                return;
            var startTimeSpan = elem.Date.AddHours(this.model.StartWorkHour);
            var endTimeSpan = elem.Date.AddHours(this.model.StartWorkHour).AddMinutes(30);           
            if (startTimeSpan == DateTime.MinValue || endTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }
            this.model.SelectedStartTimeSpan = startTimeSpan;
            this.model.SelectedEndTimeSpan = endTimeSpan;
            //this.model.SelectedStartTimeSpan = DateTime.MinValue;
            var appointment = new ScheduleAppointment() { StartTime = startTimeSpan, EndTime = endTimeSpan };
            var appWrapper = new ScheduleAppointmentWrapper(appointment, this.model, false);
            this.ShowWindow(appWrapper, Visibility.Collapsed);
            this.model.SelectedStartTimeSpan = elem.Date;
            this.model.SelectedEndTimeSpan = elem.Date;
        }

        private void NewRecurringAppointmentMethod(object parameter)
        {
            if (elem == null)
                return;
            var startTimeSpan = elem.Date.AddHours(this.model.StartWorkHour);
            var endTimeSpan = elem.Date.AddHours(this.model.StartWorkHour).AddMinutes(30);
            this.model.SelectedStartTimeSpan = startTimeSpan;
            this.model.SelectedEndTimeSpan = endTimeSpan;
            if (startTimeSpan == DateTime.MinValue || endTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }
            //this.model.SelectedStartTimeSpan = DateTime.MinValue;
            var appointment = new ScheduleAppointment() { StartTime = startTimeSpan, EndTime = endTimeSpan, IsRecurrenceAppointment = true };
            var appWrapper = new ScheduleAppointmentWrapper(appointment, this.Model, false);
            this.ShowWindow(appWrapper, Visibility.Visible);
            this.model.SelectedStartTimeSpan = elem.Date;
            this.model.SelectedEndTimeSpan = elem.Date;
        }

        private void NewRecurringEventMethod(object parameter)
        {
            if (elem == null)
                return;
            var startTimeSpan = elem.Date;
            var endTimeSpan = elem.Date;
            if (startTimeSpan == DateTime.MinValue || endTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }
            var appointment = new ScheduleAppointment() { StartTime = startTimeSpan, EndTime = endTimeSpan, AllDay = true, IsRecurrenceAppointment = true };
            var appWrapper = new ScheduleAppointmentWrapper(appointment, this.Model, true);
            this.ShowWindow(appWrapper, Visibility.Visible);
        }

        private void AllDayAppointmentMethod(object parameter)
        {
            if (elem == null)
                return;
            var startTimeSpan = elem.Date;
            var endTimeSpan = elem.Date;
            this.model.SelectedStartTimeSpan = startTimeSpan;
            this.model.SelectedEndTimeSpan = endTimeSpan;
            if (startTimeSpan == DateTime.MinValue || endTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }

            var appointment = new ScheduleAppointment() { StartTime = startTimeSpan, EndTime = endTimeSpan, AllDay=true };
            this.ShowWindow(appointment, false);
        }

        /// <summary>
        /// Gets TodayDate command from Contextmenu.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ContextMenuCommand TodayDate
        {
            get
            {
                if (todayDate == null)
                    todayDate = new ContextMenuCommand(TodayDateMethod);
                return todayDate;
            }

        }

        private void TodayDateMethod(object parameter)
        {           
            ScheduleType oldScheduleType = new ScheduleType();
            oldScheduleType = this.model.CurrentScheduleType;
            this.model.SelectedDates.Clear();
            ObservableCollection<DateTime> dates = new ObservableCollection<DateTime>();
            DateTime date = new DateTime();
            date = DateTime.Now.Date;
            DateTime selectedStartedTimespan = this.model.SelectedStartTimeSpan;
            DateTime SelectedEndTimespan = this.model.SelectedEndTimeSpan;
            selectedStartedTimespan = date.AddHours(selectedStartedTimespan.Hour).AddMinutes(selectedStartedTimespan.Minute);
            SelectedEndTimespan = date.AddHours(SelectedEndTimespan.Hour).AddMinutes(SelectedEndTimespan.Minute);
            this.model.SelectedStartTimeSpan = selectedStartedTimespan;
            this.model.SelectedEndTimeSpan = SelectedEndTimespan;

            if (oldScheduleType == ScheduleType.Day)
            {
                dates.Add(date);
                this.model.SelectedDates = dates;
                this.model.CurrentScheduleType = ScheduleType.Day;
            }
            else if (oldScheduleType == ScheduleType.Month)
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
                this.model.CurrentScheduleType = ScheduleType.Month;
            }
            else if (oldScheduleType == ScheduleType.Week)
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
                this.model.CurrentScheduleType = ScheduleType.Week;
            }
            else if (oldScheduleType == ScheduleType.WorkWeek)
            {
                var firstDate = date.StartOfWeek(DayOfWeek.Sunday).AddDays(7);
                var newDates = new ObservableCollection<DateTime>();
                for (int i = 0; i < this.Model.WorkingDays.Count; i++)
                {
                    newDates.Add(firstDate.StartOfWeek(this.Model.WorkingDays[i]));
                }
                this.Model.SelectedDates = newDates;
                this.model.CurrentScheduleType = ScheduleType.WorkWeek;
            }
        }

        /// <summary>
        /// Gets GotoDate command from ContextMenu
        /// </summary>
        public ContextMenuCommand GoToDate
        {
            get
            {
                if (goToDate == null)
                    goToDate = new ContextMenuCommand(GoToDateMethod);

                return goToDate;
            }
        }

        private void GoToDateMethod(object parameter)
        {
            var content = new GoToDateWindow();
            var childwindow = this.GetChildWindow(240);
            ResourceWrapper rw = new ResourceWrapper();
            childwindow.Title = rw.GoToDateWindowHeader;
#if !SILVERLIGHT
            childwindow.Height = 130;
            childwindow.ResizeMode = ResizeMode.NoResize;
            childwindow.WindowStartupLocation = WindowStartupLocation.Manual;
            Schedule s = this.GetScheduleParent();
            Point schedulePoint = s.PointToScreen(new Point());
            childwindow.Left = schedulePoint.X + (s.ActualWidth - childwindow.Width) / 2;
            childwindow.Top = schedulePoint.Y + (s.ActualHeight - childwindow.Height) / 2;  
#endif
            childwindow.Content = content;            
            GoToDateValues value = new GoToDateValues(this.Model);
            this.model.SelectedStartTimeSpan = DateTime.MinValue;
            childwindow.DataContext = value;
            content.CancelButtonClick += (sndr, e) =>
            {
                var editor = sndr as GoToDateWindow;
#if !SILVERLIGHT
                var childWindow = editor.FindParentElementOfType<Window>();
#else
                var childWindow = editor.FindParentElementOfType<ChildWindow>();
#endif
                childWindow.Close();
            };

            content.OkButtonClick += new RoutedEventHandler(content_OkButtonClick);
#if SILVERLIGHT
            childwindow.Show();
#else
            childwindow.ShowDialog();
#endif
        }

        void content_OkButtonClick(object sender, RoutedEventArgs e)
        {
            var editor = sender as GoToDateWindow;
            if (editor.PART_ViewCombo.SelectedItem == null)
            {
                return;
            }
            if (editor.PART_GoToDatePicker.SelectedDate == null)
            {
                return;
            }
#if !SILVERLIGHT
            var childWindow = editor.FindParentElementOfType<Window>();
#else
            var childWindow = editor.FindParentElementOfType<ChildWindow>();
#endif
            ObservableCollection<DateTime> dates = new ObservableCollection<DateTime>();
            this.model.SelectedDates.Clear();
            DateTime date = editor.PART_GoToDatePicker.SelectedDate.Value;
            var s = editor.PART_ViewCombo.SelectedValue.ToString();

            if (s == "Day Calender")
            {
                dates.Add(editor.PART_GoToDatePicker.SelectedDate.Value);
                this.model.SelectedDates = dates;
                this.model.CurrentScheduleType = ScheduleType.Day;
            }
            else if (s == "Month Calender")
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
                this.model.CurrentScheduleType = ScheduleType.Month;
            }
            else if (s== "Week Calender")
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
                this.model.CurrentScheduleType = ScheduleType.Week;
            }
            else if (s== "WorkWeek Calender")
            {
                var firstDate = date.StartOfWeek(DayOfWeek.Sunday).AddDays(7);
                var newDates = new ObservableCollection<DateTime>();
                for (int i = 0; i < this.Model.WorkingDays.Count; i++)
                {
                    newDates.Add(firstDate.StartOfWeek(this.Model.WorkingDays[i]));
                }
                this.Model.SelectedDates = newDates;
                this.model.CurrentScheduleType = ScheduleType.WorkWeek;
            }

            childWindow.Close();
        }

        /// <summary>
        /// Handles the MouseMove event of the ScheduleMonthView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void ScheduleMonthView_MouseMove(object sender, MouseEventArgs e)
        {
            var schedule = (sender as ScheduleMonthView).FindParentElementOfType<Schedule>();
            if (this.isDragging && schedule != null && schedule.SelectedAppointment.AllowDragandDrop)
            {

                double currentVerticalPosition = e.GetPosition(AppointmentPopup).Y;
                double currentHorizontalPosition = e.GetPosition(AppointmentPopup).X;
                if (oldPositionY == 0.0 && oldPositionY == 0.0)
                {
                    oldPositionX = currentVerticalPosition;
                    oldPositionY = currentHorizontalPosition;
                }
                else
                    if (Math.Abs(currentHorizontalPosition - oldPositionX) > 10 && Math.Abs(currentVerticalPosition - oldPositionY) > 10)
                    {
                        if (draggedAppointment.IsRecurrenceAppointment == false && draggedAppointment.IsAppointmentProxyCollection == false)
                        {

                            this.DragPopUp.VerticalOffset = currentVerticalPosition;
                            this.DragPopUp.HorizontalOffset = currentHorizontalPosition;

                            DragPopUp.IsOpen = true;
                            this.isDragged = true;
                        }
                        else
                        {
                            this.isDragging = false;
                            this.AppointmentPopup.Children.Remove(DragPopUp);
                            MessageBox.Show("Cannot Reshedule an occurrance of recurring appointment");
                        }

                    }
            }
        }

        private void SetCurrentDateBorder()
        {
            if (this.todayBrushGrid != null)
            {
                if (this.model.SelectedDates.Contains(DateTime.Now.Date.ToLocalTime()))
                {
                    this.todayBrushGrid.Children.Clear();
                    this.todayBrushGrid.ColumnDefinitions.Clear();
                    int index = this.model.SelectedDates.IndexOf(DateTime.Now.Date);
                    for (int i = 0; i < 7; i++)
                    {
                        this.todayBrushGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    }
                    this.todayBrushGrid.RowDefinitions.Clear();
                    int rowcount;
                    if (this.model.SelectedDates.Count % 7 == 0)
                    {
                        rowcount = this.model.SelectedDates.Count / 7;
                    }
                    else
                    {
                        rowcount = (this.model.SelectedDates.Count / 7) + 1; 
                    }
                    for (int i = 0; i < rowcount; i++)
                    {
                        this.todayBrushGrid.RowDefinitions.Add(new RowDefinition());
                    }
                    int rowindex = index / 7;
                    int columnindex = index % 7;
                    Border rect = new Border();
                    rect.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xEB, 0x89, 0x00));
                    rect.BorderThickness = new Thickness(.5,.5,.5,0);
#if SILVERLIGHT
                    rect.Margin = new Thickness(0, 0, -2, 0);
#endif
                    Grid.SetColumn(rect, columnindex);
                    Grid.SetRow(rect, rowindex);
                    this.todayBrushGrid.Children.Add(rect);
                }
                else
                {               
                    this.todayBrushGrid.Children.Clear();
                    this.todayBrushGrid.ColumnDefinitions.Clear();
                    this.todayBrushGrid.RowDefinitions.Clear();
                }
            }
        }

        private Schedule GetScheduleParent()
        {
            UIElement obj = this;
            while (typeof(Schedule) != obj.GetType())
            {
                Schedule s = obj as Schedule;
                if (s == null)
                {
                    obj = VisualTreeHelper.GetParent(obj) as UIElement;
                }
                else
                {
                    obj = s;
                    return obj as Schedule;
                }
            }
            return obj as Schedule;
        }



        /// <summary>
        /// Updates the models to inner controls.
        /// </summary>
        private void UpdateModelsToInnerControls()
        {
            if (!this.isTemplateApplied || this.model == null || this.Model.CurrentScheduleType != ScheduleType.Month)
            {
                return;
            }
            //this.monthAppointmentLayoutItemsControl.SetCalendarViewModel(this.Model);
            this.monthAppointmentLayoutItemsControl.SetCalendarViewModel(this.model);
            this.monthViewHeaderItemsControl.SetCalendarViewModel(this.model);
            this.monthViewItemsControl.SetCalendarViewModel(this.model);
            this.monthViewSideItemsControl.SetCalendarViewModel(this.Model);
            this.monthappointmentNavigatorControl.SetCalendarViewModel(this.Model);
        }

        /// <summary>
        /// Sets up month date content events.
        /// </summary>
        private void SetUpMonthDateContentEvents()
        {
            //newly added
            this.monthViewItemsControl.MouseRightClick += new MouseButtonEventHandler(monthViewItemsControl_MouseRightButtonDown);
            this.monthAppointmentLayoutItemsControl.MouseRightClick+=new MouseButtonEventHandler(monthAppointmentLayoutItemsControl_MouseRightButtonDown);
            this.monthViewItemsControl.MouseLeftButtonDown += new MouseButtonEventHandler(monthViewItemsControl_MouseLeftButtonDown);
            this.monthViewItemsControl.MouseLeftButtonUp += new MouseButtonEventHandler(monthViewItemsControl_MouseLeftButtonUp);
            this.monthAppointmentLayoutItemsControl.MouseLeftButtonDown += new MouseButtonEventHandler(monthAppointmentLayoutItemsControl_MouseLeftButtonDown);
            this.monthAppointmentLayoutItemsControl.MouseMove += new MouseEventHandler(monthAppointmentLayoutItemsControl_MouseMove);
            this.monthAppointmentLayoutItemsControl.MouseLeftButtonUp += new MouseButtonEventHandler(monthAppointmentLayoutItemsControl_MouseLeftButtonUp);
            this.monthViewSideItemsControl.MouseLeftButtonDown += new MouseButtonEventHandler(monthViewSideItemsControl_MouseLeftButtonDown);
            
        }

 //newly added
        void monthViewItemsControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            elem = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleMonthDateContentControl>().FirstOrDefault();
            var itms = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<Rectangle>();
            //if (itms.Count() > 0 && itms != null && itms.FirstOrDefault().Name == "DateRectangle")
            //{
            //    if (elem != null)
            //    {
            //        this.Model.GoToSelectedStartTime(elem.Date);                    
            //    }
            //}
#else
            elem = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleMonthDateContentControl") as ScheduleMonthDateContentControl;
            Rectangle itms = this.InputHitTest(e.GetPosition(this)) as Rectangle;
            //if (itms != null && itms.Name == "DateRectangle")
            //{
            //    if (elem != null)
            //    {
            //        this.Model.GoToSelectedStartTime(elem.Date);
            //    }
            //}
            //else
            //{
            //    TextBlock rct = this.InputHitTest(e.GetPosition(this)) as TextBlock;
            //    if (rct != null && rct.Name == "Date")
            //    {
            //        if (elem != null)
            //        {
            //            this.Model.GoToSelectedStartTime(elem.Date);
            //        }
            //    }
            //}
#endif

            if (elem == null)
            {
                return;
            }
            this.Model.CurrentSelectedAppointment = null;
            this.ClearPreviousDateContentSelection();
            this.ClearPreviousAppointmentSelection();
            if (elem.IsCurrentMonth)
            {
                elem.IsSelected = true;
            }      
            
            this.model.SelectedStartTimeSpan = elem.Date;
        }
        
        void monthAppointmentLayoutItemsControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            FrameworkElement sndr = sender as FrameworkElement;
            sndr.CaptureMouse();
            ScheduleMonthAppointmentViewControl el = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleMonthAppointmentViewControl>().FirstOrDefault();
#else
            ScheduleMonthAppointmentViewControl el = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleMonthAppointmentViewControl") as ScheduleMonthAppointmentViewControl;
            Border elbdrtop = GetHitTestParentElementByName(this.InputHitTest(e.GetPosition(this)), "ScheduleMonthAppointmentViewControl", "PART_LeftEdge") as Border;
            Border elbdrbottom = GetHitTestParentElementByName(this.InputHitTest(e.GetPosition(this)), "ScheduleMonthAppointmentViewControl", "PART_RightEdge") as Border;
#endif
            if (el == null) return;

            if (this.Model != null)
            {
                ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(el.ScheduleAppointment);
                this.Model.GetAppointmentClickEvents(args);
            }
            StartingPoint = e.GetPosition(el);
            this.ClearAppointmentPopupWindow();
            this.ClearPreviousDateContentSelection();
            this.ClearPreviousAppointmentSelection();
            el.IsSelected = true;
            this.Model.CurrentSelectedAppointment = el.ScheduleAppointment;
            SelectAppointmentProxyCollection(el.ScheduleAppointment);           
        }

        private void monthViewSideItemsControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

#if SILVERLIGHT
            var items = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleMonthViewSideContentControl>();
            if (items.Count() <= 0 || items == null) return;         
            else
            {
                var item = items.FirstOrDefault() as ScheduleMonthViewSideContentControl;              
                this.Model.SelectedDates = item.Dates;
            }
#else
            //ScheduleMonthViewSideContentControl item = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleMonthViewSideContentControl") as ScheduleMonthViewSideContentControl;
            //if (item != null)
            //{
            //    this.Model.SelectedDates = item.Dates;
            //}
            //ScheduleSideTextBlock item = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleMonthViewSideContentControl") as ScheduleSideTextBlock;
            //ScheduleMonthViewSideContentControl sideVieContent = item.TemplatedParent as ScheduleMonthViewSideContentControl;
            //if (sideVieContent != null)
            //{
            //    this.Model.SelectedDates = sideVieContent.Dates;
            //}

            ScheduleMonthViewSideContentControl item = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleMonthViewSideContentControl") as ScheduleMonthViewSideContentControl;
            if (item != null)
            {
                this.Model.SelectedDates = item.Dates;
            }
            ScheduleSideTextBlock textBlock = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleMonthViewSideContentControl") as ScheduleSideTextBlock;
            if (textBlock != null)
            {
                ScheduleMonthViewSideContentControl sideVieContent = textBlock.TemplatedParent as ScheduleMonthViewSideContentControl;
                if (sideVieContent != null)
                {
                    this.Model.SelectedDates = sideVieContent.Dates;
                }
            }
#endif
        }

       
        /// <summary>
        /// 
        /// </summary>
        ScheduleAppointmentResizingEventArgs moveResizeargs;
        private int timeInterval;

        /// <summary>
        /// Handles the MouseMove event of the monthAppointmentLayoutItemsControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void monthAppointmentLayoutItemsControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDaysAppMouseDown || this.Model.AllowResize == false || mouseDownDaysAppEl == null) return;

            if (moveResizeargs != null && moveResizeargs.Cancel == true) return;
#if SILVERLIGHT
            ScheduleMonthViewItemsControl elrep = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleMonthViewItemsControl>().FirstOrDefault();
#else
            ScheduleMonthViewItemsControl elrep = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleMonthViewItemsControl") as ScheduleMonthViewItemsControl;
#endif
            if (elrep == null)
            {
                isResizeStopped = true;
                return;
            }

            var startTempTime = new DateTime();
            var endTempTime = new DateTime();
            Point curroffset = e.GetPosition(mouseDownDaysAppEl);
            if ((curroffset.X == 0 && curroffset.Y == 0) || mouseDownDaysAppEl.ScheduleAppointment.AllowResize == false) return;

            double diffWidth = curroffset.X - StartingPoint.X;
            if (diffWidth != 0) isallowResized = true;

            if (mouseDownDaysAppEl.DragStatus == MousePointerType.Resize && mouseDownDaysAppEl.MousePosition == ResizePosition.Right && diffWidth != 0)
            {
                if (popupResize != null)
                {
                    var elementApp = popupResize.Child as FrameworkElement;
                    if (elementApp.Width + diffWidth <= 0) return;
                    elementApp.Width += diffWidth;
                }
            }
            else if (mouseDownDaysAppEl.DragStatus == MousePointerType.Resize && mouseDownDaysAppEl.MousePosition == ResizePosition.Left && diffWidth != 0)
            {
                if (popupResize != null)
                {
                    var elementApp = popupResize.Child as FrameworkElement;
                    if (elementApp.Width + diffWidth <= 0 || elementApp.Width - diffWidth <= 0) return;
                    elementApp.Width -= diffWidth;
#if SILVERLIGHT
                    popupResize.Margin = new Thickness(popupResize.Margin.Left + diffWidth, popupResize.Margin.Top, 0, 0);
#else
                    double leftval = (popupResize.Child as FrameworkElement).Margin.Left;
                    double topval = (popupResize.Child as FrameworkElement).Margin.Top;
                    (popupResize.Child as FrameworkElement).Margin = new Thickness(leftval + diffWidth, topval, 0, 0);
#endif
                }
            }

            if (popupResize != null && mouseDownDaysAppEl.DragStatus == MousePointerType.Resize)
            {
                var elementApp = popupResize.Child as FrameworkElement;
                if (double.IsNaN(elementApp.Width))
                    elementApp.Width = elementApp.ActualWidth;
                resizedWidth = elementApp.Width;
            }

            if (this.Model != null)
            {
                int currCount = Convert.ToInt32(Math.Floor(resizedWidth / actualAppWidth));
                double totmin = timeInterval * currCount;
                if (mouseDownDaysAppEl.MousePosition == ResizePosition.Right)
                {
                    endTempTime = new DateTime(mouseDownDaysAppEl.ScheduleAppointment.StartTime.Year, mouseDownDaysAppEl.ScheduleAppointment.StartTime.Month, mouseDownDaysAppEl.ScheduleAppointment.StartTime.Day).AddDays(currCount - 1);
                }
                else endTempTime = mouseDownDaysAppEl.ScheduleAppointment.EndTime;
                if (mouseDownDaysAppEl.MousePosition == ResizePosition.Left)
                {
                    startTempTime = new DateTime(mouseDownDaysAppEl.ScheduleAppointment.EndTime.Year, mouseDownDaysAppEl.ScheduleAppointment.EndTime.Month, mouseDownDaysAppEl.ScheduleAppointment.EndTime.Day).AddDays(-(currCount - 1));
                }
                else startTempTime = mouseDownDaysAppEl.ScheduleAppointment.StartTime;
                moveResizeargs = new ScheduleAppointmentResizingEventArgs(startTempTime, endTempTime);
                this.Model.GetAppointmentResizingEvents(moveResizeargs);
            }

            StartingPoint = curroffset;
        }

        private double resizedWidth = 0d;
        private bool isResizeStopped = false;

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the monthAppointmentLayoutItemsControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void monthAppointmentLayoutItemsControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.dropAppointment(e);
            if (popupResize != null && !isResizeStopped)
            {
                var elementApp = popupResize.Child as FrameworkElement;
                if (double.IsNaN(elementApp.Width))
                    elementApp.Width = elementApp.ActualWidth;
                resizedWidth = elementApp.Width;
            }
            this.ClearAppointmentPopupWindow();
            if (isallowResized && isDaysAppMouseDown && mouseDownDaysAppEl != null &&
                this.Model.AllowResize == true && mouseDownDaysAppEl.ScheduleAppointment.AllowResize == true)
            {
#if SILVERLIGHT
                FrameworkElement sendr = sender as FrameworkElement;
                sendr.ReleaseMouseCapture();
#endif
                ResizedAppointment();
            }
            this.SetDefaultValuesForResize();
        }

        private void ResizedAppointment()
        {
            var startTime = mouseDownDaysAppEl.ScheduleAppointment.StartTime;
            var endTime = mouseDownDaysAppEl.ScheduleAppointment.EndTime;

            if (actualAppWidth > 0 && actualElemntWidth > 0 && mouseDownDaysAppEl.DragStatus == MousePointerType.Resize
                && mouseDownDaysAppEl.MousePosition != ResizePosition.None)
            {
                double currentwidth = resizedWidth + ((timeInterval * actualAppWidth) - actualElemntWidth);
                int currCount = Convert.ToInt32(Math.Floor(currentwidth / actualAppWidth));
                double remaniner = actualAppWidth - (currentwidth % actualAppWidth);
                if (isResizeStopped) remaniner = 0;
                currCount = (remaniner > 0) ? currCount + 1 : currCount;

                var remApp = (from res in this.Model.Appointments
                              where res.MatchWithExists(this.Model.CurrentSelectedAppointment) == true
                              select res).FirstOrDefault();
                if (mouseDownDaysAppEl.ScheduleAppointment.AllDay == false)
                {
                    mouseDownDaysAppEl.ScheduleAppointment.AllDay = true;
                    mouseDownDaysAppEl.ScheduleAppointment.StartTime = new DateTime(mouseDownDaysAppEl.ScheduleAppointment.StartTime.Year, mouseDownDaysAppEl.ScheduleAppointment.StartTime.Month, mouseDownDaysAppEl.ScheduleAppointment.StartTime.Day);
                    mouseDownDaysAppEl.ScheduleAppointment.EndTime = new DateTime(mouseDownDaysAppEl.ScheduleAppointment.EndTime.Year, mouseDownDaysAppEl.ScheduleAppointment.EndTime.Month, mouseDownDaysAppEl.ScheduleAppointment.EndTime.Day);
                }
                if (mouseDownDaysAppEl.MousePosition == ResizePosition.Right)
                {
                    mouseDownDaysAppEl.ScheduleAppointment.EndTime = new DateTime(mouseDownDaysAppEl.ScheduleAppointment.StartTime.Year, mouseDownDaysAppEl.ScheduleAppointment.StartTime.Month, mouseDownDaysAppEl.ScheduleAppointment.StartTime.Day).AddDays(currCount - 1);
                }
                else if (mouseDownDaysAppEl.MousePosition == ResizePosition.Left)
                {
                    mouseDownDaysAppEl.ScheduleAppointment.StartTime = new DateTime(mouseDownDaysAppEl.ScheduleAppointment.EndTime.Year, mouseDownDaysAppEl.ScheduleAppointment.EndTime.Month, mouseDownDaysAppEl.ScheduleAppointment.EndTime.Day).AddDays(-(currCount - 1));
                }

                if (mouseDownDaysAppEl.ScheduleAppointment.StartTime > mouseDownDaysAppEl.ScheduleAppointment.EndTime)
                {
                    mouseDownDaysAppEl.ScheduleAppointment.EndTime = mouseDownDaysAppEl.ScheduleAppointment.StartTime.AddDays(timeInterval);
                }

                var app = mouseDownDaysAppEl.ScheduleAppointment;

                this.monthAppointmentLayoutItemsControl.Items.Remove(remApp);
                if (app.AllDay == false)
                {
                    app.AllDay = true;
                }
                this.monthAppointmentLayoutItemsControl.Items.Add(app);
                if (app.Record != null)
                {
                    var record = draggedAppointment.Record;
                    this.Model.View.EditItem(record.Data);
                    var pd = this.Model.View.GetPropertyAccessProvider();
                    this.Model.SetPropertiesOnNewItem(pd, record.Data,app);
                    this.Model.View.CommitEdit();
                }
                
                 //this.Model.Appointments.Remove(remApp);

                //var addApp = new ScheduleAppointment();
                //addApp.InitializeFromApp(app);
                //if (addApp.AllDay == false)
                //{
                //    addApp.AllDay = true;
                //}
                //this.Model.Appointments.Add(addApp);
                //this.UpdateLayout();
                foreach (var item in this.monthAppointmentLayoutItemsControl.Items)
                {
                    var viewControl = this.monthAppointmentLayoutItemsControl.ItemContainerGenerator.ContainerFromItem(item) as ScheduleMonthAppointmentViewControl;
                    if (viewControl != null && viewControl.ScheduleAppointment.MatchWithExists(app) == true)
                    {
                        viewControl.IsSelected = true;
                        break;
                    }
                }
            }

            if (this.Model != null)
            {
                ScheduleAppointmentResizedEventArgs args = new ScheduleAppointmentResizedEventArgs(draggedAppointment,startTime, endTime);
                this.Model.GetAppointmentResizedEvents(args);
            }
        }

        /// <summary>
        /// Sets the default values for resize.
        /// </summary>
        private void SetDefaultValuesForResize()
        {
            StartingPoint = new Point();
            isResizeStopped = false;
            actualAppWidth = 0;
            actualElemntWidth = 0;
            popupResize = null;
            moveResizeargs = null;
            isDaysAppMouseDown = false;
            isallowResized = false;
            if (mouseDownDaysAppEl != null)
            {
                mouseDownDaysAppEl.DragStatus = MousePointerType.None;
                mouseDownDaysAppEl.MousePosition = ResizePosition.None;
                mouseDownDaysAppEl = null;
            }
        }

#if !SILVERLIGHT
        internal ScheduleAppointment GetTimeIntevalFromMousePoint(RoutedEventArgs e)
        {          
            
#if SILVERLIGHT
                
#else
                ScheduleMonthView v = FindAnchestor<ScheduleMonthView>((DependencyObject)e.OriginalSource);
#endif
                if (v != null)
                {

#if SILVERLIGHT
                    ScheduleMonthDateContentControl monthContentControl = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleMonthDateContentControl>().FirstOrDefault();
#else
                    ScheduleMonthDateContentControl monthContentControl = FindAnchestor<ScheduleMonthDateContentControl>((DependencyObject)e.OriginalSource);
#endif
                    if (monthContentControl != null)
                    {
                        DateTime appStartTime = monthContentControl.Date;
                        ScheduleAppointment newapp = new ScheduleAppointment();                        
                        newapp.StartTime = appStartTime;
                        newapp.EndTime = appStartTime.Add(TimeIntervalToTimeSpanConverter(this.model.CurrentTimeInterval));
                        newapp.AllDay = true;
                        return newapp;
                        
                    }

                }           
            return null;
        }
#endif

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the monthViewItemsControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void monthViewItemsControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.dropAppointment(e);
        }

        /// <summary>
        /// Drops the appointment.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void dropAppointment(MouseButtonEventArgs e)
        {
            if (this.isDragged)
            {
#if SILVERLIGHT
                ScheduleMonthView v = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleMonthView>().FirstOrDefault();
#else
                ScheduleMonthView v = FindAnchestor<ScheduleMonthView>((DependencyObject)e.OriginalSource);
#endif
                if (v != null)
                {

#if SILVERLIGHT
                    ScheduleMonthDateContentControl monthContentControl = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleMonthDateContentControl>().FirstOrDefault();
#else
                    ScheduleMonthDateContentControl monthContentControl = FindAnchestor<ScheduleMonthDateContentControl>((DependencyObject)e.OriginalSource);
#endif
                    if (monthContentControl != null)
                    {
                        DateTime appStartTime = monthContentControl.Date;
                        ScheduleAppointment newapp = new ScheduleAppointment();
                        //newapp.InitializeFromApp(this.draggedAppointment);
                        newapp.StartTime = appStartTime;
                        newapp.EndTime = appStartTime.Add(draggedAppointment.Duration);
                        newapp.AllDay = true;
                        Schedule sch = GetScheduleParent();
                        sch.OnAppointmentDropping(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                        this.monthAppointmentLayoutItemsControl.Items.Remove(this.draggedAppointment);
                        this.draggedAppointment.StartTime = newapp.StartTime;
                        this.draggedAppointment.EndTime = newapp.EndTime;
                        this.draggedAppointment.AllDay = true;
                        this.monthAppointmentLayoutItemsControl.Items.Add(this.draggedAppointment);
                        if (this.draggedAppointment.Record != null)
                        {
                            var record = draggedAppointment.Record;
                            this.Model.View.EditItem(record.Data);
                            var pd = this.Model.View.GetPropertyAccessProvider();
                            this.Model.SetPropertiesOnNewItem(pd, record.Data, this.draggedAppointment);
                            this.Model.View.CommitEdit();
                        }
                        sch.OnAppointmentDropped(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                        //this.Model.Appointments.Remove(draggedAppointment);
                        //this.Model.Appointments.Add(newapp);
                        //this.UpdateLayout();
                    }

                }

            }
            this.AppointmentPopup.Children.Remove(DragPopUp);
            this.DragPopUp.IsOpen = false;
            this.isDragging = false;
            this.isDragged = false;
        }

        internal TimeSpan TimeIntervalToTimeSpanConverter(TimeInterval interval)
        {
            TimeSpan retvalue = new TimeSpan();
            if (interval == TimeInterval.FiveMin)
                retvalue = new TimeSpan(0, 5, 0);
            else if (interval == TimeInterval.SixMin)
                retvalue = new TimeSpan(0, 6, 0);
            else if (interval == TimeInterval.TenMin)
                retvalue = new TimeSpan(0, 10, 0);
            else if (interval == TimeInterval.FifteenMin)
                retvalue = new TimeSpan(0, 15, 0);
            else if (interval == TimeInterval.TwentyMin)
                retvalue = new TimeSpan(0, 20, 0);
            else if (interval == TimeInterval.ThirtyMin)
                retvalue = new TimeSpan(0, 30, 0);
            else if (interval == TimeInterval.OneHour)
                retvalue = new TimeSpan(1, 0, 0);
            return retvalue;
        }


        /// <summary>
        /// Picks up appointment.
        /// </summary>
        /// <param name="el">The el.</param>
        private void PickUpAppointment(ScheduleMonthAppointmentViewControl el)
        {
            if (isDragging == false && isDragged == false)
            {
                this.draggedAppointment = el.DataContext as ScheduleAppointment;
                if (this.Model.AllowDragAndDrop)
                {
                    this.AppointmentPopup.Children.Add(DragPopUp);
                    ScheduleMonthAppointmentViewControl re = new ScheduleMonthAppointmentViewControl();
                    re.DataContext = this.draggedAppointment;
                    re.Height = this.Model.IntervalHeight;
                    re.Width = this.ActualWidth / 7;
                    re.Opacity = 0.5;
                    DragPopUp.Child = re;
                    this.isDragging = true;
                }
            }
        }

        /// <summary>
        /// Finds the anchestor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current">The current.</param>
        /// <returns></returns>
        private T FindAnchestor<T>(DependencyObject current)
          where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        ScheduleMonthAppointmentViewControl mouseDownDaysAppEl = null;
        private bool isDaysAppMouseDown = false;
        private bool isallowResized = false;
        private Point StartingPoint;
        private double actualAppWidth = 0;
        private double actualElemntWidth = 0;
        private Popup popupResize;

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the monthAppointmentLayoutItemsControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void monthAppointmentLayoutItemsControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            FrameworkElement sndr = sender as FrameworkElement;
            sndr.CaptureMouse();
            ScheduleMonthAppointmentViewControl el = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleMonthAppointmentViewControl>().FirstOrDefault();
#else
            ScheduleMonthAppointmentViewControl el = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleMonthAppointmentViewControl") as ScheduleMonthAppointmentViewControl;
            Border elbdrtop = GetHitTestParentElementByName(this.InputHitTest(e.GetPosition(this)), "ScheduleMonthAppointmentViewControl", "PART_LeftEdge") as Border;
            Border elbdrbottom = GetHitTestParentElementByName(this.InputHitTest(e.GetPosition(this)), "ScheduleMonthAppointmentViewControl", "PART_RightEdge") as Border;
#endif
            if (el == null) return;

            if (this.Model != null)
            {
                ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(el.ScheduleAppointment);
                this.Model.GetAppointmentClickEvents(args);
            }
            StartingPoint = e.GetPosition(el);
            this.ClearAppointmentPopupWindow();          
            this.ClearPreviousDateContentSelection();
            this.ClearPreviousAppointmentSelection();
            el.IsSelected = true;
            this.Model.CurrentSelectedAppointment = el.ScheduleAppointment;
            SelectAppointmentProxyCollection(el.ScheduleAppointment);
#if SILVERLIGHT
            el.CaptureMouse();
#endif

#if !SILVERLIGHT
            if (elbdrbottom != null)
            {
                el.DragStatus = MousePointerType.Resize;
                el.MousePosition = ResizePosition.Right;
            }
            else if (elbdrtop != null)
            {
                el.DragStatus = MousePointerType.Resize;
                el.MousePosition = ResizePosition.Left;
            }
            else
            {
                el.DragStatus = MousePointerType.None;
                el.MousePosition = ResizePosition.None;
            }
#endif
            mouseDownDaysAppEl = el;

            timeInterval = mouseDownDaysAppEl.ScheduleAppointment.Duration.Days;
            if (mouseDownDaysAppEl.ScheduleAppointment.IsRecurrenceAppointment == true || mouseDownDaysAppEl.ScheduleAppointment.CurrentAppointmentType == AppointmentType.MultiWeek) return;

            if (el.DragStatus == MousePointerType.Resize && this.Model.AllowResize == true && el.ScheduleAppointment.AllowResize == true)
            {
                Point currpopMargin = e.GetPosition(sender as FrameworkElement);

                popupResize = GetPopUpWithContent(el);
#if SILVERLIGHT
                popupResize.Margin = new Thickness(currpopMargin.X - StartingPoint.X, currpopMargin.Y - StartingPoint.Y, 0, 0);
#else
                try
                {
                    Point absoluteScreenPos = PointToScreen(Mouse.GetPosition(this));
                    Point StartingPointEle = e.GetPosition(el);
                    (popupResize.Child as Border).Margin = new Thickness(absoluteScreenPos.X - StartingPointEle.X, absoluteScreenPos.Y - StartingPointEle.Y, 0, 0);
                    popupResize.IsOpen = true; 
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debugger.Break();
                    throw(ex);
                }
                //Application.Current.MainWindow.UpdateLayout();
                //Point StartingPointEle = e.GetPosition(el);
                //Point MainWindowPoint = e.GetPosition(Application.Current.MainWindow);
                //double leftValue = MainWindowPoint.X + Application.Current.MainWindow.Left - StartingPointEle.X + 8;
                //double topValue = MainWindowPoint.Y + Application.Current.MainWindow.Top - StartingPointEle.Y + 30;
                //(popupResize.Child as Border).Margin = new Thickness(leftValue, topValue, 0, 0);
                //popupResize.IsOpen = true;
#endif
                this.AppointmentPopup.Children.Add(popupResize);

                isDaysAppMouseDown = true;
                mouseDownDaysAppEl = el;

                if (double.IsNaN(mouseDownDaysAppEl.Width))
                    actualElemntWidth = mouseDownDaysAppEl.ActualWidth;
                else
                    actualElemntWidth = mouseDownDaysAppEl.Width;

                var parentelem = VisualTreeHelper.GetParent(el) as ScheduleMonthAppointmentLayoutPanel;

                actualAppWidth = (parentelem != null) ? parentelem.ActualWidth / 7 : actualElemntWidth;

                timeInterval = ((timeInterval >= 0) ? timeInterval : 0) + 1;
                //actualAppWidth = actualAppWidth * timeInterval;
            }
            else
            {
                this.PickUpAppointment(el);
            }
        }

        /// <summary>
        /// Clears the appointment popup window.
        /// </summary>
        private void ClearAppointmentPopupWindow()
        {
            if (popupResize != null)
            {
                popupResize.IsOpen = false;
                popupResize = null;
            }
            this.AppointmentPopup.Children.Clear();
        }

        /// <summary>
        /// Gets the content of the pop up with.
        /// </summary>
        /// <param name="mouseDownDaysAppEl">The mouse down days app el.</param>
        /// <returns></returns>
        private Popup GetPopUpWithContent(ScheduleMonthAppointmentViewControl mouseDownDaysAppEl)
        {
            double popwidth = double.IsNaN(mouseDownDaysAppEl.Width) ? mouseDownDaysAppEl.ActualWidth : mouseDownDaysAppEl.Width;
            double popheight = double.IsNaN(mouseDownDaysAppEl.Height) ? mouseDownDaysAppEl.ActualHeight : mouseDownDaysAppEl.Height;
#if SILVERLIGHT
            var popup = new Popup()
            {
                IsOpen = true
            };
            popup.MouseMove += new MouseEventHandler(monthAppointmentLayoutItemsControl_MouseMove);
            popup.MouseLeftButtonUp += new MouseButtonEventHandler(monthAppointmentLayoutItemsControl_MouseLeftButtonUp);
            var popupChild = new Border()
            {
                Width = popwidth,
                Height = popheight,
                CornerRadius = new CornerRadius(2),
                Background = new SolidColorBrush(Colors.Gray),
                Opacity = 0.5
            };
#else
            var popup = new Popup()
            {
                Placement = PlacementMode.Custom,
                AllowsTransparency = true,
                StaysOpen = false
            };
            popup.MouseMove += new MouseEventHandler(monthAppointmentLayoutItemsControl_MouseMove);
            popup.MouseLeftButtonUp += new MouseButtonEventHandler(monthAppointmentLayoutItemsControl_MouseLeftButtonUp);
            var popupChild = new Border()
            {
                Width = popwidth,
                Height = popheight,
                CornerRadius = new CornerRadius(2),
                Background = new SolidColorBrush(Colors.Gray),
                Opacity = 0.5,
                VerticalAlignment = VerticalAlignment.Center
            };
#endif
            popup.Child = popupChild;
            return popup;
        }

        /// <summary>
        /// Selects the appointment proxy collection.
        /// </summary>
        /// <param name="app">The app.</param>
        private void SelectAppointmentProxyCollection(ScheduleAppointment app)
        {
            if (app.IsRecurrenceAppointment == true) return;

            var selectdapp = from ap in this.Model.AppointmentProxy
                             where ap.AppointmentProxy == app
                             from res in this.Model.AppointmentProxy
                             where res.ParentAppointment == ap.ParentAppointment
                             select res;

            foreach (var appItem in selectdapp)
            {
                var viewControl = this.monthAppointmentLayoutItemsControl.ItemContainerGenerator.ContainerFromItem(appItem.AppointmentProxy) as ScheduleMonthAppointmentViewControl;
                if (viewControl != null)
                {
                    viewControl.IsSelected = true;
                    this.Model.CurrentSelectedAppointment = appItem.ParentAppointment;
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the monthViewItemsControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void monthViewItemsControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            ScheduleMonthDateContentControl el = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleMonthDateContentControl>().FirstOrDefault();
            var itms = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<Rectangle>();
            if (itms.Count() > 0 && itms != null && itms.FirstOrDefault().Name == "DateRectangle")
            {
                if (el != null)
                {
                    this.Model.GoToSelectedStartTime(el.Date);
                }
            }
#else
            ScheduleMonthDateContentControl el = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleMonthDateContentControl") as ScheduleMonthDateContentControl;
            Rectangle itms = this.InputHitTest(e.GetPosition(this)) as Rectangle;
            if (itms != null && itms.Name == "DateRectangle")
            {
                if (el != null)
                {
                    this.Model.GoToSelectedStartTime(el.Date);
                }
            }
            else
            {
                TextBlock rct = this.InputHitTest(e.GetPosition(this)) as TextBlock;
                if (rct != null && rct.Name == "Date")
                {
                    if (el != null)
                    {
                        this.Model.GoToSelectedStartTime(el.Date);
                    }
                }
            }
#endif

            if (el == null)
            {
                return;
            }
            this.Model.CurrentSelectedAppointment = null;
          
            this.ClearPreviousDateContentSelection();
            this.ClearPreviousAppointmentSelection();
            if (el.IsCurrentMonth)
            {
                el.IsSelected = true;
            }
        }

#if !SILVERLIGHT
        #region InputHitTest Result / Returns Object
        private DependencyObject GetHitTestParentElement(IInputElement childobj, string dependencyobjType)
        {
            FrameworkElement dependencyObj = childobj as FrameworkElement;

            if (dependencyObj != null)
            {
                if (dependencyObj.TemplatedParent != null)
                {
                    if (dependencyObj.TemplatedParent.DependencyObjectType.Name == dependencyobjType)
                        return dependencyObj.TemplatedParent;
                }
                else if (dependencyObj.Parent != null)
                {
                    if (dependencyObj.Parent.DependencyObjectType.Name == "ScheduleSideTextBlock")
                        return dependencyObj.Parent;
                }
            }

            return null;
        }

        private DependencyObject GetHitTestParentElementByName(IInputElement childobj, string dependencyobjType, string dependencyobjName)
        {
            FrameworkElement dependencyObj = childobj as FrameworkElement;

            if (dependencyObj != null)
            {
                if (dependencyObj.TemplatedParent.DependencyObjectType.Name == dependencyobjType &&
                    dependencyObj.Name == dependencyobjName)
                    return dependencyObj;
            }

            return null;
        }
        #endregion
#endif

        /// <summary>
        /// Clears the previous appointment selection.
        /// </summary>
        private void ClearPreviousAppointmentSelection()
        {
            foreach (var item in this.monthAppointmentLayoutItemsControl.Items)
            {
                var viewControl = this.monthAppointmentLayoutItemsControl.ItemContainerGenerator.ContainerFromItem(item) as ScheduleMonthAppointmentViewControl;
                if (viewControl != null && viewControl.IsSelected)
                {
                    viewControl.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// Clears the previous date content selection.
        /// </summary>
        private void ClearPreviousDateContentSelection()
        {
            var contents = this.monthViewItemsControl.Items.OfType<ScheduleMonthDateContentControl>().Select(item => item).ToList();
            var elements = from cont in contents
                           from content in contents.Where(c => c != null)
                           select content;

            foreach (var content in elements)
            {
                content.IsSelected = false;
            }
        }

        #region AppointmentEditor

        /// <summary>
        /// Sets up appointment editor.
        /// </summary>
        private void SetUpAppointmentEditor()
        {
            this.monthViewItemsControl.MouseDoubleClick += new MouseButtonEventHandler(monthViewItemsControl_MouseDoubleClick);
            this.monthAppointmentLayoutItemsControl.MouseDoubleClick += new MouseButtonEventHandler(monthAppointmentLayoutItemsControl_MouseDoubleClick);
        
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the monthAppointmentLayoutItemsControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void monthAppointmentLayoutItemsControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
          
            if (!this.Model.AllowEdit)
            {
                return;
            }

#if SILVERLIGHT
              var showEditorWindow = false;
            var items = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleMonthAppointmentViewControl>().ToList();
            if (items.Count > 0)
            {
                if (this.Model != null)
                {
                    //ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(items[0].ScheduleAppointment);
                    ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(this.Model.CurrentSelectedAppointment);
                    this.Model.GetAppointmentDoubleClickEvents(args);
                    showEditorWindow = !args.Cancel;    
                }
                //this.Model.CurrentSelectedAppointment = items[0].ScheduleAppointment;
                if (showEditorWindow)
                    this.ShowWindow(this.Model.CurrentSelectedAppointment, true);
                else
                    e.Handled = true;
            }
#else
            var items = this.InputHitTest(e.GetPosition(this));

            ScheduleMonthAppointmentViewControl itemevt = GetHitTestParentElement(items, "ScheduleMonthAppointmentViewControl") as ScheduleMonthAppointmentViewControl;
            if (itemevt != null && itemevt.ScheduleAppointment != null)
            {
                if (this.Model != null)
                {
                    ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(this.Model.CurrentSelectedAppointment);
                    this.Model.GetAppointmentDoubleClickEvents(args);
                }
                //this.Model.CurrentSelectedAppointment = itemevt.ScheduleAppointment;
                this.ShowWindow(this.Model.CurrentSelectedAppointment, true);
            }
            e.Handled = true;
#endif
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the monthViewItemsControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void monthViewItemsControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var content = e.OriginalSource as Rectangle;
            if ((content as Rectangle) == null || (content as Rectangle).Name == "DateRectangle") return;
            var parentEl = content.FindParentElementOfType<ScheduleMonthDateContentControl>();
            if (parentEl == null) return;
            var startTimeSpan = parentEl.Date;
            var endTimeSpan = parentEl.Date;
            if (startTimeSpan == DateTime.MinValue || endTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }
            var appointment = this.Model.CreateNewAppointment(startTimeSpan, endTimeSpan, true);           

            this.ShowWindow(appointment, false);
            e.Handled = true;
        }

#if !SILVERLIGHT
        private Window GetTopParent()
        {
            DependencyObject dpParent = this.FindParentElementOfType<Window>();
            return dpParent as Window;
        }
#endif

#if !SILVERLIGHT
        /// <summary>
        ///  This virtual  return a window with center alignment
        /// </summary>
        protected virtual Window GetChildWindow()
        {
            var window = new Window();
            window.Width = 640;
            window.Height = 570;
            window.HorizontalAlignment = HorizontalAlignment.Center;
            window.VerticalAlignment = VerticalAlignment.Center;
            return window;
        }

        /// <summary>
        /// Virtual Method for the new window alignment though its width
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected virtual Window GetChildWindow(double width)
        {
            var window = new Window();
            window.Width = width;
            window.HorizontalAlignment = HorizontalAlignment.Center;
            window.VerticalAlignment = VerticalAlignment.Center;
           
            return window;
        }
#else
        /// <summary>
        /// Gets the child window.
        /// </summary>
        /// <returns></returns>
        protected virtual ChildWindow GetChildWindow()
        {
            var window = new ChildWindow();
            window.Width = 800;
            window.Height = 570;
            return window;
        }

        /// <summary>
        /// Gets the child window.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        protected virtual ChildWindow GetChildWindow(double width)
        {
            var window = new ChildWindow();
            window.Width = width;
            return window;
        }
#endif

        /// <summary>
        /// Handles the Closed event of the window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void window_Closed(object sender, EventArgs e)
        {
#if SILVERLIGHT
            var window = sender as ChildWindow;
#else
            var window = sender as Window;
#endif
            window.Closed -= new EventHandler(window_Closed);
            if (this.Model != null)
            {
                ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(new ScheduleAppointment());
                this.Model.GetAppointmentWindowClosedEvents(args);
            }
#if SILVERLIGHT

            var editor = (sender as ChildWindow).Content as ScheduleAppointmentEditorControl;
#else
            var editor = (sender as Window).Content as ScheduleAppointmentEditorControl;
#endif
            var appWrapper = editor.DataContext as ScheduleAppointmentWrapper;
            if (editor != null && (IsRecurrenceModified == true || editor.IsStateChanged))
            {
                ResourceWrapper rw = new ResourceWrapper();
                //var result = MessageBox.Show("Do you want to save changes?", "Scheduler", MessageBoxButton.OKCancel);
                var result = MessageBox.Show(rw.AppointmentSaveChangesMessageBoxContent, rw.AppointmentSaveChangesMessageBoxHeader, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    SaveAppointments(editor);
                }
            }
            IsRecurrenceModified = false;
        }

        /// <summary>
        /// Handles the DeleteButtonClick event of the editorControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void editorControl_DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            IsRecurrenceModified = false;
            var editor = sender as ScheduleAppointmentEditorControl;
            editor.DeleteButtonClick -= new RoutedEventHandler(editorControl_DeleteButtonClick);
#if !SILVERLIGHT
            var childWindow = editor.FindParentElementOfType<Window>();
#else
            var childWindow = editor.FindParentElementOfType<ChildWindow>();
#endif
            if (childWindow != null)
            {
                this.Model.DeleteCurrentSelectedAppointment();
                childWindow.Close();
            }
        }

        /// <summary>
        /// Handles the SaveButtonClick event of the editorControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void editorControl_SaveButtonClick(object sender, RoutedEventArgs e)
        {
            IsRecurrenceModified = false;
            var editor = sender as ScheduleAppointmentEditorControl;
            editor.IsStateChanged = false;
            editor.SaveButtonClick += new RoutedEventHandler(editorControl_SaveButtonClick);
#if SILVERLIGHT
            var childWindow = editor.FindParentElementOfType<ChildWindow>();
#else
            var childWindow = editor.FindParentElementOfType<Window>();
#endif
            if (childWindow != null)
            {
                SaveAppointments(editor);
                childWindow.Close();
            }
        }

        /// <summary>
        /// Saves the appointments.
        /// </summary>
        /// <param name="editor">The editor.</param>
        private void SaveAppointments(ScheduleAppointmentEditorControl editor)
        {
            var appWrapper = editor.DataContext as ScheduleAppointmentWrapper;
            var appProxy = appWrapper.Appointment;
            var newAppointment = new ScheduleAppointment();
            newAppointment.InitializeFrom(appProxy);
            newAppointment.CurrentRecurrencePatternMode = editor.RecurrencePattern;

            if (editor.IsRecurrenceOccured == true)
            {
                newAppointment.IsRecurrenceAppointment = true;
            }
            else
            {
                newAppointment.IsRecurrenceAppointment = false;
            }

            if (this.Model.CurrentSelectedAppointment != null)
            {
                if (this.Model.IsDataBound)
                {
                    // sync record
                    var record = this.Model.CurrentSelectedAppointment.Record;
                    newAppointment.Record = record;
                }

                var selApp = this.Model.CurrentSelectedAppointment;

                if (appWrapper.Appointment.IsAppointmentProxyCollection == true)
                {
                    newAppointment.IsAppointmentProxyCollection = true;
                    this.Model.RemoveCurrentAppointmentProxy(this.Model.CurrentSelectedAppointment);
                    newAppointment.CurrentAppointmentType = AppointmentType.RecurrenceProxy;
                    this.Model.CurrentSelectedAppointment.AppointmentProxy.Add(newAppointment);
                }
                else
                {
                    if (selApp.IsRecurrenceAppointment == true)
                    {
                        selApp.StartTime = this.Model.CurrentSelectedAppointment.StartRecurrenceTime;
                        selApp.EndTime = this.Model.CurrentSelectedAppointment.EndRecurrenceTime;
                    }

                    if (this.Model.RemoveCurrentAppointment(selApp))
                        this.Model.Appointments.Add(newAppointment);
                }
            }
            else
            {
                this.Model.Appointments.Add(newAppointment);
            }

            if (this.Model.CurrentSelectedAppointment != null || editor.IsRecurrenceOccured == true)
                this.SetupAppointments();
            this.Model.CurrentSelectedAppointment = null;
        }


        /// <summary>
        ///  sets bool value as false , therefore recurrence not modified
        /// </summary>
        private bool IsRecurrenceModified = false;

        /// <summary>
        /// Handles the RecurrenceButtonClick event of the editorControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void editorControl_RecurrenceButtonClick(object sender, RoutedEventArgs e)
        {
            var editor = sender as ScheduleAppointmentEditorControl;
            editor.RecurrenceButtonClick -= new RoutedEventHandler(editorControl_RecurrenceButtonClick);

            var appWrapper = editor.DataContext as ScheduleAppointmentWrapper;
            IsRecurrenceModified = true;
            appWrapper.IsAppointmentModified = true;
            if (editor.IsRecurrenceOccured == false)
            {
                if (appWrapper.Appointment.IsRecurrenceAppointment == true)
                {
                    appWrapper.Appointment.IsRecurrenceAppointment = false;
                }
            }

            editor.RecurrenceButtonClick += new RoutedEventHandler(editorControl_RecurrenceButtonClick);
        }

        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <param name="appointment">The appointment.</param>
        /// <param name="isEditing">if set to <c>true</c> [is editing].</param>
        private void ShowWindow(ScheduleAppointment appointment, bool isEditing)
        {
            IsRecurrenceModified = false;
            if (this.Model.CurrentSelectedAppointment != null)
            {
                appointment = this.Model.CurrentSelectedAppointment;
            }
            if (this.Model != null)
            {
                ScheduleAppointmentCancelEventArgs arg = new ScheduleAppointmentCancelEventArgs(appointment);
                this.Model.GetAppointmentWindowOpeningEvents(arg);
                if (arg.Cancel == true && arg != null) return;               
            }
            var appWrapper = new ScheduleAppointmentWrapper(appointment, this.Model, isEditing);

            if (appointment.IsRecurrenceAppointment == true)
            {
                var childwindow = this.GetChildWindow(250);
                ResourceWrapper rw = new ResourceWrapper();
                childwindow.Title = rw.RecurrenceAlertWindowHeader;
#if !SILVERLIGHT
                childwindow.Height = 170;
                childwindow.ResizeMode = ResizeMode.NoResize;
                childwindow.WindowStartupLocation = WindowStartupLocation.Manual;
                Schedule s = this.GetScheduleParent();
                Point schedulePoint = s.PointToScreen(new Point());
                childwindow.Left = schedulePoint.X + (s.ActualWidth- childwindow.Width) / 2;
                childwindow.Top = schedulePoint.Y + (s.ActualHeight - childwindow.Height) / 2;  
#else
                 if (this.AppointmentScheduleWindowStyle != null)
                {
                    childwindow.Style = this.AppointmentScheduleWindowStyle;
                }
#endif

                var recContent = new ScheduleRecurrenceConfirmationWindow();
                if (this.RecurrenceAlertWindowStyle != null)
                {
                    recContent.Style = this.RecurrenceAlertWindowStyle;
                }
                recContent.DataContext = appWrapper;
                recContent.CancelButtonClick += (sndr, e) =>
                {
                    var editor = sndr as ScheduleRecurrenceConfirmationWindow;
#if !SILVERLIGHT
            var childWindow = editor.FindParentElementOfType<Window>();
#else
                    var childWindow = editor.FindParentElementOfType<ChildWindow>();
#endif
                    childWindow.Close();
                };
                recContent.OkButtonClick += new RoutedEventHandler(recContent_OkButtonClick);
                childwindow.Content = recContent;
#if SILVERLIGHT
                childwindow.Show();
#else
                childwindow.ShowDialog();
#endif
            }
            else
            {
                ShowWindow(appWrapper, Visibility.Collapsed);
            }
            ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(appointment);
            this.Model.GetAppointmentWindowOpenedEvents(args);
        }

        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <param name="appWrapper">The app wrapper.</param>
        /// <param name="recurrenceVisibility">The recurrence visibility.</param>
        private void ShowWindow(ScheduleAppointmentWrapper appWrapper, Visibility recurrenceVisibility)
        {
            var editorControl = new ScheduleAppointmentEditorControl();
            editorControl.SaveButtonClick += new RoutedEventHandler(editorControl_SaveButtonClick);
            editorControl.DeleteButtonClick += new RoutedEventHandler(editorControl_DeleteButtonClick);
            editorControl.RecurrenceButtonClick += new RoutedEventHandler(editorControl_RecurrenceButtonClick);
            if (this.Model.AllowRecurrence == false || appWrapper.Appointment.AllowRecurrence == false)
                editorControl.AllowRecurrence = false;
            else
                editorControl.AllowRecurrence = true;
#if !SILVERLIGHT
            if (recurrenceVisibility == Visibility.Visible)
            {
                editorControl.DailyRecurrenceVisibility = Visibility.Collapsed;
                editorControl.WeeklyRecurrenceVisibility = Visibility.Collapsed;
                editorControl.MonlyRecurrenceVisibility = Visibility.Collapsed;
                editorControl.YearlyRecurrenceVisibility = Visibility.Collapsed;

                switch (appWrapper.Appointment.CurrentRecurrencePatternMode)
                {
                    case RecurrencePatternMode.Weekly:
                        editorControl.WeeklyRecurrenceVisibility = Visibility.Visible;
                        break;
                    case RecurrencePatternMode.Monthly:
                        editorControl.MonlyRecurrenceVisibility = Visibility.Visible;
                        break;
                    case RecurrencePatternMode.Yearly:
                        editorControl.YearlyRecurrenceVisibility = Visibility.Visible;
                        break;
                    case RecurrencePatternMode.Daily:
                    default:
                        editorControl.DailyRecurrenceVisibility = Visibility.Visible;
                        break;
                }
            }
#endif
            editorControl.DataContext = appWrapper;
            editorControl.RecurrenceVisibility = recurrenceVisibility;            
            if (this.AppointmentEditorStyle != null)
            {
                editorControl.Style = this.AppointmentEditorStyle;
            }
            var window = this.GetChildWindow();
#if SILVERLIGHT
            if (this.AppointmentScheduleWindowStyle != null)
            {
                window.Style = this.AppointmentScheduleWindowStyle;
            }
#else
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Owner = this.GetTopParent();
            Schedule s = this.GetScheduleParent();
            Point schedulePoint = s.PointToScreen(new Point());
            window.Left = schedulePoint.X + (s.ActualWidth - window.Width) / 2;
            window.Top = schedulePoint.Y + (s.ActualHeight - window.Height) / 2;  
#endif
            window.Closed += new EventHandler(window_Closed);
            window.KeyUp += new KeyEventHandler(window_KeyUp);
            var bindTitle = new Binding("Subject") { Source = appWrapper.Appointment, Mode = BindingMode.OneWay };
#if !SILVERLIGHT
            window.SetBinding(Window.TitleProperty, bindTitle);      
#else
            window.SetBinding(ChildWindow.TitleProperty, bindTitle);
#endif
            window.Content = editorControl;
            window.Show();
        }

        //window.KeyUp += new KeyEventHandler(window_KeyUp);
        void window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
#if SILVERLIGHT
                var window = (ChildWindow)sender;
#else
                var window = (Window)sender;                
#endif
                window.Close();
            }
        }

        /// <summary>
        /// Handles the OkButtonClick event of the recContent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void recContent_OkButtonClick(object sender, RoutedEventArgs e)
        {
            var editor = sender as ScheduleRecurrenceConfirmationWindow;
#if !SILVERLIGHT
            var childWindow = editor.FindParentElementOfType<Window>();
#else
            var childWindow = editor.FindParentElementOfType<ChildWindow>();
#endif
            childWindow.Close();
            var appWrapper = editor.DataContext as ScheduleAppointmentWrapper;
            var appProxy = appWrapper.Appointment;
            if (appProxy.IsRecurrenceAppointment)
            {
                appWrapper.Appointment.StartTime = appProxy.StartRecurrenceTime;
                appWrapper.Appointment.EndTime = appProxy.EndRecurrenceTime;
                ShowWindow(appWrapper, Visibility.Visible);
            }
            else
            {
                appWrapper.Appointment.IsAppointmentProxyCollection = true;
                appWrapper.Appointment.IsRecurrenceAppointment = false;
                appWrapper.Appointment.AllowRecurrence = false;
                ShowWindow(appWrapper, Visibility.Collapsed);
            }
        }

        #region AppointmentScheduleWindowStyle (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AppointmentScheduleWindowStyle.
        /// </summary>
        public Style AppointmentScheduleWindowStyle
        {
            get { return (Style)GetValue(AppointmentScheduleWindowStyleProperty); }
            set { SetValue(AppointmentScheduleWindowStyleProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AppointmentScheduleWindowStyle.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentScheduleWindowStyleProperty =
            DependencyProperty.Register("AppointmentScheduleWindowStyle", typeof(Style), typeof(ScheduleMonthView),
              new PropertyMetadata(null));

        #endregion

        #region RecurrenceAlertWindowStyle (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AppointmentEditorStyle.
        /// </summary>
        public Style RecurrenceAlertWindowStyle
        {
            get { return (Style)GetValue(RecurrenceAlertWindowStyleProperty); }
            set { SetValue(RecurrenceAlertWindowStyleProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for RecurrenceAlertWindowStyle.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RecurrenceAlertWindowStyleProperty =
            DependencyProperty.Register("RecurrenceAlertWindowStyle", typeof(Style), typeof(ScheduleMonthView),
              new PropertyMetadata(null));

        #endregion

        #region AppointmentEditorStyle (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AppointmentEditorStyle.
        /// </summary>
        public Style AppointmentEditorStyle
        {
            get { return (Style)GetValue(AppointmentEditorStyleProperty); }
            set { SetValue(AppointmentEditorStyleProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AppointmentEditorStyle.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentEditorStyleProperty =
            DependencyProperty.Register("AppointmentEditorStyle", typeof(Style), typeof(ScheduleMonthView),
              new PropertyMetadata(null));

        #endregion
        
        #endregion AppointmentEditor        

        #region Appointments population

        #region AppointmentStyle (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AppointmentStyle
        /// </summary>
        public Style AppointmentStyle
        {
            get { return (Style)GetValue(AppointmentStyleProperty); }
            set { SetValue(AppointmentStyleProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AppointmentStyle.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentStyleProperty =
            DependencyProperty.Register("AppointmentStyle", typeof(Style), typeof(ScheduleMonthView),
              new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// Setups the appointments.
        /// </summary>
        internal void SetupAppointments()
        {
            if (this.Model == null || this.monthAppointmentLayoutItemsControl == null || this.Model.Appointments == null)
            {
                return;
            }

            this.Model.Appointments.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnAppointmentsCollectionChanged);
            this.ClearAppointments();
            this.Model.AppointmentProxy.Clear();
            foreach (var app in this.Model.GetCurrentAppointments())
            {
                if (app != null)
                {
                    AddAppointmentInLayout(app.Appointment);
                }
            }
            this.Model.Appointments.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnAppointmentsCollectionChanged);
            this.Model.RaiseAppointmentDatesBold();
        }

        private void AddAppointmentInLayout(ScheduleAppointment app)
        {
            var appProxy = app.CurrentAppointmentType == AppointmentType.MultiDay?this.Model.GetMultiDayAppoinmentProxyCollection(app):this.Model.GetMultiWeekAppoinmentProxyCollection(app);
            if (appProxy != null && appProxy.Count() > 0)
            {
                foreach (var proxy in appProxy)
                {
                    proxy.CurrentAppointmentType = AppointmentType.MultiWeek;
                    this.Model.AppointmentProxy.Add(new ScheduleAppointmentProxies(app, proxy));
                    var selectedDate = from res in this.Model.SelectedDates
                                       where res.Date == proxy.StartTime.Date
                                       select res;
                    if (selectedDate.Count() > 0)
                    {
                        this.monthAppointmentLayoutItemsControl.Items.Add(proxy);
                    }
                }
            }
            else
            {
                this.monthAppointmentLayoutItemsControl.Items.Add(app);
            }
        }

        private void RemoveAppointmentFromLayout(ScheduleAppointment app)
        {
            var appProxy = this.Model.GetMultiWeekAppoinmentProxyCollection(app);
            if (appProxy != null && appProxy.Count() > 0)
            {
                var selectdapp = from ap in this.Model.AppointmentProxy
                                 where ap.AppointmentProxy == app
                                 from res in this.Model.AppointmentProxy
                                 where res.ParentAppointment == ap.ParentAppointment
                                 select res;

                foreach (var appItem in selectdapp)
                {
                    var viewControl = this.monthAppointmentLayoutItemsControl.Items.OfType<ScheduleAppointment>().FirstOrDefault(c => c == appItem.AppointmentProxy);
                    if (viewControl != null)
                    {
                        this.monthAppointmentLayoutItemsControl.Items.Remove(viewControl);
                    }
                }
            }
            else
            {
                var appointmentView = this.monthAppointmentLayoutItemsControl.Items.OfType<ScheduleAppointment>().FirstOrDefault(c => c == app);
                if (appointmentView != null)
                {
                    this.monthAppointmentLayoutItemsControl.Items.Remove(appointmentView);
                }
            }
        }

        /// <summary>
        /// Clears the appointments.
        /// </summary>
        private void ClearAppointments()
        {
            this.monthAppointmentLayoutItemsControl.Items.Clear();
#if SILVERLIGHT
            this.Dispatcher.BeginInvoke(() =>
#else
            this.Dispatcher.BeginInvoke(new Action(() =>
#endif
            {
                var appointmentsLayoutPanel = this.monthAppointmentLayoutItemsControl.FindElementOfType<ScheduleMonthAppointmentLayoutPanel>();
                if (appointmentsLayoutPanel != null)
                {
                    appointmentsLayoutPanel.Reset();
                }
#if SILVERLIGHT
            });
#else
            }));
#endif
            this.monthAppointmentLayoutItemsControl.Items.Clear();
        }

        /// <summary>
        /// Called when [appointments collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnAppointmentsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.Model == null || this.monthAppointmentLayoutItemsControl == null || this.Model.CurrentScheduleType != ScheduleType.Month)
            {
                return;
            }

            var isRecurrenceApp = false;

            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    this.Model.GenerateAppointmentID(e);
                    foreach (ScheduleAppointment app in e.NewItems)
                    {
                        if (app != null)
                        {
                            var resty = from res in this.Model.SelectedDates
                                        where (res.Date >= app.StartTime.Date && res.Date <= app.EndTime.Date) || (app.IsRecurrenceAppointment == true && app.CheckForAppointment(res.Date) != null)
                                        select res;

                            if (resty.Count() <= 0) continue;
                        }
                        if (app.IsRecurrenceAppointment || app.CurrentAppointmentType == AppointmentType.Recurrence || app.CurrentAppointmentType == AppointmentType.RecurrenceProxy)
                        {
                            isRecurrenceApp = true;
                            continue;
                        }
                        AddAppointmentInLayout(app);
                        //this.UpdatePriority(app);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (ScheduleAppointment app in e.OldItems)
                    {
                        //this.UpdatePriority(app);
                        this.RemoveAppointmentFromLayout(app);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    //this.UpdateMonthViewPriority();
                    this.ClearAppointments();
                    break;
            }
            if (isRecurrenceApp)
            {
                this.SetupAppointments();
                isRecurrenceApp = false;
            }
        }

        //private void UpdateMonthViewPriority()
        //{
        //    foreach (ScheduleAppointmentInfo app in this.Model.GetCurrentAppointments())
        //    {
        //        if(app != null)
        //    }
        //}

        #endregion Appointments population

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (isTemplateApplied == false) return;
            this.AppointmentPopup.Children.Clear();
            this.model.PropertyChanged -= new PropertyChangedEventHandler(model_PropertyChanged);
            this.Model.Appointments.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnAppointmentsCollectionChanged);
           
            this.monthViewItemsControl.Items.Clear();
            this.monthAppointmentLayoutItemsControl.Items.Clear();
            
            this.MouseMove -= new MouseEventHandler(ScheduleMonthView_MouseMove);
            this.monthViewItemsControl.MouseLeftButtonDown -= new MouseButtonEventHandler(monthViewItemsControl_MouseLeftButtonDown);
            this.monthViewItemsControl.MouseLeftButtonUp -= new MouseButtonEventHandler(monthViewItemsControl_MouseLeftButtonUp);
            this.monthAppointmentLayoutItemsControl.MouseLeftButtonDown -= new MouseButtonEventHandler(monthAppointmentLayoutItemsControl_MouseLeftButtonDown);
            this.monthAppointmentLayoutItemsControl.MouseMove -= new MouseEventHandler(monthAppointmentLayoutItemsControl_MouseMove);
            this.monthAppointmentLayoutItemsControl.MouseLeftButtonUp -= new MouseButtonEventHandler(monthAppointmentLayoutItemsControl_MouseLeftButtonUp);            
            this.monthViewItemsControl.MouseDoubleClick -= new MouseButtonEventHandler(monthViewItemsControl_MouseDoubleClick);
            this.monthAppointmentLayoutItemsControl.MouseDoubleClick -= new MouseButtonEventHandler(monthAppointmentLayoutItemsControl_MouseDoubleClick);           
            
#if (SyncfusionFramework4_0) || !(SyncfusionFramework3_5 && SILVERLIGHT)
            this.monthViewItemsControl.MouseRightButtonDown -= new MouseButtonEventHandler(monthViewItemsControl_MouseRightButtonDown);
            this.monthAppointmentLayoutItemsControl.MouseRightButtonDown -= new MouseButtonEventHandler(monthAppointmentLayoutItemsControl_MouseRightButtonDown);
#endif
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                      new PropertyChangedEventArgs(property));
            }
        }
        #endregion
    }
}
