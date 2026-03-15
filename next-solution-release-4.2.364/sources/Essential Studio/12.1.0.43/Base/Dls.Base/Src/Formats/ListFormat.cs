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

using Syncfusion.DLS;
using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents List formatting.
  /// </summary>
  public class ListFormat : FormatBase
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const int ListLevelNumberKey = 0;
    /// <summary>
    /// 
    /// </summary>
    private const int ListTypeKey = 1;
    /// <summary>
    /// 
    /// </summary>
    private const int CustomStyleNameKey = 2;
    /// <summary>
    /// 
    /// </summary>
    private const int RestartKey = 3;
    
    #endregion
    
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private IParagraph m_ownerPara;
    /// <summary>
    /// Currently used list style
    /// </summary>
    [ ThreadStatic ]
    private static string m_currentStyleName;
    /// <summary>
    /// Current level number
    /// </summary>
    [ThreadStatic]
    private static int m_currLevelNumber;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets list nesting level. 
    /// </summary>
    public int ListLevelNumber
    {
      get
      {
        return ( int )this[ ListLevelNumberKey ];
      }
      set
      {
        if( value > 8 || value < 0 )
        {
          throw new ArgumentException( "List level must be less 8 and greater then 0" );
        }
        else
        {
          this[ ListLevelNumberKey ] = value;
          m_currLevelNumber = value;
        }
      }
    }
    /// <summary>
    /// Get / sets type of the list.
    /// </summary>
    public ListType ListType
    {
      get
      {
        return ( ListType )this[ ListTypeKey ];
      }
    }
    /// <summary>
    /// Gets / sets whether numbering of the list must restart from previous list.
    /// </summary>
    public bool RestartNumbering
    {
      get
      {
        return ( bool )this[ RestartKey ];
      }
      set
      {
        this[ RestartKey ] = value;
      }
    }
    /// <summary>
    /// Gets / sets name of custom style.
    /// </summary>
    public string CustomStyleName
    {
      get
      {
        return ( string )this[ CustomStyleNameKey ];
      }
    }

    /// <summary>
    /// Get paragraph's list style.
    /// </summary>
    public ListStyle CurrentListStyle
    {
      get
      {
        if( ( string )this[ CustomStyleNameKey ] != string.Empty )
        {
          return m_ownerPara.Document.ListStyles.FindByName( CustomStyleName );
        }
        return null;
      }      
    }
    /// <summary>
    /// Get set paragraph's ListLevel.
    /// </summary>
    public ListLevel CurrentListLevel
    {
      get
      {
        if( ( string )this[ CustomStyleNameKey ] == string.Empty )
          return null;
        return CurrentListStyle.Levels[ ListLevelNumber ];
      }
    }

    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor.
    /// </summary>
    public ListFormat( IParagraph owner )
    {
      m_ownerPara = owner;
      DocumentEx = ( Document )owner.Document;
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
        case ListLevelNumberKey:
          return (int)0;
        case ListTypeKey:
          return ListType.NoList;
        case RestartKey:
          return false;
        case CustomStyleNameKey:
          return string.Empty;
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

      if( HasKey( ListLevelNumberKey ))
      {
        writer.WriteValue( XDLSConstants.ListFormatLevelNumAttr, ListLevelNumber );
      }
      if( HasKey( CustomStyleNameKey ))
      {
        writer.WriteValue( XDLSConstants.ListFormatStyleNameAttr, CustomStyleName );
      }   
      if( HasKey( ListTypeKey ))
      {
        writer.WriteValue( XDLSConstants.ListFormatTypeAttr, ListType );
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

      if( reader.HasAttribute( XDLSConstants.ListFormatLevelNumAttr ) )
      {
        this[ ListLevelNumberKey ] = reader.ReadInt( XDLSConstants.ListFormatLevelNumAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ListFormatStyleNameAttr ))
      {
        this[ CustomStyleNameKey ] = reader.ReadString(XDLSConstants.ListFormatStyleNameAttr);         
      }
      if( reader.HasAttribute( XDLSConstants.ListFormatTypeAttr ))
      {
        this[ ListTypeKey ] = ( ListType )reader.ReadEnum( XDLSConstants.ListFormatTypeAttr, typeof( ListType ));
      }      
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Increase level indent.
    /// </summary>
    public void IncreaseLevelIndent()
    {
      if( m_currLevelNumber == 8 )
        throw new ArgumentException( "List level must be less 8 and greater then 0" );
      this[ ListLevelNumberKey ] = ++m_currLevelNumber;
    }

    /// <summary>
    /// Decrease level indent.
    /// </summary>
    public void DecreaseLevelIndent()
    {
      if( m_currLevelNumber == 0 )
        throw new ArgumentException( "List level must be less 8 and greater then 0" );
      this[ ListLevelNumberKey ] = --m_currLevelNumber;
    }

    /// <summary>
    /// Continue last list.
    /// </summary>
    public void ContinueListNumbering()
    {
      ApplyStyle( m_currentStyleName );
      ListLevelNumber = m_currLevelNumber;
    }

    /// <summary>
    /// Apply liststyle 
    /// </summary>
    /// <param name="styleName">Style Name</param>
    public void ApplyStyle( string styleName )
    {
      this[ CustomStyleNameKey ] = styleName;
      m_currentStyleName = styleName;
      this[ ListTypeKey ] = CurrentListStyle.ListType;
    }

    /// <summary>
    /// Apply default bullet style for current paragraph.
    /// </summary>
    public void ApplyDefBulletStyle()
    {
      ApplyStyle( "Bulleted" );
    }

    /// <summary>
    /// Apply default numbered style for current paragraph.
    /// </summary>
    public void ApplyDefNumberedStyle()
    {
      ApplyStyle( "Numbered" );
    }
    #endregion
    
  }
}
