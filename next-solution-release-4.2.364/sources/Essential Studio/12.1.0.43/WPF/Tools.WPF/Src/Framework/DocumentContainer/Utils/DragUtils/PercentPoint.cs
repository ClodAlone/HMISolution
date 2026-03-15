// <copyright file="PercentPoint.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents point in percent's view.
    /// </summary>

    internal struct PercentPoint
    {
        #region Members
        /// <summary>
        /// Presents x axis in percent value.
        /// </summary>
        internal double m_PercentX;
        
        /// <summary>
        /// Presents y axis in percent value.
        /// </summary>
        internal double m_PercentY;
        #endregion

        #region Initializaer
        /// <summary>
        /// Initializes a new instance of the <see cref="PercentPoint"/> struct.
        /// </summary>
        /// <param name="percentX">The percent X.</param>
        /// <param name="percentY">The percent Y.</param>
        internal PercentPoint(double percentX, double percentY)
        {
            m_PercentX = percentX;
            m_PercentY = percentY;
        }
        #endregion

        #region Proeprties
        /// <summary>
        /// Gets or sets the percent X.
        /// </summary>
        /// <value>The percent X.</value>
        internal double PercentX
        {
            get
            {
                return m_PercentX;
            }

            set
            {
                m_PercentX = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the percent Y.
        /// </summary>
        /// <value>The percent Y.</value>
        internal double PercentY
        {
            get
            {
                return m_PercentY;
            }

            set
            {
                m_PercentY = value;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <remarks>Good for debug purpose</remarks>
        public override string ToString()
        {
            return string.Format("{0}, {1}", PercentX, PercentY);
        }
        #endregion
    }
}
