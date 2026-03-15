#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Interactive tool for drawing polyline connector.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.PolyLineConnector"/>
    /// </remarks>
    public class PolyLineConnectorTool
        : PolyLineBase
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
        /// Head endpoint possible connection port bounding frame.
        /// </summary>
        private System.Drawing.Rectangle m_rectHeadPC;

        /// <summary>
        /// Tail endpoint possible connection port bounding frame.
        /// </summary>
        private System.Drawing.Rectangle m_rectTailPC;
        #endregion

        #region Class proeprties
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
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLineConnectorTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public PolyLineConnectorTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("PolyLineLinkTool"))
        {
            this.ToolCursor = this.ActionCursor = Cursors.Cross;
        }
        #endregion

        #region Class override
        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            this.CurrentPoint = new Point(evtArgs.X, evtArgs.Y);
            this.CanRender = false;

            UpdateHelperNode();

            if (this.InAction)
            {
                // start point
                PointF ptStart = GetEndPointLocation(this.TailPossibleConnection, GetStartPoint(false));
                ptStart = this.Controller.ConvertFromModelToClientCoordinates(ptStart);
                
                // end point
                PointF ptCur = GetEndPointLocation(this.HeadPossibleConnection, this.CurrentPoint);
                ptCur = this.Controller.ConvertFromModelToClientCoordinates(ptCur);

                System.Drawing.Rectangle rectTemp = Geometry.ConvertRectangle(Geometry.CreateRect(ptStart, ptCur));

                // check possible head connection
                if (this.Points.Length > 0)
                    ptStart = this.Controller.ConvertFromModelToClientCoordinates(this.Points[0]);
                bool canConnectWithPort = false;
                this.TailPossibleConnection = CheckConnectionPossibility(ptStart, out canConnectWithPort);
                this.HeadPossibleConnection = CheckConnectionPossibility(this.CurrentPoint, out canConnectWithPort);

                // add point like in line connector tool
                if (evtArgs.Button == MouseButtons.Left && this.TailPossibleConnection != null
                    && this.Points.Length == 0)
                {
                    AddPoint();
                }

                if (rectTemp != this.WorkRectPrev)
                {
                    this.CanRender = true;
                    this.WorkRect = Geometry.Union(this.WorkRect, rectTemp);

                    // include possible connection point frame
                    UpdatePortRefreshRect(this.HeadPossibleConnection, ref m_rectHeadPC);
                    UpdatePortRefreshRect(this.TailPossibleConnection, ref m_rectTailPC);
                }
                UpdateCursor(CanAddNode(this.RenderingHelper) && canConnectWithPort);
            }

            return this;
        }

        /// <summary>
        /// Get the last path point position in model coordinates.
        /// </summary>
        /// <returns>Point in model coordinates.</returns>
        protected override Point GetLastPathPointPosition()
        {
            Point ptPolyline = base.GetLastPathPointPosition();

            if (this.TailPossibleConnection != null && this.Points.Length > 0)
            {
                // dock first path point to it possible port
                PointF ptFirst = this.Controller.ConvertFromModelToClientCoordinates(this.Points[0]);
                this.Points[0] = this.Controller.ConvertToModelCoordinates(SnapToPort(ptFirst, SizeF.Empty));
            }

            if (this.HeadPossibleConnection != null)
            {
                PointF ptTemp = this.Controller.ConvertToModelCoordinates(GetCurrentPoint(false));
                ptPolyline = Geometry.ConvertPoint(ptTemp);
                ptPolyline = this.Controller.ConvertFromModelToClientCoordinates(ptPolyline);
                
                // dock last path point to it possible port
                ptPolyline = Geometry.ConvertPoint(this.Controller.ConvertToModelCoordinates(SnapToPort(ptPolyline, SizeF.Empty)));
            }

            return ptPolyline;
        }

        /// <summary>
        /// Determines whether this can complete action.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>
        /// <c>true</c> if this tool can complete action the specified arguments; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanCompleteAction(MouseEventArgs evtArgs)
        {
            return base.CanCompleteAction(evtArgs) || (evtArgs.Button == MouseButtons.Left
                && this.HeadPossibleConnection != null && this.Points.Length >= c_nMIN_NODE_POINTS);
        }

        /// <summary>
        /// Completes Tool Action.
        /// Creates specific node and inserts it into document.
        /// </summary>
        /// <param name="ptsShape">points to create polyline derived node from</param>
        /// <returns>The node.</returns>
        protected override Node CompleteAction(PointF[] ptsShape)
        {
            HistoryManager mgrHistory = this.Controller.Model.HistoryManager;

            if (mgrHistory != null)
                mgrHistory.StartAtomicAction("Insert Node");

            PointF ptStart = GetEndPointLocation(this.TailPossibleConnection, GetStartPoint(false));
            PointF ptEnd = GetEndPointLocation(this.HeadPossibleConnection, this.CurrentPoint);

            PolyLineConnector node = (PolyLineConnector)base.CompleteAction(ptsShape);

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

            // clean up
            this.TailPossibleConnection = null;
            this.HeadPossibleConnection = null;

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

            // End atomic action
            if (mgrHistory != null)
                mgrHistory.EndAtomicAction();

            return node;
        }

        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public override void Draw(Graphics gfx)
        {
            if (this.InAction && this.Points.Length >= 2)
            {
                using (Pen pen = new Pen(Color.FromArgb(CommonUsedValues.HALF_OPAQUE, Color.Black)))
                {
                    // draw rendering helper
                    gfx.DrawLines(pen, this.Points);
                }
            }

            // Head endpoint possible connection
            OutlineConnectionPoint(gfx, this.HeadPossibleConnection);
            
            // Tail endpoint possible connection
            OutlineConnectionPoint(gfx, this.TailPossibleConnection);
        }

        /// <summary>
        /// Creates Node's GraphicsPath from given points.
        /// </summary>
        /// <param name="pts">points to create path from</param>
        /// <returns>path created from given points</returns>
        protected override GraphicsPath CreatePath(PointF[] pts)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLines(pts);

            return path;
        }

        /// <summary>
        /// Creates Polyline derived node.
        /// </summary>
        /// <param name="pts">points to create polyline derived node from.</param>
        /// <returns>The node</returns>
        protected override Node CreateNode(PointF[] pts)
        {
            PolyLineConnector toReturn = new PolyLineConnector(pts);
            ((ConnectorBase)toReturn).LineRoutingEnabled = true;
            SetDecorator(toReturn);

            return toReturn;
        }
        #endregion
    }
}
