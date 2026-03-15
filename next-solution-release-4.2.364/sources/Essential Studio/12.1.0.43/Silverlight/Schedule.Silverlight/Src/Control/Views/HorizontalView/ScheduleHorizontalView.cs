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
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.ComponentModel;
    using System.Windows.Data;
    using System.Windows.Controls.Primitives;
    using System.Collections.ObjectModel;
#if SILVERLIGHT
    using Syncfusion.Windows.Shared;
#endif

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  Class that holds schedule HorizontalView
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
#if !SILVERLIGHT
    [StyleTypedProperty(Property = "AppointmentScheduleWindowStyle", StyleTargetType = typeof(Window))]
#else
    /// <summary>
    /// Represents Schedule's Horizontal View.
    /// </summary>
    [StyleTypedProperty(Property = "AppointmentScheduleWindowStyle", StyleTargetType = typeof(ChildWindow))]
#endif
    [StyleTypedProperty(Property = "RecurrenceAlertWindowStyle", StyleTargetType = typeof(ScheduleRecurrenceConfirmationWindow))]
    [StyleTypedProperty(Property = "AppointmentEditorStyle", StyleTargetType = typeof(ScheduleAppointmentEditorControl))]
    [StyleTypedProperty(Property = "AppointmentStyle", StyleTargetType = typeof(ScheduleHorizontalAppointmentViewControl))]
    public class ScheduleHorizontalView : Control, IScheduleCalendarViewModelHost, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleHorizontalView"/> class.
        /// </summary>
        public ScheduleHorizontalView()
        {
            this.DefaultStyleKey = typeof(ScheduleHorizontalView);
            resourceDictionary = new ResourceDictionary();
#if SILVERLIGHT
            resourceDictionary.Source = new Uri("/Syncfusion.Schedule.Silverlight;component/Control/Themes/Generic.xaml",
                               UriKind.RelativeOrAbsolute);
#else
            resourceDictionary.Source = new Uri("/Syncfusion.Schedule.Wpf;component/Control/Themes/Generic.xaml",
                               UriKind.RelativeOrAbsolute);
#endif
        }

        private ResourceDictionary resourceDictionary;
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

        #region Dependency property

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
            DependencyProperty.Register("AppointmentScheduleWindowStyle", typeof(Style), typeof(ScheduleHorizontalView),
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
            DependencyProperty.Register("RecurrenceAlertWindowStyle", typeof(Style), typeof(ScheduleHorizontalView),
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
            DependencyProperty.Register("AppointmentEditorStyle", typeof(Style), typeof(ScheduleHorizontalView),
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
            DependencyProperty.Register("AppointmentStyle", typeof(Style), typeof(ScheduleHorizontalView),
              new PropertyMetadata(null, OnAppointmentStyleChanged));

        private bool isAppointStyleChangedBeforeLoaded = false;
        private static void OnAppointmentStyleChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var horizontalView = dpo as ScheduleHorizontalView;
            if (horizontalView.isTemplateApplied)
            {
                horizontalView.horizontalAppointmentsLayoutItems.AppointmentStyle = (Style)args.NewValue;
            }
            else
            {
                horizontalView.isAppointStyleChangedBeforeLoaded = true;
            }
        }

        #endregion


        #region TimelineVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ScheduleTimelineVisibility.
        /// </summary>


        public static readonly DependencyProperty TimelineVisibilityProperty = DependencyProperty.Register("TimelineVisibility", typeof(Visibility), typeof(ScheduleHorizontalView), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets value to determine whether the timeLine should visible or not
        /// </summary>
        public Visibility TimelineVisibility
        {
            get { return (Visibility)GetValue(TimelineVisibilityProperty); }
            set { SetValue(TimelineVisibilityProperty, value); }
        }
        #endregion


        #endregion

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

        private bool isTemplateApplied = false;
        private ScheduleHorizontalTimeLineItemsControl horizontaltimeLineItems;
        private ScheduleHorizontalDaysHeaderViewItemsControl horizontaldaysHeaderViewItems;
        private ScheduleHorizontalTimeSlotItemsControl horizontaltimeSlotItems;
        private ScheduleHorizontalAppointmentLayoutItemsControl horizontalAppointmentsLayoutItems;
        private ScrollViewer timeSlotScrollViewer;
        private Grid AppointmentPopup;

#if SILVERLIGHT
        private ContextMenuAdv ContextMenuHorizontalTimeSlotItems;
        private ContextMenuAdv ContextMenuHorizontalAppointmentItems;
        private ContextMenuAdv ContextMenuHorizontalTimeLineItems;
#else
        private ContextMenu ContextMenuHorizontalTimeSlotItems;
        private ContextMenu ContextMenuHorizontalAppointmentItems;
        private ContextMenu ContextMenuHorizontalTimeLineItems;
#endif     
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
        private ContextMenuCommand showAsCommand;
        private ContextMenuCommand colorButton1;
        private ObservableCollection<object> DefaultContextMenuHorizontalTimeSlotItems = new ObservableCollection<object>();
        private ObservableCollection<object> DefaultContextMenuHorizontalAppointmentItems = new ObservableCollection<object>();
        private ObservableCollection<object> DefaultContextMenuHorizontalTimeLineItems = new ObservableCollection<object>();

        private void UpdateTimeSlotScrollPosition()
        {
            if (this.timeSlotScrollViewer == null || this.Model == null)
            {
                return;
            }

#if SILVERLIGHT
            this.Dispatcher.BeginInvoke(() =>

            {
                var maxWidth = 0d;
                for (int i = 0; i < this.horizontaltimeSlotItems.Items.Count; i++)
                {
                    var timeSlot = (ScheduleHorizontalTimeSlotControl)this.horizontaltimeSlotItems.Items[i];
                    if (timeSlot.Hour == this.Model.StartWorkHour)
                    {
                        break;
                    }
                    maxWidth += !double.IsInfinity(timeSlot.DesiredSize.Width) ? timeSlot.DesiredSize.Width : timeSlot.ActualWidth;
                }
                this.timeSlotScrollViewer.ScrollToHorizontalOffset(maxWidth);

            });
#else
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var maxHeight = 0d;
                maxHeight = this.model.GetCurrentIntervalPerHour() * this.model.StartWorkHour * (this.model.IntervalHeight - 1);
                this.timeSlotScrollViewer.ScrollToVerticalOffset(maxHeight);
            }));
#endif
        }

        private void UpdateModelsToInnerControls()
        {
            if (!this.isTemplateApplied || this.model == null)
            {
                return;
            }
            this.horizontalAppointmentsLayoutItems.SetCalendarViewModel(this.model);
            this.horizontaltimeLineItems.SetCalendarViewModel(this.model);
            this.horizontaldaysHeaderViewItems.SetCalendarViewModel(this.model);
            this.horizontaltimeSlotItems.SetCalendarViewModel(this.model);
            this.UpdateLayout();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.DragPopUp = new Popup();
            this.AppointmentPopup = this.GetTemplateChild("PART_HorizontalAppointmentPopup") as Grid;
            this.horizontaltimeLineItems = this.GetTemplateChild("PART_HorizontalTimeLineItemsControl") as ScheduleHorizontalTimeLineItemsControl;
            this.horizontaldaysHeaderViewItems = this.GetTemplateChild("PART_HorizontalDaysHeaderControl") as ScheduleHorizontalDaysHeaderViewItemsControl;
            this.horizontaltimeSlotItems = this.GetTemplateChild("PART_HorizontalTimeSlot") as ScheduleHorizontalTimeSlotItemsControl;
            this.horizontalAppointmentsLayoutItems = this.GetTemplateChild("PART_HorizontalAppointmentsLayout") as ScheduleHorizontalAppointmentLayoutItemsControl;
            this.timeSlotScrollViewer = this.GetTemplateChild("PART_HorizontalTimeSlotScrollViewer") as ScrollViewer;
            this.isTemplateApplied = true;
            this.UpdateModelsToInnerControls();
            this.SetupTimeSlotEvents();
            this.SetupAppointments();
            this.SetupAppointmentEvents();
            this.SetupDoubleClick();
            this.UpdateTimeSlotScrollPosition();
            this.EnsureProperties();
            DefaultContextMenuHorizontalTimeSlotItems = resourceDictionary["DefaultContextMenuHorizontalTimeSlotItems"] as ObservableCollection<object>;
            DefaultContextMenuHorizontalTimeLineItems = resourceDictionary["DefaultContextMenuHorizontalTimeLineItems"] as ObservableCollection<object>;
            DefaultContextMenuHorizontalAppointmentItems = resourceDictionary["DefaultContextMenuHorizontalAppointmentItems"] as ObservableCollection<object>;
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
            ContextMenuHorizontalTimeSlotItems = this.GetTemplateChild("ContextMenuHorizontalTimeSlotItems") as ContextMenuAdv;
            ContextMenuHorizontalTimeLineItems = this.GetTemplateChild("ContextMenuHorizontalTimeLineItems") as ContextMenuAdv;
            ContextMenuHorizontalAppointmentItems = this.GetTemplateChild("ContextMenuHorizontalAppointmentItems") as ContextMenuAdv;
#else
            ContextMenuHorizontalTimeSlotItems = this.GetTemplateChild("ContextMenuHorizontalTimeSlotItems") as ContextMenu;
            ContextMenuHorizontalTimeLineItems = this.GetTemplateChild("ContextMenuHorizontalTimeLineItems") as ContextMenu;
            ContextMenuHorizontalAppointmentItems = this.GetTemplateChild("ContextMenuHorizontalAppointmentItems") as ContextMenu;
#endif
            ContextMenuHorizontalTimeSlotItems.DataContext = this;
            ContextMenuHorizontalTimeLineItems.DataContext = this;
            ContextMenuHorizontalAppointmentItems.DataContext = this;
            LoadContextMenuHorizontalViewTimeSlotItems();
            LoadContextMenuHorizontalViewTimeLineItems();
            LoadContextMenuHorizontalViewAppointmentItems(); 
        }
        private void DeInitializeContextMenu()
        {
#if SILVERLIGHT
            ContextMenuHorizontalTimeSlotItems = this.GetTemplateChild("ContextMenuHorizontalTimeSlotItems") as ContextMenuAdv;
            ContextMenuHorizontalTimeLineItems = this.GetTemplateChild("ContextMenuHorizontalTimeLineItems") as ContextMenuAdv;
            ContextMenuHorizontalAppointmentItems = this.GetTemplateChild("ContextMenuHorizontalAppointmentItems") as ContextMenuAdv;
#else
            ContextMenuHorizontalTimeSlotItems = this.GetTemplateChild("ContextMenuHorizontalTimeSlotItems") as ContextMenu;
            ContextMenuHorizontalTimeLineItems = this.GetTemplateChild("ContextMenuHorizontalTimeLineItems") as ContextMenu;
            ContextMenuHorizontalAppointmentItems = this.GetTemplateChild("ContextMenuHorizontalAppointmentItems") as ContextMenu;
#endif
            ContextMenuHorizontalTimeSlotItems.Visibility = Visibility.Collapsed;
            ContextMenuHorizontalTimeLineItems.Visibility = Visibility.Collapsed;
            ContextMenuHorizontalAppointmentItems.Visibility = Visibility.Collapsed;
            ContextMenuHorizontalTimeSlotItems = null;
            ContextMenuHorizontalTimeLineItems = null;
            ContextMenuHorizontalAppointmentItems = null; 
        }
        private void EnsureProperties()
        {
            if (this.isAppointStyleChangedBeforeLoaded)
            {
                this.horizontalAppointmentsLayoutItems.AppointmentStyle = this.AppointmentStyle;
            }
        }

        private void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Model.CurrentScheduleType != ScheduleType.ScheduleView) return;

            if (e.PropertyName == "SelectedDates" || e.PropertyName == "CurrentTimeInterval")
            {
                this.SetupAppointments();
            }
