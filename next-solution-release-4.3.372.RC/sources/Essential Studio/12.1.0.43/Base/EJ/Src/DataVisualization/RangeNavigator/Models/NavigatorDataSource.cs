#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.Shared;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
   public class NavigatorDataSource
    {
         #region Field

           private object m_data = new DataSource();
           private string m_xName = null;
           private string m_yNames =null;
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

        
           [JsonProperty("yName")]
           public string YName
           {
               get { return this.m_yNames; }
               set { this.m_yNames = value; }
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
