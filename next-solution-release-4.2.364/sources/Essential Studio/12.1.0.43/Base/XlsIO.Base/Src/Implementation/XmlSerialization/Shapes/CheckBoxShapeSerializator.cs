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

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes
{
  /// <summary>
  /// Class used for serializing Checkbox.
  /// </summary>
  class CheckBoxShapeSerializator : VmlTextBoxBaseSerializator
  {
    #region Properties
    /// <summary>
    /// Returns instance number of supported shape object. Read-only.
    /// </summary>
    protected override int ShapeInstance
    {
      get
      {
        return CheckBoxShapeImpl.ShapeInstance;
      }
    }
    /// <summary>
    /// Returns string representation of the shape type. Read-only.
    /// </summary>
    protected override string ShapeType
    {
      get
      {
        return Vml.Checkbox;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Serialize the check box shape
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
      CheckBoxShapeImpl checkbox = shape as CheckBoxShapeImpl;
      writer.WriteStartElement( Vml.ShapeTagName, Vml.VNamespace );

      SerializeShapeNameAndType( writer, shape );

      //string strShapeType = '#' + string.Format( Vml.ShapeTypeIdFormat, shape.InnerSpRecord.Instance );
      //string strShapeId = string.Format( Vml.ShapeIdFormat, shape.ShapeId );

      //if( !string.IsNullOrEmpty( checkbox.Name ) )
      //{
      //  writer.WriteAttributeString( Vml.ShapeIdAttributeName, checkbox.Name );
      //  writer.WriteAttributeString( Vml.SpIdAttributeName, Vml.ONamespace, strShapeId );
      //}
      //else
      //{
      //  writer.WriteAttributeString( Vml.ShapeIdAttributeName, strShapeId );
      //}


      //writer.WriteAttributeString( Vml.TypeAttributeName, strShapeType );
      SerializeShapeStyle( writer, shape );

      // Fill Style
      if (!checkbox.HasFill)
      {
          //f- false No fill
          writer.WriteAttributeString(Vml.FilledAttribute, FalseAttributeValue);

          //if (checkbox.FillColor != ColorExtension.Empty)
          {
              string strColor = GenerateHexColor(checkbox.Fill.BackColor);
              writer.WriteAttributeString(Vml.FillColorAttribute, strColor);
          }
      }
      else if( checkbox.Fill.FillType == ExcelFillType.SolidColor )
      {

        if( IsEmptyColor(checkbox.Fill.ForeColor) )
          checkbox.HasFill = false;
        else
        {
          string strColor = GenerateHexColor( checkbox.Fill.ForeColor );
          writer.WriteAttributeString( Vml.FillColorAttribute, strColor );
        }
      }
      if( !checkbox.HasLineFormat )
      {
        writer.WriteAttributeString( Vml.StrokedAttribute, FalseAttributeValue );
      }
      else
      {
        if( checkbox.Line.BackColor != ColorExtension.Empty )
        {
          string strColor = GenerateHexColor( checkbox.Line.BackColor );
          writer.WriteAttributeString( Vml.StrokeColorAttribute, strColor );
        }
        if( checkbox.Line.Weight > 0.0 )
        {
          string weight = checkbox.Line.Weight.ToString() + Vml.SizeInPoints;
          writer.WriteAttributeString( Vml.StrokeWeightAttribute, weight );
        }
      }
      if( checkbox.AlternativeText != null )
        writer.WriteAttributeString( Vml.AlternateTextAttribute, checkbox.AlternativeText );

      SerializeShapeTagAttribute( writer, shape );
      writer.WriteAttributeString( Vml.InsetModeAttribute, Vml.ONamespace, "auto" );

      if (checkbox.HasFill && !(!IsEmptyColor(checkbox.Fill.ForeColor) && checkbox.Fill.FillType == ExcelFillType.SolidColor))
        SerializeFill( writer, checkbox, holder, vmlRelations );

      if( checkbox.HasLineFormat )
        SerializeLine( writer, checkbox, holder.ParentHolder, vmlRelations );

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

      CheckBoxShapeImpl checkBox = shape as CheckBoxShapeImpl;

      writer.WriteElementString( Vml.AutoFillTag, Vml.XNamespace, "False" );
      writer.WriteElementString( Vml.AutoLineTag, Vml.XNamespace, "False" );
      int iChecked = ( int )checkBox.CheckState;

      if( iChecked != 0 )
        writer.WriteElementString( Vml.Checked, Vml.XNamespace, iChecked.ToString() );

      if( !checkBox.Display3DShading )
        writer.WriteElementString( Vml.NoThreeD, Vml.XNamespace, string.Empty );

      if( checkBox.LinkedCell != null )
      {
        string strFormulaLink = checkBox.LinkedCell.AddressGlobal;
        writer.WriteElementString( Vml.FormulaLink, Vml.XNamespace, strFormulaLink );
      }
    }
    /// <summary>
    /// Serialize internal part of the div object.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize.</param>
    protected override void SerializeDiv( XmlWriter writer, ShapeImpl shape )
    {
      CheckBoxShapeImpl checkBox = ( CheckBoxShapeImpl )shape;
      IRichTextString rtf = checkBox.RichText;
      string text = rtf.Text;//checkBox.Name;

      if( text != null && text.Length > 0 )
      {
        IFont font = checkBox.Workbook.CreateFont();
        font.Size -= 2;
        //SerializeFont( font );
        writer.WriteStartElement( Vml.FontTag );
        writer.WriteAttributeString( Vml.Face, font.FontName );
        writer.WriteAttributeString( Vml.Size, ( font.Size * 20 ).ToString() );
        //int iColor = font.RGBColor.ToArgb() & 0xFFFFFF;
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
      writer.WriteAttributeString( Vml.ShadowOkAttribute, FalseAttributeValue );
      writer.WriteAttributeString( Vml.ExtrusionOkAttribute, Vml.ONamespace, FalseAttributeValue );
      writer.WriteAttributeString( Vml.StrokeOkAttribute, FalseAttributeValue );
      writer.WriteAttributeString( Vml.FillOkAttribute, FalseAttributeValue );
      writer.WriteAttributeString( Vml.ConnectTypeAttribute, Vml.ONamespace, "rect" );
      writer.WriteEndElement();

      writer.WriteStartElement( Vml.LockTypeAttribute, Vml.ONamespace );
      writer.WriteAttributeString( Vml.ExtAttribute, Vml.VNamespace, "edit" );
      writer.WriteAttributeString( Vml.ShapeTypeAttribute, "t" );
      writer.WriteEndElement();
    }
    #endregion
  }
}
