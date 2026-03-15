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
    /// Represents the value of StrokeLinejoin attribute of SVG DOM.
    /// </summary>    
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public struct EStrokeLinejoin
    {
        #region Members
        private string m_name;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the miter.
        /// </summary>
        /// <value>The miter.</value>
        public static EStrokeLinejoin Miter
        {
            get
            {
                return new EStrokeLinejoin(SVG.VALUE_MITER);
            }
        }

        /// <summary>
        /// Gets the round.
        /// </summary>
        /// <value>The round.</value>
        public static EStrokeLinejoin Round
        {
            get
            {
                return new EStrokeLinejoin(SVG.VALUE_ROUND);
            }
        }

        /// <summary>
        /// Gets the bevel.
        /// </summary>
        /// <value>The bevel.</value>
        public static EStrokeLinejoin Bevel
        {
            get
            {
                return new EStrokeLinejoin(SVG.VALUE_BEVEL);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EStrokeLinejoin"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        private EStrokeLinejoin(string name)
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
        /// <returns>Returns EStrokeLinejoin.</returns>
        public static EStrokeLinejoin Parse(string str)
        {
            EStrokeLinejoin res = EStrokeLinejoin.Miter;
            str = str.ToLower();

            if (str.IndexOf(SVG.VALUE_MITER) > -1)
            {
                res = EStrokeLinejoin.Miter;
            }
            else if (str.IndexOf(SVG.VALUE_ROUND) > -1)
            {
                res = EStrokeLinejoin.Round;
            }
            else if (str.IndexOf(SVG.VALUE_BEVEL) > -1)
            {
                res = EStrokeLinejoin.Bevel;
            }

            return res;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(EStrokeLinejoin v1, EStrokeLinejoin v2)
        {
            return v1.m_name == v2.m_name;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(EStrokeLinejoin v1, EStrokeLinejoin v2)
        {
            return v1.m_name != v2.m_name;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if obj and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            bool res = false;

            if (obj is EFontStyle)
            {
                res = ((EStrokeLinejoin)obj).m_name == m_name;
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
