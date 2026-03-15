#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    public class CustomControlEditor : UITypeEditor
    {
        #region Class members

        private IWindowsFormsEditorService m_editorService = null;

        private ListBox m_listBox = null;
        #endregion

        #region Class Initialize/Finalize methods

        public CustomControlEditor()
        {
            m_listBox = new ListBox();
            m_listBox.BorderStyle = BorderStyle.None;

            m_listBox.Click += new EventHandler(ListBox_Click);
        }
        #endregion

        #region Class overrides

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                m_editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (m_editorService != null)
                {
                    m_listBox.Items.Clear();

                    IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));

                    if (designerHost != null)
                    {
                        ArrayList controlsList = new ArrayList();

                        m_listBox.Items.Add("(none)");
                        m_listBox.SelectedIndex = 0;
                        controlsList.Add(null);

                        TreeNodeAdv node = (TreeNodeAdv)context.Instance;

                        foreach (Component component in designerHost.Container.Components)
                        {
                            Control control = component as Control;

                            if (control != null)
                            {
                                Type controlType = control.GetType();

                                if (controlType != typeof(Form) && controlType != typeof(MultiColumnTreeView))
                                {
                                    controlsList.Add(control);
                                    m_listBox.Items.Add(control.Name);

                                    if (node.CustomControl == control)
                                    {
                                        m_listBox.SelectedIndex = m_listBox.Items.Count - 1;
                                    }
                                }
                            }
                        }

                        m_editorService.DropDownControl(m_listBox);

                        value = controlsList[m_listBox.SelectedIndex];
                    }
                }
            }

            return value;
        }
        #endregion

        #region Class event handlers
        /// <summary>Raises when click the List box</summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        private void ListBox_Click(object sender, EventArgs e)
        {
            if (m_editorService != null)
            {
                m_editorService.CloseDropDown();
            }
        }
        #endregion
    }

    /// <summary>
    /// Base class that provide functionality of DropDown list for PropertyGrid.
    /// </summary>
    public abstract class DropDownUITypeEditor : UITypeEditor
    {
        #region Class members

        private IWindowsFormsEditorService m_editorService = null;
  
        private ListBox m_listBox = null;

        private ITypeDescriptorContext m_context;
        #endregion

        #region Class properties
        /// <summary>Gets drop down control</summary>
        protected ListBox ListBox
        {
            get
            {
                return m_listBox;
            }
        }

        /// <summary>Gets current context of editor.</summary>
        protected ITypeDescriptorContext Context
        {
            get
            {
                return m_context;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        public DropDownUITypeEditor()
        {
            m_listBox = new ListBox();
            m_listBox.BorderStyle = BorderStyle.None;

            m_listBox.Click += new EventHandler(ListBox_Click);
        }
        #endregion

        #region Class helper methods

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null)
            {
                m_context = context;
            }

            if (provider != null)
            {
                m_editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (m_editorService != null)
                {
                    m_listBox.Items.Clear();

                    IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));

                    if (designerHost != null)
                    {
                        m_listBox.Items.AddRange(GetListItemsCollection());
                        m_listBox.SelectedIndex = GetSelectionIndex(value);

                        m_editorService.DropDownControl(m_listBox);

                        value = GetSelectionValue(m_listBox.SelectedIndex);
                    }
                }
            }

            return value;
        }

        /// <summary>ListBox Click </summary>
        /// <param name="sender">ender Object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        private void ListBox_Click(object sender, EventArgs e)
        {
            if (m_editorService != null)
            {
                m_editorService.CloseDropDown();
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Method return by order index real value that have to be stored as property value.
        /// </summary>
        /// <param name="index">selected item from list</param>
        /// <returns>corresponding value that index.</returns>
        protected abstract object GetSelectionValue(int index);

        /// <summary>
        /// On startup detect what item is selected and return it order index.
        /// </summary>
        /// <returns>Selected item from DropDown list.</returns>
        /// <param name="value">Selection Index value</param>
        protected abstract int GetSelectionIndex(object value);

        /// <summary>
        /// Method return array of items that will be added as items into ListBox. User have to 
        /// select value from this array for property.
        /// </summary>
        /// <returns>Array of values.</returns>
        protected abstract object[] GetListItemsCollection();
        #endregion
    }

    public class BaseStyleSelectorUITypeEditor : DropDownUITypeEditor
    {
        #region Class constants

        private const string NoneStyleName = "(none)";
        #endregion

        #region Class properties

        protected TreeColumnAdv TreeColumn
        {
            get
            {
                return this.Context.Instance as TreeColumnAdv;
            }
        }

        protected TreeNodeAdvSubItem TreeNodeSubItem
        {
            get
            {
                return this.Context.Instance as TreeNodeAdvSubItem;
            }
        }

        protected TreeNodeAdv TreeNode
        {
            get
            {
                return this.Context.Instance as TreeNodeAdv;
            }
        }

        protected MultiColumnTreeView TreeView
        {
            get
            {
                if (this.TreeColumn != null)
                {
                    return this.TreeColumn.TreeView;
                }
                else if (this.TreeNodeSubItem != null)
                {
                    return this.TreeNodeSubItem.TreeView;
                }
                else if (this.TreeNode != null)
                {
                    return this.TreeNode.TreeView;
                }

                return this.Context.Instance as MultiColumnTreeView;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Method return by order index real value that have to be stored as property value.
        /// </summary>
        /// <param name="index">selected item from list</param>
        /// <returns>corresponding value that index.</returns>
        protected override object GetSelectionValue(int index)
        {
            // if none style selected
            if (index == 0)
            {
                return string.Empty;
            }

            ArrayList data = new ArrayList(GetListItemsCollection());
            return data[index];
        }

        /// <summary>
        /// On startup detect what item is selected and return it order index.
        /// </summary>
        /// <returns>Selected item from DropDown list.</returns>
        /// <param name="value">Selection Index value</param>
        protected override int GetSelectionIndex(object value)
        {
            // if we have not set base style
            if (value == null || ((string)value).Length == 0)
            {
                return 0;
            }

            ArrayList data = new ArrayList(GetListItemsCollection());
            return data.IndexOf(value);
        }

        /// <summary>
        /// Method return array of items that will be added as items into ListBox. User have to 
        /// select value from this array for property.
        /// </summary>
        /// <returns>Array of values.</returns>
        protected override object[] GetListItemsCollection()
        {
            ArrayList styles = new ArrayList();
            styles.Add(NoneStyleName);

            if (this.TreeView != null)
            {
                foreach (string name in this.TreeView.BaseStyles.Keys)
                {
                    styles.Add(name);
                }
            }

            return styles.ToArray();
        }
        #endregion
    }

    public class TreeViewAdvImageListIndexUITypeEditor : DropDownUITypeEditor
    {
        #region Class constants

        protected enum ImageListType
        {
            /// <summary>Represents State </summary>
            State,

            /// <summary>Represents Right</summary>
            Right,

            /// <summary>Represents Left</summary>
            Left,

            /// <summary>Represents Node state</summary>
            NodeState,
        }
        #endregion

        #region Class members
  
        private ImageListType m_type;
        #endregion

        #region Class properties

        protected TreeColumnAdv TreeColumn
        {
            get
            {
                return this.Context.Instance as TreeColumnAdv;
            }
        }
        protected TreeNodeAdvSubItem TreeNodeSubItem
        {
            get
            {
                return this.Context.Instance as TreeNodeAdvSubItem;
            }
        }
        protected TreeNodeAdv TreeNode
        {
            get
            {
                return this.Context.Instance as TreeNodeAdv;
            }
        }

        protected MultiColumnTreeView TreeView
        {
            get
            {
                if (this.TreeColumn != null)
                {
                    return this.TreeColumn.TreeView;
                }
                else if (this.TreeNodeSubItem != null)
                {
                    return this.TreeNodeSubItem.TreeView;
                }
                else if (this.TreeNode != null)
                {
                    return this.TreeNode.TreeView;
                }

                return this.Context.Instance as MultiColumnTreeView;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the TreeViewAdvImageListIndexUITypeEditor class.
        /// </summary>
        /// <param name="type">Type of list to use.</param>
        protected TreeViewAdvImageListIndexUITypeEditor(ImageListType type)
        {
            m_type = type;

            this.ListBox.DrawItem += new DrawItemEventHandler(ListBox_DrawItem);
            this.ListBox.ItemHeight = 22;
            this.ListBox.DrawMode = DrawMode.OwnerDrawFixed;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Method return by order index real value that have to be stored as property value.
        /// </summary>
        /// <param name="index">selected item from list</param>
        /// <returns>corresponding value that index.</returns>
        protected override object GetSelectionValue(int index)
        {
            return (int)this.ListBox.Items[index];
        }

        /// <summary>
        /// On startup detect what item is selected and return it order index.
        /// </summary>
        /// <returns>Selected item from DropDown list.</returns>
        /// <param name="value">Selection Index value</param>
        protected override int GetSelectionIndex(object value)
        {
            ImageList list = GetChosenImageList();

            if (value != null && list != null)
            {
                int valueInt = (int)value;

                // convert value to order index
                valueInt = (valueInt < 0) ? 0 : valueInt + 1;
                return Math.Max(-1, Math.Min(valueInt, list.Images.Count));
            }

            return -1;
        }

        /// <summary>
        /// Method return array of items that will be added as items into ListBox. User have to 
        /// select value from this array for property.
        /// </summary>
        /// <returns>Array of values.</returns>
        protected override object[] GetListItemsCollection()
        {
            ImageList values = GetChosenImageList();
            ArrayList data = new ArrayList();
            data.Add(-1);

            if (values != null)
            {
                for (int i = 0; i < values.Images.Count; i++)
                {
                    data.Add(i);
                }
            }

            return data.ToArray();
        }
        #endregion

        #region Class utility methods

        private ImageList GetChosenImageList()
        {
            ImageList values = null;

            if (this.TreeView != null)
            {
                switch (m_type)
                {
                    case ImageListType.State:
                        values = this.TreeView.StateImageList;
                        break;
                    case ImageListType.Right:
                        values = this.TreeView.RightImageList;
                        break;
                    case ImageListType.Left:
                        values = this.TreeView.LeftImageList;
                        break;
                    case ImageListType.NodeState:
                        values = this.TreeView.NodeStateImageList;
                        break;
                }
            }

            return values;
        }

        /// <summary> ListBox DrawItem </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        private void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ImageList values = GetChosenImageList();
            int index = (int)this.ListBox.Items[e.Index];

            Graphics g = e.Graphics;
            Rectangle rc = e.Bounds;

            e.DrawBackground();
            if (e.State == DrawItemState.Selected)
            {
                e.DrawFocusRectangle();
            }

            // draw image
            if (index >= 0 && index < values.Images.Count)
            {
                g.DrawImage(values.Images[index], rc.Left + 3, rc.Top + 3, 16, 16);
            }

            g.DrawRectangle(Pens.Black, rc.Left + 2, rc.Top + 2, 16, 16);
            g.DrawString(index.ToString(), e.Font, SystemBrushes.WindowText, rc.Left + 22, rc.Top + 3);
        }
        #endregion
    }

    public class StateImageListUITypeEditor : TreeViewAdvImageListIndexUITypeEditor
    {
        public StateImageListUITypeEditor()
            : base(ImageListType.State)
        {
        }
    }

    public class NodeStateImageListUITypeEditor : TreeViewAdvImageListIndexUITypeEditor
    {
        public NodeStateImageListUITypeEditor()
            : base(ImageListType.NodeState)
        {
        }
    }

    public class RightImageListUITypeEditor : TreeViewAdvImageListIndexUITypeEditor
    {     
        public RightImageListUITypeEditor()
            : base(ImageListType.Right)
        {
        }
    }

    public class LeftImageListUITypeEditor : TreeViewAdvImageListIndexUITypeEditor
    {    
        public LeftImageListUITypeEditor()
            : base(ImageListType.Left)
        {
        }
    }
}