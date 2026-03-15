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
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Handles the NcPaint event of an ScrollControl object.
	/// </summary>
	public delegate void NCPaintEventHandler( object sender, NCPaintEventArgs e );

	/// <summary>
	/// Provides data for the NCPaint event of an ScrollControl object.
	/// </summary>
	public class NCPaintEventArgs : PaintEventArgs
	{
		#region Initialization
		/// <summary>
		/// Initializes a new instance of the NCPaintEventArgs class.
		/// </summary>
		/// <param name="pGraphics"></param>
		/// <param name="pClipRegion"></param>
		/// <param name="pDisplay"></param>
		/// <param name="pWindowInScreen"></param>
		/// <param name="pPtrClipRegion"></param>
		public NCPaintEventArgs( Graphics pGraphics, Rectangle pClipRegion,
			Rectangle pDisplay, Rectangle pWindowInScreen, IntPtr pPtrClipRegion ) : base( pGraphics, pClipRegion )
		{
			m_rcDisplay = pDisplay;
			m_rcWindowInScreen = pWindowInScreen;
			m_ptrClipRegion = pPtrClipRegion;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets bounds an ScrollControl.
		/// </summary>
		public Rectangle DisplayRectangle
		{
			get
			{
				return m_rcDisplay;
			}
			set
			{
				m_rcDisplay = value;
			}
		}
		/// <summary>
		/// Gets or sets bounds of an ScrollControl in screen coordinates.
		/// </summary>
		public Rectangle WindowInScreenRectangle
		{
			get
			{
				return m_rcWindowInScreen;
			}
			set
			{
				m_rcWindowInScreen = value;
			}
		}
		/// <summary>
		/// Gets or sets clipping region of an ScrollControl.
		/// </summary>
		public IntPtr ClipRegion
		{
			get
			{
				return m_ptrClipRegion;
			}
			set
			{
				m_ptrClipRegion = value;
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (m_ptrClipRegion != IntPtr.Zero)
			{
				Syncfusion.Runtime.InteropServices.NativeMethods.DeleteObject(m_ptrClipRegion);
				m_ptrClipRegion = IntPtr.Zero;
			}
			base.Dispose(disposing);
		}
		#endregion

		#region Fields
		/// <summary>
		/// Bounds an ScrollControl.
		/// </summary>
		private Rectangle m_rcDisplay = Rectangle.Empty;
		/// <summary>
		/// Bounds of an ScrollControl in screen coordinates. 
		/// </summary>
		private Rectangle m_rcWindowInScreen = Rectangle.Empty;
		/// <summary>
		/// Clipping region of an ScrollControl.
		/// </summary>
		private IntPtr m_ptrClipRegion;
		#endregion
	}
}
