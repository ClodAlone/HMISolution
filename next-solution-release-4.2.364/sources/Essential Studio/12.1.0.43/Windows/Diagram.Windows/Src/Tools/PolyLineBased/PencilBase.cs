#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Base class for free-hand drawing.
    /// </summary>
    public class PencilBase : PolyLineBase
    {
        #region Initialize/Finalize Methods

        public PencilBase(DiagramController controller, string name)
            : base(controller, name)
        { 
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// when mouse move draw points.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The pencil tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            this.CurrentPoint = new Point(evtArgs.X, evtArgs.Y);
            
            // call base method before update
            if (this.InAction)
            {
                PointF newPoint = new PointF(evtArgs.X, evtArgs.Y);

                AddPoint(newPoint);
                
                // UpdateHelperNode();
                UpdateHelperNodeValue();
                UpdateCursor(CanAddNode(this.RenderingHelper));
            }

            return this;
        }

        /// <summary>
        /// When mouse up stop draw.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The pencil tool</returns>
        public override Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = this;
            this.CanRender = false;

            if (this.Points.Length <= c_nMIN_NODE_POINTS)
            {
                this.InAction = false;
            }

            if (this.InAction)
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
                this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);
            }

            return toolToReturn;
        }

        /// <summary>
        /// when mouse down start drawing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The pencil tool</returns>
        public override Tool ProcessMouseDown(MouseEventArgs evtArgs)
        {
            if (evtArgs.Button == MouseButtons.Right || evtArgs.Button == MouseButtons.Middle)
                this.InAction = false;
            else
                this.InAction = true;

            return this;
        }

        /// <summary>
        /// Called when the paint is completed.
        /// </summary>
        /// <param name="ptsShape">points to create polyline derived node from</param>
        /// <returns>The pencil tool</returns>
        protected override Node CompleteAction(PointF[] ptsShape)
        {
            Node node = null;
            if (ptsShape.Length > 2)
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
        /// Create the Node for painted line.
        /// </summary>
        /// <param name="pts">points to create polyline derived node from.</param>
        /// <returns>The line base.</returns>
        protected override Node CreateNode(System.Drawing.PointF[] pts)
        {
            LineBase toReturn = new PolylineNode(pts);

            SetDecorator(toReturn);

            return toReturn;
        }

        /// <summary>
        /// Draw the graphics path for particular points.
        /// </summary>
        /// <param name="pts">points to create path from</param>
        /// <returns>path created from given points</returns>
        protected override System.Drawing.Drawing2D.GraphicsPath CreatePath(System.Drawing.PointF[] pts)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLines(pts);

            return path;
        }

        #endregion

        #region HelpherMethods
        /// <summary>
        /// Updates the painted points into diagram
        /// </summary>
        protected void UpdateHelperNodeValue()
        {
            this.CanRender = false;

            if (this.InAction && this.Points.Length >= 1)
            {
                this.CanRender = true;

                // update work rects
                this.WorkRectPrev = this.WorkRect;
                UpdateWorkRect();
            }
        }

        /// <summary>
        /// When called origin Changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewOriginEventArgs"/> instance containing the event data.</param>
        protected override void OnOriginChanged(ViewOriginEventArgs evtArgs)
        {
            // update move action
            UpdateHelperNodeValue();

            base.OnOriginChanged(evtArgs);
        }

        /// <summary>
        /// When mouse move points added.
        /// </summary>
        /// <param name="newPoint">The new point.</param>
        private void AddPoint(PointF newPoint)
        {
            PointF ptToAdd = newPoint;
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
        #endregion
    }
}
