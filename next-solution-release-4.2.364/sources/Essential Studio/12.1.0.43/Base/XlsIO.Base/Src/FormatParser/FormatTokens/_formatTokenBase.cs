#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion Copyright

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.FormatParser.FormatTokens
{
  /// <summary>
  /// Base class for formula tokens.
  /// </summary>
  public abstract class FormatTokenBase : ICloneable
  {
    #region Class constants
    /// <summary>
    /// Default regular expressions options:
    /// </summary>
      protected const RegexOptions DEF_OPTIONS =
#if  (SILVERLIGHT) || (WINRT) || (WP)
        RegexOptions.None;
#else
        RegexOptions.Compiled;
#endif
    #endregion

    #region Class members
    /// <summary>
    /// Part of format.
    /// </summary>
    protected string m_strFormat;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the FormatTokenBase class
    /// </summary>
    public FormatTokenBase()
    {
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Tries to parse format string.
    /// </summary>
    /// <param name="strFormat">Format string to parse.</param>
    /// <param name="iIndex">Position to start parsing at.</param>
    /// <returns>Position after parsed block.</returns>
    public abstract int TryParse( string strFormat, int iIndex );
    /// <summary>
    /// Tries to parse format string using regular expression.
    /// </summary>
    /// <param name="regex">Regular expression to use.</param>
    /// <param name="strFormat">Format string to parse.</param>
    /// <param name="iIndex">Start index.</param>
    /// <returns>Position after parsing.</returns>
    protected int TryParseRegex( Regex regex, string strFormat, int iIndex )
    {
      Match m;
      return TryParseRegex( regex, strFormat, iIndex, out m );
    }
    /// <summary>
    /// Tries to parse format string using regular expression.
    /// </summary>
    /// <param name="regex">Regular expression to use.</param>
    /// <param name="strFormat">Format string to parse.</param>
    /// <param name="iIndex">Start index.</param>
    /// <param name="m">Output regular expression match.</param>
    /// <returns>Position after parsing.</returns>
    protected int TryParseRegex( Regex regex, string strFormat, int iIndex, out Match m )
    {
      if( regex == null )
        throw new ArgumentNullException( "regex" );

      if( strFormat == null )
        throw new ArgumentNullException( "strFormat" );

      int iFormatLength = strFormat.Length;

      if( iFormatLength == 0 )
        throw new ArgumentException( "strFormat - string cannot be empty" );

      if( iIndex < 0 || iIndex > iFormatLength - 1 )
        throw new ArgumentOutOfRangeException( "iIndex", "Value cannot be less than 0 or greater than Format Length - 1" );

      m = regex.Match( strFormat, iIndex );

      if( m.Success && m.Index == iIndex )
      {
        Format = m.Value;
        iIndex += m_strFormat.Length;
      }

      return iIndex;
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <returns>Formatted value.</returns>
    public virtual string ApplyFormat( ref double value )
    {
      return ApplyFormat( ref value, false, null, null );
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <param name="bShowHiddenSymbols">Indicates whether to put in result hidden symbols.</param>
    /// <returns>Formatted value.</returns>
    public abstract string ApplyFormat( string value, bool bShowHiddenSymbols );
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <returns>Formatted value.</returns>
    public virtual string ApplyFormat( string value )
    {
      return ApplyFormat( value, false );
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
      return MemberwiseClone();
    }
    /// <summary>
    /// Applies format to the value.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <param name="bShowHiddenSymbols">Indicates whether to put in result hidden symbols.</param>
    /// <param name="culture">Culture used to convert value into text.</param>
    /// <param name="section">Parent section.</param>
    /// <returns>Formatted value.</returns>
    public abstract string ApplyFormat( ref double value, bool bShowHiddenSymbols,
      CultureInfo culture, FormatSection section );
    #endregion

    #region Class properties
    /// <summary>
    /// Gets or sets format of the token.
    /// </summary>
    public string Format
    {
      get
      {
        return m_strFormat;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        if( value.Length == 0 )
          throw new ArgumentException( "value - string cannot be empty." );

        if( m_strFormat != value )
        {
          m_strFormat = value;
          OnFormatChange();
        }
      }
    }
    /// <summary>
    /// Gets type of the token. Read-only.
    /// </summary>
    public abstract TokenType TokenType { get; }
    #endregion

    #region Class static methods
    /// <summary>
    /// Searches for string from strings array in the format starting from the specified position.
    /// </summary>
    /// <param name="arrStrings">Array of strings to check.</param>
    /// <param name="strFormat">String format to search in.</param>
    /// <param name="iIndex">Start index in the format.</param>
    /// <param name="bIgnoreCase">Indicates whether to ignore case.</param>
    /// <returns>String index or -1 if not found.</returns>
    public int FindString( string[] arrStrings, string strFormat, int iIndex, bool bIgnoreCase )
    {
      if( strFormat == null )
        throw new ArgumentNullException( "strFormat" );

      int iFormatLength = strFormat.Length;

      if( iFormatLength == 0 )
        throw new ArgumentException( "strFormat - string cannot be empty." );

      if( iIndex < 0 || iIndex > iFormatLength - 1 )
        throw new ArgumentOutOfRangeException( "iIndex", "Value cannot be less than 0 and greater than than format length - 1." );

      for( int i = 0, len = arrStrings.Length; i < len; i++ )
      {
        string strColor = arrStrings[ i ];
        StringComparison comparison = bIgnoreCase ?
          StringComparison.CurrentCultureIgnoreCase :
          StringComparison.CurrentCulture;

        if( string.Compare( strFormat, iIndex, strColor, 0, strColor.Length, comparison ) == 0 )
        {
          return i;
        }
      }

      return -1;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// This method is called after format string was changed.
    /// </summary>
    protected virtual void OnFormatChange()
    {
    }
    #endregion
  }
}
