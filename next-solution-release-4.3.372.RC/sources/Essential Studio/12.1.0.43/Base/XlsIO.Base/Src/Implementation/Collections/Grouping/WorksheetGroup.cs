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

#region file using directives
using System;
using System.IO;

using System.Collections;
using System.Text;

using Syncfusion.XlsIO.Interfaces;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.PivotTables;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Implementation.Exceptions;
using System.Threading.Tasks;
using Windows.Storage;
#endif

#if (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

#if !(SILVERLIGHT) && !(WINRT) && !(WP)
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
#endif



#endregion

namespace Syncfusion.XlsIO.Implementation.Collections.Grouping
{
  /// <summary>
  /// Represents group of worksheets.
  /// </summary>
  public class WorksheetGroup
    : CollectionBaseEx<IWorksheet>
    , IWorksheetGroup
    , ICloneParent
  {
    #region Class members
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Page setup.
    /// </summary>
    private PageSetupGroup m_pageSetup;
    /// <summary>
    /// Used range.
    /// </summary>
    private IRange m_usedRange;
    /// <summary>
    /// Migrant range - row and column of this range object can be changed by user.
    /// </summary>
    private IMigrantRange m_migrantRange;
    private SheetView m_view= SheetView.Normal;
    internal int unknown_formula_name = 9;
    private Syncfusion.Calculate.CalcEngine m_calcEngine;
    public event Syncfusion.XlsIO.Implementation.RangeImpl.CellValueChangedEventHandler CellValueChanged;
    #region Events
    /// <summary>
    /// Event raised when an unknown function is encountered.
    /// </summary>
    public event MissingFunctionEventHandler MissingFunction;
    #endregion

    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of the worksheet group.
    /// </summary>
    /// <param name="application">Application object for the new group.</param>
    /// <param name="parent">Parent object for the new group.</param>
    public WorksheetGroup( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
      this.Inserted += new CollectionChange( WorksheetGroup_Inserted );
      this.Removing += new CollectionChange( WorksheetGroup_Removing );
      this.Clearing += new CollectionClear( WorksheetGroup_Clearing );

      IWorksheets arrSheets = m_book.Worksheets;

      for( int i = 0, len = arrSheets.Count; i < len; i++ )
      {
        IWorksheet sheet = arrSheets[ i ];

        if( sheet.IsSelected )
        {
          base.Add( sheet );
        }
      }
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void FindParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
      {
        throw new ArgumentOutOfRangeException( "parent", "Can't find parent workbook" );
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Adds new worksheet to the collection.
    /// </summary>
    /// <param name="sheet">Worksheet to add.</param>
    /// <returns>Index of the added worksheet.</returns>
    /// <exception cref="System.ArgumentNullException">When sheet is Null.</exception>
    public int Add( ITabSheet sheet )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( sheet.Workbook != m_book )
        throw new ArgumentOutOfRangeException( "sheet", "Worksheets from different workbooks can't be grouped." );

      if( sheet.IsSelected && !m_book.Loading )
        return -1;

      if( ( ( WorksheetBaseImpl )sheet ).WindowTwo.IsPaged )
      {
        //m_book.ActiveSheet = sheet;
        //sheet.Activate();
        m_book.SetActiveWorksheet( sheet as WorksheetBaseImpl );
      }

      int result;

      if( sheet is IWorksheet )
      {
        base.Add( sheet as IWorksheet );
        result = Count - 1;
      }
      else
      {
        result = -1;
      }

      return result;
    }
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
    public void SaveAsHtml(string filename)
    {

    }

    public void SaveAsHtml(Stream stream)
    {

    }

    public void SaveAsHtml(string filename,HtmlSaveOptions saveOptions)
    {

    }

    public void SaveAsHtml(Stream stream,HtmlSaveOptions saveOptions)
    {

    }
#endif
    /// <summary>
    /// Removes worksheet from the collection.
    /// </summary>
    /// <param name="sheet">Worksheet to remove.</param>
    /// <exception cref="System.ArgumentNullException">When sheet is Null.</exception>
    public void Remove( ITabSheet sheet )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( Count > 1 )
      {
        base.Remove( sheet as IWorksheet );
        WorksheetBaseImpl activeSheet = ( List[ 0 ] as WorksheetBaseImpl );
        m_book.SetActiveWorksheet( activeSheet );
        AppImplementation.SetActiveWorksheet( activeSheet );
      }
    }

    /// <summary>
    /// Selects single tab sheet.
    /// </summary>
    /// <param name="sheet">Sheet to select.</param>
    public void Select( ITabSheet sheet )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      Clear();
      Add( sheet );
    }
    /// <summary>
    /// Creates migrant range.
    /// </summary>
    private void CreateMigrantRange()
    {
      m_migrantRange = new MigrantRangeGroup( Application, this );
      //m_migrantRange.ResetRowColumn( FirstRow, m_usFirstColumn );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates whether collection is empty. Read-only.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return Count == 0;
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl ParentWorkbook
    {
      get
      {
        return m_book;
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
#if (SILVERLIGHT) || (WINRT) || (WP)
            Dictionary<object, object> namedRanges1 = new Dictionary<object, object>();
#else
            Hashtable namedRanges1 = new Hashtable();
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
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Gets workbook to which current worksheet belong. Read-only.
    /// </summary>
    public IWorkbook Workbook
    {
      get
      {
        return m_book;
      }
    }

    /// <summary>
    /// Returns all used cells in the worksheet. Read-only.
    /// </summary>
    public IRange[] Cells
    {
      get
      {
        // TODO:  Add WorksheetGroup.Cells getter implementation
        return null;
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
            return m_view;
        }
        set
        {
            m_view = value;
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
    /// True if page breaks (both automatic and manual) on the specified
    /// worksheet are displayed. Read / write Boolean.
    /// </summary>
    public bool DisplayPageBreaks
    {
      get
      {
        IList<IWorksheet> list = InnerList;
        bool result = list[ 0 ].DisplayPageBreaks;

        for( int i = 1, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          bool curValue = sheet.DisplayPageBreaks;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        IList<IWorksheet> list = InnerList;

        for( int i = 0, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          sheet.DisplayPageBreaks = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is OLE object.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is OLE object; otherwise, <c>false</c>.
    /// </value>
    public bool HasOleObject
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException(); 
      }
    }

    /// <summary>
    /// Returns the index number of the object within the collection of
    /// similar objects. Read-only.
    /// </summary>
    public int Index
    {
      get
      {
        return -1;
      }
    }

    /// <summary>
    /// Returns index in the parent ITabSheets collection. Read-only.
    /// </summary>
    public int TabIndex
    {
      get
      {
        return -1;
      }
    }
    /// <summary>
    /// True if objects are protected. Read-only.
    /// </summary>
    public bool ProtectDrawingObjects
    {
      get
      {
        // TODO:  Add WorksheetGroup.IsObjectProtection getter implementation
        return false;
      }
    }

    /// <summary>
    /// True if the scenarios of the current sheet are protected. Read-only.
    /// </summary>
    public bool ProtectScenarios
    {
      get
      {
        // TODO:  Add WorksheetGroup.IsScenProtection getter implementation
        return false;
      }
    }

    /// <summary>
    /// Returns all merged ranges. Read-only.
    /// </summary>
    public IRange[] MergedCells
    {
      get
      {
        // TODO:  Add WorksheetGroup.MergedCells getter implementation
        return null;
      }
    }

    /// <summary>
    /// Returns or sets the name of the object. Read / write String.
    /// </summary>
    public string Name
    {
      get
      {
        return null;
      }
      set
      {
        throw new NotSupportedException();
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
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Name that is used by macros to access the workbook items.
    /// </summary>
    public string CodeName
    {
      get
      {
        return null;
      }
      set
      {
        throw new NotSupportedException();
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
        if( m_pageSetup == null )
        {
          m_pageSetup = new PageSetupGroup( Application, this );
        }

        return m_pageSetup;
      }
    }

    /// <summary>
    /// Returns a Range object that represents a cell or a range of cells.
    /// </summary>
    public IRange Range
    {
      get
      {
        return UsedRange;
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
        // TODO:  Add WorksheetGroup.Rows getter implementation
        return null;
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
        // TODO:  Add WorksheetGroup.Columns getter implementation
        return null;
      }
    }

    /// <summary>
    /// Returns the standard (default) height of all the rows in the worksheet,
    /// in points. Read-only Double.
    /// </summary>
    public double StandardHeight
    {
      get
      {
        // TODO:  Add WorksheetGroup.StandardHeight getter implementation
        return 0;
      }
      set
      {
        // TODO:  Add WorksheetGroup.StandardHeight setter implementation
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
        // TODO:  Add WorksheetGroup.StandardHeightFlag getter implementation
        return false;
      }
      set
      {
        // TODO:  Add WorksheetGroup.StandardHeightFlag setter implementation
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
        // TODO:  Add WorksheetGroup.StandardWidth getter implementation
        return 0;
      }
      set
      {
        // TODO:  Add WorksheetGroup.StandardWidth setter implementation
      }
    }

    /// <summary>
    /// Returns or sets the worksheet type. Read-only ExcelSheetType.
    /// </summary>
    public Syncfusion.XlsIO.ExcelSheetType Type
    {
      get
      {
        throw new NotImplementedException();
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
        if( IsEmpty )
          return null;

        WorksheetImpl sheet = ( WorksheetImpl )InnerList[ 0 ];
        int iFirstRow = sheet.FirstRow;
        int iLastRow = sheet.LastRow;
        int iFirstColumn = sheet.FirstColumn;
        int iLastColumn = sheet.LastColumn;

        if( m_usedRange == null || m_usedRange.Row != iFirstRow
          || m_usedRange.Column != iFirstColumn
          || m_usedRange.LastRow != iLastRow
          || m_usedRange.LastColumn != iLastColumn )
        {
          m_usedRange = new RangeGroup( Application, this,
            iFirstRow, iFirstColumn, iLastRow, iLastColumn );
        }

        return m_usedRange;
      }
    }

    /// <summary>
    /// Zoom factor of document. Value must be in range from 10 till 400.
    /// </summary>
    public int Zoom
    {
      get
      {
        IList<IWorksheet> list = InnerList;
        int result = list[ 0 ].Zoom;

        for( int i = 1, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          int curValue = sheet.Zoom;

          if( curValue != result )
          {
            return int.MinValue;
          }
        }

        return result;
      }
      set
      {
        IList<IWorksheet> list = InnerList;

        for( int i = 0, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          sheet.Zoom = value;
        }
      }
    }

    /// <summary>
    /// Control visibility of worksheet to end user.
    /// </summary>
    public WorksheetVisibility Visibility
    {
      get
      {
        IList<IWorksheet> list = InnerList;
        WorksheetVisibility result = list[ 0 ].Visibility;

        for( int i = 1, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          WorksheetVisibility curValue = sheet.Visibility;

          if( curValue != result )
          {
            return WorksheetVisibility.Visible;
          }
        }

        return result;
      }
      set
      {
        IList<IWorksheet> list = InnerList;

        for( int i = 0, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          sheet.Visibility = value;
        }
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
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Position of the horizontal split (py, 0 = No horizontal split):
    /// Unfrozen pane: Height of the top pane(s) (in twips = 1/20 of a point)
    /// Frozen pane: Number of visible rows in top pane(s)
    /// </summary>
    public int HorizontalSplit
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Index to first visible row in bottom pane(s).
    /// </summary>
    public int FirstVisibleRow
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Index to first visible column in right pane(s).
    /// </summary>
    public int FirstVisibleColumn
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Identifier of pane with active cell cursor.
    /// </summary>
    public int ActivePane
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
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
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
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
        IList<IWorksheet> list = InnerList;
        bool result = list[ 0 ].IsGridLinesVisible;

        for( int i = 1, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          bool curValue = sheet.IsGridLinesVisible;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        IList<IWorksheet> list = InnerList;

        for( int i = 0, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          sheet.IsGridLinesVisible = value;
        }
      }
    }
    /// <summary>
    /// Gets/Sets Grid line color.
    /// </summary>
    public ExcelKnownColors GridLineColor
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
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
        IList<IWorksheet> list = InnerList;
        bool result = list[ 0 ].IsRowColumnHeadersVisible;

        for( int i = 1, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          bool curValue = sheet.IsRowColumnHeadersVisible;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        IList<IWorksheet> list = InnerList;

        for( int i = 0, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          sheet.IsRowColumnHeadersVisible = value;
        }
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
        // TODO:  Add WorksheetGroup.VPageBreaks getter implementation
        return null;
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
        // TODO:  Add WorksheetGroup.HPageBreaks getter implementation
        return null;
      }
    }

    /// <summary>
    /// Indicates if all values in the workbook are preserved as strings.
    /// </summary>
    public bool IsStringsPreserved
    {
      get
      {
        IList<IWorksheet> list = InnerList;
        bool result = list[ 0 ].IsStringsPreserved;

        for( int i = 1, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          bool curValue = sheet.IsStringsPreserved;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        IList<IWorksheet> list = InnerList;

        for( int i = 0, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          sheet.IsStringsPreserved = value;
        }
      }
    }

    /// <summary>
    /// Indicates if the worksheet is password protected.
    /// </summary>
    public bool IsPasswordProtected
    {
      get
      {
        // TODO:  Add WorksheetGroup.IsPasswordProtected getter implementation
        return false;
      }
    }

    /// <summary>
    /// Comments collection.
    /// </summary>
    public IComments Comments
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Gets / sets cell by row and index.
    /// </summary>
    public IRange this[ int row, int column ]
    {
      get
      {
        // TODO:  Add WorksheetGroup.this getter implementation
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
        // TODO:  Add WorksheetGroup.Syncfusion.XlsIO.IWorksheet.this getter implementation
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
        // TODO:  Add WorksheetGroup.Syncfusion.XlsIO.IWorksheet.this getter implementation
        return null;
      }
    }

    /// <summary>
    /// Get cell range.
    /// </summary>
    public IRange this[ string name, bool IsR1C1Notation ]
    {
      get
      {
        // TODO:  Add WorksheetGroup.Syncfusion.XlsIO.IWorksheet.this getter implementation
        return null;
      }
    }
    /// <summary>
    /// Collection of all worksheet's hyperlinks.
    /// </summary>
    public IHyperLinks HyperLinks
    {
      get
      {
        throw new NotSupportedException();
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
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Returns collection of custom properties. Read-only.
    /// </summary>
    public IWorksheetCustomProperties CustomProperties
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Returns instance of migrant range - row and column of this range
    /// object can be changed by user. Read-only.
    /// </summary>
    public IMigrantRange MigrantRange
    {
      get
      {
        if( m_migrantRange == null )
          CreateMigrantRange();

        return m_migrantRange;
      }
    }
    /// <summary>
    /// Indicates whether all created range objects should be cached.
    /// </summary>
    public bool UseRangesCache
    {
      get
      {
        if( Count == 0 )
          return false;

        IList<IWorksheet> list = InnerList;
        bool result = list[ 0 ].UseRangesCache;

        for( int i = 1, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          bool curValue = sheet.UseRangesCache;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        IList<IWorksheet> list = InnerList;

        for( int i = 0, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          sheet.UseRangesCache = value;
        }
      }
    }
    /// <summary>
    /// Gets protected options. Read-only. For sets protection options use "Protect" method.
    /// </summary>
    public ExcelSheetProtection Protection
    {
      get
      {
        throw new NotSupportedException( "This property doesnot support in this case." );
      }
    }
    /// <summary>
    /// Indicates is current sheet is protected.
    /// </summary>
    public bool ProtectContents
    {
      get
      {
        throw new NotSupportedException( "This property doesnot supported yet." );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int TopVisibleRow
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int LeftVisibleColumn
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
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
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Returns pivot table collection containing all pivot tables in the worksheet. Read-only.
    /// </summary>
    public IPivotTables PivotTables
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Gets collection of all list objects in the worksheet.
    /// </summary>
    public IListObjects ListObjects
    {
      get
      {
        throw new NotSupportedException();
      }
    }
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Gets the OLE objects.
    /// </summary>
    /// <value>The OLE objects.</value>
    public IOleObjects OleObjects
    {
      get
      {
        throw new NotSupportedException();
      }
    }
#endif
    #endregion

    #region IWorksheet methods
    /// <summary>
    /// Makes the current sheet the active sheet. Equivalent to clicking the
    /// sheet's tab.
    /// </summary>
    public void Activate()
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Copies worksheet into the clipboard.
    /// </summary>
    public void CopyToClipboard()
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Clears worksheet data. Removes all formatting and merges.
    /// </summary>
    void IWorksheet.Clear()
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.Clear();
      }
    }

    /// <summary>
    /// Clears worksheet. Only the data is removed from each cell.
    /// </summary>
    public void ClearData()
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.ClearData();
      }
    }

    /// <summary>
    /// Indicates whether a cell was initialized or accessed by the user.
    /// </summary>
    /// <param name="iRow">One-based row index of the cell.</param>
    /// <param name="iColumn">One-based column index of the cell.</param>
    /// <returns>Value indicating whether the cell was initialized or accessed by the user.</returns>
    public bool Contains( int iRow, int iColumn )
    {
      IList<IWorksheet> list = InnerList;
      bool result = list[ 0 ].Contains( iRow, iColumn );

      if( !result )
        return result;

      for( int i = 1, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        bool curValue = sheet.Contains( iRow, iColumn );

        if( curValue != result )
        {
          return false;
        }
      }

      return result;
    }

    /// <summary>
    /// Creates new instance of IRanges.
    /// </summary>
    /// <returns>New instance of ranges collection.</returns>
    public IRanges CreateRangesCollection()
    {
      // TODO:  Add WorksheetGroup.CreateRangesCollection implementation
      return null;
    }

      /// <summary>
    /// Create Named Ranges
    /// </summary>
    /// <param name="namedRange">Names to create</param>
    /// <param name="referRange">Refers to range</param>
    /// <param name="vertical">True if the named range values are vertically placed in the sheet.</param>
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
      throw new NotSupportedException( "Template markers are not supported for grouped worksheets." );
    }
    /// <summary>
    /// Method check is Column with specified index visible to end user or not.
    /// </summary>
    /// <param name="columnIndex">Index of column.</param>
    /// <returns>True - column is visible; otherwise False.</returns>
    public bool IsColumnVisible( int columnIndex )
    {
      IList<IWorksheet> list = InnerList;
      bool result = list[ 0 ].IsColumnVisible( columnIndex );

      for( int i = 1, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        bool curValue = sheet.IsColumnVisible( columnIndex );

        if( curValue != result )
        {
          return false;
        }
      }

      return result;
    }

    /// <summary>
    /// Shows / Hides the specified column.
    /// </summary>
    /// <param name="columnIndex">Index at which the column should be hidden.</param>
    /// <param name="isVisible">True - Column is hidden.</param>
    public void ShowColumn( int columnIndex, bool isVisible )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.ShowColumn( columnIndex, isVisible );
      }
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
      IList<IWorksheet> list = InnerList;
      bool result = list[ 0 ].IsRowVisible( rowIndex );

      for( int i = 1, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        bool curValue = sheet.IsRowVisible( rowIndex );

        if( curValue != result )
        {
          return false;
        }
      }

      return result;
    }

    /// <summary>
    /// Shows / Hides the specified row.
    /// </summary>
    /// <param name="rowIndex">Index at which the row should be hidden.</param>
    /// <param name="isVisible">True - Row is hidden.</param>
    public void ShowRow( int rowIndex, bool isVisible )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.ShowRow( rowIndex, isVisible );
      }
    }
	/// <summary>
    /// Shows / Hides the specified range.
    /// </summary>
    /// <param name="range">Range specifies the particular range to show / hide</param>
    /// <param name="isVisible">True - Range is visible; false - hidden.</param>
    public void ShowRange(IRange range, bool isVisible)
    {
        IList<IWorksheet> list = InnerList;
                
        foreach (IWorksheet sheet in list)                   
            sheet.ShowRange(range, isVisible);               
    }
    /// <summary>
    /// Shows/ Hides the collection of range.
    /// </summary>
    /// <param name="ranges">Ranges specifies the range collection.</param>
    /// <param name="isVisible">True - Row is visible; false - hidden.</param>
    public void ShowRange(RangesCollection ranges, bool isVisible)
    {
        if (ranges.Count == 0)
            return;

        foreach (IRange range in ranges)
        {
            ShowRange(range, isVisible);
        }
    }
    /// <summary>
    /// Shows/ Hides an array of range.
    /// </summary>
    /// <param name="ranges">Ranges specifies the range array.</param>
    /// <param name="isVisible">True - Row is visible; false - hidden.</param>
    public void ShowRange(IRange[] ranges, bool isVisible)
    {
        if (ranges.Length == 0)
            return;

        RangesCollection collection = new RangesCollection(Application, this);
        foreach (IRange range in ranges)
        {
            collection.Add(range);
        }
        ShowRange(collection, isVisible);
    }

    /// <summary>
    /// Inserts an empty row with default formatting (with formulas update).
    /// </summary>
    /// <param name="index">Index at which new row should be inserted.</param>
    public void InsertRow( int index )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.InsertRow( index );
      }
    }

    /// <summary>
    /// Inserts an empty row with default formatting.
    /// </summary>
    /// <param name="iRowIndex">Index at which new row should be inserted.</param>
    /// <param name="iRowCount">Number of rows to insert.</param>
    public void InsertRow( int iRowIndex, int iRowCount )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.InsertRow( iRowIndex, iRowCount );
      }
    }

    /// <summary>
    /// Inserts an empty row with default formatting.
    /// </summary>
    /// <param name="iRowIndex">Index at which new row should be inserted.</param>
    /// <param name="iRowCount">Number of rows to insert.</param>
    /// <param name="insertOptions">Insert options.</param>
    public void InsertRow( int iRowIndex, int iRowCount,
      ExcelInsertOptions insertOptions )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.InsertRow( iRowIndex, iRowCount, insertOptions );
      }
    }

