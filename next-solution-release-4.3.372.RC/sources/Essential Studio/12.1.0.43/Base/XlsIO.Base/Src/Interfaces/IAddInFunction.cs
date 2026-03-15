#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents an Custom Addin Function in Excel.
  /// </summary>
  public interface IAddInFunction : IParentApplication
  {
    #region Properties

    /// <summary>
    /// Returns name of the add-in function. Read-only.
    /// </summary>
    string Name
    {
      get;
    }

    #endregion Properties
  }
}