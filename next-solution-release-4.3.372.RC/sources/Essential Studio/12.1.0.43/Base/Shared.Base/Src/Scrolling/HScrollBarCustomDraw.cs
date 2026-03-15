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
    [ToolboxItem( false )]
    public class HScrollBarCustomDraw:
        ScrollBarCustomDraw
    {
        #region class members
        /// <summary></summary>
        private const int DEF_MINTHUMB_WIDTH = 7;
        /// <summary></summary>
        private int m_deltaX = 0;
        /// <summary>
        /// Flag, if true than controls visivble in other case not visible.
        /// </summary>
        private bool m_bIsControlsVisible = true;
        /// <summary>
        /// Default size of the horizontal scroolbar.
        /// </summary>
        private Size m_defaultSize = new Size( 80, 17 );
        #endregion

        #region class properties
        /// <summary></summary>
        public new int Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                if( !this.KeepSystemMetrics && Width != value )
                {
                    base.Width = value;
                }
            }
        }
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
        /// Initializes a new instance of the <see cref="HScrollBarCustomDraw"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal HScrollBarCustomDraw( ScrollersFrame owner ) :
            base( owner )
        {
            this.Size = m_defaultSize;

            int widthControls = Accumulate( ControlsAfter ).Width + Accumulate( ControlsBefore ).Width;

            if( widthControls > Width - DEF_MINTHUMB_WIDTH - 4 && Width > 0 )
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
        /// Initializes a new instance of the <see cref="HScrollBarCustomDraw"/> class.
        /// </summary>
        public HScrollBarCustomDraw():
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
        /// Used for validating visibility of the ControlsAfter and ControlsBefore
        /// </summary>
        /// <returns>
        /// TODO: place correct comment here
        /// </returns>
        protected override void ValidateControlsVisibility()
        {
            int widthControls = Accumulate( ControlsAfter ).Width + Accumulate( ControlsBefore ).Width;

            if( widthControls > Width - DEF_MINTHUMB_WIDTH - 4 && Width > 0 )
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
        /// Reset horizontal scrollbar to default height.
        /// </summary>
        protected override void ResetToSystemMetrics()
        {
            this.Height = SystemInformation.HorizontalScrollBarHeight;
        }

        /// <summary>
        /// Gets dockStyle for controls which situated before scroll.
        /// </summary>
        /// <returns></returns>
        protected override DockStyle GetAfterControlsDockStyle()
        {
            return ( IsRtl ) ? DockStyle.Left : DockStyle.Right;
        }

        /// <summary>
        /// Gets dockStyle for controls which situated after scroll.
        /// </summary>
        /// <returns></returns>
        protected override DockStyle GetBeforeControlsDockStyle()
        {
            return ( IsRtl ) ? DockStyle.Right : DockStyle.Left;
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
            int minX, maxX;

            // calculating min max positions
            if( this.IsRtl )
            {
                minX = m_rects[(int)PressedZone.ThumbRightZone].X;
                maxX = m_rects[(int)PressedZone.MinButton].X;
            }
            else
            {
                minX = m_rects[(int)PressedZone.ThumbLeftZone].X;
                maxX = m_rects[(int)PressedZone.MaxButton].X;
            }

            double ppv = (double)( maxX - minX ) / ( m_max - m_min + 1 ); //pixel per 1 value
            double pixelLength = x - minX;

            return (int)Math.Round( pixelLength / ppv + m_min );
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
            return ( this.IsRtl ) ? ScrollButton.Left : ScrollButton.Right;
        }

        /// <summary></summary>
        /// <returns></returns>
        protected override ScrollButton GetMinButton()
        {
            return ( this.IsRtl ) ? ScrollButton.Right : ScrollButton.Left;
        }

        /// <summary></summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected override void OnMovedPositionChanged( int x, int y )
        {
            int min, max;
            const int thumbIndx = (int)PressedZone.Thumb;

            // calculate min value and max value
            if( !this.IsRtl )
            {
                min = m_rects[(int)PressedZone.MinButton].X + m_rects[(int)PressedZone.MinButton].Width;
                max = m_rects[(int)PressedZone.MaxButton].X;
            }
            else
            {
                min = m_rects[(int)PressedZone.MaxButton].X + m_rects[(int)PressedZone.MaxButton].Width;
                max = m_rects[(int)PressedZone.MinButton].X;
            }

            if( x - m_deltaX < min )
            {
                m_rects[thumbIndx].X = min;
            }
            else if( x - m_deltaX + m_rects[thumbIndx].Width > max )
            {
                int width = m_rects[thumbIndx].Width;
                if (m_rects[thumbIndx].Width == DEF_MINTHUMB_WIDTH)
                    width = 0;
                m_rects[thumbIndx].X = max - width; 

            }
            else
            {
                int padding = 0;
                if (m_rects[thumbIndx].Width == DEF_MINTHUMB_WIDTH)
                    padding = m_rects[thumbIndx].Width;
                m_rects[thumbIndx].X = x - m_deltaX + padding;
            }

            CalculateThumbLeftAndRightZone();
        }

        /// <summary></summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected override void CalculatedDelta( int x, int y )
        {
            m_deltaX = x - m_rects[(int)PressedZone.Thumb].X;
        }

        /// <summary></summary>
        /// <returns></returns>
        protected bool ShouldSerializeSize()
        {
            return this.Size != m_defaultSize;
        }

        /// <summary></summary>
        public void ResetSize()
        {
            this.Size = m_defaultSize;
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
                szAfter.Width = 0;
                szBefore.Width = 0;
            }

            int arrowXposition;
            int sysArrowWidth = SystemInformation.HorizontalScrollBarArrowWidth;
            int scrollLength = Width - szAfter.Width - szBefore.Width;
            int arrowWidth = ( scrollLength / 2 < sysArrowWidth ) ? scrollLength / 2 : sysArrowWidth;

            if( this.IsRtl ) // RTL mode
            {
                // calculating bounds for min button
                arrowXposition = Width - szBefore.Width - arrowWidth;
                m_rects[(int)PressedZone.MinButton] = new Rectangle( arrowXposition, 0, arrowWidth, Height );

                // calculating bounds for max button
                arrowXposition = szAfter.Width;
                m_rects[(int)PressedZone.MaxButton] = new Rectangle( arrowXposition, 0, arrowWidth, Height );
            }
            else // NORMAL mode
            {
                // calculating bounds for min button
                arrowXposition = szBefore.Width;
                m_rects[(int)PressedZone.MinButton] = new Rectangle( arrowXposition, 0, arrowWidth, Height );

                // calculating bounds for max button
                arrowXposition = Width - szAfter.Width - arrowWidth;
                m_rects[(int)PressedZone.MaxButton] = new Rectangle( arrowXposition, 0, arrowWidth, Height );
            }
        }

        /// <summary>
        /// Calculates bounds of thumb for scroll control.
        /// </summary>
        private void CalculateThumbSize()
        {
            // initializing helper variables
            int positions = m_max - m_min + 1;
            int bgWidth = Math.Abs( m_rects[(int)PressedZone.MaxButton].X -
				m_rects[(int)PressedZone.MinButton].X ) -
				m_rects[(int)PressedZone.MinButton].Width;

            double seed = (double)bgWidth / positions;
            int thumbWidth = ( m_largeChange > 0 ) ?
				(int)Math.Round( m_largeChange * seed ) :
				SystemInformation.HorizontalScrollBarThumbWidth;

            int thumbX = (int)Math.Round( seed * ( m_value - m_min ) );

            // if thumb has very small size than thumb isn't displayed.
            if( thumbWidth < DEF_MINTHUMB_WIDTH )
            {
                thumbWidth = ( bgWidth > DEF_MINTHUMB_WIDTH + 1 ) ? DEF_MINTHUMB_WIDTH : 0;
            }

			// if this.IsRtl mode than reverse thumb
            if( this.IsRtl )
            {
                thumbX += m_rects[(int)PressedZone.MaxButton].Right;
            }
            else
            {
                thumbX += m_rects[(int)PressedZone.MinButton].Right;
            }

            m_rects[(int)PressedZone.Thumb] = new Rectangle( thumbX, 0, thumbWidth, Height );

            CalculateThumbLeftAndRightZone();
        }

        /// <summary>
        /// Calculating bounds for leftThumb and rightThumb zones for scroll.
        /// </summary>
        private void CalculateThumbLeftAndRightZone()
        {
            if( this.IsRtl ) // RTL mode
            {
                // calculating bounds of left rect in this.IsRtl mode
                int thumbX = m_rects[(int)PressedZone.Thumb].X;
                int leftRectX = thumbX + m_rects[(int)PressedZone.Thumb].Width;
                int rightRectWidth = m_rects[(int)PressedZone.MinButton].X - leftRectX;
                m_rects[(int)PressedZone.ThumbLeftZone] = new Rectangle( leftRectX, 0, rightRectWidth, Height );

                // calculating bounds of right rect in this.IsRtl mode
                leftRectX = m_rects[(int)PressedZone.MaxButton].X + m_rects[(int)PressedZone.MaxButton].Width;
                m_rects[(int)PressedZone.ThumbRightZone] = new Rectangle( leftRectX, 0, thumbX - leftRectX, Height );
            }
            else // NORMAL mode
            {
                // calculating bounds of left rect in non this.IsRtl mode
                int thumbX = m_rects[(int)PressedZone.Thumb].X;
                int leftRectX = m_rects[(int)PressedZone.MinButton].X +
					m_rects[(int)PressedZone.MinButton].Width;

                m_rects[(int)PressedZone.ThumbLeftZone] = new Rectangle( leftRectX, 0, thumbX - leftRectX, Height );

                // calculating bounds of right rect in non this.IsRtl mode
                leftRectX = thumbX + m_rects[(int)PressedZone.Thumb].Width;
                int rightRectWidth = m_rects[(int)PressedZone.MaxButton].X - leftRectX;
                m_rects[(int)PressedZone.ThumbRightZone] = new Rectangle( leftRectX, 0, rightRectWidth, Height );
            }
        }
        #endregion

        #region Event handlers
        /// <summary></summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnScrollHereClick( object sender, EventArgs e )
        {
            int iThumbWidth = m_rects[(int)PressedZone.Thumb].Width;
            int iScrollWidth = iThumbWidth + m_rects[(int)PressedZone.ThumbLeftZone].Width + m_rects[(int)PressedZone.ThumbRightZone].Width;
            Point ptLocation = m_ptLocation; // To avoid compile error CS0197 in VS2002.
            int scrollPoint = PointToValue(ptLocation.X, ptLocation.Y);
            int iMiddleX = 0;
            int scrollValue = 0;
            if(m_min > 1)
            {
                iMiddleX = ptLocation.X;
                scrollValue = (int)(iMiddleX * (m_max - m_min + 1) / iScrollWidth);
                this.Value = scrollValue == scrollPoint ? scrollValue : scrollPoint;
                this.Value -= iThumbWidth;
                if (this.Value < scrollValue)
                {
                    this.Value = scrollPoint;
                }
            }
            else
            {
                iMiddleX = ptLocation.X - m_rects[(int)PressedZone.ThumbLeftZone].X - iThumbWidth / 2;
                this.Value = (int)(iMiddleX * (m_max - m_min + 1) / iScrollWidth);
            }

            if( iScrollWidth > 0 )
            {
                OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, m_value));
            }
        }

        /// <summary></summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnPageDownRightClick( object sender, EventArgs e )
        {
            this.Value += this.LargeChange;
            base.OnPageDownRightClick( sender, e );
        }

        /// <summary></summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnPageUpLeftClick( object sender, EventArgs e )
        {
            this.Value -= this.LargeChange;
            base.OnPageUpLeftClick( sender, e );
        }
        #endregion
    }
}