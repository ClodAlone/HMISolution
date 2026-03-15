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
using System.ComponentModel;
using Syncfusion.Windows.Forms.Tools.Win32API;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// 
	/// </summary>
	internal class ToolStripGalleryDropDown : ToolStripDropDown
	{
		#region Constants
		/// <summary>
		/// 
		/// </summary>
		internal const int GALLERY_DRAGGER_HEIGHT = 12;
		/// <summary>
		/// 
		/// </summary>
		internal static readonly Size DROPDOWN_MIN_SIZE = new Size( 160, 105 );
		#endregion

		#region Fields
		/// <summary>
		/// 
		/// </summary>
		private bool m_bShowGrip = true;
		/// <summary>
		/// 
		/// </summary>
		private ToolStripGallery m_gallery;
		/// <summary>
		/// 
		/// </summary>
		private Rectangle m_draggerRect;
		/// <summary>
		/// 
		/// </summary>
		private Size m_dropDownMinSize = DROPDOWN_MIN_SIZE;
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public bool ShowGrip
		{
			get
			{
				return m_bShowGrip;
			}
			set
			{
				if( m_bShowGrip != value )
				{
					m_bShowGrip = value;
					PerformLayout();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public Size DropDownMinimumSize
		{
			get
			{
				return m_dropDownMinSize;
			}
			set
			{
				m_dropDownMinSize = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal ToolStripGallery Gallery
		{
			get
			{
				return m_gallery;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gallery"></param>
		public ToolStripGalleryDropDown( ToolStripGallery gallery )
		{
			m_gallery = gallery;
			m_gallery.AutoSize = false;
			this.Items.Add( m_gallery );
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLayout( LayoutEventArgs e )
		{
            if (m_gallery != null)
            {
                base.OnLayout(e);

			m_gallery.Height = this.Height - ( GALLERY_DRAGGER_HEIGHT + this.Margin.Vertical );
			m_gallery.Width = this.Width - this.Margin.Horizontal;
			SetItemLocation( m_gallery, new Point( this.Margin.Left, this.Margin.Top ) );
			m_draggerRect = new Rectangle( this.Width - 10, this.Height - 10, 10, 10 );
            }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="proposedSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize( Size proposedSize )
		{
			Size result = m_gallery.GetPreferredSize( Size.Empty );
			m_gallery.Height = result.Height;
			result.Height += GALLERY_DRAGGER_HEIGHT + this.Margin.Vertical;
			result.Width += this.Margin.Horizontal;
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mea"></param>
		protected override void OnMouseMove( MouseEventArgs mea )
		{
			base.OnMouseMove( mea );

			if( m_draggerRect.Contains( mea.Location ) )
			{
				this.Cursor = Cursors.SizeNWSE;
			}
			else
			{
				this.Cursor = Cursors.Default;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mea"></param>
		protected override void OnMouseDown( MouseEventArgs mea )
		{
			base.OnMouseDown( mea );

			if( m_draggerRect.Contains( mea.Location ) )
			{
				int lParam = WindowsAPI.MAKELONG( Cursor.Position.X, Cursor.Position.Y );
				this.AutoSize = false;
				WindowsAPI.SendMessage(
					this.Handle, ( int )Msg.WM_SYSCOMMAND, ( int )SystemCommand.SC_SIZE | 8 /*WMSZ_BOTTOMRIGHT*/, ( IntPtr )lParam );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnOpening( CancelEventArgs e )
		{
			this.AutoSize = true;

			if ( !m_gallery.ShowItem(m_gallery.CheckedItem) )
			{
				this.m_gallery.ScrollOffset = 0;
			}
			
			base.OnOpening( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc( ref Message m )
		{
			base.WndProc( ref m );

			switch( ( Msg )m.Msg )
			{
				case Msg.WM_GETMINMAXINFO:
				{
					MINMAXINFO mmInfo = ( MINMAXINFO )Marshal.PtrToStructure( m.LParam, typeof( MINMAXINFO ) );
					mmInfo.ptMinTrackSize = new POINT( m_dropDownMinSize.Width, m_dropDownMinSize.Height );
					Marshal.StructureToPtr( mmInfo, m.LParam, false );
					break;
				}
			}
		}
		#endregion
	}
}

#endif
