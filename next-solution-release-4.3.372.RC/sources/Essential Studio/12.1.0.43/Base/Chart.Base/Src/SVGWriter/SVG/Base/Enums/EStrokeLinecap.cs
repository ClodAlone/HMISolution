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
    /// Represents the value of StrokeLinecap attribute of SVG DOM.
    /// </summary>    
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class EStrokeLinecap
    {
        #region Members
        /// <summary>
        /// The Name.
        /// </summary>
        private string m_name;
        #endregion

        #region Prorpties
        /// <summary>
        /// Gets the butt.
        /// </summary>
        /// <value>The butt.</value>
        public static EStrokeLinecap Butt
        {
            get
            {
                return new EStrokeLinecap(SVG.VALUE_BUTT);
            }
        }

        /// <summary>
        /// Gets the round.
        /// </summary>
        /// <value>The round.</value>
        public static EStrokeLinecap Round
        {
            get
            {
                return new EStrokeLinecap(SVG.VALUE_ROUND);
            }
        }

        /// <summary>
        /// Gets the square.
        /// </summary>
        /// <value>The square.</value>
        public static EStrokeLinecap Square
        {
            get
            {
                return new EStrokeLinecap(SVG.VALUE_SQUARE);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EStrokeLinecap"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        private EStrokeLinecap(string name)
        {
            m_name = name;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return m_name;
        }

        /// <summary>
        /// Parses the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>Returns EStrokeLinecap.</returns>
        public static EStrokeLinecap Parse(string str)
        {
            EStrokeLinecap res = EStrokeLinecap.Butt;
            str = str.ToLower();

            if (str.IndexOf(SVG.VALUE_BUTT) > -1)
            {
                res = EStrokeLinecap.Butt;
            }
            else if (str.IndexOf(SVG.VALUE_ROUND) > -1)
            {
                res = EStrokeLinecap.Round;
            }
            else if (str.IndexOf(SVG.VALUE_SQUARE) > -1)
            {
                res = EStrokeLinecap.Square;
            }

            return res;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(EStrokeLinecap v1, EStrokeLinecap v2)
        {
            return v1.m_name == v2.m_name;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(EStrokeLinecap v1, EStrokeLinecap v2)
        {
            return v1.m_name != v2.m_name;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            bool res = false;

            if (obj is EFontStyle)
            {
                res = ((EStrokeLinecap)obj).m_name == m_name;
            }

            return res;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
