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
#endregion

namespace Syncfusion.Layouting
{
  /// <summary>
  /// 
  /// </summary>
  public class StringSplitInfo
  {
    #region Class members
    /// <summary>
    /// The position of first symbol
    /// </summary>
    private int m_firstPos = 0;
    /// <summary>
    /// The position of last symbol
    /// </summary>
    private int m_lastPos = 0;
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public int FirstPos
    {
      get
      {
        return m_firstPos;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int LastPos
    {
      get
      {
        return m_lastPos;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Length
    {
      get
      {
        return LastPos - FirstPos + 1;
      }
    }
    #endregion
    
    #region Class initialize/finalize methods
    /// <summary>
    /// Disabled default constructor.
    /// </summary>
    private StringSplitInfo()
    {}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="firstPos"></param>
    /// <param name="lastPos"></param>
    public StringSplitInfo( int firstPos, int lastPos )
    {
      if( firstPos < 0 )
        throw new ArgumentException( "firstPos" );
      if( firstPos > lastPos )
        throw new ArgumentException( "lastPos" );
      
      m_lastPos = lastPos;
      m_firstPos = firstPos;
    }
    #endregion
    
    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="length"></param>
    public void Check( int length )
    {
      if( m_firstPos < 0 || m_firstPos > length )
        throw new ArgumentOutOfRangeException( "SplitInfo.FirstPos" );
      if( m_lastPos < m_firstPos || m_lastPos > length )
        throw new ArgumentOutOfRangeException( "SplitInfo.LastPos" );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strSplitInfo"></param>
    public void Extend( StringSplitInfo strSplitInfo )
    {
      m_firstPos += strSplitInfo.FirstPos;
      m_lastPos += strSplitInfo.FirstPos;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public StringSplitInfo GetSplitFirstPart( int position )
    {
      return new StringSplitInfo( m_firstPos, m_firstPos + position - 1 );
    }
    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public StringSplitInfo GetSplitSecondPart( int position )
    {
      return new StringSplitInfo( m_firstPos + position, m_lastPos );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string GetSubstring( string text )
    {
      return text.Substring( m_firstPos, m_lastPos - m_firstPos + 1 );
    }
    #endregion
  }
  /// <summary>
  /// 
  /// </summary>
  public class SplitStringWidget : ISplitLeafWidget
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private IStringWidget m_strWidget;
    private StringSplitInfo m_splitInfo;
    #endregion
    
    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public IStringWidget RealStringWidget
    {
      get
      {
        return m_strWidget;
      }
    }
    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns></returns>
    public string GetText()
    {
      return m_splitInfo.GetSubstring( m_strWidget.Text );
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    public SplitStringWidget( IStringWidget strWidget, StringSplitInfo splitInfo )
    {
      m_strWidget = strWidget;  
      m_splitInfo = splitInfo;
    }
    #endregion

    #region IWidget implement
    /// <summary>
    /// Gets layout info.
    /// </summary>
    public ILayoutInfo LayoutInfo
    {
      get
      {
        return m_strWidget.LayoutInfo;
      }
    }
    /// <summary>
    /// Draw range to graphics.
    /// </summary>
    public void Draw( CustomGraphics g, LayoutedWidget layoutedWidget )
    {
      m_strWidget.Draw( g, layoutedWidget, 
        m_splitInfo.GetSubstring( m_strWidget.Text ) );
    }
    #endregion
    
    #region ILeafWidget implement
    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public ISplitLeafWidget[] SplitByOffset( CustomGraphics graphics, SizeF offset )
    {
      return SplitByOffset( graphics, offset.Width, m_strWidget, m_splitInfo );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <returns></returns>
    public SizeF Measure( CustomGraphics graphics )
    {
      return m_strWidget.Measure( graphics, GetText() );
    }
    #endregion
    
    #region Class utility methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="offset"></param>
    /// <param name="strWidget"></param>
    /// <param name="splitInfo"></param>
    /// <returns></returns>
    public static ISplitLeafWidget[] SplitByOffset( CustomGraphics graphics, double offset, IStringWidget strWidget, StringSplitInfo splitInfo )
    {
      if( splitInfo == null )
        splitInfo = new StringSplitInfo( 0, strWidget.Text.Length - 1 );
        
      int index = strWidget.OffsetToIndex( graphics, offset, 
        splitInfo.GetSubstring( strWidget.Text ) );
      
      //if( index > -1 && index < strWidget.Text.Length - 1 )
      if( index > -1 && index < splitInfo.Length - 1 )
      {
        ISplitLeafWidget[] spLeafWidgets = new ISplitLeafWidget[2];
        spLeafWidgets[ 0 ] =
          new SplitStringWidget( strWidget, splitInfo.GetSplitFirstPart( index + 1 ) );
        spLeafWidgets[ 1 ] =
          new SplitStringWidget( strWidget, splitInfo.GetSplitSecondPart( index + 1 ) );
        return spLeafWidgets;
      }
      
      return null;
    }
    #endregion
  }
}