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
    using System.ComponentModel;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Threading;
    using System.Windows.Data;
    using System.Windows.Controls.Primitives;
    using Syncfusion.Windows.Controls.Schedule;
#if SILVERLIGHT
    using Syncfusion.Windows.Shared;
#endif
#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  Represents Schedule type as days view
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
#if !SILVERLIGHT
    [StyleTypedProperty(Property = "AppointmentScheduleWindowStyle", StyleTargetType = typeof(Window))]
#else
    /// <summary>
    /// Represents Schedule's Day view
    /// </summary>
    [StyleTypedProperty(Property = "AppointmentScheduleWindowStyle", StyleTargetType = typeof(ChildWindow))]
#endif
    [StyleTypedProperty(Property = "RecurrenceAlertWindowStyle", StyleTargetType = typeof(ScheduleRecurrenceConfirmationWindow))]

    [StyleTypedProperty(Property = "AppointmentEditorStyle", StyleTargetType = typeof(ScheduleAppointmentEditorControl))]
    [StyleTypedProperty(Property = "AppointmentStyle", StyleTargetType = typeof(ScheduleDaysAppointmentViewControl))]

    public class ScheduleDaysView : Control, IScheduleCalendarViewModelHost, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleDaysView"/> class.
        /// </summary>
        public ScheduleDaysView()
        {
            this.DefaultStyleKey = typeof(ScheduleDaysView);

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

        private ResourceDictionary resourceDictionary;

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
            this.model = model;
            this.UpdateModelsToInnerControls();
            this.model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);

        }

        void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Model.CurrentScheduleType != ScheduleType.Day && this.Model.CurrentScheduleType != ScheduleType.Week
                && this.Model.CurrentScheduleType != ScheduleType.WorkWeek) return;

            if (e.PropertyName == "SelectedDates" || e.PropertyName == "CurrentTimeInterval")
            {
                this.ClearTimeSlotSelection();
                this.SetupAppointments();
                this.SetCurrentDateBorder();
            }

            if (this.model.ShowContextMenu == true)
            {
                if (e.PropertyName == "ContextMenuTimeSlotItems")
                {
                    this.LoadContextMenuTimeSlotItems();
                }

                if (e.PropertyName == "ContextMenuTimeLineItems")
                {
                    this.LoadContextMenuTimeHourItems();
                }

                if (e.PropertyName == "ContextMenuDaysHeaderItems")
                {
                    this.LoadContextMenuDayHeaderItems();
                }

                if (e.PropertyName == "ContextMenuAppointmentItems")
                {
                    this.LoadContextMenuAppointmentItems();
                }

                if (e.PropertyName == "ContextMenuAllDayAppointmentItems")
                {
                    this.LoadContextMenuAllDayAppointmentItems();
                }

                if (e.PropertyName == "ContextMenuType")
                {
                    this.LoadContextMenuAllDayAppointmentItems();
                    this.LoadContextMenuAppointmentItems();
                    this.LoadContextMenuTimeSlotItems();
                    this.LoadContextMenuTimeHourItems();
                    this.LoadContextMenuDayHeaderItems();
                }
            }

            if (e.PropertyName == "ShowContextMenu")
            {
                if (this.model.ShowContextMenu == true && (this.model.CurrentScheduleType == ScheduleType.Day || this.model.CurrentScheduleType == ScheduleType.Week || this.model.CurrentScheduleType == ScheduleType.WorkWeek))
                {
                    InitializeContextMenu();
                }
                else if (this.model.ShowContextMenu == false && (this.model.CurrentScheduleType == ScheduleType.Day || this.model.CurrentScheduleType == ScheduleType.Week || this.model.CurrentScheduleType == ScheduleType.WorkWeek))
                {
                    DeInitializeContextMenu();
                }
            }
        }

        /// <summary>
        /// Load the Context Menu Items for ContextMenuAlldayAppointmentItems.
        /// </summary>
        private void LoadContextMenuAllDayAppointmentItems()
        {
            if (ContextMenuAllDaysAppointmentsLayout != null)
            {
                if (this.model.ContextMenuType == ContextMenuType.Default)
                {
                    DefaultContextMenuAlldayAppointmentItems();
                }
                else if (this.model.ContextMenuType == ContextMenuType.Custom)
                {
                    ContextMenuAllDaysAppointmentsLayout.Items.Clear();
                    if (this.model.ContextMenuAllDayAppointmentItems.Count != 0)
                    {
                        if (this.model.ShowContextMenu == true)
                        {
                            ContextMenuAllDaysAppointmentsLayout.Visibility = Visibility.Visible;
                        }
                        CustomContextMenuAlldayAppointmentItems();
                    }
                    else
                    {
                        ContextMenuAllDaysAppointmentsLayout.Visibility = Visibility.Collapsed;
                    }
                }
                else if (this.model.ContextMenuType == ContextMenuType.CustomWithDefault)
                {
                    DefaultContextMenuAlldayAppointmentItems();
                    CustomContextMenuAlldayAppointmentItems();
                }
            }

        }

        /// <summary>
        /// Load the Context Menu Items for ContextMenuAppointmentsLayout
        /// </summary>
        private void LoadContextMenuAppointmentItems()
        {
            if (ContextMenuAppointmentsLayout != null)
            {
                if (this.model.ContextMenuType == ContextMenuType.Default)
                {
                    DefaultContextMenuAppointmentItems();
                }
                else if (this.model.ContextMenuType == ContextMenuType.Custom)
                {
                    ContextMenuAppointmentsLayout.Items.Clear();
                    if (this.model.ContextMenuAppointmentItems.Count != 0)
                    {
                        if (this.model.ShowContextMenu == true)
                        {
                            ContextMenuAppointmentsLayout.Visibility = Visibility.Visible;
                        }
                        CustomContextMenuAppointmentItems();
                    }
                    else
                    {
                        ContextMenuAppointmentsLayout.Visibility = Visibility.Collapsed;
                    }
                }
                else if (this.model.ContextMenuType == ContextMenuType.CustomWithDefault)
                {
                    DefaultContextMenuAppointmentItems();
                    CustomContextMenuAppointmentItems();
                }
            }
        }

        /// <summary>
        /// Load the Context Menu Items for ContextMenuDaysHeaderControl
        /// </summary>
        private void LoadContextMenuDayHeaderItems()
        {
            if (ContextMenuDaysHeaderControl != null)
            {
                if (this.model.ContextMenuType == ContextMenuType.Default)
                {
                    DefaultContextMenuDayHeader();
                }
                else if (this.model.ContextMenuType == ContextMenuType.Custom)
                {
                    ContextMenuDaysHeaderControl.Items.Clear();
                    if (this.model.ContextMenuDaysHeaderItems.Count != 0)
                    {
                        if (this.model.ShowContextMenu == true)
                        {
                            ContextMenuDaysHeaderControl.Visibility = Visibility.Visible;
                        }
                        CustomContextMenuDayHeader();
                    }
                    else
                    {
                        ContextMenuDaysHeaderControl.Visibility = Visibility.Collapsed;
                    }
                }
                else if (this.model.ContextMenuType == ContextMenuType.CustomWithDefault)
                {
                    DefaultContextMenuDayHeader();
                    CustomContextMenuDayHeader();
                }
            }

        }

        /// <summary>
        /// Load the Context Menu Items for ContextMenuTimeSlot
        /// </summary>
        private void LoadContextMenuTimeSlotItems()
        {
            if (ContextMenuTimeSlot != null)
            {
                if (this.model.ContextMenuType == ContextMenuType.Default)
                {
                    DefaultContextMenuTimeSlotItems();
                }
                else if (this.model.ContextMenuType == ContextMenuType.Custom)
                {
                    ContextMenuTimeSlot.Items.Clear();
                    if (this.model.ContextMenuTimeSlotItems.Count != 0)
                    {
                        CustomContextMenuTimeSlotItems();
                    }

                }
                else if (this.model.ContextMenuType == ContextMenuType.CustomWithDefault)
                {
                    DefaultContextMenuTimeSlotItems();
                    CustomContextMenuTimeSlotItems();
                }
            }
            if (ContextMenuTimeSlot.Items.Count == 0)
                ContextMenuTimeSlot.Visibility = Visibility.Collapsed;
            else
                ContextMenuTimeSlot.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Load the Context Menu Items for ContextMenuTimeLineItemsControl
        /// </summary>
        private void LoadContextMenuTimeHourItems()
        {
            if (ContextMenuTimeLineItemsControl != null)
            {
                if (this.model.ContextMenuType == ContextMenuType.Default)
                {
                    DefaultContextMenuTimeHourItems();
                }
                else if (this.model.ContextMenuType == ContextMenuType.Custom)
                {
                    ContextMenuTimeLineItemsControl.Items.Clear();
                    if (this.model.ContextMenuTimeLineItems.Count != 0)
                    {
                        if (this.model.ShowContextMenu == true)
                        {
                            ContextMenuTimeLineItemsControl.Visibility = Visibility.Visible;
                        }
                        CustomContextMenuTimeHourItems();
                    }
                    else
                    {
                        ContextMenuTimeLineItemsControl.Visibility = Visibility.Collapsed;
                    }
                }
                else if (this.model.ContextMenuType == ContextMenuType.CustomWithDefault)
                {
                    DefaultContextMenuTimeHourItems();
                    CustomContextMenuTimeHourItems();
                }
            }
        }

        private void DefaultContextMenuAlldayAppointmentItems()
        {
            if (ContextMenuAllDaysAppointmentsLayout != null)
            {
                ContextMenuAllDaysAppointmentsLayout.Visibility = Visibility.Visible;
                ContextMenuAllDaysAppointmentsLayout.Items.Clear();
                foreach (var item in DefaultContextMenuItemsDayAllDayAppointments)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        ContextMenuAllDaysAppointmentsLayout.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuAllDaysAppointmentsLayout.Items.Add(s);
                    }
#else
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        ContextMenuAllDaysAppointmentsLayout.Items.Add(m);
        }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        ContextMenuAllDaysAppointmentsLayout.Items.Add(s);

                    }       
#endif
                }
            }
        }

        private void CustomContextMenuAlldayAppointmentItems()
        {
            if (this.model.ContextMenuAllDayAppointmentItems.Count != 0)
            {
                foreach (var item in this.model.ContextMenuAllDayAppointmentItems)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        m.Style = (Style)resourceDictionary["ContextMenuItemAdvStyle"];
                        ContextMenuAllDaysAppointmentsLayout.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuAllDaysAppointmentsLayout.Items.Add(s);

                    }
#else
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        m.Style = (Style)resourceDictionary["ContextMenuItemStyle"];
                        ContextMenuAllDaysAppointmentsLayout.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        s.Style = (Style)resourceDictionary["SeparatorStyle"];
                        ContextMenuAllDaysAppointmentsLayout.Items.Add(s);

                    }       
#endif
                }
            }
        }

        private void DefaultContextMenuAppointmentItems()
        {
            if (ContextMenuAppointmentsLayout != null)
            {
                ContextMenuAppointmentsLayout.Visibility = Visibility.Visible;
                ContextMenuAppointmentsLayout.Items.Clear();
                foreach (var item in DefaultContextMenuItemsDayAppointments)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        ContextMenuAppointmentsLayout.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuAppointmentsLayout.Items.Add(s);
                    }
#else
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        ContextMenuAppointmentsLayout.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        ContextMenuAppointmentsLayout.Items.Add(s);
                    }
#endif
                }
            }
        }

        private void CustomContextMenuAppointmentItems()
        {
            if (this.model.ContextMenuAppointmentItems.Count != 0)
            {

                foreach (var item in this.model.ContextMenuAppointmentItems)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        m.Style = (Style)resourceDictionary["ContextMenuItemAdvStyle"];
                        ContextMenuAppointmentsLayout.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuAppointmentsLayout.Items.Add(s);
                    }
#else
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        m.Style = (Style)resourceDictionary["ContextMenuItemStyle"];
                        ContextMenuAppointmentsLayout.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        s.Style = (Style)resourceDictionary["SeparatorStyle"];
                        ContextMenuAppointmentsLayout.Items.Add(s);
                    }
#endif
                }
            }
        }

        private void DefaultContextMenuDayHeader()
        {
            if (ContextMenuDaysHeaderControl != null)
            {
                ContextMenuDaysHeaderControl.Visibility = Visibility.Visible;
                ContextMenuDaysHeaderControl.Items.Clear();
#if SILVERLIGHT
                foreach (var item in DefaultContextMenuItemsDayViewHeaderControl)
                {
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        ContextMenuDaysHeaderControl.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuDaysHeaderControl.Items.Add(s);
                    }
                }
#else
                 foreach (var item in DefaultContextMenuItemsDayViewHeaderControl)
                {
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        ContextMenuDaysHeaderControl.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        ContextMenuDaysHeaderControl.Items.Add(s);
                    }
                }
#endif
            }
        }

        private void CustomContextMenuDayHeader()
        {
            if (this.model.ContextMenuDaysHeaderItems.Count != 0)
            {
                foreach (var item in this.model.ContextMenuDaysHeaderItems)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        m.Style = (Style)resourceDictionary["ContextMenuItemAdvStyle"];
                        ContextMenuDaysHeaderControl.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuDaysHeaderControl.Items.Add(s);
                    }
#else
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        m.Style = (Style)resourceDictionary["ContextMenuItemStyle"];
                        ContextMenuDaysHeaderControl.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        s.Style = (Style)resourceDictionary["SeparatorStyle"];
                        ContextMenuDaysHeaderControl.Items.Add(s);
                    }
#endif
                }
            }
        }

        private void DefaultContextMenuTimeSlotItems()
        {
            if (ContextMenuTimeSlot != null)
            {
                ContextMenuTimeSlot.Items.Clear();
                foreach (var item in DefaultContextMenuItemsDayTimeSlotControl)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        ContextMenuTimeSlot.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuTimeSlot.Items.Add(s);
                    }
#else
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        ContextMenuTimeSlot.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        ContextMenuTimeSlot.Items.Add(s);
                    }
#endif
                }
            }

        }

        private void CustomContextMenuTimeSlotItems()
        {
            if (this.model.ContextMenuTimeSlotItems.Count != 0)
            {
                foreach (var item in this.model.ContextMenuTimeSlotItems)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        m.Style = (Style)resourceDictionary["ContextMenuItemAdvStyle"];
                        ContextMenuTimeSlot.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuTimeSlot.Items.Add(s);
                    }

#else
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        m.Style = (Style)resourceDictionary["ContextMenuItemStyle"];
                        ContextMenuTimeSlot.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        s.Style = (Style)resourceDictionary["SeparatorStyle"];
                        ContextMenuTimeSlot.Items.Add(s);
                    }
#endif
                }
            }
        }

        private void DefaultContextMenuTimeHourItems()
        {
            if (ContextMenuTimeLineItemsControl != null)
            {
                ContextMenuTimeLineItemsControl.Visibility = Visibility.Visible;
                ContextMenuTimeLineItemsControl.Items.Clear();
                foreach (var item in DefaultContextMenuItemsDayTimeLineControl)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        ContextMenuTimeLineItemsControl.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuTimeLineItemsControl.Items.Add(s);
                    }
#else
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        ContextMenuTimeLineItemsControl.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        ContextMenuTimeLineItemsControl.Items.Add(s);
                    }
