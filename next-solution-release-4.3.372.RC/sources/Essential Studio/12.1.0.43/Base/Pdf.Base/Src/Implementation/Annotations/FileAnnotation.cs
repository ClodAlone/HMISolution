#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Drawing;
using Syncfusion.Pdf.IO;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents a base class for file attachment annotation. 
    /// </summary>
    /// <seealso cref=" PdfSoundAnnotation"/> Class
    /// <seealso cref=" Pdf3DAnnotation"/> Class
    /// <seealso cref=" PdfAttachmentAnnotation"/> Class 
    public abstract class PdfFileAnnotation : PdfAnnotation
    {
        #region Fields
        /// <summary>
        /// Annotation's appearance.
        /// </summary>
        private PdfAppearance m_appearance = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets file name of the annotation.
        /// </summary>
        public abstract string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets appearance of the annotation.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new pdf3d annotation.
        /// Pdf3DAnnotation annot = new Pdf3DAnnotation(new RectangleF(10, 50, 300, 150), @"..\..\Data\GetSubgraphBound.u3d");
        /// //Create a new pdf appreance
        /// annot.Appearance = new PdfAppearance(annot);
        /// annot.Appearance.Normal.Graphics.DrawString("Click to activate", font, brush, new PointF(40, 40));
        /// //Add this annotation to a new page
        /// annot.Appearance.Normal.Draw(page, new PointF(annot.Location.X, annot.Location.Y));
        /// //Save the document to disk.
        /// document.Save("Appearance.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new pdf3d annotation
        /// Dim annot As Pdf3DAnnotation = New Pdf3DAnnotation(New RectangleF(10, 300, 300, 150), "..\..\Data\threeLevelHierarchy.u3d")
        /// 'Creates a new pdf3d apperance
        /// annot.Appearance = New PdfAppearance(annot)
        /// 'Add this annotation to a new page.
        /// annot.Appearance.Normal.Draw(page, New PointF(annot.Location.X, annot.Location.Y))
        /// 'Save the  document to disk.
        /// document.Save("Appearance.pdf")
        /// </code>
        /// </example> 
        public PdfAppearance Appearance
        {
            get
            {
                if (this.m_appearance == null)
                {
                    this.m_appearance = new PdfAppearance(this);
                }

                return this.m_appearance;
            }

            set
            {
                if (this.m_appearance != value)
                {
                    this.m_appearance = value;
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFileAnnotation"/> class.
        /// </summary>
        protected PdfFileAnnotation()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFileAnnotation"/> class.
        /// </summary>
        /// <param name="rectangle">Bounds of the annotation.</param>
        protected PdfFileAnnotation(RectangleF rectangle)
            : base(rectangle)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Saves an annotation.
        /// </summary>
        protected override void Save()
        {
            base.Save();

            if (this.m_appearance != null && this.m_appearance.Normal != null)
            {
                Dictionary.SetProperty(DictionaryProperties.AP, this.m_appearance);
            }
        }
        #endregion
    }
}
