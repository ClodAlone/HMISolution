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

using System;

namespace Syncfusion.XlsIO.Implementation.TemplateMarkers
{
  /// <summary>
  /// Contains settings that are local to single marker and depend on marker arguments.
  /// </summary>
  public class MarkerOptionsImpl
  {
    #region Class members
    /// <summary>
    /// Marker direction (horizontal or vertical).
    /// </summary>
    private MarkerDirection m_direction;
    /// <summary>
    /// Current template marker index.
    /// </summary>
    private int m_iMarkerIndex;
    /// <summary>
    /// Workbook where operation takes place.
    /// </summary>
    private IWorkbook m_book;
    /// <summary>
    /// Original marker text.
    /// </summary>
    private string m_strOriginalMarker;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the MarkerOptionsImpl class.
    /// </summary>
    /// <param name="book">Current workbook.</param>
    public MarkerOptionsImpl( IWorkbook book )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      m_book = book;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets or sets Marker direction (horizontal or vertical).
    /// </summary>
    public MarkerDirection Direction
    {
      get
      {
        return m_direction;
      }
      set
      {
        m_direction = value;
      }
    }
    /// <summary>
    /// Gets or sets current template marker index.
    /// </summary>
    public int MarkerIndex
    {
      get
      {
        return m_iMarkerIndex;
      }
      set
      {
        m_iMarkerIndex = value;
      }
    }
    /// <summary>
    /// Gets workbook where operation takes place. Read-only.
    /// </summary>
    public IWorkbook Workbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Gets or sets original marker text.
    /// </summary>
    public string OriginalMarker
    {
      get
      {
        return m_strOriginalMarker;
      }
      set
      {
        m_strOriginalMarker = value;
      }
    }
    #endregion
  }
}
