#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Functions
{
    /// <summary>
    /// Implements PDF Sampled Function.
    /// </summary>
    internal class PdfSampledFunction : PdfFunction
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfSampledFunction"/> class.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="range">The range.</param>
        /// <param name="sizes">The sizes.</param>
        /// <param name="samples">The samples.</param>
        internal PdfSampledFunction(float[] domain, float[] range, int[] sizes, byte[] samples)
            : this()
        {
            CheckParams(domain, range, sizes, samples);
            SetDomainAndRange(domain, range);
            SetSizeAndValues(sizes, samples);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfSampledFunction"/> class.
        /// </summary>
        /// <param name="domain">The domain, which represents the range of the input values.</param>
        /// <param name="range">The range, which represents the range of the output values.</param>
        /// <param name="sizes">The sizes.</param>
        /// <param name="samples">The number of samples in each dimension.</param>
        internal PdfSampledFunction(float[] domain, float[] range, int[] sizes, int[] samples)
            : this()
        {
            CheckParams(domain, range, sizes, samples);
            SetDomainAndRange(domain, range);
            SetSizeAndValues(sizes, samples);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfSampledFunction"/> class.
        /// </summary>
        /// <param name="domain">The domain of sampled function.</param>
        /// <param name="range">The range of sampled function.</param>
        /// <param name="sizes">The sizes.</param>
        /// <param name="samples">The samples of sampled function,
        /// which should be in the range.</param>
        /// <param name="bps">The bit-per-sample value.</param>
        internal PdfSampledFunction(float[] domain, float[] range, int[] sizes, float[] samples, int bps)
            : this()
        {
            CheckParams(domain, range, sizes, samples);

            PdfStream s = Dictionary as PdfStream;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSampledFunction"/> class.
        /// </summary>
        private PdfSampledFunction()
            : base(new PdfStream())
        {
            Dictionary.SetProperty(DictionaryProperties.FunctionType, new PdfNumber(0));
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Checks the input parameters.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="range">The range.</param>
        /// <param name="sizes">The sizes.</param>
        /// <param name="samples">The samples.</param>
        /// <exception cref="ArgumentNullException">If one of the parameters is null.</exception>
        /// <exception cref="ArgumentException">If dimentions of the arrays don't agree.</exception>
        private void CheckParams(float[] domain, float[] range, int[] sizes, Array samples)
        {
            if (domain == null)
            {
                throw new ArgumentNullException("domain");
            }

            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            if (samples == null)
            {
                throw new ArgumentNullException("samples");
            }

            int rLength = range.Length;
            int dLength = domain.Length;
            int sLength = samples.Length;

            if (dLength <= 0)
            {
                throw new ArgumentException("The array has no enough elements", "domain");
            }

            if (rLength <= 0)
            {
                throw new ArgumentException("The array has no enough elements", "range");
            }

            double frameLength = (rLength * dLength / 4);

            if (sLength < frameLength)
            {
                throw new ArgumentException("There is no enough samples", "samples");
            }

            // TODO: check sizes.
        }

        /// <summary>
        /// Sets the domain and range.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="range">The range.</param>
        private void SetDomainAndRange(float[] domain, float[] range)
        {
            Domain = new PdfArray(domain);
            Range = new PdfArray(range);
        }

        /// <summary>
        /// Sets the size and values.
        /// </summary>
        /// <param name="sizes">The sizes of the sample values.</param>
        /// <param name="samples">The sample values.</param>
        private void SetSizeAndValues(int[] sizes, byte[] samples)
        {
            PdfStream s = Dictionary as PdfStream;
            Dictionary.SetProperty(DictionaryProperties.Size, new PdfArray(sizes));
            Dictionary.SetProperty(DictionaryProperties.BitsPerSample, new PdfNumber(8));

            s.Write(samples);
        }

        /// <summary>
        /// Sets the size and values.
        /// </summary>
        /// <param name="sizes">The sizes of the sample values.</param>
        /// <param name="samples">The sample values.</param>
        private void SetSizeAndValues(int[] sizes, int[] samples)
        {
            PdfStream s = Dictionary as PdfStream;
            Dictionary.SetProperty(DictionaryProperties.Size, new PdfArray(sizes));
            Dictionary.SetProperty(DictionaryProperties.BitsPerSample, new PdfNumber(32));

            const int intSize = 4;
            byte[] smpls = new byte[samples.Length * intSize];
            int index = 0;

            foreach (int value in samples)
            {
                BitConverter.GetBytes(value).CopyTo(smpls, index);
                index += intSize;
            }

            s.Write(smpls);
        }

        #endregion
    }
}
