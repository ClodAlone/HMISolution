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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class MdiListMoreWindowsItem : StandAloneBarItem
	{
		// Fields
		public Form[] forms = null;
		public Form active = null;
    
		// Constructors
		public MdiListMoreWindowsItem(Form active, Form[] all)
		{
			this.forms = all;
			this.active = active;
		}    
    
		// Methods
    
		protected override void OnItemClicked(EventArgs args)
		{
			MdiWindowDialog mdiWindowDialog;
			if (this.forms != null)
			{
				try
				{
					mdiWindowDialog = new MdiWindowDialog();
					mdiWindowDialog.MaximizeBox = false;
					try
					{
						mdiWindowDialog.SetItems(this.active,this.forms);
						if (this.active.TopMost)
							mdiWindowDialog.TopMost = true;
						if(mdiWindowDialog.ShowDialog() == DialogResult.OK)
						{
							mdiWindowDialog.ActiveChildForm.Activate();
							if (mdiWindowDialog.ActiveChildForm.ActiveControl != null)
								if (!mdiWindowDialog.ActiveChildForm.ActiveControl.Focused)
									mdiWindowDialog.ActiveChildForm.ActiveControl.Focus();
						}
					}
					finally
					{
						if (mdiWindowDialog != null)
							mdiWindowDialog.Dispose();
					}
				}
				catch{}
				{
					//Trace.WriteLine("Error when opening the Windows list dialog: " + e.Message);
				}
			}
		}
	}

	/// <summary>
	/// Represents the item that will expand to show the list of MDI Children in an MDI environment.
	/// </summary>
	/// <remarks>
	/// You would typically use this item as a child in the Window submenu in the main menu bar 
	/// of a <see cref="MainFrameBarManager"/>.
	/// When its parent is dropped down, this item will be replaced by the list of MDI children
	/// currently being shown and optionally a "Windows..." item to show the whole list when
	/// the number of children is more than 10 (or as specified in the <see cref="MdiListSize"/> property.
	/// <para>When the user clicks on one of the entries, the corresponding mdi child will be activated.
	/// Clicking on the "Windows..." item will open a dialog where the users can see all the
	/// available MDI child windows.
	/// </para>
	/// </remarks>
	/// <example>
	/// Take a look at our XPMenus samples under the Tools\Samples\Menus Package\XPMenusMDI folder
	/// for usage example.
	/// </example>
	public class MdiListBarItem : ListBarItem
	{
		private bool moreWindowsAvailable = false;
		private BarItem moreWindowsItem = null;
		private ParentBarItem latestParent = null;
		private int mdiChildListSize = 10;

		public MdiListBarItem()
		{
			// Setup the too. Needed since the Text property is overridden here.
			this.ID = this.Text;
		}

		/// <summary>
		/// Gets / sets the number of mdi child links to be shown in the expanded list before the \"More Items\" BarItem.
		/// </summary>
		/// <value>Default is 10.</value>
		[
		Category("Behavior"),
		DefaultValue(10),
		Description("Specifies the number of mdi child links to be shown before the \"More Items\" BarItem.")
		]
		public virtual int MdiListSize
		{
			get{return this.mdiChildListSize;}
			set
			{
				this.mdiChildListSize = value;
			}
		}

		private const string DEF_TEXT = "Window List";

		private string m_strText = SR.GetString( SR.MdiListMenuItem ); 

		[ReadOnly(true)]
		public override string Text
		{
			get
			{
				return m_strText;
			}
			set
			{
				base.Text = value;
			}
		}

		protected internal override void OnBeforeExpand()
		{
			// Previously, manager's Form could ne not an MDI parent, so we resubscribe to BarManager's MDI related invents.
			UnsubscribeFromEvents( this.Manager );
			SubscribeForEvents( this.Manager );

			string localizedText = SR.GetString( SR.MdiListMenuItem,this );
			m_strText = ( localizedText  == null ) ? DEF_TEXT : m_strText;

			this.ChildCaptions.Clear();
			this.Tags.Clear();
			this.CheckedIndices.Clear();
			this.moreWindowsAvailable = false;

			if(this.DesignMode || this.barManager == null
				|| this.barManager.Form == null
				|| !this.barManager.Form.IsMdiContainer
				|| this.barManager.DesignMode
				|| this.barManager.Form.MdiChildren.Length == 0)
				return;

			Form mdiParent = this.barManager.Form;
			Form[] children = mdiParent.MdiChildren;
			int i = -1;
			foreach(Form form in children)
			{
				if(form.Visible == false || form is ITabHost || form is ISplitterHost)
					continue;

				i++;
				// Show only MdiListSize items.
				if(i < this.MdiListSize)
					this.ChildCaptions.Add(form.Text);

				this.Tags.Add(form);
				
				if(mdiParent.ActiveMdiChild == form)
					this.CheckedIndices.Add(i);

				if(i == (this.MdiListSize - 1))
				{
					this.moreWindowsAvailable = true;
				}
			}
			base.OnBeforeExpand();
		}

		protected internal override void PostExpand(ParentBarItem parent, int firstChild, int lastChild)
		{
			if(this.moreWindowsAvailable)
			{
				Form[] mdiChildren = new Form[this.Tags.Count];
				for(int i = 0; i < this.Tags.Count; i++)
					mdiChildren[i] = this.Tags[i] as Form;

				this.latestParent = parent;

				// Insert the "Windows..." item.
				this.moreWindowsItem = 
					new MdiListMoreWindowsItem(this.barManager.Form, mdiChildren);

				this.moreWindowsItem.Text = SR.GetString(SR.MoreWindowsCaptionInMenu);
				this.moreWindowsItem.IsRecentlyUsedItem = this.IsRecentlyUsedItem;
				parent.Items.Insert(lastChild + 1, moreWindowsItem);
				parent.PopupClosed += new EventHandler(this.PopupClosed);
			}

			base.PostExpand(parent, firstChild, lastChild);
		}

		private void PopupClosed(object sender, EventArgs e)
		{
			if(this.latestParent != null && this.moreWindowsItem != null)
			{
				this.latestParent.Items.Remove(this.moreWindowsItem);
				this.latestParent.PopupClosed -= new EventHandler(this.PopupClosed);
				this.latestParent = null;
				this.moreWindowsItem = null;
			}
		}
		[ReadOnly(true),
		]
		public override StringCollection ChildCaptions
		{
			get
			{
				return base.ChildCaptions;
			}

		}

		[ReadOnly(true)]
		public override ArrayList Tags
		{
			get
			{
				return base.Tags;
			}
		}

		public override BarManager Manager
		{
			set
			{
				if( value != base.Manager )
				{
					UnsubscribeFromEvents( base.Manager );
					SubscribeForEvents( value );
				}

				base.Manager = value;
			}
		}

		private void SubscribeForEvents( BarManager barMan )
		{
			if( null != barMan )
			{
				MdiClient mdiClient = MainFrameBarManager.GetMDIClient(barMan);

				if( null != mdiClient )
				{
					mdiClient.ControlRemoved += new ControlEventHandler( mdiClient_ControlRemoved );
				}
			}
		}

		private void UnsubscribeFromEvents( BarManager barMan )
		{
			if( null != barMan )
			{
				MdiClient mdiClient = MainFrameBarManager.GetMDIClient(barMan);

				if( null != mdiClient )
				{
					mdiClient.ControlRemoved -= new ControlEventHandler( mdiClient_ControlRemoved );
				}
			}
		}

		private void mdiClient_ControlRemoved( object sender, ControlEventArgs e )
		{
			Form mdiChild = e.Control as Form;

			if( null != mdiChild )
			{
				int i = this.Tags.IndexOf( mdiChild );

				if( i >= 0 )
				{
					this.Tags.RemoveAt( i );
				}
			}
		}

		protected override void OnItemClicked(EventArgs args)
		{
			if(args is ListBarItemClickedEventArgs)
			{
				ListBarItemClickedEventArgs lbArgs = args as ListBarItemClickedEventArgs;
				Form childForm = this.Tags[lbArgs.IndexClicked] as Form;
				if(childForm != null)
					childForm.Activate();
			}
			base.OnItemClicked(args);
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				this.Tags.Clear();
			}

			base.Dispose( disposing );
		}
	}
}
