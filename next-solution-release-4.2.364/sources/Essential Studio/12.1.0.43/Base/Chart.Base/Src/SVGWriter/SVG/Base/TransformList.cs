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
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Contanins the 2D transformation.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class TransformList
    {
        #region Members
        private Matrix m_matrix;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the matrix.
        /// </summary>
        /// <value>The matrix.</value>
        public Matrix Matrix
        {
            get
            {
                return m_matrix;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformList"/> class.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public TransformList(Matrix matrix)
        {
            m_matrix = matrix;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformList"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public TransformList(string value)
        {
            ParseString(value);
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

            for (int i = 0; i < 6; i++)
            {
                res += Utility.GetFloat(m_matrix.Elements[i]) + " ";
            }

            return SVG.VALUE_MATRIX + "(" + res + ")";
        }
        #endregion

        #region Helper methdos
        /// <summary>
        /// Parses the string.
        /// </summary>
        /// <param name="value">The value.</param>
        private void ParseString(string value)
        {
            if (value.IndexOf(SVG.VALUE_MATRIX) > -1)
            {
                float[] m = Utility.GetNumbers(value);

                m_matrix = new Matrix(m[0], m[1], m[2], m[3], m[4], m[5]);
            }
            else if (value.IndexOf(SVG.VALUE_TRANSLATE) > -1)
            {
                m_matrix = new Matrix();
                float[] m = Utility.GetNumbers(value);

                if (m.Length > 1)
                {
                    m_matrix.Translate(m[0], m[1]);
                }
                else
                {
                    m_matrix.Translate(m[0], 0);
                }
            }
            else if (value.IndexOf(SVG.VALUE_SCALE) > -1)
            {
                m_matrix = new Matrix();
                float[] m = Utility.GetNumbers(value);

                if (m.Length > 1)
                {
                    m_matrix.Scale(m[0], m[1]);
                }
                else
                {
                    m_matrix.Scale(m[0], 0);
                }
            }
            else if (value.IndexOf(SVG.VALUE_ROTATE) > -1)
            {
                m_matrix = new Matrix();
                float[] m = Utility.GetNumbers(value);

                if (m.Length > 2)
                {
                    m_matrix.Translate(m[1], m[2]);
                    m_matrix.Rotate(m[0]);
                    m_matrix.Translate(-m[1], -m[2]);
                }
                else if (m.Length < 2)
                {
                    m_matrix.Rotate(m[0]);
                }
            }
            else if (value.IndexOf(SVG.VALUE_SKEW_X) > -1)
            {
                m_matrix = new Matrix();
                float m = Utility.GetNumber(value);
                m_matrix.Shear(m, 0);
            }
            else if (value.IndexOf(SVG.VALUE_SKEW_Y) > -1)
            {
                m_matrix = new Matrix();
                float m = Utility.GetNumber(value);
                m_matrix.Shear(0, m);
            }
        }
        #endregion
    }
}
