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
using System.Text;

using Syncfusion.Layouting;
using Syncfusion.DLS.XML;
#endregion

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents ListLevel.
  /// </summary>
  public class ListLevel : XDLSSerializableBase
  {
    #region Class constants
    /// <summary>
    /// Limit number of converting arabic to \"A\" format.
    /// </summary>
    private const float DEF_AR_TO_LETTER_LIMIT = 26.0f;
    /// <summary>
    /// Index of A char in the ASCII table.
    /// </summary>
    private const int DEF_A_ASCII_INDEX = ( int )( 'A' - 1 );
    /// <summary>
    /// 
    /// </summary>
    private readonly string[] DEF_NUMBER_WORDS = new string[]
      {
        "one", // 1
        "two",
        "three",
        "four",
        "five",
        "six",
        "seven",
        "eight",
        "nine",
        "ten", // 10
        "eleven", 
        "twelve", 
        "thirteen", 
        "fourteen", 
        "fifteen", 
        "sixteen", 
        "seventeen", 
        "eighteen", 
        "nineteen"  // 19
      };
    /// <summary>
    /// 
    /// </summary>
    private readonly string[] DEF_TENS_WORDS = new string[]
      {
        "ten", // 1
        "twenty",
        "thirty",
        "forty",
        "fifty",
        "sixty",
        "seventy",
        "eighty",
        "ninety", // 19
    };
    #endregion

    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private CharacterFormat m_chFormat;
    /// <summary>
    /// 
    /// </summary>
    private ParagraphFormat m_prFormat;
    /// <summary>
    /// 
    /// </summary>
    private ListStyle m_listStyle;
    /// <summary>
    /// 
    /// </summary>
    private string m_numberPrefix;
    /// <summary>
    /// 
    /// </summary>
    private string m_numberSufix; 
    /// <summary>
    /// 
    /// </summary>
    private string m_layoutNumSuf = ".";
    /// <summary>
    /// 
    /// </summary>
    private string m_layoutNumPref = string.Empty;
   
    /// <summary>
    /// 
    /// </summary>
    private string m_bulletChar;
    /// <summary>
    /// 
    /// </summary>
    private bool m_noRestart;
    /// <summary>
    /// 
    /// </summary>
    private int m_startAt = 1;
    /// <summary>
    /// 
    /// </summary>
    private ListNumberAlignment m_alignment;
    /// <summary>
    /// 
    /// </summary>
    private ListPatternType m_patternType = ListPatternType.Arabic;
    /// <summary>
    /// 
    /// </summary>
    private bool m_isLegal;
    /// <summary>
    /// 
    /// </summary>
    private FollowCharacterType m_followChar;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bUsePrevLevelPattern;
    /// <summary>
    /// 
    /// </summary>
    private byte[] m_charOffset = new byte[ 9 ];
    #endregion
    
    #region Class properties
    /// <summary>
    /// Get/set alignment (left, right, or centered) of the paragraph number. 
    /// </summary>
    public ListNumberAlignment NumberAlignment
    {
      get
      {
        return m_alignment;
      }
      set
      {
        m_alignment = value;
      }
    }
    /// <summary>
    /// Get/set start at value.
    /// </summary>
    public int StartAt
    {
      get
      {
        return m_startAt;
      }
      set
      {
        m_startAt = value;
      }
    }
    /// <summary>
    /// Get/set spacing after list level's number or bullet
    /// ( tab position if follow character is tab ).
    /// </summary>
    public float TabSpaceAfter
    {
      get
      {
        if( m_prFormat.Tabs.Count > 0 )
          return m_prFormat.Tabs[ 0 ].Position;
        return 0;//        Document.LastSection.PageSetup.DefaultTabWidth;
      }
      set
      {
        m_prFormat.Tabs.AddTab( value ); ;
      }
    }   
    /// <summary>
    /// Gets / sets left listlevel indent
    /// </summary>
    public float TextPosition
    {
      get
      {        
        return m_prFormat.LeftIndent;
      }
      set
      {
        m_prFormat.LeftIndent =  value;
      }
    }
    /// <summary>
    /// Gets / set prefix pattern for numbered level.
    /// </summary>
    public string NumberPrefix
    {
      get
      {
        return m_numberPrefix;
      }
      set
      {
        m_numberPrefix = value;
      }
    }
    /// <summary>
    /// Gets / sets sufix pattern for numbered level.
    /// </summary>
    public string NumberSufix
    {
      get
      {
        return m_numberSufix;
      }
      set
      {
        m_numberSufix = value;
      }
    }
    /// <summary>
    /// Get/set bullet pattern
    /// </summary>
    public string BulletCharacter
    {
      get
      {
        return m_bulletChar;
      }
      set
      {
        m_bulletChar = value;
      }
    }
    /// <summary>
    /// Getd / sets list numbering type.
    /// </summary>
    public ListPatternType PatternType
    {
      get
      {
        return m_patternType;
      }
      set
      {
        m_patternType = value;
      }
    }
    /// <summary>
    /// True if the level's number sequence is not restarted by higher
    /// (more significant) levels in the list.
    /// </summary>
    public bool NoRestartByHigher
    {
      get
      {
        return m_noRestart;
      }
      set
      {
        m_noRestart = value;
      }
    }
    /// <summary>
    /// Gets / sets character formats of list symbol.
    /// </summary>
    public CharacterFormat CharacterFormat
    {
      get
      {
        return m_chFormat;
      }
      set
      {
        m_chFormat = value;
      }
    }
    /// <summary>
    /// Gets / sets paragraph format of list level.
    /// </summary>
    public ParagraphFormat ParagraphFormat
    {
      get
      {
        return m_prFormat;
      }
      set
      {
        m_prFormat = value;
      }
    }
    /// <summary>
    /// Gets previous list.
    /// </summary>
    protected ListLevel PreviousLevel
    {
      get
      {
        int index = m_listStyle.Levels.IndexOf( this );
        
        if( index > 0 )
        {
          return m_listStyle.Levels[ index - 1 ];
        }
        
        return null;
      }
    }
    /// <summary>
    /// Get/set the type of character following the number text for the paragraph.
    /// </summary>
    public FollowCharacterType FollowCharacter
    {
      get
      {
        return m_followChar;
      }
      set
      {
        m_followChar = value;
      }
    }
    /// <summary>
    /// Get/set ArabaicNumberFormat property
    /// ( true if the level turns all inherited numbers to arabic,
    ///  false if it preserves their number format code ). 
    /// </summary>
    public bool IsLegalStyleNumbering
    {
      get
      {
        return m_isLegal;
      }
      set
      {
        m_isLegal = value;
      }
    }  
    /// <summary>
    /// Get/set number/bullet position for current listlevel. 
    /// </summary>
    public float NumberPosition
    {
      get
      {
        return m_prFormat.FirstLineIndent;
      }
      set
      {
        m_prFormat.FirstLineIndent = value;
      }
    }
    /// <summary>
    /// When true, number generated will include previous
    /// levels (used for legal numbering).
    /// </summary>
    public bool UsePrevLevelPattern
    {
      get
      {
        return m_bUsePrevLevelPattern;
      }
      set
      {
        m_bUsePrevLevelPattern = value;
      }
    }
    #endregion
    
    #region Class initialize/finalize methods
    /// <summary>
    /// Initializing constructor
    /// </summary>
    /// <param name="listStyle"></param>
    public ListLevel( ListStyle listStyle )
      : base( listStyle.Document )
    {
      m_chFormat = DocumentEx.CreateCharacterFormatImpl();
      m_prFormat = DocumentEx.CreateParagraphFormatImpl();
      m_listStyle = listStyle;
    }
    /// <summary>
    /// 
    /// </summary>
    protected internal ListLevel( Document doc ) : base ( doc )
    {
      m_chFormat = DocumentEx.CreateCharacterFormatImpl();
      m_prFormat = DocumentEx.CreateParagraphFormatImpl();
    }
    #endregion
    
    #region Class public methods
    /// <summary>
    /// Create level layout data
    /// </summary>
    /// <param name="numStr"></param>
    /// <param name="characterOffsets"></param>
    /// <param name="levelNumber"></param>
    public void CreateLayoutData ( string numStr, byte[] characterOffsets, int levelNumber )
    {
      int start = 0;
      int length = 0;
      char[] splitter = new char[ 2 ] { '\\', Convert.ToChar( levelNumber) };
      string[] patternArray = numStr.Split( splitter );
      int numOffset = patternArray[ 0 ].Length + 1;
      for( int i = 0; i < 9; i++ )
      {
        if( ( int )characterOffsets[ i ] == numOffset )
        {
          //Get prefix
          if( i == 0 ) m_layoutNumPref = numStr.Substring( 0 , numOffset - 1 );
          else
          {
            start = ( int )characterOffsets[ i - 1 ];
            length = ( numOffset - 1 ) - ( int )characterOffsets[ i-1 ]; 
            m_layoutNumPref = numStr.Substring( start, length );            
          }

          //Get sufix
          if(( i == 8 ) || ( ( int )characterOffsets[ i + 1 ] == 0 ))
          {
            m_layoutNumSuf = patternArray[ 1 ];
          }
          else
          {
            length = ( int )characterOffsets[ i + 1 ] - ( numOffset + 1 );
            start = numOffset + 1;
            m_layoutNumSuf = numStr.Substring( start, length );           
          }
          break;
        }
      }
    }
    /// <summary>
    /// Gets list symbol for specified item index
    /// </summary>
    /// <param name="listItemIndex"></param>
    /// <returns></returns>
    public string GetListItemText( int listItemIndex )
    {
      string listItemText = string.Empty;
      
      switch( m_listStyle.ListType )
      {
        case ListType.Numbered:
          listItemText = GetNumberedItemText( listItemIndex );
          break;
        case ListType.Bulleted:  
          listItemText = m_bulletChar;
          break;          
        case ListType.NoList:
        default:
          listItemText = "";
          break;
      }
      
      return listItemText;
    }
    /// <summary>
    /// Clone current listlevel.
    /// </summary>
    /// <returns>ListLevelItem</returns>
    public virtual ListLevel Clone( ListStyle owner )
    {
      ListLevel cloneLevel = new ListLevel( owner );
      cloneLevel.TextPosition = TextPosition;
      cloneLevel.NumberAlignment = NumberAlignment;
      cloneLevel.ParagraphFormat = ParagraphFormat;
      cloneLevel.CharacterFormat = CharacterFormat;
      cloneLevel.NumberPrefix = NumberPrefix;
      cloneLevel.NumberSufix = NumberSufix;
      cloneLevel.BulletCharacter = BulletCharacter;
      cloneLevel.PatternType = PatternType;
      cloneLevel.StartAt = StartAt;
      cloneLevel.UsePrevLevelPattern = UsePrevLevelPattern;
      cloneLevel.FollowCharacter = FollowCharacter;
      cloneLevel.IsLegalStyleNumbering = IsLegalStyleNumbering;
      cloneLevel.NoRestartByHigher = NoRestartByHigher;

      return cloneLevel;
    }
    /// <summary>
    /// Create default bullet level.
    /// </summary>
    /// <param name="dxLeft"></param>
    /// <param name="str"></param>
    /// <param name="listStyle"></param>
    /// <returns></returns>
    internal static ListLevel CreateDefBulletLvl( float dxLeft, string str, ListStyle listStyle )
    {
      ListLevel lvl = listStyle.DocumentEx.CreateListLevelImpl( listStyle );
      lvl.m_startAt = 1;
      lvl.m_patternType = ListPatternType.Bullet;
      //lvl.m_bUsePrevLevelPattern = true;      

      // we switch font depending on bullet style
      string fontName = "Times New Roman";
      switch (str)
      {
        case ListStyle.DEF_BULLLET_FIRST:
          fontName = "Symbol";
          break;
          
        case ListStyle.DEF_BULLLET_SECOND:
          fontName = "Courier New";
          break;
          
        case ListStyle.DEF_BULLLET_THIRD:
          fontName = "Wingdings";
          break;
      }

      lvl.m_chFormat.FontName = fontName;
      lvl.m_prFormat.LeftIndent = dxLeft; 
      lvl.m_bulletChar = str;

      return lvl;
    }
    /// <summary>
    /// Create default numbered level.
    /// </summary>
    internal static ListLevel CreateDefNumberLvl( int dxLeft, int levelNumber,
      ListPatternType patType, ListNumberAlignment align, ListStyle listStyle )
    {
      ListLevel lvl = listStyle.DocumentEx.CreateListLevelImpl( listStyle );
      lvl.m_startAt = 1;
      lvl.m_patternType = patType;
      lvl.m_alignment = align;
      lvl.NumberPrefix = string.Empty;
      lvl.NumberSufix = ".";
      //lvl.m_bUsePrevLevelPattern = true;
      
      lvl.m_prFormat.LeftIndent = dxLeft;
      lvl.m_chFormat.FontName = "Times New Roman";
      return lvl;
    }
    #endregion
    
    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="listItemIndex"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    private string GetNumberedItemText( int listItemIndex )
    {
      switch( m_patternType )
      {
        case ListPatternType.UpRoman:
          return m_layoutNumPref + GetAsRoman( listItemIndex + 1 ).ToUpper() + m_layoutNumSuf;
        case ListPatternType.LowRoman:
          return m_layoutNumPref + GetAsRoman( listItemIndex + 1 ).ToLower() + m_layoutNumSuf;
        case ListPatternType.UpLetter:
          return m_layoutNumPref + GetAsLetter( listItemIndex + 1 ).ToUpper() + m_layoutNumSuf;
        case ListPatternType.LowLetter:
          return m_layoutNumPref + GetAsLetter( listItemIndex + 1 ).ToLower() + m_layoutNumSuf;
        case ListPatternType.Ordinal:
          return m_layoutNumPref + GetAsWord( listItemIndex + 1, true ) + m_layoutNumSuf;
        default:
          return m_layoutNumPref + ( listItemIndex + 1 ).ToString() + m_layoutNumSuf;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    private string GetAsRoman( int number )
    {
      StringBuilder retval = new StringBuilder();
      
      retval.Append( GenerateNumber( ref number, 1000, "M" ) );
      retval.Append( GenerateNumber( ref number, 900,  "CM" ) );
      retval.Append( GenerateNumber( ref number, 500,  "D" ) );
      retval.Append( GenerateNumber( ref number, 400,  "CD" ) );
      retval.Append( GenerateNumber( ref number, 100,  "C" ) );
      retval.Append( GenerateNumber( ref number, 90,   "XC" ) );
      retval.Append( GenerateNumber( ref number, 50,   "L" ) );
      retval.Append( GenerateNumber( ref number, 40,   "XL" ) );
      retval.Append( GenerateNumber( ref number, 10,   "X" ) );
      retval.Append( GenerateNumber( ref number, 9,    "IX" ) );
      retval.Append( GenerateNumber( ref number, 5,    "V" ) );
      retval.Append( GenerateNumber( ref number, 4,    "IV" ) );
      retval.Append( GenerateNumber( ref number, 1,    "I" ) );

      return retval.ToString();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    private string GetAsLetter( int number )
    {
      Stack stack = ConvertToLetter( number );
      StringBuilder result = new StringBuilder();

      while( stack.Count > 0 )
      {
        int num = ( int )stack.Pop();
        AppendChar( result, num );
      }

      return result.ToString();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="number"></param>
    /// <param name="isOrdinal"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    private string GetAsWord( int number, bool isOrdinal )
    {
      string retValue = "";
      if( isOrdinal )
      {
        throw new NotImplementedException( "style list not implemented now" );
      }
      else
      {
      
        if( number > 99 )
          throw new ArgumentOutOfRangeException( "Cannot support number greater than 99" );
        
        if( number < 20 )
        {
          retValue = DEF_NUMBER_WORDS[ number ];
        }
        else
        {
          int tens = (int)Math.Floor( (double) number / 10 );
          retValue = DEF_TENS_WORDS[ tens ] + "-" + DEF_NUMBER_WORDS[ number - tens*10 ];
        }
      }
      
      return retValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="magnitude"></param>
    /// <param name="letter"></param>
    /// <returns></returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    private string GenerateNumber( ref int value, int magnitude, string letter )
    {
      StringBuilder numberstring = new StringBuilder();
      
      while( value >= magnitude )
      {
        value -= magnitude;
        numberstring.Append( letter );
      }
      
      return numberstring.ToString();
    }
    /// <summary>
    /// Utility metnod. Helps to convert arabic number to \"A\" format.
    /// </summary>
    /// <param name="arabic">Arabic number.</param>
    /// <returns>Sequence of number.</returns>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    private static Stack ConvertToLetter( float arabic )
    {
      if( arabic <= 0 )
        throw new ArgumentOutOfRangeException( "arabic", arabic, "Value can not be less 0" );
      
      Stack stack = new Stack();

        
      while( ( ( int )arabic ) > DEF_AR_TO_LETTER_LIMIT )
      {
        float remainder = arabic % DEF_AR_TO_LETTER_LIMIT;

        if( remainder == 0.0f )
        {
          arabic = arabic / DEF_AR_TO_LETTER_LIMIT - 1f;
          remainder = DEF_AR_TO_LETTER_LIMIT;
        }
        else
        {
          arabic /= DEF_AR_TO_LETTER_LIMIT;
        }

        stack.Push( ( int )remainder );
      }

      if( arabic > 0f )
      {
        stack.Push( ( int )arabic );
      }
    
      return stack;
    }
    /// <summary>
    /// Adds letter instead of number.
    /// </summary>
    /// <param name="builder">String builder object.</param>
    /// <param name="number">Number to be converted to letter.</param>
    [ Syncfusion.Documentation.DocumentationExclude() ]
    private static void AppendChar( StringBuilder builder, int number )
    {
      if( builder == null )
        throw new ArgumentNullException( "builder" );
      if( number <= 0 || number > 26 )
        throw new ArgumentOutOfRangeException( "number", number, "Value can not be less 0 and greater 26" );

      char letter = ( char ) ( DEF_A_ASCII_INDEX + number );
      builder.Append( letter );
    }
    #endregion

    #region Class overrides 
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    protected override void ReadXmlAttributes(Syncfusion.DLS.XML.IXDLSAttributeReader reader)
    {
      base.ReadXmlAttributes( reader );

      if( reader.HasAttribute( XDLSConstants.ListLevelIndentAttr ))
      {
        TextPosition = reader.ReadFloat( XDLSConstants.ListLevelIndentAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ListLevelPrefPatternAttr ))
      {
        NumberPrefix = reader.ReadString( XDLSConstants.ListLevelPrefPatternAttr );
      }
      else
      {
        NumberPrefix = null;
      }
      if( reader.HasAttribute( XDLSConstants.ListLevelSufPatternAttr ))
      {
        NumberSufix = reader.ReadString( XDLSConstants.ListLevelSufPatternAttr );
      }
      else
      {
        NumberSufix = null;
      }
      if( reader.HasAttribute( XDLSConstants.ListLevelBulletPatternAttr ))
      {
        BulletCharacter = reader.ReadString( XDLSConstants.ListLevelBulletPatternAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ListLevelPatternTypeAttr ))
      {
        PatternType = ( ListPatternType )reader.ReadEnum( XDLSConstants.ListLevelPatternTypeAttr, typeof( ListPatternType ) );
      }
      if( reader.HasAttribute( XDLSConstants.ListLevelPrevPatternAttr ))
      {
        UsePrevLevelPattern = reader.ReadBoolean( XDLSConstants.ListLevelPrevPatternAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ListLevelStartAtAttr ))
      {
        StartAt = reader.ReadInt( XDLSConstants.ListLevelStartAtAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ListLevelNumberAlignAttr ))
      {
        NumberAlignment = ( ListNumberAlignment )reader.ReadEnum( XDLSConstants.ListLevelNumberAlignAttr,
          typeof( ListNumberAlignment ));
      }
      if( reader.HasAttribute( XDLSConstants.ListLevelFollowCharacterAttr ))
      {
        FollowCharacter = ( FollowCharacterType )reader.ReadEnum( XDLSConstants.ListLevelFollowCharacterAttr, typeof( FollowCharacterType ));
      }
      if( reader.HasAttribute( XDLSConstants.ListLevelIsLegalAttr ))
      {
        IsLegalStyleNumbering = reader.ReadBoolean( XDLSConstants.ListLevelIsLegalAttr );
      }
      if( reader.HasAttribute( XDLSConstants.ListLevelNoRestartNum ))
      {
        NoRestartByHigher = reader.ReadBoolean( XDLSConstants.ListLevelNoRestartNum );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    protected override void WriteXmlAttributes(Syncfusion.DLS.XML.IXDLSAttributeWriter writer)
    {  
      base.WriteXmlAttributes( writer );

      writer.WriteValue( XDLSConstants.ListLevelIndentAttr, TextPosition );
      writer.WriteValue( XDLSConstants.ListLevelPrefPatternAttr, NumberPrefix );
      writer.WriteValue( XDLSConstants.ListLevelSufPatternAttr, NumberSufix );
      writer.WriteValue( XDLSConstants.ListLevelBulletPatternAttr, BulletCharacter );
      writer.WriteValue( XDLSConstants.ListLevelPatternTypeAttr, PatternType );
      writer.WriteValue( XDLSConstants.ListLevelPrevPatternAttr, UsePrevLevelPattern );
      writer.WriteValue( XDLSConstants.ListLevelStartAtAttr, StartAt );
      if( m_listStyle != null && m_listStyle.ListType == ListType.Numbered )
      {
        writer.WriteValue( XDLSConstants.ListLevelNumberAlignAttr, NumberAlignment );
      }
      writer.WriteValue( XDLSConstants.ListLevelIsLegalAttr, IsLegalStyleNumbering );
      writer.WriteValue( XDLSConstants.ListLevelFollowCharacterAttr, FollowCharacter );
      writer.WriteValue( XDLSConstants.ListLevelNoRestartNum, NoRestartByHigher ); 
      
    }
    /// <summary>
    /// Serialize paragraph and character properties.
    /// </summary>
    protected override void InitXDLSHolder()
    {
      base.InitXDLSHolder ();
      XDLSHolder.AddElement( XDLSConstants.ParagraphFormatTag, m_prFormat );
      XDLSHolder.AddElement( XDLSConstants.CharacterFormatTag, m_chFormat );
    }
    #endregion
  }
}