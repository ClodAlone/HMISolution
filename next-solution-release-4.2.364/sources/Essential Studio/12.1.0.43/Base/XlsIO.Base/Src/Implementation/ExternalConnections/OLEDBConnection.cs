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

namespace Syncfusion.XlsIO.Implementation
{
    public class OLEDBConnection : DataBaseProperty
    {
        /// <summary>
        /// It define the external connection
        /// </summary>
        private ExternalConnection m_connection;

        /// <summary>
        /// Define connection information
        /// </summary>
        public override object ConnectionString
        {
            get
            {
                return base.ConnectionString;
            }
            set
            {
                base.ConnectionString = value;
                string conn_str = value.ToString().ToLower();
                if (!conn_str.StartsWith("provider"))
                {
                    conn_str = value.ToString().Substring(conn_str.IndexOf(";") + 1);
                    if (m_connection != null)
                        m_connection.DBConnectionString = conn_str;
                    base.ConnectionString = ExternalConnectionCollection.Checkconnection(conn_str);
                }
            }
        }
        /// <summary>
        /// Implement the OLEDB Connection for ExternalConnection
        /// </summary>
        /// <param name="connection"></param>
        public OLEDBConnection(ExternalConnection connection)
        {
            m_connection = connection;
        }
    }
}
