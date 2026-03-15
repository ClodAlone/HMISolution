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
using System.Drawing;

using Syncfusion.XlsIO.Interfaces.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Summary description for AreaImpl.
  /// </summary>
  public class AreaImpl : CommonObject, IArea
  {
    #region Class members
    private ExcelAutoType m_auto;
    private Color m_Color;
    //private
    #endregion

    #region Class constructors
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public AreaImpl( IApplication application, object parent )
      : base( application, parent )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    public AreaImpl( IApplication application, object parent, BiffRecordRaw[] data,
      ref int iPos )
      : this( application, parent )
    {
      Parse( data, ref iPos );
    }
    #endregion

    #region IArea Members
    /// <summary>
    /// 
    /// </summary>
    public Syncfusion.XlsIO.ExcelAutoType AreaType
    {
      get
      {
        return m_auto;
      }
      set
      {
        m_auto = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public System.Drawing.Color Color
    {
      get
      {
        return m_Color;
      }
      set
      {
        m_Color = value;
      }
    }

    #endregion

    #region Parse methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void Parse( BiffRecordRaw[] data, ref int iPos )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    private void Parse( BiffReader reader )
    {
    }
    #endregion
    
    #region Serialize methods
    public void Serialize( OffsetArrayList records )
    {
    }
    #endregion
  }
}
