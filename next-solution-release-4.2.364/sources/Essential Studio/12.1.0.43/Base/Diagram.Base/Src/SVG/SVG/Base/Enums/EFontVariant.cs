#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// Class for EFontVariant.
    /// </summary>
    public struct EFontVariant
    {
        #region Members
        private string m_name;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <value>The normal.</value>
        public static EFontVariant Normal
        {
            get
            {
                return new EFontVariant(SVG.VALUE_NORMAL);
            }
        }

        /// <summary>
        /// Gets the small caps.
        /// </summary>
        /// <value>The small caps.</value>
        public static EFontVariant SmallCaps
        {
            get
            {
                return new EFontVariant(SVG.VALUE_SMALL_CAPS);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EFontVariant"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        private EFontVariant(string name)
        {
            m_name = name;
        }
        #endregion
    }
}
