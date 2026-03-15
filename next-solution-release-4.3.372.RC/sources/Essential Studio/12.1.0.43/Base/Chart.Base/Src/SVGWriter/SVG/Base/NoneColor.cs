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
using System.Globalization;
using System.Text.RegularExpressions;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Represents the color object of SVG DOM.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public struct NoneColor
    {
        #region Members
        private Color m_color;
        private bool m_isNone;
        private string m_primary;
        private static Regex m_colorRegex;
        private static Regex m_colorRegexPercent;
        private static Regex m_hex6ColorRegex;
        private static Regex m_hex3ColorRegex;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <value>The color.</value>
        public Color Color
        {
            get
            {
                return m_color;
            }
        }

        /// <summary>
        /// Gets the color regex.
        /// </summary>
        /// <value>The color regex.</value>
        private static Regex ColorRegex
        {
            get
            {
                if (m_colorRegex == null)
                {
                    m_colorRegex = new Regex(@"[rgb ]+\( *(?<r>\d+)[, ]+(?<g>\d+)[, ]+(?<b>\d+) *\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);                       
                }

                return m_colorRegex;
            }
        }

        /// <summary>
        /// Gets the color regex percent.
        /// </summary>
        /// <value>The color regex percent.</value>
        private static Regex ColorRegexPercent
        {
            get
            {
                if (m_colorRegexPercent == null)
                {
                    m_colorRegexPercent = new Regex(@"[rgb ]+\( *(?<r>\d+)%[, ]+(?<g>\d+)%[, ]+(?<b>\d+)% *\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);                        
                }

                return m_colorRegexPercent;
            }
        }

        /// <summary>
        /// Gets the hex6 color regex.
        /// </summary>
        /// <value>The hex6 color regex.</value>
        private static Regex Hex6ColorRegex
        {
            get
            {
                if (m_hex6ColorRegex == null)
                {
                    m_hex6ColorRegex = new Regex(@"^[ ]*[#](?<r>[0-9a-f]{2})(?<g>[0-9a-f]{2})(?<b>[0-9a-f]{2})", RegexOptions.Compiled | RegexOptions.IgnoreCase);                       
                }

                return m_hex6ColorRegex;
            }
        }

        /// <summary>
        /// Gets the hex3 color regex.
        /// </summary>
        /// <value>The hex3 color regex.</value>
        private static Regex Hex3ColorRegex
        {
            get
            {
                if (m_hex3ColorRegex == null)
                {
                    m_hex3ColorRegex = new Regex(@"^[ ]*[#](?<r>[0-9a-f])(?<g>[0-9a-f])(?<b>[0-9a-f])", RegexOptions.Compiled | RegexOptions.IgnoreCase);                        
                }

                return m_hex3ColorRegex;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is none.
        /// </summary>
        /// <value><c>true</c> if this instance is none; otherwise, <c>false</c>.</value>
        public bool IsNone
        {
            get
            {
                return m_isNone;
            }
        }

        /// <summary>
        /// Gets the primaty value.
        /// </summary>
        /// <value>The primaty value.</value>
        public string Primaty
        {
            get
            {
                return m_primary;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="NoneColor"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public NoneColor(Color color)
        {
            m_color = color;
            m_primary = null;

            if (m_color.IsEmpty)
            {
                m_isNone = true;
            }
            else
            {
                m_isNone = false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoneColor"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public NoneColor(string color)
        {
            m_color = Color.Empty;
            m_isNone = false;
            m_primary = null;
            ParseString(color);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Drawing.Color"/> to <see cref="Syncfusion.Windows.Forms.Chart.SvgBase.NoneColor"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator NoneColor(Color color)
        {
            return new NoneColor(color);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Syncfusion.Windows.Forms.Chart.SvgBase.NoneColor"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator NoneColor(string color)
        {
            return new NoneColor(color);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            string res = "";

            if (IsNone)
            {
                res = SVG.VALUE_NONE;
            }
            else if (!m_color.IsEmpty)
            {
                res = SVG.VALUE_RGB_COLOR + "("
                    + m_color.R.ToString() + ","
                    + m_color.G.ToString() + ","
                    + m_color.B.ToString() + ")";
            }
            else
            {
                res = m_primary;
            }

            return res;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the string.
        /// </summary>
        /// <param name="color">The color.</param>
        private void ParseString(string color)
        {
            Match rgb = ColorRegex.Match(color);
            Match rgbPercent = ColorRegexPercent.Match(color);
            Match hex3 = Hex3ColorRegex.Match(color);
            Match hex6 = Hex6ColorRegex.Match(color);

            if (rgb.Success)
            {
                int r = int.Parse(rgb.Groups[SVG.VALUE_R].Captures[0].Value);
                int g = int.Parse(rgb.Groups[SVG.VALUE_G].Captures[0].Value);
                int b = int.Parse(rgb.Groups[SVG.VALUE_B].Captures[0].Value);

                m_color = Color.FromArgb(r, g, b);
            }
            else if (rgbPercent.Success)
            {
                int r = int.Parse(rgbPercent.Groups[SVG.VALUE_R].Captures[0].Value);
                int g = int.Parse(rgbPercent.Groups[SVG.VALUE_G].Captures[0].Value);
                int b = int.Parse(rgbPercent.Groups[SVG.VALUE_B].Captures[0].Value);

                m_color = Color.FromArgb(byte.MaxValue * r / 100, byte.MaxValue * g / 100, byte.MaxValue * b / 100);
            }
            else if (hex6.Success)
            {
                int r = int.Parse(hex6.Groups[SVG.VALUE_R].Captures[0].Value, NumberStyles.HexNumber);
                int g = int.Parse(hex6.Groups[SVG.VALUE_G].Captures[0].Value, NumberStyles.HexNumber);
                int b = int.Parse(hex6.Groups[SVG.VALUE_B].Captures[0].Value, NumberStyles.HexNumber);

                m_color = Color.FromArgb(r, g, b);
            }
            else if (hex3.Success)
            {
                int r = int.Parse(hex3.Groups[SVG.VALUE_R].Captures[0].Value, NumberStyles.HexNumber);
                int g = int.Parse(hex3.Groups[SVG.VALUE_G].Captures[0].Value, NumberStyles.HexNumber);
                int b = int.Parse(hex3.Groups[SVG.VALUE_B].Captures[0].Value, NumberStyles.HexNumber);

                m_color = Color.FromArgb(r << 4, g << 4, b << 4);
            }
            else if (color.IndexOf(SVG.VALUE_NONE) > -1)
            {
                m_isNone = true;
                m_color = Color.Empty;
            }
            else
            {
                m_primary = color;
                m_color = Color.Empty;
            }
        }
        #endregion
    }
}
