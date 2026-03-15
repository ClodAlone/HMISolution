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
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram.Controls
{
    /// <summary>
    /// This control displays and edits properties of objects in a m_diagram.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This control contains an embedded System.Windows.Forms.PropertyGrid
    /// that is used to edit objects in a m_diagram. The
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.PropertyEditor.m_diagram"/>
    /// contains a reference to a
    /// <see cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram"/>
    /// object that the property editor is attached to. Once it is attached to
    /// a m_diagram, the property editor automatically displays the currently
    /// selected object in the m_diagram.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controls.Diagram"/>
    /// </remarks>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(PropertyEditor), "ToolboxIcons.PropertyEditor.bmp")]
    [Description("Property Editor control for nodes in a Diagram.")]
    public class PropertyEditor
        : Panel
    {
        #region Class members
        private ComboBox m_comboBox;
        private PropertyGrid m_propGrid;

        /// <summary>
        /// m_diagram to show properties from.
        /// </summary>
        private Diagram m_diagram;

        /// <summary>
        /// Indicates whether porperty grid will be refreshed.
        /// </summary>
        private bool m_bCanRefreshPropertyGrid;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyEditor"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public PropertyEditor(IContainer container)
            : this()
        {
            container.Add(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyEditor"/> class.
        /// </summary>
        public PropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // Create embedded property grid and configure it.
            m_propGrid = new PropertyGrid();
            m_propGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(PropertyGrid_PropertyValueChanged);
            this.Controls.Add(m_propGrid);

            InitializeCombo();

            m_propGrid.Dock = DockStyle.Fill;
            m_comboBox.Dock = DockStyle.Top;
            m_comboBox.Visible = false;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_propGrid.PropertyValueChanged -= new PropertyValueChangedEventHandler(PropertyGrid_PropertyValueChanged);
            }
            base.Dispose(disposing);
        }        

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PropertyEditor));
        }
        #endregion

        #endregion

        #region Class public interface

        #region methods
        /// <summary>
        /// Selects the list of nodes in the property grid.
        /// </summary>
        /// <param name="nodes">Nodes to display in the property editor.</param>
        public void SetSelectedObjects(NodeCollection nodes)
        {
            if (nodes.Count > 0)
                m_propGrid.SelectedObject = nodes[0];
            else
                m_propGrid.SelectedObject = null;
        }

        /// <summary>
        /// Select the node in the property grid.
        /// </summary>
        /// <param name="node">Node to display in the property editor.</param>
        public void SetSelectedObject(Node node)
        {
            m_propGrid.SelectedObject = node;
        }

        #endregion

        #region properties
        /// <summary>
        /// Gets reference to the PropertyGrid object contained by this property editor.
        /// </summary>
        public PropertyGrid PropertyGrid
        {
            get { return this.m_propGrid; }
        }

        /// <summary>
        /// Gets or sets the Diagram which should be attached to the property editor.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property contains a reference to the m_diagram that this property
        /// editor is attached to. The property editor receives events from the
        /// m_diagram when the current selection changes and it updates the currently
        /// displayed object in the property editor.
        /// </para>
        /// </remarks>
        [Browsable(true)]
        [Description("Diagram the property editor is attached to.")]
        public Diagram Diagram
        {
            get 
            { 
                return m_diagram; 
            }
            set
            {
                if (m_diagram != value)
                {
                    if (m_diagram != null)
                        UnsubscribeFromDiagramEvents();

                    m_diagram = value;

                    if (m_diagram != null)
                    {
                        SubscribeForDiagramEvents();

                        if (!this.DesignMode)
                            m_propGrid.SelectedObject = m_diagram.Model;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether combobox is shown.
        /// </summary>
        /// <value><c>true</c> if show combo; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        [Description("Determines if ComboBox is visible")]
        public bool ShowCombo
        {
            get
            {
                return this.m_comboBox.Visible;
            }
            set
            {
                this.m_comboBox.Visible = value;
            }
        }
        #endregion

        #endregion

        #region Event handlers
        [EventHandlerPriorityAttribute(true)]
        private void Diagram_SelectionChanged(CollectionExEventArgs evtArgs)
        {
            if (!this.DesignMode)
            {
                if (m_diagram != null && m_diagram.Controller != null && m_diagram.Controller.SelectionList != null)
                {
                    int selectionCount = m_diagram.Controller.SelectionList.Count;

                    if (selectionCount == 0)
                    {
                        // update selection combobox
                        UpdateComboBox(m_diagram.Model);
                        m_propGrid.SelectedObject = m_diagram.Model;
                    }
                    else
                    {
                        if (selectionCount > 0)
                        {
                            object[] ar = new object[selectionCount];
                            m_diagram.Controller.SelectionList.CopyTo(ar, 0);
                            m_propGrid.SelectedObjects = ar;

                            // update selection combobox
                            if (ar.Length == 1)
                                UpdateComboBox(ar[0] as Node);
                        }
                    }
                }
            }
        }
        [EventHandlerPriorityAttribute(true)]
        private void Diagram_PropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            if (evtArgs.PropertyName == DPN.Model)
                m_propGrid.SelectedObject = m_diagram.Model;
        }
        [EventHandlerPriorityAttribute(true)]
        private void Model_PropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            if (m_bCanRefreshPropertyGrid)
                RefreshPropertyGrid(evtArgs.NodeAffected);
        }
        [EventHandlerPriorityAttribute(true)]
        private void HistoryManager_RecordRequest(object sender, EventArgs e)
        {
            m_bCanRefreshPropertyGrid = false;
        }
        [EventHandlerPriorityAttribute(true)]
        private void HistoryManager_RecordComplete(object sender, EventArgs e)
        {
            m_bCanRefreshPropertyGrid = true;

            // force property grid refresh
            m_propGrid.Refresh();
        }

        [EventHandlerPriorityAttribute(true)]
        private void EventSink_NodeCollectionChanged(CollectionExEventArgs evtArgs)
        {
            if (!this.DesignMode)
            {
                 if (evtArgs.ChangeType == CollectionExChangeType.Remove)
                 {
                     m_comboBox.Items.Clear();
                     UpdateComboBox(Diagram.Model);
                     m_propGrid.SelectedObject = m_diagram.Model;
                 }
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the m_comboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            NodeComboItem objToSelect = ((ComboBox)sender).SelectedItem as NodeComboItem;
            Node nodeToSelect = (objToSelect != null) ? objToSelect.Tag as Node : null;

            if (m_diagram != null && m_diagram.Controller != null)
            {
                NodeCollection selectionList = m_diagram.Controller.SelectionList;

                if (nodeToSelect != null)
                {
                    if (!(selectionList.Count == 1 && selectionList.Contains(nodeToSelect)))
                    {
                        if (selectionList.Count > 0)
                            selectionList.Clear();

                        m_comboBox.SelectedItem = null;
                        selectionList.Add(nodeToSelect);
                        m_diagram.Focus();
                    }
                }
                else if (m_propGrid.SelectedObject != objToSelect)
                {
                    if (selectionList.Count > 0)
                        selectionList.Clear();
                }
            }
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Initializes the document explorer combo.
        /// </summary>
        private void InitializeCombo()
        {
            m_comboBox = new ComboBox();
            this.Controls.Add(m_comboBox);

            m_comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            m_comboBox.FlatStyle = FlatStyle.System;
            m_comboBox.DisplayMember = "DisplayName";
            m_comboBox.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
        }

        /// <summary>
        /// Performs property grid refresh.
        /// </summary>
        /// <param name="objCausedInvalidation">The object.</param>
        private void RefreshPropertyGrid(object objCausedInvalidation)
        {
            // property grid currently selected object
            object objSelObject = m_propGrid.SelectedObject;

            if (!this.DesignMode && objSelObject != null && objSelObject.Equals(objCausedInvalidation))
            {
                m_propGrid.Refresh();
            }
        }
        private void UnsubscribeFromDiagramEvents()
        {
            m_diagram.EventSink.NodeCollectionChanged -= new CollectionExEventHandler(Diagram_SelectionChanged);
            m_diagram.EventSink.PropertyChanged -= new PropertyChangedEventHandler(Diagram_PropertyChanged);

            if (m_diagram.Model != null)
            {
                m_diagram.Model.EventSink.PropertyChanged -= new PropertyChangedEventHandler(Model_PropertyChanged);
                m_diagram.Model.HistoryManager.RecordRequest -= new EventHandler(HistoryManager_RecordRequest);
                m_diagram.Model.HistoryManager.RecordComplete -= new EventHandler(HistoryManager_RecordComplete);

                m_comboBox.Items.Clear();
            }
        }
        private void SubscribeForDiagramEvents()
        {
            this.m_diagram.EventSink.SelectionListChanged += new CollectionExEventHandler(Diagram_SelectionChanged);            
            m_diagram.EventSink.PropertyChanged += new PropertyChangedEventHandler(Diagram_PropertyChanged);

            if (m_diagram.Model != null)
            {
                this.m_diagram.Model.EventSink.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);
                this.m_diagram.Model.EventSink.NodeCollectionChanged += new CollectionExEventHandler(EventSink_NodeCollectionChanged);
                this.m_diagram.Model.HistoryManager.RecordRequest += new EventHandler(HistoryManager_RecordRequest);
                this.m_diagram.Model.HistoryManager.RecordComplete += new EventHandler(HistoryManager_RecordComplete);

                m_comboBox.Items.Clear();

                if (m_diagram.Model != null)
                {
                    UpdateComboBox(m_diagram.Model);
                }
            }
        }
        private ArrayList GetNodesRecursive(ICompositeNode composite)
        {
            ArrayList nodes = new ArrayList();

            if (composite != null)
            {
                for (int i = 0, length = composite.ChildCount; i < length; i++)
                {
                    Node node = composite.GetChild(i);

                    nodes.Add(node);
                    nodes.AddRange(GetNodesRecursive(node as ICompositeNode));
                }
            }

            return nodes;
        }

        private void UpdateComboBox(INode selectedNode)
        {
            Model model = this.Diagram.Model;
            ICompositeNode group = selectedNode as Group;
            INode selectedComboItem = GetComboSelectedItem();

            if (selectedNode == null || selectedComboItem == selectedNode && !(selectedNode is Model))
                return;

            m_comboBox.Items.Clear();

            // add model as default top item
            AddComboItem(model);

            // if selected model -> add model top nodes
            if (selectedNode is Model || (group == null && selectedNode.Parent is Model))
            {
                NodeCollection modelNodes = model.Nodes;

                // add top nodes to combo box
                foreach (Node topNode in modelNodes)
                {
                    AddComboItem(topNode);
                }

                if (selectedNode != null)
                    SelectComboItem(selectedNode);
                else
                    SelectComboItem(model);
            }
            else
            {
                NodeCollection hierarchyNodes = new NodeCollection();
                Node parent = selectedNode.Parent as Node;

                // get hierarchy parent nodes
                while (parent != null)
                {
                    if (hierarchyNodes.Count > 0)
                        hierarchyNodes.Insert(0, parent);
                    else
                        hierarchyNodes.Add(parent);

                    parent = parent.Parent as Node;
                }

                // add hierarchy nodes to combo box
                foreach (Node node in hierarchyNodes)
                    AddComboItem(node);

                // add children
                if (group != null)
                {
                    AddComboItem(selectedNode);

                    for (int i = 0, length = group.ChildCount; i < length; i++)
                    {
                        AddComboItem(group.GetChild(i));
                    }
                }
                else if (selectedNode.Parent != null && !(selectedNode.Parent is Model))
                {
                    ICompositeNode parentNode = selectedNode.Parent;

                    for (int i = 0, length = parentNode.ChildCount; i < length; i++)
                    {
                        AddComboItem(parentNode.GetChild(i));
                    }
                }

                SelectComboItem(selectedNode);
            }
        }
        private void AddComboItem(INode node)
        {
            if (!m_comboBox.Items.Contains(node))
                m_comboBox.Items.Add(new NodeComboItem(node));
        }
        private void RemoveComboItem(INode node)
        {
            if (!m_comboBox.Items.Contains(node))
                m_comboBox.Items.Remove(node);
        }
        private void SelectComboItem(INode selectedNode)
        {
            if (m_comboBox.SelectedItem != selectedNode)
                m_comboBox.SelectedIndex = m_comboBox.Items.IndexOf(selectedNode);
        }
        private INode GetComboSelectedItem()
        {
            NodeComboItem comboItem = m_comboBox.SelectedItem as NodeComboItem;
            return (comboItem != null) ? comboItem.Tag : null;
        }

        protected virtual void PropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if ((m_propGrid.SelectedObject is ControlNode) && e.ChangedItem.Parent != null)
            {
                ControlNode ctrlNode = m_propGrid.SelectedObject as ControlNode;
                this.Diagram.UpdateView(ctrlNode);
            }
        }
        #endregion

        /// <summary>
        /// Helper class to display the node in the combo box.
        /// </summary>
        private class NodeComboItem
        {
            #region Class members
            /// <summary>
            /// Use double spaces as child separator.
            /// </summary>
            private const string c_DEF_CHILD_SEPARATOR = "  ";
            private readonly string m_strDisplayName;
            public readonly INode Tag;
            #endregion

            #region Class properties
            public string DisplayName
            {
                get { return m_strDisplayName; }
            }
            #endregion

            #region Class initialize
            public NodeComboItem(INode node)
            {
                this.Tag = node;
                m_strDisplayName = GetParentsSeparator(node) + node.Name;
            }
            private string GetParentsSeparator(INode node)
            {
                string parentSeparator = string.Empty;
                Node parent = node as Node;

                while (parent != null)
                {
                    parentSeparator += c_DEF_CHILD_SEPARATOR;
                    parent = parent.Parent as Node;
                }

                return parentSeparator;
            }
            public override bool Equals(object obj)
            {
                bool bEqual = true;
                INode node = obj as INode;

                if (node != null)
                    bEqual = node == this.Tag;
                else
                    bEqual = base.Equals(obj);

                return bEqual;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            #endregion
        }
    }
}
