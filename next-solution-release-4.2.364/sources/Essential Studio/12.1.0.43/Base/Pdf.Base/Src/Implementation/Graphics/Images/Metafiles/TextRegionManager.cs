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
    /// Manages TextRegion objects.
    /// </summary>
    internal class TextRegionManager
    {
#region Fields
        /// <summary>
        /// Collection of the regions.
        /// </summary>
        private ArrayList m_regions;
        #endregion

#region Constructors
        /// <summary>
        /// Creates new object.
        /// </summary>
        public TextRegionManager()
        {
            m_regions = new ArrayList();
        }
        #endregion

# region Properties
        internal int Count
        {
            get
            {
                return m_regions.Count;
            }
        }
        # endregion

#region Public Methods
        /// <summary>
        /// Adds a text region into the collection.
        /// </summary>
        /// <param name="region"></param>
        public void Add(TextRegion region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            // Search for the all regions which intersect with the current.
            TextRegion[] regions = Intersect(region);

            // Join all of them.
            TextRegion rgn = Union(regions, region);

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
            TextRegion region = new TextRegion(y, 1);
            TextRegion[] regions = Intersect(region);

            if (regions != null && regions.Length == 1)
            {
                TextRegion rgn = regions[0];
                result = rgn.Y;
            }
            else if (regions != null && regions.Length > 1)
            {
                // NOTE: This shouldn't rise if regions set is built correctly.
                //throw new InvalidOperationException("Invalie regions set");

                TextRegion rgn1 = regions[0];
                TextRegion rgn2 = regions[1];

                if (rgn1.Y < rgn2.Y)
                    result = rgn1.Y;
                else
                    result = rgn2.Y;
            }

            return result;
        }

        /// <summary>
        /// Searches for the last before text region.
        /// </summary>
        /// <param name="y">Y co-ordinate of some text region.</param>
        /// <returns>
        /// Returns the Y co-ordinate of last before text region.
        /// </returns>
        public float GetCoordinate(float y)
        {
            // Search if any region contains this Y co-ordinate.
            // If such exists - return his top co-ordinate, if doesn't - return y.
            float result = y;
            TextRegion region = new TextRegion(y, 1);
            TextRegion[] regions = Intersect(region);

            if (regions != null && regions.Length == 1)
            {
                TextRegion rgn = regions[0];
                result = rgn.Y;
                //isTrue = true;
            }
            else if (regions != null && regions.Length > 1)
            {
                // NOTE: This shouldn't rise if regions set is built correctly.
                //throw new InvalidOperationException("Invalie regions set");

                TextRegion rgn1 = regions[0];
                TextRegion rgn2 = regions[1];

                if (rgn1.Y < rgn2.Y)
                    result = rgn1.Y;
                else
                    result = rgn2.Y;
            }
            else if (regions.Length == 0)
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
        private TextRegion[] Intersect(TextRegion region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            // Search for the all regions which intersest with the current.
            ArrayList result = new ArrayList();
            for (int i = 0, len = m_regions.Count; i < len; i++)
            {
                TextRegion rgn = (TextRegion)m_regions[i];
                if (region.IntersectsWith(rgn))
                {
                    result.Add(rgn);
                }
            }

            return (TextRegion[])result.ToArray(typeof(TextRegion));
        }

        /// <summary>
        /// Removes region from the colection.
        /// </summary>
        /// <param name="region">Region that should be removed from he collection.</param>
        private void Remove(TextRegion region)
        {
            if (region == null)
                throw new ArgumentNullException("region");

            m_regions.Remove(region);
        }

        /// <summary>
        /// Removes regions from the colection.
        /// </summary>
        /// <param name="regions">Array of regions that should be removed from he collection.</param>
        private void Remove(TextRegion[] regions)
        {
            if (regions == null)
                throw new ArgumentNullException("regions");

            for (int i = 0, len = regions.Length; i < len; i++)
            {
                TextRegion region = regions[i];
                m_regions.Remove(region);
            }
        }

        /// <summary>
        /// Joins array of regions and the region into one region.
        /// </summary>
        /// <param name="regions">Array of the regions.</param>
        /// <param name="region">Current text region.</param>
        private TextRegion Union(TextRegion[] regions, TextRegion region)
        {
            if (regions == null)
                throw new ArgumentNullException("regions");

            if (region == null)
                throw new ArgumentNullException("region");

            // Join the array and the region into one region.
            for (int i = 0, len = regions.Length; i < len; i++)
            {
                TextRegion rgn = regions[i];

                if (region.IntersectsWith(rgn))
                {
                    region = TextRegion.Union(region, rgn);
                }
            }

            return region;
        }
        #endregion
    }
}
#endif