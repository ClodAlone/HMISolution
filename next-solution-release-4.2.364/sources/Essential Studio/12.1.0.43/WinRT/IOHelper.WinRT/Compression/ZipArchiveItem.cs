#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using Windows.Storage;
using compression = System.IO.Compression;
namespace Syncfusion.Compression.Zip
{
    public class ZipArchiveItem
    {
        /// <summary>
        /// Name of the archive item.
        /// </summary>
        private string m_strItemName;
        /// <summary>
        /// Compression method.
        /// </summary>
        private CompressionMethod m_compressionMethod = CompressionMethod.Deflated;
        /// <summary>
        /// Compression level.
        /// </summary>
        private compression.CompressionLevel m_compressionLevel = compression.CompressionLevel.NoCompression;
        /// <summary>
        /// Stream with item's data.
        /// </summary>
        private Stream m_streamData;
        private ZipArchive m_archive;
        private ZipArchive zipArchive;
        private Stream data;
        private bool bControlStream;
        private Windows.Storage.FileAttributes attributes;
        private FileAttributes m_iExternalAttributes;
        public Stream DataStream
        {
            get
            {
                return m_streamData;
            }
            internal set
            {
                m_streamData = value;
            }
        }
        /// <summary>
        /// Gets / sets item's external attributes.
        /// </summary>
        public FileAttributes ExternalAttributes
        {
            get
            {
                return (FileAttributes)m_iExternalAttributes;
            }
            set
            {
                m_iExternalAttributes = value; 
            }
        }
        public string ItemName
        {
            get
            {
                return m_strItemName;
            }
            internal set
            {
                m_strItemName = value;
            }
        }
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal ZipArchiveItem(ZipArchive archive)
        {
            m_streamData = new MemoryStream();
            m_archive = archive;
        }

        /// <summary>
        /// Creates new instance of the zip item.
        /// </summary>
        /// <param name="itemName">Name of the item (can be relative or absolute path).</param>
        /// <param name="streamData">Stream data.</param>
        /// <param name="controlStream">
        /// Indicates whether item controls stream and must close it when item finish its work.
        /// </param>
        /// <param name="attributes"></param>
        public ZipArchiveItem(ZipArchive archive, string itemName, Stream streamData) :
            this(archive)
        {
            m_strItemName = itemName;
            m_streamData = streamData;
        }
        internal ZipArchiveItem Clone()
        {
            ZipArchiveItem result = (ZipArchiveItem)MemberwiseClone();
            result.m_streamData = CloneStream(m_streamData);
            return result;
        }
        public ZipArchiveItem(ZipArchive zipArchive, string itemName, Stream data, bool bControlStream, Windows.Storage.FileAttributes attributes)
        {
            // TODO: Complete member initialization
            this.zipArchive = zipArchive;
            this.ItemName = itemName;
            this.DataStream = data;
            this.bControlStream = bControlStream;
            this.attributes = attributes;
        }
        /// <summary>
        /// Creates copy of the stream.
        /// </summary>
        /// <param name="stream">Stream to copy.</param>
        /// <returns>Created stream.</returns>
        public static Stream CloneStream(Stream stream)
        {
            if (stream == null)
                return null;

            long lStartPos = stream.Position;

            MemoryStream result = new MemoryStream((int)stream.Length);
            stream.Position = 0;
            const int BufferSize = 32768;
            byte[] arrBuffer = new byte[BufferSize];
            int iReadSize;

            while ((iReadSize = stream.Read(arrBuffer, 0, BufferSize)) != 0)
            {
                result.Write(arrBuffer, 0, iReadSize);
            }

            stream.Position = lStartPos;
            result.Position = lStartPos;

            return result;
        }
        /// <summary>
        /// Updates internal data stream.
        /// </summary>
        /// <param name="newDataStream">New stream to set.</param>
        /// <param name="controlStream">Indicates whether item should conrol new stream.</param>
        public void Update(Stream newDataStream, bool controlStream)
        {
            this.DataStream = newDataStream;
        }
    }
}
