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
using System.Diagnostics;
using System.Collections;
using System;
#endregion

namespace Syncfusion.Layouting
{
  /// <summary>
  /// Summary description for NumberedListCounter.
  /// </summary>
  internal class NumberedListCounter
  {
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    [ThreadStatic]
    private static NumberedListCounter m_instance;
    /// <summary>
    /// 
    /// </summary>
    private ArrayList m_levels = new ArrayList();
    /// <summary>
    /// 
    /// </summary>
    [ThreadStatic]
    private static ArrayList m_styleNames;
    /// <summary>
    /// 
    /// </summary>
    [ThreadStatic]
    private static ArrayList m_styleStartIndexes;
    /// <summary>
    /// 
    /// </summary>
    [ThreadStatic]
    private static ArrayList m_styleStartLevels;
    /// <summary>
    /// 
    /// </summary>
    [ThreadStatic]
    private static string m_strPrewStyleName;

    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public static NumberedListCounter Instance
    {
      get
      {
        if( m_instance == null )
        {
          m_instance = new NumberedListCounter();
          m_styleNames = new ArrayList();
          m_styleStartIndexes = new ArrayList();
          m_styleStartLevels = new ArrayList();
          m_strPrewStyleName = string.Empty;
        }

        return m_instance;
      }
    }        
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    public NumberedListCounter()
    {}
    #endregion
    
    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    public void ResetCounter()
    {
      m_levels.Clear();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    public void ResetLevel( int level )
    {
      for( int i = level; i < m_levels.Count; i++ )
      {
        m_levels[ i ] = ( int )0;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int NextLevelNumber( int level, string styleName )
    {
      while( level > m_levels.Count - 1 )
      {
        m_levels.Add( ( int )0 );
      }

      if( !m_styleNames.Contains( styleName ) )
      {
        m_styleNames.Add( styleName );
        ResetLevel( level );
        m_styleStartIndexes.Add( ( int )m_levels[ level ] );
        m_styleStartLevels.Add( level );
      }
      else if( m_strPrewStyleName != styleName )
      {
        int index = m_styleNames.IndexOf( styleName );

        if( (int)m_styleStartLevels[ index ] == level )
        {
          m_levels[ level ] = ( int )m_styleStartIndexes[ index ] + 1;
        }
        else
        {
          m_levels[ level ] = m_styleStartIndexes[ index ];
          m_styleStartIndexes[ index ] = (int)m_levels[ level ] + 1;          
        }
      }
      else
      {
        int index = m_styleNames.IndexOf( styleName );
        m_styleStartIndexes[ index ] = m_levels[ level ];
        m_styleStartLevels[ index ] = level;
      }

      m_strPrewStyleName = styleName;

      int num = ( int )m_levels[ level ];
      m_levels[ level ] = num + 1;      
      ResetLevel( level + 1 );
      return num;
    }   
    #endregion
  }
}