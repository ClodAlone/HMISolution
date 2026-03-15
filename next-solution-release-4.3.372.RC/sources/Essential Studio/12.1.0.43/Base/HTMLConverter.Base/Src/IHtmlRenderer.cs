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
using Syncfusion.Pdf.HtmlToPdf;

namespace Syncfusion.HtmlConverter
{
    public interface IHtmlRenderer
    {
        //Image ConvertToImage(string url, ImageType type, int width, int height, AspectRatio aspectRatio);
        //Image ConvertToImage(string url, ImageType type, int width, int height);
        //Image ConvertToImage(string url, ImageType type, int width);
        //Image ConvertToImage(string url, ImageType type);

        //Image ConvertToImage(string url, ImageType type, int width, int height, AspectRatio aspectRatio, string username, string password);
        //Image ConvertToImage(string url, ImageType type, int width, int height, string username, string password);
        //Image ConvertToImage(string url, ImageType type, int width, string username, string password);
        //Image ConvertToImage(string url, ImageType type, string username, string password);

        //Image ConvertToImage(Stream stream, Encoding encoding, ImageType type, int width, int height, AspectRatio aspectRatio);
        //Image ConvertToImage(Stream stream, Encoding encoding, ImageType type, int width, int height);
        //Image ConvertToImage(Stream stream, Encoding encoding, ImageType type, int width);
        //Image ConvertToImage(Stream stream, Encoding encoding, ImageType type);

        //Image FromString(string html, string baseUrl, ImageType type, int width, int height, AspectRatio aspectRatio, string username, string password);
        //Image FromString(string html, string baseUrl, ImageType type, int width, int height, AspectRatio aspectRatio);
        //Image FromString(string html, string baseUrl, ImageType type, int width, int height);
        //Image FromString(string html, string baseUrl, ImageType type, int width);
        //Image FromString(string html, string baseUrl, ImageType type);

        //Image FromString(string html, ImageType type, int width, int height, AspectRatio aspectRatio);
        //Image FromString(string html, ImageType type, int width, int height);
        //Image FromString(string html, ImageType type, int width);
        
        //Image[] GetImagesFromString(string html, string baseUrl, ImageType type);
        //Image[] GetImagesFromString(string html, string baseUrl, ImageType type, int width, int height, AspectRatio aspectRatio);
        //Image[] GetImagesFromUrl(string url, ImageType type, int width, int height, AspectRatio aspectRatio);

        Stream GetDocumentImageStream(string url, double width, double height);
        Stream GetDocumentImageStream(string url, double width, double height, string username, string password);
        Stream GetDocumentImageStream(string htmlText, string baseUrl, double width, double height);
        Stream GetDocumentImageStream(string htmlText, string baseUrl, double width, double height, string username, string password);
    }
}
