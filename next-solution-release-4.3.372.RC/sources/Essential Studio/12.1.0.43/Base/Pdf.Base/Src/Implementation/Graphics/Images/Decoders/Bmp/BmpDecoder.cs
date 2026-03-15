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
using System.Globalization;

namespace Syncfusion.Pdf.Graphics.Images.Decoder
{
    internal class BmpDecoder : ImageDecoder
    {
        #region Constants
        private const int c_fileHeaderSize = 14;
        private const int c_BitmapHeaderSize = 40;
        private const int c_rMask = 0x00007C00;
        private const int c_gMask = 0x000003E0;
        private const int c_bMask = 0x0000001F;
        #endregion

        #region Fields
        private byte[] m_BmpHeader = { 66, 77 };
        private PdfStream m_imageStream;
        private BitmapFileHeader m_fileHeader;
        private BitmapInfoHeader m_infoHeader;
        private byte[] m_palette;
        private byte[] m_csPalette;
        #endregion

        #region Constructor
        public BmpDecoder(Stream stream)
        {
            base.InternalStream = stream;
            base.Format = ImageFormat.Bmp;
            Initialize();
        }
        #endregion

        #region Overrides
        protected override void Initialize()
        {
            base.InternalStream.Reset();

            byte[] buffer = new byte[base.InternalStream.Length];
            base.InternalStream.Read(buffer, 0, buffer.Length);

            base.ImageData = buffer;

            ReadImage();
        }

        private void ReadImage()
        {
            ReadFileHeader();
            ReadInfoHeader();
            ReadImageData();
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
            m_imageStream[DictionaryProperties.ColorSpace] = GetColorSpace();

            return m_imageStream;
        }
        #endregion

        #region Implementation
        private void ReadFileHeader()
        {
            byte[] buff = new byte[c_fileHeaderSize];
            m_fileHeader = new BitmapFileHeader();

            base.InternalStream.Reset();
            base.InternalStream.Read(buff, 0, buff.Length);

            m_fileHeader.FileSize = BitConverter.ToInt32(buff, 2);
            m_fileHeader.OffSet = BitConverter.ToInt32(buff, 10);
        }

        private void ReadInfoHeader()
        {
            byte[] buff = new byte[c_BitmapHeaderSize];
            m_infoHeader = new BitmapInfoHeader();

            base.InternalStream.Read(buff, 0, buff.Length);

            m_infoHeader.Size = BitConverter.ToInt32(buff, 0);
            m_infoHeader.Width = BitConverter.ToInt32(buff, 4);
            m_infoHeader.Height = BitConverter.ToInt32(buff, 8);
            m_infoHeader.Planes = BitConverter.ToInt16(buff, 12);
            m_infoHeader.BitsPerPixel = BitConverter.ToInt16(buff, 14);
            m_infoHeader.Compression = (BitmapCompression)BitConverter.ToInt32(buff, 16);
            m_infoHeader.SizeImage = BitConverter.ToInt32(buff, 20);
            m_infoHeader.XPelsPerMeter = BitConverter.ToInt32(buff, 24);
            m_infoHeader.YPelsPerMeter = BitConverter.ToInt32(buff, 28);
            m_infoHeader.ClrUsed = BitConverter.ToInt32(buff, 32);
            m_infoHeader.ClrImportant = BitConverter.ToInt32(buff, 36);


            base.Height = m_infoHeader.Height;
            base.Width = m_infoHeader.Width;
            base.BitsPerComponent = m_infoHeader.BitsPerPixel;
        }

        private void ReadImageData()
        {
            int colorMapSize = -1;

            if (m_infoHeader.ClrUsed == 0)
            {
                if (m_infoHeader.BitsPerPixel == 1 ||
                    m_infoHeader.BitsPerPixel == 4 ||
                    m_infoHeader.BitsPerPixel == 8)
                {
                    colorMapSize = (int)Math.Pow(2, m_infoHeader.BitsPerPixel) * 4;
                }
            }
            else
            {
                colorMapSize = m_infoHeader.ClrUsed * 4;
            }

            if (colorMapSize > 0)
            {
                m_palette = new byte[colorMapSize];
                base.InternalStream.Read(m_palette, 0, colorMapSize);
            }

            switch (m_infoHeader.Compression)
            {
                case BitmapCompression.RGB:
                    DecodeRGBImage();
                    break;

                case BitmapCompression.Bitfield:
                case BitmapCompression.RunlengthEncoding4:
                case BitmapCompression.RunlengthEncoding8:
                    throw new NotImplementedException("The specified image format is not supported.");
                    break;
            }

            
        }

        /// <summary>
        /// Decodes the un-compressed RGB image.
        /// </summary>
        private void DecodeRGBImage()
        {
            System.Diagnostics.Debug.Assert(m_infoHeader.Size != 40,
                        string.Format(CultureInfo.CurrentCulture,
                            "Header Size value '{0}' is not valid.", m_infoHeader.Size));

            switch (m_infoHeader.BitsPerPixel)
            {
                case 32 :
                    Decode32BitRGB();
                    break;

                case 24 :
                    Decode24BitRGB();
                    break;

                case 16 :
                    Decode16BitRGB();
                    break;

                case 8 :
                    Decode8bitRGB();
                    break;

                case 4 :
                    Decode4BitRGB();
                    break;

                case 1 :
                    Decode1BitRGB();
                    break;
            }
        }

