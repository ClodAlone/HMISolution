#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.IO;
using Syncfusion.DocIO;
#if WINRT
using Syncfusion.DocIO.WinrtHelper;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
#elif !WP
using System.Drawing;
#endif
namespace Syncfusion.DocIO.DLS.Entities
{
    /// <summary>
    /// Represents the image class.
    /// </summary>
    internal class Image : IDisposable
    {
        #region Constants
        private const string DEF_GIF_HEADER = "GIF8";
        private const int DEF_TIFF_MARKER = 42;
        #endregion

        #region Fields
        private Stream m_stream;
        private int m_height;
        private int m_width;
        private ImageFormat m_format;
        private byte[] m_pngHeader = { 137, 80, 78, 71, 13, 10, 26, 10 };
        private byte[] m_jpegHeader = { 255, 216 };
        private byte[] m_bmpHeader = { 66, 77 };
        private byte[] m_tiffHeader1 = { 73, 73 };
        private byte[] m_tiffHeader2 = { 77, 77 };
        private byte[] m_imageData;
        //private PixelFormat m_pixelFormat;
        private float m_horizontalResolution;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get
            {
                return m_width;
            }
        }
        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get
            {
                return m_height;
            }
        }
        /// <summary>
        /// Gets the image format.
        /// </summary>
        /// <value>The image format.</value>
        public ImageFormat Format
        {
            get
            {
                return m_format;
            }
        }
        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public Size Size
        {
            get
            {
                return new Size(m_width, m_height);
            }
        }
        /// <summary>
        /// Gets the image data.
        /// </summary>
        /// <value>The image data.</value>
        internal byte[] ImageData
        {
            get
            {
                return m_imageData;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ImageFormat RawFormat
        {
            get
            {
                return m_format;
            }
        }
        ///// <summary>
        ///// 
        ///// </summary>
        //public PixelFormat PixelFormat
        //{
        //  get
        //  {
        //    return m_pixelFormat;
        //  }
        //}
        /// <summary>
        /// Gets the Horizontal resolution
        /// </summary>
        public float HorizontalResolution
        {
            get
            {
                return m_horizontalResolution;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is metafile.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is metafile; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsMetafile
        {
            get
            {
                return (RawFormat == ImageFormat.Emf || RawFormat == ImageFormat.Wmf) ?
                  true : false;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Image"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public Image(Stream stream)
        {
            if (!stream.CanRead || !stream.CanSeek)
                throw new ArgumentException("Stream");

            m_stream = stream;

            Initialize();

            if (m_format == ImageFormat.Unknown)
                throw new ArgumentException("The given image stream is either unsupported or not a valid image stream");

        }
        /// <summary>
        /// 
        /// </summary>
        internal Image()
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            if (CheckIfPng())
            {
                m_format = ImageFormat.Png;
                ParsePngImage();
            }

            if (m_format == ImageFormat.Unknown
                && CheckIfJpeg())
            {
                m_format = ImageFormat.Jpeg;
                ParseJpegImage();
            }

            if (m_format == ImageFormat.Unknown
                && CheckIfGif())
            {
                m_format = ImageFormat.Gif;
                ParseGifImage();
            }

            if (m_format == ImageFormat.Unknown
                && CheckIfEmfOrWmf())
            {
                ParseEmfOrWmfImage();
            }

            if (m_format == ImageFormat.Unknown
                && CheckIfIcon())
            {
                m_format = ImageFormat.Icon;
                ParseIconImage();
            }

            if (m_format == ImageFormat.Unknown
              && CheckIfBmp())
            {
                m_format = ImageFormat.Bmp;
                ParseBmpImage();
            }

            if (m_format == ImageFormat.Unknown
              && CheckIfTiff())
            {
                m_format = ImageFormat.Tiff;
                ParseTifImage();
            }
#if WINRT
            if (m_format == ImageFormat.Unknown)
            {
                m_format = ImageFormat.Emf;
                ParseEmfOrWmfImage();
            }
#endif
            if (m_format != ImageFormat.Unknown)
            {
                Reset();
                m_imageData = new byte[m_stream.Length];
                m_stream.Read(m_imageData, 0, m_imageData.Length);
            }
        }

        /// <summary>
        /// Checks if tiff.
        /// </summary>
        /// <returns></returns>
        private bool CheckIfTiff()
        {
            Reset();
            byte[] testBytes = new byte[3];
            m_stream.Read(testBytes, 0, testBytes.Length);

            if (((testBytes[0] == m_tiffHeader1[0] && testBytes[1] == m_tiffHeader1[1]) ||
              (testBytes[0] == m_tiffHeader2[0] && testBytes[1] == m_tiffHeader2[1])) 
              && testBytes[2] == DEF_TIFF_MARKER)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if image is Bitmap.
        /// </summary>
        /// <returns></returns>
        private bool CheckIfBmp()
        {
            Reset();
            for (int i = 0; i < m_bmpHeader.Length; i++)
            {
                if (m_bmpHeader[i] != m_stream.ReadByte())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Parses the BMP image.
        /// </summary>
        private void ParseBmpImage()
        {
            Reset();
            int bitmapHeaderSize = 14;
            byte[] headerBytes = new byte[bitmapHeaderSize];
            m_stream.Read(headerBytes, 0, bitmapHeaderSize);

            int dipHeaderSize = ReadInt32();
            m_width = ReadInt32();
            m_height = ReadInt32();
        }

        /// <summary>
        /// Checks if icon.
        /// </summary>
        /// <returns></returns>
        private bool CheckIfIcon()
        {
            Reset();

            int idReserved = ReadWord();
            int idType = ReadWord();

            if (idReserved == 0 && idType == 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if PNG.
        /// </summary>
        /// <returns></returns>
        private bool CheckIfPng()
        {
            Reset();

            for (int i = 0; i < m_pngHeader.Length; i++)
            {
                if (m_pngHeader[i] != m_stream.ReadByte())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if JPEG.
        /// </summary>
        /// <returns></returns>
        private bool CheckIfJpeg()
        {
            Reset();

            for (int i = 0; i < m_jpegHeader.Length; i++)
            {
                if (m_jpegHeader[i] != m_stream.ReadByte())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if GIF.
        /// </summary>
        /// <returns></returns>
        private bool CheckIfGif()
        {
            Reset();

            string header = ReadString(6);
            if (header.StartsWith(DEF_GIF_HEADER))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if EMF or WMF.
        /// </summary>
        /// <returns></returns>
        private bool CheckIfEmfOrWmf()
        {
            Reset();

            if (ReadInt32() == 1)
            {
                m_format = ImageFormat.Emf;
                return true;
            }
            else
            {
                Reset();

                if (ReadInt32() == unchecked((int)0x9AC6CDD7))
                {
                    m_format = ImageFormat.Wmf;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Parses the PNG image.
        /// </summary>
        private void ParsePngImage()
        {
#if SILVERLIGHT || WP
#if WINRT
            Reset();
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            Stream stream = randomAccessStream.AsStreamForWrite();
            if (m_stream is MemoryStream)
                stream.Write((m_stream as MemoryStream).ToArray(), 0, (int)m_stream.Length);
            else
                m_stream.CopyTo(stream);
            stream.Flush();
            randomAccessStream.Seek(0);
            Action action = new Action(
                delegate
                {
                    BitmapImage image = new BitmapImage();
                    image.SetSource(randomAccessStream as IRandomAccessStream);
                    m_width = image.PixelWidth;
                    m_height = image.PixelHeight;
                });
            UIDispatcher.Execute(action);
            randomAccessStream.Dispose();
#else
            /*System.Windows.Media.Imaging.BitmapImage image = new System.Windows.Media.Imaging.BitmapImage();
            image.SetSource(m_stream);
            m_width = image.PixelWidth;
            m_height = image.PixelHeight;*/
            byte[] temp;
            while (true)
            {
                if (m_stream.Position >= m_stream.Length - 1)
                {
                    break;
                }

                int length = ReadUInt32();
                string record = ReadString(4);

                if (record.Equals("IHDR"))
                {
                    m_width = ReadUInt32();
                    m_height = ReadUInt32();
                    break;
                }
                else
                {
                    temp = new byte[length];
                    m_stream.Read(temp, 0, length);
                }
            }
#endif
#else
            System.Drawing.Bitmap image = new Bitmap(m_stream);
            m_width = image.Width;
            m_height = image.Height;
#endif
        }

        /// <summary>
        /// Parses the JPEG image.
        /// </summary>
        private void ParseJpegImage()
        {
            Reset();
#if SILVERLIGHT || WP
#if WINRT
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            Stream stream = randomAccessStream.AsStreamForWrite();
            if (m_stream is MemoryStream)
                stream.Write((m_stream as MemoryStream).ToArray(), 0, (int)m_stream.Length);
            else
                m_stream.CopyTo(stream);
            stream.Flush();
            randomAccessStream.Seek(0);
            Action action = new Action(
                delegate
                {
                    BitmapImage image = new BitmapImage();
                    image.SetSource(randomAccessStream as IRandomAccessStream);
                    m_width = image.PixelWidth;
                    m_height = image.PixelHeight;
                });
            UIDispatcher.Execute(action);
            randomAccessStream.Dispose();
#else
            /*System.Windows.Media.Imaging.BitmapImage image = new System.Windows.Media.Imaging.BitmapImage();
            image.SetSource(m_stream);
            m_width = image.PixelWidth;
            m_height = image.PixelHeight;*/
            byte[] imgData = new byte[m_stream.Length];
            m_stream.Read(imgData, 0, imgData.Length);
            long i = 4;

            // Check for valid JPEG header
            if (imgData[i + 2] == 'J' && imgData[i + 3] == 'F' &&
                imgData[i + 4] == 'I' && imgData[i + 5] == 'F' &&
                imgData[i + 6] == 0)
            {
                long length = imgData[i] * 256 + imgData[i + 1];
                while (i < imgData.Length)
                {
                    i += length;
                    if (i >= imgData.Length)
                        break;

                    if (imgData[i + 1] == 192)
                    {
                        m_height = imgData[i + 5] * 256 + imgData[i + 6];
                        m_width = imgData[i + 7] * 256 + imgData[i + 8];
                        return;
                    }
                    else
                    {
                        i += 2;
                        length = imgData[i] * 256 + imgData[i + 1];
                    }
                }
            }
#endif
#else
            System.Drawing.Bitmap image = new Bitmap(m_stream);
            m_width = image.Width;
            m_height = image.Height;
#endif
        }

        /// <summary>
        /// Parses the GIF image.
        /// </summary>
        private void ParseGifImage()
        {
            m_width = ReadInt16();
            m_height = ReadInt16();
        }

        /// <summary>
        /// Parses the icon image.
        /// </summary>
        private void ParseIconImage()
        {
            Reset();
            byte[] temp = new byte[6];
            m_stream.Read(temp, 0, temp.Length);

            m_width = m_stream.ReadByte();
            m_height = m_stream.ReadByte();
        }

        /// <summary>
        /// Parses the EMF or WMF image.
        /// </summary>
        private void ParseEmfOrWmfImage()
        {
            Reset();
            byte[] temp;

            if (m_format == ImageFormat.Emf)
            {
                temp = new byte[16];
                m_stream.Read(temp, 0, temp.Length);
                m_width = ReadInt32();
                m_height = ReadInt32();
            }
            else if (Format == ImageFormat.Wmf)
            {
                temp = new byte[10];
                m_stream.Read(temp, 0, temp.Length);

                m_width = ReadShortLE();
                m_height = ReadShortLE();
            }
        }

        /// <summary>
        /// Parses tif image
        /// </summary>
        private void ParseTifImage() 
        {
            int defTiffWidth = 256;
            int defTiffHeight = 257;

            m_stream.Position = 4;
            int offset = ReadInt32();
            if (offset > m_stream.Length)
                throw new Exception("Tiff image file corrupted");

            m_stream.Position = offset + 2;
            while (offset < m_stream.Length) 
            {
                int tag = ReadInt16();
                int type = ReadInt16();
                int count = ReadInt32();
                int value = ReadInt32();

                if (tag == defTiffWidth)
                {
                    m_width = value;
                    continue;
                }

                if (tag == defTiffHeight)
                {
                    m_height = value;
                    continue;
                }

                if (m_height != 0 && m_width != null)
                    break;
            }

        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Reads the Uint32.
        /// </summary>
        /// <returns></returns>
        private int ReadUInt32()
        {
            byte[] buffer = new byte[4];
            m_stream.Read(buffer, 0, 4);

            return ((buffer[0] << 24) + (buffer[1] << 16) + (buffer[2] << 8) + buffer[3]);
        }

        /// <summary>
        /// Reads the int32.
        /// </summary>
        /// <returns></returns>
        private Int32 ReadInt32()
        {
            byte[] buffer = new byte[4];
            m_stream.Read(buffer, 0, 4);

            return (buffer[0] + (buffer[1] << 8) + (buffer[2] << 16) + (buffer[3] << 24));
        }

        /// <summary>
        /// Reads the Uint16.
        /// </summary>
        /// <returns></returns>
        private int ReadUInt16()
        {
            byte[] buffer = new byte[2];
            m_stream.Read(buffer, 0, 2);

            return (buffer[0] << 8) + buffer[1];
        }

        /// <summary>
        /// Reads the int16.
        /// </summary>
        /// <returns></returns>
        private int ReadInt16()
        {
            byte[] buffer = new byte[2];
            m_stream.Read(buffer, 0, 2);

            return buffer[0] | (buffer[1] << 8);
        }

        /// <summary>
        /// Reads the word.
        /// </summary>
        /// <returns></returns>
        private int ReadWord()
        {
            int num = m_stream.ReadByte();
            return (num + (m_stream.ReadByte() << 8)) & 0xffff;
        }

        /// <summary>
        /// Reads the short LE.
        /// </summary>
        /// <returns></returns>
        private int ReadShortLE()
        {
            int num = ReadWord();
            if (num > 0x7fff)
                num -= 0x10000;
            return num;
        }

        /// <summary>
        /// Reads the string.
        /// </summary>
        /// <param name="len">The len.</param>
        /// <returns></returns>
        private string ReadString(int len)
        {
            char[] chars = new char[len];

            for (int i = 0; i < len; i++)
            {
                chars[i] = (char)m_stream.ReadByte();
            }

            return new string(chars);
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        private void Reset()
        {
            m_stream.Position = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        internal static Image FromStream(MemoryStream memoryStream)
        {
            return new Image(memoryStream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <param name="imageFormat"></param>
        internal void Save(MemoryStream memoryStream, ImageFormat imageFormat)
        {

        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_imageData = null;

            if (m_stream != null)
            {
                m_stream.Dispose();
                m_stream = null;
            }
        }
        #endregion
    }

    /// <summary>
    /// Specifies the image format.
    /// </summary>
    internal enum ImageFormat
    {
        Unknown,
        Bmp,
        Emf,
        Gif,
        Jpeg,
        Png,
        Wmf,
        Icon,
        Tiff,
        MemoryBmp
    }
}

