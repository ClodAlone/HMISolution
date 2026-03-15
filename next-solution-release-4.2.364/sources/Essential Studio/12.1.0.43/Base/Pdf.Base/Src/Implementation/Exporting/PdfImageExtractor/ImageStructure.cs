#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Syncfusion.Pdf.Graphics;
using System.IO.Compression;
using Syncfusion.Pdf.Compression;
using Syncfusion.Pdf;

namespace Syncfusion.Pdf
{
    internal class ImageStructure
    {

        private PdfDictionary m_imageDictionary;
        private string[] m_imageFilter;
        private PdfDictionary[] m_decodeParam;
        private Stream m_imageStream;
        private Stream m_maskStream;
        private float m_height;
        private float m_width;
        private float m_maskWidth;
        private float m_maskHeight;
        private string m_maskFilter;
        private float m_maskBitsPerComponent;
        private float m_bitsPerComponent;
        private Image m_embeddedImage;
        private bool m_isImageStreamParsed = false;
        private PdfMatrix m_imageInfo;
        private string m_colorspace;
        private string m_colorspaceBase;
        private int m_colorspaceHival;
        private PixelFormat m_pixelFormat = PixelFormat.Format24bppRgb;
        private ColorPalette m_colorPalette;
        private MemoryStream m_colorspaceStream;
        private bool IsTransparent;
        internal StringBuilder exceptions = new StringBuilder();
        private Dictionary<string, MemoryStream> colorSpaceResourceDict = new Dictionary<string, MemoryStream>();
        private Dictionary<string, PdfStream> nonIndexedImageColorResource = new Dictionary<string, PdfStream>();
        private bool isIndexedImage = false;
        /// <summary>
        /// Occurs prior to the rendering of every image in the document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"> ImagePreRenderEventArgs</param>
        public delegate void ImagePreRenderEventHandler(object sender, ImagePreRenderEventArgs args);
        static public event ImagePreRenderEventHandler ImagePreRender;

        public ImageStructure()
        {

        }

        public ImageStructure(IPdfPrimitive fontDictionary, PdfMatrix tm)
        {
            m_imageDictionary = fontDictionary as PdfDictionary;
            ImageInfo = tm;
        }

        internal PdfDictionary ImageDictionary
        {
            get
            {
                return m_imageDictionary;
            }
        }

        internal PdfMatrix ImageInfo
        {
            get
            {
                return m_imageInfo;
            }
            set
            {
                m_imageInfo = value;
            }
        }

        internal string[] ImageFilter
        {
            get
            {
                if (m_imageFilter == null)
                {
                    m_imageFilter = GetImageFilter();
                }
                return m_imageFilter;
            }
        }

        internal string ColorSpace
        {
            get
            {
                if (m_colorspace == null)
                    GetColorSpace();
                return m_colorspace;
            }
            set
            {
                m_colorspace = value;
            }
        }

        internal Image EmbeddedImage
        {
            get
            {
                if (m_embeddedImage == null && !m_isImageStreamParsed)
                {
                    try
                    {
                        MemoryStream stream = GetImageStream() as MemoryStream;
                        m_embeddedImage = Image.FromStream(stream);
                        return m_embeddedImage;
                    }
                    catch
                    {
                        return null;
                    }
                }
                return m_embeddedImage;
            }
        }

        internal PdfDictionary[] DecodeParam
        {
            get
            {
                if (m_decodeParam == null)
                    m_decodeParam = GetDecodeParam();
                return m_decodeParam;
            }
        }

        public Stream ImageStream
        {
            get
            {
                if (m_imageStream == null)
                {
                    PdfStream imageStream = m_imageDictionary as PdfStream;
                    m_imageStream = imageStream.InternalStream;

                }
                return m_imageStream;
            }
            set
            {
                m_imageStream = value;
            }
        }

        /// <summary>
        /// Holds the stream of the mask image associated with the original image
        /// </summary>
        public Stream MaskStream
        {
            get
            {
                if (m_maskStream == null)
                {
                    PdfStream maskStream = (m_imageDictionary[DictionaryProperties.SMask] as PdfReferenceHolder).Object as PdfStream;
                    m_maskStream = maskStream.InternalStream;
                    PdfDictionary maskDictionary = (m_imageDictionary[DictionaryProperties.SMask] as PdfReferenceHolder).Object as PdfDictionary;

                    if (maskDictionary.ContainsKey(DictionaryProperties.Width))
                        m_maskWidth = (maskDictionary[DictionaryProperties.Width] as PdfNumber).IntValue;
                    if (maskDictionary.ContainsKey(DictionaryProperties.Height))
                        m_maskHeight = (maskDictionary[DictionaryProperties.Height] as PdfNumber).IntValue;
                    if (maskDictionary.ContainsKey(DictionaryProperties.BitsPerComponent))
                        m_maskBitsPerComponent = (maskDictionary[DictionaryProperties.BitsPerComponent] as PdfNumber).IntValue;
                    if (maskDictionary.ContainsKey(DictionaryProperties.Filter))
                    {
                        m_maskFilter = (maskDictionary[DictionaryProperties.Filter] as PdfName).Value;
                    }
                }
                return m_maskStream;
            }
            set
            {
                m_maskStream = value;
            }
        }

        internal float Width
        {
            get
            {
                if (m_width == 0)
                {
                    m_width = GetImageWidth();
                }
                return m_width;
            }

        }

        internal float Height
        {
            get
            {
                if (m_height == 0)
                {
                    m_height = GetImageHeight();
                }
                return m_height;
            }

        }

        /// <summary>
        /// Gets BitsPerComponent value of the original image
        /// </summary>
        internal float BitsPerComponent
        {
            get
            {
                if (m_bitsPerComponent == 0)
                {
                    m_bitsPerComponent = GetBitsPerComponent();
                }
                return m_bitsPerComponent;
            }
        }

