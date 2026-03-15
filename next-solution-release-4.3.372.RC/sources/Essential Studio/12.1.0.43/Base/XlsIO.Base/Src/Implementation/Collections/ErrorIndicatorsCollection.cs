#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections.Generic;
using System.Text;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Summary description for ErrorIndicatorsCollection.
  /// </summary>
  public class ErrorIndicatorsCollection
    : CollectionBaseEx<ErrorIndicatorImpl>
  {
    #region Class members
    /// <summary>
    /// Dictionary with error indicators. Used to check whether indicator is unique.
    /// </summary>
    private Dictionary<ExcelIgnoreError, ErrorIndicatorImpl> m_dicErrorIndicators =
      new Dictionary<ExcelIgnoreError, ErrorIndicatorImpl>();
    #endregion

    #region Class Initialize/finalize methods
    /// <summary>
    /// Initializes new instance of the collection.
    /// </summary>
    /// <param name="application">Application object for the new collection.</param>
    /// <param name="parent">Parent object for the new collection.</param>
    public ErrorIndicatorsCollection( IApplication application, object parent )
      : base( application, parent )
    {
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Adds new item to collection.
    /// </summary>
    /// <param name="errorIndicator">Error indicator to add.</param>
    /// <returns>Added error indicator.</returns>
    public ErrorIndicatorImpl Add( ErrorIndicatorImpl errorIndicator )
    {
      if( errorIndicator == null )
        throw new ArgumentNullException( "errorIndicator" );

      ExcelIgnoreError options = errorIndicator.IgnoreOptions;

      ErrorIndicatorImpl curErrorIndicator;

      Remove( errorIndicator.CellList.ToArray() );

      if( m_dicErrorIndicators.TryGetValue( errorIndicator.IgnoreOptions, out curErrorIndicator ) )
      {
        curErrorIndicator.AddCells( errorIndicator );
      }
      else
      {
        base.Add( errorIndicator );
        m_dicErrorIndicators.Add( options, errorIndicator );
      }

      return ( curErrorIndicator != null ) ? curErrorIndicator : errorIndicator;
    }
    /// <summary>
    /// Searches for error indicator that contains all specified ranges.
    /// </summary>
    /// <param name="arrRanges">Ranges to find error indicator for.</param>
    /// <returns>Error indicator if found, else null.</returns>
    public ErrorIndicatorImpl Find( Rectangle[] arrRanges )
    {
      if( arrRanges == null )
        return null;

      int iCount = arrRanges.Length;

      if( iCount == 0 )
        return null;

      for( int i = 0, len = Count; i < len; i++ )
      {
        ErrorIndicatorImpl errorIndicator = this[ i ];

        if( errorIndicator.Contains( arrRanges, 0 ) )
        {
          return errorIndicator;
        }
      }

      return null;      
    }
    /// <summary>
    /// Removes specified range from collection.
    /// </summary>
    /// <param name="rect">Range to remove.</param>
    public void Remove( Rectangle[] rect )
    {
      foreach( KeyValuePair<ExcelIgnoreError, ErrorIndicatorImpl> keyValue in m_dicErrorIndicators )
      {
        keyValue.Value.Remove( rect );
      }
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone( object parent )
    {
      ErrorIndicatorsCollection errorIndicators = ( ErrorIndicatorsCollection )base.Clone( parent );

      Dictionary<ExcelIgnoreError, ErrorIndicatorImpl> dicErrorIndicators = new Dictionary<ExcelIgnoreError, ErrorIndicatorImpl>();

      foreach( KeyValuePair<ExcelIgnoreError, ErrorIndicatorImpl> keyValue in m_dicErrorIndicators )
      {
        dicErrorIndicators.Add( keyValue.Key, keyValue.Value );
      }

      errorIndicators.m_dicErrorIndicators = dicErrorIndicators;

      return errorIndicators;
    }
    #endregion
  }
}