    /// <summary>
    /// Inserts an empty column with default formatting.
    /// </summary>
    /// <param name="index">Index at which new column should be inserted.</param>
    public void InsertColumn( int index )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.InsertColumn( index );
      }
    }

    /// <summary>
    /// Inserts an empty column with default formatting (without formulas update).
    /// </summary>
    /// <param name="iColumnIndex">Index at which new column should be inserted.</param>
    /// <param name="iColumnCount">Number of columns to insert.</param>
    public void InsertColumn( int iColumnIndex, int iColumnCount )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.InsertColumn( iColumnIndex, iColumnCount );
      }
    }

    /// <summary>
    /// Inserts an empty column with default formatting (without formulas update).
    /// </summary>
    /// <param name="iColumnIndex">Index at which new column should be inserted.</param>
    /// <param name="iColumnCount">Number of columns to insert.</param>
    /// <param name="options">Insert options.</param>
    public void InsertColumn( int iColumnIndex, int iColumnCount,
      ExcelInsertOptions options )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.InsertColumn( iColumnIndex, iColumnCount, options );
      }
    }

    /// <summary>
    /// Removes specified row (without updating formulas).
    /// </summary>
    /// <param name="index">One-based row index to remove.</param>
    public void DeleteRow( int index )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.DeleteRow( index );
      }
    }
    /// <summary>
    /// Removes specified row (without updating formulas).
    /// </summary>
    /// <param name="index">One-based row index to remove.</param>
    /// <param name="count">Number of rows to remove.</param>
    public void DeleteRow( int index, int count )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.DeleteRow( index, count );
      }
    }
    /// <summary>
    /// Removes specified column (without updating formulas).
    /// </summary>
    /// <param name="index">One-based column index to remove.</param>
    public void DeleteColumn( int index )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.DeleteColumn( index );
      }
    }
    /// <summary>
    /// Removes specified column (without updating formulas).
    /// </summary>
    /// <param name="index">One-based column index to remove.</param>
    /// <param name="count">Number of columns to delete.</param>
    public void DeleteColumn( int index, int count )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.DeleteColumn( index, count );
      }
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
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportArray( arrObject, firstRow, firstColumn, isVertical );
      }

      return iResult;
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
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportArray( arrString, firstRow, firstColumn, isVertical );
      }

      return iResult;
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
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportArray( arrInt, firstRow, firstColumn, isVertical );
      }

      return iResult;
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
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportArray( arrDouble, firstRow, firstColumn, isVertical );
      }

      return iResult;
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
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportArray( arrDateTime, firstRow, firstColumn, isVertical );
      }

      return iResult;
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
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportArray( arrObject, firstRow, firstColumn );
      }

      return iResult;
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
        IList<IWorksheet> list = InnerList;
        int iResult = 0;

        for (int i = 0, len = list.Count; i < len; i++)
        {
            IWorksheet sheet = list[i];
            iResult = sheet.ImportData(arrObject, firstRow, firstColumn, includeHeader);
        }

        return iResult;
    }
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Imports data from a DataColumn into worksheet.
    /// </summary>
    /// <param name="dataColumn">DataColumn with desired data.</param>
    /// <param name="isFieldNameShown">True if column name must also be imported.</param>
    /// <param name="firstRow">Row of the first cell where DataTable should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where DataTable should be imported.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataColumn( DataColumn dataColumn, bool isFieldNameShown,
      int firstRow, int firstColumn )
    {
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportDataColumn( dataColumn, isFieldNameShown, firstRow, firstColumn );
      }

      return iResult;
    }

    /// <summary>
    /// Imports data from a DataTable into worksheet.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="isFieldNameShown">True if column names must also be imported.</param>
    /// <param name="firstRow">Row of the first cell where DataTable should be imported.</param>
    /// <param name="firstColumn">Column of the first cell where DataTable should be imported.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataTable( DataTable dataTable, bool isFieldNameShown,
      int firstRow, int firstColumn )
    {
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportDataTable( dataTable, isFieldNameShown, firstRow, firstColumn );
      }

      return iResult;
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
    public int ImportDataTable( DataTable dataTable, bool isFieldNameShown,
      int firstRow, int firstColumn, bool preserveTypes )
    {
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportDataTable( dataTable, isFieldNameShown,
          firstRow, firstColumn, preserveTypes );
      }

      return iResult;
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
    public int ImportDataTable( DataTable dataTable, bool isFieldNameShown,
      int firstRow, int firstColumn, int maxRows, int maxColumns )
    {
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportDataTable( dataTable, isFieldNameShown,
          firstRow, firstColumn, maxRows, maxColumns );
      }

      return iResult;
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
    public int ImportDataTable( DataTable dataTable, bool isFieldNameShown,
      int firstRow, int firstColumn, int maxRows, int maxColumns, bool preserveTypes )
    {
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportDataTable( dataTable, isFieldNameShown,
          firstRow, firstColumn, maxRows, maxColumns, preserveTypes );
      }

      return iResult;
    }

    /// <summary>
    /// Imports data from a DataTable into namedRange.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="namedRange">Represents named range.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataTable( DataTable dataTable, IName namedRange, bool isFieldNameShown )
    {
      return ImportDataTable( dataTable, namedRange, isFieldNameShown, 0, 0 );
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
    public int ImportDataTable( DataTable dataTable, IName namedRange, bool isFieldNameShown
      , int rowOffset, int columnOffset )
    {
      return ImportDataTable( dataTable, namedRange, isFieldNameShown
        , rowOffset, columnOffset, -1, -1 );
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
    public int ImportDataTable( DataTable dataTable, IName namedRange, bool isFieldNameShown
      , int rowOffset, int columnOffset, int iMaxRow, int iMaxCol )
    {
      return ImportDataTable( dataTable, namedRange, isFieldNameShown
        , rowOffset, columnOffset, iMaxRow, iMaxCol, false );
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
    public int ImportDataTable( DataTable dataTable, IName namedRange, bool isFieldNameShown
      , int rowOffset, int columnOffset, int iMaxRow, int iMaxCol, bool bPreserveTypes )
    {
      if( InnerList.Count == 0 )
        return 0;

      IWorksheet sheet = ( IWorksheet )InnerList[ 0 ];

      return sheet.ImportDataTable( dataTable, namedRange, isFieldNameShown, rowOffset
        , columnOffset, iMaxRow, iMaxCol, bPreserveTypes );
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
    public int ImportDataView( DataView dataView, bool isFieldNameShown,
      int firstRow, int firstColumn )
    {
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportDataView( dataView, isFieldNameShown, firstRow, firstColumn );
      }

      return iResult;
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
    public int ImportDataView( DataView dataView, bool isFieldNameShown,
      int firstRow, int firstColumn, bool bPreserveTypes )
    {
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportDataView( dataView, isFieldNameShown,
          firstRow, firstColumn, bPreserveTypes );
      }

      return iResult;
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
    public int ImportDataView( DataView dataView, bool isFieldNameShown,
      int firstRow, int firstColumn, int maxRows, int maxColumns )
    {
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportDataView( dataView, isFieldNameShown,
          firstRow, firstColumn, maxRows, maxColumns );
      }

      return iResult;
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
    public int ImportDataView( DataView dataView, bool isFieldNameShown,
      int firstRow, int firstColumn, int maxRows, int maxColumns, bool bPreserveTypes )
    {
      IList<IWorksheet> list = InnerList;
      int iResult = 0;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        iResult = sheet.ImportDataView( dataView, isFieldNameShown,
          firstRow, firstColumn, maxRows, maxColumns, bPreserveTypes );
      }

      return iResult;
    }
#endif
    /// <summary>
    /// Removes panes from a worksheet.
    /// </summary>
    public void RemovePanes()
    {
      throw new NotSupportedException();
    }
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Exports worksheet data into a DataTable.
    /// </summary>
    /// <param name="firstRow">Row of the first cell from where DataTable should be exported.</param>
    /// <param name="firstColumn">Column of the first cell from where DataTable should be exported.</param>
    /// <param name="maxRows">Maximum number of rows to export.</param>
    /// <param name="maxColumns">Maximum number of columns to export.</param>
    /// <param name="options">Export options.</param>
    /// <returns>DataTable with worksheet data.</returns>
    public DataTable ExportDataTable( int firstRow, int firstColumn,
      int maxRows, int maxColumns, ExcelExportDataTableOptions options )
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Exports worksheet data into a DataTable.
    /// </summary>
    /// <param name="dataRange">Range to export.</param>
    /// <param name="options">Export options.</param>
    /// <returns>DataTable with worksheet data.</returns>
    public DataTable ExportDataTable( IRange dataRange, ExcelExportDataTableOptions options )
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// Exports worksheet data into a DataTable only for Pivot engine.
    /// </summary>
    /// <param name="dataRange">Range to export.</param>
    /// <param name="options">Export options.</param>
    /// <returns>DataTable with worksheet data.</returns>
    public DataTable PEExportDataTable(IRange dataRange, ExcelExportDataTableOptions options,PivotTableImpl pivotTable)
    {
        throw new NotSupportedException();
    }
#endif
    /// <summary>
    /// Protects worksheet's content with password.
    /// </summary>
    /// <param name="password">Password to protect with.</param>
    public void Protect( string password )
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Protects current worksheet.
    /// </summary>
    /// <param name="password">Represents password to protect.</param>
    /// <param name="options">Represents params to protect.</param>
    public void Protect( string password, ExcelSheetProtection options )
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// Unprotects worksheet's content with password.
    /// </summary>
    /// <param name="password">Password to unprotect.</param>
    public void Unprotect( string password )
    {
      throw new NotSupportedException();
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
      // TODO:  Add WorksheetGroup.IntersectRanges implementation
      return null;
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
      // TODO:  Add WorksheetGroup.MergeRanges implementation
      return null;
    }

    /// <summary>
    /// Autofits specified row.
    /// </summary>
    /// <param name="rowIndex">One-based row index.</param>
    public void AutofitRow( int rowIndex )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.AutofitRow( rowIndex );
      }
    }

    /// <summary>
    /// Autofits specified column.
    /// </summary>
    /// <param name="colIndex">One-based column index.</param>
    public void AutofitColumn( int colIndex )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.AutofitColumn( colIndex );
      }
    }

    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    public void Replace( string oldValue, string newValue )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.Replace( oldValue, newValue );
      }
    }

    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    public void Replace( string oldValue, double newValue )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.Replace( oldValue, newValue );
      }
    }

    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    public void Replace( string oldValue, DateTime newValue )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.Replace( oldValue, newValue );
      }
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
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.Replace( oldValue, newValues, isVertical );
      }
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
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.Replace( oldValue, newValues, isVertical );
      }
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
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.Replace( oldValue, newValues, isVertical );
      }
    }
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Replaces specified string by data table values.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Data table with new data.</param>
    /// <param name="isFieldNamesShown">Indicates whether field name must be shown.</param>
    public void Replace( string oldValue, DataTable newValues, bool isFieldNamesShown )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.Replace( oldValue, newValues, isFieldNamesShown );
      }
    }

    /// <summary>
    /// Replaces specified string by data column values.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Data table with new data.</param>
    /// <param name="isFieldNamesShown">Indicates whether field name must be shown.</param>
    public void Replace( string oldValue, DataColumn newValues, bool isFieldNamesShown )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.Replace( oldValue, newValues, isFieldNamesShown );
      }
    }

