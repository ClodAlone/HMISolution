#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a layout items control for arranging appointments in timeline view.
    /// </summary>
    public class ScheduleHorizontalAppointmentLayoutItemsControl : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleHorizontalAppointmentLayoutItemsControl">ScheduleHorizontalAppointmentLayoutItemsControl</see>
        /// class.
        /// </summary>
        public ScheduleHorizontalAppointmentLayoutItemsControl()
        {
            DefaultStyleKey = typeof(ScheduleHorizontalAppointmentLayoutItemsControl);
        }

        #endregion

        #region Dependency Properties

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
            DependencyProperty.Register("AppointmentTooltipVisibility", typeof(Visibility), typeof(ScheduleHorizontalAppointmentLayoutItemsControl), new PropertyMetadata(Visibility.Collapsed)); 
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
            DependencyProperty.Register("AppointmentToolTipTemplate", typeof(ControlTemplate), typeof(ScheduleHorizontalAppointmentLayoutItemsControl), new PropertyMetadata(null));
        #endregion
#endif

        #region AppointmentTemplate
        /// <summary>
        /// Gets or sets the template for customizing timeline view appointments.
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
            DependencyProperty.Register("AppointmentTemplate", typeof(DataTemplate), typeof(ScheduleHorizontalAppointmentLayoutItemsControl), new PropertyMetadata(null));
        #endregion

        #region ResourceName
        internal string ResourceName
        {
            get { return (string)GetValue(ResourceNameProperty); }
            set { SetValue(ResourceNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResourceName.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ResourceNameProperty =
            DependencyProperty.Register("ResourceName", typeof(string), typeof(ScheduleHorizontalAppointmentLayoutItemsControl), new PropertyMetadata(string.Empty));
        #endregion

        #region AppointmentSelectionBrush
        internal Brush AppointmentSelectionBrush
        {
            get { return (Brush)GetValue(AppointmentSelectionBrushProperty); }
            set { SetValue(AppointmentSelectionBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentSelectionBrush.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(ScheduleHorizontalAppointmentLayoutItemsControl), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Override Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = new ScheduleHorizontalAppointmentViewControl();
            var appointmentbrushbinding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("AppointmentSelectionBrush")
                };
            item.SetBinding(ScheduleHorizontalAppointmentViewControl.AppointmentSelectionBrushProperty, appointmentbrushbinding);
            if (AppointmentTemplate != null)
            {
                var appointmentTemplateBinding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("AppointmentTemplate")
                };
                item.SetBinding(ScheduleHorizontalAppointmentViewControl.AppointmentTemplateProperty, appointmentTemplateBinding);
                item.CustomTemplateVisibility = Visibility.Visible;
                item.DefaultTemplateVisibility = Visibility.Collapsed;
            }
            else
            {
                item.CustomTemplateVisibility = Visibility.Collapsed;
                item.DefaultTemplateVisibility = Visibility.Visible;
            }
#if !WINRT
            var appointmentTooltipVisibilityBinding = new Binding
            {
                Path = new PropertyPath("AppointmentTooltipVisibility"),
                Source =  this
            };
            item.SetBinding(ScheduleHorizontalAppointmentViewControl.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding); 
            var appointmentToolTipTemplateBinding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("AppointmentToolTipTemplate")
                };
            item.SetBinding(ScheduleHorizontalAppointmentViewControl.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding); 
#endif
            return item;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ScheduleHorizontalAppointmentViewControl);
        }

        #endregion
    }
}