        public Stream GetImageStream()
        {
            m_isImageStreamParsed = true;
            bool IsDecodeFilterDefined = true;
            if (ImageFilter == null)
            {
                m_imageFilter = new string[] { "FlateDecode" };
                IsDecodeFilterDefined = false;
            }
            if (ImageFilter != null)
            {
                for (int k = 0; k < ImageFilter.Length; k++)
                {
                    switch (ImageFilter[k])
                    {
                        #region ASCII85
                        case "A85":
                        case "ASCII85Decode":
                            {
                                ImageStream = DecodeASCII85Stream(ImageStream as MemoryStream);
                                ImageStream.Position = 0;
                                break;
                            }
                        #endregion
                        #region ASCIIHex
                        case "ASCIIHex":
                            {
                                ASCIIHex decoder = new ASCIIHex();
                                byte[] decodedBytes = decoder.Decode((ImageStream as MemoryStream).GetBuffer());

                                MemoryStream outputStream = new MemoryStream(decodedBytes);
                                ImageStream = outputStream as Stream;
                                ImageStream.Position = 0;
                                break;
                            }
                        #endregion
                        #region RunLengthDecode
                        case "RunLengthDecode":
                            {
                                ImageStream.Position = 0;

                                byte[] buffer = (ImageStream as MemoryStream).ToArray();
                                int pointer = 0, length;
                                byte[] decodedBuffer, currentBuffer, tempBuffer;
                                decodedBuffer = new byte[0];
                                tempBuffer = new byte[0];
                                while (pointer < buffer.Length)
                                {
                                    tempBuffer = decodedBuffer;
                                    length = buffer[pointer];
                                    if (length >= 0 && length <= 127)
                                    {
                                        pointer++;
                                        currentBuffer = new byte[length + 1];
                                        Array.Copy(buffer, pointer, currentBuffer, 0, currentBuffer.Length);

                                        decodedBuffer = new byte[tempBuffer.Length + currentBuffer.Length];

                                        Array.Copy(tempBuffer, 0, decodedBuffer, 0, tempBuffer.Length);
                                        Array.Copy(currentBuffer, 0, decodedBuffer, tempBuffer.Length, currentBuffer.Length);
                                        pointer += length + 1;
                                    }
                                    else if (length >= 129 && length <= 255)
                                    {
                                        pointer++;
                                        byte arrayValue = buffer[pointer];
                                        int noOfTimes = 257 - length;
                                        currentBuffer = new byte[noOfTimes];
                                        for (int i = 0; i < noOfTimes; i++)
                                        {
                                            currentBuffer[i] = arrayValue;
                                        }

                                        decodedBuffer = new byte[tempBuffer.Length + currentBuffer.Length];

                                        Array.Copy(tempBuffer, 0, decodedBuffer, 0, tempBuffer.Length);
                                        Array.Copy(currentBuffer, 0, decodedBuffer, tempBuffer.Length, currentBuffer.Length);
                                        pointer++;
                                    }
                                    else if (length == 128)
                                    {
                                        break;
                                    }
                                }

                                byte[] imgeBytes = decodedBuffer;

                                Bitmap embeddedImage = new Bitmap((int)Width, (int)Height, m_pixelFormat);
                                BitmapData imageData = embeddedImage.LockBits(new Rectangle(0, 0, embeddedImage.Width, embeddedImage.Height), ImageLockMode.ReadWrite, embeddedImage.PixelFormat);

                                if (m_pixelFormat == PixelFormat.Format8bppIndexed)
                                    embeddedImage.Palette = m_colorPalette;

                                int bitsPerPixel = Image.GetPixelFormatSize(embeddedImage.PixelFormat);
                                int pixelSize = bitsPerPixel / 8;

                                switch (pixelSize)
                                {
                                    case 1:
                                    case 2:
                                        break;
                                    case 3:
                                        for (int i = 0; i + 3 < imgeBytes.Length; i += 3)
                                        {
                                            int i2 = i + 2;
                                            byte r = imgeBytes[i2];
                                            imgeBytes[i2] = imgeBytes[i];
                                            imgeBytes[i] = r;
                                        }
                                        break;
                                    case 4:
                                        byte[] outputStream1 = new byte[(int)(Width * Height * 3)];

                                        int rawSize = (int)(Width * Height * 4);
                                        byte[] pixelArray = imgeBytes;

                                        for (int i = 0, j = 0; i < rawSize + 4; i += 4)
                                        {
                                            outputStream1[j + 2] = pixelArray[i];
                                            outputStream1[j + 1] = pixelArray[i + 1];
                                            outputStream1[j] = pixelArray[i + 2];

                                            j += 3;
                                        }
                                        imgeBytes = outputStream1;
                                        break;
                                }

                                length = Math.Abs(imageData.Stride) * embeddedImage.Height;
                                if (length < imgeBytes.Length)
                                {
                                    int offset = 0;
                                    long imagePointer = imageData.Scan0.ToInt64();
                                    int w = (int)Width;
                                    if (pixelSize == 3)
                                        w = (int)Width * 3;

                                    for (int i = 0; i < Height; i++)
                                    {
                                        Marshal.Copy(imgeBytes, offset, new IntPtr(imagePointer), w);
                                        offset += w;
                                        imagePointer += imageData.Stride;
                                    }
                                }
                                else
                                    Marshal.Copy(imgeBytes, 0, imageData.Scan0, imgeBytes.Length);
                                embeddedImage.UnlockBits(imageData);

                                MemoryStream outStream = new MemoryStream();
                                embeddedImage.Save(outStream, ImageFormat.Jpeg);

                                if (!m_imageDictionary.ContainsKey(DictionaryProperties.SMask))
                                {
                                    ImageStream = outStream;
                                    ImageStream.Position = 0;
                                }
                                else
                                {
                                    try
                                    {
                                        ImageStream = (MergeImages(embeddedImage, MaskStream as MemoryStream));
                                        ImageStream.Position = 0;
                                    }
                                    catch (Exception)
                                    {
                                        ImageStream = outStream;
                                        ImageStream.Position = 0;
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region DCTDecode
                        case "DCTDecode":
                            {
                                if (!m_imageDictionary.ContainsKey(DictionaryProperties.SMask))
                                {
                                    ImageStream.Position = 0;
                                    if (ColorSpace == "DeviceCMYK")
                                    {
                                        if (m_imageDictionary.ContainsKey(DictionaryProperties.Decode))
                                        {
                                            PdfArray decode = m_imageDictionary[DictionaryProperties.Decode] as PdfArray;
                                            double[] stat = { 1, 0, 1, 0, 1, 0, 1, 0 };
                                            PdfArray refArray = new PdfArray(stat);
                                            bool isSame = true;
                                            for (int i = 0; i < refArray.Count; i++)
                                            {
                                                if ((decode[i] as PdfNumber).FloatValue != (refArray[i] as PdfNumber).FloatValue)
                                                    isSame = false;
                                            }
                                            if (isSame)
                                                break;
                                        }

                                        Bitmap bmp = Image.FromStream(ImageStream) as Bitmap;
                                        BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

                                        IntPtr imagePointer = data.Scan0;
                                        int bytes = Math.Abs(data.Stride) * bmp.Height;
                                        byte[] rgbValues = new byte[bytes];

                                        // Copy the RGB values into the array.
                                        Marshal.Copy(imagePointer, rgbValues, 0, bytes);

                                        byte[] decoded = YCCKtoRGB(rgbValues);
                                        Marshal.Copy(decoded, 0, imagePointer, decoded.Length);

                                        bmp.UnlockBits(data);
                                        Image img = bmp as Image;
                                        ImageStream = new MemoryStream();
                                        img.Save(ImageStream, ImageFormat.Jpeg);
                                        ImageStream.Position = 0;
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        Bitmap input = null;
                                        try
                                        {
                                            ImageStream.Position = 0;
                                            if (ColorSpace == "DeviceCMYK")
                                            {
                                                if (m_imageDictionary.ContainsKey(DictionaryProperties.Decode))
                                                {
                                                    PdfArray decode = m_imageDictionary[DictionaryProperties.Decode] as PdfArray;
                                                    double[] stat = { 1, 0, 1, 0, 1, 0, 1, 0 };
                                                    PdfArray refArray = new PdfArray(stat);
                                                    bool isSame = true;
                                                    for (int i = 0; i < refArray.Count; i++)
                                                    {
                                                        if ((decode[i] as PdfNumber).FloatValue != (refArray[i] as PdfNumber).FloatValue)
                                                            isSame = false;
                                                    }
                                                    if (isSame)
                                                        break;
                                                }

                                                Bitmap bmp = Image.FromStream(ImageStream) as Bitmap;
                                                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

                                                IntPtr imagePointer = data.Scan0;
                                                int bytes = Math.Abs(data.Stride) * bmp.Height;
                                                byte[] rgbValues = new byte[bytes];

                                                // Copy the RGB values into the array.
                                                Marshal.Copy(imagePointer, rgbValues, 0, bytes);

                                                byte[] decoded = YCCKtoRGB(rgbValues);
                                                Marshal.Copy(decoded, 0, imagePointer, decoded.Length);

                                                bmp.UnlockBits(data);
                                                Image img = bmp as Image;
                                                ImageStream = new MemoryStream();
                                                img.Save(ImageStream, ImageFormat.Jpeg);
                                                ImageStream.Position = 0;
                                            }
                                            ImageStream.Position = 0;
                                            input = Image.FromStream(ImageStream) as Bitmap;
                                        }
                                        catch
                                        {
                                            input = null;
                                        }
                                        MaskStream.Position=0;
                                        ImageStream = MergeImages(input, MaskStream as MemoryStream);
                                        ImageStream.Position = 0;
                                    }
                                    catch
                                    {
                                        ImageStream.Position = 0;
                                    }
                                }
                            }
                            break;
                        #endregion
                        #region FlateDecode
                        case "FlateDecode":
                            {
                                int predictor = 0, colors = 1, columns = 1;
                                MemoryStream outStream;
                                if (IsDecodeFilterDefined == true)
                                {
                                    outStream = DecodeFlateStream(ImageStream as MemoryStream);
                                }
                                else
                                {
                                    outStream = ImageStream as MemoryStream;
                                }

                                string temp = ColorSpace;
                                byte[] imgeBytes = null;
                                if (colorSpaceResourceDict.Count > 0)
                                {
                                    int depth = 0, width = 0, height = 0;
                                    isIndexedImage = true;

                                    if (m_imageDictionary.ContainsKey(DictionaryProperties.BitsPerComponent))
                                        depth = (m_imageDictionary[DictionaryProperties.BitsPerComponent] as PdfNumber).IntValue;
                                    if (m_imageDictionary.ContainsKey(DictionaryProperties.Width))
                                        width = (m_imageDictionary[DictionaryProperties.Width] as PdfNumber).IntValue;
                                    if (m_imageDictionary.ContainsKey(DictionaryProperties.Height))
                                        height = (m_imageDictionary[DictionaryProperties.Height] as PdfNumber).IntValue;

                                    if (colorSpaceResourceDict.ContainsKey("DeviceCMYK"))
                                    {
                                        byte[] convertedIndexBytes = ConvertIndexCMYKToRGB(colorSpaceResourceDict["Indexed"].GetBuffer());
                                        imgeBytes = ConvertIndexedStreamToFlat(depth, width, height, outStream.GetBuffer(), convertedIndexBytes, false, false);
                                    }
                                    else
                                    {
                                        imgeBytes = ConvertIndexedStreamToFlat(depth, width, height, outStream.GetBuffer(), colorSpaceResourceDict["Indexed"].GetBuffer(), false, false);
                                    }
                                }

                                if (ColorSpace == "DeviceGray")
                                {
                                    outStream.Position = 0;
                                    if (ImageFilter.Length > 1 && k == 0)
                                    {
                                        if (ImageFilter[k + 1] == "DCTDecode")
                                        {
                                            ImageStream = outStream;
                                            ImageStream.Position = 0;
                                            break;
                                        }
                                    }
                                    ImageStream = DecodeDeviceGrayImage(outStream as MemoryStream);
                                    ImageStream.Position = 0;
                                    break;
                                }

                                if (ImageFilter.Length > 1 && k == 0)
                                {
                                    if (ImageFilter[k + 1] == "DCTDecode" || ImageFilter[k + 1] == "RunLengthDecode")
                                    {
                                        ImageStream = new MemoryStream();
                                        ImageStream = outStream;
                                        ImageStream.Position = 0;
                                        break;
                                    }
                                }

                                if (!isIndexedImage)
                                {
                                    if (nonIndexedImageColorResource != null && nonIndexedImageColorResource.Count > 0)
                                    {
                                        PdfDictionary streamDic = nonIndexedImageColorResource["ICCBased"] as PdfDictionary;
                                        if (streamDic[DictionaryProperties.N] is PdfNumber)
                                        {
                                            if ((streamDic[DictionaryProperties.N] as PdfNumber).IntValue == 1)
                                            {
                                                int depth = 0;
                                                if (m_imageDictionary.ContainsKey(DictionaryProperties.BitsPerComponent))
                                                    depth = (m_imageDictionary[DictionaryProperties.BitsPerComponent] as PdfNumber).IntValue;
                                                if (depth == 8)
                                                    m_pixelFormat = PixelFormat.Format8bppIndexed;

                                                imgeBytes = (outStream as MemoryStream).GetBuffer();
                                                for (int i = 0; i < imgeBytes.Length; i++)
                                                {
                                                    if (imgeBytes[i] != 0 && imgeBytes[i] != 255)
                                                        imgeBytes[i] = 0;
                                                }
                                            }
                                            else
                                                imgeBytes = (outStream as MemoryStream).GetBuffer();
                                        }
                                        else
                                            imgeBytes = (outStream as MemoryStream).GetBuffer();
                                    }
                                    else if (ColorSpace == "DeviceCMYK")
                                    {
                                        imgeBytes = YCCToRGB(outStream.GetBuffer());
                                        outStream = new MemoryStream(imgeBytes);
                                    }
                                    else
                                    {
                                        imgeBytes = (outStream as MemoryStream).GetBuffer();
                                    }
                                }

                                if (ImageDictionary.ContainsKey(DictionaryProperties.Mask))
                                {
                                    IsTransparent = true;
                                    if (ImageDictionary[DictionaryProperties.Mask] is PdfArray)
                                    {
                                        PdfArray masks = ImageDictionary[DictionaryProperties.Mask] as PdfArray;
                                        for (int j = 0; j < masks.Count; )
                                        {
                                            int upperLimit = 0, lowerLimit = 0;
                                            if (masks[j] is PdfNumber)
                                            {
                                                lowerLimit = (masks[j] as PdfNumber).IntValue;
                                                j++;
                                            }
                                            if (masks[j] is PdfNumber)
                                            {
                                                upperLimit = (masks[j] as PdfNumber).IntValue;
                                                j++;
                                            }

                                            for (int i = 0; i < imgeBytes.Length; i++)
                                            {
                                                if (imgeBytes[i] >= lowerLimit && imgeBytes[i] <= upperLimit)
                                                    imgeBytes[i] = 255;
                                            }
                                        }
                                    }
                                }

                                PdfDictionary decodeParams = new PdfDictionary();
                                Bitmap embeddedImage = new Bitmap((int)Width, (int)Height, m_pixelFormat);
                                BitmapData imageData = embeddedImage.LockBits(new Rectangle(0, 0, embeddedImage.Width, embeddedImage.Height), ImageLockMode.ReadWrite, embeddedImage.PixelFormat);

                                if ((m_pixelFormat == PixelFormat.Format8bppIndexed || m_pixelFormat == PixelFormat.Format4bppIndexed) && m_colorPalette != null)
                                    embeddedImage.Palette = m_colorPalette;

                                int bitsPerPixel = Image.GetPixelFormatSize(embeddedImage.PixelFormat);
                                int pixelSize = bitsPerPixel / 8;

                                if (m_imageDictionary.ContainsKey(DictionaryProperties.DecodeParms))
                                {
                                    decodeParams = DecodeParam[k];

                                    if (decodeParams.ContainsKey(DictionaryProperties.Predictor))
                                        predictor = (decodeParams[DictionaryProperties.Predictor] as PdfNumber).IntValue;
                                    if (decodeParams.ContainsKey(DictionaryProperties.Columns))
                                        columns = (decodeParams[DictionaryProperties.Columns] as PdfNumber).IntValue;
                                    if (decodeParams.ContainsKey(DictionaryProperties.Colors))
                                        colors = (decodeParams[DictionaryProperties.Colors] as PdfNumber).IntValue;

                                    MemoryStream decodedStream = DecodePredictor(predictor, colors, columns, outStream);
                                    imgeBytes = decodedStream.GetBuffer();
                                }

                                switch (pixelSize)
                                {
                                    case 1:
                                    case 2:
                                        break;
                                    case 3:
                                        for (int i = 0; i + 3 < imgeBytes.Length; i += 3)
                                        {
                                            int i2 = i + 2;
                                            byte r = imgeBytes[i2];
                                            imgeBytes[i2] = imgeBytes[i];
                                            imgeBytes[i] = r;
                                        }
                                        break;
                                    case 4:
                                        byte[] outputStream1 = new byte[(int)(Width * Height * 3)];

                                        int rawSize = (int)(Width * Height * 4);
                                        byte[] pixelArray = imgeBytes;

                                        for (int i = 0, j = 0; i < rawSize + 4; i += 4)
                                        {
                                            outputStream1[j + 2] = pixelArray[i];
                                            outputStream1[j + 1] = pixelArray[i + 1];
                                            outputStream1[j] = pixelArray[i + 2];

                                            j += 3;
                                        }
                                        imgeBytes = outputStream1;
                                        break;
                                }

                                int offset = 0;
                                long imagePointer = imageData.Scan0.ToInt64();
                                int w = (int)Width;
                                if (pixelSize == 3)
                                    w = (int)Width * 3;

                                if (m_pixelFormat == PixelFormat.Format4bppIndexed)
                                {
                                    for (int i = 0; i < Height / 2; i++)
                                    {
                                        if (i % 2 == 0)
                                        {
                                            Marshal.Copy(imgeBytes, offset, new IntPtr(imagePointer), w);
                                            imagePointer += imageData.Stride;
                                        }
                                        if (i % 3 == 0)
                                        {
                                            Marshal.Copy(imgeBytes, offset, new IntPtr(imagePointer), w);
                                            imagePointer += imageData.Stride;
                                        }
                                        if (i % 7 == 0)
                                        {
                                            Marshal.Copy(imgeBytes, offset, new IntPtr(imagePointer), w);
                                            imagePointer += imageData.Stride;
                                        }

                                        Marshal.Copy(imgeBytes, offset, new IntPtr(imagePointer), w);
                                        imagePointer += imageData.Stride;

                                        offset += w;
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < Height; i++)
                                    {
                                        Marshal.Copy(imgeBytes, offset, new IntPtr(imagePointer), w);
                                        offset += w;
                                        imagePointer += imageData.Stride;
                                    }
                                }
                                embeddedImage.UnlockBits(imageData);
                                if (IsTransparent)
                                    embeddedImage.MakeTransparent();
                                outStream = new MemoryStream();
                                embeddedImage.Save(outStream, ImageFormat.Png);

                                if (!m_imageDictionary.ContainsKey(DictionaryProperties.SMask))
                                {
                                    ImageStream = outStream;
                                    ImageStream.Position = 0;
                                }
                                else
                                {
                                    try
                                    {
                                        ImageStream = (MergeImages(embeddedImage, MaskStream as MemoryStream));
                                        ImageStream.Position = 0;
                                    }
                                    catch (Exception)
                                    {
                                        ImageStream = outStream;
                                        ImageStream.Position = 0;
                                    }
                                }

                                break;
                            }
                        #endregion
                        #region CCITTFax
                        case "CCITTFaxDecode":
                            {

                                PdfDictionary decodeParams = new PdfDictionary();
                                if (m_imageDictionary.ContainsKey(DictionaryProperties.DecodeParms))
                                    decodeParams = DecodeParam[k];

                                TiffDecode tiff = new TiffDecode();
                                tiff.m_tiffHeader.m_byteOrder = TiffDecode.LittleEndian;
                                tiff.m_tiffHeader.m_version = (short)TiffDecode.LittleEndianVersion;
                                tiff.m_tiffHeader.m_dirOffset = (uint)(ImageStream.Length + 8 + 1);

                                tiff.WriteHeader(tiff.m_tiffHeader);

                                tiff.m_stream.Seek(8, 0);
                                tiff.m_stream.Write((ImageStream as MemoryStream).GetBuffer(), 0, (int)ImageStream.Length);

                                tiff.SetField(1, (int)Width, TiffTag.ImageWidth, TiffType.Short);
                                tiff.SetField(1, (int)Height, TiffTag.ImageLength, TiffType.Short);
                                tiff.SetField(1, 1, TiffTag.BitsPerSample, TiffType.Short);
                                if (decodeParams != null && decodeParams.ContainsKey(DictionaryProperties.K))
                                {
                                    if ((decodeParams[DictionaryProperties.K] as PdfNumber).IntValue < 0)
                                        tiff.SetField(1, 4, TiffTag.Compression, TiffType.Short);
                                    else if ((decodeParams[DictionaryProperties.K] as PdfNumber).IntValue == 0)
                                    {
                                        if (decodeParams.ContainsKey(DictionaryProperties.EndOfBlock))
                                        {
                                            if(!(decodeParams[DictionaryProperties.EndOfBlock] as PdfBoolean).Value)
                                                tiff.SetField(1, 2, TiffTag.Compression, TiffType.Short);
                                        }
                                        else
                                            tiff.SetField(1, 3, TiffTag.Compression, TiffType.Short);
                                    }
                                    else
                                        tiff.SetField(1, 3, TiffTag.Compression, TiffType.Short);
                                    if (decodeParams.ContainsKey(DictionaryProperties.BlackIs1))
                                    {
                                        PdfBoolean isBlack = decodeParams[DictionaryProperties.BlackIs1] as PdfBoolean;
                                        if (isBlack.Value)
                                        {
                                            tiff.SetField(1, 1, TiffTag.Photometric, TiffType.Short);
                                        }
                                        else
                                        {
                                            tiff.SetField(1, 0, TiffTag.Photometric, TiffType.Short);
                                        }
                                    }
                                }
                                else
                                    tiff.SetField(1, 3, TiffTag.Compression, TiffType.Short);
                                tiff.SetField(1, 8, TiffTag.StripOffset, TiffType.Long);
                                tiff.SetField(1, 1, TiffTag.SamplesPerPixel, TiffType.Short);
                                tiff.SetField(1, (int)ImageStream.Length, TiffTag.StripByteCounts, TiffType.Long);
                                tiff.m_stream.Seek(9 + ImageStream.Length, 0);
                                tiff.WriteDirEntry(tiff.directoryEntries);
                                tiff.m_stream.Position = 0;
                                tiff.m_stream.Capacity = (int)tiff.m_stream.Length;
                                if (!ImageDictionary.ContainsKey(DictionaryProperties.ImageMask))
                                {
                                    ImageStream = tiff.m_stream;
                                    ImageStream.Position = 0;
                                }
                                else
                                {
                                    bool isImageMask = (ImageDictionary[DictionaryProperties.ImageMask] as PdfBoolean).Value;
                                    if (isImageMask)
                                    {
                                        MemoryStream str = tiff.m_stream;
                                        Bitmap embeddedImage = new Bitmap(str as Stream);
                                        embeddedImage.MakeTransparent(Color.White);

                                        MemoryStream outStream = new MemoryStream();
                                        embeddedImage.Save(outStream, ImageFormat.Png);

                                        ImageStream = outStream;
                                        ImageStream.Position = 0;
                                    }
                                    else
                                    {
                                        ImageStream = tiff.m_stream;
                                        ImageStream.Position = 0;
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region JBIG2Decode
                        case "JBIG2Decode":
                            {
                                MemoryStream encodedStream = ImageStream as MemoryStream;
                                JBIG2StreamDecoder decoder = new JBIG2StreamDecoder();

                                MemoryStream globalStream = new MemoryStream();

                                byte[] imagedata = null;
                                if (m_imageDictionary.ContainsKey(DictionaryProperties.DecodeParms))
                                {
                                    PdfDictionary globalDictionary = m_imageDictionary[DictionaryProperties.DecodeParms] as PdfDictionary;
                                    if (globalDictionary != null)
                                    {
                                        if (globalDictionary.ContainsKey(DictionaryProperties.JBIG2Globals))
                                        {
                                            globalStream = ((globalDictionary[DictionaryProperties.JBIG2Globals] as PdfReferenceHolder).Object as PdfStream).InternalStream;
                                        }
                                    }
                                    else
                                    {
                                        string filter = "";
                                        PdfArray decodeParams = m_imageDictionary[DictionaryProperties.DecodeParms] as PdfArray;
                                        globalDictionary = decodeParams[0] as PdfDictionary;

                                        if (globalDictionary.ContainsKey(DictionaryProperties.JBIG2Globals))
                                        {
                                            PdfDictionary globalStreamDic = (globalDictionary[DictionaryProperties.JBIG2Globals] as PdfReferenceHolder).Object as PdfDictionary;
                                            if (globalStreamDic != null && globalStreamDic.ContainsKey(DictionaryProperties.Filter))
                                            {
                                                filter = (globalStreamDic[DictionaryProperties.Filter] as PdfName).Value.ToString();
                                            }
                                            globalStream = ((globalDictionary[DictionaryProperties.JBIG2Globals] as PdfReferenceHolder).Object as PdfStream).InternalStream;
                                            if (filter == "FlateDecode")
                                            {
                                                globalStream.Position = 0;

                                                globalStream.ReadByte();
                                                globalStream.ReadByte();
                                                DeflateStream s = new DeflateStream(globalStream, CompressionMode.Decompress, true);
                                                byte[] buffer = new byte[4096];
                                                MemoryStream outStream = new MemoryStream();
                                                do
                                                {
                                                    int bytesRead = s.Read(buffer, 0, 4096);
                                                    if (bytesRead <= 0)
                                                        break;
                                                    outStream.Write(buffer, 0, bytesRead);
                                                } while (true);
                                                globalStream = outStream;
                                            }
                                        }
                                    }
                                    if (globalStream.Length > 0)
                                    {
                                        globalStream.Capacity = (int)globalStream.Length;
                                        decoder.GlobalData = globalStream.GetBuffer();
                                    }
                                }

                                decoder.DecodeJBIG2(encodedStream.GetBuffer());

                                JBIG2Image imag = decoder.GetPageAsJBIG2Bitmap(0);
                                imagedata = imag.GetData(true);

                                Bitmap iii = new Bitmap((int)Width, (int)Height, PixelFormat.Format1bppIndexed);
                                BitmapData iiiData = iii.LockBits(new Rectangle(0, 0, (int)Width, (int)Height), ImageLockMode.ReadWrite, iii.PixelFormat);

                                int rows = (int)Height;
                                int stride = iiiData.Stride;
                                IntPtr portionOffset = iiiData.Scan0;

                                int bpp = Image.GetPixelFormatSize(iii.PixelFormat);
                                int pixSize = bpp / 8;

                                int rowWidth = pixSize * (int)Width;

                                //We should align row depending to pixel size;
                                if (pixSize == 3)
                                {
                                    //NOTE: 3.0 = RGB pixel size.
                                    rowWidth = pixSize * (int)Width;
                                }
                                else if (pixSize == 0)
                                {
                                    //NOTE: 0.125 = 1/8 and its 1 bit for pixel or 8 pixels per byte.
                                    rowWidth = (int)Width * bpp / 8;

                                    if (((int)Width * bpp % 8) != 0)
                                        ++rowWidth;
                                }
                                else if (pixSize == 1)
                                {
                                    rowWidth = iii.Width;
                                }

                                int offset = 0;
                                long ptr = iiiData.Scan0.ToInt64();
                                for (int i = 0; i < Height; i++)
                                {
                                    Marshal.Copy(imagedata, offset, new IntPtr(ptr), (int)(rowWidth));
                                    offset += (int)(rowWidth);
                                    ptr += iiiData.Stride;
                                }

                                iii.UnlockBits(iiiData);

                                Image finalImage = iii as Image;
                                MemoryStream embeddedImageStream = new MemoryStream();
                                finalImage.Save(embeddedImageStream, ImageFormat.Jpeg);
                                embeddedImageStream.Position = 0;
                                ImageStream = embeddedImageStream;
                                ImageStream.Position = 0;
                                break;
                            }
                        #endregion
                        case "LZWDecode":
                            {
                                PdfLzwCompressor compression=new PdfLzwCompressor();
                                Stream stream=new MemoryStream();
                                compression.Decompress((ImageStream as MemoryStream).ToArray(),stream);
                                ImageStream = new MemoryStream();
                                ImageStream = stream;
                                break;
                            }
                        case "JPXDecode":
                            {
                                ImageStream.Position = 0;
                                JPXImage jpxImage = new JPXImage();
                                Bitmap image = (Bitmap)jpxImage.FromStream(ImageStream);
                                MemoryStream memoryStream = new MemoryStream();
                                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                                return memoryStream;
                            }
                        default:
                            {
                                if (String.IsNullOrEmpty(ImageFilter[k]))
                                {
                                    throw new Exception("Error in identifying ImageFilter");
                                }
                                else
                                {
                                    throw new Exception(ImageFilter + " does not supported");
                                }
                            }
                    }
                }
                return ImageStream;
            }
            return null;
        }

        /// <summary>
        /// Converts DeviceCMYK colorspace to RGB colourspace
        /// </summary>
        private byte[] ConvertIndexCMYKToRGB(byte[] data)
        {
            byte[] rgb = null;
            int len = data.Length;
            rgb = new byte[len * 3 / 4];
            int rgbIndex = 0;

            for (int i = 0; i < len; i = i + 4)
            {
                float[] vals = new float[4];

                for (int j = 0; j < 4; j++)
                {
                    vals[j] = (data[i + j] & 255) / 255f;
                }

                float[] rgbValues = ConvertCMYKToRGB(vals);
                rgb[rgbIndex] = (byte)(((int)rgbValues[0] ));
                rgb[rgbIndex + 1] = (byte)(((int)rgbValues[1] ) );
                rgb[rgbIndex + 2] = (byte)(((int)rgbValues[2]));

                rgbIndex = rgbIndex + 3;
                if (len - 4 - i < 4)
                    i = len;
            }
            return rgb;
        }
        /// <summary>
        /// Converts the CMYK values to RGB
        /// </summary>
        private float[] ConvertCMYKToRGB(float[] values)
        {
            float r, g, b;
            float c, m, y, k;
            c = values[0];
            m = values[1];
            y = values[2];
            k = values[3];

            r = 255 * (1 - c) * (1 - k);
            g = 255 * (1 - m) * (1 - k);
            b = 255 * (1 - y) * (1 - k);

            return new float[] { r, g, b };
        }

        /// <summary>
        /// Process the ColorSpace property in the Image dictionary
        /// </summary>
        private void GetColorSpace()
        {
            if (m_imageDictionary.ContainsKey(DictionaryProperties.ColorSpace))
            {
                string[] filter = null;
                string internalColorSpace = null;
                PdfDictionary colorspaceDictionary = null;
                PdfArray value = null;
                PdfArray colorSpaceResources = null;
                if (m_imageDictionary[DictionaryProperties.ColorSpace] is PdfArray)
                    value = m_imageDictionary[DictionaryProperties.ColorSpace] as PdfArray;
                if (m_imageDictionary[DictionaryProperties.ColorSpace] is PdfReferenceHolder)
                    value = (m_imageDictionary[DictionaryProperties.ColorSpace] as PdfReferenceHolder).Object as PdfArray;
                if (m_imageDictionary[DictionaryProperties.ColorSpace] is PdfName)
                    m_colorspace = (m_imageDictionary[DictionaryProperties.ColorSpace] as PdfName).Value;

                if (value != null)
                {
                    m_colorspace = (value[0] as PdfName).Value;
                    colorSpaceResources = value;

                    if (value.Count == 4)
                    {
                        MemoryStream indexStream;
                        bool IsRunlengthEncoding = false;
                        foreach (string imgfilter in ImageFilter)
                        {
                            if (imgfilter == "RunLengthDecode")
                                IsRunlengthEncoding = true;
                        }

                        if ((colorSpaceResources[0] as PdfName).Value == "Indexed" && colorSpaceResources[colorSpaceResources.Count - 1].GetType().Name == "PdfReferenceHolder")
                        {
                            try
                            {
                                if (((colorSpaceResources[colorSpaceResources.Count - 1] as PdfReferenceHolder).Object as PdfDictionary).Values.Count > 1)
                                {
                                    indexStream = ((colorSpaceResources[colorSpaceResources.Count - 1] as PdfReferenceHolder).Object as PdfStream).InternalStream;
                                    indexStream = DecodeFlateStream(indexStream);
                                    colorSpaceResourceDict.Add("Indexed", indexStream);
                                    isIndexedImage = true;
                                }
                                else
                                {
                                    indexStream = ((colorSpaceResources[colorSpaceResources.Count - 1] as PdfReferenceHolder).Object as PdfStream).InternalStream;
                                    colorSpaceResourceDict.Add("Indexed", indexStream);
                                    if (ImageDictionary.ContainsKey(DictionaryProperties.DecodeParms))
                                        GetIndexedColorSpace(value, internalColorSpace, colorspaceDictionary, filter);
                                    isIndexedImage = true;
                                }

                                if ((colorSpaceResources[colorSpaceResources.Count - 3]).GetType().Name == "PdfName")
                                {
                                    string indexedColorSpace = ((colorSpaceResources[colorSpaceResources.Count - 3]) as PdfName).Value;
                                    colorSpaceResourceDict.Add(indexedColorSpace, new MemoryStream());
                                }
                            }
                            catch
                            {
                                isIndexedImage = false;
                                GetIndexedColorSpace(value, internalColorSpace, colorspaceDictionary, filter);
                            }
                        }
                        else
                        {
                            isIndexedImage = false;
                            GetIndexedColorSpace(value, internalColorSpace, colorspaceDictionary, filter);
                        }
                    }
                    else
                    {
                        isIndexedImage = false;
                        GetIndexedColorSpace(value, internalColorSpace, colorspaceDictionary, filter);
                    }
                }
            }
        }
        private void GetIndexedColorSpace(PdfArray value, string internalColorSpace, PdfDictionary colorspaceDictionary,string[] filter)
        {
            if (m_colorspace == "ICCBased")
            {
                if ((value[1] as PdfReferenceHolder).Object is PdfStream)
                {
                    PdfStream colorspaceStream = (value[1] as PdfReferenceHolder).Object as PdfStream;
                    nonIndexedImageColorResource = new Dictionary<string, PdfStream>();
                    nonIndexedImageColorResource.Add(m_colorspace, colorspaceStream);
                }
            }
            else if (m_colorspace == "Indexed")
            {
                if (value[1] is PdfName)
                {
                    m_colorspaceBase = (value[1] as PdfName).Value;
                }

                else if (value[1] is PdfReferenceHolder)
                {
                    if ((value[1] as PdfReferenceHolder).Object is PdfArray)
                    {
                        PdfArray temp = (value[1] as PdfReferenceHolder).Object as PdfArray;
                        if (temp[0] is PdfName)
                            internalColorSpace = (temp[0] as PdfName).Value;
                        if (temp[1] is PdfReferenceHolder)
                        {
                            PdfDictionary internalColorDic = (temp[1] as PdfReferenceHolder).Object as PdfDictionary;
                            if (internalColorDic.ContainsKey(DictionaryProperties.Alternate))
                            {
                                m_colorspaceBase = (internalColorDic[DictionaryProperties.Alternate] as PdfName).Value;
                            }
                        }
                    }
                }
                else if ((value[1] as PdfArray) != null)
                {
                    PdfArray temp = value[1] as PdfArray;
                    if (temp[0] is PdfName)
                        internalColorSpace = (temp[0] as PdfName).Value;
                    if (temp[1] is PdfReferenceHolder)
                    {
                        PdfDictionary internalColorDic = (temp[1] as PdfReferenceHolder).Object as PdfDictionary;
                        if (internalColorDic.ContainsKey(DictionaryProperties.Alternate))
                        {
                            m_colorspaceBase = (internalColorDic[DictionaryProperties.Alternate] as PdfName).Value;
                        }
                    }
                }

                if (m_colorspaceBase == "DeviceRGB" || m_colorspaceBase == "DeviceGray")
                {
                    if (m_colorspaceBase == "DeviceGray")
                    {
                        ColorSpace = "IndexedDeviceGray";
                        IsTransparent = true;
                    }
                    m_colorspaceHival = (value[2] as PdfNumber).IntValue;
                    if (value[3] is PdfReferenceHolder)
                    {
                        m_colorspaceStream = ((value[3] as PdfReferenceHolder).Object as PdfStream).InternalStream;
                        colorspaceDictionary = (value[3] as PdfReferenceHolder).Object as PdfDictionary;
                    }
                    else if (value[3] is PdfString)
                    {
                        string colorsSpaceString = (value[3] as PdfString).Value;

                        if (colorsSpaceString.Contains("ColorFound"))
                        {
                            if (colorsSpaceString.IndexOf("ColorFound") == 0)
                                colorsSpaceString = colorsSpaceString.Remove(0, 10);
                        }
                        colorsSpaceString = GetLiteralString(colorsSpaceString);
                        colorsSpaceString = SkipEscapeSequence(colorsSpaceString);
                        byte[] buffer = GetAsciiBytes(colorsSpaceString);
                        m_colorspaceStream = new MemoryStream(buffer, 0, buffer.Length, true, true);
                    }
                    if (BitsPerComponent == 4 && internalColorSpace != "ICCBased")
                    {
                        m_pixelFormat = PixelFormat.Format4bppIndexed;
                        IsTransparent = true;
                    }
                    else if (BitsPerComponent == 8)
                        m_pixelFormat = PixelFormat.Format8bppIndexed;
                }
            }

            if (colorspaceDictionary != null)
            {
                if (colorspaceDictionary.ContainsKey("Filter"))
                {
                    if (colorspaceDictionary[DictionaryProperties.Filter] is PdfName)
                    {
                        filter = new string[1];
                        filter[0] = (colorspaceDictionary[DictionaryProperties.Filter] as PdfName).Value;
                    }
                    else if (colorspaceDictionary[DictionaryProperties.Filter] is PdfArray)
                    {
                        int count = (colorspaceDictionary[DictionaryProperties.Filter] as PdfArray).Count;
                        filter = new string[count];
                        for (int k = 0; k < count; k++)
                            filter[k] = ((colorspaceDictionary[DictionaryProperties.Filter] as PdfArray)[k] as PdfName).Value;
                    }
                    else if (colorspaceDictionary[DictionaryProperties.Filter] is PdfReferenceHolder)
                    {
                        PdfArray filterArray = ((colorspaceDictionary[DictionaryProperties.Filter] as PdfReferenceHolder).Object as PdfArray);
                        filter = new string[filterArray.Count];
                        for (int k = 0; k < filterArray.Count; k++)
                            filter[k] = (filterArray[0] as PdfName).Value;
                    }
                }
            }
            if (filter != null)
            {
                for (int k = 0; k < filter.Length; k++)
                {
                    switch (filter[k])
                    {
                        case "FlateDecode":
                            m_colorspaceStream = DecodeFlateStream(m_colorspaceStream);
                            break;
                        case "ASCII85":
                        case "ASCII85Decode":
                            m_colorspaceStream = DecodeASCII85Stream(m_colorspaceStream);
                            break;
                        default:
                            throw new Exception("Filter to decode colorspace not implemented.");
                    }
                }
            }

            if (m_colorspace == "Indexed" || m_colorspace == "IndexedDeviceGray")
            {
                byte[] colorspaceByteSrc = m_colorspaceStream.GetBuffer();
                byte[] colorspaceByte = new byte[786];
                Array.Copy(colorspaceByteSrc, colorspaceByte, colorspaceByteSrc.Length);

                Bitmap RGBImage = new Bitmap((int)Width, (int)Height, PixelFormat.Format8bppIndexed);
                m_colorPalette = RGBImage.Palette;
                int count = 0;
                for (int i = 0; i < m_colorPalette.Entries.Length; i++)
                    m_colorPalette.Entries[i] = Color.FromArgb((int)colorspaceByte[count++], (int)colorspaceByte[count++], (int)colorspaceByte[count++]);
            }
        }

        private byte[] ConvertIndexedStreamToFlat(int d, int w, int h, byte[] data, byte[] index, Boolean isARGB, Boolean isDownsampled)
        {
            int[] bandsRGB = { 0, 1, 2 };
            int[] bandsARGB = { 0, 1, 2, 3 };
            int[] bands;
            int components = 3;

            int indexLength = 0;

            if (index != null)
                indexLength = index.Length;

            if (isARGB)
            {
                bands = bandsARGB;
                components = 4;
            }
            else
                bands = bandsRGB;

            byte[] newData = ConvertIndexedStreamToFlat(d, w, h, data, index, isARGB, isDownsampled, components, indexLength);

            return newData;
        }

        private byte[] ConvertIndexedStreamToFlat(int d, int w, int h, byte[] data, byte[] index, Boolean isARGB, Boolean isDownsampled, int components, int indexLength)
        {
            int id1, pt = 0;
            int length = (w * h * components);
            byte[] newData = new byte[length];
            int id = 0;
            float ratio = 0f;

            if (d == 8)
            {
                for (int i = 0; i < data.Length - 1; i++)
                {
                    if (isDownsampled)
                        ratio = (data[i] & 0xff) / 255f;
                    else
                        id = (data[i] & 0xff) * 3;

                    if (pt >= length)
                        break;

                    if (isDownsampled)
                    {
                        if (ratio > 0)
                        {
                            newData[pt++] = (byte)((255 - index[0]) * ratio);
                            newData[pt++] = (byte)((255 - index[1]) * ratio);
                            newData[pt++] = (byte)((255 - index[2]) * ratio);
                        }
                        else
                            pt = pt + 3;
                    }
                    else
                    {
                        if (id < indexLength)
                        {
                            newData[pt++] = index[id];
                            newData[pt++] = index[id + 1];
                            newData[pt++] = index[id + 2];
                        }
                    }

                    if (isARGB)
                    {
                        if (id == 0 && ratio == 0)
                            newData[pt++] = (byte)255;
                        else
                            newData[pt++] = 0;
                    }
                }
            }
            else if (d == 4)
            {
                int[] shift = { 4, 0 };
                int widthReached = 0;

                for (int i = 0; i < data.Length; i++)
                {
                    for (int samples = 0; samples < 2; samples++)
                    {
                        id1 = ((data[i] >> shift[samples]) & 15) * 3;

                        if (pt >= length)
                            break;

                        newData[pt++] = index[id1];
                        newData[pt++] = index[id1 + 1];
                        newData[pt++] = index[id1 + 2];

                        if (isARGB)
                        {
                            if (id1 == 0)
                                newData[pt++] = (byte)0;
                            else
                                newData[pt++] = 0;
                        }                        
                        widthReached++;
                        if (widthReached == w)
                        {
                            widthReached = 0;
                            samples = 8;
                        }
                    }
                }
            }
            else if (d == 2)
            {
                int[] shift = { 6, 4, 2, 0 };
                int widthReached = 0;

                for (int i = 0; i < data.Length; i++)
                {
                    for (int samples = 0; samples < 4; samples++)
                    {
                        id1 = ((data[i] >> shift[samples]) & 3) * 3;

                        if (pt >= length)
                            break;

                        newData[pt++] = index[id1];
                        newData[pt++] = index[id1 + 1];
                        newData[pt++] = index[id1 + 2];

                        if (isARGB)
                        {
                            if (id1 == 0)
                                newData[pt++] = (byte)0;
                            else
                                newData[pt++] = 0;
                        }

                        //ignore filler bits
                        widthReached++;
                        if (widthReached == w)
                        {
                            widthReached = 0;
                            samples = 8;
                        }
                    }
                }
            }
            else if (d == 1)
            {                
                int widthReached = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    for (int bits = 0; bits < 8; bits++)
                    {                        
                        id = ((data[i] >> (7 - bits)) & 1) * 3;

                        if (pt >= length)
                            break;
                       
                        if (isARGB)
                        {
                            if (id == 0)
                            {
                                newData[pt++] = index[id];
                                newData[pt++] = index[id + 1];
                                newData[pt++] = index[id + 2];

                                newData[pt++] = (byte)255;

                            }
                            else
                            {
                                newData[pt++] = index[id];
                                newData[pt++] = index[id + 1];
                                newData[pt++] = index[id + 2];
                                newData[pt++] = 0;
                            }
                        }
                        else
                        {
                            newData[pt++] = index[id];
                            newData[pt++] = index[id + 1];
                            newData[pt++] = index[id + 2];
                        }
                        widthReached++;
                        if (widthReached == w)
                        {
                            widthReached = 0;
                            bits = 8;
                        }
                    }
                }
            }            
            return newData;
        }

        private string GetLiteralString(string encodedText)
        {
            string decodedText = encodedText;
            int octalIndex = -1;
            int limit = 3;
            while (decodedText.Contains("\\") || decodedText.Contains("\0"))
            {
                string octalText = string.Empty;
                if (decodedText.IndexOf('\\', octalIndex + 1) >= 0)
                {
                    octalIndex = decodedText.IndexOf('\\', octalIndex + 1);
                }
                else
                {
                    octalIndex = decodedText.IndexOf('\0', octalIndex + 1);
                    if (octalIndex < 0)
                        break;
                    limit = 2;
                }
                for (int i = octalIndex + 1; i <= octalIndex + limit; i++) //check for octal characters
                {
                    if (i < decodedText.Length)
                    {
                        int val = 0;
                        if (int.TryParse(decodedText[i].ToString(), out val))
                        {
                            if (val <= 8)
                                octalText += decodedText[i];
                        }
                        else
                        {
                            octalText = string.Empty;
                            break;
                        }
                    }
                    else
                        octalText = string.Empty;
                }

                if (octalText != string.Empty)
                {
                    int decimalValue = (int)Convert.ToUInt64(octalText, 8);
                    string temp;
                    char decodedChar = (char)decimalValue;

                    System.Text.Encoding encoding = System.Text.Encoding.GetEncoding(1252);
                    temp = encoding.GetString(new byte[] { Convert.ToByte(decimalValue) });

                    decodedText = decodedText.Remove(octalIndex, limit + 1);
                    decodedText = decodedText.Insert(octalIndex, temp);
                }
            }
            return decodedText;
        }

        /// <summary>
        /// Skips the escape sequence from the given input string
        /// </summary>
        /// <param name="text">String with the escape sequence</param>
        /// <returns>String without escape sequence</returns>
        private string SkipEscapeSequence(string text)
        {
            text = text.Replace("\\r", "\r");
            text = text.Replace("\\(", @"(");
            text = text.Replace("\\)", @")");
            text = text.Replace("\\n", "\n");
            text = text.Replace("\\t", "\t");
            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte[] GetAsciiBytes(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            byte[] bytes = new byte[value.Length];

            for (int i = 0, len = value.Length; i < len; ++i)
            {
                bytes[i] = (byte)value[i];
            }

            return bytes;
        }

        /// <summary>
        /// Decodes the ASCII85 encoded stream
        /// </summary>
        /// <param name="encodedStream">Encoded stream</param>
        /// <returns>Decoded Stream</returns>
        private MemoryStream DecodeASCII85Stream(MemoryStream encodedStream)
        {
            ASCII85 decoder = new ASCII85();
            byte[] decodedBytes = decoder.decode((encodedStream as MemoryStream).GetBuffer());

            MemoryStream outStream = new MemoryStream(decodedBytes, 0, decodedBytes.Length, true, true);
            outStream.Position = 0;
            return outStream;
        }

        /// <summary>
        /// Decodes the Flate encoded stream
        /// </summary>
        /// <param name="encodedStream">Encoded stream</param>
        /// <returns>Decoded Stream</returns>
        private MemoryStream DecodeFlateStream(MemoryStream encodedStream)
        {
            encodedStream.Position = 0;
            //Skip two bytes
            encodedStream.ReadByte();
            encodedStream.ReadByte();

            DeflateStream s = new DeflateStream(encodedStream, CompressionMode.Decompress, true);
            byte[] buffer = new byte[4096];
            MemoryStream outStream = new MemoryStream();

            do
            {
                int bytesRead = s.Read(buffer, 0, 4096);
                if (bytesRead <= 0)
                    break;
                outStream.Write(buffer, 0, bytesRead);
            } while (true);
            return outStream;
        }

        /// <summary>
        /// Merges the stream of the two images
        /// </summary>
        /// <param name="input">Stream of the original image</param>
        /// <param name="maskStream">Encoded stream of the mask image</param>
        /// <returns>Stream of the merged image</returns>
        private MemoryStream MergeImages(Bitmap input, MemoryStream maskStream)
        {
            Color transparentColor;
            if (input == null)
            {
                input = new Bitmap((int)m_maskWidth, (int)m_maskHeight);
                transparentColor = Color.FromArgb(255, 255, 255, 255);
            }
            else
                transparentColor = input.GetPixel(0, 0);
            Bitmap mask;
            if (m_maskFilter == "DCTDecode")
            {
                mask = Image.FromStream(maskStream) as Bitmap;
            }
            else if (m_maskFilter == "FlateDecode")
            {
                maskStream = DecodeFlateStream(maskStream);
                mask = DecodeMaskImage(maskStream);
            }
            else
            {
                mask = Image.FromStream(maskStream) as Bitmap;
            }
            Bitmap output = new Bitmap(input.Width, input.Height, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(0, 0, input.Width, input.Height);
            BitmapData bitsInput = input.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            BitmapData bitsMask = mask.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            BitmapData bitsOutput = output.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

            int bytes = Math.Abs(bitsInput.Stride) * input.Height;
            byte[] inputBytes = new byte[bytes];
            Marshal.Copy(bitsInput.Scan0, inputBytes, 0, bytes);

            bytes = Math.Abs(bitsMask.Stride) * mask.Height;
            byte[] maskBytes = new byte[bytes];
            Marshal.Copy(bitsMask.Scan0, maskBytes, 0, bytes);

            bytes = Math.Abs(bitsOutput.Stride) * output.Height;
            byte[] outputBytes = new byte[bytes];
            Marshal.Copy(bitsOutput.Scan0, outputBytes, 0, bytes);

            byte max = 255;
            for (int i = 0; i < bytes; )
            {
                if (maskBytes[i] != 0 || maskBytes[i + 1] != 0 || maskBytes[i + 2] != 0 || maskBytes[i + 3] != 255)
                {
                    Color inputColor = Color.FromArgb(inputBytes[i], inputBytes[i + 1], inputBytes[i + 2], inputBytes[i + 3]);
                    Color maskColor = Color.FromArgb((byte)(max - maskBytes[i]), (byte)(max - maskBytes[i + 1]), (byte)(max - maskBytes[i + 2]), (byte)(max - maskBytes[i + 3]));

                    float alpha, red, green, blue;
                    float inputAlpha, inputRed, inputBlue, inputGreen;
                    float maskAlpha, maskRed, maskBlue, maskGreen;
                    byte byteAlpha, byteRed, byteGreen, byteBlue;

                    inputAlpha = ConvertToFloat(inputColor.A);
                    inputRed = ConvertToFloat(inputColor.R);
                    inputGreen = ConvertToFloat(inputColor.G);
                    inputBlue = ConvertToFloat(inputColor.B);

                    maskAlpha = ConvertToFloat(maskColor.A);
                    maskRed = ConvertToFloat(maskColor.R);
                    maskGreen = ConvertToFloat(maskColor.G);
                    maskBlue = ConvertToFloat(maskColor.B);

                    alpha = inputAlpha + maskAlpha;
                    red = inputRed + maskRed;
                    green = inputGreen + maskGreen;
                    blue = inputBlue + maskBlue;

                    byteAlpha = ConvertToByte(alpha);
                    byteRed = ConvertToByte(red);
                    byteGreen = ConvertToByte(green);
                    byteBlue = ConvertToByte(blue);

                    outputBytes[i] = byteAlpha;
                    outputBytes[i + 1] = byteRed;
                    outputBytes[i + 2] = byteGreen;
                    outputBytes[i + 3] = byteBlue;
                }
                else
                {
                    if (transparentColor == Color.FromArgb(255, 0, 0, 0) || transparentColor == Color.FromArgb(255, 255, 255, 255))
                    {
                        outputBytes[i] = transparentColor.R;
                        outputBytes[i + 1] = transparentColor.G;
                        outputBytes[i + 2] = transparentColor.B;
                        outputBytes[i + 3] = transparentColor.A;
                    }
                    else
                    {
                        outputBytes[i] = 0;
                        outputBytes[i + 1] = 0;
                        outputBytes[i + 2] = 0;
                        outputBytes[i + 3] = 0;
                    }
                }
                i += 4;
            }

            Marshal.Copy(outputBytes, 0, bitsOutput.Scan0, bytes);

            mask.UnlockBits(bitsMask);
            input.UnlockBits(bitsInput);
            output.UnlockBits(bitsOutput);
            if (transparentColor == Color.FromArgb(255, 0, 0, 0) || transparentColor == Color.FromArgb(255, 255, 255, 255))
                output.MakeTransparent(transparentColor);
            else
                output.MakeTransparent();

            MemoryStream decodedStream = new MemoryStream();
            output.Save(decodedStream, ImageFormat.Png);
            decodedStream.Position = 0;

            input.Dispose();
            mask.Dispose();
            output.Dispose();

            return decodedStream;
        }

        /// <summary>
        /// sRgbToScRgb conversion
        /// </summary>
        /// <param name="byteValue">Byte value of the color</param>
        /// <returns>Equivalent float value</returns>
        private float ConvertToFloat(byte byteValue)
        {
            float number = ((float)byteValue) / 255f;
            if (number <= 0.0)
            {
                return 0f;
            }
            if (number <= 0.04045)
            {
                return (number / 12.92f);
            }
            if (number < 1f)
            {
                return (float)Math.Pow((number + 0.055) / 1.055, 2.4);
            }
            return 1f;
        }

        /// <summary>
        /// ScRgbTosRgb conversion
        /// </summary>
        /// <param name="value">Float value of the color</param>
        /// <returns>Equivalent byte value</returns>
        private byte ConvertToByte(float value)
        {
            if (value <= 0.0)
            {
                return 0;
            }
            if (value <= 0.0031308)
            {
                return (byte)(((255f * value) * 12.92f) + 0.5f);
            }
            if (value < 1.0)
            {
                return (byte)((255f * ((1.055f * ((float)Math.Pow((double)value, 0.41666666666666669))) - 0.055f)) + 0.5f);
            }
            return 0xff;
        }

        /// <summary>
        /// Decodes the stream of the mask image in the PDF document
        /// </summary>
        /// <param name="mask">Encoded stream from the PDF document</param>
        /// <returns>Decoded stream of the image</returns>
        private Bitmap DecodeMaskImage(MemoryStream mask)
        {
            PixelFormat maskPixelFormat = PixelFormat.Format8bppIndexed;

            if (m_maskBitsPerComponent == 1)
            {
                maskPixelFormat = PixelFormat.Format1bppIndexed;

                byte[] imagedata = mask.GetBuffer();

                Bitmap grayImage = new Bitmap((int)m_maskWidth, (int)m_maskHeight, maskPixelFormat);
                BitmapData grayImageData = grayImage.LockBits(new Rectangle(0, 0, (int)m_maskWidth, (int)m_maskHeight), ImageLockMode.ReadWrite, grayImage.PixelFormat);

                int rows = (int)m_maskHeight;
                int stride = grayImageData.Stride;
                IntPtr portionOffset = grayImageData.Scan0;
                int bpp = Image.GetPixelFormatSize(grayImage.PixelFormat);
                int rowWidth = (int)m_maskWidth * bpp / 8;

                if (((int)m_maskWidth * bpp % 8) != 0)
                    ++rowWidth;

                int offset = 0;
                long ptr = grayImageData.Scan0.ToInt64();
                for (int i = 0; i < m_maskHeight; i++)
                {
                    Marshal.Copy(imagedata, offset, new IntPtr(ptr), (int)(rowWidth));
                    offset += (int)(rowWidth);
                    ptr += grayImageData.Stride;
                }

                grayImage.UnlockBits(grayImageData);
                return grayImage;
            }

            byte[] outArray = mask.GetBuffer();

            Bitmap grayscaleImage = new Bitmap((int)m_maskWidth, (int)m_maskHeight, maskPixelFormat);
            ColorPalette grayscalePalette = grayscaleImage.Palette;
            for (int i = 0; i < grayscalePalette.Entries.Length; i++)
                grayscalePalette.Entries[i] = Color.FromArgb(i, i, i);

            Bitmap maskImage = new Bitmap((int)m_maskWidth, (int)m_maskHeight, maskPixelFormat);
            BitmapData maskData = maskImage.LockBits(new Rectangle(0, 0, (int)m_maskWidth, (int)m_maskHeight), ImageLockMode.ReadWrite, maskPixelFormat);

            int length = Math.Abs(maskData.Stride) * maskImage.Height;
            if (maskPixelFormat == PixelFormat.Format8bppIndexed)
            {
                int offset = 0;
                long imagePointer = maskData.Scan0.ToInt64();
                for (int i = 0; i < m_maskHeight; i++)
                {
                    Marshal.Copy(outArray, offset, new IntPtr(imagePointer), (int)(m_maskWidth));
                    offset += (int)(m_maskWidth);
                    imagePointer += maskData.Stride;
                }
            }
            else
                Marshal.Copy(outArray, 0, maskData.Scan0, outArray.Length);

            maskImage.Palette = grayscalePalette;
            maskImage.UnlockBits(maskData);

            return maskImage;
        }

        private MemoryStream DecodeDeviceGrayImage(MemoryStream imageStr)
        {
            byte[] imagedata = imageStr.GetBuffer();

            Bitmap grayImage = new Bitmap((int)Width, (int)Height, PixelFormat.Format32bppArgb);
            int num =(int) BitsPerComponent;
            byte[] buffer = new byte[imageStr.Length];
            for (int i = 0; i < (int)Height; i++)
            {
                int num2 = 0;
                int num3 = 0;
                for (int j = 0; j < (int)Width; j++)
                {
                    if (num3 < num)
                    {
                        num2 <<= 8;
                        num2 |= imageStr.ReadByte();
                        num3 += 8;
                    }
                    byte b = (byte)(num2 >> num3 - num);
                    num2 ^= (int)b << num3 - num;
                    num3 -= num;
                    Color color = Color.FromArgb(255, b, b, b);
                    grayImage.SetPixel(j, i, color);
                }
            }
            MemoryStream decodedStream = new MemoryStream();
            grayImage.Save(decodedStream, ImageFormat.Jpeg);

            return decodedStream;
        }

        /// <summary>
        /// Decodes the stream based on the predictor values
        /// </summary>
        /// <param name="predictor">Predictor from the decode params</param>
        /// <param name="colors">Colors from the decode params</param>
        /// <param name="columns">Columns from the decode params</param>
        /// <param name="data">Stream the encoded image</param>
        /// <returns>Decoded stream</returns>
        private MemoryStream DecodePredictor(int predictor, int colors, int columns, MemoryStream data)
        {
            MemoryStream outputStream = new MemoryStream();
            byte[] buffer = new byte[2048];
            if (predictor == 1)
            {
                outputStream = data as MemoryStream;
            }
            else
            {
                // calculate sizes
                int bitsPerPixel = (int)((colors * BitsPerComponent + 7) / 8);
                int rowLength = (int)((columns * colors * BitsPerComponent + 7) / 8 + bitsPerPixel);
                byte[] imageRow = new byte[rowLength];
                byte[] lastRow = new byte[rowLength];
                int linePredictor = predictor;
                bool isCompleted = false;
                data.Position = 0;
                while (!isCompleted && (data.Position < data.Length))
                {
                    if (predictor >= 10)
                    {
                        byte[] lineHeader = new byte[1];
                        data.Read(lineHeader, 0, 1);
                        linePredictor = (int)lineHeader[0];
                        if (linePredictor == -1)
                        {
                            isCompleted = true;
                            break;
                        }
                        else
                        {
                            linePredictor += 10;
                        }
                    }

                    int i = 0;
                    int offset = bitsPerPixel;

                    while (offset < rowLength && ((i = data.Read(imageRow, offset, rowLength - offset)) != -1))
                    {
                        offset += i;
                    }

                    switch (linePredictor)
                    {
                        case 2:
                            for (int pixelPosition = bitsPerPixel; pixelPosition < rowLength; pixelPosition++)
                            {
                                int sub = imageRow[pixelPosition] & 0xff;
                                int left = imageRow[pixelPosition - bitsPerPixel] & 0xff;
                                imageRow[pixelPosition] = (byte)(sub + left);
                            }
                            break;
                        case 10:
                            break;
                        case 11:
                            for (int pixekPosition = bitsPerPixel; pixekPosition < rowLength; pixekPosition++)
                            {
                                int sub = imageRow[pixekPosition] & 0xff;
                                int left = imageRow[pixekPosition - bitsPerPixel] & 0xff;
                                imageRow[pixekPosition] = (byte)(sub + left);
                            }
                            break;
                        case 12:
                            for (int pixelPosition = bitsPerPixel; pixelPosition < rowLength; pixelPosition++)
                            {
                                int up = imageRow[pixelPosition] & 0xff;
                                int prior = lastRow[pixelPosition] & 0xff;
                                imageRow[pixelPosition] = (byte)(up + prior);
                            }
                            break;
                        case 13:
                            for (int pixelPosition = bitsPerPixel; pixelPosition < rowLength; pixelPosition++)
                            {
                                int avg = imageRow[pixelPosition] & 0xff;
                                int left = imageRow[pixelPosition - bitsPerPixel] & 0xff;
                                int up = lastRow[pixelPosition] & 0xff;
                                imageRow[pixelPosition] = (byte)(avg + ((left + up) / 2));
                            }
                            break;
                        case 14:
                            for (int pixelPosition = bitsPerPixel; pixelPosition < rowLength; pixelPosition++)
                            {
                                int paeth = imageRow[pixelPosition] & 0xff;
                                int left = imageRow[pixelPosition - bitsPerPixel] & 0xff;
                                int above = lastRow[pixelPosition] & 0xff;
                                int upperLeft = lastRow[pixelPosition - bitsPerPixel] & 0xff;
                                int value = left + above - upperLeft;
                                int absLeft = Math.Abs(value - left);
                                int absAbove = Math.Abs(value - above);
                                int absUpperLeft = Math.Abs(value - upperLeft);

                                if (absLeft <= absAbove && absLeft <= absUpperLeft)
                                {
                                    imageRow[pixelPosition] = (byte)(paeth + left);
                                }
                                else if (absAbove <= absUpperLeft)
                                {
                                    imageRow[pixelPosition] = (byte)(paeth + above);
                                }
                                else
                                {
                                    imageRow[pixelPosition] = (byte)(paeth + upperLeft);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    lastRow = (byte[])imageRow.Clone();
                    outputStream.Write(imageRow, bitsPerPixel, imageRow.Length - bitsPerPixel);
                }
            }
            return outputStream;
        }

        public Image Decode()
        {
            Image embeddedImage = null;
            try
            {

                ImagePreRenderEventArgs args = new ImagePreRenderEventArgs(this);

                if (ImagePreRender != null)
                {
                    ImagePreRender(null, args);
                    embeddedImage = Image.FromStream(args.ImageStream);
                }
                else
                {
                    embeddedImage = Image.FromStream(GetImageStream());
                }
            }
            catch (Exception)
            {
                embeddedImage = null;
            }

            return embeddedImage;
        }

        /// <summary>
        /// Converts YCCK colorspace to RGB colorspace
        /// </summary>
        /// <param name="encodedData">Byte array of YCCK image</param>
        /// <returns>Byte array of RGB image</returns>
        private byte[] YCCKtoRGB(byte[] encodedData)
        {
            byte[] decodedData = new byte[encodedData.Length];
            int pixelCount = (int)Width * (int)Height * 4;
            double Y, Cr, Cb, R, G, B;

            int pixelReached = 0;
            for (int i = 0; i + 3 < encodedData.Length; i += 3)
            {
                Y = (encodedData[i] & 0xff);
                Cr = (encodedData[i + 1] & 0xff);
                Cb = (encodedData[i + 2] & 0xff);

                R = 255 - Y;
                G = 255 - Cr;
                B = 255 - Cb;

                decodedData[pixelReached++] = (byte)R;
                decodedData[pixelReached++] = (byte)G;
                decodedData[pixelReached++] = (byte)B;
            }

            return decodedData;
        }

        /// <summary>
        /// Converts YCC colorspace image to RGB image
        /// </summary>
        /// <param name="encodedData">Byte array of YCC image</param>
        /// <returns>Byte array of RGB image</returns>
        private byte[] YCCToRGB(byte[] encodedData)
        {
            byte[] decodedData = new byte[(int)Width * (int)Height * 3];

            int pixelCount = (int)Width * (int)Height * 4;

            double finalC = -1, finalM = -1.12, finalY = -1.12, finalK = -1.21;
            double x = 255;

            double c, m, y, k = 1;
            int pointer = 0;

            for (int i = 0; i < pixelCount; i = i + 4)
            {
                if (i > encodedData.Length)
                    break;
                double inCyan = (encodedData[i] & 0xff) / x;
                double inMagenta = (encodedData[i + 1] & 0xff) / x;
                double inYellow = (encodedData[i + 2] & 0xff) / x;
                double inBlack = (encodedData[i + 3] & 0xff) / x;
                double r=0, g=0, b=0;
                if ((finalC != inCyan) || (finalM != inMagenta) || (finalY != inYellow) || (finalK != inBlack))
                {


                    c = inCyan;
                    m = inMagenta;
                    y = inYellow;
                    k = inBlack;

                    r = 255 * (1 - c) * (1 - k);
                    g = 255 * (1 - m) * (1 - k);
                    b = 255 * (1 - y) * (1 - k);
                }

                decodedData[pointer++] = (byte)(r);
                decodedData[pointer++] = (byte)(g);
                decodedData[pointer++] = (byte)(b);
            }
            return decodedData;
        }

        private double RoundOff(double value)
        {

            if (value < 0)
                value = 0;

            if (value > 1)
                value = 1;

            return value;
        }

        private string[] GetImageFilter()
        {
            string[] imageFilter = null;

            if (m_imageDictionary != null)
            {
                if (m_imageDictionary.ContainsKey("Filter"))
                {
                    if (m_imageDictionary[DictionaryProperties.Filter] is PdfName)
                    {
                        imageFilter = new string[1];
                        imageFilter[0] = (m_imageDictionary[DictionaryProperties.Filter] as PdfName).Value;
                    }
                    else if (m_imageDictionary[DictionaryProperties.Filter] is PdfArray)
                    {
                        PdfArray filters = m_imageDictionary[DictionaryProperties.Filter] as PdfArray;
                        imageFilter = new string[filters.Count];
                        for (int i = 0; i < filters.Count; i++)
                        {
                            imageFilter[i] = (filters[i] as PdfName).Value;
                        }
                    }
                    else if (m_imageDictionary[DictionaryProperties.Filter] is PdfReferenceHolder)
                    {
                        PdfArray filters = (m_imageDictionary[DictionaryProperties.Filter] as PdfReferenceHolder).Object as PdfArray;
                        imageFilter = new string[filters.Count];
                        for (int i = 0; i < filters.Count; i++)
                        {
                            imageFilter[i] = (filters[i] as PdfName).Value;
                        }
                    }
                }
            }

            return imageFilter;
        }

        private PdfDictionary[] GetDecodeParam()
        {
            PdfDictionary[] decodeParam = null;

            if (m_imageDictionary != null)
            {
                if (m_imageDictionary.ContainsKey(DictionaryProperties.DecodeParms))
                {
                    if (m_imageDictionary[DictionaryProperties.DecodeParms] is PdfDictionary)
                    {
                        decodeParam = new PdfDictionary[1];
                        decodeParam[0] = (m_imageDictionary[DictionaryProperties.DecodeParms] as PdfDictionary);
                    }
                    else if (m_imageDictionary[DictionaryProperties.DecodeParms] is PdfArray)
                    {
                        PdfArray decodeParams = m_imageDictionary[DictionaryProperties.DecodeParms] as PdfArray;
                        decodeParam = new PdfDictionary[decodeParams.Count];
                        for (int i = 0; i < decodeParams.Count; i++)
                        {
                            decodeParam[i] = (decodeParams[i] as PdfDictionary);
                        }
                    }
                }
            }

            return decodeParam;
        }


        private float GetImageHeight()
        {
            float height = 0;

            if (m_imageDictionary != null)
            {
                if (m_imageDictionary.ContainsKey("Height"))
                {
                    if (m_imageDictionary[DictionaryProperties.Height] is PdfReferenceHolder)
                    {
                        height = ((m_imageDictionary[DictionaryProperties.Height] as PdfReferenceHolder).Object as PdfNumber).FloatValue;
                        return height;
                    }
                    height = (m_imageDictionary[DictionaryProperties.Height] as PdfNumber).FloatValue;
                }
            }
            return height;
        }

        private float GetBitsPerComponent()
        {
            float bitsPerComponent = 0;

            if (m_imageDictionary != null)
                bitsPerComponent = (m_imageDictionary[DictionaryProperties.BitsPerComponent] as PdfNumber).FloatValue;
            return bitsPerComponent;
        }

        private float GetImageWidth()
        {
            float width = 0;

            if (m_imageDictionary != null)
            {
                if (m_imageDictionary.ContainsKey("Width"))
                {
                    width = (m_imageDictionary[DictionaryProperties.Width] as PdfNumber).FloatValue;
                }
            }
            return width;
        }

    }

    /// <summary>
    /// Custom event argument class used to notify before image is rendered in the viewer.
    /// </summary>
    public class ImagePreRenderEventArgs : EventArgs
    {
        internal Stream m_imageStream;
        internal float m_height;
        internal float m_width;
        internal string[] m_filter;

        /// <summary>
        /// Gets or sets the content stream of the image.
        /// </summary>
        public Stream ImageStream
        {
            get
            {
                return m_imageStream;
            }
            set
            {
                m_imageStream = value;
            }
        }

        /// <summary>
        /// Gets or sets height of the image.
        /// </summary>
        public float Height
        {
            get
            {
                return m_height;
            }
            set
            {
                m_height = value;
            }
        }

        /// <summary>
        /// Gets or sets width of the image.
        /// </summary>
        public float Width
        {
            get
            {
                return m_width;
            }
            set
            {
                m_width = value;
            }
        }

        /// <summary>
        /// Returns filter names used in the image.
        /// </summary>
        public string[] Filter
        {
            get
            {
                return m_filter;
            }
        }
        internal ImagePreRenderEventArgs(ImageStructure structure)
        {
            m_imageStream = structure.ImageStream;
            m_height = structure.Height;
            m_width = structure.Width;
            m_filter = structure.ImageFilter;
        }
    }
}
