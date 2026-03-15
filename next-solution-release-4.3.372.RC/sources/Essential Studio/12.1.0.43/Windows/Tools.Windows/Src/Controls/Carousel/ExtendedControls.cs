#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Image which adds as a object when adding to Carousel's ImageListCollection
    /// </summary>
    public class CarouselImage
    {
        /// <summary>
        /// Creates a new instance of Image
        /// </summary>
        public CarouselImage()
        {
        }

        private Image itemImage = null;
        /// <summary>
        /// Image to render in the carousel
        /// </summary>
        [Category("Appearance"),
        Description("Gets or sets image to load in the Carousel.")]
        public Image ItemImage
        {
            get
            {
                return itemImage;
            }
            set
            {
                itemImage = value;
            }
        }
    }

    /// <summary>
    /// Object which used within Preview Element
    /// </summary>
    public class CarouselElement
    {
        static public int m_ThumbSize = 200;

        internal Image m_bmpOriginal = null;
        internal Image m_bmpMain = null;
        internal Bitmap m_bmpShadow = null;

        internal double m_dAngleOriginal = 0;
        internal double m_dAngleActual = 0;
        internal double m_dDistanceFromScreen = 0;

        internal Rectangle m_Rect = new Rectangle();
        internal Rectangle m_RectShadow = new Rectangle();


        /// <summary>
        /// Creates a new instance of carousel element
        /// </summary>
        /// <param name="image">image to load in the collection</param>
        public CarouselElement(Image image)
        {
            m_bmpOriginal = image;
            ApplyImageToCarouselElement();
        }

        /// <summary>
        /// Creates a new instance of carousel element
        /// </summary>
        /// <param name="strFileName">filename of the image to load in collection</param>
        public CarouselElement(String strFileName)
        {
            m_bmpOriginal = new Bitmap(strFileName);
            ApplyImageToCarouselElement();
        }

        /// <summary>
        /// Applies image to the carousel element
        /// </summary>
        private void ApplyImageToCarouselElement()
        {
            int nWidth = m_ThumbSize;
            int nHeight = m_bmpOriginal.Height * m_ThumbSize / m_bmpOriginal.Width;

            if (nHeight > m_ThumbSize)
            {
                nHeight = m_ThumbSize;
                nWidth = m_bmpOriginal.Width * m_ThumbSize / m_bmpOriginal.Height;
            }

            m_bmpMain = new Bitmap(nWidth, nHeight);

            Graphics g = Graphics.FromImage(m_bmpMain);

            g.DrawImage(m_bmpOriginal, 0, 0, nWidth, nHeight);

            m_bmpShadow = new Bitmap(m_bmpMain);

            unsafe
            {
                m_bmpShadow.RotateFlip(RotateFlipType.RotateNoneFlipY);

                System.Drawing.Imaging.BitmapData bmd = m_bmpShadow.LockBits(new Rectangle(0, 0, m_bmpShadow.Width, m_bmpShadow.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    m_bmpShadow.PixelFormat);

                int PixelSize = 4;

                byte* row = (byte*)bmd.Scan0;

                for (int y = 0; y < bmd.Height; y++)
                {
                    byte trasp = (byte)(100 * ((m_bmpMain.Height - y)) / m_bmpMain.Height);

                    int xx = 3;

                    for (int x = 0; x < bmd.Width; x++)
                    {
                        row[xx] = trasp;

                        xx += PixelSize;
                    }

                    row += bmd.Stride;
                }
                m_bmpShadow.UnlockBits(bmd);
            }

        }
    }


    /// <summary>
    /// Object which used to display in preview
    /// </summary>
    public class PreviewElement
    {

        #region Variables

        private Bitmap previewBitmap = null;
        private Rectangle previewBitmapStartRect = new Rectangle();
        private Rectangle previewBitmapRect = new Rectangle();
        private int previewBitmapPerc;
        private int previewBitmapState;
        private CarouselElement previewObject = null;

        #endregion

        /// <summary>
        /// Constructor. Creates a new instance of Preview Element
        /// </summary>
        public PreviewElement()
        {
        }

        /// <summary>
        /// Bitmap used to display in preview
        /// </summary>
        internal Bitmap PreviewBitmap
        {
            get
            {
                return previewBitmap;
            }
            set
            {
                previewBitmap = value;
            }
        }

        /// <summary>
        /// Rectangle where preview image starts to draw
        /// </summary>
        internal Rectangle PreviewBitmapStartRect
        {
            get
            {
                return previewBitmapStartRect;
            }
            set
            {
                previewBitmapStartRect = value;
            }

        }

        /// <summary>
        /// Recatngle where preview image displays at end
        /// </summary>
        internal Rectangle PreviewBitmapRect
        {
            get
            {
                return previewBitmapRect;
            }
            set
            {
                previewBitmapRect = value;
            }
        }

        /// <summary>
        /// Perspective value of preview bitmap
        /// </summary>
        internal int PreviewBitmapPerc
        {
            get
            {
                return previewBitmapPerc;
            }
            set
            {
                previewBitmapPerc = value;
            }
        }

        /// <summary>
        /// State of Preview bitmap : either in display or hide
        /// </summary>
        internal int PreviewBitmapState
        {
            get
            {
                return previewBitmapState;
            }
            set
            {
                previewBitmapState = value;
            }
        }

        /// <summary>
        /// Image which used to display as preview
        /// </summary>
        internal CarouselElement PreviewObject
        {
            get
            {
                return previewObject;
            }
            set
            {
                previewObject = value;
            }
        }
    }

    [Designer(typeof(CarouselItemDesigner))]
    [ToolboxItem(false)]
    public class CarouselItem : PictureBox
    {
        public CarouselItem()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

    }

    /// <summary>
    /// Desginer class of CarouselItem
    /// </summary>
    public class CarouselItemDesigner : ScrollableControlDesigner
    {
        public override SelectionRules SelectionRules
        {
            get { return SelectionRules.Locked; }
        }
    }

    /// <summary>
    /// List of available carousel path
    /// </summary>
    public enum CarouselPath
    {
        Default = 1,
        Orbital = 2,
        Linear = 3,
        Oval = 4

    }

    /// <summary>
    /// List of available Visual Styles
    /// </summary>
    public enum CarouselVisualStyle
    {
        Default,
        OfficeStyle,
        Metro
    }
}
