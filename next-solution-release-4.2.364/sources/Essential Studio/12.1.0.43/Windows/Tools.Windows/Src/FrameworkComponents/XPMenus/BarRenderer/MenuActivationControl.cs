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

using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools.XPMenus;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	/// <summary>
	/// MenuActivationControl class using subclassing to detect deactivate application message.
	/// </summary>
	public class MenuActivationControl
		: NativeWindowSubclass
	{
		#region Class events
		/// <summary>
		/// Delegate for event.
		/// </summary>
		public delegate void EventHandler( object o, EventArgs e );

		/// <summary>
		/// Event method.
		/// </summary>
		public event EventHandler AppDeactivate;
		/// <summary>
		/// Event fired on mouseActivate message.
		/// </summary>
		public event EventHandler MouseActivate;
		/// <summary>
		/// Event fired on activate application message.
		/// </summary>
		public event EventHandler AppActivate;

		/// <summary>
		/// Fires application deactivate event.
		/// </summary>
		private void RaiseAppDeactivateEvent()
		{
			if( AppDeactivate != null )
			{
				AppDeactivate( this, EventArgs.Empty );
			}
		}
		private void RaiseMouseActivateEvent()
		{
			if( MouseActivate != null )
			{
				MouseActivate( this, EventArgs.Empty );
			}
		}
		private void RaiseAppActivateEvent()
		{
			if( AppActivate != null )
			{
				AppActivate( this, EventArgs.Empty );
			}
		}

		#endregion

		#region Class initialise/finalize methods
		/// <summary>
		/// Constructor.
		/// </summary>
		public MenuActivationControl()
			: base()
		{}
		#endregion

		#region Class overrides
		/// <summary>
		/// Detecting deactivate application message.
		/// </summary>
		/// <param name="m">windows message.</param>
		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			base.WndProc( ref m );

			if( m.Msg == NativeMethods.WM_ACTIVATEAPP )
			{
				if( m.WParam == IntPtr.Zero )
				{
					RaiseAppDeactivateEvent();
				}
				else
				{
					RaiseAppActivateEvent();
				}
			}
			else if( m.Msg == NativeMethods.WM_MOUSEACTIVATE )
			{
				RaiseMouseActivateEvent();
			}
		}
		#endregion
	}
}
