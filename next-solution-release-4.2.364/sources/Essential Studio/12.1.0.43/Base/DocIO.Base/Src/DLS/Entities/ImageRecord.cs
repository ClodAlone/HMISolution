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
using System.IO;
#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Metafile = Syncfusion.DocIO.DLS.Entities.Metafile;
using Syncfusion.DocIO.DLS.Entities;
#else
using Image = System.Drawing.Image;
using Metafile = System.Drawing.Imaging.Metafile;
using System.Drawing.Imaging;
#endif
#if WINRT
using Syncfusion.DocIO.Security.Cryptography;
#else 
#if !WP
using System.Drawing;
#endif
using System.Security.Cryptography;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    internal class ImageRecord
    {
        #region Constants
        /// <summary>
        /// Specifies the key used by hashing algorithm to compute hash. Encoding.Unicode.GetBytes("ImgHashKey").
        /// </summary>
        internal static readonly byte[] HashKey = { 73, 0, 109, 0, 103, 0, 72, 0, 97, 0, 115, 0, 104, 0, 75, 0, 101, 0, 121, 0};
        #endregion

        #region Fields
        private int m_imageId;
        internal byte[] m_imageBytes;
        private int m_occurenceCount;
        private bool m_isMetafile;
        private WordDocument m_doc;
        private Size m_size = new Size(int.MaxValue, int.MaxValue);
        private ImageFormat m_imageFormat;
        private int m_length = int.MinValue;
        #endregion

        #region Properties
        /// <summary>
        /// Gets image byte array.
        /// </summary>
        internal int ImageId
        {
            get
            {
                return m_imageId;
            }
            set
            {
                m_imageId = value;
            }
        }
        /// <summary>
        /// Gets image byte array.
        /// </summary>
        internal byte[] ImageBytes
        {
            get
            {
                if (m_isMetafile && m_doc != null)
                    return m_doc.Images.DecompressImageBytes(m_imageBytes);
                return m_imageBytes;
            }
        }
        /// <summary>
        /// Gets hash of the image.
        /// </summary>
        internal byte[] ImageHash
        {
            get
            {
                if (m_imageBytes != null)
                {
                    //Compute Hash
                    HMACSHA1 hmacSha1 = new HMACSHA1();
                    hmacSha1.Key = HashKey;
                    return hmacSha1.ComputeHash(m_imageBytes);
                }
                return null;
            }
        }
        /// <summary>
        /// Gets or sets the occurence count.
        /// </summary>
        /// <value>The occurence count.</value>
        internal int OccurenceCount
        {
            get
            {
                return m_occurenceCount;
            }
            set
            {
                m_occurenceCount = value;
                if (m_occurenceCount == 0)
                {
                    if (m_doc != null)
                        m_doc.Images.Remove(m_imageId);
                    Close();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is metafile.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is metafile; otherwise, <c>false</c>.
        /// </value>
        internal bool IsMetafile
        {
            get
            {
                return m_isMetafile;
            }
            set
            {
                m_isMetafile = value;
            }
        }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        internal Size Size
        {
            get
            {
                if (m_size.Width == float.MinValue
                    || m_size.Height == float.MinValue)
                    UpdateImageSize(GetImageInternal(ImageBytes));
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }
        /// <summary>
        /// Gets or sets the image format.
        /// </summary>
        /// <value>The image format.</value>
        internal ImageFormat ImageFormat
        {
            get
            {
                if (m_imageFormat == null)
                    UpdateImageSize(GetImageInternal(ImageBytes));
                return m_imageFormat;
            }
            set
            {
                m_imageFormat = value;
            }
        }
        /// <summary>
        /// Gets or sets the image format.
        /// </summary>
        /// <value>The image format.</value>
        internal int Length
        {
            get
            {
                if (m_length == int.MinValue)
                    m_length = ImageBytes.Length;
                return m_length;
            }
            set
            {
                m_length = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRecord"/> class.
        /// </summary>
        /// <param name="doc">The word document.</param>
        /// <param name="imageBytes">The image bytes.</param>
        internal ImageRecord(WordDocument doc, byte[] imageBytes)
        {
            m_doc = doc;
            m_imageBytes = imageBytes;
        }
        internal ImageRecord(WordDocument doc, ImageRecord imageRecord)
        {
            m_doc = doc;
            m_imageBytes = imageRecord.m_imageBytes;
            m_isMetafile = imageRecord.m_isMetafile;
            m_length = imageRecord.m_length;
            m_imageFormat = imageRecord.m_imageFormat;
            m_size = imageRecord.m_size;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Detaches from document image collection
        /// </summary>
        internal void Detach()
        {
            m_occurenceCount--;
            if (m_occurenceCount == 0)
            {
                if (m_doc != null)
                    m_doc.Images.Remove(m_imageId);
                m_imageId = 0;
                m_occurenceCount = 0;
            }
        }
        internal void Attach()
        {
            m_occurenceCount++;
            m_doc.Images.Add(this);
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            m_imageBytes = null;
            m_imageId = 0;
            m_occurenceCount = 0;
        }
        /// <summary>
        /// Updates the size of the image.
        /// </summary>
        /// <param name="image">The image.</param>
        internal void UpdateImageSize(Image image)
        {
            if (image != null)
            {
                m_size = image.Size;
                m_imageFormat = image.RawFormat;
                image.Dispose();
            }
        }
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="imageBytes">The image bytes.</param>
        /// <returns></returns>
        private Image GetImageInternal(byte[] imageBytes)
        {
            if (imageBytes != null)
            {
                m_length = imageBytes.Length;
                return GetImage(imageBytes);
            }
            return null;
        }
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="imageBytes">The image bytes.</param>
        /// <returns></returns>
        internal static Image GetImage(byte[] imageBytes)
        {
            Image image = null;
            if (imageBytes != null)
            {
                try
                {
#if SILVERLIGHT || WP
                    image = Image.FromStream(new MemoryStream(imageBytes));
#else
                    image = Image.FromStream(new MemoryStream(imageBytes), true, false);
#endif
                    imageBytes = null;
                }
                catch
                {
                    throw new ArgumentException("Argument is not image byte array");
                }
            }
            return image;
        }
        #endregion
    }
}
