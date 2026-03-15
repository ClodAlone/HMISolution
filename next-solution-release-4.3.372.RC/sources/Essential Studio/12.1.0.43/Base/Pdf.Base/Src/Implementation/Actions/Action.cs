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
    /// Represents base class for all action types.
    /// </summary>
    /// <seealso cref="IPdfWrapper"/> Interface
    public abstract class PdfAction : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Next action to perform.
        /// </summary>
        private PdfAction m_action = null;

        /// <summary>
        /// Internal variable to store dictionary;
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAction"/> class.
        /// </summary>
        protected PdfAction()
            : base()
        {
            Initialize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the next action to be performed after the action represented by this instance.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        ///  //Creates a new sound action
        /// PdfSoundAction soundAction = new PdfSoundAction(@"..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav");
        /// soundAction.Sound.Bits = 16;
        /// soundAction.Sound.Channels = PdfSoundChannels.Stereo;
        /// soundAction.Sound.Encoding = PdfSoundEncoding.Signed;
        /// soundAction.Volume = 0.9f;
        /// soundAction.Mix = true;
        /// //Create a new PdfUriAction
        /// PdfUriAction uriAction = new PdfUriAction("http://www.google.com");
        /// //Set the next action to the soundAction.
        /// soundAction.Next = uriAction;
        /// //Save document to disk.
        /// document.Save("ActionDestination.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new sound annotation
        /// Dim soundAction As PdfSoundAction = New PdfSoundAction("..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav")
        /// soundAction.Sound.Bits = 16
        /// soundAction.Sound.Channels = PdfSoundChannels.Stereo
        /// soundAction.Sound.Encoding = PdfSoundEncoding.Signed
        /// soundAction.Volume = 0.9F
        /// soundAction.Mix = True
        /// 'Create a new PdfUriAction.
        /// Dim uriAction As PdfUriAction  = New PdfUriAction("http://www.google.com")
        /// 'Set the next action to the soundAction.
        /// soundAction.Next = uriAction
        /// 'Save document to disk.
        /// document.Save("ActionDestination.pdf")
        /// </code>
        /// </example> 
        public PdfAction Next
        {
            get
            {
                return m_action;
            }
           
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Next");
                }

                if (m_action != value)
                {
                    m_action = value;
                    Dictionary.SetArray(DictionaryProperties.Next, new PdfReferenceHolder(m_action));
                }
            }
        }

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        internal PdfDictionary Dictionary
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes an instance.
        /// </summary>
        protected virtual void Initialize()
        {
            Dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties.Action));
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
