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
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation
{
  class IconSetImpl :
    IIconSet,
    ICloneable
  {
    #region Membres
    /// <summary>
    /// An IconCriteria collection which represents the set of criteria for
    /// an icon set conditional formatting rule.
    /// </summary>
    private IConditionValue[] m_arrCriteria;
    /// <summary>
    /// An IconSets collection which specifies the icon set used
    /// in the conditional format.
    /// </summary>
    private ExcelIconSetType m_iconSet;
    /// <summary>
    /// A Boolean value indicating if the thresholds for an icon
    /// set conditional format are determined using percentiles. 
    /// </summary>
    private bool m_bPercentileValues;
    /// <summary>
    /// A Boolean value indicating if the order of icons is
    /// reversed for an icon set.
    /// </summary>
    private bool m_bReverseOrder;
    /// <summary>
    /// A Boolean value indicating if only the icon is displayed
    /// for an icon set conditional format.
    /// </summary>
    private bool m_bShowIconOnly;
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public IconSetImpl Clone()
    {
      IconSetImpl result = ( IconSetImpl )MemberwiseClone();

      if( m_arrCriteria != null )
      {
        int iLength = m_arrCriteria.Length;
        result.m_arrCriteria = new ConditionValue[ iLength ];

        for( int i = 0; i < iLength; i++ )
        {
          result.m_arrCriteria[ i ] = ( m_arrCriteria[ i ] as ConditionValue ).Clone();
        }
      }

      return result;
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    object ICloneable.Clone()
    {
      return Clone();
    }
    #endregion

    #region IIconSet Members
    /// <summary>
    /// Returns an IconCriteria collection which represents the set of criteria for
    /// an icon set conditional formatting rule.
    /// </summary>
    public IList<IConditionValue> IconCriteria
    {
      get
      {
          if (m_arrCriteria == null)
              UpdateCriteria();

        return m_arrCriteria;
      }
    }
    /// <summary>
    /// Returns or sets an IconSets collection which specifies the icon set used
    /// in the conditional format.
    /// </summary>
    public ExcelIconSetType IconSet
    {
      get
      {
        return m_iconSet;
      }
      set
      {
        if( m_iconSet != value )
        {
          m_iconSet = value;
          UpdateCriteria();
        }
      }
    }
    /// <summary>
    /// Returns or sets a Boolean value indicating if the thresholds for an icon
    /// set conditional format are determined using percentiles. 
    /// </summary>
    public bool PercentileValues
    {
      get
      {
        return m_bPercentileValues;
      }
      set
      {
        m_bPercentileValues = value;
      }
    }
    /// <summary>
    /// Returns or sets a Boolean value indicating if the order of icons is
    /// reversed for an icon set.
    /// </summary>
    public bool ReverseOrder
    {
      get
      {
        return m_bReverseOrder;
      }
      set
      {
        m_bReverseOrder = value;
      }
    }
    /// <summary>
    /// Returns or sets a Boolean value indicating if only the icon is displayed
    /// for an icon set conditional format.
    /// </summary>
    public bool ShowIconOnly
    {
      get
      {
        return m_bShowIconOnly;
      }
      set
      {
        m_bShowIconOnly = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Updates criteria collection.
    /// </summary>
    private void UpdateCriteria()
    {
      string strIconSet = m_iconSet.ToString();
      int iCount = 0;

      if( strIconSet.StartsWith( "Three" ) )
      {
        iCount = 3;
      }
      else if( strIconSet.StartsWith( "Four" ) )
      {
        iCount = 4;
      }
      else if( strIconSet.StartsWith( "Five" ) )
      {
        iCount = 5;
      }
      else
      {
        throw new InvalidOperationException();
      }

      m_arrCriteria = new ConditionValue[ iCount ];

      for( int i = 0; i < iCount; i++ )
      {
        int iValue = ( int )Math.Round( i * 100 / ( double )iCount );
        ConditionValue criteria = new ConditionValue( ConditionValueType.Percent, iValue.ToString() );
        m_arrCriteria[ i ] = criteria;
      }
    }
    #endregion

    internal void ClearAll()
    {
        m_arrCriteria = null;
    }
  }
}
