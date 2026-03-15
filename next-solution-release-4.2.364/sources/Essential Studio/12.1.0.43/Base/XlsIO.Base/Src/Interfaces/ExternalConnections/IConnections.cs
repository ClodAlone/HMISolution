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
    public interface IConnections : IParentApplication,ICollection<IConnection>
    {
        /// <summary>
        /// Add the connection to the workbook
        /// </summary>
        /// <param name="ConnectionName">Connection Name</param>
        /// <param name="Description"> Connection Description</param>
        /// <param name="ConnectionSting">Connection String</param>
        /// <param name="CommandText">Command Text</param>
        /// <param name="CommandType">Command Type</param>
        /// <returns></returns>
        IConnection Add(string ConnectionName, string Description, object ConnectionSting, object CommandText, ExcelCommandType CommandType);
        /// <summary>
        /// returns the connection
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IConnection this[int index] { get; }        
    }
}
