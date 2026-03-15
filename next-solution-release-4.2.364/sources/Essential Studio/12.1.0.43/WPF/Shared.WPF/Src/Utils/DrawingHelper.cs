// <copyright file="DrawingHelper.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Interaction logic for DrawingHelper.xaml
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DrawingHelper : ContentControl
    {
        #region Fields

        /// <summary>
        /// Hash table for maintain drawing brush.
        /// </summary>
        private readonly Hashtable m_drawingsHash = new Hashtable();

        /// <summary>
        /// Holds top most visual object reference.
        /// </summary>
        private Visual m_topMostVisual = null;

        /// <summary>
        /// conversion chain count.
        /// </summary>
        private int m_convertionChainCount = 0;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Identifies Drawing dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawingBrushProperty =
            DependencyProperty.Register("DrawingBrush", typeof(DrawingBrush), typeof(DrawingHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnDrawingBrushChanged)));

        #endregion

        #region Methods

        /// <summary>
        /// Called when [drawing brush changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDrawingBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets the points from drawing.
        /// </summary>
        /// <param name="drawing">The drawing.</param>
        /// <returns>Pointes collection.</returns>
        private List<Point> GetPointsFromDrawing(Drawing drawing)
        {
            List<Point> points = new List<Point>();

            if (drawing is DrawingGroup)
            {
                DrawingGroup drawingGroup = drawing as DrawingGroup;

                foreach (Drawing d in drawingGroup.Children)
                {
                    points.AddRange(GetPointsFromDrawing(d));
                }
            }
            else if (drawing is GeometryDrawing)
            {
                points.AddRange(GetPointsFromGeometryDrawing(drawing));
            }
            //// else if (drawing is GlyphRunDrawing)
            // {
            // }
            // else if (drawing is ImageDrawing)
            // {
            // }
            // else if (drawing is VideoDrawing)
            // {
            //// }

            return points;
        }

        /// <summary>
        /// Gets the points from geometry drawing.
        /// </summary>
        /// <param name="drawing">The drawing.</param>
        /// <returns>List of points which is present in geometry drawing.</returns>
        private List<Point> GetPointsFromGeometryDrawing(Drawing drawing)
        {
            GeometryDrawing gdrawing = (GeometryDrawing)drawing;
            List<Point> points = GetPointsFromGeometry(gdrawing.Geometry);

            m_drawingsHash[drawing] = points;

            return points;
        }

        /// <summary>
        /// Gets the points from geometry.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns>List of points which is present in Geometry.</returns>
        private static List<Point> GetPointsFromGeometry(Geometry geometry)
        {
            List<Point> points = new List<Point>();

            if (geometry is CombinedGeometry)
            {
                CombinedGeometry cg = geometry as CombinedGeometry;

                points.AddRange(GetPointsFromGeometry(cg.Geometry1));
                points.AddRange(GetPointsFromGeometry(cg.Geometry2));
            }
            else if (geometry is GeometryGroup)
            {
                GeometryGroup gg = geometry as GeometryGroup;

                foreach (Geometry g in gg.Children)
                {
                    points.AddRange(GetPointsFromGeometry(g));
                }
            }
            else if (geometry is LineGeometry)
            {
                LineGeometry lg = geometry as LineGeometry;
                points.Add(lg.StartPoint);
                points.Add(lg.EndPoint);
            }
            else if (geometry is RectangleGeometry)
            {
                RectangleGeometry rg = geometry as RectangleGeometry;
                points.Add(rg.Rect.TopLeft);
                points.Add(rg.Rect.TopRight);
                points.Add(rg.Rect.BottomRight);
                points.Add(rg.Rect.BottomLeft);
            }
            else
            {
                if (geometry != null)
                {
                    PathGeometry pg = geometry.GetFlattenedPathGeometry();
                    foreach (PathFigure figure in pg.Figures)
                    {
                        points.Add(figure.StartPoint);

                        foreach (PathSegment segment in figure.Segments)
                        {
                            if (segment is LineSegment)
                            {
                                LineSegment ls = segment as LineSegment;
                                points.Add(ls.Point);
                            }
                            else if (segment is ArcSegment)
                            {
                                ArcSegment arcs = segment as ArcSegment;
                                points.Add(arcs.Point);
                            }
                            else if (segment is BezierSegment)
                            {
                                BezierSegment bs = segment as BezierSegment;
                                points.AddRange(new Point[] { bs.Point1, bs.Point2, bs.Point3 });
                            }
                            else if (segment is PolyLineSegment)
                            {
                                PolyLineSegment pls = segment as PolyLineSegment;
                                points.AddRange(pls.Points);
                            }
                            else if (segment is PolyBezierSegment)
                            {
                                PolyBezierSegment pbs = segment as PolyBezierSegment;
                                points.AddRange(pbs.Points);
                            }
                            else if (segment is QuadraticBezierSegment)
                            {
                                QuadraticBezierSegment qbs = segment as QuadraticBezierSegment;
                                points.AddRange(new Point[] { qbs.Point1, qbs.Point2 });
                            }
                            else if (segment is PolyQuadraticBezierSegment)
                            {
                                PolyQuadraticBezierSegment pqbs = segment as PolyQuadraticBezierSegment;
                                points.AddRange(pqbs.Points);
                            }
                        }
                    }
                }

                if (geometry is EllipseGeometry)
                {
                    EllipseGeometry ellipse = geometry as EllipseGeometry;
                    points.Add(ellipse.Center);
                }
            }

            return points;
        }

        /// <summary>
        /// Gets the guideline set for drawing.
        /// </summary>
        /// <param name="drawing">The drawing.</param>
        /// <returns>The guideline set.</returns>
        private GuidelineSet GetGuidelineSetForDrawing(Drawing drawing)
        {
            m_drawingsHash.Clear();

            GetPointsFromDrawing(drawing);

            GuidelineSet set = new GuidelineSet();
            foreach (Drawing dr in m_drawingsHash.Keys)
            {
                List<Point> list = m_drawingsHash[dr] as List<Point>;
                double penWidth = 1;

                if (dr is GeometryDrawing)
                {
                    GeometryDrawing gd = dr as GeometryDrawing;
                    if (gd.Pen != null)
                    {
                        penWidth = gd.Pen.Thickness;
                    }
                }

                for (int i = 0, cnt = list.Count; i < cnt; i++)
                {
                    Point p = list[i];

                    set.GuidelinesX.Add(p.X - penWidth / 2d);
                    set.GuidelinesX.Add(p.X + penWidth / 2d);

                    set.GuidelinesY.Add(p.Y - penWidth / 2d);
                    set.GuidelinesY.Add(p.Y + penWidth / 2d);
                }
            }

            return set;
        }

        /// <summary>
        /// Applies the transform to drawing.
        /// </summary>
        /// <param name="drawing">The drawing.</param>
        /// <param name="generalTransform">The general transform.</param>
        /// <returns>Drawing object from Transform.</returns>
        internal static Drawing ApplyTransformToDrawing(Drawing drawing, GeneralTransform generalTransform)
        {
            if (generalTransform is Transform)
            {
                Transform transform = generalTransform as Transform;

                if (drawing is DrawingGroup)
                {
                    DrawingGroup drawingGroup = drawing as DrawingGroup;
                    drawingGroup.Transform = transform;
                }
                else if (drawing is GeometryDrawing)
                {
                    GeometryDrawing gdrawing = drawing as GeometryDrawing;
                    gdrawing.Geometry.Transform = transform;
                }
                else if (drawing is GlyphRunDrawing)
                {
                    GlyphRunDrawing gr = drawing as GlyphRunDrawing;
                    gr.GlyphRun.BuildGeometry().Transform = transform;
                }
                else if (drawing is ImageDrawing)
                {
                    ImageDrawing id = drawing as ImageDrawing;
                    id.Rect = transform.TransformBounds(id.Rect);
                }
                else if (drawing is VideoDrawing)
                {
                    VideoDrawing vd = drawing as VideoDrawing;
                    vd.Rect = transform.TransformBounds(vd.Rect);
                }
            }

            return drawing;
        }

        /// <summary>
        /// Gets the drawing.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The Drawing.</returns>
        private Drawing GetDrawing(object obj)
        {
            Drawing dr = null;

            if (obj is Visual)
            {
                Visual visual = obj as Visual;

                dr = ConvertVisualToDrawing(visual);
            }
            else if (obj is DrawingBrush)
            {
                DrawingBrush brush = obj as DrawingBrush;

                dr = ConvertDrawingBrushToDrawing(brush);
            }

            return dr;
        }

        /// <summary>
        /// Converts the drawing brush to drawing.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns>Drawing from drawing brush.</returns>
        private static Drawing ConvertDrawingBrushToDrawing(DrawingBrush brush)
        {
            if (brush != null)
            {
                Drawing drawing = brush.Drawing;
                if (drawing != null)
                {
                    return drawing;
                }
            }

            return null;
        }

        /// <summary>
        /// Converts the visual to drawing.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <returns>The Drawing.</returns>
        private Drawing ConvertVisualToDrawing(Visual visual)
        {
            DrawingGroup drawing = null;

            if (visual != null)
            {
                if (m_topMostVisual == null || m_convertionChainCount == 0)
                {
                    m_topMostVisual = visual;
                }

                m_convertionChainCount++;

                drawing = VisualTreeHelper.GetDrawing(visual);
                if (drawing != null)
                {
                    IEnumerable en = LogicalTreeHelper.GetChildren(visual);
                    if (en != null)
                    {
                        IEnumerator enumerator = en.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            Visual child = enumerator.Current as Visual;
                            if (child != null)
                            {
                                Drawing childDrawing = ConvertVisualToDrawing(child);
                                if (childDrawing != null)
                                {
                                    GeneralTransform gt = child.TransformToVisual(m_topMostVisual);
                                    Drawing transformedDrawing = ApplyTransformToDrawing(childDrawing, gt);

                                    drawing.Children.Add(transformedDrawing);
                                }
                            }
                        }
                    }
                }

                m_convertionChainCount--;
            }

            return drawing;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Drawing instance to draw.
        /// </summary>
        public DrawingBrush DrawingBrush
        {
            get
            {
                return (DrawingBrush)GetValue(DrawingBrushProperty);
            }

            set
            {
                SetValue(DrawingBrushProperty, value);
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            Drawing dr = GetDrawing(Content ?? DrawingBrush);

            if (dr != null)
            {
                GuidelineSet set = GetGuidelineSetForDrawing(dr);
                drawingContext.PushGuidelineSet(set);
            }

            base.OnRender(drawingContext);

            if (dr != null)
            {
                if (Content == null)
                {
                    drawingContext.DrawDrawing(dr);
                }

                drawingContext.Pop();
            }
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (VisualChildrenCount > 0)
            {
                UIElement visualChild = (UIElement)GetVisualChild(0);

                if (visualChild != null)
                {
                    visualChild.Measure(availableSize);
                    return visualChild.DesiredSize;
                }
            }

            return new Size(0.0, 0.0);
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            IEnumerable en = LogicalTreeHelper.GetChildren(this);

            if (en != null)
            {
                IEnumerator enumerator = en.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    UIElement child = enumerator.Current as UIElement;
                    if (child != null)
                    {
                        Vector elementOffset = VisualTreeHelper.GetOffset(child);
                        child.Arrange(new Rect(new Point(elementOffset.X, elementOffset.Y), child.DesiredSize));
                    }
                }
            }

            return finalSize;
        }

        #endregion
    }
}