if (this.model.ShowContextMenu == true)
            {

                if (e.PropertyName == "ContextMenuHorizontalViewItems")
                {
                    LoadContextMenuHorizontalViewTimeSlotItems();
                }
                if (e.PropertyName == "ContextMenuHorizontalViewTimeLineItems")
                {
                    LoadContextMenuHorizontalViewTimeLineItems();
                }
                if (e.PropertyName == "ContextMenuHorizontalViewAppointmentItems")
                {
                    LoadContextMenuHorizontalViewAppointmentItems();
                }
                if (e.PropertyName == "ContextMenuType")
                {
                    LoadContextMenuHorizontalViewTimeSlotItems();
                    LoadContextMenuHorizontalViewTimeLineItems();
                    LoadContextMenuHorizontalViewAppointmentItems();
                }
            }

            if (e.PropertyName == "ShowContextMenu")
            {
                if (this.model.ShowContextMenu == true && this.model.CurrentScheduleType==ScheduleType.ScheduleView)
                {
                    InitializeContextMenu();
                }
                else if (this.model.ShowContextMenu == false && this.model.CurrentScheduleType == ScheduleType.ScheduleView)
                {
                    DeInitializeContextMenu(); 
                }
            }
        }
#if (SyncfusionFramework4_0) || !(SyncfusionFramework3_5 && SILVERLIGHT)
        private void LoadContextMenuHorizontalViewAppointmentItems()
        {
            if (ContextMenuHorizontalAppointmentItems != null)
            {
                if (this.model.ContextMenuType == ContextMenuType.Default)
                {
                    DefaultContextMenuHorizontalViewAppointmentItems();
                }
                else if (this.model.ContextMenuType == ContextMenuType.Custom)
                {
                    ContextMenuHorizontalAppointmentItems.Items.Clear();
                    if (this.model.ContextMenuHorizontalViewAppointmentItems.Count!=0)
                    {
                        ContextMenuHorizontalAppointmentItems.Visibility = Visibility.Visible;
                        CustomContextMenuHorizontalViewAppointmentItems();
                    }
                    else
                    {
                        ContextMenuHorizontalAppointmentItems.Visibility = Visibility.Collapsed;
                    }
                }
                else if (this.model.ContextMenuType == ContextMenuType.CustomWithDefault)
                {
                    DefaultContextMenuHorizontalViewAppointmentItems();
                    CustomContextMenuHorizontalViewAppointmentItems();
                }
            }
        }

        private void LoadContextMenuHorizontalViewTimeLineItems()
        {
            if (ContextMenuHorizontalTimeLineItems != null)
            {
                if (this.model.ContextMenuType == ContextMenuType.Default)
                {
                    DefaultContextMenuHorizontalViewTimeLineItems(); 
                }
                else if (this.model.ContextMenuType == ContextMenuType.Custom)
                {
                    ContextMenuHorizontalTimeLineItems.Items.Clear();
                    if (this.model.ContextMenuHorizontalViewTimeLineItems.Count!=0)
                    {
                        ContextMenuHorizontalTimeLineItems.Visibility = Visibility.Visible;
                        CustomContextMenuHorizontalViewTimeLineItems();
                    }
                    else
                    {
                        ContextMenuHorizontalTimeLineItems.Visibility = Visibility.Collapsed;
                    }
                }
                else if (this.model.ContextMenuType == ContextMenuType.CustomWithDefault)
                {
                    DefaultContextMenuHorizontalViewTimeLineItems();
                    CustomContextMenuHorizontalViewTimeLineItems();
                }
            }
        }

        private void LoadContextMenuHorizontalViewTimeSlotItems()
        {
            if (ContextMenuHorizontalTimeSlotItems != null)
            {
                if (this.model.ContextMenuType == ContextMenuType.Default)
                {
                    DefaultContextMenuHorizontalViewTimeSlotItems(); 
                }
                else if (this.model.ContextMenuType == ContextMenuType.Custom)
                {
                    ContextMenuHorizontalTimeSlotItems.Items.Clear();
                    if (this.model.ContextMenuHorizontalViewItems.Count!=0)
                    {
                        ContextMenuHorizontalTimeSlotItems.Visibility = Visibility.Visible;
                        CustomContextMenuHorizontalViewTimeSlotItems();
                    }
                    else
                    {
                        ContextMenuHorizontalTimeSlotItems.Visibility = Visibility.Collapsed;
                    }
                }
                else if (this.model.ContextMenuType == ContextMenuType.CustomWithDefault)
                {
                    DefaultContextMenuHorizontalViewTimeSlotItems();
                    CustomContextMenuHorizontalViewTimeSlotItems();
                }
            }
        }


        private void DefaultContextMenuHorizontalViewTimeSlotItems()
        {
            if (ContextMenuHorizontalTimeSlotItems != null)
            {
                ContextMenuHorizontalTimeSlotItems.Visibility = Visibility.Visible;
                ContextMenuHorizontalTimeSlotItems.Items.Clear();
                foreach (var item in DefaultContextMenuHorizontalTimeSlotItems)
                {
                    
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        ContextMenuHorizontalTimeSlotItems.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuHorizontalTimeSlotItems.Items.Add(s);
                    }
#else
                     if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        ContextMenuHorizontalTimeSlotItems.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        ContextMenuHorizontalTimeSlotItems.Items.Add(s);
                    }           
#endif

                }
            } 
        }

        private void CustomContextMenuHorizontalViewTimeSlotItems()
        {
            if (this.model.ContextMenuHorizontalViewItems != null)
            {
                foreach (var item in this.model.ContextMenuHorizontalViewItems)
                {
#if SILVERLIGHT
                     if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        m.Style = (Style)resourceDictionary["ContextMenuItemAdvStyle"];
                        ContextMenuHorizontalTimeSlotItems.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuHorizontalTimeSlotItems.Items.Add(s);
                    }           
#else
                     if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        m.Style = (Style)resourceDictionary["ContextMenuItemStyle"];
                        ContextMenuHorizontalTimeSlotItems.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        s.Style = (Style)resourceDictionary["SeparatorStyle"];
                        ContextMenuHorizontalTimeSlotItems.Items.Add(s);
                    }           
#endif
                           
                }
            }
        }

        private void DefaultContextMenuHorizontalViewTimeLineItems()
        {
            if (ContextMenuHorizontalTimeLineItems != null)
            {
                ContextMenuHorizontalTimeLineItems.Items.Clear();
                ContextMenuHorizontalTimeLineItems.Visibility = Visibility.Visible;
                foreach (var item in DefaultContextMenuHorizontalTimeLineItems)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        ContextMenuHorizontalTimeLineItems.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuHorizontalTimeLineItems.Items.Add(s);
                    }
#else
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        ContextMenuHorizontalTimeLineItems.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        ContextMenuHorizontalTimeLineItems.Items.Add(s);
                    }
