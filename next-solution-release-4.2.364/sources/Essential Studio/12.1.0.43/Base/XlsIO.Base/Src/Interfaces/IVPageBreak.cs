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
using System.Collections;
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents a vertical page break. The VPageBreak object is a member
  /// of the VPageBreaks collection.
  /// </summary>
  public interface IVPageBreak
  {
    #region Not supported methods/properties
#if NOT_SUPPORTED
/*
    XlCreator Creator { get; }

    void DragOff( XlDirection Direction, int RegionIndex );
*/
#endif
    #endregion

    #region Skipped
#if SKIPPED
/*
    /// <summary>
    /// Returns the type of the specified page break: full-screen or only
    /// within a print area. Can be either of the following XlPageBreakExtent
    /// constants: xlPageBreakFull or xlPageBreakPartial. Read-only Long.
    /// </summary>
    XlPageBreakExtent Extent { get; }
    /// <summary>
    /// Returns or sets the page break type. Read / write XlPageBreak.
    /// </summary>
    XlPageBreak Type { get; set; }
    /// <summary>
    /// Deletes the object.
    /// </summary>
    void Delete();
*/
#endif
    #endregion

    #region Interface properties
    /// <summary>
    /// Used without an object qualifier, this property returns an
    /// Application object that represents the Excel application.
    /// </summary>
    IApplication  Application { get; }
    /// <summary>
    /// For the HPageBreak and VPageBreak objects, this property returns or
    /// sets the cell (a Range object) that defines the page-break location.
    /// Horizontal page breaks are aligned with the top edge of the location
    /// cell; vertical page breaks are aligned with the left edge of the
    /// location cell. Read / write Range.
    /// </summary>
    IRange        Location { get; set; }
    /// <summary>
    /// Returns the parent object for the specified object.
    /// </summary>
    object        Parent { get; }
    #endregion

    #region Interface methods
    #endregion
  }
}
