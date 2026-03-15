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
using System.Collections.Generic;

using Syncfusion.XlsIO.FormatParser.FormatTokens;
using Syncfusion.XlsIO.Implementation;
#endregion

namespace Syncfusion.XlsIO.FormatParser
{
  /// <summary>
  /// Class used for format parsing.
  /// </summary>
  public class FormatParserImpl : CommonObject
  {
    #region Class members
    /// <summary>
    /// List with all known format tokens.
    /// </summary>
    private List<FormatTokenBase> m_arrFormatTokens = new List<FormatTokenBase>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the FormatParserImpl class.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    public FormatParserImpl( IApplication application, object parent )
      : base( application, parent )
    {
      m_arrFormatTokens.Add( new GeneralToken() );
      m_arrFormatTokens.Add( new StringToken() );
      m_arrFormatTokens.Add( new ReservedPlaceToken() );
      m_arrFormatTokens.Add( new CharacterToken() );
      m_arrFormatTokens.Add( new YearToken() );
      m_arrFormatTokens.Add( new MonthToken() );
      m_arrFormatTokens.Add( new DayToken() );
      m_arrFormatTokens.Add( new HourToken() );
      m_arrFormatTokens.Add( new Hour24Token() );
      m_arrFormatTokens.Add( new MinuteToken() );
      m_arrFormatTokens.Add( new MinuteTotalToken() );
      m_arrFormatTokens.Add( new SecondToken() );
      m_arrFormatTokens.Add( new SecondTotalToken() );
      m_arrFormatTokens.Add( new AmPmToken() );
      m_arrFormatTokens.Add( new SectionSeparatorToken() );
      m_arrFormatTokens.Add( new ColorToken() );
      m_arrFormatTokens.Add( new ConditionToken() );
      m_arrFormatTokens.Add( new TextToken() );
      m_arrFormatTokens.Add( new SignificantDigitToken() );
      m_arrFormatTokens.Add( new InsignificantDigitToken() );
      m_arrFormatTokens.Add( new PlaceReservedDigitToken() );
      m_arrFormatTokens.Add( new PercentToken() );
      m_arrFormatTokens.Add( new DecimalPointToken() );
      m_arrFormatTokens.Add( new ThousandsSeparatorToken() );
      m_arrFormatTokens.Add( new AsterixToken() );
      m_arrFormatTokens.Add( new ScientificToken() );
      m_arrFormatTokens.Add( new FractionToken() );
      m_arrFormatTokens.Add( new CultureToken() );

      // NOTE: Unknown token must be added last.
      m_arrFormatTokens.Add( new UnknownToken() );
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Parses format string.
    /// </summary>
    /// <param name="strFormat">String to parse.</param>
    /// <returns>Collection with parsed tokens.</returns>
    public FormatSectionCollection Parse( string strFormat )
    {
      if( strFormat == null )
        throw new ArgumentNullException( "strFormat" );

      int iFormatLength = strFormat.Length;

      if( iFormatLength == 0 )
        throw new ArgumentException( "strFormat - string cannot be empty" );

      List<FormatTokenBase> arrParsedExpression = new List<FormatTokenBase>();
      int iPos = 0;

      while( iPos < iFormatLength )
      {
        for( int i = 0, len = m_arrFormatTokens.Count; i < len; i++ )
        {
          FormatTokenBase token = m_arrFormatTokens[ i ];
          int iNewPos = token.TryParse( strFormat, iPos );

          if( iNewPos > iPos )
          {
            token = ( FormatTokenBase )token.Clone();
              if ((token.TokenType == TokenType.Section && arrParsedExpression.Count == 0) )
            {
                arrParsedExpression.Add(m_arrFormatTokens[17]);
            }
            else if (arrParsedExpression.Count > 0)
            {
                if (arrParsedExpression [arrParsedExpression .Count -1].TokenType ==  TokenType.Section && token .TokenType ==  TokenType.Section)
                {
                    arrParsedExpression.Add(m_arrFormatTokens[17]);
                }
            }
            iPos = iNewPos;
            arrParsedExpression.Add( token );
            break;
          }
        }
      }
         if (arrParsedExpression.Count > 0)
      {
          if (arrParsedExpression[arrParsedExpression.Count - 1].TokenType == TokenType.Section)
              arrParsedExpression.Add(m_arrFormatTokens[17]);
      }
      return new FormatSectionCollection( Application, this, arrParsedExpression );
    }
    #endregion

    internal void Clear()
    {
        if(m_arrFormatTokens!=null)m_arrFormatTokens.Clear();

        m_arrFormatTokens = null;
    }
  }
}
