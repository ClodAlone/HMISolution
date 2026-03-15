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

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// PseudoGroup is a logical container for selection list nodes,
    /// its children are not positioned relative to it.
    /// It is used to improve selection list handles render speed.
    /// To enable this feature You should set View.EnableSelectionListSubstitute flag to true.
    /// </summary>
    public sealed class PseudoGroup
        : Group
    {
        #region Class members
        private bool m_bLock;

        /// <summary>
        /// Pervious PinOffset value.
        /// Used to calc correct child nodes position after Pin relocation.
        /// OldPinOffset updated on resize is not valid!
        /// </summary>
        private SizeF m_szOldPinOffset;

        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoGroup"/> class.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public PseudoGroup(NodeCollection nodes)
        {
            this.EditStyle.AspectRatio = true;
            this.Nodes.UpdateReferences = false;
            this.Nodes.AddRange(nodes);
            this.UpdateChildrenContainer();
            UpdateGroupInfo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoGroup"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public PseudoGroup(PseudoGroup src)
            : base(src)
        {
            m_bLock = src.m_bLock;
            Node srcNode;
            Node clnNode;

            // update clone nodes position
            for (int nIdx = 0, nLength = src.Nodes.Count; nIdx < nLength; nIdx++)
            {
                srcNode = src.Nodes[nIdx];
                clnNode = this.Nodes[nIdx];

                UpdateNodeLocation(clnNode, srcNode.Parent);
            }
            this.UpdateChildrenContainer();
            UpdateBoundingRectangle();
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Updates the children parents.
        /// </summary>
        protected override void UpdateChildrenParents()
        {
            // do not update child nodes parents
        }

        /// <summary>
        /// Performs additional changes on pin offset value changed.
        /// </summary>
        /// <param name="szOldPinOffset">The old pin offset value.</param>
        /// <param name="szNewPinOffset">The new pin offset value.</param>
        protected override void DoPinOffsetRelatedActions(SizeF szOldPinOffset, SizeF szNewPinOffset)
        {
            if (!m_bLock)
            {
                m_bLock = true;

                base.DoPinOffsetRelatedActions(szOldPinOffset, szNewPinOffset);

                m_szOldPinOffset = szOldPinOffset;
                m_bLock = false;
            }
        }

        /// <summary>
        /// Renders the cached image.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        protected override void RenderCachedImage(Graphics gfx)
        {
            PointF ptPin = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);
            SizeF szOffset = this.BoundsInfo.GetPinOffset(MeasureUnits.Pixel);
            PointF ptTemp = new PointF(ptPin.X - szOffset.Width, ptPin.Y - szOffset.Height);

            Matrix mtx = GetTransformations();

            RectangleF rcBounding = RectangleF.Empty;
            rcBounding.Location = ptTemp;
            rcBounding.Size = this.BoundsInfo.GetSize(MeasureUnits.Pixel);

            PointF[] pts = new PointF[]
            {
                rcBounding.Location,
                new PointF( rcBounding.Right, rcBounding.Top ),
                new PointF( rcBounding.Left, rcBounding.Bottom ),
                new PointF( rcBounding.Right, rcBounding.Bottom )
            };

            mtx.TransformPoints(pts);

            gfx.TranslateTransform(ptTemp.X, ptTemp.Y, MatrixOrder.Append);

            Matrix mtxTemp = new Matrix();

            float fRotationAngle = this.RotationAngle;

            if ((this.FlipX || this.FlipY) && !(this.FlipX && this.FlipY))
            {
                fRotationAngle = 360 - fRotationAngle;
                fRotationAngle = Geometry.ConvertToPartCircle(fRotationAngle);
            }

            mtxTemp.RotateAt(fRotationAngle, ptPin);

            gfx.MultiplyTransform(mtxTemp, MatrixOrder.Append);

            base.RenderCachedImage(gfx);
        }

        /// <summary>
        /// Called after change the rotation by give angle.
        /// </summary>
        /// <param name="fRotationChange">The rotation angle offset.</param>
        protected override void ChangeRotationBy(float fRotationChange)
        {
            if (!m_bLock && CanRotate())
            {
                m_bLock = true;

                // suppress Model.BoundaryConstrains flag
                Model model = this.Nodes[0].Root;
                bool bBCE = false;

                if (model != null && model.BoundaryConstraintsEnabled)
                {
                    model.BridgeManager.BeginUpdateIntersection();
                    model.LinkManager.BeginSynchronization();
                    bBCE = model.BoundaryConstraintsEnabled;
                    model.BoundaryConstraintsEnabled = false;
                }

                foreach (Node nodeCur in this.Nodes)
                {
                    UpdateChildNodeRotationAngle(nodeCur, fRotationChange);
                }

                base.ChangeRotationBy(fRotationChange);

                if (model != null)
                {
                    model.BoundaryConstraintsEnabled = bBCE;
                    model.LinkManager.EndSynchronization();
                    model.BridgeManager.EndUpdateIntersection();
                }

                m_bLock = false;
                //UpdateGroupInfo();
            }
        }

        /// <summary>
        /// Prepares the graphics to draw.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        protected override void PrepareGraphics(Graphics gfx)
        {
            // Skip applying pseudo group transforms to graphics as
            // its children are not positioned relative to it.
            // They are positioned relative to Model.
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            return new PseudoGroup(this);
        }

        /// <summary>
        /// Performs additional changes on pin position changed.
        /// </summary>
        /// <param name="fX">The pin offset by x axis.</param>
        /// <param name="fY">The pin offset by y axis.</param>
        protected override void DoMoveRelatedActions(float fX, float fY)
        {
            if (!m_bLock)
            {
                m_bLock = true;

                foreach (Node nodeChild in m_nodesChildren)
                {
                    nodeChild.Translate(fX, fY);
                }

                base.DoMoveRelatedActions(fX, fY);

                m_bLock = false;
            }
        }

        /// <summary>
        /// Set the pin point position.
        /// </summary>
        /// <param name="ptValue">The pin position.</param>
        /// <param name="unit">The measure unit.</param>
        protected override void SetPinPoint(PointF ptValue, MeasureUnits unit)
        {
            if (!m_bLock && CanMove())
            {
                //// as children are not positioned relative to PseudoGroup
                //// 1) convert child pin to group's relative coordinates
                //// 2) update group's pin
                //// 3) convert child's pin to its parent relative coordinates

                //// get old transformtaion matrix !!using previous PinOffset value
                //PointF ptPinPointUnitIndependent = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);
                //SizeF szPinOffsetUnitIndependent = m_bIsValid ? m_szOldPinOffset : this.BoundsInfo.GetPinOffset(MeasureUnits.Pixel);

                //// rotation angle
                //float fAngle = this.RotationAngle;
                //Matrix mtxPSGPrev = GetTransformations(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, fAngle);
                //AppendFlipTransforms(mtxPSGPrev);
                //mtxPSGPrev.Invert();

                //// get new transformation matrix
                //Matrix mtxPSG = GetTransformations(ptValue, szPinOffsetUnitIndependent, fAngle);
                //AppendFlipTransforms(mtxPSG);

                //PointF[] pts = new PointF[1];
               
                //// iterate through child nodes updating their pin positions
                //foreach (IUnitIndependent node in this.Nodes)
                //{
                //    // convert node pin position to group relative coordinates
                //    // before updating group's pin
                //    pts[0] = node.GetPinPoint(MeasureUnits.Pixel);
                //    //// convert to group's relative coordinates
                //    mtxPSGPrev.TransformPoints(pts);
                //    //// convert to new position
                //    mtxPSG.TransformPoints(pts);
                //    //// assign new pin position
                //    node.SetPinPoint(pts[0], MeasureUnits.Pixel);
                //}
                this.BoundsInfo.SetPinPoint(ptValue, unit);
                UpdateContainerBounds();

                // update bounding to new nodes positions
                UpdateGroupInfo();
            }
        }

        /// <summary>
        /// Used to update child nodes sizes.
        /// </summary>
        /// <param name="szOldSize">Old size value.</param>
        /// <param name="szNewSize">New size value.</param>
        protected override void DoSizeRelatedActions(SizeF szOldSize, SizeF szNewSize)
        {
            if (!m_bLock && CanResize())
            {
                m_bLock = true;

                // temp matrix to convert pin point coordinates
                Matrix mtxAppend;

                // create groups' transformation matrix to determine child node's position relative to group
                Matrix mtxPSG = GetTransformations();
                AppendFlipTransforms(mtxPSG);

                Matrix mtxPSGInverted = mtxPSG.Clone();
                mtxPSGInverted.Invert();

                // calc group scale factor
                float fGroupScaleFactorX = (szNewSize.Width != 0) ? szNewSize.Width / szOldSize.Width : 1;
                float fGroupScaleFactorY = (szNewSize.Height != 0) ? szNewSize.Height / szOldSize.Height : 1;

                SizeF szPinOffset = this.BoundsInfo.GetPinOffset(MeasureUnits.Pixel);
                
                // Distance between group and child node's pin point.
                SizeF szPinDistance = SizeF.Empty;
                NodeCollection nodes = this.Nodes;

                bool bLineBridgingEnabled = QuiteBridgingSet(false);
                ArrayList connections = SaveConnections(nodes);

                // iterate through child nodes updating their's pin positions
                foreach (Node node in nodes)
                {
                    // as pseudo group children are positioned not relatively to it
                    // 1) we have to covert their's pin positions
                    //    to PseudoGroup relative coordinates 
                    // 2) define new child position
                    // 3) convert to current nodes's parent coordinates

                    // get current node's distance to group's pin point
                    // -------------------------------------------------
                    // get current child pin position
                    mtxAppend = HandlesHitTesting.GetParentsTransformations(node);
                    mtxAppend.Multiply(mtxPSGInverted, MatrixOrder.Append);

                    PointF ptChildPin = GetNodePinPGRelative(node, mtxAppend);
                    //// calc pin distance
                    szPinDistance.Width = ptChildPin.X - szPinOffset.Width;
                    szPinDistance.Height = ptChildPin.Y - szPinOffset.Height;
                    //// apply group's scale factor to PinDistance
                    szPinDistance.Width *= fGroupScaleFactorX;
                    szPinDistance.Height *= fGroupScaleFactorY;
                    //// calc new pin position in group's relative coordinates
                    ptChildPin.X = szPinOffset.Width + szPinDistance.Width;
                    ptChildPin.Y = szPinOffset.Height + szPinDistance.Height;
                    //// convert to child node's parent relative coordinates
                    mtxAppend.Invert();
                    UpdatePinRelativeToParent(ref ptChildPin, mtxAppend);
                    //// assign new pin value
                    ((IUnitIndependent)node).SetPinPoint(ptChildPin, MeasureUnits.Pixel);

                    // UPDATE NODE's SIZE
                    UpdateSize(node, fGroupScaleFactorX, fGroupScaleFactorY);
                }

                RestoreConnections(connections);
                QuiteBridgingSet(bLineBridgingEnabled);

                UpdateBoundingRectangle();
                ResetRenderCache();

                m_bLock = false;
            }
        }

        /// <summary>
        /// Updates the bounding rectangle.
        /// </summary>
        protected override void UpdateBoundingRectangle()
        {
            RectangleF rcRectangle = GetPathBounds();
            this.BoundingRect = rcRectangle;

            Matrix mtxTransform = GetTransformations();
            AppendFlipTransforms(mtxTransform);

            UpdateRefreshRect();
        }

        /// <summary>
        /// Called after changing the flipX value.
        /// </summary>
        /// <param name="value">New flip x value.</param>
        protected override void ChangeFlipX(bool value)
        {
            PointF ptPin = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);
            
            // create transform matrix
            Matrix mtxTemp = new Matrix();
            mtxTemp.Translate(-ptPin.X, -ptPin.Y, MatrixOrder.Append);
            
            // Flip horizontal
            mtxTemp.Scale(-1.0f, 1.0f, MatrixOrder.Append);
            mtxTemp.Translate(ptPin.X, ptPin.Y, MatrixOrder.Append);

            bool bBridging = QuiteBridgingSet(false);
            ArrayList list = HandlesHitTesting.SaveConnections(this.Nodes);

            // iterate through children updating their positions
            foreach (Node nodeCur in this.Nodes)
            {
                UpdateNodeLocation(nodeCur, mtxTemp);

                nodeCur.FlipX = !nodeCur.FlipX;
            }

            ResetRenderCache();

            base.ChangeFlipX(value);

            HandlesHitTesting.RestoreConnections(list);
            QuiteBridgingSet(bBridging);
        }

        /// <summary>
        /// Called after changing the flipY value.
        /// </summary>
        /// <param name="value">New flip y value.</param>
        protected override void ChangeFlipY(bool value)
        {
            PointF ptPin = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);
            
            // create transform matrix
            Matrix mtxTemp = new Matrix();
            mtxTemp.Translate(-ptPin.X, -ptPin.Y, MatrixOrder.Append);
            
            // Flip horizontal
            mtxTemp.Scale(1.0f, -1.0f, MatrixOrder.Append);
            mtxTemp.Translate(ptPin.X, ptPin.Y, MatrixOrder.Append);

            bool bBridging = QuiteBridgingSet(false);
            ArrayList list = HandlesHitTesting.SaveConnections(this.Nodes);

            // iterate through children updating their positions
            foreach (Node nodeCur in this.Nodes)
            {
                UpdateNodeLocation(nodeCur, mtxTemp);

                nodeCur.FlipY = !nodeCur.FlipY;
            }

            ResetRenderCache();

            base.ChangeFlipY(value);

            HandlesHitTesting.RestoreConnections(list);
            QuiteBridgingSet(bBridging);
        }

        /// <summary>
        /// Updates the references.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public override void UpdateReferences(IServiceReferenceProvider provider)
        {
            this.BoundsInfo.UpdateServiceReferences(provider);

            if (provider != null)
            {
                m_eventSink = (DocumentEventSink)provider.ProvideServiceReference(typeof(DocumentEventSink).TypeHandle);

                if (m_eventSink != null)
                {
                    m_eventSink.PinPointChanged += new PinPointChangedEventHandler(m_eventSink_PinPointChanged);
                    m_eventSink.PinOffsetChanged += new PinOffsetChangedEventHandler(m_eventSink_PinOffsetChanged);
                    m_eventSink.SizeChanged += new SizeChangedEventHandler(m_eventSink_SizeChanged);
                    m_eventSink.FlipChanged += new FlipChangedEventHandler(m_eventSink_FlipChanged);
                    m_eventSink.RotationChanged += new RotationChangedEventHandler(m_eventSink_RotationChanged);
                }
            }
            else if (m_eventSink != null)
            {
                m_eventSink.PinPointChanged -= new PinPointChangedEventHandler(m_eventSink_PinPointChanged);
                m_eventSink.PinOffsetChanged -= new PinOffsetChangedEventHandler(m_eventSink_PinOffsetChanged);
                m_eventSink.SizeChanged -= new SizeChangedEventHandler(m_eventSink_SizeChanged);
                m_eventSink.FlipChanged -= new FlipChangedEventHandler(m_eventSink_FlipChanged);
                m_eventSink.RotationChanged -= new RotationChangedEventHandler(m_eventSink_RotationChanged);
            }

            // history for PseudoGroup is not tracked
            m_mgrHistory = null;
        }

        /// <summary>
        /// Renders the children.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        protected override void RenderChildren(Graphics gfx)
        {
            foreach (Node nodeCur in this.Nodes)
            {
                nodeCur.Draw(gfx);
            }
        }

        /// <summary>
        /// Gets the bounding rect in local coordinates.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>The bounding rect.</returns>
        protected override RectangleF GetBoundingRect(NodeCollection nodes)
        {
            int nLength = nodes.Count;
            RectangleF rcBoundsToReturn = RectangleF.Empty;

            if (nLength > 0)
            {
                RectangleF rcBounds;
                rcBoundsToReturn = ((IUnitIndependent)nodes[0]).GetBoundingRectangle(MeasureUnits.Pixel, true);

                for (int i = 1; i < nLength; i++)
                {
                    rcBounds = ((IUnitIndependent)nodes[i]).GetBoundingRectangle(MeasureUnits.Pixel, true);
                    rcBoundsToReturn = RectangleF.Union(rcBoundsToReturn, rcBounds);
                }
            }
            return rcBoundsToReturn;
        }

        /// <summary>
        /// Updates node's refresh rect
        /// </summary>
        /// <remarks>
        /// Uncludes all contiguous data like ports, labels etc.
        /// </remarks>
        protected override void UpdateRefreshRect()
        {
            RectangleF rcRefreshRect = new RectangleF();
            int nChildCount = this.Nodes.Count;

            Node child;
            Matrix mtxParent;

            // generate rect from children refresh rect
            if (nChildCount > 0)
            {
                child = this.Nodes[0];
                mtxParent = HandlesHitTesting.GetParentsTransformations(child);
                rcRefreshRect = Geometry.AppendMatrix(child.RefreshRect, mtxParent);

                for (int i = 1; i < nChildCount; i++)
                {
                    child = this.Nodes[i];
                    mtxParent = HandlesHitTesting.GetParentsTransformations(child);
                    rcRefreshRect = RectangleF.Union(rcRefreshRect, Geometry.AppendMatrix(child.RefreshRect, mtxParent));
                }
            }

            // set new refresh rect
            m_rectRefresh = rcRefreshRect;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Refresh the instance.
        /// </summary>
        public void Refresh()
        {
            UpdateGroupInfo();
        }
        #endregion

        #region Class helper methods
        private void UpdateChildrenContainer()
        {
            foreach (Node child in this.Nodes)
                child.Container = this;
        }

        private bool QuiteBridgingSet(bool newValue)
        {
            Model model = this.Nodes[0].Root;
            bool bLineBridgingEnabled = false;

            if (model != null)
            {
                SafeHistoryPause();
                bLineBridgingEnabled = model.LineBridgingEnabled;
                model.LineBridgingEnabled = newValue;
                SafeHistoryResume();
            }

            return bLineBridgingEnabled;
        }
        private void UpdateSize(Node node, float fGroupScaleFactorX, float fGroupScaleFactorY)
        {
            node.Scale(fGroupScaleFactorX, fGroupScaleFactorY);
        }
        private void UpdatePinRelativeToParent(ref PointF ptNodePin, Matrix mtxParent)
        {
            ptNodePin = Geometry.AppendMatrix(ptNodePin, mtxParent);
        }
        private PointF GetNodePinPGRelative(Node node, Matrix mtxParent)
        {
            //// get current node's pin position
            PointF[] pts = new PointF[1];
            pts[0] = ((IUnitIndependent)node).GetPinPoint(MeasureUnits.Pixel);
            //// convert pin position to group relative coordinates
            mtxParent.TransformPoints(pts);
            //// return transformed pin
            return pts[0];
        }
        private new void UpdateGroupInfo()
        {
            if (!m_bLock && this.Nodes.Count > 0)
            {
                m_bLock = true;

                // bounding rect values are in Pixels
                RectangleF rcNewBounding = RectangleF.Empty;
                
                // get group transformations
                Matrix mtxPseudo = this.GetTransformations();
                this.AppendFlipTransforms(mtxPseudo);
                mtxPseudo.Invert();

                for (int i = 0, nLength = this.Nodes.Count; i < nLength; i++)
                {
                    Matrix mtxNode = HandlesHitTesting.GetParentsTransformations(this.Nodes[i], true);
                    mtxNode.Multiply(mtxPseudo, MatrixOrder.Append);

                    rcNewBounding = (i == 0) ? this.Nodes[i].GraphicsPath.GetBounds(mtxNode) :
                        RectangleF.Union(this.Nodes[i].GraphicsPath.GetBounds(mtxNode), rcNewBounding);
                }

                // lock update bridges
                bool bPrevLock = m_bLockUpdate;
                m_bLockUpdate = true;

                // update pin position, pin offset and size
                UpdateBoundsInfo(rcNewBounding);

                m_bLockUpdate = bPrevLock;

                UpdateBoundingRectangle();
                m_bLock = false;
            }
        }
        private void UpdateChildNodeRotationAngle(Node node, float fRotationChange)
        {
            if (node == null) throw new ArgumentNullException("node");

            // update node's pin position + rotation angle
            // PIN POSITION
            // ----------------
            // create rotate transformation matrix
            PointF ptPin = ((IUnitIndependent)this).GetPinPoint(MeasureUnits.Pixel);

            // append parent transformation
            PointF[] ptsPin = new PointF[] { ptPin };
            Matrix mtxParentTransform = HandlesHitTesting.GetParentsTransformations(node, false);
            mtxParentTransform.Invert();
            mtxParentTransform.TransformPoints(ptsPin);
            ptPin = ptsPin[0];

            bool bNodeFlipX = HandlesHitTesting.GetParentsFlipX(node, false);
            bool bNodeFlipY = HandlesHitTesting.GetParentsFlipY(node, false);

            Matrix mtxUpd = new Matrix();

            // append flip to rotate node pin
            if (NeedFlipRotateAngle(this.FlipX, this.FlipY) || NeedFlipRotateAngle(bNodeFlipX, bNodeFlipY))
            {
                fRotationChange = 360 - fRotationChange;
                fRotationChange = Geometry.ConvertToPartCircle(fRotationChange);
            }

            mtxUpd.RotateAt(fRotationChange, ptPin);

            bNodeFlipX = node.FlipX;
            bNodeFlipY = node.FlipY;

            // ROTATION ANGLE
            node.RotationAngle += (NeedFlipRotateAngle(this.FlipX, this.FlipY)
                || NeedFlipRotateAngle(bNodeFlipX, bNodeFlipY)) ? -fRotationChange : fRotationChange;

            // update node's location
            UpdateNodeLocation(node, mtxUpd);
        }

        /// <summary>
        /// Check if rotation angle must be invert.
        /// </summary>
        /// <param name="bFlipX">FlipX value container.</param>
        /// <param name="bFlipY">FlipY value container.</param>
        /// <returns>If <b>true</b> - rotation angle must be negative value, elsewere - false.</returns>
        private bool NeedFlipRotateAngle(bool bFlipX, bool bFlipY)
        {
            return (bFlipX || bFlipY) && !(bFlipX && bFlipY);
        }
        private void UpdateNodeLocation(Node node, Matrix mtxUpd)
        {
            PointF[] pts = new PointF[1];
            pts[0] = ((IUnitIndependent)node).GetPinPoint(MeasureUnits.Pixel);

            // append transformations
            mtxUpd.TransformPoints(pts);
            
            // update node's pin
            ((IUnitIndependent)node).SetPinPoint(pts[0], MeasureUnits.Pixel);
        }

        /// <summary>
        /// Updates the node location.
        /// </summary>
        /// <param name="nodeCur">The node cur.</param>
        /// <param name="composite">The node composite.</param>
        private void UpdateNodeLocation(Node nodeCur, ICompositeNode composite)
        {
            Node compositeNode = composite as Node;
            if (compositeNode == null)
                return;

            MeasureUnits units = MeasureUnits.Pixel;
            
            // get parent bounds info
            float fParentAngle = compositeNode.RotationAngle;

            // get node's Pin location unit independent value
            PointF[] ptsNodePinPoint = new PointF[] { ((IUnitIndependent)nodeCur).GetPinPoint(units) };

            // get composite node transformation
            Matrix mtxTransfrom = compositeNode.GetTransformations();
            compositeNode.AppendFlipTransforms(mtxTransfrom);
            mtxTransfrom.TransformPoints(ptsNodePinPoint);

            // update pin point
            ((IUnitIndependent)nodeCur).SetPinPoint(ptsNodePinPoint[0], MeasureUnits.Pixel);

            // update rotation angle.
            fParentAngle = NeedFlipRotateAngle(nodeCur.FlipX, nodeCur.FlipY) ? -fParentAngle : fParentAngle;
            nodeCur.RotationAngle += Geometry.ConvertToFullCircle(fParentAngle);

            // update node flip flags
            nodeCur.FlipX = compositeNode.FlipX ? !nodeCur.FlipX : nodeCur.FlipX;
            nodeCur.FlipY = compositeNode.FlipY ? !nodeCur.FlipY : nodeCur.FlipY;

            // again call method if composite node has parent
            if (compositeNode.Parent != null)
                UpdateNodeLocation(nodeCur, compositeNode.Parent);
        }
        private bool CanRotate()
        {
            EditStyle childrenProtection = new EditStyle();
            HandlesHitTesting.GetSumEditStyle(this, ref childrenProtection);

            return childrenProtection.AllowRotate;
        }
        private bool CanResize()
        {
            EditStyle childrenProtection = new EditStyle();
            HandlesHitTesting.GetSumEditStyle(this, ref childrenProtection);

            return childrenProtection.AllowChangeHeight && childrenProtection.AllowChangeWidth;
        }
        private bool CanMove()
        {
            EditStyle childrenProtection = new EditStyle();
            HandlesHitTesting.GetSumEditStyle(this, ref childrenProtection);

            return childrenProtection.AllowMoveX && childrenProtection.AllowMoveY;
        }
        #endregion

        #region Class events
        private void m_eventSink_PinPointChanged(PinPointChangedEventArgs evtArgs)
        {
            if (!m_bLock && this.Nodes.Contains(evtArgs.NodeAffected as Node) && !(evtArgs.NodeAffected is IEndPointContainer))
            {
                UpdateGroupInfo();
            }
        }
        private void m_eventSink_PinOffsetChanged(PinOffsetChangedEventArgs evtArgs)
        {
            if (!m_bLock && this.Nodes.Contains(evtArgs.NodeAffected as Node) && !(evtArgs.NodeAffected is IEndPointContainer))
            {
                UpdateGroupInfo();
            }
        }
        private void m_eventSink_SizeChanged(SizeChangedEventArgs evtArgs)
        {
            if (!m_bLock && this.Nodes.Contains(evtArgs.NodeAffected as Node) && !(evtArgs.NodeAffected is IEndPointContainer))
            {
                UpdateGroupInfo();
            }
        }
        private void m_eventSink_FlipChanged(FlipChangedEventArgs evtArgs)
        {
            if (!m_bLock && this.Nodes.Contains(evtArgs.NodeAffected as Node) && !(evtArgs.NodeAffected is IEndPointContainer))
            {
                UpdateGroupInfo();
            }
        }
        private void m_eventSink_RotationChanged(RotationChangedEventArgs evtArgs)
        {
            if (!m_bLock && this.Nodes.Contains(evtArgs.NodeAffected as Node) && !(evtArgs.NodeAffected is IEndPointContainer))
            {
                UpdateGroupInfo();
            }
        }
        #endregion
    }
}
