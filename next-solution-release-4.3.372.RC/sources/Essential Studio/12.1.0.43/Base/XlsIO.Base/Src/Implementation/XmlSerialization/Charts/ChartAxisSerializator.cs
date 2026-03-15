#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Charts
{
  /// <summary>
  /// This class is responsible for chart axis serialization.
  /// </summary>
  public class ChartAxisSerializator
  {
    #region Class constants
    /// <summary>
    /// Defines text rotation multiplier constant.
    /// </summary>
    public const int TextRotationMultiplier = 60000;
    #endregion

    #region Members
    /// <summary>
    /// Represents dictionary for TickLabel to Attribute value.
    /// </summary>
    private static Dictionary<ExcelTickLabelPosition, string> s_dictTickLabelToAttributeValue = new Dictionary<ExcelTickLabelPosition, string>( 4 );
    /// <summary>
    /// Represents dictionary for TickMark to Attribute value.
    /// </summary>
    private static Dictionary<ExcelTickMark, string> s_dictTickMarkToAttributeValue = new Dictionary<ExcelTickMark, string>( 4 );
    #endregion

    #region Methods
    /// <summary>
    /// Initializes static members of the ChartAxisSerializator class.
    /// </summary>
    static ChartAxisSerializator()
    {
      s_dictTickLabelToAttributeValue.Add( ExcelTickLabelPosition.TickLabelPosition_High,
        ChartConstants.TickLabelHigh );

      s_dictTickLabelToAttributeValue.Add( ExcelTickLabelPosition.TickLabelPosition_Low,
        ChartConstants.TickLabelLow );

      s_dictTickLabelToAttributeValue.Add( ExcelTickLabelPosition.TickLabelPosition_NextToAxis,
        ChartConstants.TickLabelNextTo );

      s_dictTickLabelToAttributeValue.Add( ExcelTickLabelPosition.TickLabelPosition_None,
        ChartConstants.TickLabelNone );

      s_dictTickMarkToAttributeValue.Add(ExcelTickMark.TickMark_None, ChartConstants.TickMarkNone );
      s_dictTickMarkToAttributeValue.Add(ExcelTickMark.TickMark_Inside, ChartConstants.TickMarkInside );
      s_dictTickMarkToAttributeValue.Add(ExcelTickMark.TickMark_Outside, ChartConstants.TickMarkOutside );
      s_dictTickMarkToAttributeValue.Add( ExcelTickMark.TickMark_Cross, ChartConstants.TickMarkCross );
    }
    /// <summary>
    /// Serializes chart axis.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="axis">Axis to serialize.</param>
    public void SerializeAxis( XmlWriter writer, IChartAxis axis, RelationCollection relations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( axis == null )
        return;

      switch( axis.AxisType )
      {
        case ExcelAxisType.Category:

          ChartCategoryAxisImpl categoryAxis = ( ChartCategoryAxisImpl )axis;
          if( !categoryAxis.IsChartBubbleOrScatter )
          {
            if( categoryAxis.CategoryType == ExcelCategoryType.Time )
            {
              SerializeDateAxis( writer, categoryAxis );
            }
            else
            {
              SerializeCategoryAxis( writer, categoryAxis );
            }
          }
          else
          {
            SerializeValueAxis( writer, ( ChartValueAxisImpl )axis, relations );
          }
          break;

        case ExcelAxisType.Value:
          SerializeValueAxis( writer, ( ChartValueAxisImpl )axis, relations );
          break;

        case ExcelAxisType.Serie:
          SerializeSeriesAxis( writer, ( ChartSeriesAxisImpl )axis );
          break;

        default:
          throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Serializes date axis.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="axis">Axis to serialize.</param>
    private void SerializeDateAxis( XmlWriter writer, ChartCategoryAxisImpl axis )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      writer.WriteStartElement( ChartConstants.DateAxisTag, ChartConstants.CNamespace );
      SerializeAxisCommon( writer, axis );

      if (axis.CategoryType == ExcelCategoryType.Automatic)
          ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.AutoCategoryAxis, true);

      if (axis.LabelAlign != null)
          ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.LabelAlignment, axis.LabelAlign);

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.LabelOffsetTag,
        axis.Offset.ToString() );

      if( !axis.IsAutoMajor )
      {
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.MajorUnitTag,
          XmlConvert.ToString( axis.MajorUnit ) );
      }
      string strUnit;
      if (!axis.MajorUnitScaleIsAuto || ((axis.ParentChart as ChartImpl).Workbook as WorkbookImpl).IsConverted)
      {
          strUnit = ConvertDateUnitToString(axis.MajorUnitScale);
          ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.MajorTimeUnit, strUnit);
      }

      if( !axis.IsAutoMinor )
      {
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.MinorUnitTag,
          XmlConvert.ToString( axis.MinorUnit ) );
      }

      if (!axis.MinorUnitScaleIsAuto || ((axis.ParentChart as ChartImpl).Workbook as WorkbookImpl).IsConverted)
      {
          strUnit = ConvertDateUnitToString(axis.MinorUnitScale);
          ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.MinorTimeUnit, strUnit);
      }

      if (!axis.BaseUnitIsAuto)
      {
          string strBaseUnit = ConvertDateUnitToString(axis.BaseUnit);
          ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.BaseTimeUnitTag,
            strBaseUnit);
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Convert date unit to string.
    /// </summary>
    /// <param name="baseUnit">ExcelChartBaseUnit to serialize.</param>
    /// <returns>Date unit as string.</returns>
    private string ConvertDateUnitToString( ExcelChartBaseUnit baseUnit )
    {
      return baseUnit.ToString().ToLower() + 's';
    }
    /// <summary>
    /// Serializes category axis.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="axis">Axis to serialize.</param>
    private void SerializeCategoryAxis( XmlWriter writer, ChartCategoryAxisImpl axis )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      writer.WriteStartElement( ChartConstants.CategoryAxisTag, ChartConstants.CNamespace );

      SerializeAxisCommon( writer, axis );

      if (axis.CategoryType == ExcelCategoryType.Automatic)
          ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.AutoCategoryAxis, true);

      if (axis.LabelAlign != null)
          ChartSerializatorCommon.SerializeValueTag(writer, ChartConstants.LabelAlignment, axis.LabelAlign);

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.LabelOffsetTag,
        axis.Offset.ToString() );

      if (axis.m_showNoMultiLvlLbl)
          ChartSerializatorCommon.SerializeBoolValueTag(writer, ChartConstants.NoMultiLvlLblTag, axis.NoMultiLevelLabel);

      if(!axis.IsAutoMajor|| !axis.AutoTickLabelSpacing )
      {
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.TickLabelSkip,
          axis.TickLabelSpacing.ToString() );
      }

      if (!axis.IsAutoMinor || !axis.AutoTickMarkSpacing)
      {
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.TickMarkSkip,
          axis.TickMarkSpacing.ToString() );
      }

      // TODO: finish implementation

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes value axis.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="valueAxis">Axis to serialize.</param>
    private void SerializeValueAxis( XmlWriter writer, ChartValueAxisImpl valueAxis, RelationCollection relations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( valueAxis == null )
        throw new ArgumentNullException( "valueAxis" );

      writer.WriteStartElement( ChartConstants.ValueAxisTag, ChartConstants.CNamespace );

      SerializeAxisCommon( writer, valueAxis );
      //TODO: implement crossBetween tag support.
      ChartImpl chart = valueAxis.ParentChart;
      IChartCategoryAxis categoryAxis = valueAxis.IsPrimary ?
        chart.PrimaryCategoryAxis :
        chart.SecondaryCategoryAxis;

      string strCrossBetween = categoryAxis.IsBetween ?
        ChartConstants.BetweenValue :
        ChartConstants.CategoryMidpoint;

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.CrossBetweenTag,
        strCrossBetween );

      if( !valueAxis.IsAutoMajor )
      {
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.MajorUnitTag,
          XmlConvert.ToString( valueAxis.MajorUnit ) );
      }

      if( !valueAxis.IsAutoMinor )
      {
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.MinorUnitTag,
          XmlConvert.ToString( valueAxis.MinorUnit ) );
      }

      SerializeDisplayUnit( writer, valueAxis, relations );//valueAxis.DisplayUnit, valueAxis.DisplayUnitCustom, valueAxis.HasDisplayUnitLabel );

      writer.WriteEndElement();

      //throw new NotImplementedException();
    }
    /// <summary>
    /// Serializes series axis.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="seriesAxis">Axis to serialize.</param>
    private void SerializeSeriesAxis( XmlWriter writer, ChartSeriesAxisImpl seriesAxis )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( seriesAxis == null )
        throw new ArgumentNullException( "seriesAxis" );

      writer.WriteStartElement( ChartConstants.SeriesAxisTag, ChartConstants.CNamespace );

      SerializeAxisCommon( writer, seriesAxis );

      if( !seriesAxis.AutoTickLabelSpacing )
      {
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.TickLabelSkip,
          seriesAxis.TickLabelSpacing.ToString() );
      }

      if( !seriesAxis.AutoTickMarkSpacing )
      {
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.TickMarkSkip,
          seriesAxis.TickMarkSpacing.ToString() );
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes common part of the axis.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="axis">Axis to serialize.</param>
    private void SerializeAxisCommon( XmlWriter writer, ChartAxisImpl axis )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      ChartImpl chart = axis.ParentChart;
      WorksheetDataHolder sheetHolder = chart.DataHolder;
      FileDataHolder holder = sheetHolder.ParentHolder;
      RelationCollection relations = chart.Relations;

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisIdTag,
        axis.AxisId.ToString() );

      SerializeScaling( writer, axis );
      // delete?
      //if( axis.Deleted )
        ChartSerializatorCommon.SerializeBoolValueTag( writer, ChartConstants.DeleteTag, axis.Deleted );

      SerializeAxisPosition( writer, axis );
      SerializeGridlines( writer, axis );

      if( axis.HasAxisTitle )
        ChartSerializatorCommon.SerializeTextArea( writer, axis.TitleArea, chart.ParentWorkbook, relations, ChartAxisParser.DefaultFontSize  );

      if (axis.isNumber)
      SerializeNumberFormat( writer, axis );
      SerializeTickMark( writer, ChartConstants.MajorTickMarkTag, axis.MajorTickMark );
      SerializeTickMark( writer, ChartConstants.MinorTickMarkTag, axis.MinorTickMark );
      SerializeTickLabel( writer, axis );

      if (axis.FrameFormat.HasInterior && axis.FrameFormat.Interior.Pattern != ExcelPattern.None)
      {
          ChartSerializatorCommon.SerializeFrameFormat(writer, axis.FrameFormat, axis.ParentChart, false);
      }
      else if(axis.FrameFormat.HasLineProperties)
      {
          writer.WriteStartElement(Drawings.ShapePropertiesTag, ChartConstants.CNamespace);
          ChartSerializatorCommon.SerializeLineProperties(writer, axis.Border, holder.Workbook);
          writer.WriteEndElement();
      }

      if((axis.ParagraphType== ChartParagraphType.CustomDefault) || (!axis.IsAutoTextRotation))
           SerializeTextSettings( writer, axis );
      SerializeCrossAxis( writer, axis );

      string strCross = ChartConstants.CrossesAutoZero;
      string strCrossesTag = ChartConstants.CrossesTag;

      ChartValueAxisImpl valueAxis = GetPairAxis( axis ) as ChartValueAxisImpl;

      if( valueAxis != null )
      {
        if( valueAxis.IsMaxCross )
        {
          strCross = ChartConstants.CrossesMaximum;
        }
        else if( !valueAxis.IsAutoCross )
        {
          strCross = XmlConvert.ToString( valueAxis.CrossesAt );
          strCrossesTag = ChartConstants.CrossesAtTag;
        }
      }

      ChartSerializatorCommon.SerializeValueTag( writer, strCrossesTag, strCross );

      // (crosses | crossesAt)?
    }
    /// <summary>
    /// Returns pair for the specified axis, it is category axis for value axis and vice versa.
    /// </summary>
    /// <param name="axis">Axis to get pair for.</param>
    /// <returns>Pair for the specified axis.</returns>
    public static IChartAxis GetPairAxis( ChartAxisImpl axis )
    {
      IChartAxis result = null;

      if( axis != null )
      {
        ChartImpl chart = axis.ParentChart;

        switch( axis.AxisType )
        {
          case ExcelAxisType.Category:
            result = axis.IsPrimary ?
              chart.PrimaryValueAxis :
              chart.SecondaryValueAxis;
            break;

          case ExcelAxisType.Value:
            result = axis.IsPrimary ?
              chart.PrimaryCategoryAxis :
              chart.SecondaryCategoryAxis;
            break;
        }
      }

      return result;
    }
    /// <summary>
    /// Serializes cross axis.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="axis">Axis to serialize.</param>
    private void SerializeCrossAxis( XmlWriter writer, ChartAxisImpl axis )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      ChartImpl chart = axis.ParentChart;
      bool bPrimary = axis.IsPrimary;
      ChartAxisImpl crossAxis;
      // category axis crosses value
      // series crosses value?
      // value crosses category
      // we have no support for secondary axis yet.
      switch( axis.AxisType )
      {
        case ExcelAxisType.Category:
          crossAxis = ( ChartAxisImpl )( bPrimary ?
            chart.PrimaryValueAxis :
            chart.SecondaryValueAxis );
          break;

        case ExcelAxisType.Serie:
          crossAxis = ( ChartAxisImpl )chart.PrimaryValueAxis;
          break;

        case ExcelAxisType.Value:
          crossAxis = ( ChartAxisImpl )( bPrimary ?
            chart.PrimaryCategoryAxis :
            chart.SecondaryCategoryAxis );
          break;

        default:
          throw new InvalidOperationException();
      }

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.CrossAxisTag,
        crossAxis.AxisId.ToString() );
    }
    /// <summary>
    /// Serializes tick mark.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="tagName">Tag name to use.</param>
    /// <param name="tickMark">Tick mark to serialize</param>
    private void SerializeTickMark( XmlWriter writer, string tagName, ExcelTickMark tickMark )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( tagName == null || tagName.Length == 0 )
        throw new ArgumentException( "tagName" );

      string strTickValue = s_dictTickMarkToAttributeValue[ tickMark ];
      ChartSerializatorCommon.SerializeValueTag( writer, tagName, strTickValue );
    }
    /// <summary>
    /// Serializes tick label position.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="axis">Axis to get tick label position from.</param>
    private void SerializeTickLabel( XmlWriter writer, ChartAxisImpl axis )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      string strValue = s_dictTickLabelToAttributeValue[ axis.TickLabelPosition ];
      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.TickLabelPositionTag,
        strValue );
    }
    /// <summary>
    /// Serializes number format used by the axis.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="axis">Axis to serialize number format for.</param>
    private void SerializeNumberFormat( XmlWriter writer, ChartAxisImpl axis )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      writer.WriteStartElement( ChartConstants.NumberFormatTag, ChartConstants.CNamespace );
      writer.WriteAttributeString( ChartConstants.FormatCodeAttribute, axis.NumberFormat );

      bool bSourceLinked = axis.IsSourceLinked;
      Excel2007Serializator.SerializeAttribute( writer, ChartConstants.SourceLinkedAttribute, bSourceLinked, !bSourceLinked );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes axis gridlines.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="axis">Axis to serialize gridlines for.</param>
    private void SerializeGridlines( XmlWriter writer, ChartAxisImpl axis )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      WorkbookImpl book = axis.ParentChart.ParentWorkbook;

      if( axis.HasMajorGridLines )
      {
        SerializeGridlines( writer, axis.MajorGridLines, ChartConstants.MajorGridlinesTag, book );
        //writer.WriteElementString( ChartConstants.MajorGridlinesTag, ChartConstants.CNamespace, string.Empty );
      }

      if( axis.HasMinorGridLines )
      {
        SerializeGridlines( writer, axis.MinorGridLines, ChartConstants.MinorGridlinesTag, book );
        //writer.WriteElementString( ChartConstants.MinorGridlinesTag, ChartConstants.CNamespace, string.Empty );
      }
    }
    /// <summary>
    /// Serializes single gridline object.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="gridLines">Gridlines to serialize.</param>
    /// <param name="tagName">Name of the xml tag to use.</param>
    /// <param name="book">Parent workbook.</param>
    private void SerializeGridlines( XmlWriter writer, IChartGridLine gridLines,
      string tagName, IWorkbook book )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( gridLines == null )
        throw new ArgumentNullException( "gridLines" );

      if( tagName == null || tagName.Length == 0 )
        throw new ArgumentOutOfRangeException( "tagName" );

      writer.WriteStartElement( tagName, ChartConstants.CNamespace );

      IChartBorder line = gridLines.Border;

      if( line != null && !line.AutoFormat )
      {
        writer.WriteStartElement( Drawings.ShapePropertiesTag, ChartConstants.CNamespace );
        ChartSerializatorCommon.SerializeLineProperties( writer, line, book );
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes axis position.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="axis">Axis to serialize.</param>
    private void SerializeAxisPosition( XmlWriter writer, ChartAxisImpl axis )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( axis == null )
        throw new ArgumentNullException( "valueAxis" );

      // TODO: we don't support any positioning on the current moment, we should add it later.
      string strPosition = ( axis.AxisPosition != null ) ? axis.AxisPosition.ToString() : null;

      if( strPosition == null )
      {
        switch( axis.AxisType )
        {
          case ExcelAxisType.Category:
            strPosition = axis.IsPrimary ? ChartConstants.AxisPosBottom : ChartConstants.AxisPosTop;
            break;

          case ExcelAxisType.Value:
            strPosition = axis.IsPrimary ? ChartConstants.AxisPosLeft : ChartConstants.AxisPosRight;
            break;

          case ExcelAxisType.Serie:
            strPosition = axis.IsPrimary ? ChartConstants.AxisPosBottom : ChartConstants.AxisPosTop;
            break;
        }
      }

      if( strPosition != null )
      {
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisPositionTag,
          strPosition );
      }
    }
    /// <summary>
    /// Serializes scaling tag and all necessary child tags.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="axis">Axis to serialize settings for.</param>
    private void SerializeScaling( XmlWriter writer, ChartAxisImpl axis )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( axis == null )
        throw new ArgumentNullException( "axis" );

      writer.WriteStartElement( ChartConstants.ScalingTag, ChartConstants.CNamespace );

      ChartValueAxisImpl valueAxis = axis as ChartValueAxisImpl;

      if( valueAxis != null && valueAxis.IsLogScale )
      {
        //valueAxis.
        //throw new NotImplementedException();
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.LogBaseTag,
          XmlConvert.ToString( ChartConstants.LogBaseDefault ) );
      }

      string strOrientation = axis.IsReversed ?
        ChartConstants.MaxMinOrientation :
        ChartConstants.MinMaxOrientation;

      ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.AxisOrientationTag,
        strOrientation );

      if( valueAxis != null )
      {
        if( !valueAxis.IsAutoMax )
        {
          ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.MaximumTag,
            XmlConvert.ToString( valueAxis.MaximumValue ) );
        }

        if( !valueAxis.IsAutoMin )
        {
          ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.MinimumTag,
            XmlConvert.ToString( valueAxis.MinimumValue ) );
        }
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes display unit.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="displayUnit">Value to serialize.</param>
    /// <param name="unitValue">Custom unit value (used only if displayUnit is set to custom).</param>
    private void SerializeDisplayUnit( XmlWriter writer, ChartValueAxisImpl axis, RelationCollection relations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      ExcelChartDisplayUnit displayUnit = axis.DisplayUnit;

      if( displayUnit == ExcelChartDisplayUnit.None )
        return;

      bool bHasDisplayUnitLabel = axis.HasDisplayUnitLabel;

      writer.WriteStartElement( ChartConstants.DisplayUnitsTag, ChartConstants.CNamespace );
      string strUnit;

      if( displayUnit != ExcelChartDisplayUnit.Custom )
      {
        strUnit = ( ( Excel2007ChartDisplayUnit )displayUnit ).ToString();
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.BuiltInUnitTag, strUnit );
      }
      else
      {
        double unitValue = axis.DisplayUnitCustom;
        strUnit = XmlConvert.ToString( unitValue );
        ChartSerializatorCommon.SerializeValueTag( writer, ChartConstants.CustomUnitTag, strUnit );
      }

      if( bHasDisplayUnitLabel )
      {
        ChartImpl chart = axis.ParentChart;
        WorkbookImpl book = chart.ParentWorkbook;
        //ChartSerializatorCommon.SerializeTextArea( writer, axis.DisplayUnitLabel, relations );
        IChartTextArea textArea = axis.DisplayUnitLabel;
        ChartTextAreaImpl labelImpl = textArea as ChartTextAreaImpl;
        writer.WriteStartElement( ChartConstants.DisplayUnitsLabel, ChartConstants.CNamespace );
        //ChartSerializatorCommon.SerializeTextAreaText( writer, textArea, book );
        ChartSerializatorCommon.SerializeFrameFormat( writer, textArea.FrameFormat, chart, false );
        if(labelImpl.ParagraphType == ChartParagraphType.CustomDefault)
            SerializeTextSettings( writer, book, textArea, false, 0 );
        //System.Diagnostics.Debugger.Break();
        //ChartSerializatorCommon.SerializeRichText( writer, textArea, book, ChartConstants.TextPropertiesTag );
        writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes text settings.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="axis">Axis to serialize settings for.</param>
    private void SerializeTextSettings( XmlWriter writer, ChartAxisImpl axis )
    {
      if( axis == null )
        throw new ArgumentNullException( "axis" );

        if (axis.IsDefaultTextSettings == true)
        {
            if (axis.TextStream != null)
            {
                axis.TextStream.Position = 0;
                ShapeParser.WriteNodeFromStream(writer, axis.TextStream);
            }
        }
        else
        {
            SerializeTextSettings( writer, axis.ParentChart.ParentWorkbook, axis.Font, axis.IsAutoTextRotation, axis.TextRotationAngle );
        }
    }
    /// <summary>
    /// Serializes text settings.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    private void SerializeTextSettings( XmlWriter writer, IWorkbook book, IFont font, bool isAutoTextRotation, int rotationAngle  )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartElement( ChartConstants.TextPropertiesTag, ChartConstants.CNamespace );
      writer.WriteStartElement( Drawings.TextBodyPropertiesTag, Drawings.ANamespace );

      if( !isAutoTextRotation )
      {
        int iAngle = rotationAngle * TextRotationMultiplier;
        writer.WriteAttributeString( Drawings.TextRotationAttribute, iAngle.ToString() );
      }

      writer.WriteEndElement();

      writer.WriteStartElement( Drawings.Paragraphs, Drawings.ANamespace );
      writer.WriteStartElement( Drawings.ParagraphProperties, Drawings.ANamespace );
      ChartSerializatorCommon.SerializeParagraphRunProperites( writer, font,
        Drawings.DefaultParagraphProperites, book, ChartAxisParser.DefaultFontSize );
      //writer.WriteStartElement( Drawings.DefaultParagraphProperites, Drawings.ANamespace );
      //int iFontSize = ( int )( axis.Font.Size * 100 );
      //writer.WriteAttributeString( Drawings.FontSizeAttribute, iFontSize.ToString() );
      //writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();

      writer.WriteEndElement();
    }
    #endregion
  }
}
