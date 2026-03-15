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
using System.ComponentModel;
using System.Reflection;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript;

namespace Syncfusion.JavaScript.Models
{
   public class ProgressBarProperties
   {
       #region Fields
       //Int Values
        private int  min=0;
        private int max=100;
        private int value=0;
        private int percentage=0;

        //Boolean Values
        private bool enabled=true;
        private bool rtl=false;
        private bool persist=false;

        //String values
        private String width = "";
        private String height = "";
        private String text="";
        private String cssClass="";

        //Events 
        private String create = null;
        private String start = null;
        private String complete = null;
        private String change = null;
        private String destroy = null;

        private ProgressBar progress = new ProgressBar();
       #endregion
        public ProgressBarProperties() {  }
        //public ProgressBarProperties(String id, ProgressBar progress)
        //{
        //    progress.ID = id;
        //    this.progress = progress;
        //}

        #region Properties
        //int
        [JsonProperty("min")]
        [DefaultValue(0)]
        public int Min
        {
            get { return this.min; }
            set { this.min = value; }
        }
        [JsonProperty("max")]
        [DefaultValue(100)]
        public int Max
        {
            get { return this.max; }
            set { this.max = value; }
        }
        [JsonProperty("value")]
        [DefaultValue(0)]
        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        [JsonProperty("percentage")]
        [DefaultValue(0)]
        public int Percentage
        {
            get { return this.percentage; }
            set { this.percentage = value; }
        }
        //Boolean Properties
        [JsonProperty("enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }
        [JsonProperty("rtl")]
        [DefaultValue(false)]
        public bool Rtl
        {
            get { return this.rtl; }
            set { this.rtl = value; }
        }
        [JsonProperty("persist")]
        [DefaultValue(false)]
        public bool Persist
        {
            get { return this.persist; }
            set { this.persist = value; }
        }
        //String properties
        [JsonProperty("cssClass")]
        [DefaultValue("")]
        public String CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }
        [JsonProperty("text")]
        [DefaultValue(null)]
        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
       
        [JsonProperty("height")]
        [DefaultValue("")]
        public String Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        [JsonProperty("width")]
        [DefaultValue("")]
        public String Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
       
        //Events 
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
        }
        [JsonProperty("start")]
        [DefaultValue(null)]
        public String Start
        {
            get { return this.start; }
            set { this.start = value; }
        }
        [JsonProperty("complete")]
        [DefaultValue(null)]
        public String Complete
        {
            get { return this.complete; }
            set { this.complete = value; }
        }
        [JsonProperty("change")]
        [DefaultValue(null)]
        public String Change
        {
            get { return this.change; }
            set { this.change = value; }
        }
        [JsonProperty("destroy")]
        [DefaultValue(null)]
        public String Destroy
        {
            get { return this.destroy; }
            set { this.destroy = value; }
        }
        #endregion
   }
}

