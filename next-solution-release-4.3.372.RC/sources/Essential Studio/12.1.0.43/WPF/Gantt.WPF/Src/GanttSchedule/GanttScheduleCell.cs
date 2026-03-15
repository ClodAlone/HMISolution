#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Gantt.Schedule
{
    /// <summary>
    /// Represents a control that displays each measurement unit.
    /// </summary>
    public class GanttScheduleCell : ContentControl
    {
        #region Dependency Registration

        // Using a DependencyProperty as the backing store for CellDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellDateProperty =
            DependencyProperty.Register("CellDate", typeof(DateTime), typeof(GanttScheduleCell), new PropertyMetadata(DateTime.Today));

        // Using a DependencyProperty as the backing store for Cell Tooltip.  This enables animation, styling, binding, etc...
        public static DependencyProperty CellToolTipProperty =
            DependencyProperty.Register("CellToolTip", typeof(object), typeof(GanttScheduleCell), new PropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for CellTimeUnit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTimeUnitProperty =
            DependencyProperty.Register("CellTimeUnit", typeof(TimeUnit), typeof(GanttScheduleCell), new PropertyMetadata(TimeUnit.Days));
        

        #endregion

        #region Public Properties

        #region Cell Date Property

        /// <summary>
        /// Gets or sets the cell date.
        /// </summary>
        /// <value>
        /// The cell date.
        /// </value>
        public DateTime CellDate
        {
            get { return (DateTime)GetValue(CellDateProperty); }
            internal set { SetValue(CellDateProperty, value); }
        }

        #endregion

        #region Cell ToolTip Text Property
        /// <summary>
        /// Gets or sets the cell tool tip.
        /// </summary>
        /// <value>
        /// The cell tool tip.
        /// </value>
        public object CellToolTip
        {
            get
            {
                return (object)GetValue(CellToolTipProperty);
            }
            set
            {
                SetValue(CellToolTipProperty, value);
            }
        }

        #endregion

        #region Cell Time Unit Property

        /// <summary>
        /// Gets or sets the cell time unit.
        /// </summary>
        /// <value>
        /// The cell time unit.
        /// </value>
        public TimeUnit CellTimeUnit
        {
            get { return (TimeUnit)GetValue(CellTimeUnitProperty); }
            internal set { SetValue(CellTimeUnitProperty, value); }
        }

        #endregion

#if SILVERLIGHT
        #region Cell BorderBrush, Foreground properties

        public Brush CellBorderBrush
        {
            get { return (Brush)GetValue(CellBorderBrushProperty); }
            set { SetValue(CellBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellBorderBrushProperty =
            DependencyProperty.Register("CellBorderBrush", typeof(Brush), typeof(GanttScheduleCell), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush CellForeground
        {
            get { return (Brush)GetValue(CellForegroundProperty); }
            set { SetValue(CellForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellForegroundProperty =
            DependencyProperty.Register("CellForeground", typeof(Brush), typeof(GanttScheduleCell), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion
#endif
        #endregion

        #region Constructors and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="GanttScheduleCell"/> class.
        /// </summary>
        public GanttScheduleCell()
        {
            DefaultStyleKey = GetType();
#if !SyncfusionFramework3_5
            UseLayoutRounding = false;
#endif           
        }

        #endregion
    }
}