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
using System.Collections;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Compares the segments by the position.
    /// </summary>
    class ChartSegmentComparer : IComparer
    {
        #region Members
        private bool m_isInvesed = false;
        #endregion

        #region Construcotr
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSegmentComparer"/> class.
        /// </summary>
        /// <param name="inversed">if set to <c>true</c> comparering inversed.</param>
        public ChartSegmentComparer(bool inversed)
        {
            m_isInvesed = inversed;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero x is less than y. Zero x equals y. Greater than zero x is greater than y.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">Neither x nor y implements the <see cref="T:System.IComparable"></see> interface.-or- x and y are of different types and neither one can handle comparisons with the other. </exception>
        public int Compare(object x, object y)
        {
            ChartSegment sx = x as ChartSegment;
            ChartSegment sy = y as ChartSegment;

            RectangleF rc1 = sx.Bounds;
            RectangleF rc2 = sy.Bounds;

            #region Equal test
            if (sx.ZOrder > sy.ZOrder)
            {
                return 1;
            }
            else if (sx.ZOrder < sy.ZOrder)
            {
                return -1;
            }
            #endregion

            if (m_isInvesed)
            {
                if (rc1.Y < rc2.Y)
                {
                    return 1;
                }
                else if (rc1.Y > rc2.Y)
                {
                    return -1;
                }

                if (rc1.X > rc2.X)
                {
                    return 1;
                }
                else if (rc1.X < rc2.X)
                {
                    return -1;
                }
            }
            else
            {
                if (rc1.X > rc2.X)
                {
                    return 1;
                }
                else if (rc1.X < rc2.X)
                {
                    return -1;
                }

                if (rc1.Y < rc2.Y)
                {
                    return 1;
                }
                else if (rc1.Y > rc2.Y)
                {
                    return -1;
                }
            }

            return 0;
        }
        #endregion
    }

    /// <summary>
    /// Provides the sorting and rendering of segments.
    /// </summary>
    class RenderViewer
    {
        #region Members
        private ArrayList m_paths = new ArrayList();
        private bool m_needRegionUpdate = false;
        private bool m_axisInverted = false;
        private IList m_regions;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether need update regions.
        /// </summary>
        /// <value><c>true</c> if need update regions; otherwise, <c>false</c>.</value>
        public bool NeedRegionUpdate
        {
            get
            {
                return m_needRegionUpdate;
            }

            set
            {
                m_needRegionUpdate = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether axis is inverted.
        /// </summary>
        /// <value><c>true</c> if axis inverted; otherwise, <c>false</c>.</value>
        public bool AxisInverted
        {
            get
            {
                return m_axisInverted;
            }

            set
            {
                m_axisInverted = value;
            }
        }

        /// <summary>
        /// Gets or sets the regions.
        /// </summary>
        /// <value>The regions.</value>
        public IList Regions
        {
            get
            {
                return m_regions;
            }

            set
            {
                m_regions = value;
            }
        }
        #endregion

        #region Pulic methods
        /// <summary>
        /// Adds the segment.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public void AddSegment(ChartSegment segment)
        {
            m_paths.Add(segment);
        }

        /// <summary>
        /// Adds segments form the specified render.
        /// </summary>
        /// <param name="render">The render.</param>
        public void Add(ChartSeriesRenderer render)
        {
            if (render.Segments != null)
            {
                m_paths.AddRange(render.Segments);
            }
        }

        /// <summary>
        /// Sorts this instance.
        /// </summary>
        /// <returns></returns>
        public IEnumerable Sort()
        {
            if (m_paths.Count > 0)
            {
                m_paths.Sort(new ChartSegmentComparer(m_axisInverted));
            }

            return m_paths;
        }

        /// <summary>
        /// Draws to the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        public void View(Graphics g)
        {
            IEnumerable sortedList = this.Sort();

            foreach (ChartSegment segment in m_paths)
            {
                segment.Draw(g);
                if(segment.GetChartRegion()!=null)
                m_regions.Add(segment.GetChartRegion());
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            m_paths.Clear();
        }
        #endregion
    }
}
