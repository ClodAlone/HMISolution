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
using System.Drawing;

using Syncfusion.Windows.Forms.Edit.Interfaces;

namespace Syncfusion.Windows.Forms.Edit.Implementation
{
	/// <summary>
	/// Cursor manager that supports scrolling.
	/// </summary>
	public class ScrollableCursorManager
		: CursorManager
	{
		#region Initialization And Finalization
		/// <summary>
		/// Creates new ScrollableCursorManager.
		/// </summary>
		/// <param name="owner">Owner of the cursor.</param>
		/// <param name="converter">IPositionConverter.</param>
		public ScrollableCursorManager( HybridScrollControl owner, IPositionConverter converter )
			: base( owner, converter )
		{
			if( owner == null ) throw new ArgumentNullException( "owner" );

			owner.VerticalScroll += new ScrollEventHandler( ProcessScrollEvent );
			owner.HorizontalScroll += new ScrollEventHandler( ProcessScrollEvent );
		}
		/// <summary>
		/// Disposes cursor manager, detaches all event handlers.
		/// </summary>
		public override void Dispose()
		{
			HybridScrollControl hsc = this.Owner as HybridScrollControl;
			if( hsc != null )
			{
				hsc.VerticalScroll -= new ScrollEventHandler( ProcessScrollEvent );
				hsc.HorizontalScroll -= new ScrollEventHandler( ProcessScrollEvent );
			}

			base.Dispose();
		}

		#endregion

		#region Event Handlers
		/// <summary>
		/// Handler of the VerticalScroll and HorizontalScroll events.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">ScrollEventArgs.</param>
		private void ProcessScrollEvent( object sender, ScrollEventArgs e )
		{
			if( e.Type == ScrollEventType.EndScroll )
			{
				Update();
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Updates position of the cursor.
		/// </summary>
		protected override void UpdatePosition()
		{
			if( this.Caret != null )
			{
				HybridScrollControl scroller = this.Owner as HybridScrollControl;
				if( scroller != null )
				{
					Point point = CursorGraphicalCoordinates.LeftTop;

					if(scroller.RightToLeft == RightToLeft.Yes)
						point.X += scroller.HScrollBar.Value;
					else
						point.X -= scroller.HScrollBar.Value;

					point.Y -= scroller.VScrollBar.Value;
					point.X += scroller.ScrollOffsetLeft;
					point.Y += scroller.ScrollOffsetTop;

					this.Caret.Position = point;
				}
			}
		}
		/// <summary>
		/// Updates visibility of the cursor.
		/// </summary>
		protected override void UpdateVisibility()
		{
			if( this.Caret != null )
			{
				HybridScrollControl scroller = this.Owner as HybridScrollControl;
				if( scroller != null )
				{
					Rectangle rect = new Rectangle( scroller.ScrollOffsetLeft, scroller.ScrollOffsetTop,
						scroller.ClientRectangle.Width - scroller.ScrollOffsetRight - scroller.ScrollOffsetLeft,
						scroller.ClientRectangle.Height - scroller.ScrollOffsetBottom - scroller.ScrollOffsetTop );

					Point endPosition = new Point( this.Caret.Position.X, this.Caret.Position.Y + this.CursorGraphicalCoordinates.Size.Height );
					this.Caret.Visible = this.Visible && ( rect.Contains( Caret.Position ) || rect.Contains( endPosition ) );
				}
			}
		}
		#endregion
	}
}
