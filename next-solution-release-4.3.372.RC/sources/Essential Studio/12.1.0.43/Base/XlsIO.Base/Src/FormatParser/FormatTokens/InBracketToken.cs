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

using System;

namespace Syncfusion.XlsIO.FormatParser.FormatTokens
{
  /// <summary>
  /// Class used for parsing Brackets.
  /// </summary>
  public abstract class InBracketToken : FormatTokenBase
  {
    #region Class constants
    /// <summary>
    /// Start character of the token.
    /// </summary>
    private const char DEF_START = '[';
    /// <summary>
    /// End character of the token.
    /// </summary>
    private const char DEF_END = ']';
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the InBracketToken class.
    /// </summary>
    public InBracketToken()
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

      int iStartIndex = iIndex;

      if( strFormat[ iIndex ] != DEF_START ) return iIndex;
      iIndex++;

      int iEndPos = strFormat.IndexOf( DEF_END, iIndex );

      if( iEndPos < iIndex ) return iStartIndex;

      return TryParse( strFormat, iStartIndex, iIndex, iEndPos );
    }

    /// <summary>
    /// Tries to parse format string.
    /// </summary>
    /// <param name="strFormat">Format string to parse.</param>
    /// <param name="iStartIndex">Position of the first bracket.</param>
    /// <param name="iIndex">Position to start parsing at.</param>
    /// <param name="iEndIndex">Position of the end bracket.</param>
    /// <returns>Position after parsed block.</returns>
    public abstract int TryParse( string strFormat, int iStartIndex, int iIndex, int iEndIndex );
    #endregion
  }
}
