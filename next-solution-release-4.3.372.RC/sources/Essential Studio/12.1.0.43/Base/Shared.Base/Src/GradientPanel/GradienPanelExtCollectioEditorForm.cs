#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Summary description for GradienPanelExtPrimitivesEditorForm.
	/// </summary>
	public class GradienPanelExtCollectioEditorForm : Form
	{
		#region Class Members

		/// <summary>
		/// Reference to owner control.
		/// </summary>
		private GradientPanelExt m_ownerControl = null;

		/// <summary>
		/// Collection of the primitives when added.
		/// </summary>
		private PrimitiveCollection m_arrAddedPrimitives = null;
		
		/// <summary>
		/// Collection of the primitives when removed.
		/// </summary>
		private PrimitiveCollection m_arrRemovedPrimitives = null;

		#endregion

		#region Form Controls

		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.ListBox lbPrimitives;
		private System.Windows.Forms.Button btAdd;
		private System.Windows.Forms.Button btRemove;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
        private GradienPanelExtPropertyGrid pgPrimitive;
		private System.Windows.Forms.ComboBox cbPrimitiveTypes;
		private System.Windows.Forms.Label label1;

		#endregion

		#region Class Initialize/Finalize methods

		private void InitializeComponent()
		{
			this.panelBottom = new System.Windows.Forms.Panel();
			this.cbPrimitiveTypes = new System.Windows.Forms.ComboBox();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.btRemove = new System.Windows.Forms.Button();
			this.btAdd = new System.Windows.Forms.Button();
			this.lbPrimitives = new System.Windows.Forms.ListBox();
			this.pgPrimitive = new GradienPanelExtPropertyGrid();
			this.label1 = new System.Windows.Forms.Label();
			this.panelBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.label1);
			this.panelBottom.Controls.Add(this.cbPrimitiveTypes);
			this.panelBottom.Controls.Add(this.btOK);
			this.panelBottom.Controls.Add(this.btCancel);
			this.panelBottom.Controls.Add(this.btRemove);
			this.panelBottom.Controls.Add(this.btAdd);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(0, 422);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(586, 40);
			this.panelBottom.TabIndex = 0;
			// 
			// cbPrimitiveTypes
			// 
			this.cbPrimitiveTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPrimitiveTypes.Location = new System.Drawing.Point(112, 8);
			this.cbPrimitiveTypes.Name = "cbPrimitiveTypes";
			this.cbPrimitiveTypes.Size = new System.Drawing.Size(136, 21);
			this.cbPrimitiveTypes.TabIndex = 4;
			// 
			// btOK
			// 
			this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btOK.Location = new System.Drawing.Point(426, 8);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 1;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(506, 8);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 0;
			this.btCancel.Text = "Cancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// btRemove
			// 
			this.btRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btRemove.Location = new System.Drawing.Point(336, 8);
			this.btRemove.Name = "btRemove";
			this.btRemove.TabIndex = 2;
			this.btRemove.Text = "Remove";
			this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
			// 
			// btAdd
			// 
			this.btAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btAdd.Location = new System.Drawing.Point(256, 8);
			this.btAdd.Name = "btAdd";
			this.btAdd.TabIndex = 3;
			this.btAdd.Text = "Add";
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			// 
			// lbPrimitives
			// 
			this.lbPrimitives.Dock = System.Windows.Forms.DockStyle.Left;
			this.lbPrimitives.Location = new System.Drawing.Point(0, 0);
			this.lbPrimitives.Name = "lbPrimitives";
			this.lbPrimitives.Size = new System.Drawing.Size(280, 420);
			this.lbPrimitives.TabIndex = 1;
			this.lbPrimitives.SelectedIndexChanged += new System.EventHandler(this.lbPrimitives_SelectedIndexChanged);
			// 
			// pgPrimitive
			// 
			this.pgPrimitive.CommandsVisibleIfAvailable = true;
			this.pgPrimitive.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgPrimitive.LargeButtons = false;
			this.pgPrimitive.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.pgPrimitive.Location = new System.Drawing.Point(280, 0);
			this.pgPrimitive.Name = "pgPrimitive";
			this.pgPrimitive.Size = new System.Drawing.Size(306, 422);
			this.pgPrimitive.TabIndex = 2;
			this.pgPrimitive.Text = "pgPrimitive";
			this.pgPrimitive.ViewBackColor = System.Drawing.SystemColors.Window;
			this.pgPrimitive.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.TabIndex = 5;
			this.label1.Text = "Types of primitive:";
			// 
			// GradienPanelExtCollectioEditorForm
			// 
			this.AcceptButton = this.btOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btCancel;
			this.ClientSize = new System.Drawing.Size(586, 462);
			this.Controls.Add(this.pgPrimitive);
			this.Controls.Add(this.lbPrimitives);
			this.Controls.Add(this.panelBottom);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MinimumSize = new System.Drawing.Size(592, 488);
			this.Name = "GradienPanelExtCollectioEditorForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "GradientPanelExt PrimitiveCollection Editor";
			this.panelBottom.ResumeLayout(false);
			this.ResumeLayout(false);

		}
	

		public GradienPanelExtCollectioEditorForm( GradientPanelExt ownerControl )
		{
			InitializeComponent();

			this.m_ownerControl = ownerControl;

			this.m_arrAddedPrimitives = new PrimitiveCollection();
			this.m_arrRemovedPrimitives = new PrimitiveCollection();

			this.m_arrAddedPrimitives.CollectionChanged += new CollectionChangeEventHandler( Primitives_CollectionChanged );

			FillTypesPrimitive();
			
			if( this.m_ownerControl != null && this.m_ownerControl.Primitives != null )
			{
				FillListBoxPrimitives( this.m_ownerControl.Primitives );
			}
		}

        public GradienPanelExtCollectioEditorForm( GradientPanelExt ownerControl, IServiceProvider provider ) : this( ownerControl )
        {
            pgPrimitive.Provider = provider;
        }

		#endregion

		#region Class Utility Methods

		/// <summary>
		/// Fills ListBox of primitives.
		/// </summary>
		private void FillListBoxPrimitives( PrimitiveCollection primitives )
		{
			if( primitives != null )
			{
				foreach( Primitive primitive in primitives )
				{
					lbPrimitives.Items.Add( primitive );
				}
			}
			
			if( lbPrimitives.Items.Count > 0 )
			{
				lbPrimitives.SelectedIndex = 0;
			}
		}

		/// <summary>
		/// Fills cbPrimitives of types prmitive.
		/// </summary>
		private void FillTypesPrimitive()
		{
			string[] primitiveNames = Enum.GetNames( typeof( PrimitiveTypes ) );
			cbPrimitiveTypes.Items.AddRange( primitiveNames );

			if( primitiveNames.Length > 0 )
			{
				cbPrimitiveTypes.SelectedIndex = 0;
			}
		}

		/// <summary>
		/// Creates primitive.
		/// </summary>
		private Primitive CreatePrimitive( PrimitiveTypes primitiveType )
		{
			Primitive primitive = null;

			IDesignerHost designerHost = this.m_ownerControl.GetIDesignerHost();

			if( designerHost != null )
			{
				switch( primitiveType )
				{
					case PrimitiveTypes.Collapse :
					{
						primitive = ( CollapsePrimitive )designerHost.CreateComponent( typeof( CollapsePrimitive ) );
						break;
					}
					case PrimitiveTypes.Image :
					{
						primitive = ( ImagePrimitive )designerHost.CreateComponent( typeof( ImagePrimitive ) );
						break;
					}
					case PrimitiveTypes.Text :
					{
						primitive = ( TextPrimitive )designerHost.CreateComponent( typeof( TextPrimitive ) );
						break;
					}
					case PrimitiveTypes.Host : 
					{
						primitive = ( HostPrimitive )designerHost.CreateComponent( typeof( HostPrimitive ) );
						break;
					}
				}
			}

			return primitive;
		}

		#endregion 

		#region Class Event Handlers

		private void lbPrimitives_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			Primitive primitive = lbPrimitives.SelectedItem as Primitive;

			if( primitive != null )
			{
				pgPrimitive.SelectedObject = primitive;
			}
		}

		private void btAdd_Click( object sender, System.EventArgs e )
		{
			Primitive primitive = null;

			if( cbPrimitiveTypes.SelectedIndex > -1 )
			{
				PrimitiveTypes primitiveType = ( PrimitiveTypes )cbPrimitiveTypes.SelectedIndex;
		
				primitive = CreatePrimitive( primitiveType );

				if( primitive != null )
				{
					this.m_arrAddedPrimitives.Add( primitive );
				}
			}
		}

		private void Primitives_CollectionChanged( object sender, CollectionChangeEventArgs e )
		{
			Primitive primitive = e.Element as Primitive;

			if( primitive != null )
			{
				if( e.Action == CollectionChangeAction.Add )
				{
					this.m_ownerControl.Primitives.Add( primitive );
					lbPrimitives.Items.Add( primitive );
					lbPrimitives.SelectedItem = primitive;
				}
			}
		}

		private void btRemove_Click(object sender, System.EventArgs e)
		{
			Primitive primitive = lbPrimitives.SelectedItem as Primitive;

			this.m_arrRemovedPrimitives.Add( primitive );

			this.lbPrimitives.Items.Remove( primitive );

			if( lbPrimitives.Items.Count > 0 )
			{
				lbPrimitives.SelectedIndex = lbPrimitives.Items.Count - 1;
			}
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			if( this.m_arrRemovedPrimitives != null && this.m_arrRemovedPrimitives.Count > 0
				&& this.m_ownerControl != null )
			{
				IDesignerHost designerHost = this.m_ownerControl.GetIDesignerHost();

				if( designerHost != null )
				{
					foreach( Primitive primitive in this.m_arrRemovedPrimitives )
					{
						designerHost.DestroyComponent( primitive );
						this.m_ownerControl.Primitives.Remove( primitive );
					}
				}
			}

			this.m_ownerControl.Invalidate();
			Close();
		}

		private void btCancel_Click(object sender, System.EventArgs e)
		{
			IDesignerHost designerHost = this.m_ownerControl.GetIDesignerHost();

			if( this.m_arrAddedPrimitives != null && this.m_arrAddedPrimitives.Count > 0
				&& designerHost != null )
			{

				foreach( Primitive primitive in this.m_arrAddedPrimitives )
				{
					designerHost.DestroyComponent( primitive );

					this.m_ownerControl.Primitives.Remove( primitive );
				}
			}

			this.m_ownerControl.Invalidate();
		}

		#endregion
	}
}
