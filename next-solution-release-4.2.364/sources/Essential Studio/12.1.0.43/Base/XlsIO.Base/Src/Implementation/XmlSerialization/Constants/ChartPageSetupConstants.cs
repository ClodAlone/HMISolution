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
  /// This class gives access to xml tag/attribute names used for chart object
  /// pagesetup serialization/parsing.
  /// </summary>
  public class ChartPageSetupConstants : IPageSetupConstantsProvider
  {
    #region IPageSetupConstantsProvider Members
    /// <summary>
    /// Gets name of the xml tag that stores margin settings. Read-only.
    /// </summary>
    public string PageMarginsTag
    {
      get
      {
        return ChartConstants.PageMarginsTag;
      }
    }
    /// <summary>
    /// Gets name of the xml attribute used to store left margin. Read-only.
    /// </summary>
    public string LeftMargin
    {
      get
      {
        return ChartConstants.LeftMargin;
      }
    }
    /// <summary>
    /// Gets name of the xml attribute used to store right margin. Read-only.
    /// </summary>
    public string RightMargin
    {
      get
      {
        return ChartConstants.RightMargin;
      }
    }
    /// <summary>
    /// Gets name of the xml attribute used to store top margin. Read-only.
    /// </summary>
    public string TopMargin
    {
      get
      {
        return ChartConstants.TopMargin;
      }
    }
    /// <summary>
    /// Gets name of the xml attribute used to store bottom margin. Read-only.
    /// </summary>
    public string BottomMargin
    {
      get
      {
        return ChartConstants.BottomMargin;
      }
    }
    /// <summary>
    /// Gets name of the xml attribute used to store header margin. Read-only.
    /// </summary>
    public string HeaderMargin
    {
      get
      {
        return ChartConstants.HeaderMargin;
      }
    }
    /// <summary>
    /// Gets name of the xml attribute used to store footer margin. Read-only.
    /// </summary>
    public string FooterMargin
    {
      get
      {
        return ChartConstants.FooterMargin;
      }
    }
    /// <summary>
    /// Gets tag's namespace. Read-only.
    /// </summary>
    public string Namespace
    {
      get
      {
        return ChartConstants.CNamespace;
      }
    }
    #endregion
  }
}
