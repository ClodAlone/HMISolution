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
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Interactive tool for panning (scrolling) the view of a diagram.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.Origin"/>
    /// </remarks>
    public class PanTool
        : UITool
    {
        #region Class members
        private PointF m_ptViewOrigin;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PanTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public PanTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("PanTool"))
        {
            this.ToolCursor = this.ActionCursor = Resources.Cursors.PanReady;
            this.SingleActionTool = false;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the view origin.
        /// </summary>
        /// <value>The view origin.</value>
        protected PointF ViewOrigin
        {
            get 
            { 
                return m_ptViewOrigin; 
            }
            set
            {
                if (m_ptViewOrigin != value)
                    m_ptViewOrigin = value;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Processes the mouse down.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The pan tool.</returns>
        public override Tool ProcessMouseDown(MouseEventArgs evtArgs)
        {
            this.ToolToActivate = SingleActionTools.None;
            base.ProcessMouseDown(evtArgs);

            if (evtArgs.Button == MouseButtons.Left && !this.InAction)
            {
                this.InAction = true;
            }

            this.ViewOrigin = this.Controller.Viewer.Origin;

            return this;
        }

        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The pan tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = base.ProcessMouseMove(evtArgs);

            if (this.InAction)
            {
                this.CurrentCursor = Resources.Cursors.Panning;
                UpdateOrigin(evtArgs.X, evtArgs.Y);
            }
            else
            {
                this.CurrentCursor = Resources.Cursors.PanReady;
            }

            return toolToReturn;
        }

        /// <summary>
        /// Processes the mouse up.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The pan tool.</returns>
        public override Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = base.ProcessMouseUp(evtArgs);

            if (this.InAction)
            {
                this.InAction = false;

                UpdateOrigin(evtArgs.X, evtArgs.Y);
            }

            return toolToReturn;
        }

        /// <summary>
        /// Processes the double click.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The pan tool.</returns>
        public override Tool ProcessDoubleClick(MouseEventArgs evtArgs)
        {
            if (this.InAction)
            {
                this.InAction = false;
                View view = this.Controller.View;

                if (view != null)
                {
                    PointF newOrigin;
                    Model model = this.Controller.Model;

                    if (model != null)
                    {
                        newOrigin = new PointF(model.Bounds.Location.X, model.Bounds.Location.Y);
                    }
                    else
                    {
                        newOrigin = PointF.Empty;
                    }

                    // change cursor
                    this.CurrentCursor = this.ToolCursor;

                    if (model != null)
                        model.BeginUpdate();

                    // apply new origin
                    view.Origin = newOrigin;
                    
                    // reset scroll virtual bounds
                    view.ScrollVirtualBounds = RectangleF.Empty;

                    if (model != null)
                        model.EndUpdate();
                }
            }

            return base.ProcessDoubleClick(evtArgs);
        }

        /// <summary>
        /// Activates the tool.
        /// </summary>
        public override void ActivateTool()
        {
            base.ActivateTool();

            this.SingleActionTool = false;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Update origin offset.
        /// </summary>
        /// <param name="dMouseX">The mouse position by X axis.</param>
        /// <param name="dMouseY">The mouse position by Y axis.</param>
        private void UpdateOrigin(double dMouseX, double dMouseY)
        {
            double dMagnification = this.Controller.Viewer.Magnification / 100d;
            double dWidthOffset = this.StartPoint.X - dMouseX;
            double dHeightOffset = this.StartPoint.Y - dMouseY;

            dWidthOffset = dWidthOffset / dMagnification + this.ViewOrigin.X;
            dHeightOffset = dHeightOffset / dMagnification + this.ViewOrigin.Y;

            PointF newOrigin = new PointF((float)dWidthOffset, (float)dHeightOffset);

            this.Controller.View.Origin = newOrigin;
        }
        #endregion
    }
}
