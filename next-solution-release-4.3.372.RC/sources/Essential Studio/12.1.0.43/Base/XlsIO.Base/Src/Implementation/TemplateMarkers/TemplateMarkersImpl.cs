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
using System.Collections;
using System.Reflection;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.TemplateMarkers;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System.Collections.Generic;
using System.Globalization;
using Syncfusion.XlsIO.Implementation.PivotTables;
#if ( WINRT )
using System.Reflection;
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;

using Syncfusion.XlsIO;

#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#endif

#if SILVERLIGHT
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using Syncfusion.XlsIO.Implementation.WP;
#elif !(WINRT )
using System.Drawing;
using System.Data;
using Syncfusion.XlsIO.FormatParser;


#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.TemplateMarkers
{
	/// <summary>
	/// This class is used to apply template markers to the workbook or worksheet.
	/// </summary>
  public class TemplateMarkersImpl
    : CommonObject
    , ITemplateMarkersProcessor
  {
    #region Class constants
      ///<sumarry>
      ///Number of rows need to be created
      /// </sumarry>
      private int m_insertCount = 0;
    /// <summary>
    /// Default marker prefix.
    /// </summary>
    private const string DEF_MARKER_PREFIX = "%";
    /// <summary>
    /// Delimiter between property parts.
    /// </summary>
    private const string DEF_PARTS_SEPARATOR = ".";
    /// <summary>
    /// Default string for Null value.
    /// </summary>
    private const string DEF_NULL_VALUE = "NULL";
    /// <summary>
    /// Standard separator between marker arguments.
    /// </summary>
    private const char DEF_STANDARD_SEPARATOR = ';';
    #endregion

    #region Class static members
    /// <summary>
    /// List with instances of possible argument classes.
    /// </summary>
    private static readonly List<MarkerArgument> s_arrArguments = new List<MarkerArgument>();
    /// <summary>
    /// Possible parent types.
    /// </summary>
    private static readonly Type[] DEF_PARENT_TYPES = new Type[]
    {
      typeof( WorksheetImpl ), typeof( WorkbookImpl )
    };
    #endregion

    #region Class members
    /// <summary>
    /// Dictionary with user-defined variables.
    /// </summary>
    private Dictionary<string, object> m_dicVariables = new Dictionary<string, object>();
    /// <summary>
    /// Marker prefix. String that indicates that cell contains marker.
    /// </summary>
    private string m_strMarkerPrefix = DEF_MARKER_PREFIX;
    /// <summary>
    /// Arguments separator.
    /// </summary>
    private char m_chSeparator = DEF_STANDARD_SEPARATOR;
    /// <summary>
    /// Parent workbook object.
    /// </summary>
    private WorkbookImpl m_book;
      /// <summary>
      /// Represents the auto detection of the marker variable.
      /// </summary>
    private Dictionary<string, VariableTypeAction> m_variableTypeActions = new Dictionary<string, VariableTypeAction>();
      /// <summary>
      /// Represent the conditional formats in the template marker.
      /// </summary>
    private Dictionary<string, CondFormatCollectionWrapper> m_conditionalFormats;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes static members of the TemplateMarkersImpl class.
    /// </summary>
    static TemplateMarkersImpl()
    {
      s_arrArguments.Add( new CopyRangeArgument() );
      s_arrArguments.Add( new DirectionArgument() );
      s_arrArguments.Add( new JumpArgument() );
      s_arrArguments.Add( new NewSpaceArgument() );
    }
    /// <summary>
    /// Initializes a new instance of the TemplateMarkersImpl class.
    /// </summary>
    /// <param name="application">Application object for the new object.</param>
    /// <param name="parent">Parent object for the new object.</param>
    public TemplateMarkersImpl( IApplication application, object parent )
      : base( application, parent )
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "Cannot find parent workbook." );
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Applies markers to the parent object.
    /// </summary>
    public void ApplyMarkers()
    {
      ApplyMarkers( UnknownVariableAction.Exception );
    }
    /// <summary>
    /// Applies markers to the parent object.
    /// </summary>
    public void ApplyMarkers( UnknownVariableAction action )
    {
      object parent = FindParent( DEF_PARENT_TYPES );

      if( parent == null )
        throw new ArgumentOutOfRangeException( "Can't find parent workbook or worksheet" );

      IWorksheet sheet = parent as IWorksheet;
      m_book.InnerFormats.FillFormatIndexes();
      if( sheet != null )
      {
        ApplyMarkers( sheet, action );
      }
      else
      {
        ApplyMarkers( ( IWorkbook )parent, action );
      }
    }
    /// <summary>
    /// Adds new variable to the collection.
    /// </summary>
    /// <param name="strName">Name of the new variable.</param>
    /// <param name="variable">Variable value.</param>
    public void AddVariable( string strName, object variable )
    {
        AddVariable(strName, variable, VariableTypeAction.None);
    }
    /// <summary>
    /// Adds new variable to the collection.
    /// </summary>
    /// <param name="strName">Name of the new variable.</param>
    /// <param name="variable">Variable value.</param>\
    /// <param name="makerType">Represents the marker Type.</param>
    public void AddVariable(string strName, object variable, VariableTypeAction makerType)
    {
        if (strName == null)
            throw new ArgumentNullException("strName");

        if (strName.Length == 0)
            throw new ArgumentException("strName - string cannot be empty.");

        if (variable == null)
            throw new ArgumentNullException("variable");
        m_variableTypeActions.Add(strName, makerType);
        m_dicVariables.Add(strName, variable);
    }
    /// <summary>
    /// Removes variable from the collection.
    /// </summary>
    /// <param name="strName">Variable name.</param>
    public void RemoveVariable( string strName )
    {
        m_variableTypeActions.Remove(strName);
      m_dicVariables.Remove( strName );
    }
    /// <summary>
    /// Applies markers to the specified worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet to apply markers to.</param>
    public void ApplyMarkers( IWorksheet sheet )
    {
      ApplyMarkers( sheet, UnknownVariableAction.Exception );
    }
    /// <summary>
    /// Applies markers to the specified worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet to apply markers to.</param>
    public void ApplyMarkers( IWorksheet sheet, UnknownVariableAction action )
    {
      // First of all we have to locate all smart markers in the worksheet.
      // To do this as quick as possible we have to access to SST dictionary
      // and ask it for all string indexes starting with marker prefix
      // then we have to find all cells with smart markers on the worksheet

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      WorkbookImpl book = ( WorkbookImpl )sheet.Workbook;
      SSTDictionary sst = book.InnerSST;
      List<int> arrLabels = sst.StartWith( m_strMarkerPrefix );
      ApplyMarkers( sheet, arrLabels, action );
    }
    /// <summary>
    /// Applies markers to the whole workbook.
    /// </summary>
    /// <param name="book">Workbook to apply markers to.</param>
    public void ApplyMarkers( IWorkbook book )
    {
      ApplyMarkers( book, UnknownVariableAction.Exception );
    }
    /// <summary>
    /// Applies markers to the whole workbook.
    /// </summary>
    /// <param name="book">Workbook to apply markers to.</param>
    public void ApplyMarkers( IWorkbook book, UnknownVariableAction action )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      WorkbookImpl workbook = ( WorkbookImpl )book;
      SSTDictionary sst = workbook.InnerSST;
      List<int> arrLabels = sst.StartWith( m_strMarkerPrefix );
      IWorksheets arrSheets = book.Worksheets;

      for( int i = 0, len = arrSheets.Count; i < len; i++ )
      {
        IWorksheet sheet = arrSheets[ i ];
        ApplyMarkers( sheet, arrLabels, action );
      }
    }
    /// <summary>
    /// Checks whether template markers object contains variable with specified name.
    /// </summary>
    /// <param name="strName">Name to locate.</param>
    /// <returns>Value indicating whether template markers object contains variable with specified name.</returns>
    public bool ContainsVariable( string strName )
    {
      return m_dicVariables.ContainsKey( strName );
    }
	/// <summary>
    /// Adds conditional format to the Template Marker.
    /// </summary>
    /// <param name="range">Represents the range where the conditional format to be applied.</param>
    /// <remarks>The conditional format range should be within the template marker range.</remarks>
    public IConditionalFormats CreateConditionalFormats(IRange range)
    {
        CondFormatCollectionWrapper conditionalFormat = null;
        if (m_conditionalFormats == null)
            m_conditionalFormats = new Dictionary<string, CondFormatCollectionWrapper>();

        if (m_conditionalFormats.ContainsKey(range.AddressGlobal))
            conditionalFormat = m_conditionalFormats[range.AddressGlobal];
        else
        {
            conditionalFormat = range.ConditionalFormats as CondFormatCollectionWrapper;
            m_conditionalFormats.Add(range.AddressGlobal, conditionalFormat);
        }

        return conditionalFormat;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Applies markers to the worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet to apply markers to.</param>
    /// <param name="arrLabels">All cell indexes with markers.</param>
    private void ApplyMarkers( IWorksheet sheet, List<int> arrLabels, UnknownVariableAction action )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( arrLabels == null )
        throw new ArgumentNullException( "arrLabels" );

      IList<long> arrCells = PrepareCellIndexes( sheet, arrLabels );

      int pivotTableCount = sheet.PivotTables.Count;
      if (pivotTableCount > 0)
      {
          List<long> filteredCells = new List<long>();
          foreach (long cellIndex in arrCells)
          {
              int iRow = RangeImpl.GetRowFromCellIndex(cellIndex);
              int iColumn = RangeImpl.GetColumnFromCellIndex(cellIndex);
              for (int pivotTableIndex = 0; pivotTableIndex < sheet.PivotTables.Count; pivotTableIndex++)
              {
                  PivotTableImpl pivotTable = sheet.PivotTables[pivotTableIndex] as PivotTableImpl;
                  if (!(iRow >= pivotTable.Location.Row && iRow <= pivotTable.Location.LastRow && iColumn >= pivotTable.Location.Column && iColumn <= pivotTable.Location.LastColumn))
                  {
                      filteredCells.Add(cellIndex);
                  }
              }
          }
          arrCells = filteredCells;
      }
      int iCount = arrCells.Count;
      List<RangeBuilder> arrResultRanges = new List<RangeBuilder>( iCount );
      IMigrantRange range = new MigrantRangeImpl( Application, sheet );

      if (CheckClassMarker(sheet, action, arrCells, range))
      {
          WorkbookImpl workbook = (WorkbookImpl)sheet.Workbook;
          SSTDictionary sst = workbook.InnerSST;
          arrLabels = sst.StartWith(m_strMarkerPrefix);
          arrCells = PrepareCellIndexes(sheet, arrLabels);
          iCount = arrCells.Count;
          arrResultRanges = new List<RangeBuilder>(iCount);
          range = new MigrantRangeImpl(Application, sheet);
      }
      for( int i = 0; i < iCount; i++ )
      {
        arrResultRanges.Add( ApplyMarker( sheet, arrCells, i, range, action ) );
      }
      ApplyConditionalFormats(arrResultRanges,sheet);

      if( iCount > 0 )
        UpdateChartRanges( sheet.Workbook, sheet, arrCells, arrResultRanges );
    }
    /// <summary>
    /// Checks the class marker in the worksheet.
    /// </summary>
    /// <param name="sheet">The current sheet object.</param>
    /// <param name="action">The unknown variable action.</param>
    /// <param name="arrCells">The array of cell indexes.</param>
    /// <param name="migrantRange">The migrant range to apply the indexes.</param>
    /// <returns>Indicates whether the sheet contains the class marker. </returns>
    public bool CheckClassMarker(IWorksheet sheet, UnknownVariableAction action, IList<long> arrCells,
        IMigrantRange migrantRange)
    {
        Dictionary<string, object>.ValueCollection.Enumerator valueEnum = m_dicVariables.Values.GetEnumerator();
         object obj;
         bool hasChange = false;
        while (valueEnum.MoveNext())
        {
           obj=valueEnum.Current;
           if (obj == null)
               continue;
            if(IsArray(obj))
            {
                IList listObj = (IList)obj;
                if (listObj.Count != 0)
                    obj=listObj[0];
            }
            else if(IsCollection(obj))
            {
                IEnumerator enu= ((ICollection)obj).GetEnumerator();
                enu.MoveNext();
                obj=enu.Current;
            }
            else
                return hasChange;
            if (obj.GetType().Namespace==null || (obj.GetType().Namespace!=null && !obj.GetType().Namespace.Contains("System")))
            {
                hasChange = CheckAndApplyHeaders(obj, sheet, action, arrCells, migrantRange);
            }
        }
            return hasChange;
    }
    /// <summary>
    /// Checks and applies headers.
    /// </summary>
    /// <param name="obj">The obj to get the headers.</param>
    /// <param name="sheet">The current sheet object.</param>
    /// <param name="action">The unknown variable action.</param>
    /// <param name="arrCells">The array of cells indexes.</param>
    /// <param name="migrantRange">The migrant range to set the header names.</param>
    /// <returns>Indicates whether the markers are applied.</returns>
    public bool CheckAndApplyHeaders(object obj, IWorksheet sheet, UnknownVariableAction action, IList<long> arrCells,
        IMigrantRange migrantRange)
    {
        string className = obj.GetType().Name;
        bool hasChange = false;
        foreach (long cellIndex in arrCells)
        {
            WorksheetImpl sheetImpl = sheet as WorksheetImpl;
            string marker = sheetImpl.GetStringValue(cellIndex);
            if (marker.Contains(className))
            {
                if (marker.Split(DEF_PARTS_SEPARATOR.ToCharArray()).Length == 1)
                {
                    MarkerOptionsImpl option;
                    string arguments = null;
                    string variableName = null;
                    string orginalMarker = marker;
                    variableName = GetVariableName(ref marker, out arguments);
                    IList lstArguments= ParseArguments(arguments, out option);
                    migrantRange = new MigrantRangeImpl(Application, sheet);
                    int iRow = RangeImpl.GetRowFromCellIndex(cellIndex);
                    int iColumn = RangeImpl.GetColumnFromCellIndex(cellIndex);
                    migrantRange.ResetRowColumn(iRow, iColumn);
                    SetObjectHeader(obj, orginalMarker, sheet, migrantRange, arrCells, lstArguments, option, action);
                    hasChange = true;
                }
            }
        }

        return hasChange;
    }

    /// <summary>
    /// Sets the object header.
    /// </summary>
    /// <param name="obj">The obj to get the headers.</param>
    /// <param name="strMarkerText">The marker text.</param>
    /// <param name="sheet">The current sheet object.</param>
    /// <param name="migrantRange">The migrant range to set the header.</param>
    /// <param name="arrMarkerCells">The index of the marker cells.</param>
    /// <param name="lstArguments">The list of marker arguments.</param>
    /// <param name="options">The marker options.</param>
    /// <param name="action">The unknown variable action.</param>
    private void SetObjectHeader(object obj,string strMarkerText, IWorksheet sheet, IMigrantRange migrantRange, IList<long> arrMarkerCells, 
        IList lstArguments, MarkerOptionsImpl options,UnknownVariableAction action)
    {
        IList columnNames;
        IList membersInfo = GetObjectMembersInfo(obj,out columnNames);
        int row, column;
        row = migrantRange.Row;
        column = migrantRange.Column;
        int tmpColumn = column;
        int tmpRow = row;
        bool isRowRelative = ArrangeRowsOrColumns(sheet, migrantRange, options, membersInfo.Count);
        RangeBuilder builder = new RangeBuilder();
        int index = strMarkerText.IndexOf(DEF_STANDARD_SEPARATOR);
        if (index == -1)
            index = strMarkerText.Length;
        for (int i = 0; i < membersInfo.Count; i++)
        {
            string item = membersInfo[i].ToString();
            string name=columnNames[i].ToString();
            if (i != 0)
            {
                string[] names = strMarkerText.Split(new char[]{DEF_STANDARD_SEPARATOR});
                string tmpDirection = new DirectionArgument().TryGetDirection(strMarkerText);
                if (tmpDirection != null)
                {
                    strMarkerText = names[0] + DEF_STANDARD_SEPARATOR + tmpDirection;
                }
                else
                    strMarkerText = names[0];

            }
            string tmpMarkerText = strMarkerText.Insert(index, DEF_PARTS_SEPARATOR + item);
            if (isRowRelative)
            {
                sheet.SetValue(tmpRow, tmpColumn, name);
                sheet.SetValue(tmpRow, tmpColumn + 1, tmpMarkerText);
                tmpRow++;
            }
            else
            {

                sheet.SetValue(tmpRow, tmpColumn, name);
                sheet.SetValue(tmpRow + 1, tmpColumn, tmpMarkerText);
                tmpColumn++;
            }
           
        }
    }
    /// <summary>
    /// Arrange the row and columns based on the object headers and direction.
    /// </summary>
    /// <param name="sheet">The curretn sheet object.</param>
    /// <param name="migrantRange">The migrant range to inser the rows or columns.</param>
    /// <param name="options">The marker options.</param>
    /// <param name="insertCount">Represent the number of row or columns to insert.</param>
    /// <returns>indicates whether arrange cells in rows or columns relatives.</returns>
    private bool ArrangeRowsOrColumns(IWorksheet sheet, IMigrantRange migrantRange, MarkerOptionsImpl options,int insertCount)
    {
        bool isRowRelative = true;
        int iRow;
        int iColumn;

        if (options.Direction == MarkerDirection.Vertical)
        {
            isRowRelative = false;
            sheet.InsertColumn(migrantRange.Column, insertCount-1, ExcelInsertOptions.FormatAsAfter);
            sheet.InsertRow(migrantRange.Row+1);
        }
        else
        {
            isRowRelative = true;
            iRow = 1;
            iColumn = 0;
            sheet.InsertRow(migrantRange.Row, insertCount-1, ExcelInsertOptions.FormatAsAfter);
            sheet.InsertColumn(migrantRange.Column+1);
        }
        return isRowRelative;
    }
    /// <summary>
    /// Gets the object members info.
    /// </summary>
    /// <param name="obj">The obj to get the members.</param>
    /// <returns></returns>
    private IList GetObjectMembersInfo(object obj, out IList columnNames)
    {
        Type type = obj.GetType();
        IList propertyNames = new List<string>();
        columnNames = new List<string>();
        PropertyInfo[] propertiesInfo = type.GetProperties();
        foreach (PropertyInfo property in propertiesInfo)
        {
            string name = property.Name;
            object[] objects = 
#if ( WINRT )
        objects = property.GetCustomAttributes<TemplateMarkerAttributes>().ToArray<object>();
#else
        objects = property.GetCustomAttributes(true);
#endif
            if (objects != null && objects.Length > 0)
            {
                TemplateMarkerAttributes attribute = objects[0] as TemplateMarkerAttributes;
                if (attribute.Exclude)
                    continue;
                if (attribute.HeaderName != null)
                    name = attribute.HeaderName;
            }
            columnNames.Add(name);
            propertyNames.Add(property.Name);
        }
        return propertyNames;

    }
    /// <summary>
    /// Applies the condtional format to the given ranges.
    /// </summary>
    /// <param name="ranges">Represents the ranges where the conditional format to be applied.</param>
    private void ApplyConditionalFormats(List<RangeBuilder> ranges,IWorksheet sheet)
    {
        if (m_conditionalFormats == null)
            return;

        if(ranges.Count==0)
            return;

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        bool isVertical = false;
        int length = 0;
        bool contains = false;
        foreach (KeyValuePair<string, CondFormatCollectionWrapper> keyValue in m_conditionalFormats)
        {
            RangesOperations rangesOperations = new RangesOperations();
            IRange valueRange = keyValue.Value.Range;
            Rectangle rectangle = RangeImpl.GetRectangeOfRange(valueRange, true);
            rangesOperations.AddRectangles(new Rectangle[]{rectangle});
            if (!valueRange.Worksheet.Name.Equals(sheet.Name))
                continue;
            foreach (RangeBuilder rangeBuilder in ranges)
            {
                if (rangeBuilder.Count == 0)
                {
                    contains=true;
                    continue;
                }
                
                Rectangle r = rangeBuilder[0];
                int width = r.Width;
                int height = r.Height;
                isVertical = width == 0;

                r.Width = 0;
                r.Height = 0;
                if (rangesOperations.Contains(r))
                {
                    if (isVertical)
                        length = height;
                    else
                        length = width;
                    contains = true;
                    break;
                }
            }

            if (!contains)
                 throw new ArgumentException("The specified conditional format range is an invalid Template marker range.");

            Rectangle rect = keyValue.Value.ConditionalFormats.CellRectangles[0];
            if (isVertical)
                rect.Height = rect.Height +length;
            else
                rect.Width = rect.Width +length;

            keyValue.Value.ConditionalFormats.AddRange(rect);
            contains = false;
               
        }
    }
    /// <summary>
    /// Updates chart ranges.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="arrCells">List with cells that contained smartmarkers.</param>
    /// <param name="arrResultRanges">List with RangeBuilder instances.</param>
    private void UpdateChartRanges( IWorkbook book, IWorksheet sheet, IList<long> arrCells,
      IList<RangeBuilder> arrResultRanges )
    {
      if( arrCells == null )
        throw new ArgumentNullException( "arrCells" );

      if( arrResultRanges == null )
        throw new ArgumentNullException( "arrResultRanges" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      for( int i = 0, iCount = arrCells.Count; i < iCount; i++ )
      {
        long lCellIndex = arrCells[ i ];
        RangeBuilder builder = arrResultRanges[ i ];

        UpdateChartRanges( book, sheet, lCellIndex, builder );
      }

    }
    /// <summary>
    /// Update reference to the cell with template.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="cellIndex">Cell index.</param>
    /// <param name="builder">Builder with info about new range.</param>
    private void UpdateChartRanges( IWorkbook book, IWorksheet sheet, long cellIndex, RangeBuilder builder )
    {
      IWorksheets arrSheets = book.Worksheets;;

      for( int i = 0, iSheetCount = arrSheets.Count; i < iSheetCount; i++ )
      {
        UpdateSheetCharts( arrSheets[ i ], sheet, cellIndex, builder );
      }

      ICharts arrCharts = book.Charts;
      for( int i = 0, iChartCount = arrCharts.Count; i < iChartCount; i++ )
      {
        IChart chart = arrCharts[ i ];
        UpdateChartRanges( chart, sheet, cellIndex, builder );
      }
    }

    /// <summary>
    /// Updates all charts in the worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet to update.</param>
    /// <param name="sheetCell">Worksheet that contains specified cell.</param>
    /// <param name="cellIndex">Cell index with cell address to update.</param>
    /// <param name="builder">Contains updated range info.</param>
    private void UpdateSheetCharts( IWorksheet sheet, IWorksheet sheetCell, long cellIndex, RangeBuilder builder )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( builder == null )
        throw new ArgumentNullException( "builder" );

      if( sheetCell == null )
        throw new ArgumentNullException( "sheetCell" );

      IChartShapes arrCharts = sheet.Charts;

      for( int j = 0, iChartCount = arrCharts.Count; j < iChartCount; j++ )
      {
        IChartShape chart = arrCharts[ j ];
        UpdateChartRanges( chart, sheetCell, cellIndex, builder );
      }
    }

    /// <summary>
    /// Updates chart ranges after marker application for single chart.
    /// </summary>
    /// <param name="chart">Chart to update.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="cellIndex">Cell index with cell address to update.</param>
    /// <param name="builder">Contains updated range info.</param>
    private void UpdateChartRanges( IChart chart, IWorksheet sheet, long cellIndex, RangeBuilder builder )
    {
      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( builder == null )
        throw new ArgumentNullException( "builder" );

      IChartSeries arrSeries = chart.Series;

      for( int iSerieIndex = 0, len = arrSeries.Count; iSerieIndex < len; iSerieIndex++ )
      {
        IChartSerie serie = arrSeries[ iSerieIndex ];

        IRange range = serie.Values;
        if( CompareRange( range, sheet, cellIndex ) )
        {
          serie.Values = builder.ToRange( sheet );
        }

        range = serie.CategoryLabels;
        if( CompareRange( range, sheet, cellIndex ) )
        {
          serie.CategoryLabels = builder.ToRange( sheet );
        }

        range = serie.Bubbles;
        if( CompareRange( range, sheet, cellIndex ) )
        {
          serie.Bubbles = builder.ToRange( sheet );
        }
      }
    }
    /// <summary>
    /// Compares range and cell index.
    /// </summary>
    /// <param name="range">Range to compare.</param>
    /// <param name="sheet">Parent worksheet for the second range to compare.</param>
    /// <param name="cellIndex">CellIndex of the second range to compare.</param>
    /// <returns>True if range is equal to pair of sheet and cell index.</returns>
    private bool CompareRange( IRange range, IWorksheet sheet, long cellIndex )
    {
      if( range == null ) return false;

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      int iRow = RangeImpl.GetRowFromCellIndex( cellIndex );
      int iColumn = RangeImpl.GetColumnFromCellIndex( cellIndex );

      return( range.Row == iRow && range.LastRow == iRow
        && range.Column == iColumn && range.LastColumn == iColumn );
    }
    /// <summary>
    /// Searches for all strings that contains smart markers.
    /// </summary>
    /// <param name="sheet">Worksheet to search into.</param>
    /// <param name="arrLabels">Labels with smart markers.</param>
    /// <returns>List with all cell indexes that contains smart markers.</returns>
    private IList<long> PrepareCellIndexes( IWorksheet sheet, List<int> arrLabels )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      WorksheetImpl worksheet = ( WorksheetImpl )sheet;
      CellRecordCollection cells = worksheet.CellRecords;
      List<long> result = new List<long>();
      int iCount = arrLabels.Count;

      foreach( DictionaryEntry entry in cells )
      {
        BiffRecordRaw record = ( BiffRecordRaw )entry.Value;
        long lCellIndex = ( long )entry.Key;

        switch( record.TypeCode )
        {
          case TBIFFRecord.LabelSST:
            if( iCount > 0 )
            {
              LabelSSTRecord labelSST = ( LabelSSTRecord )record;
              int iSSTIndex = labelSST.SSTIndex;

              int iIndex = arrLabels.BinarySearch( iSSTIndex );

              if( iIndex >= 0 )
              {
                result.Add( lCellIndex );
              }
            }
            break;

          case TBIFFRecord.Label:
          case TBIFFRecord.RString:
            IStringValue stringValue = ( IStringValue )record;
            string value = stringValue.StringValue;

            if( value.StartsWith( m_strMarkerPrefix ) )
            {
              result.Add( lCellIndex );
            }

            break;
        }
      }

      return result;
    }
    /// <summary>
    /// Applies marker that is located in the specified cell.
    /// </summary>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="arrCells">List with all cells that contains markers.</param>
    /// <param name="i">Cell index in the list.</param>
    /// <param name="migrantRange">Object that defines current cell, where value must be placed.</param>
    /// <returns>Range builder with update marker range information.</returns>
    private RangeBuilder ApplyMarker( IWorksheet sheet, IList<long> arrCells, int i,
      IMigrantRange migrantRange, UnknownVariableAction action )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( arrCells == null )
        throw new ArgumentNullException( "arrCells" );

      if( migrantRange == null )
        throw new ArgumentNullException( "migrantRange" );

      int iCount = arrCells.Count;

      if( i < 0 || i > iCount - 1 )
        throw new ArgumentOutOfRangeException( "i", "Value cannot be less than 0 and greater than iCount - 1." );

      WorksheetImpl worksheet = ( WorksheetImpl )sheet;
      long lCellIndex = arrCells[ i ];
      string strText = worksheet.GetStringValue( lCellIndex );
      string strArguments;
      string strOriginalText = strText;
      int iRow = RangeImpl.GetRowFromCellIndex( lCellIndex );
      int iColumn = RangeImpl.GetColumnFromCellIndex( lCellIndex );
      if (strText == null || (strText!=null && strText.Trim().Length==0))
      {
          throw new ArgumentNullException("Cannot find the variable in the cell ["+iRow+","+iColumn+"]");
      }
      string strVariable = GetVariableName( ref strText, out strArguments );
      strVariable = strVariable.Trim();
      // Sorted list with arguments: key - argument priority, value - argument object.
      MarkerOptionsImpl options;
      IList lstArguments = ParseArguments( strArguments, out options );
      options.MarkerIndex = i;
      options.OriginalMarker = strOriginalText;
      RangeBuilder builder = new RangeBuilder();
      migrantRange.ResetRowColumn( iRow, iColumn );

      SetVariable( strVariable, strText, sheet, migrantRange, arrCells, lstArguments, options, builder, action );

      return builder;
    }
    /// <summary>
    /// Extracts variable name from existing text and removes it from original text.
    /// </summary>
    /// <param name="strText">Text to parse.</param>
    /// <param name="strArguments">Marker arguments.</param>
    /// <returns>Variable name.</returns>
    private string GetVariableName( ref string strText, out string strArguments )
    {
      strArguments = null;

      if( strText == null )
        throw new ArgumentNullException( "strText" );

      if( strText.Length == 0 )
        throw new ArgumentException( "strText - string cannot be empty." );

      int iIndex = strText.IndexOf( m_chSeparator );

      if( iIndex > 0 )
      {
        strArguments = strText.Substring( iIndex + 1 );
        strText = strText.Substring( 0, iIndex );
      }

      if( strText.StartsWith( m_strMarkerPrefix ) )
      {
        strText = strText.Remove( 0, m_strMarkerPrefix.Length );
      }

      iIndex = strText.IndexOf( DEF_PARTS_SEPARATOR );

      string strVariableName = ( iIndex >= 0 ) ? strText.Substring( 0, iIndex ) : strText;
      strText = ( iIndex >= 0 ) ? strText.Remove( 0, iIndex + 1 ) : string.Empty;

      return strVariableName;
    }
    /// <summary>
    /// Imports variable value.
    /// </summary>
    /// <param name="strVariable">Variable name to set.</param>
    /// <param name="strText">Text after variable that can allow to access to different properties.</param>
    /// <param name="sheet">Worksheet that contains cell with marker to process.</param>
    /// <param name="migrantRange">Object that defines current cell, where value must be placed.</param>
    /// <param name="arrMarkerCells">List with all cells that contain template markers.</param>
    /// <param name="lstArguments">List with marker arguments sorted by priority.</param>
    /// <param name="options">Marker options.</param>
    /// <param name="builder">Range builder that will get resulting range information.</param>
    private void SetVariable( string strVariable, string strText, IWorksheet sheet,
      IMigrantRange migrantRange, IList<long> arrMarkerCells, IList lstArguments, MarkerOptionsImpl options,
      RangeBuilder builder, UnknownVariableAction action )
    {
      if( strVariable == null )
        throw new ArgumentNullException( "strVariable" );

      if( strVariable.Length == 0 )
        throw new ArgumentException( "strVariable - string cannot be empty." );

      object value;
      m_dicVariables.TryGetValue( strVariable, out value );

      if( value == null )
      {
        switch( action )
        {
          case UnknownVariableAction.ReplaceBlank:
            value = string.Empty;
            break;

          case UnknownVariableAction.Skip:
            return;

          case UnknownVariableAction.Exception:
            throw new ArgumentOutOfRangeException( strVariable, "Can't find variable" );
        }
      }
      VariableTypeAction variableTypeAction = VariableTypeAction.None;
      m_variableTypeActions.TryGetValue(strVariable, out variableTypeAction);       
        
      //sheet.SetValue( iRow, iColumn, string.Empty );
      //sheet[ iRow, iColumn ].Text = string.Empty;
      migrantRange.Text = string.Empty;
      SetUnknownVariable( value, strText, sheet, migrantRange, arrMarkerCells,
        lstArguments, options, builder, action, variableTypeAction );
    }
    /// <summary>
    /// Detects variable type and sets value.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="strText">Text after variable that can allow to access to different properties.</param>
    /// <param name="sheet">Worksheet that contains cell with marker to process.</param>
    /// <param name="migrantRange">Object that defines current cell, where value must be placed.</param>
    /// <param name="arrMarkerCells">List with all cells that contain template markers.</param>
    /// <param name="lstArguments">List with marker arguments sorted by priority.</param>
    /// <param name="options">Template marker options.</param>
    /// <param name="builder">Range builder that will get resulting range information.</param>
    private void SetUnknownVariable( object value, string strText, IWorksheet sheet,
      IMigrantRange migrantRange, IList<long> arrMarkerCells, IList lstArguments,
      MarkerOptionsImpl options, RangeBuilder builder, UnknownVariableAction action,VariableTypeAction variableTypeAction )
    {
      // TODO: here we have to extract all arguments that can change behaviour of the algorithm.
#if !SILVERLIGHT && !WINRT && !WP
      if( IsDataView( value ) )
      {
        SetDataView( ( DataView )value, strText, sheet, migrantRange,
          arrMarkerCells, lstArguments, options, builder, action,variableTypeAction );
      }
      else
#endif
      if( IsArray( value ) )
      {
        SetArrayValue( ( IList )value, strText, sheet, migrantRange, arrMarkerCells,
          lstArguments, options, builder, action,variableTypeAction ); 
      }
      else if( IsCollection( value ) )
      {
        SetCollectionValue( ( ICollection )value, strText, sheet, migrantRange,
          arrMarkerCells, lstArguments, options, builder, action );
      }
#if !SILVERLIGHT && !WINRT && !WP
      else if( IsDataSet( value ) )
      {
        SetDataSetValue( ( DataSet )value, strText, sheet, migrantRange,
          arrMarkerCells, lstArguments, options, builder, action,variableTypeAction );
      }
      else if( IsDataTable( value ) )
      {
        SetDataTable( ( DataTable )value, strText, sheet, migrantRange,
          arrMarkerCells, lstArguments, options, builder, action,variableTypeAction );
      }
      else if( IsDataColumn( value ) )
      {
        SetDataColumn( ( DataColumn )value, strText, sheet, migrantRange,
          arrMarkerCells, lstArguments, options, builder, action,variableTypeAction );
      }
#endif
      else
      {
        SetSimpleValue( value, strText, sheet, migrantRange, arrMarkerCells,
          lstArguments, options, builder, action,null,null );
      }
    }
    /// <summary>
    /// Detects variable type and sets value.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="strText">Text after variable that can allow to access to different properties.</param>
    /// <param name="sheet">Worksheet that contains cell with marker to process.</param>
    /// <param name="migrantRange">Object that defines current cell, where value must be placed.</param>
    /// <param name="arrMarkerCells">List with all cells that contain template markers.</param>
    /// <param name="lstArguments">List with marker arguments sorted by priority.</param>
    /// <param name="options">Template marker options.</param>
    /// <param name="builder">Range builder that will get resulting range information.</param>
    private void SetUnknownVariable(object value, string strText, IWorksheet sheet,
      IMigrantRange migrantRange, IList<long> arrMarkerCells, IList lstArguments,
      MarkerOptionsImpl options, RangeBuilder builder, UnknownVariableAction action, VariableTypeAction variableTypeAction,
        string numberFormat,Type type)
    {
        // TODO: here we have to extract all arguments that can change behaviour of the algorithm.
#if !SILVERLIGHT && !WINRT && !WP
        if (IsDataView(value))
        {
            SetDataView((DataView)value, strText, sheet, migrantRange,
              arrMarkerCells, lstArguments, options, builder, action, variableTypeAction);
        }
        else
#endif
            if (IsArray(value))
            {

                    SetArrayValue((IList)value, strText, sheet, migrantRange, arrMarkerCells,
              lstArguments, options, builder, action, variableTypeAction);
            }
            else if (IsCollection(value))
            {

                SetCollectionValue((ICollection)value, strText, sheet, migrantRange,
                  arrMarkerCells, lstArguments, options, builder, action);
            }
#if !SILVERLIGHT && !WINRT && !WP
            else if (IsDataSet(value))
            {
                SetDataSetValue((DataSet)value, strText, sheet, migrantRange,
                  arrMarkerCells, lstArguments, options, builder, action, variableTypeAction);
            }
            else if (IsDataTable(value))
            {
                SetDataTable((DataTable)value, strText, sheet, migrantRange,
                  arrMarkerCells, lstArguments, options, builder, action, variableTypeAction);
            }
            else if (IsDataColumn(value))
            {
                SetDataColumn((DataColumn)value, strText, sheet, migrantRange,
                  arrMarkerCells, lstArguments, options, builder, action, variableTypeAction);
            }
#endif
            else
            {

                SetSimpleValue(value, strText, sheet, migrantRange, arrMarkerCells,
                  lstArguments, options, builder, action, numberFormat,type);
            }
    }


    /// <summary>
    /// Sets simple value.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="strText">Properties of the object to set cell value to.</param>
    /// <param name="sheet">Worksheet to set cell in.</param>
    /// <param name="migrantRange">Object that defines current cell, where value must be placed.</param>
    /// <param name="arrMarkerCells">List with all cells that contain template markers.</param>
    /// <param name="lstArguments">List with marker arguments sorted by priority.</param>
    /// <param name="options">Marker options.</param>
    /// <param name="builder">Range builder that will get resulting range information.</param>
    private void SetSimpleValue(object value, string strText, IWorksheet sheet,
      IMigrantRange migrantRange, IList<long> arrMarkerCells, IList lstArguments,
      MarkerOptionsImpl options, RangeBuilder builder, UnknownVariableAction action,
        string numberFormat,Type valueType)
    {
        if (sheet == null)
            throw new ArgumentNullException("sheet");

        if (builder == null)
            throw new ArgumentNullException("builder");
        VariableTypeAction variableTypeAction= VariableTypeAction.None;
        // TODO: setting value can be optimized after implementing API that will
        // allow to set cell values without IRange object.
        //string strValue = GetStringValue( value, strText );
        if (strText == null || strText.Length == 0)
        {
            //string strValue = ( value != null ) ? value.ToString() : null;
            if (valueType != null)
            {
                variableTypeAction = VariableTypeAction.DetectDataType;
                value = (value is string) ? GetValue(value as string, valueType) : value;
                migrantRange.Value2 = (value != null) ? value : DEF_NULL_VALUE;
                if (numberFormat != null)
                {
                    variableTypeAction= VariableTypeAction.DetectNumberFormat;
                    migrantRange.NumberFormat = numberFormat;
                }
            }
            else
            {
                string currentNumberFormat = migrantRange.NumberFormat;
                if (currentNumberFormat.Contains(RangeImpl.DEF_TEXT_FORMAT) && value is string)
                    migrantRange.Text = value.ToString ();
                else
                    migrantRange.Value2 = (value != null) ? value : DEF_NULL_VALUE;
                builder.Add(migrantRange.Row, migrantRange.Column);
            }
        }
        else
        {
            string newNumberFormat = null;
            Type newType = null;

            value = GetNextValue(value, ref strText,out newNumberFormat,out newType);
            SetUnknownVariable(value, strText, sheet, migrantRange, arrMarkerCells,
              lstArguments, options, builder, action, variableTypeAction, newNumberFormat, newType);
        }
    }
    /// <summary>
    /// Detects and get the value type of the each column.
    /// </summary>
    /// <param name="values">values of the data columns.</param>
    /// <param name="numberFormats">detected number formats.</param>
    /// <param name="valueTypes">Detected values Types.</param>
    /// <param name="variableTypeAction">Format Type.</param>
    public void GetColumnType(string[] values, out List<string> numberFormats, out List<Type> valueTypes,VariableTypeAction variableTypeAction)
    {
        numberFormats = new List<string>();
        valueTypes = new List<Type>();

        if (variableTypeAction == VariableTypeAction.None)
            return;

        string numberFormat = null;
        Type valueType = null;
        bool detectNumberFormat = (variableTypeAction == VariableTypeAction.DetectNumberFormat);
        
        foreach (string value in values)
        {
            GetValue(value, ref numberFormat, ref valueType,detectNumberFormat);
            if (detectNumberFormat)
                numberFormats.Add(numberFormat);
            valueTypes.Add(valueType);
        }

    }
    /// <summary>
    /// Get the value based on the valueType
    /// </summary>
    /// <param name="value">string value.</param>
    /// <param name="valueType">Type of the given value.</param>
    /// <returns>value as object.</returns>
    public object GetValue(string value, Type valueType)
    {
        string strDuplicate=value;
        int index = value.IndexOf(FormatsCollection.Percentage);
        if (index!= -1)
            strDuplicate = value.Remove(index);
        
        index = value.IndexOf(FormatsCollection.Fraction);
        if (index != -1)
        {
            string[] values = value.Split(FormatsCollection.Fraction[0]);
            if (values.Length == 2)
            {
                double first = 0, second = 0;
                bool bFirst = Double.TryParse(values[0], out first);
                bool bSecond = Double.TryParse(values[1], out second);
                if (bFirst && bSecond)
                    strDuplicate = (first / second).ToString();
                else
                    strDuplicate = value;
            }
        }
        if (valueType == typeof(int))
        {
            int iResult = 0;
            if (Int32.TryParse(strDuplicate, System.Globalization.NumberStyles.Any, null, out iResult))
            {
                return iResult;
            }
        }
        else if (valueType == typeof(double))
        {
            double dbValue = 0.0;
            if (Double.TryParse(strDuplicate, System.Globalization.NumberStyles.Any, null, out dbValue))
                return dbValue;
        }
        else if (valueType == typeof(DateTime))
        {
            DateTime dtValue = DateTime.Now;
            if (DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out dtValue))
                return dtValue;
        }
        else if (valueType == typeof(bool))
        {
            bool bValue = false;
            if (bool.TryParse(value, out bValue))
                return bValue;
        }
        return value;
    }
      /// <summary>
      /// Get value, value type and the number format.
      /// </summary>
      /// <param name="value">string value.</param>
      /// <param name="numberFormat">number format of the value.</param>
      /// <param name="valueType">Type of the value.</param>
      /// <param name="detectNumberFormat">Indicates, whether to detect Number format.</param>
      /// <returns>value as object.</returns>
    public object GetValue(string value, ref string numberFormat,ref Type valueType, bool detectNumberFormat)
    {
        string strDuplicate = value;
        DateTime dtValue = DateTime.Now;
        double dbValue = 0.0;
        bool bResult = false;
        int iResult = 0;
        if (value == null)
            return DEF_NULL_VALUE;
        int index = value.IndexOf(FormatsCollection.Percentage);
        if (index!= -1)
            strDuplicate = value.Remove(index);
        
        index = value.IndexOf(FormatsCollection.Fraction);
        if (index != -1)
        {
            string[] values = value.Split(FormatsCollection.Fraction[0]);
            if (values.Length == 2)
            {
                double first = 0, second = 0;
                bool bFirst = Double.TryParse(values[0], out first);
                bool bSecond = Double.TryParse(values[1], out second);
                if (bFirst && bSecond)
                    strDuplicate = (first / second).ToString();
                else
                    strDuplicate = value;
            }
        }
        if (Double.TryParse(strDuplicate, System.Globalization.NumberStyles.Any, null, out dbValue))
        {
            if (detectNumberFormat)
                numberFormat = m_book.InnerFormats.GetNumberFormat(value);
            valueType = typeof(double);
            return dbValue;
        }
        else if (Int32.TryParse(strDuplicate, NumberStyles.AllowThousands|NumberStyles.Currency| NumberStyles.AllowLeadingWhite, null, out iResult))
        {
            if(detectNumberFormat)
                numberFormat = m_book.InnerFormats.GetNumberFormat(value);
            valueType = typeof(int);
            return iResult;
        }
       
       
        else if (DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out dtValue))
        {
            if (detectNumberFormat)
            numberFormat = m_book.InnerFormats.GetDateFormat(value);
            valueType = typeof(DateTime);
            return dtValue;
        }
        else if (bool.TryParse(value, out bResult))
        {
            valueType = typeof(bool);
            return bResult;
        }
        return value;
    }
    /// <summary>
    /// Returns next value in the properties chain.
    /// </summary>
    /// <param name="value">First value in the chain.</param>
    /// <param name="strText">String representation of the properties chain.</param>
    /// <returns>Next value.</returns>
    private object GetNextValue( object value, ref string strText,out string newNumberFormat,out Type newType )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      if( strText == null )
        throw new ArgumentNullException( "strText" );

      if( strText.Length == 0 )
        throw new ArgumentException( "strText - string cannot be empty." );

      // Get property

      string strProperty = GetNextPropertyName( ref strText );

      // Get property value
      Type type = value.GetType();
      PropertyInfo prop = type.GetProperty( strProperty );

      if( prop == null )
      {
        throw new ArgumentOutOfRangeException( "strText", "Can't find property" );
      }

      value = prop.GetValue(value, null);
