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
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Interfaces;
using System.IO;

#if !(SILVERLIGHT) && !(WINRT) && !(WP)
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Syncfusion.XlsIO.Implementation.PivotTables;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This class is used to convert worksheet into image.
  /// </summary>
  class WorksheetImageConverter
  {
#region Members
    /// <summary>
    /// Object that helps to get formatting of the cells with conditional formats.
    /// </summary>
    private CFApplier m_cfApplier = new CFApplier();
    /// <summary>
    /// Represents the final resultant image.
    /// </summary>
    private Image result;

    /// <summary>
    /// Represents the RightToLeft Worksheet.
    /// </summary>
    private bool EnableRTL;
    PivotTableImpl pivotImpl = null;
    IRange pivotTableRange;

    #endregion

#region Methods
    /// <summary>
    /// Converts worksheet into image.
    /// </summary>
    /// <param name="sheet">Worksheet to convert.</param>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <returns>Image containing worksheet data.</returns>
    public Image ConvertToImage( WorksheetImpl sheet, int firstRow, int firstColumn, int lastRow, int lastColumn )
    {
      return ConvertToImage( sheet, firstRow, firstColumn, lastRow, lastColumn, ImageType.Bitmap, null );
    }
    /// <summary>
    /// Converts worksheet into image.
    /// </summary>
    /// <param name="sheet">Worksheet to convert.</param>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="emfType">Metafile EmfType.</param>
    /// <param name="outputStream">Output stream, if null it is ignored.</param>
    /// <returns>Image containing worksheet data.</returns>
    public Image ConvertToImage( WorksheetImpl sheet, int firstRow, int firstColumn, int lastRow, int lastColumn,
      EmfType emfType, Stream outputStream )
    {
      return ConvertToImage( sheet, firstRow, firstColumn, lastRow, lastColumn,
        ImageType.Metafile, outputStream, emfType );
    }
    /// <summary>
    /// Converts worksheet into image.
    /// </summary>
    /// <param name="sheet">Worksheet to convert.</param>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="imageType">Type of the image to create.</param>
    /// <param name="outputStream">Output stream, if null it is ignored.</param>
    /// <returns>Image containing worksheet data.</returns>
    public Image ConvertToImage( WorksheetImpl sheet, int firstRow, int firstColumn, int lastRow, int lastColumn,
      ImageType imageType, Stream outputStream )
    {
      return ConvertToImage( sheet, firstRow, firstColumn, lastRow, lastColumn,
        imageType, outputStream, EmfType.EmfOnly );
    }
    /// <summary>
    /// Converts worksheet into image.
    /// </summary>
    /// <param name="sheet">Worksheet to convert.</param>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="imageType">Type of the image to create.</param>
    /// <param name="outputStream">Output stream, if null it is ignored.</param>
    /// <param name="emfType">Metafile EmfType.</param>
    /// <returns>Image containing worksheet data.</returns>
    public Image ConvertToImage( WorksheetImpl sheet, int firstRow, int firstColumn, int lastRow, int lastColumn,
      ImageType imageType, Stream outputStream, EmfType emfType )
    {
      // 1. Image size
      ItemSizeHelper rowHeightGetter = new ItemSizeHelper( sheet.GetRowHeightInPixels );
      ItemSizeHelper columnWidthGetter = new ItemSizeHelper( sheet.GetColumnWidthInPixels );
      int iHeight = rowHeightGetter.GetTotal( firstRow, lastRow );
      int iWidth = columnWidthGetter.GetTotal( firstColumn, lastColumn );
      if (sheet.IsRightToLeft)
      {
          this.EnableRTL = true;
      }
      sheet.EnableSheetCalculations();
        
        //Pivot table layout creation
        if (sheet.PivotTables.Count > 0)
        {
            for (int index = 0; index < sheet.PivotTables.Count; index++)
            {
                IPivotTable pivotTable = sheet.PivotTables[index];
#if !SyncfusionFramework2_0
                pivotTable.Layout();
#endif
                pivotImpl = pivotTable as PivotTableImpl;
                pivotTableRange = pivotTable.Location;
            }
        }

        result = CreateImage( iWidth, iHeight, imageType, outputStream, emfType );
      
        //CreateBitmap( firstRow, firstColumn, lastRow, lastColumn, rowHeightGetter, columnWidthGetter );
      ConvertToImage( result, sheet, firstRow, firstColumn, lastRow, lastColumn,
        rowHeightGetter, columnWidthGetter, iWidth, iHeight );
      sheet.DisableSheetCalculations();

      if (imageType == ImageType.Bitmap && outputStream != null)
      {
      	ImageFormat format = ( result.RawFormat.Equals( ImageFormat.MemoryBmp ) ) ?
      	  ImageFormat.Bmp :
      	  result.RawFormat;

        result.Save( outputStream, format );
      }
      return result;
    }

    private void ConvertToImage( Image image, WorksheetImpl sheet, int firstRow, int firstColumn,
      int lastRow, int lastColumn, ItemSizeHelper rowHeightGetter, ItemSizeHelper columnWidthGetter,
      int width, int height)
    {
      using( Graphics graphics = Graphics.FromImage( image ) )
      {
        graphics.FillRectangle( Brushes.White, new Rectangle( 0, 0, width, height ) );

          DrawBackgroundImage(sheet, graphics, firstRow, firstColumn, lastRow, lastColumn,
       rowHeightGetter, columnWidthGetter);

          DrawGridlines(sheet, firstRow, firstColumn, lastRow, lastColumn, graphics,
            rowHeightGetter, columnWidthGetter, width, height);        

        IterateCells( sheet, firstRow, firstColumn, lastRow, lastColumn, graphics,
          rowHeightGetter, columnWidthGetter, DrawBackground );

        IterateMerges( sheet, firstRow, firstColumn, lastRow, lastColumn,
          graphics, rowHeightGetter, columnWidthGetter, DrawMergeBackground );

        DrawCells( sheet, firstRow, firstColumn, lastRow, lastColumn, graphics,
          rowHeightGetter, columnWidthGetter );

        IterateMerges( sheet, firstRow, firstColumn, lastRow, lastColumn,
          graphics, rowHeightGetter, columnWidthGetter, DrawMerge );

        DrawImages(sheet, graphics, firstRow, firstColumn, lastRow, lastColumn,
          rowHeightGetter, columnWidthGetter);

        DrawShapes(sheet, graphics, firstRow, firstColumn, lastRow, lastColumn,
        rowHeightGetter, columnWidthGetter);
      }
    }
    /// <summary>
    /// Draws the shapes.
    /// </summary>
    /// <param name="sheet">Worksheet that is being converted into image.</param>
    /// <param name="graphics">Graphics to draw cells at.</param>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="rowHeightGetter">Helper object to get row height faster.</param>
    /// <param name="columnWidthGetter">Helper object to get column width faster.</param>
    private void DrawShapes(WorksheetImpl sheet, Graphics graphics, int firstRow, int firstColumn,
     int lastRow, int lastColumn, ItemSizeHelper rowHeightGetter, ItemSizeHelper columnWidthGetter)
    {
        MigrantRangeImpl cell = new MigrantRangeImpl( sheet.Application, sheet );
        ITextBoxShape shape;
        if (sheet.TextBoxes != null)
        {
            IFont font;
            ITextBoxes textbox = sheet.TextBoxes;
            float sheetHeight = rowHeightGetter.GetTotal(firstRow, lastRow);
            float sheetWidth = columnWidthGetter.GetTotal(firstColumn, lastColumn); 
            int positionX = 0;
            int positionY = 0;
            float valueX = positionX + sheetWidth;
            float valueY = positionY + sheetHeight;
             System.Drawing.RectangleF rect;
             Font nativeFont = null;
      
            for (int i = 0; i < textbox.Count; i++)
            {
                shape = textbox[i];
                if (shape.IsShapeVisible)
                {                   
                    float left = (float)shape.Left - (((int)shape.Left / (int)sheetWidth) * sheetWidth);                   
                    float top = (float)shape.Top - (((int)shape.Top / (int)sheetHeight) * sheetHeight);
                    float height =(float)shape.Height;
                    float width = (float)shape.Width;
                    if (height != 0 && width != 0)
                    {
                        rect = new RectangleF(left, top, width, height);
                        if (shape.Top <= valueY && shape.Top >= positionY && shape.Left <= valueX
                            && shape.Left >= positionX)
                        {
                            Color color =shape.Fill.ForeColor;
                            if (shape.Fill.ForeColor.A == 0x00)
                                color = Color.White;

                            if (!String.IsNullOrEmpty(shape.RichText.Text))
                            {
                                cell.ResetRowColumn(firstRow, firstColumn);
                                int iXFIndex = cell.ExtendedFormatIndex;
                                ExtendedFormatImpl xf = cell.Workbook.InnerExtFormats[iXFIndex];
                                StringFormatFlags flags = xf.WrapText ? 0 :StringFormatFlags.NoWrap;
                                font = shape.RichText.GetFont(0);
                                StringFormat format = new StringFormat(flags);
                                format.Alignment = GetHorizontalAlignment(xf, cell);
                                format.LineAlignment = GetVerticalAlignment(xf);
                                format.Trimming = StringTrimming.None;
                                nativeFont = font.GenerateNativeFont();
                                if (shape.ShapeRotation != 0)
                                {
                                    GraphicsState state=graphics.Save();                                    
                                    PointF transformPoint = new PointF(0, 0);
                                    transformPoint.X = rect.X + rect.Width / 2;
                                    transformPoint.Y = rect.Y + rect.Height / 2;
                                    graphics.TranslateTransform(transformPoint.X, transformPoint.Y);
                                    graphics.RotateTransform((float)shape.ShapeRotation);
                                    RectangleF reverseAngle = new RectangleF(0, 0,rect.Height, rect.Width);
                                    Rectangle rectangle = new Rectangle(0, 0, (int)rect.Height, (int)rect.Width);
                                    Pen pen = new Pen(Color.Black);
                                    graphics.DrawRectangle(pen, rectangle);
                                  //  Brush brush = new SolidBrush (font.RGBColor);
                                    graphics.DrawString(shape.Text,nativeFont,new SolidBrush(font.RGBColor), reverseAngle,format);
                                    graphics.Restore(state);

                                }
                                else
                                {
                                    Rectangle rectangle = new Rectangle((int)rect.X,(int)rect.Y,(int)rect.Width, (int)rect.Height );
                                    Pen pen = new Pen(Color.Black);
                                    graphics.DrawRectangle(pen, rectangle);
                                    graphics.DrawString(shape.Text, nativeFont,
                                                        new SolidBrush (font.RGBColor ), rect,format);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Draws the background image
    /// </summary>
    /// <param name="sheet">Worksheet that is being converted into image.</param>
    /// <param name="graphics">Graphics to draw cells at.</param>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="rowHeightGetter">Helper object to get row height faster.</param>
    /// <param name="columnWidthGetter">Helper object to get column width faster.</param>
    private void DrawBackgroundImage(WorksheetImpl sheet, Graphics graphics, int firstRow, int firstColumn,
      int lastRow, int lastColumn, ItemSizeHelper rowHeightGetter, ItemSizeHelper columnWidthGetter)
    {
        if (sheet == null)
        {
            throw new ArgumentNullException("sheet");
        }
        if (sheet.PageSetup.BackgoundImage != null)
        {
            Dictionary<PointF, SizeF> imageCoordinates = new Dictionary<PointF, SizeF>();
            imageCoordinates = this.GetBackgroundWidthCoordinates(0, 0,
                                                                 (float)sheet.PageSetupBase.BackgoundImage.Width,
                                                                 (float)sheet.PageSetupBase.BackgoundImage.Height,
                                                                  imageCoordinates);

            if (imageCoordinates.Count != 0)
            {
                foreach (KeyValuePair<PointF, SizeF> coordinate in imageCoordinates)
                {
                    graphics.DrawImage(sheet.PageSetup.BackgoundImage, firstRow, firstColumn, result.Width, result.Height);
                }
            }
            else
            {
                graphics.DrawImage(sheet.PageSetup.BackgoundImage, 0, 0, result.Width, result.Height);                
            }
        }
    }
    /// <summary>
    /// Gets the background image width coordinates.
    /// </summary>
    /// <param name="startX">The start X.</param>
    /// <param name="startY">The start Y.</param>
    /// <param name="imageWidth">Width of the image.</param>
    /// <param name="imageHeight">Height of the image.</param>
    /// <param name="imageCoordinates">The image coordinates.</param>
    /// <param name="pdfPage">The PDF page.</param>
    /// <returns>
    /// The collection coordinates and the sizes of Background image width .
    /// </returns>
    protected Dictionary<PointF, SizeF> GetBackgroundWidthCoordinates(float startX, float startY,
                                                                      float imageWidth, float imageHeight,
                                                                      Dictionary<PointF, SizeF> imageCoordinates)
    {
        if (this.result.Width >= startX)
        {
            if (!imageCoordinates.ContainsKey(new PointF(startX, startY)))
            {
                imageCoordinates.Add(new PointF(startX, startY), new SizeF(imageWidth, imageHeight));
            }

            imageCoordinates = this.GetBackgroundHeightCoordinates(startX, startY, imageWidth, imageHeight,
                                                                   imageCoordinates);
            this.GetBackgroundWidthCoordinates(startX + imageWidth, startY, imageWidth, imageHeight,
                                               imageCoordinates);
        }      

        return imageCoordinates;
    }

    /// <summary>
    /// Gets the background height coordinates.
    /// </summary>
    /// <param name="startX">The start X.</param>
    /// <param name="startY">The start Y.</param>
    /// <param name="imageWidth">Width of the image.</param>
    /// <param name="imageHeight">Height of the image.</param>
    /// <param name="imageCoordinates">The image coordinates.</param>
    /// <param name="pdfPage">The PDF page.</param>
    /// <returns>
    /// The collection coordinates and the sizes of Background height.
    /// </returns>
    protected Dictionary<PointF, SizeF> GetBackgroundHeightCoordinates(float startX, float startY,
                                                                       float imageWidth, float imageHeight,
                                                                       Dictionary<PointF, SizeF> imageCoordinates)
    {
        if (this.result.Height >= startY)
        {
            if (!imageCoordinates.ContainsKey(new PointF(startX, startY)))
            {
                imageCoordinates.Add(new PointF(startX, startY), new SizeF(imageWidth, imageHeight));
            }

            this.GetBackgroundHeightCoordinates(startX, imageHeight + startY, imageWidth, imageHeight,
                                                    imageCoordinates);
        }
        return imageCoordinates;
    }
    /// <summary>
    /// Draws all required merged regions.
    /// </summary>
    /// <param name="sheet">Worksheet that is being converted into image.</param>
    /// <param name="firstRow">One-based index of the first row to export.</param>
    /// <param name="firstColumn">One-based index of the first column to export.</param>
    /// <param name="lastRow">One-based index of the last row to export.</param>
    /// <param name="lastColumn">One-based index of the last column to export.</param>
    /// <param name="graphics">Graphics object to draw at.</param>
    /// <param name="rowHeightGetter">Helper object to get row height faster.</param>
    /// <param name="columnWidthGetter">Helper object to get column width faster.</param>
    /// <param name="method">Method to call for each found merge.</param>
    private void IterateMerges( WorksheetImpl sheet, int firstRow, int firstColumn, int lastRow, int lastColumn,
      Graphics graphics, ItemSizeHelper rowHeightGetter, ItemSizeHelper columnWidthGetter, MergeMethod method )
    {
      if( sheet.HasMergedCells )
      {
        List<MergeCellsRecord.MergedRegion> lstRegions = new List<MergeCellsRecord.MergedRegion>();
        MergeCellsImpl mergedCells = sheet.MergeCells;
        mergedCells.CacheMerges( sheet[ firstRow, firstColumn, lastRow, lastColumn ], lstRegions );

        for( int i = 0, len = lstRegions.Count; i < len; i++ )
        {
          method( sheet, lstRegions[ i ], firstRow, firstColumn,
            graphics, rowHeightGetter, columnWidthGetter );
        }
      }
    }
    /// <summary>
    /// Draws merged region.
    /// </summary>
    /// <param name="sheet">Worksheet that is being converted into image.</param>
    /// <param name="mergedRegion">Merged region to draw.</param>
    /// <param name="firstRow">One-based index of the first row of the drawn range that contains part or whole merged range.</param>
    /// <param name="firstColumn">One-based index of the first column of the drawn range that contains part or whole merged range.</param>
    /// <param name="graphics">Graphics to draw at.</param>
    /// <param name="rowHeightGetter">Helper object to get row height faster.</param>
    /// <param name="columnWidthGetter">Helper object to get column width faster.</param>
    private void DrawMerge( WorksheetImpl sheet,
      MergeCellsRecord.MergedRegion mergedRegion,
      int firstRow,
      int firstColumn,
      Graphics graphics,
      ItemSizeHelper rowHeightGetter,
      ItemSizeHelper columnWidthGetter )
    {
      MergeCellsImpl mergedCells = sheet.MergeCells;
      ExtendedFormatImpl format = mergedCells.GetFormat( mergedRegion );
      Rectangle rect = GetMergeRectangle( sheet, mergedRegion, firstRow, firstColumn, rowHeightGetter, columnWidthGetter );
      if (this.EnableRTL)
      {
          rect.X = this.result.Width - rect.X;
          rect.X = rect.X - rect.Width;
      }

      IRange cell = sheet[ mergedRegion.RowFrom + 1, mergedRegion.ColumnFrom + 1 ];

      if( rect.Height > 0 && rect.Width > 0 )
        DrawCell( format, cell, rect, rect, graphics );
    }
    /// <summary>
    /// Gets coordinates of the merge.
    /// </summary>
    /// <param name="sheet">Worksheet containing merge region.</param>
    /// <param name="mergedRegion">Merge region to get rectangle for.</param>
    /// <param name="firstRow">One-based index of the first row of the drawn range that contains part or whole merged range.</param>
    /// <param name="firstColumn">One-based index of the first column of the drawn range that contains part or whole merged range.</param>
    /// <param name="rowHeightGetter">Helper object to get row height faster.</param>
    /// <param name="columnWidthGetter">Helper object to get column width faster.</param>
    /// <returns></returns>
    private Rectangle GetMergeRectangle( WorksheetImpl sheet,
      MergeCellsRecord.MergedRegion mergedRegion,
      int firstRow, int firstColumn, ItemSizeHelper rowHeightGetter, ItemSizeHelper columnWidthGetter )
    {
      // 1. Get region (or part of the region.
      int y = rowHeightGetter.GetTotal( mergedRegion.RowFrom );
      int x = columnWidthGetter.GetTotal( mergedRegion.ColumnFrom );

      y -= rowHeightGetter.GetTotal( firstRow - 1 );

      x -= columnWidthGetter.GetTotal( firstColumn - 1 );

      int height = rowHeightGetter.GetTotal( mergedRegion.RowFrom + 1, mergedRegion.RowTo + 1 );
      int width = columnWidthGetter.GetTotal( mergedRegion.ColumnFrom + 1, mergedRegion.ColumnTo + 1 );
      IRange cell = sheet[ mergedRegion.RowFrom + 1, mergedRegion.ColumnFrom + 1 ];
      //string value = cell.DisplayText;
      return new Rectangle( x, y, width, height );
    }
    /// <summary>
    /// Draws merged region background.
    /// </summary>
    /// <param name="sheet">Worksheet that is being converted into image.</param>
    /// <param name="mergedRegion">Merged region to draw.</param>
    /// <param name="firstRow">One-based index of the first row of the drawn range that contains part or whole merged range.</param>
    /// <param name="firstColumn">One-based index of the first column of the drawn range that contains part or whole merged range.</param>
    /// <param name="graphics">Graphics to draw at.</param>
    /// <param name="rowHeightGetter">Helper object to get row height faster.</param>
    /// <param name="columnWidthGetter">Helper object to get column width faster.</param>
    private void DrawMergeBackground( WorksheetImpl sheet,
      MergeCellsRecord.MergedRegion mergedRegion,
      int firstRow,
      int firstColumn,
      Graphics graphics,
      ItemSizeHelper rowHeightGetter,
      ItemSizeHelper columnWidthGetter )
    {
      MergeCellsImpl mergedCells = sheet.MergeCells;
      ExtendedFormatImpl format = mergedCells.GetFormat( mergedRegion );

      // 1. Get region (or part of the region.
      int y = rowHeightGetter.GetTotal( mergedRegion.RowFrom );
      int x = columnWidthGetter.GetTotal( mergedRegion.ColumnFrom );

      //if( mergedRegion.RowFrom <= firstRow )
      y -= rowHeightGetter.GetTotal( firstRow - 1 );

      //if( mergedRegion.ColumnFrom <= firstColumn )
      x -= columnWidthGetter.GetTotal( firstColumn - 1 );
      if (this.EnableRTL)
      {
          x = result.Width - x;
      }

      int height = rowHeightGetter.GetTotal( mergedRegion.RowFrom + 1, mergedRegion.RowTo + 1 );
      int width = columnWidthGetter.GetTotal( mergedRegion.ColumnFrom + 1, mergedRegion.ColumnTo + 1 );
      if (this.EnableRTL)
      {
          x = x - width;
      }
      IRange cell = sheet[ mergedRegion.RowFrom + 1, mergedRegion.ColumnFrom + 1 ];
      //string value = cell.DisplayText;
      Rectangle rect = new Rectangle( x, y, width, height );

      if( height > 0 && width > 0 )
        //DrawCell( format, cell, rect, rect, graphics );
        DrawBackground( format, rect, graphics,cell );
    }
    delegate void MergeMethod( WorksheetImpl sheet,
      MergeCellsRecord.MergedRegion mergedRegion,
      int firstRow,
      int firstColumn,
      Graphics graphics,
      ItemSizeHelper rowHeightGetter,
      ItemSizeHelper columnWidthGetter );
    delegate void CellMethod( IRange cell, Rectangle rect, Graphics graphics );
    /// <summary>
    /// Iterates through all cells and calls specified method for each.
    /// </summary>
    /// <param name="sheet">Worksheet that is being converted into image.</param>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="graphics">Graphics to draw cells at.</param>
    /// <param name="rowHeightGetter">Helper object to get row height faster.</param>
    /// <param name="columnWidthGetter">Helper object to get column width faster.</param>
    /// <param name="method">Method to call for each cell.</param>
    private void IterateCells( WorksheetImpl sheet, int firstRow, int firstColumn,
      int lastRow, int lastColumn, Graphics graphics,
      ItemSizeHelper rowHeightGetter, ItemSizeHelper columnWidthGetter, CellMethod method )
    {
      MigrantRangeImpl cell = new MigrantRangeImpl( sheet.Application, sheet );
      int x;
      int y;

      for( int iRow = firstRow; iRow <= lastRow; iRow++ )
      {
        y = rowHeightGetter.GetTotal( firstRow, iRow - 1 );
        int iHeight = rowHeightGetter.GetSize( iRow );

        if( iHeight > 0 )
        {
          for( int iColumn = firstColumn; iColumn <= lastColumn; iColumn++ )
          {
            // TODO: evaluate y.
            x = columnWidthGetter.GetTotal( firstColumn, iColumn - 1 );
            cell.ResetRowColumn( iRow, iColumn );
            int iWidth = columnWidthGetter.GetSize( iColumn );
            Rectangle rect = new Rectangle( x, y, iWidth, iHeight );

            method( cell, rect, graphics );
          }
        }
      }
    }
    /// <summary>
    /// Draws separate cell value.
    /// </summary>
    /// <param name="sheet">Worksheet that is being converted into image.</param>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="graphics">Graphics to draw cells at.</param>
    /// <param name="rowHeightGetter">Helper object to get row height faster.</param>
    /// <param name="columnWidthGetter">Helper object to get column width faster.</param>
    private void DrawCells( WorksheetImpl sheet, int firstRow, int firstColumn,
      int lastRow, int lastColumn, Graphics graphics,
      ItemSizeHelper rowHeightGetter, ItemSizeHelper columnWidthGetter )
    {
      MigrantRangeImpl cell = new MigrantRangeImpl( sheet.Application, sheet );
      MigrantRangeImpl cell2 = new MigrantRangeImpl( sheet.Application, sheet );
      int iLastPossibleColumn = cell.Workbook.MaxColumnCount;
      int x;
      int y;

      for( int iRow = firstRow; iRow <= lastRow; iRow++ )
      {
        y = rowHeightGetter.GetTotal( firstRow, iRow - 1 );
        int iHeight = rowHeightGetter.GetSize( iRow );

        if( iHeight > 0 )
        {
          for( int iColumn = firstColumn; iColumn <= lastColumn; iColumn++ )
          {
            // TODO: evaluate y.
            x = columnWidthGetter.GetTotal( firstColumn, iColumn - 1 );
            if (this.EnableRTL)
            {
                x = this.result.Width - x;
            }
            cell.ResetRowColumn( iRow, iColumn );

            if( !cell.IsMerged )
            {
              int iWidth = columnWidthGetter.GetSize( iColumn );
              if (this.EnableRTL)
              {
                  x = x - iWidth;
              }
              if( iWidth > 0 )
              {
                Rectangle rect = new Rectangle( x, y, iWidth, iHeight );
                Rectangle rect2 = GetAdjecentCells( cell, rect, columnWidthGetter,
                  firstColumn, cell2 );

                DrawCell( cell, rect, rect2, graphics );
              }
            }
          }
        }
      }
    }
    /// <summary>
    /// Adds adjacent cells to the cell's range if necessary.
    /// </summary>
    /// <param name="cell">Cell to try to add to.</param>
    /// <param name="rect">Original rectangle.</param>
    /// <param name="columnWidthGetter">Helper object to get column width faster.</param>
    /// <param name="firstColumn">Represents first column.</param>
    /// <param name="cell2">Represents second cell.</param>
    /// <returns>Updated rectangle.</returns>
    private Rectangle GetAdjecentCells( IRange cell, Rectangle rect, ItemSizeHelper columnWidthGetter,
      int firstColumn, MigrantRangeImpl cell2 )
    {
      Rectangle result = rect;

      if( !cell.IsBlank && !cell.WrapText && ( cell.HasString || cell.FormulaStringValue != null ) )
      {
        int iRow = cell.Row;
        int iColumn = cell.Column;
        int iLastColumn = iColumn;
        WorksheetImpl sheet = cell.Worksheet as WorksheetImpl;
        int iLastPossibleColumn = sheet.ParentWorkbook.MaxColumnCount;
        SizeF cellSize = sheet.MeasureCell( cell, false, false );
        int iCurrentWidth = columnWidthGetter.GetSize( iColumn );
        int iRequiredWidth = ( int )( cellSize.Width );

        // Do we need adjacent cells to display current cell's value?
        if( iRequiredWidth > iCurrentWidth )
        {
          IStyle style = cell.CellStyle;
          ExcelHAlign hAlign = style.HorizontalAlignment;
          int iDelta = 1;

          if( hAlign == ExcelHAlign.HAlignRight )
            iDelta = -1;

          cell2.ResetRowColumn( iRow, iLastColumn + iDelta );

          while( cell2.IsBlank &&
            iLastColumn < iLastPossibleColumn &&
            iLastColumn > 0 &&
            iRequiredWidth > iCurrentWidth )
          {
              int cell2Column = cell2.Column;
              if (cell2Column == 0)
              {
                  cell2Column = 1;
              }
              iCurrentWidth += columnWidthGetter.GetSize(cell2Column);
              iLastColumn += iDelta;
              cell2.ResetRowColumn( iRow, iLastColumn + iDelta );
          }

          int iFirstColumn = Math.Min( iLastColumn, cell.Column );
          if (iFirstColumn == 0)
              iFirstColumn = 1;
          int iNewLastColumn = Math.Max( iLastColumn, cell.Column );
          int newX = columnWidthGetter.GetTotal( firstColumn, iFirstColumn - 1 );
          int newWidth = columnWidthGetter.GetTotal( iFirstColumn, iNewLastColumn );
          if (this.EnableRTL)
          {
              newX = this.result.Width - newX;
              newX = newX - newWidth;
          }
          result = new Rectangle( newX, rect.Y, newWidth, rect.Height );
        }
      }

      return result;
    }
    /// <summary>
    /// Draws worksheet gridlines.
    /// </summary>
    /// <param name="sheet">Worksheet that is being converted into image.</param>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="graphics">Graphics to draw cells at.</param>
    /// <param name="rowHeightGetter">Helper object to get row height faster.</param>
    /// <param name="columnWidthGetter">Helper object to get column width faster.</param>
    /// <param name="width">Total image width.</param>
    /// <param name="height">Total image height.</param>
    private void DrawGridlines( WorksheetImpl sheet, int firstRow, int firstColumn, int lastRow, int lastColumn,
      Graphics graphics, ItemSizeHelper rowHeightGetter, ItemSizeHelper columnWidthGetter,
      int width, int height )
    {
      if( sheet.IsGridLinesVisible )
      {
        Pen pen = sheet.DefaultGridlineColor ?
          Pens.LightGray :
          new Pen( sheet.Workbook.GetPaletteColor( sheet.GridLineColor ) );

        graphics.DrawLine( pen, 0, 0, width, 0 );

        for( int i = firstRow, y = 0; i <= lastRow; i++ )
        {
          y += rowHeightGetter.GetSize( i );
          graphics.DrawLine( pen, 0, y, width, y );
        }

        graphics.DrawLine( pen, 0, 0, 0, height );
        for( int i = firstColumn, x = 0; i <= lastColumn; i++ )
        {
          x += columnWidthGetter.GetSize( i );
          graphics.DrawLine( pen, x, 0, x, height );
        }

        if( !sheet.DefaultGridlineColor )
          pen.Dispose();
      }
    }
    /// <summary>
    /// Draws all necessary images.
    /// </summary>
    /// <param name="sheet">Worksheet that is being converted into image.</param>
    /// <param name="graphics">Graphics to draw cells at.</param>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="rowHeightGetter">Helper object to get row height faster.</param>
    /// <param name="columnWidthGetter">Helper object to get column width faster.</param>
    private void DrawImages( WorksheetImpl sheet, Graphics graphics, int firstRow, int firstColumn,
      int lastRow, int lastColumn, ItemSizeHelper rowHeightGetter, ItemSizeHelper columnWidthGetter )
    {
      if( sheet.HasPictures )
      {
        IPictures pictures = sheet.Pictures;
        int x = columnWidthGetter.GetTotal( firstColumn - 1 );
        int y = rowHeightGetter.GetTotal( firstRow - 1 );
        int x2 = columnWidthGetter.GetTotal( lastColumn );
        int y2 = rowHeightGetter.GetTotal( lastRow );

        for( int i = 0, len = pictures.Count; i < len; i++ )
        {
          IPictureShape picture = pictures[ i ];

          // Is image visible?
          if( picture.Top <= y2 &&
            picture.Top + picture.Height >= y &&
            picture.Left <= x2 &&
            picture.Left + picture.Width >= x )
          {
            Rectangle pictureRect = new Rectangle( picture.Left, picture.Top, picture.Width, picture.Height );
            pictureRect.Offset( -x, -y );
            graphics.DrawImage( picture.Picture, pictureRect );
          }
        }
      }
    }
    /// <summary>
    /// Draws specified cell object.
    /// </summary>
    /// <param name="cell">Cell to draw.</param>
    /// <param name="rect">Rectangle to draw cell inside (borders).</param>
    /// <param name="rect2">Bounding rectangle for cell value.</param>
    /// <param name="graphics">Graphics to draw at.</param>
    private void DrawCell( MigrantRangeImpl cell, Rectangle rect, Rectangle rect2, Graphics graphics )
    {
      if( cell == null )
        throw new ArgumentNullException( "cell" );

      if( graphics == null )
        throw new ArgumentNullException( "graphics" );

      int iXFIndex = cell.ExtendedFormatIndex;
      ExtendedFormatImpl xf = cell.Workbook.InnerExtFormats[ iXFIndex ];
      DrawCell( xf, cell, rect, rect2, graphics );
    }
    /// <summary>
    /// Draws specified cell object.
    /// </summary>
    /// <param name="xf">Extended format to use for cell drawing.</param>
    /// <param name="cell">Cell to draw.</param>
    /// <param name="rect">Rectangle to draw cell inside (borders).</param>
    /// <param name="rect2">Bounding rectangle for cell value.</param>
    /// <param name="graphics">Graphics to draw at.</param>
    private void DrawCell( ExtendedFormatImpl xf, IRange cell, Rectangle rect, Rectangle rect2, Graphics graphics )
    {
        int intentLevel = xf.IndentLevel;
#if !SyncfusionFramework2_0
        int startRow = 1;
        int startCol = 1;
        for (int pivotCount = 0; pivotCount < cell.Worksheet.PivotTables.Count; pivotCount++)
        {
            if (cell.Row == cell.Worksheet.PivotTables[pivotCount].Location.Row && cell.Column == cell.Worksheet.PivotTables[pivotCount].Location.Column)
            {
                pivotTableRange = cell.Worksheet.PivotTables[pivotCount].Location;
                pivotImpl = cell.Worksheet.PivotTables[pivotCount] as PivotTableImpl;
            }
        }
        if (pivotTableRange != null)
        {
            startRow = pivotTableRange.Row;
            startCol = pivotTableRange.Column;
        }
        PivotTableStyleRenderer renderer = new PivotTableStyleRenderer(cell.Worksheet);
        if (cell.Worksheet.PivotTables.Count != 0 && pivotImpl.PageFields.Count != 0)
        {
            if (startRow - 2 == cell.Row && startCol == cell.Column)
            {
                xf = renderer.GetPageFilterLabel(pivotImpl.BuiltInStyle);
                xf.VerticalAlignment = ExcelVAlign.VAlignTop; ;
            }
            if (startRow - 2 == cell.Row && startCol + 1 == cell.Column)
            {
                xf = renderer.GetPageFilterValue(pivotImpl.BuiltInStyle);
                xf.VerticalAlignment = ExcelVAlign.VAlignTop;
            }
        }

        if (cell.Worksheet.PivotTables.Count != 0 && pivotTableRange != null && pivotTableRange.Row <= cell.Row && pivotTableRange.LastRow >= cell.LastRow
          && pivotTableRange.Column <= cell.Column && pivotTableRange.LastColumn >= cell.LastColumn)
        {
            xf = pivotImpl.PivotLayout[cell.Row - startRow, cell.Column - startCol].XF;
            xf.VerticalAlignment = ExcelVAlign.VAlignTop;
            if (xf.HorizontalAlignment == ExcelHAlign.HAlignGeneral && xf.HorizontalAlignment != cell.CellStyle.HorizontalAlignment)
            {
                xf.HorizontalAlignment = cell.CellStyle.HorizontalAlignment;
                if (intentLevel == 0)
                {
                    intentLevel = 10;
                }
                else
                {
                    intentLevel = intentLevel * 23;
                }
                xf.IndentLevel = intentLevel;
            }
        }
        else
#endif
            xf = m_cfApplier.ApplyCF( cell, xf );

      // 0. Background.
      Brush brush;

      // 1. Cell value.
      IFont font = xf.Font;
      Color fontColor = NormalizeColor( font.RGBColor );
      brush = new SolidBrush( fontColor );

      StringFormatFlags flags = xf.WrapText ?
        0 :
        StringFormatFlags.NoWrap;

      StringFormat format = new StringFormat( flags );
      format.Alignment = GetHorizontalAlignment( xf, cell );
      format.LineAlignment = GetVerticalAlignment( xf );
      format.Trimming = StringTrimming.None;

      string value = cell.DisplayText;
      value = UpdateToToBottomText( value, xf.Rotation );
      Font nativeFont = font.GenerateNativeFont();

      if( xf.Rotation != ExtendedFormatImpl.TopToBottomRotation && xf.Rotation != 0 )
      {
        DrawRotatedText( rect2, xf, cell, value, graphics, nativeFont, brush, format );
      }
#if !SyncfusionFramework4_0
      else if (cell.HasRichText)
      {
          DrawRtfText(rect2, cell, value, graphics);
      }
      else
      {
        graphics.DrawString( value, nativeFont, brush, rect2, format );
      }
#else
      else
      {
          graphics.DrawString(value, nativeFont, brush, rect2, format);
      }
#endif
      brush.Dispose();

      // 3. Borders
      //if( !cell.IsMerged )
      DrawBorders( xf.Borders, rect, graphics, cell );
    }

    /// <summary>
    /// Draws the RTF string (Rich Text Format string)
    /// </summary>    
    /// <param name="rect2">Bounding rectangle for cell value.</param>
    /// <param name="cell">Cell to draw.</param>
    /// <param name="value">Cell string value</param>
    /// <param name="graphics">Graphics to draw at.</param>
    private void DrawRtfText(Rectangle rect2, IRange cell,String value,Graphics graphics)
    {    
        if (value == string.Empty)
            throw new ArgumentNullException("text");

        Image image = new Bitmap(rect2.Width, rect2.Height);
        Graphics g = Graphics.FromImage(image);
        
        RichTextBoxPrintCtrl richTextBox=new RichTextBoxPrintCtrl();
        richTextBox.Rtf = cell.RichText.RtfText;
        richTextBox.PrintImage(0, g);
        
        graphics.DrawImage(image, rect2);
        g.Dispose();
    }
    /// <summary>
    /// Draws rotated text.
    /// </summary>
    /// <param name="rect2">Rectangle used for cell drawing.</param>
    /// <param name="xf">Cell's format.</param>
    /// <param name="cell">Cell to draw.</param>
    /// <param name="value"></param>
    /// <param name="graphics">Graphics object to draw at.</param>
    /// <param name="nativeFont">Represents native font.</param>
    /// <param name="brush">Represents brush to fill the interior.</param>
    /// <param name="format">Represents string format.</param>
    private void DrawRotatedText( Rectangle rect2, ExtendedFormatImpl xf, IRange cell, string value,
      Graphics graphics, Font nativeFont, Brush brush, StringFormat format )
    {
      GraphicsState state = graphics.Save();
      int x = rect2.X;
      int y = rect2.Y;

      Matrix matrix = new Matrix();
      //matrix.OffsetX = rect2.X;
      //matrix.OffsetY = rect2.Y;
      int rotationAngle = GetCounterClockwiseRotation( xf.Rotation );
      double dRadianAngle = rotationAngle * Math.PI / 180.0;

      int iRotationAngle = GetCounterClockwiseRotation( xf.Rotation );
      WorksheetImpl sheet = cell.Worksheet as WorksheetImpl;
      //SizeF size = sheet.MeasureCell( cell, false, true );
      SizeF size = sheet.MeasureCell( cell, false, false );
      int height = ( int )size.Height;
      double dX = rect2.X;
      double dY = rect2.Y;

      matrix.RotateAt( iRotationAngle, new PointF( ( float )dX, ( float )dY ) );

      PointF[] arrPoints = new PointF[]
      {
        new PointF( ( float )dX, ( float )dY )
      };

      matrix.TransformPoints( arrPoints );
      x = ( int )Math.Round( arrPoints[ 0 ].X );
      y = ( int )Math.Round( arrPoints[ 0 ].Y );

      arrPoints = RotateRectangle( size, xf.Rotation );
      //arrPoints = RotateRectangle( size, iRotationAngle );
      PointF point = AlignRectangle( arrPoints, rect2, format.Alignment, format.LineAlignment );

      matrix.Translate( point.X, point.Y, MatrixOrder.Append );
      graphics.Transform = matrix;
      graphics.DrawString( value, nativeFont, brush, x, y, format );
      graphics.Restore( state );
    }
    /// <summary>
    /// Rotates rectangle at coordinates (0,0) and specified size at specified angle.
    /// </summary>
    /// <param name="size">Size of the rectangle to rotate.</param>
    /// <param name="angleDegrees">Angle to rotate at.</param>
    /// <returns>Rectangle points after rotation.</returns>
    private PointF[] RotateRectangle( SizeF size, int angleDegrees )
    {
      Matrix matrix = new Matrix();
      //RectangleF rect = new RectangleF( new PointF( 0, 0 ), size );
      PointF[] arrPoints = new PointF[]
      {
        new PointF( 0, 0 ),
        new PointF( 0, size.Height ),
        new PointF( size.Width, size.Height ),
        new PointF( size.Width, 0 ),
      };

      matrix.Rotate( angleDegrees );
      matrix.TransformPoints( arrPoints );
      return arrPoints;
    }
    /// <summary>
    /// Aligns rectangle specified by points inside another rectangle.
    /// </summary>
    /// <param name="arrPoints">Points to align.</param>
    /// <param name="rect2">Rectangle to align in.</param>
    /// <param name="horizontal">Horizontal alignment.</param>
    /// <param name="vertical">Vertical alignment.</param>
    /// <returns>Required offset to get correct point for string drawing.</returns>
    private PointF AlignRectangle( PointF[] arrPoints, Rectangle rect2, StringAlignment horizontal, StringAlignment vertical )
    {
      float x = 0;
      float y = 0;
      float iMinX = float.MaxValue;
      float iMinY = float.MaxValue;
      float iMaxX = float.MinValue;
      float iMaxY = float.MinValue;

      for( int i = 0, len = arrPoints.Length; i < len; i++ )
      {
        PointF point = arrPoints[ i ];

        if( iMinX > point.X )
          iMinX = point.X;

        if( iMaxX < point.X )
          iMaxX = point.X;

        if( iMinY > point.Y )
          iMinY = point.Y;

        if( iMaxY < point.Y )
          iMaxY = point.Y;
      }

      switch( horizontal )
      {
        case StringAlignment.Near:
          x = -iMinX;
          break;

        case StringAlignment.Center:
          x = ( rect2.Width - iMaxX ) / 2.0F;
          break;

        case StringAlignment.Far:
          x = rect2.Width - iMaxX;
          break;
      }

      switch( vertical )
      {
        case StringAlignment.Near:
          y = -iMinY;
          break;

        case StringAlignment.Center:
          y = ( rect2.Height - iMaxY ) / 2.0F;
          break;

        case StringAlignment.Far:
          x = rect2.Height - iMaxY;
          break;
      }


      return new PointF( x, y );
    }
    /// <summary>
    /// Converts rotation angle into counter-clockwise value.
    /// </summary>
    /// <param name="rotationAngle">Angle to convert.</param>
    /// <returns>Converted value.</returns>
    private int GetCounterClockwiseRotation( int rotationAngle )
    {
      if( rotationAngle > 90 )
      {
        rotationAngle -= 90;
      }
      else
      {
        rotationAngle = -rotationAngle;
      }

      return rotationAngle;
    }
    /// <summary>
    /// Updates string for top to bottom text drawing if necessary.
    /// </summary>
    /// <param name="value">Value to update.</param>
    /// <param name="rotationAngle">Rotation angle.</param>
    /// <returns>Updated value.</returns>
    private string UpdateToToBottomText( string value, int rotationAngle )
    {
      if( rotationAngle == ExtendedFormatImpl.TopToBottomRotation )
      {
        //format.FormatFlags |= StringFormatFlags.DirectionVertical;
        StringBuilder builder = new StringBuilder( value );

        for( int i = 0, j = 1, len = value.Length; i < len; i++, j += 2 )
        {
          builder.Insert( j, '\n' );
        }

        value = builder.ToString();
      }

      return value;
    }
    /// <summary>
    /// Draws cell background.
    /// </summary>
    /// <param name="cell">Cell to draw background for.</param>
    /// <param name="rect">Cell's rectangle.</param>
    /// <param name="graphics">Graphics to draw at.</param>
    private void DrawBackground( IRange cell, Rectangle rect, Graphics graphics )
    {
      ExtendedFormatImpl xf = ( cell.CellStyle as CellStyle ).Wrapped;
#if !SyncfusionFramework2_0
      int startRow = 1;
      int startCol = 1;
      for (int pivotCount = 0; pivotCount < cell.Worksheet.PivotTables.Count; pivotCount++)
      {
          if (cell.Row == cell.Worksheet.PivotTables[pivotCount].Location.Row && cell.Column == cell.Worksheet.PivotTables[pivotCount].Location.Column)
          {
              pivotTableRange = cell.Worksheet.PivotTables[pivotCount].Location;
              pivotImpl = cell.Worksheet.PivotTables[pivotCount] as PivotTableImpl;
          }
      }

      if (pivotTableRange != null)
      {
          startRow = pivotTableRange.Row;
          startCol = pivotTableRange.Column;
      }
      PivotTableStyleRenderer renderer = new PivotTableStyleRenderer(cell.Worksheet);
      if (cell.Worksheet.PivotTables.Count != 0 && pivotImpl.PageFields.Count != 0)
      {
          if (startRow - 2 == cell.Row && startCol == cell.Column)
          {
              xf = renderer.GetPageFilterLabel(pivotImpl.BuiltInStyle);
          }
          if (startRow - 2 == cell.Row && startCol + 1 == cell.Column)
          {
              xf = renderer.GetPageFilterValue(pivotImpl.BuiltInStyle);
          }
      }

      if (pivotTableRange != null && pivotTableRange.Row <= cell.Row && pivotTableRange.LastRow >= cell.LastRow
        && pivotTableRange.Column <= cell.Column && pivotTableRange.LastColumn >= cell.LastColumn)
      {
          xf = pivotImpl.PivotLayout[cell.Row - startRow, cell.Column - startCol].XF;
      }
      else
#endif      
            xf = m_cfApplier.ApplyCF(cell, xf);
       DrawBackground( xf, rect, graphics,cell );
    }
    /// <summary>
    /// Draws background at the specified position.
    /// </summary>
    /// <param name="xf">Object containing background settings.</param>
    /// <param name="rect">Rectangle specifying coordinates of the drawn background.</param>
    /// <param name="graphics">Graphics to draw at.</param>
    private void DrawBackground( IInternalExtendedFormat xf, Rectangle rect, Graphics graphics,IRange cell)
    {
      if( rect.Height > 0 && rect.Width > 0 )
      {
        Brush brush;

        if( xf.FillPattern == ExcelPattern.None )
        {
          //Rectangle rect2 = rect;
          rect.Offset( 1, 1 );
          rect.Width--;
          rect.Height--;

          IBorders borders = xf.Borders;
          rect = UpdateRectangleCoordinates( rect, borders );

          if (cell.Worksheet.PageSetup.BackgoundImage == null)
          {
              brush = Brushes.White;
              graphics.FillRectangle(brush, rect);
          }
        }
        else
        {
            if (xf.Color != Color.White)
            {
                brush = GetBrush(xf);
                graphics.FillRectangle(brush, rect);
                brush.Dispose();
            }
        }
      }
    }
    private Rectangle UpdateRectangleCoordinates( Rectangle rect, IBorders borders )
    {
      if( borders[ ExcelBordersIndex.EdgeLeft ].LineStyle != ExcelLineStyle.None )
      {
        rect.Offset( -1, 0 );
        rect.Width++;
      }

      if( borders[ ExcelBordersIndex.EdgeTop ].LineStyle != ExcelLineStyle.None )
      {
        rect.Offset( 0, -1 );
        rect.Height++;
      }

      if( borders[ ExcelBordersIndex.EdgeBottom ].LineStyle != ExcelLineStyle.None )
        rect.Height++;

      if( borders[ ExcelBordersIndex.EdgeRight ].LineStyle != ExcelLineStyle.None )
        rect.Width++;

      return rect;
    }
    /// <summary>
    /// Creates brush object based on specified extended format.
    /// </summary>
    /// <param name="xf">Extended format to get brush for.</param>
    /// <returns>Created brush object.</returns>
    private Brush GetBrush( IInternalExtendedFormat xf )
    {
      Brush result = null;

      if( xf.FillPattern == ExcelPattern.Solid )
      {
        Color color = NormalizeColor( xf.Color );
        result = new SolidBrush( color );
      }
      else
      {
        HatchStyle style = HatchStyle.Percent05;

        switch( xf.FillPattern )
        {
          case ExcelPattern.Percent50:
            style = HatchStyle.Percent50;
            break;

          case ExcelPattern.Percent70:
            style = HatchStyle.Percent70;
            break;

          case ExcelPattern.Percent25:
            style = HatchStyle.Percent25;
            break;

          case ExcelPattern.Percent75:
            style = HatchStyle.Percent75;
            break;

          case ExcelPattern.Percent60:
            style = HatchStyle.Percent60;
            break;

          case ExcelPattern.Percent10:
            style = HatchStyle.Percent10;
            break;

          case ExcelPattern.Percent05:
            style = HatchStyle.Percent05;
            break;

          case ExcelPattern.DarkHorizontal:
            style = HatchStyle.DarkHorizontal;
            break;

          case ExcelPattern.DarkVertical:
            style = HatchStyle.DarkVertical;
            break;

          case ExcelPattern.DarkDownwardDiagonal:
            style = HatchStyle.DarkDownwardDiagonal;
            break;

          case ExcelPattern.DarkUpwardDiagonal:
            style = HatchStyle.DarkUpwardDiagonal;
            break;

          case ExcelPattern.ForwardDiagonal:
            style = HatchStyle.ForwardDiagonal;
            break;

          case ExcelPattern.Horizontal:
            style = HatchStyle.Horizontal;
            break;

          case ExcelPattern.Vertical:
            style = HatchStyle.Vertical;
            break;

          case ExcelPattern.LightDownwardDiagonal:
            style = HatchStyle.LightDownwardDiagonal;
            break;

          case ExcelPattern.LightUpwardDiagonal:
            style = HatchStyle.LightUpwardDiagonal;
            break;

          case ExcelPattern.Angle:
            style = HatchStyle.SmallGrid;
            break;
        }

        result = new HatchBrush( style, xf.PatternColor, xf.Color );
      }

      return result;
    }
    /// <summary>
    /// Normalizes color - makes it non-transparent.
    /// </summary>
    /// <param name="color">Color to normalize.</param>
    /// <returns>Non-transparent color (Alpha = 255) based on specified color.</returns>
    private Color NormalizeColor( Color color )
    {
      return Color.FromArgb( 0xFF, color.R, color.G, color.B );
    }
    /// <summary>
    /// Gets vertical string alignment based on the cell style.
    /// </summary>
    /// <param name="style">Style to get data from.</param>
    /// <returns>String alignment.</returns>
    private StringAlignment GetVerticalAlignment( IExtendedFormat style )
    {
      StringAlignment result;

      switch( style.VerticalAlignment )
      {
        case ExcelVAlign.VAlignCenter:
          result = StringAlignment.Center;
          break;

        case ExcelVAlign.VAlignTop:
          result = StringAlignment.Near;
          break;

        case ExcelVAlign.VAlignBottom:
        default:
          result = StringAlignment.Far;
          break;
      }

      return result;
    }
    /// <summary>
    /// Gets horizontal string alignment based on the cell style.
    /// </summary>
    /// <param name="style">Style to get data from.</param>
    /// <param name="cell">Cell to get alignment for.</param>
    /// <returns>String alignment.</returns>
    private StringAlignment GetHorizontalAlignment( IExtendedFormat style, IRange cell )
    {
      StringAlignment result;

      switch( style.HorizontalAlignment )
      {
        case ExcelHAlign.HAlignRight:
          result = StringAlignment.Far;
          break;

        case ExcelHAlign.HAlignCenter:
        case ExcelHAlign.HAlignCenterAcrossSelection:
          result = StringAlignment.Center;
          break;

        case ExcelHAlign.HAlignGeneral:
          // here we have to check value and formatting type.
          // 1. Number & formatting != text -> right
          // 2. else text.
          if( style.Rotation == ExtendedFormatImpl.TopToBottomRotation )
          {
            result = StringAlignment.Center;
          }
          else if( cell.HasNumber ||
            cell.HasFormula &&
            cell.FormulaStringValue == null &&
            !cell.HasFormulaErrorValue &&
            !cell.HasFormulaBoolValue )
          {
            WorkbookImpl book = cell.Worksheet.Workbook as WorkbookImpl;
            FormatImpl numberFormat = book.InnerFormats[ style.NumberFormatIndex ];
            ExcelFormatType formatType = numberFormat.GetFormatType( cell.Number );

            result = ( formatType == ExcelFormatType.Text ) ?
              StringAlignment.Near :
              StringAlignment.Far;
          }
          else
          {
            result = StringAlignment.Near;
          }
          break;

        case ExcelHAlign.HAlignLeft:
        default:
          result = StringAlignment.Near;
          break;
      }

      return result;
    }
    /// <summary>
    /// Draws cell borders (including gridlines if necessary).
    /// </summary>
    /// <param name="borders">Border collection.</param>
    /// <param name="rect">Cell's rectangle.</param>
    /// <param name="graphics">Graphics to draw at.</param>
    /// <param name="cell">Represents cell Range to draw border.</param>
    private void DrawBorders( IBorders borders, Rectangle rect, Graphics graphics, IRange cell )
    {
      IBorder border;

      border = borders[ ExcelBordersIndex.EdgeLeft ];
      if (EnableRTL)
      {
         border = borders[ExcelBordersIndex.EdgeRight];
      }
      DrawBorder( borders, border, rect.Left, rect.Top, rect.Left, rect.Bottom, graphics, cell );

      border = borders[ ExcelBordersIndex.EdgeRight ];
      if (EnableRTL)
      {
          border = borders[ExcelBordersIndex.EdgeLeft];
      }
      DrawBorder( borders, border, rect.Right, rect.Top, rect.Right, rect.Bottom, graphics, cell );

      border = borders[ ExcelBordersIndex.EdgeTop ];
      DrawBorder( borders, border, rect.Left, rect.Top, rect.Right, rect.Top, graphics, cell );

      border = borders[ ExcelBordersIndex.EdgeBottom ];
      DrawBorder( borders, border, rect.Left, rect.Bottom, rect.Right, rect.Bottom, graphics, cell );

      border = borders[ ExcelBordersIndex.DiagonalDown ];

      if( border.ShowDiagonalLine )
        DrawBorder( borders, border, rect.Left, rect.Top, rect.Right, rect.Bottom, graphics, cell );

      border = borders[ ExcelBordersIndex.DiagonalUp ];

      if( border.ShowDiagonalLine )
        DrawBorder( borders, border, rect.Left, rect.Bottom, rect.Right, rect.Top, graphics, cell );
    }
    /// <summary>
    /// Draws specified border.
    /// </summary>
    /// <param name="borders"></param>
    /// <param name="border"></param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="graphics"></param>
    /// <param name="cell"></param>
    private void DrawBorder( IBorders borders, IBorder border, int x1, int y1, int x2, int y2,
      Graphics graphics, IRange cell )
    {
      if( border.LineStyle == ExcelLineStyle.Double )
      {
        DrawDoubleBorder( borders, border, x1, y1, x2, y2, graphics, cell );
      }
      else
      {
        DrawOrdinaryBorder( border, x1, y1, x2, y2, graphics );
      }
    }
    /// <summary>
    /// Draws doubled border.
    /// </summary>
    /// <param name="borders"></param>
    /// <param name="border"></param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="graphics"></param>
    /// <param name="cell"></param>
    private void DrawDoubleBorder( IBorders borders, IBorder border,
      int x1, int y1, int x2, int y2,
      Graphics graphics, IRange cell )
    {
      Pen pen = CreatePen( border );
      int xDelta;
      int yDelta;

      BorderImpl borderImpl = border as BorderImpl;
      ExcelBordersIndex borderIndex = borderImpl.BorderIndex;

      switch( borderIndex )
      {
        case ExcelBordersIndex.EdgeBottom:
          xDelta = 0;
          yDelta = 1;
          break;

        case ExcelBordersIndex.EdgeLeft:
          xDelta = -1;
          yDelta = 0;
          break;

        case ExcelBordersIndex.EdgeRight:
          xDelta = 1;
          yDelta = 0;
          break;

        case ExcelBordersIndex.EdgeTop:
          xDelta = 0;
          yDelta = -1;
          break;

        default:
          // maybe different diagonal borders should differ.
          xDelta = 1;
          yDelta = 1;
          break;
      }

      DrawInnerLine( graphics, pen, borders, borderIndex, x1, y1, x2, y2, xDelta, yDelta, cell );
      DrawOuterLine( graphics, pen, borders, borderIndex, x1, y1, x2, y2, xDelta, yDelta, cell );
    }
    /// <summary>
    /// Draws outer line of the double border.
    /// </summary>
    /// <param name="graphics">Graphics to draw at.</param>
    /// <param name="pen">Pen to draw with.</param>
    /// <param name="borders">Borders collection containing border that is being drawn.</param>
    /// <param name="borderIndex">Border that is being drawn.</param>
    /// <param name="x1">X coordinate of the starting point for ordinary border.</param>
    /// <param name="y1">Y coordinate of the starting point for ordinary border.</param>
    /// <param name="x2">X coordinate of the finishing point for ordinary border.</param>
    /// <param name="y2">Y coordinate of the finishing point for ordinary border.</param>
    /// <param name="xDelta"></param>
    /// <param name="yDelta"></param>
    /// <param name="cell"></param>
    private void DrawOuterLine( Graphics graphics, Pen pen, IBorders borders, ExcelBordersIndex borderIndex,
      int x1, int y1, int x2, int y2, int xDelta, int yDelta, IRange cell )
    {
      ExcelBordersIndex start;
      ExcelBordersIndex end;
      GetStartEndBorderIndex( borderIndex, out start, out end );

      int xDelta1 = xDelta;
      int xDelta2 = xDelta;
      int yDelta1 = yDelta;
      int yDelta2 = yDelta;
      int iRow = cell.Row + yDelta;
      int iColumn = cell.Column + xDelta;
      if (iColumn == 0)
          iColumn = 1;
      if (iRow == 0)
          iRow = 1;
      IBorders adjacentBorders = cell.Worksheet[ iRow, iColumn ].Borders;

      UpdateBorderDelta( cell.Worksheet, iRow, iColumn, xDelta, yDelta, ref xDelta1, ref yDelta1, true,
        adjacentBorders, end, start, true );

      UpdateBorderDelta( cell.Worksheet, iRow, iColumn, xDelta, yDelta, ref xDelta2, ref yDelta2, true,
        adjacentBorders, start, end, false );
      
      graphics.DrawLine( pen, x1 + xDelta1, y1 + yDelta1, x2 + xDelta2, y2 + yDelta2 );
    }
    /// <summary>
    /// Draws inner line of the double border.
    /// </summary>
    /// <param name="graphics">Graphics to draw at.</param>
    /// <param name="pen">Pen to draw with.</param>
    /// <param name="borders">Borders collection containing border that is being drawn.</param>
    /// <param name="borderIndex">Border that is being drawn.</param>
    /// <param name="x1">X coordinate of the starting point for ordinary border.</param>
    /// <param name="y1">Y coordinate of the starting point for ordinary border.</param>
    /// <param name="x2">X coordinate of the finishing point for ordinary border.</param>
    /// <param name="y2">Y coordinate of the finishing point for ordinary border.</param>
    /// <param name="xDelta"></param>
    /// <param name="yDelta"></param>
    /// <param name="cell"></param>
    private void DrawInnerLine( Graphics graphics, Pen pen, IBorders borders, ExcelBordersIndex borderIndex,
      int x1, int y1, int x2, int y2, int xDelta, int yDelta, IRange cell )
    {
      ExcelBordersIndex start;
      ExcelBordersIndex end;
      GetStartEndBorderIndex( borderIndex, out start, out end );

      int xDelta1 = xDelta;
      int xDelta2 = xDelta;
      int yDelta1 = yDelta;
      int yDelta2 = yDelta;
      int iRow = cell.Row;
      int iColumn = cell.Column;
      IWorksheet sheet = cell.Worksheet;

      UpdateBorderDelta( sheet, iRow, iColumn, -xDelta, -yDelta, ref xDelta1, ref yDelta1, false,
        borders, end, start, true );

      UpdateBorderDelta( sheet, iRow, iColumn, -xDelta, -yDelta, ref xDelta2, ref yDelta2, false,
        borders, start, end, false );

      graphics.DrawLine( pen, x1 - xDelta1, y1 - yDelta1, x2 - xDelta2, y2 - yDelta2 );
    }
    /// <summary>
    /// Updates delta coordinates for double border drawing, if necessary.
    /// </summary>
    /// <param name="sheet"></param>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="xDelta"></param>
    /// <param name="yDelta"></param>
    /// <param name="xDelta1"></param>
    /// <param name="yDelta1"></param>
    /// <param name="isInvertCondition"></param>
    /// <param name="borders"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="isLineStart"></param>
    private void UpdateBorderDelta( IWorksheet sheet, int row, int column, int xDelta, int yDelta,
      ref int xDelta1, ref int yDelta1,
      bool isInvertCondition,
      IBorders borders, ExcelBordersIndex start, ExcelBordersIndex end, bool isLineStart )
    {
      int iCheckRow = row + xDelta;
      int iCheckColumn = column + yDelta;
      IWorkbook book = sheet.Workbook;
      int iMaxRow = book.MaxRowCount;
      int iMaxColumn = book.MaxColumnCount;

      if( iCheckRow <= 0 || iCheckRow > iMaxRow ||
        iCheckColumn <= 0 || iCheckColumn > iMaxColumn )
      {
        return;
      }

      IBorders adjacentBorders = sheet[ row + xDelta, column + yDelta ].Borders;
      bool bCondition = borders[ end ].LineStyle == ExcelLineStyle.Double ||
        adjacentBorders[ start ].LineStyle == ExcelLineStyle.Double;

      if( isInvertCondition )
        bCondition = !bCondition;

      int valueToAssign;

      if( end != ( ExcelBordersIndex )( -1 ) && bCondition )
      {
        valueToAssign = isLineStart ? -1 : 1;
      }
      else
      {
        valueToAssign = isLineStart ? 1 : -1;
      }

      if( yDelta != 0 )
      {
        xDelta1 = valueToAssign;
      }
      else if( xDelta != 0 )
      {
        yDelta1 = valueToAssign;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="borderIndex"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    private void GetStartEndBorderIndex( ExcelBordersIndex borderIndex, out ExcelBordersIndex start,
      out ExcelBordersIndex end )
    {
      start = ( ExcelBordersIndex )( -1 );
      end = ( ExcelBordersIndex )( -1 );

      switch( borderIndex )
      {
        case ExcelBordersIndex.EdgeTop:
        case ExcelBordersIndex.EdgeBottom:
          start = ExcelBordersIndex.EdgeLeft;
          end = ExcelBordersIndex.EdgeRight;
          break;

        case ExcelBordersIndex.EdgeRight:
        case ExcelBordersIndex.EdgeLeft:
          start = ExcelBordersIndex.EdgeTop;
          end = ExcelBordersIndex.EdgeBottom;
          break;
      }
    }
    /// <summary>
    /// Draws single border at specified position.
    /// </summary>
    /// <param name="border">Border to draw.</param>
    /// <param name="x1">X coordinate of the first point.</param>
    /// <param name="y1">Y coordinate of the first point.</param>
    /// <param name="x2">X coordinate of the second point.</param>
    /// <param name="y2">Y coordinate of the second point.</param>
    /// <param name="graphics">Graphics to draw at.</param>
    private void DrawOrdinaryBorder( IBorder border, int x1, int y1, int x2, int y2, Graphics graphics )
    {
      ExcelLineStyle lineStyle = border.LineStyle;

      if( lineStyle != ExcelLineStyle.None )
      {
        using( Pen pen = CreatePen( border ) )
        {
          graphics.DrawLine( pen, x1, y1, x2, y2 );
        }
      }
    }
    /// <summary>
    /// Creates pen for the specified border.
    /// </summary>
    /// <param name="border">Border to create pen for.</param>
    /// <returns>Created pen.</returns>
    private Pen CreatePen( IBorder border )
    {
      Color penColor = NormalizeColor( border.ColorRGB );
      Pen result = new Pen( penColor, GetBorderWidth( border ) );
      result.DashStyle = GetDashStyle( border );
      return result;
    }
    /// <summary>
    /// Gets DashStyle for the pen based on the border settings.
    /// </summary>
    /// <param name="border">Border to get DashStyle for.</param>
    /// <returns>DashStyle closest to the border line settings.</returns>
    private DashStyle GetDashStyle( IBorder border )
    {
      DashStyle result = DashStyle.Solid;

      switch( border.LineStyle )
      {
        case ExcelLineStyle.Thin:
        case ExcelLineStyle.Medium:
        case ExcelLineStyle.Thick:
        case ExcelLineStyle.Double:
        case ExcelLineStyle.Hair:
          result = DashStyle.Solid;
          break;

        case ExcelLineStyle.Dashed:
        case ExcelLineStyle.Medium_dashed:
          result = DashStyle.Dash;
          break;

        case ExcelLineStyle.Dotted:
          result = DashStyle.Dot;
          break;

        case ExcelLineStyle.Dash_dot:
        case ExcelLineStyle.Medium_dash_dot:
        case ExcelLineStyle.Slanted_dash_dot:
          result = DashStyle.DashDot;
          break;

        case ExcelLineStyle.Dash_dot_dot:
        case ExcelLineStyle.Medium_dash_dot_dot:
          result = DashStyle.DashDotDot;
          break;
      }

      return result;
    }
    /// <summary>
    /// Evaluates line width based on border settings.
    /// </summary>
    /// <param name="border">Border to get data from.</param>
    /// <returns></returns>
    private float GetBorderWidth( IBorder border )
    {
      float result = 0;

      switch( border.LineStyle )
      {
        case ExcelLineStyle.Hair:
          result = 0.5F;
          break;

        // TODO: add Double border correct support.
        case ExcelLineStyle.Double:

        case ExcelLineStyle.Thin:
        case ExcelLineStyle.Dashed:
        case ExcelLineStyle.Dotted:
        case ExcelLineStyle.Dash_dot:
        case ExcelLineStyle.Slanted_dash_dot:
        case ExcelLineStyle.Dash_dot_dot:
          result = 1F;
          break;

        case ExcelLineStyle.Medium:
        case ExcelLineStyle.Medium_dashed:
        case ExcelLineStyle.Medium_dash_dot:
        case ExcelLineStyle.Medium_dash_dot_dot:
          result = 2F;
          break;

        case ExcelLineStyle.Thick:
          result = 3F;
          break;
      }

      return result;
    }
    /// <summary>
    /// Creates image of the specified size and type.
    /// </summary>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    /// <param name="imageType">Type of the image to create.</param>
    /// <param name="outputStream">Stream to save into.</param>
    /// <param name="emfType">Metafile EmfType.</param>
    /// <returns>Created image.</returns>
    private Image CreateImage( int width, int height,
      ImageType imageType, Stream outputStream, EmfType emfType )
    {
      Image result;

      switch( imageType )
      {
        case ImageType.Bitmap:
          result = new Bitmap( width, height );
          break;

        case ImageType.Metafile:
          if( outputStream == null )
            outputStream = new MemoryStream();

          using( Image bitmap = new Bitmap( width, height ) )
          {
            using( Graphics g = Graphics.FromImage( bitmap ) )
            {
              IntPtr hdc = g.GetHdc();
              Rectangle rect = new Rectangle( 0, 0, width, height );
              result = new Metafile( outputStream, hdc, rect, MetafileFrameUnit.Pixel, emfType );
              g.ReleaseHdc();
            }
          }
          break;

        default:
          throw new ArgumentOutOfRangeException( "imageType" );
      }

      //using( Graphics graphics = Graphics.FromImage( result ) )
      {
        //graphics.FillRectangle( Brushes.White, new Rectangle( 0, 0, width, height ) );
      }

      return result;
    }
    #endregion
  }
#region Helper Class

  //Class Rich Text Box Print (Print The Rich Text Box control)
internal class RichTextBoxPrintCtrl : RichTextBox
{
#region Print Implementation
    //Convert the unit used by the .NET framework (1/100 inch)
    //and the unit used by Win32 API calls (twips 1/1440 inch)
    private const double anInch = 14.4;
 

    //Defining the required Win32 structures
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
 
    [StructLayout(LayoutKind.Sequential)]
    private struct CHARRANGE
    {
        public int cpMin;         //First character of range (0 for start of doc)
        public int cpMax;         //Last character of range (-1 for end of doc)
    }
 
    [StructLayout(LayoutKind.Sequential)]
    private struct FORMATRANGE
    {
        public IntPtr hdc;             //Actual DC to draw on
        public IntPtr hdcTarget;       //Target DC for determining text formatting
        public RECT rc;                //Region of the DC to draw to (in twips)
        public RECT rcPage;            //Region of the whole DC (page size) (in twips)
        public CHARRANGE chrg;         //Range of text to draw (see earlier declaration)
    }
 
    private const int WM_USER = 0X0400;
    private const int EM_FORMATRANGE = WM_USER + 57;
 
    [DllImport("USER32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);
 
    internal int PrintImage(int nStartFrom, Graphics g)
    {
        RECT rcToPrint = default(RECT);
        FORMATRANGE fr = default(FORMATRANGE);
        IntPtr iptHdc = IntPtr.Zero;
        IntPtr iptRes = IntPtr.Zero;
        IntPtr iptParam = IntPtr.Zero;
 
        // Calculate the area to render and print
        rcToPrint.Top = 0;
        rcToPrint.Bottom = (int)Math.Ceiling((g.VisibleClipBounds.Height * anInch));
        rcToPrint.Left = 0;
        rcToPrint.Right = (int)Math.Ceiling((g.VisibleClipBounds.Width * anInch));
 
        iptHdc = g.GetHdc();
 
        //Indicate character from to character to
        fr.chrg.cpMin = nStartFrom;
        fr.chrg.cpMax = -1;
        //Use the same DC for measuring and rendering point at printer hDC       
        fr.hdc = fr.hdcTarget = iptHdc;
 
        //Indicate the area on page to print
        fr.rc = rcToPrint;
 
        //Get the pointer to the FORMATRANGE structure in memory
        iptParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fr));
        Marshal.StructureToPtr(fr, iptParam, false);
 
        //Send the rendered data for printing
        iptRes = SendMessage(this.Handle, EM_FORMATRANGE, 1, iptParam.ToInt32());
 
        //Free the block of memory allocated
        Marshal.FreeCoTaskMem(iptParam);
 
        //Release the device context handle obtained by a previous call
        g.ReleaseHdc(iptHdc);
 
        //Return last + 1 character printer
        return iptRes.ToInt32();
    }
    #endregion
}
  #endregion
}
#endif
