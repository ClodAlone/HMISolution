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

namespace Syncfusion.XlsIO
{
	/// <summary>
	/// Summary description for _constants.
	/// </summary>
	public sealed class ExcelConstants
	{
    #region Class constants
    /// <summary>
    /// This value is used instead of possible minimum row or column index.
    /// </summary>
    public const int MinimumIndex = -1;
    /// <summary>
    /// This value is used instead of possible maximum row or column index.
    /// </summary>
    public const int MaximumIndex = -2;
    /// <summary>
    /// This value is used instead of maximum used row or column index.
    /// </summary>
    public const int MaximumUsedIndex = -3;
    /// <summary>
    /// This value is used instead of minimum used row or column index.
    /// </summary>
    public const int MinimumUsedIndex = -4;
    /// <summary>
    /// Size of the Int32 value.
    /// </summary>
    internal const int IntSize = 4;
    /// <summary>
    /// Size of the Int16 value.
    /// </summary>
    internal const int ShortSize = 2;
    /// <summary>
    /// Size of the Int64 value.
    /// </summary>
    internal const int LongSize = 8;
    /// <summary>
    /// Number of bits inside single short value.
    /// </summary>
    internal const int BitsInShort = 16;
    /// <summary>
    /// Number of bits inside single byte value.
    /// </summary>
    internal const int BitsInByte = 8;
    /// <summary>
    /// Size of the Double value in bytes.
    /// </summary>
    internal const int DoubleSize = 8;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Used to prevent user from creation of instances of this type.
    /// </summary>
    private ExcelConstants()
    {
    }
    #endregion
  }
}
