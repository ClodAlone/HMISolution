#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Shared.Serializer;


namespace Syncfusion.JavaScript.Models
{
    public class ScrollerProperties
    {
	
        #region Fields

        //Boolean Values
        private bool persist = false;        
       
        //Integer Values
        private int height = 250;
        private int width = 0;
        private int oneStep = 57;
        private int buttonSize = 18;
        private int scrollLeft = 0;
        private int scrollTop = 0;
        private int scrollerSize = 18;

        //Events 
        private String rtl = "";       

        //Button 
        private Scroller scroller = new Scroller();

        #endregion
        #region Properties
        public ScrollerProperties() {  }
        //public ScrollerProperties(String id, Scroller scroller)
        //{
        //    scroller.ID = id;
        //    this.scroller = scroller;
        //}

        //Integer values
         [JsonProperty("height")]
         [DefaultValue(250)]
         public int Height
         {
             get { return this.height; }
             set { this.height = value; }
         }
         [JsonProperty("width")]
         [DefaultValue(0)]
         public int Width
         {
             get { return this.width; }
             set { this.width = value; }
         }
         [JsonProperty("oneStep")]
         [DefaultValue(57)]
         public int OneStep
         {
             get { return this.oneStep; }
             set { this.oneStep = value; }
         }
		 //Integer values
         [JsonProperty("buttonSize")]
         [DefaultValue(18)]
         public int ButtonSize
         {
             get { return this.buttonSize; }
             set { this.buttonSize = value; }
         }
         [JsonProperty("scrollTop")]
         [DefaultValue(0)]
         public int ScrollTop
         {
             get { return this.scrollTop; }
             set { this.scrollTop = value; }
         }
         [JsonProperty("scrollLeft")]
         [DefaultValue(0)]
         public int ScrollLeft
         {
             get { return this.scrollLeft; }
             set { this.scrollLeft = value; }
         }
         [JsonProperty("scrollerSize")]
         [DefaultValue(18)]
         public int ScrollerSize
         {
             get { return this.scrollerSize; }
             set { this.scrollerSize = value; }
         }
        //string values
        [JsonProperty("rtl")]
        [DefaultValue("")]
        public String Rtl
        {
            get { return this.rtl; }
            set { this.rtl = value; }
        }
        //boolean values
		[JsonProperty("persist")]
         [DefaultValue(false)]
         public bool Persist
         {
             get { return this.persist; }
             set { this.persist = value; }
         }
        #endregion
        #region ShouldSerialize Methods
       
        #endregion
     
    }
}
