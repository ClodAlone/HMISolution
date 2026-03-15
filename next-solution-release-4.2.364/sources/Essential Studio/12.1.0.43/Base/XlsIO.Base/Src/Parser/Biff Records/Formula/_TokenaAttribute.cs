#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Reflection;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// Attribute provides link information between formula token and
  /// formula token class.
  /// </summary>
  [ AttributeUsage( AttributeTargets.Class, AllowMultiple = true ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  sealed public class TokenAttribute : Attribute
  {
    #region Class members
    /// <summary>
    /// Type of the formula token.
    /// </summary>
    private FormulaToken m_Code;
    /// <summary>
    /// String representation of the operation (if this is attribute for
    /// operation), otherwise string.Empty.
    /// </summary>
    private string m_strOperationSymbol = string.Empty;
    /// <summary>
    /// True if operation symbol should be placed after operand
    /// (only for unary operands).
    /// </summary>
    private bool m_bPlaceAfter = false;
    #endregion

    #region Class constructors
    /// <summary>
    /// Cannot create without parameters.
    /// </summary>
    private TokenAttribute()
    {
    }

    /// <summary>
    /// Creates token attribute by Formula code.
    /// </summary>
    /// <param name="Code">Code of the token that will be created.</param>
    public TokenAttribute( FormulaToken Code )
    {
      this.m_Code = Code;
    }

    /// <summary>
    /// Creates token attribute by formula code and operation sign.
    /// </summary>
    /// <param name="Code">Token code.</param>
    /// <param name="OperationSymbol">String representation of the operation.</param>
    public TokenAttribute( FormulaToken Code, string OperationSymbol )
    {
      this.m_Code = Code;
      this.m_strOperationSymbol = OperationSymbol;
    }
    /// <summary>
    /// Creates token attribute by formula code, operation sign, and position of
    /// placement (before the False or after the True operand - this is only for unary operations,
    /// default value is False).
    /// </summary>
    /// <param name="Code">Code of the operation.</param>
    /// <param name="OperationSymbol">String representation of the operation.</param>
    /// <param name="bPlaceAfter">True if operation is placed after operands; False if before.</param>
    public TokenAttribute( FormulaToken Code, string OperationSymbol, bool bPlaceAfter )
    {
      this.m_Code = Code;
      this.m_strOperationSymbol = OperationSymbol;
      this.m_bPlaceAfter = bPlaceAfter;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Read-only. Type of the formula token.
    /// </summary>
    public FormulaToken FormulaType
    {
      get
      {
        return this.m_Code;
      }
    }
    /// <summary>
    /// Read-only. String representation of the operation (if this is attribute for
    /// operation); otherwise string.Empty.
    /// </summary>
    public string OperationSymbol
    {
      get
      {
        return m_strOperationSymbol;
      }
    }
    /// <summary>
    /// Read-only. True if operation symbol should be placed after operand
    /// (only for unary operands).
    /// </summary>
    public bool IsPlaceAfter
    {
      get
      {
        return m_bPlaceAfter;
      }
    }
    #endregion
  }

  /// <summary>
  /// There can be multiple token codes that correspond to one class,
  /// i.e. tRef1, tRef2, tRef3 correspond to class RefPtg.
  /// This attribute is used to help FormulaUtil choose the token code to use.
  /// </summary>
  [ AttributeUsage( AttributeTargets.Field, AllowMultiple = true ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  sealed public class ReferenceIndexAttribute : Attribute
  {
    #region Class members
    /// <summary>
    /// Index of the reference type (for tokens that can have several types).
    /// </summary>
    private int m_iIndex = 1;
    /// <summary>
    /// Array of indexes, first correspond to first parameter,
    /// second  for the second parameter and so on.
    /// </summary>
    private int[] m_arrIndex;
    /// <summary>
    /// Expected token type.
    /// </summary>
    private Type m_TargetType;
    #endregion

    #region Class constructors
    /// <summary>
    /// To prevent construction without arguments.
    /// </summary>
    private ReferenceIndexAttribute()
    {
    }
    /// <summary>
    /// Creates attribute that describes the function that has
    /// the same token index index for all arguments.
    /// </summary>
    /// <param name="index">Index of the token.</param>
    public ReferenceIndexAttribute( int index )
    {
      if( index < 1 || index > 3 ) throw new ArgumentOutOfRangeException();

      m_iIndex = index;

      m_TargetType = typeof( RefPtg );
    }
    /// <summary>
    /// Creates attribute for function with specified token indexes order.
    /// First member of the array corresponds to the first argument of the function,
    /// second member for the second argument and so on.
    /// </summary>
    /// <param name="arrParams">Array of token indexes.</param>
    public ReferenceIndexAttribute( params int[] arrParams )
      : this( typeof( RefPtg ), arrParams )
    {
    }
    /// <summary>
    /// Creates attribute for specified token type with specified token indexes.
    /// </summary>
    /// <param name="targetType">Target token class.</param>
    /// <param name="arrParams">Array of token indexes.</param>
    public ReferenceIndexAttribute( Type targetType, params int[] arrParams )
    {
      m_arrIndex = new int[ arrParams.Length ];
      arrParams.CopyTo( m_arrIndex, 0 );
      m_TargetType = targetType;
    }

    /// <summary>
    /// Creates attribute for specified token type with specified token indexes.
    /// </summary>
    /// <param name="targetType">Target token class.</param>
    /// <param name="index">Token index for all function parameters.</param>
    public ReferenceIndexAttribute( Type targetType, int index )
    {
      m_TargetType = targetType;
      m_iIndex = index;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns index of the reference.
    /// </summary>
    public int Index
    {
      get
      {
        return m_iIndex;
      }
    }
    /// <summary>
    /// Returns token index for the specified argument number.
    /// </summary>
    public int this[ int index ]
    {
      get
      {
        if( m_arrIndex != null && index >= 0 && index < m_arrIndex.Length )
        {
          return m_arrIndex[ index ];
        }
        else
        {
          return Index;
        }
      }
    }

    /// <summary>
    /// Target token class
    /// </summary>
    public Type TargetType
    {
      get
      {
        return m_TargetType;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int Count
    {
      get
      {
        return ( m_arrIndex != null ) ?
          m_arrIndex.Length :
          0;
      }
    }
    #endregion
  }

  /// <summary>
  /// This attribute describes error code. Used for converting error messages from
  /// and to string.
  /// </summary>
  [ AttributeUsage( AttributeTargets.Class, AllowMultiple = true ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  sealed public class ErrorCodeAttribute : Attribute
  {
    #region Class members
    /// <summary>
    /// String that represents error.
    /// </summary>
    private string m_StringValue = string.Empty;
    /// <summary>
    /// Code of the error.
    /// </summary>
    private int m_ErrorCode;
    #endregion

    #region Class constructors
    /// <summary>
    /// To prevent creation without arguments.
    /// </summary>
    private ErrorCodeAttribute()
    {
    }
    /// <summary>
    /// Creates attribute for error with specified string value and error code.
    /// </summary>
    /// <param name="stringValue">String representation of the error.</param>
    /// <param name="errorCode">Error code.</param>
    public ErrorCodeAttribute( string stringValue, int errorCode )
    {
      m_StringValue = stringValue;
      m_ErrorCode = errorCode;
    }
    #endregion

    #region Class properties
    /// <summary>
    ///
    /// </summary>
    public string StringValue
    {
      get
      {
        return m_StringValue;
      }
    }
    /// <summary>
    ///
    /// </summary>
    public int ErrorCode
    {
      get
      {
        return m_ErrorCode;
      }
    }
    #endregion
  }
}
