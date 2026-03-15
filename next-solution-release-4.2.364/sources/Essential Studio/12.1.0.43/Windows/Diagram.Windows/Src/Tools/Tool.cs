#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Base class for user interface tools.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A tool is an object that receives input from a controller and
    /// implements a piece of functionality or feature. Tools are helper
    /// objects that plug into a controller. Tools are attached to a
    /// controller using the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.RegisterTool"/>
    /// method. Each tool has a
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Tool.Name"/> which
    /// must be unique within a controller. The
    /// <see cref="Syncfusion.Windows.Forms.Diagram.DiagramController.GetTool"/>
    /// method in the controller can be used to look up a tool by
    /// name.
    /// </para>
    /// <para>
    /// Activation and deactivation of tools is coordinated by the controller.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controller"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View"/>
    /// </remarks>
    public abstract class Tool
    {
        #region Class members
        /// <summary>
        /// Store rectangle to display on rulers.
        /// </summary>
        protected System.Drawing.Rectangle m_rulerDisplayRect;
        private System.Drawing.Rectangle m_rectWork;
        private System.Drawing.Rectangle m_rectWorkPrev;
        private bool m_bSingleActionTool = true;
        private bool m_bInAction;
        private Cursor m_cursorTool = Cursors.Default;
        private Cursor m_cursorAction = Cursors.Default;
        private Cursor m_cursorCurrent;
        private Point m_startingPoint;
        private Point m_currentPoint;

        /// <summary>
        /// Preceding tool.
        /// </summary>
        protected Tool m_toolPreceding;
        private DiagramController m_controller;
        private string m_name = string.Empty;
        private bool m_bCanRender;

        /// <summary>
        /// Start origin position to calc origin offset.
        /// </summary>
        protected PointF m_ptStartOrigin;
        #endregion

        #region Class initialilize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Tool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="name">The name.</param>
        public Tool(DiagramController controller, string name)
        {
            m_controller = controller;
            m_name = name;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance can render.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can render; otherwise, <c>false</c>.
        /// </value>
        protected bool CanRender
        {
            get { return m_bCanRender; }
            set { m_bCanRender = value; }
        }

        /// <summary>
        /// Gets current work refresh rectangle.
        /// </summary>
        /// <remarks>
        /// Used primarily by controller for updating viewer.
        /// </remarks>
        public System.Drawing.Rectangle ToolWorkRect
        {
            get
            {
                System.Drawing.Rectangle rectToReturn = System.Drawing.Rectangle.Empty;

                if (this.CanRender)
                {
                    if (!this.WorkRect.IsEmpty && !this.WorkRectPrev.IsEmpty)
                    {
                        rectToReturn = System.Drawing.Rectangle.Union(this.WorkRect, this.WorkRectPrev);
                    }
                    else if (this.WorkRectPrev.IsEmpty && !this.WorkRect.IsEmpty)
                    {
                        rectToReturn = this.WorkRect;
                    }
                    else if (!this.WorkRectPrev.IsEmpty && this.WorkRect.IsEmpty)
                    {
                        rectToReturn = this.WorkRectPrev;
                    }
                }

                return rectToReturn;
            }
        }

        /// <summary>
        /// Gets or sets the previous work refresh rectangle.
        /// </summary>
        /// <value>The previous work rectangle.</value>
        protected System.Drawing.Rectangle WorkRectPrev
        {
            get { return m_rectWorkPrev; }
            set { m_rectWorkPrev = value; }
        }

        /// <summary>
        /// Gets or sets the work refresh rectangle.
        /// </summary>
        /// <value>The work rectangle.</value>
        public System.Drawing.Rectangle WorkRect
        {
            get { return m_rectWork; }
            set { m_rectWork = value; }
        }

        /// <summary>
        /// Gets the height of the ruler.
        /// </summary>
        /// <value>The height of the ruler.</value>
        protected int RulerHeight
        {
            get { return this.Controller.Viewer.ShowRulers ? this.Controller.Viewer.RulersHeight : 0; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        /// <summary>
        /// Gets the preceding tool.
        /// </summary>
        /// <value>The preceding tool.</value>
        public Tool PrecedingTool
        {
            get
            {
                return m_toolPreceding;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Tool is currently in action.
        /// </summary>
        public bool InAction
        {
            get
            {
                return m_bInAction;
            }
            set
            {
                m_bInAction = value;

                if (value)
                {
                    m_cursorCurrent = this.ActionCursor;
                }
                else
                {
                    m_cursorCurrent = this.ToolCursor;
                }
            }
        }

        /// <summary>
        /// Gets or sets Tool Action start point in client coordinates.
        /// </summary>
        protected Point StartPoint
        {
            get
            {
                return m_startingPoint;
            }
            set
            {
                if (m_startingPoint != value)
                    m_startingPoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the current point in client coordinates.
        /// </summary>
        /// <value>The current point.</value>
        protected Point CurrentPoint
        {
            get
            {
                return m_currentPoint;
            }
            set
            {
                m_currentPoint = value;
            }
        }

        /// <summary>
        /// Gets the Tool Controller.
        /// </summary>
        public DiagramController Controller
        {
            get
            {
                return m_controller;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tool lifetime should be single action.
        /// SingleActionTool restores preceding tool
        /// after performing its action.
        /// </summary>
        public bool SingleActionTool
        {
            get
            {
                return m_bSingleActionTool;
            }
            set
            {
                m_bSingleActionTool = value;
            }
        }

        /// <summary>
        /// Gets or sets Tool Cursor
        /// </summary>
        public Cursor ToolCursor
        {
            get
            {
                return m_cursorTool;
            }
            set
            {
                if (m_cursorTool != value)
                    m_cursorTool = value;
            }
        }

        /// <summary>
        /// Gets or setsTool Action cursor.
        /// </summary>
        public Cursor ActionCursor
        {
            get
            {
                return m_cursorAction;
            }
            set
            {
                if (IsImageCursor(value) || (IsImageCursor(m_cursorAction) && m_cursorAction != value) || (m_cursorAction.ToString() != value.ToString()))
                    m_cursorAction = value;
            }
        }

        /// <summary>
        /// Gets the property is used by controller to update current viewer cursor.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Cursor CurrentToolCursor
        {
            get { return m_cursorCurrent; }
        }

        /// <summary>
        /// Gets or sets helper property holding current cursor.
        /// </summary>
        protected Cursor CurrentCursor
        {
            get 
            { 
                return m_cursorCurrent; 
            }
            set
            {
                if (m_cursorCurrent != value)
                    m_cursorCurrent = value;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Aborts tool actions.
        /// </summary>
        /// <returns>The tool to abort.</returns>
        public virtual Tool Abort()
        {
            this.InAction = false;

            // reset working rects
            this.WorkRectPrev = System.Drawing.Rectangle.Empty;
            this.WorkRect = System.Drawing.Rectangle.Empty;

            Tool toolToReturn;

            if (this.SingleActionTool)
            {
                toolToReturn = m_toolPreceding;
            }
            else
            {
                toolToReturn = this;
            }

            return toolToReturn;
        }

        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public virtual void Draw(Graphics gfx)
        { 
        }

        /// <summary>
        /// Activates the tool.
        /// </summary>
        public virtual void ActivateTool()
        {
            m_cursorCurrent = this.ToolCursor;

            // sign to origin change event
            this.Controller.Viewer.EventSink.OriginChanged += new ViewOriginEventHandler(this.OnOriginChanged);
        }

        /// <summary>
        /// Deactivates the tool.
        /// </summary>
        public virtual void DeactivateTool()
        {
            this.CanRender = false;
            
            // reset working rects
            this.WorkRectPrev = System.Drawing.Rectangle.Empty;
            this.WorkRect = System.Drawing.Rectangle.Empty;

            // remove origin change event
            this.Controller.Viewer.EventSink.OriginChanged -= new ViewOriginEventHandler(this.OnOriginChanged);
        }

        /// <summary>
        /// Processes the mouse down.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public virtual Tool ProcessMouseDown(MouseEventArgs evtArgs)
        {
            // update start origin point
            m_ptStartOrigin = m_controller.View.Origin;

            if (!this.InAction)
            {
                this.CanRender = false;
                
                // reset working rects
                this.WorkRectPrev = System.Drawing.Rectangle.Empty;
                this.WorkRect = System.Drawing.Rectangle.Empty;
				double dMagnification = this.Controller.View.Magnification / 100f;

                float fHrzSpacing = MeasureUnitsConverter.ToPixelX(this.Controller.View.Grid.HorizontalSpacing, this.Controller.View.Grid.MeasureUnit);
                float fVertSpacing = MeasureUnitsConverter.ToPixelX(this.Controller.View.Grid.VerticalSpacing, this.Controller.View.Grid.MeasureUnit);

                // Scene spacing
                double dSceneVertSpacing = fVertSpacing * dMagnification;
                double dSceneHorzSpacing = fHrzSpacing * dMagnification;
                Point ptCur = Point.Empty;
                if (CheckConnectionPossibility(evtArgs.Location) == null && this.Controller.View.Grid.SnapToGrid)              
                    ptCur = new Point(evtArgs.X - (int)(this.RulerHeight % dSceneHorzSpacing), evtArgs.Y - (int)(this.RulerHeight % dSceneVertSpacing));                
                else
                    ptCur = evtArgs.Location;
                // init start point
                this.StartPoint = ptCur;
                
                // init current point
                this.CurrentPoint = ptCur;
            }

            return this;
        }

        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public virtual Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            // Update current point
            this.CurrentPoint = new Point(evtArgs.X, evtArgs.Y);
            return this;
        }

        /// <summary>
        /// Processes the mouse up.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public virtual Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            return RestorePreviousTool(evtArgs);
        }

        /// <summary>
        /// Processes the key up.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public virtual Tool ProcessKeyUp(KeyEventArgs evtArgs)
        {
            return this;
        }

        /// <summary>
        /// Processes the key press.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public virtual Tool ProcessKeyPress(KeyPressEventArgs evtArgs)
        {
            return this;
        }

        /// <summary>
        /// Processes the key down.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public virtual Tool ProcessKeyDown(KeyEventArgs evtArgs)
        {
            return this;
        }

        /// <summary>
        /// Processes the click.
        /// </summary>
        /// <returns>The tool.</returns>
        public virtual Tool ProcessClick()
        {
            return this;
        }

        /// <summary>
        /// Processes the double click.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public virtual Tool ProcessDoubleClick(MouseEventArgs evtArgs)
        {
             return RestorePreviousTool(evtArgs);
        }

        /// <summary>
        /// Get selection list using model EnableSelectionListSubstitute property value.
        /// </summary>
        /// <returns>Selection list.</returns>
        public NodeCollection GetSelectionList()
        {
            return (this.Controller.Model.EnableSelectionListSubstitute && this.Controller.SelectionList.Count > 1)
                ? this.Controller.View.SelectionListSubstitute : this.Controller.View.SelectionList;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Raise the origin changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewOriginEventArgs"/> instance containing the event data.</param>
        protected virtual void OnOriginChanged(ViewOriginEventArgs evtArgs)
        { 
        }

        /// <summary>
        /// Gets the local point without node parents transformations.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="ptLocation">The given point.</param>
        /// <returns>The local point.</returns>
        protected PointF GetLocalPoint(Node parent, PointF ptLocation)
        {
            PointF[] ptPoint = new PointF[1] { ptLocation };

            if (parent != null)
            {
                Matrix mtxTransformation = parent.GetTransformations();
                parent.AppendFlipTransforms(mtxTransformation);
                mtxTransformation.Multiply(HandlesHitTesting.GetParentsTransformations(parent), MatrixOrder.Append);

                mtxTransformation.Invert();

                mtxTransformation.TransformPoints(ptPoint);
            }

            return ptPoint[0];
        }

        /// <summary>
        /// Get the port bounds.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="fZoomFactor">Graphics zoom factor.</param>
        /// <returns>Bounds of given port.</returns>
        protected RectangleF GetPortBounds(ConnectionPoint port, float fZoomFactor)
        {
            // calc port position
            PointF[] pts = new PointF[1];

            SizeF szFrame = Size.Empty;

            if (port is CentralPort)
            {
                // frame size equals port container node bounding rect size
                Node nodePortCtnr = port.Container;

                if (nodePortCtnr != null)
                {
                    // get frame bounds
                    RectangleF rect = ((IUnitIndependent)nodePortCtnr).GetBoundingRectangle(MeasureUnits.Pixel, false);
                    szFrame = rect.Size;

                    // get port position
                    SizeF szTemp = ((IUnitIndependent)nodePortCtnr).GetSize(MeasureUnits.Pixel);

                    PointF ptTemp = PointF.Empty;
                    ptTemp.X = szTemp.Width / 2;
                    ptTemp.Y = szTemp.Height / 2;

                    pts[0] = ptTemp;
                }
            }
            else
            {
                // get port position
                pts[0] = port.GetPosition();
                
                // get port frame size
                float fTemp = (CommonUsedValues.RESIZE_HANDLE_SIZE + 2) / fZoomFactor;
                szFrame.Width = fTemp;
                szFrame.Height = fTemp;
            }

            // get port containers transformations
            Matrix matrixParent = port.Container.GetTransformations();
            port.Container.AppendFlipTransforms(matrixParent);

            Matrix matrixTemp = HandlesHitTesting.GetParentsTransformations(port.Container);
            matrixParent.Multiply(matrixTemp, MatrixOrder.Append);

            matrixParent.TransformPoints(pts);
            return Geometry.CreateRect(pts[0], new SizeF(szFrame.Width, szFrame.Height));
        }

        /// <summary>
        /// Checks whether node could be inserted into document.
        /// </summary>
        /// <param name="nodeToInsert">Node to insert</param>
        /// <returns>true, if can insert.</returns>
        protected bool CanInsert(Node nodeToInsert)
        {
            RectangleF rectBounding = ((IUnitIndependent)nodeToInsert).GetBoundingRectangle(MeasureUnits.Pixel, false);
            rectBounding = MeasureUnitsConverter.Convert(rectBounding, MeasureUnits.Pixel, this.Controller.Model.MeasurementUnits);
            bool bCanInsert = !rectBounding.Size.IsEmpty;

            if (bCanInsert && this.Controller.Model.BoundaryConstraintsEnabled)
            {
                bCanInsert = this.Controller.Model.Bounds.Contains(rectBounding);
            }

            return bCanInsert;
        }

        /// <summary>
        /// Updates the cursor to current tool state.
        /// </summary>
        /// <param name="bCanDo">if set to <c>true</c> cursor change to current tool cursor, else to NoCursor.</param>
        protected void UpdateCursor(bool bCanDo)
        {
            // update cursor
            if (bCanDo)
            {
                this.CurrentCursor = this.ActionCursor;
            }
            else
            {
                this.CurrentCursor = Cursors.No;
            }
        }

        /// <summary>
        /// Check whether the given cursor is image cursor or not.
        /// </summary>
        /// <param name="cursor">Cursor</param>
        /// <returns>true, if the given cursor is image cursor</returns>
        internal bool IsImageCursor(Cursor cursor)
        {
            return cursor.Tag != null && cursor.Tag.ToString() == "ImageCursor";
        }

        /// <summary>
        /// Get point without parent transformations.
        /// </summary>
        /// <param name="ptCur">The given point.</param>
        /// <param name="node">The node.</param>
        /// <returns>The transformation points.</returns>
        protected PointF AppendParentTransformations(PointF ptCur, Node node)
        {
            PointF[] pts = new PointF[] { ptCur };
            Node parent = node.Parent as Node;

            if (parent != null)
            {
                Matrix mtx = HandlesHitTesting.GetParentsTransformations(node);
                mtx.Invert();

                mtx.TransformPoints(pts);
            }

            return pts[0];
        }

        /// <summary>
        /// Gets the rotation angle.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The rotation angle.</returns>
        protected float GetFullRotationAngle(Node node)
        {
            float fAngle = node.RotationAngle;
            Node parent = node.Parent as Node;

            if (parent != null)
            {
                fAngle += GetFullRotationAngle(parent);
            }

            return fAngle;
        }

        /// <summary>
        /// Quites set boundary value without record in history and calling sink events.
        /// </summary>
        /// <param name="bBoundaryConstraintsEnabled">if set to <c>true</c> boundary constraints enabled.</param>
        protected void QuiteBoundarySet(bool bBoundaryConstraintsEnabled)
        {
            Model model = this.Controller.Model;

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

        /// <summary>
        /// Quites set line bridging flag without record in history and calling sink events.
        /// </summary>
        /// <param name="bLineBridgingEnabled">if set to <c>true</c> line bridging enabled.</param>
        protected void QuiteBridgingSet(bool bLineBridgingEnabled)
        {
            Model model = this.Controller.Model;

            // Quite set boundary constrains
            // Needed for property change pin point and offset
            // without checking only one changes
            if (model != null)
            {
                model.EventSink.Pause();
                model.HistoryManager.Pause();
                model.LineBridgingEnabled = bLineBridgingEnabled;
                model.HistoryManager.Resume();
                model.EventSink.Resume();
            }
        }

        /// <summary>
        /// Gets the point offset.
        /// </summary>
        /// <param name="ptPoint">The pt point.</param>
        /// <param name="node">The node.</param>
        /// <returns>The point offset.</returns>
        protected SizeF GetPointOffset(PointF ptPoint, Node node)
        {
            PointF ptPinPoint = ((IUnitIndependent)node).GetPinPoint(MeasureUnits.Pixel);
            SizeF szPinOffset = ((IUnitIndependent)node).GetPinPointOffset(MeasureUnits.Pixel);

            PointF ptUpperLeft = new PointF(ptPinPoint.X - szPinOffset.Width, ptPinPoint.Y - szPinOffset.Height);

            return new SizeF(ptPoint.X - ptUpperLeft.X, ptPoint.Y - ptUpperLeft.Y);
        }

        /// <summary>
        /// Snaps and updates snapped point.
        /// </summary>
        /// <param name="ptSnapping">snapping point</param>
        /// <param name="szOffset">moving entity bounding rectangle left-top point offset</param>
        /// <returns>snapped point</returns>
        protected PointF SmartSnap(PointF ptSnapping, SizeF szOffset)
        {
            PointF ptToReturn = new PointF(ptSnapping.X - szOffset.Width, ptSnapping.Y - szOffset.Height);

            if(this.Controller.ActiveTool is MoveTool)
                ptToReturn = this.Controller.View.Grid.GetNearestGridPoint(ptToReturn);
            else
                ptToReturn = this.Controller.View.Grid.GetNearestGridPoint(ptToReturn,RulerHeight);

            ptToReturn.X += szOffset.Width;
            ptToReturn.Y += szOffset.Height;

            return ptToReturn;
        }

        /// <summary>
        /// Append alters the style to node.
        /// </summary>
        /// <param name="node">The node.</param>
        internal void AlterStyle(Node node)
        {
            if (node is ICompositeNode)
            {
                ICompositeNode nodeComposite = (ICompositeNode)node;
                int nLength = nodeComposite.ChildCount;
                Node nodeChild;

                for (int nCounter = 0; nCounter < nLength; nCounter++)
                {
                    nodeChild = nodeComposite.GetChild(nCounter);

                    if (nodeChild != null)
                    {
                        AlterStyle(nodeChild);
                    }
                }
            }
            else if (node is FilledPath)
            {
                AlterFillStyle(((FilledPath)node).FillStyle);
            }
            else if (node is TextNode)
            {
                TextNode nodeTemp = (TextNode)node;
                AlterFillStyle(nodeTemp.FontColorStyle);
                AlterFillStyle(nodeTemp.BackgroundStyle);
            }

            if (node is LineBase)
            {
                LineBase decoratorContainer = (LineBase)node;
                
                // alter fillstyles
                AlterFillStyle(decoratorContainer.HeadDecorator.FillStyle);
                AlterFillStyle(decoratorContainer.TailDecorator.FillStyle);
                
                // alter linestyles
                AlterLineStyle(decoratorContainer.HeadDecorator.LineStyle);
                AlterLineStyle(decoratorContainer.TailDecorator.LineStyle);
            }

            AlterLineStyle(node.LineStyle);

            ShadowStyle shadowStyle = node.ShadowStyle;
            shadowStyle.Visible = false;

            node.DrawPorts = false;
        }

        /// <summary>
        /// Check if node is contained in model.
        /// </summary>
        /// <param name="node">Node to check.</param>
        /// <param name="mtxParentTransformations">The matrix of parent transformations.</param>
        /// <returns>true, if node inside model</returns>
        protected bool CheckBoundaryConstraints(Node node, Matrix mtxParentTransformations)
        {
            bool bSuccess = true;
            Model document = this.Controller.Model;

            if (document != null && document.BoundaryConstraintsEnabled)
            {
                PointF ptPinPoint = ((IUnitIndependent)node).GetPinPoint(MeasureUnits.Pixel);
                SizeF szSize = ((IUnitIndependent)node).GetSize(MeasureUnits.Pixel);
                RectangleF rectBounding = new RectangleF(PointF.Empty, szSize);
                Matrix mtxNode = node.GetTransformations();
                node.AppendFlipTransforms(mtxNode);

                PointF[] pts = new PointF[]
                    {
                       rectBounding.Location,
                       new PointF( rectBounding.Right, rectBounding.Top ),
                       new PointF( rectBounding.Left, rectBounding.Bottom ),
                       new PointF( rectBounding.Right, rectBounding.Bottom )
                    };

                PointF[] ptsPinPoint = new PointF[] { ptPinPoint };

                mtxNode.TransformPoints(pts);

                // append parent transformations
                if (mtxParentTransformations != null)
                {
                    mtxParentTransformations.TransformPoints(ptsPinPoint);
                    mtxParentTransformations.TransformPoints(pts);
                }

                rectBounding = Geometry.CreateRect(pts);
                rectBounding = RectangleF.Union(rectBounding, new RectangleF(ptsPinPoint[0], SizeF.Empty));

                // check for bounds contains
                RectangleF rcMBounds = RectangleF.Empty;
                rcMBounds.Size = MeasureUnitsConverter.Convert(document.LogicalSize, document.MeasurementUnits, MeasureUnits.Pixel);
                bSuccess = rcMBounds.Contains(rectBounding);
            }

            return bSuccess;
        }

        /// <summary>
        /// Check if node can be added to diagram.
        /// </summary>
        /// <param name="renderingHelper">The rendering helper node.</param>
        /// <returns>true, if can add node.</returns>
        protected bool CanAddNode(Node renderingHelper)
        {
            bool bCanAddNode = true;
            if (renderingHelper != null)
            {
                bCanAddNode = CheckBoundaryConstraints(renderingHelper, new Matrix());
            }
            return bCanAddNode;
        }

        /// <summary>
        /// Make the control node snapshot.
        /// </summary>
        /// <param name="nodeCtrl">The control node .</param>
        /// <returns>Snapshot of given control node.</returns>
        protected Image MakeControlNodeSnapshot(ControlNode nodeCtrl)
        {
            SizeF szNode = ((IUnitIndependent)nodeCtrl).GetSize(MeasureUnits.Pixel);
            szNode = Controller.ConvertFromModelToClientCoordinates(szNode);
            
            // MAKE SNAPSHOT
            Image img = ControlSnapshot.PrintControl(nodeCtrl.HostingControl, Geometry.ConvertSize(szNode));
            
            return img;
        }

        /// <summary>
        /// Get the inside node composite node.
        /// </summary>
        /// <param name="ptLocation">The point to check.</param>
        /// <returns>Node inside composite node what contain given point.</returns>
        protected Node GetInsideNodeAt(PointF ptLocation)
        {
            return GetInsideNodeAt(this.Controller.Model, ptLocation);
        }

        /// <summary>
        /// Get the origin offset from mouse down in model coordinates.
        /// </summary>
        /// <returns>Origin offset.</returns>
        protected SizeF GetOriginOffset()
        {
            PointF ptCurOrigin = this.Controller.View.Origin;
            SizeF szToReturn = new SizeF(m_ptStartOrigin.X - ptCurOrigin.X, m_ptStartOrigin.Y - ptCurOrigin.Y);

            return szToReturn;
        }

        /// <summary>
        /// Get start mouse position in client coordinates with append origin offset.
        /// </summary>
        /// <param name="bSnapToGrid">if set to <c>true</c> point will snap to grid before return.</param>
        /// <returns>Start mouse position in client coordinates with origin offset.</returns>
        protected Point GetStartPoint(bool bSnapToGrid)
        {
            // calc origin offset
            PointF ptOrigin = this.Controller.ConvertFromModelToClientCoordinates(this.Controller.View.Origin);
            PointF ptPrevOrigin = this.Controller.ConvertFromModelToClientCoordinates(m_ptStartOrigin);
            SizeF szOriginOffset = new SizeF(ptPrevOrigin.X - ptOrigin.X, ptPrevOrigin.Y - ptOrigin.Y);

            // append origin offset only to start point, because current point in client coordinates
            PointF ptStartPoint = new PointF(this.StartPoint.X + szOriginOffset.Width, this.StartPoint.Y + szOriginOffset.Height);

            // snap points to grid if need
            if (bSnapToGrid)
            {
                ptStartPoint = this.Controller.View.Grid.GetNearestGridPoint(ptStartPoint,RulerHeight);
            }

            // create bounds used start and current points
            return Geometry.ConvertPoint(ptStartPoint);
        }

        /// <summary>
        /// Get current mouse position in client coordinates with origin offset.
        /// </summary>
        /// <param name="bSnapToGrid">if set to <c>true</c> point will snap to grid before return.</param>
        /// <returns>Current mouse position in client coordinates.</returns>
        protected Point GetCurrentPoint(bool bSnapToGrid)
        {
            PointF ptCurrentPoint = this.CurrentPoint;

            // snap points to grid if need
            if (bSnapToGrid)
            {
                ptCurrentPoint = this.Controller.View.Grid.GetNearestGridPoint(ptCurrentPoint,RulerHeight);               
            }

            // create bounds used start and current points
            return Geometry.ConvertPoint(ptCurrentPoint);
        }

        /// <summary>
        /// Get rectangle created from start and current point with
        /// append origin offset.
        /// </summary>
        /// <param name="bSnapToGrid">if set to <c>true</c> start and current point will snap before generate frame rectangle.</param>
        /// <returns>The frame rect</returns>
        protected RectangleF GetFrameRectangle(bool bSnapToGrid)
        {
            RectangleF rcFrame;

            if (bSnapToGrid)
            {
                rcFrame = Geometry.CreateRect(GetSnapStartPoint(), GetSnapCurrentPoint());
            }
            else
            {
                rcFrame = Geometry.CreateRect(GetStartPoint(false), GetCurrentPoint(false));
            }

            // create bounds used start and current points
            return rcFrame;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Restore the previous Tool
        /// </summary>
        /// <param name="evtArgs"></param>
        /// <returns>the tool</returns>
        private Tool RestorePreviousTool(MouseEventArgs evtArgs)
        {
            // Update current point
            this.CurrentPoint = new Point(evtArgs.X, evtArgs.Y);

            // restore cursor
            //Fix SD209, no need to update the diagram(viewer) cursor.
            //We can get the current cursor of viewer by ActiveTool.ActionCursor

            //this.Controller.Cursor = this.ToolCursor;

            Tool toolToReturn = null;

            if (this.SingleActionTool)
            {
                toolToReturn = m_toolPreceding;
            }
            else if (toolToReturn == null)
            {
                toolToReturn = this;
            }
            return toolToReturn;
        }
        /// <summary>
        /// Get snap start mouse position in client coordinates with append origin offset.
        /// </summary>
        /// <returns>Start mouse position in client coordinates with origin offset.</returns>
        protected PointF GetSnapStartPoint()
        {
            // calc origin offset
            PointF ptOrigin = this.Controller.ConvertFromModelToClientCoordinates(this.Controller.View.Origin);
            PointF ptPrevOrigin = this.Controller.ConvertFromModelToClientCoordinates(m_ptStartOrigin);
            SizeF szOriginOffset = new SizeF(ptPrevOrigin.X - ptOrigin.X, ptPrevOrigin.Y - ptOrigin.Y);

            // append origin offset only to start point, because current point in client coordinates
            PointF ptStartPoint = new PointF(this.StartPoint.X + szOriginOffset.Width, this.StartPoint.Y + szOriginOffset.Height);

            // snap points to grid if need
            ptStartPoint = this.Controller.View.Grid.GetNearestGridPoint(ptStartPoint,this.RulerHeight);

            // create bounds used start and current points
            return ptStartPoint;
        }

        /// <summary>
        /// Get snap current mouse position in client coordinates with origin offset.
        /// </summary>
        /// <returns>Current mouse position in client coordinates.</returns>
        protected PointF GetSnapCurrentPoint()
        {
            return this.Controller.View.Grid.GetNearestGridPoint(this.CurrentPoint,this.RulerHeight);
        }

        /// <summary>
        /// Get the node inside composite node.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="ptLocation">The point to check.</param>
        /// <returns>Node inside composite node what contain given point.</returns>
        private Node GetInsideNodeAt(ICompositeNode container, PointF ptLocation)
        {
            Node nodeToReturn = null;
            PointF[] pts = new PointF[] { ptLocation };

            Node node;
            ICompositeNode compositeNode;
            Node parent = container as Node;

            // append invert parent transformation to get a child.
            if (parent != null)
            {
                Matrix mtxParent = parent.GetTransformations();
                parent.AppendFlipTransforms(mtxParent);
                mtxParent.Invert();
                mtxParent.TransformPoints(pts);
            }

            for (int i = 0, nLength = container.ChildCount; i < nLength; i++)
            {
                node = container.GetChild(i);

                if (node.ContainsPoint(pts[0]))
                {
                    compositeNode = node as ICompositeNode;
                    if (compositeNode != null)
                    {
                        nodeToReturn = GetInsideNodeAt(compositeNode, pts[0]);
                    }
                    else
                    {
                        nodeToReturn = node;
                    }
                }
            }

            return nodeToReturn;
        }

        /// <summary>
        /// Alters the fill style to node.
        /// </summary>
        /// <param name="styleFill">The style fill.</param>
        private void AlterFillStyle(FillStyle styleFill)
        {
            if (styleFill == null)
                throw new ArgumentNullException("styleFill");

            styleFill.ForeColorAlphaFactor = styleFill.ForeColorAlphaFactor / 2;
            styleFill.ColorAlphaFactor = styleFill.ColorAlphaFactor / 2;
        }

        /// <summary>
        /// Alters the line style to node.
        /// </summary>
        /// <param name="styleLine">The style line.</param>
        private void AlterLineStyle(LineStyle styleLine)
        {
            if (styleLine == null)
                throw new ArgumentNullException("styleLine");

            styleLine.LineColor = Color.FromArgb(styleLine.LineColor.A / 2, styleLine.LineColor);
        }

        /// <summary>
        /// Draws render helper
        /// </summary>
        /// <param name="renderHelper">The render helper.</param>
        /// <param name="gfx">Graphics to draw on.</param>
        protected virtual void DrawRenderHelper(Node renderHelper, Graphics gfx)
        {
            // append parent's transforms
            Matrix mtxTemp = HandlesHitTesting.GetParentsTransformations(renderHelper);
            HandlesHitTesting.AppendScaleTransforms(renderHelper, mtxTemp, true);
            gfx.MultiplyTransform(mtxTemp);

            System.Drawing.Rectangle renderHelperBounds = System.Drawing.Rectangle.Empty;
            SizeF renderHelperSize = MeasureUnitsConverter.Convert(new SizeF(renderHelper.Size.Width, renderHelper.Size.Height), renderHelper.MeasurementUnit, MeasureUnits.Pixel);
            renderHelperBounds = new System.Drawing.Rectangle(Point.Empty, new Size((int)renderHelperSize.Width, (int)renderHelperSize.Height));
            // append node transformations
            Matrix mtx = renderHelper.GetTransformations();
            renderHelper.AppendFlipTransforms(mtx);
            gfx.MultiplyTransform(mtx);

            DrawRenderHelper(renderHelper, gfx, renderHelperBounds);
        }

        /// <summary>
        /// Draws render helper
        /// </summary>
        /// <param name="renderHelper">The render helper.</param>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="bounds">The bounds of render helper.</param>
        protected void DrawRenderHelper(Node renderHelper, Graphics gfx, System.Drawing.Rectangle bounds)
        {
            RenderingHelperStyle renderingHelperStyle = RenderingHelperStyle.GhostCopy;

            if (this is MoveTool)
                renderingHelperStyle = Controller.DraggingStyle;
            else if (this is ResizeTool || this is HandleMoveTool)
                renderingHelperStyle = Controller.ResizingStyle;
            else if (this is RotateTool)
                renderingHelperStyle = Controller.RotatingStyle;

            switch (renderingHelperStyle)
            {
                case RenderingHelperStyle.DashedOutline:
                    //draw dashed ouline
                    LineStyle dashStyle = new LineStyle();
                    dashStyle.DashStyle = DashStyle.Dash;
                    using (Pen pen = dashStyle.CreatePen())
                    {
                        if (renderHelper is LineBase || renderHelper is ConnectorBase)
                            gfx.DrawPath(pen, renderHelper.GraphicsPath);
                        else
                            gfx.DrawRectangle(pen, bounds);
                    }
                    break;
                case RenderingHelperStyle.SolidOutline:
                    //draw solid outline
                    LineStyle solidStyle = new LineStyle();
                    using (Pen pen = solidStyle.CreatePen())
                    {
                        if (renderHelper is LineBase || renderHelper is ConnectorBase)
                            gfx.DrawPath(pen, renderHelper.GraphicsPath);
                        else
                            gfx.DrawRectangle(pen, bounds);
                    }
                    break;
                case RenderingHelperStyle.FilledRectangle:
                    //draw filled rectangle
                    FillStyle filledStyle = new FillStyle();
                    if (renderHelper is LineBase || renderHelper is ConnectorBase)
                        filledStyle.Color = Color.Black;
                    else
                    {
                        filledStyle.Color = Color.Gray;
                        filledStyle.Type = FillStyleType.LinearGradient;
                    }
                    filledStyle.ColorAlphaFactor = filledStyle.ColorAlphaFactor / 2;
                    filledStyle.ForeColorAlphaFactor = filledStyle.ForeColorAlphaFactor / 2;
                    using (Brush brush = filledStyle.CreateBrush(gfx, bounds))
                    {
                        if (renderHelper is LineBase || renderHelper is ConnectorBase)
                            gfx.DrawPath(new Pen(brush), renderHelper.GraphicsPath);
                        else
                        {
                            gfx.FillRectangle(brush, bounds);
                            LineStyle border = new LineStyle();
                            using (Pen pen = border.CreatePen())
                            {
                                gfx.DrawRectangle(pen, bounds);
                            }
                        }
                    }
                    break;
            }
        }
        #endregion

        #region Connections helpers methods
        /// <summary>
        /// Updates the port refresh rectangle.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="rectPortFrame">The port frame.</param>
        protected void UpdatePortRefreshRect(ConnectionPoint port, ref System.Drawing.Rectangle rectPortFrame)
        {
            if (port != null)
            {
                float fZoomFactor = this.Controller.View.Magnification / 100f;
                RectangleF rectTemp = GetPortBounds(port, fZoomFactor);
                rectTemp.Inflate(2f / fZoomFactor, 2f / fZoomFactor);

                rectPortFrame = Geometry.ConvertRectangle(rectTemp);
                rectPortFrame = this.Controller.ConvertFromModelToClientCoordinates(rectPortFrame);

                this.WorkRect = Geometry.UpdateRectWith(this.WorkRect, rectPortFrame);
            }
            else
            {
                if (!rectPortFrame.IsEmpty)
                {
                    this.WorkRect = Geometry.UpdateRectWith(this.WorkRect, rectPortFrame);
                    rectPortFrame = System.Drawing.Rectangle.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the end point location.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="ptLocation">The location.</param>
        /// <returns>The end point location.</returns>
        protected PointF GetEndPointLocation(ConnectionPoint port, PointF ptLocation)
        {
            PointF ptToReturn;

            if (port == null)
            {
                ptToReturn = this.Controller.View.Grid.GetNearestGridPoint(ptLocation,RulerHeight);
                ptToReturn = this.Controller.ConvertToModelCoordinates(ptToReturn);
               
            }
            else
            {
                ptToReturn = HandlesHitTesting.GetPortPosition(port);
            }

            return ptToReturn;
        }

        /// <summary>
        /// Checks for connection possibility.
        /// </summary>
        /// <param name="ptTest">The point.</param>
        /// <returns>The connection point</returns>
        protected ConnectionPoint CheckConnectionPossibility(PointF ptTest)
        {
            // Convert to model coordinates
            ptTest = this.Controller.ConvertToModelCoordinates(ptTest);

            ConnectionPoint portToReturn = HandlesHitTesting.GetConnectionPointAtPoint(this.Controller.Model.Nodes, ptTest); 

            // check for connection to center port under mouse node
            if (portToReturn == null)
            {
                portToReturn = CheckConnectionCenterPort(ptTest);
            }

            if (portToReturn != null && portToReturn.Connections.Count >= portToReturn.ConnectionsLimit)
                portToReturn = null;

            return portToReturn;
        }

        /// <summary>
        /// Checks the for possibility connect to center port.
        /// </summary>
        /// <param name="ptPoint">The point.</param>
        /// <returns>The connection point</returns>
        protected ConnectionPoint CheckConnectionCenterPort(PointF ptPoint)
        {
            ConnectionPoint portToReturn = null;

            Node headUnderNode = null;
            
            // check for head endpoint
            NodeCollection nodes = this.Controller.GetAllNodesAtPoint(this.Controller.Model, ptPoint,true);

            foreach (Node node in nodes)
            {
                if (node.EnableCentralPort)
                {
                    headUnderNode = node;
                    break;
                }
            }
            if (headUnderNode != null && headUnderNode.EnableCentralPort)
            {
                portToReturn = headUnderNode.CentralPort;
            }

            return portToReturn;
        }

        /// <summary>
        /// Applies the decorator to node.
        /// </summary>
        /// <param name="decorTool">The decorator tool.</param>
        /// <param name="decorNode">The decorator node.</param>
        protected void ApplyDecorator(Decorator decorTool, Decorator decorNode)
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
        /// Outlines possible connection point.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="port">ConnectionPoint to outline.</param>
        protected void OutlineConnectionPoint(Graphics gfx, ConnectionPoint port)
        {
            // EndPoint possible connection
            if (port != null)
            {
                if (port is CentralPort)
                    HighlightCenterPortContainer(gfx, port);

                DrawFrameAroundPort(gfx, port);
            }
        }

        /// <summary>
        /// Snaps to port.
        /// </summary>
        /// <param name="ptPoint">The given point in client coordinates.</param>
        /// <param name="szOffset">The size offset.</param>
        /// <returns>The point that snapped to port.</returns>
        protected PointF SnapToPort(PointF ptPoint, SizeF szOffset)
        {
            Node node;
            return SnapToPort(ptPoint, szOffset, out node);
        }

        /// <summary>
        /// Snaps to nearest port.
        /// </summary>
        /// <param name="ptPoint">The snapping point in client coordinates.</param>
        /// <param name="szOffset">moving entity bounding rectangle left-top point offset</param>
        /// <param name="nodeHit">The node hit.</param>
        /// <returns>Port position.</returns>
        protected PointF SnapToPort(PointF ptPoint, SizeF szOffset, out Node nodeHit)
        {
            PointF ptToReturn = ptPoint;

            ptToReturn.X -= szOffset.Width;
            ptToReturn.Y -= szOffset.Height;

            PointF ptPointCheck = this.Controller.ConvertToModelCoordinates(ptToReturn);
            
            // get node to check for central port
            nodeHit = GetInsideNodeAt(ptPointCheck);

            // check whether there is port under endpoint
            ConnectionPoint port = HandlesHitTesting.GetConnectionPointAtPoint(this.Controller.Model.Nodes, ptPointCheck);

            if (nodeHit != null && nodeHit.EnableCentralPort)
            {
                port = nodeHit.CentralPort;
            }

            // check for connection possibility
            if (port != null)
            {
                ptToReturn = HandlesHitTesting.GetPortPosition(port);
                ptToReturn = this.Controller.ConvertFromModelToClientCoordinates(ptToReturn);
            }

            ptToReturn.X += szOffset.Width;
            ptToReturn.Y += szOffset.Height;

            return ptToReturn;
        }

        /// <summary>
        /// Draws the frame around port.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="port">The port.</param>
        protected virtual void DrawFrameAroundPort(Graphics gfx, ConnectionPoint port)
        {
            // calc port position
            PointF[] pts = new PointF[1];
            pts[0] = port.GetPosition();

            SizeF szFrameSize = SizeF.Empty;

            if (port is CentralPort)
            {
                Node m_container = port.Container;
                SizeF szContainerSize = SizeF.Empty;
                if (m_container != null)
                    szContainerSize = ((IUnitIndependent)m_container).GetSize(MeasureUnits.Pixel);
                PointF ptContainerCenter = new PointF(szContainerSize.Width / 2, szContainerSize.Height / 2);

                pts[0].X = ptContainerCenter.X;
                pts[0].Y = ptContainerCenter.Y;

                SizeF szTemp = ((IUnitIndependent)port.Container).GetBoundingRectangle(MeasureUnits.Pixel, false).Size;

                szFrameSize.Width = szTemp.Width;
                szFrameSize.Height = szTemp.Height;
            }
            else
            {
                szFrameSize.Width = port.Size;
                szFrameSize.Height = szFrameSize.Width;
            }

            // exclude rotation
            Matrix mtxTransforms = HandlesHitTesting.GetParentsTransformations(port.Container, true);
            mtxTransforms.TransformPoints(pts);

            // draw port
            using (Pen penPortOutline = new Pen(port.LineStyle.LineColor, 2f / gfx.PageScale))
            {
                RectangleF rectPortBounds = Geometry.CreateRect(pts[0], szFrameSize);

                gfx.DrawRectangle(penPortOutline, rectPortBounds.X, rectPortBounds.Y, rectPortBounds.Width, rectPortBounds.Height);
            }
        }

        /// <summary>
        /// Highlights the center port container.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="port">The end point.</param>
        protected void HighlightCenterPortContainer(Graphics gfx, ConnectionPoint port)
        {
            Node container = port.Container;

            if (container != null && container.EnableCentralPort)
            {
                GraphicsPath gphClone = container.GraphicsPath;
                Matrix mtxTransformations = HandlesHitTesting.GetParentsTransformations(container, true);
                Color solidColor = Color.FromArgb(125, Color.YellowGreen);

                using (Pen pen = new Pen(Color.Black, 3.0f))
                using (SolidBrush outLine = new SolidBrush(solidColor))
                {
                    gphClone.Transform(mtxTransformations);
                    if (gphClone.GetBounds(mtxTransformations, pen).Size != SizeF.Empty)
                        gphClone.Widen(pen);
                    gfx.FillPath(outLine, gphClone);
                }
            }
        }

        /// <summary>
        /// Gets the node under end point.
        /// </summary>
        /// <param name="endPointLocation">The end point container.</param>
        /// <returns>The node under end point.</returns>
        protected Node GetNodeUnderEndPoint(PointF endPointLocation)
        {
            return GetInsideNodeAt(endPointLocation);
        }
        #endregion
    }

    /// <summary>
    /// Single action tools.
    /// </summary>
    public enum SingleActionTools
    {
        /// <summary>
        /// No tool.
        /// </summary>
        None = 0,

        /// <summary>
        /// Move tool
        /// </summary>
        MoveTool,

        /// <summary>
        /// Resize tool
        /// </summary>
        ResizeTool,

        /// <summary>
        /// Rotate tool
        /// </summary>
        RotateTool,

        /// <summary>
        /// Move link port tool.
        /// </summary>
        MoveLinkPortTool,

        /// <summary>
        /// Vertex move tool
        /// </summary>
        VertexMoveTool,

        /// <summary>
        /// Pinpoint move tool.
        /// </summary>
        PinPointMoveTool,

        /// <summary>
        /// Control point move tool.
        /// </summary>
        ControlPointMoveTool,

        /// <summary>
        /// Vertex delete action
        /// </summary>
        VertexDeleteAction,

        /// <summary>
        /// Vertex insert action
        /// </summary>
        VertexInsertAction,

        /// <summary>
        /// Line segment tool
        /// </summary>
        LineSegmentTool
    }
}
