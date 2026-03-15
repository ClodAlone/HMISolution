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
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation
{
  class IconSetWrapper :
    IIconSet,
    IOptimizedUpdate
  {
    #region Members
    /// <summary>
    /// Parent format.
    /// </summary>
    private ConditionalFormatWrapper m_format;
    /// <summary>
    /// Wrapper over condition values.
    /// </summary>
    private List<IConditionValue> m_arrConditions = new List<IConditionValue>();
    /// <summary>
    /// Read-only part copy of the criteria collection.
    /// </summary>
    private IList<IConditionValue> m_readOnly;
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
          if (m_readOnly == null)
#if ( WINRT )
              m_readOnly = m_arrConditions;
#else
          m_readOnly = m_arrConditions.AsReadOnly();
#endif

        return m_readOnly;
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
        return GetIconSet().IconSet;
      }
      set
      {
        BeginUpdate();
        GetIconSet().IconSet = value;
        EndUpdate();
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
        return GetIconSet().PercentileValues;
      }
      set
      {
        BeginUpdate();
        GetIconSet().PercentileValues = value;
        EndUpdate();
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
        return GetIconSet().ReverseOrder;
      }
      set
      {
        BeginUpdate();
        GetIconSet().ReverseOrder = value;
        EndUpdate();
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
        return GetIconSet().ShowIconOnly;
      }
      set
      {
        BeginUpdate();
        GetIconSet().ShowIconOnly = value;
        EndUpdate();
      }
    }
    #endregion

    #region IOptimizedUpdate Members
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public void BeginUpdate()
    {
      m_format.BeginUpdate();
      UpdateCollection( GetIconSet().IconCriteria );
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
      m_format.EndUpdate();
      //m_wrapped = m_format.GetCondition().IconSet as IconSetImpl;
      UpdateCollection( GetIconSet().IconCriteria );
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the wrapper.
    /// </summary>
    /// <param name="format">Parent format wrapper.</param>
    public IconSetWrapper( ConditionalFormatWrapper format )
    {
      m_format = format;
      UpdateCollection( GetIconSet().IconCriteria );
    }
    /// <summary>
    /// Updates internal criteria collection.
    /// </summary>
    /// <param name="arrSource">Conditions to update.</param>
    private void UpdateCollection( IList<IConditionValue> arrSource )
    {
      int iSourceLength = ( arrSource != null ) ? arrSource.Count : 0;
      int iDestLength = m_arrConditions.Count;

      if( iSourceLength > iDestLength )
      {
        Add( iSourceLength - iDestLength, arrSource );
      }
      else if( iDestLength > iSourceLength )
      {
        Remove( iDestLength - iSourceLength );
      }

      Update( Math.Min( iSourceLength, iDestLength ) );
    }
    /// <summary>
    /// Adds required number of new wrappers to the criteria collection.
    /// </summary>
    /// <param name="count">Number of items to add.</param>
    /// <param name="arrSource">Source collection to wrap.</param>
    private void Add( int count, IList<IConditionValue> arrSource )
    {
      int startIndex = m_arrConditions.Count;
      for( int i = 0; i < count; i++ )
      {
        ConditionValueWrapper wrapper = new ConditionValueWrapper( arrSource[ i ], this );
        m_arrConditions.Add( wrapper );
      }
    }
    /// <summary>
    /// Updates wrappers inside criteria collection.
    /// </summary>
    /// <param name="count">Number of wrappers to update.</param>
    private void Update( int count )
    {
      IconSetImpl iconSetImpl = GetIconSet();
      IList<IConditionValue> arrValues = iconSetImpl.IconCriteria;

      for( int i = 0; i < count-1; i++ )
      {
        ConditionValueWrapper wrapper = m_arrConditions[ i ] as ConditionValueWrapper;
        wrapper.Wrapped = arrValues[ i+1 ];
      }
    }
    /// <summary>
    /// Removes wrappers from criteria collection.
    /// </summary>
    /// <param name="count">Number of wrappers to remove.</param>
    private void Remove( int count )
    {
      m_arrConditions.RemoveRange( m_arrConditions.Count - count, count );
    }
    /// <summary>
    /// Returns wrapped icon set.
    /// </summary>
    /// <returns>Wrapped icon set.</returns>
    private IconSetImpl GetIconSet()
    {
      return m_format.GetCondition().IconSet as IconSetImpl;
    }
    #endregion
  }
}
