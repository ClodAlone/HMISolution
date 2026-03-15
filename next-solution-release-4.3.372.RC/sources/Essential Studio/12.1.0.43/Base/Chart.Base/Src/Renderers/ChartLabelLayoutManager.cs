#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Represents the layout information of point label. 
    /// </summary>
    internal sealed class ChartLabel
    {
        #region Members
        private SizeF m_size;
        private SizeF m_offset;
        private PointF m_connectPoint;
        private PointF m_symbolPoint;
        private RectangleF m_rect;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public SizeF Size
        {
            get
            {
                return m_size;
            }
        }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public SizeF Offset
        {
            get
            {
                return m_offset;
            }
        }

        /// <summary>
        /// Gets the connect point.
        /// </summary>
        /// <value>The connect point.</value>
        public PointF ConnectPoint
        {
            get
            {
                return m_connectPoint;
            }
        }

        /// <summary>
        /// Gets the symbol point.
        /// </summary>
        /// <value>The symbol point.</value>
        public PointF SymbolPoint
        {
            get
            {
                return m_symbolPoint;
            }
        }

        /// <summary>
        /// Gets or sets the rect.
        /// </summary>
        /// <value>The rect.</value>
        public RectangleF Rect
        {
            get
            {
                return m_rect;
            }

            set
            {
                m_rect = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates instance of the ChartLabel.
        /// </summary>
        /// <param name="connectPoint">The connection point.</param>
        /// <param name="symbolPoint">The symbol point.</param>
        /// <param name="size">The size of the label.</param>
        /// <param name="offset">The label offset.</param>
        public ChartLabel(PointF connectPoint, PointF symbolPoint, SizeF size, SizeF offset)
        {
            m_symbolPoint = symbolPoint;
            m_connectPoint = connectPoint;
            m_size = size;
            m_offset = offset;
            m_rect = RectangleF.Empty;
        }

        /// <summary>
        /// Draw pointing line.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> to render line.</param>
        /// <param name="style">Line style.</param>
        /// <param name="mm_series">The Chart Series</param>
        public void DrawPointingLine(Graphics g, ChartStyleInfo style, ChartSeries mm_series)
        {
            if (m_rect.IsEmpty) return;
            PointF[] pnts = new PointF[]{ m_rect.Location, new PointF( (m_rect.Right + m_rect.Left)/2, m_rect.Top ), new PointF( m_rect.Right, m_rect.Top ),
                                    new PointF( m_rect.Right,(m_rect.Top + m_rect.Bottom)/2), new PointF( m_rect.Right, m_rect.Bottom ),
                                    new PointF((m_rect.Right + m_rect.Left)/2, m_rect.Bottom ), new PointF( m_rect.Left, m_rect.Bottom ), 
                                    new PointF( m_rect.Left, (m_rect.Top + m_rect.Bottom)/2 ) };
            int mindistind = 0;
            float mindist = float.MaxValue;
            for (int i = 0; i < pnts.Length; i++)
            {
                float d = ChartMath.DistanceBetweenPoints(m_symbolPoint, pnts[i]);
                if (mindist > d)
                {
                    mindist = d;
                    mindistind = i;
                }
            }

            PointF p = pnts[mindistind], neigh1p, neigh2p;
            if (mindistind == pnts.Length - 1)
                neigh1p = pnts[0];
            else neigh1p = pnts[mindistind + 1];
            if (mindistind == 0)
                neigh2p = pnts[pnts.Length - 1];
            else neigh2p = pnts[mindistind - 1];

            if ((!m_rect.Contains(m_symbolPoint)) && (mindist > 10))
            {
                // Pen pen = Pens.Red;//RenderingHelper.GetOuterBorderPen(style);

                //g.DrawLine(pen, neigh1p, p);
                //g.DrawLine(pen, neigh2p, p);

                Pen pen = new Pen(mm_series.SmartLabelsBorderColor, mm_series.SmartLabelsBorderWidth);
                g.DrawLine(pen, m_symbolPoint, p);
                g.DrawRectangle(pen, Rectangle.Round(m_rect));

            }
        }
        #endregion
    }

    /// <summary>
    /// Provides the 'SmartLabels' feature.
    /// </summary>
    internal sealed class ChartLabelLayoutManager : IEnumerable
    {
        #region Members
        private RectangleF m_workArea = Rectangle.Empty;
        private ArrayList m_labels = new ArrayList();

        private ArrayList m_currectArea = new ArrayList();
        private SizeF m_minimalLabelSize = SizeF.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the size of the minimal.
        /// </summary>
        /// <value>The size of the minimal.</value>
        public SizeF MinimalSize
        {
            get
            {
                return m_minimalLabelSize;
            }

            set
            {
                m_minimalLabelSize = value;
            }
        }

        /// <summary>
        /// Gets the count of labels.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return m_labels.Count;
            }
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Forms.Chart.ChartLabel"/> at the specified index.
        /// </summary>
        /// <value></value>
        public ChartLabel this[int index]
        {
            get
            {
                return (ChartLabel)m_labels[index];
            }
        }
        #endregion

        #region Consrtuctor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLabelLayoutManager"/> class.
        /// </summary>
        /// <param name="workArea">The work area.</param>
        public ChartLabelLayoutManager(RectangleF workArea)
        {
            m_workArea = workArea;
            m_currectArea.Add(m_workArea);
        }
        #endregion

        #region Public methos
        /// <summary>
        /// Add label to collection.
        /// </summary>
        /// <param name="label">The label to add.</param>
        public RectangleF AddLabel(ChartLabel label)
        {
            label.Rect = FindFreeSpace(label);
            m_labels.Add(label);
            Exclude(label.Rect);

            return label.Rect;
        }

        /// <summary>
        /// Add point to collection.
        /// </summary>
        /// <param name="p">Point to add.</param>
        public void AddPoint(PointF p)
        {
            Exclude(p);
        }

        /// <summary>
        /// Clears the labels.
        /// </summary>
        public void Clear()
        {
            m_labels.Clear();
            m_currectArea.Clear();
            m_currectArea.Add(m_workArea);
        }

        /// <summary>
        /// Draws the labels to the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        public void Draw(Graphics g)
        {
            foreach (RectangleF rc in m_currectArea)
            {
                g.DrawRectangle(Pens.Red, rc.X, rc.Y, rc.Width, rc.Height);
                g.DrawLine(Pens.Red, rc.Left, rc.Top, rc.Right, rc.Bottom);
                g.DrawLine(Pens.Red, rc.Right, rc.Top, rc.Left, rc.Bottom);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_labels.GetEnumerator();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Exclude2s the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        private void Exclude2(RectangleF rect)
        {
            ArrayList result = new ArrayList();

            foreach (RectangleF source in m_currectArea)
            {
                if (source.IntersectsWith(rect))
                {
                    if (source.Left < rect.Left)
                    {
                        result.Add(new RectangleF(source.Left, source.Top, rect.Left - source.Left, source.Height));
                    }

                    if (source.Top < rect.Top)
                    {
                        result.Add(new RectangleF(source.Left, source.Top, source.Width, rect.Top - source.Top));
                    }

                    if (source.Right > rect.Right)
                    {
                        result.Add(new RectangleF(rect.Right, source.Top, source.Right - rect.Right, source.Height));
                    }

                    if (source.Bottom > rect.Bottom)
                    {
                        result.Add(new RectangleF(source.Left, rect.Bottom, source.Width, source.Bottom - rect.Bottom));
                    }
                }
                else
                {
                    result.Add(source);
                }
            }

            m_currectArea = result;
        }

        /// <summary>
        /// Excludes the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        private void Exclude(RectangleF rect)
        {
            ArrayList result = new ArrayList();

            foreach (RectangleF source in m_currectArea)
            {
                if (source.IntersectsWith(rect))
                {
                    if (source.Left < rect.Left)
                    {
                        RectangleF tr = new RectangleF(source.Left, source.Top, rect.Left - source.Left, source.Height);
                        if (CheckWithMinSize(tr, result)) result.Add(tr);
                    }

                    if (source.Top < rect.Top)
                    {
                        RectangleF tr = new RectangleF(source.Left, source.Top, source.Width, rect.Top - source.Top);
                        if (CheckWithMinSize(tr, result)) result.Add(tr);
                    }

                    if (source.Right > rect.Right)
                    {
                        RectangleF tr = new RectangleF(rect.Right, source.Top, source.Right - rect.Right, source.Height);
                        if (CheckWithMinSize(tr, result)) result.Add(tr);
                    }

                    if (source.Bottom > rect.Bottom)
                    {
                        RectangleF tr = new RectangleF(source.Left, rect.Bottom, source.Width, source.Bottom - rect.Bottom);
                        if (CheckWithMinSize(tr, result)) result.Add(tr);
                    }
                }
                else
                {
                    if (CheckWithMinSize(source, result)) result.Add(source);
                }
                result.Sort(new RectangleAreaComparer());
            }

            m_currectArea = result;
        }

        /// <summary>
        /// Excludes the specified p.
        /// </summary>
        /// <param name="p">The p.</param>
        private void Exclude(PointF p)
        {
            RectangleF r = new RectangleF(p, Size.Empty);
            this.Exclude(r);
        }

        /// <summary>
        /// Finds the free space.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <returns></returns>
        private RectangleF FindFreeSpace(ChartLabel label)
        {
            RectangleF resultRect = RectangleF.Empty;
            double currRadius = double.PositiveInfinity;
            RectangleF wantedRect = new RectangleF(label.ConnectPoint.X + label.Offset.Width,
              label.ConnectPoint.Y + label.Offset.Height, label.Size.Width, label.Size.Height);

            if (m_workArea.IntersectsWith(wantedRect))
            {
                foreach (RectangleF rc in m_currectArea)
                {
                    if (rc.Contains(wantedRect))
                    {
                        resultRect = wantedRect;
                        break;
                    }

                    if ((rc.Width > label.Size.Width) && (rc.Height > label.Size.Height) /*&& wantedRect.IntersectsWith(rc)*/)
                    {
                        RectangleF res = CalcBestPlace(label, rc);

                        if (currRadius > CalcRadius(label.ConnectPoint, res.Location))
                        {
                            currRadius = CalcRadius(label.ConnectPoint, res.Location);
                            resultRect = res;
                        }
                    }
                }
            }

            return resultRect;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Calculates the best place.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        private RectangleF CalcBestPlace(ChartLabel label, RectangleF rect)
        {
            RectangleF result = RectangleF.Empty;
            RectangleF wantedRect = new RectangleF(label.ConnectPoint.X + label.Offset.Width,
              label.ConnectPoint.Y + label.Offset.Height, label.Size.Width, label.Size.Height);

            if (rect.Contains(wantedRect))
            {
                result = wantedRect;
            }
            else
            {
                result = wantedRect; //new RectangleF( label.ConnectPoint, label.Size );

                if (result.Left < rect.Left)
                {
                    result.Offset(rect.Left - result.Left, 0);
                }

                if (result.Top < rect.Top)
                {
                    result.Offset(0, rect.Top - result.Top);
                }

                if (result.Right > rect.Right)
                {
                    result.Offset(rect.Right - result.Right, 0);
                }

                if (result.Bottom > rect.Bottom)
                {
                    result.Offset(0, rect.Bottom - result.Bottom);
                }

                //float x = rect.Left > label.ConnectPoint.X ? rect.Left : rect.Right - label.Size.Width;
                //float y = rect.Top > label.ConnectPoint.Y ? rect.Top : rect.Bottom - label.Size.Height;

                //result = new RectangleF( x, y, label.Size.Width, label.Size.Height );
            }

            return result;
        }

        /// <summary>
        /// Checks the size of the with min.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private bool CheckWithMinSize(RectangleF rect, ArrayList result)
        {
            if (rect.Width < this.MinimalSize.Width) return false;
            if (rect.Height < this.MinimalSize.Height) return false;
            foreach (RectangleF source in result)
            {
                if (source.Contains(rect)) return false;
            }
            return true;
        }

        /// <summary>
        /// Calculates the radius.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <returns></returns>
        private double CalcRadius(PointF pt1, PointF pt2)
        {
            double rw = Math.Abs(pt1.X - pt2.X);
            double rh = Math.Abs(pt1.Y - pt2.Y);

            return Math.Sqrt(rw * rw + rh * rh);
        }

        /// <summary>
        /// Calculates the center.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        private PointF CalcCenter(RectangleF rect)
        {
            return new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        }
        #endregion

        #region Internal methods
        /// <summary>
        /// Compares <see cref="RectangleF"/> by the area value.
        /// </summary>
        class RectangleAreaComparer : IComparer
        {
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
            int System.Collections.IComparer.Compare(object x, object y)
            {
                RectangleF r1 = (RectangleF)x;
                RectangleF r2 = (RectangleF)y;

                float a1 = r1.Width * r1.Height;
                float a2 = r2.Width * r2.Height;

                if (a1 > a2)
                {
                    return -1;
                }
                else if (a1 < a2)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            #endregion
        }
        #endregion
    }
}
