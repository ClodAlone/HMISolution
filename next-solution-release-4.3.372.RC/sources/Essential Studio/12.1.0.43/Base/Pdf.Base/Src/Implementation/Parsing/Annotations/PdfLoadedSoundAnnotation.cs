#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Globalization;
using System.IO;
#if !NETFX_CORE && !WP
using Syncfusion.Compression;
#endif
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
#if NETFX_CORE || WP
using Windows.Storage;
#endif

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the loaded sound annotation class.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    /// //Load an existing document.
    /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
    /// //Gets the annotation from loaded document.
    /// PdfLoadedSoundAnnotation soundAnnotation = document.Pages[1].Annotations[5] as PdfLoadedSoundAnnotation;
    /// //Sets the sound annotation border
    /// soundAnnotation.Border.Width = 4;
    /// soundAnnotation.Border.HorizontalRadius = 20;
    /// soundAnnotation.Border.VerticalRadius = 30;
    /// //Set the pdf sound annotation icon.
    /// soundAnnotation.Icon = PdfSoundIcon.Speaker;
    /// //Sets the pdf sound.
    ///  PdfSound sound = new PdfSound("Startup.wav");
    ///  soundAnnotation.Sound=sound;
    /// //Save the document.
    /// document.Save("SoundAnnotation.pdf");
    /// </code>
    /// <code lang="VB">
    /// 'Load an existing document.
    /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
    /// 'Gets the annotation from loaded document.
    /// Dim soundAnnotation As PdfLoadedSoundAnnotation = document.Pages(1).Annotations(5) as PdfLoadedSoundAnnotation
    /// 'Sets the sound annotation border
    /// soundAnnotation.Border.Width = 4
    /// soundAnnotation.Border.HorizontalRadius = 20
    /// soundAnnotation.Border.VerticalRadius = 30
    /// 'Set the pdf sound annotation icon.
    /// soundAnnotation.Icon = PdfSoundIcon.Speaker
    /// Sets the pdf sound.
    /// Dim sound As PdfSound  = New PdfSound("Startup.wav")
    /// soundAnnotation.Sound=sound
    /// 'Save the document.
    /// document.Save("SoundAnnotation.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLoadedAttachmentAnnotation"/> Class
    /// <seealso cref="PdfLoadedDocumentLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedFileLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedLineAnnotation"/> Class
    /// <seealso cref="PdfLoadedPopupAnnotation"/> Class
    /// <seealso cref="PdfLoadedRubberStampAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextMarkupAnnotation"/> Class
    /// <seealso cref="PdfLoadedTextWebLinkAnnotation"/> Class
    /// <seealso cref="PdfLoadedUriAnnotation"/> Class
    public class PdfLoadedSoundAnnotation : PdfLoadedStyledAnnotation
    {
        #region Fields
        /// <summary>
        /// CrossTable
        /// </summary>
        private PdfCrossTable m_crossTable;
        /// <summary>
        /// Internal variable to store sound.
        /// </summary>
        private PdfSound m_sound;
        /// <summary>
        /// Dictionary
        /// </summary>
        private PdfDictionary m_dictionary;
        /// <summary>
        /// Indicates the sound icon of the annotation.
        /// </summary>
        private PdfSoundIcon m_icon;
        /// <summary>
        /// Indicates the appearance of the annotation.
        /// </summary>
        private PdfAppearance m_appearance;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the sound of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedSoundAnnotation soundAnnotation = document.Pages[1].Annotations[5] as PdfLoadedSoundAnnotation;
        /// //Sets the pdf sound.
        /// PdfSound sound = new PdfSound("Startup.wav");
        /// soundAnnotation.Sound=sound;
        /// //Save the document.
        /// document.Save("SoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim soundAnnotation As PdfLoadedSoundAnnotation = document.Pages(1).Annotations(5) as PdfLoadedSoundAnnotation
        /// Sets the pdf sound.
        /// Dim sound As PdfSound  = New PdfSound("Startup.wav")
        /// soundAnnotation.Sound=sound
        /// 'Save the document.
        /// document.Save("SoundAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfSound Sound
        {
            get
            {
                return GetSound();
            }
            set
            {
                m_sound = value;
                Dictionary.Remove(DictionaryProperties.Sound);
                PdfReferenceHolder rh = new PdfReferenceHolder(m_sound as IPdfWrapper);
                Dictionary.SetProperty(DictionaryProperties.Sound, rh);
                Dictionary.Modify();
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Gets the filename of the annotation.
        /// </summary>
        //[System.Security.SecurityCritical]
#else
        /// <summary>
        /// Gets the filename of the annotation.
        /// </summary>
#endif
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedSoundAnnotation soundAnnotation = document.Pages[1].Annotations[5] as PdfLoadedSoundAnnotation;
        /// 'Gets the file name
        /// string filename =soundAnnotation.FileName;
        /// //Save the document.
        /// document.Save("SoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim soundAnnotation As PdfLoadedSoundAnnotation = document.Pages(1).Annotations(5) as PdfLoadedSoundAnnotation
        /// 'Gets the file name
        /// Dim filename As String=soundAnnotation.FileName
        /// 'Save the document.
        /// document.Save("SoundAnnotation.pdf")
        /// </code>
        /// </example>
        public string FileName
        {
            get
            {
                return GetFileName();
            }
            //set
            //{
            //    PdfDictionary dic = Dictionary;
            //    if (Dictionary.ContainsKey(DictionaryProperties.Sound))
            //    {
            //        dic = m_crossTable.GetObject(Dictionary[DictionaryProperties.Sound]) as PdfDictionary;
            //        dic.SetString(DictionaryProperties.T, value);
            //        Dictionary.Modify();
            //    }
            //}
        }

        /// <summary>
        /// Gets or sets the icon of the annotation.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //Load an existing document.
        /// PdfLoadedDocument document = new PdfLoadedDocument(@"..\..\Annotations.pdf");
        /// //Gets the annotation from loaded document.
        /// PdfLoadedSoundAnnotation soundAnnotation = document.Pages[1].Annotations[5] as PdfLoadedSoundAnnotation;
        /// //Set the pdfsound icon
        /// soundAnnotation.Icon = PdfSoundIcon.Speaker;
        /// //Save the document.
        /// document.Save("SoundAnnotation.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Load an existing document.
        /// Dim document As New PdfLoadedDocument("..\..\Annotations.pdf")
        /// 'Gets the annotation from loaded document.
        /// Dim soundAnnotation As PdfLoadedSoundAnnotation = document.Pages(1).Annotations(5) as PdfLoadedSoundAnnotation
        /// 'Set the pdfsound icon
        /// soundAnnotation.Icon = PdfSoundIcon.Speaker
        /// 'Save the document.
        /// document.Save("SoundAnnotation.pdf")
        /// </code>
        /// </example>
        public PdfSoundIcon Icon
        {
            get
            {
                return GetIcon();
            }
            set
            {
                m_icon = value;
                Dictionary.SetName(DictionaryProperties.Name, m_icon.ToString());
            }
        }

        ///// <summary>
        ///// Gets sound of the annotation.
        ///// </summary>
        //public PdfAppearance Appearance
        //{
        //    get
        //    {
        //        return m_appearance;
        //    }
        //    //set
        //    //{
        //    //    m_appearance = value;
        //    //    Dictionary.SetName(DictionaryProperties.Name, m_name.ToString());
        //    //}
        //}

        #endregion

        #region Implementations
        /// <summary>
        /// Gets the filename.
        /// </summary>
        /// <returns>File name</returns>
        private string GetFileName()
        {
            string filename = String.Empty;
            if (Dictionary.ContainsKey(DictionaryProperties.Sound))
            {
                PdfDictionary soundDic = m_crossTable.GetObject(Dictionary[DictionaryProperties.Sound]) as PdfDictionary;
                PdfString text = soundDic[DictionaryProperties.T] as PdfString;
                filename = text.Value.ToString();
            }
            return filename;
        }
        /// <summary>
        /// Gets the sound annotation icon.
        /// </summary>
        /// <returns>Sound annotation icon</returns>
        private PdfSoundIcon GetIcon()
        {
            PdfSoundIcon iconType = PdfSoundIcon.Mic;
            if (Dictionary.ContainsKey(DictionaryProperties.Name))
            {
                PdfName name = Dictionary[DictionaryProperties.Name] as PdfName;
                iconType = GetIconName(name.Value.ToString());
            }
            return iconType;
        }
        /// <summary>
        /// Gets the icon name
        /// </summary>
        /// <param name="iType">Icon type</param>
        /// <returns>Sound icon</returns>
        private PdfSoundIcon GetIconName(string iType)
        {
            PdfSoundIcon m_iconType = PdfSoundIcon.Mic;
            switch (iType)
            {
                case "Mic":
                    m_iconType = PdfSoundIcon.Mic;
                    break;
                case "Speaker":
                    m_iconType = PdfSoundIcon.Speaker;
                    break;
            }
            return m_iconType;
        }
        /// <summary>
        /// Gets the sound file.
        /// </summary>
        /// <returns>Sound file</returns>
        private PdfSound GetSound()
        {
            string fileName = GetFileName();
            PdfSound sound = new PdfSound(fileName);

            if (Dictionary.ContainsKey(DictionaryProperties.Sound))
            {
                PdfDictionary soundDic = m_crossTable.GetObject(Dictionary[DictionaryProperties.Sound]) as PdfDictionary;
                if (soundDic.ContainsKey(DictionaryProperties.B))
                    sound.Bits = (soundDic[DictionaryProperties.B] as PdfNumber).IntValue;
                if (soundDic.ContainsKey(DictionaryProperties.R))
                    sound.Rate = (soundDic[DictionaryProperties.R] as PdfNumber).IntValue;
                if (soundDic.ContainsKey(DictionaryProperties.C))
                {
                    int channel = (soundDic[DictionaryProperties.C] as PdfNumber).IntValue;
                    if (channel == 1)
                        sound.Channels = PdfSoundChannels.Mono;
                    else
                        sound.Channels = PdfSoundChannels.Stereo;
                }
                if (soundDic.ContainsKey(DictionaryProperties.E))
                {
                    PdfName eName = soundDic[DictionaryProperties.E] as PdfName;
                    sound.Encoding = GetEncodigType(eName.Value.ToString());
                }
            }
            return sound;
        }
        /// <summary>
        /// Gets the sound annottation encoding type
        /// </summary>
        /// <param name="eType">Encoding type</param>
        /// <returns>Encoding type</returns>
        private PdfSoundEncoding GetEncodigType(string eType)
        {
            PdfSoundEncoding encodeType = PdfSoundEncoding.Raw;
            switch (eType)
            {
                case "Raw":
                    encodeType = PdfSoundEncoding.Raw;
                    break;
                case "Signed":
                    encodeType = PdfSoundEncoding.Signed;
                    break;
                case "MuLaw":
                    encodeType = PdfSoundEncoding.MuLaw;
                    break;
                case "ALaw":
                    encodeType = PdfSoundEncoding.ALaw;
                    break;
            }
            return encodeType;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedLineAnnotation"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        /// <param name="rectangle">The rectangle</param>
        /// <param name="fileName">The filename</param>
        internal PdfLoadedSoundAnnotation(PdfDictionary dictionary, PdfCrossTable crossTable, RectangleF rectangle)
            : base(dictionary, crossTable)
        {
            PdfDictionary dic = PdfCrossTable.Dereference(dictionary[DictionaryProperties.Sound]) as PdfDictionary;
            PdfString m_fileName = PdfCrossTable.Dereference(dic[DictionaryProperties.T]) as PdfString;
            if (m_fileName != null)
            {
                string fileName = m_fileName.Value;
                PdfReferenceHolder refh = dictionary[DictionaryProperties.Sound] as PdfReferenceHolder;
                if (refh == null)
                    throw new ArgumentNullException();
                PdfStream soundstream = refh.Object as PdfStream;
                byte[] bytes = soundstream.Data;
                string path = System.IO.Path.GetFileName(fileName);
#if NETFX_CORE || WP
                //StorageFolder folder = Windows.Storage.KnownFolders.DocumentsLibrary;
                //StorageFile stFile = await folder.CreateFileAsync(filename);
#else
                FileStream file = File.Create(path);
                file.Write(bytes, 0, bytes.Length);
                file.Close();
#endif
                m_dictionary = dictionary;
                m_crossTable = crossTable;
                m_sound = new PdfSound(fileName, true);
            }
            else
            {
                PdfReferenceHolder refh = dictionary[DictionaryProperties.Sound] as PdfReferenceHolder;
                if (refh == null)
                    throw new ArgumentNullException();
                //PdfStream soundstream = refh.Object as PdfStream;
                //byte[] bytes = soundstream.Data;
                //FileStream file = File.Create("sample.wav");
                //file.Write(bytes, 0, bytes.Length);
                //file.Close();
                m_dictionary = dictionary;
                m_crossTable = crossTable;
                m_sound = new PdfSound();
            }
        }

        #endregion

    }
}
