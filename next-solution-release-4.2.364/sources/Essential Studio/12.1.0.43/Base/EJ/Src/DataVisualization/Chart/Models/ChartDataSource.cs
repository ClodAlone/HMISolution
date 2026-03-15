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
using System.Collections;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataSources;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;

using Syncfusion.JavaScript.Shared;
using System.Drawing;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class ChartDataSource
    {
        #region Feilds

        private object m_data = null;
        private string m_xName = null;
        private string m_High = null;
        private string m_Low = null;
        private string m_Open = null;
        private string m_Close = null;
        private string m_Size = null;
        private string m_yName = null;
        private string m_query = null;

        #endregion

       #region Properties
      
        [JsonProperty("data")]
        [JsonConverter(typeof(DataManagerConverter))]
        public object Data
        {
            get { return this.m_data; }
            set { this.m_data = value; }
        }

        [JsonProperty("xName")]
		 [DefaultValue(null)]
        public string XName
        {
            get { return this.m_xName; }
            set { this.m_xName = value; }
        }

        [JsonProperty("high")]
		 [DefaultValue(null)]
        public string High
        {
            get { return this.m_High; }
            set { this.m_High = value; }
        }

        [JsonProperty("low")]
		 [DefaultValue(null)]
        public string Low
        {
            get { return this.m_Low; }
            set { this.m_Low = value; }
        }

        [JsonProperty("open")]
		 [DefaultValue(null)]
        public string Open
        {
            get { return this.m_Open; }
            set { this.m_Open = value; }
        }

        [JsonProperty("close")]
		 [DefaultValue(null)]
        public string Close
        {
            get { return this.m_Close; }
            set { this.m_Close = value; }
        }

        [JsonProperty("size")]
		 [DefaultValue(null)]
        public string Size
        {
            get { return this.m_Size; }
            set { this.m_Size = value; }
        }

        [JsonProperty("yName")]
          [DefaultValue(null)]
        public string YName
        {
            get { return this.m_yName; }
            set { this.m_yName = value; }
        }

      
        [JsonProperty("query")]
        [DefaultValue(null)]
        [JsonConverter(typeof(QueryConverter))]
        public string Query
        {
            get { return this.m_query; }
            set { this.m_query = value; }
        }
       #endregion

      #region ShouldSerialize Methods
       

      #endregion
    }
}
