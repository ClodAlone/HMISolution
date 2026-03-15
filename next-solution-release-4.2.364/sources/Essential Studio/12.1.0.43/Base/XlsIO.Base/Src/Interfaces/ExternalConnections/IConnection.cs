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
using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO
{
    public interface IConnection : IParentApplication
    { 
        /// <summary>
        /// Represent the connection name.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Represent the connection description.
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// Represent the connection range.read only
        /// </summary>
        IRange Range { get; }
        /// <summary>
        /// Gets the oledb connection.
        /// </summary>
        OLEDBConnection OLEDBConnection { get; }
        /// <summary>
        /// Gets the odbc connection
        /// </summary>
        ODBCConnection ODBCConnection { get; }
        /// <summary>
        /// Delete the connection
        /// </summary>
        void Delete();
        /// <summary>
        /// Gets the connection id
        /// </summary>
        uint ConncetionId { get; }
        /// <summary>
        /// Represent the connection password event
        /// </summary>
        event ConnectionPasswordEventHandler OnConnectionPassword;
        /// <summary>
        /// Gets the connection Type
        /// </summary>
        ExcelConnectionsType DataBaseType { get; }
    }
}
