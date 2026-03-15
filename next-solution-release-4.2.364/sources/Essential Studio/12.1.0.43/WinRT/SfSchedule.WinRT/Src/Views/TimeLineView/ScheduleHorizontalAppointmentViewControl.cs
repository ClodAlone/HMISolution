#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Syncfusion.UI.Xaml.Controls.Navigation;
using System;
using System.Collections;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#else
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
#if WPF
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Controls.Navigation;
using System.Collections.Generic;
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
    /// Represents a timeline view appointment.
    /// </summary>
    public class ScheduleHorizontalAppointmentViewControl : Control, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleHorizontalAppointmentViewControl">ScheduleHorizontalAppointmentViewControl</see>
        /// class.
        /// </summary>
        public ScheduleHorizontalAppointmentViewControl()
        {
            DefaultStyleKey = typeof(ScheduleHorizontalAppointmentViewControl);
            Loaded += ScheduleHorizontalAppointmentViewControl_Loaded;

#if WINRT
            tooltiptimer = new DispatcherTimer();
            tooltiptimer.Tick += tooltiptimer_Tick;
            PointerPressed += ScheduleHorizontalAppointmentViewControl_PointerPressed;
#endif
        }

        #endregion

        #region Private Fields

        bool IsPressed;
        bool IsDragDropItem;
#if WINRT
        bool enablePopup;
        bool IsEntered;
        readonly DispatcherTimer tooltiptimer;
