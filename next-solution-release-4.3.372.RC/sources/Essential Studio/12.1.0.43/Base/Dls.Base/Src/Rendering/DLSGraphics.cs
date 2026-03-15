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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

using Syncfusion.DLS.Collections;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Summary description for DLSGraphics.
  /// </summary>
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class DLSGraphics : CustomGraphics
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const float DEF_SCRIPT_FACTOR = 2f;
    #endregion
    
    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    public DLSGraphics()
    {}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    public DLSGraphics( Graphics graphics )
      : base( graphics )
    {}
    #endregion

    #region Class virtual methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paragraph"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawParagraph( Paragraph paragraph, LayoutedWidget ltWidget )
    {
      if( !paragraph.ParagraphFormat.Borders.NoBorder )
      {
        DrawBorders( paragraph.ParagraphFormat.Borders, ltWidget );
      }
      
      if( paragraph.ListFormat.ListType != ListType.NoList )
      {
        ListStyleCollection listStyles = paragraph.Document.ListStyles;
        int levelNumber = paragraph.ListFormat.ListLevelNumber;
        ListStyle listStyle = listStyles.FindByName( paragraph.ListFormat.CustomStyleName );
        ListLevel level = listStyle.GetNearLevel( levelNumber );
        LayoutParagraphInfo paragraphInfo = ltWidget.Widget.LayoutInfo as LayoutParagraphInfo;
        
        string symbol = level.GetListItemText( paragraphInfo.ListItemIndex );
        float max = paragraph.ParagraphFormat.FirstLineIndent;

        if( Math.Abs( max ) < Math.Abs( level.NumberPosition ) )
        {
          max = level.NumberPosition;
        }

        float x = ltWidget.Bounds.X + max;// - paragraph.ParagraphFormat.LeftIndent;
        float y = ltWidget.Bounds.Y;
        Font font;

        switch( listStyle.ListType )
        {
          case ListType.Bulleted:
            font = level.CharacterFormat.Font;
            break;
          default:
            font = paragraph.CharacterFormat.Font;
            break;
        }

        SolidBrush brush = new SolidBrush( paragraph.CharacterFormat.TextColor );
        Graphics.DrawString( symbol.ToString(), font, brush, x, y );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="setup"></param>
    /// <param name="image"></param>
    public virtual void DrawBackgroundImage( PageSetup setup, Image image )
    {
      Graphics.DrawImage( image, 0, 0, setup.PageSize.Width, setup.PageSize.Height );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="txtShape"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawTextShape( TextShape txtShape, LayoutedWidget ltWidget )
    {
      float x = txtShape.X + ltWidget.Bounds.X;
      float y = txtShape.Y + ltWidget.Bounds.Y;
      float fontSize = txtShape.CharacterFormat.Font.SizeInPoints;
      
      SolidBrush brush = new SolidBrush( txtShape.CharacterFormat.TextColor );
      //Graphics.DrawString( txtShape.Text, txtShape.CharacterFormat.Font, brush, x, y );
      DrawText( txtShape.Text, txtShape.CharacterFormat, brush, x, y, fontSize );
      
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawImageShape( ImageShape shape, LayoutedWidget ltWidget)
    {
      float x = shape.X + ltWidget.Bounds.X;
      float y = shape.Y + ltWidget.Bounds.Y;

      Graphics.DrawImage( shape.Image, x, y, ltWidget.Bounds.Width, ltWidget.Bounds.Height );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawArcShape( ArcShape shape, LayoutedWidget ltWidget )
    {
      float x = shape.X + ltWidget.Bounds.X;
      float y = shape.Y + ltWidget.Bounds.Y;
      
      // Draw arc border
      if( !shape.ShapeFormat.Line.NoLine )
      {
        using( Pen pen = CreatePen( shape.ShapeFormat ) )
        {
          Graphics.DrawArc( pen, x, y, shape.Bounds.Width, shape.Bounds.Height, 
            shape.StartAngle, shape.SweepAngle );
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawPolygonShape( PolygonShape shape, LayoutedWidget ltWidget )
    {
      PointF[] temporaryPoints = OffsetPoints( shape.Points, ltWidget.Bounds.Location );

      if( !shape.ShapeFormat.Fill.NoFill )
      {
        using( Brush brush = CreateBrush( shape.ShapeFormat.Fill, ltWidget.Bounds ) )
        {
          Graphics.FillPolygon( brush, temporaryPoints );
        }
      }
      
      if( !shape.ShapeFormat.Line.NoLine )
      {
        using( Pen pen = CreatePen( shape.ShapeFormat ) )
        {
          Graphics.DrawPolygon( pen, temporaryPoints );
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawPieShape( PieShape shape, LayoutedWidget ltWidget )
    {
      float x = shape.X + ltWidget.Bounds.X;
      float y = shape.Y + ltWidget.Bounds.Y;

      // Fill ellipse
      if( !shape.ShapeFormat.Fill.NoFill )
      {
        using( Brush brush = CreateBrush( shape.ShapeFormat.Fill, ltWidget.Bounds ) )
        {
          Graphics.FillPie( brush, x, y, shape.Bounds.Width, shape.Bounds.Height, 
            shape.StartAngle, shape.SweepAngle );
        }
      }
      
      // Draw ellipse border
      if( !shape.ShapeFormat.Line.NoLine )
      {
        using( Pen pen = CreatePen( shape.ShapeFormat ) )
        {
          Graphics.DrawPie( pen, x, y, shape.Bounds.Width, shape.Bounds.Height, 
            shape.StartAngle, shape.SweepAngle );
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawRectangleShape( RectangleShape shape, LayoutedWidget ltWidget )
    {
      float x = shape.X + ltWidget.Bounds.X;
      float y = shape.Y + ltWidget.Bounds.Y;

      // Fill rectangle      
      if( !shape.ShapeFormat.Fill.NoFill )
      {
        using( Brush brush = CreateBrush( shape.ShapeFormat.Fill, ltWidget.Bounds ) )
        {
          Graphics.FillRectangle( brush, x, y, shape.Width, shape.Height );
        }
      }
      
      // Draw rectangle border
      if( !shape.ShapeFormat.Line.NoLine )
      {
        using( Pen pen = CreatePen( shape.ShapeFormat ) )
        {
          Graphics.DrawRectangle( pen, x, y, shape.Width, shape.Height );
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawEllipseShape( EllipseShape shape, LayoutedWidget ltWidget )
    {
      float x = shape.X + ltWidget.Bounds.X;
      float y = shape.Y + ltWidget.Bounds.Y;

      // Fill ellipse
      if( !shape.ShapeFormat.Fill.NoFill )
      {
        using( Brush brush = CreateBrush( shape.ShapeFormat.Fill, ltWidget.Bounds ) )
        {
          Graphics.FillEllipse( brush, x, y, shape.Bounds.Width, shape.Bounds.Height );
        }
      }
      
      // Draw ellipse border
      if( !shape.ShapeFormat.Line.NoLine )
      {
        using( Pen pen = CreatePen( shape.ShapeFormat ) )
        {
          Graphics.DrawEllipse( pen, x, y, shape.Bounds.Width, shape.Bounds.Height );
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawLineShape( LineShape shape, LayoutedWidget ltWidget )
    {
      float x  = shape.X1 + ltWidget.Bounds.X;
      float y  = shape.Y1 + ltWidget.Bounds.Y;
      float x1 = shape.X2 + ltWidget.Bounds.X;
      float y1 = shape.Y2 + ltWidget.Bounds.Y;

      if( !shape.ShapeFormat.Line.NoLine )
      {
        using( Pen pen = CreatePen( shape.ShapeFormat ) )
        {
          Graphics.DrawLine( pen, x, y, x1, y1 );
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawBezierShape( BezierShape shape, LayoutedWidget ltWidget )
    {
      float x1 = shape.X1 + ltWidget.Bounds.X;
      float y1 = shape.Y1 + ltWidget.Bounds.Y;
      float x2 = shape.X2 + ltWidget.Bounds.X;
      float y2 = shape.Y2 + ltWidget.Bounds.Y;
      float x3 = shape.X3 + ltWidget.Bounds.X;
      float y3 = shape.Y3 + ltWidget.Bounds.Y;
      float x4 = shape.X4 + ltWidget.Bounds.X;
      float y4 = shape.Y4 + ltWidget.Bounds.Y;

      if( !shape.ShapeFormat.Line.NoLine )
      {
        using( Pen pen = CreatePen( shape.ShapeFormat ) )
        {
          Graphics.DrawBezier( pen, x1, y1, x2, y2, x3, y3, x4, y4 );
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="picture"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawPicture( Picture picture, LayoutedWidget ltWidget )
    {
      //UnitsConvertor convertor = new UnitsConvertor( Graphics );
      SizeF size = MeasurePicture( picture );
      //size = new SizeF( (size.Width * picture.WidthScale) / 100, (size.Height * picture.HeightScale) / 100 );
      //RectangleF srcRect = new RectangleF( new PointF( 0 , 0 ), picture.Image.Size );      
      
      /*Graphics.DrawImage( 
        picture.Image, 
        ltWidget.Bounds.X,
        ltWidget.Bounds.Y,
        size.Width,
        size.Height );
       */
      RectangleF bounds = ltWidget.Bounds;

      if( float.IsNaN( bounds.X ) )
      {
        bounds.X = 0;
      }

      Graphics.SetClip( bounds );
      Graphics.DrawImage( picture.Image, bounds.X, bounds.Y, size.Width,
        size.Height   );
      Graphics.ResetClip();

#if DEBUG_RENDERING
      DrawBounds( Color.Violet, ltWidget.Bounds );
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="txtRange"></param>
    /// <param name="ltWidget"></param>
    /// <param name="text"></param>
    public virtual void DrawTextRange( TextRange txtRange, LayoutedWidget ltWidget, string text )
    {
      double x = ltWidget.Bounds.Left;
      double y = ltWidget.Bounds.Top;
      
#if DEBUG_LAYOUTING
      Pen pen = new Pen( Color.Gray );
      Graphics.DrawRectangle( pen, lWidget.Bounds.X, lWidget.Bounds.Y,
        lWidget.Bounds.Width, lWidget.Bounds.Height );
#endif      

      CharacterFormat format = txtRange.CharacterFormat;
      SolidBrush brush = new SolidBrush( format.TextColor );
      float fontSize = format.Font.SizeInPoints;
      
      switch( format.SubSuperScript )
      {
        case SubSuperScript.None:
          break;
        case SubSuperScript.SubScript:
          // NOTE: Empiric approximate subscript dimensions, will change later
          double ascent = txtRange.GetTextAscent( this );
          fontSize /= DEF_SCRIPT_FACTOR;
          y = y + ascent - ( ascent / DEF_SCRIPT_FACTOR ) - ascent*0.10f;
          break;
        case SubSuperScript.SuperScript:
          // NOTE: Empiric approximate superscript dimensions, will change later
          y = y + txtRange.GetTextAscent( this )*0.10f;
          fontSize /= DEF_SCRIPT_FACTOR;
          break;
      }

      DrawText( text, format, brush, ( float )x, ( float )y, fontSize );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="format"></param>
    /// <param name="brush"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="fontSize"></param>
    protected virtual void DrawText( string text, CharacterFormat format, Brush brush, float x, float y, float fontSize )
    {
      Font font = format.Font;
      
      if( fontSize != font.SizeInPoints || format.DoubleStrike )
      {
        FontStyle fontStyle = format.DoubleStrike ? FontStyle.Strikeout : font.Style;
        font = new Font( font.Name, fontSize, fontStyle, GraphicsUnit.Point );
      }

      // Draw text background
      if( format.TextBackgroundColor != Color.Empty &&
        format.TextBackgroundColor != Color.White )
      {
        SolidBrush bkBrush = new SolidBrush( format.TextBackgroundColor );
        SizeF size = Graphics.MeasureString( text, font );
        
        Graphics.FillRectangle( bkBrush, x, y, size.Width, size.Height );
      }
      
      Graphics.DrawString( text, font, brush, x, y );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawPathShape( PathShape shape, LayoutedWidget ltWidget )
    {
      PointF[] pathPoints = OffsetPoints( shape.PathPoints, ltWidget.Bounds.Location );
      GraphicsPath path = shape.ToGraphicsPath( pathPoints, shape.PathTypes, shape.FillMode );
      
      // Fill rectangle      
      if( !shape.ShapeFormat.Fill.NoFill )
      {
        using( Brush brush = CreateBrush( shape.ShapeFormat.Fill, ltWidget.Bounds ) )
        {
          Graphics.FillPath( brush, path );
        }
      }
      
      // Draw rectangle border
      if( !shape.ShapeFormat.Line.NoLine )
      {
        using( Pen pen = CreatePen( shape.ShapeFormat ) )
        {
          Graphics.DrawPath( pen, path );
        }
      }

      path.Dispose();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="picture"></param>
    /// <returns></returns>
    public virtual SizeF MeasurePicture( Picture picture )
    {
      return new SizeF( 
        (picture.Size.Width * picture.WidthScale) / 100, 
        (picture.Size.Height * picture.HeightScale) / 100 
        );
      //UnitsConvertor convertor = new UnitsConvertor( Graphics );
      //return convertor.ConvertFromPixels( picture.Image.Size, PrintUnits.Point );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="txtRange"></param>
    /// <returns></returns>
    public virtual SizeF MeasureTextRange( TextRange txtRange, string text )
    {
      Font font = txtRange.CharacterFormat.Font;
      SizeF size = Graphics.MeasureString( text, font );
      
      if( txtRange.CharacterFormat.SubSuperScript == SubSuperScript.SuperScript ||
          txtRange.CharacterFormat.SubSuperScript == SubSuperScript.SubScript )
      {
        size.Width = size.Width / DEF_SCRIPT_FACTOR;
      }
        
      return size;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="txtRange"></param>
    /// <returns></returns>
    public virtual float GetAscentTextRange( TextRange txtRange )
    {
      FontMetric fMetric = new FontMetric( txtRange.CharacterFormat.Font, Graphics );
      return (float)fMetric.Ascent;
    }
    /// <summary>
    /// Draws canvas object. This method is called just before shapes drawing.
    /// </summary>
    /// <param name="canvas">Canvas object.</param>
    /// <param name="ltWidget">Object representing area of the canvas in the document.</param>
    public virtual void DrawCanvas( ICanvas canvas, LayoutedWidget ltWidget )
    {
      if( canvas == null )
        throw new ArgumentNullException( "canvas" );
      if( ltWidget == null )
        throw new ArgumentNullException( "ltWidget" );

      // Draw bounds.
      if( !canvas.BorderColor.IsEmpty )
      {
        DrawBounds( canvas.BorderColor, ltWidget.Bounds );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <param name="rect"></param>
    protected internal virtual void DrawBounds( Color color, RectangleF rect )
    {
      using( Pen pen = new Pen( color ) )
      {
        Graphics.DrawRectangle( pen, Rectangle.Ceiling( rect ) );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="borders"></param>
    /// <param name="ltWidget"></param>
    protected internal virtual void DrawBorders( Borders borders, LayoutedWidget ltWidget )
    {
      RectangleF bounds = ltWidget.Bounds;

      // left border
      DrawBorder( borders.Left,
                  new PointF( bounds.Left, bounds.Top ),
                  new PointF( bounds.Left, bounds.Bottom ) );

      // top border
      DrawBorder( borders.Top,
                  new PointF( bounds.Left, bounds.Top ),
                  new PointF( bounds.Right, bounds.Top ) );

      // right border
      DrawBorder( borders.Right,
                  new PointF( bounds.Right, bounds.Top ),
                  new PointF( bounds.Right, bounds.Bottom ) );

      // bottom border
      DrawBorder( borders.Bottom,
                  new PointF( bounds.Left, bounds.Bottom ),
                  new PointF( bounds.Right, bounds.Bottom ) );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="border"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    protected internal virtual void DrawBorder( Border border, PointF start, PointF end )
    {
      if( border.BorderType != BorderStyle.None )
      {
        Color penColor = Color.Black;
        
        if( !border.Color.IsEmpty && border.Color.ToArgb() != 0 )
        {
          penColor = border.Color;
        }

        Pen pen = new Pen( penColor, border.LineWidth );
        /*
          case BorderStyle.Thick:
            break;
          case BorderStyle.Double:
            break;
          case BorderStyle.Hairline:
            break;
          case BorderStyle.Triple:
            break;
          case BorderStyle.ThinThickSmallGap:
            break;
          case BorderStyle.ThinThinSmallGap:
            break;
          case BorderStyle.ThinThickThinSmallGap:
            break;
          case BorderStyle.ThinThickMediumGap:
            break;
          case BorderStyle.ThickThinMediumGap:
            break;
          case BorderStyle.ThickThickThinMediumGap:
            break;
          case BorderStyle.ThinThickLargeGap:
            break;
          case BorderStyle.ThickThinLargeGap:
            break;
          case BorderStyle.ThinThickThinLargeGap:
            break;
          case BorderStyle.Wave:
            break;
          case BorderStyle.DoubleWave:
            break;
          case BorderStyle.DashDotStroker:
            break;
          case BorderStyle.Emboss3D:
            break;
          case BorderStyle.Engrave3D:
            break;
*/
        switch( border.BorderType )
        {
          case BorderStyle.Dot:
            pen.DashStyle = DashStyle.Dot;
            break;
          case BorderStyle.DashLargeGap: //TODO: separate later
          case BorderStyle.DashSmallGap:
            pen.DashStyle = DashStyle.Dash;
            break;
          case BorderStyle.DotDash:
            pen.DashStyle = DashStyle.DashDot;
            break;
          case BorderStyle.DotDotDash:
            pen.DashStyle = DashStyle.DashDotDot;
            break;
          case BorderStyle.Single:
          default:
            break;
        }
        
        pen.StartCap = LineCap.Square;
        pen.EndCap = LineCap.Square;
        Graphics.DrawLine( pen, start, end );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="table"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawTable( Table table, LayoutedWidget ltWidget )
    {
      if( table.TableFormat.CellSpacing > -1 )
      {
        DrawBorders( table.TableFormat.Borders, ltWidget );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawTableRow( TableRow row, LayoutedWidget ltWidget )
    {
      //DrawBounds( Color.Black, ltWidget.Bounds );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="ltWidget"></param>
    public virtual void DrawTableCell( TableCell cell, LayoutedWidget ltWidget )
    {
      if( cell == null )
        throw new ArgumentNullException( "cell" );
      if( ltWidget == null )
        throw new ArgumentNullException( "ltWidget" );
      
      if( !cell.CellFormat.BackColor.IsEmpty )
      {
        Graphics.FillRectangle( new SolidBrush( cell.CellFormat.BackColor ),
          ltWidget.Bounds );
      }
        
      DrawCellBorders( cell, ltWidget );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="ltWidget"></param>
    protected internal virtual void DrawCellBorders( TableCell cell, LayoutedWidget ltWidget )
    {
      RectangleF bounds = ltWidget.Bounds;
      Borders cellBorders = cell.CellFormat.Borders;
      TableFormat tableFormat = cell.OwnerRow.OwnerTable.TableFormat;
      Borders tableBorders = tableFormat.Borders;
      DefaultBorders defaultBorders = new DefaultBorders( tableFormat );

      int cellIndex = cell.GetCellIndex();
      int rowIndex = cell.OwnerRow.GetRowIndex();
      int cellLast = cell.OwnerRow.Cells.Count - 1;
      int rowlLast = cell.OwnerRow.OwnerTable.Rows.Count - 1;
      int nextCellIndex = cellIndex;

      for( int i = cellIndex+1; i < cellLast+1; i++ )
      {
        if( cell.OwnerRow.Cells[ i ].CellFormat.HorizontalMerge != CellMerge.Continue )
        {
          nextCellIndex = i;
          break;
        }
      }
      
      bool bNoSpacing = tableFormat.CellSpacing < 0;
      bool bNotDrawRight = bNoSpacing && cellIndex != cellLast;
      bNotDrawRight &= ( nextCellIndex != cellIndex );
      bool bNotDrawBottom = bNoSpacing && rowIndex != rowlLast;
      LayoutTableInfo tableInfo = ltWidget.Widget.LayoutInfo as LayoutTableInfo;

      bool bRowSplitted = tableInfo.IsRowSplitted;

      #region Draw left border
      Border leftBorder = ( cellBorders.Left.BorderType != BorderStyle.None )
                      ? cellBorders.Left
                      : ( cellIndex == 0 ) ? tableBorders.Left : defaultBorders.VerticalBorder;
      DrawBorder( leftBorder,
                  new PointF( bounds.Left, bounds.Top ),
                  new PointF( bounds.Left, bounds.Bottom ) );
      #endregion

      #region Draw top border
      Border topBorder = ( cellBorders.Top.BorderType != BorderStyle.None )
                         ? cellBorders.Top
                         : ( rowIndex == 0 ) ? tableBorders.Top : defaultBorders.HorizontalBorder;
      DrawBorder( topBorder,
                  new PointF( bounds.Left, bounds.Top ),
                  new PointF( bounds.Right, bounds.Top ) );
      #endregion

      #region Draw right border
      if( !bNotDrawRight )
      {
        // right border
        Border rightBorder = ( cellBorders.Right.BorderType != BorderStyle.None )
                               ? cellBorders.Right
                               : ( cellIndex == cellLast ) ? tableBorders.Right : defaultBorders.VerticalBorder;
        DrawBorder( rightBorder,
                    new PointF( bounds.Right, bounds.Top ),
                    new PointF( bounds.Right, bounds.Bottom ) );
      }
      #endregion

      #region Draw bottom border
      if( !bNotDrawBottom || bRowSplitted || tableInfo.IsExactlyRowHeight )
      {
        // bottom border
        Border bottomBorder = ( cellBorders.Bottom.BorderType != BorderStyle.None )
                                ? cellBorders.Bottom
                                : ( rowIndex == rowlLast ) ? tableBorders.Bottom : defaultBorders.HorizontalBorder;
        DrawBorder( bottomBorder,
                    new PointF( bounds.Left, bounds.Bottom ),
                    new PointF( bounds.Right, bounds.Bottom ) );
      }
      #endregion 
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="lineData"></param>
    /// <returns></returns>
    private Pen CreatePen( LineData lineData )
    {
      Pen pen = new Pen( lineData.LineColor );
      pen.Width = lineData.LineWidth;
      pen.DashStyle = lineData.DashStyle;
      return pen;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    private Pen CreatePen( ShapeFormat format )
    {
      return CreatePen( format.Line );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private Brush CreateBrush( FillData fillData, RectangleF rect )
    {
      if( !fillData.GradientFill )
      {
        return new SolidBrush( fillData.Color );
      }
      else
      {
        return new LinearGradientBrush( rect,
                                        fillData.GradientColorStart, fillData.GradientColorEnd,
                                        fillData.GradientMode );
      }
    }
    /// <summary>
    /// Makes offset of each point in the array by start point coordinates.
    /// </summary>
    /// <param name="points">Array of points.</param>
    /// <param name="startPoint">Start point.</param>
    /// <returns>Array of points with offset.</returns>
    protected PointF[] OffsetPoints( PointF[] points, PointF startPoint )
    {
      if( points == null )
        throw new ArgumentNullException( "points" );

      PointF[] result = new PointF[ points.Length ];

      for( int i = 0, len = points.Length; i < len; i++ )
      {
        PointF curPoint = points[ i ];

        curPoint.X += startPoint.X;
        curPoint.Y += startPoint.Y;

        result[ i ] = curPoint;
      }

      return result;
    }
    #endregion

    #region Class internal declaration
    /// <summary>
    /// 
    /// </summary>
    internal class DefaultBorders
      : TableFormat
    {
      #region Class properties
      /// <summary>
      /// 
      /// </summary>
      public Border Vertical
      {
        get
        {
          return VerticalBorder;
        }
      }
      /// <summary>
      /// 
      /// </summary>
      public Border Horisontal
      {
        get
        {
          return HorizontalBorder;
        }
      }
      #endregion

      #region Class Initialise / Finalise
      /// <summary>
      /// 
      /// </summary>
      /// <param name="format"></param>
      public DefaultBorders( TableFormat format )
      {
        InitBorder( base.HorizontalBorder, format.HorizontalBorder );
        InitBorder( base.VerticalBorder, format.VerticalBorder );
      }
      #endregion

      #region Class helper methods
      /// <summary>
      /// 
      /// </summary>
      /// <param name="destination"></param>
      /// <param name="sourse"></param>
      private void InitBorder( Border destination, Border sourse )
      {
        destination.BorderType = sourse.BorderType;
        destination.Color = sourse.Color;
        destination.LineWidth = sourse.LineWidth;
        destination.Shadow = sourse.Shadow;
        destination.Space = sourse.Space;
      }

      #endregion
    }
    #endregion
  }
}
