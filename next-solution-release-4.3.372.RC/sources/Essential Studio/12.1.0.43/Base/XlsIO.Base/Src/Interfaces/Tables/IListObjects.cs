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
using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// This interface represents collection of ListObjects in the worksheet.
  /// </summary>
  public interface IListObjects : IList<IListObject>
  {
    #region Methods
    /// <summary>
    /// Creates new list object and adds it to the collection.
    /// </summary>
    /// <param name="name">Name of the new list object.</param>
    /// <param name="range">Destination range.</param>
    /// <returns>Newly created object.</returns>
    IListObject Create( string name, IRange range );
    /// <summary>
    /// To include the query table for Specific connection
    /// </summary>
    /// <param name="type">Connection Type</param>
    /// <param name="connection">Connection</param>
    /// <param name="Destination">Destination Range</param>
    /// <returns></returns>
    IListObject AddEx(ExcelListObjectSourceType type, IConnection connection, IRange Destination);
    #endregion
  }
}
