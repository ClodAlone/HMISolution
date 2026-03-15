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
  /// Represents cell padding.
  /// </summary>
  public class Paddings : FormatBase
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    public const int LeftKey = 1;
    public const int TopKey = 2;
    public const int BottomKey = 3;
    public const int RightKey = 4;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets/ sets left padding.
    /// </summary>
    public float Left
    {
      get
      {
        return ( float )this[ LeftKey ];
      }
      set
      {
        this[ LeftKey ] = value;
      }
    }
    /// <summary>
    /// Gets/ sets top padding.
    /// </summary>
    public float Top
    {
      get
      {
        return ( float )this[ TopKey ];
      }
      set
      {
        this[ TopKey ] = value;
      }
    }
    /// <summary>
    /// Gets/ sets right padding.
    /// </summary>
    public float Right
    {
      get
      {
        return ( float )this[ RightKey ];
      }
      set
      {
        this[ RightKey ] = value;
      }
    }
    /// <summary>
    /// Gets/ sets bottom padding.
    /// </summary>
    public float Bottom
    {
      get
      {
        return ( float )this[ BottomKey ];
      }
      set
      {
        this[ BottomKey ] = value;
      }
    }
    /// <summary>
    /// Sets all paddings.
    /// </summary>
    public float All
    {
      set
      {
        Left = Right = Top = Bottom = value;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="baseKey"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    internal Paddings( FormatBase parent, int baseKey )
      : base( parent, baseKey )
    {
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
        case LeftKey:
          return 0f;
        case RightKey:
          return 0f;
        case TopKey:
          return 0f;
        case BottomKey:
          return 0f;
      }
      throw new ArgumentException( "key has invalid value" );
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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected override void WriteXmlAttributes( IXDLSAttributeWriter writer )
    {
      base.WriteXmlAttributes( writer );

      if( HasKey( LeftKey ))
      {
        writer.WriteValue( XDLSConstants.PaddingLeftTag, Left );
      }
      if( HasKey( RightKey ))
      {
        writer.WriteValue( XDLSConstants.PaddingRightTag, Right );
      }
      if( HasKey( BottomKey ) )
      {
        writer.WriteValue( XDLSConstants.PaddingBottomTag, Bottom );
      }
      if( HasKey( TopKey ))
      {
        writer.WriteValue( XDLSConstants.PaddingTopTag, Top );
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
      
      if( reader.HasAttribute( XDLSConstants.PaddingLeftTag ) )
      {
        Left = reader.ReadFloat( XDLSConstants.PaddingLeftTag );
      }
      if( reader.HasAttribute( XDLSConstants.PaddingRightTag ) )
      {
        Right = reader.ReadFloat( XDLSConstants.PaddingRightTag );
      }
      if( reader.HasAttribute( XDLSConstants.PaddingBottomTag ) )
      {
        Bottom = reader.ReadFloat( XDLSConstants.PaddingBottomTag );
      }
      if( reader.HasAttribute( XDLSConstants.PaddingTopTag ) )
      {
        Top = reader.ReadFloat( XDLSConstants.PaddingTopTag );
      }
    }

    #endregion
  }
}