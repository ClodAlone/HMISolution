//-------------------------------------------------------------------------------------------------
// <copyright file="SqlDataProvider.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.Reports.Server.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Data.SqlServerCe;
    using System.Linq;
    using System.Xml.Linq;
    using System.IO;

    /// <summary>
    /// A XML data provider for EmbededXML datasource.
    /// </summary>
    internal class XMLDataProvider
    {
        public DataTable GetTable(string command, string tableName)
        {
            System.Xml.XmlReader stream = System.Xml.XmlReader.Create(new System.IO.StringReader(command));
            XElement xDoc = XElement.Load(stream);
            List<string> Columns = new List<string>();
            DataTable dataTable = new DataTable();
            XElement root = xDoc.Elements("XmlData").Elements().FirstOrDefault();
            StringReader sr = new StringReader(root.ToString());
            DataSet ds = new DataSet();
            ds.ReadXml(sr);
            dataTable = ds.Tables[0];
            dataTable.TableName = tableName;
            return dataTable;
        }
    }
}
