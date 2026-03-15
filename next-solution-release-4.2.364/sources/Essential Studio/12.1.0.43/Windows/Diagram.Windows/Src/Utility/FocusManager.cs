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
using Syncfusion.Windows.Forms.Diagram.Controls;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Class who's goal is to manage focus related actions.
    /// </summary>
    public class FocusManager
        : IServiceReferenceHolder
    {
        #region Class constants
        private const int c_nINFLATE_VALUE = 6;
        #endregion

        #region Class members
        /// <summary>
        /// Parent to draw on.
        /// </summary>
        private View m_view;
        private NodeCollection m_nodes;
        private Model m_document;

        /// <summary>
        /// Focused node.
        /// </summary>
        private Node m_nFocusedNode;

        /// <summary>
        /// Focus manager work rect.
        /// </summary>
        private System.Drawing.Rectangle m_rectWorkRect;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the working area.
        /// </summary>
        /// <value>The working area.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public System.Drawing.Rectangle WorkRect
        {
            get
            {
                System.Drawing.Rectangle rectToReturn = m_rectWorkRect;
                m_rectWorkRect = System.Drawing.Rectangle.Empty;

                return rectToReturn;
            }
        }

        /// <summary>
        /// Gets the sorted nodes.
        /// </summary>
        /// <value>The sorted nodes.</value>
        private NodeCollection SortedNodes
        {
            get
            {
                if (m_nodes == null)
                {
                    m_nodes = new NodeCollection();
                    m_nodes.UpdateReferences = false;
                }

                return m_nodes;
            }
        }

        /// <summary>
        /// Gets or sets the focused node.
        /// </summary>
        /// <value>The focused node.</value>
        public Node FocusedNode
        {
            get 
            { 
                return m_nFocusedNode; 
            }
            set
            {
                if (m_nFocusedNode != value)
                {
                    System.Drawing.Rectangle rectInvalid = System.Drawing.Rectangle.Empty;
                    RectangleF rectTemp;

                    if (m_nFocusedNode != null)
                    {
                        rectTemp = RenderingHelper.GetBoundingRectangle(m_nFocusedNode, MeasureUnits.Pixel);
                        rectTemp.Inflate(c_nINFLATE_VALUE, c_nINFLATE_VALUE);
                        rectInvalid = Geometry.ConvertRectangle(rectTemp);
                    }

                    if (value != null)
                    {
                        rectTemp = RenderingHelper.GetBoundingRectangle(value, MeasureUnits.Pixel);
                        rectTemp.Inflate(c_nINFLATE_VALUE, c_nINFLATE_VALUE);
                        System.Drawing.Rectangle rect = Geometry.ConvertRectangle(rectTemp);

                        if (rectInvalid.IsEmpty)
                        {
                            rectInvalid = rect;
                        }
                        else
                        {
                            rectInvalid = System.Drawing.Rectangle.Union(rectInvalid, rect);
                        }
                    }

                    m_rectWorkRect = rectInvalid;
                    m_nFocusedNode = value;
                }
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public virtual void Draw(Graphics gfx)
        {
            if (this.FocusedNode != null)
            {
                RectangleF rectBounding = ((IUnitIndependent)this.FocusedNode).GetBoundingRectangle(MeasureUnits.Pixel, false);
                rectBounding.Inflate(c_nINFLATE_VALUE, c_nINFLATE_VALUE);

                // save graphics state
                GraphicsState state = gfx.Save();

                gfx.SmoothingMode = SmoothingMode.HighSpeed;

                using (Pen pen = new Pen(Color.Black, 0f))
                {
                    pen.DashStyle = DashStyle.Dash;
                    gfx.DrawRectangle(pen, rectBounding.X, rectBounding.Y, rectBounding.Width, rectBounding.Height);
                }

                gfx.Restore(state);
            }
        }

        /// <summary>
        /// Moves focus forward.
        /// </summary>
        public void MoveFocusBackward()
        {
            // 1 - sort model nodeds
            UpdateSortedNodes();
            
            // 2 - move focus
            if (this.SortedNodes.Count > 0)
            {
                MoveFocus(Direction.Backward);
            }
        }

        /// <summary>
        /// Moves focus backward.
        /// </summary>
        public void MoveFocusForward()
        {
            // 1 - sort model nodeds
            UpdateSortedNodes();
            
            // 2 - move focus
            if (this.SortedNodes.Count > 0)
            {
                MoveFocus(Direction.Forward);
            }
        }
        #endregion

        #region Class helper methods
        private void UpdateSortedNodes()
        {
            m_nodes = new NodeCollection();
            m_nodes.UpdateReferences = false;
            m_nodes.AddRange(m_document.Nodes);

            m_nodes.Sort(new NodeRenderLocationComparer());
        }
        private void MoveFocus(Direction direction)
        {
            int nIndex = 0;

            if (this.FocusedNode != null)
            {
                nIndex = DetermineNodeBoudsInfoIndex(this.FocusedNode, direction);
            }
            else if (m_view.SelectionList.Count > 0)
            {
                Node nodeFirstSelected = this.m_view.SelectionList[0];
                nIndex = DetermineNodeBoudsInfoIndex(nodeFirstSelected, direction);
            }

            this.FocusedNode = this.SortedNodes[nIndex];
        }
        private int DetermineNodeBoudsInfoIndex(Node nodeFirstSelected, Direction direction)
        {
            int nIndex;
            Node node = this.SortedNodes.FindNodeByName(nodeFirstSelected.Name);

            nIndex = (node == null) ? -1 : this.SortedNodes.IndexOf(node);

            if (direction == Direction.Forward)
            {
                nIndex++;
            }
            else
            {
                nIndex--;
            }

            if (nIndex == this.SortedNodes.Count)
            {
                nIndex = 0;
            }
            else if (nIndex < 0)
            {
                nIndex = this.SortedNodes.Count - 1;
            }

            return nIndex;
        }
        #endregion

        #region IServiceReferenceHolder Members
        /// <summary>
        /// Updates the service references.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public virtual void UpdateServiceReferences(IServiceReferenceProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (provider == null)
            {
                m_view = null;
                m_document = null;
            }
            else
            {
                m_view = ( View )provider.ProvideServiceReference( typeof( View ).TypeHandle );
				m_document = ( Model )provider.ProvideServiceReference( typeof( Model ).TypeHandle );
            }
        }
        #endregion
    }
}
