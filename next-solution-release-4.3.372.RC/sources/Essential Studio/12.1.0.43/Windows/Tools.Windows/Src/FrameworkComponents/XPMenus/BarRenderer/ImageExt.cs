#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	/// <summary>
	/// Represents class for correctly drawing bitmap or icons.
	/// </summary>
	[ 
	Editor( typeof( ImageExtEditor ), typeof( UITypeEditor ) ) ,
	Serializable 
	]
	public class ImageExt:
		IDisposable
	{
		#region Class Constants

		/// <summary>
		/// A value of transparency.
		/// </summary>
		private const float DEF_TRANSPARENCY = 0.5f;

		#endregion

		#region Class Members
		
		/// <summary>
		/// Displayed icon.
		/// </summary>
		private Icon m_icon;
		
		/// <summary>
		/// Displayed bitmaps.
		/// </summary>
		private Bitmap m_bitmap;

        /// <summary>
        /// Gets or Sets, the transparency color for the image.
        /// </summary>
        private Color m_imageTransparentColor = Color.Empty;

		#endregion

		#region Class Properties

		public static implicit operator ImageExt( Icon value )
		{
			return new ImageExt( value );
		}

		public static implicit operator ImageExt( Bitmap value )
		{
			return new ImageExt( value );
		}

		public Size Size
		{
			get
			{
				Size size = new Size();

				if( null != m_icon )
				{
					size = m_icon.Size;
				}
				else if( null != m_bitmap )
				{
					size = m_bitmap.Size;
				}

				return size;
			}
		}

		public int Width
		{
			get
			{
				return this.Size.Width;
			}
		}

		public int Height
		{
			get
			{
				return this.Size.Height;
			}
		}

        /// <summary>
        /// Gets or Sets, the transparency color for the image.
        /// </summary>
        internal Color ImageTransparentColor
        {
            get
            {
                return m_imageTransparentColor;
            }
            set
            {
                if (m_imageTransparentColor != value)
                {
                    m_imageTransparentColor = value;

                    if (value != Color.Empty && m_bitmap != null)
                    {
                        m_bitmap.MakeTransparent(value);
                    }
                }
            }
        }

        #endregion

		#region Class Initialize/Finalize Methods

		public ImageExt( Icon icon )
		{
			this.m_icon = icon;
			this.m_bitmap = null;
		}

		public ImageExt( Image bitmap )
		{
			this.m_icon = null;
			this.m_bitmap = bitmap is Bitmap ? (Bitmap)bitmap : new Bitmap( bitmap );
		}

		#endregion

		#region Class Public Methods

		/// <summary>
		/// Draws icon or bitmap.
		/// </summary>
		public virtual void Draw( Graphics g, Rectangle bounds, DrawItemState state )
		{
			Rectangle imageRect = ( m_icon != null ) ? new Rectangle( new Point(0,0), m_icon.Size ) :
				new Rectangle( new Point(0,0), m_bitmap.Size );

			switch( state )
			{
				case DrawItemState.Disabled : 
				{
					if( m_icon != null )
					{
						Image bm = DrawIconHelper.GetIconToDraw( m_icon, this );

						DrawGreyImage( g, bm, bounds, imageRect, DEF_TRANSPARENCY );
					}
					else if( m_bitmap != null )
					{
						//Draw image;
						DrawGreyImage( g, m_bitmap, bounds, imageRect, DEF_TRANSPARENCY );
					}
					break;
				}
				case DrawItemState.HotLight : 
				{
					if( m_icon != null )
					{
						Image bm = DrawIconHelper.GetIconToDraw( m_icon, this );

						this.DrawShadow( g, bm, bounds, imageRect );
					}
					else if( m_bitmap != null )
					{
						//Draw image;
						this.DrawShadow( g, m_bitmap, bounds, imageRect );
					}
					break;
				}
				case DrawItemState.None : 
				{
					if( m_icon != null )
					{
						//Draw Icon
						Image bm = DrawIconHelper.GetIconToDraw( m_icon, this );

						g.DrawImage( bm, bounds );
					}
					else if( m_bitmap != null )
					{
						//Draw image;
						g.DrawImage( m_bitmap, bounds );
					}

					break;
				}
			}
		}


		/// <summary>
		/// Gets duplicate displayed image.
		/// </summary>
		public Image GetImage()
		{
			Image image = null;

			if( this.m_icon != null )
			{
				image = new Bitmap( IconToBitmap( m_icon ), m_icon.Width, m_icon.Height ); // m_icon.ToBitmap();
			}
			else if( this.m_bitmap != null )
			{
				image = this.m_bitmap;
			}

			return image.Clone() as Image;
		}


		/// <summary>
		/// Gets size of image.
		/// </summary>
		public Size GetSize()
		{
			Size size = Size.Empty;

			if( this.m_bitmap != null )
			{
				size = this.m_bitmap.Size;
			}
			else if( this.m_icon != null )
			{
				size = this.m_icon.Size;
			}

			return size;
		}

		#endregion

		#region Class Utility Methods

		/// <summary>
		/// Draws shadow for image.
		/// </summary>
		private void DrawShadow( Graphics g, Image image, Rectangle bounds, Rectangle imageRect )
		{
			ImageAttributes imageAtributes = new ImageAttributes();
			ColorMatrix colorMatrix = new ColorMatrix();

			colorMatrix.Matrix00 = 0;
			colorMatrix.Matrix11 = 0;
			colorMatrix.Matrix22 = 0;
			colorMatrix.Matrix33 = 0.25f;

			imageAtributes.SetColorMatrix( colorMatrix );

			g.DrawImage( image, bounds, imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height, GraphicsUnit.Pixel, imageAtributes );

			imageAtributes.Dispose();
		}

		
		/// <summary>
		/// Draws grayed image.
		/// </summary>
		private void DrawGreyImage( Graphics g, Image image, Rectangle destRect, RectangleF srcRect, float transparency )
		{
			ImageAttributes imageAtributes = new ImageAttributes();
			ColorMatrix colorMatrix = new ColorMatrix();

			colorMatrix.Matrix00 = 1/3f;
			colorMatrix.Matrix01 = 1/3f;
			colorMatrix.Matrix02 = 1/3f;
			colorMatrix.Matrix10 = 1/3f;
			colorMatrix.Matrix11 = 1/3f;
			colorMatrix.Matrix12 = 1/3f;
			colorMatrix.Matrix20 = 1/3f;
			colorMatrix.Matrix21 = 1/3f;
			colorMatrix.Matrix22 = 1/3f;

			colorMatrix.Matrix33 = transparency;
		
			imageAtributes.SetColorMatrix( colorMatrix );

			g.DrawImage( image, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, imageAtributes );

			imageAtributes.Dispose();
		}
	
		private Bitmap IconToBitmap( Icon icon )
		{
			IntPtr hIco = icon.Handle;
			NativeMethods.ICONINFO ii = new Syncfusion.Runtime.InteropServices.NativeMethods.ICONINFO();
			NativeMethods.GetIconInfo( hIco, out ii);
            Bitmap bmp = Bitmap.FromHbitmap( ii.hbmColor );

			NativeMethods.DeleteObject( ii.hbmColor );
			NativeMethods.DeleteObject( ii.hbmMask );

			return FixAlphaBitmap(bmp);
		}

		private Bitmap FixAlphaBitmap( Bitmap bmSource )
		{
			BitmapData bmData;
			Rectangle bmBounds = new Rectangle(0,0,bmSource.Width, bmSource.Height);

			bmData = bmSource.LockBits(bmBounds, ImageLockMode.ReadOnly, bmSource.PixelFormat);
			Bitmap destBitmap = new Bitmap(bmData.Width, bmData.Height, bmData.Stride, PixelFormat.Format32bppArgb, bmData.Scan0);
			bmSource.UnlockBits(bmData);

			return destBitmap;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if( null != m_icon )
			{
				m_icon.Dispose();
			}

			if( null != m_bitmap )
			{
				m_bitmap.Dispose();
			}
		}

		#endregion
	}
}