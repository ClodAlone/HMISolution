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

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    public class PrimitivesEditorForm : Form
    {
        #region members
 
        private static readonly string[] m_arrPrimitiveTypes = null;

        private static readonly Hashtable m_htPrimitiveTypes = null;

        private TreeNodePrimitivesCollection m_primitives;
        #endregion

        #region Form controls

        private PropertyGrid pgPrimitive;

        private ButtonAdv btnOK;
  
        private ButtonAdv btnRemove;

        private ButtonAdv btnAdd;

        private ComboBox cbPrimitives;

        private GradientPanel panelForm;

        private Panel pnlBody;

        private Panel pnlList;

        private Label lblList;

        private Splitter splitBody;
  
        private Panel pnlProperties;

        private Label label1;

        private ListBox lbPrimitives;
        #endregion

        #region Properties
        public TreeNodePrimitivesCollection Primitives
        {
            get
            {
                return m_primitives;
            }
        }
        #endregion

        #region Initialize\Deinitialize methods
        static PrimitivesEditorForm()
        {
            m_arrPrimitiveTypes = Enum.GetNames(typeof(PredefinedPrimitiveTypes));
            m_htPrimitiveTypes = new Hashtable();

            for (int i = 0, len = m_arrPrimitiveTypes.Length; i < len; i++)
            {
                string strTypeName = m_arrPrimitiveTypes[i];

                m_htPrimitiveTypes[strTypeName] = (PredefinedPrimitiveTypes)Enum.Parse(
                 typeof(PredefinedPrimitiveTypes), strTypeName, true);
            }
        }

        public PrimitivesEditorForm(TreeNodePrimitivesCollection primitives)
        {
            if (primitives == null)
            {
                throw new ArgumentNullException("primitives");
            }

            InitializeComponent();

            InitPrimitives(primitives);
        }
        #endregion

        #region Designer-generated code
        /// <summary></summary>
        private void InitializeComponent()
        {
            this.panelForm = new GradientPanel();
            this.pnlBody = new Panel();
            this.pnlProperties = new Panel();
            this.pgPrimitive = new PropertyGrid();
            this.label1 = new Label();
            this.splitBody = new Splitter();
            this.pnlList = new Panel();
            this.lbPrimitives = new ListBox();
            this.lblList = new Label();
            this.btnOK = new ButtonAdv();
            this.btnAdd = new ButtonAdv();
            this.btnRemove = new ButtonAdv();
            this.cbPrimitives = new ComboBox();
            ((ISupportInitialize)(this.panelForm)).BeginInit();
            this.panelForm.SuspendLayout();
            this.pnlBody.SuspendLayout();
            this.pnlProperties.SuspendLayout();
            this.pnlList.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelForm
            // 
            this.panelForm.BackgroundColor = new BrushInfo();
            this.panelForm.BorderColor = Color.Black;
            this.panelForm.BorderStyle = BorderStyle.None;
            this.panelForm.Controls.Add(this.pnlBody);
            this.panelForm.Controls.Add(this.btnOK);
            this.panelForm.Controls.Add(this.btnAdd);
            this.panelForm.Controls.Add(this.btnRemove);
            this.panelForm.Controls.Add(this.cbPrimitives);
            this.panelForm.Dock = DockStyle.Fill;
            this.panelForm.Location = new Point(0, 0);
            this.panelForm.Name = "panelForm";
            this.panelForm.Size = new Size(632, 446);
            this.panelForm.TabIndex = 0;
            // 
            // pnlBody
            // 
            this.pnlBody.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
              | AnchorStyles.Left)
              | AnchorStyles.Right)));
            this.pnlBody.BackColor = Color.Transparent;
            this.pnlBody.Controls.Add(this.pnlProperties);
            this.pnlBody.Controls.Add(this.splitBody);
            this.pnlBody.Controls.Add(this.pnlList);
            this.pnlBody.Location = new Point(8, 8);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Size = new Size(616, 368);
            this.pnlBody.TabIndex = 0;
            // 
            // pnlProperties
            // 
            this.pnlProperties.Controls.Add(this.pgPrimitive);
            this.pnlProperties.Controls.Add(this.label1);
            this.pnlProperties.Dock = DockStyle.Fill;
            this.pnlProperties.Location = new Point(306, 0);
            this.pnlProperties.Name = "pnlProperties";
            this.pnlProperties.Size = new Size(310, 368);
            this.pnlProperties.TabIndex = 4;
            // 
            // pgPrimitive
            // 
            this.pgPrimitive.BackColor = SystemColors.Control;
            this.pgPrimitive.CommandsVisibleIfAvailable = true;
            this.pgPrimitive.Dock = DockStyle.Fill;
            this.pgPrimitive.LargeButtons = false;
            this.pgPrimitive.LineColor = SystemColors.ScrollBar;
            this.pgPrimitive.Location = new Point(0, 23);
            this.pgPrimitive.Name = "pgPrimitive";
            this.pgPrimitive.Size = new Size(310, 345);
            this.pgPrimitive.TabIndex = 1;
            this.pgPrimitive.Text = "pgPrimitive";
            this.pgPrimitive.ToolbarVisible = false;
            this.pgPrimitive.ViewBackColor = SystemColors.Window;
            this.pgPrimitive.ViewForeColor = SystemColors.WindowText;
            // 
            // label1
            // 
            this.label1.Dock = DockStyle.Top;
            this.label1.Location = new Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(310, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Properties:";
            this.label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // splitBody
            // 
            this.splitBody.BackColor = System.Drawing.Color.FromArgb(239, 244, 250);
            this.splitBody.Location = new Point(300, 0);
            this.splitBody.MinSize = 270;
            this.splitBody.Name = "splitBody";
            this.splitBody.Size = new Size(6, 368);
            this.splitBody.TabIndex = 3;
            this.splitBody.TabStop = false;
            // 
            // pnlList
            // 
            this.pnlList.Controls.Add(this.lbPrimitives);
            this.pnlList.Controls.Add(this.lblList);
            this.pnlList.Dock = DockStyle.Left;
            this.pnlList.Location = new Point(0, 0);
            this.pnlList.Name = "pnlList";
            this.pnlList.Size = new Size(300, 368);
            this.pnlList.TabIndex = 0;
            // 
            // lbPrimitives
            // 
            this.lbPrimitives.BorderStyle = BorderStyle.FixedSingle;
            this.lbPrimitives.Dock = DockStyle.Fill;
            this.lbPrimitives.Location = new Point(0, 23);
            this.lbPrimitives.Name = "lbPrimitives";
            this.lbPrimitives.Size = new Size(300, 340);
            this.lbPrimitives.TabIndex = 1;
            this.lbPrimitives.SelectedIndexChanged += new EventHandler(this.LbPrimitives_SelectedIndexChanged);
            // 
            // lblList
            // 
            this.lblList.Dock = DockStyle.Top;
            this.lblList.Location = new Point(0, 0);
            this.lblList.Name = "lblList";
            this.lblList.Size = new Size(300, 23);
            this.lblList.TabIndex = 0;
            this.lblList.Text = "Primitives &List:";
            this.lblList.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.btnOK.Appearance = ButtonAppearance.Office2007;
            this.btnOK.ComboEditBackColor = Color.Empty;
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.ImageAlign = ContentAlignment.MiddleLeft;
            this.btnOK.IsMouseDown = false;
            this.btnOK.Location = new Point(544, 414);
            this.btnOK.Name = "btnOK";
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "&Close";
            this.btnOK.UseVisualStyle = true;
            this.btnOK.Click += new EventHandler(this.BtnOK_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            this.btnAdd.Appearance = ButtonAppearance.Office2007;
            this.btnAdd.ComboEditBackColor = Color.Empty;
            this.btnAdd.ImageAlign = ContentAlignment.MiddleLeft;
            this.btnAdd.IsMouseDown = false;
            this.btnAdd.Location = new Point(144, 384);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "&Add...";
            this.btnAdd.UseVisualStyle = true;
            this.btnAdd.Click += new EventHandler(this.BtnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            this.btnRemove.Appearance = ButtonAppearance.Office2007;
            this.btnRemove.ComboEditBackColor = Color.Empty;
            this.btnRemove.ImageAlign = ContentAlignment.MiddleLeft;
            this.btnRemove.IsMouseDown = false;
            this.btnRemove.Location = new Point(224, 384);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "&Remove";
            this.btnRemove.UseVisualStyle = true;
            this.btnRemove.Click += new EventHandler(this.BtnRemove_Click);
            // 
            // cbPrimitives
            // 
            this.cbPrimitives.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            this.cbPrimitives.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbPrimitives.Location = new Point(16, 384);
            this.cbPrimitives.Name = "cbPrimitives";
            this.cbPrimitives.Size = new Size(120, 21);
            this.cbPrimitives.TabIndex = 0;
            // 
            // PrimitivesEditorForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new Size(5, 13);
            this.ClientSize = new Size(632, 446);
            this.Controls.Add(this.panelForm);
            this.MinimumSize = new Size(500, 400);
            this.Name = "PrimitivesEditorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "TreeNodePrimitives collection editor";
            ((ISupportInitialize)(this.panelForm)).EndInit();
            this.panelForm.ResumeLayout(false);
            this.pnlBody.ResumeLayout(false);
            this.pnlProperties.ResumeLayout(false);
            this.pnlList.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Implementation
        private void FillPrimitiveTypesList()
        {
            cbPrimitives.Items.Clear();

            for (int i = 0, len = m_arrPrimitiveTypes.Length; i < len; i++)
            {
                string strTypeName = m_arrPrimitiveTypes[i];
                PredefinedPrimitiveTypes primitiveType = (PredefinedPrimitiveTypes)m_htPrimitiveTypes[strTypeName];

                if (m_primitives.IsValidPrimitiveType(primitiveType))
                {
                    cbPrimitives.Items.Add(strTypeName);
                }
            }

            if (cbPrimitives.Items.Count > 0)
            {
                cbPrimitives.Enabled = true;
                cbPrimitives.SelectedIndex = 0;
                btnAdd.Enabled = true;
            }
            else
            {
                cbPrimitives.Enabled = false;
                btnAdd.Enabled = false;
            }
        }

        private void InitPrimitives(TreeNodePrimitivesCollection primitives)
        {
            lbPrimitives.Items.Clear();

            if (primitives != null)
            {
                m_primitives = primitives.Clone();
                m_primitives.CollectionChanged += new CollectionChangeEventHandler(Primitives_CollectionChanged);

                for (int i = 0, len = m_primitives.Count; i < len; i++)
                {
                    lbPrimitives.Items.Add(m_primitives[i].PrimitiveType);
                }
            }

            FillPrimitiveTypesList();

            if (lbPrimitives.Items.Count > 0)
            {
                lbPrimitives.SelectedIndex = 0;
                pgPrimitive.SelectedObject = m_primitives[0];
            }
        }
        #endregion

        #region EventHandlers
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (cbPrimitives.SelectedIndex >= 0)
            {
                string strEnumValue = (string)cbPrimitives.SelectedItem;

                PredefinedPrimitiveTypes primitiveType = (PredefinedPrimitiveTypes)m_htPrimitiveTypes[strEnumValue];

                TreeNodePrimitive primitive = new TreeNodePrimitive(0, primitiveType);
                m_primitives.Add(primitive);
                lbPrimitives.Items.Add(primitiveType);

                lbPrimitives.SelectedIndex = lbPrimitives.Items.Count - 1;
                pgPrimitive.SelectedObject = m_primitives[lbPrimitives.SelectedIndex];

                FillPrimitiveTypesList();
            }
        }
        private void BtnRemove_Click(object sender, EventArgs e)
        {
            int selIndex = lbPrimitives.SelectedIndex;

            if (selIndex >= 0)
            {
                lbPrimitives.Items.RemoveAt(selIndex);
                TreeNodePrimitive removePrimitive = m_primitives[selIndex];
                m_primitives.Remove(removePrimitive);
            }

            if (lbPrimitives.Items.Count > 0)
            {
                if (selIndex >= lbPrimitives.Items.Count)
                {
                    selIndex = lbPrimitives.Items.Count - 1;
                    pgPrimitive.SelectedObject = m_primitives[selIndex];
                }

                lbPrimitives.SelectedIndex = selIndex;
            }
            FillPrimitiveTypesList();
        }
        private void LbPrimitives_SelectedIndexChanged(object sender, EventArgs e)
        {
            pgPrimitive.SelectedObject = (lbPrimitives.SelectedIndex >= 0) ?
              m_primitives[lbPrimitives.SelectedIndex] : null;
        }

        private void Primitives_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            TreeNodePrimitive primitive = (TreeNodePrimitive)e.Element;
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    primitive.PropertyChanged += new SyncfusionPropertyChangedEventHandler(Primitive_PropertyChanged);
                    break;
                case CollectionChangeAction.Remove:
                    primitive.PropertyChanged -= new SyncfusionPropertyChangedEventHandler(Primitive_PropertyChanged);
                    break;
            }
        }

        private void Primitive_PropertyChanged(object sender, SyncfusionPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PrimitiveType" && lbPrimitives.SelectedIndex >= 0)
            {
                lbPrimitives.Items[lbPrimitives.SelectedIndex] = e.NewValue.ToString();
                FillPrimitiveTypesList();
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion
    }
}