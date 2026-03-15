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
using System.IO;

using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes
{
  /// <summary>
  /// This serializator use to serialize shapes in DrawingML 2007 format,
  /// when shape is not supported, but it was extracted from Excel 2007
  /// document and we can preserve it.
  /// </summary>
  public class DrawingShapeSerializator : ShapeSerializator
  {
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

      Stream dataStream = shape.XmlDataStream;

      if( dataStream != null && dataStream.Length > 0 )
      {
        MsofbtClientAnchor anchor = shape.ClientAnchor;

        if( shape.EnableAlternateContent )
        {
          // TODO: move to constants.
          Excel2007Serializator.WriteAlternateContentHeader( writer );
        }

        if( anchor.OneCellAnchor )
        {
          writer.WriteStartElement( Drawings.OneCellAnchorTagName, Drawings.XdrNamespace );
          //writer.WriteAttributeString( Drawings.EditAsAttribute, GetEditAsValue( picture ) );
        }
        else
        {
          writer.WriteStartElement( Drawings.TwoCellAnchorTagName, Drawings.XdrNamespace );
        }

        SerializeAnchorPoint( writer, Drawings.FromTagName,
          shape.LeftColumn, shape.LeftColumnOffset,
          shape.TopRow, shape.TopRowOffset, ( WorksheetBaseImpl)shape.Worksheet,
          Drawings.XdrNamespace );

        if( anchor.OneCellAnchor )
        {
          writer.WriteStartElement( Drawings.Extents, Drawings.XdrNamespace );

          int iEMU = ( int )ApplicationImpl.ConvertFromPixel( shape.Width, MeasureUnits.EMU );
          writer.WriteAttributeString( Drawings.CXAttributeName, iEMU.ToString() );

          iEMU = ( int )ApplicationImpl.ConvertFromPixel( shape.Height, MeasureUnits.EMU );
          writer.WriteAttributeString( Drawings.CYAttributeName, iEMU.ToString() );

          writer.WriteEndElement();
        }
        else
        {
          SerializeAnchorPoint( writer, Drawings.ToTagName,
            shape.RightColumn, shape.RightColumnOffset,
            shape.BottomRow, shape.BottomRowOffset, ( WorksheetBaseImpl)shape.Worksheet,
            Drawings.XdrNamespace );
        }

        //SerializePicture( writer, picture, strRelationId );
        dataStream.Position = 0;
        XmlReader reader = UtilityMethods.CreateReader( dataStream );

        writer.WriteNode( reader, false );
        writer.WriteElementString( Drawings.ClientDataTagName, Drawings.XdrNamespace, string.Empty );
        writer.WriteEndElement();

        if( shape.EnableAlternateContent )
        {
          Excel2007Serializator.WriteAlternateContentFooter( writer );
        }
      }
    }
    /// <summary>
    /// This method serializes general shape settings (shape type) into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to write shape type into.</param>
    /// <param name="shapeType">Type of the shape that is going to be serialized.</param>
    public override void SerializeShapeType( XmlWriter writer, Type shapeType )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Returns editAs attribute value.
    /// </summary>
    /// <param name="shape">Shape to get settings from.</param>
    /// <returns>String value of editAs attribute for the shape.</returns>
    public static string GetEditAsValue( ShapeImpl shape )
    {
      if( shape == null )
        throw new ArgumentNullException( "shape" );

      string strResult;

      if( shape.IsMoveWithCell )
      {
        if( shape.IsSizeWithCell )
        {
          strResult = Drawings.PositionSizeRelative;
        }
        else
        {
          strResult = Drawings.PositionRelative;
        }
      }
      else
      {
        strResult = Drawings.PositionSizeAbsolute;
      }

      return strResult;
    }
    /// <summary>
    /// Serializes anchor point.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="tagName">Name of the tag for anchor point.</param>
    /// <param name="column">Column index.</param>
    /// <param name="columnOffset">Offset inside column.</param>
    /// <param name="row">Row index.</param>
    /// <param name="rowOffset">Offset inside row.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="drawingsNamespace">Xml namespace to use.</param>
    internal void SerializeAnchorPoint( XmlWriter writer, string tagName,
      int column, int columnOffset, int row, int rowOffset, WorksheetBaseImpl sheet,
      string drawingsNamespace )
    {
      // TODO: we have to convert offsets into appropriate units.
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( tagName == null || tagName.Length == 0 )
        throw new ArgumentOutOfRangeException( "tagName" );

      if( sheet as WorksheetImpl != null )
      {
        SerializeColRowAnchor( writer, tagName,
          column, columnOffset, row, rowOffset, sheet, drawingsNamespace );
      }
      else
      {
        SerializeXYAnchor( writer, tagName, column, row, drawingsNamespace );
      }
    }
    /// <summary>
    /// Serializes row-column anchor point.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="tagName">Tag name to use.</param>
    /// <param name="column">Column index.</param>
    /// <param name="columnOffset">Offset inside column.</param>
    /// <param name="row">Row index.</param>
    /// <param name="rowOffset">Offset inside row.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="drawingsNamespace">Xml namespace to use.</param>
    private void SerializeColRowAnchor( XmlWriter writer, string tagName,
      int column, int columnOffset, int row, int rowOffset, WorksheetBaseImpl sheet,
      string drawingsNamespace )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      // TODO: we have to convert offsets into appropriate units.
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( tagName == null || tagName.Length == 0 )
        throw new ArgumentOutOfRangeException( "tagName" );

      WorksheetImpl worksheet = sheet as WorksheetImpl;

      double dWidth = 0;
      double dHeight = 0;

      dWidth = ( worksheet != null ) ?
        worksheet.GetColumnWidthInPixels( column ) :
        1;

      dWidth = dWidth * columnOffset / ( double )ShapeImpl.DEF_FULL_COLUMN_OFFSET;
      dWidth = Math.Round( ApplicationImpl.ConvertFromPixel( dWidth, MeasureUnits.EMU ) );
      column--;

      dHeight = ( worksheet != null ) ?
        worksheet.GetRowHeightInPixels( row ) :
        1;

      dHeight = Math.Round(dHeight * rowOffset / (double)ShapeImpl.DEF_FULL_ROW_OFFSET, 1);
      dHeight = (int)(ApplicationImpl.ConvertFromPixel(dHeight, MeasureUnits.EMU));
      row--;     

      writer.WriteStartElement( tagName, drawingsNamespace );

      // TODO: we have to find out how Excel 97 and 2007 evaluate shapes positions in charts and worksheets.

      writer.WriteElementString( Drawings.ColumnTagName, drawingsNamespace,
        column.ToString() );

      writer.WriteElementString( Drawings.ColumnOffsetTagName, drawingsNamespace,
        ( ( int )dWidth ).ToString() );

      writer.WriteElementString( Drawings.RowTagName, drawingsNamespace,
        row.ToString() );

      writer.WriteElementString( Drawings.RowOffsetTagName, drawingsNamespace,
        ( ( int )dHeight ).ToString() );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes XY anchor point
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="tagName">Tag name to use.</param>
    /// <param name="column">X coordinate (will be divided by 1000 in the current implementation).</param>
    /// <param name="row">Y coordinate (will be divided by 1000 in the current implementation).</param>
    /// <param name="drawingsNamespace">Xml namespace to use.</param>
    private void SerializeXYAnchor( XmlWriter writer, string tagName,
      int column, int row, string drawingsNamespace )
    {
      // TODO: we have to convert offsets into appropriate units.
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( tagName == null || tagName.Length == 0 )
        throw new ArgumentOutOfRangeException( "tagName" );

      writer.WriteStartElement( tagName, drawingsNamespace );

      // TODO: we have to find out how Excel 97 and 2007 evaluate shapes positions in charts and worksheets.
      if (column > 1000)
          column = 1000;
      string value = GetCoordinateValue( column );

      writer.WriteElementString( ChartConstants.XTagName, drawingsNamespace,
        value );

      if (row > 1000)
          row = 1000;
      value = GetCoordinateValue( row );

      writer.WriteElementString( ChartConstants.YTagName, drawingsNamespace,
        value );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Converts XY Anchor coordinate into string value for serialization.
    /// </summary>
    /// <param name="coordinate">Coordinate to convert.</param>
    /// <returns>Converted value.</returns>
    private string GetCoordinateValue( int coordinate )
    {
      return XmlConvert.ToString( coordinate / ChartConstants.CoordinatesMultiplyer );
    }
    /// <summary>
    /// Serializes Transform2D tag (xfrm).
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="xmlOuterNamespace">Namespace for main (outer) tag.</param>
    /// <param name="xmlInnerNamespace">Namespace for subtags (inner).</param>
    /// <param name="x">X coordinate of the form.</param>
    /// <param name="y">X coordinate of the form.</param>
    /// <param name="cx">Width of the form.</param>
    /// <param name="cy">Height of the form.</param>
    public static void SerializeForm( XmlWriter writer, string xmlOuterNamespace,
      string xmlInnerNamespace, int x, int y, int cx, int cy )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartElement( Drawings.Transform2DTag, xmlOuterNamespace );
      writer.WriteStartElement( Drawings.Offset, xmlInnerNamespace );
      writer.WriteAttributeString( Drawings.XAttributeName, x.ToString() );
      writer.WriteAttributeString( Drawings.YAttributeName, y.ToString() );
      writer.WriteEndElement();

      writer.WriteStartElement( Drawings.Extents, xmlInnerNamespace );
      writer.WriteAttributeString( Drawings.CXAttributeName, cx.ToString() );
      writer.WriteAttributeString( Drawings.CYAttributeName, cy.ToString() );
      writer.WriteEndElement();

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes the form.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="xmlOuterNamespace">The XML outer namespace.</param>
    /// <param name="xmlInnerNamespace">The XML inner namespace.</param>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="cx">The cx.</param>
    /// <param name="cy">The cy.</param>
    /// <param name="shape">The shape.</param>
    public static void SerializeForm(XmlWriter writer, string xmlOuterNamespace,
  string xmlInnerNamespace, int x, int y, int cx, int cy,IShape shape)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        writer.WriteStartElement(Drawings.Transform2DTag, xmlOuterNamespace);
        if (shape.ShapeRotation != 0)
            writer.WriteAttributeString(Drawings.RotationAttribute, (shape.ShapeRotation * 60000).ToString());
        writer.WriteStartElement(Drawings.Offset, xmlInnerNamespace);
        writer.WriteAttributeString(Drawings.XAttributeName, x.ToString());
        writer.WriteAttributeString(Drawings.YAttributeName, y.ToString());
        writer.WriteEndElement();

        writer.WriteStartElement(Drawings.Extents, xmlInnerNamespace);
        writer.WriteAttributeString(Drawings.CXAttributeName, cx.ToString());
        writer.WriteAttributeString(Drawings.CYAttributeName, cy.ToString());
        writer.WriteEndElement();

        writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes non visual canvas properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize properties for.</param>
    /// <param name="holder">Object that stores data of the parent worksheet.</param>
    /// <param name="drawingsNamespace">Xml namespace to use.</param>
    protected void SerializeNVCanvasProperties( XmlWriter writer, ShapeImpl shape,
      WorksheetDataHolder holder, string drawingsNamespace )
    {
      writer.WriteStartElement( Drawings.NVCanvasPropertiesTag, drawingsNamespace );
      // TODO: change this.
      int id = shape.ShapeId;
      writer.WriteAttributeString( Drawings.IdAttributeName, id.ToString() );

      string strShapeName = shape.Name;

      writer.WriteAttributeString( Drawings.NameAttributeName, strShapeName );

      string strAlternativeText = shape.AlternativeText;

      if( strAlternativeText != null && strAlternativeText.Length > 0 )
        writer.WriteAttributeString( Drawings.DescriptionAttributeName, strAlternativeText );

      if( !shape.IsShapeVisible )
        writer.WriteAttributeString( Drawings.HiddenAttribute, Excel2007Serializator.TrueValue );

      if (shape.IsHyperlink)
      {
          writer.WriteStartElement(Drawings.ClickHyperlinkTag, Drawings.ANamespace);
          string rId=holder.DrawingsRelations.Add(shape.ImageRelation);
          writer.WriteAttributeString(Drawings.IdAttributeName, Excel2007Serializator.RelationNamespace, rId);

          writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes prstGeom xml tag.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    protected void SerializePresetGeometry( XmlWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      writer.WriteStartElement( Drawings.PresetGeometryTag, Drawings.ANamespace );
      writer.WriteAttributeString( Drawings.PresetShapeAttribute, "rect" );
      writer.WriteStartElement( Drawings.AdjustValuesList, Drawings.ANamespace );
      writer.WriteEndElement();
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes shape's fill.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize fill for.</param>
    /// <param name="holder">FileDataHolder object.</param>
    /// <param name="relations">Drawing relations.</param>
    protected void SerializeFill( XmlWriter writer, ShapeImpl shape, FileDataHolder holder,
      RelationCollection relations )
    {
      if( shape.HasFill )
      {
          IInternalFill fill = (IInternalFill)shape.Fill;

          if (fill.Visible)
          {
              ChartSerializatorCommon.SerializeFill(writer, fill, holder, relations);
          }
          else
          {
              writer.WriteStartElement(Drawings.NoFillTag, Drawings.ANamespace);
              writer.WriteEndElement();
          }
      }

      if( shape.HasLineFormat )
      {
        IShapeLineFormat lineFormat = shape.Line;
        SerializeLineSettings( writer, lineFormat, holder.Workbook );
        //ChartSerializator.SerializeLineSettings( writer, lineFormat.ForeColor, holder.Workbook );
      }
    }
    /// <summary>
    /// Serializes line settings.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="line">Line to serialize.</param>
    /// <param name="book">Parent workbook.</param>
    internal static void SerializeLineSettings( XmlWriter writer, IShapeLineFormat line, IWorkbook book )
    {
      writer.WriteStartElement( Drawings.LineTag, Drawings.ANamespace );

      int iWidth = ( int )( line.Weight * ShapeImpl.LineWieghtMultiplier );
      writer.WriteAttributeString( Drawings.LineWidthAttribute, iWidth.ToString() );

      Excel2007ShapeLineStyle lineStyle = ( Excel2007ShapeLineStyle )line.Style;
      writer.WriteAttributeString( Drawings.CompoundLineTypeAttribute, lineStyle.ToString() );

      if( line.Weight > 0 && line.Visible )
      {
        ChartSerializatorCommon.SerializeSolidFill( writer, line.ForeColor, false, book, 1 - line.Transparency );
      }
      else
      {
        writer.WriteElementString( Drawings.NoFillTag, Drawings.ANamespace, string.Empty );
      }

      ShapeLineFormatImpl lineFormat = line as ShapeLineFormatImpl;

      if( lineFormat.IsRound )
        writer.WriteElementString( Drawings.RoundTag, Drawings.ANamespace, string.Empty );
      if (line.DashStyle != ExcelShapeDashLineStyle.Solid)
      {
          string dash_Type = (book.Application as ApplicationImpl).StringEnum.LineDashTypeEnumToXml[line.DashStyle];          
          if (dash_Type != null && dash_Type.Length != 0)
              ChartSerializatorCommon.SerializeValueTag(writer, Drawings.PresetDashTag, Drawings.ANamespace, dash_Type);
      }
      SerializeArrowProperties(writer, lineFormat, true);
      SerializeArrowProperties(writer, lineFormat, false);
      writer.WriteEndElement();
    }
    private static void SerializeArrowProperties(XmlWriter writer, ShapeLineFormatImpl line, bool isHead)
    {
        string type = null;
        ExcelShapeArrowStyle obj2;
        ExcelShapeArrowWidth obj3;
        ExcelShapeArrowLength obj4;
        if (isHead)
        {
            type = "headEnd";
            obj2 = line.BeginArrowHeadStyle;
            obj3 = line.BeginArrowheadWidth;
            obj4 = line.BeginArrowheadLength;
        }
        else
        {
            type = "tailEnd";
            obj2 = line.EndArrowHeadStyle;
            obj3 = line.EndArrowheadWidth;
            obj4 = line.EndArrowheadLength;
        }
        writer.WriteStartElement(type, Drawings.ANamespace);
        writer.WriteAttributeString("type", GetArrowStyle(obj2));
        writer.WriteAttributeString("w", GetArrowWidth(obj3));
        writer.WriteAttributeString("len", GetArrowLength(obj4));
        writer.WriteEndElement();
    }

    private static string GetArrowLength(ExcelShapeArrowLength obj4)
    {
        switch (obj4)
        {
            case ExcelShapeArrowLength.ArrowHeadShort:
                return "sm";

            case ExcelShapeArrowLength.ArrowHeadMedium:
                return "med";

            case ExcelShapeArrowLength.ArrowHeadLong:
                return "lg";
        }
        return "med";
    }

    private static string GetArrowWidth(ExcelShapeArrowWidth obj3)
    {
        switch (obj3)
        {
            case ExcelShapeArrowWidth.ArrowHeadNarrow:
                return "sm";

            case ExcelShapeArrowWidth.ArrowHeadMedium:
                return "med";

            case ExcelShapeArrowWidth.ArrowHeadWide:
                return "lg";
        }
        return "med";
    }

    private static string GetArrowStyle(ExcelShapeArrowStyle obj2)
    {
        switch (obj2)
        {
            case ExcelShapeArrowStyle.LineNoArrow:
                return "none";

            case ExcelShapeArrowStyle.LineArrow:
                return "triangle";

            case ExcelShapeArrowStyle.LineArrowStealth:
                return "stealth";

            case ExcelShapeArrowStyle.LineArrowDiamond:
                return "diamond";

            case ExcelShapeArrowStyle.LineArrowOval:
                return "oval";

            case ExcelShapeArrowStyle.LineArrowOpen:
                return "arrow";
        }
        return "none";
    }
  }
}
