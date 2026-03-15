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

namespace Syncfusion.Pdf
{
    /// <summary>
    /// This class allows to manipulate with page
    /// labels of one of the sections.
    /// </summary>
    public class PdfPageLabel : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the numbering style.
        /// </summary>
        public PdfNumberStyle NumberStyle
        {
            get
            {
                PdfNumberStyle style = PdfNumberStyle.None;
                PdfName name = m_dictionary[DictionaryProperties.S] as PdfName;

                if (name != null)
                {
                    style = FromStringToStyle(name.Value);
                }

                return style;
            }
            set
            {
                string name = FromStyleToString(value);

                if (name == null || name == string.Empty)
                {
                    m_dictionary.Remove(DictionaryProperties.S);
                }
                else
                {
                    m_dictionary.SetName(DictionaryProperties.S, name);
                }
            }
        }

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        public string Prefix
        {
            get
            {
                string prefix = null;
                PdfString p = m_dictionary[DictionaryProperties.P] as PdfString;

                if (p != null)
                {
                    prefix = p.Value;
                }

                return prefix;
            }
            set
            {
                if (value == null || value == string.Empty)
                {
                    m_dictionary.Remove(DictionaryProperties.P);
                }
                else
                {
                    m_dictionary.SetString(DictionaryProperties.P, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the start number.
        /// </summary>
        public int StartNumber
        {
            get
            {
                int start = -1; // No start number.
                PdfNumber n = m_dictionary[DictionaryProperties.St] as PdfNumber;

                if (n != null)
                {
                    start = n.IntValue;
                }

                return start;
            }
            set
            {
                if (value < 0)
                {
                    m_dictionary.Remove(DictionaryProperties.St);
                }
                else
                {
                    m_dictionary.SetNumber(DictionaryProperties.St, value);
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfPageLabel"/> class.
        /// </summary>
        public PdfPageLabel()
            : base()
        {
            m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName("PageLabel"));
            m_dictionary.SetProperty(DictionaryProperties.S, new PdfName(DictionaryProperties.D)); // Numeric.
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Converts style to a string.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns>The PDF name value representing the specified style.</returns>
        private static string FromStyleToString(PdfNumberStyle style)
        {
            string name = null;

            switch (style)
            {
                case PdfNumberStyle.None:
                    break;

                case PdfNumberStyle.Numeric:
                    name = DictionaryProperties.D;
                    break;

                case PdfNumberStyle.UpperLatin:
                    name = DictionaryProperties.A;
                    break;

                case PdfNumberStyle.LowerLatin:
                    name = DictionaryProperties.a;
                    break;

                case PdfNumberStyle.UpperRoman:
                    name = DictionaryProperties.R;
                    break;

                case PdfNumberStyle.LowerRoman:
                    name = DictionaryProperties.r;
                    break;

                default:
                    throw new ArgumentException("Unsupported style.", "style");
            }

            return name;
        }

        /// <summary>
        /// Converts string to numbering style.
        /// </summary>
        /// <param name="name">The PDF name of the style.</param>
        /// <returns>The converted numbering style.</returns>
        private static PdfNumberStyle FromStringToStyle(string name)
        {
            PdfNumberStyle style = PdfNumberStyle.None;

            if (name != null && name != string.Empty)
            {
                switch (name)
                {
                    case DictionaryProperties.D:
                        style = PdfNumberStyle.Numeric;
                        break;

                    case DictionaryProperties.A:
                        style = PdfNumberStyle.UpperLatin;
                        break;

                    case DictionaryProperties.a:
                        style = PdfNumberStyle.LowerLatin;
                        break;

                    case DictionaryProperties.R:
                        style = PdfNumberStyle.UpperRoman;
                        break;

                    case DictionaryProperties.r:
                        style = PdfNumberStyle.LowerRoman;
                        break;

                    default:
                        throw new ArgumentException("Unsupported style name.", "name");
                }
            }

            return style;
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
