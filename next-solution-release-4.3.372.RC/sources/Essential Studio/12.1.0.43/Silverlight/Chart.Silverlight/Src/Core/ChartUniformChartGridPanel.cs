#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
using System.Globalization;
    
    /// <summary>
    /// UniformChartGrid class is used to arrange Areas with equal sizes based on the Chart's size. It can be also be used a uniform grid panel to arrange items with equal sizes.
    /// </summary>
    public class UniformChartGrid : Panel,IDisposable
    {
        /// <summary>
        /// Dependency property indicates the number of Rows in the UniformChartGrid
        /// </summary>
        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(UniformChartGrid), new PropertyMetadata(0, new PropertyChangedCallback(OnRowsChanged)));

        /// <summary>
        /// Dependency property indicates the number of Columns in the UniformChartGrid
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(UniformChartGrid), new PropertyMetadata(0, new PropertyChangedCallback(OnRowsChanged)));

        /// <summary>
        /// Dependency property indicates the Orientation on which children are to be arranged. 
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(UniformChartGrid), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Gets or sets a value indicating the number of rows in UniformChartGrid
        /// </summary>
        public int Rows
        {
            get
            {
                return (int)GetValue(RowsProperty);
            }

            set
            {
                SetValue(RowsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the number of columns in UniformChartGrid
        /// </summary>
        public int Columns
        {
            get
            {
                return (int)GetValue(ColumnsProperty);
            }

            set
            {
                SetValue(ColumnsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the Orientation of UniformChartGrid
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        #region Private Variables
        /// <summary>
        /// private variable containing the visible children count
        /// </summary>
        private int visiblechildrencount = 0;

        /// <summary>
        /// Custom collection object that contains visible childrens
        /// </summary>
        private UIElementsCollection elementslist = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of the UniformChartGrid class
        /// </summary>
        public UniformChartGrid()
        {
            elementslist = new UIElementsCollection();
        }

        /// <summary>
        /// DependencyPropertyChanged event for Rows and Columns Properties
        /// </summary>
        /// <param name="d">Represents the DependencyObject that triggers the event</param>
        /// <param name="e">Contains the information about the property changes and event arguments</param>
        private static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.NewValue < 0)
            {
                d.SetValue(e.Property, e.OldValue);
            }
            else
            {
                UniformChartGrid grid = (UniformChartGrid)d;
                grid.InvalidateMeasure();
            }
        }

        /// <summary>
        /// DependencyPropertyChanged event for Orientation Property
        /// </summary>
        /// <param name="d">Represents the DependencyObject that triggers the event</param>
        /// <param name="e">Contains the information about the property changes and event arguments</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UniformChartGrid grid = (UniformChartGrid)d;
            grid.InvalidateMeasure();
        }

        /// <summary>
        /// An override method the measures the size the of the children based on the availablesize
        /// </summary>
        /// <param name="availableSize">Represents the available size</param>
        /// <returns>available size</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsNaN(availableSize.Width) == true || availableSize.Width <= 0 || double.IsPositiveInfinity(availableSize.Width) ||
                double.IsNaN(availableSize.Height) || availableSize.Height <= 0 || double.IsPositiveInfinity(availableSize.Height))
            {
                Grid parentgrid = GetGrid();
                parentgrid.SizeChanged += new SizeChangedEventHandler(parentgrid_SizeChanged);
                if (parentgrid.ActualWidth == 0 && parentgrid.ActualHeight == 0)
                {
                    availableSize = new Size(100, 100);
                }
                else if (parentgrid.ActualWidth == 0 || parentgrid.ActualHeight == 0)
                {
                    availableSize = parentgrid.ActualWidth == 0 ? new Size(100, parentgrid.ActualHeight) : new Size(parentgrid.ActualWidth, 100);
                }
                else
                {
                    availableSize = new Size(parentgrid.ActualWidth, parentgrid.ActualHeight);
                }
            }

            Chart chart = GetChart();
            if (chart != null)
            {
                if (chart.Rows != 0 && chart.Columns != 0 && chart.Rows * chart.Columns >= chart.Areas.Count)
                {
                    this.Rows = chart.Rows;
                    this.Columns = chart.Columns;
                    this.Orientation = chart.Orientation;
                }
            }

            CalculateRowsAndColumn();
            double childwidth = availableSize.Width / Columns;
            double childheight = availableSize.Height / Rows;
            Size childsize = new Size(childwidth, childheight);
            double maxwidth = 0;
            double maxheight = 0;

            foreach (UIElement element in elementslist)
            {
                element.Measure(childsize);
                maxheight = Math.Max(maxheight, element.DesiredSize.Height);
                maxwidth = Math.Max(maxwidth, element.DesiredSize.Width);
            }

            Grid grid = GetGrid();
            if (grid != null)
            {
                RectangleGeometry clip = new RectangleGeometry();
                clip.Rect = new Rect(0, 0, availableSize.Width, availableSize.Height);
                grid.Clip = clip;
            }

            return base.MeasureOverride(new Size(maxwidth * Columns, maxheight * Rows));
        }
        void parentgrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateMeasure();
            this.InvalidateArrange();
        }

        internal Chart GetChart()
        {
            DependencyObject element = this;
            while (!(element is Chart) && element != null)
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                return element as Chart;
            }

            return null;
        }

        internal Grid GetGrid()
        {
            DependencyObject element = this;
            while (!(element is Grid) && element != null)
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                return element as Grid;
            }

            return null;
        }

        /// <summary>
        /// An override method the arranges all the visible children based on the child size and orientation
        /// </summary>
        /// <param name="finalSize">Represents the fina;l size</param>
        /// <returns>final size</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size childsize = new Size(finalSize.Width / Columns, finalSize.Height / Rows);
            Rect bounds = new Rect(new Point(0, 0), childsize);

            int rowcnt = 0;
            int colcnt = 0;

            foreach (UIElement element in elementslist)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    if (colcnt == Columns)
                    {
                        colcnt = 0;
                        rowcnt += 1;
                    }

                    element.Arrange(new Rect((childsize.Width * colcnt), (childsize.Height * rowcnt), childsize.Width, childsize.Height));
                    colcnt += 1;
                }
                else
                {
                    if (rowcnt == Rows)
                    {
                        rowcnt = 0;
                        colcnt += 1;
                    }

                    element.Arrange(new Rect((childsize.Width * colcnt), (childsize.Height * rowcnt), childsize.Width, childsize.Height));
                    rowcnt += 1;
                }
            }

            RectangleGeometry chartclip = new RectangleGeometry();
            chartclip.Rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
            this.Clip = chartclip;
            return finalSize;
        }

        /// <summary>
        /// An helper method to calculate the number of rows and columns
        /// </summary>
        private void CalculateRowsAndColumn()
        {
            elementslist = new UIElementsCollection((from child in this.Children
                                                     where child.Visibility != Visibility.Collapsed
                                                     select child).ToList<UIElement>());

            visiblechildrencount = elementslist.Count;

            if (visiblechildrencount > 0)
            {
                if (Columns == 0 && Rows == 0)
                {
                    Rows = (int)Math.Sqrt(visiblechildrencount);
                    if ((Rows * Rows) < visiblechildrencount)
                    {
                        Rows += 1;
                    }

                    Columns = Rows;
                }
                else if ((Rows * Columns) < visiblechildrencount)
                {
                    Rows = ((int)Math.Sqrt(visiblechildrencount)) + 1;
                    Columns = Rows;
                }
                else
                {
                    if (Rows == 0)
                    {
                        Rows = visiblechildrencount / Columns;
                    }
                    else if (Columns == 0)
                    {
                        Columns = visiblechildrencount / Rows;
                    }
                }
            }
            else
            {
                Rows = 1;
                Columns = 1;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.elementslist != null)
            {
                this.elementslist.Clear();
                this.elementslist = null;
            }
            this.Children.Clear();
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// UIElementsCollection represents a ObservableCollection of type UIElement
    /// </summary>
    public class UIElementsCollection : ObservableCollection<UIElement>
    {
        /// <summary>
        /// Initializes a new instance of the UIElementsCollection class
        /// </summary>
        public UIElementsCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UIElementsCollection class
        /// </summary>
        /// <param name="elements">Represents elements collection to be added the collection</param>
        public UIElementsCollection(IEnumerable<UIElement> elements)
        {
            foreach (UIElement element in elements)
            {
                this.Add(element);
            }
        }
    }


    /// <summary>
    /// Arranges child elements around the edges of the panel.  Optionally, 
    /// last added child element can occupy the remaining space.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public class ChartDockPanel : Panel
    {
        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private static bool _ignorePropertyChange;

        #region public bool LastChildFill
        /// <summary>
        /// Get or Set LastChildFill property 
        /// </summary>
        public bool LastChildFill
        {
            get { return (bool)GetValue(LastChildFillProperty); }
            set { SetValue(LastChildFillProperty, value); }
        }

        /// <summary>
        /// Identifies the LastChildFill dependency property.
        /// </summary>
        public static readonly DependencyProperty LastChildFillProperty =
            DependencyProperty.Register(
                "LastChildFill",
                typeof(bool),
                typeof(ChartDockPanel),
                new PropertyMetadata(true, OnLastChildFillPropertyChanged));

        /// <summary>
        /// LastChildFillProperty property changed handler.
        /// </summary>
        /// <param name="d">DockPanel that changed its LastChildFill.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnLastChildFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartDockPanel source = d as ChartDockPanel;
            source.InvalidateArrange();
        }
        #endregion public bool LastChildFill

        #region public attached ChartDock ChartDock
        /// <summary>
        /// Return dock value based upon the Given UIElement
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ChartDock GetDock(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (ChartDock)element.GetValue(DockProperty);
        }

        /// <summary>
        /// set the Dock property to given UIElement
        /// </summary>
        /// <param name="element"></param>
        /// <param name="dock"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void SetDock(UIElement element, ChartDock dock)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(DockProperty, dock);
        }

        /// <summary>
        /// Identifies the Dock dependency property.
        /// </summary>
        public static readonly DependencyProperty DockProperty =
            DependencyProperty.RegisterAttached(
                "Dock",
                typeof(ChartDock),
                typeof(ChartDockPanel),
                new PropertyMetadata(ChartDock.Left, OnDockPropertyChanged));

        /// <summary>
        /// DockProperty property changed handler.
        /// </summary>
        /// <param name="d">UIElement that changed its ChartDock.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnDockPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Ignore the change if requested
            if (_ignorePropertyChange)
            {
                _ignorePropertyChange = false;
                return;
            }

            UIElement element = (UIElement)d;
            ChartDock value = (ChartDock)e.NewValue;

            // Validate the ChartDock property
            if ((value != ChartDock.Left) &&
                (value != ChartDock.Top) &&
                (value != ChartDock.Right) &&
                (value != ChartDock.Bottom))
            {
                // Reset the property to its original state before throwing
                _ignorePropertyChange = true;
                element.SetValue(DockProperty, (ChartDock)e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Invalid Dock value '{0}'.",
                    value);
                throw new ArgumentException(message, "value");
            }

            // Cause the DockPanel to update its layout when a child changes
            ChartDockPanel panel = VisualTreeHelper.GetParent(element) as ChartDockPanel;
            if (panel != null)
            {
                panel.InvalidateMeasure();
            }
        }
        #endregion public attached ChartDock ChartDock

        
        
        
        #region Dependency Property
        internal static readonly DependencyProperty HostProperty = DependencyProperty.Register("Host", typeof(string), typeof(ChartDockPanel), new PropertyMetadata(string.Empty));
        /// <summary>
        /// Gets or sets the sync chart area.
        /// </summary>
        /// <value>The sync chart area.</value>
        internal string Host
        {
            set { SetValue(HostProperty, value); }
            get { return (string)GetValue(HostProperty); }
        }
        #endregion

        internal Grid GetGrid()
        {
            DependencyObject element = this;
            while (!(element is Grid) && element != null)
            {
                element = VisualTreeHelper.GetParent(element);
            }

            if (element != null)
            {
                return element as Grid;
            }

            return null;
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="constraint">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size constraint)
        {
            double usedWidth = 0.0;
            double usedHeight = 0.0;
            double maximumWidth = 0.0;
            double maximumHeight = 0.0;

            if ((double.IsNaN(constraint.Width) == true || constraint.Width <= 0 || double.IsPositiveInfinity(constraint.Width) ||
                double.IsNaN(constraint.Height) || constraint.Height <= 0 || double.IsPositiveInfinity(constraint.Height)) && this.Name == "SyncChartArea_dock")
            {
                Grid parentgrid = GetGrid();
                parentgrid.SizeChanged += new SizeChangedEventHandler(parentgrid_SizeChanged);
                if (parentgrid.ActualWidth == 0 && parentgrid.ActualHeight == 0)
                {
                    constraint = new Size(100, 100);
                }
                else if (parentgrid.ActualWidth == 0 || parentgrid.ActualHeight == 0)
                {
                    constraint = parentgrid.ActualWidth == 0 ? new Size(100, parentgrid.ActualHeight) : new Size(parentgrid.ActualWidth, 100);
                }
                else
                {
                    constraint = new Size(parentgrid.ActualWidth, parentgrid.ActualHeight);
                }
            }
            // Measure each of the Children
            foreach (UIElement element in Children)
            {
                // Get the child's desired size
                Size remainingSize= new Size ();
                //This condition is included to avoid exception when using stackpanel for AreasPanel without mentioning height for chart area
                if (double.IsInfinity(constraint.Width) || double.IsInfinity(constraint.Height))
                {
                    if (double.IsInfinity(constraint.Width))
                    {
                        remainingSize = new Size(
                           Math.Max(0.0, 0.0 - usedWidth),
                           Math.Max(0.0, constraint.Height - usedHeight));
                    }

                    if (double.IsInfinity(constraint.Height))
                    {
                        remainingSize = new Size(
                           Math.Max(0.0, constraint.Width - usedWidth),
                           Math.Max(0.0, 0.0 - usedHeight));
                    }
                
                }
                else
                {
                    remainingSize = new Size(
                        Math.Max(0.0, constraint.Width - usedWidth),
                        Math.Max(0.0, constraint.Height - usedHeight));
                }               
                   
                    element.Measure(remainingSize);
               
                    
                    Size desiredSize = element.DesiredSize;
                    // Decrease the remaining space for the rest of the children
                    switch (GetDock(element))
                    {
                        case ChartDock.Left:
                        case ChartDock.Right:
                            maximumHeight = Math.Max(maximumHeight, usedHeight + desiredSize.Height);
                            usedWidth += desiredSize.Width;
                            break;
                        case ChartDock.Top:
                        case ChartDock.Bottom:
                            maximumWidth = Math.Max(maximumWidth, usedWidth + desiredSize.Width);
                            usedHeight += desiredSize.Height;
                            break;
                    }
                }
                
           
            maximumWidth = Math.Max(maximumWidth, usedWidth);
            maximumHeight = Math.Max(maximumHeight, usedHeight);
            return new Size(maximumWidth, maximumHeight);
        }

        void parentgrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateMeasure();
            this.InvalidateArrange();
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="arrangeSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double left = 0.0;
            double top = 0.0;
            double right = 0.0;
            double bottom = 0.0;

            // Arrange each of the Children
            UIElementCollection children = Children;
            int dockedCount = children.Count - (LastChildFill ? 1 : 0);
            int index = 0;
            foreach (UIElement element in children)
            {
                // Determine the remaining space left to arrange the element
                Rect remainingRect = new Rect(
                    left,
                    top,
                    Math.Max(0.0, arrangeSize.Width - left - right),
                    Math.Max(0.0, arrangeSize.Height - top - bottom));

                // Trim the remaining Rect to the docked size of the element
                // (unless the element should fill the remaining space because
                // of LastChildFill)
                if (index < dockedCount)
                {
                    Size desiredSize = element.DesiredSize;
                    switch (GetDock(element))
                    {
                        case ChartDock.Left:
                            left += desiredSize.Width;
                            remainingRect.Width = desiredSize.Width;
                            break;
                        case ChartDock.Top:
                            top += desiredSize.Height;
                            remainingRect.Height = desiredSize.Height;
                            break;
                        case ChartDock.Right:
                            right += desiredSize.Width;
                            remainingRect.X = Math.Max(0.0, arrangeSize.Width - right);
                            remainingRect.Width = desiredSize.Width;
                            break;
                        case ChartDock.Bottom:
                            bottom += desiredSize.Height;
                            remainingRect.Y = Math.Max(0.0, arrangeSize.Height - bottom);
                            remainingRect.Height = desiredSize.Height;
                            break;
                    }
                }

                element.Arrange(remainingRect);
                index++;
            }

            return arrangeSize;
        }
    }
}
