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
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public struct Number
    {
        #region Members
        private float m_value;
        private string m_primary;
        private bool m_isEmpty;
        #endregion

        #region Proprties
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
        /// Gets the string value of number.
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
        /// Gets the empty <see cref="Number"/>.
        /// </summary>
        /// <value>The empty.</value>
        public static Number Empty
        {
            get
            {
                Number res = new Number(0);
                res.m_isEmpty = true;

                return res;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Number(float value)
        {
            m_primary = "";
            m_value = value;
            m_isEmpty = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Number"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Number(string value)
        {
            m_primary = value;
            m_value = Utility.GetNumber(value);
            m_isEmpty = false;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Number Parse(string value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Syncfusion.Windows.Forms.Chart.SvgBase.Number"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Number(string value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="Syncfusion.Windows.Forms.Chart.SvgBase.Number"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Number(int value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Single"/> to <see cref="Syncfusion.Windows.Forms.Chart.SvgBase.Number"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Number(float value)
        {
            return new Number(value);
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            return m_isEmpty ? SVG.VALUE_NONE :
                (m_primary != "" ? m_primary : Utility.GetFloat(m_value));
        }
        #endregion
    }
}
