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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary> Extended DropDown. </summary>
	internal class OfficeDropDown : ToolStripDropDown
	{
		#region Constants
		/// <summary> Separator width. </summary>
		private const int SEPARATOR_WIDTH = 2;
		/// <summary> Margin between text and Text border in Caption. </summary>
		private const int TEXT_MARGIN_IN_CAPTION = 5;
		/// <summary> Width of scroll button that used for scrolling items in Panel. </summary>
		private const int SCROLL_BUTTON_HEIGHT = 12;
		/// <summary> Interval for timer. </summary>
		private const int TIMER_INT = 200;
		/// <summary> Width of scroll button that used for scrolling items in Panel. </summary>
		private const int MIN_CAPTION_HEIGHT = 21;
		#endregion

		#region Enums
		/// <summary>
		/// Different areas of the control.
		/// </summary>
		protected enum ScrollButtonsArea
		{
			/// <summary> Out of scroll buttons. </summary>
			None,
			/// <summary> Down scroll button. </summary>
			DownScrollButton,
			/// <summary> Up scroll button. </summary>
			UpScrollButton
		}
		#endregion

		#region Initialization
		public OfficeDropDown( ToolStripItem item )
			: base()
		{
			this.OwnerItem = item;

			m_timer = new Timer();
			m_timer.Tick += new EventHandler( OnTimerTick );
		}
		#endregion

		#region Nested classes

		#region DropDownExLayoutEngine
		/// <summary>
		/// Layout engine for DropDownEx.
		/// </summary>
		private class DropDownExLayoutEngine : LayoutEngine
		{
			#region Overrides
			/// <summary>
			/// Lays out toolstrip items in StatusStripEx.
			/// </summary>
			/// <param name="container"></param>
			/// <param name="layoutEventArgs"></param>
			/// <returns></returns>
			public override bool Layout( object container, LayoutEventArgs layoutEventArgs )
			{
				OfficeDropDown parent = ( OfficeDropDown )container;

				int iItemY = parent.DisplayRectangle.Y + parent.CaptionHeight;

				if( parent.ScrollPositionInternal > 0 )
				{
					iItemY += SCROLL_BUTTON_HEIGHT;
				}

				int iParentBottom = parent.DisplayRectangle.Bottom;
				int iItemsHeight = parent.GetItemsHeight();

				if( iItemsHeight - parent.ScrollPositionInternal > parent.DisplayRectangle.Height )
				{
					iParentBottom -= SCROLL_BUTTON_HEIGHT;
				}

				int iLeft = parent.Padding.Left;
				int iTop = parent.Padding.Top + parent.CaptionHeight - parent.ScrollPositionInternal;

				int iWidth = parent.Width - parent.Padding.Horizontal;

				bool bEnough = false;

				foreach( ToolStripItem item in parent.Items )
				{
					if( item.Available )
					{
						if (item.AutoSize)
						{
							item.Height = item.GetPreferredSize(Size.Empty).Height;
							item.Width = iWidth - item.Margin.Horizontal;
						}

						int iItemTop = iTop + item.Height + item.Margin.Vertical;

						bool bWhole = iItemTop <= iParentBottom;
						bool bAbove = !( iItemTop > iItemY );

						if( bAbove )
						{
							parent.SetItemLocation( item, new Point( -item.Width - 1, iTop + item.Margin.Top ) );
							iTop += item.Height + item.Margin.Vertical;
						}
						else if( !bWhole || bEnough )
						{
							bEnough = ( iItemTop > iItemY );
							parent.SetItemLocation( item, new Point( -item.Width - 1, iTop + item.Margin.Top ) );
						}
						else
						{
							parent.SetItemLocation( item, new Point( iLeft + item.Margin.Left, iTop + item.Margin.Top ) );
							iTop += item.Height + item.Margin.Vertical;
						}
					}
				}

				return parent.AutoSize;
			}
			#endregion 
		}
		#endregion

		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			RibbonControlAdvHeader.RibbonControlAdvHeaderRenderer renderer = this.Renderer as RibbonControlAdvHeader.RibbonControlAdvHeaderRenderer;

			if( renderer != null )
			{
				int iItemsHeight = GetItemsHeight();
				int iItemsNeededHeight = Bounds.Height;

				if( iItemsHeight > iItemsNeededHeight )
				{
					// Choose needed Scroll buttons to further painting.
					m_bIsUpScroll = ( ScrollPositionInternal > 0 );
					m_bIsDownScroll = ( iItemsHeight - ScrollPositionInternal > iItemsNeededHeight );

					if( m_bIsDownScroll )
					{
						// Draw down scroll button over the items.
						renderer.DrawScrollButtonOnDropDown( this, e.Graphics, DownScrollBounds, true );
					}
					else
					{
						m_bDownScrollSelected = false;
					}

					if( m_bIsUpScroll )
					{
						// Draw up scroll button under the items.
						renderer.DrawScrollButtonOnDropDown( this, e.Graphics, UpScrollBounds, false );
					}
					else
					{
						m_bUpScrollSelected = false;
					}
				}
				else
				{
					m_bIsDownScroll = false;
					m_bIsUpScroll = false;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			// Do not process Mouse Move if cursor not in DropDown bounds
			// and unselect selected Scroll button.
			
			if( !ClientRectangle.Contains( e.Location ) )
			{
				if( m_bDownScrollSelected )
				{
					m_bDownScrollSelected = false;
					RefreshScroll();
				}

				if( m_bUpScrollSelected )
				{
					m_bUpScrollSelected = false;
					RefreshScroll();
				}

				return;
			}

			bool bDownScrollSelected = false;
			bool bUpScrollSelected = false;

			// If mouse over down scroll button than highlight it.
			if( m_bIsDownScroll )
			{
				bDownScrollSelected = DownScrollBounds.Contains( e.Location );

				if( bDownScrollSelected != m_bDownScrollSelected )
				{
					m_bDownScrollSelected = bDownScrollSelected;
					RefreshScroll();
				}
			}
			// If mouse over left scroll button than highlight it.
			if( m_bIsUpScroll && !bDownScrollSelected )
			{
				bUpScrollSelected = UpScrollBounds.Contains( e.Location );

				if( bUpScrollSelected != m_bUpScrollSelected )
				{
					m_bUpScrollSelected = bUpScrollSelected;
					RefreshScroll();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );

			if( m_bDownScrollSelected )
			{
				m_bDownScrollSelected = false;
				RefreshScroll();
			}
			else if( m_bUpScrollSelected )
			{
				m_bUpScrollSelected = false;
				RefreshScroll();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			bool bIsBaseMouseDown = true;

			if( e.Button == MouseButtons.Left )
			{
				bool bMouseDown = false;

				if( m_bIsDownScroll && DownScrollBounds.Contains( e.Location ) )
				{
					bIsBaseMouseDown = false;
					bMouseDown = true;
					Capture = true;

					PushedButton = ScrollButtonsArea.DownScrollButton;
					ScrollToDown();
					StartTimer( ScrollButtonsArea.DownScrollButton );
				}
				if( !bMouseDown && ( m_bIsUpScroll && UpScrollBounds.Contains( e.Location ) ) )
				{
					bIsBaseMouseDown = false;
					Capture = true;

					PushedButton = ScrollButtonsArea.UpScrollButton;
					ScrollToUp();
					StartTimer( ScrollButtonsArea.UpScrollButton );
				}
			}

			if( bIsBaseMouseDown )
			{
				base.OnMouseDown( e );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseCaptureChanged( EventArgs e )
		{
			base.OnMouseCaptureChanged( e );

			m_timer.Stop();
			PushedButton = ScrollButtonsArea.None;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="proposedSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize( Size proposedSize )
		{
			Size szResult = Size.Empty;
			Size szItem = Size.Empty;

			foreach( ToolStripItem item in this.Items )
			{
				if( item.Available )
				{
					szItem = GetItemSize( item );

					szResult.Height += szItem.Height;

					if( szResult.Width < szItem.Width )
					{
						szResult.Width = szItem.Width;
					}
				}
			}

			szResult.Height += this.CaptionHeight;

			szResult.Width += this.Padding.Horizontal;
			szResult.Height += this.Padding.Vertical;

			ToolStripItem tsItem = this.OwnerItem;

			if( tsItem != null )
			{
				// Set Bounds of DropDown as Auxiliary panel bounds.
				MenuDropDown menuDropDown = tsItem.GetCurrentParent() as MenuDropDown;

				if( menuDropDown != null )
				{
					Size szAuxItems = menuDropDown.AuxItemsBounds.Size;

					int iAuxItemsWidth = szAuxItems.Width;
					int iAuxItemsHeight = szAuxItems.Height;

					if( iAuxItemsWidth > 0 && iAuxItemsHeight > 0 )
					{
						if( iAuxItemsWidth > szResult.Width )
						{
							szResult.Width = iAuxItemsWidth;
						}

						szResult.Height = iAuxItemsHeight;
					}
				}
			}

			return szResult;
		}
		#endregion

		#region Implementation
		/// <summary> Call RedrawWindow method to Repaint Scroll buttons. </summary>
		private void RefreshScroll()
		{
			RedrawWindowFlags flags = RedrawWindowFlags.RDW_FRAME | RedrawWindowFlags.RDW_INVALIDATE;
			WindowsAPI.RedrawWindow( this.Handle, IntPtr.Zero, IntPtr.Zero, flags );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private Size GetItemSize( ToolStripItem item )
		{
			Size szItem;

			szItem = item.AutoSize ? item.GetPreferredSize( Size.Empty ) : item.Size;

			szItem.Height += item.Margin.Vertical;
			szItem.Width += item.Margin.Horizontal;

			return szItem;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal int GetItemsHeight()
		{
			int iHeight = 0;

			foreach( ToolStripItem item in this.Items )
			{
				if( item.Available )
				{
					Size szItem = GetItemSize( item );

					iHeight += szItem.Height;
				}
			}

			iHeight += CaptionHeight;

			return iHeight;
		}
		/// <summary> Process position in Layout for items n Panel and if it not in right bounds that set correct value to it. </summary>
		/// <param name="position"> Position to process. </param>
		/// <returns></returns>
		private int GetValidScrollPosition( int position )
		{
			int iValue = position;

			if( iValue != 0 )
			{
				int iItemHeight = GetItemsHeight();
				int iHeight = Bounds.Height;

				if( iValue < 0 || iItemHeight < iHeight )
				{
					iValue = 0;
				}
			}

			return iValue;
		}
		/// <summary> Move controls to down according to scroll position and their location. </summary>
		private void ScrollToDown()
		{
			if( m_iItemIndex + 1 > Items.Count - 1 )
			{
				return;
			}
			else
			{
				int iScrollButtonHeight = 0;

				if( !m_bIsUpScroll )
				{
					iScrollButtonHeight = SCROLL_BUTTON_HEIGHT;
				}

				ScrollPositionInternal = GetValidScrollPosition( ScrollPositionInternal + Items[ m_iItemIndex ].Height + Items[ m_iItemIndex ].Margin.Vertical - iScrollButtonHeight );
				m_iItemIndex++;
			}
		}
		/// <summary> Move controls to up according to scroll position and their location. </summary>
		private void ScrollToUp()
		{
			if( m_iItemIndex - 1 < 0 )
			{
				return;
			}
			else
			{
				ScrollPositionInternal = GetValidScrollPosition( ScrollPositionInternal - Items[ m_iItemIndex - 1 ].Height - Items[ m_iItemIndex - 1 ].Margin.Vertical );
				m_iItemIndex--;
			}
		}
		/// <summary> Initializes & starts timer. </summary>
		/// <param name="mousePushedArea">Area where mouse was pushed and caused timer to start.</param>
		private void StartTimer( ScrollButtonsArea mousePushedArea )
		{
			m_timer.Interval = m_timerInt * 4;
			m_timer.Tag = mousePushedArea;
			m_timer.Start();
		}
		/// <summary> Handles mouse keeping pushed. </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTimerTick( object sender, EventArgs e )
		{
			ScrollButtonsArea area = ( ScrollButtonsArea )( ( ( Timer )sender ).Tag );

			Point p = PointToClient( Control.MousePosition );

			switch( area )
			{
				case ScrollButtonsArea.DownScrollButton:
					if( DownScrollBounds.IsEmpty )
					{
						this.Capture = false;
					}
					else if( DownScrollBounds.Contains( p ) )
					{
						ScrollToDown();
					}
					break;

				case ScrollButtonsArea.UpScrollButton:
					if( UpScrollBounds.IsEmpty )
					{
						this.Capture = false;
					}
					else if( UpScrollBounds.Contains( p ) )
					{
						ScrollToUp();
					}
					break;
			}

			m_timer.Interval = m_timerInt;
		}
		/// <summary>
		/// 
		/// </summary>
		private void OnLayoutChanged()
		{
			m_iCaptionHeight = -1;

			if( IsHandleCreated )
			{
				PerformLayout();
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public override LayoutEngine LayoutEngine
		{
			get
			{
				if( m_layoutEngine == null )
					m_layoutEngine = new DropDownExLayoutEngine();

				return m_layoutEngine;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected override Padding DefaultPadding
		{
			get
			{
				return new Padding( 1, 2, 1, 2 );
			}
		}
		/// <summary> Area in which user pushed mouse button. </summary>
		protected ScrollButtonsArea PushedButton
		{
			get
			{
				return m_pushedButton;
			}
			set
			{
				if( m_pushedButton != value )
				{
					m_pushedButton = value;
				}
			}
		}
		/// <summary> Get bounds of up scroll button. </summary>
		protected Rectangle UpScrollBounds
		{
			get
			{
				Rectangle rc = Rectangle.Empty;

				if( m_bIsUpScroll )
				{
					Rectangle rcBounds = Bounds;

					Point pt = PointToClient( new Point( rcBounds.X, rcBounds.Y + CaptionHeight ) );
					rc = new Rectangle( pt, new Size( rcBounds.Width, SCROLL_BUTTON_HEIGHT ) );
				}

				return rc;
			}
		}
		/// <summary> Get bounds of down scroll button. </summary>
		protected Rectangle DownScrollBounds
		{
			get
			{
				Rectangle rc = Rectangle.Empty;

				if( m_bIsDownScroll )
				{
					Rectangle rcBounds = Bounds;
					Point pt = PointToClient( new Point( rcBounds.X, rcBounds.Bottom - SCROLL_BUTTON_HEIGHT ) );

					rc = new Rectangle( pt, new Size( rcBounds.Width, SCROLL_BUTTON_HEIGHT ) );
				}

				return rc;
			}
		}
		/// <summary> Gets or sets position of TopMost Item. </summary>
		internal int ScrollPositionInternal
		{
			get
			{
				return m_iScrollPosition;
			}
			set
			{
				m_iScrollPosition = value;
				this.PerformLayout();
			}
		}
		/// <summary> Gets value that indicates if down scroll button is selected. </summary>
		internal bool DownScrollSelected
		{
			get
			{
				return m_bDownScrollSelected;
			}
			set
			{
				m_bDownScrollSelected = value;
			}
		}
		/// <summary> Gets value that indicates if up scroll button is selected. </summary>
		internal bool UpScrollSelected
		{
			get
			{
				return m_bUpScrollSelected;
			}
			set
			{
				m_bUpScrollSelected = value;
			}
		}
		/// <summary> Gets or sets caption height. </summary>
		internal int CaptionHeight
		{
			get
			{
				if( m_iCaptionHeight == -1 )
				{
					int height = TextRenderer.MeasureText( m_sCaptionText, m_ftCaptionFont ).Height;

					if( height > 0 )
					{
						if( height < MIN_CAPTION_HEIGHT )
						{
							height = MIN_CAPTION_HEIGHT;
						}

						height += SEPARATOR_WIDTH;
					}

					m_iCaptionHeight = height;
				}

				return m_iCaptionHeight;
			}
			set
			{
				m_iCaptionHeight = value;
			}
		}
		/// <summary> Gets text for caption. </summary>
		internal string CaptionText
		{
			get
			{
				return m_sCaptionText;
			}
			set
			{
				m_sCaptionText = value;

				OnLayoutChanged();
			}
		}
		/// <summary> Gets font for caption. </summary>
		internal Font CaptionFont
		{
			get
			{
				return m_ftCaptionFont;
			}
			set
			{
				m_ftCaptionFont = value;

				OnLayoutChanged();
			}
		}
		#endregion

		#region Fields
		/// <summary> Instance of DropDownExLayoutEngine. </summary>
		private DropDownExLayoutEngine m_layoutEngine;
		/// <summary> Position of rightmost tab Item. </summary>
		private int m_iScrollPosition = 0;
		/// <summary> </summary>
		private bool m_bIsUpScroll;
		/// <summary> </summary>
		private bool m_bIsDownScroll;
		/// <summary> Indicates if up scroll button is selected. </summary>
		private bool m_bUpScrollSelected = false;
		/// <summary> Indicates if down scroll button is selected. </summary>
		private bool m_bDownScrollSelected = false;
		/// <summary> Timer for handling mouse keeping pushed. </summary>
		private Timer m_timer;
		/// <summary> Interval for timer. </summary>
		private int m_timerInt = TIMER_INT;
		/// <summary> Currently pushed button. </summary>
		private ScrollButtonsArea m_pushedButton;
		/// <summary> Bounds of panel. </summary>
		private Rectangle m_rcBounds = Rectangle.Empty;
		/// <summary> Index of first showed item. </summary>
		private int m_iItemIndex = 0;
		/// <summary> Caption height. </summary>
		private int m_iCaptionHeight = -1;
		/// <summary> Text for caption. </summary>
		private string m_sCaptionText;
		/// <summary> Font for caption. </summary>
		private Font m_ftCaptionFont;
		#endregion
	}
}

#endif