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
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using System.Globalization;

using Syncfusion.Runtime.InteropServices;


namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Wrapper class for setting the current <see cref="Thread"/>'s <see cref="Thread.CurrentCulture"/>.
	/// </summary>
	public class ThreadCulture
	{

		/// <summary>
		/// Event raised when the ThreadCulture is changed or the system settings 
		/// changed the culture.
		/// </summary>
		public static event CultureChangedEventHandler CultureChanged;


		/// <summary>
		/// Handles the CultureChanged event raised by the settings form.
		/// </summary>
		/// <param name="sender">The settings form.</param>
		/// <param name="e">The event data.</param>
		protected static void HandleCultureChangedEvent(object sender, CultureChangedEventArgs e)
		{
			OnCultureChanged(sender, e);
		}

		/// <summary>
		/// Sets the current thread's CurrentCulture.
		/// </summary>
		/// <param name="newInfo">The culture to be set.</param>
		public static void SetCurrentCulture(CultureInfo newInfo)
		{
			CultureInfo currentInfo = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = newInfo;

			if(currentInfo != newInfo)
				RaiseCultureChangedEvent(currentInfo.LCID , newInfo.LCID, CultureChangedReason.ThreadCulture );
		}

		/// <summary>
		/// Raises the CultureChanged event.
		/// </summary>
		/// <param name="currentLCID">The previous culture's LCID.</param>
		/// <param name="newLCID">The new culture's LCID.</param>
		static void RaiseCultureChangedEvent(int currentLCID, int newLCID, CultureChangedReason reason)
		{
			CultureChangedEventArgs e = new CultureChangedEventArgs(currentLCID,newLCID, reason);
			OnCultureChanged(Thread.CurrentThread, e);
		}

		/// <summary>
		/// Raises the <see cref="CultureChanged"/> event.
		/// </summary>
		/// <param name="sender">Object raising the event.</param>
		/// <param name="e">The event data.</param>
		static void OnCultureChanged(object sender, CultureChangedEventArgs e)
		{
			if(CultureChanged != null)
				CultureChanged(sender, e);
		}

	}

	public class ThreadCultureSystem : ThreadCulture
	{
		/// <summary>
		/// The form used for listening to the WM_SETTINGCHANGE messages.
		/// </summary>
		private static SystemSettings settingsChangeForm;

		/// <summary>
		/// Static constructor for ThreadCultureSystem class.
		/// </summary>
		static ThreadCultureSystem()
		{
			settingsChangeForm = new SystemSettings();
			settingsChangeForm.CultureChanged += new CultureChangedEventHandler(HandleCultureChangedEvent);
			settingsChangeForm.Show();
		}
	}

	/// <summary>
	/// Form derived class that is registered to receive WM_SETTINGCHANGE messages.
	/// </summary>
	[ToolboxItem(false)]
	internal class SystemSettings: Form
	{
		/// <summary>
		/// Event raised when the form senses that the culture has been changed.
		/// </summary>
		public event CultureChangedEventHandler CultureChanged;

		public SystemSettings()
		{
			this.Visible = false;
			this.TopLevel = true;
			this.Size = new Size(1,1);
			this.Location = new Point(-1000,-1000);
			this.Text = "";
			this.ShowInTaskbar = false;
			this.ControlBox = false;
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
		}

		/// <summary>
		/// Overrides <see cref="WndProc"/> to handle the WM_SETTINGCHANGE
		/// message.
		/// </summary>
		/// <param name="m">The message.</param>
		protected override void WndProc(ref Message m)
		{
			if(m.Msg ==	NativeMethods.WM_SETTINGCHANGE)
			{
				if(Marshal.PtrToStringAuto(m.LParam) == "intl")
				{
					int newLCID = NativeMethods.GetUserDefaultLCID();
					int currentLCID = Application.CurrentCulture.LCID;
					CultureChangedEventArgs e = new CultureChangedEventArgs(currentLCID,newLCID, CultureChangedReason.SystemSettings );
					this.OnCultureChanged(e);
				}
			}
			base.WndProc(ref m);
		}

		/// <summary>
		/// Overrides <see cref="Form.Show"/> to always set the location to be
		/// outside the screen bounds.
		/// </summary>
		public new void Show()
		{
			this.Location = new Point(-1000,-1000);
			base.Show();
		}

		/// <summary>
		/// Raises the <see cref="CultureChanged"/> event.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected virtual void OnCultureChanged(CultureChangedEventArgs e)
		{
			if(this.CultureChanged != null)
				this.CultureChanged(this,e);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			if(this.Visible == true)
				this.Visible = false;
		}
	}

	
	/// <summary>
	/// Handles the locale changed event.
	/// </summary>
	public delegate void CultureChangedEventHandler(object sender, CultureChangedEventArgs e);

	/// <summary>
	/// Provides data for the <see cref="ThreadCulture.CultureChanged"/> event.
	/// </summary>
	public class CultureChangedEventArgs : EventArgs
	{
		private int newLCID = 0;
		private int currentLCID = 0;
		private CultureChangedReason reason = CultureChangedReason.ThreadCulture;

		public CultureChangedEventArgs(int currentLCID, int newLCID, CultureChangedReason reason)
		{
			this.currentLCID = currentLCID;
			this.newLCID = newLCID;
			this.reason = reason;
		}

		/// <summary>
		/// Returns the LCID of the new culture that has been applied.
		/// </summary>
		public int NewLCID
		{
			get
			{
				return this.newLCID;
			}
		}

		/// <summary>
		/// Returns the LCID of the previous culture.
		/// </summary>
		public int CurrentLCID
		{
			get
			{
				return this.currentLCID;
			}
		}

		/// <summary>
		/// Returns the reason for the CultureChange.
		/// </summary>
		public CultureChangedReason Reason
		{
			get
			{
				return this.reason;
			}
		}
	}

	/// <summary>
	/// CultureChangedReason is used by the <see cref="CultureChangedEventArgs"/> to denote
	/// what action initiated the culture change.
	/// </summary>
	public enum CultureChangedReason
	{
		/// <summary>
		/// A System level setting change resulted in the culture change.
		/// </summary>
		SystemSettings = 0,

		/// <summary>
		/// Thread level culture change resulted in the culture change.
		/// </summary>
		ThreadCulture =1
	}
}
