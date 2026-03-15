#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.PivotTables;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif
#if ( WINRT )
using System.Threading.Tasks;
using Windows.Storage;
using System.Text;
#endif
namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Summary description for ExternWorksheetImpl.
  /// </summary>
  public class ExternWorksheetImpl :
    CommonObject,
    IWorksheet,
    IInternalWorksheet,
    ICloneParent
  {
    #region Class members

#if PARSE_EXTERN_WORKSHEET
    /// <summary>
    /// Index of the worksheet.
    /// </summary>
    private int m_iSheetIndex;
    /// <summary>
    /// Dictionary that contains indexes of all used cells.
    /// Key - cell index, value - cell value (OPER structure).
    /// </summary>
    private SortedListEx m_listUsedCells = new SortedListEx();
#endif
    /// <summary>
    /// 
    /// </summary>
    private XCTRecord m_xct = ( XCTRecord )BiffRecordFactory.GetRecord( TBIFFRecord.XCT );
    /// <summary>
    /// List with sheet records.
    /// </summary>
    private List<BiffRecordRaw> m_arrRecords = new List<BiffRecordRaw>();
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private ExternWorkbookImpl m_book;
    /// <summary>
    /// Worksheet name.
    /// </summary>
    private string m_strName;
    /// <summary>
    /// Collection with worksheet's cached cell records.
    /// </summary>
    private CellRecordCollection m_dicRecordsCells;
    /// <summary>
    /// First used row.
    /// </summary>
    private int m_iFirstRow = -1;
    /// <summary>
    /// First used column.
    /// </summary>
    private int m_iFirstColumn = WorksheetImpl.DEF_MIN_COLUMN_INDEX;
    /// <summary>
    /// Last used row.
    /// </summary>
    private int m_iLastRow = -1;
    /// <summary>
    /// Last used column.
    /// </summary>
    private int m_iLastColumn = WorksheetImpl.DEF_MIN_COLUMN_INDEX;
    /// <summary>
    /// Additional attributes for sheetData tag.
    /// </summary>
    private Dictionary<string, string> m_dicAdditionalAttributes;
    internal int unknown_formula_name = 9;
    private Syncfusion.Calculate.CalcEngine m_calcEngine;
    public event Syncfusion.XlsIO.Implementation.RangeImpl.CellValueChangedEventHandler CellValueChanged;
    #endregion

    #region Events
    /// <summary>
    /// Event raised when an unknown function is encountered.
    /// </summary>
    public event MissingFunctionEventHandler MissingFunction;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public ExternWorksheetImpl( IApplication application, ExternWorkbookImpl parent )
      : base( application, parent )
    {
      m_book = parent;
      m_dicRecordsCells = new CellRecordCollection( Application, this );
      m_dicAdditionalAttributes = new Dictionary<string, string>();
      m_dicAdditionalAttributes[ "refreshError" ] = "1";
    }
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Parses extern worksheet.
    /// </summary>
    /// <param name="arrData">Array with worksheet records.</param>
    /// <param name="iOffset">Offset to the worksheet records.</param>
    /// <returns>Offset after worksheet reading all worksheet records.</returns>
    [ CLSCompliant( false ) ]
    public int Parse( BiffRecordRaw[] arrData, int iOffset )
    {
      if( arrData == null )
        throw new ArgumentNullException( "arrData" );

      if( iOffset < 0 || iOffset > arrData.Length - 1 )
        throw new ArgumentOutOfRangeException( "iOffset", "Value cannot be less than 0 and greater than arrData.Length - 1" );

      BiffRecordRaw record = arrData[ iOffset ];

      if( record.TypeCode != TBIFFRecord.XCT ) return iOffset;

      XCTRecord xct = ( XCTRecord )record;
      iOffset++;
#if PARSE_EXTERN_WORKSHEET
      m_iSheetIndex = xct.SheetTableIndex;

      for( int i = 0, len = xct.CRNCount; i < len; i++, iOffset++ )
      {
        arrData[ iOffset ].CheckTypeCode( TBIFFRecord.CRN );
        CRNRecord crn = ( CRNRecord )arrData[ iOffset ];
        ParseCRNRecord( crn );
      }
#else
      m_arrRecords.Clear();
      m_arrRecords.Add( xct );

      for( int i = 0, len = xct.CRNCount; i < len; i++, iOffset++ )
      {
        record = arrData[ iOffset ];
        record.CheckTypeCode( TBIFFRecord.CRN );
        ParseCRN( ( CRNRecord )record );
        m_arrRecords.Add( record );
      }
#endif

      return iOffset;
    }
    /// <summary>
    /// Extracts data from CRN record.
    /// </summary>
    /// <param name="crn">CRN record to parse.</param>
    private void ParseCRN( CRNRecord crn )
    {
      int iRowIndex = crn.Row + 1;
      int iCurrentColumn = crn.FirstColumn + 1;
      
      for( int i = 0; iCurrentColumn <= crn.LastColumn + 1; iCurrentColumn++, i++ )
      {
        object value = crn.Values[ i ];
        string strValue = value as string;

        if( strValue != null )
        {
          m_dicRecordsCells.SetNonSSTString( iRowIndex, iCurrentColumn, 0, strValue );
        }
        else if( value is double )
        {
          m_dicRecordsCells.SetNumberValue( iRowIndex, iCurrentColumn, ( double )value, 0 );
        }
        else if( value is bool )
        {
          m_dicRecordsCells.SetBooleanValue( iRowIndex, iCurrentColumn, ( bool )value, 0 );
        }
        else if( value is byte )
        {
          m_dicRecordsCells.SetErrorValue( iRowIndex, iCurrentColumn, ( byte )value, 0 );
        }
        else
        {
         // throw new NotSupportedException( "Wrong data type" );
        }
      }
    }
    /// <summary>
    /// Parses extern worksheet.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    [ CLSCompliant( false ) ]
    public void Parse( BiffReader reader, IDecryptor decryptor )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      TBIFFRecord recordCode = reader.PeekRecordType();

      if( recordCode != TBIFFRecord.XCT ) return;

      BiffRecordRaw record = reader.GetRecord( decryptor );
      m_xct = ( XCTRecord )record;
#if PARSE_EXTERN_WORKSHEET
      m_iSheetIndex = xct.SheetTableIndex;

      for( int i = 0, len = xct.CRNCount; i < len; i++, iOffset++ )
      {
        arrData[ iOffset ].CheckTypeCode( TBIFFRecord.CRN );
        CRNRecord crn = ( CRNRecord )arrData[ iOffset ];
        ParseCRNRecord( crn );
      }
#else
      m_arrRecords.Clear();
      //m_arrRecords.Add( xct );

      //for( int i = 0, len = m_xct.CRNCount; i < len; i++ )
      recordCode = reader.PeekRecordType();

      while( recordCode == TBIFFRecord.CRN )
      {
        record = reader.GetRecord( decryptor );
        record.CheckTypeCode( TBIFFRecord.CRN );
        ParseCRN( ( CRNRecord )record );
        m_arrRecords.Add( record );
        recordCode = reader.PeekRecordType();
      }
#endif
    }
#if PARSE_EXTERN_WORKSHEET
    /// <summary>
    /// Parses single CRNRecord.
    /// </summary>
    /// <param name="crn">Record to parse.</param>
    private void ParseCRNRecord( CRNRecord crn )
    {
      if( crn == null )
        throw new ArgumentNullException( "crn" );

      int iOperandOffset = 0;

      for( int i = 0, iColumn = crn.FirstColumn, last = crn.LastColumn - crn.FirstColumn;
        i <= last; i++, iColumn++ )
      {
        object value = crn.Values[ i ];
        long lCellIndex = RangeImpl.GetCellIndex( iColumn, crn.Row );
        m_listUsedCells.Add( lCellIndex, value );
      }
    }
#endif
    /// <summary>
    /// 
    /// </summary>
    /// <param name="records"></param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );
