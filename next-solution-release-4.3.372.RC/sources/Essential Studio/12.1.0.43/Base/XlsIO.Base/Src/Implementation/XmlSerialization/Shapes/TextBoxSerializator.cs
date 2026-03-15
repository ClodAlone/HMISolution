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
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
using System.IO;

#if ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;
#endif
namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes
{
  /// <summary>
  /// This class is used for TextBox serialization.
  /// </summary>
  class TextBoxSerializator : DrawingShapeSerializator
  {
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

      if( holder == null )
        throw new ArgumentNullException( "holder" );

      TextBoxShapeImpl textBox = shape as TextBoxShapeImpl;

      if( textBox == null )
        throw new ArgumentOutOfRangeException( "textBox" );

      FileDataHolder fileHolder = holder.ParentHolder;

      // TODO: change this later
      string DrawingsNamespace = ( shape.ParentShapes.Worksheet != null ) ?
        Drawings.XdrNamespace :
        Drawings.CdrNamespace;

      WorksheetImpl sheet = shape.Worksheet as WorksheetImpl;

      if (shape.EnableAlternateContent)
      {
        // TODO: move to constants.
          writer.WriteStartElement(Excel2007Serializator.MCPrefix, Drawings.AlternateContentTag, Excel2007Serializator.MCNamespace);
          writer.WriteStartElement(Excel2007Serializator.MCPrefix, Drawings.ChoiceTag, Excel2007Serializator.MCNamespace);
          writer.WriteAttributeString(WorkbookXmlSerializator.DEF_XMLNS_PREF, "a14", null, "http://schemas.microsoft.com/office/drawing/2010/main");
          writer.WriteAttributeString("Requires", "a14");
      }

      if( sheet != null )
      {
        writer.WriteStartElement( Drawings.TwoCellAnchorTagName, DrawingsNamespace );
        writer.WriteAttributeString( Drawings.EditAsAttribute, GetEditAsValue( shape ) );
      }
      else
      {
        writer.WriteStartElement( ChartConstants.RelativeSizeAnchorTag, DrawingsNamespace );
      }

      SerializeAnchorPoint( writer, Drawings.FromTagName,
        shape.LeftColumn, shape.LeftColumnOffset,
        shape.TopRow, shape.TopRowOffset, sheet, DrawingsNamespace );

      SerializeAnchorPoint(writer, Drawings.ToTagName,
          shape.RightColumn, shape.RightColumnOffset,
          shape.BottomRow, shape.BottomRowOffset, sheet, DrawingsNamespace);

      if (shape.IsEquationShape)
      {
          Stream stream = shape.XmlDataStream;
          stream.Position = 0;
          XmlReader reader = UtilityMethods.CreateReader(stream);
          //reader.Read();
          writer.WriteNode(reader, false);
      }
      else
      {

          //SerializePicture( writer, picture, strRelationId, holder );
          writer.WriteStartElement(Drawings.Shape, DrawingsNamespace);
          Excel2007Serializator.SerializeAttribute(writer, Drawings.LockTextAttribute, textBox.IsTextLocked, true);
          Excel2007Serializator.SerializeAttribute(writer, "textlink", textBox.TextLink, null);
          //SerializeLockText( writer, textBox );
          SerializeNonVisualProperties(writer, textBox, holder, DrawingsNamespace);
          SerializeShapeProperites(writer, textBox, fileHolder, holder.Relations, DrawingsNamespace);
          SerializeRichText(writer, DrawingsNamespace, textBox);
          writer.WriteEndElement();

          
      }
      if (sheet != null)
          writer.WriteElementString(Drawings.ClientDataTagName, DrawingsNamespace, string.Empty);
      writer.WriteEndElement();

      if (shape.EnableAlternateContent)
      {
          writer.WriteEndElement();
          writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes shape properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textBox">TextBox to serialize properties for.</param>
    /// <param name="holder">File data holder.</param>
    /// <param name="relations">Drawing relations.</param>
    /// <param name="drawingsNamespace">Xml namespace to use.</param>
    private void SerializeShapeProperites( XmlWriter writer, TextBoxShapeImpl textBox,
      FileDataHolder holder, RelationCollection relations, string drawingsNamespace )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      writer.WriteStartElement( Drawings.ShapePropertiesTag, drawingsNamespace );

      Rectangle rect = textBox.Coordinates2007;
      SerializeForm(writer, Drawings.ANamespace, Drawings.ANamespace, rect.X, rect.Y, rect.Width, rect.Height, textBox as IShape);
      SerializePresetGeometry( writer );
      SerializeFill( writer, textBox, holder, relations );

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes non visual textbox properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textBox">TextBox to serialize.</param>
    /// <param name="holder">Worksheet's data holder.</param>
    /// <param name="drawingsNamespace">Xml namespace to use.</param>
    private void SerializeNonVisualProperties( XmlWriter writer, TextBoxShapeImpl textBox,
      WorksheetDataHolder holder, string drawingsNamespace )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      writer.WriteStartElement( Drawings.NonVisualShapeProperties, drawingsNamespace );

      //writer.WriteStartElement( Drawings.NVCanvasPropertiesTag, Drawings.XdrNamespace );

      //// TODO: serialize some id - find out algorithm.
      //writer.WriteAttributeString( Drawings.IdAttributeName, "2" );
      //writer.WriteAttributeString( Drawings.NameAttributeName, textBox.Name );
      //writer.WriteEndElement();
      SerializeNVCanvasProperties( writer, textBox, holder, drawingsNamespace );

      writer.WriteStartElement( Drawings.NonVisualDrawingProperties, drawingsNamespace );
      writer.WriteAttributeString( Drawings.TextBoxAttribute, Excel2007Serializator.TrueValue );
      writer.WriteEndElement();
      
      writer.WriteEndElement();
    }
    #endregion

    #region Duplicated methods - remove one of those copies
    /// <summary>
    /// Serializes rich text.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="drawingsNamespace">Xml namespace to use.</param>
    /// <param name="textBox">Textbox to serialize rich text for.</param>
    public static void SerializeRichText( XmlWriter writer, string drawingsNamespace,
      TextBoxShapeBase textBox )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textBox == null )
        throw new ArgumentNullException( "textBox" );

      RichTextString textArea = ( RichTextString )textBox.RichText;

      //writer.WriteStartElement( ChartConstants.RichTextTag, ChartConstants.CNamespace );
      writer.WriteStartElement( Drawings.TextBody, drawingsNamespace );
      SerializeBodyProperties( writer, textArea, textBox );
      SerializeListStyles( writer, textArea );
      SerializeParagraphs( writer, textArea, textBox );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize text area body properties.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textArea">Text area to serialize body properties for.</param>
    /// <param name="textBox">Text box to be serialized.</param>
    private static void SerializeBodyProperties( XmlWriter writer, RichTextString textArea,
      TextBoxShapeBase textBox )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      writer.WriteStartElement( Drawings.TextBodyPropertiesTag, Drawings.ANamespace );

      Dictionary<string, string> bodyProperties = textBox.UnknownBodyProperties;

      if( bodyProperties != null && bodyProperties.Count > 0 )
      {
        foreach( KeyValuePair<string, string> pair in bodyProperties )
        {
          writer.WriteAttributeString( pair.Key, pair.Value );
        }
      }

      SerializeTextRotation( writer, textBox );
      SerializeAnchor( writer, textBox );
      // TODO: on the current moment we don't support body properties.
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes anchor (vertical alignment).
    /// </summary>
    /// <param name="writer">XmlWriter to serialize anchor into.</param>
    /// <param name="textBox">TextBox to serialize anchor for.</param>
    private static void SerializeAnchor( XmlWriter writer, TextBoxShapeBase textBox )
    {
      //throw new Exception( "The method or operation is not implemented." );
      if( textBox.VAlignment != ExcelCommentVAlign.Top )
      {
        writer.WriteAttributeString( Drawings.AnchorAttribute, ( ( Excel2007CommentVAlign )textBox.VAlignment ).ToString() );
      }
    }
    /// <summary>
    /// Serializes text rotation.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textBox">TextBox to serialize.</param>
    private static void SerializeTextRotation( XmlWriter writer, TextBoxShapeBase textBox )
    {
      if( textBox.TextRotation != ExcelTextRotation.LeftToRight )
      {
        writer.WriteAttributeString( Drawings.TextBoxRotationAttribute,
          ( ( Excel2007TextRotation )textBox.TextRotation ).ToString() );
      }
    }
    /// <summary>
    /// Serializes list styles for a text area.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="textArea">Text area to serialize list styles for.</param>
    private static void SerializeListStyles( XmlWriter writer, RichTextString textArea )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      writer.WriteStartElement( Drawings.ListStylesTag, Drawings.ANamespace );
      // TODO: on the current moment we don't support list styles.
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes paragraph.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize paragraph tag into.</param>
    /// <param name="textArea">Text area that contains paragraph information (formatting and text).</param>
    /// <param name="textBox">Text box to serialize paragraph for.</param>
    private static void SerializeParagraphs( XmlWriter writer, RichTextString textArea, TextBoxShapeBase textBox )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      writer.WriteStartElement( Drawings.Paragraphs, Drawings.ANamespace );

      // TODO: our text area support only one formatting, so we are serializing only one paragraph run.
      //ChartTextAreaImpl textAreaImpl = ( ChartTextAreaImpl )textArea;
      TextWithFormat text = textArea.TextObject;
      int iCurrentPos = 0;
      WorkbookImpl book = (WorkbookImpl) textBox.Workbook;
      FontsCollection arrFonts = book.InnerFonts;
      string strText = text.Text;

      SerializeFormattingRunProperty(writer, book, textBox);
      if (strText == null || strText.Length <= 0 || strText=="\n")
      {
          SerializeTextFeildElement(writer, textBox);     
      }
      if( text.FormattingRunsCount > 0 && text.GetPositionByIndex( 0 ) != 0 )
      {
        text = text.TypedClone();
        int defaultFontIndex = textArea.DefaultFontIndex;
        text.FormattingRuns[ 0 ] = ( defaultFontIndex >= 0 ) ? defaultFontIndex : 0;
      }

      
      int iRunsCount = text.FormattingRunsCount;
      IFont font;

      for( int i = 0; i < iRunsCount; i++ )
      {
        int iFontIndex = text.GetFontByIndex( i );
        //int iPositionStart = text.GetPositionByIndex( i );
        int iPositionEnd = ( ( i != iRunsCount - 1 ) ?
          text.GetPositionByIndex( i + 1 )
          : strText.Length ) - 1;

        font = arrFonts[ iFontIndex ];
        string strTextToSave = strText.Substring( iCurrentPos, iPositionEnd - iCurrentPos + 1 );

        string[] arrTextParts = strTextToSave.Split( '\n' );

        for( int j = 0, lenJ = arrTextParts.Length; j < lenJ; j++ )
        {
          SerializeFormattingRun( writer, font, Drawings.TextRunProperites, book, arrTextParts[ j ] ,textBox);

          if( j != lenJ - 1 )
          {
            //if( arrTextParts[ j ].Length > 0 )
              SerializeParagraphRunProperites(writer, font, Drawings.ParagraphEndProperties, book, false);

            writer.WriteEndElement();
            writer.WriteStartElement( Drawings.Paragraphs, Drawings.ANamespace );
          }
        }

        iCurrentPos = iPositionEnd + 1;
      }

      int iDefaultFontIndex = textArea.DefaultFontIndex;

      if( iDefaultFontIndex < 0 ) iDefaultFontIndex = 0;

      font = arrFonts[ iDefaultFontIndex ];//0 ];

      if( iRunsCount == 0 && strText != null && strText.Length > 0 )
      {
        SerializeFormattingRun( writer, font, Drawings.TextRunProperites, book, strText,textBox );
      }

      SerializeParagraphRunProperites( writer, font, Drawings.ParagraphEndProperties, book,false );
      writer.WriteEndElement();
    }

    private static void SerializeTextFeildElement(XmlWriter writer, TextBoxShapeBase textBox)
    {
        TextBoxShapeImpl textBoxShapeImpl = textBox as TextBoxShapeImpl;
        if (textBoxShapeImpl != null)
        {
            string textLink = textBoxShapeImpl.TextLink;
            if (textLink != null && textLink.Length > 0)
            {
                textLink = textLink.Substring(1, textLink.Length - 1);
                IWorkbook book = textBoxShapeImpl.Workbook;
                IWorksheet sheet = textBoxShapeImpl.Worksheet as IWorksheet;
                if (sheet != null)
                {
                    IRange range = sheet.Range[textLink];
                    IFont font = range.CellStyle.Font;
                    string newGuid = textBoxShapeImpl.FieldId;
                    string fieldType = textBoxShapeImpl.FieldType;

                    if (sheet.CalcEngine == null)
                        sheet.EnableSheetCalculations();

                    if (newGuid == null || newGuid.Length < 0)
                    {
                        newGuid = string.Format("{{{0}}}", Guid.NewGuid().ToString().ToUpper());
                    }
                    if (fieldType == null || fieldType.Length < 0)
                    {
                        fieldType = "TxLink";
                    }
                    writer.WriteStartElement(PivotTable.FieldAttribute, Drawings.ANamespace);
                    writer.WriteAttributeString(Drawings.IdAttributeName, newGuid);
                    writer.WriteAttributeString(Vml.TypeAttributeName, fieldType);
                    //SerializeFormattingRun(writer, font, Drawings.TextRunProperites, textBox.Workbook, range.DisplayText, textBox);
                    SerializeParagraphRunProperites(writer, font, Drawings.TextRunProperites, book, true);
                    writer.WriteStartElement(Drawings.ParagraphText, Drawings.ANamespace);
                    writer.WriteString(range.DisplayText);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
        }
    }
    /// <summary>
    /// Serializes the formatting property.
    /// </summary>
    /// <param name="writer">Represents the Xml writer.</param>
    /// <param name="font">The font.</param>
    /// <param name="book">The book.</param>
    /// <param name="textBox">The text box.</param>
    private static void SerializeFormattingRunProperty(XmlWriter writer, IWorkbook book, TextBoxShapeBase textBox)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (book== null)
            throw new ArgumentNullException("book");

        if (textBox== null)
            throw new ArgumentNullException("textBox");

        writer.WriteStartElement(Drawings.ParagraphProperties, Drawings.ANamespace);

        Excel2007CommentHAlign alignment = (Excel2007CommentHAlign)textBox.HAlignment;
        Excel2007CommentHAlign defaultAlignment = (Excel2007CommentHAlign)ExcelCommentHAlign.Left;
        Excel2007Serializator.SerializeAttribute(writer, Drawings.HorizontalAlignment,
          alignment.ToString(), defaultAlignment.ToString());
        writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes single formatting run.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="font">Font of the formatting run.</param>
    /// <param name="tagName">Represents tag name.</param>
    /// <param name="book">Parent workbook.</param>
    /// <param name="text">Text of the formatting run.</param>
    private static void SerializeFormattingRun( XmlWriter writer, IFont font,
      string tagName, IWorkbook book, string text, TextBoxShapeBase textBox)
    {
      if( text.Length > 0 )
      {
          
        writer.WriteStartElement( Drawings.ParagraphRun, Drawings.ANamespace );
        SerializeParagraphRunProperites(writer, font, tagName, book, false);

        writer.WriteStartElement( Drawings.ParagraphText, Drawings.ANamespace );
        writer.WriteString( text );
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// Serializes paragraph run.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize paragraph tag into.</param>
    /// <param name="textArea">Text area that contains paragraph run information (formatting and text).</param>
    /// <param name="mainTagName">Name of the main xml tag.</param>
    /// <param name="book">Parent workbook.</param>
    public static void SerializeParagraphRunProperites( XmlWriter writer, IFont textArea,
      string mainTagName, IWorkbook book,bool isTextLink )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( textArea == null )
        throw new ArgumentNullException( "textArea" );

      if( mainTagName == null || mainTagName.Length == 0 )
        throw new ArgumentException( "mainTagName" );

      writer.WriteStartElement( mainTagName, Drawings.ANamespace );

      writer.WriteAttributeString( "lang", System.Globalization.CultureInfo.CurrentCulture.Name );//"en-US" );

      string strBold = textArea.Bold ?
        Excel2007Serializator.TrueValue :
        Excel2007Serializator.FalseValue;

      string strItalic = textArea.Italic ?
        Excel2007Serializator.TrueValue :
        Excel2007Serializator.FalseValue;

      writer.WriteAttributeString( Drawings.FontBoldAttribute, strBold );
      writer.WriteAttributeString( Drawings.FontItalicAttribute, strItalic );

      if( textArea.Strikethrough )
        writer.WriteAttributeString( Drawings.FontStrikeAttribute, ChartConstants.StrikeThroughSingle );

      int iSize = ( int )( textArea.Size * 100 );
      writer.WriteAttributeString( Drawings.FontSizeAttribute, iSize.ToString() );

      if( textArea.Underline != ExcelUnderline.None )
      {
        string strUnderline = ( textArea.Underline == ExcelUnderline.Single ) ?
          ChartConstants.UnderlineSingle :
          ChartConstants.UnderlineDouble;
        writer.WriteAttributeString( Drawings.FontUnterlineAttribute, strUnderline );
      }

      int iBaseline = 0;

      if( textArea.Subscript )
        iBaseline = Drawings.SubscriptBaseline;

      if( textArea.Superscript )
        iBaseline = Drawings.SuperscriptBaseline;

      if( iBaseline != 0 )
      {
        writer.WriteAttributeString( Drawings.Baseline, iBaseline.ToString() );
      }

      // TODO: add color serialization later.
      writer.WriteStartElement( Drawings.SolidFillTag, Drawings.ANamespace );
      ChartSerializatorCommon.SerializeRgbColor( writer, textArea.Color, book );
      writer.WriteEndElement();

      if (isTextLink)
      {
          writer.WriteStartElement(Drawings.LatinTag, Drawings.ANamespace);
          writer.WriteAttributeString(Drawings.TypefaceTag, textArea.FontName);
          writer.WriteEndElement();
      }
      else if ((textArea as FontImpl).showFontName)
      {
          writer.WriteStartElement(Drawings.LatinTag, Drawings.ANamespace);
          writer.WriteAttributeString(Drawings.TypefaceTag, textArea.FontName);
          writer.WriteEndElement();
      }

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes paragraph.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize paragraph tag into.</param>
    /// <param name="textArea">Text area that contains paragraph information (formatting and text).</param>
    /// <param name="textBox">Text box to serialize paragraph for.</param>
    internal static void SerializeParagraphsAutoShapes(XmlWriter writer, RichTextString textArea, WorkbookImpl book)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");

        if (textArea == null)
            throw new ArgumentNullException("textArea");

        writer.WriteStartElement(Drawings.Paragraphs, Drawings.ANamespace);

        // TODO: our text area support only one formatting, so we are serializing only one paragraph run.
        //ChartTextAreaImpl textAreaImpl = ( ChartTextAreaImpl )textArea;
        TextWithFormat text = textArea.TextObject;
        int iCurrentPos = 0;
        FontsCollection arrFonts = book.InnerFonts;

        //SerializeFormattingRunProperty(writer, book, textBox);

        if (text.FormattingRunsCount > 0 && text.GetPositionByIndex(0) != 0)
        {
            text = text.TypedClone();
            int defaultFontIndex = textArea.DefaultFontIndex;
            text.FormattingRuns[0] = (defaultFontIndex >= 0) ? defaultFontIndex : 0;
        }

        string strText = text.Text;
        int iRunsCount = text.FormattingRunsCount;
        IFont font;

        for (int i = 0; i < iRunsCount; i++)
        {
            int iFontIndex = text.GetFontByIndex(i);
            //int iPositionStart = text.GetPositionByIndex( i );
            int iPositionEnd = ((i != iRunsCount - 1) ?
              text.GetPositionByIndex(i + 1)
              : strText.Length) - 1;

            font = arrFonts[iFontIndex];
            string strTextToSave = strText.Substring(iCurrentPos, iPositionEnd - iCurrentPos + 1);

            string[] arrTextParts = strTextToSave.Split('\n');

            for (int j = 0, lenJ = arrTextParts.Length; j < lenJ; j++)
            {
                SerializeFormattingRun(writer, font, Drawings.TextRunProperites, book, arrTextParts[j], null);

                if (j != lenJ - 1)
                {
                    //if( arrTextParts[ j ].Length > 0 )
                    SerializeParagraphRunProperites(writer, font, Drawings.ParagraphEndProperties, book,false);

                    writer.WriteEndElement();
                    writer.WriteStartElement(Drawings.Paragraphs, Drawings.ANamespace);
                }
            }

            iCurrentPos = iPositionEnd + 1;
        }

        int iDefaultFontIndex = textArea.DefaultFontIndex;

        if (iDefaultFontIndex < 0) iDefaultFontIndex = 0;

        font = arrFonts[iDefaultFontIndex];//0 ];

        if (iRunsCount == 0 && strText != null && strText.Length > 0)
        {
            SerializeFormattingRun(writer, font, Drawings.TextRunProperites, book, strText, null);
        }

        SerializeParagraphRunProperites(writer, font, Drawings.ParagraphEndProperties, book, false);
        writer.WriteEndElement();
    }
    #endregion
  }
}
