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
using System.Globalization;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes
{
  /// <summary>
  /// This class is responsible for combo box serialization in Excel 2007 format.
  /// </summary>
  class ComboBoxShapeSerializator : ShapeSerializator
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
      ComboBoxShapeImpl comboBox = shape as ComboBoxShapeImpl;

      if( comboBox.ComboType != ExcelComboType.AutoFilter )
      {
        writer.WriteStartElement( Vml.ShapeTagName, Vml.VNamespace );
        string strShapeType = '#' + string.Format( Vml.ShapeTypeIdFormat, shape.InnerSpRecord.Instance );
        string strShapeId = string.Format( Vml.ShapeIdFormat, shape.ShapeId );

        writer.WriteAttributeString( Vml.ShapeIdAttributeName, strShapeId );
        //writer.WriteAttributeString( "spid", Vml.ONamespace, strShapeId );
        writer.WriteAttributeString( Vml.TypeAttributeName, strShapeType );
        string strLeftMargin = Vml.MarginLeft + ":" + shape.Left.ToString();
        string strTopMargin = Vml.MarginTop + ":" + shape.Top.ToString();
        string strWidth = Vml.Width + ":" + shape.Width.ToString();
        string strHeight = Vml.Height + ":" + shape.Height.ToString();

        string style = string.Format( "{0};{1};{2};{3}", strLeftMargin, strTopMargin, strWidth, strHeight );
        if (!shape.IsShapeVisible)
            style = string.Format("{0};{1}:{2}", style, Vml.VisibilityAttribute, Vml.VisibilityHiddenValue);
        writer.WriteAttributeString( Vml.StyleAttribute, //"position:absolute;z-index:1" );
          style );//"position:absolute;" );

        SerializeClientData( writer, shape as ComboBoxShapeImpl );

        writer.WriteEndElement();
      }
    }
    /// <summary>
    /// This method serializes general shape settings (shape type) into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to write shape type into.</param>
    /// <param name="shapeType">Type of the shape that is going to be serialized.</param>
    public override void SerializeShapeType( XmlWriter writer, Type shapeType )
    {
      //<v:shapetype
      writer.WriteStartElement( Vml.ShapeTypeTagName, Vml.VNamespace );

      string strShapeTypeId = string.Format( Vml.ShapeTypeIdFormat, ComboBoxShapeImpl.ShapeInstance );
      //id="_x0000_t201"
      writer.WriteAttributeString( Vml.ShapeIdAttributeName, strShapeTypeId );

      //coordsize="21600,21600"
      writer.WriteAttributeString( Vml.CoordSizeAttributeName, Vml.CommentCoordSize );
      // o:spt="201"
      writer.WriteAttributeString( Vml.SptAttriubteName, Vml.ONamespace,
        ComboBoxShapeImpl.ShapeInstance.ToString() );
      // path="m,l,21600r21600,l21600,xe">
      writer.WriteAttributeString( Vml.PathAttributeName, Vml.CommentPathValue );

      
      //<v:stroke joinstyle="miter"/>
      //<v:path shadowok="f" o:extrusionok="f" strokeok="f" fillok="f" o:connecttype="rect"/>
      //<o:lock v:ext="edit" shapetype="t"/>
      //</v:shapetype>
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes client data.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize data into.</param>
    /// <param name="comboBox">Shape to serialize client data for.</param>
    private void SerializeClientData( XmlWriter writer, ComboBoxShapeImpl comboBox )
    {
      writer.WriteStartElement( Vml.ClientDataTagName, Vml.XNamespace );
      writer.WriteAttributeString( Vml.ObjectTypeAttribute, Vml.Drop );

      writer.WriteElementString( Vml.SizeWithCellsTagName, Vml.XNamespace,
        ( !comboBox.IsSizeWithCell ).ToString() );

      string strAnchorValue = GetAnchorValue( comboBox );
      writer.WriteElementString( Vml.AnchorTagName, Vml.XNamespace, strAnchorValue );

      writer.WriteElementString( Vml.AutoLineTag, Vml.XNamespace, "False" );

      if (comboBox.FormulaMacro != null && comboBox.FormulaMacro.Length > 0)
          writer.WriteElementString(Vml.FormulaMacro, Vml.XNamespace, comboBox.FormulaMacro);

      IRange cellLink = comboBox.LinkedCell;

      if( cellLink != null )
        writer.WriteElementString( Vml.FormulaLink, Vml.XNamespace, cellLink.Address );
      
      //<x:Val>2</x:Val>
      writer.WriteElementString( Vml.ScrollPosition, Vml.XNamespace, "2" );
      //<x:Min>0</x:Min>
      writer.WriteElementString( Vml.ScrollMinimum, Vml.XNamespace, "0" );
      //<x:Max>2</x:Max>
      writer.WriteElementString( Vml.ScrollMaximum, Vml.XNamespace, "2" );
      //<x:Inc>1</x:Inc>
      writer.WriteElementString( Vml.ScrollIncrement, Vml.XNamespace, "1" );
      //<x:Page>8</x:Page>
      writer.WriteElementString( Vml.ScrollPageIncrement, Vml.XNamespace, "8" );
      //<x:Dx>15</x:Dx>
      writer.WriteElementString( Vml.ScrollBarWidth, Vml.XNamespace, "15" );
      //<x:FmlaRange>$A$1:$A$10</x:FmlaRange>

      IRange range = comboBox.ListFillRange;

      if( range != null )
        writer.WriteElementString( Vml.ListSourceRange, Vml.XNamespace, range.AddressGlobal );

      //<x:Sel>7</x:Sel>
      writer.WriteElementString( Vml.SelectedItem, Vml.XNamespace, comboBox.SelectedIndex.ToString() );
      //<x:NoThreeD2/>

      if( !comboBox.Display3DShading )
        writer.WriteElementString( Vml.NoThreeD2, Vml.XNamespace, string.Empty );

      //<x:SelType>Single</x:SelType>
      writer.WriteElementString( Vml.SelectionType, Vml.XNamespace, Vml.SelectionTypes.Single.ToString() );
      //<x:LCT>Normal</x:LCT>
      writer.WriteElementString( Vml.CallbackType, Vml.XNamespace, Vml.NormalLCT );
      //<x:DropStyle>Combo</x:DropStyle>
      writer.WriteElementString( Vml.DropStyle, Vml.XNamespace, Vml.DropStyles.Combo.ToString() );
      //<x:DropLines>8</x:DropLines>
      writer.WriteElementString( Vml.DropLines, Vml.XNamespace, comboBox.DropDownLines.ToString() );
      writer.WriteEndElement();
    }
  }
}
