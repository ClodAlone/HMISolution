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

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Collections;
#endregion

namespace Syncfusion.XlsIO.Interfaces
{
	/// <summary>
	/// This interface is used to get shapes from worksheet.
	/// </summary>
	public interface IShapeGetter : ICloneable
	{
    /// <summary>
    /// Returns a shape collection from the worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet to return sheet data for.</param>
    /// <returns>A shape collection from the worksheet.</returns>
    ShapeCollectionBase GetShapes( WorksheetBaseImpl sheet );
  }
}
