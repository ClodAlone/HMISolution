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

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Represents the value of LengthAdjust attribute of SVG DOM.
    /// </summary>    
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public struct ELengthAdjust
    {
        #region Members
        private string m_name;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the spacing.
        /// </summary>
        /// <value>The spacing.</value>
        public static ELengthAdjust Spacing
        {
            get
            {
                return new ELengthAdjust(SVG.VALUE_SPACING);
            }
        }

        /// <summary>
        /// Gets the spacing and glyphs.
        /// </summary>
        /// <value>The spacing and glyphs.</value>
        public static ELengthAdjust SpacingAndGlyphs
        {
            get
            {
                return new ELengthAdjust(SVG.VALUE_SPACING_AND_GLYPHS);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ELengthAdjust"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        private ELengthAdjust(string name)
        {
            m_name = name;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            return m_name;
        }

        /// <summary>
        /// Parses the specified STR.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>Returns ELengthAdjust.</returns>
        public static ELengthAdjust Parse(string str)
        {
            ELengthAdjust res = Spacing;

            if (str.IndexOf(SVG.VALUE_SPACING_AND_GLYPHS) > -1)
            {
                res = SpacingAndGlyphs;
            }

            return res;
        }
        #endregion
    }
}
