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
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Tools
{
	public partial class Office12ToolStripRenderer
	{
		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderItemBackground( System.Windows.Forms.ToolStripItemRenderEventArgs e )
		{
			if( !SystemInfo.IsVisualStyleEnabled || !PaintGallery( e ) )
			{
				base.OnRenderItemBackground( e );
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		internal virtual bool PaintGallery( System.Windows.Forms.ToolStripItemRenderEventArgs e )
		{
			bool bResult = false;

			ToolStripGallery gallery = e.Item as ToolStripGallery;
			if( gallery != null )
			{
				Graphics g = e.Graphics;

				if( gallery.ShowCaption )
				{
					PaintGalleryCaption( g, gallery );
				}

				if( gallery.BorderStyle == ToolstripGalleryBorderStyle.Single )
				{
					using( Pen p = new Pen( Color.FromArgb( 40, Color.Black ) ) )
					{
						Rectangle galleryRect = new Rectangle( 0, 0, gallery.Width - 1, gallery.Height - 1 );
						g.SetClip( gallery.ScrollArea, CombineMode.Exclude );
						g.DrawRectangle( p, galleryRect );
						g.SetClip( gallery.ScrollArea, CombineMode.Union );
					}
				}

				PaintGalleryItems( g, gallery );

				switch( gallery.ScrollerType )
				{
					case ToolStripGalleryScrollerType.Compact:
						DrawCompactScrollers( g, gallery );
						break;

					case ToolStripGalleryScrollerType.Standard:
						DrawStandardScrollers( g, gallery );
						break;
				}

				bResult = true;
			}

			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="gallery"></param>
		protected virtual void DrawStandardScrollers( Graphics g, ToolStripGallery gallery )
		{
			Rectangle scrollRect = gallery.ScrollArea;
			if (scrollRect.Width > 0 && scrollRect.Height > 0)
			{
				using (LinearGradientBrush b = GetHorizontalBrush(ref scrollRect, Color.White, this.OfficeColorTable.GalleryScrollBarBackground))
				{
					b.Blend = m_blScrollerBackground;
					g.FillRectangle(b, scrollRect);
				}

				Rectangle rect = new Rectangle(gallery.ScrollUpButton.Bounds.Location,
					new Size(gallery.ScrollUpButton.Bounds.Width - 1, gallery.ScrollUpButton.Bounds.Height - 1));
				Rectangle imageRect = new Rectangle(rect.Left + rect.Width / 2 - 4, rect.Top + rect.Height / 2 - 1, 9, 5);
				PaintStandardScrollButtonBackground(g, rect, gallery.ScrollUpButton.State);
				if (gallery.ScrollUpButton.State == ScrollButtonState.Disabled)
				{
					PaintLargeUpArrow(g, imageRect, this.OfficeColorTable.ScrollButtonLargeArrowDisabled);
				}
				else
				{
					PaintLargeUpArrow(g, imageRect, this.OfficeColorTable.ScrollButtonLargeArrow);
				}

				rect = new Rectangle(gallery.ScrollDownButton.Bounds.Location,
					new Size(gallery.ScrollDownButton.Bounds.Width - 1, gallery.ScrollDownButton.Bounds.Height - 1));
				imageRect = new Rectangle(rect.Left + rect.Width / 2 - 4, rect.Top + rect.Height / 2 - 1, 9, 5);
				PaintStandardScrollButtonBackground(g, rect, gallery.ScrollDownButton.State);
				if (gallery.ScrollDownButton.State == ScrollButtonState.Disabled)
				{
					PaintLargeDownArrow(g, imageRect, this.OfficeColorTable.ScrollButtonLargeArrowDisabled);
				}
				else
				{
					PaintLargeDownArrow(g, imageRect, this.OfficeColorTable.ScrollButtonLargeArrow);
				}

				if (!gallery.Scroller.Bounds.IsEmpty)
				{
					SmoothingMode oldSmoothingMode = g.SmoothingMode;
					g.SmoothingMode = SmoothingMode.AntiAlias;
					rect = new Rectangle(
						gallery.Scroller.Bounds.Location, new Size(gallery.Scroller.Bounds.Width - 1, gallery.Scroller.Bounds.Height - 1));
					PaintScrollerBackground(g, rect, gallery.Scroller.State);

					if (rect.Height > 8)
					{
						imageRect = new Rectangle(rect.Left + rect.Width / 2 - 4, rect.Top + rect.Height / 2 - 4, 8, 8);
						PaintScrollerAdorning(g, imageRect);
					}

					g.SmoothingMode = oldSmoothingMode;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		/// <param name="scrollButtonState"></param>
		protected virtual void PaintScrollerBackground( Graphics g, Rectangle rect, ScrollButtonState scrollButtonState )
		{
			Color c1 = Color.Empty;
			Color c2 = Color.Empty;
			Color borderColor = Color.Empty;

			switch( scrollButtonState )
			{
				case ScrollButtonState.Normal:
				case ScrollButtonState.Highlighted:
					c1 = this.OfficeColorTable.ScrollerNormalGradientBegin;
					c2 = this.OfficeColorTable.ScrollerNormalGradientEnd;
					borderColor = this.OfficeColorTable.ScrollerNormalBorder;
					break;

				case ScrollButtonState.Selected:
					c1 = this.OfficeColorTable.ScrollerSelectedGradientBegin;
					c2 = this.OfficeColorTable.ScrollerSelectedGradientEnd;
					borderColor = this.OfficeColorTable.ScrollerSelectedBorder;
					break;

				case ScrollButtonState.Pressed:
					c1 = this.OfficeColorTable.ScrollerPressedGradientBegin;
					c2 = this.OfficeColorTable.ScrollerPressedGradientEnd;
					borderColor = this.OfficeColorTable.ScrollerPressedBorder;
					break;
			}

			using( LinearGradientBrush brush = GetHorizontalBrush( ref rect, c1, c2 ) )
			{
				brush.Blend = m_blScroller;
				g.FillRectangle( brush, rect );
			}

			using( Pen p = new Pen( borderColor ) )
			{
				rect.Height++;
				rect.Width++;
				g.DrawPolygon( p, RendererUtils.GetRoundedPolygon( rect, 1 ) );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		/// <param name="scrollButtonState"></param>
		protected virtual void PaintStandardScrollButtonBackground( Graphics g, Rectangle rect, ScrollButtonState scrollButtonState )
		{
			Color c1 = Color.Empty;
			Color c2 = Color.Empty;

			switch( scrollButtonState )
			{
				case ScrollButtonState.Highlighted:
					c1 = this.OfficeColorTable.StandardScrollButtonHighlightedGradientBegin;
					c2 = this.OfficeColorTable.StandardScrollButtonHighlightedGradientEnd;
					break;

				case ScrollButtonState.Pressed:
					c1 = this.OfficeColorTable.StandardScrollButtonPressedGradientBegin;
					c2 = this.OfficeColorTable.StandardScrollButtonPressedGradientEnd;
					break;

				case ScrollButtonState.Selected:
					c1 = this.OfficeColorTable.StandardScrollButtonSelectedGradientBegin;
					c2 = this.OfficeColorTable.StandardScrollButtonSelectedGradientEnd;
					break;
			}

			using( LinearGradientBrush brush = GetVerticalBrush( ref rect, c1, c2 ) )
			{
				brush.Blend = m_blStandardScrollButton;
				g.FillRectangle( brush, rect );
			}

			if( scrollButtonState != ScrollButtonState.Disabled && scrollButtonState != ScrollButtonState.Normal )
			{
				using( Pen p = new Pen( Color.FromArgb( 100, Color.Black ) ) )
				{
					g.DrawRectangle( p, rect );
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="gallery"></param>
		protected virtual void DrawCompactScrollers( Graphics g, ToolStripGallery gallery )
		{
			Rectangle rect = new Rectangle( gallery.ScrollUpButton.Bounds.Location,
				new Size( gallery.ScrollUpButton.Bounds.Width - 1, gallery.ScrollUpButton.Bounds.Height - 1 ) );
			Rectangle arrowRect = new Rectangle( rect.Left + rect.Width / 2 - 2, rect.Top + rect.Height / 2 - 1, 5, 3 );
			PaintScrollButtonBackground( g, rect, gallery.ScrollUpButton.State );
			if( gallery.ScrollUpButton.State == ScrollButtonState.Disabled )
			{
				PaintUpArrow( g, arrowRect, this.OfficeColorTable.ScrollButtonArrowDisabled );
			}
			else
			{
				arrowRect.Y--;
				PaintUpArrow( g, arrowRect, Color.FromArgb( 150, Color.White ) );
				arrowRect.Y++;
				PaintUpArrow( g, arrowRect, this.OfficeColorTable.ScrollButtonArrow );
			}

			rect = new Rectangle( gallery.ScrollDownButton.Bounds.Location,
				new Size( gallery.ScrollDownButton.Bounds.Width - 1, gallery.ScrollDownButton.Bounds.Height - 1 ) );
			arrowRect = new Rectangle( rect.Left + rect.Width / 2 - 2, rect.Top + rect.Height / 2 - 1, 5, 3 );
			PaintScrollButtonBackground( g, rect, gallery.ScrollDownButton.State );
			if( gallery.ScrollDownButton.State == ScrollButtonState.Disabled )
			{
				PaintDownArrow( g, arrowRect, this.OfficeColorTable.ScrollButtonArrowDisabled );
			}
			else
			{
				arrowRect.Y++;
				PaintDownArrow( g, arrowRect, Color.FromArgb( 150, Color.White ) );
				arrowRect.Y--;
				PaintDownArrow( g, arrowRect, this.OfficeColorTable.ScrollButtonArrow );
			}

			rect = new Rectangle( gallery.ScrollDropdownButton.Bounds.Location,
				new Size( gallery.ScrollDropdownButton.Bounds.Width - 1, gallery.ScrollDropdownButton.Bounds.Height - 1 ) );
			arrowRect = new Rectangle( rect.Left + rect.Width / 2 - 2, rect.Top + rect.Height / 2 - 3, 5, 6 );
			PaintScrollButtonBackground( g, rect, gallery.ScrollDropdownButton.State );
			arrowRect.Y++;
			PaintDropDownArrow( g, arrowRect, Color.FromArgb( 150, Color.White ) );
			arrowRect.Y--;
			PaintDropDownArrow( g, arrowRect, this.OfficeColorTable.ScrollButtonArrow );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		/// <param name="scrollButtonState"></param>
		protected virtual void PaintScrollButtonBackground( Graphics g, Rectangle rect, ScrollButtonState scrollButtonState )
		{
			Color c1 = Color.Empty;
			Color c2 = Color.Empty;

			switch( scrollButtonState )
			{
				case ScrollButtonState.Disabled:
					c1 = this.OfficeColorTable.DisabledButtonGradientBegin;
					c2 = this.OfficeColorTable.DisabledButtonGradientEnd;
					break;

				case ScrollButtonState.Normal:
					c1 = this.OfficeColorTable.ScrollButtonGradientBegin;
					c2 = this.OfficeColorTable.ScrollButtonGradientEnd;
					break;

				case ScrollButtonState.Pressed:
					c1 = this.OfficeColorTable.ButtonPressedGradientBegin;
					c2 = this.OfficeColorTable.ButtonPressedGradientEnd;
					break;

				case ScrollButtonState.Selected:
					c1 = this.OfficeColorTable.ButtonSelectedGradientBegin;
					c2 = this.OfficeColorTable.ButtonSelectedGradientEnd;
					break;
			}

			using( LinearGradientBrush brush = GetVerticalBrush( ref rect, c1, c2 ) )
			{
				brush.Blend = m_blScrollButton;
				g.FillRectangle( brush, rect );
			}

			using( Pen p = new Pen( Color.FromArgb( 25, Color.Black ) ) )
			{
				g.DrawRectangle( p, rect );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gallery"></param>
		/// <param name="g"></param>
		protected virtual void PaintGalleryItems( Graphics g, ToolStripGallery gallery )
		{
			GraphicsState oldState = g.Save();
			g.TranslateTransform( 0, -gallery.ScrollOffset );
			Rectangle workingRect = gallery.GetWorkingArea();
			workingRect.Y += gallery.ScrollOffset;
			g.SetClip( workingRect );
			foreach( ToolStripGalleryItemLayoutInfo itemInfo in gallery.ItemsInfo )
			{
				g.SetClip( itemInfo.Bounds, CombineMode.Intersect );
				Rectangle itemWorkingRect = gallery.GetItemWorkingRect( itemInfo );
				if( !gallery.ItemBackColor.IsEmpty )
				{
					g.SetClip( itemWorkingRect, CombineMode.Exclude );
				}

				Rectangle rc = new Rectangle( itemInfo.Bounds.Location, new Size( itemInfo.Bounds.Width - 1, itemInfo.Bounds.Height - 1 ) );
				Rectangle rcBackgr = new Rectangle( rc.Left + 1, rc.Top + 1, rc.Width - 2, rc.Height - 2 );

				ToolStripGalleryItem item = itemInfo.Item;

				if (item == gallery.PressedItem && item == gallery.SelectedItem)
				{
					Color cl1 = this.ColorTable.ButtonPressedGradientBegin;
					Color cl2 = this.ColorTable.ButtonPressedGradientEnd;

					PaintGradientSelected(g, rcBackgr, cl1, cl2);
					g.DrawImage(this.PressedFlashImage, rcBackgr.X, rcBackgr.Y + rcBackgr.Height / 2, rcBackgr.Width, rcBackgr.Height);
					PaintButtonPressedBorder(g, ref rc, this.ColorTable.ButtonPressedGradientEnd);
				}
				else if (item == gallery.CheckedItem)
				{
					Color cl1 = this.ColorTable.ButtonCheckedGradientBegin;
					Color cl2 = this.ColorTable.ButtonCheckedGradientEnd;

					PaintGradientSelected(g, rcBackgr, cl1, cl2);
					g.DrawImage(this.CheckedFlashImage, rcBackgr.X, rcBackgr.Y + rcBackgr.Height / 2, rcBackgr.Width, rcBackgr.Height);
					PaintButtonPressedBorder(g, ref rc, this.ColorTable.ButtonPressedGradientEnd);
				}
				else if (item == gallery.SelectedItem)
				{
					Color cl1 = this.OfficeColorTable.ButtonSelectedGradientBegin;
					Color cl2 = this.OfficeColorTable.ButtonSelectedGradientEnd;

					PaintGradientSelected(g, rcBackgr, cl1, cl2);
					g.DrawImage(this.SelectedFlashImage, rcBackgr.X, rcBackgr.Y + rcBackgr.Height / 2, rcBackgr.Width, rcBackgr.Height);
					PaintButtonBorder(g, rc, EBUTTONSTATE.Selected);
				}
              
				if( !gallery.ItemBackColor.IsEmpty )
				{
					g.SetClip( itemWorkingRect, CombineMode.Union );
					using( Brush br = new SolidBrush( gallery.ItemBackColor ) )
					{
						g.FillRectangle( br, itemWorkingRect );
					}
				}

				if( gallery.ItemDisplayStyle == ToolStripItemDisplayStyle.Image
					|| gallery.ItemDisplayStyle == ToolStripItemDisplayStyle.ImageAndText )
				{
					Image image = item.GetImage();
                    if (image != null)
                    {
                        if (item.Enabled)
                        {
                            g.DrawImage(image, itemInfo.ImageBounds);
                        }
                        else
                        {
                            ControlPaint.DrawImageDisabled(g, item.Image, itemInfo.ImageBounds.X, itemInfo.ImageBounds.Y, Color.Transparent);
                        }
                    }
                    
				}

				if( gallery.ItemDisplayStyle == ToolStripItemDisplayStyle.Text
					|| gallery.ItemDisplayStyle == ToolStripItemDisplayStyle.ImageAndText )
				{
					TextRenderer.DrawText(g, item.Text, gallery.Font, itemInfo.TextBounds, gallery.ForeColor, TextFormatFlags.EndEllipsis |
						TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.PreserveGraphicsTranslateTransform |
						TextFormatFlags.PreserveGraphicsClipping );
				}

				g.SetClip( workingRect, CombineMode.Replace );
			}
			g.Restore( oldState );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gallery"></param>
		/// <param name="g"></param>
		protected virtual void PaintGalleryCaption( Graphics g, ToolStripGallery gallery )
		{
			Rectangle rect = new Rectangle( 0, 0, gallery.Width, gallery.Height );
			using( Brush br = new SolidBrush( this.OfficeColorTable.ContextMenuTitle ) )
			{
				g.FillRectangle( br, new Rectangle( rect.Location, new Size( rect.Width, gallery.CaptionHeight ) ) );
			}

			using( Pen pen = new Pen( Color.FromArgb( 60, Color.Black ) ) )
			{
				g.DrawLine( pen, rect.Left, rect.Top + gallery.CaptionHeight, rect.Right, rect.Top + gallery.CaptionHeight );
			}

			Padding textMargin = ToolStripGallery.CAPTION_MARGIN;
			Rectangle textRect = new Rectangle( rect.Left + textMargin.Left, rect.Top + textMargin.Top,
				rect.Width - textMargin.Horizontal, gallery.CaptionHeight - textMargin.Vertical - 1 );
			TextRenderer.DrawText( g, gallery.CaptionText, gallery.Owner.Font, textRect, this.OfficeColorTable.RibbonTabText,
				TextFormatFlags.EndEllipsis | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		/// <param name="c"></param>
		protected virtual void PaintUpArrow( Graphics g, Rectangle rect, Color c )
		{
			using( Brush b = new SolidBrush( c ) )
			{
				GraphicsPath path = new GraphicsPath();
				path.AddLine( rect.Left - 1, rect.Bottom, rect.Left + 2, rect.Bottom - 4 );
				path.AddLine( rect.Left + 2, rect.Bottom - 4, rect.Left + 5, rect.Bottom );
				path.CloseFigure();
				g.FillPath( b, path );
				path.Dispose();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		/// <param name="c"></param>
		protected virtual void PaintLargeUpArrow( Graphics g, Rectangle rect, Color c )
		{
			using( Brush b = new SolidBrush( c ) )
			{
				GraphicsPath path = new GraphicsPath();
				path.AddLine( rect.Left - 1, rect.Bottom, rect.Left + 4, rect.Bottom - 6 );
				path.AddLine( rect.Left + 4, rect.Bottom - 6, rect.Left + 9, rect.Bottom );
				path.CloseFigure();
				g.FillPath( b, path );
				path.Dispose();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		/// <param name="c"></param>
		protected virtual void PaintDownArrow( Graphics g, Rectangle rect, Color c )
		{
			using( Brush b = new SolidBrush( c ) )
			{
				GraphicsPath path = new GraphicsPath();
				path.AddLine( rect.Left, rect.Top, rect.Left + 5, rect.Top );
				path.AddLine( rect.Left + 5, rect.Top, rect.Left + 2, rect.Top + 3 );
				path.CloseFigure();
				g.FillPath( b, path );
				path.Dispose();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		/// <param name="c"></param>
		protected virtual void PaintLargeDownArrow( Graphics g, Rectangle rect, Color c )
		{
			using( Brush b = new SolidBrush( c ) )
			{
				GraphicsPath path = new GraphicsPath();
				path.AddLine( rect.Left, rect.Top, rect.Left + 9, rect.Top );
				path.AddLine( rect.Left + 9, rect.Top, rect.Left + 4, rect.Top + 5 );
				path.CloseFigure();
				g.FillPath( b, path );
				path.Dispose();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		/// <param name="c"></param>
		protected virtual void PaintDropDownArrow( Graphics g, Rectangle rect, Color c )
		{
			using( Brush b = new SolidBrush( c ) )
			{
				using( Pen p = new Pen( c ) )
				{
					g.DrawLine( p, rect.Left, rect.Top, rect.Left + 4, rect.Top );
				}

				GraphicsPath path = new GraphicsPath();
				path.AddLine( rect.Left, rect.Top + 3, rect.Left + 5, rect.Top + 3 );
				path.AddLine( rect.Left + 5, rect.Top + 3, rect.Left + 2, rect.Top + 6 );
				path.CloseFigure();
				g.FillPath( b, path );
				path.Dispose();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rect"></param>
		protected virtual void PaintScrollerAdorning( Graphics g, Rectangle rect )
		{
			using( Pen darkPen = new Pen( Color.FromArgb( 100, Color.Black ) ) )
			{
				using( Pen lightPen = new Pen( Color.FromArgb( 100, Color.White ) ) )
				{
					for( int i = 0; i < 4; i++ )
					{
						int y = rect.Top + i * 2;
						g.DrawLine( darkPen, rect.Left, y, rect.Right, y );
						y++;
						g.DrawLine( lightPen, rect.Left + 2, y, rect.Right, y );
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		protected virtual bool PaintToolStripGalleryDropDownBackground( ToolStripRenderEventArgs e )
		{
			bool result = false;

			if( e.ToolStrip is ToolStripGalleryDropDown )
			{
				ToolStripGalleryDropDown dropDown = ( ToolStripGalleryDropDown )e.ToolStrip;
				if( dropDown.ShowGrip )
				{
					Rectangle rect = new Rectangle( 0, dropDown.Height - ToolStripGalleryDropDown.GALLERY_DRAGGER_HEIGHT,
						dropDown.Width, ToolStripGalleryDropDown.GALLERY_DRAGGER_HEIGHT );
					using (LinearGradientBrush b = GetVerticalBrush(ref rect, Color.White, this.OfficeColorTable.GripGradientEnd))
					{
						b.WrapMode = WrapMode.TileFlipXY;
						e.Graphics.FillRectangle( b, rect );
					}

					Point p = new Point( rect.Right - 4 , rect.Bottom - 3 );
					DrawCornerPoint( e.Graphics, p );
					p.Y -= 4;
					DrawCornerPoint( e.Graphics, p );
					p.Y += 4;
					p.X -= 4;
					DrawCornerPoint( e.Graphics, p );
					result = true;
				}
			}

			return result;
		}
		#endregion

		#region Private Methods
		
		private void DrawCornerPoint( Graphics g, Point point )
		{
			g.FillRectangle( Brushes.White, new Rectangle( point.X - 1, point.Y - 1, 2, 2 ) );
			g.FillRectangle( Brushes.DarkGray, new Rectangle( point.X - 2, point.Y - 2, 2, 2 ) );
		}
		#endregion
	}
    public partial class MetroToolStripRenderer
    {
        #region Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderItemBackground(System.Windows.Forms.ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintGallery(e))
            {
                base.OnRenderItemBackground(e);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal virtual bool PaintGallery(System.Windows.Forms.ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            ToolStripGallery gallery = e.Item as ToolStripGallery;
            if (gallery != null)
            {
                Graphics g = e.Graphics;

                if (gallery.ShowCaption)
                {
                    PaintGalleryCaption(g, gallery);
                }

                if (gallery.BorderStyle == ToolstripGalleryBorderStyle.Single)
                {
                    using (Pen p = new Pen(Color.FromArgb(40, Color.Black)))
                    {
                        Rectangle galleryRect = new Rectangle(0, 0, gallery.Width - 1, gallery.Height - 1);
                        g.SetClip(gallery.ScrollArea, CombineMode.Exclude);
                        g.DrawRectangle(p, galleryRect);
                        g.SetClip(gallery.ScrollArea, CombineMode.Union);
                    }
                }

                PaintGalleryItems(g, gallery);

                switch (gallery.ScrollerType)
                {
                    case ToolStripGalleryScrollerType.Compact:
                        DrawCompactScrollers(g, gallery);
                        break;

                    case ToolStripGalleryScrollerType.Standard:
                        DrawStandardScrollers(g, gallery);
                        break;
                }

                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="gallery"></param>
        protected virtual void DrawStandardScrollers(Graphics g, ToolStripGallery gallery)
        {
            Rectangle scrollRect = gallery.ScrollArea;
            if (scrollRect.Width > 0 && scrollRect.Height > 0)
            {
                using (SolidBrush b =new SolidBrush(Color.Red))                
                {                 
                    g.FillRectangle(b, scrollRect);
                }

                Rectangle rect = new Rectangle(gallery.ScrollUpButton.Bounds.Location,
                    new Size(gallery.ScrollUpButton.Bounds.Width - 1, gallery.ScrollUpButton.Bounds.Height - 1));
                Rectangle imageRect = new Rectangle(rect.Left + rect.Width / 2 - 4, rect.Top + rect.Height / 2 - 1, 9, 5);
                PaintStandardScrollButtonBackground(g, rect, gallery.ScrollUpButton.State);
                if (gallery.ScrollUpButton.State == ScrollButtonState.Disabled)
                {
                    PaintLargeUpArrow(g, imageRect, Color.Gray);
                }
                else
                {
                    PaintLargeUpArrow(g, imageRect, Color.Black);
                }

                rect = new Rectangle(gallery.ScrollDownButton.Bounds.Location,
                    new Size(gallery.ScrollDownButton.Bounds.Width - 1, gallery.ScrollDownButton.Bounds.Height - 1));
                imageRect = new Rectangle(rect.Left + rect.Width / 2 - 4, rect.Top + rect.Height / 2 - 1, 9, 5);
                PaintStandardScrollButtonBackground(g, rect, gallery.ScrollDownButton.State);
                if (gallery.ScrollDownButton.State == ScrollButtonState.Disabled)
                {
                    PaintLargeDownArrow(g, imageRect, Color.Gray);
                }
                else
                {
                    PaintLargeDownArrow(g, imageRect, Color.Black);
                }

                if (!gallery.Scroller.Bounds.IsEmpty)
                {
                    SmoothingMode oldSmoothingMode = g.SmoothingMode;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    rect = new Rectangle(
                        gallery.Scroller.Bounds.Location, new Size(gallery.Scroller.Bounds.Width - 1, gallery.Scroller.Bounds.Height - 1));
                    PaintScrollerBackground(g, rect, gallery.Scroller.State);

                    if (rect.Height > 8)
                    {
                        imageRect = new Rectangle(rect.Left + rect.Width / 2 - 4, rect.Top + rect.Height / 2 - 4, 8, 8);
                        PaintScrollerAdorning(g, imageRect);
                    }

                    g.SmoothingMode = oldSmoothingMode;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="scrollButtonState"></param>
        protected virtual void PaintScrollerBackground(Graphics g, Rectangle rect, ScrollButtonState scrollButtonState)
        {
            Color c1 = Color.Empty;
            Color c2 = Color.Empty;
            Color borderColor = Color.Empty;

            switch (scrollButtonState)
            {
                case ScrollButtonState.Normal:
                case ScrollButtonState.Highlighted:
                    borderColor = Color.Gray;
                    break;

                case ScrollButtonState.Selected:
                    borderColor = StripMetroColor;
                    break;

                case ScrollButtonState.Pressed:
                    borderColor = StripMetroColor;
                    break;
            }
            using (SolidBrush brush =new SolidBrush(Color.Red))          
            {           
                g.FillRectangle(brush, rect);
            }

            using (Pen p = new Pen(borderColor))
            {
                rect.Height++;
                rect.Width++;
                g.DrawPolygon(p, RendererUtils.GetRoundedPolygon(rect, 1));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="scrollButtonState"></param>
        protected virtual void PaintStandardScrollButtonBackground(Graphics g, Rectangle rect, ScrollButtonState scrollButtonState)
        {
            using (SolidBrush brush =new SolidBrush(Color.Red))            
            {
               g.FillRectangle(brush, rect);
            }

            if (scrollButtonState != ScrollButtonState.Disabled && scrollButtonState != ScrollButtonState.Normal)
            {
                using (Pen p = new Pen(Color.FromArgb(100, Color.Black)))
                {
                    g.DrawRectangle(p, rect);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="gallery"></param>
        protected virtual void DrawCompactScrollers(Graphics g, ToolStripGallery gallery)
        {
            Rectangle rect = new Rectangle(gallery.ScrollUpButton.Bounds.Location,
                new Size(gallery.ScrollUpButton.Bounds.Width - 1, gallery.ScrollUpButton.Bounds.Height - 1));
            Rectangle arrowRect = new Rectangle(rect.Left + rect.Width / 2 - 2, rect.Top + rect.Height / 2 - 1, 5, 3);
            PaintScrollButtonBackground(g, rect, gallery.ScrollUpButton.State);
            if (gallery.ScrollUpButton.State == ScrollButtonState.Disabled)
            {
                PaintUpArrow(g, arrowRect, Color.Black);
            }
            else
            {
                arrowRect.Y--;
                PaintUpArrow(g, arrowRect, Color.FromArgb(150, Color.White));
                arrowRect.Y++;
                PaintUpArrow(g, arrowRect, Color.Black);
            }

            rect = new Rectangle(gallery.ScrollDownButton.Bounds.Location,
                new Size(gallery.ScrollDownButton.Bounds.Width - 1, gallery.ScrollDownButton.Bounds.Height - 1));
            arrowRect = new Rectangle(rect.Left + rect.Width / 2 - 2, rect.Top + rect.Height / 2 - 1, 5, 3);
            PaintScrollButtonBackground(g, rect, gallery.ScrollDownButton.State);
            if (gallery.ScrollDownButton.State == ScrollButtonState.Disabled)
            {
                PaintDownArrow(g, arrowRect, Color.Gray);
            }
            else
            {
                arrowRect.Y++;
                PaintDownArrow(g, arrowRect, Color.FromArgb(150, Color.White));
                arrowRect.Y--;
                PaintDownArrow(g, arrowRect, Color.Black);
            }

            rect = new Rectangle(gallery.ScrollDropdownButton.Bounds.Location,
                new Size(gallery.ScrollDropdownButton.Bounds.Width - 1, gallery.ScrollDropdownButton.Bounds.Height - 1));
            arrowRect = new Rectangle(rect.Left + rect.Width / 2 - 2, rect.Top + rect.Height / 2 - 3, 5, 6);
            PaintScrollButtonBackground(g, rect, gallery.ScrollDropdownButton.State);
            arrowRect.Y++;
            PaintDropDownArrow(g, arrowRect, Color.FromArgb(150, Color.White));
            arrowRect.Y--;
            PaintDropDownArrow(g, arrowRect, Color.Black);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="scrollButtonState"></param>
        protected virtual void PaintScrollButtonBackground(Graphics g, Rectangle rect, ScrollButtonState scrollButtonState)
        {            
            using (SolidBrush brush = new SolidBrush(Color.Red))            
            {               
                g.FillRectangle(brush, rect);
            }

            using (Pen p = new Pen(Color.FromArgb(25, Color.Black)))
            {
                g.DrawRectangle(p, rect);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gallery"></param>
        /// <param name="g"></param>
        protected virtual void PaintGalleryItems(Graphics g, ToolStripGallery gallery)
        {
            GraphicsState oldState = g.Save();
            g.TranslateTransform(0, -gallery.ScrollOffset);
            Rectangle workingRect = gallery.GetWorkingArea();
            workingRect.Y += gallery.ScrollOffset;
            g.SetClip(workingRect);
            foreach (ToolStripGalleryItemLayoutInfo itemInfo in gallery.ItemsInfo)
            {
                g.SetClip(itemInfo.Bounds, CombineMode.Intersect);
                Rectangle itemWorkingRect = gallery.GetItemWorkingRect(itemInfo);
                if (!gallery.ItemBackColor.IsEmpty)
                {
                    g.SetClip(itemWorkingRect, CombineMode.Exclude);
                }

                Rectangle rc = new Rectangle(itemInfo.Bounds.Location, new Size(itemInfo.Bounds.Width - 1, itemInfo.Bounds.Height - 1));
                Rectangle rcBackgr = new Rectangle(rc.Left + 1, rc.Top + 1, rc.Width - 2, rc.Height - 2);

                ToolStripGalleryItem item = itemInfo.Item;

                if (item == gallery.PressedItem && item == gallery.SelectedItem)
                {
                    Color cl1 = Color.Red;

                    PaintBackgroundSelected(g, rcBackgr, cl1);                   
                }
                else if (item == gallery.CheckedItem)
                {
                    Color cl1 = Color.Red;

                    PaintBackgroundSelected(g, rcBackgr, cl1);                   
                }
                else if (item == gallery.SelectedItem)
                {
                    Color cl1 = Color.Red;

                    PaintBackgroundSelected(g, rcBackgr, cl1);                  
                }

                if (!gallery.ItemBackColor.IsEmpty)
                {
                    g.SetClip(itemWorkingRect, CombineMode.Union);
                    using (Brush br = new SolidBrush(gallery.ItemBackColor))
                    {
                        g.FillRectangle(br, itemWorkingRect);
                    }
                }

                if (gallery.ItemDisplayStyle == ToolStripItemDisplayStyle.Image
                    || gallery.ItemDisplayStyle == ToolStripItemDisplayStyle.ImageAndText)
                {
                    Image image = item.GetImage();
                    if (image != null)
                    {
                        if (item.Enabled)
                        {
                            g.DrawImage(image, itemInfo.ImageBounds);
                        }
                        else
                        {
                            ControlPaint.DrawImageDisabled(g, item.Image, itemInfo.ImageBounds.X, itemInfo.ImageBounds.Y, Color.Transparent);
                        }
                    }

                }

                if (gallery.ItemDisplayStyle == ToolStripItemDisplayStyle.Text
                    || gallery.ItemDisplayStyle == ToolStripItemDisplayStyle.ImageAndText)
                {
                    TextRenderer.DrawText(g, item.Text, gallery.Font, itemInfo.TextBounds, gallery.ForeColor, TextFormatFlags.EndEllipsis |
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.PreserveGraphicsTranslateTransform |
                        TextFormatFlags.PreserveGraphicsClipping);
                }

                g.SetClip(workingRect, CombineMode.Replace);
            }
            g.Restore(oldState);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gallery"></param>
        /// <param name="g"></param>
        protected virtual void PaintGalleryCaption(Graphics g, ToolStripGallery gallery)
        {
            Rectangle rect = new Rectangle(0, 0, gallery.Width, gallery.Height);
            using (Brush br = new SolidBrush(Color.Black))
            {
                g.FillRectangle(br, new Rectangle(rect.Location, new Size(rect.Width, gallery.CaptionHeight)));
            }

            using (Pen pen = new Pen(Color.FromArgb(60, Color.Black)))
            {
                g.DrawLine(pen, rect.Left, rect.Top + gallery.CaptionHeight, rect.Right, rect.Top + gallery.CaptionHeight);
            }

            Padding textMargin = ToolStripGallery.CAPTION_MARGIN;
            Rectangle textRect = new Rectangle(rect.Left + textMargin.Left, rect.Top + textMargin.Top,
                rect.Width - textMargin.Horizontal, gallery.CaptionHeight - textMargin.Vertical - 1);
            TextRenderer.DrawText(g, gallery.CaptionText, gallery.Owner.Font, textRect, Color.Black,
                TextFormatFlags.EndEllipsis | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="c"></param>
        protected virtual void PaintUpArrow(Graphics g, Rectangle rect, Color c)
        {
            using (Brush b = new SolidBrush(c))
            {
                GraphicsPath path = new GraphicsPath();
                path.AddLine(rect.Left - 1, rect.Bottom, rect.Left + 2, rect.Bottom - 4);
                path.AddLine(rect.Left + 2, rect.Bottom - 4, rect.Left + 5, rect.Bottom);
                path.CloseFigure();
                g.FillPath(b, path);
                path.Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="c"></param>
        protected virtual void PaintLargeUpArrow(Graphics g, Rectangle rect, Color c)
        {
            using (Brush b = new SolidBrush(c))
            {
                GraphicsPath path = new GraphicsPath();
                path.AddLine(rect.Left - 1, rect.Bottom, rect.Left + 4, rect.Bottom - 6);
                path.AddLine(rect.Left + 4, rect.Bottom - 6, rect.Left + 9, rect.Bottom);
                path.CloseFigure();
                g.FillPath(b, path);
                path.Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="c"></param>
        protected virtual void PaintDownArrow(Graphics g, Rectangle rect, Color c)
        {
            using (Brush b = new SolidBrush(c))
            {
                GraphicsPath path = new GraphicsPath();
                path.AddLine(rect.Left, rect.Top, rect.Left + 5, rect.Top);
                path.AddLine(rect.Left + 5, rect.Top, rect.Left + 2, rect.Top + 3);
                path.CloseFigure();
                g.FillPath(b, path);
                path.Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="c"></param>
        protected virtual void PaintLargeDownArrow(Graphics g, Rectangle rect, Color c)
        {
            using (Brush b = new SolidBrush(c))
            {
                GraphicsPath path = new GraphicsPath();
                path.AddLine(rect.Left, rect.Top, rect.Left + 9, rect.Top);
                path.AddLine(rect.Left + 9, rect.Top, rect.Left + 4, rect.Top + 5);
                path.CloseFigure();
                g.FillPath(b, path);
                path.Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        /// <param name="c"></param>
        protected virtual void PaintDropDownArrow(Graphics g, Rectangle rect, Color c)
        {
            using (Brush b = new SolidBrush(c))
            {
                using (Pen p = new Pen(c))
                {
                    g.DrawLine(p, rect.Left, rect.Top, rect.Left + 4, rect.Top);
                }

                GraphicsPath path = new GraphicsPath();
                path.AddLine(rect.Left, rect.Top + 3, rect.Left + 5, rect.Top + 3);
                path.AddLine(rect.Left + 5, rect.Top + 3, rect.Left + 2, rect.Top + 6);
                path.CloseFigure();
                g.FillPath(b, path);
                path.Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        protected virtual void PaintScrollerAdorning(Graphics g, Rectangle rect)
        {
            using (Pen darkPen = new Pen(Color.FromArgb(100, Color.Black)))
            {
                using (Pen lightPen = new Pen(Color.FromArgb(100, Color.White)))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int y = rect.Top + i * 2;
                        g.DrawLine(darkPen, rect.Left, y, rect.Right, y);
                        y++;
                        g.DrawLine(lightPen, rect.Left + 2, y, rect.Right, y);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual bool PaintToolStripGalleryDropDownBackground(ToolStripRenderEventArgs e)
        {
            bool result = false;

            if (e.ToolStrip is ToolStripGalleryDropDown)
            {
                ToolStripGalleryDropDown dropDown = (ToolStripGalleryDropDown)e.ToolStrip;
                if (dropDown.ShowGrip)
                {
                    Rectangle rect = new Rectangle(0, dropDown.Height - ToolStripGalleryDropDown.GALLERY_DRAGGER_HEIGHT,
                        dropDown.Width, ToolStripGalleryDropDown.GALLERY_DRAGGER_HEIGHT);
                    using (SolidBrush b=new SolidBrush(Color.Red))
                    {
                        e.Graphics.FillRectangle(b, rect);
                    }

                    Point p = new Point(rect.Right - 4, rect.Bottom - 3);
                    DrawCornerPoint(e.Graphics, p);
                    p.Y -= 4;
                    DrawCornerPoint(e.Graphics, p);
                    p.Y += 4;
                    p.X -= 4;
                    DrawCornerPoint(e.Graphics, p);
                    result = true;
                }
            }

            return result;
        }
        #endregion

        #region Private Methods

        private void DrawCornerPoint(Graphics g, Point point)
        {
            g.FillRectangle(Brushes.White, new Rectangle(point.X - 1, point.Y - 1, 2, 2));
            g.FillRectangle(Brushes.DarkGray, new Rectangle(point.X - 2, point.Y - 2, 2, 2));
        }
        #endregion
    }
}

#endif
