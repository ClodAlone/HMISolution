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
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Implementation.Collections;

namespace Syncfusion.XlsIO.Implementation.XmlReaders.Shapes
{
  /// <summary>
  /// This class is responsible for vml comment shape parsing.
  /// </summary>
  class CommentShapeParser : VmlTextBoxBaseParser
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

      CommentShapeImpl result = ( shapes.Application as ApplicationImpl ).CreateCommentShapeImpl( shapes );
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
        CommentShapeImpl comment = textBox as CommentShapeImpl;
        switch( reader.LocalName )
        {
          case Vml.RowTagName:
            comment.Row = reader.ReadElementContentAsInt() + 1;
            break;

          case Vml.ColumnTagName:
            comment.Column = reader.ReadElementContentAsInt() + 1;
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
    /// Parses style properties.
    /// </summary>
    /// <param name="textBox">Textbox to put properties into.</param>
    /// <param name="styleProperties">String representation of the style properties
    /// (key - property name, value - property value).</param>
    protected override void ParseStyle( TextBoxShapeBase textBox, Dictionary<string, string> styleProperties )
    {
      CommentShapeImpl comment = textBox as CommentShapeImpl;
      ParseVisibility( comment, styleProperties );
    }
    /// <summary>
    /// Parses visibility options.
    /// </summary>
    /// <param name="comment">Comment to set visibility for.</param>
    /// <param name="dictProperties">Dictionary with comment properties.</param>
    private void ParseVisibility( CommentShapeImpl comment, Dictionary<string, string> dictProperties )
    {
      if( comment == null )
        throw new ArgumentNullException( "comment" );

      if( dictProperties == null )
        throw new ArgumentNullException( "dictProperties" );

      string strValue;

      bool bVisible = true;

      if( dictProperties.TryGetValue( Vml.VisibilityAttribute, out strValue ) )
      {
        bVisible = ( strValue != Vml.VisibilityHiddenValue );
      }

      comment.IsVisible = bVisible;
    }
    /// <summary>
    /// Registers shape in all necessary collections.
    /// </summary>
    /// <param name="textBox">Shape to register.</param>
    protected override void RegisterShape( TextBoxShapeBase textBox )
    {
      base.RegisterShape( textBox );

      WorksheetImpl sheet = ( WorksheetImpl )textBox.Worksheet;
      sheet.InnerComments.AddComment( textBox as ICommentShape );
    }
    #endregion
  }
}
