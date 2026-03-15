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
using System.ComponentModel;
using System.Drawing;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// The SubgraphTreeLayoutManager is a special type of the <see cref="DirectedTreeLayoutManager"/> 
    /// that enables the sub nodes of a diagram layout tree to have an orientation that is distinct from the parent node. 
    /// The sub graph orientation is specified using a <see cref="SubgraphTreeLayoutManager.SubgraphPreferredLayout"/> event 
    /// that the layout manager raises before positioning each set of sub nodes in the graph. 
    /// <see cref="Syncfusion.Windows.Forms.Diagram.GraphLayoutManager"/>
    /// <see cref="Syncfusion.Windows.Forms.Diagram.DirectedTreeLayoutManager"/>
    /// </summary>
    [Documentation.DocumentationExclude()]
    [ToolboxItem(false)]
    public class SubgraphTreeLayoutManager : DirectedTreeLayoutManager
    {
        #region Initialize / Finalize

        /// <summary>
        /// Initializes a new instance of the <see cref="SubgraphTreeLayoutManager"/> class.
        /// </summary>
        public SubgraphTreeLayoutManager()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubgraphTreeLayoutManager"/> class.
        /// </summary>
        /// <param name="model">The diagram <see cref="Syncfusion.Windows.Forms.Diagram.Model"/>.</param>
        /// <param name="fRotationDegree">The root to child orientation to be used for the tree.</param>
        /// <param name="nVerticalOffset">Specifies the vertical distance between adjacent nodes.</param>
        /// <param name="nHorizontalOffset">Specifies the horizontal distance between adjacent nodes.</param>
        public SubgraphTreeLayoutManager(Model model, float fRotationDegree, int nVerticalOffset, int nHorizontalOffset)
            : base(model, fRotationDegree, nVerticalOffset, nHorizontalOffset)
        { 
        }
        #endregion Initialize / Finalize

        #region Events

        /// <summary>
        /// Occurs before a sub node graph is positioned. 
        /// </summary>
        /// <remarks>
        /// Handling the SubgraphPreferredLayout event provides users with the ability to specify a distinct 
        /// orientation and bounds for the sub nodes.
        /// </remarks>
        [Description("Occurs before a sub node graph is positioned.")]
        public event SubgraphPreferredLayoutEventHandler SubgraphPreferredLayout;

        #endregion Events

        #region Event Handlers
        /// <summary>
        /// Fires the <see cref="SubgraphTreeLayoutManager.SubgraphPreferredLayout"/> event.
        /// </summary>
        /// <param name="e">Preferred layout event args.</param>
        private void OnSubgraphPreferredLayout(SubgraphPreferredLayoutEventArgs e)
        {
            if (SubgraphPreferredLayout != null)
                SubgraphPreferredLayout(this, e);
        }

        #endregion Event Handlers

        #region Overrides
        /// <summary>
        /// Applies the layout strategy on the diagram.
        /// </summary>
        protected override void DoGraphLayout()
        {
            Graph graph = this.SelectedNode as Graph;
            if (graph.GraphType == GraphType.OneParentDirectedTree)
                MakeChangingDirectionLayout(graph);
        }

        /// <summary>
        /// Performs the graph layout.
        /// </summary>
        /// <param name="graphSorting">The graph on which the layout is being applied.</param>
        [Documentation.DocumentationExclude()]
        protected void MakeChangingDirectionLayout(Graph graphSorting)
        {
            MakeChangingDirectionGraphLayout(graphSorting);
        }

        #endregion Overrides

        #region Helper Methods
        /// <summary>
        /// Makes the changing direction graph layout.
        /// </summary>
        /// <param name="graphToLayout">The graph to layout.</param>
        [Documentation.DocumentationExclude()]
        protected void MakeChangingDirectionGraphLayout(Graph graphToLayout)
        {
            // Give user option to perform custom layout
            TransformTree(graphToLayout);
            
            // Update Rank Dimensions hashtable
            graphToLayout.CheckRankDimensions();
            
            // Do layout to current graph
            MakeLayout(graphToLayout);
        }

        /// <summary>
        /// Transforms the tree.
        /// </summary>
        /// <param name="graphToLayout">The graph to layout.</param>
        [Documentation.DocumentationExclude()]
        private void TransformTree(Graph graphToLayout)
        {
            ArrayList lstToTransform = graphToLayout.TypeOrdered;

            for (int nCounter = lstToTransform.Count - 1; nCounter > 0; nCounter--)
            {
                ArrayList lstCurrRank = (ArrayList)lstToTransform[nCounter];
                LookForMarkedNodes(lstCurrRank, nCounter);
            }
        }

        [Documentation.DocumentationExclude()]
        private void LookForMarkedNodes(ArrayList lstCurrentRank, int nRank)
        {
            for (int nCounter = 0; nCounter < lstCurrentRank.Count; nCounter++)
            {
                GraphNode dtgnNode = lstCurrentRank[nCounter] as GraphNode;

                // if current node have children - raise the SubgraphPreferredLayoutEvent
                if (dtgnNode.Children.Count > 0)
                {
                    SubgraphPreferredLayoutEventArgs evtArgs = new SubgraphPreferredLayoutEventArgs(dtgnNode.Node);
                    OnSubgraphPreferredLayout(evtArgs);

                    if ((evtArgs.RotationDegree != 0) || (!evtArgs.SubgraphSize.IsEmpty))
                    {
                        // make graph -- current node as tree root
                        Graph graphSub = new Graph();
                        Graph graphCurrent = this.SelectedNode as Graph;

                        GraphNode gnNode = RelativesChangeRoutine(graphCurrent, nRank, dtgnNode, graphSub, lstCurrentRank);

                        ExtractSubGraph(dtgnNode, nRank, ref graphSub);

                        graphCurrent.GraphNodes.Remove(dtgnNode.FullName);
                        GraphNode topNode = graphSub.GetGraphFirstTopNode();
                        graphCurrent.GraphNodes.Add(topNode.FullName, graphSub);
                        gnNode.Children.Add(graphSub);

                        // make graph layout -- using base class
                        base.MakeLayout(graphSub);

                        graphSub.Rotate(evtArgs.RotationDegree);

                        graphSub.ResizeNodes = evtArgs.ResizeSubgraphNodes;
                        if (!(evtArgs.SubgraphSize.Height == 0))
                            graphSub.Height = evtArgs.SubgraphSize.Height;

                        if (!(evtArgs.SubgraphSize.Width == 0))
                            graphSub.Width = evtArgs.SubgraphSize.Width;
                    }
                }
            }
        }

        private GraphNode RelativesChangeRoutine(Graph graphCurrent, int nRank, GraphNode dtgnNode, Graph graphSub, ArrayList lstCurrentRank)
        {
            ArrayList lst2 = graphCurrent.TypeOrdered[nRank - 1] as ArrayList;
            GraphNode dtgnParent = dtgnNode.Parents[0] as GraphNode;

            int index = lst2.IndexOf(dtgnParent);
            dtgnParent = lst2[index] as GraphNode;
            index = dtgnParent.Children.IndexOf(dtgnNode);
            dtgnParent.Children[index] = graphSub;

            GraphNode gnNode = graphCurrent.GraphNodes[dtgnParent.Node.FullName] as GraphNode;
            gnNode.Children.Remove(dtgnNode.Node.FullName);

            // repace current node with graph
            int nIndex = lstCurrentRank.IndexOf(dtgnNode);
            lstCurrentRank.Remove(dtgnNode);
            lstCurrentRank.Insert(nIndex, graphSub);
            return gnNode;
        }
        #endregion Helper Methods
    }

    /// <summary>
    /// Delegate used for the <see cref="SubgraphTreeLayoutManager.SubgraphPreferredLayout"/> event.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="args">Preferred layout event args.</param>
    public delegate void SubgraphPreferredLayoutEventHandler(object sender, SubgraphPreferredLayoutEventArgs args);

    /// <summary>
    /// Event argument used for the <see cref="SubgraphTreeLayoutManager.SubgraphPreferredLayout"/> event.
    /// </summary>
    public class SubgraphPreferredLayoutEventArgs : EventArgs
    {
        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SubgraphPreferredLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public SubgraphPreferredLayoutEventArgs(INode node)
        {
            m_node = node;
        }
        #endregion

        #region Fields
        private INode m_node = null;
        private bool m_bResizeSubgraphNodes = false;
        private SizeF m_szSubgraphSize = SizeF.Empty;
        private float m_fRotationDegree = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the diagram node associated with this graph node object.
        /// </summary>
        public INode Node
        {
            get
            {
                return m_node;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether GraphNodes will be resized during the layout operation.
        /// </summary>
        public bool ResizeSubgraphNodes
        {
            get
            {
                return m_bResizeSubgraphNodes;
            }
            set
            {
                m_bResizeSubgraphNodes = value;
            }
        }

        /// <summary>
        /// Gets or sets the preferred sub graph size.
        /// If no values are specified the default size will be used.
        /// </summary>
        public SizeF SubgraphSize
        {
            get
            {
                return m_szSubgraphSize;
            }
            set
            {
                if (m_szSubgraphSize != value)
                    m_szSubgraphSize = value;
            }
        }

        /// <summary>
        /// Gets or sets sub graph orientation.
        /// </summary>
        public float RotationDegree
        {
            get
            {
                return m_fRotationDegree;
            }
            set
            {
                if (m_fRotationDegree != value)
                    m_fRotationDegree = value;
            }
        }
        #endregion
    }
}