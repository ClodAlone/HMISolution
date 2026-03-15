#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Styles;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    public class ColumnsEditorForm : Form
    {
        #region Class members
        private IServiceProvider m_provider;
        private TreeColumnAdvCollection m_collection;
        private bool m_bIsUpdated = false;

        /// <summary> reference to source collection </summary>
        private TreeColumnAdvCollection m_sourceCollection;
        #endregion

        #region Class properties
        /// <summary> Gets or sets the Collection edited by dialog</summary>
        public TreeColumnAdvCollection Collection
        {
            get
            {
                return m_collection;
            }
            set
            {
                UnBindItems();

                if (m_collection != null)
                {
                    m_collection.CollectionChanged -= new CollectionChangeEventHandler(Collection_CollectionChanged);
                }

                m_sourceCollection = value;
                m_collection = m_sourceCollection.Clone();

                if (m_collection != null)
                {
                    m_collection.CollectionChanged += new CollectionChangeEventHandler(Collection_CollectionChanged);
                }

                MultiColumnTreeView adv = (m_collection.Count > 0) ? m_collection[0].TreeView : null;
                int height = (null != adv) ? adv.HeaderHeight : pnlColumnDraw.Height - 4;
                SetColumnsHeight(height);

                BindItems();
            }
        }
        #endregion

        #region Form controls
       
        private PropertyGridContextMenu pgMenu;
       
        private GradientPanel gradientPanelForm;
       
        private Panel pnlBody;
       
        private TreeViewAdvEditorPropertyGrid propertyEditor;
       
        private Splitter splitterBody;
     
        private ButtonAdv btnCancel;
       
        private ButtonAdv remove;
     
        private ButtonAdv addColumn;
       
        private Panel pnlProperties;
       
        private ListBox lstItems;
     
        private Panel pnlColumnDraw;
        
        private Splitter splitPreview;
        
        private CheckBox selectPreview;
        private Label lblProperties;
        private Panel pnlList;
        private Label lblList;
        private ButtonAdv btnOk;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        #endregion

        #region Class Initialize/Finalize methods

        public ColumnsEditorForm()
        {
            // Required for Windows Form Designer suppor
            InitializeComponent();

            // attach property grid context menu
            pgMenu = new PropertyGridContextMenu(propertyEditor);
            propertyEditor.HelpVisible = true;
        }

        public ColumnsEditorForm(TreeColumnAdvCollection columns)
            : this()
        {
            this.Collection = columns;
        }

        public ColumnsEditorForm(TreeColumnAdvCollection columns, IServiceProvider provider)
            : this(columns)
        {
            m_provider = provider;

            propertyEditor.Provider = m_provider;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">Bool disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (pgMenu != null)
                {
                    pgMenu.Dispose();
                    pgMenu = null;
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
            this.gradientPanelForm = new Syncfusion.Windows.Forms.Tools.GradientPanel();
            this.btnOk = new Syncfusion.Windows.Forms.ButtonAdv();
            this.pnlBody = new System.Windows.Forms.Panel();
            this.pnlProperties = new System.Windows.Forms.Panel();
            this.propertyEditor = new Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeViewAdvEditorPropertyGrid();
            this.selectPreview = new System.Windows.Forms.CheckBox();
            this.splitPreview = new System.Windows.Forms.Splitter();
            this.pnlColumnDraw = new System.Windows.Forms.Panel();
            this.lblProperties = new System.Windows.Forms.Label();
            this.splitterBody = new System.Windows.Forms.Splitter();
            this.pnlList = new System.Windows.Forms.Panel();
            this.lstItems = new System.Windows.Forms.ListBox();
            this.lblList = new System.Windows.Forms.Label();
            this.btnCancel = new Syncfusion.Windows.Forms.ButtonAdv();
            this.addColumn = new Syncfusion.Windows.Forms.ButtonAdv();
            this.remove = new Syncfusion.Windows.Forms.ButtonAdv();
            ((System.ComponentModel.ISupportInitialize)(this.gradientPanelForm)).BeginInit();
            this.gradientPanelForm.SuspendLayout();
            this.pnlBody.SuspendLayout();
            this.pnlProperties.SuspendLayout();
            this.pnlList.SuspendLayout();
            this.SuspendLayout();
            // 
            // gradientPanelForm
            // 
            this.gradientPanelForm.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Syncfusion.Drawing.GradientStyle.Vertical, new System.Drawing.Color[]
        {
          System.Drawing.Color.FromArgb( ( ( int )( ( ( byte )( 238 ) ) ) ), ( ( int )( ( ( byte )( 243 ) ) ) ), ( ( int )( ( ( byte )( 250 ) ) ) ) ),
          System.Drawing.Color.White,
          System.Drawing.Color.FromArgb( ( ( int )( ( ( byte )( 238 ) ) ) ), ( ( int )( ( ( byte )( 243 ) ) ) ), ( ( int )( ( ( byte )( 250 ) ) ) ) )
        });
            this.gradientPanelForm.BorderColor = System.Drawing.Color.Black;
            this.gradientPanelForm.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gradientPanelForm.Controls.Add(this.btnOk);
            this.gradientPanelForm.Controls.Add(this.pnlBody);
            this.gradientPanelForm.Controls.Add(this.btnCancel);
            this.gradientPanelForm.Controls.Add(this.addColumn);
            this.gradientPanelForm.Controls.Add(this.remove);
            this.gradientPanelForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gradientPanelForm.IgnoreThemeBackground = true;
            this.gradientPanelForm.Location = new System.Drawing.Point(0, 0);
            this.gradientPanelForm.Name = "gradientPanelForm";
            this.gradientPanelForm.Size = new System.Drawing.Size(632, 446);
            this.gradientPanelForm.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(462, 416);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(76, 24);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyle = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // pnlBody
            // 
            this.pnlBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
              | System.Windows.Forms.AnchorStyles.Left)
              | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBody.BackColor = System.Drawing.Color.Transparent;
            this.pnlBody.Controls.Add(this.pnlProperties);
            this.pnlBody.Controls.Add(this.splitterBody);
            this.pnlBody.Controls.Add(this.pnlList);
            this.pnlBody.Location = new System.Drawing.Point(8, 8);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Size = new System.Drawing.Size(616, 368);
            this.pnlBody.TabIndex = 0;
            // 
            // pnlProperties
            // 
            this.pnlProperties.BackColor = System.Drawing.Color.Transparent;
            this.pnlProperties.Controls.Add(this.propertyEditor);
            this.pnlProperties.Controls.Add(this.selectPreview);
            this.pnlProperties.Controls.Add(this.splitPreview);
            this.pnlProperties.Controls.Add(this.pnlColumnDraw);
            this.pnlProperties.Controls.Add(this.lblProperties);
            this.pnlProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProperties.Location = new System.Drawing.Point(306, 0);
            this.pnlProperties.Name = "pnlProperties";
            this.pnlProperties.Size = new System.Drawing.Size(310, 368);
            this.pnlProperties.TabIndex = 2;
            // 
            // propertyEditor
            // 
            this.propertyEditor.BackColor = System.Drawing.SystemColors.Control;
            this.propertyEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyEditor.HelpVisible = false;
            this.propertyEditor.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyEditor.Location = new System.Drawing.Point(0, 23);
            this.propertyEditor.Name = "propertyEditor";
            this.propertyEditor.Size = new System.Drawing.Size(310, 275);
            this.propertyEditor.TabIndex = 1;
            this.propertyEditor.ToolbarVisible = false;
            this.propertyEditor.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PropertyEditor_PropertyValueChanged);
            // 
            // selectPreview
            // 
            this.selectPreview.BackColor = System.Drawing.Color.Transparent;
            this.selectPreview.Checked = true;
            this.selectPreview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.selectPreview.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.selectPreview.Location = new System.Drawing.Point(0, 298);
            this.selectPreview.Name = "selectPreview";
            this.selectPreview.Size = new System.Drawing.Size(310, 24);
            this.selectPreview.TabIndex = 2;
            this.selectPreview.Text = "Show Column Pre&view";
            this.selectPreview.CheckedChanged += new System.EventHandler(this.SelectPreview_CheckedChanged);
            // 
            // splitPreview
            // 
            this.splitPreview.BackColor = System.Drawing.Color.FromArgb(239, 244, 250);
            this.splitPreview.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitPreview.Location = new System.Drawing.Point(0, 322);
            this.splitPreview.Name = "splitPreview";
            this.splitPreview.Size = new System.Drawing.Size(310, 6);
            this.splitPreview.TabIndex = 1;
            this.splitPreview.TabStop = false;
            // 
            // pnlColumnDraw
            // 
            this.pnlColumnDraw.AutoScroll = true;
            this.pnlColumnDraw.BackColor = System.Drawing.Color.Transparent;
            this.pnlColumnDraw.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlColumnDraw.Location = new System.Drawing.Point(0, 328);
            this.pnlColumnDraw.Name = "pnlColumnDraw";
            this.pnlColumnDraw.Size = new System.Drawing.Size(310, 40);
            this.pnlColumnDraw.TabIndex = 2;
            this.pnlColumnDraw.Paint += new System.Windows.Forms.PaintEventHandler(this.PnlColumnDraw_Paint);
            // 
            // lblProperties
            // 
            this.lblProperties.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblProperties.Location = new System.Drawing.Point(0, 0);
            this.lblProperties.Name = "lblProperties";
            this.lblProperties.Size = new System.Drawing.Size(310, 23);
            this.lblProperties.TabIndex = 0;
            this.lblProperties.Text = "&Properties:";
            this.lblProperties.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitterBody
            // 
            this.splitterBody.BackColor = System.Drawing.Color.FromArgb(239, 244, 250);
            this.splitterBody.Location = new System.Drawing.Point(300, 0);
            this.splitterBody.MinSize = 200;
            this.splitterBody.Name = "splitterBody";
            this.splitterBody.Size = new System.Drawing.Size(6, 368);
            this.splitterBody.TabIndex = 1;
            this.splitterBody.TabStop = false;
            // 
            // pnlList
            // 
            this.pnlList.Controls.Add(this.lstItems);
            this.pnlList.Controls.Add(this.lblList);
            this.pnlList.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlList.Location = new System.Drawing.Point(0, 0);
            this.pnlList.Name = "pnlList";
            this.pnlList.Size = new System.Drawing.Size(300, 368);
            this.pnlList.TabIndex = 6;
            // 
            // lstItems
            // 
            this.lstItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstItems.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstItems.HorizontalExtent = 2;
            this.lstItems.ItemHeight = 22;
            this.lstItems.Location = new System.Drawing.Point(0, 23);
            this.lstItems.Name = "lstItems";
            this.lstItems.Size = new System.Drawing.Size(300, 332);
            this.lstItems.TabIndex = 1;
            this.lstItems.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.LstItems_DrawItem);
            this.lstItems.SelectedIndexChanged += new System.EventHandler(this.LstItems_SelectedIndexChanged);
            // 
            // lblList
            // 
            this.lblList.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblList.Location = new System.Drawing.Point(0, 0);
            this.lblList.Name = "lblList";
            this.lblList.Size = new System.Drawing.Size(300, 23);
            this.lblList.TabIndex = 0;
            this.lblList.Text = "Columns &List:";
            this.lblList.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(544, 416);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 24);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyle = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // addColumn
            // 
            this.addColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addColumn.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.addColumn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addColumn.Location = new System.Drawing.Point(16, 384);
            this.addColumn.Name = "addColumn";
            this.addColumn.Size = new System.Drawing.Size(88, 24);
            this.addColumn.TabIndex = 0;
            this.addColumn.Text = "&Add Column";
            this.addColumn.UseVisualStyle = true;
            this.addColumn.Click += new System.EventHandler(this.AddColumn_Click);
            // 
            // Remove
            // 
            this.remove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.remove.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.remove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.remove.Location = new System.Drawing.Point(108, 384);
            this.remove.Name = "Remove";
            this.remove.Size = new System.Drawing.Size(88, 24);
            this.remove.TabIndex = 1;
            this.remove.Text = "&Remove";
            this.remove.UseVisualStyle = true;
            this.remove.Click += new System.EventHandler(this.Remove_Click);
            // 
            // ColumnsEditorForm
            // 
            this.AcceptButton = this.btnCancel;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(632, 446);
            this.Controls.Add(this.gradientPanelForm);
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "ColumnsEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Columns Editor";
            ((System.ComponentModel.ISupportInitialize)(this.gradientPanelForm)).EndInit();
            this.gradientPanelForm.ResumeLayout(false);
            this.pnlBody.ResumeLayout(false);
            this.pnlProperties.ResumeLayout(false);
            this.pnlList.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Sets height for all columns
        /// </summary>
        /// <param name="height">Column Height</param>
        private void SetColumnsHeight(int height)
        {
            for (int i = 0; i < m_collection.Count; i++)
            {
                m_collection[i].Height = height;
            }
        }

        private void InvalidatePreview()
        {
            if (pnlColumnDraw.Visible)
            {
                pnlColumnDraw.Invalidate();
                pnlColumnDraw.Update();
            }
        }

        private void BindItems()
        {
            // lstItems.BeginUpdate();
            if (this.Collection != null && this.Collection.Count > 0)
            {
                lstItems.DataSource = null;
                lstItems.DisplayMember = string.Empty;

                lstItems.DataSource = this.Collection;
                lstItems.DisplayMember = "Text";

                if (lstItems.Items.Count > 0)
                {
                    lstItems.SelectedIndex = 0;
                }
            }

            // lstItems.EndUpdate();
        }
     
        private void UnBindItems()
        {
            lstItems.BeginUpdate();
            lstItems.DataSource = null;
            lstItems.Items.Clear();
            lstItems.EndUpdate();
        }

        /// <summary>
        /// Gets unique name for collection of columns.
        /// </summary>
        /// <returns>Returns Uniquw Column name</returns>
        private string UniqueColumnName()
        {
            int i = 0;
            string columnName = string.Empty;

            do
            {
                columnName = "TreeColumnAdv" + i.ToString();
                i++;
            }
            while (!IsColumnsNameUnique(columnName));

            return columnName;
        }

        /// <summary>
        /// Checks column name on unigue.
        /// </summary>
        /// <returns>Returns true if column name is unique</returns>
        /// <param name="columnName">Column Name</param>
        private bool IsColumnsNameUnique(string columnName)
        {
            foreach (TreeColumnAdv column in this.Collection)
            {
                if (column.Text == columnName)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Class event handlers
       
        private void Collection_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            // save selection
            int index = lstItems.SelectedIndex;

            UnBindItems();
            BindItems();

            // recover selection
            lstItems.SelectedIndex = Math.Min(index, lstItems.Items.Count - 1);
        }

        private void PnlColumnDraw_Paint(object sender, PaintEventArgs e)
        {
            TreeColumnAdv column = lstItems.SelectedItem as TreeColumnAdv;

            if (column != null)
            {
                Size colSize = column.Size;
                int x = (pnlColumnDraw.Width - Math.Min(colSize.Width, pnlColumnDraw.Width)) / 2;
                int y = (pnlColumnDraw.Height - Math.Min(colSize.Height, pnlColumnDraw.Height)) / 2;

                int width = Math.Max(pnlColumnDraw.Size.Width, colSize.Width);
                int height = Math.Max(pnlColumnDraw.Size.Height, colSize.Height);

                pnlColumnDraw.AutoScrollMinSize = new Size(width, height);

                Point point = new Point(x - pnlColumnDraw.AutoScrollPosition.X, y - pnlColumnDraw.AutoScrollPosition.Y);
                column.Draw(e.Graphics, point);
            }
        }

        private void LstItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (propertyEditor.SelectedObject != null)
            {
                TreeColumnAdv column = propertyEditor.SelectedObject as TreeColumnAdv;

                if (column != null)
                {
                    column.ColumnStyle.Changed -= new StyleChangedEventHandler(ColumnStyle_Changed);
                }
            }

            propertyEditor.SelectedObject = lstItems.SelectedItem;

            if (propertyEditor.SelectedObject != null)
            {
                TreeColumnAdv column = propertyEditor.SelectedObject as TreeColumnAdv;

                if (column != null)
                {
                    column.ColumnStyle.Changed += new StyleChangedEventHandler(ColumnStyle_Changed);
                }
            }

            InvalidatePreview();
        }

        /// <summary>
        /// Used for invalidate preview when user press Reset.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">EventArgs that contains the event data.</param>
        private void ColumnStyle_Changed(object sender, StyleChangedEventArgs e)
        {
            m_bIsUpdated = true;
            InvalidatePreview();
        }

        private void LstItems_DrawItem(object sender, DrawItemEventArgs e)
        {
            TreeColumnAdv column = lstItems.Items[e.Index] as TreeColumnAdv;

            e.DrawBackground();

            Graphics g = e.Graphics;
            Rectangle rcButton = new Rectangle(e.Bounds.X, e.Bounds.Y, lstItems.ItemHeight, lstItems.ItemHeight);
            ControlPaint.DrawButton(g, Rectangle.Inflate(rcButton, -1, -1), ButtonState.Normal);

            using (SolidBrush brush = new SolidBrush(e.ForeColor))
            {
                using (StringFormat format = StringFormat.GenericTypographic)
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    format.FormatFlags |= StringFormatFlags.LineLimit | StringFormatFlags.NoClip | StringFormatFlags.NoWrap;

                    // draw order index of item
                    g.DrawString(e.Index.ToString(), e.Font, brush, rcButton, format);

                    // draw column text
                    format.FormatFlags &= ~StringFormatFlags.NoClip;
                    format.Alignment = StringAlignment.Near;
                    format.Trimming = StringTrimming.EllipsisCharacter;
                    Rectangle rcText = new Rectangle(rcButton.Right + 2, e.Bounds.Top, e.Bounds.Width - rcButton.Width - 2, e.Bounds.Height);
                    string text = string.Format("Text: {0}", (column != null) ? column.Text : "<not set>");
                    g.DrawString(text, e.Font, brush, rcText, format);
                }
            }
        }

        private void SelectPreview_CheckedChanged(object sender, EventArgs e)
        {
            pnlColumnDraw.Visible = selectPreview.Checked;
            splitPreview.Visible = selectPreview.Checked;
        }

        private void PropertyEditor_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (!m_bIsUpdated)
            {
                InvalidatePreview();
            }
            m_bIsUpdated = false;
        }
     
        private void AddColumn_Click(object sender, EventArgs e)
        {
            if (this.Collection != null)
            {
                TreeColumnAdv column = new TreeColumnAdv(UniqueColumnName());
                this.Collection.Add(column);

                lstItems.SelectedItem = column;
            }
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            if (this.Collection != null)
            {
                // save reference on selected item
                TreeColumnAdv selectedItem = lstItems.SelectedItem as TreeColumnAdv;

                if (null != selectedItem)
                {
                    // reset binding position
                    if (lstItems.DataSource != null && lstItems.BindingContext.Contains(lstItems.DataSource))
                    {
                        lstItems.BindingContext[lstItems.DataSource].Position = -1;
                    }

                    this.Collection.Remove(selectedItem);
                }
            }
        }

        /// <summary>
        /// Closes editor and saves data.
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e">EventArgs that contains the event data.</param>
        private void BtnOk_Click(object sender, EventArgs e)
        {
            m_sourceCollection.Clear();
            m_sourceCollection.AddRange(m_collection);
        }

        /// <summary>
        /// Closes editor and loses data.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs that contains the event data. </param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}