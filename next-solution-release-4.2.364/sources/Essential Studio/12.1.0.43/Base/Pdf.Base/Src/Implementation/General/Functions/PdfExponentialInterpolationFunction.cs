#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Functions
{
    /// <summary>
    /// Implements PDF Exponential Interpolation Function.
    /// </summary>
    public class PdfExponentialInterpolationFunction : PdfFunction
    {
        #region Fields
        /// <summary>
        /// Local variable to store the function result when x = 0.
        /// </summary>
        protected float[] m_c0;

        /// <summary>
        /// Local variable to store the function result when x = 1.
        /// </summary>
        protected float[] m_c1;

        /// <summary>
        /// Local variable to store the interpolation exponent.
        /// </summary>
        private float m_interpolationExp;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfExponentialInterpolationFunction"/> class.
        /// </summary>
        /// <param name="Init">init</param>
        public PdfExponentialInterpolationFunction(bool Init)
            : base(new PdfDictionary())
        {
            m_interpolationExp = 1;
            float[] numArray = new float[2];
            numArray[1] = 1f;
            base.Domain = new PdfArray(numArray);
            base.Range = new PdfArray(new float[] { 0f, 1f, 0f, 1f, 0f, 1f, 0f, 1f });
            this.m_interpolationExp = 1f;
            this.C0 = new float[4];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfExponentialInterpolationFunction"/> class.
        /// </summary>
        internal PdfExponentialInterpolationFunction()
            : base(new PdfDictionary())
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the function result when x = 0.
        /// </summary>
        public float[] C0
        {
            get
            {
                return m_c0;
            }

            set
            {
                m_c0 = value;
            }
        }

        /// <summary>
        /// Gets or sets the function result when x = 1.
        /// </summary>
        public float[] C1
        {
            get
            {
                return m_c1;
            }

            set
            {
                m_c1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the Exponent.
        /// </summary>
        public float Exponent
        {
            get
            {
                return m_interpolationExp;
            }

            set
            {
                m_interpolationExp = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// TO calulate the interpolation exponent.
        /// </summary>
        /// <param name="singleArray1">singleArray1</param>
        /// <returns></returns>
        internal float[] InterpolationExponent(float[] singleArray1)
        {
            int num = base.Range.Count / 2;
            float[] numArray = new float[num];
            for (int i = 0; i < num; i++)
            {
                numArray[i] = C0[i] + (((float)Math.Pow((double)singleArray1[0], (double)this.m_interpolationExp)) * (this.C1[i] - this.C0[i]));
            }

            return numArray;
        }
        #endregion
    }
}
