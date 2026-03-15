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
    /// Class for EFontStyle.
    /// </summary>
    public struct EFontStyle
    {
        #region Members
        private string m_name;
        #endregion

        #region Proprties
        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <value>The normal.</value>
        public static EFontStyle Normal
        {
            get
            {
                return new EFontStyle(SVG.VALUE_NORMAL);
            }
        }

        /// <summary>
        /// Gets the italic.
        /// </summary>
        /// <value>The italic.</value>
        public static EFontStyle Italic
        {
            get
            {
                return new EFontStyle(SVG.VALUE_ITALIC);
            }
        }

        /// <summary>
        /// Gets the oblique.
        /// </summary>
        /// <value>The oblique.</value>
        public static EFontStyle Oblique
        {
            get
            {
                return new EFontStyle(SVG.VALUE_OBLIQUE);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EFontStyle"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        public EFontStyle(string name)
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
        /// Implements the operator ==.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(EFontStyle v1, EFontStyle v2)
        {
            return v1.m_name == v2.m_name;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(EFontStyle v1, EFontStyle v2)
        {
            return v1.m_name != v2.m_name;
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
                res = ((EFontStyle)obj).m_name == this.m_name;
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
