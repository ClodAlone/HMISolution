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
using System.Diagnostics;

using Syncfusion.Windows.Forms.Tools.XPMenus;

namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DockingWrapperForm : Office2007Form, ITabbedMDIChildForm
	{
		protected DockingManager dockingManager;
		public Control ctrlChildRef = null;
		public BarItem dockable, floating, hideItem, autohideitem, mdichilditem;
		protected TabbedMDIManager tabbedMDIManager;
		protected bool originalCloseItemVisibility = true;
		public bool bInCloseProc = false;
		protected bool bClosing = false;

		public DockingWrapperForm(DockingManager dockingmanager, Control dockcontrol)
		{
			this.dockingManager = dockingmanager;
			this.DisableOffice2007Style = !this.dockingManager.Office2007MdiChildForm;
			this.ctrlChildRef = dockcontrol;
			dockcontrol.GotFocus += new EventHandler( dockcontrol_GotFocus );
			dockcontrol.LostFocus += new EventHandler( dockcontrol_LostFocus );

			this.dockable = new BarItem(SR.GetString(SR.DockableMenuItemText, dockcontrol));
			this.dockable.Tag = this;
			this.hideItem = new BarItem(SR.GetString(SR.HideMenuItemText, dockcontrol));
			this.hideItem.Tag = this;
			if(this.dockingManager.DisallowFloating == false && this.dockingManager.GetAllowFloating(dockcontrol))
			{
				this.floating = new BarItem(SR.GetString(SR.FloatingMenuItemText, dockcontrol));
				this.floating.Tag = this;
			}
			this.mdichilditem = new BarItem(SR.GetString(SR.MDIChildMenuItemText, dockcontrol));
			this.autohideitem = new BarItem(SR.GetString(SR.AutoHideMenuItemText, dockcontrol));

			this.ClientSize = new System.Drawing.Size(552, 301);

			if(dockcontrol.Name != String.Empty)
				this.Name = String.Concat("DockingWrapperForm_", dockcontrol.Name);

		//To fix the issue for Menu merging with the Standard MS Menu control with DockingWrapperForm
			if( dockcontrol is Form )
			{
				Form f = dockcontrol as Form;
				this.Menu = f.Menu;
				this.ControlBox = f.ControlBox;
			}
		}

		private void dockcontrol_LostFocus( object sender, EventArgs e )
		{
			if( dockingManager.ActiveControl == this.ctrlChildRef )
				this.dockingManager.SetActiveControl( null );
		}

		private void dockcontrol_GotFocus( object sender, EventArgs e )
		{
			SetDMActiveControl();
		}

		void ITabbedMDIChildForm.OnAttachTabbedMDI(TabbedMDIManager manager)
		{
			this.tabbedMDIManager = manager;
		}

		void ITabbedMDIChildForm.OnDetachTabbedMDI(TabbedMDIManager manager)
		{
			this.tabbedMDIManager = null;
		}

		void ITabbedMDIChildForm.OnTabContextMenuPopup(ParentBarItem contextMenuParentItem)
		{
			// Insert the Dockable, Hide and Floating menus.
			if(this.dockingManager != null && this.dockingManager.EnableContextMenu )
			{
				foreach (BarItem item in contextMenuParentItem.Items)
				{
					if (item == this.tabbedMDIManager.CloseItem)
					{
						this.originalCloseItemVisibility = item.Visible;
						item.Visible = false;
					}
				}
				if (this.floating != null && this.dockingManager.DisallowFloating == false
					&& this.dockingManager.GetAllowFloating(ctrlChildRef))
					contextMenuParentItem.Items.Insert(0, this.floating);
				contextMenuParentItem.Items.Insert(0, this.hideItem);
				contextMenuParentItem.Items.Insert(0, this.dockable);
				PopupMenu menu = new PopupMenu();
				menu.ParentBarItem = contextMenuParentItem;
				menu.ParentBarItem.Style = this.dockingManager.VisualStyle;
				DockContextMenuEventArgs dcmeargs = new DockContextMenuEventArgs(this.ctrlChildRef, menu);
				this.dockingManager.OnDockContextMenu(dcmeargs);
				menu.ParentBarItem = null;
			}
		}

		void ITabbedMDIChildForm.OnTabContextMenuClosed(ParentBarItem contextMenuParentItem)
		{
			if(this.dockingManager != null)
			{
				foreach(BarItem item in contextMenuParentItem.Items)
				{
					if(item == this.tabbedMDIManager.CloseItem)
						item.Visible = this.originalCloseItemVisibility;
				}
				if(this.floating != null)
					contextMenuParentItem.Items.Remove(this.floating);
				contextMenuParentItem.Items.Remove(this.hideItem);
				contextMenuParentItem.Items.Remove(this.dockable);
			}
		}

		bool ITabbedMDIChildForm.AllowUserDrag
		{
			get{return true;}
		}

		void ITabbedMDIChildForm.OnMdiChildAddedToTabHost(TabHost tabHost, int tabIndex)
		{
		}

		string ITabbedMDIChildForm.GetCustomTabText(out bool validValueReturned)
		{
			validValueReturned = false;
			return null;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
            if (this.Controls != null && this.Controls.Count > 0
                && this.Controls[0] != null && this.dockingManager != null)
            {
                DockVisibilityChangingEventArgs args = new DockVisibilityChangingEventArgs(this.Controls[0]);
                this.dockingManager.FireDockVisibilityChangingEvent(args);
                this.dockingManager.bFiredVisibilityChangingEvent = !this.dockingManager.HoldEvents;
                if (args.Cancel)
                {
                    this.dockingManager.bCancelVisibilityChangingEvent = true;
                    this.dockingManager.bFiredVisibilityChangingEvent = false;
                    if (this.dockingManager.bHostFormClosing == false)
                        e.Cancel = true;
                }
                else
                {
                    this.dockingManager.bCancelVisibilityChangingEvent = false;
                }

                if (bClosing)
                {
                    if (this.dockingManager.bHostFormClosing == false)
                        e.Cancel = args.Cancel;
                    this.dockingManager.bCancelVisibilityChangingEvent |= e.Cancel;
                    bClosing = false;
                }
            }
			base.OnClosing (e);
		}

		protected override void OnClosed(EventArgs e)
		{
			if( dockingManager.bCancelVisibilityChangingEvent == false )
			{
				this.dockingManager.SaveMdiZOrder();

				// OnClosed is called only when the child form is closed and not when the app exits.
				// Update the control's DockVisibility state so that the DockingManager is aware of this.
				if( this.ctrlChildRef != null )
				{
					dockingManager.bMdiFormClosing = true;
					this.bInCloseProc = true;
					DockHostController dhc = this.dockingManager.GetDockHostController( this.ctrlChildRef );
					if( dhc != null )
						dhc.Closing = true;
					this.dockingManager.SetDockVisibility(this.ctrlChildRef, false);
					if( dhc != null )
						dhc.Closing = false;
					dockingManager.bMdiFormClosing = false;
				}
			}
			base.OnClosed(e);
		}

		protected override void OnGotFocus( EventArgs e )
		{
			if (Controls.Count > 0)
			{
				dockingManager.SetActiveControl(Controls[0]);
			}
			base.OnGotFocus( e );
		}

		protected override void WndProc(ref Message msg)
		{
			switch( msg.Msg )
			{
				case 0x0021:/*WM_MOUSEACTIVATE*/
					if( Controls.Count > 0 && dockingManager.ActiveControl != Controls[0] && 
						Controls[0] != null )
					{
						SetDMActiveControl();
						Controls[0].Focus();
					}
					break;
				case 0x00A5:/*WM_NCRBUTTONUP*/ 
					if((int)msg.WParam == 2/*HTCAPTION*/)
					{
						if(this.dockingManager != null && this.dockingManager.EnableContextMenu)
						{
							InitializeMenu(msg);
						}
					}
					break;
				case 0x0112: /*WM_SYSCOMMAND*/
					if( msg.WParam.ToInt32() == 0xf020 /*SC_Minimize*/ )
					{
						foreach( FloatingForm ff in this.OwnedForms )
						{
							if( ff.Visible )
								ff.Visible = false;
						}
					}
					if( msg.WParam.ToInt32() == 0xf120 /*SC_Restore*/ )
					{
						foreach( FloatingForm ff in this.OwnedForms )
						{
							if( !ff.Visible )
								ff.Visible = true;
						}
					}
					if( msg.WParam.ToInt32() == 0xf060 /*SC_Close*/ )
					{
						bClosing = true;
					}
					break;
			}
			base.WndProc(ref msg);
		}

		protected void InitializeMenu( Message msg )
		{
			int ptint = (int)msg.LParam;
			InitializeMenu( this.PointToClient(new Point(Syncfusion.Runtime.InteropServices.NativeMethods.LOWORD(ptint), Syncfusion.Runtime.InteropServices.NativeMethods.HIWORD(ptint))));
		}

		protected internal void InitializeMenu()
		{
			InitializeMenu(Point.Empty);
		}

		protected internal void InitializeMenu( Point ptclient )
		{
			PopupMenu menu = new PopupMenu();
			menu.ParentBarItem = new ParentBarItem();
			menu.ParentBarItem.Style = dockingManager.VisualStyle;
			menu.ParentBarItem.Items.Add(this.dockable);
			if( this.dockingManager.EnableContextMenu == true )
			{
				menu.ParentBarItem.Items.Add(this.hideItem);

				if( this.floating != null 
					&& this.dockingManager.GetAllowFloating( this.Controls[0] ) )
				{
					if( dockingManager.MenuStyle == DockMenuStyle.VS2003 )
					{
						menu.ParentBarItem.Items.Add(this.floating);
					}
					else
					{
						menu.ParentBarItem.Items.Insert(0, this.floating);
					}
				}
			}
			if( dockingManager.MenuStyle == DockMenuStyle.VS2005 )
			{
				menu.ParentBarItem.Items.Insert( menu.ParentBarItem.Items.Count, this.mdichilditem);
				menu.ParentBarItem.Items.Insert( menu.ParentBarItem.Items.Count, this.autohideitem);
				mdichilditem.Checked = true;
				autohideitem.Enabled = false;
			}
			DockContextMenuEventArgs dcmeargs = new DockContextMenuEventArgs(this.ctrlChildRef, menu);
			this.dockingManager.OnDockContextMenu(dcmeargs);
			menu.Show(this, ptclient );
		}

		private void SetDMActiveControl()
		{
			this.dockingManager.DHCInFocus = null;

			if( this.Controls.Count > 0 )
			{
				this.dockingManager.mouseActivatedControl = null;
				this.dockingManager.SetActiveControl( Controls[0] );
			}
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( this.ctrlChildRef != null )
				{
					this.ctrlChildRef.GotFocus -= new EventHandler( dockcontrol_GotFocus );
					this.ctrlChildRef.LostFocus -= new EventHandler( dockcontrol_LostFocus );
				}
				this.ctrlChildRef = null;
				this.dockingManager = null;
			}
			base.Dispose( disposing );
		}
	}
}
