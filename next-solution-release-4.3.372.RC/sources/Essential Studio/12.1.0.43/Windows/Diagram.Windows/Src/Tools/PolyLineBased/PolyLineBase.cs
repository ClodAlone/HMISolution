#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Poly line base class
    /// </summary>
    public abstract class PolyLineBase
        : UITool
    {
        #region Class members
        /// <summary>
        /// Array of Points whisch defines 
        /// polyline derived node's shape
        /// </summary>
        private PointF[] m_pts;

        /// <summary>
        /// Head Decorator applied to created node.
        /// </summary>
        private HeadDecorator m_decorHead;

        /// <summary>
        /// Tail Decorator applied to created node.
        /// </summary>
        private TailDecorator m_decorTail;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLineBase"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="name">The name.</param>
        public PolyLineBase(DiagramController controller, string name)
            : base(controller, name)
        { 
        }
        #endregion

        #region Class constants
        /// <summary>
        /// Defines minimum points length 
        /// needed to successfully create Polyline derived node.
        /// </summary>
        protected const int c_nMIN_NODE_POINTS = 2;
        #endregion

        #region Class properties
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

        /// <summary>
        /// Gets helper node used to render tool state.
        /// </summary>
        protected Node RenderingHelper
        {
            get
            {
                Node nodeRenderringHelper = null;

                if (this.Points.Length == 2 && this.Points[0] != this.Points[1])
                {
                    nodeRenderringHelper = new Line(this.Points[0], this.Points[1]);
                    AlterStyle(nodeRenderringHelper);
                }
                else if (this.Points.Length > 2)
                {
                    PointF[] pts = new PointF[this.Points.Length];
                    Array.Copy(this.Points, pts, this.Points.Length);

                    nodeRenderringHelper = CreateNode(pts);
                    AlterStyle(nodeRenderringHelper);
                }

                return nodeRenderringHelper;
            }
        }

        /// <summary>
        /// Gets or sets the points.
        /// </summary>
        /// <value>The points.</value>
        protected PointF[] Points
        {
            get
            {
                if (m_pts == null)
                {
                    m_pts = new PointF[0];
                }

                return m_pts;
            }
            set
            {
                if (m_pts != value)
                {
                    m_pts = value;
                }
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public override void Draw(Graphics gfx)
        {
            Node node = this.RenderingHelper;

            if (this.InAction && node != null)
            {
                node.Draw(gfx);
            }
        }
		public override Tool ProcessMouseDown(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = base.ProcessMouseDown(evtArgs);
            return toolToReturn;
        }
        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            this.CurrentPoint = new Point(evtArgs.X, evtArgs.Y);

            UpdateHelperNode();
            if (this.InAction)
            {
                UpdateCursor(CanAddNode(this.RenderingHelper));
            }

            return this;
        }

        /// <summary>
        /// Processes the mouse up.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The poly line tool.</returns>
        public override Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = this;
            this.CurrentPoint = new Point(evtArgs.X, evtArgs.Y);
            this.CanRender = false;

            if (this.InAction)
            {
                // if MouseButtons.Right --> create polyline derived node
                if (CanCompleteAction(evtArgs))
                {
                    if (this.Points.Length >= c_nMIN_NODE_POINTS)
                    {
                        // set InAction property before adding node to document
                        this.InAction = false;
                        
                        // complete action
                        CompleteAction(this.Points);

                        // clean up
                        this.Points = null;

                        toolToReturn = base.ProcessMouseUp(evtArgs);
                    }
                }               
                else if(evtArgs.Button == MouseButtons.Left)
                {
                    // if MouseButtons.Left --> add current point to shape points
                    AddPoint();
                }
                this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);
            }

            return toolToReturn;
        }

        /// <summary>
        /// Processes the double click.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessDoubleClick(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = this;
            this.CurrentPoint = new Point(evtArgs.X, evtArgs.Y);
            this.CanRender = false;

            if (this.InAction)
            {
                if (this.Points.Length >= c_nMIN_NODE_POINTS)
                {
                    // set InAction property before adding node to document
                    this.InAction = false;
					
                    PointF[] pts = new PointF[this.Points.Length - 1];
                    Array.Copy(this.Points, 0, pts, 0, this.Points.Length - 1);
                    this.Points = pts;
                    // complete action
					if (this.Points.Length > 1)
                    CompleteAction(this.Points);

                    // clean up
                    this.Points = null;

                    toolToReturn = base.ProcessDoubleClick(evtArgs);
                }
            }

            return toolToReturn;
        }

        /// <summary>
        /// Aborts tool actions.
        /// </summary>
        /// <returns>The tool to abort.</returns>
        public override Tool Abort()
        {
            // Clear points
            this.Points = null;

            return base.Abort();
        }

        /// <summary>
        /// Raise the origin changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewOriginEventArgs"/> instance containing the event data.</param>
        protected override void OnOriginChanged(ViewOriginEventArgs evtArgs)
        {
            // update move action
            UpdateHelperNode();

            base.OnOriginChanged(evtArgs);
        }

        #endregion

        #region Class helper methods
        /// <summary>
        /// Update the move action.
        /// </summary>
        protected void UpdateHelperNode()
        {
            this.CanRender = false;

            if (this.InAction && this.Points.Length >= 1)
            {
                Point ptPolyline = GetLastPathPointPosition();
                PointF lastPoint = this.Points[this.Points.Length - 1];

                if (ptPolyline != lastPoint)
                {
                    // update points
                    this.Points[this.Points.Length - 1] = ptPolyline;
                    this.CanRender = true;

                    // update work rects
                    this.WorkRectPrev = this.WorkRect;
                    UpdateWorkRect();
                }
            }
        }

        /// <summary>
        /// Get the last path point position in model coordinates.
        /// </summary>
        /// <returns>Point in model coordinates.</returns>
        protected virtual Point GetLastPathPointPosition()
        {
            // calc polyline node's last point
            PointF ptTemp = this.Controller.ConvertToModelCoordinates(GetCurrentPoint(true));
            Point ptPolyline = Geometry.ConvertPoint(ptTemp);

            return ptPolyline;
        }

        /// <summary>
        /// Creates Node's GraphicsPath from given points.
        /// </summary>
        /// <param name="pts">points to create path from</param>
        /// <returns>path created from given points</returns>
        protected abstract GraphicsPath CreatePath(PointF[] pts);

        /// <summary>
        /// Creates Polyline derived node.
        /// </summary>
        /// <param name="pts">points to create polyline derived node from.</param>
        /// <returns>The node</returns>
        protected abstract Node CreateNode(PointF[] pts);

        /// <summary>
        /// Determines whether this can complete action.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>
        /// <c>true</c> if this tool can complete action the specified arguments; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanCompleteAction(MouseEventArgs evtArgs)
        {
            return evtArgs.Button == MouseButtons.Right;
        }

        /// <summary>
        /// Completes Tool Action.
        /// Creates specific node and inserts it into document.
        /// </summary>
        /// <param name="ptsShape">points to create polyline derived node from</param>
        /// <returns>The node.</returns>
        protected virtual Node CompleteAction(PointF[] ptsShape)
        {
            Node node = null;
            if (!this.InAction)
            {
                System.Drawing.Drawing2D.Matrix mtxScale = this.Controller.Model.DocumentScale.GetScaleTransformation(MeasureUnits.Pixel);
                mtxScale.Invert();
                for (int i = 0; i < ptsShape.Length; i++)
                {
                    ptsShape[i] = Geometry.AppendMatrix(ptsShape[i],mtxScale);
                }
            }
            if (ptsShape[0] != ptsShape[1])
            {
                // create node
                node = CreateNode(ptsShape);

                // append node to document
                if (node != null && CanInsert(node))
                {
                    this.Controller.Model.AppendChild(node);
                }
                else
                {
                    this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);
                }
            }

            return node;
        }

        /// <summary>
        /// Copy Tool Decorator settings to created node's Decorator.
        /// </summary>
        /// <param name="decorTool">Decorator to copy settings from</param>
        /// <param name="decorNode">Decorator to copy settings to</param>
        private new void ApplyDecorator(Decorator decorTool, Decorator decorNode)
        {
            if (decorTool != null)
            {
                if (decorTool.DecoratorShape == DecoratorShape.Custom)
                {
                    decorNode.Load(decorTool.GraphicsPath);
                }
                else
                {
                    decorNode.DecoratorShape = decorTool.DecoratorShape;
                }
				//Apply FillStyle
                decorNode.FillStyle.Color = decorTool.FillStyle.Color;
                decorNode.FillStyle.ForeColor = decorTool.FillStyle.ForeColor;
                decorNode.FillStyle.ForeColorAlphaFactor = decorTool.FillStyle.ForeColorAlphaFactor;
                decorNode.FillStyle.ColorAlphaFactor = decorTool.FillStyle.ColorAlphaFactor;
                decorNode.FillStyle.Type = decorTool.FillStyle.Type;
                decorNode.FillStyle.TextureWrapMode = decorTool.FillStyle.TextureWrapMode;
                decorNode.FillStyle.Texture = decorTool.FillStyle.Texture;
                decorNode.FillStyle.PathBrushStyle = decorTool.FillStyle.PathBrushStyle;
                decorNode.FillStyle.HatchBrushStyle = decorTool.FillStyle.HatchBrushStyle;
                decorNode.FillStyle.GradientAngle = decorTool.FillStyle.GradientAngle;
                decorNode.FillStyle.GradientCenter = decorTool.FillStyle.GradientCenter;
                decorNode.Size = decorTool.Size;
                decorNode.MeasureUnit = MeasureUnits.Pixel;
                decorNode.InheritContainerMeasureUnits = decorTool.InheritContainerMeasureUnits;
            }
        }

        /// <summary>
        /// Copies Tool Decorators settings to created node's decorators.
        /// </summary>
        /// <param name="lineBase">Poly line base</param>
        protected void SetDecorator(LineBase lineBase)
        {
            Decorator decorTool = this.HeadDecorator;
            Decorator decorNode = lineBase.HeadDecorator;

            // apply HeadDecorator settings
            ApplyDecorator(decorTool, decorNode);

            // set decorator's container
            if (decorTool != null)
                decorNode.Container = lineBase;

            // apply TailDecorator settings
            decorTool = this.TailDecorator;
            decorNode = lineBase.TailDecorator;

            ApplyDecorator(decorTool, decorNode);

            // set decorator's container
            if (decorTool != null)
                decorNode.Container = lineBase;
        }

        /// <summary>
        /// Updates the work rect.
        /// </summary>
        protected void UpdateWorkRect()
        {
            RectangleF rcTemp = RectangleF.Empty;

            // get current work rect
            if (this.Points.Length >= 2)
            {
                GraphicsPath pathTemp = CreatePath(this.Points);

                if (pathTemp != null)
                {
                    rcTemp = pathTemp.GetBounds();
                }
            }

            rcTemp = this.Controller.ConvertFromModelToClientCoordinates(rcTemp);
            
            // update currrent work rect
            this.WorkRect = Geometry.ConvertRectangle(rcTemp);
        }

        /// <summary>
        /// Adds CurrentPoint to array of points
        /// defining polyline derived node's shape.
        /// </summary>
        protected void AddPoint()
        {
            PointF ptToAdd = PointF.Empty;
            if (CheckConnectionPossibility(this.CurrentPoint) == null)
                ptToAdd = this.Controller.View.Grid.GetNearestGridPoint(this.CurrentPoint, RulerHeight);            
            else
                ptToAdd = this.CurrentPoint;            
            ptToAdd = this.Controller.ConvertToModelCoordinates(ptToAdd);

            int nPointsCount = this.Points.Length;
            
            // new array
            PointF[] ptsNew;

            if (nPointsCount > 1)
            {
                ptsNew = new PointF[nPointsCount + 1];
                
                // copy all members to new array
                Array.Copy(this.Points, ptsNew, nPointsCount);
                
                // add new member
                ptsNew[nPointsCount] = ptToAdd;
            }
            else
            {
                ptsNew = new PointF[nPointsCount + 2];
                ptsNew[0] = ptToAdd;
                ptsNew[1] = ptToAdd;
            }

            // assign new array points
            this.Points = ptsNew;
        }

        /// <summary>
        /// Checks for connection possibility.
        /// </summary>
        /// <param name="ptTest">The point.</param>
        /// <param name="canConnectWithPort">true, if the port can accept connection</param>
        /// <returns>The connection point</returns>
        protected ConnectionPoint CheckConnectionPossibility(PointF ptTest, out bool canConnectWithPort)
        {
            IEndPointContainer con = this.RenderingHelper is IEndPointContainer ? this.RenderingHelper as IEndPointContainer : null;

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
        #endregion
    }
}