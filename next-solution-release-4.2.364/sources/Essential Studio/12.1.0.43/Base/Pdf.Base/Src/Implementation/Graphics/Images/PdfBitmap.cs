#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Syncfusion.Pdf.Compression;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Applicable only for monochrome TIFF images.
    /// </summary>
    public enum EncodingType
    {
        /// <summary>
        /// Default compression.
        /// </summary>
        Default,
        /// <summary>
        /// Uses JBIG2 compression for monochrome TIFF images.
        /// </summary>
        JBIG2
    }

    /// <summary>
    /// Represents the bitmap images.
    /// </summary>
    public class PdfBitmap
        : PdfImage,
        IDisposable
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
        /// <summary>
        /// Holds image.
        /// </summary>
        private Image m_image;
        /// <summary>
        /// Holds the index of active frame for multiframe images.
        /// </summary>
        private int m_activeFrame;
        /// <summary>
        /// Holds image frame dimention.
        /// </summary>
        private FrameDimension m_frameDimention;
        /// <summary>
        /// Holds mask for current image.
        /// </summary>
        private PdfMask m_mask;
        /// <summary>
        /// Holds bits per component.
        /// </summary>
        private int m_bits;
        /// <summary>
        /// Holds image color space.
        /// </summary>
        private PdfColorSpace m_colorspace;
        /// <summary>
        /// A flag indicating whether the image is to be treated as an image mask.
        /// </summary>
        private bool m_imageMask;
        /// <summary>
        /// Holds list of saved frames.
        /// </summary>
        private int[] m_frameList;
        /// <summary>
        /// Indicates if the object has been disposed.
        /// </summary>
        private bool m_bDisposed;
        /// <summary>
        /// The quality of the stored image.
        /// </summary>
        /// <remarks>When the image is stored into PDF not as a mask,
        /// you may reduce its quality, which saves the disk space.</remarks>
        private long m_quality = 100;
        /// <summary>
        /// Internal variable to store the check value.
        /// </summary>
        private int check;

        /// <summary>
        /// Holds the raw image stream
        /// </summary>
        private Stream m_internalImageStream;
       
        private System.Collections.Generic.List<byte> m_symbol;
        private PdfJBIG2Compressor m_jbig2Compressor;
        private EncodingType m_encodingType;
        #endregion

