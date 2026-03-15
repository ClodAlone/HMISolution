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

#if ( WINRT )
using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif

namespace Syncfusion.XlsIO.Implementation
{
  class ColorScaleWrapper :
    IColorScale,
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
    private List<IColorConditionValue> m_arrConditions = new List<IColorConditionValue>();
    /// <summary>
    /// Read-only part copy of the criteria collection.
    /// </summary>
    private IList<IColorConditionValue> m_readOnly;
    #endregion

    #region IColorScale Members
    /// <summary>
    /// Returns a collection of individual IColorConditionValue objects.
    /// The IColorConditionValue object specifies the type, value, and the color
    /// of threshold criteria used in the color scale conditional format. Read-only.
    /// </summary>
    public IList<IColorConditionValue> Criteria
    {
      get
      {
        if( m_readOnly == null )
#if ( WINRT )
            m_readOnly=m_arrConditions;
#else
          m_readOnly = m_arrConditions.AsReadOnly();
#endif

        return m_readOnly;
      }
    }
    /// <summary>
    /// Sets number of IColorConditionValue objects in the collection. Supported values are 2 and 3.
    /// </summary>
    /// <param name="count">Number of conditions.</param>
    public void SetConditionCount( int count )
    {
      BeginUpdate();
      GetWrapped().SetConditionCount( count );
      EndUpdate();
    }
    #endregion

    #region IOptimizedUpdate Members
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public void BeginUpdate()
    {
      m_format.BeginUpdate();
      UpdateCollection( GetWrapped().Criteria, this );
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
      m_format.EndUpdate();
      UpdateCollection( GetWrapped().Criteria, this );
    }

    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the wrapper.
    /// </summary>
    /// <param name="format">Parent format wrapper.</param>
    public ColorScaleWrapper( ConditionalFormatWrapper format )
    {
      m_format = format;
      UpdateCollection( GetWrapped().Criteria, this );
    }
    /// <summary>
    /// Updates internal criteria collection.
    /// </summary>
    /// <param name="arrSource"></param>
    /// <param name="parent"></param>
    private void UpdateCollection( IList<IColorConditionValue> arrSource, IOptimizedUpdate parent )
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
    private void Add( int count, IList<IColorConditionValue> arrSource )
    {
      int startIndex = m_arrConditions.Count;
      for( int i = 0; i < count; i++ )
      {
        ConditionValueWrapper wrapper = new ColorConditionValueWrapper( arrSource[ i ], this );
        m_arrConditions.Add( wrapper as IColorConditionValue );
      }
    }
    /// <summary>
    /// Updates wrappers inside criteria collection.
    /// </summary>
    /// <param name="count">Number of wrappers to update.</param>
    private void Update( int count )
    {
      ColorScaleImpl wrapped = GetWrapped();
      IList<IColorConditionValue> arrValues = wrapped.Criteria;

      for( int i = 0; i < count; i++ )
      {
        ConditionValueWrapper wrapper = m_arrConditions[ i ] as ConditionValueWrapper;
        wrapper.Wrapped = arrValues[ i ];
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
    /// Returns wrapped object.
    /// </summary>
    /// <returns>Wrapped object.</returns>
    private ColorScaleImpl GetWrapped()
    {
      return m_format.GetCondition().ColorScale as ColorScaleImpl;
    }
    #endregion
  }
}
