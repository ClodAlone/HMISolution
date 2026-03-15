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
using System.Drawing;

using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents character (text) format.
  /// </summary>
  public class CharacterFormat : FormatBase
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    public const int FontKey = 0;
    public const int TextColorKey = 1;
    public const int FontNameKey = 2;
    public const int FontSizeKey = 3;
    public const int BoldKey = 4;
    public const int ItalicKey = 5;
    public const int StrikeKey = 6;
    public const int UnderlineKey = 7;
    public const int TextBkgColorKey = 9;
    public const int SubSuperScriptKey = 10;
    public const int DoubleStrikeKey = 14;
    public const int PositionKey = 17;
    public const int SpacingKey = 18;
    public const int LineBreakKey = 20;
    /// <summary>
    /// 
    /// </summary>
    protected const string DEF_FONTFAMILY = "Times New Roman";
    protected const float DEF_FONTSIZE = 10f;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets font as System.Drawing.Font
    /// </summary>
    public Font Font
    {
      get
      {
        FontStyle style = FontStyle.Regular;
        if( Bold )
        {
          style |= FontStyle.Bold;
        }
        if( Italic )
        {
          style |= FontStyle.Italic;
        }
        if( UnderlineStyle != UnderlineStyle.None )
        {
          style |= FontStyle.Underline;
        }
        if( Strikeout )
        {
          style |= FontStyle.Strikeout;
        }

        // NOTE: Here we can cache Font object at least during layouting.
        return new Font( FontName, FontSize, style );
      }
      set
      {
        FontName = value.FontFamily.Name;
        FontSize = value.SizeInPoints;
        Bold = value.Bold;
        Italic = value.Italic;
        Strikeout = value.Strikeout;
        UnderlineStyle = value.Underline ? UnderlineStyle.Single : UnderlineStyle.None;
      }
    }
    /// <summary>
    /// Gets / sets font name
    /// </summary>
    public string FontName
    {
      get
      {
        return ( string )this[ FontNameKey ];
      }
      set
      {
        int fullkey = GetFullKey( FontNameKey );

        //if( !PropertiesHash.Contains( fullkey ) )
        {
          PropertiesHash[ fullkey ] = value;
          if( IsDefault ) IsDefault = false;
        }

        //this[ FontNameKey ] = value;
        OnUpdateFontName( value );
      }
    }
    /// <summary>
    /// Gets / sets font size
    /// </summary>
    public float FontSize
    {
      get
      {
        return ( float )this[ FontSizeKey ];
      }
      set
      {
        this[ FontSizeKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets bold style
    /// </summary>
    public bool Bold
    {
      get
      {
        return ( bool )this[ BoldKey ];
      }
      set
      {
        this[ BoldKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets italic style
    /// </summary>
    public bool Italic
    {
      get
      {
        return ( bool )this[ ItalicKey ];
      }
      set
      {
        this[ ItalicKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets strikeout style
    /// </summary>
    public bool Strikeout
    {
      get
      {
        return ( bool )this[ StrikeKey ];
      }
      set
      {
        this[ StrikeKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets doublestrikeout style
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public bool DoubleStrike
    {
      get
      {
        return ( bool )this[ DoubleStrikeKey ];
      }
      set
      {
        this[ DoubleStrikeKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets underline style
    /// </summary>
    /// <remarks>Essential PDF only supports single underline.</remarks>
    public UnderlineStyle UnderlineStyle
    {
      get
      {
        return ( UnderlineStyle )this[ UnderlineKey ];
      }
      set
      {
        this[ UnderlineKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets text color
    /// </summary>
    public Color TextColor
    {
      get
      {
        return ( Color )this[ TextColorKey ];
      }
      set
      {
        this[ TextColorKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets text background color
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public Color TextBackgroundColor
    {
      get
      {
        return ( Color )this[ TextBkgColorKey ];
      }
      set
      {
        this[ TextBkgColorKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets subscript/superscript mode
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public SubSuperScript SubSuperScript
    {
      get
      {
        return ( SubSuperScript )this[ SubSuperScriptKey ];
      }
      set
      {
        this[ SubSuperScriptKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets space width between characters.
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public float CharacterSpacing
    {
      get
      {
        return ( float )this[ SpacingKey ];
      }
      set
      {
        this[ SpacingKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets text vertical position.
    /// </summary>
    /// <remarks>Not supported by Essential PDF</remarks>
    public float Position
    {
      get
      {
        return ( float )this[ PositionKey ];
      }
      set
      {
        this[ PositionKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets line break after.
    /// </summary>
    public bool LineBreak
    {
      get
      {
        return ( bool )this[ LineBreakKey ];
      }
      set
      {
        this[ LineBreakKey ] = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public CharacterFormat()
    {}
    /// <summary>
    /// Default constructor
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public CharacterFormat( IDocument doc )
    : base( doc )
    {}
    #endregion
    
    #region Class overrides
    /// <summary>
    /// Called when updates font name.
    /// </summary>
    /// <param name="fontName">Name of the font.</param>
    protected virtual void OnUpdateFontName( string fontName )
    {
    }
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
        case FontKey:
          return new Font( DEF_FONTFAMILY, DEF_FONTSIZE );
        case TextColorKey:
          return Color.Black;
        case FontNameKey:
          return DEF_FONTFAMILY;
        case FontSizeKey:
          return DEF_FONTSIZE;
        case BoldKey:
        case ItalicKey:
        case StrikeKey:
          return false;
        case UnderlineKey:
          return UnderlineStyle.None;
        case SubSuperScriptKey:
          return SubSuperScript.None;
        case TextBkgColorKey:
          return Color.White;
        case PositionKey:
          return 0f;
        case SpacingKey:
          return 0f;
        case DoubleStrikeKey:
          return false;
        case LineBreakKey:
          return false;
      }

      throw new ArgumentException( "key has invalid value" );
    }
    /// <summary>
    /// 
    /// </summary>    
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void InitXDLSHolder()
    {
      if( PropertiesHash.Count == 0 )
      {
        XDLSHolder.SkipMe = true;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );

      if( HasKey( TextColorKey ) )
      {
        writer.WriteValue( XDLSConstants.TextColorAttr, TextColor );
      }
      if( HasKey( FontNameKey ) )
      {
        writer.WriteValue( XDLSConstants.TextFontNameAttr, FontName );
      }
      if( HasKey( FontSizeKey ) )
      {
        writer.WriteValue( XDLSConstants.TextFontSizeAttr, FontSize );
      }
      if( HasKey( BoldKey ) )
      {
        writer.WriteValue( XDLSConstants.TextBoldAttr, Bold );
      }
      if( HasKey( ItalicKey ) )
      {
        writer.WriteValue( XDLSConstants.TextItalicAttr, Italic );
      }
      if( HasKey( StrikeKey ) )
      {
        writer.WriteValue( XDLSConstants.TextStrikeAttr, Strikeout );
      }
      if( HasKey( DoubleStrikeKey ))
      {
        writer.WriteValue( XDLSConstants.TextDoubleStrikeAttr, DoubleStrike );
      }
      if( HasKey( UnderlineKey ) )
      {
        writer.WriteValue( XDLSConstants.TextUnderlineAttr, UnderlineStyle );
      }
      if( HasKey( SubSuperScriptKey ))
      {
        writer.WriteValue( XDLSConstants.TextSubSuperScriptAttr, SubSuperScript );
      }
      if( HasKey( SpacingKey ))
      {
        writer.WriteValue( XDLSConstants.TextLineSpacingAttr, CharacterSpacing );
      }
      if( HasKey( PositionKey ))
      {
        writer.WriteValue( XDLSConstants.TextPositionAttr, Position );
      }
      if( HasKey( TextBkgColorKey ))
      {
        writer.WriteValue( XDLSConstants.TextBackgroundColorAttr, TextBackgroundColor );
      }
      if( HasKey( LineBreakKey ) )
      {
        writer.WriteValue( XDLSConstants.TextLineBreakAttr, LineBreak );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void ReadXmlAttributes( IXDLSAttributeReader reader )
    {
      base.ReadXmlAttributes( reader );
      
      if( reader.HasAttribute( XDLSConstants.TextFontNameAttr ) )
      {
        FontName = reader.ReadString( XDLSConstants.TextFontNameAttr );
      }
      if( reader.HasAttribute( XDLSConstants.TextUnderlineAttr ) )
      {
        UnderlineStyle = ( UnderlineStyle )reader.ReadEnum( XDLSConstants.TextUnderlineAttr, typeof( UnderlineStyle ) );
      }
      if( reader.HasAttribute( XDLSConstants.TextColorAttr ) )
      {
        TextColor = reader.ReadColor( XDLSConstants.TextColorAttr );
      }
      if( reader.HasAttribute( XDLSConstants.TextFontSizeAttr ) )
      {
        FontSize = reader.ReadFloat( XDLSConstants.TextFontSizeAttr );
      }
      if( reader.HasAttribute( XDLSConstants.TextBoldAttr ) )
      {
        Bold = reader.ReadBoolean( XDLSConstants.TextBoldAttr );
      }
      if( reader.HasAttribute( XDLSConstants.TextItalicAttr ) )
      {
        Italic = reader.ReadBoolean( XDLSConstants.TextItalicAttr );
      }
      if( reader.HasAttribute( XDLSConstants.TextStrikeAttr ) )
      {
        Strikeout = reader.ReadBoolean( XDLSConstants.TextStrikeAttr );
      }
      if( reader.HasAttribute( XDLSConstants.TextDoubleStrikeAttr ) )
      {
        DoubleStrike = reader.ReadBoolean( XDLSConstants.TextDoubleStrikeAttr );
      }
      if( reader.HasAttribute( XDLSConstants.TextLineSpacingAttr ))
      {
        CharacterSpacing = reader.ReadFloat( XDLSConstants.TextLineSpacingAttr );
      }
      if( reader.HasAttribute( XDLSConstants.TextPositionAttr ))
      {
        Position = reader.ReadFloat( XDLSConstants.TextPositionAttr );
      }
      if( reader.HasAttribute( XDLSConstants.TextSubSuperScriptAttr ) )
      {
        SubSuperScript = ( SubSuperScript )reader.ReadEnum( XDLSConstants.TextSubSuperScriptAttr, typeof( SubSuperScript ) );
      }
      if( reader.HasAttribute( XDLSConstants.TextBackgroundColorAttr ))
      {
        TextBackgroundColor = reader.ReadColor( XDLSConstants.TextBackgroundColorAttr );
      }
      if( reader.HasAttribute( XDLSConstants.TextLineBreakAttr ))
      {
        LineBreak = reader.ReadBoolean( XDLSConstants.TextLineBreakAttr );
      }
    }
    #endregion
  }
}