#endif
                }
            }
        }

        private void CustomContextMenuTimeHourItems()
        {
            if (this.Model.ContextMenuTimeLineItems.Count != 0)
            {
                foreach (var item in this.model.ContextMenuTimeLineItems)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        m.Style = (Style)resourceDictionary["ContextMenuItemAdvStyle"];
                        ContextMenuTimeLineItemsControl.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuTimeLineItemsControl.Items.Add(s);
                    }
#else
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        m.Style = (Style)resourceDictionary["ContextMenuItemStyle"];
                        ContextMenuTimeLineItemsControl.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        s.Style = (Style)resourceDictionary["SeparatorStyle"];
                        ContextMenuTimeLineItemsControl.Items.Add(s);
                    }
#endif
                }
            }
        }

        private void SetCurrentDateBorder()
        {
            if (this.currentDateGrid == null)
                return;

            this.currentDateGrid.Children.Clear();
            if (this.model.SelectedDates.Contains(DateTime.Now.Date.ToLocalTime()))
            {
                int index = this.model.SelectedDates.IndexOf(DateTime.Now.Date);
                this.currentDateGrid.ColumnDefinitions.Clear();
                foreach (DateTime date in this.model.SelectedDates)
                {
                    this.currentDateGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }
                Border rect = new Border();
                rect.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xEB, 0x89, 0x00));
                rect.BorderThickness = new Thickness(0.5);
                Grid.SetColumn(rect, index);
                this.currentDateGrid.Children.Add(rect);
            }
        }

        private void UpdateModelsToInnerControls()
        {
            if (!this.isTemplateApplied || this.model == null)
            {
                return;
            }
            this.alldaysAppointmentsLayoutPanel.SetCalendarViewModel(this.model);
            this.appointmentsLayoutItems.SetCalendarViewModel(this.model);
            this.timeLineItems.SetCalendarViewModel(this.model);
            this.daysHeaderViewItems.SetCalendarViewModel(this.model);
            this.timeSlotItems.SetCalendarViewModel(this.model);
            this.appointmentNavigatorControl.SetCalendarViewModel(this.Model);
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
            DependencyProperty.Register("AppointmentScheduleWindowStyle", typeof(Style), typeof(ScheduleDaysView),
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
            DependencyProperty.Register("RecurrenceAlertWindowStyle", typeof(Style), typeof(ScheduleDaysView),
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
            DependencyProperty.Register("AppointmentEditorStyle", typeof(Style), typeof(ScheduleDaysView),
              new PropertyMetadata(null));

        #endregion

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
            DependencyProperty.Register("AppointmentStyle", typeof(Style), typeof(ScheduleDaysView),
              new PropertyMetadata(null, OnAppointmentStyleChanged));

        private bool isAppointStyleChangedBeforeLoaded = false;
        private static void OnAppointmentStyleChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var daysView = dpo as ScheduleDaysView;
            if (daysView.isTemplateApplied)
            {
                daysView.appointmentsLayoutItems.AppointmentStyle = (Style)args.NewValue;
            }
            else
            {
                daysView.isAppointStyleChangedBeforeLoaded = true;
            }
        }

        #endregion

        #region AllDayAppointmentStyle (DependencyProperty)

        /// <summary>
        /// Gets / Sets the AllDayAppointmentStyle.
        /// </summary>
        public Style AllDayAppointmentStyle
        {
            get { return (Style)GetValue(AllDayAppointmentStyleProperty); }
            set { SetValue(AllDayAppointmentStyleProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for AllDayAppointmentStyle.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllDayAppointmentStyleProperty = DependencyProperty.Register("AllDayAppointmentStyle", typeof(Style), typeof(ScheduleDaysView), new PropertyMetadata(null, OnAllDayAppointmentChanged));
        private bool isAllDayAppointmentStyleChangedBeforeLoaded = false;
        private static void OnAllDayAppointmentChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var daysView = dpo as ScheduleDaysView;
            if (daysView.isTemplateApplied)
            {
                daysView.alldaysAppointmentsLayoutPanel.AllDayAppointmentStyle = (Style)args.NewValue;
            }
            else
            {
                daysView.isAllDayAppointmentStyleChangedBeforeLoaded = true;
            }
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
        public static DependencyProperty TimelineHourDivisionVisibilityProperty = DependencyProperty.Register("TimelineHourDivisionVisibility", typeof(Visibility), typeof(ScheduleDaysView), new PropertyMetadata(Visibility.Visible));
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
        public static readonly DependencyProperty TimelineVisibilityProperty = DependencyProperty.Register("TimelineVisibility", typeof(Visibility), typeof(ScheduleDaysView), new PropertyMetadata(Visibility.Visible));
        #endregion

#if Test
        internal ScheduleTimeLineItemsControl timeLineItems;
        internal ScheduleDaysHeaderViewItemsControl daysHeaderViewItems;
        internal ScheduleTimeSlotItemsControl timeSlotItems;
        internal ScheduleDaysAppointmentLayoutItemsControl appointmentsLayoutItems;
        internal ScheduleAllDaysAppointmentItemsControl alldaysAppointmentsLayoutPanel;
        
        internal ScrollViewer timeSlotScrollViewer;
        internal Grid AppointmentPopup;
#else

        private ScheduleTimeLineItemsControl timeLineItems;
        private ScheduleDaysHeaderViewItemsControl daysHeaderViewItems;
        private ScheduleTimeSlotItemsControl timeSlotItems;
        private ScheduleDaysAppointmentLayoutItemsControl appointmentsLayoutItems;
        private ScheduleAllDaysAppointmentItemsControl alldaysAppointmentsLayoutPanel;
        private ScrollViewer timeSlotScrollViewer;
        private ScheduleAppointmentNavigatorControl appointmentNavigatorControl;
        private Grid currentDateGrid;
        private Grid AppointmentPopup;
#endif
        private bool isTemplateApplied = false;
        private ContextMenuCommand allDayAppointment;
        private ContextMenuCommand newAppointment;
        private ContextMenuCommand todayDate;
        private ContextMenuCommand goToDate;
        private ContextMenuCommand newRecurringAppointment;
        private ContextMenuCommand newRecurringEvent;
        private ContextMenuCommand timeIntervelSixtyMinutes;
        private ContextMenuCommand timeIntervelThirtyMinutes;
        private ContextMenuCommand timeIntervelTwentyMinutes;
        private ContextMenuCommand timeIntervelFifteenMinutes;
        private ContextMenuCommand timeIntervelTenMinutes;
        private ContextMenuCommand timeIntervelSixMinutes;
        private ContextMenuCommand timeIntervelFiveMinutes;
        private ContextMenuCommand deleteAppointment;
        private ContextMenuCommand openAppointment;
        private ContextMenuCommand colorButton1;
        private ContextMenuCommand showAsCommand;
        private ObservableCollection<object> DefaultContextMenuItemsDayViewHeaderControl = new ObservableCollection<object>();
        private ObservableCollection<object> DefaultContextMenuItemsDayTimeLineControl = new ObservableCollection<object>();
        private ObservableCollection<object> DefaultContextMenuItemsDayTimeSlotControl = new ObservableCollection<object>();
        private ObservableCollection<object> DefaultContextMenuItemsDayAppointments = new ObservableCollection<object>();
        private ObservableCollection<object> DefaultContextMenuItemsDayAllDayAppointments = new ObservableCollection<object>();
#if SILVERLIGHT
        private ContextMenuAdv ContextMenuTimeLineItemsControl;
        private ContextMenuAdv ContextMenuTimeSlot;
        private ContextMenuAdv ContextMenuDaysHeaderControl;
        private ContextMenuAdv ContextMenuAppointmentsLayout;
        private ContextMenuAdv ContextMenuAllDaysAppointmentsLayout;
#else
        private ContextMenu ContextMenuTimeLineItemsControl;
        private ContextMenu ContextMenuTimeSlot;
        private ContextMenu  ContextMenuDaysHeaderControl;
        private ContextMenu ContextMenuAppointmentsLayout;
        private ContextMenu ContextMenuAllDaysAppointmentsLayout;
#endif


        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.DragPopUp = new Popup();
            this.AppointmentPopup = this.GetTemplateChild("PART_AppointmentPopup") as Grid;
            this.timeLineItems = this.GetTemplateChild("PART_TimeLineItemsControl") as ScheduleTimeLineItemsControl;
            this.daysHeaderViewItems = this.GetTemplateChild("PART_DaysHeaderControl") as ScheduleDaysHeaderViewItemsControl;
            this.timeSlotItems = this.GetTemplateChild("PART_TimeSlot") as ScheduleTimeSlotItemsControl;
            this.appointmentsLayoutItems = this.GetTemplateChild("PART_AppointmentsLayout") as ScheduleDaysAppointmentLayoutItemsControl;
            this.alldaysAppointmentsLayoutPanel = this.GetTemplateChild("PART_AllDaysAppointmentsLayout") as ScheduleAllDaysAppointmentItemsControl;
            this.timeSlotScrollViewer = this.GetTemplateChild("PART_TimeSlotScrollViewer") as ScrollViewer;
            this.appointmentNavigatorControl = this.GetTemplateChild("PART_AppointmentNavigatorControl") as ScheduleAppointmentNavigatorControl;
            this.currentDateGrid = this.GetTemplateChild("CurrentDateGrid") as Grid;
            this.isTemplateApplied = true;
            this.UpdateModelsToInnerControls();
            this.SetupTimeSlotEvents();
            this.SetupDaysHeaderViewEvents();
            this.SetupAppointments();
            this.SetCurrentDateBorder();
            this.SetupAppointmentEvents();
            this.SetupDoubleClick();
            this.UpdateTimeSlotScrollPosition();
            this.EnsureProperties();
            DefaultContextMenuItemsDayViewHeaderControl = resourceDictionary["DefaultContextMenuItemsDayViewHeaderControl"] as ObservableCollection<object>;
            DefaultContextMenuItemsDayAppointments = resourceDictionary["DefaultContextMenuItemsDayAppointments"] as ObservableCollection<object>;
            DefaultContextMenuItemsDayAllDayAppointments = resourceDictionary["DefaultContextMenuItemsDayAllDayAppointments"] as ObservableCollection<object>;
            DefaultContextMenuItemsDayTimeLineControl = resourceDictionary["DefaultContextMenuItemsDayTimeLineControl"] as ObservableCollection<object>;
            DefaultContextMenuItemsDayTimeSlotControl = resourceDictionary["DefaultContextMenuItemsDayTimeSlotControl"] as ObservableCollection<object>;
            if (this.model.ShowContextMenu == true)
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
            this.ContextMenuTimeLineItemsControl = this.GetTemplateChild("ContextMenuTimeLineItemsControl") as ContextMenuAdv;
            this.ContextMenuTimeSlot = this.GetTemplateChild("ContextMenuTimeSlot") as ContextMenuAdv;
            this.ContextMenuDaysHeaderControl = this.GetTemplateChild("ContextMenuDaysHeaderControl") as ContextMenuAdv;
            this.ContextMenuAppointmentsLayout = this.GetTemplateChild("ContextMenuAppointmentsLayout") as ContextMenuAdv;
            this.ContextMenuAllDaysAppointmentsLayout = this.GetTemplateChild("ContextMenuAllDaysAppointmentsLayout") as ContextMenuAdv;
#else
            this.ContextMenuTimeLineItemsControl = this.GetTemplateChild("ContextMenuTimeLineItemsControl") as ContextMenu;
            this.ContextMenuTimeSlot = this.GetTemplateChild("ContextMenuTimeSlot") as ContextMenu;
            this.ContextMenuDaysHeaderControl = this.GetTemplateChild("ContextMenuDaysHeaderControl") as ContextMenu;
            this.ContextMenuAppointmentsLayout = this.GetTemplateChild("ContextMenuAppointmentsLayout") as ContextMenu;
            this.ContextMenuAllDaysAppointmentsLayout = this.GetTemplateChild("ContextMenuAllDaysAppointmentsLayout") as ContextMenu;
#endif
            this.ContextMenuTimeLineItemsControl.DataContext = this;
            this.ContextMenuTimeSlot.DataContext = this;
            this.ContextMenuDaysHeaderControl.DataContext = this;
            this.ContextMenuAppointmentsLayout.DataContext = this;
            this.ContextMenuAllDaysAppointmentsLayout.DataContext = this;
            this.LoadContextMenuAllDayAppointmentItems();
            this.LoadContextMenuAppointmentItems();
            this.LoadContextMenuTimeSlotItems();
            this.LoadContextMenuTimeHourItems();
            this.LoadContextMenuDayHeaderItems();
        }

        private void DeInitializeContextMenu()
        {
            if (ContextMenuTimeSlot == null)
            {
#if SILVERLIGHT
                this.ContextMenuTimeLineItemsControl = this.GetTemplateChild("ContextMenuTimeLineItemsControl") as ContextMenuAdv;
                this.ContextMenuTimeSlot = this.GetTemplateChild("ContextMenuTimeSlot") as ContextMenuAdv;
                this.ContextMenuDaysHeaderControl = this.GetTemplateChild("ContextMenuDaysHeaderControl") as ContextMenuAdv;
                this.ContextMenuAppointmentsLayout = this.GetTemplateChild("ContextMenuAppointmentsLayout") as ContextMenuAdv;
                this.ContextMenuAllDaysAppointmentsLayout = this.GetTemplateChild("ContextMenuAllDaysAppointmentsLayout") as ContextMenuAdv;
#else
                this.ContextMenuTimeLineItemsControl = this.GetTemplateChild("ContextMenuTimeLineItemsControl") as ContextMenu;
                this.ContextMenuTimeSlot = this.GetTemplateChild("ContextMenuTimeSlot") as ContextMenu;
                this.ContextMenuDaysHeaderControl = this.GetTemplateChild("ContextMenuDaysHeaderControl") as ContextMenu;
                this.ContextMenuAppointmentsLayout = this.GetTemplateChild("ContextMenuAppointmentsLayout") as ContextMenu;
                this.ContextMenuAllDaysAppointmentsLayout = this.GetTemplateChild("ContextMenuAllDaysAppointmentsLayout") as ContextMenu;
#endif
            }
            ContextMenuTimeSlot.Visibility = Visibility.Collapsed;
            ContextMenuTimeLineItemsControl.Visibility = Visibility.Collapsed;
            ContextMenuDaysHeaderControl.Visibility = Visibility.Collapsed;
            ContextMenuAppointmentsLayout.Visibility = Visibility.Collapsed;
            ContextMenuAllDaysAppointmentsLayout.Visibility = Visibility.Collapsed;
            ContextMenuTimeLineItemsControl = null;
            ContextMenuTimeSlot = null;
            ContextMenuDaysHeaderControl = null;
            ContextMenuAppointmentsLayout = null;
            ContextMenuAllDaysAppointmentsLayout = null;
        }

        /// <summary>
        /// Gets ShowAs command
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
            //    case "Free":
            //        this.model.CurrentSelectedAppointment.Priority = AppointmentPriority.Free;
            //        break;
            //    case "Tentative":
            //        this.model.CurrentSelectedAppointment.Priority = AppointmentPriority.Tentative;
            //        break;
            //    case "Busy":
            //        this.model.CurrentSelectedAppointment.Priority = AppointmentPriority.Busy;
            //        break;
            //    case "OutofOffice":
            //        this.model.CurrentSelectedAppointment.Priority = AppointmentPriority.OutOfOffice;
            //        break;
            //    default:
            //        break;
            //}
        }

        /// <summary>
        /// Gets color button1 command from Context menu command
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
            this.ContextMenuTimeSlot.IsOpen = false;
            this.ContextMenuDaysHeaderControl.IsOpen = false;            
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
        /// Gets  OpenAppointment command from Context menu command
        /// </summary>
        public ContextMenuCommand OpenAppointment
        {
            get
            {
                if (openAppointment == null)
                {
                    openAppointment = new ContextMenuCommand(OpenAppointmentMethod);
                }

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
        /// Gets  deleteAppointment command from Context menu command
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
            if (!this.Model.AllowDelete)
            {
                return;
            }
            this.model.DeleteCurrentSelectedAppointment();
        }
        /// <summary>
        /// Gets   new Appointment command from Context menu command
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
        /// Gets  All day Appointment command from Context menu command
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
        /// Gets  NewRecurringAppointment command from Context menu command
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
        /// Gets  NewRecurring event command from Context menu command
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
        /// Gets  TimeIntervelSixtyMinutes command from Context menu command
        /// </summary>
        public ContextMenuCommand TimeIntervelSixtyMinutes
        {
            get
            {
                if (timeIntervelSixtyMinutes == null)
                    timeIntervelSixtyMinutes = new ContextMenuCommand(TimeIntervelSixtyMinutesMethod);

                return timeIntervelSixtyMinutes;
            }
        }
        /// <summary>
        /// Gets  TimeIntervelThirtyMinutes command from Context menu command
        /// </summary>
        public ContextMenuCommand TimeIntervelThirtyMinutes
        {
            get
            {
                if (timeIntervelThirtyMinutes == null)
                    timeIntervelThirtyMinutes = new ContextMenuCommand(TimeIntervelThirtyMinutesMethod);

                return timeIntervelThirtyMinutes;
            }
        }
        /// <summary>
        /// Gets TimeIntervelTwentyMinutes from ContextMenuCommand
        /// </summary>
        public ContextMenuCommand TimeIntervelTwentyMinutes
        {
            get
            {
                if (timeIntervelTwentyMinutes == null)
                    timeIntervelTwentyMinutes = new ContextMenuCommand(TimeIntervelTwentyMinutesMethod);

                return timeIntervelTwentyMinutes;
            }
        }
        /// <summary>
        /// Gets TimeIntervelFifteenMinutes from ContextMenuCommand
        /// </summary>
        public ContextMenuCommand TimeIntervelFifteenMinutes
        {
            get
            {
                if (timeIntervelFifteenMinutes == null)
                    timeIntervelFifteenMinutes = new ContextMenuCommand(TimeIntervelFifteenMinutesMethod);

                return timeIntervelFifteenMinutes;
            }
        }
        /// <summary>
        /// Gets TimeIntervelTenMinutes from ContextMenuCommand
        /// </summary>
        public ContextMenuCommand TimeIntervelTenMinutes
        {
            get
            {
                if (timeIntervelTenMinutes == null)
                    timeIntervelTenMinutes = new ContextMenuCommand(TimeIntervelTenMinutesMethod);

                return timeIntervelTenMinutes;
            }
        }
        /// <summary>
        /// Gets TimeIntervelSixMinutes from ContextMenuCommand
        /// </summary>
        public ContextMenuCommand TimeIntervelSixMinutes
        {
            get
            {
                if (timeIntervelSixMinutes == null)
                    timeIntervelSixMinutes = new ContextMenuCommand(TimeIntervelSixMinutesMethod);

                return timeIntervelSixMinutes;
            }
        }
        /// <summary>
        /// Gets TimeIntervelFiveMinutes from ContextMenuCommand
        /// </summary>
        public ContextMenuCommand TimeIntervelFiveMinutes
        {
            get
            {
                if (timeIntervelFiveMinutes == null)
                    timeIntervelFiveMinutes = new ContextMenuCommand(TimeIntervelFiveMinutesMethod);

                return timeIntervelFiveMinutes;
            }
        }

        private void TimeIntervelSixtyMinutesMethod(object parameter)
        {
            this.Model.CurrentTimeInterval = TimeInterval.OneHour;
        }

        private void TimeIntervelThirtyMinutesMethod(object parameter)
        {
            this.Model.CurrentTimeInterval = TimeInterval.ThirtyMin;
        }

        private void TimeIntervelTwentyMinutesMethod(object parameter)
        {
            this.Model.CurrentTimeInterval = TimeInterval.TwentyMin;
        }

        private void TimeIntervelFifteenMinutesMethod(object parameter)
        {
            this.Model.CurrentTimeInterval = TimeInterval.FifteenMin;
        }

        private void TimeIntervelTenMinutesMethod(object parameter)
        {
            this.Model.CurrentTimeInterval = TimeInterval.TenMin;
        }

        private void TimeIntervelSixMinutesMethod(object parameter)
        {
            this.Model.CurrentTimeInterval = TimeInterval.SixMin;
        }

        private void TimeIntervelFiveMinutesMethod(object parameter)
        {
            this.Model.CurrentTimeInterval = TimeInterval.FiveMin;
        }

        private void NewAppointmentMethod(object parameter)
        {
            if (this.Model.SelectedStartTimeSpan == DateTime.MinValue || this.Model.SelectedEndTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }
            var appointment = this.Model.CreateNewAppointment(false);
            this.ShowWindow(appointment, false);
        }

        private void NewRecurringAppointmentMethod(object parameter)
        {
            if (this.Model.SelectedStartTimeSpan == DateTime.MinValue || this.Model.SelectedEndTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }
            var appointment = this.Model.CreateNewAppointment(true);
            var appWrapper = new ScheduleAppointmentWrapper(appointment, this.Model, true);
            this.ShowWindow(appWrapper, Visibility.Visible);
        }

        private void NewRecurringEventMethod(object parameter)
        {
            var startTimeSpan = this.Model.SelectedStartTimeSpan;
            var endTimeSpan = this.Model.SelectedEndTimeSpan;
            if (startTimeSpan == DateTime.MinValue || endTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }
            var appointment = this.Model.CreateNewAppointment(true);
            var appWrapper = new ScheduleAppointmentWrapper(appointment, this.Model, true);
            this.ShowWindow(appWrapper, Visibility.Visible);
        }

        private void AllDayAppointmentMethod(object parameter)
        {
            var startTimeSpan = this.Model.SelectedStartTimeSpan;
            var endTimeSpan = this.Model.SelectedEndTimeSpan;
            if (startTimeSpan == DateTime.MinValue || endTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }
            var appointment = new ScheduleAppointment() { StartTime = startTimeSpan, EndTime = endTimeSpan, AllDay = true };
            this.ShowWindow(appointment, false);
        }
        /// <summary>
        /// Gets  TodayDate command from Context menu command
        /// </summary>
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
        /// Gets  GoToDate command from Context menu command
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
            childwindow.ResizeMode = ResizeMode.NoResize;
            childwindow.Height = 130;
            childwindow.WindowStartupLocation = WindowStartupLocation.Manual;
            Schedule s = this.GetScheduleParent();
            Point schedulePoint = s.PointToScreen(new Point());
            childwindow.Left = schedulePoint.X + (s.ActualWidth- childwindow.Width) / 2;
            childwindow.Top = schedulePoint.Y + (s.ActualHeight - childwindow.Height) / 2;  
#endif
            childwindow.Content = content;
            GoToDateValues value = new GoToDateValues(this.Model);
            childwindow.DataContext = value;
            content.KeyUp += new KeyEventHandler(childwindow_KeyUp);

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
#if !SILVERLIGHT
            childwindow.ShowDialog();
#else
            childwindow.Show();
#endif

        }



        void childwindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GotoDateWindow(sender);
            }

        }

        void content_OkButtonClick(object sender, RoutedEventArgs e)
        {
            GotoDateWindow(sender);
        }

        private void GotoDateWindow(object sender)
        {
            var editor = sender as GoToDateWindow;
            if (editor.PART_ViewCombo.SelectedItem == null)
            {
                return;
            }
            if (!editor.PART_GoToDatePicker.SelectedDate.HasValue)
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
            else if (s == "Week Calender")
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
            else if (s == "WorkWeek Calender")
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

        private void EnsureProperties()
        {
            if (this.isAppointStyleChangedBeforeLoaded)
            {
                this.appointmentsLayoutItems.AppointmentStyle = this.AppointmentStyle;
            }

            if (this.isAllDayAppointmentStyleChangedBeforeLoaded)
            {
                this.alldaysAppointmentsLayoutPanel.AllDayAppointmentStyle = this.AllDayAppointmentStyle;
            }
        }

#if !SILVERLIGHT
        #region InputHitTest Result / Returns Object
        private DependencyObject GetHitTestParentElement(IInputElement childobj, string dependencyobjType)
        {
            FrameworkElement dependencyObj = childobj as FrameworkElement;

            if (dependencyObj != null)
            {
                if (dependencyObj.TemplatedParent.DependencyObjectType.Name == dependencyobjType)
                    return dependencyObj.TemplatedParent;
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

        #region AppointmentEvents
        private void SetupAppointmentEvents()
        {
            //newly added
            this.appointmentsLayoutItems.MouseRightClick += new MouseButtonEventHandler(appointmentsLayoutItems_MouseRightButtonDown);
            this.alldaysAppointmentsLayoutPanel.MouseRightClick += new MouseButtonEventHandler(alldaysAppointmentsLayoutPanel_MouseRightClick);

            this.appointmentsLayoutItems.MouseLeftButtonDown += new MouseButtonEventHandler(appointmentsLayoutItems_MouseLeftButtonDown);
            this.appointmentsLayoutItems.MouseMove += new MouseEventHandler(appointmentsLayoutItems_MouseMove);
            this.appointmentsLayoutItems.MouseLeftButtonUp += new MouseButtonEventHandler(appointmentsLayoutItems_MouseLeftButtonUp);
            this.alldaysAppointmentsLayoutPanel.MouseLeftButtonDown += new MouseButtonEventHandler(alldaysAppointmentsLayoutPanel_MouseLeftButtonDown);
            this.alldaysAppointmentsLayoutPanel.MouseLeftButtonUp += new MouseButtonEventHandler(alldaysAppointmentsLayoutPanel_MouseLeftButtonUp);
#if SILVERLIGHT
            this.alldaysAppointmentsLayoutPanel.MouseLeftButtonUp += new MouseButtonEventHandler(alldaysAppointmentsLayoutPanel_MouseLeftButtonUp);
#endif
            this.MouseMove += new MouseEventHandler(ScheduleDaysView_MouseMove);
#if !SILVERLIGHT
            this.DragPopUp.MouseLeftButtonUp += new MouseButtonEventHandler(appointmentsLayoutItems_MouseLeftButtonUp);
            this.DragPopUp.MouseMove += new MouseEventHandler(ScheduleDaysView_MouseMove); 
#endif
        }

        //newly added
        void alldaysAppointmentsLayoutPanel_MouseRightClick(object sender, MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            FrameworkElement sndr = sender as FrameworkElement;
            sndr.CaptureMouse();
            ScheduleDaysAppointmentViewControl el = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleDaysAppointmentViewControl>().FirstOrDefault();
#else
            ScheduleDaysAppointmentViewControl el = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleDaysAppointmentViewControl") as ScheduleDaysAppointmentViewControl;
#endif

            if (el == null) return;
            if (this.Model != null)
            {
                ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(el.ScheduleAppointment);
                this.Model.GetAppointmentClickEvents(args);
            }
            this.ClearAppointmentLayoutSelectedItems();
            this.ClearAllDayAppointmentLayoutSelectedItems();
            this.ClearTimeSlotSelection();
            this.ClearDaysHeaderSelection();
            el.IsSelected = true;
            this.Model.CurrentSelectedAppointment = el.ScheduleAppointment;
            //this.PickUpAppointment(el);
        }

        //newly added
        void appointmentsLayoutItems_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            timeInterval = 60 / (ScheduleTimeLineHourControl.IntervalCount[(int)this.Model.CurrentTimeInterval]);
            this.SetDefaultValuesForResize();
#if SILVERLIGHT
            FrameworkElement sendr = sender as FrameworkElement;
            sendr.CaptureMouse();
            ScheduleDaysAppointmentViewControl el = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleDaysAppointmentViewControl>().FirstOrDefault();
#else
            ScheduleDaysAppointmentViewControl el = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleDaysAppointmentViewControl") as ScheduleDaysAppointmentViewControl;
            Border elbdrtop = GetHitTestParentElementByName(this.InputHitTest(e.GetPosition(this)), "ScheduleDaysAppointmentViewControl", "PART_TopEdge") as Border;
            Border elbdrbottom = GetHitTestParentElementByName(this.InputHitTest(e.GetPosition(this)), "ScheduleDaysAppointmentViewControl", "PART_BottomEdge") as Border;
#endif
            if (el == null) return;

            if (this.Model != null)
            {
                ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(el.ScheduleAppointment);
                this.Model.GetAppointmentClickEvents(args);
            }
            StartingPoint = e.GetPosition(el);
            this.ClearAppointmentLayoutSelectedItems();
            this.ClearAllDayAppointmentLayoutSelectedItems();
            this.ClearTimeSlotSelection();
            this.ClearDaysHeaderSelection();
            el.IsSelected = true;
            this.Model.CurrentSelectedAppointment = el.ScheduleAppointment;

        }



        private void ScheduleDaysView_MouseMove(object sender, MouseEventArgs e)
        {
            var schedule = (sender as ScheduleDaysView).FindParentElementOfType<Schedule>();
            if (this.isDragging && schedule!=null && schedule.SelectedAppointment.AllowDragandDrop)
            {
                Schedule sch = GetScheduleParent();
                var pt = e.GetPosition(AppointmentPopup);
                // checking 10 is rough value, to avoid drggging in click event of appointment in WPF , which listens minute move between UP and DOWN event 
                if (Math.Abs(pt.Y - (this.currentVerticalPosition + 10)) > 0 || Math.Abs(pt.X - (this.currentHorizontalPosition + 10)) > 0)
                {
                    sch.OnAppointmentDragged(new ScheduleAppointmentDragEventArgs(this.draggedAppointment));
                    if (draggedAppointment.IsRecurrenceAppointment == false && draggedAppointment.IsAppointmentProxyCollection == false)
                    {
                        currentVerticalPosition = e.GetPosition(AppointmentPopup).Y;
                        currentHorizontalPosition = e.GetPosition(AppointmentPopup).X;
#if !SILVERLIGHT
                    this.DragPopUp.AllowsTransparency = true;                   
#endif
                        //this.DragPopUp.Placement = PlacementMode.AbsolutePoint;
                        //this.DragPopUp.PlacementTarget = AppointmentPopup;
                        this.DragPopUp.VerticalOffset = currentVerticalPosition;
                        this.DragPopUp.HorizontalOffset = currentHorizontalPosition;

                        DragPopUp.IsOpen = true;
                        this.isDragged = true;
                    }
                    else
                    {
                        this.AppointmentPopup.Children.Remove(DragPopUp);
                        this.isDragging = false;
                        MessageBox.Show("Cannot Reshedule an occurrance of recurring appointment");
                    }
                }
            }
        }

        private void alldaysAppointmentsLayoutPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            FrameworkElement sndr = sender as FrameworkElement;
            sndr.CaptureMouse();
            ScheduleDaysAppointmentViewControl el = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleDaysAppointmentViewControl>().FirstOrDefault();
#else
            ScheduleDaysAppointmentViewControl el = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleDaysAppointmentViewControl") as ScheduleDaysAppointmentViewControl;
#endif

            if (el == null) return;
            if (this.Model != null)
            {
                ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(el.ScheduleAppointment);
                this.Model.GetAppointmentClickEvents(args);
            }
            this.ClearAppointmentLayoutSelectedItems();
            this.ClearAllDayAppointmentLayoutSelectedItems();
            this.ClearTimeSlotSelection();
            this.ClearDaysHeaderSelection();
            el.IsSelected = true;
            this.Model.CurrentSelectedAppointment = el.ScheduleAppointment;
            this.currentHorizontalPosition = e.GetPosition(AppointmentPopup).X;
            this.currentVerticalPosition = e.GetPosition(AppointmentPopup).Y;
            this.PickUpAppointment(el);
        }
        private void alldaysAppointmentsLayoutPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DropAppointment(e);
        }
        ScheduleDaysAppointmentViewControl mouseDownDaysAppEl = null;
        private bool isDaysAppMouseDown = false;
        private bool isallowResized = false;
        private Point StartingPoint;
        private double actualAppHeight = 0;
        private Popup popupResize;
        private Popup DragPopUp;
        double currentVerticalPosition = 0;
        double currentHorizontalPosition = 0;
        private bool isDragged = false;
        private bool isDragging = false;
        private ScheduleAppointment draggedAppointment;
        private int timeInterval = 30;


        private void appointmentsLayoutItems_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            timeInterval = 60 / (ScheduleTimeLineHourControl.IntervalCount[(int)this.Model.CurrentTimeInterval]);
            this.SetDefaultValuesForResize();
#if SILVERLIGHT
            FrameworkElement sendr = sender as FrameworkElement;
            sendr.CaptureMouse();
            ScheduleDaysAppointmentViewControl el = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleDaysAppointmentViewControl>().FirstOrDefault();
#else
            ScheduleDaysAppointmentViewControl el = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleDaysAppointmentViewControl") as ScheduleDaysAppointmentViewControl;
            Border elbdrtop = GetHitTestParentElementByName(this.InputHitTest(e.GetPosition(this)), "ScheduleDaysAppointmentViewControl", "PART_TopEdge") as Border;
            Border elbdrbottom = GetHitTestParentElementByName(this.InputHitTest(e.GetPosition(this)), "ScheduleDaysAppointmentViewControl", "PART_BottomEdge") as Border;
#endif
            if (el == null) return;

            if (this.Model != null)
            {
                ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(el.ScheduleAppointment);
                this.Model.GetAppointmentClickEvents(args);
            }
            StartingPoint = e.GetPosition(el);
            this.ClearAppointmentLayoutSelectedItems();
            this.ClearAllDayAppointmentLayoutSelectedItems();
            this.ClearTimeSlotSelection();
            this.ClearDaysHeaderSelection();
            MarkSelectedAppointments(el);
            this.Model.CurrentSelectedAppointment = el.ScheduleAppointment;
#if SILVERLIGHT
            el.CaptureMouse();
#endif

#if !SILVERLIGHT
            if (elbdrbottom != null)
            {
                el.DragStatus = MousePointerType.Resize;
                el.MousePosition = ResizePosition.Bottom;
            }
            else if (elbdrtop != null)
            {
                el.DragStatus = MousePointerType.Resize;
                el.MousePosition = ResizePosition.Top;
            }
            else
            {
                el.DragStatus = MousePointerType.None;
                el.MousePosition = ResizePosition.None;
            }
#endif
            if (el.DragStatus == MousePointerType.Resize && this.Model.AllowResize == true && el.ScheduleAppointment.AllowResize == true)
            {
                Point currpopMargin = e.GetPosition(sender as FrameworkElement);

                this.ClearAppointmentPopupWindow();
                popupResize = GetPopUpWithContent(el);
#if SILVERLIGHT
                popupResize.Margin = new Thickness(currpopMargin.X - StartingPoint.X, currpopMargin.Y - StartingPoint.Y, 0, 0);
#else
                try
                {                    
                    Point absoluteScreenPos = PointToScreen(Mouse.GetPosition(this));
                    Point StartingPointEle = e.GetPosition(el);                   
                    (popupResize.Child as Border).Margin = new Thickness(absoluteScreenPos.X - StartingPointEle.X , absoluteScreenPos.Y - StartingPointEle.Y , 0, 0);
                    popupResize.IsOpen = true;                        
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debugger.Break();
                    throw(ex);
                }
                // Older source 
                //Point MainWindowPoint = e.GetPosition(Application.Current.MainWindow);
                //double leftValue = MainWindowPoint.X + Application.Current.MainWindow.Left - StartingPointEle.X + 8;
                //double topValue = MainWindowPoint.Y + Application.Current.MainWindow.Top - StartingPointEle.Y + 30;
                //(popupResize.Child as Border).Margin = new Thickness(leftValue, topValue, 0, 0);
                //(popupResize.Child as Border).Margin = new Thickness(StartingPointEle.X, StartingPointEle.Y, 0, 0);
                // popupResize.IsOpen = true;
#endif
                this.AppointmentPopup.Children.Add(popupResize);

                isDaysAppMouseDown = true;
                mouseDownDaysAppEl = el;

                if (double.IsNaN(mouseDownDaysAppEl.Height))
                    actualAppHeight = mouseDownDaysAppEl.ActualHeight;
                else
                    actualAppHeight = mouseDownDaysAppEl.Height;

                actualAppHeight = actualAppHeight / (mouseDownDaysAppEl.ScheduleAppointment.Duration.TotalMinutes / timeInterval);
            }
            else
            {
                this.currentHorizontalPosition = e.GetPosition(AppointmentPopup).X;
                this.currentVerticalPosition = e.GetPosition(AppointmentPopup).Y;
                this.PickUpAppointment(el);
            }
        }

        private void MarkSelectedAppointments(ScheduleDaysAppointmentViewControl el)
        {
            if (el.ScheduleAppointment.MultiDayAppointment)
            {
#if SILVERLIGHT
                var appCtl = from item in this.appointmentsLayoutItems.Items
                             where (((this.appointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleDaysAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentStartTime == el.ScheduleAppointment.MultiDayAppointmentStartTime) && ((this.appointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleDaysAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentEndTime == el.ScheduleAppointment.MultiDayAppointmentEndTime))
                             select item;
#endif
#if !SILVERLIGHT
                var appCtl = from item in this.appointmentsLayoutItems.Items.Cast<ScheduleAppointment>()
                             where (((this.appointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleDaysAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentStartTime == el.ScheduleAppointment.MultiDayAppointmentStartTime) && ((this.appointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleDaysAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentEndTime == el.ScheduleAppointment.MultiDayAppointmentEndTime))
                             select item;
#endif

                if (appCtl.Count() > 0)
                {
                    for (int i = 0; i < appCtl.Count(); i++)
                    {
                        (this.appointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(appCtl.ElementAt(i)) as ScheduleDaysAppointmentViewControl).IsSelected = true;
                    }
                }
            }
            else
                el.IsSelected = true;
        }

        private void ClearAppointmentPopupWindow()
        {
            if (popupResize != null)
            {
                popupResize.IsOpen = false;
                popupResize = null;
            }
            this.AppointmentPopup.Children.Clear();
        }

        private Popup GetPopUpWithContent(ScheduleDaysAppointmentViewControl mouseDownDaysAppEl)
        {
            double popwidth = double.IsNaN(mouseDownDaysAppEl.Width) ? mouseDownDaysAppEl.ActualWidth : mouseDownDaysAppEl.Width;
            double popheight = double.IsNaN(mouseDownDaysAppEl.Height) ? mouseDownDaysAppEl.ActualHeight : mouseDownDaysAppEl.Height;
#if SILVERLIGHT
            var popup = new Popup()
            {
                IsOpen = true
            };
            popup.MouseMove += new MouseEventHandler(appointmentsLayoutItems_MouseMove);
            popup.MouseLeftButtonUp += new MouseButtonEventHandler(appointmentsLayoutItems_MouseLeftButtonUp);
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
            popup.MouseMove += new MouseEventHandler(appointmentsLayoutItems_MouseMove);
            popup.MouseLeftButtonUp += new MouseButtonEventHandler(appointmentsLayoutItems_MouseLeftButtonUp);
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

        private void appointmentsLayoutItems_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDaysAppMouseDown || this.Model.AllowResize == false || mouseDownDaysAppEl == null) return;

            if (moveResizeargs != null && moveResizeargs.Cancel == true) return;

#if SILVERLIGHT
            ScheduleDaysAppointmentViewControl elrep = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleDaysAppointmentViewControl>().FirstOrDefault();
#else
            ScheduleDaysAppointmentViewControl elrep = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleDaysAppointmentViewControl") as ScheduleDaysAppointmentViewControl;
#endif
            Point curroffset = e.GetPosition(mouseDownDaysAppEl);

            if ((curroffset.X == 0 && curroffset.Y == 0) || mouseDownDaysAppEl.ScheduleAppointment.AllowResize == false) return;

            if (double.IsNaN(mouseDownDaysAppEl.Height))
                mouseDownDaysAppEl.Height = mouseDownDaysAppEl.ActualHeight;

            double diffHeight = curroffset.Y - StartingPoint.Y;
            if (diffHeight != 0) isallowResized = true;
            var startTempTime = new DateTime();
            var endTempTime = new DateTime();

            if (mouseDownDaysAppEl.DragStatus == MousePointerType.Resize && mouseDownDaysAppEl.MousePosition == ResizePosition.Bottom && diffHeight != 0)
            {
                if (mouseDownDaysAppEl.Height + diffHeight <= 0) return;
                mouseDownDaysAppEl.Height += diffHeight;
                if (popupResize != null)
                {
                    var elementApp = popupResize.Child as FrameworkElement;
                    if (elementApp.Height + diffHeight <= 0) return;
                    elementApp.Height += diffHeight;
                }
            }
            else if (mouseDownDaysAppEl.DragStatus == MousePointerType.Resize && mouseDownDaysAppEl.MousePosition == ResizePosition.Top && diffHeight != 0)
            {
                if (mouseDownDaysAppEl.Height + diffHeight <= 0 || mouseDownDaysAppEl.Height - diffHeight <= 0) return;
                mouseDownDaysAppEl.Height -= diffHeight;

                if (popupResize != null)
                {
                    var elementApp = popupResize.Child as FrameworkElement;
                    if (elementApp.Height + diffHeight <= 0 || elementApp.Height - diffHeight <= 0) return;
                    elementApp.Height -= diffHeight;
#if SILVERLIGHT
                    popupResize.Margin = new Thickness(popupResize.Margin.Left, popupResize.Margin.Top + diffHeight, 0, 0);
#else
                    double leftval = (popupResize.Child as FrameworkElement).Margin.Left;
                    double topval = (popupResize.Child as FrameworkElement).Margin.Top;
                    (popupResize.Child as FrameworkElement).Margin = new Thickness(leftval, topval + diffHeight, 0, 0);
#endif
                }
            }
            if (this.Model != null)
            {
                int currCount = Convert.ToInt32(Math.Floor(mouseDownDaysAppEl.Height / actualAppHeight));
                double totmin = timeInterval * currCount;
                if (mouseDownDaysAppEl.MousePosition == ResizePosition.Bottom)
                {
                    while (mouseDownDaysAppEl.ScheduleAppointment.StartTime.AddMinutes(totmin).Day > mouseDownDaysAppEl.ScheduleAppointment.StartTime.Day)
                    {
                        totmin -= 0.01;
                    }
                    endTempTime = mouseDownDaysAppEl.ScheduleAppointment.StartTime.AddMinutes(totmin);
                }
                else endTempTime = mouseDownDaysAppEl.ScheduleAppointment.EndTime;
                if (mouseDownDaysAppEl.MousePosition == ResizePosition.Top)
                {
                    while (mouseDownDaysAppEl.ScheduleAppointment.EndTime.AddMinutes(-totmin).Day < mouseDownDaysAppEl.ScheduleAppointment.EndTime.Day)
                    {
                        totmin -= timeInterval;
                    }
                    startTempTime = mouseDownDaysAppEl.ScheduleAppointment.EndTime.AddMinutes(-totmin);
                }
                else startTempTime = mouseDownDaysAppEl.ScheduleAppointment.StartTime;
                moveResizeargs = new ScheduleAppointmentResizingEventArgs(startTempTime, endTempTime);
                this.Model.GetAppointmentResizingEvents(moveResizeargs);
            }

            StartingPoint = curroffset;
        }

        ScheduleAppointmentResizingEventArgs moveResizeargs;
        private void appointmentsLayoutItems_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ClearAppointmentPopupWindow();
            if (isallowResized && isDaysAppMouseDown && mouseDownDaysAppEl != null && this.Model.AllowResize == true && mouseDownDaysAppEl.ScheduleAppointment.AllowResize == true)
            {
#if SILVERLIGHT
                FrameworkElement sendr = sender as FrameworkElement;
                sendr.ReleaseMouseCapture();
#endif
                if (double.IsNaN(mouseDownDaysAppEl.Height))
                    mouseDownDaysAppEl.Height = mouseDownDaysAppEl.ActualHeight;

                var startTime = mouseDownDaysAppEl.ScheduleAppointment.StartTime;
                var endTime = mouseDownDaysAppEl.ScheduleAppointment.EndTime;

                if (actualAppHeight > 0 && !double.IsNaN(mouseDownDaysAppEl.Height) && mouseDownDaysAppEl.DragStatus == MousePointerType.Resize
                    && mouseDownDaysAppEl.MousePosition != ResizePosition.None)
                {
                    int currCount = Convert.ToInt32(Math.Floor(mouseDownDaysAppEl.Height / actualAppHeight));
                    double remaniner = actualAppHeight - (mouseDownDaysAppEl.Height % actualAppHeight);
                    currCount = (remaniner > 0) ? currCount + 1 : currCount;
                    mouseDownDaysAppEl.Height = (remaniner > 0) ? mouseDownDaysAppEl.Height + remaniner : mouseDownDaysAppEl.Height;
                    double totmin = timeInterval * currCount;

                    var remApp = (from res in this.Model.Appointments
                                  where res.Subject == mouseDownDaysAppEl.ScheduleAppointment.Subject && res.Location == mouseDownDaysAppEl.ScheduleAppointment.Location
                                  && res.StartTime == mouseDownDaysAppEl.ScheduleAppointment.StartTime && res.EndTime == mouseDownDaysAppEl.ScheduleAppointment.EndTime
                                  select res).FirstOrDefault();

                    if (mouseDownDaysAppEl.MousePosition == ResizePosition.Bottom)
                    {
                        while (mouseDownDaysAppEl.ScheduleAppointment.StartTime.AddMinutes(totmin).Day > mouseDownDaysAppEl.ScheduleAppointment.StartTime.Day)
                        {
                            totmin -= 0.01;
                        }
                        mouseDownDaysAppEl.ScheduleAppointment.EndTime = mouseDownDaysAppEl.ScheduleAppointment.StartTime.AddMinutes(totmin);
                    }
                    else if (mouseDownDaysAppEl.MousePosition == ResizePosition.Top)
                    {
                        while (mouseDownDaysAppEl.ScheduleAppointment.EndTime.AddMinutes(-totmin).Day < mouseDownDaysAppEl.ScheduleAppointment.EndTime.Day)
                        {
                            totmin -= timeInterval;
                        }
                        mouseDownDaysAppEl.ScheduleAppointment.StartTime = mouseDownDaysAppEl.ScheduleAppointment.EndTime.AddMinutes(-totmin);
                    }

                    if (mouseDownDaysAppEl.ScheduleAppointment.StartTime >= mouseDownDaysAppEl.ScheduleAppointment.EndTime)
                    {
                        mouseDownDaysAppEl.ScheduleAppointment.EndTime = mouseDownDaysAppEl.ScheduleAppointment.StartTime.AddMinutes(timeInterval);
                    }


                    var newapp = new ScheduleAppointment();
                    newapp.InitializeFromApp(mouseDownDaysAppEl.ScheduleAppointment);
                    UpdateResizedAppointment(remApp, newapp);
                    
                    // this.UpdateLayout();

                    //var addApp = new ScheduleAppointment();
                    //addApp.InitializeFromApp(app);
                    //this.Model.Appointments.Add(addApp);
                    //this.UpdateLayout();
                    foreach (var item in this.appointmentsLayoutItems.Items)
                    {
                        var viewControl = this.appointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleDaysAppointmentViewControl;
                        if (viewControl != null && viewControl.ScheduleAppointment.MatchWithExists(newapp) == true)
                        {
                            viewControl.IsSelected = true;
                            break;
                        }
                    }
                }

                if (this.Model != null)
                {
                    ScheduleAppointmentResizedEventArgs args = new ScheduleAppointmentResizedEventArgs(draggedAppointment, startTime, endTime);
                    this.Model.GetAppointmentResizedEvents(args);
                }
            }
            else
            {
                DropAppointment(e);
            }
            this.SetDefaultValuesForResize();
        }

        private void UpdateResizedAppointment(ScheduleAppointment remApp, ScheduleAppointment app)
        {
            var resapp = (from appt in this.Model.Appointments
                         where appt == remApp
                         select appt).FirstOrDefault();
            if (resapp != null)
            {
                var index = this.Model.Appointments.IndexOf(resapp);
                this.appointmentsLayoutItems.Items.Remove(resapp);               
                this.draggedAppointment.StartTime = app.StartTime;
                this.draggedAppointment.EndTime = app.EndTime;
                this.draggedAppointment.MultiDayAppointmentStartTime = app.StartTime;
                this.draggedAppointment.MultiDayAppointmentEndTime = app.EndTime;
                this.draggedAppointment.AllDay = false;
                
                this.Model.Appointments[index].StartTime = app.StartTime;
                this.Model.Appointments[index].EndTime = app.EndTime;
                this.Model.Appointments[index].AllDay = false;

                this.appointmentsLayoutItems.Items.Add(this.Model.Appointments[index]);
                if (app.Record != null)
                {
                    var record = draggedAppointment.Record;
                    this.Model.View.EditItem(record.Data);
                    var pd = this.Model.View.GetPropertyAccessProvider();
                    this.Model.SetPropertiesOnNewItem(pd, record.Data, app);
                    this.Model.View.CommitEdit();
                }
            }           
            this.SetupAppointments();
        }
#if !SILVERLIGHT
        internal ScheduleAppointment GetTimeIntevalFromMousePoint(RoutedEventArgs e)
        {
            
#if SILVERLIGHT
            ScheduleDaysView v = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleDaysView>().FirstOrDefault();
#else
                ScheduleDaysView v = FindAnchestor<ScheduleDaysView>((DependencyObject)e.OriginalSource);
#endif
            if (v != null)
            {
                
#if SILVERLIGHT
                    ScheduleTimeSlotControl slot = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleTimeSlotControl>().FirstOrDefault();
#else
                    ScheduleTimeSlotControl slot = FindAnchestor<ScheduleTimeSlotControl>((DependencyObject)e.OriginalSource);
#endif
                    if (slot != null)
                    {
#if SILVERLIGHT
                        UniformStackPanel usp = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<UniformStackPanel>().FirstOrDefault();
#else
                        UniformStackPanel usp = FindAnchestor<UniformStackPanel>((DependencyObject)e.OriginalSource);

#endif
                        if (usp != null)
                        {
                            int count = VisualTreeHelper.GetChildrenCount(usp);
#if SILVERLIGHT
                            ScheduleRectangleBorderExt rectangle = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), usp).OfType<ScheduleRectangleBorderExt>().FirstOrDefault();
#else
                          //  ScheduleRectangleBorderExt rectangle = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleRectangleBorderExt") as ScheduleRectangleBorderExt;
                            ScheduleRectangleBorderExt rectangle = FindAnchestor<ScheduleRectangleBorderExt>((DependencyObject)e.OriginalSource);
#endif
                            int position = usp.Children.IndexOf(rectangle);
                            int min = (60 / usp.Children.Count) * position;
                            DateTime startTime  = slot.DateTime;
                            startTime = startTime.Add(new TimeSpan(slot.Hour, min, 0));
                            DateTime endTime = startTime.Add(TimeIntervalToTimeSpanConverter(this.model.CurrentTimeInterval));
                            ScheduleAppointment newApp = new ScheduleAppointment() { StartTime = startTime, EndTime = endTime, AllDay = false };
                            return newApp;
                        }
                    }

#if SILVERLIGHT
                    ScheduleDaysHeaderViewControl view = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleDaysHeaderViewControl>().FirstOrDefault();
#else
                    ScheduleDaysHeaderViewControl view = FindAnchestor<ScheduleDaysHeaderViewControl>((DependencyObject)e.OriginalSource);


#endif
                    if (view != null)
                    {
                        DateTime Startappdate = view.DateTime;
                        ScheduleAppointment app = new ScheduleAppointment();                        
                        app.StartTime = Startappdate;
                        app.EndTime = app.StartTime.Add(TimeIntervalToTimeSpanConverter(this.model.CurrentTimeInterval));
                        app.AllDay = true;
                        return app;
                    }
                    else
                    {
                        ScheduleDaysAppointmentViewControl appView = FindAnchestor<ScheduleDaysAppointmentViewControl>((DependencyObject)e.OriginalSource);
                        if (appView != null)
                        {
                            DateTime Startappdate = appView.ScheduleAppointment.StartTime;
                            ScheduleAppointment app = new ScheduleAppointment();
                            app.StartTime = Startappdate;
                            app.EndTime = app.StartTime.Add(TimeIntervalToTimeSpanConverter(this.model.CurrentTimeInterval));
                            app.AllDay = true;
                            return app;
                        }
                        else
                        {
                            DateTime Startappdate = this.model.SelectedDates[(int)((DragEventArgs)e).GetPosition(this.timeSlotItems).X / ((int)this.timeSlotItems.ActualWidth / this.model.SelectedDates.Count)];
                            Startappdate = Startappdate.AddMinutes(((int)((DragEventArgs)e).GetPosition(this.timeSlotItems).Y / (int)this.model.IntervalHeight) * this.model.GetCurrentTimeIntervalInMinutes());
                            ScheduleAppointment app = new ScheduleAppointment();
                            app.StartTime = Startappdate;
                            app.EndTime = app.StartTime.Add(TimeIntervalToTimeSpanConverter(this.model.CurrentTimeInterval));
                            app.AllDay = true;
                            return app;
                        }
                    }
                }
            return null;
            
        }
