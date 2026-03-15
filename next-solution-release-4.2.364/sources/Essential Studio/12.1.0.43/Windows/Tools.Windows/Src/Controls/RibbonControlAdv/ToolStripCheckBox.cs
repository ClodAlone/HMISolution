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
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary> 
	/// ToolStripCheckBox class, that represents checkbox item on ToolStrip.
	/// </summary>
	[ToolboxItem( false )]
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailability( System.Windows.Forms.Design.ToolStripItemDesignerAvailability.ToolStrip)]
	[ToolboxBitmap(typeof(Syncfusion.Windows.Forms.Tools.ToolStripCheckBox), "ToolboxIcons.CheckBoxItem.bmp")]
	public class ToolStripCheckBox : ToolStripButton
	{
		#region Constants
		/// <summary>
		/// 
		/// </summary>
		internal const int UNCHECKED = 0;
		/// <summary>
		/// 
		/// </summary>
		internal const int UNCHECKED_SELECTED = 1;
		/// <summary>
		/// 
		/// </summary>
		internal const int UNCHECKED_PRESSED = 2;
		/// <summary>
		/// 
		/// </summary>
		internal const int CHECKED = 3;
		/// <summary>
		/// 
		/// </summary>
		internal const int CHECKED_SELECTED = 4;
		/// <summary>
		/// 
		/// </summary>
		internal const int CHECKED_PRESSED = 5;
		/// <summary>
		/// 
		/// </summary>
		internal const int CHECKED_DISABLED = 6;
		/// <summary>
		/// 
		/// </summary>
		internal const int UNCHECKED_DISABLED = 7;
        /// <summary>
        /// 
        /// </summary>
        private bool metroCheckbox = false;
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		static ToolStripCheckBox()
		{
            Bitmap bmpCheckMarkStates;
            Bitmap bmpMetroCheckMarkStates;
            m_imglstCheckMarkStates = new ImageList();
            m_imglstMetroCheckMarkStates = new ImageList();
            m_imglstCheckMarkStates.ColorDepth = ColorDepth.Depth32Bit;
            m_imglstMetroCheckMarkStates.ColorDepth = ColorDepth.Depth32Bit;
            Bitmap bit = new Bitmap(10, 10);
            using (Graphics g = Graphics.FromImage(bit))
            {
                if (g.DpiX > 120)
                {
                    bmpCheckMarkStates = new Bitmap(typeof(ToolStripCheckBox).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.150DPIToolStripCheckBox.bmp"));
                    bmpMetroCheckMarkStates = new Bitmap(typeof(ToolStripCheckBox).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.150DPIMetroToolStripCheckBox.bmp"));
                    m_imglstCheckMarkStates.ImageSize = new Size(16, 16);
                    m_imglstMetroCheckMarkStates.ImageSize = new Size(16, 16);
                }
                else if (g.DpiX > 96)
                {
                    bmpCheckMarkStates = new Bitmap(typeof(ToolStripCheckBox).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.125DPIToolStripCheckBox.bmp"));
                    bmpMetroCheckMarkStates = new Bitmap(typeof(ToolStripCheckBox).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.125DPIMetroToolStripCheckBox.bmp"));
                    m_imglstCheckMarkStates.ImageSize = new Size(12, 12);
                    m_imglstMetroCheckMarkStates.ImageSize = new Size(12, 12);
                }
                else
                {
                    bmpCheckMarkStates = new Bitmap(typeof(ToolStripCheckBox).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.ToolStripCheckBox.bmp"));
                    bmpMetroCheckMarkStates = new Bitmap(typeof(ToolStripCheckBox).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.MetroToolStripCheckBox.bmp"));
                    m_imglstCheckMarkStates.ImageSize = new Size(9, 9);
                    m_imglstMetroCheckMarkStates.ImageSize = new Size(9, 9);
                }
            }
			m_imglstCheckMarkStates.Images.AddStrip( bmpCheckMarkStates );
            m_imglstMetroCheckMarkStates.Images.AddStrip(bmpMetroCheckMarkStates);
		}
		/// <summary>
		/// 
		/// </summary>
		public ToolStripCheckBox()
			: base()
		{
		}
		#endregion

		#region Nested classes
		/// <summary>
		/// Class for layout information of a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
		/// </summary>
		internal class CheckBoxInternalLayout
		{
			#region Constants
			/// <summary>
			/// Minimum CheckMark height.
			/// </summary>
			private const int MIN_CHECKMARK_HEIGHT = 13;
			/// <summary>
			/// Minimum CheckMark width.
			/// </summary>
			private const int MIN_CHECKMARK_WIDTH = 13;
			/// <summary>
			/// Control border width.
			/// </summary>
			private const int BORDER_WIDTH = 2;
			/// <summary>
			/// Offset from CheckMark to Text.
			/// </summary>
			private const int CHECKMARK_OFFSET = 2;
			#endregion

			#region Initialization
			/// <summary>
			/// 
			/// </summary>
			/// <param name="item"></param>
			public CheckBoxInternalLayout( ToolStripCheckBox item )
			{
				m_Item = item;
			}
			#endregion

			#region Implementation
			/// <summary>
			/// Gets Y-coordinate of check mark part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			/// <returns></returns>
			private int GetCoordinateYOfCheckMark()
			{
				int iY = 0;

				switch( m_Item.CheckAlign )
				{
					case ContentAlignment.TopCenter:
					case ContentAlignment.TopLeft:
					case ContentAlignment.TopRight:
						iY = BORDER_WIDTH + m_Item.Padding.Top;
						break;

					case ContentAlignment.MiddleCenter:
					case ContentAlignment.MiddleLeft:
					case ContentAlignment.MiddleRight:
						int iHeight = m_Item.Height - 2 * BORDER_WIDTH - m_Item.Padding.Vertical;
						iY = m_Item.Padding.Top + BORDER_WIDTH + ( iHeight - MIN_CHECKMARK_HEIGHT ) / 2;
						break;

					case ContentAlignment.BottomCenter:
					case ContentAlignment.BottomLeft:
					case ContentAlignment.BottomRight:
						iY = m_Item.Height - BORDER_WIDTH - m_Item.Padding.Bottom - MIN_CHECKMARK_HEIGHT;
						break;
				}

				return iY;
			}
			/// <summary>
			/// Gets X-coordinate of check mark part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			/// <returns></returns>
			private int GetCoordinateXOfCheckMark()
			{
				int iX = 0;
				bool bRightToLeft = m_Item.RightToLeft == RightToLeft.Yes;

				switch( m_Item.CheckAlign )
				{
					case ContentAlignment.TopLeft:
					case ContentAlignment.MiddleLeft:
					case ContentAlignment.BottomLeft:
						iX = bRightToLeft ?
							m_Item.Width - BORDER_WIDTH - m_Item.Padding.Right - MIN_CHECKMARK_WIDTH :
							BORDER_WIDTH + m_Item.Padding.Left;
						break;

					case ContentAlignment.TopCenter:
					case ContentAlignment.MiddleCenter:
					case ContentAlignment.BottomCenter:
						int iWidth = m_Item.Width - 2 * BORDER_WIDTH - m_Item.Padding.Horizontal;
						iX = m_Item.Padding.Left + BORDER_WIDTH + ( iWidth - MIN_CHECKMARK_WIDTH ) / 2;
						break;

					case ContentAlignment.TopRight:
					case ContentAlignment.MiddleRight:
					case ContentAlignment.BottomRight:
						iX = bRightToLeft ?
							BORDER_WIDTH + m_Item.Padding.Left :
							m_Item.Width - BORDER_WIDTH - m_Item.Padding.Right - MIN_CHECKMARK_WIDTH;
						break;
				}

				return iX;
			}
			/// <summary>
			/// Gets Y-coordinate of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			/// <returns></returns>
			private int GetCoordinateYOfText()
			{
				int iY = BORDER_WIDTH + m_Item.Padding.Top;

				if( m_Item.CheckAlign == ContentAlignment.TopCenter )
				{
					iY += CheckMarkSize.Height + CHECKMARK_OFFSET;
				}

				return iY;
			}
			/// <summary>
			/// Gets X-coordinate of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			/// <returns></returns>
			private int GetCoordinateXOfText()
			{
				int iX = BORDER_WIDTH + m_Item.Padding.Left;
                if (m_Item.ImageCheckBox)
                    iX += m_Item.ImageCheckBoxSize.Width - MIN_CHECKMARK_WIDTH ;
				if( m_Item.RightToLeft == RightToLeft.Yes )
				{
					switch( m_Item.CheckAlign )
					{
						case ContentAlignment.BottomRight:
						case ContentAlignment.MiddleRight:
						case ContentAlignment.TopRight:
							iX += CheckMarkSize.Width + CHECKMARK_OFFSET;
							break;
					}
				}
				else
				{
					switch( m_Item.CheckAlign )
					{
						case ContentAlignment.BottomLeft:
						case ContentAlignment.MiddleLeft:
						case ContentAlignment.TopLeft:
							iX += CheckMarkSize.Width + CHECKMARK_OFFSET;
							break;
					}
				}

				return iX;
			}
			/// <summary>
			/// Gets height of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			/// <returns></returns>
			private int GetHeightOfText()
			{
				int iHeight = m_Item.Height - 2 * BORDER_WIDTH - m_Item.Padding.Vertical;

				if( GetIsAlignmentInTwoLines() )
				{
					iHeight -= ( CheckMarkSize.Height + CHECKMARK_OFFSET );
				}

				return iHeight;
			}
			/// <summary>
			/// Gets width of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			/// <returns></returns>
			private int GetWidthOfText()
			{
				int iWidth = m_Item.Width - 2 * BORDER_WIDTH - m_Item.Padding.Horizontal;

				if( !( GetIsAlignmentInTwoLines() || m_Item.CheckAlign == ContentAlignment.MiddleCenter ) )
				{
					iWidth -= ( CheckMarkSize.Width + CHECKMARK_OFFSET );
				}

				return iWidth;
			}
			#endregion

			#region Properties
			/// <summary>
			/// Get preferred size of a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>. 
			/// </summary>
			public Size PreferredSize
			{
				get
				{
					if( m_Item != null )
					{
						if( m_szLastPreferred == Size.Empty )
						{
							int iWidth = 2 * BORDER_WIDTH + m_Item.Padding.Horizontal;
							int iHeight = 2 * BORDER_WIDTH + m_Item.Padding.Vertical;

							int iCheckMarkHeight = CheckMarkSize.Height;
							int iCheckMarkWidth = CheckMarkSize.Width;
							int iTextHeight = TextSize.Height;

							if( GetIsAlignmentInTwoLines() )
							{
								iHeight += iCheckMarkHeight + iTextHeight + CHECKMARK_OFFSET;
								iWidth += Math.Max( TextSize.Width, iCheckMarkWidth );
							}
							else
							{
								iWidth += iCheckMarkWidth + TextSize.Width + CHECKMARK_OFFSET;
								iHeight += Math.Max( iTextHeight, iCheckMarkHeight );
							}
                            if (m_Item.ImageCheckBox)
                                iWidth += (m_Item.ImageCheckBoxSize.Width - MIN_CHECKMARK_WIDTH);
							m_szLastPreferred = new Size( iWidth, iHeight );
						}
					}

					return m_szLastPreferred;
				}
			}
			/// <summary>
			/// Gets if CheckMark is positioned above or under the Text in <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			/// <returns></returns>
			private bool GetIsAlignmentInTwoLines()
			{
				bool bResult = false;

				if( m_Item.CheckAlign == ContentAlignment.TopCenter ||
					m_Item.CheckAlign == ContentAlignment.BottomCenter )
				{
					bResult = true;
				}

				return bResult;
			}
			/// <summary>
			/// Gets bounds of check mark part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			internal Rectangle CheckMarkRectangle
			{
				get
				{
					if( m_Item != null && m_rcCheckMark == Rectangle.Empty )
					{
						int iTop = GetCoordinateYOfCheckMark();
						int iLeft = GetCoordinateXOfCheckMark();

						m_rcCheckMark = new Rectangle( new Point( iLeft, iTop ), CheckMarkSize );
					}

					return m_rcCheckMark;
				}
			}
			/// <summary>
			/// Gets bounds of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			internal Rectangle TextRectangle
			{
				get
				{
					if( m_Item != null && m_rcText == Rectangle.Empty )
					{
						int iTop = GetCoordinateYOfText();
						int iHeight = GetHeightOfText();
						int iLeft = GetCoordinateXOfText();
						int iWidth = GetWidthOfText();

						m_rcText = new Rectangle( new Point( iLeft, iTop ), new Size( iWidth, iHeight ) );
					}

					return m_rcText;
				}
			}
			/// <summary>
			/// Gets size of check mark part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			internal Size CheckMarkSize
			{
				get
				{
					if( m_Item != null && m_szCheckMark == Size.Empty )
					{
                              Bitmap bit = new Bitmap(10, 10);
                              using (Graphics g = Graphics.FromImage(bit))
                              {
                                  if (g.DpiX > 96)
                                  {
                                      m_szCheckMark = CheckBoxRenderer.GetGlyphSize(g, System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal);
                                  }
                                  else
                                  {
                                      m_szCheckMark = new Size(MIN_CHECKMARK_WIDTH, MIN_CHECKMARK_HEIGHT);
                                  }
                              }
                              bit.Dispose();
					}

					return m_szCheckMark;
				}
			}
			/// <summary>
			/// Gets size of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			internal Size TextSize
			{
				get
				{
					if( m_Item != null && m_szText == Size.Empty )
					{
						m_szText = TextRenderer.MeasureText( m_Item.Text, m_Item.Font );
					}

					return m_szText;
				}
			}
			#endregion

			#region Fields
			/// <summary>
			/// Last preferred size of <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			private Size m_szLastPreferred;
			/// <summary>
			/// <summary> Instance of <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>. </summary>
			/// </summary>
			private ToolStripCheckBox m_Item;
			/// <summary>
			///  Gets bounds of check mark part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			private Rectangle m_rcCheckMark;
			/// <summary>
			///  Gets bounds of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			private Rectangle m_rcText;
			/// <summary>
			/// Size of check mark part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			private Size m_szCheckMark;
			/// <summary>
			/// Size of text part of the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
			/// </summary>
			private Size m_szText;
			#endregion
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClick( EventArgs e )
		{
			this.CheckOnClick = false;

			switch( this.CheckState )
			{
				case CheckState.Unchecked:
					this.CheckState = CheckState.Checked;
					break;

				case CheckState.Checked:
					if( !m_bThreeState )
					{
						this.CheckState = CheckState.Unchecked;
					}
					else
					{
						this.CheckState = CheckState.Indeterminate;
					}
					break;

				default:
					this.CheckState = CheckState.Unchecked;
					break;
			}

			base.OnClick( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="constrainingSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize( Size constrainingSize )
		{
			return InternalLayout.PreferredSize;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLayout( LayoutEventArgs e )
		{
			m_InternalLayout = new CheckBoxInternalLayout( this );

			base.OnLayout( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRightToLeftChanged( EventArgs e )
		{
			m_InternalLayout = new CheckBoxInternalLayout( this );

			base.OnRightToLeftChanged( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint( PaintEventArgs e )
		{
			if( base.Owner != null )
			{
				PaintCheckBoxBackground( new ToolStripItemRenderEventArgs( e.Graphics, this ) );
			}
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Gets Image for a check mark by specific ID.
		/// </summary>
		/// <param name="buttonID"> Check mark ID that indicates Image. </param>
		/// <returns></returns>
		internal Image GetCheckMarkImage( int iCheckMarkID )
		{
			Image imgCheckMark = null;
            if(metroCheckbox)
			    imgCheckMark = m_imglstMetroCheckMarkStates.Images[ iCheckMarkID ];
            else
                imgCheckMark = m_imglstCheckMarkStates.Images[iCheckMarkID];
            return imgCheckMark;
		}
		/// <summary>
		/// Draws the background of the ToolStripCheckBox.
		/// </summary>
		/// <param name="e"></param>
		private void PaintCheckBoxBackground( ToolStripItemRenderEventArgs e )
		{
			ToolStripCheckBox tsCheckBox = e.Item as ToolStripCheckBox;

			if( tsCheckBox != null )
			{
				Rectangle rc = new Rectangle( Point.Empty, e.Item.Size );

				if( rc.Width > 0 && rc.Height > 0 )
				{
					Rectangle rcBackground = GetButtonBackgroundRect( e.Item, rc );

					if( rcBackground.Width > 0 && rcBackground.Height > 0 )
					{
						Graphics g = e.Graphics;

						int iCheckMarkID = -1;

						Color clText = Color.Empty;
						Color clExternalBorder = Color.Empty;

						Rectangle rcBorder = tsCheckBox.InternalLayout.CheckMarkRectangle;
						Rectangle rcCheckBoxInScreen = tsCheckBox.GetCurrentParent().RectangleToScreen( tsCheckBox.Bounds );

						bool bSelected = tsCheckBox.Selected;
						bool bPressed = tsCheckBox.Pressed;
						bool bChecked = tsCheckBox.Checked;
						bool bPressedAndNotSelected = !rcCheckBoxInScreen.Contains( Cursor.Position );
						bool bDisabled = GetIsDisabled( tsCheckBox );

						rcBorder.Width -= 1;
						rcBorder.Height -= 1;

						if( !bDisabled )
						{
							if( bSelected && bPressed )
							{
								clExternalBorder = this.ColorTable.CheckBoxBorderSelected;
								iCheckMarkID = bChecked ? ToolStripCheckBox.CHECKED_PRESSED : ToolStripCheckBox.UNCHECKED_PRESSED;
							}
							else if( bPressedAndNotSelected || !bSelected )
							{
								clExternalBorder = this.ColorTable.CheckBoxBorder;
								iCheckMarkID = bChecked ? ToolStripCheckBox.CHECKED : ToolStripCheckBox.UNCHECKED;
							}
							else
							{
								clExternalBorder = this.ColorTable.CheckBoxBorderSelected;
								iCheckMarkID = bChecked ? ToolStripCheckBox.CHECKED_SELECTED : ToolStripCheckBox.UNCHECKED_SELECTED;
							}
						}
						else
						{
							clExternalBorder = SystemColors.GrayText;
							iCheckMarkID = bChecked ? ToolStripCheckBox.CHECKED_DISABLED : ToolStripCheckBox.UNCHECKED_DISABLED;
						}

						// Draw External border.
                        if (!this.ImageCheckBox)
                        {
                            using (Pen penExternal = new Pen(clExternalBorder))
                            {
                                g.DrawRectangle(penExternal, rcBorder);
                            }
                        }
						// Draw Internal border.
						rcBorder.Inflate( -1, -1 );
                        if (!this.ImageCheckBox)
                        {
                            using (Pen penInternal = new Pen(Color.White))
                            {
                                g.DrawRectangle(penInternal, rcBorder);
                            }
                        }
						// Draw check mark.
                        rcBorder.Inflate(-1, -1);
						if( tsCheckBox.CheckState == CheckState.Indeterminate )
						{
							using( Brush brushIndeterminate = new SolidBrush( Color.DarkGray ) )
							{
								rcBorder.Width += 1;
								rcBorder.Height += 1;
								g.FillRectangle( brushIndeterminate, rcBorder );
							}
						}
						else
						{
                            if (this.ImageCheckBox && CheckedImage != null && Checked)
                            {
                                int adjustYBound = (ImageCheckBoxSize.Height - 13) / 2;
                                g.DrawImage(CheckedImage, rcBorder.X, rcBorder.Y - adjustYBound, ImageCheckBoxSize.Width, ImageCheckBoxSize.Height);
                            }
                            else if(!this.ImageCheckBox)
                            {
                                using (Image imgCheckMark = tsCheckBox.GetCheckMarkImage(iCheckMarkID))
                                {
                                    if ((tsCheckBox.Parent.Parent as RibbonPanel) != null && (tsCheckBox.Parent.Parent as RibbonPanel).RibbonStyle == RibbonStyle.Office2013)
                                        metroCheckbox = true;
                                    else
                                        metroCheckbox = false;
                                    g.DrawImage(imgCheckMark, rcBorder.X, rcBorder.Y);
                                }
                            }
						}

						// Draw text.
						clText = bDisabled ? SystemColors.GrayText : this.ColorTable.RibbonTabText;
						TextRenderer.DrawText( g, tsCheckBox.Text, tsCheckBox.Font, tsCheckBox.InternalLayout.TextRectangle, clText, GetTextFormatFlags( tsCheckBox.TextAlign ) );
					}
				}
			}
		}
		/// <summary>
		/// Indicates if ToolStripItem is disabled.
		/// </summary>
		/// <param name="item"></param>
		private bool GetIsDisabled( ToolStripItem item )
		{
			return ( !item.Enabled || !item.GetCurrentParent().Enabled );
		}
		/// <summary>
		/// Gets TextFormatFlags according to Text alignment.
		/// </summary>
		/// <param name="alignment"></param>
		/// <returns></returns>
		private TextFormatFlags GetTextFormatFlags( ContentAlignment alignment )
		{
			TextFormatFlags flags;

			switch( alignment )
			{
				case ContentAlignment.BottomCenter:
					flags = TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
					break;

				case ContentAlignment.BottomLeft:
					flags = TextFormatFlags.Bottom | TextFormatFlags.Left;
					break;

				case ContentAlignment.BottomRight:
					flags = TextFormatFlags.Bottom | TextFormatFlags.Right;
					break;

				case ContentAlignment.MiddleCenter:
					flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
					break;

				case ContentAlignment.MiddleLeft:
					flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
					break;

				case ContentAlignment.MiddleRight:
					flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
					break;

				case ContentAlignment.TopCenter:
					flags = TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
					break;

				case ContentAlignment.TopLeft:
					flags = TextFormatFlags.Top | TextFormatFlags.Left;
					break;

				case ContentAlignment.TopRight:
					flags = TextFormatFlags.Top | TextFormatFlags.Right;
					break;

				default:
					flags = TextFormatFlags.Default;
					break;
			}

			return flags;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <param name="rc"></param>
		/// <returns></returns>
		protected Rectangle GetButtonBackgroundRect( ToolStripItem item, Rectangle rc )
		{
			Rectangle rcResult = rc;

			if( !( item is ToolStripOverflowButton ) )
			{
				rcResult.Inflate( -1, -1 );
			}

			return rcResult;
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}
			set
			{
				base.BackgroundImage = value;
			}
		}
		/// <summary>
		/// Gets or sets the background image layout used for the <see cref="T:System.Windows.Forms.ToolStripItem"></see>.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Windows.Forms.ImageLayout"></see> values. The default value is <see cref="F:System.Windows.Forms.ImageLayout.Tile"></see>. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override ImageLayout BackgroundImageLayout
		{
			get
			{
				return base.BackgroundImageLayout;
			}
			set
			{
				base.BackgroundImageLayout = value;
			}
		}
		/// <summary>
		/// Gets or sets the background color for the item.
		/// </summary>
		/// <returns>A <see cref="T:System.Drawing.Color"></see> that represents the background color of the item. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultBackColor"></see> property.</returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}
		/// <summary>Gets or sets the horizontal and vertical alignment of the check mark on a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see> control.</summary>
		/// <returns>One of the <see cref="T:System.Drawing.ContentAlignment"></see> values. The default value is MiddleLeft.</returns>
		/// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the <see cref="T:System.Drawing.ContentAlignment"></see> enumeration values. </exception>
		/// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" /></PermissionSet>
		[Description( "Gets or sets the horizontal and vertical alignment of the check mark on a ToolStripCheckBox control." )]
		[Category( "Appearance" )]
		[Bindable( true )]
		[DefaultValue( ContentAlignment.MiddleLeft )]
		public ContentAlignment CheckAlign
		{
			get
			{
				return m_calignCheckAlignment;
			}
			set
			{
				if( m_calignCheckAlignment != value )
				{
					m_calignCheckAlignment = value;
					m_InternalLayout = new CheckBoxInternalLayout( this );
					this.Size = GetPreferredSize( Size.Empty );
					Invalidate();
				}
			}
		}
		/// <summary>
		/// This property is not used, because <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see> automatically appears pressed in and not pressed in when clicked.
		/// </summary>
		[Browsable( false )]
		[Category( "Behavior" )]
		[DefaultValue( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new bool CheckOnClick
		{
			get { return false; }
			set
			{
				value = false;
			}
		}
        private bool imageCheckBox = false;
        /// <summary>
        /// Gets/Sets the value for displaying the image or not in checked state.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public bool ImageCheckBox
        {
            get { return imageCheckBox; }
            set
            {
                if (imageCheckBox != value)
                {
                    imageCheckBox = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Serialzing the property CheckedImage
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeImageCheckBox()
        {
            return ImageCheckBox != false;
        }
        /// <summary>
        /// Resetting the property CheckedImage
        /// </summary>
        void ResetImageCheckBox()
        {
            ImageCheckBox = false;
        }
        private Image checkedImage = null;
        /// <summary>
        /// Gets/Sets the value for displaying the image or not in checked state.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(null)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Image CheckedImage
        {
            get { return checkedImage; }
            set
            {
                if (checkedImage != value && value != null)
                {
                    checkedImage = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Serialzing the property CheckedImage
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeCheckedImage()
        {
            return CheckedImage != null;
        }
        /// <summary>
        /// Resetting the property CheckedImage
        /// </summary>
        void ResetCheckedImage()
        {
            CheckedImage = null;
        }
        private Size imageCheckBoxSize = new Size(13, 13);
        /// <summary>
        /// Gets/Sets the value for size of the displaying the image in checked state.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Size ImageCheckBoxSize
        {
            get { return imageCheckBoxSize; }
            set
            {
                if (imageCheckBoxSize != value)
                {
                    imageCheckBoxSize = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Serialzing the property ImageCheckBoxSize
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeImageCheckBoxSize()
        {
            return ImageCheckBoxSize != new Size(13, 13);
        }
        /// <summary>
        /// Resetting the property ImageCheckBoxSize
        /// </summary>
        void ResetImageCheckBoxSize()
        {
            ImageCheckBoxSize = new Size(13, 13);
        }
		/// <summary>
		/// Gets instance of Office12ColorTable class.
		/// If <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see> owner's renderer has Office12ToolStripRenderer type, 
		/// property returns current Office12ColorTable instance, otherwise - returns default Office12ColorTable one.
		/// </summary>
		internal Office12ColorTable ColorTable
		{
			get
			{
				Office12ColorTable cltblColorTable = null;
				ToolStrip tsOwner = this.Owner;

				if( tsOwner != null )
				{
					Office12ToolStripRenderer renderer = tsOwner.Renderer as Office12ToolStripRenderer;

					if( renderer != null )
					{
						cltblColorTable = renderer.OfficeColorTable;
					}
				}

				if( cltblColorTable == null )
				{
					cltblColorTable = new Office12ColorTable();
				}

				return cltblColorTable;
			}
		}
		/// <summary>
		/// Gets or sets whether text and images are displayed on a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Windows.Forms.ToolStripItemDisplayStyle"></see> values. The default is <see cref="F:System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText"></see>. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new ToolStripItemDisplayStyle DisplayStyle
		{
			get { return base.DisplayStyle; }
			set { base.DisplayStyle = value; }
		}
		/// <summary>
		/// Gets or sets the foreground color of the item.
		/// </summary>
		/// <returns> The foreground <see cref="T:System.Drawing.Color"></see> of the item. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultForeColor"></see> property. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
			}
		}
		/// <summary>
		/// Gets or sets the image that is displayed on a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
		/// </summary>
		/// <returns> The <see cref="T:System.Drawing.Image"></see> to be displayed. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override Image Image
		{
			get
			{
				return base.Image;
			}
			set
			{
				base.Image = value;
			}
		}
		/// <summary>
		/// Gets or sets the alignment of the image on a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Drawing.ContentAlignment"></see> values. The default is <see cref="F:System.Drawing.ContentAlignment.MiddleLeft"></see>. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new ContentAlignment ImageAlign
		{
			get
			{
				return base.ImageAlign;
			}
			set
			{
				base.ImageAlign = value;
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether an image on a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see> is automatically resized to fit in a container.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Windows.Forms.ToolStripItemImageScaling"></see> values. The default is <see cref="F:System.Windows.Forms.ToolStripItemImageScaling.SizeToFit"></see>. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new ToolStripItemImageScaling ImageScaling
		{
			get
			{
				return base.ImageScaling;
			}
			set
			{
				base.ImageScaling = value;
			}
		}
		/// <summary>
		/// Gets or sets the color to treat as transparent in a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see> image.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Drawing.Color"></see> values. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new Color ImageTransparentColor
		{
			get
			{
				return base.ImageTransparentColor;
			}
			set
			{
				base.ImageTransparentColor = value;
			}
		}
		/// <summary>
		/// Gets or sets Internal layout instance.
		/// </summary>
		internal CheckBoxInternalLayout InternalLayout
		{
			get
			{
				if( m_InternalLayout == null )
				{
					m_InternalLayout = new CheckBoxInternalLayout( this );
				}

				return m_InternalLayout;
			}
			set
			{
				m_InternalLayout = value;
			}
		}
		/// <summary>
		/// Mirrors automatically the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see> image when the <see cref="P:System.Windows.Forms.ToolStripItem.RightToLeft"></see> property is set to <see cref="F:System.Windows.Forms.RightToLeft.Yes"></see>.
		/// </summary>
		/// <returns> true to automatically mirror the image; otherwise, false. The default is false. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new bool RightToLeftAutoMirrorImage
		{
			get
			{
				return base.RightToLeftAutoMirrorImage;
			}
			set
			{
				base.RightToLeftAutoMirrorImage = value;
			}
		}
		/// <summary>
		/// Gets the orientation of text used on a <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see>.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Windows.Forms.ToolStripTextDirection"></see> values. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public override ToolStripTextDirection TextDirection
		{
			get
			{
				return base.TextDirection;
			}
			set
			{
				base.TextDirection = value;
			}
		}
		/// <summary>
		/// Gets or sets the position of <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see> text and image relative to each other.
		/// </summary>
		/// <returns> One of the <see cref="T:System.Windows.Forms.TextImageRelation"></see> values. The default is <see cref="F:System.Windows.Forms.TextImageRelation.ImageBeforeText"></see>. </returns>
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public new TextImageRelation TextImageRelation
		{
			get
			{
				return base.TextImageRelation;
			}
			set
			{
				base.TextImageRelation = value;
			}
		}
		/// <summary>Gets or sets a value indicating whether the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see> will allow three check states rather than two.</summary>
		/// <returns>true if the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see> is able to display three check states; otherwise, false. The default value is false.</returns>
		/// <filterpriority>1</filterpriority>
		[Description( "Gets or sets a value indicating whether the ToolStripCheckBox will allow three check states rather than two." )]
		[DefaultValue( false )]
		[Category( "Behavior" )]
		public bool ThreeState
		{
			get
			{
				return m_bThreeState;
			}
			set
			{
				m_bThreeState = value;
			}
		}
		#endregion

		#region Fields
		/// <summary>
		/// Internal layout instance.
		/// </summary>
		private CheckBoxInternalLayout m_InternalLayout;
		/// <summary>
		/// Horizontal and vertical alignment of the check mark on a <see cref="T:System.Windows.Forms.CheckBox"></see> control.
		/// </summary>
		private ContentAlignment m_calignCheckAlignment = ContentAlignment.MiddleLeft;
		/// <summary>
		/// <summary> A value indicating whether the <see cref="T:Syncfusion.Windows.Forms.Tools.ToolStripCheckBox"></see> will allow three check states rather than two.
		/// </summary>
		private bool m_bThreeState = false;
		/// <summary>
		/// 
		/// </summary>
		static ImageList m_imglstCheckMarkStates;
        static ImageList m_imglstMetroCheckMarkStates;
		#endregion
	}
}

#endif