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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

using Syncfusion.Windows.Forms.Edit.Implementation.Parser;
using Syncfusion.Windows.Forms.Edit.Enums;

namespace Syncfusion.Windows.Forms.Edit
{
	/// <summary>
	/// Control that is used to render content of the StreamEditControl.
	/// </summary>
	[ToolboxItem( false )]
	public class FakeEditControl
		: IntelliScrollableControl
	{
		#region Fields
		/// <summary>
		/// Real edit control used as a source for the data.
		/// </summary>
		private StreamEditControl m_control;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bInScrollersUpdate;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets real edit control used as a source for the data.
		/// </summary>
		[
		Browsable( true ),
		Category( "Data" ),
		DefaultValue( null )
		]
		public StreamEditControl Control
		{
			get
			{
				return m_control;
			}
			set
			{
				if( m_control != null )
				{
					m_control.InvalidateArea -= new InvalidateAreaEventHandler( OnControlInvalidateArea );
					m_control.ScrollbarsSizeUpdated -= new EventHandler( OnControlScrollbarsSizeUpdated );
					m_control.OnFakeControlUnbinded( this );
				}

				m_control = value;

				if( null != m_control )
				{
					m_control.InvalidateArea += new InvalidateAreaEventHandler( OnControlInvalidateArea );
					m_control.ScrollbarsSizeUpdated += new EventHandler( OnControlScrollbarsSizeUpdated );
					m_control.OnFakeControlBinded( this );
				}

				Invalidate();
			}
		}
		/// <summary>
		/// Gets maximum width of the line. Used for WordWrapping.
		/// </summary>
		public int MaxWidth
		{
			get
			{
				return ( !m_control.WordWrap ) ?
					( int.MaxValue ) : ( this.ClientRectangle.Width - m_control.ScrollOffsetLeft - m_control.ScrollOffsetRight - 4 );
			}
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates and initializes new instance of the control.
		/// </summary>
		public FakeEditControl()
		{
			ControlStyles styleTrue =
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.UserMouse |
				ControlStyles.StandardClick |
				ControlStyles.StandardDoubleClick |
				ControlStyles.UserPaint;

			ControlStyles styleFalse = ControlStyles.CacheText | ControlStyles.Selectable;

			SetStyle( styleTrue, true );
			SetStyle( styleFalse, false );
		}
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );

			m_control = null;
		}

		#endregion

