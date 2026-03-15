// <copyright file="OneNoteSplitterItemBorder.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the class for one note splitter item border for TabSplitter
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class OneNoteSplitterItemBorder : SplitterItemBorder
    {
        #region Private members
        /// <summary>
        /// Presents path
        /// </summary>
        private PathGeometry m_path;
        
        /// <summary>
        /// Presents Selected Path
        /// </summary>
        private PathGeometry m_selectPath;
        #endregion

        #region Overrides
        /// <summary>
        /// Arranges the content of a <see cref="T:System.Windows.Controls.Decorator"/> element.
        /// </summary>
        /// <param name="arrangeSize">The <see cref="T:System.Windows.Size"/> this element uses to arrange its child content.</param>
        /// <returns>
        /// The <see cref="T:System.Windows.Size"/> that represents the arranged size of this <see cref="T:System.Windows.Controls.Decorator"/> element and its child.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double awidth = arrangeSize.Width;
            double aheight = arrangeSize.Height;

            Rect arrangeRect = new Rect
            {
                Width = Math.Max(0.0, awidth - 2.0 - aheight),
                Height = Math.Max(0.0, aheight),
                X = Math.Min(aheight, awidth),
                Y = Math.Min(2.0, aheight)
            };

            if (Child != null)
            {
                Child.Arrange(arrangeRect);
            }

            m_path = new PathGeometry();
            m_selectPath = new PathGeometry();

            PathFigure figure = new PathFigure(
                new Point(1.0, aheight),
                new PathSegment[]
                {
                    new LineSegment(new Point(aheight - 3.0, 4.0), true), 
                    new LineSegment(new Point(aheight, 2.5), true), 
                    new LineSegment(new Point(aheight + 3.0, 1.5), true), 
                    new LineSegment(new Point(awidth - 5.0, 1.5), true), 
                    new LineSegment(new Point(awidth - 1.5, aheight), true)
                }, 
                false);

            PathFigure selectFigure = new PathFigure(
                new Point(0.0, aheight), 
                new PathSegment[] 
                {
                    new LineSegment(new Point(aheight - 3.0, 3.0), true),
                    new LineSegment(new Point(aheight, 1.5), true),
                    new LineSegment(new Point(aheight + 3.0, 0.5), true),
                    new LineSegment(new Point(awidth - 4.0, 0.5), true),
                    new LineSegment(new Point(awidth - 0.5, aheight), true) 
                }, 
                false);

            m_path.Figures.Add(figure);
            m_selectPath.Figures.Add(selectFigure);
            return arrangeSize;
        }

        /// <summary>
        /// Measures the child element of a <see cref="T:System.Windows.Controls.Decorator"/> to prepare for arranging it during the <see cref="M:System.Windows.Controls.Decorator.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <param name="constraint">An upper limit <see cref="T:System.Windows.Size"/> that should not be exceeded.</param>
        /// <returns>
        /// The target <see cref="T:System.Windows.Size"/> of the element.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (Child != null)
            {
                Size size = constraint;
                size.Width = Math.Max(0.0, size.Width - 8);

                Child.Measure(size);
                Size desiredSize = Child.DesiredSize;

                desiredSize.Width += !double.IsInfinity(constraint.Height)
                    ? constraint.Height : desiredSize.Height;

                return desiredSize;
            }

            return base.MeasureOverride(constraint);
        }
        
        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if ((m_selectPath != null) && ((Background != null) || (BorderBrush != null)))
            {
                DrawGeometry(drawingContext, BorderBrush, Background, m_selectPath);
                DrawGeometry(drawingContext, BorderInnerBrush, null, m_path);

                if (BottomBorderBrush != null && TemplatedParent is TabSplitterItem)
                {
                    drawingContext.DrawLine(new Pen(BottomBorderBrush, 1.0), new Point(1.8, ActualHeight), new Point(ActualWidth - 2.0, ActualHeight));
                    drawingContext.DrawLine(new Pen(BottomBorderBrush, 1.5), new Point(1.2, ActualHeight + 1), new Point(ActualWidth - 2.0, ActualHeight + 1));
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws the geometry.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="brush">The brush value.</param>
        /// <param name="background">The background value.</param>
        /// <param name="path">The path value.</param>
        private static void DrawGeometry(DrawingContext context, Brush brush, Brush background, PathGeometry path)
        {
            if (null != brush)
            {
                Pen pen = new Pen(brush, 1.0);
                context.DrawGeometry(background, pen, path);
            }
        }
        #endregion
    }
}
