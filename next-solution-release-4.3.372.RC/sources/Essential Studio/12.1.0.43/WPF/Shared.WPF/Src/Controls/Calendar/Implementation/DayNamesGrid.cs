// <copyright file="DayNamesGrid.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a grid that consists of columns
    /// that contain day name cell elements.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DayNamesGrid : FrameworkElement
    {
        #region Constants

        /// <summary>
        /// Default number of columns.
        /// </summary>
        private const int DEFCOLUMNSCOUNT = 7;

        #endregion Constants

        #region Private members

        /// <summary>
        /// Inner grid used for layout logic.
        /// </summary>
        private Grid minnerGrid;

        /// <summary>
        /// Parent instance.
        /// </summary>
        private CalendarEdit mparentCalendar;

        /// <summary>
        /// Collection of cells.
        /// </summary>
        private ArrayList mdayNameCells;

        #endregion Private members

        #region Initilization

        /// <summary>
        /// Initializes static members of the DayNamesGrid class. Overrides some dependency properties.
        /// </summary>
        static DayNamesGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DayNamesGrid), new FrameworkPropertyMetadata(typeof(DayNamesGrid)));
        }

        /// <summary>
        /// Initializes a new instance of the DayNamesGrid class.
        /// </summary>
        public DayNamesGrid()
        {
            this.minnerGrid = new Grid();
            this.DayNameCells = new ArrayList();
            this.GenerateGrid();
            this.FillGrid();
            this.AddLogicalChild(this.minnerGrid);
            this.AddVisualChild(this.minnerGrid);
            BindingUtils.SetRelativeBinding(this.minnerGrid, FrameworkElement.FlowDirectionProperty, typeof(CalendarEdit), FrameworkElement.FlowDirectionProperty, BindingMode.OneWay, 1);
        }

        #endregion Initilization

        #region Properties

        /// <summary>
        /// Gets or sets the collection of cells.
        /// </summary>
        /// <value>
        /// Type: <see cref="ArrayList"/>
        /// </value>
        /// <seealso cref="ArrayList"/>
        public ArrayList DayNameCells
        {
            get
            {
                return this.mdayNameCells;
            }

            set
            {
                this.mdayNameCells = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// Type: <see cref="CalendarEdit"/>
        /// </value>
        /// <seealso cref="CalendarEdit"/>
        public CalendarEdit ParentCalendar
        {
            get
            {
                return this.mparentCalendar;
            }

            set
            {
                this.mparentCalendar = value;
            }
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Sets the cell's content.
        /// </summary>
        /// <param name="format">The format.</param>
        protected internal void SetDayNames(DateTimeFormatInfo format)
        {
            if (this.ParentCalendar != null)
            {
                int ifirstDayOfWeek = (int)format.FirstDayOfWeek;
                string[] dayNames;

                if (this.ParentCalendar.IsDayNamesAbbreviated)
                {
                    dayNames = format.ShortestDayNames;
                }
                else
                {
                    dayNames = format.DayNames;
                }

                int curDay;

                for (int i = 0; i < this.DayNameCells.Count; i++)
                {
                    curDay = i + ifirstDayOfWeek;
                    curDay %= 7;
                    (this.DayNameCells[i] as DayNameCell).Content = dayNames[curDay];
                }
            }
        }

        /// <summary>
        /// Updates data template and data template selector on all
        /// cells.
        /// </summary>
        /// <param name="template">Data template to be set to the cell.
        /// If it is null the local value of data template would be cleared.</param>
        /// <param name="selector">Data template selector to be set to
        /// the cell. If it is null the local value would be cleared.</param>
        protected internal void UpdateTemplateAndSelector(DataTemplate template, DataTemplateSelector selector)
        {
            if (template != null && selector != null)
            {
                throw new ArgumentException("Both template and selector can not be set at one time.");
            }

            foreach (DayNameCell dnc in this.DayNameCells)
            {
                dnc.UpdateCellTemplateAndSelector(template, selector);
            }
        }

        /// <summary>
        /// Updates styles on all cells.
        /// </summary>
        /// <param name="style">Style to be set.</param>
        protected internal void UpdateStyles(Style style)
        {
            foreach (DayNameCell dnc in this.DayNameCells)
            {
                dnc.SetStyle(style);
            }
        }

        /// <summary>
        /// Fills the grid with column definitions and row definitions.
        /// </summary>
        protected void GenerateGrid()
        {
            // Define Columns
            ArrayList columns = new ArrayList();

            for (int i = 0; i < DEFCOLUMNSCOUNT; i++)
            {
                ColumnDefinition cdcolumn = new ColumnDefinition();
                cdcolumn.Width = new GridLength(1, GridUnitType.Star);
                columns.Add(cdcolumn);
            }

            foreach (ColumnDefinition cd in columns)
            {
                this.minnerGrid.ColumnDefinitions.Add(cd);
            }

            RowDefinition rdrow = new RowDefinition();
            rdrow.Height = new GridLength(15, GridUnitType.Pixel);
            this.minnerGrid.RowDefinitions.Add(rdrow);
        }

        /// <summary>
        /// Adds day and day name cells to the grid.
        /// </summary>
        protected void FillGrid()
        {
            // adding DayNameCells
            for (int i = 0; i < DEFCOLUMNSCOUNT; i++)
            {
                DayNameCell dncCell = new DayNameCell();
                this.DayNameCells.Add(dncCell);
            }

            for (int i = 0; i < this.DayNameCells.Count; i++)
            {
                Grid.SetRow((UIElement)this.DayNameCells[i], 0);
                Grid.SetColumn((UIElement)this.DayNameCells[i], i);
                this.minnerGrid.Children.Add((UIElement)this.DayNameCells[i]);
            }
        }

        /// <summary>
        /// Updates the parent.
        /// </summary>
        protected void UpdateParent()
        {
            if (this.Parent == null)
            {
                throw new NullReferenceException("Parent is null");
            }

            CalendarEdit calendar = this.Parent as CalendarEdit;

            if (calendar == null)
            {
                throw new NotSupportedException("Parent must be inherited from CalendarEdit type.");
            }

            this.ParentCalendar = calendar;
        }

        #endregion Implementation

        #region Overrides

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The index of the visual object in the
        /// VisualCollection.</param>
        /// <exception cref="ArgumentOutOfRangeException">Index must be 0 because only one child element is present.</exception>
        /// <returns>
        /// The child in the VisualCollection at the specified index
        /// value.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException("only one child element present");
            }

            return (Visual)this.minnerGrid;
        }

        /// <summary>
        /// Measures the size in layout required for child elements and
        /// determines a size for the control.
        /// </summary>
        /// <param name="availableSize">The available size that this
        /// element can give to the child. Infinity can be specified as a
        /// value to indicate that the element will size to whatever
        /// content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout,
        /// based on its calculations of children's sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            this.minnerGrid.Measure(availableSize);
            return this.minnerGrid.DesiredSize;
        }

        /// <summary>
        /// Positions child elements and determines a size for the control.
        /// </summary>
        /// <param name="finalSize">The final area within the parent
        /// that this element should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.minnerGrid.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }

        /// <summary>
        /// Invoked when the parent is changed.
        /// </summary>
        /// <param name="oldParent">The previous parent. Set to a null
        /// reference (Nothing in Visual Basic) if the DependencyObject did not have
        /// a previous parent.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            this.UpdateParent();
            CultureInfo ci = this.ParentCalendar.Culture;
            DateTimeFormatInfo format = ci.DateTimeFormat;
            this.SetDayNames(format);
        }

        #endregion Overrides
    }
}