#if ( WINRT )
      object[] arrAtt = prop.GetCustomAttributes<TemplateMarkerAttributes>().ToArray<object>();
#else
        object[] arrAtt = prop.GetCustomAttributes(true);
#endif

      if (arrAtt != null && arrAtt.Length > 0)
      {
          newType = null;
          if(value!=null)
            newType = value.GetType();
          TemplateMarkerAttributes attributes =(TemplateMarkerAttributes) arrAtt[0];
          if ((newNumberFormat = attributes.NumberFormat) == null)
              newType = null;
      }
      else
      {
          newType = null;
          newNumberFormat = null;
      }
      return value;
    }
    /// <summary>
    /// Sets array value.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="strText">Text after variable that can allow to access to different properties.</param>
    /// <param name="sheet">Worksheet that contains cell with marker to process.</param>
    /// <param name="migrantRange">Object that defines current cell, where value must be placed.</param>
    /// <param name="arrMarkerCells">List with all cells that contain template markers.</param>
    /// <param name="lstArguments">List with marker arguments sorted by priority.</param>
    /// <param name="options">Template marker options.</param>
    /// <param name="builder">Range builder that will get resulting range information.</param>
    /// <param name="variableTypeAction">Indicates whether to preserver marker type and number format.</param>
    private void SetArrayValue( IList value, string strText, IWorksheet sheet,
      IMigrantRange migrantRange, IList<long> arrMarkerCells, IList lstArguments,
      MarkerOptionsImpl options, RangeBuilder builder, UnknownVariableAction action,VariableTypeAction variableTypeAction )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( value == null )
        throw new ArgumentNullException( "value" );

      int iCount = value.Count;
      m_insertCount = value.Count;
      string numberFormat = null;
      Type valueType = null;
      if( iCount == 0 ) return;

      List<string> numberFormats = new List<string>();
      List<Type> valueTypes = new List<Type>();

      if (value[0] is string)
      {

          string[] items = new string[iCount];
          value.CopyTo(items, 0);

          GetColumnType(items, out numberFormats, out valueTypes, variableTypeAction);

          if (valueTypes.Count > 0)
          {
              valueType = valueTypes[0];
              if (numberFormats.Count > 0)
                  numberFormat = numberFormats[0];
          }
      }

      object item = value[ 0 ];
      SetSimpleValue( item, strText, sheet, migrantRange, arrMarkerCells,
        lstArguments, options, builder, action,numberFormat,valueType );

      numberFormat = null;
        valueType = null;
      for( int i = 1; i < iCount; i++ )
      {
        if( !PrepareNextCell( sheet, migrantRange, arrMarkerCells, lstArguments, options ) )
          break;
        if (valueTypes.Count > 0)
        {
            valueType = valueTypes[i];
            if (numberFormats.Count > 0)
                numberFormat = numberFormats[i];
        }
        item = value[ i ];
        SetSimpleValue( item, strText, sheet, migrantRange, arrMarkerCells,
          lstArguments, options, builder, action,numberFormat,valueType );
      }
    }
    /// <summary>
    /// Sets array value.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="strText">Text after variable that can allow to access to different properties.</param>
    /// <param name="sheet">Worksheet that contains cell with marker to process.</param>
    /// <param name="migrantRange">Object that defines current cell, where value must be placed.</param>
    /// <param name="arrMarkerCells">List with all cells that contain template markers.</param>
    /// <param name="lstArguments">List with marker arguments sorted by priority.</param>
    /// <param name="options">Marker options.</param>
    /// <param name="builder">Range builder that will get resulting range information.</param>
    /// <param name="variableTypeAction">Indicates whether to preserver marker type and number format.</param>
    private void SetCollectionValue( ICollection value, string strText, IWorksheet sheet,
      IMigrantRange migrantRange, IList<long> arrMarkerCells, IList lstArguments,
      MarkerOptionsImpl options, RangeBuilder builder, UnknownVariableAction action )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( value == null )
        throw new ArgumentNullException( "value" );

      if( builder == null )
        throw new ArgumentNullException( "builder" );

      bool bFirst = true;

      m_insertCount = value.Count;
      //for( int i = 0, len = value.Count; i < len; i++ )
      foreach( object item in value )
      {
        if( !bFirst && !PrepareNextCell( sheet, migrantRange, arrMarkerCells, lstArguments, options ) )
          break;

        SetSimpleValue( item, strText,  sheet, migrantRange, arrMarkerCells,
          lstArguments, options, builder, action ,null,null);
        bFirst = false;
      }
    }
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Sets DataSet value.
    /// </summary>
    /// <param name="value">Value to import.</param>
    /// <param name="strText">Properties chain.</param>
    /// <param name="sheet">Worksheet that contains cell with marker.</param>
    /// <param name="migrantRange">Object that defines current cell, where value must be placed.</param>
    /// <param name="arrMarkerCells">List with all cells that contain template markers.</param>
    /// <param name="lstArguments">List with marker arguments sorted by priority.</param>
    /// <param name="options">Marker options.</param>
    /// <param name="builder">Range builder that will get resulting range information.</param>
    /// <param name="variableTypeAction">Indicates whether to preserver marker type and number format.</param>
    private void SetDataSetValue( DataSet value, string strText, IWorksheet sheet,
      IMigrantRange migrantRange, IList<long> arrMarkerCells, IList lstArguments,
      MarkerOptionsImpl options, RangeBuilder builder, UnknownVariableAction action,VariableTypeAction variableTypeAction )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( value == null )
        throw new ArgumentNullException( "value" );

      DataTable table;

      if( strText == null || strText.Length == 0 )
      {
        if( value.Tables.Count == 1 )
        {
          SetDataTable( value.Tables[ 0 ], strText, sheet, migrantRange,
            arrMarkerCells, lstArguments, options, builder, action,variableTypeAction );
        }
        else
        {
          throw new ArgumentException( "Can't import DataSet" );
        }
      }
      else
      {
        string strProperty = PeekNextPropertyName( strText );
        PropertyInfo prop;
        int iPropLen = strProperty.Length;
        int iTextLen = strText.Length;

        if( IsProperty( value, strProperty, out prop ) )
        {
          object item = prop.GetValue( value, null );

          strText = ( iTextLen > iPropLen ) ? strText.Substring( iPropLen + 1 ) : string.Empty;
          SetUnknownVariable( item, strText, sheet, migrantRange, arrMarkerCells,
            lstArguments, options, builder, action ,variableTypeAction);
        }
        else if( ( table = value.Tables[ strProperty ] ) != null )
        {
          strText = ( iTextLen > iPropLen ) ? strText.Substring( iPropLen + 1 ) : string.Empty;
          SetDataTable( table, strText, sheet, migrantRange, arrMarkerCells,
            lstArguments, options, builder, action ,variableTypeAction);
        }
        else
        {
          UnknownProperty( strProperty, action, strText, sheet, migrantRange, arrMarkerCells,
            lstArguments, options, builder,variableTypeAction );
        }
      }
    }
    /// <summary> 
    /// Imports DataView into marker position. 
    /// </summary> 
    /// <param name="value">Value to import.</param> 
    /// <param name="strText">Text with remaining properties chain.</param> 
    /// <param name="sheet">Worksheet to import data table into.</param> 
    /// <param name="migrantRange">Object that defines current cell, where value must be placed.</param> 
    /// <param name="arrMarkerCells">List with all cells that contain template markers.</param> 
    /// <param name="lstArguments">List with marker arguments sorted by priority.</param> 
    /// <param name="options">Marker options.</param> 
    /// <param name="builder">Range builder that will get resulting range information.</param>
    /// <param name="variableTypeAction">Indicates whether to preserver marker type and number format.</param>
    private void SetDataView( DataView value, string strText, IWorksheet sheet,
      IMigrantRange migrantRange, IList<long> arrMarkerCells, IList lstArguments,
      MarkerOptionsImpl options, RangeBuilder builder, UnknownVariableAction action ,VariableTypeAction variableTypeAction)
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( value == null )
        throw new ArgumentNullException( "value" );

      DataColumn column;

      if( strText != null && strText.Length > 0 )
      {
        string strProperty = PeekNextPropertyName( strText );
        PropertyInfo prop;
        int iPropLen = strProperty.Length;
        int iTextLen = strText.Length;

        if( IsProperty( value, strProperty, out prop ) )
        {
          object item = prop.GetValue( value, null );

          strText = ( iTextLen > iPropLen ) ? strText.Substring( iPropLen + 1 ) : string.Empty;
          SetUnknownVariable( item, strText, sheet, migrantRange, arrMarkerCells,
            lstArguments, options, builder, action ,variableTypeAction);
        }
        else if( ( column = value.ToTable().Columns[ strProperty ] ) != null )
        {
          strText = ( iTextLen > iPropLen ) ? strText.Substring( iPropLen + 1 ) : string.Empty;
          SetDataColumn( column, strText, sheet, migrantRange, arrMarkerCells,
            lstArguments, options, builder, action, variableTypeAction );
        }
        else
        {
          UnknownProperty( strProperty, action, strText, sheet, migrantRange,
            arrMarkerCells, lstArguments, options, builder,variableTypeAction );
        }
      }
      else
      {
        throw new NotImplementedException();
      }
    } 
    /// <summary>
    /// Imports DataTable into marker position.
    /// </summary>
    /// <param name="value">Value to import.</param>
    /// <param name="strText">Text with remaining properties chain.</param>
    /// <param name="sheet">Worksheet to import data table into.</param>
    /// <param name="migrantRange">Object that defines current cell, where value must be placed.</param>
    /// <param name="arrMarkerCells">List with all cells that contain template markers.</param>
    /// <param name="lstArguments">List with marker arguments sorted by priority.</param>
    /// <param name="options">Marker options.</param>
    /// <param name="builder">Range builder that will get resulting range information.</param>
    /// <param name="variableTypeAction">Indicates whether to preserver marker type and number format.</param>
    private void SetDataTable( DataTable value, string strText, IWorksheet sheet,
      IMigrantRange migrantRange, IList<long> arrMarkerCells, IList lstArguments,
      MarkerOptionsImpl options, RangeBuilder builder, UnknownVariableAction action,VariableTypeAction variableTypeAction )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( value == null )
        throw new ArgumentNullException( "value" );

      DataColumn column;
      m_insertCount = value.Rows.Count;
      if( strText != null && strText.Length > 0 )
      {
        string strProperty = PeekNextPropertyName( strText );
        PropertyInfo prop;
        int iPropLen = strProperty.Length;
        int iTextLen = strText.Length;

        if( IsProperty( value, strProperty, out prop ) )
        {
          object item = prop.GetValue( value, null );

          strText = ( iTextLen > iPropLen ) ? strText.Substring( iPropLen + 1 ) : string.Empty;
          SetUnknownVariable( item, strText, sheet, migrantRange, arrMarkerCells,
            lstArguments, options, builder, action, variableTypeAction);
        }
        else if( ( column = value.Columns[ strProperty ] ) != null )
        {
          strText = ( iTextLen > iPropLen ) ? strText.Substring( iPropLen + 1 ) : string.Empty;
          SetDataColumn( column, strText, sheet, migrantRange, arrMarkerCells,
            lstArguments, options, builder, action, variableTypeAction);
        }
        else
        {
            UnknownProperty(strProperty, action, strText, sheet, migrantRange, arrMarkerCells, lstArguments, options, builder, variableTypeAction);
        }
      }
      else
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Processes unknown property based on user-defined action.
    /// </summary>
    /// <param name="strProperty">Property name.</param>
    /// <param name="action">Defined action.</param>
    /// <param name="strText">Text with remaining properties chain.</param>
    /// <param name="sheet">Worksheet to import data table into.</param>
    /// <param name="migrantRange">Object that defines current cell, where value must be placed.</param>
    /// <param name="arrMarkerCells">List with all cells that contain template markers.</param>
    /// <param name="lstArguments">List with marker arguments sorted by priority.</param>
    /// <param name="options">Marker options.</param>
    /// <param name="builder">Range builder that will get resulting range information.</param>
    /// <param name="variableTypeAction">Indicates whether to preserver marker type and number format.</param>
    private void UnknownProperty( string strProperty, UnknownVariableAction action,
      string strText, IWorksheet sheet, IMigrantRange migrantRange, IList<long> arrMarkerCells,
      IList lstArguments, MarkerOptionsImpl options, RangeBuilder builder,VariableTypeAction variableTypeAction )
    {
      switch( action )
      {
        case UnknownVariableAction.Exception:
          throw new ApplicationException( "Variable " + strProperty + " not found" );

        case UnknownVariableAction.ReplaceBlank:
          SetSimpleValue( string.Empty, strText, sheet, migrantRange, arrMarkerCells, lstArguments, options, builder, action ,null,null);
          break;

        case UnknownVariableAction.Skip:
          migrantRange.Text = options.OriginalMarker;
          break;

        default:
          throw new ApplicationException();
      }
    }
    /// <summary>
    /// Imports DataColumn into marker position.
    /// </summary>
    /// <param name="value">Value to import.</param>
    /// <param name="strText">Text with remaining properties chain.</param>
    /// <param name="sheet">Worksheet to import data table into.</param>
    /// <param name="migrantRange">Object that defines current cell, where value must be placed.</param>
    /// <param name="arrMarkerCells">List with all cells that contain template markers.</param>
    /// <param name="lstArguments">List with marker arguments sorted by priority.</param>
    /// <param name="options">Marker options.</param>
    /// <param name="builder">Range builder that will get resulting range information.</param>
    /// <param name="variableTypeAction">Indicates whether to preserver marker type and number format.</param>
    private void SetDataColumn( DataColumn value, string strText, IWorksheet sheet,
      IMigrantRange migrantRange, IList<long> arrMarkerCells, IList lstArguments,
      MarkerOptionsImpl options, RangeBuilder builder, UnknownVariableAction action,VariableTypeAction variableTypeAction )
    {
      DataTable table = value.Table;
      DataRowCollection arrRows = table.Rows;
      string numberFormat = null;
      Type valueType = null;

      if ( variableTypeAction== VariableTypeAction.DetectNumberFormat ||variableTypeAction == VariableTypeAction.DetectDataType )
      {
          bool detectNumberFormat = variableTypeAction == VariableTypeAction.DetectNumberFormat;
          object oValue = arrRows[0][value];
          if (arrRows[0][value].GetType() == typeof (DBNull) )
          {
              bool endLoop = false;
              for (int index = 0; index < table.Rows.Count; index++)
              {
                  if ((arrRows[index][value].ToString().Length > 0) && !endLoop )
                  {
                      endLoop = true;
                      oValue = arrRows[index][value];
                  }
              }
          }
          object strValue = (oValue is string) ? GetValue(oValue as string, ref numberFormat, ref valueType, detectNumberFormat) : oValue;
      }
      m_insertCount = arrRows.Count;
      
      for( int i = 0, len = arrRows.Count; i < len; i++ )
      {
        if( i > 0 && !PrepareNextCell( sheet, migrantRange, arrMarkerCells, lstArguments, options ) )
          break;

        DataRow row = arrRows[ i ];
        object item = row[ value ];
        SetSimpleValue( item, strText, sheet, migrantRange, arrMarkerCells,
          lstArguments, options, builder, action,numberFormat,valueType );
      }
    }
