#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools.Navigation.Design
{
    /// <summary>
    /// Bar CollectionEditor Form
    /// </summary>
    public partial class BarCollectionEditorForm :
        Form
    {
        #region Fields

       private IServiceProvider _provider;
       private NavigationView _navigationView;
        private bool _isDirty;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="BarCollectionEditorForm"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="navigationView">The navigation view.</param>
        public BarCollectionEditorForm(IServiceProvider provider, NavigationView navigationView) :
            base()
        {
            InitializeComponent();

            _provider = provider;
            _navigationView = navigationView;
            _isDirty = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether <see cref="NavigationView.Bars"/> is dirty.
        /// </summary>
        /// <value><c>true</c> if dirty; otherwise, <c>false</c>.</value>
        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _propertyGrid.Site = _navigationView.Site;

            if (_navigationView.ImageList != null)
            {
                ImageList.ImageCollection images = _imageList.Images;
                ImageList navView = _navigationView.ImageList;
                ImageList.ImageCollection navViewImages = navView.Images;
                Size size = navView.ImageSize;

                _imageList.ColorDepth = navView.ColorDepth;
                _imageList.ImageSize = size;

                foreach (Image img in navViewImages)
                {
                    images.Add(img);
                }

                using (Bitmap bmp = new Bitmap(size.Width, size.Height))
                {

                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.FillRectangle(Brushes.Transparent, 0, 0, size.Width, size.Height);
                    }

                    images.Add(bmp);
                }


                _tvBars.ImageList = _imageList;
                _tvBars.ImageIndex = navViewImages.Count;
            }

            _navigationView.BarPropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnBarPropertyChanged);

            PopulateBarsTree();
        }

        #endregion

        #region Implementation

        private void PopulateBarsTree()
        {
            _tvBars.BeginUpdate();

            PopulateBarTreeNodes(_navigationView.Bars, _tvBars.Nodes);

            Bar selectedBar = _navigationView.SelectedBar;

            if (selectedBar != null)
            {
                TreeNode selectedNode = (TreeNode)selectedBar.Tag;

                _tvBars.SelectedNode = selectedNode;
            }

            _tvBars.EndUpdate();
        }

        private void PopulateBarTreeNodes(BarCollection bars, TreeNodeCollection nodes)
        {
            foreach (Bar bar in bars)
            {
                TreeNode node = CreateNode(bar);

                PopulateBarTreeNodes(bar.Bars, node.Nodes);

                nodes.Add(node);
            }
        }

        private TreeNode CreateNode(Bar bar)
        {
            TreeNode node = new TreeNode(bar.Text);

            node.ImageIndex = bar.ImageIndex;
            node.Tag = bar;
            bar.Tag = node;

            return node;
        }

        private Bar CreateBar()
        {
            // INameCreationService nameService = _provider.GetService( typeof( INameCreationService ) ) as INameCreationService;
            // string text = ( nameService != null ) ? nameService.CreateName( this.components, typeof(Bar) ) : "bar";
            Bar bar = new Bar("bar");

            return bar;
        }

        private void OnAfterTreeNodeSelect(object sender, TreeViewEventArgs e)
        {
            Bar selectedBar = null;

            if (e.Node != null)
            {
                selectedBar = (Bar)e.Node.Tag;
            }

            _propertyGrid.SelectedObject = selectedBar;
            _btnAddChild.Enabled = selectedBar != null;
        }

        private void AddBarNode(BarCollection bars, TreeNodeCollection nodes, bool selectNode)
        {
            Bar bar = CreateBar();
            TreeNode treeNode = CreateNode(bar);

            bars.Add(bar);
            nodes.Add(treeNode);

            if (selectNode)
            {
                _tvBars.SelectedNode = treeNode;
            }

            _isDirty = true;
        }

        private void OnAddChildClick(object sender, EventArgs e)
        {
            TreeNode selectedNode = _tvBars.SelectedNode;

            if (selectedNode != null)
            {
                BarCollection bars = this.SelectedNodeBars;

                AddBarNode(bars, selectedNode.Nodes, false);
                selectedNode.Expand();
            }
            else
            {
                OnAddRootClick(sender, e);
            }
        }

        private void OnAddRootClick(object sender, EventArgs e)
        {
            AddBarNode(_navigationView.Bars, _tvBars.Nodes, true);
        }

        private void OnUpClick(object sender, EventArgs e)
        {
            TreeNode selectedNode = _tvBars.SelectedNode;

            if (selectedNode != null)
            {
                TreeNode newNode = selectedNode.PrevNode;
                TreeNode parentNode = selectedNode.Parent;
                Bar bar = (Bar)selectedNode.Tag;
                BarCollection bars = GetNodeParentBars(selectedNode);

                bars.Remove(bar);
                selectedNode.Remove();

                if (newNode != null)
                {
                    Bar newBar = (Bar)newNode.Tag;

                    newBar.Bars.Add(bar);
                    newNode.Nodes.Add(selectedNode);

                    newNode.Expand();
                }
                else
                {
                    if (parentNode != null)
                    {
                        BarCollection newBars = GetNodeParentBars(parentNode);
                        TreeNodeCollection newNodes = GetNodeParentNodes(parentNode);
                        int i = parentNode.Index - 1;

                        if (i < 0)
                        {
                            i = 0;
                        }

                        newBars.Insert(i, bar);
                        newNodes.Insert(i, selectedNode);
                    }
                    else
                    {
                        _navigationView.Bars.Insert(0, bar);
                        _tvBars.Nodes.Insert(0, selectedNode);
                    }
                }

                _tvBars.SelectedNode = selectedNode;
                _isDirty = true;
            }
        }

        private void OnDownClick(object sender, EventArgs e)
        {
            TreeNode selectedNode = _tvBars.SelectedNode;

            if (selectedNode != null)
            {
                TreeNode newNode = selectedNode.NextNode;
                TreeNode parentNode = selectedNode.Parent;
                Bar bar = (Bar)selectedNode.Tag;
                BarCollection bars = GetNodeParentBars(selectedNode);

                bars.Remove(bar);
                selectedNode.Remove();

                if (newNode != null)
                {
                    Bar newBar = (Bar)newNode.Tag;

                    newBar.Bars.Insert(0, bar);
                    newNode.Nodes.Insert(0, selectedNode);

                    newNode.Expand();
                }
                else
                {
                    if (parentNode != null)
                    {
                        BarCollection newBars = GetNodeParentBars(parentNode);
                        TreeNodeCollection newNodes = GetNodeParentNodes(parentNode);
                        int i = parentNode.Index + 1;

                        if (i < newNodes.Count)
                        {
                            newBars.Insert(i, bar);
                            newNodes.Insert(i, selectedNode);
                        }
                        else
                        {
                            newBars.Add(bar);
                            newNodes.Add(selectedNode);
                        }
                    }
                    else
                    {
                        _navigationView.Bars.Add(bar);
                        _tvBars.Nodes.Add(selectedNode);
                    }
                }

                _tvBars.SelectedNode = selectedNode;
                _isDirty = true;
            }
        }

        private void OnRemoveClick(object sender, EventArgs e)
        {
            TreeNode selectedNode = _tvBars.SelectedNode;

            if (selectedNode != null)
            {
                TreeNode parentNode = selectedNode.Parent;
                TreeNodeCollection nodes = (parentNode != null) ? parentNode.Nodes : _tvBars.Nodes;
                TreeNode nextSelectedNode = selectedNode.NextVisibleNode;

                if (nextSelectedNode == null)
                {
                    nextSelectedNode = selectedNode.PrevVisibleNode;

                    if (nextSelectedNode == null)
                    {
                        nextSelectedNode = parentNode;
                    }
                }

                BarCollection bars = this.SelectedNodeParentBars;
                Bar bar = (Bar)selectedNode.Tag;

                selectedNode.Tag = null;

                Bar nextSelectedBar = null;

                if (nextSelectedNode != null)
                {
                    _tvBars.SelectedNode = nextSelectedNode;
                    nextSelectedBar = (Bar)nextSelectedNode.Tag;
                }

                if (bar.IsSelected(_navigationView))
                {
                    _navigationView.SelectedBar = nextSelectedBar;
                }

                nodes.Remove(selectedNode);
                bars.Remove(bar);

                _isDirty = true;
            }
        }

        private BarCollection SelectedNodeParentBars
        {
            get
            {
                TreeNode node = _tvBars.SelectedNode;

                return GetNodeParentBars(node);
            }
        }

        private BarCollection SelectedNodeBars
        {
            get
            {
                TreeNode node = _tvBars.SelectedNode;
                Bar bar = (Bar)node.Tag;

                return bar.Bars;
            }
        }

        private BarCollection GetNodeParentBars(TreeNode node)
        {
            BarCollection bars = _navigationView.Bars;
            TreeNode parent = node.Parent;

            if (parent != null)
            {
                Bar bar = (Bar)parent.Tag;

                bars = bar.Bars;
            }

            return bars;
        }

        private TreeNodeCollection GetNodeParentNodes(TreeNode node)
        {
            TreeNodeCollection nodes = _tvBars.Nodes;
            TreeNode parent = node.Parent;

            if (parent != null)
            {
                nodes = parent.Nodes;
            }

            return nodes;
        }

        private void OnBarPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Bar.Properties prop = Bar.Helper.ParsePropertyEnumString(e.PropertyName);
            Bar bar = (Bar)sender;
            TreeNode node = (TreeNode)bar.Tag;

            switch (prop)
            {
                case Bar.Properties.Text:
                    {
                        node.Text = bar.Text;
                        break;
                    }
                case Bar.Properties.ImageIndex:
                    {
                        int idx = bar.ImageIndex;

                        SetNodeImageIndex(idx, node);
                        break;
                    }
                case Bar.Properties.Image:
                case Bar.Properties.DisabledImage:
                    {
                        SetNodeImageIndex(-1, node);
                        break;
                    }
            }

            _tvBars.Invalidate(node.Bounds);
            _isDirty = true;
        }

        private void SetNodeImageIndex(int idx, TreeNode node)
        {
            if (idx < 0)
            {
                ImageList imgList = _navigationView.ImageList;

                if (imgList != null)
                {
                    idx = imgList.Images.Count;
                }
            }

            node.ImageIndex = idx;
            node.SelectedImageIndex = idx;
        }

        private void OnBeforeTreeNodeSelect(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;

            if (node != null)
            {
                Bar bar = (Bar)node.Tag;
                int imgIdx = bar.ImageIndex;

                if (imgIdx < 0)
                {
                    imgIdx = _imageList.Images.Count - 1;
                }

                node.SelectedImageIndex = imgIdx;
            }
        }

        private void OnSelectClick(object sender, EventArgs e)
        {
            TreeNode selectedNode = _tvBars.SelectedNode;

            if (selectedNode != null)
            {
                Bar bar = (Bar)selectedNode.Tag;

                _navigationView.SelectedBar = bar;
                _isDirty = true;
            }
        }

        private void OnNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            OnSelectClick(sender, e);
        }

        #endregion
    }
}
