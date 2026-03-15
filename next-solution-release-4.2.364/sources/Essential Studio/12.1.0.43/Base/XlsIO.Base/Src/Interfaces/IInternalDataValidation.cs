#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

namespace Syncfusion.XlsIO.Interfaces
{
  interface IInternalDataValidation : IDataValidation
  {
    #region Members
    Ptg[] FirstFormulaTokens
    {
      get;
      set;
    }
    Ptg[] SecondFormulaTokens
    {
      get;
      set;
    }
    #endregion
  }
}
