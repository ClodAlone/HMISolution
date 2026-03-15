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
using System.Globalization;
#endregion

namespace Syncfusion.XlsIO.FormatParser.FormatTokens
{
  /// <summary>
  /// Contains Am Pm Token descriptions.
  /// </summary>
  public class AmPmToken : FormatTokenBase
  {
    #region Class constants
    /// <summary>
    /// Start of the token.
    /// </summary>
    private const string DefaultStart2 = "tt";
    /// <summary>
    /// Start of the token.
    /// </summary>
    private const string DEF_START = "AM/PM";
    /// <summary>
    /// Length of the token.
    /// </summary>
    private static readonly int DEF_LENGTH = DEF_START.Length;
    /// <summary>
    /// Edge between AM and PM symbols.
    /// </summary>
    private const int DEF_AMPM_EDGE = 12;
    /// <summary>
    /// AM symbol.
    /// </summary>
    private const string DEF_AM = "AM";
    /// <summary>
    /// PM symbol.
    /// </summary>
    private const string DEF_PM = "PM";
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the AmPmToken class
    /// </summary>
    public AmPmToken()
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Tries to parse format string.
    /// </summary>
    /// <param name="strFormat">Format string to parse.</param>
    /// <param name="iIndex">Position to start parsing at.</param>
    /// <returns>Position after parsed block.</returns>
    public override int TryParse( string strFormat, int iIndex )
    {
      if( strFormat == null )
        throw new ArgumentNullException( "strFormat" );

      int iFormatLength = strFormat.Length;

      if( iFormatLength == 0 )
        throw new ArgumentException( "strFormat - string cannot be empty." );

      if( iIndex < 0 || iIndex > iFormatLength - 1 )
        throw new ArgumentOutOfRangeException( "iIndex", "Value cannot be less than 0 and greater than than format length - 1." );

      if( string.Compare( strFormat, iIndex, DEF_START, 0, DEF_LENGTH, StringComparison.CurrentCultureIgnoreCase ) == 0 )
      {
        m_strFormat = DEF_START;
        iIndex += DEF_LENGTH;
      }

      return iIndex;
    }

    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <param name="bShowHiddenSymbols">Indicates whether to put in result hidden symbols.</param>
    /// <param name="culture">Culture used to convert value into text.</param>
    /// <param name="section">Parent section.</param>
    /// <returns>Formatted value.</returns>
    public override string ApplyFormat( ref double value, bool bShowHiddenSymbols,
      CultureInfo culture, FormatSection section )
    {
      DateTime date =

#if ( WINRT )
 Syncfusion.XlsIO.Implementation.DateTimeExtension.FromOADate(value);
#else
          DateTime.FromOADate( value );
#endif
      int iHour = date.Hour;

      return ( iHour > DEF_AMPM_EDGE )
        ? culture.DateTimeFormat.PMDesignator
        : culture.DateTimeFormat.AMDesignator;
    }

    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <param name="bShowHiddenSymbols">Indicates whether to put in result hidden symbols.</param>
    /// <returns>Formatted value.</returns>
    public override string ApplyFormat( string value, bool bShowHiddenSymbols )
    {
      throw new NotSupportedException();
    }

    #endregion

    #region Static Methods
    /// <summary>
    /// Checks the AMPM is other pattern.
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    internal static string CheckAndApplyAMPM(string format)
    {
        if (format == null)
            throw new ArgumentNullException(format);

          CultureInfo currentCulture=
#if ( WINRT )
              CultureInfo.CurrentCulture;
#else
              System.Threading.Thread.CurrentThread.CurrentCulture;
#endif
         
          int hour = new HourToken().TryParse(format, 0);
          int minute = new MinuteToken().TryParse(format, hour);
          int second = new SecondToken().TryParse(format, minute);
          bool isTimeToken = (hour != 0) || minute != 0 || second != 0;
          if (isTimeToken && format.Contains(DefaultStart2) &&
              currentCulture.DateTimeFormat.ShortTimePattern.Contains(DefaultStart2))
              return format.Replace(DefaultStart2, DEF_START);
          return format;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns type of the token. Read-only.
    /// </summary>
    public override TokenType TokenType
    {
      get
      {
        return TokenType.AmPm;
      }
    }

    #endregion
  }
}
