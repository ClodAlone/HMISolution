#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.Functions;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Implements blend brush setting and functions.
    /// </summary>
    public sealed class PdfBlend : PdfBlendBase
    {
        #region Fields
        /// <summary>
        /// Local variable to store the factors.
        /// </summary>
        private float[] m_factors;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfBlend"/> class.
        /// </summary>
        public PdfBlend()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfBlend"/> class.
        /// </summary>
        /// <param name="count">The number of elements in the Factors and Positions arrays.</param>
        public PdfBlend(int count)
            : base(count)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the factors array.
        /// </summary>
        public float[] Factors
        {
            get
            {
                return m_factors;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Factors");
                }

                m_factors = SetArray(value) as float[];
            }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Generates a correct color blend.
        /// </summary>
        /// <param name="colours">The colours.</param>
        /// <param name="colorSpace">The color space.</param>
        /// <returns>A well formed colour blend.</returns>
        internal PdfColorBlend GenerateColorBlend(PdfColor[] colours, PdfColorSpace colorSpace)
        {
            if (colours == null)
            {
                throw new ArgumentNullException("colours");
            }

            if (Positions == null)
            {
                Positions = new float[] { 0.0f };
            }

            PdfColorBlend cBlend = new PdfColorBlend(Count);

            float[] positions = Positions;
            PdfColor[] clrs = null;

            if (positions.Length == 1)
            {
                positions = new float[3];
                positions[0] = 0.0f;
                positions[1] = Positions[0];
                positions[2] = 1.0f;

                clrs = new PdfColor[3];
                clrs[0] = colours[0];
                clrs[1] = colours[0];
                clrs[2] = colours[1];
            }
            else
            {
                PdfColor c1 = colours[0];
                PdfColor c2 = colours[1];

                clrs = new PdfColor[Count];

                for (int i = 0, count = Count; i < count; ++i)
                {
                    clrs[i] = PdfColorBlend.Interpolate(m_factors[i], c1, c2, colorSpace);
                }
            }

            cBlend.Positions = positions;
            cBlend.Colors = clrs;

            return cBlend;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The copy of this instance of PdfBlend.</returns>
        internal PdfBlend Clone()
        {
            PdfBlend blend = MemberwiseClone() as PdfBlend;

            if (m_factors != null)
            {
                blend.Factors = m_factors.Clone() as float[];
            }

            if (Positions != null)
            {
                blend.Positions = Positions.Clone() as float[];
            }

            return blend;
        }
        #endregion
    }
}