#endif

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
        private void DropAppointment(MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            ScheduleDaysView v = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleDaysView>().FirstOrDefault();
#else
                ScheduleDaysView v = FindAnchestor<ScheduleDaysView>((DependencyObject)e.OriginalSource);
#endif
            if (v != null)
            {
                if (this.isDragged && !isDaysAppMouseDown)
                {
#if SILVERLIGHT
                    ScheduleTimeSlotControl slot = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleTimeSlotControl>().FirstOrDefault();
#else
                    ScheduleTimeSlotControl slot = FindAnchestor<ScheduleTimeSlotControl>((DependencyObject)e.OriginalSource);
#endif
                    if (slot != null)
                    {
#if SILVERLIGHT
                        UniformStackPanel usp = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<UniformStackPanel>().FirstOrDefault();
#else
                        UniformStackPanel usp = FindAnchestor<UniformStackPanel>((DependencyObject)e.OriginalSource);

#endif
                        if (usp != null)
                        {
                            int count = VisualTreeHelper.GetChildrenCount(usp);
#if SILVERLIGHT
                            ScheduleRectangleBorderExt rectangle = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), usp).OfType<ScheduleRectangleBorderExt>().FirstOrDefault();
#else
                            ScheduleRectangleBorderExt rectangle = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleRectangleBorderExt") as ScheduleRectangleBorderExt;
#endif
                            int position = usp.Children.IndexOf(rectangle);
                            int min = (60 / usp.Children.Count) * position;
                            DateTime newappdate = slot.DateTime;
                            newappdate = newappdate.Add(new TimeSpan(slot.Hour, min, 0));

                            if (this.draggedAppointment.MultiDayAppointment)
                            {
                                DragDropMultiDay(newappdate);
                            }
                            else//SD17775 Fixed // if (newappdate < draggedAppointment.StartTime || newappdate > draggedAppointment.EndTime)
                            {
                                ScheduleAppointment newapp = new ScheduleAppointment();
                                newapp.InitializeFromApp(this.draggedAppointment);
                                newapp.StartTime = newappdate;
                                if (draggedAppointment.Duration == new TimeSpan(0, 0, 0))
                                {
                                    newapp.EndTime = newapp.StartTime.Add(TimeIntervalToTimeSpanConverter(this.model.CurrentTimeInterval));
                                }
                                else
                                {
                                    newapp.EndTime = newapp.StartTime.Add(draggedAppointment.Duration);
                                }
                                newapp.AllDay = false;
                                Schedule sch = GetScheduleParent();
                                sch.OnAppointmentDropping(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                                if (newapp.StartTime != draggedAppointment.StartTime || newapp.EndTime != draggedAppointment.EndTime || newapp.Location != draggedAppointment.Location || newapp.Subject != draggedAppointment.Subject)
                                {
                                    UpdateDraggedAppointment(newapp);
                                    this.appointmentsLayoutItems.Items.Remove(this.draggedAppointment);
                                    this.draggedAppointment.StartTime = newapp.StartTime;
                                    this.draggedAppointment.EndTime = newapp.EndTime;
                                    this.draggedAppointment.AllDay = false;
                                    this.appointmentsLayoutItems.Items.Add(this.draggedAppointment);
                                    if (this.draggedAppointment.Record != null)
                                    {
                                        var record = draggedAppointment.Record;
                                        this.Model.View.EditItem(record.Data);
                                        var pd = this.Model.View.GetPropertyAccessProvider();
                                        this.Model.SetPropertiesOnNewItem(pd, record.Data, this.draggedAppointment);
                                        this.Model.View.CommitEdit();
                                    }
                                }
                                this.UpdateLayout();
                                sch.OnAppointmentDropped(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                            }
                        }
                    }
                    else
                    {

                        DateTime newappdate = this.model.SelectedDates[(int)e.GetPosition(this.timeSlotItems).X / ((int)this.timeSlotItems.ActualWidth / this.model.SelectedDates.Count)];
                        newappdate = newappdate.AddMinutes(((int)e.GetPosition(this.timeSlotItems).Y / ((int)this.model.IntervalHeight-1)) * this.model.GetCurrentTimeIntervalInMinutes());

                        //ScheduleDaysAppointmentViewControl appView = FindAnchestor<ScheduleDaysAppointmentViewControl>((DependencyObject)e.OriginalSource);
                        //DateTime newappdate = appView.ScheduleAppointment.StartTime;
                        //SD17775 Fixed //if (newappdate < draggedAppointment.StartTime || newappdate > draggedAppointment.EndTime)
                        {
                            this.appointmentsLayoutItems.Items.Remove(this.draggedAppointment);
                            ScheduleAppointment newapp = new ScheduleAppointment();
                            newapp.InitializeFromApp(this.draggedAppointment);
                            newapp.StartTime = newappdate;
                            if (draggedAppointment.Duration == new TimeSpan(0, 0, 0))
                            {
                                newapp.EndTime = newapp.StartTime.Add(TimeIntervalToTimeSpanConverter(this.model.CurrentTimeInterval));
                            }
                            else
                            {
                                newapp.EndTime = newapp.StartTime.Add(draggedAppointment.Duration);
                            }
                            UpdateDraggedAppointment(newapp);
                            newapp.AllDay = false;
                            this.appointmentsLayoutItems.Items.Add(newapp);
                            Schedule sch = GetScheduleParent();
                            sch.OnAppointmentDropping(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                            if (newapp.StartTime != draggedAppointment.StartTime || newapp.EndTime != draggedAppointment.EndTime || newapp.Location != draggedAppointment.Location || newapp.Subject != draggedAppointment.Subject)
                            {

                                this.appointmentsLayoutItems.Items.Remove(this.draggedAppointment);
                                this.draggedAppointment.StartTime = newapp.StartTime;
                                this.draggedAppointment.EndTime = newapp.EndTime;
                                this.draggedAppointment.AllDay = false;
                               // this.appointmentsLayoutItems.Items.Add(this.draggedAppointment);
                                if (this.draggedAppointment.Record != null)
                                {
                                    var record = draggedAppointment.Record;
                                    this.Model.View.EditItem(record.Data);
                                    var pd = this.Model.View.GetPropertyAccessProvider();
                                    this.Model.SetPropertiesOnNewItem(pd, record.Data, this.draggedAppointment);
                                    this.Model.View.CommitEdit();
                                }
                            }
                            this.UpdateLayout();
                            sch.OnAppointmentDropped(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                        }
                    }
#if SILVERLIGHT
                    ScheduleDaysHeaderViewControl view = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleDaysHeaderViewControl>().FirstOrDefault();
#else
                    ScheduleDaysHeaderViewControl view = FindAnchestor<ScheduleDaysHeaderViewControl>((DependencyObject)e.OriginalSource);


#endif
                    if (view != null)
                    {
                        DateTime Startappdate = view.DateTime;
                        ScheduleAppointment app = new ScheduleAppointment();
                        app.InitializeFromApp(draggedAppointment);
                        app.StartTime = Startappdate;
                        app.EndTime = app.StartTime.Add(draggedAppointment.Duration);
                        app.AllDay = true;
                        Schedule sch = GetScheduleParent();
                        sch.OnAppointmentDropping(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                        if (app.StartTime != draggedAppointment.StartTime || app.EndTime != draggedAppointment.EndTime || app.Location != draggedAppointment.Location || app.Subject != draggedAppointment.Subject || app.AllDay != draggedAppointment.AllDay)
                        {
                            UpdateDraggedAppointment(app);
                            this.appointmentsLayoutItems.Items.Remove(this.draggedAppointment);
                            this.draggedAppointment.StartTime = app.StartTime;
                            this.draggedAppointment.EndTime = app.EndTime;
                            this.draggedAppointment.AllDay = true;
                            this.appointmentsLayoutItems.Items.Add(this.draggedAppointment);

                            if (this.draggedAppointment.Record != null)
                            {
                                var record = draggedAppointment.Record;
                                this.Model.View.EditItem(record.Data);
                                var pd = this.Model.View.GetPropertyAccessProvider();
                                this.Model.SetPropertiesOnNewItem(pd, record.Data, this.draggedAppointment);
                                this.Model.View.CommitEdit();
                            }
                        }
                        this.UpdateLayout();
                        sch.OnAppointmentDropped(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                    }
                }
            }
            this.AppointmentPopup.Children.Remove(DragPopUp);
            this.DragPopUp.IsOpen = false;
            this.isDragging = false;
            this.isDragged = false;
        }

        private void UpdateDraggedAppointment(ScheduleAppointment newapp)
        {
            var rootappointment = from app in this.Model.Appointments
                                  where (app.StartTime == this.draggedAppointment.StartTime && app.EndTime == this.draggedAppointment.EndTime)
                                  select app;
            if (rootappointment.Count() > 0)
            {
                var index = this.Model.Appointments.IndexOf(rootappointment.FirstOrDefault());
                this.Model.Appointments[index].StartTime = newapp.StartTime;
                this.Model.Appointments[index].EndTime = newapp.EndTime;
            }
        }

        private void DragDropMultiDay(DateTime newappdate)
        {

            if (this.draggedAppointment.MultiDayAppointmentStartTime > newappdate || this.draggedAppointment.MultiDayAppointmentEndTime < newappdate)
            {
                var actualDuration = this.draggedAppointment.MultiDayAppointmentEndTime - this.draggedAppointment.MultiDayAppointmentStartTime;
                var modifiedDuration = (newappdate.TimeOfDay + actualDuration);
#if SILVERLIGHT
                 var appCtl = from item in this.appointmentsLayoutItems.Items
                             where (((this.appointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleDaysAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentStartTime == this.draggedAppointment.MultiDayAppointmentStartTime) && ((this.appointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleDaysAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentEndTime == this.draggedAppointment.MultiDayAppointmentEndTime))
                             select item;
#endif
#if !SILVERLIGHT
                var appCtl = from item in this.appointmentsLayoutItems.Items.Cast <ScheduleDaysAppointmentViewControl>()
                             where (((this.appointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleDaysAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentStartTime == this.draggedAppointment.MultiDayAppointmentStartTime) && ((this.appointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleDaysAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentEndTime == this.draggedAppointment.MultiDayAppointmentEndTime))
                             select item;
#endif
                var rootappointment = (from app in this.Model.Appointments
                                       where (app.MultiDayAppointmentStartTime == this.draggedAppointment.MultiDayAppointmentStartTime && app.MultiDayAppointmentEndTime == this.draggedAppointment.MultiDayAppointmentEndTime)
                                       select app).FirstOrDefault();

                if (appCtl.Count() > 0)
                {
                    Schedule sch = GetScheduleParent();
                    sch.OnAppointmentDropping(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                    this.appointmentsLayoutItems.Items.Remove(appCtl.FirstOrDefault());
                    this.appointmentsLayoutItems.Items.Remove(appCtl.LastOrDefault());

                    this.draggedAppointment.StartTime = newappdate;
                    this.draggedAppointment.EndTime = newappdate + actualDuration;
                    if (rootappointment != null)
                    {
                        var index = this.Model.Appointments.IndexOf(rootappointment);
                        this.draggedAppointment.MultiDayAppointment = false;
                        this.draggedAppointment.CurrentAppointmentType = AppointmentType.Normal;
                        this.Model.Appointments[index].CurrentAppointmentType = AppointmentType.Normal;
                        this.Model.Appointments[index].MultiDayAppointment = false;
                        this.Model.Appointments[index].StartTime = this.draggedAppointment.StartTime;
                        this.Model.Appointments[index].EndTime = this.draggedAppointment.EndTime;
                        this.Model.Appointments[index].MultiDayAppointmentStartTime = this.draggedAppointment.StartTime;
                        this.Model.Appointments[index].MultiDayAppointmentEndTime = this.draggedAppointment.EndTime;
                        this.Model.Appointments[index].AppointmentProxy.Clear();

                    }
                    this.appointmentsLayoutItems.Items.Add(this.draggedAppointment);
                    sch.OnAppointmentDropped(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                    this.UpdateLayout();
                }
            }
        }

        private void PickUpAppointment(ScheduleDaysAppointmentViewControl el)
        {
            if (this.Model.AllowDragAndDrop)
            {
                this.draggedAppointment = el.DataContext as ScheduleAppointment;

                Schedule sch = GetScheduleParent();
                sch.OnAppoinmentDragging(new ScheduleAppointmentDragEventArgs(this.draggedAppointment));
                if (this.AppointmentPopup.Children.Count == 0)
                {
                    this.AppointmentPopup.Children.Add(DragPopUp);
                }

                ScheduleDaysAppointmentViewControl re = new ScheduleDaysAppointmentViewControl();
                re.DataContext = this.draggedAppointment;
                re.Height = this.Model.IntervalHeight;
                re.Width = this.ActualWidth / 7;
                re.Opacity = 0.5;
                DragPopUp.Child = re;
                this.isDragging = true;
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

        private void SetDefaultValuesForResize()
        {
            StartingPoint = new Point();
            actualAppHeight = 0;
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

        private void ClearAppointmentLayoutSelectedItems()
        {
           // this.Model.CurrentSelectedAppointment = null;
            foreach (var item in this.appointmentsLayoutItems.Items)
            {
                var viewControl = this.appointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleDaysAppointmentViewControl;
                if (viewControl != null && viewControl.IsSelected)
                {
                    viewControl.IsSelected = false;
                }
            }
        }

        private void ClearAllDayAppointmentLayoutSelectedItems()
        {
            foreach (var item in this.alldaysAppointmentsLayoutPanel.Items)
            {
                var viewControl = this.alldaysAppointmentsLayoutPanel.ItemContainerGenerator.ContainerFromItem(item) as ScheduleDaysAppointmentViewControl;
                if (viewControl != null && viewControl.IsSelected)
                {
                    viewControl.IsSelected = false;
                }
            }
        }

        #endregion

        #region Mouse Selection TimeSlots Implementation

        private void SetupTimeSlotEvents()
        {

            this.timeSlotItems.MouseRightClick += new MouseButtonEventHandler(timeSlotItems_MouseRightButtonDown); //newly added

            this.timeSlotItems.MouseLeftButtonDown += new MouseButtonEventHandler(timeSlotItems_MouseLeftButtonDown);
            this.timeSlotItems.MouseMove += new MouseEventHandler(timeSlotItems_MouseMove);
            this.timeSlotItems.MouseLeftButtonUp += new MouseButtonEventHandler(timeSlotItems_MouseLeftButtonUp);
            this.KeyDown += new KeyEventHandler(ScheduleDaysView_KeyDown);
            this.KeyUp += new KeyEventHandler(ScheduleDaysView_KeyUp);
        }



        //newly added
        void timeSlotItems_MouseRightButtonDown(object sender, MouseButtonEventArgs epnt)
        {
#if SILVERLIGHT
            ScheduleRectangleBorderExt e = VisualTreeHelper.FindElementsInHostCoordinates(epnt.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleRectangleBorderExt>().FirstOrDefault();
#else
            ScheduleRectangleBorderExt e = GetHitTestParentElement(this.InputHitTest(epnt.GetPosition(this)), "ScheduleRectangleBorderExt") as ScheduleRectangleBorderExt;
#endif
            if (e == null) return;

            if (!e.IsSelected)
            {
                suspend = true;
                this.ClearTimeSlotSelection();
                this.ClearDaysHeaderSelection();
                this.ClearAppointmentLayoutSelectedItems();
                this.ClearAllDayAppointmentLayoutSelectedItems();
                suspend = false;
                //isInMouseDown = true;
                e.IsSelected = true;
                mouseDownEl = e;
                //epnt.Handled = true;
                this.UpdateSelectedTimeSpan();
            }
        }


        private void ScheduleDaysView_KeyUp(object sender, KeyEventArgs e)
        {
#if SILVERLIGHT
            if (e.Key == Key.Ctrl)
#else
            if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
#endif
            {
                this.isCtrlKeyDown = false;
            }
        }

        private void ScheduleDaysView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
#if SILVERLIGHT
                foreach (ScheduleTimeSlotControl timeSlot in this.timeSlotItems.Items)
                {
                    foreach (var rect in timeSlot.FindElementsOfType<ScheduleRectangleBorderExt>().Where(r => r != null && r.IsSelected))
                    {
                        rect.IsSelected = false;
                    }
                }
#endif
                this.ClearAllDayAppointmentLayoutSelectedItems();
                this.ClearAppointmentLayoutSelectedItems();
                this.ClearTimeSlotSelection();
                this.ClearDaysHeaderSelection();
                this.Model.CurrentSelectedAppointment = null;
            }
#if SILVERLIGHT
            else if (e.Key == Key.Ctrl)
#else
            else if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
#endif
            {
                this.isCtrlKeyDown = true;
            }
        }

        private bool isInMouseDown = false;
        private bool suspend = false;

       
        ScheduleRectangleBorderExt mouseDownEl = null;

        private void timeSlotItems_MouseLeftButtonDown(object sender, MouseButtonEventArgs epnt)
        {
#if SILVERLIGHT
            ScheduleRectangleBorderExt e = VisualTreeHelper.FindElementsInHostCoordinates(epnt.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleRectangleBorderExt>().FirstOrDefault();
#else
            ScheduleRectangleBorderExt e = GetHitTestParentElement(this.InputHitTest(epnt.GetPosition(this)), "ScheduleRectangleBorderExt") as ScheduleRectangleBorderExt;
#endif
            if (e == null) return;

            suspend = true;
            this.ClearTimeSlotSelection();
            this.ClearDaysHeaderSelection();
            this.ClearAppointmentLayoutSelectedItems();
            this.ClearAllDayAppointmentLayoutSelectedItems();
            suspend = false;
            isInMouseDown = true;
            e.IsSelected = true;
            mouseDownEl = e;
        }

        private void timeSlotItems_MouseMove(object sender, MouseEventArgs epnt)
        {
            if (!isInMouseDown) return;

            if (IsDisableMouseMove)
            {
                IsDisableMouseMove = false;
                DisableMouseSelectionUpEvent();
                return;
            }

#if SILVERLIGHT
            ScheduleRectangleBorderExt el = VisualTreeHelper.FindElementsInHostCoordinates(epnt.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleRectangleBorderExt>().FirstOrDefault();
#else
            ScheduleRectangleBorderExt el = GetHitTestParentElement(this.InputHitTest(epnt.GetPosition(this)), "ScheduleRectangleBorderExt") as ScheduleRectangleBorderExt;
#endif
            if (suspend || el == null || mouseDownEl == null)
            {
                return;
            }
            this.ExtendSelection(el, mouseDownEl);
            //if (el != null && mouseDownEl != el && el != prevEl)
            //{
            //    if (prevEl != null)
            //    {
            //        this.ExtendSelection(el, prevEl);
            //    }
            //    el.IsSelected = !el.IsSelected;
            //    if (prevEl != null)
            //    {
            //        if (!el.IsSelected && prevEl.IsSelected)
            //        {
            //            prevEl.IsSelected = false;
            //        }
            //    }
            //    prevEl = el;
            //}
        }

        private void timeSlotItems_MouseLeftButtonUp(object sender, MouseButtonEventArgs epnt)
        {
#if !SILVERLIGHT
            var mouseUp = from evt in this.timeSlotItems.FindElementsOfType<ScheduleRectangleBorderExt>()
                          select this.InputHitTest(epnt.GetPosition(this));

            if (mouseUp.Count() <= 0) return;
#endif

            DisableMouseSelectionUpEvent();
#if !SILVERLIGHT
            if (this.isDragged)
            {
                DropAppointment(epnt);
            }
#endif
        }

        private bool IsDisableMouseMove = false;

        private void DisableMouseSelectionUpEvent()
        {
            isInMouseDown = false;
            mouseDownEl = null;
            this.UpdateSelectedTimeSpan();
        }

        private void ClearTimeSlotSelection()
        {
            if (this.timeSlotItems == null)
                return;
#if SILVERLIGHT
            var rs = this.timeSlotItems.Items.OfType<ScheduleTimeSlotControl>().Select(item => item.FindElementsOfType<ScheduleRectangleBorderExt>()).ToList();
            var elements = from rects in rs
                           from rect in rects.Where(r => r != null)
                           select rect;
            foreach (var rectangle in elements)
            {
                rectangle.IsSelected = false;
            }
#endif
        }

        private void UpdateSelectedTimeSpan()
        {
#if SILVERLIGHT
            // get hours
            // compute minnutes based on the model's time interval
            // generate timespan
            var rs = this.timeSlotItems.Items.OfType<ScheduleTimeSlotControl>().Select(item => item.FindElementsOfType<ScheduleRectangleBorderExt>()).ToList();
            var selectedRects = (from rects in rs
                                 from rect in rects.Where(r => r != null && r.IsSelected)
                                 select rect).ToList();
            if (selectedRects.Count == 0)
            {
                return;
            }
            var firstItem = selectedRects[0];
            var lastItem = selectedRects.LastOrDefault();
            // get the first parent
            var firstParentTimeSlot = firstItem.FindParentElementOfType<ScheduleTimeSlotControl>();
            var firstRectList = firstParentTimeSlot.FindElementsOfType<ScheduleRectangleBorderExt>().Where(r => r != null).ToList();
            // get the last parent
            var lastParentTimeSlot = lastItem.FindParentElementOfType<ScheduleTimeSlotControl>();
            var lastRectList = lastParentTimeSlot.FindElementsOfType<ScheduleRectangleBorderExt>().Where(r => r != null).ToList();
            int index = firstRectList.IndexOf(firstItem);
            int minutes = index * Convert.ToInt32((TimeIntervalToTimeSpanConverter(this.Model.CurrentTimeInterval).TotalMinutes));
            TimeSpan startTimeSpan = new TimeSpan(firstParentTimeSlot.Hour, minutes, 0);
            index = lastRectList.IndexOf(lastItem) + 1;
            minutes = index * Convert.ToInt32((TimeIntervalToTimeSpanConverter(this.Model.CurrentTimeInterval).TotalMinutes));
            TimeSpan endTimeSpan = new TimeSpan(lastParentTimeSlot.Hour, minutes, 0);
            this.Model.SelectedStartTimeSpan = firstParentTimeSlot.DateTime.AddTimeSpan(startTimeSpan);
            this.Model.SelectedEndTimeSpan = lastParentTimeSlot.DateTime.AddTimeSpan(endTimeSpan);
#endif
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

        private TimeSpan ComputeTimeSpan(ScheduleTimeSlotControl timeSlot)
        {
            var intervalCount = this.Model.GetTimeSlotIntervals();
            var rectList = timeSlot.FindElementsOfType<ScheduleRectangleBorderExt>().Where(r => r != null && r.IsSelected).ToList();
            var time = new TimeSpan(timeSlot.Hour, 0, 0);
            var mins = 60 / intervalCount;
            for (int i = 0; i < rectList.Count; i++)
            {
                time = time.Add(new TimeSpan(0, mins, 0));
            }
            return time;
        }

        private void ExtendSelection(ScheduleRectangleBorderExt el, ScheduleRectangleBorderExt prevEl)
        {

#if SILVERLIGHT
            var currentTimeSlot = el.FindParentElementOfType<ScheduleTimeSlotControl>();
            var usp = el.FindParentElementOfType<UniformStackPanel>();
            int index = 0, index1 = 0;
            if (usp != null)
                index = usp.Children.IndexOf(el);
            DateTime date = currentTimeSlot.DateTime.AddHours(currentTimeSlot.Hour);
            date = date.Add(new TimeSpan(0, this.TimeIntervalToTimeSpanConverter(this.Model.CurrentTimeInterval).Minutes * index, 0));
            var prevTimeSlot = prevEl.FindParentElementOfType<ScheduleTimeSlotControl>();
            var usp1 = prevEl.FindParentElementOfType<UniformStackPanel>();
            if (usp1 != null)
                index1 = usp1.Children.IndexOf(prevEl);
            DateTime date1 = prevTimeSlot.DateTime.AddHours(prevTimeSlot.Hour);
            date1 = date1.Add(new TimeSpan(0, this.TimeIntervalToTimeSpanConverter(this.Model.CurrentTimeInterval).Minutes * index1, 0));

            if (date > date1)
            {
                var pIdx = this.timeSlotItems.Items.IndexOf(prevTimeSlot);
                var cIdx = this.timeSlotItems.Items.IndexOf(currentTimeSlot);
                this.SetSelected(pIdx, index1, cIdx, index);
            }
            else if (date < date1)
            {
                var pIdx = this.timeSlotItems.Items.IndexOf(currentTimeSlot);
                var cIdx = this.timeSlotItems.Items.IndexOf(prevTimeSlot);
                this.SetSelected(pIdx, index, cIdx, index1);
            }
#endif
        }

        private void SetSelected(int pIdx, int pRectIdx, int cIdx, int cRectIdx)
        {
#if SILVERLIGHT
            //MessageBox.Show(pIdx.ToString() + "\n" + pRectIdx.ToString() + "\n" + cIdx.ToString() + "\n" + cRectIdx.ToString());
            ClearTimeSlotSelection();
            if (pIdx != cIdx)
            {
                var firstItem = (ScheduleTimeSlotControl)this.timeSlotItems.Items[pIdx];
                var rects = firstItem.FindElementsOfType<ScheduleRectangleBorderExt>().Where(r => r != null);
                for (int i = pRectIdx; i < rects.Count(); i++)
                {

                    rects.ElementAt(i).IsSelected = true;
                }
                for (int i = pIdx + 1; i < cIdx; i++)
                {

                    var item = (ScheduleTimeSlotControl)this.timeSlotItems.Items[i];
                    foreach (var rect in item.FindElementsOfType<ScheduleRectangleBorderExt>().Where(r => r != null))
                    {
                        rect.IsSelected = true;
                    }
                }
                var lastItem = (ScheduleTimeSlotControl)this.timeSlotItems.Items[cIdx];
                var rects1 = lastItem.FindElementsOfType<ScheduleRectangleBorderExt>().Where(r => r != null);
                for (int i = 0; i <= cRectIdx; i++)
                {

                    rects1.ElementAt(i).IsSelected = true;
                }
            }
            else
            {
                var firstItem = (ScheduleTimeSlotControl)this.timeSlotItems.Items[pIdx];
                var rects = firstItem.FindElementsOfType<ScheduleRectangleBorderExt>().Where(r => r != null);
                for (int i = pRectIdx; i <= cRectIdx; i++)
                {

                    rects.ElementAt(i).IsSelected = true;
                }
            }
#endif
        }

        #endregion

        #region Mouse Selection DaysHeader Implementation
        private bool isCtrlKeyDown = false;
        private void SetupDaysHeaderViewEvents()
        {
            this.daysHeaderViewItems.MouseLeftButtonDown += new MouseButtonEventHandler(daysHeaderViewItems_MouseLeftButtonDown);
            this.daysHeaderViewItems.MouseRightClick += new MouseButtonEventHandler(daysHeaderViewItems_MouseRightButtonDown);
#if !SILVERLIGHT
            this.daysHeaderViewItems.MouseLeftButtonUp += new MouseButtonEventHandler(daysHeaderViewItems_MouseLeftButtonUp);
#endif
        }
#if !SILVERLIGHT
        private void daysHeaderViewItems_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.DropAppointment(e);
        }
#endif


        //newly added
        private void daysHeaderViewItems_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

#if SILVERLIGHT
            ScheduleDaysHeaderViewControl el = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleDaysHeaderViewControl>().FirstOrDefault();
#else
            ScheduleDaysHeaderViewControl el = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleDaysHeaderViewControl") as ScheduleDaysHeaderViewControl;
#endif
            if (el == null)
            {
                return;
            }

            if (!this.isCtrlKeyDown)
            {
                this.ClearAllDayAppointmentLayoutSelectedItems();
                this.ClearAppointmentLayoutSelectedItems();
                this.ClearTimeSlotSelection();
                this.ClearDaysHeaderSelection();
            }

            el.IsSelected = !el.IsSelected;
            this.UpdateAllDaySelectedTimeSpan();


#if SILVERLIGHT
            var items = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<Rectangle>();
            if (items.Count() <= 0 || items == null)
            {
                this.Model.GoToSelectedStartTime(this.Model.SelectedStartTimeSpan);
            }
            else if (items.FirstOrDefault().Name == "PART_CurrentRectangle")
            {
                this.Model.GoToSelectedStartTime(this.Model.SelectedStartTimeSpan);
            }
#else
            Rectangle items = this.InputHitTest(e.GetPosition(this)) as Rectangle;
            if (items != null && items.Name == "PART_CurrentRectangle")
            {
                this.Model.GoToSelectedStartTime(this.Model.SelectedStartTimeSpan);
            }
            else if (items == null)
            {
                this.Model.GoToSelectedStartTime(this.Model.SelectedStartTimeSpan);
            }
#endif
        }


        private void daysHeaderViewItems_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            ScheduleDaysHeaderViewControl el = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleDaysHeaderViewControl>().FirstOrDefault();
#else
            ScheduleDaysHeaderViewControl el = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleDaysHeaderViewControl") as ScheduleDaysHeaderViewControl;
#endif
            if (el == null)
            {
                return;
            }

            if (!this.isCtrlKeyDown)
            {
                this.ClearAllDayAppointmentLayoutSelectedItems();
                this.ClearAppointmentLayoutSelectedItems();
                this.ClearTimeSlotSelection();
                this.ClearDaysHeaderSelection();
            }

            el.IsSelected = !el.IsSelected;
            this.UpdateAllDaySelectedTimeSpan();


#if SILVERLIGHT
            var items = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<Rectangle>();
            if (items.Count() <= 0 || items == null)
            {
                this.Model.GoToSelectedStartTime(this.Model.SelectedStartTimeSpan);
            }
            else if (items.FirstOrDefault().Name == "PART_CurrentRectangle")
            {
                this.Model.GoToSelectedStartTime(this.Model.SelectedStartTimeSpan);
            }
#else
            Rectangle items = this.InputHitTest(e.GetPosition(this)) as Rectangle;
            if (items != null && items.Name == "PART_CurrentRectangle")
            {
                this.Model.GoToSelectedStartTime(this.Model.SelectedStartTimeSpan);
            }
            else if (items == null)
            {
                this.Model.GoToSelectedStartTime(this.Model.SelectedStartTimeSpan);
            }
#endif

        }

        private void UpdateAllDaySelectedTimeSpan()
        {
            var items = this.daysHeaderViewItems.Items.OfType<ScheduleDaysHeaderViewControl>().Where(e => e.IsSelected).ToList();
            if (items.Count > 0)
            {
                var firstItem = items[0];
                var lastItem = items[items.Count - 1];
                if (firstItem != lastItem)
                {
                    var currentDate = firstItem.DateTime;
                    var endDate = lastItem.DateTime;
                    this.Model.SelectedStartTimeSpan = currentDate;
                    this.Model.SelectedEndTimeSpan = endDate;
                }
                else
                {
                    var el = firstItem;
                    var currentDate = el.DateTime;
                    this.Model.SelectedStartTimeSpan = currentDate;
                    var endDay = currentDate.AddHours(24);
                    this.Model.SelectedEndTimeSpan = endDay;
                }
            }
        }

        private void ClearDaysHeaderSelection()
        {
            foreach (var headerView in this.daysHeaderViewItems.Items.OfType<ScheduleDaysHeaderViewControl>().Where(el => el.IsSelected))
            {
                headerView.IsSelected = false;
            }
        }

        #endregion

        #region Appointments population
        internal void SetupAppointments()
        {
            if (this.Model == null || this.appointmentsLayoutItems == null || this.Model.Appointments == null)
            {
                return;
            }

            this.Model.Appointments.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnAppointmentsCollectionChanged);
            this.ClearAppointments();
            foreach (var appInfo in this.Model.GetCurrentAppointments())
            {
                if (appInfo != null)
                {
                    if (!appInfo.IsSpanned)
                    {
                        this.appointmentsLayoutItems.Items.Add(appInfo.Appointment);
                    }
                    else
                    {
                        this.alldaysAppointmentsLayoutPanel.Items.Add(appInfo.Appointment);
                    }
                }
            }
            this.UpdateLayout();
            this.Model.RaiseAppointmentDatesBold();
            this.Model.Appointments.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnAppointmentsCollectionChanged);
        }

        private void ClearAppointments()
        {
            this.appointmentsLayoutItems.Items.Clear();
#if SILVERLIGHT
            this.Dispatcher.BeginInvoke(() =>
#else
            this.Dispatcher.BeginInvoke(new Action(() =>
#endif
    {
        var appointmentsLayoutPanel = this.appointmentsLayoutItems.FindElementOfType<ScheduleDaysAppointmentLayoutPanel>();
        if (appointmentsLayoutPanel != null)
        {
            appointmentsLayoutPanel.Reset();
        }
#if SILVERLIGHT
    });
#else
    }));
#endif
            this.alldaysAppointmentsLayoutPanel.Items.Clear();
        }

        private void OnAppointmentsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            if (this.Model == null || this.appointmentsLayoutItems == null)
            {
                return;
            }

            var isRecurrenceApp = false;
            var isMultiDayApp = false;
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
                        else if(app.CurrentAppointmentType==AppointmentType.MultiDay)
                        {
                          isMultiDayApp = true;
                          continue;
                        }
                        if (!app.IsAllDayOrSpanned())
                        {
                            this.appointmentsLayoutItems.Items.Add(app);

                        }
                        else
                        {
                            this.alldaysAppointmentsLayoutPanel.Items.Add(app);
                        }

                    }

                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (ScheduleAppointment app in e.OldItems)
                    {
                        if (!app.IsAllDayOrSpanned())
                        {
                            var appointmentView = this.appointmentsLayoutItems.Items.OfType<ScheduleAppointment>().FirstOrDefault(c => c == app);
                            if (appointmentView != null)
                            {
                                this.appointmentsLayoutItems.Items.Remove(appointmentView);
                            }
                        }
                        else
                        {
                            var appList = this.alldaysAppointmentsLayoutPanel.Items.OfType<ScheduleAppointment>().ToList();
                            var appointmentView = appList.FirstOrDefault(c => c == app);
                            if (appointmentView != null)
                            {
                                this.alldaysAppointmentsLayoutPanel.Items.Remove(appointmentView);
                            }
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    this.ClearAppointments();
                    break;
            }
            if (isRecurrenceApp)
            {
                this.SetupAppointments();
                isRecurrenceApp = false;
            }
            if (isMultiDayApp)
            {
                this.SetupAppointments();
                isMultiDayApp = false;
            }
        }      

        #endregion

        #region Appointment Editor

        private void SetupDoubleClick()
        {
            this.timeSlotItems.MouseDoubleClick += new MouseButtonEventHandler(timeSlotItems_MouseDoubleClick);
            this.daysHeaderViewItems.MouseDoubleClick += new MouseButtonEventHandler(daysHeaderViewItems_MouseDoubleClick);
            this.appointmentsLayoutItems.MouseDoubleClick += new MouseButtonEventHandler(appointmentsLayoutItems_MouseDoubleClick);
            this.alldaysAppointmentsLayoutPanel.MouseDoubleClick += new MouseButtonEventHandler(alldaysAppointmentsLayoutPanel_MouseDoubleClick);
        }

        private void daysHeaderViewItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            var items = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<Rectangle>();
            if (items.Count() <= 0 || items == null) return;
            if (items.FirstOrDefault().Name != "PART_SelectedRectangle" && items.FirstOrDefault().Name != "PART_HeaderRectangle") return;
            if (items.FirstOrDefault().Name == "PART_CurrentRectangle") return;

            var startTimeSpan = this.Model.SelectedStartTimeSpan;
            var endTimeSpan = this.Model.SelectedStartTimeSpan;
            if (startTimeSpan == DateTime.MinValue || endTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }
            IsDisableMouseMove = true;
            var appointment = new ScheduleAppointment() { StartTime = startTimeSpan, EndTime = endTimeSpan, AllDay = true };
            this.ShowWindow(appointment, false);
#else
            Rectangle items = this.InputHitTest(e.GetPosition(this)) as Rectangle;
            if (items != null && items.Name == "PART_CurrentRectangle") return;
            if (items == null) items = GetHitTestParentElement(items, "PART_SelectedRectangle") as Rectangle;
            if (items == null) return;
            var startTimeSpan = this.Model.SelectedStartTimeSpan;
            var endTimeSpan = this.Model.SelectedStartTimeSpan;
            if (startTimeSpan == DateTime.MinValue || endTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }
            IsDisableMouseMove = true;

            var appointment = this.Model.CreateNewAppointment(startTimeSpan, endTimeSpan, true);
            this.ShowWindow(appointment, false);
#endif
        }

        private void alldaysAppointmentsLayoutPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!this.Model.AllowEdit)
            {
                return;
            }

#if SILVERLIGHT
            var items = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleDaysAppointmentViewControl>().ToList();
            if (items.Count > 0)
            {
                if (this.Model != null)
                {
                    ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(items[0].ScheduleAppointment);
                    this.Model.GetAppointmentDoubleClickEvents(args);
                }
                this.Model.CurrentSelectedAppointment = items[0].ScheduleAppointment;
                this.ShowWindow(this.Model.CurrentSelectedAppointment, true);
            }
#else
            var items = this.InputHitTest(e.GetPosition(this));
            ScheduleDaysAppointmentViewControl itemevt = GetHitTestParentElement(items, "ScheduleDaysAppointmentViewControl") as ScheduleDaysAppointmentViewControl;
            if (itemevt != null && itemevt.ScheduleAppointment != null)
            {
                if (this.Model != null)
                {
                    ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(itemevt.ScheduleAppointment);
                    this.Model.GetAppointmentDoubleClickEvents(args);
                }
                this.Model.CurrentSelectedAppointment = itemevt.ScheduleAppointment;
                this.ShowWindow(this.Model.CurrentSelectedAppointment, true);
            }
#endif
        }

        private void appointmentsLayoutItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            if (!this.Model.AllowEdit)
            {
                return;
            }

#if SILVERLIGHT
            var showEditorWindow = false;
            var items = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleDaysAppointmentViewControl>().ToList();
            if (items.Count > 0)
            {
                if (this.Model != null)
                {
                    ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(items[0].ScheduleAppointment);
                    this.Model.GetAppointmentDoubleClickEvents(args);
                    showEditorWindow = !args.Cancel;
                }
                if (showEditorWindow)
                {
                    this.Model.CurrentSelectedAppointment = items[0].ScheduleAppointment;
                    if (items[0].ScheduleAppointment.MultiDayAppointment)
                    {
                        this.Model.CurrentSelectedAppointment.StartTime = items[0].ScheduleAppointment.MultiDayAppointmentStartTime;
                        this.Model.CurrentSelectedAppointment.EndTime = items[0].ScheduleAppointment.MultiDayAppointmentEndTime;
                    }
                    else
                    {
                        this.Model.CurrentSelectedAppointment.StartTime = items[0].ScheduleAppointment.StartTime;
                        this.Model.CurrentSelectedAppointment.EndTime = items[0].ScheduleAppointment.EndTime;
                    }
                    this.ShowWindow(this.Model.CurrentSelectedAppointment, true);
                }
                else
                {
                    e.Handled = true;
                }
            }
#else
            var items = this.InputHitTest(e.GetPosition(this));

            ScheduleDaysAppointmentViewControl itemevt = GetHitTestParentElement(items, "ScheduleDaysAppointmentViewControl") as ScheduleDaysAppointmentViewControl;
            if (itemevt != null && itemevt.ScheduleAppointment != null)
            {
                if (this.Model != null)
                {
                    ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(itemevt.ScheduleAppointment);
                    this.Model.GetAppointmentDoubleClickEvents(args);
                }
                this.Model.CurrentSelectedAppointment = itemevt.ScheduleAppointment;
                this.ShowWindow(this.Model.CurrentSelectedAppointment, true);
            }
            e.Handled = true;
#endif
        }

        private void timeSlotItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var startTimeSpan = this.Model.SelectedStartTimeSpan;
            var endTimeSpan = this.Model.SelectedEndTimeSpan;
            if (startTimeSpan == DateTime.MinValue || endTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }
            IsDisableMouseMove = true;
            var appointment = this.Model.CreateNewAppointment(startTimeSpan, endTimeSpan, false);
            this.ShowWindow(appointment, false);
#if !SILVERLIGHT
            e.Handled = true;
#endif
        }
       
        private void ShowWindow(ScheduleAppointment appointment, bool isEditing)
        {
            IsRecurrenceModified = false;
            if (this.Model != null)
            {
                ScheduleAppointmentCancelEventArgs args = new ScheduleAppointmentCancelEventArgs(appointment);
                this.Model.GetAppointmentWindowOpeningEvents(args);
                if (args.Cancel == true && args != null) return;
            }
            var appWrapper = new ScheduleAppointmentWrapper(appointment, this.Model, isEditing) { AppointmentStatusCollection = Model.AppointmentStatusCollection };
            if (appointment.IsRecurrenceAppointment == true)
            {
                var childwindow = this.GetChildWindow(250);
#if !SILVERLIGHT
                childwindow.Height = 170;
                childwindow.ResizeMode = ResizeMode.NoResize;
                childwindow.WindowStartupLocation = WindowStartupLocation.Manual;
                Schedule s = this.GetScheduleParent();
                Point schedulePoint = s.PointToScreen(new Point());
                childwindow.Left = schedulePoint.X + (s.ActualWidth - childwindow.Width) / 2;
                childwindow.Top = schedulePoint.Y + (s.ActualHeight - childwindow.Height) / 2;  
#endif
                ResourceWrapper rw = new ResourceWrapper();
                childwindow.Title = rw.RecurrenceAlertWindowHeader;
                if (this.AppointmentScheduleWindowStyle != null)
                {
                    childwindow.Style = this.AppointmentScheduleWindowStyle;
                }
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
#if !SILVERLIGHT
                    childwindow.ShowDialog();
#else
                childwindow.Show();
#endif
            }

            else
            {
                ShowWindow(appWrapper, Visibility.Collapsed);
            }

            if (this.Model != null)
            {
                ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(appointment);
                this.Model.GetAppointmentWindowOpenedEvents(args);
            }
        }

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
#if SILVERLIGHT
            var window = this.GetChildWindow();
            if (this.AppointmentScheduleWindowStyle != null)
            {
                window.Style = this.AppointmentScheduleWindowStyle;
            }
#else
            var window = this.GetChildWindow();
            window.Owner = this.GetTopParent();
            window.WindowStartupLocation = WindowStartupLocation.Manual;
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
			window.HorizontalAlignment = HorizontalAlignment.Center;
            window.VerticalAlignment = VerticalAlignment.Center;
            
#else
            window.SetBinding(ChildWindow.TitleProperty, bindTitle);
#endif
            window.Content = editorControl;
            window.Show();
        }
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

        private bool IsRecurrenceModified = false;

        private void editorControl_DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            IsRecurrenceModified = false;
            buttonclicked = true;
            var editor = sender as ScheduleAppointmentEditorControl;
            editor.DeleteButtonClick -= new RoutedEventHandler(editorControl_DeleteButtonClick);
#if !SILVERLIGHT
            var childWindow = editor.FindParentElementOfType<Window>();
#else
            var childWindow = editor.FindParentElementOfType<ChildWindow>();
#endif
            if (childWindow != null)
            {
                ScheduleAppointmentCancelEventArgs arg = new ScheduleAppointmentCancelEventArgs(this.Model.CurrentSelectedAppointment);
                this.Model.GetAppointmentWindowClosingEvents(arg);
                if (arg.Cancel == true && arg != null) { }
                else
                {
                    this.Model.DeleteCurrentSelectedAppointment();
                }
                childWindow.Close();
                //ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(this.Model.CurrentSelectedAppointment);
                //this.Model.GetAppointmentWindowClosedEvents(args);
            }
        }

        private void editorControl_SaveButtonClick(object sender, RoutedEventArgs e)
        {
            IsRecurrenceModified = false;
            buttonclicked = true;
            var editor = sender as ScheduleAppointmentEditorControl;
            //editor.IsStateChanged = false;
            editor.SaveButtonClick -= new RoutedEventHandler(editorControl_SaveButtonClick);
#if !SILVERLIGHT
            var childWindow = editor.FindParentElementOfType<Window>();
#else
            var childWindow = editor.FindParentElementOfType<ChildWindow>();
#endif
            if (childWindow != null)
            {
                ScheduleAppointmentCancelEventArgs arg = new ScheduleAppointmentCancelEventArgs(this.Model.CurrentSelectedAppointment);
                this.Model.GetAppointmentWindowClosingEvents(arg);
                if (arg.Cancel == true && arg != null) { }
                else
                {
                    SaveAppointments(editor);
                }
                childWindow.Close();
                //ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(this.Model.CurrentSelectedAppointment);
                //this.Model.GetAppointmentWindowClosedEvents(args);
            }
        }
        bool buttonclicked = false;

        private void SaveAppointments(ScheduleAppointmentEditorControl editor)
        {
            var appWrapper = editor.DataContext as ScheduleAppointmentWrapper;
            var appProxy = appWrapper.Appointment;
            var newAppForRec = new ScheduleAppointment();
            newAppForRec.InitializeFrom(appProxy);
            newAppForRec.CurrentRecurrencePatternMode = editor.RecurrencePattern;
            if (editor.IsStateChanged == true)
            {
                appWrapper.IsAppointmentModified = true;
            }
            editor.IsStateChanged = false;
            if (editor.IsRecurrenceOccured == true)
            {
                newAppForRec.IsRecurrenceAppointment = true;
            }
            else
            {
                newAppForRec.IsRecurrenceAppointment = false;
            }
            if (this.Model.CurrentSelectedAppointment != null)
            {
                if (appWrapper.IsAppointmentModified)
                {
                    if (this.Model.IsDataBound)
                    {
                        // sync record
                        var record = this.Model.CurrentSelectedAppointment.Record;
                        newAppForRec.Record = record;
                    }
                    if (appWrapper.Appointment.IsAppointmentProxyCollection == true)
                    {
                        newAppForRec.IsAppointmentProxyCollection = true;
                        this.Model.RemoveCurrentAppointmentProxy(this.Model.CurrentSelectedAppointment);
                        newAppForRec.CurrentAppointmentType = AppointmentType.RecurrenceProxy;
                        this.Model.CurrentSelectedAppointment.AppointmentProxy.Add(newAppForRec);
                    }
                    else
                    {
                        // edit appointment
                        if (this.Model.CurrentSelectedAppointment.IsRecurrenceAppointment == true)
                        {
                            this.Model.CurrentSelectedAppointment.StartTime = this.Model.CurrentSelectedAppointment.StartRecurrenceTime;
                            this.Model.CurrentSelectedAppointment.EndTime = this.Model.CurrentSelectedAppointment.EndRecurrenceTime;
                        }

                        if (this.Model.RemoveCurrentAppointment(this.Model.CurrentSelectedAppointment))
                            this.Model.Appointments.Add(newAppForRec);
                    }
                }
            }
            else
            {
                // add new appointment
                this.Model.Appointments.Add(newAppForRec);
            }

            if (this.Model.CurrentSelectedAppointment != null || editor.IsRecurrenceOccured == true)
                this.SetupAppointments();
            this.Model.CurrentSelectedAppointment = null;
        }

        private void editorControl_RecurrenceButtonClick(object sender, RoutedEventArgs e)
        {
            var editor = sender as ScheduleAppointmentEditorControl;
            editor.RecurrenceButtonClick -= new RoutedEventHandler(editorControl_RecurrenceButtonClick);

            var appWrapper = editor.DataContext as ScheduleAppointmentWrapper;
            IsRecurrenceModified = !IsRecurrenceModified;
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
#if !SILVERLIGHT
        private Window GetTopParent()
        {
            DependencyObject dpParent = this.FindParentElementOfType<Window>();
            return dpParent as Window;
        }
#endif

#if !SILVERLIGHT
        /// <summary>
        /// This virtual  method is called to return the size of the child window
        /// </summary>
        protected virtual Window GetChildWindow()
        {
            var window = new Window();
            window.Width = 780;
            window.Height = 570;
            return window;
        }
        /// <summary>
        /// This virtual  method is called to return the specified size of the child window
        /// </summary>
        protected virtual Window GetChildWindow(double width)
        {
            var window = new Window();
            window.Width = width;
            return window;
        }
#else
        protected virtual ChildWindow GetChildWindow()
        {
            var window = new ChildWindow();
            window.Width = 800;
            window.Height = 570;
            return window;
        }

        protected virtual ChildWindow GetChildWindow(double width)
        {
            var window = new ChildWindow();
            window.Width = width;
            return window;
        }
#endif

        private void window_Closed(object sender, EventArgs e)
        {
            if (this.Model != null && !buttonclicked)
            {
                ScheduleAppointmentCancelEventArgs arg = new ScheduleAppointmentCancelEventArgs(this.Model.CurrentSelectedAppointment);
                this.Model.GetAppointmentWindowClosingEvents(arg);
                if (arg.Cancel == true && arg != null) return;
            }
#if SILVERLIGHT
            var window = sender as ChildWindow;
#else
            var window = sender as Window;
#endif
            window.Closed -= new EventHandler(window_Closed);

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
#if !SILVERLIGHT
            window.Close();
#endif
            IsRecurrenceModified = false;

            if (this.Model != null)
            {
                ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(new ScheduleAppointment());
                this.Model.GetAppointmentWindowClosedEvents(args);
            }
        }

        #endregion

        private void UpdateTimeSlotScrollPosition()
        {
            if (this.timeSlotScrollViewer == null || this.Model == null)
            {
                return;
            }

#if SILVERLIGHT
            this.Dispatcher.BeginInvoke(() =>
             {
                 var maxHeight = 0d;
                 for (int i = 0; i < this.timeSlotItems.Items.Count; i++)
                 {
                     var timeSlot = (ScheduleTimeSlotControl)this.timeSlotItems.Items[i];
                     if (timeSlot.Hour == this.Model.StartWorkHour)
                     {
                         break;
                     }
                     maxHeight += !double.IsInfinity(timeSlot.DesiredSize.Height) ? timeSlot.DesiredSize.Height : timeSlot.ActualHeight;
                 }
                 this.timeSlotScrollViewer.ScrollToVerticalOffset(maxHeight);
             });
#else
            this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var maxHeight = 0d;
                    maxHeight = this.model.GetCurrentIntervalPerHour() * this.model.StartWorkHour * (this.model.IntervalHeight -1);
                this.timeSlotScrollViewer.ScrollToVerticalOffset(maxHeight);
                 }));
