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
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Diagram
{    
    /// <summary>
    /// Interactive tool for inserting and deleting Connection Point(Port) on a node.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ConnectionPoint"/>
    /// </remarks>
    public class ConnectionPointTool
        : UITool
    {
        #region Class members
        private ConnectionPoint m_connectionPoint;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionPointTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public ConnectionPointTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("ConnectionPointTool"))
        {
            this.ToolCursor = this.ActionCursor = Resources.Cursors.Port;
            this.SingleActionTool = false;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the <see cref="ConnectionPoint"/> to be added on a node.
        /// </summary>
        /// <value>The connection point.</value>
        [Browsable(true)]
        [Description("Specifies the connection point can be added on a node")]
        public ConnectionPoint ConnectionPoint
        {
            get
            {
                if (m_connectionPoint == null)
                    m_connectionPoint = new ConnectionPoint();
                return m_connectionPoint;
            }
            set
            {
                if (value != m_connectionPoint)
                    value = m_connectionPoint;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Processes the mouse down.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The connection point tool.</returns>
        public override Tool ProcessMouseDown(MouseEventArgs evtArgs)
        {
            this.ToolToActivate = SingleActionTools.None;
            base.ProcessMouseDown(evtArgs);

            PointF ptCur = this.Controller.ConvertToModelCoordinates(new PointF(evtArgs.X, evtArgs.Y));           
            m_connectionPoint = HandlesHitTesting.GetConnectionPointAtPoint(this.Controller.Model.Nodes, ptCur);            

            if (evtArgs.Button == MouseButtons.Left && !this.InAction)
            {
                this.InAction = true;
            }
            return this;
        }

        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The connection point tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = base.ProcessMouseMove(evtArgs);
            //Updates the tool cursor
            UpdatePortCursor(new PointF(evtArgs.X, evtArgs.Y));
            
            return toolToReturn;
        }
        
        /// <summary>
        /// Processes the mouse up.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The connection point tool.</returns>
        public override Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = base.ProcessMouseUp(evtArgs);
            if (this.InAction)
            {
                this.InAction = false;
                CompleteAction();
            }
            return toolToReturn;
        }
        #endregion

        #region Class helper methods
        private void CompleteAction()
        {
            //get the top most node under mouse
            Node node = null;
            if (m_connectionPoint == null)
            {
                node = this.Controller.GetNodeUnderMouse(this.CurrentPoint) as Node;
                if (node != null)
                {
                    //calc port offset on node
                    PointF offset = node.ConvertToNodeCoordinates(this.Controller.ConvertToModelCoordinates(this.CurrentPoint));
                    //add new port
                    m_connectionPoint = new ConnectionPoint(this.ConnectionPoint);
                    m_connectionPoint.OffsetX = offset.X;
                    m_connectionPoint.OffsetY = offset.Y;
                    node.Ports.Add(m_connectionPoint);
                    this.Controller.UpdateInfo.UpdateRefreshRect(this.Controller.ConvertFromModelToClientCoordinates(Geometry.ConvertRectangle(node.BoundingRectangle)));
                }
            }
            else if (Control.ModifierKeys == Keys.Control)
            {
                //remove the port if it exists under mouse
                if (m_connectionPoint != null)
                {
                    node = m_connectionPoint.Container;
                    node.Ports.Remove(m_connectionPoint);
                    this.Controller.UpdateInfo.UpdateRefreshRect(this.Controller.ConvertFromModelToClientCoordinates(Geometry.ConvertRectangle(node.BoundingRectangle)));
                }
            }          
            else
            {
                //calculates the port move offset
                PointF ptStart = GetStartPoint(false);
                SizeF szOffsetCur = SizeF.Empty;
                szOffsetCur.Width = this.CurrentPoint.X - ptStart.X;
                szOffsetCur.Height = this.CurrentPoint.Y - ptStart.Y;
                szOffsetCur = this.Controller.ConvertToModelCoordinates(szOffsetCur);

                PointF curPosition = PointF.Empty;
                if (m_connectionPoint is CentralPort)
                    curPosition = m_connectionPoint.GetPosition();
                else
                    curPosition = new PointF(m_connectionPoint.OffsetX, m_connectionPoint.OffsetY);

                // updates the port location
                m_connectionPoint.OffsetX = curPosition.X + szOffsetCur.Width;
                m_connectionPoint.OffsetY = curPosition.Y + szOffsetCur.Height;
            }
        }

        /// <summary>
        /// Updates the tool cursor
        /// </summary>
        /// <param name="ptCur">Current mouse postion</param>
        private void UpdatePortCursor(PointF ptCur)
        {
            ptCur = this.Controller.ConvertToModelCoordinates(ptCur);
            //Gets the connection point under mouse
            ConnectionPoint port = HandlesHitTesting.GetConnectionPointAtPoint(this.Controller.Model.Nodes, ptCur);
            //Updates the cursor
            if (Control.ModifierKeys != Keys.Control && port != null && port.Visible && (port is CentralPort ? ((CentralPort)port).DrawCentralPort : true))
                this.ToolCursor = this.ActionCursor = Cursors.SizeAll;
            else
                this.ToolCursor = this.ActionCursor = Resources.Cursors.Port;
        }
        #endregion
    }
}