        private bool CheckIfValidBmp()
        {
            return ((base.InternalStream.ReadByte() == m_BmpHeader[0])
                && (base.InternalStream.ReadByte() == m_BmpHeader[1]));
        }

        private PdfArray GetColorSpace()
        {
            PdfArray colorSpace = new PdfArray();

            if (m_infoHeader.ClrUsed > 0)
            {
                colorSpace.Add(new PdfName(DictionaryProperties.Indexed));
                colorSpace.Add(new PdfName(DictionaryProperties.DeviceRGB));
                colorSpace.Add(new PdfNumber(m_csPalette.Length / 3 - 1));
                colorSpace.Add(new PdfString((m_csPalette)));
            }
            else
            {
                colorSpace.Add(new PdfName(DictionaryProperties.DeviceRGB));
            }

            return colorSpace;
        }

        private void Decode1BitRGB()
        {
            base.ImageData = new byte[((m_infoHeader.Width + 7) / 8) * m_infoHeader.Height];
            int padding = 0;
            int bytesPerScanline = (int)Math.Ceiling((double)m_infoHeader.Width / 8.0);

            int remainder = bytesPerScanline % 4;
            if (remainder != 0)
            {
                padding = 4 - remainder;
            }

            int imSize = (bytesPerScanline + padding) * m_infoHeader.Height;

            byte[] values = new byte[imSize];
            int bytesRead = 0;
            while (bytesRead < imSize)
            {
                bytesRead += base.InternalStream.Read(values, bytesRead,
                imSize - bytesRead);
            }

            if (m_infoHeader.Height > 0)
            {
                for (int i = 0; i < m_infoHeader.Height; i++)
                {
                    Array.Copy(values,
                    imSize - (i + 1) * (bytesPerScanline + padding),
                    base.ImageData,
                    i * bytesPerScanline, bytesPerScanline);
                }
            }
            else
            {
                for (int i = 0; i < m_infoHeader.Height; i++)
                {
                    Array.Copy(values,
                    i * (bytesPerScanline + padding),
                    ImageData,
                    i * bytesPerScanline,
                    bytesPerScanline);
                }
            }

            SetColorSpacePalette(4);
        }


        private void Decode4BitRGB()
        {
            base.ImageData = new byte[((m_infoHeader.Width + 1) / 2) * m_infoHeader.Height];

            int padding = 0;
            int bytesPerScanline = (int)Math.Ceiling(m_infoHeader.Width / 2.0);
            int remainder = bytesPerScanline % 4;
            if (remainder != 0)
            {
                padding = 4 - remainder;
            }

            int imSize = (bytesPerScanline + padding) * m_infoHeader.Height;

            byte[] values = new byte[imSize];
            base.InternalStream.Read(values, 0, imSize);


            if (m_infoHeader.Height > 0)
            {

                for (int i = 0; i < m_infoHeader.Height; i++)
                {
                    Array.Copy(values,
                    imSize - (i + 1) * (bytesPerScanline + padding),
                    base.ImageData,
                    i * bytesPerScanline,
                    bytesPerScanline);
                }
            }
            else
            {
                for (int i = 0; i < m_infoHeader.Height; i++)
                {
                    Array.Copy(values,
                    i * (bytesPerScanline + padding),
                    base.ImageData,
                    i * bytesPerScanline,
                    bytesPerScanline);
                }
            }

            SetColorSpacePalette(4);
        }

        private void Decode8bitRGB()
        {
            base.ImageData = new byte[m_infoHeader.Width * m_infoHeader.Height];
            int padding = 0;

            int bitsPerScanline = m_infoHeader.Width * 8;
            if (bitsPerScanline % 32 != 0)
            {
                padding = (bitsPerScanline / 32 + 1) * 32 - bitsPerScanline;
                padding = (int)Math.Ceiling(padding / 8.0);
            }

            int imSize = (m_infoHeader.Width + padding) * m_infoHeader.Height;

            byte[] values = new byte[imSize];
            int bytesRead = 0;
            while (bytesRead < imSize)
            {
                bytesRead += base.InternalStream.Read(values, bytesRead, imSize - bytesRead);
            }

            if (m_infoHeader.Height > 0)
            {

                for (int i = 0; i < m_infoHeader.Height; i++)
                {
                    Array.Copy(values,
                    imSize - (i + 1) * (m_infoHeader.Width + padding),
                    base.ImageData,
                    i * m_infoHeader.Width,
                    m_infoHeader.Width);
                }
            }
            else
            {
                for (int i = 0; i < m_infoHeader.Height; i++)
                {
                    Array.Copy(values,
                    i * (m_infoHeader.Width + padding),
                    base.ImageData,
                    i * m_infoHeader.Width,
                    m_infoHeader.Width);
                }
            }

            SetColorSpacePalette(4);
        }

