#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Globalization;

using Syncfusion.Pdf.Functions;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Implements gradient brush capabilities.
    /// </summary>
    public abstract class PdfGradientBrush :
        PdfBrush,
        IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Local variable to store the background color.
        /// </summary>
        private PdfColor m_background;

        /// <summary>
        /// Local variable to store the background color.
        /// </summary>
        private bool m_bStroking;

        /// <summary>
        /// Local variable to store the dictionary.
        /// </summary>
        private PdfDictionary m_patternDictionary;

        /// <summary>
        /// Local variable to store the shading.
        /// </summary>
        private PdfDictionary m_shading;

        /// <summary>
        /// Local variable to store the Transformation Matrix.
        /// </summary>
        private PdfTransformationMatrix m_matrix;

        /// <summary>
        /// Local variable to store the external state.
        /// </summary>
        private PdfExternalGraphicsState m_externalState;

        /// <summary>
        /// Local variable to store the colorSpace.
        /// </summary>
        private PdfColorSpace m_colorSpace;

        /// <summary>
        /// Local variable to store the function.
        /// </summary>
        private PdfFunction m_function = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGradientBrush"/> class.
        /// </summary>
        /// <param name="shading">The shading.</param>
        internal PdfGradientBrush(PdfDictionary shading)
        {
            if (shading == null)
            {
                throw new ArgumentNullException("shading");
            }

            m_patternDictionary = new PdfDictionary();
            m_patternDictionary[DictionaryProperties.Type] = new PdfName(DictionaryProperties.Pattern);
            m_patternDictionary[DictionaryProperties.PatternType] = new PdfNumber(2);

            Shading = shading;
            ColorSpace = PdfColorSpace.RGB;

        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the background color of the brush.
        /// </summary>
        /// <remarks>This value is optional. If null is assigned to it,
        /// the associated entry is removed from the appropriate dictionary.</remarks>
        public PdfColor Background
        {
            get
            {
                return m_background;
            }

            set
            {
                m_background = value;

                PdfDictionary sh = Shading;

                if (value == PdfColor.Empty)
                {
                    sh.Remove(DictionaryProperties.Background);
                }
                else
                {
                    sh[DictionaryProperties.Background] = value.ToArray(ColorSpace);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether use anti aliasing algorithm.
        /// </summary>
        public bool AntiAlias
        {
            get
            {
                PdfDictionary sh = Shading;
                PdfBoolean aa = sh[DictionaryProperties.AntiAlias] as PdfBoolean;
                return aa.Value;
            }

            set
            {
                PdfDictionary sh = Shading;
                PdfBoolean aa = sh[DictionaryProperties.AntiAlias] as PdfBoolean;

                if (aa == null)
                {
                    aa = new PdfBoolean(value);
                    sh[DictionaryProperties.AntiAlias] = aa;
                }
                else
                {
                    aa.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the function.
        /// </summary>
        internal PdfFunction Function
        {
            get
            {
                return m_function;
            }

            set
            {
                m_function = value;

                if (value != null)
                {
                    Shading[DictionaryProperties.Function] = new PdfReferenceHolder(m_function);
                }
                else
                {
                    Shading.Remove(DictionaryProperties.Function);
                }
            }
        }

        /// <summary>
        /// Gets or sets the Boundary box.
        /// </summary>
        /// <remarks>This value is optional. If null is assigned to it,
        /// the associated entry is removed from the appropriate dictionary.</remarks>
        internal PdfArray BBox
        {
            get
            {
                PdfDictionary sh = Shading;
                PdfArray box = sh[DictionaryProperties.BBox] as PdfArray;

                return box;
            }

            set
            {
                PdfDictionary sh = Shading;

                if (value == null)
                {
                    sh.Remove(DictionaryProperties.BBox);
                }
                else
                {
                    sh[DictionaryProperties.BBox] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color space.
        /// </summary>
        internal PdfColorSpace ColorSpace
        {
            get
            {
                return m_colorSpace;
            }

            set
            {
                IPdfPrimitive colorSpace = Shading[DictionaryProperties.ColorSpace];

                if (value != m_colorSpace || colorSpace == null)
                {
                    m_colorSpace = value;

                    string csValue = ColorSpaceToDeviceName(value);

                    if (colorSpace != null)
                    {
                        PdfName csName = colorSpace as PdfName;
                        PdfArray csArray = colorSpace as PdfArray;

                        if (csName != null)
                        {
                            csName.Value = csValue;
                        }
                        else
                        {
                            // TODO: get more info and implement this case.
                            Shading[DictionaryProperties.ColorSpace] =
                                new PdfName(csValue);
                        }
                    }
                    else
                    {
                        Shading[DictionaryProperties.ColorSpace] =
                            new PdfName(csValue);
                    }

                    //ResetFunction();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// this <see cref="T:PdfGradientBrush"/> is stroking.
        /// </summary>
        internal bool Stroking
        {
            get
            {
                return m_bStroking;
            }

            set
            {
                m_bStroking = value;
            }
        }

        /// <summary>
        /// Gets the pattern dictionary.
        /// </summary>
        internal PdfDictionary PatternDictionary
        {
            get
            {
                if (m_patternDictionary == null)
                {
                    m_patternDictionary = new PdfDictionary();
                }

                return m_patternDictionary;
            }
        }

        /// <summary>
        /// Gets or sets the shading dictionary.
        /// </summary>
        /// <remarks>It's obligatory to set this dictionary
        /// as soon as deriving class can.</remarks>
        internal PdfDictionary Shading
        {
            get
            {
                return m_shading;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Shading");
                }

                if (value != m_shading)
                {
                    m_shading = value;
                    PatternDictionary[DictionaryProperties.Shading] = new PdfReferenceHolder(m_shading);
                }
            }
        }

        /// <summary>
        /// Gets or sets the transformation matrix.
        /// </summary>
        internal PdfTransformationMatrix Matrix
        {
            get
            {
                return m_matrix;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Matrix");
                }

                if (value != m_matrix)
                {
                    m_matrix = value.Clone();
                    PdfArray m = new PdfArray(m_matrix.Matrix.Elements);
                    m_patternDictionary[DictionaryProperties.Matrix] = m;
                }
            }
        }

        /// <summary>
        /// Gets or sets the external graphics state,
        /// which would be set temporary while this brush is active.
        /// </summary>
        /// <value>The external graphics state.</value>
        internal PdfExternalGraphicsState ExternalState
        {
            get
            {
                return m_externalState;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ExternalState");
                }

                if (value != m_externalState)
                {
                    m_externalState = value;
                    m_patternDictionary[DictionaryProperties.ExtGState] = (m_externalState as IPdfWrapper).Element;
                }
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectfully..
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources delegate.</param>
        /// <param name="saveChanges">if set to <c>true</c> the changes should be saved anyway.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <returns>True if the brush was different.</returns>
        internal override bool MonitorChanges(PdfBrush brush, PdfStreamWriter streamWriter,
            PdfGraphics.GetResources getResources, bool saveChanges, PdfColorSpace currentColorSpace)
        {
            bool diff = false;

            if (brush != this)
            {
                if (ColorSpace != currentColorSpace)
                {
                    ColorSpace = currentColorSpace;
                    ResetFunction();
                }

                // Set the /Pattern colour space.
                streamWriter.SetColorSpace("Pattern", m_bStroking);
                // Set the pattern for non-stroking operations.
                PdfName name = getResources().GetName(this);
                streamWriter.SetColourWithPattern(null, name, m_bStroking);
                diff = true;
            }

            return diff;
        }

        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectfully..
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources delegate.</param>
        /// <param name="saveChanges">if set to <c>true</c> the changes should be saved anyway.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <param name="check">check</param>
        /// <returns>True if the brush was different.</returns>
        internal override bool MonitorChanges(PdfBrush brush, PdfStreamWriter streamWriter,
            PdfGraphics.GetResources getResources, bool saveChanges, PdfColorSpace currentColorSpace, bool check)
        {
            bool diff = false;

            if (brush != this)
            {
                if (ColorSpace != currentColorSpace)
                {
                    ColorSpace = currentColorSpace;
                    ResetFunction();
                }

                // Set the /Pattern colour space.
                streamWriter.SetColorSpace("Pattern", m_bStroking);
                // Set the pattern for non-stroking operations.
                PdfName name = getResources().GetName(this);
                streamWriter.SetColourWithPattern(null, name, m_bStroking);
                diff = true;
            }

            return diff;
        }

        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectfully..
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources delegate.</param>
        /// <param name="saveChanges">if set to <c>true</c> the changes should be saved anyway.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <param name="check">check</param>
        /// <param name="iccbased">Indicates the IccBased Color Space.</param>
        /// <returns>True if the brush was different.</returns>
        internal override bool MonitorChanges(PdfBrush brush, PdfStreamWriter streamWriter,
           PdfGraphics.GetResources getResources, bool saveChanges, PdfColorSpace currentColorSpace, bool check, bool iccbased)
        {
            bool diff = false;

            if (brush != this)
            {
                if (ColorSpace != currentColorSpace)
                {
                    ColorSpace = currentColorSpace;
                    ResetFunction();
                }

                // Set the /Pattern colour space.
                streamWriter.SetColorSpace("Pattern", m_bStroking);
                // Set the pattern for non-stroking operations.
                PdfName name = getResources().GetName(this);
                streamWriter.SetColourWithPattern(null, name, m_bStroking);
                diff = true;
            }

            return diff;
        }


        /// <summary>
        /// Monitors the changes of the brush and modify PDF state respectfully..
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="streamWriter">The stream writer.</param>
        /// <param name="getResources">The get resources delegate.</param>
        /// <param name="saveChanges">if set to <c>true</c> the changes should be saved anyway.</param>
        /// <param name="currentColorSpace">The current color space.</param>
        /// <param name="check">check</param>
        /// <param name="iccbased">Indicates the IccBased Color Space.</param>
        /// <param name="indexed">Indicates the indexed Color Space.</param>
        /// <returns>True if the brush was different.</returns>
        internal override bool MonitorChanges(PdfBrush brush, PdfStreamWriter streamWriter,
          PdfGraphics.GetResources getResources, bool saveChanges, PdfColorSpace currentColorSpace, bool check, bool iccbased, bool indexed)
        {
            bool diff = false;

            if (brush != this)
            {
                if (ColorSpace != currentColorSpace)
                {
                    ColorSpace = currentColorSpace;
                    ResetFunction();
                }

                // Set the /Pattern colour space.
                streamWriter.SetColorSpace("Pattern", m_bStroking);
                // Set the pattern for non-stroking operations.
                PdfName name = getResources().GetName(this);
                streamWriter.SetColourWithPattern(null, name, m_bStroking);
                diff = true;
            }

            return diff;
        }

        /// <summary>
        /// Resets the changes, which were made by the brush.
        /// In other words resets the state to the initial one.
        /// </summary>
        /// <param name="streamWriter">The stream writer.</param>
        internal override void ResetChanges(PdfStreamWriter streamWriter)
        {
            // Unable reset.
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Converts colorspace enum to a PDF name.
        /// </summary>
        /// <param name="colorSpace">The color space enum value.</param>
        /// <returns>The correct string value.</returns>
        internal static string ColorSpaceToDeviceName(PdfColorSpace colorSpace)
        {
            string result;

            switch (colorSpace)
            {
                case PdfColorSpace.RGB:
                    result = "DeviceRGB";
                    break;

                case PdfColorSpace.CMYK:
                    result = "DeviceCMYK";
                    break;

                case PdfColorSpace.GrayScale:
                    result = "DeviceGray";
                    break;

                default:
                    throw new ArgumentException("Unsupported colour space: " + colorSpace, "colorSpace");
            }

            return result;
        }

        /// <summary>
        /// Resets the pattern dictionary.
        /// </summary>
        /// <param name="dictionary">A new pattern dictionary.</param>
        internal void ResetPatternDictionary(PdfDictionary dictionary)
        {
            m_patternDictionary = dictionary;
        }

        /// <summary>
        /// Resets the function.
        /// </summary>
        internal abstract void ResetFunction();

        /// <summary>
        /// Clones the anti aliasing value.
        /// </summary>
        /// <param name="brush">The brush.</param>
        protected void CloneAntiAliasingValue(PdfGradientBrush brush)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }

            PdfDictionary sh = Shading;
            PdfBoolean aa = sh[DictionaryProperties.AntiAlias] as PdfBoolean;

            if (aa != null)
            {
                brush.Shading[DictionaryProperties.AntiAlias] = new PdfBoolean(aa.Value);
            }
        }

        /// <summary>
        /// Clones the background value.
        /// </summary>
        /// <param name="brush">The brush.</param>
        protected void CloneBackgroundValue(PdfGradientBrush brush)
        {
            PdfColor background = Background;

            if (!background.IsEmpty)
            {
                brush.Background = background;
            }
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the wrapped element.
        /// </summary>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_patternDictionary;
            }
        }
        #endregion
    }
}
