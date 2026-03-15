#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using Syncfusion.DocIO;

#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
#else
using Image = System.Drawing.Image;
#endif
using System.IO;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a picture in a word document.
    /// </summary>
    public interface IWPicture : IParagraphItem
    {
        /// <summary>
        /// Gets / sets picture height.
        /// </summary>
        /// <remarks>
        /// The value is measured in points.
        /// </remarks>
        float Height
        {
            get;
            set;
        }
        /// <summary>
        /// Gets / sets picture width.
        /// </summary>
        /// /// <remarks>
        /// The value is measured in points.
        /// </remarks>
        float Width
        {
            get;
            set;
        }
        /// <summary>
        /// Gets / sets picture height scale factor in percent.
        /// </summary>
        float HeightScale
        {
            get;
            set;
        }
        /// <summary>
        /// Gets / sets picture width scale factor in percent.
        /// </summary>
        float WidthScale
        {
            get;
            set;
        }
        //    /// <summary>
        //    /// Get/set picture brightness.
        //    /// </summary>
        //    float Brightness{ get; set; }
        //    /// <summary>
        //    /// Get/set picture contrast.
        //    /// </summary>
        //    float Contrast { get; set; }
        //    /// <summary>
        //    /// Get/set picture color.
        //    /// </summary>
        //    PictureColor Color { get; set; }
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    float CropFromLeft { get; set; }
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    float CropFromRight { get; set; }
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    float CropFromTop { get; set; }
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    float CropFromBottom { get; set; }
#if SILVERLIGHT || WP
    /// <summary>
    /// Loads System.Drawing.Image object.
    /// </summary>
    /// <param name="image"></param>
    void LoadImage( Stream imageStream );
#else
        /// <summary>
        /// Loads System.Drawing.Image object.
        /// </summary>
        /// <param name="image"></param>
        void LoadImage(Image imageStream);

        /// <summary>
        /// Gets internal System.Drawing.Image object.
        /// </summary>
        Image Image
        {
            get;
        }
#endif
        /// <summary>
        /// Loads System.Drawing.Image as byte array.
        /// </summary>
        /// <param name="imageBytes"></param>
        void LoadImage(byte[] imageBytes);

        /// <summary>
        /// Gets image byte array.
        /// </summary>
        byte[] ImageBytes
        {
            get;
        }
        /// <summary>
        /// Add Caption for current Picture
        /// </summary>
        /// <param name="captionPosition"></param>
        /// <param name="name"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        IWParagraph AddCaption(string name, CaptionNumberingFormat format, CaptionPosition captionPosition);
        /// <summary>
        /// Gets \ sets horizontal origin of the picture.
        /// </summary>
        HorizontalOrigin HorizontalOrigin { get; set; }
        /// <summary>
        /// Gets \ sets vertical origin of the picture.
        /// </summary>
        VerticalOrigin VerticalOrigin { get; set; }
        /// <summary>
        /// Gets \ sets absolute horizontal position of the picture.
        /// </summary>
        /// <remarks>
        /// The value is measured in points and the position is relative to HorizontalOrigin.
        /// </remarks>
        float HorizontalPosition { get; set; }
        /// <summary>
        /// Gets \ sets absolute vertical position of the picture.
        /// </summary>
        /// <remarks>
        /// The value is measured in points and the position is relative to VerticalOrigin.
        /// </remarks>
        float VerticalPosition { get; set; }
        /// <summary>
        /// Gets \ sets text wrapping style of the picture.
        /// </summary>
        TextWrappingStyle TextWrappingStyle { get; set; }
        /// <summary>
        /// Gets \ sets text wrapping type of the picture.
        /// </summary>
        TextWrappingType TextWrappingType { get; set; }
        /// <summary>
        /// Gets \ sets whether picture is below image.
        /// </summary>
        bool IsBelowText { get; set; }
        /// <summary>
        /// Gets / sets picture horizontal alignment.
        /// </summary>
        /// <remarks>
        /// If it is set as None, then the object is explicitly positioned using position properties. Otherwise it is positioned according to the alignment specified. The position of the object is relative to HorizontalOrigin.
        /// </remarks>
        ShapeHorizontalAlignment HorizontalAlignment { get; set; }
        /// <summary>
        /// Gets / sets picture vertical alignment.
        /// </summary>
        /// <remarks>
        /// If it is set as None, then the object is explicitly positioned using position properties. Otherwise it is positioned according to the alignment specified. The position of the object is relative to VerticalOrigin.
        /// </remarks>
        ShapeVerticalAlignment VerticalAlignment { get; set; }
        /// <summary>
        /// Gets or sets the picture's alternative text.
        /// </summary>
        /// <value>The alternative text.</value>
        string AlternativeText { get; set; }
        /// <summary>
        /// Gets or sets the picture's title
        /// </summary>
        /// <value>The title text</value>
        string Title { get; set; }
    }
}
