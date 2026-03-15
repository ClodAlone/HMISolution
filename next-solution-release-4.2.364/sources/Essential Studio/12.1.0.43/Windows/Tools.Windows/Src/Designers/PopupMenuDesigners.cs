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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools.XPMenus;
using Syncfusion.Win32;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	internal class PopupDesignerForm : Form
	{
		internal PopupMenu popup;
		private System.Windows.Forms.Button toggle;
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;
//		private bool formMoved = false;
		public PopupDesignerForm()
		{
			this.InitializeComponent();
			this.SetTopLevel(true);
//			this.Owner = Form.ActiveForm;
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			this.popup.DesignTimeForm = null;
			if(this.popup != null
				&& this.popup.IsShowing())
			{
				if(this.popup.ParentBarItem.Manager != null)
					this.popup.ParentBarItem.Manager.CustomizationDone
						-= new EventHandler(this.Customization_Done);
				this.popup.Hide();
//				this.UpdateToggleButtonImage();
			}

			e.Cancel = true;
			this.Hide();
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(this.GetType());
			this.toggle = new System.Windows.Forms.Button();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// toggle
			// 
			this.toggle.AllowDrop = true;
			this.toggle.Dock = System.Windows.Forms.DockStyle.Top;
			this.toggle.ImageList = this.imageList1;
			this.toggle.Name = "toggle";
			this.toggle.Size = new System.Drawing.Size(192, 17);
			this.toggle.TabIndex = 0;
			this.toggle.Click += new System.EventHandler(this.toggle_Click);
			this.toggle.DragOver += new System.Windows.Forms.DragEventHandler(this.PopupDesigner_DragOver);
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// PopupDesignerForm
			// 
			this.AllowDrop = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CausesValidation = false;
			this.ClientSize = new System.Drawing.Size(192, 16);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.toggle});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(198, 38);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(198, 38);
			this.Name = "PopupDesignerForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Popup Designer: Click Here";
			this.TopMost = true;
			this.DragOver += new System.Windows.Forms.DragEventHandler(this.PopupDesigner_DragOver);
			this.VisibleChanged += new System.EventHandler(this.PopupDesignerForm_VisibleChanged);
			this.ResumeLayout(false);

		}
	
