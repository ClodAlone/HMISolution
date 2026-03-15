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

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Constants
{
  /// <summary>
  /// This class gives access to xml tag/attribute names used for
  /// pagesetup serialization/parsing.
  /// </summary>
  public interface IPageSetupConstantsProvider
  {
    /// <summary>
    /// Gets name of the xml tag that stores margin settings. Read-only.
    /// </summary>
    string PageMarginsTag { get; }
    /// <summary>
    /// Gets name of the xml attribute used to store left margin. Read-only.
    /// </summary>
    string LeftMargin { get; }
    /// <summary>
    /// Gets name of the xml attribute used to store right margin. Read-only.
    /// </summary>
    string RightMargin { get; }
    /// <summary>
    /// Gets name of the xml attribute used to store top margin. Read-only.
    /// </summary>
    string TopMargin { get; }
    /// <summary>
    /// Gets name of the xml attribute used to store bottom margin. Read-only.
    /// </summary>
    string BottomMargin { get; }
    /// <summary>
    /// Gets name of the xml attribute used to store header margin. Read-only.
    /// </summary>
    string HeaderMargin { get; }
    /// <summary>
    /// Gets name of the xml attribute used to store footer margin. Read-only.
    /// </summary>
    string FooterMargin { get; }
    /// <summary>
    /// Gets tag's namespace. Read-only.
    /// </summary>
    string Namespace { get; }
  }
}
