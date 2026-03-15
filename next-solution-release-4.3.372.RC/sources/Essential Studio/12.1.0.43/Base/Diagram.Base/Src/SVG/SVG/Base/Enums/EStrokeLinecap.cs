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
    /// EStrokeLinecap class.
    /// </summary>
    public class EStrokeLinecap
    {
        #region Members
        private string m_name;
        #endregion

        #region Prorpties
        /// <summary>
        /// Gets the stroke line cap.
        /// </summary>
        /// <value>The stroke line cap.</value>
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
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return m_name;
        }

        /// <summary>
        /// Parses the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The stroke line.</returns>
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
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            bool res = false;

            if (obj is EFontStyle)
            {
                res = ((EStrokeLinecap)obj).m_name == this.m_name;
            }

            return res;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
