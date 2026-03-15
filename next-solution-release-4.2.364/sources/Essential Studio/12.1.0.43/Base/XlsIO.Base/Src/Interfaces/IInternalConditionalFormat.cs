#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

namespace Syncfusion.XlsIO.Interfaces
{
  internal interface IInternalConditionalFormat : IConditionalFormat
  {
    /// <summary>
    /// Conditional format color. Read-only.
    /// </summary>
    ColorObject ColorObject { get; }
    /// <summary>
    /// Conditional format background color. Read-only.
    /// </summary>
    ColorObject BackColorObject { get; }
    /// <summary>
    /// Conditional format top border color. Read-only.
    /// </summary>
    ColorObject TopBorderColorObject { get; }
    /// <summary>
    /// Conditional format bottom border color. Read-only.
    /// </summary>
    ColorObject BottomBorderColorObject { get; }
    /// <summary>
    /// Conditional format left border color. Read-only.
    /// </summary>
    ColorObject LeftBorderColorObject { get; }
    /// <summary>
    /// Conditional format right border color. Read-only.
    /// </summary>
    ColorObject RightBorderColorObject { get; }
    /// <summary>
    /// Conditional format font color. Read-only.
    /// </summary>
    ColorObject FontColorObject { get; }
    /// <summary>
    /// Indicates whether pattern style was modified.
    /// </summary>
    bool IsPatternStyleModified { get; set; }
    /// <summary>
    /// Returns parsed tokens of the first formula.
    /// </summary>
    Ptg[] FirstFormulaPtgs { get; }
    /// <summary>
    /// Returns parsed tokens of the second formula.
    /// </summary>
    Ptg[] SecondFormulaPtgs { get; }
  }
}