#region Properties
        /// <summary>
        /// Get or sets the Check Value.
        /// </summary>
        internal int Check
        {
            get
            {
                return check;
            }
            set
            {
                check = value;
            }
        }
        /// <summary>
        /// Gets or sets the active frame of the bitmap.
        /// </summary>
        /// <value>The active frame index.</value>
        public int ActiveFrame
        {
            get
            {
                return m_activeFrame;
            }
            set
            {
                if (value < 0 || value >= FrameCount)
                    throw new ArgumentOutOfRangeException("ActiveFrame");
               
                m_activeFrame = value;
                m_image.SelectActiveFrame(m_frameDimention, m_activeFrame);

            }
        }

        /// <summary>
        /// Gets the number of frames in the bitmap.
        /// </summary>
        /// <value>The frame count.</value>
        public int FrameCount
        {
            get
            {
                return m_image.GetFrameCount(m_frameDimention);
            }
        }

        /// <summary>
        /// Gets or sets the mask of bitmap.
        /// </summary>
        /// <value>New PdfMask.</value>
        public PdfMask Mask
        {
            get
            {
                return m_mask;
            }
            set
            {
                if (value is PdfImageMask)
                {
                    PdfImageMask mask = value as PdfImageMask;

                    if (mask.SoftMask)
                    {
                        Image maskImage = mask.Mask.m_image;

                        if (m_image.Size != maskImage.Size)
                        {
                            throw new ArgumentException("Soft mask must be the same size as drawing image.");
                        }
                    }
                }
                m_mask = value;
            }
        }

        /// <summary>
        /// Gets or sets the quality.
        /// </summary>
        /// <remarks>When the image is stored into PDF not as a mask,
        /// you may reduce its quality, which saves the disk space.</remarks>
        public long Quality
        {
            get
            {
                return m_quality;
            }
            set
            {
                if (value > 100 || value < 0)
                    throw new ArgumentOutOfRangeException("Quality", value,
                        "The Quality should be in the range from 100 to 0.");

                m_quality = value;
            }
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        internal override Image InternalImage
        {
            get
            {
                return m_image;
            }
        }

        public EncodingType Encoding
        {
            get
            {
                return m_encodingType;
            }
            set
            {
                m_encodingType = value;
            }
        }
        #endregion

#region Constructors
        /// <summary>
        /// Creates new PdfBitmap instance.
        /// </summary>
        /// <param name="image">The image.</param>
        public PdfBitmap(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (image is Metafile)
            {
                image = new Bitmap(image);
            }

            //CMYK image has this pixel format in Windows 7 environment
            if ((int)image.PixelFormat == 8207)
            {               
                m_internalImageStream = new MemoryStream();
                image.Save(m_internalImageStream,image.RawFormat);               
                m_internalImageStream.Position=0;
                this.m_colorspace = PdfColorSpace.CMYK;                             
            }
          
            m_image = image;
            m_bits = GetBitsPerPixel(image.PixelFormat);

            Guid[] guid = m_image.FrameDimensionsList;

            m_frameDimention = new FrameDimension(guid[0]);

            int count = FrameCount;

            m_frameList = new int[count];

            for (int i = 0; i < count; i++)
            {
                m_frameList[i] = -1;
            }
        }

        /// <summary>
        /// Creates new PdfBitmap instance.
        /// </summary>
        /// <param name="path">The image path.</param>
        public PdfBitmap(string path)
            : this(new Bitmap(Utils.CheckFilePath(path)))
        {
        }

        /// <summary>
        /// Creates new PdfBitmap instance.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public PdfBitmap(Stream stream)
            : this(Image.FromStream(CheckStreamExistance(stream)))
        {
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="T:Syncfusion.Pdf.Graphics.PdfBitmap"/> is reclaimed by garbage collection.
        /// </summary>
        ~PdfBitmap()
        {
            Dispose(false);
        }
        #endregion

#region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the specified disposing.
        /// </summary>
        /// <param name="disposing">if it is disposing, set to <c>true</c>.</param>
        private void Dispose(bool disposing)
        {
            if (!m_bDisposed)
            {
                if (disposing) // Dispose managed objects.
                {
                    InternalImage.Dispose();
                }

                if (m_symbol != null)
                    m_symbol.Clear();
                if (m_jbig2Compressor != null)
                {
                    m_jbig2Compressor.Dispose();
                    m_jbig2Compressor = null;
                }
                m_image = null;

                m_bDisposed = true;
            }
        }
        #endregion

#region Implementation
        /// <summary>
        /// Saves the image into stream.
        /// </summary>
        #if !NETFX_CORE  && !WP
        [Syncfusion.Documentation.DocumentationExclude()] 
        #endif
        internal override void Save()
        {
            if (!IsFrameSaved(ActiveFrame))
            {
                SetContent(new PdfStream());
                SaveImageByFormat();
                SetMask();
                SetColorSpace();
                SaveRequiredItems();
                SaveAddtionalItems();
            }
        }

        /// <summary>
        /// Gets the bits per pixel.
        /// </summary>
        /// <param name="format">The pixel format of the image.</param>
        /// <returns>Number of bits per pixel</returns>
        private int GetBitsPerPixel(PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.Format48bppRgb:
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb:
                    return 16;

                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                case PixelFormat.Format8bppIndexed:
                    return 8;

                case PixelFormat.Format16bppGrayScale:
                case PixelFormat.Format1bppIndexed:
                    return 1;

                default:
                    return 8;
            }
        }
        #endregion

#region Helper methods
        /// <summary>
        /// Gets the encoder info.
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <returns></returns>
        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] encoders;
            
            encoders = ImageCodecInfo.GetImageEncoders();

            for (int i = 0; i < encoders.Length; ++i)
            {
                if (encoders[i].MimeType == mimeType)
                    return encoders[i];
            }
            return null;
        }

        /// <summary>
        /// Saves the image as JPG.
        /// </summary>
        private void SaveAsJpg()
        {
            ImageCodecInfo encoderInfo = this.GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, this.m_quality);
            MemoryStream internalStream = base.Stream.InternalStream;
            if (this.m_internalImageStream != null)
            {
                byte[] buffer = new byte[this.m_internalImageStream.Length];
                this.m_internalImageStream.Position = 0L;
                this.m_internalImageStream.Read(buffer, 0, ((int)this.m_internalImageStream.Length) - 1);
                base.Stream.InternalStream = new MemoryStream(buffer);               
                m_internalImageStream.Dispose();
            }
            else
            {
                this.m_image.Save(internalStream, encoderInfo, encoderParams);
                this.m_colorspace = this.GetJPGColorSpace(internalStream);
            }
            this.m_bits = 8;


        }

        /// <summary>
        /// Saves the current image as pixel matrix.
        /// </summary>
        private void SaveAsRawImage()
        {
            Bitmap bitmap = m_image as Bitmap;

            int imageWidth = m_image.Width;
            int imageHeight = m_image.Height;

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, imageWidth, imageHeight),
                ImageLockMode.ReadWrite, m_image.PixelFormat);

            int bpp = Image.GetPixelFormatSize(bitmap.PixelFormat);
            int pixSize = bpp / 8;

            int rowWidth = pixSize * imageWidth;

            //We should align row depending to pixel size;
            if (pixSize == 3)
            {
                //NOTE: 3.0 = RGB pixel size.
                rowWidth = pixSize * imageWidth;
            }
            else if (pixSize == 0)
            {
                //NOTE: 0.125 = 1/8 and its 1 bit for pixel or 8 pixels per byte.
                rowWidth = imageWidth * bpp / 8;

                if ((imageWidth * bpp % 8) != 0)
                    ++rowWidth;
            }
            else if (pixSize == 1)
            {
                rowWidth = bitmap.Width;
            }

            byte[] dest = new byte[rowWidth];
            byte[] outputStream = null;

            MemoryStream ms = new MemoryStream();

            IntPtr portionOffset = data.Scan0;

            int rows = imageHeight;
            int stride = data.Stride;

            for (int i = 0; i < rows; i++)
            {
                Marshal.Copy(portionOffset, dest, 0, dest.Length);
                portionOffset = new IntPtr(stride + (long)portionOffset);
                ms.Write(dest, 0, dest.Length);
            }

            switch (pixSize)
            {
                case 0:
                    m_colorspace = PdfColorSpace.GrayScale;
                    outputStream = ms.GetBuffer();

                    if (!m_imageMask)
                    {
                        PdfCcittEncoder encoder = new PdfCcittEncoder();
                        outputStream = encoder.EncodeData(outputStream, imageWidth, imageHeight);
                    }
                    break;

                case 1:
                    m_colorspace = PdfColorSpace.Indexed;
                    outputStream = ms.GetBuffer();
                    break;

                case 2:
                    m_colorspace = PdfColorSpace.RGB;
                    outputStream = ms.GetBuffer();
                    break;

                case 3:
                    m_colorspace = PdfColorSpace.RGB;
                    outputStream = ms.GetBuffer();

                    int pixelcount = (int)ms.Length;

                    for (int i = 0; i < pixelcount; i += 3)
                    {
                        int i2 = i + 2;
                        byte r = outputStream[i2];
                        outputStream[i2] = outputStream[i];
                        outputStream[i] = r;
                    }
                    break;

                case 4:
                    m_colorspace = PdfColorSpace.RGB;
                    outputStream = new byte[imageWidth * imageHeight * 3];

                    int rawSize = imageWidth * imageHeight * 4;
                    byte[] pixelArray = ms.GetBuffer();

                    for (int i = 0, j = 0; i < rawSize; i += 4)
                    {

                        outputStream[j + 2] = pixelArray[i];
                        outputStream[j + 1] = pixelArray[i + 1];
                        outputStream[j] = pixelArray[i + 2];

                        j += 3;
                    }
                    break;
            }

            MemoryStream imagestream = Stream.InternalStream;

            if (m_image.RawFormat.Equals(ImageFormat.Tiff) && m_encodingType == EncodingType.JBIG2)
            {
                if (m_frameList.Length == 1)
                {
                    if (m_tiffStream == null)
                        m_jbig2Compressor = new PdfJBIG2Compressor(m_tiffPath);
                    else
                        m_jbig2Compressor = new PdfJBIG2Compressor(m_tiffStream);
                }
                //else //multiple frame.
                //{
                //    MemoryStream tempStream = new MemoryStream();
                //    m_image.Save(tempStream, m_image.RawFormat);
                //    tempStream.Position = 0;
                //    m_jbig2Compressor = new Syncfusion.Pdf.Implementation.PdfJBIG2Compressor(tempStream);
                //    tempStream.Dispose();
                //}
                if (m_jbig2Compressor != null && m_jbig2Compressor.SymbolPage != null)
                {
                    outputStream = m_jbig2Compressor.SymbolPage.ToArray();
                    m_symbol = m_jbig2Compressor.SymbolArray;
                    m_colorspace = PdfColorSpace.GrayScale;
                }
            }

            imagestream.Write(outputStream, 0, outputStream.Length);
            bitmap.UnlockBits(data);
            ms.Close();
        }

        /// <summary>
        /// Gets the color space from image stream.
        /// </summary>
        /// <param name="imageStream">Memory stream of image.</param>
        /// <returns>Color space.</returns>
        private PdfColorSpace GetJPGColorSpace(MemoryStream imageStream)
        {
            int i = 0;
            int step = 2;
            byte[] ims = imageStream.GetBuffer();

            ushort soi = BitConverter.ToUInt16(ims, i);

            if (soi == c_SoiMarker)
            {
                i += step;

                ushort jfif = BitConverter.ToUInt16(ims, i);

                jfif &= 0xF0FF;

                if (jfif == c_JfifMarker)
                {
                    while (true)
                    {
                        i += step;

                        int markerLength = (int)BitConverter.ToUInt16(ims, i);

                        markerLength = ((markerLength >> 8) | (markerLength << 8) & 0xFFFF);
                        i += markerLength;

                        ushort marker = BitConverter.ToUInt16(ims, i);

                        if (marker == c_SosMarker)
                        {
                            i += step + 2;
                            switch (ims[i])
                            {
                                case 1:
                                    return PdfColorSpace.GrayScale;

                                case 3:
                                    return PdfColorSpace.RGB;

                                case 4:
                                    return PdfColorSpace.CMYK;
                            }
                        }
                        else if (marker == c_EoiMarker)
                        {
                            break;
                        }
                    }
                }
            }
            return m_colorspace;
        }

        /// <summary>
        /// Sets the mask for image.
        /// </summary>
        private void SetMask()
        {
            if (m_mask != null)
            {
                if (m_mask is PdfColorMask)
                {
                    PdfArray colorArray = new PdfArray();
                    PdfColorMask colorMask = m_mask as PdfColorMask;

                    PdfColor startColor = colorMask.StartColor;
                    PdfColor endColor = colorMask.EndColor;

                    colorArray.Add(new PdfNumber(startColor.R));
                    colorArray.Add(new PdfNumber(startColor.G));
                    colorArray.Add(new PdfNumber(startColor.B));

                    colorArray.Add(new PdfNumber(endColor.R));
                    colorArray.Add(new PdfNumber(endColor.G));
                    colorArray.Add(new PdfNumber(endColor.B));

                    Stream[DictionaryProperties.Mask] = colorArray;
                }
                else
                {
                    PdfImageMask imageMask = m_mask as PdfImageMask;
                    PdfBitmap mask = imageMask.Mask;
                    if (Matte != null)
                        mask.Matte = Matte;

                    mask.m_imageMask = true;

                    if (imageMask.SoftMask)
                    {
                        mask.Save();
                        mask.Stream[DictionaryProperties.ColorSpace] = new PdfName(DictionaryProperties.DeviceGray);
                        Stream[DictionaryProperties.SMask] = new PdfReferenceHolder(mask);
                        m_softmask = true;
                    }
                    else
                    {
                        mask.Save();
                        mask.Stream[DictionaryProperties.ImageMask] = new PdfBoolean(true);
                        Stream[DictionaryProperties.Mask] = new PdfReferenceHolder(mask);
                    }
                }
            }
        }

        /// <summary>
        /// Saves the required items.
        /// </summary>
        private void SaveRequiredItems()
        {
            PdfStream stream = Stream;

            PdfName xobj = stream.GetName(DictionaryProperties.XObject);

            PdfDictionary dic = stream as PdfDictionary;
            if (dic.ContainsKey(DictionaryProperties.Filter) == true)
            {
                Check = 1;
                if (dic[DictionaryProperties.Filter] is PdfArray)
                {
                    if (((dic[DictionaryProperties.Filter] as PdfArray)[0] as PdfName).Value == DictionaryProperties.CCITTFaxDecode)
                    {
                        Check = 2;
                    }
                }
            }
            stream[DictionaryProperties.Type] = xobj;

            PdfName value = stream.GetName(DictionaryProperties.Image);

            stream[DictionaryProperties.Subtype] = value;
            stream[DictionaryProperties.Width] = new PdfNumber(m_image.Width);
            stream[DictionaryProperties.Height] = new PdfNumber(m_image.Height);
            stream[DictionaryProperties.BitsPerComponent] = new PdfNumber(m_bits);
            if (Matte != null)
                stream[DictionaryProperties.Matte] = new PdfArray(Matte);
        }

        /// <summary>
        /// Creates the mask from ARGB image.
        /// </summary>
        /// <param name="argbImage">The ARGB image.</param>
        /// <returns></returns>
        static internal Bitmap CreateMaskFromARGBImage(Image argbImage)
        {
            Bitmap bitmap = argbImage as Bitmap;

            int imageWidth = argbImage.Width;
            int imageHeight = argbImage.Height;

            Bitmap mask = new Bitmap(imageWidth, imageHeight, PixelFormat.Format8bppIndexed);
            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, imageWidth, imageHeight),
                ImageLockMode.ReadWrite, argbImage.PixelFormat);

            BitmapData maskData = mask.LockBits(new System.Drawing.Rectangle(0, 0, imageWidth, imageHeight),
                ImageLockMode.ReadWrite, mask.PixelFormat);

            IntPtr maskDataPtr = maskData.Scan0;
            IntPtr dataPtr = data.Scan0;

            int pixelCounter = 0;
            int count = imageWidth * imageHeight * 4;

            //float pixelSize = ( float )maskData.Stride / ( float )maskData.Width;
            //int rowWidth = ( int )( pixelSize * imageWidth );
            //int rowWidth = imageWidth * 4; // 4 is ARGB size.
            int stride = maskData.Stride;
            int strideShift = data.Stride;
            int pixelCounterShift = stride - mask.Width;

            for (int j = 0, heightShift = 0; j < imageHeight; ++j, heightShift += strideShift)
            {
                for (int i = 0, cnt = imageWidth * 4; i < cnt; i += 4)
                {
                    byte alpha = Marshal.ReadByte(dataPtr, heightShift + i + 3);

                    if (alpha != 0)
                    {
                        Marshal.WriteByte(maskDataPtr, pixelCounter, alpha);
                    }

                    ++pixelCounter;
                }

                if ((stride != imageWidth))
                {
                    pixelCounter += pixelCounterShift;
                }
            }

            mask.UnlockBits(maskData);
            bitmap.UnlockBits(data);

            //mask.Save( "c:/temp/mask.bmp" ); // Debug purposes
            return mask;
        }

        /// <summary>
        /// Creates the mask from an indexed image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>The proper greyscale image, which represents the mask.</returns>
        internal static Image CreateMaskFromIndexedImage(Image image)
        {
            Bitmap bitmap = image as Bitmap;

            int imageWidth = image.Width;
            int imageHeight = image.Height;

            Bitmap mask = new Bitmap(imageWidth, imageHeight, PixelFormat.Format8bppIndexed);

            BitmapData maskData = mask.LockBits(new System.Drawing.Rectangle(0, 0, imageWidth, imageHeight),
                ImageLockMode.ReadWrite, mask.PixelFormat);

            IntPtr maskDataPtr = maskData.Scan0;

            int pixelCounter = 0;

            int stride = maskData.Stride;
            int pixelCounterShift = stride - mask.Width;

            for (int j = 0; j < imageHeight; ++j)
            {
                for (int i = 0, cnt = imageWidth; i < cnt; i += 1)
                {
                    Color pixel = bitmap.GetPixel(i, j);
                    byte alpha = pixel.A;

                    if (alpha != 0)
                    {
                        Marshal.WriteByte(maskDataPtr, pixelCounter, alpha);
                    }

                    ++pixelCounter;
                }

                if ((stride != imageWidth))
                {
                    pixelCounter += pixelCounterShift;
                }
            }

            mask.UnlockBits(maskData);

            mask.Save("c:/temp/mask.bmp"); // Debug purposes
            return mask;
        }

        /// <summary>
        /// Saves the image by pixel format.
        /// </summary>
        private void SaveImageByFormat()
        {
            PdfArray filters = new PdfArray();
            ImageFormat imf = m_image.RawFormat;

            if (imf.Equals(ImageFormat.Jpeg))
            {
                SaveImage(filters);
            }
            else if (imf.Equals(ImageFormat.Png))
            {
                if (Bitmap.IsAlphaPixelFormat(m_image.PixelFormat))
                {
                    Mask = new PdfImageMask(new PdfBitmap(CreateMaskFromARGBImage(m_image)));
                }
                SaveImage(filters);
            }
            else if (imf.Equals(ImageFormat.Tiff))
            {
                m_image.SelectActiveFrame(m_frameDimention, m_activeFrame);
                SaveAsRawImage();
            }
            else if (imf.Equals(ImageFormat.Bmp))
            {
                SaveImage(filters);
            }
            else if (imf.Equals(ImageFormat.Gif))
            {
                m_image.SelectActiveFrame(m_frameDimention, m_activeFrame);
                if (Bitmap.IsAlphaPixelFormat(m_image.PixelFormat))
                {
                    Mask = new PdfImageMask(new PdfBitmap(CreateMaskFromARGBImage(m_image)));
                }
                SaveImage(filters);
            }
            else if (imf.Equals(ImageFormat.MemoryBmp))
            {
                SaveImage(filters);
            }
            else
            {
                SaveImage(filters);
            }

            if (filters.Count > 0)
            {
                Stream[DictionaryProperties.Filter] = filters;
            }
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="filters">The image filters.</param>
        private void SaveImage(PdfArray filters)
        {
            if (m_imageMask)
            {
                SaveAsRawImage();
            }
            else if (m_bits == 1)
            {
                SaveAsRawImage();
                filters.Add(new PdfName(DictionaryProperties.CCITTFaxDecode));
            }
            else
            {
                SaveAsJpg();
                filters.Add(new PdfName(DictionaryProperties.DCTDecode));
            }
        }

        /// <summary>
        /// Sets the color space.
        /// </summary>
        private void SetColorSpace()
        {
            PdfStream stream = Stream;

            switch (m_colorspace)
            {
                case PdfColorSpace.CMYK:
                    stream[DictionaryProperties.Decode] =
                        new PdfArray(new float[] {1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f });

                    stream[DictionaryProperties.ColorSpace] =
                        stream.GetName(DictionaryProperties.DeviceCMYK);
                    break;

                case PdfColorSpace.GrayScale:
                    stream[DictionaryProperties.Decode] =
                        new PdfArray(new float[] { 0.0f, 1.0f });

                    stream[DictionaryProperties.ColorSpace] =
                        stream.GetName(DictionaryProperties.DeviceGray);
                    ImageFormat imgFormat = m_image.RawFormat;
                    if (imgFormat.Equals(ImageFormat.Tiff))
                    m_bits = 1;
                    break;

                case PdfColorSpace.RGB:
                default:
                    stream[DictionaryProperties.Decode] =
                        new PdfArray(new float[] { 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f });

                    stream[DictionaryProperties.ColorSpace] =
                        stream.GetName(DictionaryProperties.DeviceRGB);
                    break;

                case PdfColorSpace.Indexed:
                    PdfStream pallete = new PdfStream();
                    Color[] colors = m_image.Palette.Entries;
                    int count = colors.Length;
                    byte[] indexTable = new byte[count * 3];

                    for (int i = 0; i < count; i++)
                    {
                        int j = i * 3;
                        indexTable[j] = colors[i].R;
                        indexTable[j + 1] = colors[i].G;
                        indexTable[j + 2] = colors[i].B;
                    }

                    pallete.Data = indexTable;

                    PdfArray cs = new PdfArray();

                    cs.Add(new PdfName(DictionaryProperties.Indexed));
                    cs.Add(new PdfName(DictionaryProperties.DeviceRGB));
                    cs.Add(new PdfNumber(count - 1));
                    cs.Add(new PdfReferenceHolder(pallete));
                    stream[DictionaryProperties.ColorSpace] = cs;
                    break;
            }
        }

        /// <summary>
        /// Saves the additional items.
        /// </summary>
        private void SaveAddtionalItems()
        {
            if ((m_bits == 1 && m_image.RawFormat.Equals(ImageFormat.Tiff)) || Check ==2)
            {
                PdfStream stream = Stream;

                stream[DictionaryProperties.Filter] = new PdfName(DictionaryProperties.CCITTFaxDecode);

                PdfDictionary decodeParams = new PdfDictionary();

                if (m_symbol == null && m_encodingType == EncodingType.Default)
                {
                    decodeParams[DictionaryProperties.K] = new PdfNumber(-1);
                    decodeParams[DictionaryProperties.Columns] = new PdfNumber(Width);
                    decodeParams[DictionaryProperties.Rows] = new PdfNumber(Height);
                    decodeParams[DictionaryProperties.BlackIs1] = new PdfBoolean(true);
                }
                else
                {
                    stream[DictionaryProperties.Filter] = new PdfName(DictionaryProperties.JBIG2Decode);
                    PdfStream imgStr = new PdfStream();
                    imgStr.Data = m_symbol.ToArray();
                    decodeParams[DictionaryProperties.JBIG2Globals] = new PdfReferenceHolder(imgStr);
                }
                stream[DictionaryProperties.DecodeParms] = decodeParams;
                stream.Compress = false;
            }
            if (Check == 1)
            {
                PdfStream stream = Stream;

                stream[DictionaryProperties.Filter] = new PdfName(DictionaryProperties.DCTDecode);

                PdfDictionary decodeParams = new PdfDictionary();

                decodeParams[DictionaryProperties.K] = new PdfNumber(-1);
                decodeParams[DictionaryProperties.Columns] = new PdfNumber(Width);
                decodeParams[DictionaryProperties.Rows] = new PdfNumber(Height);
                decodeParams[DictionaryProperties.BlackIs1] = new PdfBoolean(true);
                stream[DictionaryProperties.DecodeParms] = decodeParams;
                stream.Compress = false;
            }
        }

        /// <summary>
        /// Checks if frame was saved already.
        /// </summary>
        /// <param name="frame">The frame index.</param>
        /// <returns>
        /// 	<c>true</c> if frame was saved, otherwise <c>false</c>.
        /// </returns>
        private bool IsFrameSaved(int frame)
        {
            int count = FrameCount;

            for (int i = 0; i < count; i++)
            {
                if (m_frameList[i] == -1)
                {
                    m_frameList[i] = frame;
                    return false;
                }
                else if (m_frameList[i] == frame)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
#else
using System;
using System.IO;
using System.Runtime.InteropServices;
using Syncfusion.Pdf.Compression;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Graphics.Images.Decoder;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represents the bitmap images.
    /// </summary>
    public class PdfBitmap
        : PdfImage,
        IDisposable
    {

        #region Fields
        private bool m_bDisposed;
        #endregion

        #region Properties
        internal ImageDecoder Decoder { get; set; }
        internal PdfMask Mask { get; set; }
        internal PdfColorSpace ColorSpace { get; set; }
        internal bool ImageMask { get; set; }
        internal int Check { get; set; }
        internal int BitsPerCompoent { get; set; }
        internal ImageDecoder CurrentDecoder { get; set; }
        #endregion

        bool imageStatus = true;
        #region Constructors
        /// <summary>
        /// Creates new PdfBitmap instance.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public PdfBitmap(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanRead || !stream.CanSeek)
                throw new ArgumentException("Unable to process the specified image stream.");

            if (!stream.IsJpeg())
                throw new PdfException("Only JPEG images are supported");

            ImageDecoder imageDecoder;
            if (ImageDecoder.TryGetDecoder(stream, out imageDecoder))
            {
                Decoder = imageDecoder;
                Height = Decoder.Height;
                Width = Decoder.Width;
                BitsPerCompoent = Decoder.BitsPerComponent;
                base.ImageStream = new PdfStream();
                base.ImageStream.InternalStream = new MemoryStream(Decoder.ImageData);
                CurrentDecoder = imageDecoder;
                imageStatus = false;
            }
            else
            {
                throw new ArgumentException("Invalid/Unsupported image stream.");
            }

        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="T:Syncfusion.Pdf.Graphics.PdfBitmap"/> is reclaimed by garbage collection.
        /// </summary>
        ~PdfBitmap()
        {
            Dispose(false);
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the specified disposing.
        /// </summary>
        /// <param name="disposing">if it is disposing, set to <c>true</c>.</param>
        private void Dispose(bool disposing)
        {
            if (!m_bDisposed)
            {
                if (disposing) // Dispose managed objects.
                {
                    //InternalImage.Dispose();
                }

                m_bDisposed = true;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Saves the image into stream.
        /// </summary>
        internal override void Save()
        {
            if (!imageStatus)
            {
                imageStatus = true;
                base.ImageStream = Decoder.GetImageDictionary(); ;
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Saves the image as JPG.
        /// </summary>
        private void SaveAsJpg()
        {
            ColorSpace = PdfColorSpace.RGB;
            BitsPerCompoent = 8;
        }

        /// <summary>
        /// Saves the current image as pixel matrix.
        /// </summary>
        private void SaveAsRawImage()
        {

            
        }


        /// <summary>
        /// Sets the mask for image.
        /// </summary>
        private void SetMask()
        {
            if (Mask != null)
            {
                
            }
        }

        /// <summary>
        /// Saves the required items.
        /// </summary>
        private void SaveRequiredItems()
        {
            PdfStream stream = ImageStream;

            PdfName xobj = stream.GetName(DictionaryProperties.XObject);

            PdfDictionary dic = stream as PdfDictionary;
            if (dic.ContainsKey(DictionaryProperties.Filter) == true)
            {
                Check = 1;
            }
            stream[DictionaryProperties.Type] = xobj;

            PdfName value = stream.GetName(DictionaryProperties.Image);

            stream[DictionaryProperties.Subtype] = value;
            stream[DictionaryProperties.Width] = new PdfNumber(Width);
            stream[DictionaryProperties.Height] = new PdfNumber(Height);
            stream[DictionaryProperties.BitsPerComponent] = new PdfNumber(BitsPerCompoent);
            stream[DictionaryProperties.Predictor] = new PdfNumber(15);
        }



        /// <summary>
        /// Saves the image by pixel format.
        /// </summary>
        private void SaveImageByFormat()
        {
            PdfArray filters = new PdfArray();
            //ImageFormat imf = m_image.RawFormat;

            if (this.Decoder is PngDecoder)
            {
                SaveImage(filters);
            }
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="filters">The image filters.</param>
        private void SaveImage(PdfArray filters)
        {
            if (ImageMask)
            {
                SaveAsRawImage();
            }
            else
            {
                SaveAsJpg();
                filters.Add(new PdfName(DictionaryProperties.DCTDecode));
            }
        }

        /// <summary>
        /// Sets the color space.
        /// </summary>
        private void SetColorSpace()
        {
            PdfStream stream = ImageStream;
            stream.InternalStream = new MemoryStream(this.Decoder.ImageData);
            switch (ColorSpace)
            {
                case PdfColorSpace.CMYK:
                    stream[DictionaryProperties.Decode] =
                        new PdfArray(new float[] { 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f });

                    stream[DictionaryProperties.ColorSpace] =
                        stream.GetName(DictionaryProperties.DeviceCMYK);
                    break;

                case PdfColorSpace.GrayScale:
                    stream[DictionaryProperties.Decode] =
                        new PdfArray(new float[] { 0.0f, 1.0f });

                    stream[DictionaryProperties.ColorSpace] =
                        stream.GetName(DictionaryProperties.DeviceGray);
                    break;

                case PdfColorSpace.RGB:
                default:
                    stream[DictionaryProperties.Decode] =
                        new PdfArray(new float[] { 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f });

                    stream[DictionaryProperties.ColorSpace] =
                        stream.GetName(DictionaryProperties.DeviceRGB);
                    break;

                case PdfColorSpace.Indexed:
                    //PdfStream pallete = new PdfStream();
                    //Color[] colors = m_image.Palette.Entries;
                    //int count = colors.Length;
                    //byte[] indexTable = new byte[count * 3];

                    //for (int i = 0; i < count; i++)
                    //{
                    //    int j = i * 3;
                    //    indexTable[j] = colors[i].R;
                    //    indexTable[j + 1] = colors[i].G;
                    //    indexTable[j + 2] = colors[i].B;
                    //}

                    //pallete.Data = indexTable;

                    //PdfArray cs = new PdfArray();

                    //cs.Add(new PdfName(DictionaryProperties.Indexed));
                    //cs.Add(new PdfName(DictionaryProperties.DeviceRGB));
                    //cs.Add(new PdfNumber(count - 1));
                    //cs.Add(new PdfReferenceHolder(pallete));
                    //stream[DictionaryProperties.ColorSpace] = cs;
                    break;
            }
        }

        /// <summary>
        /// Saves the additional items.
        /// </summary>
        private void SaveAddtionalItems()
        {
            //if (Check == 1)
            //{
            PdfStream stream = ImageStream;

            stream[DictionaryProperties.Filter] = new PdfName(DictionaryProperties.DCTDecode);

            PdfDictionary decodeParams = new PdfDictionary();

            decodeParams[DictionaryProperties.K] = new PdfNumber(-1);
            decodeParams[DictionaryProperties.Columns] = new PdfNumber(Width);
            decodeParams[DictionaryProperties.Rows] = new PdfNumber(Height);
            decodeParams[DictionaryProperties.BlackIs1] = new PdfBoolean(true);
            stream[DictionaryProperties.DecodeParms] = decodeParams;
            stream.Compress = false;
            //}
        }
        #endregion
    }
}
#endif