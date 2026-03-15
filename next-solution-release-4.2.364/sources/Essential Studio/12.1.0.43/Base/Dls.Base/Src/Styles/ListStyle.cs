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

using Syncfusion.DLS.Collections;
using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents a ListStyle.
  /// </summary>
  public class ListStyle : Style
  {
    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_MULTIPLIER = 72;
    /// <summary>
    /// List bullets type
    /// </summary>
    internal const string DEF_BULLLET_FIRST = "\uf0b7";
    internal const string DEF_BULLLET_SECOND = "o";
    internal const string DEF_BULLLET_THIRD = "\uf0a7";
    
    #endregion

    #region Class member
    /// <summary>
    /// 
    /// </summary>
    private ListLevelCollection m_levels;
    /// <summary>
    /// 
    /// </summary>
    private ListType m_listType;
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets list type
    /// </summary>
    public ListType ListType
    {
      get
      {
        return m_listType;
      }
      set
      {
        m_listType = value;
      }
    }
    /// <summary>
    /// Gets list levels collection
    /// </summary>
    public ListLevelCollection Levels
    {
      get
      {
        return m_levels;
      }
    }
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="listType"></param>
    public ListStyle( IDocument doc, ListType listType )
      : base( doc )
    {
      m_listType = listType;
      m_levels = new ListLevelCollection( this );
      this.CreateDefListLevels( listType );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="style"></param>
    /// <param name="doc"></param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    internal ListStyle( ListStyle style, IDocument doc )
      : this( doc, style.m_listType )
    {
      m_listType = style.m_listType;
      m_levels = style.Levels.Clone( this );
      Name = style.Name;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    internal ListStyle( IDocument doc )
      : base( doc )
    {
      m_levels = new ListLevelCollection( this );
    }
    /// <summary>
    /// Initializing constructor
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="listType"></param>
    /// <param name="isOneLevelList"></param>
    internal ListStyle( IDocument doc, ListType listType, bool isOneLevelList )
      : base( doc )
    {
      m_listType = listType;
      m_levels = new ListLevelCollection( this );
      this.CreateEmptyListLevels( listType, isOneLevelList );    
    }

    #endregion

    #region Class methods
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="listType">List type(bulleted or numbered)</param>
    /// <param name="isOneLevelList"> Is it list that consist of 1 level only.</param>
    /// <returns></returns>
    public static ListStyle CreateEmptyListStyle( IDocument doc, ListType listType, bool isOneLevelList )
    {
      ListStyle emptyStyle = new ListStyle( doc, listType, isOneLevelList ); 
      return emptyStyle;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Clones current style object
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    public override IStyle Clone( IDocument document )
    {
      return CloneImpl( document );
    }   
    /// <summary>
    /// 
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    protected virtual ListStyle CloneImpl( IDocument document )
    {
      return new ListStyle( this, document );
    }
    /// <summary>
    /// 
    /// </summary>
    protected override void InitXDLSHolder()
    {
      base.InitXDLSHolder ();
      XDLSHolder.AddElement( XDLSConstants.ListLevelsTag, Levels);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
    {
      base.WriteXmlAttributes (writer);

      writer.WriteValue( XDLSConstants.ListFormatTypeAttr, ListType );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
    {
      base.ReadXmlAttributes (reader);
      ListType = ( ListType )reader.ReadEnum( XDLSConstants.ListFormatTypeAttr, typeof( Syncfusion.DLS.ListType ));
    }


    #endregion
    
    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    internal void CreateDefListLevels( ListType listType )
    {
      Levels.Clear();
      ListLevel tempLevel = DocumentEx.CreateListLevelImpl( this );// new ListLevel( this );
      if( listType == ListType.Bulleted )
      {
        for( float i = 0.5f; i < 4.5f; i += 1.5f )
        {
          Levels.Add( ListLevel.CreateDefBulletLvl( ( int )( DEF_MULTIPLIER * i ), DEF_BULLLET_FIRST, this ) );
          Levels.Add( ListLevel.CreateDefBulletLvl( ( int )( DEF_MULTIPLIER * ( i + 0.5 ) ), DEF_BULLLET_SECOND, this ) );
          Levels.Add( ListLevel.CreateDefBulletLvl( ( int )( DEF_MULTIPLIER * ( i + 1 ) ), DEF_BULLLET_THIRD, this ) );
        }
      }
      else
      {
        int level = 0;
        for( float i = 0.5f; i < 4.5f; i += 1.5f )
        {
          Levels.Add(
            ListLevel.CreateDefNumberLvl( ( int )( DEF_MULTIPLIER * i ),
            level++, 
            ListPatternType.Arabic,
            ListNumberAlignment.Left,
            this ));
          Levels.Add(
            ListLevel.CreateDefNumberLvl( ( int )( DEF_MULTIPLIER * ( i + 0.5 ) ),
            level++,
            ListPatternType.LowLetter,
            ListNumberAlignment.Right,
            this ));
          Levels.Add(
            ListLevel.CreateDefNumberLvl( ( int )( DEF_MULTIPLIER * ( i + 1 ) ),
            level++,
            ListPatternType.LowRoman,
            ListNumberAlignment.Left,
            this ));
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="levelNumber"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    public ListLevel GetNearLevel( int levelNumber )
    {
      if( levelNumber < 0 )
        throw new ArgumentOutOfRangeException( "number", levelNumber, "Value can not be less than 0" );

      if( levelNumber > Levels.Count - 1 )
      {
        levelNumber = Levels.Count - 1;
      }
      
      return Levels[ levelNumber ];
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="listType"></param>
    /// <param name="isOneLevelList"></param>
    internal void CreateEmptyListLevels( ListType listType, bool isOneLevelList )
    {
      int cnt = ( isOneLevelList ) ? 1 : 9;
      for( int i = 0; i < cnt; i ++ )
      {
        Levels.Add( DocumentEx.CreateListLevelImpl( this ));
          // new ListLevel( this ));
      }
    }
    #endregion
  }
}