#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Resources;
using System.Text;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;
using Syncfusion.XlsIO.Implementation.Shapes;
#if ( WINRT )
using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif

namespace Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes
{
  /// <summary>
  /// Class used for serializing text box.
  /// </summary>
  abstract class VmlTextBoxBaseSerializator : ShapeSerializator
  {
    #region Properties
    /// <summary>
    /// Gets instance of the shape object. Read-only.
    /// </summary>
    protected abstract int ShapeInstance { get; }
    /// <summary>
    /// Gets string representation of the shape type. Read-only.
    /// </summary>
    protected abstract string ShapeType { get; }
    #endregion

    #region Methods
    /// <summary>
    /// This method serializes general shape settings (shape type) into specified XmlWriter.
    /// </summary>
    /// <param name="writer">XmlWriter to write shape type into.</param>
    /// <param name="shapeType">Type of the shape that is going to be serialized.</param>
    public override void SerializeShapeType( XmlWriter writer, Type shapeType )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      // We should generate something like this here.
      //<v:shapetype id="_x0000_t202" coordsize="21600,21600" o:spt="202" path="m,l,21600r21600,l21600,xe">
      // <v:stroke joinstyle="miter" /> 
      //<v:path gradientshapeok="t" o:connecttype="rect" /> 
      //</v:shapetype>

      writer.WriteStartElement( Vml.ShapeTypeTagName, Vml.VNamespace );

      string strShapeTypeId = string.Format( Vml.ShapeTypeIdFormat, ShapeInstance );
      writer.WriteAttributeString( Vml.ShapeIdAttributeName, strShapeTypeId );

      // TODO: find out what does this mean or/and move into constants.
      writer.WriteAttributeString( Vml.CoordSizeAttributeName, Vml.CommentCoordSize );
      writer.WriteAttributeString( Vml.SptAttriubteName, Vml.ONamespace,
        ShapeInstance.ToString() );

      writer.WriteAttributeString( Vml.PathAttributeName, Vml.CommentPathValue );
      SerializeShapeTypeSubNodes( writer );

      // TODO: Add some additional values if they are necessary.

      writer.WriteEndElement();
    }
    /// <summary>
    /// Serializes subnodes of the shapetype description.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    protected virtual void SerializeShapeTypeSubNodes( XmlWriter writer )
    {
    }
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
      //shape.PrepareForSerialization();
      writer.WriteStartElement( Vml.ShapeTagName, Vml.VNamespace );
      string strShapeType = '#' + string.Format( Vml.ShapeTypeIdFormat, shape.InnerSpRecord.Instance );
      string strShapeId = string.Format( Vml.ShapeIdFormat, shape.ShapeId );

      writer.WriteAttributeString( Vml.ShapeIdAttributeName, strShapeId );
      //writer.WriteAttributeString( "spid", Vml.ONamespace, strShapeId );
      writer.WriteAttributeString( Vml.TypeAttributeName, strShapeType );
      SerializeShapeStyle( writer, shape );

      // Fill Style
      if( !textBox.HasFill )
      {
        //f- false No fill
        writer.WriteAttributeString( Vml.FilledAttribute, FalseAttributeValue );

        if( textBox.FillColor != ColorExtension.Empty )
        {
          string strColor = GenerateHexColor( textBox.Fill.BackColor );
          writer.WriteAttributeString( Vml.FillColorAttribute, strColor );
        }
      }

      if( !textBox.HasLineFormat )
      {
        //f-false no line
        writer.WriteAttributeString( Vml.StrokedAttribute, FalseAttributeValue );

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
      }

      SerializeShapeTagAttribute( writer, shape );
      writer.WriteAttributeString( Vml.InsetModeAttribute, Vml.ONamespace, "auto" );

      if( textBox.HasFill )
        SerializeFill( writer, shape, holder, vmlRelations );

      if( textBox.HasLineFormat )
        SerializeLine( writer, textBox, holder.ParentHolder, vmlRelations );

