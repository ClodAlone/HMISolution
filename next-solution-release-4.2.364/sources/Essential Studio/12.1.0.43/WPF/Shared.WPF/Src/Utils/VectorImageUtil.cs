// <copyright file="VectorImageUtil.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Windows.Documents;

namespace Syncfusion.Windows.Shared
{

    /// <summary>
    /// Class that used to export vector images into Images. 
    /// </summary>
    public class VectorImageUtil
    {
        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="element">The element.</param>
        public static void SaveImage(string fileName, FrameworkElement element)
        {
            string imageExtension = null;
            imageExtension = new FileInfo(fileName).Extension.ToLower(CultureInfo.InvariantCulture);

            BitmapEncoder imgEncoder = null;
            switch (imageExtension)
            {
                case ".bmp":
                    imgEncoder = new BmpBitmapEncoder();
                    break;

                case ".jpg":
                case ".jpeg":
                    imgEncoder = new JpegBitmapEncoder();
                    break;

                case ".png":
                    imgEncoder = new PngBitmapEncoder();
                    break;

                case ".gif":
                    imgEncoder = new GifBitmapEncoder();
                    break;

                case ".tif":
                case ".tiff":
                    imgEncoder = new TiffBitmapEncoder();
                    break;

                case ".wdp":
                    imgEncoder = new WmpBitmapEncoder();
                    break;

                default:
                    imgEncoder = new BmpBitmapEncoder();
                    break;
            }

            if (element != null)
            {
                RenderTargetBitmap bmpSource = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                //Rectangle backgroundRect = new Rectangle();

                //backgroundRect.Fill = imageExtension.Equals(".bmp") || imageExtension.Equals(".jpg") || imageExtension.Equals(".jpeg") ? Brushes.White : Brushes.Transparent;
                //backgroundRect.Arrange(new Rect(new Size(element.Width,element.Height)));

                //bmpSource.Render(backgroundRect);
                bmpSource.Render(element);

                imgEncoder.Frames.Add(BitmapFrame.Create(bmpSource));
                using (Stream stream = File.Create(fileName))
                {
                    imgEncoder.Save(stream);
                    stream.Close();
                }
            }
        }

        /// <summary>
        /// Saves the XAML.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="imageContent">Content of the image.</param>
        public static void SaveXAML(string fileName, ImageSource imageContent)
        {            
            StringBuilder outputString = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            XamlDesignerSerializationManager designSerialization = new XamlDesignerSerializationManager(XmlWriter.Create(fileName, settings));
            designSerialization.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(imageContent, designSerialization);
        }

        /// <summary>
        /// Imports the Xaml contents.
        /// </summary>
        /// <param name="imageSourceContent"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public static Visual ImportXaml(ImageSource imageSourceContent, FlowDocument document)
        {
            try
            {
                TextRange textRange = new TextRange(document.ContentStart, document.ContentEnd);
                StringReader strReader = new StringReader(textRange.Text.Replace("\r\n", ""));
                XmlReader xmlReader = XmlReader.Create(strReader);
                object xamlObject = XamlReader.Load(xmlReader);

                if (xamlObject is DrawingImage)
                {
                    DrawingImage drawingImg = xamlObject as DrawingImage;
                    imageSourceContent = drawingImg as ImageSource;

                    Image img = new Image();
                    img.Source = imageSourceContent;
                    return img;
                }
                else if (xamlObject is UIElement)
                {
                    UIElement element = xamlObject as UIElement;

                    if (element != null)
                        return element;
                }


            }
            catch (Exception )
            {

            }

            return null;
        }

    }
}
