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
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using Syncfusion.XlsIO.Implementation.Exceptions;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Interfaces;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Interfaces;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
using Syncfusion.XlsIO.Interfaces;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.TemplateMarkers
{
  /// <summary>
  /// Class used for Marker Argument.
  /// </summary>
  public abstract class MarkerArgument : ICloneable
  {
    #region Class constants
    /// <summary>
    /// Argument parts separator.
    /// </summary>
    protected const char DEF_PARTS_SEPARATOR = ':';
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the MarkerArgument class.
    /// </summary>
    public MarkerArgument()
    {
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Tries to parse argument string.
    /// </summary>
    /// <param name="strArgument">Argument to parse.</param>
    /// <returns>Parsed argument if possible; null otherwise.</returns>
    public virtual MarkerArgument TryParse( string strArgument )
    {
      if( strArgument == null || strArgument.Length == 0 ) return null;

      Match m = ArgumentChecker.Match( strArgument );
      bool bResult = ( m.Success && m.Length == strArgument.Length );

      return ( bResult )
        ? Parse( m )
        : null;
    }
    /// <summary>
    /// Parses regular expression match.
    /// </summary>
    /// <param name="m">Match to parse.</param>
    /// <returns>Parsed argument of the same type as this instance is.</returns>
    protected virtual MarkerArgument Parse( Match m )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Applies argument to the marker at the specified position
    /// and to the list of cells with markers.
    /// </summary>
    /// <param name="sheet">Worksheet that contains cell to apply marker argument to.</param>
    /// <param name="pOldPosition">Previous cell position.</param>
    /// <param name="iRow">One-based row index of the cell to apply marker argument to.</param>
    /// <param name="iColumn">One-based column index of the cell to apply marker argument to.</param>
    /// <param name="arrMarkerCells">List of cells with markers.</param>
    /// <param name="options">Marker options.</param>
    public virtual void ApplyArgument( IWorksheet sheet, Point pOldPosition,
      ref int iRow, ref int iColumn, IList<long> arrMarkerCells, MarkerOptionsImpl options ,int count)
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Prepares options if necessary.
    /// </summary>
    /// <param name="options">Options to prepare.</param>
    public virtual void PrepareOptions( MarkerOptionsImpl options )
    {
      throw new NotImplementedException();
    }
    #endregion

    #region ICloneable Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public virtual object Clone()
    {
      return MemberwiseClone();
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets regular expression used to check argument. Read-only.
    /// </summary>
    protected virtual Regex ArgumentChecker
    {
      get
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Gets priority of the argument. Read-only.
    /// </summary>
    public virtual int Priority
    {
      get
      {
        return int.MaxValue;
      }
    }
    /// <summary>
    /// Indicates whether argument is used just to prepare some options before
    /// any values are proceed. Read-only.
    /// </summary>
    public virtual bool IsPreparing
    {
      get
      {
        return false;
      }
    }
    /// <summary>
    /// Indicates whether marker should be applies for each marker value. Read-only.
    /// </summary>
    public virtual bool IsApplyable
    {
      get
      {
        return false;
      }
    }
    /// <summary>
    /// Indicates whether argument can be present multiple times in single
    /// template marker. Read-only.
    /// </summary>
    public virtual bool IsAllowMultiple
    {
      get
      {
        return false;
      }
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Inserts row int list of cell indexes.
    /// </summary>
    /// <param name="arrCells">List with cell indexes.</param>
    /// <param name="i">Starting index.</param>
    /// <param name="iRowIndex">Row index.</param>
    protected static void InsertRow( IList<long> arrCells, int i, int iRowIndex )
    {
      if( arrCells == null )
        throw new ArgumentNullException( "arrCells" );

      int iCount = arrCells.Count;

      if( i < 0 || i > iCount - 1 )
        throw new ArgumentOutOfRangeException( "i", "Value cannot be less than 0 and greater than iCount - 1." );

      for( ; i < iCount; i++ )
      {
        long lCellIndex = arrCells[ i ];
        int iCellRowIndex = RangeImpl.GetRowFromCellIndex( lCellIndex );

        if( iCellRowIndex >= iRowIndex )
        {
          iCellRowIndex++;
          int iCellColumnIndex = RangeImpl.GetColumnFromCellIndex( lCellIndex );
          lCellIndex = RangeImpl.GetCellIndex( iCellColumnIndex, iCellRowIndex );
          arrCells[ i ] = lCellIndex;
        }
      }
    }
    /// <summary>
    /// Inserts column int list of cell indexes.
    /// </summary>
    /// <param name="arrCells">List with cell indexes.</param>
    /// <param name="i">Starting index.</param>
    /// <param name="iColumnIndex">One-based column index.</param>
    protected static void InsertColumn( IList<long> arrCells, int i, int iColumnIndex )
    {
      if( arrCells == null )
        throw new ArgumentNullException( "arrCells" );

      int iCount = arrCells.Count;

      if( i < 0 || i > iCount - 1 )
        throw new ArgumentOutOfRangeException( "i", "Value cannot be less than 0 and greater than iCount - 1." );

      for( ; i < iCount; i++ )
      {
        long lCellIndex = arrCells[ i ];
        int iCellColumnIndex = RangeImpl.GetColumnFromCellIndex( lCellIndex );

        if( iCellColumnIndex >= iColumnIndex )
        {
          iCellColumnIndex++;
          int iCellRowIndex = RangeImpl.GetRowFromCellIndex( lCellIndex );
          lCellIndex = RangeImpl.GetCellIndex( iCellColumnIndex, iCellRowIndex );
          arrCells[ i ] = lCellIndex;
        }
      }
    }
    #endregion
  }
}
