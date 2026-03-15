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

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Collections;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// This class is used to get header / footer shapes from a worksheet.
	/// </summary>
	public class HeaderImageGetter
    : IShapeGetter
    , ICloneable
	{
    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public HeaderImageGetter()
    {
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Returns a shape collection from the worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet to return sheet data for.</param>
    /// <returns>A shape collection from the worksheet.</returns>
    public ShapeCollectionBase GetShapes( WorksheetBaseImpl sheet )
    {
      return sheet.HeaderFooterShapes;
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
      return MemberwiseClone();
    }

    #endregion
  }
}
