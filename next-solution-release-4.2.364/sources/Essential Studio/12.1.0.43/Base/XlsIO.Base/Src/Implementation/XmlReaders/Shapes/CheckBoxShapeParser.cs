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
using System.Globalization;
using Syncfusion.XlsIO.Implementation.Shapes;
using System.Xml;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Implementation.XmlSerialization;

namespace Syncfusion.XlsIO.Implementation.XmlReaders.Shapes
{
  /// <summary>
  /// Class used to parse check box in Excel 2007 xml format.
  /// </summary>
  class CheckBoxShapeParser : VmlTextBoxBaseParser
  {
    #region Methods
    /// <summary>
    /// Extracts shape type settings from the reader and creates shape with default settings.
    /// </summary>
    /// <param name="reader">XmlReader to get general shape settings from.</param>
    /// <param name="parent">Parent worksheet for the shape.</param>
    /// <returns>Shape with default settings without adding it to any collection</returns>
    public override ShapeImpl ParseShapeType( XmlReader reader, ShapeCollectionBase shapes )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( shapes == null )
        throw new ArgumentNullException( "shapes" );

      // This parser can only parse comment shapes and we don't support anything
      // from shape type, so we simply create empty comment shape.
      reader.Skip();

      CheckBoxShapeImpl result = ( shapes.Application as ApplicationImpl ).CreateCheckBoxShapeImpl( shapes as ShapesCollection );
      result.Display3DShading = true;

      return result;
    }
    /// <summary>
    /// Tries to parse unknown client data tag.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <param name="textBox">Shape to put data into.</param>
    protected override void ParseUnknownClientDataTag( XmlReader reader, TextBoxShapeBase textBox )
    {
      if( reader.NodeType == XmlNodeType.Element )
      {
        CheckBoxShapeImpl checkBox = textBox as CheckBoxShapeImpl;

        switch( reader.LocalName )
        {
          case Vml.Checked:
            checkBox.CheckState = ( ExcelCheckState )reader.ReadElementContentAsInt();
            break;

          case Vml.FormulaLink:
            IWorksheet worksheet = checkBox.Worksheet as IWorksheet;
            WorkbookImpl book = checkBox.ParentWorkbook;
            string strRange = reader.ReadElementContentAsString();
            if (strRange.Contains(":"))
            strRange = strRange.Substring(0,strRange.IndexOf(':'));
            FormulaUtil formulaUtil = book.DataHolder.Parser.FormulaUtil;
            Ptg[] formula = formulaUtil.ParseString(strRange);
            IRangeGetter rangeHolder = formula[0] as IRangeGetter;
            checkBox.LinkedCell = rangeHolder.GetRange(book, worksheet);
            break;

          case Vml.NoThreeD:
            checkBox.Display3DShading = false;
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
    /// <summary>
    /// Registers shape in all necessary collections.
    /// </summary>
    /// <param name="textBox">Shape to register.</param>
    protected override void RegisterShape( TextBoxShapeBase textBox )
    {
      base.RegisterShape( textBox );

      WorksheetImpl sheet = ( WorksheetImpl )textBox.Worksheet;
      sheet.TypedCheckBoxes.AddCheckBox( textBox as ICheckBoxShape );
    }
    #endregion
  }
}
