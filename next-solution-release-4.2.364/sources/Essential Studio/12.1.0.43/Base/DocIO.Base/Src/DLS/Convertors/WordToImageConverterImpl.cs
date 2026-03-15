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
using Syncfusion.Layouting;
using System.IO;
using System.Drawing.Imaging;
using Syncfusion.DocIO.DLS.Rendering;
using System.Drawing.Drawing2D;
using Syncfusion.DocIO.Rendering;

namespace Syncfusion.DocIO.DLS.Convertors
{
    class WordToImageConverter
    {
        #region Constructor
        public WordToImageConverter(int pageIndex, int pageCount, Stream stream, ImageFormat imageFormat)
        {
            if (imageFormat == ImageFormat.Emf && stream == null)
                stream = new MemoryStream();

           
           
        }
        public WordToImageConverter()
        { }

        public WordToImageConverter(int pageIndex, int pageCount, string fileName, ImageFormat imageFormat)
        {

        }
        #endregion

        #region Methods
        public Image[] ConvertToImage(WordDocument document, ImageType imageType)
        {
            return ConvertToImage(document, imageType, null);
        }


        public Image[] ConvertToImage(WordDocument document, ImageType imageType, MemoryStream stream)
        {
            DocumentLayouter layouter = new DocumentLayouter();
            layouter.Layout(document);
            return layouter.DrawToImage(0, -1, imageType, stream);
        }

        public Stream ConvertToImage(int pageIndex, WordDocument document, ImageFormat imageFormat)
        {
            MemoryStream stream = new MemoryStream();
            DocumentLayouter layouter = new DocumentLayouter();
            layouter.Layout(document);
            if (imageFormat == ImageFormat.Emf)
            {
                return layouter.DrawToStream(pageIndex, 1, ImageType.Metafile, null);
            }
            Image[] images = layouter.DrawToImage(pageIndex, 1, ImageType.Metafile, null);
            if (images != null && images[0] != null)
                images[0].Save(stream, imageFormat);
            else
                return null;
            return stream;
        }
        
        public Image[] ConvertToImage(int pageIndex, int noOfPages, WordDocument document, ImageType imageType)
        {
            DocumentLayouter layouter = new DocumentLayouter();
            layouter.Layout(document);
            return layouter.DrawToImage(pageIndex, noOfPages, imageType, null);
        }
        #endregion
    }

}
