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
using System.Collections;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Implements the <see cref="float"/> array.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class FloatArray
    {
        #region Members
        /// <summary>
        /// The array float numbers.
        /// </summary>
        protected float[] m_array;
        #endregion

        #region Proprties
        /// <summary>
        /// Gets the array.
        /// </summary>
        /// <value>The array.</value>
        public float[] Array
        {
            get
            {
                return m_array;
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Single"/> at the specified index.
        /// </summary>
        /// <value></value>
        public float this[int index]
        {
            get
            {
                return m_array[index];
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FloatArray"/> class.
        /// </summary>
        /// <param name="array">The array.</param>
        public FloatArray(float[] array)
        {
            m_array = array;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatArray"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public FloatArray(string value)
        {
            ParseString(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatArray"/> class.
        /// </summary>
        protected FloatArray()
        {
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
            string res = "";

            for (int i = 0; i < m_array.Length; i++)
            {
                res += Utility.GetFloat(m_array[i]) + " ";
            }

            return res;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Single"/> to <see cref="Syncfusion.Windows.Forms.Chart.SvgBase.FloatArray"/>.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator FloatArray(float[] array)
        {
            return new FloatArray(array);
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the string.
        /// </summary>
        /// <param name="value">The value.</param>
        private void ParseString(string value)
        {
            m_array = Utility.GetNumbers(value);
        }
        #endregion
    }
}
