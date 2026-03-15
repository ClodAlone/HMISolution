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
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapGridServiceMethods
    {
         #region PrivateVariables
        private string initialize = "InitializeOlapGrid";
        private string drilldown = "DrillOlapGrid";
        #endregion
        #region Properties
        [JsonProperty("initialize")]
        [DefaultValue("InitializeOlapGrid")]
        public String Initialize
        {
            get { return this.initialize; }
            set { this.initialize = value; }
        }
        [JsonProperty("drilldown")]
        [DefaultValue("DrillOlapGrid")]
        public string Drilldown
        {
            get { return this.drilldown; }
            set { this.drilldown = value; }
        }
        #endregion
    }
    public class OlapGridServiceMethodsBuilder
    {
        private OlapGridServiceMethods serviceMethods = new OlapGridServiceMethods();
        public OlapGridServiceMethodsBuilder(OlapGridServiceMethods serviceMethods)
        {
            this.serviceMethods = serviceMethods;
        }
        public OlapGridServiceMethodsBuilder Initialize(string initialize)
        {
            this.serviceMethods.Initialize = initialize;
            return this;
        }
        public OlapGridServiceMethodsBuilder Drilldown(string drilldown)
        {
            this.serviceMethods.Drilldown = drilldown;
            return this;
        }
    }
}
