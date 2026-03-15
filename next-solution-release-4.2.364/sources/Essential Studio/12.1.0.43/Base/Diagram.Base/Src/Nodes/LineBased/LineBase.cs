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
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Base shape class to visualize line with end point decorators.
    /// </summary>
    [Serializable]
    public abstract class LineBase
        : PathNode
    {
        #region Class members
        private HeadDecorator m_decoratorHead;
        private TailDecorator m_decoratorTail;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LineBase"/> class.
        /// </summary>
        public LineBase()
            : base()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineBase"/> class.
        /// </summary>
        /// <param name="src">The LineBase instance.</param>
        public LineBase(LineBase src)
            : base(src)
        {
            if (src.m_decoratorHead != null)
            {
                m_decoratorHead = (HeadDecorator)src.m_decoratorHead.Clone();
                m_decoratorHead.Container = this;
            }

            if (src.m_decoratorTail != null)
            {
                m_decoratorTail = (TailDecorator)src.m_decoratorTail.Clone();
                m_decoratorTail.Container = this;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineBase"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected LineBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "decoratorHead":
                        m_decoratorHead = (HeadDecorator)info.GetValue("decoratorHead", typeof(HeadDecorator));
                        m_decoratorHead.Container = this;
                        break;
                    case "decoratorTail":
                        m_decoratorTail = (TailDecorator)info.GetValue("decoratorTail", typeof(TailDecorator));
                        m_decoratorTail.Container = this;
                        break;
                }
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the head end point decorator.
        /// </summary>
        /// <value>The head decorator.</value>
        [Browsable(true)]
        [Description("Defines HeadDecorator.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Decorator HeadDecorator
        {
            get
            {
                if (m_decoratorHead == null)
                {
                    m_decoratorHead = new HeadDecorator();
                    m_decoratorHead.Container = this;
                }

                return m_decoratorHead;
            }
        }

        /// <summary>
        /// Gets the tail end point decorator.
        /// </summary>
        /// <value>The tail decorator.</value>
        [Browsable(true)]
        [Description("Defines TailDecorator.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Decorator TailDecorator
        {
            get
            {
                if (m_decoratorTail == null)
                {
                    m_decoratorTail = new TailDecorator();
                    m_decoratorTail.Container = this;
                }

                return m_decoratorTail;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Indicate that selection and resize handles will draw on diagram canvas.
        /// </summary>
        /// <returns><c>True</c> if node can be resized, otherwise - <c>false</c>.</returns>
        public override bool ShowResizeHandles()
        {
            return false;
        }

        /// <summary>
        /// Renders shapes visual representation on given graphics
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        protected override void Render(Graphics gfx)
        {
            // 1 - call base method impementation
            base.Render(gfx);

            // 2 - Draw path
            DrawPath(gfx);

            // 3 - Draw end points
            DrawDecorators(gfx);
        }

        /// <summary>
        /// Draws the path to specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        protected virtual void DrawPath(Graphics gfx)
        {
            // update cut end points for decorators
            GraphicsPath gphClone = ClipDecorators(this.GraphicsPath);
            RectangleF rcBounds = gphClone.GetBounds();

            // if graphics path bounds less than 0.002f 
            // stop drawing it to canvas
            float fMinSize = 0.002f / gfx.PageScale;
            if (rcBounds.Width < fMinSize && rcBounds.Height < fMinSize)
                return;

            using (Pen pen = this.LineStyle.CreatePen())
            {
                gfx.DrawPath(pen, gphClone);
            }
        }

        /// <summary>
        /// Draw decorators.
        /// Default implementation provides decorators drawing for line.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        protected virtual void DrawDecorators(Graphics gfx)
        {
            // Draw head decorator
            if (CanDrawHeadDecorator())
            {
                // Save graphics state
                GraphicsState stateSave = gfx.Save();

                // Append transformations
                gfx.MultiplyTransform(GetHeadDecoratorTransformations());

                // render head decorator
                this.HeadDecorator.Draw(gfx);

                // restore graphics state
                gfx.Restore(stateSave);
            }

            // Draw Tail decorator
            if (CanDrawTailDecorator())
            {
                // Save graphics state
                GraphicsState stateSave = gfx.Save();

                // Append transformations
                gfx.MultiplyTransform(GetTailDecoratorTransformations());

                // render tail decorator
                this.TailDecorator.Draw(gfx);

                // restore graphics state
                gfx.Restore(stateSave);
            }
        }

        /// <summary>
        /// Creates region used for hit testing.
        /// </summary>
        protected override void UpdateHelperRegion()
        {
            // save padding value;
            float fPadding;
            if (HandlesHitTesting.TouchMode)
                fPadding = CommonUsedValues.TOUCH__HIT_TEST_PADDING;
            else
                fPadding = this.LineHitTestPadding;

            // 1 - create Pen used to widen cloned GraphicsPath with
            using (Pen pen = new Pen(Color.Black, fPadding))

            // 2 - clone existing shape's GraphicsPath
            using (GraphicsPath pathClone = this.GraphicsPath)
            {
                if (CanDrawHeadDecorator())
                {
                    using (GraphicsPath pathDecorator = (GraphicsPath)this.HeadDecorator.GraphicsPath.Clone())
                    {
                        // Add head gaphics path eith transfromations.
                        pathDecorator.Transform(GetHeadDecoratorTransformations());
                        pathClone.AddPath(pathDecorator, false);
                    }
                }

                if (CanDrawTailDecorator())
                {
                    using (GraphicsPath pathDecorator = (GraphicsPath)this.TailDecorator.GraphicsPath.Clone())
                    {
                        // Add tail gaphics path eith transfromations.
                        pathDecorator.Transform(GetTailDecoratorTransformations());
                        pathClone.AddPath(pathDecorator, false);
                    }
                }

                Matrix matrix = GetTransformations();

                AppendFlipTransforms(matrix);

                pathClone.Transform(matrix);

                // 3 - widen graphics path
                if (CanWiden(pathClone))
                {
                    pathClone.Widen(pen);
                }

                // 4 - create region from widened GraphicsPath
                m_rgnCache = Geometry.CreateRegionFromGraphicsPath(pathClone);
            }
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (m_decoratorHead != null)
                info.AddValue("decoratorHead", m_decoratorHead);

            if (m_decoratorTail != null)
                info.AddValue("decoratorTail", m_decoratorTail);
        }

        /// <summary>
        /// Updates the references from service provider.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        public override void UpdateReferences(IServiceReferenceProvider provider)
        {
            base.UpdateReferences(provider);

            this.HeadDecorator.UpdateServiceReferences(provider);
            this.TailDecorator.UpdateServiceReferences(provider);
        }

        /// <summary>
        /// Gets the property container.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <returns>The property container object.</returns>
        protected override object GetPropertyContainer(string strPropertyContainerName)
        {
            object objToReturn = base.GetPropertyContainer(strPropertyContainerName);

            if (strPropertyContainerName == "TailDecorator")
            {
                objToReturn = this.TailDecorator;
            }
            else if (strPropertyContainerName == "HeadDecorator")
            {
                objToReturn = this.HeadDecorator;
            }

            return objToReturn;
        }

        /// <summary>
        /// Accumulates the refresh rect.
        /// </summary>
        /// <param name="rcRefresh">The refresh rectangle.</param>
        protected override void AccumulateRefreshRect(ref RectangleF rcRefresh)
        {
            base.AccumulateRefreshRect(ref rcRefresh);

            // current decorator transformations
            Matrix mtxTemp;

            // current decorator bounding rectangle
            RectangleF rcDecBounds;

            // HEAD DECORATOR
            if (CanDrawHeadDecorator())
            {
                mtxTemp = GetHeadDecoratorTransformations();
                rcDecBounds = this.HeadDecorator.GraphicsPath.GetBounds(mtxTemp);

                if (rcRefresh.Size.IsEmpty)
                {
                    rcRefresh = rcDecBounds;
                }
                else
                {
                    rcRefresh = RectangleF.Union(rcRefresh, rcDecBounds);
                }
            }

            // TAIL DECORATOR
            if (CanDrawTailDecorator())
            {
                mtxTemp = GetTailDecoratorTransformations();
                rcDecBounds = this.TailDecorator.GraphicsPath.GetBounds(mtxTemp);

                if (rcRefresh.Size.IsEmpty)
                {
                    rcRefresh = rcDecBounds;
                }
                else
                {
                    rcRefresh = RectangleF.Union(rcRefresh, rcDecBounds);
                }
            }
        }

        /// <summary>
        /// Draws the shadow internal.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        protected override void DrawShadowInternal(Graphics gfx)
        {
            // draw shadow for line
            DrawLineBaseShadow(gfx);

            // render head/tail decorator's shadow
            DrawDecoratorsShadow(gfx);
        }

        /// <summary>
        /// Draw shadow for linebase node with clip end points.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        private void DrawLineBaseShadow(Graphics gfx)
        {
            float fLineWidth = this.LineStyle.LineWidth;

            if (fLineWidth == 0)
                return;

            // clone graphics path for draw shadow
            GraphicsPath gp = this.GraphicsPath;
            RectangleF rcGPBounds = gp.GetBounds();

            // update graphics path
            if (rcGPBounds.Width == 0)
            {
                gp = new GraphicsPath();
                gp.AddLine(PointF.Empty, new PointF(0, rcGPBounds.Height));
                rcGPBounds = new RectangleF(0, -fLineWidth / 2, fLineWidth, rcGPBounds.Height);
            }
            if (rcGPBounds.Height == 0)
            {
                gp = new GraphicsPath();
                gp.AddLine(PointF.Empty, new PointF(rcGPBounds.Width, 0));
                rcGPBounds = new RectangleF(-fLineWidth / 2, 0, rcGPBounds.Width, fLineWidth);
            }

            // cut end points for decorators
            gp = ClipDecorators(gp);

            if (!IsClosedPath(gp) || (rcGPBounds.Height == 0) || (rcGPBounds.Width == 0))
            {
                // draw shadow
                using (Pen pen = new Pen(Color.FromArgb(80, this.ShadowStyle.Color), this.LineStyle.LineWidth))
                {
                    gfx.DrawPath(pen, gp);
                }
            }
            else
            {
                // fill shadow
                using (Brush brushShadow = this.ShadowStyle.CreateBrush(gfx, rcGPBounds))
                {
                    gfx.FillPath(brushShadow, gp);
                }
            }
        }

        /// <summary>
        /// Draw the decorators shadow to specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        protected void DrawDecoratorsShadow(Graphics gfx)
        {
            GraphicsPath gpDecor;
            RectangleF rcBounds;
            float fLineStyleWidth = this.LineStyle.LineWidth;

            // Draw head decorator
            if (this.HeadDecorator.DecoratorShape != DecoratorShape.None)
            {
                gpDecor = this.HeadDecorator.GraphicsPath;
                rcBounds = Geometry.WidenRect(gpDecor.GetBounds(), this.LineStyle.LineWidth);

                if (rcBounds.Width != 0)
                {
                    // Save graphics state
                    GraphicsState stateSave = gfx.Save();

                    // Append transformations
                    gfx.MultiplyTransform(GetHeadDecoratorTransformations());

                    // render head decorator
                    using (Brush brushShadow = this.ShadowStyle.CreateBrush(gfx, rcBounds))
                    {
                        if (IsClosedPath(gpDecor))
                        {
                            gfx.FillPath(brushShadow, gpDecor);
                        }

                        using (Pen pen = new Pen(brushShadow, this.LineStyle.LineWidth))
                        {
                            gfx.DrawPath(pen, gpDecor);
                        }
                    }

                    // restore graphics state
                    gfx.Restore(stateSave);
                }
            }

            // Draw Tail decorator
            if (this.TailDecorator.DecoratorShape != DecoratorShape.None)
            {
                gpDecor = this.TailDecorator.GraphicsPath;
                rcBounds = Geometry.WidenRect(gpDecor.GetBounds(), this.LineStyle.LineWidth);

                if (rcBounds.Width != 0)
                {
                    // Save graphics state
                    GraphicsState stateSave = gfx.Save();

                    // Append transformations
                    gfx.MultiplyTransform(GetTailDecoratorTransformations());

                    using (Brush brushShadow = this.ShadowStyle.CreateBrush(gfx, rcBounds))
                    {
                        if (IsClosedPath(gpDecor))
                        {
                            gfx.FillPath(brushShadow, gpDecor);
                        }

                        using (Pen pen = new Pen(brushShadow, this.LineStyle.LineWidth))
                        {
                            gfx.DrawPath(pen, gpDecor);
                        }
                    }

                    // restore graphics state
                    gfx.Restore(stateSave);
                }
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Clip end points for decorator draw..
        /// </summary>
        /// <param name="gphPath">The sorce graphics path.</param>
        /// <returns>Grapphics path with clip pathPoints.</returns>
        private GraphicsPath ClipDecorators(GraphicsPath gphPath)
        {
            GraphicsPath gpClone = gphPath;
            PointF[] pts = (PointF[])gphPath.PathPoints.Clone();
            bool bRecreatePath = false;

            // check if decorators size is less than container's
            if (pts.Length > 1)
            {
                PointF ptStart;
                PointF ptEnd;

                if (CanDrawTailDecorator())
                {
                    ptStart = pts[0];
                    ptEnd = pts[1];

                    float length = (float)Geometry.PointDistance(ptStart, ptEnd);
                    length = (length == 0) ? 1 : length;
                    float width = this.TailDecorator.Size.Width - 1;
                    PointF newPoint = new PointF();
                    newPoint.X = ptStart.X + width * (ptEnd.X - ptStart.X) / length;
                    newPoint.Y = ptStart.Y + width * (ptEnd.Y - ptStart.Y) / length;
                    pts[0] = newPoint;

                    bRecreatePath = true;
                }

                if (CanDrawHeadDecorator())
                {
                    ptStart = pts[pts.Length - 1];
                    ptEnd = pts[pts.Length - 2];

                    float length = (float)Geometry.PointDistance(ptStart, ptEnd);
                    length = (length == 0) ? 1 : length;
                    float width = this.HeadDecorator.Size.Width - 1;
                    PointF newPoint = new PointF();
                    newPoint.X = ptStart.X + width * (ptEnd.X - ptStart.X) / length;
                    newPoint.Y = ptStart.Y + width * (ptEnd.Y - ptStart.Y) / length;
                    pts[pts.Length - 1] = newPoint;

                    bRecreatePath = true;
                }

                if (bRecreatePath)
                {
                    gpClone = new GraphicsPath(pts, gphPath.PathTypes);
                }
            }

            return gpClone;
        }

        /// <summary>
        /// Determines whether pathNode can draw head decorator.
        /// </summary>
        /// <returns>
        /// <c>true</c> if pathNode can draw head decorator; otherwise, <c>false</c>.
        /// </returns>
        private bool CanDrawHeadDecorator()
        {
            return this.HeadDecorator.DecoratorShape != DecoratorShape.None && this.HeadDecorator.GraphicsPath != null;
        }

        /// <summary>
        /// Determines whether head decorator  can draw tail decorator.
        /// </summary>
        /// <returns>
        /// <c>true</c> if pathNode can draw head decorator; otherwise, <c>false</c>.
        /// </returns>
        private bool CanDrawTailDecorator()
        {
            return this.TailDecorator.DecoratorShape != DecoratorShape.None && this.TailDecorator.GraphicsPath != null;
        }

        /// <summary>
        /// Calculates the decorator transform.
        /// </summary>
        /// <param name="ptPoint">The last point.</param>
        /// <param name="ptControl">The next to last point.</param>
        /// <param name="depth">The depth of decorator to next last point.</param>
        /// <returns>
        /// The <see cref="System.Drawing.Drawing2D.Matrix"/>.
        /// </returns>
        protected Matrix GetDecoratorTransfrom(PointF ptPoint, PointF ptControl, float depth)
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
        /// Gets the head decorator transformations.
        /// </summary>
        /// <returns>The matrix.</returns>
        protected virtual Matrix GetTailDecoratorTransformations()
        {
            // Get data points.
            PointF[] pts = this.GraphicsPath.PathPoints;

            // Get decorator depth
            float depth = this.TailDecorator.Size.Width;

            // Append transform to graphics.
            Matrix mtxTemp = GetDecoratorTransfrom(pts[0], pts[1], depth);
            mtxTemp.Translate(0, -this.TailDecorator.Size.Height / 2);

            return mtxTemp;
        }

        /// <summary>
        /// Gets the tail decorator transformations.
        /// </summary>
        /// <returns>The matrix.</returns>
        protected virtual Matrix GetHeadDecoratorTransformations()
        {
            // Get data points.
            PointF[] pts = this.GraphicsPath.PathPoints;
            int ptsCount = pts.Length;

            // Get decorator depth
            float depth = this.HeadDecorator.Size.Width;

            // Append transform to graphics.
            Matrix mtxTemp = GetDecoratorTransfrom(pts[ptsCount - 1], pts[ptsCount - 2], depth);
            mtxTemp.Translate(0, -this.HeadDecorator.Size.Height / 2);

            return mtxTemp;
        }

        /// <summary>
        /// Initialize the bounds info.
        /// </summary>
        /// <param name="rcBounds">The bounds rectangle.</param>
        protected void InitBoundsInfo(RectangleF rcBounds)
        {
            // calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(rcBounds.Width / 2, rcBounds.Height / 2);

            // assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(
                rcBounds.X + szPinOffsetUnitIndependent.Width,
                rcBounds.Y + szPinOffsetUnitIndependent.Height);

            // assign node size value
            SizeF szSizeUnitIndependent = rcBounds.Size;

            // Init BoundsInfo
            CreateBoundsInfo(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, szSizeUnitIndependent);
        }

        /// <summary>
        /// Gets the line segment at given point.
        /// </summary>
        /// <param name="nSegmentIndex">Index of the segment.</param>
        /// <returns>The line segment.</returns>
        public virtual LineSegment GetLineSegmentAt(int nSegmentIndex)
        {
            if (0 > nSegmentIndex || nSegmentIndex > this.PathPoints.Length - 1)
                throw new ArgumentOutOfRangeException("nSegmentIndex");

            PointF[] pts = GetPoints();
            LineSegment segmentsToReturn = new LineSegment(pts[nSegmentIndex], pts[nSegmentIndex + 1], nSegmentIndex);

            return segmentsToReturn;
        }

        /// <summary>
        /// Quite move endpoint to current offset.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="szOffset">The move offset.</param>
        protected void SynchronizeEndPoint(EndPoint endPoint, SizeF szOffset)
        {
            if (endPoint != null)
            {
                bool bLock = m_bLockUpdate;
                m_bLockUpdate = true;

                // pause history recoding
                SafeHistoryPause();

                // pause raising events
                if (this.EventSink != null)
                    this.EventSink.Pause();

                // just move to offset
                endPoint.Move(szOffset, MeasureUnits.Pixel);

                // resume raising events
                if (this.EventSink != null)
                    this.EventSink.Resume();

                m_bLockUpdate = bLock;

                // restore history recoding
                SafeHistoryResume();
            }
        }
        #endregion
    }
}