      SerializeShapeNodes( writer, shape );

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
    /// Serializes attributes of the shape tag if necessary.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize for.</param>
    protected virtual void SerializeShapeTagAttribute( XmlWriter writer, ShapeImpl shape )
    {
    }
    /// <summary>
    /// Serializes subnodes of the shape tag.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize for.</param>
    protected virtual void SerializeShapeNodes( XmlWriter writer, ShapeImpl shape )
    {
    }
    /// <summary>
    /// Serializes shadow settings.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize for.</param>
    protected void SerializeShadow( XmlWriter writer, ShapeImpl shape )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      writer.WriteStartElement( Vml.ShadowTagName, Vml.VNamespace );
      writer.WriteAttributeString( Vml.ShadowOnAttribute, TrueAttributeValue );
      writer.WriteAttributeString( Vml.ShadowColorAttribute, "black" );
      writer.WriteAttributeString( Vml.ShadowObscuredAttribute, TrueAttributeValue );
      writer.WriteEndElement();
    }
    /// <summary>
    /// Serialize internal part of the div object.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize.</param>
    protected virtual void SerializeDiv( XmlWriter writer, ShapeImpl shape )
    {
    }
    /// <summary>
    /// Serializes style attribute for the text box shape.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    /// <param name="shape">Shape to serialize style attribute for.</param>
    protected void SerializeTextBoxStyle( XmlWriter writer, ShapeImpl shape )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      List<string> arrStyleProperties = new List<string>();
      PrepareStyleProperties( arrStyleProperties, shape );

      string strStyle = UtilityMethods.Join( ";", arrStyleProperties );
      writer.WriteAttributeString( Vml.StyleAttribute, strStyle );
    }
    /// <summary>
    /// Serializes shape's style.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize style for.</param>
    protected void SerializeShapeStyle( XmlWriter writer, ShapeImpl shape )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      List<string> arrStyleProperties = new List<string>();
      PrepareStyleProperties( arrStyleProperties, shape );
      SerializeStyle( writer, arrStyleProperties );
    }
    public static void SerializeStyle( XmlWriter writer, List<string> styleProperties )
    {
      if( styleProperties != null && styleProperties.Count > 0 )
      {
        string strStyle = UtilityMethods.Join( ";", styleProperties );
        writer.WriteAttributeString( Vml.StyleAttribute, strStyle );
      }
    }
    /// <summary>
    /// Adds required style properties to the list in the format 'name':'value'.
    /// </summary>
    /// <param name="properties">List to add properties to.</param>
    /// <param name="shape">Shape to prepare styles for.</param>
    protected virtual void PrepareStyleProperties( List<string> properties, ShapeImpl shape )
    {
        if (shape.StyleProperties.Count > 0)
        {
            foreach (KeyValuePair<string, string> keyValue in shape.StyleProperties)
                properties.Add(keyValue.Key+':'+ keyValue.Value);
            return;
        }
      properties.Add( "mso-direction-alt:auto" );
      TextBoxShapeBase textBox = shape as TextBoxShapeBase;

      if( textBox != null )
      {
        ExcelTextRotation rotation = textBox.TextRotation;

        switch( rotation )
        {
          case ExcelTextRotation.TopToBottom:
            properties.Add( Vml.LayoutFlow + ':' + Vml.LayoutFlowVertical );
            properties.Add( Vml.MsoLayoutFlow + ':' + Vml.MsoLayoutFlowTopToBottom );
            break;

          case ExcelTextRotation.CounterClockwise:
            properties.Add( Vml.LayoutFlow + ':' + Vml.LayoutFlowVertical );
            properties.Add( Vml.MsoLayoutFlow + ':' + Vml.MsoLayoutFlowBottomToTop );
            break;

          case ExcelTextRotation.Clockwise:
            properties.Add( Vml.LayoutFlow + ':' + Vml.LayoutFlowVertical );
            break;
        }

        if( !textBox.IsShapeVisible )
          properties.Add( Vml.VisibilityAttribute + ':' + Vml.VisibilityHiddenValue );
      }
    }
    /// <summary>
    /// Serialize client Additional Data
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize style for.</param>
    protected override void SerializeClientDataAdditional( XmlWriter writer, ShapeImpl shape )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      TextBoxShapeBase textBox = shape as TextBoxShapeBase;

      if( textBox.HAlignment != ExcelCommentHAlign.Left )
        writer.WriteElementString( Vml.TextHAlign, Vml.XNamespace, textBox.HAlignment.ToString() );

      if( textBox.VAlignment != ExcelCommentVAlign.Top )
        writer.WriteElementString( Vml.TextVAlign, Vml.XNamespace, textBox.VAlignment.ToString() );

      if( !textBox.IsTextLocked )
        writer.WriteElementString( Vml.LockText, Vml.XNamespace, "False" );

      base.SerializeClientDataAdditional( writer, shape );
    }
    protected void SerializeShapeNameAndType( XmlWriter writer, ShapeImpl shape )
    {
      string strShapeType = '#' + string.Format( Vml.ShapeTypeIdFormat, shape.InnerSpRecord.Instance );
      string strShapeId = string.Format( Vml.ShapeIdFormat, shape.ShapeId );

      if( !string.IsNullOrEmpty( shape.Name ) )
      {
        writer.WriteAttributeString( Vml.ShapeIdAttributeName, shape.Name );
        writer.WriteAttributeString( Vml.SpIdAttributeName, Vml.ONamespace, strShapeId );
      }
      else
      {
        writer.WriteAttributeString( Vml.ShapeIdAttributeName, strShapeId );
      }

      writer.WriteAttributeString( Vml.TypeAttributeName, strShapeType );
    }
    #endregion
  }
}
