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
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// A form-derived class that can be derived to create custom top-level 
	/// windows like ToolTips, a splash window, etc.
	/// </summary>
	public class TopLevelWindow : Form 
	{
		internal const int SWP_NOSIZE = 1; // 0x0001 
		internal const int SWP_NOMOVE = 2; // 0x0002 
		internal const int SWP_NOZORDER = 4; // 0x0004 
		internal const int SWP_NOACTIVATE = 16; // 0x0010 
		internal const int SWP_SHOWWINDOW = 64; // 0x0040 
		internal const int SWP_HIDEWINDOW = 128; // 0x0080 
		internal const int SWP_DRAWFRAME = 32; // 0x0020 

		/// <summary>
		/// Creates a new instance of the TopLevelWindow class.
		/// </summary>
		public TopLevelWindow()  
		{
			this.InitializeComponent();
		}

		// Methods
		/// <summary>
		/// Shows the window as the top-level window without activating it.
		/// </summary>
		public void ShowWindowTopMost()  
		{
			NativeMethods.SetWindowPos(this.Handle, (IntPtr)NativeMethods.HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE|SWP_SHOWWINDOW);
		}

		private void InitializeComponent()  
		{
			this.SuspendLayout();
            this.ControlBox = false;
			this.FormBorderStyle = FormBorderStyle.None;
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.Manual;
			this.TopLevel = true;  // Must be TopLevel to allow transparency
			this.Location = new Point(-1000, -1000);
			this.Size = new Size(0, 0);
			this.ResumeLayout(false);
//			this.BackColor = Color.Red;
//			//this.AllowTransparency = true;
//			this.TransparencyKey = Color.Red;
//			this.Opacity = 0.69f;
//			this.ShowWindowTopMost();
		}

		/// <summary>
		/// Overridden to ignore Win32Exception.
		/// </summary>
		protected override void OnHandleCreated(EventArgs e)
		{
			try
			{
				base.OnHandleCreated (e);
			}
			catch(System.ComponentModel.Win32Exception)
			{
				// ignore the Win32Exception - this occurs when used
				// OLE Inplace in Office applications ...
			}
		}

	}
}
