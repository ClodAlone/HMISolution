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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// ImageList with support for images of different sizes and transparency.
	/// </summary>
    //[Designer(typeof(ImageListAdvDesigner), typeof(System.ComponentModel.Design.IDesigner))]
	[
	ToolboxItem(true),
	System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.PopupControlContainer), "ToolboxIcons.Imagelistadv.bmp"),
	Description("Provides support for hosting images of different sizes and transparency.")
	]

	public class ImageListAdv : Component, ICloneable
	{
		#region Constants
		/// <summary>
		/// Default image size.
		/// </summary>
		private static readonly Size IMAGE_SIZE = new Size(16, 16);
		#endregion

		#region Fields
		/// <summary>
		/// Collection of images.
		/// </summary>
		private ImageCollection m_images;
		/// <summary>
		/// Size of images. Used in drawing and for compatibility reasons.
		/// </summary>
		private Size m_imageSize = IMAGE_SIZE;
		/// <summary>
		/// Indicates whether images should be drawn using ImageSize property.
		/// </summary>
		private bool m_bUseImageSize = true;
		/// <summary>
		/// Tag object.
		/// </summary>
		private object m_tag;
		#endregion

		#region Properties
		/// <summary>
		/// Gets collection of images.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("Collection of images.")]
		public ImageCollection Images
		{
			get
			{
				return m_images;
			}
		}
		/// <summary>
		/// Gets or sets size of images. Used in drawing.
		/// </summary>
		[Description("Size of images. Used in drawing.")]
		public Size ImageSize
		{
			get
			{
				return m_imageSize;
			}
			set
			{
				m_imageSize = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether images should be drawn using ImageSize property.
		/// </summary>
		[DefaultValue(true)]
		[Description("Indicates whether images should be drawn using ImageSize property.")]
		public bool UseImageSize
		{
			get
			{
				return m_bUseImageSize;
			}
			set
			{
				m_bUseImageSize = value;
			}
		}
		/// <summary>
		/// Gets or sets tag object.
		/// </summary>
		[DefaultValue(null)]
		[Description("Tag object.")]
		public object Tag
		{
			get
			{
				return m_tag;
			}
			set
			{
				m_tag = value;
			}
		}
		#endregion

		#region Initialization And Finalization
		/// <summary>
		/// Creates and initializes new ImageListAdv.
		/// </summary>
		public ImageListAdv()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(ImageListAdv));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			m_images = new ImageCollection();
		}
		/// <summary>
		/// Creates and initializes new ImageListAdv.
		/// </summary>
		/// <param name="container">Container to add component to.</param>
		public ImageListAdv(IContainer container)
			: this()
		{
			container.Add(this);
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Draws selected image to specified Graphics. If UseImageSize property is set to true, image is drawn using ImageSize property;
		/// otherwise it's drawn using original size.
		/// </summary>
		/// <param name="g">Graphics to draw to.</param>
		/// <param name="pt">Point to draw image at.</param>
		/// <param name="index">Index of image to draw.</param>
		public void Draw(Graphics g, Point pt, int index)
		{
			this.Draw(g, pt.X, pt.Y, index);
		}
		/// <summary>
		/// Draws selected image to specified Graphics. If UseImageSize property is set to true, image is drawn using ImageSize property;
		/// otherwise it's drawn using original size.
		/// </summary>
		/// <param name="g">Graphics to draw to.</param>
		/// <param name="x">X coordinate of point to draw image at.</param>
		/// <param name="y">Y coordinate of point to draw image at.</param>
		/// <param name="index">Index of image to draw.</param>
		public void Draw(Graphics g, int x, int y, int index)
		{
			if (index < 0 || index > m_images.Count - 1) throw new ArgumentOutOfRangeException("index");

			if (m_bUseImageSize)
			{
				this.Draw(g, x, y, m_imageSize.Width, m_imageSize.Height, index);
			}
			else
			{
				g.DrawImage(m_images[index], x, y);
			}
		}
		/// <summary>
		/// Draws selected image to specified Graphics using given size.
		/// </summary>
		/// <param name="g">Graphics to draw to.</param>
		/// <param name="x">X coordinate of point to draw image at.</param>
		/// <param name="y">Y coordinate of point to draw image at.</param>
		/// <param name="width">Width of rectangle to draw image to.</param>
		/// <param name="height">Height of rectangle to draw image to.</param>
		/// <param name="index">Index of image to draw.</param>
		public void Draw(Graphics g, int x, int y, int width, int height, int index)
		{
			if (index < 0 || index > m_images.Count - 1) throw new ArgumentOutOfRangeException("index");

			g.DrawImage(m_images[index], x, y, width, height);
		}
		/// <summary>
		/// Explicitly converts ImageList to ImageListAdv.
		/// </summary>
		/// <param name="list">ImageList to convert.</param>
		/// <returns>ImageListAdv with images from given ImageList.</returns>
		public static explicit operator ImageListAdv(ImageList list)
		{
			ImageListAdv result = new ImageListAdv();
			result.ImageSize = list.ImageSize;
			Image[] images = new Image[list.Images.Count];
			for (int i = 0, len = list.Images.Count; i < len; i++)
			{
				images[i] = list.Images[i];
			}
			result.Images.AddRange(images);
			return result;
		}
		/// <summary>
		/// Explicitly converts ImageListAdv to ImageList.
		/// </summary>
		/// <param name="list">ImageListAdv to convert.</param>
		/// <returns>ImageList with images from given ImageListAdv.</returns>
		public static explicit operator ImageList(ImageListAdv list)
		{
			ImageList result = new ImageList();
			result.ImageSize = list.ImageSize;
			foreach (Image img in list.Images)
			{
				result.Images.Add(img);
			}
			return result;
		}
		/// <summary>
		/// Converts ImageListAdv to ImageList.
		/// </summary>
		/// <returns>ImageList with images from ImageListAdv.</returns>
		public ImageList ToImageList()
		{
			return (ImageList)this;
		}
		/// <summary>
		/// creates ImageListAdv from ImageList.
		/// </summary>
		/// <param name="list">ImageList to create ImageListAdv from.</param>
		/// <returns>Created ImageListAdv.</returns>
		public static ImageListAdv FromImageList(ImageList list)
		{
			return (ImageListAdv)list;
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// Converts Icon to Image with correction of alpha channel.
		/// </summary>
		/// <param name="icon">Icon to convert.</param>
		/// <returns>Resulting Image.</returns>
		internal static Image IconToImageAlphaCorrect(Icon icon)
		{
			if (icon == null) throw new ArgumentNullException("icon");

#if SyncfusionFramework1_1 || SyncfusionFramework1

			Bitmap image = null;
			NativeMethods.ICONINFO info = new NativeMethods.ICONINFO();
			NativeMethods.GetIconInfo( icon.Handle, out info );
			NativeMethods.BITMAP bm = new NativeMethods.BITMAP();
			try
			{
				if( info.hbmColor != IntPtr.Zero )
				{
					NativeMethods.GetObject( info.hbmColor, Marshal.SizeOf( typeof( NativeMethods.BITMAP ) ), out bm );
					if( bm.bmBitsPixel == 0x20 )
					{
						Bitmap bitmap3 = null;
						BitmapData bmpData = null;
						BitmapData targetData = null;
						try
						{
							bitmap3 = Image.FromHbitmap( info.hbmColor );
							bmpData = bitmap3.LockBits( new Rectangle( 0, 0, bitmap3.Width, bitmap3.Height ), ImageLockMode.ReadOnly, bitmap3.PixelFormat );
							if( BitmapHasAlpha( bmpData ) )
							{
								image = new Bitmap( bmpData.Width, bmpData.Height, PixelFormat.Format32bppArgb );
								targetData =
									image.LockBits( new Rectangle( 0, 0, bmpData.Width, bmpData.Height ), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb );

								int num = 0;
								int num2 = 0;
								for( int i = 0; i < Math.Min( bmpData.Height, targetData.Height ); i++ )
								{
									IntPtr handle = new IntPtr( bmpData.Scan0.ToInt32() + num );
									IntPtr ptr2 = new IntPtr( targetData.Scan0.ToInt32() + num2 );
									NativeMethods.CopyMemory( ptr2, handle, Math.Abs( targetData.Stride ) );
									num += bmpData.Stride;
									num2 += targetData.Stride;
								}
							}
						}
						finally
						{
							if( bitmap3 != null && bmpData != null )
							{
								bitmap3.UnlockBits( bmpData );
							}
							if( image != null && targetData != null )
							{
								image.UnlockBits( targetData );
							}
						}
						bitmap3.Dispose();
					}
				}
			}
			finally
			{
				if( info.hbmColor != IntPtr.Zero )
				{
					NativeMethods.IntDeleteObject( info.hbmColor );
				}
				if( info.hbmMask != IntPtr.Zero )
				{
					NativeMethods.IntDeleteObject( info.hbmMask );
				}
			}

			return ( image == null ) ? ( icon.ToBitmap() ) : ( image );

#else

			// It's fixed in Fr2.0
			return icon.ToBitmap();

#endif

		}
		/// <summary>
		/// Checks whether Bitmap has	alpha channel.
		/// </summary>
		/// <param name="bmpData">BitmapData to check.</param>
		/// <returns>True if bitmap has alpha channel; otherwise false.</returns>
		private static bool BitmapHasAlpha(BitmapData bmpData)
		{
			for (int i = 0; i < bmpData.Height; i++)
			{
				for (int j = 3; j < Math.Abs(bmpData.Stride); j += 4)
				{
					byte numPtr = Marshal.ReadByte((IntPtr)((bmpData.Scan0.ToInt32() + (i * bmpData.Stride)) + j));
					if (numPtr != 0)
					{
						return true;
					}
				}
			}
			return false;
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Creates string with info about ImageListAdv.
		/// </summary>
		/// <returns>String with info about ImageListAdv</returns>
		public override string ToString()
		{
			string text = base.ToString() + " Images.Count: " + this.Images.Count.ToString();
			return text;
		}
		#endregion

		#region ShouldSerialize And Reset Methods
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeImageSize()
		{
			return (m_imageSize != IMAGE_SIZE);
		}
		/// <summary>
		/// 
		/// </summary>
		protected void ResetImageSize()
		{
			m_imageSize = IMAGE_SIZE;
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			ImageListAdv clone = new ImageListAdv();
			
			clone.ImageSize = this.ImageSize;
			clone.UseImageSize = this.UseImageSize;

			ImageCollection targetImages = clone.Images;
			ImageCollection sourceImages = this.Images;

			for (int i = 0; i < sourceImages.Count; i++)
			{
				Image image = sourceImages[i];

				if (image != null)
				{
					image = (Image)image.Clone();
				}

				targetImages.Add(image);
			}
			
			return clone;
		}

		#endregion
	}
    /// <summary>
    /// ImageListAdv Designer
    /// </summary>
    [Designer(typeof(ImageListAdvDesigner), typeof(System.ComponentModel.Design.IDesigner))]
    public class ImageListAdvDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public ImageListAdvDesigner()
            : base()
        {
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        /// <summary>
        /// Gets a value indication the designer action
        /// </summary>
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == this.actionLists)
                {
                    this.actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    this.actionLists.Add(new ImageListAdvActionList(this.Component));
                }

                return this.actionLists;
            }
        }

#endif
    }
}