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
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Runtime.InteropServices;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	partial class RibbonControlAdvHeader
	{
		/// <summary>
		/// Renderer for RibbonControlAdv.
		/// </summary>
		internal class RibbonControlAdvHeaderRenderer : Office12ToolStripRenderer
		{
			#region Constants
			/// <summary>
			/// 
			/// </summary>
			static double IMAGE_MARGIN_FACTOR = (1 - Math.Sin(Math.PI / 4))/2;
			/// <summary>
			/// 
			/// </summary>
			const int ARROW_WIDTH = 3;
			/// <summary>
			/// 
			/// </summary>
			const int ARROW_HEIGHT = 5;
			/// <summary>
			/// 
			/// </summary>
			const int ARROW_UP_DOWN_WIDTH = 5;
			/// <summary>
			/// 
			/// </summary>
			const int ARROW_UP_DOWN_HEIGHT = 3;
			/// <summary>
			/// 
			/// </summary>
			const int OFFICE_ARROW_WIDTH = 4;
			/// <summary>
			/// 
			/// </summary>
			const int OFFICE_ARROW_HEIGHT = 7;
			/// <summary>
			/// 
			/// </summary>
			private const int SCROLL_BUTTON_HEIGHT = 12;
			/// <summary> </summary>
			private const int SEPARATOR_HEIGHT = 2;
			#endregion

			#region Properties
			/// <summary> Right arrow on scroll button. </summary>
			protected Bitmap RightArrow
			{
				get
				{
					Bitmap bitmap = m_htBitmaps[ EBITMAP.ebRightArrow ] as Bitmap;

					if( bitmap == null )
					{
						bitmap = new Bitmap( ARROW_WIDTH, ARROW_HEIGHT );

						using( Graphics g = Graphics.FromImage( bitmap ) )
						{
							g.Clear( Color.Transparent );

							Rectangle rcRightArrow = new Rectangle( 0, 0, 1, ARROW_HEIGHT );

							using( Region rg = new Region( rcRightArrow ) )
							{
								rg.Union( rcRightArrow );

								rcRightArrow.Inflate( 0, -1 );
								rcRightArrow.X += 1;
								rg.Union( rcRightArrow );

								rcRightArrow.Inflate( 0, -1 );
								rcRightArrow.X += 1;
								rg.Union( rcRightArrow );

								using( Brush brush = new SolidBrush( OfficeColorTable.RibbonText ) )
								{
									g.FillRegion( brush, rg );
								}
							}
						}

						m_htBitmaps[ EBITMAP.ebRightArrow ] = bitmap;
					}

					return bitmap;
				}
			}
			/// <summary> Left arrow on scroll button. </summary>
			protected Bitmap LeftArrow
			{
				get
				{
					Bitmap bitmap = m_htBitmaps[ EBITMAP.ebLeftArrow ] as Bitmap;

					if( bitmap == null )
					{
						bitmap = new Bitmap( ARROW_WIDTH, ARROW_HEIGHT );

						using( Graphics g = Graphics.FromImage( bitmap ) )
						{
							g.Clear( Color.Transparent );

							Rectangle rcLeftArrow = new Rectangle( 0, 2, 1, 1 );

							using( Region rg = new Region( rcLeftArrow ) )
							{
								rg.Union( rcLeftArrow );

								rcLeftArrow.Inflate( 0, 1 );
								rcLeftArrow.X += 1;
								rg.Union( rcLeftArrow );

								rcLeftArrow.Inflate( 0, 1 );
								rcLeftArrow.X += 1;
								rg.Union( rcLeftArrow );

								using( Brush brush = new SolidBrush( OfficeColorTable.RibbonText ) )
								{
									g.FillRegion( brush, rg );
								}
							}
						}

						m_htBitmaps[ EBITMAP.ebLeftArrow ] = bitmap;
					}

					return bitmap;
				}
			}
			/// <summary> Down arrow on scroll button. </summary>
			protected Bitmap DownArrow
			{
				get
				{
					Bitmap bitmap = m_htBitmaps[ EBITMAP.ebDownArrow ] as Bitmap;

					if( bitmap == null )
					{
						bitmap = new Bitmap( ARROW_UP_DOWN_WIDTH, ARROW_UP_DOWN_HEIGHT );

						using( Graphics g = Graphics.FromImage( bitmap ) )
						{
							g.Clear( Color.Transparent );

							Rectangle rcDownArrow = new Rectangle( 0, 0, ARROW_HEIGHT, 1 );

							using( Region rg = new Region( rcDownArrow ) )
							{
								rg.Union( rcDownArrow );

								rcDownArrow.Inflate( -1, 0 );
								rcDownArrow.Y += 1;
								rg.Union( rcDownArrow );

								rcDownArrow.Inflate( -1, 0 );
								rcDownArrow.Y += 1;
								rg.Union( rcDownArrow );

								using( Brush brush = new SolidBrush( OfficeColorTable.RibbonText ) )
								{
									g.FillRegion( brush, rg );
								}
							}
						}

						m_htBitmaps[ EBITMAP.ebDownArrow ] = bitmap;
					}

					return bitmap;
				}
			}
			/// <summary> Up arrow on scroll button. </summary>
			protected Bitmap UpArrow
			{
				get
				{
					Bitmap bitmap = m_htBitmaps[ EBITMAP.ebUpArrow ] as Bitmap;

					if( bitmap == null )
					{
						bitmap = new Bitmap( ARROW_UP_DOWN_WIDTH, ARROW_UP_DOWN_HEIGHT );

						using( Graphics g = Graphics.FromImage( bitmap ) )
						{
							g.Clear( Color.Transparent );

							Rectangle rcUpArrow = new Rectangle( 0, 2, ARROW_HEIGHT, 1 );

							using( Region rg = new Region( rcUpArrow ) )
							{
								rg.Union( rcUpArrow );

								rcUpArrow.Inflate( -1, 0 );
								rcUpArrow.Y -= 1;
								rg.Union( rcUpArrow );

								rcUpArrow.Inflate( -1, 0 );
								rcUpArrow.Y -= 1;
								rg.Union( rcUpArrow );

								using( Brush brush = new SolidBrush( OfficeColorTable.RibbonText ) )
								{
									g.FillRegion( brush, rg );
								}
							}
						}

						m_htBitmaps[ EBITMAP.ebUpArrow ] = bitmap;
					}

					return bitmap;
				}
			}
			#endregion

			#region Constructor/Destructor
			/// <summary>
			/// 
			/// </summary>
			static RibbonControlAdvHeaderRenderer()
			{
				m_blTabSelected = new Blend();
				m_blTabSelected.Positions = new float[] { 0.0f, 0.5f, 0.6f, 1.0f };
				m_blTabSelected.Factors = new float[] { 0.5f, 0.0f, 0.5f, 1.0f };

				m_blTabChecked = new Blend();
				m_blTabChecked.Positions = new float[] {0.0F, 0.6F, 0.8F, 1.0F };
				m_blTabChecked.Factors = new float[] { 1.0F, 0.8F, 0.5F, 0.8F };

				m_blTabFlash = new Blend();
				m_blTabFlash.Positions = new float[] { 0f, 0.4f, 1f };
				m_blTabFlash.Factors = new float[] { 0f, 0.6f, 1f };

				m_blTitle = new Blend();
				m_blTitle.Positions = new float[] { 0.0F, 0.27F, 0.27F, 1.0F };
				m_blTitle.Factors = new float[] { 0.0F, 0.2F, 1.0F, 0.0F };

				m_blGroupTab = new Blend();
				m_blGroupTab.Positions = new float[] { 0.0F, 0.2F, 0.2F, 1.0F };
				m_blGroupTab.Factors = new float[] { 0.2F, 0.0F, 0.8F, 0.0F };

				m_blGroupTop = new Blend();
				m_blGroupTop.Positions = new float[] { 0.0F, 0.6F, 1.0F };
				m_blGroupTop.Factors = new float[] { 0.0F, 0.2F, 0.6F };

				m_blGroupRays = new Blend();
				m_blGroupRays.Positions = new float[] { 0.0F, 0.3F, 0.85F, 1.0F };
				m_blGroupRays.Factors = new float[] { 0.0F, 0.2F, 0.0F, 0.0F };

				m_blSystemButton= new Blend();
				m_blSystemButton.Positions = new float[] { 0.0F, 0.5F, 0.5F, 1.0F };
				m_blSystemButton.Factors = new float[] { 0.2F, 0.0F, 1.0F, 0.5F };

				m_blSystemButtonBorder = new Blend();
				m_blSystemButtonBorder.Positions = new float[] { 0.0f, 0.5f, 1.0f };
				m_blSystemButtonBorder.Factors = new float[] { 0.5f, 1.0f, 0.5f };
			}
			/// <summary>
			/// Creates & initializes new instance of RibbonControlAdvHeaderRenderer.
			/// </summary>
			/// <param name="colorTable"></param>
			public RibbonControlAdvHeaderRenderer(Office12ColorTable colotTable) : base(colotTable)
			{
				m_hImages = new Bitmaps((int)EIMAGE.MAX);
			}
			/// <summary>
			/// 
			/// </summary>
			~RibbonControlAdvHeaderRenderer()
			{
				Clear();
			}
			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
			{
				if (!SystemInfo.IsVisualStyleEnabled || !PaintToolstripBackground(e))
				{
					base.OnRenderToolStripBackground(e);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
			{
				if (!(e.ToolStrip is MenuDropDown))
				{
					base.OnRenderToolStripBorder(e);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
			{
				if (!SystemInfo.IsVisualStyleEnabled || !PaintButtonBackground(e))
				{
					base.OnRenderButtonBackground(e);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
			{
				if (!SystemInfo.IsVisualStyleEnabled || !PaintDropDownButtonBackground(e))
				{
					base.OnRenderDropDownButtonBackground(e);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
			{
				if (!SystemInfo.IsVisualStyleEnabled || !PaintItemImage(e))
				{
					base.OnRenderItemImage(e);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnRenderArrow( ToolStripArrowRenderEventArgs e )
			{
				if ( !( e.Item is ToolStripMenuButton ) )
				{
					if( e.Item is OfficeDropDownButton )
					{
						PaintArrow( e );
					}
					else
					{
						base.OnRenderArrow( e );
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnRenderItemText( ToolStripItemTextRenderEventArgs e )
			{
				if( !SystemInfo.IsVisualStyleEnabled || !PaintItemText( e ) )
				{
					base.OnRenderItemText( e );
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnRenderSplitButtonBackground( ToolStripItemRenderEventArgs e )
			{
				if( !SystemInfo.IsVisualStyleEnabled || !PaintSplitButtonBackground( e ) )
				{
					base.OnRenderSplitButtonBackground( e );
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
			{
				if (!SystemInfo.IsVisualStyleEnabled || !PaintSeparator(e))
				{
					base.OnRenderSeparator(e);
				}
			}
			#endregion

			#region Implementation
			private bool PaintSeparator(ToolStripSeparatorRenderEventArgs e)
			{
				bool bResult = false;

				MenuDropDown.IPanel panel = e.Item.Owner as MenuDropDown.IPanel;
				if (panel != null)
				{
					Graphics g = e.Graphics;
					Size size = e.Item.Size;

					switch (panel.PanelType)
					{
						case MenuDropDown.PanelType.System:
                            g.DrawLine(this.ShadowPen, 1, 0, 1, size.Height);
                            g.DrawLine(this.HighlightPen, 2, 0, 2, size.Height);
							break;
						default:
                            g.DrawLine(this.ShadowPen, 0, 1, size.Width, 1);
                            g.DrawLine(this.HighlightPen, 0, 2, size.Width, 2);
							break;
					}
					bResult = true;
				}
				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintItemText( ToolStripItemTextRenderEventArgs e )
			{
				bool bResult = PaintToolStripTabItemText( e );

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintToolStripTabItemText( ToolStripItemTextRenderEventArgs e )
			{
				bool bResult = false;

				ToolStripTabItem item = e.Item as ToolStripTabItem;
				if (item!=null)
				{
					Rectangle rc = ToolStripRendererUtils.GetTextRect(e);
					TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix ;

					RibbonControlAdvHeader header = e.Item.Owner as RibbonControlAdvHeader;

					if( header != null )
					{
						Rectangle rcItemBounds = new Rectangle( new Point( e.Item.Bounds.Left + rc.X, e.Item.Bounds.Top + rc.Y ), rc.Size );
						Rectangle rcIntersection = Rectangle.Intersect( header.TabItemsRectangle, rcItemBounds );

						int iTextWidthDifference = rcItemBounds.Width - rcIntersection.Width;
						rc.Width -= iTextWidthDifference;

						if( rcItemBounds.Left < header.TabItemsRectangle.Left && rcIntersection.Width > 0 )
						{
							int iLeft = rcItemBounds.Width - rcIntersection.Width;

							if( header.m_bIsLeftScroll )
							{
								iLeft += SCROLL_BUTTON_WIDTH;
								rc.Width -= SCROLL_BUTTON_WIDTH;
							}

							rc.X += iLeft;
							rc.Width -= 2;

							flags |= TextFormatFlags.Right;
						}
						else
						{
							flags |= TextFormatFlags.Left;
							rc.Width -= 2;
						}
					}

					Color clText = item.Checked && !this.IsPanelHidden(item) ? this.RibbonTabText : this.RibbonTabInactiveText;
					TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, rc, clText, flags);

					bResult = true;	
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintItemImage( ToolStripItemImageRenderEventArgs e )
			{
				bool bResult = ( PaintToolStripMenuButtonImage( e )
											|| PaintToolStripTabItemImage( e ) );

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintToolStripMenuButtonImage( ToolStripItemImageRenderEventArgs e )
			{
				bool bResult = false;

				ToolStripMenuButton button = e.Item as ToolStripMenuButton;

				if( button != null )
				{
					if( e.Image != null )
					{
						Rectangle rc = GetMenuButtonRectangle( button );                      
						PaintMenuButtonImage( e.Graphics, ref rc, button );
					}

					bResult = true;
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintToolStripTabItemImage( ToolStripItemImageRenderEventArgs e )
			{
				bool bResult = false;

				ToolStripTabItem item = e.Item as ToolStripTabItem;

				if( item != null )
				{
					Image image = e.Image;
					Rectangle rc = ToolStripRendererUtils.GetImageRect(e);

					if( image != null && rc.Width > 0 && rc.Height > 0 )
					{
						RibbonControlAdvHeader header = e.Item.Owner as RibbonControlAdvHeader;

						if( header != null )
						{
							Rectangle rcImage = new Rectangle( new Point( e.Item.Bounds.Left + rc.X, e.Item.Bounds.Top + rc.Y ), rc.Size ); 
							Rectangle rcIntersection = Rectangle.Intersect( header.TabItemsRectangle, rcImage );

							int iTextWidthDifference = rcImage.Width - rcIntersection.Width;
							rc.Width -= iTextWidthDifference;

							if( rcImage.Left < header.TabItemsRectangle.Left && rcIntersection.Width > 0 )
							{
								int iLeft = rcImage.Width - rcIntersection.Width;

								if( header.m_bIsLeftScroll )
								{
									iLeft += SCROLL_BUTTON_WIDTH;
									rcImage.Width -= SCROLL_BUTTON_WIDTH;
								}

								rc.X += iLeft;
							}
						}

						e.Graphics.DrawImage( image, rc );
					}

					bResult = true;
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintToolstripBackground(ToolStripRenderEventArgs e)
			{
				bool bResult = ( PaintHeaderBackground( e ) ||
					PaintDropDownBackground( e ) || 
					PaintDropDownExBackground( e ) );

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintHeaderBackground(ToolStripRenderEventArgs e)
			{
				bool bResult = false;

				RibbonControlAdvHeader header = e.ToolStrip as RibbonControlAdvHeader;
				if (header != null)
				{
					Graphics g = e.Graphics;

					Rectangle rcDisplay = header.DisplayRectangle;

					// Draw Title's background
					int titleHeight = rcDisplay.Y + header.QuickPanelHeight + 1;
					Rectangle rcTitle = new Rectangle(0, 0, header.Width, titleHeight);

					RibbonForm form = header.Form;

					bool bCompositionEnabled = (form != null) ? form.CompositionEnabled && header.m_owner.Dock == DockStyleEx.TopMost : false;

					if (!bCompositionEnabled || form.FormBorderStyle == FormBorderStyle.None)
					{
						bool bActive = (form != null) ? form.ActiveState : false;

						// Paint Title background depend on form state.
						Color clBegin = bActive ? this.ActiveTitleGradientBegin : this.InActiveTitleGradientBegin;
						Color clEnd = bActive ? this.ActiveTitleGradientEnd : this.InActiveTitleGradientEnd;

						using (LinearGradientBrush brush = GetVerticalBrush(ref rcTitle, clBegin, clEnd))
						{
							brush.Blend = m_blTitle;
							g.FillRectangle(brush, rcTitle);
						}

						// Draw separators
						using (Pen penLight = bActive ?
							new Pen(OfficeColorTable.HeaderSeparatorLight) :
							new Pen(OfficeColorTable.HeaderSeparatorLightInActive))
						{
							using (Pen penDark = bActive ?
								new Pen(OfficeColorTable.HeaderSeparatorDark) :
								new Pen(OfficeColorTable.HeaderSeparatorDarkInActive))
							{
								g.DrawLine(penLight, 0, titleHeight - 1, rcTitle.Right, titleHeight - 1);
								g.DrawLine(penDark, 0, titleHeight, rcTitle.Right, titleHeight);
							}
						}
					}
					else DwmAPI.FillBlackRegion(g, rcTitle);

					// Draw Quick items region
					if( !header.ShowQuickPanelBelowRibbon && header.QuickItems.Count > 0 )
					{
						if (header.RightToLeft == RightToLeft.No)
						{
							PaintQuickItemsBackgroundLeftToRight( g, header );
						}
						else
						{
							PaintQuickItemsBackgroundRightToLeft( g, header );
						}
					}

					// Draw Tabs background
					Rectangle rcTabs = new Rectangle(0, titleHeight, header.Width, header.Height - titleHeight);
					using (Brush brush = new SolidBrush(this.RibbonBorder))
					{
						Rectangle rcTabBkg = rcTabs;

						if (!bCompositionEnabled)
						{
							if (form != null)
							{
								if (!form.ActiveState)
								{
									using (Brush brushBorder = new SolidBrush(OfficeColorTable.RibbonBorderInactive))
									{
										g.FillRectangle(brushBorder, rcTabBkg);

										Padding padding = form.BordersInternal;

										rcTabBkg.X += padding.Left;
										rcTabBkg.Width -= padding.Horizontal;
									}
								}
							}
						}

						g.FillRectangle(brush, rcTabBkg);
					}

					string sTitle = header.Title;
					if (sTitle != string.Empty)
					{
						Color titleColor = header.TitleColor == Color.Empty ? this.RibbonTitleText : header.TitleColor;
                        if (form != null)
                        {
                            bool mdiactivate = false;
                            foreach (Form mdi in form.MdiChildren)
                            {
                                if (mdi.IsHandleCreated)
                                {
                                    mdiactivate = true;
                                }
                            }
                            if (form.ContainsFocus || form.designmode || mdiactivate)
                            {
                                if (form.CompositionEnabled)
                                {
                                    DrawThemeText(g, header.Handle, header.TitleRect, sTitle, header.TitleFont, titleColor, false);
                                }
                                else
                                {
                                    TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix;
                                    TextRenderer.DrawText(g, sTitle, header.TitleFont, header.TitleRect, titleColor, flags);
                                }
                            }
                            else
                            {
                                if (form.CompositionEnabled)
                                {
                                    DrawThemeText(g, header.Handle, header.TitleRect, sTitle, header.TitleFont, ControlPaint.LightLight(titleColor), false);
                                }
                                else
                                {
                                    TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix;
                                    TextRenderer.DrawText(g, sTitle, header.TitleFont, header.TitleRect, ControlPaint.LightLight(titleColor), flags);
                                }
                            }
                        }
					}
					foreach (ToolStripTabGroup group in header.Groups)
					{
						if (group.Visible)
						{
							for (int i = 0, count = group.BoundsList.Count; i < count; i++)
							{
								Rectangle rect = group.BoundsList[ i ];

								using (LinearGradientBrush brush = GetVerticalBrush(ref rect, Color.Transparent, group.Color))
								{
									brush.Blend = m_blGroupTop;
									brush.WrapMode = WrapMode.TileFlipXY;
									g.FillRectangle( brush, rect );
								}

								Rectangle fullRect = new Rectangle(rect.X, rect.Y, rect.Width, header.Height);

								using( LinearGradientBrush brush = GetVerticalBrush( ref fullRect, Color.Transparent, Color.Black ) )
								{
									brush.Blend = m_blGroupRays;
									g.FillRectangle( brush, fullRect.X, fullRect.Y, 1, fullRect.Height );
									g.FillRectangle( brush, fullRect.Right - 1, fullRect.Y, 1, fullRect.Height );
								}

								if (form != null && form.CompositionEnabled)
								{
                                    DrawThemeText(g, header.Handle, rect, group.Name, group.Font, this.RibbonTabInactiveText, true);
								}
								else
								{
                                    TextRenderer.DrawText(g, group.Name, group.Font, rect, this.RibbonTabInactiveText,
										TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
								}
							}
						}
					}

					bResult = true;
				}

				return bResult;
			}

            void mdi_GotFocus(object sender, EventArgs e)
            {
                (sender as Form).ParentForm.Invalidate();
            }

            void ActiveMdiChild_GotFocus(object sender, EventArgs e)
            {
                (sender as Form).ParentForm.Invalidate();
            }
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintDropDownBackground(ToolStripRenderEventArgs e)
			{
				bool bResult = false;

				MenuDropDown dropDown = e.ToolStrip as MenuDropDown;

				if (dropDown != null)
				{
					Graphics g = e.Graphics;

					Point ptOffset = Point.Empty;

					PanelItemRenderEventArgs pie = e as PanelItemRenderEventArgs;

					if (pie != null)
					{
						Point offset = pie.PanelStrip.Location;

						ptOffset.X = -offset.X;
						ptOffset.Y = -offset.Y;
					}

					Rectangle rc = new Rectangle(ptOffset, dropDown.Size);

					using( Region rgBorder = new Region( rc ) )
					{
						Padding borders = dropDown.Borders;
						Rectangle rcDisplay = new Rectangle( rc.X + borders.Left, rc.Y + borders.Top, rc.Width - borders.Horizontal, rc.Height - borders.Vertical );

						rgBorder.Exclude( rcDisplay );

						Color clBegin = this.OfficeColorTable.MenuButtomDropDownGradientBegin;
						Color clEnd = this.OfficeColorTable.MenuButtomDropDownGradientEnd;

						using (LinearGradientBrush brush = new LinearGradientBrush(rc.Location, new Point(rc.X, rc.Bottom), clBegin, clEnd))
						{
							float fHeight = (float)rc.Height;

							float pos1 = (borders.Top * 0.4f) / fHeight;
							float pos2 = ((float)rc.Bottom - borders.Bottom * 0.6f) / fHeight;

							Blend blend = new Blend();
							blend.Positions = new float[] { 0.0f, pos1, pos1, pos2, pos2, 1.0f };
							blend.Factors = new float[] { 0.0f, 0.0f, 0.6f, 0.2f, 1.0f, 0.0f, };

							brush.Blend = blend;
							g.FillRectangle(brush, rc);
						}

						g.FillRectangle( this.ToolStripDropDownBackground, rcDisplay );

						SmoothingMode saveMode = g.SmoothingMode;
						g.SmoothingMode = SmoothingMode.AntiAlias;

						using( Pen pen = new Pen( Office12ColorTable.GetAlphaBlendedColor( Color.White, clBegin, 32 )/*Color.FromArgb(80, Color.White)*/) )
						{
							g.DrawRectangle( pen, rcDisplay.X - 2, rcDisplay.Y - 2, rcDisplay.Width + 3, rcDisplay.Height + 3 );
							g.DrawPolygon( pen, RendererUtils.GetRoundedPolygon( Rectangle.Inflate( rc, -1, -1 ), 1 ) );
						}
						using( Pen pen = new Pen( this.OfficeColorTable.MenuButtomDropDownBorder ) )
						{
							g.DrawRectangle( pen, rcDisplay.X - 1, rcDisplay.Y - 1, rcDisplay.Width + 1, rcDisplay.Height + 1 );
							g.DrawPolygon( pen, RendererUtils.GetRoundedPolygon( rc, 2 ) );
						}

						g.SmoothingMode = saveMode;

						ToolStripMenuButton button = dropDown.OwnerItem as ToolStripMenuButton;
						if( button != null )
						{
							Control owner = button.GetCurrentParent();
							if( owner != null )
							{
								Rectangle rcButton = owner.RectangleToScreen( button.Bounds );

								if( dropDown.Parent != null )
								{
									rcButton = dropDown.Parent.RectangleToClient( rcButton );
								}

								rcButton.Offset( ptOffset );

								rcButton.Inflate( -MENUBUTTON_PADDING, -MENUBUTTON_PADDING );

								rcButton.X -= dropDown.Bounds.X;
								rcButton.Y -= dropDown.Bounds.Y;

								if( rcButton.IntersectsWith( Rectangle.Ceiling( g.ClipBounds ) ) )
								{
									PaintMenuButtonBackground( g, ref rcButton, button );
									PaintMenuButtonImage( g, ref rcButton, button );
								}
							}
						}

						int iAuxItemsWidth = dropDown.AuxItemsBounds.Width;
						int iMainItemsWidth = dropDown.MainItemsBounds.Width;
						Rectangle rcBounds = dropDown.AuxItemsBounds;

						if( iAuxItemsWidth > 0 )
						{
							using( Brush brush = new SolidBrush( OfficeColorTable.PanelBackground ) )
							{
								g.FillRectangle( brush, dropDown.AuxItemsBounds );
							}

							int iCaptionHeight = dropDown.AuxPanel.CaptionHeight;

							if( iCaptionHeight > 0 )
							{
								// Horizontal separator.
								g.DrawLine(ShadowPen, rcBounds.X, rcBounds.Y + iCaptionHeight - 2, rcBounds.Right - 1, rcBounds.Y + iCaptionHeight - 2);
								g.DrawLine(HighlightPen, rcBounds.X, rcBounds.Y + iCaptionHeight - 1, rcBounds.Right - 1, rcBounds.Y + iCaptionHeight - 1);

								TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix;
								flags |= (dropDown.RightToLeft == RightToLeft.Yes) ? TextFormatFlags.Right : TextFormatFlags.Left;

								Rectangle rcCaptionBounds = new Rectangle(rcBounds.X, rcBounds.Y, rcBounds.Width, iCaptionHeight - SEPARATOR_HEIGHT);
								TextRenderer.DrawText(e.Graphics, dropDown.AuxPanel.Text, dropDown.AuxPanel.Font, rcCaptionBounds, dropDown.ForeColor, flags);
							}
						}

						if( iAuxItemsWidth > 0 && iMainItemsWidth > 0 )
						{
							// Vertical separator.
							if( dropDown.RightToLeft == RightToLeft.Yes )
							{
								g.DrawLine(ShadowPen, rcBounds.Right + 1, rcBounds.Y, rcBounds.Right + 1, rcBounds.Bottom - 1);
								g.DrawLine(HighlightPen, rcBounds.Right + 2, rcBounds.Y, rcBounds.Right + 2, rcBounds.Bottom - 1);
							}
							else
							{
								g.DrawLine(ShadowPen, rcBounds.X - 2, rcBounds.Y, rcBounds.X - 2, rcBounds.Bottom - 1);
								g.DrawLine(HighlightPen, rcBounds.X - 1, rcBounds.Y, rcBounds.X - 1, rcBounds.Bottom - 1);
							}
						}
					}

					bResult = true;
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintDropDownExBackground(ToolStripRenderEventArgs e)
			{
				bool bResult = false;

				OfficeDropDown dropDown = e.ToolStrip as OfficeDropDown;

				if( dropDown != null )
				{
					if( dropDown.Width > 0 && dropDown.Height > 0 )
					{
						Rectangle rc = new Rectangle( Point.Empty, dropDown.Size );

						if (!RendererUtils.IsValidRegion(dropDown, e.Graphics))
						{
							dropDown.Region = RendererUtils.GetRoundedRegion( rc, TOOLSTRIP_RADIUS );
						}

						e.Graphics.FillRectangle( ToolStripDropDownBackground, rc );

						int iCaptionHeight = dropDown.CaptionHeight;

						if( iCaptionHeight > 0 )
						{
							Rectangle rcCaptionBounds = new Rectangle( rc.X, rc.Y, rc.Width, iCaptionHeight - SEPARATOR_HEIGHT );
                            using (Brush brush =new SolidBrush( OfficeColorTable.ContextMenuTitle ))
							e.Graphics.FillRectangle(brush , rcCaptionBounds );

							TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix;
							flags |= ( dropDown.RightToLeft == RightToLeft.Yes ) ? TextFormatFlags.Right : TextFormatFlags.Left;

							TextRenderer.DrawText( e.Graphics, dropDown.CaptionText, dropDown.CaptionFont, rcCaptionBounds, dropDown.ForeColor, flags );

							// Horizontal separator.
							e.Graphics.DrawLine(ShadowPen, rcCaptionBounds.X, rcCaptionBounds.Y + iCaptionHeight - 2, rcCaptionBounds.Right - 1, rcCaptionBounds.Y + iCaptionHeight - 2);
							e.Graphics.DrawLine(HighlightPen, rcCaptionBounds.X, rcCaptionBounds.Y + iCaptionHeight - 1, rcCaptionBounds.Right - 1, rcCaptionBounds.Y + iCaptionHeight - 1);
						}
					}

					bResult = true;
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="header"></param>
			private void PaintQuickItemsBackgroundLeftToRight(Graphics g,RibbonControlAdvHeader header)
			{
				GraphicsState gState = g.Save();

				ToolStripMenuButton menuButton = header.m_menuButton;
				Rectangle rcMenuButton = menuButton.Bounds;
				int left = 0;
				if( menuButton.Available )
				{
					left = rcMenuButton.Right + MENUBUTTON_GAP;
				}
				else
				{
					rcMenuButton.Width = 0;
					left = rcMenuButton.Left;
				}

				if( header.QuickPanelVisible )
				{
					int height = header.QuickPanelHeight - 1;

					if( height > 0 )
					{
						Rectangle rcDisplay = header.DisplayRectangle;

						Rectangle rc = new Rectangle(left, rcDisplay.Y, header.QuickPanelWidth, height);
						Rectangle rcBrush = new Rectangle( rc.X, rc.Y - 1, 1, rc.Height + 2 );

						using( GraphicsPath pathBorder = GetQuickPathLeftToRight( rc ) )
						{
							using( GraphicsPath pathMenuButton = new GraphicsPath() )
							{
								if( header.MenuButtonVisible )
								{
									pathMenuButton.AddEllipse( Rectangle.Inflate( rcMenuButton, MENUBUTTON_GAP, MENUBUTTON_GAP ) );
								}
								else
								{
									pathMenuButton.AddRectangle( new Rectangle( 0, 0, rcMenuButton.Left, rcMenuButton.Height ) );
								}
								g.SetClip( pathMenuButton, CombineMode.Exclude );
							}

							RibbonForm form = header.Form;

							bool bActive = (form != null) ? form.ActiveState : false;

							Color clBegin = bActive ? this.QuickPanelGradientBegin : this.InactiveQuickPanelGradientBegin;
							Color clEnd = bActive ? this.QuickPanelGradientEnd : this.InactiveQuickPanelGradientEnd;

							bool bCompositionEnabled = (form != null) ? form.CompositionEnabled && header.m_owner.Dock == DockStyleEx.TopMost : false;
							if (!bCompositionEnabled)
							{
								using (LinearGradientBrush brush = new LinearGradientBrush(rcBrush, clBegin, clEnd, LinearGradientMode.Vertical))
								{
									g.FillPath(brush, pathBorder);
								}
							}

							g.SmoothingMode = SmoothingMode.AntiAlias;

							int x = rc.X - height;
							int right = rc.Right - 1;
							int bottom = rc.Bottom - 1;

							using( GraphicsPath path = new GraphicsPath() )
							{
								path.AddArc( rc.Right - height / 2, rc.Y, height, height, -90f, 180f );
								path.AddLine( right, bottom + 1, x, bottom + 1 );

								using( Pen pen = new Pen( Color.FromArgb( 64, Color.White ) ) )
								{
									g.DrawPath( pen, path );
								}
							}

							Color clTop = Office12ColorTable.GetAlphaBlendedColor( Color.Black, clBegin, 32 );
							Color clBottom = Office12ColorTable.GetAlphaBlendedColor( Color.Black, clEnd, 32 );

							using( Brush brush = new LinearGradientBrush( rcBrush, clTop, clBottom, LinearGradientMode.Vertical ) )
							{
								using( Pen pen = new Pen( brush ) )
								{
									using( GraphicsPath path = new GraphicsPath() )
									{
										path.AddLine( x, rc.Y, right, rc.Y );
										path.AddArc( rc.Right - height / 2, rc.Y, height - 1, height - 1, -90f, 180f );
										path.AddLine( x, bottom, right, bottom );

										g.DrawPath( pen, path );
									}

									g.SetClip( pathBorder, CombineMode.Replace );

									if( header.MenuButtonVisible )
									{
										g.DrawEllipse( pen, Rectangle.Inflate( rcMenuButton, MENUBUTTON_GAP, MENUBUTTON_GAP ) );
									}
									else
									{
										g.DrawLine( pen, rcMenuButton.Location, new Point( rcMenuButton.Left, rcMenuButton.Bottom ) );
									}
								}
							}
						}
					}
				}

				g.Restore(gState);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="header"></param>
			private void PaintQuickItemsBackgroundRightToLeft(Graphics g, RibbonControlAdvHeader header)
			{
				GraphicsState gState = g.Save();

				ToolStripMenuButton menuButton = header.m_menuButton;
				Rectangle rcMenuButton = menuButton.Bounds;
				int left = 0;

				if (menuButton.Available)
				{
					left = rcMenuButton.Left - MENUBUTTON_GAP;
				}
				else
				{
					rcMenuButton.Width = 0;
					left = rcMenuButton.Left;
				}

				if (header.QuickPanelVisible)
				{
					int height = header.QuickPanelHeight - 1;

					if (height > 0)
					{
						Rectangle rcDisplay = header.DisplayRectangle;

						Rectangle rc = new Rectangle(left - header.QuickPanelWidth, rcDisplay.Y, header.QuickPanelWidth, height);
						Rectangle rcBrush = new Rectangle(rc.X, rc.Y - 1, 1, rc.Height + 2);

						using (GraphicsPath pathBorder = GetQuickPathRightToLeft(rc))
						{
							using (GraphicsPath pathMenuButton = new GraphicsPath())
							{
								if (header.MenuButtonVisible)
								{
									pathMenuButton.AddEllipse(Rectangle.Inflate(rcMenuButton, MENUBUTTON_GAP, MENUBUTTON_GAP));
								}
								else
								{
									pathMenuButton.AddRectangle(new Rectangle(0, 0, rcMenuButton.Left, rcMenuButton.Height));
								}
								g.SetClip(pathMenuButton, CombineMode.Exclude);
							}

							RibbonForm form = header.Form;

							bool bActive = (form != null) ? form.ActiveState : false;

							Color clBegin = bActive ? this.QuickPanelGradientBegin : this.InactiveQuickPanelGradientBegin;
							Color clEnd = bActive ? this.QuickPanelGradientEnd : this.InactiveQuickPanelGradientEnd;

							bool bCompositionEnabled = (form != null) ? form.CompositionEnabled && header.m_owner.Dock == DockStyleEx.TopMost : false;
							if (!bCompositionEnabled)
							{
								using (LinearGradientBrush brush = new LinearGradientBrush(rcBrush, clBegin, clEnd, LinearGradientMode.Vertical))
								{
									g.FillPath(brush, pathBorder);
								}
							}

							g.SmoothingMode = SmoothingMode.AntiAlias;

							int x = rc.Right + height;
							left = rc.Left + 1;
							int bottom = rc.Bottom - 1;

							using (GraphicsPath path = new GraphicsPath())
							{
								path.AddArc(rc.Left - height / 2, rc.Y, height, height, -90f, -180f);
								path.AddLine(left, bottom + 1, rc.Right + height, bottom + 1);

								using (Pen pen = new Pen(Color.FromArgb(64, Color.White)))
								{
									g.DrawPath(pen, path);
								}
							}

							Color clTop = Office12ColorTable.GetAlphaBlendedColor(Color.Black, clBegin, 32);
							Color clBottom = Office12ColorTable.GetAlphaBlendedColor(Color.Black, clEnd, 32);

							using (Brush brush = new LinearGradientBrush(rcBrush, clTop, clBottom, LinearGradientMode.Vertical))
							{
								using (Pen pen = new Pen(brush))
								{
									using (GraphicsPath path = new GraphicsPath())
									{
										path.AddLine(x, rc.Y, left, rc.Y);
										path.AddArc(rc.X - height / 2, rc.Y, height - 1, height - 1, -90f, -180f);
										path.AddLine(x, bottom, left, bottom);

										g.DrawPath(pen, path);
									}

									g.SetClip(pathBorder, CombineMode.Replace);

									if (header.MenuButtonVisible)
									{
										g.DrawEllipse(pen, Rectangle.Inflate(rcMenuButton, MENUBUTTON_GAP, MENUBUTTON_GAP));
									}
									else
									{
										g.DrawLine(pen, new Point( rcMenuButton.Right, rcMenuButton.Y ), new Point(rcMenuButton.Right, rcMenuButton.Bottom));
									}
								}
							}
						}
					}
				}

				g.Restore(gState);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintButtonBackground( ToolStripItemRenderEventArgs e )
			{
				bool bResult =	PaintButtonBackGround( e ) ||
								PaintTabItemBackGround( e ) ||
								PaintSystemButtonBackground( e ) ||
								PaintButtonOnSystemPanelBackGround( e );

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintTabItemBackGround(ToolStripItemRenderEventArgs e)
			{
				if( e.Item is ToolStripTabItem )
				{
					GraphicsState saveState = e.Graphics.Save();

					RibbonControlAdvHeader header = e.Item.Owner as RibbonControlAdvHeader;

					if( header != null )
					{
						Rectangle rcItemBounds = e.Item.Bounds;
						Rectangle rcIntersection = Rectangle.Intersect( header.TabItemsRectangle, rcItemBounds );

						Point p = Point.Empty;

						if( rcItemBounds.Left < header.TabItemsRectangle.Left && rcIntersection.Width > 0 )
						{
							p.X = header.TabItemsRectangle.Left - rcItemBounds.Left;

							if( header.m_bIsLeftScroll )
							{
								p.X += SCROLL_BUTTON_WIDTH;
								rcIntersection.Width -= SCROLL_BUTTON_WIDTH;
							}
						}			

						Rectangle rcTabItemsRectangle = new Rectangle( p, rcIntersection.Size );

						e.Graphics.SetClip( rcTabItemsRectangle );
					}

					if( !PaintTabItemChecked( e ) )
					{
						PaintTabItemSelected( e );
					}

					e.Graphics.Restore(saveState);

					return true;
				}
				return false;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintSystemButtonBackground(ToolStripItemRenderEventArgs e)
			{
				if (e.Item is SystemButton)
				{
					if (!PaintSystemButtonPressed(e))
					{
						PaintSystemButtonSelected(e);
					}
					return true;
				}
				return false;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintDropDownButtonBackground(ToolStripItemRenderEventArgs e)
			{
				bool bResult = ( PaintDropDownMenuButtonBackground( e ) ||
					PaintQuickItemsDropDownButtonBackground( e ) ||
					PaintButtonBackGround( e ) );

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintQuickItemsDropDownButtonBackground( ToolStripItemRenderEventArgs e )
			{
				bool result = false;
				if( e.Item is QuickItemsDropDownButton )
				{
					result = PaintOverflowButtonBackground( e );
				}
				return result;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintDropDownMenuButtonBackground( ToolStripItemRenderEventArgs e )
			{
				bool bResult = false;

				ToolStripMenuButton button = e.Item as ToolStripMenuButton;
				if( button != null )
				{
					Rectangle rc = GetMenuButtonRectangle( button );

					PaintMenuButtonBackground( e.Graphics, ref rc, button );

					bResult = true;
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private void PaintArrow( ToolStripArrowRenderEventArgs e )
			{
				Graphics g = e.Graphics;
				ToolStrip toolStrip = e.Item.Owner;
				Rectangle rect;
				Image imgArrow;
				Point p;

				if( toolStrip != null && toolStrip.RightToLeft == RightToLeft.Yes )
				{
					if( e.Item is OfficeSplitButton )
					{
						rect = e.ArrowRectangle;
					}
					else
					{
						rect = new Rectangle( 0, 0, e.Item.Height / 2, e.Item.Height );
					}

					imgArrow = OfficeLeftArrow;

					p = new Point( rect.Left + ( rect.Width - OFFICE_ARROW_WIDTH ) / 2, ( rect.Height - OFFICE_ARROW_HEIGHT ) / 2 + 1 );
				}
				else
				{
					if( e.Item is OfficeSplitButton )
					{
						rect = e.ArrowRectangle;
					}
					else
					{
						rect = new Rectangle( e.Item.Width - e.Item.Height / 2, 0, e.Item.Height / 2, e.Item.Height );
					}

					imgArrow = OfficeRightArrow;

					p = new Point( rect.Left + ( rect.Width - OFFICE_ARROW_WIDTH ) / 2, ( rect.Height - OFFICE_ARROW_HEIGHT ) / 2 + 1 );
				}

				g.DrawImage( imgArrow, p );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintSplitButtonBackground( ToolStripItemRenderEventArgs e )
			{
				bool bResult = false;
				OfficeSplitButton tsButton = e.Item as OfficeSplitButton;

				if( tsButton != null )
				{
					Rectangle rc = GetButtonRect( e.Item );
					if( rc.Width > 0 && rc.Height > 0 )
					{
						Rectangle rcBackground = GetButtonBackgroundRect( e.Item, rc );
						if( rcBackground.Width > 0 && rcBackground.Height > 0 )
						{
							Graphics g = e.Graphics;

							EBUTTONSTATE ebState = EBUTTONSTATE.Normal;
							bool bDropDownPressed = tsButton.DropDown.Visible && ( tsButton.DropDown.OwnerItem == tsButton );
							bool bSelected = tsButton.Selected || bDropDownPressed;
							bool bSplitButtonSelected = tsButton.SplitButtonSelected;

							Rectangle rcDropDown = Rectangle.Intersect( tsButton.DropDownButtonBounds, rcBackground );
							Rectangle rcButton = rcBackground;

							rcButton.Width -= rcDropDown.Width;

							if( rcButton.Width > 0 && rcButton.Height > 0 )
							{
								if( bSelected && !bSplitButtonSelected )
								{
									PaintGradientSelected( g, rcButton, OfficeColorTable.SelectedButtonInActiveBegin, OfficeColorTable.SelectedButtonInActiveEnd );

									g.DrawImage( this.SelectedFlashImage, rcButton.X, rcButton.Y + rcButton.Height / 2, rcButton.Width, rcButton.Height );

									g.DrawLine( this.ButtonSelectedBorder, rcButton.Right - 1, rcButton.Y, rcButton.Right - 1, rcButton.Bottom - 1 );
									ebState = EBUTTONSTATE.Selected;
								}
								else if( bSelected )
								{
									PaintGradientSelected( g, rcButton, ColorTable.ButtonSelectedGradientBegin, ColorTable.ButtonSelectedGradientEnd );

									g.DrawImage( this.SelectedFlashImage, rcButton.X, rcButton.Y + rcButton.Height / 2, rcButton.Width, rcButton.Height );

									g.DrawLine( this.ButtonSelectedBorder, rcButton.Right - 1, rcButton.Y, rcButton.Right - 1, rcButton.Bottom - 1 );
									ebState = EBUTTONSTATE.Selected;
								}
							}

							if( rcDropDown.Width > 0 && rcDropDown.Height > 0 )
							{
								GraphicsState gState = g.Save();
								g.SetClip( rcDropDown );

								if( bSelected )
								{
									if( !bDropDownPressed )
									{
										PaintGradientSelected( g, rcDropDown, OfficeColorTable.SelectedButtonInActiveBegin, OfficeColorTable.SelectedButtonInActiveEnd );
										g.DrawImage( this.SelectedFlashImage, rcDropDown.X, rcDropDown.Y + rcDropDown.Height / 2, rcDropDown.Width, rcDropDown.Height );

										g.DrawLine( this.ButtonHighlightBorder, rcDropDown.X, rcDropDown.Y, rcDropDown.X, rcDropDown.Bottom - 1 );
									}
									else
									{
										PaintGradientSelected( g, rcDropDown, ColorTable.ButtonSelectedGradientBegin, ColorTable.ButtonSelectedGradientEnd );
										g.DrawImage( this.SelectedFlashImage, rcDropDown.X, rcDropDown.Y + rcDropDown.Height / 2, rcDropDown.Width, rcDropDown.Height );

										g.DrawLine( this.ButtonHighlightBorder, rcDropDown.X, rcDropDown.Y, rcDropDown.X, rcDropDown.Bottom - 1 );
									}
								}

								g.Restore( gState );
							}

							PaintArrow( new ToolStripArrowRenderEventArgs( g, e.Item, rcDropDown, OfficeColorTable.CaptionText, ArrowDirection.Right ) );

							PaintButtonBorder( e.Graphics, rc, ebState );
						}
					}
					bResult = true;
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintButtonBackGround( ToolStripItemRenderEventArgs e )
			{
				bool bResult = false;

				ToolStripItem tsButton = e.Item as ToolStripItem;
				MenuDropDown.IPanel panel = e.Item.Owner as MenuDropDown.IPanel;

				if( tsButton is OfficeButton
					|| ( panel != null && panel.PanelType != MenuDropDown.PanelType.System ) )
				{
					Rectangle rc = GetButtonRect( e.Item );
					if( rc.Width > 0 && rc.Height > 0 )
					{
						Rectangle rcBackground = GetButtonBackgroundRect( e.Item, rc );
						if( rcBackground.Width > 0 && rcBackground.Height > 0 )
						{
							Graphics g = e.Graphics;

							EBUTTONSTATE ebState = EBUTTONSTATE.Normal;
							bool bSelected = tsButton.Selected;

                            if (bSelected && ((e.ToolStrip != null && ((( e.ToolStrip.TopLevelControl != null) && (e.ToolStrip.TopLevelControl is MenuDropDown || e.ToolStrip.TopLevelControl.ContainsFocus)) || (e.ToolStrip.TopLevelControl == null))) 
                                || (tsButton!=null && (tsButton is OfficeButton && tsButton.Selected))))
							{
								PaintGradientSelected( g, rcBackground, ColorTable.ButtonSelectedGradientBegin, ColorTable.ButtonSelectedGradientEnd );

								g.DrawImage( this.SelectedFlashImage, rcBackground.X, rcBackground.Y + rcBackground.Height / 2, rcBackground.Width, rcBackground.Height );

								g.DrawLine( this.ButtonSelectedBorder, rcBackground.Right - 1, rcBackground.Y, rcBackground.Right - 1, rcBackground.Bottom - 1 );
								ebState = EBUTTONSTATE.Selected;
							}

							PaintButtonBorder( e.Graphics, rc, ebState );
						}
					}

					bResult = true;
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintButtonOnSystemPanelBackGround( ToolStripItemRenderEventArgs e )
			{
				bool bResult = false;

				MenuDropDown.IPanel panel = e.Item.Owner as MenuDropDown.IPanel;

				if( panel != null && panel.PanelType == MenuDropDown.PanelType.System )
				{
					ToolStripButton tsButton = e.Item as ToolStripButton;

					if( tsButton != null )
					{
						Rectangle rc = GetButtonRect( e.Item );
						if( rc.Width > 0 && rc.Height > 0 )
						{
							Rectangle rcBackground = GetButtonBackgroundRect( e.Item, rc );
							if( rcBackground.Width > 0 && rcBackground.Height > 0 )
							{
								Graphics g = e.Graphics;

								EBUTTONSTATE ebState = EBUTTONSTATE.Normal;
								bool bSelected = tsButton.Selected;

								if( !bSelected )
								{
									PaintGradientSelected( g, rcBackground, OfficeColorTable.SystemButtonGradientBegin, OfficeColorTable.SystemButtonGradientEnd );

									g.DrawImage( this.SystemButtonSelectedFlashImage, rcBackground.X, rcBackground.Y + rcBackground.Height / 2, rcBackground.Width, rcBackground.Height );

									PaintSystemButtonBorder( g, ref rc, OfficeColorTable.SystemButtonBorder );
								}
								else
								{
									PaintGradientSelected( g, rcBackground, ColorTable.ButtonSelectedGradientBegin, ColorTable.ButtonSelectedGradientEnd );

									g.DrawImage( this.SelectedFlashImage, rcBackground.X, rcBackground.Y + rcBackground.Height / 2, rcBackground.Width, rcBackground.Height );

									g.DrawLine( this.ButtonSelectedBorder, rcBackground.Right - 1, rcBackground.Y, rcBackground.Right - 1, rcBackground.Bottom - 1 );
									ebState = EBUTTONSTATE.Selected;

									PaintButtonBorder( e.Graphics, rc, ebState );
								}
							}
						}

						bResult = true;
					}
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintTabItemChecked(ToolStripItemRenderEventArgs e)
			{
				bool bResult = false;

				ToolStripTabItem item = e.Item as ToolStripTabItem;

				if (item != null && IsChecked(item))
				{
					Graphics g = e.Graphics;
					SmoothingMode saveMode = g.SmoothingMode;
					g.SmoothingMode = SmoothingMode.AntiAlias;

					Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);

					using (Region rgn = GetTabRegionChecked(ref rc))
					{
						ToolStripTabGroup group = null;

						RibbonControlAdvHeader header = item.Owner as RibbonControlAdvHeader;

						if( header != null )
						{
							group = header.GetItemGroup( item );
						}

						Color clEnd =
							( group == null )
							? ( this.ColorTable.ToolStripGradientEnd )
							: ( ( item.Selected ) ? ( this.ColorTable.ButtonSelectedGradientEnd ) : ( group.Color ) );

						Color clBegin =
							( group == null )
							? ( this.ColorTable.ToolStripGradientBegin )
							: ( Office12ColorTable.GetAlphaBlendedColor( Color.White, clEnd, 160 ) );

                        if (header != null && header.Parent is RibbonControlAdv && (header.Parent as RibbonControlAdv).OfficeColorScheme == ToolStripEx.ColorScheme.Black)
                        {
                            clBegin =
                                (group == null)
                                ? (Office12ColorTable.GetAlphaBlendedColor(Color.FromArgb(247, 247, 247), Color.FromArgb(218, 217, 223), 160))
                                : (Office12ColorTable.GetAlphaBlendedColor(Color.White, clEnd, 160));
                        }

						using (LinearGradientBrush brush = GetVerticalBrush(ref rc, clBegin, clEnd))
						{
							brush.Blend = ( group == null ) ? ( m_blTabChecked ) : ( m_blGroupTab );
							g.FillRegion(brush, rgn);
						}
					}
                    if (e.ToolStrip.TopLevelControl is Form)
                    {
                        if ((e.ToolStrip.TopLevelControl as Form).IsMdiContainer || e.ToolStrip.TopLevelControl.ContainsFocus)
                            PaintTabBorderChecked(g, ref rc, item.Selected);
                    }

					g.SmoothingMode = saveMode;

					bResult = true;
				}
				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintTabItemSelected(ToolStripItemRenderEventArgs e)
			{
				bool bResult = false;

				ToolStripTabItem item = e.Item as ToolStripTabItem;
				if (item != null && item.Selected && ((e.ToolStrip.TopLevelControl != null && e.ToolStrip.TopLevelControl.ContainsFocus)||(e.ToolStrip.TopLevelControl == null)))
				{
					Graphics g = e.Graphics;
					SmoothingMode saveMode = g.SmoothingMode;
					g.SmoothingMode = SmoothingMode.AntiAlias;

					Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);

					using (Region rgn = GetTabRegionSelected(ref rc))
					{
						ToolStripTabGroup group = null;

						RibbonControlAdvHeader header = item.Owner as RibbonControlAdvHeader;

						if( header != null )
						{
							group = header.GetItemGroup( item );
						}

						Color clBegin = Color.FromArgb(160, Color.White);
						Color clEnd = Color.FromArgb(128, ( group == null ) ? ( this.ColorTable.ButtonSelectedGradientEnd ) : ( group.Color ) );

						using (LinearGradientBrush brush = new LinearGradientBrush(rc, clBegin, clEnd, 90))
						{
							brush.Blend = m_blTabSelected;
							g.FillRegion(brush, rgn);
						}

						Rectangle rcFlash = new Rectangle(rc.X, rc.Y - rc.Height / 2, rc.Width, rc.Height);
						rcFlash.Inflate(rc.Width / 20, rc.Height / 2);

						Bitmap bmpFlash = GetTabFlash(rcFlash.Size);
						g.DrawImageUnscaled(bmpFlash, rcFlash.Location);
					}

					PaintTabBorderSelected(g, ref rc);

					g.SmoothingMode = saveMode;
					bResult = true;
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintSystemButtonPressed(ToolStripItemRenderEventArgs e)
			{
				bool bResult = false;

				SystemButton item = e.Item as SystemButton;

				if (item != null && item.Pressed)
				{
					Graphics g = e.Graphics;

					SmoothingMode saveMode = g.SmoothingMode;
					g.SmoothingMode = SmoothingMode.AntiAlias;

					Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);

					PaintSystemButtonGradient( g, ref rc, SystemButtonPressedGradientBegin, SystemButtonPressedGradientEnd, m_blSystemButton );

					g.DrawImage( this.SystemButtonPressedFlashImage, rc.X, rc.Y + rc.Height * 3 / 5, rc.Width, rc.Height );

					PaintSystemButtonBorder(g, ref rc, SystemBorderPressed);

					g.SmoothingMode = saveMode;

					bResult = true;
				}
				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			/// <returns></returns>
			private bool PaintSystemButtonSelected(ToolStripItemRenderEventArgs e)
			{
				bool bResult = false;

				SystemButton item = e.Item as SystemButton;

				if (item != null && item.Selected)
				{
					Graphics g = e.Graphics;

					SmoothingMode saveMode = g.SmoothingMode;
					g.SmoothingMode = SmoothingMode.AntiAlias;

					Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);

					PaintSystemButtonGradient( g, ref rc, SystemButtonSelectedGradientBegin, SystemButtonSelectedGradientEnd, m_blSystemButton );
					g.DrawImage(this.SystemButtonSelectedFlashImage, rc.X, rc.Y + rc.Height * 3 / 5, rc.Width, rc.Height);

					PaintSystemButtonBorder(g, ref rc, SystemBorderSelected);

					g.SmoothingMode = saveMode;
					bResult = true;
				}
				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			/// <param name="clBegin"></param>
			/// <param name="clEnd"></param>
			/// <param name="bl"></param>
			private void PaintSystemButtonGradient(Graphics g, ref Rectangle rc, Color clBegin, Color clEnd, Blend bl )
			{
				if (rc.Width > 0 && rc.Height > 0)
				{
					using (Region region = RendererUtils.GetRoundedRegion(rc, 1))
					{
						using (LinearGradientBrush brush = GetVerticalBrush(ref rc, clBegin, clEnd))
						{
							brush.Blend = bl;
							brush.WrapMode = WrapMode.TileFlipY;

							g.FillRegion(brush, region);
						}
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			/// <returns></returns>
			private void PaintSystemButtonBorder(Graphics g, ref Rectangle rc, Color color)
			{
				using (LinearGradientBrush brush = GetVerticalBrush(ref rc, Color.White, Color.Transparent))
				{
					brush.Blend = m_blSystemButtonBorder;

					using(Pen pen = new Pen(brush))
					{
						g.DrawRectangle(pen, rc.X+1, rc.Y+1, rc.Width-3, rc.Height-3);
					}
				}

				using (Pen pen = new Pen(color))
				{
					g.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(rc, 1));
				}

			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			private void PaintTabBorderSelected(Graphics g, ref Rectangle rc)
			{
				Color clInner = OfficeColorTable.RibbonPanelBorderBegin;
				Color clOuter = OfficeColorTable.RibbonPanelBorderEnd;

				using (Pen pen = new Pen(clInner))
				{
					Point[] points = new Point[]
					{
						new Point(rc.X+2, rc.Bottom-1),
						new Point(rc.X+2, rc.Y+2),
						new Point(rc.X+3, rc.Y+1),
						new Point(rc.Right-4, rc.Y+1),
						new Point(rc.Right-3, rc.Y+2),
						new Point(rc.Right-3, rc.Bottom-1),
					};
					g.DrawLines(pen, points);
				}

				using (Pen pen = new Pen(clOuter))
				{
					Point[] points = new Point[]
					{
						new Point(rc.X+1, rc.Bottom-1),
						new Point(rc.X+1, rc.Y+2),
						new Point(rc.X+3, rc.Y),
						new Point(rc.Right-4, rc.Y),
						new Point(rc.Right-2, rc.Y+2),
						new Point(rc.Right-2, rc.Bottom-1),
					};

					g.DrawLines(pen, points);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			/// <param name="bHighlight"></param>
			private void PaintTabBorderChecked(Graphics g, ref Rectangle rc, bool bHighlight)
			{
				Color clInner = bHighlight ? this.ColorTable.ButtonSelectedGradientBegin : OfficeColorTable.RibbonPanelBorderBegin;
				Color clOuter = bHighlight ? this.ColorTable.ButtonSelectedGradientEnd : OfficeColorTable.RibbonPanelBorderEnd;

				using (Pen pen = new Pen(clInner))
				{
					Point[] points = new Point[]
					{
						new Point(rc.X+1, rc.Bottom-1),
						new Point(rc.X+2, rc.Bottom-2),
						new Point(rc.X+2, rc.Y+2),
						new Point(rc.X+3, rc.Y+1),
						new Point(rc.Right-4, rc.Y+1),
						new Point(rc.Right-3, rc.Y+2),
						new Point(rc.Right-3, rc.Bottom-2),
						new Point(rc.Right-2, rc.Bottom-1),
					};
					g.DrawLines(pen, points);
				}

				using (Pen pen = new Pen(clOuter))
				{
					Point[] points = new Point[]
					{
						new Point(rc.X, rc.Bottom-1),
						new Point(rc.X+1, rc.Bottom-2),
						new Point(rc.X+1, rc.Y+2),
						new Point(rc.X+3, rc.Y),
						new Point(rc.Right-4, rc.Y),
						new Point(rc.Right-2, rc.Y+2),
						new Point(rc.Right-2, rc.Bottom-2),
						new Point(rc.Right-1, rc.Bottom-1),
					};

					g.DrawLines(pen, points);
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			/// <param name="button"></param>
			private void PaintMenuButtonImage(Graphics g, ref Rectangle rc, ToolStripMenuButton button)
			{
				Image image = button.Image;
				if (image != null)
				{
					int margin = (int)(rc.Width * IMAGE_MARGIN_FACTOR) + 2;

					RibbonControlAdv ribbon = button.Owner.Parent as RibbonControlAdv;
					Rectangle rcImage = Rectangle.Empty;

					if (ribbon.ScaleMenuButtonImage)
					{
						rcImage = new Rectangle(rc.X + margin, rc.Y + margin, rc.Width - margin * 2 + 1, rc.Height - margin * 2 + 1);
					}
					else
					{
						rcImage = new Rectangle(rc.X + rc.Width / 2 - button.Image.Width / 2, rc.Y + rc.Height / 2 - button.Image.Height / 2, button.Image.Width, button.Image.Height);
					}

					if (rcImage.Width > 0 && rcImage.Height > 0)
					{
						g.DrawImage(button.Image, rcImage);
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			/// <param name="button"></param>
			private void PaintMenuButtonBackground(Graphics g, ref Rectangle rc, ToolStripMenuButton button)
			{
				GraphicsState gState = g.Save();
				g.SmoothingMode = SmoothingMode.AntiAlias;

				if (!PaintMenuButtonBackgroundPressed(g, ref rc, button))
				{
					if (!PaintMenuButtonBackgroundSelected(g, ref rc, button))
					{
						PaintMenuButtonBackgroundNormal(g, ref rc, button);
					}
				}

				g.Restore(gState);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			/// <param name="button"></param>
			/// <returns></returns>
			private bool PaintMenuButtonBackgroundPressed(Graphics g, ref Rectangle rc, ToolStripMenuButton button)
			{
				bool bResult = false;

				if (button.Pressed)
				{
					g.DrawImage(GetMenuButtonPressed(rc.Size), rc);

					PaintMenuButtonBorder(g, ref rc, true);

					bResult = true;
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			/// <param name="button"></param>
			/// <returns></returns>
			private bool PaintMenuButtonBackgroundSelected(Graphics g, ref Rectangle rc, ToolStripMenuButton button)
			{
				bool bResult = false;
                if (button.Selected && ((button.GetCurrentParent().TopLevelControl != null && button.GetCurrentParent().TopLevelControl.ContainsFocus) || (button.GetCurrentParent().TopLevelControl == null)))
				{
					g.DrawImage(GetMenuButtonSelected(rc.Size), rc);

					PaintMenuButtonBorder(g, ref rc, false);

					bResult = true;
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			/// <param name="button"></param>
			private void PaintMenuButtonBackgroundNormal(Graphics g, ref Rectangle rc, ToolStripMenuButton button)
			{
				Image img = GetMenuButtonImage(rc.Size, this.MenuButtonNormal, this.MenuButtonNormalHighlight);

				g.DrawImageUnscaled (img, rc.Location);

				PaintMenuButtonBorder(g, ref rc, false);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			/// <param name="bPressed"></param>
			private void PaintMenuButtonBorder(Graphics g, ref Rectangle rc, bool bPressed)
			{
				GraphicsState gState = g.Save();
				g.SmoothingMode = SmoothingMode.AntiAlias;

				Color clBorder = Office12ColorTable.GetAlphaBlendedColor(Color.Black, ColorTable.ToolStripGradientBegin, 32);
				Color clShadow = bPressed ? Color.FromArgb(32, Color.Black):Color.FromArgb(160, Color.White);

				using (Pen pen = new Pen(clBorder, 2f))
				{
					g.DrawEllipse(pen, rc);
				}

				using (Pen pen = new Pen(clShadow, 1f))
				{
					//g.DrawEllipse(pen, Rectangle.Inflate(rc, -2, -2));
				}
				g.Restore(gState);
			}
			/// <summary> Draw right or left scroll button.  </summary>
			/// <param name="header"> Header on which scroll button is located. </param>
			/// <param name="g"> Graphics used in painting. </param>
			/// <param name="rc"> Rectangle in which arrow paints. </param>
			/// <param name="bRight"> Indicates if right or left arrow must be painted. </param>
			public void DrawTabScrollButton( RibbonControlAdvHeader header, Graphics g, Rectangle rc, bool bRight )
			{
				if (rc.Width > 0 && rc.Height > 0)
				{
					Rectangle rect = new Rectangle(rc.Left, rc.Top + 2, rc.Width - 2, rc.Height - 4);

					bool bSelected = bRight ? header.RightScrollSelected : header.LeftScrollSelected;

					using (Brush brush = GetBackgroundBrush(rc.Top, rc.Height, bSelected))
					{
						g.FillRectangle(brush, rect);
					}

					g.SmoothingMode = SmoothingMode.AntiAlias;

					g.DrawPolygon(Pens.White, bRight ? RendererUtils.GetSquareRoundedToRightPolygon(Rectangle.Inflate(rc, -1, -1), 1) :
						RendererUtils.GetSquareRoundedToLeftPolygon(Rectangle.Inflate(rc, -1, -1), 1));

					using (Pen pen = GetBorderPen())
					{
						g.DrawPolygon(pen, bRight ? RendererUtils.GetSquareRoundedToRightPolygon(rc, 2) :
							RendererUtils.GetSquareRoundedToLeftPolygon(rc, 2));
					}

					Image imgArrow = bRight ? RightArrow : LeftArrow;

					int left = rc.Left + (rc.Width - ARROW_WIDTH) / 2;
					int top = rc.Top + (rc.Height - ARROW_HEIGHT) / 2;

					g.DrawImage(imgArrow, left, top);
				}
			}
			/// <summary> Draw separators between Tab items.  </summary>
			/// <param name="header"> Header on which scroll button is located. </param>
			/// <param name="g"> Graphics used in painting. </param>
			/// <param name="rc"> Rectangle to draw separators in. </param>
			public void DrawSeparators( RibbonControlAdvHeader header, Graphics g, Rectangle rc )
			{
				int iTitleHeight = header.QuickPanelHeight + 2;

				Color clBegin = Color.Transparent;
				Color clEnd = OfficeColorTable.TabItemSeparator;
				Brush brush = GetVerticalBrush( iTitleHeight, header.Height, header.m_fFactor, clBegin, clEnd );
				Pen pen = new Pen( brush );

				Point ptBegin = new Point( 0, iTitleHeight );
				Point ptEnd = new Point( 0, header.Height );

				for( int i = 0, count = header.m_iSepatators.Count; i < count; i++ )
				{
					ptEnd.X = ptBegin.X = header.m_iSepatators[ i ];

					if( header.m_iSepatators[ i ] < rc.Right - ( header.m_bIsRightScroll ? SCROLL_BUTTON_WIDTH : 0 )
						&& header.m_iSepatators[ i ] > rc.Left + ( header.m_bIsLeftScroll ? SCROLL_BUTTON_WIDTH : 0 ) )
					{
						g.DrawLine( pen, ptBegin, ptEnd );
					}
				}
				brush.Dispose();
				pen.Dispose();
			}
			/// <summary> Draw up or down scroll button.  </summary>
			/// <param name="panel"> Panel on which scroll button is located. </param>
			/// <param name="g"> Graphics used in painting. </param>
			/// <param name="rc"> Rectangle in which arrow paints. </param>
			/// <param name="bDown"> Indicates if up or down arrow must be painted. </param>
			public void DrawUpDownScrollButton( MenuDropDown.Panel panel, Graphics g, Rectangle rc, bool bDown )
			{
				bool bSelected = bDown ? panel.DownScrollSelected : panel.UpScrollSelected;

				using( LinearGradientBrush brush = GetVerticalBrush( ref rc, bSelected ? OfficeColorTable.ButtonSelectedGradientBegin : OfficeColorTable.ToolStripGradientBegin,
					bSelected ? OfficeColorTable.ButtonSelectedGradientEnd : OfficeColorTable.ToolStripGradientEnd ) )
				{
					Rectangle rect = new Rectangle( rc.Left, rc.Top + 2, rc.Width - 2, rc.Height - 4 );
					g.FillRectangle( brush, rect );
				}

				g.SmoothingMode = SmoothingMode.AntiAlias;

				g.DrawPolygon( Pens.White, bDown ? RendererUtils.GetSquareRoundedToDownPolygon( Rectangle.Inflate( rc, -1, -1 ), 1 ) :
					RendererUtils.GetSquareRoundedToUpPolygon( Rectangle.Inflate( rc, -1, -1 ), 1 ) );

				using( Pen pen = GetBorderPen() )
				{
					g.DrawPolygon( pen, bDown ? RendererUtils.GetSquareRoundedToDownPolygon( rc, 2 ) :
						RendererUtils.GetSquareRoundedToUpPolygon( rc, 2 ) );
				}

				Image imgArrow = bDown ? DownArrow : UpArrow;

				int left = rc.Left + ( rc.Width - ARROW_UP_DOWN_WIDTH ) / 2;
				int top = rc.Top + ( rc.Height - ARROW_UP_DOWN_HEIGHT ) / 2;

				g.DrawImage( imgArrow, left, top );
			}
			/// <summary> Draw up or down scroll button on DropDownEx. </summary>
			/// <param name="dropDown"></param>
			/// <param name="g"></param>
			/// <param name="rc"></param>
			/// <param name="bDown"></param>
			public void DrawScrollButtonOnDropDown( OfficeDropDown dropDown, Graphics g, Rectangle rc, bool bDown )
			{
				bool bSelected = bDown ? dropDown.DownScrollSelected : dropDown.UpScrollSelected;

				using( LinearGradientBrush brush = GetVerticalBrush( ref rc, bSelected ? OfficeColorTable.ButtonSelectedGradientBegin : OfficeColorTable.ToolStripGradientBegin,
					bSelected ? OfficeColorTable.ButtonSelectedGradientEnd : OfficeColorTable.ToolStripGradientEnd ) )
				{
					Rectangle rect = new Rectangle( rc.Left, rc.Top + 1, rc.Width - 2, rc.Height - 2 );
					g.FillRectangle( brush, rect );
				}

				g.SmoothingMode = SmoothingMode.AntiAlias;

				g.DrawPolygon( Pens.White, bDown ? RendererUtils.GetSquareRoundedToDownPolygon( Rectangle.Inflate( rc, -1, -1 ), 1 ) :
					RendererUtils.GetSquareRoundedToUpPolygon( Rectangle.Inflate( rc, -1, -1 ), 1 ) );

				using( Pen pen = GetBorderPen() )
				{
					g.DrawPolygon( pen, bDown ? RendererUtils.GetSquareRoundedToDownPolygon( rc, 2 ) :
						RendererUtils.GetSquareRoundedToUpPolygon( rc, 2 ) );
				}

				Image imgArrow = bDown ? DownArrow : UpArrow;

				int left = rc.Left + ( rc.Width - ARROW_UP_DOWN_WIDTH ) / 2;
				int top = rc.Top + ( rc.Height - ARROW_UP_DOWN_HEIGHT ) / 2;

				g.DrawImage( imgArrow, left, top );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="rc"></param>
			/// <returns></returns>
			private Region GetTabRegionChecked(ref Rectangle rc)
			{
				Region rgn = new Region(rc);

				rgn.Exclude(new Rectangle(rc.X, rc.Y, 1, rc.Height - 1));
				rgn.Exclude(new Rectangle(rc.X + 1, rc.Y, 1, 2));
				rgn.Exclude(new Rectangle(rc.X + 2, rc.Y, 1, 1));

				rgn.Exclude(new Rectangle(rc.Right - 3, rc.Y, 1, 1));
				rgn.Exclude(new Rectangle(rc.Right - 2, rc.Y, 1, 2));
				rgn.Exclude(new Rectangle(rc.Right - 1, rc.Y, 1, rc.Height - 1));

				return rgn;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="rc"></param>
			/// <returns></returns>
			private Region GetTabRegionSelected(ref Rectangle rc)
			{
				Region rgn = new Region(rc);

				rgn.Exclude(new Rectangle(rc.X, rc.Y, 1, rc.Height));
				rgn.Exclude(new Rectangle(rc.X + 1, rc.Y, 1, 2));
				rgn.Exclude(new Rectangle(rc.X + 2, rc.Y, 1, 1));

				rgn.Exclude(new Rectangle(rc.Right - 3, rc.Y, 1, 1));
				rgn.Exclude(new Rectangle(rc.Right - 2, rc.Y, 1, 2));
				rgn.Exclude(new Rectangle(rc.Right - 1, rc.Y, 1, rc.Height));

				return rgn;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="rc"></param>
			/// <returns></returns>
			private GraphicsPath GetQuickPathLeftToRight(Rectangle rc)
			{
				GraphicsPath path = new GraphicsPath();

				int height = rc.Height;
				int left = rc.X - height;

				path.AddLine(left, rc.Y, left, rc.Bottom);
				path.AddLine(left, rc.Bottom, rc.Right, rc.Bottom );
				path.AddArc(rc.Right - height/2, rc.Y, height-1, height-1, 90f, -180f);
				path.AddLine(rc.Right, rc.Y, left, rc.Y );

				return path;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="rc"></param>
			/// <returns></returns>
			private GraphicsPath GetQuickPathRightToLeft(Rectangle rc)
			{
				GraphicsPath path = new GraphicsPath();

				int height = rc.Height;
				int left = rc.Right + height;

				path.AddLine(left, rc.Y, left, rc.Bottom);
				path.AddLine(left, rc.Bottom, rc.X, rc.Bottom);
				path.AddArc(rc.X - height / 2, rc.Y, height - 1, height - 1, 90f, 180f);
				path.AddLine(rc.X, rc.Y, left, rc.Y);

				return path;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="item"></param>
			/// <returns></returns>
			private Rectangle GetMenuButtonRectangle(ToolStripItem item)
			{
				Rectangle rc = new Rectangle(Point.Empty, item.Size);

				rc.Inflate(-MENUBUTTON_PADDING, -MENUBUTTON_PADDING);

				return rc;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="szImage"></param>
			/// <returns></returns>
			private Bitmap GetMenuButtonNormal(Size szImage)
			{
				Bitmap img = m_hImages[EIMAGE.eiMenuButtonFlashNormal] as Bitmap;

				if (img == null || img.Size != szImage)
				{
					img = GetMenuButtonImage(szImage, this.MenuButtonNormal, this.MenuButtonNormalHighlight);

					m_hImages[EIMAGE.eiMenuButtonFlashNormal] = img;
				}
				return img;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="szImage"></param>
			/// <returns></returns>
			private Bitmap GetMenuButtonSelected(Size szImage)
			{
				Bitmap img = m_hImages[EIMAGE.eiMenuButtonFlashSelected] as Bitmap;

				if (img == null || img.Size != szImage)
				{
					img = GetMenuButtonImage(szImage, this.MenuButtonSelected, this.MenuButtonSelectedHighlight);

					m_hImages[EIMAGE.eiMenuButtonFlashSelected] = img;
				}
				return img;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="szImage"></param>
			/// <returns></returns>
			private Bitmap GetMenuButtonPressed(Size szImage)
			{
				Bitmap img = m_hImages[EIMAGE.eiMenuButtonFlashPressed] as Bitmap;

				if (img == null || img.Size != szImage)
				{
					img = GetMenuButtonImage(szImage, this.MenuButtonPressed, this.MenuButtonPressedHighlight);

					m_hImages[EIMAGE.eiMenuButtonFlashPressed] = img;
				}
				return img;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="szImage"></param>
			/// <returns></returns>
			private Bitmap GetMenuButtonImage(Size szImage, Color backColor, Color highlightColor)
			{
				Rectangle rc = new Rectangle(Point.Empty, szImage);
				Bitmap img = new Bitmap(rc.Width, rc.Height);

				using (Graphics g = Graphics.FromImage(img))
				{
					g.Clear(Color.Transparent);

					using (LinearGradientBrush brush = new LinearGradientBrush(Point.Empty, new Point(0,szImage.Height), Color.White, backColor))
					{
						Blend blend = new Blend();

						blend.Positions = new float[] { 0.0f, 0.45f, 0.45f, 1.0f };
						blend.Factors = new float[] { 0.0f, 0.6f, 1.0f, 0.8f };

						brush.Blend = blend;

						g.FillEllipse(brush, rc);
					}
					using (GraphicsPath path = new GraphicsPath())
					{
						path.AddEllipse(Rectangle.Inflate(rc, rc.Width / 8, 0));
						using (PathGradientBrush brush = new PathGradientBrush(path))
						{
							brush.CenterColor = Color.Transparent;
							brush.SurroundColors = new Color[] { Color.White };

							Blend blend = new Blend();

							blend.Positions = new float[] { 0.0f, 0.2f, 1.0f };
							blend.Factors = new float[] { 0.0f, 0.8F, 1.0f };

							brush.Blend = blend;
							g.FillEllipse(brush, rc);
						}
					}
					using (GraphicsPath path = new GraphicsPath())
					{
						path.AddEllipse(rc);

						using (PathGradientBrush brush = new PathGradientBrush(path))
						{
							brush.CenterPoint = new Point(rc.Width / 2, rc.Bottom);

							brush.CenterColor = highlightColor;
							brush.SurroundColors = new Color[] { Color.Transparent};

							Blend blend = new Blend();

							blend.Positions = new float[] { 0.0f, 0.2f, 0.8F, 1.0f };
							blend.Factors = new float[] { 0.0f, 0.1f, 0.9F, 1.0f };

							brush.Blend = blend;

							g.FillEllipse(brush, rc);
						}
					}
				}

				return img;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="size"></param>
			/// <returns></returns>
			private Bitmap GetTabFlash(Size size)
			{
				Rectangle rcFlash = new Rectangle(Point.Empty, size);
				Bitmap img = new Bitmap(rcFlash.Width, rcFlash.Height);

				using (Graphics g = Graphics.FromImage(img))
				{
					using (GraphicsPath path = new GraphicsPath())
					{
						path.AddEllipse(rcFlash);

						using (PathGradientBrush brFlash = new PathGradientBrush(path))
						{
							brFlash.Blend = m_blTabFlash;

							brFlash.CenterColor = this.ColorTable.ToolStripGradientBegin;
							brFlash.SurroundColors = new Color[] { Color.Transparent };

							g.FillRectangle(brFlash, rcFlash);
						}
					}
				}
				return img;
			}
			/// <summary>
			/// 
			/// </summary>
			void Clear()
			{
				Clear(m_hImages);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="items"></param>
			void Clear(Hashtable items)
			{
				foreach (IDisposable iDispose in items.Values)
				{
					if (iDispose != null)
					{
						iDispose.Dispose();
					}
				}
				items.Clear();
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="item"></param>
			/// <returns></returns>
			bool IsChecked(ToolStripTabItem item)
			{
				bool bResult = false;
				if (item != null && item.Checked)
				{
					bResult = !IsPanelHidden(item);
				}
				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="item"></param>
			/// <returns></returns>
			bool IsPanelHidden(ToolStripItem item)
			{
				bool bResult = false;

				RibbonControlAdvHeader header = item.Owner as RibbonControlAdvHeader;
				if (header != null)
				{
					RibbonControlAdv ribbon = header.Parent as RibbonControlAdv;
					if (ribbon != null )
					{
						bResult = !ribbon.VisiblePanel;
					}
				}

				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			private LinearGradientBrush GetBackgroundBrush(int top, int nHeight, bool bSelected)
			{
				Color clBegin = bSelected ? OfficeColorTable.ButtonSelectedGradientBegin : OfficeColorTable.ToolStripGradientBegin;
				Color clEnd = bSelected ? OfficeColorTable.ButtonSelectedGradientEnd : OfficeColorTable.ToolStripGradientEnd;

				LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, top, 1, nHeight), clBegin, clEnd, LinearGradientMode.Vertical);

				return brush;
			}
			/// <summary>
			/// 
			/// </summary>
			private Pen GetBorderPen()
			{
				return new Pen( OfficeColorTable.ToolStripBorder );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			private Brush GetBackgroundBrushForArrow( int width )
			{
				Color clBegin = OfficeColorTable.OfficeArrowGradientBegin;
				Color clEnd = OfficeColorTable.OfficeArrowGradientEnd;

				LinearGradientBrush brush = new LinearGradientBrush( new Rectangle( 0, 0, width, 1 ), clBegin, clEnd, LinearGradientMode.Horizontal );

				return brush;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="top"></param>
			/// <param name="height"></param>
			/// <param name="factor"></param>
			/// <param name="cl1"></param>
			/// <param name="cl2"></param>
			/// <returns></returns>
			private LinearGradientBrush GetVerticalBrush( int top, int height, float factor, Color cl1, Color cl2 )
			{
				Rectangle rcBrush = new Rectangle( 0, top, 1, height );
				LinearGradientBrush brush = new LinearGradientBrush( rcBrush, cl1, cl2, 90 );

				if( factor > 0.0f && factor <= 1.0f )
				{
					Blend blend = new Blend();
					blend.Positions = new float[] { 0.0f, 0.3f, 1.0f };
					blend.Factors = new float[] { 0.0f, factor, factor };

					brush.Blend = blend;
				}

				return brush;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="hWnd"></param>
			/// <param name="rcText"></param>
			/// <param name="sText"></param>
			/// <param name="font"></param>
			static internal void DrawThemeText(Graphics g, IntPtr hWnd, Rectangle rcText, string sText, Font font)
			{
				DrawThemeText(g, hWnd, rcText, sText, font, Color.Empty, true);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="hWnd"></param>
			/// <param name="rcText"></param>
			/// <param name="sText"></param>
			/// <param name="font"></param>
			/// <param name="textColor"></param>
			static internal void DrawThemeText(Graphics g, IntPtr hWnd, Rectangle rcText, string sText, Font font, Color textColor, bool useMnemonic)
			{
				IntPtr hTheme = DwmAPI.OpenThemeData(hWnd, "CompositedWindow::Window");
				if (hTheme != IntPtr.Zero)
				{
					IntPtr hdc = g.GetHdc();
					if (hdc != IntPtr.Zero)
					{
						IntPtr memdc = WindowsAPI.CreateCompatibleDC(hdc);
						if (memdc != IntPtr.Zero)
						{
							int cx = rcText.Width;
							int cy = rcText.Height;

							BITMAPINFO_FLAT dib = new BITMAPINFO_FLAT();
							dib.bmiHeader_biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
							dib.bmiHeader_biHeight = -cy;
							dib.bmiHeader_biWidth = cx;
							dib.bmiHeader_biPlanes = 1;
							dib.bmiHeader_biBitCount = 32;
							dib.bmiHeader_biCompression = 0;

							IntPtr ppv = IntPtr.Zero;
							IntPtr bmp = WindowsAPI.CreateDIBSection(memdc, ref dib, 0, ref ppv, IntPtr.Zero, 0);

							if (bmp != IntPtr.Zero)
							{
								IntPtr hFont = font.ToHfont();

								IntPtr hFontOld = WindowsAPI.SelectObject(memdc, hFont);
								IntPtr bmpOld = WindowsAPI.SelectObject(memdc, bmp);

								DwmAPI.DTTOPTS DttOpts = new DwmAPI.DTTOPTS();
								if (textColor != Color.Empty)
								{
									uint val = (uint)textColor.R + (((uint)textColor.G) << 8) + (((uint)textColor.B) << 16);
									DttOpts.crText = (int)val;
								}
								DttOpts.dwSize = Marshal.SizeOf(DttOpts);
								DttOpts.dwFlags = DwmAPI.DttFlags.DTT_COMPOSITED | DwmAPI.DttFlags.DTT_GLOWSIZE | DwmAPI.DttFlags.DTT_CRTEXT;
								DttOpts.iGlowSize = 0;

								WindowsAPI.BitBlt(memdc, 0, 0, cx, cy, hdc, rcText.X, rcText.Y, (uint)PatBltTypes.SRCCOPY);

								RECT rc = new RECT(0, 0, cx, cy);
								DrawTextFormatFlags format_Flag ;
								if(textColor == Color.White )
									format_Flag = DrawTextFormatFlags.DT_RIGHT | DrawTextFormatFlags.DT_VCENTER | DrawTextFormatFlags.DT_WORD_ELLIPSIS;
								else
									format_Flag = DrawTextFormatFlags.DT_CENTER | DrawTextFormatFlags.DT_VCENTER | DrawTextFormatFlags.DT_WORD_ELLIPSIS;

								if (!useMnemonic)
								{
									format_Flag |= DrawTextFormatFlags.DT_NOPREFIX;
								}
								DwmAPI.DrawThemeTextEx(hTheme, memdc, 0, 0, sText, -1, format_Flag, ref rc, ref DttOpts);

								WindowsAPI.BitBlt(hdc, rcText.X, rcText.Y, cx, cy, memdc, 0, 0, (uint)PatBltTypes.SRCCOPY);

								WindowsAPI.SelectObject(memdc, hFontOld);
								WindowsAPI.SelectObject(memdc, bmpOld);

								WindowsAPI.DeleteObject(hFont);
								WindowsAPI.DeleteObject(bmp);
							}
							WindowsAPI.DeleteDC(memdc);
						}
						g.ReleaseHdc(hdc);
					}
					DwmAPI.CloseThemeData(hTheme);
				}
			}

			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			protected Color ActiveTitleGradientBegin
			{
				get
				{
					return this.OfficeColorTable.ActiveTitleGradientBegin;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			protected Color ActiveTitleGradientEnd
			{
				get
				{
					return this.OfficeColorTable.ActiveTitleGradientEnd;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			protected Color InActiveTitleGradientBegin
			{
				get
				{
					return this.OfficeColorTable.InActiveTitleGradientBegin;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			protected Color InActiveTitleGradientEnd
			{
				get
				{
					return this.OfficeColorTable.InActiveTitleGradientEnd;
				}
			}
			protected Color SystemButtonPressedGradientBegin
			{
				get
				{
					return this.OfficeColorTable.SystemButtonPressedGradientBegin;
				}
			}
			protected Color SystemButtonPressedGradientEnd
			{
				get
				{
					return this.OfficeColorTable.SystemButtonPressedGradientEnd;
				}
			}
			protected Color SystemButtonPressedHighlight
			{
				get
				{
					return this.OfficeColorTable.SystemButtonPressedHighlight;
				}
			}
			protected Color SystemButtonSelectedGradientBegin
			{
				get
				{
					return this.OfficeColorTable.SystemButtonSelectedGradientBegin;
				}
			}
			protected Color SystemButtonSelectedGradientEnd
			{
				get
				{
					return this.OfficeColorTable.SystemButtonSelectedGradientEnd;
				}
			}
			protected Color SystemButtonSelectedHighlight
			{
				get
				{
					return this.OfficeColorTable.SystemButtonSelectedHighlight;
				}
			}
			protected Color QuickPanelGradientBegin
			{
				get
				{
					return this.OfficeColorTable.QuickPanelGradientBegin;
				}
			}
			protected Color QuickPanelGradientEnd
			{
				get
				{
					return this.OfficeColorTable.QuickPanelGradientEnd;
				}
			}

			protected Color InactiveQuickPanelGradientBegin
			{
				get { return this.OfficeColorTable.InactiveQuickPanelGradientBegin; }
			}
			protected Color InactiveQuickPanelGradientEnd
			{
				get { return this.OfficeColorTable.InactiveQuickPanelGradientEnd; }
			}

			protected Color SystemBorderSelected
			{
				get
				{
					return this.OfficeColorTable.SystemButtonBorderSelected;
				}
			}
			protected Color SystemBorderPressed
			{
				get
				{
					return this.OfficeColorTable.SystemButtonBorderPressed;
				}
			}
			protected Color RibbonBorder
			{
				get
				{
					return this.OfficeColorTable.RibbonBorder;
				}
			}
			protected Color RibbonTabText
			{
				get
				{
					return this.OfficeColorTable.RibbonTabText;
				}
			}
			protected Color RibbonTabInactiveText
			{
				get
				{
					return this.OfficeColorTable.RibbonTabInactiveText;
				}
			}
			protected Color RibbonTitleText
			{
				get
				{
					return this.OfficeColorTable.RibbonTitleText;
				}
			}

			protected Color MenuButtonNormal
			{
				get
				{
					return this.OfficeColorTable.MenuButtonNormal;
				}
			}
			protected Color MenuButtonNormalHighlight
			{
				get
				{
					return this.OfficeColorTable.MenuButtonNormalHighlight;
				}
			}
			protected Color MenuButtonSelected
			{
				get
				{
					return this.OfficeColorTable.MenuButtonSelected;
				}
			}
			protected Color MenuButtonSelectedHighlight
			{
				get
				{
					return this.OfficeColorTable.MenuButtonSelectedHighlight;
				}
			}
			protected Color MenuButtonPressed
			{
				get
				{
					return this.OfficeColorTable.MenuButtonPressed;
				}
			}
			protected Color MenuButtonPressedHighlight
			{
				get
				{
					return this.OfficeColorTable.MenuButtonPressedHighlight;
				}
			}

			protected Bitmap SystemButtonSelectedFlashImage
			{
				get
				{
					Bitmap bitmap = m_htBitmaps[ EBITMAP.ebSystemButtonSelectedFlash ] as Bitmap;
					if (bitmap == null)
					{
						bitmap = GetFlashImage( this.SystemButtonSelectedHighlight, new Size( 20, 20 ) );
						m_htBitmaps[EBITMAP.ebPressedFlash] = bitmap;
					}
					return bitmap;
				}
			}
			protected Bitmap SystemButtonPressedFlashImage
			{
				get
				{
					Bitmap bitmap = m_htBitmaps[ EBITMAP.ebSystemButtonPressedFlash ] as Bitmap;
					if (bitmap == null)
					{
						bitmap = GetFlashImage( this.SystemButtonPressedHighlight, new Size( 20, 20 ) );
						m_htBitmaps[EBITMAP.ebPressedFlash] = bitmap;
					}
					return bitmap;
				}
			}

			protected Pen ShadowPen
			{
				get
				{
					if( m_pShadow == null )
					{
						m_pShadow = new Pen( Color.FromArgb( 30, Color.Black ) );
					}
					return m_pShadow;
				}
			}
			protected Pen HighlightPen
			{
				get
				{
					if( m_pHighlight == null )
					{
						m_pHighlight = new Pen( Color.FromArgb( 70, Color.White ) );
					}
					return m_pHighlight;
				}
			}
			/// <summary> Right office arrow. </summary>
			protected Bitmap OfficeRightArrow
			{
				get
				{
					Bitmap bitmap = m_hImages[ EIMAGE.eiRightArrow ] as Bitmap;

					if( bitmap == null )
					{
						bitmap = new Bitmap( OFFICE_ARROW_WIDTH, OFFICE_ARROW_HEIGHT );

						using( Graphics g = Graphics.FromImage( bitmap ) )
						{
							g.Clear( Color.Transparent );

							Rectangle rcRightArrow = new Rectangle( 0, 0, 1, OFFICE_ARROW_HEIGHT );

							using( Region rg = new Region( rcRightArrow ) )
							{
								rg.Union( rcRightArrow );

								rcRightArrow.Inflate( 0, -1 );
								rcRightArrow.X += 1;
								rg.Union( rcRightArrow );

								rcRightArrow.Inflate( 0, -1 );
								rcRightArrow.X += 1;
								rg.Union( rcRightArrow );

								rcRightArrow.Inflate( 0, -1 );
								rcRightArrow.X += 1;
								rg.Union( rcRightArrow );

								using( Brush brush = GetBackgroundBrushForArrow( OFFICE_ARROW_WIDTH ) )
								{
									g.FillRegion( brush, rg );
								}
							}
						}

						m_hImages[ EIMAGE.eiRightArrow ] = bitmap;
					}

					return bitmap;
				}
			}
			/// <summary> Left office arrow. </summary>
			protected Bitmap OfficeLeftArrow
			{
				get
				{

					Bitmap bitmap = m_hImages[ EIMAGE.eiLeftArrow ] as Bitmap;

					if( bitmap == null )
					{
						bitmap = new Bitmap( OFFICE_ARROW_WIDTH, OFFICE_ARROW_HEIGHT );

						using( Graphics g = Graphics.FromImage( bitmap ) )
						{
							g.Clear( Color.Transparent );

							Rectangle rcLeftArrow = new Rectangle( 0, 3, 1, 1 );

							using( Region rg = new Region( rcLeftArrow ) )
							{
								rg.Union( rcLeftArrow );

								rcLeftArrow.Inflate( 0, 1 );
								rcLeftArrow.X += 1;
								rg.Union( rcLeftArrow );

								rcLeftArrow.Inflate( 0, 1 );
								rcLeftArrow.X += 1;
								rg.Union( rcLeftArrow );

								rcLeftArrow.Inflate( 0, 1 );
								rcLeftArrow.X += 1;
								rg.Union( rcLeftArrow );

								using( Brush brush = GetBackgroundBrushForArrow( OFFICE_ARROW_WIDTH ) )
								{
									g.FillRegion( brush, rg );
								}
							}
						}

						m_hImages[ EIMAGE.eiLeftArrow ] = bitmap;
					}

					return bitmap;
				}
			}
			#endregion

			#region Enums
			enum EIMAGE
			{
				eiMenuButtonFlashNormal = 0,
				eiMenuButtonFlashSelected,
				eiMenuButtonFlashPressed,
				eiMenuDropDownBackground,
				eiRightArrow,
				eiLeftArrow,
				MAX,
			}
			#endregion

			#region Fields
			private Bitmaps m_hImages;

			private Pen m_pShadow;
			private Pen m_pHighlight;

			static Blend m_blTabSelected;
			static Blend m_blTabChecked;
			static Blend m_blTabFlash;
			static Blend m_blTitle;
			static Blend m_blGroupTab;
			static Blend m_blGroupTop;
			static Blend m_blGroupRays;
			static Blend m_blSystemButton;
			static Blend m_blSystemButtonBorder;
			#endregion

			#region Nested classes
			class Bitmaps : Hashtable
			{
				public Bitmaps(int capacity)
					: base(capacity)
				{ }

				public override object this[object key]
				{
					get
					{
						return base[key];
					}
					set
					{
						IDisposable iDispose = base[key] as IDisposable;

						if (iDispose != null)
						{
							iDispose.Dispose();
						}

						base[key] = value;
					}
				}
			}
			#endregion
		}

		/// <summary>
		/// Office2010RibbonHeaderRenderer.
		/// </summary>
		internal class Office2010RibbonHeaderRenderer : System.Windows.Forms.ToolStripProfessionalRenderer
		{
			#region Ctor
			public Office2010RibbonHeaderRenderer(Office2010ColorTable colorTable)
				: base(colorTable)
			{ }
			#endregion

			#region Fileds
			
			static int ARROW_WIDTH = 5;
			static int ARROW_HEIGHT = 6;
			Bitmap arrowOverflow, arrowRightImage, arrowDownImage,checkButton ;
			private Bitmap rightArrow = null;
			private Bitmap leftArrow = null;
			Color[] selectionColors = null;
			ColorBlend selectionBlend = null;

			#endregion

			#region Properties

			internal Office2010ColorTable OfficeColorTable
			{
				get
				{
					return base.ColorTable as Office2010ColorTable;
				}
			}
			
			protected Bitmap ArrowOverflow
			{
				get
				{
					if (arrowOverflow == null)
					{
						arrowOverflow = new Bitmap(ARROW_HEIGHT + 1, ARROW_HEIGHT + 1);

						using (Graphics g = Graphics.FromImage(arrowOverflow))
						{
							g.Clear(Color.Transparent);

							g.SmoothingMode = SmoothingMode.AntiAlias;

							// First arrow.
							Rectangle rc = new Rectangle(0, 2, 1, 3);

							using (Region rgDark = new Region(rc))
							{
								rgDark.Union(rc);

								rc.Inflate(0, -1);
								rc.X += 1;
								rgDark.Union(rc);

								g.FillRegion(SystemBrushes.ControlText, rgDark);
							}

							// Second arrow.
							rc = new Rectangle(4, 2, 1, 3);
							using (Region rgDark = new Region(rc))
							{
								rgDark.Union(rc);

								rc.Inflate(0, -1);
								rc.X += 1;
								rgDark.Union(rc);

								g.FillRegion(SystemBrushes.ControlText, rgDark);
							}

							// Highlight for first arrow.
							rc = new Rectangle(0, 1, 1, 1);
							using (Region rgLight = new Region(rc))
							{
								rgLight.Union(rc);
								rc.X += 1;
								rc.Y += 1;
								rgLight.Union(rc);
								rc.X += 1;
								rc.Y += 1;
								rgLight.Union(rc);
								rc.X -= 1;
								rc.Y += 1;
								rgLight.Union(rc);
								rc.X -= 1;
								rc.Y += 1;
								rgLight.Union(rc);

								g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
							}

							// Highlight for second arrow.
							rc = new Rectangle(4, 1, 1, 1);
							using (Region rgLight = new Region(rc))
							{
								rgLight.Union(rc);
								rc.X += 1;
								rc.Y += 1;
								rgLight.Union(rc);
								rc.X += 1;
								rc.Y += 1;
								rgLight.Union(rc);
								rc.X -= 1;
								rc.Y += 1;
								rgLight.Union(rc);
								rc.X -= 1;
								rc.Y += 1;
								rgLight.Union(rc);

								g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
							}
						}
					}
					return arrowOverflow;
				}
			}

			protected Bitmap ArrowRightImage
			{
				get
				{
					if (arrowRightImage == null)
					{
						arrowRightImage = new Bitmap(ARROW_HEIGHT + 1, ARROW_WIDTH);
						using (Graphics g = Graphics.FromImage(arrowRightImage))
						{
							g.Clear(Color.Transparent);

							Rectangle rc = new Rectangle(0, 0, 1, ARROW_WIDTH);
							using (Region rgDark = new Region(rc))
							{
								rc.Offset(3, 0);
								while (rc.Y < rc.Bottom && rc.X < ARROW_HEIGHT)
								{
									rgDark.Union(rc);

									rc.Inflate(0, -1);
									rc.X += 1;
								}

								g.FillRegion(SystemBrushes.ControlText, rgDark);

								using (Region rgLight = rgDark.Clone())
								{
									rgLight.Translate(1, 0);
									rgLight.Exclude(rgDark);
									g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
								}
							}
						}
					}
					return arrowRightImage;
				}
			}
			private bool isEnable = true;
			internal bool IsEnable
			{
				get { return isEnable; }
				set {
						isEnable = value;
						arrowDownImage = null; 
					}
			}
			
			protected Bitmap ArrowDownImage
			{
				get
				{
					if (arrowDownImage == null)
					{
						arrowDownImage = new Bitmap(ARROW_HEIGHT + 1, ARROW_HEIGHT + 1);

						using (Graphics g = Graphics.FromImage(arrowDownImage))
						{
							g.Clear(Color.Transparent);

							Rectangle rc = new Rectangle(0, 0, ARROW_WIDTH , 1);
							Pen pen = new Pen(Color.Black);
							if (!IsEnable)
								pen = new Pen(Color.Gray );
								g.DrawLine(pen, new PointF(rc.X -2, rc.Y), new PointF(rc.Width +2, rc.Y));
							using (Region rgDark = new Region(rc))
							{
								rc.Offset(1, 3);
								while (rc.X < rc.Right && rc.Y < ARROW_HEIGHT)
								{
									rgDark.Union(rc);

									rc.Inflate(-1, 0);
									rc.Y += 1;
								}
								SolidBrush brush = new SolidBrush(Color.Black);
								if (!IsEnable)
									brush = new SolidBrush(Color.Gray );

								g.FillRegion(brush , rgDark);
								brush.Dispose();

                                //using (Region rgLight = rgDark.Clone())
                                //{
                                //    rgLight.Translate(0, 1);
                                //    rgLight.Exclude(rgDark);
                                //    g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                                //}
							}
                            pen.Dispose();
						}

					}
					return arrowDownImage;
				}
			}

			protected Bitmap CheckButton
			{
				get
				{
					if (checkButton == null)
					{
						Rectangle rcFlash = new Rectangle(0, 0, 10, 14);
						checkButton = new Bitmap(rcFlash.Width, rcFlash.Height);

						using (Graphics g = Graphics.FromImage(checkButton))
						{
							g.Clear(Color.Transparent);

							Point[] points = new Point[]
						{
							new Point(1,8), 
							new Point(3,12),
							new Point(8,1)
						};

							using (GraphicsPath path = new GraphicsPath())
							{
								path.AddLines(points);
							}
							g.SmoothingMode = SmoothingMode.AntiAlias;
							g.DrawLines(Pens.MidnightBlue, points);
						}
					}
					return checkButton;
				}
			}

			protected Bitmap RightArrow
			{
				get
				{
					if (rightArrow == null)
					{
						rightArrow = new Bitmap(ARROW_WIDTH, ARROW_HEIGHT);

						using (Graphics g = Graphics.FromImage(rightArrow))
						{
							g.Clear(Color.Transparent);

							Rectangle rcRightArrow = new Rectangle(0, 0, 1, ARROW_HEIGHT);

							using (Region rg = new Region(rcRightArrow))
							{
								rg.Union(rcRightArrow);

								rcRightArrow.Inflate(0, -1);
								rcRightArrow.X += 1;
								rg.Union(rcRightArrow);

								rcRightArrow.Inflate(0, -1);
								rcRightArrow.X += 1;
								rg.Union(rcRightArrow);

								using (Brush brush = new SolidBrush(Color.Black))
								{
									g.FillRegion(brush, rg);
								}
							}
						}
					}

					return rightArrow;
				}
			}

			protected Bitmap LeftArrow
			{
				get
				{
					if (leftArrow == null)
					{
						leftArrow = new Bitmap(ARROW_WIDTH, ARROW_HEIGHT);

						using (Graphics g = Graphics.FromImage(leftArrow))
						{
							g.Clear(Color.Transparent);

							Rectangle rcLeftArrow = new Rectangle(0, 2, 1, 1);

							using (Region rg = new Region(rcLeftArrow))
							{
								rg.Union(rcLeftArrow);

								rcLeftArrow.Inflate(0, 1);
								rcLeftArrow.X += 1;
								rg.Union(rcLeftArrow);

								rcLeftArrow.Inflate(0, 1);
								rcLeftArrow.X += 1;
								rg.Union(rcLeftArrow);

								using (Brush brush = new SolidBrush(Color.Black))
								{
									g.FillRegion(brush, rg);
								}
							}
						}
					}

					return leftArrow;
				}
			}

			internal Color[] SelectionColors
			{
				get
				{
					if (selectionColors == null)
					{
						selectionColors = new Color[] 
					{
						Color.FromArgb(255,226,119),
						Color.FromArgb(255,227,124),
						Color.FromArgb(255,229,133),
						Color.FromArgb(255,238,164),
						Color.FromArgb(255,244,192),
						Color.FromArgb(255,250,214),
						Color.FromArgb(255,252,224)
					};
					}

					return selectionColors;
				}
			}

			internal ColorBlend SelectionBlend
			{
				get
				{
					if (selectionBlend == null)
					{
						selectionBlend = new ColorBlend(7);
						selectionBlend.Positions = new float[]
					{
					   0F,
					   0.83F,
					   0.86F,
					   0.9F,
					   0.93F,
					   0.96F,
					   1F
					};
					}

					return selectionBlend;
				}

			}

			#endregion

			#region Overrides

			protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
			{
				if(!(e.Item is ToolStripMenuButton))
					base.OnRenderArrow(e);
			}

			protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
			{
				if (!SystemInfo.IsVisualStyleEnabled || !PaintButtonBackground(e))
				{
                    if (ToolStripRendererUtils.GetIsChecked(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
                    {
                        Rectangle rect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);

                        Point[] polygon = RendererUtils.GetRoundedPolygon(rect, 2);

                        using (GraphicsPath path = new GraphicsPath())
                        {
                            path.AddPolygon(polygon);

                            using (PathGradientBrush brush = new PathGradientBrush(path))
                            {
                                brush.SurroundColors = new Color[] { OfficeColorTable.ToolstripButtonPressedBackground };
                                brush.CenterColor = OfficeColorTable.ToolstripButtonCheckedBackground;
                                brush.FocusScales = new PointF(0.99f, 0.99f);

                                e.Graphics.FillPolygon(brush, polygon);
                            }

                            PaintButtonBorder(e, polygon, OfficeColorTable.ToolstripButtonPressedBorder);
                        }
                    }
				}
			}

			protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
			{
				if (!SystemInfo.IsVisualStyleEnabled || !PaintDropDownButtonBackground(e))
				{
					base.OnRenderDropDownButtonBackground(e);
				}
			}

			protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
			{
				if (!SystemInfo.IsVisualStyleEnabled || !PaintImageMargin(e))
				{
					base.OnRenderImageMargin(e);
				}
			}

			protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
			{
				base.OnRenderItemImage(e);
			}

			protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
			{
				if (!SystemInfo.IsVisualStyleEnabled || !PaintItemCheck(e))
				{
					base.OnRenderItemCheck(e);
				}
			}

			protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
			{
				if (!SystemInfo.IsVisualStyleEnabled || !PaintItemText(e))
				{
					base.OnRenderItemText(e);
				}
			}

			protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
			{
				base.OnRenderSeparator(e);
			}

			protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
			{
				if (!SystemInfo.IsVisualStyleEnabled || !PaintSplitButtonBackground(e))
				{
					base.OnRenderSplitButtonBackground(e);
				}
			}

			protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
			{
				if (!SystemInfo.IsVisualStyleEnabled || !PaintToolstripBackground(e))
				{
					base.OnRenderToolStripBackground(e);
				}
			}

			protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
			{
				if (!(e.ToolStrip is MenuDropDown) && !(e.ToolStrip is RibbonControlAdvHeader))
				{
					base.OnRenderToolStripBorder(e);
				}
			}

			protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
			{
				if (!SystemInfo.IsVisualStyleEnabled || !PaintOverflowButtonBackground(e))
				{
					base.OnRenderOverflowButtonBackground(e);
				}
			}

			protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
			{
				if (!SystemInfo.IsVisualStyleEnabled || !PaintMenuItemBackground(e))
				{
					base.OnRenderMenuItemBackground(e);
				}
			}

			#endregion

			#region Implementation

			private bool PaintMenuItemBackground(ToolStripItemRenderEventArgs e)
			{
				bool result = false;

				if (e.Item.IsOnDropDown)
				{
					result = PaintDropDownMenuItemBackground(e);
				}
				else
				{
					result = PaintToolStripMenuItemBackground(e);
				}
				return result;
			}

			private bool PaintToolStripMenuItemBackground(ToolStripItemRenderEventArgs e)
			{
				bool result = false;

				return result;
			}

			private bool PaintDropDownMenuItemBackground(ToolStripItemRenderEventArgs e)
			{
				bool result = false;

				ToolStripMenuItem tsItem = e.Item as ToolStripMenuItem;

				if ((tsItem != null && tsItem.Enabled) && tsItem.Selected || tsItem.Pressed)
				{
                    Image image = GetToolstripItemImageSelected(new Rectangle(Point.Empty, tsItem.Size), true);

					e.Graphics.DrawImage(image, Point.Empty);

					result = true;
				}

				return result;
			}

			public void DrawSeparators(RibbonControlAdvHeader header, Graphics g, Rectangle rc)
			{
				int iTop = header.TabItemsRectangle.Top + 2;
				int iBottom = header.TabItemsRectangle.Bottom;

				Rectangle rcBrush = new Rectangle(0, iTop, 2, iBottom);
				using (Brush brush = new LinearGradientBrush(rcBrush, OfficeColorTable.TabItemSeparatorGradientBegin, Color.Transparent, LinearGradientMode.Vertical))
				{
					for (int i = 0, count = header.m_iSepatators.Count; i < count; i++)
					{
						rcBrush.X = header.m_iSepatators[i];

						if (header.m_iSepatators[i] < rc.Right - (header.m_bIsRightScroll ? SCROLL_BUTTON_WIDTH : 0)
							&& header.m_iSepatators[i] > rc.Left + (header.m_bIsLeftScroll ? SCROLL_BUTTON_WIDTH : 0))
						{
							g.FillRectangle(brush, rcBrush);
						}
					}
				}
			}

			private bool PaintOverflowButtonBackground(ToolStripItemRenderEventArgs e)
			{
				RibbonControlAdvHeader.QuickItemsOverflowButton item = e.Item as RibbonControlAdvHeader.QuickItemsOverflowButton;

				if (item != null)
				{
					Rectangle rc = Office12ToolStripRenderer.GetButtonRect(e.Item, new Rectangle(Point.Empty, e.Item.Size), Office12ToolStripRenderer.ERENDERTYPE.Normal);
					if (rc.Width > 0 && rc.Height > 0)
					{
						ToolStrip ts = e.ToolStrip;
						if (ts != null)
						{
							PaintButtonBackground(e);

							Image imgArrow = ArrowOverflow;

							int left = rc.X + (rc.Width - imgArrow.Size.Width) / 2;
							int top = rc.Y + (rc.Height - imgArrow.Size.Height) / 2;

							e.Graphics.DrawImage(imgArrow, left, top);
						}
					}
				}
				else
				{
					Rectangle rc = Office12ToolStripRenderer.GetButtonRect(e.Item, new Rectangle(Point.Empty, e.Item.Size), Office12ToolStripRenderer.ERENDERTYPE.Normal);
					if (rc.Width > 0 && rc.Height > 0)
					{
						ToolStrip ts = e.ToolStrip;
						if (ts != null)
						{
							PaintButtonBackground(e);

							bool bVertical = ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right;
							Image imgArrow = bVertical ? ArrowRightImage : ArrowDownImage;

							int left = rc.X + (rc.Width - imgArrow.Size.Width) / 2;
							int top = rc.Y + (rc.Height - imgArrow.Size.Height) / 2;

							e.Graphics.DrawImage(imgArrow, left, top);
						}
					}
				}

				return true;
			}

			private bool PaintItemCheck(ToolStripItemImageRenderEventArgs e)
			{
				ToolStripItem tsItem = e.Item;

				if (tsItem.Image == null || ToolStripRendererUtils.IsCheckMarginVisible(tsItem))
				{
					PaintItemCheckBackground(e);

					Rectangle rc = GetCheckRect(e);
					if (rc.Width > 0 && rc.Height > 0)
					{
						Size szImage = this.CheckButton.Size;

						int iImageX = rc.X + (rc.Width - szImage.Width) / 2;
						int iImageY = rc.Y + (rc.Height - szImage.Height) / 2;

						e.Graphics.DrawImage(this.CheckButton, new Point(iImageX, iImageY));
					}
				}

				return true;
			}

			private void PaintItemCheckBackground(ToolStripItemImageRenderEventArgs e)
			{
				Rectangle rc = GetCheckRect(e);

				if (rc.Width > 0 && rc.Height > 0)
				{
					Graphics g = e.Graphics;

					GraphicsState gState = g.Save();
					g.SmoothingMode = SmoothingMode.AntiAlias;

					using (Brush brush = new SolidBrush(this.OfficeColorTable.ToolstripButtonCheckedBackground))
					{
						g.FillRectangle(brush, Rectangle.Inflate(rc, -1, -2));
					}
					using (Pen pen = new Pen(this.OfficeColorTable.ToolstripButtonPressedBorder))
					{
						g.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(rc, 1));
					}

					g.Restore(gState);
				}
			}

			private bool PaintItemText(ToolStripItemTextRenderEventArgs e)
			{
				bool bResult = PaintMenuButtonText(e) || PaintToolStripTabItemText(e);

				return bResult;
			}

			private bool PaintMenuButtonText(ToolStripItemTextRenderEventArgs e)
			{
				bool result = false;

				ToolStripMenuButton button = e.Item as ToolStripMenuButton;
				RibbonControlAdvHeader header = e.Item.Owner as RibbonControlAdvHeader;
				if (button != null && header != null)
				{
					if (header.Form != null && header.Form.CompositionEnabled)
					{
                        RibbonControlAdvHeaderRenderer.DrawThemeText(e.Graphics, e.ToolStrip.Handle, e.TextRectangle, e.Text, e.TextFont, Color.White, false);
						result = true;
					}
                    else
                    {
                        if (header.m_owner.MenuButtonEnabled)
                            e.Graphics.DrawString(e.Text, e.TextFont, new SolidBrush(Color.White), e.TextRectangle.Location);
                        else
                            e.Graphics.DrawString(e.Text, e.TextFont, new SolidBrush(Color.LightGray), e.TextRectangle.Location);
                        result = true;
                    }
				}

				return result;
			} 

			private bool PaintToolStripTabItemText(ToolStripItemTextRenderEventArgs e)
			{
				bool bResult = false;

				ToolStripTabItem item = e.Item as ToolStripTabItem;
				if (item != null)
				{
					Rectangle rc = ToolStripRendererUtils.GetTextRect(e);
					TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix;

					RibbonControlAdvHeader header = e.Item.Owner as RibbonControlAdvHeader;

					if (header != null)
					{
						Rectangle rcItemBounds = new Rectangle(new Point(e.Item.Bounds.Left + rc.X, e.Item.Bounds.Top + rc.Y), rc.Size);
						Rectangle rcIntersection = Rectangle.Intersect(header.TabItemsRectangle, rcItemBounds);

						int iTextWidthDifference = rcItemBounds.Width - rcIntersection.Width;
						rc.Width -= iTextWidthDifference;

						if (rcItemBounds.Left < header.TabItemsRectangle.Left && rcIntersection.Width > 0)
						{
							int iLeft = rcItemBounds.Width - rcIntersection.Width;

							if (header.m_bIsLeftScroll)
							{
								iLeft += SCROLL_BUTTON_WIDTH;
								rc.Width -= SCROLL_BUTTON_WIDTH;
							}

							rc.X += iLeft;
							rc.Width -= 2;

							flags |= TextFormatFlags.Right;
						}
						else
						{
							flags |= TextFormatFlags.Left;
							rc.Width -= 2;
						}
					}

					RibbonForm form = (e.ToolStrip as RibbonControlAdvHeader).Form;
					Color clText = item.Checked && !this.IsPanelHidden(item) ? OfficeColorTable.ToolstripTabItemCheckedForeColor : OfficeColorTable.ToolstripTabItemForeColor;

					if (form != null && form.CompositionEnabled)
					{
						RibbonControlAdvHeaderRenderer.DrawThemeText(e.Graphics, e.ToolStrip.Handle, rc, e.Text, e.TextFont, clText, false);
					}
					else
					{
						TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, rc, clText, flags);
					}

					bResult = true;
				}

				return bResult;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="item"></param>
			/// <returns></returns>
			private bool IsPanelHidden(ToolStripItem item)
			{
				bool bResult = false;

				RibbonControlAdvHeader header = item.Owner as RibbonControlAdvHeader;
				if (header != null)
				{
					RibbonControlAdv ribbon = header.Parent as RibbonControlAdv;
					if (ribbon != null)
					{
						bResult = (ribbon.MinimizePanel || !ribbon.VisiblePanel);
					}
				}

				return bResult;
			}

			private bool PaintButtonBackground(ToolStripItemRenderEventArgs e)
			{
				bool bResult = PaintSystemButtonBackground(e) ||
					PaintTabItemBackGround(e) ||
					PaintButtonBackGround(e) ||
					PaintButtonOnSystemPanelBackGround(e);

				return bResult;
			}

			private bool PaintButtonOnSystemPanelBackGround(ToolStripItemRenderEventArgs e)
			{
				// Paint Buttons on BackStage, Option, Exit Button in One Note.
				return false;
			}

			private bool PaintSystemButtonBackground(ToolStripItemRenderEventArgs e)
			{
				if (e.Item is SystemButton && !(e.Item is HelpSystemButton))
				{
					if (!PaintSystemButtonPressed(e))
					{
						PaintSystemButtonSelected(e);
					}
					return true;
				}
				return false;
			}

			private bool PaintSystemButtonSelected(ToolStripItemRenderEventArgs e)
			{
				SystemButton sysButton = e.Item as SystemButton;

				if (sysButton != null && sysButton.Selected)
				{
					Rectangle rect = new Rectangle(Point.Empty, sysButton.Size);

					Point[] polygon = RendererUtils.GetRoundedPolygon(rect, 2);

					Color gradientBegin, gradientEnd, border;

					if (sysButton.SysCommand == (int)SystemCommand.SC_CLOSE)
					{
						gradientBegin = OfficeColorTable.SystemCloseButtonSelectedGradientBegin;
						gradientEnd = OfficeColorTable.SystemCloseButtonSelectedGradientEnd;
						border = OfficeColorTable.SystemCloseButtonBorder;
					}
					else
					{
						gradientBegin = OfficeColorTable.SytemButtonSelectedGradientBegin;
						gradientEnd = OfficeColorTable.SystemButtonSelectedGradientEnd;
						border = OfficeColorTable.SystemButtonBorder;
					}

					PaintSystemButtonGradient(e, rect, polygon, gradientBegin, gradientEnd);

					PaintButtonBorder(e, polygon, border);

					return true;
				}

				return false;
			}

			private void PaintButtonBorder(ToolStripItemRenderEventArgs e, Point[] polygon, Color border)
			{
				using (Pen pen = new Pen(border))
				{
					e.Graphics.DrawPolygon(pen, polygon);
				}
			}

			private void PaintSystemButtonGradient(ToolStripItemRenderEventArgs e, Rectangle rect, Point[] polygon, Color gradientBegin, Color gradientEnd)
			{
				using (LinearGradientBrush brush = new LinearGradientBrush(rect, gradientBegin, gradientEnd, LinearGradientMode.Vertical))
				{
					e.Graphics.FillPolygon(brush, polygon);
				}
			}

			private bool PaintSystemButtonPressed(ToolStripItemRenderEventArgs e)
			{
				SystemButton sysButton = e.Item as SystemButton;

				if (sysButton != null && sysButton.Pressed)
				{
					Rectangle rect = new Rectangle(Point.Empty, sysButton.Size);

					Point[] polygon = RendererUtils.GetRoundedPolygon(rect, 2);

					Color background, border;

					if (sysButton.SysCommand == (int)SystemCommand.SC_CLOSE)
					{
						background = OfficeColorTable.SystemCloseButtonPressedBackground;
						border = OfficeColorTable.SystemCloseButtonBorder;
					}
					else
					{
						background = OfficeColorTable.SystemButtonPressedBackground;
						border = OfficeColorTable.SystemButtonBorder;
					}

					PaintButtonPressedGradient(e, polygon, background, border);

					PaintButtonBorder(e, polygon, border);

					return true;
				}

				return false;
			}

			private bool PaintButtonPressedGradient(ToolStripItemRenderEventArgs e, Point[] polygon, Color background, Color border)
			{
				using (GraphicsPath path = new GraphicsPath())
				{
					path.AddPolygon(polygon);

					using (PathGradientBrush brush = new PathGradientBrush(path))
					{
						brush.SurroundColors = new Color[] { border };
						brush.CenterColor = background;
						brush.FocusScales = new PointF(0.9F, 0.9F);

						e.Graphics.FillPolygon(brush, polygon);
					}
				}

				return true;
			}

			private bool PaintTabItemBackGround(ToolStripItemRenderEventArgs e)
			{
				if (e.Item is ToolStripTabItem)
				{
					if (!PaintTabItemChecked(e))
					{
						PaintTabItemSelected(e);
					}

					return true;
				}
				return false;
			}

			private bool PaintTabItemSelected(ToolStripItemRenderEventArgs e)
			{
				bool result = false;

				ToolStripTabItem tsTabItem = e.Item as ToolStripTabItem;

				RibbonControlAdvHeader header = (tsTabItem.Owner as RibbonControlAdvHeader);

				RibbonForm form = (header != null) ? header.Form : null;

				bool isActive = (form == null) || (form.ActiveState);

				if (tsTabItem != null && isActive && ( tsTabItem.Selected || tsTabItem.Panel.Visible))
				{
					Rectangle rect = new Rectangle(Point.Empty,tsTabItem.Size);
					Color clrGroup = Color.Empty;
					ToolStripTabGroup group = (header != null) ? header.GetItemGroup(e.Item) : null;

					Point[] polygon = RendererUtils.GetSquareRoundedToUpPolygon(rect, 2);

					Rectangle innerRect = new Rectangle(1, 1, rect.Width - 3, rect.Height);

					Rectangle upperRect = new Rectangle(2, 2, innerRect.Width, innerRect.Height / 3);

					using (Brush brush = new SolidBrush(OfficeColorTable.ToolstripTabItemBackgournd))
					{
						e.Graphics.FillRectangle(brush, innerRect);
					}

					clrGroup = (group != null) ? Color.FromArgb(100, group.Color) : OfficeColorTable.ToolstripTabItemSelectedGradientBegin;

					using (Brush brush = new LinearGradientBrush(upperRect, clrGroup , OfficeColorTable.ToolstripTabItemSelectedGradientEnd, LinearGradientMode.Vertical))
					{
						e.Graphics.FillRectangle(brush, upperRect);
					}

					clrGroup = (group != null) ? clrGroup : OfficeColorTable.ToolstripTabItemAntialiasing;

					using (Pen pen = new Pen(clrGroup))
					{
						e.Graphics.DrawRectangle(pen, innerRect);
					}
                    Color clr = ColorTranslator.FromHtml("#BBCEE6");
                    if (tsTabItem.Panel.OfficeColorScheme == ToolStripEx.ColorScheme.Silver)
                       clr = ColorTranslator.FromHtml("#E5E7E9");
                    else if (tsTabItem.Panel.OfficeColorScheme == ToolStripEx.ColorScheme.Black)
                        clr = ColorTranslator.FromHtml("#717171");
                    else if (tsTabItem.Panel.OfficeColorScheme == ToolStripEx.ColorScheme.Blue)
                        clr = ColorTranslator.FromHtml("#BBCEE6");
					PaintButtonBorder(e, polygon, clr);

					result = true;
				}

				return result;
			}

			private bool PaintTabItemChecked(ToolStripItemRenderEventArgs e)
			{
				bool result = false;

				ToolStripTabItem tsTabItem = e.Item as ToolStripTabItem;

				RibbonControlAdvHeader header = tsTabItem.Owner as RibbonControlAdvHeader;

				if (tsTabItem != null &&  header!=null && tsTabItem.Checked && !header.m_owner.MinimizePanel)
				{
					if (tsTabItem.Bounds.Right > header.TabItemsRectangle.Right
						|| tsTabItem.Bounds.Left < header.TabItemsRectangle.Left)
						return true;

					Rectangle rect = new Rectangle(Point.Empty, tsTabItem.Size);
					Point[] polygon = RendererUtils.GetSquareRoundedToUpPolygon(rect, 2);
					ToolStripTabGroup group = (header != null) ? header.GetItemGroup(e.Item) : null;

					Rectangle innerRect = new Rectangle(1, 1, rect.Width - 2, rect.Height);

					Color clrGroup = (group != null) ? Color.FromArgb(10, group.Color) : OfficeColorTable.ToolstripTabItemCheckedGradientBegin;

					using (Brush brush = new LinearGradientBrush(innerRect, clrGroup, OfficeColorTable.ToolstripTabItemCheckedGradientEnd, LinearGradientMode.Vertical))
					{
						e.Graphics.FillRectangle(brush, innerRect);
					}
					Pen pen = new Pen(OfficeColorTable.ToolstripTabItemBorder);
					 pen.Dispose();
					GraphicsState state = e.Graphics.Save();

					e.Graphics.ExcludeClip(new Rectangle(rect.Left+1, rect.Height -1, rect.Width-2, 1));
					Color color = (group != null) ? group.Color : OfficeColorTable.ToolstripTabItemBorder;
					(e.Item.Owner.Parent as RibbonControlAdv).ActiveTabGroupColor = color;
					PaintButtonBorder(e, polygon, color);

				   
					result = true;
				}

				return result;
			}

			private bool PaintToolstripBackground(ToolStripRenderEventArgs e)
			{
				bool result = (PaintHeaderBackground(e) ||
					PaintDropDownBackground(e) ||
					PaintDropDownExBackground(e));

				return result;
			}

			private bool PaintDropDownExBackground(ToolStripRenderEventArgs e)
			{
				bool result = false;

				RECT rect = new RECT();

				ContextMenuStripEx menuStrip = e.ToolStrip as ContextMenuStripEx;

				if (menuStrip != null)
				{
					if (WindowsAPI.GetWindowRect(menuStrip.Handle, ref rect))
					{
						if (rect.Width > 0 && rect.Height > 0)
						{
							Graphics g = e.Graphics;

							Rectangle rc = new Rectangle(Point.Empty, rect.Size);

							Rectangle rectangleTitle = new Rectangle(0, 0, rc.Width, menuStrip.TitleHeight);
							Rectangle rectangleBody = new Rectangle(0, menuStrip.TitleHeight, rc.Width, rc.Height - menuStrip.TitleHeight);

							using (Brush brush = new SolidBrush(this.OfficeColorTable.ToolStripDropDownBackground))
							{
								e.Graphics.FillRectangle(brush, rectangleBody);
							}

							using (Brush brush = new SolidBrush(this.OfficeColorTable.ContextMenuTitleBackground))
							{
								e.Graphics.FillRectangle(brush, rectangleTitle);
							}

							using (Pen pen = new Pen(Color.FromArgb(100, Color.LightGray )))
							{
								e.Graphics.DrawLine(
									pen, rectangleTitle.Left + 3, rectangleTitle.Bottom, rectangleTitle.Right - 3, rectangleTitle.Bottom);
							}

							Rectangle rcText = Rectangle.Inflate(rectangleTitle, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN);

							TextFormatFlags flags = TextFormatFlags.EndEllipsis;

							if (menuStrip.RightToLeft == RightToLeft.Yes)
							{
								flags |= TextFormatFlags.Right;
							}
							TextRenderer.DrawText(g, menuStrip.Text, menuStrip.HeaderFont, rcText,Color.FromArgb (100,Color.Black ), flags);

							result = true;
						}
					}
				}
				return result;
			}

			private bool PaintDropDownBackground(ToolStripRenderEventArgs e)
			{
				return false;
			}

			private bool PaintHeaderBackground(ToolStripRenderEventArgs e)
			{
				bool result = false;

				RibbonControlAdvHeader header = e.ToolStrip as RibbonControlAdvHeader;
				if (header != null)
				{
					Graphics g = e.Graphics;

					GraphicsState state = g.Save();

					Rectangle rcDisplay = header.DisplayRectangle;

					Rectangle fullRect = new Rectangle(rcDisplay.X, rcDisplay.Y, header.Width + 20 , header.Height + 20);

					g.SetClip(fullRect, CombineMode.Replace);

					RibbonForm form = header.Form;

					bool bCompositionEnabled = (form != null) ? form.CompositionEnabled && header.m_owner.Dock == DockStyleEx.TopMost : false;

					if (!bCompositionEnabled)
					{
						Color color = ((form != null && form.ActiveState)) ? OfficeColorTable.ActiveHeaderBackground : OfficeColorTable.InActiveHeaderBackground;

						using (SolidBrush brush = new SolidBrush(color))
						{
							g.FillRectangle(brush, fullRect);
						}

						result = true;
					}
					else
					{
						DwmAPI.FillBlackRegion(g, fullRect);
                        form.UpdateFrame();
						Rectangle loweRect = new Rectangle(fullRect.X, fullRect.Y + fullRect.Height / 2, fullRect.Width, fullRect.Height / 2);
                        Color clr = ColorTranslator.FromHtml("#BBCEE6");
                        if (form.ColorScheme == RibbonForm.ColorSchemeType.Black)
                            clr = ColorTranslator.FromHtml("#717171");
                        else if(form.ColorScheme == RibbonForm.ColorSchemeType.Silver)
                            clr = ColorTranslator.FromHtml("#E5E7E9");
                        else if(form.ColorScheme == RibbonForm.ColorSchemeType.Blue)
                            clr = ColorTranslator.FromHtml("#f4f9ff");
                        using (LinearGradientBrush brush = new LinearGradientBrush(fullRect, Color.Transparent, clr, LinearGradientMode.Vertical))
                        {
                            e.Graphics.FillRectangle(brush, fullRect);
                        }

						result = true;
					}

					// Draw Quick items region
					if (!header.ShowQuickPanelBelowRibbon && header.QuickItems.Count > 0)
					{
						if (header.RightToLeft == RightToLeft.No)
						{
							PaintQuickItemsBackgroundLeftToRight(g, header);
						}
						else
						{
							PaintQuickItemsBackgroundRightToLeft(g, header);
						}
					}

					string sTitle = header.Title;
					if (sTitle != string.Empty)
					{
                        if (form != null)
                        {
                            bool mdiactivate = false;
                            foreach (Form mdi in form.MdiChildren)
                            {
                                if (mdi.IsHandleCreated)
                                {
                                    mdiactivate = true;
                                }
                            }
                            if (form.ContainsFocus || form.designmode || mdiactivate)
                            {
                                Color titleColor = header.TitleColor == Color.Empty ? header.ForeColor : header.TitleColor;
                                Rectangle rcTitle = header.TitleRect;
                                if (form.CompositionEnabled)
                                {
                                    rcTitle.Y += 10;
                                    RibbonControlAdvHeaderRenderer.DrawThemeText(g, header.Handle, rcTitle, sTitle, header.TitleFont, titleColor, false);
                                }
                                else
                                {
                                    rcTitle.Y += 5;
                                    TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix;
                                    TextRenderer.DrawText(g, sTitle, header.TitleFont, rcTitle, titleColor, flags);
                                }
                            }
                            else
                            {
                                Color titleColor = header.TitleColor == Color.Empty ? header.ForeColor : header.TitleColor;
                                Rectangle rcTitle = header.TitleRect;
                                if (form != null && form.CompositionEnabled)
                                {
                                    rcTitle.Y += 10;
                                    RibbonControlAdvHeaderRenderer.DrawThemeText(g, header.Handle, rcTitle, sTitle, header.TitleFont, ControlPaint.LightLight(titleColor), false);
                                }
                                else
                                {
                                    rcTitle.Y += 5;
                                    TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix;
                                    TextRenderer.DrawText(g, sTitle, header.TitleFont, rcTitle, ControlPaint.LightLight(titleColor), flags);
                                }
                            }
                        }
					}

					PaintTabGroups(header,g);

					g.Restore(state);
                    RibbonControlAdv ribbon = header.Parent as RibbonControlAdv;
					if (form != null && form.ShowIcon && form.Icon != null)
					{
						Rectangle rcIcon = Rectangle.Empty;

                        if (e.Graphics.DpiX > 120)
                        {
                            rcIcon = ribbon.RightToLeft == RightToLeft.Yes
                                    ? new Rectangle(rcDisplay.Right - 26, 10, 24, 24)
                                    : new Rectangle(8, 10, 24, 24);
                        }
                        else if (e.Graphics.DpiX > 96)
                        {
                            rcIcon = ribbon.RightToLeft == RightToLeft.Yes
                                    ? new Rectangle(rcDisplay.Right - 26, 10, 20, 20)
                                    : new Rectangle(8, 10, 20, 20);
                        }
                        else
                        {
                            rcIcon = ribbon.RightToLeft == RightToLeft.Yes
                                         ? new Rectangle(rcDisplay.Right - 26, 10, 16, 16)
                                         : new Rectangle(8, 10, 16, 16);
                        }
						e.Graphics.DrawIcon(form.Icon, rcIcon);
					}
				}

				return result;
			}

			private bool PaintTabGroups(RibbonControlAdvHeader header, Graphics g)
			{
				if (header != null)
				{
					foreach (ToolStripTabGroup group in header.Groups)
					{
						if (group.Visible)
						{
							for (int i = 0, count = group.BoundsList.Count; i < count; i++)
							{
								Rectangle rect = group.BoundsList[i];

								using (Brush brush = new LinearGradientBrush(rect, Color.FromArgb(100, group.Color), Color.Transparent, LinearGradientMode.Vertical))
								{
									g.FillRectangle(brush, rect);
								}

								using (Brush brush = new SolidBrush(group.Color))
								{
									g.FillRectangle(brush, new Rectangle(rect.X, rect.Y, rect.Width, 4));
								}

								Rectangle fullRect = new Rectangle(rect.X, rect.Y, rect.Width, header.Height);

								using (Brush brush = new LinearGradientBrush(fullRect, Color.FromArgb(250, group.Color), Color.Transparent, LinearGradientMode.Vertical))
								{
									g.FillRectangle(brush, fullRect.X, fullRect.Y, 1, fullRect.Height);
									g.FillRectangle(brush, fullRect.Right - 1, fullRect.Y, 1, fullRect.Height);
								}

                                Font font = new Font(group.Font, FontStyle.Bold);
								if (header.Form != null && header.Form.CompositionEnabled)
								{
                                    rect.Y += 5;
                                    if (OfficeColorTable.ToolstripTabItemForeColor == Color.White)
                                        RibbonControlAdvHeaderRenderer.DrawThemeText(g, header.Handle, rect, group.Name, font, Color.FromArgb(255,255,255,255), true);
                                    else
                                        RibbonControlAdvHeaderRenderer.DrawThemeText(g, header.Handle, rect, group.Name, font, OfficeColorTable.ToolstripTabItemForeColor, true);
								}
								else
								{
                                    rect.Y -= 5;
                                    TextRenderer.DrawText(g, group.Name, font, rect, OfficeColorTable.ToolstripTabItemForeColor,
                                        TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
								}
								font.Dispose();
							}
						}
					}
					return true;
				}
				return false;
			}

			private bool PaintQuickItemsBackgroundRightToLeft(Graphics g, RibbonControlAdvHeader header)
			{
				bool result = false;

				if (header.QuickPanelVisible)
				{
					int height = header.QuickPanelHeight - 1;

					if (height > 0)
					{
						Rectangle rcDisplay = header.DisplayRectangle;

						Rectangle recQI = Rectangle.Empty;
						recQI.X = (header.Form != null && header.Form.ShowIcon) ? DEF_ICON_SIZE + DEF_PADDING + 12 : DEF_PADDING;
						recQI.Y = rcDisplay.Y + smQuickItems.Top + 9;
						recQI.Width = header.QuickPanelWidth;
						recQI.Height = header.QuickPanelHeight;

						Rectangle rcSep = new Rectangle(rcDisplay.Right - recQI.X, recQI.Y + 1, QUICK_ITEM_SEPARATOR_WIDTH, QUICK_ITEM_SEPARATOR_HEIGHT);

                        if (g.DpiX > 120)
                        {
                            const int DPI_150_ExtraIconSize = 8;
                            rcSep = new Rectangle(rcDisplay.Right - (recQI.X + DPI_150_ExtraIconSize), recQI.Y + 1, QUICK_ITEM_SEPARATOR_WIDTH, DPI_150_QUICK_ITEM_SEPARATOR_HEIGHT);
                        }
                        else if (g.DpiX > 96)
                        {
                            const int DPI_125_ExtraIconSize = 4;
                            rcSep = new Rectangle(rcDisplay.Right - (recQI.X + DPI_125_ExtraIconSize), recQI.Y + 1, QUICK_ITEM_SEPARATOR_WIDTH, DPI_125_QUICK_ITEM_SEPARATOR_HEIGHT);
                        } 
                        PaintQuickItemSeparator(g, rcSep);
                        if (g.DpiX > 120)
                        {
                            const int DPI_150_ExtraIconSize = 8;
                            rcSep = new Rectangle(rcDisplay.Right - (recQI.Right - QUICK_ITEM_SEPARATOR_WIDTH) + DPI_150_ExtraIconSize, recQI.Y + 1,
                                              QUICK_ITEM_SEPARATOR_WIDTH, DPI_150_QUICK_ITEM_SEPARATOR_HEIGHT);
                        }
                        else if (g.DpiX > 96)
                        {
                            const int DPI_125_ExtraIconSize = 4;
                            rcSep = new Rectangle(rcDisplay.Right - (recQI.Right - QUICK_ITEM_SEPARATOR_WIDTH) + DPI_125_ExtraIconSize, recQI.Y + 1,
                                                 QUICK_ITEM_SEPARATOR_WIDTH, DPI_125_QUICK_ITEM_SEPARATOR_HEIGHT);
                        }
                        else
                            rcSep = new Rectangle(rcDisplay.Right - (recQI.Right - QUICK_ITEM_SEPARATOR_WIDTH), recQI.Y + 1,
                                                  QUICK_ITEM_SEPARATOR_WIDTH, QUICK_ITEM_SEPARATOR_HEIGHT);

						PaintQuickItemSeparator(g, rcSep);
					}
				}

				return result;
			}

			private bool PaintQuickItemsBackgroundLeftToRight(Graphics g, RibbonControlAdvHeader header)
			{
				bool result = false;


				if (header.QuickPanelVisible)
				{
					int height = header.QuickPanelHeight - 1;

					if (height > 0)
					{
						Rectangle rcDisplay = header.DisplayRectangle;

						Rectangle recQI = Rectangle.Empty;
						recQI.X = (header.Form != null && header.Form.ShowIcon) ? DEF_ICON_SIZE + DEF_PADDING : DEF_PADDING;
						recQI.Y = rcDisplay.Y + 9;
						recQI.Width = header.QuickPanelWidth;
						recQI.Height = header.QuickPanelHeight;

						Rectangle rcSep = new Rectangle(recQI.X, recQI.Y + 1, QUICK_ITEM_SEPARATOR_WIDTH, QUICK_ITEM_SEPARATOR_HEIGHT);
                        if (g.DpiX > 120)
                        {
                            const int DPI_150_ExtraIconSize = 8;
                            rcSep = new Rectangle(recQI.X + DPI_150_ExtraIconSize, recQI.Y + 1, QUICK_ITEM_SEPARATOR_WIDTH, DPI_150_QUICK_ITEM_SEPARATOR_HEIGHT);
                        }
                        else if (g.DpiX > 96)
                        {
                            const int DPI_125_ExtraIconSize = 4;
                            rcSep = new Rectangle(recQI.X + DPI_125_ExtraIconSize, recQI.Y + 1, QUICK_ITEM_SEPARATOR_WIDTH, DPI_125_QUICK_ITEM_SEPARATOR_HEIGHT);
                        }
                        PaintQuickItemSeparator(g, rcSep);

                        if (g.DpiX > 120)
                        {
                            const int DPI_150_ExtraIconSize = 8;
                            rcSep = new Rectangle(recQI.Right - QUICK_ITEM_SEPARATOR_WIDTH + DPI_150_ExtraIconSize, recQI.Y + 1, QUICK_ITEM_SEPARATOR_WIDTH, DPI_150_QUICK_ITEM_SEPARATOR_HEIGHT);
                        }
                        else if (g.DpiX > 96)
                        {
                            const int DPI_125_ExtraIconSize = 4;
                            rcSep = new Rectangle(recQI.Right - QUICK_ITEM_SEPARATOR_WIDTH + DPI_125_ExtraIconSize, recQI.Y + 1, QUICK_ITEM_SEPARATOR_WIDTH, DPI_125_QUICK_ITEM_SEPARATOR_HEIGHT);
                        }
                        else
                            rcSep = new Rectangle(recQI.Right - QUICK_ITEM_SEPARATOR_WIDTH, recQI.Y + 1, QUICK_ITEM_SEPARATOR_WIDTH, QUICK_ITEM_SEPARATOR_HEIGHT);
                        PaintQuickItemSeparator(g, rcSep);
					}
				}

				return result;
			}

			private Point[] PaintQuickItemSeparator(Graphics g, Rectangle bounds)
			{
				Point[] polygon = RendererUtils.GetRoundedPolygon(bounds, 1);
				bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);

				using (Pen pen = new Pen(Color.FromArgb(100, OfficeColorTable.QuickItemSeparatorBorder)))
				{
					g.DrawRectangle(pen, bounds);
				}

				using (SolidBrush brush = new SolidBrush(OfficeColorTable.QuickItemSeparatorBackground))
				{
					g.FillPolygon(brush, polygon);
				}

				using (Pen pen = new Pen(OfficeColorTable.QuickItemSeparatorBorder))
				{
					g.DrawPolygon(pen, polygon);
				}
				return polygon;
			}

			private bool PaintDropDownButtonBackground(ToolStripItemRenderEventArgs e)
			{
				bool bResult = (PaintDropDownMenuButtonBackground(e) ||
					PaintQuickItemsDropDownButtonBackground(e) ||
					PaintButtonBackGround(e));

				return bResult;
			}

			internal bool PaintButtonBackGround(ToolStripItemRenderEventArgs e)
			{
				bool result = false;

				ToolStripItem tsBtn = e.Item as ToolStripButton;

				if (tsBtn == null)
					tsBtn = e.Item as ToolStripDropDownButton;

                if (tsBtn != null )
				{
                    bool focus = true;
					if (tsBtn.Pressed)
					{
						Rectangle rect = new Rectangle(0, 0, tsBtn.Width - 1, tsBtn.Height - 1);

						Point[] polygon = RendererUtils.GetRoundedPolygon(rect, 2);

						PaintButtonPressedGradient(e, polygon, OfficeColorTable.ToolstripButtonPressedBackground, OfficeColorTable.ToolstripButtonPressedBorder);

						PaintButtonBorder(e, polygon, OfficeColorTable.ToolstripButtonPressedBorder);
					}
					else if (tsBtn.Selected)
					{
						Rectangle rect = new Rectangle(Point.Empty, tsBtn.Size);

                        Image image = GetToolstripItemImageSelected(rect, focus);

						e.Graphics.DrawImage(image, Point.Empty);
					}

					if(tsBtn is OfficeButton)
						result = true;
				}

				return result;
			}

			private LinearGradientBrush GetBackgroundBrush(int top, int nHeight, bool bSelected)
			{
				Color clBegin = bSelected ? OfficeColorTable.ButtonSelectedGradientBegin : OfficeColorTable.ToolStripGradientBegin;
				Color clEnd = bSelected ? OfficeColorTable.ButtonSelectedGradientEnd : OfficeColorTable.ToolStripGradientEnd;

				LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, top, 1, nHeight), clBegin, clEnd, LinearGradientMode.Vertical);

				return brush;
			}

			private Bitmap GetMenuButtonImage(Rectangle rectangle, ButtonState state)
			{
				Bitmap bmp = new Bitmap(rectangle.Width, rectangle.Height);

				Graphics g = Graphics.FromImage(bmp);

				Point[] polygon = RendererUtils.GetSquareRoundedToUpPolygon(rectangle, 2);

				Rectangle[] antializing = new Rectangle[] 
				{
				new Rectangle(0,1,rectangle.Width,rectangle.Height),
				new Rectangle(1,0,rectangle.Width-3,rectangle.Height)
				};

				Rectangle upperRect = rectangle;
				upperRect.X += 2; upperRect.Width -= 3;

				g.Clear(Color.Transparent);
				g.InterpolationMode = InterpolationMode.High;
				//  this.SelectionColors  
				Color color = state == ButtonState.Pushed  ? ControlPaint.LightLight (menuColor) : ControlPaint.Light(menuColor);
				using (Brush brush = new LinearGradientBrush(upperRect, MenuColor, color , LinearGradientMode.Vertical))
				{
					g.FillRectangle(brush, upperRect);
				}

				upperRect.Y += upperRect.Height;

			
				using (GraphicsPath ellipse = new GraphicsPath())
				{
					float offset = rectangle.Width / 5;
					ellipse.AddEllipse(new RectangleF(-offset / 2, 0, rectangle.Width + offset, 30));
					ellipse.Transform(new Matrix(1, 0, 0, 1, 0, (rectangle.Height - 8)));

					using (PathGradientBrush p = new PathGradientBrush(ellipse))
					{
                        //p.CenterColor =OfficeColorTable.MenuButtonGlowCenterColor;
                        //p.SurroundColors = new Color[] { OfficeColorTable.MenuButtonGlowSurroundColor };
                        //p.FocusScales = new PointF(0.6F, 0.1F);
                        //g.FillPath(p, ellipse);
					}
				}

                using (Pen pen = new Pen(MenuColor ))
				{
					g.DrawRectangles(pen, antializing);
				}

				if (state == ButtonState.Pushed)
				{
					using (Pen pen = new Pen(MenuColor))
					{
					//	g.DrawPolygon(pen, RendererUtils.GetSquareRoundedToUpPolygon(Rectangle.Inflate(rectangle, -1, -1), 2));
					}
				}

				using (Pen pen = new Pen(MenuColor ))
				{
					g.DrawPolygon(pen, polygon);
				}

				return bmp;
			}

            private Image GetToolstripItemImageSelected(Rectangle rect, bool focus)
            {
				Bitmap bmp = new Bitmap(rect.Width, rect.Height);
				rect.Width -= 1; rect.Height -= 1;
				Point[] polygon = RendererUtils.GetRoundedPolygon(rect, 2);

				Graphics g = Graphics.FromImage(bmp);
				g.Clear(Color.Transparent);
				g.InterpolationMode = InterpolationMode.NearestNeighbor;
                if (focus)
                {
                    using (SolidBrush brush = new SolidBrush(OfficeColorTable.ToolstripButtonSelectedBackground))
                    {
                        g.FillPolygon(brush, polygon);
                    }

                    using (Pen pen = new Pen(OfficeColorTable.ToolstripButtonSelectedBorder))
                    {
                        g.DrawPolygon(pen, polygon);
                    }

                    rect.Inflate(-1, -1);
                    polygon = RendererUtils.GetRoundedPolygon(rect, 2);

                    using (Pen pen = new Pen(OfficeColorTable.ToolstripButtonSelectedBottomCenterColor))
                    {
                        g.DrawPolygon(pen, polygon);
                    }

                    g.Clip = new Region(rect);

                    using (GraphicsPath ellipse = new GraphicsPath())
                    {
                        float offset = rect.Width / 5;
                        ellipse.AddEllipse(new RectangleF(-offset / 2, 0, rect.Width + offset, 30));
                        ellipse.Transform(new Matrix(1, 0, 0, 1, 0, (rect.Height - 15)));

                        using (PathGradientBrush p = new PathGradientBrush(ellipse))
                        {
                            p.CenterColor = OfficeColorTable.ToolstripButtonSelectedBottomCenterColor;
                            p.SurroundColors = new Color[] { Color.FromArgb(12, OfficeColorTable.ToolstripButtonSelectedBottomSurroundColors) };
                            p.FocusScales = new PointF(0.6F, 0.1F);
                            g.FillPath(p, ellipse);
                        }
                    }
                }
                g.Dispose();

                return bmp;
            }

			private bool PaintQuickItemsDropDownButtonBackground(ToolStripItemRenderEventArgs e)
			{
				bool result = false;
				if (e.Item is QuickItemsDropDownButton)
				{
					result = PaintOverflowButtonBackground(e);
				}
				return result;
			}

			private bool PaintDropDownMenuButtonBackground(ToolStripItemRenderEventArgs e)
			{
				bool result = false;

				ToolStripMenuButton button = e.Item as ToolStripMenuButton;

                if (button != null)
                {
                    GraphicsState gState = e.Graphics.Save();
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    if (!PaintMenuButtonBackgroundPressed(e, button))
                    {
                        if (!PaintMenuButtonBackgroundSelected(e, button))
                        {
                            result = PaintMenuButtonBackgroundNormal(e, button);
                        }
                    }

                    result = true;

                    e.Graphics.Restore(gState);
                }

				return result;
			}
            private Color menuColor = ColorTranslator.FromHtml("#0072C6");
            internal Color MenuColor
            {
                get 
                { 
                    return menuColor;
                }
                set 
                {
                    if (menuColor != value)
                        menuColor = value;
                }
            }
			private bool PaintMenuButtonBackgroundNormal(ToolStripItemRenderEventArgs e, ToolStripMenuButton button)
			{
				bool result = false;

				Rectangle rect = GetMenuButtonRectangle(button);

				Bitmap bmp = GetMenuButtonImage(rect, ButtonState.Normal);

				e.Graphics.DrawImage(bmp, Point.Empty);

				result = true;

				return result;
			}

			private Rectangle GetMenuButtonRectangle(ToolStripMenuButton button)
			{
				Rectangle rc = new Rectangle(Point.Empty, button.Size);

				return rc;
			}
			private Rectangle GetCheckRect(ToolStripItemImageRenderEventArgs e)
			{
				int IMAGE_MARGIN = 1, IMAGE_PADDING = 1;

				Rectangle rc = new Rectangle(e.ImageRectangle.Left - IMAGE_PADDING, IMAGE_MARGIN,
					e.ImageRectangle.Width + 2 * IMAGE_PADDING, e.Item.Height - 2 * IMAGE_MARGIN);

				return rc;
			}
			private bool PaintMenuButtonBackgroundSelected(ToolStripItemRenderEventArgs e, ToolStripMenuButton button)
			{
				bool result = false;

				if (button.Selected)
				{
					Rectangle rect = GetMenuButtonRectangle(button);
                    RibbonControlAdvHeader header = e.Item.Owner as RibbonControlAdvHeader;
                    Bitmap bmp;
                    if (header.m_owner.MenuButtonEnabled && ((e.ToolStrip.TopLevelControl!=null && e.ToolStrip.TopLevelControl.ContainsFocus)|| (e.ToolStrip.TopLevelControl == null)))
                    {
                        bmp = GetMenuButtonImage(rect, ButtonState.Pushed);
                        e.Graphics.DrawImage(bmp, Point.Empty);
                    }
                    else
                    {
                        bmp = GetMenuButtonImage(rect, ButtonState.Normal);
                        e.Graphics.DrawImage(bmp, Point.Empty);
                    }
					result = true;
				}

				return result;
			}

			private bool PaintImageMargin(ToolStripRenderEventArgs e)
			{
				Graphics g = e.Graphics;
				Rectangle rc = Rectangle.Inflate(e.AffectedBounds, -1, -2);

				if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
				{
					rc.X = e.ToolStrip.ClientRectangle.Width - rc.Width;
				}

				if (e.ToolStrip is ContextMenuStripEx)
				{
					ContextMenuStripEx statusStrip = e.ToolStrip as ContextMenuStripEx;

					rc.Y += statusStrip.TitleHeight;
					rc.Height -= statusStrip.TitleHeight;
				}

				using (Brush brush = new SolidBrush(ColorTable.ImageMarginGradientBegin))
				{
					g.FillRectangle(brush, rc);
				}

				int beginX = (e.ToolStrip.RightToLeft == RightToLeft.Yes) ? (rc.Right) : (rc.Left);
				int endX = (e.ToolStrip.RightToLeft == RightToLeft.Yes) ? (rc.Left) : (rc.Right);

				g.DrawLine(Pens.White, beginX, rc.Top, beginX, rc.Bottom - 1);
				using (Pen pen = new Pen(Color.LightGray ))
				{
					g.DrawLine(pen, endX, rc.Top, endX, rc.Bottom - 1);
				}

				return true;
			}

			private bool PaintMenuButtonBackgroundPressed(ToolStripItemRenderEventArgs e, ToolStripMenuButton button)
			{
				return false;
			}

			private bool PaintSplitButtonBackground(ToolStripItemRenderEventArgs e)
			{
                bool result = false;

                ToolStripSplitButton tsBtn = e.Item as ToolStripSplitButton;

                if (tsBtn != null && tsBtn.Enabled)
                {
                    Rectangle rcButton = new Rectangle(Point.Empty, new Size(tsBtn.ButtonBounds.Width, tsBtn.ButtonBounds.Height - 2));
                    Rectangle rcDropDown = new Rectangle(new Point(rcButton.Right, rcButton.Top), new Size(tsBtn.DropDownButtonBounds.Width, tsBtn.Bounds.Height - 2));

                    bool paintBorderPressed = tsBtn.ButtonPressed || tsBtn.DropDownButtonPressed;
                    bool paintBorderSelected = !paintBorderPressed && (tsBtn.DropDownButtonSelected || tsBtn.ButtonSelected);

                    GraphicsState state = e.Graphics.Save();

                    Rectangle rcBounds = new Rectangle(0, 0, tsBtn.Width - 2, tsBtn.Height - 2);

                    e.Graphics.SetClip(RendererUtils.GetRoundedRegion(rcBounds, 2), CombineMode.Intersect);

                    #region Paint Button Background

                    rcButton.X += 1; rcButton.Width -= 1;
                    if (tsBtn.ButtonPressed)
                    {
                        PaintSplitButtonBackgroundPressed(e.Graphics, rcButton);
                    }
                    else if (tsBtn.ButtonSelected || tsBtn.DropDownButtonPressed)
                    {
                        PaintSplitButtonBackgroundSelected(e.Graphics, rcButton);
                    }

                    #endregion

                    #region Paint DropDownButtonBackground

                    if (tsBtn.DropDownButtonPressed)
                    {
                        rcDropDown.X -= 1;
                        PaintSplitButtonBackgroundPressed(e.Graphics, rcDropDown);
                    }
                    else if (tsBtn.DropDownButtonSelected)
                    {
                        PaintSplitButtonBackgroundSelected(e.Graphics, rcDropDown);
                    }

                    #endregion

                    #region PaintBorder

                    if (paintBorderPressed)
                    {
                        rcBounds.X += 1; rcBounds.Width -= 1;
                        using (Pen pen = new Pen(OfficeColorTable.ToolstripButtonPressedBorder))
                        {
                            e.Graphics.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(rcBounds, 2));
                            e.Graphics.DrawLine(pen, rcDropDown.Location, new Point(rcDropDown.Left, rcDropDown.Bottom));
                        }
                    }
                    else if (paintBorderSelected)
                    {
                        rcBounds.X += 1; rcBounds.Width -= 1;

                        using (Pen pen = new Pen(OfficeColorTable.ToolstripButtonSelectedBottomCenterColor))
                        {
                            e.Graphics.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(Rectangle.Inflate(rcBounds, -1, -1), 2));
                            e.Graphics.DrawLine(pen, new Point(rcDropDown.Left - 1, rcDropDown.Top), new Point(rcDropDown.Left - 1, rcDropDown.Bottom));
                            e.Graphics.DrawLine(pen, new Point(rcDropDown.Left + 1, rcDropDown.Top), new Point(rcDropDown.Left + 1, rcDropDown.Bottom));
                        }

                        using (Pen pen = new Pen(OfficeColorTable.ToolstripButtonSelectedBorder))
                        {
                            e.Graphics.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(rcBounds, 2));
                            e.Graphics.DrawLine(pen, rcDropDown.Location, new Point(rcDropDown.Left, rcDropDown.Bottom));
                        }
                    }

                    #endregion

                    #region Paint Arrrow

                    Color color = e.Item.Enabled ? SystemColors.ControlText : SystemColors.ControlDark;
                    Rectangle rcArrow = Rectangle.Inflate(rcDropDown, -1, -1);

                    bool bVertical = tsBtn != null && (tsBtn.Dock == DockStyle.Left || tsBtn.Dock == DockStyle.Right);
                    ArrowDirection dir = bVertical ? ArrowDirection.Right : ArrowDirection.Down;

                    base.DrawArrow(new ToolStripArrowRenderEventArgs(e.Graphics, e.Item, rcArrow, color, dir));

                    #endregion

                    e.Graphics.Restore(state);

                    result = true;
                }

                return result;
            }

            private bool PaintSplitButtonBackgroundPressed(Graphics g, Rectangle rect)
            {
                using (Brush brush = new SolidBrush(OfficeColorTable.ToolstripButtonPressedBackground))
                {
                    g.FillRectangle(brush, rect);
                }

                return true;
            }

            private bool PaintSplitButtonBackgroundSelected(Graphics g, Rectangle rect)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.Empty, Color.Empty, LinearGradientMode.Vertical))
                {
                    this.SelectionBlend.Colors = this.SelectionColors;
                    brush.InterpolationColors = this.SelectionBlend;

                    g.FillRectangle(brush, rect);
                }

                return true;
            }

			public void DrawTabScrollButton(RibbonControlAdvHeader header, Graphics g, Rectangle rc, bool bRight)
			{
				if (rc.Width > 0 && rc.Height > 0)
				{
					bool bSelected = (bRight) ? header.RightScrollSelected : header.LeftScrollSelected;

					Rectangle rect = new Rectangle(rc.Left, rc.Top, rc.Width - 2, rc.Height);

					if (bSelected)
					{
						Rectangle rcTemp = Rectangle.Inflate(new Rectangle(Point.Empty,rect.Size), 2, 2);
                        g.DrawImage(GetToolstripItemImageSelected(rcTemp, true), rect.Location);
					}
					else
					{
						using (Brush brush = new LinearGradientBrush(rect, OfficeColorTable.TabScrollButtonGradientBegin, OfficeColorTable.TabScrollButtonGradientEnd, LinearGradientMode.Vertical))
						{
							g.FillRectangle(brush, rect);
						}
					}

					g.SmoothingMode = SmoothingMode.AntiAlias;

					using (Pen pen = new Pen(OfficeColorTable.TabScrollButtonBorder))
					{
						g.DrawRectangle(pen, rect);
					}

					Image imgArrow = bRight ? RightArrow : LeftArrow;

					int left = rc.Left + (rc.Width - ARROW_WIDTH) / 2;
					int top = rc.Top + (rc.Height - ARROW_HEIGHT) / 2;

					g.DrawImage(imgArrow, left, top);
				}
			}

			#endregion
		}
        internal class Office2013RibbonHeaderRenderer : System.Windows.Forms.ToolStripProfessionalRenderer
        {
            #region Ctor
            public Office2013RibbonHeaderRenderer(Office2010ColorTable colorTable)
                : base(colorTable)
            { }
            #endregion

            #region Fileds

            static int ARROW_WIDTH = 5;
            static int ARROW_HEIGHT = 6;
            Bitmap arrowOverflow, arrowRightImage, arrowDownImage, checkButton;
            private Bitmap rightArrow = null;
            private Bitmap leftArrow = null;
            Color[] selectionColors = null;
            ColorBlend selectionBlend = null;

            #endregion

            #region Properties

            internal Office2010ColorTable OfficeColorTable
            {
                get
                {
                    return base.ColorTable as Office2010ColorTable;
                }
            }

            protected Bitmap ArrowOverflow
            {
                get
                {
                    if (arrowOverflow == null)
                    {
                        arrowOverflow = new Bitmap(ARROW_HEIGHT + 1, ARROW_HEIGHT + 1);

                        using (Graphics g = Graphics.FromImage(arrowOverflow))
                        {
                            g.Clear(Color.Transparent);

                            g.SmoothingMode = SmoothingMode.AntiAlias;

                            // First arrow.
                            Rectangle rc = new Rectangle(0, 2, 1, 3);

                            using (Region rgDark = new Region(rc))
                            {
                                rgDark.Union(rc);

                                rc.Inflate(0, -1);
                                rc.X += 1;
                                rgDark.Union(rc);

                                g.FillRegion(SystemBrushes.ControlText, rgDark);
                            }

                            // Second arrow.
                            rc = new Rectangle(4, 2, 1, 3);
                            using (Region rgDark = new Region(rc))
                            {
                                rgDark.Union(rc);

                                rc.Inflate(0, -1);
                                rc.X += 1;
                                rgDark.Union(rc);

                                g.FillRegion(SystemBrushes.ControlText, rgDark);
                            }

                            // Highlight for first arrow.
                            rc = new Rectangle(0, 1, 1, 1);
                            using (Region rgLight = new Region(rc))
                            {
                                rgLight.Union(rc);
                                rc.X += 1;
                                rc.Y += 1;
                                rgLight.Union(rc);
                                rc.X += 1;
                                rc.Y += 1;
                                rgLight.Union(rc);
                                rc.X -= 1;
                                rc.Y += 1;
                                rgLight.Union(rc);
                                rc.X -= 1;
                                rc.Y += 1;
                                rgLight.Union(rc);

                                g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                            }

                            // Highlight for second arrow.
                            rc = new Rectangle(4, 1, 1, 1);
                            using (Region rgLight = new Region(rc))
                            {
                                rgLight.Union(rc);
                                rc.X += 1;
                                rc.Y += 1;
                                rgLight.Union(rc);
                                rc.X += 1;
                                rc.Y += 1;
                                rgLight.Union(rc);
                                rc.X -= 1;
                                rc.Y += 1;
                                rgLight.Union(rc);
                                rc.X -= 1;
                                rc.Y += 1;
                                rgLight.Union(rc);

                                g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                            }
                        }
                    }
                    return arrowOverflow;
                }
            }

            protected Bitmap ArrowRightImage
            {
                get
                {
                    if (arrowRightImage == null)
                    {
                        arrowRightImage = new Bitmap(ARROW_HEIGHT + 1, ARROW_WIDTH);
                        using (Graphics g = Graphics.FromImage(arrowRightImage))
                        {
                            g.Clear(Color.Transparent);

                            Rectangle rc = new Rectangle(0, 0, 1, ARROW_WIDTH);
                            using (Region rgDark = new Region(rc))
                            {
                                rc.Offset(3, 0);
                                while (rc.Y < rc.Bottom && rc.X < ARROW_HEIGHT)
                                {
                                    rgDark.Union(rc);

                                    rc.Inflate(0, -1);
                                    rc.X += 1;
                                }

                                g.FillRegion(SystemBrushes.ControlText, rgDark);

                                using (Region rgLight = rgDark.Clone())
                                {
                                    rgLight.Translate(1, 0);
                                    rgLight.Exclude(rgDark);
                                    g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                                }
                            }
                        }
                    }
                    return arrowRightImage;
                }
            }
            private bool isEnable = true;
            internal bool IsEnable
            {
                get { return isEnable; }
                set
                {
                    isEnable = value;
                    arrowDownImage = null;
                }
            }

            protected Bitmap ArrowDownImage
            {
                get
                {
                    if (arrowDownImage == null)
                    {
                        arrowDownImage = new Bitmap(ARROW_HEIGHT + 1, ARROW_HEIGHT + 1);

                        using (Graphics g = Graphics.FromImage(arrowDownImage))
                        {
                            g.Clear(Color.Transparent);

                            Rectangle rc = new Rectangle(0, 0, ARROW_WIDTH, 1);
                            Pen pen = new Pen(Color.Black);
                            if (!IsEnable)
                                pen = new Pen(Color.Gray);
                            g.DrawLine(pen, new PointF(rc.X - 2, rc.Y), new PointF(rc.Width + 2, rc.Y));
                            using (Region rgDark = new Region(rc))
                            {
                                rc.Offset(1, 3);
                                while (rc.X < rc.Right && rc.Y < ARROW_HEIGHT)
                                {
                                    rgDark.Union(rc);

                                    rc.Inflate(-1, 0);
                                    rc.Y += 1;
                                }
                                SolidBrush brush = new SolidBrush(Color.Black);
                                if (!IsEnable)
                                    brush = new SolidBrush(Color.Gray);

                                g.FillRegion(brush, rgDark);
                                brush.Dispose();

                                //using (Region rgLight = rgDark.Clone())
                                //{
                                //    rgLight.Translate(0, 1);
                                //    rgLight.Exclude(rgDark);
                                //    g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                                //}
                            }
                        }

                    }
                    return arrowDownImage;
                }
            }

            protected Bitmap CheckButton
            {
                get
                {
                    if (checkButton == null)
                    {
                        Rectangle rcFlash = new Rectangle(0, 0, 10, 14);
                        checkButton = new Bitmap(rcFlash.Width, rcFlash.Height);

                        using (Graphics g = Graphics.FromImage(checkButton))
                        {
                            g.Clear(Color.Transparent);

                            Point[] points = new Point[]
						{
							new Point(1,8), 
							new Point(3,12),
							new Point(8,1)
						};

                            using(GraphicsPath path = new GraphicsPath())
                            path.AddLines(points);

                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.DrawLines(Pens.MidnightBlue, points);
                        }
                    }
                    return checkButton;
                }
            }

            protected Bitmap RightArrow
            {
                get
                {
                    if (rightArrow == null)
                    {
                        rightArrow = new Bitmap(ARROW_WIDTH, ARROW_HEIGHT);

                        using (Graphics g = Graphics.FromImage(rightArrow))
                        {
                            g.Clear(Color.Transparent);

                            Rectangle rcRightArrow = new Rectangle(0, 0, 1, ARROW_HEIGHT);

                            using (Region rg = new Region(rcRightArrow))
                            {
                                rg.Union(rcRightArrow);

                                rcRightArrow.Inflate(0, -1);
                                rcRightArrow.X += 1;
                                rg.Union(rcRightArrow);

                                rcRightArrow.Inflate(0, -1);
                                rcRightArrow.X += 1;
                                rg.Union(rcRightArrow);

                                using (Brush brush = new SolidBrush(Color.Black))
                                {
                                    g.FillRegion(brush, rg);
                                }
                            }
                        }
                    }

                    return rightArrow;
                }
            }

            protected Bitmap LeftArrow
            {
                get
                {
                    if (leftArrow == null)
                    {
                        leftArrow = new Bitmap(ARROW_WIDTH, ARROW_HEIGHT);

                        using (Graphics g = Graphics.FromImage(leftArrow))
                        {
                            g.Clear(Color.Transparent);

                            Rectangle rcLeftArrow = new Rectangle(0, 2, 1, 1);

                            using (Region rg = new Region(rcLeftArrow))
                            {
                                rg.Union(rcLeftArrow);

                                rcLeftArrow.Inflate(0, 1);
                                rcLeftArrow.X += 1;
                                rg.Union(rcLeftArrow);

                                rcLeftArrow.Inflate(0, 1);
                                rcLeftArrow.X += 1;
                                rg.Union(rcLeftArrow);

                                using (Brush brush = new SolidBrush(Color.Black))
                                {
                                    g.FillRegion(brush, rg);
                                }
                            }
                        }
                    }

                    return leftArrow;
                }
            }

            internal Color[] SelectionColors
            {
                get
                {
                    if (selectionColors == null)
                    {
                        selectionColors = new Color[] 
					{
						Color.FromArgb(255,226,119),
						Color.FromArgb(255,227,124),
						Color.FromArgb(255,229,133),
						Color.FromArgb(255,238,164),
						Color.FromArgb(255,244,192),
						Color.FromArgb(255,250,214),
						Color.FromArgb(255,252,224)
					};
                    }

                    return selectionColors;
                }
            }

            internal ColorBlend SelectionBlend
            {
                get
                {
                    if (selectionBlend == null)
                    {
                        selectionBlend = new ColorBlend(7);
                        selectionBlend.Positions = new float[]
					{
					   0F,
					   0.83F,
					   0.86F,
					   0.9F,
					   0.93F,
					   0.96F,
					   1F
					};
                    }

                    return selectionBlend;
                }

            }

            #endregion

            #region Overrides

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                if (!(e.Item is ToolStripMenuButton))
                    base.OnRenderArrow(e);
            }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                if (!SystemInfo.IsVisualStyleEnabled || !PaintButtonBackground(e))
                {
                    if (ToolStripRendererUtils.GetIsChecked(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
                    {
                        Rectangle rect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);
                        Color color = ControlPaint.LightLight(menuColor);
                        if (UseDefaultHighlightColor)
                            color = ColorTranslator.FromHtml("#cde6f7");
                        using (SolidBrush brush = new SolidBrush(color))
                        {
                            e.Graphics.FillRectangle(brush, rect);
                        }

                    }
                }
            }

            protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
            {
                if (!SystemInfo.IsVisualStyleEnabled || !PaintDropDownButtonBackground(e))
                {
                    base.OnRenderDropDownButtonBackground(e);
                }
            }

            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
                if (!SystemInfo.IsVisualStyleEnabled || !PaintImageMargin(e))
                {
                    base.OnRenderImageMargin(e);
                }
            }

            protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
            {
                base.OnRenderItemImage(e);
            }

            protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
            {
                if (!SystemInfo.IsVisualStyleEnabled || !PaintItemCheck(e))
                {
                    base.OnRenderItemCheck(e);
                }
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                if (!SystemInfo.IsVisualStyleEnabled || !PaintItemText(e))
                {
                    base.OnRenderItemText(e);
                }
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                base.OnRenderSeparator(e);
            }

            protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
            {
                if (!SystemInfo.IsVisualStyleEnabled || !PaintSplitButtonBackground(e))
                {
                    base.OnRenderSplitButtonBackground(e);
                }
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                if (!SystemInfo.IsVisualStyleEnabled || !PaintToolstripBackground(e))
                {
                    base.OnRenderToolStripBackground(e);
                }
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                if (!(e.ToolStrip is MenuDropDown) && !(e.ToolStrip is RibbonControlAdvHeader))
                {
                    if (e.ToolStrip.Width > 0 && e.ToolStrip.Height > 0)
                    {
                        Rectangle rc = new Rectangle(Point.Empty, new Size(e.ToolStrip.Size.Width -1 , e.ToolStrip.Height-1));
                        if (e.ToolStrip.IsDropDown)
                        {
                            using (Pen BorderPen = new Pen(Color.LightGray))
                            {
                                e.Graphics.DrawRectangle(BorderPen, rc);
                            }
                        }
                    }
                }
            }

            protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
            {
                if (!SystemInfo.IsVisualStyleEnabled || !PaintOverflowButtonBackground(e))
                {
                    base.OnRenderOverflowButtonBackground(e);
                }
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (!SystemInfo.IsVisualStyleEnabled || !PaintMenuItemBackground(e))
                {
                    base.OnRenderMenuItemBackground(e);
                }
            }

            #endregion

            #region Implementation

            private bool PaintMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                bool result = false;

                if (e.Item.IsOnDropDown)
                {
                    result = PaintDropDownMenuItemBackground(e);
                }
                else
                {
                    result = PaintToolStripMenuItemBackground(e);
                }
                return result;
            }

            private bool PaintToolStripMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                bool result = false;

                return result;
            }

            private bool PaintDropDownMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                bool result = false;

                ToolStripMenuItem tsItem = e.Item as ToolStripMenuItem;

                if ((tsItem != null && tsItem.Enabled) && tsItem.Selected || tsItem.Pressed)
                {
                    Image image = GetToolstripItemImageSelected(new Rectangle(Point.Empty, tsItem.Size), true);

                    e.Graphics.DrawImage(image, Point.Empty);

                    result = true;
                }

                return result;
            }

            public void DrawSeparators(RibbonControlAdvHeader header, Graphics g, Rectangle rc)
            {
                int iTop = header.TabItemsRectangle.Top + 2;
                int iBottom = header.TabItemsRectangle.Bottom;

                Rectangle rcBrush = new Rectangle(0, iTop, 2, iBottom);
                using (Brush brush = new LinearGradientBrush(rcBrush, OfficeColorTable.TabItemSeparatorGradientBegin, Color.Transparent, LinearGradientMode.Vertical))
                {
                    for (int i = 0, count = header.m_iSepatators.Count; i < count; i++)
                    {
                        rcBrush.X = header.m_iSepatators[i];

                        if (header.m_iSepatators[i] < rc.Right - (header.m_bIsRightScroll ? SCROLL_BUTTON_WIDTH : 0)
                            && header.m_iSepatators[i] > rc.Left + (header.m_bIsLeftScroll ? SCROLL_BUTTON_WIDTH : 0))
                        {
                            g.FillRectangle(brush, rcBrush);
                        }
                    }
                }
            }

            private bool PaintOverflowButtonBackground(ToolStripItemRenderEventArgs e)
            {
                RibbonControlAdvHeader.QuickItemsOverflowButton item = e.Item as RibbonControlAdvHeader.QuickItemsOverflowButton;

                if (item != null)
                {
                    Rectangle rc = Office12ToolStripRenderer.GetButtonRect(e.Item, new Rectangle(Point.Empty, e.Item.Size), Office12ToolStripRenderer.ERENDERTYPE.Normal);
                    if (rc.Width > 0 && rc.Height > 0)
                    {
                        ToolStrip ts = e.ToolStrip;
                        if (ts != null)
                        {
                            PaintButtonBackground(e);

                            Image imgArrow = ArrowOverflow;

                            int left = rc.X + (rc.Width - imgArrow.Size.Width) / 2;
                            int top = rc.Y + (rc.Height - imgArrow.Size.Height) / 2;

                            e.Graphics.DrawImage(imgArrow, left, top);
                        }
                    }
                }
                else
                {
                    Rectangle rc = Office12ToolStripRenderer.GetButtonRect(e.Item, new Rectangle(Point.Empty, e.Item.Size), Office12ToolStripRenderer.ERENDERTYPE.Normal);
                    if (rc.Width > 0 && rc.Height > 0)
                    {
                        ToolStrip ts = e.ToolStrip;
                        if (ts != null)
                        {
                            PaintButtonBackground(e);

                            bool bVertical = ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right;
                            Image imgArrow = bVertical ? ArrowRightImage : ArrowDownImage;

                            int left = rc.X + (rc.Width - imgArrow.Size.Width) / 2;
                            int top = rc.Y + (rc.Height - imgArrow.Size.Height) / 2;

                            e.Graphics.DrawImage(imgArrow, left, top);
                        }
                    }
                }

                return true;
            }

            private bool PaintItemCheck(ToolStripItemImageRenderEventArgs e)
            {
                ToolStripItem tsItem = e.Item;

                if (tsItem.Image == null || ToolStripRendererUtils.IsCheckMarginVisible(tsItem))
                {

                    Rectangle rc = GetCheckRect(e);
                    if (rc.Width > 0 && rc.Height > 0)
                    {
                        Size szImage = this.CheckButton.Size;

                        int iImageX = rc.X + (rc.Width - szImage.Width) / 2;
                        int iImageY = rc.Y + (rc.Height - szImage.Height) / 2;

                        e.Graphics.DrawImage(this.CheckButton, new Point(iImageX, iImageY));
                    }
                }

                return true;
            }

            private void PaintItemCheckBackground(ToolStripItemImageRenderEventArgs e)
            {
                Rectangle rc = GetCheckRect(e);

                if (rc.Width > 0 && rc.Height > 0)
                {
                    Graphics g = e.Graphics;

                    GraphicsState gState = g.Save();
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    using (Brush brush = new SolidBrush(this.OfficeColorTable.ToolstripButtonCheckedBackground))
                    {
                        g.FillRectangle(brush, Rectangle.Inflate(rc, -1, -2));
                    }
                    using (Pen pen = new Pen(this.OfficeColorTable.ToolstripButtonPressedBorder))
                    {
                        g.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(rc, 1));
                    }

                    g.Restore(gState);
                }
            }

            private bool PaintItemText(ToolStripItemTextRenderEventArgs e)
            {
                bool bResult = PaintMenuButtonText(e) || PaintToolStripTabItemText(e);

                return bResult;
            }

            private bool PaintMenuButtonText(ToolStripItemTextRenderEventArgs e)
            {
                bool result = false;

                ToolStripMenuButton button = e.Item as ToolStripMenuButton;
                RibbonControlAdvHeader header = e.Item.Owner as RibbonControlAdvHeader;
                if (button != null && header != null)
                {
                    if (header.Form != null && DwmAPI.IsCompositionEnabled)
                    {
                        RibbonControlAdvHeaderRenderer.DrawThemeText(e.Graphics, e.ToolStrip.Handle, e.TextRectangle, e.Text, e.TextFont, Color.White, false);
                        result = true;
                    }
                    else
                    {
                        if (header.m_owner.MenuButtonEnabled)
                            e.Graphics.DrawString(e.Text, e.TextFont, new SolidBrush(Color.White), e.TextRectangle.Location);
                        else
                            e.Graphics.DrawString(e.Text, e.TextFont, new SolidBrush(Color.LightGray), e.TextRectangle.Location);
                        result = true;
                    }
                }

                return result;
            }

            private bool PaintToolStripTabItemText(ToolStripItemTextRenderEventArgs e)
            {
                bool bResult = false;

                ToolStripTabItem item = e.Item as ToolStripTabItem;
                if (item != null)
                {
                    Rectangle rc = ToolStripRendererUtils.GetTextRect(e);
                    TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix;

                    RibbonControlAdvHeader header = e.Item.Owner as RibbonControlAdvHeader;

                    if (header != null)
                    {
                        Rectangle rcItemBounds = new Rectangle(new Point(e.Item.Bounds.Left + rc.X, e.Item.Bounds.Top + rc.Y), rc.Size);
                        Rectangle rcIntersection = Rectangle.Intersect(header.TabItemsRectangle, rcItemBounds);

                        int iTextWidthDifference = rcItemBounds.Width - rcIntersection.Width;
                        rc.Width -= iTextWidthDifference;

                        if (rcItemBounds.Left < header.TabItemsRectangle.Left && rcIntersection.Width > 0)
                        {
                            int iLeft = rcItemBounds.Width - rcIntersection.Width;

                            if (header.m_bIsLeftScroll)
                            {
                                iLeft += SCROLL_BUTTON_WIDTH;
                                rc.Width -= SCROLL_BUTTON_WIDTH;
                            }

                            rc.X += iLeft;
                            rc.Width -= 2;

                            flags |= TextFormatFlags.Right;
                        }
                        else
                        {
                            flags |= TextFormatFlags.Left;
                            rc.Width -= 2;
                        }
                    }

                    RibbonForm form = (e.ToolStrip as RibbonControlAdvHeader).Form;
                    Color clText = item.Checked && !this.IsPanelHidden(item) ? MenuColor : ColorTranslator.FromHtml("#22222");
                    if (tsactiveTabItem == item)
                    {
                        clText = MenuColor;

                    }

                    if (form != null && form.CompositionEnabled)
                    {
                        RibbonControlAdvHeaderRenderer.DrawThemeText(e.Graphics, e.ToolStrip.Handle, rc, e.Text, e.TextFont, clText, false);
                    }
                    else
                    {
                        TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, rc, clText, flags);
                    }

                    bResult = true;
                }

                return bResult;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            private bool IsPanelHidden(ToolStripItem item)
            {
                bool bResult = false;

                RibbonControlAdvHeader header = item.Owner as RibbonControlAdvHeader;
                if (header != null)
                {
                    RibbonControlAdv ribbon = header.Parent as RibbonControlAdv;
                    if (ribbon != null)
                    {
                        bResult = (ribbon.MinimizePanel || !ribbon.VisiblePanel);
                    }
                }

                return bResult;
            }

            private bool PaintButtonBackground(ToolStripItemRenderEventArgs e)
            {
                bool bResult = PaintSystemButtonBackground(e) ||
                    PaintTabItemBackGround(e) ||
                    PaintButtonBackGround(e) ||
                    PaintButtonOnSystemPanelBackGround(e);

                return bResult;
            }

            private bool PaintButtonOnSystemPanelBackGround(ToolStripItemRenderEventArgs e)
            {
                // Paint Buttons on BackStage, Option, Exit Button in One Note.
                return false;
            }

            private bool PaintSystemButtonBackground(ToolStripItemRenderEventArgs e)
            {
                if (e.Item is SystemButton && !(e.Item is HelpSystemButton))
                {
                    if (!PaintSystemButtonPressed(e))
                    {
                        PaintSystemButtonSelected(e);
                    }
                    return true;
                }
                return false;
            }

            private bool PaintSystemButtonSelected(ToolStripItemRenderEventArgs e)
            {
                SystemButton sysButton = e.Item as SystemButton;

                if (sysButton != null && sysButton.Selected && (e.Item.GetCurrentParent().FindForm().ContainsFocus))
                {
                    Rectangle rect = new Rectangle(Point.Empty, sysButton.Size);

                    Point[] polygon = RendererUtils.GetRoundedPolygon(rect, 2);

                    Color gradientBegin, gradientEnd, border;

                    if (sysButton.SysCommand == (int)SystemCommand.SC_CLOSE)
                    {
                        if(UseDefaultHighlightColor)
                            gradientEnd = gradientBegin = ColorTranslator.FromHtml("#cde6f7");
                        else
                            gradientEnd = gradientBegin = ControlPaint.LightLight(MenuColor);
                        border = OfficeColorTable.SystemCloseButtonBorder;
                    }
                    else
                    {
                        if (UseDefaultHighlightColor)
                            gradientEnd = gradientBegin = ColorTranslator.FromHtml("#cde6f7");
                        else
                            gradientEnd = gradientBegin = ControlPaint.LightLight(MenuColor);
                        border = OfficeColorTable.SystemButtonBorder;
                    }
                    Color color;
                    if (UseDefaultHighlightColor)
                        color = ColorTranslator.FromHtml("#cde6f7");
                    else
                       color = ControlPaint.LightLight(MenuColor);
                    using(Brush brush =new SolidBrush(color))
                        e.Graphics.FillRectangle(brush, rect);

                    return true;
                }

                return false;
            }

            private void PaintButtonBorder(ToolStripItemRenderEventArgs e, Point[] polygon, Color border)
            {
                using (Pen pen = new Pen(border))
                {
                    e.Graphics.DrawPolygon(pen, polygon);
                }
            }

            private void PaintSystemButtonGradient(ToolStripItemRenderEventArgs e, Rectangle rect, Point[] polygon, Color gradientBegin, Color gradientEnd)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, gradientBegin, gradientEnd, LinearGradientMode.Vertical))
                {
                    e.Graphics.FillPolygon(brush, polygon);
                }
            }

            private bool PaintSystemButtonPressed(ToolStripItemRenderEventArgs e)
            {
                SystemButton sysButton = e.Item as SystemButton;

                if (sysButton != null && sysButton.Pressed)
                {
                    Rectangle rect = new Rectangle(Point.Empty, sysButton.Size);

                    Point[] polygon = RendererUtils.GetRoundedPolygon(rect, 2);

                    Color background, border;

                    if (sysButton.SysCommand == (int)SystemCommand.SC_CLOSE)
                    {
                        if(UseDefaultHighlightColor)
                            background = ColorTranslator.FromHtml("#92c0e0");
                        else
                            background = ControlPaint.LightLight(MenuColor);
                        border = OfficeColorTable.SystemCloseButtonBorder;
                    }
                    else
                    {
                        if (UseDefaultHighlightColor)
                            background = ColorTranslator.FromHtml("#92c0e0");
                        else
                            background = ControlPaint.LightLight(MenuColor);
                        border = OfficeColorTable.SystemButtonBorder;
                    }
                    Color color;
                    if (UseDefaultHighlightColor)
                        color = ColorTranslator.FromHtml("#92c0e0");
                    else
                        color = ControlPaint.LightLight(MenuColor);
                    using(Brush brush =new SolidBrush(color))
                        e.Graphics.FillRectangle(brush, rect);

                    return true;
                }

                return false;
            }

            private bool PaintButtonPressedGradient(ToolStripItemRenderEventArgs e, Point[] polygon, Color background, Color border)
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(polygon);

                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.SurroundColors = new Color[] { border };
                        brush.CenterColor = background;
                        brush.FocusScales = new PointF(0.9F, 0.9F);

                        e.Graphics.FillPolygon(brush, polygon);
                    }

                }

                return true;
            }

            private bool PaintTabItemBackGround(ToolStripItemRenderEventArgs e)
            {
                if (e.Item is ToolStripTabItem)
                {
                    if (!PaintTabItemChecked(e))
                    {
                        PaintTabItemSelected(e);
                    }

                    return true;
                }
                return false;
            }
            ToolStripTabItem tsactiveTabItem = null;
            private bool PaintTabItemSelected(ToolStripItemRenderEventArgs e)
            {
                bool result = false;

                ToolStripTabItem tsTabItem = e.Item as ToolStripTabItem;

                RibbonControlAdvHeader header = (tsTabItem.Owner as RibbonControlAdvHeader);
                RibbonControlAdv ribbon = header.Parent as RibbonControlAdv;
                RibbonForm form = (header != null) ? header.Form : null;

                bool isActive = (form == null) || (form.ActiveState);

                if (tsTabItem != null && isActive && (tsTabItem.Selected || tsTabItem.Panel.Visible))
                {
                    tsactiveTabItem = tsTabItem;
                    Rectangle rect = new Rectangle(Point.Empty, tsTabItem.Size);
                    Color clrGroup = Color.Empty;
                    Brush br = new SolidBrush(Color.White);
                    Color clr = Color.Empty;
                    ToolStripTabGroup group = (header != null) ? header.GetItemGroup(e.Item) : null;

                    Point[] polygon = RendererUtils.GetSquareRoundedToUpPolygon(rect, 0);

                    Rectangle innerRect = new Rectangle(1, 1, rect.Width - 3, rect.Height);

                    Rectangle upperRect = new Rectangle(2, 2, innerRect.Width, innerRect.Height / 3);

                    if (ribbon.Office2013ColorScheme == Office2013ColorScheme.DarkGray)
                    {
                        clr = ColorTranslator.FromHtml("#F0F0F0");
                        using (br= new SolidBrush(clr))
                        {
                            e.Graphics.FillRectangle(br, innerRect);
                        }
                    }
                    else if (ribbon.Office2013ColorScheme == Office2013ColorScheme.LightGray)
                    {
                        clr = ColorTranslator.FromHtml("#F8F8F8");
                        using (br = new SolidBrush(clr))
                        {
                            e.Graphics.FillRectangle(br, innerRect);
                        }
                    }
                    else if (ribbon.Office2013ColorScheme == Office2013ColorScheme.White)
                    {
                        e.Graphics.FillRectangle(br, innerRect);
                    }
                    clrGroup = (group != null) ? Color.FromArgb(100, group.Color) : OfficeColorTable.ToolstripTabItemSelectedGradientBegin;

                    if (ribbon.Office2013ColorScheme == Office2013ColorScheme.DarkGray)
                    {
                        clr = ColorTranslator.FromHtml("#F0F0F0");
                        using (br = new SolidBrush(clr))
                        {
                            e.Graphics.FillRectangle(br, upperRect);
                        }
                    }
                    else if (ribbon.Office2013ColorScheme == Office2013ColorScheme.LightGray)
                    {
                        clr = ColorTranslator.FromHtml("#F8F8F8");
                        using (br= new SolidBrush(clr))
                        {
                            e.Graphics.FillRectangle(br, upperRect);
                        }
                    }
                    else if (ribbon.Office2013ColorScheme == Office2013ColorScheme.White)
                    {
                            e.Graphics.FillRectangle(br, upperRect);
                    }
                    clrGroup = (group != null) ? clrGroup : OfficeColorTable.ToolstripTabItemAntialiasing;

                    using (Pen pen = new Pen(Color.White))
                    {
                        e.Graphics.DrawRectangle(pen, innerRect);
                    }
                    Color color = (group != null) ? group.Color : OfficeColorTable.ToolstripTabItemBorder;
                    header.m_owner.ActiveTabGroupColor = color;
                    if (header.m_owner.PopupVisible)
                    {
                        e.Graphics.ExcludeClip(new Rectangle(rect.Left + 1, rect.Height - 1, rect.Width - 2, 1));
                        PaintButtonBorder(e, polygon, color);
                    }
                    br.Dispose();
                    result = true;
                }
                if (tsactiveTabItem == tsTabItem && !tsTabItem.Selected)
                {
                    tsactiveTabItem = null;
                }
                return result;
            }

            private bool PaintTabItemChecked(ToolStripItemRenderEventArgs e)
            {
                bool result = false;

                ToolStripTabItem tsTabItem = e.Item as ToolStripTabItem;

                RibbonControlAdvHeader header = tsTabItem.Owner as RibbonControlAdvHeader;
                RibbonControlAdv ribbon = header.Parent as RibbonControlAdv;
                if (tsTabItem != null && header != null && tsTabItem.Checked && !header.m_owner.MinimizePanel)
                {
                    if (tsTabItem.Bounds.Right > header.TabItemsRectangle.Right
                        || tsTabItem.Bounds.Left < header.TabItemsRectangle.Left)
                        return true;

                    Rectangle rect = new Rectangle(Point.Empty, tsTabItem.Size);
                    Point[] polygon = RendererUtils.GetSquareRoundedToUpPolygon(rect, 0);
                    ToolStripTabGroup group = (header != null) ? header.GetItemGroup(e.Item) : null;
                    Brush br = new SolidBrush(Color.White);
                    Color clr = Color.Empty;
                    Rectangle innerRect = new Rectangle(1, 1, rect.Width - 2, rect.Height);

                    Color clrGroup = (group != null) ? Color.FromArgb(10, group.Color) : OfficeColorTable.ToolstripTabItemCheckedGradientBegin;

                    if (ribbon.Office2013ColorScheme == Office2013ColorScheme.DarkGray)
                    {
                        clr = ColorTranslator.FromHtml("#F0F0F0");
                        using (br= new SolidBrush(clr))
                        {
                            e.Graphics.FillRectangle(br, innerRect);
                        }
                    }
                    else if (ribbon.Office2013ColorScheme == Office2013ColorScheme.LightGray)
                    {
                        clr= ColorTranslator.FromHtml("#F8F8F8");
                        using (br= new SolidBrush(clr))
                        {
                            e.Graphics.FillRectangle(br, innerRect);
                        }
                    }
                    else if (ribbon.Office2013ColorScheme == Office2013ColorScheme.White)
                    {
                        e.Graphics.FillRectangle(br, innerRect);
                    }
                    br.Dispose();
                    Pen pen = new Pen(OfficeColorTable.ToolstripTabItemBorder);
                    pen.Dispose();
                    GraphicsState state = e.Graphics.Save();

                    e.Graphics.ExcludeClip(new Rectangle(rect.Left + 1, rect.Height - 1, rect.Width - 2, 1));
                    Color color = (group != null) ? group.Color : OfficeColorTable.ToolstripTabItemBorder;
                    (e.Item.Owner.Parent as RibbonControlAdv).ActiveTabGroupColor = color;
                    PaintButtonBorder(e, polygon, color);


                    result = true;
                }

                return result;
            }

            private bool PaintToolstripBackground(ToolStripRenderEventArgs e)
            {
                bool result = (PaintHeaderBackground(e) ||
                    PaintDropDownBackground(e) ||
                    PaintDropDownExBackground(e));

                return result;
            }

            private bool PaintDropDownExBackground(ToolStripRenderEventArgs e)
            {
                bool result = false;

                RECT rect = new RECT();

                ContextMenuStripEx menuStrip = e.ToolStrip as ContextMenuStripEx;

                if (menuStrip != null)
                {
                    if (WindowsAPI.GetWindowRect(menuStrip.Handle, ref rect))
                    {
                        if (rect.Width > 0 && rect.Height > 0)
                        {
                            Graphics g = e.Graphics;

                            Rectangle rc = new Rectangle(Point.Empty, rect.Size);

                            Rectangle rectangleTitle = new Rectangle(0, 0, rc.Width, menuStrip.TitleHeight);
                            Rectangle rectangleBody = new Rectangle(0, menuStrip.TitleHeight, rc.Width, rc.Height - menuStrip.TitleHeight);

                            using (Brush brush = new SolidBrush(Color.White))
                            {
                                e.Graphics.FillRectangle(brush, rectangleBody);
                            }

                            using (Brush brush = new SolidBrush(ColorTranslator.FromHtml("#eeeeee")))
                            {
                                e.Graphics.FillRectangle(brush, rectangleTitle);
                            }


                            
                            Rectangle rcText = Rectangle.Inflate(rectangleTitle, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN);

                            TextFormatFlags flags = TextFormatFlags.EndEllipsis;

                            if (menuStrip.RightToLeft == RightToLeft.Yes)
                            {
                                flags |= TextFormatFlags.Right;
                            }
                            TextRenderer.DrawText(g, menuStrip.Text, menuStrip.HeaderFont, rcText, ColorTranslator.FromHtml("#777777"), flags);

                            result = true;
                        }
                    }
                }
                return result;
            }

            private bool PaintDropDownBackground(ToolStripRenderEventArgs e)
            {
                return false;
            }

            private bool PaintHeaderBackground(ToolStripRenderEventArgs e)
            {
                bool result = false;

                RibbonControlAdvHeader header = e.ToolStrip as RibbonControlAdvHeader;
                if (header != null)
                {
                    Graphics g = e.Graphics;
                    if (header.BackStageView != null && header.BackStageView.IsVisible)
                    {

                        GraphicsState state = g.Save();

                        Rectangle rcDisplay = header.DisplayRectangle;

                        Rectangle fullRect = new Rectangle(rcDisplay.X, rcDisplay.Y, header.Width + 20, header.Height + 20);

                        g.SetClip(fullRect, CombineMode.Replace);

                        RibbonForm form = header.Form;

                        bool bCompositionEnabled = (form != null) ? form.CompositionEnabled && header.m_owner.Dock == DockStyleEx.TopMost : false;

                        if (!bCompositionEnabled)
                        {
                            Color color = ((form != null && form.ActiveState)) ? OfficeColorTable.ActiveHeaderBackground : OfficeColorTable.InActiveHeaderBackground;

                            SolidBrush brush = new SolidBrush(Color.White);
                            fullRect.Width = header.BackStageView.BackStage.ItemSize.Width - 1;
                            fullRect.Height = 800;
                            g.FillRectangle(brush, fullRect);
                            brush.Dispose();
                            result = true;
                        }
                        else
                        {
                            DwmAPI.FillBlackRegion(g, fullRect);

                            Rectangle loweRect = new Rectangle(fullRect.X, fullRect.Y + fullRect.Height / 2, fullRect.Width, fullRect.Height / 2);
                                 
                            result = true;
                        }


                        string sTitle = header.Title;
                        if (sTitle != string.Empty)
                        {
                            Color titleColor = header.TitleColor == Color.Empty ? header.ForeColor : header.TitleColor;
                            Rectangle rcTitle = header.TitleRect;
                            if (form != null && form.CompositionEnabled)
                            {
                                rcTitle.Y += 10;
                                RibbonControlAdvHeaderRenderer.DrawThemeText(g, header.Handle, rcTitle, sTitle, header.TitleFont, titleColor, false);
                            }
                            else
                            {
                                rcTitle.Y += 5;
                                TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix;
                                TextRenderer.DrawText(g, sTitle, header.TitleFont, rcTitle, titleColor, flags);
                            }
                        }

                        PaintTabGroups(header, g);

                        g.Restore(state);




                    }
                    else
                    {

                        GraphicsState state = g.Save();
                        Color clr = Color.Empty;
                        Brush br = new SolidBrush(Color.White);
                        Rectangle rcDisplay = header.DisplayRectangle;

                        Rectangle fullRect = new Rectangle(rcDisplay.X, rcDisplay.Y, header.Width + 20, header.Height + 20);

                        g.SetClip(fullRect, CombineMode.Replace);

                        RibbonForm form = header.Form;

                        bool bCompositionEnabled = (form != null) ? form.CompositionEnabled && header.m_owner.Dock == DockStyleEx.TopMost : false;
                        Color color = ((form != null && form.ActiveState)) ? OfficeColorTable.ActiveHeaderBackground : OfficeColorTable.InActiveHeaderBackground;
                        RibbonControlAdv ribbon = header.Parent as RibbonControlAdv;
                        if (ribbon.Office2013ColorScheme == Office2013ColorScheme.DarkGray)
                        {
                            clr = ColorTranslator.FromHtml("#E0E0E0");
                            using (br = new SolidBrush(clr))
                            {
                                if ((ribbon.Header as RibbonControlAdvHeader).AutoHide && !ribbon.RibbonStatus)
                                {
                                    ribbon.NormalState = false;
                                    Font font = new System.Drawing.Font("Segoe UI", 12.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                                    if (ribbon.RightToLeft == RightToLeft.Yes)
                                    {
                                        int SubractValue = ribbon.HeaderInternal.Ribbon2013MinimizeButton.Width;
                                        int yLocation = 0;
                                        foreach (RibbonControlAdvHeader.SystemButton button in ribbon.HeaderInternal.SystemButtons)
                                        {
                                            if (button.Visible)
                                                SubractValue += button.Width;
                                        }
                                        if (ribbon.RibbonTouchModeEnabled)
                                            yLocation = 5;
                                        fullRect.X += SubractValue;
                                        br = new SolidBrush((ribbon.Header as RibbonControlAdvHeader).AutoHideMouseMoveColor);
                                        g.FillRectangle(br, fullRect);
                                        if ((ribbon.Header as RibbonControlAdvHeader).AutoHideMouseMoveColor != Color.White)
                                            g.DrawString(". . . ", font, new SolidBrush(Color.FromArgb(150, ribbon.MenuColor)), new PointF(SubractValue, yLocation));
                                        else
                                            g.DrawString(". . .", font, new SolidBrush(Color.Gray), new PointF(SubractValue, yLocation));
                                    }
                                    else
                                    {
                                        int BarWidth = 20;
                                        int SubractValue = ribbon.HeaderInternal.Ribbon2013MinimizeButton.Width + 30;
                                        int yLocation = 0;
                                        if (ribbon.RibbonTouchModeEnabled)
                                            yLocation = 5;
                                        foreach (RibbonControlAdvHeader.SystemButton button in ribbon.HeaderInternal.SystemButtons)
                                        {
                                            if (button.Visible)
                                                SubractValue += button.Width;
                                        }
                                        if (form != null && form.HelpButton)
                                        {
                                            SubractValue += ribbon.HeaderInternal.SystemButtonsWidth;
                                        }
                                        else
                                            fullRect.Width -= SubractValue;
                                        br = new SolidBrush((ribbon.Header as RibbonControlAdvHeader).AutoHideMouseMoveColor);
                                        g.FillRectangle(br, fullRect);
                                        if ((ribbon.Header as RibbonControlAdvHeader).AutoHideMouseMoveColor != Color.White)
                                            g.DrawString(". . . ", font, new SolidBrush(ColorTranslator.FromHtml("#2a8dd4")), new PointF(ribbon.Width - SubractValue - BarWidth, yLocation));
                                        else
                                            g.DrawString(". . .", font, new SolidBrush(ColorTranslator.FromHtml("#777777")), new PointF(ribbon.Width - SubractValue - BarWidth, yLocation));
                                    }
                                    font.Dispose();
                                }
                                else
                                    g.FillRectangle(br, fullRect);
                            }
                        }
                        else if (ribbon.Office2013ColorScheme == Office2013ColorScheme.LightGray)
                        {
                            clr = ColorTranslator.FromHtml("#F0F0F0");
                            using (br = new SolidBrush(clr))
                            {
                                if ((ribbon.Header as RibbonControlAdvHeader).AutoHide && !ribbon.RibbonStatus)
                                {
                                    ribbon.NormalState = false;
                                    Font font = new System.Drawing.Font("Segoe UI", 12.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                                    if (ribbon.RightToLeft == RightToLeft.Yes)
                                    {
                                        int SubractValue = ribbon.HeaderInternal.Ribbon2013MinimizeButton.Width;
                                        int yLocation = 0;
                                        foreach (RibbonControlAdvHeader.SystemButton button in ribbon.HeaderInternal.SystemButtons)
                                        {
                                            if (button.Visible)
                                                SubractValue += button.Width;
                                        }
                                        if (ribbon.RibbonTouchModeEnabled)
                                            yLocation = 5;
                                        fullRect.X += SubractValue;
                                        br = new SolidBrush((ribbon.Header as RibbonControlAdvHeader).AutoHideMouseMoveColor);
                                        g.FillRectangle(br, fullRect);
                                        if ((ribbon.Header as RibbonControlAdvHeader).AutoHideMouseMoveColor != Color.White)
                                            g.DrawString(". . . ", font, new SolidBrush(Color.FromArgb(150, ribbon.MenuColor)), new PointF(SubractValue, yLocation));
                                        else
                                            g.DrawString(". . .", font, new SolidBrush(Color.Gray), new PointF(SubractValue, yLocation));
                                    }
                                    else
                                    {
                                        int BarWidth = 20;
                                        int SubractValue = ribbon.HeaderInternal.Ribbon2013MinimizeButton.Width + 30;
                                        int yLocation = 0;
                                        if (ribbon.RibbonTouchModeEnabled)
                                            yLocation = 5;
                                        foreach (RibbonControlAdvHeader.SystemButton button in ribbon.HeaderInternal.SystemButtons)
                                        {
                                            if (button.Visible)
                                                SubractValue += button.Width;
                                        }
                                        if (form != null && form.HelpButton)
                                        {
                                            SubractValue += ribbon.HeaderInternal.SystemButtonsWidth;
                                        }
                                        else
                                            fullRect.Width -= SubractValue;
                                        br = new SolidBrush((ribbon.Header as RibbonControlAdvHeader).AutoHideMouseMoveColor);
                                        g.FillRectangle(br, fullRect);
                                        if ((ribbon.Header as RibbonControlAdvHeader).AutoHideMouseMoveColor != Color.White)
                                            g.DrawString(". . . ", font, new SolidBrush(ColorTranslator.FromHtml("#2a8dd4")), new PointF(ribbon.Width - SubractValue - BarWidth, yLocation));
                                        else
                                            g.DrawString(". . .", font, new SolidBrush(ColorTranslator.FromHtml("#777777")), new PointF(ribbon.Width - SubractValue - BarWidth, yLocation));
                                    }
                                    font.Dispose();
                                }
                                else
                                    g.FillRectangle(br, fullRect);
                            }
                        }
                        else if (ribbon.Office2013ColorScheme == Office2013ColorScheme.White)
                        {
                            if ((ribbon.Header as RibbonControlAdvHeader).AutoHide && !ribbon.RibbonStatus)
                            {
                                ribbon.NormalState = false;
                                Font font = new System.Drawing.Font("Segoe UI", 12.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                                g.FillRectangle(br, fullRect);
                                if (ribbon.RightToLeft == RightToLeft.Yes)
                                {
                                    int SubractValue = ribbon.HeaderInternal.Ribbon2013MinimizeButton.Width;
                                    int yLocation = 0;
                                    foreach (RibbonControlAdvHeader.SystemButton button in ribbon.HeaderInternal.SystemButtons)
                                    {
                                        if (button.Visible)
                                            SubractValue += button.Width;
                                    }
                                    if (ribbon.RibbonTouchModeEnabled)
                                        yLocation = 5;
                                    fullRect.X += SubractValue;
                                    br = new SolidBrush((ribbon.Header as RibbonControlAdvHeader).AutoHideMouseMoveColor);
                                    g.FillRectangle(br, fullRect);
                                    if ((ribbon.Header as RibbonControlAdvHeader).AutoHideMouseMoveColor != Color.White)
                                        g.DrawString(". . . ", font, new SolidBrush(Color.FromArgb(150, ribbon.MenuColor)), new PointF(SubractValue, yLocation));
                                    else
                                        g.DrawString(". . .", font, new SolidBrush(Color.Gray), new PointF(SubractValue, yLocation));
                                }
                                else
                                {
                                    int BarWidth = 20;
                                    int SubractValue = ribbon.HeaderInternal.Ribbon2013MinimizeButton.Width + 30;
                                    int yLocation = 0;
                                    if (ribbon.RibbonTouchModeEnabled)
                                        yLocation = 5;
                                    foreach (RibbonControlAdvHeader.SystemButton button in ribbon.HeaderInternal.SystemButtons)
                                    {
                                        if (button.Visible)
                                            SubractValue += button.Width;
                                    }
                                    if (form != null && form.HelpButton)
                                    {
                                        SubractValue += ribbon.HeaderInternal.SystemButtonsWidth;
                                    }
                                    else
                                        fullRect.Width -= SubractValue;
                                    br = new SolidBrush((ribbon.Header as RibbonControlAdvHeader).AutoHideMouseMoveColor);
                                    g.FillRectangle(br, fullRect);
                                    if ((ribbon.Header as RibbonControlAdvHeader).AutoHideMouseMoveColor != Color.White)
                                        g.DrawString(". . . ", font, new SolidBrush(ColorTranslator.FromHtml("#2a8dd4")), new PointF(ribbon.Width - SubractValue - BarWidth, yLocation));
                                    else
                                        g.DrawString(". . .", font, new SolidBrush(ColorTranslator.FromHtml("#777777")), new PointF(ribbon.Width - SubractValue - BarWidth, yLocation));
                                }
                                font.Dispose();
                            }
                            else
                                g.FillRectangle(br, fullRect);                            
                        }
                        result = true;
                        br.Dispose();
                        if ((header.Parent as RibbonControlAdv).RibbonHeaderImage != RibbonHeaderImage.None && (!header.AutoHide || ribbon.NormalState))
                        {
                            Image headerImage = null;

                            if ((header.Parent as RibbonControlAdv).RibbonHeaderImage == RibbonHeaderImage.Custom)
                            {
                                headerImage = (header.Parent as RibbonControlAdv).CustomRibbonHeaderImage;
                            }
                            else
                            {
                                headerImage = Image.FromStream(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources." + (header.Parent as RibbonControlAdv).RibbonHeaderImage + ".bmp"));
                                if (ribbon.Office2013ColorScheme == Office2013ColorScheme.White)
                                    headerImage = Image.FromStream(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources." + (header.Parent as RibbonControlAdv).RibbonHeaderImage + ".bmp"));

                                else if (ribbon.Office2013ColorScheme == Office2013ColorScheme.LightGray)
                                    headerImage = Image.FromStream(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources." + (header.Parent as RibbonControlAdv).RibbonHeaderImage + "l.bmp"));

                                else if (ribbon.Office2013ColorScheme == Office2013ColorScheme.DarkGray)
                                    headerImage = Image.FromStream(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources." + (header.Parent as RibbonControlAdv).RibbonHeaderImage + "d.bmp"));

                            }

                            if (headerImage != null)
                            {

                                if (e.Graphics.DpiX > 120)
                                {
                                    if (ribbon.RightToLeft == RightToLeft.Yes)
                                    {
                                        e.Graphics.ScaleTransform(1.5F, 1.4F);
                                        e.Graphics.DrawImage(headerImage, new Rectangle(0, header.Bounds.Y, headerImage.Size.Width, headerImage.Size.Height));
                                    }
                                    else
                                    {
                                        e.Graphics.ScaleTransform(1.5F, 1.4F);
                                        e.Graphics.DrawImage(headerImage, new Rectangle(header.Width - (int)headerImage.Size.Width - (int)header.Width / 3, header.Bounds.Y, headerImage.Size.Width, headerImage.Size.Height));
                                    }
                                }
                                else if (e.Graphics.DpiX > 96)
                                {
                                    if (ribbon.RightToLeft == RightToLeft.Yes)
                                    {
                                        e.Graphics.ScaleTransform(1.5F, 1.3F);
                                        e.Graphics.DrawImage(headerImage, new Rectangle(0, header.Bounds.Y, headerImage.Size.Width, headerImage.Size.Height));
                                    }
                                    else
                                    {
                                        e.Graphics.ScaleTransform(1.5F, 1.3F);
                                        e.Graphics.DrawImage(headerImage, new Rectangle(header.Width - (int)headerImage.Size.Width - (int)header.Width / 4, header.Bounds.Y, headerImage.Size.Width, headerImage.Size.Height));
                                    }
                                }
                                else
                                    if (ribbon.RightToLeft == RightToLeft.Yes)
                                    {
                                        e.Graphics.DrawImage(headerImage, new Rectangle(0, header.Bounds.Y, headerImage.Size.Width, headerImage.Size.Height));
                                    }
                                    else
                                    {
                                        e.Graphics.DrawImage(headerImage, new Rectangle(header.Width - (int)headerImage.Size.Width, header.Bounds.Y, headerImage.Size.Width, headerImage.Size.Height));
                                    }
                            }
                        }

                        string sTitle = header.Title;
                        if (sTitle != string.Empty)
                        {
                            Color titleColor = header.TitleColor ;
                            Rectangle rcTitle = header.TitleRect;
                            if (!header.AutoHide)
                            {
                                if (form != null)
                                {
                                    bool mdiactivate = false;
                                    foreach (Form mdi in form.MdiChildren)
                                    {
                                        if (mdi.IsHandleCreated)
                                        {
                                            mdiactivate = true;
                                        }
                                    }
                                    if (form.ContainsFocus || form.designmode || mdiactivate)
                                    {
                                        if (form.CompositionEnabled)
                                        {
                                            rcTitle.Y += 10;
                                            if (ribbon.RightToLeft != RightToLeft.Yes)
                                                RibbonControlAdvHeaderRenderer.DrawThemeText(g, header.Handle, rcTitle, sTitle, header.TitleFont, titleColor, false);
                                            else
                                            {
                                                TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                                                TextRenderer.DrawText(g, sTitle, header.TitleFont, header.DisplayRectangle, titleColor, flags);
                                            }
                                        }
                                        else
                                        {
                                            rcTitle.Y += 5;
                                            TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix;
                                            TextRenderer.DrawText(g, sTitle, header.TitleFont, rcTitle, titleColor, flags);
                                        }
                                    }
                                    else
                                    {
                                        if (form.CompositionEnabled)
                                        {
                                            rcTitle.Y += 10;
                                            if (ribbon.RightToLeft != RightToLeft.Yes)
                                                RibbonControlAdvHeaderRenderer.DrawThemeText(g, header.Handle, rcTitle, sTitle, header.TitleFont, ControlPaint.LightLight(titleColor), false);
                                            else
                                            {
                                                TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                                                TextRenderer.DrawText(g, sTitle, header.TitleFont, header.DisplayRectangle, ControlPaint.LightLight(titleColor), flags);
                                            }
                                        }
                                        else
                                        {
                                            rcTitle.Y += 5;
                                            TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix;
                                            TextRenderer.DrawText(g, sTitle, header.TitleFont, rcTitle, ControlPaint.LightLight(titleColor), flags);
                                        }
                                    }
                                }
                            }
                        }

                        PaintTabGroups(header, g);

                        g.Restore(state);
                        if (form != null && form.ShowIcon && form.Icon != null)
                        {
                            Rectangle rcIcon = Rectangle.Empty;

                            if (e.Graphics.DpiX > 120)
                            {
                                rcIcon = ribbon.RightToLeft == RightToLeft.Yes
                                        ? new Rectangle(rcDisplay.Right - 26, 10, 24, 24)
                                        : new Rectangle(8, 10, 24, 24);
                            }
                            else if (e.Graphics.DpiX > 96|| header.RibbonTouchModeEnabled)
                            {
                                rcIcon = ribbon.RightToLeft == RightToLeft.Yes
                                        ? new Rectangle(rcDisplay.Right - 26, 10, 20, 20)
                                        : new Rectangle(8, 10, 20, 20);
                            }
                            else
                            {
                                rcIcon = ribbon.RightToLeft == RightToLeft.Yes
                                             ? new Rectangle(rcDisplay.Right - 26, 10, 16, 16)
                                             : new Rectangle(8, 10, 16, 16);
                            }
                            e.Graphics.DrawIcon(form.Icon, rcIcon);
                        }
                    }
                }
                return result;
            }

            private bool PaintTabGroups(RibbonControlAdvHeader header, Graphics g)
            {
                if (header != null)
                {
                    foreach (ToolStripTabGroup group in header.Groups)
                    {
                        if (group.Visible)
                        {
                            for (int i = 0, count = group.BoundsList.Count; i < count; i++)
                            {
                                Rectangle rect = group.BoundsList[i];

                                using (Brush brush = new LinearGradientBrush(rect, Color.FromArgb(100, group.Color), Color.Transparent, LinearGradientMode.Vertical))
                                {
                                    g.FillRectangle(brush, rect);
                                }

                                using (Brush brush = new SolidBrush(group.Color))
                                {
                                    g.FillRectangle(brush, new Rectangle(rect.X, rect.Y, rect.Width, 4));
                                }

                                Rectangle fullRect = new Rectangle(rect.X, rect.Y, rect.Width, header.Height);

                                using (Brush brush = new LinearGradientBrush(fullRect, Color.FromArgb(250, group.Color), Color.Transparent, LinearGradientMode.Vertical))
                                {
                                    g.FillRectangle(brush, fullRect.X, fullRect.Y, 1, fullRect.Height);
                                    g.FillRectangle(brush, fullRect.Right - 1, fullRect.Y, 1, fullRect.Height);
                                }

                                Font font = new Font(group.Font, FontStyle.Bold);
                                if (header.Form != null && header.Form.CompositionEnabled)
                                {
                                    rect.Y += 5;
                                    RibbonControlAdvHeaderRenderer.DrawThemeText(g, header.Handle, rect, group.Name, font, OfficeColorTable.ToolstripTabItemForeColor, true);
                                }
                                else
                                {
                                    rect.Y -= 5;
                                    TextRenderer.DrawText(g, group.Name, font, rect, OfficeColorTable.ToolstripTabItemForeColor,
                                        TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                                }
                                font.Dispose();
                            }
                        }
                    }
                    return true;
                }
                return false;
            }

            private bool PaintQuickItemsBackgroundRightToLeft(Graphics g, RibbonControlAdvHeader header)
            {
                bool result = false;

                if (header.QuickPanelVisible)
                {
                    int height = header.QuickPanelHeight - 1;

                    if (height > 0)
                    {
                        Rectangle rcDisplay = header.DisplayRectangle;

                        Rectangle recQI = Rectangle.Empty;
                        recQI.X = (header.Form != null && header.Form.ShowIcon) ? DEF_ICON_SIZE + DEF_PADDING : DEF_PADDING;
                        recQI.Y = rcDisplay.Y + smQuickItems.Top + 9;
                        recQI.Width = header.QuickPanelWidth;
                        recQI.Height = header.QuickPanelHeight;

                        Rectangle rcSep = new Rectangle(rcDisplay.Right - recQI.X, recQI.Y + 1, QUICK_ITEM_SEPARATOR_WIDTH, QUICK_ITEM_SEPARATOR_HEIGHT);

                        PaintQuickItemSeparator(g, rcSep);

                        rcSep = new Rectangle(rcDisplay.Right - (recQI.Right - QUICK_ITEM_SEPARATOR_WIDTH), recQI.Y + 1,
                                              QUICK_ITEM_SEPARATOR_WIDTH, QUICK_ITEM_SEPARATOR_HEIGHT);

                        PaintQuickItemSeparator(g, rcSep);
                    }
                }

                return result;
            }

            private bool PaintQuickItemsBackgroundLeftToRight(Graphics g, RibbonControlAdvHeader header)
            {
                bool result = false;


                if (header.QuickPanelVisible)
                {
                    int height = header.QuickPanelHeight - 1;

                    if (height > 0)
                    {
                        Rectangle rcDisplay = header.DisplayRectangle;

                        Rectangle recQI = Rectangle.Empty;
                        recQI.X = (header.Form != null && header.Form.ShowIcon) ? DEF_ICON_SIZE + DEF_PADDING : DEF_PADDING;
                        recQI.Y = rcDisplay.Y + 9;
                        recQI.Width = header.QuickPanelWidth;
                        recQI.Height = header.QuickPanelHeight;

                        Rectangle rcSep = new Rectangle(recQI.X, recQI.Y + 1, QUICK_ITEM_SEPARATOR_WIDTH, QUICK_ITEM_SEPARATOR_HEIGHT);

                        PaintQuickItemSeparator(g, rcSep);

                        rcSep = new Rectangle(recQI.Right - QUICK_ITEM_SEPARATOR_WIDTH, recQI.Y + 1, QUICK_ITEM_SEPARATOR_WIDTH, QUICK_ITEM_SEPARATOR_HEIGHT);

                        PaintQuickItemSeparator(g, rcSep);
                    }
                }

                return result;
            }

            private Point[] PaintQuickItemSeparator(Graphics g, Rectangle bounds)
            {
                Point[] polygon = RendererUtils.GetRoundedPolygon(bounds, 1);
                bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);

                using (Pen pen = new Pen(Color.FromArgb(100, OfficeColorTable.QuickItemSeparatorBorder)))
                {
                    g.DrawRectangle(pen, bounds);
                }

                using (SolidBrush brush = new SolidBrush(OfficeColorTable.QuickItemSeparatorBackground))
                {
                    g.FillPolygon(brush, polygon);
                }

                using (Pen pen = new Pen(OfficeColorTable.QuickItemSeparatorBorder))
                {
                    g.DrawPolygon(pen, polygon);
                }
                return polygon;
            }

            private bool PaintDropDownButtonBackground(ToolStripItemRenderEventArgs e)
            {
                bool bResult = (PaintDropDownMenuButtonBackground(e) ||
                    PaintQuickItemsDropDownButtonBackground(e) ||
                    PaintButtonBackGround(e));

                return bResult;
            }

            internal bool PaintButtonBackGround(ToolStripItemRenderEventArgs e)
            {
                bool result = false;

                ToolStripItem tsBtn = e.Item as ToolStripButton;

                if (tsBtn == null)
                    tsBtn = e.Item as ToolStripDropDownButton;

                if (tsBtn != null)
                {
                    bool focus = true;
                    if (tsBtn.Pressed)
                    {
                        Rectangle rect = new Rectangle(Point.Empty, tsBtn.Size);

                        Image image = GetToolstripItemImageSelected(rect, focus);

                        e.Graphics.DrawImage(image, Point.Empty);
                       
                    }
                    else if (tsBtn.Selected)
                    {
                        if (e.Item.GetCurrentParent().FindForm() == null || (e.Item.GetCurrentParent().FindForm()!=null && e.Item.GetCurrentParent().FindForm().ContainsFocus))
                        {

                            Rectangle rect = new Rectangle(Point.Empty, tsBtn.Size);

                            Image image = GetToolstripItemImageSelected(rect, focus);

                            e.Graphics.DrawImage(image, Point.Empty);
                        }
                    }
                    else if ((tsBtn is QuickButtonReflectable) && (tsBtn as QuickButtonReflectable).CheckState == CheckState.Checked)
                    {
                        Rectangle rect = new Rectangle(Point.Empty, tsBtn.Size);

                        Image image = GetToolstripItemImageSelected(rect, focus);
                        e.Graphics.DrawImage(image, Point.Empty);
                    }
                    if (e.Item.Name == "Touch/Mouse Mode")
                    {
                        if ((e.Item.Owner as RibbonControlAdvHeader).dropDownSelected)
                        {
                            Rectangle rect = new Rectangle(Point.Empty, tsBtn.Size);

                            Image image = GetToolstripItemImageSelected(rect, focus);

                            e.Graphics.DrawImage(image, Point.Empty);
                        }
                    }
                    else if (e.Item is Ribbon2013MinimizeSystemButton)
                    {
                        if ((e.Item.GetCurrentParent() is RibbonControlAdvHeader) && (e.Item.GetCurrentParent() as RibbonControlAdvHeader).ribbonOptionDropDownSelected)
                        {
                            Rectangle rect = new Rectangle(Point.Empty, tsBtn.Size);

                            Image image = GetToolstripItemImageSelected(rect, focus);

                            e.Graphics.DrawImage(image, Point.Empty);
                        }
                    }
                    result = true;
                }

                return result;
            }

            private LinearGradientBrush GetBackgroundBrush(int top, int nHeight, bool bSelected)
            {
                Color clBegin = bSelected ? OfficeColorTable.ButtonSelectedGradientBegin : OfficeColorTable.ToolStripGradientBegin;
                Color clEnd = bSelected ? OfficeColorTable.ButtonSelectedGradientEnd : OfficeColorTable.ToolStripGradientEnd;

                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, top, 1, nHeight), clBegin, clEnd, LinearGradientMode.Vertical);

                return brush;
            }

            private Bitmap GetMenuButtonImage(Rectangle rectangle, ButtonState state)
            {
                Bitmap bmp = new Bitmap(rectangle.Width + 20, rectangle.Height);

                using (Graphics g = Graphics.FromImage(bmp))
                {

                    Point[] polygon = RendererUtils.GetSquareRoundedToUpPolygon(rectangle, 2);

                    //Rectangle[] antializing = new Rectangle[] 
                    //{
                    //new Rectangle(0,1,rectangle.Width,rectangle.Height),
                    //new Rectangle(1,0,rectangle.Width-3,rectangle.Height)
                    //};

                    Rectangle upperRect = rectangle;
                    upperRect.X += 2; upperRect.Width -= 3;
                    // menuColor = ColorTranslator.FromHtml("#0072C6");
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = InterpolationMode.High;
                    //  this.SelectionColors  
                    Color color;
                    if(UseDefaultHighlightColor)
                        color = state == ButtonState.Pushed ? ColorTranslator.FromHtml("#2a8ad4") : menuColor;
                    else
                        color = state == ButtonState.Pushed ? Color.FromArgb(200, menuColor) : menuColor;
                    using (Brush brush = new LinearGradientBrush(upperRect, color, color, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(brush, upperRect);
                    }

                    upperRect.Y += upperRect.Height;
                }

                return bmp;
            }

            private Image GetToolstripItemImageSelected(Rectangle rect, bool focus)
            {
                Bitmap bmp = new Bitmap(rect.Width, rect.Height);

                if (focus)
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.Transparent);
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        Color lightcolor;
                        if(UseDefaultHighlightColor)
                            lightcolor = ColorTranslator.FromHtml("#cde6f7");
                        else
                            lightcolor = ControlPaint.LightLight(ControlPaint.LightLight(MenuColor));
                        using (SolidBrush brush = new SolidBrush(lightcolor))
                        {
                            g.FillRectangle(brush, rect);
                        }
                    }
                }
               
                return bmp;
            }

            private bool PaintQuickItemsDropDownButtonBackground(ToolStripItemRenderEventArgs e)
            {
                bool result = false;
                if (e.Item is QuickItemsDropDownButton)
                {
                    result = PaintOverflowButtonBackground(e);
                }
                return result;
            }

            private bool PaintDropDownMenuButtonBackground(ToolStripItemRenderEventArgs e)
            {
                bool result = false;

                ToolStripMenuButton button = e.Item as ToolStripMenuButton;

                if (button != null)
                {
                    GraphicsState gState = e.Graphics.Save();
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    if (!PaintMenuButtonBackgroundPressed(e, button))
                    {
                        if (!PaintMenuButtonBackgroundSelected(e, button))
                        {
                            result = PaintMenuButtonBackgroundNormal(e, button);
                        }
                    }

                    result = true;

                    e.Graphics.Restore(gState);
                }

                return result;
            }
            private Color menuColor = ColorTranslator.FromHtml("#0072C6");
            internal Color MenuColor
            {
                get
                {
                    return menuColor;
                }
                set { menuColor = value; }
            }
            private bool useDefaultHighlightColor = true;
            internal bool UseDefaultHighlightColor
            {
                get
                {
                    return useDefaultHighlightColor;
                }
                set { useDefaultHighlightColor = value; }
            }
            private bool PaintMenuButtonBackgroundNormal(ToolStripItemRenderEventArgs e, ToolStripMenuButton button)
            {
                bool result = false;

                Rectangle rect = GetMenuButtonRectangle(button);

                Bitmap bmp = GetMenuButtonImage(rect, ButtonState.Normal);

                e.Graphics.DrawImage(bmp, Point.Empty);

                result = true;

                return result;
            }

            private Rectangle GetMenuButtonRectangle(ToolStripMenuButton button)
            {
                Rectangle rc = new Rectangle(Point.Empty, button.Size);

                return rc;
            }
            private Rectangle GetCheckRect(ToolStripItemImageRenderEventArgs e)
            {
                int IMAGE_MARGIN = 1, IMAGE_PADDING = 1;

                Rectangle rc = new Rectangle(e.ImageRectangle.Left - IMAGE_PADDING, IMAGE_MARGIN,
                    e.ImageRectangle.Width + 2 * IMAGE_PADDING, e.Item.Height - 2 * IMAGE_MARGIN);

                return rc;
            }
            private bool PaintMenuButtonBackgroundSelected(ToolStripItemRenderEventArgs e, ToolStripMenuButton button)
            {
                bool result = false;

                if (button.Selected && ((e.ToolStrip.TopLevelControl != null && e.ToolStrip.TopLevelControl.ContainsFocus) || (e.ToolStrip.TopLevelControl == null)))
                {
                    Rectangle rect = GetMenuButtonRectangle(button);
                    RibbonControlAdvHeader header = e.Item.Owner as RibbonControlAdvHeader;
                    Bitmap bmp;
                    if (header.m_owner.MenuButtonEnabled)
                    {
                         bmp= GetMenuButtonImage(rect, ButtonState.Pushed);
                         e.Graphics.DrawImage(bmp, Point.Empty);
                    }
                    else
                    {
                        bmp = GetMenuButtonImage(rect, ButtonState.Normal);
                        e.Graphics.DrawImage(bmp, Point.Empty);
                    }
                    result = true;
                }

                return result;
            }

            private bool PaintImageMargin(ToolStripRenderEventArgs e)
            {
                Graphics g = e.Graphics;
                Rectangle rc = Rectangle.Inflate(e.AffectedBounds, -1, -2);

                if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
                {
                    rc.X = e.ToolStrip.ClientRectangle.Width - rc.Width;
                }

                if (e.ToolStrip is ContextMenuStripEx)
                {
                    ContextMenuStripEx statusStrip = e.ToolStrip as ContextMenuStripEx;

                    rc.Y += statusStrip.TitleHeight;
                    rc.Height -= statusStrip.TitleHeight;
                }

                using (Brush brush = new SolidBrush(ColorTable.ImageMarginGradientBegin))
                {
                    g.FillRectangle(brush, rc);
                }

                int beginX = (e.ToolStrip.RightToLeft == RightToLeft.Yes) ? (rc.Right) : (rc.Left);
                int endX = (e.ToolStrip.RightToLeft == RightToLeft.Yes) ? (rc.Left) : (rc.Right);


                return true;
            }

            private bool PaintMenuButtonBackgroundPressed(ToolStripItemRenderEventArgs e, ToolStripMenuButton button)
            {
                return false;
            }

            private bool PaintSplitButtonBackground(ToolStripItemRenderEventArgs e)
            {
                bool result = false;

                ToolStripSplitButton tsBtn = e.Item as ToolStripSplitButton;

                if (tsBtn != null && tsBtn.Enabled)
                {
                    Rectangle rcButton = new Rectangle(Point.Empty, new Size(tsBtn.ButtonBounds.Width, tsBtn.ButtonBounds.Height - 2));
                    Rectangle rcDropDown = new Rectangle(new Point(rcButton.Right, rcButton.Top), new Size(tsBtn.DropDownButtonBounds.Width, tsBtn.Bounds.Height - 2));

                    bool paintBorderPressed = tsBtn.ButtonPressed || tsBtn.DropDownButtonPressed;
                    bool paintBorderSelected = !paintBorderPressed && (tsBtn.DropDownButtonSelected || tsBtn.ButtonSelected);

                    GraphicsState state = e.Graphics.Save();

                    Rectangle rcBounds = new Rectangle(0, 0, tsBtn.Width - 2, tsBtn.Height - 2);


                    #region Paint Button Background

                    rcButton.X += 1; rcButton.Width -= 1;
                    if (tsBtn.ButtonPressed)
                    {
                        PaintSplitButtonBackgroundPressed(e.Graphics, rcButton, e.Item);
                    }
                    else if (tsBtn.ButtonSelected || tsBtn.DropDownButtonPressed)
                    {
                        PaintSplitButtonBackgroundSelected(e.Graphics, rcButton, e.Item);
                    }

                    #endregion

                    #region Paint DropDownButtonBackground

                    if (tsBtn.DropDownButtonPressed)
                    {
                        rcDropDown.X -= 1;
                        PaintSplitButtonBackgroundPressed(e.Graphics, rcDropDown, e.Item);
                    }
                    else if (tsBtn.DropDownButtonSelected)
                    {
                        PaintSplitButtonBackgroundSelected(e.Graphics, rcDropDown, e.Item);
                    }

                    #endregion

                    #region Paint Arrrow

                    Color color = e.Item.Enabled ? SystemColors.ControlText : SystemColors.ControlDark;
                    Rectangle rcArrow = Rectangle.Inflate(rcDropDown, -1, -1);

                    bool bVertical = tsBtn != null && (tsBtn.Dock == DockStyle.Left || tsBtn.Dock == DockStyle.Right);
                    ArrowDirection dir = bVertical ? ArrowDirection.Right : ArrowDirection.Down;

                    base.DrawArrow(new ToolStripArrowRenderEventArgs(e.Graphics, e.Item, rcArrow, color, dir));

                    #endregion

                    e.Graphics.Restore(state);

                    result = true;
                }

                return result;
            }

            private bool PaintSplitButtonBackgroundPressed(Graphics g, Rectangle rect, ToolStripItem item)
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(120, (item.GetCurrentParent() as RibbonControlAdvHeader).MenuColor)))
                {
                    Color c = item.BackColor;
                    g.FillRectangle(brush, rect);
                }

                return true;
            }

            private bool PaintSplitButtonBackgroundSelected(Graphics g, Rectangle rect, ToolStripItem item)
            {
                if (UseDefaultHighlightColor)
                {
                    using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#cde6f7")))
                    {
                        g.FillRectangle(brush, rect);
                    }
                }
                else
                {
                    if (item.GetCurrentParent() is RibbonControlAdvHeader)
                    {
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(120, (item.GetCurrentParent() as RibbonControlAdvHeader).MenuColor)))
                        {
                            g.FillRectangle(brush, rect);
                        }
                    }
                    else if (item.Owner is RibbonControlAdvHeader)
                    {
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(120, (item.Owner as RibbonControlAdvHeader).MenuColor)))
                        {
                            g.FillRectangle(brush, rect);
                        }
                    }
                  
                }
                return true;
            }

            public void DrawTabScrollButton(RibbonControlAdvHeader header, Graphics g, Rectangle rc, bool bRight)
            {
                if (rc.Width > 0 && rc.Height > 0)
                {
                    bool bSelected = (bRight) ? header.RightScrollSelected : header.LeftScrollSelected;

                    Rectangle rect = new Rectangle(rc.Left, rc.Top, rc.Width - 2, rc.Height);

                    if (bSelected)
                    {
                        Rectangle rcTemp = Rectangle.Inflate(new Rectangle(Point.Empty, rect.Size), 2, 2);
                        g.DrawImage(GetToolstripItemImageSelected(rcTemp, true), rect.Location);
                    }
                    else
                    {
                        using (Brush brush = new LinearGradientBrush(rect, OfficeColorTable.TabScrollButtonGradientBegin, OfficeColorTable.TabScrollButtonGradientEnd, LinearGradientMode.Vertical))
                        {
                            g.FillRectangle(brush, rect);
                        }
                    }

                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    using (Pen pen = new Pen(OfficeColorTable.TabScrollButtonBorder))
                    {
                        g.DrawRectangle(pen, rect);
                    }

                    Image imgArrow = bRight ? RightArrow : LeftArrow;

                    int left = rc.Left + (rc.Width - ARROW_WIDTH) / 2;
                    int top = rc.Top + (rc.Height - ARROW_HEIGHT) / 2;

                    g.DrawImage(imgArrow, left, top);
                }
            }

            #endregion
        }
        
	}
}
#endif