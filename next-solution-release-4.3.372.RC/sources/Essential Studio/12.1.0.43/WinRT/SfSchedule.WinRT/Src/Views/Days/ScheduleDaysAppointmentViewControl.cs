#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using System;
using Windows.Foundation;
using System.Linq;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using System.Collections;
using Syncfusion.UI.Xaml.Controls.Navigation;
#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections;
using System.Windows.Shapes;
#if WPF
using Syncfusion.Windows.Controls.Navigation;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using System.Data;
#elif SILVERLIGHT
using Syncfusion.Tools.Controls.Navigation;
#endif
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    #region ScheduleDaysAppointmentViewControl

    /// <summary>
    /// Represents a day view appointment.
    /// </summary>
    public class ScheduleDaysAppointmentViewControl : Control, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleDaysAppointmentViewControl">ScheduleDaysAppointmentViewControl</see>
        /// class.
        /// </summary>
        public ScheduleDaysAppointmentViewControl()
        {
#if WINRT
            tooltiptimer = new DispatcherTimer();
            PointerPressed += ScheduleDaysAppointmentViewControl_PointerPressed;
            tooltiptimer.Tick += tooltiptimer_Tick;
#endif
            DefaultStyleKey = typeof(ScheduleDaysAppointmentViewControl);
            Loaded += ScheduleDaysAppointmentViewControl_Loaded;
        }

        #endregion

        #region Private Fields

        internal bool IsSameAppointment = false;
        internal SfSchedule schedule;
        bool IsPressed;
        bool IsDragDropItem;
#if WINRT
        bool IsEntered;
        bool enablePopup;
        readonly DispatcherTimer tooltiptimer;
