#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using Syncfusion.ComponentModel;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	public class PrimitivesEditorForm : Form
	{
		#region members
		private static readonly string[] m_arrPrimitiveTypes = null;
		private static readonly Hashtable m_htPrimitiveTypes = null;

		private TreeNodePrimitivesCollection m_primitives = new TreeNodePrimitivesCollection();
		#endregion		

		#region Form controls
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.PropertyGrid pgPrimitive;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.ComboBox cbPrimitives;
		private System.Windows.Forms.ListBox lbPrimitives;
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
			m_arrPrimitiveTypes = Enum.GetNames( typeof( PredefinedPrimitiveTypes ) );
			m_htPrimitiveTypes = new Hashtable();

			for( int i = 0, len = m_arrPrimitiveTypes.Length; i < len; i++ )
			{
				string strTypeName = m_arrPrimitiveTypes[ i ];

				m_htPrimitiveTypes[ strTypeName ] = ( PredefinedPrimitiveTypes )Enum.Parse( 
					typeof( PredefinedPrimitiveTypes ), strTypeName, true );

			}
		}
		public PrimitivesEditorForm( TreeNodePrimitivesCollection primitives )
		{
			if( primitives == null )
				throw new ArgumentNullException( "primitives" );

			InitializeComponent();

			InitPrimitives( primitives );
		}
		#endregion

		#region Designer-generated code
		private void InitializeComponent()
		{
			this.panelBottom = new System.Windows.Forms.Panel();
			this.cbPrimitives = new System.Windows.Forms.ComboBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.pgPrimitive = new System.Windows.Forms.PropertyGrid();
			this.lbPrimitives = new System.Windows.Forms.ListBox();
			this.panelBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.cbPrimitives);
			this.panelBottom.Controls.Add(this.btnOK);
			this.panelBottom.Controls.Add(this.btnCancel);
			this.panelBottom.Controls.Add(this.btnRemove);
			this.panelBottom.Controls.Add(this.btnAdd);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(0, 342);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(480, 40);
			this.panelBottom.TabIndex = 2;
			// 
			// cbPrimitives
			// 
			this.cbPrimitives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPrimitives.Location = new System.Drawing.Point(8, 8);
			this.cbPrimitives.Name = "cbPrimitives";
			this.cbPrimitives.Size = new System.Drawing.Size(104, 21);
			this.cbPrimitives.TabIndex = 0;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Location = new System.Drawing.Point(320, 8);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(400, 8);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnRemove.Location = new System.Drawing.Point(200, 8);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.TabIndex = 2;
			this.btnRemove.Text = "Remove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAdd.Location = new System.Drawing.Point(120, 8);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.TabIndex = 1;
			this.btnAdd.Text = "Add...";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// pgPrimitive
			// 
			this.pgPrimitive.CommandsVisibleIfAvailable = true;
			this.pgPrimitive.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgPrimitive.LargeButtons = false;
			this.pgPrimitive.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.pgPrimitive.Location = new System.Drawing.Point(232, 0);
			this.pgPrimitive.Name = "pgPrimitive";
			this.pgPrimitive.Size = new System.Drawing.Size(248, 342);
			this.pgPrimitive.TabIndex = 1;
			this.pgPrimitive.Text = "pgPrimitive";
			this.pgPrimitive.ViewBackColor = System.Drawing.SystemColors.Window;
			this.pgPrimitive.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// lbPrimitives
			// 
			this.lbPrimitives.Dock = System.Windows.Forms.DockStyle.Left;
			this.lbPrimitives.Location = new System.Drawing.Point(0, 0);
			this.lbPrimitives.Name = "lbPrimitives";
			this.lbPrimitives.Size = new System.Drawing.Size(232, 342);
			this.lbPrimitives.TabIndex = 0;
			this.lbPrimitives.SelectedIndexChanged += new System.EventHandler(this.lbPrimitives_SelectedIndexChanged);
			// 
			// PrimitivesEditorForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(480, 382);
			this.Controls.Add(this.pgPrimitive);
			this.Controls.Add(this.lbPrimitives);
			this.Controls.Add(this.panelBottom);
			this.Name = "PrimitivesEditorForm";
			this.ShowInTaskbar = false;
			this.Text = "TreeNodePrimitives collection editor";
			this.panelBottom.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Implementation
		private void FillPrimitiveTypesList()
		{
			cbPrimitives.Items.Clear();

			for( int i = 0, len = m_arrPrimitiveTypes.Length; i < len; i++ )
			{
				string strTypeName = m_arrPrimitiveTypes[ i ];
				PredefinedPrimitiveTypes primitiveType = ( PredefinedPrimitiveTypes )m_htPrimitiveTypes[ strTypeName ];

				if( m_primitives.IsValidPrimitiveType( primitiveType ) )
				{
					cbPrimitives.Items.Add( strTypeName );
				}
			}
	
			if( cbPrimitives.Items.Count > 0 )
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

		/// <summary>
		///
		/// </summary>
		private void InitPrimitives( TreeNodePrimitivesCollection primitives )
		{
			lbPrimitives.Items.Clear();
			
			m_primitives.Clear();
			m_primitives.CollectionChanged +=new System.ComponentModel.CollectionChangeEventHandler(m_primitives_CollectionChanged);
			if( primitives != null && primitives.Count > 0 )
			{				
				for( int i = 0, len = primitives.Count; i < len; i++ )
				{
					m_primitives.Add( ( TreeNodePrimitive )primitives[ i ].Clone() );
					lbPrimitives.Items.Add( primitives[ i ].PrimitiveType );
				}
			}
			
			FillPrimitiveTypesList();

			if( lbPrimitives.Items.Count > 0 )
			{
				lbPrimitives.SelectedIndex = 0;
				pgPrimitive.SelectedObject = m_primitives[ 0 ];
			}
		}
		#endregion

		#region EventHandlers
		private void btnAdd_Click(object sender, EventArgs e)
		{
			if( cbPrimitives.SelectedIndex >= 0 )
			{
				string strEnumValue = ( string )cbPrimitives.SelectedItem;

				PredefinedPrimitiveTypes primitiveType = ( PredefinedPrimitiveTypes )m_htPrimitiveTypes[ strEnumValue ];

				TreeNodePrimitive primitive = new TreeNodePrimitive( 0, primitiveType );
				m_primitives.Add( primitive );
				lbPrimitives.Items.Add( primitiveType );

				lbPrimitives.SelectedIndex = lbPrimitives.Items.Count - 1;
				pgPrimitive.SelectedObject = m_primitives[ lbPrimitives.SelectedIndex ];

				FillPrimitiveTypesList();
			}
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			int selIndex = lbPrimitives.SelectedIndex;

			if( selIndex >= 0 )
			{
				lbPrimitives.Items.RemoveAt( selIndex );
				m_primitives.RemoveAt( selIndex );
			}

			if( lbPrimitives.Items.Count > 0 )
			{
				if( selIndex >= lbPrimitives.Items.Count )
				{
					selIndex = lbPrimitives.Items.Count - 1;
					pgPrimitive.SelectedObject = m_primitives[ selIndex ];
				}

				lbPrimitives.SelectedIndex = selIndex;

			}
			FillPrimitiveTypesList();
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void lbPrimitives_SelectedIndexChanged(object sender, EventArgs e)
		{
            pgPrimitive.SelectedObject = ( lbPrimitives.SelectedIndex >= 0 ) ? 
				m_primitives [ lbPrimitives.SelectedIndex ] : null;
		}
		#endregion

		private void m_primitives_CollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			TreeNodePrimitive primitive = ( TreeNodePrimitive )e.Element;
			switch( e.Action )
			{
				case CollectionChangeAction.Add:
					primitive.PropertyChanged += new SyncfusionPropertyChangedEventHandler( primitive_PropertyChanged );
					break;
				case CollectionChangeAction.Remove:
					primitive.PropertyChanged -= new SyncfusionPropertyChangedEventHandler( primitive_PropertyChanged );
					break;
			}
		}

		private void primitive_PropertyChanged(object sender, SyncfusionPropertyChangedEventArgs e)
		{
			if( e.PropertyName == "PrimitiveType" && lbPrimitives.SelectedIndex >= 0 )
			{
				lbPrimitives.Items[ lbPrimitives.SelectedIndex ] = e.NewValue.ToString();
				FillPrimitiveTypesList();
			}
		}
	}
}
