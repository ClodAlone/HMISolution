#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Exceptions;

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Parses formula tokens extracted by FormulaTokenizer and converts them into tokens in RPN form.
	/// </summary>
	public class FormulaParser
	{
    #region Class constants
    /// <summary>
    /// Name of if function in uppercase.
    /// </summary>
    private const string DEF_IF_FUNCTION = "IF";
    /// <summary>
    /// Options value for tAttr token in the case of space token.
    /// </summary>
    private const int DEF_SPACE_OPTIONS = 64;
    /// <summary>
    /// Data value for tAttr token in the case of space token.
    /// </summary>
    private const int DEF_SPACE_DATA = 256;
    /// <summary>
    /// Default options for ExternName that is used as part of DDE link.
    /// </summary>
    private const int DDELinkNameOptions = 32738;
    /// <summary>
    /// Represents the Abosulte cell references.
    /// </summary>
    private const char AbsoluteCellReference = '$';
    #endregion

    #region Class members
    /// <summary>
    /// Converts string into set of tokens.
    /// </summary>
    private FormulaTokenizer m_tokenizer;
    /// <summary>
    /// Array with tokens in RPN order.
    /// </summary>
    private List<Ptg> m_arrTokens = new List<Ptg>();
    /// <summary>
    /// 
    /// </summary>
    private Stack<AttrPtg> m_tokenSpaces = new Stack<AttrPtg>();
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of the formula parser.
    /// </summary>
    /// <param name="book">Parent workbook object.</param>
    public FormulaParser( WorkbookImpl book )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      m_book = book;
      m_tokenizer = new FormulaTokenizer( book );
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Sets formula separators.
    /// </summary>
    /// <param name="operandsSeparator">Operand separator to set.</param>
    /// <param name="arrayRowsSeparator">Array rows separator to set.</param>
    public void SetSeparators( char operandsSeparator, char arrayRowsSeparator )
    {
      m_tokenizer.ArgumentSeparator = operandsSeparator;
    }
    /// <summary>
    /// Parses formula string.
    /// </summary>
    /// <param name="formula">String to parse.</param>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="arguments">Constant arguments required by some parse methods.</param>
    public void Parse( string formula, Dictionary<Type, ReferenceIndexAttribute> indexes, int i,
      ExcelParseFormulaOptions options, ParseParameters arguments )
    {
      if( formula == null )
        throw new ArgumentNullException( "formula" );

      //formula = formula.Replace( "_xlfn.", "" );
      formula = formula.TrimEnd( ' ' );

      if( formula.Length == 0 )
        throw new ArgumentException( "formula - string cannot be empty." );
      CultureInfo cultureInfo = m_book.AppImplementation.CheckAndApplySeperators();
      m_tokenizer.NumberFormat = cultureInfo.NumberFormat;
      m_arrTokens.Clear();
      m_tokenSpaces.Clear();
      m_tokenizer.Prepare( formula );
      m_tokenizer.NextToken();

      AttrPtg tokenSpace = null;
      ParseExpression( Priority.None, indexes, i, options, arguments );

      if (m_tokenSpaces.Count > 0)
      {
          tokenSpace = m_tokenSpaces.Pop();
          tokenSpace.SpaceAfterToken = true;
          m_arrTokens.Add(tokenSpace);
          tokenSpace = null;
      }
    }
    /// <summary>
    /// Parses expression.
    /// </summary>
    /// <param name="priority">Subexpression priority.</param>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="arguments">Constant arguments required by some parse methods.</param>
    /// <returns></returns>
    private Ptg ParseExpression( Priority priority, Dictionary<Type, ReferenceIndexAttribute> indexes, int i,
      ExcelParseFormulaOptions options, ParseParameters arguments )
    {
      Ptg code2 = null;
      FormulaToken curTokenType;

      code2 = ParseFirstOperand( priority, indexes, i, ref options, arguments );

      m_arrTokens.Add( code2 );
      AttrPtg tokenSpace = null;

      if( code2 == null )
      {
        m_tokenizer.RaiseUnexpectedToken( "No Expression found" );
      }

      while( true )
      {
        curTokenType = m_tokenizer.TokenType;

        switch( curTokenType )
        {
          case FormulaToken.EndOfFormula:
            return code2;

          case FormulaToken.tAdd:
            if( priority >= Priority.PlusMinus )
            {
              if( tokenSpace != null )
              {
                m_tokenSpaces.Push( tokenSpace );
                tokenSpace = null;
              }

              return code2;
            }

            m_tokenizer.NextToken();
            ParseExpression( Priority.PlusMinus, indexes, i, options, arguments );
            code2 = CreateBinaryOperation( curTokenType, ref tokenSpace );
            break;

          case FormulaToken.tSub:
            if( priority >= Priority.PlusMinus )
            {
              if( tokenSpace != null )
              {
                m_tokenSpaces.Push( tokenSpace );
                tokenSpace = null;
              }

              return code2;
            }

            m_tokenizer.NextToken();
            ParseExpression( Priority.PlusMinus, indexes, i, options, arguments );
            code2 = CreateBinaryOperation( curTokenType, ref tokenSpace );
            break;

          case FormulaToken.tPercent:
            m_tokenizer.NextToken();
            code2 = new UnaryOperationPtg( "%" );
            m_arrTokens.Add( code2 );
            break;

          case FormulaToken.tPower:
            if( priority >= Priority.Power )
            {
              return code2;
            }

            m_tokenizer.NextToken();
            ParseExpression( Priority.Power, indexes, i, options, arguments );
            code2 = CreateBinaryOperation( FormulaToken.tPower, ref tokenSpace );
            break;

          case FormulaToken.tConcat:
            if( priority >= Priority.Concat )
            {
              return code2;
            }

            m_tokenizer.NextToken();
            ParseExpression( Priority.Concat, indexes, i, options, arguments );
            //code2 = new BinaryOperationPtg( FormulaToken.tConcat );
            code2 = CreateBinaryOperation( FormulaToken.tConcat, ref tokenSpace );
            break;

          case FormulaToken.tCellRange:
            if( priority >= Priority.CellRange )
            {
              return code2;
            }

            UpdateOptions( ref options );
            m_tokenizer.NextToken();
            ParseExpression( Priority.CellRange, indexes, i, options, arguments );
            code2 = CreateBinaryOperation( FormulaToken.tCellRange, ref tokenSpace );
            break;

          case FormulaToken.tMul:
          case FormulaToken.tDiv:
            if( priority >= Priority.MulDiv )
            {
              return code2;
            }

            UpdateOptions( ref options );
            m_tokenizer.NextToken();
            ParseExpression( Priority.MulDiv, indexes, i, options, arguments );
            string strOperation = ( curTokenType == FormulaToken.tMul )
              ? "*"
              : "/";
            code2 = CreateBinaryOperation( curTokenType, ref tokenSpace );
            break;

          //        case FormulaToken.OpenParenthesis:
          //        case FormulaToken.Comma:
          //        case FormulaToken.Dot:
          case FormulaToken.CloseParenthesis:
            if( tokenSpace != null )
            {
              m_tokenSpaces.Push( tokenSpace );

              //tokenSpace.SpaceAfterToken = true;
              //m_arrTokens.Add( tokenSpace );
              tokenSpace = null;
            }
            //else if( m_tokenSpaces.Count > 0 )
            //{
            //  AttrPtg space = ( AttrPtg )m_tokenSpaces.Pop();
            //  space.SpaceAfterToken = true;
            //  m_arrTokens.Add( space );
            //}

            return code2;
          //        case FormulaToken.OperatorNot:
          //        case FormulaToken.OperatorIf:
          //        case FormulaToken.ValueIdentifier:
          //        case FormulaToken.ValueTrue:
          //        case FormulaToken.ValueFalse:
          //          return code2;
          //
          case FormulaToken.tNotEqual:
          case FormulaToken.tGreater:
          case FormulaToken.tGreaterEqual:
          case FormulaToken.tEqual:
          case FormulaToken.tLessEqual:
          case FormulaToken.tLessThan:
            if( priority >= Priority.Equality )
            {
              return code2;
            }

            curTokenType = m_tokenizer.TokenType;
            m_tokenizer.NextToken();
            ParseExpression( Priority.Equality, indexes, i, options, arguments );
            code2 = CreateBinaryOperation( curTokenType, ref tokenSpace );
            break;

          case FormulaToken.Space:
            tokenSpace = ParseSpaces( indexes, i, options, arguments );
            continue;

          case FormulaToken.Comma:
            if( ( options & ( ExcelParseFormulaOptions.ParseOperand
              | ExcelParseFormulaOptions.ParseComplexOperand ) ) == 0 )
            {
              m_tokenizer.NextToken();
              ParseExpression( Priority.None, indexes, i, options, arguments );
              code2 = new CellRangeListPtg( m_tokenizer.ArgumentSeparator.ToString() );
              m_arrTokens.Add( code2 );
              break;
            }
            else
            {
              return code2;
            }
            case FormulaToken.tParentheses:
            return code2;

          default:
            m_tokenizer.RaiseUnexpectedToken( "Unexpected token." );
            break;
        }

        tokenSpace = null;
//        if( m_tokenSpaces.Count > 0 )
//          m_tokenSpaces.Pop();
      }

      //return code2;
    }
    /// <summary>
    /// Creates binary operation.
    /// </summary>
    /// <param name="tokenType">Type of the token to create.</param>
    /// <param name="tokenSpace">Space token that can be added before operation if not null,
    /// becomes null after adding to the tokens array.</param>
    /// <returns>Created operation.</returns>
    private Ptg CreateBinaryOperation( FormulaToken tokenType, ref AttrPtg tokenSpace )
    {
      if( tokenSpace != null )
      {
        m_arrTokens.Add( tokenSpace );
        tokenSpace = null;
      }
      else if( m_tokenSpaces.Count > 0 )
      {
        m_arrTokens.Add( m_tokenSpaces.Pop() );
      }

      Ptg operation = FormulaUtil.CreatePtgByType( tokenType );
      m_arrTokens.Add( operation );
      return operation;
    }
    /// <summary>
    /// Parses spaces and creates required tokens.
    /// </summary>
    /// <param name="indexes"></param>
    /// <param name="i"></param>
    /// <param name="options"></param>
    /// <param name="arguments"></param>
    /// <returns>Space token if any was created.</returns>
    private AttrPtg ParseSpaces( Dictionary<Type, ReferenceIndexAttribute> indexes, int i, ExcelParseFormulaOptions options,
      ParseParameters arguments )
    {
      string strToken = m_tokenizer.TokenString;
      int iTokenLength = strToken.Length;
      int iSpaceCount = 0;
      Ptg operation = null;
      bool bNeedNextToken = true;

      switch( m_tokenizer.PreviousTokenType )
      {
        case FormulaToken.CloseParenthesis:
        case FormulaToken.Identifier:
          m_tokenizer.NextToken();
          bNeedNextToken = false;

          FormulaToken tokenType = m_tokenizer.TokenType;

          if( tokenType == FormulaToken.Identifier || tokenType == FormulaToken.tParentheses || tokenType==FormulaToken.Identifier3D )
          {
            iSpaceCount = iTokenLength - 1;
            ParseExpression( Priority.None, indexes, i, options, arguments );

//            if( tokenSpace != null )
//              throw new NotSupportedException( "Unexpected token space" );

            operation = FormulaUtil.CreatePtgByType( FormulaToken.tCellRangeIntersection );//new BinaryOperationPtg( " " );
            m_arrTokens.Add( operation );
          }
          else
          {
            goto default;
          }
          break;

        default:
          iSpaceCount = iTokenLength;

          if( bNeedNextToken ) m_tokenizer.NextToken();
          break;
      };

      AttrPtg token = null;

      if( iSpaceCount > 0 )
      {
        token = CreateSpaceToken( iSpaceCount );

        if( operation != null )
        {
          m_arrTokens.Insert( m_arrTokens.Count - 1, token );
          token = null;
        }
      }

//      if( token != null )
//        m_tokenSpaces.Push( token );

      return token;
    }
    /// <summary>
    /// Creates spaces token.
    /// </summary>
    /// <param name="spaceCount">Number of spaces.</param>
    /// <returns>Created space token.</returns>
    private AttrPtg CreateSpaceToken( int spaceCount )
    {
      AttrPtg attr = ( AttrPtg )FormulaUtil.CreatePtg( FormulaToken.tAttr,
        DEF_SPACE_OPTIONS, DEF_SPACE_DATA );

      attr.SpaceCount = spaceCount;
      return attr;
      //m_arrTokens.Add( attr );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="priority"></param>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="arguments">Constant arguments required by some parse methods.</param>
    /// <returns></returns>
    private Ptg ParseFirstOperand( Priority priority, Dictionary<Type, ReferenceIndexAttribute> indexes, int i, 
      ref ExcelParseFormulaOptions options, ParseParameters arguments )
    {
      Ptg result = null;
      bool bContinue;
      AttrPtg space = null;
      do
      {
        bContinue = false;

        switch( m_tokenizer.TokenType )
        {
          case FormulaToken.tAdd:
            m_tokenizer.NextToken();

            if( space == null && m_tokenSpaces.Count > 0 )
              m_arrTokens.Add( m_tokenSpaces.Pop() );

            if( ( options & ExcelParseFormulaOptions.ParseOperand ) != 0 )
            {
              options -= ExcelParseFormulaOptions.ParseOperand;
              options |= ExcelParseFormulaOptions.ParseComplexOperand;
            }

            result = ParseExpression( Priority.UnaryMinus, indexes, i, options, arguments );
            result = new UnaryOperationPtg( "+" );
            break;

          case FormulaToken.tSub:
            m_tokenizer.NextToken();

            if( space == null && m_tokenSpaces.Count > 0 )
              m_arrTokens.Add( m_tokenSpaces.Pop() );

            if( ( options & ExcelParseFormulaOptions.ParseOperand ) != 0 )
            {
              options -= ExcelParseFormulaOptions.ParseOperand;
              options |= ExcelParseFormulaOptions.ParseComplexOperand;
            }

            result = ParseExpression( Priority.UnaryMinus, indexes, i, options, arguments );
            result = new UnaryOperationPtg( "-" );
            break;

          case FormulaToken.tParentheses:
            if( space != null )
            {
              m_arrTokens.RemoveAt( m_arrTokens.Count - 1 );
            }

            m_tokenizer.NextToken();
            ExcelParseFormulaOptions modifiedOptions = options & (~ExcelParseFormulaOptions.ParseOperand );
            result = ParseExpression( Priority.None, indexes, i, modifiedOptions, arguments );

            if( m_tokenizer.TokenType != FormulaToken.CloseParenthesis )
            {
              m_tokenizer.RaiseUnexpectedToken( "End parenthesis not found" );
            }

            // TODO: maybe we will need to change some logic here.
            if( space != null )
            {
              m_arrTokens.Add( space );
            }
            else if( m_tokenSpaces.Count > 0 )
            {
              space = ( AttrPtg )m_tokenSpaces.Pop();
              space.SpaceAfterToken = true;
              m_arrTokens.Add( space );
            }
              //m_arrTokens.Add( m_tokenSpaces.Pop() );
            //}

            result = new ParenthesesPtg();
            m_tokenizer.NextToken();
            break;

          case FormulaToken.tFunction1:
            result = ParseFunction( indexes, i, options, arguments );
            break;

          case FormulaToken.Identifier:
          case FormulaToken.Identifier3D:
            string strIdentifier = m_tokenizer.TokenString;
            if (m_tokenizer.TokenString == ("Overview!" + RefErrorPtg.ReferenceError))
                m_tokenizer.TokenType = FormulaToken.tError;
            m_tokenizer.NextToken();
            UpdateOptions( ref options );
            result = ParseIdentifier( strIdentifier, indexes, i, options, arguments );
            //m_tokenizer.NextToken();
            break;

          case FormulaToken.DDELink:
            result = ParseDDELink( indexes, i, options, arguments );
            break;

          case FormulaToken.ValueTrue:
            result = new BooleanPtg( true );
            m_tokenizer.NextToken();
            break;

          case FormulaToken.ValueFalse:
            if( space == null && m_tokenSpaces.Count > 0 )
              m_arrTokens.Add( m_tokenSpaces.Pop() );

            result = new BooleanPtg( false );
            m_tokenizer.NextToken();
            break;

          case FormulaToken.tNumber:
            try
            {
                double dValue = double.Parse(m_tokenizer.TokenString, NumberStyles.Float,(IFormatProvider)m_tokenizer.NumberFormat);
              result = new DoublePtg( dValue );
            }
            catch( Exception ex )
            {
              m_tokenizer.RaiseException( string.Format( "Invalid number {0}", m_tokenizer.TokenString ), ex );
            }

            if( space == null && m_tokenSpaces.Count > 0 )
              m_arrTokens.Add( m_tokenSpaces.Pop() );

            m_tokenizer.NextToken();
            UpdateOptions( ref options );
            break;

          case FormulaToken.tInteger:
            ushort usResult;

            if( ushort.TryParse( m_tokenizer.TokenString, NumberStyles.Float,(IFormatProvider)m_tokenizer.NumberFormat, out usResult ) )
            {
              result = new IntegerPtg( usResult );

              if( space == null && m_tokenSpaces.Count > 0 )
                m_arrTokens.Add( m_tokenSpaces.Pop() );

              m_tokenizer.NextToken();
              UpdateOptions( ref options );
            }
            else
            {
              goto case FormulaToken.tNumber;
            }
            break;

          case FormulaToken.tStringConstant:
            if( space == null && m_tokenSpaces.Count > 0 )
              m_arrTokens.Add( m_tokenSpaces.Pop() );

            result = new StringConstantPtg( m_tokenizer.TokenString );
            m_tokenizer.NextToken();
            break;

          case FormulaToken.tError:
            if( space == null && m_tokenSpaces.Count > 0 )
              m_arrTokens.Add( m_tokenSpaces.Pop() );

            result = ParseError( indexes, i, options, arguments.Worksheet );
            m_tokenizer.NextToken();
            break;

          case FormulaToken.tArray1:
            if( space == null && m_tokenSpaces.Count > 0 )
              m_arrTokens.Add( m_tokenSpaces.Pop() );

            int iRefIndex = FormulaUtil.GetIndex( typeof( ArrayPtg ), FormulaUtil.DEF_TYPE_VALUE,
              indexes, i, options );
            FormulaToken token = ArrayPtg.IndexToCode( iRefIndex );

            result = ParseArray( token, arguments );
            m_tokenizer.NextToken();
            break;

          case FormulaToken.Space:
            // TODO: we have to process spaces somehow, in the current implementation we just skip them.
            space = ( AttrPtg )FormulaUtil.CreatePtg( FormulaToken.tAttr,
              DEF_SPACE_OPTIONS, DEF_SPACE_DATA );

            space.SpaceCount = m_tokenizer.TokenString.Length;
            //m_tokenSpaces.Push( space );
            m_arrTokens.Add( space );
            m_tokenizer.NextToken();
            bContinue = true;
            break;

          case FormulaToken.Comma:
            if( space == null && m_tokenSpaces.Count > 0 )
              m_arrTokens.Add( m_tokenSpaces.Pop() );

            result = FormulaUtil.CreatePtg( FormulaToken.tMissingArgument );
            //m_tokenizer.NextToken();
            break;
        }
      }
      while( bContinue );

      return result;
    }
    /// <summary>
    /// Updates parsing options if necessary.
    /// </summary>
    /// <param name="options">Options to update.</param>
    private void UpdateOptions( ref ExcelParseFormulaOptions options )
    {
      if( ( options & ExcelParseFormulaOptions.ParseOperand ) != 0 )
      {
        FormulaToken tokenType = m_tokenizer.TokenType;

        if( tokenType != FormulaToken.Comma
          && tokenType != FormulaToken.EndOfFormula
          && tokenType != FormulaToken.CloseParenthesis
          && tokenType != FormulaToken.Space
          && tokenType != FormulaToken.tCellRange )
        {
          options -= ExcelParseFormulaOptions.ParseOperand;
          options |= ExcelParseFormulaOptions.ParseComplexOperand;
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="indexes"></param>
    /// <param name="i"></param>
    /// <param name="options"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    private Ptg ParseDDELink( Dictionary<Type, ReferenceIndexAttribute> indexes, int i, ExcelParseFormulaOptions options,
      ParseParameters arguments )
    {
      if( arguments == null )
        throw new ArgumentNullException( "arguments" );

      string strDDELink = m_tokenizer.TokenString;
      m_tokenizer.NextToken();

      string strParamName = m_tokenizer.TokenString;
      m_tokenizer.NextToken();

      string strName = m_tokenizer.TokenString;
      m_tokenizer.TokenType = FormulaToken.None;
      m_tokenizer.NextToken();

      return CreateDDELink( strDDELink, strParamName, strName, indexes, i, options, arguments );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strDDELink"></param>
    /// <param name="strParamName"></param>
    /// <param name="strName"></param>
    /// <param name="indexes"></param>
    /// <param name="i"></param>
    /// <param name="options"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    private Ptg CreateDDELink( string strDDELink, string strParamName, string strName,
      Dictionary<Type, ReferenceIndexAttribute> indexes, int i, ExcelParseFormulaOptions options, ParseParameters arguments )
    {
      string strSupBookName = strDDELink + "|" + strParamName;
      WorkbookImpl book = ( WorkbookImpl )arguments.Workbook;
      ExternBookCollection arrExternBooks = book.ExternWorkbooks;
      ExternWorkbookImpl externBook = arrExternBooks[ strSupBookName ];
      int iExternBookIndex = -1;

      if( externBook == null )
      {
        iExternBookIndex = arrExternBooks.AddDDEFile( strSupBookName );
        externBook = arrExternBooks[ iExternBookIndex ];
      }
      else
      {
        iExternBookIndex = externBook.Index;
      }

      ExternNamesCollection arrNames = externBook.ExternNames;
      ExternNameImpl externName;
      int iNameIndex = arrNames.GetNameIndex( strName );

      if( iNameIndex < 0 )
      {
        iNameIndex = arrNames.Add( strName );
      }

      externName = arrNames[ iNameIndex ];
      externName.Record.Options = DDELinkNameOptions;

      int iRefIndex = FormulaUtil.GetIndex( typeof( ArrayPtg ), FormulaUtil.DEF_TYPE_VALUE,
        indexes, i, options );
      FormulaToken token = NameXPtg.IndexToCode( iRefIndex );
      NameXPtg result = ( NameXPtg )FormulaUtil.CreatePtg( token );
      result.NameIndex = ( ushort )( iNameIndex + 1 );
      result.RefIndex = ( ushort )iExternBookIndex;

      return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifier">Identifier to parse.</param>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="arguments">Constant arguments required by some parse methods.</param>
    /// <returns>Created token for the specified identifier.</returns>
    private Ptg ParseIdentifier( string identifier, Dictionary<Type, ReferenceIndexAttribute> indexes, int i,
      ExcelParseFormulaOptions options, ParseParameters arguments )
    {
      // Possible value are:
      // 1. Function ->if / built-in / custom
      // 2. Range (cell/area + local, with sheet name, with book name).
      // 3. Named range (glogal or with sheet or book name).
      if( identifier == null )
        throw new ArgumentNullException( "identifier" );

      if( identifier.Length == 0 )
        throw new ArgumentException( "identifier - string cannot be empty." );

      Ptg result = null;
      int iRefIndex = -1;

      if( m_tokenizer.PreviousTokenType == FormulaToken.Identifier3D )
      {
        int iExclamationIndex = identifier.LastIndexOf( '!' );

        if( iExclamationIndex <= 0 )
          throw new ArgumentOutOfRangeException( "identifier" );

        string strLocation = identifier.Substring( 0, iExclamationIndex );
        identifier = identifier.Substring( iExclamationIndex + 1 );
        iRefIndex = ConvertLocationIntoReference( strLocation, arguments );
      }

      /*object oFunctionId = FormulaUtil.FunctionAliasToId[ strToken ];

      if( oFunctionId != null )
      {
        ExcelFunction functionId = ( ExcelFunction )oFunctionId;
        result = CreateFunction( functionId, sheet, indexes, i, hashWorksheetNames,
          bR1C1, options, iCellRow, iCellColumn );
      }
      else*/
      if (TryGetNamedRange(identifier, arguments, indexes, i, options, iRefIndex,out result))
      {
         
      }
      else if( TryCreateRange( identifier, indexes, i, options, arguments, out result, iRefIndex ) )
      {
        //throw new NotImplementedException();
      }
      else
      {
        //IWorksheet sheet = arguments.Worksheet;
        result = CreateNamedRange( identifier, arguments, indexes, i, options, iRefIndex );
      }

      return result;
    }
/// <summary>
    /// Tries to get named range object based on the formula string.
    /// </summary>
    /// <param name="strToken"></param>
    /// <param name="arguments"></param>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="iRefIndex">Location reference index; -1 means local sheet reference.</param>
    /// <returns></returns>
    private bool TryGetNamedRange(string strToken, ParseParameters arguments,
      Dictionary<Type, ReferenceIndexAttribute> indexes, int i, ExcelParseFormulaOptions options, int iRefIndex,out Ptg result)
    {
        if (arguments == null)
            throw new ArgumentNullException("arguments");

        if (strToken == null || strToken.Length == 0)
            throw new ArgumentOutOfRangeException("strToken");
        // Here we have to create named range or throw exception if name is unknown
        // and new named range creation is forbidden.
        WorkbookImpl book = (WorkbookImpl)arguments.Workbook;
        result = null;
        if(iRefIndex < 0 || book.IsLocalReference(iRefIndex)) 
        {
            IWorksheet sheet = arguments.Worksheet;
            IName name;
            INames namesToAddInto = null;

            if (iRefIndex == -1)
            {
                name = (sheet != null) ?
                  sheet.Names[strToken] :
                    name = book.Names[strToken];

            }
            else
            {
                IWorksheet nameSheet = book.GetSheetByReference(iRefIndex, false);

                namesToAddInto = (nameSheet != null) ?
                  nameSheet.Names :
                  book.Names;

                name = namesToAddInto[strToken];
            }
            if(name!=null)
                result=CreateNameToken(iRefIndex, name.Index, arguments, indexes, i, options);
        }
        else
        {
            int iExternBookIndex = book.GetBookIndex(iRefIndex);
            ExternWorkbookImpl externBook = book.ExternWorkbooks[iExternBookIndex];
            ExternNamesCollection externNames = externBook.ExternNames;
            int iNameIndex = externNames.GetNameIndex(strToken);

            if (iNameIndex >= 0)
                result= CreateNameToken(iRefIndex, iNameIndex, arguments, indexes, i, options);
        }
        return (result != null);
    }
    /// <summary>
    /// Converts location string into reference.
    /// </summary>
    /// <param name="location">Location to convert.</param>
    /// <param name="arguments">Constant arguments required by some parse methods.</param>
    /// <returns>Reference index.</returns>
    private int ConvertLocationIntoReference( string location, ParseParameters arguments )
    {
      // 1. - sheet + book + path
      // 2. - sheet + book
      // 3. - local worksheet

      int iLength = location.Length;

      if( location[ 0 ] == '\'' && location[ iLength - 1 ] == '\'' )
      {
        location = location.Substring( 1, iLength - 2 );
      }

      string strSheetName = null;
      string strBook = null;
      string strBookPath = null;

      int iBookPathEnd = location.IndexOf( '[' );
      int iBookNameEnd = location.IndexOf( ']' );
      int iSheetNameStart = ( iBookNameEnd > 0 ) ? iBookNameEnd + 1 : 0;

      if( iBookPathEnd > 0 )
        strBookPath = location.Substring( 0, iBookPathEnd );

      if( iBookNameEnd > 0 )
        strBook = location.Substring( iBookPathEnd + 1, iBookNameEnd - iBookPathEnd - 1 );

      int iReference = -1;

      strSheetName = location.Substring( iSheetNameStart );
      WorkbookImpl book = ( WorkbookImpl )arguments.Workbook;
      int iSupBookIndex = -1;
                
      if( strBook == null )
      {
        // local item
        iReference = book.AddSheetReference( strSheetName );
      }
      else
      {
          
        ExternWorkbookImpl externBook;

        if( book.Loading && strBookPath == null && int.TryParse( strBook, out iSupBookIndex ) )
        {
          if( iSupBookIndex == 0 )
          {
            iSupBookIndex = book.ExternWorkbooks.InsertSelfSupbook() + 1;
          }

          externBook = book.ExternWorkbooks[ iSupBookIndex - 1 ];
        }
        else
        {
          // TODO: implement this.
          // 1. Find or add external workbook
          externBook = book.ExternWorkbooks.FindOrAdd( strBook, strBookPath );
        }
        // 2. Find or add external worksheet into workbook.
        int iSheetIndex = ( strSheetName != null && strSheetName.Length > 0 ) ?
          externBook.FindOrAddSheet( strSheetName ) :
          WorkbookImpl.DEF_BOOK_SHEET_INDEX;//65534;

        iReference = m_book.AddSheetReference( externBook.Index, iSheetIndex, iSheetIndex );

        // 3. Find or add external name?
        //throw new NotImplementedException();
        // TODO: ON the current moment we don't support external names 
      }

      return iReference;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="arguments">Constant arguments required by some parse methods.</param>
    /// <returns></returns>
    private Ptg ParseFunction( Dictionary<Type, ReferenceIndexAttribute> indexes, int i, ExcelParseFormulaOptions options,
      ParseParameters arguments )
    {
      if( ( options & ExcelParseFormulaOptions.ParseComplexOperand ) != 0 )
      {
        options -= ExcelParseFormulaOptions.ParseComplexOperand;
        options |= ExcelParseFormulaOptions.ParseOperand;
      }

      // We can have following cases:
      // 1. IF
      // 2. User defined
      // 3. Built-in.

      // We store function name in upper-case.
      string strToken = m_tokenizer.TokenString.ToUpper();

      if( strToken.StartsWith( FormulaUtil.Excel2010FunctionPrefix ) )
      {
        strToken.Replace( FormulaUtil.Excel2010FunctionPrefix, string.Empty );
      }

      Ptg result = null;
      AttrPtg spaceToken = null;

      if( m_tokenizer.PreviousTokenType == FormulaToken.Space )
      {
        int iLastIndex = m_arrTokens.Count - 1;
        AttrPtg token = ( AttrPtg )m_arrTokens[ iLastIndex ];

        if( !token.SpaceAfterToken )
        {
          spaceToken = token;
          m_arrTokens.RemoveAt( iLastIndex );
        }
      }

      int iBookIndex;
      int iNameIndex;

      if( strToken == DEF_IF_FUNCTION )
      {
        result = CreateIFFunction( indexes, i, options, arguments, spaceToken );
        spaceToken = null;
      }
      else
      {
        ExcelFunction functionId;

        if( FormulaUtil.FunctionAliasToId.TryGetValue( strToken, out functionId ) )
        {
          //ExcelFunction functionId = ( ExcelFunction )oFunctionId;
          if( IsFunctionSupported( functionId, m_book.Version ) )
          {
            result = CreateFunction( functionId, indexes, i, options, arguments );
          }
          else
          {
            result = CreateCustomFunction( indexes, i, options, arguments, true );
          }
        }
        else if ( FormulaUtil.IsCustomFunction( m_tokenizer.TokenString, arguments.Workbook,
          out iBookIndex, out iNameIndex ) ||
          !TryCreateFunction2007( ref result, indexes, i, options, arguments ) )
        {
          // Here we have try to create custom function call.
          result = CreateCustomFunction( indexes, i, options, arguments, false );
          spaceToken = null;
        }
      }

      if( spaceToken != null )
      {
        m_arrTokens.Add( spaceToken );
      }
      else if( m_tokenSpaces.Count > 0 )
      {
        spaceToken = m_tokenSpaces.Pop();
        spaceToken.AttrData1 = 4;
        m_arrTokens.Add( spaceToken );
      }

      spaceToken = null;

      return result;
    }

    private bool IsFunctionSupported( ExcelFunction functionId, ExcelVersion excelVersion )
    {
      bool result = true;

      if (excelVersion < ExcelVersion.Excel2013)
      {
          if (FormulaUtil.IsExcel2013Function(functionId))
          {
              result = false;
          }
          else if (excelVersion < ExcelVersion.Excel2010)
          {
              if (FormulaUtil.IsExcel2010Function(functionId))
              {
                  result = false;
              }
              else if (excelVersion < ExcelVersion.Excel2007)
              {
                  if (FormulaUtil.IsExcel2007Function(functionId))
                  {
                      result = false;
                  }
                  if (m_book.ExternWorkbooks.ContainsExternName((functionId.ToString())))
                  {
                      result = false;
                  }
              }
          }
      }

      return result;
    }
    /// <summary>
    /// Creates Excel 2007 function.
    /// </summary>
    /// <param name="result">Resulting formula token.</param>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="arguments">Constant arguments required by some parse methods.</param>
    /// <returns>True if current string was recognized as formula token.</returns>
    private bool TryCreateFunction2007( ref Ptg result, Dictionary<Type, ReferenceIndexAttribute> indexes, int i,
      ExcelParseFormulaOptions options, ParseParameters arguments )
    {
      string functionName = m_tokenizer.TokenString;
      functionName = functionName.ToUpper();

      if( arguments.Workbook.Version == ExcelVersion.Excel97to2003 ||
        !Enum.IsDefined( typeof( Excel2007Function ), functionName ) )
      {
        return false;
      }

      Excel2007Function functionId = ( Excel2007Function )Enum.Parse( typeof( Excel2007Function ), functionName, true );
      result = CreateFunction( ( ExcelFunction )functionId, indexes, i, options, arguments );
      return true;
    }
    /// <summary>
    /// This function tries to create range (cell or area) based on the formula string and other arguments.
    /// </summary>
    /// <param name="strFormula">Identifier string without location part (worksheet, book).</param>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="arguments">Constant arguments required by some parse methods.</param>
    /// <param name="resultToken">Resulting token.</param>
    /// <param name="iRefIndex">Location reference index; -1 means local sheet reference.</param>
    /// <returns>True if formula string was parsed as range.</returns>
    private bool TryCreateRange( string strFormula, Dictionary<Type, ReferenceIndexAttribute> indexes, int i,
      ExcelParseFormulaOptions options, ParseParameters arguments, out Ptg resultToken, int iRefIndex )
    {
      if( arguments == null )
        throw new ArgumentNullException( "arguments" );

      if( strFormula == null )
        throw new ArgumentNullException( "strFormula" );

      FormulaToken token;
      bool bResult = true;
      resultToken = null;
      string strCol1, strCol2, strRow1, strRow2;
      //string sheetName;
      IWorksheet sheet = arguments.Worksheet;
      WorkbookImpl book = ( WorkbookImpl )arguments.Workbook;
      bool bR1C1 = arguments.IsR1C1;
      int iCellRow = arguments.CellRow;
      int iCellColumn = arguments.CellColumn;
      Dictionary<string, string> hashWorksheetNames = arguments.WorksheetNames;

      // Check if it is cell or cell range. 
      if( m_book.FormulaUtil.IsCellRange( strFormula, bR1C1, out strRow1, out strCol1, out strRow2, out strCol2 ) )
      {
        token = AreaPtg.IndexToCode( FormulaUtil.GetIndex( typeof( AreaPtg ), FormulaUtil.DEF_TYPE_REF,
          indexes, i, options ) );

        resultToken = FormulaUtil.CreatePtg( token, iCellRow, iCellColumn, strRow1,
          strCol1, strRow2, strCol2, bR1C1, book );
      }
      else if( FormulaUtil.IsCell( strFormula, bR1C1, out strRow1, out strCol1 ) ) // Check if it is cell.
      {
          int iTokenIndex = -1;
          
          bool isAbsoluteReferece=false;
          if (strCol1 != null)
              isAbsoluteReferece = strCol1.Contains(AbsoluteCellReference.ToString());
           if (!isAbsoluteReferece && indexes!=null && indexes.ContainsKey(typeof(RefNPtg)))
           {
               iTokenIndex = FormulaUtil.GetIndex(typeof(RefNPtg), FormulaUtil.DEF_TYPE_REF,
               indexes, i, options)   +1;
               token = RefNPtg.IndexToCode(iTokenIndex);
           }
           else
           {
               iTokenIndex = FormulaUtil.GetIndex(typeof(RefPtg), FormulaUtil.DEF_TYPE_REF,
               indexes, i, options);
               token = RefPtg.IndexToCode(iTokenIndex);
           }

        resultToken = FormulaUtil.CreatePtg( token, iCellRow, iCellColumn, strRow1, strCol1, bR1C1 );//strFormula );
      }
      //else if( m_book.FormulaUtil.IsCellRange3D( strFormula, bR1C1, out sheetName, out strRow1,
      //  out strCol1, out strRow2, out strCol2 ) )                        // Check if it is 3d range.
      //{
      //  token = Area3DPtg.IndexToCode( FormulaUtil.GetIndex( typeof( Area3DPtg ), FormulaUtil.DEF_TYPE_REF,
      //    indexes, i, options ) );

      //  if( hashWorksheetNames != null && hashWorksheetNames.Contains( sheetName ) )
      //  {
      //    strFormula = strFormula.Replace( sheetName, ( string )hashWorksheetNames[ sheetName ] );
      //  }

      //  int iRefIndex = book.AddSheetReference( sheetName );
      //  resultToken = FormulaUtil.CreatePtg( token, iCellRow, iCellColumn, iRefIndex, strRow1,
      //    strCol1, strRow2, strCol2, bR1C1, m_book );
      //}
      //else if( FormulaUtil.IsCell3D( strFormula, bR1C1, out sheetName, out strRow1, out strCol1 ) ) // Check if it is 3d cell.
      //{
      //  token = Ref3DPtg.IndexToCode( FormulaUtil.GetIndex( typeof( Ref3DPtg ), FormulaUtil.DEF_TYPE_REF,
      //    indexes, i, options ) );
        
      //  if( hashWorksheetNames != null && hashWorksheetNames.Contains( sheetName ) )
      //  {
      //    sheetName =( string )hashWorksheetNames[ sheetName ];
      //  }

      //  int iRefIndex = book.AddSheetReference( sheetName );

      //  resultToken = FormulaUtil.CreatePtg( token, iCellRow, iCellColumn, iRefIndex, strRow1, strCol1, bR1C1 );
      //}
      else
      {
        bResult = false;
      }

      if( bResult && iRefIndex != -1 )
      {
        resultToken = ( resultToken as IToken3D ).Get3DToken( iRefIndex );
      }

      return bResult;
    }
    /// <summary>
    /// Tries to create named range object based on the formula string.
    /// </summary>
    /// <param name="strToken"></param>
    /// <param name="arguments"></param>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="iRefIndex">Location reference index; -1 means local sheet reference.</param>
    /// <returns></returns>
    private Ptg CreateNamedRange( string strToken, ParseParameters arguments,
      Dictionary<Type, ReferenceIndexAttribute> indexes, int i, ExcelParseFormulaOptions options, int iRefIndex )
    {
      // Here we have to create named range or throw exception if name is unknown
      // and new named range creation is forbidden.
      WorkbookImpl book = ( WorkbookImpl )arguments.Workbook;

      return ( iRefIndex < 0 || book.IsLocalReference( iRefIndex ) ) ?
        CreateLocalName( iRefIndex, strToken, arguments, indexes, i, options ) :
        CreateExternalName( iRefIndex, strToken, arguments, indexes, i, options );
    }
    /// <summary>
    /// Creates named range token for external name.
    /// </summary>
    /// <param name="iRefIndex">Location reference index.</param>
    /// <param name="strToken">Name of the named range to create token for.</param>
    /// <param name="arguments"></param>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <returns>Created token.</returns>
    private Ptg CreateExternalName( int iRefIndex, string strToken, ParseParameters arguments,
      Dictionary<Type, ReferenceIndexAttribute> indexes, int i, ExcelParseFormulaOptions options )
    {
      if( arguments == null )
        throw new ArgumentNullException( "arguments" );

      if( strToken == null )
        throw new ArgumentNullException( "strToken" );

      if( iRefIndex < 0 )
        throw new ArgumentOutOfRangeException( "iRefIndex" );

      WorkbookImpl book = ( WorkbookImpl )arguments.Workbook;
      int iExternBookIndex = book.GetBookIndex( iRefIndex );
      ExternWorkbookImpl externBook = book.ExternWorkbooks[ iExternBookIndex ];
      ExternNamesCollection externNames = externBook.ExternNames;
      int iNameIndex = externNames.GetNameIndex( strToken );

      if( iNameIndex < 0 )
        iNameIndex = externNames.Add( strToken );

      return CreateNameToken( iRefIndex, iNameIndex, arguments, indexes, i, options );
    }
    /// <summary>
    /// Creates named range token for local name.
    /// </summary>
    /// <param name="iRefIndex">Location reference index; -1 means local sheet reference.</param>
    /// <param name="strToken">Name of the named range to create token for.</param>
    /// <param name="arguments"></param>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <returns>Created token.</returns>
    private Ptg CreateLocalName( int iRefIndex, string strToken, ParseParameters arguments,
      Dictionary<Type, ReferenceIndexAttribute> indexes, int i, ExcelParseFormulaOptions options )
    {
      if( arguments == null )
        throw new ArgumentNullException( "arguments" );

      if( strToken == null || strToken.Length == 0 )
        throw new ArgumentOutOfRangeException( "strToken" );

      WorkbookImpl book = ( WorkbookImpl )arguments.Workbook;
      IWorksheet sheet = arguments.Worksheet;
      IName name;
      INames namesToAddInto = null;

      if( iRefIndex == -1 )
      {
        name = ( sheet != null ) ?
          sheet.Names[ strToken ] :
          null;

        if( name == null )
          name = book.Names[ strToken ];

        namesToAddInto = book.Names;
      }
      else
      {
        IWorksheet nameSheet = book.GetSheetByReference( iRefIndex, false );

        namesToAddInto = ( nameSheet != null ) ?
          nameSheet.Names :
          book.Names;

        name = namesToAddInto[ strToken ];
      }

      if( name == null )
      {
        // TODO: here we have to try to create named range if it is allowed or throw an exception.
        if( book.ThrowOnUnknownNames )
          throw new ParseException( strToken + " is not valid named range" );

        name = namesToAddInto.Add( strToken );
      }

      // Now we have complete information to create token.
      int iNameIndex = name.Index;
      return CreateNameToken( iRefIndex, iNameIndex, arguments, indexes, i, options );
    }
    /// <summary>
    /// Creates named range token.
    /// </summary>
    /// <param name="iRefIndex">Reference index to the token location.</param>
    /// <param name="iNameIndex">Name index.</param>
    /// <param name="arguments"></param>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <returns>Created name token.</returns>
    private Ptg CreateNameToken( int iRefIndex, int iNameIndex, ParseParameters arguments,
      Dictionary<Type, ReferenceIndexAttribute> indexes, int i, ExcelParseFormulaOptions options )
    {
      Ptg result;
      if( iRefIndex >= 0 )
      {
        FormulaToken tokenId = NameXPtg.IndexToCode( FormulaUtil.GetIndex( typeof( NameXPtg ),
          FormulaUtil.DEF_TYPE_REF, indexes, i, options ) );

        // TODO: check whether indexes are correct or 1 must be added to one/both of them.
        result = FormulaUtil.CreatePtg( tokenId, iRefIndex, iNameIndex );
      }
      else
      {
        FormulaToken tokenId = NamePtg.IndexToCode( FormulaUtil.GetIndex( typeof( NamePtg ),
          FormulaUtil.DEF_TYPE_REF, indexes, i, options ) );

        // TODO: check whether indexes are correct or 1 must be added to one/both of them.
        result = FormulaUtil.CreatePtg( tokenId, iNameIndex );
      }

      return result;
    }
    /// <summary>
    /// Creates token for local name.
    /// </summary>
    /// <param name="strNameLocation">Name location.</param>
    /// <param name="strToken">Token</param>
    /// <param name="arguments"></param>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <returns></returns>
    private Ptg CreateLocalName( string strNameLocation, string strToken,
      ParseParameters arguments, Dictionary<Type, ReferenceIndexAttribute> indexes, int i, ExcelParseFormulaOptions options )
    {
      IWorkbook book = arguments.Workbook;
      IWorksheet sheet = arguments.Worksheet;

      if( strNameLocation != null && strNameLocation.Length > 0 )
      {
        sheet = book.Worksheets[ strNameLocation ];
      }

      IName name = ( sheet != null ) ?
        sheet.Names[ strToken ] :
        null;

      if( name == null ) name = book.Names[ strToken ];

      Ptg result = null;

      if( name != null )
      {
        int iNameIndex = name.Index;

        FormulaToken tokenId = NamePtg.IndexToCode( FormulaUtil.GetIndex( typeof( NamePtg ),
          FormulaUtil.DEF_TYPE_REF, indexes, i, options ) );

        result = FormulaUtil.CreatePtg( tokenId, iNameIndex );
      }

      return result;
    }
    /// <summary>
    /// Checks whether specified location is extern.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="strLocation">Location to check.</param>
    /// <returns>True if it is extern location.</returns>
    private bool IsExternLocation( IWorkbook book, string strLocation )
    {
      if( strLocation == null || strLocation.Length == 0 ) return false;

      if( book.Worksheets[ strLocation ] != null ) return false;

      // TODO: implement this method
      return true;
    }
    /// <summary>
    /// Creates function token and adds all argument tokens to the internal token array.
    /// </summary>
    /// <param name="functionId">Id of the function to create.</param>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="arguments">Constant arguments required by some parse methods.</param>
    /// <returns>Created function token.</returns>
    private Ptg CreateFunction( ExcelFunction functionId, Dictionary<Type, ReferenceIndexAttribute> indexes,
      int i, ExcelParseFormulaOptions options, ParseParameters arguments )
    {
      FunctionPtg ptgFunction = null;

      List<int> arrOperands = ExtractOperands( options, arguments, functionId );
      int iParamCount;

      if( FormulaUtil.FunctionIdToParamCount.TryGetValue( functionId, out iParamCount ) )
      {
          if (functionId == ExcelFunction.IFERROR)
          {
              if (arrOperands.Count <= 2)
              {
                  m_arrTokens.Add(FormulaUtil.CreatePtg(FormulaToken.tMissingArgument));
                  arrOperands.Add(m_arrTokens.Count);
              }
          }

        if( iParamCount != arrOperands.Count - 1 )
        {
          m_tokenizer.RaiseException( "Wrong arguments number for function: " + functionId, null );
        }

        int iRefIndex = FormulaUtil.GetIndex( typeof( FunctionPtg ), FormulaUtil.DEF_TYPE_REF,
          indexes, i, options );

        FormulaToken token = FunctionPtg.IndexToCode( iRefIndex );
        ptgFunction = ( FunctionPtg )FormulaUtil.CreatePtg( token, functionId );
      }
      else
      {
          if (functionId == ExcelFunction.GETPIVOTDATA)
          {
              if (arrOperands.Count > 4 && ((arrOperands.Count) % 2) == 0)
              {
                  m_arrTokens.Add(FormulaUtil.CreatePtg(FormulaToken.tMissingArgument));
                  arrOperands.Add(m_arrTokens.Count);
              }
          }
        int iRefIndex = FormulaUtil.GetIndex( typeof( FunctionVarPtg ), FormulaUtil.DEF_TYPE_REF,
          indexes, i, options );

        FormulaToken token = FunctionVarPtg.IndexToCode( iRefIndex );
        ptgFunction = ( FunctionPtg )FormulaUtil.CreatePtg( token, functionId );
        ptgFunction.NumberOfArguments = ( byte )( arrOperands.Count - 1 );
      }

      return ptgFunction;
    }
    /// <summary>
    /// Creates IF function and adds all argument tokens to the internal token array.
    /// </summary>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="arguments">Constant arguments required by some parse methods.</param>
    /// <param name="spaceBeforeIf">Space token that must be inserted before IF.</param>
    /// <returns>Created function token.</returns>
    private Ptg CreateIFFunction( Dictionary<Type, ReferenceIndexAttribute> indexes, int i,
      ExcelParseFormulaOptions options, ParseParameters arguments, AttrPtg spaceBeforeIf )
    {
      List<int> arrOperands = ExtractOperands( options, arguments, ExcelFunction.IF );

      int iOperandCount = arrOperands.Count - 1;

      if( iOperandCount > 3 || iOperandCount < 2 )
        //throw new ArgumentOutOfRangeException( "Argument count for IF function must be 2 or 3" );
        m_tokenizer.RaiseException( "Argument count for IF function must be 2 or 3.", null );

      int iTrueStart = arrOperands[ 1 ];
      int iFalseStart = arrOperands[ 2 ];
      int iTrueSize = GetTokensSize( iTrueStart, arrOperands[ 2 ], arguments ) + 4;
      int iFalseSize = ( iOperandCount == 3 )
        ? GetTokensSize( iFalseStart, arrOperands[ 3 ], arguments ) + 4
        : 0;

      AttrPtg spaceAfterFalse = null;

      if( m_tokenSpaces.Count > 0 )
      {
        spaceAfterFalse = m_tokenSpaces.Pop();
        spaceAfterFalse.AttrData1 = 4;
        iFalseSize += spaceAfterFalse.GetSize( arguments.Version );
      }

      if( spaceBeforeIf != null )
      {
        iFalseSize += spaceBeforeIf.GetSize( arguments.Version );
      }

      Ptg token = FormulaUtil.CreatePtg( FormulaToken.tAttr, 2, iTrueSize );
      m_arrTokens.Insert( iTrueStart, token );
      iFalseStart++;

      bool bNotInArrayFormula = ( ( options & ExcelParseFormulaOptions.InArray ) == 0 );
      int iOptions = bNotInArrayFormula
        ? FormulaUtil.DEF_OPTIONS_OPT_GOTO
        : FormulaUtil.DEF_OPTIONS_NOT_OPT_GOTO;

      token = FormulaUtil.CreatePtg( FormulaToken.tAttr, iOptions, iFalseSize + 3 );
      m_arrTokens.Insert( iFalseStart, token );

      if( spaceBeforeIf != null )
        m_arrTokens.Add( spaceBeforeIf );

      if( spaceAfterFalse != null )
        m_arrTokens.Add( spaceAfterFalse );

      if( iOperandCount == 3 )
      {
        token = FormulaUtil.CreatePtg( FormulaToken.tAttr, iOptions, 3 );
        m_arrTokens.Add( token );
      }

      int iRefIndex = FormulaUtil.GetIndex( typeof( FunctionVarPtg ), FormulaUtil.DEF_TYPE_VALUE,
        indexes, i, options );

      FormulaToken tokenId = FunctionVarPtg.IndexToCode( iRefIndex );
      FunctionPtg ptgFunction = ( FunctionPtg )FormulaUtil.CreatePtg( tokenId, ExcelFunction.IF );
      ptgFunction.NumberOfArguments = ( byte )( arrOperands.Count - 1 );

      return ptgFunction;
    }
    /// <summary>
    /// Creates custom functions and adds all argument tokens to the internal token array.
    /// </summary>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="arguments">Constant arguments required by some parse methods.</param>
    /// <returns>Created function token.</returns>
    private Ptg CreateCustomFunction( Dictionary<Type, ReferenceIndexAttribute> indexes, int i,
      ExcelParseFormulaOptions options, ParseParameters arguments, bool registerFunction )
    {
      int iBookIndex;
      int iNameIndex;
      IWorksheet sheet = arguments.Worksheet;

      if( !FormulaUtil.IsCustomFunction( m_tokenizer.TokenString, arguments.Workbook,
        out iBookIndex, out iNameIndex ) )
      {
        if( registerFunction )
        {
          
          IName newFunction = m_book.Names.Add( m_tokenizer.TokenString );
          ( newFunction as NameImpl ).IsFunction = true;
          iNameIndex = newFunction.Index;
          iBookIndex = -1;
        }
        else
        {
          m_tokenizer.RaiseException( m_tokenizer.TokenString + " isn't custom function.", null );
        }
      }

      Ptg tokenName = ( iBookIndex != -1 )
        ? FormulaUtil.CreatePtg( FormulaToken.tNameX1, iBookIndex, iNameIndex )
        : FormulaUtil.CreatePtg( FormulaToken.tName1, iNameIndex );

      m_arrTokens.Add( tokenName );

      List<int> arrOperands = ExtractOperands( options, arguments, ExcelFunction.CustomFunction );

      int iRefIndex = FormulaUtil.GetIndex( typeof( FunctionVarPtg ), FormulaUtil.DEF_TYPE_VALUE,
        indexes, i, options );
      FormulaToken token = FunctionVarPtg.IndexToCode( iRefIndex );
      FunctionVarPtg ptgFunction = ( FunctionVarPtg )FormulaUtil.CreatePtg( token, ExcelFunction.CustomFunction );
      ptgFunction.NumberOfArguments = ( byte )arrOperands.Count;

      return ptgFunction;
    }
    /// <summary>
    /// Evaluates size of the tokens range.
    /// </summary>
    /// <param name="iStartToken">The first token to measure (included).</param>
    /// <param name="iEndToken">The last token to measure (not included).</param>
    /// <param name="arguments">Arguments required by some parse methods.</param>
    /// <returns>Size in bytes of the specified tokens.</returns>
    private int GetTokensSize( int iStartToken, int iEndToken, ParseParameters arguments )
    {
      if( iStartToken < 0 || iStartToken >= iEndToken )
        throw new ArgumentOutOfRangeException( "iStartToken" );

      int iResult = 0;

      for( int i = iStartToken; i < iEndToken; i++ )
      {
        Ptg token = ( Ptg )m_arrTokens[ i ];
        iResult += token.GetSize( arguments.Version );
      }

      return iResult;
    }
    /// <summary>
    /// Extracts operands from the string.
    /// </summary>
    /// <param name="options">Parsing options.</param>
    /// <param name="arguments">Arguments required by some parse methods.</param>
    /// <param name="functionId">Parent function id.</param>
    /// <returns></returns>
    private List<int> ExtractOperands( ExcelParseFormulaOptions options, ParseParameters arguments,
      ExcelFunction functionId )
    {
      Dictionary<Type, ReferenceIndexAttribute> hashParamIndexes = FormulaUtil.FunctionIdToIndex[ functionId ];
      ExcelParseFormulaOptions modifiedOptions = options;

      if( ( options & ExcelParseFormulaOptions.RootLevel ) != 0 )
      {
        modifiedOptions -= ExcelParseFormulaOptions.RootLevel;
      }

      modifiedOptions |= ExcelParseFormulaOptions.ParseOperand;

      if( FormulaUtil.IndexOf( FormulaUtil.SemiVolatileFunctions, functionId ) != -1 )
      {
        m_arrTokens.Add( FormulaUtil.CreatePtg( FormulaToken.tAttr, 1, 0 ) );
      }

      m_tokenizer.NextToken();

      if( m_tokenizer.TokenType != FormulaToken.tParentheses )
        throw new ArgumentOutOfRangeException( "Can't extract function arguments." );

      m_tokenizer.NextToken();

      int j = 0;
      List<int> arrResult = new List<int>();
      arrResult.Add( m_arrTokens.Count );

      while( m_tokenizer.TokenType != FormulaToken.CloseParenthesis )
      {
        ParseExpression( Priority.None, hashParamIndexes, j, modifiedOptions, arguments );

        if( m_tokenSpaces.Count > 0 )
        {
          AttrPtg space = m_tokenSpaces.Pop();
          space.SpaceAfterToken = true;
          m_arrTokens.Add( space );
        }

//        if( tokenSpace != null )
//          throw new NotSupportedException( "unexpected space position" );

        arrResult.Add( m_arrTokens.Count );

        if( m_tokenizer.TokenType == FormulaToken.Comma )
        {
          m_tokenizer.NextToken();
        }

        j++;
      }

      m_tokenizer.NextToken();

      return arrResult;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tokenId"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    private Ptg ParseArray( FormulaToken tokenId, ParseParameters arguments )
    {
      string strValue = m_tokenizer.TokenString;
      return FormulaUtil.CreatePtg( tokenId, strValue, arguments.FormulaUtility );
      //throw new NotImplementedException();
    }
    /// <summary>
    /// Parses error reference.
    /// </summary>
    /// <param name="indexes">
    /// Dictionary with attributes that describes token index that should be used at
    /// special function position ( it can be reference token, value token, or array token ).
    /// </param>
    /// <param name="i">Index of the current argument.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <returns>Reference error Ptg.</returns>
    private Ptg ParseError( Dictionary<Type, ReferenceIndexAttribute> indexes, int i,
      ExcelParseFormulaOptions options, IWorksheet sheet )
    {
      Ptg resultPtg = null;
      string strToken = m_tokenizer.TokenString;
      //strToken = strToken.Substring( 0, strToken.Length - 1 );
      int iIndex = strToken.LastIndexOf( '!', strToken.Length - 2 );
      FormulaToken formulaToken;
      int iTokenIndex;

      if( iIndex != -1 )
      {
        strToken = strToken.Substring( 0, iIndex );
        strToken = strToken.Trim( '\'' );
        int iRef = m_book.AddSheetReference( strToken );

        iTokenIndex = FormulaUtil.GetIndex( typeof( RefError3dPtg ), FormulaUtil.DEF_TYPE_VALUE,
          indexes, i, options );

        formulaToken = RefError3dPtg.IndexToCode( iTokenIndex );

        resultPtg = FormulaUtil.CreatePtg( formulaToken );
        ( ( RefError3dPtg )resultPtg ).RefIndex = ( ushort )iRef;
      }
      else
      {
        if( strToken.EndsWith( RefErrorPtg.ReferenceError ) )
        {
          iTokenIndex = FormulaUtil.GetIndex( typeof( RefErrorPtg ), FormulaUtil.DEF_TYPE_VALUE,
            indexes, i, options );

          formulaToken = ( sheet != null ) ?
            RefErrorPtg.IndexToCode( iTokenIndex ) :
            RefError3dPtg.IndexToCode( iTokenIndex );

          resultPtg = FormulaUtil.CreatePtg( formulaToken );
        }
        else
        {
          string strError = m_tokenizer.TokenString;
          ConstructorInfo constructor = FormulaUtil.ErrorNameToConstructor[ strError ];
          resultPtg = ( Ptg )constructor.Invoke( new object[] { strError } );
        }

        IReference reference = resultPtg as IReference;

        if( reference != null )
          reference.RefIndex = ushort.MaxValue;
      }

      return resultPtg;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public List<Ptg> Tokens
    {
      get
      {
        return m_arrTokens;
      }
    }
    /// <summary>
    /// Gets / sets number format info.
    /// </summary>
    public NumberFormatInfo NumberFormat
    {
      get
      {
        return m_tokenizer.NumberFormat;
      }
      set
      {
        m_tokenizer.NumberFormat = value;
      }
    }
    #endregion
  }

  /// <summary>
  /// Contains objects that can be used by some parse methods.
  /// </summary>
  public class ParseParameters
  {
    /// <summary>
    /// Instance of the FormulaUtil class - helper class for formula parsing.
    /// </summary>
    public readonly FormulaUtil FormulaUtility;
    /// <summary>
    /// Worksheet that contains cell that is currently parsed.
    /// </summary>
    public readonly IWorksheet Worksheet;
    /// <summary>
    /// 
    /// </summary>
    public readonly Dictionary<string, string> WorksheetNames;
    /// <summary>
    /// Indicates whether R1C1 notation should be used.
    /// </summary>
    public readonly bool IsR1C1;
    /// <summary>
    /// Cell row index (it is used when parsing R1C1 and Shared and Array formulas).
    /// </summary>
    public readonly int CellRow;
    /// <summary>
    /// Cell column index (it is used when parsing R1C1 and Shared and Array formulas).
    /// </summary>
    public readonly int CellColumn;
    /// <summary>
    /// Workbook that contains cell that is currently parsed.
    /// </summary>
    public readonly IWorkbook Workbook;
    /// <summary>
    /// Destination excel version.
    /// </summary>
    public readonly ExcelVersion Version;

    /// <summary>
    /// Initializes new instance of the parameters.
    /// </summary>
    /// <param name="sheet">Worksheet that contains cell that is currently parsed.</param>
    /// <param name="worksheetNames"></param>
    /// <param name="r1C1">Indicates whether R1C1 notation should be used.</param>
    /// <param name="cellRow">Cell row index (it is used when parsing R1C1 and Shared and Array formulas).</param>
    /// <param name="cellColumn">Cell column index (it is used when parsing R1C1 and Shared and Array formulas).</param>
    /// <param name="formulaUtility">Instance of the FormulaUtil class - helper class for formula parsing.</param>
    /// <param name="book">Parent workbook.</param>
    public ParseParameters( IWorksheet sheet, Dictionary<string, string> worksheetNames, bool r1C1,
      int cellRow, int cellColumn, FormulaUtil formulaUtility, IWorkbook book )
    {
      Worksheet = sheet;
      WorksheetNames = worksheetNames;
      IsR1C1 = r1C1;
      CellRow = cellRow;
      CellColumn = cellColumn;
      FormulaUtility = formulaUtility;
      Workbook = book;
      Version = ( ( WorkbookImpl )Workbook ).Version;
    }
  }
}