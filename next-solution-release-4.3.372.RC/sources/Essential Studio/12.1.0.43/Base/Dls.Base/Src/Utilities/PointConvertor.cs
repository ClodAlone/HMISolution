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

using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Helper class for dimension conversion
  /// </summary>
  public sealed class PointsConverter
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private static UnitsConvertor m_unConv = new UnitsConvertor();
    #endregion

    #region Class public methods
    /// <summary>
    /// Converts from Cantimeter to Points
    /// </summary>
    /// <param name="cantimeter"></param>
    /// <returns></returns>
    public static float FromCm( float cantimeter )
    {
      return (float)m_unConv.ConvertUnits( cantimeter, PrintUnits.Centimeter, PrintUnits.Point );
    }
    /// <summary>
    /// Converts from Inch to Points
    /// </summary>
    /// <param name="inch"></param>
    /// <returns></returns>
    public static float FromInch( float inch )
    {
      return ( float )m_unConv.ConvertUnits( inch, PrintUnits.Inch, PrintUnits.Point );
    }
    #endregion
  }
}