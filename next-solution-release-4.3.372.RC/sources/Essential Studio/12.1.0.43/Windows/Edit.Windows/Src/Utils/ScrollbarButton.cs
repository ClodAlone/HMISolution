#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Button that can be placed at the edges of the scrollbars.
	/// </summary>
	[ToolboxItem( false )]
	public class ScrollbarButton
		: ButtonAdv
	{
		#region Fields
		/// <summary>
		/// Indicates whether popup container should be shown on button click.
		/// </summary>
		private bool m_bIsPopup = false;
		/// <summary>
		/// Popup container.
		/// </summary>
		private PopupControlContainer m_popupContainer;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets value indicating whether popup container should be shown on button click.
		/// </summary>
		[DefaultValue( false )]
		public bool IsPopup
		{
			get
			{
				return m_bIsPopup;
			}
			set
			{
				m_bIsPopup = value;
			}
		}
		/// <summary>
		/// Gets or sets popup container for ScrollbarButton
		/// </summary>
		[DefaultValue( null )]
		public PopupControlContainer PopupContainer
		{
			get
			{
				return m_popupContainer;
			}
			set
			{
				m_popupContainer = value;
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Shows popup.
		/// </summary>
		/// <param name="e">EventArgs.</param>
		protected override void OnClick( EventArgs e )
		{
			base.OnClick( e );

			if( m_bIsPopup && m_popupContainer != null )
			{
				m_popupContainer.ShowPopup( this.PointToScreen( new Point( 0, this.Height ) ) );
			}
		}
		#endregion
	}
}