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
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the sound annotation.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document.
    /// PdfPage page = document.Pages.Add();
    /// //Create a new rectangle
    /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
    /// //Create a new sound annotation.
    /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
    /// soundAnnotation.Sound.Encoding = PdfSoundEncoding.Signed;
    /// soundAnnotation.Sound.Channels = PdfSoundChannels.Stereo;
    /// soundAnnotation.Sound.Bits = 16;
    /// soundAnnotation.Color = new PdfColor(Color.Red);
    /// //Sets the pdf sound icon.
    /// soundAnnotation.Icon = PdfSoundIcon.Speaker;
    /// //Add this annotation to a new page.
    /// page.Annotations.Add(soundAnnotation);
    /// //Save the document to disk.
    /// document.Save("SoundIcon.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document.
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create a new rectangle
    /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
    /// 'Create a new sound annotation.
    /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Datastartup.wav")
    /// soundAnnotation.Sound.Channels = PdfSoundChannels.Stereo;
    /// soundAnnotation.Sound.Bits = 16;
    /// soundAnnotation.Color = new PdfColor(Color.Red);
    /// 'Sets the pdf sound icon.
    /// soundAnnotation.Icon = PdfSoundIcon.Speaker
    /// 'Add this annotation to a new page.
    /// page.Annotations.Add(soundAnnotation)
    /// 'Save the document to disk.
    /// document.Save("SoundIcon.pdf")
    /// </code>
    /// </example> 
    public class PdfSoundAnnotation : PdfFileAnnotation
    {
        #region Fields
        /// <summary>
        /// Type of icon of the sound link.
        /// </summary>
        private PdfSoundIcon m_icon = PdfSoundIcon.Speaker;

        /// <summary>
        /// Internal variable to store sound.
        /// </summary>
        private PdfSound m_sound = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the icon to be used in displaying the annotation.
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Sets the pdf sound icon.
        /// soundAnnotation.Icon = PdfSoundIcon.Speaker;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation);
        /// //Save the document to disk.
        /// document.Save("SoundIcon.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Datastartup.wav")
        /// 'Sets the pdf sound icon.
        /// soundAnnotation.Icon = PdfSoundIcon.Speaker
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation)
        /// 'Save the document to disk.
        /// document.Save("SoundIcon.pdf")
        /// </code>
        /// </example> 
        /// <value>The <see cref="PdfSoundIcon"/> enumeration member specifying the icon for the annotation.</value>
        public PdfSoundIcon Icon
        {
            get
            {
                return this.m_icon;
            }

            set
            {
                if (this.m_icon != value)
                {
                    this.m_icon = value;
                    Dictionary.SetName(DictionaryProperties.Name, this.m_icon.ToString());
                }
            }
        }

        /// <summary>
        /// Gets or sets the sound.
        /// </summary>
        /// <value>The <see cref="PdfSound"/> object specified a sound for the annotation.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Sets the pdfsound
        /// soundAnnotation.Sound = new PdfSound("@..\..\Data\endsup.wav");
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation);
        /// //Save the document to disk.
        /// document.Save("PdfSound.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Data\startup.wav")
        /// 'Sets the pdfsound
        /// soundAnnotation.Sound = New PdfSound("..\..\Data\endsup.wav")
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation)
        /// 'Save the document to disk.
        /// document.Save("PdfSound.pdf")
        /// </code>
        /// </example> 
        public PdfSound Sound
        {
            get
            {
                return this.m_sound;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Sound");
                }

                if (value != this.m_sound)
                {
                    this.m_sound = value;
                }
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Gets or sets file name of the annotation.
        /// </summary>
        //[System.Security.SecurityCritical]
#else
        /// <summary>
        /// Gets or sets file name of the annotation.
        /// </summary>
#endif
        /// <value>The string specifies the file name of the sound annotation.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Gets the file name.
        /// string fileName=soundAnnotation.FileName;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation);
        /// //Save the document to disk.
        /// document.Save("SoundFileName.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Datastartup.wav")
        /// 'Gets the file name.
        /// Dim fileName As string=soundAnnotation.FileName
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation)
        /// 'Save the document to disk.
        /// document.Save("SoundFileName.pdf")
        /// </code>
        /// </example> 
        public override string FileName
        {
            get
            {
                return this.m_sound.FileName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("FileName");
                }

                if (value.Length == 0)
                {
                    throw new ArgumentException("FileName can't be empty");
                }

                if (this.m_sound.FileName != value)
                {
                    this.m_sound.FileName = value;
                }
            }
        }
        #endregion

        #region Constructors
#if SILVERLIGHT
        /// <summary>
        /// Security Critical :  Initializes a new instance of the <see cref="PdfSoundAnnotation"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        ///  Initializes a new instance of the <see cref="PdfSoundAnnotation"/> class.
        /// </summary>
#endif
        /// <param name="rectangle">RectangleF structure that specifies the bounds of the annotation.</param>
        /// <param name="fileName">The string specifies the file name of the sound annotation.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document.
        /// PdfPage page = document.Pages.Add();
        /// //Create a new rectangle
        /// RectangleF rectangle = new RectangleF(10, 40, 30, 30);
        /// //Create a new sound annotation.
        /// PdfSoundAnnotation soundAnnotation = new PdfSoundAnnotation(rectangle, @"...\..\.Data\startup.wav");
        /// //Sets the pdf sound icon.
        /// soundAnnotation.Icon = PdfSoundIcon.Speaker;
        /// //Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation);
        /// //Save the document to disk.
        /// document.Save("SoundIcon.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document.
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create a new rectangle
        /// Dim rectangle As RectangleF = New RectangleF(10, 40, 30, 30)
        /// 'Create a new sound annotation.
        /// Dim soundAnnotation As PdfSoundAnnotation = New PdfSoundAnnotation(rectangle, "..\..\Datastartup.wav")
        /// 'Sets the pdf sound icon.
        /// soundAnnotation.Icon = PdfSoundIcon.Speaker
        /// 'Add this annotation to a new page.
        /// page.Annotations.Add(soundAnnotation)
        /// 'Save the document to disk.
        /// document.Save("SoundIcon.pdf")
        /// </code>
        /// </example> 
        public PdfSoundAnnotation(RectangleF rectangle, string fileName)
            : base(rectangle)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            this.m_sound = new PdfSound(fileName);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Dictionary.SetProperty(DictionaryProperties.Subtype, new PdfName(DictionaryProperties.Sound));
        }

        /// <summary>
        /// Saves instance.
        /// </summary>
        protected override void Save()
        {
            base.Save();
            Dictionary.SetProperty(DictionaryProperties.Sound, new PdfReferenceHolder(this.m_sound));
        }
        #endregion
    }
}