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
    /// Represents the value of FontWeight attribute of SVG DOM.
    /// </summary>    
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public struct EFontWeight
    {
        #region Members
        private string m_name;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <value>The normal.</value>
        public static EFontWeight Normal
        {
            get
            {
                return new EFontWeight(SVG.VALUE_NORMAL);
            }
        }

        /// <summary>
        /// Gets the bold.
        /// </summary>
        /// <value>The bold.</value>
        public static EFontWeight Bold
        {
            get
            {
                return new EFontWeight(SVG.VALUE_BOLD);
            }
        }

        /// <summary>
        /// Gets the bolder.
        /// </summary>
        /// <value>The bolder.</value>
        public static EFontWeight Bolder
        {
            get
            {
                return new EFontWeight(SVG.VALUE_BOLDER);
            }
        }

        /// <summary>
        /// Gets the lighter.
        /// </summary>
        /// <value>The lighter.</value>
        public static EFontWeight Lighter
        {
            get
            {
                return new EFontWeight(SVG.VALUE_LIGHTER);
            }
        }

        /// <summary>
        /// Gets the value100.
        /// </summary>
        /// <value>The value100.</value>
        public static EFontWeight Value100
        {
            get
            {
                return new EFontWeight(SVG.VALUE_100);
            }
        }

        /// <summary>
        /// Gets the value200.
        /// </summary>
        /// <value>The value200.</value>
        public static EFontWeight Value200
        {
            get
            {
                return new EFontWeight(SVG.VALUE_200);
            }
        }

        /// <summary>
        /// Gets the value300.
        /// </summary>
        /// <value>The value300.</value>
        public static EFontWeight Value300
        {
            get
            {
                return new EFontWeight(SVG.VALUE_300);
            }
        }

        /// <summary>
        /// Gets the value400.
        /// </summary>
        /// <value>The value400.</value>
        public static EFontWeight Value400
        {
            get
            {
                return new EFontWeight(SVG.VALUE_400);
            }
        }

        /// <summary>
        /// Gets the value500.
        /// </summary>
        /// <value>The value500.</value>
        public static EFontWeight Value500
        {
            get
            {
                return new EFontWeight(SVG.VALUE_500);
            }
        }

        /// <summary>
        /// Gets the value600.
        /// </summary>
        /// <value>The value600.</value>
        public static EFontWeight Value600
        {
            get
            {
                return new EFontWeight(SVG.VALUE_600);
            }
        }

        /// <summary>
        /// Gets the value700.
        /// </summary>
        /// <value>The value700.</value>
        public static EFontWeight Value700
        {
            get
            {
                return new EFontWeight(SVG.VALUE_700);
            }
        }

        /// <summary>
        /// Gets the value800.
        /// </summary>
        /// <value>The value800.</value>
        public static EFontWeight Value800
        {
            get
            {
                return new EFontWeight(SVG.VALUE_800);
            }
        }

        /// <summary>
        /// Gets the value900.
        /// </summary>
        /// <value>The value900.</value>
        public static EFontWeight Value900
        {
            get
            {
                return new EFontWeight(SVG.VALUE_900);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EFontWeight"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        private EFontWeight(string name)
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
        /// Implements the operator ==.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(EFontWeight v1, EFontWeight v2)
        {
            return v1.m_name == v2.m_name;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(EFontWeight v1, EFontWeight v2)
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
                res = ((EFontWeight)obj).m_name == m_name;
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