        private void Decode16BitRGB()
        {
            byte r, g, b;

            int scaleR = 256 / 32;
            int scaleG = 256 / 64;

            int alignment = 0;
            byte[] data = GetImageArray(m_infoHeader.Width, m_infoHeader.Height, 2, ref alignment);

            int offset, row, rowOffset, arrayOffset;

            for (int y = 0; y < m_infoHeader.Height; y++)
            {
                rowOffset = y * (m_infoHeader.Width * 2 + alignment);

                row = Invert(y, m_infoHeader.Height);
                for (int x = 0; x < m_infoHeader.Width; x++)
                {
                    offset = rowOffset + x * 2;

                    short temp = BitConverter.ToInt16(data, offset);

                    r = (byte)(((temp & c_rMask) >> 11) * scaleR);
                    g = (byte)(((temp & c_gMask) >> 5) * scaleG);
                    b = (byte)(((temp & c_bMask)) * scaleR);

                    arrayOffset = (row * m_infoHeader.Width + x) * 4;
                    base.ImageData[arrayOffset + 0] = r;
                    base.ImageData[arrayOffset + 1] = g;
                    base.ImageData[arrayOffset + 2] = b;

                    base.ImageData[arrayOffset + 3] = (byte)255;
                }
            }
        }

        private void Decode24BitRGB()
        {
            base.ImageData = new byte[m_infoHeader.Width * m_infoHeader.Height * 3];
            base.BitsPerComponent = 8;

            int padding = 0;

            // width * bitsPerPixel should be divisible by 32
            int bitsPerScanline = m_infoHeader.Width * 24;
            if (bitsPerScanline % 32 != 0)
            {
                padding = (bitsPerScanline / 32 + 1) * 32 - bitsPerScanline;
                padding = (int)Math.Ceiling(padding / 8.0);
            }


            int imSize = ((m_infoHeader.Width * 3 + 3) / 4 * 4) * m_infoHeader.Height;

            byte[] values = new byte[imSize];
            base.InternalStream.Read(values, 0, values.Length);

            int l = 0, count;

            if (m_infoHeader.Height > 0)
            {
                int max = m_infoHeader.Width * m_infoHeader.Height * 3 - 1;

                count = -padding;
                for (int i = 0; i < m_infoHeader.Height; i++)
                {
                    l = max - (i + 1) * m_infoHeader.Width * 3 + 1;
                    count += padding;
                    for (int j = 0; j < m_infoHeader.Width; j++)
                    {
                        base.ImageData[l + 2] = values[count++];
                        base.ImageData[l + 1] = values[count++];
                        base.ImageData[l] = values[count++];
                        l += 3;
                    }
                }
            }
            else
            {
                count = -padding;
                for (int i = 0; i < m_infoHeader.Height; i++)
                {
                    count += padding;
                    for (int j = 0; j < m_infoHeader.Width; j++)
                    {
                        base.ImageData[l + 2] = values[count++];
                        base.ImageData[l + 1] = values[count++];
                        base.ImageData[l] = values[count++];
                        l += 3;
                    }
                }
            }

            SetColorSpacePalette(4);
        }

        private void Decode32BitRGB()
        {
            int alignment = 0;
            byte[] data = GetImageArray(m_infoHeader.Width, m_infoHeader.Height, 4, ref alignment);

            int offset, row, rowOffset, arrayOffset;

            for (int y = 0; y < m_infoHeader.Height; y++)
            {
                rowOffset = y * (m_infoHeader.Width * 4 + alignment);

                row = Invert(y, m_infoHeader.Height);

                for (int x = 0; x < m_infoHeader.Width; x++)
                {
                    offset = rowOffset + x * 4;

                    arrayOffset = (row * m_infoHeader.Width + x) * 4;
                    base.ImageData[arrayOffset + 0] = data[offset + 0];
                    base.ImageData[arrayOffset + 1] = data[offset + 1];
                    base.ImageData[arrayOffset + 2] = data[offset + 2];

                    base.ImageData[arrayOffset + 3] = (byte)255;
                }
            }

            SetColorSpacePalette(4);
        }

        private int Invert(int y, int height)
        {
            int row = 0;

            if (height > 0)
            {
                row = (height - y - 1);
            }
            else
            {
                row = y;
            }

            return row;
        }

        private byte[] GetImageArray(int width, int height, int bytes, ref int alignment)
        {
            int dataWidth = width;

            alignment = (width * bytes) % 4;
            if (alignment != 0)
            {
                alignment = 4 - alignment;
            }

            int size = (dataWidth * bytes + alignment) * height;

            byte[] data = new byte[size];
            base.InternalStream.Read(data, 0, size);

            return data;
        }

        private void SetColorSpacePalette(int bpc)
        {
            if (m_palette == null)
                return;

            m_csPalette = new byte[m_palette.Length / bpc * 3];
            int e = m_palette.Length / bpc;
            for (int k = 0; k < e; ++k)
            {
                int src = k * bpc;
                int dest = k * 3;
                m_csPalette[dest + 2] = m_palette[src++];
                m_csPalette[dest + 1] = m_palette[src++];
                m_csPalette[dest] = m_palette[src];
            }
        }
        #endregion
    }
}