#endif
        #endregion

        #region Dependency Properties

        #region AppointmentTemplate
        /// <summary>
        /// Gets or sets the template for customizing appointment in day view.
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
            DependencyProperty.Register("AppointmentTemplate", typeof(DataTemplate), typeof(ScheduleDaysAppointmentViewControl), new PropertyMetadata(null));
        #endregion

        #region CustomTemplateDataContext
        /// <summary>
        /// Gets or sets the data context for custom appointment in day view.
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
            DependencyProperty.Register("CustomTemplateDataContext", typeof(object), typeof(ScheduleDaysAppointmentViewControl), new PropertyMetadata(null));
        #endregion

        #region CustomTemplateVisibility
        /// <summary>
        /// Gets or sets the visibility of custom appointment in day view.
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
            DependencyProperty.Register("CustomTemplateVisibility", typeof(Visibility), typeof(ScheduleDaysAppointmentViewControl), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region DefaultTemplateVisibility
        /// <summary>
        /// Gets or sets the visibility of default appointment in day view.
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
            DependencyProperty.Register("DefaultTemplateVisibility", typeof(Visibility), typeof(ScheduleDaysAppointmentViewControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region AppointmentWidth
        internal double AppWidth
        {
            get { return (double)GetValue(AppWidthProperty); }
            set { SetValue(AppWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppWidthProperty =
            DependencyProperty.Register("AppWidth", typeof(double), typeof(ScheduleDaysAppointmentViewControl), new PropertyMetadata(100d));
        #endregion

        #region AppointmentSelectionBrush
        internal Brush AppointmentSelectionBrush
        {
            get { return (Brush)GetValue(AppointmentSelectionBrushProperty); }
            set { SetValue(AppointmentSelectionBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentSelectionBrush.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(ScheduleDaysAppointmentViewControl), new PropertyMetadata(null));
        #endregion

        #region DragRectangleVisibility
        internal Visibility DragRectangleVisibility
        {
            get { return (Visibility)GetValue(DragRectangleVisibilityProperty); }
            set { SetValue(DragRectangleVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragRectangleVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DragRectangleVisibilityProperty =
            DependencyProperty.Register("DragRectangleVisibility", typeof(Visibility), typeof(ScheduleDaysAppointmentViewControl), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region DragDropStartTime
        internal string DragDropStartTime
        {
            get { return (string)GetValue(DragDropStartTimeProperty); }
            set { SetValue(DragDropStartTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragDropStartTime.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DragDropStartTimeProperty =
            DependencyProperty.Register("DragDropStartTime", typeof(string), typeof(ScheduleDaysAppointmentViewControl), new PropertyMetadata(null));
        #endregion

        #region DragDropEndTime
        internal string DragDropEndTime
        {
            get { return (string)GetValue(DragDropEndTimeProperty); }
            set { SetValue(DragDropEndTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragDropEndTime.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DragDropEndTimeProperty =
            DependencyProperty.Register("DragDropEndTime", typeof(string), typeof(ScheduleDaysAppointmentViewControl), new PropertyMetadata(null));
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
            DependencyProperty.Register("AppointmentTooltipVisibility", typeof(Visibility), typeof(ScheduleDaysAppointmentViewControl), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region AppointmentToolTipTemplate
        /// <summary>
        /// Gets or sets a template for customizing appointment's tooltip.
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
            DependencyProperty.Register("AppointmentToolTipTemplate", typeof(ControlTemplate), typeof(ScheduleDaysAppointmentViewControl), new PropertyMetadata(null));
        #endregion
#endif

        #endregion

        #region Events

        void ScheduleDaysAppointmentViewControl_Loaded(object sender, RoutedEventArgs e)
        {
            schedule = this.FindParentElementOfType<SfSchedule>();
            IsDragDropItem = this.FindParentElementOfType<Canvas>() != null;
            if (schedule != null && schedule.ItemsSource != null && schedule.AppointmentMapping != null && DataContext is ScheduleAppointment)
            {
                double id = (DataContext as ScheduleAppointment).ObjectID;
                var enumerable = schedule.ItemsSource as IEnumerable;
                if (enumerable != null)
                    foreach (var item in enumerable)
                    {
                        if (item.GetHashCode() == (int)id)
                        {
                            CustomTemplateDataContext = item;
                            break;
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

        void ScheduleDaysAppointmentViewControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var sfSchedule = this.FindParentElementOfType<SfSchedule>();
            Popup popup = sfSchedule.popup;
            if (popup != null)
            {
                var appointmentViewControl = sender as ScheduleDaysAppointmentViewControl;
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
            if (schedule != null)
            {
                if (!IsDragDropItem)
                {
                    if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                    {
                        IsPressed = true;
                        schedule.IsMouseDrag = false;
                    }
                    else
                    {
                        IsPressed = true;
                    }
                }
                PointerPoint ptrPt = e.GetCurrentPoint(schedule);
                enablePopup = (ptrPt.Properties.IsLeftButtonPressed);
            }
            base.OnPointerPressed(e);
        }

        #endregion

        #region Pointer Released

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
            if (schedule != null && !IsDragDropItem)
            {
                if (IsPressed)
                {
                    schedule.DragDropCanvas.Children.Clear();

                    if (enablePopup && !schedule.isContextMenuAltered)
                    {
                        if (!IsSameAppointment)
                        {
                            schedule.editpopup.IsOpen = enablePopup;
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
                            schedule.editpopup.IsOpen = enablePopup;
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
                        if (DataContext is ScheduleAppointment && ((DataContext as ScheduleAppointment).ReadOnly))
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
                                    if (editRadialMenuControl.RadialMenu.Items[0] is SfRadialMenuItem)
                                        (editRadialMenuControl.RadialMenu.Items[0] as SfRadialMenuItem).IsEnabled = false;
                                    if (editRadialMenuControl.RadialMenu.Items[1] is SfRadialMenuItem)
                                        (editRadialMenuControl.RadialMenu.Items[1] as SfRadialMenuItem).IsEnabled = false;
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
                    else if (schedule.editpopup != null)
                    {
                        schedule.editpopup.IsOpen = false;
                        schedule.addnewpopup.IsOpen = false;
                    }
                    if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                    {
                        var viewer = this.FindParentElementOfType<ScrollViewer>();
                        viewer.VerticalScrollMode = ScrollMode.Enabled;
                        viewer.HorizontalScrollMode = ScrollMode.Enabled;
                        //schedule.flipviewscroll.HorizontalScrollMode = ScrollMode.Enabled;
                        var dayview = this.FindParentElementOfType<ScheduleDaysView>();
                        dayview.headerscrollviewer.HorizontalScrollMode = ScrollMode.Enabled;
                    }
                    else
                    {
                        IsPressed = false;
                    }
                }
                base.OnPointerReleased(e);
            }
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            if (!IsDragDropItem)
            {
                IsPressed = false;
            }
            base.OnDoubleTapped(e);
        }

        #endregion

        #region Pointer Moved

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            if (schedule == null)
            {
                schedule = this.FindParentElementOfType<SfSchedule>();
            }
            if (schedule != null && !IsDragDropItem)
            {
                if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                {
                    var viewer = this.FindParentElementOfType<ScrollViewer>();
                    viewer.VerticalScrollMode = ScrollMode.Enabled;
                    viewer.HorizontalScrollMode = ScrollMode.Enabled;
                    //schedule.flipviewscroll.HorizontalScrollMode = ScrollMode.Enabled;
                    var dayview = this.FindParentElementOfType<ScheduleDaysView>();
                    dayview.headerscrollviewer.HorizontalScrollMode = ScrollMode.Enabled;
                }
                else if (IsPressed)
                {
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

        #endregion

        #region Pointer Holding

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

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                if (schedule != null)
                {
                    var frameworkElement = e.OriginalSource as FrameworkElement;
                    if (frameworkElement != null)
                    {
                        var app = frameworkElement.DataContext as ScheduleAppointment;
                        if (app != null && !schedule.IsDragEnabled)
                        {
                            tooltiptimer.Interval = new TimeSpan(0, 0, 1);
                            tooltiptimer.Start();
                            schedule.AppointmentTooltip.DataContext = app;
                            IsEntered = true;
                        }
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

        #endregion
#else
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (schedule == null)
            {
                schedule = this.FindParentElementOfType<SfSchedule>();
            }
            else
            {
                if (schedule.SelectedAppointment != null)
                {
                    if (DataContext.Equals(schedule.SelectedAppointment) && !schedule.isAppointment)
                    {
                        IsSameAppointment = true;
                    }
                    else
                    {
                        IsSameAppointment = false;
                    }
                }
            }
            IsPressed = !IsDragDropItem;

#if SILVERLIGHT
            schedule.OnMousePressDown(e);
#endif
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!IsDragDropItem)
            {
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
                            schedule.editpopup.IsOpen = schedule.EnableTouch;
                            schedule.editpopup.Child.UpdateLayout();
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
                                var dragDropControl = schedule.editpopup.Child as DragDropControl;
                                if (dragDropControl != null)
                                    dragDropControl.FlowDirection = FlowDirection.LeftToRight;
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
                            if (schedule.TouchMenuType == MenuType.Default)
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
                        if (DataContext is ScheduleAppointment)
                        {
                            if (schedule.TouchMenuType == MenuType.Default)
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
                            if (schedule.TouchMenuType == MenuType.Default)
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
                schedule.GetScheduleClickEvent(args);
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

        #region OnApplyTemplate

#if WINRT
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if ((GetTemplateChild("Resizerectangle") as Rectangle) != null)
            {
                Rectangleopacityanimation(GetTemplateChild("Resizerectangle") as Rectangle);
            }
        }
#endif

        #endregion

        #endregion

        #region Methods
#if WINRT
        internal void Rectangleopacityanimation(UIElement rectangle)
        {
            var rectangleanimation = new Storyboard();

            var dbAniKeyFrame = new DoubleAnimationUsingKeyFrames();
            var dbkeyframe = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0)), Value = 0 };
            var dbkeyframe1 = new EasingDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 1)), Value = 1 };
            dbAniKeyFrame.KeyFrames.Add(dbkeyframe);
            dbAniKeyFrame.KeyFrames.Add(dbkeyframe1);
            rectangleanimation.Children.Add(dbAniKeyFrame);
            Storyboard.SetTarget(dbAniKeyFrame, rectangle);
            Storyboard.SetTargetProperty(dbAniKeyFrame, "(UIElement.Opacity)");
            rectangleanimation.Begin();
        }
#endif
        #endregion

        public void Dispose()
        {
#if WINRT
            PointerPressed -= ScheduleDaysAppointmentViewControl_PointerPressed;
            tooltiptimer.Tick -= tooltiptimer_Tick;
#endif
            Loaded -= ScheduleDaysAppointmentViewControl_Loaded;
        }
    }

    #endregion

    #region DragDropControl

    /// <summary>
    /// Represents a control for appointment drag and drop
    /// </summary>
    public class DragDropControl : Control, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.DragDropControl">DragDropControl</see>
        /// class.
        /// </summary>
        public DragDropControl()
        {
            DefaultStyleKey = typeof(DragDropControl);
        }

        #endregion

        #region Internal Fields

        internal Storyboard Story
        {
            get
            {
                if (grid != null)
                {
                    return grid.Resources["storyboard"] as Storyboard;
                }
                return null;
            }
        }
        
        internal Path bgPath;
        internal Button AddNew;
        internal Button Edit;
        internal SfSchedule schedule;
        internal Button Delete;
        internal Button Resize;
        internal Button Copy;
        private Grid grid;

        #endregion

        #region ButtonFlowDirection
        internal FlowDirection ButtonFlowDirection
        {
            get { return (FlowDirection)GetValue(ButtonFlowDirectionProperty); }
            set { SetValue(ButtonFlowDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonFlowDirection.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ButtonFlowDirectionProperty =
            DependencyProperty.Register("ButtonFlowDirection", typeof(FlowDirection), typeof(DragDropControl), new PropertyMetadata(FlowDirection.LeftToRight));
        #endregion

        #region Override Methods

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            grid = GetTemplateChild("popupgrid") as Grid;
            schedule = this.FindParentElementOfType<SfSchedule>();
            bgPath = GetTemplateChild("bg") as Path;
            AddNew = GetTemplateChild("Add") as Button;
            Edit = GetTemplateChild("Edit") as Button;
            Delete = GetTemplateChild("Delete") as Button;
            Resize = GetTemplateChild("Resize") as Button;
            Copy = GetTemplateChild("Copy") as Button;
#if WPF
            if (AddNew != null) AddNew.PreviewMouseDown += AddNew_MouseDown;
            if (Copy != null) Copy.PreviewMouseDown += Copy_MouseDown;
            if (Edit != null) Edit.PreviewMouseDown += Edit_MouseDown;
            if (Resize != null) Resize.PreviewMouseDown += Resize_MouseDown;
            if (Delete != null) Delete.PreviewMouseDown += Delete_MouseDown;
#else
            if (Copy != null) Copy.Click += Copy_Click;
            if (AddNew != null) AddNew.Click += AddNew_Click;
            if (Edit != null) Edit.Click += Edit_Click;
            if (Delete != null) Delete.Click += Delete_Click;
            if (Resize != null) Resize.Click += Resize_Click;
#endif
        }

#if SILVERLIGHT
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (schedule != null)
            {
                schedule.isTouchMenuHovered = false;
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (schedule != null)
            {
                schedule.isTouchMenuHovered = true;

            }
        }
#endif

        #endregion

        #region Events

#if !WPF
        void Copy_Click(object sender, RoutedEventArgs e)
#else
        void Copy_MouseDown(object sender, MouseButtonEventArgs e)
#endif
        {
            if (schedule != null)
            {
                schedule.CopyAppointment();
            }
        }

#if !WPF
        void Resize_Click(object sender, RoutedEventArgs e)
#else
        void Resize_MouseDown(object sender, MouseButtonEventArgs e)
#endif
        {
            schedule.ResizeAppointment();
        }

#if !WPF
        void Delete_Click(object sender, RoutedEventArgs e)
#else
        void Delete_MouseDown(object sender, MouseButtonEventArgs e)
#endif
        {
            schedule.DeleteAppointment();
        }

#if !WPF
        void Edit_Click(object sender, RoutedEventArgs e)
#else
        void Edit_MouseDown(object sender, MouseButtonEventArgs e)
#endif
        {
            schedule.EditAppointment();
        }

#if !WPF
        void AddNew_Click(object sender, RoutedEventArgs e)
#else
        void AddNew_MouseDown(object sender, MouseButtonEventArgs e)
#endif
        {
            schedule.AddNewAppointment();
        }

        #endregion

        #region Methods
        public void Dispose()
        {
#if WPF
            if (AddNew != null) AddNew.PreviewMouseDown -= AddNew_MouseDown;
            if (Copy != null) Copy.PreviewMouseDown -= Copy_MouseDown;
            if (Edit != null) Edit.PreviewMouseDown -= Edit_MouseDown;
            if (Resize != null) Resize.PreviewMouseDown -= Resize_MouseDown;
            if (Delete != null) Delete.PreviewMouseDown -= Delete_MouseDown;
#else
            if (Copy != null) Copy.Click -= Copy_Click;
            if (AddNew != null) AddNew.Click -= AddNew_Click;
            if (Edit != null) Edit.Click -= Edit_Click;
            if (Delete != null) Delete.Click -= Delete_Click;
            if (Resize != null) Resize.Click -= Resize_Click;
#endif
        }
        #endregion
    }

    #endregion

    #region AddAppintmentControl

    /// <summary>
    /// Represents a default menu for adding appointment
    /// </summary>
    public class AddAppintmentControl : Control, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.AddAppintmentControl">AddAppintmentControl</see>
        /// class.
        /// </summary>
        public AddAppintmentControl()
        {
            DefaultStyleKey = typeof(AddAppintmentControl);
        }

        #endregion

        #region Internal Fields

        internal Storyboard Story
        {
            get
            {
                if (grid != null)
                {
                    return grid.Resources["storyboard2"] as Storyboard;
                }
                return null;
            }
        }
        internal Button AddNew;
        internal Button Paste;
        internal SfSchedule schedule;
        private Grid grid;

        #endregion

        #region Override Methods

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            AddNew = GetTemplateChild("Add") as Button;
            Paste = GetTemplateChild("Paste") as Button;
            grid = GetTemplateChild("grid") as Grid;
            schedule = this.FindParentElementOfType<SfSchedule>();
            if (Paste != null)
            {
                Paste.Visibility = Visibility.Collapsed;
#if WPF
                Paste.PreviewMouseDown += Paste_MouseDown;
#else
                Paste.Click += Paste_Click;
#endif
            }
            if (AddNew != null)
            {
#if WPF
                AddNew.PreviewMouseDown += AddNew_MouseDown;
#else
                AddNew.Click += AddNew_Click;
#endif
            }
        }

#if SILVERLIGHT
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (schedule != null)
            {
                schedule.isTouchMenuHovered = false;
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (schedule != null)
            {
                schedule.isTouchMenuHovered = true;

            }
        }
#endif

        #endregion

        #region Events

#if !WPF
        void Paste_Click(object sender, RoutedEventArgs e)
#else
        void Paste_MouseDown(object sender, MouseButtonEventArgs e)
#endif
        {
            if (schedule != null)
            {
                schedule.PasteAppointment();
            }
        }

#if !WPF
        void AddNew_Click(object sender, RoutedEventArgs e)
#else
        void AddNew_MouseDown(object sender, MouseButtonEventArgs e)
#endif
        {
            schedule.AddNewAppointment();
        }

        #endregion

        #region Methods
        public void Dispose()
        {
            if (Paste != null)
            {
#if WPF
                Paste.PreviewMouseDown -= Paste_MouseDown;
#else
                Paste.Click -= Paste_Click;
#endif
            }
            if (AddNew != null)
            {
#if WPF
                AddNew.PreviewMouseDown -= AddNew_MouseDown;
#else
                AddNew.Click -= AddNew_Click;
#endif
            }
        }
        #endregion
    }

    #endregion

    #region AddRadialMenuControl

    /// <summary>
    /// Represents a radial menu for adding appointment.
    /// </summary>
    public class AddRadialMenuControl : Control, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.AddRadialMenuControl">AddRadialMenuControl</see>
        /// class.
        /// </summary>
        public AddRadialMenuControl()
        {
            DefaultStyleKey = typeof(AddRadialMenuControl);
        }

        #endregion

        #region Internal Fields

        internal SfRadialMenu RadialMenu;
        internal SfSchedule schedule;

        #endregion

        #region Override Methods

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            RadialMenu = GetTemplateChild("radialMenu") as SfRadialMenu;
            schedule = this.FindParentElementOfType<SfSchedule>();
            if (RadialMenu != null && RadialMenu.Items != null)
            {
                foreach (var item in RadialMenu.Items)
                {
                    if (item is SfRadialMenuItem)
                    {
#if WPF
                        (item as SfRadialMenuItem).PreviewMouseLeftButtonDown += AddRadialMenuControl_Click;
                        (item as SfRadialMenuItem).MouseMove += AddRadialMenuControl_PointerMoved;
                        (item as SfRadialMenuItem).MouseLeave += AddRadialMenuControl_PointerExited;
#elif SILVERLIGHT
                        (item as SfRadialMenuItem).MouseLeftButtonDown += AddRadialMenuControl_Click;
                        (item as SfRadialMenuItem).MouseMove += AddRadialMenuControl_PointerMoved;
                        (item as SfRadialMenuItem).MouseLeave += AddRadialMenuControl_PointerExited;
#elif WINRT
                        (item as SfRadialMenuItem).Click += AddRadialMenuControl_Click;
                        (item as SfRadialMenuItem).PointerMoved += AddRadialMenuControl_PointerMoved;
                        (item as SfRadialMenuItem).PointerExited += AddRadialMenuControl_PointerExited;
#endif
                    }
                }
            }
            base.OnApplyTemplate();
        }

        #endregion

        #region Events

#if WINRT
        void AddRadialMenuControl_PointerMoved(object sender, PointerRoutedEventArgs e) 
#else
        void AddRadialMenuControl_PointerMoved(object sender, RoutedEventArgs e) 
#endif
        {
            var path = ((sender as SfRadialMenuItem).Header as Border).FindElementOfType<Path>();
            if (path != null)
                path.Fill = new SolidColorBrush(Color.FromArgb(0XFF, 0X28, 0XA5, 0XDB));
            var textBlock = ((sender as SfRadialMenuItem).Header as Border).FindElementOfType<TextBlock>();
            if (textBlock != null)
                textBlock.Foreground = new SolidColorBrush(Colors.Black);
        }

#if WINRT
        void AddRadialMenuControl_PointerExited(object sender, PointerRoutedEventArgs e) 
#else
        void AddRadialMenuControl_PointerExited(object sender, RoutedEventArgs e)
#endif
        {
            var path = ((sender as SfRadialMenuItem).Header as Border).FindElementOfType<Path>();
            if (path != null)
                path.Fill = new SolidColorBrush(Color.FromArgb(0XFF, 0X48, 0X49, 0X49));
            var textBlock = ((sender as SfRadialMenuItem).Header as Border).FindElementOfType<TextBlock>();
            if (textBlock != null)
                textBlock.Foreground = new SolidColorBrush(Color.FromArgb(0XFF, 0X48, 0X49, 0X49));
        }

        void AddRadialMenuControl_Click(object sender, RoutedEventArgs e)
        {
            if (RadialMenu.Items != null)
            {
                int index = RadialMenu.Items.IndexOf(sender);
                switch (index)
                {
                    case 0:
                        {
                            schedule.AddNewAppointment();
                            break;
                        }
                    case 3:
                        {
                            if (schedule != null && schedule.CopiedAppointment != null)
                            {
                                var app = (ScheduleAppointment)schedule.AppointmentCloning(schedule.CopiedAppointment);
                                if (schedule.SelectedAppointment != null)
                                {
                                    schedule.SelectedAppointment.IsSelected = false;
                                    schedule.SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                                }
#if WINRT
                                DateTime date = schedule.ScheduleType == ScheduleType.Month ? schedule.Currentselecteddate : schedule.GetDate(schedule.SelectedPoint);
#else
                                DateTime date = schedule.Currentselecteddate;
#endif
                                DateTime startime = schedule.CopiedAppointment.StartTime;
                                DateTime endtime = schedule.CopiedAppointment.EndTime;

                                if (app.ResourceCollection.Count == 0)
                                {
                                    foreach (Resource selectedresource in schedule.selectedResourcename)
                                    {
                                        app.ResourceCollection.Add(selectedresource);
                                    }
                                }
                                else if (app.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource) != null)
                                {
                                    foreach (Resource selectedresource in schedule.selectedResourcename)
                                    {
                                        var firstOrDefault = app.ResourceCollection.FirstOrDefault(res => res.TypeName == selectedresource.TypeName);
                                        if (firstOrDefault != null)
                                            firstOrDefault.ResourceName = selectedresource.ResourceName;
                                    }
                                }
                                else
                                {
                                    foreach (Resource selectedresource in schedule.selectedResourcename)
                                    {
                                        app.ResourceCollection.Add(selectedresource);
                                    }
                                }
                                if (schedule.ScheduleType == ScheduleType.Month)
                                {
                                    if (startime == endtime)
                                    {
                                        app.AllDay = true;
                                    }
                                    app.StartTime = schedule.Currentselecteddate.AddMinutes(startime.Minute);
                                    app.EndTime = schedule.Currentselecteddate.Add(endtime - startime);
                                }

#if WINRT
                                else if (schedule.allDayFlag)
                                {
                                    app.AllDay = true;
                                    app.StartTime = date;
                                    app.EndTime = date;
                                }

#else
                                else if (schedule.allDayTouch)
                                {
                                    schedule.allDayTouch = false;
                                    app.AllDay = true;
                                    app.StartTime = date;
                                    app.EndTime = date;
                                }
#endif
                                else
                                {
                                    app.StartTime = date;
                                    if (endtime != startime)
                                    {
                                        app.EndTime = date.Add(endtime - startime);
                                    }
                                    else
                                    {
                                        app.EndTime = date.Add(schedule.GetTimeInterval());
                                    }
                                }
                                schedule.SelectedAppointment = app;
                                app.IsSelected = true;
                                app.AppointmentSelectionBrush = schedule.AppointmentSelectionBrush;
                                schedule.Appointments.Add(app);
                                schedule.addnewpopup.IsOpen = false;

                            }
                            break;
                        }

                }
            }
        }

        #endregion

        #region Methods
        public void Dispose()
        {
            if (RadialMenu != null && RadialMenu.Items != null)
            {
                foreach (var item in RadialMenu.Items)
                {
                    if (item is SfRadialMenuItem)

#if WPF
                        (item as SfRadialMenuItem).PreviewMouseLeftButtonDown -= AddRadialMenuControl_Click;
#elif SILVERLIGHT
                        (item as SfRadialMenuItem).MouseLeftButtonDown -= AddRadialMenuControl_Click;
#elif WINRT
                        (item as SfRadialMenuItem).Click -= AddRadialMenuControl_Click;
#endif
                }
            }
        }
        #endregion
    }

    #endregion

    #region EditRadialMenuControl

    /// <summary>
    /// Represents a radial menu for editing appointment.
    /// </summary>
    public class EditRadialMenuControl : Control, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.EditRadialMenuControl">EditRadialMenuControl</see>
        /// class.
        /// </summary>
        public EditRadialMenuControl()
        {
            DefaultStyleKey = typeof(EditRadialMenuControl);
        }

        #endregion

        #region Internal Fields

        internal SfRadialMenu RadialMenu;
        internal SfSchedule schedule;

        #endregion

        #region Overrride Methods

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            RadialMenu = GetTemplateChild("radialMenu") as SfRadialMenu;
            schedule = this.FindParentElementOfType<SfSchedule>();
            if (RadialMenu != null && RadialMenu.Items != null)
            {
                foreach (var item in RadialMenu.Items)
                {
                    if (item is SfRadialMenuItem)
                    {
#if WPF
                        (item as SfRadialMenuItem).PreviewMouseLeftButtonDown += AddRadialMenuControl_Click;
                        (item as SfRadialMenuItem).MouseMove += EditRadialMenuControl_PointerMoved;
                        (item as SfRadialMenuItem).MouseLeave += EditRadialMenuControl_PointerExited;
#elif SILVERLIGHT
                        (item as SfRadialMenuItem).MouseLeftButtonDown += AddRadialMenuControl_Click;
                        (item as SfRadialMenuItem).MouseMove += EditRadialMenuControl_PointerMoved;
                        (item as SfRadialMenuItem).MouseLeave += EditRadialMenuControl_PointerExited;
#elif WINRT
                        (item as SfRadialMenuItem).Click += AddRadialMenuControl_Click;
                        (item as SfRadialMenuItem).PointerMoved += EditRadialMenuControl_PointerMoved;
                        (item as SfRadialMenuItem).PointerExited += EditRadialMenuControl_PointerExited;
#endif
                    }
                }
            }
            base.OnApplyTemplate();
        }

        #endregion

        #region Events

#if WINRT
        void EditRadialMenuControl_PointerMoved(object sender, PointerRoutedEventArgs e) 
#else
        void EditRadialMenuControl_PointerMoved(object sender, RoutedEventArgs e) 
#endif
        {
            var path = ((sender as SfRadialMenuItem).Header as Border).FindElementOfType<Path>();
            if (path != null)
                path.Fill = new SolidColorBrush(Color.FromArgb(0XFF, 0X28, 0XA5, 0XDB));
        }

#if WINRT
        void EditRadialMenuControl_PointerExited(object sender, PointerRoutedEventArgs e) 
#else
        void EditRadialMenuControl_PointerExited(object sender, RoutedEventArgs e)
#endif
        {
            var path = ((sender as SfRadialMenuItem).Header as Border).FindElementOfType<Path>();
            if (path != null)
                path.Fill = new SolidColorBrush(Color.FromArgb(0XFF, 0X48, 0X49, 0X49));
        }

        void AddRadialMenuControl_Click(object sender, RoutedEventArgs e)
        {
            if (RadialMenu.Items != null)
            {
                int index = RadialMenu.Items.IndexOf(sender);
                switch (index)
                {
                    case 0:
                        {
                            schedule.AddNewAppointment();                            
                            break;
                        }
                    case 1:
                        {
                            schedule.EditAppointment();
                            break;
                        }
                    case 2:
                        {
                            if (schedule.CopiedAppointment == null)
                            {
                                var editRadialMenuControl = schedule.editpopup.Child as EditRadialMenuControl;
                                if (editRadialMenuControl != null && editRadialMenuControl.RadialMenu.Items != null && editRadialMenuControl.RadialMenu.Items[3] is SfRadialMenuItem)
                                {
                                    (editRadialMenuControl.RadialMenu.Items[3] as SfRadialMenuItem).IsEnabled = true;
                                    (editRadialMenuControl.RadialMenu.Items[3] as SfRadialMenuItem).Opacity = 1;
                                }
                            }
                            schedule.CopyAppointment();
                            break;
                        }
                    case 3:
                        {
                            if (schedule != null && schedule.CopiedAppointment != null)
                            {
                                var app = (ScheduleAppointment)schedule.AppointmentCloning(schedule.CopiedAppointment);
                                if (schedule.SelectedAppointment != null)
                                {
                                    schedule.SelectedAppointment.IsSelected = false;
                                    schedule.SelectedAppointment.AppointmentSelectionBrush = new SolidColorBrush(Colors.Transparent);
                                }
#if WINRT
                                DateTime date = schedule.ScheduleType == ScheduleType.Month ? schedule.Currentselecteddate : schedule.GetDate(schedule.SelectedPoint);
#else
                                DateTime date = schedule.Currentselecteddate;
#endif
                                DateTime startime = schedule.CopiedAppointment.StartTime;
                                DateTime endtime = schedule.CopiedAppointment.EndTime;

                                if (app.ResourceCollection.Count == 0)
                                {
                                    foreach (Resource selectedresource in schedule.selectedResourcename)
                                    {
                                        app.ResourceCollection.Add(selectedresource);
                                    }
                                }
                                else if (app.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource) != null)
                                {
                                    foreach (Resource selectedresource in schedule.selectedResourcename)
                                    {
                                        var firstOrDefault = app.ResourceCollection.FirstOrDefault(res => res.TypeName == selectedresource.TypeName);
                                        if (firstOrDefault != null)
                                            firstOrDefault.ResourceName = selectedresource.ResourceName;
                                    }
                                }
                                else
                                {
                                    foreach (Resource selectedresource in schedule.selectedResourcename)
                                    {
                                        app.ResourceCollection.Add(selectedresource);
                                    }
                                }
                                if (schedule.ScheduleType == ScheduleType.Month)
                                {
                                    if (startime == endtime)
                                    {
                                        app.AllDay = true;
                                    }
                                    app.StartTime = schedule.Currentselecteddate.AddMinutes(startime.Minute);
                                    app.EndTime = schedule.Currentselecteddate.Add(endtime - startime);
                                }

#if WINRT
                                else if (schedule.allDayFlag)
                                {
                                    app.AllDay = true;
                                    app.StartTime = date;
                                    app.EndTime = date;
                                }

#else
                                else if (schedule.allDayTouch)
                                {
                                    schedule.allDayTouch = false;
                                    app.AllDay = true;
                                    app.StartTime = date;
                                    app.EndTime = date;
                                }
#endif
                                else
                                {
                                    app.StartTime = date;
                                    if (endtime != startime)
                                    {
                                        app.EndTime = date.Add(endtime - startime);
                                    }
                                    else
                                    {
                                        app.EndTime = date.Add(schedule.GetTimeInterval());
                                    }
                                }
                                schedule.SelectedAppointment = app;
                                app.IsSelected = true;
                                app.AppointmentSelectionBrush = schedule.AppointmentSelectionBrush;
                                schedule.Appointments.Add(app);
                                schedule.addnewpopup.IsOpen = false;

                            }
                            break;
                        }
                    case 4:
                        {
                            schedule.DeleteAppointment();
                            break;
                        }
                    case 5:
                        {
#if SILVERLIGHT
                            schedule.contextMenuVisible = true;
#endif
                            schedule.ResizeAppointment();
                            break;
                        }
                }
            }
            var path = ((sender as SfRadialMenuItem).Header as Border).FindElementOfType<Path>();
            if (path != null)
                path.Fill = new SolidColorBrush(Color.FromArgb(0XFF, 0X48, 0X49, 0X49));
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if (RadialMenu != null && RadialMenu.Items != null)
            {
                foreach (var item in RadialMenu.Items)
                {
                    if (item is SfRadialMenuItem)
#if WPF
                        (item as SfRadialMenuItem).PreviewMouseLeftButtonDown -= AddRadialMenuControl_Click;
#elif SILVERLIGHT
                        (item as SfRadialMenuItem).MouseLeftButtonDown -= AddRadialMenuControl_Click;
#elif WINRT
                        (item as SfRadialMenuItem).Click -= AddRadialMenuControl_Click;
#endif
                }
            }
        }

        #endregion
    }

    #endregion
}
