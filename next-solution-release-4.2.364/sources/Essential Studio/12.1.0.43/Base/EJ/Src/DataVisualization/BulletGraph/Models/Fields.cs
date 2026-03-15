#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;


namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class Fields
    {
        #region Fields
        private object dataSource = new object();
        private String tableName = null;
        private String query = null;
        private String categoryField = null;
        private String featureMeasureField = null;
        private String comparativeMeasureField = null; 
        #endregion

        #region Properties
        [JsonProperty("dataSource")]
        [JsonConverter(typeof(DataManagerConverter))]
        public object DataSource
        {
            get { return this.dataSource; }
            set { this.dataSource = value; }
        }

        [JsonProperty("tableName")]
        [DefaultValue(null)]
        public String TableName
        {
            get { return this.tableName; }
            set { this.tableName = value; }
        }

        [JsonProperty("query")]
        [DefaultValue(null)]
        [JsonConverter(typeof(QueryConverter))]
        public String Query
        {
            get { return this.query; }
            set { this.query = value; }
        }

        [JsonProperty("category")]
        [DefaultValue(null)]
        public String CategoryField
        {
            get { return this.categoryField; }
            set { this.categoryField = value; }
        }

        [JsonProperty("featureMeasure")]
        [DefaultValue(null)]
        public String FeatureMeasureField
        {
            get { return this.featureMeasureField; }
            set { this.featureMeasureField = value; }
        }

        [JsonProperty("comparativeMeasure")]
        [DefaultValue(null)]
        public String ComparativeMeasureField
        {
            get { return this.comparativeMeasureField; }
            set { this.comparativeMeasureField = value; }
        }
        #endregion
    }

    
}
