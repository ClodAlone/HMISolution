#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;
using System.Collections;

namespace Syncfusion.Pdf.Graphics.Images.Metafiles
{
    /// <summary>
    /// Manages ImageRegion objects.
    /// </summary>
    internal class ImageRegionManager
    {
#region Fields
        /// <summary>
        /// Collection of the regions.
        /// </summary>
        private ArrayList m_regions;
        #endregion

#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRegionManager"/> class.
        /// </summary>
        public ImageRegionManager()
        {
            m_regions = new ArrayList();
        }
        #endregion

#region Public Methods
        /// <summary>
        /// Adds a Image region into the collection.
        /// </summary>
        /// <param name="region">region</param>
        public void Add(ImageRegion region)
        {
            if (region == null)
            {
                throw new ArgumentNullException("region");
            }

            // Search for the all regions which intersect with the current.
            ImageRegion[] regions = Intersect(region);

            // Join all of them.
            ImageRegion rgn = Union(regions, region);

            // Insert the union into the collection.
            m_regions.Add(rgn);

            // Remove all the pieces of the union from the collection.
            Remove(regions);
        }

        /// <summary>
        /// Searches for the largest Y co-ordinate of the region if the y is inside of any region
        /// or returns y if it's out of any region.
        /// </summary>
        /// <param name="y">Y co-ordinate of some text region.</param>
        /// <returns>
        /// Searches for the largest Y co-ordinate of the region if the y is inside of any region
        /// or returns y if it's out of any region.
        /// </returns>
        public float GetTopCoordinate(float y)
        {
            // Search if any region contains this Y co-ordinate.
            // If such exists - return his top co-ordinate, if doesn't - return y.
            float result = y;
            ImageRegion region = new ImageRegion(y, 1);
            ImageRegion[] regions = Intersect(region);

            if (regions != null && regions.Length == 1)
            {
                ImageRegion rgn = regions[0];
                result = rgn.Y;
            }
            else if (regions != null && regions.Length > 1)
            {
                // NOTE: This shouldn't rise if regions set is built correctly.
                //throw new InvalidOperationException("Invalid regions set");

                ImageRegion rgn1 = regions[0];
                ImageRegion rgn2 = regions[1];

                if (rgn1.Y < rgn2.Y)
                    result = rgn1.Y;
                else
                    result = rgn2.Y;
            }

            return result;
        }

        /// <summary>
        /// Searches for the image region.
        /// </summary>
        /// <param name="y">Y co-ordinate of some image region.</param>
        /// <returns>
        /// Returns the Y co-ordinate of last before image region.
        /// </returns>
        public float GetCoordinate(float y)
        {
            // Search if any region contains this Y co-ordinate.
            // If such exists - return his top co-ordinate, if doesn't - return 0.
            float result = y;
            ImageRegion region = new ImageRegion(y, 1);
            ImageRegion[] regions = Intersect(region);

            if (regions != null && regions.Length == 1)
            {
                ImageRegion rgn = regions[0];
                result = rgn.Y;
            }
            else if (regions != null && regions.Length > 1)
            {
                // NOTE: This shouldn't rise if regions set is built correctly.
                //throw new InvalidOperationException("Invalid regions set");

                ImageRegion rgn1 = regions[0];
                ImageRegion rgn2 = regions[1];

                if (rgn1.Y < rgn2.Y)
                    result = rgn1.Y;
                else
                    result = rgn2.Y;
            }
            else if (regions == null)
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            m_regions.Clear();
        }
        #endregion

#region Implementation
        /// <summary>
        /// Searches for all regions in the collection that are intersested with the current one.
        /// </summary>
        /// <param name="region">Current text region.</param>
        /// <returns>Array of regions that intersect with the current.</returns>
        private ImageRegion[] Intersect(ImageRegion region)
        {
            if (region == null)
            {
                throw new ArgumentNullException("region");
            }

            // Search for the all regions which intersest with the current.
            ArrayList result = new ArrayList();
            for (int i = 0, len = m_regions.Count; i < len; i++)
            {
                ImageRegion rgn = (ImageRegion)m_regions[i];
                if (region.IntersectsWith(rgn))
                {
                    result.Add(rgn);
                }
            }

            return (ImageRegion[])result.ToArray(typeof(ImageRegion));
        }

        /// <summary>
        /// Removes region from the colection.
        /// </summary>
        /// <param name="region">Region that should be removed from he collection.</param>
        private void Remove(ImageRegion region)
        {
            if (region == null)
            {
                throw new ArgumentNullException("region");
            }

            m_regions.Remove(region);
        }

        /// <summary>
        /// Removes regions from the collection.
        /// </summary>
        /// <param name="regions">Array of regions that should be removed from he collection.</param>
        private void Remove(ImageRegion[] regions)
        {
            if (regions == null)
            {
                throw new ArgumentNullException("regions");
            }

            for (int i = 0, len = regions.Length; i < len; i++)
            {
                ImageRegion region = regions[i];
                m_regions.Remove(region);
            }
        }

        /// <summary>
        /// Joins array of regions and the region into one region.
        /// </summary>
        /// <param name="regions">Array of the regions.</param>
        /// <param name="region">Current image region.</param>
        private ImageRegion Union(ImageRegion[] regions, ImageRegion region)
        {
            if (regions == null)
            {
                throw new ArgumentNullException("regions");
            }

            if (region == null)
            {
                throw new ArgumentNullException("region");
            }

            // Join the array and the region into one region.
            for (int i = 0, len = regions.Length; i < len; i++)
            {
                ImageRegion rgn = regions[i];

                if (region.IntersectsWith(rgn))
                {
                    region = ImageRegion.Union(region, rgn);
                }
            }

            return region;
        }
        #endregion
    }
}
#endif