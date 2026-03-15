#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Drawing;
using System.Collections;
using Syncfusion.Documentation;
using System.Collections.Generic;


namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// ChartAreaCursorCollection is a collection for <see cref="ChartInteractiveCursor"/> objects.
    /// </summary>
    public sealed class ChartAreaCursorCollection : ChartBaseList
    {
        #region Properties
        /// <summary>
        /// Looks up the collection and returns the interactive cursor at the specified index.
        /// </summary>
        public ChartInteractiveCursor this[int index]
        {
            get
            {
                return this.List[index] as ChartInteractiveCursor;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Looks up the collection and returns the index value of the specified cursor if it is present.
        /// </summary>
        /// <param name="cursor" type="ChartInteractiveCursor">
        ///     <para>
        ///      Cursor to look for.   
        ///     </para>
        /// </param>
        /// <returns>
        ///     The index value of the cursor if the look up was successful; -1 otherwise.
        /// </returns>
        public int IndexOf(ChartInteractiveCursor cursor)
        {
            return this.List.IndexOf(cursor);
        }
        /// <summary>
        ///     Adds the specified cursor to this collection.
        /// </summary>
        /// <param name="cursor" type="ChartInteractiveCursor">
        ///     <para>
        ///     An instance of the cursor that is to be added.    
        ///     </para>
        /// </param>
        public void Add(ChartInteractiveCursor cursor)
        {
            this.List.Add(cursor);
        }

        /// <summary>
        ///     Inserts the specified cursor at the specified index.
        /// </summary>
        /// <param name="index" type="int">
        ///     <para>
        ///     Index value where the cursor is to be inserted.   
        ///     </para>
        /// </param>
        /// <param name="cursor" type="ChartInteractiveCursor">
        ///     <para>
        ///     An instance of the cursor that is to be inserted.    
        ///     </para>
        /// </param>
        public void Insert(int index, ChartInteractiveCursor cursor)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, "Value can not be less than 0.");

            this.List.Insert(index, cursor);
        }

        /// <summary>
        ///     Removes the specified cursor from this collection.
        /// </summary>
        /// <param name="cursor" type="ChartInteractiveCursor">
        ///     <para>
        ///      Cursor that is to be removed.   
        ///     </para>
        /// </param>
        public void Remove(ChartInteractiveCursor cursor)
        {
            this.List.Remove(cursor);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Performs additional custom processes when validating a value
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <returns>If is true, value is approved.</returns>
        protected override bool Validate(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("value");

            if (obj is ChartInteractiveCursor)
            {
                return !this.List.Contains(obj);
            }

            throw new ArgumentException("value should be ChartInteractiveCursor");
        }
        #endregion
    }

    /// <summary>
    /// This type implements the cursor service which lets the end user drag a set of horizontal and vertical lines and dock them
    /// to a data point to help visualize the X and Y values of these data points.
    /// </summary>
    public sealed class ChartInteractiveCursor
    {
        #region Constants
        private const int c_lineSize = 3;
        #endregion

        #region Members
        private ChartSeries m_series = null;
        internal static Color m_defaultColor = Color.Red;

        private int m_xPosition = 0;
        private int m_yPosition = 0;
        private PointF m_lineLocation;
        private bool m_lineRedraw;
        private ChartPointWithIndex[] verticalSorted;
        private ChartPointWithIndex[] horizontalSorted;

        private Hashtable vertTOHoriz;
        private Hashtable horizTOVert;


        private Color m_cursorColor = m_defaultColor;
        private Color horizontalCursorColor = m_defaultColor;
        private Color verticalCursorColor = m_defaultColor;

        private InteractiveCursorOrientation cursorOrientation = InteractiveCursorOrientation.Both;
        private bool m_chartArea =false;
        private bool m_pointSymbol = true;
        private double m_xInterval = 0;
        private double m_yInterval = 0;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when properties is changed.
        /// </summary>
        public event EventHandler Changed;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the X position.
        /// </summary>
        /// <value>The X position.</value>
        public int XPosition
        {
            get
            {
                return m_xPosition;
            }

            set
            {
                if (m_xPosition != value)
                {
                    m_xPosition = ChartMath.MinMax(value, 0, horizontalSorted.Length - 1);
                    m_yPosition = (int)horizTOVert[m_xPosition];
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get and Set the Y position of the Interactive Cursor.
        /// </summary>
        /// <value>The Y position.</value>
        public int YPosition
        {
            get
            {
                return m_yPosition;
            }

            set
            {
                if (m_yPosition != value)
                {
                    m_yPosition = ChartMath.MinMax(value, 0, verticalSorted.Length - 1);
                    m_xPosition = (int)vertTOHoriz[m_yPosition];
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get and Set the Cursor Orientation of the Interactive Cursor.
        /// </summary>
        /// <value>Cursor Orientation</value>
        public InteractiveCursorOrientation CursorOrientation
        {
            get { return cursorOrientation; }
            set { cursorOrientation = value; }
        }

        /// <summary>
        /// Get or Set the cursor moving to chart area.
        /// </summary>
        /// <value>The move to chart area </value>
        public bool MoveToChartArea
        {
            get { return m_chartArea; }
            set { m_chartArea = value; }
        }

        /// <summary>
        /// Get or Set whether the cursor line need to LineRedraw or not.
        /// </summary>
        /// <value>The LineRedraw </value>
        public bool LineRedraw
        {
            get
            {
                return m_lineRedraw;
            }
            set
            {
                m_lineRedraw = value;
            }
        }

        /// <summary>
        /// Get and Set the symbol for series points.
        /// </summary>
        /// <value>ShowPointSymbol</value>
        public bool ShowPointSymbol
        {
            get
            {
                return m_pointSymbol;
            }
            set
            {
                m_pointSymbol = value;
            }
        }

        /// <summary>
        /// Get and Set the X inetrval.
        /// </summary>
        /// <value>Interval  for X Axis</value>
        public double XInterval
        {
            get 
            { 
                return m_xInterval; 
            }
            set 
            {
                m_xInterval = value; 
            }
        }
        /// <summary>
        /// Get and Set the Y inetrval.
        /// </summary>
        /// <value>Interval  for Y Axis</value>
        public double YInterval
        {
            get 
            { return m_yInterval; 
            }
            set
            {
                m_yInterval=value;
            }
        }

        /// <summary>
        /// Get and Set the Horizontal cursor color of the Interactive Cursor.
        /// </summary>
        /// <value>Horizontal cursor color</value>

        public Color HorizontalCursorColor
        {
            get { return horizontalCursorColor; }
            set
            {
                if (horizontalCursorColor != value)
                {
                    horizontalCursorColor = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get and Set the Vertical cursor color of the Interactive Cursor.
        /// </summary>
        /// <value>Vertical cursor color</value>
        public Color VerticalCursorColor
        {
            get { return verticalCursorColor; }
            set
            {
                if (verticalCursorColor != value)
                {
                    verticalCursorColor = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }

        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public PointF Location
        {
            get
            {
                return m_series.Renderer.GetPointFromIndex(horizontalSorted[m_xPosition].Index);
            }
        }

        /// <summary>
        /// Get or Set the CursorLineLocation.
        /// </summary>
        /// <value>The CursorLine location.</value>
        public PointF LineLocation
        {
            get
            {
                return m_lineLocation;
            }
                     
            set
            {
                m_lineLocation = value;
                this.RaiseChanged(this, EventArgs.Empty);
            }
        }

       

        /// <summary>
        /// Gets the point.
        /// </summary>
        /// <value>The point.</value>
        public ChartPoint Point
        {
            get
            {
                return horizontalSorted[m_xPosition].Point;
            }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public Color Color
        {
            get
            {
                return m_cursorColor;
            }

            set
            {
                if (m_cursorColor != value)
                {
                    m_cursorColor = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }       


        /// <summary>
        /// Gets the series.
        /// </summary>
        /// <value>The series.</value>
        public ChartSeries Series
        {
            get
            {
                return m_series;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartInteractiveCursor"/> class.
        /// </summary>
        /// <param name="chartSeries">Specifies the chart series data points that the cursor should track.</param>
        public ChartInteractiveCursor(ChartSeries chartSeries)
        {
            m_series = chartSeries;
            ResortArrays();
            m_yPosition = (int)horizTOVert[m_xPosition];
            m_series.SeriesChanged += new EventHandler(OnSeriesChanged);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartInteractiveCursor"/> class.
        /// </summary>
        /// <param name="chartSeries">Specifies the chart series data points that the cursor should track.</param>
        /// <param name="color">The color of the cursors.</param>
        public ChartInteractiveCursor(ChartSeries chartSeries, Color color)
        {
            m_series = chartSeries;
            ResortArrays();
            m_yPosition = (int)horizTOVert[m_xPosition];
            m_series.SeriesChanged += new EventHandler(OnSeriesChanged);
            m_cursorColor = color;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Call this method to move the horizontal cursor up or down, to the next nearest data point in Y axis.
        /// </summary>
        /// <param name="moveUp">True to move up, false to move down.</param>
        public void VerticalMove(bool moveUp)
        {
            if (moveUp)
            {
                this.YPosition++;
            }
            else
            {
                this.YPosition--;
            }
        }

        /// <summary>
        /// Call this method to move the vertical cursor left or right, to the next nearest data point in X axis.
        /// </summary>
        /// <param name="moveRight">True to move right, false to move left.</param>
        public void HorizontalMove(bool moveRight)
        {
            if (moveRight)
            {
                this.XPosition++;
            }
            else
            {
                this.XPosition--;
            }
        }

        /// <summary>
        /// Returns the series index value of the closest point to the specified x value and moves the cursor to that point.
        /// If closest point could not be found, -1 is returned.
        /// </summary>
        /// <param name="x">Specifies the X value in the chart near which the data point is to be found.</param>
        /// <returns>The series index of the data point or -1 if nothing is nearby.</returns>
        public int HorizontalMove(double x)
        {
            int xpos = this.GetClosestXPoint(x);

            if (xpos == -1)
                return -1;

            this.XPosition = xpos;

            return horizontalSorted[m_xPosition].Index;
        }

        /// <summary>
        /// Returns the series index value of the closest point to the specified y value.
        /// If closest point could not be found, -1 is returned.
        /// </summary>
        /// <param name="y">Specifies the Y value in the chart near which the data point is to be found.</param>
        /// <returns>The series index of the data point or -1 if nothing is nearby.</returns>
        public int VerticalMove(double y)
        {
            int ypos = GetClosestYPoint(y);

            if (ypos == -1)
                return -1;

            this.YPosition = ypos;

            return verticalSorted[m_yPosition].Index;
        }

        /// <summary>
        /// Returns the closest  X point for the given Y value.
        /// </summary>
        /// <param name="x">Specifies the X value in the chart near which the data point is to be found.</param>
        /// <returns>The series index of the data point or -1 if nothing is nearby.</returns>   
        public int GetClosestXPoint(double x)
        {
            int length = horizontalSorted.Length;
            int hiBound = length - 1;
            int lowBound = 0;
            int pos = m_xPosition;
            while (true)
            {
                if (length == 1)
                    return 0;
                if (length == 2)
                {
                    if (x < 0)
                        return 0;
                    else
                        return 1;
                }
                if (pos <= 0)
                    pos = 1;
                if (pos >= length - 1)
                    pos = length - 2;


                double dx = x - horizontalSorted[pos].Point.X;
                double dx1 = horizontalSorted[pos - 1].Point.X - horizontalSorted[pos].Point.X;
                double dx2 = horizontalSorted[pos + 1].Point.X - horizontalSorted[pos].Point.X;

                if (dx < 0)
                {
                    if (dx < dx1)
                    {
                        hiBound = pos;
                    }
                    else
                    {
                        if (dx < dx1 / 2.0)
                        {
                            return pos - 1;
                        }
                        else
                        {
                            return pos;
                        }
                    }
                }
                else
                {
                    if (dx > dx2)
                    {
                        lowBound = pos;
                    }
                    else
                    {
                        if (dx > dx2 / 2.0)
                        {
                            return pos + 1;
                        }
                        else
                        {
                            return pos;
                        }
                    }
                }
                if ((hiBound - lowBound) == 1)
                {
                    if (x > horizontalSorted[length - 1].Point.X)
                        return length - 1;
                    if (x < horizontalSorted[0].Point.X)
                        return 0;

                    return -1;
                }

                pos = (hiBound + lowBound) / 2;
            }
        }

        /// <summary>
        /// Returns the closest Y point for the given Y value.
        /// </summary>
        /// <param name="y">Specifies the Y value in the chart near which the data point is to be found.</param>
        /// <returns>The series index of the data point or -1 if nothing is nearby.</returns>
        public int GetClosestYPoint(double y)
        {
            int length = verticalSorted.Length;
            int hiBound = length - 1;
            int lowBound = 0;
            int pos = m_yPosition;
            while (true)
            {
                if (length == 1)
                    return 0;
                if (length == 2)
                {
                    if (y > verticalSorted[0].Point.YValues[0])
                        return 1;
                    else
                        return 0;
                }

                if (pos <= 0)
                    pos = 1;
                if (pos >= length - 1)
                    pos = length - 2;


                double dy = y - verticalSorted[pos].Point.YValues[0];
                double dy1 = verticalSorted[pos - 1].Point.YValues[0] - verticalSorted[pos].Point.YValues[0];
                double dy2 = verticalSorted[pos + 1].Point.YValues[0] - verticalSorted[pos].Point.YValues[0];

                if (dy < 0)
                {
                    if (dy < dy1)
                    {
                        hiBound = pos;
                    }
                    else
                    {
                        if (dy < dy1 / 2.0)
                        {
                            return pos - 1;
                        }
                        else
                        {
                            return pos;
                        }
                    }
                }
                else
                {
                    if (dy > dy2)
                    {
                        lowBound = pos;
                    }
                    else
                    {
                        if (dy > dy2 / 2.0)
                        {
                            return pos + 1;
                        }
                        else
                        {
                            return pos;
                        }
                    }
                }
                if ((hiBound - lowBound) == 1)
                {
                    if (y > verticalSorted[length - 1].Point.YValues[0])
                        return length - 1;
                    if (y < verticalSorted[0].Point.YValues[0])
                        return 0;

                    return -1;
                }

                pos = (hiBound + lowBound) / 2;
            }
        }

        /// <summary>
        /// Specifies if the absolute difference in X position of the specified point and the current location of the cursor is less than 3.
        /// </summary>
        /// <param name="pt">The point in question.</param>
        /// <returns>true if the specified point is within a range of 3.</returns>
        public bool IsXLocation(Point pt)
        {

            if (this.MoveToChartArea == true && this.LineRedraw==true)
            {
                return Math.Abs(pt.X - LineLocation.X) < c_lineSize;
            }
            else
            {
                return Math.Abs(pt.X - Location.X) < c_lineSize;
            }
            
        }

        /// <summary>
        /// Specifies if the absolute difference in Y position of the specified point and the current location of the cursor is less than 3.
        /// </summary>
        /// <param name="pt">The point in question.</param>
        /// <returns>true if the specified point is within a range of 3.</returns>
        public bool IsYLocation(Point pt)
        {
            if (this.MoveToChartArea == true && this.LineRedraw == true)
            {
                return Math.Abs(pt.Y - LineLocation.Y) < c_lineSize;
            }
            else
            {
                return Math.Abs(pt.Y - Location.Y) < c_lineSize;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Resorts the points arrays.
        /// </summary>
        private void ResortArrays()
        {
            List<int> m_tempPointIndex = null;

            int count;

            m_tempPointIndex = m_series.Renderer.CalCachePoints();

            count = m_tempPointIndex.Count > 0 ? m_tempPointIndex.Count : m_series.Points.Count;
           
            verticalSorted = new ChartPointWithIndex[count];
            horizontalSorted = new ChartPointWithIndex[count];

            for (int i = 0; i < count; i++)
            {
                verticalSorted[i] = m_tempPointIndex.Count > 0 ? new ChartPointWithIndex(m_series.Points[m_tempPointIndex[i]], m_tempPointIndex[i]) : new ChartPointWithIndex(m_series.Points[i], i);
                horizontalSorted[i] = m_tempPointIndex.Count > 0 ? new ChartPointWithIndex(m_series.Points[m_tempPointIndex[i]], m_tempPointIndex[i]) : new ChartPointWithIndex(m_series.Points[i], i);
            }

            Array.Sort(verticalSorted, new ComparerPointWithIndexByY());
            Array.Sort(horizontalSorted, new ComparerPointWithIndexByX());

            vertTOHoriz = new Hashtable(100);
            horizTOVert = new Hashtable(100);

            for (int i = 0; i < count; i++)
            {
                int ind = horizontalSorted[i].Index;
                for (int j = 0; j < count; j++)
                {
                    if (verticalSorted[j].Index == ind)
                    {
                        vertTOHoriz.Add(j, i);
                        horizTOVert.Add(i, j);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Called when series is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnSeriesChanged(object sender, EventArgs e)
        {
            this.ResortArrays();
        }

        /// <summary>
        /// Raises the Changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RaiseChanged(object sender, EventArgs args)
        {
            if (Changed != null)
            {
                Changed(sender, args);
            }
        }
        #endregion
    }
}
