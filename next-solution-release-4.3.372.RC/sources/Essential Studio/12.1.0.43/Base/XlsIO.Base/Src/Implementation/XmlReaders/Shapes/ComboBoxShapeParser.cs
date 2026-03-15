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
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using Syncfusion.XlsIO.Implementation.Collections;
using System.Globalization;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

namespace Syncfusion.XlsIO.Implementation.XmlReaders.Shapes
{
  /// <summary>
  /// This class is responsible for combo box parsing.
  /// </summary>
  class ComboBoxShapeParser : ShapeParser
  {
    #region Class constants
    private const string REF_ERROR = "#REF!";
      #endregion

    #region Methods
    /// <summary>
    /// Extracts shape type settings from the reader and creates shape with default settings.
    /// </summary>
    /// <param name="reader">XmlReader to get general shape settings from.</param>
    /// <param name="parent">Parent worksheet for the shape.</param>
    /// <returns>
    /// Shape with default settings without adding it to any collection.
    /// </returns>
    public override ShapeImpl ParseShapeType( XmlReader reader, ShapeCollectionBase shapes )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( shapes == null )
        throw new ArgumentNullException( "shapes" );

      reader.Skip();

      ComboBoxShapeImpl result = ( shapes.Application as ApplicationImpl ).CreateComboBoxShapeImpl( shapes );
      return result;
    }
    /// <summary>
    /// Parses shape and adds it to all necessary shapes collections.
    /// </summary>
    /// <param name="reader">XmlReader to get shape from.</param>
    /// <param name="defaultShape">Default shape that must be cloned to get resulting shape.</param>
    /// <param name="relations">Corresponding relations collection.</param>
    /// <param name="parentItemPath">Path to the parent item (item which holds all these xml tags).</param>
    public override bool ParseShape( XmlReader reader, ShapeImpl defaultShape,
      RelationCollection relations, string parentItemPath )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( defaultShape == null )
        throw new ArgumentNullException( "defaultShape" );

      // Since clone method adds shape to the collection, we have to fill default
      // shape with correct values and then call clone.
      ComboBoxShapeImpl comboBox = ( ComboBoxShapeImpl )defaultShape.Clone( defaultShape.Parent,
        null, null, false );

      VmlTextBoxBaseParser.ParseShapeId( reader, comboBox );

      if( reader.MoveToAttribute( Vml.StyleAttribute ) )
      {
        string strStyle = reader.Value;
        ParseStyle( strStyle, comboBox );
        reader.MoveToElement();
      }

      bool bResult = false;

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        bResult = true;

        while( reader.NodeType != XmlNodeType.EndElement && bResult )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Vml.ClientDataTagName:
                bResult = ParseClientData( reader, comboBox );
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }

      reader.Read();

      if( bResult )
        RegisterShape( comboBox );

      return bResult;
    }

    private void ParseStyle( string strStyle, ShapeImpl shape )
    {
      Dictionary<string, string> dictStyles = SplitStyle( strStyle );

      shape.Left = ( int )GetValue( dictStyles, Vml.MarginLeft );
      shape.Top = ( int )GetValue( dictStyles, Vml.MarginTop );
      shape.Width = ( int )GetValue( dictStyles, Vml.Width );
      shape.Height = ( int )GetValue( dictStyles, Vml.Height );
      if (dictStyles.ContainsKey(Vml.VisibilityAttribute))
      {
          if(dictStyles [Vml .VisibilityAttribute] == Vml .VisibilityHiddenValue)
              shape .IsShapeVisible = false ;
      }
        
       
    }

    private double GetValue( Dictionary<string, string> dictStyles, string tagName )
    {
      double dValue = 0;
      string value;

      if( dictStyles.TryGetValue( tagName, out value ) )
      {
        string units = null;
        MeasureUnits measureUnits = MeasureUnits.Pixel;

        if( value.Length >= 2 )
        {
          units = value.Substring( value.Length - 2 );

          if( !Char.IsNumber( units[ 1 ] ) )
            value = value.Substring( 0, value.Length - 2 );

          switch( units )
          {
            case Vml.Millimeters:
              measureUnits = MeasureUnits.Millimeter;
              break;
          }
        }

        dValue = double.Parse( value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture );
        dValue = ApplicationImpl.ConvertToPixels( dValue, measureUnits );
      }

      return dValue;
    }
    /// <summary>
    /// Parses client data tag and all its internal tags.
    /// </summary>
    /// <param name="reader">Reader to get necessary values from.</param>
    /// <param name="comboBox">Shape to parse client data for.</param>
    private bool ParseClientData( XmlReader reader, ComboBoxShapeImpl comboBox )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( comboBox == null )
        throw new ArgumentNullException( "comboBox" );

      if( reader.LocalName != Vml.ClientDataTagName )
        throw new XmlException( "Unexpected xml token" );

      if( reader.MoveToAttribute( Vml.ObjectTypeAttribute ) && reader.Value != Vml.Drop )
        return false;

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        WorkbookImpl book = comboBox.ParentWorkbook;
        IWorksheet sheet = comboBox.Worksheet as IWorksheet;

        if( sheet == null )
        {
          IWorksheets sheets = book.Worksheets;

          if( sheets.Count > 0 )
            sheet = book.Worksheets[ 0 ];
        }

        string strRange;
        comboBox.Display3DShading = true;
        FormulaUtil formulaUtil;

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Vml.FormulaLink:
                strRange = reader.ReadElementContentAsString();
                formulaUtil = book.DataHolder.Parser.FormulaUtil;
                Ptg[] formula = formulaUtil.ParseString(strRange);
                IRangeGetter rangeHolder = formula[0] as IRangeGetter;
                 comboBox.LinkedCell = rangeHolder.GetRange(book, sheet);
                break;

              case Vml.ListSourceRange:
                strRange = reader.ReadElementContentAsString();

                if (strRange != REF_ERROR)
                {
                  formulaUtil = book.DataHolder.Parser.FormulaUtil;
                  Ptg[] token = formulaUtil.ParseString( strRange );
                  IRangeGetter rangeGetter = token[0] as IRangeGetter;
                  comboBox.ListFillRange = rangeGetter.GetRange( book, sheet );
                }
                break;

              case Vml.SelectedItem:
                comboBox.SelectedIndex = reader.ReadElementContentAsInt();
                break;

              case Vml.AnchorTagName:
                if( comboBox.Worksheet is WorksheetImpl )
                {
                  ParseAnchor( reader, comboBox );
                }
                else
                {
                  goto default;
                }
                break;

              case Vml.DropLines:
                comboBox.DropDownLines = reader.ReadElementContentAsInt();
                break;

              case Vml.NoThreeD2:
                comboBox.Display3DShading = false;
                reader.Read();
                break;

              case Vml.FormulaMacro:
                comboBox.FormulaMacro = reader.ReadElementContentAsString();
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Read();
          }
        }
      }

      reader.Read();

      return true;
    }
    /// <summary>
    /// Registers shape in all necessary collections.
    /// </summary>
    /// <param name="comboBox">Shape to register.</param>
    protected virtual void RegisterShape( ComboBoxShapeImpl comboBox )
    {
      if( comboBox == null )
        throw new ArgumentNullException( "comboBox" );

      WorksheetBaseImpl sheet = comboBox.Worksheet;
      //sheet.InnerComments.AddComment( textBox );
      sheet.InnerShapes.AddShape( comboBox );
      sheet.TypedComboBoxes.AddComboBox( comboBox );
    }
    #endregion
  }
}
