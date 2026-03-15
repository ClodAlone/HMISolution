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
    /// The Tool used to move the Pinpoint.
    /// </summary>
    public class PinPointMoveTool
        : Tool
    {
        #region Class members
        /// <summary>
        /// Node which PinPoint is being moved.
        /// </summary>
        private Node m_nodePinOwner;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PinPointMoveTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="toolPrevious">The previous tool.</param>
        /// <param name="node">The node.</param>
        public PinPointMoveTool(DiagramController controller, Tool toolPrevious, INode node)
            : base(controller, Resources.Strings.Toolnames.Get("PinPointMoveTool"))
        {
            if (node == null)
                throw new ArgumentNullException("node");

            this.ToolCursor = this.ActionCursor = Cursors.Cross;
            m_nodePinOwner = (Node)node;
            m_toolPreceding = toolPrevious;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The move tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = base.ProcessMouseMove(evtArgs);

            // update cursor
            UpdateCursor(CanMove());

            return toolToReturn;
        }

        /// <summary>
        /// Processes the mouse up.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The move tool.</returns>
        public override Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            if (evtArgs.Button != MouseButtons.Right)
            {
                this.CanRender = false;
                this.InAction = false;

                CompleteAction();
            }

            return m_toolPreceding;
        }
        #endregion

        #region Class public method
        /// <summary>
        /// Checks whether pin point move can be performed.
        /// </summary>
        /// <returns>value indicating whether pin point can be moved</returns>
        public bool CanMove()
        {
            bool bSuccess = true;
            Model document = this.Controller.Model;

            if (m_nodePinOwner != null && document != null && document.BoundaryConstraintsEnabled)
            {
                PointF[] ptsPoint = new PointF[1];
                ptsPoint[0] = this.Controller.ConvertToModelCoordinates(this.CurrentPoint);

                Matrix mtxParent = HandlesHitTesting.GetParentsTransformations(m_nodePinOwner);
                mtxParent.TransformPoints(ptsPoint);

                RectangleF rectModel = MeasureUnitsConverter.ToPixels(document.Bounds, document.MeasurementUnits);
                bSuccess = rectModel.Contains(ptsPoint[0]);
            }

            return bSuccess;
        }
        #endregion

        #region Class helper methods
        private void CompleteAction()
        {
            if (CanMove())
            {
                Model model = this.Controller.Model;
                bool bBoundaryConsrtains = false;

                // Quite disable boundary constrains
                // Needed for property change pin point and offset without checking only one changes
                if (model != null)
                {
                    model.BeginUpdate();
                    model.HistoryManager.StartAtomicAction("Pin Move");

                    bBoundaryConsrtains = model.BoundaryConstraintsEnabled;
                    QuiteBoundarySet(false);
                }

                UpdatePinOffset();
                UpdatePinPoint();

                // Quite restore boundary constrains value
                if (model != null)
                {
                    QuiteBoundarySet(bBoundaryConsrtains);
                    model.HistoryManager.EndAtomicAction();
                    model.EndUpdate();
                }
            }
            else
            {
                this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);
            }
        }

        /// <summary>
        /// Updates moving pin point container node's pin point offset.
        /// </summary>
        private void UpdatePinOffset()
        {
            // get parents transforms
            Matrix mtxTemp = HandlesHitTesting.GetParentsTransformations(m_nodePinOwner, true);
            mtxTemp.Invert();
            
            // convert start or end points to model coordinates
            Point ptStart = this.Controller.ConvertToModelCoordinates(GetStartPoint(false));
            Point ptEnd = this.Controller.ConvertToModelCoordinates(this.CurrentPoint);

            // transform points
            Point[] pts = new Point[] { ptStart, ptEnd };
            mtxTemp.TransformPoints(pts);

            // update Pin Offset
            SizeF szNewPinOffset = ((IUnitIndependent)m_nodePinOwner).GetPinPointOffset(MeasureUnits.Pixel);

            szNewPinOffset.Width += pts[1].X - pts[0].X;
            szNewPinOffset.Height += pts[1].Y - pts[0].Y;

            ((IUnitIndependent)m_nodePinOwner).SetPinPointOffset(szNewPinOffset, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Updates moving pin point container node's pin point location.
        /// </summary>
        private void UpdatePinPoint()
        {
            // Pin point move offset
            PointF ptStart = GetStartPoint(false);
            SizeF szOffsetCur = SizeF.Empty;
            szOffsetCur.Width = this.CurrentPoint.X - ptStart.X;
            szOffsetCur.Height = this.CurrentPoint.Y - ptStart.Y;
            szOffsetCur = this.Controller.ConvertToModelCoordinates(szOffsetCur);

            // get parents transforms
            Matrix mtxTemp = HandlesHitTesting.GetParentsTransformations(m_nodePinOwner);
            mtxTemp.Invert();

            // get moving node location
            PointF ptPin = ((IUnitIndependent)m_nodePinOwner).GetPinPoint(MeasureUnits.Pixel);

            PointF[] pts = new PointF[] { new PointF(ptPin.X, ptPin.Y) };

            mtxTemp.TransformPoints(pts);

            PointF[] pts1 = new PointF[1];
            pts1[0].X = ptPin.X + szOffsetCur.Width;
            pts1[0].Y = ptPin.Y + szOffsetCur.Height;

            mtxTemp.TransformPoints(pts1);

            SizeF szOffset = SizeF.Empty;
            szOffset.Width = pts1[0].X - pts[0].X;
            szOffset.Height = pts1[0].Y - pts[0].Y;

            // update node pin location
            ((IUnitIndependent)m_nodePinOwner).SetPinPoint(
                new PointF(ptPin.X + szOffset.Width, ptPin.Y + szOffset.Height), MeasureUnits.Pixel);
        }
        #endregion
    }
}
