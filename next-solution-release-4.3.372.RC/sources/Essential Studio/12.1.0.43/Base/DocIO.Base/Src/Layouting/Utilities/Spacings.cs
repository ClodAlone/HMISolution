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

#if !SILVERLIGHT

namespace Syncfusion.Layouting
{
    /// <summary>
    /// Implements a Rectangle like behavior.
    /// </summary>
    internal class Spacings
    {
        #region Fields
        private double m_left = 0;
        private double m_top = 0;
        private double m_right = 0;
        private double m_bottom = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public double Left
        {
            get
            {
                return m_left;
            }

            set
            {
                m_left = value;
            }
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public double Top
        {
            get
            {
                return m_top;
            }

            set
            {
                m_top = value;
            }
        }

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>The right.</value>
        public double Right
        {
            get
            {
                return m_right;
            }

            set
            {
                m_right = value;
            }
        }

        /// <summary>
        /// Gets or sets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        public double Bottom
        {
            get
            {
                return m_bottom;
            }

            set
            {
                m_bottom = value;
            }
        }
        #endregion
    }
}

#endif