#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Processes input events and translates them into actions on the diagram.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The controller is an object that is responsible for handling input in the
    /// model-view-controller architecture. The controller receives input events
    /// and translates them into commands that affect the model and view.
    /// </para>
    /// <para>
    /// This class is an abstract base class from which concrete controller
    /// classes are derived. This class does not does not register any tools.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Model"/>
    /// </remarks>
    [Serializable]
    [TypeConverter(typeof(ControllerConverter))]
    public class Controller
        : IServiceReferenceHolder,
          IServiceReferenceProvider
    {
        #region Class members
        private IServiceReferenceProvider m_provider;

        /// <summary>
        /// Document attached to controller.
        /// </summary>
        private Model m_document;

        /// <summary>
        /// View attached to this controller.
        /// </summary>
        protected IViewer m_viewer;

        /// <summary>
        /// View attached to this controller.
        /// </summary>
        private View m_view;
        private UpdateInfo m_updateInfo;

        /// <summary>
        /// Indicates whether Undo/Redo operation is pending.
        /// </summary>
        private bool m_bUndoOrRedo;

        /// <summary>
        /// Helper collection used while Undo/Redo operation.
        /// </summary>
        private NodeCollection m_nodesHelper;

        /// <summary>
        /// Stores number of requests to update viewer.
        /// </summary>
        /// <remarks>
        /// As we can have subrequests, viewer will be updated only
        /// when requests count is 0.
        /// </remarks>
        private int m_nUpdateRequests;
        private bool m_bRecreatePG = false;

        /// <summary>
        /// Origin offset by update model to content.
        /// </summary>
        private SizeF m_szOriginOffset = SizeF.Empty;

        /// <summary>
        /// Indicate that UpdateSizeToContent() method in process.
        /// </summary>
        private bool m_bSizeToContentUpdating = false;
        protected bool m_bNeedDocumentRefresh = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a value indicating whether this instance can update view.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can update view; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool CanUpdateView
        {
            get { return m_nUpdateRequests == 0; }
        }

        /// <summary>
        /// Gets the update info.
        /// </summary>
        [Browsable(false)]
        public UpdateInfo UpdateInfo
        {
            get
            {
                if (m_updateInfo == null)
                {
                    m_updateInfo = new UpdateInfo();
                }

                return m_updateInfo;
            }
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>The view.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public View View
        {
            get { return m_view; }
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Forms.Diagram.View"/> object
        /// attached to this controller.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IViewer Viewer
        {
            get { return m_viewer; }
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Forms.Diagram.Model"/> object
        /// attached to this controller.
        /// </summary>
        /// <remarks>
        /// The Model object is attached to the controller indirectly through
        /// the View object. 
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Model Model
        {
            get { return m_document; }
        }

        /// <summary>
        /// Gets list of nodes that are currently selected.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NodeCollection SelectionList
        {
            get { return this.View.SelectionList; }
        }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>The provider.</value>
        protected IServiceReferenceProvider Provider
        {
            get { return m_provider; }
        }
        #endregion

        #region Class utility methods
        #region Command wrapper methods
        /// <summary>
        /// Groups the currently selected nodes in a diagram.
        /// </summary>
        public void Group()
        {
            if (this.Model == null)
                return;

            if (this.SelectionList.Count >= 2)
            {
                // start atomic action
                Model document = this.Model;

                document.BeginUpdate();
                document.HistoryManager.StartAtomicAction("Group");

                // iterate through adding nodes
                NodeCollection nodesToGroup = new NodeCollection(this.SelectionList, false);
                ArrayList portConnections = SaveConnections(nodesToGroup, false);

                // create group
                Group group = new Group();
                
                // set EditStyle.AspectRatio == true for group 
                group.EditStyle.AspectRatio = true;

                // sort nodes by zorder before removing it from model
                nodesToGroup.Sort(new ZOrderComparer());

                // remove children from it parent
                RemoveChildren(nodesToGroup);

                // add group to document
                document.AppendChild(group);

                int n;
                group.AppendChildren(nodesToGroup, out n);

                // lock update before reconnect connections
                group.LockUpdate = true;

                // reconnect disconnected connections
                RestoreConnections(portConnections);

                // update group bounds
                group.LockUpdate = false;
                group.UpdateCompositeBounds();

                UpdateBridges(group);

                // update selection list
                this.SelectionList.Clear();
                this.SelectionList.Add(group);

                document.HistoryManager.EndAtomicAction();
                document.EndUpdate();
            }
        }
        private void UpdateBridges(Node node)
        {
            ConnectorBase connector = node as ConnectorBase;
            ICompositeNode group = node as ICompositeNode;

            if (connector != null)
            {
                this.Model.BridgeManager.AddToIntersectCollection(connector);
            }
            else if (group != null)
            {
                for (int i = 0, length = group.ChildCount; i < length; i++)
                {
                    UpdateBridges(group.GetChild(i));
                }
            }
        }

        /// <summary>
        /// Ungroups currently selected group in a diagram.
        /// </summary>
        public void UnGroup()
        {
            if (this.Model == null)
                return;

            Model document = this.Model;
            
            // start atomic action
            document.BeginUpdate();
            document.HistoryManager.StartAtomicAction("UnGroup");

            NodeCollection nodesToAdd = new NodeCollection();
            NodeCollection nodeToUngroup = new NodeCollection(this.SelectionList, false);

            lock (this.SelectionList)
            {
                ICompositeNode compositeNode;
                ICompositeNode ungroupNodeParent;

                ArrayList portConnections;

                // iterate through given nodes searching for groups to ungroup
                foreach (Node group in nodeToUngroup)
                {
                    nodesToAdd.Clear();
                    compositeNode = group as ICompositeNode;

                    if (compositeNode != null && compositeNode.CanUngroup)
                    {
                        // save connection for a while converting child location
                        ungroupNodeParent = group.Parent;
                        portConnections = SaveConnections(nodeToUngroup, true);

                        // get group's children collection
                        for (int nIdx = 0, nLength = compositeNode.ChildCount; nIdx < nLength; nIdx++)
                            nodesToAdd.Add(compositeNode.GetChild(nIdx));

                        int n;
                        bool bLock = ((Group)group).LockUpdate;
                        ((Group)group).LockUpdate = true;

                        // remove the group's children
                        compositeNode.RemoveAllChildren();

                        ((Group)group).LockUpdate = bLock;
                        ungroupNodeParent.AppendChildren(nodesToAdd, out n);

                        bool bLineBridgingEnabled = this.Model.LineBridgingEnabled;
                        QuiteBridgingSet(false);

                        // restore connection after all children 
                        // converted to new parent coordinates
                        // and also update nodes connections
                        RestoreConnections(portConnections);

                        QuiteBridgingSet(bLineBridgingEnabled);
                    }
                }
            }

            // Update selections
            this.SelectionList.Clear();
            this.SelectionList.AddRange(nodesToAdd);

            document.HistoryManager.EndAtomicAction();
            document.EndUpdate();
        }

        /// <summary>
        /// Deletes the selected nodes from the diagram.
        /// </summary>
        public void Delete()
        {
            if (this.SelectionList.IsEmpty || this.Model == null) return;            

            this.Model.HistoryManager.StartAtomicAction(this.SelectionList.Count == 1 ? "Remove Node" : "Remove Nodes");
            NodeCollection nodes = new NodeCollection(this.SelectionList, false);
            // SelectionList must be cleared before remove operation;
            foreach (Node node in nodes)
                this.SelectionList.Remove(node);

            // remove nodes from it parent
            foreach (Node node in nodes)
            {
                if (node.Parent != null)
                {
                    node.Parent.RemoveChild(node);
                }
            }           
            this.Model.HistoryManager.EndAtomicAction();
        }

        /// <summary>
        /// Brings the selected nodes to the front of the Z-order.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        /// <remarks>
        /// This method creates and executes a
        /// <see cref="Syncfusion.Windows.Forms.Diagram.ZOrderCmd"/> command.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ZOrderCmd"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controller.SelectionList"/>
        /// </remarks>
        public bool BringToFront()
        {
            bool bSuccess = false;

            if (this.SelectionList.IsEmpty || this.Model == null) return bSuccess;

            Model document = this.Model;
            
            // weap transaction
            document.HistoryManager.StartAtomicAction("BringToFront");

            lock (this.SelectionList)
            {
                NodeCollection nodesCopy = new NodeCollection();
                nodesCopy.AddRange(this.SelectionList);
                nodesCopy.Sort(new ZOrderComparer());

                foreach (Node nodeTemp in nodesCopy)
                {
                    document.BringToFront(nodeTemp);
                }

                bSuccess = true;
            }

            document.HistoryManager.EndAtomicAction();

            return bSuccess;
        }

        /// <summary>
        /// Sends the selected nodes to the back of the Z-order.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        /// <remarks>
        /// This method creates and executes a
        /// <see cref="Syncfusion.Windows.Forms.Diagram.ZOrderCmd"/> command.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ZOrderCmd"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controller.SelectionList"/>
        /// </remarks>
        public bool SendToBack()
        {
            bool bSuccess = false;

            if (this.SelectionList.IsEmpty || this.Model == null) return bSuccess;

            Model document = this.Model;

            // weap transaction
            document.HistoryManager.StartAtomicAction("SendToBack");

            lock (this.SelectionList)
            {
                NodeCollection nodesCopy = new NodeCollection();
                nodesCopy.AddRange(this.SelectionList);
                nodesCopy.Sort(new ReverseZOrderComparer());

                foreach (Node nodeTemp in nodesCopy)
                {
                    document.SendToBack(nodeTemp);
                }

                bSuccess = true;
            }

            document.HistoryManager.EndAtomicAction();

            return bSuccess;
        }

        /// <summary>
        /// Brings the selected nodes forward in the Z-order.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        /// <remarks>
        /// This method creates and executes a
        /// <see cref="Syncfusion.Windows.Forms.Diagram.ZOrderCmd"/> command.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ZOrderCmd"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controller.SelectionList"/>
        /// </remarks>
        public bool BringForward()
        {
            bool bSuccess = false;

            if (this.SelectionList.IsEmpty || this.Model == null) return bSuccess;

            Model document = this.Model;
            
            // weap transaction
            document.HistoryManager.StartAtomicAction("Bring Forward");

            lock (this.SelectionList)
            {
                NodeCollection nodesCopy = new NodeCollection();
                nodesCopy.AddRange(this.SelectionList);
                nodesCopy.Sort(new ReverseZOrderComparer());

                foreach (Node nodeTemp in nodesCopy)
                {
                    document.BringForward(nodeTemp);
                }

                bSuccess = true;
            }

            document.HistoryManager.EndAtomicAction();

            return bSuccess;
        }

        /// <summary>
        /// Sends the selected nodes backward in the Z-order.
        /// </summary>
        /// <returns>True if successful; otherwise False.</returns>
        /// <remarks>
        /// This method creates and executes a
        /// <see cref="Syncfusion.Windows.Forms.Diagram.ZOrderCmd"/> command.
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ZOrderCmd"/>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controller.SelectionList"/>
        /// </remarks>
        public bool SendBackward()
        {
            bool bSuccess = false;

            if (this.SelectionList.IsEmpty || this.Model == null) return bSuccess;

            Model document = this.Model;
            
            // wrap transaction
            document.HistoryManager.StartAtomicAction("Send Backward");

            lock (this.SelectionList)
            {
                NodeCollection nodesCopy = new NodeCollection();
                nodesCopy.AddRange(this.SelectionList);
                nodesCopy.Sort(new ZOrderComparer());

                foreach (Node nodeTemp in nodesCopy)
                {
                    document.SendBackward(nodeTemp);
                }

                bSuccess = true;
            }

            document.HistoryManager.EndAtomicAction();

            return bSuccess;
        }

        /// <summary>
        /// Adds all nodes in the model to the SelectionList.
        /// </summary>
        public void SelectAll()
        {
            // this.Model.BeginUpdate();
            // clear selection list
            this.View.SelectionList.Clear();

            if (this.Model != null)
            {
                Node curNode;
                int nCounter = 0;
                int nLength = this.Model.ChildCount;
                NodeCollection nodes = new NodeCollection();
                nodes.QuietMode = true;

                while (nCounter < nLength)
                {
                    curNode = this.Model.GetChild(nCounter);

                    if (EditStyle.CanSelect(curNode))
                    {
                        nodes.Add(curNode);
                    }

                    nCounter++;
                }

                this.View.SelectionList.AddRange(nodes);
                nodes.QuietMode = false;
                nodes.Clear();
            }

            // this.Model.EndUpdate();
        }
        #endregion

        #region Converting coordinates

        #region To Model coordinates
        /// <summary>
        /// Converts to model coordinates.
        /// </summary>
        /// <param name="ptClientLocation">The pt client location.</param>
        /// <returns>Point in model coordinate.</returns>
        public Point ConvertToModelCoordinates(Point ptClientLocation)
        {
            float fMagnification = this.View.Magnification / 100f;
            int nRuler = this.Viewer.ShowRulers ? (int)(this.Viewer.RulersHeight / fMagnification) : 0;

            return new Point(
                (int)(ptClientLocation.X / fMagnification + this.View.Origin.X - nRuler),
                (int)(ptClientLocation.Y / fMagnification + this.View.Origin.Y - nRuler));
        }

        /// <summary>
        /// Converts to model coordinates.
        /// </summary>
        /// <param name="ptClientLocation">The pt client location.</param>
        /// <returns>Points in model coordinates.</returns>
        public PointF ConvertToModelCoordinates(PointF ptClientLocation)
        {
            float fMagnification = this.View.Magnification / 100f;
            float nRuler = this.Viewer.ShowRulers ? (this.Viewer.RulersHeight / fMagnification) : 0;

            return new PointF(
                ptClientLocation.X / fMagnification + this.View.Origin.X - nRuler,
                ptClientLocation.Y / fMagnification + this.View.Origin.Y - nRuler);
        }

        /// <summary>
        /// Convert size to model coordinate.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>Size to model coordinates.</returns>
        public Size ConvertToModelCoordinates(Size size)
        {
            float fMagnification = this.View.Magnification / 100f;

            return new Size(
                (int)(size.Width / fMagnification),
                (int)(size.Height / fMagnification));
        }

        /// <summary>
        /// Convert size to model coordinate.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>Size in model coordinates.</returns>
        public SizeF ConvertToModelCoordinates(SizeF size)
        {
            float fMagnification = this.View.Magnification / 100f;

            return new SizeF(
                size.Width / fMagnification,
                size.Height / fMagnification);
        }

        /// <summary>
        /// Converts to model coordinates.
        /// </summary>
        /// <param name="rectClientLocation">The rect client location.</param>
        /// <returns>Rect in model coordinates.</returns>
        public System.Drawing.Rectangle ConvertToModelCoordinates(System.Drawing.Rectangle rectClientLocation)
        {
            Point ptLocationInModelUnits = ConvertToModelCoordinates(rectClientLocation.Location);
            Size szSizeInModelUnits = ConvertToModelCoordinates(rectClientLocation.Size);

            return new System.Drawing.Rectangle(ptLocationInModelUnits, szSizeInModelUnits);
        }

        /// <summary>
        /// Converts to model coordinates.
        /// </summary>
        /// <param name="rectClientLocation">The rect client location.</param>
        /// <returns>Rect in model coordinates.</returns>
        public RectangleF ConvertToModelCoordinates(RectangleF rectClientLocation)
        {
            PointF ptLocationInModelUnits = ConvertToModelCoordinates(rectClientLocation.Location);
            SizeF szSizeInModelUnits = ConvertToModelCoordinates(rectClientLocation.Size);

            return new RectangleF(ptLocationInModelUnits, szSizeInModelUnits);
        }
        #endregion

        #region From Model coordinates
        /// <summary>
        /// Converts from model to client coordinates.
        /// </summary>
        /// <param name="ptModelLocation">The pt model location.</param>
        /// <returns>Point from model to client coordinates.</returns>
        public Point ConvertFromModelToClientCoordinates(Point ptModelLocation)
        {
            float fMagnification = this.View.Magnification / 100f;
            int nRuler = this.Viewer.ShowRulers ? this.Viewer.RulersHeight : 0;

            return new Point(
                (int)(ptModelLocation.X * fMagnification - (this.View.Origin.X * fMagnification - nRuler)),
                (int)(ptModelLocation.Y * fMagnification - (this.View.Origin.Y * fMagnification - nRuler)));
        }

        /// <summary>
        /// Converts from model to client coordinates.
        /// </summary>
        /// <param name="ptModelLocation">The pt model location.</param>
        /// <returns>Point from model to client coordinates.</returns>
        public PointF ConvertFromModelToClientCoordinates(PointF ptModelLocation)
        {
            float fMagnification = this.View.Magnification / 100f;
            int nRuler = this.Viewer.ShowRulers ? this.Viewer.RulersHeight : 0;

            return new PointF(
                ptModelLocation.X * fMagnification - (this.View.Origin.X * fMagnification - nRuler),
                ptModelLocation.Y * fMagnification - (this.View.Origin.Y * fMagnification - nRuler));
        }

        /// <summary>
        /// Converts from model to client coordinates.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>Size from model to client coordinates.</returns>
        public Size ConvertFromModelToClientCoordinates(Size size)
        {
            float fMagnification = this.View.Magnification / 100f;

            return new Size((int)(size.Width * fMagnification), (int)(size.Height * fMagnification));
        }

        /// <summary>
        /// Converts from model to client coordinates.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>Size from model to client coordinates.</returns>
        public SizeF ConvertFromModelToClientCoordinates(SizeF size)
        {
            float fMagnification = this.View.Magnification / 100f;

            return new SizeF(size.Width * fMagnification, size.Height * fMagnification);
        }

        /// <summary>
        /// Converts from model to client coordinates.
        /// </summary>
        /// <param name="rectModel">The rect model.</param>
        /// <returns>Rect from model to client coordinates.</returns>
        public System.Drawing.Rectangle ConvertFromModelToClientCoordinates(System.Drawing.Rectangle rectModel)
        {
            Point ptLocation = ConvertFromModelToClientCoordinates(rectModel.Location);
            Size szSize = ConvertFromModelToClientCoordinates(rectModel.Size);

            return new System.Drawing.Rectangle(ptLocation, szSize);
        }

        /// <summary>
        /// Converts from model to client coordinates.
        /// </summary>
        /// <param name="rectModel">The rect model.</param>
        /// <returns>Rect from model to client coordinates.</returns>
        public RectangleF ConvertFromModelToClientCoordinates(RectangleF rectModel)
        {
            PointF ptLocation = ConvertFromModelToClientCoordinates(rectModel.Location);
            SizeF szSize = ConvertFromModelToClientCoordinates(rectModel.Size);

            return new RectangleF(ptLocation, szSize);
        }
        #endregion

        #endregion

        #region Group helper methods
        /// <summary>
        /// Saves the connections.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="bSkipTopNodes">if set to <c>true</c> given nodes 
        /// except it chidlren wouldn't saved to result list.</param>
        /// <returns>Save connections.</returns>
        private ArrayList SaveConnections(NodeCollection nodes, bool bSkipTopNodes)
        {
            // save port connection before remove node
            ArrayList portConnections = new ArrayList();
            ArrayList containerConnections;
            EndPointConnection connection;
            ICompositeNode container;

            foreach (Node node in nodes)
            {
                container = node as ICompositeNode;

                if (container != null)
                {
                    NodeCollection containerNodes = new NodeCollection();

                    Node nodeTemp;
                    int nLength = container.ChildCount;
                    int nCounter = 0;

                    while (nCounter < nLength)
                    {
                        nodeTemp = container.GetChild(nCounter);
                        containerNodes.Add(nodeTemp);

                        nCounter++;
                    }

                    containerConnections = SaveConnections(containerNodes, false);

                    if (containerConnections != null && containerConnections.Count > 0)
                        portConnections.AddRange(containerConnections);
                }

                IEndPointContainer endPointContainer = node as IEndPointContainer;
                if (endPointContainer != null)
                {
                    if (endPointContainer.HeadEndPoint.Port != null)
                    {
                        connection = new EndPointConnection();
                        connection.Port = endPointContainer.HeadEndPoint.Port;
                        EndPointCollection endPoints = new EndPointCollection();
                        endPoints.Add(endPointContainer.HeadEndPoint);
                        connection.EndPoints = endPoints;

                        if (!bSkipTopNodes)
                            portConnections.Add(connection);

                        connection.Port.Disconnect(endPointContainer.HeadEndPoint);
                    }

                    if (endPointContainer.TailEndPoint.Port != null)
                    {
                        connection = new EndPointConnection();
                        connection.Port = endPointContainer.TailEndPoint.Port;
                        EndPointCollection endPoints = new EndPointCollection();
                        endPoints.Add(endPointContainer.TailEndPoint);
                        connection.EndPoints = endPoints;

                        if (!bSkipTopNodes)
                            portConnections.Add(connection);

                        connection.Port.Disconnect(endPointContainer.TailEndPoint);
                    }
                }

                foreach (ConnectionPoint port in node.Ports)
                {
                    if (port.Connections.Count > 0)
                    {
                        connection = new EndPointConnection();
                        connection.Port = port;
                        EndPointCollection endPoints = new EndPointCollection();
                        endPoints.AddRange(port.Connections);
                        connection.EndPoints = endPoints;

                        if (!bSkipTopNodes)
                            portConnections.Add(connection);

                        port.DisconnectAll();
                    }
                }
            }

            return portConnections;
        }

        /// <summary>
        /// Restores the connections.
        /// </summary>
        /// <param name="portConnections">The port connections.</param>
        private void RestoreConnections(ArrayList portConnections)
        {
            // reconnect disconnecter connections
            foreach (EndPointConnection portConnection in portConnections)
            {
                foreach (EndPoint endPoint in portConnection.EndPoints)
                {
                    portConnection.Port.Connect(endPoint);
                }
            }
        }
        #endregion
        #endregion

        #region IServiceReferenceHolder Members
        /// <summary>
        /// Updates the service references from service provider.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        public virtual void UpdateServiceReferences(IServiceReferenceProvider provider)
        {
            if (provider == null)
            {
                if (m_document != null)
                    UnsubscribeFromDocumentEvents(m_document);

                m_document = null;

                m_view = null;

                if (m_viewer != null)
                    UnsubscribeFromViewerEvents(m_viewer);

                m_viewer = null;
            }
            else
            {
                if (m_document != null)
                    UnsubscribeFromDocumentEvents(m_document);

                m_document = (Model)provider.ProvideServiceReference(typeof(Model).TypeHandle);

                if (m_document != null)
                    SubscribeForDocumentEvents(m_document);

                if (m_viewer != null)
                    UnsubscribeFromViewerEvents(m_viewer);

                m_viewer = (IViewer)provider.ProvideServiceReference(typeof(IViewer).TypeHandle);
                m_view = (View)provider.ProvideServiceReference(typeof(View).TypeHandle);
                if (m_view.Controller == null)
                    m_view.Controller = this;
                if (m_viewer != null)
                    SubscribeForViewerEvents(m_viewer);
            }

            m_provider = provider;
        }
        #endregion

        #region IServiceReferenceProvider Members
        /// <summary>
        /// Provides the service reference.
        /// </summary>
        /// <param name="typeHandle">The type handle.</param>
        /// <returns>The object.</returns>
        public virtual object ProvideServiceReference(RuntimeTypeHandle typeHandle)
        {
            return null;
        }
        #endregion

        #region Class Public methods
        /// <summary>
        /// Gets the bounding rectangle from node collections.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="units">The units.</param>
        /// <returns>Nodes bounding rectangle.</returns>
        public RectangleF GetBoundingRect(ICollection nodes, MeasureUnits units)
        {
            RectangleF rectTrackerToReturn = RectangleF.Empty;
            Node nodeTemp;

            foreach (object nodeCur in nodes)
            {
                nodeTemp = nodeCur as Node;

                if (nodeTemp != null)
                {
                    RectangleF rectBounding = nodeTemp.BoundingRectangle;

                    if (rectTrackerToReturn.Size.IsEmpty)
                    {
                        rectTrackerToReturn = rectBounding;
                    }
                    else
                    {
                        rectTrackerToReturn = RectangleF.Union(rectBounding, rectTrackerToReturn);
                    }
                }
            }

            return rectTrackerToReturn;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Remove unvisible nodes and layers from selection list.
        /// </summary>
        /// <param name="layer">The layer.</param>
        private void DeselectNodes(Layer layer)
        {
            if (this.SelectionList.IsEmpty || layer == null)
                return;

            NodeCollection nodesToSelect = new NodeCollection(this.SelectionList, false);

            // check for unvisible node
            foreach (Node layerNode in layer.Nodes)
            {
                if (this.SelectionList.Contains(layerNode))
                    nodesToSelect.Remove(layerNode);
            }

            // deselect layer nodes
            if (nodesToSelect.Count != this.SelectionList.Count)
            {
                this.SelectionList.Clear();
                this.SelectionList.AddRange(nodesToSelect);
            }
        }

        /// <summary>
        /// Removes the children from it parent.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        private void RemoveChildren(NodeCollection nodes)
        {
            if (nodes == null)
                return;

            lock (this.SelectionList)
            {
                ICompositeNode curParent;

                // remove nodes in selection list from their parents
                foreach (Node nodeCur in nodes)
                {
                    curParent = nodeCur.Parent;

                    // suppress AllowDelete flag
                    // AllowDeletePreviousValue
                    bool fADPV = SuppressAllowDeleteFlag(nodeCur);

                    // remove node from parent's children
                    if (curParent != null)
                        curParent.RemoveChild(nodeCur);

                    // restore prev value
                    RestoreAllowDeleteValue(nodeCur, fADPV);
                }
            }
        }

        /// <summary>
        /// Quites set AllowDelete value without record in history and calling sink events.
        /// </summary>
        /// <param name="node">Node to restore AllowDelete property value of.</param>
        /// <param name="bPrevValue">AlloeDelete property value to be set.</param>
        private void RestoreAllowDeleteValue(Node node, bool bPrevValue)
        {
            Model model = this.Model;

            // Quite set boundary constrains
            // Needed for property change pin point and offset
            // without checking only one changes
            if (model != null)
            {
                model.EventSink.Pause();
                model.HistoryManager.Pause();

                node.EditStyle.AllowDelete = bPrevValue;

                model.HistoryManager.Resume();
                model.EventSink.Resume();
            }
        }

        /// <summary>
        /// Quites set AllowDelete value without record in history and calling sink events.
        /// </summary>
        /// <param name="node">Node to suppress flag of.</param>
        /// <returns>true, if suppress allow delete value.</returns>
        private bool SuppressAllowDeleteFlag(Node node)
        {
            bool bPreviousValue = false;
            Model model = this.Model;

            // Quite set boundary constrains
            // Needed for property change pin point and offset
            // without checking only one changes
            if (model != null)
            {
                model.EventSink.Pause();
                model.HistoryManager.Pause();

                bPreviousValue = node.EditStyle.AllowDelete;
                node.EditStyle.AllowDelete = true;

                model.HistoryManager.Resume();
                model.EventSink.Resume();
            }

            return bPreviousValue;
        }

        /// <summary>
        /// Quites set boundary value without record in history and calling sink events.
        /// </summary>
        /// <param name="bBoundaryConstraintsEnabled">if set to <c>true</c> boundary constraints enabled.</param>
        private void QuiteBoundarySet(bool bBoundaryConstraintsEnabled)
        {
            Model model = this.Model;

            // Quite set boundary constrains
            // Needed for property change pin point and offset
            // without checking only one changes
            if (model != null)
            {
                model.EventSink.Pause();
                model.HistoryManager.Pause();
                model.BoundaryConstraintsEnabled = bBoundaryConstraintsEnabled;
                model.HistoryManager.Resume();
                model.EventSink.Resume();
            }
        }
        private void QuiteBridgingSet(bool bLineBridgingEnable)
        {
            Model model = this.Model;

            if (model != null)
            {
                model.EventSink.Pause();
                model.HistoryManager.Pause();
                model.LineBridgingEnabled = bLineBridgingEnable;
                model.HistoryManager.Resume();
                model.EventSink.Resume();
            }
        }
        private void UnsubscribeFromDocumentEvents(Model document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            document.EventSink.PropertyChanging -= new PropertyChangingEventHandler(Document_PropertyChanging);
            document.EventSink.PropertyChanged -= new PropertyChangedEventHandler(Document_PropertyChanged);
            document.EventSink.ZOrderChanged -= new ZOrderChangedEventHandler(Document_ZOrderChanged);
            document.EventSink.VertexChanged -= new VertexChangedEventHandler(Document_VertexChanged);
            document.EventSink.VertexChanging -= new VertexChangingEventHandler(Document_VertexChanging);
            document.EventSink.DocumentBeginUpdate -= new EventHandler(Document_BeginUpdate);
            document.EventSink.DocumentEndUpdate -= new EventHandler(Document_EndUpdate);
            document.EventSink.NodeCollectionChanged -= new CollectionExEventHandler(Document_NodeCollectionChanged);
            document.EventSink.NodeCollectionChanging -= new CollectionExEventHandler(Document_NodeCollectionChanging);
            document.EventSink.CancelCollectionChanged -= new CancelCollectionChangedEventHandler(EventSink_CancelCollectionChanged);
            document.EventSink.FlipChanging -= new FlipChangingEventHandler(EventSink_FlipChanging);
            document.EventSink.FlipChanged -= new FlipChangedEventHandler(EventSink_FlipChanged);
            document.EventSink.PinOffsetChanged -= new PinOffsetChangedEventHandler(EventSink_PinOffsetChanged);
            document.EventSink.PinOffsetChanging -= new PinOffsetChangingEventHandler(EventSink_PinOffsetChanging);
            document.EventSink.PinPointChanged -= new PinPointChangedEventHandler(EventSink_PinPointChanged);
            document.EventSink.PinPointChanging -= new PinPointChangingEventHandler(EventSink_PinPointChanging);
            document.EventSink.RotationChanged -= new RotationChangedEventHandler(EventSink_RotationChanged);
            document.EventSink.RotationChanging -= new RotationChangingEventHandler(EventSink_RotationChanging);
            document.EventSink.SizeChanged -= new SizeChangedEventHandler(EventSink_SizeChanged);
            document.EventSink.SizeChanging -= new SizeChangingEventHandler(EventSink_SizeChanging);
            document.EventSink.LabelsChanged -= new CollectionExEventHandler(Document_NodeLabelsChanged);

            document.HistoryManager.UndoCommandStarted -= new EventHandler(HistoryManager_CommandStarted);
            document.HistoryManager.UndoCommandCompleted -= new EventHandler(HistoryManager_CommandCompleted);
            document.HistoryManager.RedoCommandStarted -= new EventHandler(HistoryManager_CommandStarted);
            document.HistoryManager.RedoCommandCompleted -= new EventHandler(HistoryManager_CommandCompleted);
            document.HistoryManager.RecordRequest -= new EventHandler(HistoryManager_CommandRequest);
            document.HistoryManager.RecordComplete -= new EventHandler(HistoryManager_CommandRequestComplete);

            document.LinkManager.SynhronizeStarted -= new EventHandler(LinkManager_SynhronizeStarted);
            document.LinkManager.SynhronizeCompleted -= new EventHandler(LinkManager_SynhronizeCompleted);

            document.BridgeManager.BridgeGenerationStarted -= new EventHandler(BridgeManager_BridgeGenerationStarted);
            document.BridgeManager.BridgeGenerationCompleted -= new EventHandler(BridgeManager_BridgeGenerationCompleted);
        }

        private void SubscribeForDocumentEvents(Model document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            document.EventSink.PropertyChanging += new PropertyChangingEventHandler(Document_PropertyChanging);
            document.EventSink.PropertyChanged += new PropertyChangedEventHandler(Document_PropertyChanged);
            document.EventSink.ZOrderChanged += new ZOrderChangedEventHandler(Document_ZOrderChanged);
            document.EventSink.VertexChanged += new VertexChangedEventHandler(Document_VertexChanged);
            document.EventSink.VertexChanging += new VertexChangingEventHandler(Document_VertexChanging);
            document.EventSink.DocumentBeginUpdate += new EventHandler(Document_BeginUpdate);
            document.EventSink.DocumentEndUpdate += new EventHandler(Document_EndUpdate);
            document.EventSink.NodeCollectionChanged += new CollectionExEventHandler(Document_NodeCollectionChanged);
            document.EventSink.NodeCollectionChanging += new CollectionExEventHandler(Document_NodeCollectionChanging);
            document.EventSink.CancelCollectionChanged += new CancelCollectionChangedEventHandler(EventSink_CancelCollectionChanged);
            document.EventSink.FlipChanging += new FlipChangingEventHandler(EventSink_FlipChanging);
            document.EventSink.FlipChanged += new FlipChangedEventHandler(EventSink_FlipChanged);
            document.EventSink.PinOffsetChanged += new PinOffsetChangedEventHandler(EventSink_PinOffsetChanged);
            document.EventSink.PinOffsetChanging += new PinOffsetChangingEventHandler(EventSink_PinOffsetChanging);
            document.EventSink.PinPointChanged += new PinPointChangedEventHandler(EventSink_PinPointChanged);
            document.EventSink.PinPointChanging += new PinPointChangingEventHandler(EventSink_PinPointChanging);
            document.EventSink.RotationChanged += new RotationChangedEventHandler(EventSink_RotationChanged);
            document.EventSink.RotationChanging += new RotationChangingEventHandler(EventSink_RotationChanging);
            document.EventSink.SizeChanged += new SizeChangedEventHandler(EventSink_SizeChanged);
            document.EventSink.SizeChanging += new SizeChangingEventHandler(EventSink_SizeChanging);
            document.EventSink.LabelsChanged += new CollectionExEventHandler(Document_NodeLabelsChanged);

            document.HistoryManager.UndoCommandStarted += new EventHandler(HistoryManager_CommandStarted);
            document.HistoryManager.UndoCommandCompleted += new EventHandler(HistoryManager_CommandCompleted);
            document.HistoryManager.RedoCommandStarted += new EventHandler(HistoryManager_CommandStarted);
            document.HistoryManager.RedoCommandCompleted += new EventHandler(HistoryManager_CommandCompleted);
            document.HistoryManager.RecordRequest += new EventHandler(HistoryManager_CommandRequest);
            document.HistoryManager.RecordComplete += new EventHandler(HistoryManager_CommandRequestComplete);

            document.LinkManager.SynhronizeStarted += new EventHandler(LinkManager_SynhronizeStarted);
            document.LinkManager.SynhronizeCompleted += new EventHandler(LinkManager_SynhronizeCompleted);

            document.BridgeManager.BridgeGenerationStarted += new EventHandler(BridgeManager_BridgeGenerationStarted);
            document.BridgeManager.BridgeGenerationCompleted += new EventHandler(BridgeManager_BridgeGenerationCompleted);
        }
        private void UnsubscribeFromViewerEvents(IViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");

            viewer.EventSink.NodeCollectionChanged -= new CollectionExEventHandler(Viewer_NodeCollectionChanged);
            viewer.EventSink.OriginChanged -= new ViewOriginEventHandler(View_OriginChanged);
            viewer.EventSink.PropertyChanging -= new PropertyChangingEventHandler(EventSink_PropertyChanging);
            viewer.EventSink.SelectionListChanged -= new CollectionExEventHandler(Viewer_SelectionListChanged);
            viewer.EventSink.NodeDeselected -= new NodeSelectedEventHandler(Viewer_NodeDeselected);
        }
        private void SubscribeForViewerEvents(IViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");

            viewer.EventSink.NodeCollectionChanged += new CollectionExEventHandler(Viewer_NodeCollectionChanged);
            viewer.EventSink.OriginChanged += new ViewOriginEventHandler(View_OriginChanged);
            viewer.EventSink.PropertyChanging += new PropertyChangingEventHandler(EventSink_PropertyChanging);
            viewer.EventSink.SelectionListChanged += new CollectionExEventHandler(Viewer_SelectionListChanged);
            viewer.EventSink.NodeDeselected += new NodeSelectedEventHandler(Viewer_NodeDeselected);
        }
        private void UpdateInvalidRect(Node nodeAffected)
        {
            if (nodeAffected == null)
                throw new ArgumentNullException("nodeAffected");

            RectangleF rectTemp;
            System.Drawing.Rectangle rectUpdating;
            
            // if Model.SizeToContent == true -> update all client area
            if (this.Model.SizeToContent)
            {
                rectTemp = MeasureUnitsConverter.Convert(this.Model.Bounds, this.Model.MeasurementUnits, MeasureUnits.Pixel);

                rectTemp = ConvertFromModelToClientCoordinates(rectTemp);

                // update refresh rect
                rectUpdating = Geometry.ConvertRectangle(rectTemp);
                this.UpdateInfo.UpdateRefreshRect(rectUpdating);
            }

            // get node's bounding rectangle
            rectTemp = RenderingHelper.GetBoundingRectangle(nodeAffected, MeasureUnits.Pixel);

            // if node is in selection list
            // consider handles positions
            if (this.SelectionList.Contains(nodeAffected) || nodeAffected is PseudoGroup)
            {
                RenderingHelper.ConsiderHandles(nodeAffected, ref rectTemp, this.View.Magnification, MeasureUnits.Pixel);
            }

            rectTemp = ConvertFromModelToClientCoordinates(rectTemp);

            // update refresh rect
            rectUpdating = Geometry.ConvertRectangle(rectTemp);
            this.UpdateInfo.UpdateRefreshRect(rectUpdating);
        }
        private void UpdateOnChanged(Node nodeAffected)
        {
            UpdateInvalidRect(nodeAffected);

            if (CanUpdateView && !m_bUndoOrRedo)
            {
                this.Viewer.UpdateView();
            }
        }
        private RectangleF GetSelectedNodesBoundingRect(ICollection nodes)
        {
            RectangleF rectToReturn = RectangleF.Empty;
            Node nodeTemp;

            foreach (object nodeCur in nodes)
            {
                nodeTemp = nodeCur as Node;

                if (nodeTemp != null)
                {
                    RectangleF rectBounding = GetSelectedNodeBoundingRect(nodeTemp);

                    if (rectToReturn.Size.IsEmpty)
                    {
                        rectToReturn = rectBounding;
                    }
                    else
                    {
                        rectToReturn = RectangleF.Union(rectToReturn, rectBounding);
                    }
                }
            }

            rectToReturn = ConvertFromModelToClientCoordinates(rectToReturn);

            return rectToReturn;
        }

        private RectangleF GetSelectedNodeBoundingRect(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("nodeAffected");

            // get node's bounding rectangle
            RectangleF rectToReturn = RenderingHelper.GetBoundingRectangle(node, MeasureUnits.Pixel);
            RenderingHelper.ConsiderHandles(node, ref rectToReturn, this.View.Magnification, MeasureUnits.Pixel);

            return rectToReturn;
        }
        private void ConsiderSelectionListSubstitute(CollectionExEventArgs evtArgs)
        {
            // recreate substituting selection list if EnableSelectionListSubstitute if enabled
            if (this.Model.EnableSelectionListSubstitute)
            {
                if (this.View.SelectionListSubstitute.Count > 0)
                {
                    RemovePseudoGroup();
                }

                if (evtArgs.ChangeType != CollectionExChangeType.Clear && this.SelectionList.Count > 1)
                {
                    UpdateSelectionListSubstitute();
                }
            }
        }
        private void RemovePseudoGroup()
        {
            if (this.View.SelectionListSubstitute.Count > 0)
            {
                // update refresh rect
                RectangleF rectTemp = GetSelectedNodeBoundingRect(this.View.SelectionListSubstitute[0]);
                rectTemp = this.ConvertFromModelToClientCoordinates(rectTemp);
                System.Drawing.Rectangle rectUpdating = Geometry.ConvertRectangle(rectTemp);

                this.UpdateInfo.UpdateRefreshRect(rectUpdating);

                this.View.SelectionListSubstitute[0].UpdateServiceReferences(null);
                this.View.SelectionListSubstitute.Clear();
            }
        }
        private void UpdateSelectionListSubstitute()
        {
            PseudoGroup pgGroup = new PseudoGroup(this.SelectionList);
            EditStyle childrenProtection = new EditStyle();
            HandlesHitTesting.GetSumEditStyle(pgGroup, ref childrenProtection);
            pgGroup.EditStyle.AllowRotate = childrenProtection.AllowRotate;
            pgGroup.UpdateServiceReferences(this.Model);

            this.View.SelectionListSubstitute.Add(pgGroup);

            // update refresh rect
            RectangleF rectTemp = GetSelectedNodeBoundingRect(pgGroup);
            rectTemp = this.ConvertFromModelToClientCoordinates(rectTemp);
            System.Drawing.Rectangle rectUpdating = Geometry.ConvertRectangle(rectTemp);

            this.UpdateInfo.UpdateRefreshRect(rectUpdating);
        }
        private void RecreatePseudoGroup()
        {
            if (m_bRecreatePG && this.Model.EnableSelectionListSubstitute && this.SelectionList.Count > 1)
            {
                RemovePseudoGroup();
                UpdateSelectionListSubstitute();

                m_bRecreatePG = false;
            }
        }

        /// <summary>
        /// Update model size to nodes bounds and minimum model size.
        /// </summary>
        private void UpdateSizeToContent()
        {
            Model model = this.Model;

            // if Boundary constraints is enabled this content don't work
            if (m_bSizeToContentUpdating || model == null || !model.SizeToContent
                || model.LinkManager.IsSynchronizing || model.BridgeManager.Generating)
                return;

            // get model node collection
            NodeCollection modelNodes = model.Nodes;

            // get content bounds
            RectangleF rcNodeBounds;
            RectangleF rcContentBounds = new RectangleF(m_szOriginOffset.ToPointF(), SizeF.Empty);
            RectangleF rcModelBounds = new RectangleF(this.View.Origin, model.DocumentSize.GetSize(this.Model.MeasurementUnits));

            foreach (Node node in modelNodes)
            {
                rcNodeBounds = ((IUnitIndependent)node).GetBoundingRectangle(model.MeasurementUnits, false);
                rcContentBounds = RectangleF.Union(rcContentBounds, rcNodeBounds);
            }

            // save origin offset
            m_szOriginOffset.Width -= rcContentBounds.X;
            m_szOriginOffset.Height -= rcContentBounds.Y;

            // get larger virtual bounds
            SizeF szModelSize = model.MinimumSize;
            szModelSize.Width = Math.Max(rcContentBounds.Width, szModelSize.Width + m_szOriginOffset.Width);
            szModelSize.Height = Math.Max(rcContentBounds.Height, szModelSize.Height + m_szOriginOffset.Height);

            // Add margin to the model
            szModelSize.Width += model.RightMargin;
            szModelSize.Height += model.BottomMargin;

            // if virtual size changed
            if (!rcContentBounds.Location.IsEmpty || szModelSize != rcModelBounds.Size)
            {
                // prepare to update model size and origin
                m_bSizeToContentUpdating = true;
                model.BeginUpdate();
                model.BridgeManager.BeginUpdateIntersection();

                model.EventSink.Pause();
                model.BridgeManager.Pause();
                model.LinkManager.Pause();
                model.HistoryManager.Pause();

                // append offset to origin 
                PointF ptNewOrigin = new PointF(rcModelBounds.X - rcContentBounds.X, rcModelBounds.Y - rcContentBounds.Y);

                // move nodes to origin offset
                TranslateNodes(modelNodes, -rcContentBounds.X, -rcContentBounds.Y);

                // set new model size
                model.DocumentSize.SetSize(szModelSize.Width, szModelSize.Height, this.Model.MeasurementUnits);
                
                // set new origin
                this.View.Origin = ptNewOrigin;

                // complete updating model size and origin
                model.HistoryManager.Resume();
                model.LinkManager.Resume();
                model.BridgeManager.Resume();
                model.EventSink.Resume();

                model.BridgeManager.EndUpdateIntersection();
                model.EndUpdate();

                m_bSizeToContentUpdating = false;

                rcModelBounds.Inflate(5, 5);
                this.UpdateInfo.UpdateRefreshRect(Geometry.ConvertRectangle(rcModelBounds));
            }
        }

        /// <summary>
        /// Translate the nodes collection to offset.
        /// </summary>
        /// <param name="nodesToMove">The nodes to move.</param>
        /// <param name="fX">The offset by X axis.</param>
        /// <param name="fY">The offset by Y axis.</param>
        private void TranslateNodes(NodeCollection nodesToMove, float fX, float fY)
        {
            if (fX == 0 && fY == 0)
                return;

            // translate other nodes
            foreach (Node node in nodesToMove)
            {
                bool bX = node.EditStyle.AllowMoveX;
                bool bY = node.EditStyle.AllowMoveY;

                node.EditStyle.AllowMoveX = true;
                node.EditStyle.AllowMoveY = true;
                node.Translate(fX, fY);

                node.EditStyle.AllowMoveX = bX;
                node.EditStyle.AllowMoveY = bY;
            }
        }
        #endregion

        #region Event handlers
        [EventHandlerPriorityAttribute(true)]
        private void Document_ZOrderChanged(ZOrderChangedEventArgs evtArgs)
        {
            RectangleF rectTemp = RenderingHelper.GetBoundingRectangle(evtArgs.NodeAffected, MeasureUnits.Pixel);
            rectTemp = ConvertFromModelToClientCoordinates(rectTemp);
            System.Drawing.Rectangle rectUpdating = Geometry.ConvertRectangle(rectTemp);

            // update refresh rect
            this.UpdateInfo.UpdateRefreshRect(rectUpdating);

            if (CanUpdateView && !m_bUndoOrRedo)
            {
                this.Viewer.UpdateView();
            }
        }
        [EventHandlerPriorityAttribute(true)]
        private void Document_VertexChanging(VertexChangingEventArgs evtArgs)
        {
            RectangleF rectTemp = RenderingHelper.GetBoundingRectangle(evtArgs.NodeAffected, MeasureUnits.Pixel);
            System.Drawing.Rectangle rectUpdating = Geometry.ConvertRectangle(rectTemp);

            // if vertex container is in SelectionList considet vertex handles
            if (this.SelectionList.Contains(evtArgs.NodeAffected))
            {
                // inflate rect with handle's size
                float fTemp;
                if (HandlesHitTesting.TouchMode)
                    fTemp = CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE / 2;
                else
                    fTemp = CommonUsedValues.RESIZE_HANDLE_SIZE / 2;
                
                rectTemp.Inflate(new SizeF(fTemp, fTemp));
            }

            // update refresh rect
            this.UpdateInfo.UpdateRefreshRect(rectUpdating);
        }
        [EventHandlerPriorityAttribute(true)]
        private void Document_VertexChanged(VertexChangedEventArgs evtArgs)
        {
            RectangleF rectTemp = RenderingHelper.GetBoundingRectangle(evtArgs.NodeAffected, MeasureUnits.Pixel);

            // if vertex container is in SelectionList considet vertex handles
            if (this.SelectionList.Contains(evtArgs.NodeAffected))
            {
                // inflate rect with handle's size
                float fTemp;
                if (HandlesHitTesting.TouchMode)
                    fTemp = CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE / 2;
                else
                    fTemp = CommonUsedValues.RESIZE_HANDLE_SIZE / 2;
                rectTemp.Inflate(new SizeF(fTemp, fTemp));
            }

            System.Drawing.Rectangle rectUpdating = Geometry.ConvertRectangle(rectTemp);

            // update refresh rect
            this.UpdateInfo.UpdateRefreshRect(rectUpdating);

            if (CanUpdateView && !m_bUndoOrRedo)
            {
                this.Viewer.UpdateView();
            }
        }

        /// <summary>
        /// Raises when property is changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PropertyChangingEventArgs"/> instance containing the event data.</param>
        [EventHandlerPriorityAttribute(true)]
        protected virtual void Document_PropertyChanging(PropertyChangingEventArgs evtArgs)
        {
            Node nodeTemp = evtArgs.PropertyContainer as Node;
            Layer layerTemp = evtArgs.PropertyContainer as Layer;

            // if node or layer change visible state to False
            // then update selection list
            if (evtArgs.PropertyName == DPN.Visible && !(bool)evtArgs.NewValue)
            {
                if (layerTemp != null)
                {
                    DeselectNodes(layerTemp);
                }
            }

            if (nodeTemp != null)
            {
                m_nUpdateRequests++;
                UpdateInvalidRect(nodeTemp);
            }
        }
        [EventHandlerPriorityAttribute(true)]
        private void Document_PropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            // get helper references
            Node node = evtArgs.NodeAffected as Node;
            Model model = evtArgs.NodeAffected as Model;
            NodeCollection lstSubstitute = this.View.SelectionListSubstitute;

            // if node affected is Node type
            if (node != null)
            {
                // if node or layer change visible state to False
                // then update selection list
                if (evtArgs.PropertyName == DPN.Visible && !node.Visible)
                {
                    if(this.SelectionList.Contains(node))
                        this.SelectionList.Remove(node);
                }

                m_nUpdateRequests--;

                // get elements bounding rect
                UpdateInvalidRect((Node)evtArgs.NodeAffected);

                // update pseudo group refresh rect
                // if changed node is in selection list
                if (evtArgs.PropertyName.IndexOf(DPN.LineWidth, 0) != -1 && this.Model.EnableSelectionListSubstitute
                    && this.SelectionList.Contains(node) && lstSubstitute.Count > 0)
                {
                    UpdateCallback dg = (UpdateCallback)Delegate.CreateDelegate(typeof(UpdateCallback), lstSubstitute.First, "UpdateRefreshRect");
                    dg();
                }
            }
            else if (model != null)
            {
                // if node affected is Model type
                if (evtArgs.PropertyName == DPN.EnableSelectionListSubstitute)
                {
                    // update selection list substitute if selection list is not empty
                    if (model.EnableSelectionListSubstitute && this.SelectionList.Count > 1)
                    {
                        PseudoGroup pgGroup = new PseudoGroup(this.SelectionList);
                        pgGroup.UpdateServiceReferences(model);
                        lstSubstitute.Add(pgGroup);
                    }
                }
                else if (evtArgs.PropertyName == DPN.SizeToContent
                    || evtArgs.PropertyName == DPN.MinimumSize || evtArgs.PropertyName==DPN.RightMargin || evtArgs.PropertyName==DPN.BottomMargin)
                {
                    // update model bounds to content
                    UpdateSizeToContent();
                }
            }

            if (CanUpdateView)
            {
                this.Viewer.UpdateView();
            }
        }

        /// <summary>
        /// Documents the node collection changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        [EventHandlerPriorityAttribute(true)]
        void Document_NodeCollectionChanging(CollectionExEventArgs evtArgs)
        {
            if (evtArgs.Owner == null)
                throw new ArgumentNullException("collection owner");
            if (evtArgs.ChangeType != CollectionExChangeType.Remove)
            {
                NodeCollection nodes = new NodeCollection(this.SelectionList, false);

                // clear the selection list
                foreach (Node node in nodes)
                    this.SelectionList.Remove(node);
                if (evtArgs.ChangeType == CollectionExChangeType.Clear)
                    this.View.SelectionListSubstitute.Clear();
            }
            else
            {
                foreach (Node node in evtArgs.Elements)
                    if (this.SelectionList.Contains(node))
                        this.SelectionList.Remove(node);
            }           
            this.Viewer.EventSink.RaiseNodesChangingEvent(evtArgs);
        }

        /// <summary>
        /// Cancels the node collection changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        [EventHandlerPriorityAttribute(true)]
        protected void EventSink_CancelCollectionChanged(CollectionExEventArgs evtArgs)
        {
            if (evtArgs.ChangeType == CollectionExChangeType.Insert)
            {
                if (evtArgs.Cancel)
                {
                    foreach (object obj in evtArgs.Elements)
                    {
                        if (obj is Node)
                        {
                            Node node = obj as Node;
                            if (node != null && this.SelectionList.Contains(node))
                                this.SelectionList.Remove(node);
                        }
                    }
                }
            }
            else if (evtArgs.ChangeType == CollectionExChangeType.Remove)
            {
                if (evtArgs.Cancel)
                {
                    foreach (object obj in evtArgs.Elements)
                    {
                        Node node = obj as Node;
                        if (node != null)
                        {
                            node.Parent = this.Model;
                            this.Model.RegenerateUniqueNames();
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Documents the node collection changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        [EventHandlerPriorityAttribute(true)]
        protected virtual void Document_NodeCollectionChanged(CollectionExEventArgs evtArgs)
        {
            if (evtArgs.Owner == null)
                throw new ArgumentNullException("collection owner");

            if ((evtArgs.ChangeType == CollectionExChangeType.Insert || evtArgs.ChangeType == CollectionExChangeType.Set) && evtArgs.Owner is Model)
            {
                // concat current selection list with nodes to update
                NodeCollection nodes = new NodeCollection();
                nodes.AddRange(this.SelectionList);
                nodes.AddRange(evtArgs.Elements);

                // if we are while Undo/Redo operation merge each inserted node into document
                // as there is no Collection.RemoveRange operation
                if (m_bUndoOrRedo)
                {
                    m_nodesHelper.AddRange(evtArgs.Elements);
                }
                else
                {
                    //Raise the viewer NodeCollectionChanged event
                    this.Viewer.EventSink.RaiseNodesChangedEvent(evtArgs);
                    
                    // add new elements to selection
                    foreach (Node node in evtArgs.Elements)
                    {
                        if (node.EditStyle.AllowSelect)
                            this.SelectionList.Add(node);                       
                    }                  
                    if (evtArgs.ChangeType == CollectionExChangeType.Set)
                    {
                        this.UpdateInfo.UpdateRefreshRect(Geometry.ConvertRectangle(this.Model.GetBoundingRect()));
                        this.Viewer.UpdateView();
                    }
                    else
                        this.UpdateInfo.UpdateRefreshRect(Geometry.ConvertRectangle(GetSelectedNodesBoundingRect(evtArgs.Elements)));
                }
                
        
            }
            else if (evtArgs.ChangeType == CollectionExChangeType.Remove || evtArgs.ChangeType == CollectionExChangeType.Clear)
            {
                // get elements bounding rect
                RectangleF rectTemp = GetSelectedNodesBoundingRect(evtArgs.Elements);
                this.UpdateInfo.UpdateRefreshRect(Geometry.ConvertRectangle(rectTemp));
                this.Viewer.EventSink.RaiseNodesChangedEvent(evtArgs);
            }
            else
            {
                // get elements bounding rect
                RectangleF rectTemp = GetSelectedNodesBoundingRect(evtArgs.Elements);
                this.UpdateInfo.UpdateRefreshRect(Geometry.ConvertRectangle(rectTemp));
            }
        }
        [EventHandlerPriorityAttribute(true)]
        private void Viewer_NodeCollectionChanged(CollectionExEventArgs evtArgs)
        {
            if (evtArgs.Owner == null)
                throw new ArgumentNullException("collection owner");

            ConsiderSelectionListSubstitute(evtArgs);

            // get elements bounding rect
            if (evtArgs.Elements.Count > 0)
            {
                if (evtArgs.ChangeType == CollectionExChangeType.Set)
                    this.UpdateInfo.UpdateRefreshRect(Geometry.ConvertRectangle(this.Model.GetBoundingRect()));
                else
                    this.UpdateInfo.UpdateRefreshRect(Geometry.ConvertRectangle(GetSelectedNodesBoundingRect(evtArgs.Elements)));

                if (m_bNeedDocumentRefresh)
                    this.Viewer.UpdateView();
            }
        }

        /// <summary>
        /// Raises when node deselected.
        /// </summary>
        private void Viewer_NodeDeselected(NodeSelectedEventArgs evtArgs)
        {
            NodeCollection nodes = new NodeCollection();
            nodes.Add(evtArgs.Node);
            this.UpdateInfo.UpdateRefreshRect(Geometry.ConvertRectangle(GetSelectedNodesBoundingRect(nodes)));
        }

        /// <summary>
        /// Raises when selection list changed.
        /// </summary>
        [EventHandlerPriorityAttribute(true)]
        private void Viewer_SelectionListChanged(CollectionExEventArgs evtArgs)
        {
            if (evtArgs.Owner == null)
                throw new ArgumentNullException("collection owner");

            ConsiderSelectionListSubstitute(evtArgs);

            if (evtArgs.Elements.Count > 0)
            {
                this.UpdateInfo.UpdateRefreshRect(Geometry.ConvertRectangle(GetSelectedNodesBoundingRect(evtArgs.Elements)));
                if (m_bNeedDocumentRefresh)
                    this.Viewer.UpdateView();
            }
        }

        /// <summary>
        /// Handles the CommandStarted event of the HistoryManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [EventHandlerPriorityAttribute(true)]
        protected virtual void HistoryManager_CommandStarted(object sender, EventArgs e)
        {
            this.Model.BeginUpdate();
            m_nodesHelper = new NodeCollection();
            m_nUpdateRequests++;
            m_bUndoOrRedo = true;
        }

        /// <summary>
        /// Handles the CommandRequest event of the HistoryManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [EventHandlerPriorityAttribute(true)]
        protected virtual void HistoryManager_CommandRequest(object sender, EventArgs e)
        {
            // this.Model.BeginUpdate();
            m_nodesHelper = new NodeCollection();
            m_nUpdateRequests++;
        }

        /// <summary>
        /// Handles the CommandRequestComplete event of the HistoryManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [EventHandlerPriorityAttribute(true)]
        protected virtual void HistoryManager_CommandRequestComplete(object sender, EventArgs e)
        {
            if (!m_bUndoOrRedo)
            {
                RecreatePseudoGroup();
            }

            // this.Model.EndUpdate();
            // update model to content
            UpdateSizeToContent();

            m_nUpdateRequests--;

            if (this.CanUpdateView)
                this.Viewer.UpdateView();
        }
        [EventHandlerPriorityAttribute(true)]
        private void HistoryManager_CommandCompleted(object sender, EventArgs e)
        {
            if (m_nodesHelper != null && m_nodesHelper.Count > 0)
            {
                // if we are while Undo/Redo operation merge each inserted node into document
                // as there is no Collection.RemoveRange operation
                NodeCollection nodes = new NodeCollection();
                nodes.AddRange(m_nodesHelper);
                nodes.AddRange(this.SelectionList);

                // get elements bounding rect
                RectangleF rectTemp = GetSelectedNodesBoundingRect(nodes);
                this.UpdateInfo.UpdateRefreshRect(Geometry.ConvertRectangle(rectTemp));

                for (int i = m_nodesHelper.Count - 1; i >= 0; i--)
                {
                    if (m_nodesHelper[i].Parent == null)
                        m_nodesHelper.RemoveAt(i);
                }

                // Update Selection List
                this.SelectionList.Clear();
                this.SelectionList.AddRange(m_nodesHelper);
            }

            m_bRecreatePG = true;
            RecreatePseudoGroup();

            this.Model.EndUpdate();
            m_nUpdateRequests--;

            // update model to content
            UpdateSizeToContent();

            if (this.CanUpdateView)
            {
                // update viewer
                this.Viewer.UpdateView();
            }

            // reset helper collection
            m_nodesHelper = null;
            m_bUndoOrRedo = false;
        }
        [EventHandlerPriorityAttribute(true)]
        private void Document_BeginUpdate(object sender, EventArgs e)
        {
            m_nUpdateRequests++;
        }
        [EventHandlerPriorityAttribute(true)]
        private void Document_EndUpdate(object sender, EventArgs e)
        {
            m_nUpdateRequests--;

            if (m_nUpdateRequests == 0)
                UpdateSizeToContent();

            if (this.CanUpdateView)
                this.Viewer.UpdateView();
        }

        /// <summary>
        /// Handles the view origin changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewOriginEventArgs"/> instance containing the event data.</param>
        [EventHandlerPriorityAttribute(true)]
        protected virtual void View_OriginChanged(ViewOriginEventArgs evtArgs)
        { 
        }
        [EventHandlerPriorityAttribute(true)]
        private void EventSink_FlipChanging(FlipChangingEventArgs evtArgs)
        {
            UpdateInvalidRect(evtArgs.NodeAffected);
        }
        [EventHandlerPriorityAttribute(true)]
        private void EventSink_FlipChanged(FlipChangedEventArgs evtArgs)
        {
            Node node = evtArgs.NodeAffected;

            if (node != null)
            {
                UpdateOnChanged(node);
            }
        }
        [EventHandlerPriorityAttribute(true)]
        private void EventSink_PinOffsetChanged(PinOffsetChangedEventArgs evtArgs)
        {
            UpdateOnChanged(evtArgs.NodeAffected as Node);
        }
        [EventHandlerPriorityAttribute(true)]
        private void EventSink_PinOffsetChanging(PinOffsetChangingEventArgs evtArgs)
        {
            UpdateInvalidRect(evtArgs.NodeAffected as Node);
        }
        [EventHandlerPriorityAttribute(true)]
        private void EventSink_PinPointChanged(PinPointChangedEventArgs evtArgs)
        {
            Node node = evtArgs.NodeAffected as Node;
            if (node != null)
            {
                UpdateOnChanged(node);
            }
        }
        [EventHandlerPriorityAttribute(true)]
        private void EventSink_PinPointChanging(PinPointChangingEventArgs evtArgs)
        {
            Node node = evtArgs.NodeAffected as Node;
            if (node != null)
            {
                UpdateInvalidRect(node);
            }
        }
        [EventHandlerPriorityAttribute(true)]
        private void EventSink_RotationChanged(RotationChangedEventArgs evtArgs)
        {
            UpdateOnChanged(evtArgs.NodeAffected);
        }
        [EventHandlerPriorityAttribute(true)]
        private void EventSink_RotationChanging(RotationChangingEventArgs evtArgs)
        {
            UpdateInvalidRect(evtArgs.NodeAffected);
        }
        [EventHandlerPriorityAttribute(true)]
        private void EventSink_SizeChanged(SizeChangedEventArgs evtArgs)
        {
            Node node = evtArgs.NodeAffected as Node;
            if (node != null)
            {
                UpdateOnChanged(node);
            }
        }
        [EventHandlerPriorityAttribute(true)]
        private void EventSink_SizeChanging(SizeChangingEventArgs evtArgs)
        {
            Node node = evtArgs.NodeAffected as Node;
            if (node != null)
            {
                UpdateInvalidRect(node);
            }
        }
        [EventHandlerPriorityAttribute(true)]
        private void Document_NodeLabelsChanged(CollectionExEventArgs evtArgs)
        {
            if (evtArgs.Owner == null) throw new ArgumentNullException("evtArgs.Owner");

            Node node = (Node)evtArgs.Owner;

            // update node refresh rect by call update method
            UpdateCallback updateCallback = (UpdateCallback)Delegate.CreateDelegate(
                typeof(UpdateCallback), node, "UpdateRefreshRect");
            updateCallback.Invoke();

            // get elements bounding rect
            UpdateInvalidRect(node);

            if (CanUpdateView)
            {
                this.Viewer.UpdateView();
            }
        }

        /// <summary>
        /// Events the sink property changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PropertyChangingEventArgs"/> instance containing the event data.</param>
        [EventHandlerPriorityAttribute(true)]
        protected virtual void EventSink_PropertyChanging(PropertyChangingEventArgs evtArgs)
        { 
        }
        [EventHandlerPriorityAttribute(true)]
        private void BridgeManager_BridgeGenerationCompleted(object sender, EventArgs e)
        { 
        }
        [EventHandlerPriorityAttribute(true)]
        private void BridgeManager_BridgeGenerationStarted(object sender, EventArgs e)
        {
            // update model to content
            UpdateSizeToContent();
        }
        [EventHandlerPriorityAttribute(true)]
        private void LinkManager_SynhronizeCompleted(object sender, EventArgs e)
        { 
        }
        [EventHandlerPriorityAttribute(true)]
        private void LinkManager_SynhronizeStarted(object sender, EventArgs e)
        {
            // update model to content
            UpdateSizeToContent();
        }
        #endregion
    }
}