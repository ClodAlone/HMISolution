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

namespace Syncfusion.Pdf.Graphics
{

    /// <summary>
    /// Represents a simple transparency.
    /// </summary>
    internal class PdfTransparency : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the stroke operation alpha value.
        /// </summary>
        public float Stroke
        {
            get
            {
                float stroke = GetNumber(DictionaryProperties.CA);
                return stroke;
            }
        }

        /// <summary>
        /// Gets the fill operation alpha value.
        /// </summary>
        public float Fill
        {
            get
            {
                float fill = GetNumber(DictionaryProperties.ca);
                return fill;
            }
        }

        /// <summary>
        /// Gets the blend mode.
        /// </summary>
        public PdfBlendMode Mode
        {
            get
            {
                string mode = GetName(DictionaryProperties.ca);
                PdfBlendMode bm = (PdfBlendMode)Enum.Parse(typeof(PdfBlendMode), mode, true);
                return bm;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Transparency"/> class.
        /// </summary>
        /// <param name="stroke">The stroke operation alpha value.</param>
        /// <param name="fill">The fill operation alpha value.</param>
        /// <param name="mode">The blend mode.</param>
        public PdfTransparency(float stroke, float fill, PdfBlendMode mode)
            : base()
        {
            if (stroke < 0)
                throw new ArgumentOutOfRangeException("stroke", "The value can't be less then zero.");

            if (fill < 0)
                throw new ArgumentOutOfRangeException("fill", "The value can't be less then zero.");

            //NOTE : This is needed to attain PDF/A conformance.  Since PDF/A1B
            //does not support transparency key.
            if (PdfDocument.ConformanceLevel == PdfConformanceLevel.Pdf_A1B)
            {
                stroke = (stroke == 0) ? 1 : stroke;
                fill = (fill == 0) ? 1 : fill;
                mode = (mode != PdfBlendMode.Normal) ? PdfBlendMode.Normal : mode;
            }

            m_dictionary.SetNumber(DictionaryProperties.CA, stroke);
            m_dictionary.SetNumber(DictionaryProperties.ca, fill);
            m_dictionary.SetName(DictionaryProperties.BM, mode.ToString());
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see>
        /// is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see>
        /// to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see>
        /// is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            bool result = false;

            if (obj != null)
            {
                PdfTransparency transp = obj as PdfTransparency;

                if (transp != null)
                {
                    result = true;

                    result &= transp.Stroke != Stroke;
                    result &= transp.Fill != Fill;
                    result &= transp.Mode != Mode;
                }
            }

            return result;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// <see cref="M:System.Object.GetHashCode"></see> is suitable
        /// for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the number value.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns>The value of the number specified by the string key.</returns>
        private float GetNumber(string keyName)
        {
            float result = 0.0f;
            PdfNumber numb = m_dictionary[keyName] as PdfNumber;

            if (numb != null)
            {
                result = numb.FloatValue;
            }

            return result;
        }

        /// <summary>
        /// Gets the name value.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns>The name value specified by the key.</returns>
        private string GetName(string keyName)
        {
            string result = null;
            PdfName name = m_dictionary[keyName] as PdfName;

            if (name != null)
            {
                result = name.Value;
            }

            return result;
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion
    }
}
