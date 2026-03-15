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
	/// <summary> ToolStripSplitButton with text associated with DropDownButton instead of Image. </summary>
	[ ToolboxItem( false ) ]
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailability( System.Windows.Forms.Design.ToolStripItemDesignerAvailability.ToolStrip | System.Windows.Forms.Design.ToolStripItemDesignerAvailability.StatusStrip )]
	[ToolboxBitmap(typeof(ToolStripSplitButtonEx), "ToolboxIcons.ToolStripSplitButtonEx.bmp")]
	public class ToolStripSplitButtonEx : ToolStripDropDownItem, IMessageFilter
	{
		#region Enums
		/// <summary>
		/// Uses to paint SplitButtonEx in different states. 
		/// </summary>
		internal enum SplitButtonState
		{
			/// <summary> Out of split button. </summary>
			None,
			/// <summary> Mouse hover Image button. </summary>
			ImageSelected,
			/// <summary> Image button is pressed. </summary>
			ImagePressed,
			/// <summary> Mouse hover dropdown button. </summary>
			ButtonSelected,
			/// <summary> Dropdown button is pressed or DropDown is opened. </summary>
			ButtonPressed
		}
		#endregion

		#region Initialization
		/// <summary> </summary>
		public ToolStripSplitButtonEx()
			: base()
		{
			Image = new Bitmap( typeof( ToolStripButton ), "blank.bmp" );
		}
        /// <summary> Get preferred Font for ToolStripSplitButtonEx. </summary>>
        // Don't call base, since this will raise exception while changing the font in RibbonCOntrolAdv
        protected override void OnFontChanged(EventArgs e)
        {            
            // base.OnFontChanged(e);
        }
		#endregion

		#region Nested classes
		internal class SplitButtonExInternalLayout
		{
			#region Constants
			/// <summary> Minimum Image height. </summary>
			const int MIN_IMAGE_HEIGHT = 16;
			/// <summary> Minimum Image width. </summary>
			const int MIN_IMAGE_WIDTH = 16;
            /// <summary> Minimum Image width. </summary>
            const int DPI_125_MINSIZE = 5;
			/// <summary> Control border width. </summary>
			const int BORDER_WIDTH = 2;
			/// <summary> Control border width. </summary>
			const int SEPARATOR_WIDTH = 2;
			/// <summary> Width of DropDownButton. </summary>
			const int DROPDOWN_BUTTON_WIDTH = 11;
            /// <summary> 125 DPI Width of DropDownButton. </summary>
            const int DPI_125_DROPDOWN_BUTTON_WIDTH = 16;
            /// <summary> 150 DPI Width of DropDownButton. </summary>
            const int DPI_150_DROPDOWN_BUTTON_WIDTH = 21;
			#endregion

			#region Initialization
			/// <summary>
			/// 
			/// </summary>
			/// <param name="item"></param>
			public SplitButtonExInternalLayout( ToolStripSplitButtonEx item )
			{
				m_Item = item;
			}
			#endregion

			#region Properties
			/// <summary> Get preferred size for ToolStripSplitButtonEx. </summary>>
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

							// Calculate Image size and increase width by Image width.
							int iImageHeight = ImageSize.Height;
							iWidth += ImageSize.Width;

							if( ( m_Item.DisplayStyle & ToolStripItemDisplayStyle.Image ) == ToolStripItemDisplayStyle.Image )
							{
								// Increase width by Separator position.
								iWidth += SEPARATOR_WIDTH;
							}

							// Calculate Text size and increase width by Text width.
							int iTextHeight = TextSize.Height;
							iWidth += TextSize.Width;

							// Increase width by DropDownButton width.
                            using (Graphics g = Graphics.FromImage(new Bitmap(10, 10)))
                            {
                                if (g.DpiX > 96)
                                    iWidth += DPI_125_DROPDOWN_BUTTON_WIDTH;
                                else
                                    iWidth += DROPDOWN_BUTTON_WIDTH;
                            }
							iHeight += Math.Max( iTextHeight, iImageHeight );

							m_szLastPreferredSize = new Size( iWidth, iHeight );
						}
					}

					return m_szLastPreferredSize;
				}
			}
			/// <summary> Gets bounds of Image. </summary>
			internal Rectangle ImageRectangle
			{
				get
				{
                    if (m_Item != null && m_rcImage == Rectangle.Empty)
                    {
                        Size szImage;
                        using (Graphics g = Graphics.FromImage(new Bitmap(10, 10)))
                        {
                            if (g.DpiX > 96)
                            {
                                szImage = new Size(ImageSize.Width + DPI_125_MINSIZE, ImageSize.Height + DPI_125_MINSIZE);
                            }
                            else
                            {
                                szImage = ImageSize;
                            }
                        }
                        int iTop = BORDER_WIDTH + m_Item.Padding.Top;
                        int iBottom = m_Item.Height - BORDER_WIDTH - m_Item.Padding.Bottom - szImage.Height;
                        int iMiddle = iTop + ((m_Item.Height - 2 * BORDER_WIDTH - m_Item.Padding.Vertical) - szImage.Height) / 2;

						if( m_Item.RightToLeft == RightToLeft.Yes )
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
			/// <summary> Gets bounds of Text. </summary>
			internal Rectangle TextRectangle
			{
				get
				{
					if( m_Item != null && m_rcText == Rectangle.Empty )
					{
						int iTop = BORDER_WIDTH + m_Item.Padding.Top;
						int iHeight = m_Item.Height - 2 * BORDER_WIDTH - m_Item.Padding.Vertical;
                        Bitmap bit = new Bitmap(10, 10);
                        using (Graphics g = Graphics.FromImage(bit))
                        {
                            if (m_Item.RightToLeft == RightToLeft.Yes)
                            {
                                int iLeft = BORDER_WIDTH + m_Item.Padding.Left + DROPDOWN_BUTTON_WIDTH;
                                if (g.DpiX > 120)
                                    iLeft = BORDER_WIDTH + m_Item.Padding.Left + DPI_150_DROPDOWN_BUTTON_WIDTH;
                                else if(g.DpiX>96)
                                    iLeft = BORDER_WIDTH + m_Item.Padding.Left + DPI_125_DROPDOWN_BUTTON_WIDTH;
                                int iWidth = m_Item.Width - iLeft - BORDER_WIDTH - m_Item.Padding.Right - ImageSize.Width - SEPARATOR_WIDTH;

                                m_rcText = new Rectangle(
                                    new Point(iLeft, iTop),
                                    new Size(iWidth, iHeight));
                            }
                            else
                            {
                                int iLeft = BORDER_WIDTH + m_Item.Padding.Left + ImageSize.Width + SEPARATOR_WIDTH;
                                int iWidth = m_Item.Width - iLeft - BORDER_WIDTH - m_Item.Padding.Right - DROPDOWN_BUTTON_WIDTH;
                                if (g.DpiX > 120)
                                {
                                    iWidth = m_Item.Width - iLeft - BORDER_WIDTH - m_Item.Padding.Right - DPI_150_DROPDOWN_BUTTON_WIDTH;

                                }
                                else if (g.DpiX > 96)
                                    iWidth = m_Item.Width - iLeft - BORDER_WIDTH - m_Item.Padding.Right - DPI_125_DROPDOWN_BUTTON_WIDTH;
                                m_rcText = new Rectangle(
                                    new Point(iLeft, iTop),
                                    new Size(iWidth, iHeight));
                            }
                        }
					}

					return m_rcText;
				}
			}
			/// <summary> Gets bounds of DropDownButton. </summary>
			internal Rectangle DropDownButtonRectangle
			{
				get
				{
					if( m_Item != null && m_rcDropDownButton == Rectangle.Empty )
					{
						int iTop = BORDER_WIDTH + m_Item.Padding.Top;
						int iHeight = m_Item.Height - 2 * BORDER_WIDTH - m_Item.Padding.Vertical;
                        Bitmap bit = new Bitmap(10, 10);
                        using (Graphics g = Graphics.FromImage(bit))
                        {
                            if (m_Item.RightToLeft == RightToLeft.Yes)
                            {
                                int iLeft = BORDER_WIDTH + m_Item.Padding.Left;
                                if (g.DpiX > 120)
                                    m_rcDropDownButton = new Rectangle(
                                 new Point(iLeft, iTop),
                                 new Size(DPI_150_DROPDOWN_BUTTON_WIDTH, iHeight)); 
                                else if (g.DpiX > 96)
                                    m_rcDropDownButton = new Rectangle(
                                 new Point(iLeft, iTop),
                                 new Size(DPI_125_DROPDOWN_BUTTON_WIDTH, iHeight));
                                else
                                    m_rcDropDownButton = new Rectangle(
                                        new Point(iLeft, iTop),
                                        new Size(DROPDOWN_BUTTON_WIDTH, iHeight));
                            }
                            else
                            {
                                int iRight = BORDER_WIDTH + m_Item.Padding.Right;
                                if(g.DpiX>120)
                                    m_rcDropDownButton = new Rectangle(
              new Point(m_Item.Width - iRight - DPI_150_DROPDOWN_BUTTON_WIDTH, iTop),
              new Size(DPI_150_DROPDOWN_BUTTON_WIDTH, iHeight));
                               else if (g.DpiX > 96)
                                    m_rcDropDownButton = new Rectangle(
                            new Point(m_Item.Width - iRight - DPI_125_DROPDOWN_BUTTON_WIDTH, iTop),
                            new Size(DPI_125_DROPDOWN_BUTTON_WIDTH, iHeight));
                                else
                                    m_rcDropDownButton = new Rectangle(
                                        new Point(m_Item.Width - iRight - DROPDOWN_BUTTON_WIDTH, iTop),
                                        new Size(DROPDOWN_BUTTON_WIDTH, iHeight));
                            }
                        }
					}

					return m_rcDropDownButton;
				}
			}
			/// <summary> Gets position of separator that divide SplitButtonEx 
			/// on button with arrrow and image areas. </summary>
			internal int SeparatorPosition
			{
				get
				{
					if( m_Item != null && m_iSeparatorPosition == -1 )
					{
						if( ( m_Item.DisplayStyle & ToolStripItemDisplayStyle.Image ) == ToolStripItemDisplayStyle.Image )
						{
							if( m_Item.RightToLeft == RightToLeft.Yes )
							{
								m_iSeparatorPosition = ImageRectangle.Left - SEPARATOR_WIDTH;
							}
							else
							{
								m_iSeparatorPosition = ImageRectangle.Right + SEPARATOR_WIDTH;
							}
						}
						else
						{
							m_iSeparatorPosition = -1;
						}
					}

					return m_iSeparatorPosition;
				}
			}
			/// <summary> Gets Image size. </summary>
			internal Size ImageSize
			{
				get
				{
					if( m_Item != null && m_szImage == Size.Empty )
					{
						if( ( m_Item.DisplayStyle & ToolStripItemDisplayStyle.Image ) == ToolStripItemDisplayStyle.Image )
				{
							int iImageHeight = 0;
							int iImageWidth = 0;

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

					return m_szImage;
				}
			}
			/// <summary> Gets Text size. </summary>
			internal Size TextSize
			{
				get
				{
					if( m_Item != null && m_szText == Size.Empty )
					{
						if( ( m_Item.DisplayStyle & ToolStripItemDisplayStyle.Text ) == ToolStripItemDisplayStyle.Text )
						{
							m_szText = TextRenderer.MeasureText( m_Item.Text, m_Item.Font );
						}
					}

					return m_szText;
				}
			}
			#endregion

			#region Fields
			/// <summary> Last preferred size of item. </summary>
			private Size m_szLastPreferredSize;
			/// <summary> Instance of ToolStripSplitButtonEx. </summary>
			private ToolStripSplitButtonEx m_Item;
			/// <summary> Bounds of Image. </summary>
			private Rectangle m_rcImage;
			/// <summary> Bounds of Text. </summary>
			private Rectangle m_rcText;
			/// <summary> Bounds of DropDownButton. </summary>
			private Rectangle m_rcDropDownButton;
			/// <summary> Size of Image. </summary>
			private Size m_szImage;
			/// <summary> Size of Text. </summary>
			private Size m_szText;
			/// <summary> Position of separator that divide SplitButtonEx 
			/// on button with arrrow and image areas. </summary>
			private int m_iSeparatorPosition = -1;
			#endregion
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
		/// <returns></returns>
		protected override ToolStripDropDown CreateDefaultDropDown()
		{
			return new ToolStripDropDownMenu();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnParentRightToLeftChanged( EventArgs e )
		{
			m_rcButton = Rectangle.Empty;
			m_rcImage = Rectangle.Empty;
			m_InternalLayout = new SplitButtonExInternalLayout( this );

			base.OnParentRightToLeftChanged( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRightToLeftChanged( EventArgs e )
		{
			m_rcButton = Rectangle.Empty;
			m_rcImage = Rectangle.Empty;
			m_InternalLayout = new SplitButtonExInternalLayout( this );

			base.OnRightToLeftChanged( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLayout( LayoutEventArgs e )
		{
			m_rcButton = Rectangle.Empty;
			m_rcImage = Rectangle.Empty;
			m_InternalLayout = new SplitButtonExInternalLayout( this );

			base.OnLayout( e );
		}
     	/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint( PaintEventArgs e )
		{
			if( base.Owner != null )
			{
                if (base.Owner is RibbonControlAdvHeader)
                {
                    if ((base.Owner as RibbonControlAdvHeader).RibbonStyle == RibbonStyle.Office2013)
                    {
                        Office2013ToolStripRenderer office2013renderrer = new Office2013ToolStripRenderer();
                        office2013renderrer.MenuColor = (base.Owner as RibbonControlAdvHeader).MenuColor;
                        office2013renderrer.PaintSplitButtonExBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
                    }
                    else if ((base.Owner as RibbonControlAdvHeader).RibbonStyle == RibbonStyle.Office2010)
                    {
                        Office2010ToolStripRenderer office2010renderrer = new Office2010ToolStripRenderer();
                        office2010renderrer.PaintSplitButtonExBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
                    }
                }
				Office12ToolStripRenderer renderer = base.Owner.Renderer as Office12ToolStripRenderer;

				if( renderer != null )
				{
					Graphics g = e.Graphics;
					renderer.PaintSplitButtonExBackground( new ToolStripItemRenderEventArgs( g, this ) );
				}
				else if (base.Owner.Renderer is Office2010ToolStripRenderer)
				{
					(base.Owner.Renderer as Office2010ToolStripRenderer).PaintSplitButtonExBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
				}
				else if (base.Owner.Renderer is Office2013ToolStripRenderer)
				{
					(base.Owner.Renderer as Office2013ToolStripRenderer).PaintSplitButtonExBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mea"></param>
		protected override void OnMouseMove( MouseEventArgs mea )
		{
			if( m_bLeftMouseButtonPressedOnImage )
			{
				if( ImageBounds.Contains( mea.Location ) )
				{
					ButtonState = SplitButtonState.ImagePressed;
				}
				else
				{
					ButtonState = SplitButtonState.None;
				}
			}
			else if( this.DropDown.Visible && ( this.DropDown.OwnerItem == this ) || m_bLeftMouseButtonPressedOnButton )
			{
				ButtonState = SplitButtonState.ButtonPressed;
			}
			else if( ImageBounds.Contains( mea.Location ) )
			{
				ButtonState = SplitButtonState.ImageSelected;
			}
			else if( ButtonBounds.Contains( mea.Location ) )
			{
				ButtonState = SplitButtonState.ButtonSelected;
			}

			base.OnMouseMove( mea );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseLeave( EventArgs e )
		{
			if( this.DropDown.Visible && ( this.DropDown.OwnerItem == this ) )
			{
				ButtonState = SplitButtonState.ButtonPressed;
			}
			else
			{
				ButtonState = SplitButtonState.None;
			}

			base.OnMouseLeave( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			bool bAddMessageFilter = false;

			if( this.ButtonBounds.Contains( e.Location ) )
			{
				Application.AddMessageFilter( this );

				if( ( e.Button == MouseButtons.Left ) && !base.DropDown.Visible )
				{
					if( ( ( this.DropDown != null ) && !this.DropDown.Visible ) || ( this.DropDownItems.Count > 0 ) )
					{
						bAddMessageFilter = true;
						m_bLeftMouseButtonPressedOnButton = true;

						this.DropDown.OwnerItem = this;
						this.DropDown.Location = this.DropDownLocation;
						this.ShowDropDown();

						base.Invalidate();
					}
				}
				else if( ( e.Button == MouseButtons.Left ) && base.DropDown.Visible )
				{
					bAddMessageFilter = true;
					m_bLeftMouseButtonPressedOnButton = true;

					this.DropDown.Close();
					this.ButtonState = SplitButtonState.ButtonPressed;
				}
			}
			else
			{
				bAddMessageFilter = true;

				this.DropDown.Close();

				m_bLeftMouseButtonPressedOnImage = true;
				this.ButtonState = SplitButtonState.ImagePressed;
			}

			if( bAddMessageFilter )
			{
				Application.AddMessageFilter( this );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp( MouseEventArgs e )
		{
			m_bLeftMouseButtonPressedOnImage = false;
			m_bLeftMouseButtonPressedOnButton = false;

			if( ButtonBounds.Contains( e.Location ) && ( this.DropDown.Visible && ( this.DropDown.OwnerItem == this ) ) )
			{
				ButtonState = SplitButtonState.ButtonPressed;
			}
			else if( ButtonBounds.Contains( e.Location ) )
			{
				ButtonState = SplitButtonState.ButtonSelected;
			}
			else if( ImageBounds.Contains( e.Location ) )
			{
				ButtonState = SplitButtonState.ImageSelected;
			}

			base.OnMouseUp( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDropDownClosed( EventArgs e )
		{
			ToolStrip ts = this.Parent as ToolStrip;

			if( ts != null )
			{
				Point pt = ts.PointToScreen( this.Bounds.Location );

				Rectangle rcButton = this.ButtonBounds;
				rcButton.Location = pt;
				rcButton.X += this.ButtonBounds.X;

				if( !rcButton.Contains( Cursor.Position ) )
				{
					m_bLeftMouseButtonPressedOnButton = false;
					ButtonState = SplitButtonState.None;
				}
			}
			
			base.OnDropDownClosed( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDropDownOpened( EventArgs e )
		{
			this.ButtonState = SplitButtonState.ButtonPressed;

			base.OnDropDownOpened( e );
		}
		/// <summary>
		/// Prevent OnClick event if user press mouse not in ImageBounds.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClick( EventArgs e )
		{
			ToolStrip ts = this.Parent as ToolStrip;

			if( ts != null )
			{
				Point pt = ts.PointToScreen( this.Bounds.Location );

				Rectangle rcImage = this.ImageBounds;
				rcImage.Location = pt;

				if( rcImage.Contains( Cursor.Position ) )
				{
					base.OnClick( e );
				}
			}
		}

		/// <summary>
		/// Gets or sets the font of the text displayed by the item.
		/// </summary>
		/// <value></value>
		/// <returns>The <see cref="T:System.Drawing.Font"/> to apply to the text displayed by the <see cref="T:System.Windows.Forms.ToolStripItem"/>. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultFont"/> property.</returns>
		/// <PermissionSet>
		/// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
		/// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
		/// </PermissionSet>
		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				if( this.OwnerItem != null )
				{
					base.Font = value;
				}
				else
				{
					// To avoid null-ref exception in base font change processing.
					ToolStripDropDown dropDown = this.DropDown;
					this.DropDown = null;

					base.Font = value;

					this.DropDown = dropDown;
				}
			}
		}

		#endregion

		#region IMessageFilter Members
		/// <summary>
		/// Uses to paint SplitButtonEx in normal state after MouseUp event
		/// if cursor not in bounds of SplitButtonEx.
		/// </summary>
		/// <param name="m"> Message to process. </param>
		/// <returns></returns>
		public bool PreFilterMessage( ref Message m )
		{
			if( ( Msg )m.Msg == Msg.WM_LBUTTONUP )
			{
				Point pt = WindowsAPI.GetPointFromLPARAM( ( int )m.LParam );

				if( !this.Bounds.Contains( pt ) )
				{
					m_bLeftMouseButtonPressedOnImage = false;
					m_bLeftMouseButtonPressedOnButton = false;
				}

				Application.RemoveMessageFilter( this );
			}

			return false;
		}
		#endregion

		#region Properties
		/// <summary> Gets or sets Internal layout instance. </summary>
		internal SplitButtonExInternalLayout InternalLayout
		{
			get
			{
				if( m_InternalLayout == null )
				{
					m_InternalLayout = new SplitButtonExInternalLayout( this );
				}

				return m_InternalLayout;
			}
			set
			{
				m_InternalLayout = value;
			}
		}
		/// <summary> Gets or sets Image bounds. </summary>
		internal Rectangle ImageBounds
		{
			get
			{
				if( m_rcImage == Rectangle.Empty )
				{
					if( ( DisplayStyle & ToolStripItemDisplayStyle.Image ) == ToolStripItemDisplayStyle.Image )
					{
						if( RightToLeft == RightToLeft.Yes )
						{
							m_rcImage = new Rectangle( new Point( InternalLayout.SeparatorPosition, 0 ), new Size( Width - InternalLayout.SeparatorPosition, Height ) );
						}
						else
						{
							m_rcImage = new Rectangle( Point.Empty, new Size( InternalLayout.SeparatorPosition, Height ) );
						}
					}
				}

				return m_rcImage;
			}
		}
		/// <summary> Gets or sets Button bounds. </summary>
		internal Rectangle ButtonBounds
		{
			get
			{
				if( m_rcButton == Rectangle.Empty )
				{
					if( DisplayStyle == ToolStripItemDisplayStyle.Text || DisplayStyle == ToolStripItemDisplayStyle.None )
					{
						m_rcButton = new Rectangle( Point.Empty, Size );
					}
					else if( this.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText )
					{
						if( RightToLeft == RightToLeft.Yes )
						{
							m_rcButton = new Rectangle( Point.Empty, new Size( InternalLayout.SeparatorPosition, Height ) );
						}
						else
						{
							m_rcButton = new Rectangle( new Point( InternalLayout.SeparatorPosition, 0 ), new Size( Width - InternalLayout.SeparatorPosition, Height ) );
						}
					}
					else if( this.DisplayStyle == ToolStripItemDisplayStyle.Image ) 
					{
						if( RightToLeft == RightToLeft.Yes )
						{
							m_rcButton = new Rectangle( Point.Empty, new Size( InternalLayout.SeparatorPosition, Height ) );
						}
						else
						{
							m_rcButton = new Rectangle( new Point( InternalLayout.SeparatorPosition, 0 ), new Size( Width - InternalLayout.SeparatorPosition, Height ) );
						}
					}
				}

				return m_rcButton;
			}
		}
		/// <summary> Gets or sets current button state. </summary>
		internal SplitButtonState ButtonState
		{
			get
			{
				return m_eButtonState;
			}
			set
			{
				if( m_eButtonState != value )
				{
					m_eButtonState = value;
					Invalidate();
				}
			}
		}
		/// <summary> If we have enough height to locate DropDown under the SplitButtonEx then
		/// set DropDown location as BottomLeft corner of SplitButtonEx. 
		/// Otherwise DropDown will appear above the SplitButtonEx. </summary>
		protected override Point DropDownLocation
		{
			get
			{
				ToolStrip ts = this.Owner;

				if( ts != null && !this.DesignMode )
				{
					Screen screen = Screen.PrimaryScreen;

					int iScreenHeight = screen.Bounds.Height;
					int iDropDownHeight = this.DropDown.Height;

					Point pt = ts.PointToScreen( new Point( Bounds.Left, Bounds.Bottom ) );
					
					if( iScreenHeight - pt.Y > iDropDownHeight )
					{
						return pt;
					}
					else
					{
						pt.Y = ts.PointToScreen( this.Bounds.Location ).Y - iDropDownHeight;
						return pt;
					}
				}
				else
				{
					return base.DropDownLocation;
				}
			}
		}
		#endregion

		#region Fields
		/// <summary> Internal layout instance. </summary>
		private SplitButtonExInternalLayout m_InternalLayout;
		/// <summary> Current button state. </summary>
		private SplitButtonState m_eButtonState = SplitButtonState.None;
		/// <summary> Image bounds. </summary>
		private Rectangle m_rcImage;
		/// <summary> Button bounds. </summary>
		private Rectangle m_rcButton;
		/// <summary> Indicates if Left mouse button is pressed. </summary>
		private bool m_bLeftMouseButtonPressedOnImage = false;
		/// <summary> Indicates if Left mouse button is pressed. </summary>
		private bool m_bLeftMouseButtonPressedOnButton = false;
		#endregion
	}
}

#endif