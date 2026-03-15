#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility
{
    /// <summary>
    /// Manages TextRegion objects.
    /// </summary>
    internal class TextRegionManager
    {
        #region Class members
        /// <summary>
        /// Collection of the regions.
        /// </summary>
        private ArrayList m_regions;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the TextRegionManager class
        /// </summary>
        public TextRegionManager()
        {
            m_regions = new ArrayList();
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Adds a text region into the collection.
        /// </summary>
        /// <param name="region">TextRegion instance</param>
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
        /// or returns region containing co-ordinate.
        /// </returns>
        public TextRegion GetTopCoordinate(int y)
        {
            // Search if any region contains this Y co-ordinate.
            // If such exists - return his top co-ordinate, if doesn't - return y.
            int result = y;
            TextRegion region = new TextRegion(y, 1);
            TextRegion[] regions = Intersect(region);
            if (regions != null && regions.Length == 1)
            {
                TextRegion rgn = regions[0];
                //// result = rgn.Y;
                region = new TextRegion(rgn.Y, rgn.Height);
            }
            else if (regions != null && regions.Length > 1)
            {
                // NOTE: This shouldn't rise if regions set is built correctly.
                throw new InvalidOperationException("Invalie regions set");
            }
            else if (regions == null)
            {
                region.Height = 0;
            }

            return region;
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            m_regions.Clear();
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Searches for all regions in the collection that are intersested with the current one.
        /// </summary>
        /// <param name="region">Curent text region.</param>
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
        /// <returns>TextRegion instance</returns>
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
