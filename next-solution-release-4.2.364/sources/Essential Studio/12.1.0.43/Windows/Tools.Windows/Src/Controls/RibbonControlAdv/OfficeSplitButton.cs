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
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary> ToolStripSplitButton for MenuDropDown. </summary>
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailability(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.None)]
	[ToolboxBitmap(typeof(OfficeSplitButton), "ToolboxIcons.OfficeSplitButton.bmp")]
	public class OfficeSplitButton : ToolStripSplitButton
	{
		#region Constants
		/// <summary> Default width of DropDownButton. </summary>
		const int DROPDOWN_BUTTON_WIDTH = 11;
		/// <summary> Default interval for Timer. </summary>
		const int TIMER_INT = 500;
		#endregion

		#region Initialization
		/// <summary> </summary>
		public OfficeSplitButton()
			:base()
		{
			m_timer = new Timer();
			m_timer.Interval = TIMER_INT;
			m_timer.Tick += new EventHandler( OnTimerTick );
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLayout( LayoutEventArgs e )
		{
			int iMaxButtonWidth = Height / 2;
			DropDownButtonWidth = ( iMaxButtonWidth < DROPDOWN_BUTTON_WIDTH ) ? DROPDOWN_BUTTON_WIDTH : iMaxButtonWidth;

			base.OnLayout( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mea"></param>
		protected override void OnMouseMove( MouseEventArgs mea )
		{
			SplitButtonSelected = ( this.ButtonBounds.Contains( mea.Location ) );

			base.OnMouseMove( mea );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave( EventArgs e )
		{
			m_timer.Stop();
			base.OnMouseLeave( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseEnter( EventArgs e )
		{
			m_timer.Start();
			base.OnMouseEnter( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDropDownOpened( EventArgs e )
		{
			OfficeDropDown dropDown = this.DropDown as OfficeDropDown;

			if( dropDown != null )
			{
				// Set parent renderer to DropDown.
				if( this.Parent != null )
				{
					RibbonControlAdvHeader.RibbonControlAdvHeaderRenderer renderer = this.Parent.Renderer as RibbonControlAdvHeader.RibbonControlAdvHeaderRenderer;

					if( renderer != null )
					{
						dropDown.Renderer = renderer;
					}
				}

				dropDown.CaptionText = DropDownText;
				dropDown.CaptionFont = DropDownFont;
			}

			m_timer.Stop();

			base.OnDropDownOpened( e );
		}
		/// <summary> Get DropDown location as TopLeft corner of AuxPanel. </summary>
		protected override Point DropDownLocation
		{
			get
			{
				MenuDropDown dropDown = this.Parent as MenuDropDown;

				if( dropDown != null )
				{
					Size szDropDown = dropDown.AuxItemsBounds.Size;

					if( szDropDown.Width > 0 && szDropDown.Height > 0 )
					{
						Point location = dropDown.PointToScreen( dropDown.AuxItemsBounds.Location );
						return location;
					}
				}
				
				return base.DropDownLocation;
			}
		}
		/// <summary> Base ToolStripDropDown creating replaced. </summary>
		/// <returns> New instance of DropDownEx. </returns>
		protected override ToolStripDropDown CreateDefaultDropDown()
		{
			return new OfficeDropDown( this );
		}
		#endregion

		#region Implementation
		/// <summary> Show SplitButton's DropDown. </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTimerTick( object sender, EventArgs e )
		{
			this.DropDown.OwnerItem = this;
			this.DropDown.Bounds = new Rectangle( this.DropDownLocation, this.Size );
			this.ShowDropDown();

			Invalidate();
			m_timer.Stop();
		}
		#endregion

		#region Properties
		/// <summary> Gets or sets if DropDownButton selected. </summary>
		internal bool SplitButtonSelected
		{
			get
			{
				return m_bSplitButtonSelected;
			}
			set
			{
				if( m_bSplitButtonSelected != value )
				{
					m_bSplitButtonSelected = value;
					Invalidate();
				}
			}
		}
		/// <summary> Gets or sets caption for DropDown. </summary>
		[ Category( "Appearance" ) ]
		[ DefaultValue( "" ) ]
		public string DropDownText
		{
			get
			{
				return m_sDropDownText;
			}
			set
			{
				m_sDropDownText = value;
			}
		}
		/// <summary> Gets or sets font for DropDown caption. </summary>
		[ Category( "Appearance" ) ]
		public Font DropDownFont
		{
			get
			{
				if( m_ftDropDownFont == null )
				{
					return m_ftDropDownFont = new Font( Control.DefaultFont, FontStyle.Bold );
				}

				return m_ftDropDownFont;
			}
			set
			{
				m_ftDropDownFont = value;
			}
		}
		#endregion

		#region ShouldSerialize & Reset methods
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeDropDownFont()
		{
			return ( m_ftDropDownFont != null );
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetDropDownFont()
		{
			m_ftDropDownFont = null;
		}
		#endregion

		#region Fields
		/// <summary> Indicates if DropDownButton selected. </summary>
		private bool m_bSplitButtonSelected = false;
		/// <summary> Timer for opening SplitButton's DropDown. </summary>
		private Timer m_timer;
		/// <summary> Caption for DropDown. </summary>
		private string m_sDropDownText = String.Empty;
		/// <summary> Font for DropDown caption. </summary>
		private Font m_ftDropDownFont = null;
		#endregion
	}
}

#endif