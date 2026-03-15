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

namespace Syncfusion.XlsIO.Interfaces
{
  /// <summary>
  /// This interface is used to perform some action on demand.
  /// </summary>
  interface IOperation
  {
    /// <summary>
    /// Performs required operation.
    /// </summary>
    void Do();
  }
}
