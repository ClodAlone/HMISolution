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
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Documentation;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    [DocumentationExclude()]
    public class TreeViewAdvBaseStylesEditorForm : Form
    {
        #region Class constants

        private static string resetHintString = "Right Click to Reset";

        private static string confirmStyleChoice = "On Add Style will be created style: {0}";

        private enum StyleIndex
        {
            /// <summary>
            /// Represents Base
            /// </summary>
            Base,

            /// <summary>
            /// Represents Nodelevel
            /// </summary>
            NodeLevel,

            /// <summary>
            /// Represents Column
            /// </summary>
            Column,

            /// <summary>
            /// Represents Subitem
            /// </summary>
            SubItem
        }
        #endregion

        #region Class members

        private string m_propBaseString;

        private Hashtable m_baseStyles;
  
        private int m_tooltipIndex;

        private IServiceProvider m_provider = null;
 
        private MultiColumnTreeView m_treeView;

        private StyleIndex m_styleType = StyleIndex.Base;
        #endregion

        #region Class properties
 
        private int ToolTipIndex
        {
            get
            {
                return this.m_tooltipIndex;
            }
            set
            {
                if (this.m_tooltipIndex != value)
                {
                    this.m_tooltipIndex = value;
                    string tooltip = String.Empty;

                    if (m_tooltipIndex != -1)
                    {
                        tooltip = m_treeView.GetHintTextForStyle(this.ListBox.Items[m_tooltipIndex].ToString());
                    }

                    this.toolTip1.SetToolTip(this.ListBox, tooltip);
                }
            }
        }
 
        private ListBox ListBox
        {
            get
            {
                return this.styleNamesList.ListBox;
            }
        }

        protected object SelectedItem
        {
            get
            {
                object selected = null;
                int nSelectedIdx = this.ListBox.SelectedIndex;
                ListBox.ObjectCollection items = this.ListBox.Items;

                if (nSelectedIdx >= 0 && nSelectedIdx < items.Count)
                {
                    selected = items[nSelectedIdx];
                }

                return selected;
            }
        }

        #endregion

        #region Form controls

        private Label propLabel;

        private Label memLabel;

        private ButtonAdv addBtn;

        private ButtonAdv removeBtn;

        private EditableList styleNamesList;

        private ToolTip toolTip1;

        private ButtonAdv closeBtn;

        private IContainer components;

        private ButtonAdv addNodeLevelBtn;

        private TreeViewAdvEditorPropertyGrid properties;

        private Panel pnlList;
  
        private Panel pnlProperties;

        private Panel pnlBody;

        private Splitter splitBody;

        private GradientPanel pnlGradient;

        private ContextMenu popupStyles;

        private MenuItem mnuBaseStyle;

        private MenuItem mnuNodeLevelStyle;

        private MenuItem mnuColumnStyle;

        private MenuItem mnuSubItemStyle;

        private Label lblInfo;

        private PropertyGridContextMenu pgMenu;
        #endregion

        #region Class Initialize/Finalize methods

        protected TreeViewAdvBaseStylesEditorForm()
        {
            // Required for Windows Form Designer support
            InitializeComponent();
        }

        public TreeViewAdvBaseStylesEditorForm(MultiColumnTreeView treeView)
            : this(treeView, null)
        {
        }

        public TreeViewAdvBaseStylesEditorForm(MultiColumnTreeView treeView, IServiceProvider provider)
            : this()
        {
            // update provider references
            m_provider = provider;
            properties.Provider = m_provider;

            m_treeView = treeView;
            m_baseStyles = treeView.BaseStyles;
            this.pgMenu = new PropertyGridContextMenu(this.properties);
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
       /// <param name="disposing">Bool disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.pgMenu != null)
                {
                    this.pgMenu.Dispose();
                    this.pgMenu = null;
                }

                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.properties = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeViewAdvEditorPropertyGrid();
            this.propLabel = new System.Windows.Forms.Label();
            this.memLabel = new System.Windows.Forms.Label();
            this.styleNamesList = new Syncfusion.Windows.Forms.Tools.EditableList();
            this.addBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.removeBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.closeBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.addNodeLevelBtn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.pnlList = new System.Windows.Forms.Panel();
            this.pnlProperties = new System.Windows.Forms.Panel();
            this.pnlBody = new System.Windows.Forms.Panel();
            this.splitBody = new System.Windows.Forms.Splitter();
            this.pnlGradient = new Syncfusion.Windows.Forms.Tools.GradientPanel();
            this.lblInfo = new System.Windows.Forms.Label();
            this.popupStyles = new System.Windows.Forms.ContextMenu();
            this.mnuBaseStyle = new System.Windows.Forms.MenuItem();
            this.mnuNodeLevelStyle = new System.Windows.Forms.MenuItem();
            this.mnuColumnStyle = new System.Windows.Forms.MenuItem();
            this.mnuSubItemStyle = new System.Windows.Forms.MenuItem();
            this.pnlList.SuspendLayout();
            this.pnlProperties.SuspendLayout();
            this.pnlBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlGradient)).BeginInit();
            this.pnlGradient.SuspendLayout();
            this.SuspendLayout();
            // 
            // properties
            // 
            this.properties.BackColor = System.Drawing.SystemColors.Control;
            this.properties.CommandsVisibleIfAvailable = true;
            this.properties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.properties.LargeButtons = false;
            this.properties.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.properties.Location = new System.Drawing.Point(2, 26);
            this.properties.Name = "properties";
            this.properties.Size = new System.Drawing.Size(306, 340);
            this.properties.TabIndex = 6;
            this.properties.Text = "propertyGrid1";
            this.properties.ToolbarVisible = false;
            this.properties.ViewBackColor = System.Drawing.SystemColors.Window;
            this.properties.ViewForeColor = System.Drawing.SystemColors.WindowText;
            this.properties.SelectedObjectsChanged += new System.EventHandler(this.PropertyGrid1_SelectedObjectsChanged);
            // 
            // propLabel
            // 
            this.propLabel.BackColor = System.Drawing.Color.Transparent;
            this.propLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.propLabel.Location = new System.Drawing.Point(2, 2);
            this.propLabel.Name = "propLabel";
            this.propLabel.Size = new System.Drawing.Size(306, 24);
            this.propLabel.TabIndex = 5;
            this.propLabel.Text = "&Properties:";
            this.propLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // memLabel
            // 
            this.memLabel.BackColor = System.Drawing.Color.Transparent;
            this.memLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.memLabel.Location = new System.Drawing.Point(2, 2);
            this.memLabel.Name = "memLabel";
            this.memLabel.Size = new System.Drawing.Size(296, 24);
            this.memLabel.TabIndex = 0;
            this.memLabel.Text = "&Base Styles: (F2 or Mouse Click to edit)";
            this.memLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // styleNamesList
            // 
            // 
            // styleNamesList.Button
            // 
            this.styleNamesList.Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.styleNamesList.Button.Location = new System.Drawing.Point(112, 120);
            this.styleNamesList.Button.Name = "button";
            this.styleNamesList.Button.Size = new System.Drawing.Size(30, 20);
            this.styleNamesList.Button.TabIndex = 2;
            this.styleNamesList.Button.Text = "...";
            this.styleNamesList.Button.Visible = false;
            this.styleNamesList.Controls.Add(this.styleNamesList.Button);
            this.styleNamesList.Controls.Add(this.styleNamesList.TextBox);
            this.styleNamesList.Controls.Add(this.styleNamesList.ListBox);
            this.styleNamesList.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // styleNamesList.ListBox
            // 
            this.styleNamesList.ListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.styleNamesList.ListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.styleNamesList.ListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.styleNamesList.ListBox.Location = new System.Drawing.Point(0, 0);
            this.styleNamesList.ListBox.Name = "listBox";
            this.styleNamesList.ListBox.Size = new System.Drawing.Size(296, 340);
            this.styleNamesList.ListBox.TabIndex = 0;
            this.styleNamesList.ListBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.StyleNamesList_ListBox_MouseMove);
            this.styleNamesList.ListBox.SelectedIndexChanged += new System.EventHandler(this.StyleNamesList_ListBox_SelectedIndexChanged);
            this.styleNamesList.Location = new System.Drawing.Point(2, 26);
            this.styleNamesList.Name = "styleNamesList";
            this.styleNamesList.Size = new System.Drawing.Size(296, 340);
            this.styleNamesList.TabIndex = 1;
            // 
            // styleNamesList.TextBox
            // 
            this.styleNamesList.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.styleNamesList.TextBox.Location = new System.Drawing.Point(8, 120);
            this.styleNamesList.TextBox.Name = "textBox";
            this.styleNamesList.TextBox.TabIndex = 2;
            this.styleNamesList.TextBox.Visible = false;
            this.styleNamesList.WantButton = false;
            this.styleNamesList.ItemChanging += new Syncfusion.Windows.Forms.Tools.ListBoxTextChangingEventHandler(this.StyleNamesList_ItemChanging);
            this.styleNamesList.BeforeListItemEdit += new System.ComponentModel.CancelEventHandler(this.StyleNamesList_BeforeListItemEdit);
            // 
            // addBtn
            // 
            this.addBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.addBtn.ComboEditBackColor = System.Drawing.Color.Empty;
            this.addBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addBtn.IsMouseDown = false;
            this.addBtn.Location = new System.Drawing.Point(16, 384);
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(96, 23);
            this.addBtn.TabIndex = 3;
            this.addBtn.Text = "&Add Style";
            this.addBtn.UseVisualStyle = true;
            this.addBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // removeBtn
            // 
            this.removeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.removeBtn.ComboEditBackColor = System.Drawing.Color.Empty;
            this.removeBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.removeBtn.IsMouseDown = false;
            this.removeBtn.Location = new System.Drawing.Point(136, 384);
            this.removeBtn.Name = "removeBtn";
            this.removeBtn.TabIndex = 4;
            this.removeBtn.Text = "&Remove";
            this.removeBtn.UseVisualStyle = true;
            this.removeBtn.Click += new System.EventHandler(this.RemoveBtn_Click);
            // 
            // closeBtn
            // 
            this.closeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.closeBtn.ComboEditBackColor = System.Drawing.Color.Empty;
            this.closeBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.closeBtn.IsMouseDown = false;
            this.closeBtn.Location = new System.Drawing.Point(548, 418);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.TabIndex = 8;
            this.closeBtn.Text = "&OK";
            this.closeBtn.UseVisualStyle = true;
            this.closeBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // addNodeLevelBtn
            // 
            this.addNodeLevelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addNodeLevelBtn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.addNodeLevelBtn.ButtonType = Syncfusion.Windows.Forms.Tools.ButtonTypes.ComboXPDown;
            this.addNodeLevelBtn.ComboEditBackColor = System.Drawing.Color.Empty;
            this.addNodeLevelBtn.IsMouseDown = false;
            this.addNodeLevelBtn.Location = new System.Drawing.Point(112, 384);
            this.addNodeLevelBtn.Name = "addNodeLevelBtn";
            this.addNodeLevelBtn.Size = new System.Drawing.Size(20, 23);
            this.addNodeLevelBtn.TabIndex = 2;
            this.addNodeLevelBtn.UseVisualStyle = true;
            this.addNodeLevelBtn.Click += new System.EventHandler(this.AddNodeLevelBtn_Click);
            // 
            // pnlList
            // 
            this.pnlList.BackColor = System.Drawing.Color.Transparent;
            this.pnlList.Controls.Add(this.styleNamesList);
            this.pnlList.Controls.Add(this.memLabel);
            this.pnlList.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlList.DockPadding.All = 2;
            this.pnlList.Location = new System.Drawing.Point(0, 0);
            this.pnlList.Name = "pnlList";
            this.pnlList.Size = new System.Drawing.Size(300, 368);
            this.pnlList.TabIndex = 9;
            // 
            // pnlProperties
            // 
            this.pnlProperties.BackColor = System.Drawing.Color.Transparent;
            this.pnlProperties.Controls.Add(this.properties);
            this.pnlProperties.Controls.Add(this.propLabel);
            this.pnlProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProperties.DockPadding.All = 2;
            this.pnlProperties.Location = new System.Drawing.Point(306, 0);
            this.pnlProperties.Name = "pnlProperties";
            this.pnlProperties.Size = new System.Drawing.Size(310, 368);
            this.pnlProperties.TabIndex = 10;
            // 
            // pnlBody
            // 
            this.pnlBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
              | System.Windows.Forms.AnchorStyles.Left)
              | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBody.BackColor = System.Drawing.Color.Transparent;
            this.pnlBody.Controls.Add(this.pnlProperties);
            this.pnlBody.Controls.Add(this.splitBody);
            this.pnlBody.Controls.Add(this.pnlList);
            this.pnlBody.Location = new System.Drawing.Point(8, 8);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Size = new System.Drawing.Size(616, 368);
            this.pnlBody.TabIndex = 11;
            // 
            // splitBody
            // 
            this.splitBody.BackColor = System.Drawing.Color.FromArgb(239, 244, 250);
            this.splitBody.Location = new System.Drawing.Point(300, 0);
            this.splitBody.MinSize = 200;
            this.splitBody.Name = "splitBody";
            this.splitBody.Size = new System.Drawing.Size(6, 368);
            this.splitBody.TabIndex = 10;
            this.splitBody.TabStop = false;
            // 
            // pnlGradient
            // 
            this.pnlGradient.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Syncfusion.Drawing.GradientStyle.Vertical, new System.Drawing.Color[] { System.Drawing.Color.FromArgb(((byte)(238)), ((byte)(243)), ((byte)(250))), System.Drawing.Color.White, System.Drawing.Color.FromArgb(((byte)(238)), ((byte)(243)), ((byte)(250))) });         
            this.pnlGradient.BorderColor = System.Drawing.Color.Black;
            this.pnlGradient.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pnlGradient.Controls.Add(this.lblInfo);
            this.pnlGradient.Controls.Add(this.closeBtn);
            this.pnlGradient.Controls.Add(this.pnlBody);
            this.pnlGradient.Controls.Add(this.addNodeLevelBtn);
            this.pnlGradient.Controls.Add(this.addBtn);
            this.pnlGradient.Controls.Add(this.removeBtn);
            this.pnlGradient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGradient.Location = new System.Drawing.Point(0, 0);
            this.pnlGradient.Name = "pnlGradient";
            this.pnlGradient.Size = new System.Drawing.Size(632, 446);
            this.pnlGradient.TabIndex = 12;
            // 
            // lblInfo
            // 
            this.lblInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblInfo.Location = new System.Drawing.Point(16, 416);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(288, 16);
            this.lblInfo.TabIndex = 12;
            this.lblInfo.Text = "Will be created style: {0}";
            // 
            // popupStyles
            // 
            this.popupStyles.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
        {
          this.mnuBaseStyle,
          this.mnuNodeLevelStyle,
          this.mnuColumnStyle,
          this.mnuSubItemStyle
        });
            // 
            // mnuBaseStyle
            // 
            this.mnuBaseStyle.Checked = true;
            this.mnuBaseStyle.Index = 0;
            this.mnuBaseStyle.Text = "&Base Style";
            this.mnuBaseStyle.Click += new System.EventHandler(this.MnuStyleSelect_Click);
            // 
            // mnuNodeLevelStyle
            // 
            this.mnuNodeLevelStyle.Index = 1;
            this.mnuNodeLevelStyle.Text = "Node &Level Style";
            this.mnuNodeLevelStyle.Click += new System.EventHandler(this.MnuStyleSelect_Click);
            // 
            // mnuColumnStyle
            // 
            this.mnuColumnStyle.Index = 2;
            this.mnuColumnStyle.Text = "&Column Style";
            this.mnuColumnStyle.Click += new System.EventHandler(this.MnuStyleSelect_Click);
            // 
            // mnuSubItemStyle
            // 
            this.mnuSubItemStyle.Index = 3;
            this.mnuSubItemStyle.Text = "Node &SubItem Style";
            this.mnuSubItemStyle.Click += new System.EventHandler(this.MnuStyleSelect_Click);
            // 
            // TreeViewAdvBaseStylesEditorForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(632, 446);
            this.Controls.Add(this.pnlGradient);
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "TreeViewAdvBaseStylesEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BaseStyles Collection Editor";
            this.Load += new System.EventHandler(this.TreeViewAdvBaseStylesEditorForm_Load);
            this.pnlList.ResumeLayout(false);
            this.pnlProperties.ResumeLayout(false);
            this.pnlBody.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlGradient)).EndInit();
            this.pnlGradient.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Class helper methods

        private void FillEditor()
        {
            foreach (string name in m_baseStyles.Keys)
            {
                styleNamesList.ListBox.Items.Add(name);
            }

            if (this.ListBox.Items.Count > 0)
            {
                this.ListBox.SelectedIndex = 0;
            }
        }

        private int HitTestListBox(Point ptClient)
        {
            int ht = 0;
            int i = 0;
            for (i = 0; i < this.ListBox.Items.Count; i++)
            {
                ht += this.ListBox.ItemHeight;

                if (ht >= ptClient.Y)
                {
                    break;
                }
            }

            return (i >= this.ListBox.Items.Count) ? -1 : i;
        }

        private void AddNewBaseStyle(StyleIndex type)
        {
            string newName = this.GenerateNewBaseStyleName(type);

            switch (type)
            {
                case StyleIndex.Base:
                case StyleIndex.NodeLevel:
                    m_baseStyles[newName] = new TreeNodeAdvStyleInfo(new TreeViewAdvStyleInfoIdentity(m_treeView));
                    break;

                case StyleIndex.Column:
                    m_baseStyles[newName] = new TreeColumnAdvStyleInfo(new TreeColumnAdvStyleInfoIdentity(m_treeView));
                    break;

                case StyleIndex.SubItem:
                    m_baseStyles[newName] = new TreeNodeAdvSubItemStyleInfo(new TreeNodeAdvSubItemStyleInfoIdentity(m_treeView));
                    break;
            }

            int newIndex = this.ListBox.Items.Add(newName);
            this.ListBox.SelectedIndex = newIndex;
        }

        private string GenerateNewBaseStyleName(StyleIndex type)
        {
            string baseStyleName;

            switch (type)
            {
                case StyleIndex.Column:
                    baseStyleName = MultiColumnTreeView.ColumnStyleBaseName;
                    break;
                case StyleIndex.NodeLevel:
                    baseStyleName = MultiColumnTreeView.NodeLevelStyleBaseName;
                    break;
                case StyleIndex.SubItem:
                    baseStyleName = MultiColumnTreeView.SubItemStyleBaseName;
                    break;
                default:
                    baseStyleName = MultiColumnTreeView.BaseStyleBaseName;
                    break;
            }

            string newName = String.Empty;
            int i = 0;

            while (true)
            {
                i++;
                newName = baseStyleName + i.ToString();

                if (!m_baseStyles.Contains(newName))
                {
                    break;
                }
            }

            return newName;
        }

        private bool IsValidBaseStyleName(string name)
        {
            if (name == null || name.Length == 0)
            {
                return false;
            }

            return !m_baseStyles.Contains(name);
        }
        #endregion

        #region Class event handlers

        private void TreeViewAdvBaseStylesEditorForm_Load(object sender, EventArgs e)
        {
            this.m_propBaseString = this.propLabel.Text;

            MnuStyleSelect_Click(mnuBaseStyle, EventArgs.Empty);

            this.FillEditor();
        }

        private void StyleNamesList_BeforeListItemEdit(object sender, CancelEventArgs e)
        {
            if (!m_treeView.IsBaseStyleRemoveable(this.SelectedItem.ToString()))
            {
                e.Cancel = true;
                MessageBox.Show("Cannot edit this base style.", "Error");
            }
        }

        private void StyleNamesList_ItemChanging(object sender, ListBoxTextChangingEventArgs e)
        {
            if (!this.IsValidBaseStyleName(e.NewText))
            {
                e.Cancel = true;
                MessageBox.Show("New Style Name is not unique, please provide a unique style name.", "Error");
            }
            else
            {
                if (e.LastSelectedIndex != -1)
                {
                    string oldStyleName = this.ListBox.Items[e.LastSelectedIndex].ToString();
                    m_baseStyles[e.NewText] = m_baseStyles[oldStyleName];
                    m_baseStyles.Remove(oldStyleName);

                    this.ToolTipIndex = -1;
                }
            }
        }

        private void StyleNamesList_ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selected = this.SelectedItem;

            if (null != selected)
            {
                styleNamesList.Editing = false;

                this.properties.SelectedObject = m_baseStyles[selected.ToString()];
            }
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            this.AddNewBaseStyle(m_styleType);
        }

        private void AddNodeLevelBtn_Click(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;
            Point point = new Point(0, ctrl.Height);
            this.popupStyles.Show(ctrl, point);
        }

        private void RemoveBtn_Click(object sender, EventArgs e)
        {
            if (this.ListBox.SelectedIndex == -1)
            {
                return;
            }

            string styleName = this.SelectedItem.ToString();

            if (m_treeView.IsBaseStyleRemoveable(styleName))
            {
                m_baseStyles.Remove(styleName);
                this.ListBox.Items.RemoveAt(this.ListBox.SelectedIndex);
                this.ListBox.SelectedIndex = this.ListBox.Items.Count - 1;
            }
            else
            {
                MessageBox.Show("Cannot remove selected base style.");
            }

            this.ToolTipIndex = -1;
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            m_treeView.MakeDirty();
            this.Close();
        }
        private void PropertyGrid1_SelectedObjectsChanged(object sender, EventArgs e)
        {
            if (this.ListBox.SelectedIndex == -1)
            {
                this.propLabel.Text = this.m_propBaseString + ":" + "(" + resetHintString + ")";
            }
            else
            {
                this.propLabel.Text = this.m_propBaseString + " for style " +
              this.SelectedItem.ToString() + ":"
              + "(" + resetHintString + ")";
            }
        }

        private void StyleNamesList_ListBox_MouseMove(object sender, MouseEventArgs e)
        {
            this.ToolTipIndex = this.HitTestListBox(new Point(e.X, e.Y));
        }

        private void MnuStyleSelect_Click(object sender, EventArgs e)
        {
            StyleIndex[] styles = new StyleIndex[] { StyleIndex.Base, StyleIndex.NodeLevel, StyleIndex.Column, StyleIndex.SubItem };
            MenuItem[] menus = new MenuItem[] { mnuBaseStyle, mnuNodeLevelStyle, mnuColumnStyle, mnuSubItemStyle };

            foreach (MenuItem menu in menus)
            {
                menu.Checked = menu == sender;
            }

            int index = Array.IndexOf(menus, sender);
            m_styleType = styles[index];

            lblInfo.Text = string.Format(confirmStyleChoice, m_styleType.ToString());
        }
        #endregion
    }
}