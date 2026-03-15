#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Xml;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes
{
  /// <summary>
  /// Class used for serializing Option button.
  /// </summary>
  class OptionButtonShapeSerializator : VmlTextBoxBaseSerializator
  {

    #region Properties
    /// <summary>
    /// Returns instance number of supported shape object. Read-only.
    /// </summary>
    protected override int ShapeInstance
    {
      get
      {
        return OptionButtonShapeImpl.ShapeInstance;
      }
    }
    /// <summary>
    /// Returns string representation of the shape type. Read-only.
    /// </summary>
    protected override string ShapeType
    {
      get
      {
        return Vml.OptionButton;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Serializes additional tag into ClientData tag.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize for.</param>
    protected override void SerializeClientDataAdditional( XmlWriter writer, ShapeImpl shape )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      OptionButtonShapeImpl optionButton = shape as OptionButtonShapeImpl;

      writer.WriteElementString( Vml.AutoFillTag, Vml.XNamespace, "False" );
      writer.WriteElementString( Vml.AutoLineTag, Vml.XNamespace, "False" );

      if( !optionButton.IsTextLocked )
        writer.WriteElementString( Vml.LockText, Vml.XNamespace, "False" );

      int iChecked = ( int )optionButton.CheckState;

      if( iChecked != 0 )
        writer.WriteElementString( Vml.Checked, Vml.XNamespace, iChecked.ToString() );

      if( optionButton.LinkedCell != null && optionButton.IsFirstButton )
      {
        string strFormulaLink = optionButton.LinkedCell.AddressGlobal;
        writer.WriteElementString( Vml.FormulaLink, Vml.XNamespace, strFormulaLink );
      }

      if( !optionButton.Display3DShading )
        writer.WriteElementString( Vml.NoThreeD, Vml.XNamespace, string.Empty );
      if( optionButton.IsFirstButton )

        writer.WriteElementString( Vml.FirstButton, Vml.XNamespace, string.Empty );

    }
    /// <summary>
    /// Serialize internal part of the div object.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize.</param>
    protected override void SerializeDiv( XmlWriter writer, ShapeImpl shape )
    {
      OptionButtonShapeImpl optionButton = ( OptionButtonShapeImpl )shape;

      string text = optionButton.Text;
      IFont font = optionButton.Workbook.CreateFont();
      font.Size -= 2; // ???

      if( text != null && text.Length > 0 )
      {
        writer.WriteStartElement( Vml.FontTag );
        writer.WriteAttributeString( Vml.Face, font.FontName );
        writer.WriteAttributeString( Vml.Size, ( font.Size * 20 ).ToString() );
        writer.WriteAttributeString( Vml.Color, "auto" );//"#" + iColor.ToString( "X6" ) );
        writer.WriteString( text );

        writer.WriteEndElement();
      }
    }

    /// <summary>
    /// Serializes subnodes of the shapetype description.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    protected override void SerializeShapeTypeSubNodes( XmlWriter writer )
    {
      //<v:stroke joinstyle="miter"/>
      //<v:path shadowok="f" o:extrusionok="f" strokeok="f" fillok="f" o:connecttype="rect"/>
      //<o:lock v:ext="edit" shapetype="t"/>

      writer.WriteStartElement( Vml.Stroke, Vml.VNamespace );
      writer.WriteAttributeString( Vml.JoinStyle, "miter" );
      writer.WriteEndElement();

      writer.WriteStartElement( Vml.PathAttribute, Vml.VNamespace );
      writer.WriteAttributeString( Vml.ShadowOkAttribute, VmlTextBoxBaseSerializator.FalseAttributeValue );
      writer.WriteAttributeString( Vml.ExtrusionOkAttribute, Vml.ONamespace, VmlTextBoxBaseSerializator.FalseAttributeValue );
      writer.WriteAttributeString( Vml.StrokeOkAttribute, VmlTextBoxBaseSerializator.FalseAttributeValue );
      writer.WriteAttributeString( Vml.FillOkAttribute, VmlTextBoxBaseSerializator.FalseAttributeValue );
      writer.WriteAttributeString( Vml.ConnectTypeAttribute, Vml.ONamespace, "rect" );
      writer.WriteEndElement();

      writer.WriteStartElement( Vml.LockTypeAttribute, Vml.ONamespace );
      writer.WriteAttributeString( Vml.ExtAttribute, Vml.VNamespace, "edit" );
      writer.WriteAttributeString( Vml.ShapeTypeAttribute, "t" );
      writer.WriteEndElement();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer">XmlWriter to serialize fill color from.</param>
    /// <param name="shape">shae to set fill color to.</param>
    /// <param name="relations">relation Collection of the item</param>
    /// <param name="parentItemPath"> path of the item</param>
    public override void Serialize( XmlWriter writer, ShapeImpl shape, WorksheetDataHolder holder,
 RelationCollection vmlRelations )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );
      /*
        - <v:shape id="_x0000_s1025" type="#_x0000_t202" style="position:absolute; margin-left:59.25pt;margin-top:1.5pt;width:96pt;height:55.5pt;z-index:1; visibility:hidden" fillcolor="#ffffe1" o:insetmode="auto">
          <v:fill color2="#ffffe1" /> 
          <v:shadow on="t" color="black" obscured="t" /> 
          <v:path o:connecttype="none" /> 
        - <v:textbox style="mso-direction-alt:auto">
          <div style="text-align:left" /> 
          </v:textbox>
        - <x:ClientData ObjectType="Note">
          <x:MoveWithCells /> 
          <x:SizeWithCells /> 
          <x:Anchor>1, 15, 0, 2, 3, 15, 3, 16</x:Anchor> 
          <x:AutoFill>False</x:AutoFill> 
          <x:Row>0</x:Row> 
          <x:Column>0</x:Column> 
          </x:ClientData>
          </v:shape>
       */
      TextBoxShapeBase textBox = shape as TextBoxShapeBase;
      writer.WriteStartElement( Vml.ShapeTagName, Vml.VNamespace );
      SerializeShapeNameAndType( writer, shape );
      //string strShapeType = '#' + string.Format( Vml.ShapeTypeIdFormat, shape.InnerSpRecord.Instance );
      //string strShapeId = string.Format( Vml.ShapeIdFormat, shape.ShapeId );

      //writer.WriteAttributeString( Vml.ShapeIdAttributeName, strShapeId );
      //writer.WriteAttributeString( Vml.TypeAttributeName, strShapeType );
      SerializeShapeStyle( writer, shape );
      if( textBox.Fill.FillType == ExcelFillType.SolidColor )
      {
        if( IsEmptyColor( textBox.Fill.ForeColor ) )
        {
          textBox.HasFill = false;
        }
        else
        {
          string strColor = GenerateHexColor( textBox.Fill.ForeColor );
          writer.WriteAttributeString( Vml.FillColorAttribute, strColor );
        }
      }

      if( !textBox.HasFill )
      {
        writer.WriteAttributeString( Vml.FilledAttribute, VmlTextBoxBaseSerializator.FalseAttributeValue );
      }
      if( !textBox.HasLineFormat )
      {
        writer.WriteAttributeString( Vml.StrokedAttribute, VmlTextBoxBaseSerializator.FalseAttributeValue );
      }
      if( textBox.Line.BackColor != ColorExtension.Empty )
      {
        string strColor = GenerateHexColor( textBox.Line.BackColor );
        writer.WriteAttributeString( Vml.StrokeColorAttribute, strColor );
      }
      if( textBox.Line.Weight > 0.0 )
      {
        string weight = textBox.Line.Weight.ToString() + Vml.SizeInPoints;
        writer.WriteAttributeString( Vml.StrokeWeightAttribute, weight );
      }

      if( textBox.AlternativeText != null )
        writer.WriteAttributeString( Vml.AlternateTextAttribute, textBox.AlternativeText );

      SerializeShapeTagAttribute( writer, shape );
      writer.WriteAttributeString( Vml.InsetModeAttribute, Vml.ONamespace, "auto" );

      if( textBox.HasFill )
        SerializeFill( writer, shape, holder, vmlRelations );

      if( textBox.HasLineFormat )
        SerializeLine( writer, textBox, holder.ParentHolder, vmlRelations );

      writer.WriteStartElement( Vml.TextBoxTagName, Vml.VNamespace );
      SerializeTextBoxStyle( writer, shape );
      writer.WriteStartElement( Vml.DivTagName );
      writer.WriteAttributeString( Vml.StyleAttribute, "text-align:left" );
      SerializeDiv( writer, shape );
      writer.WriteEndElement();
      writer.WriteEndElement();

      SerializeClientData( writer, shape, ShapeType );

      writer.WriteEndElement();
    }

    #endregion

  }
}
