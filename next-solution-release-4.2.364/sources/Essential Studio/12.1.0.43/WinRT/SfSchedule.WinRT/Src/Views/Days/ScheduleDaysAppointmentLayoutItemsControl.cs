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
    /// Represents a layout items control for arranging appointments in day view.
    /// </summary>
    public class ScheduleDaysAppointmentLayoutItemsControl : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleDaysAppointmentLayoutItemsControl">ScheduleDaysAppointmentLayoutItemsControl</see>
        /// class.
        /// </summary>
        public ScheduleDaysAppointmentLayoutItemsControl()
        {
            DefaultStyleKey = typeof(ScheduleDaysAppointmentLayoutItemsControl);
        }

        #endregion

        #region Dependency Properties

        #region AppointmentSelectionBrush

        /// <summary>
        /// Gets or sets the border color of day view appointment while it gets selected.
        /// </summary>
        public Brush AppointmentSelectionBrush
        {
            get { return (Brush)GetValue(AppointmentSelectionBrushProperty); }
            internal set { SetValue(AppointmentSelectionBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentSelectionBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(ScheduleDaysAppointmentLayoutItemsControl), new PropertyMetadata(null));

        #endregion

        #region AppointmentTemplate
        /// <summary>
        /// Gets or sets the template for customizing day view appointment.
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
            DependencyProperty.Register("AppointmentTemplate", typeof(DataTemplate), typeof(ScheduleDaysAppointmentLayoutItemsControl), new PropertyMetadata(null));
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
            DependencyProperty.Register("AppointmentTooltipVisibility", typeof(Visibility), typeof(ScheduleDaysAppointmentLayoutItemsControl), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region AppointmentToolTipTemplate
        /// <summary>
        /// Gets or sets a template for customizing appointment tooltip.
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
            DependencyProperty.Register("AppointmentToolTipTemplate", typeof(ControlTemplate), typeof(ScheduleDaysAppointmentLayoutItemsControl), new PropertyMetadata(null));
        #endregion
#endif

        #endregion

        #region Overrides

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

            var appointmentTooltipVisibilityBinding = new Binding
            {
                Path = new PropertyPath("AppointmentTooltipVisibility"),
                Source = this
            };
            view.SetBinding(ScheduleDaysAppointmentViewControl.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
            var appointmentToolTipTemplateBinding = new Binding
                {
                    Path = new PropertyPath("AppointmentToolTipTemplate"),
                    Source = this
                };
            view.SetBinding(ScheduleDaysAppointmentViewControl.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
#endif
            return view;
        }

        #endregion

        #region IsItemItsOwnContainerOverride

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ScheduleDaysAppointmentViewControl);
        }

        #endregion

        #endregion
    }
}
