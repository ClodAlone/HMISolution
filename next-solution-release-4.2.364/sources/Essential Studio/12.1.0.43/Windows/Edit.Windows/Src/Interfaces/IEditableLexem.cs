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
using Syncfusion.Windows.Forms.Edit.Implementation.Parser;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Editable variant of the <see cref="ILexem"/> interface.
  /// </summary>
  internal interface IEditableLexem: ILexem
  {
    /// <summary>
    /// Text of the lexem.
    /// </summary>
    new string Text{ get; set; }
    /// <summary>
    /// Collapsable region, this lexem belongs to.
    /// </summary>
    new CollapsableRegion Collapser{ get; set; }
  }
}
