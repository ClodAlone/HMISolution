#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics.Images.Decoder
{
    internal class PngDecoder : ImageDecoder
    {
        #region Fields
        private static float[] m_decode = new float[] { 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f };
        private int m_currentChunkLength;
        private PdfStream m_imageStream;
        private PngHeader m_header;
        private bool m_bDecodeIdat;
        private int m_bitsPerPixel;
        private long m_idatLength;
        private int m_colors;
        private int m_inputBands;
        private int m_bytesPerPixel;
        private Stream m_iDatStream;
        #endregion

        #region Enums
        internal enum PngChunkTypes
        {
            //Critical chunks
            IHDR,
            PLTE,
            IDAT,
            IEND,
            //Optional/Ancillary chunks
            bKGD,
            cHRM,
            gAMA,
            hIST,
            pHYs,
            sBIT,
            tEXt,
            tIME,
            tRNS,
            zTXt,
            sRGB,
            Unknown
        }

        internal enum PngFilterTypes
        {
            None,
            Sub,
            Up,
            Average,
            Paeth
        }

        internal enum PngImageTypes
        {
            GreyScale = 0,
            TrueColor = 2,
            IndexedColor = 3,
            GrayScaleWithAlpha = 4,
            TrueColorWithAlpha = 6
        }
        #endregion

        #region Constructor
        public PngDecoder(Stream stream)
        {
            base.InternalStream = stream;
            base.Format = ImageFormat.Png;
            Initialize();
        }
        #endregion

        #region Overrides
        protected override void Initialize()
        {
            PngChunkTypes header;

            while (ReadNextchunk(out header))
            {
                switch (header)
                {
                    case PngChunkTypes.IHDR:
                        ReadHeader();
                        break;

                    case PngChunkTypes.IDAT:
                        ReadImageData();
                        break;

                    case PngChunkTypes.IEND:
                        break;

                    case PngChunkTypes.gAMA:
                    case PngChunkTypes.sRGB:
                    case PngChunkTypes.cHRM:
                    case PngChunkTypes.tEXt:
                        IgnoreChunk();
                        break;
                }
            }

        }

        internal void InitializeBase()
        {
            base.Width = m_header.Width;
            base.Height = m_header.Height;
            base.BitsPerComponent = m_header.BitDepth;
        }

        internal override PdfStream GetImageDictionary()
        {
            m_imageStream = new PdfStream();
            m_imageStream.InternalStream = new MemoryStream(base.ImageData);
            m_imageStream.Compress = false;

            m_imageStream[DictionaryProperties.Type] = new PdfName(DictionaryProperties.XObject);
            m_imageStream[DictionaryProperties.Subtype] = new PdfName(DictionaryProperties.Image);
            m_imageStream[DictionaryProperties.Width] = new PdfNumber(base.Width);
            m_imageStream[DictionaryProperties.Height] = new PdfNumber(base.Height);
            m_imageStream[DictionaryProperties.BitsPerComponent] = new PdfNumber(base.BitsPerComponent);
            m_imageStream[DictionaryProperties.Filter] = new PdfName(DictionaryProperties.FlateDecode);
            m_imageStream[DictionaryProperties.ColorSpace] = new PdfName(DictionaryProperties.DeviceRGB); //GetColorSpace();
            //m_imageStream[DictionaryProperties.Decode] = new PdfArray(m_decode);
            m_imageStream[DictionaryProperties.DecodeParms] = GetDecodeParams();

            return m_imageStream;
        }

        private PdfArray GetColorSpace()
        {
            PdfArray colorSpace = new PdfArray();

            return colorSpace;
        }

        private PdfDictionary GetDecodeParams()
        {
            PdfDictionary decodeParams = new PdfDictionary();
            decodeParams[DictionaryProperties.Columns] = new PdfNumber(base.Width);
            decodeParams[DictionaryProperties.Colors] = new PdfNumber(m_colors);
            decodeParams[DictionaryProperties.Predictor] = new PdfNumber(15);
            decodeParams[DictionaryProperties.BitsPerComponent] = new PdfNumber(base.BitsPerComponent);
            return decodeParams;
        }
        #endregion

        #region Implementation
        private bool ReadNextchunk(out PngChunkTypes header)
        {
            header = PngChunkTypes.Unknown;
            m_currentChunkLength = InternalStream.ReadUInt32();
            string chunk = InternalStream.ReadString(4);

            if (Enum.IsDefined(typeof(PngChunkTypes), chunk))
            {
                header = (PngChunkTypes)Enum.Parse(typeof(PngChunkTypes), chunk, true);
                return true;
            }

            return false;
        }

        private void ReadHeader()
        {
            m_header.Width = InternalStream.ReadUInt32();
            m_header.Height = InternalStream.ReadUInt32();
            m_header.BitDepth = InternalStream.ReadByte();
            m_header.ColorType = InternalStream.ReadByte();
            m_header.Compression = InternalStream.ReadByte();
            m_header.Filter = (PngFilterTypes)InternalStream.ReadByte();
            m_header.Interlace = InternalStream.ReadByte();

            m_bDecodeIdat = ((m_header.ColorType & 4) != 0);
            m_colors = (m_header.ColorType == 3 || (m_header.ColorType & 2) == 0) ? 1 : 3;
            m_bytesPerPixel = (m_header.BitDepth == 16) ? 2 : 1;

            InitializeBase();
            SetBitsPerPixel();

            InternalStream.Skip(4);

        }

        private void SetBitsPerPixel()
        {
            m_bitsPerPixel = (m_header.BitDepth == 16) ? 2 : 1;

            if (m_header.ColorType == 0)
            {
                m_idatLength = (base.BitsPerComponent * base.Width + 7) / 8 * base.Height;
                m_inputBands = 1;
            }
            else if (m_header.ColorType == 2)
            {
                m_idatLength = (base.Width * base.Height * 3);
                m_inputBands = 3;
                m_bitsPerPixel *= 3;
            }
            else if (m_header.ColorType == 3)
            {
                if (m_header.Interlace == 1)
                    m_idatLength = (m_header.BitDepth * base.Width + 7) / 8 * base.Height;

                m_inputBands = 1;
                m_bitsPerPixel = 1;
            }
            else if (m_header.ColorType == 4)
            {
                m_idatLength = base.Width * base.Height;
                m_inputBands = 2;
                m_bitsPerPixel *= 2;
            }
            else if (m_header.ColorType == 6)
            {
                m_idatLength = base.Width * 3 * base.Height;
                m_inputBands = 4;
                m_bitsPerPixel *= 4;
            }
        }

        private void ReadImageData()
        {
            byte[] decodedIdat = null;
            byte[] buffer = new byte[m_currentChunkLength];
            base.InternalStream.Read(buffer, 0, m_currentChunkLength);


            if (m_bDecodeIdat)
            {
                decodedIdat = new byte[m_idatLength];

                m_iDatStream = new MemoryStream();
                m_iDatStream.Write(buffer, 0, buffer.Length);
                m_iDatStream.Position = 0;

                if (m_header.Interlace == 0)
                {
                    //DecodeImageData();
                    DecodePass(0, 0, 1, 1, base.Width, base.Height);
                }

            }

            base.ImageData = (decodedIdat == null) ? buffer : decodedIdat;

            InternalStream.Skip(4);
        }

        private void DecodeImageData()
        {
            int len = (m_inputBands * base.Width * m_header.BitDepth + 7) / 8;
            byte[] curr = new byte[len];
            byte[] prior = new byte[len];

            PngFilterTypes filter;


        }


        void DecodePass(int xOffset, int yOffset,
        int xStep, int yStep,
        int passWidth, int passHeight)
        {

            int bytesPerRow = (m_inputBands * passWidth * m_header.BitDepth + 7) / 8;
            byte[] curr = new byte[bytesPerRow];
            byte[] prior = new byte[bytesPerRow];

            // Decode the (sub)image row-by-row
            int srcY, dstY;
            for (srcY = 0, dstY = yOffset;
            srcY < passHeight;
            srcY++, dstY += yStep)
            {
                int filter = m_iDatStream.ReadByte();
                m_iDatStream.Read(curr, 0, bytesPerRow);

                switch ((PngFilterTypes)filter)
                {
                    case PngFilterTypes.None:
                        break;
                    //case PngFilterTypes.Sub:
                    //    DecodeSubFilter(curr, bytesPerRow, bytesPerPixel);
                    //    break;
                    //case PngFilterTypes.Up:
                    //    DecodeUpFilter(curr, prior, bytesPerRow);
                    //    break;
                    //case PngFilterTypes.Average:
                    //    DecodeAverageFilter(curr, prior, bytesPerRow, bytesPerPixel);
                    //    break;
                    //case PngFilterTypes.Paeth:
                    //    DecodePaethFilter(curr, prior, bytesPerRow, bytesPerPixel);
                    //    break;
                    //default:
                    //    // Error -- uknown filter type
                    //    throw new Exception("PNG filter unknown.");
                }

                //ProcessPixels(curr, xOffset, xStep, dstY, passWidth);

                // Swap curr and prior
                byte[] tmp = prior;
                prior = curr;
                curr = tmp;
            }
        }

        private void IgnoreChunk()
        {
            InternalStream.Skip(m_currentChunkLength + 4);
        }
        #endregion

        #region Internals
        internal struct PngHeader
        {
            /// <summary>
            /// Indicates the width of the image.
            /// </summary>
            public int Width;
            /// <summary>
            /// Indicates the height of the image.
            /// </summary>
            public int Height;
            /// <summary>
            /// Indicates the Png image type.
            /// <remarks>Possible values : 0,2,3,4 and 6.</remarks>
            /// </summary>
            public int ColorType;
            /// <summary>
            /// Indicates the type of compression.
            /// <remarks>
            /// 0 - Zlib Compression
            /// </remarks>
            /// </summary>
            public int Compression;
            /// <summary>
            /// Indicates the number of bits per sample or per palette index.
            /// <remarks>Valid values are : 1,2,4,8 and 16.</remarks>
            /// |-----------------------------------------|
            /// | ColorType | BitDepth   | ImageType      |
            /// |-----------------------------------------|
            /// |     0     | 1,2,4,8,16 | GrayScale      |
            /// |     2     | 8,16       | TrueColor      |
            /// |     3     | 1,2,4,8    | Indexed PLTE   |
            /// |     4     | 8,16       | GrayScaleAlpha |
            /// |     6     | 8,16       | TrueColorAlpha |
            /// |------------------------------------------
            /// </summary>
            public int BitDepth;
            /// <summary>
            /// Indicates the preprocessing method applied to the image data before compression.
            /// </summary>
            public PngFilterTypes Filter;
            /// <summary>
            /// Indicates the transmission order of the image data.
            /// <remarks>Possible values : 0 and 1</remarks>
            /// </summary>
            public int Interlace;
        }
        #endregion
    }
}
