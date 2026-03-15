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
  /// This class gives access to xml tag/attribute names used for tab sheet.
  /// pagesetup serialization/parsing.
  /// </summary>
  public class WorksheetPageSetupConstants : IPageSetupConstantsProvider
  {
    #region IPageSetupConstantsProvider Members
    /// <summary>
    /// Gets name of the xml tag that stores margin settings. Read-only.
    /// </summary>
    public string PageMarginsTag
    {
      get
      {
        return PageSetup.PageMarginsTag;
      }
    }
    /// <summary>
    /// Gets name of the xml attribute used to store left margin. Read-only.
    /// </summary>
    public string LeftMargin
    {
      get
      {
        return PageSetup.LeftMargin;
      }
    }
    /// <summary>
    /// Gets name of the xml attribute used to store right margin. Read-only.
    /// </summary>
    public string RightMargin
    {
      get
      {
        return PageSetup.RightMargin;
      }
    }
    /// <summary>
    /// Gets name of the xml attribute used to store top margin. Read-only.
    /// </summary>
    public string TopMargin
    {
      get
      {
        return PageSetup.TopMargin;
      }
    }
    /// <summary>
    /// Gets name of the xml attribute used to store bottom margin. Read-only.
    /// </summary>
    public string BottomMargin
    {
      get
      {
        return PageSetup.BottomMargin;
      }
    }
    /// <summary>
    /// Gets name of the xml attribute used to store header margin. Read-only.
    /// </summary>
    public string HeaderMargin
    {
      get
      {
        return PageSetup.HeaderMargin;
      }
    }
    /// <summary>
    /// Gets name of the xml attribute used to store footer margin. Read-only.
    /// </summary>
    public string FooterMargin
    {
      get
      {
        return PageSetup.FooterMargin;
      }
    }
    /// <summary>
    /// Gets tag's namespace. Read-only.
    /// </summary>
    public string Namespace
    {
      get
      {
        return null;
      }
    }

    #endregion
  }
}
