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
using System.Xml;

namespace Syncfusion.RDL.DOM
{
    public class Query
    {
        public string DataSourceName { get; set; }
        public CommandType CommandType { get; set; }
        public string CommandText { get; set; }
        public QueryParameters QueryParameters { get; set; }
        public int Timeout { get; set; }

        #region Serialization methods

        public bool ShouldSerializeQueryParameters()
        {
            return this.QueryParameters != null && this.QueryParameters.Count > 0;
        }

        public void ResetQueryParameters()
        {
            this.QueryParameters = new QueryParameters();
        }

        public bool ShouldSerializeTimeout()
        {
            return this.Timeout != 0;
        }

        public void ResetTimeout()
        {
            this.Timeout = 0;
        }

        #endregion
        
        public Query()
        {
        }

        public Query(Query query)
        {
            this.DataSourceName = query.DataSourceName;
            this.CommandType = query.CommandType;
            this.CommandText = query.CommandText;
            this.Timeout = query.Timeout;
            this.QueryParameters = new QueryParameters();
            if (query.QueryParameters != null)
                this.QueryParameters.AddRange(query.QueryParameters.Clone());
        }
    }
}
