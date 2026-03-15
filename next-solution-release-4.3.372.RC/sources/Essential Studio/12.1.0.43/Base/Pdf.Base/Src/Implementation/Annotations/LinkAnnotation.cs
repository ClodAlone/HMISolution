#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Drawing;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the base class for link annotations.
    /// </summary>
    /// <seealso cref=" PdfDocumentLinkAnnotation"/> Class
    public abstract class PdfLinkAnnotation : PdfAnnotation
    {
	    #region Fields
        /// <summary>
        /// Highlight Mode of the annotation.
        /// </summary>
        private PdfHighlightMode m_highlightMode;
        #endregion

        #region Properties
        public PdfHighlightMode HighlightMode
        {
            get
            {
                return this.m_highlightMode;
            }

            set
            {
                this.m_highlightMode = value;
                string mode = GetHighlightMode(this.m_highlightMode);
                Dictionary.SetName(DictionaryProperties.H, mode);

            }
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Initializes new instance of <see cref="PdfLinkAnnotation"/> class.
        /// </summary>
        public PdfLinkAnnotation()
            : base()
        {
        }

        /// <summary>
        /// Initializes new instance of <see cref="PdfLinkAnnotation"/> class.
        /// </summary>
        /// <param name="rectangle">Bounds of the annotation.</param>
        public PdfLinkAnnotation(RectangleF rectangle)
            : base(rectangle)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes annotation object.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Dictionary.SetProperty(DictionaryProperties.Subtype, new PdfName(DictionaryProperties.Link));
        }
		/// <summary>
        /// Gets Hightlight Mode of the annotation.
        /// </summary>
        private string GetHighlightMode(PdfHighlightMode mode)
        {
            string hightlightMode = null;
            switch (mode)
            {
                case PdfHighlightMode.Invert:
                    hightlightMode = "I";
                    break;
                case PdfHighlightMode.NoHighlighting:
                    hightlightMode = "N";
                    break;
                case PdfHighlightMode.Outline:
                    hightlightMode = "O";
                    break;
                case PdfHighlightMode.Push:
                    hightlightMode = "P";
                    break;
            }
            return hightlightMode;
       }
        #endregion
    }
}
