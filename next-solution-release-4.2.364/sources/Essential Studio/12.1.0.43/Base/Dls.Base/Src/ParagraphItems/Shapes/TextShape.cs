#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System.Drawing;
using System;
using System.Globalization;

using Syncfusion.DLS.XML;
using Syncfusion.Layouting;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents text shape.
  /// </summary>
  /// <remarks>Supported by Essential PDF only.</remarks> 
  public class TextShape 
    : Shape,
      IWidget
  {
    #region Class members
    /// <summary>
    /// Rectangle of the text.
    /// </summary>
    private RectangleF m_bounds = RectangleF.Empty;
    /// <summary>
    /// 
    /// </summary>
    private CharacterFormat m_chProps = null;
    /// <summary>
    /// 
    /// </summary>
    private string m_strText = string.Empty;
    #endregion    

    #region Class properties
    /// <summary>
    ///  Gets / sets location of the text.
    /// </summary>
    public PointF Location
    {
      get
      {
        return m_bounds.Location;
      }
      set
      {
        if( m_bounds.Location != value )
        {
          m_bounds.Location = value;
        }
      }
    }
    /// <summary>
    /// Gets / sets x coordinate of the text.
    /// </summary>
    public float X
    {
      get
      {
        return m_bounds.Location.X;
      }
      set
      {
        if( m_bounds.Location.X != value )
        {
          PointF location = m_bounds.Location;
          location.X = value;
          m_bounds.Location = location;
        }
      }
    }
    /// <summary>
    /// Gets / sets y coordinate of the text.
    /// </summary>
    public float Y
    {
      get
      {
        return m_bounds.Location.Y;
      }
      set
      {
        if( m_bounds.Location.Y != value )
        {
          PointF location = m_bounds.Location;
          location.Y = value;
          m_bounds.Location = location;
        }
      }
    }
    /// <summary>
    /// Gets / sets size of the bounds.
    /// </summary>
    public SizeF Size
    {
      get
      {
        return m_bounds.Size;
      }
      set
      {
        if( m_bounds.Size != value )
        {
          m_bounds.Size = value;
        }
      }
    }
    /// <summary>
    /// Gets / sets bounds of the text.
    /// </summary>
    public RectangleF Bounds
    {
      get
      {
        return m_bounds;
      }
      set
      {
        if( m_bounds != value )
        {
          m_bounds = value;
        }
      }
    }
    /// <summary>
    /// Gets / sets the text.
    /// </summary>
    public string Text
    {
      get
      {
        return m_strText;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "Text" );

        if( m_strText != value )
        {
          m_strText = value;
        }
      }
    }
    /// <summary>
    /// Gets / Sets the CharacterFormat of the Text
    /// </summary>
    public CharacterFormat CharacterFormat
    {
      get
      {
        return m_chProps;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public TextShape( Canvas canvas )
      : base( canvas )
    {
      Document docEx = this.Document as Document;

      if( docEx != null )
      {
        m_chProps = docEx.CreateCharacterFormatImpl();
      }
    }
    #endregion

    #region XML serialization overrides
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      XDLSHolder.AddElement( PropertyNames.Format, m_chProps );
      base.InitXDLSHolder();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      base.WriteXmlAttributes( writer );
      
      writer.WriteValue( PropertyNames.Type, ShapeType.Text );

      writer.WriteValue( PropertyNames.X, this.X );
      writer.WriteValue( PropertyNames.Y, this.Y );
      writer.WriteValue( PropertyNames.Width, this.Size.Width );
      writer.WriteValue( PropertyNames.Height, this.Size.Height );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      base.ReadXmlAttributes( reader );
      
      this.X = reader.ReadFloat( PropertyNames.X );
      this.Y = reader.ReadFloat( PropertyNames.Y );
      float width = reader.ReadFloat( PropertyNames.Width );
      float height = reader.ReadFloat( PropertyNames.Height );
      this.Size = new SizeF( width, height );
    }
    /// <summary>
    /// Writes text to XML.
    /// </summary>
    /// <param name="writer">Writer object.</param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlContent( IXDLSContentWriter writer )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      base.WriteXmlContent( writer );
      
      writer.WriteChildStringElement( PropertyNames.Text, this.Text );
    }
    /// <summary>
    /// Reads data from XML.
    /// </summary>
    /// <param name="reader">Reader object.</param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override bool ReadXmlContent( IXDLSContentReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      bool result = base.ReadXmlContent( reader );
      
      if( string.Compare( reader.TagName, PropertyNames.Text,
        true, CultureInfo.InvariantCulture ) == 0 )
      {
        this.Text = reader.ReadChildStringContent();
        
        result = true;
      }

      return result;
    }
#if DEBUG_LAYOUTING    
    /// <summary>
    /// 
    /// </summary>
    protected override void DBG_WXC()
    {
      base.DBG_WXC();
    }
#endif
    #endregion

    #region WidgetBase override
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cg"></param>
    /// <param name="ltWidget"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    void IWidget.Draw( CustomGraphics cg, LayoutedWidget ltWidget )
    {
      ApplyTransform( cg );
      ( cg as DLSGraphics ).DrawTextShape( this, ltWidget );
      ResetTransform( cg );
    }
    #endregion
  }
}