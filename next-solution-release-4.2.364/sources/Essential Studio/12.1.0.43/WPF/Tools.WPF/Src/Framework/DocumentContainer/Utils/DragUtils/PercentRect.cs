// <copyright file="PercentRect.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents rect in percent's view.
    /// </summary>

    internal struct PercentRect
    {
        #region Members
        /// <summary>
        /// Presents x axis in percent value.
        /// </summary>
        private double m_percentX;
        
        /// <summary>
        /// Presents y axis in percent value.
        /// </summary>
        private double m_percentY;
        
        /// <summary>
        /// Presents width in percent value.
        /// </summary>
        private double m_percentWidth;
        
        /// <summary>
        /// Presents height in percent value.
        /// </summary>
        private double m_percentHeight;

        /// <summary>
        /// Presents empty value.
        /// </summary>
        private static readonly PercentRect m_Empty;
        #endregion

        #region Initializaer
        /// <summary>
        /// Initializes static members of the <see cref="PercentRect"/> struct.
        /// </summary>
        static PercentRect()
        {
            m_Empty = CreateEmptyRect();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PercentRect"/> struct.
        /// </summary>
        /// <param name="percentX">The percent X.</param>
        /// <param name="percentY">The percent Y.</param>
        /// <param name="percentWidth">Width of the percent.</param>
        /// <param name="percentHeight">Height of the percent.</param>
        internal PercentRect(double percentX, double percentY, double percentWidth, double percentHeight)
        {
            m_percentX = percentX;
            m_percentY = percentY;
            m_percentWidth = percentWidth;
            m_percentHeight = percentHeight;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the percent's X.
        /// </summary>
        /// <value>The percent X.</value>
        internal double PercentX
        {
            get
            {
                return m_percentX;
            }

            set
            {
                m_percentX = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the percent's Y.
        /// </summary>
        /// <value>The percent Y.</value>
        internal double PercentY
        {
            get
            {
                return m_percentY;
            }

            set
            {
                m_percentY = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the percent of the width.
        /// </summary>
        /// <value>The width of the percent.</value>
        internal double PercentWidth
        {
            get
            {
                return m_percentWidth;
            }

            set
            {
                m_percentWidth = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the percent  of the height.
        /// </summary>
        /// <value>The height of the percent.</value>
        internal double PercentHeight
        {
            get
            {
                return m_percentHeight;
            }

            set
            {
                m_percentHeight = value;
            }
        }

        /// <summary>
        /// Gets the empty.
        /// </summary>
        /// <value>The empty.</value>
        internal static PercentRect Empty
        {
            get
            {
                return m_Empty;
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
        /// <remarks>Good for debug purpose.</remarks>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", PercentX, PercentY, PercentWidth, PercentHeight);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates the empty rect.
        /// </summary>
        /// <returns>PercentRect rect </returns>
        private static PercentRect CreateEmptyRect()
        {
            PercentRect rect = new PercentRect
            {
                PercentX = double.PositiveInfinity,
                PercentY = double.PositiveInfinity,
                PercentWidth = double.NegativeInfinity,
                PercentHeight = double.NegativeInfinity
            };
            return rect;
        }
        #endregion
    }
}