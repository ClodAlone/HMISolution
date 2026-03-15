#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
    /// Represents the base class for 3D annotation.
    /// </summary>
    internal class Pdf3DBase : IPdfWrapper
    {
        #region Fields
        private string m_fileName = String.Empty;

        /// <summary>
        /// Internal variable to store stream.
        /// </summary>
        private Pdf3DStream m_stream = new Pdf3DStream();

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the stream.
        /// </summary>
        /// <value>The stream.</value>
        public Pdf3DStream Stream
        {
            get
            {
                return this.m_stream;
            }

            set
            {
                this.m_stream = value;
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
                return this.m_fileName;
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
                this.m_fileName = value;
#else
                this.m_fileName = Path.GetFullPath(value);
#endif
            }
        }
        #endregion

        #region Constructors
#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="PdfSound"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSound"/> class.
        /// </summary>
#endif
        /// <param name="fileName">Name of the file.</param>
        public Pdf3DBase(string fileName)
            : base()
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
#if !NETFX_CORE && !WP
            Utils.CheckFilePath(fileName);
#endif
            this.FileName = fileName;
            this.m_stream.BeginSave += new SavePdfPrimitiveEventHandler(this.Stream_BeginSave);
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
            this.Save();
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
            using (FileStream fileStream = new FileStream(this.FileName, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = Pdf3DStream.StreamToBytes(fileStream);
                this.m_stream.Clear();
                this.m_stream.InternalStream.Write(bytes, 0, bytes.Length);
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
                return this.m_stream;
            }
        }
        #endregion
    }
}
