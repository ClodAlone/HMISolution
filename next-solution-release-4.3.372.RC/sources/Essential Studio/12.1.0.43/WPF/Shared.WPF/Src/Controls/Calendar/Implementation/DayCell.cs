// <copyright file="DayCell.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a day cell of the <see cref="Syncfusion.Windows.Shared.CalendarEdit"/> control.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class DayCell : Cell
    {
        #region Dependency properties

        /// <summary>
        /// Identifies <see cref="IsCurrentMonth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCurrentMonthProperty =
            DependencyProperty.Register("IsCurrentMonth", typeof(bool), typeof(DayCell), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies <see cref="IsToday"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTodayProperty =
            DependencyProperty.Register("IsToday", typeof(bool), typeof(DayCell), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies <see cref="IsDate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDateProperty =
            DependencyProperty.Register("IsDate", typeof(bool), typeof(DayCell), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies <see cref="Date"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(Date), typeof(DayCell), new UIPropertyMetadata(new Date()));

        #endregion Dependency properties

        #region Initialization

        /// <summary>
        /// Initializes static members of the DayCell class. It overrides some dependency properties.
        /// </summary>
        static DayCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DayCell), new FrameworkPropertyMetadata(typeof(DayCell)));
        }

        #endregion Initialization

        #region Routed events

        /// <summary>
        /// Identifies <see cref="Highlight"/> routed event.
        /// </summary>
        public static readonly RoutedEvent HighlightEvent = EventManager.RegisterRoutedEvent(
            "Highlight",
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(DayCell));

        /// <summary>
        /// Bubbling routed event, fired when control needs to be
        /// highlighted.
        /// </summary>
        public event RoutedEventHandler Highlight
        {
            add
            {
                AddHandler(HighlightEvent, value);
            }

            remove
            {
                RemoveHandler(HighlightEvent, value);
            }
        }

        #endregion Routed events

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the cell date
        /// belongs to the current month. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is true.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsCurrentMonth
        {
            get
            {
                return (bool)GetValue(IsCurrentMonthProperty);
            }

            set
            {
                SetValue(IsCurrentMonthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cell date
        /// is equal to today date.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsToday
        {
            get
            {
                return (bool)GetValue(IsTodayProperty);
            }

            set
            {
                SetValue(IsTodayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cell date
        /// is equal to FirstDay of the Month
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsFirstDayofMonth
        {
            get { return (bool)GetValue(IsFirstDayofMonthProperty); }
            set { SetValue(IsFirstDayofMonthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFirstDayofMonth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFirstDayofMonthProperty =
            DependencyProperty.Register("IsFirstDayofMonth", typeof(bool), typeof(DayCell), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the cell date is equal to
        /// <see cref="Syncfusion.Windows.Shared.CalendarEdit"/> date.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsDate
        {
            get
            {
                return (bool)GetValue(IsDateProperty);
            }

            set
            {
                SetValue(IsDateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the cell date.
        /// </summary>
        /// <value>
        /// Type: <see cref="Date"/>
        /// </value>
        /// <seealso cref="Date"/>
        public Date Date
        {
            get
            {
                return (Date)GetValue(DateProperty);
            }

            set
            {
                SetValue(DateProperty, value);
            }
        }

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Updates data template and data template selector of the cell.
        /// </summary>
        /// <param name="template">Data template to be set to the cell.
        /// If it is null the local value of data template would be cleared.</param>
        /// <param name="selector">Data template selector to be set to
        /// the cell. If it is null the local value would be cleared.</param>
        /// <remarks>
        /// Both template and selector can not be set at the same time.
        /// </remarks>
        protected internal void UpdateCellTemplateAndSelector(DataTemplate template, DataTemplateSelector selector)
        {
            Debug.Assert(template == null || selector == null, "Both template and selector can not be set at one time.");

            if (selector != null)
            {
                ContentTemplateSelector = selector;
            }
            else
            {
                ClearValue(DayCell.ContentTemplateSelectorProperty);
            }

            if (template != null)
            {
                ContentTemplate = template;
            }
            else
            {
                if (selector == null)
                {
                    ClearValue(DayCell.ContentTemplateProperty);
                }
                else
                {
                    ContentTemplate = null;
                }
            }
        }

        /// <summary>
        /// Sets DayCell template property.
        /// </summary>
        /// <param name="template">Template to be set.</param>
        protected internal void SetTemplate(DataTemplate template)
        {
            if (template != null)
            {
                ClearValue(ContentTemplateSelectorProperty);
                ContentTemplate = template;
            }
        }

        /// <summary>
        /// Sets DayCell style property.
        /// </summary>
        /// <param name="style">Style to be set.</param>
        protected internal void SetStyle(Style style)
        {
            Style = style;
        }

        /// <summary>
        /// Raises the Highlight event.
        /// </summary>
        protected internal void FireHighlightEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(DayCell.HighlightEvent, this);
            RaiseEvent(newEventArgs);
        }

        #endregion Implementation
    }
}