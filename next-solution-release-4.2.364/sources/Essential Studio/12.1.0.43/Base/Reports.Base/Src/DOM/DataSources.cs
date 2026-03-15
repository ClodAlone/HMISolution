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
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
    public class DataSources : List<DataSource>
    {
    }

    public class DataSource
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        public bool Transaction { get; set; }
        public ConnectionProperties ConnectionProperties { get; set; }
        public string DataSourceReference { get; set; }

        public object Clone()
        {
            DataSource dataSource = new DataSource();
            dataSource.Update(this);
            return dataSource;
        }

        internal void Update(DataSource datasource)
        {
            this.Name = datasource.Name;
            this.Transaction = datasource.Transaction ;
            this.DataSourceReference = datasource.DataSourceReference ;
            if (datasource.ConnectionProperties != null)
                this.ConnectionProperties = new ConnectionProperties(datasource.ConnectionProperties);
        }

        public bool ShouldSerializeConnectionProperties()
        {
            return string.IsNullOrEmpty(DataSourceReference);
        }

        public void ResetConnectionProperties()
        {
            this.ConnectionProperties = new ConnectionProperties();
        }

        public bool ShouldSerializeTransaction()
        {
            return this.Transaction != false;
        }

        public void ResetTransaction()
        {
            this.Transaction = false;
        }
    }
}
