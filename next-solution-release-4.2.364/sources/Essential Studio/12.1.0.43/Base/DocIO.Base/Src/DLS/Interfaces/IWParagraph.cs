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

using System;
using System.IO;
using System.Text.RegularExpressions;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.DLS.Entities;

#if !SILVERLIGHT && !WP
using Image = System.Drawing.Image;
#endif

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a paragraph inside a Section.
    /// </summary>
    public interface IWParagraph : ITextBodyItem, IStyleHolder, ICompositeEntity
    {
        /// <summary>
        /// Gets / sets paragraph text.
        /// </summary>
        string Text
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.ParagraphItem"/> at the specified index.
        /// </summary>
        /// <value></value>
        ParagraphItem this[int index]
        {
            get;
        }
        /// <summary>
        /// Gets the paragraph items.
        /// </summary>
        /// <value>The items.</value>
        ParagraphItemCollection Items
        {
            get;
        }
        /// <summary>
        /// Gets the paragraph format.
        /// </summary>
        WParagraphFormat ParagraphFormat
        {
            get;
        }
        /// <summary>
        /// Gets list formatting for the paragraph.
        /// </summary>
        WListFormat ListFormat
        {
            get;
        }
        /// <summary>
        /// Gets the character format for the break symbol.
        /// </summary>
        WCharacterFormat BreakCharacterFormat
        {
            get;
        }
        /// <summary>
        /// Gets a value indicating whether this paragraph is in cell.
        /// </summary>
        /// <value>
        /// 	if this paragraph is in cell, set to <c>true</c>.
        /// </value>
        bool IsInCell
        {
            get;
        }
        /// <summary>
        /// Gets a value indicating whether this paragraph is end of section.
        /// </summary>
        /// <value>
        /// 	 if this paragraph is end of section, set to <c>true</c>.
        /// </value>
        bool IsEndOfSection
        {
            get;
        }
        /// <summary>
        /// Gets a value indicating whether this paragraph is end of document.
        /// </summary>
        /// <value>
        /// 	if this instance is end of document, set to <c>true</c>.
        /// </value>
        bool IsEndOfDocument
        {
            get;
        }
        /// <summary>
        /// Appends text to the end of paragraph.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        IWTextRange AppendText(string text);

#if SILVERLIGHT || WP
    /// <summary>
    /// Appends picture to the end of paragraph.
    /// </summary>
    /// <param name="image">The image stream.</param>
    /// <returns></returns>    
    IWPicture AppendPicture( Stream imageStream );
#else
        /// <summary>
        /// Appends picture to the end of paragraph.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        IWPicture AppendPicture(Image image);
#endif
        /// <summary>
        /// Appends picture to the end of paragraph.
        /// </summary>
        /// <param name="imageBytes">The image bytes.</param>
        /// <returns></returns>
        IWPicture AppendPicture(byte[] imageBytes);
        /// <summary>
        /// Appends field to the end of paragraph
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <returns></returns>
        IWField AppendField(string fieldName, FieldType fieldType);
        /// <summary>
        /// Appends start of the bookmark with specified name into paragraph.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        BookmarkStart AppendBookmarkStart(string name);
        /// <summary>
        /// Appends end of the bookmark with specified name into paragraph.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        BookmarkEnd AppendBookmarkEnd(string name);
        /// <summary>
        /// Appends the comment.
        /// </summary>
        /// <returns></returns>
        WComment AppendComment(string text);
        /// <summary>
        /// Appends the footnote.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        WFootnote AppendFootnote(FootnoteType type);
        /// <summary>
        /// Appends textbox to the end of the paragraph
        /// </summary>
        /// <param name="width">Textbox width</param>
        /// <param name="height">Textbox height</param>
        /// <returns></returns>
        IWTextBox AppendTextBox(float width, float height);
        /// <summary>
        /// Appends symbol to the end of paragraph.
        /// </summary>
        /// <param name="characterCode">The character code.</param>
        /// <returns></returns>
        WSymbol AppendSymbol(byte characterCode);
        /// <summary>
        /// Appends break to end of paragraph.
        /// </summary>
        /// <param name="breakType">Type of the break.</param>
        Break AppendBreak(BreakType breakType);
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Appends the HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        void AppendHTML(string html);
#endif
        /// <summary>
        /// Gets related style.
        /// </summary>
        /// <returns></returns>
        IWParagraphStyle GetStyle();
        /// <summary>
        /// Replaces all entries of given string with TextRangesHolder, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="caseSensitive">if it specifies case sensitive search, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies the whole word to be replace, set to <c>true</c>.</param>
        int Replace(string given, TextSelection textSelection, bool caseSensitive, bool wholeWord);
        /// <summary>
        /// Appends the check box.
        /// </summary>
        /// <returns></returns>
        WCheckBox AppendCheckBox();
        /// <summary>
        /// Appends the text form field.
        /// </summary>
        /// <param name="defaultText">The default text.</param>
        /// <returns></returns>
        WTextFormField AppendTextFormField(string defaultText);
        /// <summary>
        /// Appends the drop down form field.
        /// </summary>
        /// <returns></returns>
        WDropDownFormField AppendDropDownFormField();
        /// <summary>
        /// Appends the check box.
        /// </summary>
        /// <param name="checkBoxName">Name of the check box.</param>
        /// <param name="defaultCheckBoxValue">Default checkbox value.</param>
        /// <returns></returns>
        WCheckBox AppendCheckBox(string checkBoxName, bool defaultCheckBoxValue);
        /// <summary>
        /// Appends the text form field.
        /// </summary>
        /// <param name="formFieldName">Name of the form field.</param>
        /// <param name="defaultText">The default text.</param>
        /// <returns></returns>
        WTextFormField AppendTextFormField(string formFieldName, string defaultText);
        /// <summary>
        /// Appends the drop down form field.
        /// </summary>
        /// <param name="dropDropDownName">Name of the drop drop down.</param>
        /// <returns></returns>
        WDropDownFormField AppendDropDownFormField(string dropDropDownName);
        /// <summary>
        /// Appends the hyperlink.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="type">The hyperlink type.</param>
        /// <returns></returns>
        IWField AppendHyperlink(string link, string text, HyperlinkType type);
        /// <summary>
        /// Appends the hyperlink.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="picture">The picture to display.</param>
        /// <param name="type">The type of hyperlink.</param>
        /// <returns></returns>
        IWField AppendHyperlink(string link, WPicture picture, HyperlinkType type);
        /// <summary>
        /// Removes the absolute position data. If paragraph has absolute position in the document,
        /// all position data will be erased.
        /// </summary>
        void RemoveAbsPosition();
        /// <summary>
        /// Appends the TOC.
        /// </summary>
        /// <param name="lowerHeadingLevel">The lower heading level.</param>
        /// <param name="upperHeadingLevel">The upper heading level.</param>
        /// <returns></returns>
        TableOfContent AppendTOC(int lowerHeadingLevel, int upperHeadingLevel);
        /// <summary>
        /// Appends the OLE object.
        /// </summary>
        /// <param name="oleStream">The OLE stream.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <returns></returns>
        WOleObject AppendOleObject(Stream oleStream, WPicture olePicture, OleObjectType type);
        /// <summary>
        /// Appends the OLE object.
        /// </summary>
        /// <param name="oleBytes">The OLE bytes.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <returns></returns>
        WOleObject AppendOleObject(byte[] oleBytes, WPicture olePicture, OleObjectType type);
        /// <summary>
        /// Appends the OLE object into paragraph.
        /// </summary>
        /// <param name="oleStorage">The OLE storage.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <param name="oleLinkType">The type of OLE object link type.</param>
        /// <param name="oleLinkType"></param>
        /// <returns></returns>
        WOleObject AppendOleObject(Stream stream, WPicture pic, OleLinkType oleLinkType);
        /// <summary>
        /// Appends the OLE object.
        /// </summary>
        /// <param name="oleBytes">The OLE storage bytes.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <param name="oleLinkType">Type of the OLE link.</param>
        /// <returns></returns>
        WOleObject AppendOleObject(byte[] oleBytes, WPicture olePicture, OleLinkType oleLinkType);
        /// <summary>
        /// Appends the package OLE object (ole object without specified type).
        /// </summary>
        /// <param name="oleBytes">The OLE object bytes.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns></returns>
        WOleObject AppendOleObject(byte[] oleBytes, WPicture olePicture, string fileExtension);
        /// <summary>
        /// Appends the package OLE object (ole object without specified type).
        /// </summary>
        /// <param name="oleStream">The OLE file stream.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns></returns>
        WOleObject AppendOleObject(Stream oleStream, WPicture olePicture, string fileExtension);
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Appends the OLE object.
        /// </summary>
        /// <param name="pathToFile">The path to file.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <param name="oleObjectFileType">Type of the OLE object file.</param>
        /// <returns></returns>
        WOleObject AppendOleObject(string pathToFile, WPicture olePicture, OleObjectType type);
        /// <summary>
        /// Appends the OLE object.
        /// </summary>
        /// <param name="pathToFile">The path to file.</param>
        /// <param name="olePicture">The OLE picture.</param>
        /// <returns></returns>
        WOleObject AppendOleObject(string pathToFile, WPicture olePicture);
#endif
        /// <summary>
        /// Inserts the section break.
        /// Creates new section with the break type new page.
        /// </summary>
        /// <returns></returns>
        WSection InsertSectionBreak();
        /// <summary>
        /// Inserts the section break.
        /// Creates new section with the specified break type.
        /// </summary>
        /// <param name="breakType">Type of the break.</param>
        /// <returns></returns>
        WSection InsertSectionBreak(SectionBreakCode breakType);
    }
}