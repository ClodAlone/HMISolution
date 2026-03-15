// <copyright file="NodeConnectorAdorner.cs" company="Syncfusion">
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.ComponentModel;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the adorner used for Node connectors.
    /// </summary>
    /// <remarks>
    ///  This is displayed while creating a new connection.
    /// </remarks>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class NodeConnectorAdorner : Adorner
    {
        #region Class variables

        /// <summary>
        /// Used to store the path geometry.
        /// </summary>
        private PathGeometry m_pathGeometry;

        /// <summary>
        /// Used to store the page instance.
        /// </summary>
        private IDiagramPage m_diagramPage;

        /// <summary>
        /// Used to store the view instance.
        /// </summary>
        private DiagramView dview;

        /// <summary>
        /// Used to store the source node.
        /// </summary>
        private Node sourceNode;

        /// <summary>
        /// Used to store the hit node.
        /// </summary>
        private Node hitNode;

        /// <summary>
        /// Used to store the drawing pen.
        /// </summary>
        private Pen drawingPen;

        /// <summary>
        /// Used to store the previously hit node.
        /// </summary>
        private Node previousHitNode = null;

        /// <summary>
        /// Used to store the  current hit port.
        /// </summary>
        private ConnectionPort m_hitPort;

        /// <summary>
        /// Used to store the previously hit port.
        /// </summary>
        private ConnectionPort previoushitport = null;

        /// <summary>
        /// Used to store the DiagramControl instance.
        /// </summary>
        private DiagramControl dc;

        /// <summary>
        /// Used to store the source port.
        /// </summary>
        private ConnectionPort sourceHitPort;

        /// <summary>
        /// Used to store the boolean information of center port being hit.
        /// </summary>
        private bool centerhit = false;

        /// <summary>
        /// Used to store the center port.
        /// </summary>
        private ConnectionPort centerport;

        /// <summary>
        /// Used to store the boolean information about the instance of connection
        /// </summary>
        private bool beforeconn = false;

        ///// <summary>
        /////  Used to check port visibility om hittesting.
        ///// </summary>
        //private bool isportcheck = false;

        #endregion

        #region  Properties

        /// <summary>
        /// Gets or sets the hit port.
        /// </summary>
        /// <value>The hit port.</value>
        private ConnectionPort HitPort
        {
            get
            {
                return m_hitPort;
            }

            set
            {
                if (m_hitPort != value)
                {
                    m_hitPort = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the node which is currently selected through HitTesting.
        /// </summary>
        /// <value>
        /// Type: <see cref="Node"/>
        /// The Node which was hit.
        /// </value>
        private Node HitNode
        {
            get
            {
                return hitNode;
            }

            set
            {
                if (hitNode != value)
                {
                    hitNode = value;
                }
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeConnectorAdorner"/> class.
        /// </summary>
        /// <param name="panel">The panel.</param>
        /// <param name="sourcePort">The source port.</param>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="diagramView">The diagram view.</param>
        public NodeConnectorAdorner(IDiagramPage panel, ConnectionPort sourcePort, Node sourceNode, DiagramView diagramView)
            : base(panel as Panel)
        {
            drawingPen = new Pen(Brushes.LightSlateGray, 1);
            sourceHitPort = sourcePort;
            dc = DiagramPage.GetDiagramControl((FrameworkElement)panel);
            this.m_diagramPage = panel;
            this.sourceNode = sourceNode;
            dview = diagramView;
            this.Cursor = Cursors.Cross;
            drawingPen.LineJoin = PenLineJoin.Round;
            diagramView.AddHandler(Control.MouseMoveEvent, new MouseEventHandler(diagramView_MouseMove), true);
            diagramView.AddHandler(Control.MouseUpEvent, new MouseButtonEventHandler(diagramView_MouseUp), true);
        }

        #endregion

        #region Class Override

        /// <summary>
        /// Calls render of the NodeConnectorAdorner.
        /// </summary>
        /// <param name="drawingcontext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingcontext)
        {
            base.OnRender(drawingcontext);
            drawingcontext.DrawGeometry(null, drawingPen, this.m_pathGeometry);
            drawingcontext.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
        }


        private void PrepareConectorType(LineConnector Connection)
        {
            if (dview.DrawingTool == DrawingTools.StraightLine)
            {
                Connection.ConnectorType = ConnectorType.Straight;
            }
            if (dview.DrawingTool == DrawingTools.OrthogonalLine)
            {
                Connection.ConnectorType = ConnectorType.Orthogonal;
            }
            if (dview.DrawingTool == DrawingTools.Arc)
            {
                Connection.ConnectorType = ConnectorType.Arc;
            }
            if (dview.DrawingTool == DrawingTools.BezierLine)
            {
                Connection.ConnectorType = ConnectorType.Bezier;
            }
        }

        /// <summary>
        /// Provides class handling for the MouseUp routed event that occurs when the mouse 
        /// button is released while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs.</param>
        //// protected override void OnMouseUp(MouseButtonEventArgs e)
        void diagramView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            LineConnector newConnection = new LineConnector();
            dview.m_connectionadorner = null;
            dview.Drawing_HeadNode = null;
            dview.Drawing_HeadPort = null;
            if (HitNode != null)
            {
                previousHitNode.IsDragConnectionOver = false;
                previousHitNode = null;
                Node sourceNode = this.sourceNode;
                Node targetNode = this.HitNode;
                if (sourceNode != targetNode)
                {
                    HitNode.IsDragConnectionOver = false;
                    hitNode.IsNodeFound = false;

                    if (HitNode is Group)
                    {
                        if (!sourceNode.Groups.Contains(HitNode))
                        {
                            foreach (object o in (HitNode as Group).NodeChildren.OfType<Node>())
                            {
                                (o as Node).IsDragConnectionOver = false;
                            }
                            newConnection = new LineConnector(sourceNode, targetNode, dview);
                        }
                    }

                    if (!(HitNode is Group))
                    {
                        newConnection = new LineConnector(sourceNode, targetNode, dview);
                    }

                    try
                    {
                        if (dc.View.PortMode == ConnectionMode.Connect)
                        {
                            foreach (ConnectionPort port in this.sourceNode.Ports)
                            {
                                port.IsDragOverPort = false;
                                if (port == sourceHitPort)
                                {
                                    if (port.CenterPortReferenceNo != 0)
                                    {
                                        newConnection.ConnectionHeadPort = port;
                                        newConnection.HeadNode = sourceNode;
                                        newConnection.HeadPortReferenceNo = port.PortReferenceNo;
                                    }
                                }
                            }

                            foreach (ConnectionPort port in HitNode.Ports)
                            {
                                port.IsDragOverPort = false;
                                if (port == HitPort)
                                {
                                    if (port.CenterPortReferenceNo != 0)
                                    {
                                        newConnection.ConnectionTailPort = port;
                                        newConnection.TailNode = HitNode;
                                        newConnection.TailPortReferenceNo = port.PortReferenceNo;
                                    }
                                }
                            }
                            if (newConnection.ConnectionTailPort==null && !centerhit && dc.View.NodeMode == ConnectionMode.Move)
                            {
                                newConnection.TailNode = null;
                                newConnection.EndPointPosition = e.GetPosition(dview.Page);
                            }
                        }
                    }
                    catch
                    {
                    }

                    Panel.SetZIndex(newConnection, (m_diagramPage as Panel).Children.Count);
                    if (HitNode is Group)
                    {
                        if (!sourceNode.Groups.Contains(HitNode))
                        {
                            dc.Model.Connections.Add(newConnection);
                        }
                    }

                    if (!(HitNode is Group))
                    {
                        dc.Model.Connections.Add(newConnection);
                    }

                    HitNode.IsDragConnectionOver = false;
                }
            }

            else
            {
                    try
                    {
                        foreach (ConnectionPort port in this.sourceNode.Ports)
                        {
                            port.IsDragOverPort = false;
                            if (port == sourceHitPort)
                            {
                                if (port.CenterPortReferenceNo != 0)
                                {
                                    newConnection.ConnectionHeadPort = port;
                                    newConnection.HeadNode = sourceNode;
                                    PrepareConectorType(newConnection);
                                    newConnection.HeadPortReferenceNo = port.PortReferenceNo;
                                    dview.Drawing_HeadPort = null;
                                    dview.Drawing_HeadNode = null;
                                }
                                else
                                {
                                    newConnection.HeadNode = sourceNode;
                                    PrepareConectorType(newConnection);
                                }
                            }
                        }
                        //sourceNode = null;
                        //sourceHitPort = null;
                    }
                    catch
                    {
                    }
                   
                newConnection.EndPointPosition = e.GetPosition(dview.Page as DiagramPage);
                dc.Model.Connections.Add(newConnection);
                Panel.SetZIndex(newConnection, (m_diagramPage as Panel).Children.Count);
            }
            if (HitNode != null)
            {
                ConnDragEndRoutedEventArgs newEventArgs = new ConnDragEndRoutedEventArgs(sourceNode as Node, HitNode as Node, newConnection as LineConnector);
                newEventArgs.RoutedEvent = DiagramView.ConnectorDragEndEvent;
                RaiseEvent(newEventArgs);
            }
            if (newConnection != null)
            {
                ConnDragEndRoutedEventArgs newEventArgs1 = new ConnDragEndRoutedEventArgs(sourceNode as Node, HitNode as Node, newConnection as LineConnector);
                newEventArgs1.RoutedEvent = DiagramView.AfterConnectionCreateEvent;
                RaiseEvent(newEventArgs1);
            }
            

            if ((sender as UIElement).IsMouseCaptured)
            {
                (sender as UIElement).ReleaseMouseCapture();
            }

            AdornerLayer adorner = AdornerLayer.GetAdornerLayer(this.m_diagramPage as Panel);
            if (adorner != null)
            {
                adorner.Remove(this);
               
            }
            dc.View.RemoveHandler(Control.MouseMoveEvent, new MouseEventHandler(diagramView_MouseMove));
            dc.View.RemoveHandler(Control.MouseUpEvent, new MouseButtonEventHandler(diagramView_MouseUp));
        }

        bool _cursorchanged = false;
        /// <summary>
        /// Provides class handling for the MouseMove routed event that occurs when the mouse 
        /// pointer  is over this control.
        /// </summary>
        /// <param name="e">The MouseEventArgs.</param>
        //// protected override void OnMouseMove(MouseEventArgs e)
        void diagramView_MouseMove(object sender, MouseEventArgs e)
        {

            
            LineConnector line = new LineConnector(dview);
            line.ConnectorType = (dview.Page as DiagramPage).ConnectorType;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!(sender as UIElement).IsMouseCaptured)
                {
                    if ((sender as DiagramView).Cursor != null)
                    {
                        (sender as DiagramView)._oldcursor = (sender as DiagramView).Cursor;
                    }
                    _cursorchanged = true;
                    (sender as DiagramView).Cursor = Cursors.Cross;
                    (sender as UIElement).CaptureMouse();
                }

                bool foundNode = HitTesting(e.GetPosition(this));
                if (!beforeconn && !this.sourceNode.BeforeConnection)
                {
                    BeforeCreateConnectionRoutedEventArgs newEventArgs = new BeforeCreateConnectionRoutedEventArgs(line as LineConnector);
                    newEventArgs.RoutedEvent = DiagramView.BeforeConnectionCreateEvent;
                    RaiseEvent(newEventArgs);
                }

                beforeconn = true;

                if (foundNode && HitNode != null)
                {
                   
                    UpdateGeometryPoint(e.GetPosition(this), line);
                }
                else
                {
                    UpdateGeometryPoint(e.GetPosition(this), line);
                    foreach (Node n in dc.Model.Nodes)
                    {
                        n.IsDragConnectionOver = false;
                    }
                }

                if (HitNode != null)
                {
                   
                    if (!centerhit)
                    {
                        foreach (ConnectionPort port in HitNode.Ports)
                        {
                            if (port != HitPort)
                            {
                                port.IsDragOverPort = false;
                            }
                        }

                        if (centerport != null)
                        {
                            centerport.IsDragOverPort = false;
                        }
                    }
                    else
                    {
                        if (centerhit)
                        {
                            foreach (ConnectionPort port in HitNode.Ports)
                            {
                                if (port.CenterPortReferenceNo != 0)
                                {
                                    port.IsDragOverPort = false;
                                }
                            }
                        }
                    }
                }
                this.InvalidateVisual();
            }
            else
            {
                if (_cursorchanged)
                {
                    (sender as DiagramView).Cursor = (sender as DiagramView)._oldcursor;
                }
                if (this.IsMouseCaptured)
                {
                    this.ReleaseMouseCapture();
                }
            }

        }

        #endregion

        #region Implementation

        /// <summary>
        /// Updates the geometry.
        /// </summary>
        /// <param name="point">The mouse position. </param>
        /// <param name="line">LineConnector instance</param>
        private void UpdateGeometryPoint(Point point, LineConnector line)
        {
            if (line.ConnectorType == ConnectorType.Bezier)
            {
                this.m_pathGeometry = UpdateBezierPathGeometry(point);
            }
            else if(line.ConnectorType == ConnectorType.Arc)
            {
                this.m_pathGeometry = UpdateArcPathGeometry(point, line);
            }
            else
            {
                this.m_pathGeometry = GetLinePathGeometry(point);
            }
        }

        /// <summary>
        /// Identifies the hit object.
        /// </summary>
        /// <param name="hitPoint">The point to be tested.</param>
        /// <returns>True if hit object is Node ,false otherwise.</returns>
        private bool HitTesting(Point hitPoint)
        {
            bool isporthit = false;
            DependencyObject hitelement = (m_diagramPage as Panel).InputHitTest(hitPoint) as DependencyObject;
            while (hitelement != null &&
                   hitelement != sourceNode.ParentNode)
            {
                if (hitelement is ConnectionPort)
                {
                    if ((hitelement as ConnectionPort).CenterPortReferenceNo == 0)
                    {
                        (hitelement as ConnectionPort).IsDragOverPort = true;

                        centerport = hitelement as ConnectionPort;
                        
                        if (previoushitport != null)
                        {
                            previoushitport.IsDragOverPort = false;
                        }
                      
                        HitPort = null;
                        previoushitport = null;
                        centerhit = true;
                    }
                    else
                    {
                        centerhit = false;
                        foreach (Node n in dc.Model.Nodes)
                        {
                            foreach (ConnectionPort port in n.Ports)
                            {
                                port.IsDragOverPort = false;
                                if (port == hitelement as ConnectionPort)
                                {
                                    HitPort = hitelement as ConnectionPort;
                                    HitNode = (hitelement as ConnectionPort).Node;
                                    previoushitport = hitelement as ConnectionPort;
                                    previoushitport.IsDragOverPort = true;
                                    isporthit = true;
                                }
                            }
                        }
                    }

                    try
                    {
                        foreach (Group g in ((hitelement as ConnectionPort).Node as Node).Groups)
                        {
                            g.IsDragConnectionOver = false;
                        }
                    }
                    catch 
                    { 
                    }
                   
                    HitNode = (hitelement as ConnectionPort).Node;
                    previousHitNode = (hitelement as ConnectionPort).Node;
                    if (HitNode != sourceNode)
                    {
                        if (!isporthit && dview.NodeMode==ConnectionMode.Connect)
                        {
                            previousHitNode.IsDragConnectionOver = true;
                        }
                    }

                    return true;
                }

                if (hitelement is Node)
                {
                    Node node = hitelement as Node;
                    centerhit = false;

                    if (!node.IsGrouped)
                    {
                        HitNode = hitelement as Node;
                        previousHitNode = hitelement as Node;
                        previoushitport = null;
                        HitPort = null;
                        if (HitNode != sourceNode && dview.NodeMode == ConnectionMode.Connect)
                        {
                            previousHitNode.IsDragConnectionOver = true;
                        }
                        if (hitNode != sourceNode)
                            previousHitNode.IsNodeFound = true;
                        foreach (Group g in HitNode.Groups)
                        {
                            g.IsDragConnectionOver = false;
                        }

                        return true;
                    }
                    else
                    {
                        if (node.IsGrouped)
                        {
                            if (dview.NodeMode == ConnectionMode.Connect)
                            {
                                node.IsDragConnectionOver = true;
                                node.IsNodeFound = true;
                            }
                            else
                            {
                                node.IsNodeFound = true;
                            }

                            HitNode = node.Groups[node.Groups.Count - 1] as Node;
                            previousHitNode = node.Groups[node.Groups.Count - 1] as Node;
                            if (previoushitport != null)
                            {
                                previoushitport.IsDragOverPort = false;
                            }

                            if (centerport != null)
                            {
                                centerport.IsDragOverPort = false;
                            }

                            previoushitport = null;
                            HitPort = null;

                            if (hitNode != sourceNode)
                                previousHitNode.IsNodeFound = true;
                            if (HitNode != sourceNode && dview.NodeMode == ConnectionMode.Connect)
                            {
                                previousHitNode.IsDragConnectionOver = true;
                            }

                            return true;
                        }
                    }

                    if (centerhit)
                    {
                        return true;
                    }

                    return false;
                }

                hitelement = VisualTreeHelper.GetParent(hitelement);
            }

           // isportcheck = false;
            if (previousHitNode != null)
            {
                previousHitNode.IsDragConnectionOver = false;
                previousHitNode.IsNodeFound = false;
            }

            if (previoushitport != null)
            {
                previoushitport.IsDragOverPort = false;
            }

            if (centerport != null)
            {
                centerport.IsDragOverPort = false;
            }

            HitNode = null;
            previoushitport = null;
            HitPort = null;

            return false;
        }

        /// <summary>
        /// Calculates the pathGeometry.
        /// </summary>
        /// <param name="position">The endpoint of the connector</param>
        /// <returns>The PathGeometry.</returns>
        private PathGeometry GetLinePathGeometry(Point position)
        {
            PathGeometry geometry = new PathGeometry();
            List<Point> pathPoints = GetLinePoints(sourceNode.GetInfo(), position);
            if (pathPoints.Count > 0)
            {
                PathFigure pathfigure = new PathFigure();
                pathfigure.StartPoint = pathPoints[0];
                pathPoints.Remove(pathPoints[0]);
                pathfigure.Segments.Add(new PolyLineSegment(pathPoints, true));
                geometry.Figures.Add(pathfigure);
            }

            return geometry;
        }

        /// <summary>
        /// Calculates the points which form the path geometry. 
        /// </summary>
        /// <param name="source">The head node</param>
        /// <param name="sinkPoint">The endpoint of the connector.</param>
        /// <returns>Collection of points.</returns>
        internal List<Point> GetLinePoints(NodeInfo source, Point sinkPoint)
        {
            LineConnector line = new LineConnector(dview);
            line.ConnectorType = (dview.Page as DiagramPage).ConnectorType;
            bool b1, b2, b3, b4, b5, b6, b7, b8;

            source.Position = source.Position;// MeasureUnitsConverter.ToPixels(source.Position, source.MeasurementUnit);

            Point p = new Point(0, 0);
            List<Point> pathPoints = new List<Point>();
            Rect rectSource = new Rect(
                                 source.Left,
                                 source.Top,
                                 source.Size.Width,
                                 source.Size.Height);
            Point st = new Point(source.Position.X, source.Position.Y);
            Point endPoint = sinkPoint;

            try
            {
                Point ep = endPoint;// MeasureUnitsConverter.FromPixels(endPoint, source.MeasurementUnit);
                if (!rectSource.Contains(ep))
                {
                    if (line.ConnectorType == ConnectorType.Orthogonal)
                    {
                        Point startPoint;
                        if (HitTesting(sinkPoint))
                        {
                            if (HitNode != null)
                            {
                                NodeInfo target = HitNode.GetInfo();

                                target.Position = target.Position;// MeasureUnitsConverter.ToPixels(target.Position, target.MeasurementUnit);

                                double hitwidth = HitNode.Width;// MeasureUnitsConverter.FromPixels(HitNode.Width, target.MeasurementUnit);
                                double hitheight = HitNode.Height;// MeasureUnitsConverter.FromPixels(HitNode.Height, target.MeasurementUnit);
                                Rect rectTarget = new Rect(HitNode.PxOffsetX, HitNode.PxOffsetY, hitwidth, hitheight);
                                ConnectorBase.GetOrthogonalLineIntersect(source, target, rectSource, rectTarget, out b1, out b2, out b3, out b4, out b5, out b6, out b7, out b8, out startPoint, out endPoint);

                                if (sourceHitPort != null)
                                {
                                    startPoint = new Point(sourceHitPort.Left, sourceHitPort.Top);
                                    startPoint = (sourceNode as Node).TranslatePoint(startPoint, dview.Page);
                                }

                                if (previoushitport != null)
                                {
                                    endPoint = new Point(previoushitport.Left, previoushitport.Top);
                                    endPoint = (HitNode as Node).TranslatePoint(endPoint, dview.Page);
                                }

                                pathPoints.Add(startPoint);
                                if (dc.View.NodeMode == ConnectionMode.Move)
                                {
                                    pathPoints.Add(Mouse.GetPosition(this));
                                }
                                else
                                {
                                    pathPoints.Add(endPoint);
                                }
                                pathPoints = AddLinePoints(pathPoints, new Rect[] { rectSource, rectTarget });
                                if (dc.View.NodeMode == ConnectionMode.Move)
                                {
                                    FindConnectionEnd(pathPoints, startPoint, Mouse.GetPosition(this), b1, b2, b3, b4, b5, b6, b7, b8);
                                }
                                else
                                {
                                    FindConnectionEnd(pathPoints, startPoint, endPoint, b1, b2, b3, b4, b5, b6, b7, b8);
                                }
                            }
                        }
                        else
                        {
                            Point s = ConnectorBase.GetLineIntersect(source, st, endPoint, rectSource, out b1, out b2, out b3, out b4, line.ConnectorType);
                            if (sourceHitPort != null)
                            {
                                s = new Point(sourceHitPort.Left, sourceHitPort.Top);
                                s = (sourceNode as Node).TranslatePoint(s, dview.Page);
                            }

                            if (s != p)
                            {
                                pathPoints.Add(s);
                            }

                            endPoint = Mouse.GetPosition(this);
                            pathPoints.Add(endPoint);
                            pathPoints = AddLinePoints(pathPoints, new Rect[] { rectSource });
                        }
                    }
                    else
                    {
                        if (HitTesting(sinkPoint))
                        {
                            Point s = ConnectorBase.GetLineIntersect(source, st, endPoint, rectSource, out b1, out b2, out b3, out b4, line.ConnectorType);
                            if (sourceHitPort != null)
                            {
                                s = new Point(sourceHitPort.Left, sourceHitPort.Top);
                                s = (sourceNode as Node).TranslatePoint(s, dview.Page);
                            }

                            if (s != p)
                            {
                                pathPoints.Add(s);
                            }

                            if (previoushitport != null)
                            {
                                endPoint = new Point(previoushitport.Left, previoushitport.Top);
                                endPoint = (HitNode as Node).TranslatePoint(endPoint, dview.Page);
                            }

                            pathPoints.Add(endPoint);
                        }
                        else
                        {
                            Point s = ConnectorBase.GetLineIntersect(source, st, endPoint, rectSource, out b1, out b2, out b3, out b4, line.ConnectorType);
                            if (sourceHitPort != null)
                            {
                                s = new Point(sourceHitPort.Left, sourceHitPort.Top);
                                s = (sourceNode as Node).TranslatePoint(s, dview.Page);
                            }

                            endPoint = Mouse.GetPosition(this);

                            if (s != p)
                            {
                                pathPoints.Add(s);
                            }

                            pathPoints.Add(endPoint);
                        }
                    }
                }
            }
            catch
            {
            }

            return pathPoints;
        }

        /// <summary>
        /// Is called whenever the path geometry is to be updated.
        /// </summary>
        /// <param name="position">The endpoint of the connector</param>
        /// <returns>The updated PathGeometry.</returns>
        private PathGeometry UpdateBezierPathGeometry(Point position)
        {
            Rect rectTarget = new Rect();
            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(0, 0);
            double x1 = 0;
            double y1 = 0;
            double x2 = 0;
            double y2 = 0;
            bool isLeft = false;
            bool isRight = false;
            bool isTop = false;
            bool isBottom = false;
            bool tisLeft = false;
            bool tisRight = false;
            bool tisTop = false;
            bool tisBottom = false;
            NodeInfo source = sourceNode.GetInfo();

            //source.Position = MeasureUnitsConverter.ToPixels(source.Position, source.MeasurementUnit);

            Rect sourceRect = new Rect(
                                 source.Left,
                                 source.Top,
                                 source.Size.Width,
                                 source.Size.Height);
            PathGeometry pathgeometry = new PathGeometry();
            Point p = position;// MeasureUnitsConverter.FromPixels(position, source.MeasurementUnit);
            if (!sourceRect.Contains(p))
            {
                if (HitTesting(position))
                {
                    if (HitNode != null)
                    {
                        NodeInfo target = HitNode.GetInfo();

                        target.Position = target.Position;// MeasureUnitsConverter.ToPixels(target.Position, target.MeasurementUnit);

                        double hitwidth = HitNode.Width;// MeasureUnitsConverter.FromPixels(HitNode.Width, target.MeasurementUnit);
                        double hitheight = HitNode.Height;// MeasureUnitsConverter.FromPixels(HitNode.Height, target.MeasurementUnit);
                        rectTarget = new Rect(HitNode.PxOffsetX, HitNode.PxOffsetY, hitwidth, hitheight);
                        ConnectorBase.GetOrthogonalLineIntersect(source, target, sourceRect, rectTarget, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint);
                        if (sourceHitPort != null)
                        {
                            startPoint = new Point(sourceHitPort.Left, sourceHitPort.Top);
                            startPoint = (sourceNode as Node).TranslatePoint(startPoint, dc.View.Page);
                        }

                        if (previoushitport != null)
                        {
                            endPoint = new Point(previoushitport.Left, previoushitport.Top);
                            endPoint = (HitNode as Node).TranslatePoint(endPoint, dc.View.Page);
                        }

                        x1 = startPoint.X;
                        y1 = startPoint.Y;
                        x2 = endPoint.X;
                        y2 = endPoint.Y;
                    }
                }
                else
                {
                    Point po = new Point(0, 0);
                    po = sourceNode.PxPosition;
                    if (sourceHitPort != null)
                    {
                        po = new Point(sourceHitPort.Left, sourceHitPort.Top);
                        po = (sourceNode as Node).TranslatePoint(po, dview.Page);
                    }

                    x1 = po.X;
                    y1 = po.Y;
                    x2 = Mouse.GetPosition(this).X;
                    y2 = Mouse.GetPosition(this).Y;
                }

                PathFigure pathfigure = new PathFigure();
                double num = Math.Max((double)(Math.Abs((double)(x2 - x1)) / 2.0), (double)20.0);
                pathfigure.StartPoint = new Point(x1, y1);
                
                BezierSegment segment = new BezierSegment();
                if (isBottom)
                {
                    segment = GetSegment(x1, y1 + num, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                }
                else if (isTop)
                {
                    segment = GetSegment(x1, y1 - num, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                }
                else if (isRight)
                {
                    segment = GetSegment(x1 + num, y1, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                }
                else
                {
                    segment = GetSegment(x1 - num, y1, x2, y2, x2, y2, num, tisTop, tisBottom, tisLeft, tisRight);
                }

                pathfigure.Segments.Add(segment);
                pathgeometry.Figures.Add(pathfigure);
            }

            return pathgeometry;
        }

        private PathGeometry UpdateArcPathGeometry(Point position, LineConnector line)
        {
            Rect rectTarget = new Rect();
            Point startPoint = new Point(0, 0);
            Point endPoint = new Point(0, 0);
            double x1 = 0;
            double y1 = 0;
            double x2 = 0;
            double y2 = 0;
            bool isLeft = false;
            bool isRight = false;
            bool isTop = false;
            bool isBottom = false;
            bool tisLeft = false;
            bool tisRight = false;
            bool tisTop = false;
            bool tisBottom = false;
            NodeInfo source = sourceNode.GetInfo();

            //source.Position = MeasureUnitsConverter.ToPixels(source.Position, source.MeasurementUnit);

            Rect sourceRect = new Rect(
                                 source.Left,
                                 source.Top,
                                 source.Size.Width,
                                 source.Size.Height);
            PathGeometry pathgeometry = new PathGeometry();
            Point p = position;// MeasureUnitsConverter.FromPixels(position, source.MeasurementUnit);
            if (!sourceRect.Contains(p))
            {
                if (HitTesting(position))
                {
                    if (HitNode != null)
                    {
                        NodeInfo target = HitNode.GetInfo();

                        target.Position = target.Position;// MeasureUnitsConverter.ToPixels(target.Position, target.MeasurementUnit);

                        double hitwidth = HitNode.Width;// MeasureUnitsConverter.FromPixels(HitNode.Width, target.MeasurementUnit);
                        double hitheight = HitNode.Height;// MeasureUnitsConverter.FromPixels(HitNode.Height, target.MeasurementUnit);
                        rectTarget = new Rect(HitNode.PxOffsetX, HitNode.PxOffsetY, hitwidth, hitheight);
                        ConnectorBase.GetArcLineIntersect(source, target, sourceRect, rectTarget, out isTop, out isBottom, out isLeft, out isRight, out tisTop, out tisBottom, out tisLeft, out tisRight, out startPoint, out endPoint, line.ArcDirection);
                        if (sourceHitPort != null)
                        {
                            startPoint = new Point(sourceHitPort.Left, sourceHitPort.Top);
                            startPoint = (sourceNode as Node).TranslatePoint(startPoint, dc.View.Page);
                        }

                        if (previoushitport != null)
                        {
                            endPoint = new Point(previoushitport.Left, previoushitport.Top);
                            endPoint = (HitNode as Node).TranslatePoint(endPoint, dc.View.Page);
                        }

                        x1 = startPoint.X;
                        y1 = startPoint.Y;
                        x2 = endPoint.X;
                        y2 = endPoint.Y;
                    }
                }
                else
                {
                    Point po = new Point(0, 0);
                    po = sourceNode.PxPosition;
                    if (sourceHitPort != null)
                    {
                        po = new Point(sourceHitPort.Left, sourceHitPort.Top);
                        po = (sourceNode as Node).TranslatePoint(po, dview.Page);
                    }

                    x1 = po.X;
                    y1 = po.Y;
                    x2 = Mouse.GetPosition(this).X;
                    y2 = Mouse.GetPosition(this).Y;
                }

                PathFigure pathfigure = new PathFigure();
                pathfigure.StartPoint = new Point(x1, y1);
                double x = Math.Pow((x1 - x2), 2);
                double y = Math.Pow((y1 - y2), 2);
                double d = Math.Sqrt(x + y);

                double angle = ConnectorBase.findAngle(new Point(x1, y1), new Point(x2, y2));

                pathfigure.Segments.Add(new ArcSegment(new Point(x2, y2), new Size(d / 2, line.ArcHeight), angle, false, line.ArcDirection, true));

                pathgeometry.Figures.Add(pathfigure);
            }

            return pathgeometry;
        }        

        /// <summary>
        /// Returns the Bezier segment.
        /// </summary>
        /// <param name="x1">The x coordinate of the starting point(first control point) of the curve.</param>
        /// <param name="y1">The y coordinate of the starting point(first control point)of the curve.</param>
        /// <param name="x2">The x coordinate of the end point of the curve.</param>
        /// <param name="y2">The y coordinate of the end point of the curve.</param>
        /// <param name="temp1">The x coordinate of the second control point of the curve.</param>
        /// <param name="temp2">The y coordinate of the second control point of the curve.</param>
        /// <param name="num1">It specifies the amount of curve to be provided.Value is 150d.</param>
        /// <param name="isTop">Flag indicating the top side.</param>
        /// <param name="isBottom">Flag indicating the bottom side.</param>
        /// <param name="isLeft">Flag indicating the left side.</param>
        /// <param name="isRight">Flag indicating the right side.</param>
        /// <returns>The segment.</returns>
        private BezierSegment GetSegment(double x1, double y1, double x2, double y2, double temp1, double temp2, double num1, bool isTop, bool isBottom, bool isLeft, bool isRight)
        {
            if (isTop)
            {
                return Segment(x1, y1, x2, y2, temp1, temp2 - num1);
            }
            else if (isBottom)
            {
                return Segment(x1, y1, x2, y2, temp1, temp2 + num1);
            }
            else if (isLeft)
            {
                return Segment(x1, y1, x2, y2, temp1 - num1, temp2);
            }
            else
            {
                return Segment(x1, y1, x2, y2, temp1 + num1, temp2);
            }
        }

        /// <summary>
        /// Returns the segment.
        /// </summary>
        /// <param name="x1">The x coordinate of the starting point(first control point) of the curve.</param>
        /// <param name="y1">The y coordinate of the starting point(first control point) of the curve.</param>
        /// <param name="x2">The x coordinate of the end point of the curve.</param>
        /// <param name="y2">The y coordinate of the end point of the curve.</param>
        /// <param name="temp1">The x coordinate of the second control point of the curve.</param>
        /// <param name="temp2">The y coordinate of the second control point of the curve.</param>
        /// <returns>The segment,</returns>
        private BezierSegment Segment(double x1, double y1, double x2, double y2, double temp1, double temp2)
        {
            BezierSegment segment = new BezierSegment
            {
                Point1 = new Point(x1, y1),
                Point2 = new Point(temp1, temp2),
                Point3 = new Point(x2, y2)
            };
            return segment;
        }

        /// <summary>
        /// Adds points to the collection in case of orthogonal line .
        /// </summary>
        /// <param name="linePoints">Collection of points.</param>
        /// <param name="rectangles">The nodes involved in the connection.</param>
        /// <returns>The modified collection of points.</returns>
        private List<Point> AddLinePoints(List<Point> linePoints, Rect[] rectangles)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < linePoints.Count; i++)
            {
                points.Add(linePoints[i]);
            }

            points.Insert(1, new Point(points[1].X, points[0].Y));
            return points;
        }

        /// <summary>
        /// Makes the end connection to the respective node by finding the correct direction of the node.
        /// </summary>
        /// <param name="pathPoints">Collection of points.</param>
        /// <param name="startPoint">The start point of the connector.</param>
        /// <param name="endPoint">The end point of the connector.</param>
        /// <param name="isTop">Flag indicating the top side of the source.</param>
        /// <param name="isBottom">Flag indicating the bottom side of the source.</param>
        /// <param name="isLeft">Flag indicating the left side of the source.</param>
        /// <param name="isRight">Flag indicating the right side of the source.</param>
        /// <param name="tisTop">Flag indicating the top side of the target.</param>
        /// <param name="tisBottom">Flag indicating the bottom side of the target.</param>
        /// <param name="tisLeft">Flag indicating the left side of the target.</param>
        /// <param name="tisRight">Flag indicating the right side of the target.</param>
        private void FindConnectionEnd(List<Point> pathPoints, Point startPoint, Point endPoint, bool isTop, bool isBottom, bool isLeft, bool isRight, bool tisTop, bool tisBottom, bool tisLeft, bool tisRight)
        {
            Point startpoint = new Point(0, 0);
            Point endpoint = new Point(0, 0);

            if (isRight)
            {
                startpoint = new Point(startPoint.X - 7, startPoint.Y);
            }
            else if (isBottom)
            {
                startpoint = new Point(startPoint.X, startPoint.Y - 7);
            }
            else if (isLeft)
            {
                startpoint = new Point(startPoint.X + 7, startPoint.Y);
            }
            else if (isTop)
            {
                startpoint = new Point(startPoint.X, startPoint.Y + 7);
            }

            if (tisRight)
            {
                endpoint = new Point(endPoint.X - 7, endPoint.Y);
            }
            else if (tisBottom)
            {
                endpoint = new Point(endPoint.X, endPoint.Y - 7);
            }
            else if (tisLeft)
            {
                endpoint = new Point(endPoint.X + 7, endPoint.Y);
            }
            else if (tisTop)
            {
                endpoint = new Point(endPoint.X, endPoint.Y + 7);
            }

            pathPoints.Insert(0, startpoint);
            pathPoints.Add(endpoint);
        }
        #endregion
    }
}