#endif
    /// <summary>
    /// Prepares next cell for markers import.
    /// </summary>
    /// <param name="sheet">Worksheet to import markers into.</param>
    /// <param name="migrantRange">Object that defines current cell, where value must be placed.</param>
    /// <param name="arrMarkerCells">List with all marker cells.</param>
    /// <param name="lstArguments">List with marker arguments sorted by priority.</param>
    /// <param name="options">Marker options.</param>
    /// <returns>True if next cell was prepared; false if it weren't and we need to stop importing values.</returns>
    private bool PrepareNextCell( IWorksheet sheet, IMigrantRange migrantRange,
      IList<long> arrMarkerCells, IList lstArguments, MarkerOptionsImpl options )
    {
      int iRow = migrantRange.Row;
      int iColumn = migrantRange.Column;
      ApplyArguments( sheet, ref iRow, ref iColumn, arrMarkerCells, lstArguments, options );

      bool bResult = ( iRow != 0 && iColumn != 0 );

      if( bResult )
        migrantRange.ResetRowColumn( iRow, iColumn );

      return bResult;
    }
    /// <summary>
    /// Indicates whether object is array.
    /// </summary>
    /// <param name="value">Value to check.</param>
    /// <returns>True if object is array, False otherwise.</returns>
    private bool IsArray( object value )
    {
      if( value == null ) return false;

      return ( value is IList );
    }
    /// <summary>
    /// Indicates whether object is collection.
    /// </summary>
    /// <param name="value">Value to check.</param>
    /// <returns>True if object is collection, False otherwise.</returns>
    private bool IsCollection( object value )
    {
      if( value == null ) return false;

      return ( value is ICollection );
    }
    /// <summary>
    /// Indicates whether object is DataSet.
    /// </summary>
    /// <param name="value">Value to check.</param>
    /// <returns>True if object is collection, False otherwise.</returns>
    private bool IsDataSet( object value )
    {
#if !SILVERLIGHT && !WINRT && !WP
      if( value == null ) return false;

      return ( value is DataSet );
#else
      return false;
#endif
    }
    /// <summary>
    /// Indicates whether object is DataTable.
    /// </summary>
    /// <param name="value">Value to check.</param>
    /// <returns>True if object is collection, False otherwise.</returns>
    private bool IsDataTable( object value )
    {
#if !SILVERLIGHT && !WINRT && !WP
      if( value == null ) return false;

      return ( value is DataTable );
#else
      return false;
#endif
    }
    /// </summary> 
    /// <param name="value">Value to check.</param> 
    /// <returns>True if object is collection, False otherwise.</returns> 
    private bool IsDataView( object value )
    {
#if !SILVERLIGHT && !WINRT && !WP
      return ( value as DataView ) != null;
#else 
        return false; 
#endif
    } 
    /// <summary>
    /// Indicates whether object is DataTable.
    /// </summary>
    /// <param name="value">Value to check.</param>
    /// <returns>True if object is collection, False otherwise.</returns>
    private bool IsDataColumn( object value )
    {
#if !SILVERLIGHT && !WINRT && !WP
      if( value == null ) return false;

      return ( value is DataColumn );
#else
      return false;
#endif
    }
    /// <summary>
    /// Indicates whether specified object contains property with specified name.
    /// </summary>
    /// <param name="value">Object to search property in.</param>
    /// <param name="strPropName">Property name to search.</param>
    /// <param name="prop">Output property information, null if no property was found.</param>
    /// <returns>True if object contains such property.</returns>
    private bool IsProperty( object value, string strPropName, out PropertyInfo prop )
    {
      prop = null;

      if( value == null ) return false;

      if( strPropName == null || strPropName.Length == 0 )
        throw new ArgumentException( "strPropName - string cannot be empty." );

      Type type = value.GetType();
      prop = type.GetProperty( strPropName );

      return ( prop != null );
    }
    /// <summary>
    /// Gets name of the next property and updates original text removing that property.
    /// </summary>
    /// <param name="strText">Text that contains property chain.</param>
    /// <returns>Next property in the chain.</returns>
    private string GetNextPropertyName( ref string strText )
    {
      if( strText == null )
        throw new ArgumentNullException( "strText" );

      if( strText.Length == 0 )
        throw new ArgumentException( "strText - string cannot be empty." );

      int iDotIndex = strText.IndexOf( DEF_PARTS_SEPARATOR );
      string strProperty;

      if( iDotIndex < 0 )
      {
        strProperty = strText;
        strText = string.Empty;
      }
      else
      {
        strProperty = strText.Substring( 0, iDotIndex );
        strText = strText.Substring( iDotIndex + 1 );
      }

      return strProperty;
    }
    /// <summary>
    /// Peeks name of the next property without updating original text.
    /// </summary>
    /// <param name="strText">Text that contains property chain.</param>
    /// <returns>Next property in the chain.</returns>
    private string PeekNextPropertyName( string strText )
    {
      if( strText == null )
        throw new ArgumentNullException( "strText" );

      if( strText.Length == 0 )
        throw new ArgumentException( "strText - string cannot be empty." );

      int iDotIndex = strText.IndexOf( DEF_PARTS_SEPARATOR );
      string strProperty;

      strProperty = ( iDotIndex < 0 )
        ? strText
        : strText.Substring( 0, iDotIndex );

      return strProperty;
    }
    /// <summary>
    /// Parses arguments.
    /// </summary>
    /// <param name="strArguments">Arguments string.</param>
    /// <param name="options">Prepared marker options.</param>
    /// <returns>List with arguments that should be applied during next cell preparation.</returns>
    private IList ParseArguments( string strArguments, out MarkerOptionsImpl options )
    {
      options = new MarkerOptionsImpl( m_book );
      SortedList<int, List<MarkerArgument>> result = null;

      if( strArguments != null && strArguments.Length != 0 )
      {
        string[] arrArguments = strArguments.Split( m_chSeparator );
        int iCount = arrArguments.Length;
        result = new SortedList<int,List<MarkerArgument>>( iCount );

        for( int i = 0; i < iCount; i++ )
        {
          string strArgument = arrArguments[ i ];
          MarkerArgument argument = ParseArgument( strArgument );

          if( argument == null )
            throw new ArgumentOutOfRangeException( "strArgument", "Unknown argument" );

          if( argument.IsPreparing )
          {
            argument.PrepareOptions( options );
          }

          if( argument.IsApplyable )
          {
            int iPriority = argument.Priority;

            //if( argument.IsAllowMultiple )
            {
              List<MarkerArgument> list;

              if( !result.TryGetValue( argument.Priority, out list ) )
              {
                list = new List<MarkerArgument>();
                result.Add( iPriority, list );
              }

              list.Add( argument );
            }
            //else
            //{
            //  result.Add( iPriority, argument );
            //}
          }
        }
      }

      return ConvertToList( result, options );
    }
    /// <summary>
    /// Converts sorted list with template marker arguments into IList.
    /// </summary>
    /// <param name="lstArguments">Sorted list to convert.</param>
    /// <param name="options">Template marker options.</param>
    /// <returns>Converted IList with all arguments.</returns>
    private IList ConvertToList( SortedList<int, List<MarkerArgument>> lstArguments, MarkerOptionsImpl options )
    {
      List<MarkerArgument> arrResult = new List<MarkerArgument>();

      if( lstArguments == null )
      {
        lstArguments = new SortedList<int,List<MarkerArgument>>( 1 );
      }

      InsertJumpAttribute( lstArguments, options );
      IList<List<MarkerArgument>> values = lstArguments.Values;

      for( int i = 0, len = lstArguments.Count; i < len; i++ )
      {
        List<MarkerArgument> value = values[ i ];
        arrResult.AddRange( value );
      }

      return arrResult;
    }
    /// <summary>
    /// Inserts default jump attribute if it is necessary.
    /// </summary>
    /// <param name="lstMarkers">Sorted list with arguments. Key - argument priority, value - argument object.</param>
    /// <param name="options">Prepared marker options.</param>
    private void InsertJumpAttribute( SortedList<int, List<MarkerArgument>> lstMarkers, MarkerOptionsImpl options )
    {
      if( lstMarkers == null )
        throw new ArgumentNullException( "lstMarkers" );

      if( options == null )
        throw new ArgumentNullException( "options" );

      int iRow;
      int iColumn;

      if( options.Direction == MarkerDirection.Horizontal )
      {
        iRow = 0;
        iColumn = 1;
      }
      else
      {
        iRow = 1;
        iColumn = 0;
      }

      JumpArgument jump = new JumpArgument( iRow, iColumn, true, true );

      if( !lstMarkers.ContainsKey( jump.Priority ) )
      {
        List<MarkerArgument> jumpArray = new List<MarkerArgument>( 1 );
        jumpArray.Add( jump );
        lstMarkers.Add( jump.Priority, jumpArray );
      }
    }
    /// <summary>
    /// Parses single marker argument.
    /// </summary>
    /// <param name="strArgument">Argument to parse.</param>
    /// <returns>Newly created marker argument.</returns>
    private MarkerArgument ParseArgument( string strArgument )
    {
      if( strArgument == null )
        throw new ArgumentNullException( "strArgument" );

      if( strArgument.Length == 0 )
        throw new ArgumentException( "strArgument - string cannot be empty." );

      for( int i = 0, len = s_arrArguments.Count; i < len; i++ )
      {
        MarkerArgument argument = s_arrArguments[ i ];
        MarkerArgument result = argument.TryParse( strArgument );

        if( result != null )
          return result;
      }

      return null;
    }
    /// <summary>
    /// Applies template marker arguments.
    /// </summary>
    /// <param name="sheet">Worksheet to import markers into.</param>
    /// <param name="iRow">One-based row index of the last filled cell.</param>
    /// <param name="iColumn">One-based column index of the last filled cell.</param>
    /// <param name="arrMarkerCells">List with all marker cells.</param>
    /// <param name="lstArguments">List with arguments.</param>
    /// <param name="options">Marker options.</param>
    private void ApplyArguments( IWorksheet sheet, ref int iRow, ref int iColumn,
      IList<long> arrMarkerCells, IList lstArguments, MarkerOptionsImpl options )
    {
      if( lstArguments == null )
        throw new ArgumentNullException( "lstArguments" );

      int iCount = lstArguments.Count;

      if( iCount == 0 ) return;

      Point point = new Point(iRow, iColumn);

      for( int i = 0; i < iCount; i++ )
      {
        MarkerArgument argument = ( MarkerArgument )lstArguments[ i ];
        argument.ApplyArgument( sheet, point, ref iRow, ref iColumn, arrMarkerCells, options ,m_insertCount );
        
      }
      m_insertCount = 0;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets or sets marker prefix. String that indicates that cell contains marker.
    /// </summary>
    public string MarkerPrefix
    {
      get
      {
        return m_strMarkerPrefix;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        if( value.Length == 0 )
          throw new ArgumentException( "value - string cannot be empty." );

        m_strMarkerPrefix = value;
      }
    }
    /// <summary>
    /// Gets or sets arguments separator.
    /// </summary>
    public char ArgumentSeparator
    {
      get
      {
        return m_chSeparator;
      }
      set
      {
        m_chSeparator = value;
      }
    }
    #endregion
  }
}
