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

#region File Using
using System;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Collections;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.ComponentModel;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	/// <summary>
	/// This renderer is used to draw toolbar with chevron.
	/// </summary>
	public class XpToolBarChevronRenderer : SingleLineBarRenderer, IPopupParent
	{
		#region Class constants
		// Used to calculate chevron button bounds.
		private int DEF_CHEVRON_WIDTH
		{
			get
			{
				int width = ( this.ShouldDrawThemed ) ? 13 : 10;

				return width;

			}
		}
    
		private int DEF_MENU_OFFSET
		{
			get
			{
				int offset = 0;

				if( this.Style == VisualStyle.Office2003 
					|| this.Style == VisualStyle.VS2005 
					|| this.Style == VisualStyle.Office2007
                    || this.Style == VisualStyle.Office2010)
				{
					offset = ( this.ShouldDrawThemed ) ?  1 : 2;
				}

				return offset;

			}
		}

		private const int DEF_CHEVRON_RIGHT_BORDER_OFFSET = 2;
		private const int DEF_CHEVRON_BORDERS_OFFSET = ChevronPainter.DEF_BORDER_WIDTH;
		private const int DEF_BORDER_WIDTH = ChevronPainter.DEF_BORDER_WIDTH;		

		/// <summary>
		/// Angle to rotate transform for vertical algnment.
		/// </summary>
		private const int DEF_ROTETE_ANGLE = 90;
		#endregion
        
		#region Class members
		/// <summary>
		/// Used to determine invisible items count.
		/// </summary>
		private bool m_bNeedRecalculateVisibleItems = false;

		/// <summary>
		/// ToolBar this renderer is rendering.
		/// </summary>
		private XPToolBar m_toolBar = null;

		/// <summary>
		/// Used to determine state of chevron button.
		/// </summary>
		private ButtonsState m_buttonState = ButtonsState.None;

		/// <summary>
		/// Indicates whether chevron button is still pushed.
		/// </summary>
		private bool m_bIsChevronButtonPushed = false;

		/// <summary>
		/// Invisible items are displayed in this menu if chevron button is pressed.
		/// </summary>
		private PopupMenu m_hiddenItemsMenu = null;
		#endregion

		#region Class events
		/// <summary>
		/// Occurs when chevron button is pushed.
		/// </summary>
		public event MouseEventHandler ChevronMouseDown;
		#endregion

		#region Class properties
		protected Rectangle ChevronButtonBounds
		{
			get
			{
				Rectangle bounds = Rectangle.Empty;

				int left = this.IsRightToLeft ? 
					m_toolBar.Left + DEF_CHEVRON_RIGHT_BORDER_OFFSET :
					m_toolBar.Width - DEF_CHEVRON_WIDTH - DEF_CHEVRON_RIGHT_BORDER_OFFSET;          

				if( this.IsVerticallyAligned )
				{
					left = this.IsRightToLeft ? 
						m_toolBar.Top + DEF_CHEVRON_RIGHT_BORDER_OFFSET :
						m_toolBar.Height - DEF_CHEVRON_WIDTH - DEF_CHEVRON_RIGHT_BORDER_OFFSET;

					bounds = new Rectangle( 0, left, m_toolBar.Width, DEF_CHEVRON_WIDTH );
					bounds.Inflate( -DEF_CHEVRON_BORDERS_OFFSET * 2, 0 );
				}
				else
				{
					bounds = new Rectangle( left, 0, DEF_CHEVRON_WIDTH, m_toolBar.Height );
					bounds.Inflate( 0, -DEF_CHEVRON_BORDERS_OFFSET * 2 );
				}

				return bounds;
			}
		}     

		protected virtual bool ShouldDrawThemed
		{
			get
			{
				bool bShouldDrawThemed = ( XPThemes.IsThemedOS && XPThemes.IsThemeActive && 
					this.m_toolBar != null && this.m_toolBar.ThemesEnabled );

				return bShouldDrawThemed;
			}
		}

		protected bool ShouldShowChevron
		{
			get
			{
				return ( this.InvisibleBarItems != null && this.InvisibleBarItems.Count > 0 );
			}
		}

		/// <summary>
		/// Returns the menu, hidden items are shown in, when chevron is pressed.
		/// </summary>
		protected PopupMenu HiddenItemsMenu
		{
			get
			{
				if( m_hiddenItemsMenu == null )
				{
					InitializeMenu();
				}

				return m_hiddenItemsMenu;
			}
		}

		protected bool IsRightToLeft
		{
			get
			{
				bool bIsRightToLeft = ( m_toolBar != null &&
					m_toolBar.RightToLeft == RightToLeft.Yes );

				return bIsRightToLeft;
			}

		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Creates a new instance of this class.
		/// </summary>
		/// <param name="toolBar"></param>
        public XpToolBarChevronRenderer( XPToolBar toolBar )
			: base( toolBar )
		{
			if( toolBar == null )
				throw new ArgumentNullException( "toolBar" );

			m_toolBar = toolBar;
		}
		#endregion

		#region Class overrides

		public override void GetPreferredSize(IGraphicsProvider gp, ref SizeF preferredSize)
		{
			base.GetPreferredSize( gp, ref preferredSize );

			if( this.IsVerticallyAligned )
			{
				preferredSize = new SizeF( preferredSize.Height, preferredSize.Width );
			}
		}

		public override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			// if hidden items menu is not currently showing , determine state of chevron button.
			if( !this.HiddenItemsMenu.IsShowing() )
			{
				Point mousePos = new Point( e.X, e.Y );
				m_buttonState = GetButtonState( mousePos, m_bIsChevronButtonPushed );
			}

			InvalidateChevron();
		}

		public override bool OnMouseDown( MouseEventArgs e )
		{
			Point mousePos = new Point( e.X, e.Y );
			m_buttonState = GetButtonState( mousePos, true );

			bool handled = false;

			if( this.ShouldShowChevron &&  m_buttonState == ButtonsState.DropDownButtonPushed )
			{       
				m_bIsChevronButtonPushed = true;
				OnChevronMouseDown( e );
				handled = true;
			}
			else
			{
				if( this.HiddenItemsMenu.IsShowing() )
				{
					this.HiddenItemsMenu.Hide();
				}

				handled = base.OnMouseDown( e );
			}
            
			InvalidateChevron();

			return handled;
		}

		
		public override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );

			//      m_bIsChevronButtonPushed = false;
			//      Point mousePos = new Point( e.X, e.Y );
			//      m_buttonState = GetButtonState( mousePos, m_bIsChevronButtonPushed );

			InvalidateChevron();      
		}

		public override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );

			if( !this.HiddenItemsMenu.IsShowing() )
			{
				m_buttonState = ButtonsState.None;
			}

			InvalidateChevron();
		}

		public override void OnPaint( Graphics g, Rectangle clipRect )
		{
			base.OnPaint( g, clipRect );

			if( ShouldShowChevron )
			{
				this.DrawChevronButton( g );
			}
		}

		public override void AfterComputeBarItemPositions( IGraphicsProvider gp )
		{
			// Chevron button is shown only if at least one bar item is invisible.
			// in this case we need to recalculate other invisible items, taking into account
			// chevron button bounds, which now becomes visible.

			if( !m_bNeedRecalculateVisibleItems && this.InvisibleBarItems.Count > 0 )
			{
				m_bNeedRecalculateVisibleItems = true;
				ComputeBarItemPositions( gp );
			}

			m_bNeedRecalculateVisibleItems = false;
		}

		
		protected override RectangleF CorrectDisplayBounds( RectangleF bounds )
		{
			// skip this corrections, if recalculating first time and all bar items are visible.
			if( m_bNeedRecalculateVisibleItems )
			{
				int offset = DEF_CHEVRON_WIDTH + DEF_CHEVRON_RIGHT_BORDER_OFFSET;

				if( this.IsRightToLeft )
				{                
					bounds.Offset( offset, 0 );
				}
				else
				{
					offset += DEF_CHEVRON_BORDERS_OFFSET;
				}

				bounds.Width -= ( offset );
			}

			return bounds;
		}


		protected virtual void OnChevronMouseDown( MouseEventArgs e )
		{
			ShowDropDown();

			RaiseChevronMouseDown( e );
		}


		protected virtual PopupRelativeAlignment GetFirstAlignPreference()
		{
			bool bIsRightToLeft = this.IsRightToLeft;

			PopupRelativeAlignment aligment = bIsRightToLeft ? 
				PopupRelativeAlignment.BottomRight: 
				PopupRelativeAlignment.BottomLeft;

			if( this.IsVerticallyAligned )
			{
				if( bIsRightToLeft )
				{
					aligment = ( this.Alignment == CommandBarDockState.Right ) ? 
						PopupRelativeAlignment.TopLeft : PopupRelativeAlignment.TopRight;
				}
				else
				{
					aligment = ( this.Alignment == CommandBarDockState.Right ) ? 
						PopupRelativeAlignment.BottomLeft : PopupRelativeAlignment.BottomRight;
				}
			}

			return aligment;
		}

		protected override CustomizingPopupMenu CreateCustomizationPopup()
		{
			return ( null != this.Bar.Manager ) ? new BarControlCustomizingMenu() : null;
		}

		protected override bool ProcessKeyDown( Keys key )
		{
			bool bHandled = base.ProcessKeyDown( key );

			if( key == Keys.Tab )
			{
				bHandled = false;
			}

			return bHandled;
		}

		#endregion
        
		#region Class Public Methods
		/// <summary>
		/// Converts the point in screen coordinates to client coordinates.
		/// </summary>
		/// <param name="pt">The point to convert.</param>
		/// <returns>Point in client coordinates.</returns>
        public Point PointToClient( Point pt )
		{
            Point convertedPoint = new Point( int.MinValue, int.MinValue );
			if( m_toolBar != null )
			{
				convertedPoint = m_toolBar.PointToClient( pt );
			}

			return convertedPoint;
		}
		#endregion

		#region Class utility methods
		private void InitializeMenu()
		{
			m_hiddenItemsMenu = new PopupMenu();
			m_hiddenItemsMenu.ParentBarItem = new ParentBarItem();
		}

		/// <summary>
		/// Shows menu with hidden items.
		/// </summary>
		protected void ShowDropDown()
		{
			if( !this.HiddenItemsMenu.IsShowing() )
			{
				ParentBarItem parentItem = this.HiddenItemsMenu.ParentBarItem;
				parentItem.Style = this.Style;
				parentItem.Items.Clear();
                  
				if( this.InvisibleBarItems != null && this.InvisibleBarItems.Count > 0 )
				{
					for( int i = 0, len = this.InvisibleBarItems.Count; i < len; i++ )
					{
						parentItem.Items.Add( this.InvisibleBarItems[ i ] );
					}
				}

				this.HiddenItemsMenu.ShowChildrenUI( Point.Empty, this );
			}
		}	

		protected void InvalidateChevron()
		{
			Rectangle dropDownBounds = this.ChevronButtonBounds;
			dropDownBounds.Inflate( DEF_BORDER_WIDTH, DEF_BORDER_WIDTH );

			m_toolBar.Invalidate( dropDownBounds );
		}


		/// <summary>
		/// Returns the appropriate chevron button state, according to mouse position and whether the mouse button pushed.
		/// </summary>
		/// <param name="pt"> Mouse Position in client coordinates.</param>
		/// <param name="pressed"> True for mouse button pushed state; false otherwise. </param>
		private ButtonsState GetButtonState( Point pt, bool pressed )
		{
			ButtonsState state = ButtonsState.None;

			if( this.ChevronButtonBounds.Contains( pt ) )
			{
				state = ( pressed ) ? ButtonsState.DropDownButtonPushed : 
					ButtonsState.DropDownButtonHot;
			}
			else
			{
				state = ButtonsState.None;
			}

			return state;
		}

		private void DrawChevronButton( Graphics g )
		{
			GraphicsState state = g.Save();
			Rectangle chevronBounds = this.ChevronButtonBounds;

			if( this.IsVerticallyAligned )
			{
				chevronBounds = GetRotatedRect( chevronBounds );
				g.TranslateTransform( this.Bounds.Width, 0 );
				g.RotateTransform( DEF_ROTETE_ANGLE );
			}

			VisualStyle style = this.Style;

			if( !this.ShouldDrawThemed && style != VisualStyle.Office2003 
				&& style != VisualStyle.VS2005
				&& style != VisualStyle.Office2007
                && style != VisualStyle.Office2010)
			{
				style = VisualStyle.Default;
			}
			else if( this.ShouldDrawThemed && 
				( style == VisualStyle.Office2003 
				|| style == VisualStyle.VS2005
				|| style == VisualStyle.Office2007
                || style == VisualStyle.Office2010))
			{
				style = VisualStyle.OfficeXP;
			}

			ChevronPainter.DrawChevronButton( g, chevronBounds, style, 
				m_buttonState, this.IsRightToLeft );

			g.Restore( state );
		}

		/// <summary>
		/// Gets rotated rectangle.
		/// </summary>
		private Rectangle GetRotatedRect( Rectangle rect )
		{
			Rectangle rotateRect = Rectangle.Empty;

			if( rect != Rectangle.Empty )
			{
				rotateRect = new Rectangle( rect.Y, rect.X, rect.Height, rect.Width );
			}

			return rotateRect;
		}	

		#endregion

		#region Class Event Raisers
		protected void RaiseChevronMouseDown( MouseEventArgs e )
		{
			if( ChevronMouseDown != null )
			{
				ChevronMouseDown( this, e );
			}
		}
		#endregion		
    
		#region IPopupParent Members
		public override void ChildClosing( IPopupChild childUI, PopupCloseType popupCloseType )
		{
			base.ChildClosing( childUI, popupCloseType );

			if( m_bIsChevronButtonPushed )
			{ 
				ParentBarItem parentItem = this.HiddenItemsMenu.ParentBarItem;
				BarItems invisibleItems = parentItem.Items;
				if( invisibleItems != null && invisibleItems.Count > 0 )
				{
					invisibleItems.Clear();
				}
				m_bIsChevronButtonPushed = false;
				Point mousePos = this.m_toolBar.PointToClient( Control.MousePosition );
				this.m_buttonState = GetButtonState( mousePos, m_bIsChevronButtonPushed );
      
				InvalidateChevron();
			}
		}

		public override Point[] GetBorderOverlapCue( PopupRelativeAlignment relativeAlignment )
		{
			Point[] arrPoints = null;
			if( m_buttonState == ButtonsState.DropDownButtonPushed )
			{
				Rectangle chevronBounds = this.ChevronButtonBounds;

				chevronBounds.Width += DEF_MENU_OFFSET;
				chevronBounds.Height += DEF_MENU_OFFSET;
				chevronBounds = m_toolBar.RectangleToScreen( chevronBounds );


				arrPoints = PopupUtils.ComputeDefaultBorderOverlapCue( relativeAlignment, 
					chevronBounds );
			}
			else
			{
				arrPoints = base.GetBorderOverlapCue( relativeAlignment );
			}

			return arrPoints;
		}

		public override Point GetLocationForPopupAlignment( PopupRelativeAlignment prevAlignment, 
			out PopupRelativeAlignment newAlignment )
		{
			Point location = Point.Empty;

			if( m_buttonState == ButtonsState.DropDownButtonPushed )
			{
				Rectangle chevronBounds = this.ChevronButtonBounds;
				chevronBounds.Width += DEF_MENU_OFFSET;
				chevronBounds.Height += DEF_MENU_OFFSET;

				location = PopupUtils.ComputeDefaultPopupAlignment( prevAlignment, 
					out newAlignment, this.GetFirstAlignPreference(),
					PopupRelativeAlignment.RightTop, chevronBounds );

				location = this.m_toolBar.PointToScreen( location );
			}
			else
			{
				location = base.GetLocationForPopupAlignment( prevAlignment, out newAlignment );
			}

			return location;
		}
		#endregion		

		#region IPopupItem Members
		public override Control GetPopupParentControl()
		{
			Control popupParent = ( m_buttonState == ButtonsState.DropDownButtonPushed ) ? 
				this.m_toolBar : base.GetPopupParentControl();
			return popupParent;
		}

		public override bool IsRelatedControl( Control control, bool askPopupParent )
		{
			bool bIsRelatedControl = ( m_buttonState == ButtonsState.DropDownButtonPushed ) ?
				( control == this.m_toolBar ) : base.IsRelatedControl( control, askPopupParent );

			return bIsRelatedControl;
		}

		#endregion
	}
}
