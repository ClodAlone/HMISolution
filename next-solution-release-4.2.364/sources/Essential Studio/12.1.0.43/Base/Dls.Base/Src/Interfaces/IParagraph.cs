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
using System.Text.RegularExpressions;

using Syncfusion.DLS.Collections;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Interface publishes paragraph functionality 
  /// </summary>
  public interface IParagraph : IEntityBase, IStyleHolder
  {
    /// <summary>
    /// Gets / sets paragraph text.
    /// </summary>
    string Text { get; set; }
    /// <summary>
    /// Gets paragraph item.
    /// </summary>
    IParagraphItem this[ int index ] { get; }
    /// <summary>
    /// Gets items count.
    /// </summary>
    int ItemsCount { get; }
    /// <summary>
    /// Gets paragraph format.
    /// </summary>
    ParagraphFormat ParagraphFormat { get; }
    /// <summary>
    /// Gets character format.
    /// </summary>
    CharacterFormat CharacterFormat { get; }
    /// <summary>
    /// Gets list formatting for the paragraph.
    /// </summary>
    ListFormat ListFormat{ get; }
    /// <summary>
    /// Append field to end of the paragraph.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IField AppendField( DLSFieldType type );
    /// <summary>
    /// Append Textbox to the end of the paragraph
    /// </summary>
    /// <param name="width">Textbox width</param>
    /// <param name="height">Textbox height</param>
    /// <returns></returns>
    ITextBox AppendTextBox( float width, float height );
    /// <summary>
    /// Appends text to end of paragraph.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ITextRange AppendText( string text );
    /// <summary>
    /// Appends picture to end of paragraph.
    /// </summary>
    /// <returns></returns>
    IPicture AppendPicture( Image image );
    /// <summary>
    /// Appends canvas to end of paragraph.
    /// </summary>
    /// <param name="size">Size of the canvas.</param>
    /// <returns></returns>
    ICanvas AppendCanvas( SizeF size );
    /// <summary>
    /// Appends table to end of paragraph.
    /// </summary>
    /// <returns></returns>
    ITable AppendTable();
    /// <summary>
    /// Appends start of the bookmark with specified name into paragraph.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    BookmarkStart AppendBookmarkStart( string name );
    /// <summary>
    /// Appends end of the bookmark with specified name into paragraph.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    BookmarkEnd AppendBookmarkEnd( string name );
    /// <summary>
    /// Gets related style.
    /// </summary>
    IParagraphStyle GetStyle();
    /// <summary>
    /// Inserts paragraph item to specified position.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pItem"></param>
    void InsertItem( int index, IParagraphItem pItem );
    /// <summary>
    /// Removes specified paragraph item.
    /// </summary>
    /// <param name="item"></param>
    void RemoveItem( IParagraphItem item );
    /// <summary>
    /// Removes paragraph item at specified position.
    /// </summary>
    /// <param name="index"></param>
    void RemoveItemAt( int index );
    /// <summary>
    /// Gets index of specified paragraph item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int IndexOfItem( IParagraphItem item );
    /// <summary>
    /// Clones itself and sets new owner document.
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    IParagraph Clone( IDocument doc );
    /// <summary>
    /// Replaces all entries of given regular expression with replace string.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="replace"></param>
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
  }
}