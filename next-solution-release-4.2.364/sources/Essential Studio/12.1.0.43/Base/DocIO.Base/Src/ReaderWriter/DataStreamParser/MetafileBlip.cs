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
using Syncfusion.DocIO.DLS;
#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Metafile = Syncfusion.DocIO.DLS.Entities.Metafile;
#else
using Image = System.Drawing.Image;
using Metafile = System.Drawing.Imaging.Metafile;
#endif
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Escher
{
    /// <summary>
    /// Summary description for MetafileBlip.
    /// </summary>
    internal class MetafileBlip : Blip
    {
        #region Class Members
        /// <summary>
        /// 
        /// </summary>
        private byte[] m_rgbUid;
        /// <summary>
        /// 
        /// </summary>
        private byte[] m_rgbUidPrimary;
        /// <summary>
        /// Cache of saved size (size of m_pvBits)
        /// </summary>
        private uint m_cbSave;
        /// <summary>
        /// MSOBLIPCOMPRESSION
        /// </summary>
        private byte m_fCompression;
        /// <summary>
        /// Compressed bits of metafile
        /// </summary>
        private byte[] m_pvBits;
        /// <summary>
        /// Size of metafile in EMUs
        /// </summary>
        private int m_length;
        /// <summary>
        /// Boundary of metafile drawing commands
        /// </summary>
        private int m_rectLeft;
        private int m_rectTop;
        private int m_rectRight;
        private int m_rectBottom;
        private int m_rectWidth;
        private int m_rectHeight;
        private CompressionMethod m_compressionMethod;
        /// <summary>
        /// always msofilterNone
        /// </summary>
        private byte m_fFilter;
        private byte[] m_compressedImage;
        private byte[] m_uncompressedImage;
        private Metafile m_srcMetafile = null;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal Metafile Metafile
        {
            get
            {
                return m_srcMetafile;
            }
            set
            {
                m_srcMetafile = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize Methods
        /// <summary>
        /// 
        /// </summary>
        public MetafileBlip()
        {
            m_rgbUid = new byte[16];
            m_rgbUidPrimary = new byte[16];
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        /// <param name="hasPrimaryUid"></param>
        /// <returns></returns>
        public override Image Read(Stream stream, int length, bool hasPrimaryUid)
        {
            for (int i = 0; i < 16; i++)
            {
                m_rgbUid[i] = (byte)stream.ReadByte();
            }

            m_length = ReadInt32(stream);

            m_rectLeft = ReadInt32(stream);
            m_rectTop = ReadInt32(stream);
            m_rectRight = ReadInt32(stream);
            m_rectBottom = ReadInt32(stream);

            m_rectWidth = ReadInt32(stream);
            m_rectHeight = ReadInt32(stream);

            m_cbSave = ReadUInt32(stream);

            m_fCompression = (byte)stream.ReadByte();

            m_fFilter = (byte)stream.ReadByte();

            m_pvBits = new byte[m_cbSave];
            stream.Read(m_pvBits, 0, m_pvBits.Length);

            MemoryStream compressedStream = new MemoryStream(m_pvBits);
            if ((CompressionMethod)m_fCompression == CompressionMethod.msocompressionZip)
            {

                CompressedStreamReader compressedReader = new CompressedStreamReader(compressedStream);
                MemoryStream uncompressedStream = new MemoryStream();
                byte[] buffer1 = new byte[0x1000];
                while (true)
                {
                    int readCount = compressedReader.Read(buffer1, 0, buffer1.Length);
                    if (readCount <= 0)
                    {
                        break;
                    }
                    uncompressedStream.Write(buffer1, 0, readCount);
                }
                uncompressedStream.Position = 0;

                return new Metafile(uncompressedStream);
            }
            else
            {
                return new Metafile(compressedStream);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="image"></param>
        /// <param name="imageFormat"></param>
        /// <param name="Uid"></param>
        internal override void Write(Stream stream, MemoryStream image, MSOBlipType imageFormat,
          byte[] Uid)
        {
            m_uncompressedImage = image.ToArray();
            m_length = m_uncompressedImage.Length;
            m_compressedImage = m_uncompressedImage;
#if !SILVERLIGHT && !WP
            Rectangle rect = m_srcMetafile.GetMetafileHeader().Bounds;
            m_rectLeft = rect.Left;
            m_rectTop = rect.Top;
            m_rectRight = rect.Right;
            m_rectBottom = rect.Bottom;
            m_rectWidth = rect.Width * 0x319c;
            m_rectHeight = rect.Height * 0x319c;
            m_compressionMethod = CompressionMethod.msocompressionNone;
            m_fFilter = 0xfe;
#endif

            WriteDefaults(stream, m_length, Uid);

            stream.Write(Uid, 0, Uid.Length);
            WriteInt32(stream, m_length);
            WriteInt32(stream, m_rectLeft);
            WriteInt32(stream, m_rectTop);
            WriteInt32(stream, m_rectRight);
            WriteInt32(stream, m_rectBottom);
            WriteInt32(stream, m_rectWidth);
            WriteInt32(stream, m_rectHeight);
            WriteInt32(stream, m_compressedImage.Length);
            stream.WriteByte((byte)m_compressionMethod);
            stream.WriteByte(m_fFilter);
            stream.Write(m_compressedImage, 0, m_compressedImage.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="size"></param>
        /// <param name="Uid"></param>
        private void WriteDefaults(Stream stream, long size, byte[] Uid)
        {
            MSOFBH msofbh = new MSOFBH();
            msofbh.Msofbt = MSOFBT.msofbtBSE;
            msofbh.Inst = (uint)MSOBlipType.msoblipEMF;
            msofbh.Version = 2;
            //??????
            msofbh.Length = (uint)(size + 94);
            msofbh.Write(stream);

            FBSE fbse = new FBSE();
            fbse.Win32 = MSOBlipType.msoblipEMF;
            fbse.MacOS = MSOBlipType.msoblipPICT;
            for (int i = 0; i < 16; i++)
            {
                fbse.Uid[i] = Uid[i];
            }
            fbse.Usage = MSOBlipUsage.msoblipUsageDefault;
            fbse.Name = 0;
            fbse.Size = (uint)(size + 58);
            fbse.Delay = 68;
            fbse.Ref = 1;
            fbse.Tag = 255;
            fbse.Unused2 = 0;
            fbse.Unused3 = 0;

            fbse.Write(stream);

            msofbh = new MSOFBH();
            msofbh.Length = (uint)size + 50;
            msofbh.Msofbt = MSOFBT.msofbtBlipEMF;
            msofbh.Inst = (uint)MSOBI.msobiEMF;
            msofbh.Version = 0;

            msofbh.Write(stream);
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            base.Close();

            m_rgbUid = null;
            m_rgbUidPrimary = null;
            m_compressedImage = null;
            m_uncompressedImage = null;

            if (m_srcMetafile != null)
            {
                m_srcMetafile.Dispose();
                m_srcMetafile = null;
            }
        }
        #endregion
    }

}