#if PARSE_EXTERN_WORKSHEET
      // TODO: finish parsing
      XCTRecord xct = ( XCTRecord )BiffRecordFactory.GetRecord( TBIFFRecord.XCT );
      xct.SheetTableIndex = ( ushort )m_iSheetIndex;
      records.Add( xct );

      for( int i = 0, len = m_listUsedCells.Count; i < len; i++ )
      {
        
      }
#else
      records.Add( m_xct );
      SerializeRows( records );
      //records.AddList( m_arrRecords );
#endif
    }
    /// <summary>
    /// Serializes row CRN records.
    /// </summary>
    /// <param name="records">Record list to put records into.</param>
    private void SerializeRows( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_iFirstRow >= 0 )
      {
        for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
        {
          SerializeRow( i, records );
        }
      }
    }
    /// <summary>
    /// Serializes single row into set of CRN records.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="records"></param>
    private void SerializeRow( int i, OffsetArrayList records )
    {
      RowStorage row = WorksheetHelper.GetOrCreateRow( this, i - 1, false );

      if( row != null )
      {
        IEnumerator enumerator = row.GetEnumerator( m_dicRecordsCells.RecordExtractor );
        CRNRecord crn = ( CRNRecord )BiffRecordFactory.GetRecord( TBIFFRecord.CRN );
        crn.Row = ( ushort )( i - 1 );
        int iPreviousColumnIndex = -1;
        List<object> values = crn.Values;

        while( enumerator.MoveNext() )
        {
          BiffRecordRaw record = ( BiffRecordRaw )enumerator.Current;
          IValueHolder valueHolder = ( IValueHolder )record;
          object value = valueHolder.Value;
          int iColumnIndex = ( record as ICellPositionFormat ).Column;

          // There was no data entered 
          if( iPreviousColumnIndex < 0 )
          {
            crn.FirstColumn = ( byte )iColumnIndex;
          }
          // We have to create new record and start its fill
          else if( iPreviousColumnIndex + 1 != iColumnIndex )
          {
            records.Add( crn );
            crn = ( CRNRecord )BiffRecordFactory.GetRecord( TBIFFRecord.CRN );
            crn.Row = ( ushort )( i - 1 );
            crn.FirstColumn = ( byte )iColumnIndex;
            values = crn.Values;
          }

          values.Add( value );
          crn.LastColumn = ( byte )iColumnIndex;
          iPreviousColumnIndex = iColumnIndex;
        }

        if( values.Count > 0 )
        {
          records.Add( crn );
        }
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Sheet index.
    /// </summary>
    public int Index
    {
      get
      {
        return m_xct.SheetTableIndex;
      }
      set
      {
        m_xct.SheetTableIndex = ( ushort )value;
      }
    }
    /// <summary>
    /// Gets parent extrnal workbook.
    /// </summary>
    public ExternWorkbookImpl Workbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Gets index of the reference to this worksheet.
    /// </summary>
    public int ReferenceIndex
    {
      get
      {
        WorkbookImpl book = m_book.Workbook;
        return book.AddSheetReference( m_book.Index, Index, Index );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, string> AdditionalAttributes
    {
      get
      {
        return m_dicAdditionalAttributes;
      }
      set
      {
        m_dicAdditionalAttributes = value;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Creates copy of the object.
    /// </summary>
    /// <param name="parent">Parent object for the new object.</param>
    /// <returns>Created object that is copy of the current object.</returns>
    public ExternWorksheetImpl Clone( object parent )
    {
      ExternWorksheetImpl result = ( ExternWorksheetImpl )MemberwiseClone();

      m_xct = ( XCTRecord )CloneUtils.CloneCloneable( m_xct );
      result.SetParent( parent );
      result.m_book = ( ExternWorkbookImpl )result.FindParent( typeof( ExternWorkbookImpl ) );
      result.m_dicRecordsCells = ( CellRecordCollection )m_dicRecordsCells.Clone( result );
      m_arrRecords = CloneUtils.CloneCloneable( m_arrRecords );

      return result;
    }
    /// <summary>
    /// This method is called during dispose operation.
    /// </summary>
    protected override void OnDispose()
    {
      if( !m_bIsDisposed )
      {
        if( m_dicRecordsCells != null )
        {
          m_dicRecordsCells.Dispose();
          m_dicRecordsCells = null;
        }

        base.OnDispose();
      }
    }
    /// <summary>
    /// Caches values from specified range.
    /// </summary>
    /// <param name="sourceRange">Range to cache data from.</param>
    internal void CacheValues( IRange sourceRange )
    {
      for( int iRow = sourceRange.Row, lastRow = sourceRange.LastRow; iRow <= lastRow; iRow++ )
      {
        for( int iCol = sourceRange.Column, lastColumn = sourceRange.LastColumn; iCol <= lastColumn; iCol++ )
        {
          IRange cell = sourceRange[ iRow, iCol ];

          if( cell.HasBoolean )
          {
            m_dicRecordsCells.SetBooleanValue( iRow, iCol, cell.Boolean, 0 );
          }
          else if( cell.HasDateTime || cell.HasNumber )
          {
            m_dicRecordsCells.SetNumberValue( iRow, iCol, cell.Number, 0 );
          }
          else if( cell.HasString )
          {
            m_dicRecordsCells.SetNonSSTString( iRow, iCol, 0, cell.Text );
          }
          else if( cell.IsError )
          {
            m_dicRecordsCells.SetErrorValue( iRow, iCol, cell.Error );
          }
        }
      }
    }
    #endregion


    #region ICalcData & Calculate methods
  
     
      /// <summary>
      /// Returns or sets the a CalcEngine object associated with this ICalcData implementation.
      /// </summary>
      public Syncfusion.Calculate.CalcEngine CalcEngine
      {
          get
          {
              return m_calcEngine;
          }
          set
          {
              m_calcEngine = value;
          }
      }
    /// <summary>
    /// Enables calculation support. If you want to be able to retrieve calculated values of formulas
    /// based on values you have changed in the workbook, then call this method once for any any worksheet
    /// in the workbook. Your can then used worksheet[row, column].CalculatedValue to access the proper
    /// calculated value of a cell.
    /// </summary>
    public void EnableSheetCalculations()
    {
        if (CalcEngine == null)
        {
            CalcEngine = new Syncfusion.Calculate.CalcEngine(this);
            CalcEngine.PreserveFormula = true;
            int sheetFamilyID = Syncfusion.Calculate.CalcEngine.CreateSheetFamilyID();

            string nameList = "!";

            //register the sheet names with calculate
            foreach (IWorksheet st in this.ParentWorkbook.Worksheets)
            {
                if (st.CalcEngine == null)
                {
                    st.CalcEngine = new Syncfusion.Calculate.CalcEngine(st);
                }
                CalcEngine.RegisterGridAsSheet(st.Name, st, sheetFamilyID);
                st.CalcEngine.UnknownFunction += new Syncfusion.Calculate.UnknownFunctionEventHandler(CalcEngine_UnknownFunction);
                nameList += st.Name + "!";
            }

            //get the named ranges into calculate
            Dictionary<string, string> ranges = new Dictionary<string, string>();
            foreach (IName name in this.ParentWorkbook.Names)
            {
                //ranges.Add(name.Scope + ":" + name.Name, name.Value.Replace("'", ""));
                if (name.Scope.Length > 0 && nameList.IndexOf("!" + name.Scope + "!") > -1)
                {
                    ranges.Add((name.Scope + "!" + name.Name).ToUpper(), name.Value.Replace("'", ""));
                }
                else
                {
                    ranges.Add(name.Name.ToUpper(), name.Value.Replace("'", ""));
                }
            }
#if !SILVERLIGHT && !WINRT && !WP
            Hashtable namedRanges1 = new Hashtable();            
#else
           Dictionary<object, object> namedRanges1 = new Dictionary<object, object>();
#endif
            if (ranges != null)
            {
                foreach (string s in ranges.Keys)
                {
                    namedRanges1.Add(s.ToUpper(System.Globalization.CultureInfo.InvariantCulture), ranges[s]);
                }
            }

            foreach (IWorksheet st in this.ParentWorkbook.Worksheets)
            {
                st.CalcEngine.NamedRanges = namedRanges1;
            }
        }
    }
    /// <summary>
    /// Disables calculation support in this workbook and disposes of the associative CalcEngine objects.
    /// </summary>
    public void DisableSheetCalculations()
    {
        if (CalcEngine != null && this.ParentWorkbook != null && this.ParentWorkbook.Worksheets != null)
        {
            foreach (IWorksheet st in this.ParentWorkbook.Worksheets)
            {
                if (st.CalcEngine != null)
                {
                    st.CalcEngine.UnknownFunction -= new Syncfusion.Calculate.UnknownFunctionEventHandler(CalcEngine_UnknownFunction);
                    st.CalcEngine.Dispose();
                }
                st.CalcEngine = null;
            }
        }
    }
    void CalcEngine_UnknownFunction(object sender, Syncfusion.Calculate.UnknownFunctionEventArgs args)
    {
        if (MissingFunction != null && CalcEngine != null)
        {
            MissingFunctionEventArgs e = new MissingFunctionEventArgs();
            e.MissingFunctionName = args.MissingFunctionName;
            e.CellLocation = args.CellLocation;
            MissingFunction(this, e);
        }
    }

    #region ICalcData Members

    /// <summary>
    /// Returns the formula string if the cell contains a formula, or the value if
    /// the cell cantains anything other than a formula.
    /// </summary>
    /// <param name="row">The row of the cell.</param>
    /// <param name="col">The column of the cell.</param>
    /// <returns>The formula string or value.</returns>
    public object GetValueRowCol(int row, int col)
    {
        IRange r = this[row, col];
        if (r.HasFormula)
            return r.Formula;
        else
            return r.Value;
    }

    /// <summary>
    /// Sets the value of a cell.
    /// </summary>
    /// <param name="value">The value to be set.</param>
    /// <param name="row">The row of the cell.</param>
    /// <param name="col">The column of the cell.</param>
    public void SetValueRowCol(object value, int row, int col)
    {
        if (value != null)
        {
        this.SetValue(row, col, value.ToString());
        }
    }    

    /// <summary>
    /// Not implemented.
    /// </summary>
    public void WireParentObject()
    {
        // throw new NotImplementedException();
    }

    /// <summary>
    /// An event raised on the IWorksheet whenever a value changes.
    /// </summary>
    public event Syncfusion.Calculate.ValueChangedEventHandler ValueChanged;

    /// <summary>
    /// Raises the <see cref="ValueChanged"/> event.
    /// </summary>
    /// <param name="row">The row of the change.</param>
    /// <param name="col">The column of the change.</param>
    /// <param name="value">The changed value.</param>
    public void OnValueChanged(int row, int col, string value)
    {
        if (ValueChanged != null)
        {
            Syncfusion.Calculate.ValueChangedEventArgs e = new Syncfusion.Calculate.ValueChangedEventArgs(row, col, value);
            ValueChanged(this, e);
        }
    }

    #endregion

    #endregion

    #region IWorksheet Members
    /// <summary>
    /// Returns collection of worksheet's autofilters. Read-only.
    /// </summary>
    public IAutoFilters AutoFilters
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
#if !SILVERLIGHT && !WINRT && !WP

    public void SaveAsHtml(string filename)
    {

    }
    public void SaveAsHtml(Stream stream)
    {

    }

    public void SaveAsHtml(string filename,HtmlSaveOptions saveOptions)
    {

    }
    void SaveAsHtml(Stream stream,HtmlSaveOptions saveOptions)
    {

    }
#endif
    /// <summary>
    /// Returns all used cells in the worksheet. Read-only.
    /// </summary>
    public IRange[] Cells
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// True if page breaks (both automatic and manual) on the specified
    /// worksheet are displayed. Read / write Boolean.
    /// </summary>
    public bool DisplayPageBreaks
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Gets the sparkline groups.
    /// </summary>
    /// <value>The sparkline groups.</value>
    public ISparklineGroups SparklineGroups
    {
        get
        {
            throw new NotSupportedException("This method or operation is not implemented");
        }
       
    }
    /// <summary>
    /// Gets protected options. Read-only. For sets protection options use "Protect" method.
    /// </summary>
    public ExcelSheetProtection Protection
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Indicates is current sheet is protected.
    /// </summary>
    public bool ProtectContents
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Gets or sets the view setting of the sheet.
    /// </summary>
    /// <value></value>
    public SheetView View
    {
        get
        {
            throw new Exception("The method or operation is not implemented.");
        }
        set
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
    /// <summary>
    /// True if objects are protected. Read-only.
    /// </summary>
    public bool ProtectDrawingObjects
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// True if the scenarios of the current sheet are protected. Read-only.
    /// </summary>
    public bool ProtectScenarios
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is OLE object.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is OLE object; otherwise, <c>false</c>.
    /// </value>
    public bool HasOleObject
    {
      set
      {
        throw new NotSupportedException();
      }
      get
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Returns all merged ranges. Read-only.
    /// </summary>
    public IRange[] MergedCells
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// For a Worksheet object, returns a Names collection that represents
    /// all the worksheet-specific names (names defined with the "WorksheetName!"
    /// prefix). Read-only Names object.
    /// </summary>
    public INames Names
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Name that is used by macros to access the workbook items. Read-only.
    /// </summary>
    public string CodeName
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns a PageSetup object that contains all the page setup settings
    /// for the specified object. Read-only.
    /// </summary>
    public IPageSetup PageSetup
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns a Range object that represents a cell or a range of cells.
    /// </summary>
    public IRange Range
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// For a Worksheet object, returns an array of Range objects that represents
    /// all the rows on the specified worksheet. Read-only Range object.
    /// </summary>
    public IRange[] Rows
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// For a Worksheet object, returns an array of Range objects that represents
    /// all the columns on the specified worksheet. Read-only Range object.
    /// </summary>
    public IRange[] Columns
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns or sets the standard (default) height of all the rows in the worksheet,
    /// in points. Read/write Double.
    /// </summary>
    public double StandardHeight
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns or sets the standard (default) height option flag, which defines that
    /// standard (default) row height and book default font height do not match.
    /// Read/write Bool.
    /// </summary>
    public bool StandardHeightFlag
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns or sets the standard (default) width of all the columns in the
    /// worksheet. Read/write Double.
    /// </summary>
    public double StandardWidth
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns or sets the worksheet type. Read-only ExcelSheetType.
    /// </summary>
    public ExcelSheetType Type
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns a Range object that represents the used range on the
    /// specified worksheet. Read-only.
    /// </summary>
    public IRange UsedRange
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Zoom factor of document. Value must be in range from 10 till 400.
    /// </summary>
    public int Zoom
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Position of the vertical split (px, 0 = No vertical split):
    /// Unfrozen pane: Width of the left pane(s) (in twips = 1/20 of a point)
    /// Frozen pane: Number of visible columns in left pane(s)
    /// </summary>
    public int VerticalSplit
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Position of the horizontal split (by, 0 = No horizontal split):
    /// Unfrozen pane: Height of the top pane(s) (in twips = 1/20 of a point)
    /// Frozen pane: Number of visible rows in top pane(s)
    /// </summary>
    public int HorizontalSplit
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Index to first visible row in bottom pane(s).
    /// </summary>
    public int FirstVisibleRow
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
# if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Gets the OLE objects.
    /// </summary>
    /// <value>The OLE objects.</value>
    public IOleObjects OleObjects
    {
      get
      {
        throw new NotImplementedException("The method or operation is not implemented.");
      }
    }
#endif
    /// <summary>
    /// Index to first visible column in right pane(s).
    /// </summary>
    public int FirstVisibleColumn
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Identifier of pane with active cell cursor.
    /// </summary>
    public int ActivePane
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// True if zero values to be displayed
    /// False otherwise.
    /// </summary>
    public bool IsDisplayZeros
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// True if gridlines are visible;
    /// False otherwise.
    /// </summary>
    public bool IsGridLinesVisible
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Gets / sets Grid line color.
    /// </summary>
    public ExcelKnownColors GridLineColor
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// True if row and column headers are visible;
    /// False otherwise.
    /// </summary>
    public bool IsRowColumnHeadersVisible
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns a VPageBreaks collection that represents the vertical page
    /// breaks on the sheet. Read-only.
    /// </summary>
    public IVPageBreaks VPageBreaks
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns an HPageBreaks collection that represents the horizontal
    /// page breaks on the sheet. Read-only.
    /// </summary>
    public IHPageBreaks HPageBreaks
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Indicates if all values in the workbook are preserved as strings.
    /// </summary>
    public bool IsStringsPreserved
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Indicates if the worksheet is password protected.
    /// </summary>
    public bool IsPasswordProtected
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Comments collection.
    /// </summary>
    public IComments Comments
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Gets / sets cell by row and index.
    /// </summary>
    public IRange this[ int row, int column ]
    {
      get
      {        
          return null;
      }
    }
    /// <summary>
    /// Get cells range.
    /// </summary>
    public IRange this[ int row, int column, int lastRow, int lastColumn ]
    {
      get
      {
          return null;
      }
    }
    /// <summary>
    /// Get cell range.
    /// </summary>
    public IRange this[ string name ]
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Get cell range.
    /// </summary>
    public IRange this[ string name, bool IsR1C1Notation ]
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Collection of all worksheet's hyperlinks.
    /// </summary>
    public IHyperLinks HyperLinks
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns all not empty or accessed cells. Read-only.
    /// WARNING: This property creates Range object for each cell in the worksheet
    /// and creates new array each time user calls to it. It can cause huge memory
    /// usage especially if called frequently.
    /// </summary>
    public IRange[] UsedCells
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns collection of custom properties. Read-only.
    /// </summary>
    public IWorksheetCustomProperties CustomProperties
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Indicates whether all created range objects should be cached. Default value is false.
    /// </summary>
    public bool UseRangesCache
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Defines whether freezed panes are applied.
    /// </summary>
    public bool IsFreezePanes
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Return split cell range.
    /// </summary>
    public IRange SplitCell
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Gets/sets top visible row of the worksheet.
    /// </summary>
    public int TopVisibleRow
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Gets/sets left visible column of the worksheet.
    /// </summary>
    public int LeftVisibleColumn
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// There are two different algorithms to create UsedRange object:
    /// 1) Default. This property = true. The cell is included into UsedRange when
    /// it has some record created for it even if data is empty (maybe some formatting
    /// changed, maybe not - cell was accessed and record was created).
    /// 2) This property = false. In this case XlsIO tries to remove empty rows and
    /// columns from all sides to make UsedRange smaller.
    /// </summary>
    public bool UsedRangeIncludesFormatting
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns pivot table collection containing all pivot tables in the worksheet. Read-only.
    /// </summary>
    public IPivotTables PivotTables
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Gets collection of all list objects in the worksheet.
    /// </summary>
    public IListObjects ListObjects
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Copies worksheet into the clipboard.
    /// </summary>
    public void CopyToClipboard()
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Clears worksheet data. Removes all formatting and merges.
    /// </summary>
    public void Clear()
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Clears worksheet. Only the data is removed from each cell.
    /// </summary>
    public void ClearData()
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Indicates whether a cell was initialized or accessed by the user.
    /// </summary>
    /// <param name="iRow">One-based row index of the cell.</param>
    /// <param name="iColumn">One-based column index of the cell.</param>
    /// <returns>Value indicating whether the cell was initialized or accessed by the user.</returns>
    public bool Contains( int iRow, int iColumn )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Creates new instance of IRanges.
    /// </summary>
    /// <returns>New instance of ranges collection.</returns>
    public IRanges CreateRangesCollection()
    {
      return AppImplementation.CreateRangesCollection( this );
    }

      public void CreateNamedRanges(string namedRange, string referRange, bool vertical)
      {
          throw new NotImplementedException();
      }
    /// <summary>
    /// Creates object that can be used for template markers processing.
    /// </summary>
    /// <returns>Object that can be used for template markers processing.</returns>
    public ITemplateMarkersProcessor CreateTemplateMarkersProcessor()
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Method check is Column with specified index visible to end user or not.
    /// </summary>
    /// <param name="columnIndex">Index of column.</param>
    /// <returns>True - column is visible; otherwise False.</returns>
    public bool IsColumnVisible( int columnIndex )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Shows / Hides the specified column.
    /// </summary>
    /// <param name="columnIndex">Index at which the column should be hidden.</param>
    /// <param name="isVisible">True - Column is visible; false - hidden.</param>
    public void ShowColumn( int columnIndex, bool isVisible )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Hides the specified column.
    /// </summary>
    /// <param name="columnIndex">One-based column index to hide.</param>
    public void HideColumn(int columnIndex)
    {
        ShowColumn(columnIndex, false);
    }
    /// <summary>
    /// Hides the specified row.
    /// </summary>
    /// <param name="rowIndex">One-based row index to hide.</param>
    public void HideRow(int rowIndex)
    {
        ShowRow(rowIndex, false);
    }
    /// <summary>
    /// Method check is Row with specified index visible to user or not.
    /// </summary>
    /// <param name="rowIndex">Index of row visibility of each must be checked.</param>
    /// <returns>True - row is visible to user, otherwise False.</returns>
    public bool IsRowVisible( int rowIndex )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Shows / Hides the specified row.
    /// </summary>
    /// <param name="rowIndex">Index at which the row should be hidden.</param>
    /// <param name="isVisible">True - Row is visible; false - hidden.</param>
    public void ShowRow( int rowIndex, bool isVisible )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
	/// <summary>
    /// Shows / Hides the specified range.
    /// </summary>
    /// <param name="range">Range specifies the particular range to show / hide</param>
    /// <param name="isVisible">True - Range is visible; False - hidden.</param>
    public void ShowRange(IRange range, bool isVisible)
    {
        throw new Exception("The method or operation is not implemented.");
    }
    /// <summary>
    /// Shows/ Hides the collection of range.
    /// </summary>
    /// <param name="ranges">Ranges specifies the range collection.</param>
    /// <param name="isVisible">True - Row is visible; false - hidden.</param>
    public void ShowRange(RangesCollection ranges, bool isVisible)
    {
        throw new Exception("The method or operation is not implemented.");
    }
    /// <summary>
    /// Shows/ Hides an array of range.
    /// </summary>
    /// <param name="ranges">Ranges specifies the range array.</param>
    /// <param name="isVisible">True - Row is visible; false - hidden.</param>
    public void ShowRange(IRange[] ranges, bool isVisible)
    {
        throw new Exception("The method or operation is not implemented.");
    }
    /// <summary>
    /// Inserts an empty row with default formatting (with formulas update).
    /// </summary>
    /// <param name="index">Index at which new row should be inserted.</param>
    public void InsertRow( int index )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Inserts an empty row with default formatting.
    /// </summary>
    /// <param name="iRowIndex">Index at which new row should be inserted.</param>
    /// <param name="iRowCount">Number of rows to insert.</param>
    public void InsertRow( int iRowIndex, int iRowCount )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Inserts an empty row with default formatting.
    /// </summary>
    /// <param name="iRowIndex">Index at which new row should be inserted.</param>
    /// <param name="iRowCount">Number of rows to insert.</param>
    /// <param name="insertOptions">Insert options.</param>
    public void InsertRow( int iRowIndex, int iRowCount, ExcelInsertOptions insertOptions )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Inserts an empty column with default formatting.
    /// </summary>
    /// <param name="index">Index at which new column should be inserted.</param>
    public void InsertColumn( int index )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Inserts an empty column with default formatting (with formulas update).
    /// </summary>
    /// <param name="iColumnIndex">Index at which new column should be inserted.</param>
    /// <param name="iColumnCount">Number of columns to insert.</param>
    public void InsertColumn( int iColumnIndex, int iColumnCount )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Inserts an empty column with default formatting.
    /// </summary>
    /// <param name="iColumnIndex">Index at which new column should be inserted.</param>
    /// <param name="iColumnCount">Number of columns to insert.</param>
    /// <param name="insertOptions">Insert options.</param>
    public void InsertColumn( int iColumnIndex, int iColumnCount, ExcelInsertOptions insertOptions )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Removes specified row (with formulas update).
    /// </summary>
    /// <param name="index">One-based row index to remove.</param>
    public void DeleteRow( int index )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Removes specified row (with formulas update).
    /// </summary>
    /// <param name="index">One-based row index to remove.</param>
    /// <param name="count">Number of rows to remove.</param>
    public void DeleteRow( int index, int count )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Removes specified column (with formulas update).
    /// </summary>
    /// <param name="index">One-based column index to remove.</param>
    public void DeleteColumn( int index )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Removes specified column (without updating formulas).
    /// </summary>
    /// <param name="index">One-based column index to remove.</param>
    /// <param name="count">Number of columns to remove.</param>
    public void DeleteColumn( int index, int count )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports an array of objects into a worksheet.
    /// </summary>
    /// <param name="arrObject">Array to import.</param>
    /// <param name="firstRow">Row of the first cell where array should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where array should be imported.</param>
    /// <param name="isVertical">True if array should be imported vertically; False - horizontally.</param>
    /// <returns>Number of imported elements.</returns>
    public int ImportArray( object[] arrObject, int firstRow, int firstColumn, bool isVertical )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports an array of strings into a worksheet.
    /// </summary>
    /// <param name="arrString">Array to import.</param>
    /// <param name="firstRow">Row of the first cell where array should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where array should be imported.</param>
    /// <param name="isVertical">True if array should be imported vertically; False - horizontally.</param>
    /// <returns>Number of imported elements.</returns>
    public int ImportArray( string[] arrString, int firstRow, int firstColumn, bool isVertical )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports an array of integers into a worksheet.
    /// </summary>
    /// <param name="arrInt">Array to import.</param>
    /// <param name="firstRow">Row of the first cell where array should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where array should be imported.</param>
    /// <param name="isVertical">True if array should be imported vertically; False - horizontally.</param>
    /// <returns>Number of imported elements.</returns>
    public int ImportArray( int[] arrInt, int firstRow, int firstColumn, bool isVertical )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports an array of doubles into a worksheet.
    /// </summary>
    /// <param name="arrDouble">Array to import.</param>
    /// <param name="firstRow">Row of the first cell where array should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where array should be imported.</param>
    /// <param name="isVertical">True if array should be imported vertically; False - horizontally.</param>
    /// <returns>Number of imported elements.</returns>
    public int ImportArray( double[] arrDouble, int firstRow, int firstColumn, bool isVertical )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports an array of DateTimes into worksheet.
    /// </summary>
    /// <param name="arrDateTime">Array to import.</param>
    /// <param name="firstRow">Row of the first cell where array should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where array should be imported.</param>
    /// <param name="isVertical">True if array should be imported vertically; False - horizontally.</param>
    /// <returns>Number of imported elements.</returns>
    public int ImportArray( DateTime[] arrDateTime, int firstRow, int firstColumn, bool isVertical )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports an array of objects into a worksheet.
    /// </summary>
    /// <param name="arrObject">Array to import.</param>
    /// <param name="firstRow">Row of the first cell where array should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where array should be imported.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportArray( object[ , ] arrObject, int firstRow, int firstColumn )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports data from class objects into worksheet
    /// </summary>
    /// <param name="arrObject">IEnumerable object with desired data</param>
    /// <param name="firstRow">Row of the First cell to be imported</param>
    /// <param name="firstColumn">Column of the first cell to be imported</param>
    /// <param name="includeHeader">TRUE if class properties names must also be imported</param>
    /// <returns></returns>
    public int ImportData(IEnumerable arrObject, int firstRow, int firstColumn, bool includeHeader)
    {
        throw new Exception("The method or operation is not implemented.");
    }
# if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Imports data from a DataColumn into worksheet.
    /// </summary>
    /// <param name="dataColumn">DataColumn with desired data.</param>
    /// <param name="isFieldNameShown">True if column name must also be imported.</param>
    /// <param name="firstRow">Row of the first cell where DataTable should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where DataTable should be imported.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataColumn( System.Data.DataColumn dataColumn, bool isFieldNameShown, int firstRow, int firstColumn )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports data from a DataTable into worksheet.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="isFieldNameShown">True if column names must also be imported.</param>
    /// <param name="firstRow">Row of the first cell where DataTable should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where DataTable should be imported.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataTable( System.Data.DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports data from a DataTable into worksheet.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="isFieldNameShown">True if column names must also be imported.</param>
    /// <param name="firstRow">Row of the first cell where DataTable should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where DataTable should be imported.</param>
    /// <param name="preserveTypes">
    /// Indicates whether XlsIO should try to preserve types in DataTable,
    /// i.e. if it is set to False (default) and in DataTable we have in string column
    /// value that contains only numbers, it would be converted to number.
    /// </param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataTable( System.Data.DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, bool preserveTypes )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports data from a DataTable into worksheet.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="isFieldNameShown">True if column names must also be imported.</param>
    /// <param name="firstRow">Row of the first cell where DataTable should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where DataTable should be imported.</param>
    /// <param name="maxRows">Maximum number of rows to import.</param>
    /// <param name="maxColumns">Maximum number of columns to import.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataTable( System.Data.DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, int maxRows, int maxColumns )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports data from a DataTable into worksheet.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="isFieldNameShown">True if column names must also be imported.</param>
    /// <param name="firstRow">Row of the first cell where DataTable should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where DataTable should be imported.</param>
    /// <param name="maxRows">Maximum number of rows to import.</param>
    /// <param name="maxColumns">Maximum number of columns to import.</param>
    /// <param name="preserveTypes">
    /// Indicates whether XlsIO should try to preserve types in DataTable,
    /// i.e. if it is set to False (default) and in DataTable we have in string column
    /// value that contains only numbers, it would be converted to number.
    /// </param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataTable( System.Data.DataTable dataTable, bool isFieldNameShown, int firstRow, int firstColumn, int maxRows, int maxColumns, bool preserveTypes )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports data from a DataTable into namedRange.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="namedRange">Represents named range.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataTable( System.Data.DataTable dataTable, IName namedRange, bool isFieldNameShown )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports data from a DataTable into namedRange.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="namedRange">Represents named range.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="rowOffset">Represents row offset into named range to import.</param>
    /// <param name="columnOffset">Represents column offset into named range to import.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataTable( System.Data.DataTable dataTable, IName namedRange, bool isFieldNameShown, int rowOffset, int columnOffset )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports data from a DataTable into namedRange.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="namedRange">Represents named range.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="rowOffset">Represents row offset into named range to import.</param>
    /// <param name="columnOffset">Represents column offset into named range to import.</param>
    /// <param name="iMaxRow">Represents count of rows to import.</param>
    /// <param name="iMaxCol">Represents count of rows to import.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataTable( System.Data.DataTable dataTable, IName namedRange, bool isFieldNameShown, int rowOffset, int columnOffset, int iMaxRow, int iMaxCol )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports data from a DataTable into namedRange.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="namedRange">Represents named range.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="rowOffset">Represents row offset into named range to import.</param>
    /// <param name="columnOffset">Represents column offset into named range to import.</param>
    /// <param name="iMaxRow">Represents count of rows to import.</param>
    /// <param name="iMaxCol">Represents count of rows to import.</param>
    /// <param name="bPreserveTypes">Indicates whether to preserve column types.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataTable( System.Data.DataTable dataTable, IName namedRange, bool isFieldNameShown, int rowOffset, int columnOffset, int iMaxRow, int iMaxCol, bool bPreserveTypes )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports data from a DataView into worksheet.
    /// </summary>
    /// <param name="dataView">DataView with desired data.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataView should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataView should be imported.
    /// </param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataView( System.Data.DataView dataView, bool isFieldNameShown, int firstRow, int firstColumn )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports data from a DataView into worksheet.
    /// </summary>
    /// <param name="dataView">DataView with desired data.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataView should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataView should be imported.
    /// </param>
    /// <param name="bPreserveTypes">Indicates whether to preserve column types.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataView( System.Data.DataView dataView, bool isFieldNameShown, int firstRow, int firstColumn, bool bPreserveTypes )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports data from a DataView into worksheet.
    /// </summary>
    /// <param name="dataView">DataView with desired data.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataView should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataView should be imported.
    /// </param>
    /// <param name="maxRows">Maximum number of rows to import.</param>
    /// <param name="maxColumns">Maximum number of columns to import.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataView( System.Data.DataView dataView, bool isFieldNameShown, int firstRow, int firstColumn, int maxRows, int maxColumns )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Imports data from a DataView into worksheet.
    /// </summary>
    /// <param name="dataView">DataView with desired data.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataView should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataView should be imported.
    /// </param>
    /// <param name="maxRows">Maximum number of rows to import.</param>
    /// <param name="maxColumns">Maximum number of columns to import.</param>
    /// <param name="bPreserveTypes">Indicates whether to preserve column types.</param>
    /// <returns>Number of imported rows</returns>
    public int ImportDataView( System.Data.DataView dataView, bool isFieldNameShown, int firstRow, int firstColumn, int maxRows, int maxColumns, bool bPreserveTypes )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
#endif
    /// <summary>
    /// Removes panes from a worksheet.
    /// </summary>
    public void RemovePanes()
    {
      throw new Exception( "The method or operation is not implemented." );
    }
# if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Exports worksheet data into a DataTable.
    /// </summary>
    /// <param name="firstRow">Row of the first cell from where DataTable should be exported.</param>
    /// <param name="firstColumn">Column of the first cell from where DataTable should be exported.</param>
    /// <param name="maxRows">Maximum number of rows to export.</param>
    /// <param name="maxColumns">Maximum number of columns to export.</param>
    /// <param name="options">Export options.</param>
    /// <returns>DataTable with worksheet data.</returns>
    public System.Data.DataTable ExportDataTable( int firstRow, int firstColumn, int maxRows, int maxColumns, ExcelExportDataTableOptions options )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Exports worksheet data into a DataTable.
    /// </summary>
    /// <param name="dataRange">Range to export.</param>
    /// <param name="options">Export options.</param>
    /// <returns>DataTable with worksheet data.</returns>
    public System.Data.DataTable ExportDataTable( IRange dataRange, ExcelExportDataTableOptions options )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Exports worksheet data into a DataTable only for pivot engine.
    /// </summary>
    /// <param name="dataRange">Range to export.</param>
    /// <param name="options">Export options.</param>
    /// <returns>DataTable with worksheet data.</returns>
    public System.Data.DataTable PEExportDataTable(IRange dataRange, ExcelExportDataTableOptions options,PivotTableImpl pivotTable)
    {
        throw new Exception("The method or operation is not implemented.");
    }
#endif
    /// <summary>
    /// Protects worksheet's content with password.
    /// </summary>
    /// <param name="password">Password to protect with.</param>
    public void Protect( string password )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Protects current worksheet.
    /// </summary>
    /// <param name="password">Represents password to protect.</param>
    /// <param name="options">Represents params to protect.</param>
    public void Protect( string password, ExcelSheetProtection options )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Unprotects worksheet's content with password.
    /// </summary>
    /// <param name="password">Password to unprotect.</param>
    public void Unprotect( string password )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Intersects two ranges.
    /// </summary>
    /// <param name="range1">First range to intersect.</param>
    /// <param name="range2">Second range to intersect.</param>
    /// <returns>Intersection of two ranges or NULL if there is no range intersection.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When range1 or range2 is NULL.
    /// </exception>
    public IRange IntersectRanges( IRange range1, IRange range2 )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Merges two ranges.
    /// </summary>
    /// <param name="range1">First range to merge.</param>
    /// <param name="range2">Second range to merge.</param>
    /// <returns>Merged ranges or NULL if wasn't able to merge ranges.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When range1 or range2 is NULL.
    /// </exception>
    public IRange MergeRanges( IRange range1, IRange range2 )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Autofits specified row.
    /// </summary>
    /// <param name="rowIndex">One-based row index.</param>
    public void AutofitRow( int rowIndex )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Autofits specified column.
    /// </summary>
    /// <param name="colIndex">One-based column index.</param>
    public void AutofitColumn( int colIndex )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    public void Replace( string oldValue, string newValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    public void Replace( string oldValue, double newValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    public void Replace( string oldValue, DateTime newValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Replaces specified string by data from array.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Array of new values.</param>
    /// <param name="isVertical">
    /// Indicates whether array should be inserted vertically.
    /// </param>
    public void Replace( string oldValue, string[] newValues, bool isVertical )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Replaces specified string by data from array.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Array of new values.</param>
    /// <param name="isVertical">
    /// Indicates whether array should be inserted vertically.
    /// </param>
    public void Replace( string oldValue, int[] newValues, bool isVertical )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Replaces specified string by data from array.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Array of new values.</param>
    /// <param name="isVertical">
    /// Indicates whether array should be inserted vertically.
    /// </param>
    public void Replace( string oldValue, double[] newValues, bool isVertical )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
# if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Replaces specified string by data table values.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Data table with new data.</param>
    /// <param name="isFieldNamesShown">Indicates whether field name must be shown.</param>
    public void Replace( string oldValue, System.Data.DataTable newValues, bool isFieldNamesShown )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Replaces specified string by data column values.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Data table with new data.</param>
    /// <param name="isFieldNamesShown">Indicates whether field name must be shown.</param>
    public void Replace( string oldValue, System.Data.DataColumn newValues, bool isFieldNamesShown )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
#endif
    /// <summary>
    /// Removes worksheet from parent worksheets collection.
    /// </summary>
    public void Remove()
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Moves worksheet.
    /// </summary>
    /// <param name="iNewIndex">New index of the worksheet.</param>
    public void Move( int iNewIndex )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Converts column width into pixels.
    /// </summary>
    /// <param name="widthInChars">Width in characters.</param>
    /// <returns>Width in pixels</returns>
    public int ColumnWidthToPixels( double widthInChars )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Converts pixels into column width (in characters).
    /// </summary>
    /// <param name="pixels">Width in pixels</param>
    /// <returns>Width in characters.</returns>
    public double PixelsToColumnWidth( int pixels )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets column width.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index.</param>
    /// <param name="value">Width to set.</param>
    public void SetColumnWidth( int iColumnIndex, double value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets column width.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index.</param>
    /// <param name="value">Width in pixels to set.</param>
    public void SetColumnWidthInPixels( int iColumnIndex, int value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
      /// <summary>
      /// Set Column Width from Start Column index and End Column index
      /// </summary>
      /// <param name="iStartColumnIndex">Start Column index</param>
    /// <param name="iCount"No of Column to be set width</param>
      /// <param name="value">Value in pixels</param>
    public void SetColumnWidthInPixels(int iStartColumnIndex, int iCount, int value)
    {
        throw new Exception("The method or operation is not implemented.");
    }
    /// <summary>
    /// Sets row height.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="value">Height to set.</param>
    public void SetRowHeight( int iRow, double value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets row height in pixels.
    /// </summary>
    /// <param name="iRowIndex">One-based row index to set height.</param>
    /// <param name="value">Value in pixels to set.</param>
    public void SetRowHeightInPixels( int iRowIndex, double value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
      /// <summary>
      /// Set Row height from Start Row index to End Row index
      /// </summary>
      /// <param name="iStartRowIndex">Start Row index</param>
    /// <param name="iCount">No of Row to be set width</param>
      /// <param name="value">Value in pixels</param>
    public void SetRowHeightInPixels(int iStartRowIndex, int iCount, double value)
    {
        throw new Exception("The method or operation is not implemented.");
    }
    /// <summary>
    /// Returns width from ColumnInfoRecord if there is corresponding ColumnInfoRecord
    /// or StandardWidth if not.
    /// </summary>
    /// <param name="iColumnIndex">One-based index of the column.</param>
    /// <returns>Width of the specified column.</returns>
    public double GetColumnWidth( int iColumnIndex )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Returns width in pixels from ColumnInfoRecord if there is corresponding ColumnInfoRecord
    /// or StandardWidth if not.
    /// </summary>
    /// <param name="iColumnIndex">One-based index of the column.</param>
    /// <returns>Width in pixels of the specified column.</returns>
    public int GetColumnWidthInPixels( int iColumnIndex )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Returns height from RowRecord if there is a corresponding RowRecord.
    /// Otherwise returns StandardHeight. 
    /// </summary>
    /// <param name="iRow">One-based index of the row</param>
    /// <returns>
    /// Height from RowRecord if there is corresponding RowRecord.
    /// Otherwise returns StandardHeight.
    /// </returns>
    public double GetRowHeight( int iRow )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Returns height from RowRecord if there is a corresponding RowRecord.
    /// Otherwise returns StandardHeight. 
    /// </summary>
    /// <param name="iRowIndex">One-based index of the row.</param>
    /// <returns>
    /// Height in pixels from RowRecord if there is corresponding RowRecord.
    /// Otherwise returns StandardHeight.
    /// </returns>
    public int GetRowHeightInPixels( int iRowIndex )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( string findValue, ExcelFindType flags )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst(string findValue, ExcelFindType flags,ExcelFindOptions findOptions)
    {
        throw new Exception("The method or operation is not implemented.");
    }
    /// <summary>
    /// This method searches for the first cell with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( double findValue, ExcelFindType flags )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// This method searches for the first cell with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( bool findValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// This method searches for the first cell with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( DateTime findValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// This method searches for the first cell with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( TimeSpan findValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// This method searches for the first cell that starts with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>
    /// First found cell, or Null if value was not found.
    /// </returns>
    public IRange FindStringStartsWith(string findValue, ExcelFindType flags)
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// This method searches for the first cell that starts with specified string value which igonres the case.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="ignoreCase">true to ignore case wen comparing this string to the value;otherwise,false</param>
    /// <returns>
    /// First found cell, or Null if value was not found.
    /// </returns>
    public  IRange FindStringStartsWith(string findValue, ExcelFindType flags, bool ignoreCase)
    {
        throw new Exception("The method or operation is not implemented.");
    }
    /// <summary>
    /// This method searches for the first cell that ends  with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>
    /// First found cell, or Null if value was not found.
    /// </returns>
    public  IRange FindStringEndsWith(string findValue, ExcelFindType flags)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    /// <summary>
    /// This method searches for the first cell that ends with specified string value which igonres the case.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="ignoreCase">true to ignore case wen comparing this string to the value;otherwise,false</param>
    /// <returns>
    /// First found cell, or Null if value was not found.
    /// </returns>
    public  IRange FindStringEndsWith(string findValue, ExcelFindType flags, bool ignoreCase)
    {
        throw new Exception("The method or operation is not implemented.");
    }
    /// <summary>
    /// This method searches for the all cells with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( string findValue, ExcelFindType flags )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// This method searches for the all cells with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="findOptions">The find options.</param>
    /// <returns>
    /// All found cells, or Null if value was not found.
    /// </returns>
    public IRange[] FindAll(string findValue, ExcelFindType flags,ExcelFindOptions findOptions)
    {
        throw new Exception("The method or operation is not implemented.");
    }
    ///<summary>
    /// This method searches for the all cells with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( double findValue, ExcelFindType flags )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// This method searches for the all cells with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found</returns>
    public IRange[] FindAll( bool findValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// This method searches for the all cells with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( DateTime findValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// This method searches for the all cells with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( TimeSpan findValue )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="fileName">File to save.</param>
    /// <param name="separator">Current separator.</param>
    public void SaveAs( string fileName, string separator )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="fileName">File to save.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    public void SaveAs( string fileName, string separator, System.Text.Encoding encoding )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
#if ( WINRT )
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="storageFile">StorageFile to save. </param>
    /// <param name="separator">Current separator.</param>
   public Task<bool> SaveAsAsync(StorageFile storageFile, string separator)
    {
        throw new Exception("The method or operation is not implemented.");
    }
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="storageFile">StorageFile to save. </param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    public Task<bool> SaveAsAsync(StorageFile storageFile, string separator, Encoding encoding)
    {
        throw new Exception("The method or operation is not implemented.");
    }
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save. </param>
    /// <param name="separator">Current separator.</param>
    public Task<bool> SaveAsAsync(System.IO.Stream stream, string separator)
    {
        throw new Exception("The method or operation is not implemented.");
    }
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save. </param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    public Task<bool> SaveAsAsync(System.IO.Stream stream, string separator, System.Text.Encoding encoding)
    {
        throw new Exception("The method or operation is not implemented.");
    }
#endif
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save. </param>
    /// <param name="separator">Current separator.</param>
    public void SaveAs( System.IO.Stream stream, string separator )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save. </param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    public void SaveAs( System.IO.Stream stream, string separator, System.Text.Encoding encoding )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets by column index default style for column.
    /// </summary>
    /// <param name="iColumnIndex">Column index.</param>
    /// <param name="defaultStyle">Default style.</param>
    public void SetDefaultColumnStyle( int iColumnIndex, IStyle defaultStyle )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets by column index default style for column.
    /// </summary>
    /// <param name="iStartColumnIndex">Start column index.</param>
    /// <param name="iEndColumnIndex">End column index.</param>
    /// <param name="defaultStyle">Default style.</param>
    public void SetDefaultColumnStyle( int iStartColumnIndex, int iEndColumnIndex, IStyle defaultStyle )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets by column index default style for row.
    /// </summary>
    /// <param name="iRowIndex">Row index.</param>
    /// <param name="defaultStyle">Default style.</param>
    public void SetDefaultRowStyle( int iRowIndex, IStyle defaultStyle )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets by column index default style for row.
    /// </summary>
    /// <param name="iStartRowIndex">Start row index.</param>
    /// <param name="iEndRowIndex">End row index.</param>
    /// <param name="defaultStyle">Default style.</param>
    public void SetDefaultRowStyle( int iStartRowIndex, int iEndRowIndex, IStyle defaultStyle )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Returns default column style.
    /// </summary>
    /// <param name="iColumnIndex">Column index.</param>
    /// <returns>Default column style or null if style wasn't set.</returns>
    public IStyle GetDefaultColumnStyle( int iColumnIndex )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Returns default row style.
    /// </summary>
    /// <param name="iRowIndex">Row index.</param>
    /// <returns>Default row style or null if style wasn't set.</returns>
    public IStyle GetDefaultRowStyle( int iRowIndex )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Free's range object.
    /// </summary>
    /// <param name="range">Range to remove from internal cache.</param>
    public void FreeRange( IRange range )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Free's range object.
    /// </summary>
    /// <param name="iRow">One-based row index of the range object to remove from internal cache.</param>
    /// <param name="iColumn">One-based column index of the range object to remove from internal cache.</param>
    public void FreeRange( int iRow, int iColumn )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets value in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Value to set.</param>
    public void SetValue( int iRow, int iColumn, string value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets value in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Value to set.</param>
    public void SetNumber( int iRow, int iColumn, double value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets value in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Value to set.</param>
    public void SetBoolean( int iRow, int iColumn, bool value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets text in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Text to set.</param>
    public void SetText( int iRow, int iColumn, string value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets formula in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Formula to set.</param>
    public void SetFormula( int iRow, int iColumn, string value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets error in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Error to set.</param>
    public void SetError( int iRow, int iColumn, string value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets blank in specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    public void SetBlank( int iRow, int iColumn )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets formula number value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula number value for set.</param>
    public void SetFormulaNumberValue( int iRow, int iColumn, double value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets formula error value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula error value for set.</param>
    public void SetFormulaErrorValue( int iRow, int iColumn, string value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets formula bool value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula bool value for set.</param>
    public void SetFormulaBoolValue( int iRow, int iColumn, bool value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets formula string value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula string value for set.</param>
    public void SetFormulaStringValue( int iRow, int iColumn, string value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Returns instance of migrant range - row and column of this range
    /// object can be changed by user. Read-only.
    /// </summary>
    public IMigrantRange MigrantRange
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns string value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>String contained by the cell.</returns>
    public string GetText( int row, int column )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Returns number value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>Number contained by the cell.</returns>
    public double GetNumber( int row, int column )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Returns formula value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>Formula contained by the cell.</returns>
    public string GetFormula( int row, int column, bool bR1C1 )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Gets error value from cell.
    /// </summary>
    /// <param name="row">Row index.</param>
    /// <param name="column">Column index.</param>
    /// <returns>Returns error value or null.</returns>
    public string GetError( int row, int column )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Gets bool value from cell.
    /// </summary>
    /// <param name="row">Represents row index.</param>
    /// <param name="column">Represents column index.</param>
    /// <returns>Returns found bool value. If cannot found returns false.</returns>
    public bool GetBoolean( int row, int column )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Gets formula bool value from cell.
    /// </summary>
    /// <param name="row">Represents row index.</param>
    /// <param name="column">Represents column index.</param>
    /// <returns>Returns found bool value. If cannot found returns false.</returns>
    public bool GetFormulaBoolValue( int row, int column )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Gets formula error value from cell.
    /// </summary>
    /// <param name="row">Row index.</param>
    /// <param name="column">Column index.</param>
    /// <returns>Returns error value or null.</returns>
    public string GetFormulaErrorValue( int row, int column )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Returns formula number value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>Number contained by the cell.</returns>
    public double GetFormulaNumberValue( int row, int column )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Returns formula string value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>String contained by the cell.</returns>
    public string GetFormulaStringValue( int row, int column )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
# if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Converts range into image (Bitmap).
    /// </summary>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <returns>Created image.</returns>
    public System.Drawing.Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Converts range into image.
    /// </summary>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="imageType">Type of the image to create.</param>
    /// <param name="stream">Output stream. It is ignored if null.</param>
    /// <returns>Created image.</returns>
    public System.Drawing.Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn, ImageType imageType, System.IO.Stream stream )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Converts range into metafile image.
    /// </summary>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="emfType">Metafile EmfType.</param>
    /// <param name="outputStream">Output stream. It is ignored if null.</param>
    /// <returns>Created image.</returns>
    public System.Drawing.Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn, System.Drawing.Imaging.EmfType emfType, System.IO.Stream outputStream )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Converts range into image.
    /// </summary>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="imageType">Type of the image to create.</param>
    /// <param name="outputStream">Output stream. It is ignored if null.</param>
    /// <param name="emfType">Metafile EmfType.</param>
    /// <returns>Created image.</returns>
    public System.Drawing.Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn, ImageType imageType, System.IO.Stream outputStream, System.Drawing.Imaging.EmfType emfType )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
#endif
    #endregion

    #region ITabSheet Members
    /// <summary>
    /// Gets / sets tab color.
    /// </summary>
    public ExcelKnownColors TabColor
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Gets / sets tab color.
    /// </summary>
    public Color TabColorRGB
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns charts collection. Read-only.
    /// </summary>
    public IChartShapes Charts
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns pictures collection. Read-only.
    /// </summary>
    public IPictures Pictures
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    IWorkbook ITabSheet.Workbook
    {
      get
      {
        return m_book.Workbook;
      }
    }
    /// <summary>
    /// Returns shapes collection. Read-only.
    /// </summary>
    public IShapes Shapes
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Indicates whether worksheet is displayed right to left.
    /// </summary>
    public bool IsRightToLeft
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Indicates whether tab of this sheet is selected. Read-only.
    /// </summary>
    public bool IsSelected
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns index in the parent ITabSheets collection. Read-only.
    /// </summary>
    public int TabIndex
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Gets / sets name of the tab sheet.
    /// </summary>
    public string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        m_strName = value;
      }
    }
    /// <summary>
    /// Control visibility of worksheet to end user.
    /// </summary>
    public WorksheetVisibility Visibility
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns collection with all textboxes inside this worksheet. Read-only.
    /// </summary>
    public ITextBoxes TextBoxes
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns collection with all checkboxes inside this worksheet. Read-only.
    /// </summary>
    public ICheckBoxes CheckBoxes
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Returns collection with all OptionButton inside this worksheet. Read-only.
    /// </summary>
    public IOptionButtons  OptionButtons
    {
        get
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
    /// <summary>
    /// Returns collection with all comboboxes inside this worksheet. Read-only.
    /// </summary>
    public IComboBoxes ComboBoxes
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// Makes the current sheet the active sheet. Equivalent to clicking the
    /// sheet's tab.
    /// </summary>
    public void Activate()
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Selects current tab sheet.
    /// </summary>
    public void Select()
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Unselects current tab sheet.
    /// </summary>
    public void Unselect()
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    #endregion

    #region IInteralWorksheet
    /// <summary>
    /// Return default row height in pixel.
    /// </summary>
    public int DefaultRowHeight
    {
      get
      {
        return 0;
      }
    }
    /// <summary>
    /// Gets or sets one-based index of the first row of the worksheet.
    /// </summary>
    public int FirstRow
    {
      get
      {
        return m_iFirstRow;
      }
      set
      {
        m_iFirstRow = value;
      }
    }
    /// <summary>
    /// Gets or sets one-based index of the first column of the worksheet.
    /// </summary>
    public int FirstColumn
    {
      get
      {
        return m_iFirstColumn;
      }
      set
      {
        m_iFirstColumn = value;
      }
    }
    /// <summary>
    /// Gets or sets one-based index of the last row of the worksheet.
    /// </summary>
    public int LastRow
    {
      get
      {
        return m_iLastRow;
      }
      set
      {
        m_iLastRow = value;
      }
    }
    /// <summary>
    /// Gets or sets one-based index of the last column of the worksheet.
    /// </summary>
    public int LastColumn
    {
      get
      {
        return m_iLastColumn;
      }
      set
      {
        m_iLastColumn = value;
      }
    }
    /// <summary>
    /// Returns collection of cell records. Read-only.
    /// </summary>
    public CellRecordCollection CellRecords
    {
      [DebuggerStepThrough]
      get
      {
        //ParseData();

        return m_dicRecordsCells;
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl ParentWorkbook
    {
      get
      {
        return m_book.Workbook;
      }
    }
    public bool IsArrayFormula( long index )
    {
      return false;
    }
    public ExcelVersion Version
    {
      get
      {
        return ExcelVersion.Excel2007;
      }
    }
    /// <summary>
    /// Gets object that is clone of current worksheet in the specified workbook.
    /// </summary>
    /// <param name="hashNewNames">Dictionary with update worksheet names.</param>
    /// <param name="book">New workbook object.</param>
    /// <returns>Object that is clone of the current worksheet.</returns>
    public IInternalWorksheet GetClonedObject( Dictionary<string, string> hashNewNames, WorkbookImpl book )
    {
      int iBookIndex = m_book.Index;
      int iSheetIndex = Index;
      return book.ExternWorkbooks[ iBookIndex ].Worksheets[ iSheetIndex ];
    }
    #endregion

    #region ICloneParent Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    object ICloneParent.Clone( object parent )
    {
      return this.Clone( parent );
    }

    #endregion

    #region IWorksheet Members

# if !SILVERLIGHT && !WINRT && !WP
    void IWorksheet.SaveAsHtml(Stream stream, HtmlSaveOptions saveOptions)
    {
        throw new NotImplementedException();
    }
#endif
    #endregion
  }
}
