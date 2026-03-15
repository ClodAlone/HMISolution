// <copyright file="BalloonTipBorder.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Border for the Balloon. 
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class BalloonTipBorder : Decorator
    {
        #region Constants

        /// <summary>
        /// Stores the corner height.
        /// </summary>
        internal static double DEF_CORNERHEIGHT = 20.0;
        #endregion

        #region Private members

        /// <summary>
        /// Stores the path geometry.
        /// </summary>
        private PathGeometry m_path;
        #endregion

        #region DP properties
        /// <summary>
        /// Stores the Background color value.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty = Border.BackgroundProperty.AddOwner(typeof(BalloonTipBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Stores the BorderBrush value.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty = Border.BorderBrushProperty.AddOwner(typeof(BalloonTipBorder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Stores the BorderThickness value.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty = Border.BorderThicknessProperty.AddOwner(typeof(BalloonTipBorder), new FrameworkPropertyMetadata(new Thickness(0.0), FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Stores the CornerRadius value.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(BalloonTipBorder), new FrameworkPropertyMetadata(new CornerRadius(0.0), FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        public Brush Background
        {
            get
            {
                return (Brush)base.GetValue(BackgroundProperty);
            }

            set
            {
                base.SetValue(BackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border brush.
        /// </summary>
        /// <value>The border brush.</value>
        public Brush BorderBrush
        {
            get
            {
                return (Brush)base.GetValue(BorderBrushProperty);
            }

            set
            {
                base.SetValue(BorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border thickness.
        /// </summary>
        /// <value>The border thickness.</value>
        public Thickness BorderThickness
        {
            get
            {
                return (Thickness)base.GetValue(BorderThicknessProperty);
            }

            set
            {
                base.SetValue(BorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>The corner radius.</value>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)base.GetValue(CornerRadiusProperty);
            }

            set
            {
                base.SetValue(CornerRadiusProperty, value);
            }
        }
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
            BalloonTip parent = (BalloonTip)VisualUtils.FindAncestor(this, typeof(BalloonTip));
            Rect arrangeRect = new Rect(arrangeSize);

            double horMargin = Margin.Left + Margin.Right;
            double verMargin = Margin.Top + Margin.Bottom;

            arrangeRect.Width = Math.Max(0.0, arrangeSize.Width - horMargin);
            if (parent.BalloonTipShape == BalloonTipShapes.Balloon)
            {
                arrangeRect.Height = Math.Max(0.0, arrangeSize.Height - DEF_CORNERHEIGHT - verMargin);
            }
            else
            {
                arrangeRect.Height = Math.Max(0.0, arrangeSize.Height - verMargin);
            }

            arrangeRect.X = Math.Min(Margin.Left, arrangeSize.Width - horMargin);
            arrangeRect.Y = Math.Min(Margin.Top, arrangeSize.Height - verMargin);

            if (Child != null)
            {
                Child.Arrange(arrangeRect);
            }

            m_path = new PathGeometry();
            PathFigure pathFigure = null;

            if (parent.BalloonTipShape == BalloonTipShapes.Balloon)
            {
                pathFigure = new PathFigure(
                    new Point(CornerRadius.TopLeft, 0.0),
                    new PathSegment[]
                    {
                        new LineSegment(new Point(arrangeSize.Width - CornerRadius.TopRight, 0.0), true),
                        new ArcSegment(new Point(arrangeSize.Width, CornerRadius.TopRight), new Size(CornerRadius.TopRight, CornerRadius.TopRight), 0.0, false, SweepDirection.Clockwise, true),
                        new LineSegment(new Point(arrangeSize.Width, arrangeSize.Height - CornerRadius.BottomRight - DEF_CORNERHEIGHT), true),
                        new ArcSegment(new Point(arrangeSize.Width - CornerRadius.BottomRight, arrangeSize.Height - DEF_CORNERHEIGHT), new Size(CornerRadius.BottomRight, CornerRadius.BottomRight), 0.0, false, SweepDirection.Clockwise, true),
                        new LineSegment(new Point(arrangeSize.Width - CornerRadius.BottomRight - DEF_CORNERHEIGHT, arrangeSize.Height - DEF_CORNERHEIGHT), true),
                        new LineSegment(new Point(arrangeSize.Width - CornerRadius.BottomRight - DEF_CORNERHEIGHT, arrangeSize.Height), true),
                        new LineSegment(new Point(arrangeSize.Width - CornerRadius.BottomRight - DEF_CORNERHEIGHT - DEF_CORNERHEIGHT, arrangeSize.Height - DEF_CORNERHEIGHT), true),
                        new LineSegment(new Point(CornerRadius.BottomLeft, arrangeSize.Height - DEF_CORNERHEIGHT), true),
                        new ArcSegment(new Point(0.0, arrangeSize.Height - DEF_CORNERHEIGHT - CornerRadius.BottomLeft), new Size(CornerRadius.BottomRight, CornerRadius.BottomRight), 0.0, false, SweepDirection.Clockwise, true),
                        new LineSegment(new Point(0.0, CornerRadius.TopLeft), true),
                        new ArcSegment(new Point(CornerRadius.TopLeft, 0.0), new Size(CornerRadius.BottomRight, CornerRadius.BottomRight), 0.0, false, SweepDirection.Clockwise, true)
                    },
                    false);
            }
            else
            {
                pathFigure = new PathFigure(
                    new Point(CornerRadius.TopLeft, 0.0),
                    new PathSegment[]
                    {
                        new LineSegment(new Point(arrangeSize.Width - CornerRadius.TopRight, 0.0), true),
                        new ArcSegment(new Point(arrangeSize.Width, CornerRadius.TopRight), new Size(CornerRadius.TopRight, CornerRadius.TopRight), 0.0, false, SweepDirection.Clockwise, true),
                        new LineSegment(new Point(arrangeSize.Width, arrangeSize.Height - CornerRadius.BottomRight), true),
                        new ArcSegment(new Point(arrangeSize.Width - CornerRadius.BottomRight, arrangeSize.Height), new Size(CornerRadius.BottomRight, CornerRadius.BottomRight), 0.0, false, SweepDirection.Clockwise, true),
                        new LineSegment(new Point(CornerRadius.BottomLeft, arrangeSize.Height), true),
                        new ArcSegment(new Point(0.0, arrangeSize.Height - CornerRadius.BottomLeft), new Size(CornerRadius.BottomRight, CornerRadius.BottomRight), 0.0, false, SweepDirection.Clockwise, true),
                        new LineSegment(new Point(0.0, CornerRadius.TopLeft), true),
                        new ArcSegment(new Point(CornerRadius.TopLeft, 0.0), new Size(CornerRadius.BottomRight, CornerRadius.BottomRight), 0.0, false, SweepDirection.Clockwise, true)
                    },
                    false);
            }

            m_path.Figures.Add(pathFigure);

            return arrangeSize;
        }

        /// <summary>
        /// Measures the child elements of a <see cref="T:System.Windows.Controls.Border"/> before they are arranged during the <see cref="M:System.Windows.Controls.Border.ArrangeOverride(System.Windows.Size)"/> pass.
        /// </summary>
        /// <param name="constraint">An upper <see cref="T:System.Windows.Size"/> limit that cannot be exceeded.</param>
        /// <returns>
        /// The <see cref="T:System.Windows.Size"/> that represents the upper size limit of the element.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (Child != null)
            {
                BalloonTip parent = (BalloonTip)VisualUtils.FindAncestor(this, typeof(BalloonTip));
                Size size = constraint;

                double horMargin = Margin.Left + Margin.Right;
                double verMargin = Margin.Top + Margin.Bottom;

                size.Width = Math.Max(0.0, size.Width - horMargin);
                if (parent.BalloonTipShape == BalloonTipShapes.Balloon)
                {
                    size.Height = Math.Max(0.0, size.Height - verMargin - DEF_CORNERHEIGHT);
                }
                else
                {
                    size.Height = Math.Max(0.0, size.Height - verMargin);
                }

                this.Child.Measure(size);
                Size desiredSize = Child.DesiredSize;

                desiredSize.Width = desiredSize.Width + horMargin;
                desiredSize.Height = desiredSize.Height + verMargin;

                if (parent.BalloonTipShape == BalloonTipShapes.Balloon)
                {
                    desiredSize.Height = desiredSize.Height + DEF_CORNERHEIGHT;
                }

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

            Pen pen = new Pen(BorderBrush, BorderThickness.Top);
            drawingContext.DrawGeometry(Background, pen, m_path);
        }
        #endregion
    }
}
