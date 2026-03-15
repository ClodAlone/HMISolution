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

using Syncfusion.XlsIO.Interfaces.Charts;
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Summary description for LabelText.
  /// </summary>
  public class LabelTextImpl
    : ChartTitleImpl
    , ILabelText
  {
    #region Class Members
    /// <summary>
    /// 
    /// </summary>
    private string m_strNumberFormat;
    #endregion

    #region Class constructors
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public LabelTextImpl( IApplication application, object parent )
      : base( application, parent )
    {
    }
    #endregion

    #region ILabelText Members
    /// <summary>
    /// 
    /// </summary>
    public string NumberFormat
    {
      get
      {
        return m_strNumberFormat;
      }
      set
      {
        m_strNumberFormat = value;
      }
    }

    #endregion
  }
}