#endif
                }
            }             
        }

        private void CustomContextMenuHorizontalViewTimeLineItems()
        {
            if (this.Model.ContextMenuHorizontalViewTimeLineItems != null)
            {
                foreach (var item in this.model.ContextMenuHorizontalViewTimeLineItems)
                {
#if SILVERLIGHT
                     if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        m.Style = (Style)resourceDictionary["ContextMenuItemAdvStyle"];
                        ContextMenuHorizontalTimeLineItems.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuHorizontalTimeLineItems.Items.Add(s);
                    }
#else
                     if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        m.Style = (Style)resourceDictionary["ContextMenuItemStyle"];
                        ContextMenuHorizontalTimeLineItems.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        s.Style = (Style)resourceDictionary["SeparatorStyle"];
                        ContextMenuHorizontalTimeLineItems.Items.Add(s);
                    }
#endif                   
                }
            }
        }

        private void DefaultContextMenuHorizontalViewAppointmentItems()
        {
            if (ContextMenuHorizontalAppointmentItems != null)
            {
                ContextMenuHorizontalAppointmentItems.Items.Clear();
                ContextMenuHorizontalAppointmentItems.Visibility = Visibility.Visible;
                foreach (var item in DefaultContextMenuHorizontalAppointmentItems)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        ContextMenuHorizontalAppointmentItems.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuHorizontalAppointmentItems.Items.Add(s);
                    }
#else
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        ContextMenuHorizontalAppointmentItems.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        ContextMenuHorizontalAppointmentItems.Items.Add(s);
                    }
