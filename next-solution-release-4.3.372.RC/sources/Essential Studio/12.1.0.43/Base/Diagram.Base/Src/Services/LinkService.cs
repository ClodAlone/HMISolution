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
using System.Text;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Link Manager service that use for update connector endpoints
    /// to it connected connection points.
    /// </summary>
    /// <remarks>
    /// Collection end point changes and synchronize it on model updating.
    /// </remarks>
    public class LinkManager
        : Service
    {
        #region Class constants
        /// <summary>
        /// Determine minimum path point count for handle synchronization.
        /// </summary>
        private const int c_DEF_HEADING_POINT_COUNT = 5;
        #endregion

        #region Class members
        /// <summary>
        ///  reference to diagram model.
        /// </summary>
        private Model m_model;

        /// <summary>
        /// End point synchronization list.
        /// </summary>
        private ArrayList m_synchronizationList;
        private int m_nSubTransactions;

        /// <summary>
        /// Determne is manager is synhroniza end point from synchronize list.
        /// </summary>
        private bool m_bSynhronizing = false;
        private ArrayList m_lstPassed = new ArrayList();
        #endregion

        #region Class events handlers
        /// <summary>
        /// Occurs when EndPoints synchronizing is started.
        /// </summary>
        public event EventHandler SynhronizeStarted;

        /// <summary>
        /// Occurs when EndPoints synchronizing is completed.
        /// </summary>
        public event EventHandler SynhronizeCompleted;
        #endregion

        #region Class protected properties
        /// <summary>
        /// Gets the end point synchronization list.
        /// </summary>
        /// <value>The synchronization list.</value>
        protected ArrayList SynchronizationList
        {
            get
            {
                if (m_synchronizationList == null)
                    m_synchronizationList = new ArrayList();

                return m_synchronizationList;
            }
        }

        /// <summary>
        /// Gets the reference to diagram model.
        /// </summary>
        /// <value>The model.</value>
        protected Model Model
        {
            get { return m_model; }
        }
        #endregion

        #region Class public properties
        /// <summary>
        /// Gets a value indicating whether end point synchronization is process.
        /// </summary>
        /// <value>
        /// <c>true</c> if end points synchronizing in process; otherwise, <c>false</c>.
        /// </value>
        public bool IsSynchronizing
        {
            get { return m_nSubTransactions > 0 || m_bSynhronizing; }
        }
        #endregion

        #region Class intialization
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkManager"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public LinkManager(Model model)
            : base()
        {
            if (model == null)
                throw new ArgumentNullException("Model parameter can't be null.");

            m_model = model;

            // subscribe model
            // m_model.EventSink.
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Adds node connections to synchronization list.
        /// </summary>
        /// <remarks>
        /// If Service is Stopped the method would not do anything.
        /// </remarks>
        /// <param name="node">The node.</param>
        public void SynchronizeNodeConnections(Node node)
        {
            if (!m_bSynhronizing && node != null && this.ServiceStatus != ServiceStatus.Stopped && this.ServiceStatus != ServiceStatus.Paused)
            {
                this.BeginSynchronization();

                ICompositeNode compositeNode = node as ICompositeNode;
                IEndPointContainer container = node as IEndPointContainer;

                if (compositeNode != null)
                {
                    for (int i = 0, length = compositeNode.ChildCount; i < length; i++)
                    {
                        SynchronizeNodeConnections(compositeNode.GetChild(i));
                    }
                }

                if (container != null)
                {
                    SynchronizeEndPoint(container.HeadEndPoint);
                    SynchronizeEndPoint(container.TailEndPoint);
                }

                foreach (ConnectionPoint port in node.Ports)
                {
                    foreach (EndPoint endPoint in port.Connections)
                    {
                        SynchronizeEndPoint(endPoint);
                    }
                }

                this.EndSynchronization();
            }
        }

        /// <summary>
        /// Adds endpoint to synchronization list.
        /// </summary>
        /// <remarks>
        /// If Service is Stopped the method would not do anything.
        /// </remarks>
        /// <param name="endPoint">The end point.</param>
        public void SynchronizeEndPoint(EndPoint endPoint)
        {
            if (!m_bSynhronizing && endPoint != null && this.ServiceStatus != ServiceStatus.Stopped && this.ServiceStatus != ServiceStatus.Paused)
            {
                if (!this.SynchronizationList.Contains(endPoint))
                {
                    this.BeginSynchronization();
                    this.SynchronizationList.Add(endPoint);
                    this.EndSynchronization();
                }
            }
        }

        /// <summary>
        /// Start of batch operation .
        /// </summary>
        public void BeginSynchronization()
        {
            if (!m_bSynhronizing)
            {
                m_nSubTransactions++;
            }
        }

        /// <summary>
        /// End of batch operation.
        /// </summary>
        public void EndSynchronization()
        {
            if (!m_bSynhronizing && m_nSubTransactions > 0)
            {
                m_nSubTransactions--;

                // start synchronize connectors in synhcronization list
                if (m_nSubTransactions == 0 && this.ServiceStatus != ServiceStatus.Paused
                    && this.ServiceStatus != ServiceStatus.Stopped)
                {
                    if (!m_bSynhronizing && this.SynchronizationList.Count > 0)
                    {
                        OnSynchronizeStarted();
                        m_bSynhronizing = true;

                        Synchronize(this.SynchronizationList);

                        this.SynchronizationList.Clear();

                        // clear connection passed collection
                        m_lstPassed.Clear();

                        m_bSynhronizing = false;
                        OnSynchronizeCompleted();
                    }
                }
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Called when service is stoped.
        /// </summary>
        protected override void OnStop()
        {
            // clear end point synchronization list
            this.SynchronizationList.Clear();

            base.OnStop();
        }
        #endregion

        #region Class virtual methods
        /// <summary>
        /// Called when endpoint synchronizing started.
        /// </summary>
        protected virtual void OnSynchronizeStarted()
        {
            if (SynhronizeStarted != null)
                SynhronizeStarted(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when endpoint synchronizing completed.
        /// </summary>
        protected virtual void OnSynchronizeCompleted()
        {
            if (SynhronizeCompleted != null)
                SynhronizeCompleted(this, EventArgs.Empty);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Synchronizes the specified endpoint collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        private void Synchronize(ICollection collection)
        {
            EndPointCollection connections = new EndPointCollection();

            // move endpoint to it port
            foreach (EndPoint endPoint in collection)
            {
                Node node = endPoint.Container as Node;
                IEndPointContainer container = endPoint.Container as IEndPointContainer;

                if (m_lstPassed.Contains(endPoint) || container == null)
                    continue;

                Line line = container as Line;
                ConnectorBase conn = container as ConnectorBase;
                bool bMerge = false;

                if (line != null)
                {
                    bMerge = (line.ConnectorState & ConnectorState.MergeControlPoints) == ConnectorState.MergeControlPoints;

                    if (bMerge)
                        line.ConnectorState = ~(line.ConnectorState & ConnectorState.MergeControlPoints);
                }
                else if (conn != null)
                {
                    bMerge = (conn.ConnectorState & ConnectorState.MergeControlPoints) == ConnectorState.MergeControlPoints;

                    if (bMerge)
                        conn.ConnectorState = ~(conn.ConnectorState & ConnectorState.MergeControlPoints);
                }

                // update second end point
                EndPoint secondPoint = (endPoint is HeadEndPoint) ? container.TailEndPoint : container.HeadEndPoint;

                ConnectionPoint port = endPoint.Port;
                ConnectionPoint secondPort = secondPoint.Port;

                // record to history two handle moving as one
                if ((port != null && port.Connections.Count > 0)
                    || (secondPort != null && secondPort.Connections.Count > 0))
                {
                    this.Model.HistoryManager.RecordMoveHandle(container as PathNode);
                }

                this.Model.HistoryManager.Pause();

                if (secondPort != null)
                {
                    if(port != null && this.Model != null && this.Model.EventSink != null)
                        this.Model.EventSink.Pause();

                    secondPoint.Location = GetEndPointLocation(secondPoint.Port, secondPoint);
                    SyncWithHeading(secondPoint);

                    if (secondPort is CentralPort)
                    {
                        if (port != null)
                        {
                            endPoint.Location = GetEndPointLocation(port, endPoint);
                            SyncWithHeading(endPoint);
                        }

                        secondPoint.Location = GetDockPoint(secondPoint, secondPort.Container);
                        SyncWithHeading(secondPoint);
                    }
                    if (port != null && this.Model != null && this.Model.EventSink != null)
                        this.Model.EventSink.Resume();
                }

                m_lstPassed.Add(secondPoint);

                if (port != null && port.Container != null)
                {
                    endPoint.Location = GetEndPointLocation(port, endPoint);
                    SyncWithHeading(endPoint);

                    // dock end point to central port
                    if (port is CentralPort)
                        endPoint.Location = GetDockPoint(endPoint, port.Container);

                    SyncWithHeading(endPoint);
                }
                m_lstPassed.Add(endPoint);

                foreach (ConnectionPoint connectorPort in endPoint.Container.Ports)
                {
                    connections.AddRange(connectorPort.Connections);
                }

                if (bMerge)
                {
                    if (line != null)
                        line.ConnectorState |= ConnectorState.MergeControlPoints;
                    else if (conn != null)
                        conn.ConnectorState |= ConnectorState.MergeControlPoints;
                }

                this.Model.HistoryManager.Resume();

                // update it parent if need
                //if (node != null && node.Parent != null)
                //    node.Parent.UpdateCompositeBounds();
            }

            if (connections.Count > 0)
            {
                Synchronize(connections);
            }
        }

        /// <summary>
        /// Gets the end point location.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="endPoint">The end point.</param>
        /// <returns>The end point location.</returns>
        private PointF GetEndPointLocation(ConnectionPoint port, EndPoint endPoint)
        {
            Node nodePortContainer = port.Container;

            // get port position in node coordinates
            PointF ptPortPosition = port.GetPosition();

            // convert to model coordinates
            ptPortPosition = nodePortContainer.ConvertToModelCoordinates(ptPortPosition);

            // convert to port container's coordinates
            Node parent = (endPoint.Container != null) ? endPoint.Container.Parent as Node : null;
            if (parent != null)
            {
                ptPortPosition = parent.ConvertToNodeCoordinates(ptPortPosition);
            }

            return ptPortPosition;
        }

        /// <summary>
        /// Updates the line segments of the orthogonal/polyline connector.
        /// <param name="connector"> Connector.</param>
        /// </summary>
        private void UpdateConnector(ConnectorBase connector)
        {
            RectangleF bounds = connector.BoundingRectangle;
            ArrayList pts = new ArrayList();
            Node fromNode = connector.FromNode as Node;
            Node toNode = connector.ToNode as Node;
            if (connector.LineSegments.Count > 1)
            {
                foreach (LineSegment segment in connector.LineSegments)
                {
                    PointF startPt = segment.Point1;
                    PointF endPt = segment.Point2;
                    startPt = new PointF(startPt.X + bounds.X, startPt.Y + bounds.Y);
                    endPt = new PointF(endPt.X + bounds.X, endPt.Y + bounds.Y);

                    if (fromNode != null && toNode != null)
                    {
                        if (!(fromNode.BoundingRectangle.Contains(startPt) && fromNode.BoundingRectangle.Contains(endPt)) &&
                            !(toNode.BoundingRectangle.Contains(startPt) && toNode.BoundingRectangle.Contains(endPt)))
                        {
                            pts.Add(segment.Point1);
                            pts.Add(segment.Point2);
                        }
                    }
                }
                PointF[] pathPts = (PointF[])pts.ToArray(typeof(PointF));
                if (connector is OrthogonalConnector && pathPts != null && fromNode != null && toNode != null)
                {
                    pathPts = Geometry.MergePointsInLine(pathPts);
                    if (pathPts.Length != 0)
                        connector.SetPoints(pathPts);
                    ControlPoint[] ctrlspts = connector.GetControlPoints();
                    if (ctrlspts != null && ctrlspts.Length == 2 && pathPts.Length > 2)
                    {
                        connector.InsertPoint(pathPts.Length - 2, new PointF(pathPts[pathPts.Length - 1].X, pathPts[pathPts.Length - 1].Y));
                    }
                }
                if (connector is PolyLineConnector)
                {
                    pathPts = Geometry.UpdatePointsOfPolyLine(pathPts);
                    if (pathPts.Length != 0)
                        connector.SetPoints(pathPts);
                }
            }
        }

        /// <summary>
        /// Gets the graphics path dock point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="nodePortContainer">The node port container.</param>
        /// <returns>The dock point.</returns>
        private PointF GetDockPoint(EndPoint endPoint, Node nodePortContainer)
        {
            // create point to return
            PointF ptDockPoint = endPoint.Location;

            //Updates the orthogoanl/Polyline connector
            if ((endPoint.Container is OrthogonalConnector && !(endPoint.Container is OrgLineConnector)) || endPoint.Container is PolyLineConnector)
                UpdateConnector(endPoint.Container as ConnectorBase);

            // prepare port container's graphics path
            GraphicsPath gpPathNode = nodePortContainer.GraphicsPath;
            PointF[] ptsSegment = GetIntersectSegment(endPoint, nodePortContainer);

            if (ptsSegment != null)
            {
                Matrix mtxEPCParent = HandlesHitTesting.GetParentsTransformations(endPoint.Container, false);
                Matrix mtxNPC = HandlesHitTesting.GetParentsTransformations(nodePortContainer, true);
                mtxNPC.Invert();

                mtxEPCParent.TransformPoints(ptsSegment);
                mtxNPC.TransformPoints(ptsSegment);
                Matrix mtx = new Matrix();
                if (ptsSegment[1].X < 0 || ptsSegment[1].Y < 0)
                    mtx.Translate(-1f, -1f);
                // widen graphics path to 1f pixel to include border
                using (Pen pnWiden = new Pen(CommonUsedValues.BACK_COLOR, 1f))
                {
                    if (gpPathNode.GetBounds(mtx, pnWiden).Size != SizeF.Empty)
                        gpPathNode.Widen(pnWiden, mtx);
                }

                // if path contains second point skip docking to port container path's 
                if (!(gpPathNode.IsVisible(ptsSegment[1]) && !gpPathNode.IsOutlineVisible(ptsSegment[1], Pens.Black)))
                {
                    // intersection point with container's path
                    PointF ptIntsct;
                    ConnectorBase connector = endPoint.Container as ConnectorBase;
                    CompassHeading heading = endPoint.GetHeading();

                    if (connector != null && m_model != null && m_model.LineRouter != null &&
                        m_model.LineRouter.RoutingMode != RoutingMode.Inactive && connector.LineRoutingEnabled)
                        heading = CompassHeading.None;

                    // orthogonal can dock only to horizontal and vertical side
                    if (endPoint.Container is OrthogonalConnector)
                    {
                        if ((int)heading > 4)
                        {
                            heading = CompassHeading.None;
                        }
                    }

                    // if intersection found update end point location
                    // to intersection point otherwise to port locaiton
                    if ((endPoint.Container is OrgLineConnector && !((OrgLineConnector)endPoint.Container).ObstaclesInPath) )                        
                    {
                        ptDockPoint = GetConnectorDockPoint(endPoint, nodePortContainer);                        
                    }
                    else if (Geometry.GetBoundaryIntercept(gpPathNode, MeasureUnits.Pixel, ptsSegment[0], ptsSegment[1], heading, out ptIntsct))
                    {
                        ptDockPoint = nodePortContainer.ConvertToModelCoordinates(ptIntsct);

                        // convert to port container's coordinates
                        Node parent = (endPoint.Container != null) ? endPoint.Container.Parent as Node : null;

                        if (parent != null)
                        {
                            ptDockPoint = parent.ConvertToNodeCoordinates(ptDockPoint);
                        }
                    }
                    else if (gpPathNode.IsOutlineVisible(ptsSegment[1], Pens.Black))
                    {
                        mtxNPC.Invert();
                        mtxEPCParent.Invert();
                        mtxEPCParent.TransformPoints(ptsSegment);
                        mtxNPC.TransformPoints(ptsSegment);

                        ptDockPoint = ptsSegment[1];
                    }
                    else
                    {
                        ptDockPoint = GetEndPointLocation(endPoint.Port, endPoint);
                    }
                }
            }

            return ptDockPoint;
        }

        /// <summary>
        /// Gets the OrgLineConnector's dock point
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="nodePortContainer">The node port container</param>
        /// <returns>The dock point.</returns>
        private PointF GetConnectorDockPoint(EndPoint endPoint, Node nodePortContainer)
        {
            PointF ptDockPoint = PointF.Empty;
            ConnectorBase conn = endPoint.Container as ConnectorBase;
            RectangleF frmNodeBounds, toNodeBounds, modelBounds = RectangleF.Empty;
            Model model = conn.Parent as Model;
            if (model != null)
                modelBounds.Size = new SizeF(model.LogicalSize.Width, model.LogicalSize.Height);

            if (conn.FromNode != null && conn.ToNode == null)
            {
                frmNodeBounds = ((Node)conn.FromNode).BoundingRectangle;
                modelBounds = new RectangleF(0, frmNodeBounds.Y, modelBounds.Width, frmNodeBounds.Height);
                toNodeBounds = new RectangleF(conn.HeadEndPoint.Location, new SizeF(8, 8));
                if (!(modelBounds.IntersectsWith(toNodeBounds)))
                {
                    if (frmNodeBounds.Y > conn.HeadEndPoint.Location.Y)
                        ptDockPoint = new PointF(frmNodeBounds.X + frmNodeBounds.Width / 2, frmNodeBounds.Y + 1);
                    else
                        ptDockPoint = new PointF(frmNodeBounds.X + frmNodeBounds.Width / 2, frmNodeBounds.Y + frmNodeBounds.Height + 1);
                }
                else
                {
                    if (frmNodeBounds.X > toNodeBounds.X)
                        ptDockPoint = new PointF(frmNodeBounds.X - 1, frmNodeBounds.Y + frmNodeBounds.Height / 2);
                    else
                        ptDockPoint = new PointF(frmNodeBounds.X + frmNodeBounds.Width + 1, frmNodeBounds.Y + frmNodeBounds.Height / 2);
                }
            }
            else if (conn.ToNode != null && conn.FromNode == null)
            {
                toNodeBounds = ((Node)conn.ToNode).BoundingRectangle;
                modelBounds = new RectangleF(0, toNodeBounds.Y, modelBounds.Width, toNodeBounds.Height);
                frmNodeBounds = new RectangleF(conn.TailEndPoint.Location, new SizeF(8, 8));
                if (!(modelBounds.IntersectsWith(frmNodeBounds)))
                {
                    if (conn.TailEndPoint.Location.Y < toNodeBounds.Y)
                        ptDockPoint = new PointF(toNodeBounds.X + toNodeBounds.Width / 2, toNodeBounds.Y + 1);
                    else
                        ptDockPoint = new PointF(toNodeBounds.X + toNodeBounds.Width / 2, toNodeBounds.Y + toNodeBounds.Height + 1);
                }
                else
                {
                    if (frmNodeBounds.X < toNodeBounds.X)
                        ptDockPoint = new PointF(toNodeBounds.X - 1, toNodeBounds.Y + toNodeBounds.Height / 2);
                    else
                        ptDockPoint = new PointF(toNodeBounds.X + toNodeBounds.Width + 1, toNodeBounds.Y + toNodeBounds.Height / 2);
                }

            }
            else if (conn.FromNode != null && conn.ToNode != null)
            {
                frmNodeBounds = ((Node)conn.FromNode).BoundingRectangle;
                toNodeBounds = ((Node)conn.ToNode).BoundingRectangle;
                modelBounds = new RectangleF(0, frmNodeBounds.Y, modelBounds.Width, frmNodeBounds.Height);
                string headName, tailName;
                tailName = conn.HeadingTail.ToString();
                headName = conn.HeadingHead.ToString();

                if (conn.HeadingHead == CompassHeading.Southeast || conn.HeadingHead == CompassHeading.Northeast)
                {
                    headName = "East";
                }
                if (conn.HeadingHead == CompassHeading.Northwest || conn.HeadingHead == CompassHeading.Southwest)
                {
                    headName = "West";
                }
                if (conn.HeadingTail == CompassHeading.Northwest || conn.HeadingHead == CompassHeading.Southwest)
                {
                    tailName = "West";
                }
                if (conn.HeadingTail == CompassHeading.Northeast || conn.HeadingTail == CompassHeading.Southeast)
                {
                    tailName = "East";
                }
                string direction = tailName + "To" + headName;

                if (!(modelBounds.IntersectsWith(toNodeBounds)) && (direction != "EastToEast" && direction != "WestToEast" && direction != "EastToWest" && direction != "WestToWest" && direction != "SouthToWest"
                    && direction != "EastToSouth" && direction != "SouthToEast"))
                {
                    if (endPoint is HeadEndPoint)
                    {
                        if (frmNodeBounds.Y < toNodeBounds.Y)
                            ptDockPoint = new PointF(toNodeBounds.X + toNodeBounds.Width / 2, toNodeBounds.Y + 1);
                        else
                            ptDockPoint = new PointF(toNodeBounds.X + toNodeBounds.Width / 2, toNodeBounds.Y + toNodeBounds.Height + 1);
                    }
                    else
                    {
                        if (frmNodeBounds.Y > toNodeBounds.Y)
                            ptDockPoint = new PointF(frmNodeBounds.X + frmNodeBounds.Width / 2, frmNodeBounds.Y + 1);
                        else
                            ptDockPoint = new PointF(frmNodeBounds.X + frmNodeBounds.Width / 2, frmNodeBounds.Y + frmNodeBounds.Height + 1);
                    }
                }
                else
                {
                    if (direction == "SouthToWest" || direction == "EastToSouth" || direction == "SouthToEast")
                    {
                        if (endPoint is HeadEndPoint)
                        {
                            if (frmNodeBounds.X < toNodeBounds.X)
                                ptDockPoint = new PointF(toNodeBounds.X - 1, toNodeBounds.Y + toNodeBounds.Height / 2);
                            else
                                ptDockPoint = new PointF(toNodeBounds.X + toNodeBounds.Width + 1, toNodeBounds.Y + toNodeBounds.Height / 2);
                        }
                        else
                        {
                            if (frmNodeBounds.Y > toNodeBounds.Y)
                                ptDockPoint = new PointF(frmNodeBounds.X + frmNodeBounds.Width / 2 - 1, frmNodeBounds.Y - 1);
                            else
                                ptDockPoint = new PointF(frmNodeBounds.X + frmNodeBounds.Width / 2, frmNodeBounds.Y + frmNodeBounds.Height );
                        }
                    }                    
                    else
                    {
                        if (endPoint is HeadEndPoint)
                        {
                            if (frmNodeBounds.X < toNodeBounds.X)
                                ptDockPoint = new PointF(toNodeBounds.X - 1, toNodeBounds.Y + toNodeBounds.Height / 2);
                            else
                                ptDockPoint = new PointF(toNodeBounds.X + toNodeBounds.Width + 1, toNodeBounds.Y + toNodeBounds.Height / 2);
                        }
                        else
                        {
                            if (frmNodeBounds.X > toNodeBounds.X)
                                ptDockPoint = new PointF(frmNodeBounds.X - 1, frmNodeBounds.Y + frmNodeBounds.Height / 2);
                            else
                                ptDockPoint = new PointF(frmNodeBounds.X + frmNodeBounds.Width + 1, frmNodeBounds.Y + frmNodeBounds.Height / 2);
                        }
                    }
                }
            }
            return ptDockPoint;
        }

        /// <summary>
        /// Gets the intersect segment.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="nodePortContainer">The node port container.</param>
        /// <returns>The intersect segment.</returns>
        private PointF[] GetIntersectSegment(EndPoint endPoint, Node nodePortContainer)
        {
            PointF[] ptsToReturn = null;
            PointF pt1, pt2;
            int startIndex = 0;

            // end point container
            PathNode nodeEPC = endPoint.Container;

            if (nodeEPC != null)
            {
                PointF[] ptsPath = nodeEPC.GetPoints();
                int nLength = ptsPath.Length;
                ptsToReturn = new PointF[2];
                if (endPoint.Container is OrgLineConnector)
                {
                    if (endPoint is HeadEndPoint)
                    {
                        Array.Copy(ptsPath, nLength - 2, ptsToReturn, 0, 2);
                        Array.Reverse(ptsToReturn);
                    }
                    else
                    {
                        Array.Copy(ptsPath, 0, ptsToReturn, 0, 2);
                    }
                }
                else
                {
                    if (endPoint is HeadEndPoint)
                    {
                        startIndex = ptsPath.Length - 2;
                        for (int i = ptsPath.Length - 1, j = ptsPath.Length - 2; i > 1 && j > 0; i--, j--)
                        {
                            pt1 = nodeEPC.ConvertToModelCoordinates(ptsPath[i]);
                            pt2 = nodeEPC.ConvertToModelCoordinates(ptsPath[j]);
                            if (nodePortContainer.ContainsPoint(pt1) && nodePortContainer.ContainsPoint(pt2))
                            {
                                startIndex = j - 1;
                            }
                        }
                        Array.Copy(ptsPath, startIndex, ptsToReturn, 0, 2);
                        Array.Reverse(ptsToReturn);
                    }
                    else
                    {
                        for (int i = 0, j = i + 1; i < ptsPath.Length && j < ptsPath.Length - 1; i++, j++)
                        {
                            pt1 = nodeEPC.ConvertToModelCoordinates(ptsPath[i]);
                            pt2 = nodeEPC.ConvertToModelCoordinates(ptsPath[j]);
                            if (nodePortContainer.ContainsPoint(pt1) && nodePortContainer.ContainsPoint(pt2))
                            {
                                startIndex = j;
                            }
                        }
                        Array.Copy(ptsPath, startIndex, ptsToReturn, 0, 2);
                    }
                }

                Matrix matrix = nodeEPC.GetTransformations();
                nodeEPC.AppendFlipTransforms(matrix);
                matrix.TransformPoints(ptsToReturn);
            }

            return ptsToReturn;
        }

        /// <summary>
        /// Synchronize the endpoint with heading.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>true, if sync with headpoint.</returns>
        private bool SyncWithHeading(EndPoint endPoint)
        {
            bool bSuccess = false;

            if (endPoint != null)
            {
                OrthogonalConnector orthoConnector = endPoint.Container as OrthogonalConnector;
                CompassHeading heading = endPoint.GetHeading();

                // check if container is orthogonal connector
                if (orthoConnector != null // && heading != Geometry.GetCompassHeading( endPoint )
                    && (orthoConnector.PointCount < c_DEF_HEADING_POINT_COUNT
                    || !CheckSegmentOrientation(orthoConnector, endPoint)))
                {
                    float fDistance = orthoConnector.HeadingDistance;

                    // get connector path points in parent coordinates
                    PointF[] ptsOrthoPath = orthoConnector.GetPoints(false);
                    
                    if (ptsOrthoPath.Length == 3 && orthoConnector.HeadEndPoint.GetHeading() == CompassHeading.None && 
                        orthoConnector.TailEndPoint.GetHeading() == CompassHeading.None)
                    {
                        ptsOrthoPath = Geometry.MergePointsInLine(ptsOrthoPath);
                        orthoConnector.SetPoints(ptsOrthoPath, false);
                    }
                    if (Geometry.RouteEndPointToHeading(ref ptsOrthoPath, endPoint, fDistance))
                    {
                        // set new path points
                        orthoConnector.SetPoints(ptsOrthoPath, false);
                        bSuccess = true;
                    }
                }
                else if (orthoConnector != null)
                {
                    orthoConnector.SyncWithHeading(endPoint);
                    bSuccess = true;
                }
            }

            return bSuccess;
        }
        private bool CheckSegmentOrientation(OrthogonalConnector orthoConnector, EndPoint endPoint)
        {
            bool bSuccess = false;

            if (orthoConnector != null)
            {
                CompassHeading heading = endPoint.GetHeading();
                int nIndex = (endPoint is HeadEndPoint) ? orthoConnector.LineSegments.Count - 1 : 0;
                OrthogonalLineSegment segment = orthoConnector.LineSegments[nIndex] as OrthogonalLineSegment;

                if (segment.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    bSuccess = (heading == CompassHeading.North || heading == CompassHeading.South);
                }
                else
                {
                    bSuccess = (heading == CompassHeading.East || heading == CompassHeading.West);
                }
            }

            return bSuccess;
        }
        #endregion
    }
}