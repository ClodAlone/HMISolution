#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;

namespace Syncfusion.XlsIO
{
	/// <summary>
	/// Represents a worksheet group.
	/// </summary>
	public interface IWorksheetGroup : IWorksheet
	{
    #region Interface properties
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    IWorksheet this[ int index ] { get; }
    /// <summary>
    /// Indicates whether collection is empty. Read-only.
    /// </summary>
    bool IsEmpty { get; }
    /// <summary>
    /// Number of selected worksheets.
    /// </summary>
    int Count { get; }
    #endregion

    #region Interface methods
    /// <summary>
    /// Adds new worksheet to the collection.
    /// </summary>
    /// <param name="sheet">Worksheet to add.</param>
    /// <returns>Index of the added worksheet.</returns>
    /// <exception cref="System.ArgumentNullException">When sheet is Null.</exception>
    int Add( ITabSheet sheet );
//    /// <summary>
//    /// Removes worksheet from the collection.
//    /// </summary>
//    /// <param name="sheet">Worksheet to remove.</param>
//    /// <exception cref="System.ArgumentNullException">When sheet is Null.</exception>
//    void Remove( ITabSheet sheet );
    #endregion
  }
}
