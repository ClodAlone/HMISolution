#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.IO;
using System.Drawing;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics.Images.Decoder
{
    internal abstract class ImageDecoder
    {
        #region Properties
        public Stream InternalStream { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public byte[] ImageData { get; set; }
        public int BitsPerComponent { get; set; }
        public ImageFormat Format { get; set; }
        public float HorizontalResolution { get; set; }
        public float VerticalResolution { get; set; }
        public Size Size
        {
            get
            {
                return new Size(Width, Height);
            }
        }
        #endregion

        #region Static Methods
        public static bool TryGetDecoder(Stream stream, out ImageDecoder decoder)
        {
            decoder = null;

            if (stream.IsPng())
                decoder = new PngDecoder(stream);
            else if (stream.IsJpeg())
                decoder = new JpegDecoder(stream);
            else if (stream.IsBmp())
                decoder = new BmpDecoder(stream);

            return (decoder != null);
        }
        #endregion

        #region Abstract Members
        protected abstract void Initialize();
        internal abstract PdfStream GetImageDictionary();
        #endregion
    }
}