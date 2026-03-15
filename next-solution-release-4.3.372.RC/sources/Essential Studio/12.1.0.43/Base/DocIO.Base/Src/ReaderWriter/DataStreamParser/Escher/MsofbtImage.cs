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
using System.IO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter.Escher;
#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Metafile = Syncfusion.DocIO.DLS.Entities.Metafile;
#else
using Image = System.Drawing.Image;
using Metafile = System.Drawing.Imaging.Metafile;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Bitmap Blips
    /// Those blips have one of the following signatures: msobiJPEG, msobiPNG, or msobiDIB. 
    /// They have the same UID header as described in the Metafile Blip case. The data after the header 
    /// is just a single BYTE "tag" value and is followed by the compressed data of the bitmap in the 
    /// relevant format (JFIF or PNG, bytes as would be stored in a file). For the msobiDIB format, 
    /// the data is in the standard DIB format as a BITMAPINFO ER, BITMAPCORE ER or BITMAPV4 ER followed 
    /// by the color map (DIB_RGB_COLORS) and the bits. This data is not compressed (the format is used 
    /// for very small DIB bitmaps only).
    /// </summary>
    internal class MsofbtImage : _Blip
    {
        #region Constants
        /// <summary>
        /// Number of used colors.
        /// </summary>
        private const int DEF_COLOR_USED_OFFSET = 32;
        /// <summary>
        /// Size of header.
        /// </summary>
        private const int DEF_DIB_HEADER_SIZE = 14;
        /// <summary>
        /// Signature, must be 4D42 hex for .bmp header.
        /// </summary>
        private static readonly byte[] DEF_SIGNATURE = new byte[] { 0x42, 0x4D };
        /// <summary>
        /// Reserved data.
        /// </summary>
        private static readonly byte[] DEF_RESERVED = new byte[] { 0, 0, 0, 0 };
        /// <summary>
        /// Size of each color definition in the palette.
        /// </summary>
        private const uint DEF_COLOR_SIZE = 4;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private ImageRecord m_imageRecord;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal override byte[] ImageBytes
        {
            get
            {
                if (m_imageRecord == null)
                    return null;
                return m_imageRecord.ImageBytes;
            }
            set
            {
                m_imageRecord = m_doc.Images.LoadImage(value);
                if (m_imageRecord != null)
                    m_imageRecord.OccurenceCount--;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal override ImageRecord ImageRecord
        {
            get
            {
                return m_imageRecord;
            }
            set
            {
                m_imageRecord = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MsofbtImage"/> class.
        /// </summary>
        /// <param name="doc"></param>
        internal MsofbtImage(WordDocument doc)
            : base(doc)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MsofbtImage"/> class.
        /// </summary>
        /// <param name="imageRecord">The image record.</param>
        /// <param name="isBitmap">if set to <c>true</c> [is bitmap].</param>
        /// <param name="doc">The doc.</param>
        internal MsofbtImage(ImageRecord imageRecord, bool isBitmap, WordDocument doc)
            : base(doc)
        {
            if (isBitmap)
            {
                Header.Type = MSOFBT.msofbtBlipPNG;
                Header.Instance = 1760;
            }
            else
            {
                Header.Type = MSOFBT.msofbtBlipJPEG;
                Header.Instance = (0x46a);
            }

            Uid = (Guid.NewGuid());
            Uid2 = Uid;
            m_imageRecord = imageRecord;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void ReadRecordData(Stream stream)
        {
            ReadGuid(stream);
            stream.ReadByte();
            int imageLength = (Header.Length - 16) - 1;
            byte[] imageArray = new byte[imageLength];
            stream.Read(imageArray, 0, imageLength);
             
            if (IsDib)
                imageArray = ConvertDibToBmp(imageArray);
            m_imageRecord = m_doc.Images.LoadImage(imageArray);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void WriteRecordData(Stream stream)
        {
            byte[] guid = Uid.ToByteArray();
            stream.Write(guid, 0, guid.Length);
            if (HasUid2())
            {
                guid = base.Uid2.ToByteArray();
                stream.Write(guid, 0, guid.Length);
            }
            stream.WriteByte((byte)0xff);
            stream.Write(ImageBytes, 0, ImageBytes.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override BaseEscherRecord Clone()
        {
            MsofbtImage image = new MsofbtImage(m_doc);
            if (m_imageRecord != null)
                image.m_imageRecord = new DLS.ImageRecord(m_doc, m_imageRecord);
            image.Header = this.Header.Clone();
            image.Uid = new Guid(Uid.ToByteArray());
            image.Uid2 = new Guid(Uid2.ToByteArray());
            image.m_doc = m_doc;
            return image;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            base.Close();
        }

        #endregion

        #region Class helper methods
        /// <summary>
        /// Dib file is a .BMP file without .BMP header. This function adds BMP header.
        /// </summary>
        private byte[] ConvertDibToBmp(byte[] imageBytes)
        {
            uint uiSize = BitConverter.ToUInt32(imageBytes, 0);
            uint dibColorsCount = BitConverter.ToUInt32(imageBytes, DEF_COLOR_USED_OFFSET);
            int iFullSize = imageBytes.Length + DEF_DIB_HEADER_SIZE;

            MemoryStream bmpImage = new MemoryStream();
            byte[] byteArr = BitConverter.GetBytes(iFullSize);

            bmpImage.Write(DEF_SIGNATURE, 0, DEF_SIGNATURE.Length);
            bmpImage.Write(byteArr, 0, byteArr.Length);
            bmpImage.Write(DEF_RESERVED, 0, DEF_RESERVED.Length);

            uint uiDataOffset = uiSize + DEF_DIB_HEADER_SIZE + dibColorsCount * DEF_COLOR_SIZE;
            byteArr = BitConverter.GetBytes(uiDataOffset);
            bmpImage.Write(byteArr, 0, byteArr.Length);
            bmpImage.Write(imageBytes, 0, imageBytes.Length);

            imageBytes = bmpImage.ToArray();
#if WINRT
            bmpImage.Dispose();
#else
            bmpImage.Close();
#endif
            return imageBytes;
        }
        #endregion
    }
}
