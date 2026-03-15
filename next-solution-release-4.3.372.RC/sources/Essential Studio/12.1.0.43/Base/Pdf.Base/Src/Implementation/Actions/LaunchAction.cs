#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.IO;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents an action which launches an application or opens or prints a document.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create font and font style.
    /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
    /// //Create a new PdfButtonField
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
    /// submitButton.Font = font;
    /// submitButton.Text = "Launch";
    /// submitButton.BackColor = new PdfColor(181, 191, 203);
    /// //Create a new PdfLaunchAction
    /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
    /// //Set the launch action to submit button
    /// submitButton.Actions.GotFocus = launchAction;
    /// Add the submitButton to the new document.
    /// document.Form.Fields.Add(submitButton);
    /// //Save document to disk.
    /// document.Save("LaunchAction.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create font and font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// 'Create a new PdfButtonField
    /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
    /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
    /// submitButton.Font = font
    /// submitButton.Text = "Launch"
    /// submitButton.BackColor = new PdfColor(181, 191, 203)
    /// 'Create a new PdfLaunchAction
    /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
    /// 'Set the launch action to submit button
    /// submitButton.Actions.GotFocus = launchAction
    /// 'Add the submitButton to the new document.
    /// document.Form.Fields.Add(submitButton)
    /// 'Save document to disk.
    /// document.Save("LaunchAction.pdf")
    /// </code>
    /// </example>
    public class PdfLaunchAction : PdfAction
    {
        #region Fields
        /// <summary>
        /// Internal variable to store file specification.
        /// </summary>
        private ReferenceFileSpecification m_fileSpecification = null;

        private PdfFilePathType m_pathType = PdfFilePathType.Absolute;
        #endregion

        #region Constructors

#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="PdfLaunchAction"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLaunchAction"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file to be launched.</param>       
#endif
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a new PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
        /// //Set the launch action to submit button
        /// submitButton.Actions.GotFocus = launchAction;
        /// Add the submitButton to the new document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("LaunchAction.pdf");
        /// Add the submitButton to the new document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("LaunchAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Launch"
        /// 'Create a new PdfLaunchAction
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
        /// 'Set the launch action to submit button
        /// submitButton.Actions.GotFocus = launchAction
        /// 'Add the submitButton to the new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        public PdfLaunchAction(string fileName)
            : base()
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }  
      
            m_fileSpecification = new ReferenceFileSpecification(fileName, m_pathType);
        }


#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="PdfLaunchAction"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLaunchAction"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file to be launched.</param>    
        /// <param name="fileName">Name of the file to be launched.</param>
        /// <param name="path">Name of the path type.</param>
#endif
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Create a new PdfButtonField
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a the PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt",PdfFilePathType.Absolute);
        /// //Set the launchAction to the submitButton
        /// submitButton.Actions.GotFocus = launchAction;
        /// </code>
        /// <code lang="VB">
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Launch"
        /// 'Create a new PdfLaunchAction
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt",PdfFilePathType.Absolute)
        /// 'Set the launch action to submit button
        /// submitButton.Actions.GotFocus = launchAction
        /// 'Add the submitButton to the new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        public PdfLaunchAction(string fileName, PdfFilePathType path)
            : base()
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            m_pathType = path;
            m_fileSpecification = new ReferenceFileSpecification(fileName, m_pathType);
        }

        /// <summary>
        /// Creates a Launch Action from loaded page
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="loaded"></param>
        internal PdfLaunchAction(string fileName, bool loaded)
            : base()
        {
            if (loaded)
            {
                if (fileName == null)
                {
                    throw new ArgumentNullException("fileName");
                }
                m_fileSpecification = new ReferenceFileSpecification(fileName);
            }
        }
        #endregion

        #region Properties
#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Gets or sets file to be launched.
        /// </summary>
        //[System.Security.SecurityCritical]
#else
        /// <summary>
        /// Gets or sets file to be launched.
        /// </summary>
#endif
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a new PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt",PdfFilePathType.Absolute);
        /// //Get the filename form launchAction
        /// string fileName=launchAction.FileName;
        /// /Set the launch action to submit button
        /// submitButton.Actions.GotFocus = launchAction;
        /// Add the submitButton to the new document.
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("LaunchAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new PdfButtonField
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Text = "Launch"
        /// 'Create a new PdfLaunchAction
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt",PdfFilePathType.Absolute)
        /// 'Get the filename form launchAction
        /// Dim fileName As string =launchAction.FileName
        /// 'Set the launch action to submit button
        /// submitButton.Actions.GotFocus = launchAction
        /// 'Add the submitButton to the new document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        public string FileName
        {
            get
            {
                return m_fileSpecification.FileName;
            }
           
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("FileName");
                }

                if (value.Length == 0)
                {
                    throw new ArgumentException("File name can not be empty");
                }

                if (m_fileSpecification.FileName != value)
                {
                    m_fileSpecification.FileName = value;
                }
            }
        }

        //public PdfPath PdfPathType
        //{
        //    get 
        //    {
        //        return m_pathType;
        //    }
        //    set
        //    {
        //        if (m_pathType != value)
        //        {
        //            m_pathType = value;
        //            m_fileSpecification.PathType = value;
        //        }

                
        //    }
        //}
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            Dictionary.BeginSave += new SavePdfPrimitiveEventHandler(Dictionary_BeginSave);
            Dictionary.SetProperty(DictionaryProperties.S, new PdfName(DictionaryProperties.Launch));
        }

        /// <summary>
        /// Handles the BeginSave event of the Dictionary.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
        private void Dictionary_BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            Dictionary.SetProperty(DictionaryProperties.F, new PdfReferenceHolder(m_fileSpecification));
        }
        #endregion
    }
}
