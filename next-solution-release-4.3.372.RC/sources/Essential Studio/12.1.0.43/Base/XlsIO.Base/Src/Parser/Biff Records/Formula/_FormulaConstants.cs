#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;

namespace Syncfusion.XlsIO.Parser.Biff_Records.Formula
{
  /// <summary>
  /// Summary description for _FormulaConstants.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public enum FormulaToken
  {
    /// <summary>
    /// 
    /// </summary>
    None = 0,
    // Binary tokens:
    /// <summary>
    /// Represents the tAdd binary token.
    /// </summary>
    tAdd = 0x03,
    /// <summary>
    /// Represents the tSub binary token.
    /// </summary>
    tSub = 0x04,
    /// <summary>
    /// Represents the tMul binary token.
    /// </summary>
    tMul = 0x05,
    /// <summary>
    /// Represents the tDiv binary token.
    /// </summary>
    tDiv = 0x06,
    /// <summary>
    /// Represents the tPower binary token.
    /// </summary>
    tPower = 0x07,
    /// <summary>
    /// Represents the tConcat binary token.
    /// </summary>
    tConcat = 0x08,
    /// <summary>
    /// Represents the tLessThan binary token.
    /// </summary>
    tLessThan = 0x09,
    /// <summary>
    /// Represents the tLessEqual binary token.
    /// </summary>
    tLessEqual = 0x0A,
    /// <summary>
    /// Represents the tEqual binary token.
    /// </summary>
    tEqual = 0x0B,
    /// <summary>
    /// Represents the tGreaterEqual binary token.
    /// </summary>
    tGreaterEqual = 0x0C,
    /// <summary>
    /// Represents the tGreater binary token.
    /// </summary>
    tGreater = 0x0D,
    /// <summary>
    /// Represents the tNotEqual binary token.
    /// </summary>
    tNotEqual = 0x0E,
    /// <summary>
    /// Represents the tCellRangeIntersection binary token.
    /// </summary>
    tCellRangeIntersection = 0x0F,
    /// <summary>
    /// Represents the tCellRangeList binary token.
    /// </summary>
    tCellRangeList = 0x10,
    /// <summary>
    /// Represents the tCellRange binary token.
    /// </summary>
    tCellRange = 0x11,

    //Unary tokens:
    /// <summary>
    /// Represents the tUnaryPlus unary token.
    /// </summary>
    tUnaryPlus = 0x12,
    /// <summary>
    /// Represents the tUnaryMinus unary token.
    /// </summary>
    tUnaryMinus = 0x13,
    /// <summary>
    /// Represents the tPercent unary token.
    /// </summary>
    tPercent = 0x14,
    /// <summary>
    /// Represents the tParentheses unary token.
    /// </summary>
    tParentheses = 0x15,

    // Function tokens:
    /// <summary>
    /// Represents the tFunction1 function token.
    /// </summary>
    tFunction1 = 0x21,
    /// <summary>
    /// Represents the tFunction2 function token.
    /// </summary>
    tFunction2 = 0x41,
    /// <summary>
    /// Represents the tFunction3 function token.
    /// </summary>
    tFunction3 = 0x61,
    /// <summary>
    /// Represents the tFunctionVar1 function token.
    /// </summary>
    tFunctionVar1 = 0x22,
    /// <summary>
    /// Represents the tFunctionVar2 function token.
    /// </summary>
    tFunctionVar2 = 0x42,
    /// <summary>
    /// Represents the tFunctionVar3 function token.
    /// </summary>
    tFunctionVar3 = 0x62,
    /// <summary>
    /// Represents the tFunctionCE1 function token.
    /// </summary>
    tFunctionCE1 = 0x38,
    /// <summary>
    /// Represents the tFunctionCE2 function token.
    /// </summary>
    tFunctionCE2 = 0x58,
    /// <summary>
    /// Represents the tFunctionCE3 function token.
    /// </summary>
    tFunctionCE3 = 0x78,

    // Constant tokens:
    /// <summary>
    /// Represents the tMissingArgument constant token.
    /// </summary>
    tMissingArgument = 0x16,
    /// <summary>
    /// Represents the tStringConstant constant token.
    /// </summary>
    tStringConstant = 0x17,
    /// <summary>
    /// Represents the tError constant token.
    /// </summary>
    tError = 0x1C,
    /// <summary>
    /// Represents the tBoolean constant token.
    /// </summary>
    tBoolean = 0x1D,
    /// <summary>
    /// Represents the tInteger constant token.
    /// </summary>
    tInteger = 0x1E,
    /// <summary>
    /// Represents the tNumber constant token.
    /// </summary>
    tNumber = 0x1F,

    // Control tokens:
    /// <summary>
    /// Represents the tExp control token.
    /// </summary>
    tExp = 0x01,        // Matrix formula or shared formula
    /// <summary>
    /// Represents the tTbl control token.
    /// </summary>
    tTbl = 0x02,        // Multiple operation table
    /// <summary>
    /// Represents the tExtended control token.
    /// </summary>
    tExtended = 0x18,
    /// <summary>
    /// Represents the tAttr control token.
    /// </summary>
    tAttr = 0x19,
    /// <summary>
    /// Represents the tSheet control token.
    /// </summary>
    tSheet = 0x1A,
    /// <summary>
    /// Represents the tEndSheet control token.
    /// </summary>
    tEndSheet = 0x1B,

    //Operand tokens:
    /// <summary>
    /// Represents the tArray1 operand token.
    /// </summary>
    tArray1 = 0x20,
    /// <summary>
    /// Represents the tArray2 operand token.
    /// </summary>
    tArray2 = 0x40,
    /// <summary>
    /// Represents the tArray3 operand token.
    /// </summary>
    tArray3 = 0x60,
    /// <summary>
    /// Represents the tName1 operand token.
    /// </summary>
    tName1 = 0x23,
    /// <summary>
    /// Represents the tName2 operand token.
    /// </summary>
    tName2 = 0x43,
    /// <summary>
    /// Represents the tName3 operand token.
    /// </summary>
    tName3 = 0x63,
    /// <summary>
    /// Represents the tRef1 operand token.
    /// </summary>
    tRef1 = 0x24,
    /// <summary>
    /// Represents the tRef2 operand token.
    /// </summary>
    tRef2 = 0x44,
    /// <summary>
    /// Represents the tRef3 operand token.
    /// </summary>
    tRef3 = 0x64,
    /// <summary>
    /// Represents the tArea1 operand token.
    /// </summary>
    tArea1 = 0x25,
    /// <summary>
    /// Represents the tArea2 operand token.
    /// </summary>
    tArea2 = 0x45,
    /// <summary>
    /// Represents the tArea3 operand token.
    /// </summary>
    tArea3 = 0x65,
    /// <summary>
    /// Represents the tMemArea1 operand token.
    /// </summary>
    tMemArea1 = 0x26,
    /// <summary>
    /// Represents the tMemArea2 operand token.
    /// </summary>
    tMemArea2 = 0x46,
    /// <summary>
    /// Represents the tMemArea3 operand token.
    /// </summary>
    tMemArea3 = 0x66,
    /// <summary>
    /// Represents the tMemErr1 operand token.
    /// </summary>
    tMemErr1 = 0x27,
    /// <summary>
    /// Represents the tMemErr2 operand token.
    /// </summary>
    tMemErr2 = 0x47,
    /// <summary>
    /// Represents the tMemErr3 operand token.
    /// </summary>
    tMemErr3 = 0x67,
    /// <summary>
    /// Represents the tMemNoMem1 operand token.
    /// </summary>
    tMemNoMem1 = 0x28,
    /// <summary>
    /// Represents the tMemNoMem2 operand token.
    /// </summary>
    tMemNoMem2 = 0x48,
    /// <summary>
    /// Represents the tMemNoMem3 operand token.
    /// </summary>
    tMemNoMem3 = 0x68,
    /// <summary>
    /// Represents the tMemFunc1 operand token.
    /// </summary>
    tMemFunc1 = 0x29,
    /// <summary>
    /// Represents the tMemFunc2 operand token.
    /// </summary>
    tMemFunc2 = 0x49,
    /// <summary>
    /// Represents the tMemFunc3 operand token.
    /// </summary>
    tMemFunc3 = 0x69,
    /// <summary>
    /// Represents the tRefErr1 operand token.
    /// </summary>
    tRefErr1 = 0x2A,
    /// <summary>
    /// Represents the tRefErr2 operand token.
    /// </summary>
    tRefErr2 = 0x4A,
    /// <summary>
    /// Represents the tRefErr3 operand token.
    /// </summary>
    tRefErr3 = 0x6A,
    /// <summary>
    /// Represents the tAreaErr1 operand token.
    /// </summary>
    tAreaErr1 = 0x2B,
    /// <summary>
    /// Represents the tAreaErr2 operand token.
    /// </summary>
    tAreaErr2 = 0x4B,
    /// <summary>
    /// Represents the tAreaErr3 operand token.
    /// </summary>
    tAreaErr3 = 0x6B,
    /// <summary>
    /// Represents the tRefN1 operand token.
    /// </summary>
    tRefN1 = 0x2C,
    /// <summary>
    /// Represents the tRefN2 operand token.
    /// </summary>
    tRefN2 = 0x4C,
    /// <summary>
    /// Represents the tRefN3 operand token.
    /// </summary>
    tRefN3 = 0x6C,
    /// <summary>
    /// Represents the tAreaN1 operand token.
    /// </summary>
    tAreaN1 = 0x2D,
    /// <summary>
    /// Represents the tAreaN2 operand token.
    /// </summary>
    tAreaN2 = 0x4D,
    /// <summary>
    /// Represents the tAreaN3 operand token.
    /// </summary>
    tAreaN3 = 0x6D,
    /// <summary>
    /// Represents the tMemAreaN1 operand token.
    /// </summary>
    tMemAreaN1 = 0x2E,
    /// <summary>
    /// Represents the tMemAreaN2 operand token.
    /// </summary>
    tMemAreaN2 = 0x4E,
    /// <summary>
    /// Represents the tMemAreaN3 operand token.
    /// </summary>
    tMemAreaN3 = 0x6E,
    /// <summary>
    /// Represents the tMemNoMemN1 operand token.
    /// </summary>
    tMemNoMemN1 = 0x2F,
    /// <summary>
    /// Represents the tMemNoMemN2 operand token.
    /// </summary>
    tMemNoMemN2 = 0x4F,
    /// <summary>
    /// Represents the tMemNoMemN3 operand token.
    /// </summary>
    tMemNoMemN3 = 0x6F,
    /// <summary>
    /// Represents the tNameX1 operand token.
    /// </summary>
    tNameX1 = 0x39,
    /// <summary>
    /// Represents the tNameX2 operand token.
    /// </summary>
    tNameX2 = 0x59,
    /// <summary>
    /// Represents the tNameX3 operand token.
    /// </summary>
    tNameX3 = 0x79,
    /// <summary>
    /// Represents the tRef3d1 operand token.
    /// </summary>
    tRef3d1 = 0x3A,
    /// <summary>
    /// Represents the tRef3d2 operand token.
    /// </summary>
    tRef3d2 = 0x5A,
    /// <summary>
    /// Represents the tRef3d3 operand token.
    /// </summary>
    tRef3d3 = 0x7A,
    /// <summary>
    /// Represents the tArea3d1 operand token.
    /// </summary>
    tArea3d1 = 0x3B,
    /// <summary>
    /// Represents the tArea3d2 operand token.
    /// </summary>
    tArea3d2 = 0x5B,
    /// <summary>
    /// Represents the tArea3d3 operand token.
    /// </summary>
    tArea3d3 = 0x7B,
    /// <summary>
    /// Represents the tRefErr3d1 operand token.
    /// </summary>
    tRefErr3d1 = 0x3C,
    /// <summary>
    /// Represents the tRefErr3d2 operand token.
    /// </summary>
    tRefErr3d2 = 0x5C,
    /// <summary>
    /// Represents the tRefErr3d3 operand token.
    /// </summary>
    tRefErr3d3 = 0x7C,
    /// <summary>
    /// Represents the tAreaErr3d1 operand token.
    /// </summary>
    tAreaErr3d1 = 0x3D,
    /// <summary>
    /// Represents the tAreaErr3d2 operand token.
    /// </summary>
    tAreaErr3d2 = 0x5D,
    /// <summary>
    /// Represents the tAreaErr3d3 operand token.
    /// </summary>
    tAreaErr3d3 = 0x7D,

    /// <summary>
    /// Indicates end of formula token. This token is used only for parser
    /// internal purposes and shouldn't appear in the resulting formula.
    /// </summary>
    EndOfFormula = 0x1001,
    /// <summary>
    /// Indicates closing parenthesis. This token is used only for parser
    /// internal purposes and shouldn't appear in the resulting formula.
    /// </summary>
    CloseParenthesis = 0x1002,
    /// <summary>
    /// Indicates delimiter between arguments. This token is used only for parser
    /// internal purpose and shouldn't appear in the resulting formula.
    /// </summary>
    Comma = 0x1003,
    /// <summary>
    /// Indicates open bracket. This token is used only for parser
    /// internal purpose and shouldn't appear in the resulting formula.
    /// </summary>
    OpenBracket = 0x1004,
    /// <summary>
    /// Indicates close bracket. This token is used only for parser
    /// internal purpose and shouldn't appear in the resulting formula.
    /// </summary>
    CloseBracket = 0x1005,
    /// <summary>
    /// Indicates boolean value - true. This token is used only for parser
    /// internal purpose and shouldn't appear in the resulting formula.
    /// </summary>
    ValueTrue = 0x1006,
    /// <summary>
    /// Indicates boolean value - false. This token is used only for parser
    /// internal purpose and shouldn't appear in the resulting formula.
    /// </summary>
    ValueFalse = 0x1007,
    /// <summary>
    /// Indicates space token. This token is used only for parser
    /// internal purpose and shouldn't appear in the resulting formula.
    /// </summary>
    Space = 0x1008,
    /// <summary>
    /// Indicates identifier token. This token can be range, named range, function call, etc.
    /// This token is used only for parser internal purpose and shouldn't appear in the resulting formula.
    /// </summary>
    Identifier = 0x1009,
    /// <summary>
    /// Indicates identifier token that contains DDE link.
    /// This token is used only for parser internal purpose and shouldn't appear in the resulting formula.
    /// </summary>
    DDELink = 0x100A,
    /// <summary>
    /// Indicates 3D identifier token. This token can be range, named range.
    /// This token is used only for parser internal purpose and shouldn't appear in the resulting formula.
    /// </summary>
    Identifier3D = 0x100B,
  }
  /// <summary>
  /// Represents the operation type.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public enum TOperation
  {
    /// <summary>
    /// Represents the TYPE_UNARY operation type.
    /// </summary>
    TYPE_UNARY    = 0,
    /// <summary>
    /// Represents the TYPE_BINARY operation type.
    /// </summary>
    TYPE_BINARY   = 1,
    /// <summary>
    /// Represents the TYPE_FUNCTION operation type.
    /// </summary>
    TYPE_FUNCTION = 2
  }
  /// <summary>
  /// 
  /// </summary>
  public enum Priority
  {
    /// <summary>
    /// None priority.
    /// </summary>
    None,
    /// <summary>
    /// Logical equality priority.
    /// </summary>
    Equality,
    /// <summary>
    /// Concatenation priority.
    /// </summary>
    Concat,
    /// <summary>
    /// Plus and minus operation priority.
    /// </summary>
    PlusMinus,
    /// <summary>
    /// Multiplication and divide operations priority.
    /// </summary>
    MulDiv,
    /// <summary>
    /// Priority of the power operation.
    /// </summary>
    Power,
    /// <summary>
    /// Unary operation priority (-,+,%).
    /// </summary>
    UnaryMinus,
    /// <summary>
    /// Cell Range operator (:).
    /// </summary>
    CellRange,
  }
}
