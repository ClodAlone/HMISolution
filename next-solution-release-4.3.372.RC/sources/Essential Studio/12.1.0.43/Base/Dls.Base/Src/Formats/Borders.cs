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
using System;
using System.Collections;
using System.Drawing;

using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents border formatting.
  /// </summary>
  public class Border : FormatBase
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    public const int ColorKey = 1;
    /// <summary>
    /// 
    /// </summary>
    protected const int BorderTypeKey = 2;
    /// <summary>
    /// 
    /// </summary>
    protected const int LineWidthKey = 3;
    /// <summary>
    /// 
    /// </summary>
    protected const int SpaceKey = 4;
    /// <summary>
    /// 
    /// </summary>
    protected const int ShadowKey = 5;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets/sets color of the border.
    /// </summary>
    public Color Color
    {
      get
      {
        return ( Color )this[ ColorKey ];
      }
      set
      {
        this[ ColorKey ] = value;
      }
    }
    /// <summary>
    /// Gets/sets width of the border.
    /// </summary>
    public float LineWidth
    {
      get
      {
        return ( float )this[ LineWidthKey ];
      }
      set
      {
        this[ LineWidthKey ] = value;
        
        if( value == 0f )
        {
          if( BorderType != BorderStyle.None ) 
            BorderType = BorderStyle.None;
        }
        else
        {
          if( BorderType == BorderStyle.None ) 
            BorderType = BorderStyle.Single;
        }
      }
    }
    /// <summary>
    /// Gets/sets  style of the border.
    /// </summary>
    public BorderStyle BorderType
    {
      get
      {
        return ( BorderStyle )this[ BorderTypeKey ];
      }
      set
      {
        
        if( value == BorderStyle.None )
        {
          if( LineWidth != 0f )
          {
            LineWidth = 0f;
            Color = Color.Empty;
          }
        }
        else if( ( BorderStyle )this[ BorderTypeKey ] == BorderStyle.None )
        {
          if( LineWidth == 0f )
          {
            LineWidth = 1f;
          }
          
          if( Color == Color.Empty )
          {
            Color = Color.Black;
          }
        }
        
        this[ BorderTypeKey ] = value;
      }
    }
    /// <summary>
    /// Gets / Sets width of space to maintain between border and text within border.
    /// </summary>
    public float Space
    {
      get
      {
        return ( float )this[ SpaceKey ];
      }
      set
      {
        this[ SpaceKey ] = value;
      }
    }
    /// <summary>
    /// Setting to define if border should be drawn with shadow.
    /// </summary>
    public bool Shadow
    {
      get
      {
        return ( bool )this[ ShadowKey ];
      }
      set
      {
        this[ ShadowKey ] = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="baseKey"></param>
    public Border( FormatBase parent, int baseKey )
      : base( parent, baseKey )
    {
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceBorder"></param>
    internal void CopyBorderFormatting( Border sourceBorder )
    {
      this[ BorderTypeKey ] = sourceBorder.BorderType;
      this[ LineWidthKey ] = sourceBorder.LineWidth;
      this[ ColorKey ] = sourceBorder.Color;
      this[ ShadowKey ] = sourceBorder.Shadow;
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <param name="lineWidth"></param>
    /// <param name="borderType"></param>
    /// <param name="shadow"></param>
    public void InitFormatting( Color color, float lineWidth, BorderStyle borderType, bool shadow )
    {
      this[ ColorKey ] = color;
      this[ LineWidthKey ] = lineWidth;
      this[ BorderTypeKey ] = borderType;
      this[ ShadowKey ]= shadow;
    }
    #endregion


    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override object GetDefValue( int key )
    {
      switch( key )
      {
        case ColorKey:
          return Color.Empty;
        case BorderTypeKey:
          return BorderStyle.None;
        case LineWidthKey:
          return 0f;
        case ShadowKey:
          return false;
        case SpaceKey:
          return (float)0;
      }

      throw new ArgumentException( "key has invalid value" );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );

      if( HasKey( ColorKey ) )
      {
        writer.WriteValue( XDLSConstants.BorderColorAttr, Color );
      }
      if( HasKey( LineWidthKey ))
      {
        writer.WriteValue( XDLSConstants.BorderWidthAttr, LineWidth );
      }
      if( HasKey( BorderTypeKey ))
      {
        writer.WriteValue( XDLSConstants.BorderTypeAttr, BorderType );
      }
      if( HasKey( SpaceKey ))
      {
        writer.WriteValue( XDLSConstants.BorderSpaceAttr, Space );
      }
      if( HasKey( ShadowKey ))
      {
        writer.WriteValue( XDLSConstants.BorderShadowAttr, Shadow );
      }
      
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
    {
      base.ReadXmlAttributes (reader);
      
      if( reader.HasAttribute( XDLSConstants.BorderColorAttr ) )
      {
        Color = reader.ReadColor( XDLSConstants.BorderColorAttr );
      }
      if( reader.HasAttribute( XDLSConstants.BorderWidthAttr ))
      {
        LineWidth = reader.ReadFloat( XDLSConstants.BorderWidthAttr );
      }
      if( reader.HasAttribute( XDLSConstants.BorderTypeAttr ))
      {
        BorderType = ( BorderStyle )reader.ReadEnum( XDLSConstants.BorderTypeAttr, typeof( BorderStyle ) );
      }
      if( reader.HasAttribute( XDLSConstants.BorderSpaceAttr ))
      {
        Space = reader.ReadFloat( XDLSConstants.BorderSpaceAttr );
      }
      if( reader.HasAttribute(XDLSConstants.BorderShadowAttr ))
      {
        Shadow  = reader.ReadBoolean( XDLSConstants.BorderShadowAttr );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      if( IsDefault)
      {
        XDLSHolder.SkipMe = true;
      }
    }

    #endregion
  }

  /// <summary>
  /// Represents a collection of four borders. <see cref="Syncfusion.DLS.Border"/>
  /// </summary>
  public class Borders : FormatBase
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    public const int LeftKey = 1;
    /// <summary>
    /// 
    /// </summary>
    public const int TopKey = 2;
    /// <summary>
    /// 
    /// </summary>
    public const int BottomKey = 3;
    /// <summary>
    /// 
    /// </summary>
    public const int RightKey = 4;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets whether the border exists
    /// </summary>
    public bool NoBorder
    {
      get
      {
        return ( Left.BorderType == BorderStyle.None
                 && Right.BorderType == BorderStyle.None
                 && Top.BorderType == BorderStyle.None
                 && Bottom.BorderType == BorderStyle.None );
      }
    }
    /// <summary>
    /// Gets left border.
    /// </summary>
    public Border Left
    {
      get
      {
        return this[ LeftKey ] as Border;
      }
    }
    /// <summary>
    /// Gets top border.
    /// </summary>
    public Border Top
    {
      get
      {
        return this[ TopKey ] as Border;
      }
    }
    /// <summary>
    /// Gets right border.
    /// </summary>
    public Border Right
    {
      get
      {
        return this[ RightKey ] as Border;
      }
    }
    /// <summary>
    /// Gets bottom border.
    /// </summary>
    public Border Bottom
    {
      get
      {
        return this[ BottomKey ] as Border;
      }
    }
    /// <summary>
    /// Sets color of the borders.
    /// </summary>
    public Color Color
    {
      set
      {
        Left.Color = Right.Color = Top.Color = Bottom.Color = value;
      }
    }
    /// <summary>
    /// Sets width of the borders.
    /// </summary>
    public float LineWidth
    {
      set
      {
        Left.LineWidth = Right.LineWidth = Top.LineWidth = Bottom.LineWidth = value;
      }
    }
    /// <summary>
    /// Sets style of the borders.
    /// </summary>
    public BorderStyle BorderType
    {
      set
      {
        Left.BorderType = Right.BorderType = Top.BorderType = Bottom.BorderType = value;
      }
    }
    /// <summary>
    /// Sets width of space to maintain between borders and text within borders.
    /// </summary>
    public float Space
    {
      set
      {
        Left.Space = Right.Space = Top.Space = Bottom.Space = value;
      }
    }
    /// <summary>
    /// Sets whether borders are drawn with shadow.
    /// </summary>
    public bool Shadow
    {
      set
      {
        Left.Shadow = Right.Shadow = Top.Shadow = Bottom.Shadow = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="baseKey"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    internal Borders( FormatBase parent, int baseKey )
      : base( parent, baseKey )
    {
    }
    /// <summary>
    /// Default constructor.
    /// </summary>
    public Borders( )
      : base()
    {
    }
    /// <summary>
    /// Default constructor.
    /// </summary>
    internal Borders( Borders borders )
      : base()
    {
      this.ImportContainer( borders );
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected internal override void EnsureComposites()
    {
      EnsureComposites( LeftKey, RightKey, TopKey, BottomKey );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override object GetDefValue( int key )
    {
      throw new ArgumentException( "key has invalid value" );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override FormatBase GetDefComposite( int key )
    {
      switch( key )
      {
        case LeftKey:
          return GetDefComposite( LeftKey, new Border( this, LeftKey ) );
        case TopKey:
          return GetDefComposite( TopKey, new Border( this, TopKey ) );
        case RightKey:
          return GetDefComposite( RightKey, new Border( this, RightKey ) );
        case BottomKey:
          return GetDefComposite( BottomKey, new Border( this, BottomKey ) );
      }

      return null;
    }
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      if( IsDefault)
      {
        XDLSHolder.SkipMe = true;
      }
      XDLSHolder.AddElement( XDLSConstants.BorderBottomTag, Bottom );
      XDLSHolder.AddElement( XDLSConstants.BorderTopTag, Top );
      XDLSHolder.AddElement( XDLSConstants.BorderLeftTag, Left );
      XDLSHolder.AddElement( XDLSConstants.BorderRightTag, Right );
    }
    /// <summary>
    /// Clones self.
    /// </summary>
    /// <returns></returns>
    public Borders Clone()
    {
      return CloneImpl();
    }
    /// <summary>
    /// Clone method implementation.
    /// </summary>
    /// <returns></returns>
    protected virtual Borders CloneImpl()
    {
      return new Borders( this );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    protected override void ImportMembers(FormatBase format)
    {
      Borders borders = format as Borders;

      if( borders != null )
      {
        this.Left.CopyBorderFormatting( borders.Left );
        this.Right.CopyBorderFormatting( borders.Right );
        this.Top.CopyBorderFormatting( borders.Top );
        this.Bottom.CopyBorderFormatting( borders.Bottom );
      }
    }

    #endregion
  }
}