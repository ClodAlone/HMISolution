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
using System.Xml;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
  /// <summary>
  /// Interface for saving data to XML.
  /// </summary>
  public interface IXMLDataProvider
  {
    /// <summary>
    /// Attaches it`s data to some XML element.
    /// </summary>
    /// <param name="parent">Parent elements, data have to be saved to.</param>
    void AppendToXML( XmlElement parent );
    /// <summary>
    /// Writes data to XML.
    /// </summary>
    /// <param name="writer">Writer, data have to be saved to.</param>
    void AppendToXML( XmlTextWriter writer );
  }
}
