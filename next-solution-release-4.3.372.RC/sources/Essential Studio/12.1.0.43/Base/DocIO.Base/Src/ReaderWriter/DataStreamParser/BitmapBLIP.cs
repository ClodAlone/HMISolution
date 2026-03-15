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
    /// Summary description for BitmapBLIP.
    /// </summary>
    internal class BitmapBLIP : Blip
    {
        #region class members
        /// <summary>
        /// The secondary, or data, UID - should always be set
        /// </summary>
        private byte[] m_rgbUid;
        /* The primary UID - this defaults to 0, in which case the primary ID is
           that of the internal data. NOTE!: The primary UID is only saved to disk
           if (blip_instance ^ blip_signature == 1). Blip_instance is MSOFBH.finst and 
           blip_signature is one of the values defined in MSOBI*/
        /// <summary>
        /// optional based on the above check
        /// </summary>
        private byte[] m_rgbUidPrimary;
        /// <summary>
        /// 
        /// </summary>
        private byte m_tag;
        /// <summary>
        /// raster bits of the blip
        /// </summary>
        private MemoryStream m_pvImageBytes;
        #endregion

        #region Class Initialize/Finalize Methods
        /// <summary>
        /// 
        /// </summary>
        public BitmapBLIP()
        {
            m_rgbUid = new byte[16];
            m_rgbUidPrimary = new byte[16];
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// 
        /// </summary>
        public byte[] RgbUid
        {
            get
            {
                return m_rgbUid;
            }
            set
            {
                m_rgbUid = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] RgbUidPrimary
        {
            get
            {
                return m_rgbUidPrimary;
            }
            set
            {
                m_rgbUidPrimary = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte Tag
        {
            get
            {
                return m_tag;
            }
            set
            {
                m_tag = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public MemoryStream ImageBytes
        {
            get
            {
                return m_pvImageBytes;
            }
            set
            {
                m_pvImageBytes = value;
            }
        }
        #endregion

        #region class public methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        /// <param name="hasPrimaryUid"></param>
        /// <returns></returns>
        public override Image Read(Stream stream, int length, bool hasPrimaryUid)
        {
          int count;
          //      int i;

          //      for( i = 0; i < 16; i++ )
          //      {
          //        RgbUid[ i ] = ( byte )stream.ReadByte();
          //      }
          stream.Read( RgbUid, 0, RgbUid.Length );
          count = 16;

          if( hasPrimaryUid )
          {
            stream.Read( RgbUidPrimary, 0, RgbUidPrimary.Length );
            count += 16;
          }

          Tag = ( byte )stream.ReadByte();
          ImageBytes = null;

          count++;

          byte[] buf = new byte[ length - count ];
          stream.Read( buf, 0, buf.Length );
          ImageBytes = new MemoryStream( buf, 0, buf.Length );
#if SILVERLIGHT || WP
          return new Image ( ImageBytes )
#else
          return new System.Drawing.Bitmap(ImageBytes);
#endif
           
          ;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="image"></param>
        /// <param name="imageFormat"></param>
        /// <param name="id"></param>
        internal override void Write(Stream stream, MemoryStream image, MSOBlipType imageFormat, byte[] id)
        {
            WriteDefaults(stream, image.Length, imageFormat, id);

            byte[] pic = new byte[image.Length];

            image.Position = 0;
            image.Read(pic, 0, pic.Length);

            stream.Write(pic, 0, pic.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="size"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        private void WriteDefaults(Stream stream, long size, MSOBlipType type, byte[] id)
        {
            MSOFBH msofbh = new MSOFBH();
            msofbh.Msofbt = MSOFBT.msofbtBSE;
            msofbh.Inst = 6;
            msofbh.Version = 2;
            //??????
            msofbh.Length = (uint)(size + 61);
            msofbh.Write(stream);


            FBSE fbse = new FBSE();
            fbse.Win32 = type;
            fbse.MacOS = type;
            //      for( int i = 0; i < 16; i++ )
            //      {
            //        fbse.Uid[ i ] = id[ i ];
            //      }
            id.CopyTo(fbse.Uid, 0);
            fbse.Usage = MSOBlipUsage.msoblipUsageDefault;
            fbse.Name = 0;
            fbse.Size = (uint)(size + 25);
            fbse.Delay = 68;
            fbse.Ref = 1;
            fbse.Tag = 255;
            fbse.Unused2 = 0;
            fbse.Unused3 = 0;

            fbse.Write(stream);

            msofbh = new MSOFBH();
            msofbh.Length = (uint)size + 17;

            msofbh.Msofbt = (MSOFBT)((uint)MSOFBT.msofbtBlipFirst + (uint)type);
            msofbh.Inst = 1760;
            msofbh.Version = 0;

            msofbh.Write(stream);

            //      for( int i = 0; i < 16; i++ )
            //      {
            //        stream.WriteByte( id[ i ] );
            //      }
            stream.Write(id, 0, id.Length);

            stream.WriteByte(255);

        }
        /// <summary>
        /// 
        /// </summary>
        internal override void Close()
        {
            base.Close();

            m_rgbUid = null;
            m_rgbUidPrimary = null;
#if WINRT
            m_pvImageBytes.Dispose();
#else
            m_pvImageBytes.Close();
#endif
            m_pvImageBytes = null;
        }
        #endregion
    }
}