		#region Overrides
		/// <summary>
		/// Paints background and foreground of the control
		/// </summary>
		/// <param name="e">PaintEventArgs</param>
		protected override void OnPaint( PaintEventArgs e )
		{
			float maxWidth = 0;

			lock( this )
			{
				base.OnPaint( e );

				if( m_control != null )
				{
					try
					{
						m_control.ForFakeEdit = true;
						Point autoScrollPosition = this.AutoScrollPosition;
						Graphics g = e.Graphics;
						m_control.Parser.SetDPIFromGraphics( g );
						g.TextRenderingHint = m_control.GraphicsTextRenderingHint;
						g.CompositingQuality = m_control.GraphicsCompositingQuality;
						g.InterpolationMode = m_control.GraphicsInterpolationMode;
						g.SmoothingMode = m_control.GraphicsSmoothingMode;

						Rectangle clipRectangle = e.ClipRectangle;
						Rectangle drawRect = clipRectangle;
						m_control.FakeEditArea = this.ClientRectangle;
						clipRectangle.Height += 10;
						clipRectangle.Width += 10;
						g.SetClip( clipRectangle );

						if( this.ScrollOffsetRight > 0 )
						{
							RectangleF utilityRectRight =
								new RectangleF( this.ClientRectangle.Width - this.ScrollOffsetRight, 0, this.ScrollOffsetRight, this.ClientRectangle.Height );
							g.SetClip( utilityRectRight, CombineMode.Exclude );
						}

						// Translates drawing according current scrollers position.
						g.TranslateTransform( 0, autoScrollPosition.Y );
						drawRect.Y -= autoScrollPosition.Y;

						RenderedLine lineLastInRegion = m_control.Parser.GetLineByY( drawRect.Bottom );
						float fYAfterLastLine = 0;
						if( lineLastInRegion != null )
						{
							fYAfterLastLine = lineLastInRegion.Y + lineLastInRegion.Height;
						}

						m_control.DrawAreaBackground( g, this.ClientRectangle, drawRect, false );

						GraphicsState gs = g.Save();
						g.ResetTransform();
						g.SetClip( clipRectangle );
						Rectangle margRect = m_control.DrawUserMarginArea( g, this.ClientRectangle );
						g.Restore( gs );
						g.SetClip( margRect, CombineMode.Exclude );
						if( RightToLeft != RightToLeft.Yes )
							drawRect.Width -= this.ScrollOffsetRight;

						m_control.DrawTextArea( g, this.ClientRectangle );
						//g.SetClip( drawRect );
						RenderedLine lastLine = m_control.DrawArea(
							g, drawRect, -autoScrollPosition.X, out maxWidth, true, true, false, this.AutoScrollPosition.Y, 1, Size.Empty );
						g.ResetTransform();
						g.SetClip( clipRectangle );

						// If line position has been chaged during drawing and remeasuring,
						// then we should re-invalidate area, that is next to the last drawn line in region.
						if( lineLastInRegion != null && fYAfterLastLine != ( lineLastInRegion.Y + lineLastInRegion.Height ) )
						{
							float fNextLineY = lastLine.Y + lastLine.Height + autoScrollPosition.Y;
							Rectangle rectReInvalidate =
								new Rectangle( 0, ( int )fNextLineY, this.ClientRectangle.Width, ( int )( ClientRectangle.Height - fNextLineY ) );
							Invalidate( rectReInvalidate );
						}
					}
					catch( Exception exc )
					{
						Debug.WriteLine( "Paint failed, exception thrown :" + exc.Message );
						Debug.WriteLine( "Source:" );
						Debug.WriteLine( exc.Source );
						Debug.WriteLine( "Stack:" );
						Debug.WriteLine( exc.StackTrace );
						throw exc;
					}
					finally
					{
						e.Graphics.ResetTransform();
						m_control.ForFakeEdit = false;
					}

					//if( !m_control.WordWrap || m_control.WrapMode != WordWrapMode.Control )
					//{
					//  // Update scrollers.
					//  int iMaxWidth = ( int )Math.Ceiling( maxWidth )/* - AutoScrollPosition.X*/;
					//  int newWidth = VirtualSize.Width;

					//  if( newWidth > MaxWidth )
					//  {
					//    newWidth = MaxWidth;
					//  }

					//  if( iMaxWidth > newWidth )
					//  {
					//    newWidth = iMaxWidth;
					//  }

					//  newWidth = Math.Max( newWidth, m_control.VirtualSize.Width );

					//  if( VirtualSize.Width != newWidth )
					//  {
					//    VirtualSize = new Size( newWidth, VirtualSize.Height );
					//  }
					//}
					//else
					//{
					//  VirtualSize = new Size( 0, VirtualSize.Height );
					//}

					this.VirtualSize = m_control.VirtualSize;

					UpdateScrollBarsVisibility();
					m_control.UpdateScrollerVerticalSize( this, false );
				}
			}
		}
		/// <summary>
		/// Processes vertical scroll event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="se">ScrollEventArgs.</param>
		protected override void OnVScroll( object sender, ScrollEventArgs se )
		{
			lock( this )
			{
				int yAmount = ( this.VScrollBar.Value - se.NewValue );
				Rectangle bounds = new System.Drawing.Rectangle( 1, this.ScrollOffsetTop + 1,
					this.ScrollOffsetLeft - 1, this.ClientRectangle.Height - this.ScrollOffsetTop - this.ScrollOffsetBottom - 2 );
				this.ScrollWindow( 0, yAmount, bounds, bounds );
				bounds = new System.Drawing.Rectangle( this.ClientRectangle.Width - this.ScrollOffsetRight + 1, this.ScrollOffsetTop + 1,
					this.ScrollOffsetRight - 2, this.ClientRectangle.Height - this.ScrollOffsetTop - this.ScrollOffsetBottom - 2 );
				this.ScrollWindow( 0, yAmount, bounds, bounds);

				base.OnVScroll( sender, se );
			}
		}
		/// <summary>
		/// Scrolls control vertically by specified amount of lines.
		/// </summary>
		/// <param name="fLinesCount">Count of lines to scroll.</param>
		/// <param name="direction">Direction of scrolling.</param>
		protected override void ScrollLines( ScrollDirection direction, float fLinesCount )
		{
			lock( this )
			{
				RenderedLine firstLine = m_control.InternalGetLineByAbsoluteY(
							( ( direction == ScrollDirection.Up ) ? ( 0 ) : ( ClientRectangle.Bottom - 1 ) ) - this.AutoScrollPosition.Y );
				if( firstLine != null )
				{
					float fScrollAmount = 0;
					fLinesCount = Math.Min( fLinesCount,
						( direction == ScrollDirection.Up ) ? ( firstLine.LineIndex - 1 ) : ( m_control.Parser.TotalLines - firstLine.LineIndex ) );
					int iLinesCount = ( int )Math.Floor( fLinesCount );
					ScrollEventType type = ScrollEventType.SmallDecrement;

					if( direction == ScrollDirection.Up )
					{
						type = ScrollEventType.SmallDecrement;
						fScrollAmount = ( firstLine.Y + this.AutoScrollPosition.Y );

						if( fLinesCount - iLinesCount > 0 )
						{
							fScrollAmount -= m_control.InternalGetLine( firstLine.LineIndex - iLinesCount - 1, false ).Height * ( fLinesCount - iLinesCount );
						}
						fScrollAmount += m_control.InternalGetLine( firstLine.LineIndex - iLinesCount, false ).Y - firstLine.Y;
					}
					else
					{
						type = ScrollEventType.SmallIncrement;
						fScrollAmount = ( firstLine.Y + firstLine.Height + this.AutoScrollPosition.Y - ClientRectangle.Height );

						if( fLinesCount - iLinesCount > 0 )
						{
							fScrollAmount +=
								( m_control.Parser.GetLine( firstLine.LineIndex - iLinesCount + 1 ) as RenderedLine ).Height * ( fLinesCount - iLinesCount );
						}

						for( int i = firstLine.LineIndex + 1; i <= firstLine.LineIndex + iLinesCount; i++ )
						{
							fScrollAmount += m_control.InternalGetLine( i, true ).Height;
						}
					}

					float iValue = VScrollBar.Value;
					float newValue = iValue + ( int )Math.Ceiling( fScrollAmount );
					newValue = Math.Max( 0, Math.Min( newValue, VirtualSize.Height - VScrollBar.LargeChange + 1 ) );
					ScrollEventArgs args = new ScrollEventArgs( type, 0 );
					int moveSign = Math.Sign( newValue - iValue );

					if( moveSign != 0 )
					{
						float oneMoveSize = Math.Abs( ( float )( iValue - newValue ) / fLinesCount /
							( Math.Max( StreamEditControl.DEF_LINE_SCROLLING_STEP_FAST - ( fLinesCount - 3 ) / 3 * 2, 1 ) ) );

						do
						{
							iValue += oneMoveSize * moveSign;
							args.NewValue = ( int )Math.Ceiling( ( moveSign > 0 ) ? ( Math.Min( iValue, newValue ) ) : ( Math.Max( iValue, newValue ) ) );
							OnVScrollInternal( this, args );

							Update();
						}
						while( ( iValue - newValue ) * moveSign <= 0 );
					}
				}
			}
		}
		/// <summary>
		/// Updates visibility of the ScrollBars.
		/// </summary>
		protected internal override void UpdateScrollBarsVisibility()
		{
			if( !m_bInScrollersUpdate )
			{
				m_bInScrollersUpdate = true;

				this.DisableHorizontalScroller = m_control.DisableHorizontalScroller;
				this.DisableVerticalScroller = m_control.DisableVerticalScroller;

				if( !m_control.DisableScrollers )
				{
					if( m_control.AlwaysShowScrollers )
					{
						this.VScroll = this.HScroll = true;
					}
					else
					{
						bool bShowGripper = m_control.CheckForGripper( this );

						if( bShowGripper )
						{
							this.VScroll = !this.DisableVerticalScroller;
							this.HScroll = !this.DisableHorizontalScroller;
						}
						else
						{
							base.UpdateScrollBarsVisibility();
						}
					}
				}
				else
				{
					this.HScroll = this.VScroll = false;
				}

				m_bInScrollersUpdate = false;
			}
		}
        /// <summary>
        /// Overrides the OnRighttoLeftchanged
        /// </summary>
        /// <param name="e"></param>
		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);

			this.HScrollBar.RightToLeft = this.RightToLeft;
			this.VScrollBar.RightToLeft = this.RightToLeft;
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Invalidates control.
		/// </summary>
		/// <param name="initiator"></param>
		/// <param name="areaToInvalidate"></param>
		private void OnControlInvalidateArea( StreamEditControl initiator, Rectangle areaToInvalidate )
		{
			areaToInvalidate.X += AutoScrollPosition.X;
			areaToInvalidate.Y += AutoScrollPosition.Y;
			if( areaToInvalidate.Width == int.MaxValue )
			{
				areaToInvalidate.Width -= areaToInvalidate.X + 1;
			}

			if( areaToInvalidate.Height == int.MaxValue )
			{
				areaToInvalidate.Height -= areaToInvalidate.Y + 1;
			}

			areaToInvalidate.Intersect( ClientRectangle );

			if( areaToInvalidate.Width != 0 && areaToInvalidate.Height != 0 )
			{
				Invalidate( areaToInvalidate );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnControlScrollbarsSizeUpdated( object sender, EventArgs e )
		{
			m_control.UpdateScrollerVerticalSize( this, false );
			int maxWidth = Math.Max( this.VirtualSize.Width, m_control.VirtualSize.Width );
			if( this.VirtualSize.Width != maxWidth )
			{
				this.VirtualSize = new Size( maxWidth, this.VirtualSize.Height );
			}
		}
		#endregion
	}
}