#endif
        #endregion

        #region Internal Fields

        internal bool IsSameAppointment = false;
        internal SfSchedule schedule;

        #endregion

        #region Dependency Properties

        #region AppointmentWidth
        internal double AppWidth
        {
            get { return (double)GetValue(AppWidthProperty); }
            set { SetValue(AppWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppWidthProperty =
            DependencyProperty.Register("AppWidth", typeof(double), typeof(ScheduleHorizontalAppointmentViewControl), new PropertyMetadata(100d));
        #endregion

        #region AppointmentSelectionBrush
        internal Brush AppointmentSelectionBrush
        {
            get { return (Brush)GetValue(AppointmentSelectionBrushProperty); }
            set { SetValue(AppointmentSelectionBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentSelectionBrush.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(ScheduleHorizontalAppointmentViewControl), new PropertyMetadata(null));
        #endregion

        #region AppointmentTemplate
        internal DataTemplate AppointmentTemplate
        {
            get { return (DataTemplate)GetValue(AppointmentTemplateProperty); }
            set { SetValue(AppointmentTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentTemplate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentTemplateProperty =
            DependencyProperty.Register("AppointmentTemplate", typeof(DataTemplate), typeof(ScheduleHorizontalAppointmentViewControl), new PropertyMetadata(null));
        #endregion

        #region CustomTemplateDataContext
        internal object CustomTemplateDataContext
        {
            get { return GetValue(CustomTemplateDataContextProperty); }
            set { SetValue(CustomTemplateDataContextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomTemplateDataContext.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CustomTemplateDataContextProperty =
            DependencyProperty.Register("CustomTemplateDataContext", typeof(object), typeof(ScheduleHorizontalAppointmentViewControl), new PropertyMetadata(null));
        #endregion

        #region CustomTemplateVisibility
        internal Visibility CustomTemplateVisibility
        {
            get { return (Visibility)GetValue(CustomTemplateVisibilityProperty); }
            set { SetValue(CustomTemplateVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomTemplateVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CustomTemplateVisibilityProperty =
            DependencyProperty.Register("CustomTemplateVisibility", typeof(Visibility), typeof(ScheduleHorizontalAppointmentViewControl), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region DefaultTemplateVisibility
        internal Visibility DefaultTemplateVisibility
        {
            get { return (Visibility)GetValue(DefaultTemplateVisibilityProperty); }
            set { SetValue(DefaultTemplateVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultTemplateVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DefaultTemplateVisibilityProperty =
            DependencyProperty.Register("DefaultTemplateVisibility", typeof(Visibility), typeof(ScheduleHorizontalAppointmentViewControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region DragRectangleVisibility
        internal Visibility DragRectangleVisibility
        {
            get { return (Visibility)GetValue(DragRectangleVisibilityProperty); }
            set { SetValue(DragRectangleVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragRectangleVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DragRectangleVisibilityProperty =
            DependencyProperty.Register("DragRectangleVisibility", typeof(Visibility), typeof(ScheduleHorizontalAppointmentViewControl), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region DragDropStartTime
        internal string DragDropStartTime
        {
            get { return (string)GetValue(DragDropStartTimeProperty); }
            set { SetValue(DragDropStartTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragDropStartTime.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DragDropStartTimeProperty =
            DependencyProperty.Register("DragDropStartTime", typeof(string), typeof(ScheduleHorizontalAppointmentViewControl), new PropertyMetadata(null));
        #endregion

        #region DragDropEndTime
        internal string DragDropEndTime
        {
            get { return (string)GetValue(DragDropEndTimeProperty); }
            set { SetValue(DragDropEndTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragDropEndTime.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DragDropEndTimeProperty =
            DependencyProperty.Register("DragDropEndTime", typeof(string), typeof(ScheduleHorizontalAppointmentViewControl), new PropertyMetadata(null));
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
            DependencyProperty.Register("AppointmentTooltipVisibility", typeof(Visibility), typeof(ScheduleHorizontalAppointmentViewControl), new PropertyMetadata(Visibility.Collapsed));
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
            DependencyProperty.Register("AppointmentToolTipTemplate", typeof(ControlTemplate), typeof(ScheduleHorizontalAppointmentViewControl), new PropertyMetadata(null));
        #endregion
#endif
        #endregion

        #region Events

        void ScheduleHorizontalAppointmentViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            schedule = this.FindParentElementOfType<SfSchedule>();
            IsDragDropItem = this.FindParentElementOfType<Canvas>() != null;
            if (schedule != null && schedule.ItemsSource != null && schedule.AppointmentMapping != null)
            {
                var scheduleAppointment = DataContext as ScheduleAppointment;
                if (scheduleAppointment != null)
                {
                    double id = scheduleAppointment.ObjectID;
                    foreach (var item in ((IEnumerable)schedule.ItemsSource))
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

        void ScheduleHorizontalAppointmentViewControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var sfSchedule = this.FindParentElementOfType<SfSchedule>();
            Popup popup = sfSchedule.popup;
            if (popup != null)
            {
                var appointmentViewControl = sender as UIElement;
                if (appointmentViewControl != null)
                {
                    GeneralTransform gt = appointmentViewControl.TransformToVisual(sfSchedule);
                    PointerPoint pointerPoint = e.GetCurrentPoint(appointmentViewControl);
                    Point screenPoint = gt.TransformPoint(new Point(pointerPoint.Position.X, pointerPoint.Position.Y));
                    if (screenPoint.Y > (sfSchedule.ActualHeight - popup.Height))
                        screenPoint.Y = (sfSchedule.ActualHeight - popup.Height);
                    if (screenPoint.X > (sfSchedule.ActualWidth - popup.Width))
                        screenPoint.X = (sfSchedule.ActualWidth - popup.Width);
                    popup.HorizontalOffset = screenPoint.X;
                    popup.VerticalOffset = screenPoint.Y;
                }
            }
        }
#endif

        #endregion

        #region Override Methods
#if WINRT
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
            }
            if (!IsDragDropItem)
            {
                if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                {
                    IsPressed = true;
                }
                else
                {
                    if (schedule != null && schedule.SelectedAppointment != null)
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
                    if (!e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
                    {
                        IsPressed = true;
                    }
                }
            }
            if (schedule != null)
            {
                PointerPoint ptrPt = e.GetCurrentPoint(schedule);
                enablePopup = (ptrPt.Properties.IsLeftButtonPressed);
            }
            base.OnPointerPressed(e);
        }

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
                    viewer.HorizontalScrollMode = ScrollMode.Enabled;
                    //schedule.flipviewscroll.HorizontalScrollMode = ScrollMode.Enabled;
                }
                else if (IsPressed)
                {
                    //Removed Mouse direct drag drop interaction
                    //schedule.IsMouseDrag = true;
                    //schedule.IsDragEnabled = true;
                    //schedule.IsDragStarted = true;
                    //schedule.EnableDragDrop();
                    IsPressed = false;
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

            }
            base.OnPointerMoved(e);
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                if (schedule == null)
                {
                    schedule = this.FindParentElementOfType<SfSchedule>();
                }
                else
                {
                    schedule.InternalAppTooltipVisibility = Visibility.Collapsed;
                }
            }
            if (schedule == null)
            {
                schedule = this.FindParentElementOfType<SfSchedule>();
            }
            if (schedule != null)
            {
                if (IsPressed)
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
                        if (e.GetCurrentPoint(schedule).Position.X > schedule.ActualWidth/2)
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
                        if (DataContext is ScheduleAppointment &&
                             (DataContext as ScheduleAppointment).ReadOnly)
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
                                        (editRadialMenuControl.RadialMenu.Items[5] as SfRadialMenuItem).IsEnabled =
                                            false;
                                    if (editRadialMenuControl.RadialMenu.Items[4] is SfRadialMenuItem)
                                        (editRadialMenuControl.RadialMenu.Items[4] as SfRadialMenuItem).IsEnabled =
                                            false;
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
                                if (editRadialMenuControl != null && editRadialMenuControl.RadialMenu.Items != null &&
                                    editRadialMenuControl.RadialMenu.Items[2] is SfRadialMenuItem)
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
                                if (editRadialMenuControl != null && editRadialMenuControl.RadialMenu.Items != null &&
                                    editRadialMenuControl.RadialMenu.Items[2] is SfRadialMenuItem)
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
                                    if (editRadialMenuControl.RadialMenu.Items[0] is SfRadialMenuItem)
                                        (editRadialMenuControl.RadialMenu.Items[0] as SfRadialMenuItem).IsEnabled =
                                            false;
                                    if (editRadialMenuControl.RadialMenu.Items[1] is SfRadialMenuItem)
                                        (editRadialMenuControl.RadialMenu.Items[1] as SfRadialMenuItem).IsEnabled =
                                            false;
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
                                    if (editRadialMenuControl.RadialMenu.Items[0] is SfRadialMenuItem)
                                        (editRadialMenuControl.RadialMenu.Items[0] as SfRadialMenuItem).IsEnabled = true;
                                    if (editRadialMenuControl.RadialMenu.Items[1] is SfRadialMenuItem)
                                        (editRadialMenuControl.RadialMenu.Items[1] as SfRadialMenuItem).IsEnabled = true;
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
                }
                else
                {
                    schedule.editpopup.IsOpen = false;
                    schedule.addnewpopup.IsOpen = false;
                }
                if (!IsDragDropItem)
                {
                    schedule.DragDropCanvas.Children.Clear();

                    if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                    {
                        var viewer = this.FindParentElementOfType<ScrollViewer>();
                        viewer.HorizontalScrollMode = ScrollMode.Enabled;
                        viewer.VerticalScrollMode = ScrollMode.Enabled;
                        //schedule.flipviewscroll.HorizontalScrollMode = ScrollMode.Enabled;
                    }
                    else
                    {
                        IsPressed = false;
                    }
                }
            }
            base.OnPointerReleased(e);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                if (schedule != null && e.OriginalSource is FrameworkElement)
                {
                    var app = (e.OriginalSource as FrameworkElement).DataContext as ScheduleAppointment;
                    if (app != null && !schedule.IsDragEnabled)
                    {
                        if (tooltiptimer != null)
                        {
                            tooltiptimer.Interval = new System.TimeSpan(0, 0, 1);
                            tooltiptimer.Start();
                        }
                        schedule.AppointmentTooltip.DataContext = app;
                        IsEntered = true;
                    }
                }
            }
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                tooltiptimer.Stop();
                var sfSchedule = this.FindParentElementOfType<SfSchedule>();
                if (sfSchedule != null)
                {
                    sfSchedule.InternalAppTooltipVisibility = Visibility.Collapsed;
                }
            }
            base.OnPointerExited(e);
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            IsPressed = false;
            base.OnDoubleTapped(e);
        }

        protected override void OnHolding(HoldingRoutedEventArgs e)
        {
            if (schedule != null && e.OriginalSource is FrameworkElement)
            {
                var app = (e.OriginalSource as FrameworkElement).DataContext as ScheduleAppointment;
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
            base.OnHolding(e);
        }
#else
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!IsDragDropItem)
            {
                if (schedule == null)
                {
                    schedule = this.FindParentElementOfType<SfSchedule>();
                }
                if (IsPressed)
                {
                    schedule.DragDropCanvas.Children.Clear();
                    if (schedule.EnableTouch && !schedule.isContextMenuAltered)
                    {
#if WPF
                        schedule.editpopup.Placement = PlacementMode.AbsolutePoint;
                        schedule.editpopup.PlacementTarget = schedule;
#endif
                        if (!IsSameAppointment)
                        {
                            schedule.editpopup.IsOpen = schedule.EnableTouch;
                            schedule.editpopup.Child.UpdateLayout();
                            schedule.addnewpopup.IsOpen = false;
                            if (schedule.TouchMenuType == MenuType.RadialMenu)
                            {
                                ((EditRadialMenuControl)schedule.editpopup.Child).RadialMenu.IsOpen = true;
                            }
                        }
                        if (!schedule.editpopup.IsOpen)
                        {
                            schedule.editpopup.IsOpen = schedule.EnableTouch;
                            schedule.editpopup.Child.UpdateLayout();
                            schedule.addnewpopup.IsOpen = false;
                        }
                        if (e.GetPosition(schedule).X > schedule.ActualWidth / 2)
                        {
                            if (schedule.TouchMenuType == MenuType.Default)
                            {
                                ((DragDropControl)schedule.editpopup.Child).FlowDirection = FlowDirection.RightToLeft;
                                ((DragDropControl)schedule.editpopup.Child).ButtonFlowDirection = FlowDirection.LeftToRight;
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
                                    (editRadialMenuControl.RadialMenu.Items[2] as SfRadialMenuItem).IsEnabled = false;
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
                                    (editRadialMenuControl.RadialMenu.Items[2] as SfRadialMenuItem).IsEnabled = true;
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
                    else if (schedule.editpopup != null)
                    {
                        schedule.editpopup.IsOpen = false;
                        schedule.addnewpopup.IsOpen = false;
                    }

                    IsPressed = false;
                }
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
                    if (schedule.SelectedAppointment != null && (!schedule.SelectedAppointment.ReadOnly) && !schedule.SelectedAppointment.IsRecursive)
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
#endif
#endif
        #endregion

        #region Methods
        public void Dispose()
        {
            Loaded -= ScheduleHorizontalAppointmentViewControl_Loaded;

#if WINRT
            tooltiptimer.Tick -= tooltiptimer_Tick;
            PointerPressed -= ScheduleHorizontalAppointmentViewControl_PointerPressed;
#endif
        }
        #endregion
    }
}
