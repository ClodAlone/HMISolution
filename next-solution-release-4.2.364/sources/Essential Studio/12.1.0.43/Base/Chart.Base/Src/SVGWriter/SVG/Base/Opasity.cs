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

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Represents the value of Opacity attribute.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public struct Opacity
    {
        #region Members
        private float m_value;
        #endregion

        #region Prorerties
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public float Value
        {
            get
            {
                return m_value;
            }

            set
            {
                m_value = (value < 0) ? 0 : (value > 1f ? 1f : value);
            }
        }

        /// <summary>
        /// Gets or sets the alpha.
        /// </summary>
        /// <value>The alpha.</value>
        public byte Alpha
        {
            get
            {
                return (byte)(byte.MaxValue * m_value);
            }

            set
            {
                m_value = value / (float)byte.MaxValue;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Opacity"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Opacity(float value)
        {
            m_value = (value < 0) ? 0 : (value > 1f ? 1f : value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Opacity"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Opacity(string value)
        {
            float val = Utility.GetFloat(value);
            m_value = (val < 0) ? 0 : (val > 1f ? 1f : val);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Opacity"/> struct.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        public Opacity(byte alpha)
        {
            m_value = alpha / (float)byte.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Opacity"/> struct.
        /// </summary>
        /// <param name="color">The color.</param>
        public Opacity(Color color)
            : this(color.A)
        {
        }
        #endregion

        #region Public methdos
        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Single"/> to <see cref="Syncfusion.Windows.Forms.Chart.SvgBase.Opacity"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Opacity(float value)
        {
            return new Opacity(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Byte"/> to <see cref="Syncfusion.Windows.Forms.Chart.SvgBase.Opacity"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Opacity(byte value)
        {
            return new Opacity(value);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            return Utility.GetFloat(m_value);
        }

        /// <summary>
        /// Parses the specified string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Opacity Parse(string value)
        {
            return new Opacity(Utility.GetFloat(value));
        }
        #endregion
    }
}
