#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;

namespace Syncfusion.Windows.Diagram
{
    public partial class FourQuadrantPanel : Panel, IScrollInfo
    {
        public DiagramView view = null;
        public FourQuadrantPanel()
            : base()
        {
            Canvas.SetZIndex(this, -1);
            this.Loaded += new RoutedEventHandler(ScrollableGrid_Loaded);
            this.zoomTransform = new ScaleTransform();
        }

        # region Drawing Tools declaration

        Node n = new Node(Guid.NewGuid());
        private bool mousep = false;
        internal System.Windows.Shapes.Path tempath;
        internal System.Windows.Shapes.Path draw_ell = new System.Windows.Shapes.Path();
        internal PolyLineSegment temppoly;
        internal PathGeometry tempbezier;
        internal PathFigure pf;
        internal DateTime timeRightClick;
        private Point ortho1 = new Point();
        private Point ortho2 = new Point();
        private Point ortho3 = new Point();
        internal double oldx;
        internal double oldy;
        internal Point point3;
        internal BezierSegment bs;
        internal Rect rt = new Rect();
        internal bool DrawAllow = false;
        internal bool IsExtent = true;
        Canvas drawcanvas = new Canvas();

        #endregion
        private const double LineSize = 16;
        private const double WheelSize = 3 * LineSize;
        private bool _CanHorizontallyScroll;
        private bool _CanVerticallyScroll;
        private ScrollViewer _ScrollOwner;
        internal Point _Offset;
        private Size _Extent;
        private Size _Viewport;
        Size INeed;
        Size ButIHave;
        internal DiagramControl dc;
        internal Size Constraint;
        bool evenctcheck = true;
        bool horizontalcheck = true;
        bool verticalcheck = true;
        double Viewheight;
        double ViewWidth;
        void ScrollableGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //  view = (DiagramView)this.Parent;
            this.MouseLeftButtonUp += new MouseButtonEventHandler(ScrollableGrid_MouseLeftButtonUp);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(ScrollableGrid_MouseLeftButtonDown);
            this.MouseRightButtonDown += new MouseButtonEventHandler(ScrollableGrid_MouseRightButtonDown);
            this.MouseRightButtonUp += new MouseButtonEventHandler(ScrollableGrid_MouseRightButtonUp);
            this.MouseMove += new MouseEventHandler(ScrollableGrid_MouseMove);
            _ScrollOwner.Loaded += new RoutedEventHandler(_ScrollOwner_Loaded);
            _ScrollOwner.MouseRightButtonDown += new MouseButtonEventHandler(_ScrollOwner_MouseRightButtonDown);
            _ScrollOwner.MouseLeftButtonUp += new MouseButtonEventHandler(_ScrollOwner_MouseLeftButtonUp);
            this.SizeChanged += new SizeChangedEventHandler(ScrollableGrid_SizeChanged);
            VerifyScrollVirtual();
            view.VerifyVirtualization();
        }

