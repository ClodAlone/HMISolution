#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// Class for Number List.
    /// </summary>
    public class FloatArray
    {
        #region Members
        protected float[] m_array;
        #endregion

        #region Properties
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
        /// <param name="index">The index.</param>
        /// <value>The value at the index.</value>
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
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            string res = string.Empty;

            for (int i = 0; i < m_array.Length; i++)
            {
                res += Utility.GetFloat(m_array[i]) + " ";
            }

            return res;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:System.Single[]"/> to <see cref="Syncfusion.SVG.IO.FloatArray"/>.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator FloatArray(float[] array)
        {
            return new FloatArray(array);
        }
        #endregion

        #region Helper methods
        private void ParseString(string value)
        {
            m_array = Utility.GetNumbers(value);
        }
        #endregion
    }
}
