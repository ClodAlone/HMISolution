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
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class NativeWindowSubclass : NativeWindow
	{
		private NativeWindow cachedWindow;
		private IntPtr cachedHandle;
		private Control controlSubclassed;

		private void BeforeAssignHandle(IntPtr handle)
		{
			this.cachedWindow = NativeWindow.FromHandle(handle);	
			this.cachedHandle = handle;
		}
		public void AfterReleaseHandle()
		{
			if(this.cachedWindow is NativeWindowSubclass)
				((NativeWindowSubclass)this.cachedWindow).AfterReleaseHandle();

			this.ResetCachedWindow();
			this.cachedHandle = IntPtr.Zero;
		}

		public void DoReleaseHandle()
		{
			if (this.Handle != IntPtr.Zero)
			{
				this.ReleaseHandle();

				if (this.cachedWindow is NativeWindowSubclass)
					((NativeWindowSubclass)this.cachedWindow).DoReleaseHandle();
				else if (this.cachedWindow != null)
					this.cachedWindow.ReleaseHandle();
			}
		}

		private void ResetCachedWindow()
		{
			if(this.cachedWindow != null)
			{
				if(this.cachedWindow is NativeWindowSubclass)
					((NativeWindowSubclass)this.cachedWindow).AssignHandleCustom(this.cachedHandle);
				else
					this.cachedWindow.AssignHandle(this.cachedHandle);
			}
		}

		#region Assinging Controls
		public void AssignHandleCustom(Control control)
		{
			this.SubclassedControl = control;
		}

		private Control SubclassedControl
		{
			get{return this.controlSubclassed;}
			set
			{
				if(this.controlSubclassed != value)
				{
					if(this.controlSubclassed != null)
					{
						// Releasing handle as late as the control's dispose method, since there are problems with
						// the ReleaseHandle method that will not reset handle / control mappings properly.
						this.controlSubclassed.Disposed -= new EventHandler(Control_Disposed);
						this.controlSubclassed.HandleCreated -= new EventHandler(Control_HandleCreated);
			
						this.MyHandle = IntPtr.Zero;
					}

					this.controlSubclassed = value;

					if(this.controlSubclassed != null)
					{
						this.controlSubclassed.Disposed += new EventHandler(Control_Disposed);

						// Trying to avoid ReleaseHandle as much as possible, hence delaying AssignHandle as much as possible.
						if(this.controlSubclassed.IsHandleCreated)
							this.AssignHandleCustom(this.controlSubclassed.Handle);

						this.controlSubclassed.HandleCreated += new EventHandler(Control_HandleCreated);
					}
				}
			}
		}

		private void Control_HandleCreated(object sender, EventArgs e)
		{
			// Removing and resetting association.
			Control currentControl = this.SubclassedControl;

			this.SubclassedControl = null;
			
			this.SubclassedControl = currentControl;
		}

		private void Control_Disposed(object sender, EventArgs e)
		{
			this.SubclassedControl = null;
		}

		#endregion

		public void AssignHandleCustom(IntPtr handle)
		{
			this.MyHandle = handle;
			//this.AssignHandle(handle);
		}

		public void ReleaseHandleCustom()
		{
			this.SubclassedControl = null;
			this.MyHandle = IntPtr.Zero;
			//this.ReleaseHandle();
		}

		private IntPtr MyHandle
		{
			set
			{
				if(this.Handle != value)
				{

					if(this.Handle != IntPtr.Zero)
					{
						this.DoReleaseHandle();
					
						this.AfterReleaseHandle();
					}

					if(value != IntPtr.Zero)
					{
						this.BeforeAssignHandle(value);
						this.AssignHandle(value);
					}
				}
			}
		}
	}
}
