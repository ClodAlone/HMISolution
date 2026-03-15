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

using System;
using System.Collections;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Represents the node of BSP tree.
    /// </summary>
    internal sealed class BspNode
    {
        #region Members
        private Polygon m_plane;
        private BspNode m_front;
        private BspNode m_back;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the back node.
        /// </summary>
        /// <value>The back node.</value>
        public BspNode Back
        {
            get
            {
                return m_back;
            }

            set
            {
                m_back = value;
            }
        }

        /// <summary>
        /// Gets or sets the front node.
        /// </summary>
        /// <value>The front node.</value>
        public BspNode Front
        {
            get
            {
                return m_front;
            }

            set
            {
                m_front = value;
            }
        }

        /// <summary>
        /// Gets or sets the plane.
        /// </summary>
        /// <value>The plane.</value>
        public Polygon Plane
        {
            get
            {
                return m_plane;
            }

            set
            {
                m_plane = value;
            }
        }
        #endregion
    }
}
