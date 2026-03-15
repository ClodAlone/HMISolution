#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
	/// Summary description for IFormulaRecord.
	/// </summary>
	public interface IFormulaRecord
  {
    #region Properties
    /// <summary>
    /// Gets/sets parsed formula tokens.
    /// </summary>
    Ptg[] Formula
    {
      get;
      set;
    }
    #endregion
  }
}
