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

#region file using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms
{
  /// <summary></summary>
	[ ToolboxItem( false ) ]
	public class VScrollBarCustomDraw :
		ScrollBarCustomDraw
	{
		#region class members
    /// <summary></summary>
		private const int DEF_MINTHUMB_HEIGHT = 7;
    /// <summary>
    /// Distance from thumb Y position to mouse down Y position.
    /// </summary>
		private int m_deltaY = 0;
    /// <summary>
    /// Flag, if true than controls visivble in other case not visible.
    /// </summary>
		private bool m_bIsControlsVisible = true;
    /// <summary>
    /// Default size of the vertical scroolbar.
    /// </summary>
		private Size m_defaultSize = new Size( 17, 80 );
		#endregion

		#region class properties
    /// <summary></summary>
		public new Size Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				if( Size != value )
				{
					base.Size = value;
				}
			}
		}
		#endregion

		#region class initialize\finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="VScrollBarCustomDraw"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal VScrollBarCustomDraw( ScrollersFrame owner ):
            base( owner )
		{
            this.Size = m_defaultSize;

            int heightControls = Accumulate( ControlsAfter ).Height + Accumulate( ControlsBefore ).Height;

            if( heightControls > Height - DEF_MINTHUMB_HEIGHT - 4 && Height > 0 )
            {
                m_IsSetControlsVisiblity = true;
                SetControlsLayoutVisibility( true );
                m_IsSetControlsVisiblity = false;
                m_bIsControlsVisible = false;
            }
            else
            {
                m_IsSetControlsVisiblity = true;
                SetControlsLayoutVisibility( false );
                m_IsSetControlsVisiblity = false;
                m_bIsControlsVisible = true;
            }			
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="VScrollBarCustomDraw"/> class.
        /// </summary>
        public VScrollBarCustomDraw():
			this( null )
        {
        }
		#endregion

		#region class overrides
    /// <summary>
    /// Occurs when control size changed.
    /// </summary>
    /// <param name="e"></param>
		protected override void OnSizeChanged( EventArgs e )
		{
			ValidateControlsVisibility();

			base.OnSizeChanged( e );
		}

    /// <summary>
    ///  Used for validating visibility of the ControlsAfter and ControlsBefore
    /// </summary>
    /// <returns></returns>
		protected override void ValidateControlsVisibility()
		{
			int heightControls = Accumulate( ControlsAfter ).Height + Accumulate( ControlsBefore ).Height;

			if( heightControls > Height - DEF_MINTHUMB_HEIGHT - 4 && Height > 0 )
			{
                m_IsSetControlsVisiblity = true;
                SetControlsLayoutVisibility( true );
                m_IsSetControlsVisiblity = false;
                m_bIsControlsVisible = false;
			}
			else
			{
                m_IsSetControlsVisiblity = true;
                SetControlsLayoutVisibility( false );
                m_IsSetControlsVisiblity = false;
                m_bIsControlsVisible = true;
			}		
		}

    /// <summary>
    /// Reset vertical scrollbar to default width.
    /// </summary>
		protected override void ResetToSystemMetrics()
		{
			this.Width = SystemInformation.VerticalScrollBarWidth;
		}

    /// <summary>
    /// Gets dockStyle for controls which situated before scroll.
    /// </summary>
    /// <returns></returns>
		protected override DockStyle GetAfterControlsDockStyle()
		{
			return ( IsRtl ) ? DockStyle.Top : DockStyle.Bottom;
		}

    /// <summary>
    /// Gets dockStyle for controls which situated after scroll.
    /// </summary>
    /// <returns></returns>
		protected override DockStyle GetBeforeControlsDockStyle()
		{
			return ( IsRtl ) ? DockStyle.Bottom : DockStyle.Top;
		}

    /// <summary>
    /// Gets value by cursor position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
		public override int PointToValue( int x, int y )
		{
			if( m_max == m_min )
			{
				return m_min;
			}

			// init helper variables
			int minY, maxY;

			// calculating min max positions
            if( this.IsRtl )
            {
                minY = m_rects[ ( int )PressedZone.ThumbRightZone ].Y;
                maxY = m_rects[ ( int )PressedZone.MinButton ].Y;
            }
            else
            {
				minY = m_rects[ ( int )PressedZone.ThumbLeftZone ].Y;
				maxY = m_rects[ ( int )PressedZone.MaxButton ].Y;
			}

			double ppv = ( double )( maxY - minY ) / ( m_max - m_min + 1 ); //pixel per 1 value
			double pixelLength = y - minY;

			return ( int )Math.Round( pixelLength / ppv + m_min );
		}

    /// <summary></summary>
		protected override void RecalculateArrow()
		{
			CalculateArrowsPositions();
		}

    /// <summary></summary>
		protected override void RecalculateThumb()
		{
			CalculateThumbSize();
		}

    /// <summary></summary>
    /// <returns></returns>
		protected override ScrollButton GetMaxButton()
		{
			return ( this.IsRtl ) ? ScrollButton.Up : ScrollButton.Down;
		}

    /// <summary></summary>
    /// <returns></returns>
		protected override ScrollButton GetMinButton()
		{
			return ( this.IsRtl ) ? ScrollButton.Down : ScrollButton.Up;
		}

    /// <summary></summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
		protected override void CalculatedDelta( int x, int y )
		{
			m_deltaY = y - m_rects[ ( int )PressedZone.Thumb ].Y;
		}

    /// <summary></summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
		protected override void OnMovedPositionChanged( int x, int y )
		{
			const int thumbIndx = ( int )PressedZone.Thumb;
			int min, max;

			// calculate min value and max value
			if( !this.IsRtl )
			{
				min = m_rects[ ( int )PressedZone.MinButton ].Y + m_rects[ ( int )PressedZone.MinButton ].Height;
				max = m_rects[ ( int )PressedZone.MaxButton ].Y;
			}
			else
			{
				min = m_rects[ ( int )PressedZone.MaxButton ].Y + m_rects[ ( int )PressedZone.MaxButton ].Height;
				max = m_rects[ ( int )PressedZone.MinButton ].Y;
			}

			if( y - m_deltaY < min )
			{
				m_rects[ thumbIndx ].Y = min;
			}
			else if( y - m_deltaY + m_rects[ thumbIndx ].Height > max )
			{
                int bottomPadding = 1; 
                int height = m_rects[thumbIndx].Height-bottomPadding;
                if (m_rects[thumbIndx].Height == DEF_MINTHUMB_HEIGHT)
                    height = 0;
				m_rects[ thumbIndx ].Y = max - height;
			}
			else
			{
                int padding = 0;
                if(m_rects[thumbIndx].Height == DEF_MINTHUMB_HEIGHT)
                    padding = m_rects[thumbIndx].Height;
                m_rects[thumbIndx].Y = y - m_deltaY + padding;
			}

			CalculateThumbLeftAndRightZone();
		}

    /// <summary></summary>
    /// <returns></returns>
		protected bool ShouldSerializeSize()
		{
			return Size != m_defaultSize;
		}

    /// <summary></summary>
		public void ResetSize()
		{
			Size = m_defaultSize;
		}
		#endregion

		#region class helper methods
    /// <summary></summary>
		private void CalculateArrowsPositions()
		{
			Size szAfter = Accumulate( ControlsAfter );
			Size szBefore = Accumulate( ControlsBefore );

            if( !m_bIsControlsVisible )
            {
                szAfter.Height = 0;
                szBefore.Height = 0;
            }

			int arrowYposition;
			int sysArrowHeight = SystemInformation.VerticalScrollBarArrowHeight;
			int scrollLength = Height - szAfter.Height - szBefore.Height;
			int arrowHeight = ( scrollLength / 2 < sysArrowHeight ) ? scrollLength / 2 : sysArrowHeight;

			if( this.IsRtl ) // RTL mode
			{
				// calculating bounds for min button
				arrowYposition = Height - szBefore.Height - arrowHeight;
				m_rects[ ( int )PressedZone.MinButton ] = new Rectangle( 0, arrowYposition, Width, arrowHeight );

				// calculating bounds for max button
				arrowYposition = szAfter.Height;
				m_rects[ ( int )PressedZone.MaxButton ] = new Rectangle( 0, arrowYposition, Width, arrowHeight );
			}
			else // NORMAL mode
			{
				// calculating bounds for min button
				arrowYposition = szBefore.Height;
				m_rects[ ( int )PressedZone.MinButton ] = new Rectangle( 0, arrowYposition, Width, arrowHeight );

				// calculating bounds for max button
				arrowYposition = Height - szAfter.Height - arrowHeight;
				m_rects[ ( int )PressedZone.MaxButton ] = new Rectangle( 0, arrowYposition, Width, arrowHeight );
			}
		}

    /// <summary></summary>
		private void CalculateThumbSize()
		{
			// initializing helper variables
			int positions = m_max - m_min + 1;
			int bgHeight = Math.Abs( m_rects[ ( int )PressedZone.MaxButton ].Y - m_rects[ ( int )PressedZone.MinButton ].Y ) - m_rects[ ( int )PressedZone.MinButton ].Height;
			double seed = ( double )bgHeight / positions;
			int thumbHeight = ( m_largeChange > 0 ) ? ( int )Math.Round( m_largeChange * seed ) : SystemInformation.VerticalScrollBarThumbHeight;
			

			// if thumb has very small size than thumb isn't displayed.
			if( thumbHeight < DEF_MINTHUMB_HEIGHT )
			{
                int bottomPadding = 1;
				thumbHeight = ( bgHeight > DEF_MINTHUMB_HEIGHT + 1 ) ? DEF_MINTHUMB_HEIGHT : 0;
				if(m_rects[(int)PressedZone.MaxButton].Y > m_rects[(int)PressedZone.MinButton].Y )
                    bgHeight = Math.Abs(m_rects[(int)PressedZone.MaxButton].Y - (m_rects[(int)PressedZone.MinButton].Y + m_rects[(int)PressedZone.MinButton].Height));
                else
                    bgHeight = Math.Abs(m_rects[(int)PressedZone.MinButton].Y - DEF_MINTHUMB_HEIGHT + bottomPadding - m_rects[(int)PressedZone.MaxButton].Y) - m_rects[(int)PressedZone.MinButton].Height;
                seed = (double)bgHeight / positions;
			}

            int thumbY = (int)Math.Round(seed * (m_value - m_min));

			// if this.IsRtl mode than reverse thumb
			if( this.IsRtl )
			{
				thumbY += m_rects[ ( int )PressedZone.MaxButton ].Bottom;
			}
			else
			{
				thumbY += m_rects[ ( int )PressedZone.MinButton ].Bottom;
			}

			m_rects[ ( int )PressedZone.Thumb ] = new Rectangle( 0, thumbY, Width, thumbHeight );

            if (m_rects[(int)PressedZone.MaxButton].Y > 0 && m_rects[(int)PressedZone.Thumb].Bottom > m_rects[(int)PressedZone.MaxButton].Y)
            {
                thumbY = m_rects[(int)PressedZone.MaxButton].Y - thumbHeight;
                m_rects[(int)PressedZone.Thumb] = new Rectangle(0, thumbY, Width, thumbHeight);
            }

			CalculateThumbLeftAndRightZone();
		}

    /// <summary>
    /// Calculating bounds for leftThumb and rightThumb zones for scroll.
    /// </summary>
		private void CalculateThumbLeftAndRightZone()
		{
			if( !this.IsRtl )
			{
				// calculating bounds of left rect in non this.IsRtl mode
				int thumbY = m_rects[ ( int )PressedZone.Thumb ].Y;
				int leftRectY = m_rects[ ( int )PressedZone.MinButton ].Y + m_rects[ ( int )PressedZone.MinButton ].Height;
				m_rects[ ( int )PressedZone.ThumbLeftZone ] = new Rectangle( 0, leftRectY, Width, thumbY - leftRectY );

				// calculating bounds of right rect in non this.IsRtl mode
				leftRectY = thumbY + m_rects[ ( int )PressedZone.Thumb ].Height;
				int rightRectHeight = m_rects[ ( int )PressedZone.MaxButton ].Y - leftRectY;
				m_rects[ ( int )PressedZone.ThumbRightZone ] = new Rectangle( 0, leftRectY, Width, rightRectHeight );
			}
			else
			{
				// calculating bounds of left rect in this.IsRtl mode
				int thumbY = m_rects[ ( int )PressedZone.Thumb ].Y;
				int leftRectY = thumbY + m_rects[ ( int )PressedZone.Thumb ].Height;
				int rightRectHeight = m_rects[ ( int )PressedZone.MinButton ].Y - leftRectY;
				m_rects[ ( int )PressedZone.ThumbLeftZone ] = new Rectangle( 0, leftRectY, Width, rightRectHeight );

				// calculating bounds of right rect in this.IsRtl mode
				leftRectY = m_rects[ ( int )PressedZone.MaxButton ].Y + m_rects[ ( int )PressedZone.MaxButton ].Height;
				m_rects[ ( int )PressedZone.ThumbRightZone ] = new Rectangle( 0, leftRectY, Width, thumbY - leftRectY );
			}
		}
		#endregion

		#region Event handlers
    /// <summary></summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
		protected override void OnScrollHereClick( Object sender, EventArgs e )
		{
			int iThumbHeight = m_rects[ ( int )PressedZone.Thumb ].Height;
			int iScrollHeight = iThumbHeight + m_rects[ ( int )PressedZone.ThumbLeftZone ].Height + m_rects[ ( int )PressedZone.ThumbRightZone ].Height;
			Point ptLocation = m_ptLocation; // NOTE: To avoid compile error CS0197 in VS2002.
            int iMiddleY = 0;

            if(m_min > 1)
                iMiddleY = ptLocation.Y;
            else
                iMiddleY = ptLocation.Y - m_rects[(int)PressedZone.ThumbLeftZone].Y - iThumbHeight / 2;

			if( iScrollHeight > 0 )
			{
				this.Value = ( int )( iMiddleY * ( m_max - m_min + 1 ) / iScrollHeight );

                OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, m_value));
			}
		}

    /// <summary></summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
		protected override void OnPageDownRightClick( object sender, EventArgs e )
		{
			Value += this.LargeChange;
			base.OnPageDownRightClick( sender, e );
		}

    /// <summary></summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
		protected override void OnPageUpLeftClick( object sender, EventArgs e )
		{
			Value -= this.LargeChange;
			base.OnPageUpLeftClick( sender, e );
		}
		#endregion
	}
}
