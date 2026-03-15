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
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools
{
	#region *** ToolStripGalleryItem
	/// <summary>
	/// 
	/// </summary>
	public class ToolStripGalleryItem
	{
		#region Fields
		/// <summary>
		/// 
		/// </summary>
		private object m_tag;
		/// <summary>
		/// 
		/// </summary>
		private Image m_image;
		/// <summary>
		/// 
		/// </summary>
		private string m_text = String.Empty;
		/// <summary>
		/// Index of image in gallery's image list.
		/// </summary>
		private int m_imageIndex = -1;
		/// <summary>
		/// 
		/// </summary>
		private ImageList m_imageList;
		/// <summary>
		/// 
		/// </summary>
		private Color m_imageTransparentColor;
		/// <summary>
		/// 
		/// </summary>
        private bool m_enable =true;
        /// <summary>
        /// ToolTipText for this GalleryItem
        /// </summary>
        private string m_tooltipText = string.Empty;
      
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		[TypeConverter(typeof(StringConverter)), Localizable(false), DefaultValue((string) null)]
		public object Tag
		{
			get { return m_tag; }
			set { m_tag = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue( null )]
		public Image Image
		{
			get
			{
				return m_image;
			}
			set
			{
				if( m_image != value )
				{
					m_image = value;
					m_imageIndex = -1;

					if( m_imageTransparentColor != Color.Empty )
					{
						Bitmap bmp = value as Bitmap;
						if( bmp != null )
						{
							bmp.MakeTransparent( m_imageTransparentColor );
						}
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[ DefaultValue( "" ) , Localizable(true)]
		public string Text
		{
			get
			{
				return m_text;
			}
			set
			{
				m_text = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue( -1 )]
		public int ImageIndex
		{
			get
			{
				return m_imageIndex;
			}
			set
			{
				m_imageIndex = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal ImageList ImageList
		{
			get
			{
				return m_imageList;
			}
			set
			{
				m_imageList = value;
			}
		}
        /// <summary>
        /// Enable Gallery Item
        /// </summary>
        [DefaultValue(true)]
        public bool Enabled
        {
            get
            {
                return m_enable;
            }
            set
            { 
            m_enable=value;
            }
        }
        /// <summary>
        /// Gets or sets tooltip text
        /// </summary>
        [DefaultValue(null),
         Description("Gets or sets TooolTipText which is used to show text over the GalleryItem")
        ]
        public string ToolTipText
        {
            get
            {
                return this.m_tooltipText;
            }
            set
            {
                if (this.m_tooltipText != value)
                {
                    this.m_tooltipText = value;
                }
            }
        }
       
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue( typeof( Color ), "" )]
		public Color ImageTransparentColor
		{
			get
			{
				return m_imageTransparentColor;
			}
			set
			{
				if( m_imageTransparentColor != value )
				{
					m_imageTransparentColor = value;
					if( m_imageTransparentColor != Color.Empty )
					{
						Bitmap image = m_image as Bitmap;
						if( image != null )
						{
							image.MakeTransparent();
						}
					}
				}
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		internal Image GetImage()
		{
			Image result = null;

			if (m_imageList!=null && m_imageIndex != -1)
			{
				result = m_imageList.Images[m_imageIndex];
			}
			else
			{
				result = m_image;
			}

			return result;
		}
		#endregion
	}
	#endregion

	#region *** ToolStripGalleryItemLayoutInfo
	/// <summary>
	/// 
	/// </summary>
	internal class ToolStripGalleryItemLayoutInfo
	{
		#region Fields
		/// <summary>
		/// 
		/// </summary>
		public Rectangle Bounds;
		/// <summary>
		/// 
		/// </summary>
		public Rectangle ImageBounds;
		/// <summary>
		/// 
		/// </summary>
		public Rectangle TextBounds;
		/// <summary>
		/// 
		/// </summary>
		private ToolStripGalleryItem m_item;
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public ToolStripGalleryItem Item
		{
			get
			{
				return m_item;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public ToolStripGalleryItemLayoutInfo( ToolStripGalleryItem item )
		{
			m_item = item;
		}
		#endregion
	}
	#endregion

	#region *** ToolStripGalleryItemEventHandler
	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	/// <returns></returns>
	public delegate void ToolStripGalleryItemEventHandler( object sender, ToolStripGalleryItemEventArgs args );
	#endregion

	#region *** ToolStripGalleryItemEventArgs
	/// <summary>
	/// 
	/// </summary>
	public class ToolStripGalleryItemEventArgs
		: EventArgs
	{
		#region Public Fields
		/// <summary>
		/// 
		/// </summary>
		public ToolStripGalleryItem GalleryItem;
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		/// <param name="galleryItem"></param>
		public ToolStripGalleryItemEventArgs( ToolStripGalleryItem galleryItem )
		{
			this.GalleryItem = galleryItem;
		}
		#endregion
	}
	#endregion
}

#endif
