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
	/// Represents range object for the single cell. Coordinates of such object can be changed.
	/// There should be only one such object for single worksheet.
	/// </summary>
	public interface IMigrantRange : IRange
	{
    /// <summary>
    /// Resets row and column values.
    /// </summary>
    /// <param name="iRow">One-based row index of the new cell address.</param>
    /// <param name="iColumn">One-based column index of the new cell address.</param>
    void ResetRowColumn( int iRow, int iColumn );
    void SetValue(int value);
    void SetValue(double value);
    void SetValue(DateTime value);
    void SetValue(bool value);
    void SetValue(string value);
  }
}
