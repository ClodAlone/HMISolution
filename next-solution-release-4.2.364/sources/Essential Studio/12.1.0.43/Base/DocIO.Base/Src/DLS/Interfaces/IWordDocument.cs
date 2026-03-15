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
using System.IO;
using System.Text.RegularExpressions;
using Syncfusion.DocIO.DLS.Entities;
#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
#else
using Image = System.Drawing.Image;
using System.Web;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents the MS Word Document.
    /// </summary>
    public interface IWordDocument : ICompositeEntity
    {
        #region Properties
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets document's built-in properties.
        /// </summary>
        BuiltinDocumentProperties BuiltinDocumentProperties
        {
            get;
        }
        /// <summary>
        /// Gets document's custom properties.
        /// </summary>
        CustomDocumentProperties CustomDocumentProperties
        {
            get;
        }
#endif
        /// <summary>
        /// Gets collection of sections of this document.
        /// </summary>
        WSectionCollection Sections
        {
            get;
        }
        /// <summary>
        /// Gets collection of styles.
        /// </summary>
        IStyleCollection Styles
        {
            get;
        }
        /// <summary>
        /// Gets collection of list styles.
        /// </summary>
        ListStyleCollection ListStyles
        {
            get;
        }
        /// <summary>
        /// Gets collection of bookmarks of the document
        /// </summary>
        BookmarkCollection Bookmarks
        {
            get;
        }
        /// <summary>
        /// Get/set document textboxes 
        /// </summary>
        TextBoxCollection TextBoxes
        {
            get;
        }
        /// <summary>
        /// Gets Collection of Comments of the document.
        /// </summary>
        CommentsCollection Comments
        {
            get;
        }
        /// <summary>
        /// Gets last section object.
        /// </summary>
        WSection LastSection
        {
            get;
        }
        /// <summary>
        /// Gets the last paragraph.
        /// </summary>
        /// <value>The last paragraph.</value>
        WParagraph LastParagraph
        {
            get;
        }
        /// <summary>
        /// Gets / sets image that represents background image of a document.
        /// </summary>
#if SILVERLIGHT || WP
    byte[] BackgroundImage
#else
        Image BackgroundImage
#endif
        {
            get;
            set;
        }
        /// <summary>
        /// Gets/sets the type of protection of the document.
        /// </summary>
        ProtectionType ProtectionType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets / sets view type in MSWord.
        /// </summary>
        ViewSetup ViewSetup
        {
            get;
        }
        /// <summary>
        /// Gets/sets watermark for the document.
        /// </summary>
        Watermark Watermark
        {
            get;
            set;
        }
        /// <summary>
        /// Gets mail merge engine.
        /// </summary>
        MailMerge MailMerge
        {
            get;
        }
        /// <summary>
        /// Gets background for the document.
        /// </summary>
        Background Background
        {
            get;
        }
        /// <summary>
        /// Gets or sets the document variables.
        /// </summary>
        /// <value>The variables.</value>
        DocVariables Variables
        {
            get;
        }
        /// <summary>
        /// Gets the document properties.
        /// </summary>
        /// <value>The properties.</value>
        DocProperties Properties
        {
            get;
        }
        /// <summary>
        /// Gets a value indicating whether the document has tracked changes.
        /// </summary>
        /// <value>
        /// 	if the document has tracked changes, set to <c>true</c>.
        /// </value>
        bool HasChanges
        {
            get;
        }
        /// <summary>
        /// Gets a value indicating whether the document has macros.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the document has macros; otherwise, <c>false</c>.
        /// </value>
        bool HasMacros
        {
            get;
        }
        /// <summary>
        /// Gets or sets a value indicating whether to update fields in the document.
        /// </summary>
        /// <value>if update fields, set to <c>true</c>.</value>
        [ObsoleteAttribute("This property has been deprecated. Use the UpdateDocumentFields method of WordDocument class to update the fields in the document.")]
        bool UpdateFields
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// If the document contains no sections, creates one section 
        /// with one paragraph. 
        /// </summary>
        void EnsureMinimal();
        /// <summary>
        /// Adds new section to document.
        /// </summary>
        /// <returns></returns>
        IWSection AddSection();
        /// <summary>
        /// Adds new paragraph style to document.
        /// </summary>
        /// <param name="styleName">Paragraph style name</param>
        /// <returns></returns>
        IWParagraphStyle AddParagraphStyle(string styleName);
        /// <summary>
        /// Adds new list style to document.
        /// </summary>
        /// <param name="listType">Type of the list style.</param>
        /// <param name="styleName">List style name</param>
        /// <returns></returns>
        ListStyle AddListStyle(ListType listType, string styleName);
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets the document's text.
        /// </summary>
        /// <returns></returns>
        string GetText();
        /// <summary>
        /// Converts the whole document into images
        /// </summary>
        /// <param name="type">The ImageType</param>
        /// <returns>Return the images</returns>
        /// <remarks>Layouting of the pages is not exactly the same as the layouting made by Microsoft Word. The total number of pages and layouting of the elements may vary.</remarks>
        Image[] RenderAsImages(ImageType type);
        /// <summary>
        /// Converts the specified page into image
        /// </summary>
        /// <param name="pageIndex">Zero based page index</param>
        /// <param name="imageFormat">The ImageFormat</param>
        /// <returns>Returns the image as stream</returns>
        /// <remarks>Layouting of the pages is not exactly the same as the layouting made by MS-Word. The total number of pages and layouting of the elements may vary.</remarks>
        Stream RenderAsImages(int pageIndex, System.Drawing.Imaging.ImageFormat imageFormat);
        /// <summary>
        /// Converts the specified page into image
        /// </summary>
        /// <param name="pageIndex">Zero based page index</param>
        /// <param name="type"> The ImageType</param>
        /// <returns>Returns the image</returns>
        /// <remarks>Layouting of the pages is not exactly the same as the layouting made by MS-Word. The total number of pages and layouting of the elements may vary.</remarks>
        Image RenderAsImages(int pageIndex, ImageType type);
        /// <summary>
        /// Converts the specified range of pages into images
        /// </summary>
        /// <param name="pageIndex">Starting page index (Zero based)</param>
        /// <param name="noOfPages">Number of pages</param>
        /// <param name="type">The ImageType</param>
        /// <returns>Return the images</returns>
        /// <remarks>Layouting of the pages is not exactly the same as the layouting made by MS-Word. The total number of pages and layouting of the elements may vary.</remarks>
        Image[] RenderAsImages(int pageIndex, int noOfPages, ImageType type);
#endif
        /// <summary>
        /// Creates the paragraph.
        /// </summary>
        /// <returns></returns>
        IWParagraph CreateParagraph();
        /// <summary>
        /// Make deep copy of word document.
        /// </summary>
        /// <returns></returns>
        new WordDocument Clone();
        /// <summary>
        /// Adds the style to the document style.
        /// </summary>
        /// <param name="builtinStyle">The built-in style.</param>
        IStyle AddStyle(BuiltinStyle builtinStyle);
        /// <summary>
        /// Protects the document.
        /// </summary>
        /// <param name="type">The type of the protection.</param>
        void Protect(ProtectionType type);
        /// <summary>
        /// Protects the document.
        /// </summary>
        /// <param name="type">The type of the protection</param>
        /// <param name="password">The password used for protection.</param>
        void Protect(ProtectionType type, string password);
        /// <summary>
        /// Encrypts the document.
        /// </summary>
        /// <param name="password">The password.</param>
        void EncryptDocument(string password);
        /// <summary>
        /// Removes the encryption.
        /// </summary>
        void RemoveEncryption();
        /// <summary>
        /// Update fields present in the document.
        /// </summary>
        void UpdateDocumentFields();
        /// <summary>
        /// Removes the macros in the document.
        /// </summary>
        void RemoveMacros();
        #if !SILVERLIGHT && !WP
        /// <summary>
        /// Update Paragraphs count, Word count and Character count
        /// </summary>
        void UpdateWordCount();
        /// <summary>
        /// Update Paragraphs count, Word count and Character count. Updates page count if performLayout set to true using Doc to PDF layouting engine.
        /// </summary>
        /// <param name="performRelayout">By default performLayout set to false, to update page count using Doc to PDF layouting engine, set to <c>true</c>.</param>
        void UpdateWordCount(bool performLayout);
        #endif
        #endregion

        #region Methods / find & replace
        /// <summary>
        /// Finds and returns entry of specified regular expression along with formatting.
        /// </summary>
        /// <param name="pattern"></param>
        TextSelection Find(Regex pattern);
        /// <summary>
        /// Finds and returns entry of specified string along with formatting,
        /// taking into consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="wholeWord"></param>
        /// <returns></returns>
        TextSelection Find(string given, bool caseSensitive, bool wholeWord);
        /// <summary>
        /// Finds the first entry of specified pattern in single-line mode.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        TextSelection[] FindSingleLine(Regex pattern);
        /// <summary>
        /// Finds the first entry of given text in single-line mode.
        /// </summary>
        /// <param name="given">The string to find.</param>
        /// <param name="caseSensitive">if it specifies case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies to search a whole word, set to <c>true</c>.</param>
        /// <returns></returns>
        TextSelection[] FindSingleLine(string given, bool caseSensitive, bool wholeWord);
        /// <summary>
        /// Finds and returns all entries of specified regular expression along with formatting.
        /// </summary>
        /// <param name="pattern"></param>
        TextSelection[] FindAll(Regex pattern);
        /// <summary>
        /// Finds and returns all entries of specified string along with formatting,
        /// taking into consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="wholeWord"></param>
        /// <returns></returns>
        TextSelection[] FindAll(string given, bool caseSensitive, bool wholeWord);
        /// <summary>
        /// Replaces all occurrences of a character pattern specified 
        /// by a regular expression with replace string.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        int Replace(Regex pattern, string replace);
        /// <summary>
        /// Replaces all entries of given string with replace string, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given"></param>
        /// <param name="replace"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="wholeWord"></param>
        int Replace(string given, string replace, bool caseSensitive, bool wholeWord);
        /// <summary>
        /// Replaces all entries of given regular expression with TextRangesHolder.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        int Replace(Regex pattern, TextSelection textSelection);
        /// <summary>
        /// Replaces all entries of given string with TextSelection, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="caseSensitive">if it specifies case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies to search whole word, set to <c>true</c>.</param>
        int Replace(string given, TextSelection textSelection, bool caseSensitive, bool wholeWord);
        /// <summary>
        /// Replaces all entries of given regular expression with TextBodyPart.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="bodyPart">The body part.</param>
        int Replace(Regex pattern, TextBodyPart bodyPart);
        /// <summary>
        /// Replaces all entries of given string with TextBodyPart, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="bodyPart">The body part.</param>
        /// <param name="caseSensitive">if it specifies case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies the whole word to be search, set to <c>true</c>.</param>
        /// <returns></returns>
        int Replace(string given, TextBodyPart bodyPart, bool caseSensitive, bool wholeWord);
        /// <summary>
        /// Replaces all entries of given string with TextRangesHolder, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="caseSensitive">if it specifies case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies the whole word to be search, set to <c>true</c>.</param>
        /// <param name="saveFormatting">if it specifies save formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        int Replace(string given, TextSelection textSelection, bool caseSensitive, bool wholeWord, bool saveFormatting);
        /// <summary>
        /// Replaces the specified given.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="bodyPart">The body part.</param>
        /// <param name="caseSensitive">if it specifies case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies the whole word to be search, set to <c>true</c>.</param>
        /// <param name="saveFormatting">if it specifies save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        int Replace(string given, TextBodyPart bodyPart, bool caseSensitive, bool wholeWord, bool saveFormatting);
        /// <summary>
        /// Replaces the specified given.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="replaceDoc">The replace doc.</param>
        /// <param name="caseSensitive">if it specifies case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies whole word to be search, set to <c>true</c>.</param>
        /// <param name="saveFormatting">if it specifies save source formatting, set to <c>true</c> .</param>
        /// <returns></returns>
        int Replace(string given, IWordDocument replaceDoc, bool caseSensitive, bool wholeWord, bool saveFormatting);
        /// <summary>
        /// Replaces all entries of given text with replace text in single-line mode.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="replace">The replace.</param>
        /// <param name="caseSensative">if it specifies case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies whole word to be replace, set to <c>true</c>.</param>
        /// <returns>The number of performed replaces.</returns>
        int ReplaceSingleLine(string given, string replace, bool caseSensitive, bool wholeWord);
        /// <summary>
        /// Replaces all entries with specified pattern with replace text in single-line mode.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replace">The replace.</param>
        /// <returns>The number of performed replaces.</returns>
        int ReplaceSingleLine(Regex pattern, string replace);
        /// <summary>
        /// Replaces the given text with replacement in single-line mode.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="replacement">The replacement.</param>
        /// <param name="caseSensitive">if it specifies case sensitive replace, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies whole word to be replace, set to <c>true</c>.</param>
        /// <returns>The number of performed replaces.</returns>
        int ReplaceSingleLine(string given, TextSelection replacement, bool caseSensitive, bool wholeWord);
        /// <summary>
        /// Replaces the given pattern with replacement in single-line mode.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>The number of performed replaces.</returns>
        int ReplaceSingleLine(Regex pattern, TextSelection replacement);
        /// <summary>
        /// Replaces the given text with specified replacement in single-line mode.
        /// </summary>
        /// <param name="given">The given text.</param>
        /// <param name="replacement">The replacement.</param>
        /// <param name="caseSensitive">if it specifies case sensitive replace,set to <c>true</c> .</param>
        /// <param name="wholeWord">if it specifies whole word to be replace, set to <c>true</c>.</param>
        /// <returns>The number of performed replaces.</returns>
        int ReplaceSingleLine(string given, TextBodyPart replacement, bool caseSensitive, bool wholeWord);
        /// <summary>
        /// Replaces the pattern with specified replacement in single-line mode.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>The number of performed replaces.</returns>
        int ReplaceSingleLine(Regex pattern, TextBodyPart replacement);
        /// <summary>
        /// Finds the next entry of given string, taking into consideration caseSensitive
        /// and wholeWord options.
        /// </summary>
        /// <param name="startTextBodyItem">The text body item at which search starts (paragraph or table).</param>
        /// <param name="given">The string to find.</param>
        /// <param name="caseSensitive">if it specifies case sensitive search, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies whole word to be search, set to <c>true</c>.</param>
        /// <returns></returns>
        TextSelection FindNext(TextBodyItem startTextBodyItem, string given, bool caseSensitive, bool wholeWord);
        /// <summary>
        /// Finds the next entry of given pattern.
        /// </summary>
        /// <param name="startBodyItem">The start body item at which search starts (paragraph or table).</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        TextSelection FindNext(TextBodyItem startBodyItem, Regex pattern);
        /// <summary>
        /// Finds the next given text starting from specified.
        /// TextBodyItem using single-line mode.
        /// </summary>
        /// <param name="startTextBodyItem">The start text body item.</param>
        /// <param name="given">The given.</param>
        /// <param name="caseSensitive">if it specifies case sensitive search, set to <c>true</c> .</param>
        /// <param name="wholeWord">if it specifies whole word to be search, set to <c>true</c>.</param>
        /// <returns></returns>
        TextSelection[] FindNextSingleLine(TextBodyItem startTextBodyItem, string given, bool caseSensitive,
          bool wholeWord);
        /// <summary>
        /// Finds the next text which fit the specified pattern starting from start TextBodyItem
        /// using single-line mode.
        /// </summary>
        /// <param name="startBodyItem">The start body item.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        TextSelection[] FindNextSingleLine(TextBodyItem startBodyItem, Regex pattern);
        /// <summary>
        /// Resets the FindNext.
        /// </summary>
        void ResetFindNext();
        #endregion

        #region Methods / open & save
        /// <summary>
        /// Opens the document from stream in Xml or Microsoft Word format.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="formatType"></param>
        void Open(Stream stream, FormatType formatType);
        /// <summary>
        /// Saves the document to a stream in Xml or Microsoft Word format.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="formatType"></param>
        void Save(Stream stream, FormatType formatType);

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Opens the document from file in Microsoft Word format.
        /// </summary>
        /// <param name="fileName">File name</param>
        void Open(string fileName);
        /// <summary>
        /// Opens the document from file in Xml or Microsoft Word format.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="formatType"></param>
        void Open(string fileName, FormatType formatType);
#if !CLIENTPROFILE
        /// <summary>
        /// Streams the document to the client browser.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="formatType"></param>
        /// <param name="response"></param>
        /// <param name="contentDisposotion"></param>
        void Save(string fileName, FormatType formatType, HttpResponse response,
                   HttpContentDisposition contentDisposotion);
#endif
        /// <summary>
        /// Open new document in read-only mode.
        /// </summary>
        /// <param name="strFileName">File to open.</param>
        /// <param name="formatType">Type of the format.</param>   
        void OpenReadOnly(string strFileName, FormatType formatType);
        /// <summary>
        /// Saves the document to file in Microsoft Word format.
        /// </summary>
        /// <param name="fileName">File name</param>
        void Save(string fileName);
        /// <summary>
        /// Saves the document to file in Xml or Microsoft Word format.
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="formatType">Type of the format</param>
        void Save(string fileName, FormatType formatType);
#endif
        #if MVC
        /// <summary>
        /// Save as ActionResult
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="formatType">The Type.</param>
        /// <param name="response">The Response.</param>
        /// <param name="contentDisposition">The Content.</param>
        /// <returns></returns>
        DocumentResult SaveAsActionResult(string fileName, FormatType formatType, HttpResponse response,
          HttpContentDisposition contentDisposition);
        #endif
        #endregion

        #region Methods / import
        /// <summary>
        /// Imports all content into the document.
        /// </summary>
        /// <param name="doc">The doc.</param>
        void ImportContent(IWordDocument doc);
        /// <summary>
        /// Imports all content into document.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="importStyles">If the document styles which have same names will be
        ///  also imported to the destination document, set to <c>true</c>.</param>
        void ImportContent(IWordDocument doc, bool importStyles);
        #endregion
    }
}