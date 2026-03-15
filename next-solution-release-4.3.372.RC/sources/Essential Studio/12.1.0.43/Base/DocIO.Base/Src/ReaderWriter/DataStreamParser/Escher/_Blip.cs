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
#else
using Image = System.Drawing.Image;
#endif
using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.DLS.Entities;
using Syncfusion.DocIO.DLS;
#if WINRT
using Syncfusion.DocIO.Security.Cryptography;
#else
using System.Security.Cryptography;
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher
{
    /// <summary>
    /// Summary description for _Blip.
    /// </summary>
    internal abstract class _Blip : BaseEscherRecord
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        protected const int DEF_UID_LENGTH = 16;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private Guid m_guid;
        private Guid m_guid2;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal MSOBlipType Type
        {
            get
            {
                return (MSOBlipType)((int)Header.Type - (int)MSOFBT.msofbtBlipFirst);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal abstract byte[] ImageBytes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        internal abstract ImageRecord ImageRecord { get; set; }
        /// <summary>
        /// 
        /// </summary>
        internal ImageFormat ImageFormat
        {
            get
            {
                switch (Type)
                {
                    case MSOBlipType.msoblipEMF:
                        {
                            return ImageFormat.Emf;
                        }
                    case MSOBlipType.msoblipWMF:
                        {
                            return ImageFormat.Wmf;
                        }
                    case MSOBlipType.msoblipJPEG:
                        {
                            return ImageFormat.Jpeg;
                        }
                    case MSOBlipType.msoblipPNG:
                        {
                            return ImageFormat.Png;
                        }
                    case MSOBlipType.msoblipDIB:
                        {
                            return ImageFormat.Bmp;
                        }
                }
                throw new Exception(Type.ToString() + "is not supported");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal Guid Uid
        {
            get
            {
                return m_guid;
            }
            set
            {
                m_guid = value;
            }
        }
        /// <summary>
        /// Gets or sets the uid2.
        /// </summary>
        /// <value>The uid2.</value>
        internal Guid Uid2
        {
            get
            {
                return m_guid2;
            }
            set
            {
                m_guid2 = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is has .dib format.
        /// </summary>
        /// <value><c>true</c> if this instance has .dib format; otherwise, <c>false</c>.</value>
        internal bool IsDib
        {
            get
            {
                return (Type == MSOBlipType.msoblipDIB) ? true : false;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        protected _Blip(WordDocument doc) : base(doc)
        { }
        #endregion

        #region Class static methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected void ReadGuid(Stream stream)
        {
            byte[] buff = new byte[DEF_UID_LENGTH];
            stream.Read(buff, 0, buff.Length);
            m_guid = new Guid(buff);

            if (HasUid2())
            {
                buff = new byte[DEF_UID_LENGTH];
                stream.Read(buff, 0, buff.Length);
                m_guid2 = new Guid(buff);
            }
        }
        /// <summary>
        /// Determines whether this instance has uid2.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance has uid2; otherwise, <c>false</c>.
        /// </returns>
        internal bool HasUid2()
        {
            //Specifically handled for Uid2
            //OfficeArtBlipEMF - 0xF01A has Uid2 only if Header.Instance is 0x3D5.
            //OfficeArtBlipWMF - 0xF01B has Uid2 only if Header.Instance is 0x217.
            //OfficeArtBlipPICT - 0xF01C has Uid2 only if Header.Instance is 0x543.
            //OfficeArtBlipJPEG - 0xF01D or 0xF02A has Uid2 only if Header.Instance is either 0x46B or 0x6E3.
            //OfficeArtBlipPNG - 0xF01E has Uid2 only if Header.Instance is 0x6E1.
            //OfficeArtBlipDIB - 0xF01F has Uid2 only if Header.Instance is 0x7A9.
            //OfficeArtBlipTIFF - 0xF029 has Uid2 only if Header.Instance is 0x6E5.
            return (((int)Header.Type == 0xF01A && Header.Instance == 0x3D5)
                || ((int)Header.Type == 0xF01B && Header.Instance == 0x217)
                || ((int)Header.Type == 0xF01C && Header.Instance == 0x543)
                || (((int)Header.Type == 0xF01D || (int)Header.Type == 0xF02A)
                && (Header.Instance == 0x46B || Header.Instance == 0x6E3))
                || ((int)Header.Type == 0xF01E && Header.Instance == 0x6E1)
                || ((int)Header.Type == 0xF01F && Header.Instance == 0x7A9)
                || ((int)Header.Type == 0xF029 && Header.Instance == 0x6E5));
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal abstract override BaseEscherRecord Clone();
        #endregion

    }
}
