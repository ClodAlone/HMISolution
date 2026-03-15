#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Diagram;
using PropertyChangedEventArgs = Syncfusion.Windows.Forms.Diagram.PropertyChangedEventArgs;

namespace Syncfusion.Windows.Forms.Diagram.Controls
{
    /// <summary>
    /// This control is used to list all the nodes added the diagram model in a tree fashion.
    /// Its in-built contextmenu provides options to hide, rename, delete a node from the collection.
    /// It also provides support to add layers and delete or rename a layer through its context menu.
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(DocumentExplorer), "ToolboxIcons.DocumentExplorer.bmp")]
    [Description("Overview control that provides a perspective view of the diagram model.")]
    public class DocumentExplorer
        : TreeView
    {
        #region Class internal declarations
        /// <summary>
        /// Tree node types.
        /// </summary>
        protected enum TreeNodeType
        {
            /// <summary>
            /// Symbol node
            /// </summary>
            Node,

            /// <summary>
            /// Single Layer 
            /// </summary>
            Layer,

            /// <summary>
            /// Node collection
            /// </summary>
            NodeCollection,

            /// <summary>
            /// Layer collection
            /// </summary>
            LayersCollection
        }
        #endregion

        #region Class constants
     
        protected const string c_strNODE_COLLECTION = "NodeCollection";

        protected const string c_strLAYER_COLLECTION = "LayerCollection";

        protected const string c_strNODES = "Nodes";

        protected const string c_strLAYERS = "Layers";
        #endregion
        private System.Windows.Forms.ImageList smallImageList;
        private System.ComponentModel.IContainer components;

        #region Class members
        /// <summary>
        /// Array of Models to display content of.
        /// </summary>
        private Model[] m_models;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentExplorer"/> class.
        /// </summary>
        public DocumentExplorer()
        {
            m_models = new Model[1];
            this.InitializeComponent();
            this.ImageList = this.smallImageList;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Adds Model to DocumentExplorer
        /// </summary>
        /// <param name="model">Model to add</param>
        /// <returns>
        /// true - model successfully added
        /// false - adding model to document explorer failed
        /// </returns>
        public bool AttachModel(Model model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            return AddModel(model);
        }

        /// <summary>
        /// Removes Model from DocumentExplorerr
        /// </summary>
        /// <param name="model">Model to remove</param>
        /// <returns>
        /// true - model successfully removed
        /// false - removing model to document explorer failed
        /// </returns>
        public bool DetachModel(Model model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            bool bSuccess = false;
            TreeNode nodeTemp;

            UnSubscribeForModelEvents(model);
            
            // iterate through tree node's collection representing model collection
            for (int nCounter = 0, nLength = this.Nodes.Count; nCounter < nLength; nCounter++)
            {
                nodeTemp = this.Nodes[nCounter];

                // if match found
                // remove current tree node from node's tree
                if (nodeTemp.Tag.Equals(model))
                {
                    this.Nodes.Remove(nodeTemp);
                    
                    // stop iterating
                    break;
                }
            }

            return bSuccess;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Reflects given model's hierarchy in TreeView
        /// </summary>
        /// <param name="model">model to reflect hierarchy of</param>
        /// <returns>
        /// true - model successfully added
        /// false - adding model failed
        /// </returns>
        protected bool AddModel(Model model)
        {
            bool bSuccess = true;
            
            // 1 - Parse model
            try
            {
                // create treenode for new model
                TreeNode nodeModel = new TreeNode("Model");
                
                // parse model
                ParseModel(model, nodeModel);
                
                // Set reference to given model as a Tag
                nodeModel.Tag = model;
                
                // Add treenode to TreeView node's collection
                this.Nodes.Add(nodeModel);
            }
            catch (Exception)
            {
                bSuccess = false;
            }

            if (bSuccess)
            {
                // 2 - add model to m_models
                AddToModelsArray(model);

                // 3 - subscribe for model events
                SubscribeForModelEvents(model);
            }

            return bSuccess;
        }

        #region Parsing helper methods
        /// <summary>
        /// Parses model representation
        /// </summary>
        /// <param name="model">model to parse</param>
        /// <param name="nodeModel">TreeNode representing model ( root node for adding model )</param>
        protected void ParseModel(Model model, TreeNode nodeModel)
        {
            // 1 - parse model's nodes collection
            ParseChildren(model, nodeModel);
            
            // 2 - parse model's layers collection
            ParseLayers(model, nodeModel);
        }

        /// <summary>
        /// Parses the layers.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="nodeModel">The node model.</param>
        protected void ParseLayers(Model model, TreeNode nodeModel)
        {
            // Create root treenode for model layers
            TreeNode nodeLayers = new TreeNode(c_strLAYERS);
            nodeLayers.Tag = c_strLAYER_COLLECTION;
            nodeLayers.ImageIndex = 3;
            nodeLayers.SelectedImageIndex = 3;
            
            // Append to model node
            nodeModel.Nodes.Add(nodeLayers);
            
            // parse children
            ParseLayers(model.Layers, nodeLayers);
        }

        /// <summary>
        /// Parses the layers.
        /// </summary>
        /// <param name="layers">The layers.</param>
        /// <param name="nodeLayersParent">The node layers parent.</param>
        protected void ParseLayers(LayerCollection layers, TreeNode nodeLayersParent)
        {
            // Iterate though given node collection
            // and add corresponding TreeNode to nodesParent
            foreach (Layer layerCur in layers)
            {
                ParseLayer(layerCur, nodeLayersParent);
            }
        }

        /// <summary>
        /// Parses the layers.
        /// </summary>
        /// <param name="layers">The layers.</param>
        /// <param name="nodeLayersParent">The node layers parent.</param>
        protected void ParseLayers(ICollection layers, TreeNode nodeLayersParent)
        {
            // Iterate though given node collection
            // and add corresponding TreeNode to nodesParent
            foreach (Layer layerCur in layers)
            {
                ParseLayer(layerCur, nodeLayersParent);
            }
        }

        /// <summary>
        /// Parses the layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="nodeLayersParent">The node layers parent.</param>
        protected void ParseLayer(Layer layer, TreeNode nodeLayersParent)
        {
            // Create corresponding TreeNode
            TreeNode nodeLayer = new TreeNode(layer.Name);
            nodeLayer.Tag = layer;
            nodeLayer.ImageIndex = 4;
            nodeLayer.SelectedImageIndex = 4;
            
            // if given layer contains child nodes
            // reflect its hierarchy
            if (layer.Count > 0)
            {
                // !!! Layer doesn't provide direct reference to 
                // its chid nodes collection
                // Compose NodeCollection of given layer
                ParseChildren(layer, nodeLayer);
            }

            // Add corresponding TreeNode
            nodeLayersParent.Nodes.Add(nodeLayer);
        }

        /// <summary>
        /// Parses model's node collection
        /// </summary>
        /// <param name="model">model to parse node collection of</param>
        /// <param name="nodeModel">TreeNode to add node collection corresponding TreeNodes into</param>
        protected void ParseChildren(Model model, TreeNode nodeModel)
        {
            // Create root treenode for model nodes
            TreeNode nodeChildren = new TreeNode(c_strNODES);
            nodeChildren.Text = c_strNODES + " (" + model.Nodes.Count + ")";
            nodeChildren.Tag = c_strNODE_COLLECTION;
            nodeChildren.ImageIndex = 1;
            nodeChildren.SelectedImageIndex = 1;
            
            // Append to model node
            nodeModel.Nodes.Add(nodeChildren);
            
            // parse children
            ParseChildren(model.Nodes, nodeChildren);
        }

        /// <summary>
        /// Parses the children.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="nodesParent">The nodes parent.</param>
        protected void ParseChildren(NodeCollection nodes, TreeNode nodesParent)
        {
            // Iterate though given node collection
            // and add corresponding TreeNode to nodesParent
            foreach (INode nodeCur in nodes)
            {
                ParseNode(nodeCur, nodesParent);
            }
        }

        /// <summary>
        /// Parses the children.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="nodesParent">The nodes parent.</param>
        protected void ParseChildren(ICollection nodes, TreeNode nodesParent)
        {
            // Iterate though given node collection
            // and add corresponding TreeNode to nodesParent
            foreach (INode nodeCur in nodes)
            {
                ParseNode(nodeCur, nodesParent);
            }
        }

        /// <summary>
        /// Parses the children.
        /// </summary>
        /// <param name="nodeComposite">The node composite.</param>
        /// <param name="nodesParent">The nodes parent.</param>
        private void ParseChildren(ICompositeNode nodeComposite, TreeNode nodesParent)
        {
            int nLenght = nodeComposite.ChildCount;
            INode nodeTemp;

            for (int nCounter = 0; nCounter < nLenght; nCounter++)
            {
                nodeTemp = nodeComposite.GetChild(nCounter);
                ParseNode(nodeTemp, nodesParent);
            }
        }

        private void ParseChildren(Layer layer, TreeNode nodesParent)
        {
            foreach (INode node in layer.Nodes)
            {
                ParseNode(node, nodesParent);
            }
        }

        private void ParseNode(INode nodeCur, TreeNode nodesParent)
        {
            // Create corresponding TreeNode 
            TreeNode nodeTreeTemp = new TreeNode(nodeCur.Name);
            nodeTreeTemp.Tag = nodeCur;
            
            // if nodeCur is CompositeNode
            // reflect its hierarchy
            if (nodeCur is ICompositeNode)
            {
                ICompositeNode nodeComposite = nodeCur as ICompositeNode;

                if (nodeComposite != null)
                {
                    // Parse composite node's children
                    nodeTreeTemp.ImageIndex = 2;
                    nodeTreeTemp.SelectedImageIndex = 2;
                    ParseChildren(nodeComposite, nodeTreeTemp);
                    nodeTreeTemp.Text = (nodeComposite as INode).Name + " (" + nodeComposite.ChildCount + ")";
                }
            }
            else
            {
                nodeTreeTemp.Tag = nodeCur;
                nodeTreeTemp.ImageIndex = 1;
                nodeTreeTemp.SelectedImageIndex = 1;
            }

            // Add created TreeNode to nodesParent
            nodesParent.Nodes.Add(nodeTreeTemp);
        }
        #endregion

        #region Tree Search helpers
        /// <summary>
        /// Searches for TreeNode corresponding model node.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="node">The node.</param>
        /// <returns>The node found</returns>
        protected TreeNode FindTreeNode(Model model, TreeNodeType nodeType, INode node)
        {
            TreeNode nodeToReturn;
            
            // 1 - find TreeNode correponding to given model
            TreeNode nodeModel = FindModelCorrespondingTreeNode(model);
            
            // 2 - look for nodeType
            nodeToReturn = FindTreeNode(nodeModel, nodeType, node);

            return nodeToReturn;
        }

        /// <summary>
        /// Finds the tree node.
        /// </summary>
        /// <param name="nodeModel">The node model.</param>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="searchSubject">The search subject.</param>
        /// <returns>The node found.</returns>
        protected TreeNode FindTreeNode(TreeNode nodeModel, TreeNodeType nodeType, object searchSubject)
        {
            if (nodeModel == null)
                throw new ArgumentNullException("nodeModel");

            TreeNode nodeToReturn = null;
            bool bFound = false;

            for (int nCounter = 0, nLength = nodeModel.Nodes.Count; nCounter < nLength; nCounter++)
            {
                switch (nodeType)
                {
                    case TreeNodeType.NodeCollection:
                        nodeToReturn = nodeModel.Nodes[nCounter];

                        // if we found node we were looking for -- break
                        // else reset nodeToReturn value
                        if ((string)nodeToReturn.Tag == c_strNODE_COLLECTION)
                            bFound = true;
                        else
                            nodeToReturn = null;
                        break;
                    case TreeNodeType.LayersCollection:
                        nodeToReturn = nodeModel.Nodes[nCounter];

                        // if we found node we were looking for -- break
                        // else reset nodeToReturn value
                        if ((string)nodeToReturn.Tag == c_strLAYER_COLLECTION)
                            bFound = true;
                        else
                            nodeToReturn = null;
                        break;
                    case TreeNodeType.Node:
                        
                        // 1 - navigate to Nodes subtree
                        TreeNode nodeCollection = FindTreeNode(nodeModel, TreeNodeType.NodeCollection, null);
                        
                        // 2 - look for searching node
                        nodeToReturn = FindTreeNode(nodeCollection, searchSubject as INode);

                        if (nodeToReturn != null)
                            bFound = true;
                        break;
                    case TreeNodeType.Layer:
                        
                        // 1 - navigate to Layers subtree
                        TreeNode nodeLayers = FindTreeNode(nodeModel, TreeNodeType.LayersCollection, null);
                        
                        // 2 - look for searching layer
                        nodeToReturn = FindTreeNode(nodeLayers, searchSubject as Layer);
                        break;
                }

                if (bFound)
                    break;
            }

            return nodeToReturn;
        }

        /// <summary>
        /// Finds the tree node.
        /// </summary>
        /// <param name="nodeSubRoot">The node sub root.</param>
        /// <param name="searchSubject">The search subject.</param>
        /// <returns>The node found.</returns>
        protected TreeNode FindTreeNode(TreeNode nodeSubRoot, object searchSubject)
        {
            if (nodeSubRoot == null)
                throw new ArgumentNullException("nodeSubRoot");

            if (searchSubject == null)
                throw new ArgumentNullException("searchSubject");

            TreeNode nodeToReturn = null;

            if (nodeSubRoot.Nodes != null && nodeSubRoot.Nodes.Count > 0)
            {
                // iterate through given TreeNodes child nodes
                for (int nCounter = 0, nLength = nodeSubRoot.Nodes.Count; nCounter < nLength; nCounter++)
                {
                    nodeToReturn = nodeSubRoot.Nodes[nCounter];

                    // is match found -> break
                    if (nodeToReturn.Tag.Equals(searchSubject))
                        break;
                    else
                    {
                        // if current tree node has siblings
                        // search though its siblings collection,
                        // otherwise proceed to next sibling
                        if (nodeToReturn.Nodes != null && nodeToReturn.Nodes.Count > 0)
                        {
                            nodeToReturn = FindTreeNode(nodeToReturn, searchSubject);

                            if (nodeToReturn != null)
                                break;
                        }
                        else
                            nodeToReturn = null;
                    }
                }
            }

            return nodeToReturn;
        }

        /// <summary>
        /// Looks for given model's corresponding TreeNode.
        /// </summary>
        /// <param name="model">model to look for corresponding treenode</param>
        /// <returns>
        /// Null - if match not found,
        /// otherwise - match tree node
        /// </returns>
        protected TreeNode FindModelCorrespondingTreeNode(Model model)
        {
            TreeNode nodeToReturn = null;

            // iterate through tree node's collection representing model collection
            for (int nCounter = 0, nLength = this.Nodes.Count; nCounter < nLength; nCounter++)
            {
                nodeToReturn = this.Nodes[nCounter];

                // if match found stop iterating
                if (nodeToReturn.Tag.Equals(model))
                {
                    break;
                }
                else
                    nodeToReturn = null;
            }

            return nodeToReturn;
        }

        #endregion

        #region Context Menus
        /// <summary>
        /// Creates the layers pop up menu.
        /// </summary>
        /// <returns>The context menu.</returns>
        private ContextMenu CreateLayersPopUpMenu()
        {
            // Create menu
            ContextMenu menuToReturn = new ContextMenu();
            
            // Create items
            // MenuItem Add Layer
            MenuItem itemTemp = new MenuItem();
            itemTemp.Text = "Add Layer";
            itemTemp.Click += new EventHandler(Layers_Add);
            
            // Merge with context menu
            menuToReturn.MenuItems.Add(itemTemp);

            return menuToReturn;
        }

        /// <summary>
        /// Creates the layer pop up menu.
        /// </summary>
        /// <param name="forLayer">For layer.</param>
        /// <returns>The context menu</returns>
        private ContextMenu CreateLayerPopUpMenu(Layer forLayer)
        {
            // Create menu
            ContextMenu menuToReturn = new ContextMenu();
            
            // Create items
            // MenuItem Active
            MenuItem itemTemp = new MenuItem();
            itemTemp.Checked = forLayer.Enabled;
            itemTemp.Text = "Active";
            itemTemp.Click += new EventHandler(Layer_SetActive);
            
            // Merge with context menu
            menuToReturn.MenuItems.Add(itemTemp);

            // MenuItem Visible
            itemTemp = new MenuItem();
            itemTemp.Checked = forLayer.Visible;
            itemTemp.Text = "Visible";
            itemTemp.Click += new EventHandler(Node_SetVisibility);
            
            // Merge with context menu
            menuToReturn.MenuItems.Add(itemTemp);

            // MenuItem Rename
            itemTemp = new MenuItem();
            itemTemp.Text = "Rename";
            itemTemp.Click += new EventHandler(Node_Rename);
            
            // Merge with context menu
            menuToReturn.MenuItems.Add(itemTemp);

            // MenuItem Delete
            itemTemp = new MenuItem();
            itemTemp.Text = "Delete";
            itemTemp.Click += new EventHandler(Node_Delete);
            
            // Merge with context menu
            menuToReturn.MenuItems.Add(itemTemp);

            return menuToReturn;
        }

        /// <summary>
        /// Creates the node pop up menu.
        /// </summary>
        /// <param name="forNode">For node.</param>
        /// <returns>The context menu</returns>
        private ContextMenu CreateNodePopUpMenu(Node forNode)
        {
            // Create menu
            ContextMenu menuToReturn = new ContextMenu();
            MenuItem itemTemp;

            // MenuItem Visible
            itemTemp = new MenuItem();
            itemTemp.Checked = forNode.Visible;
            itemTemp.Text = "Visible";
            itemTemp.Click += new EventHandler(Node_SetVisibility);
            
            // Merge with context menu
            menuToReturn.MenuItems.Add(itemTemp);

            // MenuItem Rename
            itemTemp = new MenuItem();
            itemTemp.Text = "Rename";
            itemTemp.Click += new EventHandler(Node_Rename);
            
            // Merge with context menu
            menuToReturn.MenuItems.Add(itemTemp);

            // MenuItem Delete
            itemTemp = new MenuItem();
            itemTemp.Text = "Delete";
            itemTemp.Click += new EventHandler(Node_Delete);
            
            // Merge with context menu
            menuToReturn.MenuItems.Add(itemTemp);

            return menuToReturn;
        }
        #endregion

        /// <summary>
        /// Subscribes for model events.
        /// </summary>
        /// <param name="model">The model.</param>
        protected void SubscribeForModelEvents(Model model)
        {
            model.EventSink.NodeCollectionChanged += new CollectionExEventHandler(Model_ChildrenChangeComplete);
            model.EventSink.PropertyChanged += new Syncfusion.Windows.Forms.Diagram.PropertyChangedEventHandler(Document_PropertyChanged);
            model.EventSink.LayersChanged += new CollectionExEventHandler(Layers_ChangeComplete);
        }

        /// <summary>
        /// Unsubscribe for model events.
        /// </summary>
        /// <param name="model">The model.</param>
        protected void UnSubscribeForModelEvents(Model model)
        {
            model.EventSink.NodeCollectionChanged -= new CollectionExEventHandler(Model_ChildrenChangeComplete);
            model.EventSink.PropertyChanged -= new Syncfusion.Windows.Forms.Diagram.PropertyChangedEventHandler(Document_PropertyChanged);
            model.EventSink.LayersChanged -= new CollectionExEventHandler(Layers_ChangeComplete);
        }

        /// <summary>
        /// Adds to models array.
        /// </summary>
        /// <param name="model">The model.</param>
        protected void AddToModelsArray(Model model)
        {
            // Cache array length value
            int nLength = m_models.Length;
            
            // Create new array
            Model[] models = new Model[nLength + 1];
            
            // Copy old values to just created array
            Array.Copy(m_models, models, nLength);
            
            // Add new model
            models[nLength] = model;
            
            // Assign reference to new model's array
            m_models = models;
        }

        #endregion

        #region Class overrides
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            TreeNode nodeUnderMouse = GetNodeAt(new Point(e.X, e.Y));

            if (nodeUnderMouse != null)
                this.SelectedNode = nodeUnderMouse;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (nodeUnderMouse != null)
                {
                    if (this.SelectedNode.Tag is Layer)
                    {
                        Layer layer = this.SelectedNode.Tag as Layer;

                        if (layer != null)
                        {
                            if (this.ContextMenu == null)
                                this.ContextMenu = new ContextMenu();

                            this.ContextMenu.MenuItems.Clear();
                            ContextMenu menuLayers = CreateLayerPopUpMenu(layer);
                            this.ContextMenu.MergeMenu(menuLayers);
                            this.ContextMenu.Show(this, new Point(e.X, e.Y));

                            this.ContextMenu = null;
                        }
                    }
                    else
                        if ((this.SelectedNode.Tag is string) && ((string)this.SelectedNode.Tag == c_strLAYER_COLLECTION))
                        {
                            if (this.ContextMenu == null)
                                this.ContextMenu = new ContextMenu();

                            this.ContextMenu.MenuItems.Clear();
                            ContextMenu menuLayers = CreateLayersPopUpMenu();
                            this.ContextMenu.MergeMenu(menuLayers);
                            this.ContextMenu.Show(this, new Point(e.X, e.Y));

                            this.ContextMenu = null;
                        }
                        else
                            if (this.SelectedNode.Tag is Node)
                            {
                                Node node = this.SelectedNode.Tag as Node;

                                if (node != null)
                                {
                                    if (this.ContextMenu == null)
                                        this.ContextMenu = new ContextMenu();

                                    this.ContextMenu.MenuItems.Clear();
                                    ContextMenu menu = CreateNodePopUpMenu(node);
                                    this.ContextMenu.MergeMenu(menu);
                                    this.ContextMenu.Show(this, new Point(e.X, e.Y));
                                    this.ContextMenu = null;
                                }
                            }
                            else
                                if (this.SelectedNode.Tag is Model)
                                {
                                    if (this.ContextMenu == null)
                                        this.ContextMenu = new ContextMenu();

                                    this.ContextMenu.MenuItems.Clear();
                                    
                                    // MenuItem Rename
                                    MenuItem itemTemp = new MenuItem();
                                    itemTemp.Text = "Rename";
                                    itemTemp.Click += new EventHandler(Node_Rename);
                                    
                                    // Merge with context menu
                                    this.ContextMenu.MenuItems.Add(itemTemp);
                                    this.ContextMenu.Show(this, new Point(e.X, e.Y));
                                    this.ContextMenu = null;
                                }
                }
                else
                {
                    this.ContextMenu = null;
                }
            }
        }

        #endregion

        #region event handlers
        [EventHandlerPriorityAttribute(true)]
        private void Document_PropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            if (evtArgs.PropertyName == DPN.Name)
            {
                Model model;

                if (evtArgs.NodeAffected is Model)
                {
                    model = (Model)evtArgs.NodeAffected;
                    
                    // Find affectedNode corresponding TreeNode
                    TreeNode nodeModel = FindModelCorrespondingTreeNode(model);
                    
                    // update corresponding tree node
                    nodeModel.Text = model.Name;
                }
                else if (evtArgs.NodeAffected is Layer)
                {
                    Layer nodeAffected = (Layer)evtArgs.NodeAffected;
                    model = (Model)nodeAffected.Container;
                    
                    // Find affectedNode corresponding TreeNode
                    TreeNode nodeModel = FindModelCorrespondingTreeNode(model);

                    if (nodeModel != null)
                    {
                        // 1 - update tree node in corresponding Model subTree
                        TreeNode nodeTreeAffected = FindTreeNode(nodeModel, TreeNodeType.Layer, nodeAffected);
                        
                        // Set New Name
                        nodeTreeAffected.Text = nodeAffected.Name;
                    }
                }
                else if (evtArgs.NodeAffected is Node)
                {
                    Node nodeAffected = (Node)evtArgs.NodeAffected;
                    model = nodeAffected.Root;
                    
                    // Find affectedNode corresponding TreeNode
                    TreeNode nodeModel = FindModelCorrespondingTreeNode(model);

                    if (nodeModel != null)
                    {
                        // 1 - update tree node in corresponding Model subTree
                        TreeNode nodeTreeAffected = FindTreeNode(nodeModel, TreeNodeType.Node, nodeAffected);
                        ICompositeNode composite = evtArgs.NodeAffected as ICompositeNode;

                        if (nodeTreeAffected != null)
                        {
                            // Set New Name
                            nodeTreeAffected.Text = (composite != null) ? string.Format("{0} ({1})", nodeAffected.Name, composite.ChildCount) : nodeAffected.Name;
                        }

                        // 2 - update tree node in corresponding Model subTree
                        TreeNode nodeLayers = FindTreeNode(nodeModel, TreeNodeType.LayersCollection, null);

                        if (nodeLayers != null)
                        {
                            foreach (TreeNode layer in nodeLayers.Nodes)
                            {
                                nodeTreeAffected = FindTreeNode(layer, nodeAffected);

                                if (nodeTreeAffected != null)
                                {
                                    // Set New Name
                                    nodeTreeAffected.Text = (composite != null) ? string.Format("{0} ({1})", ((Node)nodeTreeAffected.Tag).Name, composite.ChildCount) : nodeTreeAffected.Name;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void Layers_Add(object sender, EventArgs evtArgs)
        {
            Model activemodel = this.SelectedNode.Parent.Tag as Model;
            if (activemodel != null)
            {
                Layer tmpLayer = new Layer();
                activemodel.Layers.Add(tmpLayer);
                if (!SelectedNode.IsExpanded)
                    SelectedNode.Expand();
            }
        }

        private void Node_SetVisibility(object sender, EventArgs evtArgs)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                if (this.SelectedNode.Tag is Layer)
                {
                    Layer layer = this.SelectedNode.Tag as Layer;
                    layer.Visible = !menuItem.Checked;

                    ResetForeColor(this.SelectedNode, Color.Black);
                }
                else
                    if (this.SelectedNode.Tag is Node)
                    {
                        this.SetNodesVisibility(!menuItem.Checked, this.SelectedNode);
                    }

                if (menuItem.Checked)
                    this.SelectedNode.ForeColor = Color.LightGray;
                else
                    this.SelectedNode.ForeColor = Color.Black;
            }
        }
        private void ResetForeColor(TreeNode selectedNode, Color foreColor)
        {
            foreach (TreeNode node in selectedNode.Nodes)
            {
                node.ForeColor = foreColor;
                ResetForeColor(node, foreColor);
            }
        }
        private void SetNodesVisibility(bool visible, TreeNode curNode)
        {
            Node node = curNode.Tag as Node;
            node.Visible = visible;

            if (node is ICompositeNode)
            {
                for (int i = 0; i < (node as ICompositeNode).ChildCount; i++)
                {
                    Node n = (node as ICompositeNode).GetChild(i);

                    if (n is ICompositeNode)
                        this.SetNodesVisibility(visible, curNode.Nodes[i]);

                    n.Visible = visible;

                    if (visible)
                        curNode.Nodes[i].ForeColor = Color.Black;
                    else
                        curNode.Nodes[i].ForeColor = Color.LightGray;
                }
            }
            else
            {
                ICompositeNode parent = node.Parent;
                if (parent is Node)
                {
                    if ((!(parent as Node).Visible) && visible)
                    {
                        (parent as Node).Visible = visible;
                        curNode.Parent.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void Layer_SetActive(object sender, EventArgs evtArgs)
        {
            Layer layer = this.SelectedNode.Tag as Layer;
            MenuItem menuItem = sender as MenuItem;

            if (layer != null && menuItem != null)
            {
                layer.Enabled = !menuItem.Checked;
            }
        }

        private void Node_Rename(object sender, EventArgs evtArgs)
        {
            ICompositeNode group = this.SelectedNode.Tag as ICompositeNode;
            Node node = this.SelectedNode.Tag as Node;

            if (group != null && node != null)
            {
                this.SelectedNode.Text = node.Name;
            }

            this.LabelEdit = true;
            this.SelectedNode.BeginEdit();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.TreeView.AfterLabelEdit"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.NodeLabelEditEventArgs"/> that contains the event data.</param>
        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                if (e.Label.Length > 0)
                {
                    if (e.Label.IndexOfAny(new char[] { '@', '.', ',', '!' }) == -1)
                    {
                        string strName = e.Label;
                        Node node = e.Node.Tag as Node;
                        Layer layer = e.Node.Tag as Layer;
                        Model model = e.Node.Tag as Model;

                        if (node != null)
                        {
                            node.Name = strName;
                            
                            // get node new name label
                            strName = node.Name;
                        }
                        else
                        {
                            if (layer != null)
                            {
                                layer.Name = strName;
                                
                                // get layer new name label
                                strName = layer.Name;
                            }
                            else if (model != null)
                                model.Name = strName;
                        }

                        e.Node.Text = strName;
                        e.CancelEdit = true;

                        // Stop editing
                        e.Node.EndEdit(false);
                    }
                    else
                    {
                        /* Cancel the label edit action, inform the user, and 
                           place the node in edit mode again. */
                        e.CancelEdit = true;
                        MessageBox.Show(
                            "Invalid tree node label.\n" +
                            "The invalid characters are: '@','.', ',', '!'",
                            "Node Label Edit");
                        e.Node.BeginEdit();
                    }
                }
                else
                {
                    /* Cancel the label edit action, inform the user, and 
                       place the node in edit mode again. */
                    e.CancelEdit = true;
                    MessageBox.Show(
                        "Invalid tree node label.\nThe label cannot be blank",
                        "Node Label Edit");
                    e.Node.BeginEdit();
                }
            }

            ICompositeNode composite = e.Node.Tag as ICompositeNode;

            if (composite != null)
            {
                e.Node.Text = ((Node)composite).Name + " (" + composite.ChildCount.ToString() + ")";
                e.Node.EndEdit(false);
                e.CancelEdit = true;
            }

            this.LabelEdit = false;
            base.OnAfterLabelEdit(e);
        }
        private void Node_Delete(object sender, EventArgs evtArgs)
        {
            if (this.SelectedNode.Tag is Layer)
            {
                Layer layer = this.SelectedNode.Tag as Layer;
                ILayerContainer container = layer.Container;

                // Update Container's Layers
                if (container.Layers.Contains(layer))
                    container.Layers.Remove(layer);

                // Update Container's Layers
                if (container.ActiveLayers.Contains(layer))
                    container.ActiveLayers.Remove(layer);
            }
            else
                if (this.SelectedNode.Tag is Node)
                {
                    Node node = this.SelectedNode.Tag as Node;
                    ICompositeNode parent = node.Parent;
                    Model model = node.Root;

                    // remove from model to optimize rendering
                    if (model != null)
                    {
                        model.BeginUpdate();
                        parent.RemoveChild(node);
                        model.EndUpdate();
                    }
                }
        }
        [EventHandlerPriorityAttribute(true)]
        private void Model_ChildrenChangeComplete(CollectionExEventArgs evtArgs)
        {
            ICompositeNode nodeComposite = evtArgs.Owner as ICompositeNode;
            Model model = null;

            if (nodeComposite != null)
            {
                if (nodeComposite is Model)
                {
                    model = (Model)nodeComposite;
                }
                else
                {
                    model = ((Node)nodeComposite).Root;
                }
            }

            if (model != null)
            {
                TreeNode nodeModel;
                TreeNode nodes;

                switch (evtArgs.ChangeType)
                {
                    case CollectionExChangeType.Insert:
                        
                        // 1 - find corresponding TreeNode
                        nodeModel = FindModelCorrespondingTreeNode(model);
                        
                        // 2 - get Nodes TreeNode
                        nodes = FindTreeNode(nodeModel, TreeNodeType.NodeCollection, null);

                        if (!(nodeComposite is Model))
                        {
                            nodes = FindTreeNode(nodeModel, TreeNodeType.Node, nodeComposite);

                            // nodes.Nodes.Clear();
                            // 3 - add evtArgs.Nodes corresponding TreeNodes
                            ParseChildren(evtArgs.Elements, nodes);
                            nodes.Text = (nodeComposite as INode).Name + " (" + nodeComposite.ChildCount + ")";
                        }
                        else
                        {
                            // 3 - add evtArgs.Nodes corresponding TreeNodes
                            ParseChildren(evtArgs.Elements, nodes);
                            nodes.Text = c_strNODES + " (" + model.Nodes.Count + ")";
                        }
                        
                        // 4 - update activelayers subtrees
                        // get Layer's TreeNode
                        TreeNode nodeLayers = FindTreeNode(nodeModel, TreeNodeType.LayersCollection, null);

                        foreach (TreeNode nodeLayer in nodeLayers.Nodes)
                        {
                            if (model.ActiveLayers.Contains(nodeLayer.Tag as Layer))
                            {
                                if (!(nodeComposite is Model))
                                {
                                    nodes = FindTreeNode(nodeLayer, nodeComposite);

                                    // nodes.Nodes.Clear();
                                    // 3 - add evtArgs.Nodes corresponding TreeNodes
                                    ParseChildren(evtArgs.Elements, nodes);
                                    nodes.Text = ((Node)nodes.Tag).Name + " (" + ((ICompositeNode)nodes.Tag).ChildCount + ")";
                                }
                                else
                                {
                                    ParseChildren(evtArgs.Elements, nodeLayer);
                                }
                            }
                        }
                        break;
                    case CollectionExChangeType.Set:

                        break;
                    case CollectionExChangeType.Remove:
                    case CollectionExChangeType.Clear:
                        
                        // 1 - find corresponding TreeNode
                        nodeModel = FindModelCorrespondingTreeNode(model);
                        
                        // 2 - get Nodes TreeNode
                        if (!(nodeComposite is Model))
                        {
                            nodes = FindTreeNode(nodeModel, TreeNodeType.Node, nodeComposite);
                        }
                        else
                        {
                            nodes = FindTreeNode(nodeModel, TreeNodeType.NodeCollection, null);
                        }

                        TreeNode layers = FindTreeNode(nodeModel, TreeNodeType.LayersCollection, null);
                        TreeNode nodeTreeTemp;
                        Node nodeTemp;
                        
                        // 3 - iterate through nodecollection involved in removing action and
                        // remove it members from Nodes and from Layers subtrees
                        IEnumerator enumerator = evtArgs.Elements.GetEnumerator();

                        while (enumerator.MoveNext())
                        {
                            nodeTemp = enumerator.Current as Node;
                            
                            // 3a - find node to remove in Nodes subtree
                            nodeTreeTemp = FindTreeNode(nodes, nodeTemp);
                            
                            // 3b - remove from Nodes subtree
                            if (nodeTreeTemp != null)
                                nodeTreeTemp.Remove();
                        }
                        
                        // if current node belogns to layer
                        // then update Layers subtree
                        // 4 - update activelayers subtrees
                        // get Layer's TreeNode
                        nodeLayers = FindTreeNode(nodeModel, TreeNodeType.LayersCollection, null);

                        Layer layerTemp;

                        foreach (TreeNode nodeLayer in nodeLayers.Nodes)
                        {
                            // Clear layer nodes
                            nodeLayer.Nodes.Clear();
                            
                            // Refresh nodes
                            layerTemp = nodeLayer.Tag as Layer;
                            ParseChildren(layerTemp.Nodes, nodeLayer);
                        }

                        if (!(nodeComposite is Model))
                        {
                            nodes.Text = (nodeComposite as INode).Name + " (" + nodeComposite.ChildCount + ")";
                        }
                        else
                            nodes.Text = c_strNODES + " (" + model.Nodes.Count + ")";
                        break;
                }
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DocumentExplorer));
            this.smallImageList = new System.Windows.Forms.ImageList(this.components);
            
            // smallImageList
            this.smallImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.smallImageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("smallImageList.ImageStream");
            this.smallImageList.TransparentColor = System.Drawing.Color.Magenta;
        }

        [EventHandlerPriorityAttribute(true)]
        private void Layers_ChangeComplete(CollectionExEventArgs evtArgs)
        {
            // Aquire model containing LayerCollection
            Model model = evtArgs.Owner as Model;

            // if layers doesn't belong to model raise exception
            if (model == null)
                throw new NullReferenceException("model");

            if (evtArgs.Elements != null)
            {
                TreeNode nodeModel;
                TreeNode nodeLayers;

                switch (evtArgs.ChangeType)
                {
                    case CollectionExChangeType.Insert:
                        // 1 - find layer's container corresponding TreeNode
                        nodeModel = FindModelCorrespondingTreeNode(model);
                        
                        // 2 - get Layer's TreeNode
                        nodeLayers = FindTreeNode(nodeModel, TreeNodeType.LayersCollection, null);
                        
                        // 3 - add evtArgs.Layers corresponding TreeNodes
                        ParseLayers(evtArgs.Elements, nodeLayers);
                        break;
                    case CollectionExChangeType.Clear:
                        
                        // 1 - find layer's container corresponding TreeNode
                        nodeModel = FindModelCorrespondingTreeNode(model);
                        
                        // 2 - get Layer's TreeNode
                        nodeLayers = FindTreeNode(nodeModel, TreeNodeType.LayersCollection, null);
                        
                        // 3 - clear layer collection
                        nodeModel.Nodes.Remove(nodeLayers);
                        
                        // 4 - add new TreeNode
                        TreeNode nodeLayersNew = new TreeNode(c_strLAYERS);
                        nodeLayersNew.Tag = c_strLAYER_COLLECTION;
                        nodeModel.Nodes.Add(nodeLayersNew);
                        break;
                    case CollectionExChangeType.Remove:
                        
                        // 1 - find layer's container corresponding TreeNode
                        nodeModel = FindModelCorrespondingTreeNode(model);
                        TreeNode nodeTemp;

                        foreach (Layer layer in evtArgs.Elements)
                        {
                            // 3 - find layer's corresponding TreeNode
                            nodeTemp = FindTreeNode(nodeModel, TreeNodeType.Layer, layer);

                            if (nodeTemp != null)
                                nodeTemp.Remove();
                        }
                        break;
                }
            }
        }
        #endregion
    }
}