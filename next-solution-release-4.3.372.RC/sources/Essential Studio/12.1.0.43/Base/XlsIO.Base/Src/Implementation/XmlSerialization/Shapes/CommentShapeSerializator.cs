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
  /// Class used for serializing comments.
  /// </summary>
  class CommentShapeSerializator : VmlTextBoxBaseSerializator
  {
    #region Properties
    /// <summary>
    /// Returns instance number of supported shape object. Read-only.
    /// </summary>
    protected override int ShapeInstance
    {
      get
      {
        return CommentShapeImpl.ShapeInstance;
      }
    }
    /// <summary>
    /// Returns string representation of the shape type. Read-only.
    /// </summary>
    protected override string ShapeType
    {
      get
      {
        return "Note";
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Adds required style properties to the list in the format 'name':'value'.
    /// </summary>
    /// <param name="properties">List to add properties to.</param>
    /// <param name="shape">Shape to prepare styles for.</param>
    protected override void PrepareStyleProperties( List<string> properties, ShapeImpl shape )
    {
      base.PrepareStyleProperties( properties, shape );

      CommentShapeImpl comment = shape as CommentShapeImpl;

      if( !comment.IsVisible )
        properties.Add( Vml.VisibilityAttribute + ":" + Vml.VisibilityHiddenValue );

      properties.Add(comment.AutoSize ? (Vml.MsoFitShapeToText + ":" + Vml.TrueExpression) :
          (Vml.MsoFitShapeToText + ":" + Vml.FalseExpression));        
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

      base.SerializeClientDataAdditional( writer, shape );

      CommentShapeImpl comment = shape as CommentShapeImpl;
      writer.WriteElementString( Vml.RowTagName, Vml.XNamespace, ( comment.Row - 1 ).ToString() );
      writer.WriteElementString( Vml.ColumnTagName, Vml.XNamespace, ( comment.Column - 1 ).ToString() );

      //TODO: maybe we have to add some more values here.
    }
    /// <summary>
    /// Serializes subnodes of the shape tag.
    /// </summary>
    /// <param name="writer">XmlWriter to serialize into.</param>
    /// <param name="shape">Shape to serialize for.</param>
    protected override void SerializeShapeNodes( XmlWriter writer, ShapeImpl shape )
    {
      SerializeShadow( writer, shape );
    }
    /// <summary>
    /// Serializes subnodes of the shapetype description.
    /// </summary>
    /// <param name="writer">Writer to serialize into.</param>
    protected override void SerializeShapeTypeSubNodes( XmlWriter writer )
    {
      //  <v:stroke joinstyle="miter"/>
      writer.WriteStartElement( Vml.Stroke, Vml.VNamespace );
      writer.WriteAttributeString( Vml.JoinStyle, Drawings.MiterJoinTag );
      writer.WriteEndElement();

      //  <v:path gradientshapeok="t" o:connecttype="rect"/>
      writer.WriteStartElement( Vml.PathAttribute, Vml.VNamespace );
      writer.WriteAttributeString( Vml.GradientShapeOk, TrueAttributeValue );
      writer.WriteAttributeString( Vml.ConnectTypeAttribute, Vml.ONamespace, "rect" );
      writer.WriteEndElement();
    }
    #endregion
  }
}
