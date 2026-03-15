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
using Syncfusion.Windows.Forms;
using System.ComponentModel.Design;
using System.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class BarCustomizationDialog : System.Windows.Forms.Form, IDontCallKillFocus,
		IDontCallSetFocus,
		IAmACustomizationForm
	{
		private CustomizationPanel custPanel;
		private BarManager customizingBarManager;
		private IDesignerHost designerHost;
		private System.ComponentModel.IContainer components = null;

		public BarCustomizationDialog(BarManager manager)
		{
			this.customizingBarManager = manager;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			this.AllowDrop = true;
#endif
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.custPanel.InitCustomizationPanel(this, this.customizingBarManager);
		}

		public void SetCustomizationPanel(CustomizationPanel customizationPanel)
		{
			Trace.Assert(customizationPanel != null, "The customizationPanel argument cannot be null while calling SetCustomizationPanel.");
			if(customizationPanel == null)
				return;

			this.SuspendLayout();
			if(this.custPanel != null)
			{
				this.Controls.Remove(this.custPanel);
				this.custPanel.Dispose();
			}
			this.custPanel = customizationPanel;
			this.InitCustPanelProperties();
			this.Controls.Add(this.custPanel);
			this.ResumeLayout(false);

			this.custPanel.InitCustomizationPanel(this, this.customizingBarManager);
		}

		private void InitCustPanelProperties()
		{
			// Copied from InitializeComponent
			this.custPanel.SuspendLayout();
			this.custPanel.Name = "custPanel";
			this.custPanel.TabIndex = 0;
			this.custPanel.ResumeLayout(true);
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp;
				cp = base.CreateParams;
				Version version = Environment.OSVersion.Version;
				cp.ExStyle = (cp.ExStyle | 0x80/*WS_EX_TOOLWINDOW*/);

				// Otherwise has trouble createing window in NT4.0
				if(Environment.OSVersion.Platform != PlatformID.Win32NT ||
					Environment.OSVersion.Version.Major > 5)
					cp.ExStyle |= 0x08000000/*WS_EX_NOACTIVATE*/;

				return cp;
			}
		}

		public IDesignerHost DesignerHost
		{
			set
			{
				this.designerHost = value;
				this.custPanel.DesignerHost = value;
			}
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			this.custPanel.OnDialogClosing();

			e.Cancel = true;
			this.Hide();
			this.customizingBarManager.OnCustomizationDone(EventArgs.Empty);
			if(!this.customizingBarManager.DesignMode)
			{
				NativeMethods.SetActiveWindow(
					this.customizingBarManager.Form.Handle);
			}
			IntPtr activeWindow = NativeMethods.GetActiveWindow();
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(BarCustomizationDialog));
			this.custPanel = new Syncfusion.Windows.Forms.Tools.XPMenus.CustomizationPanel();
			this.SuspendLayout();
			// 
			// custPanel
			// 
			this.custPanel.AccessibleDescription = resources.GetString("custPanel.AccessibleDescription");
			this.custPanel.AccessibleName = resources.GetString("custPanel.AccessibleName");
			this.custPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("custPanel.Anchor")));
			this.custPanel.AutoScroll = ((bool)(resources.GetObject("custPanel.AutoScroll")));
			this.custPanel.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("custPanel.AutoScrollMargin")));
			this.custPanel.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("custPanel.AutoScrollMinSize")));
			this.custPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("custPanel.BackgroundImage")));
			this.custPanel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("custPanel.Dock")));
			this.custPanel.Enabled = ((bool)(resources.GetObject("custPanel.Enabled")));
			this.custPanel.Font = ((System.Drawing.Font)(resources.GetObject("custPanel.Font")));
			this.custPanel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("custPanel.ImeMode")));
			this.custPanel.Location = ((System.Drawing.Point)(resources.GetObject("custPanel.Location")));
			this.custPanel.Name = "custPanel";
			this.custPanel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("custPanel.RightToLeft")));
			this.custPanel.Size = ((System.Drawing.Size)(resources.GetObject("custPanel.Size")));
			this.custPanel.TabIndex = ((int)(resources.GetObject("custPanel.TabIndex")));
			this.custPanel.Visible = ((bool)(resources.GetObject("custPanel.Visible")));
			// 
			// BarCustomizationDialog
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.custPanel);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimizeBox = false;
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "BarCustomizationDialog";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = GetLocalizedString(SR.BarCustomizationDialogCaption,resources,"$this.Text");
			this.Load += new System.EventHandler(this.BarCustomizationForm_Load);
			this.Activated += new System.EventHandler(this.BarCustomizationDialog_Activated);
			this.Deactivate += new System.EventHandler(this.BarCustomizationDialog_Deactivate);
			this.ResumeLayout(false);

		}
		#endregion

		private string GetLocalizedString(string Key, System.Resources.ResourceManager resources, string name)
		{
			string result = SR.GetString(Key);

			if (result == string.Empty)
				return resources.GetString(name);

			return result;
		}

		private void BarCustomizationForm_Load(object sender, System.EventArgs e)
		{
			this.custPanel.OnDialogLoad();
		}

		protected override void OnDragLeave( EventArgs e )
		{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			if( this.customizingBarManager != null )
			{
				this.customizingBarManager.CustomDrag = 
					( this.customizingBarManager.CustomizingItem != null );
			}
#endif
			base.OnDragLeave( e );
		}

		protected override void OnDragEnter( DragEventArgs drgevent )
		{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			if( this.customizingBarManager != null )
			{
				this.customizingBarManager.CustomDrag = false;
			}
#endif
			base.OnDragEnter( drgevent );
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			this.custPanel.OnDialogVisibiltyChanged();
		}

		private void BarCustomizationDialog_Activated(object sender, System.EventArgs e)
		{
			this.custPanel.OnDialogActivated();
		}

		private void BarCustomizationDialog_Deactivate(object sender, System.EventArgs e)
		{
			this.custPanel.OnDialogDeactivated();
		}

		public void RemoveReferencesToItem(BarItem item)
		{
			this.custPanel.RemoveReferencesToItem(item);
		}
		public void RemoveReferencesToItem(Bar bar)
		{
			this.custPanel.RemoveReferencesToItem(bar);
		}
	}
}
