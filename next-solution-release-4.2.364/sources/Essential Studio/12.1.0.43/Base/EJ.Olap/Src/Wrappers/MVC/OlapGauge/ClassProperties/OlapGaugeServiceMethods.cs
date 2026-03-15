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
using Syncfusion.JavaScript.Shared.Serializer;
using System.ComponentModel;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapGaugeServiceMethods
    {
        #region PrivateVariables
        private string initialize = "InitializeOlapGauge";
        #endregion
        #region Properties
        [JsonProperty("initialize")]
        [DefaultValue("InitializeOlapGauge")]
        public String Initialize
        {
            get { return this.initialize; }
            set { this.initialize = value; }
        }
        #endregion
    }
    public class OlapGaugeServiceMethodsBuilder
    {
        private OlapGaugeServiceMethods serviceMethods = new OlapGaugeServiceMethods();
        public OlapGaugeServiceMethodsBuilder(OlapGaugeServiceMethods serviceMethods)
        {
            this.serviceMethods = serviceMethods;
        }
        public OlapGaugeServiceMethodsBuilder Initialize(string initialize)
        {
            this.serviceMethods.Initialize = initialize;
            return this;
        }
    }
}
