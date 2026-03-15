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
  /// Class used for format constants.
  /// </summary>
  public class FormatConstants
  {
    #region Constants
    /// <summary>
    /// Number of hours in a day.
    /// </summary>
    public const int HoursInDay = 24;
    /// <summary>
    /// Number of minutes in a hour.
    /// </summary>
    public const int MinutesInHour = 60;
    /// <summary>
    /// Number of minutes in a day.
    /// </summary>
    public const int MinutesInDay = HoursInDay * MinutesInHour;
    /// <summary>
    /// Number of seconds in a minute.
    /// </summary>
    public const int SecondsInMinute = 60;
    /// <summary>
    /// Number of seconds in a day.
    /// </summary>
    public const int SecondsInDay = MinutesInDay * SecondsInMinute;
    #endregion
  }
}
