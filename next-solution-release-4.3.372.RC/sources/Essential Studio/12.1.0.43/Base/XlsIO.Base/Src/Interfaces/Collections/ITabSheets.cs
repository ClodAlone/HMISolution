#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO
{
	/// <summary>
	/// Summary description for ITabSheets.
	/// </summary>
	public interface ITabSheets
	{
    /// <summary>
    /// Returns number of elements in the collection. Read-only.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns single entry from the collection by its index. Read-only.
    /// </summary>
    ITabSheet this[ int index ] { get; }
//    /// <summary>
//    /// Returns single entry from the collection by its name. Read-only.
//    /// </summary>
//    ITabSheet this[ string strTabSheetName ] { get; }
//
    /// <summary>
    /// Moves tab sheet into new location.
    /// </summary>
    /// <param name="iOldIndex">Index of the tab sheet to move.</param>
    /// <param name="iNewIndex">Desired new index.</param>
    void Move( int iOldIndex, int iNewIndex );
    /// <summary>
    /// Moves specified tab sheet before another tab sheet.
    /// </summary>
    /// <param name="sheetToMove">The tab sheet to move.</param>
    /// <param name="sheetForPlacement">The tab sheet to locate new position.</param>
    void MoveBefore( ITabSheet sheetToMove, ITabSheet sheetForPlacement );
    /// <summary>
    /// Moves specified tab sheet after another tab sheet.
    /// </summary>
    /// <param name="sheetToCopy">The tab sheet to move.</param>
    /// <param name="sheetForPlacement">The tab sheet to locate new position.</param>
    void MoveAfter( ITabSheet sheetToCopy, ITabSheet sheetForPlacement );
	}
}