#endif

        }

        #region IDisposable Members

        /// <summary>
        /// Releases all resources used by the. <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleDaysView"/>.
        /// </summary>
        public void Dispose()
        {
            if (isTemplateApplied == false) return;
            this.AppointmentPopup.Children.Clear();
            this.timeLineItems.Items.Clear();
            this.daysHeaderViewItems.Items.Clear();
#if SILVERLIGHT
            this.timeSlotItems.Items.Clear();
#endif
            this.appointmentsLayoutItems.Items.Clear();
            this.alldaysAppointmentsLayoutPanel.Items.Clear();
            this.appointmentsLayoutItems.MouseLeftButtonDown -= new MouseButtonEventHandler(appointmentsLayoutItems_MouseLeftButtonDown);
            this.appointmentsLayoutItems.MouseMove -= new MouseEventHandler(appointmentsLayoutItems_MouseMove);
            this.appointmentsLayoutItems.MouseLeftButtonUp -= new MouseButtonEventHandler(appointmentsLayoutItems_MouseLeftButtonUp);
            this.alldaysAppointmentsLayoutPanel.MouseLeftButtonDown -= new MouseButtonEventHandler(alldaysAppointmentsLayoutPanel_MouseLeftButtonDown);
#if SILVERLIGHT
            this.alldaysAppointmentsLayoutPanel.MouseLeftButtonUp -= new MouseButtonEventHandler(alldaysAppointmentsLayoutPanel_MouseLeftButtonUp);
#endif
            this.MouseMove -= new MouseEventHandler(ScheduleDaysView_MouseMove);
#if !SILVERLIGHT
            this.DragPopUp.MouseLeftButtonUp -= new MouseButtonEventHandler(appointmentsLayoutItems_MouseLeftButtonUp);
            this.DragPopUp.MouseMove -= new MouseEventHandler(ScheduleDaysView_MouseMove); 
#endif
            this.model.PropertyChanged -= new PropertyChangedEventHandler(model_PropertyChanged);
            this.timeSlotItems.MouseLeftButtonDown -= new MouseButtonEventHandler(timeSlotItems_MouseLeftButtonDown);
            this.timeSlotItems.MouseMove -= new MouseEventHandler(timeSlotItems_MouseMove);
            this.timeSlotItems.MouseLeftButtonUp -= new MouseButtonEventHandler(timeSlotItems_MouseLeftButtonUp);
            this.KeyDown -= new KeyEventHandler(ScheduleDaysView_KeyDown);
            this.KeyUp -= new KeyEventHandler(ScheduleDaysView_KeyUp);
            this.daysHeaderViewItems.MouseLeftButtonDown -= new MouseButtonEventHandler(daysHeaderViewItems_MouseLeftButtonDown);
#if !SILVERLIGHT
            this.daysHeaderViewItems.MouseLeftButtonUp -= new MouseButtonEventHandler(daysHeaderViewItems_MouseLeftButtonUp);
#endif

            this.Model.Appointments.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnAppointmentsCollectionChanged);
            this.timeSlotItems.MouseDoubleClick -= new MouseButtonEventHandler(timeSlotItems_MouseDoubleClick);
            this.appointmentsLayoutItems.MouseDoubleClick -= new MouseButtonEventHandler(appointmentsLayoutItems_MouseDoubleClick);
            this.alldaysAppointmentsLayoutPanel.MouseDoubleClick -= new MouseButtonEventHandler(alldaysAppointmentsLayoutPanel_MouseDoubleClick);

#if (SyncfusionFramework4_0) || !(SyncfusionFramework3_5 && SILVERLIGHT)
            this.appointmentsLayoutItems.MouseRightButtonDown -= new MouseButtonEventHandler(appointmentsLayoutItems_MouseRightButtonDown);
            this.timeSlotItems.MouseRightButtonDown -= new MouseButtonEventHandler(timeSlotItems_MouseRightButtonDown);
#endif
        }

        #endregion
    }



    /// <summary>
    ///  class to hold collection of objects
    /// </summary>
    public class ItemsCollection : ObservableCollection<object>
    {
    }


}
