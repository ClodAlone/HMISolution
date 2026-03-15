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

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents additional actions of the annotations.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Creata a new PdfButtonField
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
    /// submitButton.Text = "Launch";
    /// //Create a new PdfLaunchAction
    /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
    /// //Set the launch action to submit button
    /// submitButton.Actions.MouseLeave = launchAction;
    /// Add the submit button to a new document.
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
    /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
    /// 'Set the launch action to  submit button
    /// submitButton.Actions.MouseLeave = launchAction
    /// 'Add the submit button to document.
    /// document.Form.Fields.Add(submitButton)
    /// 'Save document to disk.
    /// document.Save("LaunchAction.pdf")
    /// </code>
    /// </example>
    public class PdfAnnotationActions : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store enter action.
        /// </summary>
        private PdfAction m_mouseEnter = null;

        /// <summary>
        /// Internal variable to store leave action.
        /// </summary>
        private PdfAction m_mouseLeave = null;

        /// <summary>
        /// Internal variable to store mouse down action.
        /// </summary>
        private PdfAction m_mouseDown = null;

        /// <summary>
        /// Internal variable to store mouse up action.
        /// </summary>
        private PdfAction m_mouseUp = null;

        /// <summary>
        /// Internal variable to store get focus action.
        /// </summary>
        private PdfAction m_gotFocus = null;

        /// <summary>
        /// Internal variable to store lost focus action.
        /// </summary>
        private PdfAction m_lostFocus = null;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAnnotationActions"/> class.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// PdfAnnotationActions annotationActions = new PdfAnnotationActions();
        /// //Create a new PdfJavaScriptAction
        /// PdfJavaScriptAction javaAction = new PdfJavaScriptAction("app.alert(\"You are looking at Java script action of PDF \")");
        /// annotationActions.MouseEnter = javaAction;
        /// </code>
        /// <code lang="VB">
        /// Dim annotationActions As PdfAnnotationActions  = New PdfAnnotationActions()
        /// 'Create a new PdfJavaScriptAction.
        /// Dim javaAction As PdfJavaScriptAction  = New PdfJavaScriptAction("app.alert(\"You are looking at Java script action of PDF \")")
        /// annotationActions.MouseEnter = javaAction
        /// </code>
        /// </example>
        public PdfAnnotationActions()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the action to be performed when the cursor enters the annotation�s 
        /// active area.
        /// </summary>
        /// <value>The mouse enter action.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creata a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a new PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
        /// //Set the launch action to submit button
        /// submitButton.Actions.MouseEnter = launchAction;
        /// Add the submit button to a new document.
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
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
        /// 'Set the launch action to  submit button
        /// submitButton.Actions.MouseEnter = launchAction
        /// 'Add the submit button to document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        public PdfAction MouseEnter
        {
            get
            {
                return m_mouseEnter;
            }
           
            set
            {
                if (m_mouseEnter != value)
                {
                    m_mouseEnter = value;
                    m_dictionary.SetProperty(DictionaryProperties.E, m_mouseEnter);
                }
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the cursor exits the annotation�s 
        /// active area.
        /// </summary>
        /// <value>The mouse leave action.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creata a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a new PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
        /// //Set the launch action to submit button
        /// submitButton.Actions.MouseLeave = launchAction;
        /// Add the submit button to a new document.
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
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
        /// 'Set the launch action to  submit button
        /// submitButton.Actions.MouseLeave = launchAction
        /// 'Add the submit button to document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        public PdfAction MouseLeave
        {
            get
            {
                return m_mouseLeave;
            }
          
            set
            {
                if (m_mouseLeave != value)
                {
                    m_mouseLeave = value;
                    m_dictionary.SetProperty(DictionaryProperties.X, m_mouseLeave);
                }
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the mouse button is pressed inside the 
        /// annotation�s active area.
        /// </summary>
        /// <value>The mouse down action.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creata a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a new PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
        /// //Set the launch action to submit button
        /// submitButton.Actions.MouseDown = launchAction;
        /// Add the submit button to a new document.
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
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
        /// 'Set the launch action to  submit button
        /// submitButton.Actions.MouseDown = launchAction
        /// 'Add the submit button to document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        public PdfAction MouseDown
        {
            get
            {
                return m_mouseDown;
            }
           
            set
            {
                if (m_mouseDown != value)
                {
                    m_mouseDown = value;
                    m_dictionary.SetProperty(DictionaryProperties.D, m_mouseDown);
                }
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the mouse button is released 
        /// inside the annotation�s active area..
        /// </summary>
        /// <value>The mouse up action.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creata a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a new PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
        /// //Set the launch action to submit button
        /// submitButton.Actions.MouseUp = launchAction;
        /// Add the submit button to a new document.
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
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
        /// 'Set the launch action to  submit button
        /// submitButton.Actions.MouseUp = launchAction
        /// 'Add the submit button to document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        public PdfAction MouseUp
        {
            get
            {
                return m_mouseUp;
            }
           
            set
            {
                if (m_mouseUp != value)
                {
                    m_mouseUp = value;
                    m_dictionary.SetProperty(DictionaryProperties.U, m_mouseUp);
                }
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the annotation receives the 
        /// input focus.
        /// </summary>
        /// <value>The got focus action.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creata a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a new PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
        /// //Set the launch action to submit button
        /// submitButton.Actions.GotFocus = launchAction;
        /// Add the submit button to a new document.
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
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
        /// 'Set the launch action to  submit button
        /// submitButton.Actions.GotFocus = launchAction
        /// 'Add the submit button to document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        public PdfAction GotFocus
        {
            get
            {
                return m_gotFocus;
            }
           
            set
            {
                if (m_gotFocus != value)
                {
                    m_gotFocus = value;
                    m_dictionary.SetProperty(DictionaryProperties.Fo, m_gotFocus);
                }
            }
        }

        /// <summary>
        /// Gets or sets the action to be performed when the annotation loses the 
        /// input focus.
        /// </summary>
        /// <value>The lost focus action.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Creata a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Launch";
        /// //Create a new PdfLaunchAction
        /// PdfLaunchAction launchAction = new PdfLaunchAction(@"..\..\Data\Sample.txt");
        /// //Set the launch action to submit button
        /// submitButton.Actions.LostFocus = launchAction;
        /// Add the submit button to a new document.
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
        /// Dim launchAction As PdfLaunchAction  = new PdfLaunchAction(@"..\..\Data\Sample.txt")
        /// 'Set the launch action to  submit button
        /// submitButton.Actions.LostFocus = launchAction
        /// 'Add the submit button to document.
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("LaunchAction.pdf")
        /// </code>
        /// </example>
        public PdfAction LostFocus
        {
            get
            {
                return m_lostFocus;
            }
           
            set
            {
                if (m_lostFocus != value)
                {
                    m_lostFocus = value;
                    m_dictionary.SetProperty(DictionaryProperties.Bl, m_lostFocus);
                }
            }
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
