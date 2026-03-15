#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using Syncfusion.CompoundFile.XlsIO;

namespace Syncfusion.XlsIO
{
	/// <summary>
	/// Represents user interface for built-in document properties.
	/// </summary>
	public interface IBuiltInDocumentProperties
	{
    #region Class properties
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    IDocumentProperty this[ ExcelBuiltInProperty index ] { get; }
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    IDocumentProperty this[ int iIndex ] { get; }
    /// <summary>
    /// Returns number of elements in the collection. Read-only.
    /// </summary>
    int Count { get; }
    #endregion

    #region Class methods
    /// <summary>
    /// Removes all properties from the collection.
    /// </summary>
    void Clear();
    /// <summary>
    /// Indicates whether collection contains specified property.
    /// </summary>
    /// <param name="index">Property id.</param>
    /// <returns>True if collection contains required property.</returns>
    bool Contains( ExcelBuiltInProperty index );
    #endregion

    #region Typed property values
    /// <summary>
    /// Title document property.
    /// </summary>
    string Title { get; set; }
    /// <summary>
    /// Subject document property.
    /// </summary>
    string Subject { get; set; }
    /// <summary>
    /// Author document property.
    /// </summary>
    string Author { get; set; }
    /// <summary>
    /// Keywords document property.
    /// </summary>
    string Keywords { get; set; }
    /// <summary>
    /// Comments document property.
    /// </summary>
    string Comments { get; set; }
    /// <summary>
    /// Template document property.
    /// </summary>
    string Template { get; set; }
    /// <summary>
    /// LastAuthor document property.
    /// </summary>
    string LastAuthor { get; set; }
    /// <summary>
    /// Revnumber document property.
    /// </summary>
    string RevisionNumber { get; set; }
    /// <summary>
    /// EditTime document property.
    /// </summary>
    TimeSpan EditTime { get; set; }
    /// <summary>
    /// LastPrinted document property.
    /// </summary>
    DateTime LastPrinted { get; set; }
    /// <summary>
    /// CreationDate document property.
    /// </summary>
    DateTime CreationDate { get; set; }
    /// <summary>
    /// LastSaveDate document property.
    /// </summary>
    DateTime LastSaveDate { get; set; }
    /// <summary>
    /// PageCount document property.
    /// </summary>
    int PageCount { get; set; }
    /// <summary>
    /// WordCount document property.
    /// </summary>
    int WordCount { get; set; }
    /// <summary>
    /// CharCount document property.
    /// </summary>
    int CharCount { get; set; }
    //    /// <summary>
    //    /// Thumbnail document property.
    //    /// </summary>
    //Thumbnail { get; set; }
    /// <summary>
    /// ApplicationName document property.
    /// </summary>
    string ApplicationName { get; set; }
//    /// <summary>
//    /// Security document property.
//    /// </summary>
//    int Security { get; set; }
    /// <summary>
    /// Category.
    /// </summary>
    string Category { get; set; }
    /// <summary>
    /// Target format for presentation (35mm, printer, video, and so on).
    /// </summary>
    string PresentationTarget { get; set; }
    /// <summary>
    /// ByteCount.
    /// </summary>
    int ByteCount { get; set; }
    /// <summary>
    /// LineCount.
    /// </summary>
    int LineCount { get; set; }
    /// <summary>
    /// ParCount.
    /// </summary>
    int ParagraphCount { get; set; }
    /// <summary>
    /// SlideCount.
    /// </summary>
    int SlideCount { get; set; }
    /// <summary>
    /// NoteCount.
    /// </summary>
    int NoteCount { get; set; }
    /// <summary>
    /// HiddenCount.
    /// </summary>
    int HiddenCount { get; set; }
    /// <summary>
    /// MmclipCount.
    /// </summary>
    int MultimediaClipCount { get; set; }
//    /// <summary>
//    /// Set to True when scaling of the thumbnail is desired. If not set, cropping is desired.
//    /// </summary>
//    bool ScaleCrop { get; set; }
    //    /// <summary>
    //    /// HeadingPair.
    //    /// </summary>
    //HeadingPair { get; set; }
    //    /// <summary>
    //    /// DocParts.
    //    /// </summary>
    //DocParts { get; set; }
    /// <summary>
    /// Manager.
    /// </summary>
    string Manager { get; set; }
    /// <summary>
    /// Company.
    /// </summary>
    string Company { get; set; }
    /// <summary>
    /// Boolean value to indicate whether the custom links are
    /// hampered by excessive noise, for all applications.
    /// </summary>
    bool LinksDirty { get; set; }
    #endregion
  }
}
