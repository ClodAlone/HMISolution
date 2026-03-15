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
    /// Specifies the selection mode used by the <see cref="Syncfusion.Windows.Forms.Diagram.SelectTool"/>.
    /// </summary>
    public enum SelectMode
    {
        /// <summary>
        /// Specifies that objects fully enveloped by the tracking rectangle will be selected by the tool.
        /// </summary>
        Containing = 0,

        /// <summary>
        /// Specifies that objects intersecting the tracking rectangle will be selected by the tool.
        /// </summary>
        Intersecting
    }

    /// <summary>
    /// Interactive tool for changing the current selected nodes in a diagram.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controller.SelectionList"/>
    /// </remarks>
    public class SelectTool
        : UITool
    {
        #region Class members
        /// <summary>
        /// Defines selection mode.
        /// </summary>
        private SelectMode m_selectionMode;
        private LineStyle m_trackingstyle = null;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public SelectTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("SelectTool"))
        {
            this.SingleActionTool = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="selectMode">The select mode.</param>
        public SelectTool(DiagramController controller, SelectMode selectMode)
            : base(controller, Resources.Strings.Toolnames.Get("SelectTool"))
        {
            this.SingleActionTool = false;
            m_selectionMode = selectMode;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the selection mode for the tool.
        /// </summary>
        /// <value>A <see cref="Syncfusion.Windows.Forms.Diagram.SelectMode"/> value.</value>
        public SelectMode SelectMode
        {
            get { return m_selectionMode; }
            set { m_selectionMode = value; }
        }

        /// <summary>
        /// Gets or sets the tracking style.
        /// </summary>
        /// <value>The tracking style.</value>
        public LineStyle TrackingStyle
        {
            get
            {
                if (m_trackingstyle == null)
                {
                    m_trackingstyle = new LineStyle();
                    m_trackingstyle.LineWidth = 1;
                    m_trackingstyle.DashStyle = DashStyle.Dash;
                    m_trackingstyle.LineColor = Color.FromArgb(128, m_trackingstyle.LineColor);
                }
                return m_trackingstyle;
            }
            set
            {
                m_trackingstyle = value;
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
            if (this.InAction)
            {
                // draw selection rectangle
                GraphicsState save = gfx.Save();

                // reset graphics transforms
                gfx.PageScale = 1f;
                gfx.Transform = new Matrix();

                // Fix for D10311
                // calc frame recatngle
                // System.Drawing.Rectangle rcFrame = this.WorkRect;
                // rcFrame.X += (int)Math.Ceiling(this.TrackingStyle.LineWidth + 1);
                // rcFrame.Y += (int)Math.Ceiling(this.TrackingStyle.LineWidth + 1);
                // rcFrame.Width -= (int)Math.Ceiling(this.TrackingStyle.LineWidth * 2f + 1);
                // rcFrame.Height -= (int)Math.Ceiling(this.TrackingStyle.LineWidth * 2f + 1);

                // draw selection frame
                using (Pen pen = TrackingStyle.CreatePen())
                {
                    System.Drawing.Rectangle rect = this.WorkRect;
                    rect.Inflate(
                        (int)Math.Floor(-this.TrackingStyle.LineWidth),
                        (int)Math.Floor(-this.TrackingStyle.LineWidth));

                    Region clip = new Region(this.WorkRect);
                    gfx.Clip = clip;
                    gfx.DrawRectangle(pen, rect);
                }
                
                // restore graphics state
                gfx.Restore(save);
            }

            base.Draw(gfx);
        }

        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            // call base to update curret mouse position
            Tool toolToReturn = base.ProcessMouseMove(evtArgs);

            this.CanRender = false;

            if (this.InAction)
            {
                // update work rect
                this.WorkRectPrev = this.WorkRect;
                this.WorkRect = Geometry.ConvertRectangle(GetFrameRectangle(false));

                m_rulerDisplayRect = this.WorkRect;
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

            if (this.InAction)
            {
                // get selection rect
                RectangleF rectBouning = GetFrameRectangle(false);
                rectBouning = this.Controller.ConvertToModelCoordinates(rectBouning);

                // update selection
                UpdateSelection(rectBouning);

                if (rectBouning.Width != 0 || rectBouning.Height != 0)
                {
                    // update Controller's update rect manually
                    this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);
                }

                // than empty WorkRect --> which will be used bu rulers
                this.WorkRect = System.Drawing.Rectangle.Empty;
                this.InAction = false;
            }

            return toolToReturn;
        }
        #endregion

        #region Class helper methods
        private void UpdateSelection(RectangleF rectSelection)
        {
            NodeCollection selectionList = this.Controller.SelectionList;
            NodeCollection nodesToSelect = new NodeCollection();
            NodeCollection nodesToDeselect = new NodeCollection();
            NodeCollection nodesModel = this.Controller.Model.Nodes;
            NodeCollection nodesHit;

            // get nodes to select
            if (this.SelectMode == SelectMode.Intersecting)
            {
                nodesHit = HandlesHitTesting.GetNodesIntersecting(nodesModel, rectSelection);
            }
            else
            {
                nodesHit = HandlesHitTesting.GetNodesContainedBy(nodesModel, rectSelection);
            }

            // exclude non selectable nodes
            ExcludeNonSelectableNodes(nodesHit);

            // merge hit nodes with current SelectionList
            if (Control.ModifierKeys != Keys.Control)
            {
                nodesToDeselect.AddRange(selectionList);
            }

            // filter already selected node
            foreach (Node nodeToSelect in nodesHit)
            {
                if (!selectionList.Contains(nodeToSelect))
                {
                    nodesToSelect.Add(nodeToSelect);
                }
                else
                {
                    nodesToDeselect.Remove(nodeToSelect);
                }
            }

            if(nodesToDeselect.Count > 0)
            {
                if (this.Controller.Model.EnableSelectionListSubstitute && nodesToDeselect.Count > 1)
                    selectionList.Remove(nodesToDeselect);
                else
                    foreach (Node node in nodesToDeselect)
                        selectionList.Remove(node);
            }
            if (nodesToSelect.Count > 0)
            {
                if (this.Controller.Model.EnableSelectionListSubstitute && nodesToSelect.Count > 1)
                    selectionList.AddRange(nodesToSelect);
                else
                    foreach (Node node in nodesToSelect)
                        selectionList.Add(node);
            }
        }

        /// <summary>
        /// Excludes non selectable nodes
        /// </summary>
        /// <param name="nodesHit">NodeCollection to filter for non selelctable nodes</param>
        private static void ExcludeNonSelectableNodes(NodeCollection nodesHit)
        {
            NodeCollection nodesToRemove = new NodeCollection();
            
            // Exclude non selectable nodes
            foreach (Node node in nodesHit)
            {
                if (!EditStyle.CanSelect(node) || !node.Visible)
                {
                    nodesToRemove.Add(node);
                }
            }

            // remove non selectable nodes
            foreach (Node nodeTmp in nodesToRemove)
            {
                nodesHit.Remove(nodeTmp);
            }
        }
        #endregion
    }
}
