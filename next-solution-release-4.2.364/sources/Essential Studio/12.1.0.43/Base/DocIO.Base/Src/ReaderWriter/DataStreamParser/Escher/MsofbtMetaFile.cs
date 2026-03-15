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
using Syncfusion.Compression;
using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.DLS.Entities;
using Syncfusion.DocIO.DLS;
#if SyncfusionFramework2_0
using System.IO.Compression;
#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Metafile = Syncfusion.DocIO.DLS.Entities.Metafile;
using System.Drawing;
#else
using Image = System.Drawing.Image;
using Metafile = System.Drawing.Imaging.Metafile;
using System.Drawing.Imaging;
#endif
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Metafile/PICT Blips
    /// Those blips have one of the following signatures: msobiEMF, msobiWMF, or msobiPICT. They are normally 
    /// stored in a compressed format using the LZ compression algorithm in the format used by GNU Zip 
    /// deflate/inflate with a 32k window. The format is zlib format . The only metafile compression version 
    /// number currently defined identifies this format and is analogous to the PNG compression type value in the 
    /// PNG file format. The filter values (MSOBLIPFILTER) define pre-filtering of metafile data to give better 
    /// compression. Currently no pre-filtering is done (it is likely that filtering on a per-record basis will give 
    /// substantially better compression in the future). However, if there is an exception due to out-of-memory or 
    /// out-of-disk space when saving those blips, the compression operation is skipped and the blips are then 
    /// saved in a non-compressed format- in this case the compressed bits are simply the original metafile data.. 
    /// When the blips are loaded back in memory, a check is performed based on a "compression status" flag 
    /// (MSOBLIPCOMPRESSION) that follows the blip header encoded as follows:
    /// </summary>
    internal class MsofbtMetaFile : _Blip
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int m_length;
        private int m_rectLeft;
        private int m_rectTop;
        private int m_rectRight;
        private int m_rectBottom;
        private int m_rectWidth;
        private int m_rectHeight;
        private byte m_fFilter;
        private ImageRecord m_imageRecord;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal CompressionMethod Compression
        {
            get
            {
                return CompressionMethod.msocompressionZip;
            }
        }

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
                m_imageRecord = m_doc.Images.LoadMetaFileImage(value, false);
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
        /// 
        /// </summary>
        internal MsofbtMetaFile(WordDocument doc)
            : base(doc)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <param name="srcMetafile"></param>
        internal MsofbtMetaFile(ImageRecord imageRecord, WordDocument doc)
            : base(doc)
        {
            if (imageRecord != null)
            {
                base.Header.Type = MSOFBT.msofbtBlipEMF;
                base.Header.Instance = 980;
                base.Uid = (Guid.NewGuid());
                Uid2 = Uid;
                m_imageRecord = imageRecord;
                m_length = m_imageRecord.Length;
                m_fFilter = 254;
#if SILVERLIGHT || WP
                m_rectWidth = ( int )imageRecord.Size.Width * 0x319c;
                m_rectHeight = ( int )imageRecord.Size.Height * 0x319c;
#else
                m_rectRight = imageRecord.Size.Width;
                m_rectBottom = imageRecord.Size.Height;
                m_rectWidth = imageRecord.Size.Width * 0x319c * 72 / 96;
                m_rectHeight = imageRecord.Size.Height * 0x319c * 72 / 96;
#endif
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override BaseEscherRecord Clone()
        {
            MsofbtMetaFile metaFile = (MsofbtMetaFile)this.MemberwiseClone();
            if (m_imageRecord != null)
                metaFile.m_imageRecord = new DLS.ImageRecord(m_doc, m_imageRecord);
            metaFile.Header = this.Header.Clone();
            metaFile.Uid = new Guid(Uid.ToByteArray());
            metaFile.Uid2 = new Guid(Uid2.ToByteArray());
            metaFile.m_doc = m_doc;
            return metaFile;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            base.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void ReadRecordData(Stream stream)
        {
            int startPos = (int)stream.Position;
            ReadGuid(stream);
            m_length = ReadInt32(stream);
            m_rectLeft = ReadInt32(stream);
            m_rectTop = ReadInt32(stream);
            m_rectRight = ReadInt32(stream);
            m_rectBottom = ReadInt32(stream);
            m_rectWidth = ReadInt32(stream);
            m_rectHeight = ReadInt32(stream);
            int cbSave = ReadInt32(stream);
            CompressionMethod compressionMethod = (CompressionMethod)stream.ReadByte();
            m_fFilter = (byte)stream.ReadByte();
            byte[] imageBytes = new byte[cbSave];
            stream.Read(imageBytes, 0, cbSave);
            if (compressionMethod == CompressionMethod.msocompressionZip)
                m_imageRecord = m_doc.Images.LoadMetaFileImage(imageBytes, true);
            else
                m_imageRecord = m_doc.Images.LoadMetaFileImage(imageBytes, false);
 
            stream.Position = (startPos + base.Header.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void WriteRecordData(Stream stream)
        {
            byte[] buf = base.Uid.ToByteArray();
            stream.Write(buf, 0, buf.Length);
            if (HasUid2())
            {
                buf = base.Uid2.ToByteArray();
                stream.Write(buf, 0, buf.Length);
            }
            WriteInt32(stream, m_length);
            WriteInt32(stream, m_rectLeft);
            WriteInt32(stream, m_rectTop);
            WriteInt32(stream, m_rectRight);
            WriteInt32(stream, m_rectBottom);
            WriteInt32(stream, m_rectWidth);
            WriteInt32(stream, m_rectHeight);
            WriteInt32(stream, ImageRecord.m_imageBytes.Length);
            stream.WriteByte((byte)CompressionMethod.msocompressionZip);
            stream.WriteByte(m_fFilter);
            stream.Write(ImageRecord.m_imageBytes, 0, ImageRecord.m_imageBytes.Length);
        }
        #endregion
    }
}
