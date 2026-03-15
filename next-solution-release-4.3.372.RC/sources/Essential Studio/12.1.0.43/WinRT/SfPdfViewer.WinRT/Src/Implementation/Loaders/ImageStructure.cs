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
//using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Syncfusion.Pdf.Graphics;
using System.IO.Compression;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Syncfusion.DirectXWrapper.WinRT;
using System.Threading.Tasks;

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
        private float m_maskBitsPerComponent;
        private float m_bitsPerComponent;
        private Image m_embeddedImage;
        private bool m_isImageStreamParsed = false;
        private PdfMatrix m_imageInfo;
        private string m_colorspace;
        private string m_colorspaceBase;
        private int m_colorspaceHival;
        private Guid m_pixelFormat = PixelFormat.Format24bppRGB;
        private Palette m_colorPalette;
        private MemoryStream m_colorspaceStream;
        internal StringBuilder exceptions = new StringBuilder();
        internal bool IsTransparent;
        internal bool IsAlternateDeviceRGB;
        internal bool IsExceptionThrown = false;
        private Dictionary<string, MemoryStream> colorSpaceResourceDict = new Dictionary<string, MemoryStream>();
        private bool isIndexedImage = false;
        private bool IsDeviceNColorSpace = false;
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

        ~ImageStructure()
        {
            if (m_colorPalette != null)
                m_colorPalette.Dispose();
            if (m_colorspaceStream != null)
                m_colorspaceStream.Dispose();
            if (m_embeddedImage != null)
                m_embeddedImage = null;
            if (m_imageStream != null)
                m_imageStream.Dispose();
            if (m_maskStream != null)
                m_maskStream.Dispose();

            m_colorPalette = null;
            m_colorspaceStream = null;
            m_imageStream = null;
            m_decodeParam = null;
            m_imageDictionary = null;
            m_maskStream = null;
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
                        //m_embeddedImage = Image.FromStream(stream);
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
                    m_imageStream = new MemoryStream(imageStream.Data);
                    m_imageStream.Position = 0;
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
                    m_maskStream = new MemoryStream(maskStream.Data);
                    PdfDictionary maskDictionary = (m_imageDictionary[DictionaryProperties.SMask] as PdfReferenceHolder).Object as PdfDictionary;

                    if (maskDictionary.ContainsKey(DictionaryProperties.Width))
                        m_maskWidth = (maskDictionary[DictionaryProperties.Width] as PdfNumber).IntValue;
                    if (maskDictionary.ContainsKey(DictionaryProperties.Height))
                        m_maskHeight = (maskDictionary[DictionaryProperties.Height] as PdfNumber).IntValue;
                    if (maskDictionary.ContainsKey(DictionaryProperties.BitsPerComponent))
                        m_maskBitsPerComponent = (maskDictionary[DictionaryProperties.BitsPerComponent] as PdfNumber).IntValue;
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

        public MemoryRandomAccessStream GetImageStream(bool IsWinrt)
        {
            ImageStream.Position = 0;
            MemoryRandomAccessStream imageStream = new MemoryRandomAccessStream(ImageStream);
            return imageStream;
        }

        public byte[] convertIndexedDeviceNToRGB(byte[] data)
        {
            int componentCount=3;
            byte[] newdata = new byte[3 * 256];
            try
            {
                int outputReached = 0;
                float[] opValues = new float[1];
                int byteCount = data.Length;
                float[] values = new float[componentCount];
                for (int i = 0; i < byteCount; i = i + componentCount)
                {
                    float r = 0, g = 0, b = 0;

                    for (int j = 0; j < componentCount; j++)
                        values[j] = (data[i + j] & 255) / 255f;
                    float c = 0, m = 0, y = 0, k = 0;
                    float.TryParse(values[0].ToString(), out c);
                    float.TryParse(values[1].ToString(), out m);
                    float.TryParse(values[2].ToString(), out y);
                    r = 255 * (1 - c) * (1 - k);
                    g = 255 * (1 - m) * (1 - k);
                    b = 255 * (1 - y) * (1 - k);

                    newdata[outputReached] = (byte)r;
                    outputReached++;
                    newdata[outputReached] = (byte)g;
                    outputReached++;
                    newdata[outputReached] = (byte)b;
                    outputReached++;
                }
            }
            catch (Exception)
            {
            }
            return newdata;
        }

        public Stream GetImageStream()
        {
            bool IsDecodeFilterDefined = true;
            if (m_isImageStreamParsed)
            {
                if (IsExceptionThrown)
                    return null;
                return ImageStream;
            }
            m_isImageStreamParsed = true;
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
                                byte[] decodedBytes = decoder.Decode((ImageStream as MemoryStream).ToArray());

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

                                //Bitmap embeddedImage = new Bitmap((int)Width, (int)Height, m_pixelFormat);
                                //BitmapData imageData = embeddedImage.LockBits(new Rectangle(0, 0, embeddedImage.Width, embeddedImage.Height), ImageLockMode.ReadWrite, embeddedImage.PixelFormat);

                                //if (m_pixelFormat == PixelFormat.Format8bppIndexed)
                                //    embeddedImage.Palette = m_colorPalette;

                                //int bitsPerPixel = Image.GetPixelFormatSize(embeddedImage.PixelFormat);
                                //int pixelSize = bitsPerPixel / 8;
                                var factory = new ImageFactory();

                                ImageStream stream = null;
                                IRandomAccessStream wicImageStream = new InMemoryRandomAccessStream();
                                stream = factory.CreateStream(wicImageStream);

                                // Initialize a Jpeg encoder with this stream
                                var encoder = factory.CreatePngEncoder();
                                encoder.Initialize(stream);

                                // Create a Frame encoder
                                var bitmapFrameEncode = encoder.CreateBitmapFrameEncode();

                                //bitmapFrameEncode.Options.ImageQuality = 0.8f;
                                bitmapFrameEncode.Initialize();

                                bitmapFrameEncode.SetSize((int)Width, (int)Height);


                                if (m_colorPalette != null)
                                    bitmapFrameEncode.Palette = m_colorPalette;
                                Guid pFormat = PixelFormat.Format32bppBGRA;
                                bitmapFrameEncode.SetPixelFormat(m_pixelFormat);

                                int bitsPerPixel = PixelFormat.GetBitsPerPixel(m_pixelFormat);
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
                                int stride = PixelFormat.GetStride(m_pixelFormat, (int)Width);
                                var bufferSize = (int)Height * stride;
                                int mask = 15;
                                var ptrFinal = Marshal.AllocHGlobal(bufferSize + mask + IntPtr.Size);
                                var imageBuffer = new DataPointer();
                                imageBuffer.Size = bufferSize;
                                imageBuffer.Pointer = ptrFinal.ToInt64();
                                length = Math.Abs(stride) * (int)Height;
                                if (length < imgeBytes.Length)
                                {
                                    int offset = 0;
                                    long imagePointer = imageBuffer.Pointer;
                                    int w = (int)Width;
                                    if (pixelSize == 3)
                                        w = (int)Width * 3;

                                    for (int i = 0; i < Height; i++)
                                    {
                                        Marshal.Copy(imgeBytes, offset, new IntPtr(imagePointer), w);
                                        offset += w;
                                        imagePointer += stride;
                                    }
                                }
                                else
                                    Marshal.Copy(imgeBytes, 0, ptrFinal, imgeBytes.Length);
                                //imageBuffer.Write(imgeBytes, 0, imgeBytes.Length);
                                // Copy the pixels from the buffer to the Wic Bitmap Frame encoder
                                bitmapFrameEncode.WritePixels((int)Height, stride, imageBuffer);

                                // Commit changes
                                bitmapFrameEncode.Commit();
                                encoder.Commit();
                                bitmapFrameEncode.Dispose();
                                encoder.Dispose();
                                stream.Dispose();
                                if (!m_imageDictionary.ContainsKey(DictionaryProperties.SMask))
                                {
                                    ImageStream = wicImageStream.AsStream();
                                    ImageStream.Position = 0;
                                }
                                else
                                {
                                    //try
                                    //{
                                    //    ImageStream = (MergeImages(embeddedImage, MaskStream as MemoryStream));
                                    //    ImageStream.Position = 0;
                                    //}
                                    //catch (Exception exec)
                                    //{
                                    ImageStream = wicImageStream.AsStream();
                                    ImageStream.Position = 0;
                                    //}
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
                                    MemoryRandomAccessStream imageStream = new MemoryRandomAccessStream(ImageStream);
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
                                        var factory = new ImageFactory();
                                        var formatConverter = factory.CreateFormatConverter();
                                        int bmpWidth = 0;
                                        int bmpHeight = 0;
                                        if (m_imageDictionary.ContainsKey(DictionaryProperties.Width) && m_imageDictionary.ContainsKey(DictionaryProperties.Height))
                                        {
                                            bmpWidth = (m_imageDictionary[DictionaryProperties.Width] as PdfNumber).IntValue;
                                            bmpHeight = (m_imageDictionary[DictionaryProperties.Height] as PdfNumber).IntValue;
                                        }
                                        //WriteableBitmap writeableBmp = new WriteableBitmap(bmpWidth, bmpHeight);
                                        //MemoryRandomAccessStream msBmp = new MemoryRandomAccessStream(ImageStream);
                                        //writeableBmp.SetSource(msBmp);

                                        //byte[] rgbValues = writeableBmp.PixelBuffer.ToArray();
                                        var bitmapDecoder = factory.CreateBitmapDecoder(ConvertStream(ImageStream).Result, DecodeOptions.CacheOnDemand);
                                        formatConverter.Initialize(
                                    bitmapDecoder.GetFrame(0),
                                    PixelFormat.Format32bppCMYK,
                                    BitmapDitherType.None,
                                    null,
                                    0.0,
                                    BitmapPaletteType.Custom);
                                        WicBitmap inputImage = factory.CreateBitmap(formatConverter, BitmapCreateCacheOption.CacheOnLoad);
                                        BitmapLock inputImageData = inputImage.Lock(new global::Windows.Foundation.Rect(0, 0, inputImage.Size.Width, inputImage.Size.Height), BitmapLockFlags.Read);
                                        int bytes = (int)(inputImageData.Stride * inputImageData.Size.Height);

                                        int size;
                                        DataPointer inputPointer = inputImageData.GetDataPointer(out size);
                                        byte[] rgbValues = new byte[bytes];
                                        Marshal.Copy(new IntPtr(inputPointer.Pointer), rgbValues, 0, bytes);
                                        byte[] decoded = YCCKtoRGB(rgbValues);

                                        ImageStream stream = null;
                                        IRandomAccessStream wicImageStream = new InMemoryRandomAccessStream();
                                        stream = factory.CreateStream(wicImageStream);

                                        var encoder = factory.CreateJpegEncoder();
                                        encoder.Initialize(stream);

                                        var bitmapFrameEncode = encoder.CreateBitmapFrameEncode();

                                        bitmapFrameEncode.Initialize();

                                        bitmapFrameEncode.SetSize((int)bmpWidth, (int)bmpHeight);


                                        Guid pFormat = PixelFormat.Format32bppCMYK;
                                        bitmapFrameEncode.SetPixelFormat(pFormat);

                                        int bitsPerPixel = PixelFormat.GetBitsPerPixel(pFormat);
                                        int pixelSize = bitsPerPixel / 8;
                                        int stride = PixelFormat.GetStride(pFormat, (int)bmpWidth);
                                        var bufferSize = (int)bmpHeight * stride;

                                        bitmapFrameEncode.WritePixels((int)bmpHeight, stride, bufferSize, decoded);
                                        bitmapFrameEncode.Commit();
                                        encoder.Commit();
                                        stream.Commit();
                                        bitmapFrameEncode.Dispose();
                                        encoder.Dispose();
                                        stream.Dispose();
                                        factory.Dispose();
                                        inputImageData.Dispose();
                                        inputImage.Dispose();
                                        bitmapDecoder.Dispose();
                                        ImageStream = wicImageStream.AsStream();
                                        ImageStream.Position = 0;
                                    }
                                    //return imageStream;
                                }
                                else
                                {
                                    try
                                    {
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
                                            var factory = new ImageFactory();
                                            var formatConverter = factory.CreateFormatConverter();
                                            int bmpWidth = 0;
                                            int bmpHeight = 0;
                                            if (m_imageDictionary.ContainsKey(DictionaryProperties.Width) && m_imageDictionary.ContainsKey(DictionaryProperties.Height))
                                            {
                                                bmpWidth = (m_imageDictionary[DictionaryProperties.Width] as PdfNumber).IntValue;
                                                bmpHeight = (m_imageDictionary[DictionaryProperties.Height] as PdfNumber).IntValue;
                                            }
                                            //WriteableBitmap writeableBmp = new WriteableBitmap(bmpWidth, bmpHeight);
                                            //MemoryRandomAccessStream msBmp = new MemoryRandomAccessStream(ImageStream);
                                            //writeableBmp.SetSource(msBmp);

                                            //byte[] rgbValues = writeableBmp.PixelBuffer.ToArray();
                                            var bitmapDecoder = factory.CreateBitmapDecoder(ConvertStream(ImageStream).Result, DecodeOptions.CacheOnDemand);
                                            formatConverter.Initialize(
                                        bitmapDecoder.GetFrame(0),
                                        PixelFormat.Format32bppCMYK,
                                        BitmapDitherType.None,
                                        null,
                                        0.0,
                                        BitmapPaletteType.Custom);
                                            WicBitmap inputImage = factory.CreateBitmap(formatConverter, BitmapCreateCacheOption.CacheOnLoad);
                                            BitmapLock inputImageData = inputImage.Lock(new global::Windows.Foundation.Rect(0, 0, inputImage.Size.Width, inputImage.Size.Height), BitmapLockFlags.Read);
                                            int bytes = (int)(inputImageData.Stride * inputImageData.Size.Height);

                                            int size;
                                            DataPointer inputPointer = inputImageData.GetDataPointer(out size);
                                            byte[] rgbValues = new byte[bytes];
                                            Marshal.Copy(new IntPtr(inputPointer.Pointer), rgbValues, 0, bytes);
                                            byte[] decoded = YCCKtoRGB(rgbValues);

                                            ImageStream stream = null;
                                            IRandomAccessStream wicImageStream = new InMemoryRandomAccessStream();
                                            stream = factory.CreateStream(wicImageStream);

                                            var encoder = factory.CreateJpegEncoder();
                                            encoder.Initialize(stream);

                                            var bitmapFrameEncode = encoder.CreateBitmapFrameEncode();

                                            bitmapFrameEncode.Initialize();

                                            bitmapFrameEncode.SetSize((int)bmpWidth, (int)bmpHeight);


                                            Guid pFormat = PixelFormat.Format32bppCMYK;
                                            bitmapFrameEncode.SetPixelFormat(pFormat);

                                            int bitsPerPixel = PixelFormat.GetBitsPerPixel(pFormat);
                                            int pixelSize = bitsPerPixel / 8;
                                            int stride = PixelFormat.GetStride(pFormat, (int)bmpWidth);
                                            var bufferSize = (int)bmpHeight * stride;

                                            bitmapFrameEncode.WritePixels((int)bmpHeight, stride, bufferSize, decoded);
                                            bitmapFrameEncode.Commit();
                                            encoder.Commit();
                                            stream.Commit();
                                            bitmapFrameEncode.Dispose();
                                            encoder.Dispose();
                                            stream.Dispose();
                                            factory.Dispose();
                                            inputImageData.Dispose();
                                            inputImage.Dispose();
                                            bitmapDecoder.Dispose();
                                            ImageStream = wicImageStream.AsStream();
                                            ImageStream.Position = 0;
                                        }
                                        IRandomAccessStream imageStream = ConvertStream(ImageStream).Result;
                                        if (imageStream.Size > 2)
                                        {
                                            try
                                            {
                                                imageStream.Seek(0);
                                                var factory = new ImageFactory();
                                                var formatConverter = factory.CreateFormatConverter();
                                                var bitmapDecoder = factory.CreateBitmapDecoder(imageStream, DecodeOptions.CacheOnDemand);
                                                formatConverter.Initialize(
                                            bitmapDecoder.GetFrame(0),
                                            PixelFormat.Format32bppBGR,
                                            BitmapDitherType.None,
                                            null,
                                            0.0,
                                            BitmapPaletteType.Custom);
                                                ImageStream.Position = 0;
                                            }
                                            catch
                                            {
                                                ImageStream = new MemoryStream();
                                            }
                                        }
                                        else
                                        {
                                            ImageStream = new MemoryStream();
                                            imageStream = ConvertStream(ImageStream).Result;
                                            imageStream.Seek(0);
                                        }
                                        IRandomAccessStream mask = ConvertStream(MaskStream).Result;
                                        ImageStream = MergeImages(imageStream, mask).AsStream();
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

                                byte[] bytes = outStream.ToArray();
                                object temp = ColorSpace;
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
                                    if (IsDeviceNColorSpace)
                                    {
                                        byte[] convertedIndexBytes = convertIndexedDeviceNToRGB(colorSpaceResourceDict["Indexed"].ToArray());
                                        imgeBytes = ConvertIndexedStreamToFlat(depth, width, height, outStream.ToArray(), convertedIndexBytes, false, false);
                                    }
                                    else
                                        imgeBytes = ConvertIndexedStreamToFlat(depth, width, height, outStream.ToArray(), colorSpaceResourceDict["Indexed"].ToArray(), false, false);
                                }

                                if (ColorSpace == "DeviceGray")
                                {
                                    ImageStream = DecodeDeviceGrayImage(bytes).AsStream();
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

                                //byte[] imgeBytes = null;
                                if (!isIndexedImage)
                                {
                                    if (ColorSpace == "DeviceCMYK")
                                    {
                                        imgeBytes = YCCToRGB(outStream.ToArray());
                                        outStream = new MemoryStream(imgeBytes);
                                    }
                                    else
                                    {
                                        imgeBytes = (outStream as MemoryStream).ToArray();
                                    }
                                }
                                if (ImageDictionary.ContainsKey(DictionaryProperties.Mask))
                                {
                                    IsTransparent = true;
                                    if (ColorSpace == "Indexed")
                                        return null;
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
                                //Lock bits
                                var factory = new ImageFactory();

                                ImageStream stream = null;

                                IRandomAccessStream ms = new InMemoryRandomAccessStream();

                                stream = factory.CreateStream(ms);

                                // Initialize a Jpeg encoder with this stream
                                var encoder = factory.CreatePngEncoder();
                                encoder.Initialize(stream);

                                // Create a Frame encoder
                                var bitmapFrameEncode = encoder.CreateBitmapFrameEncode();

                                //bitmapFrameEncode.Options.ImageQuality = 0.8f;
                                bitmapFrameEncode.Initialize();

                                bitmapFrameEncode.SetSize((int)Width, (int)Height);


                                if (m_colorPalette != null)
                                    bitmapFrameEncode.Palette = m_colorPalette;
                                Guid pFormat = PixelFormat.Format32bppBGRA;
                                bitmapFrameEncode.SetPixelFormat(m_pixelFormat);

                                int bitsPerPixel = PixelFormat.GetBitsPerPixel(m_pixelFormat);
                                int pixelSize = bitsPerPixel / 8;

                                if (m_imageDictionary.ContainsKey(DictionaryProperties.DecodeParms))
                                {
                                    decodeParams = DecodeParam[k];
                                    if (decodeParams != null && decodeParams.Items.Count > 0)
                                    {
                                        if (decodeParams.ContainsKey(DictionaryProperties.Predictor))
                                            predictor = (decodeParams[DictionaryProperties.Predictor] as PdfNumber).IntValue;
                                        if (decodeParams.ContainsKey(DictionaryProperties.Columns))
                                            columns = (decodeParams[DictionaryProperties.Columns] as PdfNumber).IntValue;
                                        if (decodeParams.ContainsKey(DictionaryProperties.Colors))
                                            colors = (decodeParams[DictionaryProperties.Colors] as PdfNumber).IntValue;

                                        MemoryStream decodedStream = DecodePredictor(predictor, colors, columns, outStream);
                                        imgeBytes = decodedStream.ToArray();
                                    }
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


                                int w = (int)Width;
                                if (pixelSize == 3)
                                    w = (int)Width * 3;


                                int stride = PixelFormat.GetStride(m_pixelFormat, (int)Width);
                                var bufferSize = (int)Height * stride;
                                var buffer = new MemoryStream();
                                if (IsTransparent)
                                {
                                    stride = PixelFormat.GetStride(pFormat, (int)Width);
                                    bufferSize = (int)Height * stride;
                                    buffer = new MemoryStream();
                                    bitmapFrameEncode.SetPixelFormat(pFormat);
                                    byte[] transparentImage = new byte[bufferSize];
                                    int j = 0;
                                    int i = 0;

                                    for (i = 0; i < imgeBytes.Length; i = i += pixelSize)
                                    {
                                        if (m_colorspace == "Indexed")
                                        {
                                            int indexcolorValue = imgeBytes[i];
                                            byte[] paletteColor = ConvertToByte(m_colorPalette.Colors[indexcolorValue]);//

                                            if (paletteColor[0] == 255 && paletteColor[1] == 255 && paletteColor[2] == 255 && paletteColor[3] == 255)
                                            {
                                                transparentImage[j++] = paletteColor[3];
                                                transparentImage[j++] = paletteColor[2];
                                                transparentImage[j++] = paletteColor[1];
                                                transparentImage[j++] = 0;
                                            }
                                            else
                                            {
                                                transparentImage[j++] = paletteColor[1];
                                                transparentImage[j++] = paletteColor[2];
                                                transparentImage[j++] = paletteColor[3];
                                                transparentImage[j++] = paletteColor[0];
                                            }
                                        }
                                        else
                                        {
                                            if (imgeBytes[i] == 0 && imgeBytes[i + 1] == 0 && imgeBytes[i + 2] == 0)
                                            {
                                                transparentImage[j++] = 255;
                                                transparentImage[j++] = 255;
                                                transparentImage[j++] = 255;
                                                transparentImage[j++] = 0;
                                            }
                                            else
                                            {

                                                transparentImage[j++] = imgeBytes[i];
                                                transparentImage[j++] = imgeBytes[i + 1];
                                                transparentImage[j++] = imgeBytes[i + 2];
                                                transparentImage[j++] = 255;
                                            }
                                        }
                                    }

                                    buffer.Write(transparentImage, 0, transparentImage.Length);
                                    bitmapFrameEncode.WritePixels((int)Height, stride, bufferSize, buffer.ToArray());
                                }
                                else
                                {
                                    int offset = 0;
                                    int mask = 15;
                                    var ptrFinal = Marshal.AllocHGlobal(bufferSize + mask + IntPtr.Size);
                                    var ptrBuffer = new DataPointer();
                                    ptrBuffer.Size = bufferSize;
                                    ptrBuffer.Pointer = ptrFinal.ToInt64();
                                    long imagePointer = ptrBuffer.Pointer;

                                    for (int i = 0; i < Height; i++)
                                    {
                                        Marshal.Copy(imgeBytes, offset, new IntPtr(imagePointer), w);
                                        offset += w;
                                        imagePointer += stride;
                                    }

                                    bitmapFrameEncode.WritePixels((int)Height, stride, ptrBuffer);
                                }

                                bitmapFrameEncode.Commit();
                                encoder.Commit();
                                stream.Commit();
                                bitmapFrameEncode.Dispose();
                                encoder.Dispose();
                                stream.Dispose();
                                factory.Dispose();

                                if (!m_imageDictionary.ContainsKey(DictionaryProperties.SMask))
                                {
                                    ImageStream = ms.AsStream();
                                    ImageStream.Position = 0;
                                }
                                else
                                {
                                    try
                                    {
                                        IRandomAccessStream randomAccessStream = ConvertStream(MaskStream).Result;

                                        ImageStream = MergeImages(ms, randomAccessStream).AsStream();
                                        ImageStream.Position = 0;
                                    }
                                    catch (Exception)
                                    {
                                        ImageStream = ms.AsStream();
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
                                tiff.m_stream.Write((ImageStream as MemoryStream).ToArray(), 0, (int)ImageStream.Length);

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
                                            if (!(decodeParams[DictionaryProperties.EndOfBlock] as PdfBoolean).Value)
                                                tiff.SetField(1, 2, TiffTag.Compression, TiffType.Short);
                                        }
                                        else
                                            tiff.SetField(1, 3, TiffTag.Compression, TiffType.Short);
                                    }
                                    else
                                        tiff.SetField(1, 3, TiffTag.Compression, TiffType.Short);
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
                                ImageStream = tiff.m_stream;
                                ImageStream.Position = 0;
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
                                        decoder.GlobalData = globalStream.ToArray();
                                    }
                                }

                                decoder.DecodeJBIG2(encodedStream.ToArray());
                                int bmpWidth = 0;
                                int bmpHeight = 0;
                                if (m_imageDictionary.ContainsKey(DictionaryProperties.Width) && m_imageDictionary.ContainsKey(DictionaryProperties.Height))
                                {
                                    bmpWidth = (m_imageDictionary[DictionaryProperties.Width] as PdfNumber).IntValue;
                                    bmpHeight = (m_imageDictionary[DictionaryProperties.Height] as PdfNumber).IntValue;
                                }
                                JBIG2Image imag = decoder.GetPageAsJBIG2Bitmap(0);
                                imagedata = imag.GetData(true);
                                var factory = new ImageFactory();
                                ImageStream stream = null;
                                IRandomAccessStream wicImageStream = new InMemoryRandomAccessStream();
                                stream = factory.CreateStream(wicImageStream);

                                var encoder = factory.CreatePngEncoder();
                                encoder.Initialize(stream);

                                var bitmapFrameEncode = encoder.CreateBitmapFrameEncode();

                                bitmapFrameEncode.Initialize();

                                bitmapFrameEncode.SetSize((int)bmpWidth, (int)bmpHeight);
                                m_colorPalette = factory.CreatePalette();

                                int[] colors = new int[2];
                                colors[0] = ConvertToInt(new byte[] { 0, 0, 0, 255 });
                                colors[1] = ConvertToInt(new byte[] { 255, 255, 255, 255 });
                                m_colorPalette.Initialize(colors);
                                bitmapFrameEncode.Palette = m_colorPalette;

                                Guid pFormat = PixelFormat.Format1bppIndexed;
                                bitmapFrameEncode.SetPixelFormat(pFormat);

                                int bitsPerPixel = PixelFormat.GetBitsPerPixel(pFormat);
                                int pixelSize = bitsPerPixel / 8;
                                int stride = PixelFormat.GetStride(pFormat, (int)bmpWidth);
                                var bufferSize = (int)bmpHeight * stride;
                                //var encodeBuffer = new DataStream(bufferSize, true, true);


                                //IntPtr imagePointer = encodeBuffer.DataPointer;
                                //Marshal.Copy(imagedata, 0, imagePointer, imagedata.Length);


                                bitmapFrameEncode.WritePixels((int)bmpHeight, stride, bufferSize, imagedata);
                                bitmapFrameEncode.Commit();
                                encoder.Commit();
                                stream.Commit();

                                bitmapFrameEncode.Dispose();
                                encoder.Dispose();
                                stream.Dispose();
                                factory.Dispose();
                                ImageStream = wicImageStream.AsStream();
                                ImageStream.Position = 0;
                                //        Bitmap iii = new Bitmap((int)Width, (int)Height, PixelFormat.Format1bppIndexed);
                                //        BitmapData iiiData = iii.LockBits(new Rectangle(0, 0, (int)Width, (int)Height), ImageLockMode.ReadWrite, iii.PixelFormat);

                                //        int rows = (int)Height;
                                //        int stride = iiiData.Stride;
                                //        IntPtr portionOffset = iiiData.Scan0;

                                //        int bpp = Image.GetPixelFormatSize(iii.PixelFormat);
                                //        int pixSize = bpp / 8;

                                //        int rowWidth = pixSize * (int)Width;

                                //        //We should align row depending to pixel size;
                                //        if (pixSize == 3)
                                //        {
                                //            //NOTE: 3.0 = RGB pixel size.
                                //            rowWidth = pixSize * (int)Width;
                                //        }
                                //        else if (pixSize == 0)
                                //        {
                                //            //NOTE: 0.125 = 1/8 and its 1 bit for pixel or 8 pixels per byte.
                                //            rowWidth = (int)Width * bpp / 8;

                                //            if (((int)Width * bpp % 8) != 0)
                                //                ++rowWidth;
                                //        }
                                //        else if (pixSize == 1)
                                //        {
                                //            rowWidth = iii.Width;
                                //        }

                                //        int offset = 0;
                                //        long ptr = iiiData.Scan0.ToInt64();
                                //        for (int i = 0; i < Height; i++)
                                //        {
                                //            Marshal.Copy(imagedata, offset, new IntPtr(ptr), (int)(rowWidth));
                                //            offset += (int)(rowWidth);
                                //            ptr += iiiData.Stride;
                                //        }

                                //        iii.UnlockBits(iiiData);

                                //        Image finalImage = iii as Image;
                                //        MemoryStream embeddedImageStream = new MemoryStream();
                                //        finalImage.Save(embeddedImageStream, ImageFormat.Jpeg);
                                //        embeddedImageStream.Position = 0;
                                //        ImageStream = embeddedImageStream;
                                //        ImageStream.Position = 0;
                                break;
                            }
                        #endregion
                        case "JPXDecode":
                            {
                                ImageStream.Position = 0;
                                JPXImage jpxImage = new JPXImage();
                                ImageStream = jpxImage.FromStream(ImageStream);
                                ImageStream.Position = 0;
                                break;
                            }
                        default:
                            {
                                IsExceptionThrown = true;
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
            IsExceptionThrown = true;
            return null;
        }

        internal static async Task<IRandomAccessStream> ConvertStream(Stream ImageStream)
        {
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            DataWriter writer = new DataWriter(randomAccessStream.GetOutputStreamAt(0));
            byte[] temp = new byte[ImageStream.Length];
            ImageStream.Position = 0;
            ImageStream.Read(temp, 0, (int)ImageStream.Length);
            writer.WriteBytes(temp);
            await writer.StoreAsync();
            await writer.FlushAsync();
            writer.Dispose();

            return randomAccessStream;
        }


        /// <summary>
        /// Process the ColorSpace property in the Image dictionary
        /// </summary>
        private void GetColorSpace()
        {
            IsDeviceNColorSpace = false;
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
                        if ((colorSpaceResources[0] as PdfName).Value == "Indexed")
                        {
                            try
                            {
                                if (((colorSpaceResources[colorSpaceResources.Count - 1] as PdfReferenceHolder).Object as PdfDictionary).Values.Count > 1)
                                {
                                    PdfArray imageResourceArray = ((colorSpaceResources[colorSpaceResources.Count - 3] as PdfReferenceHolder).Object as PdfArray);
                                    if (imageResourceArray[0] is PdfName)
                                    {
                                        IsDeviceNColorSpace = true;
                                        indexStream = ((colorSpaceResources[colorSpaceResources.Count - 1] as PdfReferenceHolder).Object as PdfStream).InternalStream;
                                        indexStream = DecodeFlateStream(indexStream);
                                        colorSpaceResourceDict.Add("Indexed", indexStream);
                                    }
                                    else
                                    {
                                        indexStream = ((colorSpaceResources[colorSpaceResources.Count - 1] as PdfReferenceHolder).Object as PdfStream).InternalStream;
                                        indexStream = DecodeFlateStream(indexStream);
                                        colorSpaceResourceDict.Add("Indexed", indexStream);
                                    }
                                    isIndexedImage = true;
                                }
                                else
                                {
                                    indexStream = ((colorSpaceResources[colorSpaceResources.Count - 1] as PdfReferenceHolder).Object as PdfStream).InternalStream;
                                    colorSpaceResourceDict.Add("Indexed", indexStream);
                                    isIndexedImage = true;
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

        private void GetIndexedColorSpace(PdfArray value, string internalColorSpace, PdfDictionary colorspaceDictionary, string[] filter)
        {
            if (m_colorspace == "Indexed")
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
                        m_colorspaceStream = new MemoryStream(buffer, 0, buffer.Length, true);
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
            else if (m_colorspace == "ICCBased")
            {
                if (value[1] is PdfReferenceHolder)
                {
                    if ((value[1] as PdfReferenceHolder).Object is PdfDictionary)
                    {
                        PdfDictionary temp = (value[1] as PdfReferenceHolder).Object as PdfDictionary;
                        if (temp.ContainsKey(DictionaryProperties.Alternate))
                        {
                            if ((temp[DictionaryProperties.Alternate] as PdfName).Value == "DeviceRGB")
                            {
                                IsAlternateDeviceRGB = true;
                            }
                        }
                    }
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

            if (m_colorspace == "Indexed")
            {
                if (BitsPerComponent == 8)
                    m_pixelFormat = PixelFormat.Format8bppIndexed;
                if (m_colorspaceStream == null)
                {
                    m_colorspaceStream = ((value[3] as PdfReferenceHolder).Object as PdfStream).InternalStream;
                    m_colorspaceStream.Position = 0;
                }

                byte[] colorspaceByteSrc = m_colorspaceStream.ToArray();
                byte[] colorspaceByte = new byte[786];
                Array.Copy(colorspaceByteSrc, colorspaceByte, colorspaceByteSrc.Length);
                var factory = new ImageFactory();
                m_colorPalette = factory.CreatePalette();

                int count = 0;
                int[] colors = new int[256];
                for (int i = 0; i < 256; i++)
                {
                    byte[] color = new byte[4];

                    color[2] = colorspaceByte[count++];
                    color[1] = colorspaceByte[count++];
                    color[0] = colorspaceByte[count++];
                    color[3] = 255;

                    colors[i] = ConvertToInt(color);
                }
                m_colorPalette.Initialize(colors);
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

                    System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("Windows-1252");
                    byte[] encodeBuffer = new byte[] { Convert.ToByte(decimalValue) };
                    temp = encoding.GetString(encodeBuffer, 0, encodeBuffer.Length);

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
            byte[] decodedBytes = decoder.decode((encodedStream as MemoryStream).ToArray());

            MemoryStream outStream = new MemoryStream(decodedBytes, 0, decodedBytes.Length, true);
            outStream.Position = 0;
            return outStream;
        }

        /// <summary>
        /// Decodes the Flate encoded stream
        /// </summary>
        /// <param name="encodedStream">Encoded stream</param>
        /// <returns>Decoded Stream</returns>
        private MemoryStream DecodeFlateStream(Stream encodedStream)
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
        /// <param name="imageStream">Stream of the original image</param>
        /// <param name="maskStream">Encoded stream of the mask image</param>
        /// <returns>Stream of the merged image</returns>
        private InMemoryRandomAccessStream MergeImages(IRandomAccessStream input, IRandomAccessStream maskStream)
        {
            Guid globalPixelFormat = PixelFormat.Format32bppBGRA;

            global::Windows.UI.Color transparentColor;
            input.Seek(0);
            maskStream.Seek(0);

            var factory = new ImageFactory();
            var formatConverter = factory.CreateFormatConverter();
            if (input == null || input.Size == 0)
            {
                ImageStream blankStream = null;
                IRandomAccessStream blankWicImageStream = new InMemoryRandomAccessStream();
                blankStream = factory.CreateStream(blankWicImageStream);

                var blankEncoder = factory.CreatePngEncoder();
                blankEncoder.Initialize(blankStream);

                var blankBitmapFrameEncode = blankEncoder.CreateBitmapFrameEncode();

                blankBitmapFrameEncode.Initialize();

                blankBitmapFrameEncode.SetSize((int)m_maskWidth, (int)m_maskHeight);
                transparentColor = global::Windows.UI.Colors.White;
                blankBitmapFrameEncode.SetPixelFormat(globalPixelFormat);

                int blankBitsPerPixel = PixelFormat.GetBitsPerPixel(globalPixelFormat);

                int blankStride = PixelFormat.GetStride(globalPixelFormat, (int)m_maskWidth);
                var blankBufferSize = (int)m_maskHeight * blankStride;

                byte[] blankOutputBytes = new byte[blankBufferSize];

                blankBitmapFrameEncode.WritePixels((int)m_maskHeight, blankStride, blankBufferSize, blankOutputBytes);

                blankBitmapFrameEncode.Commit();
                blankEncoder.Commit();
                blankBitmapFrameEncode.Dispose();
                blankEncoder.Dispose();
                blankStream.Dispose();
                input = blankWicImageStream;
            }
            //else
            //    transparentColor = input.GetPixel(0, 0);
            var bitmapDecoder = factory.CreateBitmapDecoder(input, DecodeOptions.CacheOnDemand);
            formatConverter.Initialize(bitmapDecoder.GetFrame(0), globalPixelFormat, BitmapDitherType.None, null, 0.0, BitmapPaletteType.Custom);
            WicBitmap inputImage = factory.CreateBitmap(formatConverter, BitmapCreateCacheOption.CacheOnLoad);
            BitmapLock inputImageData = inputImage.Lock(new global::Windows.Foundation.Rect(0, 0, inputImage.Size.Width, inputImage.Size.Height), BitmapLockFlags.Read);
            int bytes = (int)(inputImageData.Stride * inputImageData.Size.Height);

            int size;
            DataPointer dPtr = inputImageData.GetDataPointer(out size);

            IntPtr ptr = new IntPtr(dPtr.Pointer);
            byte[] inputBytes = new byte[bytes];
            Marshal.Copy(ptr, inputBytes, 0, bytes);

            IRandomAccessStream mask = DecodeMaskImage(maskStream);
            bitmapDecoder = factory.CreateBitmapDecoder(mask, DecodeOptions.CacheOnDemand);
            formatConverter = factory.CreateFormatConverter();
            formatConverter.Initialize(bitmapDecoder.GetFrame(0), globalPixelFormat, BitmapDitherType.None, null, 0.0, BitmapPaletteType.Custom);
            WicBitmap maskImage = factory.CreateBitmap(formatConverter, BitmapCreateCacheOption.CacheOnLoad);
            BitmapLock maskImageData = maskImage.Lock(new global::Windows.Foundation.Rect(0, 0, inputImage.Size.Width, inputImage.Size.Height), BitmapLockFlags.Read);
            dPtr = maskImageData.GetDataPointer(out size);
            ptr = new IntPtr(dPtr.Pointer);
            byte[] maskBytes = new byte[bytes];
            Marshal.Copy(ptr, maskBytes, 0, bytes);


            ImageStream stream = null;
            InMemoryRandomAccessStream wicImageStream = new InMemoryRandomAccessStream();
            stream = factory.CreateStream(wicImageStream);

            var encoder = factory.CreatePngEncoder();
            encoder.Initialize(stream);

            var bitmapFrameEncode = encoder.CreateBitmapFrameEncode();

            bitmapFrameEncode.Initialize();

            bitmapFrameEncode.SetSize((int)inputImage.Size.Width, (int)inputImage.Size.Height);

            bitmapFrameEncode.SetPixelFormat(globalPixelFormat);

            int bitsPerPixel = PixelFormat.GetBitsPerPixel(globalPixelFormat);
            int pixelSize = bitsPerPixel / 8;
            int stride = PixelFormat.GetStride(globalPixelFormat, (int)inputImage.Size.Width);
            var bufferSize = (int)inputImage.Size.Height * stride;

            byte[] outputBytes = new byte[bytes];

            byte max = 255;
            for (int i = 0; i < bytes; )
            {
                if (maskBytes[i] != 0 || maskBytes[i + 1] != 0 || maskBytes[i + 2] != 0 || maskBytes[i + 3] != 255)
                {
                    global::Windows.UI.Color inputColor = global::Windows.UI.Color.FromArgb(inputBytes[i], inputBytes[i + 1], inputBytes[i + 2], inputBytes[i + 3]);
                    global::Windows.UI.Color maskColor = global::Windows.UI.Color.FromArgb((byte)(max - maskBytes[i]), (byte)(max - maskBytes[i + 1]), (byte)(max - maskBytes[i + 2]), (byte)(max - maskBytes[i + 3]));

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
                    if (transparentColor == global::Windows.UI.Color.FromArgb(255, 0, 0, 0) || transparentColor == global::Windows.UI.Color.FromArgb(255, 255, 255, 255))
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

            //Marshal.Copy(outputBytes, 0, buffer.DataPointer, bytes);

            bitmapFrameEncode.WritePixels((int)inputImage.Size.Height, stride, bytes, outputBytes);

            bitmapFrameEncode.Commit();
            encoder.Commit();
            stream.Commit();
            bitmapFrameEncode.Dispose();
            encoder.Dispose();
            stream.Dispose();
            bitmapDecoder.Dispose();
            inputImage.Dispose();
            inputImageData.Dispose();
            maskImage.Dispose();
            maskImageData.Dispose();
            //if (transparentColor == global::Windows.UI.Color.FromArgb(255, 0, 0, 0) || transparentColor == global::Windows.UI.Color.FromArgb(255, 255, 255, 255))
            //    output.MakeTransparent(transparentColor);
            //else
            //    output.MakeTransparent();
            wicImageStream.Seek(0);
            return wicImageStream;
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

        ///<summary>
        ///Decodes the stream of the mask image in the PDF document
        ///</summary>
        ///<param name="mask">Encoded stream from the PDF document</param>
        ///<returns>Decoded stream of the image</returns>
        private IRandomAccessStream DecodeMaskImage(IRandomAccessStream mask)
        {
            Guid maskPixelFormat = PixelFormat.Format8bppIndexed;
            int masking = 15;

            if (m_maskBitsPerComponent == 1)
            {
                maskPixelFormat = PixelFormat.Format1bppIndexed;

                byte[] imagedata = DecodeFlateStream(mask.AsStream()).ToArray();
                var factory = new ImageFactory();

                ImageStream stream = null;
                IRandomAccessStream wicImageStream = new InMemoryRandomAccessStream();
                stream = factory.CreateStream(wicImageStream);

                var encoder = factory.CreatePngEncoder();
                encoder.Initialize(stream);

                var bitmapFrameEncode = encoder.CreateBitmapFrameEncode();

                bitmapFrameEncode.Initialize();

                bitmapFrameEncode.SetSize((int)m_maskWidth, (int)m_maskHeight);
                Palette colorPalette = factory.CreatePalette();

                int[] colors = new int[2];

                colors[0] = ConvertToInt(new byte[] { 0, 0, 0, 255 });
                colors[1] = ConvertToInt(new byte[] { 255, 255, 255, 255 });
                colorPalette.Initialize(colors);

                bitmapFrameEncode.Palette = colorPalette;
                bitmapFrameEncode.SetPixelFormat(maskPixelFormat);

                int rows = (int)m_maskHeight;
                int stride = PixelFormat.GetStride(maskPixelFormat, (int)m_maskWidth);
                var bufferSize = (int)m_maskHeight * stride;
                int bpp = PixelFormat.GetBitsPerPixel(maskPixelFormat);
                int rowWidth = (int)m_maskWidth * bpp / 8;

                if (((int)m_maskWidth * bpp % 8) != 0)
                    ++rowWidth;

                var ptrFinal = Marshal.AllocHGlobal(bufferSize + masking + IntPtr.Size);
                var buffer = new DataPointer();
                buffer.Pointer = ptrFinal.ToInt64();
                buffer.Size = bufferSize;
                int offset = 0;
                long imagePointer = buffer.Pointer;
                for (int i = 0; i < m_maskHeight; i++)
                {
                    Marshal.Copy(imagedata, offset, new IntPtr(imagePointer), (int)(rowWidth));
                    offset += (int)(rowWidth);
                    imagePointer += stride;
                }

                bitmapFrameEncode.WritePixels((int)m_maskHeight, stride, buffer);

                bitmapFrameEncode.Commit();
                encoder.Commit();
                stream.Commit();
                bitmapFrameEncode.Dispose();
                encoder.Dispose();
                stream.Dispose();
                factory.Dispose();
                return wicImageStream;
            }

            byte[] outArray = DecodeFlateStream(mask.AsStream()).ToArray();
            var maskFactory = new ImageFactory();

            ImageStream maskStream = null;
            IRandomAccessStream maskWicImageStream = new InMemoryRandomAccessStream();
            maskStream = maskFactory.CreateStream(maskWicImageStream);

            var maskEncoder = maskFactory.CreatePngEncoder();
            maskEncoder.Initialize(maskStream);

            var maskBitmapFrameEncode = maskEncoder.CreateBitmapFrameEncode();

            maskBitmapFrameEncode.Initialize();

            maskBitmapFrameEncode.SetSize((int)m_maskWidth, (int)m_maskHeight);
            Palette maskColorPalette = maskFactory.CreatePalette();

            int[] maskColors = new int[256];
            for (int i = 0; i < 256; i++)
            {
                maskColors[i] = ConvertToInt(new byte[] { (byte)i, (byte)i, (byte)i, 255 });
            }
            maskColorPalette.Initialize(maskColors);
            maskBitmapFrameEncode.Palette = maskColorPalette;
            maskBitmapFrameEncode.SetPixelFormat(maskPixelFormat);

            int maskStride = PixelFormat.GetStride(m_pixelFormat, (int)m_maskWidth);
            var maskBufferSize = (int)Height * maskStride;
            var maskBuffer = new DataPointer();
            var ptr = Marshal.AllocHGlobal(maskBufferSize + masking + IntPtr.Size);

            maskBuffer.Pointer = ptr.ToInt64();
            maskBuffer.Size = maskBufferSize;
            float length = Math.Abs(maskStride) * m_maskHeight;
            if (maskPixelFormat == PixelFormat.Format8bppIndexed)
            {
                maskStride = PixelFormat.GetStride(maskPixelFormat, (int)m_maskWidth);
                maskBufferSize = (int)m_maskHeight * maskStride;
                maskBuffer = new DataPointer();
                ptr = Marshal.AllocHGlobal(maskBufferSize + masking + IntPtr.Size);

                maskBuffer.Pointer = ptr.ToInt64();
                maskBuffer.Size = maskBufferSize;
                length = Math.Abs(maskStride) * m_maskHeight;
                int offset = 0;
                long imagePointer = maskBuffer.Pointer;

                for (int i = 0; i < m_maskHeight; i++)
                {
                    Marshal.Copy(outArray, offset, new IntPtr(imagePointer), (int)(m_maskWidth));
                    offset += (int)(m_maskWidth);
                    imagePointer += maskStride;
                }

                maskBitmapFrameEncode.WritePixels((int)m_maskHeight, maskStride, maskBuffer);
            }
            else
                maskBitmapFrameEncode.WritePixels((int)Height, maskStride, outArray.Length, outArray);

            maskBitmapFrameEncode.Commit();
            maskEncoder.Commit();
            maskStream.Commit();
            maskWicImageStream.Seek(0);
            maskBitmapFrameEncode.Dispose();
            maskEncoder.Dispose();
            maskStream.Dispose();
            maskFactory.Dispose();

            return maskWicImageStream;
        }

        private IRandomAccessStream DecodeDeviceGrayImage(byte[] imageStr)
        {
            var factory = new ImageFactory();
            byte[] imagedata = imageStr;
            ImageStream stream = null;
            IRandomAccessStream wicImageStream = new InMemoryRandomAccessStream();
            stream = factory.CreateStream(wicImageStream);

            var encoder = factory.CreatePngEncoder();
            encoder.Initialize(stream);

            var bitmapFrameEncode = encoder.CreateBitmapFrameEncode();

            bitmapFrameEncode.Initialize();

            bitmapFrameEncode.SetSize((int)Width, (int)Height);
            Guid pixelFormat = PixelFormat.Format1bppIndexed;
            bitmapFrameEncode.SetPixelFormat(pixelFormat);

            m_colorPalette = factory.CreatePalette();

            int[] colors = new int[2];
            colors[0] = ConvertToInt(new byte[] { (byte)0, (byte)0, (byte)0, 255 });
            colors[1] = ConvertToInt(new byte[] { 255, 255, 255, 255 });

            m_colorPalette.Initialize(colors);
            bitmapFrameEncode.Palette = m_colorPalette;

            int stride = PixelFormat.GetStride(pixelFormat, (int)Width);
            var bufferSize = (int)Height * stride;

            int rows = (int)Height;
            int bpp = PixelFormat.GetBitsPerPixel(pixelFormat);
            int pixSize = bpp / 8;
            int rowWidth = (int)Width * bpp / 8;

            if (((int)Width * bpp % 8) != 0)
                ++rowWidth;

            int masking = 15;
            var ptrFinal = Marshal.AllocHGlobal(bufferSize + masking + IntPtr.Size);
            var buffer = new DataPointer();
            buffer.Pointer = ptrFinal.ToInt64();
            buffer.Size = bufferSize;
            int offset = 0;
            long imagePointer = buffer.Pointer;
            for (int i = 0; i < Height; i++)
            {
                Marshal.Copy(imagedata, offset, new IntPtr(imagePointer), (int)(rowWidth));
                offset += (int)(rowWidth);
                imagePointer += stride;
            }

            bitmapFrameEncode.WritePixels((int)Height, stride, buffer);

            bitmapFrameEncode.Commit();
            encoder.Commit();
            stream.Commit();
            bitmapFrameEncode.Dispose();
            encoder.Dispose();
            stream.Dispose();
            factory.Dispose();

            return wicImageStream;
        }

        private int ConvertToInt(byte[] array)
        {
            int MASK = 0xFF;
            int result = 0;
            result = array[0] & MASK;
            result = result + ((array[1] & MASK) << 8);
            result = result + ((array[2] & MASK) << 16);
            result = result + ((array[3] & MASK) << 24);
            return result;
        }

        private byte[] ConvertToByte(int color)
        {
            byte[] bytes = new byte[4];

            bytes[0] = (byte)(color >> 24);
            bytes[1] = (byte)(color >> 16);
            bytes[2] = (byte)(color >> 8);
            bytes[3] = (byte)color;

            return bytes;
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
                    var randomAccessStream = new MemoryRandomAccessStream(args.ImageStream);


                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(randomAccessStream);
                    embeddedImage.Source = bitmapImage;
                }
                else
                {
                    //embeddedImage = Image.FromStream(GetImageStream());
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

            double c, m, y, k = 1, interW, interC, interM, interY, interR, interG, interB;
            double outRed = 0, outGreen = 0, outBlue = 0;
            int pointer = 0;

            for (int i = 0; i < pixelCount; i = i + 4)
            {
                if (i > encodedData.Length)
                    break;
                double inCyan = (encodedData[i] & 0xff) / x;
                double inMagenta = (encodedData[i + 1] & 0xff) / x;
                double inYellow = (encodedData[i + 2] & 0xff) / x;
                double inBlack = (encodedData[i + 3] & 0xff) / x;

                if ((finalC != inCyan) || (finalM != inMagenta) || (finalY != inYellow) || (finalK != inBlack))
                {
                    c = RoundOff(inCyan + inBlack);
                    m = RoundOff(inMagenta + inBlack);
                    y = RoundOff(inYellow + inBlack);
                    interW = (k - c) * (k - m) * (k - y);
                    interC = c * (k - m) * (k - y);
                    interM = (k - c) * m * (k - y);
                    interY = (k - c) * (k - m) * y;
                    interR = (k - c) * m * y;
                    interG = c * (k - m) * y;
                    interB = c * m * (k - y);
                    outRed = x * RoundOff(interW + 0.9137 * interM + 0.9961 * interY + 0.9882 * interR);
                    outGreen = x * RoundOff(interW + 0.6196 * interC + interY + 0.5176 * interG);
                    outBlue = x * RoundOff(interW + 0.7804 * interC + 0.5412 * interM + 0.0667 * interR + 0.2118 * interG + 0.4863 * interB);

                    finalC = inCyan;
                    finalM = inMagenta;
                    finalY = inYellow;
                    finalK = inBlack;
                }

                decodedData[pointer++] = (byte)(outRed);
                decodedData[pointer++] = (byte)(outGreen);
                decodedData[pointer++] = (byte)(outBlue);
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

    public class ImagePreRenderEventArgs : EventArgs
    {
        internal Stream m_imageStream;
        internal float m_height;
        internal float m_width;
        internal string[] m_filter;

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
