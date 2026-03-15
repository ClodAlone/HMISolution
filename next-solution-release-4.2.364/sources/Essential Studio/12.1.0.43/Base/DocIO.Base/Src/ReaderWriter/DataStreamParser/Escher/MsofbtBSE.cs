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
#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Metafile = Syncfusion.DocIO.DLS.Entities.Metafile;
using Syncfusion.DocIO.DLS.Entities;
#else
using Image = System.Drawing.Image;
using Metafile = System.Drawing.Imaging.Metafile;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
#endif

using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.DLS;

#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// BLIP Store Entry Record msofbtBSE
    /// Each BLIP in the BStore is serialized to a File BLIP Store Entry (FBSE) record. The instance field 
    /// encodes the type of the blip. A fixed size header contains the rest of the common information about 
    /// the BLIP. If the cbName field in the FBSE is nonzero, a null-terminated Unicode string is written 
    /// immediately after the FBSE in the file.
    /// </summary>
    internal class MsofbtBSE : BaseEscherRecord
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private _FBSE m_fbse;
        private _Blip m_blip;
        private bool m_isInlineBlip;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal MsofbtBSE(WordDocument doc)
            : base(MSOFBT.msofbtBSE, 2, doc)
        {
            m_fbse = new _FBSE();
        }
        #endregion

        #region Class methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void ReadRecordData(Stream stream)
        {
            m_fbse.Read(stream);
            if (Header.Length > _FBSE.DEF_FBSE_LENGTH)
            {
                m_isInlineBlip = true;
                m_blip = _MSOFBH.ReadHeaderWithRecord(stream, m_doc) as _Blip;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        protected override void WriteRecordData(Stream stream)
        {
            int startPos = Convert.ToInt32(stream.Position);
            m_fbse.Write(stream);
            if (m_isInlineBlip && m_blip != null)
            {
                m_fbse.m_size = m_blip.WriteMsofbhWithRecord(stream);
                int endPos = Convert.ToInt32(stream.Position);
                stream.Position = startPos;
                m_fbse.Write(stream);
                stream.Position = endPos;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Read(Stream stream)
        {
            long startPos = stream.Position;
            stream.Position = m_fbse.m_foDelay;
            m_blip = _MSOFBH.ReadHeaderWithRecord(stream, m_doc) as _Blip;
            stream.Position = startPos;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        internal void Write(Stream stream)
        {
            m_fbse.m_foDelay = (int)stream.Position;
            m_fbse.m_size = 0;
            if (m_blip != null)
            {
                m_fbse.m_size = m_blip.WriteMsofbhWithRecord(stream);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageBytes"></param>
        internal void Initialize(ImageRecord imageRecord)
        {
            _Blip blip;
            if (imageRecord.IsMetafile)
            {
                blip = new MsofbtMetaFile(imageRecord, m_doc);
            }
            else
            {
                bool isBitmap = IsBitmap(imageRecord.ImageFormat);
                blip = new MsofbtImage(imageRecord, isBitmap, m_doc);
            }

            this.Header.Instance = ((int)blip.Type);
            this.Fbse.m_btWin32 = (int)blip.Type;
            this.Fbse.m_btMacOS = (int)blip.Type;
            this.Fbse.m_rgbUid = blip.Uid.ToByteArray();
            this.Fbse.m_tag = 255;
            this.Fbse.m_cRef = 1;
            this.Blip = blip;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal _Blip Blip
        {
            get
            {
                return m_blip;
            }
            set
            {
                m_blip = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal _FBSE Fbse
        {
            get
            {
                return m_fbse;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsInlineBlip
        {
            get
            {
                return m_isInlineBlip;
            }
            set
            {
                m_isInlineBlip = value;
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
            MsofbtBSE fbtBSE = new MsofbtBSE(m_doc);
            fbtBSE.m_isInlineBlip = m_isInlineBlip;
            fbtBSE.m_fbse = m_fbse.Clone();
            if (m_blip != null)
            {
                fbtBSE.m_blip = (_Blip)m_blip.Clone();
            }
            fbtBSE.Header = Header.Clone();
            fbtBSE.m_doc = m_doc;
            return fbtBSE;
        }

        #endregion

        #region Class helper methods
        /// <summary>
        /// Determines whether the specified image format is metafile.
        /// </summary>
        /// <param name="imageFormat">The image format.</param>
        /// <returns>
        /// 	<c>true</c> if the specified image format is metafile; otherwise, <c>false</c>.
        /// </returns>
        private bool IsMetafile(ImageFormat imageFormat)
        {
            return (imageFormat.Equals(ImageFormat.Emf) || imageFormat.Equals(ImageFormat.Wmf));
        }
        /// <summary>
        /// Determines whether the specified image format is bitmap.
        /// </summary>
        /// <param name="imageFormat">The image format.</param>
        /// <returns>
        /// 	<c>true</c> if the specified image format is bitmap; otherwise, <c>false</c>.
        /// </returns>
        private bool IsBitmap(ImageFormat imageFormat)
        {
            return (imageFormat.Equals(ImageFormat.Png) || imageFormat.Equals(ImageFormat.Bmp));
        }
        #endregion
    }
}
