#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
#if WINRT
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    #region AllDayAppointmentItemscontrol

    /// <summary>
    /// Represents an items control for arranging all day appointment collection.
    /// </summary>
    public class AllDayAppointmentItemscontrol : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.AllDayAppointmentItemscontrol">AllDayAppointmentItemscontrol</see> class.
        /// </summary>
        public AllDayAppointmentItemscontrol()
        {
            DefaultStyleKey = typeof(AllDayAppointmentItemscontrol);
            ChildCollection = new List<ScheduleAppointment>();
        }

        #endregion

        #region Private Fields

        internal ScrollViewer AllDayScrollView;

        #endregion

        #region Dependency Properties

        #region AppointmentTemplate
        /// <summary>
        /// Gets or sets the template for schedule appointment.
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
            DependencyProperty.Register("AppointmentTemplate", typeof(DataTemplate), typeof(AllDayAppointmentItemscontrol), new PropertyMetadata(null));
        #endregion

        #region ChildCollection
        /// <summary>
        /// Gets or sets the list of schedule appointment.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleAppointment"></seealso>
        public List<ScheduleAppointment> ChildCollection
        {
            get { return (List<ScheduleAppointment>)GetValue(ChildCollectionProperty); }
            set { SetValue(ChildCollectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ChildCollection.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ChildCollectionProperty =
            DependencyProperty.Register("ChildCollection", typeof(List<ScheduleAppointment>), typeof(AllDayAppointmentItemscontrol), new PropertyMetadata(null, ChildCollectionChaged));

        private static void ChildCollectionChaged(DependencyObject dpo, DependencyPropertyChangedEventArgs arg)
        {
            var alldayitemcontrol = dpo as AllDayAppointmentItemscontrol;
            if (alldayitemcontrol != null && alldayitemcontrol.ChildCollection != null)
            {
                alldayitemcontrol.AddLimitedChild();
            }
        }
        #endregion

        #region AppointmentSelectionBrush
        internal Brush AppointmentSelectionBrush
        {
            get { return (Brush)GetValue(AppointmentSelectionBrushProperty); }
            set { SetValue(AppointmentSelectionBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentSelectionBrush.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(AllDayAppointmentItemscontrol), new PropertyMetadata(null));
        #endregion


        #region DayViewVerticaLineStroke
        public Brush DayViewVerticaLineStroke
        {
            get { return (Brush)GetValue(DayViewVerticaLineStrokeProperty); }
            set { SetValue(DayViewVerticaLineStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayViewVerticaLineStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayViewVerticaLineStrokeProperty =
            DependencyProperty.Register("DayViewVerticaLineStroke", typeof(Brush), typeof(AllDayAppointmentItemscontrol), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

#if !WINRT
        #region AppointmentTooltipVisibility
        internal Visibility AppointmentTooltipVisibility
        {
            get { return (Visibility)GetValue(AppointmentTooltipVisibilityProperty); }
            set { SetValue(AppointmentTooltipVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentTooltipVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentTooltipVisibilityProperty =
            DependencyProperty.Register("AppointmentTooltipVisibility", typeof(Visibility), typeof(AllDayAppointmentItemscontrol), new PropertyMetadata(Visibility.Collapsed)); 
        #endregion

        #region AppointmentToolTipTemplate
        internal ControlTemplate AppointmentToolTipTemplate
        {
            get { return (ControlTemplate)GetValue(AppointmentToolTipTemplateProperty); }
            set { SetValue(AppointmentToolTipTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentToolTipTemplate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentToolTipTemplateProperty =
            DependencyProperty.Register("AppointmentToolTipTemplate", typeof(ControlTemplate), typeof(AllDayAppointmentItemscontrol), new PropertyMetadata(null));
        #endregion
#endif

        #endregion

        #region Methods

        #region Add Limited Child

        private void AddLimitedChild()
        {
            foreach (ScheduleAppointment scheduleAppointment in ChildCollection)
            {
                if (Items != null && Items.Count < 3)
                {
                    if (!Items.Contains(scheduleAppointment))
                        Items.Add(scheduleAppointment);
                }
                else if (Items != null && Items.Count == 3)
                {
                    var coll_app = new CollapsedScheduleAppointment { AppointmentBackground = new SolidColorBrush(Colors.Green), EventCount = (ChildCollection.Count - 3) + " More", ReadOnlyVisibility = Visibility.Collapsed };

                    Items.Add(coll_app);
                }
                else
                {
                    break;
                }
            }
        }



        #endregion

        #region AddAllChild

        private void AddAllChild()
        {
            if (Items != null)
            {
                Items.Clear();
                foreach (ScheduleAppointment scheduleappointment in ChildCollection)
                {
                    if (!Items.Contains(scheduleappointment))
                        Items.Add(scheduleappointment);
                }
            }
            AllDayScrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        #endregion

        #region PointerPressed

#if WINRT
        void ScheduleDaysAppointmentViewControl_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#else
        void sh_app_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
#endif
        {
            if((sender as FrameworkElement).DataContext.GetType()== typeof(CollapsedScheduleAppointment))
            AddAllChild();
        }

        #endregion

        #endregion

        #region Overrides

#if !WINRT
       

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            SfSchedule schedule = this.FindParentElementOfType<SfSchedule>();   
#if WPF
            schedule.currentAllDaySelectedItem = this;
            schedule.currentAllDaySelectedItem.Background = new SolidColorBrush(Colors.LightGray);
#else
            if (schedule.currentAllDaySelectedItem != null)
            {

                        schedule.currentAllDaySelectedItem.Background = new SolidColorBrush(Colors.Transparent);
                        schedule.currentAllDaySelectedItem = null;
            }
            schedule.currentAllDaySelectedItem = this;
#endif
        }

#endif

        #region GetContainerForItemOverride

        protected override DependencyObject GetContainerForItemOverride()
        {
            var view = new ScheduleDaysAppointmentViewControl();
            var appointmentbrushbinding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("AppointmentSelectionBrush")
                };
            view.SetBinding(ScheduleDaysAppointmentViewControl.AppointmentSelectionBrushProperty, appointmentbrushbinding);
            if (AppointmentTemplate != null)
            {
                var appointmentTemplateBinding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("AppointmentTemplate")
                };
                view.SetBinding(ScheduleDaysAppointmentViewControl.AppointmentTemplateProperty, appointmentTemplateBinding);
                view.CustomTemplateVisibility = Visibility.Visible;
                view.DefaultTemplateVisibility = Visibility.Collapsed;
            }
            else
            {
                view.CustomTemplateVisibility = Visibility.Collapsed;
                view.DefaultTemplateVisibility = Visibility.Visible;
            }
#if !WINRT
            var appointmentToolTipTemplateBinding = new Binding
                {
                    Path = new PropertyPath("AppointmentToolTipTemplate"),
                    Source = this
                };
            view.SetBinding(ScheduleDaysAppointmentViewControl.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);

            var appointmentTooltipVisibilityBinding = new Binding
        {
            Path = new PropertyPath("AppointmentTooltipVisibility"),
            Source = this
        };
            view.SetBinding(ScheduleDaysAppointmentViewControl.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
#endif
#if WINRT
            view.PointerPressed += ScheduleDaysAppointmentViewControl_PointerPressed;
#else
                    view.MouseLeftButtonDown += sh_app_MouseLeftButtonDown;
#endif
            view.Height = 30;
            view.AppWidth = Width;

            return view;
        }

        #endregion

        #region IsItemItsOwnContainerOverride

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ScheduleDaysAppointmentViewControl);
        }

        #endregion

        #region OnApplyTemplate

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            AllDayScrollView = GetTemplateChild("AllDayScrollViewer") as ScrollViewer;
            if (AllDayScrollView != null) AllDayScrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }

        #endregion

        #endregion
    }

    #endregion

    #region CollapsedScheduleAppointment

    /// <summary>
    /// Represents a schedule appointment which is collapsed.
    /// </summary>
    public class CollapsedScheduleAppointment
    {
        #region CLR Properties

        public Visibility ReadOnlyVisibility { get; set; }
        public Brush AppointmentSelectionBrush { get; set; }
        public Brush AppointmentBackground { get; set; }
        public string EventCount { get; set; }
        public DateTime CorrespondingDate { get; set; }
        public ScheduleAppointmentStatus Status { get; set; }
        public string Subject { get; set; }
        public bool AllDay { get; set; }

        #endregion
    }

    #endregion
}
