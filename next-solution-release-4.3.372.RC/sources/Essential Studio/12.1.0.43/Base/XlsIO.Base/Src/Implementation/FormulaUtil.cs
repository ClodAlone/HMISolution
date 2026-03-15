#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using StorageType =

#if !SILVERLIGHT && !WINRT && !WP
System.Collections.SortedList;
#else
 Syncfusion.XlsIO.Implementation.TypedSortedListEx<string, object>;
#endif
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif


#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif !(WINRT )
using System.Drawing;
#endif


namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This class provides functionality needed for formula parsing.
  /// </summary>
  //[ CLSCompliant( false ) ]
  public class FormulaUtil : CommonObject
  {
    #region Internal classes
    /// <summary>
    /// Constructor id.
    /// </summary>
    internal enum ConstructorId
    {
      Default,
      String,
      ByteArrayOffset,
      StringParent,
      TwoUShorts,
      FunctionIndex,
      TwoStrings,
      FourStrings,
      Int3String4Bool,
      TokenType,
    };

    /// <summary>
    /// This class is used in the construction of formula tokens.
    /// </summary>
    internal class TokenConstructor
    {
      #region Class members
      /// <summary>
      /// Dictionary key (int) - constructor id, value - ConstructorInfo.
      /// </summary>
      private Dictionary<int, ConstructorInfo> m_hashConstructorToId = new Dictionary<int, ConstructorInfo>();
      /// <summary>
      /// Token class for which this instance was created.
      /// </summary>
      private Type m_type;
      #endregion
    
      #region Class constructors
      /// <summary>
      /// Token class for which this instance was created.
      /// </summary>
      private TokenConstructor()
      {
      }
      /// <summary>
      /// Creates class instance for specified type.
      /// </summary>
      /// <param name="type">
      /// Type for which an object will be created. Must be inherited from Ptg class.
      /// </param>
      /// <exception cref="System.ArgumentException">
      /// When specified type is not inherited from Ptg class.
      /// </exception>
      /// <exception cref="System.ArgumentNullException">
      /// When specified type is null.
      /// </exception>
      public TokenConstructor( Type type )
      {
        if( type == null )
          throw new ArgumentNullException( "type", "Token type can't be null" );
         
#if ( WINRT )
          if( !type.GetTypeInfo().IsSubclassOf( typeof( Ptg ) ) )
#else
        if( !type.IsSubclassOf( typeof( Ptg ) ) )
#endif
          throw new ArgumentException( "class should be descendant of Ptg", "type" );

        m_type = type;
#if ( WINRT )
        DefaultConstructor = type.GetConstructor( new Type[]{} );
#else
          DefaultConstructor = type.GetConstructor( new Type[]{} );
#endif
        StringConstructor = type.GetConstructor(new Type[] { typeof(string) });
        ArrayConstructor = type.GetConstructor( new Type[]{ typeof( DataProvider ), 
                                                              typeof( int ) } );
        StringParentConstructor = type.GetConstructor( 
          new Type[]{ typeof( string ), typeof( IWorkbook ) } );

        TwoUShortsConstructor = type.GetConstructor( new Type[]{ typeof( ushort ), typeof( ushort ) } );
        FunctionIndexConstructor = type.GetConstructor( new Type[]{ typeof( ExcelFunction ) } );

        Type[] arrTypes = new Type[]{ typeof( int ),    typeof( int ),
                                      typeof( string ), typeof( string ), typeof( bool ) };
        TwoStringsConstructor = type.GetConstructor( arrTypes );

        arrTypes = new Type[]{ typeof( int ),    typeof( int ),
                               typeof( string ), typeof( string ),
                               typeof( string ), typeof( string ), typeof( bool ), typeof( IWorkbook ) };

        FourStringsConstructor = type.GetConstructor( arrTypes );

        arrTypes = new Type[]{ typeof( int ), typeof( int ), typeof( int ),
                               typeof( string ), typeof( string ), typeof( string ),
                               typeof( string ), typeof( bool ), typeof ( IWorkbook ) };

        Int3String4BoolConstructor = type.GetConstructor( arrTypes );

        arrTypes = new Type[]{ typeof( FormulaToken ) };
        TokenTypeConstructor = type.GetConstructor( arrTypes );

      }
      #endregion
    
      #region Create methods
      /// <summary>
      /// Creates token using default constructor.
      /// </summary>
      /// <returns>Newly created token.</returns>
      public Ptg CreatePtg()
      {
        try
        {
          return ( Ptg )DefaultConstructor.Invoke( null );
        }
        catch( TargetInvocationException ex )
        {
          throw ex.InnerException;
        }
      }
      /// <summary>
      /// Creates token using string parameter.
      /// </summary>
      /// <param name="tokenType">Token type to pass to constructor.</param>
      /// <returns>Newly created token.</returns>
      public Ptg CreatePtg( FormulaToken tokenType )
      {
        try
        {
          return ( Ptg )TokenTypeConstructor.Invoke( new object[]{ tokenType } );
        }
        catch( TargetInvocationException ex )
        {
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, m_type, "Exception creating Ptg token" );
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message + Environment.NewLine + ex.StackTrace );
          throw ex.InnerException;
        }
      }
      /// <summary>
      /// Creates token using string parameter.
      /// </summary>
      /// <param name="strParam">
      /// String parameter that will be passed to constructor.
      /// </param>
      /// <returns>Newly created token.</returns>
      public Ptg CreatePtg( string strParam )
      {
        try
        {
          return ( Ptg )StringConstructor.Invoke( new object[]{ strParam } );
        }
        catch( TargetInvocationException ex )
        {
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, m_type, "Exception creating Ptg token" );
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message + Environment.NewLine + ex.StackTrace );
          throw ex.InnerException;
        }
      }
      /// <summary>
      /// Creates token using array of bytes and offset
      /// where token data begins.
      /// </summary>
      /// <param name="provider">Object that provides access to the data.</param>
      /// <param name="offset">Offset to the token data.</param>
      /// <param name="arguments">Arguments required by some parse methods.</param>
      /// <returns>Newly created token</returns>
      public Ptg CreatePtg( DataProvider provider, ref int offset, ParseParameters arguments )
      {
        try
        {
          Ptg result = ( Ptg )ArrayConstructor.Invoke( new object[]{ provider, offset } );
          offset += result.GetSize( arguments.Version );
          return result;
        }
        catch( TargetInvocationException ex )
        {
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, m_type, "Exception creating Ptg token" );
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message + Environment.NewLine + ex.StackTrace );
          throw ex.InnerException;
        }
      }
      /// <summary>
      /// This method looks for constructor that takes specified 
      /// parameters and invokes it.
      /// </summary>
      /// <param name="arrParams">
      /// Parameters that will be passed to the constructor.
      /// </param>
      /// <returns>Newly created token.</returns>
      public Ptg CreatePtg( params object[] arrParams )
      {
        Type[] types = new Type[ arrParams.Length ];
        
        for( int i = 0; i < arrParams.Length; i++ )
        {
          types[ i ] = arrParams[ i ].GetType();
        }

        ConstructorInfo constructor = m_type.GetConstructor( types );
        try
        {
          return ( Ptg )constructor.Invoke( arrParams );
        }
        catch( TargetInvocationException ex )
        {
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, m_type, "Exception creating Ptg token" );
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message + Environment.NewLine + ex.StackTrace );
          throw ex.InnerException;
        }
      }
      /// <summary>
      /// Creates token using string and parent workbook.
      /// </summary>
      /// <param name="strParam">String representation of the token.</param>
      /// <param name="parent">Parent workbook.</param>
      /// <returns>Created formula token.</returns>
      public Ptg CreatePtg( string strParam, IWorkbook parent )
      {
        try
        {
          return ( Ptg )StringParentConstructor.Invoke( new object[]{ strParam, parent } );
        }
        catch( TargetInvocationException ex )
        {
          throw ex.InnerException;
        }
      }
      /// <summary>
      /// Creates token using two integer values.
      /// </summary>
      /// <param name="iParam1">First value.</param>
      /// <param name="iParam2">Second value.</param>
      /// <returns>Created formula token.</returns>
      public Ptg CreatePtg( ushort iParam1, ushort iParam2 )
      {
        try
        {
          return ( Ptg )TwoUShortsConstructor.Invoke( new object[]{ iParam1, iParam2 } );
        }
        catch( TargetInvocationException ex )
        {
          throw ex.InnerException;
        }
      }
      /// <summary>
      /// Creates token using function index.
      /// </summary>
      /// <param name="functionIndex">Function index.</param>
      /// <returns>Created formula token.</returns>
      public Ptg CreatePtg( ExcelFunction functionIndex )
      {
        try
        {
          return ( Ptg )FunctionIndexConstructor.Invoke( new object[]{ functionIndex } );
        }
        catch( TargetInvocationException ex )
        {
          throw ex.InnerException;
        }
      }
      /// <summary>
      /// Creates token using two string values.
      /// </summary>
      /// <param name="iCellRow">Row index of the cell that contains new token.</param>
      /// <param name="iCellColumn">Column index of the cell that contains new token.</param>
      /// <param name="strParam1">First value.</param>
      /// <param name="strParam2">Second value.</param>
      /// <param name="bR1C1">Indicates whether R1C1 notation is used.</param>
      /// <returns>Created formula token.</returns>
      public Ptg CreatePtg( int iCellRow, int iCellColumn, string strParam1, string strParam2, bool bR1C1 )
      {
        try
        {
          return ( Ptg )TwoStringsConstructor.Invoke( 
            new object[]{ iCellRow, iCellColumn, strParam1, strParam2, bR1C1 } );
        }
        catch( TargetInvocationException ex )
        {
          throw ex.InnerException;
        }
      }
      /// <summary>
      /// Creates token using two string values.
      /// </summary>
      /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
      /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
      /// <param name="strParam1">First value.</param>
      /// <param name="strParam2">Second value.</param>
      /// <param name="strParam3">Third value.</param>
      /// <param name="strParam4">Fourth value.</param>
      /// <param name="bR1C1">Indicates whether R1C1 notation is used.</param>
      /// <param name="book">Parent workbook.</param>
      /// <returns>Created formula token.</returns>
      public Ptg CreatePtg( int iCellRow, int iCellColumn, string strParam1,
        string strParam2, string strParam3, string strParam4, bool bR1C1, IWorkbook book )
      {
        try
        {
          object[] arrArguments = new object[]{ iCellRow, iCellColumn, strParam1,
                                                strParam2, strParam3, strParam4, bR1C1, book };
          return ( Ptg )FourStringsConstructor.Invoke( arrArguments );
        }
        catch( TargetInvocationException ex )
        {
          throw ex.InnerException;
        }
      }
      /// <summary>
      /// Creates token using two string values.
      /// </summary>
      /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
      /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
      /// <param name="iRefIndex">Worksheet reference index.</param>
      /// <param name="strParam1">First value.</param>
      /// <param name="strParam2">Second value.</param>
      /// <param name="strParam3">Third value.</param>
      /// <param name="strParam4">Fourth value.</param>
      /// <param name="bR1C1">Indicates whether R1C1 notation is used.</param>
      /// <param name="book">Parent workbook.</param>
      /// <returns>Created formula token.</returns>
      public Ptg CreatePtg( int iCellRow, int iCellColumn, int iRefIndex, string strParam1,
        string strParam2, string strParam3, string strParam4, bool bR1C1, IWorkbook book )
      {
        try
        {
          object[] arrArguments = new object[]{ iCellRow, iCellColumn, iRefIndex, strParam1,
                                                strParam2, strParam3, strParam4, bR1C1, book };
          return ( Ptg )Int3String4BoolConstructor.Invoke( arrArguments );
        }
        catch( TargetInvocationException ex )
        {
          throw ex.InnerException;
        }
      }
      #endregion

      #region Class properties
      /// <summary>
      /// Gets / sets default constructor.
      /// </summary>
      private ConstructorInfo DefaultConstructor
      {
        get
        {
          return GetConstructor( ConstructorId.Default );
        }
        set
        {
          SetConstructor( ConstructorId.Default, value );
        }
      }
      /// <summary>
      /// Gets / sets string constructor.
      /// </summary>
      private ConstructorInfo StringConstructor
      {
        get
        {
          return GetConstructor( ConstructorId.String );
        }
        set
        {
          SetConstructor( ConstructorId.String, value );
        }
      }
      /// <summary>
      /// Gets / sets ByteArrayOffset constructor.
      /// </summary>
      private ConstructorInfo ArrayConstructor
      {
        get
        {
          return GetConstructor( ConstructorId.ByteArrayOffset );
        }
        set
        {
          SetConstructor( ConstructorId.ByteArrayOffset, value );
        }
      }
      /// <summary>
      /// Gets / sets StringParent constructor.
      /// </summary>
      private ConstructorInfo StringParentConstructor
      {
        get
        {
          return GetConstructor( ConstructorId.StringParent );
        }
        set
        {
          SetConstructor( ConstructorId.StringParent, value );
        }
      }
      /// <summary>
      /// Gets / sets TwoInts constructor.
      /// </summary>
      private ConstructorInfo TwoUShortsConstructor
      {
        get
        {
          return GetConstructor( ConstructorId.TwoUShorts );
        }
        set
        {
          SetConstructor( ConstructorId.TwoUShorts, value );
        }
      }
      /// <summary>
      /// Gets / sets FunctionIndex constructor.
      /// </summary>
      private ConstructorInfo FunctionIndexConstructor
      {
        get
        {
          return GetConstructor( ConstructorId.FunctionIndex );
        }
        set
        {
          SetConstructor( ConstructorId.FunctionIndex, value );
        }
      }
      /// <summary>
      /// Gets / sets constructor that accepts two strings as arguments.
      /// </summary>
      private ConstructorInfo TwoStringsConstructor
      {
        get
        {
          return GetConstructor( ConstructorId.TwoStrings );
        }
        set
        {
          SetConstructor( ConstructorId.TwoStrings, value );
        }
      }
      /// <summary>
      /// Gets / sets constructor that accepts four strings as arguments.
      /// </summary>
      private ConstructorInfo FourStringsConstructor
      {
        get
        {
          return GetConstructor( ConstructorId.FourStrings );
        }
        set
        {
          SetConstructor( ConstructorId.FourStrings, value );
        }
      }
      /// <summary>
      /// Gets / sets constructor that accepts three ints, four strings
      /// and a bool as arguments.
      /// </summary>
      private ConstructorInfo Int3String4BoolConstructor
      {
        get
        {
          return GetConstructor( ConstructorId.Int3String4Bool );
        }
        set
        {
          SetConstructor( ConstructorId.Int3String4Bool, value );
        }
      }
      /// <summary>
      /// Gets / sets constructor that accepts three ints, four strings
      /// and a bool as arguments.
      /// </summary>
      private ConstructorInfo TokenTypeConstructor
      {
        get
        {
          return GetConstructor( ConstructorId.TokenType );
        }
        set
        {
          SetConstructor( ConstructorId.TokenType, value );
        }
      }
      #endregion

      #region Class helper methods
      /// <summary>
      /// Gets constructor.
      /// </summary>
      /// <param name="id">Constructor id.</param>
      /// <returns>Constructor with specified id.</returns>
      private ConstructorInfo GetConstructor( ConstructorId id )
      {
        ConstructorInfo result;
        m_hashConstructorToId.TryGetValue( ( int )id, out result );
        return result;
      }
      /// <summary>
      /// Sets constructor.
      /// </summary>
      /// <param name="id">Constructor id.</param>
      /// <param name="value">Corresponding ConstructorInfo.</param>
      private void SetConstructor( ConstructorId id, ConstructorInfo value )
      {
        int iId = ( int )id;

        if( value != null )
        {
          m_hashConstructorToId[ iId ] = value;
        }
        else
        {
          m_hashConstructorToId.Remove( iId );
        }
      }

      #endregion
    }
    #endregion

    #region Class constants
    /// <summary>
    /// 
    /// </summary>
    internal const int DEF_TYPE_REF    = 0;
    /// <summary>
    /// 
    /// </summary>
    internal const int DEF_TYPE_VALUE  = 1;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_TYPE_ARRAY  = 2;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_INDEX_DEFAULT = 0;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_INDEX_ARRAY = 1;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_INDEX_NAME = 2;
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_INDEX_ROOT_LEVEL = 3;
    /// <summary>
    /// Default regular expression options.
    /// </summary>
    private const RegexOptions DEF_REGEX = 
#if !SILVERLIGHT && !WINRT && !WP
        RegexOptions.Compiled;
#else
        RegexOptions.None;
