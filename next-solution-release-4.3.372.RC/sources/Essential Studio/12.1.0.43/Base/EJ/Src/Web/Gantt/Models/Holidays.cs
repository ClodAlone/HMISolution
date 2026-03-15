#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Shared;

namespace Syncfusion.JavaScript.Models
{
    public class Holidays
    {
        private String day = null;
        private string label = null;
        private string background = null;

        [JsonProperty("day")]
        [DefaultValue(null)]
        public String Day
        {
            get { return this.day; }
            set { this.day = value; }
        }
        [JsonProperty("label")]
        [DefaultValue(null)]
        public string Label
        {
            get { return this.label; }
            set { this.label = value; }
        }

        [JsonProperty("background")]
        [DefaultValue(null)]
        public String Background
        {
            get { return this.background; }
            set { this.background = value; }
        }
    }
}