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
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Syncfusion.DLS.Collections;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Interface publishes functionality of one document
  /// </summary>
  public interface IDocument : IEntityBase
  {
    /// <summary>
    /// Gets last section object.
    /// </summary>
    ISection LastSection{ get; }
    /// <summary>
    /// Gets last paragraph in last section object
    /// </summary>
    IParagraph LastParagraph{ get; }
    /// <summary>
    /// Gets collection of sections of this document.
    /// </summary>
    ISectionCollection Sections { get; }
    /// <summary>
    /// Gets collection of styles.
    /// </summary>
    IStyleCollection Styles { get; }
    /// <summary>
    /// Gets collection of list styles.
    /// </summary>
    ListStyleCollection ListStyles{ get; }
    /// <summary>
    /// Gets / sets image that represents background image of a document.
    /// </summary>
    Image BackgroundImage{ get; set; }
    /// <summary>
    /// If the document contains no sections, creates one section 
    /// with one paragraph. 
    /// </summary>
    void EnsureMinimal();
    /// <summary>
    /// Adds new section to document.
    /// </summary>
    /// <returns></returns>
    ISection AddSection();
    /// <summary>
    /// Adds new paragraph style to document.
    /// </summary>
    /// <param name="styleName">Paragraph style name</param>
    /// <returns></returns>
    IParagraphStyle AddParagraphStyle( string styleName );
    /// <summary>
    /// Adds new paragraph style to document.
    /// </summary>
    /// <param name="listType">List Type</param>
    /// <param name="styleName">Paragraph style name</param>
    /// <returns></returns>
    ListStyle AddListStyle( ListType listType, string styleName );
    /// <summary>
    /// Adds new style to document.
    /// </summary>
    /// <param name="styleType">Style type</param>
    /// <param name="styleName">Style name</param>
    /// <returns></returns>
    IStyle AddStyle( StyleType styleType, string styleName );
    /// <summary>
    /// Creates new pargraph instance.
    /// </summary>
    /// <returns></returns>
    IParagraph CreateParagraph();
    /// <summary>
    /// Creates new paragraph item instance.
    /// </summary>
    /// <param name="itemType">Paragraph item type</param>
    /// <returns></returns>
    IParagraphItem CreateParagraphItem( ParagraphItemType itemType );
    /// <summary>
    /// Creates new shape instance.
    /// </summary>
    /// <param name="shapeType"></param>
    /// <param name="canvas"></param>
    /// <returns></returns>
    Shape CreateShape( ShapeType shapeType, Canvas canvas );
    /// <summary>
    /// Open document from xml format file.
    /// </summary>
    /// <param name="fileName">The name of file</param>
    void OpenXml( string fileName );
    /// <summary>
    /// Save document in xml format.
    /// </summary>
    /// <param name="fileName">The name of target file</param>
    void SaveXml( string fileName );
    /// <summary>
    /// Open document from xml format file.
    /// </summary>
    /// <param name="stream">The stream object</param>
    void OpenXml( Stream stream );
    /// <summary>
    /// Save document in xml format.
    /// </summary>
    /// <param name="stream">The stream object</param>
    void SaveXml( Stream stream );
    /// <summary>
    /// Makes deep copy of document.
    /// </summary>
    /// <returns></returns>
    IDocument Clone();
    /// <summary>
    /// Gets collection of bookmarks of the document
    /// </summary>
    BookmarkCollection Bookmarks{ get; }
    /// <summary>
    /// Replaces all occurrences of a character pattern specified 
    /// by a regular expression with replace string.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="replace"></param>
    /// <returns></returns>
    int Replace( Regex pattern, string replace );
    /// <summary>
    /// Replaces all entries of given string with replace string, taking into
    /// consideration caseSensitive and wholeWord options.
    /// </summary>
    /// <param name="given"></param>
    /// <param name="replace"></param>
    /// <param name="caseSensitive"></param>
    /// <param name="wholeWord"></param>
    int Replace( string given, string replace, bool caseSensitive, bool wholeWord );
    /// <summary>
    /// Finds and returns entry of specified regular expression along with formatting.
    /// </summary>
    /// <param name="pattern"></param>
    TextRangesHolder Find( Regex pattern );
    /// <summary>
    /// Finds and returns entry of specified string along with formatting,
    /// taking into consideration caseSensitive and wholeWord options.
    /// </summary>
    /// <param name="given"></param>
    /// <param name="caseSensitive"></param>
    /// <param name="wholeWord"></param>
    /// <returns></returns>
    TextRangesHolder Find( string given, bool caseSensitive, bool wholeWord );
    /// <summary>
    /// Finds and returns all entries of specified regular expression along with formatting.
    /// </summary>
    /// <param name="pattern"></param>
    TextRangesHolder[] FindAll( Regex pattern );
    /// <summary>
    /// Finds and returns all entries of specified string along with formatting,
    /// taking into consideration caseSensitive and wholeWord options.
    /// </summary>
    /// <param name="given"></param>
    /// <param name="caseSensitive"></param>
    /// <param name="wholeWord"></param>
    /// <returns></returns>
    TextRangesHolder[] FindAll( string given, bool caseSensitive, bool wholeWord );
    /// <summary>
    /// Replaces all entries of given regular expression with TextRangesHolder.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="rangesHolder"></param>
    /// <returns></returns>
    void Replace( Regex pattern, TextRangesHolder rangesHolder );
    /// <summary>
    /// Replaces all entries of given string with TextRangesHolder, taking into
    /// consideration caseSensitive and wholeWord options.
    /// </summary>
    /// <param name="given"></param>
    /// <param name="rangesHolder"></param>
    /// <param name="caseSensitive"></param>
    /// <param name="wholeWord"></param>
    void Replace( string given, TextRangesHolder rangesHolder, bool caseSensitive, bool wholeWord );
    /// <summary>
    /// Get/set document textboxes 
    /// </summary>
    TextBoxCollection TextBoxCollection { get; set; }
    /// <summary>
    /// Gets the document's text.
    /// </summary>
    /// <returns></returns>
    string GetText();
  }
}