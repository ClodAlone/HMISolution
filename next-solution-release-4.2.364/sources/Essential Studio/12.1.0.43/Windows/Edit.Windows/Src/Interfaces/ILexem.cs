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

using  Syncfusion.Windows.Forms.Edit.Enums;
using  Syncfusion.Windows.Forms.Edit.Implementation.Parser;
#endregion


namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Abstraction which link language and formating
  /// </summary>
  public interface ILexem
  {
    /// <summary>
    /// Text of the lexem.
    /// </summary>
    string Text{ get; }
    /// <summary>
    /// Config of the lexem.
    /// </summary>
    IConfigLexem Config{ get; set; }
    /// <summary>
    /// Collapsable region, this lexem belongs to.
    /// </summary>
    CollapsableRegion Collapser{ get; }
    /// <summary>
    /// Gets text length.
    /// </summary>
    int Length{ get; }
		/// <summary>
		/// Gets column of lexem in line.
		/// </summary>
		int Column{ get; }
  }
}