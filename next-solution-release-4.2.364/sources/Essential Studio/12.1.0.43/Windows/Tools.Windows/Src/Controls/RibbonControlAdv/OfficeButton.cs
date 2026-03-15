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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary> ToolStripButton with HelpText below Text that displayed in Bold style. </summary>
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailability(System.Windows.Forms.Design.ToolStripItemDesignerAvailability.None)]
	[ToolboxBitmap(typeof(OfficeButton), "ToolboxIcons.OfficeButton.bmp")]
	public class OfficeButton : ToolStripButton
	{
		#region Constants
		/// <summary> Minimum height of control. </summary>
		const int MIN_HEIGHT = 23;
		#endregion

		#region Nested classes
		internal class OfficeButtonInternalLayout
		{
			#region Initialization
			/// <summary>
			/// 
			/// </summary>
			/// <param name="item"></param>
			public OfficeButtonInternalLayout( OfficeButton item )
			{
				m_Item = item;
			}
			#endregion

			#region Constants
			/// <summary> Control border width. </summary>
			const int BORDER_WIDTH = 2;
			/// <summary> Margin between Text and Additional Text. </summary>
			const int MARGIN = 2;
			/// <summary> Minimum Image height. </summary>
			const int MIN_IMAGE_HEIGHT = 16;
			/// <summary> Minimum Image width. </summary>
			const int MIN_IMAGE_WIDTH = 16;
			#endregion

			#region Properties
			/// <summary> Get preferred size for OfficeButton. </summary>
			public Size PreferredSize
			{
				get
				{
					if( m_Item != null )
					{
						if( m_szLastPreferredSize == Size.Empty )
						{
							int iWidth = 2 * BORDER_WIDTH + m_Item.Padding.Horizontal;
							int iHeight = 2 * BORDER_WIDTH + m_Item.Padding.Vertical;

							int iImageWidth = ImageSize.Width;
							int iImageHeight = ImageSize.Height;

							iWidth += iImageWidth;

							int iTextHeight = 0;

							if( ( m_Item.DisplayStyle & ToolStripItemDisplayStyle.Text ) == ToolStripItemDisplayStyle.Text )
							{
								iTextHeight = TextSize.Height + MARGIN + HelpTextSize.Height;
								iWidth += Math.Max(TextSize.Width, HelpTextSize.Width);
							}

							iHeight += Math.Max( iTextHeight, iImageHeight );
							iHeight = Math.Max( iHeight, MIN_HEIGHT );

							m_szLastPreferredSize = new Size( iWidth, iHeight );
						}
					}

					return m_szLastPreferredSize;
				}
			}
			/// <summary> Gets size of image. </summary>
			private Size ImageSize
			{
				get
				{
					if( m_szImage == Size.Empty )
					{
						if( m_Item != null )
						{
							int iImageWidth = 0;
							int iImageHeight = 0;

							if( ( m_Item.DisplayStyle & ToolStripItemDisplayStyle.Image ) == ToolStripItemDisplayStyle.Image )
							{
								Size szImageScaling = new Size( MIN_IMAGE_WIDTH, MIN_IMAGE_HEIGHT );

								if( m_Item.Owner != null )
								{
									szImageScaling = m_Item.Owner.ImageScalingSize;
								}

								iImageWidth = szImageScaling.Width;
								iImageHeight = szImageScaling.Height;

								if( m_Item.ImageScaling == ToolStripItemImageScaling.None )
								{
									if( m_Item.Image != null )
									{
										iImageWidth = m_Item.Image.Width;
										iImageHeight = m_Item.Image.Height;
									}
								}

								m_szImage = new Size( iImageWidth, iImageHeight );
							}
						}
					}

					return m_szImage;
				}
			}
			/// <summary> Gets size of text. </summary>
			private Size TextSize
			{
				get
				{
					if( m_szText == Size.Empty && m_Item != null)
					{
						m_szText = TextRenderer.MeasureText(m_Item.Text, m_Item.FontBold);
					}
					return m_szText;
				}
			}
			/// <summary> Gets size of help text. </summary>
			private Size HelpTextSize
			{
				get
				{
					if (m_szHelpText == Size.Empty && m_Item != null)
					{
						m_szHelpText = TextRenderer.MeasureText(m_Item.HelpText, m_Item.Font, new Size(TextSize.Width, 1), this.TextFormat);
					}
					return m_szHelpText;
				}
			}
			/// <summary> Gets TextFormatFlags to display Text. </summary>
			internal TextFormatFlags TextFormat
			{
				get
				{
					return TextFormatFlags.Default;
				}
			}
			/// <summary> Gets bounds of image. </summary>
			internal Rectangle ImageRectangle
			{
				get
				{
					if( m_Item != null && m_rcImage == Rectangle.Empty )
					{
						Size szImage = ImageSize;
						ToolStrip toolStrip = m_Item.Owner;

						int iTop = BORDER_WIDTH + m_Item.Padding.Top;
						int iBottom = m_Item.Height - BORDER_WIDTH - m_Item.Padding.Bottom - szImage.Height;
						int iMiddle = iTop + ( ( m_Item.Height - 2 * BORDER_WIDTH - m_Item.Padding.Vertical ) - szImage.Height ) / 2;

						if( ( toolStrip != null && toolStrip.RightToLeft == RightToLeft.Yes ) || m_Item.RightToLeft == RightToLeft.Yes )
						{
							int iRight = m_Item.Width - ( BORDER_WIDTH + m_Item.Padding.Right + szImage.Width );

							switch( m_Item.ImageAlign )
							{
								case ContentAlignment.TopRight:
									m_rcImage = new Rectangle( new Point( iRight, iTop ), szImage );
									break;

								case ContentAlignment.MiddleRight:
									m_rcImage = new Rectangle( new Point( iRight, iMiddle ), szImage );
									break;

								case ContentAlignment.BottomRight:
									m_rcImage = new Rectangle( new Point( iRight, iBottom ), szImage );
									break;

								default:
									m_rcImage = new Rectangle( new Point( iRight, iMiddle ), szImage );
									break;
							}
						}
						else
						{
							int iLeft = BORDER_WIDTH + m_Item.Padding.Left;

							switch( m_Item.ImageAlign )
							{
								case ContentAlignment.TopLeft:
									m_rcImage = new Rectangle( new Point( iLeft, iTop ), szImage );
									break;

								case ContentAlignment.MiddleLeft:
									m_rcImage = new Rectangle( new Point( iLeft, iMiddle ), szImage );
									break;

								case ContentAlignment.BottomLeft:
									m_rcImage = new Rectangle( new Point( iLeft, iBottom ), szImage );
									break;

								default:
									m_rcImage = new Rectangle( new Point( iLeft, iMiddle ), szImage );
									break;
							}
						}
					}

					return m_rcImage;
				}
			}
			/// <summary> Gets bounds of text. </summary>
			internal Rectangle TextRectangle
			{
				get
				{
					if( m_Item != null && m_rcText == Rectangle.Empty )
					{
						ToolStrip toolStrip = m_Item.Owner;
						int iLeft = BORDER_WIDTH + m_Item.Padding.Left;
						int iTop = BORDER_WIDTH + m_Item.Padding.Top;
						int iHeight = m_Item.Height - 2 * BORDER_WIDTH - m_Item.Padding.Vertical - HelpTextSize.Height;/*HelpTextRectangle.Height*/

						if( ( toolStrip != null && toolStrip.RightToLeft == RightToLeft.Yes ) || m_Item.RightToLeft == RightToLeft.Yes )
						{
							bool bTextEmpty = ( m_Item.Text.Length == 0 );

							if( !bTextEmpty )
							{
								m_rcText = new Rectangle( new Point( iLeft, iTop ), new Size( TextSize.Width, iHeight ) );
							}
						}
						else
						{
							bool bImageShown = ( m_Item.DisplayStyle & ToolStripItemDisplayStyle.Image ) == ToolStripItemDisplayStyle.Image;
							bool bTextEmpty = ( m_Item.Text.Length == 0 );

							if( !bTextEmpty )
							{
								if( bImageShown )
								{
									m_rcText = new Rectangle( new Point( ImageRectangle.Right, iTop ), new Size( TextSize.Width, iHeight ) );
								}
								else
								{
									m_rcText = new Rectangle( new Point( iLeft, iTop ), new Size( TextSize.Width, iHeight ) );
								}
							}
						}
					}

					return m_rcText;
				}
			}
			/// <summary> Gets bounds of help text. </summary>
			internal Rectangle HelpTextRectangle
			{
				get
				{
					if( m_Item != null )
					{
						ToolStrip toolStrip = m_Item.Owner;

						if( ( toolStrip != null && toolStrip.RightToLeft == RightToLeft.Yes ) || m_Item.RightToLeft == RightToLeft.Yes )
						{
							if( m_Item.Text.Length != 0 && m_HelpTextRectangle == Rectangle.Empty )
							{
								int iPaddingLeft = BORDER_WIDTH + m_Item.Padding.Left;
								int iPaddingTop = BORDER_WIDTH + m_Item.Padding.Top;

								bool bTextEmpty = ( m_Item.Text.Length == 0 );

								if( !bTextEmpty )
								{
									m_HelpTextRectangle = new Rectangle( new Point( iPaddingLeft, iPaddingTop + m_rcText.Height + MARGIN ), HelpTextSize );
								}
								else
								{
									m_HelpTextRectangle = new Rectangle( new Point( iPaddingLeft, iPaddingTop ), HelpTextSize );
								}
							}
						}
						else
						{
							if( m_Item.Text.Length != 0 && m_HelpTextRectangle == Rectangle.Empty )
							{
								int iPaddingLeft = BORDER_WIDTH + m_Item.Padding.Left;
								int iPaddingTop = BORDER_WIDTH + m_Item.Padding.Top;

								bool bImageShown = ( m_Item.DisplayStyle & ToolStripItemDisplayStyle.Image ) == ToolStripItemDisplayStyle.Image;
								bool bTextEmpty = ( m_Item.Text.Length == 0 );

								if( bImageShown && !bTextEmpty )
								{
									m_HelpTextRectangle = new Rectangle( new Point( ImageRectangle.Right, iPaddingTop + m_rcText.Height + MARGIN ), HelpTextSize );
								}
								else if( bImageShown && bTextEmpty )
								{
									m_HelpTextRectangle = new Rectangle( new Point( ImageRectangle.Right, iPaddingTop ), HelpTextSize );
								}
								else if( !bTextEmpty )
								{
									m_HelpTextRectangle = new Rectangle( new Point( iPaddingLeft, iPaddingTop + m_rcText.Height + MARGIN ), HelpTextSize );
								}
								else
								{
									m_HelpTextRectangle = new Rectangle( new Point( iPaddingLeft, iPaddingTop ), HelpTextSize );
								}
							}
						}
					}

					return m_HelpTextRectangle;
				}
			}
			#endregion

			#region Fields
			/// <summary> Last preferred size of OfficeButton. </summary>
			private Size m_szLastPreferredSize;
			/// <summary> Instance of OfficeButton. </summary>
			private OfficeButton m_Item;
			/// <summary> Bounds of image. </summary>
			private Rectangle m_rcImage;
			/// <summary> Bounds of text. </summary>
			private Rectangle m_rcText;
			/// <summary> Bounds of help text. </summary>
			private Rectangle m_HelpTextRectangle;
			/// <summary> Size of text. </summary>
			private Size m_szText;
			/// <summary> Size of help text. </summary>
			private Size m_szHelpText;
			/// <summary> Size of image. </summary>
			private Size m_szImage;
			#endregion
		}
		#endregion

		#region Initialization
		/// <summary> </summary>
		public OfficeButton()
			: base()
		{
		}
		#endregion

		#region Overrides
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
		protected override void OnFontChanged( EventArgs e )
		{
			m_Font = null;

			m_InternalLayout = null;

			base.OnFontChanged( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint( PaintEventArgs e )
		{
			if( base.Owner != null )
			{
				ToolStripRenderer renderer = base.Owner.Renderer;

				renderer.DrawButtonBackground( new ToolStripItemRenderEventArgs( e.Graphics, this ) );

				// Drawing image.
				if( ( this.DisplayStyle & ToolStripItemDisplayStyle.Image ) == ToolStripItemDisplayStyle.Image )
				{
					ToolStripItemImageRenderEventArgs args = new ToolStripItemImageRenderEventArgs( e.Graphics, this, this.Image, InternalLayout.ImageRectangle );
					renderer.DrawItemImage( args );
				}

				// Drawing Text and Additional Text.
				if( ( this.DisplayStyle & ToolStripItemDisplayStyle.Text ) == ToolStripItemDisplayStyle.Text )
				{
					renderer.DrawItemText( new ToolStripItemTextRenderEventArgs( e.Graphics, this, this.Text, InternalLayout.TextRectangle, this.ForeColor, this.FontBold, this.TextAlign ) );
					renderer.DrawItemText( new ToolStripItemTextRenderEventArgs( e.Graphics, this, this.HelpText, InternalLayout.HelpTextRectangle, this.ForeColor, this.Font, InternalLayout.TextFormat ) );
				}
			}
			else
			{
				base.OnPaint( e );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected override void OnBoundsChanged()
		{
			m_InternalLayout = null;

			base.OnBoundsChanged();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnParentRightToLeftChanged( EventArgs e )
		{
			m_InternalLayout = null;

			base.OnParentRightToLeftChanged( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRightToLeftChanged( EventArgs e )
		{
			m_InternalLayout = null;

			base.OnRightToLeftChanged( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLayout( LayoutEventArgs e )
		{
			m_InternalLayout = null;

			base.OnLayout( e );
		}
		#endregion

		#region Properties
		/// <summary> Gets or sets additional text displayed in Bold style. </summary>
		[Category( "Appearance" )]
		[Localizable(true), DefaultValue( "" ) ]
		[Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string HelpText
		{
			get
			{
				return m_sHelpText;
			}
			set
			{
				m_sHelpText = value;

				ToolStrip parent = this.GetCurrentParent() as ToolStrip;

				if( parent != null )
				{
					m_InternalLayout = null;
					parent.PerformLayout();
				}
			}
		}
		/// <summary> Gets font for additional text. </summary>
		[Category( "Appearance" )]
		internal Font FontBold
		{
			get
			{
				if( m_Font == null )
				{
					m_Font = new Font( base.Font, FontStyle.Bold );
				}

				return m_Font;
			}
		}
		/// <summary> Gets or sets area for painting Image. </summary>
		internal Rectangle ImageBounds
		{
			get
			{
				return m_rcImageBounds;
			}
			set
			{
				m_rcImageBounds = value;
			}
		}
		/// <summary> Gets or sets area for painting Text. </summary>
		internal Rectangle TextBounds
		{
			get
			{
				return m_rcTextBounds;
			}
			set
			{
				m_rcTextBounds = value;
			}
		}
		/// <summary> Gets or sets area for painting HelpText. </summary>
		internal Rectangle HelpTextBounds
		{
			get
			{
				return m_rcHelpTextBounds;
			}
			set
			{
				m_rcHelpTextBounds = value;
			}
		}
		/// <summary> Gets or sets area for painting Text. </summary>
		internal Size TextSize
		{
			get
			{
				return m_szText;
			}
			set
			{
				m_szText = value;
			}
		}
		/// <summary> Gets or sets area for painting HelpText. </summary>
		internal Size HelpTextSize
		{
			get
			{
				return m_szHelpText;
			}
			set
			{
				m_szHelpText = value;
			}
		}
		/// <summary> Gets or sets Internal layout instance. </summary>
		internal OfficeButtonInternalLayout InternalLayout
		{
			get
			{
				if( m_InternalLayout == null )
				{
					m_InternalLayout = new OfficeButtonInternalLayout( this );
				}

				return m_InternalLayout;
			}
			set
			{
				m_InternalLayout = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[ DefaultValue( ToolStripItemDisplayStyle.Image ) ]
		public override ToolStripItemDisplayStyle DisplayStyle
		{
			get
			{
				return base.DisplayStyle;
			}
			set
			{
				base.DisplayStyle = value;
			}
		}
		#endregion

		#region Fields
		/// <summary> Additional text displayed in Bold style near the Image. </summary>
		private string m_sHelpText = String.Empty;
		/// <summary> Font for additional text. </summary>
		private Font m_Font;
		/// <summary> Area for painting Image. </summary>
		private Rectangle m_rcImageBounds = Rectangle.Empty;
		/// <summary> Area for painting Text. </summary>
		private Rectangle m_rcTextBounds = Rectangle.Empty;
		/// <summary> Area for painting HelpText. </summary>
		private Rectangle m_rcHelpTextBounds = Rectangle.Empty;
		/// <summary> Size of Text. </summary>
		private Size m_szText = Size.Empty;
		/// <summary> Size of HelpText. </summary>
		private Size m_szHelpText = Size.Empty;
		/// <summary> Internal layout instance. </summary>
		private OfficeButtonInternalLayout m_InternalLayout;
		#endregion
	}
}

#endif
