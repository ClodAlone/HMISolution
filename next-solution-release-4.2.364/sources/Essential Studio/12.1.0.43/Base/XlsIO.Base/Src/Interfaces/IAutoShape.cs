#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.XlsIO
{
    /// <summary>
    /// Returns a TextFrame object that contains the 
    /// alignment and anchoring properties for the specified shape. Read-only.
    /// </summary>
    public interface ITextFrame
    {
        bool IsTextOverFlow { get; set; }
        bool WrapTextInShape { get; set; }
        bool IsAutoSize { get; set; }
        int MarginLeftPt { get; set; }
        int TopMarginPt { get; set; }
        int RightMarginPt { get; set; }
        int BottomMarginPt { get; set; }
        bool IsAutoMargins { get; set; }
        TextVertOverflowType TextVertOverflowType { get; set; }
        TextHorzOverflowType TextHorzOverflowType { get; set; }
        ExcelHorizontalAlignment HorizontalAlignment { get; set; }
        ExcelVerticalAlignment VerticalAlignment { get; set; }
        TextDirection TextDirection { get; set; }
        ITextRange TextRange { get; }
    }
    /// <summary>
    /// Returns the TextRange object that represents the text in the object. Read-only.
    /// </summary>
    public interface ITextRange
    {
        string Text { get; set; }
        /// <summary>
        /// Text of the comment. Read-only.
        /// </summary>
        IRichTextString RichText { get; }
    }
}
