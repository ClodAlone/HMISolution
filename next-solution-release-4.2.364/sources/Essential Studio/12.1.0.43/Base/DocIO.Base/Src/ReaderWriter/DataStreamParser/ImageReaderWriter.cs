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
using System.Text;

#if SILVERLIGHT || WP
using Syncfusion.DocIO.DLS.Entities;
#else
using System.Drawing;
using System.Drawing.Imaging;
#endif

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;

#endregion

namespace Syncfusion.DocIO.ReaderWriter.Escher
{
    /// <summary>
    /// Summary description for ImageWriter
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class WordImageWriter
    {
        #region Class Members
        /// <summary>
        /// 
        /// </summary>
        private MemoryStream m_dataStream;
        /// <summary>
        /// 
        /// </summary>
        private PICF m_picData = new PICF();
        /// <summary>
        /// 
        /// </summary>
        private Metafile m_srcMetafile = null;
        #endregion

        #region Class Initialize/Finalize Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataStream"></param>
        /// <param name="memConverter"></param>
        internal WordImageWriter(MemoryStream dataStream)
        {
            m_dataStream = dataStream;
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// Get imagewriter's data stream
        /// </summary>
        internal MemoryStream DataStream
        {
            get
            {
                return m_dataStream;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal PICF PictureData
        {
            get
            {
                return m_picData;
            }
        }
        #endregion

        #region Class internal Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pict"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        internal int WriteImage(WPicture pict, int height, int width)
        {
            if (!pict.PictureShape.PictureDescriptor.BorderLeft.IsDefault)
                m_picData = pict.PictureShape.PictureDescriptor.Clone();
            m_picData.SetBasePictureOptions(height, width, pict.HeightScale, pict.WidthScale);

            m_picData.cProps = 0; //Old: 15;
            m_picData.mm = 100;
            m_picData.bm_rcWinMF = 8;

            MsofbtSpContainer container = new MsofbtSpContainer(pict.Document);
            container.CreateInlineImageContainer(pict);
            long startPos = m_dataStream.Position;

            m_dataStream.Position += 68;
            container.WriteContainer(m_dataStream);
            int endPos = (int)m_dataStream.Position;

            m_picData.lcb = (int)(endPos - startPos);
            m_picData.cbHeader = 68;

            m_dataStream.Position = startPos;
            m_picData.Write(m_dataStream);
            m_dataStream.Position = endPos;

            return endPos;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeObj"></param>
        /// <returns></returns>
        internal int WriteInlineShapeObject(InlineShapeObject shapeObj)
        {
            MsofbtSpContainer spContainer = shapeObj.ShapeContainer as MsofbtSpContainer;
            if (spContainer != null)
            {
                if (shapeObj.PictureDescriptor.cbHeader == 68)
                {
                    long startPos = m_dataStream.Position;

                    m_dataStream.Position += 68;
                    spContainer.WriteContainer(m_dataStream);
                    int endPos = (int)m_dataStream.Position;

                    shapeObj.PictureDescriptor.lcb = (int)(endPos - startPos);
                    shapeObj.PictureDescriptor.cbHeader = 68;

                    m_dataStream.Position = startPos;
                    shapeObj.PictureDescriptor.Write(m_dataStream);
                    m_dataStream.Position = endPos;
                }
                else
                {
                    shapeObj.PictureDescriptor.Write(m_dataStream);
                    spContainer.WriteContainer(m_dataStream);
                }
            }
            else if (shapeObj.PictureDescriptor != null && shapeObj.UnparsedData != null)
            {
                long startPos = m_dataStream.Position;

                m_dataStream.Position += 68;
                BinaryWriter writer = new BinaryWriter(m_dataStream);
                writer.Write(shapeObj.UnparsedData);
                int endPos = (int)m_dataStream.Position;

                shapeObj.PictureDescriptor.lcb = (int)(endPos - startPos);
                shapeObj.PictureDescriptor.cbHeader = 68;

                m_dataStream.Position = startPos;
                shapeObj.PictureDescriptor.Write(m_dataStream);
                m_dataStream.Position = endPos;
            }

            return (int)m_dataStream.Position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="txbxProps"></param>
        internal int WriteInlineTxBxPicture(WTextBoxFormat txbxFormat)
        {
            MsofbtSpContainer spContainer = new MsofbtSpContainer(txbxFormat.Document);
            spContainer.CreateInlineTxbxImageCont();
            PICF pictDesc = new PICF();
            int height = (int)Math.Round(txbxFormat.Height * DLSConstants.TwipsInOnePoint);
            int width = (int)Math.Round(txbxFormat.Width * DLSConstants.TwipsInOnePoint);
            m_picData.SetBasePictureOptions(height, width, 100, 100);

            long startPos = (int)m_dataStream.Position;
            m_dataStream.Position += 68;
            spContainer.WriteContainer(m_dataStream);
            long endPos = (int)m_dataStream.Position;

            m_picData.lcb = (int)(endPos - startPos);
            m_picData.cbHeader = 68;
            m_picData.mm = 100;
            m_picData.bm_rcWinMF = 2;
            m_dataStream.Position = startPos;
            m_picData.Write(m_dataStream);
            m_dataStream.Position = endPos;
            return (int)m_dataStream.Position;
        }
        #endregion

        #region Class Helper Methods
        /// <summary>
        /// 
        /// </summary>
        private void SavePicf()
        {
            m_picData.lcb += (int)(205);
            m_picData.cbHeader = 68;
            m_picData.Write(m_dataStream);
        }
        #endregion

        #region Class Metafile helper methods
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="recordType"></param>
        ///// <param name="flags"></param>
        ///// <param name="dataSize"></param>
        ///// <param name="data"></param>
        ///// <param name="callbackData"></param>
        ///// <returns></returns>
        //private bool PlayInMeta( EmfPlusRecordType recordType, int flags,
        //  int dataSize, IntPtr data, PlayRecordCallback callbackData )
        //{           
        //  byte[] recordData = new byte[ dataSize ];

        //  if( data != IntPtr.Zero )
        //  {
        //    Marshal.Copy( data, recordData, 0, dataSize );

        //  }

        //  m_srcMetafile.PlayRecord( recordType, flags, dataSize, recordData );


        //  return true;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="image"></param>
        ///// <returns></returns>
        //private MemoryStream CreateStreamFromImage( Image image )
        //{
        //  MemoryStream memStream = new MemoryStream();

        //  if( image is Metafile )
        //  {
        //    m_srcMetafile = image as Metafile;
        //    Rectangle rect = m_srcMetafile.GetMetafileHeader().Bounds;
        //    Bitmap bitmap = new Bitmap( rect.Width, rect.Height, m_srcMetafile.PixelFormat );
        //    Graphics graphics1 = Graphics.FromImage( bitmap );
        //    IntPtr ptr = graphics1.GetHdc();
        //    Metafile metafile = new Metafile( memStream, ptr, EmfType.EmfOnly );
        //    graphics1.ReleaseHdc( ptr );
        //    using( Graphics g = Graphics.FromImage( metafile ) )
        //    {
        //      g.EnumerateMetafile( m_srcMetafile, rect.Location,
        //        new Graphics.EnumerateMetafileProc( PlayInMeta ) );
        //    }
        //    m_picData.lcb = 65;
        //  }
        //  else
        //  {
        //    if( image == null )
        //    {
        //      image = new Bitmap( 1, 1 );
        //    }
        //    image.Save( memStream, ImageFormat.Jpeg );
        //  }
        //  return memStream;
        //}
        #endregion
    }

    /// <summary>
    /// Summary description for ImageReader.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class WordImageReader : IWordImageReader
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private readonly MemoryStream m_dataStream;
        /// <summary>
        /// 
        /// </summary>
        private string m_strImageName = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        private int m_iStartImage = 0;
        /// <summary>
        /// 
        /// </summary>
        private PICF m_picData = new PICF();
        /// <summary>
        /// 
        /// </summary>
        private Image m_bitmap = null;
        /// <summary>
        /// 
        /// </summary>
        private MsofbtSpContainer m_spContainer;
        /// <summary>
        /// Defines whether image bytes are compresssed.
        /// </summary>
        //private bool m_isCompressed;
        /// <summary>
        /// 
        /// </summary>
        private string m_altText;
        /// <summary>
        /// Unparsed data;
        /// </summary>
        private byte[] m_unparsedData;
        /// <summary>
        /// Data stream position
        /// </summary>
        private int m_dataStreamPosiotion;
        private ImageRecord m_imageRecord;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets image name
        /// </summary>
        internal string ImageName
        {
            get
            {
                return m_strImageName;
            }
        }
        /// <summary>
        /// Gets/sets image width
        /// </summary>
        public short Width
        {
            get
            {
                return m_picData.dxaGoal;
            }
            set
            {
                m_picData.dxaGoal = value;
            }
        }
        /// <summary>
        /// Gets/sets image height
        /// </summary>
        public short Height
        {
            get
            {
                return m_picData.dyaGoal;
            }
            set
            {
                m_picData.dyaGoal = value;
            }
        }
        /// <summary>
        /// Gets bitmap
        /// </summary>
        public Image Image
        {
            get
            {
                return m_bitmap;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int WidthScale
        {
            get
            {
                return m_picData.mx;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int HeightScale
        {
            get
            {
                return m_picData.my;
            }
        }
        /// <summary>
        /// Gets inline shape's container.
        /// </summary>
        internal MsofbtSpContainer InlineShapeContainer
        {
            get
            {
                return m_spContainer;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal PICF PictureDescriptor
        {
            get
            {
                return m_picData;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ImageRecord ImageRecord
        {
            get
            {
                return m_imageRecord;
            }
        }
        /// <summary>
        /// Gets the alternative text.
        /// </summary>
        /// <value>The alternative text.</value>
        internal string AlternativeText
        {
            get
            {
                return m_altText;
            }
            set
            {
                m_altText = value;
            }
        }
        /// <summary>
        /// Get unparsed data
        /// </summary>
        internal byte[] UnparsedData
        {
            get
            {
                return m_unparsedData;
            }
        }
        #endregion

        #region Class initialize/finalize method
        /// <summary>
        /// Initialize class members and parse image from stream
        /// </summary>
        /// <param name="dataStream"></param>
        /// <param name="offset"></param>
        /// <param name="memConverter"></param>
        internal WordImageReader(MemoryStream dataStream, int offset, WordDocument doc)
        {
            try
            {
                if (offset > dataStream.Length)
                {
                    return;
                }
                m_dataStream = dataStream;
                m_iStartImage = offset;
                m_dataStream.Position = offset;

                BinaryReader reader = new BinaryReader(dataStream);

                m_picData.Read(reader);
                m_dataStreamPosiotion = (int)m_dataStream.Position;
                m_spContainer = MsofbtSpContainer.ReadInlineImageContainers((int)(m_picData.lcb - m_picData.cbHeader), m_dataStream, doc);
                UpdateProps();

                _Blip blip = MsofbtSpContainer.GetBlipFromShapeContainer(m_spContainer);

                m_imageRecord = blip.ImageRecord;
            }
            catch
            {
                if (m_spContainer == null)
                {
                    int dataLength = m_picData.lcb - m_picData.cbHeader;
                    if (dataLength > 0 && dataLength < (m_dataStream.Length - m_dataStreamPosiotion))
                    {
                        m_dataStream.Position = 0;
                        byte[] buffer = new byte [m_dataStream.Length];
                        m_dataStream.Read(buffer, 0, buffer.Length);

                        m_unparsedData = new byte[dataLength];
                        for (int i = 0; i < dataLength; i++ )
                        {
                            m_unparsedData[i] = buffer[m_dataStreamPosiotion + i];
                        }
                    }
                }
            }
        }
        private void UpdateProps()
        {
            byte[] complexProp = m_spContainer.GetComplexPropValue((int)FOPTEGroupShape.wzDescription);
            if (complexProp != null)
            {
#if SILVERLIGHT || WP
              string complexString = Encoding.Unicode.GetString( complexProp, 0, complexProp.Length );
#else
              string complexString = Encoding.Unicode.GetString( complexProp );
#endif
              m_altText = complexString.Replace( "\0", string.Empty );
            }
        }
        #endregion
    }
}
