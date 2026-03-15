#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using Syncfusion.Pdf.IO;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the annotation link to external file.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new pdf file link annotation.
    /// PdfFileLinkAnnotation fileLinkAnnotation = new PdfFileLinkAnnotation(rectangle,@"..\..\Images\logo.png");
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(soundAnnotation);
    /// //Save the document to disk.
    /// document.Save("FileLinkAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new pdf file link annotation.
    /// Dim fileLinkAnnotation As PdfFileLinkAnnotation = New PdfFileLinkAnnotation(rectangle, "..\..\Images\logo.png")
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(fileLinkAnnotation)
    /// 'Save the document to disk.
    /// document.Save("FileLinkAnnotation.pdf")
    /// </code>
    /// </example> 
    public class PdfFileLinkAnnotation : PdfActionLinkAnnotation
    {
        #region Fields
        /// <summary>
        /// Internal variable to store file launch action.
        /// </summary>
        private PdfLaunchAction m_action = null;
        #endregion

        #region Properties
#if SILVERLIGHT
        /// <summary>
        /// Security Critical :  Gets or sets name of the file to be references by the annotation.
        /// </summary>
        //[System.Security.SecurityCritical]
#else
        /// <summary>
        ///  Gets or sets name of the file to be references by the annotation.
        /// </summary>
#endif       
        /// <value>A string value specifying the full path to the file to be embedded.</value>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new pdf file link annotation.
        /// PdfFileLinkAnnotation fileLinkAnnotation = new PdfFileLinkAnnotation(rectangle,@"..\..\Images\logo.png");
        /// //Gets the file name.
        /// string fileName=fileLinkAnnotation.FileName;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation);
        /// //Save the document to disk.
        /// document.Save("FileLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new pdf file link annotation.
        /// Dim fileLinkAnnotation As PdfFileLinkAnnotation = New PdfFileLinkAnnotation(rectangle, "..\..\Images\logo.png")
        /// 'Gets the file name.
        /// Dim fileName As String=fileLinkAnnotation.FileName
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(fileLinkAnnotation)
        /// 'Save the document to disk.
        /// document.Save("FileLinkAnnotation.pdf")
        /// </code>
        /// </example> 
        public string FileName
        {
            get
            {
                return this.m_action.FileName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("FileName");
                }

                if (value.Length == 0)
                {
                    throw new ArgumentException("FileName - string can not be empty");
                }

                if (this.m_action.FileName != value)
                {
                    this.m_action.FileName = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action to be executed when the annotation is activated.</value>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new pdf file link annotation.
        /// PdfFileLinkAnnotation fileLinkAnnotation = new PdfFileLinkAnnotation(rectangle,@"..\..\Images\logo.png");
        /// //Set a action to filelink annotation.
        /// PdfJavaScriptAction javaAction = new PdfJavaScriptAction("app.alert(\"You are looking at Java script action of PDF \")");
        /// fileLinkAnnotation.Action=javaAction;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation);
        /// //Save the document to disk.
        /// document.Save("FileLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new pdf file link annotation.
        /// Dim fileLinkAnnotation As PdfFileLinkAnnotation = New PdfFileLinkAnnotation(rectangle, "..\..\Images\logo.png")
        /// 'Set a action to fileLinkAnnotation.
        /// Dim javaAction As PdfJavaScriptAction = New PdfJavaScriptAction("app.alert(""You are looking at Java script action of PDF "")")
        /// fileLinkAnnotation.Action=javaAction
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(fileLinkAnnotation)
        /// 'Save the document to disk.
        /// document.Save("FileLinkAnnotation.pdf")
        /// </code>
        /// </example> 
        public override PdfAction Action
        {
            get
            {
                return base.Action;
            }

            set
            {
                base.Action = value;
                this.m_action.Next = value;
            }
        }
        #endregion

        #region Constructors
#if SILVERLIGHT
        /// <summary>
        /// Security Critical :  Initializes a new instance of the <see cref="PdfFileLinkAnnotation"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        ///  Initializes a new instance of the <see cref="PdfFileLinkAnnotation"/> class.
        /// </summary>
#endif      
        /// <param name="rectangle">Bounds of the annotation.</param>
        /// <param name="fileName">A string value specifying the full path to the file to be embedded.</param>
        /// <example>
        /// <code lang = "C#">
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new pdf file link annotation.
        /// PdfFileLinkAnnotation fileLinkAnnotation = new PdfFileLinkAnnotation(rectangle,@"..\..\Images\logo.png");
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation);
        /// //Save the document to disk.
        /// document.Save("FileLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new pdf file link annotation.
        /// Dim fileLinkAnnotation As PdfFileLinkAnnotation = New PdfFileLinkAnnotation(rectangle, "..\..\Images\logo.png")
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(fileLinkAnnotation)
        /// 'Save the document to disk.
        /// document.Save("FileLinkAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfFileLinkAnnotation(RectangleF rectangle, string fileName)
            : base(rectangle)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("fileName - string can not be empty");
            }

            this.m_action = new PdfLaunchAction(fileName);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Saves annotation object.
        /// </summary>
        protected override void Save()
        {
            base.Save();
            Dictionary.SetProperty(DictionaryProperties.A, this.m_action);
        }
        #endregion
    }
}