#endif

                }
            } 
        }

        private void CustomContextMenuHorizontalViewAppointmentItems()
        {
            if (this.model.ContextMenuHorizontalViewAppointmentItems != null)
            {
                foreach (var item in this.model.ContextMenuHorizontalViewAppointmentItems)
                {
#if SILVERLIGHT
                    if (item.GetType() == typeof(ContextMenuItemAdv))
                    {
                        ContextMenuItemAdv m = item as ContextMenuItemAdv;
                        m.Style = (Style)resourceDictionary["ContextMenuItemAdvStyle"];
                        ContextMenuHorizontalAppointmentItems.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(SeparatorAdv))
                    {
                        SeparatorAdv s = item as SeparatorAdv;
                        ContextMenuHorizontalAppointmentItems.Items.Add(s);
                    }
#else
                    if (item.GetType() == typeof(MenuItem))
                    {
                        MenuItem m = item as MenuItem;
                        m.Style = (Style)resourceDictionary["ContextMenuItemStyle"];
                        ContextMenuHorizontalAppointmentItems.Items.Add(m);
                    }
                    else if (item.GetType() == typeof(Separator))
                    {
                        Separator s = item as Separator;
                        s.Style = (Style)resourceDictionary["SeparatorStyle"];
                        ContextMenuHorizontalAppointmentItems.Items.Add(s);
                    }
#endif
                    
                }
            }
        }

        /// <summary>
        /// Gets showAsCommand from ContextMenuCommand
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

        private void ColorButton1Method(object parameter)
        {
            string choice = (string)parameter;
            ContextMenuHorizontalTimeSlotItems.IsOpen = false;
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
        /// Gets OpenAppointment from ContextMenuCommand
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
            if (!this.Model.AllowDelete)
            {
                return;
            }
            this.model.DeleteCurrentSelectedAppointment();
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
        /// Gets TimeIntervalSixtyMinutes  from contextMenu
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
        ///  Using a DependencyProperty as the backing store for AppointmentStyle.  This
        /// enables animation, styling, binding, etc...
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
        ///  Gets TimeIntervalTwentyMinutes property from Context Menu
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
        /// Gets TimeIntervelFiveMinutes from context menu
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
        /// Gets TimeIntervelSixMinutesfrom contextMenu
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
        /// Gets TimeIntervelFiveMinutes contextMenu
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
            var appointment = this.Model.CreateNewAppointment(false);
            appointment.IsRecurrenceAppointment = true;
            var appWrapper = new ScheduleAppointmentWrapper(appointment, this.Model, true);
            this.ShowWindow(appWrapper, Visibility.Visible);
        }

        private void NewRecurringEventMethod(object parameter)
        {
            if (this.Model.SelectedStartTimeSpan == DateTime.MinValue || this.Model.SelectedEndTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }
            var appointment = this.Model.CreateNewAppointment(true);
            appointment.IsRecurrenceAppointment = true;
            var appWrapper = new ScheduleAppointmentWrapper(appointment, this.Model, true);
            this.ShowWindow(appWrapper, Visibility.Visible);
        }

        private void AllDayAppointmentMethod(object parameter)
        {
            if (this.Model.SelectedStartTimeSpan == DateTime.MinValue || this.Model.SelectedEndTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }
            var appointment = this.Model.CreateNewAppointment(true);            
            this.ShowWindow(appointment, false);
        }

        /// <summary>
        ///  Gets TodayDate property from Context Menu
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
            this.model.SelectedDates.Clear();
            ObservableCollection<DateTime> dates = new ObservableCollection<DateTime>();
            dates.Add(DateTime.Now.Date);
            this.model.SelectedDates = dates;
        }
        /// <summary>
        /// Gets GoToDate from context menu
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

            if (s.ToString() == "Day Calender")
            {
                dates.Add(editor.PART_GoToDatePicker.SelectedDate.Value);
                this.model.SelectedDates = dates;
                this.model.CurrentScheduleType = ScheduleType.Day;
            }
            else if (s.ToString() == "Month Calender")
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
            else if (s.ToString() == "Week Calender")
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
            else if (s.ToString() == "WorkWeek Calender")
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
#endif
        #region AppointmentEvents
        private void SetupAppointmentEvents()
        {
            this.horizontalAppointmentsLayoutItems.MouseRightClick += new MouseButtonEventHandler(horizontalAppointmentsLayoutItems_MouseRightClick);
            this.horizontalAppointmentsLayoutItems.MouseLeftButtonDown += new MouseButtonEventHandler(appointmentsLayoutItems_MouseLeftButtonDown);
            this.horizontalAppointmentsLayoutItems.MouseMove += new MouseEventHandler(appointmentsLayoutItems_MouseMove);
            this.horizontalAppointmentsLayoutItems.MouseLeftButtonUp += new MouseButtonEventHandler(appointmentsLayoutItems_MouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(ScheduleDaysView_MouseMove);
#if !SILVERLIGHT
            this.DragPopUp.MouseLeftButtonUp += new MouseButtonEventHandler(appointmentsLayoutItems_MouseLeftButtonUp);
            this.DragPopUp.MouseMove += new MouseEventHandler(ScheduleDaysView_MouseMove); 
#endif
        }

        void horizontalAppointmentsLayoutItems_MouseRightClick(object sender, MouseButtonEventArgs e)
        {
            timeInterval = 60 / (ScheduleHorizontalTimeLineHourControl.IntervalCount[(int)this.Model.CurrentTimeInterval]);
            this.SetDefaultValuesForResize();
#if SILVERLIGHT
            FrameworkElement sendr = sender as FrameworkElement;
            sendr.CaptureMouse();
            ScheduleHorizontalAppointmentViewControl el = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleHorizontalAppointmentViewControl>().FirstOrDefault();
#else
            ScheduleHorizontalAppointmentViewControl el = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleHorizontalAppointmentViewControl") as ScheduleHorizontalAppointmentViewControl;
            Border elbdrtop = GetHitTestParentElementByName(this.InputHitTest(e.GetPosition(this)), "ScheduleHorizontalAppointmentViewControl", "PART_LeftEdge") as Border;
            Border elbdrbottom = GetHitTestParentElementByName(this.InputHitTest(e.GetPosition(this)), "ScheduleHorizontalAppointmentViewControl", "PART_RightEdge") as Border;
#endif
            if (el == null) return;

            if (this.Model != null)
            {
                ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(el.ScheduleAppointment);
                this.Model.GetAppointmentClickEvents(args);
            }
            StartingPoint = e.GetPosition(el);
            this.ClearAppointmentLayoutSelectedItems();
            this.ClearTimeSlotSelection();
            el.IsSelected = true;
            this.Model.CurrentSelectedAppointment = el.ScheduleAppointment;
        }

        private void ScheduleDaysView_MouseMove(object sender, MouseEventArgs e)
        {
            var schedule = (sender as ScheduleHorizontalView).FindParentElementOfType<Schedule>();
            if (this.isDragging && schedule != null && schedule.SelectedAppointment.AllowDragandDrop)
            {
                Schedule sch = GetScheduleParent();
                sch.OnAppointmentDragged(new ScheduleAppointmentDragEventArgs(this.draggedAppointment));
                if (draggedAppointment.IsRecurrenceAppointment == false && draggedAppointment.IsAppointmentProxyCollection == false)
                {
                    double currentVerticalPosition = e.GetPosition(AppointmentPopup).Y;
                    double currentHorizontalPosition = e.GetPosition(AppointmentPopup).X;
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

        ScheduleHorizontalAppointmentViewControl mouseDownDaysAppEl = null;
        private bool isDaysAppMouseDown = false;
        private bool isallowResized = false;
        private Point StartingPoint;
        private double actualAppWidth = 0;
        private Popup popupResize;
        private Popup DragPopUp;

        private bool isDragged = false;
        private bool isDragging = false;
        private ScheduleAppointment draggedAppointment;
        private int timeInterval = 30;

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
            }

            return null;
        }

        private DependencyObject GetHitTestParentElementByName(IInputElement childobj, string dependencyobjType, string dependencyobjName)
        {
            FrameworkElement dependencyObj = childobj as FrameworkElement;

            if (dependencyObj != null)
            {
                if (dependencyObj.TemplatedParent != null)
                {
                    if (dependencyObj.TemplatedParent.DependencyObjectType.Name == dependencyobjType &&
                        dependencyObj.Name == dependencyobjName)
                        return dependencyObj;
                }
            }

            return null;
        }
        #endregion
#endif

        private void appointmentsLayoutItems_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            timeInterval = 60 / (ScheduleHorizontalTimeLineHourControl.IntervalCount[(int)this.Model.CurrentTimeInterval]);
            this.SetDefaultValuesForResize();
#if SILVERLIGHT
            FrameworkElement sendr = sender as FrameworkElement;
            sendr.CaptureMouse();
            ScheduleHorizontalAppointmentViewControl el = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleHorizontalAppointmentViewControl>().FirstOrDefault();
#else
            ScheduleHorizontalAppointmentViewControl el = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleHorizontalAppointmentViewControl") as ScheduleHorizontalAppointmentViewControl;
            Border elbdrtop = GetHitTestParentElementByName(this.InputHitTest(e.GetPosition(this)), "ScheduleHorizontalAppointmentViewControl", "PART_LeftEdge") as Border;
            Border elbdrbottom = GetHitTestParentElementByName(this.InputHitTest(e.GetPosition(this)), "ScheduleHorizontalAppointmentViewControl", "PART_RightEdge") as Border;
#endif
            if (el == null) return;

            if (this.Model != null)
            {
                ScheduleAppointmentEventArgs args = new ScheduleAppointmentEventArgs(el.ScheduleAppointment);
                this.Model.GetAppointmentClickEvents(args);
            }
            StartingPoint = e.GetPosition(el);
            this.ClearAppointmentLayoutSelectedItems();
            this.ClearTimeSlotSelection();
            MarkSelectedAppointments(el);
            this.Model.CurrentSelectedAppointment = el.ScheduleAppointment;
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
            if (el.DragStatus == MousePointerType.Resize && this.Model.AllowResize == true && el.ScheduleAppointment.AllowResize == true)
            {
                Point currpopMargin = e.GetPosition(sender as FrameworkElement);

                this.ClearAppointmentPopupWindow();
                popupResize = GetPopUpWithContent(el);
#if SILVERLIGHT
                popupResize.Margin = new Thickness(currpopMargin.X - StartingPoint.X, currpopMargin.Y - StartingPoint.Y, 0, 0);
#else
                Application.Current.MainWindow.UpdateLayout();
                Point StartingPointEle = e.GetPosition(el);
                Point MainWindowPoint = e.GetPosition(Application.Current.MainWindow);
                double leftValue = MainWindowPoint.X + Application.Current.MainWindow.Left - StartingPointEle.X + 8;
                double topValue = MainWindowPoint.Y + Application.Current.MainWindow.Top - StartingPointEle.Y + 30;
                (popupResize.Child as Border).Margin = new Thickness(leftValue, topValue, 0, 0);
                popupResize.IsOpen = true;
#endif
                this.AppointmentPopup.Children.Add(popupResize);

                isDaysAppMouseDown = true;
                mouseDownDaysAppEl = el;

                if (double.IsNaN(mouseDownDaysAppEl.Width))
                    actualAppWidth = mouseDownDaysAppEl.ActualWidth;
                else
                    actualAppWidth = mouseDownDaysAppEl.Width;

                actualAppWidth = actualAppWidth / (mouseDownDaysAppEl.ScheduleAppointment.Duration.TotalMinutes / timeInterval);
            }
            else
            {
                this.PickUpAppointment(el);
            }
        }

        private void MarkSelectedAppointments(ScheduleHorizontalAppointmentViewControl el)
        {
            if (el.ScheduleAppointment.MultiDayAppointment)
            {
# if Silverlight
                 var appCtl = from item in this.horizontalAppointmentsLayoutItems.Items
                             where (((this.horizontalAppointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleHorizontalAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentStartTime == el.ScheduleAppointment.MultiDayAppointmentStartTime) && ((this.horizontalAppointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleHorizontalAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentEndTime == el.ScheduleAppointment.MultiDayAppointmentEndTime))
                             select item;

#endif
# if !Silverlight
                var appCtl = from item in this.horizontalAppointmentsLayoutItems.Items.Cast<ScheduleHorizontalAppointmentViewControl>()
                             where (((this.horizontalAppointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleHorizontalAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentStartTime == el.ScheduleAppointment.MultiDayAppointmentStartTime) && ((this.horizontalAppointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleHorizontalAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentEndTime == el.ScheduleAppointment.MultiDayAppointmentEndTime))
                             select item;
#endif
                if (appCtl.Count() > 0)
                {
                    for (int i = 0; i < appCtl.Count(); i++)
                    {
                        (this.horizontalAppointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(appCtl.ElementAt(i)) as ScheduleHorizontalAppointmentViewControl).IsSelected = true;
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

        private Popup GetPopUpWithContent(ScheduleHorizontalAppointmentViewControl mouseDownDaysAppEl)
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
            ScheduleHorizontalAppointmentViewControl elrep = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleHorizontalAppointmentViewControl>().FirstOrDefault();
#else
            ScheduleHorizontalAppointmentViewControl elrep = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleHorizontalAppointmentViewControl") as ScheduleHorizontalAppointmentViewControl;
#endif
            Point curroffset = e.GetPosition(mouseDownDaysAppEl);

            if ((curroffset.X == 0 && curroffset.Y == 0) || mouseDownDaysAppEl.ScheduleAppointment.AllowResize == false) return;

            if (double.IsNaN(mouseDownDaysAppEl.Width))
                mouseDownDaysAppEl.Width = mouseDownDaysAppEl.ActualWidth;

            double diffWidth = curroffset.X - StartingPoint.X;
            if (diffWidth != 0) isallowResized = true;
            var startTempTime = new DateTime();
            var endTempTime = new DateTime();

            if (mouseDownDaysAppEl.DragStatus == MousePointerType.Resize && mouseDownDaysAppEl.MousePosition == ResizePosition.Right && diffWidth != 0)
            {
                if (mouseDownDaysAppEl.Width + diffWidth <= 0) return;
                mouseDownDaysAppEl.Width += diffWidth;
                if (popupResize != null)
                {
                    var elementApp = popupResize.Child as FrameworkElement;
                    if (elementApp.Width + diffWidth <= 0) return;
                    elementApp.Width += diffWidth;
                }
            }
            else if (mouseDownDaysAppEl.DragStatus == MousePointerType.Resize && mouseDownDaysAppEl.MousePosition == ResizePosition.Left && diffWidth != 0)
            {
                if (mouseDownDaysAppEl.Width + diffWidth <= 0 || mouseDownDaysAppEl.Width - diffWidth <= 0) return;
                mouseDownDaysAppEl.Width -= diffWidth;

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
            if (this.Model != null)
            {
                int currCount = Convert.ToInt32(Math.Floor(mouseDownDaysAppEl.Width / actualAppWidth));
                double totmin = timeInterval * currCount;
                if (mouseDownDaysAppEl.MousePosition == ResizePosition.Right)
                {
                    while (mouseDownDaysAppEl.ScheduleAppointment.StartTime.AddMinutes(totmin).Day > mouseDownDaysAppEl.ScheduleAppointment.StartTime.Day)
                    {
                        totmin -= 0.01;
                    }
                    endTempTime = mouseDownDaysAppEl.ScheduleAppointment.StartTime.AddMinutes(totmin);
                }
                else endTempTime = mouseDownDaysAppEl.ScheduleAppointment.EndTime;
                if (mouseDownDaysAppEl.MousePosition == ResizePosition.Left)
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
                if (double.IsNaN(mouseDownDaysAppEl.Width))
                    mouseDownDaysAppEl.Width = mouseDownDaysAppEl.ActualWidth;

                var startTime = mouseDownDaysAppEl.ScheduleAppointment.StartTime;
                var endTime = mouseDownDaysAppEl.ScheduleAppointment.EndTime;

                if (actualAppWidth > 0 && !double.IsNaN(mouseDownDaysAppEl.Width) && mouseDownDaysAppEl.DragStatus == MousePointerType.Resize
                    && mouseDownDaysAppEl.MousePosition != ResizePosition.None)
                {
                    int currCount = Convert.ToInt32(Math.Floor(mouseDownDaysAppEl.Width / actualAppWidth));
                    double remaniner = actualAppWidth - (mouseDownDaysAppEl.Width % actualAppWidth);
                    currCount = (remaniner > 0) ? currCount + 1 : currCount;
                    mouseDownDaysAppEl.Width = (remaniner > 0) ? mouseDownDaysAppEl.Width + remaniner : mouseDownDaysAppEl.Width;
                    double totmin = timeInterval * currCount;

                    var remApp = (from res in this.Model.Appointments
                                  where res.Subject == mouseDownDaysAppEl.ScheduleAppointment.Subject && res.Location == mouseDownDaysAppEl.ScheduleAppointment.Location
                                  && res.StartTime == mouseDownDaysAppEl.ScheduleAppointment.StartTime && res.EndTime == mouseDownDaysAppEl.ScheduleAppointment.EndTime
                                  select res).FirstOrDefault();

                    if (mouseDownDaysAppEl.MousePosition == ResizePosition.Right)
                    {
                        while (mouseDownDaysAppEl.ScheduleAppointment.StartTime.AddMinutes(totmin).Day > mouseDownDaysAppEl.ScheduleAppointment.StartTime.Day)
                        {
                            totmin -= 0.01;
                        }
                        mouseDownDaysAppEl.ScheduleAppointment.EndTime = mouseDownDaysAppEl.ScheduleAppointment.StartTime.AddMinutes(totmin);
                    }
                    else if (mouseDownDaysAppEl.MousePosition == ResizePosition.Left)
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

                    var app = mouseDownDaysAppEl.ScheduleAppointment;
                    UpdateResizedAppointment(remApp, app);                   
                    //this.Model.Appointments.Remove(remApp);
                    //var addApp = new ScheduleAppointment();
                    //addApp.InitializeFromApp(app);
                    //this.Model.Appointments.Add(addApp);
                    //this.UpdateLayout();
                    foreach (var item in this.horizontalAppointmentsLayoutItems.Items)
                    {
                        var viewControl = this.horizontalAppointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleHorizontalAppointmentViewControl;
                        if (viewControl != null &&  viewControl.ScheduleAppointment.MatchWithExists(app) == true)
                        {
                            viewControl.IsSelected = true;
                            break;
                        }
                    }
                }

                if (this.Model != null)
                {
                    ScheduleAppointmentResizedEventArgs args = new ScheduleAppointmentResizedEventArgs(mouseDownDaysAppEl.ScheduleAppointment,startTime, endTime);
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
                this.horizontalAppointmentsLayoutItems.Items.Remove(resapp);
                this.draggedAppointment.StartTime = app.StartTime;
                this.draggedAppointment.EndTime = app.EndTime;
                this.draggedAppointment.MultiDayAppointmentStartTime = app.StartTime;
                this.draggedAppointment.MultiDayAppointmentEndTime = app.EndTime;
                this.draggedAppointment.AllDay = false;

                this.Model.Appointments[index].StartTime = app.StartTime;
                this.Model.Appointments[index].EndTime = app.EndTime;
                this.Model.Appointments[index].AllDay = false;

                this.horizontalAppointmentsLayoutItems.Items.Add(this.Model.Appointments[index]);
                if (app.Record != null)
                {
                    var record = draggedAppointment.Record;
                    this.Model.View.EditItem(record.Data);
                    var pd = this.Model.View.GetPropertyAccessProvider();
                    this.Model.SetPropertiesOnNewItem(pd, record.Data, app);
                    this.Model.View.CommitEdit();
                }
            }
            // this.UpdateLayout();
            this.SetupAppointments();
        }
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
#if !SILVERLIGHT
        internal ScheduleAppointment GetTimeIntevalFromMousePoint(RoutedEventArgs e)
        {
#if SILVERLIGHT
            ScheduleHorizontalView v = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleHorizontalView>().FirstOrDefault();
#else
                ScheduleHorizontalView v = FindAnchestor<ScheduleHorizontalView>((DependencyObject)e.OriginalSource);
#endif
            if (v != null)
            {
                
                
#if SILVERLIGHT
                    ScheduleHorizontalTimeSlotControl slot = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleHorizontalTimeSlotControl>().FirstOrDefault();
#else
                    ScheduleHorizontalTimeSlotControl slot = FindAnchestor<ScheduleHorizontalTimeSlotControl>((DependencyObject)e.OriginalSource);
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
                            ScheduleHorizontalRectangleBorderExt rectangle = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), usp).OfType<ScheduleHorizontalRectangleBorderExt>().FirstOrDefault();
#else
                            //ScheduleHorizontalRectangleBorderExt rectangle = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleHorizontalRectangleBorderExt") as ScheduleHorizontalRectangleBorderExt;
                            ScheduleHorizontalRectangleBorderExt rectangle = FindAnchestor<ScheduleHorizontalRectangleBorderExt>((DependencyObject)e.OriginalSource);
#endif
                            int position = usp.Children.IndexOf(rectangle);
                            int min = (60 / usp.Children.Count) * position;
                            DateTime newappdate = slot.DateTime;
                            newappdate = newappdate.Add(new TimeSpan(slot.Hour, min, 0));
                            ScheduleAppointment newapp = new ScheduleAppointment();
                            newapp.StartTime = newappdate;
                            newapp.EndTime = newapp.StartTime.Add(TimeIntervalToTimeSpanConverter(this.model.CurrentTimeInterval));
                            return newapp;
                        }
                    }
#if SILVERLIGHT
                    ScheduleHorizontalDaysHeaderViewControl view = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleHorizontalDaysHeaderViewControl>().FirstOrDefault();
#else
                    ScheduleHorizontalDaysHeaderViewControl view = FindAnchestor<ScheduleHorizontalDaysHeaderViewControl>((DependencyObject)e.OriginalSource);
#endif
                    if (view != null)
                    {
                        DateTime Startappdate = view.DateTime;
                        ScheduleAppointment app = new ScheduleAppointment();
                        app.StartTime = Startappdate;
                        app.EndTime = Startappdate.Add(TimeIntervalToTimeSpanConverter(this.model.CurrentTimeInterval));                       
                        app.AllDay = true;
                        return app;
                    }
                
            }

            return null;
        }
#endif
        private void DropAppointment(MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            ScheduleHorizontalView v = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleHorizontalView>().FirstOrDefault();
#else
                ScheduleHorizontalView v = FindAnchestor<ScheduleHorizontalView>((DependencyObject)e.OriginalSource);
#endif
            if (v != null)
            {
                if (this.isDragged && !isDaysAppMouseDown)
                {
#if SILVERLIGHT
                    ScheduleHorizontalTimeSlotControl slot = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleHorizontalTimeSlotControl>().FirstOrDefault();
#else
                    ScheduleHorizontalTimeSlotControl slot = FindAnchestor<ScheduleHorizontalTimeSlotControl>((DependencyObject)e.OriginalSource);
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
                            ScheduleHorizontalRectangleBorderExt rectangle = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), usp).OfType<ScheduleHorizontalRectangleBorderExt>().FirstOrDefault();
#else
                            ScheduleHorizontalRectangleBorderExt rectangle = GetHitTestParentElement(this.InputHitTest(e.GetPosition(this)), "ScheduleHorizontalRectangleBorderExt") as ScheduleHorizontalRectangleBorderExt;
#endif
                            int position = usp.Children.IndexOf(rectangle);
                            int min = (60 / usp.Children.Count) * position;
                            DateTime newappdate = slot.DateTime;
                            newappdate = newappdate.Add(new TimeSpan(slot.Hour, min, 0));
                            if (this.draggedAppointment.MultiDayAppointment)
                            { 
                              DragDropMultiDay(newappdate);
                            }
                            else //SD17775 Fixed // if (newappdate < draggedAppointment.StartTime || newappdate > draggedAppointment.EndTime)
                            {
                                ScheduleAppointment newapp = new ScheduleAppointment();
                                newapp.StartTime = newappdate;
                                newapp.EndTime = newapp.StartTime.Add(draggedAppointment.Duration);
                                newapp.Subject = draggedAppointment.Subject;
                                newapp.Location = draggedAppointment.Location;
                                Schedule sch = GetScheduleParent();
                                sch.OnAppointmentDropping(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                                
                                this.horizontalAppointmentsLayoutItems.Items.Remove(this.draggedAppointment);
                                UpdateDraggedAppointment(newapp);
                                this.draggedAppointment.StartTime = newapp.StartTime;
                                this.draggedAppointment.EndTime = newapp.EndTime;
                                this.horizontalAppointmentsLayoutItems.Items.Add(this.draggedAppointment);

                                if (this.draggedAppointment.Record != null)
                                {
                                    var record = draggedAppointment.Record;
                                    this.Model.View.EditItem(record.Data);
                                    var pd = this.Model.View.GetPropertyAccessProvider();
                                    this.Model.SetPropertiesOnNewItem(pd, record.Data, this.draggedAppointment);
                                    this.Model.View.CommitEdit();
                                }
                                //if (newapp.StartTime != draggedAppointment.StartTime || newapp.EndTime != draggedAppointment.EndTime || newapp.Location != draggedAppointment.Location || newapp.Subject != draggedAppointment.Subject)
                                //{
                                //    this.Model.Appointments.Remove(draggedAppointment);
                                //    this.Model.Appointments.Add(newapp);
                                //}
                                this.UpdateLayout();


                                sch.OnAppointmentDropped(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                            }
                        }
                    }
                    else
                    {

                        DateTime newappdate = this.model.SelectedDates[(int)e.GetPosition(this.horizontaltimeSlotItems).Y / ((int)this.horizontaltimeSlotItems.Model.GetTimeSlotWidth() / this.model.SelectedDates.Count)];
                        newappdate = newappdate.AddMinutes(((int)e.GetPosition(this.horizontaltimeSlotItems).X / (int)this.model.IntervalHeight) * this.model.GetCurrentTimeIntervalInMinutes());

                        //ScheduleDaysAppointmentViewControl appView = FindAnchestor<ScheduleDaysAppointmentViewControl>((DependencyObject)e.OriginalSource);
                        //DateTime newappdate = appView.ScheduleAppointment.StartTime;
                        //SD17775 Fixed // if (newappdate < draggedAppointment.StartTime || newappdate > draggedAppointment.EndTime)
                        {
                            this.horizontalAppointmentsLayoutItems.Items.Remove(this.draggedAppointment);
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
                            this.horizontalAppointmentsLayoutItems.Items.Add(newapp);
                            Schedule sch = GetScheduleParent();
                            sch.OnAppointmentDropping(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                            if (newapp.StartTime != draggedAppointment.StartTime || newapp.EndTime != draggedAppointment.EndTime || newapp.Location != draggedAppointment.Location || newapp.Subject != draggedAppointment.Subject)
                            {

                                this.horizontalAppointmentsLayoutItems.Items.Remove(this.draggedAppointment);
                                this.draggedAppointment.StartTime = newapp.StartTime;
                                this.draggedAppointment.EndTime = newapp.EndTime;
                                this.draggedAppointment.AllDay = false;
                                this.horizontalAppointmentsLayoutItems.Items.Add(this.draggedAppointment);
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
                    ScheduleHorizontalDaysHeaderViewControl view = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleHorizontalDaysHeaderViewControl>().FirstOrDefault();
#else
                    ScheduleHorizontalDaysHeaderViewControl view = FindAnchestor<ScheduleHorizontalDaysHeaderViewControl>((DependencyObject)e.OriginalSource);
#endif
                    if (view != null)
                    {
                        DateTime Startappdate = view.DateTime;
                        ScheduleAppointment app = new ScheduleAppointment();
                        app.StartTime = Startappdate;
                        app.EndTime = app.StartTime.Add(draggedAppointment.Duration);
                        app.Subject = draggedAppointment.Subject;
                        app.Location = draggedAppointment.Location;
                        app.AllDay = true;
                        Schedule sch = GetScheduleParent();
                        sch.OnAppointmentDropping(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                        this.horizontalAppointmentsLayoutItems.Items.Remove(this.draggedAppointment);
                        this.draggedAppointment.StartTime = app.StartTime;
                        this.draggedAppointment.EndTime = app.EndTime;
                        this.draggedAppointment.AllDay = true;
                        this.horizontalAppointmentsLayoutItems.Items.Add(this.draggedAppointment);

                        //if (app.StartTime != draggedAppointment.StartTime || app.EndTime != draggedAppointment.EndTime || app.Location != draggedAppointment.Location || app.Subject != draggedAppointment.Subject || app.AllDay != draggedAppointment.AllDay)
                        //{
                        //    this.Model.Appointments.Remove(draggedAppointment);
                        //    this.Model.Appointments.Add(app);
                        //}
                        //this.UpdateLayout();
                        sch.OnAppointmentDropped(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                    }
                }
            }
            this.AppointmentPopup.Children.Remove(DragPopUp);
            if (DragPopUp != null) this.DragPopUp.IsOpen = false;
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
        /// <summary>
        /// Pefroms the drag Drop operation in cases of Multidays
        /// </summary>
        /// <param name="newappdate"></param>
        private void DragDropMultiDay(DateTime newappdate)
        {

            if (this.draggedAppointment.MultiDayAppointmentStartTime > newappdate || this.draggedAppointment.MultiDayAppointmentEndTime < newappdate)
            {
                var actualDuration = this.draggedAppointment.MultiDayAppointmentEndTime - this.draggedAppointment.MultiDayAppointmentStartTime;
                var modifiedDuration = (newappdate.TimeOfDay + actualDuration);
#if SILVERLIGHT
   var appCtl = from item in this.horizontalAppointmentsLayoutItems.Items
                             where (((this.horizontalAppointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleHorizontalAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentStartTime == this.draggedAppointment.MultiDayAppointmentStartTime) && ((this.horizontalAppointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleHorizontalAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentEndTime == this.draggedAppointment.MultiDayAppointmentEndTime))
                             select item;
#endif
           #if !SILVERLIGHT        
            var appCtl = from item in this.horizontalAppointmentsLayoutItems.Items.Cast < ScheduleHorizontalAppointmentViewControl>()
                             where (((this.horizontalAppointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleHorizontalAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentStartTime == this.draggedAppointment.MultiDayAppointmentStartTime) && ((this.horizontalAppointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleHorizontalAppointmentViewControl).ScheduleAppointment.MultiDayAppointmentEndTime == this.draggedAppointment.MultiDayAppointmentEndTime))
                             select item;
#endif
                var rootappointment = (from app in this.Model.Appointments
                                       where (app.MultiDayAppointmentStartTime == this.draggedAppointment.MultiDayAppointmentStartTime && app.MultiDayAppointmentEndTime == this.draggedAppointment.MultiDayAppointmentEndTime)
                                       select app).FirstOrDefault();

                if (appCtl.Count() > 0)
                {
                    Schedule sch = GetScheduleParent();
                    sch.OnAppointmentDropping(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                    this.horizontalAppointmentsLayoutItems.Items.Remove(appCtl.FirstOrDefault());
                    this.horizontalAppointmentsLayoutItems.Items.Remove(appCtl.LastOrDefault());

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
                    this.horizontalAppointmentsLayoutItems.Items.Add(this.draggedAppointment);
                    sch.OnAppointmentDropped(new ScheduleAppointmentDropEventArgs(this.draggedAppointment));
                    this.UpdateLayout();
                }
            }
        }
        private void PickUpAppointment(ScheduleHorizontalAppointmentViewControl el)
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

                ScheduleHorizontalAppointmentViewControl re = new ScheduleHorizontalAppointmentViewControl();
                re.DataContext = this.draggedAppointment;
                re.Height = this.ActualHeight / 7;
                re.Width = this.Model.IntervalHeight;
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
            actualAppWidth = 0;
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
            foreach (var item in this.horizontalAppointmentsLayoutItems.Items)
            {
                var viewControl = this.horizontalAppointmentsLayoutItems.ItemContainerGenerator.ContainerFromItem(item) as ScheduleHorizontalAppointmentViewControl;
                if (viewControl != null && viewControl.IsSelected)
                {
                    viewControl.IsSelected = false;
                }
            }
        }

        #endregion

        #region Appointments population
        internal void SetupAppointments()
        {
            if (this.Model == null || this.horizontalAppointmentsLayoutItems == null || this.Model.Appointments == null)
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
                        this.horizontalAppointmentsLayoutItems.Items.Add(appInfo.Appointment);
                    }
                }
            }
            this.UpdateLayout();
            this.Model.RaiseAppointmentDatesBold();
            this.Model.Appointments.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnAppointmentsCollectionChanged);
        }

        private void ClearAppointments()
        {
            this.horizontalAppointmentsLayoutItems.Items.Clear();
#if SILVERLIGHT
            this.Dispatcher.BeginInvoke(() =>
#else
            this.Dispatcher.BeginInvoke(new Action(() =>
#endif
            {
                var appointmentsLayoutPanel = this.horizontalAppointmentsLayoutItems.FindElementOfType<ScheduleHorizontalAppointmentLayoutPanel>();
                if (appointmentsLayoutPanel != null)
                {
                    appointmentsLayoutPanel.Reset();
                }
#if SILVERLIGHT
            });
#else
    }));
#endif
        }

        private void OnAppointmentsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.Model == null || this.horizontalAppointmentsLayoutItems == null || this.Model.CurrentScheduleType != ScheduleType.ScheduleView)
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
                        if (!app.IsAllDayOrSpanned())
                        {
                            this.horizontalAppointmentsLayoutItems.Items.Add(app);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (ScheduleAppointment app in e.OldItems)
                    {
                        if (!app.IsAllDayOrSpanned())
                        {
                            var appointmentView = this.horizontalAppointmentsLayoutItems.Items.OfType<ScheduleAppointment>().FirstOrDefault(c => c == app);
                            if (appointmentView != null)
                            {
                                this.horizontalAppointmentsLayoutItems.Items.Remove(appointmentView);
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
        }

        #endregion

        #region Appointment Editor

        private void SetupDoubleClick()
        {
            this.horizontaltimeSlotItems.MouseDoubleClick += new MouseButtonEventHandler(timeSlotItems_MouseDoubleClick);
            this.horizontalAppointmentsLayoutItems.MouseDoubleClick += new MouseButtonEventHandler(appointmentsLayoutItems_MouseDoubleClick);
        }

        private void appointmentsLayoutItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
            if (!this.Model.AllowEdit)
            {
                return;
            }

#if SILVERLIGHT
             var showEditorWindow = false;
            var items = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), this).OfType<ScheduleHorizontalAppointmentViewControl>().ToList();
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

            ScheduleHorizontalAppointmentViewControl itemevt = GetHitTestParentElement(items, "ScheduleHorizontalAppointmentViewControl") as ScheduleHorizontalAppointmentViewControl;
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

        private void timeSlotItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            Point currentObjPos = e.GetPosition(Application.Current.RootVisual);
            ScheduleHorizontalRectangleSel etop = VisualTreeHelper.FindElementsInHostCoordinates(currentObjPos, this).OfType<ScheduleHorizontalRectangleSel>().FirstOrDefault();
#else
            Point currentObjPos = e.GetPosition(this);
            ScheduleHorizontalRectangleSel etop = GetHitTestParentElement(this.InputHitTest(currentObjPos), "ScheduleHorizontalRectangleSel") as ScheduleHorizontalRectangleSel;
#endif
            if (etop != null) return;
            var startTimeSpan = this.Model.SelectedStartTimeSpan;
            var endTimeSpan = this.Model.SelectedEndTimeSpan;
            if (startTimeSpan == DateTime.MinValue || endTimeSpan == DateTime.MinValue || !this.Model.AllowAddNew)
            {
                return;
            }
            IsDisableMouseMove = true;
            var appointment = this.Model.CreateNewAppointment(startTimeSpan, endTimeSpan, false );
            this.ShowWindow(appointment, false);
        }

        private void ShowWindow(ScheduleAppointment appointment, bool isEditing)
        {
            IsRecurrenceModified = false;
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
#if SILVERLIGHT
                if (this.AppointmentScheduleWindowStyle != null)
                {
                    childwindow.Style = this.AppointmentScheduleWindowStyle;
                }
#else
                childwindow.Height = 170;
                childwindow.ResizeMode = ResizeMode.NoResize;
                childwindow.WindowStartupLocation = WindowStartupLocation.Manual;
                Schedule s = this.GetScheduleParent();
                Point schedulePoint = s.PointToScreen(new Point());
                childwindow.Left = schedulePoint.X + (s.ActualWidth - childwindow.Width) / 2;
                childwindow.Top = schedulePoint.Y + (s.ActualHeight - childwindow.Height) / 2;  
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
                appWrapper.Appointment.IsRecurrenceAppointment  = false;
                appWrapper.Appointment.AllowRecurrence = false;
                ShowWindow(appWrapper, Visibility.Collapsed);
            }
        }

        private void editorControl_RecurrenceButtonClick(object sender, RoutedEventArgs e)
        {
            var editor = sender as ScheduleAppointmentEditorControl;
            editor.RecurrenceButtonClick -= new RoutedEventHandler(editorControl_RecurrenceButtonClick);

            IsRecurrenceModified = true;
            var appWrapper = editor.DataContext as ScheduleAppointmentWrapper;
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
        ///  Get a new child window
        /// </summary>
        protected virtual Window GetChildWindow()
        {
            var window = new Window();
            window.Width = 640;
            window.Height = 570;
            return window;
        }
        /// <summary>
        /// this method gets the child window based on the width
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

//            var editor = (sender as ChildWindow).Content as ScheduleAppointmentEditorControl;
            var appWrapper = editor.DataContext as ScheduleAppointmentWrapper;
            if (editor != null && (IsRecurrenceModified == true || editor.IsStateChanged))
            {
                ResourceWrapper rw = new ResourceWrapper();
                //var result = MessageBox.Show("Do you want to save changes?", "Scheduler", MessageBoxButton.OKCancel);
                var result = MessageBox.Show(rw.AppointmentSaveChangesMessageBoxContent,rw.AppointmentSaveChangesMessageBoxHeader, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    SaveAppointments(editor);
                }
            }
            IsRecurrenceModified = false;
        }

        private bool IsRecurrenceModified = false;

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

        private void editorControl_SaveButtonClick(object sender, RoutedEventArgs e)
        {
            IsRecurrenceModified = false;
            var editor = sender as ScheduleAppointmentEditorControl;
            editor.IsStateChanged = false;
            editor.SaveButtonClick -= new RoutedEventHandler(editorControl_SaveButtonClick);
#if !SILVERLIGHT
            var childWindow = editor.FindParentElementOfType<Window>();
#else
            var childWindow = editor.FindParentElementOfType<ChildWindow>();
#endif
            if (childWindow != null)
            {
                SaveAppointments(editor);
                childWindow.Close();
            }
        }

        private void SaveAppointments(ScheduleAppointmentEditorControl editor)
        {
            var appWrapper = editor.DataContext as ScheduleAppointmentWrapper;
            var appProxy = appWrapper.Appointment;
            var newAppForRec = new ScheduleAppointment();
            newAppForRec.InitializeFrom(appProxy);
            newAppForRec.CurrentRecurrencePatternMode = editor.RecurrencePattern;
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

        #endregion

        #region Mouse Selection TimeSlots Implementation

        private void SetupTimeSlotEvents()
        {
            horizontaltimeLineItems.TimelineVisibility = this.TimelineVisibility;
            this.horizontaltimeSlotItems.MouseRightClick += new MouseButtonEventHandler(horizontaltimeSlotItems_MouseRightClick);
            this.horizontaltimeSlotItems.MouseLeftButtonDown += new MouseButtonEventHandler(timeSlotItems_MouseLeftButtonDown);
            this.horizontaltimeSlotItems.MouseMove += new MouseEventHandler(timeSlotItems_MouseMove);
            this.horizontaltimeSlotItems.MouseLeftButtonUp += new MouseButtonEventHandler(timeSlotItems_MouseLeftButtonUp);
            this.KeyDown += new KeyEventHandler(ScheduleDaysView_KeyDown);
            this.KeyUp += new KeyEventHandler(ScheduleDaysView_KeyUp);
        }

        void horizontaltimeSlotItems_MouseRightClick(object sender, MouseButtonEventArgs epnt)
        {
#if SILVERLIGHT
            Point currentObjPos = epnt.GetPosition(Application.Current.RootVisual);
            ScheduleHorizontalRectangleBorderExt e = VisualTreeHelper.FindElementsInHostCoordinates(currentObjPos, this).OfType<ScheduleHorizontalRectangleBorderExt>().FirstOrDefault();
#else
            Point currentObjPos = epnt.GetPosition(this);
            ScheduleHorizontalRectangleBorderExt e = GetHitTestParentElement(this.InputHitTest(currentObjPos), "ScheduleHorizontalRectangleBorderExt") as ScheduleHorizontalRectangleBorderExt;
#endif
            if (e == null) return;

            if (!e.IsSelected)
            {
                suspend = true;
                this.ClearTimeSlotSelection();
                this.ClearAppointmentLayoutSelectedItems();
                suspend = false;
                //isInMouseDown = true;
                e.IsSelected = true;
                mouseDownEl = e;
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
            }
        }

        private void ScheduleDaysView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
#if SILVERLIGHT
                foreach (ScheduleHorizontalTimeSlotControl timeSlot in this.horizontaltimeSlotItems.Items)
                {
                    foreach (var rect in timeSlot.FindElementsOfType<ScheduleHorizontalRectangleBorderExt>().Where(r => r != null && r.IsSelected))
                    {
                        rect.IsSelected = false;
                    }
                }
#endif
            }
#if SILVERLIGHT
            else if (e.Key == Key.Ctrl)
#else
            else if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
#endif
            {
            }
        }

        private bool isInMouseDown = false;
        private bool suspend = false;

        ScheduleHorizontalRectangleBorderExt mouseDownEl = null;
       

        private Popup GetPopUpWithSized(FrameworkElement element)
        {
            double popwidth = double.IsNaN(element.Width) ? element.ActualWidth : element.Width;
            double popheight = double.IsNaN(element.Height) ? element.ActualHeight : element.Height;
#if SILVERLIGHT
            var popup = new Popup()
            {
                IsOpen = true
            };
            popup.MouseMove += new MouseEventHandler(timeSlotItems_MouseMove);
            popup.MouseLeftButtonUp += new MouseButtonEventHandler(timeSlotItems_MouseLeftButtonUp);
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
            popup.MouseMove += new MouseEventHandler(timeSlotItems_MouseMove);
            popup.MouseLeftButtonUp += new MouseButtonEventHandler(timeSlotItems_MouseLeftButtonUp);
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

        private void timeSlotItems_MouseLeftButtonDown(object sender, MouseButtonEventArgs epnt)
        {
#if SILVERLIGHT
            Point currentObjPos = epnt.GetPosition(Application.Current.RootVisual);
            ScheduleHorizontalRectangleBorderExt e = VisualTreeHelper.FindElementsInHostCoordinates(currentObjPos, this).OfType<ScheduleHorizontalRectangleBorderExt>().FirstOrDefault();
#else
            Point currentObjPos = epnt.GetPosition(this);
            ScheduleHorizontalRectangleBorderExt e = GetHitTestParentElement(this.InputHitTest(currentObjPos), "ScheduleHorizontalRectangleBorderExt") as ScheduleHorizontalRectangleBorderExt;
#endif
            if (e == null) return;

            suspend = true;
            this.ClearTimeSlotSelection();
            this.ClearAppointmentLayoutSelectedItems();
            suspend = false;
            isInMouseDown = true;
            e.IsSelected = true;
            mouseDownEl = e;
        }

        private void timeSlotItems_MouseMove(object sender, MouseEventArgs epnt)
        {
            Point AppPosition = new Point();
#if SILVERLIGHT
            AppPosition = epnt.GetPosition(Application.Current.RootVisual);
#else
            AppPosition = epnt.GetPosition(this);
#endif
            if (!isInMouseDown) return;

            if (IsDisableMouseMove)
            {
                IsDisableMouseMove = false;
                DisableMouseSelectionUpEvent();
                return;
            }

#if SILVERLIGHT
            ScheduleHorizontalRectangleBorderExt el = VisualTreeHelper.FindElementsInHostCoordinates(AppPosition, this).OfType<ScheduleHorizontalRectangleBorderExt>().FirstOrDefault();
#else
            ScheduleHorizontalRectangleBorderExt el = GetHitTestParentElement(this.InputHitTest(AppPosition), "ScheduleHorizontalRectangleBorderExt") as ScheduleHorizontalRectangleBorderExt;
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
            this.ClearAppointmentPopupWindow();
#if !SILVERLIGHT
            var mouseUp = from evt in this.horizontaltimeSlotItems.FindElementsOfType<ScheduleHorizontalRectangleBorderExt>()
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

        private void ClearTimeSlotHeaderSelection()
        {
#if SILVERLIGHT
            var rs = this.horizontaltimeSlotItems.Items.OfType<ScheduleHorizontalTimeSlotControl>().Select(item => item.FindElementsOfType<ScheduleHorizontalRectangleSel>()).ToList();
            var elements = from rects in rs
                           from rect in rects.Where(r => r != null)
                           select rect;
            foreach (var rectangle in elements)
            {
                rectangle.IsSelected = false;
            }
#endif
        }

        private void ClearTimeSlotSelection()
        {
#if SILVERLIGHT
            var rs = this.horizontaltimeSlotItems.Items.OfType<ScheduleHorizontalTimeSlotControl>().Select(item => item.FindElementsOfType<ScheduleHorizontalRectangleBorderExt>()).ToList();
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
            var rs = this.horizontaltimeSlotItems.Items.OfType<ScheduleHorizontalTimeSlotControl>().Select(item => item.FindElementsOfType<ScheduleHorizontalRectangleBorderExt>()).ToList();
            var selectedRects = (from rects in rs
                                 from rect in rects.Where(r => r != null && r.IsSelected)
                                 select rect).ToList();
            if (selectedRects.Count == 0)
            {
                return;
            }
            var firstItem = selectedRects[0];
            var lastItem = selectedRects[selectedRects.Count - 1];
            // get the first parent
            var firstParentTimeSlot = firstItem.FindParentElementOfType<ScheduleHorizontalTimeSlotControl>();
            var firstRectList = firstParentTimeSlot.FindElementsOfType<ScheduleHorizontalRectangleBorderExt>().Where(r => r != null).ToList();
            // get the last parent
            var lastParentTimeSlot = lastItem.FindParentElementOfType<ScheduleHorizontalTimeSlotControl>();
            var lastRectList = lastParentTimeSlot.FindElementsOfType<ScheduleHorizontalRectangleBorderExt>().Where(r => r != null).ToList();
            int index = firstRectList.IndexOf(firstItem);
            int minutes = index * Convert.ToInt32((TimeIntervalToTimeSpanConverter(this.Model.CurrentTimeInterval).TotalMinutes));
            TimeSpan startTimeSpan = new TimeSpan(firstParentTimeSlot.Hour, minutes, 0);
            index = lastRectList.IndexOf(lastItem) + 1;
            minutes = index * Convert.ToInt32((TimeIntervalToTimeSpanConverter(this.Model.CurrentTimeInterval).TotalMinutes));
            TimeSpan endTimeSpan = new TimeSpan(lastParentTimeSlot.Hour, minutes, 0);
            this.Model.SelectedStartTimeSpan = firstParentTimeSlot.DateTime.AddTimeSpan(startTimeSpan);
            this.Model.SelectedEndTimeSpan =  lastParentTimeSlot.DateTime.AddTimeSpan(endTimeSpan);
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

        private TimeSpan ComputeTimeSpan(ScheduleHorizontalTimeSlotControl timeSlot)
        {
            var intervalCount = this.Model.GetTimeSlotIntervals();
            var rectList = timeSlot.FindElementsOfType<ScheduleHorizontalRectangleBorderExt>().Where(r => r != null && r.IsSelected).ToList();
            var time = new TimeSpan(timeSlot.Hour, 0, 0);
            var mins = 60 / intervalCount;
            for (int i = 0; i < rectList.Count; i++)
            {
                time = time.Add(new TimeSpan(0, mins, 0));
            }
            return time;
        }

        private void ExtendSelection(ScheduleHorizontalRectangleBorderExt el, ScheduleHorizontalRectangleBorderExt prevEl)
        {
#if SILVERLIGHT
            var currentTimeSlot = el.FindParentElementOfType<ScheduleHorizontalTimeSlotControl>();
            var usp = el.FindParentElementOfType<UniformStackPanel>();
            int index = 0, index1 = 0;
            if (usp != null)
                index = usp.Children.IndexOf(el);
            DateTime date = currentTimeSlot.DateTime.AddHours(currentTimeSlot.Hour);
            date = date.Add(new TimeSpan(0, this.TimeIntervalToTimeSpanConverter(this.Model.CurrentTimeInterval).Minutes * index, 0));
            var prevTimeSlot = prevEl.FindParentElementOfType<ScheduleHorizontalTimeSlotControl>();
            var usp1 = prevEl.FindParentElementOfType<UniformStackPanel>();
            if (usp1 != null)
                index1 = usp1.Children.IndexOf(prevEl);
            DateTime date1 = prevTimeSlot.DateTime.AddHours(prevTimeSlot.Hour);
            date1 = date1.Add(new TimeSpan(0, this.TimeIntervalToTimeSpanConverter(this.Model.CurrentTimeInterval).Minutes * index1, 0));
            
            if (date > date1)
            {
                var pIdx = this.horizontaltimeSlotItems.Items.IndexOf(prevTimeSlot);
                var cIdx = this.horizontaltimeSlotItems.Items.IndexOf(currentTimeSlot);
                this.SetSelected(pIdx, index1, cIdx, index);
            }
            else if (date < date1)
            {
                var pIdx = this.horizontaltimeSlotItems.Items.IndexOf(currentTimeSlot);
                var cIdx = this.horizontaltimeSlotItems.Items.IndexOf(prevTimeSlot);
                this.SetSelected(pIdx, index, cIdx, index1);
            }

            //var currentTimeSlot = el.FindParentElementOfType<ScheduleHorizontalTimeSlotControl>();
            //var prevTimeSlot = prevEl.FindParentElementOfType<ScheduleHorizontalTimeSlotControl>();
            //if (currentTimeSlot.DateTime.Date > prevTimeSlot.DateTime.Date)
            //{
            //    var pIdx = this.horizontaltimeSlotItems.Items.IndexOf(prevTimeSlot);
            //    var cIdx = this.horizontaltimeSlotItems.Items.IndexOf(currentTimeSlot);
            //    this.SetSelected(pIdx, cIdx);
            //}
            //else if (currentTimeSlot.DateTime < prevTimeSlot.DateTime.Date)
            //{
            //    var pIdx = this.horizontaltimeSlotItems.Items.IndexOf(currentTimeSlot);
            //    var cIdx = this.horizontaltimeSlotItems.Items.IndexOf(prevTimeSlot);
            //    this.SetSelected(pIdx, cIdx);
            //}
#endif
        }

        private void SetSelected(int pIdx, int pRectIdx, int cIdx, int cRectIdx)
        {
#if SILVERLIGHT
            ClearTimeSlotSelection();
            //MessageBox.Show(pIdx.ToString() + "\n" + pRectIdx.ToString() + "\n" + cIdx.ToString() + "\n" + cRectIdx.ToString());
            if (pIdx != cIdx)
            {
                var firstItem = (ScheduleHorizontalTimeSlotControl)this.horizontaltimeSlotItems.Items[pIdx];
                var rects = firstItem.FindElementsOfType<ScheduleHorizontalRectangleBorderExt>().Where(r => r != null);
                for (int i = pRectIdx; i < rects.Count(); i++)
                {

                    rects.ElementAt(i).IsSelected = true;
                }
                for (int i = pIdx + 1; i < cIdx; i++)
                {

                var item = (ScheduleHorizontalTimeSlotControl)this.horizontaltimeSlotItems.Items[i];
                    foreach (var rect in item.FindElementsOfType<ScheduleHorizontalRectangleBorderExt>().Where(r => r != null))
                {
                    rect.IsSelected = true;
                }
            }
                var lastItem = (ScheduleHorizontalTimeSlotControl)this.horizontaltimeSlotItems.Items[cIdx];
                var rects1 = lastItem.FindElementsOfType<ScheduleHorizontalRectangleBorderExt>().Where(r => r != null);
                for (int i = 0; i <= cRectIdx; i++)
                {

                    rects1.ElementAt(i).IsSelected = true;
                }
            }
            else
            {
                var firstItem = (ScheduleHorizontalTimeSlotControl)this.horizontaltimeSlotItems.Items[pIdx];
                var rects = firstItem.FindElementsOfType<ScheduleHorizontalRectangleBorderExt>().Where(r => r != null);
                for (int i = pRectIdx; i <= cRectIdx; i++)
                {

                    rects.ElementAt(i).IsSelected = true;
                }
            }
#endif
        }

        //private void SetSelected(int pIdx, int cIdx)
        //{
        //    for (int i = pIdx; i < cIdx; i++)
        //    {
        //        var item = (ScheduleHorizontalTimeSlotControl)this.horizontaltimeSlotItems.Items[i];
        //        foreach (var rect in item.FindElementsOfType<ScheduleHorizontalRectangleBorderExt>().Where(r => r != null))
        //        {
        //            rect.IsSelected = true;
        //        }
        //    }
        //}

        #endregion

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
            this.horizontaltimeLineItems.Items.Clear();
            this.horizontaldaysHeaderViewItems.Items.Clear();
#if SILVERLIGHT
            this.horizontaltimeSlotItems.Items.Clear();
#endif
            this.horizontalAppointmentsLayoutItems.Items.Clear();

            this.horizontalAppointmentsLayoutItems.MouseLeftButtonDown -= new MouseButtonEventHandler(appointmentsLayoutItems_MouseLeftButtonDown);
            this.horizontalAppointmentsLayoutItems.MouseMove -= new MouseEventHandler(appointmentsLayoutItems_MouseMove);
            this.horizontalAppointmentsLayoutItems.MouseLeftButtonUp -= new MouseButtonEventHandler(appointmentsLayoutItems_MouseLeftButtonUp);
            this.MouseMove -= new MouseEventHandler(ScheduleDaysView_MouseMove);
#if !SILVERLIGHT
            this.DragPopUp.MouseLeftButtonUp -= new MouseButtonEventHandler(appointmentsLayoutItems_MouseLeftButtonUp);
            this.DragPopUp.MouseMove -= new MouseEventHandler(ScheduleDaysView_MouseMove);
#endif
            this.model.PropertyChanged -= new PropertyChangedEventHandler(model_PropertyChanged);
            this.horizontaltimeLineItems.MouseLeftButtonDown -= new MouseButtonEventHandler(timeSlotItems_MouseLeftButtonDown);
            this.horizontaltimeLineItems.MouseMove -= new MouseEventHandler(timeSlotItems_MouseMove);
            this.horizontaltimeLineItems.MouseLeftButtonUp -= new MouseButtonEventHandler(timeSlotItems_MouseLeftButtonUp);
            this.KeyDown -= new KeyEventHandler(ScheduleDaysView_KeyDown);
            this.KeyUp -= new KeyEventHandler(ScheduleDaysView_KeyUp);
            this.Model.Appointments.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnAppointmentsCollectionChanged);
            this.horizontaltimeSlotItems.MouseDoubleClick -= new MouseButtonEventHandler(timeSlotItems_MouseDoubleClick);
            this.horizontalAppointmentsLayoutItems.MouseDoubleClick -= new MouseButtonEventHandler(appointmentsLayoutItems_MouseDoubleClick);
        }

        #endregion
    }
}
