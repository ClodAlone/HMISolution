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

using System;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents the autofilter conditions.
  /// </summary>
  public interface IAutoFilterCondition
  {
    /// <summary>
    /// Data type. 
    /// </summary>
    ExcelFilterDataType DataType { get; set; }
    /// <summary>
    /// Comparison operator. 
    /// </summary>
    ExcelFilterCondition ConditionOperator { get; set; }
    /// <summary>
    /// String value. 
    /// </summary>
    string String { get; set; }
    /// <summary>
    /// Boolean value. Read-only.
    /// </summary>
    bool Boolean { get; }
    /// <summary>
    /// Error code. Read-only.
    /// </summary>
    byte ErrorCode { get; }
    /// <summary>
    /// Floating-point value. 
    /// </summary>
    double Double { get; set; }
  }
}
