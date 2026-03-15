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
    /// Class for EFontStretch.
    /// </summary>
    public struct EFontStretch
    {
        #region Members
        private string m_name;
        #endregion

        #region Proprties
        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <value>The normal.</value>
        public static EFontStretch Normal
        {
            get
            {
                return new EFontStretch(SVG.VALUE_NORMAL);
            }
        }

        /// <summary>
        /// Gets the wider.
        /// </summary>
        /// <value>The wider.</value>
        public static EFontStretch Wider
        {
            get
            {
                return new EFontStretch(SVG.VALUE_WIDER);
            }
        }

        /// <summary>
        /// Gets the narrower.
        /// </summary>
        /// <value>The narrower.</value>
        public static EFontStretch Narrower
        {
            get
            {
                return new EFontStretch(SVG.VALUE_NARROWER);
            }
        }

        /// <summary>
        /// Gets the ultra condensed.
        /// </summary>
        /// <value>The ultra condensed.</value>
        public static EFontStretch UltraCondensed
        {
            get
            {
                return new EFontStretch(SVG.VALUE_ULTRA_CONDENSED);
            }
        }

        /// <summary>
        /// Gets the extra condensed.
        /// </summary>
        /// <value>The extra condensed.</value>
        public static EFontStretch ExtraCondensed
        {
            get
            {
                return new EFontStretch(SVG.VALUE_EXTRA_CONDENSED);
            }
        }

        /// <summary>
        /// Gets the condensed.
        /// </summary>
        /// <value>The condensed.</value>
        public static EFontStretch Condensed
        {
            get
            {
                return new EFontStretch(SVG.VALUE_CONDENSED);
            }
        }

        /// <summary>
        /// Gets the semi condensed.
        /// </summary>
        /// <value>The semi condensed.</value>
        public static EFontStretch SemiCondensed
        {
            get
            {
                return new EFontStretch(SVG.VALUE_SEMI_CONDENSED);
            }
        }

        /// <summary>
        /// Gets the semi expanded.
        /// </summary>
        /// <value>The semi expanded.</value>
        public static EFontStretch SemiExpanded
        {
            get
            {
                return new EFontStretch(SVG.VALUE_SEMI_EXPANDED);
            }
        }

        /// <summary>
        /// Gets the expanded.
        /// </summary>
        /// <value>The expanded.</value>
        public static EFontStretch Expanded
        {
            get
            {
                return new EFontStretch(SVG.VALUE_EXPANDED);
            }
        }

        /// <summary>
        /// Gets the extra expanded.
        /// </summary>
        /// <value>The extra expanded.</value>
        public static EFontStretch ExtraExpanded
        {
            get
            {
                return new EFontStretch(SVG.VALUE_EXTRA_EXPANDED);
            }
        }

        /// <summary>
        /// Gets the ultra expanded.
        /// </summary>
        /// <value>The ultra expanded.</value>
        public static EFontStretch UltraExpanded
        {
            get
            {
                return new EFontStretch(SVG.VALUE_ULTRA_EXPANDED);
            }
        }
        #endregion

        #region Consructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EFontStretch"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        private EFontStretch(string name)
        {
            m_name = name;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            return m_name;
        }

        /// <summary>
        /// Parses the specified string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>The font</returns>
        public static EFontStretch Parse(string str)
        {
            return new EFontStretch(str);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            bool res = false;

            if (obj is EFontStyle)
            {
                res = ((EFontStretch)obj).m_name == this.m_name;
            }

            return res;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
