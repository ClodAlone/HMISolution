#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a uniform grid.
    /// </summary>
    public class UniformGrid : Panel
    {
        #region Private Fields

        private int ComputedRows { get; set; }
        private int ComputedColumns { get; set; }
        internal double maxElementWidth = 0d;

        #endregion

        #region Dependency Properties

        #region Rows
        /// <summary>
        /// Gets or sets the number of rows in a grid.
        /// </summary>
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        /// <summary>
        ///Using a DependencyProperty as the backing store for Rows.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(int), typeof(UniformGrid), new PropertyMetadata(0, OnIntegerDependencyPropertyChanged));
        #endregion

        #region Columns
        /// <summary>
        /// Gets or sets the number of columns in a grid.
        /// </summary>
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(UniformGrid), new PropertyMetadata(0, OnIntegerDependencyPropertyChanged));

        private static void OnIntegerDependencyPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            // Silently coerce the value back to >= 0 if negative.
            if (!(e.NewValue is int) || (int)e.NewValue < 0)
            {
                o.SetValue(e.Property, e.OldValue);
            }
        }

        #endregion

        #region FirstColumn
        /// <summary>
        /// Gets or sets the fist column in a grid.
        /// </summary>
        public int FirstColumn
        {
            get { return (int)GetValue(FirstColumnProperty); }
            set { SetValue(FirstColumnProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for FirstColumn.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FirstColumnProperty =
            DependencyProperty.Register("FirstColumn", typeof(int), typeof(UniformGrid), new PropertyMetadata(0, OnIntegerDependencyPropertyChanged));

        #endregion

        #region ParentScrollVisibleHeight
        internal double ParentScrollVisibleHeight
        {
            get { return (double)GetValue(ParentScrollVisibleHeightProperty); }
            set { SetValue(ParentScrollVisibleHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentScrollVisibleHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentScrollVisibleHeightProperty =
            DependencyProperty.Register("ParentScrollVisibleHeight", typeof(double), typeof(UniformGrid), new PropertyMetadata(0d));
        #endregion

        #endregion

        #region Overrides

        #region MeasureOverride

        protected override Size MeasureOverride(Size constraint)
        {
            UpdateComputedValues();

            if (double.IsInfinity(constraint.Height))
            {
                constraint.Height = (constraint.Width / 7) * (ComputedRows);
            }
            var childSize = new Size(constraint.Width / ComputedColumns, (constraint.Height) / ComputedRows);
            foreach (UIElement child in Children)
            {
                child.Measure(constraint);
                maxElementWidth = Math.Max(maxElementWidth, child.DesiredSize.Width);
            }
            foreach (UIElement child in Children)
            {
                child.Measure(childSize);
            }
            var scheduleMonthViewItemsControl = this.FindParentElementOfType<ScheduleMonthViewItemsControl>();
            if (scheduleMonthViewItemsControl.schedule != null)
            {
                var schedule = scheduleMonthViewItemsControl.schedule;
                schedule.needAutoFormat = (childSize.Width <= maxElementWidth);
                scheduleMonthViewItemsControl.AutoHeaderFormat = (schedule.EnableAutoFormat && schedule.needAutoFormat) ? "dd" : schedule.MonthHeaderDateFormat;
            }
            return constraint;
        }

        #endregion

        #region ArrangeOverride

        /// <summary>
        /// Arrange the children of the UniformGrid by distributing space evenly
        /// among the children, making each child the size equal to a cell
        /// portion of the arrangeSize parameter.
        /// </summary>
        /// <param name="arrangeSize">The arrange size.</param>
        /// <returns>Returns the updated Size.</returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var childBounds = new Rect(0, 0, arrangeSize.Width / ComputedColumns, (arrangeSize.Height) / ComputedRows);
            double xStep = childBounds.Width;
            double xBound = arrangeSize.Width - 1.0;
            childBounds.X += childBounds.Width * FirstColumn;
            var borderthickness = new Thickness(3, 2, 0, 2);
            var Lastitemborderthickness = new Thickness(3, 2, 2, 2);
            // Arrange and Position each child to the same cell size
            foreach (UIElement child in Children)
            {
                child.Arrange(childBounds);
                if (child.Visibility != Visibility.Collapsed)
                {
                    childBounds.X += xStep;
                    if (childBounds.X >= xBound)
                    {
                        var scheduleMonthDateContentControl = child as ScheduleMonthDateContentControl;
                        if (scheduleMonthDateContentControl != null)
                            scheduleMonthDateContentControl.BorderThickness = Lastitemborderthickness;
                        childBounds.Y += childBounds.Height;
                        childBounds.X = 0;
                    }
                    else
                    {
                        var scheduleMonthDateContentControl = child as ScheduleMonthDateContentControl;
                        if (scheduleMonthDateContentControl != null)
                            scheduleMonthDateContentControl.BorderThickness = borderthickness;
                    }
                }
            }

            return arrangeSize;
        }

        #endregion

        #endregion

        #region Methods

        #region Update Computed Values

        private void UpdateComputedValues()
        {
            ComputedColumns = Columns;
            ComputedRows = Rows;

            // Reset the first column.
            if (FirstColumn >= ComputedColumns)
            {
                FirstColumn = 0;
            }

            if ((ComputedRows == 0) || (ComputedColumns == 0))
            {
                int nonCollapsedCount = 0;
                for (int i = 0, count = Children.Count; i < count; ++i)
                {
                    UIElement child = Children[i];
                    if (child.Visibility != Visibility.Collapsed)
                    {
                        nonCollapsedCount++;
                    }
                }
                if (nonCollapsedCount == 0)
                {
                    nonCollapsedCount = 1;
                }
                if (ComputedRows == 0)
                {
                    if (ComputedColumns > 0)
                    {
                        ComputedRows = (nonCollapsedCount + FirstColumn + (ComputedColumns - 1)) / ComputedColumns;
                    }
                    else
                    {
                        ComputedRows = (int)Math.Sqrt(nonCollapsedCount);
                        if ((ComputedRows * ComputedRows) < nonCollapsedCount)
                        {
                            ComputedRows++;
                        }
                        ComputedColumns = ComputedRows;
                    }
                }
                else if (ComputedColumns == 0)
                {
                    ComputedColumns = (nonCollapsedCount + (ComputedRows - 1)) / ComputedRows;
                }

                if ((ComputedColumns * ComputedRows) > Children.Count)
                {
                    if (ComputedColumns * (ComputedRows - 1) >= Children.Count)
                    {
                        ComputedRows -= 1;
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