        void ScrollableGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            view.UpdateLayout();
        }

        void ScrollableGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (view != null)
            {
                if (view.EnableDrawingTools == true)
                {
                    if (view.DrawingTool == DrawingTools.Polygon)
                    {
                        (view.Page.Children[view.Page.Children.Count - 1] as Node).Measure(new Size((view.Page.Children[view.Page.Children.Count - 1] as Node).Width, (view.Page.Children[view.Page.Children.Count - 1] as Node).Height));
                        (view.Page.Children[view.Page.Children.Count - 1] as Node).Arrange(new Rect((view.Page.Children[view.Page.Children.Count - 1] as Node).OffsetX, (view.Page.Children[view.Page.Children.Count - 1] as Node).OffsetY, (view.Page.Children[view.Page.Children.Count - 1] as Node)._Width, (view.Page.Children[view.Page.Children.Count - 1] as Node)._Height));
                        view.Page.InvalidateMeasure();
                        if(this.view.DrawingMode==DrawingMode.Default)
                        this.view.EnableDrawingTools = false;
                    }
                    else if (this.view.DrawingTool == DrawingTools.PolyLine)
                    {
                        if (temppoly != null)
                        {
                            ConnectorBase line = null;
                            line = new LineConnector();
                            line.ConnectorType = ConnectorType.Straight;
                            line.m_LineDrawing = true;
                            line.PxStartPointPosition = pf.StartPoint;
                            line.IntermediatePoints = new List<Point>();
                            for (int i = 0; i < temppoly.Points.Count; i++)
                            {
                                if (i != temppoly.Points.Count - 1)
                                    line.IntermediatePoints.Add(temppoly.Points[i]);
                            }
                            line.PxEndPointPosition = temppoly.Points[temppoly.Points.Count - 1];
                            this.view.Page.Children.Remove(tempath);
                            drawcanvas.Children.Remove(tempath);
                            (view.Page as DiagramPage).Children.Remove(drawcanvas);
                            tempath = null;
                            temppoly = null;
                            line.UpdateConnectorPathGeometry();
                            dc.Model.Connections.Add(line);
                            view.SelectionList.Clear();
                            view.SelectionList.Add(line);
                            e.Handled = true;
                            line.Focus();
                            if (this.view.DrawingMode == DrawingMode.Default)
                            this.view.EnableDrawingTools = false;
                            timeRightClick = DateTime.Now;
                        }
                    }
                }
            }
        }

        void _ScrollOwner_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
        private Point PanStartPoint = new Point(0, 0);
        private Point oldstartpoint = new Point(0, 0);
        internal bool NodeDragged = false;
        //private void AllowDraw(double newoffsetx,double newoffsety)
        //{

        //     //double x =0;
        //     //double y = 0;


        //    if (newoffsetx > (this.view.Page as DiagramPage).ActualWidth && newoffsety > (this.view.Page as DiagramPage).ActualHeight)
        //    {
        //        (this.view.Page as DiagramPage).Width = newoffsetx;
        //        (this.view.Page as DiagramPage).Height = newoffsety;
        //    }
        //    else if (newoffsetx > (this.view.Page as DiagramPage).ActualWidth)
        //    {
        //        (this.view.Page as DiagramPage).Width = newoffsetx;

        //    }
        //    else if (newoffsety > (this.view.Page as DiagramPage).ActualHeight)
        //    {
        //        (this.view.Page as DiagramPage).Height = newoffsety;

        //    }

        //     //x = newoffsetx;
        //     //y = newoffsety;
        //}
        void ScrollableGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (this.view.IsPageEditable)
            {
                #region Drawing Tools
                if ((this.view as DiagramView).EnableDrawingTools == true)
                {
                    IsExtent = true;
                    if (tempath != null)
                    {
                        if (this.view.DrawingTool == DrawingTools.Polygon)
                        {

                            draw_ell = new System.Windows.Shapes.Path();
                            draw_ell.Data = new PathGeometry();
                            // string val = ((drawcanvas.Children[0] as Path).Data).ToString();
                            // string pathXaml =
                            //"<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Data=\"" + val + "\">" +
                            //                     "</Path>";
                            // draw_ell = System.Windows.Markup.XamlReader.Load(pathXaml) as System.Windows.Shapes.Path;
                            double boundx = (((drawcanvas.Children[0] as Path).Data) as PathGeometry).Bounds.X;
                            double boundy = (((drawcanvas.Children[0] as Path).Data) as PathGeometry).Bounds.Y;
                            double boundheight = (((drawcanvas.Children[0] as Path).Data) as PathGeometry).Bounds.Height;
                            double boundwidth = (((drawcanvas.Children[0] as Path).Data) as PathGeometry).Bounds.Width;
                            Node n2 = new Node();
                            n2.OffsetX = boundx;
                            n2.OffsetY = boundy;
                            n2.Width = boundwidth;
                            n2.Height = boundheight;
                            n2.Shape = Shapes.CustomPath;
                            //n2.NodeShape = System.Windows.Markup.XamlReader.Load(pathXaml) as System.Windows.Shapes.Path;
                            n2.NodeShape = tempath as System.Windows.Shapes.Path;
                            n2.NodeShape.Stretch = Stretch.Fill;
                            tempath.IsHitTestVisible = false;
                            this.view.Page.Children.Remove(tempath);
                            drawcanvas.Children.Remove(tempath);
                            (view.Page as DiagramPage).Children.Remove(drawcanvas);
                            tempath = null;
                            dc.Model.Nodes.Add(n2);
                            view.SelectionList.Clear();
                            view.SelectionList.Add(n2);
                            n2.Focus();
                            e.Handled = true;
                            n2.m_NodeDrawing = true;
                            timeRightClick = DateTime.Now;

                        }

                    }

                }

                #endregion
            }
        }
        void ScrollableGrid_MouseMove(object sender, MouseEventArgs e)
        {


            if (view.IsPageEditable)
            {
                if (view.IsPanEnabled)
                {
                    this.Cursor = Cursors.Hand;
                    if (this.view.Page.CaptureMouse())
                    {
                        var ScrollablePoint = e.GetPosition(view.ScrollGrid._ScrollOwner);

                        var PagePoint = e.GetPosition(view.Page);
                        
                        if (HorizontalOffset < 1 && VerticalOffset < 1)
                        {
                            PanningPage(new Point(ScrollablePoint.X + (oldpoint.X * view.CurrentZoom) - this.PanStartPoint.X, ScrollablePoint.Y + (oldpoint.Y * view.CurrentZoom) - this.PanStartPoint.Y));
                        }
                        else
                        {
                            PanningPage(new Point(ScrollablePoint.X - (oldpoint.X * view.CurrentZoom) - this.PanStartPoint.X, ScrollablePoint.Y - (oldpoint.Y * view.CurrentZoom) - this.PanStartPoint.Y));
                        }
                        oldstartpoint = PanStartPoint;
                    }
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                }

                #region DrawingTools



                if ((this.view as DiagramView).EnableDrawingTools == true)
                {
                    double newx = e.GetPosition(this.view.Page).X;
                    double newy = e.GetPosition(this.view.Page).Y;
                    Canvas drawcanvas = new Canvas();
                    Rect drawArea = new Rect(new Point(oldx, oldy), new Point(newx, newy));
                    Border b = new Border();
                    b.Background = new SolidColorBrush(Colors.Gray);
                    Canvas.SetLeft(b, drawArea.X);
                    Canvas.SetTop(b, drawArea.Y);
                    b.Width = drawArea.Width;
                    b.Height = drawArea.Height;
                    n.Opacity = 0.1;
                    drawcanvas.Children.Add(b);

                    DrawingTools i = (this.view as DiagramView).DrawingTool;

                    if ((this.view as DiagramView).SizeToContent == false)
                    {
                        if ((this.view as DiagramView).BoundaryConstraintsEnabled == true)
                        {
                            if (((this.view as DiagramView).BoundaryConstraintsArea.Contains(new Point(newx, newy))))
                            {
                                (this.view as DiagramView).EnableDrawingTools = true;
                                this.DrawAllow = true;
                                (this.view as DiagramView).DrawingTool = i;
                            }
                            else
                            {
                                this.DrawAllow = false;
                            }
                        }
                    }

                    if (this.DrawAllow == true)
                    {

                        if (this.view.DrawingTool == DrawingTools.Polygon)
                        {


                            if (tempath != null)
                            {
                                tempath.Cursor = Cursors.Arrow;
                                tempath.CaptureMouse();
                                pf.IsClosed = true;
                                pf.IsFilled = true;
                                temppoly.Points[temppoly.Points.Count - 1] = e.GetPosition(this.view.Page);
                            }

                        }

                        else if (this.view.DrawingTool == DrawingTools.StraightLine)
                        {
                            if (tempath != null)
                            {

                                tempath.Cursor = Cursors.Arrow;
                                tempath.CaptureMouse();
                                (tempath.Data as LineGeometry).StartPoint = new Point(oldx, oldy);
                                (tempath.Data as LineGeometry).EndPoint = new Point(newx, newy);

                            }
                        }

                        else if (this.view.DrawingTool == DrawingTools.PolyLine)
                        {
                            if (tempath != null)
                            {
                                tempath.Cursor = Cursors.Arrow;
                                tempath.CaptureMouse();
                                temppoly.Points[temppoly.Points.Count - 1] = e.GetPosition(this.view.Page);
                            }
                        }
                        else if (this.view.DrawingTool == DrawingTools.OrthogonalLine)
                        {
                            ortho1 = e.GetPosition(this.view.Page);
                            ortho2 = e.GetPosition(this.view.Page);
                            ortho3 = e.GetPosition(this.view.Page);
                            if (tempath != null)
                            {

                                tempath.Cursor = Cursors.Arrow;
                                tempath.CaptureMouse();
                                double newy1 = new double();
                                double newx1 = new double();
                                if (temppoly.Points.Count <= 3)
                                {
                                    if (oldx > 0)
                                    {
                                        double diffy1 = 25;
                                        if (newy > oldy)
                                        {
                                            newy1 = diffy1 + oldy;
                                        }
                                        else
                                        {
                                            //newy1 = Math.Abs(diffy1 - oldy);
                                            newy1 = (diffy1 - oldy);
                                            newy1 = -newy1;
                                        }

                                        ortho1 = new Point(oldx, newy1);
                                        temppoly.Points[0] = ortho1;
                                        double diffx1 = ((newx - oldx));
                                        if (newx > oldx)
                                        {
                                            newx1 = diffx1 + oldx;
                                        }
                                        else
                                        {
                                            if (diffx1 < 0)
                                            {
                                                newx1 = (diffx1 + oldx);
                                            }
                                            else
                                            {
                                                newx1 = (diffx1 + oldx);
                                            }
                                        }
                                        ortho2 = new Point(newx1, newy1);
                                        temppoly.Points[1] = ortho2;
                                        ortho3 = new Point(newx1, newy);
                                        temppoly.Points[2] = ortho3;
                                    }
                                    else
                                    {
                                        double diffy1 = 25;
                                        if (newy > oldy)
                                        {
                                            newy1 = diffy1 + oldy;
                                        }
                                        else
                                        {
                                            //newy1 = Math.Abs(oldy - diffy1);
                                            newy1 = oldy - diffy1;
                                        }

                                        ortho1 = new Point(oldx, newy1);
                                        temppoly.Points[0] = ortho1;
                                        if (newx > oldx)
                                        {
                                            double diffx1 = ((newx - oldx));
                                            if (diffx1 < 0)
                                            {
                                                newx1 = diffx1 - oldx;
                                            }
                                            else
                                            {
                                                newx1 = diffx1 + oldx;
                                            }

                                        }
                                        else
                                        {
                                            double diffx1 = ((oldx + newx));
                                            if (diffx1 < 0)
                                            {
                                                newx1 = diffx1 - oldx;
                                            }
                                            else
                                            {
                                                newx1 = diffx1 + oldx;
                                            }

                                        }
                                        ortho2 = new Point(newx1, newy1);
                                        temppoly.Points[1] = ortho2;
                                        ortho3 = new Point(newx1, newy);
                                        temppoly.Points[2] = ortho3;
                                    }

                                }
                            }
                        }
                        else if (this.view.DrawingTool == DrawingTools.BezierLine)
                        {

                            if (tempath != null)
                            {

                                tempath.Cursor = Cursors.Arrow;
                                tempath.CaptureMouse();
                                double diffx = Math.Abs(newx - oldx);
                                double x = new double();
                                double y = new double();
                                double dis = new double();
                                Point b1 = new Point();
                                Point b2 = new Point();
                                Point b3 = new Point();

                                if (oldx > 0)
                                {
                                    x = Math.Pow((oldx - newx), 2);
                                    y = Math.Pow((oldy - newy), 2);
                                    dis = Math.Sqrt((x + y) / 4);
                                    if (newx <= oldx)
                                    {
                                        bs.Point1 = new Point(Math.Abs(oldx - dis), oldy);
                                        bs.Point2 = new Point(Math.Abs(newx + dis), newy);
                                        bs.Point3 = new Point(newx, newy);
                                        point3 = bs.Point3;
                                    }
                                    else
                                    {
                                        bs.Point1 = new Point(Math.Abs(oldx - dis), oldy);
                                        bs.Point2 = new Point(Math.Abs(newx + dis), newy);
                                        bs.Point3 = new Point(newx, newy);
                                        point3 = bs.Point3;
                                    }
                                    if (newx <= 0)
                                    {

                                        b1 = new Point((Math.Abs(oldx - dis)), oldy);
                                        b2 = new Point((Math.Abs(newx + dis)), newy);
                                        b3 = new Point(newx, newy);
                                        bs.Point1 = new Point((b1.X), oldy);
                                        bs.Point2 = new Point(-(b2.X), newy);
                                        bs.Point3 = new Point(newx, newy);
                                        point3 = bs.Point3;
                                    }
                                }

                                else if (oldx < 0)
                                {
                                    x = Math.Pow((oldx - newx), 2);
                                    y = Math.Pow((oldy - newy), 2);
                                    dis = Math.Sqrt((x + y) / 4);

                                    b1 = new Point(Math.Abs((-oldx) + dis), oldy);
                                    b2 = new Point(Math.Abs((-newx) - dis), newy);
                                    b3 = new Point(newx, newy);
                                    if (newx >= oldx)
                                    {
                                        b1 = new Point(Math.Abs(oldx - dis), oldy);
                                        b2 = new Point(Math.Abs(newx + dis), newy);
                                        b3 = new Point(newx, newy);
                                        bs.Point1 = new Point(-(b1.X), oldy);
                                        bs.Point2 = new Point(-(b2.X), newy);
                                        bs.Point3 = new Point(newx, newy);
                                        point3 = bs.Point3;
                                    }
                                    else
                                    {
                                        b1 = new Point(Math.Abs(oldx - dis), oldy);
                                        b2 = new Point(Math.Abs(newx + dis), newy);
                                        b3 = new Point(newx, newy);
                                        bs.Point1 = new Point(-(b1.X), oldy);
                                        bs.Point2 = new Point(-(b2.X), newy);
                                        bs.Point3 = new Point(newx, newy);
                                        point3 = bs.Point3;

                                    }
                                    if (newx >= 0)
                                    {

                                        if (newx > 0)
                                        {
                                            b1 = new Point(Math.Abs((oldx) - dis), oldy);
                                            b2 = new Point(Math.Abs((newx) + dis), newy);
                                            b3 = new Point(newx, newy);
                                            bs.Point1 = new Point(-(b1.X), oldy);
                                            bs.Point2 = new Point((b2.X), newy);
                                            bs.Point3 = new Point(newx, newy);
                                            point3 = bs.Point3;
                                        }
                                    }
                                }

                            }
                        }

                        if (mousep)
                        {

                            if (this.view.DrawingTool == DrawingTools.Ellipse)
                            {

                                if (tempath != null)
                                {
                                    tempath.Cursor = Cursors.Arrow;
                                    (tempath.Data as EllipseGeometry).Center = new Point(((newx + oldx) / 2), ((newy + oldy) / 2));
                                    (tempath.Data as EllipseGeometry).RadiusX = Math.Abs((newx - oldx) / 2);
                                    (tempath.Data as EllipseGeometry).RadiusY = Math.Abs((newy - oldy) / 2);
                                    tempath.CaptureMouse();
                                }
                            }
                            else if (this.view.DrawingTool == DrawingTools.Rectangle)
                            {

                                if (tempath != null)
                                {
                                    tempath.Cursor = Cursors.Arrow;
                                    Point startpoint = new Point(oldx, oldy);
                                    tempath.CaptureMouse();
                                    Point startpoint_rect = new Point(oldx, oldy);
                                    if ((e.GetPosition(this.view.Page).Y < oldy))
                                    {
                                        startpoint_rect.X = oldx;
                                        startpoint_rect.Y = e.GetPosition(this.view.Page).Y;
                                    }
                                    if (e.GetPosition(this.view.Page).X < oldx)
                                    {
                                        startpoint_rect.Y = oldy;
                                        startpoint_rect.X = e.GetPosition(this.view.Page).X;
                                    }
                                    if ((e.GetPosition(this.view.Page).X < oldx) && (e.GetPosition(this.view.Page).Y < oldy))
                                    {
                                        startpoint_rect.X = e.GetPosition(this.view.Page).X;
                                        startpoint_rect.Y = e.GetPosition(this.view.Page).Y;
                                    }

                                    (tempath.Data as RectangleGeometry).Rect = new Rect(new Point(startpoint_rect.X, startpoint_rect.Y), new Size(Math.Abs(oldx - newx), Math.Abs(oldy - newy)));

                                }
                            }
                            else if (this.view.DrawingTool == DrawingTools.RoundedRectangle)
                            {
                                if (tempath != null)
                                {
                                    tempath.Cursor = Cursors.Arrow;
                                    tempath.CaptureMouse();
                                    Point startpoint_rect = new Point(oldx, oldy);
                                    if ((e.GetPosition(this.view.Page).Y < oldy))
                                    {
                                        startpoint_rect.X = oldx;
                                        startpoint_rect.Y = e.GetPosition(this.view.Page).Y;
                                    }
                                    if (e.GetPosition(this.view.Page).X < oldx)
                                    {
                                        startpoint_rect.Y = oldy;
                                        startpoint_rect.X = e.GetPosition(this.view.Page).X;
                                    }
                                    if ((e.GetPosition(this.view.Page).X < oldx) && (e.GetPosition(this.view.Page).Y < oldy))
                                    {
                                        startpoint_rect.X = e.GetPosition(this.view.Page).X;
                                        startpoint_rect.Y = e.GetPosition(this.view.Page).Y;
                                    }
                                    (tempath.Data as RectangleGeometry).RadiusX = Math.Abs(newx - oldx) / 16;
                                    (tempath.Data as RectangleGeometry).RadiusY = Math.Abs(newy - oldy) / 16;
                                    (tempath.Data as RectangleGeometry).Rect = new Rect(new Point(startpoint_rect.X, startpoint_rect.Y), new Size(Math.Abs(newx - oldx), Math.Abs(newy - oldy)));
                                }
                            }
                        }
                    }

                #endregion
                }
            }
            else
            {
                this.Cursor = Cursors.Arrow;
            }

            if (this.view.IsPageEditable == true)
            {

                if ((this.view.Page as DiagramPage).startPoint.HasValue)
                {
                    if (this.view.SelectionCanvas.Children.Count > 0)
                    {
                        this.view.SelectionCanvas.Children.Remove(this.view.SelectionCanvas.Children.ElementAt(this.view.SelectionCanvas.Children.Count() - 1));
                    }
                    this.CaptureMouse();

                    {

                        this.view.SelectionList.Clear();
                        (this.view.Page as DiagramPage).endPoint = e.GetPosition(this.view);
                        Rect selectedArea = new Rect((this.view.Page as DiagramPage).startPoint.Value, (this.view.Page as DiagramPage).endPoint);
                        Border selectionadorner = new Border();
                        if ((this.view as DiagramView).EnableDrawingTools == false)
                        {
                            selectionadorner.Background = new SolidColorBrush(Colors.Gray);
                            selectionadorner.BorderBrush = new SolidColorBrush(Colors.Black);
                        }
                        else
                        {
                            selectionadorner.Background = new SolidColorBrush(Colors.Transparent);
                            selectionadorner.BorderBrush = new SolidColorBrush(Colors.Transparent);
                        }

                        selectionadorner.BorderThickness = new Thickness(2);
                        Canvas.SetLeft(selectionadorner, selectedArea.X);
                        Canvas.SetTop(selectionadorner, selectedArea.Y);
                        selectionadorner.Width = selectedArea.Width;
                        selectionadorner.Height = selectedArea.Height;
                        selectionadorner.Opacity = .5;
                        this.view.SelectionCanvas.Children.Add(selectionadorner);
                        foreach (UIElement item in (this.view.Page as DiagramPage).Children)
                        {
                            if (item is Node)
                            {
                                Point p = new Point();
                                p = e.GetPosition(this.view);
                                selectedArea = new Rect((this.view.Page as DiagramPage).spoint, p);
                                Rect itemRect = new Rect(0, 0, (item as Node).ActualWidth, (item as Node).ActualHeight);
                                Rect itemBounds = item.TransformToVisual(this.view).TransformBounds(itemRect);
                                // Rect itemRect = new Rect((item as Node).PxOffsetX, (item as Node).PxOffsetY, item.ActualWidth, item.ActualHeight);
                                // if (selectedArea.Contains(new Point(itemRect.X - this.dview.Scrollviewer.HorizontalOffset, itemRect.Y - this.dview.Scrollviewer.VerticalOffset)) && selectedArea.Contains(new Point(itemRect.X + itemRect.Width - this.dview.Scrollviewer.HorizontalOffset, itemRect.Y + itemRect.Height - this.dview.Scrollviewer.VerticalOffset)))
                                if (selectedArea.Contains(new Point(itemBounds.X, itemBounds.Y)) && selectedArea.Contains(new Point(itemBounds.Right, itemBounds.Bottom)))
                                {
                                    if ((item as Node).AllowSelect)
                                    {
                                        this.view.SelectionList.Add(item);
                                        //(this.view.Page as DiagramPage).SelectionList.Add(item);
                                        (item as Node).IsSelected = true;
                                    }
                                }

                            }
                            else if (item is LineConnector)
                            {
                                double linewidth = ((item as LineConnector).PxEndPointPosition.X - (item as LineConnector).PxStartPointPosition.X);
                                double lineHeight = ((item as LineConnector).PxEndPointPosition.Y - (item as LineConnector).PxStartPointPosition.Y);
                                Rect itemRect = new Rect((item as LineConnector).PxStartPointPosition.X, (item as LineConnector).PxStartPointPosition.Y, Math.Abs(linewidth), Math.Abs(lineHeight));
                                Rect itemBounds = item.TransformToVisual(this.view).TransformBounds(itemRect);
                                if (selectedArea.Contains(new Point(itemBounds.X, itemBounds.Y)) && selectedArea.Contains(new Point(itemBounds.Right, itemBounds.Bottom)) && !(item as LineConnector).IsSelected)
                                {

                                    this.view.SelectionList.Add(item);
                                    (item as LineConnector).IsSelected = true;
                                    //(this.view.Page as DiagramPage).SelectionList.Add(item);

                                }
                            }
                        }
                    }
                }
            }

        }


        internal Point ScrollPoint = new Point(0, 0);
        internal void PanningPage(Point pandelta)
        {
            this._Offset.X = -pandelta.X;
            this._Offset.Y = -pandelta.Y;
            if (this._Offset.X < 0)
            {
                ScrollPoint.X = -this._Offset.X;
            }
            if (this._Offset.Y < 0)
            {
                ScrollPoint.Y = -this._Offset.Y;
            }
            this.InvalidateArrange();
            view.Page.InvalidateMeasure();
            if (ViewportHeight - ExtentHeight == 0)
            {
                if (ExtentWidth - ViewportWidth == 0)
                {
                    VerifyScrollVirtual();
                }
            }
            if (HorizontalOffset == 0)
            {
                if (ExtentWidth - ViewportWidth == 0)
                {
                    VerifyScrollVirtual();
                }

            }

        }

        void _ScrollOwner_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        void _ScrollOwner_Loaded(object sender, RoutedEventArgs e)
        {
            _ScrollOwner.LostMouseCapture += new MouseEventHandler(_ScrollOwner_LostMouseCapture);
            _ScrollOwner.GotFocus += new RoutedEventHandler(_ScrollOwner_GotFocus);
            _ScrollOwner.MouseLeave += new MouseEventHandler(_ScrollOwner_MouseLeave);
            Deployment.Current.Dispatcher.BeginInvoke(() => { this.dc.View.Page.InvalidateMeasure(); });
        }
        void _ScrollOwner_GotFocus(object sender, RoutedEventArgs e)
        {
            //view.focuselement();
        }
        void _ScrollOwner_MouseLeave(object sender, MouseEventArgs e)
        {
            if (view != null && view.EnableVirtualization)
            {
                this.callCalculate();
            }
        }

        void _ScrollOwner_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (view != null)
            {
                view.nodragging = true;
            }
            if (Verticlscroll)
            {
                if (view != null && view.EnableVirtualization)
                {
                    this.callCalculate();
                }
                Verticlscroll = false;
            }
            dc.View.Page.InvalidateMeasure();
            for (int i = 0; i < 20000; i++)
            {
                if (i == 19999)
                {
                    if (view != null && view.EnableVirtualization)
                    {
                        this.callCalculate();
                    }
                }
            }
        }
        Point oldpoint = new Point();
        bool check = true;
        void ScrollableGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (view.IsPageEditable && view.IsPanEnabled)
            {
                if (check)
                {
                    check = false;
                    oldpoint.X = (view.Page as DiagramPage).Left;
                    oldpoint.Y = (view.Page as DiagramPage).Top;
                }
                this.PanStartPoint = e.GetPosition(view.Page);
                this.PanStartPoint.X = this.PanStartPoint.X * view.CurrentZoom;
                this.PanStartPoint.Y = this.PanStartPoint.Y * view.CurrentZoom;
                this.view.Page.CaptureMouse();
            }
            else if (view.IsPageEditable && view.mnodeDownEvent)
            {
                (this.view.Page as DiagramPage).SelectionList.Clear();
                (this.view.Page as DiagramPage).startPoint = e.GetPosition(this.view);
                (this.view.Page as DiagramPage).spoint = e.GetPosition(this.view);
            }

            #region Drawing Tools
          
            mousep = true;
            if (view.IsPageEditable && !view.IsPanEnabled)
            {
                if ((this.view as DiagramView).EnableDrawingTools == true)
                {
                    if (view.mallowDrawandDrag)
                    {
                        oldx = e.GetPosition(this.view.Page as DiagramPage).X;
                        oldy = e.GetPosition(this.view.Page as DiagramPage).Y;
                        this.DrawAllow = true;

                        if (this.DrawAllow == true)
                        {
                            DrawingTools i = (this.view as DiagramView).DrawingTool;
                            if ((this.view as DiagramView).SizeToContent == false)
                            {
                                if ((this.view as DiagramView).BoundaryConstraintsEnabled == true)
                                {
                                    if (((this.view as DiagramView).BoundaryConstraintsArea.Contains(new Point(oldx, oldy))))
                                    {

                                        (this.view as DiagramView).EnableDrawingTools = true;
                                        this.DrawAllow = true;
                                        (this.view as DiagramView).DrawingTool = i;
                                    }
                                    else
                                    {

                                        this.DrawAllow = false;
                                    }
                                }
                            }
                        }
                        if (this.DrawAllow == true)
                        {
                            if (this.view.DrawingTool == DrawingTools.StraightLine)
                            {
                                if (tempath == null)
                                {
                                    tempath = new System.Windows.Shapes.Path();
                                    tempath.Stroke = new SolidColorBrush(Colors.Black);
                                    LineGeometry lg = new LineGeometry();
                                    lg.StartPoint = new Point(oldx, oldy);
                                    lg.EndPoint = new Point(oldx, oldy);

                                    tempath.Data = lg;
                                }

                                tempath.Stroke = new SolidColorBrush(Colors.Black);
                                drawcanvas.Children.Add(tempath);
                                if (this.view.Page.Children.IndexOf(drawcanvas) < 0)
                                    this.view.Page.Children.Add(drawcanvas);
                            }

                            else if (this.view.DrawingTool == DrawingTools.PolyLine)
                            {
                                if (tempath == null)
                                {
                                    tempath = new System.Windows.Shapes.Path();
                                    tempbezier = new PathGeometry();
                                    pf = new PathFigure();
                                    pf.StartPoint = new Point(oldx, oldy);
                                    temppoly = new PolyLineSegment();
                                    pf.Segments.Add(temppoly);
                                    tempbezier.Figures.Add(pf);
                                    tempath.Data = tempbezier;
                                    tempath.Stroke = new SolidColorBrush(Colors.Black);
                                }
                                temppoly.Points.Add(e.GetPosition(this.view.Page));

                                if (!drawcanvas.Children.Contains(tempath))
                                {
                                    drawcanvas.Children.Add(tempath);
                                }
                                if (this.view.Page.Children.IndexOf(drawcanvas) < 0)
                                    this.view.Page.Children.Add(drawcanvas);
                            }
                            else if (this.view.DrawingTool == DrawingTools.OrthogonalLine)
                            {

                                if (tempath == null)
                                {
                                    ortho1 = new Point(oldx, oldy);
                                    ortho2 = new Point(oldx, oldy);
                                    ortho3 = new Point(oldx, oldy);
                                    tempath = new System.Windows.Shapes.Path();
                                    tempbezier = new PathGeometry();
                                    pf = new PathFigure();
                                    pf.StartPoint = new Point(oldx, oldy);
                                    temppoly = new PolyLineSegment();
                                    pf.Segments.Add(temppoly);
                                    tempbezier.Figures.Add(pf);
                                    tempath.Data = tempbezier;
                                    tempath.Stroke = new SolidColorBrush(Colors.Black);
                                }
                                if (temppoly.Points.Count <= 2)
                                {
                                    temppoly.Points.Add(ortho1);
                                    temppoly.Points.Add(ortho2);
                                    temppoly.Points.Add(ortho3);
                                }

                                drawcanvas.Children.Add(tempath);
                                if (this.view.Page.Children.IndexOf(drawcanvas) < 0)
                                    this.view.Page.Children.Add(drawcanvas);
                            }
                            else if (this.view.DrawingTool == DrawingTools.BezierLine)
                            {

                                if (tempath == null)
                                {
                                    tempath = new System.Windows.Shapes.Path();
                                    tempbezier = new PathGeometry();
                                    pf = new PathFigure();
                                    pf.StartPoint = new Point(oldx, oldy);
                                    bs = new BezierSegment();
                                    bs.Point1 = new Point(oldx, oldy);
                                    bs.Point2 = new Point(oldx, oldy);
                                    bs.Point3 = new Point(oldx, oldy);
                                    point3 = bs.Point3;
                                    pf.Segments.Add(bs);
                                    tempbezier.Figures.Add(pf);
                                    tempath.Stroke = new SolidColorBrush(Colors.Black);
                                    tempath.StrokeThickness = 1;
                                    tempath.Data = tempbezier;
                                }
                                drawcanvas.Children.Add(tempath);
                                if (this.view.Page.Children.IndexOf(drawcanvas) < 0)
                                    this.view.Page.Children.Add(drawcanvas);
                            }
                            else if (this.view.DrawingTool == DrawingTools.Ellipse)
                            {

                                if (tempath == null)
                                {
                                    tempath = new System.Windows.Shapes.Path();



                                    Style CPS = new Style();
                                    CPS.BasedOn = n.CustomPathStyle;
                                    CPS.TargetType = typeof(System.Windows.Shapes.Path);
                                    Setter s = new Setter();
                                    s.Property = System.Windows.Shapes.Path.FillProperty;
                                    s.Value = CreateStyle();
                                    CPS.Setters.Add(s);
                                    Setter s1 = new Setter();
                                    s1.Property = System.Windows.Shapes.Path.StrokeProperty;
                                    s1.Value = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                                    CPS.Setters.Add(s1);
                                    Setter s2 = new Setter();
                                    s2.Property = System.Windows.Shapes.Path.StrokeThicknessProperty;
                                    s2.Value = 1d;
                                    CPS.Setters.Add(s2);
                                    n.CustomPathStyle = CPS;
                                    tempath.Style = (n as Node).CustomPathStyle;


                                    EllipseGeometry g = new EllipseGeometry();
                                    g.Center = new Point(oldx, oldy);
                                    g.RadiusX = 0;
                                    g.RadiusY = 0;
                                    tempath.Data = g;
                                }
                                drawcanvas.Children.Add(tempath);

                                if (this.view.Page.Children.IndexOf(drawcanvas) < 0)
                                    this.view.Page.Children.Add(drawcanvas);

                            }

                            else if (this.view.DrawingTool == DrawingTools.Rectangle)
                            {

                                if (tempath == null)
                                {
                                    tempath = new System.Windows.Shapes.Path();
                                    tempath.Stretch = Stretch.None;

                                    Style CPS = new Style();
                                    CPS.BasedOn = n.CustomPathStyle;
                                    CPS.TargetType = typeof(System.Windows.Shapes.Path);
                                    Setter s = new Setter();
                                    s.Property = System.Windows.Shapes.Path.FillProperty;
                                    s.Value = CreateStyle();
                                    CPS.Setters.Add(s);
                                    Setter s1 = new Setter();
                                    s1.Property = System.Windows.Shapes.Path.StrokeProperty;
                                    s1.Value = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                                    CPS.Setters.Add(s1);
                                    Setter s2 = new Setter();
                                    s2.Property = System.Windows.Shapes.Path.StrokeThicknessProperty;
                                    s2.Value = 1d;
                                    CPS.Setters.Add(s2);
                                    n.CustomPathStyle = CPS;
                                    tempath.Style = (n as Node).CustomPathStyle;

                                    RectangleGeometry rg = new RectangleGeometry();
                                    rt.X = oldx;
                                    rt.Y = oldy;
                                    rt.Height = 0;
                                    rt.Width = 0;
                                    rg.Rect = rt;
                                    tempath.Data = rg;

                                }
                                drawcanvas.Children.Add(tempath);
                                if (this.view.Page.Children.IndexOf(drawcanvas) < 0)
                                    this.view.Page.Children.Add(drawcanvas);
                            }
                            else if (this.view.DrawingTool == DrawingTools.RoundedRectangle)
                            {

                                if (tempath == null)
                                {
                                    tempath = new System.Windows.Shapes.Path();
                                    tempath.Stretch = Stretch.None;
                                    Style CPS = new Style();
                                    CPS.BasedOn = n.CustomPathStyle;
                                    CPS.TargetType = typeof(System.Windows.Shapes.Path);
                                    Setter s = new Setter();
                                    s.Property = System.Windows.Shapes.Path.FillProperty;
                                    s.Value = CreateStyle();
                                    CPS.Setters.Add(s);
                                    Setter s1 = new Setter();
                                    s1.Property = System.Windows.Shapes.Path.StrokeProperty;
                                    s1.Value = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                                    CPS.Setters.Add(s1);
                                    Setter s2 = new Setter();
                                    s2.Property = System.Windows.Shapes.Path.StrokeThicknessProperty;
                                    s2.Value = 1d;
                                    CPS.Setters.Add(s2);
                                    n.CustomPathStyle = CPS;
                                    tempath.Style = (n as Node).CustomPathStyle;

                                    RectangleGeometry rgt = new RectangleGeometry();
                                    rt.X = oldx;
                                    rt.Y = oldy;
                                    rt.Height = 0;
                                    rt.Width = 0;
                                    rgt.RadiusX = 0;
                                    rgt.RadiusY = 0;
                                    rgt.Rect = rt;
                                    tempath.Data = rgt;

                                }
                                drawcanvas.Children.Add(tempath);
                                if (this.view.Page.Children.IndexOf(drawcanvas) < 0)
                                    this.view.Page.Children.Add(drawcanvas);
                            }
                            else if (this.view.DrawingTool == DrawingTools.Polygon)
                            {

                                if (tempath == null)
                                {
                                    tempath = new System.Windows.Shapes.Path();
                                    tempath.Stretch = Stretch.None;
                                    Style CPS = new Style();
                                    CPS.BasedOn = n.CustomPathStyle;
                                    CPS.TargetType = typeof(System.Windows.Shapes.Path);
                                    Setter s = new Setter();
                                    s.Property = System.Windows.Shapes.Path.FillProperty;
                                    s.Value = CreateStyle();
                                    CPS.Setters.Add(s);
                                    Setter s1 = new Setter();
                                    s1.Property = System.Windows.Shapes.Path.StrokeProperty;
                                    s1.Value = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                                    CPS.Setters.Add(s1);
                                    Setter s2 = new Setter();
                                    s2.Property = System.Windows.Shapes.Path.StrokeThicknessProperty;
                                    s2.Value = 1d;
                                    CPS.Setters.Add(s2);
                                    n.CustomPathStyle = CPS;
                                    tempath.Style = (n as Node).CustomPathStyle;

                                    tempbezier = new PathGeometry();
                                    pf = new PathFigure();
                                    pf.StartPoint = new Point(oldx, oldy);
                                    temppoly = new PolyLineSegment();
                                    PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
                                    pf.Segments = myPathSegmentCollection;
                                    pf.Segments.Add(temppoly);
                                    tempbezier.Figures.Add(pf);
                                    tempath.Data = tempbezier;

                                }
                                temppoly.Points.Add(e.GetPosition(this.view.Page));
                                pf.IsClosed = true;
                                pf.IsFilled = true;

                                if (!drawcanvas.Children.Contains(tempath))
                                {
                                    drawcanvas.Children.Add(tempath);
                                }
                                if (this.view.Page.Children.IndexOf(drawcanvas) < 0)

                                    this.view.Page.Children.Add(drawcanvas);
                            }


                        }
                    }
                }
            }
            #endregion
        }

        private LinearGradientBrush CreateStyle()
        {
            LinearGradientBrush lgb = new LinearGradientBrush();
            lgb.StartPoint = new Point(0, 0);
            lgb.EndPoint = new Point(0, 1);
            GradientStop blueGS = new GradientStop();
            blueGS.Color = Color.FromArgb(255, 250, 251, 233);
            blueGS.Offset = 0;
            lgb.GradientStops.Add(blueGS);

            GradientStop blueGS1 = new GradientStop();
            blueGS1.Color = Color.FromArgb(255, 116, 160, 237);
            blueGS1.Offset = 1;
            lgb.GradientStops.Add(blueGS1);
            return lgb;
        }

        #region CloneObj
        private bool IsPresentationFrameworkCollection(Type type)
        {
            if (type == typeof(object))
            {
                return false;
            }

            if (type.Name.StartsWith("PresentationFrameworkCollection"))
            {
                return true;
            }

            return this.IsPresentationFrameworkCollection(type.BaseType);
        }

        internal object Clone(object obj, int isPath)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            object cloneObj = obj.GetType().GetConstructors()[0].Invoke(null);
            if (cloneObj is Node || cloneObj is Group)
            {
                if (cloneObj is Node)
                {
                    cloneObj = obj as Node;
                    DiagramPageXamlWriter xamlWriter = new DiagramPageXamlWriter();
                    string xamlstring = xamlWriter.WriteXaml(cloneObj);
                    cloneObj = XamlReader.Load(xamlstring) as object;
                }
            }
            else
            {
                foreach (PropertyInfo property in properties)
                {
                    if (!property.Name.Contains("Name"))
                    {
                        if (obj is TextBox)
                        {
                            if (property.Name.Contains("InputScope") || property.Name.Contains("Watermark"))
                            {
                                continue;
                            }
                        }
                        object value = property.GetValue(obj, null);
                        if (value != null)
                        {
                            try
                            {
                                if (IsPresentationFrameworkCollection(value.GetType()))
                                {
                                    object collection = property.GetValue(obj, null);
                                    int count = (int)collection.GetType().GetProperty("Count").GetValue(collection, null);
                                    for (int i = 0; i < count; i++)
                                    {
                                        object child = collection.GetType().GetProperty("Item").GetValue(collection, new object[] { i });
                                        object cloneChild = this.Clone(child, 0);
                                        object cloneCollection = property.GetValue(cloneObj, null);
                                        collection.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, cloneCollection, new object[] { cloneChild });
                                    }
                                }

                                if (value is UIElement)
                                {
                                    object obj2 = property.PropertyType.GetConstructors()[0].Invoke(null);
                                    Clone(obj2, 0);
                                    property.SetValue(cloneObj, obj2, null);
                                }

                                else if (property.CanWrite)
                                {
                                    if (property.ToString().Contains("Data") && isPath > 0)
                                    {
                                        PathGeometry geo = value as PathGeometry;
                                        PathGeometry pathgeo = (PathGeometry)this.Clone(geo, 0);
                                        property.SetValue(cloneObj, pathgeo, null);
                                    }
                                    else
                                    {
                                        property.SetValue(cloneObj, value, null);
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            return cloneObj;
        }

        private void SetUIElementProperties(FrameworkElement element)
        {
            if (element is Node)
            {
                if (double.IsNaN(element.Width) || element.Width <= 0)
                {
                    element.Width = 50;
                }
                if (double.IsNaN(element.Height) || element.Height <= 0)
                {
                    element.Height = 50;
                }
            }

        }

        private void RefreshConnection(List<LineConnector> listofline, int increasedcount)
        {
            foreach (LineConnector line in listofline)
            {
                int h1 = line.HeadNodeReferenceNo;
                int l1 = line.TailNodeReferenceNo;
                var connection = from Node nod in dc.Model.Nodes where nod.ReferenceNo == h1 || nod.ReferenceNo == l1 select nod;
                foreach (Node node in connection.ToList())
                {
                    if (line.HeadNodeReferenceNo == node.ReferenceNo && !(node is Group))
                    {
                        line.HeadNode = node;
                    }
                    else if (line.TailNodeReferenceNo == node.ReferenceNo && !(node is Group))
                    {
                        line.TailNode = node;

                    }
                }
            }
        }

        List<IShape> dropbox = new List<IShape>();

        private List<Point> nodedropRect(List<IShape> _dropbox, Point _dropPosition)
        {
            List<Point> dropbox = new List<Point>();
            double offx = _dropbox.Min(p => p.OffsetX);
            double offy = _dropbox.Min(p => p.OffsetY);
            double offx1 = (_dropbox.Max(p => p.OffsetX));
            double offy1 = _dropbox.Max(p => p.OffsetY);
            dropbox.Add(new Point(offx, offy));
            dropbox.Add(new Point(Math.Abs(offx1 - offx) / 2, Math.Abs(offy1 - offy) / 2));
            return dropbox;
        }

        private void SetPosition(List<object> ContentCollection, List<Point> correctposition, Point currentposition)
        {
            foreach (object obj in ContentCollection)
            {
                if (obj is IShape && correctposition.Count > 0&&!(obj is Group))
                {
                    (obj as IShape).OffsetX = (obj as IShape).OffsetX - correctposition[0].X + currentposition.X - correctposition[1].X;
                    (obj as IShape).OffsetY = (obj as IShape).OffsetY - correctposition[0].Y + currentposition.Y - correctposition[1].Y;
                }
            }
        }
        #endregion

        Group _dropgrp;
        void ScrollableGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (view.IsPanEnabled)
            {
                if (HorizontalOffset > 0 && VerticalOffset > 0)
                {

                }
                else
                {
                    // PanningPage(new Point(0, 0));
                }
                view.Page.InvalidateMeasure();
                if (view.VerRuler != null)
                {
                    view.VerRuler.OffsetY = -(VerticalOffset);
                    view.VerRuler.PxStartValue = -page.Top;
                    view.VerRuler.Margin = new Thickness(0, 0, (-VerticalOffset), 0);
                }
                if (HorizontalOffset == 0)
                {
                    if (ExtentWidth - ViewportWidth == 0)
                    {
                        VerifyScrollVirtual();
                    }

                }
                if (view.HorRuler != null)
                {
                    view.HorRuler.OffsetX = -(-page.Left + HorizontalOffset);
                    view.HorRuler.PxStartValue = -page.Left;
                    view.HorRuler.Margin = new Thickness((-HorizontalOffset), 0, 0, 0);

                }
            }
            this.ReleaseMouseCapture();
            if ((this.view.Page as DiagramPage).startPoint.HasValue)
            {
                (this.view.Page as DiagramPage).startPoint = null;
            }

            if (this.view.SelectionCanvas.Children.Count > 0)
            {
                this.view.SelectionCanvas.Children.Remove(this.view.SelectionCanvas.Children.ElementAt(this.view.SelectionCanvas.Children.Count() - 1));
            }
            CollectionExt.Cleared = false;
            if (this.dc.View.IsPageEditable)
            {
                if (SymbolPaletteItem.Hasvalue && !view.IsPanEnabled)
                {
                    List<LineConnector> listofline = new List<LineConnector>();
                    if (view.BoundaryConstraintsEnabled && !DropAvailable(e.GetPosition(view.Page)))
                    {
                        SymbolPaletteItem.Hasvalue = false;
                    }
                    else
                    {
                        Node newItem = new Node();
                        SymbolPaletteItem.Hasvalue = false;
                        if (DiagramPage.o != null)
                        {
                            object content = DiagramPage.o;
                            if (content != null)
                            {
                                if (content is LineConnector)
                                {
                                    LineConnector line=this.Clone(content,1) as LineConnector;
                                    listofline.Add(line);
                                    (this.view.Page as DiagramPage).DropLine(line, e.GetPosition(this.view.Page), this.dc);
                                }
                                else if (content is System.Windows.Shapes.Path && (content as System.Windows.Shapes.Path).Tag.ToString().Contains("Orthogonal"))
                                {
                                    (this.view.Page as DiagramPage).DropLine(ConnectorType.Orthogonal, e.GetPosition(this.view.Page), this.dc);
                                }
                                else if (content is System.Windows.Shapes.Path && (content as System.Windows.Shapes.Path).Tag.ToString().Contains("Straight"))
                                {
                                    (this.view.Page as DiagramPage).DropLine(ConnectorType.Straight, e.GetPosition(this.view.Page), this.dc);
                                }
                                else if (content is System.Windows.Shapes.Path && (content as System.Windows.Shapes.Path).Tag.ToString().Contains("Bezier"))
                                {
                                    (this.view.Page as DiagramPage).DropLine(ConnectorType.Bezier, e.GetPosition(this.view.Page), this.dc);
                                }
                                else if (content is System.Windows.Shapes.Path && (content as System.Windows.Shapes.Path).Tag.ToString().Contains("Arc"))
                                {
                                    (this.view.Page as DiagramPage).DropLine(ConnectorType.Arc, e.GetPosition(this.view.Page), this.dc);
                                }
                                else
                                {
                                    PreviewNodeDropEventRoutedEventArgs newEventArgs1 = new PreviewNodeDropEventRoutedEventArgs();
                                    view.OnPreviewNodeDrop(newItem, newEventArgs1);
                                    if (newEventArgs1.Cancel == false)
                                    {
                                        if (content is Node)
                                        {
                                            newItem = new Node();
                                            if (content is Group)
                                            {
                                                _dropgrp = content as Group;
                                                newItem = content as Group;
                                                newItem.Content = (content as Group).Content as object;
                                                content = (content as Group).Content;
                                            }
                                            else
                                            {
                                                newItem = content as Node;
                                                newItem.Content = (content as Node).Content as object;
                                                content = (content as Node).Content;
                                            }

                                        }
                                        else if (newEventArgs1.Node != null)
                                        {
                                            newItem = newEventArgs1.Node as Node;
                                        }
                                        else
                                        {
                                            newItem = new Node();
                                        }
                                        string name = string.Empty;
                                        if (content is System.Windows.Shapes.Path)
                                        {
                                            var uri = new Uri("/Syncfusion.Diagram.Silverlight;component/Themes/NodeShapes.xaml", UriKind.Relative);
                                            var streamResourceInfo = Application.GetResourceStream(uri);
                                            string rsxaml = null;
                                            using (var resourceStream = streamResourceInfo.Stream)
                                            {
                                                using (var streamReader = new System.IO.StreamReader(resourceStream))
                                                {
                                                    rsxaml = streamReader.ReadToEnd();
                                                }
                                            }
                                            (this.view.Page as DiagramPage).symbols = XamlReader.Load(rsxaml) as ResourceDictionary;
                                            newItem.NodeShape = new System.Windows.Shapes.Path();
                                            System.Windows.Shapes.Path p = content as System.Windows.Shapes.Path;
                                            newItem.NodeShape.IsHitTestVisible = false;
                                            Shapes sp = Shapes.CustomPath;
                                            if (!Enum.TryParse<Shapes>((p as FrameworkElement).Tag.ToString().Replace("PART_", ""), out sp))
                                            {
                                                sp = Shapes.CustomPath;
                                            }
                                            newItem.NodeShape.Style = (this.view.Page as DiagramPage).symbols[(p as FrameworkElement).Tag.ToString()] as Style;
                                            if (newItem.NodeShape.Data == null && DiagramPage.Pathstring != string.Empty)
                                            {
                                                string pathXaml = "<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Data=\"" + DiagramPage.Pathstring + "\"/>";
                                                newItem.NodeShape = (System.Windows.Shapes.Path)System.Windows.Markup.XamlReader.Load(pathXaml);
                                            }
                                            //newItem.Shape = sp;
                                            newItem.MeasurementUnits = (this.view.Page as DiagramPage).MeasurementUnits;
                                            newItem.NodePathFill = p.Fill;
                                            newItem.NodeShape.Stretch = p.Stretch;
                                            newItem.NodePathStroke = p.Stroke;
                                            newItem.Shape = sp;
                                        }
                                        else
                                        {
                                            if (content is UIElement)
                                            {
                                                if (content is Panel)
                                                {
                                                    Panel panel = content as Panel;
                                                    for (int i = 0; i < (content as Panel).Children.Count; i++)
                                                    {
                                                        if ((content as Panel).Children[i] is System.Windows.Shapes.Path)
                                                        {
                                                            System.Windows.Shapes.Path ele = (content as Panel).Children[i] as System.Windows.Shapes.Path;
                                                            System.Windows.Shapes.Path p = ele;
                                                            string pathXaml = "<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Data=\"" + ele.Tag + "\"/>";
                                                            ele = (System.Windows.Shapes.Path)System.Windows.Markup.XamlReader.Load(pathXaml);
                                                            ele.Fill = p.Fill;
                                                            ele.Stroke = p.Stroke;
                                                            ele.Style = p.Style;
                                                            ele.Height = p.Height;
                                                            ele.Width = p.Width;
                                                            ele.Stretch = p.Stretch;
                                                            (content as Panel).Children[i] = ele;
                                                        }

                                                    }
                                                }
                                                if (dc.SymbolPalette.SelectedItem != null)
                                                {
                                                    name = dc.SymbolPalette.SelectedItem.Name;
                                                }
                                                (content as UIElement).IsHitTestVisible = true;
                                                if (!(content is Node))
                                                {
                                                    newItem.Content = content;
                                                }
                                                if (newItem != null && newItem.Content is FrameworkElement)
                                                {
                                                    (newItem.Content as FrameworkElement).Name = (content as FrameworkElement).Name + "n";
                                                }
                                            }
                                            else
                                            {
                                                newItem.Content = content;
                                            }
                                        }
                                        Point position = e.GetPosition((this.view.Page as DiagramPage));
                                        NodeDroppedRoutedEventArgs newEventArgs = new NodeDroppedRoutedEventArgs(newItem, name, new Point(position.X - 25, position.Y - 25));
                                        view.OnNodeDropped(newItem, newEventArgs);
                                        this.view.SelectionList.Clear();
                                        this.view.SelectionList.Add(newItem);
                                        newItem.IsHitTestVisible = true;
                                        newItem.Focus();
                                        newItem.PxOffsetX = position.X - 25;// MeasureUnitsConverter.FromPixels(Math.Max(0, position.X), this.MeasurementUnits);
                                        newItem.PxOffsetY = position.Y - 25;// MeasureUnitsConverter.FromPixels(Math.Max(0, position.Y), this.MeasurementUnits);
                                        SetUIElementProperties(newItem);
                                        if ((this.view.Page as DiagramPage).childcount)
                                        {
                                            (this.view.Page as DiagramPage).no = this.Children.Count + 1;
                                            (this.view.Page as DiagramPage).childcount = false;
                                        }
                                        (this.view.Page as DiagramPage).namecount = (this.view.Page as DiagramPage).no++;
                                        string str = "Node" + (this.view.Page as DiagramPage).namecount;
                                        try
                                        {
                                            if (string.IsNullOrEmpty(newItem.Name))
                                            {
                                                if (this.dc.Model.Nodes.Count == 0)
                                                {
                                                    newItem.Name = str;
                                                }

                                                foreach (IShape n in this.dc.Model.Nodes)
                                                {
                                                    if (!(n is LineConnector))
                                                    {
                                                        if (n != null && (n as Node).Name != str)
                                                        {
                                                            newItem.Name = str;
                                                        }
                                                        else
                                                        {
                                                            newItem.Name = "new" + str;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                        }
                                        if (newItem.Content is FrameworkElement)
                                        {                                 
                                            (newItem.Content as FrameworkElement).Name = newItem.Name + "content";
                                        }
                                        newItem.Page = this;
                                        dc.View.nodragging = true;
                                        if (newItem.Content != null && newItem.Content is UIElement)
                                        {
                                            (newItem.Content as UIElement).IsHitTestVisible = true;
                                        }
                                        this.dc.Model.Nodes.Add(newItem);
                                        dc.View.Page.InvalidateMeasure();
                                        dc.View.ScrollGrid.InvalidateMeasure();
                                        newItem.Measure(new Size(newItem.Width, newItem.Height));
                                        newItem.Arrange(new Rect(newItem.OffsetX, newItem.OffsetY, newItem.Width, newItem.Height));
                                        (newItem as Node).UpdateLayout();
                                    }
                                }
                            }
                        }

                        if (DiagramPage._groupcollection != null && DiagramPage._groupcollection.Count > 0)
                        {
                            List<object> ContentCollection = new List<object>();                           
                            foreach (UIElement Ele in DiagramPage._groupcollection)
                            {

                                object content = null;
                                if (Ele is Node)
                                {
                                    newItem = new Node();
                                    newItem = this.Clone(Ele, 0) as Node;
                                }
                                else if (Ele is LineConnector)
                                {
                                    ContentCollection.Add(Ele);
                                    LineConnector line = this.Clone(Ele, 1) as LineConnector;
                                    listofline.Add(line);
                                    (this.view.Page as DiagramPage).DropLine(line, e.GetPosition(this.view.Page), this.dc);
                                    continue;
                                }
                                else
                                {
                                    content = Ele;
                                }
                                if (content != null||Ele!=null)
                                {
                                    if (content is System.Windows.Shapes.Path && (content as System.Windows.Shapes.Path).Tag.ToString().Contains("Orthogonal"))
                                    {
                                        (this.view.Page as DiagramPage).DropLine(ConnectorType.Orthogonal, e.GetPosition(this.view.Page), this.dc);
                                    }
                                    else if (content is System.Windows.Shapes.Path && (content as System.Windows.Shapes.Path).Tag.ToString().Contains("Straight"))
                                    {
                                        (this.view.Page as DiagramPage).DropLine(ConnectorType.Straight, e.GetPosition(this.view.Page), this.dc);
                                    }
                                    else if (content is System.Windows.Shapes.Path && (content as System.Windows.Shapes.Path).Tag.ToString().Contains("Bezier"))
                                    {
                                        (this.view.Page as DiagramPage).DropLine(ConnectorType.Bezier, e.GetPosition(this.view.Page), this.dc);
                                    }
                                    else if (content is System.Windows.Shapes.Path && (content as System.Windows.Shapes.Path).Tag.ToString().Contains("Arc"))
                                    {
                                        (this.view.Page as DiagramPage).DropLine(ConnectorType.Arc, e.GetPosition(this.view.Page), this.dc);
                                    }
                                    else
                                    {
                                        PreviewNodeDropEventRoutedEventArgs newEventArgs1 = new PreviewNodeDropEventRoutedEventArgs();
                                        view.OnPreviewNodeDrop(newItem, newEventArgs1);
                                        if (newEventArgs1.Cancel == false)
                                        {
                                            if (newEventArgs1.Node != null)
                                            {
                                                newItem = newEventArgs1.Node as Node;
                                            }
                                            else if (!(Ele is Node))
                                            {
                                                newItem = new Node();
                                            }
                                            string name = string.Empty;
                                            if (content is System.Windows.Shapes.Path)
                                            {
                                                var uri = new Uri("/Syncfusion.Diagram.Silverlight;component/Themes/NodeShapes.xaml", UriKind.Relative);
                                                var streamResourceInfo = Application.GetResourceStream(uri);
                                                string rsxaml = null;
                                                using (var resourceStream = streamResourceInfo.Stream)
                                                {
                                                    using (var streamReader = new System.IO.StreamReader(resourceStream))
                                                    {
                                                        rsxaml = streamReader.ReadToEnd();
                                                    }
                                                }

                                                (this.view.Page as DiagramPage).symbols = XamlReader.Load(rsxaml) as ResourceDictionary;
                                                newItem.NodeShape = new System.Windows.Shapes.Path();
                                                System.Windows.Shapes.Path p = content as System.Windows.Shapes.Path;
                                                newItem.NodeShape.IsHitTestVisible = false;
                                                Shapes sp = Shapes.CustomPath;
                                                if (!Enum.TryParse<Shapes>((p as FrameworkElement).Tag.ToString().Replace("PART_", ""), out sp))
                                                {
                                                    sp = Shapes.CustomPath;
                                                }
                                                newItem.NodeShape.Style = (this.view.Page as DiagramPage).symbols[(p as FrameworkElement).Tag.ToString()] as Style;
                                                if (newItem.NodeShape.Data == null && DiagramPage.Pathstring != string.Empty)
                                                {
                                                    string pathXaml = "<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Data=\"" + DiagramPage.Pathstring + "\"/>";
                                                    newItem.NodeShape = (System.Windows.Shapes.Path)System.Windows.Markup.XamlReader.Load(pathXaml);
                                                }
                                                //newItem.Shape = sp;
                                                newItem.MeasurementUnits = (this.view.Page as DiagramPage).MeasurementUnits;
                                                newItem.NodePathFill = p.Fill;
                                                newItem.NodeShape.Stretch = p.Stretch;
                                                newItem.NodePathStroke = p.Stroke;
                                                newItem.Shape = sp;
                                            }
                                            else
                                            {
                                                if (content is UIElement)
                                                {
                                                    if (content is Panel)
                                                    {
                                                        Panel panel = content as Panel;
                                                        for (int i = 0; i < (content as Panel).Children.Count; i++)
                                                        {
                                                            if ((content as Panel).Children[i] is System.Windows.Shapes.Path)
                                                            {
                                                                System.Windows.Shapes.Path ele = (content as Panel).Children[i] as System.Windows.Shapes.Path;
                                                                System.Windows.Shapes.Path p = ele;
                                                                string pathXaml = "<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Data=\"" + ele.Tag + "\"/>";
                                                                ele = (System.Windows.Shapes.Path)System.Windows.Markup.XamlReader.Load(pathXaml);
                                                                ele.Fill = p.Fill;
                                                                ele.Stroke = p.Stroke;
                                                                ele.Style = p.Style;
                                                                ele.Height = p.Height;
                                                                ele.Width = p.Width;
                                                                ele.Stretch = p.Stretch;
                                                                (content as Panel).Children[i] = ele;
                                                            }

                                                        }
                                                    }
                                                    name = dc.SymbolPalette.SelectedItem.Name;
                                                    (content as UIElement).IsHitTestVisible = true;
                                                    newItem.Content = content;
                                                    (newItem.Content as FrameworkElement).Name = (content as FrameworkElement).Name + "n";
                                                }
                                                else
                                                {
                                                    newItem.Content = content;
                                                }
                                            }
                                            Point position = e.GetPosition((this.view.Page as DiagramPage));
                                            NodeDroppedRoutedEventArgs newEventArgs = new NodeDroppedRoutedEventArgs(newItem, name, new Point(position.X - 25, position.Y - 25));
                                            view.OnNodeDropped(newItem, newEventArgs);
                                            //this.view.SelectionList.Clear();
                                            //this.view.SelectionList.Add(newItem);
                                            newItem.Focus();
                                            //newItem.PxOffsetX = position.X - 25;// MeasureUnitsConverter.FromPixels(Math.Max(0, position.X), this.MeasurementUnits);
                                            //newItem.PxOffsetY = position.Y - 25;// MeasureUnitsConverter.FromPixels(Math.Max(0, position.Y), this.MeasurementUnits);
                                            SetUIElementProperties(newItem);
                                            if ((this.view.Page as DiagramPage).childcount)
                                            {
                                                (this.view.Page as DiagramPage).no = this.Children.Count + 1;
                                                (this.view.Page as DiagramPage).childcount = false;
                                            }

                                            (this.view.Page as DiagramPage).namecount = (this.view.Page as DiagramPage).no++;
                                            string str = "Node" + (this.view.Page as DiagramPage).namecount;
                                            try
                                            {
                                                if (string.IsNullOrEmpty(newItem.Name))
                                                {
                                                    if (this.dc.Model.Nodes.Count == 0)
                                                    {
                                                        newItem.Name = str;
                                                    }

                                                    foreach (IShape n in this.dc.Model.Nodes)
                                                    {
                                                        if (!(n is LineConnector))
                                                        {
                                                            if (n != null && (n as Node).Name != str)
                                                            {
                                                                newItem.Name = str;
                                                            }
                                                            else
                                                            {
                                                                newItem.Name = "new" + str;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                            }

                                            if (newItem.Content is FrameworkElement)
                                            {
                                                (newItem.Content as FrameworkElement).Height = double.NaN;
                                                (newItem.Content as FrameworkElement).Width = double.NaN;
                                                (newItem.Content as FrameworkElement).Name = newItem.Name + "content";
                                            }
                                            newItem.Page = this;
                                            dc.View.nodragging = true;
                                            if(!(newItem is Group))
                                            ContentCollection.Add(newItem);
                                            this.dc.Model.Nodes.Add(newItem);
                                            dc.View.Page.InvalidateMeasure();
                                            dc.View.ScrollGrid.InvalidateMeasure();
                                            newItem.Measure(new Size(newItem.Width, newItem.Height));
                                            newItem.Arrange(new Rect(newItem.OffsetX, newItem.OffsetY, newItem.Width, newItem.Height));
                                            (newItem as Node).UpdateLayout();
                                        }
                                    }
                                }
                            }
                           
                            List<Point> correctposition = new List<Point>();
                            if (ContentCollection.Count > 0)
                            {
                                correctposition = nodedropRect(ContentCollection.OfType<IShape>().ToList(), e.GetPosition((this.view.Page as DiagramPage)));
                                dropbox.Clear();
                            }
                            SetPosition(ContentCollection, correctposition, e.GetPosition((this.view.Page as DiagramPage)));
                            if (ContentCollection.Count > 1)
                            {

                                if (_dropgrp != null)
                                {
                                    view.SelectionList.Clear();
                                    GroupDroppedRoutedEventArgs groupdropevt = new GroupDroppedRoutedEventArgs(_dropgrp);
                                    view.onGroupDropped(_dropgrp as Node, groupdropevt);
                                    foreach (ICommon com in ContentCollection)
                                    {
                                        if (com is Node)
                                        {
                                            _dropgrp.AddChild(com as Node);
                                        }
                                        if (com is LineConnector)
                                        {
                                            _dropgrp.AddChild(com as LineConnector);
                                        }
                                    }
                                    view.SelectionList.Add(_dropgrp);
                                    _dropgrp.IsHitTestVisible = true;
                                }
                                else
                                {
                                    view.SelectionList.Clear();
                                    foreach (ICommon com in ContentCollection)
                                    {

                                        view.SelectionList.Add(com);
                                    }
                                    dc.Group.Execute(this.dc.View);
                                    GroupDroppedRoutedEventArgs groupdropevt = new GroupDroppedRoutedEventArgs(dc.Model.Nodes[dc.Model.Nodes.Count - 1] as Group);
                                    view.onGroupDropped(dc.Model.Nodes[dc.Model.Nodes.Count - 1] as Group, groupdropevt);
                                }
                                view.SelectionList.Clear();
                            }
                            RefreshConnection(listofline, 3);
                        }
                        // e.Handled = true;
                    }
                    DiagramPage.o = null;
                    DiagramPage.symboll = null;
                    if (DiagramPage._groupcollection != null)
                    {
                        DiagramPage._groupcollection.Clear();
                    }
                    //e.Handled = true;
                }
                #region Darwing Tools


                if ((this.view as DiagramView).EnableDrawingTools == true)
                {
                    //(this.view as DiagramView).AllowDrawandDrag = true;
                    this.IsExtent = true;
                    double newx2 = e.GetPosition(this.view.Page as DiagramPage).X;
                    double newy2 = e.GetPosition(this.view.Page as DiagramPage).Y;
                    DrawingTools draw = (this.view as DiagramView).DrawingTool;
                    if ((this.view as DiagramView).SizeToContent == false)
                    {
                        if ((this.view as DiagramView).BoundaryConstraintsEnabled == true)
                        {
                            if (!((this.view as DiagramView).BoundaryConstraintsArea.Contains(new Point(newx2, newy2))))
                            {
                                (this.view as DiagramView).EnableDrawingTools = true;
                                this.DrawAllow = true;
                                (this.view as DiagramView).DrawingTool = draw;
                            }
                        }
                    }

                    if (tempath != null)
                    {

                        if (this.view.DrawingTool == DrawingTools.StraightLine)
                        {
                                ConnectorBase line = null;
                                line = new LineConnector();
                                line.ConnectorType = ConnectorType.Straight;
                                line.m_LineDrawing = true;
                                line.PxStartPointPosition = (tempath.Data as LineGeometry).StartPoint;
                                line.PxEndPointPosition = (tempath.Data as LineGeometry).EndPoint;
                                line.UpdateConnectorPathGeometry();
                                (view.Page as DiagramPage).Children.Remove(tempath);
                                drawcanvas.Children.Remove(tempath);
                                (view.Page as DiagramPage).Children.Remove(drawcanvas);
                                dc.Model.Connections.Add(line);
                                view.SelectionList.Clear();
                                view.SelectionList.Add(line);
                                line.Focus();
                                tempath = null;
                                e.Handled = true;
                                if (this.view.DrawingMode == DrawingMode.Default)
                                this.view.EnableDrawingTools = false;
                                timeRightClick = DateTime.Now;
     
                            
                        }
                        else if (this.view.DrawingTool == DrawingTools.OrthogonalLine)
                        {

                            ConnectorBase line = null;
                            line = new LineConnector();
                            line.ConnectorType = ConnectorType.Orthogonal;
                            line.m_LineDrawing = true;
                            line.PxStartPointPosition = pf.StartPoint;
                            line.PxEndPointPosition = temppoly.Points[temppoly.Points.Count - 1];
                            this.view.Page.Children.Remove(tempath);
                            drawcanvas.Children.Remove(tempath);
                            (view.Page as DiagramPage).Children.Remove(drawcanvas);
                            tempath = null;
                            line.UpdateConnectorPathGeometry();
                            dc.Model.Connections.Add(line);
                            view.SelectionList.Clear();
                            view.SelectionList.Add(line);
                            e.Handled = true;
                            line.Focus();
                            if (this.view.DrawingMode == DrawingMode.Default)
                            this.view.EnableDrawingTools = false;
                            timeRightClick = DateTime.Now;
                        }
                        else if (this.view.DrawingTool == DrawingTools.BezierLine)
                        {

                            ConnectorBase line = null;
                            line = new LineConnector();
                            line.ConnectorType = ConnectorType.Bezier;
                            line.m_LineDrawing = true;
                            line.PxStartPointPosition = pf.StartPoint;
                            line.PxEndPointPosition = point3;
                            // Drawtools_Remove(temppth);
                            this.view.Page.Children.Remove(tempath);
                            drawcanvas.Children.Remove(tempath);
                            (view.Page as DiagramPage).Children.Remove(drawcanvas);
                            tempath = null;
                            line.UpdateConnectorPathGeometry();
                            dc.Model.Connections.Add(line);
                            view.SelectionList.Clear();
                            view.SelectionList.Add(line);
                            e.Handled = true;
                            line.Focus();
                            if (this.view.DrawingMode == DrawingMode.Default)
                            this.view.EnableDrawingTools = false;
                            timeRightClick = DateTime.Now;
                        }
                        else if (this.view.DrawingTool == DrawingTools.Ellipse)
                        {

                            System.Windows.Shapes.Path draw_ell = new System.Windows.Shapes.Path();
                            draw_ell.Data = new EllipseGeometry();
                            (draw_ell.Data as EllipseGeometry).RadiusX = (tempath.Data as EllipseGeometry).RadiusX;
                            (draw_ell.Data as EllipseGeometry).RadiusY = (tempath.Data as EllipseGeometry).RadiusY;
                            double offx = (tempath.Data as EllipseGeometry).Bounds.X;
                            double offy = (tempath.Data as EllipseGeometry).Bounds.Y;
                            draw_ell.Stretch = Stretch.Fill;
                            Node n1 = null;
                            if ((draw_ell.Data as EllipseGeometry).RadiusX != 0 && (draw_ell.Data as EllipseGeometry).RadiusY != 0)
                            {
                                n1 = new Node();
                                n1.OffsetX = offx;
                                n1.OffsetY = offy;
                                n1.Width = ((draw_ell.Data as EllipseGeometry).RadiusX) * 2;
                                n1.Height = ((draw_ell.Data as EllipseGeometry).RadiusY) * 2;
                                //n1.NodePathStroke = new SolidColorBrush(Colors.Blue);
                                n1.Shape = Shapes.CustomPath;
                                //n1.NodeShape = tempath as System.Windows.Shapes.Path;
                                n1.NodeShape = drawcanvas.Children[0] as System.Windows.Shapes.Path;
                                n1.NodeShape.Stretch = Stretch.Fill;
                                n1.m_NodeDrawing = true;
                                this.view.Page.Children.Remove(tempath);
                                drawcanvas.Children.Remove(tempath);
                                (view.Page as DiagramPage).Children.Remove(drawcanvas);
                                dc.Model.Nodes.Add(n1);
                                view.SelectionList.Clear();
                                view.SelectionList.Add(n1);
                                tempath.IsHitTestVisible = false;
                                n1.Focus();
                                if (this.view.DrawingMode == DrawingMode.Default)
                                this.view.EnableDrawingTools = false;
                            }
                            else
                            {
                                drawcanvas.Children.Remove(tempath);
                                (view.Page as DiagramPage).Children.Remove(drawcanvas);
                            }
                            tempath = null;
                            e.Handled = true;
                            timeRightClick = DateTime.Now;
                        }
                        else if (this.view.DrawingTool == DrawingTools.Rectangle)
                        {
                            draw_ell = new System.Windows.Shapes.Path();
                            draw_ell.Data = new RectangleGeometry();
                            Rect rt = new Rect();
                            rt.Width = (tempath.Data as RectangleGeometry).Rect.Width;
                            rt.Height = (tempath.Data as RectangleGeometry).Rect.Height;
                            draw_ell.Stretch = Stretch.Fill;
                            Node n1 = null;
                            if ((tempath.Data as RectangleGeometry).Rect.Width != 0 && (tempath.Data as RectangleGeometry).Rect.Height != 0)
                            {
                                n1 = new Node();
                                n1.OffsetX = (tempath.Data as RectangleGeometry).Bounds.X;
                                n1.OffsetY = (tempath.Data as RectangleGeometry).Bounds.Y;
                                n1.Width = rt.Width;
                                n1.Height = rt.Height;
                                n1.Shape = Shapes.CustomPath;
                                n1.NodeShape = drawcanvas.Children[0] as System.Windows.Shapes.Path;
                                n1.NodeShape.Stretch = Stretch.Fill;
                                n1.m_NodeDrawing = true;
                                this.view.Page.Children.Remove(tempath);
                                drawcanvas.Children.Remove(tempath);
                                (view.Page as DiagramPage).Children.Remove(drawcanvas);
                                dc.Model.Nodes.Add(n1);
                                view.SelectionList.Clear();
                                view.SelectionList.Add(n1);
                                tempath.IsHitTestVisible = false;
                                n1.Focus();
                                if (this.view.DrawingMode == DrawingMode.Default)
                                this.view.EnableDrawingTools = false;
                            }
                            else
                            {
                                drawcanvas.Children.Remove(tempath);
                                (view.Page as DiagramPage).Children.Remove(drawcanvas);
                            }
                            tempath = null;
                            e.Handled = true;
                            timeRightClick = DateTime.Now;

                        }
                        else if (this.view.DrawingTool == DrawingTools.RoundedRectangle)
                        {

                            System.Windows.Shapes.Path draw_ell = new System.Windows.Shapes.Path();
                            draw_ell.Data = new RectangleGeometry();
                            Rect rt = new Rect();
                            rt.Height = (tempath.Data as RectangleGeometry).Rect.Height;
                            rt.Width = (tempath.Data as RectangleGeometry).Rect.Width;
                            draw_ell.Stretch = Stretch.Fill;
                            Node n1 = null;
                            if ((tempath.Data as RectangleGeometry).Rect.Width != 0 && (tempath.Data as RectangleGeometry).Rect.Height != 0)
                            {
                                n1 = new Node();
                                n1.OffsetX = (tempath.Data as RectangleGeometry).Bounds.X;
                                n1.OffsetY = (tempath.Data as RectangleGeometry).Bounds.Y;
                                n1.Width = rt.Width;
                                n1.Height = rt.Height;
                                n1.Shape = Shapes.CustomPath;
                                n1.NodeShape = drawcanvas.Children[0] as System.Windows.Shapes.Path;
                                n1.NodeShape.Stretch = Stretch.Fill;
                                n1.m_NodeDrawing = true;
                                this.view.Page.Children.Remove(tempath);
                                drawcanvas.Children.Remove(tempath);
                                (view.Page as DiagramPage).Children.Remove(drawcanvas);
                                dc.Model.Nodes.Add(n1);
                                view.SelectionList.Clear();
                                view.SelectionList.Add(n1);
                                tempath.IsHitTestVisible = false;
                                n1.Focus();
                                if (this.view.DrawingMode == DrawingMode.Default)
                                this.view.EnableDrawingTools = false;
                            }
                            else
                            {
                                drawcanvas.Children.Remove(tempath);
                                (view.Page as DiagramPage).Children.Remove(drawcanvas);
                            }
                            tempath = null;
                            e.Handled = true;
                            timeRightClick = DateTime.Now;

                        }
                       
                    }
                    (this.view.Page as DiagramPage).InvalidateMeasure();
                    (this.view.Page as DiagramPage).InvalidateArrange();
                }
                #endregion
            }

        }
        public void LineDown()
        {
            if (view != null)
            {
                view.InvalidateViewGrid();
            }
            SetVerticalOffset(VerticalOffset + LineSize);

            if (view != null)
            {
                view.InvalidateViewGrid();
            }
            if (view.VerRuler != null)
            {
                view.VerRuler.OffsetY = -(VerticalOffset);
                view.VerRuler.PxStartValue = -page.Top;
                view.VerRuler.Margin = new Thickness(0, 0, (-VerticalOffset), 0);
            }
            VerifyScrollVirtual();
        }

        public void LineUp()
        {
            if (view != null)
            {
                view.InvalidateViewGrid();
            }
            SetVerticalOffset(VerticalOffset - LineSize);
            if (view != null)
            {
                view.InvalidateViewGrid();
            }
            if (view.VerRuler != null)
            {
                view.VerRuler.OffsetY = -(VerticalOffset);
                view.VerRuler.PxStartValue = -page.Top;
                view.VerRuler.Margin = new Thickness(0, 0, (-VerticalOffset), 0);
            }
            VerifyScrollVirtual();
        }

        public void LineLeft()
        {
            if (view != null)
            {
                view.InvalidateViewGrid();
            }
            SetHorizontalOffset(HorizontalOffset - LineSize);
            if (view.HorRuler != null)
            {
                view.HorRuler.OffsetX = -(-page.Left + HorizontalOffset);
                view.HorRuler.PxStartValue = -page.Left;


                //if (view.HorRuler.OffsetX < 0)
                //{
                view.HorRuler.Margin = new Thickness((-HorizontalOffset), 0, 0, 0);

            }
            VerifyScrollVirtual();
        }

        public void LineRight()
        {
            if (view != null)
            {
                view.InvalidateViewGrid();
            }
            SetHorizontalOffset(HorizontalOffset + LineSize);
            if (view.HorRuler != null)
            {
                view.HorRuler.OffsetX = -(-page.Left + HorizontalOffset);
                view.HorRuler.PxStartValue = -page.Left;


                //if (view.HorRuler.OffsetX < 0)
                //{
                view.HorRuler.Margin = new Thickness((-HorizontalOffset), 0, 0, 0);

            }
            VerifyScrollVirtual();
        }

        public void MouseWheelDown()
        {
            if (view != null)
            {
                view.InvalidateViewGrid();
            }
            SetVerticalOffset(VerticalOffset + WheelSize);
            if (view.VerRuler != null)
            {
                view.VerRuler.OffsetY = -(VerticalOffset);
                view.VerRuler.PxStartValue = -page.Top;
                view.VerRuler.Margin = new Thickness(0, 0, (-VerticalOffset), 0);
            }
            VerifyScrollVirtual();
        }

        public void MouseWheelUp()
        {
            if (view != null)
            {
                view.InvalidateViewGrid();
            }
            SetVerticalOffset(VerticalOffset - WheelSize);
            if (view.VerRuler != null)
            {
                view.VerRuler.OffsetY = -(VerticalOffset);
                view.VerRuler.PxStartValue = -page.Top;
                view.VerRuler.Margin = new Thickness(0, 0, (-VerticalOffset), 0);
            }
            VerifyScrollVirtual();
        }

        public void MouseWheelLeft()
        {
            if (view != null)
            {
                view.InvalidateViewGrid();
            }
            SetHorizontalOffset(HorizontalOffset - WheelSize);
        }

        public void MouseWheelRight()
        {
            if (view != null)
            {
                view.InvalidateViewGrid();
            }
            SetHorizontalOffset(HorizontalOffset + WheelSize);
        }

        public void PageDown()
        {
            if (view != null)
            {
                view.InvalidateViewGrid();
            }
            SetVerticalOffset(VerticalOffset + ViewportHeight);
            if (page.dview.VerRuler != null)
            {
                page.dview.VerRuler.OffsetY = (-page.dview.ScrollGrid.VerticalOffset);
                page.dview.VerRuler.PxStartValue = -page.Top;
            }
        }

        public void PageUp()
        {
            if (view != null)
            {
                view.InvalidateViewGrid();
            }
            SetVerticalOffset(VerticalOffset - ViewportHeight);
            if (page.dview.VerRuler != null)
            {
                //page.dview.VerRuler.OffsetY = (-VerticalOffset);
                page.dview.VerRuler.OffsetY = -(Math.Max(0, page.dview.ScrollGrid.VerticalOffset));
                page.dview.VerRuler.PxStartValue = -page.Top;
                page.dview.VerRuler.Margin = new Thickness(0, 0, -(Math.Max(0, page.dview.ScrollGrid.VerticalOffset)), 0);
                page.dview.UpdateRuler(page.dview);
                page.dview.VerRuler.InvalidateMeasure();
            }

        }

        public void PageLeft()
        {
            if (view != null)
            {
                view.InvalidateViewGrid();
            } SetHorizontalOffset(HorizontalOffset - ViewportWidth);
            if (page.dview.HorRuler != null)
            {
                page.dview.HorRuler.OffsetX = -(-page.Left);
                page.dview.HorRuler.PxStartValue = -page.Left;
                ;
                view.HorRuler.Margin = new Thickness((-HorizontalOffset), 0, 0, 0);
            }
            VerifyScrollVirtual();
        }

        public void PageRight()
        {
            if (view != null)
            {
                view.InvalidateViewGrid();
            } SetHorizontalOffset(HorizontalOffset + ViewportWidth);
            if (page.dview.HorRuler != null)
            {
                page.dview.HorRuler.OffsetX = -(-page.Left + page.dview.ScrollGrid.HorizontalOffset);
                page.dview.HorRuler.PxStartValue = -page.Left;
                page.dview.HorRuler.Margin = new Thickness(-HorizontalOffset, 0, 0, 0);
            }
            VerifyScrollVirtual();
        }
        internal ScaleTransform zoomTransform;
        public ScrollViewer ScrollOwner
        {
            get { return _ScrollOwner; }
            set { _ScrollOwner = value; }
        }

        public bool CanHorizontallyScroll
        {
            get { return _CanHorizontallyScroll; }
            set { _CanHorizontallyScroll = value; }
        }

        public bool CanVerticallyScroll
        {
            get { return _CanVerticallyScroll; }
            set { _CanVerticallyScroll = value; }
        }

        #region ScrollChanged
        internal void ScrollChanged()
        {
            if (evenctcheck)
            {
                FrameworkElement fe = VisualTreeHelper.GetChild(_ScrollOwner, 0) as FrameworkElement;
                if (fe == null)
                    return;
                if (horizontalcheck)
                {
                    ScrollBar Horizontalthumb = fe.FindName("HorizontalScrollBar") as ScrollBar;
                    Horizontalthumb.Scroll += new ScrollEventHandler(Horizontalthumb_Scroll);
                    if (Horizontalthumb != null)
                    {
                        if (VisualTreeHelper.GetChildrenCount(Horizontalthumb) > 0)
                        {
                            Thumb HorThumb = (Thumb)((FrameworkElement)VisualTreeHelper.GetChild(Horizontalthumb, 0)).FindName("HorizontalThumb");
                            if (HorThumb != null)
                            {
                                HorThumb.DragCompleted += new DragCompletedEventHandler(HorThumb_DragCompleted);
                                horizontalcheck = false;
                            }
                        }
                    }
                    //VerifyScrollVirtual();

                }

                if (verticalcheck)
                {
                    ScrollBar VerticalThumb = fe.FindName("VerticalScrollBar") as ScrollBar;
                    VerticalThumb.Scroll += new ScrollEventHandler(VerticalThumb_Scroll);
                    if (VerticalThumb != null)
                    {
                        if (VisualTreeHelper.GetChildrenCount(VerticalThumb) > 0)
                        {
                            Thumb VerThumb = (Thumb)((FrameworkElement)VisualTreeHelper.GetChild(VerticalThumb, 0)).FindName("VerticalThumb");
                            if (VerThumb != null)
                            {
                                // VerThumb.MouseLeftButtonUp += new MouseButtonEventHandler(VerThumb_MouseLeftButtonUp);
                                VerThumb.DragCompleted += new DragCompletedEventHandler(VerThumb_DragCompleted);
                                VerThumb.DragStarted += new DragStartedEventHandler(VerThumb_DragStarted);
                                VerThumb.LostMouseCapture += new MouseEventHandler(VerThumb_LostMouseCapture);
                                VerticalThumb.MouseLeave += new MouseEventHandler(VerticalThumb_MouseLeave);
                                VerThumb.AddHandler(Control.MouseLeftButtonUpEvent, new MouseButtonEventHandler(VerThumb_MouseLeftButtonUp), true);
                                verticalcheck = false;
                            }
                        }
                    }
                    //VerifyScrollVirtual();

                }
                if (!horizontalcheck && !verticalcheck)
                { evenctcheck = false; }

            }
            if (ViewWidth != _ScrollOwner.ViewportWidth)
            {
                //    if (Viewheight != _ScrollOwner.ViewportHeight)
                //    {
                //        if (dc.View.EnableVirtualization)
                //        {
                //            this.callCalculate();
                //        }
                //    }
                //    else
                //    {
                //        if (dc.View.EnableVirtualization)
                //        {
                //            this.callCalculate();
                //        }
                //    }
                //}
                //else if (Viewheight != _ScrollOwner.ViewportHeight)
                //{
                //    if (dc.View.EnableVirtualization)
                //    {
                //        this.callCalculate();
                //    }
                //}
                ViewWidth = _ScrollOwner.ViewportWidth;
                Viewheight = _ScrollOwner.ViewportHeight;
            }
        }
        void VerticalThumb_MouseLeave(object sender, MouseEventArgs e)
        {
            LineUpdate();
        }
        void VerThumb_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (view.HorRuler != null)
            {

                view.HorRuler.OffsetX = -(-page.Left + HorizontalOffset);
                view.HorRuler.PxStartValue = -page.Left;
                view.HorRuler.Margin = new Thickness((-HorizontalOffset), 0, 0, 0);

            }
            this.InvalidateMeasure();
            this.ScrollOwner.InvalidateScrollInfo();
            VerifyScrollVirtual();
        }

        internal bool DropAvailable(Point droppoint)
        {
            if (view != null && view.BoundaryConstraintsEnabled)
            {

                if (droppoint.X > view.BoundaryConstraintsArea.Left && droppoint.X + 50 < view.BoundaryConstraintsArea.Right && droppoint.Y > view.BoundaryConstraintsArea.Top && droppoint.Y + 50 < view.BoundaryConstraintsArea.Bottom)
                {
                    return true;
                }
                else
                { return false; }
            }
            else
            {
                return true;
            }
        }
        internal bool HorScroll = false;
        void Horizontalthumb_Scroll(object sender, ScrollEventArgs e)
        {
            HorScroll = true;
            if (HorizontalOffset == 0)
            {
                if (ExtentWidth - ViewportWidth == 0)
                {
                    //  VerifyScrollVirtual();
                }
                _Offset.X = 0.01;
                _ScrollOwner.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
            if (view.HorRuler != null)
            {
                view.HorRuler.OffsetX = -(-page.Left + HorizontalOffset);
                view.HorRuler.PxStartValue = -page.Left;
                view.HorRuler.Margin = new Thickness((-HorizontalOffset), 0, 0, 0);

            }
            this.ScrollOwner.InvalidateScrollInfo();
            if (view != null && view.EnableVirtualization && view.IsScrollVirtual)
            {
                this.callCalculate();
            }
        }
        #endregion
        private bool Verticlscroll = false;

        void VerticalThumb_Scroll(object sender, ScrollEventArgs e)
        {
            Verticlscroll = true;

            if (ViewportHeight - ExtentHeight == 0)
            {
                if (ExtentWidth - ViewportWidth == 0)
                {
                    VerifyScrollVirtual();
                }
                _Offset.Y = 0.001;
                ScrollOwner.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
            if (view.VerRuler != null)
            {
                view.VerRuler.OffsetY = -(VerticalOffset);
                view.VerRuler.PxStartValue = -page.Top;
                view.VerRuler.Margin = new Thickness(0, 0, (-VerticalOffset), 0);
            }
            if (view != null && view.EnableVirtualization && view.IsScrollVirtual)
            {
                this.callCalculate();

            }
        }
        private void LineUpdate()
        {
            if (!view.IsScrollVirtual)
            {
                foreach (LineConnector line in dc.Model.Connections)
                {
                    line.UpdateConnectorPathGeometry();
                }
            }
        }
        void VerThumb_DragStarted(object sender, DragStartedEventArgs e)
        {

        }

        void VerThumb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        void VerThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {

        }
        void HorThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (view.HorRuler != null)
            {

                view.HorRuler.OffsetX = -(-page.Left + HorizontalOffset);
                view.HorRuler.PxStartValue = -page.Left;
                view.HorRuler.Margin = new Thickness((-HorizontalOffset), 0, 0, 0);

            }
            this.InvalidateMeasure();
            this.ScrollOwner.InvalidateScrollInfo();
            VerifyScrollVirtual();
            LineUpdate();
        }

        public double ExtentHeight
        {
            get
            {
                if (view != null && view.Page != null)
                {
                    if ((view.Page as DiagramPage).Top > 0)
                    {
                        return Math.Max(_Viewport.Height + _Offset.Y, _Extent.Height);
                    }
                } return _Extent.Height;
            }
        }

        public double ExtentWidth
        {
            get
            {
                if (view != null && view.Page != null)
                {
                    if ((view.Page as DiagramPage).Left > 0)
                    {
                        return Math.Max(_Viewport.Width + _Offset.X, _Extent.Width);
                    }
                }
                return _Extent.Width;
            }
        }

        public double HorizontalOffset
        {
            get
            {

                //if (page != null && page.isdragging)
                //{
                //    return page.Left + _Offset.X;
                //}

                return _Offset.X;

            }
        }

        public double VerticalOffset
        {
            get
            {

                return _Offset.Y;
            }
        }

        internal double _VerticalOffset
        {
            get
            {
                if (VerticalOffset < 0)
                {
                    return 0;
                }
                else
                {
                    return VerticalOffset;
                }
            }
        }
        internal double _HorizontalOffset
        {
            get
            {
                if (HorizontalOffset < 0)
                {
                    return 0;
                }
                else
                {
                    return HorizontalOffset;
                }
            }
        }
        public double ViewportHeight
        { get { return _Viewport.Height; } }

        public double ViewportWidth
        { get { return _Viewport.Width; } }

        public Rect MakeVisible(UIElement visual, Rect rectangle)
        {
            if (rectangle.IsEmpty || visual == null
               || visual == this)
            { return Rect.Empty; }
            rectangle = visual.TransformToVisual(this).TransformBounds(rectangle);
            Rect viewRect = new Rect(HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight);
            rectangle.X += viewRect.X;
            rectangle.Y += viewRect.Y;
            viewRect.X = CalculateNewVisibleArea(viewRect.Left, viewRect.Right, rectangle.Left, rectangle.Right);
            viewRect.Y = CalculateNewVisibleArea(viewRect.Top, viewRect.Bottom, rectangle.Top, rectangle.Bottom);
            SetHorizontalOffset(viewRect.X);
            SetVerticalOffset(viewRect.Y);
            rectangle.Intersect(viewRect);
            rectangle.X -= viewRect.X;
            rectangle.Y -= viewRect.Y;
            return rectangle;
        }

        private double CalculateNewVisibleArea(double top1, double bottom1, double top2, double bottom2)
        {
            bool offBottom = top2 < top1 && bottom2 < bottom1;
            bool offTop = bottom2 > bottom1 && top2 > top1;
            bool tooLarge = (bottom2 - top2) > (bottom1 - top1);

            if (!offBottom && !offTop)
            { return top1; }

            if ((offBottom && !tooLarge) || (offTop && tooLarge))
            { return top2; }

            return (bottom2 - (bottom1 - top1));
        }
        public void SetHorizontalOffset(double offset)
        {

            offset = Math.Max(0,
              Math.Min(offset, ExtentWidth - ViewportWidth));
            if (offset != _Offset.X)
            {
                _Offset.X = offset;
                InvalidateArrange();
            }
            this.ScrollChanged();
            //MakeVirtualization();
            //view.HorRuler.PxStartValue = (-page.Left);
        }

        public void SetVerticalOffset(double offset)
        {
            offset = Math.Max(0,
              Math.Min(offset, ExtentHeight - ViewportHeight));
            if (offset != _Offset.Y)
            {
                _Offset.Y = offset;
                InvalidateArrange();
            }
            this.ScrollChanged();
            // MakeVirtualization();

        }
        private bool oncecorrect = true;
        protected void VerifyScrollData(Size viewport, Size extent)
        {
            if (oncecorrect)
            {
                oncecorrect = false;
                Border bor = (Border)VisualTreeHelper.GetChild(_ScrollOwner, 0);
                Grid gri = (Grid)VisualTreeHelper.GetChild(bor, 0);
                if(VisualTreeHelper.GetChild(gri, 0)!=null)
                {
                    FrameworkElement scr = (FrameworkElement)VisualTreeHelper.GetChild(gri, 0);
                    //ScrollContentPresenter scr = (ScrollContentPresenter)VisualTreeHelper.GetChild(gri, 0);
                    scr.Margin = new Thickness(0);
                }
                bor.CornerRadius = new CornerRadius(0);
                bor.BorderThickness = new Thickness(0);
                _ScrollOwner.BorderThickness = new Thickness(0);
            }
            if (double.IsInfinity(viewport.Width))
            {
                viewport.Width = extent.Width;
            }
            if (double.IsInfinity(viewport.Height))
            {
                viewport.Height = extent.Height;
            }
            _Extent = extent;
            _Viewport = viewport;

            _Offset.X = Math.Max(0,
              Math.Min(_Offset.X, ExtentWidth - ViewportWidth));
            _Offset.Y = Math.Max(0,
              Math.Min(_Offset.Y, ExtentHeight - ViewportHeight));
            if (ExtentWidth - ViewportWidth > 1)
            {
                if (_ScrollOwner.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    _ScrollOwner.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
                else
                {
                    _ScrollOwner.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }
            }
            else
            {
                if (_ScrollOwner.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    _ScrollOwner.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                }
                else
                {
                    _ScrollOwner.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }
            }
            if (ExtentHeight - ViewportHeight > 1)
            {
                if (_ScrollOwner.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    _ScrollOwner.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
                else
                {
                    _ScrollOwner.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }
            }
            else
            {
                if (_ScrollOwner.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
                {
                    _ScrollOwner.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                }
                else
                {
                    _ScrollOwner.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }
            }
            if (ScrollOwner != null)
            { ScrollOwner.InvalidateScrollInfo(); }
        }
        Size pagesize = new Size();
        protected override Size MeasureOverride(Size constraint)
        {
            if (Double.IsNaN(constraint.Width) || double.IsInfinity(constraint.Width))
            {
                constraint.Width = this._ScrollOwner.ViewportWidth;
            }
            if (Double.IsNaN(constraint.Height) || double.IsInfinity(constraint.Height))
            {
                constraint.Height = this._ScrollOwner.ViewportHeight;
            }
            if (dc == null)
            {
                dc = DiagramPage.GetDiagramControl(this);
                page = dc.View.Page as DiagramPage;
                view = dc.View;

                if (dc.View != null)
                {
                    dc.View.ScrollGrid = this;
                }
            }
            if (dc != null && !dc.IsLoaded)
            {
                Constraint = constraint;
                if (dc.View.EnableVirtualization)
                {
                    this.callCalculate();
                }
                else
                {
                    this.CallNormailzation();
                }
            }
            foreach (FrameworkElement ele in this.Children)
            {
                ele.Measure(constraint);
                if (ele is ContentPresenter)
                {
                    if (view.SizeToContent == true)
                    {
                        pagesize = page.DesiredSize;
                        if (page.ActualWidth > page.DesiredSize.Width)
                        {
                            pagesize.Width = page.ActualWidth;
                        }
                        if (page.ActualHeight > page.DesiredSize.Height)
                        {
                            pagesize.Height = page.ActualHeight;
                        }
                        ele.Measure(new Size(pagesize.Width + page.Left, pagesize.Height + page.Top));
                        // INeed = (new Size(page.ActualWidth+ page.Left, ele.ActualHeight + page.Top));
                        INeed.Width = page.ActualWidth * view.CurrentZoom + page.Left;
                        INeed.Height = page.ActualHeight * view.CurrentZoom + page.Top;
                    }
                   else
                    {
                        //pagesize = new Size(view.BoundaryConstraintsArea.Right * view.CurrentZoom + view.PageMargin.Left, view.BoundaryConstraintsArea.Bottom * view.CurrentZoom + view.PageMargin.Top);
                        //pagesize = new Size(view.BoundaryConstraintsArea.Right + view.PageMargin.Left, view.BoundaryConstraintsArea.Bottom + view.PageMargin.Top);                           
                        pagesize = page.DesiredSize;
                        if (page.ActualWidth > page.DesiredSize.Width)
                        {
                            pagesize.Width = page.ActualWidth;
                        }
                        if (page.ActualHeight > page.DesiredSize.Height)
                        {
                            pagesize.Height = page.ActualHeight;
                        }
                        ele.Measure(new Size(pagesize.Width + page.Left, pagesize.Height + page.Top));
                        // INeed = (new Size(page.ActualWidth+ page.Left, ele.ActualHeight + page.Top));

                        //INeed.Width = pagesize.Width + page.Left * view.CurrentZoom;
                        //INeed.Height = pagesize.Height + page.Top * view.CurrentZoom;

                        INeed.Width = page.ActualWidth * view.CurrentZoom + page.Left;
                        INeed.Height = page.ActualHeight * view.CurrentZoom + page.Top;
                    }
                }

                if (view.nodragging)
                {
                    if (ele is DiagramViewGrid)
                    {
                        ele.Measure(new Size(_ScrollOwner.ExtentWidth + page.PxGridHorizontalOffset * 2, _ScrollOwner.ExtentHeight + page.PxGridVerticalOffset * 2));
                        view.diagramviewgri = ele as DiagramViewGrid;
                    }
                    if (ele is Rectangle)
                    {
                        ele.Measure(new Size(constraint.Width + page.Left, constraint.Height + page.Top));

                    }
                }
                ButIHave = constraint;
                // INeed = base.MeasureOverride(constraint);
                if (constraint.Width > INeed.Width)
                {
                    INeed.Width = constraint.Width;
                }
                if (constraint.Height > INeed.Height)
                {
                    INeed.Height = constraint.Height;
                }
            }
            VerifyScrollData(ButIHave, INeed);
            return constraint;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Size actualSize = base.ArrangeOverride(arrangeSize);
            foreach (UIElement ele in this.Children)
            {
                if (ele is Rectangle && view != null)
                {
                    if (!view.SizeToContent)
                    {
                        //view.background.Margin = new Thickness(view.BoundaryConstraintsArea.Left * view.CurrentZoom, view.BoundaryConstraintsArea.Top * view.CurrentZoom, 0, 0);
                        ele.Arrange(new Rect((view.Page as DiagramPage).Left - HorizontalOffset, (view.Page as DiagramPage).Top - VerticalOffset, view.BoundaryConstraintsArea.Right * view.CurrentZoom, view.BoundaryConstraintsArea.Bottom * view.CurrentZoom));
                    }
                    else if (view.PageMargin.Left > 0)
                    {
                        if (view.PageMargin.Top > 0)
                        {
                            //view.background.Margin = new Thickness(view.PageMargin.Left * 2 - HorizontalOffset, view.PageMargin.Top * 2 - VerticalOffset, 0, 0);
                        }
                        else
                        {
                            //view.background.Margin = new Thickness(view.PageMargin.Left * 2, 0, 0, 0);
                        }
                    }
                    else if (view.PageMargin.Top > 0)
                    {
                        //view.background.Margin = new Thickness(0, view.PageMargin.Top * 2, 0, 0);
                    }

                    // ele.Arrange(new Rect(new Point(-page.Left, -page.Top), new Size((view.Page as DiagramPage).ActualWidth + (view.Page as DiagramPage).Left, (view.Page as DiagramPage).ActualHeight + (view.Page as DiagramPage).Top)));
                    ele.Arrange(new Rect(-(view.Page as DiagramPage).Left, -(view.Page as DiagramPage).Top, Math.Abs((view.Page as DiagramPage).Right * view.CurrentZoom + (view.Page as DiagramPage).Left * 2 - HorizontalOffset), Math.Abs((view.Page as DiagramPage).Bottom * view.CurrentZoom + (view.Page as DiagramPage).Top * 2 - VerticalOffset)));
                    if (!view.SizeToContent)
                    {
                        //view.background.Margin = new Thickness(view.BoundaryConstraintsArea.Left * view.CurrentZoom, view.BoundaryConstraintsArea.Top * view.CurrentZoom, 0, 0);
                        ele.Arrange(new Rect((view.Page as DiagramPage).Left - HorizontalOffset, (view.Page as DiagramPage).Top - VerticalOffset, view.BoundaryConstraintsArea.Right * view.CurrentZoom, view.BoundaryConstraintsArea.Bottom * view.CurrentZoom));
                    }
                    //else
                    //{
                    //    if (view.SizeToContent)
                    //    {

                    //        if (view.PageMargin.Left > 0)
                    //        {
                    //            if (view.PageMargin.Top > 0)
                    //            {
                    //                view.background.Margin = new Thickness(view.PageMargin.Left * 2 - HorizontalOffset, view.PageMargin.Top * 2 - VerticalOffset, 0, 0);
                    //            }
                    //            else
                    //            {
                    //                view.background.Margin = new Thickness(view.PageMargin.Left * 2, 0, 0, 0);
                    //            }
                    //        }
                    //        else if (view.PageMargin.Top > 0)
                    //        {
                    //            view.background.Margin = new Thickness(0, view.PageMargin.Top * 2, 0, 0);
                    //        }
                    //        else
                    //        {
                    //            view.background.Margin = new Thickness(0);
                    //        }
                    //    }
                    //}
                }
                else if (ele is DiagramViewGrid)
                {
                    if (view.nodragging)
                    {

                        ele.Arrange(new Rect(-page.Left, -page.Top, _ScrollOwner.ExtentWidth, _ScrollOwner.ExtentHeight));
                    }
                }
                else
                {
                    ele.Arrange(new Rect(0, 0, arrangeSize.Width, arrangeSize.Height));
                }
            }
            foreach (FrameworkElement ele in this.Children)
            {
                if (ele is DiagramViewGrid)
                {
                    if (view.nodragging)
                    {
                        ele.Arrange(new Rect(-(view.Page as DiagramPage).Left, -(view.Page as DiagramPage).Top, arrangeSize.Width, arrangeSize.Height));
                    }
                    // ele.Arrange(new Rect(-page.Left, -page.Top, arrangeSize.Width, arrangeSize.Height));
                }
                if (view != null && view.Page != null)
                {
                    page = view.Page as DiagramPage;
                    if (!(ele is System.Windows.Shapes.Rectangle))
                    {
                        ele.Arrange(new Rect(new Point(page.Left - HorizontalOffset, page.Top - VerticalOffset), new Size(ele.ActualWidth, ele.ActualHeight)));
                    }
                    else
                    {
                        //  ele.Arrange(new Rect(new Point(-HorizontalOffset, -VerticalOffset), new Size(ele.ActualWidth, ele.ActualHeight)));
                        org = new Size(page.ActualWidth, ele.ActualHeight);
                    }
                }
                if (ele is Rectangle)
                {
                    //ele.Arrange(new Rect(-page.Left, -page.Top, page.ActualWidth+page.Left, page.ActualHeight+ page.Top));
                }

            }
            VerifyScrollData(ButIHave, INeed);
            if (arrangeSize != new Size(1090, 506))
            {

            }
            return arrangeSize;

        }


        Size org = new Size();
        internal DiagramPage page;

        #region Virtualization members
        internal void callCalculate()
        {
            this.calculatesize(_ScrollOwner);
        }
        internal void MakeVirtualization()
        {
            if (view != null)
            {
                if (view.EnableVirtualization)
                {
                    this.callCalculate();
                    //disp = this.Dispatcher.BeginInvoke(System.Windows.Threading.Dispatcher.SystemIdle,
                    //                  new Syncfusion.Windows.Diagram.DiagramModel.Initialize(this.callCalculate));
                }
            }

        }
        internal void VerifyScrollVirtual()
        {
            if (view != null)
            {
                if (view.EnableVirtualization)
                {

                    this.calculatesize(_ScrollOwner);
                    //disp = this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
                    //                  new Syncfusion.Windows.Diagram.DiagramModel.Initialize(this.callCalculate));
                }
            }

        }

        internal void CallNormailzation()
        {
            this.CalculateNormalization();
            view.Page.InvalidateMeasure();
            view.Page.InvalidateArrange();
        }

        internal void VirtualizationRelatedToLine()
        {
            foreach (UIElement element in view.Model.Connections)
            {


                if (!(element as LineConnector).AllowVirtualization)
                {
                    (view.Page as DiagramPage).AddingChildren(element);

                    //}
                    //if (element is ICommon)
                    //{
                    //    if ((element as LineConnector).IsLoaded)
                    //    {
                    //        linebounds = (element as LineConnector).GetBounds();
                    //    }
                    //    else
                    //    {
                    //        linebounds = LineBounds(element as LineConnector);

                    //    }
                    //    if (viewablesize.IntersectsWith(linebounds))
                    //    {


                    //        if (!view.Page.Children.Contains(element))
                    //        {
                    //            (view.Page as DiagramPage).AddingChildren(element);
                    //            // view.Page.Children.Add(element);
                    //        }

                    //    }
                    //    else
                    //    {

                    //        view.Page.Children.Remove(element);
                    //    }
                    //}
                    if (view.EnableCaching)
                    {
                        if (view.Page.Children.Contains(element))
                        {
                            if ((element as LineConnector).HeadNode != null)
                            {
                                if (!(view.Page as DiagramPage).Children.Contains(((element as LineConnector).HeadNode as Node)))
                                {

                                    (view.Page as DiagramPage).AddingChildren(((element as LineConnector).HeadNode as Node));
                                }
                            }
                            if ((element as LineConnector).TailNode != null)
                            {
                                if (!(view.Page as DiagramPage).Children.Contains(((element as LineConnector).TailNode as Node)))
                                { (view.Page as DiagramPage).AddingChildren(((element as LineConnector).TailNode as Node)); }
                            }
                        }
                    }
                }
            }

        }
        internal Rect LineBounds(LineConnector line)
        {
            Rect rect = new Rect();
            if (line.PxStartPointPosition.X < line.PxEndPointPosition.X)
            {
                rect = new Rect(line.PxStartPointPosition.X, line.PxStartPointPosition.Y, Math.Abs(line.PxEndPointPosition.X - line.PxStartPointPosition.X), Math.Abs(line.PxEndPointPosition.Y - line.PxStartPointPosition.Y));
                return rect;
            }
            else
            {
                rect = new Rect(line.PxEndPointPosition.X, line.PxEndPointPosition.Y, Math.Abs(line.PxStartPointPosition.X - line.PxEndPointPosition.X), Math.Abs(line.PxStartPointPosition.Y - line.PxEndPointPosition.Y));
                return rect;
            }
        }

        private DiagramControl GetDiagramControl(DependencyObject dep)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(dep);
            while (parent != null)
            {
                if (parent is DiagramControl)
                {
                    return parent as DiagramControl;
                }
                DependencyObject temp = VisualTreeHelper.GetParent(parent);
                if (temp == null && parent is FrameworkElement)
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }
                else
                {
                    parent = temp;
                }
            }
            return null;
        }

        private void CalculateNormalization()
        {
            view = dc.View;
            foreach (UIElement node in (view.Page as DiagramPage).AllChildren)
            {
                if (node is Node)
                {
                    if (!(node as Node).IsInternallyLoaded)
                    {
                        if (!view.Page.Children.Contains(node))
                        {
                            (view.Page as DiagramPage).AddingChildren(node);
                            if (node.ReadLocalValue(Canvas.ZIndexProperty) == DependencyProperty.UnsetValue)
                            {
                                Canvas.SetZIndex((node as Node), (view.Page as DiagramPage).Children.Count);

                            }
                        }

                    }

                }
                else if (node is LineConnector)
                {

                    if (!(node as LineConnector).IsInternallyLoaded)
                    {
                        if (!view.Page.Children.Contains(node))
                        {
                            (view.Page as DiagramPage).AddingChildren(node);
                            if (node.ReadLocalValue(Canvas.ZIndexProperty) == DependencyProperty.UnsetValue)
                            {
                                Canvas.SetZIndex((node as LineConnector), (view.Page as DiagramPage).Children.Count);

                            }
                        }

                    }

                }

            }
        }

        internal void VirtualzingNode(Node element)
        {
            double Negativeaxistop = -(view.Page as DiagramPage).Top;
            double NegativeaxisLeft = -(view.Page as DiagramPage).Left;
            Rect viewablesize = new Rect();
            if (dc != null && !dc.IsLoaded)
            {
                view = dc.View;
                viewablesize = new Rect(0, 0, Constraint.Width, Constraint.Height);
            }
            else
            {
                viewablesize = GetViewableSize(_ScrollOwner);
            }

            if (!(element as Node).AllowVirtualization)
            {
                if (!(element as Node).IsInternallyLoaded)
                {
                    (view.Page as DiagramPage).AddingChildren(element);
                    (element as Node).Measure(new Size((element as Node).Width, (element as Node).Height));
                    (element as Node).Arrange(new Rect((element as Node).OffsetX, (element as Node).OffsetY, (element as Node).Width, (element as Node).Height));
                }
            }
            if (element is UIElement)
            {
                Rect nodebounds = new Rect((element as Node).PxOffsetX, (element as Node).PxOffsetY, (element as Node)._Width, (element as Node)._Height);
                if (IntersectWith(viewablesize, nodebounds))
                {
                    if (!(element as Node).IsInternallyLoaded)
                    {
                        (view.Page as DiagramPage).AddingChildren(element);
                    }
                }
                else if ((element as Node).IsInternallyLoaded)
                {
                    if ((element as Node).AllowVirtualization)
                    {
                        if (!view.EnableCaching)
                        {
                            if (!(element as Node).currentdragging)
                            {
                                (view.Page as DiagramPage).RemovingChildren(element);
                            }
                        }

                    }
                }
            }

        }

        internal bool IntersectWith(Rect Viewablearea, Rect childArea)
        {
            Viewablearea.Intersect(childArea);
            if (Viewablearea == Rect.Empty)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        internal void VirtualizingLine(LineConnector element)
        {
            double Negativeaxistop = -(view.Page as DiagramPage).Top;
            double NegativeaxisLeft = -(view.Page as DiagramPage).Left;
            Rect viewablesize = new Rect();
            if (dc != null && !dc.IsLoaded)
            {
                view = dc.View;
                viewablesize = new Rect(0, 0, Constraint.Width, Constraint.Height);
            }
            else
            {

                viewablesize = GetViewableSize(_ScrollOwner);
            }

            Rect linebounds = new Rect();
            if (!(element as LineConnector).AllowVirtualization)
            {
                if (!(element as LineConnector).IsInternallyLoaded)
                {
                    (view.Page as DiagramPage).AddingChildren(element);
                }
                (element as LineConnector).UpdateConnectorPathGeometry();
            }
            if (element is ICommon)
            {
                if ((element as LineConnector).IsInternallyLoaded)
                {
                    if ((element as LineConnector).HeadNode == null && (element as LineConnector).TailNode == null)
                    {
                        linebounds = LineBounds(element as LineConnector);
                        if (IntersectWith(viewablesize, linebounds))
                        {
                            if (!(element as LineConnector).IsInternallyLoaded)
                            {
                                (view.Page as DiagramPage).AddingChildren(element);
                            }
                        }
                        else if (!(element as LineConnector).IsInternallyLoaded && !view.EnableCaching)
                        {
                            (view.Page as DiagramPage).RemovingChildren(element);
                        }
                        else if (!view.EnableCaching)
                        {
                            (view.Page as DiagramPage).RemovingChildren(element);
                        }

                    }
                }
                else if (!(element as LineConnector).IsInternallyLoaded && !view.EnableCaching)
                {
                    (view.Page as DiagramPage).RemovingChildren(element);
                }
                else if (!view.EnableCaching)
                {
                    (view.Page as DiagramPage).RemovingChildren(element);
                }
            }
            if ((element as LineConnector).HeadNode != null && (element as LineConnector).TailNode != null)
            {

                if (((element as LineConnector).HeadNode as Node).IsInternallyLoaded || ((element as LineConnector).TailNode as Node).IsInternallyLoaded)
                {
                    if (!(element as LineConnector).IsInternallyLoaded)
                    {
                        (view.Page as DiagramPage).AddingChildren(element);
                    }
                }
                else
                {
                    if (!(element as LineConnector).IsInternallyLoaded && !view.EnableCaching)
                    {

                        (view.Page as DiagramPage).RemovingChildren(element);
                    }
                    else if (!view.EnableCaching)
                    {
                        (view.Page as DiagramPage).RemovingChildren(element);
                    }
                }
            }
            else
            {

                linebounds = LineBounds(element as LineConnector);
                if (IntersectWith(viewablesize, linebounds))
                {
                    if (!(element as LineConnector).IsInternallyLoaded)
                    {
                        (view.Page as DiagramPage).AddingChildren(element);
                    }
                }

            }
            (element as LineConnector).UpdateConnectorPathGeometry();
            (view.Page as DiagramPage).InvalidateMeasure();
            (view.Page as DiagramPage).InvalidateArrange();
            (element as LineConnector).UpdateConnectorPathGeometry();

        }
        private int CompareBandIndex(UIElement bar1, UIElement bar2)
        {
            if (bar1 is Node && bar2 is Node)
            {
                if ((bar1 as Node).OffsetX > (bar2 as Node).OffsetX)
                    return 1;
                else if ((bar1 as Node).OffsetX == (bar2 as Node).OffsetX)
                    return 0;
                else
                    return -1;
            }
            else
            {
                return 0;
            }
        }

        internal Rect GetViewableSize(ScrollViewer scroll)
        {
            Rect viewablesize = new Rect();
            if (dc != null && !dc.IsLoaded)
            {
                view = dc.View;
                viewablesize = new Rect(0, 0, Constraint.Width, Constraint.Height);
                return viewablesize;
            }
            else
            {
                double Negativeaxistop = -(view.Page as DiagramPage).Top;
                double NegativeaxisLeft = -(view.Page as DiagramPage).Left;
                double horoffset = HorizontalOffset;
                double veroffset = VerticalOffset;
                if (HorizontalOffset < 0)
                {
                    horoffset = 0;
                }
                if (VerticalOffset < 0)
                {
                    veroffset = 0;
                }
                if (NegativeaxisLeft < 0)
                {
                    if (Negativeaxistop < 0)
                    {
                        viewablesize = new Rect(horoffset + NegativeaxisLeft, veroffset + Negativeaxistop, scroll.ViewportWidth, scroll.ViewportHeight);
                    }
                    else
                    {
                        viewablesize = new Rect(horoffset + NegativeaxisLeft, veroffset, scroll.ViewportWidth, scroll.ViewportHeight);
                    }
                }
                else if (Negativeaxistop < 0)
                {
                    viewablesize = new Rect(horoffset, veroffset + Negativeaxistop, scroll.ViewportWidth, scroll.ViewportHeight);
                }
                else
                {
                    viewablesize = new Rect(horoffset, veroffset, scroll.ViewportWidth, scroll.ViewportHeight);
                }
                return viewablesize;
            }
        }
        bool onecheck = true;
        private void calculatesize(ScrollViewer scroll)
        {
            if (dc == null)
            {
                dc = DiagramPage.GetDiagramControl(this);
            }
            if (dc != null && !dc.IsLoaded && dc.Model.LayoutType != LayoutType.None && onecheck)
            {
                if (dc.Model.LayoutType == LayoutType.HierarchicalTreeLayout)
                {
                    HierarchicalTreeLayout tree = new HierarchicalTreeLayout(dc.Model, dc.View);
                    tree.RefreshLayout();
                }
                else if (dc.Model.LayoutType == LayoutType.DirectedTreeLayout)
                {
                    DirectedTreeLayout tree = new DirectedTreeLayout(dc.Model, dc.View);
                    tree.RefreshLayout();
                }
                else if (dc.Model.LayoutType == LayoutType.RadialTreeLayout)
                {
                    RadialTreeLayout tree = new RadialTreeLayout(dc.Model, dc.View);
                    tree.RefreshLayout();
                }
                else if (dc.Model.LayoutType == LayoutType.TableLayout)
                {
                    TableLayout tree = new TableLayout(dc.Model, dc.View);
                    tree.RefreshLayout();
                }
                else if (dc.Model.LayoutType == LayoutType.BowtieLayout)
                {
                    BowtieLayout tree = new BowtieLayout(dc.Model, dc.View);
                    tree.RefreshLayout();
                }
                onecheck = false;
            }

            Rect viewablesize = new Rect();
            viewablesize = GetViewableSize(scroll);
            foreach (UIElement element in dc.Model.Nodes)
            {
                VirtualzingNode(element as Node);
            }

            foreach (UIElement element in dc.Model.Connections)
            {
                VirtualizingLine(element as LineConnector);
            }
        }
        #endregion

    }

}