#endif
    /// <summary>
    /// Removes worksheet from parent worksheets collection.
    /// </summary>
    public void Remove()
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.Remove();
      }

      base.Clear();
    }

    /// <summary>
    /// Moves worksheet.
    /// </summary>
    /// <param name="iNewIndex">New index of the worksheet.</param>
    public void Move( int iNewIndex )
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Converts column width into pixels.
    /// </summary>
    /// <param name="widthInChars">Width in characters.</param>
    /// <returns>Width in pixels</returns>
    public int ColumnWidthToPixels( double widthInChars )
    {
      IWorksheet sheet = ( IWorksheet )List[ 0 ];
      return sheet.ColumnWidthToPixels( widthInChars );
    }

    /// <summary>
    /// Converts pixels into column width (in characters).
    /// </summary>
    /// <param name="pixels">Width in pixels</param>
    /// <returns>Width in characters.</returns>
    public double PixelsToColumnWidth( int pixels )
    {
      IWorksheet sheet = ( IWorksheet )List[ 0 ];
      return sheet.PixelsToColumnWidth( pixels );
    }

    /// <summary>
    /// Sets column width.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index.</param>
    /// <param name="value">Width to set.</param>
    public void SetColumnWidth( int iColumnIndex, double value )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetColumnWidth( iColumnIndex, value );
      }
    }

    /// <summary>
    /// Sets column width.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index.</param>
    /// <param name="value">Width in pixels to set.</param>
    public void SetColumnWidthInPixels( int iColumnIndex, int value )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetColumnWidthInPixels( iColumnIndex, value );
      }
    }
      /// <summary>
    /// Set Column width from start Column index to End Column index
      /// </summary>
    /// <param name="iStartColumnIndex">start Column index</param>
    /// <param name="iCount">No of Column to be set width</param>
      /// <param name="value">Value to set in pixels</param>
    public void SetColumnWidthInPixels(int iStartColumnIndex, int iCount, int value)
    {
        IList<IWorksheet> list = InnerList;

        for (int i = 0, len = list.Count; i < len; i++)
        {
            IWorksheet sheet = list[i];
            sheet.SetColumnWidthInPixels(iStartColumnIndex, iCount, value);
        }
    }
    /// <summary>
    /// Sets row height.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="value">Height to set.</param>
    public void SetRowHeight( int iRow, double value )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetRowHeight( iRow, value );
      }
    }

    /// <summary>
    /// Sets row height in pixels.
    /// </summary>
    /// <param name="iRowIndex">One-based row index to set height.</param>
    /// <param name="value">Value in pixels to set.</param>
    public void SetRowHeightInPixels( int iRowIndex, double value )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetRowHeightInPixels( iRowIndex, value );
      }
    }
      /// <summary>
    /// Set Row height from Start Row index to End Row index
      /// </summary>
    /// <param name="iStartRowIndex">Start Row index</param>
    /// <param name="iCount">No of Row to be set width</param>
      /// <param name="value">value in pixels to set</param>
    public void SetRowHeightInPixels(int iStartRowIndex, int iCount, double value)
    {
        IList<IWorksheet> list = InnerList;

        for (int i = 0, len = list.Count; i < len; i++)
        {
            IWorksheet sheet = list[i];
            sheet.SetRowHeightInPixels(iStartRowIndex,iCount, value);
        }
    }
    /// <summary>
    /// Returns width from ColumnInfoRecord if there is corresponding ColumnInfoRecord
    /// or StandardWidth if not.
    /// </summary>
    /// <param name="iColumnIndex">One-based index of the column.</param>
    /// <returns>Width of the specified column.</returns>
    public double GetColumnWidth( int iColumnIndex )
    {
      IList<IWorksheet> list = InnerList;
      double result = list[ 0 ].GetColumnWidth( iColumnIndex );

      for( int i = 1, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = ( IWorksheet )list[ i ];
        double curValue = sheet.GetColumnWidth( iColumnIndex );

        if( curValue != result )
        {
          result = double.NaN;
          break;
        }
      }

      return result;
    }

    /// <summary>
    /// Returns width in pixels from ColumnInfoRecord if there is corresponding ColumnInfoRecord
    /// or StandardWidth if not.
    /// </summary>
    /// <param name="iColumnIndex">One-based index of the column.</param>
    /// <returns>Width in pixels of the specified column.</returns>
    public int GetColumnWidthInPixels( int iColumnIndex )
    {
      IList<IWorksheet> list = InnerList;
      int result = list[ 0 ].GetColumnWidthInPixels( iColumnIndex );

      for( int i = 1, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        int curValue = sheet.GetColumnWidthInPixels( iColumnIndex );

        if( curValue != result )
        {
          return int.MinValue;
        }
      }

      return result;
    }

    /// <summary>
    /// Returns height from RowRecord if there is a corresponding RowRecord.
    /// Otherwise returns StandardHeight. 
    /// </summary>
    /// <param name="iRowIndex">One-based index of the row</param>
    /// <returns>
    /// Height from RowRecord if there is corresponding RowRecord.
    /// Otherwise returns StandardHeight.
    /// </returns>
    public double GetRowHeight( int iRowIndex )
    {
      IList<IWorksheet> list = InnerList;
      double result = list[ 0 ].GetRowHeight( iRowIndex );

      for( int i = 1, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        double curValue = sheet.GetRowHeight( iRowIndex );

        if( curValue != result )
        {
          return double.NaN;
        }
      }

      return result;
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
      IList<IWorksheet> list = InnerList;
      int result = list[ 0 ].GetRowHeightInPixels( iRowIndex );

      for( int i = 1, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        int curValue = sheet.GetRowHeightInPixels( iRowIndex );

        if( curValue != result )
        {
          return int.MinValue;
        }
      }

      return result;
    }

    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( string findValue, ExcelFindType flags )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst(string findValue, ExcelFindType flags, ExcelFindOptions findOptions)
    {
        throw new Exception("The method or operation is not implemented.");
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
        throw new Exception("The method or operation is not implemented.");
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
    public IRange FindStringStartsWith(string findValue, ExcelFindType flags, bool ignoreCase)
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
    public IRange FindStringEndsWith(string findValue, ExcelFindType flags)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    public IRange FindStringEndsWith(string findValue, ExcelFindType flags, bool ignoreCase)
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
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the first cell with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( bool findValue )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the first cell with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( DateTime findValue )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the first cell with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( TimeSpan findValue )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the all cells with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( string findValue, ExcelFindType flags )
    {
      throw new NotImplementedException();
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
    public IRange[] FindAll(string findValue, ExcelFindType flags, ExcelFindOptions findOptions)
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
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the all cells with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found</returns>
    public IRange[] FindAll( bool findValue )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the all cells with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( DateTime findValue )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// This method searches for the all cells with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( TimeSpan findValue )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="fileName">File to save.</param>
    /// <param name="separator">Current separator.</param>
    public void SaveAs( string fileName, string separator )
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="fileName">File to save.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    public void SaveAs( string fileName, string separator, Encoding encoding )
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save. </param>
    /// <param name="separator">Current separator.</param>
    public void SaveAs( Stream stream, string separator )
    {
      throw new NotSupportedException();
    }
#if ( WINRT )
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="storageFile">StorageFile to save. </param>
    /// <param name="separator">Current separator.</param>
    public Task<bool> SaveAsAsync(StorageFile storageFile, string separator)
    {
        throw new NotSupportedException();
    }
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="storageFile">StorageFile to save. </param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    public Task<bool> SaveAsAsync(StorageFile storageFile, string separator, Encoding encoding)
    {
        throw new NotSupportedException();
    }
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save. </param>
    /// <param name="separator">Current separator.</param>
    public Task<bool> SaveAsAsync(Stream stream, string separator)
    {
        throw new NotSupportedException();
    }
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save. </param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    public Task<bool> SaveAsAsync(Stream stream, string separator, Encoding encoding)
    {
        throw new NotSupportedException();
    }

#endif
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save. </param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    public void SaveAs( Stream stream, string separator, Encoding encoding )
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Sets by column index default style for column.
    /// </summary>
    /// <param name="iColumnIndex">Column index.</param>
    /// <param name="defaultStyle">Default style.</param>
    public void SetDefaultColumnStyle( int iColumnIndex, IStyle defaultStyle )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetDefaultColumnStyle( iColumnIndex, defaultStyle );
      }
    }

    /// <summary>
    /// Sets by column index default style for column.
    /// </summary>
    /// <param name="iStartColumnIndex">Start column index.</param>
    /// <param name="iEndColumnIndex">End column index.</param>
    /// <param name="defaultStyle">Default style.</param>
    public void SetDefaultColumnStyle( int iStartColumnIndex, int iEndColumnIndex, IStyle defaultStyle )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetDefaultColumnStyle( iStartColumnIndex, iEndColumnIndex, defaultStyle );
      }
    }

    /// <summary>
    /// Sets by column index default style for row.
    /// </summary>
    /// <param name="iRowIndex">Row index.</param>
    /// <param name="defaultStyle">Default style.</param>
    public void SetDefaultRowStyle( int iRowIndex, IStyle defaultStyle )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetDefaultRowStyle( iRowIndex, defaultStyle );
      }
    }

    /// <summary>
    /// Sets by column index default style for row.
    /// </summary>
    /// <param name="iStartRowIndex">Start row index.</param>
    /// <param name="iEndRowIndex">End row index.</param>
    /// <param name="defaultStyle">Default style.</param>
    public void SetDefaultRowStyle( int iStartRowIndex, int iEndRowIndex, IStyle defaultStyle )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetDefaultRowStyle( iStartRowIndex, iEndRowIndex, defaultStyle );
      }
    }

    /// <summary>
    /// Returns default row style.
    /// </summary>
    /// <param name="iRowIndex">Row index.</param>
    /// <returns>Default row style or null if style wasn't set.</returns>
    public IStyle GetDefaultRowStyle( int iRowIndex )
    {
      // TODO: finish implementation if necessary.
      return null;
    }

    /// <summary>
    /// Returns default Column style.
    /// </summary>
    /// <param name="iColumnIndex">Column index.</param>
    /// <returns>Default column style or null if style wasn't set.</returns>
    public IStyle GetDefaultColumnStyle( int iColumnIndex )
    {
      // TODO: finish implementation if necessary.
      return null;
    }
    /// <summary>
    /// Free's range object.
    /// </summary>
    /// <param name="range">Range to remove from internal cache.</param>
    public void FreeRange( IRange range )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Free's range object.
    /// </summary>
    /// <param name="iRow">One-based row index of the range object to remove from internal cache.</param>
    /// <param name="iColumn">One-based column index of the range object to remove from internal cache.</param>
    public void FreeRange( int iRow, int iColumn )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Sets value in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Value to set.</param>
    public void SetValue( int iRow, int iColumn, string value )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetValue( iRow, iColumn, value );
      }
    }
    /// <summary>
    /// Sets value in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Value to set.</param>
    public void SetNumber( int iRow, int iColumn, double value )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetNumber( iRow, iColumn, value );
      }
    }
    /// <summary>
    /// Sets value in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Value to set.</param>
    public void SetBoolean( int iRow, int iColumn, bool value )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetBoolean( iRow, iColumn, value );
      }
    }
    /// <summary>
    /// Sets text in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Text to set.</param>
    public void SetText( int iRow, int iColumn, string value )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetText( iRow, iColumn, value );
      }
    }
    /// <summary>
    /// Sets formula in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Formula to set.</param>
    public void SetFormula( int iRow, int iColumn, string value )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetFormula( iRow, iColumn, value );
      }
    }
    /// <summary>
    /// Sets error in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Error to set.</param>
    public void SetError( int iRow, int iColumn, string value )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetError( iRow, iColumn, value );
      }
    }
    /// <summary>
    /// Sets blank in specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    public void SetBlank( int iRow, int iColumn )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetBlank( iRow, iColumn );
      }
    }
    /// <summary>
    /// Sets formula number value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula number value for set.</param>
    public void SetFormulaNumberValue( int iRow, int iColumn, double value )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetFormulaNumberValue( iRow, iColumn, value );
      }
    }
    /// <summary>
    /// Sets formula error value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula error value for set.</param>
    public void SetFormulaErrorValue( int iRow, int iColumn, string value )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetFormulaErrorValue( iRow, iColumn, value );
      }
    }
    /// <summary>
    /// Sets formula bool value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula bool value for set.</param>
    public void SetFormulaBoolValue( int iRow, int iColumn, bool value )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetFormulaBoolValue( iRow, iColumn, value );
      }
    }
    /// <summary>
    /// Sets formula string value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula string value for set.</param>
    public void SetFormulaStringValue( int iRow, int iColumn, string value )
    {
      IList<IWorksheet> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        IWorksheet sheet = list[ i ];
        sheet.SetFormulaStringValue( iRow, iColumn, value );
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
      throw new NotSupportedException();
    }
    /// <summary>
    /// Returns number value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>Number contained by the cell.</returns>
    public double GetNumber( int row, int column )
    {
      throw new NotSupportedException();
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
      throw new NotSupportedException();
    }
    /// <summary>
    /// Gets error value from cell.
    /// </summary>
    /// <param name="row">Row index.</param>
    /// <param name="column">Column index.</param>
    /// <returns>Returns error value or null.</returns>
    public string GetError( int row, int column )
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// Gets bool value from cell.
    /// </summary>
    /// <param name="row">Represents row index.</param>
    /// <param name="column">Represents column index.</param>
    /// <returns>Returns found bool value. If cannot found returns false.</returns>
    public bool GetBoolean( int row, int column )
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// Returns formula string value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>String contained by the cell.</returns>
    public string GetFormulaStringValue( int row, int column )
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// Returns formula number value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>Number contained by the cell.</returns>
    public double GetFormulaNumberValue( int row, int column )
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// Gets formula error value from cell.
    /// </summary>
    /// <param name="row">Row index.</param>
    /// <param name="column">Column index.</param>
    /// <returns>Returns error value or null.</returns>
    public string GetFormulaErrorValue( int row, int column )
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// Gets formula bool value from cell.
    /// </summary>
    /// <param name="row">Represents row index.</param>
    /// <param name="column">Represents column index.</param>
    /// <returns>Returns found bool value. If cannot found returns false.</returns>
    public bool GetFormulaBoolValue( int row, int column )
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// Defines whether freeze panes are applied.
    /// </summary>
    public bool IsFreezePanes
    {
      get
      {
        IList<IWorksheet> list = InnerList;

        for( int i = 0, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];

          if( !sheet.IsFreezePanes )
          {
            return false;
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Return split cell range.
    /// </summary>
    public IRange SplitCell
    {
      get
      {
        throw new NotImplementedException( "Split Cell" );
      }
    }
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Converts range into image (Bitmap).
    /// </summary>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <returns></returns>
    public Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn )
    {
      throw new NotSupportedException();
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
    public Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn,
      ImageType imageType, Stream stream )
    {
      throw new NotSupportedException();
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
    public Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn,
      EmfType emfType, Stream outputStream )
    {
      throw new NotImplementedException();
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
    public Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn,
      ImageType imageType, Stream outputStream, EmfType emfType )
    {
      throw new NotImplementedException();
    }
#endif
    #endregion

    #region ITabSheet Members
    /// <summary>
    /// Gets / sets tab color.
    /// </summary>
    public Syncfusion.XlsIO.ExcelKnownColors TabColor
    {
      get
      {
        IList<IWorksheet> list = InnerList;
        ExcelKnownColors result = list[ 0 ].TabColor;

        for( int i = 1, len = list.Count; i < len; i++ )
        {
          ITabSheet sheet = list[ i ];
          ExcelKnownColors curColor = sheet.TabColor;

          if( curColor != result )
          {
            return ExcelKnownColors.None;
          }
        }

        return result;
      }
      set
      {
        IList<IWorksheet> list = InnerList;

        for( int i = 0, len = list.Count; i < len; i++ )
        {
          ITabSheet sheet = list[ i ];
          sheet.TabColor = value;
        }
      }
    }

    /// <summary>
    /// Gets / sets tab color.
    /// </summary>
    public Color TabColorRGB
    {
      get
      {
        IList<IWorksheet> list = InnerList;
        Color result = list[ 0 ].TabColorRGB;

        for( int i = 1, len = list.Count; i < len; i++ )
        {
          ITabSheet sheet = list[ i ];
          Color curColor = sheet.TabColorRGB;

          if( curColor != result )
          {
            return ColorExtension.Empty;
          }
        }

        return result;
      }
      set
      {
        // TODO:  Add WorksheetGroup.TabColorRGB setter implementation
      }
    }

    /// <summary>
    /// Returns charts collection. Read-only.
    /// </summary>
    public IChartShapes Charts
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Returns pictures collection. Read-only.
    /// </summary>
    public IPictures Pictures
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Returns shapes collection. Read-only.
    /// </summary>
    public IShapes Shapes
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Selects current tab sheet.
    /// </summary>
    public void Select()
    {
    }
    /// <summary>
    /// Unselects current tab sheet.
    /// </summary>
    public void Unselect()
    {
    }
    /// <summary>
    /// Indicates whether worksheet is displayed right to left.
    /// </summary>
    public bool IsRightToLeft
    {
      get
      {
        IList<IWorksheet> list = InnerList;
        bool result = list[ 0 ].IsRightToLeft;

        for( int i = 1, len = list.Count; i < len; i++ )
        {
          ITabSheet sheet = list[ i ];
          bool curValue = sheet.IsRightToLeft;

          if( curValue != result || !result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        IList<IWorksheet> list = InnerList;

        for( int i = 0, len = list.Count; i < len; i++ )
        {
          ITabSheet sheet = list[ i ];
          sheet.IsRightToLeft = value;
        }
      }
    }
    /// <summary>
    /// Indicates whether tab of this sheet is selected. Read-only.
    /// </summary>
    public bool IsSelected
    {
      get
      {
        return true;
      }
    }
    /// <summary>
    /// Returns collection with all textboxes inside this worksheet. Read-only.
    /// </summary>
    public ITextBoxes TextBoxes
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Returns collection with all checkboxes inside this worksheet. Read-only.
    /// </summary>
    public ICheckBoxes CheckBoxes
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Returns collection with all OptionButtons inside this worksheet. Read-only.
    /// </summary>
    public IOptionButtons  OptionButtons
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Returns collection with all comboboxes inside this worksheet. Read-only.
    /// </summary>
    public IComboBoxes ComboBoxes
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    #endregion

    #region Event handlers
    /// <summary>
    /// Event handler for Inserted event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="args">Event arguments.</param>
    private void WorksheetGroup_Inserted( object sender, CollectionChangeEventArgs<IWorksheet> args )
    {
      WorksheetBaseImpl sheet = args.Value as WorksheetBaseImpl;
      sheet.SelectTab();
      m_book.WindowOne.NumSelectedTabs = ( ushort )Count;

      if( Count == 1 )
      {
        m_book.WindowOne.SelectedTab = ( ushort )sheet.RealIndex;
      }
    }
    /// <summary>
    /// Event handler for Removing event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="args">Event arguments.</param>
    private void WorksheetGroup_Removing( object sender, CollectionChangeEventArgs<IWorksheet> args )
    {
      if( Count == 1 )
        throw new ApplicationException( "Can't deselect all worksheets." );

      ITabSheet sheet = args.Value;
      sheet.Unselect();
    }
    /// <summary>
    /// This method is called before clearing all elements.
    /// </summary>
    private void WorksheetGroup_Clearing()
    {
      for( int i = 0, len = Count; i < len; i++ )
      {
        // Use 'as' to increase performance.
        WorksheetBaseImpl sheet = List[ i ] as WorksheetBaseImpl;
        sheet.Unselect( false );
      }

      m_book.WindowOne.NumSelectedTabs = 0;
    }
    #endregion

    #region ICloneParent Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone( object parent )
    {
      WorksheetGroup result = new WorksheetGroup( Application, parent );
      IList<IWorksheet> listSource = InnerList;
      IList<IWorksheet> listDest = result.InnerList;
      WorkbookObjectsCollection objectsDest = result.m_book.Objects;

      for( int i = 0, len = Count; i < len; i++ )
      {
        WorksheetBaseImpl sheet = ( WorksheetBaseImpl )listSource[ i ];
        int iRealIndex = sheet.RealIndex;
        listDest.Add( objectsDest[ iRealIndex ] as IWorksheet );
      }

      return result;
    }

    internal bool? GetStringPreservedValue( RangeGroup rangeGroup )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    internal void SetStringPreservedValue( RangeGroup rangeGroup, bool? value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    #endregion

      #region Dispose
    protected override void OnClear()
    {
        base.OnClear();
        if (m_book == null)
        {
            m_book = null;
  }
        if (m_calcEngine != null)
        {
            m_calcEngine.Dispose();
            ValueChanged = null;
            m_calcEngine = null;
        }
        if (m_pageSetup != null)
        {
            m_pageSetup.Dispose();
        }
        if (m_usedRange != null)
        {
            m_usedRange = null;
        }
    }
      #endregion
  }
}
