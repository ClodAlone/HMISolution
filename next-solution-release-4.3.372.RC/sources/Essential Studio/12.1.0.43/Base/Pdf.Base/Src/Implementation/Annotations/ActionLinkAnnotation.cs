#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents base class for link annotations with associated action.
    /// </summary>
    /// <example>
    /// <code lang="c#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new file link annotation.
    /// PdfFileLinkAnnotation fileLinkAnnotation = new PdfFileLinkAnnotation(rectangle,@"..\..\Images\logo.png");
    /// //Set a action to file link annotation.
    /// PdfJavaScriptAction javaAction = new PdfJavaScriptAction("app.alert(\"You are looking at Java script action of PDF \")");
    /// fileLinkAnnotation.Action=javaAction;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(fileLinkAnnotation);
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
    /// 'Create a new file link annotation.
    /// Dim fileLinkAnnotation As PdfFileLinkAnnotation = New PdfFileLinkAnnotation(fileLinkAnnotationRectangle, "..\..\Images\logo.png")
    /// 'Set a action to file link annotation.
    /// Dim javaAction As PdfJavaScriptAction = New PdfJavaScriptAction("app.alert(""You are looking at Java script action of PDF "")")
    /// fileLinkAnnotation.Action=javaAction
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(fileLinkAnnotation)
    /// 'Save the  document to disk.
    /// document.Save("FileLinkAnnotation.pdf")
    /// </code>
    /// </example> 
    public abstract class PdfActionLinkAnnotation : PdfLinkAnnotation
    {
        #region Fields
        /// <summary>
        /// Internal variable to store annotation's action.
        /// </summary>
        private PdfAction m_action = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the action for the link annotation.
        /// </summary>
        /// <value>The action to be executed when the link is activated.</value>
        /// <example>
        /// <code lang="c#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new file link annotation.
        /// PdfFileLinkAnnotation fileLinkAnnotation = new PdfFileLinkAnnotation(rectangle,@"..\..\Images\logo.png");
        /// //Set a action to file link annotation.
        /// PdfJavaScriptAction javaAction = new PdfJavaScriptAction("app.alert(\"You are looking at Java script action of PDF \")");
        /// fileLinkAnnotation.Action=javaAction;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(fileLinkAnnotation);
        /// //Save the  document to disk.
        /// document.Save("FileLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle.
        ///  Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new file link annotation.
        /// Dim fileLinkAnnotation As PdfFileLinkAnnotation = New PdfFileLinkAnnotation(fileLinkAnnotationRectangle, "..\..\Images\logo.png")
        /// 'Set a action to file link annotation.
        /// Dim javaAction As PdfJavaScriptAction = New PdfJavaScriptAction("app.alert(""You are looking at Java script action of PDF "")")
        /// fileLinkAnnotation.Action=javaAction
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(fileLinkAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("FileLinkAnnotation.pdf")
        /// </code>
        /// </example> 
        public virtual PdfAction Action
        {
            get
            {
                return m_action;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Action");
                }

                m_action = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfActionLinkAnnotation"/> class.
        /// </summary>
        /// <param name="rectangle">Bounds of the annotation.</param>
        /// <example>
        /// <code lang="c#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new file link annotation.
        /// PdfFileLinkAnnotation fileLinkAnnotation = new PdfFileLinkAnnotation(rectangle);
        /// //Set a action to file link annotation.
        /// PdfJavaScriptAction javaAction = new PdfJavaScriptAction("app.alert(\"You are looking at Java script action of PDF \")");
        /// fileLinkAnnotation.Action=javaAction;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(fileLinkAnnotation);
        /// //Save the  document to disk.
        /// document.Save("FileLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new file link annotation.
        /// Dim fileLinkAnnotation As PdfFileLinkAnnotation = New PdfFileLinkAnnotation(fileLinkAnnotationRectangle)
        /// 'Set a action to file link annotation.
        /// Dim javaAction As PdfJavaScriptAction = New PdfJavaScriptAction("app.alert(""You are looking at Java script action of PDF "")")
        /// fileLinkAnnotation.Action=javaAction
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(fileLinkAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("FileLinkAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfActionLinkAnnotation(RectangleF rectangle)
            : base(rectangle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfActionLinkAnnotation"/> class.
        /// </summary>
        /// <param name="rectangle">Bounds specifies the location of the drawn text.</param>
        /// <param name="action">The <see cref="PdfAction"/> specifies an action to be executed when the link is activated.</param>
        /// <example>
        /// <code lang="c#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// PdfJavaScriptAction javaAction = new PdfJavaScriptAction("app.alert(\"You are looking at Java script action of PDF \")");
        /// //Create a new file link annotation.
        /// PdfFileLinkAnnotation fileLinkAnnotation = new PdfFileLinkAnnotation(rectangle,javaAction);
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(fileLinkAnnotation);
        /// //Save the  document to disk.
        /// document.Save("FileLinkAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// Dim javaAction As PdfJavaScriptAction = New PdfJavaScriptAction("app.alert(""You are looking at Java script action of PDF "")")
        /// 'Create a new file link annotation.
        /// Dim fileLinkAnnotation As PdfFileLinkAnnotation = New PdfFileLinkAnnotation(fileLinkAnnotationRectangle,javaAction)
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(fileLinkAnnotation)
        /// 'Save the  document to disk.
        /// document.Save("FileLinkAnnotation.pdf")
        /// </code>
        /// </example> 
        public PdfActionLinkAnnotation(RectangleF rectangle, PdfAction action)
            : base(rectangle)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            m_action = action;
        }
        #endregion

    }
}
