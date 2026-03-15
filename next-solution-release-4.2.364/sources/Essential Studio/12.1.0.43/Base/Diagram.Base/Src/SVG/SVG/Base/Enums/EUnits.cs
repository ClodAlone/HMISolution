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
    /// EUnits class.
    /// </summary>
    public struct EUnits
    {
        #region Members
        private string m_name;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the user space on use.
        /// </summary>
        /// <value>The user space on use.</value>
        public static EUnits UserSpaceOnUse
        {
            get
            {
                return new EUnits(SVG.VALUE_USER_SPACE_ON_USE);
            }
        }

        /// <summary>
        /// Gets the object bounding box.
        /// </summary>
        /// <value>The object bounding box.</value>
        public static EUnits ObjectBoundingBox
        {
            get
            {
                return new EUnits(SVG.VALUE_OBJECT_BOUND_BOX);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EUnits"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        private EUnits(string name)
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
        /// <param name="str">The string.</param>
        /// <returns>The units</returns>
        public static EUnits Parse(string str)
        {
            EUnits res = UserSpaceOnUse;

            if (str.IndexOf(SVG.VALUE_OBJECT_BOUND_BOX) > -1)
            {
                res = ObjectBoundingBox;
            }

            return res;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(EUnits v1, EUnits v2)
        {
            return v1.m_name == v2.m_name;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(EUnits v1, EUnits v2)
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
            bool res = base.Equals(obj);

            if (obj is EUnits)
            {
                res = ((EUnits)obj).m_name == m_name;
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
