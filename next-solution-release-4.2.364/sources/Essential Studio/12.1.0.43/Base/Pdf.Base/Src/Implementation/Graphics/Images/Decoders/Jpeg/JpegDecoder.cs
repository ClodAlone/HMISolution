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
    internal class JpegDecoder : ImageDecoder
    {
        #region Constants
        /// <summary>
        /// Start of image marker.
        /// </summary>
        private const ushort c_SoiMarker = 0xD8FF;
        /// <summary>
        /// JFIF marker.
        /// </summary>
        private const ushort c_JfifMarker = 0xE0FF;
        /// <summary>
        /// Start of scan marker.
        /// </summary>
        private const ushort c_SosMarker = 0xDAFF;
        /// <summary>
        /// End of image marker.
        /// </summary>
        private const ushort c_EoiMarker = 0xD9FF;
        #endregion

        #region Fields
        private byte[] m_jpegHeader = { 255, 216 };
        private PdfStream m_imageStream;
        #endregion

        #region Constructor
        public JpegDecoder(Stream stream)
        {
            base.InternalStream = stream;
            base.Format = ImageFormat.Jpeg;
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

            ReaderHeader();
        }

        private void ReaderHeader()
        {
            base.InternalStream.Reset();
            base.BitsPerComponent = 8;

            byte[] imgData = new byte[base.InternalStream.Length];
            base.InternalStream.Read(imgData, 0, imgData.Length);
            long i = 4;

            if ((imgData[i + 2] == 'J' && imgData[i + 3] == 'F' &&
                imgData[i + 4] == 'I' && imgData[i + 5] == 'F' &&
                imgData[i + 6] == 0) || (imgData[i + 2] == 'E' && imgData[i + 3] == 'x' &&
                imgData[i + 4] == 'i' && imgData[i + 5] == 'f' &&
                imgData[i + 6] == 0))
            {
                long length = imgData[i] * 256 + imgData[i + 1];
                while (i < imgData.Length)
                {
                    i += length;
                    if (imgData[i + 1] == 192)
                    {
                        base.Width = imgData[i + 7] * 256 + imgData[i + 8];
                        base.Height = imgData[i + 5] * 256 + imgData[i + 6];
                        return;
                    }
                    else
                    {
                        i += 2;
                        length = imgData[i] * 256 + imgData[i + 1];
                    }
                }
            }
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
            m_imageStream[DictionaryProperties.Filter] = new PdfName(DictionaryProperties.DCTDecode);
            m_imageStream[DictionaryProperties.ColorSpace] = new PdfName(GetColorSpace());
            m_imageStream[DictionaryProperties.DecodeParms] = GetDecodeParams();

            return m_imageStream;
        }
        #endregion

        #region Implementation
        private string GetColorSpace()
        {
            string colorSpace = "DeviceRGB";
            int i = 0;
            int step = 2;

            ushort soi = BitConverter.ToUInt16(base.ImageData, i);

            if (soi == c_SoiMarker)
            {
                i += step;

                ushort jfif = BitConverter.ToUInt16(base.ImageData, i);

                jfif &= 0xF0FF;

                if (jfif == c_JfifMarker)
                {
                    while (true)
                    {
                        i += step;

                        int markerLength = (int)BitConverter.ToUInt16(base.ImageData, i);

                        markerLength = ((markerLength >> 8) | (markerLength << 8) & 0xFFFF);
                        i += markerLength;

                        ushort marker = BitConverter.ToUInt16(base.ImageData, i);

                        if (marker == c_SosMarker)
                        {
                            i += step + 2;
                            switch (base.ImageData[i])
                            {
                                case 1:
                                    return "DeviceGray";

                                case 3:
                                    return "DeviceRGB";

                                case 4:
                                    return "DeviceCMYK";
                            }
                        }
                        else if (marker == c_EoiMarker)
                        {
                            break;
                        }
                    }
                }
            }
            return colorSpace;
        }

        private PdfDictionary GetDecodeParams()
        {
            PdfDictionary decodeParams = new PdfDictionary();
            decodeParams[DictionaryProperties.Columns] = new PdfNumber(base.Width);
            decodeParams[DictionaryProperties.BlackIs1] = new PdfBoolean(true);
            decodeParams[DictionaryProperties.K] = new PdfNumber(-1);
            decodeParams[DictionaryProperties.Predictor] = new PdfNumber(15);
            decodeParams[DictionaryProperties.BitsPerComponent] = new PdfNumber(base.BitsPerComponent);
            return decodeParams;
        }
        #endregion
    }
}
