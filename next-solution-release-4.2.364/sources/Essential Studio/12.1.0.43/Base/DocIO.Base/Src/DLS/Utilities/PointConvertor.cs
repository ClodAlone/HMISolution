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

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Helper class for dimension conversion
    /// </summary>
    public sealed class PointsConverter
    {
        #region Class public methods
        /// <summary>
        /// Converts from Centimeter to Points
        /// </summary>
        /// <param name="centimeter"></param>
        /// <returns></returns>
        public static float FromCm(float centimeter)
        {
            return (float)UnitsConvertor.Instance.ConvertUnits(centimeter, PrintUnits.Centimeter, PrintUnits.Point);
        }
        /// <summary>
        /// Converts from Inch to Points
        /// </summary>
        /// <param name="inch"></param>
        /// <returns></returns>
        public static float FromInch(float inch)
        {
            return (float)UnitsConvertor.Instance.ConvertUnits(inch, PrintUnits.Inch, PrintUnits.Point);
        }
        /// <summary>
        /// Converts from Inch to Points
        /// </summary>
        /// <param name="inch"></param>
        /// <returns></returns>
        public static float FromPixel(float px)
        {
            return (float)UnitsConvertor.Instance.ConvertUnits(px, PrintUnits.Pixel, PrintUnits.Point);
        }
        #endregion
    }
}