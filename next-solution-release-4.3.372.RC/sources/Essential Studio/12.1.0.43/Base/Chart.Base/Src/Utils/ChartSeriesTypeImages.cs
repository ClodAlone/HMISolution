#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Syncfusion.Windows.Forms.Chart.Utils
{
    /// <summary>
    /// Contains the icons of <see cref="ChartSeriesType"/>.
    /// </summary>
    public static class ChartSeriesTypeImages
    {
        #region Constants
        private const string c_resourceName = "Syncfusion.Windows.Forms.Chart.Utils.ChartSeriesTypeImages";
        #endregion

        #region Members
        private readonly static ResourceManager m_manager;
        private readonly static Hashtable m_images = new Hashtable();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="ChartSeriesTypeImages"/> class.
        /// </summary>
        static ChartSeriesTypeImages()
        {
            m_manager = new ResourceManager(c_resourceName, Assembly.GetAssembly(typeof(ChartSeriesTypeImages)));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the icon image by the specified <see cref="ChartSeriesType"/>.
        /// </summary>
        /// <param name="type">The <see cref="ChartSeriesType"/>.</param>
        /// <returns>Returns Image.</returns>
        public static Image GetImage(ChartSeriesType type)
        {
            if (!m_images.Contains(type))
            {
                m_images[type] = m_manager.GetObject(string.Format("{0}.gif", type));
            }

            return m_images[type] as Image;
        }
        #endregion
    }
}
