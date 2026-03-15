#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// Class for EFontStyle.
    /// </summary>
    public struct EClipRule
    {
        #region Members
        private string m_name;
        #endregion

        #region Proprties
        /// <summary>
        /// Gets the nonzero.
        /// </summary>
        /// <value>The nonzero.</value>
        public static EClipRule Nonzero
        {
            get
            {
                return new EClipRule(SVG.VALUE_NONZERO);
            }
        }

        /// <summary>
        /// Gets the evenodd.
        /// </summary>
        /// <value>The evenodd.</value>
        public static EClipRule Evenodd
        {
            get
            {
                return new EClipRule(SVG.VALUE_EVENODD);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EClipRule"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        public EClipRule(string name)
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
        public static bool operator ==(EClipRule v1, EClipRule v2)
        {
            return v1.m_name == v2.m_name;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(EClipRule v1, EClipRule v2)
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

            if (obj is EClipRule)
            {
                res = ((EClipRule)obj).m_name == this.m_name;
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
