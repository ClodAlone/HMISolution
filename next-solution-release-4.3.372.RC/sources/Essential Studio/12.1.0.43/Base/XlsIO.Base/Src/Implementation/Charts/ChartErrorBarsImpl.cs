#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using TErrorBarType = Syncfusion.XlsIO.Parser.Biff_Records.Charts.ChartSerAuxErrBarRecord.TErrorBarValue;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Represents chart error bars.
  /// </summary>
  public class ChartErrorBarsImpl
    : CommonObject
    , IChartErrorBars
  {
    #region Class constants
    /// <summary>
    /// Represents default value for X error bar.
    /// </summary>
    public const int DEF_NUMBER_X_VALUE = 1;
    /// <summary>
    /// Represents default value for Y axis.
    /// </summary>
    public const int DEF_NUMBER_Y_VALUE = 10;
    #endregion

    #region Class static methods
    /// <summary>
    /// Serialize Series record.
    /// </summary>
    /// <param name="records">Represents record holder.</param>
    /// <param name="count">Values number.</param>
    public static void  SerializeSerieRecord( IList<IBiffStorage> records, int count )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      ChartSeriesRecord serie = ( ChartSeriesRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartSeries );

      serie.BubbleDataType = ChartSeriesRecord.DataType.Numeric;
      serie.StdX = ChartSeriesRecord.DataType.Numeric;
      serie.StdY = ChartSeriesRecord.DataType.Numeric;

      serie.ValuesCount = ( ushort )count;

      records.Add( serie );
    }
    /// <summary>
    /// Serialize data format records.
    /// </summary>
    /// <param name="records">Represents record holder.</param>
    /// <param name="border">Represents border object.</param>
    /// <param name="iSerieIndex">Represents Series index.</param>
    /// <param name="iIndex">Represents Series indexes.</param>
    public static void SerializeDataFormatRecords( IList<IBiffStorage> records, ChartBorderImpl border
      , int iSerieIndex, int iIndex, ChartMarkerFormatRecord marker )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( border == null )
        throw new ArgumentNullException( "border" );

      ChartDataFormatRecord data = ( ChartDataFormatRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartDataFormat );

      data.PointNumber = ChartSerieImpl.DEF_FORMAT_ALLPOINTS_INDEX;
      data.SeriesIndex = ( ushort )iIndex;
      data.SeriesNumber = ( ushort )iSerieIndex;
      records.Add( data );

      BiffRecordRaw record = BiffRecordFactory.GetRecord( TBIFFRecord.Begin );
      records.Add( record );

      record = BiffRecordFactory.GetRecord( TBIFFRecord.Chart3DDataFormat );
      records.Add( record );

      border.Serialize( records );

      record = BiffRecordFactory.GetRecord( TBIFFRecord.ChartAreaFormat );
      records.Add( record );

      record = BiffRecordFactory.GetRecord( TBIFFRecord.ChartPieFormat );
      records.Add( record );

      record = ( marker == null ) ?
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartMarkerFormat ) :
        marker;

      records.Add( record );

      record = BiffRecordFactory.GetRecord( TBIFFRecord.End );
      records.Add( record );
    }
    #endregion

    #region Class members
    /// <summary>
    /// Represents border object.
    /// </summary>
    private ChartBorderImpl m_border;
    /// <summary>
    /// Represents error bar record.
    /// </summary>
    private ChartSerAuxErrBarRecord m_errorBarRecord;
    /// <summary>
    /// Represents Shadow
    /// </summary>
    private ShadowImpl m_shadow;
    /// <summary>
    /// Represents error include.
    /// </summary>
    private ExcelErrorBarInclude m_include;
    /// <summary>
    /// Represents parent Series.
    /// </summary>
    private ChartSerieImpl m_serie;
    /// <summary>
    /// Represents positive range of custom values.
    /// </summary>
    private IRange m_plusRange;
    /// <summary>
    /// Represents negative range of custom values.
    /// </summary>
    private IRange m_minusRange;
    /// <summary>
    /// Indicates that is on Y axis.
    /// </summary>
    private bool m_bIsY;
    /// <summary>
    /// ChartAI record containing referred range.
    /// </summary>
    private ChartAIRecord m_chartAi = ( ChartAIRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartAI );
    /// <summary>
    /// Preserved marker formta record
    /// </summary>
    private ChartMarkerFormatRecord m_markerFormat;
    private bool m_bChanged;
        /// <summary>
        /// Represents the 3D features
        /// </summary>
        private ThreeDFormatImpl m_3D;
    /// <summary>
    /// Represents the chart ErrorBar plus range values
    /// </summary>
    private object[] m_plusRangeValues;
    /// <summary>
    /// Represents the chart ErrorBar minus range values
    /// </summary>
    private object[] m_minusRangeValues;
    private bool m_isPlusNumberLiteral;
    private bool m_isMinusNumberLiteral;
    private string m_formatCode;
    #endregion

    #region Class initialize methods
    /// <summary>
    /// Creates new instance of error bars implementation.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Represents parent object.</param>
    /// <param name="bIsY">Indicates if it's Y axis bar.</param>
    public ChartErrorBarsImpl( IApplication application, object parent, bool bIsY )
      : base( application, parent )
    {
      m_border = new ChartBorderImpl( application, this );
      m_bIsY = bIsY;

      m_errorBarRecord = ( ChartSerAuxErrBarRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartSerAuxErrBar );

      if( !bIsY )
        NumberValue = DEF_NUMBER_X_VALUE;

      FindParents();
    }
    /// <summary>
    /// Parses new instance of error bars from stream.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Represents parent object.</param>
    /// <param name="data">Represents data holder.</param>
    public ChartErrorBarsImpl( IApplication application, object parent, IList<BiffRecordRaw> data )
      : base( application, parent )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      FindParents();
      Parse( data );
    }
    /// <summary>
    /// Finds parent objects.
    /// </summary>
    private void FindParents()
    {
      m_serie = ( ChartSerieImpl )FindParent( typeof( ChartSerieImpl ) );

      if( m_serie == null )
        throw new NotSupportedException( "Cannot find parent objects" );
    }
    #endregion

    #region IChartErrorBars properties
    /// <summary>
    /// Represents border object. Read-only.
    /// </summary>
    public IChartBorder Border
    {
      get
      {
        return m_border;
      }
    }
    /// <summary>
    /// Represents error bar include type.
    /// </summary>
    public ExcelErrorBarInclude Include
    {
      get
      {
        return m_include;
      }
      set
      {
        if( value != Include )
        {
          if( !m_serie.ParentBook.Loading && !CheckInclude( value ) )
            throw new NotSupportedException( "Cannot change include value, before set range values." );

          m_include = value;
        }
      }
    }
    /// <summary>
    /// Indicates if error bar has cap.
    /// </summary>
    public bool HasCap
    {
      get
      {
        return m_errorBarRecord.TeeTop;
      }
      set
      {
        m_errorBarRecord.TeeTop = value;
      }
    }
    /// <summary>
    /// Represents excel error bar type.
    /// </summary>
    public ExcelErrorBarType Type
    {
      get
      {
        return m_errorBarRecord.ErrorBarType;
      }
      set
      {
        m_errorBarRecord.ErrorBarType = value;
      }
    }
    /// <summary>
    /// Represents number value.
    /// </summary>
    public double NumberValue
    {
      get
      {
        return m_errorBarRecord.NumValue;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "NumberValue" );

        m_errorBarRecord.NumValue = value;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is plus number literal.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is plus number literal; otherwise, <c>false</c>.
    /// </value>
    internal bool IsPlusNumberLiteral
    {
        get
        {
            return m_isPlusNumberLiteral;
        }
        set
        {
            m_isPlusNumberLiteral = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is minus number literal.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is minus number literal; otherwise, <c>false</c>.
    /// </value>
    internal bool IsMinusNumberLiteral
    {
        get
        {
            return m_isMinusNumberLiteral;
        }
        set
        {
            m_isMinusNumberLiteral = value;
        }

    }

    /// <summary>
    /// Represents custom positive value.
    /// </summary>
    public IRange PlusRange
    {
      get
      {
        return m_plusRange;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "PlusRange" );

        m_include = ( m_minusRange == null )
          ? ExcelErrorBarInclude.Plus
          : ExcelErrorBarInclude.Both;

        m_errorBarRecord.ErrorBarType = ExcelErrorBarType.Custom;

        m_plusRange = value;

        if( !m_serie.ParentBook.Loading )
        {
          m_chartAi.ParsedExpression = ( value as INativePTG ).GetNativePtg();
          m_bChanged = true;
        }
      }
    }
    /// <summary>
    /// Represents custom negative value.
    /// </summary>
    public IRange MinusRange
    {
      get
      {
        return m_minusRange;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "PlusRange" );

        m_include = ( m_plusRange == null )
          ? ExcelErrorBarInclude.Minus
          : ExcelErrorBarInclude.Both;

        m_errorBarRecord.ErrorBarType = ExcelErrorBarType.Custom;

        m_minusRange = value;

        if( !m_serie.ParentBook.Loading )
        {
          m_chartAi.ParsedExpression = ( value as INativePTG ).GetNativePtg();
          m_bChanged = true;
        }
      }
    }
        /// <summary>
        /// Represents the Shadow.Read-only
        /// </summary>
        public IShadow Shadow
        {
            get
            {
                if (m_shadow == null)
                    m_shadow = new ShadowImpl(Application, this);

                return m_shadow;
            }
        }
        /// <summary>
        /// This property indicates whether the shadow object has been created 
        /// </summary>
        public bool HasShadowProperties
        {
            get
            {
                return m_shadow != null;
            }
            internal set
            {
                if (value)
                {
                    IShadow shadow = Shadow;
                }
                else
                {
                    m_shadow = null;
                }
            }
        }
        /// <summary>
        /// Gets the chart3 D options.
        /// </summary>
        /// <value>The chart3 D options.</value>
        public IThreeDFormat Chart3DOptions
        {
            get
            {
                if (m_3D == null)
                    m_3D = new ThreeDFormatImpl(Application, this);

                return m_3D;
            }
        }
     
        /// <summary>
        /// This property Indicates whether the Shadow object has been created(which includes the 3D properties)
        /// </summary>
        public bool Has3dProperties
        {
            get
            {
                return m_3D != null;
            }
            internal set
            {
                if (value)
                {
                    IThreeDFormat Threed = Chart3DOptions;
                }
                else
                {
                    m_3D = null;
                }
      }
    }
    #endregion

    #region IChartErrorBars methods
    /// <summary>
    /// Clears current error bar.
    /// </summary>
    public void ClearFormats()
    {
      Type = ExcelErrorBarType.Fixed;
      Border.AutoFormat = true;
      HasCap = true;
      NumberValue = ( m_bIsY  ) ? DEF_NUMBER_Y_VALUE : DEF_NUMBER_X_VALUE;

      m_include = ExcelErrorBarInclude.Both;
      m_plusRange = null;
      m_minusRange = null;
    }
    /// <summary>
    /// Deletes current error bar.
    /// </summary>
    public void Delete()
    {
      if( m_bIsY )
      {
        m_serie.HasErrorBarsY = false;
      }
      else
      {
        m_serie.HasErrorBarsX = false;
      }
    }
    #endregion

    #region Class parse methods
    /// <summary>
    /// Parses error bars object from stream.
    /// </summary>
    /// <param name="data">Represents record holder.</param>
    private void Parse( IList<BiffRecordRaw> data )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      IRange range = null;

      for( int i = 0, iLen = data.Count; i < iLen; i++ )
      {
        BiffRecordRaw record = ( BiffRecordRaw )data[ i ];

        switch( record.TypeCode )
        {
          case TBIFFRecord.ChartSerAuxErrBar:
            m_errorBarRecord = ( ChartSerAuxErrBarRecord )record;
            break;

          case TBIFFRecord.ChartLineFormat:
            m_border = new ChartBorderImpl( Application, this, ( ChartLineFormatRecord )record );
            break;

          case TBIFFRecord.ChartAI:
            ChartAIRecord chartAi = ( ChartAIRecord )record;

            if( chartAi.IndexIdentifier == ChartAIRecord.LinkIndex.LinkToValues )
            {
              m_chartAi = chartAi;

              if( m_chartAi.Reference == ChartAIRecord.ReferenceType.Worksheet )
                range = m_serie.GetRange( m_chartAi );
            }
            break;

          case TBIFFRecord.ChartMarkerFormat:
            m_markerFormat = ( ChartMarkerFormatRecord )record;
            break;
        }
      }

      TErrorBarType type = m_errorBarRecord.ErrorBarValue;
      bool bIsPlus = type == TErrorBarType.XDirectionPlus || type == TErrorBarType.YDirectionPlus;

      m_bIsY = type == TErrorBarType.YDirectionMinus || type == TErrorBarType.YDirectionPlus;

      m_include = ( bIsPlus )
        ? ExcelErrorBarInclude.Plus
        : ExcelErrorBarInclude.Minus;

      if( range != null )
      {
        if( bIsPlus )
        {
          m_plusRange = range;
        }
        else
        {
          m_minusRange = range;
        }
      }
    }
    #endregion

    #region Class serialize methods
    /// <summary>
    /// Serializes records to biff stream
    /// </summary>
    /// <param name="records">Represents record holder.</param>
    public void Serialize( IList<IBiffStorage> records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if(m_errorBarRecord.ValuesNumber==0)  
           m_errorBarRecord.ValuesNumber =1;

      int iIndex = m_serie.Index;

      if( m_include == ExcelErrorBarInclude.Both )
      {
        SerializeErrorBar( records, m_bIsY, true, iIndex );

        m_errorBarRecord = ( ChartSerAuxErrBarRecord )m_errorBarRecord.Clone();
        SerializeErrorBar( records, m_bIsY, false, iIndex );
      }
      else
      {
        bool bIsPlus = m_include == ExcelErrorBarInclude.Plus;
        SerializeErrorBar( records, m_bIsY, bIsPlus, iIndex );
      }
    }
    /// <summary>
    /// Serializes AI records.
    /// </summary>
    /// <param name="records">Represents records holder.</param>
    /// <param name="bIsPlus">If true than plus sub bar; otherwise minus.</param>
    private void SerializeAiRecords( IList<IBiffStorage> records, bool bIsPlus )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      ChartAIRecord AIRecord = ( ChartAIRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartAI );
      AIRecord.IndexIdentifier = ChartAIRecord.LinkIndex.LinkToTitleOrText;
      AIRecord.Reference = ChartAIRecord.ReferenceType.EnteredDirectly;
      records.Add( AIRecord );

      AIRecord = ( ChartAIRecord )AIRecord.Clone();
      AIRecord.IndexIdentifier = ChartAIRecord.LinkIndex.LinkToValues;
      records.Add( AIRecord );

      if( Type == ExcelErrorBarType.Custom && m_bIsY )
      {
        AIRecord.Reference = ChartAIRecord.ReferenceType.Worksheet;
        AIRecord.ParsedExpression = GetPtg( bIsPlus );
      }

      AIRecord = ( ChartAIRecord )AIRecord.Clone();
      AIRecord.IndexIdentifier = ChartAIRecord.LinkIndex.LinkToCategories;

      if( Type == ExcelErrorBarType.Custom && !m_bIsY )
      {
        AIRecord.Reference = ChartAIRecord.ReferenceType.Worksheet;
        AIRecord.ParsedExpression = GetPtg( bIsPlus );
      }
      else
      {
        AIRecord.ParsedExpression = null;
        AIRecord.Reference = ChartAIRecord.ReferenceType.EnteredDirectly;
      }

      records.Add( AIRecord );

      AIRecord = ( ChartAIRecord )AIRecord.Clone();
      AIRecord.IndexIdentifier = ChartAIRecord.LinkIndex.LinkToBubbles;
      AIRecord.ParsedExpression = null;
      AIRecord.Reference = ChartAIRecord.ReferenceType.EnteredDirectly;
      records.Add( AIRecord );
    }
    private int GetCount( IRange range )
    {
      int iCount1;

      if( range != null )
      {
        iCount1 = ( range as ICombinedRange ).CellsCount;
      }
      else if( m_chartAi != null && m_chartAi.ParsedExpression != null && m_chartAi.ParsedExpression.Length > 0 )
      {
        IRectGetter rectGetter = ( m_chartAi.ParsedExpression[ 0 ] as IRectGetter );

        iCount1 = ( rectGetter != null ) ? 1 : 0;
      }
      else
      {
        iCount1 = 0;
      }

      return iCount1;
    }
    /// <summary>
    /// Serializes single error bar to biff stream.
    /// </summary>
    /// <param name="records">Represents record holder.</param>
    /// <param name="bIsYAxis">Indicates if in Y axis.</param>
    /// <param name="bIsPlus">Indicates if Plus error bar.</param>
    /// <param name="iSerieIndex">Represents Series index.</param>
    private void SerializeErrorBar( IList<IBiffStorage> records, bool bIsYAxis, bool bIsPlus, int iSerieIndex )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      int iCount1 = GetCount( m_minusRange ); ;
      int iCount2 = GetCount( m_plusRange );
      SerializeSerieRecord( records, ( bIsPlus ) ? iCount2 : iCount1 );

      BiffRecordRaw record = BiffRecordFactory.GetRecord( TBIFFRecord.Begin );
      records.Add( record );

      SerializeAiRecords( records, bIsPlus );

      ChartSeriesCollection seriesColl = m_serie.ParentSeries;
      SerializeDataFormatRecords( records, m_border, iSerieIndex, seriesColl.TrendErrorBarIndex, m_markerFormat );
      seriesColl.TrendErrorBarIndex++;

      ChartSerParentRecord serRecord = ( ChartSerParentRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartSerParent );

      serRecord.Series = ( ushort )( iSerieIndex + 1 );
      records.Add( serRecord );

      if( bIsYAxis )
      {
        m_errorBarRecord.ErrorBarValue = ( bIsPlus )
          ? TErrorBarType.YDirectionPlus
          : TErrorBarType.YDirectionMinus;
      }
      else
      {
        m_errorBarRecord.ErrorBarValue = ( bIsPlus )
          ? TErrorBarType.XDirectionPlus
          : TErrorBarType.XDirectionMinus;
      }

      records.Add( m_errorBarRecord );

      record = BiffRecordFactory.GetRecord( TBIFFRecord.End );
      records.Add( record );
    }
    /// <summary>
    /// Gets native PTG.
    /// </summary>
    /// <param name="bIsPlus">Indicates if it plus error bar.</param>
    /// <returns>Returns PTG.</returns>
    private Ptg[] GetPtg( bool bIsPlus )
    {
      Ptg[] result;

      if( m_bChanged )
      {
        IRange range = ( bIsPlus )
          ? m_plusRange
          : m_minusRange;

        result = ( range == null ) ?
          null :
          ( ( INativePTG )range ).GetNativePtg();
      }
      else
      {
        result = m_chartAi.ParsedExpression;
      }

      return result;
    }
    #endregion

    #region Class helper properties
    /// <summary>
    /// If true, error bar is on Y axis; otherwise - on X axis.
    /// </summary>
    public bool IsY
    {
      get
      {
        return m_bIsY;
      }
    }
    /// <summary>
    /// Represents the chart ErrorBar plus range values
    /// </summary>
    internal object[] PlusRangeValues
    {
        get
        {
            return m_plusRangeValues;
        }
        set
        {            
            m_plusRangeValues = value;
        }
    }
    /// <summary>
    /// Represents the chart ErrorBar minus range values
    /// </summary>
    internal object[] MinusRangeValues
    {
        get
        {
            return m_minusRangeValues;
        }
        set
        {            
            m_minusRangeValues = value;
        }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      if( m_chartAi != null )
        FormulaUtil.MarkUsedReferences( m_chartAi.ParsedExpression, usedItems );
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      if( m_chartAi != null )
      {
        Ptg[] tokens = m_chartAi.ParsedExpression;

        if( FormulaUtil.UpdateReferenceIndexes( tokens, arrUpdatedIndexes ) )
          m_chartAi.ParsedExpression = tokens;
      }
    }
    /// <summary>
    /// Checks include value.
    /// </summary>
    /// <param name="value">Represents include value.</param>
    /// <returns>Returns true if class support current inclure; otherwise false.</returns>
    private bool CheckInclude( ExcelErrorBarInclude value )
    {
      if( Type != ExcelErrorBarType.Custom )
        return true;

      if( value == ExcelErrorBarInclude.Plus )
      {
        m_minusRange = null;
        return m_plusRange != null;
      }

      if( value == ExcelErrorBarInclude.Minus )
      {
        m_plusRange = null;
        return m_minusRange != null;
      }

      return m_minusRange != null && m_plusRange != null;
    }
    #endregion

    #region Clone methods
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Represents parent object for new cloned instance.</param>
    /// <param name="hashNewNames">Represents new names.</param>
    /// <returns>Returns cloned object.</returns>
    public ChartErrorBarsImpl Clone( object parent, Dictionary<string, string> hashNewNames )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      ChartErrorBarsImpl result = ( ChartErrorBarsImpl )MemberwiseClone();
      result.SetParent( parent );
      result.FindParents();
      WorkbookImpl book = result.m_serie.ParentBook;

      result.m_border = m_border.Clone( result );

      if( m_errorBarRecord != null )
      {
        result.m_errorBarRecord = ( ChartSerAuxErrBarRecord )
          CloneUtils.CloneCloneable( m_errorBarRecord );
      }

      if( m_plusRange != null )
        result.m_plusRange = ( ( ICombinedRange )m_plusRange ).Clone( result, hashNewNames, book );

      if( m_minusRange != null )
        result.m_minusRange = ( ( ICombinedRange )m_minusRange ).Clone( result, hashNewNames, book );

      return result;
    }
    #endregion
  }
}
