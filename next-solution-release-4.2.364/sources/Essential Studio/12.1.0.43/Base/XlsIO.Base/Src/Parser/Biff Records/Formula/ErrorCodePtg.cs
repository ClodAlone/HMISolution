#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Collections;
using System.Reflection;

using Syncfusion.XlsIO.Implementation;
using System.Collections.Generic;
using System.Globalization;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  ///<exclude/>
  /// <summary>
  /// This token contains an error code.
  /// </summary>
  [ ErrorCode( "#NULL!",  0 ) ]
  [ ErrorCode( "#DIV/0!", 7 ) ]
  [ ErrorCode( "#VALUE!", 15 ) ]
  //[ ErrorCode( "#REF!", 23 ) ]
  [ ErrorCode( "#NAME?",  29 ) ]
  [ ErrorCode( "#NUM!",   36 ) ]
  [ ErrorCode( "#N/A",     42 ) ]
  [ Token ( FormulaToken.tError ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class ErrorPtg : Ptg
  {
    #region Class static constants
    /// <summary>
    /// Dictionary that allows to get error code by error name.
    /// </summary>
    public static readonly Dictionary<string, int> ErrorNameToCode = new Dictionary<string, int>( 6 );
    /// <summary>
    /// Dictionary that allows to get error name by error code.
    /// </summary>
    public static readonly Dictionary<int, string> ErrorCodeToName = new Dictionary<int, string>( 6 );
    /// <summary>
    /// Default error name - used when unknown error code was used.
    /// </summary>
    public const string DEF_ERROR_NAME = "#N/A";
    #endregion

    #region Class members
    /// <summary>
    /// Error code.
    /// </summary>
    private byte m_errorCode;
    #endregion

    #region Class static constructor
    /// <summary>
    /// Static constructor. Fills hashtables which allow to convert
    /// error code to error string and vice versa.
    /// </summary>
    static ErrorPtg()
    {
      //ErrorCodeAttribute[] attributes = ( ErrorCodeAttribute[] )
      //  Attribute.GetCustomAttributes( typeof( ErrorPtg ), typeof( ErrorCodeAttribute ) );
      ErrorCodeAttribute[] attributes = new ErrorCodeAttribute[]
      {
        new ErrorCodeAttribute( "#NULL!",  0 ),
        new ErrorCodeAttribute( "#DIV/0!", 7 ),
        new ErrorCodeAttribute( "#VALUE!", 15 ),
        new ErrorCodeAttribute( "#NAME?",  29 ),
        new ErrorCodeAttribute( "#NUM!",   36 ),
        new ErrorCodeAttribute( "#N/A",    42 ),
      };

      for( int i = 0, len = attributes.Length; i < len; i++ )
      {
        ErrorCodeAttribute attr = attributes[ i ];
        ErrorNameToCode.Add( attr.StringValue, attr.ErrorCode );
        ErrorCodeToName.Add( attr.ErrorCode, attr.StringValue );
      }
    }
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor. To prevent user from creating a token without
    /// parameters and to allow descendants do this.
    /// </summary>
    public ErrorPtg()
    {
    }
    /// <summary>
    /// Constructs token using byte array.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="offset">Offset to the token data.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public ErrorPtg( DataProvider provider, int offset, ExcelVersion version )
      : base( provider, offset, version )
    {
    }

    /// <summary>
    /// Constructs token using error code.
    /// </summary>
    /// <param name="errorCode">
    /// String representation of the error that must be created.
    /// </param>
    /// <exception cref="System.ArgumentException">
    /// When the specified error name is unknown.
    /// </exception>
    public ErrorPtg( int errorCode )
    {
      this.TokenCode = FormulaToken.tError;

      m_errorCode = ( byte )errorCode;
    }
    /// <summary>
    /// Constructs token using error name.
    /// </summary>
    /// <param name="errorName">Name of the error.</param>
    public ErrorPtg( string errorName )
      : this( ErrorNameToCode[ errorName ] )
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets error code value.
    /// </summary>
    public byte ErrorCode
    {
      get
      {
        return m_errorCode;
      }
      set
      {
        m_errorCode = value;
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Read-only. Size of the token.
    /// </summary>
    public override int GetSize( ExcelVersion version )
    {
      return 2;
    }

    /// <summary>
    /// Converts token to a string.
    /// </summary>
    /// <param name="formulaUtil">Formula util.</param>
    /// <param name="iRow">Zero-based row index of the cell that contains this token.</param>
    /// <param name="iColumn">Zero-based row index of the cell that contains this token.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>String representation of this token.</returns>
    public override string ToString( FormulaUtil formulaUtil, int iRow, int iColumn, bool bR1C1,
      NumberFormatInfo numberFormat, bool isForSerialization )
    {
      string strResult;

      return ( ErrorCodeToName.TryGetValue( m_errorCode, out strResult ) )
        ? strResult
        : DEF_ERROR_NAME;
    }
    /// <summary>
    /// Converts token to byte array.
    /// </summary>
    /// <returns>Array of bytes representing this token.</returns>
    public override byte[] ToByteArray( ExcelVersion version )
    {
      byte[] result = base.ToByteArray( version );
      result[ 1 ] = m_errorCode;
      return result;
    }

    #endregion

    #region Class infill methods
    /// <summary>
    /// Infill PTG structure.
    /// </summary>
    /// <param name="provider">Represents storage.</param>
    /// <param name="offset">Offset in storage.</param>
    /// <param name="version">Excel version that was used to infill data provider.</param>
    public override void InfillPTG( DataProvider provider, ref int offset, ExcelVersion version )
    {
      m_errorCode = provider.ReadByte( offset++ );
    }
    #endregion
  }
}
