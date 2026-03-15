#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#region file using directives
using System;
using System.Collections;

using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Interfaces;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
	/// <summary>
	/// Represents single data point in the collection.
	/// </summary>
	public class ChartDataPointImpl
    : CommonObject
    , IChartDataPoint
	{
    #region Class members
    /// <summary>
    /// Data labels.
    /// </summary>
    private ChartDataLabelsImpl m_dataLabels;
    /// <summary>
    /// Point index.
    /// </summary>
    private int m_iIndex;
    /// <summary>
    /// Data format.
    /// </summary>
    private ChartSerieDataFormatImpl m_dataFormat;
    /// <summary>
    /// Represents parent chart.
    /// </summary>
    private ChartImpl m_parentChart;
    /// <summary>
    /// Represents whether the series has datapoint or not
    /// </summary>
    private bool m_bHasDataPoint;
    /// <summary>
    /// Represent whether it's having the seperate seperate marker
    /// </summary>
    private bool m_defaultMarker;
    /// <summary>
    /// Indicates whether bubbles have a 3-D effect applied to them or not
    /// </summary>
    private bool m_bBubble3D;
    /// <summary>
    /// Represents the amount the data shall be moved from the center of the pie.
    /// </summary>
    private int m_explosion;
    /// <summary>
    /// Represents whether the pie has explosion
    /// </summary>
    private bool m_bHasExplosion;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance and sets its application and parent objects.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="index">Index of the data point.</param>
    public ChartDataPointImpl( IApplication application, object parent, int index )
      : base( application, parent )
    {
      m_iIndex = index;

      m_dataFormat = new ChartSerieDataFormatImpl( application, this );
      m_dataFormat.DataFormat.PointNumber = ( ushort )m_iIndex;
      m_parentChart = ( ChartImpl )FindParent( typeof( ChartImpl ) );

      if( m_parentChart == null )
        throw new Exception( "cannot find parent chart." );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns data labels object for the data point. Read-only.
    /// </summary>
    public IChartDataLabels DataLabels
    {
      get
      {
        CreateDataLabels();
        return m_dataLabels;
      }
    }
    /// <summary>
    /// Returns data format. Read-only.
    /// </summary>
    public IChartSerieDataFormat DataFormat
    {
      get
      {
        if( m_dataFormat == null )
          m_dataFormat = new ChartSerieDataFormatImpl( Application, this );

        return m_dataFormat;
      }
    }
    /// <summary>
    /// Gets /sets inner data format.
    /// </summary>
    public ChartSerieDataFormatImpl InnerDataFormat
    {
      get
      {
        return m_dataFormat;
      }
      set
      {
        m_dataFormat = value;

        if( value != null )
        {
          value.SetParent( this );
          value.SetParents();
        }
      }
    }
    /// <summary>
    /// Gets / sets index of the point in the points collection.
    /// </summary>
    public int Index
    {
      get
      {
        return m_iIndex;
      }
      set
      {
        m_iIndex = value;
      }
    }
    /// <summary>
    /// Returns data format or null.
    /// </summary>
    public ChartSerieDataFormatImpl DataFormatOrNull
    {
      get
      {
        return m_dataFormat;
      }
    }
    /// <summary>
    /// Indicates whether this data point is default data point. Read-only.
    /// </summary>
    public bool IsDefault
    {
      get
      {
        return m_iIndex == ChartSerieImpl.DEF_FORMAT_ALLPOINTS_INDEX;
      }
    }
    /// <summary>
    /// Indicates whether data labels object was created for this data point. Read-only.
    /// </summary>
    public bool HasDataLabels
    {
      get
      {
        return m_dataLabels != null;
      }
    }
    /// <summary>
    /// Indicates whether the series has datapoint or not.
    /// </summary>
    internal bool HasDataPoint
    {
        get
        {
            return m_bHasDataPoint;
        }
        set
        {
            m_bHasDataPoint = value;
        }
    }
    /// <summary>
    /// Indicate It's having the seperate marker type
    /// </summary>
    public bool IsDefaultmarkertype
    {
        get
        {
            return m_defaultMarker;
        }
        set
        {
            m_defaultMarker = value;
        }
    }
    /// <summary>
    /// Indicates whether bubbles have a 3-D effect applied to them or not
    /// </summary>
    internal bool Bubble3D
    {
        get
        {
            return m_bBubble3D;
        }
        set
        {
            m_bBubble3D = value;
        }
    }
    /// <summary>
    /// Gets or sets the amount the data shall be moved from the center of the pie.
    /// </summary>
    internal int Explosion
    {
        get
        {
            return m_explosion;
        }
        set
        {
            m_explosion = value;
            m_bHasExplosion = true;
        }
    }
    /// <summary>
    /// Gets the boolean value which represents whether the pie has explosion
    /// </summary>
    internal bool HasExplosion
    {
        get
        {
            return m_bHasExplosion;
        }
    }
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Serializes data labels data.
    /// </summary>
    /// <param name="records">List to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void SerializeDataLabels( OffsetArrayList records )
    {
      if( m_dataLabels != null )
      {
        ISerializable serializable = ( ISerializable )m_dataLabels;
        serializable.Serialize( records );
      }
    }
    /// <summary>
    /// Serializes used data formats.
    /// </summary>
    /// <param name="records">List to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void SerializeDataFormat( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_dataFormat == null ) return;

      m_dataFormat.UpdateDataFormatInDataPoint();
      m_dataFormat.Serialize( records );
    }
    /// <summary>
    /// Sets data labels text area format.
    /// </summary>
    /// <param name="textArea">Text area to set.</param>
    public void SetDataLabels( ChartTextAreaImpl textArea )
    {
      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      CreateDataLabels();
      m_dataLabels.TextArea = textArea;
    }
    /// <summary>
    /// Creates data labels object if necessary.
    /// </summary>
    private void CreateDataLabels()
    {
      if( m_dataLabels == null )
      {
        m_dataLabels = new ChartDataLabelsImpl( Application, this, Index );
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for the cloned object.</param>
    /// <param name="dicFontIndexes">New font indexes.</param>
    /// <param name="dicNewSheetNames">Dictionary with new worksheet names.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone( object parent, Dictionary<int, int> dicFontIndexes, Dictionary<string, string> dicNewSheetNames )
    {
      ChartDataPointImpl result = new ChartDataPointImpl( Application, parent, m_iIndex );

      if( m_dataLabels != null )
        result.m_dataLabels = ( ChartDataLabelsImpl )m_dataLabels.Clone( result,
          dicFontIndexes, dicNewSheetNames );

      if( m_dataFormat != null )
      {
        result.m_dataFormat = ( ChartSerieDataFormatImpl )m_dataFormat.Clone( result );
      }

      return result;
    }
    /// <summary>
    /// Updates Series index.
    /// </summary>
    public void UpdateSerieIndex()
    {
      if( m_dataLabels != null )
      {
        m_dataLabels.UpdateSerieIndex();
      }

      if( m_dataFormat != null )
      {
        m_dataFormat.UpdateSerieIndex();
      }
    }
    /// <summary>
    /// Changes data format to create ChartStockHighLowClose chart type.
    /// </summary>
    public void ChangeChartStockHigh_Low_CloseType()
    {
      DataFormat.MarkerStyle = ExcelChartMarkerType.DowJones;
      m_dataFormat.IsAutoMarker = false;
      m_dataFormat.MarkerForegroundColorIndex = ChartWallOrFloorImpl.DEF_CATEGORY_BACKGROUND_COLOR_INDEX;
      m_dataFormat.MarkerBackgroundColorIndex = ChartWallOrFloorImpl.DEF_CATEGORY_BACKGROUND_COLOR_INDEX;
      m_dataFormat.LineProperties.LinePattern = ExcelChartLinePattern.None;
      m_dataFormat.LineProperties.LineWeight = ExcelChartLineWeight.Hairline;
      m_dataFormat.LineProperties.ColorIndex = ChartWallOrFloorImpl.DEF_VALUE_BACKGROUND_COLOR_INDEX;
    }
    /// <summary>
    /// Changes data format to create ChartStockHighLowClose chart type.
    /// </summary>
    public void ChangeChartStockVolume_High_Low_CloseType()
    {
      DataFormat.MarkerStyle = ExcelChartMarkerType.DowJones;
      m_dataFormat.IsAutoMarker = false;
      m_dataFormat.MarkerForegroundColorIndex = ChartWallOrFloorImpl.DEF_VALUE_BACKGROUND_COLOR_INDEX;
      m_dataFormat.MarkerBackgroundColorIndex = ChartWallOrFloorImpl.DEF_VALUE_BACKGROUND_COLOR_INDEX;

      ExcelChartType destType = m_parentChart.DestinationType;
      m_parentChart.DestinationType = ExcelChartType.Line;
      m_dataFormat.LineProperties.LinePattern = ExcelChartLinePattern.None;
      m_parentChart.DestinationType = destType;
    }
    /// <summary>
    /// Changes intimate bubble series.
    /// </summary>
    /// <param name="typeToChange">Type to change.</param>
    public void ChangeIntimateBuble( ExcelChartType typeToChange )
    {
      DataFormat.LineProperties.LinePattern = ExcelChartLinePattern.Solid;
      DataFormat.Is3DBubbles = ( typeToChange == ExcelChartType.Bubble ) ? false : true;
    }
    /// <summary>
    /// Updates current data format.
    /// </summary>
    /// <param name="serieFormat">Represents data format for update.</param>
    public void CloneDataFormat( ChartSerieDataFormatImpl serieFormat )
    {
      if( serieFormat == null /*|| m_parentChart.Loading*/ )
        return;

      if( m_dataFormat == null || !m_dataFormat.IsFormatted )
      {
        ChartDataFormatRecord data = m_dataFormat.DataFormat;

        m_dataFormat = serieFormat.Clone( this );

        m_dataFormat.DataFormat = data;
      }
    }
    /// <summary>
    /// Clears data formats.
    /// </summary>
    /// <param name="format">Represents format to update.</param>
    public void ClearDataFormats( ChartSerieDataFormatImpl format )
    {
      if( m_dataFormat != null && m_dataFormat.IsFormatted )
      {
        ChartDataFormatRecord data = m_dataFormat.DataFormat;

        m_dataFormat = format.Clone( this );

        m_dataFormat.DataFormat = data;
      }
    }
    #endregion
  }
}