#endif
    /// <summary>
    /// Default index inside named range formula.
    /// </summary>
    public const int DEF_NAME_INDEX = 1;
    /// <summary>
    /// Default reference index.
    /// </summary>
    public const int DEF_REFERENCE_INDEX = 2;
    /// <summary>
    /// Default reference index for array token.
    /// </summary>
    public const int DEF_ARRAY_INDEX = 2;
    /// <summary>
    /// 
    /// </summary>
    internal const int DEF_OPTIONS_OPT_GOTO = 8;
    /// <summary>
    /// 
    /// </summary>
    internal const int DEF_OPTIONS_NOT_OPT_GOTO = 0;
    /// <summary>
    /// 
    /// </summary>
    private const char DEF_BOOKNAME_OPENBRACKET = '[';
    /// <summary>
    /// 
    /// </summary>
    private const char DEF_BOOKNAME_CLOSEBRACKET = ']';
    /// <summary>
    /// Column of the first cell.
    /// </summary>
    public const string DEF_GROUP_COLUMN1 = "Column1";
    /// <summary>
    /// Column of the second cell.
    /// </summary>
    public const string DEF_GROUP_COLUMN2 = "Column2";
    /// <summary>
    /// Row of the first cell.
    /// </summary>
    public const string DEF_GROUP_ROW1 = "Row1";
    /// <summary>
    /// Row of the second cell.
    /// </summary>
    public const string DEF_GROUP_ROW2 = "Row2";
    /// <summary>
    /// Worksheet name can be enclosed in this characters.
    /// </summary>
    private const char DEF_SHEET_NAME_DELIM = '\'';
    /// <summary>
    /// 
    /// </summary>
    private static readonly int[][][] DEF_INDEXES_CONVERTION = new int[][][]
    {
      // Current position is a function parameter expecting a reference class token.
      // First index - token type (0 - ref, 1 - val, 2 - arr).
      // Second index - result token type.
      new int[][]
      {
        new int[]{ 1, 1, 1 },
        new int[]{ 2, 3, 3 },
        new int[]{ 3, 3, 3 },
      },

      // Current position is a function parameter expecting a value class token.
      new int[][]
      {
        new int[]{ 2, 2, 3 },
        new int[]{ 2, 2, 3 },
        new int[]{ 2, 2, 3 },
      },

      // Current position is a function parameter expecting an array class token.
      new int[][]
      {
        new int[]{ 3, 3, 3 },
        new int[]{ 3, 3, 3 },
        new int[]{ 3, 3, 3 },
      },

      // Current position is the root level of the formula.
      new int[][]
      {
        new int[]{ 2, 2, 1 },
        new int[]{ 2, 2, 3 },
        new int[]{ 2, 2, 3 },
      },
    };
    public const string Excel2010FunctionPrefix = "_xlfn.";
    #endregion

    #region Class static constants
    /// <summary>
    /// Array of all open brackets.
    /// </summary>
    public static readonly char[] OpenBrackets  = new char[]{ '{', '(', '"', '\'', '[' };
    /// <summary>
    /// T corresponding close brackets.
    /// </summary>
    public static readonly char[] CloseBrackets = new char[]{ '}', ')', '"', '\'', ']' };
    /// <summary>
    /// String 'brackets'.
    /// </summary>
    public static readonly char[] StringBrackets = new char[]{ '"' };
    /// <summary>
    /// All known unary operations.
    /// </summary>
    public static readonly string[] UnaryOperations = new string[]
    {
      "%",
      "(",
      "+",
      "-",
    };
    /// <summary>
    /// Plus and minus signs.
    /// </summary>
    public static readonly string[] PlusMinusArray = new string[]
    {
      "+",
      "-",
    };

    /// <summary>
    /// Plus minus signs in sorted storage.
    /// </summary>
    private static readonly StorageType m_listPlusMinus = GetSortedList( PlusMinusArray );
    /// <summary>
    /// Gives access to the function name by its id.
    /// </summary>
    public static readonly Dictionary<ExcelFunction, string> FunctionIdToAlias = new Dictionary<ExcelFunction, string>( 407 );
    /// <summary>
    /// Gives access to the number of parameter needed by the function.
    /// </summary>
    public static readonly Dictionary<ExcelFunction, int> FunctionIdToParamCount = new Dictionary<ExcelFunction, int>( 407 );
    /// <summary>
    /// Provides access to function id by its name.
    /// </summary>
    public static readonly Dictionary<string, ExcelFunction> FunctionAliasToId = new Dictionary<string, ExcelFunction>();
    /// <summary>
    /// Provides access to Dictionary that contains all Reference index 
    /// by type of the argument token.
    /// </summary>
    public static readonly Dictionary<ExcelFunction, Dictionary<Type, ReferenceIndexAttribute>> FunctionIdToIndex =
      new Dictionary<ExcelFunction,Dictionary<Type,ReferenceIndexAttribute>>( 407 );

    /// <summary>
    /// Provides access to constructor that takes one string argument 
    /// by name of the error.
    /// </summary>
    public static readonly Dictionary<string, ConstructorInfo> ErrorNameToConstructor = new Dictionary<string, ConstructorInfo>( 7 );
    /// <summary>
    /// Dictionary error code to name.
    /// </summary>
    private static readonly Dictionary<int, string> s_hashErrorCodeToName = new Dictionary<int, string>( 7 );
    /// <summary>
    /// Dictionary error name to error code.
    /// </summary>
    private static readonly Dictionary<string, int> s_hashNameToErrorCode = new Dictionary<string, int>( 7 );
    /// <summary>
    /// Provides access to TokenConstructorAttribute by token code.
    /// This table allows user to rewrite default token classes.
    /// </summary>
    private static readonly Dictionary<FormulaToken, TokenConstructor> TokenCodeToConstructor =
      new Dictionary<FormulaToken, TokenConstructor>( 25 );
    /// <summary>
    /// Represents hash table that get access by token code to PTG.
    /// Key - token code. Value - instance of created PTG.
    /// </summary>
    private static readonly Dictionary<FormulaToken, Ptg> s_hashTokenCodeToPtg = new Dictionary<FormulaToken, Ptg>();

    /// <summary>
    /// Regular expression for checking if specified string is cell reference.
    /// </summary>
    public static readonly Regex  CellRegex = new Regex( 
      @"(?<Column1>[\$]?[A-Za-z]{1,3})(?<Row1>[\$]?\d+)", 
      DEF_REGEX );
    /// <summary>
    /// Regular expression for checking if specified string is cell reference.
    /// </summary>
    public static readonly Regex  CellR1C1Regex = new Regex( 
      @"(?<Row1>R[\[]?[\-]?[0-9]*[\]]?)(?<Column1>C[\[]?[\-]?[0-9]*[\]]?)",
      DEF_REGEX );
    /// <summary>
    /// Regular expression for checking if specified string is cell range.
    /// </summary>
    public static readonly Regex  CellRangeRegex = new Regex( 
      @"(?<Column1>[\$]?[A-Za-z]{1,3})(?<Row1>[\$]?\d+):(?<Column2>[\$]?[A-Za-z]{1,3})(?<Row2>[\$]?\d+)",
      //"(?<Column1>[\\$][A-Z]{1,3})(?<Row1>[\\$]\\d+):(?<Column2>[\\$][A-Z]{1,3})(?<Row2>[\\$]\\d+)", 
      DEF_REGEX );
    /// <summary>
    /// Regular expression for checking if specified string is full row range.
    /// </summary>
    public static readonly Regex FullRowRangeRegex = new Regex(
      @"(?<Row1>[\$]?\d+):(?<Row2>[\$]?\d+)", DEF_REGEX );
    /// <summary>
    /// Regular expression for checking if specified string is full column range.
    /// </summary>
    public static readonly Regex FullColumnRangeRegex = new Regex(
      @"(?<Column1>[\$]?[A-Za-z]{1,3}):(?<Column2>[\$]?[A-Za-z]{1,3})", DEF_REGEX );
    /// <summary>
    /// Regular expression for checking if specified string is full row range in R1C1 notation
    /// </summary>
    public static readonly Regex FullRowRangeR1C1Regex = new Regex(
      @"(?<Row1>R[\[]?[\-]?[0-9]*[\]]?):(?<Row2>R[\[]?[\-]?[0-9]*[\]]?)", DEF_REGEX);
    /// <summary>
    /// Regular expression for checking if specified string is full column range in R1C1 notation.
    /// </summary>
    public static readonly Regex FullColumnRangeR1C1Regex = new Regex(
      @"(?<Column1>C[\[]?[\-]?[0-9]*[\]]?):(?<Column2>C[\[]?[\-]?[0-9]*[\]]?)", DEF_REGEX);
    /// <summary>
    /// Regular expression for checking if specified string is 3D full row range.
    /// </summary>
    public static readonly Regex Full3DRowRangeRegex = new Regex(
      @"(?<" + DEF_SHEET_NAME + @">" + DEF_SHEET_NAME_REG_EXPR
      + @")[\!](?<Row1>[\$]\d+):(?<Row2>[\$]\d+)", DEF_REGEX );
    /// <summary>
    /// Regular expression for checking if specified string is 3D full column range.
    /// </summary>
    public static readonly Regex Full3DColumnRangeRegex = new Regex(
      @"(?<" + DEF_SHEET_NAME + @">" + DEF_SHEET_NAME_REG_EXPR
      + @")[\!](?<Column1>[\$][A-Za-z]{1,3}):(?<Column2>[\$][A-Za-z]{1,3})", DEF_REGEX );
    /// <summary>
    /// Regular expression for checking if specified string is cell range in R1C1 notation.
    /// </summary>
    public static readonly Regex  CellRangeR1C1Regex = new Regex( 
      @"(?<Row1>R[\[]?[\-]?[0-9]*[\]]?)(?<Column1>C[\[]?[\-]?[0-9]*[\]]?):"
      + @"(?<Row2>R[\[]?[\-]?[0-9]*[\]]?)(?<Column2>C[\[]?[\-]?[0-9]*[\]]?)",
      DEF_REGEX );
    /// <summary>
    /// Regular expression for checking if specified string is cell range in R1C1 notation.
    /// </summary>
    public static readonly Regex  CellRangeR1C1ShortRegex = new Regex( 
      @"[R|C][\[]?[\-]?[\-0-9]*[\]]?",
      DEF_REGEX );
    /// <summary>
    /// Regular expression for checking if specified string is cell range in R1C1 notation.
    /// </summary>
    public static readonly Regex  CellRangeR1C13DShortRegex = new Regex(
      @"(?<" + DEF_SHEET_NAME + @">" + DEF_SHEET_NAME_REG_EXPR + @")[\!][R|C][\[]?[\-]?[0-9]*[\]]?",
      DEF_REGEX );
    /// <summary>
    /// Name of the sheet name group.
    /// </summary>
    public const string DEF_SHEETNAME_GROUP = "SheetName";
    /// <summary>
    /// Name of the book name group.
    /// </summary>
    public const string DEF_BOOKNAME_GROUP = "BookName";
    /// <summary>
    /// Name of the range name group.
    /// </summary>
    public const string DEF_RANGENAME_GROUP = "RangeName";
    /// <summary>
    /// Name of the first row name group.
    /// </summary>
    public const string DEF_ROW_GROUP = "Row1";
    /// <summary>
    /// Name of the first column name group.
    /// </summary>
    public const string DEF_COLUMN_GROUP = "Column1";
    /// <summary>
    /// Name of the path group.
    /// </summary>
    public const string DEF_PATH_GROUP = "Path";
    /// <summary>
    /// Name of sheet.
    /// </summary>
    private const string DEF_SHEET_NAME = "SheetName";
    /// <summary>
    /// Default sheet name regular expression.
    /// </summary>
    private const string DEF_SHEET_NAME_REG_EXPR = @"[^][:\/?]*";
    /// <summary>
    /// Regular expression for 3d reference detection.
    /// </summary>
    public static readonly Regex  Cell3DRegex = new Regex(
      //@"(?<SheetName>[^][:\/?]*)[\!](?<Column1>[\$]?[A-Z]{1,3})(?<Row1>[\$]?\d+)",
      @"(?<" + DEF_SHEET_NAME + @">" + DEF_SHEET_NAME_REG_EXPR
      + @")[\!](?<Column1>[\$]?[A-Za-z]{1,3})(?<Row1>[\$]?\d+)",
      DEF_REGEX );
    /// <summary>
    /// Regular expression for 3d reference detection in R1C1 notation.
    /// </summary>
    public static readonly Regex  CellR1C13DRegex = new Regex(
      @"(?<" + DEF_SHEET_NAME + @">" + DEF_SHEET_NAME_REG_EXPR
      + @")[\!](?<Row1>R[\[]?[\-]?[0-9]*[\]]?)(?<Column1>C[\[]?[\-]?[0-9]*[\]]?)",
      DEF_REGEX );
    /// <summary>
    /// Regular expression for 3d cell range detection.
    /// </summary>
    public static readonly Regex  CellRange3DRegex = new Regex(
      @"(?<" + DEF_SHEET_NAME + @">" + DEF_SHEET_NAME_REG_EXPR
      + @")[\!](?<Column1>[\$]?[A-Za-z]{1,3})(?<Row1>[\$]?\d+):(?<Column2>[\$]?[A-Za-z]{1,3})(?<Row2>[\$]?\d+)",
      DEF_REGEX );
    /// <summary>
    /// Regular expression for 3d cell range detection second possible case.
    /// </summary>
    public static readonly Regex  CellRange3DRegex2 = new Regex(
      @"(?<" + DEF_SHEET_NAME + @">" + DEF_SHEET_NAME_REG_EXPR
      + @")[\!](?<Column1>[\$]?[A-Za-z]{1,3})(?<Row1>[\$]?\d+):(?<" + DEF_SHEET_NAME + @"2>"
      + DEF_SHEET_NAME_REG_EXPR + @")[\!](?<Column2>[\$]?[A-Za-z]{1,3})(?<Row2>[\$]?\d+)",
      DEF_REGEX );
    /// <summary>
    /// Regular expression for 3d cell range detection.
    /// </summary>
    public static readonly Regex  CellRangeR1C13DRegex = new Regex(
      @"(?<" + DEF_SHEET_NAME + @">" + DEF_SHEET_NAME_REG_EXPR
      + @")[\!](?<Row1>[R]?[\[]?[\-]?[0-9]*[\]]?)(?<Column1>[C]?[\[]?[\-]?[0-9]*[\]]?):"
      + @"(?<Row2>[R]?[\[]?[\-]?[0-9]*[\]]?)(?<Column2>[C]?[\[]?[\-]?[0-9]*[\]]?)",
      DEF_REGEX );
    /// <summary>
    /// Regular expression for 3d cell range detection second possible case.
    /// </summary>
    public static readonly Regex  CellRangeR1C13DRegex2 = new Regex(
      @"(?<" + DEF_SHEET_NAME + @">" + DEF_SHEET_NAME_REG_EXPR
      + @")[\!](?<Row1>[R]?[\[]?[\-]?[0-9]*[\]]?)(?<Column1>[C]?[\[]?[\-]?[0-9]*[\]]?):"
      + @"(?<" + DEF_SHEET_NAME + @"2>" + DEF_SHEET_NAME_REG_EXPR
      + @")[\!](?<Row2>[R]?[\[]?[\-]?[0-9]*[\]]?)(?<Column2>[C]?[\[]?[\-]?[0-9]*[\]]?)",
      DEF_REGEX );
    /// <summary>
    /// Regular expression for add-in function detection.
    /// </summary>
    private static readonly Regex  AddInFunctionRegEx = new Regex(
      //@"(?<BookName>\[[\S^ .]+\])?(?<SheetName>[\S .']+)\!(?<RangeName>[\S]+)",
      @"('?)(?<Path>[^'][^\[]+\\)?(?<BookName>[^\]]+\])?(?<" + DEF_SHEET_NAME + @">"
      + DEF_SHEET_NAME_REG_EXPR + @")\1!(?<" + DEF_RANGENAME_GROUP + @">" + DEF_SHEET_NAME_REG_EXPR + @")",
      DEF_REGEX );
    /// <summary>
    /// Array of functions that need tAttr with HasSemiVolatile = true before them.
    /// </summary>
    internal static readonly ExcelFunction[] SemiVolatileFunctions = new ExcelFunction[]
    {
      ExcelFunction.CELL,
      ExcelFunction.INFO,
      ExcelFunction.NOW,
      ExcelFunction.TODAY,
      //ExcelFunction.INDIRECT,
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly FormulaToken[] NameXCodes = new FormulaToken[]
    {
      FormulaToken.tNameX1,
      FormulaToken.tNameX2,
      FormulaToken.tNameX3
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly FormulaToken[] NameCodes = new FormulaToken[]
    {
      FormulaToken.tName1,
      FormulaToken.tName2,
      FormulaToken.tName3
    };
    private static readonly ExcelFunction[] m_excel2007Supported = new ExcelFunction[]
    {
        ExcelFunction.HEX2BIN,
        ExcelFunction.HEX2DEC,
        ExcelFunction.HEX2OCT,
        ExcelFunction.COUNTIFS,
        ExcelFunction.BIN2DEC,
        ExcelFunction.BIN2HEX,
        ExcelFunction.BIN2OCT,
        ExcelFunction.DEC2BIN,
        ExcelFunction.DEC2HEX,
        ExcelFunction.DEC2OCT,
        ExcelFunction.OCT2BIN,
        ExcelFunction.OCT2DEC,
        ExcelFunction.OCT2HEX,
        ExcelFunction.ODDFPRICE,
        ExcelFunction.ODDFYIELD,
        ExcelFunction.ODDLPRICE,
        ExcelFunction.ODDLYIELD,
        ExcelFunction.ISODD,
        ExcelFunction.ISEVEN,
        ExcelFunction.LCM,
        ExcelFunction.GCD,
        ExcelFunction.SUMIFS,
        ExcelFunction.AVERAGEIF,
        ExcelFunction.AVERAGEIFS,
        ExcelFunction.CONVERT,
        ExcelFunction.COMPLEX,
        ExcelFunction.COUPDAYBS,
        ExcelFunction.COUPDAYS,
        ExcelFunction.COUPDAYSNC,
        ExcelFunction.COUPNCD,
        ExcelFunction.COUPNUM,
        ExcelFunction.COUPPCD,
        ExcelFunction.DELTA,
        ExcelFunction.DISC,
        ExcelFunction.DOLLARDE,
        ExcelFunction.DOLLARFR,
        ExcelFunction.DURATION,
        ExcelFunction.EDATE,
        ExcelFunction.EFFECT,
        ExcelFunction.EOMONTH,
        ExcelFunction.ERF,
        ExcelFunction.ERFC,
        ExcelFunction.FACTDOUBLE,
        ExcelFunction.GESTEP,
        ExcelFunction.IFERROR,
        ExcelFunction.IMABS,
        ExcelFunction.IMAGINARY,
        ExcelFunction.IMARGUMENT,
        ExcelFunction.IMCONJUGATE,
        ExcelFunction.IMCOS,
        ExcelFunction.IMEXP,
        ExcelFunction.IMLN,
        ExcelFunction.IMLOG10,
        ExcelFunction.IMLOG2,
        ExcelFunction.IMREAL,
        ExcelFunction.IMSIN,
        ExcelFunction.IMSQRT,
        ExcelFunction.IMSUB,
        ExcelFunction.IMSUM,
        ExcelFunction.IMDIV,
        ExcelFunction.IMPOWER,
        ExcelFunction.IMPRODUCT,
        ExcelFunction.ACCRINT,
        ExcelFunction.ACCRINTM,
    };
    private static readonly ExcelFunction[] m_excel2010Supported = new ExcelFunction[]
        {
            ExcelFunction.AGGREGATE,
            ExcelFunction.CHISQ_DIST,
            ExcelFunction.CHISQ_DIST,            
            ExcelFunction.BETA_INV,
            ExcelFunction.BETA_DIST,
            ExcelFunction.BINOM_DIST,
            ExcelFunction.BINOM_INV,
            ExcelFunction.CEILING_PRECISE,
            ExcelFunction.CHISQ_DIST_RT,
            ExcelFunction.CHISQ_INV_RT,
            ExcelFunction.CHISQ_TEST,
            ExcelFunction.CONFIDENCE_NORM,
            ExcelFunction.CONFIDENCE_T,
            ExcelFunction.COVARIANCE_P,
            ExcelFunction.COVARIANCE_S,
            ExcelFunction.ERF_PRECISE,
            ExcelFunction.ERFC_PRECISE,
            ExcelFunction.EXPON_DIST,
            ExcelFunction.F_DIST,
            ExcelFunction.F_DIST_RT,
            ExcelFunction.F_INV,
            ExcelFunction.F_INV_RT,
            ExcelFunction.F_TEST,
            ExcelFunction.FLOOR_PRECISE,
            ExcelFunction.GAMMA_DIST,
            ExcelFunction.GAMMA_INV,
            ExcelFunction.GAMMALN_PRECISE,
            ExcelFunction.HYPGEOM_DIST,
            ExcelFunction.LOGNORM_DIST,
            ExcelFunction.LOGNORM_INV,
            ExcelFunction.MODE_MULT,
            ExcelFunction.MODE_SNGL,
            ExcelFunction.NEGBINOM_DIST,
            ExcelFunction.NETWORKDAYS_INTL,
            ExcelFunction.NORM_DIST,
            ExcelFunction.NORM_INV,
            ExcelFunction.NORM_S_DIST,
            ExcelFunction.PERCENTILE_EXC,
            ExcelFunction.PERCENTILE_INC,
            ExcelFunction.PERCENTRANK_EXC,
            ExcelFunction.PERCENTRANK_INC,
            ExcelFunction.POISSON_DIST,
            ExcelFunction.QUARTILE_EXC,
            ExcelFunction.QUARTILE_INC,
            ExcelFunction.RANK_AVG,
            ExcelFunction.RANK_EQ,
            ExcelFunction.STDEV_P,
            ExcelFunction.STDEV_S,
            ExcelFunction.T_DIST,
            ExcelFunction.T_DIST_2T,
            ExcelFunction.T_DIST_RT,
            ExcelFunction.T_INV,
            ExcelFunction.T_INV_2T,
            ExcelFunction.T_TEST,
            ExcelFunction.VAR_P,
            ExcelFunction.VAR_S,
            ExcelFunction.WEIBULL_DIST,
            ExcelFunction.WORKDAY_INTL,
            ExcelFunction.Z_TEST
        };
    private static readonly ExcelFunction[] m_excel2013Supported = new ExcelFunction[]
        {
            ExcelFunction.DAYS,
            ExcelFunction.ISOWEEKNUM,
            ExcelFunction.BITAND,
            ExcelFunction.BITLSHIFT,
            ExcelFunction.BITOR,
            ExcelFunction.BITRSHIFT,
            ExcelFunction.BITXOR,
            ExcelFunction.IMCOSH,
            ExcelFunction.IMCOT,
            ExcelFunction.IMCSC,
            ExcelFunction.IMCSCH,
            ExcelFunction.IMSEC,
            ExcelFunction.IMSECH,
            ExcelFunction.IMSINH,
            ExcelFunction.IMTAN,
            ExcelFunction.PDURATION,
            ExcelFunction.RRI,
            ExcelFunction.ISFORMULA,
            ExcelFunction.SHEET,
            ExcelFunction.SHEETS,
            ExcelFunction.IFNA,
            ExcelFunction.XOR,
            ExcelFunction.FORMULATEXT,
            ExcelFunction.ACOT,
            ExcelFunction.ACOTH,
            ExcelFunction.ARABIC,
            ExcelFunction.BASE,
            ExcelFunction.CEILING_MATH,
            ExcelFunction.COMBINA,
            ExcelFunction.COT,
            ExcelFunction.COTH,
            ExcelFunction.CSC,
            ExcelFunction.CSCH,
            ExcelFunction.DECIMAL,
            ExcelFunction.FLOOR_MATH,
            ExcelFunction.ISO_CEILING,
            ExcelFunction.MUNIT,
            ExcelFunction.SEC,
            ExcelFunction.SECH,
            ExcelFunction.BINOM_DIST_RANGE,
            ExcelFunction.GAMMA,
            ExcelFunction.GAUSS,
            ExcelFunction.PERMUTATIONA,
            ExcelFunction.PHI,
            ExcelFunction.SKEW_P,
            ExcelFunction.NUMBERVALUE,
            ExcelFunction.UNICHAR,
            ExcelFunction.UNICODE,
            ExcelFunction.ENCODEURL,
            ExcelFunction.FILTERXML,
            ExcelFunction.WEBSERVICE,
        };
    #endregion

    #region Class members
    /// <summary>
    /// Represents number format for parsing double value.
    /// </summary>
    private NumberFormatInfo m_numberFormat;
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// All known operations for invariant culture.
    /// </summary>
    private static readonly string[] m_arrAllOperationsDefault = new string[]
    {
      " ",
      "&",
      "*",
      "+",
      ",",
      "-",
      "/",
      //      ":",
      "<>",
      "<=",
      "<",
      "=",
      ">=",
      ">",
      "^",
    };
    /// <summary>
    /// Operations with priorities.
    /// </summary>
    private string[][] m_arrOperationGroups = new string[][]
    {
      new string[]{ " ", "," },
      new string[]{ "^" },
      new string[]{ "*", "/" },
      new string[]{ "+", "-" },
      new string[]{ "&" },
      new string[]{ "<", "<=", "<>", "=", ">", ">=", },
    };
    /// <summary>
    /// All operations sorted by operation string.
    /// Key - operation string, Value - operation priority.
    /// </summary>
    private StorageType m_arrAllOperations =new StorageType(
#if ( WINRT )
    StringComparer.CurrentCulture    
#elif  (SILVERLIGHT || WP)
      StringComparer.Create( CultureInfo.CurrentCulture, false )
#else
    new StringComparer()
#endif
);
    /// <summary>
    /// 
    /// </summary>
    private StorageType[] m_arrOperationsWithPriority;
    /// <summary>
    /// Array row separator.
    /// </summary>
    private string m_strArrayRowSeparator = ";";
    /// <summary>
    /// Operands separator.
    /// </summary>
    private string m_strOperandsSeparator = ",";
    /// <summary>
    /// Formula parser.
    /// </summary>
    private FormulaParser m_parser;
    #endregion

    #region Class events
    /// <summary>
    /// Event handler that will receive array of Ptg after parsing.
    /// </summary>
    public static event EvaluateEventHandler FormulaEvaluator;
    #endregion

    #region Static constructor
    /// <summary>
    /// Static constructor. It does all necessary preparations,
    /// such as preparing all hashtables for functions, errors 
    /// and token constructor.
    /// </summary>
    static FormulaUtil()
    {
      FillTokenConstructors();
      FillExcelFunctions();
      FillErrorNames();
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of the formula parsing utility.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    public FormulaUtil( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();

      FillDefaultOperations();
      m_arrOperationsWithPriority = new StorageType[ m_arrOperationGroups.Length ];
      FillPriorities();

      IApplication app = Application;
      m_parser = new FormulaParser( m_book );
      SetSeparators( app.ArgumentsSeparator, app.RowSeparator );
    }

    /// <summary>
    /// Initializes new instance of the formula parsing utility.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="numberFormat">Number format information.</param>
    /// <param name="chArgumentsSeparator">Argument separator.</param>
    /// <param name="chRowSeparator">Row separator.</param>
    public FormulaUtil( IApplication application, object parent, NumberFormatInfo numberFormat,
      char chArgumentsSeparator, char chRowSeparator )
      : base( application, parent )
    {
      FindParents();

      FillDefaultOperations();
      m_arrOperationsWithPriority = new StorageType[ m_arrOperationGroups.Length ];
      FillPriorities();

      m_parser = new FormulaParser( m_book );
      m_parser.NumberFormat = numberFormat;
      m_numberFormat = numberFormat;
      SetSeparators( chArgumentsSeparator, chRowSeparator );
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void FindParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "Workbook", "Can't find parent workbook" );
    }
    /// <summary>
    /// Fills operations with default values.
    /// </summary>
    private void FillDefaultOperations()
    {
      for( int i = 0, len = m_arrAllOperationsDefault.Length; i < len; i++ )
      {
        m_arrAllOperations.Add( m_arrAllOperationsDefault[ i ], null );
      }
    }
    #endregion

    #region Private methods for constructor use
    /// <summary>
    /// Fills internal hashtable that permits to construct
    /// token classes by token code.
    /// </summary>
    private static void FillTokenConstructors()
    {
      //Assembly currentAssembly = Assembly.GetAssembly( typeof( Ptg ) );
     
#if ( WINRT )
         if(ApplicationImpl.AssemblyTypes==null)
             ApplicationImpl.InitAssemblyTypes();
         Type[] allTypes = ApplicationImpl.AssemblyTypes;
#else
        Type[] allTypes =  ApplicationImpl.AssemblyTypes;
#endif
         Type ptgType = typeof( Ptg );

      for( int i = 0; i < allTypes.Length; i++ )
      {
        if( allTypes[ i ].IsSubclassOf( ptgType ) )
          RegisterTokenClass( allTypes[ i ] );
      }
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Fills all information about known Excel functions.
    /// </summary>
    private static void FillExcelFunctionsReflection()
    {
      string[] arrNames = Enum.GetNames( typeof( ExcelFunction ) );
      Type excelFunctionType = typeof( ExcelFunction );

      for( int i = 0, len = arrNames.Length; i < len; i++ )
      {
        string strName = arrNames[ i ];

        ExcelFunction index = ( ExcelFunction ) Enum.Parse( typeof( ExcelFunction ), strName, true );
        MemberInfo[] m = excelFunctionType.GetMember( strName );
        MemberInfo member = m[ 0 ];
        
        DefaultValueAttribute defAttr = ( DefaultValueAttribute )
          Attribute.GetCustomAttribute( member, typeof( DefaultValueAttribute ) );
        
        DescriptionAttribute aliasAttr = ( DescriptionAttribute )
          Attribute.GetCustomAttribute( member, typeof( DescriptionAttribute ) );
        
        ReferenceIndexAttribute[] refAttributes = ( ReferenceIndexAttribute[] )
          Attribute.GetCustomAttributes( member, typeof( ReferenceIndexAttribute ) );

        RegisterFunction( ( aliasAttr != null ) ? aliasAttr.Description : strName, index,
          refAttributes, ( defAttr != null ) ? (int)defAttr.Value : -1 );
      }
    }
#endif
    /// <summary>
    /// Fills all information about known Excel functions.
    /// </summary>
    private static void FillExcelFunctions()
    {
      ReferenceIndexAttribute[] attributes;

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "COUNT", ExcelFunction.COUNT, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "IF", ExcelFunction.IF, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ISNA", ExcelFunction.ISNA, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ISERROR", ExcelFunction.ISERROR, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "SUM", ExcelFunction.SUM, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "AVERAGE", ExcelFunction.AVERAGE, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 ),
        new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "MIN", ExcelFunction.MIN, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 ),
      new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "MAX", ExcelFunction.MAX, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "ROW", ExcelFunction.ROW, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "COLUMN", ExcelFunction.COLUMN, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "NA", ExcelFunction.NA, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 ),
        new ReferenceIndexAttribute( typeof( AreaPtg ), 2, 1 )
		  };
      RegisterFunction( "NPV", ExcelFunction.NPV, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "STDEV", ExcelFunction.STDEV, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "DOLLAR", ExcelFunction.DOLLAR, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "FIXED", ExcelFunction.FIXED, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "SIN", ExcelFunction.SIN, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "COS", ExcelFunction.COS, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "TAN", ExcelFunction.TAN, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ATAN", ExcelFunction.ATAN, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "PI", ExcelFunction.PI, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "SQRT", ExcelFunction.SQRT, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "EXP", ExcelFunction.EXP, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "LN", ExcelFunction.LN, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "LOG10", ExcelFunction.LOG10, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ABS", ExcelFunction.ABS, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "INT", ExcelFunction.INT, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "SIGN", ExcelFunction.SIGN, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ROUND", ExcelFunction.ROUND, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
            new ReferenceIndexAttribute(typeof(RefPtg),2)
        };
            RegisterFunction("HEX2BIN", ExcelFunction.HEX2BIN, attributes, 1);

            attributes = new ReferenceIndexAttribute[]
        {
            new ReferenceIndexAttribute(typeof(RefPtg),2)
        };
            RegisterFunction("HEX2DEC", ExcelFunction.HEX2DEC, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
        {
            new ReferenceIndexAttribute(typeof(RefPtg),1,2)
        };
            RegisterFunction("HEX2OCT", ExcelFunction.HEX2OCT, attributes, -1);
            attributes = new ReferenceIndexAttribute[]
        {
            new ReferenceIndexAttribute(typeof(RefPtg),2)
        };
            RegisterFunction("BIN2DEC", ExcelFunction.BIN2DEC, attributes, -1);
            attributes = new ReferenceIndexAttribute[]
        {
            new ReferenceIndexAttribute(typeof(RefPtg),2)
        };
            RegisterFunction("BIN2HEX", ExcelFunction.BIN2HEX, attributes, -1);
            attributes = new ReferenceIndexAttribute[]
        {
            new ReferenceIndexAttribute(typeof(RefPtg),2)
        };
            RegisterFunction("BIN2OCT", ExcelFunction.BIN2OCT, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
        {
            new ReferenceIndexAttribute(typeof(RefPtg),1,2)
        };
            RegisterFunction("DEC2BIN", ExcelFunction.DEC2BIN, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
        {
            new ReferenceIndexAttribute(typeof(RefPtg),1,2)
        };
            RegisterFunction("DEC2HEX", ExcelFunction.DEC2HEX, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
        {
            new ReferenceIndexAttribute(typeof(RefPtg),1,2)
        };
            RegisterFunction("DEC2OCT", ExcelFunction.DEC2OCT, attributes, -1);
            attributes = new ReferenceIndexAttribute[]
        {
            new ReferenceIndexAttribute(typeof(RefPtg),1,2)
        };
            RegisterFunction("OCT2BIN", ExcelFunction.OCT2BIN, attributes, -1);
            attributes = new ReferenceIndexAttribute[]
        {
            new ReferenceIndexAttribute(typeof(RefPtg),2)
        };
            RegisterFunction("OCT2DEC", ExcelFunction.OCT2DEC, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
        {
            new ReferenceIndexAttribute(typeof(RefPtg),1,2)
        };
            RegisterFunction("OCT2HEX", ExcelFunction.OCT2HEX, attributes, -1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("ODDFPRICE", ExcelFunction.ODDFPRICE, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("ODDFYIELD", ExcelFunction.ODDFYIELD, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("ODDLPRICE", ExcelFunction.ODDLPRICE, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("ODDLYIELD", ExcelFunction.ODDLYIELD, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("ISEVEN", ExcelFunction.ISEVEN, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("ISODD", ExcelFunction.ISODD, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("AVERAGEIFS", ExcelFunction.AVERAGEIFS, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
             {
                 new ReferenceIndexAttribute(typeof(RefPtg),2)
             };
            RegisterFunction("AVERAGEIF", ExcelFunction.AVERAGEIF, attributes, -1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CONVERT", ExcelFunction.CONVERT, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("COMPLEX", ExcelFunction.COMPLEX, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("COUPDAYBS", ExcelFunction.COUPDAYBS, attributes, 4);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("COUPDAYS", ExcelFunction.COUPDAYS, attributes, 4);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("COUPDAYSNC", ExcelFunction.COUPDAYSNC, attributes, 4);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)

            };
            RegisterFunction("COUPNCD", ExcelFunction.COUPNCD, attributes, 4);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)

            };
            RegisterFunction("COUPNUM", ExcelFunction.COUPNUM, attributes, 4);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)

            };
            RegisterFunction("COUPPCD", ExcelFunction.COUPPCD, attributes, 4);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),1,2)
            };

            RegisterFunction("DELTA", ExcelFunction.DELTA, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("DISC", ExcelFunction.DISC, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("DOLLARDE", ExcelFunction.DOLLARDE, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("DOLLARFR", ExcelFunction.DOLLARFR, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("DURATION", ExcelFunction.DURATION, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("EDATE", ExcelFunction.EDATE, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("EFFECT", ExcelFunction.EFFECT, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
                    
            };
            RegisterFunction("EOMONTH", ExcelFunction.EOMONTH, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),1,2)
            };
            RegisterFunction("ERF", ExcelFunction.ERF, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("ERFC", ExcelFunction.ERFC, attributes, 1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("FACTDOUBLE", ExcelFunction.FACTDOUBLE, attributes, 1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),1,2)
            };
            RegisterFunction("GESTEP", ExcelFunction.GESTEP, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IFERROR", ExcelFunction.IFERROR, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMABS", ExcelFunction.IMABS, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMAGINARY", ExcelFunction.IMAGINARY, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMARGUMENT", ExcelFunction.IMARGUMENT, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMCONJUGATE", ExcelFunction.IMCONJUGATE, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMCOS", ExcelFunction.IMCOS, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMEXP", ExcelFunction.IMEXP, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMLN", ExcelFunction.IMLN, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMLOG10", ExcelFunction.IMLOG10, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMLOG2", ExcelFunction.IMLOG2, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMPOWER", ExcelFunction.IMPOWER, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMPRODUCT", ExcelFunction.IMPRODUCT, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMREAL", ExcelFunction.IMREAL, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
             {
                 new ReferenceIndexAttribute(typeof(RefPtg),2)
             };
            RegisterFunction("IMSIN", ExcelFunction.IMSIN, attributes, 1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMSQRT", ExcelFunction.IMSQRT, attributes, 1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMSUB", ExcelFunction.IMSUB, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMSUM", ExcelFunction.IMSUM, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("IMDIV", ExcelFunction.IMDIV, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
                {
                    new ReferenceIndexAttribute(typeof(RefPtg),2)
                };
            RegisterFunction("LCM", ExcelFunction.LCM, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("SUMIFS", ExcelFunction.SUMIFS, attributes, -1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("GCD", ExcelFunction.GCD, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
        {
            new ReferenceIndexAttribute(typeof(RefPtg),2)
        };
            RegisterFunction("COUNTIFS", ExcelFunction.COUNTIFS, attributes, -1);


            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("ACCRINT", ExcelFunction.ACCRINT, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("ACCRINTM", ExcelFunction.ACCRINTM, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),1)  ,    
                new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 )
            };
            RegisterFunction("AGGREGATE", ExcelFunction.AGGREGATE, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("AMORDEGRC", ExcelFunction.AMORDEGRC, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("AMORLINC", ExcelFunction.AMORLINC, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("BAHTTEXT", ExcelFunction.BAHTTEXT, attributes, 1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("BESSELI", ExcelFunction.BESSELI, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("BESSELJ", ExcelFunction.BESSELJ, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("BESSELK", ExcelFunction.BESSELK, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("BESSELY", ExcelFunction.BESSELY, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CUBEKPIMEMBER", ExcelFunction.CUBEKPIMEMBER, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CUBEMEMBER", ExcelFunction.CUBEMEMBER, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CUBERANKEDMEMBER", ExcelFunction.CUBERANKEDMEMBER, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CUBESET", ExcelFunction.CUBESET, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CUBESETCOUNT", ExcelFunction.CUBESETCOUNT, attributes, 1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CUBEMEMBERPROPERTY", ExcelFunction.CUBEMEMBERPROPERTY, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CUMIPMT", ExcelFunction.CUMIPMT, attributes, 6);
            attributes = new ReferenceIndexAttribute[]
             {
                 new ReferenceIndexAttribute(typeof(RefPtg),2)
             };
            RegisterFunction("CUMPRINC", ExcelFunction.CUMPRINC, attributes, 6);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("FVSCHEDULE", ExcelFunction.FVSCHEDULE, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("INTRATE", ExcelFunction.INTRATE, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CUBEVALUE", ExcelFunction.CUBEVALUE, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("MDURATION", ExcelFunction.MDURATION, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("MROUND", ExcelFunction.MROUND, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("MULTINOMIAL", ExcelFunction.MULTINOMIAL, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("NETWORKDAYS", ExcelFunction.NETWORKDAYS, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("NOMINAL", ExcelFunction.NOMINAL, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("PRICE", ExcelFunction.PRICE, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("PRICEDISC", ExcelFunction.PRICEDISC, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("PRICEMAT", ExcelFunction.PRICEMAT, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("QUOTIENT", ExcelFunction.QUOTIENT, attributes, 2);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("RANDBETWEEN", ExcelFunction.RANDBETWEEN, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("RECEIVED", ExcelFunction.RECEIVED, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("SERIESSUM", ExcelFunction.SERIESSUM, attributes, 4);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("SQRTPI", ExcelFunction.SQRTPI, attributes, 1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("TBILLEQ", ExcelFunction.TBILLEQ, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("TBILLPRICE", ExcelFunction.TBILLPRICE, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("TBILLYIELD", ExcelFunction.TBILLYIELD, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("WEEKNUM", ExcelFunction.WEEKNUM, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("WORKDAY", ExcelFunction.WORKDAY, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("XIRR", ExcelFunction.XIRR, attributes, -1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("XNPV", ExcelFunction.XNPV, attributes, 3);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("YEARFRAC", ExcelFunction.YEARFRAC, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("YIELD", ExcelFunction.YIELD, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("YIELDDISC", ExcelFunction.YIELDDISC, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("YIELDMAT", ExcelFunction.YIELDMAT, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("WORKDAY.INTL", ExcelFunction.WORKDAYINTL, attributes, -1);
            attributes = new ReferenceIndexAttribute[]
             {
                 new ReferenceIndexAttribute(typeof(RefPtg),2)
             };
            RegisterFunction("BETA.INV", ExcelFunction.BETA_INV, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("BINOM.DIST", ExcelFunction.BINOM_DIST, attributes, 4);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("BINOM.INV", ExcelFunction.BINOM_INV, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CEILING.PRECISE", ExcelFunction.CEILING_PRECISE, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CHISQ.DIST", ExcelFunction.CHISQ_DIST, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CHISQ.DIST.RT", ExcelFunction.CHISQ_DIST_RT, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CHISQ.INV", ExcelFunction.CHISQ_INV, attributes, 2);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CHISQ.INV.RT", ExcelFunction.CHISQ_INV_RT, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CHISQ.TEST", ExcelFunction.CHISQ_TEST, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CONFIDENCE.NORM", ExcelFunction.CONFIDENCE_NORM, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("CONFIDENCE.T", ExcelFunction.CONFIDENCE_T, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("COVARIANCE.P", ExcelFunction.COVARIANCE_P, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("COVARIANCE.S", ExcelFunction.COVARIANCE_S, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("ERF.PRECISE", ExcelFunction.ERF_PRECISE, attributes, 1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("ERFC.PRECISE", ExcelFunction.ERFC_PRECISE, attributes, 1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("F.DIST", ExcelFunction.F_DIST, attributes, 4);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("F.DIST.RT", ExcelFunction.F_DIST_RT, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("F.INV", ExcelFunction.F_INV, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("F.TEST", ExcelFunction.F_TEST, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("FLOOR.PRECISE", ExcelFunction.FLOOR_PRECISE, attributes, -1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("F.INV.RT", ExcelFunction.F_INV_RT, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("GAMMA.DIST", ExcelFunction.GAMMA_DIST, attributes, 4);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("GAMMA.INV", ExcelFunction.GAMMA_INV, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("GAMMALN.PRECISE", ExcelFunction.GAMMALN_PRECISE, attributes, 1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("HYPGEOM.DIST", ExcelFunction.HYPGEOM_DIST, attributes, 5);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("LOGNORM.DIST", ExcelFunction.LOGNORM_DIST, attributes, 4);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("LOGNORM.INV", ExcelFunction.LOGNORM_INV, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("MODE.MULT", ExcelFunction.MODE_MULT, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("MODE.SNGL", ExcelFunction.MODE_SNGL, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("NEGBINOM.DIST", ExcelFunction.NEGBINOM_DIST, attributes, 4);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("NETWORKDAYS.INTL", ExcelFunction.NETWORKDAYS_INTL, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("NORM.DIST", ExcelFunction.NORM_DIST, attributes, 4);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("NORM.INV", ExcelFunction.NORM_INV, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("NORM.S.DIST", ExcelFunction.NORM_S_DIST, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("PERCENTILE.EXC", ExcelFunction.PERCENTILE_EXC, attributes, 2);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("PERCENTILE.INC", ExcelFunction.PERCENTILE_INC, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("PERCENTRANK.EXC", ExcelFunction.PERCENTRANK_EXC, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("PERCENTRANK.INC", ExcelFunction.PERCENTRANK_INC, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("POISSON.DIST", ExcelFunction.POISSON_DIST, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("QUARTILE.EXC", ExcelFunction.QUARTILE_EXC, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("QUARTILE.INC", ExcelFunction.QUARTILE_INC, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("RANK.AVG", ExcelFunction.RANK_AVG, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("RANK.EQ", ExcelFunction.RANK_EQ, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("STDEV.P", ExcelFunction.STDEV_P, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("STDEV.S", ExcelFunction.STDEV_S, attributes, -1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("T.DIST", ExcelFunction.T_DIST, attributes, 3);

            attributes = new ReferenceIndexAttribute[]
            {
               new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("T.DIST.2T", ExcelFunction.T_DIST_2T, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("T.DIST.RT", ExcelFunction.T_DIST_RT, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("T.INV", ExcelFunction.T_INV, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("VAR.P", ExcelFunction.VAR_P, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("VAR.S", ExcelFunction.VAR_S, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("WEIBULL.DIST", ExcelFunction.WEIBULL_DIST, attributes, 4);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("Z.TEST", ExcelFunction.Z_TEST, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2, 1, 1 ),
        new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 )
		  };
      RegisterFunction( "LOOKUP", ExcelFunction.LOOKUP, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 ),
        new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 )
		  };
      RegisterFunction( "INDEX", ExcelFunction.INDEX, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "REPT", ExcelFunction.REPT, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "MID", ExcelFunction.MID, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "LEN", ExcelFunction.LEN, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "VALUE", ExcelFunction.VALUE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "TRUE", ExcelFunction.TRUE, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "FALSE", ExcelFunction.FALSE, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "AND", ExcelFunction.AND, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "OR", ExcelFunction.OR, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "NOT", ExcelFunction.NOT, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "MOD", ExcelFunction.MOD, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "DCOUNT", ExcelFunction.DCOUNT, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "DSUM", ExcelFunction.DSUM, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "DAVERAGE", ExcelFunction.DAVERAGE, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "DMIN", ExcelFunction.DMIN, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "DMAX", ExcelFunction.DMAX, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "DSTDEV", ExcelFunction.DSTDEV, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "VAR", ExcelFunction.VAR, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "DVAR", ExcelFunction.DVAR, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "TEXT", ExcelFunction.TEXT, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "LINEST", ExcelFunction.LINEST, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 ),
        new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 )
		  };
      RegisterFunction( "TREND", ExcelFunction.TREND, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 ),
        new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 )
		  };
      RegisterFunction( "LOGEST", ExcelFunction.LOGEST, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "GROWTH", ExcelFunction.GROWTH, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GOTO", ExcelFunction.GOTO, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "HALT", ExcelFunction.HALT, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "PV", ExcelFunction.PV, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "FV", ExcelFunction.FV, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "NPER", ExcelFunction.NPER, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "PMT", ExcelFunction.PMT, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "RATE", ExcelFunction.RATE, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 ),
        new ReferenceIndexAttribute( typeof( RefPtg ), 1, 2, 2 )
		  };
      RegisterFunction( "MIRR", ExcelFunction.MIRR, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 ),
        new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "IRR", ExcelFunction.IRR, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "RAND", ExcelFunction.RAND, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2, 1 ),
        new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 )
		  };
      RegisterFunction( "MATCH", ExcelFunction.MATCH, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "DATE", ExcelFunction.DATE, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "TIME", ExcelFunction.TIME, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "DAY", ExcelFunction.DAY, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "MONTH", ExcelFunction.MONTH, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "YEAR", ExcelFunction.YEAR, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "WEEKDAY", ExcelFunction.WEEKDAY, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "HOUR", ExcelFunction.HOUR, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "MINUTE", ExcelFunction.MINUTE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "SECOND", ExcelFunction.SECOND, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "NOW", ExcelFunction.NOW, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "AREAS", ExcelFunction.AREAS, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "ROWS", ExcelFunction.ROWS, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "COLUMNS", ExcelFunction.COLUMNS, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "OFFSET", ExcelFunction.OFFSET, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "ABSREF", ExcelFunction.ABSREF, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "RELREF", ExcelFunction.RELREF, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "ARGUMENT", ExcelFunction.ARGUMENT, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "SEARCH", ExcelFunction.SEARCH, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 ),
        new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "TRANSPOSE", ExcelFunction.TRANSPOSE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "ERROR", ExcelFunction.ERROR, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "STEP", ExcelFunction.STEP, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 ),
        new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 )
		  };
      RegisterFunction( "TYPE", ExcelFunction.TYPE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "ECHO", ExcelFunction.ECHO, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "SETNAME", ExcelFunction.SETNAME, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "CALLER", ExcelFunction.CALLER, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "DEREF", ExcelFunction.DEREF, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "WINDOWS", ExcelFunction.WINDOWS, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "SERIES", ExcelFunction.SERIES, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "DOCUMENTS", ExcelFunction.DOCUMENTS, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "ACTIVECELL", ExcelFunction.ACTIVECELL, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "SELECTION", ExcelFunction.SELECTION, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "RESULT", ExcelFunction.RESULT, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ATAN2", ExcelFunction.ATAN2, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ASIN", ExcelFunction.ASIN, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ACOS", ExcelFunction.ACOS, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "CHOOSE", ExcelFunction.CHOOSE, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2, 1 )
		  };
      RegisterFunction( "HLOOKUP", ExcelFunction.HLOOKUP, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2, 1 )
		  };
      RegisterFunction( "VLOOKUP", ExcelFunction.VLOOKUP, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "LINKS", ExcelFunction.LINKS, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "INPUT", ExcelFunction.INPUT, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "ISREF", ExcelFunction.ISREF, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETFORMULA", ExcelFunction.GETFORMULA, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETNAME", ExcelFunction.GETNAME, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "SETVALUE", ExcelFunction.SETVALUE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "LOG", ExcelFunction.LOG, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "EXEC", ExcelFunction.EXEC, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "CHAR", ExcelFunction.CHAR, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "LOWER", ExcelFunction.LOWER, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "UPPER", ExcelFunction.UPPER, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "PROPER", ExcelFunction.PROPER, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "LEFT", ExcelFunction.LEFT, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "RIGHT", ExcelFunction.RIGHT, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "EXACT", ExcelFunction.EXACT, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "TRIM", ExcelFunction.TRIM, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "REPLACE", ExcelFunction.REPLACE, attributes, 4 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "SUBSTITUTE", ExcelFunction.SUBSTITUTE, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "CODE", ExcelFunction.CODE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "NAMES", ExcelFunction.NAMES, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "DIRECTORY", ExcelFunction.DIRECTORY, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "FIND", ExcelFunction.FIND, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2, 1 )
		  };
      RegisterFunction( "CELL", ExcelFunction.CELL, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ISERR", ExcelFunction.ISERR, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ISTEXT", ExcelFunction.ISTEXT, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ISNUMBER", ExcelFunction.ISNUMBER, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ISBLANK", ExcelFunction.ISBLANK, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "T", ExcelFunction.T, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "N", ExcelFunction.N, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "FOPEN", ExcelFunction.FOPEN, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "FCLOSE", ExcelFunction.FCLOSE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "FSIZE", ExcelFunction.FSIZE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "FREADLN", ExcelFunction.FREADLN, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "FREAD", ExcelFunction.FREAD, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "FWRITELN", ExcelFunction.FWRITELN, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "FWRITE", ExcelFunction.FWRITE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "FPOS", ExcelFunction.FPOS, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "DATEVALUE", ExcelFunction.DATEVALUE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "TIMEVALUE", ExcelFunction.TIMEVALUE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "SLN", ExcelFunction.SLN, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "SYD", ExcelFunction.SYD, attributes, 4 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "DDB", ExcelFunction.DDB, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETDEF", ExcelFunction.GETDEF, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "REFTEXT", ExcelFunction.REFTEXT, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "TEXTREF", ExcelFunction.TEXTREF, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "INDIRECT", ExcelFunction.INDIRECT, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "REGISTER", ExcelFunction.REGISTER, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "CALL", ExcelFunction.CALL, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "ADDBAR", ExcelFunction.ADDBAR, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "ADDMENU", ExcelFunction.ADDMENU, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "ADDCOMMAND", ExcelFunction.ADDCOMMAND, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "ENABLECOMMAND", ExcelFunction.ENABLECOMMAND, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "CHECKCOMMAND", ExcelFunction.CHECKCOMMAND, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "RENAMECOMMAND", ExcelFunction.RENAMECOMMAND, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "SHOWBAR", ExcelFunction.SHOWBAR, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "DELETEMENU", ExcelFunction.DELETEMENU, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "DELETECOMMAND", ExcelFunction.DELETECOMMAND, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETCHARTITEM", ExcelFunction.GETCHARTITEM, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "DIALOGBOX", ExcelFunction.DIALOGBOX, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "CLEAN", ExcelFunction.CLEAN, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "MDETERM", ExcelFunction.MDETERM, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "MINVERSE", ExcelFunction.MINVERSE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "MMULT", ExcelFunction.MMULT, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "FILES", ExcelFunction.FILES, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "IPMT", ExcelFunction.IPMT, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "PPMT", ExcelFunction.PPMT, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "COUNTA", ExcelFunction.COUNTA, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "CANCELKEY", ExcelFunction.CANCELKEY, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "INITIATE", ExcelFunction.INITIATE, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "REQUEST", ExcelFunction.REQUEST, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "POKE", ExcelFunction.POKE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "EXECUTE", ExcelFunction.EXECUTE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "TERMINATE", ExcelFunction.TERMINATE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "RESTART", ExcelFunction.RESTART, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "HELP", ExcelFunction.HELP, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETBAR", ExcelFunction.GETBAR, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 ),
        new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 )
		  };
      RegisterFunction( "PRODUCT", ExcelFunction.PRODUCT, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "FACT", ExcelFunction.FACT, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETCELL", ExcelFunction.GETCELL, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETWORKSPACE", ExcelFunction.GETWORKSPACE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETWINDOW", ExcelFunction.GETWINDOW, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETDOCUMENT", ExcelFunction.GETDOCUMENT, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "DPRODUCT", ExcelFunction.DPRODUCT, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ISNONTEXT", ExcelFunction.ISNONTEXT, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETNOTE", ExcelFunction.GETNOTE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "NOTE", ExcelFunction.NOTE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "STDEVP", ExcelFunction.STDEVP, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "VARP", ExcelFunction.VARP, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "DSTDEVP", ExcelFunction.DSTDEVP, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "DVARP", ExcelFunction.DVARP, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "TRUNC", ExcelFunction.TRUNC, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ISLOGICAL", ExcelFunction.ISLOGICAL, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "DCOUNTA", ExcelFunction.DCOUNTA, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "DELETEBAR", ExcelFunction.DELETEBAR, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "UNREGISTER", ExcelFunction.UNREGISTER, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "USDOLLAR", ExcelFunction.USDOLLAR, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "FINDB", ExcelFunction.FINDB, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "SEARCHB", ExcelFunction.SEARCHB, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "REPLACEB", ExcelFunction.REPLACEB, attributes, 4 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "LEFTB", ExcelFunction.LEFTB, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "RIGHTB", ExcelFunction.RIGHTB, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "MIDB", ExcelFunction.MIDB, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "LENB", ExcelFunction.LENB, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ROUNDUP", ExcelFunction.ROUNDUP, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ROUNDDOWN", ExcelFunction.ROUNDDOWN, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "ASC", ExcelFunction.ASC, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "DBCS", ExcelFunction.DBCS, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2, 1 )
		  };
      RegisterFunction( "RANK", ExcelFunction.RANK, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "ADDRESS", ExcelFunction.ADDRESS, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "DAYS360", ExcelFunction.DAYS360, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "TODAY", ExcelFunction.TODAY, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "VDB", ExcelFunction.VDB, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 ),
        new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 )
		  };
      RegisterFunction( "MEDIAN", ExcelFunction.MEDIAN, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "SUMPRODUCT", ExcelFunction.SUMPRODUCT, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "SINH", ExcelFunction.SINH, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "COSH", ExcelFunction.COSH, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "TANH", ExcelFunction.TANH, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ASINH", ExcelFunction.ASINH, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ACOSH", ExcelFunction.ACOSH, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ATANH", ExcelFunction.ATANH, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "DGET", ExcelFunction.DGET, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "CREATEOBJECT", ExcelFunction.CREATEOBJECT, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "VOLATILE", ExcelFunction.VOLATILE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "LASTERROR", ExcelFunction.LASTERROR, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "CUSTOMUNDO", ExcelFunction.CUSTOMUNDO, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "CUSTOMREPEAT", ExcelFunction.CUSTOMREPEAT, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "FORMULACONVERT", ExcelFunction.FORMULACONVERT, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETLINKINFO", ExcelFunction.GETLINKINFO, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "TEXTBOX", ExcelFunction.TEXTBOX, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "INFO", ExcelFunction.INFO, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GROUP", ExcelFunction.GROUP, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETOBJECT", ExcelFunction.GETOBJECT, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "DB", ExcelFunction.DB, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "PAUSE", ExcelFunction.PAUSE, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "RESUME", ExcelFunction.RESUME, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "FREQUENCY", ExcelFunction.FREQUENCY, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "ADDTOOLBAR", ExcelFunction.ADDTOOLBAR, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "DELETETOOLBAR", ExcelFunction.DELETETOOLBAR, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "CustomFunction", ExcelFunction.CustomFunction, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "RESETTOOLBAR", ExcelFunction.RESETTOOLBAR, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "EVALUATE", ExcelFunction.EVALUATE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETTOOLBAR", ExcelFunction.GETTOOLBAR, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETTOOL", ExcelFunction.GETTOOL, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "SPELLINGCHECK", ExcelFunction.SPELLINGCHECK, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ERROR.TYPE", ExcelFunction.ERRORTYPE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "APPTITLE", ExcelFunction.APPTITLE, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "WINDOWTITLE", ExcelFunction.WINDOWTITLE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "SAVETOOLBAR", ExcelFunction.SAVETOOLBAR, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "ENABLETOOL", ExcelFunction.ENABLETOOL, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "PRESSTOOL", ExcelFunction.PRESSTOOL, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "REGISTERID", ExcelFunction.REGISTERID, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETWORKBOOK", ExcelFunction.GETWORKBOOK, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 ),
        new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 )
		  };
      RegisterFunction( "AVEDEV", ExcelFunction.AVEDEV, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
            RegisterFunction("BETA.DIST", ExcelFunction.BETA_DIST, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("T.TEST", ExcelFunction.T_TEST, attributes, 4);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("T.INV.2T", ExcelFunction.T_INV_2T, attributes, 2);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("EXPON.DIST", ExcelFunction.EXPON_DIST, attributes, 3);


            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction( "BETADIST", ExcelFunction.BETADIST, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
            RegisterFunction("EUROCONVERT", ExcelFunction.EUROCONVERT, attributes, 5);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("REGISTER.ID", ExcelFunction.REGISTER_ID, attributes, -1);
            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("PHONETIC", ExcelFunction.PHONETIC, attributes, 1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("SQL.REQUEST", ExcelFunction.SQL_REQUEST, attributes, -1);

            attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
            RegisterFunction("JIS", ExcelFunction.JIS, attributes, 1);

            attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "GAMMALN", ExcelFunction.GAMMALN, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "BETAINV", ExcelFunction.BETAINV, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "BINOMDIST", ExcelFunction.BINOMDIST, attributes, 4 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "CHIDIST", ExcelFunction.CHIDIST, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "CHIINV", ExcelFunction.CHIINV, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "COMBIN", ExcelFunction.COMBIN, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "CONFIDENCE", ExcelFunction.CONFIDENCE, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "CRITBINOM", ExcelFunction.CRITBINOM, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "EVEN", ExcelFunction.EVEN, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "EXPONDIST", ExcelFunction.EXPONDIST, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "FDIST", ExcelFunction.FDIST, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "FINV", ExcelFunction.FINV, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "FISHER", ExcelFunction.FISHER, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "FISHERINV", ExcelFunction.FISHERINV, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "FLOOR", ExcelFunction.FLOOR, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "GAMMADIST", ExcelFunction.GAMMADIST, attributes, 4 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "GAMMAINV", ExcelFunction.GAMMAINV, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "CEILING", ExcelFunction.CEILING, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "HYPGEOMDIST", ExcelFunction.HYPGEOMDIST, attributes, 4 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "LOGNORMDIST", ExcelFunction.LOGNORMDIST, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "LOGINV", ExcelFunction.LOGINV, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "NEGBINOMDIST", ExcelFunction.NEGBINOMDIST, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "NORMDIST", ExcelFunction.NORMDIST, attributes, 4 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "NORMSDIST", ExcelFunction.NORMSDIST, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "NORMINV", ExcelFunction.NORMINV, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "NORMSINV", ExcelFunction.NORMSINV, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "STANDARDIZE", ExcelFunction.STANDARDIZE, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ODD", ExcelFunction.ODD, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "PERMUT", ExcelFunction.PERMUT, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "POISSON", ExcelFunction.POISSON, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "TDIST", ExcelFunction.TDIST, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "WEIBULL", ExcelFunction.WEIBULL, attributes, 4 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "SUMXMY2", ExcelFunction.SUMXMY2, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "SUMX2MY2", ExcelFunction.SUMX2MY2, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "SUMX2PY2", ExcelFunction.SUMX2PY2, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "CHITEST", ExcelFunction.CHITEST, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "CORREL", ExcelFunction.CORREL, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "COVAR", ExcelFunction.COVAR, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2, 3, 3 )
		  };
      RegisterFunction( "FORECAST", ExcelFunction.FORECAST, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "FTEST", ExcelFunction.FTEST, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "INTERCEPT", ExcelFunction.INTERCEPT, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "PEARSON", ExcelFunction.PEARSON, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "RSQ", ExcelFunction.RSQ, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "STEYX", ExcelFunction.STEYX, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "SLOPE", ExcelFunction.SLOPE, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "TTEST", ExcelFunction.TTEST, attributes, 4 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3, 3, 2 )
		  };
      RegisterFunction( "PROB", ExcelFunction.PROB, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "DEVSQ", ExcelFunction.DEVSQ, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "GEOMEAN", ExcelFunction.GEOMEAN, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "HARMEAN", ExcelFunction.HARMEAN, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "SUMSQ", ExcelFunction.SUMSQ, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 ),
        new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "KURT", ExcelFunction.KURT, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "SKEW", ExcelFunction.SKEW, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 ),
        new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 )
		  };
      RegisterFunction( "ZTEST", ExcelFunction.ZTEST, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 ),	
        new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 )
		  };
      RegisterFunction( "LARGE", ExcelFunction.LARGE, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "SMALL", ExcelFunction.SMALL, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1, 2 )
		  };
      RegisterFunction( "QUARTILE", ExcelFunction.QUARTILE, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1, 2 )
		  };
      RegisterFunction( "PERCENTILE", ExcelFunction.PERCENTILE, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1, 2 )
		  };
      RegisterFunction( "PERCENTRANK", ExcelFunction.PERCENTRANK, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "MODE", ExcelFunction.MODE, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 3 )
		  };
      RegisterFunction( "TRIMMEAN", ExcelFunction.TRIMMEAN, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "TINV", ExcelFunction.TINV, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "MOVIECOMMAND", ExcelFunction.MOVIECOMMAND, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETMOVIE", ExcelFunction.GETMOVIE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "CONCATENATE", ExcelFunction.CONCATENATE, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "POWER", ExcelFunction.POWER, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "PIVOTADDDATA", ExcelFunction.PIVOTADDDATA, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETPIVOTTABLE", ExcelFunction.GETPIVOTTABLE, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETPIVOTFIELD", ExcelFunction.GETPIVOTFIELD, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "GETPIVOTITEM", ExcelFunction.GETPIVOTITEM, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "RADIANS", ExcelFunction.RADIANS, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "DEGREES", ExcelFunction.DEGREES, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2, 1 )
		  };
      RegisterFunction( "SUBTOTAL", ExcelFunction.SUBTOTAL, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1, 2, 1 )
		  };
      RegisterFunction( "SUMIF", ExcelFunction.SUMIF, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1, 2 )
		  };
      RegisterFunction( "COUNTIF", ExcelFunction.COUNTIF, attributes, 2 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "COUNTBLANK", ExcelFunction.COUNTBLANK, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "SCENARIOGET", ExcelFunction.SCENARIOGET, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "OPTIONSLISTSGET", ExcelFunction.OPTIONSLISTSGET, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ISPMT", ExcelFunction.ISPMT, attributes, 4 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "DATEDIF", ExcelFunction.DATEDIF, attributes, 3 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "DATESTRING", ExcelFunction.DATESTRING, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "NUMBERSTRING", ExcelFunction.NUMBERSTRING, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "ROMAN", ExcelFunction.ROMAN, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "OPENDIALOG", ExcelFunction.OPENDIALOG, attributes, 1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "SAVEDIALOG", ExcelFunction.SAVEDIALOG, attributes, 0 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "GETPIVOTDATA", ExcelFunction.GETPIVOTDATA, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 2 )
		  };
      RegisterFunction( "HYPERLINK", ExcelFunction.HYPERLINK, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "AVERAGEA", ExcelFunction.AVERAGEA, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 ),
        new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 )
		  };
      RegisterFunction( "MAXA", ExcelFunction.MAXA, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 ),
        new ReferenceIndexAttribute( typeof( ArrayPtg ), 3 )
		  };
      RegisterFunction( "MINA", ExcelFunction.MINA, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "STDEVPA", ExcelFunction.STDEVPA, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "VARPA", ExcelFunction.VARPA, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "STDEVA", ExcelFunction.STDEVA, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {
		    new ReferenceIndexAttribute( typeof( RefPtg ), 1 )
		  };
      RegisterFunction( "VARA", ExcelFunction.VARA, attributes, -1 );

      attributes = new ReferenceIndexAttribute[]
		  {

		  };
      RegisterFunction( "NONE", ExcelFunction.NONE, attributes, -1 );


      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("DAYS", ExcelFunction.DAYS, attributes, 2);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("ISOWEEKNUM", ExcelFunction.ISOWEEKNUM, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("BITAND", ExcelFunction.BITAND, attributes, 2);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("BITLSHIFT", ExcelFunction.BITLSHIFT, attributes, 2);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("BITOR", ExcelFunction.BITOR, attributes, 2);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("BITRSHIFT", ExcelFunction.BITRSHIFT, attributes, 2);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("BITXOR", ExcelFunction.BITXOR, attributes, 2);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("IMCOSH", ExcelFunction.IMCOSH, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("IMCOT", ExcelFunction.IMCOT, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("IMCSC", ExcelFunction.IMCSC, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("IMCSCH", ExcelFunction.IMCSCH, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("IMSEC", ExcelFunction.IMSEC, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("IMSECH", ExcelFunction.IMSECH, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("IMSINH", ExcelFunction.IMSINH, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("IMTAN", ExcelFunction.IMTAN, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("PDURATION", ExcelFunction.PDURATION, attributes, 3);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("RRI", ExcelFunction.RRI, attributes, 3);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("ISFORMULA", ExcelFunction.ISFORMULA, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("SHEET", ExcelFunction.SHEET, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("SHEETS", ExcelFunction.SHEETS, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("IFNA", ExcelFunction.IFNA, attributes, 2);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("XOR", ExcelFunction.XOR, attributes, -1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("FORMULATEXT", ExcelFunction.FORMULATEXT, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("ACOT", ExcelFunction.ACOT, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("ACOTH", ExcelFunction.ACOTH, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("ARABIC", ExcelFunction.ARABIC, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("BASE", ExcelFunction.BASE, attributes, 3);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("CEILING.MATH", ExcelFunction.CEILING_MATH, attributes, 3);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("COMBINA", ExcelFunction.COMBINA, attributes, 2);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("COT", ExcelFunction.COT, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("COTH", ExcelFunction.COTH, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("CSC", ExcelFunction.CSC, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("CSCH", ExcelFunction.CSCH, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("DECIMAL", ExcelFunction.DECIMAL, attributes, 2);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("FLOOR.MATH", ExcelFunction.FLOOR_MATH, attributes, 3);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("ISO.CEILING", ExcelFunction.ISO_CEILING, attributes, 2);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("MUNIT", ExcelFunction.MUNIT, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("SEC", ExcelFunction.SEC, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("SECH", ExcelFunction.SECH, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("BINOM.DIST.RANGE", ExcelFunction.BINOM_DIST_RANGE, attributes, 4);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("GAMMA", ExcelFunction.GAMMA, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("GAUSS", ExcelFunction.GAUSS, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("PERMUTATIONA", ExcelFunction.PERMUTATIONA, attributes, 2);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("PHI", ExcelFunction.PHI, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("SKEW.P", ExcelFunction.SKEW_P, attributes, -1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("NUMBERVALUE", ExcelFunction.NUMBERVALUE, attributes, 3);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("UNICHAR", ExcelFunction.UNICHAR, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("UNICODE", ExcelFunction.UNICODE, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("ENCODEURL", ExcelFunction.ENCODEURL, attributes, 1);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("FILTERXML", ExcelFunction.FILTERXML, attributes, 2);

      attributes = new ReferenceIndexAttribute[]
            {
                new ReferenceIndexAttribute(typeof(RefPtg),2)
            };
      RegisterFunction("WEBSERVICE", ExcelFunction.WEBSERVICE, attributes, 1);
        //Excel 2013

    }
    /// <summary>
    /// Fills information about all known error names.
    /// </summary>
    private static void FillErrorNames()
    {
      //Assembly curAssembly = Assembly.GetAssembly( typeof( FormulaUtil ) );
      Type[] allTypes = ApplicationImpl.AssemblyTypes;//curAssembly.GetTypes();
      
      for( int i = 0, len = allTypes.Length; i < len; i++ )
      {
        AddErrorNames( allTypes[ i ] );
      }
    }
    /// <summary>
    /// Add information about all supported errors from specified type.
    /// </summary>
    /// <param name="type">Type that supports some errors.</param>
    private static void AddErrorNames( Type type )
    {
      object[] attributes = type.GetCustomAttributes( 
        typeof( ErrorCodeAttribute ), false );

      for( int i = 0, len = attributes.Length; i < len; i++ )
      {
        ErrorCodeAttribute attribute =  attributes[ i ] as ErrorCodeAttribute;
        ConstructorInfo constructor = type.GetConstructor( new Type[]{ typeof( string ) } );

        ErrorNameToConstructor.Add( attribute.StringValue, constructor );
        s_hashErrorCodeToName.Add( attribute.ErrorCode, attribute.StringValue );
        s_hashNameToErrorCode.Add( attribute.StringValue, attribute.ErrorCode );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    private void FillPriorities()
    {
      int iGroupsCount = m_arrOperationGroups.Length;
      for( int i = 0; i < iGroupsCount; i++ )
      {
        for( int j = 0, lenJ = m_arrOperationGroups[ i ].Length; j < lenJ; j++ )
        {
          //m_hashOperationPriority[ m_arrOperationGroups[ i ][ j ] ] = i;
          m_arrAllOperations[ m_arrOperationGroups[ i ][ j ] ] = i;
        }
      }

      int iLen = 0;
      int iAddLen = 0;

      string[] arrPrevious = null;

      for( int i = iGroupsCount - 1; i >= 0; i-- )
      {
        iAddLen = m_arrOperationGroups[ i ].Length;
        iLen += iAddLen;
        string[] arrCurrent = new string[ iLen ];

        if( i < iGroupsCount - 1 )
        {
          arrPrevious.CopyTo( arrCurrent, iAddLen );
        }

        m_arrOperationGroups[ i ].CopyTo( arrCurrent, 0 );
        m_arrOperationsWithPriority[ i ] = GetSortedList( arrCurrent );
        arrPrevious = arrCurrent;
      }
    }

    /// <summary>
    /// Converts string array into sorted list.
    /// </summary>
    /// <param name="arrStrings">Array to convert.</param>
    /// <returns>Created sorted list.</returns>
    private static StorageType GetSortedList( string[] arrStrings )
    {
      if( arrStrings == null )
        throw new ArgumentNullException( "arrStrings" );

      int iCount = arrStrings.Length;
      StorageType result = new StorageType(
#if ( WINRT )
          StringComparer.CurrentCulture,
#elif  (SILVERLIGHT || WP)  
          StringComparer.Create( CultureInfo.CurrentCulture, false ),
#else
        new StringComparer(),
#endif
        iCount );

      for( int i = 0; i < iCount; i++ )
      {
        result.Add( arrStrings[ i ], null );
      }

      return result;
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Converts string to shared formula token array.
    /// </summary>
    /// <param name="strFormula">Represents formula string.</param>
    /// <param name="iFirstRow">Represents first row from cells range. One-based.</param>
    /// <param name="iFirstColumn">Represents first column from cells range. One-based.</param>
    /// <param name="sheet">Parent worksheet object.</param>
    /// <returns>Shared formula tokens.</returns>
    public Ptg[]   ParseSharedString( string strFormula, int iFirstRow, int iFirstColumn, IWorksheet sheet )
    {
      Ptg[] arrResult = ParseString( strFormula, sheet, null );   
      IWorkbook book = sheet.Workbook;
      return ConvertTokensToShared( arrResult, iFirstRow, iFirstColumn, book );
    }
    public Ptg[] ConvertTokensToShared( Ptg[] tokens, int row, int column, IWorkbook book )
    {
      if( tokens != null )
      {
        int iCount = tokens.Length;
        for( int i = 0; i < iCount; i++ )
        {
            IReference reference = tokens[i] as IReference;
            if (reference == null)
            {
                tokens[i] = tokens[i].ConvertPtgToNPtg(book, row - 1, column - 1);
            }
        }
      }

      return tokens;
    }
    /// <summary>
    /// Converts string to token array.
    /// </summary>
    /// <param name="strFormula">
    /// String that should be parsed into Ptg array.
    /// </param>
    /// <returns>Token array representing specified string.</returns>
    /// <exception cref="System.ArgumentException">
    /// When any error occurs in the specified formula string.
    /// </exception>
    public Ptg[]   ParseString( string strFormula )
    {
      return ParseString( strFormula, null, null );
    }
    /// <summary>
    /// Converts string to token array. Used when copying worksheets into another workbook.
    /// </summary>
    /// <param name="strFormula">
    /// String that should be parsed into Ptg array.
    /// </param>
    /// <param name="sheet">Parent sheet.</param>
    /// <param name="hashWorksheetNames">
    /// Dictionary that contains old name of the worksheet as
    /// a key and new name of the worksheet as value.
    /// </param>
    /// <returns>Token array representing specified string.</returns>
    /// <exception cref="System.ArgumentException">
    /// When any error occurs in the specified formula string.
    /// </exception>
    public Ptg[]   ParseString( string strFormula, IWorksheet sheet,
      Dictionary<string, string> hashWorksheetNames )
    {
      return ParseString( strFormula, sheet, null, 0, hashWorksheetNames,
        ExcelParseFormulaOptions.RootLevel, 0, 0 );
    }
    /// <summary>
    /// Converts string to token array. Used when copying worksheets into another workbook.
    /// </summary>
    /// <param name="strFormula">
    /// String that should be parsed into Ptg array.
    /// </param>
    /// <param name="sheet">Parent sheet.</param>
    /// <param name="hashWorksheetNames">
    /// Dictionary that contains old name of the worksheet as
    /// a key and new name of the worksheet as value.
    /// </param>
    /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
    /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation is used.</param>
    /// <returns>Token array representing specified string.</returns>
    /// <exception cref="System.ArgumentException">
    /// When any error occurs in the specified formula string.
    /// </exception>
    public Ptg[]   ParseString( string strFormula, IWorksheet sheet,
      Dictionary<string, string> hashWorksheetNames, int iCellRow, int iCellColumn, bool bR1C1 )
    {
      ExcelParseFormulaOptions options = bR1C1
        ? ExcelParseFormulaOptions.RootLevel | ExcelParseFormulaOptions.UseR1C1
        : ExcelParseFormulaOptions.RootLevel;

      //return ParseString( strFormula, sheet, null, 0, hashWorksheetNames,
      //  options, iCellRow, iCellColumn );

      // TODO: uncomment and finish formula parsing functionality.
      ParseParameters arguments = new ParseParameters( sheet, hashWorksheetNames,
        bR1C1, iCellRow, iCellColumn, this, m_book );

      m_parser.Parse( strFormula, null, 0, options, arguments );
      return m_parser.Tokens.ToArray();
    }
    /// <summary>
    /// Converts string to token array.
    /// </summary>
    /// <param name="strFormula">String that should be parsed into Ptg array.</param>
    /// <param name="sheet">Worksheet that contains formula.</param>
    /// <param name="indexes">Token indexes, indicates whether to use reference token, value token, or array token.</param>
    /// <param name="i">Index in string.</param>
    /// <param name="hashWorksheetNames">Hash table with worksheet names.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
    /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
    /// <returns>Parsed formula.</returns>
    /// <exception cref="System.ArgumentException">
    /// When formula string is empty or if there is another error in formula string
    /// or when string consists only of %.
    /// </exception>
    public Ptg[]   ParseString( string strFormula, IWorksheet sheet,
      Dictionary<Type, ReferenceIndexAttribute> indexes, int i, Dictionary<string, string> hashWorksheetNames,
      ExcelParseFormulaOptions options, int iCellRow, int iCellColumn )
    {
      // TODO: uncomment and finish formula parsing functionality.
      bool bR1C1 = ( options & ExcelParseFormulaOptions.UseR1C1 ) != 0;
      ParseParameters arguments = new ParseParameters( sheet, hashWorksheetNames,
        bR1C1, iCellRow, iCellColumn, this, m_book );

      m_parser.Parse( strFormula, indexes, i, options, arguments );
      return m_parser.Tokens.ToArray();
    }
    /// <summary>
    /// Returns operand that is placed before unary operation.
    /// </summary>
    /// <param name="strFormula">
    /// Formula string that contains operand and unary operation.
    /// </param>
    /// <param name="OpIndex">
    /// Index of unary operation in the string.
    /// </param>
    /// <returns>Left operand of the specified unary operation.</returns>
    public string  GetLeftUnaryOperand( string strFormula, int OpIndex )
    {
      return GetOperand( strFormula, OpIndex, m_arrAllOperations, true );
    }
    /// <summary>
    /// Returns operand that is placed after unary operation.
    /// </summary>
    /// <param name="strFormula">
    /// Formula string that contains operation and operand.
    /// </param>
    /// <param name="OpIndex">Index of the operation.</param>
    /// <returns>Right operand of the specified unary operation.</returns>
    public string  GetRightUnaryOperand( string strFormula, int OpIndex )
    {
      string strResult = GetOperand( strFormula, OpIndex, m_arrAllOperations, false );
      int iLength = strResult.Length;

      if( iLength > 0 && strResult[ iLength - 1 ] == '%' )
      {
        strResult = '%' + strResult.Substring( 0, iLength - 1 );
      }

      return strResult;
    }
    /// <summary>
    /// Returns right operand for binary operation.
    /// </summary>
    /// <param name="strFormula">Formula string.</param>
    /// <param name="iFirstChar">Index of first operand character.</param>
    /// <param name="operation">String with operation.</param>
    /// <returns>Right operand for the specified operation.</returns>
    public string  GetRightBinaryOperand( string strFormula, int iFirstChar, string operation )
    {
      int iPriority = ( int )m_arrAllOperations[ operation ];

      return GetOperand( strFormula, iFirstChar - 1, m_arrOperationsWithPriority[ iPriority ], false );
    }
    /// <summary>
    /// Extracts function operand from formula string.
    /// </summary>
    /// <param name="strFormula">Formula string to extract operand from.</param>
    /// <param name="iFirstChar">First character of the operand.</param>
    /// <returns>Extracted operand.</returns>
    public string  GetFunctionOperand( string strFormula, int iFirstChar )
    {
      StorageType listSeparator = GetSortedList( new string[]{ OperandsSeparator } );
      return GetOperand( strFormula, iFirstChar, listSeparator, false );
    }
    /// <summary>
    /// This method converts FormulaRecord to its string representation.
    /// </summary>
    /// <param name="formula">FormulaRecord that will be parsed.</param>
    /// <returns>String representation of the specified formula.</returns>
    [ CLSCompliant( false ) ]
    public string  ParseFormulaRecord( FormulaRecord formula )
    {
      return ParseFormulaRecord( formula, false );
    }
    /// <summary>
    /// This method converts FormulaRecord to its string representation.
    /// </summary>
    /// <param name="formula">FormulaRecord that will be parsed.</param>
    /// <param name="bR1C1">Indicates whether formula must be parsed using R1C1 notation.</param>
    /// <returns>String representation of the specified formula.</returns>
    [ CLSCompliant( false ) ]
    public string  ParseFormulaRecord( FormulaRecord formula, bool bR1C1 )
    {
      Ptg[] ptgs = formula.ParsedExpression;
      return ParsePtgArray( ptgs, formula.Row, formula.Column, bR1C1, false );
    }
    /// <summary>
    /// Parses shared formula.
    /// </summary>
    /// <param name="sharedFormula">Formula to parse.</param>
    /// <returns>String representation of the specified formula.</returns>
    [ CLSCompliant( false ) ]
    public string  ParseSharedFormula( ISharedFormula sharedFormula )
    {
      Ptg[] ptgs = sharedFormula.Formula;
      return ParsePtgArray( ptgs, 0, 0, false, false );
    }
    /// <summary>
    /// Parses shared formula.
    /// </summary>
    /// <param name="sharedFormula">Formula to parse.</param>
    /// <param name="row">Zero-based row index of the cell with shared formula.</param>
    /// <param name="col">Zero-based column index of the cell with shared formula.</param>
    /// <returns>String representation of the specified formula.</returns>
    [ CLSCompliant( false ) ]
    public string  ParseSharedFormula( ISharedFormula sharedFormula, int row, int col )
    {
      return ParseSharedFormula( sharedFormula, row, col, false, false );
    }
    /// <summary>
    /// Parses shared formula.
    /// </summary>
    /// <param name="sharedFormula">Formula to parse.</param>
    /// <param name="row">Zero-based row index of the cell with shared formula.</param>
    /// <param name="col">Zero-based column index of the cell with shared formula.</param>
    /// <param name="bR1C1">Indicates whether formula must be parsed using R1C1 notation.</param>
    /// <returns>String representation of the specified formula.</returns>
    [ CLSCompliant( false ) ]
    public string  ParseSharedFormula( ISharedFormula sharedFormula, int row, int col, bool bR1C1, bool isForSerialization )
    {
      Ptg[] ptgs = sharedFormula.Formula;
      return ParsePtgArray( ptgs, row, col, bR1C1, isForSerialization );
    }
    /// <summary>
    /// Converts array of tokens into string.
    /// </summary>
    /// <param name="ptgs">Tokens to convert.</param>
    /// <returns>String representation of the specified tokens array.</returns>
    public string  ParsePtgArray( Ptg[] ptgs )
    {
      if( ptgs == null ) return null;

      return ParsePtgArray( ptgs, 0, 0, false, false );
    }
    /// <summary>
    /// This method converts array of Ptg to its string representation.
    /// </summary>
    /// <param name="ptgs">Ptg array that will be parsed.</param>
    /// <param name="row">First row to convert.</param>
    /// <param name="col">First column to convert.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation must be used.</param>
    /// <returns>String representation of the specified Ptg array.</returns>
    public string  ParsePtgArray( Ptg[] ptgs, int row, int col, bool bR1C1, bool isForSerialization )
    {
      return ParsePtgArray( ptgs, row, col, bR1C1, null, isForSerialization );
    }
    /// <summary>
    /// This method converts array of Ptg to its string representation.
    /// </summary>
    /// <param name="ptgs">Ptg array that will be parsed.</param>
    /// <param name="row">First row to convert.</param>
    /// <param name="col">First column to convert.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation must be used.</param>
    /// <param name="numberInfo">Represents current number info, can be null.</param>
    /// <returns>String representation of the specified Ptg array.</returns>
    public string ParsePtgArray( Ptg[] ptgs, int row, int col, bool bR1C1, NumberFormatInfo numberInfo,
      bool isForSerialization)
    {
      return ParsePtgArray( ptgs, row, col, bR1C1, numberInfo, false, isForSerialization, null );
    }
    /// <summary>
    /// This method converts array of Ptg to its string representation.
    /// </summary>
    /// <param name="ptgs">Ptg array that will be parsed.</param>
    /// <param name="row">First row to convert.</param>
    /// <param name="col">First column to convert.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation must be used.</param>
    /// <param name="numberInfo">Represents current number info, can be null.</param>
    /// <param name="bRemoveSheetNames">Indicates whether to remove worksheet name from 3d tokens.</param>
    /// <returns>String representation of the specified Ptg array.</returns>
    public string  ParsePtgArray( Ptg[] ptgs, int row, int col, bool bR1C1,
      NumberFormatInfo numberInfo, bool bRemoveSheetNames, bool isForSerialization, IWorksheet sheet )
    {
      if( ptgs == null )
        return null;

      if( numberInfo == null )
        numberInfo = m_numberFormat;

      ptgs = SkipUnnecessaryTokens( ptgs );
      string result = string.Empty;
      int ptgCount = ptgs.Length;
      Stack<object> stack = new Stack<object>();

      for( int i = 0, iLen = ptgs.Length; i < iLen; i++ )
      {
        Ptg token = ptgs[ i ];

        if( !token.IsOperation )
        {
          string strTokenValue;
          ISheetReference token3D = token as ISheetReference;

          if( bRemoveSheetNames && token3D != null )
          {
            strTokenValue = token3D.BaseToString( this, row, col, bR1C1 );
          }
          else
          {
            strTokenValue = token.ToString( this, row, col, bR1C1, numberInfo, isForSerialization, sheet );
          }

          //stack.Push( token.ToString( m_book, row, col, bR1C1, numberInfo ) );
          FormulaUtil.PushOperandToStack( stack, strTokenValue );
          continue;
        }

        OperationPtg optg = ( OperationPtg )token;
        optg.PushResultToStack( this, stack, isForSerialization );
      }

      if( stack.Count != 0 )
        result += stack.Pop().ToString();

      return result;
        }
        /// <summary>
        /// Checks the formula version.
        /// </summary>
        /// <param name="ptgs">The PTGS.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public void CheckFormulaVersion(Ptg[] ptgs)
        {
            if (ptgs == null)
                return;

          for( int i = 0, len = ptgs.Length; i < len; i++ )
          {
            Ptg token = ptgs[ i ];
            FunctionPtg function = token as FunctionPtg;

            if( function != null )
            {
                if (IsExcel2010Function(function.FunctionIndex) && m_book.Version != ExcelVersion.Excel2010 && m_book.Version != ExcelVersion.Excel2013)
              {
                throw new NotSupportedException( "The formula is not supported in this Version" );
              }
            }
          }
    }
    /// <summary>
    /// Splits array. Each string in result represents single row of the array.
    /// </summary>
    /// <param name="strFormula">Array string to split.</param>
    /// <param name="strSeparator">Separator</param>
    /// <returns>Splitted array.</returns>
    public List<string> SplitArray( string strFormula, string strSeparator )
    {
      if( strFormula == null )
        throw new ArgumentNullException( "strFormula" );

      if( strFormula.Length == 0 )
        throw new ArgumentException( "strFormula - string cannot be empty" );

      List<string> arrRows = new List<string>();
      StorageType arrArrayRowSeparator = new StorageType( 1 );
      arrArrayRowSeparator.Add( strSeparator, null );

      int index = -1;
      int iLength = strFormula.Length;

      while( index < iLength )
      {
        string strCurRow = GetOperand( strFormula, index, arrArrayRowSeparator, false );
        index += strCurRow.Length + 1;
        arrRows.Add( strCurRow );
      }
      
      return arrRows;
    }

    /// <summary>
    /// Updates index of the name.
    /// </summary>
    /// <param name="ptg">Token to update.</param>
    /// <param name="arrNewIndex">Array with new named ranges indexes.</param>
    public bool UpdateNameIndex( Ptg ptg, int[] arrNewIndex )
    {
      if( ptg == null ) return false;

      bool bResult = false;

      if( IndexOf( FormulaUtil.NameCodes, ptg.TokenCode ) != -1 )
      {
        NamePtg name = ( NamePtg )ptg;

        int iOldIndex = name.ExternNameIndex - 1;
        int iNewIndex = arrNewIndex[ iOldIndex ];

        if( iOldIndex != iNewIndex )
        {
          name.ExternNameIndex = ( ushort )( iNewIndex + 1 );
          bResult = true;
        }
      }
      else if( IndexOf( FormulaUtil.NameXCodes, ptg.TokenCode ) != -1 )
      {
        NameXPtg nameX = ( NameXPtg )ptg;

        if( m_book.IsLocalReference( nameX.RefIndex ) )
        {
          int iOldIndex = nameX.NameIndex - 1;
          int iNewIndex = arrNewIndex[ iOldIndex ];

          if( iNewIndex != iOldIndex )
          {
            nameX.NameIndex = ( ushort )( iNewIndex + 1 );
            bResult = true;
          }
        }
      }

      return bResult;
    }
    /// <summary>
    /// Updates index of the name.
    /// </summary>
    /// <param name="ptg">Token to update.</param>
    /// <param name="dicNewIndex">Dictionary with new named ranges indexes.</param>
    public bool UpdateNameIndex( Ptg ptg, IDictionary<int, int> dicNewIndex )
    {
      if( ptg == null )
        throw new ArgumentNullException( "ptg" );

      bool bResult = false;

      if( IndexOf( FormulaUtil.NameCodes, ptg.TokenCode ) != -1 )
      {
        NamePtg name = ( NamePtg )ptg;

        int iOldIndex = name.ExternNameIndex - 1;
        int iNewIndex = dicNewIndex.ContainsKey( iOldIndex )
          ? dicNewIndex[ iOldIndex ]
          : iOldIndex;

        if( iOldIndex != iNewIndex )
        {
          name.ExternNameIndex = ( ushort )( iNewIndex + 1 );
          bResult = true;
        }
      }
      else if( IndexOf( FormulaUtil.NameXCodes, ptg.TokenCode ) != -1 )
      {
        NameXPtg nameX = ( NameXPtg )ptg;

        if( m_book.IsLocalReference( nameX.RefIndex ) )
        {
          int iOldIndex = nameX.NameIndex - 1;
          int iNewIndex = dicNewIndex.ContainsKey( iOldIndex )
            ? dicNewIndex[ iOldIndex ]
            : iOldIndex;

          if( iNewIndex != iOldIndex )
          {
            nameX.NameIndex = ( ushort )( iNewIndex + 1 );
            bResult = true;
          }
        }
      }

      return bResult;
    }
    /// <summary>
    /// Updates name indexes.
    /// </summary>
    /// <param name="arrExpression">Parsed expression to update.</param>
    /// <param name="dicNewIndex">Dictionary with new indexes.</param>
    /// <returns>True if at least one of references to named ranges was updated.</returns>
    public bool UpdateNameIndex( Ptg[] arrExpression, IDictionary<int, int> dicNewIndex )
    {
      if( arrExpression == null ) return false;

      if( dicNewIndex == null )
        throw new ArgumentNullException( "dicNewIndex" );

      bool bResult = false;

      for( int i = 0, len = arrExpression.Length; i < len; i++ )
      {
        bResult |= UpdateNameIndex( arrExpression[ i ], dicNewIndex );
      }

      return bResult;
    }
    /// <summary>
    /// Updates name indexes.
    /// </summary>
    /// <param name="arrExpression">Parsed expression to update.</param>
    /// <param name="arrNewIndex">Array with new indexes.</param>
    /// <returns>True if at least one of references to named ranges was updated.</returns>
    public bool UpdateNameIndex( Ptg[] arrExpression, int[] arrNewIndex )
    {
      if( arrExpression == null )return false;

      if( arrNewIndex == null )
        throw new ArgumentNullException( "arrNewIndex" );

      bool bResult = false;

      for( int i = 0, len = arrExpression.Length; i < len; i++ )
      {
        bResult |= UpdateNameIndex( arrExpression[ i ], arrNewIndex );
      }

      return bResult;
    }
    /// <summary>
    /// Sets separators.
    /// </summary>
    /// <param name="operandsSeparator">Operand separator to set.</param>
    /// <param name="arrayRowsSeparator">Array rows separator to set.</param>
    public void SetSeparators( char operandsSeparator, char arrayRowsSeparator )
    {
//      if( strOperandsSeparator == null )
//        throw new ArgumentNullException( "strOperandsSeparator" );
//
//      if( strOperandsSeparator.Length == 0 )
//        throw new ArgumentException( "strOperandsSeparator - string cannot be empty" );
//
//      if( strArrayRowsSeparator == null )
//        throw new ArgumentNullException( "strArrayRowsSeparator" );
//
//      if( strArrayRowsSeparator.Length == 0 )
//        throw new ArgumentException( "strArrayRowsSeparator - string cannot be empty" );
      string strOperandsSeparator = operandsSeparator.ToString();
      string strArrayRowsSeparator = arrayRowsSeparator.ToString();

      if( strOperandsSeparator == m_strOperandsSeparator
        && strArrayRowsSeparator == m_strArrayRowSeparator )
      {
        return;
      }

      // Make substitution in m_arrOperationGroups - unnecessary
      // Make substitution in m_arrAllOperations

      string[] arrOldSeparators = new string[]{ m_strOperandsSeparator, m_strArrayRowSeparator };
      string[] arrNewSeparators = new string[]{ strOperandsSeparator, strArrayRowsSeparator };
      ReplaceInDictionary( m_arrAllOperations, arrOldSeparators, arrNewSeparators );

      // Make substitution in m_arrOperationsWithPriority
      for( int i = 0, len = m_arrOperationsWithPriority.Length; i < len; i++ )
      {
        StorageType list = m_arrOperationsWithPriority[ i ];
        ReplaceInDictionary( list, arrOldSeparators, arrNewSeparators );
      }

      // Now we can assign new separators.
      m_strOperandsSeparator = strOperandsSeparator;
      m_strArrayRowSeparator = strArrayRowsSeparator;

      m_parser.SetSeparators( operandsSeparator, arrayRowsSeparator );
    }
    /// <summary>
    /// Replaces keys in the list.
    /// </summary>
    /// <param name="list">List to replace in.</param>
    /// <param name="arrOldKey">Old keys to replace.</param>
    /// <param name="arrNewKey">New keys to replace.</param>
    private void ReplaceInDictionary( IDictionary list, string[] arrOldKey, string[] arrNewKey )
    {
      if( list == null )
        throw new ArgumentNullException( "list" );

      if( arrOldKey == null )
        throw new ArgumentNullException( "arrOldKey" );

      if( arrNewKey == null )
        throw new ArgumentNullException( "arrNewKey" );

      int iCount = arrOldKey.Length;

      if( iCount != arrNewKey.Length )
        throw new ArgumentException( "arrOldKey and arrNewKey do not correspond each other" );

      object[] arrValue = new object[ iCount ];
      bool[] arrContains = new bool[ iCount ];

      for( int i = 0; i < iCount; i++ )
      {
        string value = arrOldKey[ i ];

        bool bContains = arrContains[ i ] = list.Contains( value );

        if( bContains )
        {
          arrValue[ i ] = list[ value ];
          list.Remove( value );
        }
      }

      for( int i = 0; i < iCount; i++ )
      {
        if( arrContains[ i ] )
        {
          list.Add( arrNewKey[ i ], arrValue[ i ] );
        }
      }
    }
    /// <summary>
    /// Marks used references.
    /// </summary>
    /// <param name="tokens">Tokens to get used references from.</param>
    /// <param name="usedItems">Array to mark used references in.</param>
    public static void MarkUsedReferences( Ptg[] tokens, bool[] usedItems )
    {
      if( tokens == null )
        return;

      for( int i = 0, len = tokens.Length; i < len; i++ )
      {
        IReference token = tokens[ i ] as IReference;

        if( token != null )
        {
          usedItems[ token.RefIndex ] = true;
        }
      }
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="tokens">Tokens to get used references from.</param>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public static bool UpdateReferenceIndexes( Ptg[] tokens, int[] arrUpdatedIndexes )
    {
      bool bResult = false;

      if( tokens != null )
      {
        for( int i = 0, len = tokens.Length; i < len; i++ )
        {
          IReference token = tokens[ i ] as IReference;

          if( token != null )
          {
            int iRefIndex = token.RefIndex;
            int iNewRefIndex = arrUpdatedIndexes[ iRefIndex ];
            //usedItems[ iRefIndex ] = true;
            token.RefIndex = ( ushort )iNewRefIndex;
            bResult = true;
          }
        }
      }

      return bResult;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Converts string that represents constant value to Ptg.
    /// </summary>
    /// <param name="strFormula">String that represents constant.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="hashWorksheetNames">Dictionary with worksheet names</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Token corresponding to the specified string.</returns>
    private Ptg CreateConstantPtg( string strFormula, IWorksheet sheet,
      Dictionary<string, string> hashWorksheetNames, ExcelParseFormulaOptions options )
    {
      return CreateConstantPtg( strFormula, sheet, null, 0, hashWorksheetNames,
        options, 0, 0 );
    }
    /// <summary>
    /// Converts string that represents constant value to Ptg.
    /// </summary>
    /// <param name="strFormula">String that contains constant.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index 
    /// that should be used at special function position.
    /// </param>
    /// <param name="i">Parameter position.</param>
    /// <param name="hashWorksheetNames"></param>
    /// <param name="options"></param>
    /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
    /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
    /// <returns>Newly created token.</returns>
    /// <exception cref="System.ArgumentException">
    /// When specified string is not constant formula token.
    /// </exception>
    private Ptg     CreateConstantPtg( string strFormula, IWorksheet sheet,
      Dictionary<Type, ReferenceIndexAttribute> indexes, int i, Dictionary<string, string> hashWorksheetNames,
      ExcelParseFormulaOptions options, int iCellRow, int iCellColumn )
    {
      // String can be empty.
      FormulaToken token;
      bool bR1C1 = ( ( options & ExcelParseFormulaOptions.UseR1C1 ) != 0 );

      if( strFormula.Length == 0 )
      {
        return CreatePtg( FormulaToken.tMissingArgument );
      }

      // Check if it is string ( starts with " ).
      if( strFormula[ 0 ] == '"' )
      {
        return CreatePtg( FormulaToken.tStringConstant, 
          strFormula.Substring( 1, strFormula.Length - 2 ) );
      }

      // Check if it is array?
      if( strFormula[ 0 ] == '{' )
      {
        token = ArrayPtg.IndexToCode( GetIndex( typeof( ArrayPtg ), DEF_TYPE_ARRAY,
          indexes, i, options ) );

        return CreatePtg( token, strFormula, this );
      }

      string strCol1, strCol2, strRow1, strRow2;
      // Check if it is cell or cell range. 
      if( IsCellRange( strFormula, bR1C1, out strRow1, out strCol1, out strRow2, out strCol2 ) )
      {
        token = AreaPtg.IndexToCode( GetIndex( typeof( AreaPtg ), DEF_TYPE_REF,
          indexes, i, options ) );

        return CreatePtg( token, iCellRow, iCellColumn, strRow1, strCol1, strRow2,
          strCol2, bR1C1, m_book );
      }

      // Check if it is cell.
      if( IsCell( strFormula, bR1C1, out strRow1, out strCol1 ) )
      {
        token = RefPtg.IndexToCode( GetIndex( typeof( RefPtg ), DEF_TYPE_REF,
          indexes, i, options ) );

        return CreatePtg( token, iCellRow, iCellColumn, strRow1, strCol1, bR1C1 );//strFormula );
      }

      // Check if it is 3d range.
      string sheetName;

      if( IsCellRange3D( strFormula, bR1C1, out sheetName, out strRow1,
        out strCol1, out strRow2, out strCol2 ) )
      {
        token = Area3DPtg.IndexToCode( GetIndex( typeof( Area3DPtg ), DEF_TYPE_REF,
          indexes, i, options ) );

        if( hashWorksheetNames != null && hashWorksheetNames.ContainsKey( sheetName ) )
        {
          strFormula = strFormula.Replace( sheetName, hashWorksheetNames[ sheetName ] );
        }

        int iRefIndex = m_book.AddSheetReference( sheetName );
        return CreatePtg( token, iCellRow, iCellColumn, iRefIndex, strRow1,
          strCol1, strRow2, strCol2, bR1C1 );
      }

      // Check if it is 3d cell.
      if( IsCell3D( strFormula, bR1C1, out sheetName, out strRow1, out strCol1 ) )
      {
        token = Ref3DPtg.IndexToCode( GetIndex( typeof( Ref3DPtg ), DEF_TYPE_REF,
          indexes, i, options ) );
        
        if( hashWorksheetNames != null && hashWorksheetNames.ContainsKey( sheetName ) )
        {
          sheetName =hashWorksheetNames[ sheetName ];
        }

        int iRefIndex = m_book.AddSheetReference( sheetName );

        return CreatePtg( token, iCellRow, iCellColumn, iRefIndex, strRow1, strCol1, bR1C1 );
      }

      // Check if it is named range.
      if( IsNamedRange( strFormula, m_book, sheet ) )
      {
        //        if( parent.Names[ strFormula ].RefersToRange != null )
        //        {
        //          token = NameXPtg.IndexToCode( GetIndex( typeof( Ref3DPtg ), indexes, i ) );
        //        }
        //        else
      {
        token = NamePtg.IndexToCode( GetIndex( typeof( NamePtg ), DEF_TYPE_REF, indexes, i, options ) );
      }

        if( sheet != null )
        {
          return CreatePtg( token, strFormula, m_book, sheet );
        }
        else
        {
          return CreatePtg( token, strFormula, m_book );
        }
      }

      double dNumber;
      // Try to get int

      bool bResult = double.TryParse( strFormula, NumberStyles.Integer, null, out dNumber );

      if( bResult && dNumber <= ushort.MaxValue && dNumber >= ushort.MinValue )
      {
        ushort number = ( ushort )dNumber;
        return CreatePtg( FormulaToken.tInteger, strFormula );
      }

      // Try to get double.
      if( !bResult )
      {
        bResult = double.TryParse( strFormula, NumberStyles.Any, m_numberFormat, out dNumber );
      }

      if( bResult )
      {
        return CreatePtg( FormulaToken.tNumber, dNumber );
      }

      // Try to get boolean value.
      try
      {
        bool value = bool.Parse( strFormula );
        return CreatePtg( FormulaToken.tBoolean, strFormula );
      }
      catch( System.FormatException )
      {
      }
      
      if( !m_book.ThrowOnUnknownNames )
      {
        // Here we should add new extern name.
        m_book.Names.Add( strFormula );
        token = NamePtg.IndexToCode( GetIndex( typeof( NamePtg ), DEF_TYPE_REF, indexes, i, options ) );
        return CreatePtg( token, strFormula, m_book );
      }

      throw new ArgumentException( "Can't parse formula: " + strFormula );
      // Now let's return string constant.
      //return new StringConstantPtg( strFormula );
    }

    /// <summary>
    /// Normalizes sheet name.
    /// </summary>
    /// <param name="strSheetName"></param>
    /// <returns></returns>
    private static string NormalizeSheetName( string strSheetName )
    {
      if( strSheetName == null ) return null;

      int iLength = strSheetName.Length;

      if( iLength >= 2 && strSheetName[ 0 ] == DEF_SHEET_NAME_DELIM
        && DEF_SHEET_NAME_DELIM == strSheetName[ iLength - 1 ] )
        strSheetName = strSheetName.Substring( 1, iLength -2 );

      return strSheetName;
    }
    #endregion

    #region Public static methods
    /// <summary>
    /// Converts byte array to Ptg array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iLength">
    /// Number of bytes to parse (there can be array data 
    /// after of all tokens).
    /// </param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    /// <returns>
    /// String representation of the specified byte array that contains tokens.
    /// </returns>
    public static Ptg[]   ParseExpression( DataProvider provider, int iLength, ExcelVersion version )
    {
      List<Ptg> parsed = new List<Ptg>();

      for( int offset = 0; offset < iLength; )
      {
        parsed.Add( CreatePtg( provider, ref offset, version ) );
      }

      return parsed.ToArray();
    }

    /// <summary>
    /// Converts byte array to Ptg array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Start position in data array to parse from.</param>
    /// <param name="iExpressionLength">
    /// Number of bytes to parse (there can be array data 
    /// after of all tokens).
    /// </param>
    /// <param name="finalOffset">
    /// Receives offset of first byte after all tokens' data.
    /// </param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    /// <returns>Converted Ptg array.</returns>
    public static Ptg[]   ParseExpression( DataProvider provider, int offset,
      int iExpressionLength, out int finalOffset, ExcelVersion version )
    {
      List<Ptg> parsed = new List<Ptg>();
      int iEndOffset = offset + iExpressionLength;

      while( offset < iEndOffset )
      {
        parsed.Add( CreatePtg( provider, ref offset, version ) );
      }

      finalOffset = offset;

      for( int i = 0; i < parsed.Count; i++ )
      {
        if( parsed[ i ] is IAdditionalData )
        {
          finalOffset = ( parsed[ i ] as IAdditionalData ).ReadArray( provider, finalOffset );
        }
      }

      return parsed.ToArray();
    }

    /// <summary>
    /// Converts token array to corresponding byte array.
    /// </summary>
    /// <param name="tokens">Ptg array that will be converted to byte array.</param>
    /// <param name="version">Excel version that should be used to infill data.</param>
    /// <returns>Converted byte array</returns>
    public static byte[]  PtgArrayToByteArray( Ptg[] tokens, ExcelVersion version )
    {
      if( tokens == null ) return null;

      int Len;
      return PtgArrayToByteArray( tokens, out Len, version );
    }
    /// <summary>
    /// Converts token array to corresponding byte array.
    /// </summary>
    /// <param name="arrTokens">Ptg array that will be converted to byte array.</param>
    /// <param name="formulaLen">
    /// Length of formula without tArray data if there is tArray in Tokens.
    /// </param>
    /// <param name="version">Excel version that should be used to infill data.</param>
    /// <returns>Converted byte array.</returns>
    public static byte[]  PtgArrayToByteArray( Ptg[] arrTokens, out int formulaLen, ExcelVersion version )
    {
      if( arrTokens == null )
        throw new ArgumentNullException( "arrTokens" );

      BytesList array = new BytesList( true );
      int iLength = arrTokens.Length;

      for( int i = 0, len = iLength; i < len; i++ )
      {
        array.AddRange( arrTokens[ i ].ToByteArray( version ) );
      }

      formulaLen = array.Count;

      for( int i = 0; i < iLength; i++ )
      {
        if( arrTokens[ i ] is ArrayPtg )
        {
          BytesList list = ( arrTokens[ i ] as ArrayPtg ).GetListBytes();
          array.AddRange( list );
        }
      }
           
      return array.InnerBuffer;
    }
    /// <summary>
    /// Returns left operand for binary operation.
    /// </summary>
    /// <param name="strFormula">Formula string.</param>
    /// <param name="OpIndex">Index of operation.</param>
    /// <returns>Left operand for specified operation.</returns>
    public static string  GetLeftBinaryOperand( string strFormula, int OpIndex )
    {
      return GetOperand( strFormula, OpIndex, m_listPlusMinus, true );
    }
    /// <summary>
    /// Searches for position of corresponding bracket.
    /// </summary>
    /// <param name="strFormula">String to search.</param>
    /// <param name="BracketPos">Position of bracket( "(){}" ).</param>
    /// <returns>
    /// Position of corresponding bracket if there is one, otherwise -1.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When specified position does not contain bracket.
    /// </exception>
    public static int     FindCorrespondingBracket( string strFormula, int BracketPos )
    {
      int delta;
      char[] CurBrackets;

      if( IndexOf( OpenBrackets, strFormula[ BracketPos ] ) != - 1 )
      {
        delta = 1;
        CurBrackets = OpenBrackets;
      }
      else if( IndexOf( CloseBrackets, strFormula[ BracketPos ] ) != -1  )
      {
        delta = -1;
        CurBrackets = CloseBrackets;
      }
      else
      {
        throw new ArgumentOutOfRangeException( "Specified position is not a position of bracket" );
      }

      return FindCorrespondingBracket( strFormula, BracketPos, CurBrackets, delta );
    }

    /// <summary>
    /// Returns operand of the operation.
    /// </summary>
    /// <param name="strFormula">
    /// Formula string that contains operation and operands.
    /// </param>
    /// <param name="OpIndex">Index of the operation.</param>
    /// <param name="arrBreakStrings">Delimiters between operands.</param>
    /// <param name="IsLeft">
    /// Search direction (TRUE -right to left, 
    /// FALSE - left to right).
    /// </param>
    /// <returns>Operand defined by function parameters.</returns>
    /// <exception cref="System.ArgumentException">
    /// When a open bracket is found without a corresponding closing bracket.
    /// </exception>
    public static string  GetOperand( string strFormula, int OpIndex,
      StorageType arrBreakStrings, bool IsLeft )
    {
      if( strFormula == null )
        throw new ArgumentNullException( "strFormula" );

      if( arrBreakStrings == null )
        throw new ArgumentNullException( "arrBreakStrings" );

      char[] arrBrackets = IsLeft ? CloseBrackets : OpenBrackets;
      char[] arrBreakBrackets = IsLeft ? OpenBrackets : CloseBrackets;
      int iDelta = IsLeft ? -1 : 1;
      int i = OpIndex + iDelta;
      string strResult;

      if( !IsLeft && strFormula[ i ] == '#' )
      {
        strResult = GetErrorOperand( strFormula, i );
        i += strResult.Length;

        if( i >= strFormula.Length || IndexOf( strFormula, i, arrBreakStrings ) != -1 || IndexOf( arrBreakBrackets, strFormula[ i ] ) != -1 )
          return strResult;
      }

      int iFormulaLen = strFormula.Length;

      while( i >= 0 && i < iFormulaLen && IndexOf( PlusMinusArray, strFormula[ i ].ToString() ) != -1 )
      {
        i += iDelta;
      }

      for( ; i >= 0 && i < iFormulaLen; i += iDelta )
      {
        if( IndexOf( arrBrackets, strFormula[ i ] ) != -1 )
        {
          i = FindCorrespondingBracket( strFormula, i );
        }
        else if( IndexOf( arrBreakBrackets, strFormula[ i ] ) != -1 
          && FindCorrespondingBracket( strFormula, i ) != -1
          || IndexOf( strFormula, i, arrBreakStrings ) != -1 )
        {
          strResult = IsLeft
            ? strFormula.Substring( i + 1, OpIndex - i - 1 )
            : strFormula.Substring( OpIndex + 1, i - OpIndex - 1 );
          return strResult;
        }
      }

      return IsLeft
        ? strFormula.Substring( 0, OpIndex )
        : strFormula.Substring( OpIndex + 1 );
    }

    /// <summary>
    /// Registers function in internal collections.
    /// </summary>
    /// <param name="functionName">
    /// Name of the function that must be registered.
    /// </param>
    /// <param name="index">
    /// Index of the function that must be registered.
    /// </param>
    /// <param name="paramIndexes">
    /// Array of ReferenceIndexAttribute that contains information 
    /// about proper token index.
    /// </param>
    [ CLSCompliant( false ) ]
    public static void RegisterFunction( string functionName, ExcelFunction index, 
      ReferenceIndexAttribute[] paramIndexes )
    {
      RegisterFunction( functionName, index, paramIndexes, -1 );
    }
    /// <summary>
    /// Registers function in internal collections.
    /// </summary>
    /// <param name="functionName">
    /// Name of the function that must be registered.
    /// </param>
    /// <param name="index">
    /// Index of the function that must be registered.
    /// </param>
    /// <param name="paramIndexes">
    /// Array of ReferenceIndexAttribute that contains information 
    /// about proper token index.
    /// </param>
    /// <param name="paramCount">
    /// Number of parameters in the function,
    /// -1, for variable parameters.
    /// </param>
    [ CLSCompliant( false ) ]
    public static void RegisterFunction( string functionName, ExcelFunction index, 
      ReferenceIndexAttribute[] paramIndexes, int paramCount )
    {
      /*System.Text.StringBuilder builder = new System.Text.StringBuilder();
      builder.Append( "attributes = new ReferenceIndexAttribute[]\n\t\t{\n" );
      for( int i = 0, len = paramIndexes.Length; i < len; i++ )
      {
        ReferenceIndexAttribute attribute = paramIndexes[ i ];
        builder.Append( string.Format( "\t\t    new ReferenceIndexAttribute( typeof( {0} )", attribute.TargetType.Name ) );

        builder.Append( ", " );

        if( attribute.Count > 0 )
        {
          for( int j = 0, lenJ = attribute.Count; j < lenJ; j++ )
          {
            builder.Append( attribute[ j ] );

            if( j != lenJ - 1 )
              builder.Append( ",\n" );
          }
        }
        else
        {
          builder.Append( attribute.Index );
        }

        builder.Append( " )" );

        if( i != len - 1 )
          builder.Append( "," );
      }

      builder.Append( "\n\t\t  };" );

      Console.WriteLine( builder.ToString() );
      Console.WriteLine( "RegisterFunction( \"{0}\", ExcelFunction.{1}, attributes, {2} );\n", functionName, index, paramCount );
      */
      Dictionary<Type, ReferenceIndexAttribute> indexes = null;
        
      if( paramIndexes != null && paramIndexes.Length != 0 )
      {
        indexes = new Dictionary<Type, ReferenceIndexAttribute>();
      }

      for( int i = 0; i < paramIndexes.Length; i++ )
      {
        indexes.Add( paramIndexes[ i ].TargetType, paramIndexes[ i ] );
      }

      if( indexes == null )
      {
        indexes = new Dictionary<Type, ReferenceIndexAttribute>( 1 );
        indexes.Add( typeof( RefPtg ), new ReferenceIndexAttribute( DEF_REFERENCE_INDEX ) );
      }

      FunctionIdToIndex.Add( index, indexes );
      FunctionIdToAlias.Add( index, functionName );
      FunctionAliasToId.Add( functionName, index );
      
      if( paramCount != -1 )
      {
        FunctionIdToParamCount.Add( index, paramCount );
      }
    }
    /// <summary>
    /// Registers function in internal collections.
    /// </summary>
    /// <param name="functionName">
    /// Name of the function that must be registered.
    /// </param>
    /// <param name="index">
    /// Index of the function that must be registered.
    /// </param>
    /// <param name="paramCount">
    /// Number of parameter in the function,
    /// -1, for variable parameters number.
    /// </param>
    [ CLSCompliant( false ) ]
    public static void RegisterFunction( string functionName, ExcelFunction index, 
      int paramCount )
    {
      RegisterFunction( functionName, index, null, paramCount );
    }
    /// <summary>
    /// Registers function in internal collections.
    /// </summary>
    /// <param name="functionName">
    /// Name of the function that must be registered.
    /// </param>
    /// <param name="index">
    /// Index of the function that must be registered.
    /// </param>
    [ CLSCompliant( false ) ]
    public static void RegisterFunction( string functionName, ExcelFunction index )
    {
      RegisterFunction( functionName, index, -1 );
    }
    /// <summary>
    ///  This method raises the FormulaEvaluation event.
    /// </summary>
    /// <param name="sender">Range that caused FormulaEvaluation event.</param>
    /// <param name="e">Object that contains event arguments.</param>
    public static void RaiseFormulaEvaluation( object sender, EvaluateEventArgs e )
    {
      if( FormulaEvaluator != null )
      {
        FormulaEvaluator( sender, e );
      }
    }
    /// <summary>
    /// Registers token class (can be user defined).
    /// </summary>
    /// <param name="type">Token class that will be registered.</param>
    /// <exception cref="System.ArgumentException">
    /// When class is not derived from Ptg.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// When parameter type is NULL.
    /// </exception>
    public static void RegisterTokenClass( Type type )
    {
      if( type == null )
        throw new ArgumentNullException( "type" );

      if( type.IsSubclassOf( typeof( Ptg ) ) == false )
        throw new ArgumentException( "class must be derived from Ptg class", "type" );

#if ( WINRT )

      TokenAttribute[] allAttributes = (TokenAttribute[])type.GetTypeInfo().GetCustomAttributes<TokenAttribute>(false).ToArray<TokenAttribute>();
      
#else
      TokenAttribute[] allAttributes = (TokenAttribute[])type.GetCustomAttributes(typeof(TokenAttribute), false);
#endif

      if( allAttributes.Length == 0 )
        return;

      TokenConstructor constructor = new TokenConstructor( type );
      Ptg toAdd = constructor.CreatePtg();

      for( int i = 0; i < allAttributes.Length; i++ )
      {
        FormulaToken token = allAttributes[ i ].FormulaType;
        TokenCodeToConstructor.Add( allAttributes[ i ].FormulaType, constructor );
        s_hashTokenCodeToPtg.Add( token, toAdd );
      }
    }
    /// <summary>
    /// Registers new function alias.
    /// </summary>
    /// <param name="aliasName">New alias name.</param>
    /// <param name="functionIndex">Function index.</param>
    [ CLSCompliant( false ) ]
    public static void RegisterAdditionalAlias( string aliasName, ExcelFunction functionIndex )
    {
      if( aliasName == null )
        throw new ArgumentNullException( "aliasName" );

      if( aliasName.Length == 0 )
        throw new ArgumentException( "aliasName - string cannot be empty" );

      if( FunctionAliasToId.ContainsKey( aliasName ) )
        throw new ArgumentOutOfRangeException( "aliasName", "Alias name already exists." );

      FunctionAliasToId.Add( aliasName, functionIndex );
    }
    /// <summary>
    /// Updates index of the name.
    /// </summary>
    /// <param name="ptg">Ptg to Update</param>
    /// <param name="iOldIndex">Old index.</param>
    /// <param name="iNewIndex">New index.</param>
    public static void UpdateNameIndex( Ptg ptg, int iOldIndex, int iNewIndex )
    {
      if( ptg == null )
        throw new ArgumentNullException( "ptg" );

      if( IndexOf( FormulaUtil.NameCodes, ptg.TokenCode ) != -1 )
      {
        NamePtg name = ( NamePtg )ptg;

        if( name.ExternNameIndex - 1 == iOldIndex )
        {
          name.ExternNameIndex = ( ushort )( iNewIndex + 1 );
        }
      }
      else if( IndexOf( FormulaUtil.NameXCodes, ptg.TokenCode ) != -1 )
      {
        NameXPtg nameX = ( NameXPtg )ptg;
            
        if( nameX.NameIndex - 1 == iOldIndex )
        {
          nameX.NameIndex = ( ushort )( iNewIndex + 1 );
        }
      }
    }
    /// <summary>
    /// Searches for the specified object and returns the index of
    /// the first occurrence within the entire one-dimensional array.
    /// </summary>
    /// <param name="array">Array to search.</param>
    /// <param name="value">Value to locate in the array.</param>
    /// <returns>
    /// The index of the first occurrence of value within the entire array, if found;
    ///  otherwise, -1.</returns>
    public static int IndexOf( FormulaToken[] array, FormulaToken value )
    {
      if( array == null )
        throw new ArgumentNullException( "array" );

      for( int i = 0, len = array.Length; i < len; i++ )
      {
        if( array[ i ] == value ) return i;
      }

      return -1;
    }
    /// <summary>
    /// Searches for the specified object and returns the index of
    /// the first occurrence within the entire one-dimensional array.
    /// </summary>
    /// <param name="array">Array to search.</param>
    /// <param name="value">Value to locate in the array.</param>
    /// <returns>
    /// The index of the first occurrence of value within the entire array, if found;
    ///  otherwise, -1.</returns>
    public static int IndexOf( string[] array, string value )
    {
      if( array == null )
        throw new ArgumentNullException( "array" );

      for( int i = 0, len = array.Length; i < len; i++ )
      {
        if( array[ i ] == value ) return i;
      }

      return -1;
    }
    /// <summary>
    /// Searches for the specified object and returns the index of
    /// the first occurrence within the entire one-dimensional array.
    /// </summary>
    /// <param name="array">Array to search.</param>
    /// <param name="value">Value to locate in the array.</param>
    /// <returns>
    /// The index of the first occurrence of value within the entire array, if found;
    ///  otherwise, -1.</returns>
    [ CLSCompliant( false ) ]
    public static int IndexOf( ExcelFunction[] array, ExcelFunction value )
    {
      if( array == null )
        throw new ArgumentNullException( "array" );

      for( int i = 0, len = array.Length; i < len; i++ )
      {
        if( array[ i ] == value ) return i;
      }

      return -1;
    }
    /// <summary>
    /// Searches for the specified object and returns the index of
    /// the first occurrence within the entire one-dimensional array.
    /// </summary>
    /// <param name="array">Array to search.</param>
    /// <param name="value">Value to locate in the array.</param>
    /// <returns>
    /// The index of the first occurrence of value within the entire array, if found;
    ///  otherwise, -1.</returns>
    private static int IndexOf( char[] array, char value )
    {
      if( array == null )
        throw new ArgumentNullException( "array" );

      for( int i = 0, len = array.Length; i < len; i++ )
      {
        char element = array[ i ];

        if( value == element )
          return i;
      }

      return -1;
    }

    /// <summary>
    /// Searches for the specified object and returns the index of
    /// the first occurrence within the entire one-dimensional array.
    /// </summary>
    /// <param name="strFormula">String to search in.</param>
    /// <param name="index">Start index.</param>
    /// <param name="arrBreakStrings">Break strings.</param>
    /// <returns>Position of one of the break strings.</returns>
    private static int IndexOf( string strFormula, int index, string[] arrBreakStrings )
    {
      if( strFormula == null )
        throw new ArgumentNullException( "strFormula" );

      if( arrBreakStrings == null )
        throw new ArgumentNullException( "arrBreakStrings" );

      int iLength = arrBreakStrings.Length;

      if( iLength == 0 ) return -1;

      char charCurrent = strFormula[ index ];
      int iLowerBound = GetLowerBound( arrBreakStrings, charCurrent );

      if( iLowerBound < 0 ) return -1;

      for( int i = iLowerBound; i < iLength; i++ )
      {
        string strToCompare = arrBreakStrings[ i ];

        int iCompareLength = strToCompare.Length;

        if( strToCompare == null )
          throw new ArgumentNullException();

        if( iCompareLength == 0 )
          throw new ArgumentException( "String can't be empty" );

        if( strToCompare[ 0 ] != charCurrent ) return -1;

        if( iCompareLength == 1 || string.Compare( strFormula, index + 1, strToCompare, 1,
          iCompareLength - 1 ) == 0 )
        {
          return i;
        }
      }

      return -1;
    }
    /// <summary>
    /// Searches for the specified object and returns the index of
    /// the first occurrence within the entire one-dimensional array.
    /// </summary>
    /// <param name="strFormula">String to search in.</param>
    /// <param name="index">Start index.</param>
    /// <param name="arrBreakStrings">Key is break string.</param>
    /// <returns>Position of one of the break strings.</returns>
    private static int IndexOf( string strFormula, int index, StorageType arrBreakStrings )
    {
      if( strFormula == null )
        throw new ArgumentNullException( "strFormula" );

      if( arrBreakStrings == null )
        throw new ArgumentNullException( "arrBreakStrings" );

      int iLength = arrBreakStrings.Count;

      if( iLength == 0 ) return -1;

      char charCurrent = strFormula[ index ];
      int iUpperBound = GetUpperBound( arrBreakStrings, charCurrent );

      if( iUpperBound < 0 ) return -1;

      for( int i = iUpperBound; i >= 0; i-- )
      {
        string strToCompare = ( string )arrBreakStrings.GetKey( i );

        int iCompareLength = strToCompare.Length;

        if( strToCompare == null )
          throw new ArgumentNullException();

        if( iCompareLength == 0 )
          throw new ArgumentException( "String can't be empty" );

        if( strToCompare[ 0 ] != charCurrent ) return -1;

        if( iCompareLength == 1 || string.Compare( strFormula, index + 1, strToCompare, 1,
          iCompareLength - 1 ) == 0 )
        {
          return i;
        }
      }

      return -1;
    }
    /// <summary>
    /// Returns lower bound of strings that have specified first character.
    /// </summary>
    /// <param name="arrStringValues">Sorted array of string to search.</param>
    /// <param name="chFirst">Desired first character.</param>
    /// <returns>
    /// Lower bound of the strings that have chFirst as the first character
    ///  if there are such strings; otherwise -1.</returns>
    private static int GetLowerBound( string[] arrStringValues, char chFirst )
    {
      if( arrStringValues == null )
        throw new ArgumentNullException( "arrStringValues" );

      int iFirstIndex = 0;
      int iLastIndex = arrStringValues.Length - 1;

      while( iLastIndex != iFirstIndex )
      {
        int iMiddleIndex = ( iLastIndex + iFirstIndex ) / 2;
        string strValue = arrStringValues[ iMiddleIndex ];

        if( strValue == null )
          throw new ArgumentNullException( "String in the array can't be null." );

        char chStringChar = strValue[ 0 ];

        if( chStringChar >= chFirst )
        {
          if( iLastIndex == iMiddleIndex ) break;

          iLastIndex = iMiddleIndex;
        }
        else if( chStringChar < chFirst )
        {
          if( iFirstIndex == iMiddleIndex ) break;

          iFirstIndex = iMiddleIndex;
        }
      }

      string value = arrStringValues[ iFirstIndex ];

      if( value == null )
        throw new ArgumentNullException();

      if( value[ 0 ] == chFirst )
        return iFirstIndex;

      value = arrStringValues[ iLastIndex ];

      if( value == null )
        throw new ArgumentNullException();

      if( value[ 0 ] == chFirst )
        return iLastIndex;

      return -1;
    }
    /// <summary>
    /// Returns lower bound of strings that have specified first character.
    /// </summary>
    /// <param name="arrStringValues">Sorted array of string to search.</param>
    /// <param name="chFirst">Desired first character.</param>
    /// <returns>
    /// Lower bound of the strings that have chFirst as the first character
    ///  if there are such strings; otherwise -1.</returns>
    private static int GetLowerBound( StorageType arrStringValues, char chFirst )
    {
      if( arrStringValues == null )
        throw new ArgumentNullException( "arrStringValues" );

      int iFirstIndex = 0;
      int iLastIndex = arrStringValues.Count - 1;

      while( iLastIndex != iFirstIndex )
      {
        int iMiddleIndex = ( iLastIndex + iFirstIndex ) / 2;
        string strValue = ( string )arrStringValues.GetKey( iMiddleIndex );

        if( strValue == null )
          throw new ArgumentNullException( "String in the array can't be null." );

        char chStringChar = strValue[ 0 ];

        if( chStringChar >= chFirst )
        {
          if( iLastIndex == iMiddleIndex ) break;

          iLastIndex = iMiddleIndex;
        }
        else if( chStringChar < chFirst )
        {
          if( iFirstIndex == iMiddleIndex ) break;

          iFirstIndex = iMiddleIndex;
        }
      }

      string value = ( string )arrStringValues.GetKey( iFirstIndex );

      if( value == null )
        throw new ArgumentNullException();

      if( value[ 0 ] == chFirst )
        return iFirstIndex;

      value = ( string )arrStringValues.GetKey( iLastIndex );

      if( value == null )
        throw new ArgumentNullException();

      if( value[ 0 ] == chFirst )
        return iLastIndex;

      return -1;
    }
    /// <summary>
    /// Returns lower bound of strings that have specified first character.
    /// </summary>
    /// <param name="arrStringValues">Sorted array of string to search.</param>
    /// <param name="chFirst">Desired first character.</param>
    /// <returns>
    /// Lower bound of the strings that have chFirst as the first character
    ///  if there are such strings; otherwise -1.</returns>
    private static int GetUpperBound( StorageType arrStringValues, char chFirst )
    {
      if( arrStringValues == null )
        throw new ArgumentNullException( "arrStringValues" );

      int iFirstIndex = 0;
      int iLastIndex = arrStringValues.Count - 1;

      while( iLastIndex != iFirstIndex )
      {
        int iMiddleIndex = ( iLastIndex + iFirstIndex ) / 2;
        string strValue = ( string )arrStringValues.GetKey( iMiddleIndex );

        if( strValue == null )
          throw new ArgumentNullException( "String in the array can't be null." );

        char chStringChar = strValue[ 0 ];

        if( chStringChar > chFirst )
        {
          if( iLastIndex == iMiddleIndex ) break;

          iLastIndex = iMiddleIndex;
        }
        else if( chStringChar <= chFirst )
        {
          if( iFirstIndex == iMiddleIndex ) break;

          iFirstIndex = iMiddleIndex;
        }
      }

      string value = ( string )arrStringValues.GetKey( iFirstIndex );

      if( value == null )
        throw new ArgumentNullException();

      if( value[ 0 ] == chFirst )
        return iFirstIndex;

      value = ( string )arrStringValues.GetKey( iLastIndex );

      if( value == null )
        throw new ArgumentNullException();

      if( value[ 0 ] == chFirst )
        return iLastIndex;

      return -1;
    }
    /// <summary>
    /// Converts SharedFormula tokens into regular formula tokens.
    /// </summary>
    /// <param name="shared">Shared formula to convert.</param>
    /// <param name="book">Parent workbook.</param>
    /// <param name="iRow">Row of the destination formula.</param>
    /// <param name="iColumn">Column of the destination formula.</param>
    /// <returns>Converted token array.</returns>
    [ CLSCompliant( false ) ]
    public static Ptg[] ConvertSharedFormulaTokens( SharedFormulaRecord shared, IWorkbook book,
      int iRow, int iColumn )
    {
      if( shared == null )
        throw new ArgumentNullException( "shared" );

      Ptg[] arrPtg = shared.Formula;
      int iCount = arrPtg.Length;
      Ptg[] arrResult = new Ptg[ iCount ];

      for( int i = 0; i < iCount; i++ )
      {
        arrResult[ i ] = arrPtg[ i ].ConvertSharedToken( book, iRow, iColumn );
      }

      return arrResult;
    }
    /// <summary>
    /// Updates formula after move / copy operation.
    /// </summary>
    /// <param name="arrPtgs">Tokens to update.</param>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="iColumn">One-based column index.</param>
    /// <returns>Updated tokens array.</returns>
    public Ptg[] UpdateFormula( Ptg[] arrPtgs, int iCurIndex, int iSourceIndex,
      Rectangle sourceRect, int iDestIndex, Rectangle destRect, int iRow, int iColumn )
    {
      bool bChanged;

      for( int i = 0, len = arrPtgs.Length; i < len; i++ )
      {
        Ptg curPtg = arrPtgs[ i ];
        arrPtgs[ i ] = curPtg.Offset( iCurIndex, iRow - 1, iColumn - 1, iSourceIndex,
          sourceRect, iDestIndex, destRect, out bChanged, m_book );
      }

      return arrPtgs;
    }

    /// <summary>
    /// Updates formula after move / copy operation.
    /// </summary>
    /// <param name="arrPtgs">Tokens to update.</param>
    /// <param name="iRowDelta">Value to add to the row index.</param>
    /// <param name="iColumnDelta">Value to add to the column index.</param>
    /// <returns>Updated tokens array.</returns>
    public Ptg[] UpdateFormula( Ptg[] arrPtgs, int iRowDelta, int iColumnDelta )
    {
      for( int i = 0, len = arrPtgs.Length; i < len; i++ )
      {
        Ptg curPtg = arrPtgs[ i ];
        arrPtgs[ i ] = curPtg.Offset( iRowDelta, iColumnDelta, m_book );
      }

      return arrPtgs;
    }
    /// <summary>
    /// Pushes operand into stack correctly.
    /// </summary>
    /// <param name="operands"></param>
    /// <param name="operand"></param>
    public static void PushOperandToStack( Stack<object> operands, string operand )
    {
      if( operands == null )
        throw new ArgumentNullException( "operands" );

      if( operand == null )
        throw new ArgumentNullException( "operand" );

      string strNewOperand = operand;

      if( operands.Count > 0 )
      {
        object lastOperand = operands.Peek();

        AttrPtg spaces = lastOperand as AttrPtg;

        if( spaces != null )
        {
          operands.Pop();
          strNewOperand = spaces.ToString() + operand;
        }
      }

      operands.Push( strNewOperand );
    }
    #endregion

    #region Private utility methods
    /// <summary>
    /// Creates function ptg.
    /// </summary>
    /// <param name="strFormula">Formula string that contains function.</param>
    /// <param name="bracketIndex">Index of bracket after function name.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="indexes">Dictionary with indexes.</param>
    /// <param name="index">Current index.</param>
    /// <param name="hashWorksheetNames">Dictionary with new worksheet names.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
    /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
    /// <returns>Created function token.</returns>
    private Ptg[]   CreateFunction( string strFormula, int bracketIndex, IWorkbook parent,
      IWorksheet sheet, Dictionary<Type, ReferenceIndexAttribute> indexes, int index,
      Dictionary<string, string> hashWorksheetNames,
      ExcelParseFormulaOptions options, int iCellRow, int iCellColumn )
    {
      List<Ptg> result = new List<Ptg>();
      string strExactName = strFormula.Substring( 0, bracketIndex );
      string name = strExactName.ToUpper();
      
      int iRefVarIndex = GetIndex( typeof( FunctionVarPtg ), DEF_TYPE_VALUE, indexes, index,
        options );

      // It is if function?
      if( name == "IF" )
      {
        return CreateIfFunction( iRefVarIndex, strFormula, bracketIndex, parent,
          sheet, hashWorksheetNames, options, iCellRow, iCellColumn );
      }

      ExcelFunction funcIndex;
      int iBookIndex;
      int iNameIndex;

      if( FunctionAliasToId.ContainsKey( name ) )
      {
        funcIndex = FunctionAliasToId[ name ];
      }
      else if( IsCustomFunction( strExactName, parent, out iBookIndex, out iNameIndex ) )
      {
        if( iNameIndex != -1 )
        {
          return CreateCustomFunction( iRefVarIndex, strFormula, bracketIndex,
            iBookIndex, iNameIndex, parent, sheet, hashWorksheetNames, options,
            iCellRow, iCellColumn );
        }
        else
        {
          return CreateCustomFunction( iRefVarIndex, strFormula, bracketIndex,
            parent, sheet, hashWorksheetNames, options, iCellRow, iCellColumn );
        }
      }
      else
      {
        throw new ArgumentException( "Unknown function name: '" + name + "' formula: " + strFormula );
      }

      funcIndex = FunctionAliasToId[ name ];
      OperationPtg ptgFunction = null;
      
      if( FunctionIdToParamCount.ContainsKey( funcIndex ) )
      {
        int iRefIndex = GetIndex( typeof( FunctionPtg ), DEF_TYPE_VALUE, indexes, index, options );
        FormulaToken token = FunctionPtg.IndexToCode( iRefIndex );
        ptgFunction = ( OperationPtg )CreatePtg( token, funcIndex );
      }
      else
      {
        int iRefIndex = GetIndex( typeof( FunctionVarPtg ), DEF_TYPE_VALUE, indexes, index, options );
        FormulaToken token = FunctionVarPtg.IndexToCode( iRefIndex );
        ptgFunction = ( OperationPtg )CreatePtg( token, funcIndex );
      }

      if( IndexOf( SemiVolatileFunctions, funcIndex ) != -1 )
      {
        result.Add( CreatePtg( FormulaToken.tAttr, 1, 0 ) );
      }

      string[] operands = ptgFunction.GetOperands( strFormula, ref bracketIndex, this );

      int i = 0;

      Dictionary<Type, ReferenceIndexAttribute> hashParamIndexes = FunctionIdToIndex[ funcIndex ];
      ExcelParseFormulaOptions modifiedOptions = options;

      if( ( options & ExcelParseFormulaOptions.RootLevel ) != 0 )
      {
        modifiedOptions -= ExcelParseFormulaOptions.RootLevel;
      }

      modifiedOptions |= ExcelParseFormulaOptions.ParseOperand;

      for( int j = 0, len = operands.Length; j < len; j++ )
      {
        string operand = operands[ j ];

        if( hashParamIndexes != null )
        {
          result.AddRange( ParseOperandString( operand, parent, sheet, hashParamIndexes,
            i, hashWorksheetNames, modifiedOptions, iCellRow, iCellColumn ) );
        }
        else
        {
          result.AddRange( ParseOperandString( operand, parent, sheet, null,
            i, hashWorksheetNames, modifiedOptions, iCellRow, iCellColumn ) );
        }
        i++;
      }
      result.Add( ptgFunction );

      return result.ToArray();
    }

    /// <summary>
    /// Indicates whether specified string is name of custom function.
    /// </summary>
    /// <param name="strFunctionName">String to check.</param>
    /// <param name="book">Workbook that contains function.</param>
    /// <param name="iBookIndex">Index to the extern workbook.</param>
    /// <param name="iNameIndex">Name index.</param>
    /// <returns>True if specified string is name of custom function.</returns>
    internal static bool IsCustomFunction( string strFunctionName, IWorkbook book,
      out int iBookIndex, out int iNameIndex )
    {
      if( strFunctionName == null )
        throw new ArgumentNullException( "strFunctionName" );

      if( strFunctionName.Length == 0 )
        throw new ArgumentException( "strFunctionName - string cannot be empty" );

      iBookIndex = -1;
      iNameIndex = -1;

      if( book == null ) return false;

      WorkbookImpl workbook = ( WorkbookImpl )book;

      Match m = AddInFunctionRegEx.Match( strFunctionName );
      bool bResult = false;

      if( m.Success && m.Value == strFunctionName )
      {
        bResult = IsCustomFunction( workbook, m, ref iBookIndex, ref iNameIndex );
      }
      else if( IsLocalCustomFunction( workbook, strFunctionName, ref iNameIndex ) )
      {
        bResult = true;
      }
      else
      {
        bResult = workbook.ExternWorkbooks.ContainsExternName( strFunctionName, ref iBookIndex, ref iNameIndex );
      }

      if( !bResult && !workbook.ThrowOnUnknownNames )
      {
        IName name = workbook.Names[ strFunctionName ];

        if( name == null )
          name = workbook.Names.Add( strFunctionName );

        iNameIndex = name.Index;
        bResult = true;
      }
      //if (workbook.ThrowOnUnknownNames)
      //{
      //    bResult = true;
      //}

      return bResult;
    }
    /// <summary>
    /// Indicates whether specified name is name of local custom function.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="strFunctionName">Function name.</param>
    /// <param name="iNameIndex">Resulting name index.</param>
    /// <returns></returns>
    private static bool IsLocalCustomFunction( WorkbookImpl book,
      string strFunctionName, ref int iNameIndex )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      if( strFunctionName == null )
        throw new ArgumentNullException( "strFunctionName" );

      if( strFunctionName.Length == 0 )
        throw new ArgumentException( "strFunctionName - string cannot be empty" );

      NameImpl name = book.InnerNamesColection[ strFunctionName ] as NameImpl;

      if( name == null )
        return false;

      iNameIndex = name.Index;
      return name.IsFunction;
    }

    /// <summary>
    /// Checks is custom function.
    /// </summary>
    /// <param name="workbook">Parent Workbook.</param>
    /// <param name="m">Current Match.</param>
    /// <param name="iBookIndex">Book index.</param>
    /// <param name="iNameIndex">Name index.</param>
    /// <returns>True if it is custom function, otherwise false.</returns>
    private static bool IsCustomFunction( WorkbookImpl workbook, Match m, ref int iBookIndex, ref int iNameIndex )
    {
      if( m == null )
        throw new ArgumentNullException( "m" );

      string strPath = m.Groups[ DEF_PATH_GROUP ].Value;
      string strBookName = m.Groups[ DEF_BOOKNAME_GROUP ].Value;
      string strSheetName = m.Groups[ DEF_SHEETNAME_GROUP ].Value;
      string strFunctionName = m.Groups[ DEF_RANGENAME_GROUP ].Value;

      if( strBookName == null || strBookName.Length == 0 )
      {
        strBookName = strSheetName;
        strSheetName = null;
      }

      int iLength = strBookName.Length;

      if( strBookName[ 0 ] == DEF_BOOKNAME_OPENBRACKET
        && strBookName[ iLength - 1 ] == DEF_BOOKNAME_CLOSEBRACKET )
      {
        strBookName = strBookName.Substring( 1, iLength - 2 );
      }

      ExternWorkbookImpl externBook  = null;

      if( strPath.Length > 0 )
      {
        externBook = workbook.ExternWorkbooks[ strPath + strBookName ];

        if( externBook == null ) return false;
      }
      else
      {
        externBook = workbook.ExternWorkbooks.GetBookByShortName( strBookName );

        if( externBook == null ) return false;
      }

      iBookIndex = externBook.Index;
      iNameIndex = externBook.ExternNames.GetNameIndex( strFunctionName );

      return ( iNameIndex >= 0 );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iRefIndex"></param>
    /// <param name="strFormula"></param>
    /// <param name="bracketIndex"></param>
    /// <param name="parent"></param>
    /// <param name="sheet"></param>
    /// <param name="hashWorksheetNames"></param>
    /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
    /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
    /// <param name="options"></param>
    /// <returns></returns>
    private Ptg[]   CreateIfFunction( int iRefIndex, string strFormula, int bracketIndex,
      IWorkbook parent, IWorksheet sheet, Dictionary<string, string> hashWorksheetNames,
      ExcelParseFormulaOptions options, int iCellRow, int iCellColumn )
    {
      List<Ptg> result = new List<Ptg>();
      ExcelFunction funcIndex = ExcelFunction.IF;
      OperationPtg ptgFunction = null;
      
      FormulaToken token = FunctionVarPtg.IndexToCode( iRefIndex );
      ptgFunction = (OperationPtg) CreatePtg( token, funcIndex );

      if( IndexOf( SemiVolatileFunctions, funcIndex ) != -1 )
      {
        result.Add( CreatePtg( FormulaToken.tAttr, 1, 0 ) );
      }

      string[] operands = ptgFunction.GetOperands( strFormula, ref bracketIndex, this );
      
      // IF function can accept only three arguments
      if( operands.Length > 3 || operands.Length < 2 )
        throw new ArgumentOutOfRangeException( "Argument count must be 2 or 3", strFormula );

      int i = 0;

      Dictionary<Type, ReferenceIndexAttribute> indexes = FunctionIdToIndex[ funcIndex ];

      List<Ptg[]> arrParsedOperands = new List<Ptg[]>( 4 );//3 + 1
      ExcelParseFormulaOptions modifiedOptions = options;

      if( ( options & ExcelParseFormulaOptions.RootLevel ) != 0 )
      {
        modifiedOptions -= ExcelParseFormulaOptions.RootLevel;
      }

      for( int j = 0, len = operands.Length; j < len; j++ )
      {
        string operand = operands[ j ];

        if( indexes != null )
        {
          arrParsedOperands.Add( ParseOperandString( operand, parent, sheet,
            indexes, i, hashWorksheetNames, modifiedOptions, iCellRow, iCellColumn ) );
        }
        else
        {
          arrParsedOperands.Add( ParseOperandString( operand, parent, sheet,
            null, i, hashWorksheetNames, modifiedOptions, iCellRow, iCellColumn ) );
        }

        i++;
      }

      result.AddRange( arrParsedOperands[ 0 ] );

      int iTrueSize = 0;
      int iFalseSize = 0;

      Ptg[] arrTrue = arrParsedOperands[ 1 ];
      Ptg[] arrFalse = null;
      ExcelVersion version = ( ( WorkbookImpl )parent ).Version;

      for( int j = 0, len = arrTrue.Length; j < len; j++ )
      {
        iTrueSize += arrTrue[ j ].GetSize( version );
      }

      if( operands.Length == 3 )
      {
        arrFalse = ( Ptg[] )arrParsedOperands[ 2 ];

        for( int j = 0, len = arrFalse.Length; j < len; j++ )
        {
          iFalseSize += arrFalse[ j ].GetSize( version );
        }

        iFalseSize += 4;
      }

      result.Add( CreatePtg( FormulaToken.tAttr, 2, iTrueSize + 4 ) );
      result.AddRange( arrTrue );

      bool bNotInArrayFormula = ( ( options & ExcelParseFormulaOptions.InArray ) == 0 );
      int iOptions = bNotInArrayFormula ? DEF_OPTIONS_OPT_GOTO : DEF_OPTIONS_NOT_OPT_GOTO;
      result.Add( CreatePtg( FormulaToken.tAttr, iOptions, iFalseSize + 3 ) );

      if( arrFalse != null )
      {
        result.AddRange( arrFalse );
        result.Add( CreatePtg( FormulaToken.tAttr, iOptions, 3 ) );
      }

      result.Add( ptgFunction );

      return result.ToArray();
    }

    /// <summary>
    /// Creates tokens for custom function and its arguments.
    /// </summary>
    /// <param name="iRefIndex">Token reference index.</param>
    /// <param name="strFormula">Formula string.</param>
    /// <param name="bracketIndex">Bracket index in the formula.</param>
    /// <param name="parent">Parent workbook.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="hashWorksheetNames">Dictionary with worksheet names (key - old name, value - new name).</param>
    /// <param name="options">Parse options.</param>
    /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
    /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
    /// <returns>Array of tokens that describes custom function.</returns>
    private Ptg[]   CreateCustomFunction( int iRefIndex, string strFormula, int bracketIndex,
      IWorkbook parent, IWorksheet sheet, Dictionary<string, string> hashWorksheetNames,
      ExcelParseFormulaOptions options, int iCellRow, int iCellColumn )
    {
      string strFunctionName = GetLeftUnaryOperand( strFormula, bracketIndex );
      WorkbookImpl book = ( WorkbookImpl )parent;
      int iNameIndex;
      int iNameBookIndex = book.ExternWorkbooks.GetNameIndexes( strFunctionName, out iNameIndex );

      return CreateCustomFunction( iRefIndex, strFormula, bracketIndex, iNameBookIndex, iNameIndex,
        parent, sheet, hashWorksheetNames, options, iCellRow, iCellColumn );
    }

    /// <summary>
    /// Creates tokens for custom function and its arguments.
    /// </summary>
    /// <param name="iRefIndex">Token reference index.</param>
    /// <param name="strFormula">Formula string.</param>
    /// <param name="bracketIndex">Bracket index in the formula.</param>
    /// <param name="iBookIndex"></param>
    /// <param name="iNameIndex"></param>
    /// <param name="parent">Parent workbook.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="hashWorksheetNames">Dictionary with worksheet names (key - old name, value - new name).</param>
    /// <param name="options">Parse options.</param>
    /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
    /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
    /// <returns>Array of tokens that describes custom function.</returns>
    private Ptg[]   CreateCustomFunction( int iRefIndex, string strFormula,
      int bracketIndex, int iBookIndex, int iNameIndex, IWorkbook parent,
      IWorksheet sheet, Dictionary<string, string> hashWorksheetNames, ExcelParseFormulaOptions options,
      int iCellRow, int iCellColumn )
    {
      List<Ptg> result = new List<Ptg>();
      ExcelFunction funcIndex = ExcelFunction.CustomFunction;
      OperationPtg ptgFunction = null;
      
      FormulaToken token = FunctionVarPtg.IndexToCode( iRefIndex );
      ptgFunction = ( OperationPtg )CreatePtg( token, funcIndex );

      if( IndexOf( SemiVolatileFunctions, funcIndex ) != -1 )
      {
        result.Add( CreatePtg( FormulaToken.tAttr, 1, 0 ) );
      }

      WorkbookImpl book = ( WorkbookImpl )parent;
      //int iReferenceIndex = ( iBookIndex == -1 ) ? -1 : book.GetReferenceIndex( iBookIndex );

      string[] operands = ptgFunction.GetOperands( strFormula, ref bracketIndex, this );
      
      int i = 0;

      Dictionary<Type, ReferenceIndexAttribute> indexes = FunctionIdToIndex[ funcIndex ];

      ExcelParseFormulaOptions modifiedOptions = options;

      if( ( options & ExcelParseFormulaOptions.RootLevel ) != 0 )
      {
        modifiedOptions -= ExcelParseFormulaOptions.RootLevel;
      }

      Ptg tokenName = ( iBookIndex != -1 )
        ? CreatePtg( FormulaToken.tNameX1, iBookIndex, iNameIndex )
        : tokenName = CreatePtg( FormulaToken.tName1, iNameIndex );

      result.Add( tokenName );

      for( int j = 0, len = operands.Length; j < len; j++ )
      {
        string operand = operands[ j ];

        if( indexes != null )
        {
          result.AddRange( ParseOperandString( operand, parent, sheet,
            indexes, i, hashWorksheetNames, modifiedOptions, iCellRow, iCellColumn ) );
        }
        else
        {
          result.AddRange( ParseOperandString( operand, parent, sheet,
            null, i, hashWorksheetNames, modifiedOptions, iCellRow, iCellColumn ) );
        }

        i++;
      }

      result.Add( ptgFunction );

      return result.ToArray();
    }
    /// <summary>
    /// Creates token that describes specified error.
    /// </summary>
    /// <param name="strFormula">Formula string that contains error string.</param>
    /// <param name="errorIndex">Index of the error name.</param>
    /// <returns>Created token that contains error.</returns>
    public static Ptg     CreateError( string strFormula, int errorIndex )
    {
      string errorName = GetErrorOperand( strFormula, errorIndex );
      ConstructorInfo constr = ErrorNameToConstructor[ errorName ];
      return ( Ptg )constr.Invoke( new object[] { strFormula } );
    }
    /// <summary>
    /// Converts operand string into Ptg array.
    /// </summary>
    /// <param name="operand">Operand string that should be parsed.</param>
    /// <param name="parent">Parent workbook.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="indexes"></param>
    /// <param name="i"></param>
    /// <param name="hashWorksheetNames"></param>
    /// <param name="options">Parse options.</param>
    /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
    /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
    /// <returns>Parsed operand as array of ptg.</returns>
    private Ptg[]   ParseOperandString( string operand, IWorkbook parent,
      IWorksheet sheet, Dictionary<Type, ReferenceIndexAttribute> indexes, int i, Dictionary<string, string> hashWorksheetNames,
      ExcelParseFormulaOptions options, int iCellRow, int iCellColumn )
    {
//      int refIndex;

      if( operand.Length == 0 )
      {
        return new Ptg[]{ CreatePtg( FormulaToken.tMissingArgument, operand ) };
      }

      try
      {
        options |= ExcelParseFormulaOptions.ParseOperand;
        return ParseString( operand, sheet, indexes, i, hashWorksheetNames, options, iCellRow, iCellColumn );
      }
      catch( Exception ex )
      {
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message + Environment.NewLine + ex.StackTrace
        //  , "Exception in formula: '" + operand + "'" );
        throw;
      }
    }
    /// <summary>
    /// Creates unary operation.
    /// </summary>
    /// <param name="OperationSymbol">Operation symbol that should be created.</param>
    /// <returns>Newly created unary operation.</returns>
    private static OperationPtg CreateUnaryOperation( char OperationSymbol )
    {
      string strOperation = OperationSymbol.ToString();

      if( OperationSymbol == '(' )
        return ( OperationPtg )CreatePtg( FormulaToken.tParentheses, strOperation );

      FormulaToken tokenId = UnaryOperationPtg.GetTokenId( strOperation );

      return ( OperationPtg )CreatePtg( tokenId, strOperation );
    }
    /// <summary>
    /// Return string array of arguments of the operation.
    /// </summary>
    /// <param name="strFormula">Formula string.</param>
    /// <returns>OperationPtg that is built by OpIndex.</returns>
    private OperationPtg CreateOperation( string strFormula )
    {
      // TODO: There can be not only binary operation.
      FormulaToken tokenId = ( strFormula == m_strOperandsSeparator )
        ? FormulaToken.tCellRangeList
        : BinaryOperationPtg.GetTokenId( strFormula );

      return ( OperationPtg )CreatePtg( tokenId, strFormula );

    }
    /// <summary>
    /// Indicates if specified string is cell name.
    /// </summary>
    /// <param name="strFormula">Formula string that will be checked.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation must be used.</param>
    /// <param name="strRow">String representation of the row.</param>
    /// <param name="strColumn">String representation of the column.</param>
    /// <returns>True if parameter is cell name, i.e. "A1", False otherwise.</returns>
    public static bool    IsCell( string strFormula, bool bR1C1, out string strRow, out string strColumn )
    {
      Regex regex = bR1C1 ? CellR1C1Regex : CellRegex;
      Match m = regex.Match( strFormula );
      bool bResult = ( m.Success && m.Value == strFormula );

      if( bResult )
      {
        strRow = m.Groups[ DEF_GROUP_ROW1 ].Value;
        strColumn = m.Groups[ DEF_GROUP_COLUMN1 ].Value;
      }
      else
      {
        strRow = null;
        strColumn = null;
      }

      return bResult;
    }
      /// <summary>
      /// Returns true, if specified string is R1C1 reference.
      /// </summary>
      /// <param name="strFormula">Formula string that will be checked.</param>
      /// <returns>True, if paramater is R1C1 cell reference.</returns>
    internal static bool IsR1C1(string strFormula)
    {
        Regex regex =  CellR1C1Regex ;//: CellRegex;
        Match m = regex.Match(strFormula);
        return (m.Success && m.Value == strFormula);
    }
    /// <summary>
    /// Indicates if specified string is cell range.
    /// </summary>
    /// <param name="strFormula">Formula string that will be checked.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation must be used.</param>
    /// <param name="strRow1">The first cell's row.</param>
    /// <param name="strColumn1">The first cell's column.</param>
    /// <param name="strRow2">The second cell's row.</param>
    /// <param name="strColumn2">The second cell's column</param>
    /// <returns>True if parameter is cell range, i.e. "A1:D1", False otherwise.</returns>
    public bool    IsCellRange( string strFormula, bool bR1C1,
      out string strRow1, out string strColumn1,
      out string strRow2,out string strColumn2 )
    {
      strColumn1 = null;
      strColumn2 = null;
      strRow1 = null;
      strRow2 = null;

      Regex regex = bR1C1 ? CellRangeR1C1Regex : CellRangeRegex;
      Match m = regex.Match( strFormula );
      bool bResult = IsSuccess( m, strFormula );

      if( bResult )
      {
        strColumn1 = m.Groups[ DEF_GROUP_COLUMN1 ].Value;
        strColumn2 = m.Groups[ DEF_GROUP_COLUMN2 ].Value;
        strRow1 = m.Groups[ DEF_GROUP_ROW1 ].Value;
        strRow2 = m.Groups[ DEF_GROUP_ROW2 ].Value;
      }
      else if( bR1C1 )
      {
        m = CellRangeR1C1ShortRegex.Match( strFormula );
        bResult = IsSuccess( m, strFormula );
        
		if (bResult)
        {
            if (strFormula[0].ToString() == RefPtg.DEF_R1C1_ROW )
            {
                strRow2 = strRow1 = strFormula;
            }
            else
            {
                strColumn1 = strColumn2 = strFormula;
            }
        }
        else
        {
            m = FullRowRangeR1C1Regex.Match(strFormula);
            bResult = IsSuccess(m, strFormula);

            if (bResult)
            {
                strColumn1 = RefPtg.DEF_R1C1_COLUMN + "1";
                strColumn2 = RefPtg.DEF_R1C1_COLUMN + (m_book.MaxColumnCount).ToString();
                strRow1 = m.Groups[DEF_GROUP_ROW1].Value;
                strRow2 = m.Groups[DEF_GROUP_ROW2].Value;
            }
            else
            {
                m = FullColumnRangeR1C1Regex.Match(strFormula);
                bResult = IsSuccess(m, strFormula);

                if (bResult)
                {
                    strColumn1 = m.Groups[DEF_GROUP_COLUMN1].Value;
                    strColumn2 = m.Groups[DEF_GROUP_COLUMN2].Value;
                    strRow1 = RefPtg.DEF_R1C1_ROW + "1";
                    strRow2 = RefPtg.DEF_R1C1_ROW + (m_book.MaxRowCount).ToString();
                }
            }
        }
      }
      else
      {
        m = FullRowRangeRegex.Match( strFormula );
        bResult = IsSuccess( m, strFormula );

        if( bResult )
        {
          strColumn1 = "$A";
          strColumn2 = "$" + RangeImpl.GetColumnName( m_book.MaxColumnCount );
          strRow1 = m.Groups[ DEF_GROUP_ROW1 ].Value;
          strRow2 = m.Groups[ DEF_GROUP_ROW2 ].Value;
        }
        else
        {
          m = FullColumnRangeRegex.Match( strFormula );
          bResult = IsSuccess( m, strFormula );

          if( bResult )
          {
            strColumn1 = m.Groups[ DEF_GROUP_COLUMN1 ].Value;
            strColumn2 = m.Groups[ DEF_GROUP_COLUMN2 ].Value;
            strRow1 = "$1";
            strRow2 = "$" + m_book.MaxRowCount.ToString();
          }
        }
      }

      return bResult;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="m"></param>
    /// <param name="strFormula"></param>
    /// <returns></returns>
    private static bool IsSuccess( Match m, string strFormula )
    {
      return m.Success && m.Index == 0 && m.Length == strFormula.Length;
    }
    /// <summary>
    /// Indicates if specified strings is 3d cell reference.
    /// </summary>
    /// <param name="strFormula">Formula string that will be checked.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation must be used.</param>
    /// <param name="strSheetName">Name of the worksheet if succeeded; otherwise - NULL.</param>
    /// <param name="strRow">String representation of the row part of the cell reference.</param>
    /// <param name="strColumn">String representation of the column part of the cell reference.</param>
    /// <returns>
    /// True if parameter is 3d cell reference, i.e. "Sheet1!A1", False otherwise.
    /// </returns>
    public static bool    IsCell3D( string strFormula, bool bR1C1,
      out string strSheetName, out string strRow, out string strColumn )
    {
      Regex regex = bR1C1 ? CellR1C13DRegex : Cell3DRegex;
      Match m = regex.Match( strFormula );
      bool bResult = m.Success && m.Index == 0 && m.Length == strFormula.Length;

      if( bResult )
      {
        strSheetName = m.Groups[ DEF_SHEETNAME_GROUP ].Value;
        strSheetName = NormalizeSheetName( strSheetName );
        strRow = m.Groups[ DEF_GROUP_ROW1 ].Value;
        strColumn = m.Groups[ DEF_GROUP_COLUMN1 ].Value;
        return true;
      }

      strSheetName = null;
      strRow = null;
      strColumn = null;

      return false;
    }
    /// <summary>
    /// Indicates whether specified string is 3d cell range.
    /// </summary>
    /// <param name="strFormula">String that should be checked.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation must be used.</param>
    /// <param name="strSheetName">String that initialize by current sheet name.</param>
    /// <param name="strRow1">String representation of the row part of the first cell reference.</param>
    /// <param name="strColumn1">String representation of the column part of the first cell reference.</param>
    /// <param name="strRow2">String representation of the row part of the second cell reference.</param>
    /// <param name="strColumn2">String representation of the column part of the second cell reference.</param>
    /// <returns>True if string is 3d cell range.</returns>
    public bool    IsCellRange3D( string strFormula, bool bR1C1,
      out string strSheetName, out string strRow1, out string strColumn1,
      out string strRow2, out string strColumn2 )
    {
      strSheetName = null;
      strRow1 = null;
      strColumn1 = null;
      strRow2 = null;
      strColumn2 = null;

      Regex regex = bR1C1 ? CellRangeR1C13DRegex : CellRange3DRegex;
      Match m = regex.Match( strFormula );
      bool bResult = m.Success && m.Index == 0 && m.Length == strFormula.Length;
      
      if( !bResult )
      {
        regex = bR1C1 ? CellRangeR1C13DRegex2 : CellRange3DRegex2;
        m = regex.Match( strFormula );
        bResult = m.Success && m.Index == 0 && m.Length == strFormula.Length;
      }
      
      if( !bResult )
      {
        m = Full3DRowRangeRegex.Match( strFormula );
        bResult = IsSuccess( m, strFormula );

        if( bResult )
        {
          strSheetName = m.Groups[ DEF_SHEETNAME_GROUP ].Value;
          strColumn1 = "$A";
          strColumn2 = "$" + RangeImpl.GetColumnName( m_book.MaxColumnCount );
          strRow1 = m.Groups[ DEF_GROUP_ROW1 ].Value;
          strRow2 = m.Groups[ DEF_GROUP_ROW2 ].Value;
          strSheetName = NormalizeSheetName( strSheetName );
          return bResult;
        }
        else
        {
          m = Full3DColumnRangeRegex.Match( strFormula );
          bResult = IsSuccess( m, strFormula );

          if( bResult )
          {
            strSheetName = m.Groups[ DEF_SHEETNAME_GROUP ].Value;
            strColumn1 = m.Groups[ DEF_GROUP_COLUMN1 ].Value;
            strColumn2 = m.Groups[ DEF_GROUP_COLUMN2 ].Value;
            strRow1 = "$1";
            strRow2 = "$" + m_book.MaxRowCount.ToString();
            strSheetName = NormalizeSheetName( strSheetName );
            return bResult;
          }
        }
      }

      if( !bResult && bR1C1 )
      {
        m = CellRangeR1C13DShortRegex.Match( strFormula );
        bResult = IsSuccess( m, strFormula );

        if( bResult )
        {
          strSheetName = m.Groups[ DEF_SHEETNAME_GROUP ].Value;

          if( strFormula[ strSheetName.Length + 1 ] == 'R' )
          {
            strRow2 = strRow1 = strFormula.Substring( strSheetName.Length + 1 );
          }
          else
          {
            strColumn1 = strColumn2 = strFormula.Substring( strSheetName.Length + 1 );
          }
        }
      }
      else if( bResult )
      {
        strSheetName = m.Groups[ DEF_SHEETNAME_GROUP ].Value;
        strRow1 = m.Groups[ DEF_GROUP_ROW1 ].Value;
        strColumn1 = m.Groups[ DEF_GROUP_COLUMN1 ].Value;
        strRow2 = m.Groups[ DEF_GROUP_ROW2 ].Value;
        strColumn2 = m.Groups[ DEF_GROUP_COLUMN2 ].Value;
      }

      strSheetName = NormalizeSheetName( strSheetName );
      return bResult;
    }
    /// <summary>
    /// Tells whether specified string contains error
    /// at the specified position.
    /// </summary>
    /// <param name="strFormula">String that can contain error string.</param>
    /// <param name="errorIndex">Index were error.</param>
    /// <returns>
    /// True if specified string contains error
    /// at the specified position.
    /// </returns>
    private static bool    IsErrorString( string strFormula, int errorIndex )
    {
      foreach( string errorName in ErrorNameToConstructor.Keys )
      {
        if( string.Compare( strFormula, errorIndex, errorName, 0, errorName.Length ) == 0 )
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Indicates whether specified string is name defined in the workbook.
    /// </summary>
    /// <param name="strFormula">String to check.</param>
    /// <param name="parent">Parent workbook.</param>
    /// <param name="sheet">IWorksheet implement.</param>
    /// <returns>True if specified string is name defined in the workbook; False otherwise.</returns>
    private static bool    IsNamedRange( string strFormula, IWorkbook parent, IWorksheet sheet )
    {
      bool result = false;
      
      if( sheet != null )
      {
        result = sheet.Names.Contains( strFormula );
      }
      
      if( !result && parent != null )
      {
        result = parent.Names.Contains( strFormula );
      }
      
      return result;
    }
    /// <summary>
    /// Searches for corresponding bracket.
    /// </summary>
    /// <param name="strFormula">Formula string where bracket was found.</param>
    /// <param name="BracketPos">Position of the found bracket.</param>
    /// <param name="StartBrackets">Array of all possible opening brackets.</param>
    /// <param name="delta">Search direction.</param>
    /// <returns>Position of the corresponding record.</returns>
    /// <exception cref="System.ArgumentException">
    /// If the corresponding bracket is not found.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If there is no bracket at the specified bracket position.
    /// </exception>
    private static int     FindCorrespondingBracket( string strFormula, int BracketPos, 
      char[] StartBrackets, int delta )
    {
      char EndBracket;
      bool bString = false;

      int index = IndexOf( OpenBrackets, strFormula[ BracketPos ] );

      if( index != -1 )
      {
        EndBracket = CloseBrackets[ index ];
      }
      else
      {
        index = IndexOf( CloseBrackets, strFormula[ BracketPos ] );
        
        if( index == -1 )
          throw new ArgumentOutOfRangeException( "Specified position is not a position of bracket" );
        
        EndBracket = OpenBrackets[ index ];
      }

      if( IndexOf( StringBrackets, EndBracket ) != -1 )
        bString = true;

      for( int i = BracketPos + delta, iLength = strFormula.Length; i < iLength && i >= 0; i += delta )
      {
        if( strFormula[ i ] == EndBracket )
        {
          return i;
        }
        else if( !bString && IndexOf( StartBrackets, strFormula[ i ] ) != -1 )
        {
          i = FindCorrespondingBracket( strFormula, i, StartBrackets, delta );
        }
      }

      //return -1;
      throw new ArgumentException( "Expression is invalid. Can't find corresponding bracket" );
    } 
    /// <summary>
    /// Indicates if specified string contains unary operation 
    /// at the specified position.
    /// </summary>
    /// <param name="strFormula">Formula string.</param>
    /// <param name="OpIndex">Index of unary operation.</param>
    /// <returns>Returns true if operation is unary, false otherwise.</returns>
    private static bool    IsUnaryOperation( string strFormula, int OpIndex )
    {
      // TODO: change this function
      return IndexOf( strFormula, OpIndex, UnaryOperations ) != -1;
      //return ( strFormula[ OpIndex ] == '+' || strFormula[ OpIndex ] == '-' || strFormula[ OpIndex ] == '(' );
    }
    /// <summary>
    /// Checks whether the specified string contains operation at the specified position.
    /// </summary>
    /// <param name="strFormula">Formula string.</param>
    /// <param name="index">Index of operation symbol in the string.</param>
    /// <param name="iOperationIndex">Operation index.</param>
    /// <returns>True if operation is in specified location.</returns>
    private bool    IsOperation( string strFormula, int index, out int iOperationIndex )
    {
      iOperationIndex = IndexOf( strFormula, index, m_arrAllOperations );

      return iOperationIndex != -1;
    }
    /// <summary>
    /// Checks whether specified operand is a function call.
    /// </summary>
    /// <param name="strOperand">Operand that will be checked.</param>
    /// <param name="iBracketPos">Position of the opening bracket.</param>
    /// <returns>True if operand denotes function call, False otherwise.</returns>
    private static bool    IsFunction( string strOperand, out int iBracketPos )
    {
      iBracketPos = -1;

      if( IndexOf( StringBrackets, strOperand[ 0 ] ) != -1 )
      {
        return false;
      }

      iBracketPos = strOperand.IndexOf( '(' );

      return ( iBracketPos != -1 && FindCorrespondingBracket( strOperand, iBracketPos )
        == strOperand.Length - 1 );
    }
    /// <summary>
    /// Gets string representing error name that started from errorIndex.
    /// </summary>
    /// <param name="strFormula">Formula string that contains error name.</param>
    /// <param name="errorIndex">Index of the first char of the error.</param>
    /// <returns>
    /// String representing error name that started from errorIndex.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    /// When starting symbol of the possible error is not '#'
    /// or if can't find such error name.
    /// </exception>
    private static string  GetErrorOperand( string strFormula, int errorIndex )
    {
      if( strFormula[ errorIndex ] != '#' )
      {
        throw new ArgumentException( "Not error string" );
      }

      foreach( string errorName in ErrorNameToConstructor.Keys )
      {
        if( string.Compare( strFormula, errorIndex, errorName, 0, errorName.Length ) == 0 )
        {
          return errorName;
        }
      }

      throw new ArgumentException( "Error name was not found" );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetType"></param>
    /// <param name="indexes"></param>
    /// <param name="i"></param>
    /// <returns></returns>
    private static int     GetExpectedIndex( Type targetType, Dictionary<Type, ReferenceIndexAttribute> indexes, int i )
    {
      if( indexes == null ) return DEF_REFERENCE_INDEX;

      ReferenceIndexAttribute index;
      
      if( indexes.TryGetValue( targetType, out index ) )
      {
        return index[ i ];
      }
      else if( index == null && targetType != typeof( RefPtg ) )
      {
        return GetExpectedIndex( typeof( RefPtg ), indexes, i );
      }
      
      return DEF_REFERENCE_INDEX;
    }
    /// <summary>
    /// Returns index of the token that should be used at 
    /// position i by type targetType in function call.
    /// </summary>
    /// <param name="targetType">Target type for which index will be searched.</param>
    /// <param name="valueType"></param>
    /// <param name="indexes">Dictionary with indexes.</param>
    /// <param name="i">Position of the function parameter.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Index of the token code.</returns>
    public static int     GetIndex( Type targetType, int valueType, Dictionary<Type, ReferenceIndexAttribute> indexes, int i,
      ExcelParseFormulaOptions options )
    {
      bool bInNames = ( options & ExcelParseFormulaOptions.InName ) != 0;

      if( bInNames )
        return DEF_NAME_INDEX;

      if( indexes == null )
        return DEF_REFERENCE_INDEX;
      
      int iExpectedIndex = GetExpectedIndex( targetType, indexes, i ) - 1;

      bool bRoot = ( options & ExcelParseFormulaOptions.RootLevel ) != 0;
      bool bInArray = ( options & ExcelParseFormulaOptions.InArray ) != 0;
      bool bOperand = ( options & ExcelParseFormulaOptions.ParseComplexOperand ) != 0;

      int iFirstMatrixIndex = bRoot ? DEF_INDEX_ROOT_LEVEL : iExpectedIndex;
      int iLastMatrixIndex = bInNames ? DEF_INDEX_NAME
        : bInArray ? DEF_ARRAY_INDEX : DEF_INDEX_DEFAULT;

      int iResult = DEF_INDEXES_CONVERTION[ iFirstMatrixIndex ][ valueType ][ iLastMatrixIndex ];

      // For some reason if argument index should be 1 and it was met in formula
      // (not only constant) then MS Excel changes it to 2
      if( iResult == 1 && bOperand && !bRoot )
        iResult = 2;

      return iResult;
    }
    /// <summary>
    /// Creates new Ptg and sets offset to point just after its data.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    /// <returns>Parsed Ptg token.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When data array is smaller than token that should be stored in it.
    /// </exception>
    public static Ptg      CreatePtg( DataProvider provider, ref int offset, ExcelVersion version )
    {
      FormulaToken id = ( FormulaToken )provider.ReadByte( offset );
      Ptg ptg;
      
      if( !s_hashTokenCodeToPtg.TryGetValue( id, out ptg ) )
        throw new ArgumentException( "Cannot find Formula token with code: " + id );

      ptg = ( Ptg )ptg.Clone();
      ptg.TokenCode = id;
      offset++;
      ptg.InfillPTG( provider, ref offset , version );

      return ptg;
    }
    /// <summary>
    /// Creates formula token using token code.
    /// </summary>
    /// <param name="token">Token code.</param>
    /// <returns>Newly created token.</returns>
    public static Ptg CreatePtg( FormulaToken token )
    {
      TokenConstructor constr = TokenCodeToConstructor[ token ];
      Ptg result = constr.CreatePtg();
      result.TokenCode = token;
      
      return result;
    }
    /// <summary>
    /// Creates formula token using token code.
    /// </summary>
    /// <param name="token">Token code that is used to get constructor and as constructor argument as well.</param>
    /// <returns>Newly created token.</returns>
    public static Ptg CreatePtgByType( FormulaToken token )
    {
      TokenConstructor constr = TokenCodeToConstructor[ token ];
      Ptg result = constr.CreatePtg( token );
      result.TokenCode = token;
      
      return result;
    }
    /// <summary>
    /// Creates token using token id and string representing this token.
    /// </summary>
    /// <param name="token">Token id of the token that should be created.</param>
    /// <param name="tokenString">String that will be passed to the token constructor.</param>
    /// <returns>Newly created token.</returns>
    public static Ptg CreatePtg( FormulaToken token, string tokenString )
    {
      TokenConstructor constr = TokenCodeToConstructor[ token ];
      Ptg result = constr.CreatePtg( tokenString );
      result.TokenCode = token;
      return result;
    }
    /// <summary>
    /// Creates token using token code, token string, and parent workbook.
    /// </summary>
    /// <param name="token">Code of the new token.</param>
    /// <param name="tokenString">String representation of the token.</param>
    /// <param name="parent">Parent workbook.</param>
    /// <returns>Newly created token.</returns>
    public static Ptg CreatePtg( FormulaToken token, string tokenString, IWorkbook parent )
    {
      TokenConstructor constr = TokenCodeToConstructor[ token ];
      Ptg result = constr.CreatePtg( tokenString, parent );
      result.TokenCode = token;
      return result;
    }
    /// <summary>
    /// Creates specified token, passes specified parameters to its constructor.
    /// </summary>
    /// <param name="token">Code of the token that will be created.</param>
    /// <param name="arrParams">Constructor parameters.</param>
    /// <returns>Newly created token.</returns>
    public static Ptg CreatePtg( FormulaToken token, params object[] arrParams )
    {
      TokenConstructor constr = TokenCodeToConstructor[ token ];
      Ptg result = constr.CreatePtg( arrParams );
      result.TokenCode = token;
      return result;
    }
    /// <summary>
    /// Creates specified token, passes two integers to its constructor.
    /// </summary>
    /// <param name="token">Token to create.</param>
    /// <param name="iParam1">First integer argument.</param>
    /// <param name="iParam2">Second integer argument.</param>
    /// <returns>Newly created token.</returns>
    [ CLSCompliant( false ) ]
    public static Ptg CreatePtg( FormulaToken token, ushort iParam1, ushort iParam2 )
    {
      TokenConstructor constr = TokenCodeToConstructor[ token ];
      Ptg result = constr.CreatePtg( iParam1, iParam2 );
      result.TokenCode = token;
      return result;
    }
    /// <summary>
    /// Creates specified token, passes function index to its constructor.
    /// </summary>
    /// <param name="token">Token to create.</param>
    /// <param name="functionIndex">Function index.</param>
    /// <returns>Newly created token.</returns>
    [ CLSCompliant( false ) ]
    public static Ptg CreatePtg( FormulaToken token, ExcelFunction functionIndex )
    {
      TokenConstructor constr = TokenCodeToConstructor[ token ];
      Ptg result = constr.CreatePtg( functionIndex );
      result.TokenCode = token;
      return result;
    }
    /// <summary>
    /// Creates token using two string values.
    /// </summary>
    /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
    /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
    /// <param name="token">Token to create.</param>
    /// <param name="strParam1">First value.</param>
    /// <param name="strParam2">Second value.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation is used.</param>
    /// <returns>Created formula token.</returns>
    public static Ptg CreatePtg( FormulaToken token, int iCellRow, int iCellColumn,
      string strParam1, string strParam2, bool bR1C1 )
    {
      TokenConstructor constr = TokenCodeToConstructor[ token ];
      Ptg result = constr.CreatePtg( iCellRow, iCellColumn, strParam1, strParam2, bR1C1 );
      result.TokenCode = token;
      return result;
    }
    /// <summary>
    /// Creates token using two string values.
    /// </summary>
    /// <param name="token">Token to create.</param>
    /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
    /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
    /// <param name="strParam1">First value.</param>
    /// <param name="strParam2">Second value.</param>
    /// <param name="strParam3">Third value.</param>
    /// <param name="strParam4">Fourth value.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation is used.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns>Created formula token.</returns>
    public static Ptg CreatePtg( FormulaToken token, int iCellRow, int iCellColumn,
      string strParam1, string strParam2, string strParam3, string strParam4, bool bR1C1, IWorkbook book )
    {
      TokenConstructor constr = TokenCodeToConstructor[ token ];
      Ptg result = constr.CreatePtg( iCellRow, iCellColumn, strParam1, strParam2, strParam3, strParam4, bR1C1, book );
      result.TokenCode = token;
      return result;
    }
    /// <summary>
    /// Creates token using two string values.
    /// </summary>
    /// <param name="token">Token to create.</param>
    /// <param name="iCellRow">Row index of the cell that contains formula to parse.</param>
    /// <param name="iCellColumn">Column index of the cell that contains formula to parse.</param>
    /// <param name="iRefIndex">Worksheet reference index.</param>
    /// <param name="strParam1">First value.</param>
    /// <param name="strParam2">Second value.</param>
    /// <param name="strParam3">Third value.</param>
    /// <param name="strParam4">Fourth value.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation is used.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns>Created formula token.</returns>
    public static Ptg CreatePtg( FormulaToken token, int iCellRow, int iCellColumn, int iRefIndex,
      string strParam1, string strParam2, string strParam3, string strParam4, bool bR1C1, IWorkbook book )
    {
      TokenConstructor constr = TokenCodeToConstructor[ token ];
      Ptg result = constr.CreatePtg( iCellRow, iCellColumn, iRefIndex,
        strParam1, strParam2, strParam3, strParam4, bR1C1, book );
      result.TokenCode = token;

      return result;
    }
    /// <summary>
    /// Removes tokens that are unnecessary.
    /// </summary>
    /// <param name="ptgs"></param>
    /// <returns></returns>
    private static Ptg[] SkipUnnecessaryTokens( Ptg[] ptgs )
    {
      List<Ptg> result = new List<Ptg>();
      AttrPtg attr;

      foreach( Ptg ptg in ptgs )
      {
        if( ptg is AttrPtg )
        {
          attr = ptg as AttrPtg;

          if( attr.HasOptGoto || attr.HasOptimizedIf || attr.HasSemiVolatile || attr.HasOptimizedChoose )
            continue;
        }

        if( ptg is MemFuncPtg ) continue;

        if( ptg is MemAreaPtg )
        {
          MemAreaPtg memArea = ptg as MemAreaPtg;
          result.AddRange( memArea.Subexpression );
        }

        result.Add( ptg );
      }

      return result.ToArray();
    }
    /// <summary>
    /// Puts all right sided unary operations before operand.
    /// </summary>
    /// <param name="strFormula">Formula to transform.</param>
    /// <returns>Formula after transformation.</returns>
    private string PutUnaryOperationsAhead( string strFormula )
    {
      if( strFormula == null )
        throw new ArgumentNullException( "strFormula" );

      int iteration = strFormula.Length;

      while( strFormula[ strFormula.Length - 1 ] == '%' )
      {
        string left = GetLeftUnaryOperand( strFormula, strFormula.Length - 1 );
        int newPos = strFormula.Length - left.Length - 1;
        strFormula = strFormula.Insert( newPos, "%" );
        strFormula = strFormula.Substring( 0, strFormula.Length - 1 );
        iteration--;

        if( iteration == 0 )
          throw new ArgumentException( "strFormula" );
      }

      return strFormula;
    }
     /// <summary>
    /// Registers function in internal collections.
    /// </summary>
    /// <param name="functionName">
    /// Name of the function that must be registered.
    /// </param>
    /// <param name="index">
    /// Index of the function that must be registered.
    /// </param>
    /// <param name="paramIndexes">
    /// Array of ReferenceIndexAttribute that contains information 
    /// about proper token index.
    /// </param>
    /// <param name="paramCount">
    /// Number of parameters in the function,
    /// -1, for variable parameters.
    /// </param>
    [ CLSCompliant( false ) ]
    public static void EditRegisteredFunction( string functionName, ExcelFunction index, 
      ReferenceIndexAttribute[] paramIndexes, int paramCount )
    {

        FunctionAliasToId.Remove(functionName);
        FunctionIdToIndex.Remove(index);
        FunctionIdToAlias.Remove(index);
        FunctionIdToParamCount.Remove(index);

        RegisterFunction(functionName, index, paramIndexes, paramCount);
    }

    #endregion

    #region Public properties
    /// <summary>
    /// Returns IDictionary error code - to - name. Read-only.
    /// </summary>
    public static Dictionary<int, string> ErrorCodeToName
    {
      get
      {
        return s_hashErrorCodeToName;
      }
    }
    /// <summary>
    /// Returns IDictionary error name - to - error code. Read-only.
    /// </summary>
    public static Dictionary<string, int> ErrorNameToCode
    {
      get
      {
        return s_hashNameToErrorCode;
      }
    }
    /// <summary>
    /// Returns array row separator. Read-only.
    /// </summary>
    public string ArrayRowSeparator
    {
      get
      {
        return m_strArrayRowSeparator;
      }
    }
    /// <summary>
    /// Returns operands separator. Read-only.
    /// </summary>
    public string OperandsSeparator
    {
      get
      {
        return m_strOperandsSeparator;
      }
    }
    /// <summary>
    /// Gets or sets number format for parsing double value.
    /// </summary>
    public NumberFormatInfo NumberFormat
    {
      get
      {
        return m_numberFormat;
      }
      set
      {
        m_numberFormat = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public IWorkbook ParentWorkbook
    {
      get
      {
        return m_book;
      }
    }
    #endregion
    /// <summary>
    /// Indicates whether specified function is supported just in Excel 2013.
    /// </summary>
    /// <param name="functionIndex">Function id.</param>
    /// <returns>Value indicating whether specified function appeared in Excel 2013.</returns>
    public static bool IsExcel2013Function(ExcelFunction functionIndex)
    {
        return Array.IndexOf<ExcelFunction>(m_excel2013Supported, functionIndex) >= 0;
    }
    /// <summary>
    /// Indicates whether specified function is supported just in Excel 2010.
    /// </summary>
    /// <param name="functionIndex">Function id.</param>
    /// <returns>Value indicating whether specified function appeared in Excel 2010.</returns>
    public static bool IsExcel2010Function( ExcelFunction functionIndex )
    {
      return Array.IndexOf<ExcelFunction>( m_excel2010Supported, functionIndex ) >= 0;
    }
    /// <summary>
    /// Indicates whether specified function is supported just in Excel 2007.
    /// </summary>
    /// <param name="functionIndex">Function id.</param>
    /// <returns>Value indicating whether specified function appeared in Excel 2007.</returns>
    public static bool IsExcel2007Function( ExcelFunction functionIndex )
    {
      return Array.IndexOf<ExcelFunction>( m_excel2007Supported, functionIndex ) >= 0;
    }
    /// <summary>
    /// Checks whether any of these tokens has external reference.
    /// </summary>
    /// <param name="ptg"></param>
    /// <returns></returns>
    internal bool HasExternalReference( Ptg[] ptg )
    {
      if( ptg == null )
        return false;

      bool result = false;

      for( int i = 0, len = ptg.Length; i < len; i++ )
      {
        Ptg token = ptg[ i ];

        IReference reference = token as IReference;

        if( reference != null && m_book.IsExternalReference( reference.RefIndex ) )
        {
          result = true;
          break;
        }
      }

      return result;
    }
  }

  /// <summary>
  /// This class is used when formula value needs to be
  /// evaluated. Provides range that contains formula that should 
  /// be evaluated and Ptg array - parsed formula string.
  /// </summary>
  public class EvaluateEventArgs : EventArgs
  {
    #region Class members
    /// <summary>
    /// Range that raised this event. Read-only.
    /// </summary>
    private IRange m_range;
    /// <summary>
    /// Ptg array with formula tokens of the range.
    /// </summary>
    private Ptg[]  m_FormulaTokens;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    private EvaluateEventArgs()
    {
    }
    /// <summary>
    /// Main constructor.
    /// </summary>
    /// <param name="range">Range containing formula.</param>
    /// <param name="array">Formula tokens array.</param>
    public EvaluateEventArgs( IRange range, Ptg[] array )
    {
      m_range = range;
      m_FormulaTokens = array;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Range that raised this event. Read-only.
    /// </summary>
    public IRange Range
    {
      get
      {
        return m_range;
      }
    }
    /// <summary>
    /// Ptg array with formula tokens of the range.
    /// </summary>
    public Ptg[] PtgArray
    {
      get
      {
        return m_FormulaTokens;
      }
    }
    #endregion

    /// <summary>
    /// Empty arguments (all properties with default values). Read-only.
    /// </summary>
    new public static EvaluateEventArgs Empty
    {
      get
      {
        return new EvaluateEventArgs();
      }
    }
  }

  ///<exclude/>
  /// <summary>
  /// Delegate that can be used for formula evaluation purposes.
  /// </summary>
  public delegate void EvaluateEventHandler( object sender, EvaluateEventArgs e );
}
