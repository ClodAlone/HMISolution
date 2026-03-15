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
#if NETFX_CORE || WP
using Windows.Storage;
#endif

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Class which represents embedded file into Pdf document.
    /// </summary>
    internal class EmbeddedFile : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store file name.
        /// </summary>
        private string m_fileName = String.Empty;

        /// <summary>
        /// Internal variable to store file path.
        /// </summary>
        private string m_filePath = String.Empty;

        /// <summary>
        /// Internal variable to store mime type.
        /// </summary>
        private string m_mimeType = String.Empty;

        /// <summary>
        /// Internal variable to store embedded data.
        /// </summary>
        private byte[] m_data;

        /// <summary>
        /// Internal variable to store specification of the embedded file.
        /// </summary>
        private EmbeddedFileParams m_params = new EmbeddedFileParams();

        /// <summary>
        /// Internal variable to store stream.
        /// </summary>
        private PdfStream m_stream = new PdfStream();
        #endregion

        #region Constructors
#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="EmbeddedFile"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedFile"/> class.
        /// </summary>
#endif
        /// <param name="fileName">Name of the file.</param>
        public EmbeddedFile(string fileName)
            : base()
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            Initialize();
            FileName = fileName;
            FilePath = fileName;
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical :  Initializes a new instance of the <see cref="EmbeddedFile"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        ///  Initializes a new instance of the <see cref="EmbeddedFile"/> class.
        /// </summary>
#endif
        /// <param name="fileName">Name of the file.</param>
        /// <param name="data">The data.</param>
        public EmbeddedFile(string fileName, byte[] data)
            : this(fileName)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            Data = data;
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="EmbeddedFile"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedFile"/> class.
        /// </summary>
#endif
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stream">The stream.</param>
        public EmbeddedFile(string fileName, Stream stream)
            : this(fileName)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            int bytesToRead = (int)stream.Length;
            int totalBytesRead = 0;
            int bytesRead = 0;

            m_data = new byte[stream.Length];

            while (bytesToRead > 0)
            {
                bytesRead = stream.Read(m_data, totalBytesRead, bytesToRead);
                totalBytesRead += bytesRead;
                bytesToRead -= bytesRead;
            }
            m_stream.InternalStream.Write(m_data, 0, m_data.Length);
        }
        #endregion

        #region Properties
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
                if (m_fileName != value)
                {
                    m_fileName = GetFileName(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the path of the file.
        /// </summary>
        /// <value>The path of the file.</value>
        internal string FilePath
        {
            get
            {
                return m_filePath;
            }

            set
            {
                if (m_filePath != value)
                {
                    m_filePath = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public byte[] Data
        {
            get
            {
                return m_data;
            }

            set
            {
                m_data = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the MIME.
        /// </summary>
        /// <value>The type of the MIME.</value>
        public string MimeType
        {
            get
            {
                return m_mimeType;
            }

            set
            {
                if (m_mimeType != value)
                {
                    m_mimeType = value;
                    m_stream.SetName(DictionaryProperties.Subtype, m_mimeType, true);
                }
            }
        }

        /// <summary>
        /// Gets the params.
        /// </summary>
        /// <value>The params.</value>
        internal EmbeddedFileParams Params
        {
            get
            {
                return m_params;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes an instance.
        /// </summary>
        protected void Initialize()
        {
            m_stream.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties.EmbeddedFile));
            m_stream.SetProperty(DictionaryProperties.Params, m_params);

            m_stream.BeginSave += new SavePdfPrimitiveEventHandler(Stream_BeginSave);
        }

        /// <summary>
        /// Saves an object.
        /// </summary>
#if NETFX_CORE || WP
        protected async void Save()
        {
            if (m_data == null)
            {               
                m_stream.Clear();
                m_stream.InternalStream.Write(m_data, 0, m_data.Length);
                m_params.Size = m_data.Length;
            }
        }

#else
        protected void Save()
        {
            if (m_data == null)
            {
                using (FileStream fileStream = new FileStream(m_filePath,
                    FileMode.Open, FileAccess.Read))
                {
                    m_data = PdfStream.StreamToBytes(fileStream);
                }
            }

            m_stream.Clear();
            m_stream.InternalStream.Write(m_data, 0, m_data.Length);
            m_params.Size = m_data.Length;
        }
#endif
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
        /// Get attachment's name through names array.
        /// </summary>
        /// <returns>Attachment's file name.</returns>
        private string GetFileName(string attachmentName)
        {
            char[] separator = { '\\','/' };
            string[] fileName = attachmentName.Split(separator);
            return fileName[fileName.Length - 1];
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
                return m_stream;
            }
        }
        #endregion
    }
}
