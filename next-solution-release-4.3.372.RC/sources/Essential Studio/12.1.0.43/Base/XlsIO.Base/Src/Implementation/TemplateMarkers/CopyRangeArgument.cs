#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
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
  /// CopyRangeArgument for template marker. Specifies range that should be copied
  /// for each new marker element.
  /// </summary>
  [ TemplateMarker ]
  public class CopyRangeArgument : JumpArgument
  {
    #region Class constants
    /// <summary>
    /// /// Start of the copy range argument.
    /// </summary>
    private const string DEF_COPYRANGE = "copyrange";
    /// <summary>
    /// Default copy options.
    /// </summary>
    private const ExcelCopyRangeOptions DEF_DEFAULT_COPY_OPTIONS
      = ExcelCopyRangeOptions.UpdateFormulas | ExcelCopyRangeOptions.UpdateMerges;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the CopyRangeArgument class.
    /// </summary>
    public CopyRangeArgument()
    {
    }
    #endregion

    #region Class members
    /// <summary>
    /// Destination row index.
    /// </summary>
    private int m_iSecondRow;
    /// <summary>
    /// Destination column index.
    /// </summary>
    private int m_iSecondColumn;
    /// <summary>
    /// Indicates whether row index is relative.
    /// </summary>
    private bool m_bSecondRowRelative;
    /// <summary>
    /// Indicates whether column index is relative.
    /// </summary>
    private bool m_bSecondColumnRelative;
    #endregion

    #region Class overrides
    /// <summary>
    /// Tries to parse argument string.
    /// </summary>
    /// <param name="strArgument">Argument to parse.</param>
    /// <returns>Parsed argument if possible; null otherwise.</returns>
    public override MarkerArgument TryParse( string strArgument )
    {
      if( strArgument == null || strArgument.Length == 0 ) return null;

      string[] arrParts = strArgument.Split( DEF_PARTS_SEPARATOR );
      int iLength = arrParts.Length;

      if( arrParts[ 0 ].ToLower() != DEF_COPYRANGE ) return null;

      if( iLength > 1 )
      {
        m_bCopyStyles = arrParts[ iLength - 1 ].ToLower() == DEF_COPY_STYLES;

        if( m_bCopyStyles ) iLength--;
      }

      int iCellIndex = 1;

      if( iCellIndex < iLength )
      {
        if( !TryParseCell( arrParts[ iCellIndex ], out m_iRow, out m_iColumn,
          out m_bRowRelative, out m_bColumnRelative ) ) return null;
        iCellIndex++;
      }

      if( iCellIndex < iLength )
      {
        if( !TryParseCell( arrParts[ iCellIndex ], out m_iSecondRow, out m_iSecondColumn,
          out m_bSecondRowRelative, out m_bSecondColumnRelative ) ) return null;
      }
      else
      {
        m_iSecondRow = m_iRow;
        m_iSecondColumn = m_iColumn;
        m_bSecondRowRelative = m_bRowRelative;
        m_bSecondColumnRelative = m_bColumnRelative;
      }

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
      int iRowDelta = iRow - pOldPosition.X;
      int iColumnDelta = iColumn - pOldPosition.Y;

      Point pRangeStart = GetCellLocation( pOldPosition, options.Workbook );
      Point pRangeEnd = GetCellLocation( pOldPosition, m_iSecondRow, m_iSecondColumn,
        m_bSecondRowRelative, m_bSecondColumnRelative, options.Workbook );

      Point pDestinationRangeStart = pRangeStart;

      pDestinationRangeStart.Offset( iRowDelta, iColumnDelta );

      // Here we have to copy range.
      WorksheetImpl worksheet = ( WorksheetImpl )sheet;
      IRange destRange = sheet.Range[ pDestinationRangeStart.X, pDestinationRangeStart.Y ];
      IRange sourceRange = sheet.Range[ pRangeStart.X, pRangeStart.Y, pRangeEnd.X, pRangeEnd.Y ];

      ExcelCopyRangeOptions copyOptions = m_bCopyStyles
        ? DEF_DEFAULT_COPY_OPTIONS
        : ExcelCopyRangeOptions.All;

      worksheet.CopyRange( destRange, sourceRange, copyOptions );
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
        return base.Priority + 1;
      }
    }
    /// <summary>
    /// Indicates whether argument can be present multiple times in single
    /// template marker. Read-only.
    /// </summary>
    public override bool IsAllowMultiple
    {
      get
      {
        return true;
      }
    }

    #endregion
  }
}
