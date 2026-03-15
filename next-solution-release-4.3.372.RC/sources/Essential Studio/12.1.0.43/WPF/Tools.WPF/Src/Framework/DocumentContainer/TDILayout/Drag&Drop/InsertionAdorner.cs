// <copyright file="InsertionAdorner.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class Represents the Insertion Adorner
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class InsertionAdorner : Adorner
    {
        #region Private members
        /// <summary>
        /// Represents IsSeparatorHorizontal
        /// </summary>
        private bool m_isSeparatorHorizontal;
        
        /// <summary>
        /// Represents AdornerLayer
        /// </summary>
        private AdornerLayer m_adornerLayer;
        
        /// <summary>
        /// Represents Pen
        /// </summary>
        private static Pen m_pen;
        
        /// <summary>
        /// Represents Triangle
        /// </summary>
        private static PathGeometry m_triangle;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="InsertionAdorner"/> class.
        /// </summary>
        static InsertionAdorner()
        {
            m_pen = new Pen
            {
                Brush = Brushes.Gray,
                Thickness = 2
            };
            m_pen.Freeze();

            LineSegment firstLine = new LineSegment(new Point(0, -5), false);
            firstLine.Freeze();
            LineSegment secondLine = new LineSegment(new Point(0, 5), false);
            secondLine.Freeze();

            PathFigure figure = new PathFigure
            {
                StartPoint = new Point(5, 0)
            };
            figure.Segments.Add(firstLine);
            figure.Segments.Add(secondLine);
            figure.Freeze();

            m_triangle = new PathGeometry();
            m_triangle.Figures.Add(figure);
            m_triangle.Freeze();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertionAdorner"/> class.
        /// </summary>
        /// <param name="isSeparatorHorizontal">if set to <c>true</c> [is separator horizontal].</param>
        /// <param name="isInFirstHalf">if set to <c>true</c> [is in first half].</param>
        /// <param name="adornedElement">The adorned element.</param>
        /// <param name="adornerLayer">The adorner layer.</param>
        public InsertionAdorner(bool isSeparatorHorizontal, bool isInFirstHalf, Dock tabStripPlacement, UIElement adornedElement, AdornerLayer adornerLayer)
            : base(adornedElement)
        {
            m_isSeparatorHorizontal = isSeparatorHorizontal;
            IsInFirstHalf = isInFirstHalf;
            TabStripPlacement = tabStripPlacement;
            m_adornerLayer = adornerLayer;
            IsHitTestVisible = false;
            m_adornerLayer.Add(this);
        }
        #endregion

        #region Properties

        public Dock TabStripPlacement
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is in first half.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in first half; otherwise, <c>false</c>.
        /// </value>
        public bool IsInFirstHalf
        {
            get;
            set;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public void Detach()
        {
            m_adornerLayer.Remove(this);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            Point startPoint;
            Point endPoint;

            CalculateStartAndEndPoint(out startPoint, out endPoint);
            drawingContext.DrawLine(m_pen, startPoint, endPoint);

            if (m_isSeparatorHorizontal)
            {
                DrawTriangle(drawingContext, startPoint, 0);
                DrawTriangle(drawingContext, endPoint, 180);
            }
            else
            {
                DrawTriangle(drawingContext, startPoint, 90);
                DrawTriangle(drawingContext, endPoint, -90);
            }
        }

        /// <summary>
        /// Draws the triangle.
        /// </summary>
        /// <param name="drawingContext">The drawing context.</param>
        /// <param name="origin">The origin.</param>
        /// <param name="angle">The angle.</param>
        private void DrawTriangle(DrawingContext drawingContext, Point origin, double angle)
        {
            drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
            drawingContext.PushTransform(new RotateTransform(angle));

            drawingContext.DrawGeometry(m_pen.Brush, null, m_triangle);

            drawingContext.Pop();
            drawingContext.Pop();
        }
        
        /// <summary>
        /// Calculates the start and end point.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point.</param>
        private void CalculateStartAndEndPoint(out Point startPoint, out Point endPoint)
        {
            startPoint = new Point();
            endPoint = new Point();

            double width = AdornedElement.RenderSize.Width;
            double height = AdornedElement.RenderSize.Height;

            if (m_isSeparatorHorizontal)
            {
                endPoint.X = width;
                if (!IsInFirstHalf)
                {
                    startPoint.Y = height;
                    endPoint.Y = height;
                }
            }
            else
            {
                endPoint.Y = height;
                if (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Right)
                {
                    if (!IsInFirstHalf)
                    {
                        startPoint.X = width;
                        endPoint.X = width;
                    }
                }
                if (TabStripPlacement == Dock.Bottom || TabStripPlacement == Dock.Left)
                {
                    if (IsInFirstHalf)
                    {
                        startPoint.X = width;
                        endPoint.X = width;
                    }
                }
            }
        }
        #endregion
    }
}