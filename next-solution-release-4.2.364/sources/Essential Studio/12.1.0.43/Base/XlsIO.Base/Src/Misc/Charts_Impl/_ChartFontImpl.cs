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
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Summary description for ChartFontImpl.
  /// </summary>
  public class ChartFontImpl : CommonObject, IChartFont
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private string m_strName;
    /// <summary>
    /// 
    /// </summary>
    private int m_iSize;
    /// <summary>
    /// 
    /// </summary>
    private Color m_color;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bAutoColor;
    /// <summary>
    /// 
    /// </summary>
    private Color m_backColor;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bAutoBackColor;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bStrikethrough;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bSuperscript;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bSubscript;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bAutoScale;
    #endregion

    #region Class constructors
    public ChartFontImpl( IApplication application, object parent )
      : base( application, parent )
    {
      //
      // TODO: Add constructor logic here
      //
    }
    #endregion

    #region IChartFont Members
    /// <summary>
    /// 
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        m_strName = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Style
    {
      get
      {
        // TODO:  Add ChartFontImpl.Style getter implementation
        return 0;
      }
      set
      {
        // TODO:  Add ChartFontImpl.Style setter implementation
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Size
    {
      get
      {
        return m_iSize;
      }
      set
      {
        m_iSize = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Underline
    {
      get
      {
        // TODO:  Add ChartFontImpl.Underline getter implementation
        return 0;
      }
      set
      {
        // TODO:  Add ChartFontImpl.Underline setter implementation
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public System.Drawing.Color Color
    {
      get
      {
        return m_color;
      }
      set
      {
        m_color = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsAutoColor
    {
      get
      {
        return m_bAutoColor;
      }
      set
      {
        m_bAutoColor = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public System.Drawing.Color BackColor
    {
      get
      {
        return m_backColor;
      }
      set
      {
        m_backColor = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsAutoBackColor
    {
      get
      {
        return m_bAutoBackColor;
      }
      set
      {
        m_bAutoBackColor = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsStrikethrough
    {
      get
      {
        return m_bStrikethrough;
      }
      set
      {
        m_bStrikethrough = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsSuperscript
    {
      get
      {
        return m_bSuperscript;
      }
      set
      {
        if( m_bSuperscript != value )
        {
          m_bSuperscript = value;
          
          if( value ) m_bSubscript = false;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsSubscript
    {
      get
      {
        return m_bSubscript;
      }
      set
      {
        if( m_bSubscript != value )
        {
          m_bSubscript = value;

          if( value ) m_bSuperscript = false;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsAutoScale
    {
      get
      {
        return m_bAutoScale;
      }
      set
      {
        m_bAutoScale = value;
      }
    }

    #endregion
  }
}
