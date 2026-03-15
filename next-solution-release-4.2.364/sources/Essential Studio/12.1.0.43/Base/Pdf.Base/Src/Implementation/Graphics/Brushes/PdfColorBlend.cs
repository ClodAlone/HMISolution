#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Text;

using Syncfusion.Pdf.Functions;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.IO;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents the arrays of colors and positions used for
    /// interpolating color blending in a multicolor gradient.
    /// </summary>
    public sealed class PdfColorBlend : PdfBlendBase
    {
        #region Fields
        /// <summary>
        /// Array of colors.
        /// </summary>
        private PdfColor[] m_colors;
        private PdfBrush m_brush;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfColorBlend"/> class.
        /// </summary>
        public PdfColorBlend()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfColorBlend"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        public PdfColorBlend(int count)
            : base(count)
        {
        }

        internal PdfColorBlend(PdfBrush brush)
            : base()
        {
            m_brush = brush;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the colours array.
        /// </summary>
        public PdfColor[] Colors
        {
            get
            {
                return m_colors;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Colors");
                }

                m_colors = SetArray(value) as PdfColor[];
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the function.
        /// </summary>
        /// <param name="colorSpace">The color space.</param>
        /// <returns>The properly prepared sampled function.</returns>
        internal PdfFunction GetFunction(PdfColorSpace colorSpace)
        {
            float[] domain = new float[] { 0.0f, 1.0f };

            int colourComponents = GetColorComponentsCount(colorSpace);
            int maxComponentValue = GetMaxComponentValue(colorSpace);

            float[] range = SetRange(colourComponents, maxComponentValue);

            PdfSampledFunction func = null;

            if (m_brush == null)
            {
                int[] sizes = new int[1];
                int samplesCount;
                float step = 1;

                if (Positions.Length == 2)
                {
                    samplesCount = 2;
                }
                else
                {
                    float[] positions = Positions;

                    float[] intervals = GetIntervals(positions);
                    float gcd = Gcd(intervals);
                    step = gcd;
                    samplesCount = ((int)(1.0f / gcd)) + 2;
                }

                sizes[0] = samplesCount;

                byte[] samples = GetSamplesValues(colorSpace, samplesCount, maxComponentValue, step);

                func = new PdfSampledFunction(domain, range, sizes, samples);

                return func;
            }
            else if(m_brush is PdfLinearGradientBrush || m_brush is PdfRadialGradientBrush)
            {
                PdfLinearGradientBrush brushLinear = m_brush as PdfLinearGradientBrush;
                PdfRadialGradientBrush brushRadial = m_brush as PdfRadialGradientBrush;

                if ((brushLinear != null && brushLinear.Extend == PdfExtend.Both) || brushRadial != null)
                {
                    PdfStitchingFunction stiFunc = new PdfStitchingFunction();
                    PdfArray fnarray = new PdfArray();

                    StringBuilder bounds = new StringBuilder();
                    StringBuilder encode = new StringBuilder();

                    for (int k = 1; k < Positions.Length; k++)
                    {
                        PdfExponentialInterpolationFunction exFunc = new PdfExponentialInterpolationFunction(true);
                        exFunc.Domain = new PdfArray(new float[] { 0, 1 });
                        exFunc.Range = new PdfArray(range);
                        float[] c0 = new float[] { Colors[k - 1].Red, Colors[k - 1].Green, Colors[k - 1].Blue };
                        float[] c1 = new float[] { Colors[k].Red, Colors[k].Green, Colors[k].Blue };
                        exFunc.Dictionary["FunctionType"] = new PdfNumber(2);
                        exFunc.Dictionary["N"] = new PdfNumber(1);

                        // Issue with Function, these two values are not preserved if updated to the property.
                        // Hence edit the dictionary directly.
                        exFunc.Dictionary["C0"] = new PdfArray(c0);
                        exFunc.Dictionary["C1"] = new PdfArray(c1);

                        if (k > 1)
                        {
                            bounds.Append(' ');
                            encode.Append(' ');
                        }
                        if (k < Positions.Length - 1)
                            bounds.Append(Positions[k]);

                        if (brushLinear != null)
                            encode.Append("0 1");
                        else if(brushRadial != null)
                            encode.Append("1 0");

                        PdfReferenceHolder holder = new PdfReferenceHolder(exFunc);
                        fnarray.Add(holder);
                    }

                    float[] encodeFloat = new float[encode.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length];
                    float[] boundsFloat = new float[bounds.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length];

                    for (int j = 0; j < encodeFloat.Length; j++)
                        encodeFloat[j] = float.Parse(encode.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[j]);

                    for (int j = 0; j < boundsFloat.Length; j++)
                        boundsFloat[j] = float.Parse(bounds.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[j]);

                    stiFunc.Dictionary["Bounds"] = new PdfArray(boundsFloat);
                    stiFunc.Dictionary["Encode"] = new PdfArray(encodeFloat);

                    if (brushRadial != null)
                    {
                        stiFunc.Range = new PdfArray(range);
                    }

                    stiFunc.Domain = new PdfArray(new float[] { 0, 1 });
                    stiFunc.Dictionary["Functions"] = fnarray;
                    stiFunc.Dictionary["FunctionType"] = new PdfNumber(3);

                    return stiFunc;
                }
                else
                {
                    if (brushLinear != null)
                        brushLinear.Extend = PdfExtend.Both;
                }
            }

            return func;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The copy of this instance of the PdfColorBlend class.</returns>
        internal PdfColorBlend Clone()
        {
            PdfColorBlend cBlend = MemberwiseClone() as PdfColorBlend;

            if (m_colors != null)
            {
                cBlend.Colors = m_colors.Clone() as PdfColor[];
            }

            if (Positions != null)
            {
                cBlend.Positions = Positions.Clone() as float[];
            }

            return cBlend;
        }

        /// <summary>
        /// Sets the range.
        /// </summary>
        /// <param name="colourComponents">The colour components.</param>
        /// <param name="maxValue">The max value.</param>
        /// <returns>The ranges array.</returns>
        private static float[] SetRange(int colourComponents, float maxValue)
        {
            float[] range = new float[colourComponents * 2];

            for (int i = 0; i < colourComponents; ++i)
            {
                range[i * 2] = 0.0f;
                range[i * 2 + 1] = 1.0f; //maxValue;
            }

            return range;
        }

        /// <summary>
        /// Calculates the color components count according to colour space.
        /// </summary>
        /// <param name="colorSpace">The color space.</param>
        /// <returns>The number of colour components.</returns>
        private static int GetColorComponentsCount(PdfColorSpace colorSpace)
        {
            int count = 0;

            switch (colorSpace)
            {
                case PdfColorSpace.RGB:
                    count = 3;
                    break;

                case PdfColorSpace.CMYK:
                    count = 4;
                    break;

                case PdfColorSpace.GrayScale:
                    count = 1;
                    break;

                default:
                    throw new ArgumentException("Unsupported color space: " + colorSpace, "colorSpace");
            }

            return count;
        }

        /// <summary>
        /// Gets samples values for specified colour space.
        /// </summary>
        /// <param name="colorSpace">The color space.</param>
        /// <param name="sampleCount">The sample count.</param>
        /// <param name="maxComponentValue">The max component value.</param>
        /// <param name="step">The step.</param>
        /// <returns>The byte array of the sample values.</returns>
        private byte[] GetSamplesValues(PdfColorSpace colorSpace, int sampleCount,
            int maxComponentValue, float step)
        {
            byte[] values;

            switch (colorSpace)
            {
                case PdfColorSpace.GrayScale:
                    values = GetGrayscaleSamples(sampleCount, maxComponentValue, step);
                    break;

                case PdfColorSpace.CMYK:
                    values = GetCmykSamples(sampleCount, maxComponentValue, step);
                    break;

                case PdfColorSpace.RGB:
                    values = GetRgbSamples(sampleCount, maxComponentValue, step);
                    break;

                default:
                    throw new ArgumentException("Unsupported color space: " + colorSpace, "colorSpace");
            }

            return values;
        }

        /// <summary>
        /// Gets the grayscale samples.
        /// </summary>
        /// <param name="sampleCount">The sample count.</param>
        /// <param name="maxComponentValue">The max component value.</param>
        /// <param name="step">The step.</param>
        /// <returns></returns>
        private byte[] GetGrayscaleSamples(int sampleCount, int maxComponentValue, float step)
        {
            byte[] values = new byte[sampleCount * 2];

            for (int i = 0; i < sampleCount; ++i)
            {
                PdfColor color = GetNextColor(i, step, PdfColorSpace.GrayScale);
                int index = i * 2;

                byte[] v = BitConverter.GetBytes((short)(color.Gray * maxComponentValue));

                values[index] = v[0];
                values[index + 1] = v[1];
            }

            return values;
        }

        /// <summary>
        /// Gets the CMYK samples.
        /// </summary>
        /// <param name="sampleCount">The sample count.</param>
        /// <param name="maxComponentValue">The max component value.</param>
        /// <param name="step">The step.</param>
        /// <returns></returns>
        private byte[] GetCmykSamples(int sampleCount, int maxComponentValue, float step)
        {
            byte[] values = new byte[sampleCount * 4];

            for (int i = 0; i < sampleCount; ++i)
            {
                PdfColor color = GetNextColor(i, step, PdfColorSpace.CMYK);
                int index = i * 4;

                values[index] = (byte)(color.C * maxComponentValue);
                values[index + 1] = (byte)(color.M * maxComponentValue);
                values[index + 2] = (byte)(color.Y * maxComponentValue);
                values[index + 3] = (byte)(color.K * maxComponentValue);
            }

            return values;
        }

        /// <summary>
        /// Gets the RGB samples.
        /// </summary>
        /// <param name="sampleCount">The sample count.</param>
        /// <param name="maxComponentValue">The max component value.</param>
        /// <param name="step">The step.</param>
        /// <returns>The values of RGB samples.</returns>
        private byte[] GetRgbSamples(int sampleCount, int maxComponentValue, float step)
        {
            byte[] values = new byte[sampleCount * 3];

            for (int i = 0; i < sampleCount; ++i)
            {
                PdfColor color = GetNextColor(i, step, PdfColorSpace.RGB);
                int index = i * 3;

                values[index] = color.R;
                values[index + 1] = color.G;
                values[index + 2] = color.B;
            }

            return values;
        }

        /// <summary>
        /// Calculates the color that should be at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="step">The step.</param>
        /// <param name="colorSpace">The color space.</param>
        /// <returns>The colour obtained from the calculation.</returns>
        private PdfColor GetNextColor(int index, float step, PdfColorSpace colorSpace)
        {
            float position = step * index;
            int indexLow, indexHi;

            GetIndices(position, out indexLow, out indexHi);

            PdfColor color;

            if (indexLow == indexHi)
            {
                color = m_colors[indexLow];
            }
            else
            {
                float positionLow = Positions[indexLow];
                float positionHi = Positions[indexHi];
                PdfColor colorLow = m_colors[indexLow];
                PdfColor colorHi = m_colors[indexHi];
                double t = (position - positionLow) / (positionHi - positionLow);

                color = Interpolate(t, colorLow, colorHi, colorSpace);
            }

            return color;
        }

        /// <summary>
        /// Gets the indices.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="indexLow">The index low.</param>
        /// <param name="indexHi">The index hi.</param>
        private void GetIndices(float position, out int indexLow, out int indexHi)
        {
            float[] positions = Positions;

            indexLow = 0;
            indexHi = 0;

            for (int i = 0; i < m_colors.Length; ++i)
            {
                float currPos = positions[i];

                if (currPos == position)
                {
                    indexLow = indexHi = i;
                    break;
                }
                else if (currPos > position)
                {
                    indexHi = i;
                    break;
                }

                indexLow = i;
                indexHi = i;
            }
        }

        /// <summary>
        /// Calculates the max component value.
        /// </summary>
        /// <param name="colorSpace">The color space.</param>
        /// <returns>The maximal component value.</returns>
        private int GetMaxComponentValue(PdfColorSpace colorSpace)
        {
            int result = 0;

            switch (colorSpace)
            {
                case PdfColorSpace.CMYK:
                case PdfColorSpace.RGB:
                    result = 255;
                    break;

                case PdfColorSpace.GrayScale:
                    result = 65535;
                    break;

                default:
                    throw new ArgumentException("Unsupported color space: " + colorSpace, "colorSpace");
            }

            return result;
        }

        /// <summary>
        /// Gets an intervals array from the positions array.
        /// </summary>
        /// <param name="positions">The positions array.</param>
        /// <returns>The intervals obtained from the positions.</returns>
        private float[] GetIntervals(float[] positions)
        {
            int count = positions.Length;
            float[] intervals = new float[count - 1];

            float prev = positions[0];

            for (int i = 1; i < count; ++i)
            {
                float v = positions[i];
                intervals[i - 1] = v - prev;
                prev = v;
            }

            return intervals;
        }
        #endregion
    }
}
