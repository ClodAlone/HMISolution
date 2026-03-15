#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Interfaces.Charts
{
  internal interface IInternalChartTextArea :
    IChartTextArea,
    IInternalFont
  {
    /// <summary>
    /// Returns textarea's color object. Read-only.
    /// </summary>
    ColorObject ColorObject { get; }
    /// <summary>
    /// Gets value indicating whether TextRotation was changed. Read-only.
    /// </summary>
    bool HasTextRotation { get; }
    /// <summary>
    /// Represents the Legend Paragraph 
    /// </summary>
    ChartParagraphType ParagraphType{get;set;}
      
  }
}
