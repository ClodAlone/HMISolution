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

#region Clas using directives
using System;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// VSSubclass is used to detect showing/hiding of Visual studio.
	/// </summary>
	internal class VSSubclass
		: NativeWindowSubclass
	{
		#region Class members
		/// <summary>
		/// Is VS window is visible.
		/// </summary>
		private bool m_bVisible = false;
		#endregion

		#region Class properties
		/// <summary>
		/// Indicates whether VS window is visible.
		/// </summary>
		protected bool StudioVisible
		{
			get
			{
				return m_bVisible;
			}
			set
			{
				if( m_bVisible != value )
				{
					m_bVisible = value;
					RaiseVisibleChangedEvent();
				}
			}
		}
		#endregion

		#region Class overrides
		/// <summary>
		/// Override.
		/// </summary>
		/// <param name="m">Windows message.</param>
		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			if( m.Msg == 0x0018 )
				if( m.WParam == IntPtr.Zero )
					StudioVisible = false;
				else
					StudioVisible = true;

			base.WndProc (ref m);
		}
		#endregion

		#region Visible Changed event implementation
		public delegate void VisibleChangedEventH( object sender, bool visible );
		public event VisibleChangedEventH VisibleChanged;
		/// <summary>
		/// Raises VisibleChanged event.
		/// </summary>
		private void RaiseVisibleChangedEvent()
		{
			if( VisibleChanged != null )
				VisibleChanged( this, m_bVisible );
		}
		#endregion
	}
}
