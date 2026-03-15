#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents tab item's classic border.
    /// </summary>
    public class TabItemAdvBorderClassic : TabItemAdvBorder
    {
        #region Constants
        /// <summary>
        /// White color.
        /// </summary>
        private static readonly Color ColorBrightest = Colors.White;

        /// <summary>
        /// LightGray color.
        /// </summary>
        private static readonly Color ColorBright = Color.FromArgb(255, 235, 233, 237);

        /// <summary>
        /// Gray color.
        /// </summary>
        private static readonly Color ColorDark = Color.FromArgb(255, 128, 128, 128);

        /// <summary>
        /// Black color.
        /// </summary>
        private static readonly Color ColorDarkest = Color.FromArgb(255, 64, 64, 64);

        /// <summary>
        /// Defines line thickness.
        /// </summary>
        private const double DEFLINETHICKNESS = 1d;
        #endregion

        #region Private members
        /// <summary>
        /// Path used for drawing of the border.
        /// </summary>
        private Path topOuterPath;

        /// <summary>
        /// Path used for drawing of the border.
        /// </summary>
        private Path topInnerPath;

        /// <summary>
        /// Path used for drawing of the border.
        /// </summary>
        private Path leftOuterPath;

        /// <summary>
        /// Path used for drawing of the border.
        /// </summary>
        private Path leftInnerPath;

        /// <summary>
        /// Path used for drawing of the border.
        /// </summary>
        private Path bottomOuterPath;

        /// <summary>
        /// Path used for drawing of the border.
        /// </summary>
        private Path bottomInnerPath;

        /// <summary>
        /// Path used for drawing of the border.
        /// </summary>
        private Path rightOuterPath;

        /// <summary>
        /// Path used for drawing of the border.
        /// </summary>
        private Path rightInnerPath;

        /// <summary>
        /// Border used for drawing of the border.
        /// </summary>
        private Border backBorder;

        /// <summary>
        /// Indicates whether mouse pointer is over the border.
        /// </summary>
        private bool isMouseOver = false;

        /// <summary>
        /// Indicates whether control is initialized.
        /// </summary>
        private bool isInitialized = false;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the TabItemAdvBorderClassic class.
        /// </summary>
        public TabItemAdvBorderClassic()
        {
            DefaultStyleKey = typeof(TabItemAdvBorderClassic);

            this.MouseEnter += new MouseEventHandler(this.TabItemAdvBorderClassicMouseEnter);
            this.MouseLeave += new MouseEventHandler(this.TabItemAdvBorderClassicMouseLeave);
        }
        #endregion

        #region DP getters and setters
        /// <summary>
        /// Gets or sets the corner radius of the border.
        /// </summary>
        internal CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius) GetValue(CornerRadiusProperty);
            }

            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="CornerRadius"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CornerRadiusChanged;        
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(TabItemAdvBorderClassic), new PropertyMetadata(new PropertyChangedCallback(OnCornerRadiusChanged)));        
        #endregion

        #region Overrides
        /// <summary>
        /// Occurs when the mouse enters an element.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabItemAdvBorderClassicMouseEnter(object sender, MouseEventArgs e)
        {
            if (!this.IsTabItemBorder)
            {
                this.BorderThickness = new Thickness(1);
            }

            if (this.TabControlParent != null && this.TabControlParent.HotTrackingEnabled)
            {
                if (this.backBorder != null && this.TabParent != null && this.TabParent.TabItemParent != null
                    && this.TabParent.TabItemParent.HoverBackground != null)
                {
                    this.backBorder.Background = this.TabParent.TabItemParent.HoverBackground;
                }
            }

            this.isMouseOver = true;
        }

        /// <summary>
        /// Occurs when the mouse leaves an element.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabItemAdvBorderClassicMouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.IsTabItemBorder)
            {
                this.BorderThickness = new Thickness(0);
            }

            if (this.backBorder != null)
            {
                if (this.Background != null)
                {
                    this.backBorder.Background = this.Background;
                }
                else
                {
                    this.backBorder.Background = new SolidColorBrush(ColorBright);
                }
            }

            this.isMouseOver = false;
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) 
        /// call System.Windows.Controls.Control.ApplyTemplate() method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.backBorder = this.GetTemplateChild("BackBorder") as Border;
            this.topInnerPath = this.GetTemplateChild("TopInnerPath") as Path;
            this.topOuterPath = this.GetTemplateChild("TopOuterPath") as Path;
            this.leftInnerPath = this.GetTemplateChild("LeftInnerPath") as Path;
            this.leftOuterPath = this.GetTemplateChild("LeftOuterPath") as Path;
            this.bottomInnerPath = this.GetTemplateChild("BottomInnerPath") as Path;
            this.bottomOuterPath = this.GetTemplateChild("BottomOuterPath") as Path;
            this.rightInnerPath = this.GetTemplateChild("RightInnerPath") as Path;
            this.rightOuterPath = this.GetTemplateChild("RightOuterPath") as Path;

            isInitialized = true;
        }

        /// <summary>
        /// Refreshes border paths.
        /// </summary>
        internal override void RefreshBorderPaths()
        {
            if (!this.IsEnabled)
            {
                this.BorderThickness = new Thickness(0);
            }
        }

        /// <summary>
        /// Initializes paths with data.
        /// </summary>
        private void InitializePaths(Size borderSize)
        {
            if (this.topOuterPath != null)
            {                
                this.topOuterPath.Data = this.GetTopOuterGeometry(borderSize);
            }

            if (this.topInnerPath != null)
            {
                this.topInnerPath.Data = this.GetTopInnerGeometry(borderSize);
            }

            if (this.leftOuterPath != null)
            {
                this.leftOuterPath.Data = this.GetLeftOuterGeometry(borderSize);
            }

            if (this.leftInnerPath != null)
            {
                this.leftInnerPath.Data = this.GetLeftInnerGeometry(borderSize);
            }

            if (this.bottomOuterPath != null)
            {
                this.bottomOuterPath.Data = this.GetBottomOuterGeometry(borderSize);
            }

            if (this.bottomInnerPath != null)
            {
                this.bottomInnerPath.Data = this.GetBottomInnerGeometry(borderSize);
            }

            if (this.rightOuterPath != null)
            {
                this.rightOuterPath.Data = this.GetRightOuterGeometry(borderSize);
            }

            if (this.rightInnerPath != null)
            {
                this.rightInnerPath.Data = this.GetRightInnerGeometry(borderSize);
            }
        }

        /// <summary>
        /// Refreshes border paths.
        /// </summary>
        /// <param name="borderSize">Size of the border.</param>
        protected override void RefreshBorderPaths(Size borderSize)
        {
            if (this.isInitialized)
            {
                if (this.IsTabItemBorder)
                {
                    if (this.TabControlParent != null)
                    {
                        this.BorderThickness = new Thickness(1, 1, 1, 0);
                        this.CornerRadius = new CornerRadius(2, 2, 0, 0);
                        this.backBorder.CornerRadius = new CornerRadius(2);
                        this.backBorder.BorderThickness = new Thickness(2);

                        switch (this.TabControlParent.TabStripPlacement)
                        {
                            case TabStripPlacement.Top:
                                this.CornerRadius = new CornerRadius(2, 0, 0, 0);
                                this.topOuterPath.Stroke = new SolidColorBrush(ColorBrightest);
                                this.leftOuterPath.Stroke = new SolidColorBrush(ColorBrightest);
                                this.topInnerPath.Stroke = new SolidColorBrush(ColorBright);
                                this.leftInnerPath.Stroke = new SolidColorBrush(ColorBright);
                                this.rightOuterPath.Stroke = new SolidColorBrush(ColorDarkest);
                                this.rightInnerPath.Stroke = new SolidColorBrush(ColorDark);
                                break;

                            case TabStripPlacement.Left:
                                this.topOuterPath.Stroke = new SolidColorBrush(ColorBrightest);
                                this.leftOuterPath.Stroke = new SolidColorBrush(ColorDarkest);
                                this.topInnerPath.Stroke = new SolidColorBrush(ColorBright);
                                this.leftInnerPath.Stroke = new SolidColorBrush(ColorDark);
                                this.rightOuterPath.Stroke = new SolidColorBrush(ColorBrightest);
                                this.rightInnerPath.Stroke = new SolidColorBrush(ColorBright);
                                break;

                            case TabStripPlacement.Right:
                                this.topOuterPath.Stroke = new SolidColorBrush(ColorDarkest);
                                this.leftOuterPath.Stroke = new SolidColorBrush(ColorBrightest);
                                this.topInnerPath.Stroke = new SolidColorBrush(ColorDark);
                                this.leftInnerPath.Stroke = new SolidColorBrush(ColorBright);
                                this.rightOuterPath.Stroke = new SolidColorBrush(ColorDarkest);
                                this.rightInnerPath.Stroke = new SolidColorBrush(ColorDark);
                                break;

                            case TabStripPlacement.Bottom:
                                this.topOuterPath.Stroke = new SolidColorBrush(ColorDarkest);
                                this.leftOuterPath.Stroke = new SolidColorBrush(ColorDarkest);
                                this.topInnerPath.Stroke = new SolidColorBrush(ColorDark);
                                this.leftInnerPath.Stroke = new SolidColorBrush(ColorDark);
                                this.rightOuterPath.Stroke = new SolidColorBrush(ColorBrightest);
                                this.rightInnerPath.Stroke = new SolidColorBrush(ColorBright);
                                break;
                        }
                    }
                }

                this.InitializePaths(borderSize);

                if (!this.IsTabItemBorder)
                {
                    this.CornerRadius = new CornerRadius(0);
                    if (this.backBorder != null)
                    {
                        this.backBorder.CornerRadius = new CornerRadius(0);
                    }

                    if (this.IsReversed)
                    {
                        this.topOuterPath.Stroke = new SolidColorBrush(ColorDarkest);
                        this.leftOuterPath.Stroke = new SolidColorBrush(ColorDarkest);
                    }
                    else
                    {
                        this.topOuterPath.Stroke = new SolidColorBrush(ColorBrightest);
                        this.leftOuterPath.Stroke = new SolidColorBrush(ColorBrightest);
                    }

                    if (this.IsReversed)
                    {
                        this.topInnerPath.Stroke = new SolidColorBrush(ColorDark);
                        this.leftInnerPath.Stroke = new SolidColorBrush(ColorDark);
                    }
                    else
                    {
                        this.topInnerPath.Stroke = new SolidColorBrush(ColorBright);
                        this.leftInnerPath.Stroke = new SolidColorBrush(ColorBright);
                    }

                    if (this.IsReversed)
                    {
                        this.bottomOuterPath.Stroke = new SolidColorBrush(ColorBrightest);
                        this.rightOuterPath.Stroke = new SolidColorBrush(ColorBrightest);
                    }
                    else
                    {
                        this.bottomOuterPath.Stroke = new SolidColorBrush(ColorDarkest);
                        this.rightOuterPath.Stroke = new SolidColorBrush(ColorDarkest);
                    }

                    if (this.IsReversed)
                    {
                        this.bottomInnerPath.Stroke = new SolidColorBrush(ColorBright);
                        this.rightInnerPath.Stroke = new SolidColorBrush(ColorBright);
                    }
                    else
                    {
                        this.bottomInnerPath.Stroke = new SolidColorBrush(ColorDark);
                        this.rightInnerPath.Stroke = new SolidColorBrush(ColorDark);
                    }
                }

                if (this.backBorder != null)
                {
                    if (this.TabControlParent != null && this.TabControlParent.HotTrackingEnabled && this.isMouseOver)
                    {
                        if (this.TabParent != null && this.TabParent.TabItemParent != null
                            && this.TabParent.TabItemParent.HoverBackground != null)
                        {
                            this.backBorder.Background = this.TabParent.TabItemParent.HoverBackground;
                        }
                    }
                    else
                    {
                        if (this.Background != null)
                        {
                            this.backBorder.Background = this.Background;
                        }
                        else
                        {
                            this.backBorder.Background = new SolidColorBrush(ColorBright);
                        }
                    }

                    this.backBorder.Width = borderSize.Width;
                    this.backBorder.Height = borderSize.Height;
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the left top outer geometry.
        /// </summary>
        /// <param name="borderSize">The size of the border.</param>
        /// <returns>The geometry.</returns>
        private Geometry GetTopOuterGeometry(Size borderSize)
        {
            double halfThickness = DEFLINETHICKNESS / 2d;
            double left = halfThickness;
            double top = halfThickness;
            double right = borderSize.Width - halfThickness;

            return this.GetTopGeometry(left, top, right, false);
        }

        /// <summary>
        /// Gets the left top inner geometry.
        /// </summary>
        /// <param name="borderSize">The size of the border.</param>
        /// <returns>The geometry.</returns>
        private Geometry GetTopInnerGeometry(Size borderSize)
        {
            double halfThickness = DEFLINETHICKNESS / 2d;
            double left = DEFLINETHICKNESS + halfThickness;
            double top = DEFLINETHICKNESS + halfThickness;
            double right = borderSize.Width - halfThickness - DEFLINETHICKNESS;

            return this.GetTopGeometry(left, top, right, true);
        }

        /// <summary>
        /// Gets the left top outer geometry.
        /// </summary>
        /// <param name="borderSize">The size of the border.</param>
        /// <returns>The geometry.</returns>
        private Geometry GetLeftOuterGeometry(Size borderSize)
        {
            double halfThickness = DEFLINETHICKNESS / 2d;
            double left = halfThickness;
            double top = halfThickness;
            double bottom = borderSize.Height - halfThickness;

            return this.GetLeftGeometry(left, top, bottom, false);
        }

        /// <summary>
        /// Gets the left top inner geometry.
        /// </summary>
        /// <param name="borderSize">The size of the border.</param>
        /// <returns>The geometry.</returns>
        private Geometry GetLeftInnerGeometry(Size borderSize)
        {
            double halfThickness = DEFLINETHICKNESS / 2d;
            double left = DEFLINETHICKNESS + halfThickness;
            double top = DEFLINETHICKNESS + halfThickness;
            double bottom = borderSize.Height - halfThickness - DEFLINETHICKNESS;

            return this.GetLeftGeometry(left, top, bottom, true);
        }

        /// <summary>
        /// Gets the right bottom outer geometry.
        /// </summary>
        /// <param name="borderSize">The size of the border.</param>
        /// <returns>The geometry.</returns>
        private Geometry GetBottomOuterGeometry(Size borderSize)
        {
            double halfThickness = DEFLINETHICKNESS / 2d;
            double left = DEFLINETHICKNESS + halfThickness;
            double right = borderSize.Width - halfThickness;
            double bottom = borderSize.Height - halfThickness;

            return this.GetBottomGeometry(left, right, bottom, false);
        }

        /// <summary>
        /// Gets the right bottom inner geometry.
        /// </summary>
        /// <param name="borderSize">The size of the border.</param>
        /// <returns>The geometry.</returns>
        private Geometry GetBottomInnerGeometry(Size borderSize)
        {
            double halfThickness = DEFLINETHICKNESS / 2d;
            double left = (DEFLINETHICKNESS * 2) + halfThickness;
            double right = borderSize.Width - halfThickness - DEFLINETHICKNESS;
            double bottom = borderSize.Height - halfThickness - DEFLINETHICKNESS;

            return this.GetBottomGeometry(left, right, bottom, true);
        }

        /// <summary>
        /// Gets the right bottom outer geometry.
        /// </summary>
        /// <param name="borderSize">The size of the border.</param>
        /// <returns>The geometry.</returns>
        private Geometry GetRightOuterGeometry(Size borderSize)
        {
            double halfThickness = DEFLINETHICKNESS / 2d;
            double top = halfThickness;
            double right = borderSize.Width - halfThickness;
            double bottom = borderSize.Height - halfThickness;

            return this.GetRightGeometry(top, right, bottom, false);
        }
        
        /// <summary>
        /// Gets the right bottom inner geometry.
        /// </summary>
        /// <param name="borderSize">The size of the border.</param>
        /// <returns>The geometry.</returns>
        private Geometry GetRightInnerGeometry(Size borderSize)
        {
            double halfThickness = DEFLINETHICKNESS / 2d;
            double top = DEFLINETHICKNESS + halfThickness;
            double right = borderSize.Width - halfThickness - DEFLINETHICKNESS;
            double bottom = borderSize.Height - halfThickness - DEFLINETHICKNESS;

            return this.GetRightGeometry(top, right, bottom, true);
        }

        /// <summary>
        /// Gets the top geometry.
        /// </summary>
        /// <param name="left">Left side of the border.</param>
        /// <param name="top">Top side of the border.</param>
        /// <param name="right">Right side of the border.</param>
        /// <param name="isInner">Is inner geometry.</param>
        /// <returns>The geometry.</returns>
        private Geometry GetTopGeometry(double left, double top, double right, bool isInner)
        {
            PathGeometry topGeom = new PathGeometry();
            PathFigure figure;
            LineSegment line;

            double adjustValue = isInner ? 0.5 : 0;

            if (BorderThickness.Top > 0)
            {
                figure = new PathFigure();
                figure.StartPoint = new Point(left + CornerRadius.TopLeft, top);
                line = new LineSegment();
                line.Point = new Point(right - CornerRadius.TopRight, top);
                figure.Segments.Add(line);
                topGeom.Figures.Add(figure);
            }

            if (CornerRadius.TopLeft > 0 && (BorderThickness.Left > 0 || BorderThickness.Top > 0))
            {
                figure = new PathFigure();
                figure.StartPoint = new Point(left, top + CornerRadius.TopLeft - adjustValue);
                ArcSegment segment = new ArcSegment();
                segment.Point = new Point(left + CornerRadius.TopLeft - adjustValue, top);
                segment.Size = new Size(CornerRadius.TopLeft, CornerRadius.TopLeft);
                segment.SweepDirection = SweepDirection.Counterclockwise;
                figure.Segments.Add(segment);
                figure.IsClosed = false;
                topGeom.Figures.Add(figure);
            }
            
            if (CornerRadius.TopRight > 0 && (BorderThickness.Top > 0 || BorderThickness.Right > 0))
            {
                figure = new PathFigure();
                figure.StartPoint = new Point(right, top + CornerRadius.TopRight - adjustValue);
                ArcSegment segment = new ArcSegment();
                segment.Point = new Point(right - CornerRadius.TopRight + adjustValue, top);
                segment.Size = new Size(CornerRadius.TopRight, CornerRadius.TopRight);
                segment.SweepDirection = SweepDirection.Counterclockwise;
                figure.Segments.Add(segment);
                figure.IsClosed = false;
                topGeom.Figures.Add(figure);
            }

            return topGeom;
        }

        /// <summary>
        /// Gets the left geometry.
        /// </summary>
        /// <param name="left">Left side of the border.</param>
        /// <param name="top">Top side of the border.</param>
        /// <param name="bottom">Bottom side of the border.</param>
        /// <param name="isInner">Is inner geometry.</param>
        /// <returns>The geometry.</returns>
        private Geometry GetLeftGeometry(double left, double top, double bottom, bool isInner)
        {
            PathGeometry leftGeom = new PathGeometry();
            PathFigure figure;
            LineSegment line;

            double adjustValue = isInner ? 0.5 : 0;

            if (BorderThickness.Left > 0)
            {
                figure = new PathFigure();
                figure.StartPoint = new Point(left, top + CornerRadius.TopLeft);
                line = new LineSegment();
                line.Point = new Point(left, bottom - CornerRadius.BottomLeft);
                figure.Segments.Add(line);
                leftGeom.Figures.Add(figure);
            }

            if (CornerRadius.TopLeft > 0 && (BorderThickness.Left > 0 || BorderThickness.Top > 0))
            {
                figure = new PathFigure();
                figure.StartPoint = new Point(left, top + CornerRadius.TopLeft - adjustValue);
                ArcSegment segment = new ArcSegment();
                segment.Point = new Point(left + CornerRadius.TopLeft - adjustValue, top);
                segment.Size = new Size(CornerRadius.TopLeft, CornerRadius.TopLeft);
                segment.SweepDirection = SweepDirection.Counterclockwise;
                figure.Segments.Add(segment);
                figure.IsClosed = false;
                leftGeom.Figures.Add(figure);
            }

            if (CornerRadius.BottomLeft > 0 && (BorderThickness.Left > 0 || BorderThickness.Bottom > 0))
            {
                figure = new PathFigure();
                figure.StartPoint = new Point(left, bottom - CornerRadius.BottomLeft + adjustValue);
                ArcSegment segment = new ArcSegment();
                segment.Point = new Point(left + CornerRadius.BottomLeft - adjustValue, bottom);
                segment.Size = new Size(CornerRadius.BottomLeft, CornerRadius.BottomLeft);
                segment.SweepDirection = SweepDirection.Clockwise;
                figure.Segments.Add(segment);
                figure.IsClosed = false;
                leftGeom.Figures.Add(figure);
            }

            return leftGeom;
        }

        /// <summary>
        /// Gets the bottom geometry.
        /// </summary>
        /// <param name="left">Left side of the border.</param>
        /// <param name="right">Right side of the border.</param>
        /// <param name="bottom">Bottom side of the border.</param>
        /// <param name="isInner">Is inner geometry.</param>
        /// <returns>The geometry.</returns>
        private Geometry GetBottomGeometry(double left, double right, double bottom, bool isInner)
        {
            PathGeometry bottomGeom = new PathGeometry();
            PathFigure figure;
            LineSegment line;

            double adjustValue = isInner ? 0.5 : 0;

            if (BorderThickness.Bottom > 0)
            {
                figure = new PathFigure();
                figure.StartPoint = new Point(left + CornerRadius.TopLeft, bottom);
                line = new LineSegment();
                line.Point = new Point(right - CornerRadius.TopRight, bottom);
                figure.Segments.Add(line);
                bottomGeom.Figures.Add(figure);
            }

            if (CornerRadius.BottomRight > 0 && (BorderThickness.Bottom > 0 || BorderThickness.Right > 0))
            {
                figure = new PathFigure();
                figure.StartPoint = new Point(right, bottom - CornerRadius.BottomRight + adjustValue);
                ArcSegment segment = new ArcSegment();
                segment.Point = new Point(right - CornerRadius.BottomRight + adjustValue, bottom);
                segment.Size = new Size(CornerRadius.BottomRight, CornerRadius.BottomRight);
                segment.SweepDirection = SweepDirection.Clockwise;
                figure.Segments.Add(segment);
                figure.IsClosed = false;
                bottomGeom.Figures.Add(figure);
            }

            if (CornerRadius.BottomLeft > 0 && (BorderThickness.Bottom > 0 || BorderThickness.Left > 0))
            {
                figure = new PathFigure();
                figure.StartPoint = new Point(left, bottom - CornerRadius.BottomLeft + adjustValue);
                ArcSegment segment = new ArcSegment();
                segment.Point = new Point(left + CornerRadius.BottomLeft - adjustValue, bottom);
                segment.Size = new Size(CornerRadius.BottomLeft, CornerRadius.BottomLeft);
                segment.SweepDirection = SweepDirection.Clockwise;
                figure.Segments.Add(segment);
                figure.IsClosed = false;
                bottomGeom.Figures.Add(figure);
            }

            return bottomGeom;
        }

        /// <summary>
        /// Gets the right geometry.
        /// </summary>
        /// <param name="top">Top side of the border.</param>
        /// <param name="right">Right side of the border.</param>
        /// <param name="bottom">Bottom side of the border.</param>
        /// <param name="isInner">Is inner geometry.</param>
        /// <returns>The geometry.</returns>
        private Geometry GetRightGeometry(double top, double right, double bottom, bool isInner)
        {
            PathGeometry rightBottomGeom = new PathGeometry();
            PathFigure figure;
            LineSegment line;

            double adjustValue = isInner ? 0.5 : 0;

            if (BorderThickness.Right > 0)
            {
                figure = new PathFigure();
                figure.StartPoint = new Point(right, top + CornerRadius.TopLeft);
                line = new LineSegment();
                line.Point = new Point(right, bottom - CornerRadius.BottomLeft);
                figure.Segments.Add(line);
                rightBottomGeom.Figures.Add(figure);
            }

            if (CornerRadius.BottomRight > 0 && (BorderThickness.Bottom > 0 || BorderThickness.Right > 0))
            {
                figure = new PathFigure();
                figure.StartPoint = new Point(right, bottom - CornerRadius.BottomRight + adjustValue);
                ArcSegment segment = new ArcSegment();
                segment.Point = new Point(right - CornerRadius.BottomRight + adjustValue, bottom);
                segment.Size = new Size(CornerRadius.BottomRight, CornerRadius.BottomRight);
                segment.SweepDirection = SweepDirection.Clockwise;
                figure.Segments.Add(segment);
                figure.IsClosed = false;
                rightBottomGeom.Figures.Add(figure);
            }

            if (CornerRadius.TopRight > 0 && (BorderThickness.Right > 0 || BorderThickness.Top > 0))
            {
                figure = new PathFigure();
                figure.StartPoint = new Point(right, top + CornerRadius.TopRight - adjustValue);
                ArcSegment segment = new ArcSegment();
                segment.Point = new Point(right - CornerRadius.TopRight + adjustValue, top);
                segment.Size = new Size(CornerRadius.TopRight, CornerRadius.TopRight);
                segment.SweepDirection = SweepDirection.Counterclockwise;
                figure.Segments.Add(segment);
                figure.IsClosed = false;
                rightBottomGeom.Figures.Add(figure);
            }

            return rightBottomGeom;
        }

        /// <summary>
        /// Calls OnCornerRadiusChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdvBorderClassic instance = (TabItemAdvBorderClassic) d;
            instance.OnCornerRadiusChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises CornerRadiusChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.CornerRadiusChanged != null)
            {
                this.CornerRadiusChanged(this, e);
            }
        }
        #endregion
    }    
}
