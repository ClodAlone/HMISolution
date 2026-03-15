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
    /// Interactive tool for zooming in and out of a diagram's view.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Left click increases magnification by 25%. Right click decreases
    /// magnification by 25%.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View.Magnification"/>
    /// </remarks>
    public class ZoomTool : UITool
    {
        #region initalize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public ZoomTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("ZoomTool"))
        {
            this.ToolCursor = this.ActionCursor = Resources.Cursors.Zoom;
            this.SingleActionTool = false;
        }
        #endregion

        #region Class contants
        /// <summary>
        /// Magnification text size near cursor.
        /// </summary>
        private const float c_fZOOM_TEXT_HEIGHT = 7f;

        /// <summary>
        /// Magnification font family near cursor.
        /// </summary>
        private const string c_strZOOM_TEXT_FAMILY = "Arial";

        /// <summary>
        /// Default magnification value.
        /// </summary>
        private const float c_fDEF_MAGNIFICATION = 100f;
        #endregion

        #region fields
        private int zoomIncrement = 25;
        private int minMagnification = 10;
        private int maxMagnification = 1000;
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the amount to zoom each time the mouse is clicked.
        /// </summary>
        public int ZoomIncrement
        {
            get
            {
                return this.zoomIncrement;
            }
            set
            {
                this.zoomIncrement = value;
            }
        }

        /// <summary>
        /// Gets or sets minimum magnification value the tool will zoom to.
        /// </summary>
        public int MinimumMagnification
        {
            get
            {
                return this.minMagnification;
            }
            set
            {
                if (value >= 1)
                {
                    this.minMagnification = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets maximum magnification value the tool will zoom to.
        /// </summary>
        public int MaximumMagnification
        {
            get
            {
                return this.maxMagnification;
            }
            set
            {
                if (value <= 1000)
                {
                    this.maxMagnification = value;
                }
            }
        }

        #endregion

        #region override
        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public override void Draw(System.Drawing.Graphics gfx)
        {
            if (this.InAction)
            {
                // draw selection rectangle
                GraphicsState save = gfx.Save();

                // reset graphics transforms
                gfx.PageScale = 1f;
                gfx.Transform = new Matrix();

                // calc frame recatngle
                System.Drawing.Rectangle rcFrame = this.WorkRect;
                rcFrame.Inflate(-1, -1);

                // draw selection frame
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(25, Color.Blue)))
                using (Pen outline = new Pen(Color.Black))
                {
                    gfx.FillRectangle(brush, rcFrame);
                    gfx.DrawRectangle(outline, rcFrame);
                }

                // restore graphics state
                gfx.Restore(save);
            }

            base.Draw(gfx);
        }

        /// <summary>
        /// Processes the mouse down.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseDown(MouseEventArgs evtArgs)
        {
            this.ToolToActivate = SingleActionTools.None;
            Tool toolToReturn = base.ProcessMouseDown(evtArgs);
            this.InAction = true;

            return toolToReturn;
        }

        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = base.ProcessMouseMove(evtArgs);
            UpdateCursor();

            this.CanRender = false;

            if (this.InAction)
            {
                // update work rect
                UpdateWorkRect();
                this.CanRender = true;
            }

            return toolToReturn;
        }

        /// <summary>
        /// Processes the mouse up.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = base.ProcessMouseUp(evtArgs);

            if (this.Controller.Viewer != null && this.InAction)
            {
                this.InAction = false;

                float magWidth = this.Controller.View.Magnification;
                System.Drawing.Rectangle rcZoom = this.WorkRect;

                if (rcZoom.Width != 0 && rcZoom.Height != 0)
                {
                    // fit viewport to frame
                    ZoomFrame(rcZoom);
                }
                else
                {
                    if (evtArgs.Button == MouseButtons.Left)
                    {
                        magWidth += this.zoomIncrement;
                    }
                    else if (evtArgs.Button == MouseButtons.Right)
                    {
                        magWidth -= this.zoomIncrement;
                    }

                    this.SetMagnification(magWidth);
                }

                if (this.SingleActionTool)
                    toolToReturn = null;
            }

            // update work rect
            UpdateWorkRect();

            return toolToReturn;
        }

        /// <summary>
        /// Processes the key down.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessKeyDown(KeyEventArgs evtArgs)
        {
            if (evtArgs.KeyCode == Keys.Escape)
            {
                // set default value
                this.SetMagnification(c_fDEF_MAGNIFICATION);
            }

            return base.ProcessKeyDown(evtArgs);
        }
        #endregion

        #region helper methods
        private void UpdateWorkRect()
        {
            this.WorkRectPrev = this.WorkRect;
            this.WorkRect = Geometry.ConvertRectangle(GetFrameRectangle(false));
        }

        /// <summary>
        /// Fit viewport to given frame.
        /// </summary>
        /// <param name="rcZoom">The zoom frame.</param>
        protected virtual void ZoomFrame(System.Drawing.Rectangle rcZoom)
        {
            // get magnification value from current zoom frame
            float magWidth = GetZoomFrameMagnification(rcZoom);
            //Gets the Diagram view's current zoom type.
            ZoomType viewType = this.Controller.View.ZoomType;
            //sets the View's current zoomtype as 'TopLeft' in order to perform the frame zooming.
            this.Controller.View.ZoomType = ZoomType.TopLeft;
            // Zoom In operation
            if (this.StartPoint.Y < this.CurrentPoint.Y)
            {
                // append new magnification with origin offset
                if (magWidth != this.Controller.View.Magnification)
                {
                    Point ptLocation = new Point(Math.Min(this.StartPoint.X, this.CurrentPoint.X), this.StartPoint.Y);
                    this.Controller.View.Origin = this.Controller.ConvertToModelCoordinates(ptLocation);
                    this.SetMagnification(magWidth);
                }
            }
            else
            {
                // append new magnification with origin offset
                this.SetCenterMagnification(magWidth);
            }
            //revert back the view's zoomtype to current zoomtype
            this.Controller.View.ZoomType = viewType;
        }
        private float GetZoomFrameMagnification(System.Drawing.Rectangle rcZoom)
        {
            // get controller objects
            Controller controller = this.Controller;
            Control control = controller.Viewer as Control;
            View view = controller.View;

            // get view bounds
            float magWidth = view.Magnification;
            PointF ptOrigin = controller.ConvertFromModelToClientCoordinates(view.Origin);
            Size szSize = (control != null) ? control.ClientSize : view.Size;

            // convert bounds to model coordinates
            rcZoom = controller.ConvertToModelCoordinates(rcZoom);
            szSize = controller.ConvertToModelCoordinates(szSize);

            // calc increase factor
            double dWidth = (double)szSize.Width / (double)rcZoom.Width;
            double dHeight = (double)szSize.Height / (double)rcZoom.Height;

            // Zoom In operation
            if (this.StartPoint.Y < this.CurrentPoint.Y)
            {
                // cal new magnification factor
                magWidth *= (float)Math.Min(dWidth, dHeight);
            }
            else
            {
                // cal new magnification factor
                magWidth *= 1f - (float)Math.Max(1d / dWidth, 1d / dHeight);
            }

            return magWidth;
        }
        private void UpdateCursor()
        {
            System.Drawing.Rectangle rcZoom = this.WorkRect;
            Cursor zoomCursor = GetZoomCursor(GetZoomFrameMagnification(rcZoom));

            this.CurrentCursor = zoomCursor;
        }

        /// <summary>
        /// Gets the cursor with magnification value.
        /// </summary>
        /// <param name="magnification">The magnification.</param>
        /// <returns>The zoom cursor.</returns>
        private Cursor GetZoomCursor(float magnification)
        {
            Cursor cursor = this.ActionCursor;

            if (this.InAction && this.WorkRect.Width != 0 && this.WorkRect.Height != 0)
            {
                using (Bitmap bmpCursor = new Bitmap(cursor.Size.Width, cursor.Size.Height))
                using (Graphics gfx = Graphics.FromImage(bmpCursor))
                {
                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                    using (Font font = new Font(c_strZOOM_TEXT_FAMILY, c_fZOOM_TEXT_HEIGHT))
                    using (StringFormat format = new StringFormat())
                    {
                        format.Alignment = StringAlignment.Center;
                        System.Drawing.Rectangle rcFrame = new System.Drawing.Rectangle(
                                                                     0, 
                                                                     bmpCursor.Height - font.Height,
                                                                     bmpCursor.Width, 
                                                                     font.Height);

                        // round magnifcation value
                        string text = Math.Round(magnification, 1).ToString();
                        
                        // draw magnification number near tool cursor
                        gfx.DrawString(text, font, Brushes.Black, rcFrame, format);
                    }

                    cursor = RenderingHelper.CreateCursor(bmpCursor, cursor);
                }
            }

            return (cursor != null) ? cursor : this.ActionCursor;
        }

        /// <summary>
        /// Set magnification with align.
        /// </summary>
        /// <param name="magnification">The new magnification value.</param>
        private void SetCenterMagnification(float magnification)
        {
            magnification = Math.Max(this.MinimumMagnification, magnification);
            magnification = Math.Min(this.MaximumMagnification, magnification);

            // get size in model coordinates
            Size szSize = this.Controller.ConvertToModelCoordinates(((Control)this.Controller.Viewer).ClientSize);

            double dScale = this.Controller.View.Magnification / 100d;
            double dFactor = magnification / 100d;

            // calc offset and convert to model coordinates
            double x = (szSize.Width * (dFactor - dScale)) / 2 / dScale;
            double y = (szSize.Height * (dFactor - dScale)) / 2 / dScale;

            if (this.Controller.View.Magnification != magnification)
            {
                this.Controller.View.Origin = new PointF(this.Controller.View.Origin.X + (float)x, this.Controller.View.Origin.Y + (float)y);
                this.Controller.View.Magnification = magnification;
            }
        }
        private void SetMagnification(float magWidth)
        {
            magWidth = Math.Max(this.MinimumMagnification, magWidth);
            magWidth = Math.Min(this.MaximumMagnification, magWidth);

            this.Controller.View.Magnification = magWidth;
        }
        #endregion
    }
}