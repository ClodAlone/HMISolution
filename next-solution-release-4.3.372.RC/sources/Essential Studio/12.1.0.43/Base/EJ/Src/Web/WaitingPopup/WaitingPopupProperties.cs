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
    public class WaitingPopupProperties
    {
        #region Fields
        //Boolean Values
        private bool autoDisplay = false;
        private bool showImage = true;

        //String Values
        private String text = null;
        private String cssClass = "";
        private String template = null;

        //Events 
        private String create = null;
        private String destroy = null;

        //WaitingPopUp 
        private WaitingPopup waitingPopUp = new WaitingPopup();
        #endregion
        public WaitingPopupProperties() { }
        //public WaitingPopupProperties(String id, WaitingPopup waitingPopUp)
        //{
        //    waitingPopUp.ID = id;
        //    this.waitingPopUp = waitingPopUp;
        //}

        #region Properties
        //Boolean values
        [JsonProperty("autoDisplay")]
        [DefaultValue(false)]
        public bool AutoDisplay
        {
            get { return this.autoDisplay; }
            set { this.autoDisplay = value; }
        }

        [JsonProperty("showImage")]
        [DefaultValue(true)]
        public bool ShowImage
        {
            get { return this.showImage; }
            set { this.showImage = value; }
        }
        //String values
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
        [JsonProperty("template")]
        [DefaultValue(null)]
        public String Template
        {
            get { return this.template; }
            set { this.template = value; }
        }
        //Events 
        [JsonProperty("create")]
        [DefaultValue(null)]
        public String Create
        {
            get { return this.create; }
            set { this.create = value; }
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
