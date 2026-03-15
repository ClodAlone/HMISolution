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
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
using System.IO;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.Compression.Zip;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes
{
  /// <summary>
  /// This class is responsible for chart shape serialization.
  /// </summary>
  public class ChartShapeSerializator : DrawingShapeSerializator
  {
    #region Constants
    /// <summary>
    /// Format for the default chart item.
    /// </summary>
    public const string ChartItemPath = "/xl/charts/chart{0}.xml";
    #endregion

    #region Methods
    /// <summary>
    /// This method serializes specified shape into specified writer.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize shape settings into.</param>
    /// <param name="shape">Shape to serialize.</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    public override void Serialize( XmlWriter writer, ShapeImpl shape, WorksheetDataHolder holder,
      RelationCollection vmlRelations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      if( holder == null )
        throw new ArgumentNullException( "holder" );

      ChartShapeImpl chartShape = ( ( ChartShapeImpl )shape );
      ChartImpl chart = chartShape.ChartObject;
      if (chart.Relations.Count != 0)
      {
          foreach (KeyValuePair<string, Relation> entry in chart.Relations)
          {
              if (entry.Value.Target.Contains("drawing"))
              {
                  chart.Relations.Remove(entry.Key);
                  break;
              }             
          }
      }
      string strChartFileName;
      if (chart.DataHolder == null && holder != null) chart.DataHolder = holder;
      string strRelationId = SerializeChartFile( holder, chart, out strChartFileName );

      if (!shape.IsAbsoluteAnchor)
      {
          writer.WriteStartElement(Drawings.TwoCellAnchorTagName, Drawings.XdrNamespace);          

      SerializeAnchorPoint( writer, Drawings.FromTagName,
        shape.LeftColumn, shape.LeftColumnOffset,
        shape.TopRow, shape.TopRowOffset, shape.Worksheet,
        Drawings.XdrNamespace );

      SerializeAnchorPoint( writer, Drawings.ToTagName,
        shape.RightColumn, shape.RightColumnOffset,
        shape.BottomRow, shape.BottomRowOffset, shape.Worksheet,
        Drawings.XdrNamespace );

      SerializeChartProperties( writer, chartShape, strRelationId, holder, false );

          writer.WriteEndElement();
      }
      else
      {
          ChartSerializator.SerializeAbsoluteAnchorChart(writer, chart, strRelationId);
      }
        
      holder.SerializeRelations( chart.Relations, strChartFileName.Substring( 1 ), holder );
    }
    /// <summary>
    /// Serialize chart object into separate file and returns relation id.
    /// </summary>
    /// <param name="holder">Parent worksheet data holder.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="chartFileName">Gets name of the chart item.</param>
    /// <returns>Relation id of the serialize chart object.</returns>
    internal string SerializeChartFile( WorksheetDataHolder holder, ChartImpl chart, out string chartFileName )
    {
      if( holder == null )
        throw new ArgumentNullException( "holder" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      MemoryStream stream = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter( stream );
      XmlWriter writer = UtilityMethods.CreateWriter( streamWriter );

      chartFileName = GetChartFileName( holder, chart );
      ChartSerializator serializator = new ChartSerializator();
      FileDataHolder parentHolder = holder.ParentHolder;

      if( chart.DataHolder == null )
      {
        parentHolder.CreateDataHolder( chart, chartFileName );
      }

      serializator.SerializeChart( writer, chart, chartFileName );
      writer.Flush();
      streamWriter.Flush();

      string strItemName = UtilityMethods.RemoveFirstCharUnsafe( chartFileName );
      parentHolder.Archive.UpdateItem( strItemName, stream, true, 
#if ( WINRT )
          Windows.Storage.FileAttributes.Archive 
#else
              FileAttributes.Archive 
#endif
          );
      
      string strRelationId = holder.DrawingsRelations.GenerateRelationId();
      holder.DrawingsRelations[ strRelationId ] = new Relation( chartFileName, RelationTypes.Chart );
      parentHolder.OverriddenContentTypes[ chartFileName ] = ContentTypes.Chart;

      return strRelationId;
    }
    /// <summary>
    /// Generates item name for the new chart object.
    /// </summary>
    /// <param name="holder">Parent worksheet data holder.</param>
    /// <param name="chart">Chart to generate name for.</param>
    /// <returns>Item name that should be used to save chart.</returns>
    public static string GetChartFileName( WorksheetDataHolder holder, ChartImpl chart )
    {
      if( holder == null )
        throw new ArgumentNullException( "holder" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      int iIndex = ++holder.ParentHolder.LastChartIndex;
      return string.Format( ChartItemPath, iIndex );
    }
    /// <summary>
    /// Serializes chart shape properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="strRelationId">Relation id that points to the chart object.</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    internal void SerializeChartProperties( XmlWriter writer, ChartShapeImpl chart, string strRelationId,
      WorksheetDataHolder holder, bool isGroupShape )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( strRelationId == null || strRelationId.Length == 0 )
        throw new ArgumentOutOfRangeException( "strRelationId" );

      if( holder == null )
        throw new ArgumentNullException( "holder" );

      writer.WriteStartElement( Drawings.GraphicFrame, Drawings.XdrNamespace );
      writer.WriteAttributeString( Drawings.MacroAttribute, string.Empty );
      SerializeNonVisualGraphicFrameProperties( writer, chart, holder );
      if (!isGroupShape)
          SerializeForm(writer, Drawings.XdrNamespace, Drawings.ANamespace, 0, 0, 0, 0);
      else
          SerializeForm(writer, Drawings.XdrNamespace, Drawings.ANamespace, chart.OffsetX, chart.OffsetY, chart.ExtentsX, chart.ExtentsY);
      SerializeGraphics( writer, chart, strRelationId );
      writer.WriteEndElement();

      if (!isGroupShape)
        writer.WriteElementString(Drawings.ClientDataTagName, Drawings.XdrNamespace, string.Empty);
    }
    /// <summary>
    /// Serializes all graphics xml tag and all subtags.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart to serialize.</param>
    /// <param name="strRelationId">Relation id of the chart object item.</param>
    private void SerializeGraphics( XmlWriter writer, ChartShapeImpl chart, string strRelationId )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( strRelationId == null || strRelationId.Length == 0 )
        throw new ArgumentOutOfRangeException( "strRelationId" );

      writer.WriteStartElement( Drawings.GraphicTag, Drawings.ANamespace );
      writer.WriteStartElement( Drawings.GraphicDataTag, Drawings.ANamespace );
      writer.WriteAttributeString( Drawings.UriAttribute, ChartConstants.CNamespace );
      writer.WriteStartElement( ChartConstants.CPrefix, ChartConstants.ChartTag, ChartConstants.CNamespace );

      //writer.WriteAttributeString( WorkbookXmlSerializator.DEF_XMLNS_PREF, "c",
      //  null, ChartConstants.CNamespace );

      writer.WriteAttributeString( Drawings.IdAttributeName, Excel2007Serializator.RelationNamespace,
        strRelationId );
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize slicer elements of graphic frames
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">shape to serialize.</param>
    /// <param name="strRelationId">Relation id of the chart object item.</param>
    internal void SerializeSlicerGraphics(XmlWriter writer, ChartShapeImpl shape)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (shape == null)
            throw new ArgumentNullException("chart");

      
        writer.WriteStartElement(Drawings.GraphicTag, Drawings.ANamespace);
        writer.WriteStartElement(Drawings.GraphicDataTag, Drawings.ANamespace);
        writer.WriteAttributeString(Drawings.UriAttribute, ChartConstants .SlicerNamespace );
        shape.GraphicFrameStream .Position = 0;
        Syncfusion.XlsIO.Implementation.XmlReaders.Shapes.ShapeParser.WriteNodeFromStream(writer, shape.GraphicFrameStream);

        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes non visual graphic frame properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="chart">Chart shape to serialize.</param>
    /// <param name="holder">Parent worksheet data holder.</param>
    internal void SerializeNonVisualGraphicFrameProperties( XmlWriter writer, ChartShapeImpl chart,
      WorksheetDataHolder holder )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      writer.WriteStartElement( Drawings.NonVisualGraphicFramePr, Drawings.XdrNamespace );
      SerializeNVCanvasProperties( writer, chart, holder, Drawings.XdrNamespace );
      writer.WriteElementString( Drawings.CNVGraphicFramePr, Drawings.XdrNamespace, string.Empty );
      writer.WriteEndElement();
    }
    #endregion
  }
}
