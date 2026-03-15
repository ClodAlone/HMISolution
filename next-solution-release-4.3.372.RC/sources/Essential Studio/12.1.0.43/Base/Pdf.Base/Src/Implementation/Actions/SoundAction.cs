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
    /// Represents the sound action.
    /// </summary>
    /// <example>
    /// <code lang = "C#">
    /// //Create a new PDF document.
    /// PdfDocument document = new PdfDocument();
    /// //Creates a new page and adds it as the last page of the document
    /// PdfPage page = document.Pages.Add();
    /// //Create a new PdfButtonField
    /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
    /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
    /// submitButton.Text = "Apply";
    /// //Create sound action
    /// PdfSoundAction soundAction = new PdfSoundAction(@"..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav");
    /// soundAction.Sound.Bits = 16;
    /// soundAction.Sound.Channels = PdfSoundChannels.Stereo;
    /// soundAction.Sound.Encoding = PdfSoundEncoding.Signed;
    /// soundAction.Volume = 0.9f;
    /// soundAction.Synchronous = true;
    /// soundAction.Mix = true;
    /// //Set the sound action to submit button
    /// submitButton.Actions.GotFocus = soundAction;
    /// document.Form.Fields.Add(submitButton);
    /// //Save document to disk.
    /// document.Save("SoundAction.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Create a new PDF document.
    /// Dim document As PdfDocument = New PdfDocument()
    /// 'Creates a new page and adds it as the last page of the document
    /// Dim page As PdfPage = document.Pages.Add()
    /// 'Create font and font style.
    /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
    /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
    /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
    /// submitButton.Font = font
    /// submitButton.Text = "Apply"
    /// submitButton.BackColor = new PdfColor(181, 191, 203)
    /// //Create a new sound annotation
    /// Dim soundAction As PdfSoundAction = New PdfSoundAction("..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav")
    /// soundAction.Sound.Bits = 16
    /// soundAction.Sound.Channels = PdfSoundChannels.Stereo
    /// soundAction.Sound.Encoding = PdfSoundEncoding.Signed
    /// soundAction.Volume = 0.9F
    /// soundAction.Synchronous = True
    /// soundAction.Mix = True
    /// submitButton.Actions.GotFocus = soundAction
    /// document.Form.Fields.Add(submitButton)
    /// 'Save document to disk.
    /// document.Save("SoundAction.pdf")
    /// </code>
    /// </example>
    public class PdfSoundAction : PdfAction
    {
        #region Fields
        /// <summary>
        /// Internal variable to store the volume at which to play the sound.
        /// </summary>
        private float m_volume = 1.0f;

        /// <summary>
        /// Internal variable to store sound.
        /// </summary>
        private PdfSound m_sound = null;

        /// <summary>
        /// Internal variable to store value whether to play sound synchronously or asynchronously.
        /// </summary>
        private bool m_synchronous = false;

        /// <summary>
        /// Internal variable to store value whether to repeat playing.
        /// </summary>
        private bool m_repeat = false;

        /// <summary>
        /// Internal variable to store value whether to mix this sound with any other sound already playing.
        /// </summary>
        private bool m_mix = false;
        #endregion

        #region Constructor
#if SILVERLIGHT
        /// <summary>
        /// Security Critical :  Initializes a new instance of the <see cref="PdfSoundAction"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        ///  Initializes a new instance of the <see cref="PdfSoundAction"/> class.
        /// </summary>
        /// <param name="fileName">Name of the sound file.</param>
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
        /// submitButton.Text = "Apply";
        ///  //Create sound action
        /// PdfSoundAction soundAction = new PdfSoundAction(@"..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav");
        /// soundAction.Sound.Bits = 16;
        /// soundAction.Sound.Channels = PdfSoundChannels.Stereo;
        /// soundAction.Sound.Encoding = PdfSoundEncoding.Signed;
        /// soundAction.Volume = 0.9f;
        /// soundAction.Synchronous = true;
        /// soundAction.Mix = true;
        /// //Set the sound action to submit button
        /// submitButton.Actions.GotFocus = soundAction;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SoundAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Font = font
        /// submitButton.Text = "Apply"
        /// submitButton.BackColor = new PdfColor(181, 191, 203)
        /// //Create a new sound annotation
        /// Dim soundAction As PdfSoundAction = New PdfSoundAction("..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav")
        /// soundAction.Sound.Bits = 16
        /// soundAction.Sound.Channels = PdfSoundChannels.Stereo
        /// soundAction.Sound.Encoding = PdfSoundEncoding.Signed
        /// soundAction.Volume = 0.9F
        /// soundAction.Synchronous = True
        /// soundAction.Mix = True
        /// submitButton.Actions.GotFocus = soundAction
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SoundAction.pdf")
        /// </code>
        /// </example>
        public PdfSoundAction(string fileName)
            : base()
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            m_sound = new PdfSound(fileName);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the volume at which to play the sound, in the range -1.0 to 1.0.
        /// </summary>
        /// <value>The volume of the sound.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        ///  //Create sound action
        /// PdfSoundAction soundAction = new PdfSoundAction(@"..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav");
        /// soundAction.Volume = 0.9f;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SoundAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Font = font
        /// submitButton.Text = "Apply"
        /// submitButton.BackColor = new PdfColor(181, 191, 203)
        ///  'Create sound action
        /// Dim soundAction As PdfSoundAction = New PdfSoundAction("..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav")
        /// soundAction.Volume = 0.9F
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SoundAction.pdf")
        /// </code>
        /// </example>
        public float Volume
        {
            get
            {
                return m_volume;
            }
           
            set
            {
                if (value > 1.0f || value < -1.0f)
                {
                    throw new ArgumentOutOfRangeException("Volume");
                }

                if (m_volume != value)
                {
                    m_volume = value;
                    Dictionary.SetNumber(DictionaryProperties.Volume, m_volume);
                }
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical :  Gets or sets the name of the sound file.
        /// </summary>
        //[System.Security.SecurityCritical]
#else
        /// <summary>
        /// Gets or sets the name of the sound file.
        /// </summary>
#endif     
        /// <value>The name of the sound file.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        ///  //Create sound action
        /// PdfSoundAction soundAction = new PdfSoundAction(@"..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav");
        /// //Get the fileName form soundAction
        /// string fileName=soundAction.FileName;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SoundAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Font = font
        /// submitButton.Text = "Apply"
        /// submitButton.BackColor = new PdfColor(181, 191, 203)
        /// //Create a new sound annotation
        /// Dim soundAction As PdfSoundAction = New PdfSoundAction("..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav")
        /// //Get the fileName form soundAction
        /// Dim fileName Aas string =soundAction.FileName;
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SoundAction.pdf")
        /// </code>
        /// </example>
        public string FileName
        {
            get
            {
                return m_sound.FileName;
            }
           
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("FileName");
                }

                if (value.Length == 0)
                {
                    throw new ArgumentException("FileName can't be an empty string.");
                }

                if (value != m_sound.FileName)
                {
                    m_sound.FileName = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the sound.
        /// </summary>
        /// <value><see cref="PdfSound"/> represents the sound.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        ///  //Create sound action
        /// PdfSoundAction soundAction = new PdfSoundAction(@"..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav");
        /// soundAction.Sound.Bits = 16;
        /// soundAction.Sound.Channels = PdfSoundChannels.Stereo;
        /// soundAction.Sound.Encoding = PdfSoundEncoding.Signed;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SoundAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Font = font
        /// submitButton.Text = "Apply"
        /// submitButton.BackColor = new PdfColor(181, 191, 203)
        /// //Create a new sound annotation
        /// Dim soundAction As PdfSoundAction = New PdfSoundAction("..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav")
        /// soundAction.Sound.Bits = 16
        /// soundAction.Sound.Channels = PdfSoundChannels.Stereo
        /// soundAction.Sound.Encoding = PdfSoundEncoding.Signed
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SoundAction.pdf")
        /// </code>
        /// </example>
        public PdfSound Sound
        {
            get
            {
                return m_sound;
            }
           
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Sound");
                }

                if (value != m_sound)
                {
                    m_sound = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value whether to play the sound synchronously or asynchronously.
        /// If this flag is true, the viewer application retains control, allowing no further 
        /// user interaction other than canceling the sound, until the sound has been 
        /// completely played. Default value: false.
        /// </summary>
        /// <value><c>true</c> if synchronous; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        ///  //Create sound action
        /// PdfSoundAction soundAction = new PdfSoundAction(@"..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav");
        /// soundAction.Synchronous = true;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SoundAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Font = font
        /// submitButton.Text = "Apply"
        /// submitButton.BackColor = new PdfColor(181, 191, 203)
        ///  'Create sound action
        /// Dim soundAction As PdfSoundAction = New PdfSoundAction("..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav")
        /// soundAction.Synchronous = True
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SoundAction.pdf")
        /// </code>
        /// </example>
        public bool Synchronous
        {
            get
            {
                return m_synchronous;
            }
            
            set
            {
                if (m_synchronous != value)
                {
                    m_synchronous = value;
                    Dictionary.SetBoolean(DictionaryProperties.Synchronous, m_synchronous);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to repeat the sound indefinitely. 
        /// If this entry is present, the <see cref="Synchronous"/> property is ignored. Default value: false.
        /// </summary>
        /// <value><c>true</c> if repeat; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        ///  //Create sound action
        /// PdfSoundAction soundAction = new PdfSoundAction(@"..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav");
        /// soundAction.Repeat = true;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SoundAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Font = font
        /// submitButton.Text = "Apply"
        /// submitButton.BackColor = new PdfColor(181, 191, 203)
        ///  'Create sound action
        /// Dim soundAction As PdfSoundAction = New PdfSoundAction("..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav")
        /// soundAction.Repeat = True
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SoundAction.pdf")
        /// </code>
        /// </example>
        public bool Repeat
        {
            get
            {
                return m_repeat;
            }
            
            set
            {
                if (m_repeat != value)
                {
                    m_repeat = value;
                    Dictionary.SetBoolean(DictionaryProperties.Repeat, m_repeat);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to mix this sound with any other 
        /// sound already playing. If this flag is false, any previously playing sound is 
        /// stopped before starting this sound; this can be used to stop a repeating 
        /// sound. Default value: false.
        /// </summary>
        /// <value><c>true</c> if mix; otherwise, <c>false</c>.</value>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create a new PdfButtonField
        /// PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
        /// submitButton.Bounds = new RectangleF(25, 160, 100, 20);
        /// submitButton.Text = "Apply";
        ///  //Create sound action
        /// PdfSoundAction soundAction = new PdfSoundAction(@"..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav");
        /// soundAction.Mix = true;
        /// document.Form.Fields.Add(submitButton);
        /// //Save document to disk.
        /// document.Save("SoundAction.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Create font and font style.
        /// Dim font As PdfFont = New PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold)
        /// Dim submitButton As PdfButtonField  = New PdfButtonField(page, "submitButton")
        /// submitButton.Bounds = New RectangleF(25, 160, 100, 20)
        /// submitButton.Font = font
        /// submitButton.Text = "Apply"
        /// submitButton.BackColor = new PdfColor(181, 191, 203)
        /// Dim soundAction As PdfSoundAction = New PdfSoundAction("..\..\..\..\..\..\..\..\..\Common\Data\PDF\startup.wav")
        /// soundAction.Mix = True
        /// document.Form.Fields.Add(submitButton)
        /// 'Save document to disk.
        /// document.Save("SoundAction.pdf")
        /// </code>
        /// </example>
        public bool Mix
        {
            get
            {
                return m_mix;
            }
            
            set
            {
                if (value != m_mix)
                {
                    m_mix = value;
                    Dictionary.SetBoolean(DictionaryProperties.Mix, m_mix);
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            Dictionary.BeginSave += new SavePdfPrimitiveEventHandler(Dictionary_BeginSave);
            Dictionary.SetProperty(DictionaryProperties.S, new PdfName(DictionaryProperties.Sound));
        }

        /// <summary>
        /// Handles the BeginSave event of the Dictionary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
        private void Dictionary_BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            Dictionary.SetProperty(DictionaryProperties.Sound, new PdfReferenceHolder(m_sound));
        }
        #endregion
    }
}
