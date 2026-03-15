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
#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents a hyperlink.
  /// </summary>
  public interface IHyperLink : IParentApplication
  {
    /// <summary>
    /// Returns or sets the address of the target document.
    /// </summary>
    string Address { get; set; }
    /// <summary>
    /// Returns a name of the object.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Returns a Range object that represents the range the specified hyperlink is attached to. 
    /// </summary>
    IRange Range { get; }
    /// <summary>
    /// Returns or sets the ScreenTip text for the specified hyperlink.
    /// </summary>
    string ScreenTip { get; set; }
    /// <summary>
    /// Returns or sets the location within the document associated with the hyperlink.
    /// </summary>
    string SubAddress { get; set; }
    /// <summary>
    /// Returns or sets the text to be displayed for the specified hyperlink.
    /// The default value is the address of the hyperlink.
    /// </summary>
    string TextToDisplay { get; set; }
    /// <summary>
    /// Returns or sets the object type.
    /// </summary>
    ExcelHyperLinkType Type { get; set; }
  }
}
