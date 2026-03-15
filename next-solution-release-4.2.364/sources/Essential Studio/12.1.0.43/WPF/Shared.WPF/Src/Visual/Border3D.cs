// <copyright file="Border3D.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Border that renders old-style 3D border when no border brush is specified.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class Border3D : Border
    {
        #region Constants

        /// <summary>
        /// White color.
        /// </summary>
        private static readonly Color ColorBightest = Colors.White;

        /// <summary>
        /// LightGray color.
        /// </summary>
        private static readonly Color ColorBright = SystemColors.ControlColor;

        /// <summary>
        /// Gray color.
        /// </summary>
        private static readonly Color ColorDark = Color.FromRgb(128, 128, 128);

        /// <summary>
        /// Black color.
        /// </summary>
        private static readonly Color ColorDarkest = Color.FromRgb(64, 64, 64);

        /// <summary>
        /// Defines line thickness.
        /// </summary>
        private const double DEF_LINE_THICKNESS = 1d;

        #endregion

        #region Private members

        /// <summary>
        /// Pen using to draw brightest part of border.
        /// </summary>
        private Pen m_penBrightest;

        /// <summary>
        /// Pen using to draw bright part of border.
        /// </summary>
        private Pen m_penBright;

        /// <summary>
        /// Pen using to draw dark part of border.
        /// </summary>
        private Pen m_penDark;

        /// <summary>
        /// Pen using to draw darkest part of border.
        /// </summary>
        private Pen m_penDarkest;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the border drawing is "inverted". This is a dependency property.
        /// </summary>
        public bool IsInverted
        {
            get
            {
                return (bool)GetValue(IsInvertedProperty);
            }

            set
            {
                SetValue(IsInvertedProperty, value);
            }
        }
        #endregion

        #region Dependency properties

        /// <summary>
        /// Identifies <see cref="IsInverted"/> dependency property.         
        /// </summary>
        public static readonly DependencyProperty IsInvertedProperty =
            DependencyProperty.Register("IsInverted", typeof(bool), typeof(Border3D), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="Border3D"/> class.
        /// </summary>
        public Border3D()
        {
            PreparePens();
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Creates freeze <see cref="Pen"/> object based on a specified color.
        /// </summary>
        /// <param name="color">Specified color.</param>
        /// <returns><see cref="Pen"/> object based on a specified color.</returns>
        private Pen CreatePen(Color color)
        {
            Brush brush = new SolidColorBrush(color);
            brush.Freeze();
            Pen pen = new Pen(brush, 1);
            pen.StartLineCap = PenLineCap.Square;
            pen.EndLineCap = PenLineCap.Square;
            pen.Freeze();
            return pen;
        }

        /// <summary>
        /// Creates all needed pen objects.
        /// </summary>
        private void PreparePens()
        {
            m_penBrightest = CreatePen(ColorBightest);
            m_penBright = CreatePen(ColorBright);
            m_penDark = CreatePen(ColorDark);
            m_penDarkest = CreatePen(ColorDarkest);
        }

        /// <summary>
        /// Renders borders.
        /// </summary>
        /// <param name="dc">Context for drawing.</param>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            bool hasBorders = BorderThickness.Left > 0 || BorderThickness.Right > 0 || BorderThickness.Top > 0 || BorderThickness.Bottom > 0;

            if (BorderBrush == null && hasBorders)
            {
                double halfThickness = DEF_LINE_THICKNESS / 2d;

                double left1 = halfThickness;
                double left2 = left1 + DEF_LINE_THICKNESS;
                double top1 = halfThickness;
                double top2 = top1 + DEF_LINE_THICKNESS;
                double right1 = RenderSize.Width - halfThickness;
                double right2 = right1 - DEF_LINE_THICKNESS;
                double bottom1 = RenderSize.Height - halfThickness;
                double bottom2 = bottom1 - DEF_LINE_THICKNESS;

                double offset = 0.5d;
                GuidelineSet set = new GuidelineSet(new double[] { left1 - offset, left2 - offset, right2 + offset, right1 + offset }, new double[] { top1 - offset, top2 - offset, bottom2 + offset, bottom1 + offset });

                set.Freeze();
                dc.PushGuidelineSet(set);

                Pen penBrightest = !IsInverted ? m_penBrightest : m_penDarkest;
                Pen penBright = !IsInverted ? m_penBright : m_penDark;
                Pen penDark = !IsInverted ? m_penDark : m_penBright;
                Pen penDarkest = !IsInverted ? m_penDarkest : m_penBrightest;

                if (BorderThickness.Left > 0)
                {
                    dc.DrawLine(penBrightest, new Point(left1, top1 + CornerRadius.TopLeft), new Point(left1, bottom2 - CornerRadius.BottomLeft));
                    dc.DrawLine(penBright, new Point(left2, top2 + CornerRadius.TopLeft), new Point(left2, bottom2 - CornerRadius.BottomLeft));
                }

                if (BorderThickness.Top > 0)
                {
                    dc.DrawLine(penBrightest, new Point(left1 + CornerRadius.TopLeft, top1), new Point(right2 - CornerRadius.TopRight, top1));
                    dc.DrawLine(penBright, new Point(left2 + CornerRadius.TopLeft, top2), new Point(right2 - CornerRadius.TopRight, top2));
                }

                if (BorderThickness.Right > 0)
                {
                    dc.DrawLine(penDark, new Point(right2, top2 + CornerRadius.TopRight), new Point(right2, bottom2 - CornerRadius.BottomRight));
                    dc.DrawLine(penDarkest, new Point(right1, top1 + CornerRadius.TopRight), new Point(right1, bottom1 - CornerRadius.BottomRight));
                }

                if (BorderThickness.Bottom > 0)
                {
                    dc.DrawLine(penDark, new Point(left2 + CornerRadius.BottomLeft, bottom2), new Point(right2 - CornerRadius.BottomRight, bottom2));
                    dc.DrawLine(penDarkest, new Point(left1 + CornerRadius.BottomLeft, bottom1), new Point(right1 - CornerRadius.BottomRight, bottom1));
                }

                if (CornerRadius.TopLeft > 0 &&
                    (BorderThickness.Left > 0 || BorderThickness.Top > 0))
                {
                    ArcSegment segment1 = new ArcSegment();
                    segment1.Point = new Point(left1 + CornerRadius.TopLeft, top1);
                    segment1.Size = new Size(CornerRadius.TopLeft, CornerRadius.TopLeft);
                    segment1.SweepDirection = SweepDirection.Clockwise;

                    PathFigure figure1 = new PathFigure();
                    figure1.StartPoint = new Point(left1, top1 + CornerRadius.TopLeft);
                    figure1.Segments.Add(segment1);

                    Drawing dr1 = new GeometryDrawing(
                        null,
                        penBrightest,
                        new PathGeometry(new List<PathFigure> { figure1 }));

                    ArcSegment segment2 = new ArcSegment();
                    segment2.Point = new Point(left2 + CornerRadius.TopLeft, top2);
                    segment2.Size = new Size(CornerRadius.TopLeft, CornerRadius.TopLeft);
                    segment2.SweepDirection = SweepDirection.Clockwise;

                    PathFigure figure2 = new PathFigure();
                    figure2.StartPoint = new Point(left2, top1 + CornerRadius.TopLeft);
                    figure2.Segments.Add(segment2);

                    Drawing dr2 = new GeometryDrawing(
                        null,
                        penBright,
                        new PathGeometry(new List<PathFigure> { figure2 }));

                    dc.DrawDrawing(dr1);
                    dc.DrawDrawing(dr2);
                }

                if (CornerRadius.TopRight > 0 &&
                    (BorderThickness.Top > 0 || BorderThickness.Right > 0))
                {
                    ArcSegment segment1 = new ArcSegment();
                    segment1.Point = new Point(right1, top1 + CornerRadius.TopRight);
                    segment1.Size = new Size(CornerRadius.TopRight, CornerRadius.TopRight);
                    segment1.SweepDirection = SweepDirection.Clockwise;

                    PathFigure figure1 = new PathFigure();
                    figure1.StartPoint = new Point(right1 - CornerRadius.TopRight, top1);
                    figure1.Segments.Add(segment1);

                    Drawing dr1 = new GeometryDrawing(
                        null,
                        penBrightest,
                        new PathGeometry(new List<PathFigure> { figure1 }));

                    ArcSegment segment2 = new ArcSegment();
                    segment2.Point = new Point(right2, top2 + CornerRadius.TopRight);
                    segment2.Size = new Size(CornerRadius.TopRight, CornerRadius.TopRight);
                    segment2.SweepDirection = SweepDirection.Clockwise;

                    PathFigure figure2 = new PathFigure();
                    figure2.StartPoint = new Point(right2 - CornerRadius.TopRight, top2);
                    figure2.Segments.Add(segment2);

                    Drawing dr2 = new GeometryDrawing(
                        null,
                        penBright,
                        new PathGeometry(new List<PathFigure> { figure2 }));

                    dc.DrawDrawing(dr1);
                    dc.DrawDrawing(dr2);
                }

                if (CornerRadius.BottomRight > 0 &&
                    (BorderThickness.Right > 0 || BorderThickness.Bottom > 0))
                {
                    ArcSegment segment1 = new ArcSegment();
                    segment1.Point = new Point(right1 - CornerRadius.BottomRight, bottom1);
                    segment1.Size = new Size(CornerRadius.BottomRight, CornerRadius.BottomRight);
                    segment1.SweepDirection = SweepDirection.Clockwise;

                    PathFigure figure1 = new PathFigure();
                    figure1.StartPoint = new Point(right1, bottom1 - CornerRadius.BottomRight);
                    figure1.Segments.Add(segment1);

                    Drawing dr1 = new GeometryDrawing(
                        null,
                        penDarkest,
                        new PathGeometry(new List<PathFigure> { figure1 }));

                    ArcSegment segment2 = new ArcSegment();
                    segment2.Point = new Point(right2 - CornerRadius.BottomRight, bottom2);
                    segment2.Size = new Size(CornerRadius.BottomRight, CornerRadius.BottomRight);
                    segment2.SweepDirection = SweepDirection.Clockwise;

                    PathFigure figure2 = new PathFigure();
                    figure2.StartPoint = new Point(right2, bottom2 - CornerRadius.BottomRight);
                    figure2.Segments.Add(segment2);

                    Drawing dr2 = new GeometryDrawing(
                        null,
                        penDark,
                        new PathGeometry(new List<PathFigure> { figure2 }));

                    dc.DrawDrawing(dr1);
                    dc.DrawDrawing(dr2);
                }

                if (CornerRadius.BottomLeft > 0 &&
                    (BorderThickness.Bottom > 0 || BorderThickness.Left > 0))
                {
                    ArcSegment segment1 = new ArcSegment();
                    segment1.Point = new Point(left1, bottom1 - CornerRadius.BottomLeft);
                    segment1.Size = new Size(CornerRadius.BottomLeft, CornerRadius.BottomLeft);
                    segment1.SweepDirection = SweepDirection.Clockwise;

                    PathFigure figure1 = new PathFigure();
                    figure1.StartPoint = new Point(left1 + CornerRadius.BottomLeft, bottom1);
                    figure1.Segments.Add(segment1);

                    Drawing dr1 = new GeometryDrawing(
                        null,
                        penBrightest,
                        new PathGeometry(new List<PathFigure> { figure1 }));

                    ArcSegment segment2 = new ArcSegment();
                    segment2.Point = new Point(left2, bottom2 - CornerRadius.BottomLeft);
                    segment2.Size = new Size(CornerRadius.BottomLeft, CornerRadius.BottomLeft);
                    segment2.SweepDirection = SweepDirection.Clockwise;

                    PathFigure figure2 = new PathFigure();
                    figure2.StartPoint = new Point(left2 + CornerRadius.BottomLeft, bottom2);
                    figure2.Segments.Add(segment2);

                    Drawing dr2 = new GeometryDrawing(
                        null,
                        penBright,
                        new PathGeometry(new List<PathFigure> { figure2 }));

                    dc.DrawDrawing(dr1);
                    dc.DrawDrawing(dr2);
                }

                dc.Pop();
            }
        }

        #endregion
    }
}
