#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Collections;
using Syncfusion.UI.Xaml.Controls.Navigation;
using System;
#else
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Collections;
using System.Windows.Input;
#if WPF
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using Syncfusion.Windows.Controls.Navigation;
using System.Linq;
using System;
using System.Data;
#elif SILVERLIGHT
using Syncfusion.Tools.Controls.Navigation;
using System;
#endif
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a appointment in month view.
    /// </summary>
    public class ScheduleMonthAppointmentViewControl : Control, IDisposable
    {
        #region Contructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleMonthAppointmentViewControl">ScheduleMonthAppointmentViewControl</see>
        /// class.
        /// </summary>
        public ScheduleMonthAppointmentViewControl()
        {
            DefaultStyleKey = typeof(ScheduleMonthAppointmentViewControl);

            Loaded += ScheduleMonthAppointmentViewControl_Loaded;
#if WINRT
            PointerPressed += ScheduleMonthAppointmentViewControl_PointerPressed;
            tooltiptimer = new DispatcherTimer();
            tooltiptimer.Tick += tooltiptimer_Tick;
#endif
        }

        #endregion

        #region Private Fields

#if WINRT
        readonly DispatcherTimer tooltiptimer;
        bool IsEntered;
        bool enablePopup;
#endif
        SfSchedule schedule;
        bool IsPressed;
        bool IsDragDropItem;
        internal bool IsSameAppointment = false;

        #endregion

        #region Dependency Properties

        #region Public Properties

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
            DependencyProperty.Register("AppointmentTemplate", typeof(DataTemplate), typeof(ScheduleMonthAppointmentViewControl), new PropertyMetadata(null));
        #endregion

        #region CustomTemplateDataContext
        /// <summary>
        /// Gets or sets the data context for custom appointment in month view.
        /// </summary>
        public object CustomTemplateDataContext
        {
            get { return GetValue(CustomTemplateDataContextProperty); }
            set { SetValue(CustomTemplateDataContextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CustomTemplateDataContext.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CustomTemplateDataContextProperty =
            DependencyProperty.Register("CustomTemplateDataContext", typeof(object), typeof(ScheduleMonthAppointmentViewControl), new PropertyMetadata(null));
        #endregion

        #region CustomTemplateVisibility
        /// <summary>
        /// Gets or sets the visibility of custom appointment in month view.
        /// </summary>
        public Visibility CustomTemplateVisibility
        {
            get { return (Visibility)GetValue(CustomTemplateVisibilityProperty); }
            set { SetValue(CustomTemplateVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CustomTemplateVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CustomTemplateVisibilityProperty =
            DependencyProperty.Register("CustomTemplateVisibility", typeof(Visibility), typeof(ScheduleMonthAppointmentViewControl), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region DefaultTemplateVisibility
        /// <summary>
        /// Gets or sets the visibility of default appointment in month view.
        /// </summary>
        public Visibility DefaultTemplateVisibility
        {
            get { return (Visibility)GetValue(DefaultTemplateVisibilityProperty); }
            set { SetValue(DefaultTemplateVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DefaultTemplateVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DefaultTemplateVisibilityProperty =
            DependencyProperty.Register("DefaultTemplateVisibility", typeof(Visibility), typeof(ScheduleMonthAppointmentViewControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region EventCount
        /// <summary>
        /// Gets or sets the count of events in a month view item.
        /// </summary>
        public string EventCount
        {
            get { return (string)GetValue(EventCountProperty); }
            set { SetValue(EventCountProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EventCount.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EventCountProperty =
            DependencyProperty.Register("EventCount", typeof(string), typeof(ScheduleMonthAppointmentViewControl), new PropertyMetadata(string.Empty));
        #endregion

        #region TextForeground
        /// <summary>
        /// Gets or sets the foreground for specifying number of events in a month view item.
        /// </summary>
        public Brush TextForeground
        {
            get { return (Brush)GetValue(TextForegroundProperty); }
            set { SetValue(TextForegroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TextForeground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextForegroundProperty =
            DependencyProperty.Register("TextForeground", typeof(Brush), typeof(ScheduleMonthAppointmentViewControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

#if !WINRT
        #region AppointmentTooltipVisibility
        /// <summary>
        /// Gets or sets the visibility of appointment's tooltip.
        /// </summary>
		public Visibility AppointmentTooltipVisibility
        {
            get { return (Visibility)GetValue(AppointmentTooltipVisibilityProperty); }
            set { SetValue(AppointmentTooltipVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentTooltipVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentTooltipVisibilityProperty =
            DependencyProperty.Register("AppointmentTooltipVisibility", typeof(Visibility), typeof(ScheduleMonthAppointmentViewControl), new PropertyMetadata(Visibility.Collapsed)); 
        #endregion

        #region AppointmentToolTipTemplate
        /// <summary>
        /// Gets or sets the template for customizing appointment's tooltip.
        /// </summary>
        public ControlTemplate AppointmentToolTipTemplate
        {
            get { return (ControlTemplate)GetValue(AppointmentToolTipTemplateProperty); }
            set { SetValue(AppointmentToolTipTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentToolTipTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentToolTipTemplateProperty =
            DependencyProperty.Register("AppointmentToolTipTemplate", typeof(ControlTemplate), typeof(ScheduleMonthAppointmentViewControl), new PropertyMetadata(null));
        #endregion
#endif
        
        #endregion

        #region Internal Properties

        #region AppointmentSelectionBrush
        internal Brush AppointmentSelectionBrush
        {
            get { return (Brush)GetValue(AppointmentSelectionBrushProperty); }
            set { SetValue(AppointmentSelectionBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentSelectionBrush.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(ScheduleMonthAppointmentViewControl), new PropertyMetadata(null));
        #endregion

        #region AppointmentWidth
        internal double AppWidth
        {
            get { return (double)GetValue(AppWidthProperty); }
            set { SetValue(AppWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppWidthProperty =
            DependencyProperty.Register("AppWidth", typeof(double), typeof(ScheduleMonthAppointmentViewControl), new PropertyMetadata(100d));
        #endregion

        #region DragRectangleVisibility
        internal Visibility DragRectangleVisibility
        {
            get { return (Visibility)GetValue(DragRectangleVisibilityProperty); }
            set { SetValue(DragRectangleVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragRectangleVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DragRectangleVisibilityProperty =
            DependencyProperty.Register("DragRectangleVisibility", typeof(Visibility), typeof(ScheduleMonthAppointmentViewControl), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #endregion

        #endregion

        #region Events

        #region Control Loaded

        void ScheduleMonthAppointmentViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            schedule = this.FindParentElementOfType<SfSchedule>();
            IsDragDropItem = this.FindParentElementOfType<Canvas>() != null;
            if (schedule != null && schedule.ItemsSource != null && schedule.AppointmentMapping != null && DataContext is ScheduleAppointment)
            {
                double id = (DataContext as ScheduleAppointment).ObjectID;
                if (schedule.ItemsSource is IEnumerable)
                {
                    foreach (var item in (schedule.ItemsSource as IEnumerable))
                    {
                        if (item.GetHashCode() == (int)id)
                        {
                            CustomTemplateDataContext = item;
                            break;
                        }
                    }
                }
                if (CustomTemplateDataContext == null)
                {
                    CustomTemplateDataContext = DataContext;
                }
            }
            else
            {
                CustomTemplateDataContext = DataContext;
            }
        }

        #endregion

#if WINRT
        void tooltiptimer_Tick(object sender, object e)
        {
            if (schedule != null)
            {
                if (schedule.AppointmentTooltipVisibility == Visibility.Visible)
                {
                    if (schedule.editpopup.IsOpen || schedule.addnewpopup.IsOpen)
                        schedule.InternalAppTooltipVisibility = Visibility.Collapsed;
                    else
                        schedule.InternalAppTooltipVisibility = Visibility.Visible;
                }
                IsEntered = false;
            }
        }

        void ScheduleMonthAppointmentViewControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var sfSchedule = this.FindParentElementOfType<SfSchedule>();
            Popup popup = sfSchedule.popup;
            if (popup != null)
            {
                var appointmentViewControl = sender as ScheduleMonthAppointmentViewControl;
                if (appointmentViewControl != null)
                {
                    GeneralTransform gt = appointmentViewControl.TransformToVisual(sfSchedule);
                    GeneralTransform gt1 = appointmentViewControl.TransformToVisual(appointmentViewControl);
                    PointerPoint pointerPoint = e.GetCurrentPoint(appointmentViewControl);

                    Point screenPoint = gt.TransformPoint(new Point(pointerPoint.Position.X, pointerPoint.Position.Y));
                    Point screenPoint1 = gt1.TransformPoint(new Point(pointerPoint.Position.X, pointerPoint.Position.Y));
                    double centerWidth = (appointmentViewControl.ActualWidth - popup.Width) / 2;
                    double x = screenPoint.X - screenPoint1.X + centerWidth;
                    double y = popup.VerticalOffset = screenPoint.Y - screenPoint1.Y - popup.Height - 4; // 4 represents the border thickness of border inside the popup

                    if (x + popup.Width > sfSchedule.ActualWidth)
                        x = sfSchedule.ActualWidth - popup.Width;
                    else if (x < 0)
                        x = 0;
                    if (y + popup.Height > sfSchedule.ActualHeight)
                        y = sfSchedule.ActualHeight - popup.Height;
                    else if (y < 0)
                        y = 0;

                    popup.HorizontalOffset = x;
                    popup.VerticalOffset = y;
                }
            }
        }
#endif
        #endregion

        #region Overrides

#if WINRT
        #region PointerPressed

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (schedule == null)
            {
                schedule = this.FindParentElementOfType<SfSchedule>();
            }
            else
            {
                if (schedule.SelectedAppointment != null)
                {
                    if (DataContext == schedule.SelectedAppointment && !schedule.isAppointment)
                    {
                        IsSameAppointment = true;
                    }
                    else
                    {
                        IsSameAppointment = false;
                    }
                }
                tooltiptimer.Stop();
                schedule.InternalAppTooltipVisibility = Visibility.Collapsed;

            }
            if (!IsDragDropItem)
            {
                IsPressed = true;
            }
            if (schedule != null)
            {
                PointerPoint ptrPt = e.GetCurrentPoint(schedule);
                enablePopup = (ptrPt.Properties.IsLeftButtonPressed);
            }

            base.OnPointerPressed(e);
        }

        #endregion

        #region Pointer Released

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (schedule == null)
            {
                schedule = this.FindParentElementOfType<SfSchedule>();
            }
            if (schedule != null && IsPressed)
            {
                if (enablePopup && !schedule.isContextMenuAltered)
                {
                    if (!IsSameAppointment)
                    {
                        schedule.editpopup.IsOpen = true;
                        if (schedule.contextMenuOpeningEventArgs != null)
                        {
                            schedule.GetContextMenuOpeningEvent(schedule.contextMenuOpeningEventArgs);
                        }

                        schedule.editpopup.Child.UpdateLayout();
                        if (schedule.ContextMenuType == MenuType.Default)
                        {
                            var dragDropControl = schedule.editpopup.Child as DragDropControl;
                            if (dragDropControl != null)
                                dragDropControl.Story.Begin();
                        }
                        else
                        {
                            var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                            if (editRadialMenuControl != null)
                                editRadialMenuControl.RadialMenu.IsOpen = true;
                        }
                        schedule.addnewpopup.IsOpen = false;
                    }
                    if (!schedule.editpopup.IsOpen)
                    {
                        schedule.editpopup.IsOpen = true;
                        schedule.editpopup.Child.UpdateLayout();
                        if (schedule.ContextMenuType == MenuType.Default)
                        {
                            var dragDropControl = schedule.editpopup.Child as DragDropControl;
                            if (dragDropControl != null)
                                dragDropControl.Story.Begin();
                        }
                        else
                        {
                            var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                            if (editRadialMenuControl != null)
                                editRadialMenuControl.RadialMenu.IsOpen = true;
                        }
                        schedule.addnewpopup.IsOpen = false;
                    }
                    if (e.GetCurrentPoint(schedule).Position.X > schedule.ActualWidth / 2)
                    {
                        if (schedule.ContextMenuType == MenuType.Default)
                        {
                            var dragDropControl = schedule.editpopup.Child as DragDropControl;
                            if (dragDropControl != null)
                            {
                                dragDropControl.FlowDirection = FlowDirection.RightToLeft;
                                dragDropControl.ButtonFlowDirection = FlowDirection.LeftToRight;
                            }
                            schedule.editpopup.HorizontalOffset = e.GetCurrentPoint(schedule).Position.X - 150;
                        }
                        else
                        {
                            schedule.editpopup.HorizontalOffset = e.GetCurrentPoint(schedule).Position.X - 200;

                        }
                    }
                    else
                    {
                        schedule.editpopup.HorizontalOffset = e.GetCurrentPoint(schedule).Position.X + 50;
                        if (schedule.ContextMenuType == MenuType.Default)
                        {
                            var dragDropControl = schedule.editpopup.Child as DragDropControl;
                            if (dragDropControl != null)
                                dragDropControl.FlowDirection = FlowDirection.LeftToRight;
                        }
                    }
                    if (schedule.ContextMenuType == MenuType.Default)
                    {
                        if (e.GetCurrentPoint(schedule).Position.Y > schedule.ActualHeight - 75)
                        {
                            schedule.editpopup.VerticalOffset = e.GetCurrentPoint(schedule).Position.Y - 140;
                        }
                        else
                        {
                            schedule.editpopup.VerticalOffset = e.GetCurrentPoint(schedule).Position.Y - 60;
                        }
                    }
                    else
                    {
                        if (e.GetCurrentPoint(schedule).Position.Y > schedule.ActualHeight - 300)
                        {
                            schedule.editpopup.VerticalOffset = e.GetCurrentPoint(schedule).Position.Y - 200;
                        }
                        else if (e.GetCurrentPoint(schedule).Position.Y < 300)
                        {
                            schedule.editpopup.VerticalOffset = e.GetCurrentPoint(schedule).Position.Y;
                        }
                        else
                        {
                            schedule.editpopup.VerticalOffset = e.GetCurrentPoint(schedule).Position.Y - 100;
                        }
                    }
                    if (DataContext is ScheduleAppointment && (DataContext as ScheduleAppointment).ReadOnly)
                    {
                        if (schedule.ContextMenuType == MenuType.Default)
                        {
                            var dragDropControl = schedule.editpopup.Child as DragDropControl;
                            if (dragDropControl != null)
                            {
                                dragDropControl.Resize.Visibility = Visibility.Collapsed;
                                dragDropControl.Delete.Visibility = Visibility.Collapsed;
                            }
                        }
                        else
                        {
                            var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                            if (editRadialMenuControl != null && editRadialMenuControl.RadialMenu.Items != null)
                            {
                                if (editRadialMenuControl.RadialMenu.Items[5] is SfRadialMenuItem)
                                    (editRadialMenuControl.RadialMenu.Items[5] as SfRadialMenuItem).IsEnabled = false;
                                if (editRadialMenuControl.RadialMenu.Items[4] is SfRadialMenuItem)
                                    (editRadialMenuControl.RadialMenu.Items[4] as SfRadialMenuItem).IsEnabled = false;
                            }
                        }
                    }
                    else
                    {
                        if (schedule.ContextMenuType == MenuType.Default)
                        {
                            var dragDropControl = schedule.editpopup.Child as DragDropControl;
                            if (dragDropControl != null)
                            {
                                dragDropControl.Resize.Visibility = Visibility.Visible;
                                dragDropControl.Delete.Visibility = Visibility.Visible;
                            }
                        }
                        else
                        {
                            var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                            if (editRadialMenuControl != null && editRadialMenuControl.RadialMenu.Items != null)
                            {
                                if (editRadialMenuControl.RadialMenu.Items[5] is SfRadialMenuItem)
                                    (editRadialMenuControl.RadialMenu.Items[5] as SfRadialMenuItem).IsEnabled = true;
                                if (editRadialMenuControl.RadialMenu.Items[4] is SfRadialMenuItem)
                                    (editRadialMenuControl.RadialMenu.Items[4] as SfRadialMenuItem).IsEnabled = true;
                            }
                        }
                    }
                    if (DataContext is ScheduleAppointment && (DataContext as ScheduleAppointment).IsRecursive)
                    {
                        if (schedule.ContextMenuType == MenuType.Default)
                        {
                            var dragDropControl = schedule.editpopup.Child as DragDropControl;
                            if (dragDropControl != null)
                                dragDropControl.Copy.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                            if (editRadialMenuControl != null && editRadialMenuControl.RadialMenu.Items != null && editRadialMenuControl.RadialMenu.Items[2] is SfRadialMenuItem)
                                (editRadialMenuControl.RadialMenu.Items[2] as SfRadialMenuItem).IsEnabled = false;
                        }
                    }
                    else
                    {
                        if (schedule.ContextMenuType == MenuType.Default)
                        {
                            var dragDropControl = schedule.editpopup.Child as DragDropControl;
                            if (dragDropControl != null)
                                dragDropControl.Copy.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                            if (editRadialMenuControl != null && editRadialMenuControl.RadialMenu.Items != null && editRadialMenuControl.RadialMenu.Items[2] is SfRadialMenuItem)
                                (editRadialMenuControl.RadialMenu.Items[2] as SfRadialMenuItem).IsEnabled = true;
                        }
                    }
                    if (!schedule.AllowEditing)
                    {
                        if (schedule.ContextMenuType == MenuType.Default)
                        {
                            var dragDropControl = schedule.editpopup.Child as DragDropControl;
                            if (dragDropControl != null)
                            {
                                dragDropControl.Edit.Visibility = Visibility.Collapsed;
                                dragDropControl.AddNew.Visibility = Visibility.Collapsed;
                            }
                        }
                        else
                        {
                            var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                            if (editRadialMenuControl != null && editRadialMenuControl.RadialMenu.Items != null)
                            {
                                if (editRadialMenuControl.RadialMenu.Items[1] is SfRadialMenuItem)
                                    (editRadialMenuControl.RadialMenu.Items[1] as SfRadialMenuItem).IsEnabled = false;
                                if (editRadialMenuControl.RadialMenu.Items[0] is SfRadialMenuItem)
                                    (editRadialMenuControl.RadialMenu.Items[0] as SfRadialMenuItem).IsEnabled = false;
                            }
                        }
                    }
                    else
                    {
                        if (schedule.ContextMenuType == MenuType.Default)
                        {
                            var dragDropControl = schedule.editpopup.Child as DragDropControl;
                            if (dragDropControl != null)
                            {
                                dragDropControl.Edit.Visibility = Visibility.Visible;
                                dragDropControl.AddNew.Visibility = Visibility.Visible;
                            }
                        }
                        else
                        {
                            var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                            if (editRadialMenuControl != null && editRadialMenuControl.RadialMenu.Items != null)
                            {
                                if (editRadialMenuControl.RadialMenu.Items[1] is SfRadialMenuItem)
                                    (editRadialMenuControl.RadialMenu.Items[1] as SfRadialMenuItem).IsEnabled = true;
                                if (editRadialMenuControl.RadialMenu.Items[0] is SfRadialMenuItem)
                                    (editRadialMenuControl.RadialMenu.Items[0] as SfRadialMenuItem).IsEnabled = true;
                            }
                        }
                    }
                    if (schedule != null && schedule.editpopup != null && schedule.editpopup.Child is DragDropControl)
                    {
                        DragDropControl dragDropControl = schedule.editpopup.Child as DragDropControl;
                        if (!schedule.AllowEditing || (DataContext is ScheduleAppointment &&
                            (DataContext as ScheduleAppointment).ReadOnly || (DataContext as ScheduleAppointment).IsRecursive))
                        {
                            dragDropControl.bgPath.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            dragDropControl.bgPath.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    schedule.editpopup.IsOpen = false;
                    schedule.addnewpopup.IsOpen = false;
                }
            }

            if (!IsDragDropItem)
            {
                if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                {
                    var viewer = this.FindParentElementOfType<ScrollViewer>();
                    viewer.VerticalScrollMode = ScrollMode.Enabled;
                }
                else
                {
                    IsPressed = false;
                }
            }
            base.OnPointerReleased(e);
        }

        #endregion

        #region PointerMoved

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (schedule == null)
            {
                schedule = this.FindParentElementOfType<SfSchedule>();
            }
            if (!IsDragDropItem)
            {
                if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                {
                    var viewer = this.FindParentElementOfType<ScrollViewer>();
                    viewer.VerticalScrollMode = ScrollMode.Enabled;                    
                }
                else if (IsPressed)
                {
                    IsPressed = false;
                }
            }
            if (schedule != null && schedule.AppointmentTooltipTemplate != null && IsEntered)
            {
                Point position = e.GetCurrentPoint(schedule).Position;
                if (position.X > schedule.ActualWidth / 2)
                {
                    schedule.AppointmentTooltip.UpdateLayout();
                    schedule.AppointmentTooltip.Measure(schedule.RenderSize);
                    position.X = position.X - schedule.AppointmentTooltip.DesiredSize.Width - 20;
                    if (position.Y > schedule.ActualHeight / 2)
                    {
                        position.Y = position.Y - schedule.AppointmentTooltip.ActualHeight;
                    }

                }
                else if (position.Y > schedule.ActualHeight / 2)
                {
                    schedule.AppointmentTooltip.UpdateLayout();
                    schedule.AppointmentTooltip.Measure(schedule.RenderSize);
                    position.Y = position.Y - schedule.AppointmentTooltip.ActualHeight;
                    if (position.X < schedule.ActualWidth / 2)
                    {
                        position.X = position.X + 20;
                    }
                }
                else
                {
                    position.X = position.X + 20;
                }
                schedule.ToolTipMargin = new Thickness(position.X, position.Y, 0, 0);
            }


            base.OnPointerMoved(e);
        }

        #endregion

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                var sfSchedule = this.FindParentElementOfType<SfSchedule>();
                if (sfSchedule != null && e.OriginalSource is FrameworkElement)
                {
                    var app = (e.OriginalSource as FrameworkElement).DataContext as ScheduleAppointment;
                    if (app != null && !sfSchedule.IsDragEnabled)
                    {
                        tooltiptimer.Interval = new System.TimeSpan(0, 0, 1);
                        tooltiptimer.Start();
                        sfSchedule.AppointmentTooltip.DataContext = app;
                        IsEntered = true;
                    }
                }
            }
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            var sfSchedule = this.FindParentElementOfType<SfSchedule>();
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                tooltiptimer.Stop();
                if (sfSchedule != null)
                {
                    sfSchedule.InternalAppTooltipVisibility = Visibility.Collapsed;
                }
            }
            base.OnPointerExited(e);
        }

        #region Holding

        protected override void OnHolding(HoldingRoutedEventArgs e)
        {
            if (schedule != null)
            {
                var frameworkElement = e.OriginalSource as FrameworkElement;
                if (frameworkElement != null)
                {
                    var app = frameworkElement.DataContext as ScheduleAppointment;
                    if (app != null && schedule.AppointmentTooltipTemplate != null)
                    {

                        schedule.AppointmentTooltip.DataContext = app;
                        Point position = e.GetPosition(schedule);
                        if (position.X > schedule.ActualWidth / 2)
                        {
                            schedule.AppointmentTooltip.UpdateLayout();
                            schedule.AppointmentTooltip.Measure(schedule.RenderSize);
                            position.X = position.X - schedule.AppointmentTooltip.DesiredSize.Width - 20;
                            if (position.Y > schedule.ActualHeight / 2)
                            {
                                position.Y = position.Y - schedule.AppointmentTooltip.ActualHeight;
                            }

                        }
                        else if (position.Y > schedule.ActualHeight / 2)
                        {
                            schedule.AppointmentTooltip.UpdateLayout();
                            schedule.AppointmentTooltip.Measure(schedule.RenderSize);
                            position.Y = position.Y - schedule.AppointmentTooltip.ActualHeight;
                            if (position.X < schedule.ActualWidth / 2)
                            {
                                position.X = position.X + 20;
                            }
                        }
                        else
                        {
                            position.X = position.X + 20;
                        }


                        schedule.ToolTipMargin = new Thickness(position.X, position.Y, 0, 0);
                        if (schedule.AppointmentTooltipVisibility == Visibility.Visible)
                        {
                            if (schedule.editpopup.IsOpen)
                            {
                                schedule.editpopup.IsOpen = false;
                            }
                            if (schedule.addnewpopup.IsOpen)
                            {
                                schedule.addnewpopup.IsOpen = false;
                            }
                            schedule.InternalAppTooltipVisibility = Visibility.Visible;
                        }
                        schedule.prevPoisition = e.GetPosition(schedule);

                    }
                }
            }
            base.OnHolding(e);
        }

        #endregion

        #region Tapped

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            IsPressed = false;
            base.OnDoubleTapped(e);
        }

        #endregion
#else
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (schedule == null)
            {
                schedule = this.FindParentElementOfType<SfSchedule>();
            }
            if (IsPressed)
            {
                if (schedule.EnableTouch && !schedule.isContextMenuAltered)
                {
#if WPF
                    schedule.editpopup.Placement = PlacementMode.AbsolutePoint;
                    schedule.editpopup.PlacementTarget = schedule;
#endif
                    if (!IsSameAppointment)
                    {
                        schedule.editpopup.IsOpen = true;
                        schedule.editpopup.Child.UpdateLayout();
                        if (schedule.TouchMenuType == MenuType.RadialMenu)
                        {
                            var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                            if (editRadialMenuControl != null)
                                editRadialMenuControl.RadialMenu.IsOpen = true;
                        }

                        schedule.addnewpopup.IsOpen = false;
                    }
                    if (!schedule.editpopup.IsOpen)
                    {
                        schedule.editpopup.IsOpen = true;

                        schedule.addnewpopup.IsOpen = false;
                    }
                    if (e.GetPosition(schedule).X > schedule.ActualWidth / 2)
                    {
                        if (schedule.TouchMenuType == MenuType.Default)
                        {
                            var dragDropControl = schedule.editpopup.Child as DragDropControl;
                            if (dragDropControl != null)
                            {
                                dragDropControl.FlowDirection = FlowDirection.RightToLeft;
                                dragDropControl.ButtonFlowDirection = FlowDirection.LeftToRight;
                            }
                            schedule.editpopup.HorizontalOffset = e.GetPosition(schedule).X - 150;
                        }
                        else
                        {
#if WPF
                            schedule.editpopup.HorizontalOffset = e.GetPosition(schedule).X;
#else
                            schedule.editpopup.HorizontalOffset = e.GetPosition(schedule).X - 200;
#endif
                        }
                    }
                    else
                    {
                        if (schedule.TouchMenuType == MenuType.Default)
                        {
                            ((DragDropControl)schedule.editpopup.Child).FlowDirection = FlowDirection.LeftToRight;
                            schedule.editpopup.HorizontalOffset = e.GetPosition(schedule).X + 50;

                        }
                        else
                        {
#if WPF
                            schedule.editpopup.HorizontalOffset = e.GetPosition(schedule).X + 250;
#elif SILVERLIGHT
                            schedule.editpopup.HorizontalOffset = e.GetPosition(schedule).X;
#endif

                        }
                    }

                    if (schedule.TouchMenuType == MenuType.Default)
                    {
                        if (e.GetPosition(schedule).Y > schedule.ActualHeight - 75)
                        {
                            schedule.editpopup.VerticalOffset = e.GetPosition(schedule).Y - 140;
                        }
                        else
                        {
                            schedule.editpopup.VerticalOffset = e.GetPosition(schedule).Y - 60;
                        }
                    }
                    else
                    {
                        if (e.GetPosition(schedule).Y > schedule.ActualHeight - 300)
                        {
#if WPF
                            schedule.editpopup.VerticalOffset = e.GetPosition(schedule).Y - 100;
#else
                            schedule.editpopup.VerticalOffset = e.GetPosition(schedule).Y - 200;
#endif
                        }
                        else
                        {
#if WPF
                            schedule.editpopup.VerticalOffset = e.GetPosition(schedule).Y + 100;
#else
                            schedule.editpopup.VerticalOffset = e.GetPosition(schedule).Y;
#endif
                        }
                    }
                    if (DataContext is ScheduleAppointment &&  (DataContext as ScheduleAppointment).ReadOnly)
                    {
                        if (schedule.TouchMenuType == MenuType.Default)
                        {
                            ((DragDropControl)schedule.editpopup.Child).Resize.Visibility = Visibility.Collapsed;
                            ((DragDropControl)schedule.editpopup.Child).Delete.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                            if (editRadialMenuControl != null)
                            {
                                if (editRadialMenuControl.RadialMenu.Items[5] is SfRadialMenuItem)
                                    (editRadialMenuControl.RadialMenu.Items[5] as SfRadialMenuItem).IsEnabled = false;
                                if (editRadialMenuControl.RadialMenu.Items[4] is SfRadialMenuItem)
                                    (editRadialMenuControl.RadialMenu.Items[4] as SfRadialMenuItem).IsEnabled = false;
                            }
                        }
                    }
                    else
                    {
                        if (schedule.TouchMenuType == MenuType.Default)
                        {
                            ((DragDropControl)schedule.editpopup.Child).Resize.Visibility = Visibility.Visible;
                            ((DragDropControl)schedule.editpopup.Child).Delete.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                            if (editRadialMenuControl != null)
                            {
                                if (editRadialMenuControl.RadialMenu.Items[5] is SfRadialMenuItem)
                                    (editRadialMenuControl.RadialMenu.Items[5] as SfRadialMenuItem).IsEnabled = true;
                                if (editRadialMenuControl.RadialMenu.Items[4] is SfRadialMenuItem)
                                    (editRadialMenuControl.RadialMenu.Items[4] as SfRadialMenuItem).IsEnabled = true;
                            }
                        }
                    }
                    if (DataContext is ScheduleAppointment)
                    {
                        if (schedule.TouchMenuType == MenuType.Default)
                        {
                            ((DragDropControl)schedule.editpopup.Child).Copy.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                            if (editRadialMenuControl != null && editRadialMenuControl.RadialMenu.Items[2] is SfRadialMenuItem)
                            {
                                (editRadialMenuControl.RadialMenu.Items[2] as SfRadialMenuItem).IsEnabled = false;
                            }
                        }
                    }
                    else
                    {
                        if (schedule.TouchMenuType == MenuType.Default)
                        {
                            ((DragDropControl)schedule.editpopup.Child).Copy.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                            if (editRadialMenuControl != null && editRadialMenuControl.RadialMenu.Items[2] is SfRadialMenuItem)
                            {
                                (editRadialMenuControl.RadialMenu.Items[2] as SfRadialMenuItem).IsEnabled = true;
                            }
                        }

                    }
                    if (schedule.TouchMenuType == MenuType.Default)
                    {
                        var dragDropControl = schedule.editpopup.Child as DragDropControl;
                        if (dragDropControl != null)
                        {
                            dragDropControl.Edit.Visibility = (!schedule.AllowEditing) ? Visibility.Collapsed : Visibility.Visible;
                            dragDropControl.AddNew.Visibility = (!schedule.AllowEditing) ? Visibility.Collapsed : Visibility.Visible;
                        }
                    }
                    else
                    {
                        var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                        if (editRadialMenuControl != null && editRadialMenuControl.RadialMenu.Items != null)
                        {
                            if (editRadialMenuControl.RadialMenu.Items[0] is SfRadialMenuItem)
                            {
                                (editRadialMenuControl.RadialMenu.Items[0] as SfRadialMenuItem).IsEnabled = schedule.AllowEditing;
                                (editRadialMenuControl.RadialMenu.Items[0] as SfRadialMenuItem).Opacity = schedule.AllowEditing ? 1 : 0.5;
                            }
                            if (editRadialMenuControl.RadialMenu.Items[1] is SfRadialMenuItem)
                            {
                                (editRadialMenuControl.RadialMenu.Items[1] as SfRadialMenuItem).IsEnabled = schedule.AllowEditing;
                                (editRadialMenuControl.RadialMenu.Items[1] as SfRadialMenuItem).Opacity = schedule.AllowEditing ? 1 : 0.5;
                            }
                        }
                    }
                    if (schedule != null && schedule.editpopup != null && schedule.editpopup.Child is DragDropControl)
                    {
                        DragDropControl dragDropControl = schedule.editpopup.Child as DragDropControl;
                        if (!schedule.AllowEditing || (DataContext is ScheduleAppointment &&
                            (DataContext as ScheduleAppointment).ReadOnly ))
                        {
                            dragDropControl.bgPath.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            dragDropControl.bgPath.Visibility = Visibility.Visible;
                        }
                    }
                }
                IsPressed = false;
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!IsDragDropItem)
            {
                IsPressed = true;
            }
#if SILVERLIGHT
            schedule.OnMousePressDown(e);
#endif
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!IsDragDropItem && IsPressed)
            {
                if (!schedule.EnableTouch)
                {
                    schedule.IsDragEnabled = true;
                    if (schedule.SelectedAppointment != null && (!schedule.SelectedAppointment.ReadOnly))
                        schedule.EnableDragDrop();
                }
                IsPressed = false;
            }
            base.OnMouseMove(e);
        }

#if WPF
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            IsPressed = false;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            IsPressed = false;
            if (schedule != null)
            {
                var args = new ScheduleClickEventArgs();
                if (schedule.ItemsSource != null)
                {
                    IEnumerable<object> source = null;
#if WPF
                    if (schedule.ItemsSource is DataRowCollection)
                        source = (schedule.ItemsSource as DataRowCollection).OfType<DataRow>();
                    else
#endif
                        source = (IEnumerable<object>)schedule.ItemsSource;
                    object obj = source.FirstOrDefault(x => x.GetHashCode() == (int)schedule.SelectedAppointment.ObjectID);
                    args.Appointment = obj;
                }
                else
                {
                    args.Appointment = DataContext;
                }
                schedule.GetScheduleDoubleClickEvent(args);
            }
            base.OnMouseDoubleClick(e);
        }

#else
        protected override void OnHold(GestureEventArgs e)
        {
            e.Handled = true;
            base.OnHold(e);
        }
#endif

#endif

        #endregion

        public void Dispose()
        {

            Loaded -= ScheduleMonthAppointmentViewControl_Loaded;
#if WINRT
            PointerPressed -= ScheduleMonthAppointmentViewControl_PointerPressed;
            tooltiptimer.Tick -= tooltiptimer_Tick;
#endif
        }
    }
}
