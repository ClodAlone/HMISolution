#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents the base class for PdfBlend and PdfColorBlend classes.
    /// Implements basic routines needed by both classes.
    /// </summary>
    public abstract class PdfBlendBase
    {
        #region Constants
        /// <summary>
        /// Precision of the GCD calculations.
        /// </summary>
        private const float Precision = 1000.0f;
        #endregion

        #region Fields
        /// <summary>
        /// Local variable to store the count.
        /// </summary>
        private int m_count;

        /// <summary>
        /// Local variable to store the positions.
        /// </summary>
        private float[] m_positions;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfBlendBase"/> class.
        /// </summary>
        protected PdfBlendBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfBlendBase"/> class.
        /// </summary>
        /// <param name="count">The number of the elements.</param>
        protected PdfBlendBase(int count)
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the positions array.
        /// </summary>
        public float[] Positions
        {
            get
            {
                return m_positions;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Positions");
                }

                m_positions = SetArray(value) as float[];
            }
        }

        /// <summary>
        /// Gets the number of elements that specify the blend.
        /// </summary>
        protected int Count
        {
            get
            {
                return m_count;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Calculate the GCD of the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>The calculated GCD value.</returns>
        protected static float Gcd(float[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            if (values.Length < 1)
            {
                throw new ArgumentException("Not enough values in the array.", "values");
            }

            float gcd = values[0];
            if (values.Length > 1)
            {
                for (int i = 1, count = values.Length; i < count; ++i)
                {
                    gcd = Gcd(values[i], gcd);

                    if (gcd == 1 / Precision) break; // We've reached the minimal value.
                }
            }

            return gcd;
        }

        /// <summary>
        /// Determines greatest common divisor of the specified u and v.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <param name="v">The v.</param>
        /// <returns>The GCD value</returns>
        protected static float Gcd(float u, float v)
        {
            if (u < 0 || u > 1.0f)
            {
                throw new ArgumentOutOfRangeException("u");
            }

            if (v < 0 || v > 1.0f)
            {
                throw new ArgumentOutOfRangeException("v");
            }

            int iU = (int)Math.Max(1, (u * Precision));
            int iV = (int)Math.Max(1, (v * Precision));

            int iResult = Gcd(iU, iV);
            float result = ((float)iResult) / Precision;

            return result;
        }

        /// <summary>
        /// Determines greatest common divisor of the specified u and v.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <param name="v">The v.</param>
        /// <returns>The GCD value</returns>
        protected static int Gcd(int u, int v)
        {
            if (u <= 0)
            {
                throw new ArgumentOutOfRangeException("u", "The arguments can't be less or equal to zero.");
            }

            if (v <= 0)
            {
                throw new ArgumentOutOfRangeException("v", "The arguments can't be less or equal to zero.");
            }

            if (u == 1 || v == 1)
            {
                return 1;
            }

            int shift = 0;

            // B1
            while (IsEven(u, v))
            {
                ++shift;
                u >>= 1; // u/2
                v >>= 1; // v/2
            }

            while ((u & 1) <= 0)
                u >>= 1;
            do
            {
                while ((v & 1) <= 0)  /* Loop X */
                    v >>= 1;

                if (u > v)
                {
                    int t = v; v = u; u = t;
                } 
                v = v - u;                      
            } while (v != 0);

            return u << shift;
        }

        /// <summary>
        /// Determines if both parameters are even numbers.
        /// </summary>
        /// <param name="u">The first value.</param>
        /// <param name="v">The second value.</param>
        /// <returns>result</returns>
        private static bool IsEven(int u, int v)
        {
            bool result = true;

            result &= (u & 0x1) <= 0; // Is u even?
            result &= (v & 0x1) <= 0; // Is v even?

            return result;
        }

        /// <summary>
        /// Determines if the u value is even.
        /// </summary>
        /// <param name="u">The u value.</param>
        /// <returns>bool</returns>
        private static bool IsEven(int u)
        {
            return ((u & 0x1) <= 0);
        }

        /// <summary>
        /// Interpolates the specified colours according to the t value.
        /// </summary>
        /// <param name="t">The t value, which show the imagine position on a line from 0 to 1.</param>
        /// <param name="color1">The minimal colour.</param>
        /// <param name="color2">The maximal colour.</param>
        /// <param name="colorSpace">The color space.</param>
        /// <returns>color</returns>
        internal static PdfColor Interpolate(double t, PdfColor color1, PdfColor color2, PdfColorSpace colorSpace)
        {
            PdfColor color = new PdfColor();

            switch (colorSpace)
            {
                case PdfColorSpace.RGB:
                    float red = (float)Interpolate(t, color1.Red, color2.Red);
                    float green = (float)Interpolate(t, color1.Green, color2.Green);
                    float blue = (float)Interpolate(t, color1.Blue, color2.Blue);

                    color = new PdfColor(red, green, blue);
                    break;

                case PdfColorSpace.GrayScale:
                    float gray = (float)Interpolate(t, color1.Gray, color2.Gray);
                    color = new PdfColor(gray);
                    break;

                case PdfColorSpace.CMYK:
                    float cyan = (float)Interpolate(t, color1.C, color2.C);
                    float magenta = (float)Interpolate(t, color1.M, color2.M);
                    float yellow = (float)Interpolate(t, color1.Y, color2.Y);
                    float black = (float)Interpolate(t, color1.K, color2.K);

                    color = new PdfColor(cyan, magenta, yellow, black);
                    break;

                default:
                    throw new ArgumentException("Unsupported colour space");
            }

            return color;
        }

        /// <summary>
        /// Interpolates the specified colours according to the t value.
        /// </summary>
        /// <param name="t">The t value, which show the imagine position on a line from 0 to 1.</param>
        /// <param name="v1">The minimal value.</param>
        /// <param name="v2">The maximal value.</param>
        /// <returns><result/returns>
        internal static double Interpolate(double t, double v1, double v2)
        {
            const double t0 = 0.0;
            const double t1 = 1.0;
            double result = 0.0;

            if (t == t0)
            {
                result = v1;
            }
            else if (t == t1)
            {
                result = v2;
            }
            else
            {
                result = v1 + ((t - t0) * (v2 - v1) / (t1 - t0));
            }

            return result;
        }

        /// <summary>
        /// Sets the array.
        /// </summary>
        /// <param name="array">The array, which has values.</param>
        /// <returns>The array if it's passed all tests.</returns>
        protected Array SetArray(Array array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            int length = array.Length;

            if (length < 0)
            {
                throw new ArgumentException("The array can't be an empmy array", "array");
            }

            if (Count <= 0)
            {
                m_count = length;
            }
            else
            {
                if (length != Count)
                {
                    throw new ArgumentException("The array should agree with Count property", "Positions");
                }
            }

            return array;
        }

        #endregion
    }
}
