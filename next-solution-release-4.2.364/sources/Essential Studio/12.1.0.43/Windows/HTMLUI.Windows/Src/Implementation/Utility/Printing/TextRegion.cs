#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility
{
    /// <summary>
    /// Region of the space which text in metafile can take.
    /// </summary>
    internal class TextRegion
    {
        #region Class members
        /// <summary>
        /// Y co-ordinate of the region.
        /// </summary>
        private int m_y;

        /// <summary>
        /// Height of the region.
        /// </summary>
        private int m_height;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets Y co-ordinate of the region when the text starts.
        /// </summary>
        public int Y
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
        public int Height
        {
            get
            {
                return m_height;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", value, "Height can not be less 0");

                m_height = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the TextRegion class
        /// </summary>
        public TextRegion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TextRegion class
        /// </summary>
        /// <param name="y">Gets or sets Y co-ordinate of the region when the text starts.</param>
        /// <param name="height">Gets or sets Height of the text region.</param>
        public TextRegion(int y, int height)
        {
            if (height < 0)
                throw new ArgumentOutOfRangeException("height", height, "Value can not be less 0");

            Y = y;
            Height = height;
        }
        #endregion

        #region Class static methods
        /// <summary>
        /// Joins two regions.
        /// </summary>
        /// <param name="region1">Text region in which another region is to be joined.</param>
        /// <param name="region2">Text region to be joined.</param>
        /// <returns>Joined region.</returns>
        public static TextRegion Union(TextRegion region1, TextRegion region2)
        {
            if (region1 == null)
                throw new ArgumentNullException("region1");
            if (region2 == null)
                throw new ArgumentNullException("region2");
            if (!region1.IntersectsWith(region2))
                throw new ArgumentException("The specified regions don't intersect");

            TextRegion region = new TextRegion();
            int y = Math.Min(region1.Y, region2.Y);
            int height = Math.Max(region1.Y + region1.Height, region2.Y + region2.Height) - y;
            region.Y = y;
            region.Height = height;

            return region;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Checks whether region intersestc with the current one.
        /// </summary>
        /// <param name="region">Region object.</param>
        /// <returns>True - if they're intersected, False - otherwise.</returns>
        public bool IntersectsWith(TextRegion region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            Rectangle sourceRect = new Rectangle(0, Y, 1, Height);
            Rectangle destRect = new Rectangle(0, region.Y, 1, region.Height);

            bool intersects = sourceRect.IntersectsWith(destRect);

            return intersects;
        }
        #endregion
    }
}
