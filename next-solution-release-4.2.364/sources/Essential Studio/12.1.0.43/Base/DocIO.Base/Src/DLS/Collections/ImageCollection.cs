#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections.Generic;
using Syncfusion.DocIO.ReaderWriter.Security;
using System.IO;
#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
#else
using Image = System.Drawing.Image;
using System.Drawing.Imaging;
using System.IO.Compression;
#endif
#if WINRT
using Syncfusion.DocIO.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    internal class ImageCollection
    {
        #region Fields
        private Dictionary<int, ImageRecord> m_collection = new Dictionary<int, ImageRecord>();
        private List<int> m_removedImageIds = new List<int>();
        private int m_maxId;
        private WordDocument m_doc;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.ImageRecord"/> with the specified image id.
        /// </summary>
        /// <value></value>
        internal ImageRecord this[int imageId]
        {
            get
            {
                if (m_collection.ContainsKey(imageId))
                    return m_collection[imageId];
                return null;
            }
        }
        /// <summary>
        /// Gets the owner document.
        /// </summary>
        /// <value>The document.</value>
        internal WordDocument Document
        {
            get
            {
                return m_doc;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageCollection"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal ImageCollection(WordDocument doc)
        {
            m_doc = doc;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        internal void Add(ImageRecord image)
        {
            int imageId = 1;
            if (m_removedImageIds.Count > 0)
            {
                imageId = m_removedImageIds[0];
                m_removedImageIds.RemoveAt(0);
            }
            else if (m_collection.Count > 0)
                imageId = ++m_maxId;
            else
                m_maxId++;
            image.ImageId = imageId;
            m_collection.Add(imageId, image);
        }
        /// <summary>
        /// Removes the specified image id.
        /// </summary>
        /// <param name="imageId">The image id.</param>
        /// <returns></returns>
        internal bool Remove(int imageId)
        {
            if (m_collection.ContainsKey(imageId))
            {
                m_collection.Remove(imageId);
                m_removedImageIds.Add(imageId);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Clears this instance.
        /// </summary>
        internal void Clear()
        {
            foreach(ImageRecord imageRecord in m_collection.Values)
            {
                imageRecord.Close();
            }
            m_collection.Clear();
            m_removedImageIds.Clear();
            m_maxId = 0;
        }
        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="imageBytes">The image bytes.</param>
        internal ImageRecord LoadImage(byte[] imageBytes)
        {
            //Compute Hash
            HMACSHA1 hmacSha1 = new HMACSHA1();
            hmacSha1.Key = ImageRecord.HashKey;
            SecurityHelper securityHelper = new SecurityHelper();
            ImageRecord newImageRecord = null;
            foreach (ImageRecord imageRecord in m_collection.Values)
            {
                if (imageRecord.IsMetafile)
                    continue;
                if (imageRecord.m_imageBytes.Length == imageBytes.Length && securityHelper.CompareArray(imageRecord.ImageHash, hmacSha1.ComputeHash(imageBytes)))
                {
                    newImageRecord = imageRecord;
                    break;
                }
            }
            if (newImageRecord == null)
            {
                newImageRecord = new ImageRecord(m_doc, imageBytes);
                Add(newImageRecord);
            }
            imageBytes = null;
            newImageRecord.OccurenceCount++;

            return newImageRecord;
        }
        /// <summary>
        /// Loads the meta file image.
        /// </summary>
        /// <param name="imageBytes">The image bytes.</param>
        /// <param name="isCompressed">if set to <c>true</c> [is compressed].</param>
        /// <returns></returns>
        internal ImageRecord LoadMetaFileImage(byte[] imageBytes, bool isCompressed)
        {
            int length = imageBytes.Length;
            if (!isCompressed)
                imageBytes = CompressImageBytes(imageBytes);
            //Compute Hash
            HMACSHA1 hmacSha1 = new HMACSHA1();
            hmacSha1.Key = ImageRecord.HashKey;
            SecurityHelper securityHelper = new SecurityHelper();
            ImageRecord newImageRecord = null;
            foreach (ImageRecord imageRecord in m_collection.Values)
            {
                if (!imageRecord.IsMetafile)
                    continue;
                if (imageRecord.m_imageBytes.Length == imageBytes.Length && securityHelper.CompareArray(imageRecord.ImageHash, hmacSha1.ComputeHash(imageBytes)))
                {
                    newImageRecord = imageRecord;
                    break;
                }
            }
            if (newImageRecord == null)
            {
                newImageRecord = new ImageRecord(m_doc, imageBytes);
                Add(newImageRecord);
                if (!isCompressed)
                    newImageRecord.Length = length;
            }
            imageBytes = null;
            newImageRecord.OccurenceCount++;
            newImageRecord.IsMetafile = true;
            return newImageRecord;
        }
        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="imageBytes">The image bytes.</param>
        internal ImageRecord LoadXmlItemImage(byte[] imageBytes)
        {
            Image image = ImageRecord.GetImage(imageBytes);
            ImageRecord imageRecord = null;
#if SILVERLIGHT || WP || WINRT
            if (image.IsMetafile)
#else
            if (image is Metafile)
#endif
                imageRecord = LoadMetaFileImage(imageBytes, false);
            else
                imageRecord = LoadImage(imageBytes);
            return imageRecord;
        }
        /// <summary>
        /// Compresses the image bytes.
        /// </summary>
        /// <param name="imageBytes">The image bytes.</param>
        /// <returns></returns>
        private byte[] CompressImageBytes(byte[] imageBytes)
        {
            byte[] compressedImage;
            try
            {
                MemoryStream compressedStream = new MemoryStream();
                Compression.CompressedStreamWriter compressedWriter = new Compression.CompressedStreamWriter(compressedStream, true);
                compressedWriter.Write(imageBytes, 0, imageBytes.Length, true);
#if WINRT
                compressedStream.Dispose();
#else
                compressedStream.Close();
#endif
                compressedImage = compressedStream.ToArray();
            }
            catch
            {
#if !SILVERLIGHT && !WP
                MemoryStream compressedStream = new MemoryStream();
                GZipStream compressedzipStream = new GZipStream(compressedStream, CompressionMode.Compress, true);
                compressedzipStream.Write(imageBytes, 0, imageBytes.Length);
                compressedzipStream.Close();
                compressedImage = compressedStream.ToArray();
                compressedStream.Close();
#else
                compressedImage = imageBytes;
#endif
            }
            return compressedImage;
        }
        /// <summary>
        /// Decompresses the image bytes.
        /// </summary>
        /// <param name="compressedImage">The compressed image.</param>
        /// <returns></returns>
        internal byte[] DecompressImageBytes(byte[] compressedImage)
        {
            byte[] uncompressedImage;
            try
            {
                MemoryStream compressedStream = new MemoryStream(compressedImage);
                Compression.CompressedStreamReader compressedReader = new Compression.CompressedStreamReader(compressedStream);
                MemoryStream uncompressedStream = new MemoryStream();
                byte[] buf = new byte[4096];
                while (true)
                {
                    int readCount = compressedReader.Read(buf, 0, buf.Length);
                    if (readCount <= 0)
                    {
                        break;
                    }
                    uncompressedStream.Write(buf, 0, readCount);
                }
#if WINRT
                compressedStream.Dispose();
                compressedStream = null;
                uncompressedImage = uncompressedStream.ToArray();
                uncompressedStream.Dispose();
                uncompressedStream = null;
#else
                compressedStream.Close();
                compressedStream = null;
                uncompressedImage = uncompressedStream.ToArray();
                uncompressedStream.Close();
                uncompressedStream = null;
#endif
            }
            catch
            {
#if !SILVERLIGHT && !WP
                using (GZipStream stream = new GZipStream(new MemoryStream(compressedImage), CompressionMode.Decompress, true))
                {
                    byte[] buffer = new byte[4096];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, buffer.Length);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        uncompressedImage = memory.ToArray();
                    }
                }
#else
                uncompressedImage = compressedImage;
#endif
            }
            return uncompressedImage;
        }
        #endregion
    }
}
