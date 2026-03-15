#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Interactive tool for drawing lines.
    /// </summary>
    public class DirectedLineConnectorTool
        : LineBaseTool
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectedLineConnectorTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public DirectedLineConnectorTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("DirectedLineLinkTool"))
        {
            this.ToolCursor = this.ActionCursor = Cursors.Cross;
        }
        #endregion

        #region Class override
        /// <summary>
        /// Creates the line shape node.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <returns>The node.</returns>
        protected override Node CreateNode(PointF ptStart, PointF ptEnd)
        {
            LineBase toReturn = new LineConnector(ptStart, ptEnd);
            toReturn.HeadDecorator.DecoratorShape = DecoratorShape.FilledFancyArrow;
            ((ConnectorBase)toReturn).LineRoutingEnabled = true;
            return toReturn;
        }
        #endregion
    }

    /// <summary>
    /// Implement base actions to create line shape.
    /// </summary>
    public abstract class LineBaseTool
        : UITool
    {
        #region Class members
        /// <summary>
        /// ConnectionPoint to which Head EndPoint could be connected.
        /// </summary>
        private ConnectionPoint m_portHeadPossConn;

        /// <summary>
        /// ConnectionPoint to which Tail EndPoint could be connected.
        /// </summary>
        private ConnectionPoint m_portTailPossConn;

        /// <summary>
        /// Head Decorator applied to created node.
        /// </summary>
        private HeadDecorator m_decorHead;

        /// <summary>
        /// Tail Decorator applied to created node.
        /// </summary>
        private TailDecorator m_decorTail;

        /// <summary>
        /// Head endpoint possible connection port bounding frame.
        /// </summary>
        private System.Drawing.Rectangle m_rectHeadPC;

        /// <summary>
        /// Tail endpoint possible connection port bounding frame.
        /// </summary>
        private System.Drawing.Rectangle m_rectTailPC;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LineBaseTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="name">The name.</param>
        public LineBaseTool(DiagramController controller, string name)
            : base(controller, name)
        { 
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the head possible connection.
        /// </summary>
        /// <value>The head possible connection.</value>
        protected ConnectionPoint HeadPossibleConnection
        {
            get { return m_portHeadPossConn; }
            set { m_portHeadPossConn = value; }
        }

        /// <summary>
        /// Gets or sets the tail possible connection.
        /// </summary>
        /// <value>The tail possible connection.</value>
        protected ConnectionPoint TailPossibleConnection
        {
            get { return m_portTailPossConn; }
            set { m_portTailPossConn = value; }
        }

        /// <summary>
        /// Gets the rendering  helper.
        /// </summary>
        /// <value>The rendering  helper.</value>
        protected Node RenderringHelper
        {
            get
            {
                Node nodeToReturn = null;

                // start point
                PointF ptStart = GetEndPointLocation(this.TailPossibleConnection, GetStartPoint(false));
                
                // end point
                PointF ptEnd = GetEndPointLocation(this.HeadPossibleConnection, this.CurrentPoint);

                if (ptStart != ptEnd)
                {
                    nodeToReturn = CreateNode(ptStart, ptEnd);
                    AlterStyle(nodeToReturn);
                }

                return nodeToReturn;
            }
        }

        /// <summary>
        /// Gets or sets Head Decorator applied to created node.
        /// </summary>
        public HeadDecorator HeadDecorator
        {
            get
            {
                if (m_decorHead == null)
                {
                    m_decorHead = new HeadDecorator();
                }

                return m_decorHead;
            }
            set 
            { 
                m_decorHead = value; 
            }
        }

        /// <summary>
        /// Gets or sets Tail Decorator applied to created node.
        /// </summary>
        public TailDecorator TailDecorator
        {
            get
            {
                if (m_decorTail == null)
                {
                    m_decorTail = new TailDecorator();
                }

                return m_decorTail;
            }
            set 
            { 
                m_decorTail = value; 
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Draws the line shape to specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public override void Draw(Graphics gfx)
        {
            Node nodeRH = this.RenderringHelper;

            if (this.InAction && nodeRH != null)
            {
                nodeRH.Draw(gfx);
            }

            // Head endpoint possible connection
            OutlineConnectionPoint(gfx, this.HeadPossibleConnection);
            
            // Tail endpoint possible connection
            OutlineConnectionPoint(gfx, this.TailPossibleConnection);
        }

        /// <summary>
        /// Processes the mouse down event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseDown(MouseEventArgs evtArgs)
        {
            // call base before set start point
            Tool toolToRetrun = base.ProcessMouseDown(evtArgs);

            bool canConnectWithPort = false;
            this.TailPossibleConnection = CheckConnectionPossibility(this.StartPoint, out canConnectWithPort);
            
            // include possible connection point frame
            UpdatePortRefreshRect(this.TailPossibleConnection, ref m_rectTailPC);

            return toolToRetrun;
        }

        /// <summary>
        /// Processes the mouse move event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            // call base to update current position
            this.CurrentPoint = new Point(evtArgs.X, evtArgs.Y);

            this.CanRender = false;

            if (this.InAction)
            {
                // start point
                PointF ptStart = GetEndPointLocation(this.TailPossibleConnection, GetStartPoint(false));
                ptStart = this.Controller.ConvertFromModelToClientCoordinates(ptStart);
                
                // end point
                PointF ptCur = GetEndPointLocation(this.HeadPossibleConnection, this.CurrentPoint);
                ptCur = this.Controller.ConvertFromModelToClientCoordinates(ptCur);

                System.Drawing.Rectangle rectTemp = Geometry.ConvertRectangle(Geometry.CreateRect(ptStart, ptCur));

                bool canConnectWithPort = false;
                // check possible head connection
                this.HeadPossibleConnection = CheckConnectionPossibility(this.CurrentPoint, out canConnectWithPort);

                if (rectTemp != this.WorkRectPrev)
                {
                    this.CanRender = true;
                    this.WorkRectPrev = this.WorkRect;
                    this.WorkRect = rectTemp;
                    PathNode node = this.RenderringHelper as PathNode;
                    if(node!=null)
                    foreach (Label label in node.Labels)
                    {
                        if(label.Position == Position.Custom)
                            this.WorkRect = System.Drawing.Rectangle.Inflate(this.WorkRect, (int)label.OffsetX, (int)label.OffsetY);
                    }                    
                    // include possible connection point frame
                    UpdatePortRefreshRect(this.HeadPossibleConnection, ref m_rectHeadPC);
                    UpdatePortRefreshRect(this.TailPossibleConnection, ref m_rectTailPC);
                    this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);
                }

                UpdateCursor(CanAddNode(this.RenderringHelper) && canConnectWithPort);
            }

            return this;
        }

        /// <summary>
        /// Processes the mouse up event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            this.CanRender = false;
            this.CurrentPoint = new Point(evtArgs.X, evtArgs.Y);

            if (this.InAction)
            {
                this.InAction = false;

                PointF ptStart = GetEndPointLocation(this.TailPossibleConnection, GetStartPoint(false));
                PointF ptEnd = GetEndPointLocation(this.HeadPossibleConnection, this.CurrentPoint);

                if (ptStart != ptEnd)
                {
                    Model model = this.Controller.Model;

                    if (model != null)
                    {
                        model.BeginUpdate();
                    }

                    CompleteAction(ptStart, ptEnd);

                    // include possible connection point frame
                    if (!m_rectHeadPC.IsEmpty)
                    {
                        this.Controller.UpdateInfo.UpdateRefreshRect(m_rectHeadPC);
                        m_rectHeadPC = System.Drawing.Rectangle.Empty;
                    }

                    if (!m_rectTailPC.IsEmpty)
                    {
                        this.Controller.UpdateInfo.UpdateRefreshRect(m_rectTailPC);
                        m_rectTailPC = System.Drawing.Rectangle.Empty;
                    }
                    this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);

                    if (model != null)
                    {
                        model.EndUpdate();
                    }
                }
            }

            return base.ProcessMouseUp(evtArgs);
        }
        #endregion

        #region Class helper methods

        /// <summary>
        /// Checks for connection possibility.
        /// </summary>
        /// <param name="ptTest">The point.</param>
        /// <param name="canConnectWithPort">true, if the port can accept connection</param>
        /// <returns>The connection point</returns>
        protected ConnectionPoint CheckConnectionPossibility(PointF ptTest, out bool canConnectWithPort)
        {
            IEndPointContainer con = this.RenderringHelper is IEndPointContainer ? this.RenderringHelper as IEndPointContainer : null;
            
            //Gets the current port
            ConnectionPoint portToReturn = base.CheckConnectionPossibility(ptTest);
            canConnectWithPort = true;
            if (portToReturn != null && con != null && !portToReturn.CheckType(con.HeadEndPoint))
            {
                portToReturn = null;
                canConnectWithPort = false;
            }

            return portToReturn;
        }

        private void CompleteAction(PointF ptStart, PointF ptEnd)
        {
            RectangleF rectModelBoudns = MeasureUnitsConverter.ToPixels(
                this.Controller.Model.Bounds,
                this.Controller.Model.MeasurementUnits);

            // Check whether start and end points lie in document bounds
            if (!this.Controller.Model.BoundaryConstraintsEnabled || rectModelBoudns.Contains(ptStart) && rectModelBoudns.Contains(ptEnd))
            {
                if (!this.InAction)
                {
                    System.Drawing.Drawing2D.Matrix mtxScale = this.Controller.Model.DocumentScale.GetScaleTransformation(MeasureUnits.Pixel);
                    mtxScale.Invert();
                    ptStart = Geometry.AppendMatrix(ptStart, mtxScale);
                    ptEnd = Geometry.AppendMatrix(ptEnd, mtxScale);
                }
                // Create tool specific node
                Node node = CreateNode(ptStart, ptEnd);
                
                // Begin atomic action
                HistoryManager mgrHistory = this.Controller.Model.HistoryManager;

                if (mgrHistory != null)
                    mgrHistory.StartAtomicAction("Insert Node");

                // Insert node
                this.Controller.Model.AppendChild(node);

                // Connect if possible
                IEndPointContainer endPointContainer = node as IEndPointContainer;

                if (endPointContainer != null)
                {
                    if (this.HeadPossibleConnection != null)
                    {
                        this.HeadPossibleConnection.TryConnect(endPointContainer.HeadEndPoint);
                    }

                    if (this.TailPossibleConnection != null)
                    {
                        this.TailPossibleConnection.TryConnect(endPointContainer.TailEndPoint);
                    }
                }

                // End atomic action
                if (mgrHistory != null)
                    mgrHistory.EndAtomicAction();

                // clean up
                this.TailPossibleConnection = null;
                this.HeadPossibleConnection = null;
            }
        }

        /// <summary>
        /// Creates the line shape node.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <returns>The node.</returns>
        protected abstract Node CreateNode(PointF ptStart, PointF ptEnd);
        #endregion

        #region Connection helper methods
        /// <summary>
        /// Sets the decorator for given line node.
        /// </summary>
        /// <param name="line">The line.</param>
        protected void SetDecorator(LineBase line)
        {
            // line.HeadDecorator
            Decorator decorTool = this.HeadDecorator;
            Decorator decorNode = line.HeadDecorator;

            ApplyDecorator(decorTool, decorNode);

            if (decorTool != null)
                decorNode.Container = line;

            decorTool = this.TailDecorator;
            decorNode = line.TailDecorator;

            ApplyDecorator(decorTool, decorNode);

            if (decorTool != null)
                decorNode.Container = line;
        }
        #endregion
    }
}
