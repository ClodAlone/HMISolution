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
using System.Text;
using System.Collections;
using System.Globalization;

using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System.Collections.Generic;
#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#endif

#if SILVERLIGHT
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Summary description for RangesCollection.
  /// </summary>
  public class RangesCollection
    : CollectionBaseEx<IRange>
    ,IEnumerable<IRange>
    , IRanges
    , ICombinedRange
    , INativePTG
  {
    #region Class constants
    /// <summary>
    /// Error message for wrong worksheet exception.
    /// </summary>
    private const string DEF_WRONG_WORKSHEET = "Can't operate with ranges from different worksheet";
    #endregion

    #region Class members
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private IWorksheet m_worksheet;
    /// <summary>
    /// One-based first row index.
    /// </summary>
    private int m_iFirstRow;
    /// <summary>
    /// One-based first column index.
    /// </summary>
    private int m_iFirstColumn;
    /// <summary>
    /// One-based last row index.
    /// </summary>
    private int m_iLastRow;
    /// <summary>
    /// One-based last column index.
    /// </summary>
    private int m_iLastColumn;
    /// <summary>
    /// Rich text string.
    /// </summary>
    private RTFStringArray m_rtfString;
//    /// <summary>
//    /// Indicates whether collection is disposed.
//    /// </summary>
//    private bool m_bDisposed;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of RangesCollection.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    public RangesCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
      m_iFirstRow = m_worksheet.Workbook.MaxRowCount + 1;
      m_iFirstColumn = m_worksheet.Workbook.MaxColumnCount + 1;
    }
    /// <summary>
    /// Searches for all necessary parents of this collection.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">
    /// If can't find parent of some class.
    /// </exception>
    private void SetParents()
    {
      m_worksheet = FindParent( typeof( IWorksheet ) ) as IWorksheet;

      if( m_worksheet == null )
        throw new ArgumentNullException( "Worksheet", "Can't find parent worksheet" );
    }

    #endregion

    #region IRange Members
    /// <summary>
    /// Returns the range reference in the language of the macro.
    /// Read-only String.
    /// </summary>
    public string Address
    {
      get
      {
        CheckDisposed();
        StringBuilder builder = new StringBuilder();
        int iCount = Count;

        if( iCount == 0 ) return string.Empty;

        IRange range = ( IRange )InnerList[ 0 ];
        builder.Append( range.Address );
        string strAddressSeparator = GetAddressSeparator();

        for( int i = 1, len = Count; i < len; i++ )
        {
          builder.Append( strAddressSeparator );

          range = ( IRange )InnerList[ i ];
          builder.Append( range.Address );
        }

        return builder.ToString();
      }
    }

    /// <summary>
    /// Returns the range reference for the specified range in the language
    /// of the user. Read-only String.
    /// </summary>
    public string AddressLocal
    {
      get
      {
        CheckDisposed();
        StringBuilder builder = new StringBuilder();
        int iCount = Count;

        if( iCount == 0 ) return string.Empty;

        IRange range = ( IRange )InnerList[ 0 ];
        builder.Append( range.AddressLocal );
        string strAddressSeparator = GetAddressSeparator();

        for( int i = 1, len = Count; i < len; i++ )
        {
          builder.Append( strAddressSeparator );

          range = ( IRange )InnerList[ i ];
          builder.Append( range.AddressLocal );
        }

        return builder.ToString();
      }
    }

    /// <summary>
    /// Returns the range reference in the language of the macro. 
    /// Read-only String.
    /// </summary>
    public string AddressGlobal
    {
      get
      {
        CheckDisposed();
        StringBuilder builder = new StringBuilder();
        int iCount = Count;

        if( iCount == 0 ) return string.Empty;

        IRange range = ( IRange )InnerList[ 0 ];
        builder.Append( range.AddressGlobal );

        string strAddressSeparator = GetAddressSeparator();

        for( int i = 1, len = Count; i < len; i++ )
        {
          builder.Append( strAddressSeparator );

          range = ( IRange )InnerList[ i ];
          builder.Append( range.AddressGlobal );
        }

        return builder.ToString();
      }
    }
    /// <summary>
    /// Returns the range reference in the language of the macro using R1C1-style reference.
    /// Read-only String.
    /// </summary>
    public string AddressR1C1
    {
      get
      {
        CheckDisposed();
        StringBuilder builder = new StringBuilder();
        int iCount = Count;

        if( iCount == 0 ) return string.Empty;

        IRange range = ( IRange )InnerList[ 0 ];
        builder.Append( range.AddressR1C1 );
        string strAddressSeparator = GetAddressSeparator();

        for( int i = 1, len = Count; i < len; i++ )
        {
          builder.Append( strAddressSeparator );

          range = ( IRange )InnerList[ i ];
          builder.Append( range.AddressR1C1 );
        }

        return builder.ToString();
      }
    }

    /// <summary>
    /// Returns the range reference for the specified range in the language
    /// of the user using R1C1 style reference . Read-only String.
    /// </summary>
    public string AddressR1C1Local
    {
      get
      {
        CheckDisposed();
        StringBuilder builder = new StringBuilder();
        int iCount = Count;

        if( iCount == 0 ) return string.Empty;

        IRange range = ( IRange )InnerList[ 0 ];
        builder.Append( range.AddressR1C1Local );
        string strAddressSeparator = GetAddressSeparator();

        for( int i = 1, len = Count; i < len; i++ )
        {
          builder.Append( strAddressSeparator );

          range = ( IRange )InnerList[ i ];
          builder.Append( range.AddressR1C1Local );
        }

        return builder.ToString();
      }
    }

    /// <summary>
    /// Gets / sets boolean value that is contained by this range.
    /// </summary>
    public bool Boolean
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.Boolean;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.Boolean ) return false;
        }

        return bResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.Boolean = value;
        }
      }
    }

    /// <summary>
    /// Returns a  Borders collection that represents the borders of a style
    /// or a range of cells (including a range defined as part of a
    /// conditional format).
    /// </summary>
    public IBorders Borders
    {
      get
      {
        CheckDisposed();
        return CellStyle.Borders;
      }
    }

    /// <summary>
    /// Returns a Range object that represents the cells in the specified range.
    /// For big number of ranges can be very slow operation. Read-only.
    /// </summary>
    public IRange[] Cells
    {
      get
      {
        CheckDisposed();

        List<IRange> arrResult = new List<IRange>();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = InnerList[ i ];
          arrResult.AddRange( range.Cells );
        }

        return arrResult.ToArray();
      }
    }

    /// <summary>
    /// Returns the number of the first column in the first area in the specified
    /// range. Read-only.
    /// </summary>
    public int Column
    {
      get
      {
        CheckDisposed();
        return m_iFirstColumn;
      }
    }

    /// <summary>
    /// Column group level. Read-only.
    /// -1 - Not all columns in the range have same group level.
    /// 0 - No grouping,
    /// 1 - 7 - Group level.
    /// </summary>
    public int ColumnGroupLevel
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return int.MinValue;

        IRange range = ( IRange )InnerList[ 0 ];
        int iResult = range.ColumnGroupLevel;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( iResult != range.ColumnGroupLevel ) return int.MinValue;
        }

        return iResult;
      }
    }

    /// <summary>
    /// Returns or sets the width of all columns in the specified range.
    /// Read/write Double.
    /// </summary>
    public double ColumnWidth
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return double.MinValue;

        IRange range = ( IRange )InnerList[ 0 ];
        double dResult = range.ColumnWidth;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( dResult != range.ColumnWidth ) return double.MinValue;
        }

        return dResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.ColumnWidth = value;
        }
      }
    }

    /// <summary>
    /// Returns the number of objects in the collection. Read-only.
    /// </summary>
    int IRange.Count
    {
      get
      {
        CheckDisposed();

        int iResult = 0;

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          iResult += range.Count;
        }

        return iResult;
      }
    }

    /// <summary>
    /// Gets / sets DateTime contained by this cell. Read-write DateTime.
    /// </summary>
    public DateTime DateTime
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return DateTime.MinValue;

        IRange range = ( IRange )InnerList[ 0 ];
        DateTime dateResult = range.DateTime;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( dateResult != range.DateTime ) return DateTime.MinValue;
        }

        return dateResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.DateTime = value;
        }
      }
    }

    /// <summary>
    /// Returns cell value after number format application. Read-only.
    /// </summary>
    public string DisplayText
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return null;

        IRange range = ( IRange )InnerList[ 0 ];
        string strResult = range.DisplayText;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( strResult != range.DisplayText ) return null;
        }

        return strResult;
      }
    }

    /// <summary>
    /// Returns a Range object that represents the cell at the end of the
    /// region that contains the source range.
    /// </summary>
    public IRange End
    {
      get
      {
        CheckDisposed();

        if( m_iLastRow < 1 || m_iLastColumn < 1 ) return null;

        return Worksheet[ m_iLastRow, m_iLastColumn ];
      }
    }

    /// <summary>
    /// Returns a Range object that represents the entire column (or
    /// columns) that contains the specified range. Read-only.
    /// </summary>
    public IRange EntireColumn
    {
      get
      {
        CheckDisposed();
        return GetEntireColumnRow( true );
      }
    }

    /// <summary>
    /// Returns a Range object that represents the entire row (or
    /// rows) that contains the specified range. Read-only.
    /// </summary>
    public IRange EntireRow
    {
      get
      {
        CheckDisposed();
        return GetEntireColumnRow( false );
      }
    }

    /// <summary>
    /// Gets / sets error value that is contained by this range.
    /// </summary>
    public string Error
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return null;

        IRange range = ( IRange )InnerList[ 0 ];
        string strResult = range.Error;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( strResult != range.Error ) return null;
        }

        return strResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.Error = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the object's formula in A1-style notation and in
    /// the language of the macro. Read/write Variant.
    /// </summary>
    public string Formula
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return null;

        IRange range = ( IRange )InnerList[ 0 ];
        string strResult = range.Formula;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( strResult != range.Formula ) return null;
        }

        return strResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.Formula = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the object's formula in R1C1-style notation and in
    /// the language of the macro. Read/write Variant.
    /// </summary>
    public string FormulaR1C1
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return null;

        IRange range = ( IRange )InnerList[ 0 ];
        string strResult = range.FormulaR1C1;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( strResult != range.FormulaR1C1 ) return null;
        }

        return strResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.FormulaR1C1 = value;
        }
      }
    }

    /// <summary>
    /// Represents array-entered formula.
    /// Visit http://www.cpearson.com/excel/array.htm for more information.
    /// </summary>
    public string FormulaArray
    {
      get
      {
        CheckDisposed();
        int iCount = Count;

        if( iCount == 0 ) return null;

        IRange range = ( IRange )InnerList[ 0 ];
        string strResult = range.FormulaArray;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( strResult != range.FormulaArray ) return null;
        }

        return strResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.FormulaArray = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the array-entered formula in R1C1-style notation and in
    /// the language of the macro. Read/write Variant.
    /// </summary>
    public string FormulaArrayR1C1
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return null;

        IRange range = ( IRange )InnerList[ 0 ];
        string strResult = range.FormulaArrayR1C1;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( strResult != range.FormulaArrayR1C1 ) return null;
        }

        return strResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.FormulaArrayR1C1 = value;
        }
      }
    }

    /// <summary>
    /// True if the formula will be hidden when the worksheet is protected.
    /// False if at least part of formula in the range is not hidden.
    /// </summary>
    public bool FormulaHidden
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.FormulaHidden;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.FormulaHidden ) return false;
        }

        return bResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.FormulaHidden = value;
        }
      }
    }

    /// <summary>
    /// Get / set formula DateTime value contained by this cell.
    /// DateTime.MinValue if not all cells of the range have same DateTime value.
    /// </summary>
    public DateTime FormulaDateTime
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return DateTime.MinValue;

        IRange range = ( IRange )InnerList[ 0 ];
        DateTime dateResult = range.FormulaDateTime;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( dateResult != range.FormulaDateTime ) return DateTime.MinValue;
        }

        return dateResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.FormulaDateTime = value;
        }
      }
    }

    /// <summary>
    /// Indicates whether specified range object has data validation.
    /// If Range is not single cell, then returns true only if all cells have data validation. Read-only.
    /// </summary>
    public bool HasDataValidation
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.HasDataValidation;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.HasDataValidation )
          {
            bResult = false;
            break;
          }
        }

        return bResult;
      }
    }

    /// <summary>
    /// Indicates whether range contains bool value. Read-only.
    /// </summary>
    public bool HasBoolean
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 )
          return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.HasBoolean;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.HasBoolean )
            return false;
        }

        return bResult;
      }
    }
    /// <summary>
    /// Indicates whether range contains DateTime value. Read-only.
    /// </summary>
    public bool HasDateTime
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.HasDateTime;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.HasDateTime ) return false;
        }

        return bResult;
      }
    }

    /// <summary>
    /// Indicates if current range has formula bool value. Read-only.
    /// </summary>
    public bool HasFormulaBoolValue
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.HasFormulaBoolValue;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.HasFormulaBoolValue ) return false;
        }

        return bResult;
      }
    }
    /// <summary>
    /// Indicates if current range has formula error value. Read-only.
    /// </summary>
    public bool HasFormulaErrorValue
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.HasFormulaErrorValue;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.HasFormulaErrorValue ) return false;
        }

        return bResult;
      }
    }
    /// <summary>
    /// Indicates if current range has formula value formatted as DateTime. Read-only.
    /// </summary>
    public bool HasFormulaDateTime
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.HasFormulaDateTime;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.HasFormulaDateTime ) return false;
        }

        return bResult;
      }
    }
    public bool HasFormulaNumberValue
    {
        get
        {
            CheckDisposed();

            int iCount = Count;

            if (iCount == 0) return false;

            IRange range = (IRange)InnerList[0];
            bool bResult = range.HasFormulaNumberValue;

            for (int i = 0, len = Count; i < len; i++)
            {
                range = (IRange)InnerList[i];

                if (bResult != range.HasFormulaNumberValue) return false;
            }

            return bResult;
        }
    }

    public bool HasFormulaStringValue
    {
        get
        {
            CheckDisposed();

            int iCount = Count;

            if (iCount == 0) return false;

            IRange range = (IRange)InnerList[0];
            bool bResult = range.HasFormulaStringValue;

            for (int i = 0, len = Count; i < len; i++)
            {
                range = (IRange)InnerList[i];

                if (bResult != range.HasFormulaStringValue) return false;
            }

            return bResult;
        }
    }
    /// <summary>
    /// True if all cells in the range contain formulas; False if
    /// at least one of the cells in the range doesn't contain a formula.
    /// Read-only Boolean.
    /// </summary>
    public bool HasFormula
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.HasFormula;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.HasFormula ) return false;
        }

        return bResult;
      }
    }

    /// <summary>
    /// Indicates whether range contains array-entered formula. Read-only.
    /// </summary>
    public bool HasFormulaArray
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.HasFormulaArray;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.HasFormulaArray ) return false;
        }

        return bResult;
      }
    }

    /// <summary>
    /// Indicates whether the range contains number. Read-only.
    /// </summary>
    public bool HasNumber
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.HasNumber;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.HasNumber ) return false;
        }

        return bResult;
      }
    }

    /// <summary>
    /// Indicates whether cell contains formatted rich text string.
    /// </summary>
    public bool HasRichText
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.HasRichText;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.HasRichText ) return false;
        }

        return bResult;
      }
    }

    /// <summary>
    /// Indicates whether the range contains String. Read-only.
    /// </summary>
    public bool HasString
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.HasString;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.HasString ) return false;
        }

        return bResult;
      }
    }

    /// <summary>
    /// Indicates whether range has default style. False means default style.
    /// Read-only.
    /// </summary>
    public bool HasStyle
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.HasStyle;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.HasStyle ) return false;
        }

        return bResult;
      }
    }

    /// <summary>
    /// Returns or sets the horizontal alignment for the specified object.
    /// Read/write ExcelHAlign.
    /// </summary>
    public ExcelHAlign HorizontalAlignment
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return ExcelHAlign.HAlignGeneral;

        IRange range = ( IRange )InnerList[ 0 ];
        ExcelHAlign alignResult = range.HorizontalAlignment;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( alignResult != range.HorizontalAlignment )
            return ExcelHAlign.HAlignGeneral;
        }

        return alignResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.HorizontalAlignment = value;
        }
      }
    }

    /// <summary>
    /// Returns hyperlinks for this ranges collection.
    /// </summary>
    public IHyperLinks Hyperlinks
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 )
          return null;

        HyperLinksCollection links = new HyperLinksCollection( Application, this, true );
        IRange range;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          HyperLinksCollection rangeLinks = ( HyperLinksCollection )range.Hyperlinks;

          if( rangeLinks != null )
          {
            links.AddRange( rangeLinks );
          }
        }

        return links;
      }
    }

    /// <summary>
    /// Returns or sets the indent level for the cell or range. Can be an integer
    /// from 0 to 15 for Excel 97-2003 and 250 for Excel 2007. Read/write Integer.
    /// </summary>
    public int IndentLevel
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return int.MinValue;

        IRange range = ( IRange )InnerList[ 0 ];
        int iResult = range.IndentLevel;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( iResult != range.IndentLevel ) return int.MinValue;
        }

        return iResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.IndentLevel = value;
        }
      }
    }

    /// <summary>
    /// Indicates whether the range is blank. Read-only.
    /// </summary>
    public bool IsBlank
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.IsBlank;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.IsBlank ) return false;
        }

        return bResult;
      }
    }

    /// <summary>
    /// Indicates whether range contains boolean value. Read-only.
    /// </summary>
    public bool IsBoolean
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.IsBoolean;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.IsBoolean ) return false;
        }

        return bResult;
      }
    }

    /// <summary>
    /// Indicates whether range contains error value.
    /// </summary>
    public bool IsError
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.IsError;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.IsError ) return false;
        }

        return bResult;
      }
    }

    /// <summary>
    /// Indicates whether this range is grouped by column. Read-only.
    /// </summary>
    public bool IsGroupedByColumn
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.IsGroupedByColumn;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.IsGroupedByColumn ) return false;
        }

        return bResult;
      }
    }

    /// <summary>
    /// Indicates whether this range is grouped by row. Read-only.
    /// </summary>
    public bool IsGroupedByRow
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.IsGroupedByRow;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.IsGroupedByRow ) return false;
        }

        return bResult;
      }
    }

    /// <summary>
    /// Indicates whether cell is initialized. Read-only.
    /// </summary>
    public bool IsInitialized
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.IsInitialized;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.IsInitialized ) return false;
        }

        return bResult;
      }
    }

    /// <summary>
    /// Returns last column of the range. Read-only.
    /// </summary>
    public int LastColumn
    {
      get
      {
        return m_iLastColumn;
      }
    }
    /// <summary>
    /// Returns last row of the range. Read-only.
    /// </summary>
    public int LastRow
    {
      get
      {
        return m_iLastRow;
      }
    }
    /// <summary>
    /// Gets / sets double value of the range.
    /// </summary>
    public double Number
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return double.MinValue;

        IRange range = ( IRange )InnerList[ 0 ];
        double dResult = range.Number;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( dResult != range.Number ) return double.MinValue;
        }

        return dResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.Number = value;
        }
      }
    }

    /// <summary>
    /// Format of current cell. Analog of Style.NumberFormat property.
    /// </summary>
    public string NumberFormat
    {
      get
      {
        CheckDisposed();
        return RangeImpl.GetNumberFormat( InnerList );
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.NumberFormat = value;
        }
      }
    }

    /// <summary>
    /// Returns the number of the first row of the first area in
    /// the range. Read-only Long.
    /// </summary>
    public int Row
    {
      get
      {
        CheckDisposed();
        return m_iFirstRow;
      }
    }

    /// <summary>
    /// Row group level. Read-only.
    /// -1 - Not all rows in the range have same group level.
    /// 0 - No grouping,
    /// 1 - 7 - Group level.
    /// </summary>
    public int RowGroupLevel
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return int.MinValue;

        IRange range = ( IRange )InnerList[ 0 ];
        int iResult = range.RowGroupLevel;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( iResult != range.RowGroupLevel ) return int.MinValue;
        }

        return iResult;
      }
    }

    /// <summary>
    /// Returns the height of all the rows in the range specified,
    /// measured in points. Returns Double.MinValue if the rows in the specified range
    /// aren't all the same height. Read / write Double.
    /// </summary>
    public double RowHeight
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return double.MinValue;

        IRange range = ( IRange )InnerList[ 0 ];
        double dResult = range.RowHeight;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( dResult != range.RowHeight ) return double.MinValue;
        }

        return dResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.RowHeight = value;
        }
      }
    }

    /// <summary>
    /// For a Range object, returns an array of Range objects that represent the
    /// rows in the specified range.
    /// </summary>
    public IRange[] Rows
    {
      get
      {
        CheckDisposed();
        return GetColumnRows( false );
      }
    }

    /// <summary>
    /// For a Range object, returns an array of Range objects that represent the
    /// columns in the specified range.
    /// </summary>
    public IRange[] Columns
    {
      get
      {
        CheckDisposed();
        return GetColumnRows( true );
      }
    }

    /// <summary>
    /// Returns a Style object that represents the style of the specified
    /// range. Read/write IStyle.
    /// </summary>
    public IStyle CellStyle
    {
      get
      {
        CheckDisposed();
        return new StyleArrayWrapper( this );
      }
      set
      {
        CheckDisposed();

        if( value == null )
          throw new ArgumentNullException( "CellStyle" );

        CellStyleName = value.Name;
      }
    }

    /// <summary>
    /// Returns name of the Style object that represents the style of the specified
    /// range. Read/write String.
    /// </summary>
    public string CellStyleName
    {
      get
      {
        CheckDisposed();
        return RangeImpl.GetCellStyleName( List );
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.CellStyleName = value;
        }
      }
    }

    /// <summary>
    /// Gets / sets string value of the range.
    /// </summary>
    public string Text
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return null;

        IRange range = ( IRange )InnerList[ 0 ];
        string strResult = range.Text;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( strResult != range.Text ) return null;
        }

        return strResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.Text = value;
        }
      }
    }

    /// <summary>
    /// Gets / sets time value of the range.
    /// </summary>
    public TimeSpan TimeSpan
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return TimeSpan.MinValue;

        IRange range = ( IRange )InnerList[ 0 ];
        TimeSpan timeResult = range.TimeSpan;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( timeResult != range.TimeSpan ) return TimeSpan.MinValue;
        }

        return timeResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.TimeSpan = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the value of the specified range.
    /// Read/write Variant.
    /// </summary>
    public string Value
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return null;

        IRange range = ( IRange )InnerList[ 0 ];
        string strResult = range.Value;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( strResult != range.Value ) return null;
        }

        return strResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.Value = value;
        }
      }
    }
     
    /// <summary>
    /// Returns the calculated value of a formula using the most current inputs.
    /// </summary>
    public string CalculatedValue
    {
        get
        {
            if (Parent is IWorksheet && ((IWorksheet)Parent).CalcEngine != null)
            {
                string cellRef = Syncfusion.Calculate.RangeInfo.GetAlphaLabel(Column) + Row.ToString();
                return ((IWorksheet)Parent).CalcEngine.PullUpdatedValue(cellRef);
            }
            return null;
        }
    }

    /// <summary>
    /// Returns or sets the cell value. Read/write Variant.
    /// The only difference between this property and the Value property is
    /// that the Value2 property doesn't use the Currency and Date data types.
    /// </summary>
    public object Value2
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return null;

        IRange range = ( IRange )InnerList[ 0 ];
        object objResult = range.Value2;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( !objResult.Equals( range.Value2 ) ) return null;
        }

        return objResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.Value2 = value;
        }
      }
    }

    /// <summary>
    /// Returns or sets the vertical alignment of the specified object.
    /// Read/write ExcelVAlign.
    /// </summary>
    public ExcelVAlign VerticalAlignment
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return ExcelVAlign.VAlignTop;

        IRange range = ( IRange )InnerList[ 0 ];
        ExcelVAlign alignResult = range.VerticalAlignment;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( alignResult != range.VerticalAlignment ) return ExcelVAlign.VAlignTop;
        }

        return alignResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.VerticalAlignment = value;
        }
      }
    }

    /// <summary>
    /// Returns a Worksheet object that represents the worksheet
    /// containing the specified range. Read-only.
    /// </summary>
    public IWorksheet Worksheet
    {
      get
      {
        CheckDisposed();

        return m_worksheet;
      }
    }

    /// <summary>
    /// Gets / sets cell by row and index.
    /// </summary>
    public IRange this[ int row, int column ]
    {
      get
      {
        CheckDisposed();
        return Worksheet.UsedRange[ row, column ];
      }
      set
      {
        CheckDisposed();
        Worksheet.UsedRange[ row, column ] = value;
      }
    }

    /// <summary>
    /// Get cell range.
    /// </summary>
    public IRange this[ int row, int column, int lastRow, int lastColumn ]
    {
      get
      {
        CheckDisposed();
        return Worksheet.UsedRange[ row, column, lastRow, lastColumn ];
      }
    }

    /// <summary>
    /// Get cell range.
    /// </summary>
    public IRange this[ string name ]
    {
      get
      {
        return this[ name, false ];
      }
    }
    /// <summary>
    /// Gets cell range. Read-only.
    /// </summary>
    public IRange this[ string name, bool IsR1C1Notation ]
    {
      get
      {
        CheckDisposed();
        return Worksheet.UsedRange[ name, IsR1C1Notation ];
      }
    }
    /// <summary>
    /// Collection of conditional formats.
    /// </summary>
    public IConditionalFormats ConditionalFormats
    {
      get
      {
        CheckDisposed();
        return AppImplementation.CreateCondFormatCollectionWrapper( this );
      }
    }

    /// <summary>
    /// Data validation for the range.
    /// </summary>
    public IDataValidation DataValidation
    {
      get
      {
        CheckDisposed();
        return AppImplementation.CreateDataValidationArrayImpl( this );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public string FormulaStringValue
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return null;

        IRange range = ( IRange )InnerList[ 0 ];
        string strResult = range.FormulaStringValue;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( strResult != range.FormulaStringValue ) return null;
        }

        return strResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.FormulaStringValue = value;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public double FormulaNumberValue
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return double.MinValue;

        IRange range = ( IRange )InnerList[ 0 ];
        double dResult = range.FormulaNumberValue;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( dResult != range.FormulaNumberValue ) return double.MinValue;
        }

        return dResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.FormulaNumberValue = value;
        }
      }
    }
    /// <summary>
    /// Returns the calculated value of the formula as a boolean.
    /// </summary>
    public bool FormulaBoolValue
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.FormulaBoolValue;

        for( int i = 1, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.FormulaBoolValue )
            return false;
        }

        return bResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.FormulaBoolValue = value;
        }
      }
    }
    /// <summary>
    /// Returns the calculated value of the formula as a string.
    /// </summary>
    public string FormulaErrorValue
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return null;

        IRange range = ( IRange )InnerList[ 0 ];
        string strResult = range.FormulaErrorValue;

        for( int i = 1, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( strResult != range.FormulaErrorValue )
            return null;
        }

        return strResult;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.FormulaErrorValue = value;
        }
      }
    }
    /// <summary>
    /// Comment assigned to the range. Read-only.
    /// </summary>
    public ICommentShape Comment
    {
      get
      {
        CheckDisposed();
        return ( ( ApplicationImpl ) Application ).CreateCommentsRange( this );
      }
    }
    /// <summary>
    /// String with rich text formatting. Read-only.
    /// </summary>
    public IRichTextString RichText
    {
      get
      {
        CheckDisposed();

        if( m_rtfString == null )
        {
          m_rtfString = new RTFStringArray( this );
        }

        return m_rtfString;
      }
    }
    /// <summary>
    /// Indicates whether this range is part of merged range. Read-only.
    /// </summary>
    public bool IsMerged
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return false;

        IRange range = ( IRange )InnerList[ 0 ];
        bool bResult = range.IsMerged;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( bResult != range.IsMerged ) return false;
        }

        return bResult;
      }
    }
    /// <summary>
    /// Returns a Range object that represents the merged range containing
    /// the specified cell. If the specified cell isn�t in a merged range,
    /// this property returns NULL. Read-only.
    /// </summary>
    public IRange MergeArea
    {
      get
      {
        CheckDisposed();

        RangesCollection result = AppImplementation.CreateRangesCollection( Worksheet );

        for( int i = 0, len = Count; i < len; i++ )
        {
          RangeImpl range = ( RangeImpl )InnerList[ i ];
          result.Add( range.MergeArea );
        }

        return result;
      }
    }
    /// <summary>
    /// True if Microsoft Excel wraps the text in the object.
    /// Read/write Boolean.
    /// </summary>
    public bool         WrapText
    {
      get
      {
        CheckDisposed();
        return RangeImpl.GetWrapText( Cells );
      }
      set
      {
        CheckDisposed();
        RangeImpl.SetWrapText( Cells, value );
      }
    }
    /// <summary>
    /// Indicates is current range has external formula. Read-only.
    /// </summary>
    public bool HasExternalFormula
    {
      get
      {
        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];

          if( !range.HasExternalFormula )
          {
            return false;
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Represents ignore error options. If not single cell returs concatenateed flags.
    /// </summary>
    public ExcelIgnoreError IgnoreErrorOptions
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 ) return ExcelIgnoreError.None;

        ExcelIgnoreError result = ExcelIgnoreError.All;
        IList list = InnerList;

        for( int i = 0, len = Count; i < len && result != ExcelIgnoreError.None; i++ )
        {
          IRange range = ( IRange )list[ i ];

          result &= range.IgnoreErrorOptions;
        }

        return result;
      }
      set
      {
        CheckDisposed();
        IList list = InnerList;

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )list[ i ];
          range.IgnoreErrorOptions = value;
        }
      }
    }
    /// <summary>
    /// Indicates whether all values in the range are preserved as strings.
    /// </summary>
    public bool? IsStringsPreserved
    {
      get
      {
        return ( m_worksheet as WorksheetImpl ).GetStringPreservedValue( this );
      }
      set
      {
        ( m_worksheet as WorksheetImpl ).SetStringPreservedValue( this, value );
      }
    }
    /// <summary>
    /// Gets/sets built in style.
    /// </summary>
    public BuiltInStyles? BuiltInStyle
    {
      get
      {
        CheckDisposed();

        int iCount = Count;

        if( iCount == 0 )
          return null;

        IRange range = ( IRange )InnerList[ 0 ];
        BuiltInStyles? result = range.BuiltInStyle;

        for( int i = 0, len = Count; i < len; i++ )
        {
          range = ( IRange )InnerList[ i ];

          if( result != range.BuiltInStyle )
          {
            result = null;
            break;
          }
        }

        return result;
      }
      set
      {
        CheckDisposed();

        for( int i = 0, len = Count; i < len; i++ )
        {
          IRange range = ( IRange )InnerList[ i ];
          range.BuiltInStyle = value;
        }
      }
    }
    /// <summary>
    /// Gets address global in the format required by Excel 2007.
    /// </summary>
    public string AddressGlobal2007
    {
      get
      {
        return AddressGlobal;
      }
    }
    #endregion

    #region IRange methods
    /// <summary>
    /// Activates a single cell, which must be inside the current selection.
    /// To select a range of cells, use the Select method.
    /// </summary>
    /// <returns></returns>
    public IRange Activate()
    {
      CheckDisposed();
      return null;
    }
      /// <summary>
      /// Activages a single cell, scroll to it and activates the respective sheet
      /// To select a range of cells, use the Select method.
      /// </summary>
    /// <param name="scroll">True to scroll to the cell</param>
      /// <returns></returns>
    public IRange Activate(bool scroll)
    {
        CheckDisposed();
        return null;
    }
    /// <summary>
    /// This method groups current range.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should
    /// be performed by rows or by columns. 
    /// </param>
    /// <returns>Current range after grouping.</returns>
    public IRange Group( ExcelGroupBy groupBy )
    {
      CheckDisposed();

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.Group( groupBy );
      }

      return this;
    }

    /// <summary>
    /// This method groups current range.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether grouping should
    /// be performed by rows or by columns. 
    /// </param>
    /// <param name="bCollapsed">Indicates whether group should be collapsed.</param>
    /// <returns>Current range after grouping.</returns>
    public IRange Group( ExcelGroupBy groupBy, bool bCollapsed )
    {
      CheckDisposed();

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.Group( groupBy, bCollapsed );
      }

      return this;
    }
    /// <summary>
    /// Creates Subtotal for the corresponding ranges
    /// </summary>
    /// <param name="groupBy">GroupBy</param>
    /// <param name="function">ConsolidationFunction</param>
    /// <param name="totalList">TotalList</param>
    public void SubTotal(int groupBy, ConsolidationFunction function, int[] totalList)
    {
        throw new NotSupportedException();
    }
    /// <summary>
    /// Creates SubTotal for the corresponding Ranges
    /// </summary>
    /// <param name="groupBy">GroupByGroupBy</param>
    /// <param name="function">ConsolidationFunction</param>
    /// <param name="totalList">TotalList</param>
    /// <param name="replace">Replace exisiting SubTotal</param>
    /// <param name="pageBreaks">Insert PageBreaks</param>
    /// <param name="summaryBelowData">SummaryBelowData</param>
    public void SubTotal(int groupBy, ConsolidationFunction function, int[] totalList, bool replace, bool pageBreaks, bool summaryBelowData)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Creates a merged cell from the specified Range object.
    /// </summary>
    public void Merge()
    {
      CheckDisposed();

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.Merge();
      }
    }

    /// <summary>
    /// Creates a merged cell from the specified Range object.
    /// </summary>
    /// <param name="clearCells">Indicates whether to clear unnecessary cells.</param>
    public void Merge( bool clearCells )
    {
      CheckDisposed();

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.Merge( clearCells );
      }
    }

    /// <summary>
    /// Ungroups current range.
    /// </summary>
    /// <param name="groupBy">
    /// Indicates type of ungrouping. Ungroup by columns or by rows.
    /// </param>
    /// <returns>Current range after ungrouping.</returns>
    public IRange Ungroup( ExcelGroupBy groupBy )
    {
      CheckDisposed();

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.Ungroup( groupBy );
      }

      return this;
    }

    /// <summary>
    /// Separates a merged area into individual cells.
    /// </summary>
    public void UnMerge()
    {
      CheckDisposed();

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.UnMerge();
      }
    }

    /// <summary>
    /// Freezes pane at the current range.
    /// </summary>
    public void FreezePanes()
    {
      CheckDisposed();

      if( Count == 1 )
      {
        IRange range = ( IRange )InnerList[ 0 ];
        range.FreezePanes();
      }
    }

    /// <summary>
    /// Clear the contents of the Range.
    /// </summary>
    void IRange.Clear()
    {
      CheckDisposed();

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.Clear();
      }
    }

    /// <summary>
    /// Clear the contents of the Range with formatting.
    /// </summary>
    /// <param name="isClearFormat">True if formatting should also be cleared.</param>
    void IRange.Clear( bool isClearFormat )
    {
      CheckDisposed();

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.Clear( isClearFormat );
      }
    }
    /// <summary>
    /// Clears the cell based on clear options.
    /// </summary>
    /// <param name="option"></param>
    void IRange.Clear(ExcelClearOptions option)
    {
        CheckDisposed();

        for (int i = 0, len = Count; i < len; i++)
        {
            IRange range = (IRange)InnerList[i];
            range.Clear(option);
        }
    }

    /// <summary>
    /// Clear the contents of the Range and shifts the cells Up or Left
    /// without formula or merged ranges update.
    /// </summary>
    /// <param name="direction">Cells shift direction Up/Left.</param>
    void IRange.Clear( ExcelMoveDirection direction )
    {
      CheckDisposed();

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.Clear( direction );
      }
    }

    /// <summary>
    /// Clear the contents of the Range and shifts the cells Up or Left.
    /// </summary>
    /// <param name="direction">Cells shift direction Up/Left.</param>
    /// <param name="options">Cells shifting options.</param>
    void IRange.Clear( ExcelMoveDirection direction, ExcelCopyRangeOptions options )
    {
      CheckDisposed();

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.Clear( direction, options );
      }
    }

    /// <summary>
    /// Moves the cells to the specified Range (without updating formulas).
    /// </summary>
    /// <param name="destination">Destination Range.</param>
    public void MoveTo( IRange destination )
    {
      CheckDisposed();

      if( destination == null )
        throw new ArgumentNullException( "destination" );

      int iRowDelta = destination.Row - Row;
      int iColumnDelta = destination.Column - Column;

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        int iNewRow = range.Row + iRowDelta;
        int iNewColumn = range.Column + iColumnDelta;

        if( iNewRow <= m_worksheet.Workbook.MaxRowCount && iNewRow > 0
          && iNewColumn <= m_worksheet.Workbook.MaxColumnCount && iNewColumn > 0 )
        {
          range.MoveTo( destination.Worksheet[ iNewRow, iNewColumn ] );
        }
      }
    }

    /// <summary>
    /// Copies the range to the specified destination Range (without updating formulas).
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <returns>Range were this range was copied.</returns>
    public IRange CopyTo( IRange destination )
    {
      return CopyTo( destination, ExcelCopyRangeOptions.All );
    }

    /// <summary>
    /// Copies this range into another location.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="options">Copy range options.</param>
    /// <returns>Destination range.</returns>
    public IRange CopyTo( IRange destination, ExcelCopyRangeOptions options )
    {
      CheckDisposed();

      if( destination == null )
        throw new ArgumentNullException( "destination" );

      int iRowDelta = destination.Row - Row;
      int iColumnDelta = destination.Column - Column;

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        int iNewRow = range.Row + iRowDelta;
        int iNewColumn = range.Column + iColumnDelta;

        if( iNewRow <= m_worksheet.Workbook.MaxRowCount && iNewRow > 0
          && iNewColumn <= m_worksheet.Workbook.MaxColumnCount && iNewColumn > 0 )
        {
          range.CopyTo( destination.Worksheet[ iNewRow, iNewColumn ], options );
        }
      }

      return destination;
    }

    /// <summary>
    /// Returns intersection of this range with the specified one.
    /// </summary>
    /// <param name="range">The Range with which to intersect.</param>
    /// <returns>Range intersection; if there is no intersection, NULL is returned.</returns>
    public IRange IntersectWith( IRange range )
    {
      CheckDisposed();

      RangesCollection ranges = AppImplementation.CreateRangesCollection( Worksheet );
        //new RangesCollection( Application, this );

      for( int i = 0, len = ranges.Count; i < len; i++ )
      {
        IRange curRange = ( IRange )InnerList[ i ];
        ranges.Add( curRange.IntersectWith( range ) );
      }

      return ( ranges.Count > 0 ) ? ranges : null;
    }

    /// <summary>
    /// Returns merge of this range with the specified one.
    /// </summary>
    /// <param name="range">The Range to merge with.</param>
    /// <returns>Merged ranges or NULL if wasn't able to merge ranges.</returns>
    public IRange MergeWith( IRange range )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    public void AutofitRows()
    {
      CheckDisposed();

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.AutofitRows();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public void AutofitColumns()
    {
      CheckDisposed();

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.AutofitColumns();
      }
    }

    /// <summary>
    /// Adds comment to the range.
    /// </summary>
    /// <returns>Range's comment.</returns>
    public ICommentShape AddComment()
    {
      CheckDisposed();

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.AddComment();
      }

      return Comment;
    }
    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( string findValue, ExcelFindType flags )
    {
      CheckDisposed();

      if( findValue == null ) return null;

      bool bIsFormula = ( ( flags & ExcelFindType.Formula ) == ExcelFindType.Formula );
      bool bIsText = ( ( flags & ExcelFindType.Text ) == ExcelFindType.Text );
      bool bIsFormulaStringValue = ( ( flags & ExcelFindType.FormulaStringValue ) == ExcelFindType.FormulaStringValue );
      bool bIsError = ( ( flags & ExcelFindType.Error ) == ExcelFindType.Error );

      if( !( bIsFormula || bIsText || bIsFormulaStringValue || bIsError ) )
        throw new ArgumentException( "Parameter flag is not valid.", "flags" );

      IList list = InnerList;

      for( int i = 0, cellsCount = list.Count; i < cellsCount; i++ )
      {
        IRange cell = ( ( IRange )list[ i ] ).FindFirst( findValue, flags );

        if( cell != null )
        {
          return cell;
        }
      }

      return null;
    }
    /// <summary>
    /// This method searches for the first cell with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( double findValue, ExcelFindType flags )
    {
      CheckDisposed();

      bool bIsFormulaValue = ( ( flags & ExcelFindType.FormulaValue ) == ExcelFindType.FormulaValue );
      bool bIsNumber = ( ( flags & ExcelFindType.Number ) == ExcelFindType.Number );

      if( !( bIsFormulaValue || bIsNumber ) )
        throw new ArgumentException( "Parameter flag is not valid.", "flags" );

      IList list = InnerList;

      for( int i = 0, cellsCount = list.Count; i < cellsCount; i++ )
      {
        IRange cell = ( ( IRange )list[ i ] ).FindFirst( findValue, flags );

        if( cell != null )
        {
          return cell;
        }
      }

      return null;
    }

    /// <summary>
    /// This method searches for the first cell with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( bool findValue )
    {
      CheckDisposed();

      IList list = InnerList;

      for( int i = 0, cellsCount = list.Count; i < cellsCount; i++ )
      {
        IRange cell = ( ( IRange )list[ i ] ).FindFirst( findValue );

        if( cell != null )
        {
          return cell;
        }
      }

      return null;
    }

    /// <summary>
    /// This method searches for the first cell with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( DateTime findValue )
    {
      CheckDisposed();

      IList list = InnerList;

      for( int i = 0, cellsCount = list.Count; i < cellsCount; i++ )
      {
        IRange cell = ( ( IRange )list[ i ] ).FindFirst( findValue );

        if( cell != null )
        {
          return cell;
        }
      }

      return null;
    }
    /// <summary>
    /// This method searches for the first cell with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( TimeSpan findValue )
    {
      CheckDisposed();

      IList list = InnerList;

      for( int i = 0, cellsCount = list.Count; i < cellsCount; i++ )
      {
        IRange cell = ( ( IRange )list[ i ] ).FindFirst( findValue );

        if( cell != null )
        {
          return cell;
        }
      }

      return null;
    }
    /// <summary>
    /// This method searches for the all cells with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( DateTime findValue )
    {
      CheckDisposed();

      List<IRange> cellsArray = new List<IRange>();
      IList<IRange> list = InnerList;

      for( int i = 0, cellsCount = list.Count; i < cellsCount; i++ )
      {
        IRange cell = list[ i ];
        IRange[] ranges  = cell.FindAll( findValue );

        if( ranges != null )
        {
          cellsArray.AddRange( ranges );
        }
      }

      if( cellsArray.Count == 0 ) return null;

      return cellsArray.ToArray();
    }
    /// <summary>
    /// This method searches for the all cells with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( TimeSpan findValue )
    {
      CheckDisposed();

      List<IRange> cellsArray = new List<IRange>();
      List<IRange> list = InnerList;

      for( int i = 0, cellsCount = list.Count; i < cellsCount; i++ )
      {
        IRange cell = list[ i ];
        IRange[] ranges  = cell.FindAll( findValue );

        if( ranges != null )
        {
          cellsArray.AddRange( ranges );
        }
      }

      if( cellsArray.Count == 0 ) return null;

      return cellsArray.ToArray();
    }
    /// <summary>
    /// This method searches for the all cells with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( string findValue, ExcelFindType flags )
    {
      CheckDisposed();

      if( findValue == null ) return null;

      bool bIsFormula = ( ( flags & ExcelFindType.Formula ) == ExcelFindType.Formula );
      bool bIsText = ( ( flags & ExcelFindType.Text ) == ExcelFindType.Text );
      bool bIsFormulaStringValue = ( ( flags & ExcelFindType.FormulaStringValue ) == ExcelFindType.FormulaStringValue );
      bool bIsError = ( ( flags & ExcelFindType.Error ) == ExcelFindType.Error );

      if( !( bIsFormula || bIsText || bIsFormulaStringValue || bIsError ) )
        throw new ArgumentException( "Parameter flag is not valid.", "flags" );

      if( findValue == null )
        return null;

      List<IRange> cellsArray = new List<IRange>();
      List<IRange> list = InnerList;

      for( int i = 0, cellsCount = list.Count; i < cellsCount; i++ )
      {
        IRange cell = list[ i ];
        IRange[] ranges  = cell.FindAll( findValue, flags );

        if( ranges != null )
        {
          cellsArray.AddRange( ranges );
        }
      }

      if( cellsArray.Count == 0 ) return null;

      return cellsArray.ToArray();
    }
    ///<summary>
    /// This method searches for the all cells with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( double findValue, ExcelFindType flags )
    {
      CheckDisposed();

      bool bIsFormulaValue = ( ( flags & ExcelFindType.FormulaValue ) == ExcelFindType.FormulaValue );
      bool bIsNumber = ( ( flags & ExcelFindType.Number ) == ExcelFindType.Number );

      if( !( bIsFormulaValue || bIsNumber ) )
        throw new ArgumentException( "Parameter flag is not valid.", "flags" );

      List<IRange> cellsArray = new List<IRange>();
      List<IRange> list = InnerList;

      for( int i = 0, cellsCount = list.Count; i < cellsCount; i++ )
      {
        IRange cell = list[ i ];
        IRange[] ranges  = cell.FindAll( findValue, flags );

        if( ranges != null )
        {
          cellsArray.AddRange( ranges );
        }
      }

      if( cellsArray.Count == 0 ) return null;

      return cellsArray.ToArray();
    }
    /// <summary>
    /// This method searches for the all cells with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found</returns>
    public IRange[] FindAll( bool findValue )
    {
      CheckDisposed();

      List<IRange> cellsArray = new List<IRange>();
      List<IRange> list = InnerList;

      for( int i = 0, cellsCount = list.Count; i < cellsCount; i++ )
      {
        IRange cell = list[ i ];
        IRange[] ranges  = cell.FindAll( findValue );

        if( ranges != null )
        {
          cellsArray.AddRange( ranges );
        }
      }

      if( cellsArray.Count == 0 ) return null;

      return cellsArray.ToArray();
    }
    /// <summary>
    /// Copies range to the clipboard.
    /// </summary>
    public void CopyToClipboard()
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    public void BorderAround()
    {
      BorderAround( ExcelLineStyle.Thin );
    }
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    public void BorderAround( ExcelLineStyle borderLine )
    {
      BorderAround( borderLine, ExcelKnownColors.Black );
    }
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color.</param>
    public void BorderAround( ExcelLineStyle borderLine, Color borderColor )
    {
      ExcelKnownColors color = m_worksheet.Workbook.GetNearestColor( borderColor );

      BorderAround( borderLine, color );
    }
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color as ExcelKnownColors.</param>
    public void BorderAround( ExcelLineStyle borderLine, ExcelKnownColors borderColor )
    {
      for( int i = 0, iLen = Count; i < iLen; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.BorderAround( borderLine, borderColor );
      }
    }
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    public void BorderInside()
    {
      BorderInside( ExcelLineStyle.Thin );
    }
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    public void BorderInside( ExcelLineStyle borderLine )
    {
      BorderInside( borderLine, ExcelKnownColors.Black );
    }
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color.</param>
    public void BorderInside( ExcelLineStyle borderLine, Color borderColor )
    {
      ExcelKnownColors color = m_worksheet.Workbook.GetNearestColor( borderColor );

      BorderInside( borderLine, color );
    }
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color as ExcelKnownColors.</param>
    public void BorderInside( ExcelLineStyle borderLine, ExcelKnownColors borderColor )
    {
      for( int i = 0, iLen = Count; i < iLen; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.BorderInside( borderLine, borderColor );
      }
    }
    /// <summary>
    /// Sets none border for current range.
    /// </summary>
    public void BorderNone()
    {
      for( int i = 0, iLen = Count; i < iLen; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.BorderNone();
      }
    }
    /// <summary>
    /// Collapses current group.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    public void CollapseGroup( ExcelGroupBy groupBy )
    {
      for( int i = 0, iLen = Count; i < iLen; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.CollapseGroup( groupBy );
      }
    }
    /// <summary>
    /// Expands current group.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    public void ExpandGroup( ExcelGroupBy groupBy )
    {
      for( int i = 0, iLen = Count; i < iLen; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.ExpandGroup( groupBy );
      }
    }
    /// <summary>
    /// Expands current group.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    /// <param name="flags">Additional option flags.</param>
    public void ExpandGroup( ExcelGroupBy groupBy, ExpandCollapseFlags flags )
    {
      for( int i = 0, iLen = Count; i < iLen; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];
        range.ExpandGroup( groupBy, flags );
      }
    }
    #endregion

    #region ICombinedRange methods
    /// <summary>
    /// Gets new address of range.
    /// </summary>
    /// <param name="names">Dictionary with Worksheet names.</param>
    /// <param name="strSheetName">String that sets as a worksheet name.</param>
    /// <returns>Returns string with new name.</returns>
    public string GetNewAddress( Dictionary<string, string> names, out string strSheetName )
    {
      strSheetName = m_worksheet.Name;

      if( names == null )
        return Address;

      StringBuilder builder = new StringBuilder();
      int iCount = Count;

      if( iCount == 0 ) return string.Empty;

      IRange range = ( IRange )InnerList[ 0 ];

      builder.Append( ( ( ICombinedRange )range ).GetNewAddress( names, out strSheetName ) );
      string strAddressSeparator = GetAddressSeparator();

      for( int i = 1; i < iCount; i++ )
      {
        builder.Append( strAddressSeparator );

        range = ( IRange )InnerList[ i ];
        builder.Append( ( ( ICombinedRange )range ).GetNewAddress( names, out strSheetName ) );
      }      
     
      return builder.ToString();
    }
    /// <summary>
    /// Clones current IRange.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="hashNewNames">Dictionary with new names.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns>Returns clone of current instance.</returns>
    public IRange Clone( object parent, Dictionary<string, string> hashNewNames, WorkbookImpl book )
    {
      IWorksheet sheet = ( m_worksheet as IInternalWorksheet ).GetClonedObject( hashNewNames, book );
      RangesCollection result = new RangesCollection( Application, sheet );

      List<IRange> list = InnerList;

      for( int i = 0, iLen = list.Count; i < iLen; i++ )
      {
        object range = ( ( ICombinedRange )list[ i ] ).Clone( result
          , hashNewNames, book );
        result.Add( ( IRange )range );
      }

      return result;
    }
    /// <summary>
    /// Number of cells in the range. Read-only.
    /// </summary>
    public int CellsCount
    {
      get
      {
        int iResult = 0;

        for( int i = 0, len = Count; i < len; i++ )
        {
          ICombinedRange range = ( ICombinedRange )InnerList[ i ];
          iResult += range.CellsCount;
        }

        return iResult;
      }
    }
    /// <summary>
    /// Clears conditional formats.
    /// </summary>
    public void ClearConditionalFormats()
    {
      for( int i = 0, len = Count; i < len; i++ )
      {
        ICombinedRange range = ( ICombinedRange )this[ i ];
        range.ClearConditionalFormats();
      }
    }
    /// <summary>
    /// Returns array that contains information about range.
    /// </summary>
    /// <returns>Rectangles that describes range</returns>
    public Rectangle[] GetRectangles()
    {
      int iSize = 0;
      int iCount = Count;

      for( int i = 0; i < iCount; i++ )
      {
        ICombinedRange range = ( ICombinedRange )this[ i ];
        iSize += range.GetRectanglesCount();
      }

      Rectangle[] arrResult = new Rectangle[ iSize ];

      for( int i = 0, iIndex = 0; i < iCount; i++ )
      {
        ICombinedRange range = ( ICombinedRange )this[ i ];
        Rectangle[] arrRect = range.GetRectangles();
        arrRect.CopyTo( arrResult, iIndex );
        iIndex += arrRect.Length;
      }

      return arrResult;
    }
    /// <summary>
    /// Returns number of rectangles returned by GetRectangles method.
    /// </summary>
    /// <returns>Number of rectangles returned by GetRectangles method.</returns>
    public int GetRectanglesCount()
    {
      int iSize = 0;

      for( int i = 0, len = Count; i < len; i++ )
      {
        ICombinedRange range = ( ICombinedRange )this[ i ];
        iSize += range.GetRectanglesCount();
      }

      return iSize;
    }
    /// <summary>
    /// Gets name of the parent worksheet.
    /// </summary>
    public string WorksheetName
    {
      get
      {
        return Worksheet.Name;
      }
    }
    #endregion

    #region IRangesCollection methods
    /// <summary>
    /// Adds new range to the collection.
    /// </summary>
    /// <param name="range">Range to add.</param>
    public void Add( IRange range )
    {
      CheckDisposed();

      if( range == null )
        throw new ArgumentNullException( "range" );

      if( range.Worksheet != Worksheet )
        throw new ArgumentException( DEF_WRONG_WORKSHEET );

      m_iFirstRow = Math.Min( m_iFirstRow, range.Row );
      m_iFirstColumn = Math.Min( m_iFirstColumn, range.Column );
      m_iLastRow = Math.Max( m_iLastRow, range.LastRow );
      m_iLastColumn = Math.Max( m_iLastColumn, range.LastColumn );

      InnerList.Add( range );
    }
    /// <summary>
    /// Adds range to the collection.
    /// </summary>
    /// <param name="range">Range to add.</param>
    public void AddRange( IRange range )
    {
      CheckDisposed();

      if( range is RangesCollection )
      {
        RangesCollection col = ( RangesCollection )range;

        for( int i = 0, len = col.Count; i < len; i++ )
        {
          col.Add( col[ i ] );
        }
      }
      else
      {
        Add( range );
      }
    }
    /// <summary>
    /// Removes range from the collection.
    /// </summary>
    /// <param name="range">Range to remove.</param>
    public void Remove( IRange range )
    {
      CheckDisposed();

      List<IRange> arrList = InnerList;

      for( int i = 0, len = arrList.Count; i < len; i++ )
      {
        IRange item = ( IRange )arrList[ i ];

        if( range.Worksheet == item.Worksheet && range.AddressLocal == item.AddressLocal )
        {
          arrList.RemoveAt( i );
          i--;
          len--;
        }
      }

      InnerList.Remove( range );
      EvaluateDimensions();
    }
    /// <summary>
    /// Returns item by index from the collection.
    /// </summary>
    public IRange this[ int index ]
    {
      get
      {
        CheckDisposed();
        return ( IRange )InnerList[ index ];
      }
      set
      {
        CheckDisposed();

        if( value == null )
          throw new ArgumentNullException();

        InnerList[ index ] = value;
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Evaluates dimensions of the ranges collection.
    /// </summary>
    private void EvaluateDimensions()
    {
      CheckDisposed();
      m_iFirstRow = m_worksheet.Workbook.MaxRowCount + 1;
      m_iFirstColumn = m_worksheet.Workbook.MaxColumnCount + 1;
      m_iLastRow = 0;
      m_iLastColumn = 0;

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];

        m_iFirstRow = Math.Min( m_iFirstRow, range.Row );
        m_iFirstColumn = Math.Min( m_iFirstColumn, range.Column );
        m_iLastRow = Math.Max( m_iLastRow, range.LastRow );
        m_iLastColumn = Math.Max( m_iLastColumn, range.LastColumn );
      }
    }
    /// <summary>
    /// Returns SortedListEx that describes used rows / columns.
    /// </summary>
    /// <param name="bIsColumn">Indicates whether information about columns should be returned.</param>
    /// <returns>SortedListEx that describes used rows / columns.</returns>
    private SortedList<int, KeyValuePair<int, int>> GetColumnRowIndexes( bool bIsColumn )
    {
      CheckDisposed();

      SortedList<int, KeyValuePair<int, int>> result = new SortedList<int, KeyValuePair<int, int>>();

      for( int i = 0, len = Count; i < len; i++ )
      {
        IRange range = ( IRange )InnerList[ i ];

        if( range is RangesCollection )
        {
          SortedList<int, KeyValuePair<int, int>> list = ( ( RangesCollection )range ).GetColumnRowIndexes( bIsColumn );
          IList<int> listKeys = list.Keys;
          IList<KeyValuePair<int, int>> listValues = list.Values;

          for( int j = 0, iCount = list.Count; j < iCount; j++ )
          {
            //result.Add( list.GetKey( j ), list.GetByIndex( j ) );
            AddRowColumnIndex( result, listKeys[ j ], listValues[ j ] );
          }
        }
        else
        {
          int iStart = ( bIsColumn ) ? range.Column : range.Row;
          int iEnd = ( bIsColumn ) ? range.LastColumn : range.LastRow;

          int iSecondaryStart = ( bIsColumn ) ? range.Row : range.Column;
          int iSecondaryEnd = ( bIsColumn ) ? range.LastRow : range.LastColumn;

          for( int j = iStart; j <= iEnd; j++ )
          {
            AddRowColumnIndex( result, j, iSecondaryStart, iSecondaryEnd );
          }
        }
      }

      return result;
    }
    /// <summary>
    /// Adds new element to the list of used rows / columns.
    /// </summary>
    /// <param name="list">List to add entry to.</param>
    /// <param name="iIndex">Row / column index.</param>
    /// <param name="entry">Entry to add.</param>
    private void AddRowColumnIndex( SortedList<int, KeyValuePair<int, int>> list, int iIndex,
      KeyValuePair<int, int> entry )
    {
      CheckDisposed();

      if( list == null )
        throw new ArgumentNullException( "list" );

      if( list.ContainsKey( iIndex ) )
      {
        KeyValuePair<int, int> entryCurrent = list[ iIndex ];
        int iSecondaryStart = ( int )entry.Key;
        int iSecondaryEnd = ( int )entry.Value;

        int iMinIndex = Math.Min( entryCurrent.Key, iSecondaryStart );
        int iMaxIndex = Math.Max( entryCurrent.Value, iSecondaryEnd );

        entry = new KeyValuePair<int, int>( iMinIndex, iMaxIndex );
      }

      list[iIndex ] = entry;
    }
    /// <summary>
    /// Adds new element to the list of used rows / columns.
    /// </summary>
    /// <param name="list">List to add entry to.</param>
    /// <param name="iIndex">Row / column index.</param>
    /// <param name="iSecondaryStart">Start index of the new entry.</param>
    /// <param name="iSecondaryEnd">End index of the new entry.</param>
    private void AddRowColumnIndex( SortedList<int, KeyValuePair<int, int>> list, int iIndex,
      int iSecondaryStart, int iSecondaryEnd )
    {
      CheckDisposed();

      if( list == null )
        throw new ArgumentNullException( "list" );

      KeyValuePair<int, int> entry;

      if( !list.ContainsKey( iIndex ) )
      {
        entry = new KeyValuePair<int, int>( iSecondaryStart, iSecondaryEnd );
      }
      else
      {
        KeyValuePair<int, int> entryCurrent = list[ iIndex ];

        int iMinIndex = Math.Min( entryCurrent.Key, iSecondaryStart );
        int iMaxIndex = Math.Max( entryCurrent.Value, iSecondaryEnd );

        entry = new KeyValuePair<int, int>( iMinIndex, iMaxIndex );
      }

      list[ iIndex ] = entry;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bIsColumn"></param>
    /// <returns></returns>
    private IRange GetEntireColumnRow( bool bIsColumn )
    {
      CheckDisposed();

      RangesCollection result = AppImplementation.CreateRangesCollection( Worksheet );
      SortedList<int, KeyValuePair<int, int>> arrColumns = GetColumnRowIndexes( bIsColumn );

      if( arrColumns.Count == 0 ) return null;

      IRange rangeToAdd;
      IList<int> columnKeys = arrColumns.Keys;

      int iStartIndex = columnKeys[ 0 ];
      int iLastIndex = iStartIndex;

      //        int iFirstRow, iLastRow;
      //        m_worksheet.InnerGetColumnDimensions( iStartColumn, out iFirstRow, out iLastRow );
      int iFirstRow = ( bIsColumn )
        ? m_worksheet.UsedRange.Row
        : m_worksheet.UsedRange.Column;

      int iLastRow = ( bIsColumn )
        ? m_worksheet.UsedRange.LastRow
        : m_worksheet.UsedRange.LastColumn;

      for( int i = 1, len = arrColumns.Count; i < len; i++ )
      {
        int iCurrentIndex = columnKeys[ i ];

        if( iCurrentIndex - iLastIndex == 1 )
        {
          iLastIndex = iCurrentIndex;
        }
        else
        {
          rangeToAdd = ( bIsColumn )
            ? Worksheet.Range[ iFirstRow, iStartIndex, iLastRow, iLastIndex ]
            : Worksheet.Range[ iStartIndex, iFirstRow, iLastIndex, iLastRow ];

          result.Add( rangeToAdd );
          iStartIndex = iLastIndex = iCurrentIndex;
        }
      }

      rangeToAdd = ( bIsColumn )
        ? Worksheet.Range[ iFirstRow, iStartIndex, iLastRow, iLastIndex ]
        : Worksheet.Range[ iStartIndex, iFirstRow, iLastIndex, iLastRow ];

      result.Add( rangeToAdd );

      if( result.Count == 1 )
      {
        return result[ 0 ];
      }
      else
      {
        return result;
      }
    }
    /// <summary>
    /// Returns array of used rows / columns.
    /// </summary>
    /// <param name="bIsColumn">Indicates whether columns array should be returned.</param>
    /// <returns>Array of used rows / columns.</returns>
    private IRange[] GetColumnRows( bool bIsColumn )
    {
      CheckDisposed();

      SortedList<int, KeyValuePair<int, int>> arrRows = GetColumnRowIndexes( bIsColumn );
      IList<int> rowKeys = arrRows.Keys;
      IList<KeyValuePair<int, int>> rowValues = arrRows.Values;
      IRange[] arrResult = new IRange[ arrRows.Count ];

      for( int i = 0, len = arrRows.Count; i < len; i++ )
      {
        int iCurrentIndex = rowKeys[ i ];
        KeyValuePair<int, int> entry = rowValues[ i ];
        int iStartIndex = entry.Key;
        int iEndIndex = entry.Value;

        arrResult[ i ] = bIsColumn
          ? m_worksheet.Range[ iStartIndex, iCurrentIndex, iEndIndex, iCurrentIndex ]
          : m_worksheet.Range[ iCurrentIndex, iStartIndex, iCurrentIndex, iEndIndex ];
      }

      return arrResult;
    }
    /// <summary>
    /// Checks whether collection is disposed.
    /// </summary>
    private void CheckDisposed()
    {
//      if( m_bDisposed )
//        throw new ApplicationException( "Object is disposed" );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private string GetAddressSeparator()
    {
        bool isFormulaParsed = this.AppImplementation.IsFormulaParsed;
        this.AppImplementation.IsFormulaParsed = false;
        
        CultureInfo cultureInfo = this.AppImplementation.CheckAndApplySeperators();

        string strAddressSeparator = cultureInfo.TextInfo.ListSeparator;
        this.AppImplementation.IsFormulaParsed = isFormulaParsed;

        return strAddressSeparator;
    }
    #endregion

    #region INativePTG methods
    /// <summary>
    /// Gets ptg of current range.
    /// </summary>
    /// <returns>Returns native ptg.</returns>
    public Ptg[] GetNativePtg()
    {
      int iLen = List.Count;

      if( iLen == 0 )
        return null;

      if( List[ 0 ] is RangesCollection )
        throw new NotSupportedException( "Not supported : Range collection as element in range collection" );

      List<Ptg> result = new List<Ptg>();
      INativePTG ptg = ( INativePTG )List[ 0 ];
      Ptg binary = FormulaUtil.CreatePtg( FormulaToken.tCellRangeList , new object[ 1 ]{ "," } );

      result.Add( ptg.GetNativePtg()[ 0 ] );

      for( int i = 1; i < iLen; i++ )
      {
        if( List[ i ] is RangesCollection )
          throw new NotSupportedException( "Not supported : Range collection as element in range collection" );

        ptg = ( INativePTG )List[ i ];

        result.Add( ptg.GetNativePtg()[ 0 ] );
        result.Add( binary );
      }

      if( iLen > 1 )
      {
        Ptg unary = FormulaUtil.CreatePtg( FormulaToken.tParentheses , new object[ 1 ]{ "(" } );
        result.Add( unary );
      }

      return result.ToArray();
    }
    #endregion

    #region IEnumerable Members

    public new IEnumerator GetEnumerator()
    {
        return this.Cells.GetEnumerator();
    }

    #endregion
  }
}
