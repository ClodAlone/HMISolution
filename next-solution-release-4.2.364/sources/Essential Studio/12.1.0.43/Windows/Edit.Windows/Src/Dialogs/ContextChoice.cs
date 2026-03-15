#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Edit;
using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Interfaces;

namespace Syncfusion.Windows.Forms.Edit.Forms.Popup
{
	/// <summary>
	/// Form used for context choice functionality.
	/// </summary>
	internal class ContextChoice
		: Syncfusion.Windows.Forms.Edit.Forms.Popup.BasePopupForm
	{
		#region Constants
		/// <summary>
		/// 
		/// </summary>
		private const int DEF_PAGE_LINES = 8;
		#endregion

		#region Fields
		private TreeNode m_nodeLastWithTip;
		/// <summary>
		/// Contextchoice items collection.
		/// </summary>
		private ContextChoiceItemCollection m_items;
		/// <summary>
		/// Named image collection.
		/// </summary>
		private NamedImageList m_images;
		/// <summary>
		/// Hashtable of the context choice items.
		/// </summary>
		private Hashtable m_hashItems = new Hashtable();
		/// <summary>
		/// Parent control that receives focus after item is selected.
		/// </summary>
		private StreamEditControl m_parent = null;
		/// <summary>
		/// Indicates whether double click must close the form.
		/// </summary>
		private bool m_bProcessDblClick = true;
		/// <summary>
		/// 
		/// </summary>
		private ToolTipEx m_toolTip;
		/// <summary>
		/// 
		/// </summary>
		private Timer m_timer;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets selected item.
		/// </summary>
		protected ContextChoiceItem SelectedItemInternal
		{
			get
			{
				return lstItems.SelectedNode as ContextChoiceItem;
			}
			set
			{
				if( lstItems.SelectedNode != value )
				{
					lstItems.SelectedNode = value;
					if( null != value )
					{
						lstItems.SelectedNode.EnsureVisible();
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets ID of currently selected item.
		/// </summary>
		public int SelectedItemID
		{
			get
			{
				ContextChoiceItem item = SelectedItemInternal;
				int id = ( null != item ) ? ( item.ID ) : ( -1 );
				return id;
			}
			set
			{
				if( SelectedItemID != value )
				{
					if( !m_hashItems.Contains( value ) )
						throw new ArgumentException( "Given item does not belong to the visible context choice items list.", "SelectedItemID" );

					ContextChoiceItem item = ( ContextChoiceItem )m_hashItems[ value ];
					SelectedItemInternal = item;
				}
			}
		}
		/// <summary>
		/// Gets collection of items.
		/// </summary>
		public ICollection ContextChoiceList
		{
			get
			{
				return lstItems.Nodes;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether double click must close the form.
		/// </summary>
		public bool ProcessDblClick
		{
			get
			{
				return m_bProcessDblClick;
			}
			set
			{
				m_bProcessDblClick = value;
			}
		}
		#endregion

		#region Nonpublic Properties
		/// <summary>
		/// Gets control representing items.
		/// </summary>
		internal Control ItemsView
		{
			get
			{
				return lstItems;
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Event, that is raised when some item gets selected.
		/// </summary>
		public event ContextChoiceItemSelectedEventHandler ItemSelected;
		#endregion

		#region Form Controls
		private System.Windows.Forms.ImageList imgList;
		private System.Windows.Forms.TreeView lstItems;
		private System.ComponentModel.IContainer components;
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates and initializes context choice form.
		/// </summary>
		/// <param name="images">List of images for context choice items.</param>
		/// <param name="items">Collection of context choice items.</param>
		/// <param name="editor">Underlying StreamEditControl.</param>
		/// <param name="inactive">Indicates whether popup form should stay inactive.</param>
		/// <param name="parentWindow">Parent control that receives focus after item is selected.</param>
		/// <param name="bCloseOnParentClick">Indicates whether form should be closed on click above parent window.</param>
		/// <param name="bCatchParentActivate">Indicates whether activation of parent window should be caught.</param>
		public ContextChoice( NamedImageList images, ContextChoiceItemCollection items,
			StreamEditControl editor, bool inactive, Control parentWindow, bool bCloseOnParentClick, bool bCatchParentActivate )
			: base( parentWindow, true, 0.2f, inactive, bCloseOnParentClick, bCatchParentActivate )
		{
			if( null == items ) throw new ArgumentNullException( "items" );
			if( null == images ) throw new ArgumentNullException( "images" );
			if( null == editor ) throw new ArgumentNullException( "parent" );

			m_items = items;
			m_items.CollectionChanged += new EventHandler( m_items_CollectionChanged );
			m_images = images;
			m_parent = editor;

			InitializeComponent();

			images.WriteToImageList( imgList );
			lstItems.Nodes.Clear();
			m_toolTip = new ToolTipEx( this, false );

			m_timer = new Timer();
			m_timer.Interval = 300;
			m_timer.Tick += new EventHandler( OnTooltipTimerTick );
		}
		/// <summary>
		/// Creates and initializes new instance of class.
		/// </summary>
		/// <param name="images">List of images for context choice items.</param>
		/// <param name="items">Collection of context choice items.</param>
		/// <param name="editor">Underlying StreamEditControl.</param>
		/// <param name="inactive">Indicates whether popup form should stay inactive.</param>
		/// <param name="parentWindow">Parent control that receives focus after item is selected.</param>
		/// <param name="bCloseOnParentClick">Indicates whether form should be closed on click above parent window.</param>
		/// <param name="bCatchParentActivate">Indicates whether activation of parent window should be caught.</param>
		/// <param name="size">Size of the form.</param>
		public ContextChoice( NamedImageList images, ContextChoiceItemCollection items,
			StreamEditControl editor, bool inactive, Control parentWindow, bool bCloseOnParentClick, bool bCatchParentActivate, Size size )
			: this( images, items, editor, inactive, parentWindow, bCloseOnParentClick, bCatchParentActivate )
		{
			this.Size = size;
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
				{
					components.Dispose();
				}

				if( null != m_images && null != imgList )
				{
					m_images.RemoveImageListInfo( imgList );
					m_images = null;
				}

				m_items = null;

				m_timer.Dispose();
			}
			base.Dispose( disposing );
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager( typeof( ContextChoice ) );
			this.imgList = new System.Windows.Forms.ImageList( this.components );
			this.lstItems = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// imgList
			// 
			this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imgList.ImageSize = new System.Drawing.Size( 16, 16 );
			this.imgList.ImageStream = ( ( System.Windows.Forms.ImageListStreamer )( resources.GetObject( "imgList.ImageStream" ) ) );
			this.imgList.TransparentColor = System.Drawing.Color.Magenta;
			// 
			// lstItems
			// 
			this.lstItems.BackColor = System.Drawing.SystemColors.Window;
			this.lstItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstItems.FullRowSelect = true;
			this.lstItems.HideSelection = false;
			this.lstItems.ImageList = this.imgList;
			this.lstItems.Location = new System.Drawing.Point( 3, 3 );
			this.lstItems.Name = "lstItems";
			this.lstItems.Nodes.AddRange( new System.Windows.Forms.TreeNode[] {
																																				 new System.Windows.Forms.TreeNode("Node0"),
																																				 new System.Windows.Forms.TreeNode("Node1"),
																																				 new System.Windows.Forms.TreeNode("Node10"),
																																				 new System.Windows.Forms.TreeNode("Node2"),
																																				 new System.Windows.Forms.TreeNode("Node3"),
																																				 new System.Windows.Forms.TreeNode("Node4"),
																																				 new System.Windows.Forms.TreeNode("Node5"),
																																				 new System.Windows.Forms.TreeNode("Node6"),
																																				 new System.Windows.Forms.TreeNode("Node7"),
																																				 new System.Windows.Forms.TreeNode("Node8"),
																																				 new System.Windows.Forms.TreeNode("Node9")} );
			this.lstItems.ShowLines = false;
			this.lstItems.ShowPlusMinus = false;
			this.lstItems.ShowRootLines = false;
			this.lstItems.Size = new System.Drawing.Size( 170, 82 );
			this.lstItems.TabIndex = 1;
			this.lstItems.KeyDown += new System.Windows.Forms.KeyEventHandler( this.lstItems_KeyDown );
			this.lstItems.DoubleClick += new System.EventHandler( this.lstItems_DoubleClick );
			this.lstItems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler( this.lstItems_AfterSelect );
			this.lstItems.MouseMove += new System.Windows.Forms.MouseEventHandler( this.lstItems_MouseMove );
			// 
			// frmContextChoice
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size( 176, 88 );
			this.ControlBox = false;
			this.Controls.Add( this.lstItems );
			this.DockPadding.All = 3;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmContextChoice";
			this.RightToLeftLayout = true;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Auto Complete Items";
			this.Paint += new System.Windows.Forms.PaintEventHandler( this.frmContextChoice_Paint );
			this.ResumeLayout( false );
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Updates nodes list.
		/// </summary>
		/// <param name="stringToAutoComplete">String with the beginning of word to auto complete.</param>
		public void UpdateNodesList( string stringToAutoComplete )
		{
			if( m_items == null ) throw new NullReferenceException( "Context choice items collection is null." );

			m_hashItems.Clear();
			lstItems.BeginUpdate();
			TreeNode nodeSelected = null;
			bool bStringCompleted = false;
			try
			{
				TreeNodeCollection nodes = lstItems.Nodes;
				nodes.Clear();

				foreach( ContextChoiceItem item in m_items )
				{
					if( item.Visible )
					{
						ContextChoiceItem itemNew = ( ContextChoiceItem )item.Clone();
						itemNew.ImageIndex = itemNew.SelectedImageIndex = m_images.GetIndex( itemNew.Image, imgList );
						if( !bStringCompleted && itemNew.Text.ToLower().StartsWith( stringToAutoComplete ) )
						{
							nodeSelected = itemNew;
							bStringCompleted = true;
						}

						nodes.Add( itemNew );
						m_hashItems[ itemNew.ID ] = itemNew;
					}
				}
			}
			finally
			{
				lstItems.EndUpdate();
			}
			if (nodeSelected == null)
				m_toolTip.ToolTipText = string .Empty ;

			lstItems.SelectedNode = nodeSelected;
		}
		/// <summary>
		/// Selects next item.
		/// </summary>
		public void NextItem()
		{
			if( CheckSelection() )
			{
				int item = lstItems.SelectedNode.Index;
				int count = lstItems.Nodes.Count - 1;
				if( item < 0 )
				{
					item = 0;
				}
				if( item > count )
				{
					item = count;
				}

				if( item < count )
				{
					lstItems.SelectedNode = lstItems.Nodes[ lstItems.SelectedNode.Index + 1 ];
					lstItems.SelectedNode.EnsureVisible();
				}
			}
		}
		/// <summary>
		/// Selects previous item.
		/// </summary>
		public void PreviousItem()
		{
			if( CheckSelection() )
			{
				int item = lstItems.SelectedNode.Index;
				if( item > 0 )
				{
					lstItems.SelectedNode = lstItems.Nodes[ lstItems.SelectedNode.Index - 1 ];
				}
			}
		}
		/// <summary>
		/// Jumps to the next page.
		/// </summary>
		public void NextPage()
		{
			if( lstItems.SelectedNode != null )
			{
				lstItems.SelectedNode = lstItems.Nodes[ Math.Min( lstItems.SelectedNode.Index + DEF_PAGE_LINES, lstItems.Nodes.Count - 1 ) ];
			}
		}
		/// <summary>
		/// Jumps to the previous page.
		/// </summary>
		public void PreviousPage()
		{
			if( lstItems.SelectedNode != null )
			{
				lstItems.SelectedNode = lstItems.Nodes[ Math.Max( lstItems.SelectedNode.Index - DEF_PAGE_LINES, 0 ) ];
			}
		}
		/// <summary>
		/// Closes form with Cancel dialog result.
		/// </summary>
		public void Dismiss()
		{
			CloseCancel();
		}
		/// <summary>
		/// Locks update of the control.
		/// </summary>
		public void BeginUpdate()
		{
			lstItems.BeginUpdate();
		}
		/// <summary>
		/// Unlocks update of the control.
		/// </summary>
		public void EndUpdate()
		{
			lstItems.EndUpdate();
			CheckSelection();
		}
		/// <summary>
		/// Cancels selection and closes form.
		/// </summary>
		public new void Close()
		{
			CloseCancel();
		}
		#endregion

		#region Class Utility methods
		/// <summary>
		/// Checks if control has some item selected.
		/// </summary>
		/// <returns></returns>
		protected internal bool CheckSelection()
		{
			if( lstItems.SelectedNode == null )
			{
				if( lstItems.Nodes.Count == 0 ) return false;

				lstItems.SelectedNode = lstItems.Nodes[ 0 ];
			}

			return true;
		}
		/// <summary>
		/// Closes dialog with DialogResult property set to OK.
		/// </summary>
		protected internal void CloseOk()
		{
			base.DialogResult = ( SelectedItemInternal != null ) ? ( DialogResult.OK ) : ( DialogResult.Cancel );
			m_toolTip.Close();
			base.Close();
		}
		/// <summary>
		/// Closes dialog with DialogResult property set to Cancel.
		/// </summary>
		protected internal void CloseCancel()
		{
			base.DialogResult = DialogResult.Cancel;
			base.Close();
		}
		/// <summary>
		/// Sets active tooltip`s text.
		/// </summary>
		/// <param name="item">Item with text to be set.</param>
		private void SetItemToolTip( ContextChoiceItem item )
		{
			m_timer.Stop();

			if( item != null )
			{
				m_toolTip.Hide();
				m_toolTip.UseXPStyleBorder= ((StreamEditControl)m_parent).UseXPStyleBorder;
				m_toolTip.ToolTipText = item.ToolTip;
				m_toolTip.Location = PointToScreen( new Point( this.Width, item.Bounds.Top ) );
				m_timer.Start();
			}
		}
		/// <summary>
		/// Sets active tooltip's text.
		/// </summary>
		/// <param name="item">Item with text to be set.</param>
		private void SetNodeToolTip( TreeNode item )
		{
			SetItemToolTip( ( ContextChoiceItem )item );
		}
		#endregion

		#region Keyboard Handling
		/// <summary>
		/// 
		/// </summary>
		/// <param name="keys"></param>
		internal void ProccessKeys( KeyEventArgs keys )
		{
			switch( keys.KeyData )
			{
				case Keys.Up:
					PreviousItem();
					keys.Handled = true;
					break;

				case Keys.Down:
					NextItem();
					keys.Handled = true;
					break;

				case Keys.PageUp:
					PreviousPage();
					keys.Handled = true;
					break;

				case Keys.PageDown:
					NextPage();
					keys.Handled = true;
					break;

				case Keys.Enter:
				case Keys.Tab:
				case Keys.Control | Keys.Space:
				case Keys.Alt | Keys.Right:
					CloseOk();
					keys.Handled = true;
					break;

				case Keys.Escape:
					CloseCancel();
					keys.Handled = true;
					break;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void frmContextChoice_Paint( object sender, System.Windows.Forms.PaintEventArgs e )
		{
			Rectangle rect = new Rectangle( 0, 0, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1 );
			if( ( ( StreamEditControl )m_parent ).UseXPStyle )
			{
				this.DockPadding.All = 3;
				if (((StreamEditControl)m_parent).UseXPStyleBorder)
					GraphicsUtils.Draw3DBorder( e.Graphics, rect );
			}
			else
			{
				this.DockPadding.All = 1;
				e.Graphics.DrawRectangle( m_borderPen, rect );
			}
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Updates tooltip with information about currently selected item.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lstItems_AfterSelect( object sender, System.Windows.Forms.TreeViewEventArgs e )
		{
			SetNodeToolTip( e.Node );

			ContextChoiceItem item = e.Node as ContextChoiceItem;

			if( item != null )
			{
				if( ItemSelected != null )
				{
					ContextChoiceItemSelectedEventArgs args =
						new ContextChoiceItemSelectedEventArgs( item );

					ItemSelected( null, args );
				}
			}
		}
		/// <summary>
		/// Updates tooltip.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lstItems_MouseMove( object sender, System.Windows.Forms.MouseEventArgs e )
		{
			TreeNode node = lstItems.GetNodeAt( e.X, e.Y );

			if( m_nodeLastWithTip != node )
			{
				m_nodeLastWithTip = node;
				SetNodeToolTip( node );
                if (node!=null && CheckSelection())
                {
                    lstItems.SelectedNode = node;
                    lstItems.SelectedNode.EnsureVisible();
                }
			}
		}
		/// <summary>
		/// Closes dialog with modal result OK.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lstItems_DoubleClick( object sender, System.EventArgs e )
		{
			// do not make direct call - this will raise exception: An unhandled 
			// exception of type 'System.ObjectDisposedException' occurred in 
			// system.windows.forms.dll; Cannot access a disposed object named 
			// "ListBox".
			if( m_bProcessDblClick )
			{
				base.BeginInvoke( new MethodInvoker( CloseOk ) );
			}
		}
		/// <summary>
		/// Processes key presses.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lstItems_KeyDown( object sender, System.Windows.Forms.KeyEventArgs e )
		{
			ProccessKeys( e );
		}
		/// <summary>
		/// Updates nodes list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_items_CollectionChanged( object sender, EventArgs e )
		{
			if( m_items != null ) UpdateNodesList( string.Empty );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTooltipTimerTick( object sender, EventArgs e )
		{
			if( !this.IsDisposed )
			{
				if( m_toolTip.ToolTipText != string.Empty )
				{
					m_toolTip.ShowToolTip();
				}
				m_timer.Stop();
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClosing( CancelEventArgs e )
		{
			base.OnClosing( e );
			m_timer.Stop();
		}

		#endregion
	}
}