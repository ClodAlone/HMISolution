#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.JavaScript.DataSources
{
    public class DataSourceBuilder
    {
        private DataSource dataSource = new DataSource();
        public DataSourceBuilder(DataSource dataSource)
        {
            this.dataSource = dataSource;
        }
        public DataSourceBuilder URL(String url)
        {
            this.dataSource.URL = url;
            return this;
        }
        public DataSourceBuilder Table(String table)
        {
            this.dataSource.Table = table;
            return this;
        }
        public DataSourceBuilder Json(Object json)
        {
            this.dataSource.Json = json;
            return this;
        }
        public DataSourceBuilder Headers(List<String> headers)
        {
            this.dataSource.Headers = headers;
            return this;
        }

        public DataSourceBuilder Accept(String accept)
        {
            this.dataSource.Accept = accept;
            return this;
        }

        public DataSourceBuilder Key(String key)
        {
            this.dataSource.Key = key;
            return this;
        }

        public DataSourceBuilder CrossDomain(bool crossDomain)
        {
            this.dataSource.CrossDomain = crossDomain;
            return this;
        }

        public DataSourceBuilder Jsonp(String jsonp)
        {
            this.dataSource.Jsonp = jsonp;
            return this;
        }

        public DataSourceBuilder DataType(String dataType)
        {
            this.dataSource.DataType = dataType;
            return this;
        }

        public DataSourceBuilder Offline(bool offline)
        {
            this.dataSource.Offline = offline;
            return this;
        }

        public DataSourceBuilder Accept(bool requireFormat)
        {
            this.dataSource.RequireFormat = requireFormat;
            return this;
        }
    }
}
