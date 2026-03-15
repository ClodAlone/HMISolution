#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;
using System.Drawing;

namespace Syncfusion.Pdf.Graphics.Images.Metafiles
{
    /// <summary>
    /// Region of the space in which image in metafile can take.
    /// </summary>
    internal class ImageRegion
    {
#region Fields
        /// <summary>
        /// Y co-ordinate of the region.
        /// </summary>
        private float m_y;

        /// <summary>
        /// Height of the region.
        /// </summary>
        private float m_height;
        #endregion

#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRegion"/> class.
        /// </summary>
        public ImageRegion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRegion"/> class.
        /// </summary>
        /// <param name="y">The y.</param>
        /// <param name="height">The height.</param>
        public ImageRegion(float y, float height)
        {
            if (height < 0)
            {
                throw new ArgumentOutOfRangeException("height", height, "Value can not be less 0");
            }

            Y = y;
            Height = height;
        }
        #endregion

#region Properties
        /// <summary>
        /// Gets or sets Y co-ordinate of the region when the text starts.
        /// </summary>
        public float Y
        {
            get
            {
                return m_y;
            }
           
            set
            {
                m_y = value;
            }
        }

        /// <summary>
        /// Gets or sets Height of the text region.
        /// </summary>
        public float Height
        {
            get
            {
                return m_height;
            }
           
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Height can not be less 0");
                }

                m_height = value;
            }
        }
        #endregion

#region Static methods
        /// <summary>
        /// Joins two regions.
        /// </summary>
        /// <param name="region1">Image region to be joined.</param>
        /// <param name="region2">Image region to be joined.</param>
        /// <returns>Joined region.</returns>
        public static ImageRegion Union(ImageRegion region1, ImageRegion region2)
        {
            if (region1 == null)
            {
                throw new ArgumentNullException("region1");
            }

            if (region2 == null)
            {
                throw new ArgumentNullException("region2");
            }

            if (!region1.IntersectsWith(region2))
            {
                throw new ArgumentException("The specified regions don't intersect");
            }

            ImageRegion region = new ImageRegion();
            float y = Math.Min(region1.Y, region2.Y);
            float height = Math.Max(region1.Y + region1.Height, region2.Y + region2.Height) - y;
            region.Y = y;
            region.Height = height;

            return region;
        }
        #endregion

#region Public Methods
        /// <summary>
        /// Checks whether region intersect with the current one.
        /// </summary>
        /// <param name="region">Region object.</param>
        /// <returns>True - if they're intersected, False - otherwise.</returns>
        public bool IntersectsWith(ImageRegion region)
        {
            if (region == null)
            {
                throw new ArgumentNullException("region");
            }

            RectangleF sourceRect = new RectangleF(0, Y, 1, Height);
            RectangleF destRect = new RectangleF(0, region.Y, 1, region.Height);

            bool intersects = sourceRect.IntersectsWith(destRect);

            return intersects;
        }
        #endregion
    }
}
#endif