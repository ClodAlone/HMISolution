#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Globalization;
using System.IO;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
#if NETFX_CORE || WP
using Windows.Storage;
#endif

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents sound embedded into pdf document.
    /// </summary>
    public class PdfSound : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store sampling rate.
        /// </summary>
        private int m_rate = 22050;

        /// <summary>
        /// Internal variable to store encoding format.
        /// </summary>
        private PdfSoundEncoding m_encoding = PdfSoundEncoding.Raw;

        /// <summary>
        /// Internal variable to store number of sound channels.
        /// </summary>
        private PdfSoundChannels m_channels = PdfSoundChannels.Mono;

        /// <summary>
        /// Internal variable to store number of bits per sample value per channel.
        /// </summary>
        private int m_bits = 8;

        /// <summary>
        /// Internal variable to store sound file name.
        /// </summary>
        private string m_fileName = String.Empty;

        /// <summary>
        /// Internal variable to store stream.
        /// </summary>
        private PdfStream m_stream = new PdfStream();
        #endregion

        #region Constructors
#if SILVERLIGHT
        /// <summary>
        /// Security Critical :  Initializes a new instance of the <see cref="PdfSound"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        ///  Initializes a new instance of the <see cref="PdfSound"/> class.
        /// </summary>
#endif
        /// <param name="fileName">Name of the file.</param>
        public PdfSound(string fileName)
            : base()
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
#if !NETFX_CORE && !WP
            Utils.CheckFilePath(fileName);
#endif
            FileName = fileName;
            m_stream.SetString(DictionaryProperties.T, fileName);
            m_stream.SetProperty(DictionaryProperties.R, new PdfNumber(m_rate));
            m_stream.BeginSave += new SavePdfPrimitiveEventHandler(Stream_BeginSave);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSound"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="test">if set to <c>true</c> [test].</param>
        internal PdfSound(string fileName, bool test)
            : base()
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            FileName = fileName;
            m_stream.SetString(DictionaryProperties.T, fileName);
            m_stream.SetProperty(DictionaryProperties.R, new PdfNumber(m_rate));
            m_stream.BeginSave += new SavePdfPrimitiveEventHandler(Stream_BeginSave);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSound"/> class.
        /// </summary>
        internal PdfSound()
            : base()
        {
            m_stream.SetProperty(DictionaryProperties.R, new PdfNumber(m_rate));
            m_stream.BeginSave += new SavePdfPrimitiveEventHandler(Stream_BeginSave);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the sampling rate, in samples per second (in Hz).
        /// </summary>
        public int Rate
        {
            get
            {
                return m_rate;
            }

            set
            {
                if (m_rate != value)
                {
                    m_rate = value;
                    m_stream.SetNumber(DictionaryProperties.R, m_rate);
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of bits per sample value per channel.
        /// </summary>
        public int Bits
        {
            get
            {
                return m_bits;
            }

            set
            {
                if (m_bits != value)
                {
                    m_bits = value;
                    m_stream.SetNumber(DictionaryProperties.B, m_bits);
                }
            }
        }

        /// <summary>
        /// Gets or sets the encoding format for the sample data.
        /// </summary>
        public PdfSoundEncoding Encoding
        {
            get
            {
                return m_encoding;
            }

            set
            {
                if (m_encoding != value)
                {
                    m_encoding = value;
                    m_stream.SetName(DictionaryProperties.E, m_encoding.ToString());
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of sound channels.
        /// </summary>
        public PdfSoundChannels Channels
        {
            get
            {
                return m_channels;
            }

            set
            {
                if (m_channels != value)
                {
                    m_channels = value;
                    m_stream.SetNumber(DictionaryProperties.C, (int)m_channels);
                }
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Gets or sets the name of the file.
        /// </summary>
        //[System.Security.SecurityCritical]
#else
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
#endif
        /// <value>The name of the file.</value>
        public string FileName
        {
            get
            {
                return m_fileName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("FileName");
                }

                if (value.Length == 0)
                {
                    throw new ArithmeticException("FileName can't be empty string.");
                }

#if NETFX_CORE || WP
                m_fileName = value;
#else
                m_fileName = Path.GetFullPath(value);
#endif
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Handles the BeginSave event of the Stream control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
        private void Stream_BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            Save();
        }

        /// <summary>
        /// Saves an instance.
        /// </summary>
#if NETFX_CORE || WP
        protected async void Save()
        {

            StorageFolder folder = Windows.Storage.KnownFolders.DocumentsLibrary;
            StorageFile stFile = await folder.GetFileAsync(FileName);
            Windows.Storage.Streams.IRandomAccessStream fileStream = await stFile.OpenAsync(FileAccessMode.Read);
            byte[] bytes = PdfStream.StreamToBigEndian(fileStream.AsStream());
            m_stream.Clear();
            m_stream.InternalStream.Write(bytes, 0, bytes.Length);         

        }
#else
protected void Save()
        {
            using (FileStream fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = PdfStream.StreamToBigEndian(fileStream);
                m_stream.Clear();
                m_stream.InternalStream.Write(bytes, 0, bytes.Length);
            }
        }
#endif
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
                return m_stream;
            }
        }
        #endregion
    }
}
