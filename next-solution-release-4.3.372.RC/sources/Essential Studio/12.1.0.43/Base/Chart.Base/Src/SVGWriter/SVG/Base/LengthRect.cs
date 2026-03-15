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
using System.Drawing;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Stores the location and size of a rectangle.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public struct LengthRect
    {
        #region Members
        private Length m_x;
        private Length m_y;
        private Length m_width;
        private Length m_height;
        private string m_primary;
        private bool m_isEmpty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        /// <value>The X.</value>
        public Length X
        {
            get
            {
                return m_x;
            }
        }

        /// <summary>
        /// Gets the Y coordinate.
        /// </summary>
        /// <value>The Y.</value>
        public Length Y
        {
            get
            {
                return m_y;
            }
        }

        /// <summary>
        /// Gets the width of rectangle.
        /// </summary>
        /// <value>The width.</value>
        public Length Width
        {
            get
            {
                return m_width;
            }
        }

        /// <summary>
        /// Gets the height of rectangle.
        /// </summary>
        /// <value>The height.</value>
        public Length Height
        {
            get
            {
                return m_height;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return m_isEmpty;
            }
        }

        /// <summary>
        /// Gets the empty.
        /// </summary>
        /// <value>The empty.</value>
        public static LengthRect Empty
        {
            get
            {
                LengthRect res = new LengthRect("");
                res.m_isEmpty = true;

                return res;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthRect"/> struct.
        /// </summary>
        /// <param name="rect">The rect.</param>
        public LengthRect(RectangleF rect)
        {
            m_isEmpty = false;
            m_x = new Length(rect.X);
            m_y = new Length(rect.Y);
            m_width = new Length(rect.Width);
            m_height = new Length(rect.Height);
            m_primary = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthRect"/> struct.
        /// </summary>
        /// <param name="primary">The primary.</param>
        public LengthRect(string primary)
        {
            m_isEmpty = false;
            m_primary = primary;
            m_x = Length.Empty;
            m_y = Length.Empty;
            m_width = Length.Empty;
            m_height = Length.Empty;
            ParseString(primary);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LengthRect"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public LengthRect(Length x, Length y, Length width, Length height)
        {
            m_isEmpty = false;
            m_x = x;
            m_y = y;
            m_width = width;
            m_height = height;
            m_primary = "";
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Returns the new LengthRect instance.</returns>
        public static LengthRect Parse(string value)
        {
            return new LengthRect(value);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            return m_x.ToString() + " " + m_y.ToString() + " " +
                m_width.ToString() + " " + m_height.ToString();
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the string.
        /// </summary>
        /// <param name="str">The text.</param>
        private void ParseString(string str)
        {
            MatchCollection mchs = Length.LengthRegex.Matches(str);

            if (mchs.Count != 0)
            {
                m_x = new Length(mchs[0].Value);
                m_y = new Length(mchs[1].Value);
                m_width = new Length(mchs[2].Value);
                m_height = new Length(mchs[3].Value);
            }
        }
        #endregion
    }
}