//		[EditorBrowsable(EditorBrowsableState.Advanced)] 
//		protected override /*ContainerControl*/ void WndProc(ref Message m)
//		{
//			//TODO: Toggle Popup visibility here based on NC button up or down
//			if(m.Msg == 0xa1/*WM_NCLBUTTONDOWN*/)
//			{
////				base.WndProc(ref m);
//				if(this.TogglePopupVisibility())
//					return;
////				return;
//			}
////			else if(m.Msg == 0x00A2/*WM_NCLBUTTONUP*/)
////			{
////				if(this.formMoved)
////					this.popup.Show(this, new Point(1, 1));
////				this.formMoved = false;
////			}
//
//			base.WndProc(ref m);
//		}
//		protected override void OnMove(EventArgs e)
//		{
//			base.OnMove(e);
//			this.formMoved = true;
//			if(this.popup.IsShowing())
//			{
//				this.popup.Hide();
//				this.UpdateToggleButtonImage();
//			}
//		}
		private void Customization_Done(object sender, EventArgs e)
		{
			this.OnClosing(new CancelEventArgs());
		}
		internal void Customize(PopupMenu popup)
		{
			popup.DesignTimeForm = this;
			this.popup = popup;
			if(this.popup.ParentBarItem.Manager != null)
			{
				this.popup.ParentBarItem.Manager.CustomizationDone
					+= new EventHandler(this.Customization_Done);
			}

			this.Location = new Point(100, 100);

			this.Visible = true;
			this.Show();
			this.TogglePopupVisibility();
		}

		// Toggle Popup visibility
		private void TogglePopupVisibility()
		{
			if(this.popup.IsShowing())
				this.popup.Hide();
			else
			{
				this.popup.ParentBarItem.PopupClosed += new EventHandler(this.PopupClosed_Event);
				this.SetToggleButtonImage(false);
				this.popup.Show(this.toggle, new Point(1, this.toggle.Height + 1));
			}
		}

		private void PopupClosed_Event(object sender, EventArgs e)
		{
			this.popup.ParentBarItem.PopupClosed -= new EventHandler(this.PopupClosed_Event);
			this.UpdateToggleButtonImage();
		}

		private void UpdateToggleButtonImage()
		{
			if(this.popup.IsShowing())
				this.SetToggleButtonImage(false);
			else
				this.SetToggleButtonImage(true);
		}

		private void SetToggleButtonImage(bool dropDown)
		{
			if(dropDown)
				this.toggle.ImageIndex = 1;
			else
				this.toggle.ImageIndex = 0;
		}

		private void toggle_Click(object sender, System.EventArgs e)
		{
			this.TogglePopupVisibility();
		}

		private void PopupDesigner_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			this.ShowPopup();
		}
		public void ShowPopup()
		{
			if(this.popup != null && !this.popup.IsShowing())
				this.TogglePopupVisibility();
		}
		public void ClosePopup()
		{
			if(this.popup != null && this.popup.IsShowing())
				this.TogglePopupVisibility();
		}

		private void PopupDesignerForm_VisibleChanged(object sender, System.EventArgs e)
		{
			this.UpdateToggleButtonImage();
		}
	}
	public class PopupMenuDesigner : ComponentDesigner, IListenForMainFormVisibilityChange
	{
		// Not subclassing for now. Possible choke due to multiple subclassing (here and in BarManagerDesigner).
		// BarManagerDesigner.VSMainFormWnd vsMainForm;
		private PopupDesignerForm popupForm;
		private DesignerVerbCollection verbs;
		private ParentBarItem latestParentBarItem;
		private IDesignerHost iDesignerHost;

		// To force the popup menus to close before serialization begins
		private CustomSerializationProvider dummySerProvider;

		public PopupMenuDesigner()
		{
			//vsMainForm = new BarManagerDesigner.VSMainFormWnd(this);
			popupForm = new PopupDesignerForm();
		}
		public override void Initialize(IComponent component)
		{
			PopupMenu popupMenu = component as PopupMenu;
			popupMenu.ParentBarItemChanged += new EventHandler(this.ParentBarItemChanged);
			base.Initialize(component);
			this.UpdateCachedParentBarItem(popupMenu);

			iDesignerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			iDesignerHost.Activated += new System.EventHandler(this.OnActivated);
			iDesignerHost.Deactivated += new System.EventHandler(this.OnDeactivated);

			// Serialization listeners
			System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager =
				(System.ComponentModel.Design.Serialization.IDesignerSerializationManager)
				this.GetService(typeof(System.ComponentModel.Design.Serialization.IDesignerSerializationManager));

			// Subclass the VS Mainform
//			if(this.vsMainForm.Handle == IntPtr.Zero)
//			{	
//				IntPtr hwndvsform = NativeMethods.GetAncestor(this.GetHostForm().Handle, 3);
//				if(hwndvsform != IntPtr.Zero)
//					this.vsMainForm.AssignHandleCustom(hwndvsform);
//			}

			if(manager != null)
			{
				dummySerProvider = new CustomSerializationProvider(this);
				manager.AddSerializationProvider(this.dummySerProvider);
			}
		}

		protected void OnActivated(Object sender, EventArgs e)
		{
			this.OnChangedVisibility(true);
		}

		protected void OnDeactivated(Object sender, EventArgs e)
		{
			this.OnChangedVisibility(false);
		}

		private Form GetHostForm()
		{
			Form hostform = iDesignerHost.RootComponent as Form;
			return hostform;
		}

		public override DesignerVerbCollection Verbs 
		{ 
			get
			{
				if (this.verbs == null)
				{
					this.verbs = new DesignerVerbCollection();
					this.verbs.Add(new DesignerVerb("Customize...",new EventHandler(this.OnCustomize)));
					this.verbs.Add(new DesignerVerb("Add Default ParentBarItem...",new EventHandler(this.OnAddDefaultParentBarItem)));
				}
				return this.verbs;
			}
		}
		private void OnAddDefaultParentBarItem(object sender, EventArgs e)
		{
			PopupMenu popupMenu = this.Component as PopupMenu;
			if(popupMenu.ParentBarItem != null)
				MessageBox.Show("The PopupMenu's ParentBarItem should be null in order to add a New ParentBarItem.");
			else
			{
				IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
				ParentBarItem parentBarItem = (ParentBarItem)designerHost.CreateComponent(typeof(ParentBarItem));
				popupMenu.ParentBarItem = parentBarItem;
			}
		}

		private void OnCustomize(object sender, EventArgs eevent)
		{
			PopupMenu popupMenu = this.Component as PopupMenu;
			if(popupMenu.ParentBarItem == null)
				MessageBox.Show("Please associate a ParentBarItem with the PopupMenu before Customizing it.");
			else
			{
//				if(this.vsMainForm.Handle == IntPtr.Zero)
//				{	
//					IntPtr hwndvsform = NativeMethods.GetAncestor(this.GetHostForm().Handle, 3);
//					if(hwndvsform != IntPtr.Zero)
//						this.vsMainForm.AssignHandleCustom(hwndvsform);
//				}

				if(popupMenu.ParentBarItem.Manager != null)
					popupMenu.ParentBarItem.Manager.Customize((System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost)));
				this.popupForm.Customize(popupMenu);
			}
		}
		private void ParentBarItemChanged(object sender, EventArgs e)
		{
			this.UpdateCachedParentBarItem(this.Component as PopupMenu);
		}
		private void UpdateCachedParentBarItem(PopupMenu popupMenu)
		{
			this.CloseDesigner();
			this.latestParentBarItem = popupMenu.ParentBarItem;
			if(this.latestParentBarItem != null
				&& this.latestParentBarItem.Manager != null)
				this.latestParentBarItem.Manager.CustomizationDone += new EventHandler(this.CustomizationDone);
		}
		private void CustomizationDone(object sender, EventArgs e)
		{
			this.CloseDesigner();
		}
		private void CloseDesigner()
		{
			if(this.popupForm != null && this.popupForm.Visible )
			{
				this.popupForm.Close();
			}
			if(this.latestParentBarItem != null)
			{
				if(this.latestParentBarItem.Manager != null)
					this.latestParentBarItem.Manager.CustomizationDone -= new EventHandler(this.CustomizationDone);
				this.latestParentBarItem = null;
			}
		}
		public void ClosePopup()
		{
			if(this.popupForm != null)
				this.popupForm.ClosePopup();
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				//			if(this.vsMainForm.Handle != IntPtr.Zero)
				//				this.vsMainForm.ReleaseHandleCustom();

				if(this.popupForm != null)
				{
					PopupMenu popupMenu = this.Component as PopupMenu;
					popupMenu.ParentBarItemChanged -= new EventHandler(this.ParentBarItemChanged);
					this.CloseDesigner();
					this.popupForm.Dispose();
					this.popupForm = null;
				}
			}
			base.Dispose(disposing);
		}
		public void OnChangedVisibility(bool visible)
		{
			if(!visible)
				this.CloseDesigner();
		}
	}
	public class CustomSerializationProvider : IDesignerSerializationProvider
	{
		private PopupMenuDesigner notifyDesigner;
		public CustomSerializationProvider(PopupMenuDesigner notifyDesigner)
		{
			this.notifyDesigner = notifyDesigner;
		}
		public virtual object GetSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType,
			Type serializerType)
		{
			if(objectType != null && 
				(objectType.IsSubclassOf(typeof(BarItems))
					|| objectType == typeof(BarItems)))
				notifyDesigner.ClosePopup();
			return null;
		}
	}
}
