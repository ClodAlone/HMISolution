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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Helper class that contains methods to check hit testing and append node transformations.
    /// </summary>
    public class HandlesHitTesting
    {
        #region Class static members
        /// <summary>
        /// Scale factor for handles hit testing.
        /// </summary>
        private static float m_scaleFactor = 1.0f;
        private static bool m_bTouchMode;
        #endregion

        #region Class static properties
        /// <summary>
        /// Gets or sets the scale factor for handles hit testing.
        /// </summary>
        /// <value>The scale factor.</value>
        /// <remarks>
        /// This properties used for checking hit testing on 
        /// handles with given scale factor.
        /// <c> Needed to set before every handle getting. </c>
        /// </remarks>
        public static float ScaleFactor
        {
            get { return m_scaleFactor; }
            set { m_scaleFactor = value; }
        }

        /// <summary>
        /// Gets the regex instance to split node name and index.
        /// </summary>
        /// <value>The regex instance.</value>
        public static Regex NameIndex
        {
            get
            {
                string strRegex = @"([0-9]*$)";

                return new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
        }

        /// <summary>
        /// Gets or sets the value indicating that whether the touch mode is enabled or not.
        /// </summary>
        public static bool TouchMode
        {
            get { return m_bTouchMode; }
            set
            {
                if (value != m_bTouchMode)
                {
                    m_bTouchMode = value;
                }
            }
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Gets the node transformations with all 
        /// parent hierarchy transformations.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>returns the Parent transformation Matrix value.</returns>
        public static Matrix GetNodeParentTransformations(Node node)
        {
            Matrix mtxToReturn = new Matrix();

            if (node != null)
            {
                Node parentNode = node;
                Matrix matrixTemp;

                // iterate through given node parents multiplying their transformations
                while (parentNode != null)
                {
                    matrixTemp = parentNode.GetTransformations();
                    parentNode.AppendFlipTransforms(matrixTemp);

                    mtxToReturn.Multiply(matrixTemp, MatrixOrder.Append);
                    matrixTemp.Reset();

                    parentNode = parentNode.Parent as Node;
                }
            }

            return mtxToReturn;
        }

        /// <summary>
        /// Gets the parents rotation.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>returns the Parents local transformation Matrix value.</returns>
        public static Matrix GetParentsLocalTransformations(Node node)
        {
            if (node == null)
                return new Matrix();

            Matrix matrixToReturn = new Matrix();
            Matrix matrixTemp;
            ICompositeNode nodeParent = node.Parent;
            Node parentNode;

            // iterate through given node parents multiplying their transformations
            while (!(nodeParent is Model))
            {
                parentNode = nodeParent as Node;

                if (parentNode != null)
                {
                    matrixTemp = parentNode.GetLocalTransformations();
                    parentNode.AppendLocalFlipTransforms(matrixTemp);

                    matrixToReturn.Multiply(matrixTemp, MatrixOrder.Append);
                    matrixTemp.Reset();

                    nodeParent = ((Node)nodeParent).Parent;
                }
                else
                    break;
            }

            return matrixToReturn;
        }

        /// <summary>
        /// Gets the parents rotation.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>returns the Parent transformation Matrix value.</returns>
        public static Matrix GetParentsTransformations(Node node)
        {
            return GetParentsTransformations(node, false);
        }

        /// <summary>
        ///  Returns matrix which is an accumulated transformations from parents according to hierarchy, if bWithGivenNode parameter is set to true, node's own matrix will also be included).
        /// </summary>
        /// <param name="node">Node for verification</param>
        /// <param name="bWithGivenNode">if bWithGivenNode parameter is set to true, node's own matrix will also be included. </param>
        /// <returns>returns the Parents transformation Matrix value.</returns>
        public static Matrix GetParentsTransformations(Node node, bool bWithGivenNode)
        {
            Matrix mtxToReturn = new Matrix();

            if (bWithGivenNode)
            {
                mtxToReturn = node.GetTransformations();
                node.AppendFlipTransforms(mtxToReturn);
            }

            if (node != null && node.Parent != null)
            {
                Node parentNode = node.Parent as Node;
                Matrix mtxTemp = GetNodeParentTransformations(parentNode);
                mtxToReturn.Multiply(mtxTemp, MatrixOrder.Append);
            }

            return mtxToReturn;
        }

        /// <summary>
        /// Gets the upper left point of node by pin position and pint offset.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="units">The units.</param>
        /// <returns>Returns the Upper Left Point value</returns>
        public static PointF GetUpperLeftPoint(IUnitIndependent node, MeasureUnits units)
        {
            PointF ptPin = node.GetPinPoint(units);
            SizeF szOffset = node.GetPinPointOffset(units);

            return new PointF(ptPin.X - szOffset.Width, ptPin.Y - szOffset.Height);
        }

        /// <summary>
        /// Gets the upper left point of node by pin position and pint offset.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="units">The units.</param>
        /// <returns>Returns the Upper Left Point value</returns>
        public static PointF GetUpperLeftPoint(Node node, MeasureUnits units)
        {
            return GetUpperLeftPoint((IUnitIndependent)node, units);
        }

        /// <summary>
        /// Indicates whether FlipX is enabled for parent of the given Node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Returns true, if the parents of the given node's FlipX is enabled</returns>
        public static bool GetParentsFlipX(Node node)
        {
            return GetParentsFlipX(node, true);
        }

        /// <summary>
        /// Indicates whether FlipX is enabled for parent of the given Node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="bGivenInclude">if set to <c>true</c> given node will include to result.</param>
        /// <returns>Returns true, if the parent of the given node's FlipX is enabled</returns>
        public static bool GetParentsFlipX(Node node, bool bGivenInclude)
        {
            Node nodeParent = bGivenInclude ? node : node.Parent as Node;
            bool bFlipX = false;

            while (nodeParent != null)
            {
                bFlipX = (bFlipX != nodeParent.FlipX);
                nodeParent = nodeParent.Parent as Node;
            }

            return bFlipX;
        }

        /// <summary>
        /// Indicates whether FlipY is enabled for parent of the given Node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Returns true, if the parents of the given node's FlipY is enabled</returns>
        public static bool GetParentsFlipY(Node node)
        {
            return GetParentsFlipY(node, true);
        }

        /// <summary>
        /// Indicates whether FlipX is enabled for parent of the given Node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="bGivenInclude">if set to <c>true</c> given node will include to result.</param>
        /// <returns>Returns true, if the parent of the given node's FlipX is enabled</returns>
        public static bool GetParentsFlipY(Node node, bool bGivenInclude)
        {
            Node nodeParent = bGivenInclude ? node : node.Parent as Node;
            bool bFlipY = false;

            while (nodeParent != null)
            {
                bFlipY = (bFlipY != nodeParent.FlipY);
                nodeParent = nodeParent.Parent as Node;
            }

            return bFlipY;
        }

        /// <summary>
        /// Appends the parents flip transformations.
        /// </summary>
        /// <param name="mtxMatrix">The matrix to append.</param>
        /// <param name="node">The node.</param>
        public static void AppendParentsFlipTransformations(Matrix mtxMatrix, Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node + parent");

            Node nodeParent = node.Parent as Node;

            // iterate through given node parents multiplying their transformations
            while (nodeParent != null)
            {
                nodeParent.AppendFlipTransforms(mtxMatrix);
                nodeParent = nodeParent.Parent as Node;
            }
        }

        /// <summary>
        /// Get docked the end point location to port location.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="endPoint">The end point.</param>
        /// <returns>The endpoint.</returns>
        public static PointF GetEndPointLocation(ConnectionPoint port, EndPoint endPoint)
        {
            PointF[] pts = new PointF[1] { PointF.Empty };

            if (port != null)
            {
                // append parent transformation from current container
                Matrix mtxTransform = port.Container.GetTransformations();
                port.Container.AppendFlipTransforms(mtxTransform);

                if (port.Container.Parent != null)
                {
                    Matrix mtxParentTransform = GetParentsTransformations(port.Container);
                    mtxTransform.Multiply(mtxParentTransform, MatrixOrder.Append);
                }

                if (endPoint.Container.Parent != null)
                {
                    Matrix mtxEndPoint = GetParentsTransformations(endPoint.Container);
                    mtxEndPoint.Invert();
                    mtxTransform.Multiply(mtxEndPoint, MatrixOrder.Append);
                }

                pts[0] = port.GetPosition();

                // append parent transformations from endPoint Container
                mtxTransform.TransformPoints(pts);
            }

            return pts[0];
        }

        /// <summary>
        /// Gets the port position with parent transformations.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns>The port position.</returns>
        public static PointF GetPortPosition(ConnectionPoint port)
        {
            if (port == null)
                throw new ArgumentNullException("port");

            PointF[] pts = new PointF[1];

            // node port container
            Node nodePC = port.Container;
            pts[0] = port.GetPosition();

            if (nodePC != null)
            {
                // append parent transformation from current container
                Matrix mtxTemp = GetParentsTransformations(nodePC, true);

                // append parent transformations from endPoint Container
                mtxTemp.TransformPoints(pts);
            }

            return pts[0];
        }

        /// <summary>
        /// Gets the parents rotation transform.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The matrix.</returns>
        public static Matrix GetParentsRotationTransform(Node node)
        {
            if (node.Parent == null) return new Matrix();

            if (node == null)
                throw new ArgumentNullException("node + parent");

            MeasureUnits units = MeasureUnits.Pixel;
            Matrix mtxToReturn = new Matrix();
            float fAngle;
            ICompositeNode nodeParent = node.Parent;

            // iterate through given node parents multiplying their transformations
            while (!(nodeParent is Model) && (nodeParent != null))
            {
                Node parentNode = nodeParent as Node;

                if (nodeParent != null)
                {
                    PointF ptPinPoint = ((IUnitIndependent)parentNode).GetPinPoint(units);
                    fAngle = parentNode.RotationAngle;
                    mtxToReturn.RotateAt(fAngle, ptPinPoint, MatrixOrder.Append);

                    nodeParent = ((Node)nodeParent).Parent;
                }
            }

            return mtxToReturn;
        }

        /// <summary>
        /// Gets the parents rotation.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The parents rotation value.</returns>
        public static float GetParentsRotation(Node node)
        {
            if (node.Parent == null) return 0;

            if (node == null)
                throw new ArgumentNullException("node + parent");

            float fRotationAngleToReturn = 0;
            float fAngle;
            ICompositeNode nodeParent = node.Parent;

            // iterate through given node parents multiplying their transformations
            while (!(nodeParent is Model) && (nodeParent != null))
            {
                Node parentNode = nodeParent as Node;

                if (nodeParent != null)
                {
                    fAngle = parentNode.RotationAngle;
                    fRotationAngleToReturn += fAngle;

                    nodeParent = ((Node)nodeParent).Parent;
                }
            }

            return fRotationAngleToReturn;
        }

        /// <summary>
        /// Convert nodes from model to parent node coordinates.
        /// </summary>
        /// <param name="node">The node to covert.</param>
        /// <param name="parent">The node parent.</param>
        public static void ConvertToParentCoordinates(Node node, Node parent)
        {
            NodeInfo nodeInfo = new NodeInfo(node, MeasureUnits.Pixel);
            ConvertToParentCoordinates(node, parent, ref nodeInfo);
            nodeInfo.AppendChanges(node);
        }

        /// <summary>
        /// Convert from parent node to model coordinates.
        /// </summary>
        /// <param name="node">The node to convert.</param>
        /// <param name="parent">The node parent.</param>
        public static void ConvertToModelCoordinates(Node node, Node parent)
        {
            NodeInfo nodeInfo = new NodeInfo(node, MeasureUnits.Pixel);
            ConvertToModelCoordinates(node, parent, ref nodeInfo);
            nodeInfo.AppendChanges(node);
        }

        /// <summary>
        /// Determines whether the specified given parent node is a child parent.
        /// </summary>
        /// <param name="child">The child node.</param>
        /// <param name="parent">The parent node.</param>
        /// <returns>
        /// <c>true</c> if the specified parent is a child parent; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsParent(Node child, Node parent)
        {
            bool bSuccess = false;

            if (child != null)
            {
                Node childParent = child.Parent as Node;

                // get next parent node
                while (childParent != null)
                {
                    if (childParent.Equals(parent))
                    {
                        bSuccess = true;
                        break;
                    }
                    else
                    {
                        childParent = childParent.Parent as Node;
                    }
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Get all nodes at given point.
        /// </summary>
        /// <param name="compositeNode">The parent.</param>
        /// <param name="ptPoint">The point to check .</param>
        /// <param name="bOnlyCanSelected">if set to <c>true</c> result collection contain only nodes  what can be selected.</param>
        /// <param name="bGroupFirst">if set to <c>true</c> group will add before it children.</param>
        /// <returns>The node collection.</returns>
        public static NodeCollection GetAllNodesAtPoint(ICompositeNode compositeNode, PointF ptPoint, bool bOnlyCanSelected, bool bGroupFirst)
        {
            NodeCollection nodesToReturn = new NodeCollection();
            Node parent = compositeNode as Node;

            // if current node can't be select then return empty collection
            if (bOnlyCanSelected && parent != null && !CanSelectNode(parent))
                return nodesToReturn;

            PointF ptLocation;
            ICompositeNode container;
            Node nodeCur;

            // iterate through nodes performing hit test
            for (int i = 0, nLength = compositeNode.ChildCount; i < nLength; i++)
            {
                nodeCur = compositeNode.GetChild(i);

                // check if node can be selected
                if ((bOnlyCanSelected && !CanSelectNode(nodeCur)) || !IsVisible(nodeCur))
                    continue;

                ptLocation = GetLocalPoint(nodeCur.Parent as Node, ptPoint);

                // check if node contain given point
                if (nodeCur.ContainsPoint(ptLocation))
                {
                    container = nodeCur as ICompositeNode;

                    // if node is composite then check for contains point his children
                    if (container != null)
                    {
                        NodeCollection childNodes = GetAllNodesAtPoint(container, ptPoint, bOnlyCanSelected, bGroupFirst);
                        childNodes.AddRange(nodesToReturn);
                        nodesToReturn = childNodes;
                    }

                    // add parent after/before it children
                    if (!bGroupFirst && container != null)
                    {
                        nodesToReturn.Add(nodeCur);
                    }
                    else
                    {
                        nodesToReturn.Insert(0, nodeCur);
                    }
                }
            }

            return nodesToReturn;
        }

        /// <summary>
        /// Check if node can select.
        /// </summary>
        /// <param name="node">Node to check.</param>
        /// <returns>
        /// <b>True</b> if node can be selected, otherwise - <b>false</b>.
        /// </returns>
        public static bool CanSelectNode(Node node)
        {
            return (node != null && IsVisible(node) && node.EditStyle.AllowSelect);
        }

        /// <summary>
        /// Check if node visible in diagram.
        /// </summary>
        /// <param name="node">Node to check.</param>
        /// <returns><b>True</b> if node is visible, otherwise - <b>false</b>. </returns>
        public static bool IsVisible(Node node)
        {
            bool bSuccess = true;

            while (bSuccess && node != null)
            {
                bSuccess = node.Visible;
                node = node.Parent as Node;
            }

            return bSuccess;
        }

        /// <summary>
        /// Saves the connections.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>The connections list.</returns>
        public static ArrayList SaveConnections(NodeCollection nodes)
        {
            // save port connection before remove node
            ArrayList portConnections = new ArrayList();
            ArrayList containerConnections;
            EndPointConnection connection;
            ICompositeNode container;

            Model model = null;
            bool bLineRouting = false;

            foreach (Node node in nodes)
            {
                container = node as ICompositeNode;

                if (model == null)
                    model = node.Root;

                if (model != null)
                {
                    // save and disable connector routing
                    bLineRouting = model.LineRoutingEnabled;
                    model.LineRoutingEnabled = false;
                }

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

                    containerConnections = SaveConnections(containerNodes);

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

                        portConnections.Add(connection);
                        port.DisconnectAll();
                    }
                }

                if (model != null)
                {
                    // restore previous state of lineRouting mode
                    model.LineRoutingEnabled = bLineRouting;
                }
            }

            return portConnections;
        }

        /// <summary>
        /// Restores the connections.
        /// </summary>
        /// <param name="portConnections">The port connections.</param>
        public static void RestoreConnections(ArrayList portConnections)
        {
            // reconnect disconnecter connections
            foreach (EndPointConnection portConnection in portConnections)
            {
                foreach (EndPoint endPoint in portConnection.EndPoints)
                {
                    Group group = portConnection.Port.Container as Group;

                    if (group != null)
                        group.LockUpdate = true;

                    portConnection.Port.Connect(endPoint);

                    if (group != null)
                        group.LockUpdate = false;
                }
            }
        }

        /// <summary>
        /// Determines whether composite node can resize children in one dimension.
        /// </summary>
        /// <param name="compositeNode">The composite node to check.</param>
        /// <returns>
        /// <c>true</c> if given composite can resize children in one dimension; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanResizeChildren(ICompositeNode compositeNode)
        {
            if (compositeNode == null)
                return false;

            bool bCanResize = true;

            for (int i = 0, length = compositeNode.ChildCount; i < length && bCanResize; i++)
            {
                Node node = compositeNode.GetChild(i);
                ICompositeNode composite = node as ICompositeNode;

                // look in children
                if (composite != null && !CanResizeChildren(composite))
                {
                    bCanResize = false;
                    break;
                }

                // check if node turn off aspect ratio option
                bCanResize = !node.EditStyle.AspectRatio;

                //if (bCanResize)
                //{
                    // check if node at right angle
                    //bCanResize = (node.RotationAngle % (CommonUsedValues.CIRCLE / 4) == 0);
                //}
            }

            return bCanResize;
        }

        /// <summary>
        /// Gets the union of children edit styles.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="editStyle">The edit style.</param>
        public static void GetSumEditStyle(Node node, ref EditStyle editStyle)
        {
            ICompositeNode group = node as PseudoGroup;

            if (!node.EditStyle.AllowChangeWidth)
                editStyle.AllowChangeWidth = false;

            if (!node.EditStyle.AllowChangeHeight)
                editStyle.AllowChangeHeight = false;

            if (!node.EditStyle.AllowMoveX)
                editStyle.AllowMoveX = false;

            if (!node.EditStyle.AllowMoveY)
                editStyle.AllowMoveY = false;

            if (!node.EditStyle.AllowRotate)
                editStyle.AllowRotate = false;

            if (group != null)
            {
                for (int i = 0, length = group.ChildCount; i < length; i++)
                {
                    node = group.GetChild(i);
                    GetSumEditStyle(node, ref editStyle);
                }
            }
        }

        /// <summary>
        /// Append the node scale transformation.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="mtxTemp">The MTX temp.</param>
        /// <param name="bSkipGivenNode">if set to <c>true</c> [b skip given node].</param>
        public static void AppendScaleTransforms(Node node, Matrix mtxTemp, bool bSkipGivenNode)
        {
            if (node == null)
                return;

            Node parent = node.Parent as Node;
            Model model = node.Root;
            Matrix mtxScale = bSkipGivenNode ? new Matrix() : node.NodeScale.GetScaleTransformation(node.MeasurementUnit);

            while (parent != null)
            {
                mtxScale.Multiply(parent.NodeScale.GetScaleTransformation(node.MeasurementUnit));
                parent = parent.Parent as Node;
            }

            if (model != null)
                mtxScale.Multiply(model.DocumentScale.GetScaleTransformation(node.MeasurementUnit));

            mtxTemp.Multiply(mtxScale);
        }

        /// <summary>
        /// Gets the node graphics path refresh rectangle include shadow, line width, decorators, labels and ports.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The refresh rect.</returns>
        public static RectangleF GetRefreshRectangle(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("Node parameter value can't be null.");

            PathNode pathNode = node as PathNode;
            IEndPointContainer endPointContainer = node as IEndPointContainer;
            LineBase line = node as LineBase;
            ICompositeNode compositeNode = node as ICompositeNode;
            Group group = node as Group;

            // GRAPHICS PATH BOUNDS
            Matrix mtxTransform = GetParentsTransformations(node, true);
            //RectangleF rcRefresh = GetGraphisPathBounds(node.GraphicsPath, mtxTransform);
			 RectangleF rcRefresh = new RectangleF();
            if (node.GraphicsPath.PointCount > 0)
            {
                rcRefresh= GetGraphisPathBounds(node.GraphicsPath, mtxTransform);
            }
            // LINE WIDTH
            float fWidth = node.LineStyle.LineWidth;
            rcRefresh = Geometry.WidenRect(rcRefresh, fWidth);

            // CONNECTION POINTS
            foreach (ConnectionPoint port in node.Ports)
            {
                if (port is CentralPort)
                    continue;

                RectangleF rcPBounds = Geometry.CreateRect(port.GetPosition(), port.GraphicsPath.GetBounds().Size);
                rcPBounds = Geometry.AppendMatrix(rcPBounds, mtxTransform);
                rcRefresh = RectangleF.Union(rcRefresh, rcPBounds);
            }

            // NODE's SHADOW
            if (node.ShadowStyle.Visible)
            {
                RectangleF rcShadow = node.GraphicsPath.GetBounds(mtxTransform);
                rcShadow.Offset(node.ShadowStyle.OffsetX, node.ShadowStyle.OffsetY);

                // merge shadow rect with current refresh rect
                rcRefresh = RectangleF.Union(rcRefresh, rcShadow);
            }

            // LABELS
            LabelCollection labels = (pathNode != null) ? pathNode.Labels : (group != null) ? group.Labels : null;

            if (labels != null)
            {
                RectangleF rcLBounds = RectangleF.Empty;

                // current label's bounding rect
                foreach (Label label in labels)
                {
                    if (label == null) continue;

                    rcLBounds.Size = label.Size;
                    // get label location
                    // label location
                    PointF ptLocation = label.GetPosition();
                    // update label location
                    if (label.UpdatePosition == false)
                    {
                        switch (label.Position)
                        {
                            case Position.BottomCenter:
                                ptLocation = new PointF(ptLocation.X - rcLBounds.Size.Width / 2, ptLocation.Y);
                                 break;
                            case Position.Center:
                                 ptLocation = new PointF(ptLocation.X - rcLBounds.Size.Width / 2, ptLocation.Y - rcLBounds.Size.Height / 2);
                                 break;
                            case Position.TopCenter:
                                 ptLocation = new PointF(ptLocation.X - rcLBounds.Size.Width / 2, ptLocation.Y - rcLBounds.Size.Height);
                                 break;
                            case Position.TopLeft:
                                 ptLocation = new PointF(ptLocation.X, ptLocation.Y - rcLBounds.Size.Height);
                                 break;
                            case Position.MiddleLeft:
                                 ptLocation = new PointF(ptLocation.X - rcLBounds.Size.Width, ptLocation.Y - rcLBounds.Size.Height / 2);
                                 break;
                            case Position.MiddleRight:
                                 ptLocation = new PointF(ptLocation.X, ptLocation.Y - rcLBounds.Size.Height / 2);
                                 break;
                            case Position.TopRight:
                                 ptLocation = new PointF(ptLocation.X - rcLBounds.Size.Width, ptLocation.Y - rcLBounds.Size.Height);
                                 break;
                            case Position.BottomRight:
                                 ptLocation = new PointF(ptLocation.X - rcLBounds.Size.Width, ptLocation.Y);
                                 break;
                        }
                    }
                    rcLBounds.Location = ptLocation;
                    rcLBounds = Geometry.AppendMatrix(rcLBounds, mtxTransform);

                    // merge port bounding rect with current refresh rect
                    rcRefresh = RectangleF.Union(rcRefresh, rcLBounds);
                }
            }

            // current decorator transformations
            if (line != null)
            {
                Matrix mtxTemp;
                Decorator dcHead = line.HeadDecorator;
                Decorator dcTail = line.TailDecorator;

                // HEAD DECORATOR
                if (dcHead.DecoratorShape != DecoratorShape.None && dcHead.GraphicsPath != null  && dcHead.GraphicsPath.PointCount>0)
                {
                    mtxTemp = GetHeadDecoratorTransformations(line);
                    mtxTemp.Multiply(mtxTransform, MatrixOrder.Append);
                    rcRefresh = RectangleF.Union(rcRefresh, GetGraphisPathBounds(dcHead.GraphicsPath, mtxTemp));
                }

                // TAIL DECORATOR
                if (dcTail.DecoratorShape != DecoratorShape.None && dcTail.GraphicsPath != null && dcTail.GraphicsPath.PointCount>0)
                {
                    mtxTemp = GetTailDecoratorTransformations(line);
                    mtxTemp.Multiply(mtxTransform, MatrixOrder.Append);
                    rcRefresh = RectangleF.Union(rcRefresh, GetGraphisPathBounds(dcTail.GraphicsPath, mtxTemp));
                }
            }

            if (compositeNode != null)
            {
                RectangleF rcChildRect;

                for (int i = 0, length = compositeNode.ChildCount; i < length; i++)
                {
                    rcChildRect = GetRefreshRectangle(compositeNode.GetChild(i));

                    if (!rcChildRect.IsEmpty)
                        rcRefresh = RectangleF.Union(rcRefresh, rcChildRect);
                }
            }

            return rcRefresh;
        }

        /// <summary>
        /// Split the name of the node to two part where
        /// first is a rootName and second is name index.
        /// </summary>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="regex">The Regex type instance.</param>
        /// <returns>
        /// Array of string what contains two elements, rootName and index
        /// </returns>
        public static string[] SplitNodeName(string nodeName, Regex regex)
        {
            string[] nameToReturn = new string[] { nodeName, string.Empty };

            if (regex.IsMatch(nodeName))
            {
                string[] strsName = regex.Split(nodeName);
                Array.Copy(strsName, 0, nameToReturn, 0, 2);
            }

            return nameToReturn;
        }

        /// <summary>
        /// Gets the skipped indexes in indexes array.
        /// </summary>
        /// <param name="indexes">The indexes array.</param>
        /// <param name="nNewIdxLength">Length of the new index.</param>
        /// <returns>Array with skipped indexes.&gt;</returns>
        public static Int64[] GetSkippedIndexes(Int64[] indexes, Int64 nNewIdxLength)
        {
            bool bSuccess = false;

            // copy list indexes
            Int64[] lstIndexes = (Int64[])indexes.Clone();

            // list with indexes what skipped in given list
            ArrayList lstToReturn = new ArrayList();

            // perform ascending sort
            Array.Sort(lstIndexes);

            int nLength = lstIndexes.Length;
            int nCounter = 0;

            // current index
            Int64 nCurIdx = 0;

            if (lstIndexes.Length > 0)
            {
                Int64 nPrevIdx = 0;

                while (nLength > nCounter && !bSuccess)
                {
                    // get current index
                    nCurIdx = lstIndexes[nCounter];

                    // fill range between two idxs
                    while (nCurIdx > nPrevIdx + 1 && !bSuccess)
                    {
                        nPrevIdx++;
                        lstToReturn.Add(nPrevIdx);

                        bSuccess = (nNewIdxLength != 0 && lstToReturn.Count == nNewIdxLength);
                    }

                    nPrevIdx = nCurIdx;
                    nCounter++;
                }
            }

            // if requred idxs count is more than current add more idxs
            if (nNewIdxLength > lstToReturn.Count)
            {
                while (nNewIdxLength > lstToReturn.Count)
                {
                    lstToReturn.Add(++nCurIdx);
                    nCounter++;
                }
            }

            return (Int64[])lstToReturn.ToArray(typeof(Int64));
        }

        /// <summary>
        /// Evaluates whether new index for given node name should be generated.
        /// </summary>
        /// <param name="nameTable">The name table hash where key - node name, 
        /// value list of indexes.</param>
        /// <param name="strNodeName">Node name.</param>
        /// <param name="regex">The Regex type instance.</param>
        /// <returns>
        /// true- new index should be generated, otherwise no.
        /// </returns>
        public static bool ShouldGenerateNewIndex(Hashtable nameTable, string strNodeName, Regex regex)
        {
            if (nameTable == null)
                throw new ArgumentNullException("nameTable entry can't be null");

            bool bSuccess = false;
            string[] strTmp = SplitNodeName(strNodeName, regex);
            string strRootName = strTmp[0];

            Int64 nNodeIdx = 0;

            // new index should be generated only
            // if node name is already present in NameHash and its index is occupied
            if (nameTable.ContainsKey(strRootName))
            {
                // get node index
                if (strTmp[1] != string.Empty)
                {
                    Int64.TryParse(strTmp[1], out nNodeIdx);
                    ArrayList lstIdx = nameTable[strRootName] as ArrayList;

                    // new index should be generated
                    bSuccess = !(nNodeIdx > -1 && !lstIdx.Contains(nNodeIdx));
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Called to generate a unique name when inserting a new node.
        /// </summary>
        /// <param name="nameTable">The name table hash where key - node name, 
        /// value list of indexes.</param>
        /// <param name="obj">Node to generate unique name for.</param>
        /// <param name="nodeName">Node name generated.</param>
        /// <param name="regex">Regex used to split node name.</param>
        /// <returns>
        /// True if a new name was generated; False if the name is already unique.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The implementation of this method first checks to see if the name is
        /// already unique. If it is, then it returns False to the caller and
        /// the nodeName parameter contains the original name. If the node's name
        /// is not unique within the model, then this method adds a numeric suffix
        /// to the name and continues to increment it in a loop until the name
        /// is unique. If the nodeName output parameter contains a value other
        /// than the original node name, then this method returns True.
        /// </para>
        /// <para>
        /// This method can be overriden in derived classes in order to customize or
        /// replace the algorithm for generating unique names.
        /// </para>
        /// </remarks>
        public static bool GenerateUniqueNodeName(Hashtable nameTable, Node obj, out string nodeName, Regex regex)
        {
            nodeName = obj.Name;
            bool nameChanged = false;

            if (nodeName == null || nodeName == string.Empty)
            {
                nodeName = obj.GetType().Name + "1";
                nameChanged = true;
            }

            // if index is not in associated indexes -> generate new otherwise skip
            if (ShouldGenerateNewIndex(nameTable, nodeName, regex))
            {
                string[] strTmp = SplitNodeName(nodeName, regex);
                nodeName = strTmp[0];

                if (!nameTable.ContainsKey(nodeName))
                {
                    nameTable[nodeName] = new ArrayList();
                }

                ArrayList list = (ArrayList)nameTable[nodeName];

                if (list.Count > 0)
                {
                    Int64[] indexes = (Int64[])list.ToArray(typeof(Int64));
                    indexes = GetSkippedIndexes(indexes, 1);

                    if (indexes.Length == 0)
                        indexes = new Int64[] { indexes[indexes.Length - 1] + 1 };

                    if (indexes[0] != 0)
                    {
                        nodeName += indexes[0].ToString();
                    }

                    nameChanged = true;
                }
            }

            return nameChanged;
        }

        /// <summary>
        /// Register the name of the node to name table.
        /// </summary>
        /// <param name="nameTable">The name table hash where key - node name, 
        /// value list of indexes.</param>
        /// <param name="node">Node to register.</param>
        /// <param name="regex">The Regex type instance.</param>
        public static void RegisterNode(Hashtable nameTable, Node node, Regex regex)
        {
            if (nameTable == null)
                throw new ArgumentNullException("nameTable entry can't be null");

            string[] arrName = SplitNodeName(node.Name, regex);

            if (arrName.Length > 0)
            {
                string strRootName = arrName[0];

                if (nameTable[strRootName] == null)
                {
                    nameTable[strRootName] = new ArrayList();
                }

                if (arrName.Length > 1)
                {
                    ArrayList lstIdx = (ArrayList)nameTable[strRootName];
                    Int64 nNodeIndex = 0;
                    Int64 nNumber;

                    if (arrName[1] != string.Empty && Int64.TryParse(arrName[1], out nNumber))
                        nNodeIndex = nNumber;

                    if (lstIdx.Contains(nNodeIndex))
                    {
                        Int64[] idxs = (Int64[])lstIdx.ToArray(typeof(Int64));

                        // generate new index
                        Int64[] idxsNew = GetSkippedIndexes(idxs, 1);
                        lstIdx.Add(idxsNew[0]);

                        // assing new node name
                        node.Name = (idxsNew[0] != 0) ? strRootName + idxsNew[0] : strRootName;
                    }
                    else
                    {
                        lstIdx.Add(nNodeIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Unregister the name of the node form name table.
        /// </summary>
        /// <param name="nameTable">The name table hash where key - node name, 
        /// value list of indexes.</param>
        /// <param name="node">Node to unregister.</param>
        /// <param name="regex">The Regex type instance.</param>
        public static void UnregisterNode(Hashtable nameTable, Node node, Regex regex)
        {
            if (nameTable == null)
                throw new ArgumentNullException("nameTable entry can't be null");

            string[] arrName = SplitNodeName(node.Name, regex);

            if (arrName.Length > 1)
            {
                string strRootName = arrName[0];
                Int64 nNodeIndex = 0;
                Int64 nNumber;

                if (arrName[1] != string.Empty && Int64.TryParse(arrName[1], out nNumber))
                    nNodeIndex = nNumber;

                if (nameTable.ContainsKey(strRootName))
                {
                    ((ArrayList)nameTable[strRootName]).Remove(nNodeIndex);
                }
            }
        }
        #endregion

        #region Class utility methods

        /// <summary>
        ///  Returns topmost node and segment index in it, that contains/intersects with ptTesting point.  
        /// </summary>
        /// <param name="nodes">Nodes collection for verification.</param>
        /// <param name="ptTesting">Search point of the segment. </param>
        /// <param name="nSegmentID">Index of segment that contains / intersects with ptTesting point.</param>
        /// <returns>node to which the segment belongs.</returns>
        public static PathNode GetSegmentLineAtPoint(NodeCollection nodes, Point ptTesting, ref int nSegmentID)
        {
            return GetSegmentLineAtPoint(nodes, new PointF(ptTesting.X, ptTesting.Y), ref nSegmentID);
        }

        /// <summary>
        ///  Returns topmost node and segment index in it, that contains/intersects with ptTesting point.  
        /// </summary>
        /// <param name="nodes">Nodes collection for verification.</param>
        /// <param name="ptTesting">Search point of the segment. </param>
        /// <param name="nSegmentID">Index of segment that contains / intersects with ptTesting point.</param>
        /// <returns>node to which the segment belongs.</returns>
        public static PathNode GetSegmentLineAtPoint(NodeCollection nodes, PointF ptTesting, ref int nSegmentID)
        {
            PathNode nodeHit = null;

            foreach (Node nodeCurrent in nodes)
            {
                if (nodeCurrent.Visible && GetSegmentLineAtPoint(nodeCurrent, ptTesting, ref nSegmentID))
                {
                    nodeHit = (PathNode)nodeCurrent;
                    break;
                }
            }

            return nodeHit;
        }

        /// <summary>
        /// Returns topmost node and segment index in it, that contains/intersects with ptTesting point.
        /// </summary>
        /// <param name="node">Node for verification.</param>
        /// <param name="ptTesting">Search point of the segment.</param>
        /// <param name="nSegmentID">Index of segment that contains / intersects with ptTesting point.</param>
        /// <returns>node to which the segment belongs.</returns>
        public static bool GetSegmentLineAtPoint(Node node, PointF ptTesting, ref int nSegmentID)
        {
            if (!node.Visible) return false;

            bool nodeHit = false;

            // Get's path nodes only.
            PathNode pathNode = node as PathNode;

            if (pathNode != null && node.EditStyle.DefaultHandleEditMode == HandleEditMode.Vertex)
            {
                nSegmentID = pathNode.GetLineSegmentAtPoint(ptTesting);

                nodeHit = (nSegmentID >= 0);
            }

            return nodeHit;
        }

        /// <summary>
        /// Returns topmost node and segment index in it, that contains/intersects with ptTesting point.
        /// </summary>
        /// <param name="node">Node for verification.</param>
        /// <param name="ptTesting">Search point of the segment.</param>
        /// <param name="nSegmentID">Index of segment that contains / intersects with ptTesting point.</param>
        /// <returns>node to which the segment belongs.</returns>
        public static bool GetSegmentLineAtPoint(Node node, Point ptTesting, ref int nSegmentID)
        {
            return GetSegmentLineAtPoint(node, new PointF(ptTesting.X, ptTesting.Y), ref nSegmentID);
        }

        /// <summary>
        /// Returns topmost node from the nodes collection and index of vertex, that contains/intersects with the testing point.
        /// </summary>
        /// <param name="nodes">Nodes collection for verification.</param>
        /// <param name="ptTesting">Search point of the vertex.</param>
        /// <param name="nVertexID">Index of vertex that contains/intersects with ptTesting point.</param>
        /// <returns>The path node.</returns>
        public static PathNode GetVertexHandleAtPoint(NodeCollection nodes, Point ptTesting, ref int nVertexID)
        {
            return GetVertexHandleAtPoint(nodes, new PointF(ptTesting.X, ptTesting.Y), ref nVertexID);
        }

        /// <summary>
        /// Returns topmost node from the nodes collection and index of vertex, that contains/intersects with the testing point.
        /// </summary>
        /// <param name="nodes">Nodes collection for verification.</param>
        /// <param name="ptTesting">Search point of the vertex.</param>
        /// <param name="nVertexID">Index of vertex that contains/intersects with ptTesting point.</param>
        /// <returns>The path node.</returns>
        public static PathNode GetVertexHandleAtPoint(NodeCollection nodes, PointF ptTesting, ref int nVertexID)
        {
            PathNode nodeHit = null;

            foreach (Node nodeCurrent in nodes)
            {
                if (nodeCurrent.Visible && GetVertexHandleAtPoint(nodeCurrent, ptTesting, ref nVertexID))
                {
                    nodeHit = (PathNode)nodeCurrent;
                    break;
                }
            }

            return nodeHit;
        }

        /// <summary>
        ///  Shows whether there is at least one vertex containing / intersecting with ptTesting point.
        /// </summary>
        /// <param name="node">Vertex container.</param>
        /// <param name="ptTesting">Search point of the vertex.</param>
        /// <param name="nVertexID">Index of vertex that contains / intersects with ptTesting point.</param>
        /// <returns>The vertex handle.</returns>
        public static bool GetVertexHandleAtPoint(Node node, PointF ptTesting, ref int nVertexID)
        {
            if (!node.Visible) return false;

            bool nodeHit = false;
            float handleWidth;
            Model model = node.Parent as Model;
            if (HandlesHitTesting.TouchMode)
            {
                m_scaleFactor = m_scaleFactor / 2;
                handleWidth = CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE / m_scaleFactor;
            }
            else
                handleWidth = CommonUsedValues.RESIZE_HANDLE_SIZE / m_scaleFactor;

            // Get's path nodes only.
            PathNode pathNode = node as PathNode;

            if (pathNode != null && node.EditStyle.DefaultHandleEditMode == HandleEditMode.Vertex)
            {
                //// Create shapes trasform matrix - Without flips
                Matrix matrix = node.GetTransformations();
                //// append flips
                node.AppendFlipTransforms(matrix);
                //// get parent transformations
                Matrix mtxParent = GetParentsTransformations(node);
                //// append parent transformations
                matrix.Multiply(mtxParent, MatrixOrder.Append);
                //// Get nodes vertex points.
                PointF[] pathPoints = pathNode.GetPoints();
                //// Transform vertex points.
                matrix.TransformPoints(pathPoints);

                PointF ptHandleLocation;

                for (int i = 0, len = pathPoints.Length; i < len; i++)
                {
                    // 1 - Get current vertex location.
                    ptHandleLocation = pathPoints[i];
                    ptHandleLocation = new PointF(ptHandleLocation.X - 10, ptHandleLocation.Y - 10);
                    RectangleF rectBounds = Geometry.CreateRect(ptHandleLocation, new SizeF(handleWidth, handleWidth));
                    rectBounds.Inflate(handleWidth / 2, handleWidth / 2);
                    // 3 - Check point visibility
                    if (rectBounds.Contains(ptTesting))
                    {
                        nodeHit = true;
                        nVertexID = i;
                        break;
                    }
                }
            }

            return nodeHit;
        }

        /// <summary>
        ///  Shows whether there is at least one vertex containing / intersecting with ptTesting point.
        /// </summary>
        /// <param name="node">Vertex container.</param>
        /// <param name="ptTesting">Search point of the vertex.</param>
        /// <param name="nVertexID">Index of vertex that contains / intersects with ptTesting point.</param>
        /// <returns>true, if get vertex handle.</returns>
        public static bool GetVertexHandleAtPoint(Node node, Point ptTesting, ref int nVertexID)
        {
            return GetVertexHandleAtPoint(node, new PointF(ptTesting.X, ptTesting.Y), ref nVertexID);
        }

        /// <summary>
        ///  Returns node and ResizeHandle position that is found in ptTesting point from nodes collection.
        /// </summary>
        /// <param name="nodes">Collection for verification.</param>
        /// <param name="ptTesting"> Search point of ResizeHandle.</param>
        /// <param name="handleHit">Handle position in ptTesting point. </param>
        /// <returns>The node and resize handle position at the given point.</returns>
        public static Node GetResizeHandleAtPoint(NodeCollection nodes, PointF ptTesting, ref BoxPosition handleHit)
        {
            Node nodeHit = null;

            foreach (Node nodeCurrent in nodes)
            {
                if (nodeCurrent.Visible && nodeCurrent.ShowResizeHandles())
                {
                    if (GetResizeHandleAtPoint(nodeCurrent, ptTesting, ref handleHit))
                    {
                        nodeHit = nodeCurrent;
                        break;
                    }
                }
            }

            return nodeHit;
        }

        /// <summary>
        ///  Returns node and ResizeHandle position that is found in ptTesting point from nodes collection.
        /// </summary>
        /// <param name="nodes">Collection for verification.</param>
        /// <param name="ptTesting"> Search point of ResizeHandle.</param>
        /// <param name="handleHit">Handle position in ptTesting point. </param>
        /// <returns>The resize handle at the specified point.</returns>
        public static Node GetResizeHandleAtPoint(NodeCollection nodes, Point ptTesting, ref BoxPosition handleHit)
        {
            return GetResizeHandleAtPoint(nodes, new PointF(ptTesting.X, ptTesting.Y), ref handleHit);
        }

        /// <summary>
        ///  Shows whether there is at least one handle in the  ptTesting point that belongs to the  node.
        /// </summary>
        /// <param name="node">Handles owner for verification.</param>
        /// <param name="ptTesting">Point of  possible location of the ResizeHandle.</param>
        /// <param name="handleHit">Handle position in ptTesting point.</param>
        /// <returns>true, if get the resize handle.</returns>
        public static bool GetResizeHandleAtPoint(Node node, PointF ptTesting, ref BoxPosition handleHit)
        {
            if (!node.Visible) return false;

            bool nodeHit = false;
            float handleWidth;
            if (HandlesHitTesting.TouchMode)
            {
                m_scaleFactor = m_scaleFactor / 2;
                handleWidth = CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE / m_scaleFactor;
            }
            else
                handleWidth = CommonUsedValues.RESIZE_HANDLE_SIZE / m_scaleFactor;

            if (node.EditStyle.DefaultHandleEditMode == HandleEditMode.Resize)
            {
                // apply node transformations to graphics
                Matrix matrixTemp = CreateParentMatrix(node);

                // Create shapes trasform matrix - Without flips
                Matrix matrix = node.GetTransformations();
                
                // append flips
                node.AppendFlipTransforms(matrix);

                matrix.Multiply(matrixTemp, MatrixOrder.Append);

                // Test each handle.
                Array positions = Enum.GetValues(typeof(BoxPosition));

                PointF ptHandleLocation;
                PointF[] pts = new PointF[] { PointF.Empty };

                foreach (BoxPosition curPos in positions)
                {
                    if (curPos != BoxPosition.Center && IsHittestable(node, curPos))
                    {
                        //// 1 - Get handle location
                        ptHandleLocation = new HandleRenderer().GetHandlePosition(curPos, node);
                        //// 2 - Use Matrix.TransformPoints to define exact handle location
                        pts[0] = new PointF(ptHandleLocation.X, ptHandleLocation.Y);
                        //// transform point
                        matrix.TransformPoints(pts);

                        RectangleF rectBounds = Geometry.CreateRect(pts[0], new SizeF(handleWidth, handleWidth));

                        // 3 - Check point visibility
                        if (rectBounds.Contains(ptTesting))
                        {
                            nodeHit = true;
                            handleHit = curPos;
                            break;
                        }
                    }
                }
            }

            return nodeHit;
        }

        /// <summary>
        /// Returns node and ResizeHandle position that is found in ptTesting point from nodes collection.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="ptTesting">Search point of ResizeHandle.</param>
        /// <param name="handleHit">Handle position in ptTesting point.</param>
        /// <returns>true, if get the resize handle.</returns>
        public static bool GetResizeHandleAtPoint(Node node, Point ptTesting, ref BoxPosition handleHit)
        {
            return GetResizeHandleAtPoint(node, new PointF(ptTesting.X, ptTesting.Y), ref handleHit);
        }

        /// <summary>
        /// Returns node and RotationHandle position that is located in ptTesting point from nodes collection.
        /// </summary>
        /// <param name="nodes">Collection for verification.</param>
        /// <param name="ptTesting">Search point for RotationHandle.</param>
        /// <returns>true, if get the rotation handle.</returns>
        public static Node GetRotationHandleAtPoint(NodeCollection nodes, Point ptTesting)
        {
            return GetRotationHandleAtPoint(nodes, new PointF(ptTesting.X, ptTesting.Y));
        }

        /// <summary>
        /// Returns node and RotationHandle position that is located in ptTesting point from nodes collection.
        /// </summary>
        /// <param name="nodes">Collection for verification.</param>
        /// <param name="ptTesting">Search point for RotationHandle.</param>
        /// <returns>The rotation handle at the specified point.</returns>
        public static Node GetRotationHandleAtPoint(NodeCollection nodes, PointF ptTesting)
        {
            Node nodeToReturn = null;

            foreach (Node nodeCurrent in nodes)
            {
                if (nodeCurrent.Visible && nodeCurrent.ShowResizeHandles())
                {
                    nodeToReturn = GetRotationHandleAtPoint(nodeCurrent, ptTesting);

                    if (nodeToReturn != null)
                        break;
                }
            }

            return nodeToReturn;
        }

        /// <summary>
        ///  Shows whether there is at least one RotationHandle in the ptTesting point that belongs to node.
        /// </summary>
        /// <param name="node">Handle owner for verification.</param>
        /// <param name="ptTesting">Point of  possible location of the RotationHandle.</param>
        /// <returns>The node at the specified point.</returns>
        public static Node GetRotationHandleAtPoint(Node node, Point ptTesting)
        {
            return GetRotationHandleAtPoint(node, new PointF(ptTesting.X, ptTesting.Y));
        }

        /// <summary>
        ///  Shows whether there is at least one RotationHandle in the ptTesting point that belongs to node.
        /// </summary>
        /// <param name="node">Handle owner for verification.</param>
        /// <param name="ptTesting">Point of  possible location of the RotationHandle.</param>
        /// <returns>The node at the specified point.</returns>
        public static Node GetRotationHandleAtPoint(Node node, PointF ptTesting)
        {
            return GetHandleAtPoint(node, ptTesting, HandlePrimitive.RotationHandle);
        }

        /// <summary>
        ///  Returns topmost node from collection, PinPoint of which, is in ptTesting point.
        /// </summary>
        /// <param name="nodes">Collection for PinPoint search.</param>
        /// <param name="ptTesting">Point of the possible location of the Pinpoint.</param>
        /// <returns>The node at the specified point.</returns>
        public static Node GetPinPointAtPoint(NodeCollection nodes, PointF ptTesting)
        {
            Node nodeToReturn = null;

            foreach (Node nodeCurrent in nodes)
            {
                if (nodeCurrent.Visible && !(nodeCurrent is IEndPointContainer))
                {
                    if (GetPinPointAtPoint(nodeCurrent, ptTesting))
                    {
                        nodeToReturn = nodeCurrent;
                        break;
                    }
                }
            }

            return nodeToReturn;
        }

        /// <summary>
        ///  Returns topmost node from collection, PinPoint of which, is in ptTesting point.
        /// </summary>
        /// <param name="nodes">Collection for PinPoint search.</param>
        /// <param name="ptTesting">Point of the possible location of the Pinpoint.</param>
        /// <returns>The node at the specified point.</returns>
        public static Node GetPinPointAtPoint(NodeCollection nodes, Point ptTesting)
        {
            return GetPinPointAtPoint(nodes, new PointF(ptTesting.X, ptTesting.Y));
        }

        /// <summary>
        ///  Returns topmost node from collection, PinPoint of which, is in ptTesting point.
        /// </summary>
        /// <param name="node">PinPoint container.</param>
        /// <param name="ptTesting">Point of the possible location of the Pinpoint.</param>
        /// <returns>true, if get pin point.</returns>
        public static bool GetPinPointAtPoint(Node node, PointF ptTesting)
        {
            if (!node.Visible || node.EditStyle.HidePinPoint) return false;

            bool bSuccess = false;

            PointF[] ptsPin = new PointF[1];
            ptsPin[0] = MeasureUnitsConverter.ToPixels(node.PinPoint, node.MeasurementUnit);

            //// get parent transformations
            Matrix mtxParent = GetParentsTransformations(node);
            //// append parent transformations
            mtxParent.TransformPoints(ptsPin);
            //// Get Pin Point bounding rectangle
            RectangleF rectBounds = Geometry.CreateRect(ptsPin[0], GetHandleSize(HandlePrimitive.RotationHandle, node));

            // Check point visibility
            if (rectBounds.Contains(ptTesting))
                bSuccess = true;

            return bSuccess;
        }

        /// <summary>
        ///  Shows whether PinPoint can be found in the ptTesting point.
        /// </summary>
        /// <param name="node">Node for PinPoint verification.</param>
        /// <param name="ptTesting">Point for PinPoint search.</param>
        /// <returns>true, if get pin point.</returns>
        public static bool GetPinPointAtPoint(Node node, Point ptTesting)
        {
            return GetPinPointAtPoint(node, new Point(ptTesting.X, ptTesting.Y));
        }

        /// <summary>
        /// Shows whether PinPoint can be found in the ptTesting point.
        /// </summary>
        /// <param name="nodes">Collection for PinPoint verification.</param>
        /// <param name="ptTesting">Point for PinPoint search.</param>
        /// <param name="handleHit">The handle hit.</param>
        /// <returns>The node at the specified point.</returns>
        public static Node GetEndPointAtPoint(NodeCollection nodes, PointF ptTesting, ref IHandle handleHit)
        {
            Node nodeToReturn = null;

            foreach (Node nodeCurrent in nodes)
            {
                if (nodeCurrent.Visible && nodeCurrent is IEndPointContainer)
                {
                    if (GetEndPointAtPoint((IEndPointContainer)nodeCurrent, ptTesting, ref handleHit))
                    {
                        nodeToReturn = nodeCurrent;
                        break;
                    }
                }
            }

            return nodeToReturn;
        }

        /// <summary>
        ///  Returns topmost container and EndPoint that is found in ptTesting point.
        /// </summary>
        /// <param name="nodes">Collection for search of the EndPoint container.</param>
        /// <param name="ptTesting">Search point of the EndPoint from nodes collection.</param>
        /// <param name="handleHit">Container EndPoint that is found in search point.</param>
        /// <returns>The node at the specified point.</returns>
        public static Node GetEndPointAtPoint(NodeCollection nodes, Point ptTesting, ref IHandle handleHit)
        {
            return GetEndPointAtPoint(nodes, new PointF(ptTesting.X, ptTesting.Y), ref handleHit);
        }

        /// <summary>
        ///  Returns topmost container and EndPoint that is found in ptTesting point.
        /// </summary>
        /// <param name="node">Possibly EndPoint container.</param>
        /// <param name="ptTesting">Search point of the EndPoint from nodes collection.</param>
        /// <param name="handleHit">Container EndPoint that is found in search point.</param>
        /// <returns>true, if get end point.</returns>
        public static bool GetEndPointAtPoint(IEndPointContainer node, Point ptTesting, ref IHandle handleHit)
        {
            return GetEndPointAtPoint(node, new PointF(ptTesting.X, ptTesting.Y), ref handleHit);
        }

        /// <summary>
        ///  Returns topmost container and EndPoint that is found in ptTesting point.
        /// </summary>
        /// <param name="node">Possibly the EndPoint container.</param>
        /// <param name="ptTesting">Search point of the EndPoint from nodes collection.</param>
        /// <param name="handleHit">Container EndPoint that is found in search point.</param>
        /// <returns>true, if get end point.</returns>
        public static bool GetEndPointAtPoint(IEndPointContainer node, PointF ptTesting, ref IHandle handleHit)
        {
            if (!((Node)node).Visible) return false;

            bool bSuccess = false;

            PointF[] ptsPoint = new PointF[1];

            // get parent transformations
            Matrix mtxParent = GetParentsTransformations((Node)node);

            ptsPoint[0] = node.HeadEndPoint.Location;
            
            // append parent transformations
            mtxParent.TransformPoints(ptsPoint);

            // Get End Point bounding rectangle
            RectangleF rectBounds = Geometry.CreateRect(ptsPoint[0], GetHandleSize(HandlePrimitive.RotationHandle, node as Node));

            // Check point visibility
            if (rectBounds.Contains(ptTesting))
            {
                handleHit = node.HeadEndPoint;
                bSuccess = true;
            }

            if (!bSuccess)
            {
                ptsPoint[0] = node.TailEndPoint.Location;

                // append parent transformations
                mtxParent.TransformPoints(ptsPoint);

                // Get End Point bounding rectangle
                rectBounds = Geometry.CreateRect(ptsPoint[0], GetHandleSize(HandlePrimitive.RotationHandle, node as Node));

                // Check point visibility
                if (rectBounds.Contains(ptTesting))
                {
                    handleHit = node.TailEndPoint;
                    bSuccess = true;
                }
            }

            return bSuccess;
        }

        /// <summary>
        ///  Returns topmost node and its handle that is in search point.
        /// </summary>
        /// <param name="nodes">Nodes collection for handle search.</param>
        /// <param name="ptTesting">Handle search point.</param>
        /// <param name="handleHit">Node handle that is in search point.</param>
        /// <returns>The node at the specified point.</returns>
        public static PathNode GetControlPointAtPoint(NodeCollection nodes, PointF ptTesting, ref IHandle handleHit)
        {
            PathNode nodeToReturn = null;
            PathNode nodeCur;

            foreach (Node nodeCurrent in nodes)
            {
                nodeCur = nodeCurrent as PathNode;

                if (nodeCur != null && nodeCur.Visible)
                {
                    handleHit = nodeCur.GetControlPointAt(ptTesting);

                    if (handleHit != null)
                    {
                        nodeToReturn = nodeCur;
                        break;
                    }
                }
            }

            return nodeToReturn;
        }

        /// <summary>
        ///  Returns topmost node and its handle that is in search point.
        /// </summary>
        /// <param name="nodes">Nodes collection for handle search.</param>
        /// <param name="ptTesting">Handle search point.</param>
        /// <param name="handleHit">Node handle that is in search point.</param>
        /// <returns>The node at the specified point.</returns>
        public static PathNode GetControlPointAtPoint(NodeCollection nodes, Point ptTesting, ref IHandle handleHit)
        {
            return GetControlPointAtPoint(nodes, new PointF(ptTesting.X, ptTesting.Y), ref handleHit);
        }

        /// <summary>
        ///  Returns ConnectionPoint, that belongs to the node transformed using given transformation matrix and intersects with point.
        /// </summary>
        /// <param name="nodes">Nodes collection for port search.</param>
        /// <param name="ptTesting">Point that contains port.</param>
        /// <returns>The connection point.</returns>
        public static ConnectionPoint GetConnectionPointAtPoint(NodeCollection nodes, PointF ptTesting)
        {
            ConnectionPoint portToReturn = null;

            foreach (Node node in nodes)
            {
                portToReturn = GetConnectionPointAtPoint(node, ptTesting);

                if (portToReturn != null)
                    break;
            }

            return portToReturn;
        }

        /// <summary>
        /// Returns ConnectionPoint, that belongs to the node transformed using given transformation matrix and intersects with point.
        /// </summary>
        /// <param name="node">Node for port search.</param>
        /// <param name="ptTesting">Point that contains port.</param>
        /// <returns>Connection point.</returns>
        public static ConnectionPoint GetConnectionPointAtPoint(Node node, PointF ptTesting)
        {
            return GetConnectionPointAtPoint(node, GetParentsTransformations(node, false), ptTesting);
        }

        /// <summary>
        ///  Returns ConnectionPoint that belongs to the node transformed using the given transformation matrix and intersects with point.
        /// </summary>
        /// <param name="node">Ports container.</param>
        /// <param name="matrixParent">Parent's transformation matrix.</param>
        /// <param name="ptTesting">Search point of the port.</param>
        /// <returns>Connection point.</returns>
        public static ConnectionPoint GetConnectionPointAtPoint(Node node, Matrix matrixParent, PointF ptTesting)
        {
            ConnectionPoint portToReturn = null;

            // create shapes trasform matrix
            Matrix matrixTransform = node.GetTransformations();
            node.AppendFlipTransforms(matrixTransform);
            matrixTransform.Multiply(matrixParent, MatrixOrder.Append);

            // convert to composite node if can
            ICompositeNode group = node as ICompositeNode;

            // find looking port into group children
            if (group != null)
            {
                for (int i = 0, nLength = group.ChildCount; i < nLength; i++)
                {
                    portToReturn = GetConnectionPointAtPoint(group.GetChild(i), matrixTransform, ptTesting);

                    if (portToReturn != null)
                        break;
                }
            }

            // get node port at testing point
            if (node.Ports.Count > 0 && portToReturn == null)
                portToReturn = GetPortAtPoint(node, matrixTransform, ptTesting);

            return portToReturn;
        }

        /// <summary>
        ///  Returns ConnectionPoint, that belongs to the node transformed using given transformation matrix and intersects with point.
        /// </summary>
        /// <param name="nodes">Nodes collection for port search.</param>
        /// <param name="ptTesting">Point that contains port.</param>
        /// <returns>Connection point.</returns>
        public static ConnectionPoint GetConnectionPointAtPoint(NodeCollection nodes, Point ptTesting)
        {
            return GetConnectionPointAtPoint(nodes, new PointF(ptTesting.X, ptTesting.Y));
        }

        /// <summary>
        /// Returns ConnectionPoint, that belongs to the node transformed using given transformation matrix and intersects with point.
        /// </summary>
        /// <param name="node">Node for port search.</param>
        /// <param name="ptTesting">Point that contains port.</param>
        /// <returns>Connection point.</returns>
        public static ConnectionPoint GetConnectionPointAtPoint(Node node, Point ptTesting)
        {
            return GetConnectionPointAtPoint(node, new PointF(ptTesting.X, ptTesting.Y));
        }

        /// <summary>
        ///  Returns nodes from collection given, which intersects with rectangle recbBounding.
        /// </summary>
        /// <param name="nodes">Collection for verification.</param>
        /// <param name="recbBounding">Rectangle in which nodes are located..</param>
        /// <returns>Node collection.</returns>
        public static NodeCollection GetNodesIntersecting(NodeCollection nodes, RectangleF recbBounding)
        {
            if (nodes == null)
                throw new ArgumentNullException("nodes");

            NodeCollection nodesToReturn = new NodeCollection();
            RectangleF rectNodeBounding;

            foreach (Node node in nodes)
            {
                rectNodeBounding = ((IUnitIndependent)node).GetBoundingRectangle(MeasureUnits.Pixel, false);

                if (recbBounding.IntersectsWith(rectNodeBounding))
                    nodesToReturn.Add(node);
            }

            return nodesToReturn;
        }

        /// <summary>
        ///  Returns nodes from collection given, which are located inside rectangle recbBounding.
        /// </summary>
        /// <param name="nodes">Collection for verification.</param>
        /// <param name="recbBounding">Rectangle in which nodes are located..</param>
        /// <returns>Contained node collection.</returns>
        public static NodeCollection GetNodesContainedBy(NodeCollection nodes, RectangleF recbBounding)
        {
            if (nodes == null)
                throw new ArgumentNullException("nodes");

            NodeCollection nodesToReturn = new NodeCollection();
            RectangleF rectNodeBounding;

            foreach (Node node in nodes)
            {
                rectNodeBounding = ((IUnitIndependent)node).GetBoundingRectangle(MeasureUnits.Pixel, false);

                if (recbBounding.Contains(rectNodeBounding))
                    nodesToReturn.Add(node);
            }

            return nodesToReturn;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Gets the graphis path bounds with matrix transfomation.
        /// </summary>
        /// <param name="grPath">The graphics path.</param>
        /// <param name="mtxTransform">The matrix transform.</param>
        /// <returns>Graphics rect.</returns>
        private static RectangleF GetGraphisPathBounds(GraphicsPath grPath, Matrix mtxTransform)
        {
            PointF[] ptsPathPoints = grPath.PathPoints;
            mtxTransform.TransformPoints(ptsPathPoints);
            return Geometry.CreateRect(ptsPathPoints);
        }

        /// <summary>
        /// Gets the decorator transfromation.
        /// </summary>
        /// <param name="ptPoint">The decorator position.</param>
        /// <param name="ptControl">The last control position.</param>
        /// <param name="depth">The decorator end point depth.</param>
        /// <returns>The matrix.</returns>
        private static Matrix GetDecoratorTransfrom(PointF ptPoint, PointF ptControl, float depth)
        {
            // Calc points angle for decorator.
            float angle = (float)Math.Atan2(ptPoint.Y - ptControl.Y, ptPoint.X - ptControl.X);

            // Convert from radian to degree
            angle = (float)(180 * angle / Math.PI);

            // Calc transformations.
            Matrix matrixTemp = new Matrix();
            matrixTemp.Translate(ptPoint.X - depth, ptPoint.Y, MatrixOrder.Append);
            matrixTemp.RotateAt(angle, ptPoint, MatrixOrder.Append);

            return matrixTemp;
        }

        /// <summary>
        /// Gets the tail decorator transformations.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>The matrix.</returns>
        private static Matrix GetTailDecoratorTransformations(LineBase line)
        {
            // Get data points.
            PointF[] pts = line.GraphicsPath.PathPoints;

            // Get decorator depth
            float depth = line.TailDecorator.Size.Width - 1;

            // Append transform to graphics.
            Matrix mtxTemp = GetDecoratorTransfrom(pts[0], pts[1], depth);
            mtxTemp.Translate(0, -line.TailDecorator.Size.Height / 2);

            return mtxTemp;
        }

        /// <summary>
        /// Gets the head decorator transformations.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>The matrix.</returns>
        private static Matrix GetHeadDecoratorTransformations(LineBase line)
        {
            // Get data points.
            PointF[] pts = line.GraphicsPath.PathPoints;
            int ptsCount = pts.Length;

            // Get decorator depth
            float depth = line.HeadDecorator.Size.Width - 1;

            // Append transform to graphics.
            Matrix mtxTemp = GetDecoratorTransfrom(pts[ptsCount - 1], pts[ptsCount - 2], depth);
            mtxTemp.Translate(0, -line.HeadDecorator.Size.Height / 2);

            return mtxTemp;
        }

        /// <summary>
        /// Create matrix with append parent's
        /// transformations on given graphics - WITHOUT ROTATIONS
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The matrix.</returns>
        private static Matrix CreateParentMatrix(Node node)
        {
            if (node is PseudoGroup) return new Matrix();

            if (node == null || node.Parent == null)
                throw new ArgumentNullException("node + parent");

            Matrix matrixToReturn = new Matrix();
            Matrix matrixTemp;
            ICompositeNode nodeParent = node.Parent;

            // iterate through given node parents multiplying their transformations
            while (!(nodeParent is Model))
            {
                matrixTemp = ((Node)nodeParent).GetTransformations();
                ((Node)nodeParent).AppendFlipTransforms(matrixTemp);
                matrixToReturn.Multiply(matrixTemp, MatrixOrder.Append);

                nodeParent = ((Node)nodeParent).Parent;
            }

            return matrixToReturn;
        }
        private static ConnectionPoint GetPortAtPoint(Node node, Matrix mtxTransformations, PointF ptTesting)
        {
            ConnectionPoint portToReturn = null;

            if (node != null)
            {
                RectangleF rectHandle;
                PointF[] pts = new PointF[1];
                RectangleF endPointRect = Geometry.CreateRect(ptTesting, new SizeF(3, 3));
                SizeF szHandleSize = new SizeF(CommonUsedValues.RESIZE_HANDLE_SIZE, CommonUsedValues.RESIZE_HANDLE_SIZE);

                foreach (ConnectionPoint port in node.Ports)
                {
                    //// get port location
                    pts[0] = port.GetPosition(); // GetPortLocation( node, port );
                    //// 2 - Use Matrix.TransformPoints to define exact handle location
                    mtxTransformations.TransformPoints(pts);
                    //// 3 - create handle rectangle for hit testing 
                    rectHandle = Geometry.CreateRect(pts[0], szHandleSize);

                    // 4 - Check point visibility
                    if (rectHandle.IntersectsWith(endPointRect))
                    {
                        portToReturn = port;
                        break;
                    }
                }
            }

            return portToReturn;
        }
        private static bool IsHittestable(Node node, BoxPosition curPosition)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            bool bSuccess = true;

            if (!EditStyle.CanChangeHeight(node) &&
                ((curPosition == BoxPosition.TopLeft) || (curPosition == BoxPosition.TopCenter) ||
                  (curPosition == BoxPosition.TopRight) || (curPosition == BoxPosition.BottomLeft) ||
                  (curPosition == BoxPosition.BottomCenter) || (curPosition == BoxPosition.BottomRight)))
            {
                bSuccess = false;
            }

            if (bSuccess && !EditStyle.CanChangeWidth(node) &&
                ((curPosition == BoxPosition.TopLeft) || (curPosition == BoxPosition.MiddleLeft) ||
                (curPosition == BoxPosition.TopRight) || (curPosition == BoxPosition.BottomLeft) ||
                (curPosition == BoxPosition.MiddleRight) || (curPosition == BoxPosition.BottomRight)))
            {
                bSuccess = false;
            }

            return bSuccess;
        }
        private static Node GetHandleAtPoint(Node node, PointF ptTesting, HandlePrimitive handle)
        {
            Node nodetoReturn = null;

            // check whether given handle can take part in hit testing
            if ((handle == HandlePrimitive.RotationHandle && node.EditStyle.AllowRotate) ||
                (handle == HandlePrimitive.PinPoint && node.EditStyle.AllowMoveX && node.EditStyle.AllowMoveY && !node.EditStyle.HidePinPoint))
            {
                // 1 - Create node's trasform matrix.
                Matrix matrixTemp = node.GetTransformations();
                node.AppendFlipTransforms(matrixTemp);

                Node parent = node.Parent as Node;
                if (parent != null)
                {
                    Matrix mtxParent = GetParentsTransformations(node);
                    matrixTemp.Multiply(mtxParent, MatrixOrder.Append);
                }

                // 2 - Get handle location
                PointF[] pts = new PointF[1];
                pts[0] = GetHandleLocation(node, handle);

                // 3 - Use Matrix.TransformPoints to define exact handle location
                matrixTemp.TransformPoints(pts);

                RectangleF rectBounds = Geometry.CreateRect(pts[0], GetHandleSize(handle, node));

                // 4 - Check point visibility
                if (rectBounds.Contains(ptTesting) && !node.EditStyle.HideRotationHandle)
                    nodetoReturn = node;
            }

            return nodetoReturn;
        }
        private static PointF GetHandleLocation(Node node, HandlePrimitive handle)
        {
            PointF ptLocationToReturn = PointF.Empty;

            // get unit independent values
            SizeF szUnitIndependentPinOffsetValue = ((IUnitIndependent)node).GetPinPointOffset(MeasureUnits.Pixel);

            switch (handle)
            {
                case HandlePrimitive.PinPoint:
                    ptLocationToReturn = ((IUnitIndependent)node).GetPinPoint(MeasureUnits.Pixel);
                    break;
                case HandlePrimitive.RotationHandle:
                    float fRHO;
                    if (HandlesHitTesting.TouchMode)
                        fRHO = CommonUsedValues.ROTATION_HANDLE_TOUCH_OFFSET / ScaleFactor;
                    else
                        fRHO = CommonUsedValues.ROTATION_HANDLE_OFFSET / ScaleFactor;

                    float fYOffset = (szUnitIndependentPinOffsetValue.Height < 0)
                                        ? (szUnitIndependentPinOffsetValue.Height - fRHO) : -fRHO;

                    ptLocationToReturn = new PointF(szUnitIndependentPinOffsetValue.Width, fYOffset);
                    break;
            }

            return ptLocationToReturn;
        }
        private static SizeF GetHandleSize(HandlePrimitive handle, Node node)
        {
            SizeF szToReturn = SizeF.Empty;

            Model model = node.Parent as Model;

            float pinHandleSize, rotationHandleSize, resizeHandleSize;
            if (HandlesHitTesting.TouchMode)
            {
                m_scaleFactor = m_scaleFactor / 2;
                pinHandleSize = CommonUsedValues.PIN_POINT_TOUCH_SIZE / m_scaleFactor;
                rotationHandleSize = CommonUsedValues.ROTATION_HANDLE_TOUCH_SIZE / m_scaleFactor;
                resizeHandleSize = CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE / m_scaleFactor;
            }
            else
            {
                pinHandleSize = CommonUsedValues.PIN_POINT_SIZE / m_scaleFactor;
                rotationHandleSize = CommonUsedValues.ROTATION_HANDLE_SIZE / m_scaleFactor;
                resizeHandleSize = CommonUsedValues.RESIZE_HANDLE_SIZE / m_scaleFactor;
            }          

            switch (handle)
            {
                case HandlePrimitive.PinPoint:
                    szToReturn = new SizeF(pinHandleSize, pinHandleSize);
                    break;
                case HandlePrimitive.RotationHandle:
                    szToReturn = new SizeF(rotationHandleSize, rotationHandleSize);
                    break;
                case HandlePrimitive.ControlPoint:
                    szToReturn = new SizeF(resizeHandleSize, resizeHandleSize);
                    break;
            }

            return szToReturn;
        }

        /// <summary>
        /// Convert point to node local coordinates.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="ptLocation">The given point.</param>
        /// <returns>The local point.</returns>
        private static PointF GetLocalPoint(Node parent, PointF ptLocation)
        {
            if (parent != null)
            {
                Matrix mtxTransformation = GetParentsTransformations(parent, true);
                mtxTransformation.Invert();

                ptLocation = Geometry.AppendMatrix(ptLocation, mtxTransformation);
            }

            return ptLocation;
        }
        private static void ConvertToParentCoordinates(Node node, Node parent, ref NodeInfo nodeInfo)
        {
            if (parent == null)
                return;

            Node parentRoot = parent.Parent as Node;

            // 1 - check if parent is not a model
            if (parentRoot != null)
                ConvertToParentCoordinates(node, parentRoot, ref nodeInfo);

            IUnitIndependent nodeIndependent = node;

            // 2 - update pin point
            using (Matrix mtxTransform = parent.GetTransformations())
            {
                parent.AppendFlipTransforms(mtxTransform);
                mtxTransform.Invert();
                nodeInfo.PinPoint = Geometry.AppendMatrix(nodeInfo.PinPoint, mtxTransform);
            }

            // 3 - update flip flags
            nodeInfo.FlipX = parent.FlipX ? !nodeInfo.FlipX : nodeInfo.FlipX;
            nodeInfo.FlipY = parent.FlipY ? !nodeInfo.FlipY : nodeInfo.FlipY;

            // 4 - update rotation angle
            float fAngle = parent.RotationAngle;
            fAngle = ((nodeInfo.FlipX || nodeInfo.FlipY) && !(nodeInfo.FlipX && nodeInfo.FlipY)) ? -fAngle : fAngle;
            nodeInfo.RotationAngle -= fAngle;
        }
        private static void ConvertToModelCoordinates(Node node, Node parent, ref NodeInfo nodeInfo)
        {
            if (parent == null)
                return;

            IUnitIndependent nodeIndependent = node;

            // 1 - update pin point
            using (Matrix mtxTransform = parent.GetTransformations())
            {
                parent.AppendFlipTransforms(mtxTransform);
                nodeInfo.PinPoint = Geometry.AppendMatrix(nodeInfo.PinPoint, mtxTransform);
            }

            // 2 - update rotation angle
            float fAngle = parent.RotationAngle;
            fAngle = ((nodeInfo.FlipX || nodeInfo.FlipY) && !(nodeInfo.FlipX && nodeInfo.FlipY)) ? -fAngle : fAngle;
            nodeInfo.RotationAngle += fAngle;

            // 3 - update flip flags
            nodeInfo.FlipX = parent.FlipX ? !nodeInfo.FlipX : nodeInfo.FlipX;
            nodeInfo.FlipY = parent.FlipY ? !nodeInfo.FlipY : nodeInfo.FlipY;

            parent = parent.Parent as Node;

            // 4 - check if parent not is model
            if (parent != null)
                ConvertToModelCoordinates(node, parent, ref nodeInfo);
        }
        #endregion

        /// <summary>
        /// Helper structure to save goal node information. Used internal only.
        /// </summary>
        private struct NodeInfo
        {
            public PointF PinPoint;
            public SizeF PinPointOffset;
            public SizeF Size;
            public float RotationAngle;
            public bool FlipX;
            public bool FlipY;
            public MeasureUnits Unit;

            public NodeInfo(Node node, MeasureUnits units)
            {
                this.Unit = units;
                IUnitIndependent nodeInd = (IUnitIndependent)node;
                this.PinPoint = nodeInd.GetPinPoint(units);
                this.PinPointOffset = nodeInd.GetPinPointOffset(units);
                this.Size = nodeInd.GetSize(units);
                this.RotationAngle = node.RotationAngle;
                this.FlipX = node.FlipX;
                this.FlipY = node.FlipY;
            }

            #region Class public methods
            public void AppendChanges(Node node)
            {
                IUnitIndependent nodeInd = (IUnitIndependent)node;
                nodeInd.SetPinPoint(this.PinPoint, this.Unit);
                nodeInd.SetPinPointOffset(this.PinPointOffset, this.Unit);
                nodeInd.SetSize(this.Size, this.Unit);
                node.RotationAngle = this.RotationAngle;
                node.FlipX = this.FlipX;
                node.FlipY = this.FlipY;
            }
            #endregion
        }
    }
}
