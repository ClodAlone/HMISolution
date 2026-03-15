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

namespace Syncfusion.DLS
{
  /// <summary>
  /// Class allows to store Page Margins (integer values).
  /// </summary>
  public sealed class Margins
  {
    #region Class members
    /// <summary>
    /// storage of Left margin
    /// </summary>
    private int m_iLeft;
    /// <summary>
    /// storage of Right margin
    /// </summary>
    private int m_iRight;
    /// <summary>
    /// storage of Top margin
    /// </summary>
    private int m_iTop;
    /// <summary>
    /// storage of Bottom margin
    /// </summary>
    private int m_iBottom;
    #endregion

    #region Class properties
    /// <summary>
    /// Allow to Get or Set value of all Margins on one call
    /// </summary>
    public int All
    {
      get
      {
        return ( IsAll ) ? m_iLeft : 0;
      }
      set
      {
        if( !IsAll && m_iLeft != value )
        {
          m_iLeft = m_iRight = m_iTop = m_iBottom = value;
        }
      }
    }
    /// <summary>
    /// Left mergin setting
    /// </summary>
    public int Left
    {
      get
      {
        return m_iLeft;
      }
      set
      {
        if( value != m_iLeft )
        {
          m_iLeft = value;
        }
      }
    }
    /// <summary>
    /// Right margin
    /// </summary>
    public int Right
    {
      get
      {
        return m_iRight;
      }
      set
      {
        if( value != m_iRight )
        {
          m_iRight = value;
        }
      }
    }
    /// <summary>
    /// Top margin
    /// </summary>
    public int Top
    {
      get
      {
        return m_iTop;
      }
      set
      {
        if( value != m_iTop )
        {
          m_iTop = value;
        }
      }
    }
    /// <summary>
    /// Bottom margin
    /// </summary>
    public int Bottom
    {
      get
      {
        return m_iBottom;
      }
      set
      {
        if( value != m_iBottom )
        {
          m_iBottom = value;
        }
      }
    }
    /// <summary>
    /// Allow to check is all settings has the same value or not
    /// </summary>
    private bool IsAll
    {
      get
      {
        return ( m_iLeft == m_iRight && m_iRight == m_iTop && m_iTop == m_iBottom );
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor. Set all margins to 0.
    /// </summary>
    public Margins()
    {
    }
    /// <summary>
    /// Create margins with specified values
    /// </summary>
    /// <param name="left">Left margin</param>
    /// <param name="top">Top margin</param>
    /// <param name="right">Right margins</param>
    /// <param name="bottom">Bottom margin</param>
    public Margins( int left, int top, int right, int bottom )
    {
      m_iLeft = left;
      m_iTop = top;
      m_iRight = right;
      m_iBottom = bottom;
    }
    #endregion
  }

  /// <summary>
  /// Class allows to store Page Margins (float values).
  /// </summary>
  public sealed class MarginsF
  {
    #region Class members
    /// <summary>
    /// storage of Left margin
    /// </summary>
    private float m_fLeft;
    /// <summary>
    /// storage of Right margin
    /// </summary>
    private float m_fRight;
    /// <summary>
    /// storage of Top margin
    /// </summary>
    private float m_fTop;
    /// <summary>
    /// storage of Bottom margin
    /// </summary>
    private float m_fBottom;
    #endregion

    #region Class properties
    /// <summary>
    /// Allow to Get or Set value of all Margins on one call
    /// </summary>
    public float All
    {
      get
      {
        return ( IsAll ) ? m_fLeft : 0;
      }
      set
      {
        if( m_fLeft != value || !IsAll )
        {
          m_fLeft = m_fRight = m_fTop = m_fBottom = value;
        }
      }
    }
    /// <summary>
    /// Left mergin setting
    /// </summary>
    public float Left
    {
      get
      {
        return m_fLeft;
      }
      set
      {
        if( value != m_fLeft )
        {
          m_fLeft = value;
        }
      }
    }
    /// <summary>
    /// Right margin
    /// </summary>
    public float Right
    {
      get
      {
        return m_fRight;
      }
      set
      {
        if( value != m_fRight )
        {
          m_fRight = value;
        }
      }
    }
    /// <summary>
    /// Top margin
    /// </summary>
    public float Top
    {
      get
      {
        return m_fTop;
      }
      set
      {
        if( value != m_fTop )
        {
          m_fTop = value;
        }
      }
    }
    /// <summary>
    /// Bottom margin
    /// </summary>
    public float Bottom
    {
      get
      {
        return m_fBottom;
      }
      set
      {
        if( value != m_fBottom )
        {
          m_fBottom = value;
        }
      }
    }
    /// <summary>
    /// Allow to check is all settings has the same value or not
    /// </summary>
    private bool IsAll
    {
      get
      {
        return ( m_fLeft == m_fRight && m_fRight == m_fTop && m_fTop == m_fBottom );
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor. Set all margins to 0.
    /// </summary>
    public MarginsF()
    {
    }
    /// <summary>
    /// Create margins with specified values
    /// </summary>
    /// <param name="left">Left margin</param>
    /// <param name="top">Top margin</param>
    /// <param name="right">Right margins</param>
    /// <param name="bottom">Bottom margin</param>
    public MarginsF( float left, float top, float right, float bottom )
    {
      m_fLeft = left;
      m_fTop = top;
      m_fRight = right;
      m_fBottom = bottom;
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public MarginsF Clone()
    {
      return new MarginsF( Left, Top, Right, Bottom );
    }
    #endregion
  }
}