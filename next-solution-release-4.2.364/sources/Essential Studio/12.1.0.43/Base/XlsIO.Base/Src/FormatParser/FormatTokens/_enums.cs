#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;

namespace Syncfusion.XlsIO.FormatParser.FormatTokens
{
  /// <summary>
  /// Represents possible token types.
  /// </summary>
  public enum TokenType
  {
    /// <summary>
    /// Represents unknown format token.
    /// </summary>
    Unknown,
    /// <summary>
    /// Represents section format token.
    /// </summary>
    Section,
    /// <summary>
    /// Represents hour format token.
    /// </summary>
    Hour,
    /// <summary>
    /// Represents hours in 24 hours format format token.
    /// </summary>
    Hour24,
    /// <summary>
    /// Represents minute format token.
    /// </summary>
    Minute,
    /// <summary>
    /// Represents total minutes format token.
    /// </summary>
    MinuteTotal,
    /// <summary>
    /// Represents second format token.
    /// </summary>
    Second,
    /// <summary>
    /// Represents total seconds format token.
    /// </summary>
    SecondTotal,
    /// <summary>
    /// Represents year format token.
    /// </summary>
    Year,
    /// <summary>
    /// Represents month format token.
    /// </summary>
    Month,
    /// <summary>
    /// Represents day format token.
    /// </summary>
    Day,
    /// <summary>
    /// Represents string format token.
    /// </summary>
    String,
    /// <summary>
    /// Represents reserved place format token.
    /// </summary>
    ReservedPlace,
    /// <summary>
    /// Represents character format token.
    /// </summary>
    Character,
    /// <summary>
    /// Represents am/pm format token.
    /// </summary>
    AmPm,
    /// <summary>
    /// Represents color format token.
    /// </summary>
    Color,
    /// <summary>
    /// Represents condition format token.
    /// </summary>
    Condition,
    /// <summary>
    /// Represents text format token.
    /// </summary>
    Text,
    /// <summary>
    /// Represents significant digit format token.
    /// </summary>
    SignificantDigit,
    /// <summary>
    /// Represents insignificant digit format token.
    /// </summary>
    InsignificantDigit,
    /// <summary>
    /// Represents place reserved digit format token.
    /// </summary>
    PlaceReservedDigit,
    /// <summary>
    /// Represents percent format token.
    /// </summary>
    Percent,
    /// <summary>
    /// Represents scientific format token.
    /// </summary>
    Scientific,
    /// <summary>
    /// Represents general format token.
    /// </summary>
    General,
    /// <summary>
    /// Represents thousands separator format token.
    /// </summary>
    ThousandsSeparator,
    /// <summary>
    /// Represents decimal point format token.
    /// </summary>
    DecimalPoint,
    /// <summary>
    /// Represents asterix format token.
    /// </summary>
    Asterix,
    /// <summary>
    /// Represents fraction format token.
    /// </summary>
    Fraction,
    /// <summary>
    /// Represents millisecond format token.
    /// </summary>
    MilliSecond,
    /// <summary>
    /// Represents culture token.
    /// </summary>
    Culture,
  }
}
