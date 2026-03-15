#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// Class for Length.
    /// </summary>
    public struct Length
    {
        #region Constants
        private const float MAX_VALUE = 8388607f;
        #endregion

        #region Members
        private float m_value;
        private LengthType m_type;
        private string m_primary;
        private bool m_isEmpty;
        private static Regex m_lengthRegex = null;
        #endregion

        #region Proprties
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public LengthType Type
        {
            get
            {
                return m_type;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public float Value
        {
            get
            {
                return m_value;
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
        public static Length Empty
        {
            get
            {
                Length res = new Length(0);
                res.m_isEmpty = true;

                return res;
            }
        }

        /// <summary>
        /// Gets the primary.
        /// </summary>
        /// <value>The primary.</value>
        public string Primary
        {
            get
            {
                return m_primary;
            }
        }

        /// <summary>
        /// Gets the length regex.
        /// </summary>
        /// <value>The length regex.</value>
        public static Regex LengthRegex
        {
            get
            {
                if (m_lengthRegex == null)
                {
                    m_lengthRegex = new Regex(
                      @"(?<number>[-]?[0-9]+([.][0-?9]+)?(\s*e[+-][0-9]+)?)(?<type>%|em|ex|px|cm|mm|in|pt|pc)?",
                      RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }

                return m_lengthRegex;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Length"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Length(float value)
        {
            m_primary = string.Empty;
            m_value = value;
            m_type = LengthType.Unknown;
            m_isEmpty = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        public Length(float value, LengthType type)
        {
            m_primary = string.Empty;
            m_value = value;
            m_type = type;
            m_isEmpty = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Length"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Length(string value)
        {
            m_primary = value;
            m_type = LengthType.Unknown;
            m_value = 0;
            m_isEmpty = false;

            ParseString(value);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The length.</returns>
        public static Length Parse(string value)
        {
            return new Length(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Syncfusion.SVG.IO.Length"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Length(string value)
        {
            return new Length(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="Syncfusion.SVG.IO.Length"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Length(int value)
        {
            return new Length(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Single"/> to <see cref="Syncfusion.SVG.IO.Length"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Length(float value)
        {
            return new Length(value);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Syncfusion.SVG.IO.Length"/> to <see cref="System.Single"/>.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator float(Length length)
        {
            return length.Value;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            string suf = string.Empty;

            switch (m_type)
            {
                case LengthType.Percentage:
                    suf = SVG.VALUE_PERCENT;
                    break;
                case LengthType.CM:
                    suf = SVG.VALUE_CM;
                    break;
                case LengthType.EMS:
                    suf = SVG.VALUE_EM;
                    break;
                case LengthType.EXS:
                    suf = SVG.VALUE_EX;
                    break;
                case LengthType.IN:
                    suf = SVG.VALUE_IN;
                    break;
                case LengthType.MM:
                    suf = SVG.VALUE_MM;
                    break;
                case LengthType.PC:
                    suf = SVG.VALUE_PC;
                    break;
                case LengthType.PT:
                    suf = SVG.VALUE_PT;
                    break;
                case LengthType.PX:
                    suf = SVG.VALUE_PX;
                    break;
            }

            return Utility.GetFloat(m_value) + suf;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="max">The max.</param>
        /// <returns>The value.</returns>
        public float GetValue(float max)
        {
            float res = (Type == LengthType.Percentage) ? m_value * max / 100f : m_value;

            return (res > MAX_VALUE) ? MAX_VALUE : res;
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
            bool result = base.Equals(obj);

            if (obj is Length)
            {
                result = (m_type == ((Length)obj).m_type) && (m_value == ((Length)obj).m_value);
            }

            return result;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return m_type.GetHashCode() ^ m_value.GetHashCode();
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the string.
        /// </summary>
        /// <param name="value">The value.</param>
        private void ParseString(string value)
        {
            Match lgth = LengthRegex.Match(value);

            if (lgth.Success)
            {
                m_value = Utility.GetFloat(lgth.Groups["number"].Value);

                switch (lgth.Groups["type"].Value)
                {
                    case SVG.VALUE_PERCENT:
                        m_type = LengthType.Percentage;
                        break;
                    case SVG.VALUE_CM:
                        m_type = LengthType.CM;
                        break;
                    case SVG.VALUE_EM:
                        m_type = LengthType.EMS;
                        break;
                    case SVG.VALUE_EX:
                        m_type = LengthType.EXS;
                        break;
                    case SVG.VALUE_IN:
                        m_type = LengthType.IN;
                        break;
                    case SVG.VALUE_MM:
                        m_type = LengthType.MM;
                        break;
                    case SVG.VALUE_PC:
                        m_type = LengthType.PC;
                        break;
                    case SVG.VALUE_PT:
                        m_type = LengthType.PT;
                        break;
                    case SVG.VALUE_PX:
                        m_type = LengthType.PX;
                        break;
                    default:
                        m_type = LengthType.Unknown;
                        break;
                }
            }
        }
        #endregion
    }
}
