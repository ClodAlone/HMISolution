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
using Syncfusion.DocIO.ReaderWriter.Biff_Records;

#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
using Metafile = Syncfusion.DocIO.DLS.Entities.Metafile;
#else
using Image = System.Drawing.Image;
using Metafile = System.Drawing.Imaging.Metafile;
#endif

#endregion

namespace Syncfusion.DocIO.ReaderWriter.Escher
{
    /// <summary>
    /// Summary description for BstoreContainer.
    /// </summary>
    internal class BstoreContainer : BaseWordRecord
    {
        #region Class Members

        /// <summary>
        /// 
        /// </summary>
        private FBSE m_fbse = null;

        /// <summary>
        /// 
        /// </summary>
        private BitmapBLIP m_bitmapBlip = null;
        /// <summary>
        /// 
        /// </summary>
        private Image m_bitmap;

        /// <summary>
        /// 
        /// </summary>
        private Blip m_blip;

        #endregion

        #region Class Inaitialize/Finalize Methods

        /// <summary>
        /// 
        /// </summary>
        public BstoreContainer()
        {
            m_fbse = new FBSE();
            m_bitmapBlip = new BitmapBLIP();
        }

        #endregion

        #region Class Properties

        /// <summary>
        /// 
        /// </summary>
        internal FBSE Fbse
        {
            get { return m_fbse; }
            set { m_fbse = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Image Bitmap
        {
            get
            {
                return m_bitmap;
            }
        }

        #endregion

        #region Class Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public void Read(Stream stream)
        {
            m_fbse.Read(stream);

            MSOFBH amsofbh = new MSOFBH();
            amsofbh.Read(stream);

            bool hasPrimaryUid = false;

            switch ((MSOBlipType)(amsofbh.Msofbt - MSOFBT.msofbtBlipFirst))
            {
                case MSOBlipType.msoblipPNG:
                    if ((amsofbh.Inst ^ (uint)MSOBI.msobiPNG) == 1)
                    {
                        hasPrimaryUid = true;
                    }
                    m_blip = new BitmapBLIP();
                    break;

                case MSOBlipType.msoblipJPEG:
                    if ((amsofbh.Inst ^ (uint)MSOBI.msobiJFIF) == 1)
                    {
                        hasPrimaryUid = true;
                    }
                    m_blip = new BitmapBLIP();
                    break;

                case MSOBlipType.msoblipDIB:
                    if ((amsofbh.Inst ^ (uint)MSOBI.msobiDIB) == 1)
                    {
                        hasPrimaryUid = true;
                    }
                    m_blip = new BitmapBLIP();
                    break;

                case MSOBlipType.msoblipWMF:
                    if ((amsofbh.Inst ^ (uint)MSOBI.msobiWMF) == 1)
                    {
                        hasPrimaryUid = true;
                    }
                    m_blip = new MetafileBlip();
                    break;

                case MSOBlipType.msoblipEMF:
                    if ((amsofbh.Inst ^ (uint)MSOBI.msobiEMF) == 1)
                    {
                        hasPrimaryUid = true;
                    }
                    m_blip = new MetafileBlip();
                    break;

                case MSOBlipType.msoblipPICT:
                    if ((amsofbh.Inst ^ (uint)MSOBI.msobiPICT) == 1)
                    {
                        hasPrimaryUid = true;
                    }
                    m_blip = new MetafileBlip();
                    break;
            }

            m_bitmap = m_blip.Read(stream, (int)amsofbh.Length, hasPrimaryUid);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="imageStream"></param>
        /// <param name="id"></param>
        /// <param name="image"></param>
        internal void Write(Stream stream, MemoryStream imageStream, byte[] id, Image image)
        {
#if SILVERLIGHT || WP
            if (image.IsMetafile)
#else
            if (image is Metafile)
#endif
            {
                m_blip = new MetafileBlip();
                (m_blip as MetafileBlip).Metafile = image as Metafile;
                m_blip.Write(stream, imageStream, MSOBlipType.msoblipEMF, id);

            }
            else
            {
                m_blip = new BitmapBLIP();
                m_blip.Write(stream, imageStream, MSOBlipType.msoblipPNG, id);
            }
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            if (m_bitmap != null)
            {
                m_bitmap.Dispose();
                m_bitmap = null;
            }

            if (m_blip != null)
            {
                m_blip.Close();
            }
        }
        #endregion
    }
}
