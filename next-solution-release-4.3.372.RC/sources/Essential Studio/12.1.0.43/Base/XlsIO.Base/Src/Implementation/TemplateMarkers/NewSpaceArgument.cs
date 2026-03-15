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

#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;
using Windows.UI;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif

namespace Syncfusion.XlsIO.Implementation.TemplateMarkers
{
  /// <summary>
  /// Class used for Space Argument.
  /// </summary>
  [ TemplateMarker ]
  public class NewSpaceArgument : MarkerArgument
  {
    #region Class constants
     /// <summary>
     /// It mentions the whether row has to be inserted.
     /// </summary>
     private bool m_bInsertRow = false;
    /// <summary>
    /// Priority of the argument.
    /// </summary>
    private const int DEF_PRIORITY = 1;
    /// <summary>
    /// Correct marker value.
    /// </summary>
    private const string DEF_MARKER_VALUE = "insert";
    /// <summary>
    /// Index of the group for copystyles part of the argument.
    /// </summary>
    private const int DEF_COPY_STYLES_GROUP = 1;
    #endregion

    #region Class static members
    /// <summary>
    /// Regular expression used to check argument.
    /// </summary>
    private static readonly Regex s_newSpaceRegex = new Regex( "insert(:copystyles)?",
#if !SILVERLIGHT && !WINRT && !WP
      RegexOptions.Compiled
#else
      RegexOptions.None
#endif
      );
    #endregion

    #region Class members
    /// <summary>
    /// Indicates whether styles should be copied.
    /// </summary>
    private bool m_bCopyStyles;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the NewSpaceArgument class.
    /// </summary>
    public NewSpaceArgument()
    {
    }
    #endregion

    #region Class overrides
//    /// <summary>
//    /// Tries to parse argument string.
//    /// </summary>
//    /// <param name="strArgument">String to parse.</param>
//    /// <returns></returns>
//    public override MarkerArgument TryParse( string strArgument )
//    {
//      if( strArgument == null || strArgument.Length == 0 )
//        return null;
//
//      return ( strArgument.ToLower() == DEF_MARKER_VALUE )
//        ? ( MarkerArgument )Clone()
//        : null;
//    }
//
    /// <summary>
    /// Parses regular expression match.
    /// </summary>
    /// <param name="m">Match to parse.</param>
    /// <returns>Parsed argument of the same type as this instance is.</returns>
    protected override MarkerArgument Parse( Match m )
    {
      if( m == null )
        throw new ArgumentNullException( "m" );

      m_bCopyStyles = ( m.Groups[ DEF_COPY_STYLES_GROUP ].Value.Length > 0 );
      return ( MarkerArgument )Clone();
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
    public override void ApplyArgument( IWorksheet sheet, Point pOldPosition,
      ref int iRow, ref int iColumn, IList<long> arrMarkerCells, MarkerOptionsImpl options,int count )
    {
      // Here we have to insert row or column in some location.
      // Main problem is to detect correct place for insertion.
      // On the current moment row / column is inserted just after current cell.
        if (count != 0)
            m_bInsertRow = false;
        ExcelInsertOptions insertOptions = m_bCopyStyles
        ? ExcelInsertOptions.FormatAsBefore
        : ExcelInsertOptions.FormatDefault;

      if( options.Direction == MarkerDirection.Horizontal )
      {
        int iColumnIndex = iColumn + 1;
        sheet.InsertColumn( iColumnIndex, 1, insertOptions );
        InsertColumn( arrMarkerCells, options.MarkerIndex, iColumnIndex );
      }
      else
      {
          if (!m_bInsertRow)
          {
              m_bInsertRow = true;
              sheet.InsertRow(iRow + 1, count-1, insertOptions);
          }
        int iRowIndex = iRow + 1;
        InsertRow( arrMarkerCells, options.MarkerIndex, iRowIndex );
      }
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Returns priority of the argument. Read-only.
    /// </summary>
    public override int Priority
    {
      get
      {
        return DEF_PRIORITY;
      }
    }

    /// <summary>
    /// Indicates whether marker should be applies for each marker value. Read-only.
    /// </summary>
    public override bool IsApplyable
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Regular expression used to check argument. Read-only.
    /// </summary>
    protected override Regex ArgumentChecker
    {
      get
      {
        return s_newSpaceRegex;
      }
    }
    #endregion
  }
}
