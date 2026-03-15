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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents progress bar able to reflect functionality of referenced toolstrip progress bar.
	/// </summary>
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailabilityAttribute(
		System.Windows.Forms.Design.ToolStripItemDesignerAvailability.None )]
	public class ProgressbarReflectable
		: ToolStripProgressBar
		, INativeMessageFilter
	{
		#region Constants
		/// <summary>
		/// Values for progress bar related windows messages.
		/// </summary>
		
		private const int PBM_SETRANGE = ( int )Win32API.Msg.WM_USER + 1;
		private const int PBM_SETPOS = ( int )Win32API.Msg.WM_USER + 2;
		private const int PBM_DELTAPOS = ( int )Win32API.Msg.WM_USER + 3;
		private const int PBM_SETSTEP = ( int )Win32API.Msg.WM_USER + 4;
		private const int PBM_STEPIT = ( int )Win32API.Msg.WM_USER + 5;
		private const int PBM_SETRANGE32 = ( int )Win32API.Msg.WM_USER + 6;
		#endregion

		#region Fields
		/// <summary>
		/// Reflected toolstrip progress bar.
		/// </summary>
		private ToolStripProgressBar m_reflectedProgressbar;
		/// <summary>
		/// NativeMessageHandler instance for subclassing reflected progress bar.
		/// </summary>
		private NativeMessageHandler m_nativeMesHandler;
		private bool m_bExactCopy = false;
		#endregion


		#region Properties
		/// <summary>
		/// Gets or sets reflected progress bar.
		/// </summary>
		public ToolStripProgressBar ReflectedProgressBar
		{
			get
			{
				return m_reflectedProgressbar;
			}
			set
			{
				if( m_reflectedProgressbar != null )
				{
					m_nativeMesHandler.MessageFilter = null;
				}

				m_reflectedProgressbar = value;
				
				if (m_reflectedProgressbar != null)
				{
					this.Maximum = m_reflectedProgressbar.Maximum;
					this.Minimum = m_reflectedProgressbar.Minimum;
					this.Step = m_reflectedProgressbar.Step;
					this.Style = m_reflectedProgressbar.Style;
					this.Value = m_reflectedProgressbar.Value;

					if (m_bExactCopy)
					{
						this.AutoSize = m_reflectedProgressbar.AutoSize;
					}

					m_nativeMesHandler = new NativeMessageHandler();
					m_nativeMesHandler.MessageFilter = this;
					m_nativeMesHandler.Assign(m_reflectedProgressbar.ProgressBar.Handle);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override Size Size
		{
			get
			{
				if (m_bExactCopy && m_reflectedProgressbar != null && !this.AutoSize)
				{
					return m_reflectedProgressbar.Size;
				}
				return base.Size;
			}
			set
			{
				base.Size = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates new instance of ProgressbarReflectable.
		/// </summary>
		public ProgressbarReflectable()
		{ }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reflectedItem"></param>
		public ProgressbarReflectable(ToolStripProgressBar reflectedItem) : this(reflectedItem, false)
		{}
		/// <summary>
		/// Creates & initializes new instance of ProgressbarReflectable.
		/// </summary>
		/// <param name="reflectedItem">Toolstrip progress bar to reflect.</param>
		public ProgressbarReflectable( ToolStripProgressBar reflectedItem, bool bExactCopy )
		{
			m_bExactCopy = bExactCopy;
			this.ReflectedProgressBar = reflectedItem;
		}
		#endregion

		#region INativeMessageFilter Members
		/// <summary>
		/// Processes messages of subclassed progress bar.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public bool ProcessMessage(ref Message m)
		{
			switch( m.Msg )
			{
				case PBM_SETRANGE :
				case PBM_SETPOS :
				case PBM_DELTAPOS :
				case PBM_SETSTEP :
				case PBM_STEPIT :
				case PBM_SETRANGE32 :
					WindowsAPI.SendMessage( this.ProgressBar.Handle, m.Msg, m.WParam, m.LParam );
					break;
			}

			return false;
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( m_reflectedProgressbar != null )
			{
				m_nativeMesHandler.MessageFilter = null;
			}
			base.Dispose( disposing );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="constrainingSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize(Size constrainingSize)
		{
			if (m_bExactCopy && m_reflectedProgressbar != null)
			{
				return m_reflectedProgressbar.GetPreferredSize(constrainingSize);
			}
			return base.GetPreferredSize(constrainingSize);
		}
			 
		#endregion
	}
}
#endif